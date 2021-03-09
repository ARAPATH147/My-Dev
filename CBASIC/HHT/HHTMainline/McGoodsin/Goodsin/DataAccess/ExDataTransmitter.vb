Imports System.IO
Imports System.Text
Imports System.Runtime.InteropServices
#If NRF Then


'''****************************************************************************
''' <FileName>ExDataTransmitter.vb</FileName>
''' <summary>
''' Used for transmitting the export data present in the MC70 device to the 
''' EPOS controller.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''****************************************************************************
'''
'''****************************************************************************
'''* Modification Log 
'''****************************************************************************
'''* No:        Author             Date                 Description 
'''* 1.1     Kiran Krishnan      27/04/2015  Modified as part of DALLAS project.
'''                  (KK)                    Added new function sendDAC to 
'''                                          verify if a store is Dallas +ve
'''                                          Receiving enabled or not
'''*************************************************************************** 
Public Class ExDataTransmitter
    Private m_ExDatFilePath As String = Nothing
    Private aExDataRecords() As String = Nothing
    Private m_SockectConnMgr As SocketConnectionMgr = Nothing
    Private m_ListID As String = Nothing
    Private m_Retry As Integer = 0
    Private m_ControllerDateTime As String = Nothing
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
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        'Get export data file name.
        m_ExDatFilePath = ConfigDataMgr.GetInstance(). _
                          GetParam(ConfigKey.EXPORT_FILE_PATH).ToString()
        'Initialise the socket connection.
        m_SockectConnMgr = New SocketConnectionMgr()
        'Get retry attempt to send export data.
        m_Retry = Macros.WRITE_RETRY
    End Sub
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
                    'Read next line from the array.
                    m_Line = aExDataRecords(iCount)
                    'Replace all commas in a line with no space.
                    m_Line = m_Line.Replace(",", "")
                    If m_Line.StartsWith("GIA") Then
                        'Send export data for module starts
                        
                        If Not SendRecord(m_Line) Then
                            'Add the exception to the device log.
                            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                        "Error in sending export data to controller:" _
                                        & m_Line, Logger.LogLevel.RELEASE)
                            'Failed to send the record or receive a list ID.
                            Return "-1"
                        End If
                        'DO not wait for GIX if a GIA for COnfig values is sent
                        If m_Line.Substring(6, 10) <> "XXCX000000" Then

                            'Send all the records until GIX record is read.
                            Do
                                iCount = iCount + 1
                                m_Line = aExDataRecords(iCount).ToString()
                                ''Substitute the List ID in GAP record to be sent.
                                'm_Line = m_Line.Replace("LID", m_ListID)
                                m_Line = m_Line.Replace(",", "")

                                'Return false if error occurred while transmitting.
                                If Not SendRecord(m_Line) Then
                                    'Add the exception to the device log.
                                    AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                                "Error in sending export data to controller:" _
                                                & m_Line, Logger.LogLevel.RELEASE)
                                    'Failed to send the record or receive a list ID.
                                    Return "-1"
                                End If
                            Loop Until m_Line.StartsWith("GIX")
                        End If
                    ElseIf m_Line.StartsWith("SOR") Then
                        'Check for the IP address in the record.
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
                            Return "-1"
                        End If
                    Else
                        'Send export data record for all pickign lists,
                        'Price Check, Count lists, SEL print request.
                        If Not SendRecord(m_Line) Then
                            'Add the exception to the device log.
                            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                        "Error in sending export data to controller:" _
                                        & "Message" & m_Line & "not sent.", _
                                        Logger.LogLevel.RELEASE)
                            Return "-1"
                        End If
                    End If
                    'Increment the counter variable.
                    iCount = iCount + 1
                    m_Line = Nothing
                    m_Status = Nothing
                    m_TempLine1 = ""
                    m_TempLine2 = ""
                End While 'End of first while
            ElseIf aExDataRecords.Length = 3 Then
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                        "ExDataTransmitter:" _
                                        & "No data available in the export data file.", _
                                        Logger.LogLevel.RELEASE)
                'delete export data file as no export data available
                'except SOR and OFF
                File.Delete(m_ExDatFilePath & "/" & Macros.EXPORT_FILE_NAME)
                'return false to the calling function
                Return "0"
            Else
                'Add the exception to the device log.
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                        "Error in sending export data to controller:" _
                                        & "No data available in the export data file.", _
                                        Logger.LogLevel.RELEASE)
                'return false to the calling function
                Return "-1"
            End If
            'Before returning the status, delete the export data file in the 
            'local device.
            File.Delete(m_ExDatFilePath & "/" & Macros.EXPORT_FILE_NAME)
            'Update the time in config file.
            ConfigDataMgr.GetInstance.SetParam(ConfigKey.LAST_EXDATA_DOWNLOAD_TIME, _
                                               DateTime.Now.ToString())
        Catch ex As Exception
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                    "Error in sending export data:" _
                                    & ex.Message.ToString(), _
                                    Logger.LogLevel.RELEASE)
            Return "-1"
        Finally
            aExDataRecords = Nothing
            'close the socket connection established with the controller.
            m_SockectConnMgr.TerminateConnection()
        End Try
        'return true if no error occured during the export data download.
        Return "1"
    End Function
    ''' <summary>
    ''' Function to read the content of export data file to string array.
    ''' </summary>
    ''' <param name="m_FileName"></param>
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
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                    "Error in reading export data file:" _
                                    & ex.Message.ToString(), _
                                    Logger.LogLevel.RELEASE)
            Return False
        End Try
        'return the array.
        Return True
    End Function
    ''' <summary>
    ''' To parse the resonse received from the TRANSACT service and return status
    ''' accordingly.
    ''' </summary>
    ''' <param name="m_ResponseMessage">Response message received from the 
    ''' TRANSACT service</param>
    ''' <returns>Bool
    ''' True - ACK, SNR is received.
    ''' False - NAK is received or any error occurred.
    ''' </returns>
    ''' <remarks></remarks>
    Private Function ParseResponse(ByVal m_ResponseMessage As String) As Boolean
        'new message format
        m_ResponseMessage = m_ResponseMessage.Substring(5, m_ResponseMessage.Length - 5)
        'Response received from the controller.
        objAppContainer.objLogger.WriteAppLog("ExDataTransmitter: Response received: " _
                                              & m_ResponseMessage, Logger.LogLevel.INFO)
        'Based on the message type parse the response and return the value.
        Select m_ResponseMessage.Substring(0, 3)
            Case "ACK"
                Return True
            Case "GIB"
                Return True
            Case "GIR"
                Return True
            Case "SNR"
                'set the device date time according to the date time received 
                'in the response.
                m_ControllerDateTime = m_ResponseMessage.Substring(Macros.SNR_DATETIME_START_INDEX, _
                                                      Macros.SNR_DATETIME_LENGTH)
                Return SetDeviceDateTime(m_ControllerDateTime)
            Case "NAK"
                Dim strNakMessage As String = ""
                strNakMessage = m_ResponseMessage.Replace("NAK", "")    'Supress NAK String
                strNakMessage = strNakMessage.Replace("NAKERROR", "")   'Suppress NAKERROR string
                'Display the recevied NAK message to the user.
                MessageBox.Show("Received error from controller:" + strNakMessage, _
                                "Error", MessageBoxButtons.OK, _
                                MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                'If response received is NAK
                Return False
        End Select
    End Function
    ''' <summary>
    ''' Convert the record to bytes and send to the TRANSACT service.
    ''' Receive the response and parse it to get the details.
    ''' </summary>
    ''' <param name="strRecord">Record to be sent to the TRANSACT</param>
    ''' <returns> Bool
    ''' True if successfully sent and received the records.
    ''' False is any error occurred during send / receive operation.
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
                                                  & strRecord, Logger.LogLevel.INFO)
             'new message format
            'strRecord = strRecord.Substring(0, strRecord.Length - 2)
            strRecord = Chr(255) + (strRecord.Length + 5).ToString.PadLeft(4, "0") + strRecord
            m_SendBytes = Encoding.ASCII.GetBytes(strRecord.ToString())
            m_SendBytes(0) = &HFF
            m_RetryWrite = m_Retry
            '  m_SendBytes = Encoding.ASCII.GetBytes(strRecord)

            'Read the rety attempt for writing data to the socket stream.
            m_RetryWrite = Macros.WRITE_RETRY
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
                                                             m_ReadBytes.Length))
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
                'write the error message to the app log
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                            "Unable to write record to the stream. Retry attempt" _
                            & "failed for" & m_RetryWrite & "times", _
                            Logger.LogLevel.RELEASE)
                Return False
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
        Dim strRequest As String = Nothing
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
            'strRecord.Append(Environment.NewLine)
            'new message format
            ' m_SendBytes = Encoding.ASCII.GetBytes(strRecord.ToString())

            'strData = strData.Substring(0, strData.Length - 2)
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
    ' V1.1 - KK
    ' Added new sendDAC function as part of DALLAS project.

    '''**************************************************************************
    ''' <summary>
    ''' The function creates transact message DAC and sends it to controller
    ''' </summary>
    ''' <param name = "strUserID"></param>
    ''' <returnvalue>NAK/ACK</returnvalue>
    ''' <remarks></remarks>	
    '''**************************************************************************
    Public Function SendDAC(ByVal strUserID As String) As String
        Dim strRecord As StringBuilder = Nothing

        Dim byaSendBytes As [Byte]() = Nothing
        Dim byaReadBytes As [Byte]() = Nothing
        Dim cResponse As String = Nothing
        Dim iRetryWrite As Integer = 0
        Dim cRequest As String = Nothing
        Dim cData As String = Nothing
        Try

            'GEnerate record for DAC
            strRecord = New StringBuilder()
            strRecord.Append("DAC")
            strRecord.Append(strUserID)
            cData = strRecord.ToString()
            cData = Chr(255) + (cData.Length + 5).ToString.PadLeft(4, "0") + cData
            byaSendBytes = Encoding.ASCII.GetBytes(cData.ToString())
            byaSendBytes(0) = &HFF
            iRetryWrite = m_Retry
            'Send the record to the controller.
            'Records sent to the controller.
            objAppContainer.objLogger.WriteAppLog("ExDataTransmitter: Record sent: " _
                                                  & strRecord.ToString(), Logger.LogLevel.INFO)
            Do
                If m_SockectConnMgr.TransmitData(byaSendBytes) Then
                    'Read the response stream from the client.
                    If m_SockectConnMgr.ReadData(byaReadBytes) And _
                       byaReadBytes.Length > 0 Then
                        'Return the response after parsing it.
                        cResponse = Encoding.ASCII.GetString(byaReadBytes, _
                                                        0, _
                                                        byaReadBytes.Length)
                        cResponse = cResponse.Trim()
                        'new message format 
                        cResponse = cResponse.Substring(5, cResponse.Length - 5)
                        'Response received from the controller.
                        objAppContainer.objLogger.WriteAppLog("ExDataTransmitter: Response received: " _
                                                              & cResponse, Logger.LogLevel.INFO)
                        If cResponse.StartsWith("ACK") Then
                            ' V1.1 - KK
                            ' Sets the bDallasPosReceiptEnabled to True once an ACK is received from DPR store
                            objAppContainer.bDallasPosReceiptEnabled = True
                            iRetryWrite = 0

                        ElseIf cResponse.StartsWith("NAK") And Not (cResponse.Substring(0, 8) = "NAKERROR") Then
                            'Sets the bDallasPosReceiptEnabled to False once an NAK is received from DPR store
                            objAppContainer.bDallasPosReceiptEnabled = False
                            iRetryWrite = 0

                        ElseIf cResponse.StartsWith("NAKERROR") Then
                            cResponse = cResponse.Replace("NAKERROR", "")
                            'Sets the bDallasPosReceiptEnabled to False once an NAK Error is received from DPR store
                            objAppContainer.bDallasPosReceiptEnabled = False
                            iRetryWrite = 0
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
            Loop Until iRetryWrite = 0
        Catch ex As Exception
            'Add the exception to the application log.
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
        Finally
            'Clear the variable memories.
            byaSendBytes = Nothing
            byaReadBytes = Nothing
        End Try
    End Function


    ''' <summary>
    ''' Gets and return the status of the socket connection established
    ''' with the controller.
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function GetSocketStatus() As Boolean
        Return m_SockectConnMgr.ConnectionStatus
    End Function
    ''' <summary>
    ''' To send SOR record before sending ALR record.
    ''' </summary>
    ''' <param name="strRecords"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendSOR(ByVal strRecords As String, ByRef strDateTime As String) As Boolean
        strRecords = strRecords.Replace(",", "")
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
End Class
#End If