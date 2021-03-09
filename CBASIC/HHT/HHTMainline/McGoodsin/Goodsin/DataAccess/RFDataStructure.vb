'''******************************************************************************
'''* Modification Log 
'''******************************************************************************* 
'''* No:      Author            Date                 Description 
'''* 1.1     Christopher Kitto  28/04/2015  Modified as part of DALLAS project. Added
'''                  (CK)                  new structures DARMessage & DARRecord.
'''********************************************************************************


Public Class RFDataStructure
    Public Enum ENQType
        Item
        Parent
    End Enum

    Public Enum ENQFunction
        PriceCheck
        FastFill
        Other
    End Enum
    Public Enum DeliveryType
        SSCReceiving = 1
        DirectsReceiving = 2
        ASNs = 3
        None
    End Enum
    Public Enum GFunction
        BookIn = 1
        Audit = 2
        View = 3
        None
    End Enum
    Public Enum GIARequestType
        Configuration
        Summary
        List
    End Enum
    Public Enum GIQRequestType
        BookingIn
        FullSummary
        SupplierQuery
    End Enum
    Public Enum ContentType
        Container
        Item
    End Enum
    Public Enum GPeriod
        Today
        Future
        None
    End Enum
    Public Enum SupplierType
        NoSupplier
        ASN
        Directs
    End Enum
    
    Public Enum ScanLevel
        NotUsed
        DeliveryScan
        ItemScan
    End Enum

    ' V1.1 - CK
    ' Added new structure DARMessage

    ''' <summary>
    ''' Structure to create DAR message
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure DARMessage
        Public cDallasBarcode As String
        Public cScanDate As String
        Public cScanStatus As Char
    End Structure

    Public Structure GIFMessage
        Public eType As AppContainer.DeliveryType
        Public eFunc As AppContainer.FunctionType
        Public strScanCode As String
        Public strDespatchDate As String
        Public eSType As AppContainer.ScanType
        Public eSLevel As ScanLevel
        Public strScanDate As String
        Public strScanTime As String
        Public strDriverBadge As String
        Public cGITNote As Char
        Public cBatRescan As Char
        Public strBarcode As String
        Public strQuantity As String
        Public strItemStatus As String
        Public strSequence As String
    End Structure
    Public Structure GIQMessage
        Public eType As AppContainer.DeliveryType
        Public eFunc As AppContainer.FunctionType
        Public strSelectedCode As String
        Public strSequence As String
        Public eRequestType As GIQRequestType
        Public eContType As ContentType
        Public eSupType As SupplierType
        Public iPointer As Integer
    End Structure
    Public Structure GIAMessage
        Public eType As DeliveryType
        Public eFunc As GFunction
        Public eRequestType As GIARequestType
        Public ePeriod As GPeriod
        Public iPointer As Integer
    End Structure
    Public Structure GIXMessage
        Public eDeliveryType As AppContainer.DeliveryType
        Public eFunction As AppContainer.FunctionType
        Public eIsAbort As AppContainer.IsAbort

    End Structure
    Public Structure EQRMEssage
        Public strDescription As String
        Public strParent As String
        Public strBarcode As String
        Public strBootsCode As String
        Public strStatus As String
        Public strStockFigure As String
        Public strPrimaryCurrency As String
        Public strEuroPrice As String
        Public strPrice As String
        Public strAdvantage As String
        Public strstrPriceCheckTarget As Integer
        Public strPriceCheckDone As Integer
        Public strCheckValid As String
        Public strRejectMessage As String
        Public strActiveDeal As String
        Public strActiveItemDeal As String
        Public strOSSRItem As String
        Public strLongDescription As String
        Public strMultiLocationCore As String
        Public strMultiLocationSalesPlan As String
        Public cRecallItem As Char
        Public cMarkDown As Char
        Public cRecallType As Char
        Public strPGGroup As String
    End Structure

    Public Structure GIBCMessage
        Public strTransactionID As String
        Public strOperatorID As String
        Public cResponseType As Char
        Public cDirectsActive As Char
        Public cUODActive As Char
        Public cASNActive As Char
        Public cONightDelivery As Char
        Public cONightScan As Char
        Public strBatchSize As String

    End Structure
    Public Structure GIBSMessage
        Public TransactionID As String
        Public OperatorID As String
        Public ResponseType As String
        Public Count As Integer
        Public Pointer As Integer
        Public arrGIBSData As ArrayList
    End Structure
    Public Structure GIBSummary
        Public strIdentifier As String
        Public strSequence As String
        Public strName As String
        Public cSupplierType As Char
        Public cContentType As Char
        Public strExpectedDate As String
        Public cBookedIn As Char
        Public strQuantity As String
    End Structure
    '''Value class for GIRMessage for GIQ Request Type B
    Public Structure GIRBMessage
        Public strSelectedCode As String
        Public cResponseType As Char
        Public strDespatchDate As String
        Public cOuterType As Char
        Public cContentType As Char
        Public cUODReason As Char
        Public cStatus As Char
        Public cBOLUOD As Char
        Public strOrderNum As String
        Public cOrderSuffix As Char
        Public cBusCentre As Char

    End Structure

    Public Structure GIRSMessage
        Public strTransactionID As String
        Public strOperatorID As String
        Public strSelectedCode As String
        Public cResponseType As Char
        Public strSupplierNum As String
        Public strSupplierName As String
        Public strSupplierType As String
    End Structure
    Public Structure GIRFMessage
        Public strSelectedCode As String
        Public cResponseType As Char
        Public strDespatchDate As String
        Public cOuterType As Char
        Public cContentType As Char
        Public cUODReason As Char
        Public cStatus As Char
        Public cBOLUOD As Char
        Public strOrderNum As String
        Public cOrderSuffix As Char
        Public cBusCentre As Char
        Public strEstDeliveryDate As String
        Public strDriverBadge As String
        Public strDriverCheckInDate As String
        Public strDriverCheckInTime As String
        Public strStoreOPID As String
        Public strBookInDate As String
        Public strBookInTime As String
        Public iCount As Integer
        Public iPointer As Integer
        Public arrGIRFData As ArrayList
    End Structure
    ''Part of GIR Message with ResponseType F which is repeated COUNT Times
    Public Structure GIRFMessageData
        Public strIdentifier As String
        Public strName As String
        Public cBookedIn As Char
        Public cContentType As Char
        Public strDescription As String
        Public strQuantity As String
        Public strSequence As String
    End Structure

    Public Structure SNRMessage
        Public strUserID As String
        Public cAuthorityFlag As Char
        Public strUserName As String
        Public strDateTime As String
        Public strPrtNum As String
        Public strPrtDesc As String
    End Structure
    Public Structure GIARecord
        Dim strTransactionID As String
        Dim strOperatorID As String
        Dim strDeliveryType As String
        Dim strFunction As String
        Dim strRequestType As String
        Dim strPeriod As String
        Dim iPointer As Integer
    End Structure
    ''' <summary>
    ''' Structure to create export data for GIX
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure GIXRecord
        Dim strTransactionID As String
        Dim strOperatorID As String
        Dim strDeliveryType As String
        Dim strFunction As String
        Dim cIsAbort As Char
    End Structure
    ' V1.1 - CK
    ' Added new structure DARRecord

    ''' <summary>
    ''' Structure to create export data for DARRecord
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure DARRecord
        Dim cDallasBarcode As String
        Dim cScanDate As string
        Dim cScanStatus As String
    End Structure
    ''' <summary>
    ''' Structure to create export data for GIF
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure GIFRecord
        'Dim strListID As String
        Dim strTransactionID As String
        Dim strOperatorID As String
        Dim strDeliveryType As String
        Dim strFunction As String
        Dim strScanCode As String
        Dim strDespatchDate As String
        Dim cScanType As Char
        Dim cScanLevel As Char
        Dim strScanDate As String
        Dim strScanTime As String
        Dim strDriverBadge As String
        Dim cGITNote As Char
        Dim cBatchRescan As Char
        Dim strBarcode As String
        Dim strQuantity As String
        Dim strItemStatus As String
        Dim Sequence As String
    End Structure

End Class
