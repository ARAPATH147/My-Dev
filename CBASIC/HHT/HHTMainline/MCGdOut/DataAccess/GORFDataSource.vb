#If RF Then
Imports System.Threading
Imports System.Text.RegularExpressions

Public Class GORFDataSource
    ''' <summary>
    ''' Gets the roduct details using the product code.
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="objGOItemInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProductInfoUsingPC(ByVal strProductCode As String, ByRef objGOItemInfo As GOItemInfo) As Boolean
        Dim bTemp As Boolean = False
        Try
            'ambli
            'System testing
            If strProductCode.Length <= 7 Then
                strProductCode = objAppContainer.objHelper.GenerateBCwithCDV(strProductCode)
            Else
                strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(strProductCode)
            End If
            If objAppContainer.objExportDataManager.CreateENQ(strProductCode) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is EQRRecord Then
                        Dim objEQR As EQRRecord = CType(objResponse, EQRRecord)
                        With objGOItemInfo
                            .BootsCode = objEQR.strBootsCode
                            .BusinessCentreType = objEQR.cBusinessCentre
                            'System Testing - Modified Lakshmi
                            .Description = objEQR.strSELDesc
                            .FirstBarcode = objEQR.strBarcode
                            .ItemPrice = objEQR.strPrice
                            'System Testing - Lakshmi
                            .ProductCode = objEQR.strBarcode.Remove(objEQR.strBarcode.Length - 1, 1)
                            If objEQR.strStockFigure.Trim = "" Then
                                .TSF = "0"
                            Else
                                .TSF = objEQR.strStockFigure
                            End If
                            .SecondBarcode = objEQR.strBarcode
                            'System Testing - Modified Lakshmi
                            .ShortDescription = objEQR.strItemDesc
                            .Status = objEQR.cStatus
                            .SupplyRoute = objEQR.cSupply
                            If objEQR.strStockFigure.Trim = "" Then
                                .TSF = "0"
                            Else
                                .TSF = objEQR.strStockFigure
                            End If
                            .strRecallType = objEQR.strRecallType

                        End With
                        bTemp = True
                        objResponse = Nothing
                        objEQR = Nothing
                    End If
                End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception: " + ex.Message + _
                                            "@ GetProduct Info Using PC", Logger.LogLevel.INFO)
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            If ex.Message = Macros.CONNECTIVITY_TIMEOUTCANCEL And objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.RECALL Then
                Throw ex
            End If
#End If
        End Try
        Return bTemp
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
    ''' Get the list of recalls available for the store.
    ''' </summary>
    ''' <param name="arrRecallIst"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRecallList(ByRef arrRecallIst As ArrayList, ByVal m_RecallCount As RecallCount, Optional ByVal strRecallTypes As String = "*,C,I") As Boolean
        Dim arrTempArrList As ArrayList = New ArrayList()
        Dim bTemp As Boolean = False
        Dim objResponse As Object = Nothing
        Dim objRecallInfo As RLRecallInfo = Nothing
        Dim objRCC As RCCRecord = Nothing
        Dim bRecallSpecialIns As Boolean = False
        Dim objStatusComparer As New RecallStatusComparer
        Try
            Dim arrRecallTypes() As String
            arrRecallTypes = strRecallTypes.Split(",")
            'If objAppContainer.objExportDataManager.CreateRCD(strRecallType) Then
            For Each strRecallType As String In arrRecallTypes
                arrTempArrList = New ArrayList()
                If objAppContainer.objExportDataManager.CreateRCD(strRecallType) Then
                    Do While (DATAPOOL.getInstance.GetNextObject(objResponse))
                        If TypeOf (objResponse) Is RCCRecord Then
                            objRCC = CType(objResponse, RCCRecord)
                            objRecallInfo = New RLRecallInfo()
                            With objRecallInfo
                                .ActiveDate = objRCC.strActiveDate
                                .RecallNumber = objRCC.strRecallRefNo
                                .RecallDescription = objRCC.strRecallDesc
                                .RecallType = objRCC.cRecallType
                                .RecallQuantity = objRCC.strRecallCount
                                If objRCC.strMRQ.Trim(" ") = "" Then
                                    .MinRecallQty = 0
                                Else
                                    .MinRecallQty = CInt(objRCC.strMRQ)
                                End If
                                .ListStatus = objRCC.strListStatus
                                .RecallMessage = objRCC.cSpecialInst
                                'Tailoring
                                .Tailored = objRCC.strTailored
                                .LabelType = objRCC.strLabType
                                'BATCH NOS
                                .BatchNos = objRCC.strBatchNos
                            End With
                            arrTempArrList.Add(objRecallInfo)
                            ' arrRecallIst.Add(objRecallInfo)
                            objRCC = Nothing
                            objRecallInfo = Nothing
                            objResponse = Nothing
                        ElseIf TypeOf (objResponse) Is NAKRecord Then
                            DATAPOOL.getInstance.ResetPoolData()
                            objResponse = Nothing
                            objRCC = Nothing
                            objRecallInfo = Nothing
                            arrTempArrList.Clear()
                            arrRecallIst.Clear()
                            Return False
                        End If
                    Loop
                    If arrTempArrList.Count > 0 Then
                        For Each objRecallItem As RLRecallInfo In arrTempArrList
                            If objRecallItem.RecallMessage = "Y" Then
                                'objAppContainer.objExportDataManager.CreateRCI(objRecallItem.RecallNumber, objRecallItem.RecallMessage)
                            Else
                                objRecallItem.RecallMessage = "N"
                            End If
                            arrRecallIst.Add(objRecallItem)
                        Next
                    End If

                    objRCC = Nothing
                    objRecallInfo = Nothing
                    objResponse = Nothing

                    arrTempArrList.Clear()
                    arrTempArrList = Nothing
                End If
            Next
            If arrRecallIst.Count > 0 Then
                bTemp = True
                arrRecallIst.Sort(0, arrRecallIst.Count, objStatusComparer)
            End If
            If bTemp Then
                'Recall CR
                'Get the list of recalls available for the store.
                Dim query_Emergency = From objEmergency As RLRecallInfo In arrRecallIst _
                Where objEmergency.RecallType = "E" _
                Select objEmergency

                Dim query_Withdrawn = From objWithdrawn As RLRecallInfo In arrRecallIst _
                Where objWithdrawn.RecallType = "W" _
                Select objWithdrawn

                Dim query_Returns = From objReturns As RLRecallInfo In arrRecallIst _
                Where objReturns.RecallType = "R" _
                Select objReturns

                Dim query_PlannerLeaver = From objPlannerLeaver As RLRecallInfo In arrRecallIst _
                Where objPlannerLeaver.RecallType = "I" _
                Select objPlannerLeaver

                Dim query_ExcessSalesplan = From objExcessSalesplan As RLRecallInfo In arrRecallIst _
                Where objExcessSalesplan.RecallType = "C" _
                Select objExcessSalesplan

                m_RecallCount.Customer = query_Emergency.Count
                m_RecallCount.Withdrawn = query_Withdrawn.Count
                m_RecallCount.Returns = query_Returns.Count
                m_RecallCount.PlannerLeaver = query_PlannerLeaver.Count
                m_RecallCount.ExcessSalesPlan = query_ExcessSalesplan.Count
            Else
                'If not recalls present then set all list counts to 0.
                m_RecallCount.Customer = 0
                m_RecallCount.Withdrawn = 0
                m_RecallCount.Returns = 0
                m_RecallCount.PlannerLeaver = 0
                m_RecallCount.ExcessSalesPlan = 0
            End If
            'End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            If ex.Message = Macros.CONNECTIVITY_TIMEOUTCANCEL And objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.RECALL Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("Error:: Get Recall List :: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' function to get all the recall items from the controller
    ''' </summary>
    ''' <param name="strRecallNo"></param>
    ''' <param name="arrRecallItems"></param>
    ''' <returns>Bool
    ''' True - If successfully recevied and updated the details in 
    ''' the object array.
    ''' False - If any occured during the updation.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetRecallItemDetails(ByVal strRecallNo As String, ByVal iRecallQty As Integer, ByRef arrRecallItems As ArrayList) As Boolean
        Dim bTemp As Boolean = False
        Dim objResponse As Object = Nothing
        Dim objIteminfo As GOItemInfo = Nothing
        Dim objRLItemInfo As RLItemInfo = Nothing
        Dim bFirstRequest As Boolean = True
        Try
            If (objAppContainer.objExportDataManager.CreateRCH(strRecallNo)) Then
                Do While (DATAPOOL.getInstance.GetNextObject(objResponse))
                    If TypeOf (objResponse) Is RCFRecord Then
                        For Each objRecallItemInfo As RCFInnerRecord In CType(objResponse, RCFRecord).InnerRecordArrays
                            objRLItemInfo = New RLItemInfo()
                            With objRLItemInfo
                                .BootsCode = objAppContainer.objHelper.GenerateBCwithCDV(objRecallItemInfo.strRecallItem)
                                .Description = objRecallItemInfo.strItemDesc
                                .RecallItemStatus = objRecallItemInfo.cItemFlag

                                .UODCount = objRecallItemInfo.strRecallCount
                                If objRecallItemInfo.strRecallCount.Trim() = "" Then
                                    .StockCount = " "
                                Else
                                    .StockCount = objRecallItemInfo.strRecallCount
                                End If

                                If (objRecallItemInfo.strTSF.Trim = "") Then
                                    .TSF = 0
                                Else
                                    .TSF = objRecallItemInfo.strTSF
                                End If
                                   
                                If objRecallItemInfo.cItemFlag = "Y" Then
                                    If objRecallItemInfo.strRecallCount.Trim() = .TSF Then
                                        .Status = "Actioned"
                                    Else
                                        .Status = "Discrepancy"
                                    End If
                                    .RecallItemStatus = Macros.RECALL_ITEM_PICKED
                                    'Fix to avoid displaying the item count in a completed recall list
                                    'Scan status will be set to true only if there is a connection loss and
                                    'the user is taken out of recalls. The when reloading will have items flag 
                                    'Set to Y. In other cases such as using more than 1 UOD or completing with
                                    '1 UOD the item flag will not be 'Y'.
                                    .ScanStatus = True
                                'FIX for DEFECT BTCPR00005012(PPC/POD - Completed recalls - View - Status against each item should be Actioned not Unactioned)
                                ElseIf CInt(.UODCount) >= 0 Then
                                    'If Val(objRecallItemInfo.strRecallCount.Trim()) = .TSF Then
                                        .Status = "Actioned"
                                    'Else
                                    '    .Status = "Discrepancy"
                                    'End If
                                    .RecallItemStatus = Macros.RECALL_ITEM_PICKED
                                Else
                                    .Status = "Unactioned"
                                    .RecallItemStatus = Macros.RECALL_ITEM_UNPICKED
                                    .StockCount = " "   'Set the stock count to nothing if ths item is encountered for the second time.
                                End If
                                If objRecallItemInfo.cVisible = "Y" Then
                                    .TailoringFlag = True
                                Else
                                    .TailoringFlag = False
                                End If
                            End With
                            arrRecallItems.Add(objRLItemInfo)
                            objRLItemInfo = Nothing
                            'objIteminfo = Nothing
                        Next
                        bTemp = True
                    End If
                Loop
            End If
        Catch ex As Exception
            '  If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Or _
             (ex.Message = Macros.CONNECTIVITY_TIMEOUTCANCEL And objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.RECALL) Then

                Throw ex
            End If
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' gets the list of suppliers
    ''' </summary>
    ''' <param name="arrSupplierList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSupplierList(ByRef arrSupplierList As ArrayList, ByVal strSupplierBC As String) As Boolean
        Dim bTemp As Boolean = False
        Dim objResponse As Object = Nothing
        Dim objDSR As DSRRecord = Nothing
        Dim objSupplierInfo As SupplierList = Nothing
        Try
            If strSupplierBC = "R" Then
                strSupplierBC = "*"
            End If
            If (objAppContainer.objExportDataManager.CreateDSS()) Then
                If (objAppContainer.objExportDataManager.CreateDSG(GOSessionMgr.GetInstance().BusinessCentre)) Then
                    Do While (DATAPOOL.getInstance.GetNextObject(objResponse))
                        If Not (TypeOf (objResponse) Is DSRRecord) Then
                            DATAPOOL.getInstance.ResetPoolData()
                            Return bTemp
                        Else
                            objDSR = CType(objResponse, DSRRecord)
                            objSupplierInfo = New SupplierList()
                            With objSupplierInfo
                                .SupplierID = objDSR.strSupplierNo
                                .SupplierName = objDSR.strSupplierName
                            End With
                            arrSupplierList.Add(objSupplierInfo)
                            objSupplierInfo = Nothing
                            objResponse = Nothing
                        End If
                    Loop
                    bTemp = True
                End If
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' Check if the product scanned is a valid product using Boots Code.
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckIsProductValidUsingBC(ByVal strBootsCode As String) As Boolean

    End Function
    ''' <summary>
    ''' Check if the product scanned is a valid product using Product Code.
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckIsProductValidUsingPC(ByVal strProductCode As String) As Boolean

    End Function
    ''' <summary>
    ''' To Add for User Auth 
    ''' </summary>
    ''' <param name="strUserID"></param>
    ''' <param name="objUserInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUserDetails(ByVal strUserID As String, ByRef objUserInfo As UserInfo) As Boolean
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
                            .user_ID = objSNR.strOperatorID
                            .user_Name = objSNR.strUserName
                            .accessFlag = objSNR.strAccessStock
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
            'log the exception
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
        Finally
            objResponse = Nothing
            objSNR = Nothing
        End Try
        Return True
    End Function
End Class
#End If
#Region "DATAPOOL"
#If RF Then
Class DATAPOOL
    Private bError As Boolean = False
    Private strErrMsg As String = ""
    Private areNotifier As AutoResetEvent
    Private m_DataArray As ArrayList
    Private m_Index As Integer = 0
    Private bWaitingForNotification As Boolean = False
    Private Shared objDataPool As DATAPOOL
    Private bWaitingforConnection As Boolean = False
    Private m_ConnectionStatus As ConnectionStatus


    Public Enum ConnectionStatus
        Connected
        Disconnected
        MessageRecieved
        TimeOut
    End Enum

    ''' <summary>
    ''' Whether a active socket available for sending and recieving data. true - available, false - not available
    ''' </summary>
    ''' <remarks></remarks>
    Private bConnectionStatus As Boolean = False
    ''' <summary>
    ''' Reset event which is used to connect to the socket
    ''' </summary>
    ''' <remarks></remarks>
    Private areConnect As AutoResetEvent
    ''' <summary>
    ''' Whether a active socket available for sending and recieving data. true - available, false - not available
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property isConnected() As Boolean
        Get
            Return bConnectionStatus
        End Get
        Set(ByVal value As Boolean)
            bConnectionStatus = value
        End Set
    End Property
    Public Property WaitingForNotification() As Boolean
        Get
            Return bWaitingForNotification
        End Get
        Set(ByVal value As Boolean)
            bWaitingForNotification = value
        End Set
    End Property

    Public Property WaitingForConnection() As Boolean
        Get
            Return bWaitingforConnection
        End Get
        Set(ByVal value As Boolean)
            bWaitingforConnection = value
        End Set
    End Property

    ''' <summary>
    ''' Function to wait for Connection. the execution stops untill a connection is completed
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WaitForConnection() As ConnectionStatus
        Try
            areConnect = New AutoResetEvent(False)
            WaitingForConnection = True
            objAppContainer.objLogger.WriteAppLog("Waiting for Connection", Logger.LogLevel.RELEASE)
            Thread.CurrentThread.Priority = ThreadPriority.Lowest
            areConnect.WaitOne()
            Thread.CurrentThread.Priority = ThreadPriority.Lowest
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("DATAPOOL::Error Occured while waiting for a connection :: " _
                                                  + ex.Message, Logger.LogLevel.INFO)
        End Try
        Return m_ConnectionStatus
    End Function

    ''' <summary>
    ''' Notification for the Connection Status. 
    ''' </summary>
    ''' <param name="enumStauts"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function NotifyConnectionStatus(ByVal enumStauts As ConnectionStatus) As Boolean
        Try
            'Priority is set to lowest Becaause, if the object 
            'is not waiting then It might give some priority  for that thread
            'to execute and wait
            Thread.CurrentThread.Priority = ThreadPriority.Lowest
            If Not areConnect Is Nothing Then
                'Status denote connection status
                WaitingForConnection = False
                Thread.CurrentThread.Priority = ThreadPriority.Highest
                m_ConnectionStatus = enumStauts
                If m_ConnectionStatus = ConnectionStatus.Connected Then
                    isConnected = True
                End If
                areConnect.Set()
                Thread.CurrentThread.Priority = ThreadPriority.Highest
                objAppContainer.objLogger.WriteAppLog("Connection Notification Recieved", Logger.LogLevel.RELEASE)
                If isConnected Then
                    objAppContainer.objLogger.WriteAppLog("Connection status : CONNECTED")
                Else
                    objAppContainer.objLogger.WriteAppLog("Connection status : NOTCONNECTED")
                End If
            Else
                objAppContainer.objLogger.WriteAppLog("Nothing to notify", Logger.LogLevel.RELEASE)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("DATAPOOL::Error Occured while Notifying the connection Status :: " _
                                                + ex.Message, Logger.LogLevel.INFO)
        End Try
    End Function


    Public Property IsError() As Boolean

        Get
            Return bError
        End Get
        Set(ByVal value As Boolean)
            bError = value
        End Set
    End Property
    Public Property ErrorMessage() As String
        Get
            Return strErrMsg
        End Get
        Set(ByVal value As String)
            strErrMsg = value
        End Set
    End Property
    Private Sub New()
        m_DataArray = New ArrayList()
    End Sub
    Public Shared Function getInstance() As DATAPOOL
        If objDataPool Is Nothing Then
            objDataPool = New DATAPOOL()
        End If
        Return objDataPool
    End Function

    Public Function GetNextObject(ByRef objData As Object) As Boolean
        Dim bTemp As Boolean = False
        SyncLock (m_DataArray)
            If (Not (m_DataArray.Count = 0)) And (m_Index < m_DataArray.Count) Then
                objData = m_DataArray(m_Index)
                m_Index += 1
                bTemp = True
            Else
                bTemp = False
            End If
        End SyncLock
        Return bTemp
    End Function
    Public Sub addObject(ByVal objDataContainer As Object)
        m_DataArray.Add(objDataContainer)
    End Sub
    Public Sub ResetPoolData()
        bError = False
        strErrMsg = ""
        m_DataArray.Clear()
        m_Index = 0
    End Sub

    Public Property StoreData() As ArrayList
        Get
            Return m_DataArray
        End Get
        Set(ByVal value As ArrayList)
            m_DataArray = value
        End Set
    End Property
    Public Function WaitForNotification() As ConnectionStatus
        If isConnected Then
            objAppContainer.objLogger.WriteAppLog("DATAPOOL::Waiting for Response")
            WaitingForNotification = True
            Try
                If Not (areNotifier Is Nothing) Then
                    areNotifier = Nothing
                End If
                areNotifier = New AutoResetEvent(False)
                areNotifier.WaitOne()
                Thread.CurrentThread.Priority = ThreadPriority.Lowest
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("Error Occured In Data pool Wait for Notification" + _
                                                      ex.Message, Logger.LogLevel.RELEASE)
            End Try
            areNotifier = Nothing
        Else
            m_ConnectionStatus = ConnectionStatus.Disconnected
        End If
        Return m_ConnectionStatus
    End Function
    Public Sub NotifyDataEngine(ByVal Status As ConnectionStatus)
        Try
            If WaitingForNotification Then
                Thread.CurrentThread.Priority = ThreadPriority.Highest
                m_ConnectionStatus = Status
                areNotifier.Set()
                objAppContainer.objLogger.WriteAppLog("DATAPOOL::Recieved Notification")
                areNotifier = Nothing
            End If
            WaitingForNotification = False
        Catch ex As Exception
            'Log the exception Here
        End Try
    End Sub
    ''' <summary>
    ''' Ends the DATA POOL
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks>Call datpool end session only  when the application ends</remarks>
    Public Function EndSession() As Boolean
        Try
            If Not (objDataPool Is Nothing) Then
                objDataPool = Nothing
            End If
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function
End Class
#End If
#End Region