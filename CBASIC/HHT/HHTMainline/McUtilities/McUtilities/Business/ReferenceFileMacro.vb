''' <summary>
''' Define Macros that are used by MCDownloader application.
''' </summary>
''' <remarks></remarks>
Public Class ReferenceFileMacro
    Public Const ConnectionString = "\Program Files\Parsing\BTCDB.sdf"
    Public Const Delimiter = ","
    Public Const DeleteRetry = 3
    Public Const ReferenceTables = "BootsCodeView,BarCodeView,DealList," _
                                    & "ProductGroup,SuppliersList,UserList," _
                                    & "RecallList,RecallListItems,LivePOG," _
                                    & "POGCategory,ModuleList,ModuleListItems," _
                                    & "ShelfDesc"
    Public Const BOOTCODE = "BOOTCODE.CSV"
    Public Const BARCODE = "BARCODE.CSV"
    Public Const CATEGORY = "CATEGORY.CSV"
    Public Const DEAL = "DEAL.CSV"
    Public Const LIVEPOG = "LIVEPOG.CSV"
    Public Const MODULE_LIST = "MODULE.CSV"
    Public Const PGROUP = "PGROUP.CSV"
    Public Const RECALL = "RECALL.CSV"
    Public Const SUPPLIER = "SUPPLIER.CSV"
    Public Const USER = "USER.CSV"
    Public Const SHELFDES = "SHELFDES.CSV"
End Class
