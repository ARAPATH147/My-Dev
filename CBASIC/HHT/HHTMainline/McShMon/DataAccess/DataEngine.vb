Imports System
Imports System.Data
'''****************************************************************************
''' <FileName>DataEngine.vb</FileName>
''' <summary>
''' Data Engine class being a part of the data access layer exposes 
''' APIs to business layer for accessing data from the location database.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''****************************************************************************
Public Class DataEngine
    Dim isRFDevice As Boolean = Nothing
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        GetDeviceType()
    End Sub
    ''' <summary>
    ''' Get all the Count List available in the local database.
    ''' </summary>
    ''' <param name="arrObjectList">Array list to store the list of objects of 
    ''' type CountList
    ''' </param>
    ''' <returns>Bool
    ''' True - If successfully got the Count List.
    ''' False - If error occured while retreiving the Count Lists.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetCountList(ByRef arrObjectList As ArrayList) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the Count list from the local DB.
            Return objRFDataSource.GetCountList(arrObjectList)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            objRFDataSource = Nothing
        End Try
#End If
#If NRF Then
            Dim objNRFDataSource As New SMNRFDataSource()
            Try
                'Get the Count list from the local DB.
                Return objNRFDataSource.GetCountList(arrObjectList)
            Catch ex As Exception
                Return False
            Finally
                objNRFDataSource = Nothing
            End Try
#End If
    End Function
    ''' <summary>
    ''' Get details of each item in the selected Count Lists.
    ''' </summary>
    ''' <param name="strListID"></param>
    ''' <param name="aCLProductInfo"></param>
    ''' <returns>Bool    
    ''' True - If successfully obtained the item details.
    ''' False - If error occurred in getting the item details.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetCountListItemDetails(ByVal strListID As String, ByRef aCLProductInfo As ArrayList, ByRef strErrMsg As String) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details for the products in a Count list.
            Return objRFDataSource.GetCountListItemDetails(strListID, aCLProductInfo, strErrMsg)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details for the products in a Count list.
            Return objNRFDataSource.GetCountListItemDetails(strListID, aCLProductInfo)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Get site information for count list
    ''' </summary>
    ''' <param name="strListID">List ID of the selected Count List.</param>
    ''' <param name="arrPlannerList">Reference to the object that stores the
    ''' planner details.</param>
    ''' <returns>Bool
    ''' True - If successfully obtained the item details.
    ''' False - If error occurred in getting the item details.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetPlannerListDetails(ByVal strListID As String, ByRef arrPlannerList As ArrayList) As Boolean
        'Stock File Accuracy  - Added new function to retrieve planners
#If NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details for the products in a Count list.
            Return objNRFDataSource.GetPlannerListDetails(strListID, arrPlannerList)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Get the list id for the list.
    ''' </summary>
    ''' <param name="objCountList">product groupinfo object to store the list id
    ''' </param>
    ''' <returns>Bool
    ''' True - If successfully created the list.
    ''' False - If error occured while creating the Count Lists.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetCreateCountListID(ByVal strStatus As String, ByRef objCountList As CLProductGroupInfo) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            Return objRFDataSource.GetCreateCountListID(strStatus, objCountList)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            objRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Returns multisite list
    ''' </summary>
    ''' <param name="strListID"></param>
    ''' <param name="strBootsCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMultiSiteDetails(ByVal strListID As String, ByVal strBootsCode As String, _
                                          ByRef arrPlannerList As ArrayList) As Boolean
        'Stock File Accuracy  - Added new function to get multi site
#If RF Then
         Dim objRFDataSource As New SMRFDataSource()
        Try

            'Get the site details.
            Return objRFDataSource.GetMultiSiteInfo(strBootsCode,True, arrPlannerList)

        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the site details for the products in a Count list.
            Return objNRFDataSource.GetMultiSiteInfo(strListID, strBootsCode, arrPlannerList)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function

   
    ''' <summary>
    ''' Get boots code list for each site selected
    ''' </summary>
    ''' <param name="strLocation"></param>
    ''' <param name="strPlannerId"></param>
    ''' <param name="strListId"></param>
    ''' <param name="arrBootsCodeList"></param>
    ''' <returns>Bool   
    ''' True - If successfully obtained the item details.
    ''' False - If error occurred in getting the item details.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetBootsCodeItemList(ByVal strLocation As String, ByVal strPlannerDesc As String, _
                 ByVal strListId As String, ByRef arrBootsCodeList As ArrayList) As Boolean
        'Stock File Accuracy  - Added new function to retrieve items


        'Get the item details for the products in a Count list.
        'Clear the variables
#If NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details for the products in a Count list.
            Return objNRFDataSource.GetBootsCodeItemList(strLocation, strPlannerDesc, strListId, arrBootsCodeList)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try

#End If
    End Function
    ''' <summary>
    ''' Retrieves site count for an item in a planner
    '''     ''' 
    ''' </summary>
    ''' <param name="strPlannerDesc"></param>
    ''' <param name="strListId"></param>
    ''' <param name="arrBootsCodeList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetItemSiteCount(ByVal strPlannerDesc As String, _
                 ByVal strListId As String, ByRef arrBootsCodeList As ArrayList) As Boolean
        'Stock File Accuracy  - Added new function to retrieve site count for an item
#If NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details for the products in a Count list.
            Return objNRFDataSource.GetItemSiteCount(strPlannerDesc, strListId, arrBootsCodeList)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If

    End Function

    ''' <summary>
    ''' Gets the item details for Products scanned in Shelf Monitor module.
    ''' </summary>
    ''' <param name="strBootsCode">Boots Code</param>
    ''' <param name="objSMProductInfo">Object reference to update the 
    ''' item details.</param>
    ''' <returns>Bool
    ''' True - If successfully obtained the item details.
    ''' False - If error occurred in getting the item details.</returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingBC(ByVal strBootsCode As String, ByRef objSMProductInfo As SMProductInfo) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetProductInfoUsingBC(strBootsCode, objSMProductInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetProductInfoUsingBC(strBootsCode, objSMProductInfo)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Gets the item details for Products scanned in Auto Stuff Your Shelves module.
    ''' </summary>
    ''' <param name="strBootsCode">Boots Code</param>
    ''' <param name="objASYSProductInfo">Object reference to update the 
    ''' item details.</param>
    ''' <returns>Bool
    ''' True - If successfully obtained the item details.
    ''' False - If error occurred in getting the item details.</returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingBC(ByVal strBootsCode As String, ByRef objASYSProductInfo As ASYSProductInfo) As Boolean
#If RF Then

        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            'strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
            Return objRFDataSource.GetProductInfoUsingBC(strBootsCode, objASYSProductInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try

#ElseIf NRF Then

        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetProductInfoUsingBC(strBootsCode, objASYSProductInfo)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try

#End If

    End Function
    Public Function ValidateUsingPCAndBC(ByVal strBootsCode As String, ByVal strProductCode As String) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.

            Return objRFDataSource.ValidateUsingPCAndBC(strBootsCode, strProductCode)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.ValidateUsingPCAndBC(strBootsCode, strProductCode)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Get item details for a product in Fast Fill module.
    ''' </summary>
    ''' <param name="strBootsCode">Boots Code</param>
    ''' <param name="objFFProductInfo">Object reference to update the item 
    ''' details.</param>
    ''' <returns>Bool
    ''' True - If successfully obtained the item details.
    ''' False - If error occurred in getting the item details.</returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingBC(ByVal strBootsCode As String, ByRef objFFProductInfo As FFProductInfo) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetProductInfoUsingBC(strBootsCode, objFFProductInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetProductInfoUsingBC(strBootsCode, objFFProductInfo)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Get item details for a product in Excess Stock module.
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <param name="objEXProductInfo"></param>
    ''' <returns>Bool
    ''' True - If successfully obtained the item details.
    ''' False - If error occurred in getting the item details.</returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingBC(ByVal strBootsCode As String, ByRef objEXProductInfo As EXProductInfo) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetProductInfoUsingBC(strBootsCode, objEXProductInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetProductInfoUsingBC(strBootsCode, objEXProductInfo)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Get item details for a product in Print SEL module.
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <param name="objPSProductInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingBC(ByVal strBootsCode As String, ByRef objPSProductInfo As PSProductInfo) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetProductInfoUsingBC(strBootsCode, objPSProductInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
            Dim objNRFDataSource As New SMNRFDataSource()
            Try
                'Get the item details.
                Return objNRFDataSource.GetProductInfoUsingBC(strBootsCode, objPSProductInfo)
            Catch ex As Exception
                Return False
            Finally
                'Clear the variables
                objNRFDataSource = Nothing
            End Try
#End If
    End Function
    ''' <summary>
    ''' retrieve detals for count list
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <param name="objCLProductInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCLProductInfo(ByVal strBootsCode As String, ByRef objCLProductInfo As CLProductInfo) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetCLProductInfo(strBootsCode, objCLProductInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#End If
    End Function
#Region "Create Own List"
    ''' <summary>
    ''' Get item details for a product in Create Own List using Product code.
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="objCLProductInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, Optional ByRef objCLProductInfo As CLProductInfo = Nothing, Optional ByVal objCountList As CLProductGroupInfo = Nothing, Optional ByRef arrCLObject As ArrayList = Nothing, Optional ByRef iNakErrorFlag As Boolean = False) As Boolean
#If RF Then
        strProductCode = strProductCode.PadLeft(12, "0")
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetProductInfoUsingPC(strProductCode, objCLProductInfo, objCountList, arrCLObject, iNakErrorFlag)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        strProductCode = strProductCode.PadLeft(12, "0")
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetProductInfoUsingPC(strProductCode, objCLProductInfo)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Gets the item details for Products scanned in Create Own List.
    ''' </summary>
    ''' <param name="strBootsCode">Boots Code</param>
    ''' <param name="objCLProductInfo">Object reference to update the 
    ''' item details.</param>
    ''' <returns>Bool
    ''' True - If successfully obtained the item details.
    ''' False - If error occurred in getting the item details.</returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingBC(ByVal strBootsCode As String, ByRef objCLProductInfo As CLProductInfo, Optional ByVal objCountList As CLProductGroupInfo = Nothing, Optional ByRef arrCLObject As ArrayList = Nothing, Optional ByRef iNakErrorFlag As Boolean = False) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetProductInfoUsingPC(strBootsCode, objCLProductInfo, objCountList, arrCLObject, iNakErrorFlag)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetProductInfoUsingBC(strBootsCode, objCLProductInfo)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
#End Region
    ''' <summary>
    ''' Get item details for a product in Shelf Monitor module using Product code.
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="objSMProductInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, ByRef objSMProductInfo As SMProductInfo) As Boolean
#If RF Then
        strProductCode = strProductCode.PadLeft(12, "0")
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetProductInfoUsingPC(strProductCode, objSMProductInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        strProductCode = strProductCode.PadLeft(12, "0")
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetProductInfoUsingPC(strProductCode, objSMProductInfo)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Get item details for a product in Fast Fill module using Product code.
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="objFFProductInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, ByRef objFFProductInfo As FFProductInfo) As Boolean
#If RF Then
        'in case of RF the Query should have a 13 digit barcode
        'So generating a CDV
        strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(strProductCode).PadLeft(13, "0")
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetProductInfoUsingPC(strProductCode, objFFProductInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        strProductCode = strProductCode.PadLeft(12, "0")
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetProductInfoUsingPC(strProductCode, objFFProductInfo)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Get item details for a product in Excess Stock module using Product code.
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="objEXProductInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, ByRef objEXProductInfo As EXProductInfo) As Boolean
#If RF Then

        strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(strProductCode)
        strProductCode = strProductCode.PadLeft(13, "0")

        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetProductInfoUsingPC(strProductCode, objEXProductInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        strProductCode = strProductCode.PadLeft(12, "0")
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetProductInfoUsingPC(strProductCode, objEXProductInfo)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Get item details for a product in Print SEL module using Product code.
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="objPSProductInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, ByRef objPSProductInfo As PSProductInfo) As Boolean
#If RF Then
        strProductCode = strProductCode.PadLeft(12, "0")
        strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(strProductCode)
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetProductInfoUsingPC(strProductCode, objPSProductInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        strProductCode = strProductCode.PadLeft(12, "0")
        'strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(strProductCode)
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetProductInfoUsingPC(strProductCode, objPSProductInfo)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Get item details for a product in Auto Stuff Your Shelves module using Product code.
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="objASYSProductInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, ByRef objASYSProductInfo As ASYSProductInfo) As Boolean
#If RF Then
        strProductCode = strProductCode.PadLeft(12, "0")
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(strProductCode)
            Return objRFDataSource.GetProductInfoUsingPC(strProductCode, objASYSProductInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        strProductCode = strProductCode.PadLeft(12, "0")
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetProductInfoUsingPC(strProductCode, objASYSProductInfo)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Get item details for a product in Item Info module using Boots code.
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <param name="objItemInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetItemDetailsAllUsingBC(ByVal strBootsCode As String, ByRef objItemInfo As ItemInfo) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetItemDetailsAllUsingBC(strBootsCode, objItemInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetItemDetailsAllUsingBC(strBootsCode, objItemInfo)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Get item details for a product in Item Info module using Product code.
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="objItemInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetItemDetailsAllUsingPC(ByVal strProductCode As String, ByRef objItemInfo As ItemInfo) As Boolean
        'Pad the scanned or key entered product to 13 digit length.
        strProductCode = strProductCode.PadLeft(13, "0")
#If RF Then
        'strProductCode = strProductCode.PadLeft(13, "0")
        'strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(strProductCode)
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetItemDetailsAllUsingPC(strProductCode, objItemInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        'strProductCode = strProductCode.PadLeft(13, "0")
        'remove the last character in the product i.e., CDV digit as database doesnot contain CDV for product code.
        strProductCode = strProductCode.Remove(strProductCode.Length -1,1)
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetItemDetailsAllUsingPC(strProductCode, objItemInfo)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Gets information about a deal.
    ''' </summary>
    ''' <param name="strDealNo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDealDetails(ByVal strDealNo As String, ByRef objDealDetails As DQRRECORD) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetDealDetails(strDealNo, objDealDetails)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetDealDetails(strDealNo, objDealDetails)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Gets the price check information
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="objPCProductInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPriceCheckInfo(ByVal strBootsCode As String, ByVal strProductCode As String, ByRef objPCProductInfo As PriceCheckInfo) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'For Full Price Check we send the Boots code only so commenting out the str Product code code
            ' strProductCode = strProductCode.PadLeft(13, "0")
            'Get the item details.
            Return objRFDataSource.GetPriceCheckInfo(strBootsCode, objPCProductInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            strProductCode = strProductCode.PadLeft(12, "0")
            'Get the item details.
            Return objNRFDataSource.GetPriceCheckInfo(strBootsCode, _
                                                      strProductCode, _
                                                      objPCProductInfo)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Get the entire list of pciking list available.
    ''' </summary>
    ''' <param name="arrObjectList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPickingList(ByVal arrObjectList As ArrayList) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetPickingList(arrObjectList)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetPickingList(arrObjectList)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Validate a product using Boots code.
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckIsProductValidUsingBC(ByVal strBootsCode As String) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.CheckIsProductValidUsingBC(strBootsCode)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.CheckIsProductValidUsingBC(strBootsCode)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Validate a product using Product code.
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckIsProductValidUsingPC(ByVal strProductCode As String) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.CheckIsProductValidUsingPC(strProductCode)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.CheckIsProductValidUsingPC(strProductCode)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Gets the price check target details.
    ''' </summary>
    ''' <param name="objPCTargetDetails"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPCTargetDetails(ByRef objPCTargetDetails As PCTargetDetails) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetPCTargetDetails(objPCTargetDetails)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetPCTargetDetails(objPCTargetDetails)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Gets the details of the User ID supplied.
    ''' </summary>
    ''' <param name="objUserInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserDetails(ByVal strUserID As String, ByRef objUserInfo As UserInfo) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetUserDetails(strUserID, objUserInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetUserDetails(strUserID, objUserInfo)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Gets the Product details and picking list details for S, F, B
    ''' </summary>
    ''' <param name="strListID"></param>
    ''' <param name="arrPickignList"></param>
    ''' <param name="strListType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProductInfo(ByVal strListID As String, ByRef arrPickignList As ArrayList, ByVal strListType As String) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetProductInfo(strListID, arrPickignList, strListType)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetProductInfo(strListID, arrPickignList, strListType)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Gets the entire list of categories available for Core and Non Core type.
    ''' </summary>
    ''' <param name="strCore"></param>
    ''' <param name="arrCategoryList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPOGCategoryList(ByVal strCore As String, ByRef arrCategoryList As ArrayList, Optional ByVal isPending As Boolean = False) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetPOGCategoryList(strCore, arrCategoryList, isPending)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetPOGCategoryList(strCore, arrCategoryList)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Get the list of planners which contains the item whose
    ''' Boots code is supplied.
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <param name="arrPlannerList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPlannerListUsingBC(ByVal strBootsCode As String, ByVal bIsMultisiteCall As Boolean, ByRef arrPlannerList As ArrayList) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetPlannerListUsingBC(strBootsCode, bIsMultisiteCall, arrPlannerList)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetPlannerListUsingBC(strBootsCode, _
                                                          bIsMultisiteCall, _
                                                          arrPlannerList)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Get the multisite list of items in picking list
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <param name="arrPlannerList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPickingListMultisiteList(ByVal strBootsCode As String, ByVal strListID As String, ByRef arrPlannerList As ArrayList) As Boolean
#If RF Then
        'TODO
          Dim objRFDataSource As New SMRFDataSource()
        Try
            Return objRFDataSource.GetMultiSiteInfo(strBootsCode,True, arrPlannerList)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetPickingListMultisiteList(strBootsCode, _
                                                          strListID, _
                                                          arrPlannerList)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Gets the list of planners for a Product whose
    ''' Product code is supplied.
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="arrPlannerList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPlannerListUsingPC(ByVal strProductCode As String, ByVal bIsMultisiteCall As Boolean, ByRef arrPlannerList As ArrayList) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            strProductCode = strProductCode.PadLeft(12, "0")
            strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(strProductCode)
            'Get the item details.
            Return objRFDataSource.GetPlannerListUsingPC(strProductCode, _
                                                          bIsMultisiteCall, _
                                                          arrPlannerList)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then

        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            strProductCode = strProductCode.PadLeft(12, "0")
            'Get the item details.
            Return objNRFDataSource.GetPlannerListUsingPC(strProductCode, _
                                                          bIsMultisiteCall, _
                                                          arrPlannerList)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Gets the list of module for the Planner ID supplied.
    ''' </summary>
    ''' <param name="strPOGID"></param>
    ''' <param name="arrModuleList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetModuleList(ByVal strPOGID As String, ByRef arrModuleList As ArrayList) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetModuleList(strPOGID, arrModuleList)

        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetModuleList(strPOGID, arrModuleList)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Gets the list of module for the Planner ID supplied.
    ''' </summary>
    ''' <param name="strPOGID"></param>
    ''' <param name="arrModuleList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetModuleList(ByVal strPOGID As String, ByVal strBootsCode As String, ByRef arrModuleList As ArrayList) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetModuleList(strPOGID, strBootsCode, arrModuleList)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetModuleList(strPOGID, strBootsCode, arrModuleList)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Function to get the details to print SELs and clearance labels.
    ''' </summary>
    ''' <param name="objPSProductInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetLabelDetails(ByRef objPSProductInfo As PSProductInfo) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetLabelDetails(objPSProductInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Gets the line list for the Module whose Module ID is supplied.
    ''' </summary>
    ''' <param name="strModuleID"></param>
    ''' <param name="strSequenceNumber"></param>
    ''' <param name="arrLineList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetLineList(ByVal strModuleID As String, ByVal strSequenceNumber As String, ByRef arrLineList As ArrayList) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetLineList(strModuleID, _
                                                      strSequenceNumber, _
                                                      arrLineList)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetLineList(strModuleID, _
                                                      strSequenceNumber, _
                                                      arrLineList)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Gets the list of planned for a Category whose CategoryID is supplied.
    ''' </summary>
    ''' <param name="strCategoryID"></param>
    ''' <param name="arrPlannerList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPlannerListForCategory(ByVal strCategoryID As String, ByRef arrPlannerList As ArrayList, Optional ByVal isPendingPlanner As Boolean = False) As Boolean
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetPlannerListForCategory(strCategoryID, arrPlannerList, isPendingPlanner)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetPlannerListForCategory(strCategoryID, arrPlannerList)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' To get Boots code using a product's Product code.
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <returns>
    ''' Boots Code - If Boots code is available for the Product code entered.
    ''' 0 - If any error occured while trying to get data from DB.</returns>
    ''' <remarks></remarks>
    Public Function GetBootsCode(ByVal strProductCode As String) As String
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Dim strBootsCode As String = Nothing
        Try
            strProductCode = strProductCode.PadLeft(12, "0")
            strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(strProductCode)
            'Get the item details.
            strBootsCode = objRFDataSource.GetBootsCode(strProductCode)
            Return strBootsCode
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return 0
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
        Return 0
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Dim strBootsCode As String = Nothing
        Try
            strProductCode = strProductCode.PadLeft(12, "0")
            'Get the item details.
            strBootsCode = objNRFDataSource.GetBootsCode(strProductCode)
            Return strBootsCode
        Catch ex As Exception
            Return 0
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' To get the product code for a product using its boots code.
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <returns>
    ''' BarCode - If Product Bar code is available for the Boots code entered.
    ''' 0 - If any error occured while trying to get data from DB.</returns>
    ''' <remarks></remarks>
    Public Function GetProductCode(ByVal strBootsCode As String) As String
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Dim strProductCode As String = Nothing
        Try
            'Get the item details.
            strProductCode = objRFDataSource.GetProductCode(strBootsCode)
            Return strProductCode
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return 0
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
        Return 0
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Dim strProductCode As String = Nothing
        Try
            'Get the item details.
            strProductCode = objNRFDataSource.GetProductCode(strBootsCode)
            Return strProductCode
        Catch ex As Exception
            Return 0
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
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
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetItemDescription(strBootsCode)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return 0
            End If
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
        Return 0
#ElseIf NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.GetItemDescription(strBootsCode)
        Catch ex As Exception
            Return 0
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Check if the item is multisited and return a boolean value.
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckMultisite(ByVal strBootsCode As String) As String
#If NRF Then
        Dim objNRFDataSource As New SMNRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.CheckMultisite(strBootsCode)
        Catch ex As Exception
            Return 0
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' Gets the device type read from the configuration file.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetDeviceType()
        'Read the device type from the cionfiguration file.
        Dim strDeviceType As String = Nothing
        strDeviceType = ConfigDataMgr.GetInstance().GetParam(ConfigKey.DEVICE_TYPE).ToString()

        If strDeviceType = Macros.MC70 Then
            isRFDevice = False
        ElseIf strDeviceType = Macros.RF Then
            isRFDevice = True
        End If
    End Sub
    Public Function CreateTableIndex() As Boolean
#If NRF Then
            Dim objNRFDataSource As New SMNRFDataSource()
            Try
                'Get the item details.
                Return objNRFDataSource.CreateTableIndex()
            Catch ex As Exception
                Return False
            Finally
                'Clear the variables
                objNRFDataSource = Nothing
            End Try
#End If
    End Function
    'Lakshmi
    ''' <summary>
    ''' To get the item sales info
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="objItemSalesInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
#If RF Then
    Public Function GetItemSalesDetailsAllUsingPC(ByVal strProductCode As String, ByRef objItemSalesInfo As ItemSalesInfo) As Boolean
        Dim objRFDataSource As New SMRFDataSource()
        Try
            strProductCode = strProductCode.PadLeft(12, "0")
            strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(strProductCode)
            Return objRFDataSource.GetItemSalesDetailsAllUsingPC(strProductCode, objItemSalesInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                                ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return 0
            End If
        Finally
            objRFDataSource = Nothing
        End Try
        Return True
    End Function
    ''' <summary>
    ''' To get the item sales info
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="objItemSalesInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetItemSalesDetailsAllUsingBC(ByVal strProductCode As String, ByRef objItemSalesInfo As ItemSalesInfo) As Boolean
        Dim objRFDataSource As New SMRFDataSource()
        strProductCode = strProductCode.Substring(0, strProductCode.Length - 1)
        Try
            Return GetItemSalesDetailsAllUsingPC(strProductCode, objItemSalesInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            objRFDataSource = Nothing
        End Try
        Return True
    End Function
    ''' <summary>
    ''' Gets the store sales details
    ''' </summary>
    ''' <param name="objStoreSales"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetStoreSalesInfo(ByRef objStoreSales As StoreSalesInfo)
        Dim objRFDataSource As New SMRFDataSource()
        Try
            Return objRFDataSource.GetStoreSalesInfo(objStoreSales)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
         ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return False
            End If
        Finally
            objRFDataSource = Nothing
        End Try
    End Function
    Public Function GetReportHeaders(ByVal strReportID As String, ByRef arrReportHeaders As System.Collections.ArrayList) As Boolean
        Dim objRFDataSource As New SMRFDataSource()
        Try
            Return objRFDataSource.GetReportHeaders(strReportID, arrReportHeaders)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return 0
            End If
        Finally
            objRFDataSource = Nothing
        End Try
    End Function
    Public Function GetReportList(ByRef arrReports As System.Collections.ArrayList) As Boolean
        Dim objRFDataSource As New SMRFDataSource()
        Try
            Return objRFDataSource.GetReportList(arrReports)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return 0
            End If
        Finally
            objRFDataSource = Nothing
        End Try
    End Function
    Public Function GetReportDetails(ByVal strReportID As String, ByVal strSeqNum As String, ByRef arrReportDetails As System.Collections.ArrayList) As Boolean
        Dim objRFDataSource As New SMRFDataSource()
        Try
            Return objRFDataSource.GetReportDetails(strReportID, strSeqNum, arrReportDetails)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return 0
            End If
        Finally
            objRFDataSource = Nothing
        End Try
    End Function
#End If
    'End-Lakshmi
End Class
