'******************************************************************************
' Modification Log 
'******************************************************************************* 
' No:      Author            Date            Description 
' 1.1     Christopher Kitto  09/04/2015   Modified as part of DALLAS project.
'           (CK)                          Amended the definition of the function
'                                         GetBookInDeliverySummary()to get DALLAS 
'                                         UOD details by sending DAL message and
'                                         processing the DAD response message.
'                                         Added new function SendDallasDeliveryConfirmation()
' 
'         Kiran Krishnan     28/04/2015   Added CheckDallasStore() function 
'           (KK)                          definition. This function to be used 
'                                         in RF mode. Added new function GetDallasUODListForView
'                                         for processing Dallas UOD data for View UOD Menu.
' 1.2     Charles Skadorwa   04/08/2016   Modified as part of Order & Collect project.
'           (CS)                          Suppress Boots.com supplier from displaying - see 
'                                         function GetSupplierList() and 
'                                         GetSupplierListForView(). 
'********************************************************************************

Imports Goodsin.GIValueHolder
Imports Goodsin.AppContainer

#If RF Then


Public Class RFDataSource
    Inherits DataEngine

    Private strOrderNumber As String = ""
    Private strData As String
    Private strPreviosGIA As String
    Private arrOrderList As New ArrayList
    Private strViewSequence As String
    Dim iPointer As Integer = 0
    Dim strSequenceNum As String = ""
    Public m_RFDataStructure As RFDataStructure = New RFDataStructure
    Public m_RFDataManager As RFDataManager = New RFDataManager
    'Public m_RFDataConnectionMgr As RFDataConnectionMgr = New RFDataConnectionMgr


    ' V1.1 - CK
    ' Amended the function definition to pass an additional argument - 
    ' arraylist to hold the details of the DALLAS UODS. The DAL messages are sent
    ' before sending GIA messages as request for DALLAS UOD details. The response 
    ' DAD message is processed after sending DAL.
    ' Public Overrides Function GetBookInDeliverySummary _
    '                      (ByRef arrDelvSummary As ArrayList) As Boolean

    ''' <summary>
    ''' The function gets the Delivery summary details for Book in Delvery SSC Receiving
    ''' </summary>
    ''' <param name="arrDelvSummary"></param>
    ''' <param name="arrDALLASDelSummary"></param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the delivery summary.
    ''' </returns>
    ''' <remarks> </remarks>

    Public Overrides Function GetBookInDeliverySummary _
                          (ByRef arrDelvSummary As ArrayList, ByRef arrDALLASDelSummary As ArrayList) As Boolean

        ' V1.1 - CK
        ' String variable cDADNextrecordno created for holding
        ' next record no.
        Dim cDADNextrecordno As String = "0000"

        Dim objGIAMessage As RFDataStructure.GIAMessage

        ' V1.1 - CK
        ' Initialised the variable
        ' Dim strReceivedData As String
        Dim strReceivedData As String = ""
        Dim bTemp As Boolean = True
        Try
            'to connect if after using the app 3 retry logic failure
            If objAppContainer.bCommFailure Then
                If Not m_RFDataManager.CheckReconnect() Then
                    Return False
                End If
            End If

            ' V1.1 - CK
            ' If store is enabled for Dallas positive receiving then send DAL 
            ' message and process the DAD response message.
            ' DAL message is sent multiple times until DAE message is received.

            If objAppContainer.bDallasPosReceiptEnabled = True Then

                If arrDALLASDelSummary.Count > 0 Then
                    arrDALLASDelSummary.Clear()
                End If

                Do
                    If m_RFDataManager.SendDAL(cDADNextrecordno) Then

                        If m_RFDataManager.WaitForResponse(strReceivedData) Then

                            If strReceivedData.Substring(0, 3) = "DAD" Then
                                m_RFDataManager.ProcessDADBookin(strReceivedData, arrDALLASDelSummary)
                                cDADNextrecordno = m_RFDataManager.cNextRecordNo
                            Else
                                If Not strReceivedData.Substring(0, 3) = "DAE" Then
                                    Return False
                                End If
                            End If

                        Else
                            If Not m_RFDataManager.CheckReconnect() Then
                                BDSessionMgr.GetInstance().bReconnected = False
                                If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                    objAppContainer.bTimeOut = False
                                    Do Until Not objAppContainer.bRetryAtTimeout
                                        m_RFDataManager.CheckReconnect(True)
                                    Loop
                                End If
                                Return False
                            Else
                                BDSessionMgr.GetInstance().bReconnected = True
                                Return False
                            End If
                        End If

                    Else
                        m_RFDataManager.CheckReconnect()
                        Return False
                    End If

                Loop While strReceivedData.Substring(0, 3) = "DAD"

            End If

            objGIAMessage = New RFDataStructure.GIAMessage
            With objGIAMessage
                .eType = RFDataStructure.DeliveryType.SSCReceiving
                .eFunc = RFDataStructure.GFunction.BookIn
                .eRequestType = RFDataStructure.GIARequestType.Summary
                .ePeriod = RFDataStructure.GPeriod.None
                .iPointer = Message.INITIALPOINTER
            End With
            'If Not m_RFDataManager.Connected() Then
            '    If Not m_RFDataManager.CheckReconnect() Then
            '        Return False
            '    End If
            'End If
            ' use a do while loop here to manipulate the GIA sending and recieving
            If m_RFDataManager.SendGIA(objGIAMessage) Then

                If m_RFDataManager.WaitForResponse(strReceivedData) Then


                    If strReceivedData.Substring(0, 3) = "GIB" Then
                        m_RFDataManager.ProcessGIBSSCBookin(strReceivedData, arrDelvSummary)
                        'If m_RFDataManager.ProcessGIBSSCBookin(strReceivedData) Then
                        '    For Each objDelvSummary As DeliverySummary In m_RFDataManager.arrTempList
                        '        arrDelvSummary.Add(objDelvSummary)
                        '    Next
                        'End If
                        bTemp = True
                    Else
                        'Fix to enter the Book in module if there are no expected/outstadning UODS
                        bTemp = True
                    End If
                Else
                    If Not m_RFDataManager.CheckReconnect() Then
                        'If Not m_RFDataManager.CheckReconnect() Then
                        BDSessionMgr.GetInstance().bReconnected = False
                        If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                            objAppContainer.bTimeOut = False
                            Do Until Not objAppContainer.bRetryAtTimeout
                                m_RFDataManager.CheckReconnect(True)
                            Loop
                        End If
                        Return False

                    Else
                        BDSessionMgr.GetInstance().bReconnected = True
                        Return False
                    End If
                End If

            Else

                m_RFDataManager.CheckReconnect()
                Return False

            End If




        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetBookInDeliverySummary():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return bTemp
    End Function
    ''' <summary>
    ''' The Function validates the Scanned UOD and get the details if it is valid
    ''' </summary>
    ''' <param name="strUODNumber"></param>
    ''' <param name="objUODInfo"></param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the scanned UOD.
    ''' </returns>
    ''' <remarks></remarks>
    Public Overrides Function ValidateUODScanned _
                        (ByVal strUODNumber As String, _
                         ByRef objUODInfo As UODInfo, _
                         ByVal tyFuncType As AppContainer.FunctionType) _
                         As Boolean
        Dim bTemp As Boolean = True
        Dim strUODStatus As String
        Dim strReceivedData As String = ""
        Dim strAuditStatus As String
        Dim strBOLUOD As String
        Dim objGIQMessage As RFDataStructure.GIQMessage
        Dim objGIRMessage As RFDataStructure.GIRBMessage
        Try
            With objGIQMessage
                .eType = RFDataStructure.DeliveryType.SSCReceiving
                .eFunc = tyFuncType
                .eRequestType = RFDataStructure.GIQRequestType.BookingIn
                .strSelectedCode = strUODNumber
                .eContType = RFDataStructure.ContentType.Container
                .eSupType = RFDataStructure.SupplierType.NoSupplier
                .iPointer = Message.GIQ.FIRSTPOINTER
            End With
            If m_RFDataManager.SendGIQ(objGIQMessage) Then
                If m_RFDataManager.WaitForResponse(strReceivedData) Then
                    If strReceivedData.Substring(0, 3) = "GIR" Then

                        objGIRMessage = Nothing
                        objGIRMessage = New RFDataStructure.GIRBMessage
                        If m_RFDataManager.ProcessGIRB(strReceivedData, objGIRMessage) Then
                            objUODInfo = New UODInfo
                            With objUODInfo
                                .UODNumber = objGIRMessage.strSelectedCode. _
                                                           Substring(0, 10)
                                .DespatchDate = objGIRMessage.strDespatchDate
                                .UODType = objGIRMessage.cOuterType
                                .UODReason = objGIRMessage.cUODReason
                                .UODStatus = objGIRMessage.cStatus
                                'CR if uod is partial booked
                                If .UODStatus = "P" Then
                                    .PartialBkd = Macros.Y
                                    'CR setting UOD status to UNbooked if the UOD is Partial booked
                                    .UODStatus = Macros.UNBOOKED
                                    'CR set the partial booked flag to TRUE
                                    BDSessionMgr.GetInstance().bPartialBkd = True
                                End If
                                .ImmLicenseNo = objGIRMessage.strSelectedCode. _
                                                Substring(10, 10).TrimStart(" ")
                                .ExpectedDeliveryDate = objGIRMessage.cOrderSuffix
                                .NoOfChildren = objGIRMessage.strOrderNum
                                .BOLUOD = objGIRMessage.cBOLUOD
                                .SequenceNumber = objGIRMessage.strOrderNum
                            End With
                            Return True
                        Else
                            Return False
                        End If
                    Else
                        'returning false when NAK received
                        Return False

                    End If
                Else
                    If Not m_RFDataManager.CheckReconnect() Then
                        If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                            objAppContainer.bTimeOut = False
                            Do Until Not objAppContainer.bRetryAtTimeout
                                m_RFDataManager.CheckReconnect(True)
                            Loop
                        End If
                    End If
                    Return False
                    End If
            Else
                    m_RFDataManager.CheckReconnect()
                    Return False
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:ValidateUODScanned():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
    End Function


    ''' <summary>
    ''' The Function validates the Scanned UOD and get the details if it is valid
    ''' </summary>
    ''' <param name="cUODNumber"></param>
    ''' <param name="objUODInfo"></param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the scanned UOD.
    ''' </returns>
    ''' <remarks></remarks>
    Public Overrides Function ValidateDallasUODScanned _
                        (ByVal cUODNumber As String, _
                         ByRef objDalUODInfo As DallasScanDetail, _
                         ByRef objDALLASItemList As ArrayList, _
                         ByVal tyFuncType As AppContainer.FunctionType) _
                         As Boolean
        Dim bTemp As Boolean = False

        Try
            For Each objScanDetails As GIValueHolder.DallasDeliverySummary In objDALLASItemList
                If objScanDetails.DallasBarcode = cUODNumber Then
                    objDalUODInfo = New GIValueHolder.DallasScanDetail
                    With objDalUODInfo
                        .DallasBarcode = cUODNumber
                        .DallasExpectedDate = objScanDetails.ExpectedDelDate
                        .ScanStatus = objScanDetails.Status
                    End With
                    bTemp = True
                End If
            Next
            Return bTemp

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:ValidateUODScanned():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
    End Function
    ''' <summary>
    ''' The function sets the 
    ''' </summary>
    ''' <param name="arrUODDetails"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function SendBookInDetails _
                        (ByRef arrUODDetails As ArrayList) As Boolean
        Dim objGIFMessage As RFDataStructure.GIFMessage
        Dim strReceivedData As String = ""
        Dim bTemp As Boolean = True
        Try
            For Each objScanDetails As GIValueHolder.ScanDetails In arrUODDetails
                objGIFMessage = New RFDataStructure.GIFMessage
                With objGIFMessage
                    .eFunc = RFDataStructure.GFunction.BookIn
                    .eType = RFDataStructure.DeliveryType.SSCReceiving ''to do
                    .strScanCode = objScanDetails.ScannedCode
                    .strDespatchDate = objScanDetails.DespatchDate
                    .strScanDate = objScanDetails.ScanDate
                    .strScanTime = objScanDetails.ScanTime
                    .cBatRescan = "X"
                    Select Case objScanDetails.ScanType
                        Case "B"
                            .eSType = Goodsin.AppContainer.ScanType.BookInScan
                        Case "L"
                            .eSType = Goodsin.AppContainer.ScanType.LateUODDetails
                        Case "N"
                            .eSType = Goodsin.AppContainer.ScanType.RetMisdirect
                    End Select
                    .cGITNote = " "

                    .eSLevel = RFDataStructure.ScanLevel.DeliveryScan
                End With
                If m_RFDataManager.SendGIF(objGIFMessage) Then
                    If m_RFDataManager.WaitForResponse(strReceivedData) Then
                        If strReceivedData.Substring(0, 3) = Message.ACK Then
                            bTemp = True
                        Else
                            bTemp = False
                        End If
                    Else
                        If Not m_RFDataManager.CheckReconnect() Then
                            objAppContainer.bTimeOut = False
                            Do Until Not objAppContainer.bRetryAtTimeout
                                m_RFDataManager.CheckReconnect(True)
                            Loop
                        End If
                        bTemp = False
                        Exit For
                    End If
                Else
                    m_RFDataManager.CheckReconnect()
                    bTemp = False
                    Exit For
                End If

            Next
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:SendBookInDetails():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return bTemp
    End Function
    Public Overrides Function SendBatchConfirmation _
                (ByRef objDriverDetails As GIValueHolder.DriverDetails) As Boolean
        Dim objGIFMessage As RFDataStructure.GIFMessage
        Dim strReceivedData As String = ""
        Dim bTemp As Boolean = True
        Try

            objGIFMessage = New RFDataStructure.GIFMessage
            With objGIFMessage
                .eFunc = RFDataStructure.GFunction.BookIn
                .eType = RFDataStructure.DeliveryType.SSCReceiving ''to do
                .strScanCode = "X".PadLeft(20, "X")
                .strDespatchDate = "XXXXXX"
                .strScanDate = objDriverDetails.ScanDate
                .strScanTime = objDriverDetails.ScanTime
                .strDriverBadge = objDriverDetails.DriverBadge
                .cGITNote = objDriverDetails.GITNote
                .cBatRescan = objDriverDetails.BatchRescan
                .eSType = AppContainer.ScanType.BatchConfirm
                .eSLevel = RFDataStructure.ScanLevel.NotUsed
            End With
            'send GIF message
            'If Not m_RFDataManager.Connected() Then
            '    If Not m_RFDataManager.CheckReconnect() Then
            '        Return False
            '    End If
            'End If
            If m_RFDataManager.SendGIF(objGIFMessage) Then
                '    m_RFDataManager.SendGIF(objGIFMessage)
                'wait and receive resposne from controller
                If m_RFDataManager.WaitForResponse(strReceivedData) Then
                    'm_RFDataManager.WaitForResponse(strReceivedData)
                    ' End If
                    'check the validity of response
                    If strReceivedData.Substring(0, 3) = Message.ACK Then
                        Return True
                    Else
                        Return False
                    End If
                Else
                    If Not m_RFDataManager.CheckReconnect() Then
                        If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                            objAppContainer.bTimeOut = False
                            Do Until Not objAppContainer.bRetryAtTimeout
                                m_RFDataManager.CheckReconnect(True)
                            Loop
                        End If
                    End If
                    Return False
                End If
            Else
                m_RFDataManager.CheckReconnect()
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:SendBatchConfirmation():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function
    Public Overrides Function SendDeliveryConfirmation _
                  (ByRef objDriverDetails As GIValueHolder.DriverDetails) As Boolean
        Dim objGIFMessage As RFDataStructure.GIFMessage
        Dim strReceivedData As String = ""
        Dim bTemp As Boolean = True
        Try

            objGIFMessage = New RFDataStructure.GIFMessage
            With objGIFMessage
                .eFunc = RFDataStructure.GFunction.BookIn
                .eType = RFDataStructure.DeliveryType.SSCReceiving ''to do
                .strScanCode = "X".PadLeft(20, "X")
                .strDespatchDate = "XXXXXX"
                .strScanDate = objDriverDetails.ScanDate
                .strScanTime = objDriverDetails.ScanTime
                .strDriverBadge = objDriverDetails.DriverBadge
                .cGITNote = objDriverDetails.GITNote
                .cBatRescan = objDriverDetails.BatchRescan
                .eSType = AppContainer.ScanType.DeliveryConfirm
                .eSLevel = RFDataStructure.ScanLevel.NotUsed
            End With
            'If Not m_RFDataManager.Connected() Then
            '    If Not m_RFDataManager.CheckReconnect() Then
            '        Return False
            '    End If
            'End If
            If m_RFDataManager.SendGIF(objGIFMessage) Then
                If m_RFDataManager.WaitForResponse(strReceivedData) Then
                    If strReceivedData.Substring(0, 3) = Message.ACK Then
                        Return True
                    Else
                        Return False
                    End If
                Else
                    If Not m_RFDataManager.CheckReconnect() Then
                        If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                            objAppContainer.bTimeOut = False
                            Do Until Not objAppContainer.bRetryAtTimeout
                                m_RFDataManager.CheckReconnect(True)
                            Loop
                        End If
                    End If
                    Return False
                End If
            Else
                m_RFDataManager.CheckReconnect()
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:SendDeliveryConfirmation():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try

    End Function
    ' V1.1 - CK
    ' Added function SendDallasDeliveryConfirmation to send Dallas Delivery confirmation message

    ''' <summary>
    ''' The function retrieves the scanned UOD details and calls SendDAR to send DAR message
    ''' </summary>
    ''' <param name="arrDallasUODdetails"></param>
    ''' <returns>Boolean
    ''' True - If successfully sent Dallas Delivery confirmation messages
    ''' False - If sending Dallas Delivery confirmation messages were not successful.
    ''' </returns>
    ''' <remarks></remarks
    Public Overrides Function SendDallasDeliveryConfirmation(ByRef arrDallasUODdetails As ArrayList) As Boolean
        Dim objDARMessage As RFDataStructure.DARMessage
        Dim cReceivedData As String = ""
        Dim cNakMessage As String = ""
        Dim bTemp As Boolean = True
        Try
            For Each objDallasScannedDetails As GIValueHolder.DallasScanDetail In arrDallasUODdetails
                objDARMessage = New RFDataStructure.DARMessage
                With objDARMessage
                    .cDallasBarcode = objDallasScannedDetails.DallasBarcode
                    .cScanDate = objDallasScannedDetails.DallasScanDate
                    .cScanStatus = objDallasScannedDetails.ScanStatus
                End With
                If m_RFDataManager.SendDAR(objDARMessage) Then
                    If m_RFDataManager.WaitForResponse(cReceivedData) Then
                        If cReceivedData.Substring(0, 3) = Message.ACK Then
                            bTemp = True
                        Else
                            bTemp = False
                        End If
                    Else
                        If Not m_RFDataManager.CheckReconnect() Then
                            objAppContainer.bTimeOut = False
                            Do Until Not objAppContainer.bRetryAtTimeout
                                m_RFDataManager.CheckReconnect(True)
                            Loop
                        End If
                        bTemp = False
                        Exit For
                    End If
                Else
                    m_RFDataManager.CheckReconnect()
                    Return False
                    Exit For
                End If
            Next
        Catch ex As Exception
            ' Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource SendDallasDeliveryConfirmation: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function

    Public Overrides Function SendSessionExit( _
                        ByVal tyDeliveryType As AppContainer.DeliveryType, _
                        ByVal tyFunction As AppContainer.FunctionType, _
                        ByVal tyIsAbort As AppContainer.IsAbort) _
                        As Boolean
        Dim objGIXMessage As New RFDataStructure.GIXMessage
        Try
            'For saving data
            objAppContainer.eCurrLocation = CurrentLocation.SendingSessionExit
            With objGIXMessage
                .eDeliveryType = tyDeliveryType
                .eFunction = tyFunction
                .eIsAbort = tyIsAbort
            End With
            'CHANGE
            If Not objAppContainer.bSaveDetails Then
                objAppContainer.objSavedGIXMessage = objGIXMessage
                If m_RFDataManager.SendGIX(objGIXMessage) Then
                    If m_RFDataManager.WaitForResponse(strData) Then
                        If strData.Substring(0, 3) = Message.ACK Then
                            'if logoff success
                            Return True
                        Else
                            'if logoff failed
                            Return False
                        End If
                    Else
                        If Not m_RFDataManager.CheckReconnect() Then
                            If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                objAppContainer.bTimeOut = False
                                Do Until Not objAppContainer.bRetryAtTimeout
                                    m_RFDataManager.CheckReconnect(True)
                                Loop
                            End If
                        End If
                        Return False
                    End If
                Else
                    m_RFDataManager.CheckReconnect()
                    Return False
                End If
            Else
                objAppContainer.objSaveGIXMessage = objGIXMessage
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:SendSessionExit():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function
    Public Overrides Function GetAuditSummary() As Boolean
        Dim objDeliverySummary As DeliverySummary
        Dim objGIAMessage As RFDataStructure.GIAMessage

        Dim strReceivedData As String
        Dim bTemp As Boolean = True
        Try
            'If objAppContainer.bCommFailure Then
            '    m_RFDataManager.ReconnectOn()
            'End If
            'to connect if after using the app 3 retry logic failure
            If objAppContainer.bCommFailure Then
                If Not m_RFDataManager.CheckReconnect() Then
                    'If Not m_RFDataManager.CheckReconnect() Then
                    Return False
                End If
            End If
            objGIAMessage = New RFDataStructure.GIAMessage
            With objGIAMessage
                .eType = RFDataStructure.DeliveryType.SSCReceiving
                .eFunc = RFDataStructure.GFunction.Audit
                .eRequestType = RFDataStructure.GIARequestType.Summary
                .ePeriod = RFDataStructure.GPeriod.None
                .iPointer = 0
            End With
            'If Not m_RFDataManager.Connected() Then
            '    If Not m_RFDataManager.CheckReconnect() Then
            '        Return False
            '    End If
            'End If
            If m_RFDataManager.SendGIA(objGIAMessage) Then
                If m_RFDataManager.WaitForResponse(strReceivedData) Then
                    If strReceivedData.Substring(0, 3) = "ACK" Then
                        Return True
                    Else
                        Return False
                    End If
                Else
                    If Not m_RFDataManager.CheckReconnect() Then
                        If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                            objAppContainer.bTimeOut = False
                            Do Until Not objAppContainer.bRetryAtTimeout
                                m_RFDataManager.CheckReconnect(True)
                            Loop
                        End If
                    End If
                    Return False

                End If
            Else
                m_RFDataManager.CheckReconnect()
                Return False

            End If


        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetAuditSummary():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try

    End Function
    ''' <summary>
    ''' Gets the Item Details for an Item Scanned
    ''' </summary>
    ''' <param name="strItemCode"></param>
    ''' <param name="objItemInfo"></param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If any error occurred while updating the results or 
    ''' there is no details available for the Item .
    ''' </returns>
    ''' <remarks></remarks>
    Public Overrides Function GetItemDetails(ByVal strItemCode As String, _
                                             ByVal strType As AppContainer.ItemDetailType, _
                                             ByRef objItemInfo As ItemInfo) _
                                             As Boolean
        Dim strReceivedData As String = ""
        Dim objEQRMessage As New RFDataStructure.EQRMEssage
        Dim bTemp As Boolean = True
        Try
            'For saving data
            objAppContainer.eCurrLocation = CurrentLocation.SendingENQ
            strItemCode = strItemCode.PadLeft(13, "0")
            If m_RFDataManager.SendENQ(RFDataStructure.ENQType.Item, _
                                              RFDataStructure.ENQFunction.Other, _
                                              strItemCode, _
                                              False) Then
                If m_RFDataManager.WaitForResponse(strReceivedData) Then
                    If strReceivedData.Substring(0, 3) = "EQR" Then
                        m_RFDataManager.ProcessEQR(strReceivedData, objEQRMessage)
                        With objItemInfo
                            .ProductCode = objEQRMessage.strBarcode
                            .ItemDesc = objEQRMessage.strLongDescription
                            .ItemQty = "1" ' objEQRMessage.strStockFigure 
                            .BootsCode = objEQRMessage.strBootsCode
                        End With
                    Else
                        Return False
                    End If
                Else
                    If Not m_RFDataManager.CheckReconnect() Then
                        If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                            objAppContainer.bTimeOut = False
                            Do Until Not objAppContainer.bRetryAtTimeout
                                m_RFDataManager.CheckReconnect(True)
                            Loop
                        End If
                    End If
                    Return False
                End If

            Else
                m_RFDataManager.CheckReconnect()
                Return False
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetItemDetails():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function
    Public Overrides Function SendItemQuantity(ByRef iItemCount As Integer, _
                                               ByVal DelType As AppContainer.DeliveryType, _
                                               ByVal FuncType As AppContainer.FunctionType) _
                                               As Boolean
        Dim objGIFMessage As New RFDataStructure.GIFMessage
        Dim strReceivedData As String = ""
        Try
            'For saving data
            objAppContainer.eCurrLocation = CurrentLocation.SendingItemQuantity
            If (DelType <> DeliveryType.Directs AndAlso FuncType <> FunctionType.BookIn) Then

                With objGIFMessage
                    If DelType = DeliveryType.Directs Then
                        .eType = RFDataStructure.DeliveryType.DirectsReceiving
                    ElseIf DelType = DeliveryType.ASN Then
                        .eType = RFDataStructure.DeliveryType.ASNs
                    ElseIf DelType = DeliveryType.SSC Then
                        .eType = RFDataStructure.DeliveryType.SSCReceiving
                    End If
                    If FuncType = FunctionType.BookIn Then
                        .eFunc = RFDataStructure.GFunction.BookIn
                    ElseIf FuncType = FunctionType.Audit Then
                        .eFunc = RFDataStructure.GFunction.Audit
                    ElseIf FuncType = FunctionType.View Then
                        .eFunc = RFDataStructure.GFunction.View
                    End If

                    If DelType = DeliveryType.Directs AndAlso FuncType = FunctionType.BookIn Then
                        .strItemStatus = "XXXXX"
                    Else
                        .strItemStatus = (iItemCount + 2).ToString().PadLeft(5, "0")
                    End If

                    'Fix for sending XXXXXX in despatch date for the extra GIF message sent
                    .strDespatchDate = "XXXXXX"
                    .eSType = Goodsin.AppContainer.ScanType.None
                    'End fix

                End With
                'CHANGE
                If Not objAppContainer.bSaveDetails Then
                    'Save it for reconnect
                    objAppContainer.objSavedGIFFinish = objGIFMessage

                    If m_RFDataManager.SendGIF(objGIFMessage) Then
                        If m_RFDataManager.WaitForResponse(strReceivedData) Then
                            If strReceivedData.Substring(0, 3) = Message.ACK Then
                                'if successfully sent to controller the details
                                Return True
                            Else
                                Return False
                            End If
                        Else
                            If Not m_RFDataManager.CheckReconnect() Then
                                If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                    objAppContainer.bTimeOut = False
                                    Do Until Not objAppContainer.bRetryAtTimeout
                                        m_RFDataManager.CheckReconnect(True)
                                    Loop
                                End If
                            End If
                            Return False
                        End If
                    Else
                        m_RFDataManager.CheckReconnect()
                        Return False
                    End If
                Else
                    objAppContainer.m_FinishedDetails.Add(objGIFMessage)
                End If
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:SendItemQuantity():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function
    Public Overrides Function GetChildUODDetails(ByVal strUODNumber As String, _
                                                 ByRef objUODInfo As UODInfo, _
                                                 ByRef arrChildUODlist As ArrayList) As Boolean
        Dim objGIQMessage As New RFDataStructure.GIQMessage
        Dim objGIRMessage As New RFDataStructure.GIRFMessage
        Dim objUODList As UODList
        Dim strExpectedDate As Date
        Dim strTodaysDate As Date = DateTime.Now.Date
        Dim bTemp As Boolean = True
        Try


            With objGIQMessage
                .eType = RFDataStructure.DeliveryType.SSCReceiving
                .eFunc = RFDataStructure.GFunction.BookIn
                .eRequestType = RFDataStructure.GIQRequestType.FullSummary
                .strSelectedCode = strUODNumber.PadLeft(20, "0")
                .eSupType = RFDataStructure.SupplierType.NoSupplier
                .eContType = RFDataStructure.ContentType.Container
                .iPointer = Message.INITIALPOINTER
            End With
            m_RFDataManager.iPointer = Message.INITIALPOINTER
            'send GIQ until the Response GIR has -1 as Pointer value
            Do Until m_RFDataManager.iPointer = Message.ENDTPOINTER


                'Send GIR message to controller
                If m_RFDataManager.SendGIQ(objGIQMessage) Then
                    'Wait and receive response from Controller
                    If m_RFDataManager.WaitForResponse(strData) Then
                        'check validity of Response
                        If strData.Substring(0, 3) = Message.GIR Then

                            'Process the valid response
                            m_RFDataManager.ProcessGIRF(strData, objGIRMessage, iPointer)
                            objGIQMessage.iPointer = m_RFDataManager.iPointer


                        Else
                            bTemp = False
                            Exit Do
                        End If
                    Else
                        If Not m_RFDataManager.CheckReconnect() Then
                            If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                objAppContainer.bTimeOut = False
                                Do Until Not objAppContainer.bRetryAtTimeout
                                    m_RFDataManager.CheckReconnect(True)
                                Loop
                            End If
                        End If
                        bTemp = False
                        Exit Do
                    End If
                Else
                    m_RFDataManager.CheckReconnect()
                    bTemp = False
                    Exit Do
                End If

            Loop
            For Each objGIRFData As RFDataStructure.GIRFMessageData In objGIRMessage.arrGIRFData

                objUODList = New UODList
                With objUODList
                    .UODID = objGIRFData.strIdentifier
                    .UODType = objGIRFData.strName
                    .BookedIn = objGIRFData.cBookedIn
                End With
                arrChildUODlist.Add(objUODList)

            Next
            If bTemp Then
                objUODInfo.NoOfChildren = arrChildUODlist.Count.ToString()
            End If

            m_RFDataManager.iPointer = Message.INITIALPOINTER
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetChildUODDetails():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' The function retrieves the UOD list for View UOD 
    ''' </summary>
    ''' <param name="strPeriod"></param>
    ''' <param name="arrUODList"></param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If any error occurred while updating the results or 
    ''' there is no details available for the UOD List for view.
    ''' </returns>
    ''' <remarks></remarks>
    Public Overrides Function GetUODListForView(ByVal strPeriod As String, _
                                                ByRef arrUODList As ArrayList) _
                                                As Boolean
        Dim objGIAMessage As New RFDataStructure.GIAMessage
        Dim bTemp As Boolean = False
        Dim strReceivedData As String
        Try
            'to connect if after using the app 3 retry logic failure
            'If objAppContainer.bCommFailure Then
            '    m_RFDataManager.ReconnectOn()
            'End If
            'to connect if after using the app 3 retry logic failure
            If objAppContainer.bCommFailure Then
                If Not m_RFDataManager.CheckReconnect() Then
                    Return False
                End If
            End If
            iPointer = 0
            With objGIAMessage
                .eType = RFDataStructure.DeliveryType.SSCReceiving
                .eFunc = RFDataStructure.GFunction.View
                .eRequestType = RFDataStructure.GIARequestType.List
                If strPeriod = "T" Then
                    .ePeriod = RFDataStructure.GPeriod.Today
                Else
                    .ePeriod = RFDataStructure.GPeriod.Future
                End If
                m_RFDataManager.iPointer = Message.INITIALPOINTER
            End With
            Do Until m_RFDataManager.iPointer = Message.ENDTPOINTER
                objGIAMessage.iPointer = m_RFDataManager.iPointer
                If m_RFDataManager.SendGIA(objGIAMessage) Then
                    If m_RFDataManager.WaitForResponse(strReceivedData) Then
                        If strReceivedData.Substring(0, 3) = Message.NAK Then
                            If Not bTemp Then
                                bTemp = False
                            End If

                            Exit Do
                        Else
                            If Not m_RFDataManager.ProcessGIBSSCView(strReceivedData, arrUODList) Then
                                bTemp = False
                                Exit Do
                            Else
                                bTemp = True
                            End If
                        End If
                    Else
                        If Not m_RFDataManager.CheckReconnect() Then
                            If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                objAppContainer.bTimeOut = False
                                Do Until Not objAppContainer.bRetryAtTimeout
                                    m_RFDataManager.CheckReconnect(True)
                                Loop
                            End If
                        End If
                        bTemp = False
                        Exit Do
                    End If
                Else
                    m_RFDataManager.CheckReconnect()
                    bTemp = False
                    Exit Do
                End If
            Loop
            m_RFDataManager.iPointer = Message.INITIALPOINTER

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetUODListForView(): " + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details

        Return bTemp
    End Function

    ' V1.1 - KK
    ' Added new function GetDallasUODListForView for processing Dallas UOD data for View UOD Menu

    ''' <summary>
    ''' The function gets the Dallas delivery data for View UOD Menu
    ''' </summary>
    ''' <param name="arrDalViewUOD"></param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If any error occurred while updating the results
    ''' </returns>
    ''' <remarks> </remarks>
    Public Overrides Function GetDallasUODListForView(ByRef arrDalViewUOD As ArrayList) As Boolean
        ' String variable cDADNextrecordno created
        Dim cDADNextrecordno As String = "0000"
        ' Initialised the variable
        Dim cReceivedData As String = ""
        Dim bTemp As Boolean = True
        Try
            If objAppContainer.bCommFailure Then
                If Not m_RFDataManager.CheckReconnect() Then
                    Return False
                End If
            End If

            If arrDalViewUOD.Count > 0 Then
                arrDalViewUOD.Clear()
            End If

            Do
                If m_RFDataManager.SendDAL(cDADNextrecordno) Then

                    If m_RFDataManager.WaitForResponse(cReceivedData) Then

                        If cReceivedData.Substring(0, 3) = "DAD" Then
                            m_RFDataManager.ProcessDADBookin(cReceivedData, arrDalViewUOD)
                            cDADNextrecordno = m_RFDataManager.cNextRecordNo
                        Else
                            If Not cReceivedData.Substring(0, 3) = "DAE" Then
                                Return False
                            End If
                        End If

                    Else
                        If Not m_RFDataManager.CheckReconnect() Then
                            BDSessionMgr.GetInstance().bReconnected = False
                            If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                objAppContainer.bTimeOut = False
                                Do Until Not objAppContainer.bRetryAtTimeout
                                    m_RFDataManager.CheckReconnect(True)
                                Loop
                            End If
                            Return False
                        Else
                            BDSessionMgr.GetInstance().bReconnected = True
                            Return False
                        End If
                    End If

                Else
                    m_RFDataManager.CheckReconnect()
                    Return False
                End If

            Loop While cReceivedData.Substring(0, 3) = "DAD"

        Catch ex As Exception
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetDallasUODListForView(): " + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        ' If successfully updated the details
        Return True
       
    End Function

    ''' <summary>
    ''' to get the list of crates for a selected dolly
    ''' </summary>
    ''' <param name="strUODNumber"></param>
    ''' <param name="arrCrateList"></param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the List of Crates.
    ''' </returns>
    ''' <remarks></remarks>
    Public Overrides Function GetCrateListForView(ByVal strUODNumber As String, _
                                                  ByRef arrCrateList As ArrayList, _
                                                  Optional ByVal objUODInfo As UODInfo = Nothing, _
                                                  Optional ByVal strSequence As String = "XXXXX") As Boolean
        Dim objGIQMessage As New RFDataStructure.GIQMessage
        Dim objGIRMessage As New RFDataStructure.GIRFMessage
        Dim strStatus As String = "N"
        Dim bTemp As Boolean = True
        Dim objCrateList As CrateList
        Try
            With objGIQMessage
                .eType = RFDataStructure.DeliveryType.SSCReceiving
                .eFunc = RFDataStructure.GFunction.View
                .strSelectedCode = strUODNumber
                .eRequestType = RFDataStructure.GIQRequestType.FullSummary
                .eSupType = RFDataStructure.SupplierType.NoSupplier
                .eContType = RFDataStructure.ContentType.Container
                .strSequence = strSequence
            End With
            m_RFDataManager.iPointer = Message.INITIALPOINTER
            Do Until m_RFDataManager.iPointer = Message.ENDTPOINTER
                objGIQMessage.iPointer = m_RFDataManager.iPointer
                If m_RFDataManager.SendGIQ(objGIQMessage) Then
                    If m_RFDataManager.WaitForResponse(strData) Then
                        If strData.Substring(0, 3) = Message.NAK Then
                            bTemp = False
                            Exit Do
                        Else
                            m_RFDataManager.ProcessGIRF(strData, objGIRMessage, iPointer)
                            With objUODInfo
                                .BookInDate = objGIRMessage.strBookInDate
                                .ExpectedDeliveryDate = objGIRMessage.strEstDeliveryDate
                                strStatus = objGIRMessage.cStatus
                                Select Case strStatus
                                    Case "B"
                                        .UODStatus = Macros.Y
                                    Case "U"
                                        .UODStatus = Macros.N
                                    Case "A"
                                        .UODStatus = strStatus
                                        'Fix for Partial UOD
                                    Case "P"
                                        .UODStatus = Macros.N
                                End Select

                                .UODNumber = strUODNumber
                                .UODReason = objGIRMessage.cUODReason
                                .UODType = objGIRMessage.cOuterType
                            End With

                        End If
                    Else
                        If Not m_RFDataManager.CheckReconnect() Then
                            If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                objAppContainer.bTimeOut = False
                                Do Until Not objAppContainer.bRetryAtTimeout
                                    m_RFDataManager.CheckReconnect(True)
                                Loop
                            End If
                        End If
                        bTemp = False
                        Exit Do
                    End If
                Else
                    m_RFDataManager.CheckReconnect()
                    bTemp = False
                    Exit Do
                End If
            Loop
            If bTemp Then
                For Each objGIRFData As RFDataStructure.GIRFMessageData In objGIRMessage.arrGIRFData
                    objCrateList = New CrateList
                    With objCrateList
                        strStatus = objGIRFData.cBookedIn
                        Select Case strStatus
                            Case "B"
                                .BookedIn = Macros.Y
                            Case "U"
                                .BookedIn = Macros.N
                            Case "A"
                                .BookedIn = strStatus
                        End Select

                        .CrateId = objGIRFData.strIdentifier
                        .CrateType = objGIRFData.strName.Trim(" ")
                        .NoOfItems = objGIRFData.strQuantity.TrimStart("0")
                        .Sequence = objGIRFData.strSequence
                    End With
                    arrCrateList.Add(objCrateList)
                Next


            End If
            m_RFDataManager.iPointer = Message.INITIALPOINTER



        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetCrateListForView():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return bTemp

    End Function
    ''' <summary>
    ''' to get the list of items for a selected container other than dolly.
    ''' </summary>
    ''' <param name="strCrateId"></param>
    ''' <param name="arrItemList"></param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the Item list for the selected container
    ''' </returns>
    ''' <remarks></remarks>
    Public Overrides Function GetItemListForView(ByVal strCrateId As String, _
                                     ByRef arrItemList As ArrayList, _
                                     Optional ByVal objUODinfo As UODInfo = Nothing, _
                                     Optional ByVal strSequence As String = "XXXXX") As Boolean
        Dim bTemp As Boolean = True
        Dim objGIQMessage As New RFDataStructure.GIQMessage
        Dim objGIRMessage As New RFDataStructure.GIRFMessage
        Dim objItemList As ItemList
        Try
            With objGIQMessage
                .eType = RFDataStructure.DeliveryType.SSCReceiving
                .eFunc = RFDataStructure.GFunction.View
                .strSelectedCode = strCrateId
                .eRequestType = RFDataStructure.GIQRequestType.FullSummary
                .eSupType = RFDataStructure.SupplierType.NoSupplier
                .eContType = RFDataStructure.ContentType.Item
                .strSequence = strSequence
            End With
            m_RFDataManager.iPointer = Message.INITIALPOINTER

            Do Until m_RFDataManager.iPointer = Message.ENDTPOINTER
                objGIQMessage.iPointer = m_RFDataManager.iPointer
                If m_RFDataManager.SendGIQ(objGIQMessage) Then
                    If m_RFDataManager.WaitForResponse(strData) Then
                        If strData.Substring(0, 3) = "GIR" Then

                            m_RFDataManager.ProcessGIRF(strData, objGIRMessage, iPointer)
                            With objUODinfo
                                .UODNumber = strCrateId
                                .UODType = objGIRMessage.cOuterType

                                Select Case objGIRMessage.cStatus
                                    Case "B"
                                        .UODStatus = Macros.Y
                                    Case "U"
                                        .UODStatus = Macros.N
                                    Case "A"
                                        .UODStatus = objGIRMessage.cStatus

                                End Select

                                .UODReason = objGIRMessage.cUODReason
                                .ExpectedDeliveryDate = objGIRMessage.strEstDeliveryDate
                                .BookInDate = objGIRMessage.strBookInDate
                                .BOLUOD = objGIRMessage.cBOLUOD
                                .DespatchDate = objGIRMessage.strDespatchDate
                            End With


                        Else
                            bTemp = False
                            Exit Do
                        End If
                    Else
                        If Not m_RFDataManager.CheckReconnect() Then
                            If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                objAppContainer.bTimeOut = False
                                Do Until Not objAppContainer.bRetryAtTimeout
                                    m_RFDataManager.CheckReconnect(True)
                                Loop
                            End If
                        End If
                        bTemp = False
                        Exit Do
                    End If
                Else
                    m_RFDataManager.CheckReconnect()
                    bTemp = False
                    Exit Do
                End If



            Loop
            If bTemp Then


                For Each objGIRFData As RFDataStructure.GIRFMessageData In objGIRMessage.arrGIRFData
                    objItemList = New ItemList
                    With objItemList
                        .ItemCode = objGIRFData.strIdentifier.Substring(3, 7)
                        .ItemDesc = objGIRFData.strDescription
                        If objGIRFData.strQuantity = "000000" Then
                            .ItemQty = "0"
                        Else
                            .ItemQty = objGIRFData.strQuantity.TrimStart("0")
                        End If

                    End With
                    arrItemList.Add(objItemList)
                Next
            End If
            m_RFDataManager.iPointer = Message.INITIALPOINTER



        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetItemListForView():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return bTemp

    End Function
    ''' <summary>
    ''' The function retrieves the list of suppliers
    ''' </summary>
    ''' <param name="arrSupplierList"></param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If any error occurred while updating the results or 
    ''' there is no details available for the Suppliers.
    ''' </returns>
    ''' <remarks></remarks>
    Public Overrides Function GetSupplierList(ByRef arrSupplierList As ArrayList) As Boolean
        Dim objGIAMessage As New RFDataStructure.GIAMessage
        Dim objGIBMessage As New RFDataStructure.GIBSMessage
        Dim objSupplierList As New SupplierList
        Dim bTemp As Boolean = True
        Try
            If objAppContainer.bCommFailure Then
                If Not m_RFDataManager.CheckReconnect() Then
                    'If Not m_RFDataManager.CheckReconnect() Then
                    Return False
                End If
            End If
            If arrSupplierList.Count > 0 Then
                arrSupplierList.Clear()
            End If
            With objGIAMessage
                .eType = RFDataStructure.DeliveryType.DirectsReceiving
                .eFunc = RFDataStructure.GFunction.BookIn
                .ePeriod = RFDataStructure.GPeriod.None
                .eRequestType = RFDataStructure.GIARequestType.Summary
                .iPointer = Message.INITIALPOINTER
            End With
            m_RFDataManager.iPointer = Message.INITIALPOINTER
            Do Until m_RFDataManager.iPointer = Message.ENDTPOINTER
                'send the GIA Request message to Controller
                If m_RFDataManager.SendGIA(objGIAMessage) Then
                    'wait and receive reponse from controller
                    If m_RFDataManager.WaitForResponse(strData) Then
                        'check if respone is valid or not
                        If strData.Substring(0, 3) = "GIB" Then
                            m_RFDataManager.ProcessGIBS(strData, objGIBMessage)
                            objGIAMessage.iPointer = m_RFDataManager.iPointer

                        Else
                            'exit if reponse is invalid
                            bTemp = False
                            Exit Do
                        End If
                    Else
                        If Not m_RFDataManager.CheckReconnect() Then
                            If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                objAppContainer.bTimeOut = False
                                Do Until Not objAppContainer.bRetryAtTimeout
                                    m_RFDataManager.CheckReconnect(True)
                                Loop
                            End If
                        End If
                        bTemp = False
                        Exit Do
                    End If
                Else
                    m_RFDataManager.CheckReconnect()
                    bTemp = False
                    Exit Do
                End If
            Loop
            For Each objGIBSummary As RFDataStructure.GIBSummary In objGIBMessage.arrGIBSData

                objSupplierList = New SupplierList
                With objSupplierList
                    .SupplierName = objGIBSummary.strName.TrimEnd(" ")
                    .SupplierNo = objGIBSummary.strIdentifier.TrimStart("0")
                    If objGIBSummary.strQuantity = "000000" Then
                        .SupplierQty = "0"
                    Else
                        .SupplierQty = objGIBSummary.strQuantity.TrimStart("0")

                    End If


                    .SupplierType = objGIBSummary.cSupplierType

                End With

                ' O&C suppress Boots.com supplier from displaying
                If objSupplierList.SupplierNo <> ConfigDataMgr.GetInstance.GetParam(ConfigKey.DOTCOM_SUPPLIER_NUM) Then

                    arrSupplierList.Add(objSupplierList)

                End If



            Next
            m_RFDataManager.iPointer = Message.INITIALPOINTER





        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetSupplierList():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False


        End Try
        'return
        Return bTemp
    End Function
    ''' <summary>
    ''' The function retrieves the List of Orders for a supplier selected
    ''' </summary>
    ''' <param name="strSupplierNo"></param>
    ''' <param name="arrOrderList"></param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the Order numbers
    ''' </returns>
    ''' <remarks></remarks>
    Public Overrides Function GetOrderList(ByVal strSupplierNo As String, ByRef arrOrderList As ArrayList) As Boolean
        Dim objGIQMessage As New RFDataStructure.GIQMessage
        Dim objGIRMessage As New RFDataStructure.GIRFMessage
        Dim objOrderList As OrderList
        Dim bTemp As Boolean = True
        Dim strOrderCode As String

        Try
            If arrOrderList.Count > 0 Then
                arrOrderList.Clear()
            End If
            With objGIQMessage
                .eType = RFDataStructure.DeliveryType.DirectsReceiving
                .eFunc = RFDataStructure.GFunction.BookIn
                .eRequestType = RFDataStructure.GIQRequestType.FullSummary
                .eContType = RFDataStructure.ContentType.Container
                .eSupType = RFDataStructure.SupplierType.Directs
                .strSelectedCode = strSupplierNo

            End With
            m_RFDataManager.iPointer = Message.INITIALPOINTER
            Do Until m_RFDataManager.iPointer = Message.ENDTPOINTER
                If m_RFDataManager.SendGIQ(objGIQMessage) Then
                    If m_RFDataManager.WaitForResponse(strData) Then
                        If strData.Substring(0, 3) = "GIR" Then
                            m_RFDataManager.ProcessGIRF(strData, objGIRMessage, iPointer)

                            objGIQMessage.iPointer = m_RFDataManager.iPointer
                        Else
                            'If any error occured in reading the data adapter then return false.
                            bTemp = False
                            Exit Do
                        End If
                    Else
                        If Not m_RFDataManager.CheckReconnect() Then
                            If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                objAppContainer.bTimeOut = False
                                Do Until Not objAppContainer.bRetryAtTimeout
                                    m_RFDataManager.CheckReconnect(True)
                                Loop
                            End If
                        End If
                        bTemp = False
                        Exit Do
                    End If
                Else
                    m_RFDataManager.CheckReconnect()
                    bTemp = False
                    Exit Do
                End If
            Loop
            If bTemp Then
                For Each objGIRFData As RFDataStructure.GIRFMessageData In objGIRMessage.arrGIRFData
                    objOrderList = New OrderList
                    With objOrderList
                        ' .BookInDate = objGIRFData.
                        .BookInStatus = objGIRFData.cBookedIn
                        .EstDeliveryDate = objGIRFData.strName
                        .OrderNo = objGIRFData.strDescription.Substring(6, 4)
                        strOrderCode = objGIRFData.strDescription.TrimEnd(" ")
                        .Code = strOrderCode
                        .SupplierNo = objGIRFData.strDescription.Substring(0, 6)
                        strSequenceNum = objGIRFData.strSequence
                        .PageNo = objGIRFData.strSequence
                    End With
                    arrOrderList.Add(objOrderList)
                Next
            End If
            m_RFDataManager.iPointer = Message.INITIALPOINTER
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetOrderList():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return bTemp
    End Function
    Public Overrides Function GetItemListForOrder(ByRef objOrderList As GIValueHolder.OrderList, _
                                            ByRef arrOrderList As ArrayList) As Boolean
        'to do from business layer - pass the Orderlist.Code in strOrderNum if an ORder is selected
        Dim objGIQMessage As New RFDataStructure.GIQMessage
        Dim objGIRMessage As New RFDataStructure.GIRFMessage
        Dim bTemp As Boolean = True
        Dim objItemList As ItemListForOrder
        Try
            If arrOrderList.Count > 0 Then
                arrOrderList.Clear()
            End If
            With objGIQMessage
                .eType = RFDataStructure.DeliveryType.DirectsReceiving
                .eFunc = RFDataStructure.GFunction.BookIn
                .eRequestType = RFDataStructure.GIQRequestType.FullSummary
                .eSupType = RFDataStructure.SupplierType.Directs
                .eContType = RFDataStructure.ContentType.Item
                .strSelectedCode = objOrderList.Code
                .strSequence = "XXXXX"
                .iPointer = Message.INITIALPOINTER
            End With
            m_RFDataManager.iPointer = Message.INITIALPOINTER
            Do Until m_RFDataManager.iPointer = Message.ENDTPOINTER
                If m_RFDataManager.SendGIQ(objGIQMessage) Then
                    If m_RFDataManager.WaitForResponse(strData) Then
                        If strData.Substring(0, 3) = "GIR" Then
                            m_RFDataManager.ProcessGIRF(strData, objGIRMessage, iPointer)

                            objGIQMessage.iPointer = m_RFDataManager.iPointer
                        Else
                            bTemp = False
                            Exit Do
                        End If
                    Else
                        If Not m_RFDataManager.CheckReconnect() Then
                            If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                objAppContainer.bTimeOut = False
                                Do Until Not objAppContainer.bRetryAtTimeout
                                    m_RFDataManager.CheckReconnect(True)
                                Loop
                            End If
                        End If
                        bTemp = False
                        Exit Do
                    End If
                Else
                    m_RFDataManager.CheckReconnect()
                    bTemp = False
                    Exit Do
                End If
            Loop
            For Each objGIRFData As RFDataStructure.GIRFMessageData In objGIRMessage.arrGIRFData
                objItemList = New ItemListForOrder
                With objItemList
                    .ExptDate = objGIRMessage.strEstDeliveryDate
                    .BootsCode = objGIRFData.strIdentifier.Substring(3, 7)
                    .ProductCode = objGIRFData.strName
                    .ItemDesc = objGIRFData.strDescription
                    .ExptdQty = objGIRFData.strQuantity
                    .Status = objGIRFData.cBookedIn
                    .PageNo = objGIRFData.strSequence
                End With
                arrOrderList.Add(objItemList)
            Next
            m_RFDataManager.iPointer = Message.INITIALPOINTER

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetItemListForOrder():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try

        Return bTemp
    End Function
    ''' <summary>
    ''' The function gets the details of the Supplier number entered
    ''' </summary>
    ''' <param name="SupplierNo"></param>
    ''' <param name="arrSupplierNumber"></param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the Supplier number entered
    ''' </returns>
    ''' <remarks></remarks>
    Public Overrides Function ValidateSupplierNumber(ByVal SupplierNo As String, ByRef arrSupplierNumber As ArrayList) As Boolean

        Dim strSupplierType As String
        Dim strStaticSupplier As String
        Dim objGIQMessage As New RFDataStructure.GIQMessage
        Dim objGIRMessage As New RFDataStructure.GIRSMessage
        Dim objSupplierData As SupplierData
        Try
            With objGIQMessage
                .eType = DeliveryType.Directs
                .eFunc = FunctionType.BookIn
                .eRequestType = RFDataStructure.GIQRequestType.SupplierQuery
                .eSupType = RFDataStructure.SupplierType.NoSupplier
                .strSelectedCode = SupplierNo
                .iPointer = Message.INITIALPOINTER
            End With
            If m_RFDataManager.SendGIQ(objGIQMessage) Then
                If m_RFDataManager.WaitForResponse(strData) Then
                    If strData.Substring(0, 3) = "GIR" Then
                        m_RFDataManager.ProcessGIRS(strData, objGIRMessage)
                        objSupplierData = New SupplierData
                        With objSupplierData
                            .SupplierName = objGIRMessage.strSupplierName
                            .SupplierNo = objGIRMessage.strSupplierNum
                            .SupplierType = objGIRMessage.strSupplierType
                        End With
                    Else
                        Return False
                    End If
                Else
                    If Not m_RFDataManager.CheckReconnect() Then
                        If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                            objAppContainer.bTimeOut = False
                            Do Until Not objAppContainer.bRetryAtTimeout
                                m_RFDataManager.CheckReconnect(True)
                            Loop
                        End If
                    End If
                    Return False
                End If
            Else
                m_RFDataManager.CheckReconnect()
                Return False
            End If


            'process datareader and retrieve the details in it.

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:ValidateSupplierNumber():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function
    ''' <summary>
    ''' To get the configuration details for the Goods In application
    ''' </summary>
    ''' <param name="objConfigValue"></param>
    ''' <remarks></remarks>TT
    Public Overrides Function GetConfigValues(ByRef objConfigValue As AppContainer.ConfigValues) As Boolean
        Dim objGIAMessage As New RFDataStructure.GIAMessage
        Dim objGIBMessage As New RFDataStructure.GIBCMessage

        Try
            With objGIAMessage
                .eType = RFDataStructure.DeliveryType.None
                .eFunc = RFDataStructure.GFunction.None
                .eRequestType = RFDataStructure.GIARequestType.Configuration
                .ePeriod = RFDataStructure.GPeriod.None
                .iPointer = Message.INITIALPOINTER
            End With
            If m_RFDataManager.SendGIA(objGIAMessage) Then
                If m_RFDataManager.WaitForResponse(strData) Then
                    If strData.Substring(0, 3) = "GIB" Then
                        m_RFDataManager.ProcessGIBC(strData, objGIBMessage)

                        objConfigValue = New AppContainer.ConfigValues
                        With objConfigValue
                            .ASNActive = objGIBMessage.cASNActive
                            .UODActive = objGIBMessage.cUODActive
                            .DirectsActive = objGIBMessage.cDirectsActive
                            .ONightDelivery = objGIBMessage.cONightDelivery
                            .ONightScan = objGIBMessage.cONightScan
                            .BatchSize = objGIBMessage.strBatchSize
                        End With
                    Else
                        Return False
                    End If
                Else
                    If Not m_RFDataManager.CheckReconnect() Then
                        If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                            objAppContainer.bTimeOut = False
                            Do Until Not objAppContainer.bRetryAtTimeout
                                m_RFDataManager.CheckReconnect(True)
                            Loop
                        End If
                    End If
                    Return False
                End If
            Else
                m_RFDataManager.CheckReconnect()
                Return False
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetConfigValues():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        Return True
    End Function
    Public Overrides Function SendItemDetails(ByRef arrItemDetails As ArrayList, _
                                               ByVal tyDelType As AppContainer.DeliveryType, _
                                               ByVal tyFunction As AppContainer.FunctionType) _
                                               As Boolean
        Dim objGIFMessage As RFDataStructure.GIFMessage
        Dim bTemp As Boolean = True
        Dim iTemp As Integer = 0
        Try
            'For saving data
            objAppContainer.eCurrLocation = CurrentLocation.SendingItemCount
            For Each objItemDetails As GIValueHolder.ScanDetails In arrItemDetails
                objGIFMessage = New RFDataStructure.GIFMessage
                With objGIFMessage
                    'scanned code should be Orderlist.Code if OrderNumber booking in
                    .eType = tyDelType
                    .eFunc = tyFunction
                    If objItemDetails.ScannedCode = Nothing Then
                        .strScanCode = "X".PadLeft(20, "X")
                    Else
                        .strScanCode = objItemDetails.ScannedCode
                    End If
                    .strDespatchDate = objItemDetails.DespatchDate
                    'strbarcode should be 7 digit boots code for Directs booking in
                    .strBarcode = objItemDetails.ProductCode
                    .strItemStatus = objItemDetails.ItemStatus
                    If objItemDetails.ItemQty = Nothing Then
                        .strQuantity = "XXXXX"
                    Else
                        .strQuantity = objItemDetails.ItemQty
                    End If

                    .eSLevel = RFDataStructure.ScanLevel.ItemScan
                    Select Case objItemDetails.ScanType
                        Case "B"
                            .eSType = Goodsin.AppContainer.ScanType.BookInScan
                        Case "A"
                            .eSType = Goodsin.AppContainer.ScanType.AuditScan
                        Case "C"
                            .eSType = Goodsin.AppContainer.ScanType.BatchConfirm
                        Case "S"
                            .eSType = Goodsin.AppContainer.ScanType.DeliveryConfirm
                        Case "L"
                            .eSType = Goodsin.AppContainer.ScanType.LateUODDetails
                        Case "N"
                            .eSType = Goodsin.AppContainer.ScanType.RetMisdirect
                    End Select

                    .strScanDate = objItemDetails.ScanDate
                    .strScanTime = objItemDetails.ScanTime
                    If iTemp = 0 AndAlso _
                       (tyDelType <> DeliveryType.Directs AndAlso _
                        tyFunction <> FunctionType.BookIn) Then
                        .strItemStatus = "S    "
                    Else
                        .strItemStatus = "XXXXX"
                    End If
                    iTemp += 1

                End With
                'CHANGE
                If Not objAppContainer.bSaveDetails Then
                    If m_RFDataManager.SendGIF(objGIFMessage) Then
                        If Not m_RFDataManager.WaitForResponse(strData) Then
                            If Not m_RFDataManager.CheckReconnect() Then
                                If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                    objAppContainer.bTimeOut = False
                                    Do Until Not objAppContainer.bRetryAtTimeout
                                        m_RFDataManager.CheckReconnect(True)
                                    Loop
                                End If
                            End If
                            bTemp = False
                            Exit For
                        Else
                            If strData.Substring(0, 3) = Message.ACK Then
                                bTemp = True
                            Else
                                bTemp = False
                                Exit For
                            End If
                        End If
                    Else
                        m_RFDataManager.CheckReconnect()
                        bTemp = False
                        Exit For
                    End If
                Else
                    objAppContainer.m_FinishedDetails.Add(objGIFMessage)
                End If
            Next


        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:SendItemDetails():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        Return bTemp
    End Function
    Public Overrides Function SendAuditSession() As Boolean
        Dim objGIAMessage As New RFDataStructure.GIAMessage
        Dim strReceivedData As String = ""
        Try
            'to connect if after using the app 3 retry logic failure
            'If objAppContainer.bCommFailure Then
            '    m_RFDataManager.ReconnectOn()
            'End If
            'to connect if after using the app 3 retry logic failure
            If objAppContainer.bCommFailure Then
                If Not m_RFDataManager.CheckReconnect() Then
                    'If Not m_RFDataManager.CheckReconnect() Then
                    Return False
                End If
            End If
            With objGIAMessage
                .eType = RFDataStructure.DeliveryType.DirectsReceiving
                .eFunc = RFDataStructure.GFunction.Audit
                .ePeriod = RFDataStructure.GPeriod.None
                .eRequestType = RFDataStructure.GIARequestType.Summary
                .iPointer = 0
            End With
            'If Not m_RFDataManager.Connected() Then
            '    If Not m_RFDataManager.CheckReconnect() Then
            '        Return False
            '    End If
            'End If
            'Send GIA request message to controller
            If m_RFDataManager.SendGIA(objGIAMessage) Then
                'wait and receive response from controller
                If m_RFDataManager.WaitForResponse(strReceivedData) Then
                    'check validity of reponse received from controller
                    If strReceivedData.Substring(0, 3) = Message.ACK Then
                        'return true if valid
                        Return True
                    Else
                        'return false if invalid
                        Return False
                    End If
                Else
                    If Not m_RFDataManager.CheckReconnect() Then
                        'TIMEOUT LOGIC
                        If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                            objAppContainer.bTimeOut = False
                            Do Until Not objAppContainer.bRetryAtTimeout
                                m_RFDataManager.CheckReconnect(True)
                            Loop
                        End If
                    End If
                    Return False
                End If
            Else
                m_RFDataManager.CheckReconnect()
                Return False
            End If


        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:SendAuditSession():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False

        End Try
    End Function
    ''' <summary>
    ''' to retrieve the details of a scanned carton
    ''' </summary>
    ''' <param name="strASNNumber"></param>
    ''' <param name="objCartonInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function ValidateCartonScanned(ByVal strASNNumber As String, _
                                             ByRef objCartonInfo As GIValueHolder.CartonInfo, _
                                             ByVal tyDelType As AppContainer.DeliveryType, _
                                             ByVal tyFunction As AppContainer.FunctionType) _
                                             As Boolean
        Dim objGIQMessage As New RFDataStructure.GIQMessage
        Dim objGIRMessage As New RFDataStructure.GIRBMessage

        Try
            With objGIQMessage
                .eType = tyDelType
                .eFunc = tyFunction
                .eRequestType = RFDataStructure.GIQRequestType.BookingIn
                .eSupType = RFDataStructure.SupplierType.ASN
                .iPointer = Message.GIQ.FIRSTPOINTER
                .strSequence = "XX"
                .strSelectedCode = strASNNumber
            End With
            If m_RFDataManager.SendGIQ(objGIQMessage) Then
                If m_RFDataManager.WaitForResponse(strData) Then
                    If strData.Substring(0, 3) = "GIR" Then
                        m_RFDataManager.ProcessGIRB(strData, objGIRMessage)
                        objCartonInfo = New GIValueHolder.CartonInfo
                        With objCartonInfo
                            .CartonNumber = objGIRMessage.strSelectedCode.Substring(8, 8)
                            .ExpDeliveryDate = objGIRMessage.strDespatchDate
                            .Status = objGIRMessage.cStatus
                            .ASNNumber = objGIRMessage.strSelectedCode.Substring(2, 18)
                            .SupplierNo = objGIRMessage.strSelectedCode.Substring(2, 6)
                        End With

                    Else
                        Return False
                    End If
                Else
                    If Not m_RFDataManager.CheckReconnect() Then
                        If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                            objAppContainer.bTimeOut = False
                            Do Until Not objAppContainer.bRetryAtTimeout
                                m_RFDataManager.CheckReconnect(True)
                            Loop
                        End If
                    End If
                    Return False
                End If
            Else
                m_RFDataManager.CheckReconnect()
                Return False

            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:ValidateCartonScanned():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function

    Public Overrides Function SendCartonDetails(ByRef arrCartonDetails As ArrayList, _
                                                ByVal tyDelType As AppContainer.DeliveryType, _
                                                ByVal tyFunction As AppContainer.FunctionType) _
                                                As Boolean
        Dim objGIFMessage As RFDataStructure.GIFMessage
        Dim iCount As Integer = 0
        Dim bTemp As Boolean = True
        Try
            For Each objCartonDetails As ScanDetails In arrCartonDetails
                objGIFMessage = New RFDataStructure.GIFMessage
                With objGIFMessage
                    .strScanCode = objCartonDetails.ScannedCode.PadLeft(20, "0")
                    .strScanDate = objCartonDetails.ScanDate
                    .strScanTime = objCartonDetails.ScanTime
                    Select Case objCartonDetails.ScanType
                        Case "B"
                            .eSType = Goodsin.AppContainer.ScanType.BookInScan
                        Case "A"
                            .eSType = Goodsin.AppContainer.ScanType.AuditScan
                    End Select

                    '.eSLevel = RFDataStructure.ScanLevel.DeliveryScan
                    If objCartonDetails.ScanLevel = "D" Then
                        .eSLevel = RFDataStructure.ScanLevel.DeliveryScan
                    ElseIf objCartonDetails.ScanLevel = "I" Then
                        .eSLevel = RFDataStructure.ScanLevel.ItemScan
                        .strBarcode = objCartonDetails.ProductCode.PadLeft(13, "0")
                        .strQuantity = objCartonDetails.ItemQty.PadLeft(6, "0")
                    End If

                    .eType = tyDelType
                    .eFunc = tyFunction
                    If iCount = 0 AndAlso objCartonDetails.ScanLevel = "I" Then

                        .strItemStatus = "S    "

                    Else

                        If CInt(objCartonDetails.ItemStatus) > 0 Then
                            .strItemStatus = (CInt(objCartonDetails.ItemStatus) + 2).ToString().PadLeft(5, "0")
                            '-1 used as the value will get incremented below
                            iCount = -1
                        Else
                            .strItemStatus = "XXXXX"


                        End If

                    End If

                End With
                If Not objAppContainer.bSaveDetails Then


                    If m_RFDataManager.SendGIF(objGIFMessage) Then
                        If m_RFDataManager.WaitForResponse(strData) Then
                            'If Not strData.Substring(0, 3) = Message.ACK Then
                            '    Return False
                            'End If
                            If objCartonDetails.ScanLevel = "I" Then
                                iCount += 1
                            Else
                                iCount = 0
                            End If
                        Else
                            If Not m_RFDataManager.CheckReconnect() Then
                                If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                    objAppContainer.bTimeOut = False
                                    Do Until Not objAppContainer.bRetryAtTimeout
                                        m_RFDataManager.CheckReconnect(True)
                                    Loop
                                End If
                            End If
                            bTemp = False
                            Exit For
                        End If
                    Else
                        m_RFDataManager.CheckReconnect()
                        bTemp = False
                        Exit For
                    End If
                Else
                    objAppContainer.m_FinishedDetails.Add(objGIFMessage)
                End If
            Next

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:SendCartonDetails():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return bTemp
    End Function
    Public Overrides Function GetSupplierListForView(ByRef arrSupplierList As ArrayList) _
                                                     As Boolean
        Dim objGIAMessage As New RFDataStructure.GIAMessage
        Dim objGIBMessage As New RFDataStructure.GIBSMessage
        Dim strSupplierType As String
        Dim objSupplierList As SupplierList
        Dim bTemp As Boolean = True
        Try
            'to connect if after using the app 3 retry logic failure
            'If objAppContainer.bCommFailure Then
            '    m_RFDataManager.ReconnectOn()
            'End If
            'to connect if after using the app 3 retry logic failure
            If objAppContainer.bCommFailure Then
                If Not m_RFDataManager.CheckReconnect() Then
                    'If Not m_RFDataManager.CheckReconnect() Then
                    Return False
                End If
            End If
            With objGIAMessage
                .eType = RFDataStructure.DeliveryType.DirectsReceiving
                .eFunc = RFDataStructure.GFunction.View
                .ePeriod = RFDataStructure.GPeriod.None
                .eRequestType = RFDataStructure.GIARequestType.Summary
                .iPointer = Message.INITIALPOINTER
            End With
            m_RFDataManager.iPointer = Message.INITIALPOINTER
            'If Not m_RFDataManager.Connected() Then
            '    If Not m_RFDataManager.CheckReconnect() Then
            '        Return False
            '    End If
            'End If
            Do Until m_RFDataManager.iPointer = Message.ENDTPOINTER
                objGIBMessage.Pointer = m_RFDataManager.iPointer

                If m_RFDataManager.SendGIA(objGIAMessage) Then
                    If m_RFDataManager.WaitForResponse(strData) Then
                        If strData.Substring(0, 3) = "GIB" Then
                            m_RFDataManager.ProcessGIBS(strData, objGIBMessage)

                        Else
                            bTemp = False
                            Exit Do
                        End If
                    Else
                        If Not m_RFDataManager.CheckReconnect() Then
                            If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                objAppContainer.bTimeOut = False
                                Do Until Not objAppContainer.bRetryAtTimeout
                                    m_RFDataManager.CheckReconnect(True)
                                Loop
                            End If
                        End If
                        bTemp = False
                        Exit Do
                    End If
                Else
                    m_RFDataManager.CheckReconnect()
                    bTemp = False
                    Exit Do
                End If

            Loop
            For Each objGIBData As RFDataStructure.GIBSummary In objGIBMessage.arrGIBSData
                objSupplierList = New SupplierList
                With objSupplierList
                    .SupplierName = objGIBData.strName.TrimEnd(" ")
                    .SupplierNo = objGIBData.strIdentifier.Substring(4, 6)
                    .SupplierQty = objGIBData.strQuantity.TrimStart("0")
                    .SupplierType = objGIBData.cSupplierType
                End With
                If objSupplierList.SupplierType = "A" Then
                    ' O&C suppress Boots.com supplier (CS)
                    If objSupplierList.SupplierNo <> ConfigDataMgr.GetInstance.GetParam(ConfigKey.DOTCOM_SUPPLIER_NUM) Then

                        arrSupplierList.Add(objSupplierList)

                    End If
                End If

            Next
            m_RFDataManager.iPointer = Message.INITIALPOINTER

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetSupplierListForView():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function
    Public Overrides Function GetSupplierDetails(ByVal strSupplierNo As String, _
                                                 ByRef arrSupplierDetails As ArrayList) _
                                                 As Boolean
        Dim objGIQMessage As New RFDataStructure.GIQMessage
        Dim objGIRMessage As New RFDataStructure.GIRFMessage
        Dim bTemp As Boolean = True
        Dim objSupplierDetails As SupplierDetails
        Dim strStatus As String
        Try
            With objGIQMessage
                .eType = RFDataStructure.DeliveryType.ASNs
                .eFunc = RFDataStructure.GFunction.View
                .eRequestType = RFDataStructure.GIQRequestType.FullSummary
                .eSupType = RFDataStructure.SupplierType.ASN
                .eContType = RFDataStructure.ContentType.Container
                .strSelectedCode = strSupplierNo.PadLeft(20, "0")
                .iPointer = Message.INITIALPOINTER
            End With
            m_RFDataManager.iPointer = Message.INITIALPOINTER

            Do Until m_RFDataManager.iPointer = Message.ENDTPOINTER

                If m_RFDataManager.SendGIQ(objGIQMessage) Then
                    If m_RFDataManager.WaitForResponse(strData) Then
                        If strData.Substring(0, 3) = "GIR" Then
                            m_RFDataManager.ProcessGIRF(strData, objGIRMessage, iPointer)
                            'Seting the value of the Pointer for the next GIQ from the Response GIR
                            ' if POINTER is not -1
                            objGIQMessage.iPointer = m_RFDataManager.iPointer

                        Else
                            bTemp = False
                            Exit Do
                        End If
                    Else
                        If Not m_RFDataManager.CheckReconnect() Then
                            'if retry selected on timeout 
                            If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                objAppContainer.bTimeOut = False
                                Do Until Not objAppContainer.bRetryAtTimeout
                                    m_RFDataManager.CheckReconnect(True)
                                Loop
                            End If
                        End If
                        bTemp = False
                        Exit Do
                    End If
                Else
                    m_RFDataManager.CheckReconnect()
                    bTemp = False
                    Exit Do
                End If

            Loop
            For Each objGIRFData As RFDataStructure.GIRFMessageData In objGIRMessage.arrGIRFData
                objSupplierDetails = New SupplierDetails
                With objSupplierDetails
                    .CartonNumber = objGIRFData.strIdentifier.Substring(2, 8)
                    '.ExptDate = objGIRFData.strName.Substring(2, 6)
                    .ExptDate = objGIRFData.strName.Substring(0, 8)
                    Select Case objGIRFData.cBookedIn
                        Case Status.Booked
                            .Status = Macros.Y
                        Case Status.UnBooked
                            .Status = Macros.N
                        Case Status.Audited
                            .Status = Macros.AUDITED
                    End Select


                    .TotalItemsInCarton = objGIRFData.strQuantity.TrimStart("0")
                End With
                arrSupplierDetails.Add(objSupplierDetails)
            Next
            m_RFDataManager.iPointer = Message.INITIALPOINTER


        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetSupplierDetails():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return bTemp
    End Function
    ''' <summary>
    ''' To retrieve the Item list for a selected carton
    ''' </summary>
    ''' <param name="strCartonNumber"></param>
    ''' <param name="arrCartoDetails"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetCartonDetails(ByVal strCartonNumber As String, _
                                               ByRef arrItemDetails As ArrayList, _
                                               ByVal tyDelType As AppContainer.DeliveryType, _
                                               ByVal tyFunc As AppContainer.FunctionType, _
                                               Optional ByRef objCartonInfo As GIValueHolder.CartonInfo = Nothing) As Boolean

        Dim objGIQMessage As New RFDataStructure.GIQMessage
        Dim objGIRMessage As New RFDataStructure.GIRFMessage
        Dim bTemp As Boolean = True
        Dim objItemDetails As ItemDetails
        Try
            With objGIQMessage
                .eType = RFDataStructure.DeliveryType.ASNs
                .eFunc = RFDataStructure.GFunction.View
                .strSelectedCode = strCartonNumber
                .eRequestType = RFDataStructure.GIQRequestType.FullSummary
                .eSupType = RFDataStructure.SupplierType.ASN ''to be changed
                .eContType = RFDataStructure.ContentType.Item
                .iPointer = Message.INITIALPOINTER
            End With
            m_RFDataManager.iPointer = Message.INITIALPOINTER
            Do Until m_RFDataManager.iPointer = Message.ENDTPOINTER
                If m_RFDataManager.SendGIQ(objGIQMessage) Then
                    'wait and receive response from Controller
                    If m_RFDataManager.WaitForResponse(strData) Then
                        'check if response is valid or not
                        If strData.Substring(0, 3) = "GIR" Then
                            m_RFDataManager.ProcessGIRF(strData, objGIRMessage, iPointer)
                            objGIQMessage.iPointer = m_RFDataManager.iPointer

                        Else
                            ' if response is invalid 
                            bTemp = False
                            Exit Do
                        End If
                    Else
                        If Not m_RFDataManager.CheckReconnect() Then
                            'if retry selected on timeout 
                            If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                objAppContainer.bTimeOut = False
                                Do Until Not objAppContainer.bRetryAtTimeout
                                    m_RFDataManager.CheckReconnect(True)
                                Loop
                            End If
                        End If
                        bTemp = False
                        Exit Do
                    End If
                Else
                    m_RFDataManager.CheckReconnect()
                    bTemp = False
                    Exit Do
                End If
            Loop
            'RF RECONNECT
            'If objAppContainer.bCommFailure Then
            '    Return bTemp
            'End If
            If objGIRMessage.arrGIRFData.Count > 0 Then
                For Each objGIRFData As RFDataStructure.GIRFMessageData In objGIRMessage.arrGIRFData
                    objItemDetails = New ItemDetails
                    With objItemDetails
                        .ItemCode = objGIRFData.strIdentifier.Substring(3, 7)
                        .ProductCode = objGIRFData.strName
                        .ItemDesc = objGIRFData.strDescription
                        If objGIRFData.strQuantity = "000000" Then
                            .ItemQty = "0"
                        Else
                            .ItemQty = objGIRFData.strQuantity.TrimStart("0")
                        End If

                        .Status = objGIRFData.cBookedIn.ToString()

                    End With
                    arrItemDetails.Add(objItemDetails)
                Next
            End If
            m_RFDataManager.iPointer = Message.INITIALPOINTER




        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetCartonDetails():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'Return
        Return bTemp

    End Function
    Public Overrides Function GetSupplierData(ByVal strSupplierNumber As String, _
                                              ByRef objSupplierData As SupplierList, _
                                              ByRef tyDelType As AppContainer.DeliveryType, _
                                              ByVal tyFunction As AppContainer.FunctionType) _
                                              As Boolean
        Dim strSupplierType As String
        Dim strStaticSupplier As String
        Dim objGIQMessage As New RFDataStructure.GIQMessage
        Dim objGIRMessage As New RFDataStructure.GIRSMessage

        Try
            With objGIQMessage
                .eType = tyDelType
                .eFunc = tyFunction
                .eRequestType = RFDataStructure.GIQRequestType.SupplierQuery
                .eSupType = RFDataStructure.SupplierType.NoSupplier
                .strSelectedCode = strSupplierNumber.PadLeft(20, "0")
                .iPointer = Message.INITIALPOINTER
            End With

            If m_RFDataManager.SendGIQ(objGIQMessage) Then
                If m_RFDataManager.WaitForResponse(strData) Then
                    If strData.Substring(0, 3) = "GIR" Then
                        m_RFDataManager.ProcessGIRS(strData, objGIRMessage)
                        objSupplierData = New SupplierList
                        With objSupplierData
                            .SupplierName = objGIRMessage.strSupplierName
                            .SupplierNo = objGIRMessage.strSupplierNum
                            .SupplierType = objGIRMessage.strSupplierType
                        End With
                    Else
                        Return False
                    End If
                Else
                    If Not m_RFDataManager.CheckReconnect() Then
                        'if retry selected on timeout 
                        If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                            objAppContainer.bTimeOut = False
                            Do Until Not objAppContainer.bRetryAtTimeout
                                m_RFDataManager.CheckReconnect(True)
                            Loop
                        End If
                    End If
                    Return False
                End If

            Else
                m_RFDataManager.CheckReconnect()
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetSupplierData():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        Return True
    End Function
    Public Overrides Function GetUODDetails(ByVal strUODNumber As String, _
                                            ByRef objUODInfo As UODInfo, _
                                            ByRef arrItemDetails As ArrayList) _
                                            As Boolean
        Dim objGIQMessage As New RFDataStructure.GIQMessage
        Dim objGIRMessage As New RFDataStructure.GIRFMessage
        Dim objItemList As ItemList
        Dim bTemp As Boolean = True
        Try
            With objGIQMessage
                .eType = RFDataStructure.DeliveryType.SSCReceiving
                .eFunc = RFDataStructure.GFunction.Audit
                .eRequestType = RFDataStructure.GIQRequestType.FullSummary
                .strSelectedCode = strUODNumber.PadLeft(20, "0")
                .eSupType = RFDataStructure.SupplierType.NoSupplier
                .eContType = RFDataStructure.ContentType.Item
                .iPointer = Message.INITIALPOINTER
            End With
            m_RFDataManager.iPointer = Message.INITIALPOINTER
            'send GIQ until the Response GIR has -1 as Pointer value
            Do Until m_RFDataManager.iPointer = Message.ENDTPOINTER
                'Send GIR message to controller
                If m_RFDataManager.SendGIQ(objGIQMessage) Then


                    'Wait and receive response from Controller
                    If m_RFDataManager.WaitForResponse(strData) Then

                        'check validity of Response
                        If strData.Substring(0, 3) = Message.GIR Then
                            'Process the valid response
                            m_RFDataManager.ProcessGIRF(strData, objGIRMessage, iPointer)
                            objGIQMessage.iPointer = m_RFDataManager.iPointer
                            With objUODInfo
                                .UODNumber = objGIRMessage.strSelectedCode.Substring(0, 10)
                                .ImmLicenseNo = objGIRMessage.strSelectedCode.Substring(10, 10).Trim(" ")
                                .UODType = objGIRMessage.cOuterType
                                .NoOfChildren = objGIRMessage.strOrderNum
                                .UODReason = objGIRMessage.cUODReason
                                .UODStatus = objGIRMessage.cStatus
                                .BOLUOD = objGIRMessage.cBOLUOD
                                .ExpectedDeliveryDate = objGIRMessage.strEstDeliveryDate
                                .BookInDate = objGIRMessage.strBookInDate
                                .DespatchDate = objGIRMessage.strDespatchDate
                            End With

                        Else
                            bTemp = False
                            Exit Do

                        End If
                    Else
                        If Not m_RFDataManager.CheckReconnect() Then
                            'if retry selected on timeout 
                            If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                                objAppContainer.bTimeOut = False
                                Do Until Not objAppContainer.bRetryAtTimeout
                                    m_RFDataManager.CheckReconnect(True)
                                Loop
                                '   m_RFDataManager.CheckReconnect(True)
                            End If
                        End If
                        bTemp = False
                        Exit Do
                    End If

                Else
                    m_RFDataManager.CheckReconnect()
                    bTemp = False
                    Exit Do
                End If

            Loop
            If bTemp Then
                For Each objGIRFData As RFDataStructure.GIRFMessageData In objGIRMessage.arrGIRFData
                    objItemList = New ItemList
                    With objItemList
                        .ItemCode = objGIRFData.strIdentifier.Substring(3, 7)
                        .ProductCode = objGIRFData.strName
                        .ItemDesc = objGIRFData.strDescription
                        If objGIRFData.strQuantity = "000000" Then
                            .ItemQty = "0"
                        Else
                            .ItemQty = objGIRFData.strQuantity.TrimStart("0")
                        End If

                    End With
                    arrItemDetails.Add(objItemList)
                Next
            End If
            m_RFDataManager.iPointer = Message.INITIALPOINTER

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetUODDetails():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        Return bTemp
    End Function

    Public Overrides Function GetUserDetails(ByVal strUserId As String, _
                                             ByRef objUserInfo As UserInfo) As Boolean

        Dim objSNRMessage As New RFDataStructure.SNRMessage
        Try
            If m_RFDataManager.SendSOR(strUserId.Substring(0, 3), strUserId.Substring(3, 3)) Then


                If m_RFDataManager.WaitForResponse(strData) Then


                    If strData.Substring(0, 3) = "SNR" Then
                        m_RFDataManager.ProcessSNR(strData, objSNRMessage)
                        With objUserInfo
                            .UserID = objSNRMessage.strUserID
                            .user_Name = objSNRMessage.strUserName
                            .Password = strUserId.Substring(3, 3)
                        End With
                        With objAppContainer
                            .strUser = strUserId.Substring(3, 3)
                            .strCurrentUserID = strUserId.Substring(0, 3)
                        End With
                    Else
                        ' if user login failed
                        Return False
                    End If
                Else
                    If Not m_RFDataManager.CheckReconnect() Then
                        'if retry selected on timeout 
                        If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                            objAppContainer.bTimeOut = False
                            Do Until Not objAppContainer.bRetryAtTimeout
                                m_RFDataManager.CheckReconnect(True)
                            Loop
                        End If
                        Return False
                    Else
                        Return False
                    End If
                End If

            Else
                If Not m_RFDataManager.CheckReconnect() Then
                    Return False
                Else
                    Return False
                End If
            End If


        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetUserDetails():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        Return True
    End Function

    Public Overrides Function LogOff(Optional ByVal isCallForCrash As Boolean = False) As Boolean
        Try

            If m_RFDataManager.SendOFF() Then
                If m_RFDataManager.WaitForResponse(strData) Then
                    If strData.Substring(0, 3) = Message.ACK Then
                        Return True
                    ElseIf strData.Contains("NAKERROR") Then
                        Return True
                    Else
                        Return False
                    End If
                Else

                    If Not m_RFDataManager.CheckReconnect() Then
                        'if retry selected on timeout 
                        If objAppContainer.bTimeOut And objAppContainer.bRetryAtTimeout Then
                            objAppContainer.bTimeOut = False
                            Do Until Not objAppContainer.bRetryAtTimeout
                                m_RFDataManager.CheckReconnect(True)
                            Loop
                        End If
                        Return False
                    Else
                        Return False
                    End If
                End If
            Else
                If Not m_RFDataManager.CheckReconnect() Then
                    Return False
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:LogOff():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try

    End Function
    Public Overrides Function GetUODChildCount(ByVal strUODNumber As String, _
                                                  ByRef iChildCount As Integer) As Boolean
        Dim objGIQMessage As New RFDataStructure.GIQMessage
        Dim objGIRMessage As New RFDataStructure.GIRFMessage
        Dim objCrateList As CrateList
        Dim strStatus As String
        Dim bTemp As Boolean = True
        Dim objUODInfo As New UODInfo
        Dim bIsGIQCorrect As Boolean = False
        iChildCount = 0
        Try
            bIsGIQCorrect = CBool(ConfigDataMgr.GetInstance().GetParam(ConfigKey.IS_GIQB_CORRECT))

            'CR only send the GIQ F message if the UOD Is partial Booked
            If BDSessionMgr.GetInstance().bPartialBkd = True OrElse Not bIsGIQCorrect Then
                With objGIQMessage
                    .eType = RFDataStructure.DeliveryType.SSCReceiving
                    .eFunc = RFDataStructure.GFunction.BookIn
                    .eRequestType = RFDataStructure.GIQRequestType.FullSummary
                    .strSelectedCode = strUODNumber.PadLeft(20, "0")
                    .eSupType = RFDataStructure.SupplierType.NoSupplier
                    .eContType = RFDataStructure.ContentType.Container
                    .iPointer = Message.INITIALPOINTER
                End With
                m_RFDataManager.iPointer = Message.INITIALPOINTER
                'send GIQ until the Response GIR has -1 as Pointer value
                Do Until m_RFDataManager.iPointer = Message.ENDTPOINTER
                    'Send GIR message to controller
                    m_RFDataManager.SendGIQ(objGIQMessage)
                    'Wait and receive response from Controller
                    m_RFDataManager.WaitForResponse(strData)
                    'check validity of Response
                    If strData.Substring(0, 3) = Message.GIR Then
                        'Process the valid response
                        m_RFDataManager.ProcessGIRF(strData, objGIRMessage, iPointer)
                        objGIQMessage.iPointer = m_RFDataManager.iPointer
                        With objUODInfo
                            .UODNumber = objGIRMessage.strSelectedCode.Substring(0, 10)
                            .ImmLicenseNo = objGIRMessage.strSelectedCode.Substring(10, 10).Trim(" ")
                            .UODType = objGIRMessage.cOuterType
                            .NoOfChildren = objGIRMessage.strOrderNum
                            .UODReason = objGIRMessage.cUODReason
                            .UODStatus = objGIRMessage.cStatus
                            .BOLUOD = objGIRMessage.cBOLUOD
                            .ExpectedDeliveryDate = objGIRMessage.strEstDeliveryDate
                            .BookInDate = objGIRMessage.strBookInDate
                            .DespatchDate = objGIRMessage.strDespatchDate
                        End With

                    Else
                        bTemp = False
                        Exit Do

                    End If
                Loop
                For Each objGIRFData As RFDataStructure.GIRFMessageData In objGIRMessage.arrGIRFData
                    objCrateList = New CrateList
                    With objCrateList
                        strStatus = objGIRFData.cBookedIn
                        Select Case strStatus
                            Case "B"
                                .BookedIn = Macros.Y
                            Case "U"
                                iChildCount += 1
                                .BookedIn = Macros.N
                            Case "A"
                                .BookedIn = strStatus
                        End Select


                    End With
                    'arrItemDetails.Add(objItemList)
                Next
                m_RFDataManager.iPointer = Message.INITIALPOINTER
            Else
                Return False

            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:GetUODDetails():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        Return bTemp
    End Function

    ' V1.1 - KK
    ' Added new function CheckDallasStore

    ''' <summary>
    ''' The function is used to check if the store is enabled for Dallas positive receiving
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function CheckDallasStore() As Boolean

        Dim cReceivedData As String = ""
        Try
            If m_RFDataManager.SendDAC() Then
                'If DAC data send successfully to Transact then process the logic below
                If m_RFDataManager.WaitForResponse(cReceivedData) Then
                    'Verifying the response receivied back from Transact
                    If cReceivedData.Substring(0, 3) = Message.ACK Then
                        'Setting the Dallas receipt enabled flag "True"
                        objAppContainer.bDallasPosReceiptEnabled = True
                        Return True
                    ElseIf cReceivedData.Contains("NAKERROR") Then
                        'setting the Dallas receipt enabled flag "False"
                        objAppContainer.bDallasPosReceiptEnabled = False
                        Return False
                    Else
                        'setting the Dallas receipt enabled flag "False"
                        objAppContainer.bDallasPosReceiptEnabled = False
                        Return True
                    End If
                End If
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("RFDatasource:CheckDallasStore():" + _
                                           ex.Message.ToString(), _
                                           Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try

    End Function



    Public Overrides Function Signon(ByRef strPassword As String) As Boolean
        Return True
    End Function
    Public Overrides Function GetBootsCode(ByVal strProductCode As String) As String
        Return True
    End Function
    Public Overrides Function GetItemList(ByVal strCartonNumber As String, _
                                             ByVal arrItemList As ArrayList) As Boolean
        Return True

    End Function
    Public Overrides Function GetCartonList(ByVal strSupplierNo As String, ByRef arrCartonList As ArrayList) As Boolean
        Return True
    End Function

    Public Enum UODContainerType
        Dolly
        Crate
        RollyCage
        Pallet
        InterStoreTransfer
    End Enum

End Class
#End If