''******************************************************************************
'' Modification Log 
''******************************************************************************* 
'' No:       Author            Date            Description 
'' 1.1     Christopher Kitto  09/04/2015   Modified as part of DALLAS project.
''           (CK)                         Amended function GetBookInDeliverySummary()
''                                        to retrieve the Dallas delivery summary 
''                                        and to populate arraylist holding the Dallas 
''                                        UOD details. Added new function 
''                                        SendDallasDeliveryConfirmation.
''        Kiran Krishnan      18/05/2015  Added new function ValidateDallasUODScanned(),
''                                        GetDallasUODListForView() as part of 
''                                        Dallas positive receiving project                     
''********************************************************************************

Imports System
Imports System.Data
Imports System.Data.SqlServerCe
Imports System.Linq
Imports Goodsin.GIValueHolder
Imports System.Globalization

#If NRF Then
''' <summary>
''' DataSource Class for Non-RF Goods In Application
''' </summary>
''' <remarks></remarks>
Public Class NRFDataSource
    Inherits DataEngine


#Region "Constant Values"
    Private Const YES = "Y"
    Private Const NO = "N"
    Private Const BOOKED = "B"
    Private Const UNBOOKED = "U"
    Private Const AUDITED = "A"
    Private Const EXPECTED = "E"
    Private Const OUTSTANDING = "O"
    Private Const TODAY = "T"
    Private Const FUTURE = "F"
    Private Const ASNSUPPLIER = "A"
    Private Const DIRECTSUPPLIER = "D"
    Private Const STATICSUPPLIER = "S"
    Private Const SSC = "1"
    Private Const ASN = "3"
    Private Const DIRECTS = "2"
    Private Const BOOKIN = "1"
    Private Const AUDIT = "2"
    Private Const VIEW = "3"
    Private Const NONE = "X"
    Private Const INITPOINTER As Integer = 0
#End Region

    Public objNRFDataManager As NRFDataManager
    Sub New()
        objNRFDataManager = New NRFDataManager
    End Sub
    Public Overrides Function LogOff(Optional ByVal isCallForCrash As Boolean = False) As Boolean
        Return objNRFDataManager.CreateOFF(isCallForCrash)
    End Function
    ''' <summary>
    ''' The function writes the Sign on record to the export data file
    ''' </summary>
    ''' <param name="strPassword"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function SignOn(ByRef strPassword As String) As Boolean
        Return objNRFDataManager.CreateSOR(strPassword)
    End Function

    ' V1.1 - CK
    ' Amended the function to pass an additional argument - arraylist to hold the details
    ' of DALLAS UODs and to populate the arraylist.
    ' Public Overrides Function GetBookInDeliverySummary(ByRef arrDelvSummary As ArrayList) As Boolean

    ''' <summary>
    ''' The function gets the Delivery summary details for Book in Delvery SSC Receiving
    ''' </summary>
    ''' <param name="arrDelvSummary"></param>
    ''' <param name="arrDALLASDelSummary"></param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the delivery summary.
    ''' </returns><remarks>
    ''' </remarks>

    Public Overrides Function GetBookInDeliverySummary(ByRef arrDelvSummary As ArrayList, ByRef arrDALLASDelSummary As ArrayList) As Boolean
        Dim strSQLCmd As String = QueryMacro.GET_DELIVERY_SUMMARY
        Dim objGIARecord As New RFDataStructure.GIARecord
        Dim objDeliverySummary As DeliverySummary
        Dim dsList As DataSet = Nothing
        Dim cDallasSQLCmd As String = QueryMacro.GET_DALLAS_DELIVERY_SUMMARY
        Dim objDallasDeliverySummary As DallasDeliverySummary
        Dim dsDallasList As DataSet = Nothing
        Try
            ' v1.1 - CK
            ' Retrieve the Dallas delivery summary if the store is enabled for
            ' Dallas positive receiving
            If objAppContainer.bDallasPosReceiptEnabled Then      
                dsDallasList = New DataSet()
                'execute the query
                dsDallasList = objAppContainer.objDBConnection.RunSQLGetDataSet(cDallasSQLCmd)
                'process dataset and retreive the details in it.
                If dsDallasList.Tables(0).Rows.Count > 0 Then
                    For iCount As Integer = 0 To dsDallasList.Tables(0).Rows.Count - 1
                        objDallasDeliverySummary = New DallasDeliverySummary
                        With objDallasDeliverySummary
                            .DallasBarcode = dsDallasList.Tables(0).Rows(iCount)("DalUOD_Num").ToString().PadLeft(14, "0")
                            .ExpectedDelDate = Format(dsDallasList.Tables(0).Rows(iCount)("DalUOD_Exp_Date"), "yyMMdd")
                            .Status = System.Convert.ToChar(dsDallasList.Tables(0).Rows(iCount)("DalUOD_Status").ToString())
                        End With
                        arrDALLASDelSummary.Add(objDallasDeliverySummary)
                    Next
                End If
            End If

            dsList = New DataSet()
            'execute the query
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSQLCmd)
            'process dataset and retreive the details in it.
            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objDeliverySummary = New DeliverySummary
                    With objDeliverySummary
                        .SummaryType = dsList.Tables(0).Rows(iCount)("Summary_Type").ToString()
                        .ContainerType = dsList.Tables(0).Rows(iCount)("Container_Type").ToString()
                        .ContainerQty = dsList.Tables(0).Rows(iCount)("Container_Quantity").ToString()
                    End With
                    arrDelvSummary.Add(objDeliverySummary)
                Next
            Else
                'Fix to enter the Book in module if there are no expected/outstadning UODS
                Return True
            End If
            With objGIARecord
                .strDeliveryType = SSC
                .strFunction = BOOKIN
                .strPeriod = "X"
                .strRequestType = "S"
                .iPointer = INITPOINTER
            End With
            objNRFDataManager.CreateGIA(objGIARecord)


        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetBookInDeliverySummary: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
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
    Public Overrides Function ValidateUODScanned(ByVal strUODNumber As String, ByRef objUODInfo As UODInfo, ByVal tyFunction As AppContainer.FunctionType) As Boolean

        Dim strSQLCmd As String = QueryMacro.GET_UOD_SCAN_DATA
        Dim strUODStatus As String
        Dim strUODType As Char
        Dim strAuditStatus As String
        Dim strBOLUOD As String
        Dim strExpectedDate As Date
        Dim sqlResultSet As SqlCeDataReader
        Try
            If strUODNumber.Length = 14 Then
                strUODNumber = strUODNumber.Substring(4, 10)
            Else
                strUODNumber = strUODNumber.Substring(0, 10)
            End If

            strSQLCmd = String.Format(strSQLCmd, strUODNumber)
            'execute the query
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSQLCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then

                With objUODInfo
                    .BookInDate = sqlResultSet("Booked_In_Date").ToString()
                    'Minu store date in yyMMdd format
                    .DespatchDate = Format(sqlResultSet("Despatch_Date"), "yyMMdd")
                    strAuditStatus = sqlResultSet("Audited").ToString()
                    strUODStatus = sqlResultSet("Booked_In").ToString()
                    If (strAuditStatus = YES) Then
                        .UODStatus = AUDITED
                    ElseIf strUODStatus = YES Then
                        .UODStatus = BOOKED
                    ElseIf strUODStatus = NO Then
                        .UODStatus = UNBOOKED
                    End If
                    .UODNumber = strUODNumber
                    strUODType = sqlResultSet("Outer_Type").ToString()
                    .UODType = strUODType
                    'Minu REmoved the line as date already in datetime format
                    'strExpectedDate = objAppContainer.objHelper.ConvertToDate(sqlResultSet("Estimated_Delivery_Date").ToString())
                    'Minu Added new line for expected delivery date
                    strExpectedDate = sqlResultSet("Estimated_Delivery_Date")
                    If strExpectedDate >= DateTime.Now.Date Then
                        .ExpectedDeliveryDate = EXPECTED
                    Else
                        .ExpectedDeliveryDate = OUTSTANDING
                    End If
                    '  .NoOfChildren = sqlResultSet("No_Of_Children").ToString()
                    .UltLicensePlate = sqlResultSet("Ultimate_License_Number").ToString()
                    .ImmLicenseNo = sqlResultSet("Immediate_License_Number").ToString()
                    If strUODType = "D" Then

                        .NoOfChildren = sqlResultSet("Number_Childrens").ToString()
                        strBOLUOD = sqlResultSet("Number_Of_Items").ToString()
                        .NoOfItems = "0"

                    Else

                        strBOLUOD = sqlResultSet("Number_Childrens").ToString()
                        .NoOfItems = sqlResultSet("Number_Of_Items").ToString()
                        .NoOfChildren = "0"
                    End If
                    If strBOLUOD = "B" Then
                        .BOLUOD = YES
                    Else
                        .BOLUOD = NO
                    End If
                    .PartialBkd = sqlResultSet("Partial_Booked").ToString()
                    .UODReason = sqlResultSet("Reason").ToString()
                    'CR if Dolly is Partial Booked Then 
                    If .PartialBkd = Macros.Y Then
                        'CR set UOD Status to Unbooked 
                        .UODStatus = UNBOOKED
                        'CR set Partial booked flag to TRUE
                        BDSessionMgr.GetInstance().bPartialBkd = True
                    End If
                End With
            Else
                'If any error occured in reading the data adapter then return false.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource ValidateUODScanned: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function

    ' V1.1 - KK
    ' Added new function ValidateDallasUODScanned
    
    ''' <summary>
    ''' The Function validates the Scanned UOD and get the details if it is valid
    ''' </summary>
    ''' <param name="cUODNumber"></param>
    ''' <param name="objDalUODInfo"></param>
    ''' <param name="arrDallasDelSummary"></param>
    ''' <param name="tyFunction"></param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the scanned UOD.
    ''' </returns>
    ''' <remarks></remarks>
    Public Overrides Function ValidateDallasUODScanned(ByVal cUODNumber As String, ByRef objDalUODInfo As GIValueHolder.DallasScanDetail, ByRef arrDallasDelSummary As ArrayList, ByVal tyFunction As AppContainer.FunctionType) As Boolean

        Dim cQLCmd As String = QueryMacro.GET_DALLASUOD_SCAN_DATA
        Dim sqlResultSet As SqlCeDataReader
        Try
            cUODNumber = cUODNumber.Substring(1, 13)
            cQLCmd = String.Format(cQLCmd, cUODNumber)
            'execute the query
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(cQLCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                objDalUODInfo = New GIValueHolder.DallasScanDetail
                With objDalUODInfo
                    .DallasBarcode = sqlResultSet("DalUOD_Num").ToString().PadLeft(14, "0")
                    .DallasExpectedDate = Format(sqlResultSet("DalUOD_Exp_Date"), "yyMMdd")
                    .ScanStatus = System.Convert.ToChar(sqlResultSet("DalUOD_Status").ToString())
                End With
            Else
                'If any error occured in reading the data adapter then return false.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource ValidateDallasUODScanned: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function

    ''' <summary>
    ''' The function sets the Book in details to GIF Record for writing to ExportDataFile
    ''' </summary>
    ''' <param name="arrUODDetails"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function SendBookInDetails(ByRef arrUODDetails As ArrayList) As Boolean
        Dim objGIFMessage As RFDataStructure.GIFRecord
        Try
            For Each objBookInDetails As ScanDetails In arrUODDetails
                objGIFMessage = New RFDataStructure.GIFRecord
                With objGIFMessage
                    .strDeliveryType = SSC
                    .strFunction = BOOKIN
                    .strTransactionID = "GIF"
                    .strOperatorID = objAppContainer.strCurrentUserID
                    .strScanCode = objBookInDetails.ScannedCode.PadLeft(20, "0")
                    .strScanDate = objBookInDetails.ScanDate
                    .strScanTime = objBookInDetails.ScanTime
                    .cScanLevel = objBookInDetails.ScanLevel
                    .cScanType = objBookInDetails.ScanType
                    .strDespatchDate = objBookInDetails.DespatchDate
                    .strDriverBadge = "XXXXXXXX"
                    .strBarcode = "X".PadLeft(13, "X")
                    .strQuantity = "XXXXXX"
                    .strItemStatus = "XXXXX"
                    .cBatchRescan = NONE
                    .cGITNote = " "
                    .Sequence = "XX"
                End With
                objNRFDataManager.CreateGIF(objGIFMessage)
            Next

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource SendBookinDetails: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function
    ''' <summary>
    ''' The function writes the batch confirmation data to the export data file
    ''' </summary>
    ''' <param name="objDriverDetails"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function SendBatchConfirmation(ByRef objDriverDetails As DriverDetails) As Boolean
        Dim objGIFMessage As New RFDataStructure.GIFRecord
        Try
            With objGIFMessage
                .strDeliveryType = SSC
                .strFunction = BOOKIN
                .strOperatorID = objAppContainer.strCurrentUserID
                .strScanCode = "X".PadLeft(20, "X")
                .strScanDate = objDriverDetails.ScanDate
                .strScanTime = objDriverDetails.ScanTime
                .cScanLevel = NONE
                .cScanType = "C"
                .strDespatchDate = "XXXXXX"
                .strDriverBadge = objDriverDetails.DriverBadge
                .cBatchRescan = Convert.ToChar(objDriverDetails.BatchRescan)
                .cGITNote = " "
                .strBarcode = "X".PadLeft(13, "X")
                .strItemStatus = "XXXXX"
                .strQuantity = "XXXXXX"
                .Sequence = "XX"
            End With
            objNRFDataManager.CreateGIF(objGIFMessage)
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            "Exception occured at DataSource SendBatchConfirmation: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function
    ''' <summary>
    ''' The function writes the Delivery confirmation data to the export data file
    ''' </summary>
    ''' <param name="objDriverDetails"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function SendDeliveryConfirmation(ByRef objDriverDetails As DriverDetails) As Boolean
        Dim objGIFMessage As New RFDataStructure.GIFRecord
        Try
            With objGIFMessage
                .strDeliveryType = SSC
                .strFunction = BOOKIN
                .strOperatorID = objAppContainer.strCurrentUserID
                .strScanCode = "X".PadLeft(20, "X")
                .strScanDate = objDriverDetails.ScanDate
                .strScanTime = objDriverDetails.ScanTime
                .cScanLevel = NONE
                .cScanType = "S"
                .strDespatchDate = "XXXXXX"
                .strDriverBadge = objDriverDetails.DriverBadge
                .cBatchRescan = NONE
                .cGITNote = objDriverDetails.GITNote
                .strBarcode = "X".PadLeft(13, "X")
                .strItemStatus = "XXXXX"
                .strQuantity = "XXXXXX"
                .Sequence = "XX"
            End With
            objNRFDataManager.CreateGIF(objGIFMessage)
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            "Exception occured at DataSource SendDeliveryConfirmation: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function

    ' V1.1 - CK
    '  Added function SendDallasDeliveryConfirmation to send Dallas Delivery confirmation message

    ''' <summary>
    ''' Writes the Dallas delivery confirmation to the export data file.
    ''' </summary>
    ''' <param name="arrDallasUODDetails"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function SendDallasDeliveryConfirmation(ByRef arrDallasUODDetails As ArrayList) As Boolean
        Dim objDARMessage As RFDataStructure.DARRecord

        Try
            For Each objDallasScannedDetails As DallasScanDetail In arrDallasUODDetails
                objDARMessage = New RFDataStructure.DARRecord
                With objDARMessage
                    .cDallasBarcode = objDallasScannedDetails.DallasBarcode
                    .cScanDate = objDallasScannedDetails.DallasScanDate
                    .cScanStatus = objDallasScannedDetails.ScanStatus
                End With
                objNRFDataManager.CreateDAR(objDARMessage)
            Next
        Catch ex As Exception
            ' Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource SendDallasDeliveryConfirmation: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        ' If successfully updated the details
        Return True
    End Function
    ''' <summary>
    ''' Writes the GIX for each function exit to Export data file
    ''' </summary>
    ''' <param name="tyDeliveryType"></param>
    ''' <param name="tyFunction"></param>
    ''' <param name="tyIsAbort"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function SendSessionExit(ByVal tyDeliveryType As AppContainer.DeliveryType, ByVal tyFunction As AppContainer.FunctionType, ByVal tyIsAbort As AppContainer.IsAbort) As Boolean
        Dim objGIXMessage As New RFDataStructure.GIXRecord
        Try
            With objGIXMessage
                .strDeliveryType = CType(tyDeliveryType, Integer).ToString()
                .strFunction = CType(tyFunction, Integer).ToString()
                .strOperatorID = objAppContainer.strCurrentUserID
                .strTransactionID = "GIX"
                If tyIsAbort = IsAbort.No Then
                    .cIsAbort = NO
                Else
                    .cIsAbort = YES
                End If
            End With
            objNRFDataManager.CreateGIX(objGIXMessage)
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource SendSessionExit: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function

    ''' <summary>
    ''' The function writes the GIA to exportdata file when audit UOD is initiated
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAuditSummary() As Boolean
        Dim objGIAMessage As New RFDataStructure.GIARecord
        Try
            With objGIAMessage
                .strDeliveryType = SSC
                .strFunction = AUDIT
                .strOperatorID = objAppContainer.strCurrentUserID
                .strRequestType = "S"
                .strPeriod = NONE
                .iPointer = 0
            End With
            If objNRFDataManager.CreateGIA(objGIAMessage) Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetAuditSummary: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
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
    Public Overrides Function GetItemDetails(ByVal strItemCode As String, ByVal strType As AppContainer.ItemDetailType, ByRef objItemInfo As ItemInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_ITEM_DETAILS
        Dim strSqlCmdBootsCode As String = QueryMacro.GET_ITEM_DETAILS_BOOTSCODE
        Dim strPC As String
        'Dim dsList As DataSet = Nothing


        Dim sqlResultSet As SqlCeDataReader


        Try
            'dsList = New DataSet()
            If strItemCode.Length <= 7 Then
                strItemCode = objAppContainer.objHelper.GenerateBCwithCDV(strItemCode)
                strSqlCmd = strSqlCmdBootsCode
                strPC = strItemCode
            ElseIf strItemCode.Substring(0, 6) = "000000" Then
                strItemCode = objAppContainer.objHelper.GenerateBCwithCDV(strItemCode.Substring(6, 6))
                strSqlCmd = strSqlCmdBootsCode
                strPC = strItemCode
            Else
                'Minu Remove padleft
                'strItemCode = strItemCode.PadLeft(13, "0")
                strPC = strItemCode.Remove(strItemCode.Length - 1, 1)
            End If

            strSqlCmd = String.Format(strSqlCmd, strPC)
            'execute the query
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                'If dsList.Tables(0).Rows.Count > 0 Then
                With objItemInfo
                    .BootsCode = sqlResultSet("Boots_Code").ToString()
                    .ItemDesc = sqlResultSet("SEL_Desc").ToString()
                    '.BootsCode = dsList.Tables(0).Rows(0)("Boots_Code").ToString()
                    '.ItemDesc = dsList.Tables(0).Rows(0)("Item_Desc").ToString()
                    .ItemQty = "1"
                    .ProductCode = objAppContainer.objHelper.GeneratePCwithCDV(sqlResultSet("Second_Barcode").ToString())
                    '.ProductCode = objAppContainer.objHelper.GeneratePCwithCDV(dsList.Tables(0).Rows(0)("Second_Barcode").ToString())
                    If .ProductCode.Trim("0") = "" Then
                        '.ProductCode = objAppContainer.objHelper.GeneratePCwithCDV(dsList.Tables(0).Rows(0)("First_Barcode").ToString())
                        .ProductCode = objAppContainer.objHelper.GeneratePCwithCDV(sqlResultSet("First_Barcode").ToString())

                    End If
                End With
            Else
                'If any error occured in reading the data adapter then return false.
                'Return False
                If strItemCode.Length <= 7 Then
                    'strItemCode = objAppContainer.objHelper.GenerateBCwithCDV(strItemCode)
                    With objItemInfo
                        .BootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strItemCode)
                        .ItemDesc = "Unknown Item"
                        .ItemQty = "1"
                        .ProductCode = objAppContainer.objHelper.GeneratePCwithCDV(strItemCode.Substring(0, 6)).PadLeft(13, "0")
                    End With
                Else
                    With objItemInfo
                        ' need to check this logic as the discripancy screen can cause issues.
                        '????????????????????????
                        .BootsCode = strItemCode 'objAppContainer.objHelper.GenerateBCwithCDV(strItemCode.Substring(6, 6))
                        .ItemDesc = "Unknown Item"
                        .ItemQty = "1"
                        .ProductCode = strItemCode
                    End With
                End If
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetItemDetails: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function
    ''' <summary>
    ''' The function writes the GIF for Item Count into exportdata file
    ''' </summary>
    ''' <param name="iItemCount"></param>
    ''' <param name="DelType"></param>
    ''' <param name="FuncType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function SendItemQuantity(ByRef iItemCount As Integer, ByVal DelType As AppContainer.DeliveryType, ByVal FuncType As AppContainer.FunctionType) As Boolean
        Dim objGIFMessage As New RFDataStructure.GIFRecord
        Try

            With objGIFMessage
                .strTransactionID = "GIF"
                .strDeliveryType = CType(DelType, Integer).ToString()
                .strFunction = CType(FuncType, Integer).ToString()
                .strOperatorID = objAppContainer.strCurrentUserID
                .strScanCode = "X".PadLeft(20, "X")
                .strScanDate = "XXXXXXXX"
                .strScanTime = "XXXXXX"
                .cScanLevel = NONE
                .cScanType = NONE
                .strDespatchDate = "XXXXXX"
                .strDriverBadge = "XXXXXXXX"
                .strBarcode = "X".PadLeft(13, "X")
                .strQuantity = "XXXXXX"
                If DelType = AppContainer.DeliveryType.Directs AndAlso FuncType = AppContainer.FunctionType.BookIn Then
                    .strItemStatus = "XXXXX"
                Else
                    .strItemStatus = (iItemCount + 2).ToString().PadLeft(5, "0")
                End If

                .cBatchRescan = NONE
                .cGITNote = " "
                .Sequence = "XX"
            End With
            objNRFDataManager.CreateGIF(objGIFMessage)



        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource SendItemQuantity: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
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
    Public Overrides Function GetUODListForView(ByVal strPeriod As String, ByRef arrUODList As ArrayList) As Boolean

        Dim strSqlCmd As String = QueryMacro.GET_UOD_LIST
        Dim DateToday As Date = DateTime.Now.Date
        Dim strAuditStatus As String
        Dim strExptDate As Date
        Dim strDate = Format(DateTime.Now, "yyMMdd")
        Dim strBookedInDate As String
        Dim dsList As DataSet = Nothing
        Dim objUODList As UODList
        Dim bToday As Boolean = False
        Try
            If (strPeriod = TODAY) Then
                bToday = True
                ' strSqlCmd = strSqlCmdToday
            Else
                bToday = False
                ' strSqlCmd = strSqlCmdFuture
            End If
            'execute the query
            dsList = New DataSet()
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'process dataset and retreive the details in it.
            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    'Minu commented below line
                    ' strExptDate = objAppContainer.objHelper.ConvertToDate(dsList.Tables(0).Rows(iCount)("Estimated_Delivery_Date").ToString())
                    'Minu Added
                    strExptDate = dsList.Tables(0).Rows(iCount)("Estimated_Delivery_Date")
                    'checking if user selected TOday or Future
                    If bToday Then
                        'filtering data for Today's selection
                        'If ((strExptDate = DateToday) Or (strExptDate.AddDays(1) = DateToday) _
                        '      Or (strExptDate.AddDays(2) = DateToday) _
                        '      Or (strExptDate.AddDays(3) = DateToday)) Then
                        'Filtering option changed to check for today's delivery or whatever is previous.
                        If ((strExptDate = DateToday) Or (strExptDate < DateToday)) Then
                            objUODList = New UODList
                            With objUODList
                                .UODID = dsList.Tables(0).Rows(iCount)("UOD_License_Number").ToString()
                                .UODType = dsList.Tables(0).Rows(iCount)("Outer_Type").ToString()
                                strAuditStatus = dsList.Tables(0).Rows(iCount)("Audited").ToString()
                                If strAuditStatus = YES Then
                                    .BookedIn = AUDITED
                                Else
                                    .BookedIn = dsList.Tables(0).Rows(iCount)("Booked_In").ToString()
                                End If
                                .ExptDate = dsList.Tables(0).Rows(iCount)("Estimated_Delivery_Date").ToString()
                                .BookedInDate = dsList.Tables(0).Rows(iCount)("Booked_In_Date").ToString()
                                .Reason = dsList.Tables(0).Rows(iCount)("Reason").ToString()
                            End With
                            If strExptDate = DateToday Then
                                arrUODList.Add(objUODList)
                            ElseIf objUODList.BookedIn = NO Then
                                arrUODList.Add(objUODList)
                            End If
                        End If
                    Else
                        'filtering data for Future
                        If (strExptDate > DateToday) Then
                            objUODList = New UODList
                            With objUODList
                                .UODID = dsList.Tables(0).Rows(iCount)("UOD_License_Number").ToString()
                                .UODType = dsList.Tables(0).Rows(iCount)("Outer_Type").ToString()
                                strAuditStatus = dsList.Tables(0).Rows(iCount)("Audited").ToString()
                                If strAuditStatus = YES Then
                                    .BookedIn = AUDITED
                                Else
                                    .BookedIn = dsList.Tables(0).Rows(iCount)("Booked_In").ToString()
                                End If
                                .ExptDate = dsList.Tables(0).Rows(iCount)("Estimated_Delivery_Date").ToString()
                                .BookedInDate = dsList.Tables(0).Rows(iCount)("Booked_In_Date").ToString()
                                .Reason = dsList.Tables(0).Rows(iCount)("Reason").ToString()
                            End With
                            arrUODList.Add(objUODList)
                        End If
                    End If
                Next
            Else
                'If any error occured in reading the data adapter then return false.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetUODListForView: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True

    End Function
	
	' V1.1 - KK
	' Added new function GetDallasUODListForView

	''' <summary>
    ''' The function retrieves the Dallas UOD list for View UOD 
    ''' </summary>
    ''' <param name="strPeriod"></param>
    ''' <param name="arrUODList"></param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If any error occurred while updating the results or 
    ''' there is no details available for the Dallas UOD List for view.
    ''' </returns>
    ''' <remarks></remarks>

    Public Overrides Function GetDallasUODListForView(ByRef arrDalViewUOD As ArrayList) As Boolean
        Dim cDallasSQLCmd As String = QueryMacro.GET_DALLAS_DELIVERY_SUMMARY
        Dim objDallasDeliverySummary As DallasDeliverySummary
        Dim dsDallasList As DataSet = Nothing
        Try
            ' Retrieve the Dallas delivery summary if the store is enabled for
            ' Dallas positive receiving
                dsDallasList = New DataSet()
            'execute the query
            dsDallasList = objAppContainer.objDBConnection.RunSQLGetDataSet(cDallasSQLCmd)
            'process dataset and retreive the details in it.
            If dsDallasList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsDallasList.Tables(0).Rows.Count - 1
                    objDallasDeliverySummary = New DallasDeliverySummary
                    With objDallasDeliverySummary
                        .DallasBarcode = dsDallasList.Tables(0).Rows(iCount)("DalUOD_Num").ToString().PadLeft(14, "0")
                        .ExpectedDelDate = Format(dsDallasList.Tables(0).Rows(iCount)("DalUOD_Exp_Date"), "yyMMdd")
                        .Status = System.Convert.ToChar(dsDallasList.Tables(0).Rows(iCount)("DalUOD_Status").ToString())
                    End With
                    arrDalViewUOD.Add(objDallasDeliverySummary)
                Next
            Else
                Return False
            End If

        Catch ex As Exception
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetDallasUODListForView: " _
                                                                            + ex.StackTrace, Logger.LogLevel.RELEASE)
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
                                    Optional ByVal objUODinfo As UODInfo = Nothing, _
                                     Optional ByVal strSequence As String = "XXXXX") As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_CRATE_LIST
        strSqlCmd = String.Format(strSqlCmd, strUODNumber)
        Dim strAuditStatus As String
        Dim dsList As DataSet = Nothing
        Dim objCrateList As CrateList
        Try
            ValidateUODScanned(strUODNumber, objUODinfo, AppContainer.FunctionType.View)
            'execute the query
            'sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            dsList = New DataSet()
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'process dataset and retreive the details in it.
            '  If sqlResultSet.Read Then
            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objCrateList = New CrateList
                    With objCrateList
                        .CrateId = dsList.Tables(0).Rows(iCount)("UOD_License_Number").ToString()
                        .CrateType = dsList.Tables(0).Rows(iCount)("Outer_Type").ToString()
                        strAuditStatus = dsList.Tables(0).Rows(iCount)("Audited").ToString()
                        If strAuditStatus = YES Then
                            'if Audit then status is A
                            .BookedIn = AUDITED
                        Else
                            .BookedIn = dsList.Tables(0).Rows(iCount)("Booked_In").ToString()
                        End If
                        .NoOfItems = dsList.Tables(0).Rows(iCount)("Number_Of_Items").ToString()


                    End With
                    arrCrateList.Add(objCrateList)
                Next
            Else
                'If any error occured in reading the data adapter then return false.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetCrateListForView: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'Dispose the data set object.
            dsList.Dispose()
        End Try
        'if successfully updated the details
        Return True

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
        '////////////////////////////////////////
        Dim strSQLCmdForUOD As String = QueryMacro.GET_UOD_SCAN_DATA
        Dim strUODStatus As String
        Dim strUODType As Char
        Dim strAuditStatus As String = ""

        Dim sqlResultSet As SqlCeDataReader
        Dim strSqlCmd As String = QueryMacro.GET_UODITEM_LIST
        strSqlCmd = String.Format(strSqlCmd, strCrateId)

        Dim dsList As DataSet = Nothing
        Dim objItemList As ItemList
        Try
            If Not objUODinfo Is Nothing Then

                strSQLCmdForUOD = String.Format(strSQLCmdForUOD, strCrateId)
                'execute the query
                sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSQLCmdForUOD)
                'process dataset and retreive the details in it.
                If sqlResultSet.Read Then

                    With objUODinfo
                        .BookInDate = sqlResultSet("Booked_In_Date").ToString()
                        'Minu Format as yyMMdd
                        .DespatchDate = Format(sqlResultSet("Despatch_Date"), "yyMMdd")
                        strAuditStatus = sqlResultSet("Audited").ToString()
                        .UODStatus = sqlResultSet("Booked_In").ToString()
                        If strAuditStatus = YES Then
                            .UODStatus = AUDITED
                        End If
                        .UODNumber = strCrateId
                        strUODType = sqlResultSet("Outer_Type").ToString()
                        .UODType = strUODType
                        .ExpectedDeliveryDate = sqlResultSet("Estimated_Delivery_Date").ToString()

                    End With
                Else
                    'If any error occured in reading the data adapter then return false.
                    Return False
                End If
            End If
            'execute the query
            dsList = New DataSet()
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'process dataset and retreive the details in it.
            '  If sqlResultSet.Read Then
            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objItemList = New ItemList
                    With objItemList
                        .ProductCode = objAppContainer.objHelper.GeneratePCwithCDV(dsList.Tables(0).Rows(iCount)("Second_Barcode").ToString())
                        .ItemCode = dsList.Tables(0).Rows(iCount)("Boots_Code").ToString()
                        .ItemDesc = dsList.Tables(0).Rows(iCount)("Item_Desc").ToString()
                        .ItemQty = dsList.Tables(0).Rows(iCount)("Despatch_Quantity").ToString()
                    End With
                    arrItemList.Add(objItemList)
                Next

            Else
                'If any error occured in reading the data adapter then return false.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetitemListForView: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False


        Finally
            sqlResultSet = Nothing
            'Dispose the data set object.
            dsList.Dispose()
        End Try
        'if successfully updated the details
        Return True

    End Function
    ''' <summary>
    ''' The function retrieves the list of suppliers
    ''' </summary>
    ''' <param name="arrSupplierList"></param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the Suppliers.
    ''' </returns>
    ''' <remarks></remarks>
    Public Overrides Function GetSupplierList(ByRef arrSupplierList As ArrayList) As Boolean
        'Dim strSqlCmdASN As String = QueryMacro.GET_SUPPLIER_LIST_ASN
        'Dim strSqlCmdDirects As String = QueryMacro.GET_SUPPLIER_LIST_DIRECTS
        Dim strSqlCmdASN As String = QueryMacro.GET_SUPPLIER_LIST_UNBOOKED_ASN
        Dim strSqlCmdDirects As String = QueryMacro.GET_SUPPLIER_LIST_DIRECTS_UNBOOKED
        Dim strSqlCmdStaticSupplier As String = QueryMacro.GET_SUPPLIER_LIST_STATIC
        Dim dsListASN As DataSet = Nothing
        Dim dsListDirects As DataSet = Nothing
        Dim dsListStatic As DataSet = Nothing
        Dim bTemp As Boolean = False
        ' Dim sqlResultSet As SqlCeDataReader
        Dim objSupplierList As SupplierList
        Dim strSupplierType As String
        Dim strStaticSupplier As String
        Dim iQuantity As Integer
        Dim objGIARecord As RFDataStructure.GIARecord
        Try
            If arrSupplierList.Count > 0 Then
                arrSupplierList.Clear()
            End If
            dsListASN = New DataSet()
            'execute the query
            dsListASN = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmdASN)


            'process dataset and retreive the details in it.

            If dsListASN.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsListASN.Tables(0).Rows.Count - 1
                    strStaticSupplier = dsListASN.Tables(0).Rows(iCount)("Supplier_Static_Flag").ToString()
                    iQuantity = dsListASN.Tables(0).Rows(iCount)("Cartons_In_ASN")
                    If (strStaticSupplier = "S" AndAlso iQuantity = 0) Or iQuantity > 0 Then
                        objSupplierList = New SupplierList
                        With objSupplierList
                            .SupplierName = dsListASN.Tables(0).Rows(iCount)("Supplier_Name").ToString()
                            .SupplierNo = dsListASN.Tables(0).Rows(iCount)("Supplier_ID").ToString()
                            strSupplierType = dsListASN.Tables(0).Rows(iCount)("Supplier_ASN_Flag").ToString()
                            If strSupplierType = ASNSUPPLIER Then
                                .SupplierType = ASNSUPPLIER
                            Else
                                .SupplierType = DIRECTSUPPLIER
                            End If
                            .SupplierQty = iQuantity.ToString()

                            If strStaticSupplier = STATICSUPPLIER Then
                                'if supplier is a static supplier
                                .StaticSupplier = STATICSUPPLIER
                            Else
                                .StaticSupplier = NO
                            End If
                        End With
                        arrSupplierList.Add(objSupplierList)
                    End If


                Next
                bTemp = True
            Else
                bTemp = False
            End If
            'execute the query
            ' sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmdDirects)
            dsListDirects = New DataSet()
            dsListDirects = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmdDirects)
            'process dataset and retreive the details in it.

            If dsListDirects.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsListDirects.Tables(0).Rows.Count - 1
                    objSupplierList = New SupplierList
                    With objSupplierList
                        .SupplierName = dsListDirects.Tables(0).Rows(iCount)("Supplier_Name").ToString()
                        .SupplierNo = dsListDirects.Tables(0).Rows(iCount)("Supplier_ID").ToString()
                        strSupplierType = dsListDirects.Tables(0).Rows(iCount)("Supplier_ASN_Flag").ToString()
                        If strSupplierType = ASNSUPPLIER Then
                            .SupplierType = ASNSUPPLIER
                        Else
                            .SupplierType = DIRECTSUPPLIER
                        End If
                        strStaticSupplier = dsListDirects.Tables(0).Rows(iCount)("Supplier_Static_Flag").ToString()
                        If strStaticSupplier = STATICSUPPLIER Then
                            'if supplier is a static supplier
                            .StaticSupplier = STATICSUPPLIER
                        Else
                            .StaticSupplier = NO
                        End If
                        .SupplierQty = dsListDirects.Tables(0).Rows(iCount)("Cartons_In_ASN").ToString()
                    End With
                    arrSupplierList.Add(objSupplierList)
                Next
                bTemp = True
            Else
                If Not bTemp Then
                    bTemp = False
                End If
            End If
            dsListStatic = New DataSet()
            'execute the query
            dsListStatic = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmdStaticSupplier)


            'process dataset and retreive the details in it.

            If dsListStatic.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsListStatic.Tables(0).Rows.Count - 1
                    strStaticSupplier = dsListStatic.Tables(0).Rows(iCount)("Supplier_Static_Flag").ToString()
                    iQuantity = "0"
                    objSupplierList = New SupplierList
                    With objSupplierList
                        .SupplierName = dsListStatic.Tables(0).Rows(iCount)("Supplier_Name").ToString()
                        .SupplierNo = dsListStatic.Tables(0).Rows(iCount)("Supplier_ID").ToString()
                        strSupplierType = dsListStatic.Tables(0).Rows(iCount)("Supplier_ASN_Flag").ToString()
                        If strSupplierType = ASNSUPPLIER Then
                            .SupplierType = ASNSUPPLIER
                        Else
                            .SupplierType = DIRECTSUPPLIER
                        End If
                        .SupplierQty = iQuantity.ToString()

                        If strStaticSupplier = STATICSUPPLIER Then
                            'if supplier is a static supplier
                            .StaticSupplier = STATICSUPPLIER
                        Else
                            .StaticSupplier = NO
                        End If
                    End With
                    Dim linQuery = From objSupList As SupplierList In arrSupplierList Select objSupList _
                               Where objSupList.SupplierNo = objSupplierList.SupplierNo
                    If linQuery.Count = 0 Then
                        arrSupplierList.Add(objSupplierList)
                    End If
                Next
                bTemp = True
            Else
                If Not bTemp Then
                    bTemp = False
                End If
            End If
            '  If arrSupplierList.Count > 0 Then
            'arrSupplierList.Sort()
            '  End If
            With objGIARecord
                .strDeliveryType = DIRECTS
                .strFunction = BOOKIN
                .strPeriod = NONE
                .strRequestType = "S"
                .iPointer = 0
            End With
            objNRFDataManager.CreateGIA(objGIARecord)

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetSUpplierList: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False

        Finally
            'Dispose the data set object.
            dsListASN.Dispose()
            dsListDirects.Dispose()
            dsListStatic.Dispose()
            objGIARecord = Nothing
        End Try
        'if successfully updated the details
        'Return True
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
        Dim strSqlCmd As String = QueryMacro.GET_ORDER_LIST
        strSqlCmd = String.Format(strSqlCmd, strSupplierNo)
        Dim strStatus As String
        Dim strOrderNo As String
        Dim strSource As String
        Dim strOrderSuffix As String = " "
        Dim HexValue As String
        Dim StrValue As String = ""
        Dim strBC As String
        Dim dsList As DataSet = Nothing
        Dim objOrderList As OrderList
        Dim bTEmp As Boolean = False
        Try
            'execute the query
            dsList = New DataSet()
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'process dataset and retreive the details in it.
            '  If sqlResultSet.Read Then
            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objOrderList = New OrderList
                    With objOrderList
                        .EstDeliveryDate = dsList.Tables(0).Rows(iCount)("Exp_Delivery_Date").ToString()
                        strStatus = dsList.Tables(0).Rows(iCount)("Confirm_Flag").ToString()
                        If strStatus = "A" Or strStatus = "C" Then
                            .BookInStatus = BOOKED
                        Else
                            .BookInStatus = UNBOOKED
                        End If
                        HexValue = dsList.Tables(0).Rows(iCount)("BC").ToString()
                        StrValue = System.Convert.ToChar(System.Convert.ToUInt32(HexValue, 16)).ToString()
                        ' .Source = dsList.Tables(0).Rows(iCount)("Source").ToString()
                        strOrderNo = dsList.Tables(0).Rows(iCount)("Order_No").ToString()
                        strOrderSuffix = dsList.Tables(0).Rows(iCount)("Order_Suffix").ToString()
                        If strOrderSuffix = "0" Then
                            strOrderSuffix = " "
                        End If
                        .OrderNo = strOrderNo
                        .SupplierNo = dsList.Tables(0).Rows(iCount)("Supplier").ToString()

                        strBC = StrValue
                        If strBC = "0" Then
                            strBC = " "
                        End If
                        strSource = dsList.Tables(0).Rows(iCount)("Source").ToString()
                        If strSource = "0" Then
                            strSource = " "
                        End If
                        .Code = strSupplierNo + strOrderNo + strOrderSuffix + strBC + strSource + "00"
                    End With
                    'get the Order number with the latest delivery date
                    For Each objOrderNo As OrderList In arrOrderList
                        If objOrderNo.OrderNo = objOrderList.OrderNo Then
                            If objAppContainer.objHelper.ConvertToDate(objOrderList.OrderNo) < _
                                   objAppContainer.objHelper.ConvertToDate(objOrderNo.OrderNo) Then
                                arrOrderList.Remove(objOrderList)
                                bTEmp = True
                                Exit For
                            End If
                        End If
                    Next
                    If Not bTEmp Then
                        arrOrderList.Add(objOrderList)
                    End If

                Next
            Else
                'If any error occured in reading the data adapter then return false.
                Return False
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetOrderList: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'Dispose the data set object.
            dsList.Dispose()
        End Try
        'if successfully updated the details
        Return True
    End Function
    ''' <summary>
    ''' FUnction retrieves the list of Items for an Order selected 
    ''' </summary>
    ''' <param name="objOrderList"></param>
    ''' <param name="arrItemListForOrder"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetItemListForOrder(ByRef objOrderList As GIValueHolder.OrderList, _
                                                  ByRef arrItemListForOrder As ArrayList) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_ITEM_LIST_FOR_ORDERS
        strSqlCmd = String.Format(strSqlCmd, objOrderList.OrderNo, objOrderList.SupplierNo)

        Dim dsList As DataSet = Nothing
        Dim objItemListForOrder As ItemListForOrder
        Try
            'execute the query
            dsList = New DataSet()
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'process dataset and retreive the details in it.
            '  If sqlResultSet.Read Then
            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objItemListForOrder = New ItemListForOrder
                    With objItemListForOrder
                        .ProductCode = objAppContainer.objHelper.GeneratePCwithCDV(dsList.Tables(0).Rows(iCount)("Second_Barcode").ToString())
                        .BootsCode = dsList.Tables(0).Rows(iCount)("Boots_Code").ToString()
                        .ItemDesc = dsList.Tables(0).Rows(iCount)("Item_Desc").ToString()
                        .ExptdQty = dsList.Tables(0).Rows(iCount)("Exp_Quantity").ToString()
                        'to do- check if a zero needs to be padded left if the page no is just a single letter
                        .PageNo = dsList.Tables(0).Rows(iCount)("List_ID").ToString().PadLeft(2, "0")
                    End With
                    arrItemListForOrder.Add(objItemListForOrder)
                Next
            Else
                'If any error occured in reading the data adapter then return false.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetItemListForORder: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'Dispose the data set object.
            dsList.Dispose()
        End Try
        'if successfully updated the details
        Return True
    End Function
    Public Overrides Function GetUODChildCount(ByVal strUODNumber As String, _
                                                  ByRef iChildCount As Integer) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_UOD_COUNT
        strSqlCmd = String.Format(strSqlCmd, strUODNumber)
        Dim sqlResultSet As SqlCeDataReader
        Try
            'CR only get Unbooked Counts of a child UOD's in a Dolly if partially Bkd
            If BDSessionMgr.GetInstance().bPartialBkd Then
                'execute the query
                sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
                'process datareader and retreive the details in it.
                If sqlResultSet.Read Then
                    iChildCount = sqlResultSet("UODChildCount")
                Else
                    Return False
                End If
            Else
                Return False
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource ValidateUODScanned: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            sqlResultSet = Nothing
        End Try
        Return True
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
    Public Overrides Function ValidateSupplierNumber(ByVal SupplierNo As String, ByRef objSupplierData As SupplierData) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_SUPPLIER_DATA
        Dim strSupplierType As String
        Dim strStaticSupplier As String
        Dim sqlResultSet As SqlCeDataReader

        Try
            'execute the query
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)

            'process datareader and retrieve the details in it.
            If sqlResultSet.Read Then
                objSupplierData = New SupplierData
                With objSupplierData
                    .SupplierNo = sqlResultSet("Supplier_ID").ToString()
                    .SupplierName = sqlResultSet("Supplier_Name").ToString()
                    strSupplierType = sqlResultSet("Supplier_ASN_Flag").ToString()

                    If strSupplierType = ASNSUPPLIER Then
                        'if supplier type is ASN
                        .SupplierType = ASNSUPPLIER
                    Else
                        'if supplier type is DIRECTS
                        .SupplierType = DIRECTSUPPLIER
                    End If
                    strStaticSupplier = sqlResultSet("Supplier_Static_Flag").ToString()
                    If strStaticSupplier Then
                        'if supplier is a static supplier
                        .StaticSupplier = YES
                    Else
                        .StaticSupplier = NO
                    End If
                End With
            Else
                'If any error occured in reading the data adapter then return false.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource ValidateSupplierNumber: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False

        Finally
            'Clear the memory occupied by the variable.
            sqlResultSet = Nothing
        End Try
        'if successfully updated the details
        Return True
    End Function
    ''' <summary>
    ''' To get the configuration details for the Goods In application
    ''' </summary>
    ''' <param name="objConfigValue"></param>
    ''' <remarks></remarks>
    Public Overrides Function GetConfigValues(ByRef objConfigValue As AppContainer.ConfigValues) As Boolean
        Dim objGIARecord As New RFDataStructure.GIARecord
        Try
            With objConfigValue
                .ASNActive = ConfigDataMgr.GetInstance.GetParam(ConfigKey.ASN_ACTIVE)
                .UODActive = ConfigDataMgr.GetInstance.GetParam(ConfigKey.UOD_ACTIVE)
                .DirectsActive = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DIRECTS_ACTIVE)
                .ONightDelivery = ConfigDataMgr.GetInstance.GetParam(ConfigKey.ONIGHT_DELIVERY)
                .ONightScan = ConfigDataMgr.GetInstance.GetParam(ConfigKey.ONIGHT_SCAN)
                .BatchSize = ConfigDataMgr.GetInstance.GetParam(ConfigKey.SCAN_BATCH_SIZE)
            End With
            With objGIARecord
                .strDeliveryType = NONE
                .strFunction = NONE
                .strRequestType = "C"
                .strPeriod = NONE
                .iPointer = INITPOINTER
            End With
            objNRFDataManager.CreateGIA(objGIARecord)

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetConfigValues: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        Return True


    End Function
    ''' <summary>
    ''' The function writes the Item details to the exportdata file
    ''' </summary>
    ''' <param name="arrItemDetails"></param>
    ''' <param name="DelType"></param>
    ''' <param name="FuncType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function SendItemDetails(ByRef arrItemDetails As ArrayList, ByVal DelType As AppContainer.DeliveryType, ByVal FuncType As AppContainer.FunctionType) As Boolean
        Dim objGIFMessage As RFDataStructure.GIFRecord
        Dim iCount As Integer = 0
        Try

            For Each objItemDetails As ScanDetails In arrItemDetails
                objGIFMessage = New RFDataStructure.GIFRecord
                With objGIFMessage
                    If DelType = AppContainer.DeliveryType.SSC Then
                        .strDeliveryType = "1"
                    ElseIf DelType = AppContainer.DeliveryType.Directs Then
                        .strDeliveryType = "2"
                    ElseIf DelType = AppContainer.DeliveryType.ASN Then
                        .strDeliveryType = "3"
                    End If
                    If FuncType = AppContainer.FunctionType.BookIn Then
                        .strFunction = "1"
                    ElseIf FuncType = AppContainer.FunctionType.Audit Then
                        .strFunction = "2"
                    ElseIf FuncType = AppContainer.FunctionType.View Then
                        .strFunction = "3"
                    End If

                    .strTransactionID = "GIF"
                    .strOperatorID = objAppContainer.strCurrentUserID

                    If objItemDetails.ScannedCode = Nothing Then
                        .strScanCode = "X".PadLeft(20, "X")
                    Else
                        .strScanCode = objItemDetails.ScannedCode.PadLeft(20, "0")
                    End If
                    .strScanDate = objItemDetails.ScanDate
                    .strScanTime = objItemDetails.ScanTime
                    .cScanLevel = objItemDetails.ScanLevel
                    .cScanType = objItemDetails.ScanType
                    If objItemDetails.DespatchDate = Nothing Then
                        .strDespatchDate = "XXXXXX"
                    Else
                        .strDespatchDate = objItemDetails.DespatchDate

                    End If
                    '  .strDespatchDate = objItemDetails.DespatchDate
                    .strDriverBadge = "XXXXXXXX"
                    .strBarcode = objItemDetails.ProductCode.PadLeft(13, "0")
                    .strQuantity = objItemDetails.ItemQty.PadLeft(6, "0")

                    If DelType = AppContainer.DeliveryType.Directs AndAlso AppContainer.FunctionType.BookIn Then
                        .strItemStatus = "XXXXX"
                    Else
                        If iCount = 0 Then
                            'setting the first GIF for sending Item status as S
                            .strItemStatus = "S".PadRight(5, " ")
                        Else
                            'setting all other GIFs for sending Item status as X
                            .strItemStatus = "XXXXX"
                        End If
                        iCount += 1

                    End If
                    .cBatchRescan = "X"
                    .cGITNote = " "
                    .Sequence = "XX"
                End With
                objNRFDataManager.CreateGIF(objGIFMessage)
            Next

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource SendItemDetails: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function
    ''' <summary>
    ''' Function writes the GIA when a Audit Carton session is initiated
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function SendAuditSession() As Boolean
        Dim objGIAMessage As New RFDataStructure.GIARecord
        Try
            With objGIAMessage
                .strDeliveryType = "2"
                .strFunction = "2"
                .strOperatorID = objAppContainer.strCurrentUserID
                .strRequestType = "S"
                .strPeriod = "X"
                .iPointer = 0
            End With
            If objNRFDataManager.CreateGIA(objGIAMessage) Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource SendAuditSession: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
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
    Public Overrides Function ValidateCartonScanned(ByVal strASNNumber As String, ByRef objCartonInfo As GIValueHolder.CartonInfo, ByVal tyDelType As AppContainer.DeliveryType, ByVal tyFunction As AppContainer.FunctionType) As Boolean
        Dim strSQLCmd As String = QueryMacro.GET_CARTON_SCAN_DATA
        strSQLCmd = String.Format(strSQLCmd, strASNNumber.Substring(0, 6), strASNNumber.Substring(6, 8))
        Dim strStatus As String
        Dim sqlResultSet As SqlCeDataReader
        Try
            'execute the query
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSQLCmd)
            'process datareader and retreive the details in it.
            If sqlResultSet.Read Then
                objCartonInfo = New GIValueHolder.CartonInfo
                With objCartonInfo
                    .ASNNumber = strASNNumber
                    .CartonNumber = sqlResultSet("Carton_Number").ToString()
                    .SupplierName = sqlResultSet("Supplier_Ref").ToString()

                    strStatus = sqlResultSet("Status").ToString()
                    If strStatus = UNBOOKED Then
                        .Status = UNBOOKED
                    Else
                        .Status = BOOKED
                    End If
                    .SupplierNo = sqlResultSet("Supplier_Ref").ToString()
                    .TotalItemsInCarton = sqlResultSet("Total_Item_In_Carton").ToString()
                    .ExpDeliveryDate = sqlResultSet("Exp_Delivery_Date").ToString()
                End With
            Else
                'If any error occured in reading the data adapter then return false.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource ValidateCartonScanned: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function
    ''' <summary>
    ''' The Function writes the GIF for carton scanned into Export data file
    ''' </summary>
    ''' <param name="arrCartonDetails"></param>
    ''' <param name="DelType"></param>
    ''' <param name="FuncType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function SendCartonDetails(ByRef arrCartonDetails As ArrayList, _
                                                ByVal DelType As AppContainer.DeliveryType, _
                                                ByVal FuncType As AppContainer.FunctionType) As Boolean
        Dim objGIFMessage As RFDataStructure.GIFRecord
        Dim iCount As Integer = 0
        Dim iItemCount As Integer
        Try
            For Each objBookInDetails As ScanDetails In arrCartonDetails
                objGIFMessage = New RFDataStructure.GIFRecord
                With objGIFMessage
                    If DelType = AppContainer.DeliveryType.Directs Then
                        .strDeliveryType = "2"
                    ElseIf DelType = AppContainer.DeliveryType.ASN Then
                        .strDeliveryType = "3"
                    End If
                    If FuncType = AppContainer.FunctionType.BookIn Then
                        .strFunction = "1"
                    ElseIf FuncType = AppContainer.FunctionType.Audit Then
                        .strFunction = "2"
                    ElseIf FuncType = AppContainer.FunctionType.View Then
                        .strFunction = "3"
                    End If
                    .strTransactionID = "GIF"
                    .strOperatorID = objAppContainer.strCurrentUserID
                    If objBookInDetails.ScannedCode = Nothing Then
                        .strScanCode = "X".PadLeft(20, "X")
                    Else
                        .strScanCode = objBookInDetails.ScannedCode.PadLeft(20, "0")
                    End If

                    .strScanDate = objBookInDetails.ScanDate
                    .strScanTime = objBookInDetails.ScanTime

                    .cScanLevel = objBookInDetails.ScanLevel
                    .cScanType = objBookInDetails.ScanType


                    If objBookInDetails.DespatchDate <> Nothing Then
                        .strDespatchDate = objBookInDetails.DespatchDate
                    Else
                        .strDespatchDate = "XXXXXX"
                    End If

                    .strDriverBadge = "XXXXXXXX"
                    If objBookInDetails.ProductCode = Nothing Then
                        .strBarcode = "X".PadLeft(13, "X")
                    Else
                        .strBarcode = objBookInDetails.ProductCode.PadLeft(13, "0")

                    End If
                    If objBookInDetails.ItemQty = Nothing Then
                        .strQuantity = "XXXXXX"
                    Else
                        .strQuantity = objBookInDetails.ItemQty.PadLeft(6, "0")
                    End If
                    If iCount = 0 AndAlso objBookInDetails.ScanLevel = "I" Then
                        .strItemStatus = "S    "
                    Else
                        iItemCount = CType(objBookInDetails.ItemStatus, Integer)
                        If iItemCount > 0 Then
                            .strItemStatus = (iItemCount + 2).ToString().PadLeft(5, "0")
                            '-1 used as the value will get incremented below
                            iCount = -1
                        Else
                            .strItemStatus = "XXXXX"

                        End If
                    End If



                    .cBatchRescan = "X"
                    .cGITNote = " "
                    .Sequence = "XX"
                End With
                objNRFDataManager.CreateGIF(objGIFMessage)
                If objBookInDetails.ScanLevel = "I" Then
                    iCount += 1
                Else
                    iCount = 0
                End If


            Next

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource SendCartonDetails: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function
    ''' <summary>
    ''' Retrieves the list of ASN Suppliers for View Carton
    ''' </summary>
    ''' <param name="arrSupplierList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetSupplierListForView(ByRef arrSupplierList As ArrayList) As Boolean
        Dim strSqlCmdASN As String = QueryMacro.GET_SUPPLIER_LIST_VIEW

        Dim dsListASN As DataSet = Nothing

        Dim bTemp As Boolean = False
        ' Dim sqlResultSet As SqlCeDataReader
        Dim objSupplierList As SupplierList
        Dim strSupplierType As String
        Dim strStaticSupplier As String
        Dim iQuantity As Integer
        Try
            dsListASN = New DataSet()
            'execute the query
            dsListASN = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmdASN)


            'process dataset and retreive the details in it.

            If dsListASN.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsListASN.Tables(0).Rows.Count - 1
                    strStaticSupplier = dsListASN.Tables(0).Rows(iCount)("Supplier_Static_Flag").ToString()
                    iQuantity = dsListASN.Tables(0).Rows(iCount)("Cartons_In_ASN")
                    If (strStaticSupplier = "S" AndAlso iQuantity = 0) Or iQuantity > 0 Then
                        objSupplierList = New SupplierList
                        With objSupplierList
                            .SupplierName = dsListASN.Tables(0).Rows(iCount)("Supplier_Name").ToString()
                            .SupplierNo = dsListASN.Tables(0).Rows(iCount)("Supplier_ID").ToString()
                            strSupplierType = dsListASN.Tables(0).Rows(iCount)("Supplier_ASN_Flag").ToString()
                            If strSupplierType = ASNSUPPLIER Then
                                .SupplierType = ASNSUPPLIER
                            End If
                            .SupplierQty = iQuantity.ToString()

                            If strStaticSupplier = "S" Then
                                'if supplier is a static supplier
                                .StaticSupplier = "S"
                            Else
                                .StaticSupplier = "N"
                            End If
                        End With
                        arrSupplierList.Add(objSupplierList)
                    End If


                Next

            Else
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetSupplierListForView: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        End Try
        'if successfully updated the details
        Return True
    End Function
    ''' <summary>
    ''' The function retrieves the list of details of Cartons for a supplier selected
    ''' </summary>
    ''' <param name="strSupplierNo"></param>
    ''' <param name="arrSupplierDetails"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetSupplierDetails(ByVal strSupplierNo As String, ByRef arrSupplierDetails As ArrayList) As Boolean
        Dim strSQLCmd As String = QueryMacro.GET_SUPPLIER_DETAILS

        Dim objSupplierDetails As SupplierDetails
        Dim strDate = Format(DateTime.Now, "yyyyMMdd")
        strSQLCmd = String.Format(strSQLCmd, strSupplierNo)
        Dim DateToday As Date = DateTime.Now.Date
        Dim strStatus As String
        Dim strCartonsInASN As String
        Dim strCartonNumber As String
        Dim strExptDate As Date
        Dim sqlResultSet As SqlCeDataReader
        Try
            'execute the query
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSQLCmd)
            'process datareader and retreive the details in it.
            While sqlResultSet.Read
                objSupplierDetails = New SupplierDetails
                strExptDate = New DateTime(Convert.ToInt32(strDate.Substring(0, 4)), strDate.Substring(4, 2), strDate.Substring(6, 2), 0, 0, 0) 'objAppContainer.objHelper.ConvertToDate(sqlResultSet("Exp_Delivery_Date").ToString())
                If ((strExptDate = DateToday) Or (strExptDate > DateToday) _
                      Or (strExptDate.AddDays(1) = DateToday) Or (strExptDate.AddDays(2) = DateToday) _
                      Or (strExptDate.AddDays(3) = DateToday)) Then
                    With objSupplierDetails

                        strCartonNumber = sqlResultSet("Carton_Number").ToString()
                        strCartonsInASN = sqlResultSet("Cartons_In_ASN").ToString()
                        .CartonNumber = strCartonNumber
                        .ASNNumber = strSupplierNo + strCartonNumber + strCartonsInASN.PadLeft(4, "0")
                        strStatus = sqlResultSet("Status").ToString()
                        If strStatus = UNBOOKED Then
                            .Status = NO
                        ElseIf strStatus = AUDITED Then
                            .Status = Macros.AUDITED
                        Else
                            .Status = YES
                        End If
                        .ExptDate = sqlResultSet("Exp_Delivery_Date").ToString()
                        .TotalItemsInCarton = sqlResultSet("Total_Item_In_Carton").ToString()
                    End With
                    arrSupplierDetails.Add(objSupplierDetails)
                End If
            End While
            '   'If any error occured in reading the data adapter then return false.
            '  Return False

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetSupplierDetails: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'Clear the memory occupied by the variable.
            sqlResultSet = Nothing
        End Try
        'if successfully updated the details
        Return True
    End Function
    ''' <summary>
    ''' To retrieve the Item list for a selected carton
    ''' </summary>
    ''' <param name="strCartonNumber"></param>
    ''' <param name="arrCartonDetails"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetCartonDetails(ByVal strCartonNumber As String, _
                                               ByRef arrItemDetails As ArrayList, _
                                               ByVal tyDelType As AppContainer.DeliveryType, _
                                               ByVal tyFunc As AppContainer.FunctionType, _
                                               ByVal bBkd As Boolean, _
                                               Optional ByRef objCartonInfo As GIValueHolder.CartonInfo = Nothing) As Boolean
        Dim strSQLCmd As String = QueryMacro.GET_ITEM_LIST_VIEW
        Dim strSQLCmdBkd As String = QueryMacro.GET_ITEM_LIST_VIEW_BOOKED




        If objCartonInfo Is Nothing Then
            objCartonInfo = New GIValueHolder.CartonInfo
        End If
        Dim objItemDetails As ItemDetails

        Dim sqlResultSet As SqlCeDataReader
        Try

            If ValidateCartonScanned(strCartonNumber, objCartonInfo, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.View) Then
                'getting carton number from the asn carton number selected
                strCartonNumber = strCartonNumber.Substring(6, 8)
                strSQLCmd = String.Format(strSQLCmd, strCartonNumber)
                strSQLCmdBkd = String.Format(strSQLCmdBkd, strCartonNumber)
                'execute the query
                If Not objCartonInfo.Status = Macros.UNBOOKED Then
                    'If bBkd Then
                    sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSQLCmdBkd)
                Else
                    sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSQLCmd)
                End If

                'process datareader and retreive the details in it.
                While sqlResultSet.Read
                    objItemDetails = New ItemDetails
                    With objItemDetails

                        If sqlResultSet("Second_Barcode").ToString().Trim("0") = "" Then
                            .ProductCode = objAppContainer.objHelper.GeneratePCwithCDV(sqlResultSet("First_Barcode").ToString())
                        Else
                            .ProductCode = objAppContainer.objHelper.GeneratePCwithCDV(sqlResultSet("Second_Barcode").ToString())
                        End If

                        .ItemCode = sqlResultSet("Boots_Code").ToString()
                        .ItemDesc = sqlResultSet("Item_Desc").ToString()
                        .Status = sqlResultSet("Item_Status").ToString()
                        If Not objCartonInfo.Status = Macros.UNBOOKED Then
                            .ItemQty = sqlResultSet("Booked_Qty").ToString()
                        Else
                            .ItemQty = sqlResultSet("Despatched_Qty").ToString()
                        End If



                    End With
                    arrItemDetails.Add(objItemDetails)
                End While
                '   'If any error occured in reading the data adapter then return false.
                '  Return False
            Else
                Return False
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetCartonDetails: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'Clear the memory occupied by the variable.
            sqlResultSet = Nothing
        End Try
        'if successfully updated the details
        Return True

    End Function
    ''' <summary>
    ''' The function gets the supplier details for a Supplier Number 
    ''' </summary>
    ''' <param name="strSupplierNumber"></param>
    ''' <param name="objSupplierData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetSupplierData(ByVal strSupplierNumber As String, _
                                              ByRef objSupplierData As SupplierList, _
                                              ByRef tyDelType As AppContainer.DeliveryType, _
                                              ByVal tyFunction As AppContainer.FunctionType) As Boolean
        Dim strSQLCmd As String = QueryMacro.GET_SUPPLIER_DATA
        strSQLCmd = String.Format(strSQLCmd, strSupplierNumber)
        Dim strSupplierType As String
        Dim strStaticSupplier As String
        Dim objSupplierDetails As SupplierList
        Dim sqlResultSet As SqlCeDataReader
        Try
            Try
                'execute the query
                sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSQLCmd)
                'process datareader and retreive the details in it.
                If sqlResultSet.Read Then
                    objSupplierData = New SupplierList
                    With objSupplierData
                        .SupplierName = sqlResultSet("Supplier_Name").ToString()
                        .SupplierNo = sqlResultSet("Supplier_ID").ToString()
                        strSupplierType = sqlResultSet("Supplier_ASN_Flag").ToString()
                        If strSupplierType = ASNSUPPLIER Then
                            .SupplierType = ASNSUPPLIER
                        Else
                            .SupplierType = DIRECTSUPPLIER
                        End If
                        strStaticSupplier = sqlResultSet("Supplier_Static_Flag").ToString()
                        If strStaticSupplier = STATICSUPPLIER Then
                            'if supplier is a static supplier
                            .StaticSupplier = "S"
                        Else
                            .StaticSupplier = NO
                        End If
                        '  .SupplierQty = sqlResultSet("").ToString()

                    End With
                Else
                    '   'If any error occured in reading the data adapter or no data then return false.
                    Return False
                End If



            Catch ex As Exception
                'Add the exception to the application log.
                AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetSupplierData: " _
                                                                    + ex.StackTrace, Logger.LogLevel.RELEASE)
                'return false after logging the exception to the log.
                Return False
            End Try
            'if successfully updated the details
            Return True

        Catch ex As Exception

        End Try
    End Function


#Region "Enumerated Values"
    Public Enum GITNote
        Yes
        No
    End Enum
    Public Enum IsAbort
        Yes
        No
    End Enum
    Public Enum UODContainerType
        Dolly
        Crate
        RollyCage
        Pallet
        InterStoreTransfer
    End Enum
#End Region



    ''' <summary>
    ''' Gets the user details using User ID.
    ''' </summary>
    ''' <param name="strUserID">User ID</param>
    ''' <param name="objUserInfo">Object to be updated.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or
    ''' there is no such User Id present in the database.
    ''' </returns>
    ''' <remarks></remarks>
    Public Overrides Function GetUserDetails(ByVal strUserID As String, ByRef objUserInfo As UserInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_USER_DETAILS
        strSqlCmd = String.Format(strSqlCmd, strUserID)
        Dim sqlResultSet As SqlCeDataReader
        Try
            'execute the command to get the user details.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objUserInfo
                    .UserID = sqlResultSet("User_ID").ToString()
                    .Password = sqlResultSet("Password").ToString()
                    .SupervisorFlag = sqlResultSet("Supervisor_Flag").ToString()
                End With
            Else
                'If error occured while reading the data reader.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetUserDetails: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'Clear the memory occupied by the variable.
            sqlResultSet = Nothing
        End Try
        'If successfully updated the details.
        Return True
    End Function
    ''' <summary>
    ''' The function retrieves the list of cartons for a supplier
    ''' </summary>
    ''' <param name="strSupplierNo"></param>
    ''' <param name="arrCartonList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetCartonList(ByVal strSupplierNo As String, ByRef arrCartonList As ArrayList) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_CARTON_LIST
        strSqlCmd = String.Format(strSqlCmd, strSupplierNo)
        Dim strAuditStatus As String
        Dim dsList As DataSet = Nothing
        Dim objCartonList As CartonList
        Try
            'execute the query

            dsList = New DataSet()
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'process dataset and retreive the details in it.

            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objCartonList = New CartonList
                    With objCartonList
                        .CartonId = dsList.Tables(0).Rows(iCount)("Carton_Number").ToString()
                        .ExptdDate = dsList.Tables(0).Rows(iCount)("Exp_Delivery_Date").ToString()
                        .BkdInStatus = dsList.Tables(0).Rows(iCount)("Status").ToString()
                        .NoOfItems = dsList.Tables(0).Rows(iCount)("Total_Item_In_Carton").ToString()
                    End With
                    arrCartonList.Add(objCartonList)
                Next
            Else
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetCartonList: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'Clear the memory occupied by the variable.
            dsList.Dispose()
        End Try
        'If successfully updated the details.
        Return True

    End Function
    ''' <summary>
    ''' The function gets the list of items for a Carton
    ''' </summary>
    ''' <param name="strCartonNumber"></param>
    ''' <param name="arrItemList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetItemList(ByVal strCartonNumber As String, ByVal arrItemList As ArrayList) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_ITEM_LIST
        strSqlCmd = String.Format(strSqlCmd, strCartonNumber)
        Dim objItemList As ItemList
        Dim dsList As DataSet = Nothing

        Try
            'execute the query

            dsList = New DataSet()
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'process dataset and retreive the details in it.

            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objItemList = New ItemList
                    With objItemList
                        .ProductCode = dsList.Tables(0).Rows(iCount)("Second_Barcode").ToString()
                        .ItemCode = dsList.Tables(0).Rows(iCount)("Boots_Code").ToString()
                        .ItemDesc = dsList.Tables(0).Rows(iCount)("Item_Desc").ToString()
                        .ItemQty = dsList.Tables(0).Rows(iCount)("Despatched_Qty").ToString()
                    End With
                    arrItemList.Add(objItemList)
                Next
            Else
                Return False
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetItemList: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'Clear the memory occupied by the variable.
            dsList.Dispose()
        End Try
        'If successfully updated the details.
        Return True
    End Function
    ''' <summary>
    ''' to get UOD Details on scanning UOD number in audit UOD
    ''' </summary>
    ''' <param name="strUODNumber"></param>
    ''' <param name="objUODInfo"></param>
    ''' <param name="arrItemDetails"TT></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function GetUODDetails(ByVal strUODNumber As String, ByRef objUODInfo As UODInfo, ByRef arrItemDetails As ArrayList) As Boolean
        Try
            If ValidateUODScanned(strUODNumber, objUODInfo, AppContainer.FunctionType.Audit) Then
                GetItemListForView(strUODNumber, arrItemDetails)
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetUODDetails: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try

    End Function

    Public Overrides Function GetChildUODDetails(ByVal strUODNumber As String, _
                                                    ByRef objUODInfo As UODInfo, _
                                                    ByRef arrChildUODList As ArrayList) As Boolean
        Dim strSQLCmd As String = QueryMacro.GET_UOD_CHILD_LIST
        strSQLCmd = String.Format(strSQLCmd, strUODNumber)
        Dim objUODList As UODList
        Dim strUODStatus As String
        Dim dsList As DataSet = Nothing
        Try
            dsList = New DataSet()
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSQLCmd)
            'process dataset and retreive the details in it.

            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objUODList = New UODList
                    With objUODList
                        .UODID = dsList.Tables(0).Rows(iCount)("UOD_License_Number").ToString()
                        .UODType = dsList.Tables(0).Rows(iCount)("Outer_type").ToString()
                        strUODStatus = dsList.Tables(0).Rows(iCount)("Booked_In").ToString()
                        Select Case strUODStatus
                            Case "Y"
                                .BookedIn = Macros.Y
                            Case "N"
                                .BookedIn = Macros.N
                        End Select
                        If dsList.Tables(0).Rows(iCount)("Audited") = "Y" Then
                            .BookedIn = Macros.AUDITED
                        End If

                    End With
                    arrChildUODList.Add(objUODList)
                Next
            Else
                Return False
            End If
            objUODInfo.NoOfChildren = arrChildUODList.Count.ToString()
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at DataSource GetChildUODDetails: " _
                                                                + ex.StackTrace, Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'Clear the memory occupied by the variable.
            dsList.Dispose()
        End Try
        Return True
    End Function
    ''' <summary>
    ''' To get the Boots Code corresponding to a Product Code.
    ''' </summary>
    ''' <param name="strProductCode">Product code wihtout check digit</param>
    ''' <returns>
    ''' Boots Code - If a Boots code is available for the Product code passed.
    ''' 0 - if there is no Boots code available for the Product passed.
    ''' </returns>
    ''' <remarks></remarks>
    Public Overrides Function GetBootsCode(ByVal strProductCode As String) As String
        Dim strSqlCmd As String = QueryMacro.GET_BOOTS_CODE
        'Substitue Product code in the query string.
        'Minu Removed padleft of strproductcode
        'strProductCode = strProductCode.PadLeft(13, "0")
        Dim strPC As String = strProductCode.Remove(strProductCode.Length - 1, 1)
        strSqlCmd = String.Format(strSqlCmd, strPC)
        Dim sqlResultSet As SqlCeDataReader
        Try
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'Return the value to the calling fucntion.
            If sqlResultSet.Read() Then
                Return sqlResultSet("Boots_Code").ToString()
            Else
                'return 0 if the sql reader does not have any data.
                'Return 0
                Return strProductCode 'objAppContainer.objHelper.GenerateBCwithCDV(strProductCode.Substring(6, 6))
            End If
        Catch ex As Exception
            'return false in case of any exception.
            Return 0
        Finally
            'Dispose the data reader.
            sqlResultSet = Nothing
        End Try
    End Function

End Class
#End If