Imports Goodsin.GIValueHolder

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
'''***************************************************************************
'''* Modification Log 
'''**************************************************************************** 
'''* No:     Author            Date            Description 
'''* 1.1   Christopher Kitto  09/04/2015  Modified as part of DALLAS project.
'''            (CK)                       Amended declaration of function
'''                                       GetBookInDeliverySummary.
'''                                       Added new function
'''                                       SendDallasDeliveryConfirmation
''' 
'''        Kiran Krishnan     27/04/2015  Added new function CheckDallasStore() 
'''            (KK)                       GetDallasUODListForView and 
'''                                       ValidateDallasUODScanned()  
'''********************************************************************************
Public MustInherit Class DataEngine

    Public MustOverride Function GetConfigValues(ByRef objConfigValue As AppContainer.ConfigValues) As Boolean

    ' V1.1 - CK
    ' Ammending the declaration of GetBookInDeliverySummary inorder to pass an extra argument - 
    ' array to hold the details of Dallas UODS
    ' Public MustOverride Function GetBookInDeliverySummary(ByRef arrDelvSummary As ArrayList)_
    '                                                       As Boolean

    Public MustOverride Function GetBookInDeliverySummary(ByRef arrDelvSummary As ArrayList, ByRef arrDallasDelSummary As ArrayList) _
                                                          As Boolean

    Public MustOverride Function ValidateUODScanned(ByVal strUODNumber As String, _
                                                     ByRef objUODInfo As UODInfo, _
                                                     ByVal FuncType As AppContainer.FunctionType) _
                                                     As Boolean

    'V1.1 - KK
    'Added new function ValidateDallasUODScanned() as part of Dallas Positive Receiving Project

    Public MustOverride Function ValidateDallasUODScanned(ByVal cUODNumber As String, _
                                                     ByRef objDalUODInfo As GIValueHolder.DallasScanDetail, _
                                                     ByRef arrDallasDelSummary As ArrayList, _
                                                     ByVal FuncType As AppContainer.FunctionType) _
                                                     As Boolean
    
    Public MustOverride Function SendBookInDetails(ByRef arrUODDetails As ArrayList) _
                                                   As Boolean

    Public MustOverride Function SendBatchConfirmation(ByRef objDriverDetails As GIValueHolder.DriverDetails) As Boolean

    Public MustOverride Function SendDeliveryConfirmation(ByRef objDriverDetails As GIValueHolder.DriverDetails) As Boolean

    ' V1.1 - CK
    ' New function SendDallasDeliveryConfirmation() for sending the Dallas UOD receipt message DAR
    Public MustOverride Function SendDallasDeliveryConfirmation(ByRef arrDallasUODDetails As ArrayList) As Boolean

    Public MustOverride Function SendSessionExit(ByVal tyDeliveryType As AppContainer.DeliveryType, _
                                                 ByVal tyFunction As AppContainer.FunctionType, _
                                                 ByVal tyIsAbort As AppContainer.IsAbort) As Boolean

    Public MustOverride Function GetAuditSummary() As Boolean

    Public MustOverride Function GetItemDetails(ByVal strItemCode As String, _
                                                ByVal strType As AppContainer.ItemDetailType, _
                                                ByRef objItemInfo As ItemInfo) As Boolean

    Public MustOverride Function GetUODDetails(ByVal strUODNumber As String, _
                                               ByRef objUODInfo As UODInfo, _
                                               ByRef arrItemDetails As ArrayList) As Boolean
    Public MustOverride Function GetChildUODDetails(ByVal strUODNumber As String, _
                                               ByRef objUODInfo As UODInfo, _
                                               ByRef arrItemDetails As ArrayList) As Boolean
    Public MustOverride Function GetUODListForView(ByVal strPeriod As String, _
                                                   ByRef arrUODList As ArrayList) As Boolean
    'V1.1 - KK
    'Added new function for getting Dallas UOD list for view
    Public MustOverride Function GetDallasUODListForView(ByRef arrDalViewUOD As ArrayList) As Boolean

    Public MustOverride Function GetCrateListForView(ByVal strUODNumber As String, _
                                                     ByRef arrCrateList As ArrayList, _
    Optional ByVal objUODInfo As UODInfo = Nothing, _
                                                     Optional ByVal strSequence As String = "XXXXX") As Boolean

    Public MustOverride Function GetItemListForView(ByVal strCrateId As String, _
                                                    ByRef arrItemList As ArrayList, _
                                    Optional ByVal objUODinfo As UODInfo = Nothing, _
                                     Optional ByVal strSequence As String = "XXXXX") As Boolean

    Public MustOverride Function GetSupplierList(ByRef arrSupplierList As ArrayList) As Boolean

    Public MustOverride Function GetOrderList(ByVal strSupplierNo As String, _
                                              ByRef arrOrderList As ArrayList) As Boolean

    Public MustOverride Function GetItemListForOrder(ByRef objOrderList As GIValueHolder.OrderList, _
                                                     ByRef arrOrderList As ArrayList) _
                                                     As Boolean

    Public MustOverride Function GetSupplierData(ByVal strSupplierNumber As String, _
                                                 ByRef objSupplierData As SupplierList, _
                                                 ByRef tyDelType As AppContainer.DeliveryType, _
                                                 ByVal tyFunction As AppContainer.FunctionType) As Boolean
#If RF Then
    Public MustOverride Function GetCartonDetails(ByVal strCartonNumber As String, _
                                               ByRef arrItemDetails As ArrayList, _
                                               ByVal tyDelType As AppContainer.DeliveryType, _
                                               ByVal tyFunc As AppContainer.FunctionType, _
                                               Optional ByRef objCartonInfo As GIValueHolder.CartonInfo = Nothing) As Boolean
#End If
#If NRF Then
    Public MustOverride Function GetCartonDetails(ByVal strCartonNumber As String, _
                                              ByRef arrItemDetails As ArrayList, _
                                              ByVal tyDelType As AppContainer.DeliveryType, _
                                              ByVal tyFunc As AppContainer.FunctionType, _
                                               ByVal bBkd As Boolean, _
                                              Optional ByRef objCartonInfo As GIValueHolder.CartonInfo = Nothing) As Boolean
#End If


    Public MustOverride Function SendItemDetails(ByRef arrItemDetails As ArrayList, _
                                                 ByVal tyDelType As AppContainer.DeliveryType, _
                                                 ByVal tyFunction As AppContainer.FunctionType) As Boolean

    Public MustOverride Function SendAuditSession() As Boolean

    Public MustOverride Function ValidateCartonScanned(ByVal strASNNumber As String, _
                                                       ByRef objCartonInfo As GIValueHolder.CartonInfo, _
                                                       ByVal tyDelType As AppContainer.DeliveryType, _
                                                       ByVal tyFunction As AppContainer.FunctionType) As Boolean



    'Public MustOverride Function SendSessionExit(ByVal enAbort As IsAbort) As Boolean

    Public MustOverride Function GetSupplierDetails(ByVal strSupplierNo As String, _
                                                    ByRef arrSupplierDetails As ArrayList) As Boolean

    Public MustOverride Function SendItemQuantity(ByRef iItemCount As Integer, _
                                                  ByVal DelType As AppContainer.DeliveryType, _
                                                  ByVal FuncType As AppContainer.FunctionType) As Boolean

    Public MustOverride Function GetSupplierListForView(ByRef arrSupplierList As ArrayList) As Boolean

    Public MustOverride Function SendCartonDetails(ByRef arrCartonDetails As ArrayList, _
                                                 ByVal tyDelType As AppContainer.DeliveryType, _
                                                 ByVal tyFunction As AppContainer.FunctionType) _
                                                 As Boolean
    Public MustOverride Function GetUODChildCount(ByVal strUODNumber As String, _
                                                  ByRef iChildCOunt As Integer) As Boolean

    Public MustOverride Function GetUserDetails(ByVal strUDetId As String, _
                                                ByRef objUserInfo As UserInfo) As Boolean

    Public MustOverride Function LogOff(Optional ByVal isCallForCrash As Boolean = False) As Boolean

    Public MustOverride Function SignOn(ByRef strPassword As String) As Boolean

    Public MustOverride Function GetBootsCode(ByVal strProductCode As String) As String
    Public MustOverride Function GetItemList(ByVal strCartonNumber As String, _
                                           ByVal arrItemList As ArrayList) As Boolean
#If NRF Then
    Public MustOverride Function ValidateSupplierNumber(ByVal SupplierNo As String, _
                                                       ByRef objSupplierData As SupplierData) As Boolean
#End If
#If RF Then
     Public MustOverride Function ValidateSupplierNumber(ByVal SupplierNo As String, ByRef arrSupplierNumber As ArrayList) As Boolean

#End If

    Public MustOverride Function GetCartonList(ByVal strSupplierNo As String, _
                                              ByRef arrCartonList As ArrayList) As Boolean
    'V1.1 - KK
    'Added new function CheckDallasStore() as part of Dallas Positive Receiving Project
#If RF Then
    Public MustOverride Function CheckDallasStore() As Boolean
#End If

End Class



