#If NRF Then
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
Public Class QueryMacro
    ''' <summary>
    ''' Query to Get all the available Count list.
    ''' </summary>
    ''' <remarks>
	'''SFA  DEf#840 - Display outstanding SF count</remarks>
    Public Const GET_COUNT_LIST = _
    "SELECT List_ID, List_Type, List_Name, OutStanding_SFCount " _
    & "FROM CountList " _
    & "ORDER BY List_Type"
   
    'Public Const GET_COUNT_LIST_ITEMS = _
    ' "SELECT CountListItems.Sequence_Number, CountListItems.Boots_Code, CountListItems.Back_Shop_Count, " _
    ' & "CountListItems.Sales_Floor_Count, BootsCodeView.First_Barcode, " _
    ' & "BootsCodeView.Second_Barcode, BootsCodeView.Item_Status, " _
    ' & "BootsCodeView.SEL_Desc, BootsCodeView.SOD_TSF, " _
    ' & "BootsCodeView.PSP_Flag " _
    ' & "FROM CountListItems INNER JOIN " _
    ' & "BootsCodeView ON CountListItems.Boots_Code = BootsCodeView.Boots_Code " _
    ' & "WHERE (CountListItems.List_ID = '{0}')"
    ''' <summary>
    ''' Query to getitem count for a count list  - Change query once csv  updation done
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_COUNT_LIST_ITEMS = _
     "SELECT BootsCodeView.Boots_Code AS Check_Planner, CountListItems.Boots_Code, CountListItems.Sequence_Number, " _
     & "CountListItems.Back_Shop_Count, CountListItems.Sales_Floor_Count, BootsCodeView.First_Barcode, " _
     & "BootsCodeView.Second_Barcode, BootsCodeView.Item_Status, BootsCodeView.SEL_Desc, CountListItems.Sales_At_POD_Dock, " _
     & "BootsCodeView.PSP_Flag, CountListItems.Stock_Figure, CountListItems.Back_Shop_PSP_Count FROM CountListItems LEFT OUTER JOIN " _
     & "BootsCodeView ON BootsCodeView.Boots_Code = CountListItems.Boots_Code WHERE(CountListItems.List_ID = '{0}')"
    'Stock File Accuracy  - added new query
    ''' <summary>
    ''' Query to get unique planners in store where count need to be done
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_COUNT_LIST_SITE_INFO = _
    "SELECT CAST(LivePOG.POG_Desc AS nvarchar) AS Site " _
    & "FROM CountListPlanograms INNER JOIN LivePOG ON CountListPlanograms.Module_ID = LivePOG.Module_ID " _
    & "WHERE (CountListPlanograms.List_ID = '{0}') GROUP BY CAST(LivePOG.POG_Desc AS nvarchar)"
    ''' <summary>
    ''' Query to get item count in a site
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_COUNT_LIST_SF_ITEMS = _
    "SELECT DISTINCT CountListPlanograms.Boots_Code, CountListItems.Sales_Floor_Count AS Count " _
    & "FROM CountListPlanograms INNER JOIN LivePOG ON CountListPlanograms.Module_ID = LivePOG.Module_ID " _
    & "INNER JOIN CountListItems ON CountListPlanograms.List_ID = CountListItems.List_ID AND " _
    & "CountListPlanograms.Boots_Code = CountListItems.Boots_Code WHERE " _
    & "(CountListPlanograms.List_ID = '{0}') AND (LivePOG.POG_Desc LIKE '{1}')"
    ''' <summary>
    ''' Query to get site coutn for an item in one planner
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_COUNT_LIST_ITEM_SITE_COUNT = _
    "SELECT CountListPlanograms.Module_ID, CountListPlanograms.Repeat_Count, CountListPlanograms.Module_Seq " _
    & "FROM CountListPlanograms INNER JOIN LivePOG ON CountListPlanograms.Module_ID = LivePOG.Module_ID " _
    & "WHERE (CountListPlanograms.List_ID = '{0}') AND (LivePOG.POG_Desc LIKE '{1}') AND " _
    & "(CountListPlanograms.Boots_Code = '{2}')"
    ''' <summary>
    ''' Query to get site information for BS
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_COUNT_LIST_PSP_INFO = _
    "SELECT COUNT(CountListItems.Boots_Code) AS PSP_Items FROM BootsCodeView INNER JOIN " _
    & "CountListItems ON BootsCodeView.Boots_Code = CountListItems.Boots_Code " _
    & "WHERE(BootsCodeView.PSP_Flag = 'Y') AND (CountListItems.List_ID = '{0}')"
    ''' <summary>
    ''' Query to get product list for MBS
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_COUNT_LIST_MBS_PRODUCTS = _
     "SELECT CountListItems.Boots_Code, CountListItems.Back_Shop_Count AS Count FROM " _
     & "CountListItems INNER JOIN BootsCodeView ON CountListItems.Boots_Code = BootsCodeView.Boots_Code " _
     & "WHERE (CountListItems.List_ID = '{0}')"
    ''' <summary>
    ''' Query to get product list for PSP
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_COUNT_LIST_PSP_PRODUCTS = _
    "SELECT CountListItems.Boots_Code, Back_Shop_PSP_Count AS Count FROM CountListItems INNER JOIN  " _
    & "BootsCodeView ON CountListItems.Boots_Code = BootsCodeView.Boots_Code " _
    & "WHERE (CountListItems.List_ID = {0}) AND (BootsCodeView.PSP_Flag = 'Y')"
    ''' <summary>
    ''' Query to get multisitelist for an item
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_COUNT_LIST_MULTISITE_LIST = _
    "SELECT CountListPlanograms.Boots_Code, CountListPlanograms.Module_ID, " _
    & "CountListPlanograms.Module_Seq, CountListPlanograms.Repeat_Count, LivePOG.POG_Desc, " _
    & "ModuleList.Module_Desc FROM CountListPlanograms INNER JOIN LivePOG ON " _
    & "CountListPlanograms.Module_ID = LivePOG.Module_ID INNER JOIN ModuleList ON " _
    & "CountListPlanograms.Module_Seq = ModuleList.Module_Seq AND " _
    & "CountListPlanograms.Module_ID = ModuleList.Module_ID WHERE " _
    & "(CountListPlanograms.Boots_Code = {0}) AND (CountListPlanograms.List_ID = {1})"
    ''' <summary>
    ''' Query to get details of an item using Boots Code.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_DETAILS_USING_BC = _
    "SELECT     Boots_Code, First_Barcode, Second_Barcode, Item_Status, " _
    & "SEL_Desc, SOD_TSF,  Item_Desc, PSP_Flag, Date_Last_Counted " _
    & " FROM     BootsCodeView " _
    & "WHERE    (Boots_Code = '{0}')"
    ''' <summary>
    ''' Query to get details of an item using Product Code.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_DETAILS_USING_PC = _
    "SELECT BarCodeView.BarCode, BarCodeView.Boots_Code, BootsCodeView.Item_Status, " _
    & "BootsCodeView.SEL_Desc, BootsCodeView.SOD_TSF, BootsCodeView.Item_Desc, " _
    & "BootsCodeView.First_Barcode, BootsCodeView.Second_Barcode,BootsCodeView.PSP_Flag, Date_Last_Counted " _
    & "FROM BootsCodeView INNER JOIN " _
    & "BarCodeView ON BootsCodeView.Boots_Code = BarCodeView.Boots_Code " _
    & "WHERE (BarCodeView.BarCode = '{0}')"
   
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Const VALIDATE_USING_PC_AND_BC = _
    "SELECT * FROM " _
    & "BarCodeView WHERE (BarCode = '{0}') AND (Boots_Code='{1}')"
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
    ''' Query to get item details of an item using Product Code for 
    ''' item info module.
    ''' Modified for SFA due to db changes
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_ITEM_DETAILS_ALL_USING_PC = _
    "SELECT BarCodeView.Boots_Code, BootsCodeView.Item_Status, " _
    & "BootsCodeView.SEL_Desc, BootsCodeView.SOD_TSF, BootsCodeView.Current_Price, " _
    & "BootsCodeView.First_Barcode, BootsCodeView.Second_Barcode, " _
    & "BootsCodeView.Item_Status3, BootsCodeView.Deal_No1, " _
    & "BootsCodeView.Deal_No2, BootsCodeView.Deal_No3, BootsCodeView.Deal_No4, " _
    & "BootsCodeView.Deal_No5, BootsCodeView.Deal_No6, BootsCodeView.Deal_No7, " _
    & "BootsCodeView.Deal_No8, BootsCodeView.Deal_No9, BootsCodeView.Deal_No10 " _
    & "FROM BootsCodeView INNER JOIN " _
    & "BarCodeView ON BootsCodeView.Boots_Code = BarCodeView.Boots_Code " _
    & "WHERE (BarCodeView.BarCode = '{0}')"
    ''' <summary>
    ''' Query to get details for printing SELs and Clearance labels.
    ''' Modified for SFA due to db changes
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_SEL_PRINT_DETAILS = _
    "SELECT BootsCodeView.Boots_Code, BootsCodeView.First_Barcode, BootsCodeView.Second_Barcode, " _
    & "BootsCodeView.Item_Status, BootsCodeView.SEL_Desc, BootsCodeView.SOD_TSF, BootsCodeView.Item_Desc, " _
    & "BootsCodeView.Supply_Route, BootsCodeView.Unit_Name, BootsCodeView.Unit_Item_Quantity, " _
    & "BootsCodeView.Current_Price, BootsCodeView.PH1_Price, BootsCodeView.PH2_Price, BootsCodeView.Unit_Measure, " _
    & "BootsCodeView.Markdown_Flag, BootsCodeView.Label_Type, BootsCodeView.Item_Status3, BootsCodeView.Item_Status8 " _
    & "FROM BootsCodeView INNER JOIN BarCodeView ON BootsCodeView.Boots_Code = BarCodeView.Boots_Code " _
    & "WHERE (BarCodeView.BarCode = '{0}')"
    ''' <summary>
    ''' Query to get the entire list of picking list available
    '''  Removed substrin from userlist
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_PICKING_LIST = _
    "SELECT PickingList.List_Time, PickingList.List_Type, UserList.User_Name, " _
    & "PickingList.Number_Of_Items, PickingList.List_ID, PickingList.List_Status, " _
    & "PickingList.Creator_ID " _
    & "FROM PickingList INNER JOIN " _
    & "UserList ON UserList.User_ID = PickingList.Creator_ID  " _
    & "WHERE (PickingList.List_Status='U') AND (List_Type <> 'N') AND PickingList.Number_Of_Items > 0"
    ''' <summary>
    ''' To Get picking list item details for a picking list denoted by @ListID
    ''' </summary>
    ''' <remarks>
    ''' Update to consider item status as 'X' to be picked item.
    ''' SFA DEF# 847 - Retrieve Stock figure fom pickListItem table 
    ''' </remarks>
    Public Const GET_PICKING_LIST_DETAILS = _
    "SELECT PickListItems.Boots_Code, BootsCodeView.First_Barcode, BootsCodeView.PSP_Flag, " _
    & "BootsCodeView.Second_Barcode, BootsCodeView.Item_Status, PickListItems.Stock_Figure, " _
    & "BootsCodeView.SEL_Desc, BootsCodeView.BC, BootsCodeView.Product_Group, BootsCodeView.Item_Desc, " _
    & "PickListItems.Shelf_Qty, PickListItems.Fill_Qty, PickListItems.Back_Shop_Count, " _
    & "PickListItems.Item_Status  AS List_Item_Status, PickListItems.Sequence_Number " _
    & "FROM BootsCodeView INNER JOIN " _
    & "PickListItems ON BootsCodeView.Boots_Code = PickListItems.Boots_Code " _
    & "WHERE (PickListItems.List_ID = '{0}') AND (PickListItems.Item_Status NOT IN ('P','X'))"
    'SFA
    ''' <summary>
    ''' Query to get price check details for an item. modified for SFA
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_PRICE_CHECK_DETAILS = _
    "SELECT BootsCodeView.Current_Price, BootsCodeView.Pending_Price, BootsCodeView.Last_Price_Check_Date, " _
    & "ProductGroup.Product_Grp_Flag, BootsCodeView.Pending_Price_Date " _
    & "FROM BarCodeView INNER JOIN " _
    & "BootsCodeView ON BarCodeView.Boots_Code = BootsCodeView.Boots_Code INNER JOIN " _
    & "ProductGroup ON BootsCodeView.Product_Group = ProductGroup.Product_Grp_No WHERE " _
    & "(BarCodeView.BarCode = '{0}') AND (BarCodeView.Boots_Code = '{1}')"
    '''<summary >
    ''' Query to get the Line list items.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_LINELIST = _
    "SELECT ModuleListItems.Module_ID, ModuleListItems.Module_Seq, BootsCodeView.Boots_Code, " _
    & "ModuleListItems.Shelf_Numb, ModuleListItems.Facings, ModuleListItems.Notch_No, " _
    & "BootsCodeView.Item_Desc, ShelfDesc.Shelf_Desc FROM BootsCodeView INNER JOIN " _
    & "ModuleListItems WITH (INDEX (ModuleListSeq)) ON BootsCodeView.Boots_Code = ModuleListItems.Boots_Code " _
    & "INNER JOIN ShelfDesc ON ModuleListItems.Shelf_Desc_Index = ShelfDesc.Shelf_Desc_Index " _
    & "WHERE (ModuleListItems.Module_ID = '{0}') AND (ModuleListItems.Module_Seq = '{1}')"
    ' _
    '& "ORDER BY Shelf_Numb"
    ''' <summary>
    ''' Query to get the list of modules available for the selected planner.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_MODULE_LIST = _
    "SELECT Module_ID, Module_Seq, Module_Desc " _
    & "FROM ModuleList " _
    & "WHERE (Module_ID = '{0}') " _
    & "ORDER BY Module_Seq"
    ''' <summary>
    ''' Query to get the list of modules available for the selected planner
    ''' under search planner.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_MODULE_LIST_FOR_SEARCH_PLANNER = _
    "SELECT ModuleList.Module_ID, ModuleList.Module_Seq, ModuleList.Module_Desc " _
    & "FROM ModuleList INNER JOIN " _
    & "ModuleListItems WITH(INDEX (ModuleListItems_IDX)) ON ModuleList.Module_ID = ModuleListItems.Module_ID " _
    & "AND ModuleList.Module_Seq = ModuleListItems.Module_Seq " _
    & "WHERE (ModuleListItems.Boots_Code = @BootsCode) AND (ModuleListItems.Module_ID = @ModID)"

    '"SELECT Module_ID, Module_Seq, Module_Desc FROM ModuleList " _
    '& "WHERE (Module_ID = @ModID) AND (Module_Seq IN " _
    '& "(SELECT Module_Seq FROM ModuleListItems " _
    '& " WHERE (Boots_Code = @BootsCode) AND (Module_ID = @ModID)))"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Const VALIDATE_PRODUCT_USING_BC = _
    "SELECT * FROM BootsCodeView " _
    & "WHERE (Boots_Code = '{0}')"
    ''' <summary>
    ''' Query to validate the presence of a product using product code.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const VALIDATE_PRODUCT_USING_PC = _
    "SELECT * FROM BootsCodeView INNER JOIN " _
    & "BarCodeView ON BootsCodeView.Boots_Code = BarCodeView.Boots_Code " _
    & "WHERE (BarCodeView.BarCode = '{0}')"
    ''' <summary>
    ''' Query to get the list of planners using Boots Code.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_PLANNER_LIST_USING_BC = _
    "SELECT Live_POG1, Live_POG2, Live_POG3, Live_POG4, Live_POG5, " _
    & "Live_POG6, Live_POG7, Live_POG8, Live_POG9, Live_POG10, " _
    & "Live_POG11, Live_POG12, Live_POG13, Live_POG14, Live_POG15, Live_POG16 " _
    & "FROM BootsCodeView " _
    & "WHERE (Boots_Code = '{0}')"
    '"Select Module_ID From ModuleListItems WHERE (Boots_Code = '{0}')"
    ''' <summary>
    ''' Query to get the list of planners for a product denoted by
    ''' Product Code.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_PLANNER_LIST_USING_PC = _
    "SELECT BootsCodeView.Live_POG1, BootsCodeView.Live_POG2, " _
    & "BootsCodeView.Live_POG3, BootsCodeView.Live_POG4, BootsCodeView.Live_POG5, " _
    & "BootsCodeView.Live_POG6, BootsCodeView.Live_POG7, BootsCodeView.Live_POG8, " _
    & "BootsCodeView.Live_POG9, BootsCodeView.Live_POG10, " _
    & "BootsCodeView.Live_POG11, BootsCodeView.Live_POG12, BootsCodeView.Live_POG13, " _
    & "BootsCodeView.Live_POG14, BootsCodeView.Live_POG15, BootsCodeView.Live_POG16 " _
    & "FROM BootsCodeView INNER JOIN " _
    & "BarCodeView ON BootsCodeView.Boots_Code = BarCodeView.Boots_Code " _
    & "WHERE (BarCodeView.BarCode = '{0}')"
    ''' <summary>
    ''' Query to get the list of all planners present.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_PLANNER_DETAILS = _
    "SELECT Module_ID, POG_Desc, Start_Date " _
    & "FROM LivePOG " _
    & "WHERE (Module_ID = '{0}')"
    ''' <summary>
    ''' Query to get the details for multisite items.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_MULTISITE_DETAILS = _
    "SELECT ModuleList.Module_Seq, ModuleList.Repeat_Count, ModuleListItems.Shelf_Numb, " _
    & "ModuleList.Module_Desc, ModuleList.Module_ID, LivePOG.POG_Desc, ModuleListItems.PSC " _
    & "FROM ModuleList INNER JOIN " _
    & "ModuleListItems  WITH(INDEX (ModuleListItems_IDX)) ON ModuleList.Module_ID = ModuleListItems.Module_ID " _
    & "AND ModuleList.Module_Seq = ModuleListItems.Module_Seq " _
    & "INNER JOIN LivePOG ON LivePOG.Module_ID = ModuleList.Module_ID " _
    & "WHERE ModuleListItems.Module_ID IN ({1}) AND (ModuleListItems.Boots_Code = '{0}')"
    ''' <summary>
    ''' Query to get the details for multisite items.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_PICKINGLIST_MULTISITE_DETAILS = _
     "SELECT PickingListPlanograms.Repeat_Count, ModuleList.Module_Desc, LivePOG.POG_Desc " _
    & "FROM PickingListPlanograms INNER JOIN " _
    & "ModuleList ON PickingListPlanograms.Module_Seq = ModuleList.Module_Seq AND " _
    & "PickingListPlanograms.[Module_ID] = ModuleList.Module_ID INNER JOIN " _
    & "LivePOG ON ModuleList.Module_ID = LivePOG.Module_ID " _
    & "WHERE (PickingListPlanograms.List_ID = '{1}') AND (PickingListPlanograms.Boots_Code = '{0}') "

    '"SELECT LivePOG.Module_ID, ModuleList.Module_Seq, ModuleListItems.Shelf_Numb, " _
    '& "ModuleList.Module_Desc, LivePOG.Start_Date FROM LivePOG INNER JOIN " _
    '& "ModuleList ON LivePOG.Module_ID = ModuleList.Module_ID INNER JOIN " _
    '& "ModuleListItems ON ModuleList.Module_ID = ModuleListItems.Module_ID " _
    '& "AND ModuleList.Module_Seq = ModuleListItems.Module_Seq " _
    '& "WHERE (ModuleListItems.Module_ID = '{1}') AND (ModuleListItems.Boots_Code = '{0}')"

    ''' <summary>
    ''' Query to get the category list
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_CATEGORY_LIST = _
    "SELECT Category_Desc, Category_ID " _
    & "FROM POGCategory " _
    & "WHERE (Core_Non_Core = '{0}')"
    ''' <summary>
    ''' Query to get the list of planners available for a category
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_PLANNER_LIST_USING_CATEGORYID = _
    "SELECT Module_ID, POG_Desc, Start_Date " _
    & "FROM LivePOG " _
    & "WHERE (Category_ID1 = '{0}') OR " _
    & "(Category_ID2 = '{0}') OR " _
    & "(Category_ID3 = '{0}')"
    ''' <summary>
    ''' Query to get the price check target details.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_PC_TARGET_DETAILS = _
    "SELECT PC_Weekly_Target, PC_Count_Current_Week, PC_Date_Threshold " _
    & "FROM CountsAndTargets"
    ''' <summary>
    ''' Query to get the user details.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_USER_DETAILS = _
    "SELECT User_ID, Password, Supervisor_Flag, Stock_Adjustment_Flag " _
    & "FROM UserList " _
    & "WHERE (User_ID = '{0}')"
    ''' <summary>
    ''' To get deal details for a deal
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_DEAL_DETAILS = _
    "SELECT Deal_Type, Start_Date, End_Date " _
    & "FROM DealList " _
    & "WHERE (Deal_Number = '{0}')"
    ''' <summary>
    ''' To get Boots Code using product code.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_BOOTS_CODE = _
    "SELECT Boots_Code " _
    & "FROM BarCodeView  " _
    & "WHERE (BarCode = '{0}')"
    ''' <summary>
    ''' To get Product code using Boots code.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_PRODUCT_CODE = _
    "SELECT Boots_Code, First_Barcode, Second_Barcode " _
    & "FROM     BootsCodeView " _
    & "WHERE    (Boots_Code = '{0}')"
    ''' <summary>
    ''' To get the item description
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_ITEM_DESCRIPTION = _
    "SELECT Item_Desc " _
    & "FROM BootsCodeView  " _
    & "WHERE (Boots_Code = {0})"
    ''' <summary>
    ''' To get the dummy values for bootscode
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_DUMMY_VALUE_BOOTCODE = _
    "SELECT * from BootsCodeView"
    ''' <summary>
    ''' To get the dummy values for barcode
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_DUMMY_VALUE_BARCODE = _
    "SELECT * from BarCodeView"
    ''' <summary>
    ''' To get the PSC
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_PSC = _
    "SELECT ModuleListItems.PSC AS PSC, ModuleList.Repeat_Count AS Repeat_Count " _
    & "FROM ModuleList INNER JOIN " _
    & "ModuleListItems ON ModuleList.Module_ID = ModuleListItems.Module_ID AND " _
    & "ModuleList.Module_Seq = ModuleListItems.Module_Seq INNER JOIN " _
    & "LivePOG ON LivePOG.Module_ID = ModuleList.Module_ID " _
    & "WHERE (ModuleListItems.Boots_Code={0})"
End Class
#End If

