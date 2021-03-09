Imports System
Imports System.Data
Imports System.Text.RegularExpressions


#If NRF Then
Imports System.Data.SqlServerCe
'''******************************************************************************
''' <FileName>SMNRFDataSource.vb</FileName>
''' <summary>
''' This class is the data source class for the Shelf Management application.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>14-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''******************************************************************************

Public Class SMNRFDataSource
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub
    ''' <summary>
    ''' Gets the list of all Count list available in the active tables.
    ''' </summary>
    ''' <param name="arrObjectList">Array reference to update the details of 
    ''' Count list in objects.</param>
    ''' <returns>Bool
    ''' True - If successfully recevied and updated the details in the object 
    ''' array.
    ''' False - If any occured during the updation.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetCountList(ByRef arrObjectList As ArrayList) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_COUNT_LIST
        Dim dsList As DataSet = Nothing
        Dim bStatus As Boolean = Nothing
        Dim objCountList As CLProductGroupInfo = Nothing
        Try
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'Traverse through each element of the data set and update the details
            'in the object.
            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objCountList = New CLProductGroupInfo()
                    With objCountList
                        .ListID = dsList.Tables(0).Rows(iCount)("List_ID").ToString()
                        .ListDescription = dsList.Tables(0).Rows(iCount) _
                                           ("List_Name").ToString()
                        .ListType = dsList.Tables(0).Rows(iCount) _
                                    ("List_Type").ToString()
                        .NumberOfItems = dsList.Tables(0).Rows(iCount) _
                                        ("OutStanding_SFCount").ToString()
                        '.DateOfLastCount = dsList.Tables(0).Rows(iCount) _  'SIT Removed as part of SFA
                        '                ("Date_Of_Last_Count").ToString()
                    End With
                    'Add the object with details to the array list.
                    arrObjectList.Add(objCountList)
                Next
            Else
                'return false if the dataset does not have any data.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false in case of any exception.
            Return False
        Finally
            dsList.Dispose()
            objCountList = Nothing
            strSqlCmd = Nothing
        End Try
        'If control reaches here after executing the successfully return true.
        Return True
    End Function
    ''' <summary>
    ''' Gets the details of all items in the Count list selected by the user.
    ''' Update details of each items in an object in the array list.
    ''' </summary>
    ''' <param name="strListID">List ID of a count list</param>
    ''' <param name="arrCLProductInfo">Varible to store the item  list</param>
    ''' <returns>Bool
    ''' True - If successfully recevied and updated the details in 
    ''' the object array.
    ''' False - If any occured during the updation.</returns>
    ''' <remarks></remarks>
    Public Function GetCountListItemDetails(ByVal strListID As String, ByRef arrCLProductInfo As ArrayList) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_COUNT_LIST_ITEMS
        strSqlCmd = String.Format(strSqlCmd, strListID)
        Dim dsListItems As DataSet = Nothing
        'SFA   
        Dim strFirstBarCode As String = ""
        Dim strSecondBarCode As String = ""
        Dim strPSPFlag As String
        Dim strPlannerCheck As String = ""
        Dim iSalesFloorQty As Integer
        Dim iBackShopQty As Integer
        Dim iPSPQty As Integer
        Dim objArrayItem As CLProductInfo

        ' end
        Try
            dsListItems = New DataSet()
            dsListItems = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'Traverse through each element of the data set and update the details
            'in the object.
            'If arrCLProductInfo.Count.Equals _
            '   (dsListItems.Tables(0).Rows.Count) Then
            'Update the properties int he object with the values retreived from
            'database.
            'For Each objArrayItem As CLProductInfo In arrCLProductInfo
            If dsListItems.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsListItems.Tables(0).Rows.Count - 1
                    strPlannerCheck = dsListItems.Tables(0).Rows(iCount)("Check_Planner").ToString()
                    If Not strPlannerCheck = "" And Not strPlannerCheck Is Nothing Then
                        objArrayItem = New CLProductInfo()
                        With objArrayItem
                            .SequenceNumber = dsListItems.Tables(0) _
                                        .Rows(iCount)("Sequence_Number").ToString()
                            .BootsCode = dsListItems.Tables(0) _
                                         .Rows(iCount)("Boots_Code").ToString()
                            strPSPFlag = dsListItems.Tables(0) _
                                         .Rows(iCount)("PSP_Flag").ToString()
                            If strPSPFlag = "Y" Or strPSPFlag = "y" Then
                                .PendingSalesFlag = True
                            Else
                                .PendingSalesFlag = False
                            End If
                            .SalesAtPODDock = dsListItems.Tables(0) _
                                         .Rows(iCount)("Sales_At_POD_Dock").ToString()
                            .Description = dsListItems.Tables(0) _
                                         .Rows(iCount)("SEL_Desc").ToString().Trim()

                            .ProductCode = dsListItems.Tables(0) _
                                         .Rows(iCount)("Boots_Code").ToString().Substring(0, 6).PadLeft(12, "0")

                            .TSF = dsListItems.Tables(0) _
                                         .Rows(iCount)("Stock_Figure").ToString()
                            .Status = dsListItems.Tables(0) _
                                         .Rows(iCount)("Item_Status").ToString()

                            .SalesFloorQuantity = dsListItems.Tables(0) _
                                         .Rows(iCount)("Sales_Floor_Count").ToString()
                            .BackShopQuantity = dsListItems.Tables(0) _
                                         .Rows(iCount)("Back_Shop_Count").ToString()
                            .BackShopMBSQuantity = dsListItems.Tables(0) _
                                        .Rows(iCount)("Back_Shop_Count").ToString()
                            .BackShopPSPQuantity = dsListItems.Tables(0) _
                                         .Rows(iCount)("Back_Shop_PSP_Count").ToString()
                            iSalesFloorQty = .SalesFloorQuantity
                            iBackShopQty = .BackShopQuantity
                            iPSPQty = .BackShopPSPQuantity

                            If (iSalesFloorQty < 0) Then
                                iSalesFloorQty = 0
                            End If
                            If (iBackShopQty < 0) Then
                                iBackShopQty = 0
                            End If
                            If (iPSPQty < 0) Then
                                iPSPQty = 0
                            End If

                            If .SalesFloorQuantity < 0 And .BackShopQuantity < 0 And .BackShopPSPQuantity < 0 Then
                                .TotalQuantity = .SalesFloorQuantity + .BackShopQuantity + .BackShopPSPQuantity
                            Else
                                .TotalQuantity = iSalesFloorQty + iBackShopQty + iPSPQty
                            End If
                            arrCLProductInfo.Add(objArrayItem)
                        End With
                    Else
                        'Handle unknown items
                        objArrayItem = New CLProductInfo()
                        With objArrayItem
                            .SequenceNumber = dsListItems.Tables(0) _
                                        .Rows(iCount)("Sequence_Number").ToString()
                            .BootsCode = dsListItems.Tables(0) _
                                         .Rows(iCount)("Boots_Code").ToString()
                            .PendingSalesFlag = False
                            .SalesAtPODDock = dsListItems.Tables(0) _
                                        .Rows(iCount)("Sales_At_POD_Dock").ToString()
                            .Description = "Unknown Item   Check from the Controller"
                            .ProductCode = dsListItems.Tables(0) _
                                         .Rows(iCount)("Boots_Code").ToString().Substring(0, 6).PadLeft(12, "0")
                            .TSF = "N/A"
                            .Status = "N/A"
                            .SalesFloorQuantity = dsListItems.Tables(0) _
                                         .Rows(iCount)("Sales_Floor_Count").ToString()
                            .BackShopQuantity = dsListItems.Tables(0) _
                                         .Rows(iCount)("Back_Shop_Count").ToString()
                            .BackShopMBSQuantity = dsListItems.Tables(0) _
                                        .Rows(iCount)("Back_Shop_Count").ToString()
                            .BackShopPSPQuantity = dsListItems.Tables(0) _
                                         .Rows(iCount)("Back_Shop_PSP_Count").ToString()
                            iSalesFloorQty = .SalesFloorQuantity
                            iBackShopQty = .BackShopQuantity
                            iPSPQty = .BackShopPSPQuantity
                            If (iSalesFloorQty < 0) Then
                                iSalesFloorQty = 0
                            End If
                            If (iBackShopQty < 0) Then
                                iBackShopQty = 0
                            End If
                            If (iPSPQty < 0) Then
                                iPSPQty = 0
                            End If
                            If .SalesFloorQuantity < 0 And .BackShopQuantity < 0 And .BackShopPSPQuantity < 0 Then
                                .TotalQuantity = .SalesFloorQuantity + .BackShopQuantity + .BackShopPSPQuantity
                            Else
                                .TotalQuantity = iSalesFloorQty + iBackShopQty + iPSPQty
                            End If
                            .IsUnknownItem = True
                            arrCLProductInfo.Add(objArrayItem)
                        End With
                    End If
                Next
            Else
                Return False
            End If
            ' Next
            'Else
            ''If the count of objects in the array list is nor equal to the
            ''number of items retreived from the database return flase.
            'Return False
            'End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false in case of any exception.
            Return False
        Finally
            dsListItems.Dispose()
            strSqlCmd = Nothing
        End Try
        'If control reaches here after executing successfully, return true.
        Return True
    End Function

    ''' <summary>
    ''' Gets the site info for the Count list selected by the user.
    ''' Update details of each sites in an object in the array list.
    ''' </summary>
    ''' <param name="strListID">List ID of a count list</param>
    ''' <param name="arrPlannerList">Array of objects to update the site 
    ''' details</param>
    ''' <returns>Bool
    ''' True - If successfully recevied and updated the details in 
    ''' the object array.
    ''' False - If any occured during the updation.</returns>
    ''' <remarks></remarks>
    Public Function GetPlannerListDetails(ByVal strListID As String, ByRef arrPlannerList As ArrayList) As Boolean
        'Stock File Accuracy  - Added new function
        Dim objPlanners As New CLSessionMgr.CLMultiSiteInfo
        Dim strSqlCmd As String = QueryMacro.GET_COUNT_LIST_SITE_INFO
        Dim arrTempPlannerList As New ArrayList()
        Dim strSqlItemCountCmd As String
        strSqlCmd = String.Format(strSqlCmd, strListID)

        Dim dsList As DataSet = Nothing
        Dim dsItemCountList As DataSet = Nothing
        Try
            dsList = New DataSet()
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'Traverse through each element of the data set and update the planner
            'in the object.
            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objPlanners = New CLSessionMgr.CLMultiSiteInfo()
                    With objPlanners
                        .strPlannerDesc = dsList.Tables(0).Rows(iCount)("Site") _
                                                                .ToString()
                        '.iItemCount = dsList.Tables(0).Rows(iCount)("Items") _
                        '                                        .ToString()
                        .strSeqNumber = iCount + 1
                    End With
                    'Add the object with details to the array list.
                    arrTempPlannerList.Add(objPlanners)
                Next
                'Dim iComp As PogCompare = New PogCompare()
                'arrPlannerList.Sort(iComp)

                'Execute the second query to retrieve the item count in each site
                For Each objPlanner As CLSessionMgr.CLMultiSiteInfo In arrTempPlannerList
                    strSqlItemCountCmd = QueryMacro.GET_COUNT_LIST_SF_ITEMS
                    dsItemCountList = New DataSet()
                    strSqlItemCountCmd = String.Format(strSqlItemCountCmd, strListID, objPlanner.strPlannerDesc)
                    dsItemCountList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlItemCountCmd)
                    If dsItemCountList.Tables(0).Rows.Count > 0 Then
                        objPlanner.iItemCount = dsItemCountList.Tables(0).Rows.Count
                        arrPlannerList.Add(objPlanner)
                    Else
                        Return False
                    End If
                Next
            Else
                'return false if the dataset does not have any data.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false in case of any exception.
            Return False
        Finally
            dsList.Dispose()
            dsItemCountList.Dispose()
            strSqlItemCountCmd = Nothing
            strSqlCmd = Nothing
        End Try
        'If control reaches here after executing successfully, return true.
        Return True
    End Function
    ''' <summary>
    ''' Retrieves the multi site list
    ''' </summary>
    ''' <param name="strListID"></param>
    ''' <param name="strBootsCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMultiSiteInfo(ByVal strListID As String, ByVal strBootsCode As String, _
                                            ByRef arrPlannerList As ArrayList) As Boolean
        'Stock File Accuracy  - Added new function
        Dim strSqlCmd As String = QueryMacro.GET_COUNT_LIST_MULTISITE_LIST
        strSqlCmd = String.Format(strSqlCmd, strBootsCode, strListID)
        Dim dsList As DataSet = Nothing
        Dim objMultiSite As CLSessionMgr.CLMultiSiteInfo
        Dim strPlanogramDescription As String
        Dim strModuleDescription As String
        Try
            dsList = New DataSet()
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'Traverse through each element of the data set and update the planner
            'in the object.
            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objMultiSite = New CLSessionMgr.CLMultiSiteInfo()
                    With objMultiSite
                        strPlanogramDescription = dsList.Tables(0).Rows(iCount)("POG_Desc").ToString()
                        strModuleDescription = dsList.Tables(0).Rows(iCount)("Module_Desc").ToString()
                        .strPOGDescription = strPlanogramDescription.Trim() + " - " + strModuleDescription.Trim()
                        .IsCounted = "N"
                        '.strSalesFloorQuantiy = iSalesFloorQuantity
                        'Assign planogram description to strPlannerDesc which  uniquely identifies the planner
                        'in which items are present
                        .strPlannerDesc = dsList.Tables(0).Rows(iCount)("POG_Desc").ToString().Trim()
                        .strModuleDesc = dsList.Tables(0).Rows(iCount)("Module_Desc").ToString()
                        .strPlannerID = dsList.Tables(0).Rows(iCount)("Module_ID").ToString()
                        .iRepeatCount = dsList.Tables(0).Rows(iCount)("Repeat_Count").ToString()

                    End With
                    arrPlannerList.Add(objMultiSite)
                Next
            Else
                'return false if the dataset does not have any data.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false in case of any exception.
            Return False
        Finally
            dsList.Dispose()
            strSqlCmd = Nothing
        End Try
        'If control reaches here after executing successfully, return true.
        Return True
    End Function
    ''' <summary>
    ''' Retrieves the items present in each site selected
    ''' </summary>
    ''' <param name="strLocation">Location in which counting done</param>
    ''' <param name="strPlannerDesc">Planner ID</param>
    ''' <param name="strListId">Count list id</param>
    ''' <param name="arrBootsCodeList">array to store boots code in a site</param>
    ''' <returns>Bool
    ''' True - If successfully recevied and updated the details in 
    ''' the object array.
    ''' False - If any occured during the updation.</returns>
    ''' <remarks></remarks>
    Public Function GetBootsCodeItemList(ByVal strLocation As String, ByVal strPlannerDesc As String, _
                 ByVal strListId As String, ByRef arrBootsCodeList As ArrayList) As Boolean
        'Stock File Accuracy  -Added new function
        Dim strSqlCmd As String = Nothing
        If strLocation.Equals("MBS") Then
            strSqlCmd = QueryMacro.GET_COUNT_LIST_MBS_PRODUCTS
            strSqlCmd = String.Format(strSqlCmd, strListId)
        ElseIf strLocation.Equals("PSP") Then
            strSqlCmd = QueryMacro.GET_COUNT_LIST_PSP_PRODUCTS
            strSqlCmd = String.Format(strSqlCmd, strListId)
        ElseIf strLocation.Equals("SF") Then
            strSqlCmd = QueryMacro.GET_COUNT_LIST_SF_ITEMS
            strSqlCmd = String.Format(strSqlCmd, strListId, strPlannerDesc)
        End If

        Dim objProductInfo As CLProductInfo = Nothing
        Dim dsListItems As DataSet = Nothing
        Dim iItemCount As Integer
        'SFA  
        Dim strFirstBarCode As String = ""
        Dim strSecondBarCode As String = ""
        'end
        Try
            dsListItems = New DataSet()
            dsListItems = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'Update the properties int he object with the values retreived from
            'database.
            If dsListItems.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsListItems.Tables(0).Rows.Count - 1
                    objProductInfo = New CLProductInfo()
                    iItemCount = 0
                    With objProductInfo
                        .BootsCode = dsListItems.Tables(0) _
                                     .Rows(iCount)("Boots_Code").ToString()
                        iItemCount = dsListItems.Tables(0) _
                                     .Rows(iCount)("Count").ToString()
                        If iItemCount < 0 Then
                            .CountStatus = "N"
                        Else
                            .CountStatus = "Y"
                        End If
                    End With
                    arrBootsCodeList.Add(objProductInfo)
                Next
            Else
                'return false if the dataset does not have any data.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false in case of any exception.
            Return False
        Finally
            dsListItems.Dispose()
            strSqlCmd = Nothing
        End Try
        'If control reaches here after executing successfully, return true.
        Return True
    End Function
    ''' <summary>
    ''' 
    ''' Retrieve site count for item in one planner
    ''' </summary>
    ''' <param name="strPlannerDesc"></param>
    ''' <param name="strListId"></param>
    ''' <param name="arrBootsCodeList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetItemSiteCount(ByVal strPlannerDesc As String, _
                 ByVal strListId As String, ByRef arrBootsCodeList As ArrayList) As Boolean
        'Stock File Accuracy  -Added new function
        Dim strSqlCmd As String = Nothing
        Dim dsListItems As DataSet = Nothing
        Dim arrTempBootsCodeTable As New Hashtable()
        Dim iRepeatCount As Integer
        Dim objSiteInfo As CLSessionMgr.CLMultiSiteInfo = Nothing
        Dim arrSiteInfoList As ArrayList

        Try
            For Each objProductInfo As CLProductInfo In arrBootsCodeList

                strSqlCmd = QueryMacro.GET_COUNT_LIST_ITEM_SITE_COUNT
                strSqlCmd = String.Format(strSqlCmd, strListId, strPlannerDesc, objProductInfo.BootsCode)

                arrSiteInfoList = New ArrayList()
                dsListItems = New DataSet()
                dsListItems = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
                'Update the properties int he object with the values retreived from
                'database.
                If dsListItems.Tables(0).Rows.Count > 0 Then
                    For iCount As Integer = 0 To dsListItems.Tables(0).Rows.Count - 1
                        objSiteInfo = New CLSessionMgr.CLMultiSiteInfo()
                        With objSiteInfo
                            .strModuleID = dsListItems.Tables(0) _
                                         .Rows(iCount)("Module_ID").ToString()
                            .iRepeatCount = dsListItems.Tables(0) _
                                         .Rows(iCount)("Repeat_Count").ToString()
                        End With
                        arrSiteInfoList.Add(objSiteInfo)
                    Next
                    'Expand repeat count for each module in one planner to get the exact number of 
                    'sites for an item in one planner
                    Dim iSiteCount As Integer = 0
                    For Each objCurrentSiteInfo As CLSessionMgr.CLMultiSiteInfo In arrSiteInfoList
                        iRepeatCount = objCurrentSiteInfo.iRepeatCount
                        For counter As Integer = 1 To iRepeatCount
                            iSiteCount = iSiteCount + 1
                        Next
                    Next
                    objProductInfo.TotalSiteCount = iSiteCount
                Else
                    'return false if the dataset does not have any data.
                    Return False
                End If
            Next
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false in case of any exception.
            Return False
        Finally
            dsListItems.Dispose()
            strSqlCmd = Nothing
        End Try
        'If control reaches here after executing successfully, return true.
        Return True
    End Function
    ''' <summary>
    ''' Gets the item details required for a product in Shelf Monitor 
    ''' session.
    ''' </summary>
    ''' <param name="strBootsCode">Boots code of the item with check digit</param>
    ''' <param name="objSMProductInfo">Object to be updated with the value read.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the Boots code supplied.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingBC(ByVal strBootsCode As String, ByRef objSMProductInfo As SMProductInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_DETAILS_USING_BC
        strSqlCmd = String.Format(strSqlCmd, strBootsCode)
        'SFA
        Dim strFirstBarCode As String
        Dim strSecondBarCode As String
        ' end
        Dim sqlResultSet As SqlCeDataReader
        Try
            'execute the sql command and collect the result in the reader.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objSMProductInfo
                    .BootsCode = sqlResultSet("Boots_Code").ToString()
                    'SFA
                    strFirstBarCode = sqlResultSet("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarCode = sqlResultSet("Second_Barcode").ToString().PadLeft(12, "0")
                    .FirstBarcode = strFirstBarCode
                    'Compare teh first barcode and Boots code to get hte actual 
                    'barcode for the item.
                    If CheckBarcode(.BootsCode, strFirstBarCode, strSecondBarCode) Then
                        .ProductCode = strSecondBarCode
                    Else
                        .ProductCode = strFirstBarCode
                    End If
                    .SecondBarcode = strSecondBarCode
                    ' end
                    .Description = sqlResultSet("SEL_Desc").ToString()
                    .Status = sqlResultSet("Item_Status").ToString()
                    .TSF = sqlResultSet("SOD_TSF").ToString()
                    .ShortDescription = sqlResultSet("Item_Desc").ToString().Trim()
                End With
            Else
                'If the data adapter does not have the required fields or 
                'not able to read from the data adapter return false.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'clear the data adapter.
            sqlResultSet = Nothing
        End Try
        'if all the execution part is comepleted successfully, return true.
        Return True
    End Function
    ''' <summary>
    ''' Gets the item details required for a product in Auto Stuff Your Shelves
    ''' session.
    ''' </summary>
    ''' <param name="strBootsCode">Boots code of the item with check digit</param>
    ''' <param name="objASYSProductInfo">Object to be updated with the value read.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the Boots code supplied.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingBC(ByVal strBootsCode As String, ByRef objASYSProductInfo As ASYSProductInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_DETAILS_USING_BC
        Dim strSqlCmdForPSC As String = QueryMacro.GET_PSC
        Dim iPSC As Integer = 0
        strSqlCmd = String.Format(strSqlCmd, strBootsCode)
        'SFA
        Dim strFirstBarCode As String
        Dim strSecondBarCode As String
        Dim sqlResultSet As SqlCeDataReader

        Try
            'execute the sql command and collect the result in the reader.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)

            ''process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objASYSProductInfo
                    .BootsCode = sqlResultSet("Boots_Code").ToString()
                    'SFA
                    strFirstBarCode = sqlResultSet("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarCode = sqlResultSet("Second_Barcode").ToString().PadLeft(12, "0")
                    .FirstBarcode = strFirstBarCode
                    'Compare the first barcode and Boots code to get hte actual 
                    'barcode for the item.
                    If CheckBarcode(.BootsCode, strFirstBarCode, strSecondBarCode) Then
                        .ProductCode = strSecondBarCode
                    Else
                        .ProductCode = strFirstBarCode
                    End If
                    'SFA
                    .Description = sqlResultSet("SEL_Desc").ToString()
                    .Status = sqlResultSet("Item_Status").ToString()
                    .TSF = sqlResultSet("SOD_TSF").ToString()
                    .ShortDescription = sqlResultSet("Item_Desc").ToString().Trim()

                    'fix for SYS performance
                    Dim arrPlannerDetails As New ArrayList
                    GetPlannerListUsingBC(strBootsCode, True, arrPlannerDetails)
                    If arrPlannerDetails.Count >= 1 Then
                        objASYSProductInfo.MutiSite = arrPlannerDetails.Count.ToString()
                        objASYSProductInfo.MutiSite = arrPlannerDetails.Count
                        For Each objPlanners As PlannerInfo In arrPlannerDetails
                            iPSC = iPSC + (CInt(objPlanners.PhysicalShelfQty) * CInt(objPlanners.RepeatCount))
                        Next
                    Else
                        Return False
                    End If
                End With
                objASYSProductInfo.PSC = iPSC.ToString()
                If objASYSProductInfo.PSC = "" Then
                    Return False
                End If
            Else
                'If the data adapter does not have the required fields or 
                'not able to read from the data adapter return false.
                Return False
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'clear the data adapter.
            sqlResultSet = Nothing

        End Try
        'if all the execution part is comepleted successfully, return true.
        Return True
    End Function
    ''' <summary>
    ''' Gets the item details required for a product in Fast Fill 
    ''' session.
    ''' </summary>
    ''' <param name="strBootsCode">Boots code of the item with check digit</param>
    ''' <param name="objFFProductInfo">Object to be updated.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the Boots code supplied.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingBC(ByVal strBootsCode As String, ByRef objFFProductInfo As FFProductInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_DETAILS_USING_BC
        strSqlCmd = String.Format(strSqlCmd, strBootsCode)
        'SFA
        Dim strFirstBarCode As String
        Dim strSecondBarCode As String
        Dim sqlResultSet As SqlCeDataReader
        Try
            'execute the query
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objFFProductInfo
                    .BootsCode = sqlResultSet("Boots_Code").ToString()
                    'SFA
                    strFirstBarCode = sqlResultSet("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarCode = sqlResultSet("Second_Barcode").ToString().PadLeft(12, "0")
                    .FirstBarcode = strFirstBarCode
                    'Compare Boots code and the first barcode to get the real
                    'barcode of the item.
                    If CheckBarcode(.BootsCode, strFirstBarCode, strSecondBarCode) Then
                        .ProductCode = strSecondBarCode
                    Else
                        .ProductCode = strFirstBarCode
                    End If
                    ' end
                    .Description = sqlResultSet("SEL_Desc").ToString()
                    .Status = sqlResultSet("Item_Status").ToString()
                    .TSF = sqlResultSet("SOD_TSF").ToString()
                    .ShortDescription = sqlResultSet("Item_Desc").ToString().Trim()
                End With
            Else
                'if any error occurs in reading the data adapter return false.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            sqlResultSet = Nothing
        End Try
        'if successfully updated the details
        Return True
    End Function
    ''' <summary>
    ''' Gets the item details required for a product in Excess Stock 
    ''' session.
    ''' </summary>
    ''' <param name="strBootsCode">Boots code with check digit</param>
    ''' <param name="objEXProductInfo">Object to be updated.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the Boots Code supplied.
    ''' </returns>
    ''' <modified>Added Pending Sales Plan Flag as part of SFA</modified><Date>25-Oct-2011</Date>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingBC(ByVal strBootsCode As String, ByRef objEXProductInfo As EXProductInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_DETAILS_USING_BC
        strSqlCmd = String.Format(strSqlCmd, strBootsCode)
        'SFA
        Dim strFirstBarCode As String
        Dim strSecondBarCode As String
        Dim strPSPFlag As String
        Dim sqlResultSet As SqlCeDataReader
        Try
            'execute the query
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objEXProductInfo
                    .BootsCode = sqlResultSet("Boots_Code").ToString()
                    'SFA
                    strFirstBarCode = sqlResultSet("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarCode = sqlResultSet("Second_Barcode").ToString().PadLeft(12, "0")
                    'Comapre the first barcode and Boots code to get the actual
                    'barcode of the item.
                    If CheckBarcode(.BootsCode, strFirstBarCode, strSecondBarCode) Then
                        .ProductCode = strSecondBarCode
                    Else
                        .ProductCode = strFirstBarCode
                    End If
                    .SecondBarcode = strSecondBarCode
                    ' end
                    .Description = sqlResultSet("SEL_Desc").ToString()
                    .Status = sqlResultSet("Item_Status").ToString()
                    .TSF = sqlResultSet("SOD_TSF").ToString()
                    .ShortDescription = sqlResultSet("Item_Desc").ToString().Trim()
                    strPSPFlag = sqlResultSet("PSP_Flag").ToString()
                    If strPSPFlag = "Y" Or strPSPFlag = "y" Then
                        .PendingSalesFlag = True
                    Else
                        .PendingSalesFlag = False
                    End If

                End With
            Else
                'If any error occured in reading the data adapter then return false.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            sqlResultSet = Nothing
        End Try
        'if successfully updated the details
        Return True
    End Function
    ''' <summary>
    ''' Gets the item details required for a product in Print SEL module.
    ''' </summary>
    ''' <param name="strBootsCode">Boots code with check digit</param>
    ''' <param name="objPSProductInfo">Object to be updated.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the Boots Code supplied.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingBC(ByVal strBootsCode As String, ByRef objPSProductInfo As PSProductInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_ITEM_DETAILS_ALL_USING_BC
        'SFA
        Dim strFirstBarcode As String = ""
        Dim strSecondBarcode As String = ""

        'Fix: System Testing
        'Passing StrBootsCode to the query.
        strSqlCmd = String.Format(strSqlCmd, strBootsCode)
        Dim sqlResultSet As SqlCeDataReader
        Try
            'Execute the query and get the result set.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objPSProductInfo
                    'SFA
                    strFirstBarcode = sqlResultSet("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarcode = sqlResultSet("Second_Barcode").ToString().PadLeft(12, "0")
                    '.FirstBarcode = strBarcode
                    If CheckBarcode(sqlResultSet("Boots_Code").ToString(), _
                                   strFirstBarcode, strSecondBarcode) Then
                        Return GetProductInfoUsingPC(strSecondBarcode, objPSProductInfo)
                    Else
                        Return GetProductInfoUsingPC(strFirstBarcode, objPSProductInfo)
                    End If
                    'end
                End With
            Else
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'dispose the memory used by the data reader.
            sqlResultSet = Nothing
        End Try
        'if successfully updated the details.
        Return True
    End Function
    ''' <summary>
    ''' Gets the item details required for a product in Shelf Monitor 
    ''' session.
    ''' </summary>
    ''' <param name="strProductCode">Boots code without check digit.</param>
    ''' <param name="objSMProductInfo">Object to be updated.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the Boots code supplied.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, ByRef objSMProductInfo As SMProductInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_DETAILS_USING_PC
        'SFA Added Val(strProductCode)
        strSqlCmd = String.Format(strSqlCmd, Val(strProductCode))
        Dim sqlResultSet As SqlCeDataReader
        Dim strFirstBarCode As String = ""
        Dim strSecondBarCode As String = ""
        'end
        Try
            'Execute the query using the sql db connection.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'Process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objSMProductInfo
                    .BootsCode = sqlResultSet("Boots_Code").ToString()
                    'SFA modified
                    strFirstBarCode = sqlResultSet("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarCode = sqlResultSet("Second_Barcode").ToString().PadLeft(12, "0")
                    If sqlResultSet("First_Barcode").ToString().Equals(Val(strProductCode).ToString()) Then
                        .FirstBarcode = strSecondBarCode
                    Else
                        .FirstBarcode = strFirstBarCode
                    End If
                    .ProductCode = strProductCode.PadLeft(12, "0")
                    'end
                    .Description = sqlResultSet("SEL_Desc").ToString()
                    .Status = sqlResultSet("Item_Status").ToString()
                    .TSF = sqlResultSet("SOD_TSF").ToString()
                    .ShortDescription = sqlResultSet("Item_Desc").ToString().Trim()
                End With
            Else
                'If any error in reading the data reader.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'dispose the data reader.
            sqlResultSet = Nothing
        End Try
        'if successfully updated the details.
        Return True
    End Function
#Region "Create Own List"
    ''' <summary>
    ''' Gets the item details required for a product in Create Own List 
    ''' session.
    ''' </summary>
    ''' <param name="strProductCode">Boots code without check digit.</param>
    ''' <param name="objCLProductInfo">Object to be updated.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the Boots code supplied.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, ByRef objCLProductInfo As CLProductInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_DETAILS_USING_PC
        ' Added Val(strProductCode)
        strSqlCmd = String.Format(strSqlCmd, Val(strProductCode))
        Dim sqlResultSet As SqlCeDataReader
        Dim strPSPFlag As String
        'SFA 
        Dim strFirstBarCode As String = ""
        Dim strSecondBarCode As String = ""
        'end
        Try
            'Execute the query using the sql db connection.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'Process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objCLProductInfo
                    .BootsCode = sqlResultSet("Boots_Code").ToString()
                    'SFA modified
                    strFirstBarCode = sqlResultSet("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarCode = sqlResultSet("Second_Barcode").ToString().PadLeft(12, "0")
                    If sqlResultSet("First_Barcode").ToString().Equals(Val(strProductCode).ToString()) Then
                        .FirstBarcode = strSecondBarCode
                    Else
                        .FirstBarcode = strFirstBarCode
                    End If
                    .ProductCode = strProductCode.PadLeft(12, "0")
                    ' end
                    .Description = sqlResultSet("SEL_Desc").ToString()
                    .Status = sqlResultSet("Item_Status").ToString()
                    .TSF = sqlResultSet("SOD_TSF").ToString()
                    .ShortDescription = sqlResultSet("Item_Desc").ToString().Trim()
                    .LastCountDate = sqlResultSet("Date_Last_Counted").ToString()
                    strPSPFlag = sqlResultSet("PSP_Flag").ToString()
                    .IsUnknownItem = False
                    If strPSPFlag = "Y" Or strPSPFlag = "y" Then
                        .PendingSalesFlag = True
                    Else
                        .PendingSalesFlag = False
                    End If
                End With
            Else
                'If any error in reading the data reader.

                With objCLProductInfo
                    .BootsCode = strProductCode
                    .FirstBarcode = strProductCode
                    .ProductCode = strProductCode
                    .Description = "Unknown Item   Check from the Controller"
                    .Status = "N/A"
                    .TSF = "N/A"
                    .ShortDescription = "Unknown Item"
                    .LastCountDate = "01/01/1900"
                    .PendingSalesFlag = False
                    .IsUnknownItem = True
                    .IsSEL = False
                End With
                Return False
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'dispose the data reader.
            sqlResultSet = Nothing
        End Try
        'if successfully updated the details.
        Return True
    End Function
    ''' <summary>
    ''' Gets the item details required for a product in Create Own List
    ''' session.
    ''' </summary>
    ''' <param name="strBootsCode">Boots code of the item with check digit</param>
    ''' <param name="objCLProductInfo">Object to be updated with the value read.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the Boots code supplied.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingBC(ByVal strBootsCode As String, ByRef objCLProductInfo As CLProductInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_DETAILS_USING_BC
        strSqlCmd = String.Format(strSqlCmd, strBootsCode)
        'SFA
        Dim strFirstBarCode As String
        Dim strSecondBarCode As String
        Dim strPSPFlag As String
        'end
        Dim sqlResultSet As SqlCeDataReader
        Try
            'execute the sql command and collect the result in the reader.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objCLProductInfo
                    .BootsCode = sqlResultSet("Boots_Code").ToString()
                    'SFA
                    strFirstBarCode = sqlResultSet("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarCode = sqlResultSet("Second_Barcode").ToString().PadLeft(12, "0")
                    .FirstBarcode = strFirstBarCode
                    'Compare teh first barcode and Boots code to get hte actual 
                    'barcode for the item.
                    If CheckBarcode(.BootsCode, strFirstBarCode, strSecondBarCode) Then
                        .ProductCode = strSecondBarCode
                    Else
                        .ProductCode = strFirstBarCode
                    End If
                    '.SecondBarcode = strSecondBarCode
                    ' end
                    .Description = sqlResultSet("SEL_Desc").ToString()
                    .Status = sqlResultSet("Item_Status").ToString()
                    .TSF = sqlResultSet("SOD_TSF").ToString()
                    .ShortDescription = sqlResultSet("Item_Desc").ToString().Trim()
                    .LastCountDate = sqlResultSet("Date_Last_Counted").ToString()
                    .IsUnknownItem = False
                    strPSPFlag = sqlResultSet("PSP_Flag").ToString()
                    If strPSPFlag = "Y" Or strPSPFlag = "y" Then
                        .PendingSalesFlag = True
                    Else
                        .PendingSalesFlag = False
                    End If
                End With
            Else
                'If the data adapter does not have the required fields or 
                'not able to read from the data adapter return false.
                Dim strProdCode As String = ""
                With objCLProductInfo
                    .BootsCode = strBootsCode
                    strProdCode = strBootsCode.Substring(0, 6).PadLeft(12, "0")
                    .FirstBarcode = strProdCode
                    .ProductCode = strProdCode
                    .Description = "Unknown Item   Check from the Controller"
                    .Status = "N/A"
                    .TSF = "N/A"
                    .ShortDescription = "Unknown Item"
                    .LastCountDate = "01/01/1900"
                    .PendingSalesFlag = False
                    .IsUnknownItem = True
                    .IsSEL = True
                End With
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'clear the data adapter.
            sqlResultSet = Nothing
        End Try
        'if all the execution part is comepleted successfully, return true.
        Return True
    End Function
#End Region
    ''' <summary>
    ''' Gets the item details required for a product in Auto Stuff Your Shelves
    ''' session.
    ''' </summary>
    ''' <param name="strProductCode">Boots code without check digit.</param>
    ''' <param name="objASYSProductInfo">Object to be updated.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the Boots code supplied.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, ByRef objASYSProductInfo As ASYSProductInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_DETAILS_USING_PC
        Dim strSqlCmdForPSC As String = QueryMacro.GET_PSC
        'SFA Added val(strproductcode)
        strSqlCmd = String.Format(strSqlCmd, Val(strProductCode))
        Dim sqlResultSet As SqlCeDataReader
        Dim strModuleLists As String = ""
        Dim iPSC As Integer = 0
        'Dim strLivePOG As String
        'Dim iIndex As Integer = 1

        Dim strFirstBarCode As String = ""
        Dim strSecondBarCode As String = ""
        'end
        Try
            'Execute the query using the sql db connection.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'Process dataset and retreive the details in it.
            If sqlResultSet.Read Then

                With objASYSProductInfo
                    .BootsCode = sqlResultSet("Boots_Code").ToString()
                    'SFA modified
                    strFirstBarCode = sqlResultSet("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarCode = sqlResultSet("Second_Barcode").ToString().PadLeft(12, "0")
                    If sqlResultSet("First_Barcode").ToString().Equals(Val(strProductCode).ToString()) Then
                        .FirstBarcode = strSecondBarCode
                    Else
                        .FirstBarcode = strFirstBarCode
                    End If
                    .ProductCode = strProductCode.PadLeft(12, "0")
                    'end
                    .Description = sqlResultSet("SEL_Desc").ToString()
                    .Status = sqlResultSet("Item_Status").ToString()
                    .TSF = sqlResultSet("SOD_TSF").ToString()
                    .ShortDescription = sqlResultSet("Item_Desc").ToString().Trim()

                    Dim arrPlannerDetails As New ArrayList
                    'fix for performance issue in SYS
                    GetPlannerListUsingPC(strProductCode, True, arrPlannerDetails)
                    If arrPlannerDetails.Count >= 1 Then
                        objASYSProductInfo.MutiSite = arrPlannerDetails.Count
                        For Each objPlanners As PlannerInfo In arrPlannerDetails
                            iPSC = iPSC + (CInt(objPlanners.PhysicalShelfQty) * CInt(objPlanners.RepeatCount))
                        Next
                    Else
                        Return False
                    End If
                End With
            Else
                'If any error in reading the data reader.
                Return False
            End If
            objASYSProductInfo.PSC = iPSC.ToString()
            If objASYSProductInfo.PSC = "" Then
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'dispose the data reader.
            sqlResultSet = Nothing

        End Try
        'if successfully updated the details.
        Return True
    End Function
    ''' <summary>
    ''' Gets the item details required for a product in Fast Fill 
    ''' session.
    ''' </summary>
    ''' <param name="strProductCode">Barcode without check digit</param>
    ''' <param name="objFFProductInfo">Object to be updated.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details available for the Product code supplied.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, ByRef objFFProductInfo As FFProductInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_DETAILS_USING_PC
        'SFA Added val
        strSqlCmd = String.Format(strSqlCmd, Val(strProductCode))
        Dim sqlResultSet As SqlCeDataReader
        Dim strFirstBarCode As String = ""
        Dim strSecondBarCode As String = ""
        ' end
        Try
            'Execute the query using the sql db connection.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objFFProductInfo
                    .BootsCode = sqlResultSet("Boots_Code").ToString()
                    'SFA modified
                    strFirstBarCode = sqlResultSet("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarCode = sqlResultSet("Second_Barcode").ToString().PadLeft(12, "0")
                    If sqlResultSet("First_Barcode").ToString().Equals(Val(strProductCode).ToString()) Then
                        .FirstBarcode = strSecondBarCode
                    Else
                        .FirstBarcode = strFirstBarCode
                    End If
                    .SecondBarcode = strSecondBarCode
                    .ProductCode = strProductCode.PadLeft(12, "0")
                    'end
                    .Description = sqlResultSet("SEL_Desc").ToString()
                    .Status = sqlResultSet("Item_Status").ToString()
                    .TSF = sqlResultSet("SOD_TSF").ToString()
                    .ShortDescription = sqlResultSet("Item_Desc").ToString().Trim()
                End With
            Else
                'If any error in reading the data reader
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            sqlResultSet = Nothing
        End Try
        'if successfully updated the details
        Return True
    End Function
    ''' <summary>
    ''' Gets the item details required for a product in Excess Stock
    ''' session.
    ''' </summary>
    ''' <param name="strProductCode">Product code without check digit</param>
    ''' <param name="objEXProductInfo">Object to update the details.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is not details available for the Product code supplied.
    ''' </returns>
    ''' <modified>Added Pending Sales Plan Flag as part of SFA</modified><Date>25-Oct-2011</Date>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, ByRef objEXProductInfo As EXProductInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_DETAILS_USING_PC
        'SFA Added Val
        strSqlCmd = String.Format(strSqlCmd, Val(strProductCode))
        Dim sqlResultSet As SqlCeDataReader
        Dim strPSPFlag As String
        Dim strFirstBarCode As String = ""
        Dim strSecondBarCode As String = ""
        ' end
        Try
            'execute the query
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objEXProductInfo
                    .BootsCode = sqlResultSet("Boots_Code").ToString()
                    'SFA modified
                    strFirstBarCode = sqlResultSet("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarCode = sqlResultSet("Second_Barcode").ToString().PadLeft(12, "0")
                    If sqlResultSet("First_Barcode").ToString().Equals(Val(strProductCode).ToString()) Then
                        .FirstBarcode = strSecondBarCode
                    Else
                        .FirstBarcode = strFirstBarCode
                    End If
                    .SecondBarcode = strSecondBarCode
                    .ProductCode = strProductCode.PadLeft(12, "0")
                    'end
                    .Description = sqlResultSet("SEL_Desc").ToString()
                    .Status = sqlResultSet("Item_Status").ToString()
                    .TSF = sqlResultSet("SOD_TSF").ToString()
                    .ShortDescription = sqlResultSet("Item_Desc").ToString().Trim()
                    strPSPFlag = sqlResultSet("PSP_Flag").ToString()
                    If strPSPFlag = "Y" Or strPSPFlag = "y" Then
                        .PendingSalesFlag = True
                    Else
                        .PendingSalesFlag = False
                    End If

                End With
            Else
                'If any error in reading the data reader.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'clear the data reader.
            sqlResultSet = Nothing
        End Try
        'if successfully updated the details.
        Return True
    End Function
    ''' <summary>
    ''' Gets the item details required for a product in Print SEL module.
    ''' </summary>
    ''' <param name="strProductCode">Product code without check digit</param>
    ''' <param name="objPSProductInfo">Object to be updated.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, ByRef objPSProductInfo As PSProductInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_SEL_PRINT_DETAILS
        ' Added Val(strproductcode)
        strSqlCmd = String.Format(strSqlCmd, Val(strProductCode))
        Dim sqlResultSet As SqlCeDataReader
        Dim strDealList As String = ""
        Try
            'Execute the query using the sql db connection.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objPSProductInfo
                    .BootsCode = sqlResultSet("Boots_Code").ToString()
                    'SFA - Added padleft
                    .ProductCode = strProductCode.PadLeft(12, "0")
                    .Description = sqlResultSet("SEL_Desc").ToString()
                    .ShortDescription = sqlResultSet("Item_Desc").ToString()
                    .Status = sqlResultSet("Item_Status").ToString()
                    .TSF = sqlResultSet("SOD_TSF").ToString()
                    .CurrentPrice = sqlResultSet("Current_Price").ToString()
                    .SupplyRoute = sqlResultSet("Supply_Route").ToString()
                    .UnitType = sqlResultSet("Unit_Name").ToString()
                    .UnitMeasure = sqlResultSet("Unit_Measure").ToString()
                    .UnitQuantity = sqlResultSet("Unit_Item_Quantity").ToString()
                    .WasPrice1 = sqlResultSet("PH1_Price").ToString()
                    .WasPrice2 = sqlResultSet("PH2_Price").ToString()
                    .CIPFlag = sqlResultSet("Markdown_Flag").ToString()
                    .SELLabelType = sqlResultSet("Label_Type").ToString()
                    .Advantage = objAppContainer.objHelper.GetRedeemableFlag(sqlResultSet("Item_Status3").ToString())
                    .WEEEFlag = sqlResultSet("Item_Status8").ToString()
                End With
            Else
                'If any error in reading the data reader.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'Clear the data reader variable.
            sqlResultSet = Nothing
        End Try
        'If successfully updated the details.
        Return True
    End Function
    ''' <summary>
    ''' Gets the item details required for a product in Item Info module.
    ''' </summary>
    ''' <param name="strBootsCode">Boots code with check digit</param>
    ''' <param name="objItemInfo">Object to update the details</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no details for the Boots code supplied.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetItemDetailsAllUsingBC(ByVal strBootsCode As String, ByRef objItemInfo As ItemInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_ITEM_DETAILS_ALL_USING_BC
        strSqlCmd = String.Format(strSqlCmd, strBootsCode)
        Dim sqlResultSet As SqlCeDataReader
        'SFA 
        Dim strFirstBarCode As String = ""
        Dim strSecondBarCode As String = ""
        'end
        Try
            'Execute the query and get the result set.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objItemInfo
                    'SFA modified
                    strFirstBarCode = sqlResultSet("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarCode = sqlResultSet("Second_Barcode").ToString().PadLeft(12, "0")
                    .FirstBarcode = strFirstBarCode
                    If CheckBarcode(sqlResultSet("Boots_Code").ToString(), _
                                   strFirstBarCode, strSecondBarCode) Then
                        Return GetItemDetailsAllUsingPC(strSecondBarCode, objItemInfo)
                    Else
                        Return GetItemDetailsAllUsingPC(strFirstBarCode, objItemInfo)
                    End If
                    ' end
                End With
            Else
                'If any error in reading the data reader.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            sqlResultSet = Nothing
        End Try
        'If successfully updated the details.
        Return True
    End Function
    ''' <summary>
    ''' Gets the item details required for a product in Item Info module.
    ''' </summary>
    ''' <param name="strProductCode">Product code without check digit</param>
    ''' <param name="objItemInfo">Object to be updated.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' no details available for the product.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetItemDetailsAllUsingPC(ByVal strProductCode As String, ByRef objItemInfo As ItemInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_ITEM_DETAILS_ALL_USING_PC
        'SFA added Val
        strSqlCmd = String.Format(strSqlCmd, Val(strProductCode))
        Dim sqlResultSet As SqlCeDataReader
        Dim strDealList As String = ""
        Dim strFirstBarCode As String = ""
        Dim strSecondBarCode As String = ""
        ' end
        Try
            'Execute the query using the sql db connection.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objItemInfo
                    .BootsCode = sqlResultSet("Boots_Code").ToString()
                    'SFA modified
                    strFirstBarCode = sqlResultSet("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarCode = sqlResultSet("Second_Barcode").ToString().PadLeft(12, "0")
                    'SFA added val
                    If sqlResultSet("First_Barcode").ToString().Equals(Val(strProductCode).ToString()) Then
                        .FirstBarcode = strSecondBarCode
                    Else
                        .FirstBarcode = strFirstBarCode
                    End If
                    .ProductCode = strProductCode.PadLeft(12, "0")
                    'end
                    .Description = sqlResultSet("SEL_Desc").ToString()
                    .Status = sqlResultSet("Item_Status").ToString()
                    .TSF = sqlResultSet("SOD_TSF").ToString()
                    .Price = sqlResultSet("Current_Price").ToString()
                    .RedemptionFlag = objAppContainer.objHelper.GetRedeemableFlag(sqlResultSet("Item_Status3").ToString())
                    .Advantage = .RedemptionFlag
                    'Get the list of deal numbers for the item.
                    'If the items has more than one deal number associated with 
                    'it, the deal numbers will be appended using a comma.
                    If Not sqlResultSet("Deal_No1").ToString().Equals("0000") Then
                        strDealList = sqlResultSet("Deal_No1").ToString()
                    End If
                    If Not sqlResultSet("Deal_No2").ToString().Equals("0000") Then
                        strDealList = strDealList + "," + _
                                    sqlResultSet("Deal_No2").ToString()
                    End If
                    If Not sqlResultSet("Deal_No3").ToString().Equals("0000") Then
                        strDealList = strDealList + "," + _
                                    sqlResultSet("Deal_No3").ToString()
                    End If
                    If Not sqlResultSet("Deal_No4").ToString().Equals("0000") Then
                        strDealList = strDealList + "," + _
                                    sqlResultSet("Deal_No4").ToString()
                    End If
                    If Not sqlResultSet("Deal_No5").ToString().Equals("0000") Then
                        strDealList = strDealList + "," + _
                                    sqlResultSet("Deal_No5").ToString()
                    End If
                    If Not sqlResultSet("Deal_No6").ToString().Equals("0000") Then
                        strDealList = strDealList + "," + _
                                    sqlResultSet("Deal_No6").ToString()
                    End If
                    If Not sqlResultSet("Deal_No7").ToString().Equals("0000") Then
                        strDealList = strDealList + "," + _
                                    sqlResultSet("Deal_No7").ToString()
                    End If
                    If Not sqlResultSet("Deal_No8").ToString().Equals("0000") Then
                        strDealList = strDealList + "," + _
                                    sqlResultSet("Deal_No8").ToString()
                    End If
                    If Not sqlResultSet("Deal_No9").ToString().Equals("0000") Then
                        strDealList = strDealList + "," + _
                                    sqlResultSet("Deal_No9").ToString()
                    End If
                    If Not sqlResultSet("Deal_No10").ToString().Equals("0000") Then
                        strDealList = strDealList + "," + _
                                    sqlResultSet("Deal_No10").ToString()
                    End If
                    .DealList = strDealList.Trim()
                End With
            Else
                'If any error in reading the data reader.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'Clear the data reader variable.
            sqlResultSet = Nothing
        End Try
        'If successfully updated the details.
        Return True
    End Function
    Public Function GetLabelDetailsUsingBC(ByVal strBarcode As String, ByVal objLabelInfo As PSProductInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_ITEM_DETAILS_ALL_USING_BC
        strSqlCmd = String.Format(strSqlCmd, strBarcode)
        Dim sqlResultSet As SqlCeDataReader
        'SFA
        Dim strSecondBarCode As String = ""
        'end
        'Execute the query and get the result set.
        sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
        'process dataset and retreive the details in it.
        If sqlResultSet.Read Then
            With objLabelInfo
                strBarcode = sqlResultSet("First_Barcode").ToString().PadLeft(12, "0") 'Minu padleft
                'SFA
                strSecondBarCode = sqlResultSet("Second_Barcode").ToString().PadLeft(12, "0")
                '.FirstBarcode = strBarcode
                If CheckBarcode(sqlResultSet("Boots_Code").ToString(), _
                               strBarcode, strSecondBarCode) Then
                    Return GetLabelDetailsUsingPC( _
                                        strSecondBarCode, objLabelInfo)
                    'end
                Else
                    Return GetLabelDetailsUsingPC(strBarcode, objLabelInfo)
                End If
            End With
        Else
            'If any error in reading the data reader.
            Return False
        End If
    End Function
    Public Function GetLabelDetailsUsingPC(ByVal strProductCode As String, ByRef objItemInfo As PSProductInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_SEL_PRINT_DETAILS
        'SFA added val
        strSqlCmd = String.Format(strSqlCmd, Val(strProductCode))
        Dim sqlResultSet As SqlCeDataReader
        Dim strDealList As String = ""
        Try
            'Execute the query using the sql db connection.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objItemInfo
                    .BootsCode = sqlResultSet("Boots_Code").ToString()
                    'SFA - Added padleft
                    .ProductCode = strProductCode.PadLeft(12, "0")
                    .Description = sqlResultSet("SEL_Desc").ToString()
                    .ShortDescription = sqlResultSet("Item_Desc").ToString()
                    .Status = sqlResultSet("Item_Status").ToString()
                    .TSF = sqlResultSet("SOD_TSF").ToString()
                    .CurrentPrice = sqlResultSet("Current_Price").ToString()
                    .SupplyRoute = sqlResultSet("Supply_Route").ToString()
                    .UnitType = sqlResultSet("Unit_Name").ToString()
                    .UnitMeasure = sqlResultSet("Unit_Measure").ToString()
                    .UnitQuantity = sqlResultSet("Unit_Item_Quantity").ToString()
                    .WasPrice1 = sqlResultSet("PH1_Price").ToString()
                    .WasPrice2 = sqlResultSet("PH2_Price").ToString()
                    .CIPFlag = sqlResultSet("Markdown_Flag").ToString()
                End With
            Else
                'If any error in reading the data reader.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'Clear the data reader variable.
            sqlResultSet = Nothing
        End Try
        'If successfully updated the details.
        Return True
    End Function
    ''' <summary>
    ''' Check if the product scanned is a valid product using Boots Code.
    ''' </summary>
    ''' <param name="strBootsCode">Boots code with check digit.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' Boots code supplied is not valid.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function CheckIsProductValidUsingBC(ByVal strBootsCode As String) As Boolean
        Dim strSqlCmd As String = QueryMacro.VALIDATE_PRODUCT_USING_BC
        strSqlCmd = String.Format(strSqlCmd, strBootsCode)
        Dim sqlResultSet As SqlCeDataReader
        Try
            'execute the query
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read And _
                sqlResultSet("Boots_Code").ToString().Equals(strBootsCode) Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            sqlResultSet = Nothing
        End Try
    End Function
    ''' <summary>
    ''' Check if the product scanned is a valid product using Product Code.
    ''' </summary>
    ''' <param name="strProductCode">Product code without check digit</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or
    ''' Product code supplied is not valid.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function CheckIsProductValidUsingPC(ByVal strProductCode As String) As Boolean
        Dim strSqlCmd As String = QueryMacro.VALIDATE_PRODUCT_USING_PC
        strSqlCmd = String.Format(strSqlCmd, strProductCode)
        Dim sqlResultSet As SqlCeDataReader
        Try
            'execute the query
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read And _
                sqlResultSet("BarCode").ToString().Equals(strProductCode) Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'Clear the data reader variable.
            sqlResultSet = Nothing
        End Try
    End Function
    ''' <summary>
    ''' Gets the item details required for a product in Price Check module
    ''' </summary>
    ''' <param name="strProductCode">Product code without check digit.</param>
    ''' <param name="objPCProductInfo">Object to be updated.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or
    ''' there is no price check information is present in the database.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetPriceCheckInfo(ByVal strBootsCode As String, ByVal strProductCode As String, ByRef objPCProductInfo As PriceCheckInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_PRICE_CHECK_DETAILS
        strSqlCmd = String.Format(strSqlCmd, Val(strProductCode), strBootsCode) 'SFA added val
        Dim sqlResultSet As SqlCeDataReader = Nothing
        Try
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            'Added padleft to current price to mekae it 8 digit
            If sqlResultSet.Read Then
                With objPCProductInfo
                    .CurrentPrice = sqlResultSet("Current_Price").ToString().PadLeft(8, "0")
                    .LastPriceCheckDate = sqlResultSet("Last_Price_Check_Date").ToString()
                    .PendingPrice = sqlResultSet("Pending_Price").ToString()
                    .SELPrintFlag = sqlResultSet("Product_Grp_Flag").ToString()
                    .PendingPriceDate = sqlResultSet("Pending_Price_Date") '@Service Fix
                End With
            Else
                'If any error occured during the data reader read
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'Clear up data reader.
            sqlResultSet = Nothing
        End Try
        'If successfully updated the details.
        Return True
    End Function
    ''' <summary>
    ''' Get details of Price Check targets completed, target set etc.,
    ''' </summary>
    ''' <param name="objPCTargetDetails">Object to be updated with details.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or 
    ''' there is no price check details present in the database.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetPCTargetDetails(ByRef objPCTargetDetails As PCTargetDetails) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_PC_TARGET_DETAILS
        Dim sqlResultSet As SqlCeDataReader
        Try
            'Execute the query using the sql db connection.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objPCTargetDetails
                    .PriceCheckTarget = sqlResultSet("PC_Weekly_Target").ToString()
                    .PriceCheckCompleted = sqlResultSet _
                                           ("PC_Count_Current_Week").ToString()
                    .PCThreshold = sqlResultSet("PC_Date_Threshold").ToString()
                End With
            Else
                'If error in reading the data reader
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            sqlResultSet = Nothing
        End Try
        'If successfully updated the details.
        Return True
    End Function
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
    Public Function GetUserDetails(ByVal strUserID As String, ByRef objUserInfo As UserInfo) As Boolean
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
                    .StockSpecialist = sqlResultSet("Stock_Adjustment_Flag").ToString()
                End With
            Else
                'If error occured while reading the data reader.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
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
    ''' Gets the deal details for the deal number passed as argument.
    ''' </summary>
    ''' <param name="strDealNo">Deal Number</param>
    ''' <param name="objDealDetails">Object to hold the deal details.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Public Function GetDealDetails(ByVal strDealNo As String, ByRef objDealDetails As DQRRECORD) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_DEAL_DETAILS

        Dim dtStartDate As DateTime = Nothing
        Dim dtEndDate As DateTime = Nothing

        strSqlCmd = String.Format(strSqlCmd, strDealNo)
        Dim sqlResultSet As SqlCeDataReader
        Try
            'execute the command to get the user details.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read() Then
                objDealDetails.strDealType = sqlResultSet("Deal_Type").ToString()                 
                objDealDetails.strStartDate = sqlResultSet("Start_Date").ToString()
                objDealDetails.strEndDate = sqlResultSet("End_Date").ToString()

                dtStartDate = DateTime.Parse(objDealDetails.strStartDate)
                dtEndDate = DateTime.Parse(objDealDetails.strEndDate)

                objDealDetails.strStartDate = dtStartDate.Year.ToString() _
                                              + dtStartDate.Month.ToString().PadLeft(2, "0"c) _
                                              + dtStartDate.Day.ToString().PadLeft(2, "0"c)
                objDealDetails.strEndDate = dtEndDate.Year.ToString() _
                                            + dtEndDate.Month.ToString().PadLeft(2, "0"c) _
                                            + dtEndDate.Day.ToString().PadLeft(2, "0"c)
            Else
                'If error in reading the data reader.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            sqlResultSet = Nothing
        End Try
        'If successfully updated the details.
        Return True
    End Function
    ''' <summary>
    ''' Gets the list of all picking list available in the active tables.
    ''' </summary>
    ''' <param name="arrObjectList">Reference to the array for collecting 
    ''' Picking list object for each picking list present in the local database
    ''' </param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Public Function GetPickingList(ByRef arrObjectList As ArrayList) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_PICKING_LIST
        Dim dsList As DataSet = Nothing
        Dim objPickingList As PickingList
        Dim TempArrayList As ArrayList = New ArrayList
        Dim ElementList As Array = "S,E,A,F,N".Split(",")
        Dim arrTempObjectList As ArrayList = New ArrayList

        Try
            dsList = New DataSet()
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'Traverse through each element of the data set and update the details
            'in the object.
            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objPickingList = New PickingList()
                    With objPickingList
                        .ListTime = dsList.Tables(0).Rows(iCount)("List_Time").ToString()
                        .ListType = dsList.Tables(0).Rows(iCount)("List_Type").ToString()
                        .ListID = dsList.Tables(0).Rows(iCount)("List_ID").ToString()
                        .Creator = dsList.Tables(0).Rows(iCount)("User_Name").ToString()
                        .UserID = dsList.Tables(0).Rows(iCount)("Creator_ID").ToString()
                        .TotalItems = dsList.Tables(0).Rows(iCount)("Number_Of_Items") _
                                      .ToString()
                        .ListStatus = dsList.Tables(0).Rows(iCount)("List_Status") _
                                      .ToString()
                    End With
                    'Add the object with details to the array list.
                    TempArrayList.Add(objPickingList)
                Next
                For Each PickListItem As String In ElementList
                    For Each PickingListObject As PickingList In TempArrayList
                        If PickingListObject.ListType = PickListItem Then
                            'arrObjectList.Add(PickingListObject)
                            'TempArrayList.Remove(PickingListObject)
                            PickingListObject.ListTime = PickingListObject.ListTime.PadLeft(4, "0")
                            arrTempObjectList.Add(PickingListObject)
                        End If
                    Next
                    arrTempObjectList.Sort()
                    arrTempObjectList.Reverse()
                    For Each PickingListObject As PickingList In arrTempObjectList
                        arrObjectList.Add(PickingListObject)
                    Next
                    arrTempObjectList.Clear()
                Next
            Else
                'return false if the dataset does not have any data.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            'return false in case of any exception.
            Return False
        Finally
            dsList.Dispose()
        End Try
        'If control reaches here after executing the successfully return true.
        Return True
    End Function
    ''' <summary>
    ''' Gets details for all the items in a picking list referred by ListID.
    ''' </summary>
    ''' <param name="strListID">Picking List ID</param>
    ''' <param name="arrPickingList">Array list containing object</param>
    ''' <param name="strListType">Picking List type</param>
    ''' <returns>Bool
    ''' True - If successfully updated the details in the object.
    ''' False - If any error occurred during the transaction.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetProductInfo(ByVal strListID As String, ByRef arrPickingList As ArrayList, ByVal strListType As String) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_PICKING_LIST_DETAILS
        strSqlCmd = String.Format(strSqlCmd, strListID)
        Dim dsProdInfo As DataSet = Nothing
        Dim bStatus As Boolean = Nothing
        Try
            dsProdInfo = New DataSet()
            dsProdInfo = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'Check number of rows in the data set.
            If dsProdInfo.Tables(0).Rows.Count > 0 Then
                Select Case strListType
                    Case Macros.SHELF_MONITOR_PL
                        'update the picking list details for SM type.
                        bStatus = UpdateSMPickingList(arrPickingList, dsProdInfo)
                    Case Macros.FAST_FILL_PL
                        'update the picking list details for FF type.
                        bStatus = UpdateFFPickingList(arrPickingList, dsProdInfo)
                    Case Macros.AUTO_FAST_FILL_PL
                        'update the picking list details for FF type.
                        bStatus = UpdateAFFPickingList(arrPickingList, dsProdInfo)
                    Case Macros.EXCESS_STOCK_PL
                        'update the picking list details for EX type.
                        bStatus = UpdateEXPickingList(arrPickingList, dsProdInfo)
                    Case Macros.EXCESS_STOCK_PL_SF
                        'update the picking list details for EX type.
                        bStatus = UpdateEXPickingList(arrPickingList, dsProdInfo)
                End Select
            Else
                'If there is no result in the data set
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False
        Finally
            'Dispose the dataset object.
            dsProdInfo.Dispose()
        End Try
        'If successfully updated the details.
        Return bStatus
    End Function
    ''' <summary>
    ''' Gets the list of Categories available.
    ''' </summary>
    ''' <param name="strCore">Core or Non Core</param>
    ''' <param name="arrCatergoryList">Array list to hold objects.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Public Function GetPOGCategoryList(ByVal strCore As String, ByRef arrCatergoryList As ArrayList) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_CATEGORY_LIST
        Dim dsList As DataSet = Nothing
        Dim objCategory As CategoryInfo
        'Format the query to substitute the core type.
        strSqlCmd = String.Format(strSqlCmd, strCore)
        Try
            dsList = New DataSet()
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'Traverse through each element of the data set and update the details
            'in the object.
            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objCategory = New CategoryInfo()
                    With objCategory
                        .CategoryID = dsList.Tables(0).Rows(iCount)("Category_ID") _
                                                                .ToString()
                        .Description = dsList.Tables(0).Rows(iCount)("Category_Desc") _
                                                                .ToString()
                    End With
                    'Filter objects with description "Core", "SalesPlan" or "Boots"
                    If Trim(objCategory.Description) <> "Boots" And Trim(objCategory.Description) <> "Sales Plan" And _
                    Trim(objCategory.Description) <> "Core" Then
                        'Add the object with details to the array list.
                        arrCatergoryList.Add(objCategory)
                    End If
                Next
            Else
                'return false if the dataset does not have any data.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            'return false in case of any exception.
            Return False
        Finally
            'Dispose the data set object.
            dsList.Dispose()
        End Try
        'If executed successfully, return true.
        Return True
    End Function
    ''' <summary>
    ''' Gets the Planner list where the scanned product is present using 
    ''' Boots Code.
    ''' </summary>
    ''' <param name="strBootsCode">Boots code with check digit</param>
    ''' <param name="arrPlannerList">Array list to hold the objects.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Public Function GetPlannerListUsingBC(ByVal strBootsCode As String, ByVal bIsMultisiteCall As Boolean, ByRef arrPlannerList As ArrayList) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_PLANNER_LIST_USING_BC
        Dim sqlResultSet As SqlCeDataReader
        Dim arrPOGList As ArrayList = Nothing
        Dim tempPOGList As ArrayList = New ArrayList()
        Dim bStatus As Boolean
        strSqlCmd = String.Format(strSqlCmd, strBootsCode)
        Try
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'Traverse through each element of the data set and update the details
            'in the object.
            arrPOGList = New ArrayList()
            If sqlResultSet.Read() Then
                tempPOGList.Add(sqlResultSet("Live_POG1").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG2").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG3").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG4").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG5").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG6").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG7").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG8").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG9").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG10").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG11").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG12").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG13").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG14").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG15").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG16").ToString())

                For Each strPOGID As String In tempPOGList
                    If strPOGID <> "" And strPOGID <> "0" Then
                        arrPOGList.Add(strPOGID)
                    End If
                Next


                If arrPOGList.Count >= 1 Then
                    If bIsMultisiteCall And arrPOGList.Count >= 1 Then
                        bStatus = UpdatePlannerList(arrPOGList, strBootsCode, _
                                                    arrPlannerList)
                        'The Following Code is commented because for auto shelf quantity
                        'the PSC is needed which has to be get from the database
                        'Instead of adding a new querry the same function is reused.
                        'ElseIf bIsMultisiteCall And arrPOGList.Count = 1 Then
                        '    Dim objPlannerInfo As PlannerInfo = New PlannerInfo()
                        '    With objPlannerInfo
                        '        .PlannerID = arrPOGList(0)
                        '        .Description = "NA"
                        '        .RebuildDate = "NA"
                        '        .SequenceNumber = "NA"
                        '        .ShelfNumber = "NA"
                        '    End With
                        'arrPlannerList.Add(objPlannerInfo)
                        'bStatus = True
                    Else
                        bStatus = UpdatePlannerList(arrPOGList, arrPlannerList)
                        If arrPlannerList.Count > 0 Then
                            'bTemp = True
                            'sort the planner list
                            Dim iComp As PogCompare = New PogCompare()
                            arrPlannerList.Sort(iComp)
                        End If
                    End If
                Else
                    bStatus = False
                End If
                Return bStatus
            Else
                'return false if the dataset does not have any data.
                Return False

            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            'return false in case of any exception.
            Return False
        Finally
            'dispose the data reader.
            sqlResultSet = Nothing
        End Try
    End Function
    ''' <summary>
    ''' Gets the Planner list where the scanned product is present using 
    ''' Product Code.
    ''' </summary>
    ''' <param name="strProductCode">Product code without check digit</param>
    ''' <param name="arrPlannerList">Array list to contain the objects.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Public Function GetPlannerListUsingPC(ByVal strProductCode As String, ByVal bIsMultisiteCall As Boolean, ByRef arrPlannerList As ArrayList) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_PLANNER_LIST_USING_PC
        'SFA Added Val()
        strSqlCmd = String.Format(strSqlCmd, Val(strProductCode))
        Dim sqlResultSet As SqlCeDataReader
        Dim arrPOGList As ArrayList = Nothing
        Dim tempPOGList As ArrayList = New ArrayList()
        Dim bStatus As Boolean
        Try
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'Traverse through each element of the data set and update the details
            'in the object.
            arrPOGList = New ArrayList()
            If sqlResultSet.Read() Then
                tempPOGList.Add(sqlResultSet("Live_POG1").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG2").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG3").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG4").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG5").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG6").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG7").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG8").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG9").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG10").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG11").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG12").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG13").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG14").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG15").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG16").ToString())

                For Each strPOGID As String In tempPOGList
                    If strPOGID <> "" And strPOGID <> "0" Then
                        arrPOGList.Add(strPOGID)
                    End If
                Next

                If arrPOGList.Count >= 1 Then
                    If bIsMultisiteCall And arrPOGList.Count >= 1 Then
                        bStatus = UpdatePlannerList(arrPOGList, _
                                                    GetBootsCode(strProductCode), _
                                                    arrPlannerList)
                        'The Following Code is commented because for auto shelf quantity
                        'the PSC is needed which has to be get from the database
                        'Instead of adding a new querry the same function is reused.
                        'ElseIf bIsMultisiteCall And arrPOGList.Count = 1 Then
                        '    Dim objPlannerInfo As PlannerInfo = New PlannerInfo()
                        '    With objPlannerInfo
                        '        .PlannerID = arrPOGList(0)
                        '        .Description = "NA"
                        '        .RebuildDate = "NA"
                        '        .SequenceNumber = "NA"
                        '        .ShelfNumber = "NA"
                        '    End With
                        '    arrPlannerList.Add(objPlannerInfo)
                        '    bStatus = True
                    Else
                        bStatus = UpdatePlannerList(arrPOGList, arrPlannerList)
                    End If
                Else
                    bStatus = False
                End If

                Return bStatus
            Else
                'return false if the dataset does not have any data.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false in case of any exception.
            Return False
        Finally
            'Dispose the data reader.
            sqlResultSet = Nothing
        End Try
    End Function
    ''' <summary>
    ''' Gets the Module list for the selected planner.
    ''' </summary>
    ''' <param name="strPOGID">Planogram ID</param>
    ''' <param name="arrModuleList">Arraylist to hold the objects.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Public Function GetModuleList(ByVal strPOGID As String, ByRef arrModuleList As ArrayList) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_MODULE_LIST
        strSqlCmd = String.Format(strSqlCmd, strPOGID)
        Dim dsList As DataSet = Nothing
        Dim objModules As ModuleInfo
        Try
            dsList = New DataSet()
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'Traverse through each element of the data set and update the details
            'in the object.
            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objModules = New ModuleInfo()
                    With objModules
                        .ModuleID = dsList.Tables(0).Rows(iCount)("Module_ID") _
                                                                .ToString()
                        .SequenceNumber = dsList.Tables(0).Rows(iCount) _
                                                ("Module_Seq").ToString()
                        .Description = dsList.Tables(0).Rows(iCount)("Module_Desc") _
                                                                .ToString()
                    End With
                    'Add the object with details to the array list.
                    arrModuleList.Add(objModules)
                Next
            Else
                'return false if the dataset does not have any data.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false in case of any exception.
            Return False
        Finally
            'Dispose the dataset objects.
            dsList.Dispose()
        End Try
        'If executed successfully, return true.
        Return True
    End Function
    ''' <summary>
    ''' Gets the Module list for the selected planner in Search Planner module
    ''' </summary>
    ''' <param name="strPOGID">Planogram ID</param>
    ''' <param name="arrModuleList">Arraylist to hold the objects.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Public Function GetModuleList(ByVal strPOGID As String, ByVal strBootsCode As String, ByRef arrModuleList As ArrayList) As Boolean
        Dim sqlResultSet As SqlCeDataReader
        Dim sqlCmd As SqlCeCommand
        Dim objModules As ModuleInfo
        Dim bStatus As Boolean = False
        Try
            objAppContainer.objLogger.WriteAppLog("Get Module List Start", Logger.LogLevel.RELEASE)
            'create sql command.
            sqlCmd = New SqlCeCommand(QueryMacro.GET_MODULE_LIST_FOR_SEARCH_PLANNER)
            sqlCmd.Parameters.Add("@BootsCode", SqlDbType.NVarChar)
            sqlCmd.Parameters("@BootsCode").Value = strBootsCode
            sqlCmd.Parameters.Add("@ModID", SqlDbType.NVarChar)
            sqlCmd.Parameters("@ModID").Value = strPOGID
            If objAppContainer.objDBConnection.ExecuteSQLQuery(sqlCmd, sqlResultSet) Then
                'Traverse through each row and update the details
                While sqlResultSet.Read()
                    objModules = New ModuleInfo()
                    With objModules
                        .ModuleID = sqlResultSet("Module_ID").ToString()
                        .SequenceNumber = sqlResultSet("Module_Seq").ToString()
                        .Description = sqlResultSet("Module_Desc").ToString()
                    End With
                    'Add the object with details to the array list.
                    arrModuleList.Add(objModules)
                    bStatus = True
                End While
            Else
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false in case of any exception.
            Return False
        Finally
            'Dispose the dataset objects.
            sqlResultSet = Nothing
        End Try
        objAppContainer.objLogger.WriteAppLog("Get Module List End", Logger.LogLevel.RELEASE)
        'return status.
        Return bStatus
    End Function
    ''' <summary>
    ''' Gets the Line list items for the selected Module.
    ''' </summary>
    ''' <param name="strModuleID">Module ID</param>
    ''' <param name="arrLineList">Arraylist to hold linelist items.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If any error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Public Function GetLineList(ByVal strModuleID As String, ByVal strSequenceNumber As String, ByRef arrLineList As ArrayList) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_LINELIST
        strSqlCmd = String.Format(strSqlCmd, strModuleID, strSequenceNumber)
        Dim objLineList As SPLineListInfo = Nothing
        Dim dsLineList As DataSet = Nothing
        Try
            objAppContainer.objLogger.WriteAppLog("Get line list start", Logger.LogLevel.RELEASE)
            dsLineList = New DataSet()
            dsLineList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'Traverse through each element of the data set and update the details
            'in the object.
            If dsLineList.Tables(0).Rows.Count > 0 Then
                'If the data set contains more that one row put in all the 
                'details to the object.
                For iCount As Integer = 0 To dsLineList.Tables(0).Rows.Count - 1
                    objLineList = New SPLineListInfo()
                    With objLineList
                        .ShelfNumber = dsLineList.Tables(0).Rows(iCount)("Shelf_Numb").ToString()
                        .ShelfDesc = dsLineList.Tables(0).Rows(iCount)("Shelf_Desc").ToString()
                        .ItemCode = dsLineList.Tables(0).Rows(iCount)("Boots_Code").ToString()
                        .ItemDescription = dsLineList.Tables(0).Rows(iCount)("Item_Desc").ToString()
                        .FaceCount = dsLineList.Tables(0).Rows(iCount)("Facings").ToString()
                        .NotchNumber = dsLineList.Tables(0).Rows(iCount)("Notch_No").ToString()
                        .ModuleSeqNumber = dsLineList.Tables(0).Rows(iCount)("Module_Seq").ToString()
                    End With
                    'Add the object to the array list.
                    arrLineList.Add(objLineList)
                Next
            Else
                'return false if the dataset does not have any data.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false in case of any exception.
            Return False
        Finally
            'Dispose dataset object after the usage.
            dsLineList.Dispose()
        End Try
        objAppContainer.objLogger.WriteAppLog("Get line list end", Logger.LogLevel.RELEASE)
        'Return true if successfully updated the object.
        Return True
    End Function
    ''' <summary>
    ''' Gets the list of Planners which contains the Category selected.
    ''' </summary>
    ''' <param name="strCategoryID">Category ID</param>
    ''' <param name="arrPlannerList">Array list to hold the objects.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Public Function GetPlannerListForCategory(ByVal strCategoryID As String, ByRef arrPlannerList As ArrayList) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_PLANNER_LIST_USING_CATEGORYID
        strSqlCmd = String.Format(strSqlCmd, strCategoryID)
        Dim dsList As DataSet = Nothing
        Dim objPlanners As PlannerInfo
        Try
            dsList = New DataSet()
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'Traverse through each element of the data set and update the details
            'in the object.
            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objPlanners = New PlannerInfo()
                    With objPlanners
                        .PlannerID = dsList.Tables(0).Rows(iCount)("Module_ID") _
                                                                .ToString()
                        .Description = dsList.Tables(0).Rows(iCount)("POG_Desc") _
                                                                .ToString()
                        .RebuildDate = dsList.Tables(0).Rows(iCount)("Start_Date") _
                                                                .ToString()
                    End With
                    'Add the object with details to the array list.
                    arrPlannerList.Add(objPlanners)
                Next
                Dim iComp As PogCompare = New PogCompare()
                arrPlannerList.Sort(iComp)
            Else
                'return false if the dataset does not have any data.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false in case of any exception.
            Return False
        Finally
            'Dispose the dataset
            dsList.Dispose()
        End Try
        'If executed successfully, return true.
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
    Public Function GetBootsCode(ByVal strProductCode As String) As String
        Dim strSqlCmd As String = QueryMacro.GET_BOOTS_CODE
        'Substitue Product code in the query string.
        'SFA Added val()
        strSqlCmd = String.Format(strSqlCmd, Val(strProductCode))
        Dim sqlResultSet As SqlCeDataReader
        Try
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'Return the value to the calling fucntion.
            If sqlResultSet.Read() Then
                Return sqlResultSet("Boots_Code").ToString()
            Else
                'return 0 if the sql reader does not have any data.
                Return 0
            End If
        Catch ex As Exception
            'return false in case of any exception.
            Return 0
        Finally
            'Dispose the data reader.
            sqlResultSet = Nothing
        End Try
    End Function
    ''' <summary>
    ''' To get Product Code for a product's Boots Code
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <returns>
    ''' BarCode - If Product Bar code is available for the Boots code entered.
    ''' 0 - If any error occured while trying to get data from DB.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetProductCode(ByVal strBootsCode As String) As String
        Dim strSqlCmd As String = QueryMacro.GET_PRODUCT_CODE
        'Format the string with the Product code in query string.
        strSqlCmd = String.Format(strSqlCmd, strBootsCode)
        Dim sqlResultSet As SqlCeDataReader
        'SFA
        Dim strFirstBarCode As String = ""
        Dim strSecondBarCode As String = ""
        Try
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            If sqlResultSet.Read() Then
                'SFA
                strFirstBarCode = sqlResultSet("First_Barcode").ToString().PadLeft(12, "0") 'SFA-Added padleft
                strSecondBarCode = sqlResultSet("Second_Barcode").ToString().PadLeft(12, "0")
                'Compare first barcode and Boots code and return the 
                'actual barcode accordingly.
                If CheckBarcode(sqlResultSet("Boots_Code").ToString(), _
                                       strFirstBarCode, strSecondBarCode) Then
                    Return strSecondBarCode
                Else
                    Return strFirstBarCode
                End If
                'end
            End If
            'return 0 if the sql reader does not have any data.
            Return 0
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false in case of any exception.
            Return 0
        Finally
            'Dispose the data reader.
            sqlResultSet = Nothing
        End Try
    End Function
    ''' <summary>
    ''' To get the item description for a product using its boots code.
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <returns>
    ''' Item Description - If Product description is available for the Boots code.
    ''' 0 - If any error occured while trying to get data from DB.</returns>
    ''' <remarks></remarks>
    Public Function GetItemDescription(ByVal strBootsCode As String) As String
        Dim strSqlCmd As String = QueryMacro.GET_ITEM_DESCRIPTION
        'Format the string with the Product code in query string.
        strSqlCmd = String.Format(strSqlCmd, strBootsCode)
        Dim sqlResultSet As SqlCeDataReader
        Try
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            If sqlResultSet.Read() Then
                'Return the description
                Return sqlResultSet("Item_Desc").ToString()
            End If
            'return 0 if the sql reader does not have any data.
            Return 0
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false in case of any exception.
            Return 0
        Finally
            'Dispose the data reader.
            sqlResultSet = Nothing
        End Try
    End Function
    ''' <summary>
    ''' To get the item description for a product using its boots code.
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <returns>
    ''' Item Description - If Product description is available for the Boots code.
    ''' 0 - If any error occured while trying to get data from DB.</returns>
    ''' <remarks></remarks>
    Public Function ValidateUsingPCAndBC(ByVal strBootsCode As String, ByVal strProductCode As String) As Boolean
        Dim strSqlCmd As String = QueryMacro.VALIDATE_USING_PC_AND_BC
        'Format the string with the Product code in query string.
        'SFA Added VAL() of product code as it is prepended with zeros
        strSqlCmd = String.Format(strSqlCmd, Val(strProductCode), strBootsCode)
        Dim sqlResultSet As SqlCeDataReader
        Try
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            If sqlResultSet.Read() Then
                'Return the description
                Return True
            Else
                'return 0 if the sql reader does not have any data.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false in case of any exception.
            Return False
        Finally
            'Dispose the data reader.
            sqlResultSet = Nothing
        End Try
    End Function
    ''' <summary>
    ''' Compares the Boots code and the First Barcode to check whether the
    ''' First Barcode and Boots are same.
    ''' </summary>
    ''' <param name="strBootsCode">Boots Code</param>
    ''' <param name="strFirstBarcode">First Barcode</param>
    ''' <returns>Bool
    ''' True - If the pattern matched i.e., if barcode contains Boots Code.
    ''' False - If the pattern does not match i.e., if the barcode not 
    '''         contains Boots code.
    ''' </returns>
    ''' <remarks></remarks>
    Private Function CheckBarcode(ByVal strBootsCode As String, ByVal strFirstBarcode As String, ByVal strSecbarcode As String) As Boolean
        Dim strRegExp As Regex
        strBootsCode = strBootsCode.Substring(0, 6)
        strRegExp = New Regex("^[0]{6}" & strBootsCode & "$")
        'If the first barcode matches the first six digits of Boots code
        'in the last then return true else return false.
        If strRegExp.IsMatch(strFirstBarcode) Then
            If Val(strSecbarcode).Equals(0) Then
                Return False
            Else
                Return True
            End If
        Else
            Return False
        End If
    End Function
    ''' <summary>
    ''' Updates the Shelf Monitor picking list.
    ''' </summary>
    ''' <param name="arrPickingList">Arraylist to hold the SMProductInfo type objects.</param>
    ''' <param name="dsProductInfo">Dataset with Pickign list details from database.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Private Function UpdateSMPickingList(ByRef arrPickingList As ArrayList, ByRef dsProductInfo As DataSet) As Boolean
        Dim objSMPLProductInfo As PLProductInfo = Nothing
        'SFA
        Dim strFirstBarCode As String = ""
        Dim strSecondBarCode As String = ""
        Dim strPSPFlag As String
        'end
        'Check if the number of objects in the array list is same as the
        'number of rows in the result data set.
        If dsProductInfo.Tables(0).Rows.Count > 0 Then
            For iCount As Integer = 0 To dsProductInfo.Tables(0).Rows.Count - 1
                objSMPLProductInfo = New PLProductInfo()
                With objSMPLProductInfo
                    .BootsCode = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Boots_Code").ToString()
                    'SFA
                    strFirstBarCode = dsProductInfo.Tables(0).Rows(iCount) _
                            ("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarCode = dsProductInfo.Tables(0).Rows(iCount) _
                            ("Second_Barcode").ToString().PadLeft(12, "0")
                    'Compare first barcode and Boots code to identify the real
                    'barcode of the item.
                    If CheckBarcode( _
                    dsProductInfo.Tables(0).Rows(iCount)("Boots_Code").ToString(), _
                    strFirstBarCode, strSecondBarCode) Then
                        .ProductCode = strSecondBarCode
                    Else
                        .ProductCode = strFirstBarCode
                    End If
                    'Fix for 2377
                    .FirstBarcode = strFirstBarCode
                    'End fix for 2377
                    ' end
                    .Description = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("SEL_Desc").ToString()
                    .ShortDescription = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Item_Desc").ToString()
                    .Status = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Item_Status").ToString()
                    'SFA DEF# 847 - Retrieve Stock figure fom pickListItem table 
                    .TSF = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Stock_Figure").ToString()
                    .ListItemStatus = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("List_Item_Status").ToString()
                    'Fields corresponding to the Shelf Monitor picking list.
                    .SequenceNumber = dsProductInfo.Tables(0) _
                                    .Rows(iCount)("Sequence_Number").ToString()
                    .SalesFloorQuantity = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Shelf_Qty").ToString()
                    .QuantityRequired = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Fill_Qty").ToString()
                    .BCType = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("BC").ToString()
                    .ProductGrp = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Product_Group").ToString()
                    strPSPFlag = dsProductInfo.Tables(0) _
                                         .Rows(iCount)("PSP_Flag").ToString()
                    If strPSPFlag = "Y" Or strPSPFlag = "y" Then
                        .PendingSalesFlag = True
                    Else
                        .PendingSalesFlag = False
                    End If
                End With
                'Add the item to the array list
                arrPickingList.Add(objSMPLProductInfo)
            Next
        Else
            'If the count is 0
            Return False
        End If
        'If successfully updated the detals in the object.
        Return True
    End Function

    ''' <summary>
    ''' Updated the Fast Fill Picking List.
    ''' </summary>
    ''' <param name="arrPickingList">Arraylist to hold the FFProductInfo type 
    ''' objects.</param>
    ''' <param name="dsProductInfo">Data set with values read from database</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Private Function UpdateFFPickingList(ByRef arrPickingList As ArrayList, ByRef dsProductInfo As DataSet) As Boolean
        Dim objFFPLProductInfo As PLProductInfo = Nothing
        'SFA
        Dim strFirstBarCode As String = ""
        Dim strSecondBarCode As String = ""
        ' end
        'Check if the no. of objects in the array list is same as
        'the number of rows in the dataset.
        If dsProductInfo.Tables(0).Rows.Count > 0 Then
            For iCount As Integer = 0 To dsProductInfo.Tables(0).Rows.Count - 1
                objFFPLProductInfo = New PLProductInfo()
                With objFFPLProductInfo
                    .BootsCode = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Boots_Code").ToString()
                    'SFA
                    strFirstBarCode = dsProductInfo.Tables(0).Rows(iCount) _
                            ("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarCode = dsProductInfo.Tables(0).Rows(iCount) _
                            ("Second_Barcode").ToString().PadLeft(12, "0")
                    'Compare first barcode and Boots code to identify the real
                    'barcode for the item.
                    If CheckBarcode( _
                    dsProductInfo.Tables(0).Rows(iCount)("Boots_Code").ToString(), _
                                            strFirstBarCode, strSecondBarCode) Then
                        .ProductCode = strSecondBarCode
                    Else
                        .ProductCode = strFirstBarCode
                    End If
                    'Fix for 2377
                    .FirstBarcode = strFirstBarCode
                    'End Fix for 2377
                    ' end
                    .Description = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("SEL_Desc").ToString()
                    .ShortDescription = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Item_Desc").ToString()
                    .Status = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Item_Status").ToString()
                    'SFA DEF# 847 - Retrieve Stock figure fom pickListItem table 
                    .TSF = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Stock_Figure").ToString()
                    .ListItemStatus = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("List_Item_Status").ToString()
                    'Fields correspondin to the FF picking list.
                    .SequenceNumber = dsProductInfo.Tables(0) _
                                    .Rows(iCount)("Sequence_Number").ToString()
                    .QuantityRequired = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Fill_Qty").ToString()
                    .BCType = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("BC").ToString()
                    .ProductGrp = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Product_Group").ToString()
                End With
                'Add the item to the array list
                arrPickingList.Add(objFFPLProductInfo)
            Next
        Else
            'If the row count and the object count mismatches.
            Return False
        End If
        'If successfully updated the details in the product.
        Return True
    End Function
    Private Function UpdateAFFPickingList(ByRef arrPickingList As ArrayList, ByRef dsProductInfo As DataSet) As Boolean
        Dim objAFFPLProductInfo As PLProductInfo = Nothing
        'SFA
        Dim strFirstBarCode As String = ""
        Dim strSecondBarCode As String = ""
        'end
        'Check if the no. of objects in the array list is same as
        'the number of rows in the dataset.
        If dsProductInfo.Tables(0).Rows.Count > 0 Then
            For iCount As Integer = 0 To dsProductInfo.Tables(0).Rows.Count - 1
                objAFFPLProductInfo = New PLProductInfo()


                With objAFFPLProductInfo
                    .BootsCode = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Boots_Code").ToString()
                    'SFA
                    strFirstBarCode = dsProductInfo.Tables(0).Rows(iCount) _
                            ("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarCode = dsProductInfo.Tables(0).Rows(iCount) _
                            ("Second_Barcode").ToString().PadLeft(12, "0")
                    'Compare first barcode and Boots code to identify the real
                    'barcode for the item.
                    If CheckBarcode( _
                    dsProductInfo.Tables(0).Rows(iCount)("Boots_Code").ToString(), _
                    strFirstBarCode, strSecondBarCode) Then
                        .ProductCode = strSecondBarCode
                    Else
                        .ProductCode = strFirstBarCode
                    End If
                    'Fix for 2377
                    .FirstBarcode = strFirstBarCode
                    'End Fix for 2377
                    ' end
                    .Description = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("SEL_Desc").ToString()

                    .ShortDescription = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Item_Desc").ToString()
                    .Status = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Item_Status").ToString()
                    'SFA DEF# 847 - Retrieve Stock figure fom pickListItem table 
                    .TSF = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Stock_Figure").ToString()
                    .ListItemStatus = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("List_Item_Status").ToString()
                    'Fields correspondin to the FF picking list.
                    .SequenceNumber = dsProductInfo.Tables(0) _
                                    .Rows(iCount)("Sequence_Number").ToString()
                    .QuantityRequired = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Fill_Qty").ToString()
                    .BCType = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("BC").ToString()
                    .ProductGrp = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Product_Group").ToString()
                End With
                'Add the item to the array list
                arrPickingList.Add(objAFFPLProductInfo)
            Next
        Else
            'If the row count and the object count mismatches.
            Return False
        End If
        'If successfully updated the details in the product.
        Return True
    End Function
    ''' <summary>
    ''' Updates Excess Stock picking list.
    ''' </summary>
    ''' <param name="arrPickingList">Array list to hold EXPLProductInfo type 
    ''' objects.</param>
    ''' <param name="dsProductInfo">Dataset with values read from database.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Private Function UpdateEXPickingList(ByRef arrPickingList As ArrayList, ByRef dsProductInfo As DataSet) As Boolean
        Dim objEXPLProductInfo As PLProductInfo = Nothing
        'SFA
        Dim strFirstBarCode As String = ""
        Dim strSecondBarCode As String = ""
        'end
        'Check if the no. of objects in the array list is same as
        'the number of rows in the dataset.
        If dsProductInfo.Tables(0).Rows.Count > 0 Then
            For iCount As Integer = 0 To dsProductInfo.Tables(0).Rows.Count - 1
                objEXPLProductInfo = New PLProductInfo()
                With objEXPLProductInfo
                    .BootsCode = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Boots_Code").ToString()
                    'SFA
                    strFirstBarCode = dsProductInfo.Tables(0).Rows(iCount) _
                            ("First_Barcode").ToString().PadLeft(12, "0")
                    strSecondBarCode = dsProductInfo.Tables(0).Rows(iCount) _
                            ("Second_Barcode").ToString().PadLeft(12, "0")
                    'Compare first barcode and Boots code to identify the real
                    'barcode of the item.
                    If CheckBarcode( _
                    dsProductInfo.Tables(0).Rows(iCount)("Boots_Code").ToString(), _
                    strFirstBarCode, strSecondBarCode) Then
                        .ProductCode = strSecondBarCode
                    Else
                        .ProductCode = strFirstBarCode
                    End If
                    'Fix for 2377
                    .FirstBarcode = strFirstBarCode
                    'End fix for 2377
                    'end
                    .Description = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("SEL_Desc").ToString()
                    .ShortDescription = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Item_Desc").ToString()
                    .Status = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Item_Status").ToString()
                    'SFA DEF# 847 - Retrieve Stock figure fom pickListItem table 
                    .TSF = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Stock_Figure").ToString()
                    'Fields corresponding to the Excess Stock picking list.
                    .SequenceNumber = dsProductInfo.Tables(0) _
                                    .Rows(iCount)("Sequence_Number").ToString()
                    .ListItemStatus = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("List_Item_Status").ToString()
                    .BackShopQuantity = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Back_Shop_Count").ToString()
                    .BCType = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("BC").ToString()
                    .ProductGrp = dsProductInfo.Tables(0) _
                                 .Rows(iCount)("Product_Group").ToString()
                End With
                'Add the item to the array list
                arrPickingList.Add(objEXPLProductInfo)
            Next
        Else
            'If there is a count mismatch
            Return False
        End If
        'If successfully updated the details in the object.
        Return True
    End Function
    ''' <summary>
    ''' To update the planner list after gathering the planner ID's for
    ''' a product using either Boots code or the Product code.
    ''' </summary>
    ''' <param name="arrPOGList">list of all Planner IDs</param>
    ''' <param name="arrPlannerList">Array list to hold PlannerInfo type 
    ''' objects.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results
    ''' </returns>
    ''' <remarks></remarks>
    Private Function UpdatePlannerList(ByVal arrPOGList As ArrayList, ByRef arrPlannerList As ArrayList) As Boolean
        Dim objPlanners As PlannerInfo = Nothing
        Dim strSqlCmd As String = Nothing
        Dim sqlResultSet As SqlCeDataReader = Nothing
        Dim bStatus As Boolean = False

        For Each strPOGID As String In arrPOGList
            'Get details for that planner key and store it in the object
            'of type PlannerInfo and return the status.
            strSqlCmd = QueryMacro.GET_PLANNER_DETAILS
            strSqlCmd = String.Format(strSqlCmd, strPOGID.Trim())
            objPlanners = New PlannerInfo()
            Try
                sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
                If sqlResultSet.Read() Then
                    With objPlanners
                        .PlannerID = sqlResultSet("Module_ID").ToString()
                        .Description = sqlResultSet("POG_Desc").ToString()
                        .RebuildDate = sqlResultSet("Start_Date").ToString()

                    End With
                    arrPlannerList.Add(objPlanners)
                    'Set the status
                    bStatus = True
                End If
            Catch ex As Exception
                'Add the exception to the application log.
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                                ex.Message.ToString(), _
                                                Logger.LogLevel.RELEASE)
                'move to next item in the array.
                Continue For
            Finally
                'dispose the variables used.
                strSqlCmd = ""
                sqlResultSet = Nothing
            End Try
        Next
        'Return the status
        Return bStatus
    End Function
    ''' <summary>
    ''' To get the multisite list of an item in the picking list
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <param name="strListID"></param>
    ''' <param name="arrPlannerList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPickingListMultisiteList(ByVal strBootsCode As String, ByVal strListID As String, ByRef arrPlannerList As ArrayList) As Boolean
        Dim objPlanners As PlannerInfo = Nothing
        Dim strSqlCmd As String = Nothing
        Dim sqlResultSet As SqlCeDataReader = Nothing
        Dim bStatus As Boolean = False
        Dim isRead As Boolean = False
        Try

            strSqlCmd = QueryMacro.GET_PICKINGLIST_MULTISITE_DETAILS
            strSqlCmd = String.Format(strSqlCmd, strBootsCode, strListID)

            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            arrPlannerList.Clear()
            While sqlResultSet.Read()
                isRead = True
                objPlanners = New PlannerInfo()
                With objPlanners
                    .Description = sqlResultSet("Module_Desc").ToString().Trim()
                    .POGDesc = sqlResultSet("POG_Desc").ToString().Trim()
                    .RepeatCount = sqlResultSet("Repeat_Count").ToString.Trim()
                End With
                arrPlannerList.Add(objPlanners)
            End While
            'Set the status
            If isRead = True Then
                bStatus = True
            Else
                bStatus = False
            End If


        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'Set the status
            bStatus = False
        Finally
            'dispose the variables used.
            strSqlCmd = ""
            sqlResultSet = Nothing
        End Try
        'Next
        'Return the status
        Return bStatus
    End Function
    ''' <summary>
    ''' To update the planner list after gathering the planner ID's for
    ''' a product using either Boots code or the Product code.
    ''' </summary>
    ''' <param name="arrPOGList">list of all Planner IDs</param>
    ''' <param name="arrPlannerList">Array list to hold PlannerInfo type 
    ''' objects.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results
    ''' </returns>
    ''' <remarks></remarks>
    Private Function UpdatePlannerList(ByVal arrPOGList As ArrayList, ByVal strBootsCode As String, ByRef arrPlannerList As ArrayList) As Boolean
        Dim objPlanners As PlannerInfo = Nothing
        Dim tempArrayList As ArrayList = New ArrayList()
        'Get the sql command to read the Planner details from the database.
        Dim strSqlCmd As String = Nothing
        Dim sqlResultSet As SqlCeDataReader = Nothing
        Dim bStatus As Boolean = False
        Dim strModID As String = ""
        Dim isRead As Boolean = False
        Try
            For Each strPOGID As String In arrPOGList
                strModID = strModID + "'" + strPOGID.Trim() + "',"
            Next
            'Trim the last comma.
            strModID = strModID.TrimEnd(",")

            'For Each strPOGID As String In arrPOGList
            'Get details for that planner key and store it in the object
            'of type PlannerInfo and return the status.
            strSqlCmd = QueryMacro.GET_MULTISITE_DETAILS
            strSqlCmd = String.Format(strSqlCmd, strBootsCode, strModID)

            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)

            While sqlResultSet.Read()
                isRead = True
                objPlanners = New PlannerInfo()
                With objPlanners
                    .PlannerID = sqlResultSet("Module_ID").ToString()
                    .Description = sqlResultSet("Module_Desc").ToString().Trim()
                    .SequenceNumber = sqlResultSet("Module_Seq").ToString()
                    .ShelfNumber = sqlResultSet("Shelf_Numb").ToString()
                    .POGDesc = sqlResultSet("POG_Desc").ToString().Trim()
                    .PhysicalShelfQty = sqlResultSet("PSC").ToString.Trim()
                    .RepeatCount = sqlResultSet("Repeat_Count").ToString.Trim()
                End With
                arrPlannerList.Add(objPlanners)
            End While
            'Set the status
            If isRead = True Then
                bStatus = True
            Else
                bStatus = False
            End If


        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'Set the status
            bStatus = False
        Finally
            'dispose the variables used.
            strSqlCmd = ""
            sqlResultSet = Nothing
        End Try
        'Next
        'Return the status
        Return bStatus
    End Function
    ''' <summary>
    ''' To create an index for the table
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CreateTableIndex() As Boolean
        'TODo -> Create dummy queries for bootscodeview,barcodeview,
        Dim strSqlCmd As String = QueryMacro.GET_DUMMY_VALUE_BOOTCODE
        Dim strSqlCmd1 As String = QueryMacro.GET_DUMMY_VALUE_BARCODE
        Dim sqlResultSet As SqlCeDataReader
        Dim sqlResultSet1 As SqlCeDataReader
        Dim objItemInfo As ItemInfo = New ItemInfo()
        Dim objBootsCode As String = Nothing
        Dim strBarCode As String = Nothing
        Try
            ''execute the command 
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            sqlResultSet1 = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd1)
            If sqlResultSet.Read Then
                strBarCode = sqlResultSet("First_Barcode").ToString()
                If CheckBarcode(sqlResultSet("Boots_Code").ToString(), _
                               strBarCode, _
                               sqlResultSet("Second_Barcode").ToString()) Then
                    GetItemDetailsAllUsingPC( _
                                        sqlResultSet("Second_Barcode").ToString(), _
                                        objItemInfo)
                Else
                    GetItemDetailsAllUsingPC(strBarCode, objItemInfo)
                End If
            Else
                'If any error in reading the data reader.
                Return False
            End If
            GetItemDetailsAllUsingBC(objItemInfo.BootsCode, objItemInfo)
            Dim objPlannerList As ArrayList = New ArrayList()
            GetPlannerListUsingPC(objItemInfo.ProductCode, True, objPlannerList)
            GetPlannerListUsingBC(objItemInfo.BootsCode, True, objPlannerList)

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
        Finally
            'Clear the memory occupied by the variable.
            sqlResultSet1 = Nothing
            'Clear the memory occupied by the variable.
            sqlResultSet = Nothing
        End Try
    End Function
    ''' <summary>
    ''' Check if an item is multisite or not.
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckMultisite(ByVal strBootsCode As String) As String
        Dim strSqlCmd As String = QueryMacro.GET_PLANNER_LIST_USING_BC
        Dim sqlResultSet As SqlCeDataReader
        Dim arrPOGList As ArrayList = Nothing
        Dim tempPOGList As ArrayList = New ArrayList()
        strSqlCmd = String.Format(strSqlCmd, strBootsCode)
        Try
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'Traverse through each element of the data set and update the details
            'in the object.
            arrPOGList = New ArrayList()
            If sqlResultSet.Read() Then
                tempPOGList.Add(sqlResultSet("Live_POG1").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG2").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG3").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG4").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG5").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG6").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG7").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG8").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG9").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG10").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG11").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG12").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG13").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG14").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG15").ToString())
                tempPOGList.Add(sqlResultSet("Live_POG16").ToString())

                For Each strPOGID As String In tempPOGList
                    If strPOGID <> "" And strPOGID <> "0" Then
                        arrPOGList.Add(strPOGID)
                    End If
                Next
                If arrPOGList.Count > 1 Then
                    Return "Y"
                Else
                    Return " "
                End If
            Else
                Return " "
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return " "
        Finally
            'Clear the memory occupied by the variable.
            sqlResultSet = Nothing
            'Clear the memory occupied by the variable.
            sqlResultSet = Nothing
        End Try
    End Function
End Class
#End If