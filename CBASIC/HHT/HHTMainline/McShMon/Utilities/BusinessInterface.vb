''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Interface ISMInterface
    Function StartSession() As Boolean
    Sub DisplayHomeScreen(ByVal o As Object, ByVal e As EventArgs)
End Interface
''' <summary>
''' This interface handles Scan data 
''' </summary>
''' <remarks></remarks>
Public Interface IScanInterface
    Inherits ISMInterface
    Sub HandleScanData(ByVal strBarcode As String, ByVal Type As BCType)
End Interface
''' <summary>
''' This interface defines Write Export data
''' </summary>
''' <remarks></remarks>
Public Interface ISendData
    Function WriteExportData() As Boolean
End Interface

'Store Sales interface
Public Interface ISSInterface
    Inherits ISMInterface
    Sub DisplaySSScreen(ByVal ScreenName As SSSCREENS)
    Function GetStoreSalesInfo(ByRef strToday As String, ByRef strWeeks As String) As Boolean
    'Function GetInstance() As SSSessionManager
End Interface
Public Enum SSSCREENS
    Home
End Enum

'Item Sales Interface
Public Interface IISInterface
    Inherits IScanInterface
    Sub DisplayISScreen(ByVal ScreenName As ISSCREENS)
    Sub DisplayItemSalesDetailsScreen(ByVal o As Object, ByVal e As EventArgs)
    'Function GetItemSalesInfo(ByVal strBarcode As String, ByRef arrSalesInfo As ArrayList) As Boolean
    'Function GetInstance() As ISSessionManager
End Interface

Public Enum ISSCREENS
    Home
    ItemSalesDetails
End Enum

'Reports Session
Public Interface IReports
    Inherits ISMInterface
    'Function GetInstance() As ReportsSessionManager
    Function GetReportList(ByRef objReports As ArrayList) As Boolean
    Function GetReportHeaders(ByVal strReportID As String, ByRef arrReportHeaders As ArrayList) As Boolean
    Function GetReportDetails(ByVal strReportID As String, ByVal strInitialSeqNo As String, ByRef arrReportDetails As ArrayList) As Boolean
    Sub DisplayReportScreen(ByVal ScreenName As REPORTSCREENS)
    Sub DisplayNoReports(ByVal o As Object, ByVal e As EventArgs)
    Sub DisplayDetailsScreen(ByVal o As Object, ByVal e As EventArgs)
End Interface
Public Enum REPORTSCREENS
    NoReports
    Home
    DetailsScreen
End Enum
