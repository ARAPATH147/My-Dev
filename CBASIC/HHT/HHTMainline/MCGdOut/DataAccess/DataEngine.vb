Imports System
Imports System.Data
''' ***************************************************************************
''' <fileName>DataEngine.vb</fileName>
''' <summary>Data Engine class being a part of the data access layer exposes 
''' APIs to business layer for accessing data from the location database.
''' </summary>
''' <author>Infosys Technologies Ltd.,</author>
''' <DateModified></DateModified>
''' <remarks></remarks>
''' ***************************************************************************
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Connects to alternate controller while primary is down
''' </Summary>
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
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, ByRef objGOItemInfo As GOItemInfo) As Boolean
#If NRF Then
        If isRFDevice Then
            'Code for RF device
        Else
            Dim objNRFDataSource As New GONRFDataSource()
            Try
                'Get the item details.
                Return objNRFDataSource.GetProductInfoUsingPC(strProductCode, objGOItemInfo)
            Catch ex As Exception
                Return False
            Finally
                'Clear the variables
                objNRFDataSource = Nothing
            End Try
        End If
#ElseIf RF Then
        Dim objRFDataSource As New GORFDataSource()
        Try
            Return objRFDataSource.GetProductInfoUsingPC(strProductCode, objGOItemInfo)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Data Engine , Get Product Info using PC - " + ex.Message, Logger.LogLevel.RELEASE)
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
        End Try
#End If
    End Function
    Public Function GetProductInfoUsingBC(ByVal strBootsCode As String, ByRef objGOItemInfo As GOItemInfo) As Boolean
#If NRF Then
            Dim objNRFDataSource As New GONRFDataSource()
            Try
                'Get the item details.
                Return objNRFDataSource.GetProductInfoUsingBC(strBootsCode, objGOItemInfo)
            Catch ex As Exception
                Return False
            Finally
                'Clear the variables
                objNRFDataSource = Nothing
            End Try
#ElseIf RF Then
        Dim objRFDataSource As New GORFDataSource()
        Try
            ''System testing - Lakshmi
            'strBootsCode = strBootsCode.Remove(strBootsCode.Length - 1, 1)
            Return objRFDataSource.GetProductInfoUsingPC(strBootsCode, objGOItemInfo)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Data Engine , Get Product Info using BC" + ex.Message, Logger.LogLevel.RELEASE)
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            If ex.Message = Macros.CONNECTIVITY_TIMEOUTCANCEL And objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.RECALL Then
                Throw ex
            End If
            Return False
        End Try
#End If

    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckIsProductValidUsingBC(ByVal strBootsCode As String) As Boolean
#If NRF Then
        If isRFDevice Then
            'Insert the code for RF based solution.
        Else
            Dim objNRFDataSource As New GONRFDataSource()
            Try
                'Get the item details.
                Return objNRFDataSource.CheckIsProductValidUsingBC(strBootsCode)
            Catch ex As Exception
                Return False
            Finally
                'Clear the variables
                objNRFDataSource = Nothing
            End Try
        End If
#ElseIf RF Then
#End If

    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckIsProductValidUsingPC(ByVal strProductCode As String) As Boolean
#If NRF Then
        If isRFDevice Then
            'Insert the code for RF based solution.
        Else
            Dim objNRFDataSource As New GONRFDataSource()
            Try
                'Get the item details.
                Return objNRFDataSource.CheckIsProductValidUsingPC(strProductCode)
            Catch ex As Exception
                Return False
            Finally
                'Clear the variables
                objNRFDataSource = Nothing
            End Try
        End If
#ElseIf RF Then
#End If
    End Function
    Public Function GetSupplierList(ByRef arrObjectList As ArrayList, ByVal strSupplierBC As String) As Boolean
#If NRF Then
           Dim objNRFDataSource As New GONRFDataSource()
            Try
                'Get the Count list from the local DB.
                Return objNRFDataSource.GetSupplierList(arrObjectList, strSupplierBC)
            Catch ex As Exception
                Return False
            Finally
                objNRFDataSource = Nothing
            End Try
#ElseIf RF Then
        Dim objRFDataSource As New GORFDataSource()
        Try
            Return objRFDataSource.GetSupplierList(arrObjectList, strSupplierBC)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
        Finally
            objRFDataSource = Nothing
        End Try

#End If
    End Function
    Public Function GetRecallList(ByRef arrObjectList As ArrayList, ByVal m_RecallCount As RecallCount, Optional ByVal strRecallType As String = "*,C,I") As Boolean
#If NRF Then
            Dim objNRFDataSource As New GONRFDataSource()
            Try
                'Get the Count list from the local DB.
            Return objNRFDataSource.GetRecallList(arrObjectList, strRecallType,m_RecallCount)
            Catch ex As Exception
                Return False
            Finally
                objNRFDataSource = Nothing
            End Try
#ElseIf RF Then
        Dim objRFDataSource As New GORFDataSource()
        Try
            'Get the Count list from the local DB.
            Return objRFDataSource.GetRecallList(arrObjectList, m_RecallCount, strRecallType)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            If ex.Message = Macros.CONNECTIVITY_TIMEOUTCANCEL And objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.RECALL Then
                Throw ex
            End If
            Return False
        Finally
            objRFDataSource = Nothing
        End Try
#End If
    End Function
    Public Function GetRecallItemDetails(ByVal strRecallNo As String, ByVal iRecallQty As Integer, ByRef arrObjectList As ArrayList, Optional ByVal isRecallTailored As Boolean = False) As Boolean
#If NRF Then
        Dim objNRFDataSource As New GONRFDataSource()
        Try
            'Get the item details for the products in a Count list.
            Return objNRFDataSource.GetRecallItemDetails(strRecallNo, iRecallQty, arrObjectList, isRecallTailored)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
#ElseIf RF Then
        Dim objRFDataSource As New GORFDataSource()
        Try
            'Get the item details for the products in a Count list.
            Return objRFDataSource.GetRecallItemDetails(strRecallNo, iRecallQty, arrObjectList)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' <summary>
    ''' To Add for User Auth
    ''' </summary>
    ''' <param name="strUserID"></param>
    ''' <param name="objUserInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserDetails(ByVal strUserID As String, ByRef objUserInfo As UserInfo) As Boolean
#If NRF Then
            Dim objNRFDataSource As New GONRFDataSource()
            Try
                'Get the item details.
                Return objNRFDataSource.GetUserDetails(strUserID, objUserInfo)
            Catch ex As Exception
                Return False
            Finally
                'Clear the variables
                objNRFDataSource = Nothing
            End Try
#ElseIf RF Then
        Dim objRFDataSource As New GORFDataSource()
        Try
            'Get the item details.
            Return objRFDataSource.GetUserDetails(strUserID, objUserInfo)
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
        Finally
            'Clear the variables
            objRFDataSource = Nothing
        End Try
#End If
    End Function
    ''' Gets the device type from the config file.
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
#If NRF Then
    Public Function CheckIteminModuleListItems(ByVal strBootsCode As String, ByVal iCount As Integer, _
                                               ByRef objItemsInPlanner As Hashtable, Optional ByVal bCheckFullPlanner As Boolean = False) As Boolean
        Dim objNRFDataSource As New GONRFDataSource()
        Try
            'Get the item details.
            Return objNRFDataSource.CheckIteminModuleListItems(strBootsCode, iCount, objItemsInPlanner, bCheckFullPlanner)
        Catch ex As Exception
            Return False
        Finally
            'Clear the variables
            objNRFDataSource = Nothing
        End Try
    End Function
    'v1.1 MCF Change
    '' <summary>
    '' change the IP to alternate controller IP
    '' </summary>
    '' <remarks>none</remarks>
    Public Sub sConnectAlternateInBatch()
        Try
            objAppContainer.iConnectedToAlternate = 1
            If objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString() Then
                objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.SECONDARY_IPADDRESS).ToString()
            Else
                objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString()
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in sConnectAlternateInBatch " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End If
End Class