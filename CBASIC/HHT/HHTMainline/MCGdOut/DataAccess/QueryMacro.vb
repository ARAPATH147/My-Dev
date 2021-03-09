
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class QueryMacro
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    ''' 'Recall CR,Special Instructions
    Public Const GET_RECALL_LIST = _
    "SELECT Recall_Active_Date , Recall_Message,Recall_Type, Recall_Number, Recall_Desc, Recall_Qty, Status, UOD_Label_Type, Batch_Nos, Min_Return_Qty " _
    & "FROM RecallList "
    '    & "FROM RecallList " _
    ' & "WHERE Recall_Type ='{0}'"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_COMPANYHORECALL_LIST = _
    "SELECT     Recall_Active_Date, Recall_Type, Recall_Number, Recall_Desc, Recall_Qty, Status " _
    & "FROM         RecallList " _
    & "WHERE     (Recall_Type NOT IN ('C', 'I'))"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    ''' 'Tailoring
    Public Const GET_RECALL_LIST_ITEMS = _
    "SELECT BootsCodeView.Boots_Code, " _
    & "BootsCodeView.First_Barcode, BootsCodeView.Second_Barcode, " _
    & "BootsCodeView.Item_Status, BootsCodeView.Item_Desc, BootsCodeView.SOD_TSF, " _
    & "BootsCodeView.Last_Stock_Movement_Date, " _
    & "BootsCodeView.Current_Price, BootsCodeView.Live_POG1, RecallListItems.Recall_Status " _
    & "FROM BootsCodeView INNER JOIN " _
    & "RecallListItems ON BootsCodeView.Boots_Code = RecallListItems.Boots_Code " _
    & "INNER JOIN " _
    & "BarCodeView ON BarCodeView.BarCode = BootsCodeView.First_Barcode " _
    & "WHERE (RecallListItems.Recall_Number = '{0}')"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_MISSING_RECALL_ITEMS = _
    "SELECT Boots_Code, Recall_Status FROM RecallListItems " _
    & "WHERE (Recall_Number = '{0}') AND (Boots_Code NOT IN " _
    & "(SELECT BootsCodeView.Boots_Code " _
    & "FROM BootsCodeView INNER JOIN " _
    & "RecallListItems AS RecallListItems_1 ON BootsCodeView.Boots_Code = RecallListItems_1.Boots_Code " _
    & "WHERE (RecallListItems_1.Recall_Number = '{1}')))"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_SUPPLIER_ITEMS = _
    "SELECT SupplierBC.Supplier_ID, SuppliersList.Supplier_Name" _
    & " FROM SupplierBC INNER JOIN" _
    & " SuppliersList ON SupplierBC.Supplier_ID = SuppliersList.Supplier_ID " _
    & " AND SupplierBC.Supplier_BC = '{0}'"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_DETAILS_USING_BC = _
    "SELECT BootsCodeView.First_Barcode, BootsCodeView.Second_Barcode, " _
    & "BootsCodeView.Item_Status, BootsCodeView.SEL_Desc, BootsCodeView.SOD_TSF, " _
    & "BootsCodeView.Current_Price, BootsCodeView.Boots_Code, BootsCodeView.BC, " _
    & "BootsCodeView.Item_Desc, BootsCodeView.Supply_Route " _
    & "FROM BootsCodeView INNER JOIN " _
    & "BarCodeView ON BootsCodeView.Boots_Code = BarCodeView.Boots_Code " _
    & "WHERE (BootsCodeView.Boots_Code = '{0}')"
    ''' <summary>
    ''' Query to get item details of an item using Boots Code for 
    ''' Item Info module.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_ITEM_DETAILS_ALL_USING_BC = _
    "SELECT Boots_Code, First_Barcode, Second_Barcode " _
    & "FROM BootsCodeView " _
    & "WHERE (Boots_Code = '{0}')"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    ''' 'BootsCodeView.Second_Barcode included for getting second barCode
    ''' Recalls CR
    ''' create recalls
    Public Const GET_DETAILS_USING_PC = _
    "SELECT BootsCodeView.First_Barcode,BootsCodeView.Second_Barcode,BarCodeView.BarCode, BarCodeView.Boots_Code, BootsCodeView.Current_Price, " _
    & "BootsCodeView.Item_Status, BootsCodeView.BC, " _
    & "BootsCodeView.SEL_Desc, BootsCodeView.SOD_TSF, " _
    & "BootsCodeView.Item_Desc, BootsCodeView.Supply_Route,BootsCodeView.Item_Status0,BootsCodeView.Item_Status8,BootsCodeView.Last_Stock_Movement_Date, " _
    & "BootsCodeView.Live_POG1 " _
    & "FROM BarCodeView INNER JOIN " _
    & "BootsCodeView ON BarCodeView.Boots_Code = BootsCodeView.Boots_Code " _
    & "WHERE (BarCodeView.BarCode = '{0}')"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Const VALIDATE_PRODUCT_USING_BC = _
    "SELECT * FROM BootsCodeView " _
    & "WHERE (Boots_Code = '{0}')"
    '''Tailoring
    ''' <summary>
    ''' To check whether the item is present in Modulelist Items
    ''' </summary>
    ''' <remarks></remarks>
    Public Const VALIDATE_MODULEITEMS = _
    "SELECT * FROM ModuleListItems " _
    & "WHERE (Boots_Code = '{0}')"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Const VALIDATE_PRODUCT_USING_PC = _
    "SELECT * FROM BootsCodeView INNER JOIN " _
    & "BarCodeView ON BootsCodeView.Boots_Code = BarCodeView.Boots_Code " _
    & "WHERE (BarCodeView.BarCode = '{0}')"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_USER_DETAILS = _
    "SELECT User_ID, Password, Supervisor_Flag, User_Name,Stock_Adjustment_Flag " _
    & "FROM UserList " _
    & "WHERE (User_ID = '{0}')"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_BOOTCODE_MODULELISTITEMS = _
    "SELECT Module_ID " _
    & "FROM ModuleListItems " _
    & "WHERE (Boots_Code = '{0}')"

    Public Const GET_BOOTCODE_FROM_MODULELISTITEMS = _
   "SELECT DISTINCT Boots_Code " _
   & "FROM ModuleListItems "
    '& "WHERE (Boots_Code IN " _
    '& " (SELECT Boots_Code " _
    '& "FROM RecallListItems " _
    '& "WHERE (Recall_Number = '{0}')))"

End Class

