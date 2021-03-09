Imports System
Imports System.Data
Imports System.Text.RegularExpressions
#If NRF Then
Imports System.Data.SqlServerCe
#End If
Imports System.Threading

#If RF Then
'''******************************************************************************
''' <FileName>SMRFDataSource.vb</FileName>
''' <summary>
''' This class is the data source class for the Shelf Management application 
''' to source data from TRANSACT in RF world using the messaging protocol.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>18-Oct-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70/PPC</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''******************************************************************************

Public Class SMRFDataSource
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
        Dim objResponse As Object = Nothing
        If objAppContainer.objExportDataManager.CreateCLR() Then
            If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                If TypeOf (objResponse) Is NAKRecord Then
                    Return False
                Else
                    Dim objCLL As CLLRecord
                    Dim objCountList As CLProductGroupInfo = Nothing
                    Do
                        objCLL = CType(objResponse, CLLRecord)
                        objCountList = New CLProductGroupInfo()
                        With objCountList
                            .ListDescription = objCLL.strBusinessGroup.Trim()
                            .ListID = Convert.ToInt16(objCLL.strReturnedListID)
                            .ListType = objCLL.cListType
                            .NumberOfItems = objCLL.strTotalItems
                            .BackshopCount = objCLL.strNo_BS_Items
                            .SalesFloorCount = objCLL.strNo_SF_Items
                            .OSSRCount = objCLL.strOSSR_Item
                            .ActiveType = objCLL.cActiveFlag
                            .DateOfLastCount = objCLL.strLastCntDate
                            .CounterID = objCLL.strCounterID
                        End With
                        arrObjectList.Add(objCountList)
                        objCLL = Nothing
                        objCountList = Nothing
                    Loop While (DATAPOOL.getInstance.GetNextObject(objResponse))
                    Return True
                End If
            End If
        End If
    End Function
 ''' <summary>
    ''' Gets the listid for create own list.
    ''' </summary>
    ''' <param name="strStatus"></param>
    ''' <returns>Bool
    ''' True - If successfully recevied and updated the details in the object 
    ''' array.
    ''' False - If any occured during the updation.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetCreateCountListID(ByVal strStatus As String, ByRef objCountList As CLProductGroupInfo) As Boolean
        Dim objResponse As Object = Nothing
        If objAppContainer.objExportDataManager.CreateCLA(strStatus) Then
            If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                If TypeOf (objResponse) Is NAKRecord Then
                    Return False
                Else
                    Dim objCLB As CLBRecord
                    'Do
                    objCLB = CType(objResponse, CLBRecord)
                    'objCountList = New CLProductGroupInfo()
                    With objCountList
                        .ListID = objCLB.strListID
                    End With
                    objCLB = Nothing
                    'Loop While (DATAPOOL.getInstance.GetNextObject(objResponse))
                    Return True
                End If
            End If
        End If
    End Function
    ''' <summary>
    ''' Gets the details of all items in the Count list selected by the user.
    ''' Update details of each items in an object in the array list.
    ''' </summary>
    ''' <param name="strListID">List ID of a count list</param>
    ''' <param name="arrCLProductInfo">Array of objects to update the item 
    ''' details</param>
    ''' <returns>Bool
    ''' True - If successfully recevied and updated the details in 
    ''' the object array.
    ''' False - If any occured during the updation.</returns>
    ''' <remarks></remarks>
    Public Function GetCountListItemDetails(ByVal strListID As String, ByRef arrCLProductInfo As ArrayList, ByRef strErrMsg As String) As Boolean
        Dim objCLS As New CLSRecord()
        'Stock File Accuracy RF-  added new variable to store ossr status
        Dim cOSSR As Char
        Dim iSalesFloorQty As Integer
        Dim iBackShopQty As Integer
        Dim iBSPSPQty As Integer
        Dim iOSSRBSQty As Integer
        Dim iOSSRPSPQty As Integer
        objCLS.strListID = strListID
        Dim objCountList As CLProductInfo = Nothing
        If objAppContainer.objExportDataManager.CreateCLS(objCLS) Then
            Dim objCLI As CLIRecord
            Dim objResponse As Object = Nothing
            Dim objItem As Object = Nothing
            If (DATAPOOL.getInstance.GetNextObject(objResponse)) Then
                If TypeOf (objResponse) Is ArrayList Then
                    For Each objItem In objResponse
                        If TypeOf (objItem) Is CLIRecord Then
                            objCLI = CType(objItem, CLIRecord)
                            objCountList = New CLProductInfo
                            With objCountList
                                .BackShopQuantity = objCLI.iBSMBS_Count
                                .BackShopMBSQuantity = objCLI.iBSMBS_Count
                                .BackShopPSPQuantity = objCLI.iBSPSP_Count
                                .SalesFloorQuantity = objCLI.iSF_Count
                                .OSSRQuantity = objCLI.iOSSRMBS_Count
                                .OSSRMBSQuantity = objCLI.iOSSRMBS_Count
                                .OSSRPSPQuantity = objCLI.iOSSRPSP_Count
                                .BootsCode = objCLI.strBootsCode
                                .FirstBarcode = objCLI.strBootsCode
                                .Description = objCLI.strSEL_Desc
                                .ProductCode = objCLI.strBarcode
                                If .ProductCode.Length = 13 Then
                                    .ProductCode = .ProductCode.Remove(.ProductCode.Length - 1, 1)
                                End If
                                .SequenceNumber = objCLI.strSequence
                                .ShortDescription = objCLI.strSEL_Desc
                                .Status = objCLI.cStatus
                                .OSSRFlag = objCLI.cOSSR_Flag
                                .ProductGroup = objCLI.strProdGroup
                                iSalesFloorQty = .SalesFloorQuantity
                                iBackShopQty = .BackShopQuantity
                                iBSPSPQty = .BackShopPSPQuantity
                                iOSSRBSQty = .OSSRMBSQuantity
                                iOSSRPSPQty = .OSSRPSPQuantity

                                If (iSalesFloorQty < 0) Then
                                    iSalesFloorQty = 0
                                End If
                                If (iBackShopQty < 0) Then
                                    iBackShopQty = 0
                                End If
                                If (iBSPSPQty < 0) Then
                                    iBSPSPQty = 0
                                End If
                                If .OSSRFlag = "O" Then
                                    If (iOSSRBSQty < 0) Then
                                        iOSSRBSQty = 0
                                    End If
                                    If (iOSSRPSPQty < 0) Then
                                        iOSSRPSPQty = 0
                                    End If
                                End If
                                If objAppContainer.OSSRStoreFlag = "Y" Then
                                    If .OSSRFlag = "O" Then
                                        If .SalesFloorQuantity < 0 And .BackShopQuantity < 0 And .BackShopPSPQuantity < 0 And _
                                                                       .OSSRQuantity < 0 And .OSSRPSPQuantity < 0 Then
                                            .TotalQuantity = .SalesFloorQuantity + .BackShopQuantity + .BackShopPSPQuantity + _
                                                             .OSSRQuantity + .OSSRPSPQuantity
                                        Else
                                            .TotalQuantity = iSalesFloorQty + iBackShopQty + iBSPSPQty + iOSSRBSQty + iOSSRPSPQty
                                        End If
                                    Else
                                        If .SalesFloorQuantity < 0 And .BackShopQuantity < 0 And .BackShopPSPQuantity < 0 Then
                                            .TotalQuantity = .SalesFloorQuantity + .BackShopQuantity + .BackShopPSPQuantity
                                        Else
                                            .TotalQuantity = iSalesFloorQty + iBackShopQty + iBSPSPQty
                                        End If
                                    End If
                                Else
                                    If .SalesFloorQuantity < 0 And .BackShopQuantity < 0 And .BackShopPSPQuantity < 0 Then
                                        .TotalQuantity = .SalesFloorQuantity + .BackShopQuantity + .BackShopPSPQuantity
                                    Else
                                        .TotalQuantity = iSalesFloorQty + iBackShopQty + iBSPSPQty
                                    End If
                                End If
                                If Not objCLI.strPendSale_Flag.ToString().Trim(" ") = "" Then
                                    If objCLI.strPendSale_Flag = "y" Or objCLI.strPendSale_Flag = "Y" Then
                                        .PendingSalesFlag = True
                                    Else
                                        .PendingSalesFlag = False
                                    End If
                                Else
                                    .PendingSalesFlag = False
                                End If
                                If Not objCLI.strStockFigure.Trim(" ") = "" Then
                                    .TSF = Convert.ToInt16(objCLI.strStockFigure)
                                Else
                                    .TSF = "0"
                                End If
                            End With
                            arrCLProductInfo.Add(objCountList)
                        End If
                    Next
                    Return True
                Else
                    'Handle Nak here
                    Dim objNAK As NAKRecord = Nothing
                    If TypeOf (objResponse) Is NAKRecord Then
                        objNAK = CType(objResponse, NAKRecord)
                        strErrMsg = objNAK.strErrorMessage.ToString()
                    End If
                    Return False
                End If
                objCLI = Nothing
                objCountList = Nothing
            End If
        Else
            Return False
        End If
        'Return True
    End Function
    ''' <summary>
    ''' To get Pending Sales Plan flag for a product using Boots Code
    ''' </summary>
    ''' <param name="objProductInfo"></param>
    ''' <returns>
    ''' BarCode - If Product Bar code is available for the Boots code entered.
    ''' 0 - If any error occured while trying to get data from DB.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetPendingSalesFlag(ByRef objProductInfo As PLProductInfo) As Boolean
        'System testing SFA  added new function for RF functionality for Picking list
        Dim objResponse As Object = Nothing
        Dim objEQR As EQRRecord
        Try
            If objAppContainer.objExportDataManager.CreateENQ(objProductInfo.BootsCode, False) Then
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is EQRRecord Then
                        objEQR = CType(objResponse, EQRRecord)
                        If Not objEQR.cPendSale.ToString().Trim(" ") = "" Then
                            If objEQR.cPendSale = "y" Or objEQR.cPendSale = "Y" Then
                                objProductInfo.PendingSalesFlag = True
                            Else
                                objProductInfo.PendingSalesFlag = False
                            End If
                        Else
                            objProductInfo.PendingSalesFlag = False
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
        End Try
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
        'anoop
        Return GetProductInfoUsingPC(strBootsCode, objSMProductInfo)
        Return True
    End Function
''' <summary>
    ''' Gets the item details required for a product in Create own list 
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
    Public Function GetProductInfoUsingBC(ByVal strBootsCode As String, ByRef objCLProductInfo As CLProductInfo, ByVal objCountList As CLProductGroupInfo, ByRef arrCLObject As ArrayList, ByRef iNakErrorFlag As Boolean) As Boolean
        'anoop
        Return GetProductInfoUsingPC(strBootsCode, objCLProductInfo, objCountList, arrCLObject, iNakErrorFlag)
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
        Return GetProductInfoUsingPC(strBootsCode, objASYSProductInfo)
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
        Return GetProductInfoUsingPC(strBootsCode, objFFProductInfo)
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
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingBC(ByVal strBootsCode As String, ByRef objEXProductInfo As EXProductInfo) As Boolean

        Return GetProductInfoUsingPC(strBootsCode, objEXProductInfo)
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
        Return GetProductInfoUsingPC(strBootsCode, objPSProductInfo)
    End Function
    ''' <summary>
    ''' Gets the item details required for a product in Create own list 
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
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, ByRef objCLProductInfo As CLProductInfo, ByVal objCountList As CLProductGroupInfo, ByRef arrCLObject As ArrayList, ByRef iNakErrorFlag As Boolean) As Boolean

        Try
            arrCLObject = New ArrayList
            Dim objCLD As CLDRecord = New CLDRecord
            Dim strPSPFlag As String = Nothing
            Dim strTemp As String = Nothing
            objCLD.strListID = objCountList.ListID
            objCLD.strBootsCode = strProductCode
            objCLD.strSequence = objCountList.SeqNumber
            objCLD.cSitetype = objCountList.SiteType
            If objAppContainer.objExportDataManager.CreateCLD(objCLD) Then
                Dim objCLI As CLIRecord
                Dim objData As Object = Nothing
                Dim objItem As Object = Nothing
                If (DATAPOOL.getInstance.GetNextObject(objData)) Then
                    If TypeOf (objData) Is ArrayList Then
                        For Each objItem In objData
                            If TypeOf (objItem) Is CLIRecord Then
                                objCLI = CType(objItem, CLIRecord)
                                objCLProductInfo = New CLProductInfo
                                With objCLProductInfo
                                    .BootsCode = objCLI.strBootsCode
                                    .FirstBarcode = objCLI.strBarcode
                                    .Description = objCLI.strSEL_Desc
                                    .Status = objCLI.cStatus
                                    .ProductCode = objCLI.strBarcode
                                    .LastCountDate = objCLI.strDateLastCount
                                    .SequenceNumber = objCountList.SeqNumber
                                    .OSSRFlag = objCLI.cOSSR_Flag
                                    If .ProductCode.Length = 13 Then
                                        .ProductCode = .ProductCode.Remove(.ProductCode.Length - 1, 1)
                                    End If
                                    'Def#344 - SFA System Testing
                                    If Not objCLI.strStockFigure.Trim(" ") = "" Then
                                        .TSF = Convert.ToInt16(objCLI.strStockFigure)
                                    Else
                                        .TSF = "0"
                                    End If
                                    strPSPFlag = objCLI.strPendSale_Flag
                                    If Not objCLI.strPendSale_Flag.ToString().Trim(" ") = "" Then
                                        If objCLI.strPendSale_Flag = "y" Or objCLI.strPendSale_Flag = "Y" Then
                                            .PendingSalesFlag = True
                                        Else
                                            .PendingSalesFlag = False
                                        End If
                                    Else
                                        .PendingSalesFlag = False
                                    End If
                                End With
                            Else
                                Return False
                            End If
                            arrCLObject.Add(objCLProductInfo)
                            objCLI = Nothing
                        Next
                    Else
                        If objData.ToString.StartsWith("MCShMon.NAKERROR") Then
                            iNakErrorFlag = True
                        Else
                            iNakErrorFlag = False
                        End If
                        Return False
                    End If
                End If
                objData = Nothing
                objCLI = Nothing
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
            ''Log it here
        End Try
        'anoop:end
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
        'anoop:Start
        Try
            If objAppContainer.objExportDataManager.CreateENQ(strProductCode, False) Then
                Dim objEQR As EQRRecord
                Dim objData As Object = Nothing
                If (DATAPOOL.getInstance.GetNextObject(objData)) Then
                    If TypeOf (objData) Is EQRRecord Then
                        objEQR = CType(objData, EQRRecord)
                        With objSMProductInfo
                            .BootsCode = objEQR.strBootsCode
                            .Description = objEQR.strSELDesc
                            .FirstBarcode = objEQR.strBarcode
                            .ProductCode = objEQR.strBarcode.ToString().Remove(objEQR.strBarcode.Length - 1, 1)
                            .ShortDescription = objEQR.strItemDesc
                            .Status = objEQR.cStatus
                            .OSSRFlag = objEQR.cOSSRFlag
                            .iMultisiteCnt = Val(objEQR.strCorePlannerLoc) + Val(objEQR.strSalesPlanerLoc)
                            .Price = objEQR.strPrice
                            If objEQR.strPriceCHKDone.Trim() <> "" Then
                                .m_completed = Convert.ToInt16(objEQR.strPriceCHKDone)
                            Else
                                .m_completed = 0
                            End If

                            If objEQR.strPriceCHKTarget.Trim() <> "" Then
                                .m_Target = Convert.ToInt16(objEQR.strPriceCHKTarget)
                            Else
                                .m_Target = 0
                            End If
                            .m_PriceAcceptedFlag = objEQR.cFlagPriceCheck
                            '.iMultisiteCnt = CType(objEQR.strCorePlannerLoc, Integer) + CType(objEQR.strSalesPlanerLoc, Integer)
                            If Not objEQR.strStockFigure.Trim(" ") = "" Then
                                .TSF = Convert.ToInt16(objEQR.strStockFigure)
                            Else
                                .TSF = "0"
                            End If
                            .CIPFlag = objEQR.cMarkDown
                            .SupplyRoute = objEQR.cSupply
                            .Advantage = objEQR.cReedemable
                        End With
                    Else
                        Return False
                    End If
                End If
                objData = Nothing
                objEQR = Nothing
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
            ''Log it here
        End Try
        'anoop:end
        Return True
    End Function
    ''' <summary>
    ''' Gets the item details required for a product in count list 
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
    Public Function GetCLProductInfo(ByVal strProductCode As String, ByRef objCLProductInfo As CLProductInfo) As Boolean
        Try
            If objAppContainer.objExportDataManager.CreateENQ(strProductCode, False) Then
                Dim objEQR As EQRRecord
                Dim objData As Object = Nothing
                If (DATAPOOL.getInstance.GetNextObject(objData)) Then
                    If TypeOf (objData) Is EQRRecord Then
                        objEQR = CType(objData, EQRRecord)
                        With objCLProductInfo
                            .BootsCode = objEQR.strBootsCode
                            .Description = objEQR.strSELDesc
                            .FirstBarcode = objEQR.strBarcode
                            .ProductCode = objEQR.strBarcode.ToString().Remove(objEQR.strBarcode.Length - 1, 1)
                            .ShortDescription = objEQR.strItemDesc
                            .Status = objEQR.cStatus
                            .OSSRFlag = objEQR.cOSSRFlag
                            .Price = objEQR.strPrice
                            If Not objEQR.strStockFigure.Trim(" ") = "" Then
                                .TSF = Convert.ToInt16(objEQR.strStockFigure)
                            Else
                                .TSF = "0"
                            End If
                            .CIPFlag = objEQR.cMarkDown
                            .SupplyRoute = objEQR.cSupply
                            .Advantage = objEQR.cReedemable
                        End With
                    Else
                        Return False
                    End If
                End If
                objData = Nothing
                objEQR = Nothing
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
            ''Log it here
        End Try
        'anoop:end
        Return True
    End Function
    ''' <summary>
    ''' Gets the item details required for printing SEL or Clearance label.
    ''' </summary>
    ''' <param name="objPSProductInfo">Object to be updated.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Public Function GetLabelDetails(ByRef objPSProductInfo As PSProductInfo) As Boolean
        Try
            Dim objLPR As LPRRecord
            Dim objData As Object = Nothing
            If objAppContainer.objExportDataManager.CreatePRT(objPSProductInfo.BootsCode, SMTransactDataManager.ExFileType.EXData) Then
                'MessageBox.Show("Message sent successfully.")
                If DATAPOOL.getInstance.GetNextObject(objData) Then
                    If TypeOf (objData) Is LPRRecord Then
                        objLPR = CType(objData, LPRRecord)
                        With objPSProductInfo
                            .SELLabelType = objLPR.cPHF_SEL_Type
                            .MSFlag = objLPR.charMS_Flag
                            .UnitMeasure = objLPR.strUnitMeasure
                            .UnitPriceFlag = objLPR.cUnitPriceFlag
                            .UnitQuantity = objLPR.strUnitQuantity
                            .UnitType = objLPR.strUnittype
                            .WasPrice1 = objLPR.WASPrice1
                            .WasPrice2 = objLPR.WASPrice2
                            '.WEEEPrfPrice = objLPR.strWEE_Price
                            '.WEEEFlag = objLPR.cWEEE_Flag
                            .PainKillerMessage = objLPR.strPainKillerMessage
                        End With
                        'MessageBox.Show("Updated LPR data")
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If
            Else
                Return False
            End If
            objLPR = Nothing
            objData = Nothing
            'if the object is populated with required data.
            Return True
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
            'Include logs
        Finally

        End Try
    End Function
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
        Try
            If objAppContainer.objExportDataManager.CreateENQ(strProductCode, False) Then
                Dim objEQR As EQRRecord
                Dim objData As Object = Nothing
                Dim arrPlannerList As New ArrayList()
                If (DATAPOOL.getInstance.GetNextObject(objData)) Then
                    If TypeOf (objData) Is EQRRecord Then
                        objEQR = CType(objData, EQRRecord)
                        With objASYSProductInfo
                            If objEQR.cActiveDeal = "Y" Then
                                .ActiveDeal = "YES"
                            ElseIf objEQR.cActiveDeal = "N" Then
                                .ActiveDeal = "NO"
                            End If
                            .BootsCode = objEQR.strBootsCode
                            .Description = objEQR.strSELDesc
                            .FirstBarcode = objEQR.strParentBC.Substring(0, objEQR.strParentBC.Length - 1).PadLeft(12, "0")
                            .ProductCode = objEQR.strBarcode.Substring(0, objEQR.strBarcode.Length - 1)
                            'Fix-In AutoStuff Your Shelves -The View Screen doent display MS count.
                            .MutiSite = Val(objEQR.strCorePlannerLoc) + Val(objEQR.strSalesPlanerLoc)
                            .ShortDescription = objEQR.strItemDesc
                            .Status = objEQR.cStatus
                            .Price = objEQR.strPrice
                            .PSC = "0"
                            'DEFECT FIX - BTCPR00004169(PPC - Items not on Planner within Stuff Your Shelves, 
                            'that are TSF zero, are reported picked up as requiring a fill of zero)
                            If Me.GetPlannerListUsingBC(objASYSProductInfo.BootsCode, False, arrPlannerList) Then
                                For Each objPlannerInfo As PlannerInfo In arrPlannerList
                                    .PSC = Val(.PSC) + Val(objPlannerInfo.PhysicalShelfQty)
                                Next
                            Else
                                Return False
                            End If

                            If Not objEQR.strStockFigure.Trim(" ") = "" Then
                                .TSF = Convert.ToInt16(objEQR.strStockFigure)
                            Else
                                .TSF = "0"
                            End If
                            .CIPFlag = objEQR.cMarkDown
                            .Advantage = objEQR.cReedemable
                            .SupplyRoute = objEQR.cSupply
                        End With
                    Else
                        Return False
                    End If
                End If
                objData = Nothing
                objEQR = Nothing
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
            ''Log it here
        End Try
        'Return True
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

        '#If STUB Then
        '        objFFProductInfo.BootsCode = "2551934"
        '        objFFProductInfo.Description = "Description of Item will be displayed here "
        '        objFFProductInfo.FillQuantity = "25"
        '        objFFProductInfo.FirstBarcode = "000000255193"
        '        objFFProductInfo.MultiSiteCount = "1"
        '        objFFProductInfo.ProductCode = "502409174702"
        '        objFFProductInfo.ShortDescription = "Zirtek         antihistamine  tablets x7 (G)"
        '        objFFProductInfo.Status = "A"
        '        objFFProductInfo.TSF = "25"
        '#End If
        Try
            If objAppContainer.objExportDataManager.CreateENQ(strProductCode, False) Then
                Dim objEQR As EQRRecord
                Dim objData As Object = Nothing
                If (DATAPOOL.getInstance.GetNextObject(objData)) Then
                    If TypeOf (objData) Is EQRRecord Then
                        objEQR = CType(objData, EQRRecord)
                        With objFFProductInfo
                            .BootsCode = objEQR.strBootsCode
                            .Description = objEQR.strSELDesc
                            .FirstBarcode = objEQR.strParentBC.Substring(0, objEQR.strParentBC.Length - 1).PadLeft(12, "0")
                            'System Testing second barcode display issue in Item details screen
                            '.ProductCode = objEQR.strParentBC
                            '.ProductCode = .FirstBarcode
                            .ProductCode = objEQR.strBarcode.Substring(0, objEQR.strBarcode.Length - 1)
                            .ShortDescription = objEQR.strItemDesc
                            .Price = objEQR.strPrice
                            .Status = objEQR.cStatus
                            'Bug fix for Getting OSSR Status - Lakshmi
                            .OSSRFlag = objEQR.cOSSRFlag
                            If Not objEQR.strStockFigure.Trim(" ") = "" Then
                                .TSF = Convert.ToInt16(objEQR.strStockFigure)
                            Else
                                .TSF = "0"
                            End If
                            .CIPFlag = objEQR.cMarkDown
                            .Advantage = objEQR.cReedemable
                            .SupplyRoute = objEQR.cSupply
                        End With
                    Else
                        Return False
                    End If
                End If
                objData = Nothing
                objEQR = Nothing
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
            ''Log it here
        End Try
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
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, ByRef objEXProductInfo As EXProductInfo) As Boolean
        '#If STUB Then
        '        objEXProductInfo.BootsCode = "2551934"
        '        objEXProductInfo.Description = "Description of Item will be displayed here "
        '        objEXProductInfo.FirstBarcode = "000000255193"
        '        objEXProductInfo.MultiSiteCount = "1"
        '        objEXProductInfo.ProductCode = "502409174702"
        '        objEXProductInfo.ShortDescription = "Zirtek         antihistamine  tablets x7 (G)"
        '        objEXProductInfo.Status = "A"
        '        objEXProductInfo.TSF = "25"
        '        objEXProductInfo.BackShopQty = "12"
        '#End If

        Try
            If objAppContainer.objExportDataManager.CreateENQ(strProductCode, False) Then
                Dim objEQR As EQRRecord
                Dim objData As Object = Nothing
                If (DATAPOOL.getInstance.GetNextObject(objData)) Then
                    If TypeOf (objData) Is EQRRecord Then
                        objEQR = CType(objData, EQRRecord)
                        With objEXProductInfo
                            .BootsCode = objEQR.strBootsCode
                            .Description = objEQR.strSELDesc
                            'Fix for Scanning the same barcode Second Time
                            .FirstBarcode = objEQR.strParentBC.Substring(0, objEQR.strParentBC.Length - 1).PadLeft(12, "0")
                            '.FirstBarcode = objEQR.strParentBC
                            .MultiSiteCount = Val(objEQR.strCorePlannerLoc) + Val(objEQR.strSalesPlanerLoc)
                            .ProductCode = objEQR.strBarcode.Remove(objEQR.strBarcode.Length - 1, 1)
                            .ShortDescription = objEQR.strItemDesc
                            .Status = objEQR.cStatus
                            .Price = objEQR.strPrice
                            .OSSRFlag = objEQR.cOSSRFlag
                            If Not objEQR.strStockFigure.Trim(" ") = "" Then
                                .TSF = Convert.ToInt16(objEQR.strStockFigure)
                            Else
                                .TSF = "0"
                            End If
                            .CIPFlag = objEQR.cMarkDown
                            .Advantage = objEQR.cReedemable
                            .SupplyRoute = objEQR.cSupply
                            If Not objEQR.cPendSale.ToString().Trim(" ") = "" Then
                                If objEQR.cPendSale = "y" Or objEQR.cPendSale = "Y" Then
                                    .PendingSalesFlag = True
                                Else
                                    .PendingSalesFlag = False
                                End If
                            Else
                                .PendingSalesFlag = False
                            End If
                        End With
                    Else
                        Return False
                    End If
                End If
                objData = Nothing
                objEQR = Nothing
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
            ''Log it here
        End Try
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
        Try
            If objAppContainer.objExportDataManager.CreateENQ(strProductCode, True) Then
                Dim objEQR As EQRRecord
                Dim objData As Object = Nothing
                If (DATAPOOL.getInstance.GetNextObject(objData)) Then
                    If TypeOf (objData) Is EQRRecord Then
                        objEQR = CType(objData, EQRRecord)
                        With objPSProductInfo
                            .BootsCode = objEQR.strBootsCode
                            .Description = objEQR.strSELDesc
                            .ProductCode = objEQR.strBarcode.Remove(objEQR.strBarcode.Length - 1, 1)
                            .ShortDescription = objEQR.strItemDesc
                            .Status = objEQR.cStatus
                            .Price = objEQR.strPrice
                            If Not objEQR.strStockFigure.Trim(" ") = "" Then
                                .TSF = Convert.ToInt16(objEQR.strStockFigure)
                            Else
                                .TSF = "0"
                            End If
                            .CurrentPrice = CInt(objEQR.strPrice)
                            .CIPFlag = objEQR.cMarkDown
                            .Advantage = objEQR.cReedemable
                            .SupplyRoute = objEQR.cSupply
                        End With
                    Else
                        Return False
                    End If
                End If
                objData = Nothing
                objEQR = Nothing
                Return True
            Else
                'Add log here
                Return False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            Return False
            ''Log it here
        End Try
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
        Return GetItemDetailsAllUsingPC(strBootsCode, objItemInfo)
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
        Dim objResponse As Object = Nothing
        Dim objEQR As EQRRecord
        Dim strDeal As New System.Text.StringBuilder

        Try
            If objAppContainer.objExportDataManager.CreateENQ(strProductCode, False) Then
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is EQRRecord Then
                        objEQR = CType(objResponse, EQRRecord)
                        ''Include EQR Here
                        With objItemInfo
                            .BootsCode = objEQR.strBootsCode
                            .FirstBarcode = objEQR.strBarcode
                            .ProductCode = objEQR.strBarcode.Substring(0, objEQR.strBarcode.Length - 1)
                            .ShortDescription = objEQR.strSELDesc
                            'SFA UAT 
                            If Not objEQR.strStockFigure.Trim(" ") = "" Then
                                .TSF = Convert.ToInt16(objEQR.strStockFigure)
                            Else
                                .TSF = "0"
                            End If
                            .Status = objEQR.cStatus
                            .Price = objEQR.strPrice
                            .RedemptionFlag = objEQR.cReedemable
                            .Description = objEQR.strSELDesc
                            'Fix for OSSR Flag
                            .OSSRFlag = objEQR.cOSSRFlag
                            If objEQR.cActiveDeal = "y" Or objEQR.cActiveDeal = "Y" Then
                                .ActiveDeal = objEQR.cActiveDeal
                                For Each DealInfo As String In objEQR.arrDealSum
                                    strDeal.Append(DealInfo + ",")
                                Next
                                .DealList = strDeal.ToString().Trim(",").ToString()
                            Else
                                .DealList = ""
                            End If
                            .Advantage = objEQR.cReedemable
                            .CIPFlag = objEQR.cMarkDown
                            .SupplyRoute = objEQR.cSupply
                            .ProductGrp = objEQR.strPGGroup
                            .BCType = objEQR.cBusinessCentre.ToString()
                        End With
                    Else
                        Return False
                    End If
                    objResponse = Nothing
                Else
                    Return False
                End If
                Return True
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            'log the exception
        Finally
            objResponse = Nothing
            objEQR = Nothing
        End Try
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
        Return True
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
        Return True
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
    Public Function GetPriceCheckInfo(ByVal strProductCode As String, ByRef objPCProductInfo As PriceCheckInfo) As Boolean
        Dim bTemp As Boolean = False
        Try
            Dim objEQR As EQRRecord = Nothing
            Dim objResponse As Object = Nothing
            If objAppContainer.objExportDataManager.CreateENQ(strProductCode, True) Then
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is EQRRecord Then
                        objEQR = CType(objResponse, EQRRecord)
                        ''Include EQR Here
                        With objPCProductInfo
                            .BootsCode = objEQR.strBootsCode
                            .ShortDescription = objEQR.strSELDesc
                            .PriceAcceptedFlag = objEQR.cFlagPriceCheck
                            If Not objEQR.strStockFigure.Trim(" ") = "" Then
                                .TSF = Convert.ToInt16(objEQR.strStockFigure)
                            Else
                                .TSF = "0"
                            End If
                            .Status = objEQR.cStatus
                            .Price = objEQR.strPrice
                            .Description = objEQR.strSELDesc
                            If Not objEQR.strPriceCHKTarget.Trim(" ") = "" Then
                                .PCTarget = Convert.ToInt32(objEQR.strPriceCHKTarget)
                            Else
                                .PCTarget = 0
                            End If

                            If Not objEQR.strPriceCHKDone.Trim(" ") = "" Then
                                .PCComplete = Convert.ToInt32(objEQR.strPriceCHKDone)
                            Else
                                .PCComplete = 0
                            End If
                            .RejectMessage = objEQR.strRejectMessage
                        End With
                        bTemp = True
                    End If
                End If
                objEQR = Nothing
                objResponse = Nothing
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
        End Try
        Return bTemp
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
        Dim bTemp As Boolean = False
        Try
            If objAppContainer.objExportDataManager.CreatePCS() Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is PCRRecord Then
                        objPCTargetDetails.PriceCheckCompleted = CType(objResponse, PCRRecord).strPriceChkDone
                        objPCTargetDetails.PriceCheckTarget = CType(objResponse, PCRRecord).strPriceChkTarget
                        bTemp = True
                    End If
                End If
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("Exception at get price check details", Logger.LogLevel.RELEASE)
        End Try
        Return bTemp
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
        'anoop:Start
        Dim objResponse As Object = Nothing
        Dim objSNR As SNRRecord
        Try
            DATAPOOL.getInstance.ResetPoolData()
            If objAppContainer.objExportDataManager.CreateSOR(strUserID) Then
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is SNRRecord Then
                        objSNR = CType(objResponse, SNRRecord)
                        With objUserInfo
                            .authorityFlag = objSNR.cAuthorityFlag
                            .dateTime = objSNR.strDateTime
                            .prtDesc = objSNR.strPrinterDescription
                            .prtNum = objSNR.strPrinterNumber
                            .ossrFlag = objSNR.cOSSRFlag
                            .user_ID = objSNR.strOperatorID
                            .user_Name = objSNR.strUserName
                            .stockAccess = objSNR.cStockAccess
                        End With
                    Else
                        Return False
                    End If
                    objResponse = Nothing
                    objSNR = Nothing
                Else
                    Return False
                End If
                Return True
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            'log the exception
        Finally
            objResponse = Nothing
            objSNR = Nothing
        End Try
        'anoop:end

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
        Dim bTemp As Boolean = False
        Dim objResponse As Object = Nothing
        'Dim objDealDetails As DQRRECORD
        Try
            DATAPOOL.getInstance.ResetPoolData()
            If objAppContainer.objExportDataManager.CreateDNQ(strDealNo.Substring(0, 4)) Then
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is DQRRECORD Then
                        objDealDetails = CType(objResponse, DQRRECORD)
                        objDealDetails.strDealType = strDealNo.Substring(4, 2)
                        bTemp = True
                    End If
                End If
            End If
            Return True
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
        Finally
            objResponse = Nothing
        End Try
        Return bTemp
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
        Dim objResponse As Object = Nothing
        If objAppContainer.objExportDataManager.CreatePLR() Then
            If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                If TypeOf (objResponse) Is NAKRecord Then
                    Return False
                ElseIf TypeOf (objResponse) Is NAKERRORRecord Then
                    Return False
                Else
                    Dim objPickingList As PickingList = Nothing
                    Dim objPLL As PLLRecord
                    Do
                        objPLL = CType(objResponse, PLLRecord)
                        If CInt(objPLL.strLines) > 0 And Not objPLL.cStatus.Equals("P") Then
                            objPickingList = New PickingList()
                            With objPickingList
                                .ListID = objPLL.strListID
                                .ListStatus = objPLL.cStatus
                                .ListTime = objPLL.DateTime
                                .Creator = objPLL.strDisplayName
                                .TotalItems = objPLL.strLines
                                '.Location = objPLL.cStockLocation
                                .ListType = objPLL.cStockLocation
                                .UserID = objAppContainer.strCurrentUserID
                                .PickerID = objPLL.strPickerID
                            End With
                            arrObjectList.Add(objPickingList)
                            objPLL = Nothing
                            objPickingList = Nothing
                        End If
                    Loop While (DATAPOOL.getInstance.GetNextObject(objResponse))
                    Return True
                End If
            End If
        End If
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
        Dim bStatus As Boolean = Nothing
        Select Case strListType
            Case Macros.SHELF_MONITOR_PL
                'update the picking list details for SM type.
                bStatus = UpdateSMPickingList(arrPickingList, strListID)
            Case Macros.FAST_FILL_PL
                'update the picking list details for FF type.
                bStatus = UpdateFFPickingList(arrPickingList, strListID)
            Case Macros.AUTO_FAST_FILL_PL
                'update the picking list details for FF type.
                bStatus = UpdateAFFPickingList(arrPickingList, strListID)
            Case Macros.EXCESS_STOCK_PL
                'update the picking list details for EX type.
                bStatus = UpdateSMPickingList(arrPickingList, strListID)
            Case Macros.EXCESS_STOCK_PL_SF
                'update the picking list details for EX type.
                bStatus = UpdateEXPickingList(arrPickingList, strListID)
            Case Macros.OSSR_PL
                bStatus = UpdateOSSRPickingList(arrPickingList, strListID)
            Case Macros.EXCESS_STOCK_PL_OSSR
                bStatus = UpdateEXPickingList(arrPickingList, strListID)
            Case Macros.EXCESS_STOCK_OSSR
                bStatus = UpdateOSSRPickingList(arrPickingList, strListID)
        End Select
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
    Public Function GetPOGCategoryList(ByVal strCore As String, ByRef arrCatergoryList As ArrayList, ByVal isPending As Boolean) As Boolean
        Dim bTemp As Boolean = False
        Dim objResponse As Object = Nothing
        If objAppContainer.objExportDataManager.CreatePGF(strCore, isPending) Then
            While DATAPOOL.getInstance.GetNextObject(objResponse)
                If TypeOf (objResponse) Is PGGRecord Then
                    Dim objPGGRecord As PGGRecord = CType(objResponse, PGGRecord)
                    Dim objCategoryInfo As New CategoryInfo()
                    With objCategoryInfo
                        .CategoryID = objPGGRecord.strSequence
                        .Description = objPGGRecord.strDescription
                        .POINTER = objPGGRecord.strPOG_StartPtr
                    End With
                    arrCatergoryList.Add(objCategoryInfo)
                    objCategoryInfo = Nothing
                    objPGGRecord = Nothing
                    objResponse = Nothing
                Else
                    arrCatergoryList.Clear()
                    DATAPOOL.getInstance.ResetPoolData()
                    objResponse = Nothing
                    Return False
                End If
            End While
            If arrCatergoryList.Count > 0 Then
                Return True
            Else
                Return False
            End If
        End If
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
        If objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.SHLFMNTR Or _
         objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PICKGLST Or _
         objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.CUNTLIST Or _
         objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.AUTOSTUFFYOURSHELVES Then
            Return GetMultisiteInfo(strBootsCode, bIsMultisiteCall, arrPlannerList)
        Else
            Return GetPlannerDetails(strBootsCode, arrPlannerList)
        End If
    End Function
    ''' <summary>
    ''' Function to get planner details.
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <param name="arrPlannerList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPlannerDetails(ByVal strBootsCode As String, ByRef arrPlannerList As ArrayList) As Boolean
        Dim bTemp As Boolean = False
        Dim objResponse As Object = Nothing
        Dim objPGI As PGIRecord = Nothing
        Dim objPlannerInfo As PlannerInfo
        Try
            If objAppContainer.objExportDataManager.CreatePGL(strBootsCode) Then
                Do While DATAPOOL.getInstance.GetNextObject(objResponse)
                    If TypeOf (objResponse) Is PGIRecord Then
                        objPGI = CType(objResponse, PGIRecord)
                        objPlannerInfo = New PlannerInfo()
                        'For Planners Only Module ID, Description and Date  is Needed
                        'This logic Corresponds to UpdatePlannerList(ByVal ArrayList, ByRef ArrayList) function of MC70
                        With objPlannerInfo
                            .PlannerID = objPGI.strPOGKEY
                            .Description = objPGI.strPOGDESC.Trim()
                            'Hard Coding the Rebuild Date 
                            'Fix for System Testing:For RF No Rebuild Date required.
                            '.RebuildDate = "1234567890".Trim()
                            .RebuildDate = "  "
                        End With
                        arrPlannerList.Add(objPlannerInfo)
                        objPGI = Nothing
                        objResponse = Nothing
                        objPlannerInfo = Nothing
                    Else
                        'Else Condition is Negative Acknowledgement
                        objResponse = Nothing
                        arrPlannerList.Clear()
                        DATAPOOL.getInstance.ResetPoolData()
                    End If
                Loop
            End If
            If arrPlannerList.Count > 0 Then
                bTemp = True
                'sort the planner list
                Dim iComp As PogCompare = New PogCompare()
                arrPlannerList.Sort(iComp)
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("Error in Getting Planner info using BC ::" + ex.Message, Logger.LogLevel.RELEASE)
        End Try

        Return bTemp
    End Function

    Public Function GetMultisiteInfo(ByVal strBootsCode As String, ByVal bIsMultisiteCall As Boolean, ByRef arrPlannerList As ArrayList) As Boolean
        'Get multisite location details for SM, FF, EX and picking list.
        'Used for count List also to fetch the multisite details for item
        'For count list, we will get  plannerId in transact and according to the plannerID
        'we need to retrieve module description for the items.
        strBootsCode = strBootsCode.Substring(0, 6)
        Dim bTemp As Boolean = False
        Dim objResponse As Object = Nothing
        Dim objPGB As New PGBRecord
        Dim objPlannerInfo As PlannerInfo
        Try
            DATAPOOL.getInstance.ResetPoolData()
            If objAppContainer.objExportDataManager.CreatePGA(strBootsCode) Then
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    Do
                        If TypeOf (objResponse) Is NAKRecord Then
                            DATAPOOL.getInstance.ResetPoolData()
                            Return False
                        End If
                        objPGB = CType(objResponse, PGBRecord)
                        objPlannerInfo = New PlannerInfo
                        objPlannerInfo.PlannerID = objPGB.strPOGKey
                        objPlannerInfo.Description = objPGB.strModuleDesc
                        objPlannerInfo.POGDesc = objPGB.strPOGDesc
                        objPlannerInfo.PhysicalShelfQty = objPGB.strPSC
                        objPlannerInfo.RepeatCount = objPGB.strRepeatCount
                        arrPlannerList.Add(objPlannerInfo)
                        objPlannerInfo = Nothing
                    Loop While (DATAPOOL.getInstance.GetNextObject(objResponse))
                End If
            Else
                objAppContainer.objLogger.WriteAppLog("Cannot Create PGA record at SM Start Session", Logger.LogLevel.RELEASE)
                Return False
            End If
            Return True
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
        Finally
            objResponse = Nothing
            objPGB = Nothing
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
        Dim bTemp As Boolean = False
        Dim objIteminfo As New ItemInfo
        If GetItemDetailsAllUsingPC(strProductCode, objIteminfo) Then
            bTemp = GetPlannerDetails(objIteminfo.BootsCode, arrPlannerList)
            ''Fix for EX Muti Sited item
            'bTemp = GetMultisiteInfo(objIteminfo.BootsCode, bIsMultisiteCall, arrPlannerList)
        End If
        objIteminfo = Nothing
        Return bTemp
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
        Dim bTemp As Boolean = False
        Dim objResponse As Object = Nothing
        Try
            If objAppContainer.objExportDataManager.CreatePGM(strPOGID) Then
                While DATAPOOL.getInstance.GetNextObject(objResponse)
                    If TypeOf (objResponse) Is PGNRecord Then
                        Dim objPGNRecord As PGNRecord = CType(objResponse, PGNRecord)
                        Dim objModuleInfo As New ModuleInfo()
                        With objModuleInfo
                            .ModuleID = strPOGID
                            .Description = objPGNRecord.strMODDesc.TrimEnd(" ")
                            .SequenceNumber = objPGNRecord.strMODSeq
                            .SHELFCOUNT = objPGNRecord.strShelfCount
                        End With
                        arrModuleList.Add(objModuleInfo)
                        objPGNRecord = Nothing
                        objResponse = Nothing
                    Else
                        arrModuleList.Clear()
                        DATAPOOL.getInstance.ResetPoolData()
                        objResponse = Nothing
                        Return False
                    End If
                End While
                If arrModuleList.Count > 0 Then
                    bTemp = True
                End If
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
        End Try
        Return bTemp
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
        Dim bTemp As Boolean = False
        Dim objResponse As Object = Nothing
        Try
            If objAppContainer.objExportDataManager.CreatePGM(strPOGID, strBootsCode) Then
                While DATAPOOL.getInstance.GetNextObject(objResponse)
                    If TypeOf (objResponse) Is PGNRecord Then
                        Dim objPGNRecord As PGNRecord = CType(objResponse, PGNRecord)
                        If objPGNRecord.cFilter = "Y" Then
                            Dim objModuleInfo As New ModuleInfo()
                            With objModuleInfo
                                .ModuleID = strPOGID 'Test
                                .Description = objPGNRecord.strMODDesc
                                .SequenceNumber = objPGNRecord.strMODSeq 'Test
                                .SHELFCOUNT = objPGNRecord.strShelfCount
                            End With
                            arrModuleList.Add(objModuleInfo)
                            objModuleInfo = Nothing
                        End If
                        objPGNRecord = Nothing
                        objResponse = Nothing
                    Else
                        arrModuleList.Clear()
                        DATAPOOL.getInstance.ResetPoolData()
                        objResponse = Nothing
                        Return False
                    End If
                End While
            End If
            If arrModuleList.Count >= 1 Then
                bTemp = True
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
        End Try
        Return bTemp
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
        Dim bTemp As Boolean = False
        Dim objResponse As Object = Nothing
        If objAppContainer.objExportDataManager.CreatePSL(strModuleID, strSequenceNumber) Then
            Do While DATAPOOL.getInstance.GetNextObject(objResponse)
                If TypeOf (objResponse) Is PSRRecord Then
                    Dim objPSRRecord As PSRRecord = CType(objResponse, PSRRecord)
                    Dim objLineList As SPLineListInfo
                    objLineList = New SPLineListInfo
                    With objLineList
                        'Item Descripton
                        .FaceCount = objPSRRecord.strFacings.Trim("0")
                        .ItemCode = objAppContainer.objHelper.GenerateBCwithCDV(objPSRRecord.strBootsCode)
                        .ItemDescription = objPSRRecord.strItemDesc.Trim()
                        'Shelf Description
                        'Fix for 0 trim in sequence no
                        .ModuleSeqNumber = objPSRRecord.strModuleID
                        .NotchNumber = objPSRRecord.strNotchNo
                        .ShelfDesc = objPSRRecord.strShelfDesc.Trim()
                        .ShelfNumber = objPSRRecord.strShelfNumber
                    End With
                    arrLineList.Add(objLineList)
                    objPSRRecord = Nothing
                    objResponse = Nothing
                Else
                    arrLineList.Clear()
                    DATAPOOL.getInstance.ResetPoolData()
                    objResponse = Nothing
                    Return False
                End If

            Loop
            bTemp = True
        End If
        Return bTemp
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
    Public Function GetPlannerListForCategory(ByVal strCategoryID As String, ByRef arrPlannerList As ArrayList, ByVal isPendingPlanner As Boolean) As Boolean
        Dim objResponse As Object = Nothing
        If objAppContainer.objExportDataManager.CreatePGQ(strCategoryID, isPendingPlanner) Then
            While DATAPOOL.getInstance.GetNextObject(objResponse)
                If TypeOf (objResponse) Is PGRRecord Then
                    Dim objPGRRecord As PGRRecord = CType(objResponse, PGRRecord)
                    Dim objPlannerInfo As New PlannerInfo()
                    With objPlannerInfo
                        .Description = objPGRRecord.strPOGDesc.TrimEnd(" ")
                        .PlannerID = objPGRRecord.strPOG_IndexPtr
                        .RebuildDate = objPGRRecord.strActDate.Trim()
                    End With
                    arrPlannerList.Add(objPlannerInfo)
                    objPlannerInfo = Nothing
                    objPGRRecord = Nothing
                    objResponse = Nothing
                Else
                    arrPlannerList.Clear()
                    DATAPOOL.getInstance.ResetPoolData()
                    objResponse = Nothing
                    Return False
                End If
            End While
        End If
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
        Dim objResponse As Object = Nothing
        Dim objEQR As EQRRecord
        Try
            If objAppContainer.objExportDataManager.CreateENQ(strProductCode, False) Then
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is EQRRecord Then
                        objEQR = CType(objResponse, EQRRecord)
                        Return objEQR.strBootsCode
                    End If
                End If
            End If
            Return False
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
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
        Dim objResponse As Object = Nothing
        Dim strProductCode As String
        Dim objEQR As EQRRecord
        Try
            If objAppContainer.objExportDataManager.CreateENQ(strBootsCode, False) Then
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is EQRRecord Then
                        objEQR = CType(objResponse, EQRRecord)
                        strProductCode = objEQR.strBarcode
                        Return strProductCode
                    End If
                End If
            End If
            Return False
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
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
        Return True
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
        'Return True
        'Fix for Incorrect Product code scanning for PL and CL.
        Dim objResponse As Object = Nothing
        Dim objEQR As EQRRecord
        Try
            strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(strProductCode)

            If objAppContainer.objExportDataManager.CreateENQ(strProductCode, False) Then
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is EQRRecord Then
                        objEQR = CType(objResponse, EQRRecord)
                        If objEQR.strBootsCode = strBootsCode Then
                            Return True
                        Else
                            Return False
                        End If

                    End If
                End If
            End If
            Return False
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
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
        Return True
    End Function
    ''' <summary>
    ''' Updates the Shelf Monitor picking list.
    ''' </summary>
    ''' <param name="arrPickingList">Arraylist to hold the SMProductInfo type objects.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Private Function UpdateSMPickingList(ByRef arrPickingList As ArrayList, ByVal strListID As String) As Boolean

        Dim objPLS As New PLSRecord()
        objPLS.strListID = strListID
        If objAppContainer.objExportDataManager.CreatePLS(objPLS) Then
            Dim objResponse As Object = Nothing
            Do While (DATAPOOL.getInstance.GetNextObject(objResponse))
                If TypeOf (objResponse) Is NAKRecord Then
                    Return False
                Else
                    Dim objPLI As PLIRecord
                    Dim objSMPLProductInfo As PLProductInfo = Nothing
                    objPLI = CType(objResponse, PLIRecord)
                    objSMPLProductInfo = New PLProductInfo()
                    With objSMPLProductInfo
                        .BootsCode = objPLI.strBootsCode
                        .Description = objPLI.strSEL_Desc
                        .BackShopQuantity = objPLI.strBackShopQuantity
                        .SalesFloorQuantity = objPLI.strShelfQuantity
                        .ActiveDeal = objPLI.cActiveDeal
                        .GapFlag = objPLI.cGapFlags
                        .MSFlag = objPLI.cMS_Flag
                        .OSSRFlag = objPLI.cOSSR_Flag
                        .ListItemStatus = objPLI.cStatus
                        If objPLI.cStatus = " " Then
                            .Status = "A"
                        Else
                            .Status = objPLI.cStatus
                        End If

                        .ProductCode = objPLI.strBarcode
                        If .ProductCode.Length = 13 Then
                            .ProductCode = .ProductCode.Remove(.ProductCode.Length - 1, 1)
                        End If
                        .ShortDescription = objPLI.strDescription
                        'System Testing SFA - Corrected for -ve stock figure
                        If Not objPLI.strStockFigure.Trim(" ") = "" Then
                            .TSF = Convert.ToInt16(objPLI.strStockFigure)
                        Else
                            .TSF = "0"
                        End If
                        '.OperatorID = objPLI.strOperatorID
                        .ParentsCode = objPLI.strParentBootsCode
                        .QuantityRequired = objPLI.strRequired
                        .Sequence = objPLI.strSequence
                    End With
                    arrPickingList.Add(objSMPLProductInfo)
                    objPLI = Nothing
                    objSMPLProductInfo = Nothing
                End If
            Loop
            Return True
        End If
    End Function
    ''' <summary>
    ''' Updates the OSSR picking list.
    ''' </summary>
    ''' <param name="arrPickingList">Arraylist to hold the OSSRProductInfo type objects.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Private Function UpdateOSSRPickingList(ByRef arrPickingList As ArrayList, ByVal strListID As String) As Boolean

        Dim objPLS As New PLSRecord()
        objPLS.strListID = strListID
        If objAppContainer.objExportDataManager.CreatePLS(objPLS) Then
            Dim objResponse As Object = Nothing
            Do While (DATAPOOL.getInstance.GetNextObject(objResponse))
                If TypeOf (objResponse) Is NAKRecord Then
                    Return False
                ElseIf TypeOf (objResponse) Is PLIRecord Then
                    Dim objPLI As PLIRecord
                    Dim objOSSRPLProductInfo As PLProductInfo = Nothing
                    objPLI = CType(objResponse, PLIRecord)
                    objOSSRPLProductInfo = New PLProductInfo()  'System Tesitng SFA -OSSRPLProductinfo changed to PLProductInfo
                    With objOSSRPLProductInfo
                        .BootsCode = objPLI.strBootsCode
                        .Description = objPLI.strSEL_Desc
                        .BackShopQuantity = objPLI.strBackShopQuantity
                        .SalesFloorQuantity = objPLI.strShelfQuantity
                        .ActiveDeal = objPLI.cActiveDeal
                        .GapFlag = objPLI.cGapFlags
                        .MSFlag = objPLI.cMS_Flag
                        .OSSRFlag = objPLI.cOSSR_Flag
                        .ListItemStatus = objPLI.cStatus
                        If objPLI.cStatus = " " Then
                            .Status = "A"
                        Else
                            .Status = objPLI.cStatus
                        End If
                        .ProductCode = objPLI.strBarcode
                        If .ProductCode.Length = 13 Then
                            .ProductCode = .ProductCode.Remove(.ProductCode.Length - 1, 1)
                        End If
                        .BootsCode = objPLI.strBootsCode
                        .Description = objPLI.strSEL_Desc
                        .ShortDescription = objPLI.strDescription
                        'System Testing SFA - Corrected for -ve stock figure
                        If Not objPLI.strStockFigure.Trim(" ") = "" Then
                            .TSF = Convert.ToInt16(objPLI.strStockFigure)
                        Else
                            .TSF = "0"
                        End If
                        '.OperatorID = objPLI.strOperatorID
                        .ParentsCode = objPLI.strParentBootsCode
                        .QuantityRequired = objPLI.strRequired
                        .Sequence = objPLI.strSequence
                    End With
                    arrPickingList.Add(objOSSRPLProductInfo)
                    objPLI = Nothing
                    objOSSRPLProductInfo = Nothing
                End If
            Loop
            Return True
        End If
    End Function

    ''' <summary>
    ''' Updated the Fast Fill Picking List.
    ''' </summary>
    ''' <param name="arrPickingList">Arraylist to hold the FFProductInfo type 
    ''' objects.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Private Function UpdateFFPickingList(ByRef arrPickingList As ArrayList, ByVal strListID As String) As Boolean
        Dim objPLS As New PLSRecord()
        objPLS.strListID = strListID
        If objAppContainer.objExportDataManager.CreatePLS(objPLS) Then
            Dim objResponse As Object = Nothing
            Do While (DATAPOOL.getInstance.GetNextObject(objResponse))
                If TypeOf (objResponse) Is NAKRecord Then
                    Return False
                Else
                    Dim objPLI As PLIRecord
                    Dim objFFPLProductInfo As PLProductInfo = Nothing 'System Tesitng SFA -FFPlProductinfo changed to PLProductInfo
                    objPLI = CType(objResponse, PLIRecord)
                    objFFPLProductInfo = New PLProductInfo()
                    With objFFPLProductInfo
                        .BootsCode = objPLI.strBootsCode
                        .Description = objPLI.strSEL_Desc
                        .BackShopQuantity = objPLI.strBackShopQuantity
                        .SalesFloorQuantity = objPLI.strShelfQuantity
                        .ActiveDeal = objPLI.cActiveDeal
                        .GapFlag = objPLI.cGapFlags
                        .MSFlag = objPLI.cMS_Flag
                        .OSSRFlag = objPLI.cOSSR_Flag
                        .ListItemStatus = objPLI.cStatus
                        If objPLI.cStatus = " " Then
                            .Status = "A"
                        Else
                            .Status = objPLI.cStatus
                        End If
                        .ProductCode = objPLI.strBarcode
                        If .ProductCode.Length = 13 Then
                            .ProductCode = .ProductCode.Remove(.ProductCode.Length - 1, 1)
                        End If
                        .ShortDescription = objPLI.strDescription
                        'System Testing SFA - Corrected for -ve stock figure
                        If Not objPLI.strStockFigure.Trim(" ") = "" Then
                            .TSF = Convert.ToInt16(objPLI.strStockFigure)
                        Else
                            .TSF = "0"
                        End If
                        '.OperatorID = objPLI.strOperatorID
                        .ParentsCode = objPLI.strParentBootsCode
                        .QuantityRequired = objPLI.strRequired
                        .Sequence = objPLI.strSequence
                    End With
                    arrPickingList.Add(objFFPLProductInfo)
                    objPLI = Nothing
                    objFFPLProductInfo = Nothing
                End If
            Loop
            Return True
        End If
    End Function
    ''' <summary>
    ''' Updated the Auto Fast Fill Picking List.
    ''' </summary>
    ''' <param name="arrPickingList">Arraylist to hold the AFFProductInfo type 
    ''' objects.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Private Function UpdateAFFPickingList(ByRef arrPickingList As ArrayList, ByVal strListID As String) As Boolean
        Dim objPLS As New PLSRecord()
        objPLS.strListID = strListID
        If objAppContainer.objExportDataManager.CreatePLS(objPLS) Then
            Dim objResponse As Object = Nothing
            Do While (DATAPOOL.getInstance.GetNextObject(objResponse))
                If TypeOf (objResponse) Is NAKRecord Then
                    Return False
                Else
                    Dim objPLI As PLIRecord
                    Dim objAFFPLProductInfo As PLProductInfo = Nothing 'System Tesitng SFA -FFPlProductinfo changed to PLProductInfo
                    objPLI = CType(objResponse, PLIRecord)
                    objAFFPLProductInfo = New PLProductInfo()

                    With objAFFPLProductInfo

                        .BootsCode = objPLI.strBootsCode
                        .Description = objPLI.strSEL_Desc
                        .QuantityRequired = objPLI.strShelfQuantity
                        .ActiveDeal = objPLI.cActiveDeal
                        .GapFlag = objPLI.cGapFlags
                        .MSFlag = objPLI.cMS_Flag
                        .OSSRFlag = objPLI.cOSSR_Flag
                        .ListItemStatus = objPLI.cStatus
                        If objPLI.cStatus = " " Then
                            .Status = "A"
                        Else
                            .Status = objPLI.cStatus
                        End If
                        .ProductCode = objPLI.strBarcode
                        If .ProductCode.Length = 13 Then
                            .ProductCode = .ProductCode.Remove(.ProductCode.Length - 1, 1)
                        End If
                        .ShortDescription = objPLI.strDescription
                        'System Testing SFA - Corrected for -ve stock figure
                        If Not objPLI.strStockFigure.Trim(" ") = "" Then
                            .TSF = Convert.ToInt16(objPLI.strStockFigure)
                        Else
                            .TSF = "0"
                        End If
                        '.OperatorID = objPLI.strOperatorID
                        .ParentsCode = objPLI.strParentBootsCode
                        .QuantityRequired = objPLI.strRequired
                        .Sequence = objPLI.strSequence
                    End With
                    arrPickingList.Add(objAFFPLProductInfo)
                    objPLI = Nothing
                    objAFFPLProductInfo = Nothing
                End If
            Loop
            'Send ENQ for each item and get the business center type and product group for each item.
            Try
                Dim objProdInfo As ItemInfo = Nothing
                For Each objAFFProductInfo As AFFPLProductInfo In arrPickingList
                    objProdInfo = New ItemInfo()
                    GetItemDetailsAllUsingBC(objAFFProductInfo.BootsCode, objProdInfo)
                    'GetItemDetailsAllUsingPC(objAFFProductInfo.ProductCode, objProdInfo)
                    objAFFProductInfo.ProductGrp = objProdInfo.ProductGrp
                    objAFFProductInfo.BCType = objProdInfo.BCType
                Next
            Catch ex As Exception

            End Try
            'Return True
            Return True
        End If
    End Function
    ''' <summary>
    ''' Updates Excess Stock picking list.
    ''' </summary>
    ''' <param name="arrPickingList">Array list to hold EXPLProductInfo type 
    ''' objects.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results</returns>
    ''' <remarks></remarks>
    Private Function UpdateEXPickingList(ByRef arrPickingList As ArrayList, ByVal strListID As String) As Boolean
        Dim objPLS As New PLSRecord()
        objPLS.strListID = strListID
        If objAppContainer.objExportDataManager.CreatePLS(objPLS) Then
            Dim objResponse As Object = Nothing
            Do While (DATAPOOL.getInstance.GetNextObject(objResponse))
                If TypeOf (objResponse) Is NAKRecord Then
                    Return False
                Else
                    Dim objPLI As PLIRecord
                    Dim objEXPLProductInfo As PLProductInfo = Nothing
                    objPLI = CType(objResponse, PLIRecord)
                    objEXPLProductInfo = New PLProductInfo()
                    'nan TODO Changes in PLI record needs to be incorporated
                    With objEXPLProductInfo
                        .BootsCode = objPLI.strBootsCode
                        .Description = objPLI.strSEL_Desc
                        .BackShopQuantity = objPLI.strBackShopQuantity
                        .SalesFloorQuantity = objPLI.strShelfQuantity
                        .ActiveDeal = objPLI.cActiveDeal
                        .GapFlag = objPLI.cGapFlags
                        .MSFlag = objPLI.cMS_Flag
                        .OSSRFlag = objPLI.cOSSR_Flag
                        .ListItemStatus = objPLI.cStatus
                        If objPLI.cStatus = " " Then
                            .Status = "A"
                        Else
                            .Status = objPLI.cStatus
                        End If
                        .ProductCode = objPLI.strBarcode
                        If .ProductCode.Length = 13 Then
                            .ProductCode = .ProductCode.Remove(.ProductCode.Length - 1, 1)
                        End If
                        .ShortDescription = objPLI.strDescription
                        'System Testing SFA - Corrected for -ve stock figure
                        If Not objPLI.strStockFigure.Trim(" ") = "" Then
                            .TSF = Convert.ToInt16(objPLI.strStockFigure)
                        Else
                            .TSF = "0"
                        End If
                        '.OperatorID = objPLI.strOperatorID
                        .ParentsCode = objPLI.strParentBootsCode
                        '.QuantityRequired = objPLI.strRequired
                        .Sequence = objPLI.strSequence
                    End With
                    arrPickingList.Add(objEXPLProductInfo)
                    objPLI = Nothing
                    objEXPLProductInfo = Nothing
                End If
            Loop
            Return True
        End If
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
#If STUB Then
        Dim objPlanners As PlannerInfo = Nothing
        Dim bStatus As Boolean = False

        For Each strPOGID As String In arrPOGList

            objPlanners = New PlannerInfo()
            Try

                With objPlanners
                    .PlannerID = "178418"
                    .Description = "MassFrag5                     "
                    .RebuildDate = "20080915"

                End With
                arrPlannerList.Add(objPlanners)
                'Set the status
                bStatus = True

            Catch ex As Exception
                'Add the exception to the application log.
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                                ex.Message.ToString(), _
                                                Logger.LogLevel.RELEASE)
                'move to next item in the array.
                Continue For
            Finally

            End Try
        Next
        'Return the status
        Return bStatus
#End If
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
    Private Function UpdatePlannerList(ByVal arrPOGList As ArrayList, ByVal strBootsCode As String, ByRef arrPlannerList As ArrayList) As Boolean
#If STUB Then
        Dim objPlanners As PlannerInfo = Nothing
        Dim tempArrayList As ArrayList = New ArrayList()
        Dim bStatus As Boolean = False
        Dim strModID As String = ""

        Try
            For Each strPOGID As String In arrPOGList
                strModID = strModID + "'" + strPOGID.Trim() + "',"
            Next
            'Trim the last comma.
            strModID = strModID.TrimEnd(",")


            objPlanners = New PlannerInfo()
            With objPlanners
                .PlannerID = "126753"
                .Description = "MODULE 0007"
                .SequenceNumber = "1"
                .ShelfNumber = "1"
                .POGDesc = "MassFrag5"
                .PhysicalShelfQty = "40"
                .RepeatCount = "1"
            End With
            arrPlannerList.Add(objPlanners)
            bStatus = True


        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'Set the status
            bStatus = False
        Finally

        End Try
        'Next
        'Return the status
        Return bStatus
#End If
    End Function
    ''' <summary>
    ''' To create an index for the table
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CreateTableIndex() As Boolean
        Return True
    End Function
    'Lakshmi
    'Lakshmi
    ''' <summary>
    ''' To get the item sales info
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="objItemSalesInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetItemSalesDetailsAllUsingPC(ByVal strProductCode As String, ByRef objItemSalesInfo As ItemSalesInfo) As Boolean
        Dim objItemsInfo As ItemInfo
        Try
            objItemsInfo = New ItemInfo()
            If GetItemDetailsAllUsingPC(strProductCode, objItemsInfo) Then
                If objAppContainer.objExportDataManager.CreateISE(strProductCode) Then
                    Dim objResponse As Object = Nothing
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is ISRRecord Then
                            Dim objISR As ISRRecord
                            objISR = CType(objResponse, ISRRecord)
                            With objItemSalesInfo
                                .BootsCode = objItemsInfo.BootsCode
                                .Description = objItemsInfo.Description
                                .ProductCode = objItemsInfo.ProductCode
                                .ShortDescription = objItemsInfo.ShortDescription
                                .Status = objItemsInfo.Status
                                .ThisWeekUnits = objISR.strItemSoldWeek
                                .ThisWeekValue = objISR.strValueItemSoldWeek
                                .TodayUnits = objISR.strItemSoldTody
                                .TodayValue = objISR.strValueItemSoldToday
                                .TSF = Val(objItemsInfo.TSF)
                            End With
                        Else
                            Return False
                        End If
                        objResponse = Nothing
                    Else
                        Return False
                    End If
                    Return True
                End If
            Else
                Return False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
            ''Log the error here 
        End Try
    End Function

    ''' <summary>
    ''' To get store sales information
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetStoreSalesInfo(ByRef objStoreSales As StoreSalesInfo) As Boolean
        Try
            If objAppContainer.objExportDataManager.CreateSSE() Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is SSRRecord Then
                        objStoreSales.Today = CType(objResponse, SSRRecord).strTodaysSales
                        objStoreSales.ThisWeek = CType(objResponse, SSRRecord).strWeeklySales
                    Else
                        Return False
                    End If
                    objResponse = Nothing
                Else
                    Return False
                End If
                Return True
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
            ''Log the error here 
        End Try
    End Function
    Public Function GetReportHeaders(ByVal strReportID As String, ByRef arrReportHeaders As System.Collections.ArrayList) As Boolean
        Try
            Dim objRLS As New RLSRecord()
            Dim objResponse As Object = Nothing
            Dim iLastIndex As Integer
            Dim iNextSequence As Integer
            Dim objRLDInnerStructure As New RLDInnerStructure()
            Dim objRLD As New RLDRecord()
            objRLS.strSequenceNo = "0000"
            objRLS.strReportID = strReportID
            Do
                If objAppContainer.objExportDataManager.CreateRLS(objRLS) Then
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is RLDRecord Then
                            'arrReportHeaders.Add(CType(objResponse, RLDRecord))
                            objRLD = CType(objResponse, RLDRecord)
                            arrReportHeaders.Add(objRLD)
                            iLastIndex = objRLD.strCount - 1
                            iNextSequence = CType(objRLD.arrRLDInnerStructure(iLastIndex), RLDInnerStructure).strSequence
                            objRLS.strSequenceNo = iNextSequence + 1
                        ElseIf objResponse.ToString() = "RLF" Then
                            Return True
                        Else
                            Return False
                        End If
                        objResponse = Nothing
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If
            Loop
            Return True
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False

        End Try
    End Function

    Public Function GetReportList(ByRef arrReports As System.Collections.ArrayList) As Boolean
        Try
            Dim objRLE As New RLERecord()
            Dim objResponse As Object = Nothing
            objRLE.strSequenceNo = "0000"
            Do
                If objAppContainer.objExportDataManager.CreateRLE(objRLE) Then
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is RLRRecord Then
                            arrReports.Add(CType(objResponse, RLRRecord))
                            objRLE.strSequenceNo = CType(objResponse, RLRRecord).strSequence
                        ElseIf objResponse.ToString() = "RLF" Then
                            Return True
                        Else
                            Return False
                        End If

                        objResponse = Nothing
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If
            Loop
            Return True
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False

        End Try
    End Function
    Public Function GetReportDetails(ByVal strReportID As String, ByVal strSeqNum As String, ByRef arrReportDetails As System.Collections.ArrayList) As Boolean
        Try
            Dim objRPS As New RPSRecord()
            Dim objResponse As Object = Nothing
            objRPS.strSequenceNo = strSeqNum + 1
            objRPS.strReportID = strReportID
            Do
                If objAppContainer.objExportDataManager.CreateRPS(objRPS) Then
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is RUPRecord Then
                            arrReportDetails.Add(CType(objResponse, RUPRecord))
                            objRPS.strSequenceNo = objRPS.strSequenceNo + Val(CType(objResponse, RUPRecord).strCount)
                        ElseIf objResponse.ToString() = "RLF" Then
                            Return (True)
                        Else
                            Return False
                        End If
                        objResponse = Nothing
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If
            Loop
            Return True
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False

        End Try
    End Function
    'End-Lakshmi
End Class
#End If