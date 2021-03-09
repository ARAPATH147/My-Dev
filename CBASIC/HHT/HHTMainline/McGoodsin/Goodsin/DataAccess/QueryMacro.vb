'''****************************************************************************
''' <FileName>QueryMacro.vb</FileName>
''' <summary>
''' This class is the data source class for the Shelf Management application.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''****************************************************************************
''' 
'''******************************************************************************* 
'''* Modification Log 
'''******************************************************************************* 
'''* No:      Author            Date            Description 
'''* 1.1  Christopher Kitto  09/04/2015   Modified as part of DALLAS project
'''             (CK)                     Added a new constant variable 
'''                                      GET_DALLAS_DELIVERY_SUMMARY to hold the
'''                                      SQL query command for retrieving Dallas 
'''                                      Delivery Summary details.
'''       Kiran Krishnan     28/04/2015  Modified as part of Dallas project for
'''             (KK)                     including the SQL query command for 
'''                                      retrieving scanned Dallas UOD details
'''                                      from DALUODList table
'''********************************************************************************
Public Class QueryMacro
    ''' <summary>
    ''' Query to get the user details.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_USER_DETAILS = _
    "SELECT User_ID, Password, Supervisor_Flag " _
    & "FROM UserList " _
    & "WHERE (User_ID = '{0}')"
    ''' <summary>
    ''' Query to Get the details of a Supplier.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_SUPPLIER_DETAILS = _
    "SELECT Carton_Number, ASN_Number, Exp_Delivery_Date, Status, Cartons_In_ASN, Total_Item_In_Carton " _
    & "FROM ASNList " _
    & "WHERE ( Supplier_Ref = '{0}' ) "

    ''' <summary>
    ''' QUery to get Supplier list 
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_SUPPLIER_LIST_ASN = _
    "SELECT COUNT(ASNList.Carton_Number) As Cartons_In_ASN, SuppliersList.Supplier_Name, SuppliersList.Supplier_ASN_Flag, SuppliersList.Supplier_Static_Flag, " _
    & "SuppliersList.Supplier_ID " _
    & "FROM ASNList, SuppliersList " _
    & "WHERE (SuppliersList.Supplier_ASN_Flag = 'A') " _
    & "AND ASNList.Supplier_Ref = SuppliersList.Supplier_ID " _
    & "GROUP BY SuppliersList.Supplier_Name, SuppliersList.Supplier_ASN_Flag, SuppliersList.Supplier_Static_Flag, SuppliersList.Supplier_ID "

    ''' <summary>
    ''' Query to get Supplier list where status of Unbooked Asn
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_SUPPLIER_LIST_UNBOOKED_ASN = _
   "SELECT COUNT(ASNList.Carton_Number) As Cartons_In_ASN, SuppliersList.Supplier_Name, SuppliersList.Supplier_ASN_Flag, SuppliersList.Supplier_Static_Flag, " _
   & "SuppliersList.Supplier_ID " _
   & "FROM ASNList, SuppliersList " _
   & "WHERE (ASNList.Status='U') " _
   & "AND ASNList.Supplier_Ref = SuppliersList.Supplier_ID " _
   & "GROUP BY SuppliersList.Supplier_Name, SuppliersList.Supplier_ASN_Flag, SuppliersList.Supplier_Static_Flag, SuppliersList.Supplier_ID "

    ''' <summary>
    ''' Query to get the ist of suppliers for Directs
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_SUPPLIER_LIST_DIRECTS = _
    "SELECT     COUNT(DirectList.Order_No) AS Cartons_In_ASN, SuppliersList.Supplier_ID, SuppliersList.Supplier_Name, " _
    & "SuppliersList.Supplier_ASN_Flag, SuppliersList.Supplier_Static_Flag " _
    & "FROM DirectList, SuppliersList " _
    & "WHERE (SuppliersList.Supplier_ASN_Flag = '0') " _
    & "AND DirectList.Supplier = SuppliersList.Supplier_ID " _
    & "GROUP BY SuppliersList.Supplier_ID, SuppliersList.Supplier_Name, SuppliersList.Supplier_ASN_Flag, SuppliersList.Supplier_Static_Flag "

    ''' <summary>
    ''' Query to get the ist of suppliers for Unbooked Directs
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_SUPPLIER_LIST_DIRECTS_UNBOOKED = _
  "SELECT  COUNT(DirectList.Order_No) AS Cartons_In_ASN, SuppliersList.Supplier_ID, SuppliersList.Supplier_Name, " _
    & "SuppliersList.Supplier_ASN_Flag, SuppliersList.Supplier_Static_Flag " _
    & "FROM DirectList, SuppliersList " _
    & "WHERE (DirectList.Confirm_Flag='0') " _
    & "AND DirectList.Supplier = SuppliersList.Supplier_ID " _
    & "GROUP BY SuppliersList.Supplier_ID, SuppliersList.Supplier_Name,SuppliersList.Supplier_ASN_Flag, SuppliersList.Supplier_Static_Flag "


    ''' <summary>
    ''' Query to get the list of suppliers for Static Suppliers
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_SUPPLIER_LIST_STATIC = _
    "SELECT SuppliersList.Supplier_ID, SuppliersList.Supplier_Name, " _
    & "SuppliersList.Supplier_ASN_Flag, SuppliersList.Supplier_Static_Flag " _
    & "FROM SuppliersList " _
    & "WHERE (SuppliersList.Supplier_Static_Flag = 'S') "
    ''' <summary>
    ''' QUery to get Supplier list for View
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_SUPPLIER_LIST_VIEW = _
    "SELECT  COUNT(ASNList.ASN_Number) AS Cartons_In_ASN, SuppliersList.Supplier_Name, SuppliersList.Supplier_ASN_Flag, " _
    & " SuppliersList.Supplier_Static_Flag, SuppliersList.Supplier_ID " _
    & "FROM ASNList, SuppliersList " _
    & "WHERE ASNList.Supplier_Ref = SuppliersList.Supplier_ID " _
    & "GROUP BY SuppliersList.Supplier_Name, SuppliersList.Supplier_ASN_Flag, " _
    & "SuppliersList.Supplier_Static_Flag, SuppliersList.Supplier_ID "

    ''' <summary>
    ''' Query to get the details of a Supplier Number
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_SUPPLIER_DATA = _
    "SELECT Supplier_ID, Supplier_Name, Supplier_ASN_Flag, Supplier_Static_Flag " _
    & "FROM SuppliersList " _
    & "WHERE (Supplier_ID = '{0}')"
    ''' <summary>
    ''' Query to get the Delivery Summary List for Book In Delivery
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_DELIVERY_SUMMARY As String = _
    "SELECT Summary_Type, Container_Type, Container_Quantity " _
    & "FROM UODOUTSummary "

    ' v1.1 - CK
    ' Added constant GET_DALLAS_DELIVERY_SUMMARY to hold query command for Dallas delivery
    ' summary

    ''' <summary>
    ''' Query to get the DALLAS Delivery Summary List for Book In Delivery
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_DALLAS_DELIVERY_SUMMARY As String = _
    "SELECT DalUOD_Num, DalUOD_Exp_Date, DalUOD_Status " _
    & "FROM DALUODList "

    'v1.1 - KK
    'Added constant GET_DALLASUOD_SCAN_DATA to hold query to get scanned Dallas UOD details

    ''' <summary>
    ''' Query to get the scanned DALLAS UOD detail for Book In Delivery
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_DALLASUOD_SCAN_DATA As String = _
    "SELECT DalUOD_Num, DalUOD_Exp_Date, DalUOD_Status " _
    & "FROM DALUODList " _
    & "WHERE (DalUOD_Num = '{0}')"

    ''' <summary>
    ''' Query to get the child count
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_UOD_COUNT As String = _
    "SELECT COUNT(UOD_License_Number) AS UODChildCount " _
    & "FROM UODOUTDetail " _
    & "WHERE (Immediate_License_Number = '{0}') " _
    & "AND (Booked_In = 'N')"

    ''' <summary>
    ''' Query to get the Details of a Scanned UOD
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_UOD_SCAN_DATA = _
    "SELECT     UOD_License_Number, Despatch_Date, Estimated_Delivery_Date, Outer_Type, " _
    & "Booked_In, Audited, Number_Childrens, Number_Of_Items, Partial_Booked, " _
    & "Ultimate_License_Number, Immediate_License_Number, Booked_In_Date, Reason " _
    & "FROM UODOUTDetail " _
    & "WHERE (UOD_License_Number = '{0}')"
    ''' <summary>
    ''' Query to get the UOD List for Today
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_UOD_LIST = _
    "SELECT UOD_License_Number, Outer_type, Booked_In, Audited, " _
    & "Estimated_Delivery_Date, Booked_In_Date, Reason " _
    & "FROM UODOUTDetail where (Immediate_License_Number='0000000000')"
    ''' <summary>
    ''' Query to get the UOD List for Today
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_UOD_CHILD_LIST = _
    "SELECT UOD_License_Number, Outer_type, Booked_In, Audited, " _
    & "Estimated_Delivery_Date, Booked_In_Date, Reason " _
    & "FROM UODOUTDetail " _
    & "WHERE (Immediate_License_Number = '{0}')"

    ''' <summary>
    ''' Query to get the list of crates for a Dolly selected
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_CRATE_LIST = _
    "SELECT UOD_License_Number, Outer_Type, Booked_In, Audited, Number_Childrens, Number_Of_Items " _
    & "FROM UODOUTDetail " _
    & "WHERE ( Immediate_License_Number = '{0}')"
    ''' <summary>
    ''' Query to get the item list for UOD
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_UODITEM_LIST = _
    "SELECT UODOUTItems.UOD_License_Plate, UODOUTItems.Boots_Code, " _
    & "UODOUTItems.Despatch_Quantity, BootsCodeView.Item_Desc, BootsCodeView.Second_Barcode " _
    & "FROM UODOUTItems, BootsCodeView  " _
    & "WHERE (UODOUTItems.UOD_License_Plate = '{0}') " _
    & "AND UODOUTItems.Boots_Code = BootsCodeView.Boots_Code"
    ''' <summary>
    ''' QUery to get the list of Items for Cartons in View Carton
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_ITEM_LIST_VIEW = _
    "SELECT BootsCodeView.Boots_Code, BootsCodeView.First_Barcode, BootsCodeView.Second_Barcode, " _
    & "BootsCodeView.Item_Desc, BootsCodeView.Item_Status, ASNListItems.Carton_Number, " _
    & "ASNListItems.Supplier_Number, ASNListItems.Despatched_Qty " _
    & "FROM ASNListItems, BootsCodeView " _
    & "WHERE (ASNListItems.Carton_Number = '{0}') " _
    & "AND ASNListItems.Boots_Code = BootsCodeView.Boots_Code " _
    & "GROUP BY BootsCodeView.Boots_Code, BootsCodeView.First_Barcode, " _
    & "BootsCodeView.Second_Barcode, BootsCodeView.Item_Desc, " _
    & "BootsCodeView.Item_Status, ASNListItems.Carton_Number, ASNListItems.Supplier_Number, " _
    & "ASNListItems.Despatched_Qty "



    Public Const GET_ITEM_LIST_VIEW_BOOKED = _
   "SELECT BootsCodeView.Boots_Code, BootsCodeView.First_Barcode, BootsCodeView.Second_Barcode, " _
   & "BootsCodeView.Item_Desc, BootsCodeView.Item_Status, ASNListItems.Carton_Number, " _
   & "ASNListItems.Supplier_Number, ASNListItems.Booked_Qty " _
   & "FROM ASNListItems, BootsCodeView " _
   & "WHERE (ASNListItems.Carton_Number = '{0}') " _
   & "AND ASNListItems.Boots_Code = BootsCodeView.Boots_Code " _
   & "GROUP BY BootsCodeView.Boots_Code, BootsCodeView.First_Barcode, " _
   & "BootsCodeView.Second_Barcode, BootsCodeView.Item_Desc, " _
   & "BootsCodeView.Item_Status, ASNListItems.Carton_Number, ASNListItems.Supplier_Number, " _
   & "ASNListItems.Booked_Qty "

    ''' <summary>
    ''' Query to get the item Details
    ''' </summary>
    ''' <remarks></remarks>
    
    Public Const GET_ITEM_DETAILS_UOD As String = _
   "SELECT UODOUTItems.Boots_Code, UODOUTItems.UOD_License_Plate, BootsCodeView.Item_Desc, BootsCodeView.Item_Status, " _
   & "BootsCodeView.Second_Barcode, UODOUTItems.Despatch_Quantity AS Item_Quantity " _
   & "FROM  BootsCodeView INNER JOIN " _
   & "BarCodeView ON BootsCodeView.Boots_Code = BarCodeView.Boots_Code INNER JOIN " _
   & "UODOUTItems ON BootsCodeView.Boots_Code = UODOUTItems.Boots_Code " _
   & "WHERE BarCodeView.BarCode = '{0}')"

    'Public Const GET_ITEM_DETAILS_UOD As String = _
    '"SELECT UODOUTItems.Boots_Code, UODOUTItems.UOD_License_Plate, BootsCodeView.Item_Desc, " _
    '& "BootsCodeView.Item_Status, BootsCodeView.Second_Barcode, UODOUTItems.Despatch_Quantity As Item_Quantity " _
    '& "FROM UODOUTItems, BootsCodeView " _
    '& "WHERE (BootsCodeView.Second_Barcode = '{0}') " _
    '& "AND UODOUTItems.Boots_Code = BootsCodeView.Boots_Code "

    ''' <summary>
    ''' Query to get the item Details
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_ITEM_DETAILS As String = _
    "SELECT BarCodeView.Boots_Code, BootsCodeView.Item_Status, BootsCodeView.SEL_Desc, " _
    & "BootsCodeView.First_Barcode, BootsCodeView.Second_Barcode " _
    & "FROM BootsCodeView INNER JOIN " _
    & "BarCodeView ON BootsCodeView.Boots_Code = BarCodeView.Boots_Code " _
    & "WHERE (BarCodeView.BarCode = '{0}')"

    'Public Const GET_ITEM_DETAILS As String = _
    '"SELECT Item_Desc, Second_Barcode, Boots_Code, First_Barcode " _
    '& "FROM BootsCodeView " _
    '& "WHERE (Second_Barcode = '{0}') "
    ''' <summary>
    ''' Query to get the item Details
    ''' </summary>
    ''' <remarks></remarks>
    
    Public Const GET_ITEM_DETAILS_DIRECTS As String = _
    "SELECT UODOUTItems.Boots_Code, UODOUTItems.UOD_License_Plate, BootsCodeView.Item_Desc, " _
    & "BootsCodeView.Item_Status, BootsCodeView.Second_Barcode AS Item_Quantity " _
    & "FROM  BootsCodeView INNER JOIN " _
    & "BarCodeView ON BootsCodeView.Boots_Code = BarCodeView.Boots_Code INNER JOIN " _
    & "UODOUTItems ON BootsCodeView.Boots_Code = UODOUTItems.Boots_Code " _
    & "WHERE BarCodeView.BarCode = '{0}')"

    'Public Const GET_ITEM_DETAILS_DIRECTS As String = _
    '"SELECT UODOUTItems.Boots_Code, UODOUTItems.UOD_License_Plate, BootsCodeView.Item_Desc, " _
    '& "BootsCodeView.Item_Status, BootsCodeView.Second_Barcode, As Item_Quantity " _
    '& "FROM UODOUTItems, BootsCodeView " _
    '& "WHERE (BootsCodeView.Second_Barcode = '{0}') " _
    '& "AND UODOUTItems.Boots_Code = BootsCodeView.Boots_Code "


    ''' <summary>
    ''' Query to get the item details for Carton
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_ITEM_DETAILS_BOOTSCODE As String = _
    "SELECT SEL_Desc, Second_Barcode, Boots_Code, First_Barcode " _
    & "FROM BootsCodeView " _
    & "WHERE (Boots_Code = '{0}') "
    ''' <summary>
    ''' Query to get tghe item details for Order
    ''' </summary>
    ''' <remarks></remarks>
    ''' 

    Public Const GET_ITEM_DETAILS_FORORDER As String = _
    "SELECT BootsCodeView.Boots_Code, BootsCodeView.First_Barcode, BootsCodeView.Second_Barcode, " _
    & "BootsCodeView.Item_Desc, DirectListItems.Order_No, DirectListItems.Exp_Quantity As Item_Quantity " _
    & "FROM  BootsCodeView INNER JOIN " _
    & "BarCodeView ON BootsCodeView.Boots_Code = BarCodeView.Boots_Code INNER JOIN " _
    & "DirectListItems ON BootsCodeView.Boots_Code = DirectListItems.Boots_Code " _
    & "WHERE BarCodeView.BarCode = '{0}')"

    'Public Const GET_ITEM_DETAILS_FORORDER As String = _
    '"SELECT     BootsCodeView.Boots_Code, BootsCodeView.First_Barcode, BootsCodeView.Second_Barcode, " _
    '& "BootsCodeView.Item_Desc, DirectListItems.Order_No, DirectListItems.Exp_Quantity As Item_Quantity " _
    '& "FROM BootsCodeView, DirectListItems " _
    '& "WHERE  BootsCodeView.Second_Barcode = '{0}' " _
    '& "AND BootsCodeView.Boots_Code = DirectListItems.Boots_Code "

    ''' <summary>
    ''' Query to get the item list for Orders
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_ITEM_LIST_FOR_ORDERS As String = _
    "SELECT     DirectListItems.Order_No, DirectListItems.Exp_Quantity, DirectListItems.Boots_Code, " _
    & "DirectListItems.Supplier, BootsCodeView.Item_Desc, BootsCodeView.Second_Barcode, DirectListItems.List_ID " _
    & "FROM DirectListItems, BootsCodeView " _
    & "WHERE (DirectListItems.Order_No = '{0}') " _
    & "AND DirectListItems.Boots_Code = BootsCodeView.Boots_Code" _
    & " AND DirectListItems.Supplier='{1}'"
    ''' <summary>
    ''' Query to get the details of a scanned carton
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_CARTON_SCAN_DATA As String = _
    "SELECT ASNList.ASN_Number, ASNList.Supplier_Ref, ASNList.Carton_Number, ASNList.Cartons_In_ASN, ASNList.Status, " _
    & "ASNList.Exp_Delivery_Date, ASNList.Total_Item_In_Carton " _
    & "FROM ASNList  " _
    & "WHERE (ASNList.Supplier_Ref = '{0}') " _
    & "AND (ASNList.Carton_Number = '{1}' ) "
    ' & "AND ASNList.Supplier_Ref = SuppliersList.Supplier_ID "
    ''' <summary>
    ''' QUery to get the list of cartons for a supplier
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_CARTON_LIST As String = _
    "SELECT Supplier_Ref, Carton_Number, Status, Exp_Delivery_Date, Total_Item_In_Carton " _
    & "FROM  ASNList " _
    & "WHERE (Supplier_Ref = '{0}')"
    ''' <summary>
    ''' QUery to get the list of items for a carton number
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_ITEM_LIST As String = _
    "SELECT BootsCodeView.Second_Barcode, BootsCodeView.Item_Desc, " _
    & "BootsCodeView.Item_Status, ASNListItems.Despatched_Qty, ASNListItems.Boots_Code " _
    & "FROM ASNListItems, BootsCodeView " _
    & "WHERE (ASNListItems.Carton_Number = '{0}') " _
    & "AND ASNListItems.Boots_Code = BootsCodeView.Boots_Code"
    ''' <summary>
    ''' Query to get the list of order numbers for a supplier number
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_ORDER_LIST As String = _
    "SELECT Supplier, Order_No, Exp_Delivery_Date, No_Of_Order_Items, No_Of_Items_BookedIn, Confirm_Flag, " _
    & " BC, Source, Order_Suffix " _
    & "FROM DirectList " _
    & "WHERE (Supplier = '{0}')"
    ''' <summary>
    ''' QUery to get the Item details for UOD in Audit
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_UOD_DETAILS As String = _
    "SELECT UODOUTItems.UOD_License_Plate, UODOUTItems.Boots_Code, UODOUTItems.Despatch_Quantity, " _
    & "BootsCodeView.Second_Barcode, BootsCodeView.Item_Desc, BootsCodeView.Item_Status " _
    & "FROM UODOUTItems, BootsCodeView " _
    & "WHERE (UODOUTItems.UOD_License_Plate = '{0}' " _
    & "AND BootsCodeView ON UODOUTItems.Boots_Code = BootsCodeView.Boots_Code"


    ''' <summary>
    ''' To get Boots Code using product code.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_BOOTS_CODE = _
    "SELECT Boots_Code " _
    & "FROM BarCodeView  " _
    & "WHERE (BarCode = '{0}')"
End Class

