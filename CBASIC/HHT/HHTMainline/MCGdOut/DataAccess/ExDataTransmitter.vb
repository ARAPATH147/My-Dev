Imports System.IO
Imports System.Text
Imports System.Runtime.InteropServices
#If RF Then
Imports System.Net
Imports System.Net.Sockets
#End If

'''****************************************************************************
''' <FileName>ExDataTransmitter.vb</FileName>
''' <summary>
''' Used for transmitting the export data present in the MC70 device to the 
''' EPOS controller.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>08-Dec-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''****************************************************************************
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Connects to alternate controller while primary is down
''' </Summary>
'''****************************************************************************
Public Class ExDataTransmitter
    Private m_ExDatFilePath As String = Nothing
    Private aExDataRecords() As String = Nothing
#If NRF Then
    Private m_SockectConnMgr As SocketConnectionMgr = Nothing
#ElseIf RF Then
    Private m_rfSocketManager As RFSocketManager
    Private m_socketSentText As String
    Private m_socketReceivedText As String
    Private m_TransactMessageParser As TransactMessageParser
    Private m_connector As frmConnector
    Private Delegate Sub m_Invoker(ByVal Connection_Message As String, ByVal attempt As String)
    Private status As DATAPOOL.ConnectionStatus
    Private m_status As DATAPOOL.ConnectionStatus
    Private m_WaitTimeBeforeReconnect As Integer
    Private bCancelReconnect As Boolean = False
    Private bConnectionCompleted As Boolean = False
    Private CurrentStamp As Date
    Private Delegate Sub UpdateStatusCallback(ByVal Connectivity_Message As String, ByVal RetryAttempt As Integer)
#End If
    Private bAlternate As Boolean = False ' v1.1 MCF Change
    Private m_ControllerDateTime As String = Nothing
    Private m_Retry As Integer = 0
    Private m_ListNum As String = Nothing
    Private m_UODNumber As String = Nothing
    'System time structure used to set system time.
    Public Structure SYSTEMTIME
        Public wYear As Short
        Public wMonth As Short
        Public wDayOfWeek As Short
        Public wDay As Short
        Public wHour As Short
        Public wMinute As Short
        Public wSecond As Short
        Public wMilliseconds As Short
    End Structure
    'To get system time.
    <DllImport("coredll.dll")> _
    Public Shared Sub GetSystemTime(ByRef lpSystemTime As SYSTEMTIME)
    End Sub
    'P/Invoke dec for setting the system time
    <DllImport("coredll.dll")> _
    Private Shared Function SetLocalTime(ByRef time As SYSTEMTIME) As Boolean
    End Function
    Public Sub EndSession()
#If NRF Then
        If Not m_SockectConnMgr is Nothing then
          m_SockectConnMgr.TerminateConnection()
          m_SockectConnMgr = Nothing 
        End if
#ElseIf RF Then
        If Not m_connector Is Nothing Then
            m_connector.Close()
            m_connector.Dispose()
            m_connector = Nothing
        End If
        If Not m_rfSocketManager Is Nothing Then
            m_rfSocketManager.Disconnect()
            m_rfSocketManager = Nothing
        End If
        m_TransactMessageParser = Nothing
#End If
    End Sub
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        'Get export data file name.
        m_ExDatFilePath = ConfigDataMgr.GetInstance(). _
                          GetParam(ConfigKey.EXPORT_FILE_PATH).ToString()
        'Get retry attempt to send export data.
        m_Retry = Macros.WRITE_RETRY
#If NRF Then
          'Initialise the socket connection.
        m_SockectConnMgr = New SocketConnectionMgr()
#End If
#If RF Then
        m_WaitTimeBeforeReconnect = Convert.ToInt16(ConfigDataMgr.GetInstance().GetParam(ConfigKey.WAIT_TIME_BEFORE_RECONNECT).ToString)
        bAlternate = False
        m_connector = New frmConnector()
        'm_rfSocketManager = New RFSocketManager(ConfigDataMgr.GetInstance(). _
        '                GetParam(ConfigKey.SERVER_IPADDRESS).ToString(), ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT))
        m_rfSocketManager = New RFSocketManager(objAppContainer.strActiveIP, ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT))
        'Initialise Socket connection for RF
        ConnectToSocket()
        If DATAPOOL.getInstance.WaitForConnection() = DATAPOOL.ConnectionStatus.Disconnected Then
            'v1.1 MCF Change
            If objAppContainer.bMCFEnabled Then
                frmSplashScreen.Enabled = False
                frmSplashScreen.Visible = False
                If fConnectAlternateInRF() Then
                    frmSplashScreen.Visible = True
                    frmSplashScreen.Enabled = True
                    m_rfSocketManager = Nothing
                    bAlternate = True
                    DATAPOOL.getInstance.EndSession()
                    objAppContainer.objSplashScreen.ChangeLabelText("Connecting to Alternate controller...")
                    m_rfSocketManager = New RFSocketManager(objAppContainer.strActiveIP, + _
                                                            ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT))
                    ConnectToSocket()
                    m_status = DATAPOOL.getInstance.WaitForConnection
                    If m_status = DATAPOOL.ConnectionStatus.Disconnected Then
                        bAlternate = False
                    End If
                    If m_status = DATAPOOL.ConnectionStatus.Connected Then
                        ConfigDataMgr.GetInstance.SetActiveIP()
                    End If
                Else
                    frmSplashScreen.Enabled = True
                End If
            End If
        Else
            bAlternate = True
        End If
        If Not bAlternate Then
            MessageBox.Show(MessageManager.GetInstance.GetMessage("M64"), _
                                "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
            Throw New Exception(Macros.CONNECTION_LOSS_EXCEPTION)
        End If
        objAppContainer.objLogger.WriteAppLog("Initialising Parser")
        'Initialising Parser
        m_TransactMessageParser = New TransactMessageParser()
#End If
    End Sub

 
#Region "NRF - DownloadExData"
#If NRF Then
    ''' <summary>
    ''' Download export data record from device to the controller.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DownloadExData() As String
        Dim iCount As Integer = 0
        Dim m_Line As String = Nothing
        Dim m_Status As Boolean = Nothing
        Dim m_TempLine1 As String = Nothing
        Dim m_TempLine2 As String = Nothing
        Dim m_Ip As String = Nothing
        Try
            'Read export data file contents to an array.
            m_Status = ReadFileToArray( _
                            m_ExDatFilePath & "/" & Macros.EXPORT_FILE_NAME)
            If m_Status And aExDataRecords.Length > 3 Then
                'Read the export data content till the end of the array list.
                While iCount < aExDataRecords.Length And _
                        aExDataRecords(iCount).Trim() <> Nothing
                    m_Line = aExDataRecords(iCount)
                    'Replace all commas in a line with no space.
                    m_Line = m_Line.Replace(",", "")
                    If m_Line.StartsWith("UOS") Then
                        'Get the List ID from the response and use it for the
                        'UOA messages to be sent.
                        If Not SendRecord(m_Line) Or m_ListNum = Nothing Then
                            'Add the exception to the device log.
                            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                        "Error in sending export data to controller:" _
                                        & "List ID not received for GAS message.", _
                                        Logger.LogLevel.RELEASE)
                            'Update the file status
                            objAppContainer.objHelper.UpdateFileStatus("McGdOut_ExData", "F", _
                                                                       "Failed to send record: " & _
                                                                       m_Line)
                            'Failed to send the record or receive a list ID.
                            Return "-1"
                        End If
                        'Pad the list number to length 4
                        'm_ListNum = m_ListNum.PadLeft(4, "0")
                        'Send all the records until UOX record is read.
                        iCount = iCount + 1
                        m_Line = aExDataRecords(iCount).ToString()
                        Do Until m_Line.StartsWith("UOX")
                            'Insert the list ID in this place.
                            'Substitute the List ID in UOA record to be sent.
                            m_Line = m_Line.Replace(",", "")
                            'Insert the list ID in this place.
                            'Substitute the List ID in UOA record to be sent.
                            If m_ListNum <> Nothing And m_Line.StartsWith("UOA") Then
                                '<Manu:25Mar09> Change for Goods Out UOA issue, updated parse index 
                                m_TempLine1 = m_Line.Substring(0, 6)
                                m_TempLine2 = m_Line.Substring(10)
                                m_Line = m_TempLine1 + m_ListNum + m_TempLine2
                            End If
                            'Return false if error occurred while transmitting.
                            If Not SendRecord(m_Line) Then
                                'Add the exception to the device log.
                                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            "Error in sending export data to controller:" _
                                            & "Record" & m_Line & "not sent.", _
                                            Logger.LogLevel.RELEASE)
                                Return False
                            End If
                            'Increment iCount and read the next value
                            iCount = iCount + 1
                            m_Line = aExDataRecords(iCount).ToString()
                        Loop
                        'Substitute the List ID in UOA record to be sent.
                        m_Line = m_Line.Replace(",", "")
                        'Send STQ record and get a new UOD number.
                        If m_Line.StartsWith("UOX") Then
                            m_TempLine1 = m_Line.Substring(0, 6)
                            m_TempLine2 = m_Line.Substring(10)
                            m_Line = m_TempLine1 + m_ListNum + m_TempLine2
                            '<Manu:25Mar09> Not sure if this is required as i cant find a reason why this should be done. 
                            'Not changing for fear of regression. 
                            If m_UODNumber <> Nothing Then
                                m_TempLine1 = m_Line.Substring(0, 11)
                                m_TempLine2 = m_Line.Substring(25)
                                m_Line = m_TempLine1 + m_UODNumber + m_TempLine2
                            End If
                        End If
                        'Return false if error occurred while transmitting.
                        If Not SendRecord(m_Line) Then
                            'Add the exception to the device log.
                            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                        "Error in sending export data to controller:" _
                                        & "Record" & m_Line & "not sent.", _
                                        Logger.LogLevel.RELEASE)
                            'Update the file status
                            objAppContainer.objHelper.UpdateFileStatus("McGdOut_ExData", "F", _
                                                                       "Failed to send record: " & _
                                                                       m_Line)
                            Return "-1"
                        End If
                    ElseIf m_Line.StartsWith("SOR") Then
                        'Check for the IP address in the record.
                        'If Mid(m_Line, 28, 15) = "127.000.000.001" Then
                        '    m_TempLine1 = Mid(m_Line, 0, 30)
                        '    m_TempLine2 = Mid(m_Line, 46)
                        '    m_Ip = objAppContainer.objHelper.GetIPAddress()
                        '    m_Line = m_TempLine1 + m_Ip + m_TempLine2
                        'End If
                        If Mid(m_Line, 28, 15) = "127.000.000.001" Then
                            'Fixed in integration testing
                            m_TempLine1 = Mid(m_Line, 1, 27)
                            m_TempLine2 = Mid(m_Line, 44)
                            m_Ip = objAppContainer.objHelper.GetIPAddress()
                            m_Line = m_TempLine1 + m_Ip + m_TempLine2
                        End If
                        'Now send the record.
                        If Not SendRecord(m_Line) Then
                            'Add the exception to the device log.
                            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                        "Error in sending export data to controller:" _
                                        & "Record" & m_Line & "not sent.", _
                                        Logger.LogLevel.RELEASE)
                            'Update the file status
                            objAppContainer.objHelper.UpdateFileStatus("McGdOut_ExData", "F", _
                                                                       "Failed to send record: " & _
                                                                       m_Line)
                            Return "-1"
                        End If
                    Else
                        'Send export data record for Recall
                        If Not SendRecord(m_Line) Then
                            'Add the exception to the device log.
                            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                        "Error in sending export data to controller:" _
                                        & "Message" & m_Line & "not sent.", _
                                        Logger.LogLevel.RELEASE)
                            'Update the file status
                            objAppContainer.objHelper.UpdateFileStatus("McGdOut_ExData", "F", _
                                                                       "Failed to send record: " & _
                                                                       m_Line)
                            Return "-1"
                        End If
                    End If
                    'Increment the counter variable.
                    iCount = iCount + 1
                    m_Line = Nothing
                    m_Status = Nothing
                End While 'End of first while
            ElseIf aExDataRecords.Length = 3 Then
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                        "ExDataTransmitter:" _
                                        & "No data available in the export data file.", _
                                        Logger.LogLevel.RELEASE)
                'delete export data file as no export data available
                'except SOR and OFF
                File.Delete(m_ExDatFilePath & "/" & Macros.EXPORT_FILE_NAME)
                ''Update the file status
                objAppContainer.objHelper.UpdateFileStatus("McGdOut_ExData", "F", _
                                                           "No export data available")
                'return false to the calling function
                Return "0"
            Else
                'Add the exception to the application log.
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                "ExDataTransmitter: No export data available in the file", _
                                Logger.LogLevel.RELEASE)
                'Update the file status
                objAppContainer.objHelper.UpdateFileStatus("McGdOut_ExData", "F", _
                                                           "No export data available")
                Return "-1"
            End If
            'Before returning the status, delete the export data file in the 
            'local device.
            File.Delete(m_ExDatFilePath & "/" & Macros.EXPORT_FILE_NAME)
            'Update the time in config file.
            'ConfigDataMgr.GetInstance.SetParam(ConfigKey.LAST_EXDATA_DOWNLOAD_TIME, _
            '                                   DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
        Catch ex As Exception
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                    "Error in sending export data:" _
                                    & ex.Message.ToString(), _
                                    Logger.LogLevel.RELEASE)
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus("McGdOut_ExData", "F", ex.Message)
            'return the status
            Return "-1"
        Finally
            aExDataRecords = Nothing
            'close the socket connection established with the controller.
            m_SockectConnMgr.TerminateConnection()
        End Try
        Try
            'Update the time in config file.
            ConfigDataMgr.GetInstance.SetParam(ConfigKey.LAST_EXDATA_DOWNLOAD_TIME, _
                                               DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus("McGdOut_ExData", "P", "NA")
        Catch ex As Exception
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                 "Error in sending export data:" _
                                 & ex.Message.ToString(), _
                                 Logger.LogLevel.RELEASE)
        End Try
        'return true if no error occured during the export data download.
        Return "1"
    End Function
#End If
#End Region

    ''' <summary>
    ''' Function to read the content of export data file to string array.
    ''' </summary>
    ''' <param name="m_FileName">File name with path</param>
    ''' <returns>Bool
    ''' True - If successfuly opened the file and read the contents.
    ''' False - If any error occurred and the file is not read.
    ''' </returns>
    ''' <remarks></remarks>
    Private Function ReadFileToArray(ByVal m_FileName As String) As Boolean
        Dim m_FileReader As StreamReader = Nothing
        Try
            m_FileReader = New StreamReader(m_FileName)
            'Read the export data file until SOR type record is read.
            aExDataRecords = Split(m_FileReader.ReadToEnd(), ControlChars.NewLine)
            'Close the stream reader.
            m_FileReader.Close()
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                    "Error in reading export data file:" _
                                    & ex.Message.ToString(), _
                                    Logger.LogLevel.RELEASE)
            Return False
        End Try
        'return the array.
        Return True
    End Function
#Region "NRF - Parse Response"
#If NRF Then
    ''' <summary>
    ''' To parse the resonse received from the TRANSACT service and return status
    ''' accordingly.
    ''' </summary>
    ''' <param name="m_ResponseMessage">Response message received from the 
    ''' TRANSACT service</param>
    ''' <returns>Bool
    ''' True - ACK, UOR and SNR is received.
    ''' False - NAK is received or any error occurred.
    ''' </returns>
    ''' <remarks></remarks>
    Private Function ParseResponse(ByVal m_ResponseMessage As String, ByVal strRecordSent As String) As Boolean
        m_ResponseMessage = NTrim(m_ResponseMessage)
        'new message format
        m_ResponseMessage = m_ResponseMessage.Substring(5, m_ResponseMessage.Length - 5)
        'Response received from the controller.
        objAppContainer.objLogger.WriteAppLog("ExDataTransmitter: Response received: " _
                                              & m_ResponseMessage, Logger.LogLevel.RELEASE)
        'Based on the message type parse the response and return the value.
        'Based on the message type parse the response and return the value.
        If m_ResponseMessage.StartsWith("ACK") Then
            'If Response received is ACK
            Return True
            'Based on the message type parse the response and return the value.
        ElseIf m_ResponseMessage.StartsWith("UOR") Then
            'If Response received is UOR
            m_ResponseMessage = m_ResponseMessage.Trim()
            Try
                m_ListNum = m_ResponseMessage.Substring(6, 4)
                Return True
            Catch ex As Exception
                'Add the exception to the application log.
                objAppContainer.objLogger.WriteAppLog("ExDataTransmitter: Parse and get list ID" _
                                                & ex.Message.ToString(), _
                                                Logger.LogLevel.RELEASE)
                'return false on exception
                Return False
            End Try
        ElseIf m_ResponseMessage.StartsWith("STR") Then
            'If Response received is STR for STQ message
            m_ResponseMessage = m_ResponseMessage.Trim()
            Try
            'DEFECT - STQ parsing Issue
                m_UODNumber = m_ResponseMessage.Substring(3, 14)
               ' m_UODNumber = m_ResponseMessage.Substring(4, 14)
                Return True
            Catch ex As Exception
                'Add the exception to the application log.
                objAppContainer.objLogger.WriteAppLog("ExDataTransmitter: UOD number not present" _
                                                & ex.StackTrace, _
                                                Logger.LogLevel.RELEASE)
                'return false on exception
                Return False
            End Try
        ElseIf m_ResponseMessage.StartsWith("SNR") Then
            'set the device date time according to the date time received 
            'in the response.
            m_ControllerDateTime = m_ResponseMessage.Substring(22, 12)
            Return SetDeviceDateTime(m_ControllerDateTime)
        ElseIf m_ResponseMessage.StartsWith("NAK") Then
            If Not strRecordSent.StartsWith("OFF") Then
                Dim strNakMessage As String = ""
                strNakMessage = m_ResponseMessage.Replace("NAK", "")    'Supress NAK String
                strNakMessage = strNakMessage.Replace("NAKERROR", "")   'Suppress NAKERROR string
                'Display the recevied NAK message to the user.
                MessageBox.Show("Received error from controller:" + strNakMessage, _
                                "Error", MessageBoxButtons.OK, _
                                MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                'Update the file status

                objAppContainer.objHelper.UpdateFileStatus("McGdOut_ExData", "F", strNakMessage)

                'If response received is NAK
                Return False
            Else
                Return True
            End If
        End If
        'Add the exception to the application log.
        objAppContainer.objLogger.WriteAppLog("ExDataTransmitter: Response " _
                                              & "received is not expected." + _
                                              m_ResponseMessage, _
                                              Logger.LogLevel.RELEASE)
        Return False
    End Function
#End If
#End Region

    ''' <summary>
    ''' Truncates a string at the first occurrence of the null character.
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function NTrim(ByVal Text As String) As String
        Dim Pos As Long
        Try
            Pos = InStr(Text, vbNullChar)

            If Pos > 0 Then
                NTrim = Left$(Text, Pos - 1)
            Else
                NTrim = Text
            End If
        Catch ex As Exception
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                    "Error in sending export data to controller:" _
                                    & ex.Message.ToString(), _
                                    Logger.LogLevel.RELEASE)
            NTrim = Text
        End Try
    End Function
#If NRF Then
    ''' <summary>
    ''' Convert the record to bytes and send to the TRANSACT service.
    ''' Receive the response and parse it to get the details.
    ''' </summary>
    ''' <param name="strRecord">Record to be sent to the TRANSACT</param>
    ''' <returns> Bool
    ''' True - if successfully sent and received the records.
    ''' False - is any error occurred during send / receive operation.
    ''' </returns>
    ''' <remarks></remarks>
    Private Function SendRecord(ByVal strRecord As String) As Boolean
        Dim m_SendBytes As [Byte]() = Nothing
        Dim m_ReadBytes As [Byte]() = Nothing
        Dim m_Status As Boolean = Nothing
        Dim m_RetryWrite As Integer = 0
        Try
            'Records sent ot the controller.
            objAppContainer.objLogger.WriteAppLog("ExDataTransmitter: Record sent: " _
                                                  & strRecord, Logger.LogLevel.RELEASE)
            ' m_SendBytes = Encoding.ASCII.GetBytes(strRecord)
            Dim iLength As Integer = strRecord.Length
            'new message format
            ' strRecord = strRecord.Substring(0, strRecord.Length - 2)
            strRecord = Chr(255) + (strRecord.Length + 5).ToString.PadLeft(4, "0") + strRecord
            m_SendBytes = Encoding.ASCII.GetBytes(strRecord.ToString())
            m_SendBytes(0) = &HFF
            m_RetryWrite = m_Retry
            'Send the record to the controller.
            Do
                If m_SockectConnMgr.TransmitData(m_SendBytes) Then
                    'Read the response stream from the client.
                    If m_SockectConnMgr.ReadData(m_ReadBytes) And _
                       m_ReadBytes.Length > 0 Then
                        'Return the response after parsing it.
                        Return ParseResponse( _
                                    Encoding.ASCII.GetString(m_ReadBytes, _
                                                             0, _
                                                             m_ReadBytes.Length), strRecord)
                    Else
                        'Add the exception to the application log.
                        objAppContainer.objLogger.WriteAppLog("ExDataTransmitter: Cannot " _
                                                              & "read from socket.", _
                                                              Logger.LogLevel.RELEASE)
                        'If reading response from the controller is failed.
                        Return False
                    End If
                End If
                m_RetryWrite = m_RetryWrite - 1
            Loop Until m_RetryWrite = 0
            'If all the write attempt failed.
            If m_RetryWrite = 0 Then
                'Add the exception to the application log.
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                                "Error sending data to controller", _
                                                Logger.LogLevel.RELEASE)
                Return False
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'incase of exception return false.
            Return False
        Finally
            m_SendBytes = Nothing
            m_ReadBytes = Nothing
        End Try
    End Function

    ''' <summary>
    ''' Convert ALR record to bytes and send to the TRANSACT service.
    ''' Receive the response and send the details to the calling function.
    ''' </summary>
    ''' <returns> String
    ''' Message retured by controller in strin format.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function SendALR(ByVal strUserID As String) As String
        Dim strRecord As StringBuilder = Nothing
        Dim strOFFRecord As StringBuilder = Nothing
        Dim m_SendBytes As [Byte]() = Nothing
        Dim m_ReadBytes As [Byte]() = Nothing
        Dim m_Status As Boolean = Nothing
        Dim strResponse As String = Nothing
        Dim m_RetryWrite As Integer = 0
        Dim m_ACK1 As String = Nothing
        Dim m_ACK2 As String = Nothing
        Dim strData As String = Nothing


        Try
            m_ACK1 = ConfigDataMgr.GetInstance.GetParam( _
                                            ConfigKey.ACK_1).ToString()
            m_ACK2 = ConfigDataMgr.GetInstance.GetParam( _
                                            ConfigKey.ACK_2).ToString()
            'Generate record for OFF
            strOFFRecord = New StringBuilder()
            strOFFRecord.Append("OFF")
            strOFFRecord.Append(strUserID)
            strOFFRecord.Append(Environment.NewLine)

            'GEnerate record for ALR
            strRecord = New StringBuilder()
            strRecord.Append("ALR")
            strRecord.Append(strUserID)
            strRecord.Append("ACTBUILD")
            strData = strRecord.ToString()
            strRecord.Append(Environment.NewLine)
            'new message format
            ' m_SendBytes = Encoding.ASCII.GetBytes(strRecord.ToString())


            strData = Chr(255) + (strData.Length + 5).ToString.PadLeft(4, "0") + strData
            m_SendBytes = Encoding.ASCII.GetBytes(strData.ToString())
            m_SendBytes(0) = &HFF
            m_RetryWrite = m_Retry
            'Send the record to the controller.
            'Records sent ot the controller.
            objAppContainer.objLogger.WriteAppLog("ExDataTransmitter: Record sent: " _
                                                  & strRecord.ToString(), Logger.LogLevel.INFO)
            Do
                If m_SockectConnMgr.TransmitData(m_SendBytes) Then
                    'Read the response stream from the client.
                    If m_SockectConnMgr.ReadData(m_ReadBytes) And _
                       m_ReadBytes.Length > 0 Then
                        'Return the response after parsing it.
                        strResponse = Encoding.ASCII.GetString(m_ReadBytes, _
                                                        0, _
                                                        m_ReadBytes.Length)
                        'remove the null characters present at the end.
                        strResponse = NTrim(strResponse)
                        'trim the spaces in the trailing end
                        strResponse = strResponse.Trim()
                        'new message format 
                        strResponse = strResponse.Substring(5, strResponse.Length - 5)
                        'Response received from the controller.
                        objAppContainer.objLogger.WriteAppLog("ExDataTransmitter: Response received: " _
                                                              & strResponse, Logger.LogLevel.INFO)
                        If strResponse.StartsWith(m_ACK1) Then
                            Return "ACK1"
                        ElseIf strResponse.StartsWith(m_ACK2) Then
                            Return "ACK2"
                        ElseIf strResponse.StartsWith("NAK") Then
                            strResponse = strResponse.Replace("NAK", "")
                            'Display the recevied NAK message to the user.
                            MessageBox.Show("Received NAK :" + strResponse, _
                                            "NAK Received", MessageBoxButtons.OK, _
                                            MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                            Return "NAK"
                        End If

                    Else
                        'If the device does not read data successfully
                        objAppContainer.objLogger.WriteAppLog("ExDataTransmitter: Data " _
                                                              & "receive failure", _
                                                              Logger.LogLevel.RELEASE)
                        'If reading response from the controller is failed.
                        Return "NAK"
                    End If
                End If
                m_RetryWrite = m_RetryWrite - 1
            Loop Until m_RetryWrite = 0

            If m_RetryWrite = 0 Then
                'Add the exception to the application log.
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                                "Error sending data to controller", _
                                                Logger.LogLevel.RELEASE)
                Return "NAK"
            End If
            'Send OFF record and close the transaction.
            SendRecord(strOFFRecord.ToString())

        Catch ex As Exception
            'Add the exception to the application log.
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
            'Close the socket connection to the controller.
            m_SockectConnMgr.TerminateConnection()
            'incase of exception return false.
            Return "NAK"
        Finally
            'Clear the variable memories.
            m_SendBytes = Nothing
            m_ReadBytes = Nothing
            'Close the socket connection to the controller.
            m_SockectConnMgr.TerminateConnection()
        End Try

    End Function
#End If


    ''' <summary>
    ''' Gets and return the status of the socket connection established
    ''' with the controller.
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function GetSocketStatus() As Boolean
#If NRF Then
        Return m_SockectConnMgr.ConnectionStatus
#End If
    End Function
    ''' <summary>
    ''' To send SOR record before sending ALR record.
    ''' </summary>
    ''' <param name="strRecords"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendSOR(ByVal strRecords As String, ByRef strDateTime As String) As Boolean
        strRecords = strRecords.Replace(",", "")
#If NRF Then
        If SendRecord(strRecords) Then
            'Set controller date time
            strDateTime = m_ControllerDateTime
            'insert parsing characters accordingly
            strDateTime = strDateTime.Insert(4, "-")
            strDateTime = strDateTime.Insert(7, "-")
            strDateTime = strDateTime.Insert(10, " ")
            strDateTime = strDateTime.Insert(13, ":")
            strDateTime = strDateTime.Insert(16, ":")
            strDateTime = strDateTime & "00"
            'return the status
            Return True
        Else
            Return False
        End If
#End If
    End Function
    ''' <summary>
    ''' To set device time to same as controller time.
    ''' </summary>
    ''' <param name="strDateTime">Datetime string recevived from controller</param>
    ''' <returns>
    ''' True - If successfully set the device time.
    ''' False - If error in setting the device time.
    ''' </returns>
    ''' <remarks></remarks>
    Private Function SetDeviceDateTime(ByVal strDateTime As String) As Boolean
        Dim objSysTime As SYSTEMTIME
        'Get the device time.
        GetSystemTime(objSysTime)
        'Populate structure to update the table.
        With objSysTime
            .wYear = Convert.ToUInt16(strDateTime.Substring(0, 4))
            .wMonth = Convert.ToUInt16(strDateTime.Substring(4, 2))
            .wDay = Convert.ToUInt16(strDateTime.Substring(6, 2))
            .wHour = Convert.ToUInt16(strDateTime.Substring(8, 2))
            .wMinute = Convert.ToUInt16(strDateTime.Substring(10, 2))
            .wSecond = Convert.ToUInt16(0)
        End With

        'Set the new time`
        Return SetLocalTime(objSysTime)
    End Function

    '' These are RFSTAB codes - Writern by NPR
#Region "RF"
#If RF Then
    Public Function EstablishConnection(Optional ByVal ConnectionLostScenario As GOExportDataManager.ConnectionLostScenario = GOExportDataManager.ConnectionLostScenario.While_Others, Optional ByVal bIsReconnectingAfterDataTimeout As Boolean = False) As Boolean
        DATAPOOL.getInstance.isConnected = False
        objAppContainer.objLogger.WriteAppLog("EStablishing a New Connection", Logger.LogLevel.RELEASE)
        Dim isCancelled As Boolean = False
        If ModuleReconnect(isCancelled, ConnectionLostScenario, bIsReconnectingAfterDataTimeout) Then
            'Connected.
            'Send an RCN
            'Show Product Scan screen here
            m_connector.Hide()
            'Show the appropriate scan screen
            If Not (ConnectionLostScenario = GOExportDataManager.ConnectionLostScenario.While_Log_OFF Or _
                    ConnectionLostScenario = GOExportDataManager.ConnectionLostScenario.While_Login Or _
                        ConnectionLostScenario = GOExportDataManager.ConnectionLostScenario.While_Sending_Start_Record) Then
                Select Case objAppContainer.objActiveModule
                    Case AppContainer.ACTIVEMODULE.CRDCLM
                        'credit claim
                        CCSessionMgr.GetInstance().DisplayCCScreen(CCSessionMgr.CCSCREENS.Scan)
                        Exit Select
                    Case AppContainer.ACTIVEMODULE.CRTRCL
                        'create recall
                        GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.Scan)
                        Exit Select
                    Case AppContainer.ACTIVEMODULE.GDSOUT
                        'Goods Out
                        GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.Scan)
                        Exit Select
                    Case AppContainer.ACTIVEMODULE.GDSTFR
                        'goods out transfer
                        GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTransferMgr.GOTRANSFER.Scan)
                        Exit Select
                    Case AppContainer.ACTIVEMODULE.PHSLWT
                        'pharmacy special waste.
                        PSWSessionMgr.GetInstance().DisplayPSWScreen(PSWSessionMgr.PSWSCREENS.ScanUOD)
                        Exit Select
                    Case AppContainer.ACTIVEMODULE.RECALL
                        'create recall.
                        ' If ConnectionLostScenario <> GOExportDataManager.ConnectionLostScenario.While_Retrieving_the_List Then
                        If ConnectionLostScenario <> GOExportDataManager.ConnectionLostScenario.While_Retrieving_the_List And _
                         ConnectionLostScenario <> GOExportDataManager.ConnectionLostScenario.While_Retreiving_Act_Recall_List Then
                            If Not objAppContainer.bIsActiveRecallListSCreen Then
                                RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Scan)
                            End If

                        End If
                        Exit Select
                    Case Else
                        Exit Select
                End Select
            End If
            Return True
        Else
            '******************Time out handle*********************************
            'Incase of time out don't close any application if it fails to reconnect.
            'Just return the status as not connected so that the user can be
            'left in the same screen as they were and display the timeout message.
            If bIsReconnectingAfterDataTimeout Then
                Return False
            End If
            If objAppContainer.iConnectedToAlternate <> 0 Then
                m_connector.Visible = False
                If objAppContainer.iConnectedToAlternate = 1 Then
                    sCloseSession()
                End If
                Return False
            End If
            'If not cancelled show unable to reconnect message
            If Not isCancelled Then
                Dim bRecallModule As Boolean = False
                Dim bCreateRecall As Boolean = False
                '1.3. refers to the modules under Recalls
                'If WorkflowMgr.GetInstance.WFIndex.StartsWith("1.3.1") Or _
                '  WorkflowMgr.GetInstance.WFIndex.StartsWith("1.3.2") Or _
                '  WorkflowMgr.GetInstance.WFIndex.StartsWith("1.3.3") Then
                If WorkflowMgr.GetInstance.WFIndex.StartsWith("1.3.2") Or _
                   WorkflowMgr.GetInstance.WFIndex.StartsWith("1.3.3") Or _
                   WorkflowMgr.GetInstance.WFIndex.StartsWith("1.3.4") Or _
                   WorkflowMgr.GetInstance.WFIndex.StartsWith("1.3.5") Or _
                   WorkflowMgr.GetInstance.WFIndex.StartsWith("1.3.6") Or _
                   WorkflowMgr.GetInstance.WFIndex.StartsWith("1.3") Then

                    bRecallModule = True
                    'Set the variable is RCB sent to true so that after a reconnection failure
                    'RCA is sent before trying to fetch recall list using RCD
                    RLSessionMgr.GetInstance().bIsRCBSent = True
                ElseIf WorkflowMgr.GetInstance.WFIndex.StartsWith("1.3.7") Then
                    bCreateRecall = True
                End If
                'Check whether this is a inclusive list
                Dim ReconnectMessage As String = ""
                Select Case ConnectionLostScenario
                    Case GOExportDataManager.ConnectionLostScenario.While_Sending_End_Record
                        If bRecallModule Then
                            ReconnectMessage = "Unable to regain connectivity. " + _
                                            "Your list has been saved. " + _
                                            "Select OK to continue. Reselect the same recall " + _
                                            "once reconnected to complete the recall."
                        ElseIf bCreateRecall Then
                            ReconnectMessage = "Unable to regain connectivity. " + _
                                            "Your list has been saved. " + _
                                            "Select OK to continue. Reselect Create Recall " + _
                                            "once reconnected to complete the recall."
                        Else
                            ReconnectMessage = "Unable to regain connectivity. " + _
                                            "Your list has been saved. " + _
                                            "Select OK to continue. Reselect the same option " + _
                                            "once reconnected to complete the claim."
                        End If
                    Case GOExportDataManager.ConnectionLostScenario.While_Others
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                            "The last item may not be saved. " + _
                                            "Select OK to continue."
                    Case GOExportDataManager.ConnectionLostScenario.While_Sending_Item_Add
                        If bRecallModule Then
                            ReconnectMessage = "Unable to regain connectivity. " + _
                                            "The last item may not be saved. " + _
                                            "Select OK to continue. Reselect the same recall " + _
                                            "once reconnected to complete the recall."
                        ElseIf bCreateRecall Then
                            ReconnectMessage = "Unable to regain connectivity. " + _
                                            "Your list has been saved. " + _
                                            "Select OK to continue. Reselect Create Recall " + _
                                            "once reconnected to complete the recall."
                        Else
                            ReconnectMessage = "Unable to regain connectivity. " + _
                                            "The last item may not be saved. " + _
                                            "Select OK to continue. Reselect the same option " + _
                                            "once reconnected to complete the claim."
                        End If
                    Case GOExportDataManager.ConnectionLostScenario.While_Sending_Void
                        If bRecallModule Then
                            ReconnectMessage = "Unable to regain connectivity. " + _
                                            "The last void item may not be saved. " + _
                                            "Select OK to continue. Reselect the same recall " + _
                                            "once reconnected to complete the recall."
                        ElseIf bCreateRecall Then
                            ReconnectMessage = "Unable to regain connectivity. " + _
                                            "Your list has been saved. " + _
                                            "Select OK to continue. Reselect Create Recall " + _
                                            "once reconnected to complete the recall."
                        Else
                            ReconnectMessage = "Unable to regain connectivity. " + _
                                            "The last void item may not be saved. " + _
                                            "Select OK to continue. Reselect the same option " + _
                                            "once reconnected to complete the claim."
                        End If
                    Case GOExportDataManager.ConnectionLostScenario.While_Scanning_UOD_Label
                        If bRecallModule Then
                            ReconnectMessage = "Unable to regain connectivity. " + _
                                            "Your list has been saved. " + _
                                            "Select OK to continue. Reselect the same recall " + _
                                            "once reconnected to complete the recall."
                        ElseIf bCreateRecall Then
                            ReconnectMessage = "Unable to regain connectivity. " + _
                                            "Your list has been saved. " + _
                                            "Select OK to continue. Reselect Create Recall " + _
                                            "once reconnected to complete the recall."
                        Else
                            ReconnectMessage = "Unable to regain connectivity. " + _
                                            "Your list has been saved. " + _
                                            "Select OK to continue. Reselect the same option " + _
                                            "once reconnected to complete the claim."
                        End If
                    Case Else
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                            "Select OK to continue."
                End Select

                m_connector.lblMessage.Text = ReconnectMessage
                m_connector.btnCancel.Visible = False
                m_connector.btnCancel.Enabled = False
                m_connector.btnOK.Visible = True
                m_connector.btnOK.Enabled = True
                Cursor.Current = Cursors.Default
                m_connector.Location = New System.Drawing.Point(7, 65)
                m_connector.Refresh()
                m_connector.ShowDialog()
                m_connector.Hide()
            Else
                'Hide the connector if cancel clicked
                m_connector.Hide()
            End If
            If ConnectionLostScenario <> GOExportDataManager.ConnectionLostScenario.While_Log_OFF Or _
            ConnectionLostScenario <> GOExportDataManager.ConnectionLostScenario.While_Login Then
                If ConnectionLostScenario <> GOExportDataManager.ConnectionLostScenario.While_Sending_Start_Record Then
                    Select Case objAppContainer.objActiveModule
                        Case AppContainer.ACTIVEMODULE.CRDCLM
                            'credit claim
                            CCSessionMgr.GetInstance().EndSession(True)
                            Exit Select
                        Case AppContainer.ACTIVEMODULE.CRTRCL
                            'create recall
                            GOSessionMgr.GetInstance().EndSession(True)
                            Exit Select
                        Case AppContainer.ACTIVEMODULE.GDSOUT
                            'Goods Out
                            GOSessionMgr.GetInstance().EndSession(True)
                            Exit Select
                        Case AppContainer.ACTIVEMODULE.GDSTFR
                            'goods out transfer
                            GOTransferMgr.GetInstance().EndSession(True)
                            Exit Select
                        Case AppContainer.ACTIVEMODULE.PHSLWT
                            'pharmacy special waste.
                            PSWSessionMgr.GetInstance().EndSession()
                            Exit Select
                        Case AppContainer.ACTIVEMODULE.RECALL
                            'create recall.
                            RLSessionMgr.GetInstance().EndSession()
                            Exit Select
                        Case Else
                            Exit Select
                    End Select
                
                End If
                WorkflowMgr.GetInstance.Reset()
                'WorkflowMgr.GetInstance().ExecQuit()
            End If
#If RF Then
            objAppContainer.bIsActiveRecallListSCreen = False
#End If

            'Else
            '    Cursor.Current = Cursors.WaitCursor
            '    objAppContainer.bActiveform = False
            '    'Give some time for application to terminate
            '    Threading.Thread.Sleep(100)
            '    'Set cursor to default
            '    Cursor.Current = Cursors.Default
            '    'Exit the Application
            '    Application.Exit()
            'End If
            Return False
        End If
    End Function
    ''' <summary>
    ''' Function to perform the reconnect operation.
    ''' </summary>
    ''' <param name="isCancelled"></param>
    ''' <param name="ConnectionLostSceanrio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ModuleReconnect(ByRef isCancelled As Boolean, ByVal ConnectionLostSceanrio As GOExportDataManager.ConnectionLostScenario, Optional ByVal bIsReconnectingAfterDataTimeout As Boolean = False) As Boolean
        objAppContainer.objLogger.WriteAppLog("EStablishing a New Connection", Logger.LogLevel.RELEASE)
        m_connector.cancelled = False
        Dim bTemp As Boolean = False
        Dim strConnectivityMessage As String = Nothing
        Select Case ConnectionLostSceanrio
            Case GOExportDataManager.ConnectionLostScenario.While_Others
                strConnectivityMessage = Macros.CONNECTIVITY_MESSAGE_MIDDLE
            Case GOExportDataManager.ConnectionLostScenario.While_Sending_Start_Record
                strConnectivityMessage = Macros.CONNECTIVITY_MESSAGE_START
            Case GOExportDataManager.ConnectionLostScenario.While_Retrieving_the_List
                strConnectivityMessage = Macros.CONNECTIVITY_MESSAGE_START
            Case GOExportDataManager.ConnectionLostScenario.While_Sending_End_Record
                strConnectivityMessage = Macros.CONNECTIVITY_MESSAGE_END
            Case GOExportDataManager.ConnectionLostScenario.While_Log_OFF
                strConnectivityMessage = Macros.CONNECTIVITY_MESSAGE_START
            Case GOExportDataManager.ConnectionLostScenario.While_Login
                strConnectivityMessage = Macros.CONNECTIVITY_MESSAGE_START
            Case GOExportDataManager.ConnectionLostScenario.While_Sending_Item_Add
                strConnectivityMessage = Macros.CONNECTIVITY_ITEM_ADD
            Case GOExportDataManager.ConnectionLostScenario.While_Sending_Void
                strConnectivityMessage = Macros.CONNECTIVITY_ITEM_VOID
            Case GOExportDataManager.ConnectionLostScenario.While_Scanning_UOD_Label
                strConnectivityMessage = Macros.CONNECTIVITY_MESSAGE_END
            Case GOExportDataManager.ConnectionLostScenario.While_Retreiving_Act_Recall_List
                strConnectivityMessage = Macros.CONNECTIVITY_MESSAGE_START
            Case Else
                strConnectivityMessage = Macros.CONNECTIVITY_MESSAGE_START
        End Select
        'Dim m_ReconnectTime As Integer = 1
        'Display the form here
        'm_connector.Invoke(New EventHandler(AddressOf DisplayConnector))
        If bIsReconnectingAfterDataTimeout Then     'Timeout changes start
            m_connector.btnCancel.Visible = False   '  To avoid displaying cancel
            m_connector.btnCancel.Enabled = False   '  button in the message box
            strConnectivityMessage = Macros.TIMEOUT_RECONNECT_MESSAGE
        Else                                        '  displayed for reconnection
            m_connector.btnCancel.Visible = True    '  after a data time out has occurred
            m_connector.btnCancel.Enabled = True    '  by Govindh
        End If
        m_connector.btnOK.Visible = False
        m_connector.btnOK.Enabled = False
        UpdateStatus(strConnectivityMessage, "1")
        m_connector.cancelled = False
        bCancelReconnect = False
        m_connector.Location = New System.Drawing.Point(7, 65)
        m_connector.Show()
        Application.DoEvents()
        m_rfSocketManager = Nothing
        objAppContainer.objLogger.WriteAppLog("Disconnected Trying to reconnect", Logger.LogLevel.RELEASE)
        'here reconnector logic has to come 
        'Do While m_ReconnectTime <= Macros.RECONNECT_ATTEMPTS
        'waiting for cancel event click to process
        'If at all cancel is clicked then ideally the process happens here  because this threads priority is lowest
        'Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Lowest
        'Application.DoEvents()
        'here reconnector logic has to come 
        'There should be three reconnect attempts.
        CurrentStamp = DateAndTime.Now
        DATAPOOL.getInstance.isConnected = False
        objAppContainer.objLogger.WriteAppLog("Starting a new Thread for connection")
        Dim ReconnectThread As New Threading.Thread(DirectCast(Function() ReconnectionHandler(strConnectivityMessage, CurrentStamp), Threading.ThreadStart))
        ReconnectThread.Priority = Threading.ThreadPriority.Highest
        bConnectionCompleted = False
        ReconnectThread.Start()
        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Lowest
        While Not (bConnectionCompleted)
            Application.DoEvents()
            If m_connector.cancelled Then
                If Not bCancelReconnect Then
                    objAppContainer.objLogger.WriteAppLog("Reconnect Cancelled")
                    bCancelReconnect = True
                    isCancelled = True
                    'DATAPOOL.getInstance.NotifyConnectionStatus(DATAPOOL.ConnectionStatus.Cancelled)
                    'Return False
                End If
                Threading.Thread.Sleep(Macros.CANCEL_SLEEP_TIME)
            End If
        End While
        'v1.1 MCF Change
        If (objAppContainer.bMCFEnabled And Not DATAPOOL.getInstance.isConnected And Not bCancelReconnect) Then
            CurrentStamp = DateAndTime.Now
            DATAPOOL.getInstance.isConnected = False
            If fConnectAlternateInRF() Then
                objAppContainer.objLogger.WriteAppLog("Starting a new Thread for alternate connection")
                Dim AlternateConnectThread As New Threading.Thread(DirectCast(Function() ReconnectionHandler(strConnectivityMessage, CurrentStamp), Threading.ThreadStart))
                ReconnectThread.Priority = Threading.ThreadPriority.Highest
                bConnectionCompleted = False
                AlternateConnectThread.Start()
                Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Lowest
                While Not (bConnectionCompleted)
                    Application.DoEvents()
                    If m_connector.cancelled Then
                        If Not bCancelReconnect Then
                            objAppContainer.objLogger.WriteAppLog("Reconnect to alternate Cancelled")
                            bCancelReconnect = True
                            isCancelled = True
                            'DATAPOOL.getInstance.NotifyConnectionStatus(DATAPOOL.ConnectionStatus.Cancelled)
                            'Return False
                        End If
                        Threading.Thread.Sleep(Macros.CANCEL_SLEEP_TIME)
                    End If
                End While
            Else
                bIsReconnectingAfterDataTimeout = True
                sCloseSession()
                'Archana
                'Add condition to close the form
            End If
        End If


        objAppContainer.objLogger.WriteAppLog("Establish Connection END")
        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Normal
        'during a retry connection is cancel and at the same retry connection is established
        'then cancel superceeds the connected event.
        If isCancelled Then
            Return False
        Else
            Return DATAPOOL.getInstance.isConnected
        End If
        Return bTemp
    End Function
    Public Function ReconnectionHandler(ByVal ReconnectMessage As String, ByVal CurrentTryStamp As Date) As Boolean
        bConnectionCompleted = False
        Dim iReconnectTime As Integer = 1
        objAppContainer.objLogger.WriteAppLog("Entered Reconnection Handler", Logger.LogLevel.RELEASE)
        objAppContainer.objLogger.WriteAppLog("Current Stamp : " + CurrentTryStamp.ToString() + "Try Stamp :" + CurrentTryStamp.ToString())
        Do While ((iReconnectTime <= Macros.RECONNECT_ATTEMPTS) And _
                  (CurrentTryStamp = CurrentStamp))
            If Not bCancelReconnect Then
                UpdateStatus(ReconnectMessage, iReconnectTime)
                objAppContainer.objLogger.WriteAppLog("Reconnect Attempt - " + iReconnectTime.ToString(), Logger.LogLevel.RELEASE)
                Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Highest
                objAppContainer.objLogger.WriteAppLog("Initialising Socket", Logger.LogLevel.RELEASE)
                'm_rfSocketManager = New RFSocketManager(ConfigDataMgr.GetInstance(). _
                '     GetParam(ConfigKey.SERVER_IPADDRESS).ToString(), ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT))\
                m_rfSocketManager = New RFSocketManager(objAppContainer.strActiveIP, ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT))
                objAppContainer.objLogger.WriteAppLog("SOCKET DEFINED... ESTABLISHING CONNECTION", Logger.LogLevel.RELEASE)
                ConnectToSocket()
                status = DATAPOOL.getInstance.WaitForConnection()
                objAppContainer.objLogger.WriteAppLog("Completed Waiting", Logger.LogLevel.RELEASE)
                Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Normal
                'Wat ever might be the status if the cancel is click Quit the current Module
                If status = DATAPOOL.ConnectionStatus.Connected Then
                    objAppContainer.objLogger.WriteAppLog("CONNECTION ESTABLISHED AND TERMINATING THE THREAD", Logger.LogLevel.RELEASE)
                    DATAPOOL.getInstance.isConnected = True
                    'objAppContainer.objLogger.WriteAppLog("Connection was Successfully established", Logger.LogLevel.RELEASE)
                    Exit Do
                Else
                    DATAPOOL.getInstance.isConnected = False
                    m_rfSocketManager = Nothing
                End If
                'If m_ReconnectTime > 3 Then
                'objAppContainer.objLogger.WriteAppLog("Connection was not established, sleep before the Next Attempt", Logger.LogLevel.RELEASE)
                'Check before the Sleep
                If bCancelReconnect Then
                    Exit Do
                End If
                System.Threading.Thread.Sleep(m_WaitTimeBeforeReconnect)
                iReconnectTime = iReconnectTime + 1
            Else
                Exit Do
            End If
        Loop
        objAppContainer.objLogger.WriteAppLog("Completed the connection thread", Logger.LogLevel.RELEASE)
        bConnectionCompleted = True
    End Function
    Public Sub DisplayConnector()
        m_connector.Visible = True
    End Sub

    Public Sub HideConnector()
        m_connector.Visible = False
    End Sub
    Public Sub UpdateStatus(ByVal Connection_Message As String, ByVal Attempt As String)
        If m_connector.InvokeRequired Then
            m_connector.Invoke(New m_Invoker(AddressOf UpdateStatus), Connection_Message, Attempt)
        End If
        m_connector.setStatus(String.Format(Connection_Message, Attempt))
        Application.DoEvents()
    End Sub
    Public Function HandleTimeOut(Optional ByVal ConnectionLostScenario As GOExportDataManager.ConnectionLostScenario = GOExportDataManager.ConnectionLostScenario.While_Others, _
                                  Optional ByRef strException As String = "") As Boolean
        Dim strMessage As String = "Unable to communicate with the controller. " + _
                                 "This may be due to controller reload, network or power failure. Once ready " + _
                                 "select RETRY to reconnect or select CANCEL to quit."
        m_connector.lblMessage.Text = strMessage
        m_connector.lblMessage.Font = New System.Drawing.Font("Tahoma", 8.5!, System.Drawing.FontStyle.Regular)
        m_connector.Label1.Text = "Timeout Occurred"
        m_connector.btnCancel.Visible = False
        m_connector.btnOK.Visible = False
        m_connector.btnTimeoutRetry.Visible = True
        m_connector.btnTimeoutRetry.Enabled = True
        m_connector.btnTimeoutCancel.Visible = True
        m_connector.btnTimeoutCancel.Enabled = True
        Cursor.Current = Cursors.Default
        m_connector.Location = New System.Drawing.Point(7, 65)
        m_connector.Refresh()
        m_connector.ShowDialog()
        m_connector.Hide()
        'At this point either of the option is selected.
        'Reset the buttons and label text to default so as not to disturb reconnect form display.
        m_connector.btnTimeoutRetry.Visible = False
        m_connector.btnTimeoutRetry.Enabled = False
        m_connector.btnTimeoutCancel.Visible = False
        m_connector.btnTimeoutCancel.Enabled = False
        m_connector.lblMessage.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
        m_connector.Label1.Text = "Unable to Connect to Controller"
        If m_connector.TimeoutCancel Then

            If Not (ConnectionLostScenario = GOExportDataManager.ConnectionLostScenario.While_Log_OFF Or _
                       ConnectionLostScenario = GOExportDataManager.ConnectionLostScenario.While_Login Or _
                           ConnectionLostScenario = GOExportDataManager.ConnectionLostScenario.While_Sending_Start_Record) Then
                Select Case objAppContainer.objActiveModule
                    Case AppContainer.ACTIVEMODULE.CRDCLM
                        'credit claim
                        CCSessionMgr.GetInstance().EndSession(True)
                        Exit Select
                    Case AppContainer.ACTIVEMODULE.CRTRCL
                        'create recall
                        GOSessionMgr.GetInstance().EndSession(True)
                        Exit Select
                    Case AppContainer.ACTIVEMODULE.GDSOUT
                        'Goods Out
                        GOSessionMgr.GetInstance().EndSession(True)
                        Exit Select
                    Case AppContainer.ACTIVEMODULE.GDSTFR
                        'goods out transfer
                        GOTransferMgr.GetInstance().EndSession(True)
                        Exit Select
                    Case AppContainer.ACTIVEMODULE.PHSLWT
                        'pharmacy special waste.
                        PSWSessionMgr.GetInstance().EndSession()
                        Exit Select
                    Case AppContainer.ACTIVEMODULE.RECALL
                        'create recall.
                        RLSessionMgr.GetInstance().EndSession()
                        Exit Select
                    Case Else
                        Exit Select
                End Select
            End If
            WorkflowMgr.GetInstance.Reset()
            Threading.Thread.Sleep(100)
            strException = Macros.CONNECTIVITY_TIMEOUTCANCEL
            Return False
        Else
            'Code to reattempt to connect to transact.
            Return True
        End If
    End Function
    ''' <summary>
    ''' Function to Parse the response received in RF Mode.
    ''' </summary>
    ''' <param name="m_ResponseMessage"></param>
    ''' <param name="strRecordSent"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ParseResponseRF(ByVal m_ResponseMessage As String, ByVal strRecordSent As String) As Boolean
        Dim bTemp As Boolean = False
        m_ResponseMessage = NTrim(m_ResponseMessage)
        'new message format 
        m_ResponseMessage = m_ResponseMessage.Substring(5, m_ResponseMessage.Length - 5)
        '''''
        'Response received from the controller.
        objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Response received: " _
                                              & m_ResponseMessage, Logger.LogLevel.RELEASE)
        'Based on the message type parse the response and return the value.
        Select Case (m_ResponseMessage.Substring(0, 3))
            Case "EQR"
                bTemp = m_TransactMessageParser.ParseEQR(m_ResponseMessage)
            Case "NAK"
                If m_ResponseMessage.StartsWith("NAKERROR") Then
                    bTemp = m_TransactMessageParser.ParseNAKERROR(m_ResponseMessage)
                Else
                    bTemp = m_TransactMessageParser.ParseNAK(m_ResponseMessage)
                End If
            Case "ACK"
                bTemp = m_TransactMessageParser.ParseACK(m_ResponseMessage)
            Case "SNR"
                bTemp = m_TransactMessageParser.ParseSNR(m_ResponseMessage)
            Case "UOR"
                bTemp = m_TransactMessageParser.ParseUOR(m_ResponseMessage)
            Case "DSR"
                bTemp = m_TransactMessageParser.ParseDSR(m_ResponseMessage)
            Case "DSE"
                bTemp = m_TransactMessageParser.ParseDSE()
            Case "RCC"
                bTemp = m_TransactMessageParser.ParseRCC(m_ResponseMessage)
            Case "RCE"
                bTemp = m_TransactMessageParser.ParseRCE()
            Case "RCF"
                bTemp = m_TransactMessageParser.ParseRCF(m_ResponseMessage)
            Case "STR"
                bTemp = m_TransactMessageParser.ParseSTR(m_ResponseMessage)
            Case "RCJ"
                bTemp = m_TransactMessageParser.ParseRCJ(m_ResponseMessage)
        End Select
        Return bTemp
    End Function
    Public Function SendRecordRF(ByVal strRecord As String) As Boolean
        'Dim m_SendBytes As [Byte]() = Nothing
        'Dim m_ReadBytes As [Byte]() = Nothing
        'Dim m_Status As Boolean = Nothing
        'Dim m_RetryWrite As Integer = 0
        Dim bTemp As Boolean = False
        Try
            'Read the rety attempt for writing data to the socket stream.
            'm_RetryWrite = Macros.WRITE_RETRY
            'Send the record to the controller.
            'Do
            Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Highest
            If Not m_rfSocketManager.SendText(strRecord) Then
                objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Cannot " _
                                                          & "send to socket", _
                                                          Logger.LogLevel.RELEASE)
                Return bTemp
            End If
        
        Catch ex As Exception
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                    "Error in sending export data to controller:" _
                                    & ex.Message.ToString(), _
                                    Logger.LogLevel.RELEASE)
            'incase of exception return false.
            Return False
        Finally
            bTemp = True
            'm_SendBytes = Nothing
            'm_ReadBytes = Nothing
        End Try
        Return bTemp
    End Function
    Private Sub ConnectToSocket()
        If m_rfSocketManager IsNot Nothing Then
            ' m_rfSocketManager.ReconnectInterval = 5000
            AddHandler m_rfSocketManager.OnConnect, New RFSocketManager.ConnectionDelegate(AddressOf Me.HandleOnConnect)
            AddHandler m_rfSocketManager.OnDisconnect, New RFSocketManager.ConnectionDelegate(AddressOf Me.HandleOnDisconnect)
            AddHandler m_rfSocketManager.OnError, New RFSocketManager.ErrorDelegate(AddressOf HandleOnError)
            AddHandler m_rfSocketManager.OnRead, New RFSocketManager.ConnectionDelegate(AddressOf HandleOnRead)
            AddHandler m_rfSocketManager.OnWrite, New RFSocketManager.ConnectionDelegate(AddressOf HandleOnWrite)
            AddHandler m_rfSocketManager.OnReceiveTimeout, New RFSocketManager.ConnectionDelegate(AddressOf HandleOnReceiveTimeout)
            m_rfSocketManager.Connect()
        Else
            MessageBox.Show("Error Port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Cannot Connect to Socket ", _
                                                          Logger.LogLevel.RELEASE)
        End If
    End Sub
    Private Sub DisConnectSocket()
        m_rfSocketManager.Disconnect()
    End Sub
    Private Sub SentData(ByVal data As String)
        m_rfSocketManager.SendText(data)
    End Sub
    Private Sub HandleOnConnect(ByVal soc As Socket)
        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Lowest
        DATAPOOL.getInstance.NotifyConnectionStatus(DATAPOOL.ConnectionStatus.Connected)
        objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: EvntOnConnect ", _
                                                          Logger.LogLevel.RELEASE)
    End Sub
    Private Sub HandleOnDisconnect(ByVal soc As Socket)
        objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: EvntOnDisconnect ", _
                                                       Logger.LogLevel.RELEASE)
        DATAPOOL.getInstance.isConnected = False
    End Sub
    Private Sub HandleOnError(ByVal ErrorMessage As String, ByVal soc As Socket, ByVal ErrorCode As Integer)
        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Lowest
        Dim m_attempts As Integer = 1
        objAppContainer.objLogger.WriteAppLog("HandleOnError:: In Error :: Message :" + _
                                              ErrorMessage + ",Error Code : " + _
                                              ErrorCode.ToString(), Logger.LogLevel.RELEASE)
        DATAPOOL.getInstance.isConnected = False
        Do While m_attempts <= 2
            If DATAPOOL.getInstance.WaitingForConnection Then
                DATAPOOL.getInstance.NotifyConnectionStatus(DATAPOOL.ConnectionStatus.Disconnected)
                Exit Do
            ElseIf DATAPOOL.getInstance.WaitingForNotification Then
                DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.Disconnected)
                Exit Do
            Else
                'checking whether a control comes and waits after certain time 
                objAppContainer.objLogger.WriteAppLog("Nothing is waiting:: sleeping for 100ms :: attempt - " + _
                                                      m_attempts.ToString(), Logger.LogLevel.RELEASE)
                m_attempts = m_attempts + 1
                Threading.Thread.Sleep(100)
            End If
        Loop
        'End If
    End Sub
    Private Sub HandleOnRead(ByVal soc As Socket)
        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Lowest
        m_socketReceivedText = m_rfSocketManager.ReceivedText
        ParseResponseRF(m_socketReceivedText, m_socketSentText)
        objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Receive - " _
                                              & m_socketReceivedText, _
                                              Logger.LogLevel.RELEASE)
    End Sub
    Private Sub HandleOnWrite(ByVal soc As Socket)
        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Highest
        m_socketSentText = m_rfSocketManager.WriteText
        objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter::HandleOnWrite:: Sent - " _
                                              & m_socketSentText, _
                                              Logger.LogLevel.RELEASE)
    End Sub
    Private Sub HandleOnReceiveTimeout(ByVal soc As Socket)
        objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Receive Timed Out ", _
                                                        Logger.LogLevel.RELEASE)
        'Recieve Time Out
        DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.TimeOut)
    End Sub
    'v1.1 MCF Changes
    '' <summary>
    '' Prompts the User Whether he wishes to conenct to Alternate Controller
    '' </summary>
    '' <param>None </param>
    '' <return> Boolean True if the user wishes to connect to alternate; False otherwise </return>
    Public Function fConnectAlternateInRF() As Boolean
        Try
            Dim bConnectToAlternate As Boolean = False
            Dim strMessage As String = "Unable to regain connectivity to the current controller." + _
                           " All work in this session may be lost. Select OK to exit or ALTERNATE " + _
                           "to continue with a new session on the alternate controller."
            m_connector.lblMessage.Text = strMessage
            m_connector.lblMessage.Font = New System.Drawing.Font("Tahoma", 8.5!, System.Drawing.FontStyle.Regular)
            m_connector.Label1.Text = "Unable to Connect to Controller"
            m_connector.btnCancel.Visible = False
            m_connector.btnOK.Visible = False
            m_connector.btnTimeoutRetry.Visible = False
            m_connector.btnTimeoutCancel.Visible = False
            m_connector.btnCancelAlternate.Visible = True
            m_connector.btnCancelAlternate.Enabled = True
            m_connector.btnConnectAlternate.Visible = True
            m_connector.btnConnectAlternate.Enabled = True
            m_connector.Refresh()
            Cursor.Current = Cursors.Default
            m_connector.Location = New System.Drawing.Point(7, 65)
            m_connector.ShowDialog()
            'At this point either of the option is selected.
            'Reset the buttons and label text to default so as not to disturb reconnect form display.
            m_connector.btnCancelAlternate.Visible = False
            m_connector.btnCancelAlternate.Enabled = False
            m_connector.btnConnectAlternate.Visible = False
            m_connector.btnConnectAlternate.Enabled = False
            m_connector.btnCancel.Visible = True
            m_connector.btnCancel.Enabled = True
            m_connector.lblMessage.Text = ""
            m_connector.lblMessage.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
            If m_connector.ConnectToAlternate = 1 Then
                bConnectToAlternate = True
            Else
                m_connector.HideConnector()
            End If
            If bConnectToAlternate Then
                objAppContainer.iConnectedToAlternate = 1
                If objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString() Then
                    objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.SECONDARY_IPADDRESS).ToString()
                Else
                    objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString()
                End If
            Else
                objAppContainer.iConnectedToAlternate = -1
            End If
            Return bConnectToAlternate
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
        End Try
    End Function
    '' <summary>
    '' Closes the current sessions
    '' </summary>
    '' <param>None </param>
    '' <return> Boolean True if successfully close the session; False otherwise </return>
    Public Sub sCloseSession()
        Select Case objAppContainer.objActiveModule
            Case AppContainer.ACTIVEMODULE.CRDCLM
                'credit claim
                CCSessionMgr.GetInstance().EndSession(True)
                Exit Select
            Case AppContainer.ACTIVEMODULE.CRTRCL
                'create recall
                GOSessionMgr.GetInstance().EndSession(True)
                Exit Select
            Case AppContainer.ACTIVEMODULE.GDSOUT
                'Goods Out
                GOSessionMgr.GetInstance().EndSession(True)
                Exit Select
            Case AppContainer.ACTIVEMODULE.GDSTFR
                'goods out transfer
                GOTransferMgr.GetInstance().EndSession(True)
                Exit Select
            Case AppContainer.ACTIVEMODULE.PHSLWT
                'pharmacy special waste.
                PSWSessionMgr.GetInstance().EndSession()
                Exit Select
            Case AppContainer.ACTIVEMODULE.RECALL
                'create recall.
                RLSessionMgr.GetInstance().EndSession()
                Exit Select
            Case Else
                Exit Select
        End Select
        objAppContainer.bActiveform = False
        WorkflowMgr.GetInstance.EndSession()
    End Sub
    'Public Function ReadDatafromSocket(ByRef strData As String) As Boolean
    '    'Have to implement Timer logic Becoz now connection problem can occur
    '    Try
    '        Dim m_temp As Boolean = False
    '        Dim m_RecievedBytes As [Byte]() = Nothing
    '        m_SockectConnMgr.ReadData(m_RecievedBytes)
    '        If m_RecievedBytes.Length > 0 Then
    '            strData = m_RecievedBytes.ToString()
    '            m_temp = True
    '        End If
    '    Catch ex As Exception
    '        ''have to implement logger here
    '    End Try
    'End Function

#End If
#End Region
End Class