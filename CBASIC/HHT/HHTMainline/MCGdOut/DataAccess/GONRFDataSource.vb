#If NRF Then
Imports System
Imports System.Data
Imports System.Text.RegularExpressions
Imports System.Data.SqlServerCe
'''******************************************************************************
''' <FileName>SMNRFDataSource.vb</FileName>
''' <summary>
''' This class is the data source class for the Shelf Management application.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>08-Dec-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''******************************************************************************
Public Class GONRFDataSource
    ''' <summary>
    ''' Gets the roduct details using the product code.
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="objGOItemInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, ByRef objGOItemInfo As GOItemInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_DETAILS_USING_PC
        ' Added val() to remove zeros
        strSqlCmd = String.Format(strSqlCmd, Val(strProductCode))
        Dim sqlResultSet As SqlCeDataReader
        Dim strStatus0 As String
        Dim strStatus8 As String
        Dim cRecallType As Char
        Dim bTailoredFlag As Boolean
        Dim strStockDate As Date
        Try
            'execute the sql command and collect the result in the reader.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objGOItemInfo
                    .BootsCode = sqlResultSet("Boots_Code").ToString()
                    .FirstBarcode = sqlResultSet("First_Barcode").ToString()
                    '.ProductCode = strProductCode
                    'Sytem Testing -Lakshmi Added to display Product code
                    .ProductCode = sqlResultSet("Second_Barcode").ToString()
                    .Description = sqlResultSet("SEL_Desc").ToString()
                    .Status = sqlResultSet("Item_Status").ToString()
                    .TSF = sqlResultSet("SOD_TSF").ToString()
                    .BusinessCentreType = sqlResultSet("BC").ToString()
                    .ItemPrice = sqlResultSet("Current_Price").ToString()
                    .SupplyRoute = sqlResultSet("Supply_Route").ToString()
                    .ShortDescription = sqlResultSet("Item_Desc").ToString()
                    If objAppContainer.bIsCreateRecalls Then
                        .strItemStatus_0 = sqlResultSet("Item_Status0").ToString()
                        .strItemStatus_8 = sqlResultSet("Item_Status8").ToString()
                        strStatus0 = objAppContainer.objHelper.DecimaltoBinary(.strItemStatus_0)
                        strStatus8 = objAppContainer.objHelper.DecimaltoBinary(.strItemStatus_8)
                        .StockMoveDate = sqlResultSet("Last_Stock_Movement_Date").ToString()
                        'Minu REmoved conversion and pad left
                        strStockDate = .StockMoveDate
                        RLSessionMgr.GetInstance().GetRecallType(strStatus0, strStatus8, cRecallType)
                        RLSessionMgr.GetInstance().SetTailoringFlag(Convert.ToInt32(.TSF), strStockDate, bTailoredFlag, _
                                                                    sqlResultSet("Live_POG1").ToString())
                        .strRecallType = cRecallType
                        If strStatus0.Substring(3, 1) = "1" Then
                            .IsTillBarSet = True
                        Else
                            .IsTillBarSet = False
                        End If
                        .Tailored = bTailoredFlag
                    End If
                End With
            Else
                With objGOItemInfo
                    .BootsCode = "0000000"
                    '.BootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strProductCode.Substring(6, 6))
                    '.FirstBarcode = ""
                    .FirstBarcode = strProductCode
                    .Status = ""
                    .TSF = 0
                    .BusinessCentreType = ""
                    .ItemPrice = "0"
                    .SupplyRoute = ""
                    .ShortDescription = "Unknown Item"
                    .Description = "Unknown Item"
                    .ProductCode = strProductCode
                End With
            End If
                'Return False
            'End If
        Catch ex As Exception
            Return False
        Finally
            sqlResultSet = Nothing
        End Try
        'if all the execution part is comepleted successfully, return true.
        Return True
    End Function
    ''' <summary>
    ''' Gets the item details required for a product in Shelf Monitor 
    ''' session.
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <param name="objGOItemInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingBC(ByVal strBootsCode As String, ByRef objGOItemInfo As GOItemInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_ITEM_DETAILS_ALL_USING_BC
        strSqlCmd = String.Format(strSqlCmd, strBootsCode)
        Dim strBarCode As String
        Dim sqlResultSet As SqlCeDataReader
        Try
            'execute the sql command and collect the result in the reader.
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                strBarCode = sqlResultSet("First_Barcode").ToString()
                If CompareFirstBarcode(sqlResultSet("Boots_Code").ToString(), _
                                       strBarCode, _
                                       sqlResultSet("Second_Barcode").ToString()) Then
                    Return GetProductInfoUsingPC( _
                                        sqlResultSet("Second_Barcode").ToString(), _
                                        objGOItemInfo)
                Else
                    Return GetProductInfoUsingPC(strBarCode, objGOItemInfo)
                End If
            Else
                Dim strProdCode As String = ""
                With objGOItemInfo
                    .BootsCode = strBootsCode
                    'Create a barcode similar to first barcode.
                    strProdCode = strBootsCode.Substring(0, 6).PadLeft(12, "0")
                    .FirstBarcode = strProdCode
                    .Status = ""
                    .TSF = 0
                    .BusinessCentreType = ""
                    .ItemPrice = "0"
                    .SupplyRoute = ""
                    .ShortDescription = "Unknown Item"
                    .Description = "Unknown Item"
                    .ProductCode = strProdCode
                End With
            End If
        Catch ex As Exception
            Return False
        Finally
            sqlResultSet = Nothing
        End Try
        'if all the execution part is comepleted successfully, return true.
        Return True
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <param name="strFirstBarcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CompareFirstBarcode(ByVal strBootsCode As String, ByVal strFirstBarcode As String, ByVal strSecbarcode As String) As Boolean
        Dim strRegExp As Regex
        strBootsCode = strBootsCode.Substring(0, 6)
        strRegExp = New Regex("^[0]{6}" & strBootsCode & "$")
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
    ''' 
    ''' </summary>
    ''' <param name="arrRecallIst"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRecallList(ByRef arrRecallIst As ArrayList,ByVal strRecallType As String, ByVal m_RecallCount As RecallCount) As Boolean
        Dim strSqlCmd As String = Nothing
        'If strRecallType = "R" Then
        '    strSqlCmd = QueryMacro.GET_COMPANYHORECALL_LIST
        'Else
        If arrRecallIst Is Nothing Then
            arrRecallIst = New ArrayList
        End If
        strSqlCmd = QueryMacro.GET_RECALL_LIST
        'strSqlCmd = String.Format(strSqlCmd, strRecallType)
        'End If
        Dim objRLRecallInfo As RLRecallInfo
        Dim dsList As DataSet = Nothing
        Dim sqlResultSet As SqlCeDataReader
        Dim strStatus As String = ""
        'Tailoring
        Dim strType As String = ""
        Dim strLength As Integer
        Dim objStatusComparer As New RecallStatusComparer
        Try
            dsList = New DataSet()
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'Traverse through each element of the data set and update the details
            'in the object.
            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objRLRecallInfo = New RLRecallInfo()
                    With objRLRecallInfo
                        .ActiveDate = dsList.Tables(0).Rows(iCount) _
                                          ("Recall_Active_Date").ToString()
                        .RecallNumber = dsList.Tables(0).Rows(iCount) _
                                        ("Recall_Number").ToString()
                        .RecallDescription = dsList.Tables(0).Rows(iCount) _
                                           ("Recall_Desc").ToString()
                        .RecallType = dsList.Tables(0).Rows(iCount) _
                                    ("Recall_Type").ToString()
                        strType = .RecallType
                        .RecallQuantity = dsList.Tables(0).Rows(iCount) _
                                        ("Recall_Qty").ToString()
                        If strType = "R" Then
                            If .RecallDescription.StartsWith("NT*") Then
                                .Tailored = False
                            Else
                                .Tailored = True
                            End If
                        End If
                        'Pilot Support: Recall CR
                        .ListStatus = dsList.Tables(0).Rows(iCount) _
                                        ("Status").ToString()
                        .RecallMessage = dsList.Tables(0).Rows(iCount) _
                                        ("Recall_Message").ToString()
                        'If strStatus = "A" Then
                        '    .ListStatus = "Completed"
                        'ElseIf strStatus = "P" Then
                        '    .ListStatus = "Partial"
                        'Else
                        '    .ListStatus = "Unactioned"
                        'End If
                        'End Fix
                        'For Label Type for UOD scanning
                        .LabelType = dsList.Tables(0).Rows(iCount) _
                                  ("UOD_Label_Type").ToString().PadLeft(2, "0")
                        'FIX FOR DEFECT 4982,4957
                        .BatchNos = dsList.Tables(0).Rows(iCount) _
                                  ("Batch_Nos").ToString()
                        .MinRecallQty = dsList.Tables(0).Rows(iCount) _
                                  ("Min_Return_Qty").ToString()
                    End With
                    'Add the object with details to the array list.
                    arrRecallIst.Add(objRLRecallInfo)
                Next
                If arrRecallIst.Count > 0 Then
                    arrRecallIst.Sort(0, arrRecallIst.Count, objStatusComparer)
                End If
                Dim query_Emergency = From obj As RLRecallInfo In arrRecallIst _
                Where obj.RecallType = "E" _
                Select obj

                Dim query_Withdrawn = From obj As RLRecallInfo In arrRecallIst _
                Where obj.RecallType = "W" _
                Select obj

                Dim query_Returns = From obj As RLRecallInfo In arrRecallIst _
                Where obj.RecallType = "R" _
                Select obj

                Dim query_PlannerLeaver = From obj As RLRecallInfo In arrRecallIst _
                Where obj.RecallType = "I" _
                Select obj

                Dim query_ExcessSalesplan = From obj As RLRecallInfo In arrRecallIst _
                Where obj.RecallType = "C" _
                Select obj

                m_RecallCount.Customer = query_Emergency.Count
                m_RecallCount.Withdrawn = query_Withdrawn.Count
                m_RecallCount.Returns = query_Returns.Count
                m_RecallCount.PlannerLeaver = query_PlannerLeaver.Count
                m_RecallCount.ExcessSalesPlan = query_ExcessSalesplan.Count

            Else
                'return false if the dataset does not have any data.
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            sqlResultSet = Nothing
            dsList = Nothing
        End Try
        'if all the execution part is comepleted successfully, return true.
        Return True
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="strRecallNo"></param>
    ''' <param name="arrRecallItems"></param>
    ''' <returns>Bool
    ''' True - If successfully recevied and updated the details in 
    ''' the object array.
    ''' False - If any occured during the updation.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetRecallItemDetails(ByVal strRecallNo As String, ByVal iRecallQty As Integer, ByRef arrRecallItems As ArrayList, Optional ByVal isRecallTailored As Boolean = False) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_RECALL_LIST_ITEMS
        Dim strCmd As String = QueryMacro.GET_MISSING_RECALL_ITEMS
        strSqlCmd = String.Format(strSqlCmd, strRecallNo)
        Dim dsListItems As DataSet = Nothing
        Dim dsMissingItems As DataSet = Nothing
        Dim objArrayItem As RLItemInfo
        Dim iCount As Integer = 0
        Dim bTemp As Boolean = True
        Dim strLivePog1 As String = ""
        Dim objItemsinPlanner As Hashtable = Nothing
        objItemsinPlanner = New Hashtable
        'Correcting the count of Recall quantity to suit zero index.
        arrRecallItems.Clear()
        Try
            dsListItems = New DataSet()
            dsListItems = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)

            'Traverse through each element of the data set and update the details
            'in the object.
            Dim iRecallCount As Integer = dsListItems.Tables(0).Rows.Count
            
            For iCount = 0 To iRecallCount - 1
                objArrayItem = New RLItemInfo()
                With objArrayItem
                    .BootsCode = dsListItems.Tables(0) _
                                 .Rows(iCount)("Boots_Code").ToString()

                    If CompareFirstBarcode( _
                    dsListItems.Tables(0).Rows(iCount)("Boots_Code").ToString(), _
                    dsListItems.Tables(0).Rows(iCount)("First_Barcode").ToString(), _
                    dsListItems.Tables(0).Rows(iCount)("Second_Barcode").ToString()) Then
                        .ProductCode = dsListItems.Tables(0) _
                                 .Rows(iCount)("Second_Barcode").ToString()
                    Else
                        .ProductCode = dsListItems.Tables(0) _
                                 .Rows(iCount)("First_Barcode").ToString()
                    End If
                    .FirstBarcode = dsListItems.Tables(0) _
                                 .Rows(iCount)("First_Barcode").ToString()
                    .Description = dsListItems.Tables(0) _
                                 .Rows(iCount)("Item_Desc").ToString()
                    .Status = "Unactioned"
                    '.Status = dsListItems.Tables(0) _
                    '.Rows(iCount)("Item_Status").ToString()
                    .TSF = dsListItems.Tables(0) _
                                 .Rows(iCount)("SOD_TSF").ToString()
                    .ItemPrice = dsListItems.Tables(0) _
                                 .Rows(iCount)("Current_Price").ToString()
                    'Pilot Support: Recall CR
                    .RecallItemStatus = dsListItems.Tables(0) _
                                 .Rows(iCount)("Recall_Status").ToString()
                    If .RecallItemStatus.Equals(Macros.RECALL_ITEM_PICKED) Then
                        .StockCount = "0"
                        .UODCount = "0"
                        .Status = "Actioned"
                    Else
                        .StockCount = " "
                        .UODCount = " "
                    End If
                    strLivePog1 = dsListItems.Tables(0) _
                                 .Rows(iCount)("Live_POG1").ToString()

                    'Minu Removed padleft
                    .StockDateMove = dsListItems.Tables(0) _
                                 .Rows(iCount)("Last_Stock_Movement_Date").ToString()
                    'Fix for BTCPR00004984(POD - reporting 'Unable to load Recall' for certain recalls)
                    'Minu start commenting lines
                    ' If .StockDateMove = "000000" Then
                    '.StockDateMove = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DEFAULT_STOCK_MOVEMENT_DATE)
                    ' End If
                    'Minu end coomenting lines
                    'Minu removed dateexact function and added stock movememnt date directly
                    If isRecallTailored Then
                        RLSessionMgr.GetInstance().SetTailoringFlag(Convert.ToInt32(.TSF), dsListItems.Tables(0) _
                                                                .Rows(iCount)("Last_Stock_Movement_Date"), _
                                                                    .TailoringFlag, strLivePog1)
                    Else
                        .TailoringFlag = False
                    End If
                End With
                arrRecallItems.Add(objArrayItem)
            Next
            'For handling unknown item.
            If Not arrRecallItems.Count.Equals(iRecallQty) Then
                dsMissingItems = New DataSet()
                Dim objRLRecallInfo As RLItemInfo
                iRecallQty = iRecallQty - iCount
                Dim strBootsCode As String = Nothing
                'Get the recall items that are missing in the planner.
                strCmd = String.Format(strCmd, strRecallNo, strRecallNo)
                dsMissingItems = objAppContainer.objDBConnection.RunSQLGetDataSet(strCmd)
                iCount = 0
                iRecallQty = dsMissingItems.Tables(0).Rows().Count
                'Add all the missing items found to the array list.
                For iIndex As Integer = iCount To iRecallQty - 1
                    objRLRecallInfo = New RLItemInfo
                    objRLRecallInfo.BootsCode = dsMissingItems.Tables(0).Rows(iIndex)("Boots_Code").ToString()
                    strBootsCode = objRLRecallInfo.BootsCode.Substring(0, 6)
                    'Create a first barcode out of boots code and assign to product code.
                    objRLRecallInfo.ProductCode = strBootsCode.PadLeft(12, "0")
                    'objRLRecallInfo.ProductCode = objAppContainer.objHelper.GeneratePCwithCDV(objRLRecallInfo.ProductCode)
                    'objRLRecallInfo.FirstBarcode = objAppContainer.objHelper.GeneratePCwithCDV(objRLRecallInfo.ProductCode)
                    objRLRecallInfo.FirstBarcode = strBootsCode.PadLeft(12, "0")
                    objRLRecallInfo.Description = "Item Not On File"
                    objRLRecallInfo.Status = "Unactioned"
                    objRLRecallInfo.TSF = "0"
                    objRLRecallInfo.ItemPrice = "0"
                    objRLRecallInfo.RecallItemStatus = dsMissingItems.Tables(0).Rows(iIndex)("Recall_Status").ToString()
                    If objRLRecallInfo.RecallItemStatus.Equals(Macros.RECALL_ITEM_PICKED) Then
                        objRLRecallInfo.StockCount = "0"
                        objRLRecallInfo.UODCount = "0"
                    Else
                        objRLRecallInfo.StockCount = " "
                        objRLRecallInfo.UODCount = " "
                    End If
                    objRLRecallInfo.TailoringFlag = False
                    objRLRecallInfo.StockDateMove = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DEFAULT_STOCK_MOVEMENT_DATE)
                    arrRecallItems.Add(objRLRecallInfo)
                Next
            End If
            If arrRecallItems.Count > 0 Then
                bTemp = True
            Else
                bTemp = False
            End If
        Catch ex As Exception
            'return false in case of any exception.
            Return False
        Finally
            dsListItems = Nothing
            dsMissingItems = Nothing
            strSqlCmd = Nothing
            strCmd = Nothing

            'dslistItemTailrd = Nothing
            objItemsinPlanner.Clear()
            objItemsinPlanner = Nothing
        End Try
        'If control reaches here after executing successfully, return true.
        Return bTemp
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="arrSupplierList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSupplierList(ByRef arrSupplierList As ArrayList, ByVal strSupplierBC As String) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_SUPPLIER_ITEMS
        strSqlCmd = String.Format(strSqlCmd, strSupplierBC)
        Dim objSupplierList As SupplierList
        Dim dsList As DataSet = Nothing
        Dim sqlResultSet As SqlCeDataReader
        Try
            dsList = New DataSet()
            dsList = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmd)
            'Traverse through each element of the data set and update the details
            'in the object.
            If dsList.Tables(0).Rows.Count > 0 Then
                For iCount As Integer = 0 To dsList.Tables(0).Rows.Count - 1
                    objSupplierList = New SupplierList()
                    With objSupplierList
                        .SupplierID = dsList.Tables(0).Rows(iCount)("Supplier_ID").ToString()
                        .SupplierName = dsList.Tables(0).Rows(iCount) _
                                           ("Supplier_Name").ToString()
                    End With
                    'Add the object with details to the array list.
                    arrSupplierList.Add(objSupplierList)
                Next
            Else
                'return false if the dataset does not have any data.
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            sqlResultSet = Nothing
            dsList = Nothing
        End Try
        'if all the execution part is comepleted successfully, return true.
        Return True
    End Function
    ''' <summary>
    ''' Check if the product scanned is a valid product using Boots Code.
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckIsProductValidUsingBC(ByVal strBootsCode As String) As Boolean
        Dim strSqlCmd As String = QueryMacro.VALIDATE_PRODUCT_USING_BC
        strSqlCmd = String.Format(strSqlCmd, strBootsCode)
        Dim sqlResultSet As SqlCeDataReader
        Try
            'execute the query
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            sqlResultSet = Nothing
        End Try
    End Function
    ''' <summary>
    ''' Check if the product scanned is a valid product using Product Code.
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckIsProductValidUsingPC(ByVal strProductCode As String) As Boolean
        Dim strSqlCmd As String = QueryMacro.VALIDATE_PRODUCT_USING_PC
        strSqlCmd = String.Format(strSqlCmd, strProductCode)
        Dim sqlResultSet As SqlCeDataReader
        Try
            'execute the query
            sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            sqlResultSet = Nothing
        End Try
    End Function
    ''' <summary>
    ''' To Add for User Auth 
    ''' </summary>
    ''' <param name="strUserID"></param>
    ''' <param name="objUserInfo"></param>
    ''' <returns></returns>
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
                    .UserName = sqlResultSet("User_Name").ToString()
                    .StockAccess = sqlResultSet("Stock_Adjustment_Flag").ToString()
                End With
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            sqlResultSet = Nothing
        End Try
        'If successfully updated the details.
        Return True
    End Function
    ''' <summary>
    ''' To Add for User Auth 
    ''' </summary>
    ''' <param name="strUserID"></param>
    ''' <param name="objUserInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckIteminModuleListItems(ByVal strBootsCode As String, ByVal iCount As Integer, _
                                               ByRef objItemsinPlanner As Hashtable, Optional ByVal bCheckFullPlanner As Boolean = False) As Boolean
        Dim strSqlCmdTailrd As String = QueryMacro.GET_BOOTCODE_FROM_MODULELISTITEMS
        Dim dslistItemTailrd As DataSet = Nothing
        Dim strSqlCmd As String = QueryMacro.GET_BOOTCODE_MODULELISTITEMS
        strSqlCmd = String.Format(strSqlCmd, strBootsCode)
        Dim sqlResultSet As SqlCeDataReader
        '  Dim objItemsinPlanner As Hashtable = Nothing
        Try

            ''''''''''''''''
            If bCheckFullPlanner Then
                If iCount = 0 Then
                    dslistItemTailrd = New DataSet()
                    dslistItemTailrd = objAppContainer.objDBConnection.RunSQLGetDataSet(strSqlCmdTailrd)
                    Dim iItemsinPlanner As Integer = dslistItemTailrd.Tables(0).Rows.Count
                    objItemsinPlanner = New Hashtable
                    Dim strTemp As String
                    For iCnt As Integer = 0 To iItemsinPlanner - 1
                        strTemp = dslistItemTailrd.Tables(0).Rows(iCnt)("Boots_Code").ToString()
                        objItemsinPlanner.Add(Convert.ToInt32(strTemp), strTemp)

                    Next
                End If
                If objItemsinPlanner.Contains(strBootsCode) Then
                    Return True
                Else
                    Return False
                End If
            Else
                'execute the command to get the user details.
                sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
                'process dataset and retrieve the details in it.
                If sqlResultSet.Read Then
                    'Get the item details.
                    Return True
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            Return False
        Finally
            dslistItemTailrd = Nothing
        End Try

        '    Dim strSqlCmd As String = QueryMacro.GET_BOOTCODE_MODULELISTITEMS
        '    strSqlCmd = String.Format(strSqlCmd, strBootsCode)
        '    Dim sqlResultSet As SqlCeDataReader
        '    Try
        '        'execute the command to get the user details.
        '        sqlResultSet = objAppContainer.objDBConnection.ExecuteSQLQuery(strSqlCmd)
        '        'process dataset and retrieve the details in it.
        '        If sqlResultSet.Read Then
        '            'Get the item details.
        '            Return True
        '        Else
        '            Return False
        '        End If

        '    Catch ex As Exception
        '        Return False
        '    Finally
        '        sqlResultSet = Nothing
        '    End Try
    End Function
End Class
#End If