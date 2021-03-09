Imports System.Net.Sockets
Imports System.Net
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Fixed defect #16
''' </Summary>
'''****************************************************************************
Public Class LOGTransmitter
    Private m_Socket As TcpClient = Nothing
    Private m_NetworkStream As NetworkStream
    Private m_LOGHeader As String

    Public Sub New()
        Dim objUtils As Utility = New Utility()
        ' m_LOGHeader = "LOG" + Macros.DEFAULT_USERID + Utility.GetMacAddress() + Utility.GetIPAddress().PadRight(15, " ")
        m_LOGHeader = "LOG" + Macros.DEFAULT_USERID + objUtils.GetSerialNumber() + Utility.GetIPAddress().PadRight(15, " ")
    End Sub
    Public Sub EndSession()
        Try
            If Not m_Socket Is Nothing Then
                If m_Socket.Client.Connected Then m_Socket.Close()
                m_Socket = Nothing
                m_NetworkStream = Nothing
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("LOGTransmitter:: End Session: Error while closing the session" + _
                                                          "Error message: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Public Function sendLog(ByVal LOG_Action As Action, ByVal LOG_Status As Status, ByVal enumFileName As FileName, Optional ByVal Error_Reason As Reasons = Reasons.Not_Applicable, Optional ByVal RowNo As String = "")
        Dim bTemp As Boolean = False
        Dim strFileName As String = ""
        'Log Message is declared
        'Appending Transaction Identifier and operator ID
        'Operator ID Hard coded 
        Dim strLogMessage As New System.Text.StringBuilder(m_LOGHeader)
        Dim strFile As String = ""
        Try
            objAppContainer.objLogger.WriteAppLog("LOGTransmitter::sendLog:: Preparing LOG Transaction", _
                                                            Logger.LogLevel.RELEASE)
            'Appending Data and Time 
            strLogMessage.Append(DateAndTime.Now.ToString("yyMMddhhmmss"))

            'Set File Name
            Select Case enumFileName
                Case FileName.ASN
                    strLogMessage.Append("17")
                    strFileName = "ASN.CSV"
                Case FileName.BARCODE
                    strLogMessage.Append("02")
                    strFileName = "BARCODE.CSV"
                Case FileName.BOOTCODE
                    strLogMessage.Append("01")
                    strFileName = "BOOTCODE.CSV"
                Case FileName.CATEGORY
                    strLogMessage.Append("09")
                    strFileName = "CATEGORY.CSV"
                Case FileName.CONTROL
                    strLogMessage.Append("12")
                    strFileName = "CONTROL.CSV"
                Case FileName.COUNT
                    strLogMessage.Append("14")
                    strFileName = "COUNT.CSV"
                Case FileName.CREDIT
                    strLogMessage.Append("15")
                    strFileName = "CREDIT.CSV"
                Case FileName.DEAL
                    strLogMessage.Append("03")
                    strFileName = "DEAL.CSV"
                Case FileName.DIRECTS
                    strLogMessage.Append("16")
                    strFileName = "DIRECTS.CSV"
                Case FileName.LIVEPOG
                    strLogMessage.Append("08")
                    strFileName = "LIVEPOG.CSV"
                Case FileName.MODULES
                    strLogMessage.Append("10")
                    strFileName = "MODULE.CSV"
                Case FileName.PGROUP
                    strLogMessage.Append("04")
                    strFileName = "PGROUP.CSV"
                Case FileName.PICKING
                    strLogMessage.Append("13")
                    strFileName = "PICKING.CSV"
                Case FileName.POD_Log_Files
                    strLogMessage.Append("51")
                    strFileName = "PODLOGFILES"
                Case FileName.RECALL
                    strLogMessage.Append("07")
                    strFileName = "RECALL.CSV"
                Case FileName.SSCUODOT
                    strLogMessage.Append("18")
                    strFileName = "SSCUODOT.CSV"
                Case FileName.SUPPLIER
                    strLogMessage.Append("05")
                    strFileName = "SUPPLIER.CSV"
                Case FileName.SYNCTRL
                    strLogMessage.Append("00")
                    strFileName = "SYNCTRL"
                Case FileName.USERS
                    strLogMessage.Append("06")
                    strFileName = "USERS.CSV"
                Case FileName.SHELFDES
                    strLogMessage.Append("11")
                    strFileName = "SHELFDES.CSV"
            End Select

            'Set the Corresponding Action
            Select Case LOG_Action
                Case Action.TFTP_LOAD
                    strLogMessage.Append("L")
                Case Action.TFTP_SEND
                    strLogMessage.Append("S")
                Case Action.FTP_LOAD
                    strLogMessage.Append("F")
                Case Action.FTP_SEND
                    strLogMessage.Append("P")
            End Select

            ''Set the status
            Select Case LOG_Status
                Case Status.START
                    strLogMessage.Append("S")
                Case Status.END_OK
                    strLogMessage.Append("E")
                Case Status.ABEND
                    strLogMessage.Append("X")
                Case Else
                    strLogMessage.Append(" ")
            End Select

            'Append File NAme
            strLogMessage.Append(strFileName.PadRight(12, " "))

            'Error Messages and Reason text comes here 
            Select Case Error_Reason
                Case Reasons.Not_Applicable
                    strLogMessage.Append("00")
                    strLogMessage.Append("".PadRight(20, " "))
                Case Reasons.File_Build_Incomplete
                    strLogMessage.Append("01")
                    strLogMessage.Append("REFBUILD INCOMPLETE ")
                Case Reasons.Build_Ready
                    strLogMessage.Append("00")
                    strLogMessage.Append("REFBUILD FILE READY ")
                Case Reasons.Length_Mismatch
                    strLogMessage.Append("02")
                    strLogMessage.Append("REC LENGTH MISMATCH ")
                Case Reasons.PKEY_Violation
                    strLogMessage.Append("03")
                    strLogMessage.Append("PKEY VIOLATION")
                    strLogMessage.Append(RowNo.PadLeft(6, " "))
                Case Reasons.File_Not_Found
                    strLogMessage.Append("04")
                    strLogMessage.Append("FILE NOT FOUND      ")
                Case Reasons.Disconnected
                    strLogMessage.Append("05")
                    strLogMessage.Append("DISCONNECTED        ")
                Case Reasons.Header_Name_Mismatch
                    strLogMessage.Append("06")
                    strLogMessage.Append("HEADER NAME MISMATCH")
                Case Reasons.Download_Fail
                    strLogMessage.Append("07")
                    strLogMessage.Append("DOWNLOAD FAILED     ")
                Case Reasons.Parse_File
                    strLogMessage.Append("00")
                    strLogMessage.Append("PARSE FILE STARTED  ")
                Case Reasons.Downloading_File
                    strLogMessage.Append("00")
                    If enumFileName = FileName.POD_Log_Files Then
                        strLogMessage.Append(("SENDING LOG FILES").PadRight(20, " "))
                    Else
                        strLogMessage.Append("START DOWNLOADING   ")
                    End If
                Case Reasons.Parse_Complete
                    strLogMessage.Append("00")
                    strLogMessage.Append("FILE PARSE COMPLETED")
                Case Reasons.Update_Success
                    strLogMessage.Append("00")
                    strLogMessage.Append("DB UPDATE SUCCESS   ")
                Case Reasons.Update_Error
                    strLogMessage.Append("08")
                    strLogMessage.Append("DBUPDATE:ERRORS AVBL")
                Case Reasons.Download_Complete
                    strLogMessage.Append("00")
                    If enumFileName = FileName.POD_Log_Files Then
                        strLogMessage.Append(("LOG FILES SENT").PadRight(20, " "))
                    Else
                        strLogMessage.Append("DOWNLOAD SUCCESS    ")
                    End If
                Case Reasons.Other_Errors
                    strLogMessage.Append("08")
                    strLogMessage.Append("OTHERS-CHECK LOGs   ")
            End Select

            bTemp = SendText(strLogMessage.ToString())
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("LOGTransmitter::sendLog:: Exception in Preparing LOG Transaction" & ex.Message, _
                                                  Logger.LogLevel.RELEASE)
            bTemp = False
        End Try
        strLogMessage = Nothing
        Return bTemp
    End Function
    ''' <summary>
    ''' Send Log _ Overload the FileName given as string
    ''' </summary>
    ''' <param name="LOG_Action"></param>
    ''' <param name="LOG_Status"></param>
    ''' <param name="strFileName"></param>
    ''' <param name="Error_Reason"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function sendLog(ByVal LOG_Action As Action, ByVal LOG_Status As Status, ByVal strFileName As String, Optional ByVal Error_Reason As Reasons = Reasons.Not_Applicable, Optional ByVal RowNo As String = "")
        Dim bTemp As Boolean = False
        'Log Message is declared
        'Appending Transaction Identifier and operator ID
        'Operator ID Hard coded 
        Dim strLogMessage As New System.Text.StringBuilder(m_LOGHeader)
        Try
            objAppContainer.objLogger.WriteAppLog("LOGTransmitter::sendLog:: Preparing LOG Transaction", _
                                                  Logger.LogLevel.RELEASE)
            'Appending Data and Time 
            strLogMessage.Append(DateAndTime.Now.ToString("yyMMddHHmmss"))
            'Set File Name
            Select Case strFileName
                Case objAppContainer.objConfigFileParams.BOOTCODE
                    strLogMessage.Append("01")
                    strFileName = "BOOTCODE.CSV"
                Case objAppContainer.objConfigFileParams.BARCODE
                    strLogMessage.Append("02")
                    strFileName = "BARCODE.CSV"
                Case objAppContainer.objConfigFileParams.CATEGORY
                    strLogMessage.Append("09")
                    strFileName = "CATEGORY.CSV"
                Case objAppContainer.objConfigFileParams.DEAL
                    strLogMessage.Append("03")
                    strFileName = "DEAL.CSV"
                Case objAppContainer.objConfigFileParams.LIVEPOG
                    strLogMessage.Append("08")
                    strFileName = "LIVEPOG.CSV"
                Case objAppContainer.objConfigFileParams.SUPPLIER
                    strLogMessage.Append("05")
                    strFileName = "SUPPLIER.CSV"
                Case objAppContainer.objConfigFileParams.USERS
                    strLogMessage.Append("06")
                    strFileName = "USERS.CSV"
                Case objAppContainer.objConfigFileParams.PGROUP
                    strLogMessage.Append("04")
                    strFileName = "PGROUP.CSV"
                Case objAppContainer.objConfigFileParams.POGMODULE
                    strLogMessage.Append("10")
                    strFileName = "MODULE.CSV"
                Case objAppContainer.objConfigFileParams.RECALL
                    strLogMessage.Append("07")
                    strFileName = "RECALL.CSV"
                Case objAppContainer.objConfigFileParams.SHELFDES
                    strLogMessage.Append("11")
                    strFileName = "SHELFDES.CSV"
                Case "PODLOGFILES"
                    strLogMessage.Append("51")
                    strFileName = "PODLOGFILES"
            End Select

            'Set the Corresponding Action
            Select Case LOG_Action
                Case Action.TFTP_LOAD
                    strLogMessage.Append("L")
                Case Action.TFTP_SEND
                    strLogMessage.Append("S")
                Case Action.FTP_LOAD
                    strLogMessage.Append("F")
                Case Action.FTP_SEND
                    strLogMessage.Append("P")
            End Select

            ''Set the status
            Select Case LOG_Status
                Case Status.START
                    strLogMessage.Append("S")
                Case Status.END_OK
                    strLogMessage.Append("E")
                Case Status.ABEND
                    strLogMessage.Append("X")
            End Select

            'Append File NAme
            strLogMessage.Append(strFileName.PadRight(12, " "))

            'Error Messages and Reason text comes here 
            Select Case Error_Reason
                Case Reasons.Not_Applicable
                    strLogMessage.Append("00")
                    strLogMessage.Append("".PadRight(20, " "))
                Case Reasons.File_Build_Incomplete
                    strLogMessage.Append("01")
                    strLogMessage.Append("REFBUILD INCOMPLETE ")
                Case Reasons.Build_Ready
                    strLogMessage.Append("00")
                    strLogMessage.Append("REFBUILD FILE READY ")
                Case Reasons.Length_Mismatch
                    strLogMessage.Append("02")
                    strLogMessage.Append("REC LENGTH MISMATCH ")
                Case Reasons.PKEY_Violation
                    strLogMessage.Append("03")
                    strLogMessage.Append("PKEY VIOLATION")
                    strLogMessage.Append(RowNo.PadLeft(6, " "))
                Case Reasons.File_Not_Found
                    strLogMessage.Append("04")
                    strLogMessage.Append("FILE NOT FOUND      ")
                Case Reasons.Disconnected
                    strLogMessage.Append("05")
                    strLogMessage.Append("DISCONNECTED        ")
                Case Reasons.Header_Name_Mismatch
                    strLogMessage.Append("06")
                    strLogMessage.Append("HEADER NAME MISMATCH")
                Case Reasons.Download_Fail
                    strLogMessage.Append("07")
                    strLogMessage.Append("DOWNLOAD FAILED     ")
                Case Reasons.Parse_File
                    strLogMessage.Append("00")
                    strLogMessage.Append("PARSING FILE STARTED")
                Case Reasons.Downloading_File
                    strLogMessage.Append("00")
                    If strFileName.Equals("PODLOGFILES") Then
                        strLogMessage.Append(("SENDING LOG FILES").PadRight(20, " "))
                    Else
                        strLogMessage.Append("START DOWNLOADING   ")
                    End If
                Case Reasons.Parse_Complete
                    strLogMessage.Append("00")
                    strLogMessage.Append("FILE PARSE COMPLETE ")
                Case Reasons.Update_Success
                    strLogMessage.Append("00")
                    strLogMessage.Append("DB UPDATE SUCCESS   ")
                Case Reasons.Update_Error
                    strLogMessage.Append("08")
                    strLogMessage.Append("DBUPDATE:ERRORS AVBL")
                Case Reasons.Download_Complete
                    strLogMessage.Append("00")
                    strLogMessage.Append("DOWNLOAD SUCCESS    ")
                Case Reasons.Other_Errors
                    strLogMessage.Append("08")
                    strLogMessage.Append("OTHERS-CHECK LOGS   ")
            End Select
            bTemp = SendText(strLogMessage.ToString())
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("LOGTransmitter::sendLog::Exception in Preparing LOG Transaction" & ex.Message, _
                                                  Logger.LogLevel.RELEASE)
            bTemp = False
        End Try
        strLogMessage = Nothing
        Return bTemp
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="strMessage"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function SendText(ByVal strMessage As String) As Boolean
        Dim btemp As Boolean = False
        Dim bConnect As Boolean = False

        'Static Dim id As Long = 0
        Try
            objAppContainer.objLogger.WriteAppLog("LOG MESSAGE: " + strMessage, Logger.LogLevel.RELEASE)
            ' v1.1 MCF Change: Defect # 16 
            Try
                m_Socket = New TcpClient()
                m_Socket.Connect(objAppContainer.strActiveIP, ConfigDataMgr.GetInstance().GetParam( _
                                                                                 ConfigKey.IPPORT).ToString())
                If (m_Socket.Client.Connected) Then
                    bConnect = True
                End If
            Catch ex As Exception
                bConnect = False
                objAppContainer.objLogger.WriteAppLog("Could not connect to Primary IP: " + objAppContainer.strActiveIP)
            End Try
            If Not bConnect And objAppContainer.bMCFEnabled Then
                objAppContainer.objLogger.WriteAppLog("CONNECTING TO ALTERNATE CONTROLLER" + _
                                                               objAppContainer.strActiveIP, Logger.LogLevel.RELEASE)
                If objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString() Then

                    objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.SECONDARY_IPADDRESS).ToString()
                Else
                    objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString()
                End If
                m_Socket = Nothing
                m_Socket = New TcpClient()
                m_Socket.Connect(objAppContainer.strActiveIP, ConfigDataMgr.GetInstance().GetParam( _
                                                                                 ConfigKey.IPPORT).ToString())
                If (m_Socket.Client.Connected) Then
                    bConnect = True
                    objAppContainer.objLogger.WriteAppLog("CONNECTED TO ALTERNATE CONTROLLER" + _
                                                           objAppContainer.strActiveIP, Logger.LogLevel.RELEASE)
                    ConfigDataMgr.GetInstance.SetActiveIP()
                End If
            End If
            m_NetworkStream = m_Socket.GetStream()
            'new message format
            'adding 0xFF and message length to the request to be send
            '  Dim bytedata As Byte() = System.Text.Encoding.ASCII.GetBytes(strMessage)
            If bConnect Then
                strMessage = Chr(255) + (strMessage.Length + 5).ToString.PadLeft(4, "0") + strMessage
                Dim bytedata As Byte() = System.Text.Encoding.ASCII.GetBytes(strMessage)
                bytedata(0) = &HFF
                ''''''
                If (m_NetworkStream.CanWrite) Then
                    m_NetworkStream.Write(bytedata, 0, bytedata.Length)
                    'id = (id + 1) Mod &H4
                    'Dim request As New Request(id)

                    Dim RecievedResponse As String = ""

                    If (m_NetworkStream.CanRead) Then
                        Dim arrbyReadData(m_Socket.ReceiveBufferSize) As Byte
                        Dim BytesRead As Integer
                        BytesRead = m_NetworkStream.Read(arrbyReadData, 0, arrbyReadData.Length)
                        RecievedResponse = System.Text.Encoding.GetEncoding(850).GetString(arrbyReadData, 0, BytesRead)
                        'new Message Format
                        'removing the message length part from the recieved response
                        RecievedResponse = RecievedResponse.Substring(5, RecievedResponse.Length - 5)
                        ''''
                        If RecievedResponse.StartsWith("ACK") Then
                            btemp = True
                        End If
                    End If
                    objAppContainer.objLogger.WriteAppLog("LOG MESSAGE RESPONSE: " + RecievedResponse, Logger.LogLevel.RELEASE)
                End If
                m_Socket.Client.Close()
                m_Socket.Close()
                m_NetworkStream.Close()
                m_Socket = Nothing
            End If
        Catch ex As Exception
            btemp = False
        End Try
        Return btemp
    End Function
#Region "Enums"
#Region "Enum Action"
    ''' <summary>
    ''' Log Message - Current Process
    ''' </summary>
    ''' <remarks>Current Process</remarks>
    Public Enum Action
        ''' <summary>
        ''' TFTP Load
        ''' </summary>
        ''' <remarks>value : L</remarks>
        TFTP_LOAD
        ''' <summary>
        ''' TFTP Send 
        ''' </summary>
        ''' <remarks>Value : S</remarks>
        TFTP_SEND
        ''' <summary>
        ''' FTP Load 
        ''' </summary>
        ''' <remarks>Value : F</remarks>
        FTP_LOAD
        ''' <summary>
        ''' FTP Send 
        ''' </summary>
        ''' <remarks>Value: P</remarks>
        FTP_SEND
    End Enum

#End Region
#Region "Enum Status"
    ''' <summary>
    ''' Log Message - currently Procesing File Status
    ''' </summary>
    ''' <remarks>Current Process File Staus</remarks>
    Public Enum Status
        ''' <summary>
        ''' Processing of File Started 
        ''' </summary>
        ''' <remarks>value : S</remarks>
        START
        ''' <summary>
        ''' Processing of File Completed Successfully
        ''' </summary>
        ''' <remarks>value : E</remarks>
        END_OK
        ''' <summary>
        ''' Processing of File Abended
        ''' </summary>
        ''' <remarks>value : X</remarks>
        ABEND
        INFO
    End Enum
#End Region
#Region "ENUM FILE NAME"
    ''' <summary>
    ''' Log Message - currently Procesing File 
    ''' </summary>
    ''' <remarks>File Name which is currently under Process</remarks>
    Public Enum FileName
        ''' <summary>
        ''' Sync Control file  (Control File)
        ''' </summary>
        ''' <remarks>File ID : 00</remarks>
        SYNCTRL
        ''' <summary>
        ''' BootsCode.CSV File (Reference File)
        ''' </summary>
        ''' <remarks>File ID : 01</remarks>
        BOOTCODE
        ''' <summary>
        ''' Barcode.csv File (Reference File) 
        ''' </summary>
        ''' <remarks>File ID : 02</remarks>
        BARCODE
        ''' <summary>
        ''' Deal.csv File File (Reference File)
        ''' </summary>
        ''' <remarks>File ID : 03</remarks>
        DEAL
        ''' <summary>
        ''' PGGroup.csv File (Reference File)
        ''' </summary>
        ''' <remarks>File ID : 04</remarks>
        PGROUP
        ''' <summary>
        ''' Supplier.csv File (Reference File)
        ''' </summary>
        ''' <remarks>File ID : 05</remarks>
        SUPPLIER
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>File ID : 06</remarks>
        USERS
        ''' <summary>
        ''' Recall.CSV File (Reference File)
        ''' </summary>
        ''' <remarks>File ID : 07</remarks>
        RECALL
        ''' <summary>
        ''' Livepog.csv File (Reference File)
        ''' </summary>
        ''' <remarks>File ID : 08</remarks>
        LIVEPOG
        ''' <summary>
        ''' Category.csv File (Reference File)
        ''' </summary>
        ''' <remarks>File ID : 09</remarks>
        CATEGORY
        ''' <summary>
        ''' Module.csv File (Reference File)
        ''' </summary>
        ''' <remarks>File ID : 10</remarks>
        MODULES
        ''' <summary>
        ''' Shelfdes.csv File (Reference File)
        ''' </summary>
        ''' <remarks>File ID : 11</remarks>
        SHELFDES
        ''' <summary>
        ''' Control.csv File (Active File)
        ''' </summary>
        ''' <remarks>File ID : 12</remarks>
        CONTROL
        ''' <summary>
        ''' Picking.csv file (Active File)
        ''' </summary>
        ''' <remarks>File ID : 13</remarks>
        PICKING
        ''' <summary>
        ''' Count.csv File (Active File)
        ''' </summary>
        ''' <remarks>File ID : 14</remarks>
        COUNT
        ''' <summary>
        ''' Credit.csv File (Active File)
        ''' </summary>
        ''' <remarks>File ID : 15</remarks>
        CREDIT
        ''' <summary>
        ''' Directs.CSV File (Active File)
        ''' </summary>
        ''' <remarks>File ID : 16</remarks>
        DIRECTS
        ''' <summary>
        ''' ASN.CSV File. (Active File)
        ''' </summary>
        ''' <remarks>File ID : 17</remarks>
        ASN
        ''' <summary>
        ''' SSCUODOT.csv File (Active File)
        ''' </summary>
        ''' <remarks>File ID : 18</remarks>
        SSCUODOT
        ''' <summary>
        ''' All Log Files. Have to be zipped and send to controller
        ''' </summary>
        ''' <remarks>File ID : 99</remarks>
        POD_Log_Files
    End Enum
#End Region
#Region "Enum Reason"
    ''' <summary>
    ''' Reason for the error
    ''' Choose from the category or choose Other error
    ''' </summary>
    ''' <remarks>Enum to have all the errors</remarks>
    Public Enum Reasons
        Not_Applicable
        File_Build_Incomplete
        Build_Ready
        Length_Mismatch
        PKEY_Violation
        File_Not_Found
        Disconnected
        Header_Name_Mismatch
        Download_Fail
        Parse_File
        Downloading_File
        Download_Complete
        Update_Success
        Update_Error
        Parse_Complete
        Other_Errors
    End Enum
#End Region
#End Region
End Class


