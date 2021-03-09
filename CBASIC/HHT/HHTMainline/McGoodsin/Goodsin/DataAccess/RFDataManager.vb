#If RF Then


'---------------------------------------------------------------------------
' Module  : RFDataManager.vb
' Desc    : Controls Client/Server connection and transferral of messages 
'           (transactions) between the two.
' Notes   : 
'  
'-----------------------------------------------------------------------------
' Version 1.0   Dave Horrocks                                      28/04/2004
'-----------------------------------------------------------------------------
'''*******************************************************************************
''' Modification Log 
'''******************************************************************************* 
''' No:      Author            Date            Description 
''' 1.1 Christopher Kitto  09/04/2015   Modified as part of DALLAS project
'''           (CK)                      Added new functions ProcessDADBookin, 
'''                                     SendDAL and SendDAR.
'''
'''     Kiran Krishnan     27/04/2015   Added DAC request & response processing 
'''           (KK)                      as part of Dallas Positive receiving 
'''                                     project
'''********************************************************************************

Imports System.Net
Imports System.Net.Sockets
Imports System.IO
Imports System.Diagnostics
Imports Goodsin.AppContainer
Imports Goodsin.Message
Imports MCGdin.AppContainer
Imports MCGdin.Message
Imports System.Runtime.InteropServices
Imports System.Globalization

Public Class RFDataManager


    Public Structure SYSTEMTIME
        Public wYear As Short
        Public wMonth As Short
        Public wDayOfWeek As Short
        Public wDay As Short
        Public wHour As Short
        Public wMinute As Short
        Public wSecond As Short
        Public wMilliSeconds As Short
    End Structure
    <DllImport("coredll.dll")> _
    Public Shared Sub GetSystemTime(ByRef lpSystemTime As SYSTEMTIME)
    End Sub
    <DllImport("coredll.dll")> _
    Public Shared Function SetLocalTime(ByRef lpSystemTime As SYSTEMTIME) As Boolean
    End Function



    'screen to return to if NAK recieved
    Public nakReturn As UINAV = UINAV.None
    ' General purpose
    Private Shared ASCII As New System.Text.ASCIIEncoding
    '    Private Shared ASCII As New 

    ' Set up a public signal last NAK state from CLS txn   23/3/05 PAB
    Public WasNAKreceived As Boolean

    ' Socket comms control

    Private _client As RFDataConnectionMgr = New RFDataConnectionMgr
    'Private ConnectionStatus As Completion_Status = modControl.Completion_Status.Nop
    'Private SendStatus As Completion_Status = modControl.Completion_Status.Nop
    Private id As Integer = 0

    Public Shared _txn As Transaction
    Private _heartBeat As Date = Nothing
    Public strViewSequence As String
    Private strData As String
    Private strPreviosGIA As String
    Public iPointer
    ' V1.1 - CK
    ' A new string variable to hold the next record no. obtained from DAD message
    Public cNextRecordNo As String
    Public arrTempList As New ArrayList

    Public Sub New()
        strData = ""
        strPreviosGIA = ""
        iPointer = 0
    End Sub
    'to check if connection to controller exists or not
    Public Function Connected() As Boolean
        Return _client.Connected()

    End Function
    'to create a socket connection to controller on no connection
    'after retry reconnection logic fails and user uses application
    'from Goods In main menu
    Public Function ReconnectOn() As Boolean

        Connect()
        If _client.Connected() Then
            SendRCN()
            WaitForResponse(strData)
            If strData.Substring(0, 3) = "ACK" Then
                'DEFECT FIX BTCPR00004354(Goods In - out of rf range - successful reconnect 
                'within retries - 'Reconnect Successful' text has 'ACKYou have been success.)
                MessageBox.Show(strData.Substring(3, strData.Length - 3), "Reconnect Successful", _
                                       MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                       MessageBoxDefaultButton.Button1)

                objAppContainer.bCommFailure = False
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            End If
        End If
        Return False

    End Function
    'to create a socket connection to the controller
    Public Function Connect()
        _client.Connect(objAppContainer.strActiveIP, ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT))
    End Function
    'to disconnect a live socket connection to the controller
    Public Sub Disconnect()
        _client.Disconnect()
        If Not _txn Is Nothing Then
            _txn.Completion(0, "")
            _txn = Nothing
        End If
    End Sub

    Public Sub Restart()
        Disconnect()
        Connect()
    End Sub
    ''' to receive the data from 
    'Public Function Receive(ByVal ReceivedData As String) As Boolean
    Public Function Receive(ByRef ReceivedData As String) As Boolean
        Return _client.Receive(ReceivedData)
    End Function
    'Public Sub WaitForResponse(ByRef strReceivedData As String)
    '    Dim iCount As Integer = 0
    '    While Not Receive(strReceivedData) Or iCount = 10
    '        Threading.Thread.Sleep(100)
    '        iCount += 1
    '    End While
    'End Sub
    Public Function WaitForResponse(ByRef strReceivedData As String) As Boolean
        Dim bTemp As Boolean
        Dim iCount As Integer = 0
        strReceivedData = ""
        If _client.CheckTimeout() Then
            bTemp = Receive(strReceivedData)
        Else
            objAppContainer.bTimeOut = True
        End If
        'While Not bTemp And iCount < 3
        '    bTemp = Receive(strReceivedData)
        '    'if comm. fails then no need to wait for response
        '    If objAppContainer.bCommFailure Then
        '        Exit While
        '    End If
        '    Threading.Thread.Sleep(100)
        '    iCount += 1
        'End While
        'If iCount = 3 Then
        '    objAppContainer.bTimeOut = True
        'End If
        'If Not bTemp Then
        '    bTemp = CheckReconnect()
        'End If
        Return bTemp
    End Function
#Region " Transaction Listener "
    ' Process socket notification, executes on GUI's thread
    Private Sub ProcessSocketCommand(ByVal sender As NotifyCommand, ByVal data As Object) 'Handles _client.Notify
        SyncLock Me

            Dim status As String = ""
            Dim returnData As String = ""



        End SyncLock

    End Sub
#End Region
#Region " Receive Transaction Functions "
    '**************************************************************************
    ' <summary>
    ' The function Process transact message for GIB withRESPONSETYPE C
    ' </summary>
    ' <param name="objGIBCMessage"></param>
    ' <returnvalue>boolean</returnvalue>
    '<remarks></remarks>	
    '**************************************************************************

    Public Function ProcessGIBC(ByVal strGIBCMessage As String, ByRef objGIBCMessage As RFDataStructure.GIBCMessage) As Boolean
        Try
            objGIBCMessage = New RFDataStructure.GIBCMessage

            With objGIBCMessage
                .cResponseType = strGIBCMessage.Substring(GIB.RESPONSETYPE_OFFSET, GIB.RESPONSETYPE)
                .cDirectsActive = strGIBCMessage.Substring(GIB.DIRECTS_ACTIVE_OFFSET, GIB.DIRECTS_ACTIVE)
                .cUODActive = strGIBCMessage.Substring(GIB.POS_UOD_ACTIVE_OFFSET, GIB.POS_UOD_ACTIVE)
                .cASNActive = strGIBCMessage.Substring(GIB.ASN_ACTIVE_OFFSET, GIB.ASN_ACTIVE)
                .cONightDelivery = strGIBCMessage.Substring(GIB.ONIGHT_DELIV_OFFSET, GIB.ONIGHT_DELIV)
                .cONightScan = strGIBCMessage.Substring(GIB.ONIGHT_SCAN_OFFSET, GIB.ONIGHT_SCAN)
                .strBatchSize = strGIBCMessage.Substring(GIB.SCAN_BATCH_SIZE_OFFSET, GIB.SCAN_BATCH_SIZE)
            End With

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
        End Try

    End Function
    '**************************************************************************
    ' <summary>
    ' The function Process transact message for GIB with RESPONSETYPE S
    ' </summary>
    ' <param name="arrGIBSData"></param>
    ' <returnvalue>boolean</returnvalue>
    '<remarks></remarks>	
    '**************************************************************************
    Public Sub ProcessGIBS(ByVal strGIBSMessage As String, ByRef objGIBSMessage As RFDataStructure.GIBSMessage)


        Dim objGIBSummary As RFDataStructure.GIBSummary
        Dim Count As Integer
        Try
            'Initialising the GIBS message structure 
            objGIBSMessage = New RFDataStructure.GIBSMessage
            With objGIBSMessage
                ' .TransactionID = strGIBSMessage.Substring(0, 3)
                ' .OperatorID = strGIBSMessage.Substring(3, 3)
                .ResponseType = strGIBSMessage.Substring(GIB.RESPONSETYPE_OFFSET, GIB.RESPONSETYPE)
                Count = Convert.ToInt32(strGIBSMessage.Substring(GIB.COUNT_OFFSET, GIB.COUNT))
                .Count = Count
                iPointer = Convert.ToInt32(strGIBSMessage.Substring(GIB.POINTER_OFFSET, GIB.POINTER))
                .Pointer = iPointer
                For iCount As Integer = 0 To (Count - 1)
                    objGIBSummary = New RFDataStructure.GIBSummary
                    With objGIBSummary
                        .strIdentifier = strGIBSMessage.Substring(GIB.IDENTIFIER_OFFSET + iCount * GIB.TRAILERTOTAL, GIB.IDENTIFIER)
                        .strSequence = strGIBSMessage.Substring(GIB.SEQUENCE_OFFSET + iCount * GIB.TRAILERTOTAL, GIB.SEQUENCE)
                        .strName = strGIBSMessage.Substring(GIB.NAME_OFFSET + iCount * GIB.TRAILERTOTAL, GIB.NAME)
                        .cSupplierType = strGIBSMessage.Substring(GIB.SUPPLIERTYPE_OFFSET + iCount * GIB.TRAILERTOTAL, GIB.SUPPLIERTYPE)
                        .cContentType = strGIBSMessage.Substring(GIB.CONTENTTYPE_OFFSET + iCount * GIB.TRAILERTOTAL, GIB.CONTENTTYPE)
                        .strExpectedDate = strGIBSMessage.Substring(GIB.EXPECTEDDATE_OFFSET + iCount * GIB.TRAILERTOTAL, GIB.EXPECTEDDATE)
                        .cBookedIn = strGIBSMessage.Substring(GIB.BOOKEDIN_OFFSET + iCount * GIB.TRAILERTOTAL, GIB.BOOKEDIN)
                        .strQuantity = strGIBSMessage.Substring(GIB.QUANTITY_OFFSET + iCount * GIB.TRAILERTOTAL, GIB.QUANTITY)
                    End With
                    If .arrGIBSData Is Nothing Then
                        .arrGIBSData = New ArrayList
                    End If
                    .arrGIBSData.Add(objGIBSummary)
                Next
            End With
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
        End Try


    End Sub
    '**************************************************************************
    ' <summary>
    ' The function Process transact message for GIR with RESPONSETYPE B
    ' </summary>
    ' <param name="arrGIBSData"></param>
    ' <returnvalue>boolean</returnvalue>
    '<remarks></remarks>	
    '**************************************************************************

    Public Function ProcessGIRB(ByVal strGIBRMessage As String, ByRef objGIRBMessage As RFDataStructure.GIRBMessage) As Boolean

        Try
            With objGIRBMessage
                .strSelectedCode = strGIBRMessage.Substring(GIR_B.SELECTEDCODE_OFFSET, GIR_B.SELECTEDCODE)
                .cResponseType = strGIBRMessage.Substring(GIR_B.RESPONSETYPE_OFFSET, GIR_B.RESPONSETYPE)
                .strDespatchDate = strGIBRMessage.Substring(GIR_B.DESPATCHDATE_OFFSET, GIR_B.DESPATCHDATE)
                .cOuterType = strGIBRMessage.Substring(GIR_B.OUTERTYPE_OFFSET, GIR_B.OUTERTYPE)
                .cContentType = strGIBRMessage.Substring(GIR_B.CONTENTTYPE_OFFSET, GIR_B.CONTENTTYPE)
                .cUODReason = strGIBRMessage.Substring(GIR_B.UODREASON_OFFSET, GIR_B.UODREASON)
                .cStatus = strGIBRMessage.Substring(GIR_B.STATUS_OFFSET, GIR_B.STATUS)
                .cBOLUOD = strGIBRMessage.Substring(GIR_B.BOLUOD_OFFSET, GIR_B.BOLUOD)
                .strOrderNum = strGIBRMessage.Substring(GIR_B.ORDERNUM_OFFSET, GIR_B.ORDERNUM)
                .cOrderSuffix = strGIBRMessage.Substring(GIR_B.ORDERSUFFIX_OFFSET, GIR_B.ORDERSUFFIX)
                .cBusCentre = strGIBRMessage.Substring(GIR_B.BUSCENTRE_OFFSET, GIR_B.BUSCENTRE)
            End With
            ProcessGIRB = True
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            ProcessGIRB = False
        End Try


    End Function
    '**************************************************************************
    ' <summary>
    ' The function Process transact message for GIR with RESPONSETYPE S
    ' </summary>
    ' <param name="arrGIBSData"></param>
    ' <returnvalue>boolean</returnvalue>
    '<remarks></remarks>	
    '**************************************************************************

    Public Sub ProcessGIRS(ByVal strGIRSMessage As String, ByRef objGIRSMessage As RFDataStructure.GIRSMessage)
        ' objGIRSMessage = New RFDataStructure.GIRSMessage
        Try
            With objGIRSMessage
                .strSelectedCode = strGIRSMessage.Substring(GIR_S.SELECTEDCODE_OFFSET, GIR_S.SELECTEDCODE)
                .cResponseType = strGIRSMessage.Substring(GIR_S.RESPONSETYPE_OFFSET, GIR_S.RESPONSETYPE)
                .strSupplierNum = strGIRSMessage.Substring(GIR_S.SUPPLIERNO_OFFSET, GIR_S.SUPPLIERNO)
                .strSupplierName = strGIRSMessage.Substring(GIR_S.SUPPLIERNAME_OFFSET, GIR_S.SUPPLIERNAME)
                .strSupplierType = strGIRSMessage.Substring(GIR_S.SUPPLIERTYPE_OFFSET, GIR_S.SUPPLIERTYPE)
            End With
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
        End Try

    End Sub
    Public Sub ProcessGIRF(ByVal strGIRFMessage As String, ByRef objGIRFMessage As RFDataStructure.GIRFMessage, ByRef Pointer As Integer)

        Dim objGIRFData As RFDataStructure.GIRFMessageData
        Dim iCount As Integer
        Dim strPointer As String
        Try
            With objGIRFMessage
                .strSelectedCode = strGIRFMessage.Substring(GIR_F.SELECTEDCODE_OFFSET, GIR_F.SELECTEDCODE)
                .cResponseType = strGIRFMessage.Substring(GIR_F.RESPONSETYPE_OFFSET, GIR_F.RESPONSETYPE)
                .strDespatchDate = strGIRFMessage.Substring(GIR_F.DESPATCHDATE_OFFSET, GIR_F.DESPATCHDATE)
                .cOuterType = strGIRFMessage.Substring(GIR_F.OUTERTYPE_OFFSET, GIR_F.OUTERTYPE)
                .cContentType = strGIRFMessage.Substring(GIR_F.CONTENTTYPE_OFFSET, GIR_F.CONTENTTYPE)
                .cUODReason = strGIRFMessage.Substring(GIR_F.UODREASON_PART1_OFFSET, GIR_F.UODREASON_PART1)
                .cStatus = strGIRFMessage.Substring(GIR_F.STATUS_OFFSET, GIR_F.STATUS)
                .cBOLUOD = strGIRFMessage.Substring(GIR_F.BOLUOD_OFFSET, GIR_F.BOLUOD)
                .strOrderNum = strGIRFMessage.Substring(GIR_F.ORDERNUM_OFFSET, GIR_F.ORDERNUM)
                .cOrderSuffix = strGIRFMessage.Substring(GIR_F.ORDERSUFFIX_OFFSET, GIR_F.ORDERSUFFIX)
                .cBusCentre = strGIRFMessage.Substring(GIR_F.BUSCENTRE_OFFSET, GIR_F.BUSCENTRE)
                .strEstDeliveryDate = strGIRFMessage.Substring(GIR_F.ESTDELIVERYDATE_OFFSET, GIR_F.ESTDELIVERYDATE)
                .strDriverBadge = strGIRFMessage.Substring(GIR_F.DRIVERBADGE_OFFSET, GIR_F.DRIVERBADGE)
                .strDriverCheckInDate = strGIRFMessage.Substring(GIR_F.DRIVERCHECKINDATE_OFFSET, GIR_F.DRIVERCHECKINDATE)
                .strDriverCheckInTime = strGIRFMessage.Substring(GIR_F.DRIVERCHECKINTIME_OFFSET, GIR_F.DRIVERCHECKINTIME)
                .strStoreOPID = strGIRFMessage.Substring(GIR_F.STOREOPID_OFFSET, GIR_F.STOREOPID)
                .strBookInDate = strGIRFMessage.Substring(GIR_F.BOOKINDATE_OFFSET, GIR_F.BOOKINDATE)
                .strBookInTime = strGIRFMessage.Substring(GIR_F.BOOKINTIME_OFFSET, GIR_F.BOOKINTIME)
                iCount = Convert.ToInt32(strGIRFMessage.Substring(GIR_F.COUNT_OFFSET, GIR_F.COUNT))
                .iCount = iCount
                strPointer = strGIRFMessage.Substring(GIR_F.POINTER_OFFSET, GIR_F.POINTER)
                If strPointer <> Message.ENDPOINTER_STR Then
                    iPointer = Convert.ToInt32(strPointer.TrimStart("0"))
                Else
                    iPointer = Message.ENDTPOINTER
                End If
                ' to get the COUNT Number of repeated data 
                For i As Integer = 0 To iCount - 1
                    objGIRFData = New RFDataStructure.GIRFMessageData
                    With objGIRFData
                        .strIdentifier = strGIRFMessage.Substring(GIR_F.IDENTIFIER_OFFSET + i * GIR_F.TRAILERTOTAL, GIR_F.IDENTIFIER)
                        .strName = strGIRFMessage.Substring(GIR_F.NAME_OFFSET + i * GIR_F.TRAILERTOTAL, GIR_F.NAME).Trim(" ")
                        .cBookedIn = strGIRFMessage.Substring(GIR_F.BOOKEDIN_OFFSET + i * GIR_F.TRAILERTOTAL, GIR_F.BOOKEDIN)
                        .cContentType = strGIRFMessage.Substring(GIR_F.CONTENTTYPE_PART2_OFFSET + i * GIR_F.TRAILERTOTAL, GIR_F.CONTENTTYPE_PART2)
                        .strDescription = strGIRFMessage.Substring(GIR_F.DESCRIPTION_OFFSET + i * GIR_F.TRAILERTOTAL, GIR_F.DESCRIPTION)
                        .strQuantity = strGIRFMessage.Substring(GIR_F.QUANTITY_OFFSET + i * GIR_F.TRAILERTOTAL, GIR_F.QUANTITY)
                        .strSequence = strGIRFMessage.Substring(GIR_F.SEQUENCE_OFFSET + i * GIR_F.TRAILERTOTAL, GIR_F.SEQUENCE)
                    End With
                    If .arrGIRFData Is Nothing Then
                        .arrGIRFData = New ArrayList
                    End If
                    .arrGIRFData.Add(objGIRFData)
                Next

            End With
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
        End Try


    End Sub
    Public Sub ProcessEQR(ByVal strEQRMessage As String, ByRef objEQRMessage As RFDataStructure.EQRMEssage)
        objEQRMessage = New RFDataStructure.EQRMEssage
        Try
            With objEQRMessage
                .strBootsCode = strEQRMessage.Substring(EQR.BCDE_OFFSET, EQR.BCDE)
                .strParent = strEQRMessage.Substring(EQR.PARENT_OFFSET, EQR.PARENT)
                .strDescription = strEQRMessage.Substring(EQR.DESC_OFFSET, EQR.DESC)
                .strLongDescription = strEQRMessage.Substring(EQR.LONGDESC_OFFSET, EQR.LONGDESC)
                .strStatus = strEQRMessage.Substring(EQR.STATUS_OFFSET, EQR.STATUS)
                .strBarcode = strEQRMessage.Substring(EQR.BARCODE_OFFSET, EQR.BARCODE)
            End With
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
        End Try

    End Sub
    Public Function ProcessACK(ByVal strACKMessage As String) As Boolean
        Return True
    End Function

    Public Function ProcessNAK(ByVal strNAKMessage As String) As Boolean
        Return False
    End Function
    Public Function ProcessSNR(ByVal strSNRResponse As String, ByRef objSNRMessage As RFDataStructure.SNRMessage) As Boolean
        Dim strSystemTime As String
        objAppContainer.bUserSession = False
        Try
            With objSNRMessage
                .strUserID = strSNRResponse.Substring(SNR.ID_OFFSET, SNR.ID)
                .strUserName = strSNRResponse.Substring(SNR.UNAME_OFFSET, SNR.UNAME)
                strSystemTime = strSNRResponse.Substring(SNR.DATETIME_OFFSET, SNR.DATETIME)
                .cAuthorityFlag = Convert.ToChar(strSNRResponse.Substring(SNR.AUTH_OFFSET, SNR.AUTH))
                SetDeviceDateTime(strSystemTime)
            End With
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
        End Try

    End Function
    
    ' V1.1 - CK
    ' Added new function ProcessDADBookin as part of DALLAS project.

    '''**************************************************************************
    ''' <summary>
    ''' The function processes the transact message DAD and extracts the fields
    ''' </summary>
    ''' <param name="cTempData" & "arrDALLASUODList"></param>
    ''' <returnvalue>boolean</returnvalue>
    ''' <remarks></remarks>	
    '**************************************************************************

    Public Function ProcessDADBookin(ByVal cTempData As String, ByRef arrDALLASUODList As ArrayList) As Boolean

        'If Not Left(cTempData, 3) = "DAD" Then
        'Return False
        'EndIf

        cNextRecordNo = cTempData.Substring(DAD.NEXTRECORDNO_OFFSET, DAD.NEXTRECORDNO)

        Dim objDALLASDelvSummary As New GIValueHolder.DallasDeliverySummary
        With objDALLASDelvSummary
            .DallasBarcode = cTempData.Substring(DAD.DALLASBARCODE_OFFSET, DAD.DALLASBARCODE)
            .ExpectedDelDate = cTempData.Substring(DAD.EXPECTEDDELDATE_OFFSET, DAD.EXPECTEDDELDATE)
            .Status = cTempData.Substring(DAD.STATUS_OFFSET)
        End With
        arrDALLASUODList.Add(objDALLASDelvSummary)
        Return True
    End Function
#End Region
#Region " Send Transaction Functions "
    '**************************************************************************
    ' <summary>
    ' The function creates transact message for GIA and sends it to controller
    ' </summary>
    ' <param name=></param>
    ' <returnvalue>boolean</returnvalue>
    '<remarks></remarks>	
    '**************************************************************************

    Public Function SendSOR(ByVal UserName As String, ByVal Password As String) As Boolean
        ' Sign On Request (SOR)

        ' Connect to 4690 Transaction Server
        Try
            Dim lFreeMem As Long = Nothing
            Dim bTransactionSent As Boolean = False
            Dim strMacID As String = objAppContainer.objHelper.GetMacAddress()
            'Read app version from config file
            'To split the appverion and release version. Actual app version should be send in SOR
            Dim strAppVersion As String = Nothing
            Dim aReleaseVersion() As String = Nothing
            Dim strDeviceType As String = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DEVICE_TYPE)
            strAppVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
            aReleaseVersion = strAppVersion.Split("-")
            strAppVersion = aReleaseVersion(1)
            'If Not CheckReconnect() Then
            '    Return False
            'End If

            Dim sSend As String = "SOR" & _
                                    UserName & _
                                    Password & _
                                    ConfigDataMgr.GetInstance().GetParam(ConfigKey.APP_ID) & _
                                    strAppVersion.PadLeft(4, "0")
            ' sSend = "SOR111111005"
            'new Message format
            If strMacID = "" Then
                sSend += "000000000000"
            Else
                sSend += strMacID.PadLeft(12, "0")
            End If
            sSend += strDeviceType & objAppContainer.objHelper.GetIPAddress()
            sSend += objAppContainer.objHelper.CheckForFreeMemory("Program Files", lFreeMem).PadLeft(8, "0")
            objAppContainer.bUserSession = True
            _client.Send(sSend)
            bTransactionSent = True

            Return True
        Catch ex As Exception
            If Not _client.IsConnected() Then
                _client.EstablishConnection()
            End If
            Return False
        End Try


    End Function
    '**************************************************************************
    ' <summary>
    ' The function creates transact message for GIA and sends it to controller
    ' </summary>
    ' <param name="objGIAMessage"></param>
    ' <returnvalue>boolean</returnvalue>
    '<remarks></remarks>	
    '**************************************************************************

    Public Function SendGIA(ByRef objGIAMessage As RFDataStructure.GIAMessage) As Boolean
        'objGIAMessage = New RFDataStructure.GIAMessage
        ' GIA Transaction (GIA)
        Dim bTransactionSent As Boolean = False
        Try
            'If Not CheckReconnect() Then
            '    Return False
            'End If
            'If Not _client.Connected() Then
            '    _client.EstablishConnection()
            '    Return False
            'End If


            Dim sSend As String = "GIA" & _
                                    objAppContainer.strCurrentUserID
            With objGIAMessage

                If .eType = RFDataStructure.DeliveryType.None Then
                    sSend += "X"
                Else
                    sSend += CType(objGIAMessage.eType, Integer).ToString()
                End If
                If .eFunc = RFDataStructure.GFunction.None Then
                    sSend += "X"
                Else
                    sSend += CType(objGIAMessage.eFunc, Integer).ToString()
                End If
                If .eRequestType = RFDataStructure.GIARequestType.Configuration Then
                    sSend += "C"
                ElseIf .eRequestType = RFDataStructure.GIARequestType.Summary Then
                    sSend += "S"
                ElseIf .eRequestType = RFDataStructure.GIARequestType.List Then
                    sSend += "L"
                End If
                If .ePeriod = RFDataStructure.GPeriod.Future Then
                    sSend += "F"
                ElseIf .ePeriod = RFDataStructure.GPeriod.Today Then
                    sSend += "T"
                Else
                    sSend += "X"
                End If
                sSend += .iPointer.ToString().PadLeft(6, "0")
            End With
            ' Dim bytes() As Byte = ASCII.GetBytes(sSend)

            Return _client.Send(sSend)

        Catch ex As Exception
            'If Not _client.IsConnected() Then
            '    _client.EstablishConnection()
            'End If
            Return False
        End Try

    End Function
    '**************************************************************************
    ' <summary>
    ' The function creates transact message for GIQ and sends it to controller
    ' </summary>
    ' <param name="objGIQMessage"></param>
    ' <returnvalue>boolean</returnvalue>
    '<remarks></remarks>	
    '**************************************************************************

    Public Function SendGIQ(ByRef objGIQMessage As RFDataStructure.GIQMessage) As Boolean
        'objGIQMessage = New RFDataStructure.GIQMessage
        ' Enquiry Transaction (GIQ)
        Dim bTransactionSent As Boolean = False
        Try
            'If Not CheckReconnect() Then
            '    Return False
            'End If
            'If Not _client.Connected() Then
            '    _client.EstablishConnection()
            '    Return False
            'End If

            Dim sSend As String = "GIQ" & _
                                    objAppContainer.strCurrentUserID
            'DELIVERY TYPE
            sSend += CType(objGIQMessage.eType, Integer).ToString()
            'FUNCTION TYPE
            sSend += CType(objGIQMessage.eFunc, Integer).ToString()
            'SELECTED CODE
            If objGIQMessage.eType = DeliveryType.Directs AndAlso _
             objGIQMessage.eFunc = FunctionType.BookIn Then
                sSend += objGIQMessage.strSelectedCode.PadRight(20, "0")
            Else
                sSend += objGIQMessage.strSelectedCode.PadLeft(20, "0")
            End If

            'SEQUENCE
            If objGIQMessage.eType = DeliveryType.ASN Or objGIQMessage.eType = DeliveryType.Directs Then
                sSend += "XXXXX"
            ElseIf objGIQMessage.strSequence Is Nothing Then
                sSend += "00000"
            Else
                sSend += objGIQMessage.strSequence.PadLeft(5, "0")
            End If
            'sSend += IIf(objGIQMessage.strSequence Is Nothing, "00000", objGIQMessage.strSequence.PadLeft(5, "0"))
            'REQUEST TYPE

            Select Case objGIQMessage.eRequestType
                Case RFDataStructure.GIQRequestType.BookingIn
                    sSend += "B"
                Case RFDataStructure.GIQRequestType.FullSummary
                    sSend += "F"
                Case RFDataStructure.GIQRequestType.SupplierQuery
                    sSend += "S"
            End Select
            'CONTAINER TYPE

            Select Case objGIQMessage.eContType
                Case RFDataStructure.ContentType.Container
                    sSend += "C"
                Case RFDataStructure.ContentType.Item
                    sSend += "I"
            End Select

            'SUPPLIER TYPE


            If objGIQMessage.eSupType = RFDataStructure.SupplierType.NoSupplier Then
                sSend += "X"
            ElseIf objGIQMessage.eSupType = RFDataStructure.SupplierType.ASN Then
                sSend += "A"
            ElseIf objGIQMessage.eSupType = RFDataStructure.SupplierType.Directs Then
                sSend += "D"
            Else
                sSend += "X"
            End If
            sSend += objGIQMessage.iPointer.ToString().PadLeft(6, "0")
            ' Dim bytes() As Byte = ASCII.GetBytes(sSend)
            Return _client.Send(sSend)
        Catch ex As Exception
            If Not _client.IsConnected() Then
                _client.EstablishConnection()
            End If
            Return False
        End Try

    End Function
    '**************************************************************************
    ' <summary>
    ' The function creates transact message for GIF and sends it to controller
    ' </summary>
    ' <param name="objGIFMessage"></param>
    ' <returnvalue>boolean</returnvalue>
    '<remarks></remarks>	
    '**************************************************************************

    Public Function SendGIF(ByRef objGIFMessage As RFDataStructure.GIFMessage) As Boolean
        'objGIFMessage = New RFDataStructure.GIFMessage
        ' Enquiry Transaction (GIQ)
        Dim bTransactionSent As Boolean = False
        Try
            'If Not CheckReconnect() Then
            '    Return False
            'End If
            'If Not _client.IsConnected() Then
            '    _client.EstablishConnection()
            '    Return False
            'End If

            Dim sSend As String = "GIF" & _
                                    objAppContainer.strCurrentUserID
            'Delivery Type
            sSend += CType(objGIFMessage.eType, Integer).ToString()
            'Function Type
            sSend += CType(objGIFMessage.eFunc, Integer).ToString()
            If objGIFMessage.strScanCode = Nothing Then
                sSend += "X".PadLeft(20, "X")
            Else
                sSend += objGIFMessage.strScanCode.PadLeft(20, "0")
            End If

            'Despatch Date
            If objGIFMessage.strDespatchDate = Nothing Then
                sSend += "000000"
            Else
                sSend += objGIFMessage.strDespatchDate
            End If

            'Scan Type
            If objGIFMessage.eSType = AppContainer.ScanType.BookInScan Then
                sSend += "B"
            ElseIf objGIFMessage.eSType = AppContainer.ScanType.AuditScan Then
                sSend += "A"
            ElseIf objGIFMessage.eSType = AppContainer.ScanType.RetMisdirect Then
                sSend += "N"
            ElseIf objGIFMessage.eSType = AppContainer.ScanType.LateUODDetails Then
                sSend += "L"
            ElseIf objGIFMessage.eSType = AppContainer.ScanType.BatchConfirm Then
                sSend += "C"
            ElseIf objGIFMessage.eSType = AppContainer.ScanType.DeliveryConfirm Then
                sSend += "S"
            Else
                sSend += "X"
            End If
            'Scan Level
            If objGIFMessage.eSLevel = RFDataStructure.ScanLevel.DeliveryScan Then
                sSend += "D"
            ElseIf objGIFMessage.eSLevel = RFDataStructure.ScanLevel.ItemScan Then
                sSend += "I"
            ElseIf objGIFMessage.eSLevel = RFDataStructure.ScanLevel.NotUsed Then
                sSend += "X"
            Else
                sSend += "X"
            End If
            'Scan Date
            If objGIFMessage.strScanDate = Nothing Then
                sSend += "XXXXXXXX"
            Else
                sSend += objGIFMessage.strScanDate
            End If
            'Scan Time
            If objGIFMessage.strScanTime = Nothing Then
                sSend += "XXXXXX"
            Else
                sSend += objGIFMessage.strScanTime
            End If
            'Driver Badge
            If objGIFMessage.strDriverBadge = Nothing Then
                sSend += "XXXXXXXX"
            Else
                sSend += objGIFMessage.strDriverBadge.PadLeft(8, "0")
            End If

            'GIT Note Match Flag
            If objGIFMessage.cGITNote = Nothing Then
                sSend += " "
            Else
                sSend += objGIFMessage.cGITNote
            End If

            'Batch Rescan Flag
            If objGIFMessage.cBatRescan = Nothing Then
                sSend += "X"
            Else
                sSend += objGIFMessage.cBatRescan
            End If


            'Barcode
            If objGIFMessage.strBarcode = Nothing Then
                sSend += "X".PadLeft(13, "X")
            Else
                sSend += objGIFMessage.strBarcode.PadLeft(13, "0")
            End If
            'QUantity
            If objGIFMessage.strQuantity = Nothing Then
                sSend += "X".PadLeft(6, "X")
            Else
                sSend += objGIFMessage.strQuantity.PadLeft(6, "0")
            End If
            If objGIFMessage.strItemStatus = Nothing Then
                sSend += "XXXXX"
            Else
                sSend += objGIFMessage.strItemStatus.PadLeft(5, "0")
            End If


            'Sequence Number 
            If objGIFMessage.strSequence = Nothing Then
                sSend += "XX"
            Else
                sSend += objGIFMessage.strSequence.PadLeft(2, "0")
            End If
            objAppContainer.objLogger.WriteAppLog(sSend, Logger.LogLevel.DEBUG)

            ' Dim bytes() As Byte = ASCII.GetBytes(sSend)

            Return _client.Send(sSend)

        Catch ex As Exception
            If Not _client.IsConnected() Then
                _client.EstablishConnection()
            End If
            Return False
        End Try
    End Function
    Public Function SendGIX(ByRef objGIXMessage As RFDataStructure.GIXMessage)
        Dim bTransactionSent As Boolean = False
        Try
            'If Not CheckReconnect() Then
            '    Return False
            'End If
            'If Not _client.IsConnected() Then
            '    _client.EstablishConnection()
            '    Return False
            'End If

            Dim sSend As String = "GIX" & _
                                    objAppContainer.strCurrentUserID
            'Delivery Type
            sSend += CType(objGIXMessage.eDeliveryType, Integer).ToString()
            'Function Type
            sSend += CType(objGIXMessage.eFunction, Integer).ToString()
            'is abort or not
            If objGIXMessage.eIsAbort = IsAbort.No Then
                sSend += "N"
            Else
                sSend += "Y"
            End If

            'Convert string message to abyte format
            'Dim bytes() As Byte = ASCII.GetBytes(sSend)
            'send the message in byte format
            Return _client.Send(sSend)
        Catch ex As Exception
            If Not _client.IsConnected() Then
                _client.EstablishConnection()
            End If
            Return False
        End Try
    End Function


    '**************************************************************************
    ' <summary>
    ' The function creates transact message for ENQ and sends it to controller
    ' </summary>
    ' <param name=""></param>
    ' <returnvalue>boolean</returnvalue>
    '<remarks></remarks>	
    '**************************************************************************
    Public Function SendENQ( _
        ByVal Type As RFDataStructure.ENQType, _
        ByVal Func As RFDataStructure.ENQFunction, _
        ByVal Barcode As String, _
        ByVal StkFigReq As Boolean, Optional ByVal OSSRFLag As String = "", Optional ByVal Planner As String = "") As Boolean

        ' Enquiry Transaction (ENQ)
        Dim bTransactionSent As Boolean = False
        Try
            'If Not CheckReconnect() Then
            '    Return False
            'End If
            'If Not _client.IsConnected() Then
            '    _client.EstablishConnection()
            '    Return False
            'End If

            Dim sSend As String = "ENQ" & _
                                    objAppContainer.strCurrentUserID
            sSend += "I"
            sSend += " "
            sSend += Barcode.PadLeft(13, "0")
            sSend += "N"
            sSend += " "
            sSend += " "
            '  Dim bytes() As Byte = ASCII.GetBytes(sSend)


            Return _client.Send(sSend)
        Catch ex As Exception
            If Not _client.IsConnected() Then
                _client.EstablishConnection()
            End If
            Return False
        End Try

    End Function
    '**************************************************************************
    ' <summary>
    ' The function creates transact message for OFF and sends it to controller
    ' </summary>
    ' <param name=""></param>
    ' <returnvalue>boolean</returnvalue>
    '<remarks></remarks>	
    '**************************************************************************

    Public Function SendOFF() As Boolean
        ' Sign Off (OFF)
        Dim bTransactionSent As Boolean = False
        Try
            Dim sSend As String = "OFF" & _
                                    objAppContainer.strCurrentUserID

            Return _client.Send(sSend)
        Catch ex As Exception
            If Not _client.IsConnected() Then
                _client.EstablishConnection()
            End If
            Return False
        End Try

    End Function
    Public Function ProcessGIB(ByVal strReceivedData As String) As Boolean
        strData = strReceivedData
        Dim bReturn As Boolean = False
        Select Case strData.Substring(6, 1)
            Case CType(RFDataStructure.DeliveryType.SSCReceiving, Integer).ToString()
                Return ProcessGIB_SSC()

            Case Else
                Return bReturn
        End Select
    End Function
    Public Function ProcessGIB_SSC() As Boolean
        Select Case strData.Substring(7, 1)
            Case RFDataStructure.GFunction.BookIn.ToString()
                Select Case strData.Substring(8, 1)
                    Case "S"
                        ProcessSSCBookin()
                        Return True
                    Case Else
                        Return False
                End Select
            Case RFDataStructure.GFunction.Audit.ToString()
                Select Case strData.Substring(8, 1)
                    Case "S"
                        Return True
                    Case Else
                        Return False
                End Select
            Case RFDataStructure.GFunction.View.ToString()
                Select Case strData.Substring(8, 1)
                    Case "L"

                        Select Case strData.Substring(9, 1)
                            Case "T"
                                ProcessSSCView("T")
                                Return True
                            Case "F"
                                ProcessSSCView("F")
                                Return True
                            Case Else
                                Return False
                        End Select
                    Case Else
                        Return False
                End Select
            Case Else
                Return False
        End Select
    End Function
    Public Sub ProcessSSCBookin()
        Dim arrList As New ArrayList
        Dim strPointer As String
        Dim iCount As Integer = 0
        ' objAppContainer.objDataEngine.GetBookInDeliverySummary(arrList)
        Dim strResponse As String = ""
        iCount = Convert.ToInt32(strData.Substring(7, 2).TrimStart("0"))
        strPointer = strData.Substring(9, 6)
        If strPointer <> Message.ENDPOINTER_STR Then
            iPointer = Convert.ToInt32(strPointer.TrimStart("0"))
        Else
            iPointer = -1
        End If
        For iCounter As Integer = 0 To iCount - 1
            Dim objDelvSummary As New GIValueHolder.DeliverySummary
            With objDelvSummary
                .ContainerQty = strData.Substring(67 + 52 * iCounter, 6)
                .ContainerType = strData.Substring(30 + 52 * iCounter, 20).Trim(" ")
                .SummaryType = strData.Substring(15 + 52 * iCounter, 10).TrimStart(" ")
                '.ExpectedDate = strData.Substring(60 + 52 * iCounter, 6)
                '.BookedIn = strData.Substring(66 + 52 * iCounter, 1)
            End With
            arrTempList.Add(objDelvSummary)
        Next
    End Sub
    ' Public Function ProcessGIBSSCBookin(ByVal strTempData As String) As Boolean
    Public Function ProcessGIBSSCBookin(ByVal strTempData As String, ByRef arrUODList As ArrayList) As Boolean
        'Dim arrList As New ArrayList
        Dim strPointer As String
        Dim iCount As Integer = 0
        ' objAppContainer.objDataEngine.GetBookInDeliverySummary(arrList)
        'Dim strResponse As String = ""
        If Not Left(strTempData, 3) = "GIB" Then
            Return False
        End If
        iCount = Convert.ToInt32(strTempData.Substring(7, 2))
        strPointer = strTempData.Substring(9, 6)
        If strPointer <> Message.ENDPOINTER_STR Then
            iPointer = Convert.ToInt32(strPointer.TrimStart("0"))
        Else
            iPointer = -1
        End If
        'If arrTempList.Count > 0 Then
        '    arrTempList.Clear()
        'End If
        If arrUODList.Count > 0 Then
            arrUODList.Clear()
        End If
        For iCounter As Integer = 0 To iCount - 1
            Dim objDelvSummary As New GIValueHolder.DeliverySummary
            With objDelvSummary
                .ContainerQty = CType(strTempData.Substring(GIB.QUANTITY_OFFSET + GIB.TRAILERTOTAL * iCounter, GIB.QUANTITY), Integer)
                .ContainerType = strTempData.Substring(GIB.NAME_OFFSET + GIB.TRAILERTOTAL * iCounter, 1).Trim(" ")
                .SummaryType = strTempData.Substring(GIB.IDENTIFIER_OFFSET + GIB.TRAILERTOTAL * iCounter, GIB.IDENTIFIER).Trim(" ")
                ' date recieved in YYYMMDD --------------------------------------------------------------
                ' .ExpectedDate = strTempData.Substring(52 + 52 * iCounter, 8)
                ' .BookedIn = strTempData.Substring(60 + 52 * iCounter, 1)
            End With
            ' arrTempList.Add(objDelvSummary)
            arrUODList.Add(objDelvSummary)
        Next
        Return True
    End Function

    Public Function ProcessGIBSSCView(ByVal strTempData As String, ByRef arrUODList As ArrayList) As Boolean
        Dim strPointer As String
        Dim iCount As Integer = 0
        Dim strStatus As String
        Try
            If Not Left(strTempData, 3) = "GIB" Then
                Return False
            End If
            iCount = Convert.ToInt32(strTempData.Substring(7, 2).TrimStart("0"))
            strPointer = strTempData.Substring(9, 6)
            If strPointer <> Message.ENDPOINTER_STR Then
                iPointer = Convert.ToInt32(strPointer.TrimStart("0"))
            Else
                iPointer = Message.ENDTPOINTER
            End If
            For iCounter As Integer = 0 To iCount - 1
                Dim objUODList As New GIValueHolder.UODList
                With objUODList
                    .UODType = strTempData.Substring(GIB.NAME_OFFSET + GIB.TRAILERTOTAL * iCounter, GIB.NAME).Trim(" ")
                    .UODID = strTempData.Substring(GIB.IDENTIFIER_OFFSET + GIB.TRAILERTOTAL * iCounter, GIB.IDENTIFIER).TrimStart(" ")

                    strStatus = strTempData.Substring(GIB.BOOKEDIN_OFFSET + GIB.TRAILERTOTAL * iCounter, GIB.BOOKEDIN)
                    Select Case strStatus
                        Case "B"
                            .BookedIn = "Y"
                        Case "U"
                            .BookedIn = "N"
                        Case "A"
                            .BookedIn = "A"
                            'Fix for Partial UOD
                        Case "P"
                            .BookedIn = "N"
                    End Select
                    .ExptDate = strTempData.Substring(GIB.EXPECTEDDATE_OFFSET + 2 + GIB.TRAILERTOTAL * iCounter, GIB.EXPECTEDDATE - 2)
                    .Sequencenumber = strTempData.Substring(GIB.SEQUENCE_OFFSET + GIB.TRAILERTOTAL * iCounter, GIB.SEQUENCE)
                    '.Reason = strTempData.Substring(GIB.
                End With
                arrUODList.Add(objUODList)
            Next
        Catch ex As Exception
            '-------------------------------------------------Log goes here
            Return False
        End Try
        Return True
    End Function
    Public Sub ProcessSSCView(ByVal strPeriod As String)
        Dim iPointer As Integer = 0
        Dim iArraylistCount As Integer = 0

        Try


            Dim strResponse As String = ""
            strResponse = Left(strData, 6)
            strResponse += "S"
            strResponse += IIf((iArraylistCount - iPointer) <= 20, "000020", (iArraylistCount - iPointer).ToString().PadLeft(6, "0"))
            strResponse += IIf((iArraylistCount - iPointer) <= 20, Message.ENDPOINTER_STR, (iPointer + 20).ToString().PadLeft(6, "0"))  '"0000-1" 'DD say's send -1

            For i As Integer = iPointer To iArraylistCount
                Dim objUODList As GIValueHolder.UODList '= m_arrUODList(i)
                strResponse += objUODList.UODID.PadLeft(10, "0")
                'need to verify sequence number ------------------------------
                strResponse += "XXXXX" '5
                strResponse += objUODList.UODType.PadLeft(20, " ")
                strResponse += "X"
                'This value depends on the container, passing C for time being -------------------
                strResponse += "C"
                strResponse += objUODList.ExptDate
                strResponse += objUODList.BookedIn
                strResponse += "XXXXXX"
                i += 1
                If ((i - iPointer) = 20) Then
                    strPreviosGIA = strData
                    Exit For
                ElseIf (iArraylistCount = i) Then
                    strPreviosGIA = ""
                    Exit For
                End If
            Next
        Catch ex As Exception

        End Try
        'Hope the below statement is not required
        'strResponse.PadRight(1055," ")
        '  objServer.SendMessage(strResponse)
    End Sub

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
            .wYear = Convert.ToInt16(strDateTime.Substring(0, 4))
            .wMonth = Convert.ToInt16(strDateTime.Substring(4, 2))
            .wDay = Convert.ToInt16(strDateTime.Substring(6, 2))
            .wHour = Convert.ToInt16(strDateTime.Substring(8, 2))
            .wMinute = Convert.ToInt16(strDateTime.Substring(10, 2))
            .wSecond = Convert.ToInt16(0)
        End With

        'Set the new time`
        Return SetLocalTime(objSysTime)
    End Function
    '**************************************************************************
    ' <summary>
    ' The function creates transact message for RCN Reconnect and sends it to controller
    ' </summary>
    ' <param name=""></param>
    ' <returnvalue>boolean</returnvalue>
    '<remarks></remarks>	
    '**************************************************************************

    Public Function SendRCN() As Boolean
        Try
            Dim lFreeMem As Long = Nothing
            Dim bTransactionSent As Boolean = False
            Dim strMacID As String = objAppContainer.objHelper.GetMacAddress()
            'Read app version from config file
            'To split the appverion and release version. Actual app version should be send in SOR
            Dim strAppVersion As String = Nothing
            Dim aReleaseVersion() As String = Nothing
            Dim strDeviceType As String = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DEVICE_TYPE)
            strAppVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
            aReleaseVersion = strAppVersion.Split("-")
            strAppVersion = aReleaseVersion(1)
            Dim sSend As String = "RCN" & _
                                   objAppContainer.strCurrentUserID & _
                                   objAppContainer.strUser
            sSend += ConfigDataMgr.GetInstance().GetParam(ConfigKey.APP_ID) & strAppVersion.PadLeft(4, "0")
            If strMacID = "" Then
                sSend += "000000000000"
            Else
                sSend += strMacID.PadLeft(12, "0")
            End If
            sSend += strDeviceType & objAppContainer.objHelper.GetIPAddress()
            sSend += objAppContainer.objHelper.CheckForFreeMemory("Program Files", lFreeMem).PadLeft(8, "0")
            sSend += "000"
            objAppContainer.objLogger.WriteAppLog("Send RCN:" + sSend, Logger.LogLevel.RELEASE)
            '  Dim bytes() As Byte = ASCII.GetBytes(sSend)
            _client.Send(sSend)
            bTransactionSent = True

            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function
    Public Function CheckReconnect(Optional ByVal bCheckReconnect As Boolean = False) As Boolean
        Dim strRecieve As String = "NAK"
        Dim bTemp As Boolean = True
        '  Return _client.ModuleReconnect()
        Try

            If Not _client.Connected() Or bCheckReconnect Then
                If _client.ModuleReconnect() Then
                    If objAppContainer.bUserSession Then
                        objAppContainer.bReconnectSuccess = True
                        Return False
                    Else
                        objAppContainer.bUserSession = False
                        If SendRCN() Then
                            If WaitForResponse(strRecieve) Then
                                'TIMEOUT
                                objAppContainer.bRetryAtTimeout = False
                                objAppContainer.bTimeOut = False
                                If strRecieve.Substring(0, 3) = "ACK" Then
                                    If objAppContainer.iConnectedToAlternate <> 1 Then
                                        MessageBox.Show(strRecieve.Substring(3, strRecieve.Length - 3), "Reconnect Successful", _
                                                               MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                                               MessageBoxDefaultButton.Button1)
                                    Else
                                        objAppContainer.objLogger.WriteAppLog("Successfully auto logged in to the Alternate Controller", _
                                                                               Logger.LogLevel.RELEASE)
                                    End If
                                    objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                                    objAppContainer.objLogger.WriteAppLog("Reconnect Successful", Logger.LogLevel.RELEASE)
                                    objAppContainer.bReconnectSuccess = True
                                    objAppContainer.bCommFailure = False
                                    objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                                    ''checking if any details of item/cartons are saved during connection loss
                                    If objAppContainer.m_FinishedDetails.Count > 0 Then
                                        objAppContainer.objLogger.WriteAppLog("Reconnect Successful: Sending GIF for saved items or cartons", Logger.LogLevel.RELEASE)
                                        'sending the saved item/carton details on reconnection success
                                        For Each objGIFMessage As RFDataStructure.GIFMessage In objAppContainer.m_FinishedDetails
                                            Cursor.Current = Cursors.WaitCursor
                                            If SendGIF(objGIFMessage) Then
                                                If WaitForResponse(strRecieve) Then
                                                    If strRecieve.Substring(0, 3) = "ACK" Then
                                                        bTemp = True
                                                    Else
                                                        bTemp = False
                                                        ' objAppContainer.m_SaveDetails.Remove(objGIFMessage)
                                                    End If
                                                Else
                                                    Cursor.Current = Cursors.Default
                                                    If objAppContainer.bTimeOut Then
                                                        _client.HandleTimeOut()
                                                    End If
                                                    bTemp = False
                                                    Exit For
                                                End If
                                            Else
                                                Cursor.Current = Cursors.Default
                                                CheckReconnect()
                                                bTemp = False
                                                Exit For
                                            End If
                                        Next


                                        Cursor.Current = Cursors.Default
                                        If objAppContainer.m_FinishedDetails.Count > 0 Then
                                            objAppContainer.m_FinishedDetails.Clear()
                                        End If
                                        If objAppContainer.bTimeOut Then
                                            Return False
                                        End If
                                    End If
                                    'Sending Sessing session exit on reconnection success
                                    If objAppContainer.m_SavedDetails.Count = 0 Then
                                        If SendGIX(objAppContainer.objSaveGIXMessage) Then
                                            If WaitForResponse(strRecieve) Then
                                                If strRecieve.Substring(0, 3) = "ACK" Then
                                                    objAppContainer.objSaveGIXMessage = New RFDataStructure.GIXMessage
                                                    objAppContainer.objSavedGIXMessage = New RFDataStructure.GIXMessage
                                                    bTemp = True
                                                Else
                                                    bTemp = False
                                                    ' objAppContainer.m_SaveDetails.Remove(objGIFMessage)
                                                End If
                                            Else
                                                Cursor.Current = Cursors.Default
                                                If objAppContainer.bTimeOut Then
                                                    _client.HandleTimeOut()
                                                End If
                                                Return False
                                            End If
                                        Else
                                            Cursor.Current = Cursors.Default
                                            CheckReconnect()
                                            Return False
                                        End If
                                    End If
                                    Return bTemp
                                ElseIf strRecieve.Substring(0, 3) = "NAK" Then
                                    Dim strMessage As String = strRecieve.Substring(3, strRecieve.Length - 3)
                                    If strMessage <> "" Then
                                        MessageBox.Show(strMessage, "Error while Connecting", _
                                                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                                    MessageBoxDefaultButton.Button1)


                                    Else
                                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M64"), "Error while Connecting", _
                                                  MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                                  MessageBoxDefaultButton.Button1)
                                    End If
                                    objAppContainer.bCommFailure = True
                                    objAppContainer.bReconnectSuccess = False
                                    Return False
                                End If
                            Else
                                If objAppContainer.bTimeOut Then
                                    _client.HandleTimeOut()
                                End If
                                Return False
                            End If
                        Else
                            CheckReconnect()
                        End If

                    End If
                Else
                    Return False
                End If
            Else
                If objAppContainer.bTimeOut Then
                    _client.HandleTimeOut()
                End If
                Return False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("CheckReconnect : Exception" + ex.Message + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
        End Try
    End Function
    Public Function CheckReconnect1() As Boolean
        Dim strRecieve As String = "NAK"
        If Not _client.Connected() Then
            'If Not _client.IsConnected() Then
            If _client.EstablishConnection() Then
                objAppContainer.bCommFailure = False
                If objAppContainer.bUserSession Then
                    objAppContainer.bReconnectSuccess = True
                    Return False
                Else
                    objAppContainer.bUserSession = False
                    SendRCN()

                    WaitForResponse(strRecieve)
                    If strRecieve.Substring(0, 3) = "ACK" Then
                        MessageBox.Show(strRecieve.Substring(3, strRecieve.Length - 3), "Reconnect Successful", _
                                               MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                               MessageBoxDefaultButton.Button1)
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                        objAppContainer.objLogger.WriteAppLog("Reconnect Successful", Logger.LogLevel.RELEASE)
                        objAppContainer.bReconnectSuccess = True
                        objAppContainer.bCommFailure = False
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                    ElseIf strRecieve.Substring(0, 3) = "NAK" Then
                        Dim strMessage As String = strRecieve.Substring(3, strRecieve.Length - 3)
                        If strMessage <> "" Then
                            MessageBox.Show(strMessage, "Error while Connecting", _
                                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)


                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M64"), "Error while Connecting", _
                                      MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                      MessageBoxDefaultButton.Button1)
                        End If
                        objAppContainer.bCommFailure = True
                        objAppContainer.bReconnectSuccess = False

                        Return True
                    End If
                    Return True
                End If


            Else
                Return False
            End If
        Else
            If objAppContainer.bTimeOut Then
                _client.HandleTimeOut()
            End If
            Return True

        End If

    End Function

    ' V1.1 - CK
    ' Added new function SendDAL as part of DALLAS project.

    '''**************************************************************************
    ''' <summary>
    ''' The function creates transact message for DAL and sends it to controller
    ''' </summary>
    ''' <param name="m_NextRecordNo"></param>
    ''' <returnvalue>boolean</returnvalue>
    ''' <remarks></remarks>	
    '**************************************************************************

    Public Function SendDAL(ByVal cNextRecordNo As String) As Boolean

        Try
            Dim cSend As String = "DAL" & _
                                    objAppContainer.strCurrentUserID
            cSend += cNextRecordNo

            Return _client.Send(cSend)

        Catch ex As Exception
            Return False
        End Try

    End Function

    ' V1.1 - CK
    ' Added new function SendDAR as part of DALLAS project.

    '''**************************************************************************
    ''' <summary>
    ''' The function creates transact message DAR and sends it to controller
    ''' </summary>
    ''' <param name="objDARMessage"></param>
    ''' <returnvalue>boolean</returnvalue>
    ''' <remarks></remarks>	
    '**************************************************************************

    Public Function SendDAR(ByVal objDARMessage As RFDataStructure.DARMessage) As Boolean
        Dim bTransactionSent As Boolean = False
        Try
            ' Transaction Id
            Dim cSend As String = "DAR"

            ' Appending operator ID
            cSend += objAppContainer.strCurrentUserID

            ' Appending scanned Dallas barcode
            cSend += objDARMessage.cDallasBarcode

            ' Appending scanned date
            cSend += objDARMessage.cScanDate

            ' Appending Scan Status
            cSend += objDARMessage.cScanStatus

            objAppContainer.objLogger.WriteAppLog(cSend, Logger.LogLevel.DEBUG)

            Return _client.Send(cSend)

        Catch ex As Exception
            If Not _client.IsConnected() Then
                _client.EstablishConnection()
            End If
            Return False
        End Try

    End Function
    
    ' V1.1 - KK
    ' Added new function as part of DALLAS Positive Receiving project.

    '''**************************************************************************
    ''' <summary>
    ''' The function creates transact message for DAC request and sends it to 
    ''' controller. The DAC request is used to check whether a store is 
    ''' Dallas Positive Receiving or not.
    ''' </summary>
    ''' <param name></param>
    ''' <returnvalue>boolean</returnvalue>
    ''' <remarks></remarks>	
    '***************************************************************************

    Public Function SendDAC() As Boolean

        Try
            Dim cSend As String = "DAC" & _
                                    objAppContainer.strCurrentUserID

            Return _client.Send(cSend)

        Catch ex As Exception
            Return False
        End Try

    End Function

    'end
#End Region
#Region " Send Data Functions "

#End Region
#Region " Transaction Class "

    Public Class Transaction

        'Static Variables
        Private Shared Uid As Long = 0
        Private Shared Outstanding As Long = 0

        'Member Variables
        Private Unique As Integer

        Public ID As TxnCommand
        Public Status As TxnStatus
        Public StatusRc As Long
        Public Resp As String
        Public Completed As Boolean
        Public Started As Date

        Public Sub New(ByVal TxnIdentifier As TxnCommand, _
                        ByVal TxnStatusCode As TxnStatus)

            ID = TxnIdentifier
            Uid = (Uid + 1) Mod &H10000000
            Unique = Uid
            Status = TxnStatusCode
            StatusRc = 0
            Resp = ""
            Completed = False
            Started = Now()

            ' Track number of outstanding requests
            Outstanding += 1

        End Sub

        Public Sub Completion(ByVal Rc As Long, _
                                Optional ByVal Response As String = "")
            StatusRc = Rc
            Resp = Response
            Completed = True
            Outstanding -= 1
        End Sub

    End Class

#End Region     'Store for tracking pending communications operation

End Class
#Region " Transaction Commands "
' Transaction command types
Public Enum TxnCommand
    Connect
    ENQ
    NOP
    OFF
    SOR
    XXX
    GIA
    GIQ
    GIF
    GIX
    'end
    LPR         'A8A Mobile Printing Phase-1 - Charles Skadorwa 12/09/2007
End Enum
#End Region

#Region " Transaction Status "
' Transaction progression status codes
Public Enum TxnStatus
    NOP
    PENDING
    SUCCESS
    FAILURE
End Enum
#End Region
#End If