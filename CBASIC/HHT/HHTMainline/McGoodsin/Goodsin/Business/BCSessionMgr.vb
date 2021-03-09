Imports System.Globalization
'''***************************************************************
''' <FileName>BCSessionMgr.vb</FileName>
''' <summary>
''' The BookInOrder Container Class.
''' Implements all business logic and GUI navigation for Audit Carton.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>08-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 1.1 for PPC</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
'''****************************************************************************
''' 1.1    Archana Chandramathi    Jul 2013    
''' <Summary>
''' ISO 008-009 Changes : Change the flow when a boots.com/ie not on carton
''' item booked in at the store 
''' </Summary>
''' 1.2    Kiran Krishnan          15 April 2016
''' <Summary>
''' The Version 1.1 changes was missing from HHT Main stream 
''' causing issue with Boots.com not on file deliveries in stores.
''' Merged the changes related to V1.1 back in this module.
''' </Summary>
''' 1.3    Andrew Paton            06 May 2016
''' <Summary>
''' Order & Collect Parcel Management
''' When a Boots.com/.ie Parcel is scanned, User will be prompted to use the 
''' Order & Collect App, GoodsIn will no longer booking boots.com /.ie parcels 
''' </Summary>
''' 1.4    Charles Skadorwa         04 Aug 2016
''' <Summary>
''' Modified sub SupplierNoEntry to suppress Boots.com booking in.
''' Commented: O&C CSK 4/8/2016
''' </Summary>
'''****************************************************************************
Public Class BCSessionMgr
    Private Shared m_BCSessionMgr As BCSessionMgr
    Private m_BookInCartonExpectedOrder As frmBookInCartonExpectedOrder
    Private m_BookInCartonScanCarton As frmBookInCartonScanCarton
    Private m_BookInCartonSummary As frmBookInCartonSummary
    Private m_BookInCartonScanItem As frmBookInCartonScanItem
    Private m_BookInCartonItemInfo As frmBookInCartonItemInfo
    Private m_BookInOrderInitial As frmBookInOrderInitial
    Private m_BookInOrderSummaryOfContents As frmBookInOrderSummaryOfContents
    Private m_BookInOrderSummary As frmBookInOrderSummary
    Private m_BookInItemForNoOrderNumber As frmBookInItemForNoOrderNumber
    Private m_FinalBookInItemSummary As frmFinalBookInItemSummary

    Private m_ItemViewList As ArrayList = Nothing
    Private m_SupplierList As ArrayList = Nothing
    'Private m_CartonList As ArrayList = Nothing
    'CHANGE
    Public m_CartonList As ArrayList = Nothing
    Public m_ItemList As ArrayList = Nothing
    Private m_OrderList As ArrayList = Nothing
    Private m_ItemListForOrder As ArrayList = Nothing
    Private m_ItemListForOrderDetails As ArrayList = Nothing
    Private m_CartonDetails As GIValueHolder.CartonDetails
    Private m_CartonInfo As GIValueHolder.CartonInfo
    Private m_BCItemInfo As ItemInfo
    Private m_CartonCount As Integer
    ' Private m_ItemList As ArrayList = Nothing
    Private m_objItem As Item
    Private m_strScanCheck As Boolean
    Private m_bInOrder As Boolean
    Private m_Outstanding As String
    Private m_SupplierName As String
    Private m_SupplierNo As String
    Private m_SupplierType As String
    Private m_ItemOrder As String
    Private m_OrderNo As String
    Private m_Qty As String
    Private m_objASNCode As GIValueHolder.ASNCode
    Private m_iOrderExpectedQty As Integer
    Private m_CartonNIFItemCount As Integer
    Private m_strBookInCaronFinish As Boolean
    Public m_strSupplierSelectionCheck As String
    Public m_strBookIncartonShowMsg As String
    Public m_OrderNumber As String = "X".PadLeft(20, "X")
    Public m_PageNo As String = "00"
    Private m_tyDeliveryType As AppContainer.DeliveryType = AppContainer.DeliveryType.ASN
    Private bIsNewSessionAfterConnectionLoss As Boolean = True
    Private m_bIsDotcomItem As Boolean
    ''' <summary>
    ''' Functions for getting the object instance for the BCSessionMgr. 
    ''' Use this method to get the object refernce for the Singleton BCSessionMgr.
    ''' </summary>
    ''' <returns>Object reference of BCSessionMgr Class</returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As BCSessionMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.BOOKINCARTON
        If m_BCSessionMgr Is Nothing Then
            m_BCSessionMgr = New BCSessionMgr()
            Return m_BCSessionMgr
        Else
            Return m_BCSessionMgr
        End If
    End Function
    ''' <summary>
    ''' Initialises the BookInOrder Session 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartSession()
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
            m_BookInCartonExpectedOrder = New frmBookInCartonExpectedOrder
            m_BookInCartonScanCarton = New frmBookInCartonScanCarton
            m_BookInCartonSummary = New frmBookInCartonSummary
            m_BookInCartonScanItem = New frmBookInCartonScanItem
            m_BookInCartonItemInfo = New frmBookInCartonItemInfo
            m_BookInOrderInitial = New frmBookInOrderInitial
            m_BookInOrderSummaryOfContents = New frmBookInOrderSummaryOfContents
            m_BookInOrderSummary = New frmBookInOrderSummary
            m_BookInItemForNoOrderNumber = New frmBookInItemForNoOrderNumber
            m_FinalBookInItemSummary = New frmFinalBookInItemSummary
            m_strSupplierSelectionCheck = Nothing
            m_ItemViewList = New ArrayList
            m_SupplierList = New ArrayList
            m_CartonList = New ArrayList
            m_ItemListForOrder = New ArrayList
            m_ItemListForOrderDetails = New ArrayList
            m_OrderList = New ArrayList
            m_ItemList = New ArrayList
            m_objItem = New Item
            m_strScanCheck = False
            m_bInOrder = False
            m_iOrderExpectedQty = 0
            m_CartonDetails = New GIValueHolder.CartonDetails
            m_objASNCode = New GIValueHolder.ASNCode
            m_CartonInfo = New GIValueHolder.CartonInfo
            m_BCItemInfo = New ItemInfo
            m_SupplierName = Nothing
            m_SupplierNo = Nothing
            m_SupplierType = Nothing
            m_Outstanding = Nothing
            m_ItemOrder = Nothing
            m_OrderNo = Nothing
            m_Qty = Nothing
            m_strBookIncartonShowMsg = Nothing
            m_strBookInCaronFinish = False
            m_PageNo = Nothing
            m_CartonCount = 0
            m_CartonNIFItemCount = 0
            m_tyDeliveryType = AppContainer.DeliveryType.Directs
            m_PageNo = "00"
            m_bIsDotcomItem = False
            If objAppContainer.strAuditCartonNotinFile = Nothing Then
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                Me.DisplayBCScreen(BCSCREENS.ExpectedOrder)
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at BookIn carton start Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Screen Display method for BookInOrder. 
    ''' All BookIn sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName"></param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function DisplayBCScreen(ByVal ScreenName As BCSCREENS) As Boolean

        Try
            ' objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
            Select Case ScreenName
                Case BCSCREENS.ExpectedOrder
                    m_BookInCartonExpectedOrder.Invoke(New EventHandler(AddressOf DisplayExpectedOrder))
                Case BCSCREENS.CartonScan
                    m_BookInCartonScanCarton.Invoke(New EventHandler(AddressOf DisplayScanCarton))
                Case BCSCREENS.Summary
                    m_BookInCartonSummary.Invoke(New EventHandler(AddressOf DisplayCartonSummary))
                Case BCSCREENS.ItemScan
                    m_BookInCartonScanItem.Invoke(New EventHandler(AddressOf DisplayScanItem))
                Case BCSCREENS.ItemInfo
                    m_BookInCartonItemInfo.Invoke(New EventHandler(AddressOf DisplayBookIncartonItemInfo))
                Case BCSCREENS.BookInOrderInitial
                    m_BookInOrderInitial.Invoke(New EventHandler(AddressOf DisplayBookInOrderInitial))
                Case BCSCREENS.BookInOrderSummaryOfContents
                    m_BookInOrderSummaryOfContents.Invoke(New EventHandler(AddressOf DisplayBookInOrderSummaryOfContents))
                Case BCSCREENS.BookInOrderSummary
                    m_BookInOrderSummary.Invoke(New EventHandler(AddressOf DisplayBookInOrderSummary))
                Case BCSCREENS.BookInItemforNoOrder
                    m_BookInItemForNoOrderNumber.Invoke(New EventHandler(AddressOf DisplayBookInItemForNoOrder))
                Case BCSCREENS.FinalBookInItemSummary
                    m_FinalBookInItemSummary.Invoke(New EventHandler(AddressOf DisplayFinalBookInItemSummary))
            End Select
        Catch ex As Exception
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at BookIn carton Display Session: " + ex.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' Displays the expected order screen for BookIn carton
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayExpectedOrder(ByVal o As Object, ByVal e As EventArgs)
        Try
            Dim objItemComparer As New SupplierComparer
            m_BookInCartonExpectedOrder.lvwSuppliers.Items.Clear()
            objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONEXPECTEDORDER
#If RF Then
            If objAppContainer.objDataEngine.GetSupplierList(m_SupplierList) Then
#ElseIf NRF Then
            objAppContainer.objDataEngine.GetSupplierList(m_SupplierList)
#End If

            m_SupplierList.Sort(0, m_SupplierList.Count, objItemComparer)
            Dim objListView As ListViewItem = New ListViewItem
            'To populate the list view with supplier names and quantity
            For Each objSupplier As GIValueHolder.SupplierList In m_SupplierList
                'Displays the details only if qty greater than zero or static supplier
                'The below check is already there in PPC
                ' If CInt(objSupplier.SupplierQty) > 0 Or objSupplier.StaticSupplier = "S" Then
                objListView = m_BookInCartonExpectedOrder.lvwSuppliers.Items.Add(New ListViewItem(objSupplier.SupplierName))
                objListView.SubItems.Add(objSupplier.SupplierQty.ToString())
                ' End If

            Next
            objAppContainer.objStatusBar.SetMessage("")

            With m_BookInCartonExpectedOrder
                .lblCode.Text = ""
                .lblCode.Focus()
                .Visible = True
                .Refresh()
            End With
            'To display status bar message
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            objAppContainer.objPrevMod = AppContainer.ACTIVEMODULE.BOOKINCARTON
#If RF Then

            End If
#End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at BookIn carton start Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try

    End Sub
    ''' <summary>
    ''' Displays the carton scanning screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayScanCarton(ByVal o As Object, ByVal e As EventArgs)
        m_tyDeliveryType = AppContainer.DeliveryType.ASN
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONSCAN
        With m_BookInCartonScanCarton
            .txtProductCode.Text = ""
            .txtProductCode.Focus()
            .lblSupplier.Text = m_SupplierName
            .lblBookedInCount.Text = m_CartonCount.ToString()
            .lblOutstandingCount.Text = m_Outstanding
            If m_Outstanding = Nothing Then
                .lblOutstandingCount.Text = Quantity.Zero
            End If
            'If m_CartonList.Count = 0 And m_ItemList.Count = 0 Then
            '    .Btn_Finish1.Visible = False
            'Else
            '    .Btn_Finish1.Visible = True
            'End If
            objAppContainer.objStatusBar.SetMessage("")
            .Visible = True
            .Refresh()
        End With
        'To display status bar message
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
    End Sub
    ''' <summary>
    ''' Displays the Carton BookIn summary screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayCartonSummary(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONSUMMARY
        'If summary screen is displayed after clicking finish then carton quantity and message is shown
        If m_strBookInCaronFinish Then
            m_BookInCartonSummary.lblMsg.Visible = True
            m_BookInCartonSummary.lblCartonCount.Text = m_CartonCount.ToString()
            m_strBookInCaronFinish = False
            'If summary screen is displayed by quitting from intermediate
            ' screens then the quantity will be zero and message wont be shown
        ElseIf m_strBookInCaronFinish = False Then
            m_BookInCartonSummary.lblCartonCount.Text = Quantity.Zero

        End If

        With m_BookInCartonSummary
            .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' Displays the Item scanning screen for BookIn carton Not in file.
    ''' Also for Audit Carton Not in file
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayScanItem(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONITEMSCAN
        objAppContainer.objStatusBar.SetMessage("")
        m_tyDeliveryType = AppContainer.DeliveryType.ASN
        With m_BookInCartonScanItem
            .txtProductCode.Text = ""
            .txtProductCode.Focus()
            'for Audit Carton Not in file 'Next Carton' button is not needed
            If objAppContainer.strAuditCartonNotinFile = "NIF" Then
                .btnNextCarton.Visible = False
            End If
            'If m_ItemList.Count > 0 Then
            '    .Btn_Finish1.Visible = True
            'Else
            '    .Btn_Finish1.Visible = False
            'End If
            .Visible = True
            .Refresh()
        End With
        'To display status bar message
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
    End Sub

    ''' <summary>
    ''' Displays the ItemInfo screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayBookIncartonItemInfo(ByVal o As Object, ByVal e As EventArgs)
        Try

            objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONITEMINFO
            Dim objDescriptionArray As ArrayList = New ArrayList
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(m_BCItemInfo.ItemDesc)
            objAppContainer.objStatusBar.SetMessage("")
            With m_BookInCartonItemInfo
                'For Order the order number is shown
                If m_ItemOrder = BookInOrder.Order Then
                    .lblOrder.Text = "Order  " + m_OrderNo
                    .lblBookin.Text = "Book In Order"
                End If
                If m_ItemOrder = BookInOrder.NoOrder Then
                    .lblOrder.Text = "No Order Number"
                    .lblBookin.Text = "Book In Order"
                End If
                'for Audit Carton Not in file Supplier name is not needed
                If objAppContainer.strAuditCartonNotinFile = "NIF" Then
                    .lblSupplier.Visible = False

                End If

                .lblSupplier.Text = m_SupplierName
                'Displaying the item description in 3 lines
                .lblItemDesc.Text = objDescriptionArray.Item(0).ToString()
                .lblItem2.Text = objDescriptionArray.Item(1).ToString()
                .lblItem3.Text = objDescriptionArray.Item(2).ToString()
                If (m_BCItemInfo.BootsCode.Length <= 7) Then
                    .lblBootscode.Text = objAppContainer.objHelper.FormatBarcode(m_BCItemInfo.BootsCode)
                Else
                    .lblBootscode.Text = ""
                End If
                If (m_BCItemInfo.ProductCode.Length <= 7) Then
                    .lblEAN.Text = ""
                Else
                    .lblEAN.Text = objAppContainer.objHelper.FormatBarcode(m_BCItemInfo.ProductCode)
                End If

                'If the item is previously scanned then the corresponding quantity is displayed
                'else the quantity is prepopulated with value 1

                If Not m_Qty = Nothing Then
                    .lblQty.Text = m_Qty
                Else
                    If m_strScanCheck Then
                        .lblQty.Text = m_BCItemInfo.ItemQty.ToString()
                        m_strScanCheck = False

                    Else
                        .lblQty.Text = Quantity.One
                    End If
                End If
                .Visible = True
                .Refresh()
            End With
            'To display status bar message
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at BookIn carton Display BookInCartonItemInfo Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Displays the bookInOrder item screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayBookInOrderInitial(ByVal o As Object, ByVal e As EventArgs)
        Try
            Dim objItemComparer As New OrderComparer
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERINITIAL
            objAppContainer.objDataEngine.GetOrderList(m_SupplierNo, m_OrderList)
#If RF Then
            If Not objAppContainer.bCommFailure And Not objAppContainer.bReconnectSuccess Then
#End If
            m_OrderList.Sort(0, m_OrderList.Count, objItemComparer)

            Dim objListView As ListViewItem = New ListViewItem
            'Populates the list view with orders which are unbooked
            For Each objOrder As GIValueHolder.OrderList In m_OrderList
                If objOrder.BookInStatus = Status.UnBooked Then
                    ReformatDate(objOrder.EstDeliveryDate)
                    objListView = m_BookInOrderInitial.lvwOrders.Items.Add(New ListViewItem(objOrder.OrderNo))
                    objListView.SubItems.Add(objOrder.EstDeliveryDate)
                End If
            Next
            objAppContainer.objStatusBar.SetMessage("")
            With m_BookInOrderInitial
                .lblSupplier.Text = m_SupplierName
                .Visible = True
                .Refresh()
            End With
#If RF Then
            ElseIf objAppContainer.bReconnectSuccess Then
                With m_BookInCartonExpectedOrder
                    .Visible = True
                    .Refresh()
                End With
            End If
#End If
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at BookIn carton Display BookInOrder Initial Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try

    End Sub
    ''' <summary>
    ''' Displays the BookInOrder Summary of Contents screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayBookInOrderSummaryOfContents(ByVal o As Object, ByVal e As EventArgs)
        Try
            Dim objItemComparer As New GenericComparer
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARYOFCONTENTS
            SetItemOrder()
            m_BookInOrderSummaryOfContents.lvwSumOfContents.Items.Clear()
            Dim objListView As ListViewItem = New ListViewItem
            'Populates the listview with item details of the order
            m_ItemViewList.Sort(0, m_ItemViewList.Count, objItemComparer)
            For Each objItem As Item In m_ItemViewList
                Dim strBootsCode As String = objAppContainer.objHelper.FormatBarcode(objItem.strItemNo)
                objListView = m_BookInOrderSummaryOfContents.lvwSumOfContents.Items.Add(New ListViewItem(strBootsCode))
                objListView.SubItems.Add(objItem.strDesc)
                objListView.SubItems.Add(objItem.iExpected.ToString())
                objListView.SubItems.Add(objItem.iRecd.ToString())
            Next
            objAppContainer.objStatusBar.SetMessage("")
            With m_BookInOrderSummaryOfContents
                .lblOrderNo.Text = m_OrderNo
                .lblSupplier.Text = m_SupplierName
                .Visible = True
                'If m_ItemList.Count > 0 Then
                '    .Btn_Finish1.Visible = True
                'Else
                '    .Btn_Finish1.Visible = False
                'End If
                .Refresh()
            End With
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            'Check in case of reconnect, if the carton and supplier are same then set the item details.
            'else clear the items.
            Try
#If RF Then


                If objAppContainer.m_SavedDetails.Count > 0 And objAppContainer.eDeliveryType = AppContainer.DeliveryType.Directs And _
                objAppContainer.eFunctionType = AppContainer.FunctionType.BookIn Then
                    Dim objCartonItem As GIValueHolder.ScanDetails = objAppContainer.m_SavedDetails(0)
                    Dim objTempCode As String = ""
                    For Each objOrder As GIValueHolder.OrderList In m_OrderList
                        If objOrder.OrderNo = m_OrderNo Then
                            objTempCode = objOrder.Code.Substring(0, objOrder.Code.Length - 2) + m_PageNo
                            Exit For
                        End If
                    Next

                    If objCartonItem.ScannedCode.StartsWith("0000000000") Or _
                       objCartonItem.ScannedCode.StartsWith("000000" + m_OrderNo) Or _
                       objCartonItem.ScannedCode.StartsWith(m_SupplierNo + m_OrderNo) Or _
                       objCartonItem.ScannedCode.StartsWith(m_SupplierNo + "0000") Or _
                       objCartonItem.ScannedCode.Equals(objTempCode) Then
                        m_ItemList = objAppContainer.m_SavedDetails
                        'm_objCarton.iCountItems = m_ItemList.Count
                        'The received qty is updated in order to display in the listview
                        'For Each objTempItem As GIValueHolder.ScanDetails In m_ItemList
                        '    For Each objItem As Item In m_ItemViewList
                        '        If objItem.strItemNo = objTempItem.BootCode Then
                        '            objItem.iRecd = objTempItem.ItemQty
                        '            Exit For
                        '        End If
                        '    Next
                        'Next
                    Else
                        objAppContainer.eDeliveryType = AppContainer.DeliveryType.Directs
                        objAppContainer.eFunctionType = AppContainer.FunctionType.BookIn
                        objAppContainer.m_SavedDetails = New ArrayList()
                    End If
                Else
                    objAppContainer.eDeliveryType = AppContainer.DeliveryType.Directs
                    objAppContainer.eFunctionType = AppContainer.FunctionType.BookIn
                    objAppContainer.m_SavedDetails = New ArrayList()
                End If
                'Set the boolean session state to false.
                bIsNewSessionAfterConnectionLoss = False
#End If
            Catch ex As Exception
                AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit Carton Display Carton Item Scan: " + ex.Message + ex.ToString(), _
                                                                    Logger.LogLevel.RELEASE)
            End Try
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at BookIn carton Display BookInOrder Summary of contents Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    ''' <summary>
    ''' Displays the summary screen for BookInOrder item
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayBookInOrderSummary(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARY
        With m_BookInOrderSummary

            'In case where order number is there
            If m_ItemOrder = BookInOrder.Order Then
                .lblSummary.Text = "Summary"
                'In case of No order
            ElseIf m_ItemOrder = BookInOrder.NoOrder Then
                .lblSummary.Text = "No Order Number"
            End If
            .lblSupplier.Text = m_SupplierName
            .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' Displays the BookIn Item screen for no order
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayBookInItemForNoOrder(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINITEMFORNOORDERNUMBER
        SetItemOrder()
        objAppContainer.objStatusBar.SetMessage("")
        With m_BookInItemForNoOrderNumber
            .lblSupplier.Text = m_SupplierName
            .Visible = True
            'If m_ItemList.Count > 0 Then
            '    .Btn_Finish1.Visible = True
            'Else
            '    .Btn_Finish1.Visible = False
            'End If
            .Refresh()
        End With
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
    End Sub
    ''' <summary>
    ''' Displays the final summary screen for BookIn order of item
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayFinalBookInItemSummary(ByVal o As Object, ByVal e As EventArgs)
        Dim bShowListView As Boolean = False
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.FINALBOOKINITEMSUMMARY
#If RF Then
            objAppContainer.m_ModScreen = AppContainer.ModScreen.BCITEMFINISH
#End If
            With m_FinalBookInItemSummary
                If m_ItemOrder = BookInOrder.Order Then

                    Dim objListView As ListViewItem = New ListViewItem
                    For Each objItem As Item In m_ItemViewList

                        'Populates the listview with discripancies
                        If Not objItem.iExpected = objItem.iRecd Then
                            bShowListView = True
                            Dim strBootsCode As String = objAppContainer.objHelper.FormatBarcode(objItem.strItemNo)
                            objListView = m_FinalBookInItemSummary.lvwDiscrepancies.Items.Add(New ListViewItem(strBootsCode))
                            objListView.SubItems.Add(objItem.strDesc)
                            objListView.SubItems.Add(objItem.iExpected.ToString())
                            objListView.SubItems.Add(objItem.iRecd.ToString())
                        End If

                    Next
                    .lblOrder.Text = "Order    " + m_OrderNo

                    If bShowListView = False Then
                        .lvwDiscrepancies.Visible = False
                        .lblDiscr.Visible = False
                        .lblMsg2.Visible = True
                        .lblMsg.Visible = False
                    Else

                        .lblMsg2.Visible = False
                    End If
                ElseIf m_ItemOrder = BookInOrder.NoOrder Then
                    .lblOrder.Text = "No Order Number"
                    'For no order no discrepancies would be there
                    .lblDiscr.Visible = False
                    .lvwDiscrepancies.Visible = False
                    .lblMsg2.Visible = True
                    .lblMsg.Visible = False
                End If
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                .lblSupplier.Text = m_SupplierName
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at BookIn carton Display FinalBookInItemSummary Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Sets a variable to know whether BookIn is carried for 'Order' or 'No Order'
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetItemOrder()
        If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARYOFCONTENTS Then
            m_ItemOrder = BookInOrder.Order
            m_tyDeliveryType = AppContainer.DeliveryType.Directs
        ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINITEMFORNOORDERNUMBER Then
            m_ItemOrder = BookInOrder.NoOrder
            m_tyDeliveryType = AppContainer.DeliveryType.Directs
        End If
    End Sub
    ''' <summary>
    ''' To set the quantity of audited item and store the details to an arraylist
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetItemQty(ByVal strQty As String)
        Try
            m_BCItemInfo.ItemQty = CInt(strQty)
            '    m_BCItemInfo.ItemQty = CInt(m_BookInCartonItemInfo.lblQty.Text)
            Dim bIsPresentInList As Boolean = False
            'Checks whether the item is scanned previously
            For Each objPreviousItem As GIValueHolder.ScanDetails In m_ItemList
                If m_ItemOrder = BookInOrder.Order Or m_ItemOrder = BookInOrder.NoOrder Then

                    If objPreviousItem.ProductCode = m_BCItemInfo.BootsCode Or _
                    objPreviousItem.BootCode = m_BCItemInfo.BootsCode Then
                        objPreviousItem.ItemQty = m_BCItemInfo.ItemQty.ToString()
                        bIsPresentInList = True

                        For Each objItem As Item In m_ItemViewList
                            If objItem.strItemNo = m_BCItemInfo.BootsCode Then
                                objItem.iRecd = m_BCItemInfo.ItemQty
                                Exit For
                            End If
                        Next


                        Exit For
                    End If
                Else
                    If objPreviousItem.ProductCode = m_BCItemInfo.ProductCode Or _
                    objPreviousItem.BootCode = m_BCItemInfo.BootsCode Then
                        objPreviousItem.ItemQty = m_BCItemInfo.ItemQty.ToString()
                        bIsPresentInList = True
                        Exit For
                    End If
                End If
            Next
            'If scanned for the first time added to the arraylist
            If Not bIsPresentInList Then
                Dim objCurrentItem As New GIValueHolder.ScanDetails


                objCurrentItem.ScanType = ScanType.Booked
                objCurrentItem.ScanLevel = ScanLevel.Item
                objCurrentItem.ScanDate = Format(DateTime.Now, "yyyyMMdd")
                objCurrentItem.ScanTime = Format(DateTime.Now, "HHmmss")
                objCurrentItem.ItemQty = m_BCItemInfo.ItemQty.ToString()
                If m_ItemOrder = BookInOrder.Order Or m_ItemOrder = BookInOrder.NoOrder Then

                    'adding the not in order item to the arraylist to view the item in the listview
                    If Not m_bInOrder Then
                        Dim objTempItem As Item
                        objTempItem = New Item
                        objTempItem.strItemNo = m_BCItemInfo.BootsCode
                        objTempItem.strDesc = m_BCItemInfo.ItemDesc
                        objTempItem.iExpected = 0
                        objTempItem.iRecd = m_BCItemInfo.ItemQty
                        m_ItemViewList.Add(objTempItem)
                    End If
                    objCurrentItem.BootCode = m_BCItemInfo.BootsCode
                    'objCurrentItem.ScannedCode = m_OrderNumber.Substring(0, m_OrderNumber.Length - 2) + m_PageNo
                    'send 7 digit boots code for Book in Order Number
                    objCurrentItem.ProductCode = m_BCItemInfo.BootsCode
                    If m_ItemOrder = BookInOrder.Order Then
                        If m_bInOrder Then
                            For Each objOrder As GIValueHolder.OrderList In m_OrderList
                                If objOrder.OrderNo = m_OrderNo Then
                                    'objCurrentItem.ScannedCode = objOrder.Code
                                    objCurrentItem.ScannedCode = objOrder.Code.Substring(0, objOrder.Code.Length - 2) + m_PageNo
                                    Exit For
                                End If
                            Next
                        Else
                            If Not m_SupplierNo = "" Then
                                objCurrentItem.ScannedCode = m_SupplierNo + m_OrderNo + "  D00"
                            Else
                                objCurrentItem.ScannedCode = "000000" + m_OrderNo + "  D00"
                            End If
                        End If
                    ElseIf m_ItemOrder = BookInOrder.NoOrder Then
                        If Not m_SupplierNo = "" Then
                            objCurrentItem.ScannedCode = m_SupplierNo + "0000  D00"
                        Else
                            objCurrentItem.ScannedCode = "000000" + "0000  D00"
                        End If
                    End If
                    m_ItemList.Add(objCurrentItem)
                Else
                    'send 13 digit Product code for Book in Carton level
                    objCurrentItem.ScannedCode = objAppContainer.strNIFCartonCode
                    objCurrentItem.ProductCode = m_BCItemInfo.ProductCode
                    objCurrentItem.BootCode = m_BCItemInfo.BootsCode
                    'In case of BookIn carton Not in file
                    m_ItemList.Add(objCurrentItem)
                    m_CartonList.Add(objCurrentItem)
                    m_CartonNIFItemCount = m_CartonNIFItemCount + 1
                End If

                'The received qty is updated in order to display in the listview
                For Each objItem As Item In m_ItemViewList
                    If objItem.strItemNo = m_BCItemInfo.BootsCode Then
                        objItem.iRecd = m_BCItemInfo.ItemQty
                        Exit For
                    End If
                Next

            End If


            If m_ItemOrder = BookInOrder.Order Then
                DisplayBCScreen(BCSCREENS.BookInOrderSummaryOfContents)

            ElseIf m_ItemOrder = BookInOrder.NoOrder Then
                DisplayBCScreen(BCSCREENS.BookInItemforNoOrder)
                'Start : 1.1    Archana Chandramathi  ISO 008-009 Changes
            ElseIf m_bIsDotcomItem = True Then
                m_bIsDotcomItem = False
                'Increment the carton count
                CartonCountIncrement()
                'Display the next carton scan screen
                DisplayBCScreen(BCSCREENS.CartonScan)
                'End : 1.1    Archana Chandramathi  ISO 008-009 Changes
            Else
                DisplayBCScreen(BCSCREENS.ItemScan)
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at BookIn carton set item Qty Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try

    End Sub
    ''' <summary>
    ''' To increment the carton count after Book in of item in the case of Not in file
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CartonCountIncrement()
        Dim objScanDetails As New GIValueHolder.ScanDetails

        If m_CartonNIFItemCount > 0 Then
            m_CartonCount = m_CartonCount + 1
            With objScanDetails
                .ScannedCode = "X".PadLeft(20, "X")
                .ProductCode = "X".PadLeft(13, "X")
                .ScanDate = "X".PadLeft(8, "X")
                .ScanTime = "XXXXXX"
                .ItemQty = "XXXXXX"
                .ScanType = "B"
                .ScanLevel = "I"
                .ItemStatus = m_CartonNIFItemCount.ToString()
            End With

            m_CartonList.Add(objScanDetails)
        End If
        m_ItemList.Clear()
        m_CartonNIFItemCount = 0
    End Sub
    ''' <summary>
    ''' To check whether the quantity entered for an item 
    ''' is greater than expected in the case of BookIn order item
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckExpectedOrderQty(ByVal iQty As Integer) As Boolean
        If m_ItemOrder = BookInOrder.Order Then
            ' If (CInt(m_BookInCartonItemInfo.lblQty.Text) > m_iOrderExpectedQty) And Not (m_iOrderExpectedQty = 0) Then
            If (iQty > m_iOrderExpectedQty) And Not (m_iOrderExpectedQty = 0) Then
                Return False
            End If
        End If
        m_iOrderExpectedQty = 0
        Return True
    End Function
    ''' <summary>
    ''' Function to format the date
    ''' </summary>
    ''' <param name="strDate"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReformatDate(ByRef strDate As String) As String
        If strDate.Length = 8 Then
            strDate = strDate.Substring(2, 6)
        End If
        If strDate.Length = 6 Then
            Dim yy As String = strDate.Substring(0, 2)
            Dim mm As String = strDate.Substring(2, 2)
            Dim dd As String = strDate.Substring(4, 2)
            strDate = dd + "/" + mm + "/" + yy
            Return strDate
        Else
            Return strDate
        End If
    End Function

    ''' <summary>
    ''' Handles the item scan
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <remarks></remarks>
    Private Sub HandleItem(ByVal strBarcode As String)
        Dim iResult As Integer = 0
        Dim bValidate As Boolean = True
        Dim iMsg As Integer = 0
        Try
            'Validates Item code
            Dim bTemp As Boolean = False
            Dim bBootsCode As Boolean = True
            If strBarcode.Length < 8 Then
                'strBarcode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                bTemp = objAppContainer.objHelper.ValidateBootsCode(strBarcode)
                If Not bTemp Then
                    bBootsCode = False
                End If
            Else
                bTemp = objAppContainer.objHelper.ValidateEAN(strBarcode)
            End If
            'Validates Item code
            If Not bTemp Then


                'If item code is invalid
                bValidate = False
                If Not bBootsCode Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Error ", MessageBoxButtons.OK, _
                                                                                                 MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                Else
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                                                                                 MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                End If

                ' objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                'Barcode not recognised  and corresponding screens are displayed



                If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONITEMSCAN Then

                    DisplayBCScreen(BCSCREENS.ItemScan)
                    Exit Sub
                ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARYOFCONTENTS Then
                    DisplayBCScreen(BCSCREENS.BookInOrderSummaryOfContents)
                    Exit Sub

                ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINITEMFORNOORDERNUMBER Then

                    DisplayBCScreen(BCSCREENS.BookInItemforNoOrder)
                    Exit Sub
                ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONSCAN Then

                    DisplayBCScreen(BCSCREENS.CartonScan)
                    Exit Sub
                ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONEXPECTEDORDER Then

                    DisplayBCScreen(BCSCREENS.ExpectedOrder)
                    Exit Sub
                End If

            Else


                'if item scanned from carton scanning screen
                If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONSCAN Then
                    ' objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                    'Dont book in at item level
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M49"), "Alert ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    DisplayBCScreen(BCSCREENS.CartonScan)
                    Exit Sub
                    'if item scanned from carton scanning screen
                ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONEXPECTEDORDER Then
                    'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                    'Dont book in at item level
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M49"), "Alert ", MessageBoxButtons.OK, _
                                                                              MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    DisplayBCScreen(BCSCREENS.ExpectedOrder)
                    Exit Sub


                    'Item scan
                ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONITEMSCAN Or _
               objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARYOFCONTENTS Or _
               objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINITEMFORNOORDERNUMBER Then
                    Dim bCheck As Boolean
                    If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONITEMSCAN Then
#If RF Then
                        'RECONNECT to show message for Retry properly for each screen
                        objAppContainer.m_ModScreen = AppContainer.ModScreen.ITEMSCAN
#End If
                        'Getting the details of scanned item
                        bCheck = (objAppContainer.objDataEngine.GetItemDetails(strBarcode, AppContainer.ItemDetailType.Carton, m_BCItemInfo))
                    Else
                        bCheck = (objAppContainer.objDataEngine.GetItemDetails(strBarcode, AppContainer.ItemDetailType.OrderNo, m_BCItemInfo))



                    End If
                    If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARYOFCONTENTS Then
                        'Checking whether the scanned item belongs to the order
                        Dim bIsPresent As Boolean = False
                        Dim strBootsCode As String
                        Dim strTemItemCode As String = " "
                        If strBarcode.Length <= 7 Then
                            strTemItemCode = strBarcode
                        Else
                            strTemItemCode = objAppContainer.objDataEngine.GetBootsCode(strBarcode)
                        End If
                        'to check the if the item is present in the order if the item is in file
                        If bCheck Then

                            For Each objPresentItem As GIValueHolder.ItemListForOrder In m_ItemListForOrder
                                'checking if the entered product code or boots code matches the boots code of child items
                                If (objPresentItem.ProductCode = m_BCItemInfo.ProductCode) OrElse (objPresentItem.BootsCode = m_BCItemInfo.BootsCode) Then
                                    m_PageNo = objPresentItem.PageNo
                                    bIsPresent = True
                                    m_bInOrder = True
                                    Exit For
                                End If
                                'to check if the scanned barcode is a First barcode
                                If strBarcode.Substring(0, 6) = "000000" Then
                                    'Dim strBootsCode As String
                                    'if first barcode get Boots code from first Barcode
                                    strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode.Substring(6, 6))
                                    'checking if boots code from first barcode matches the boots code of child items
                                    If objPresentItem.BootsCode = m_BCItemInfo.BootsCode Then
                                        m_PageNo = objPresentItem.PageNo
                                        bIsPresent = True
                                        m_bInOrder = True
                                        Exit For
                                    End If
                                End If
                            Next


                            If Not bIsPresent Then
                                m_bInOrder = False
                                ' objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                                iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M106"), "Alert ", MessageBoxButtons.YesNo, _
                                                                                                  MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
                                If iResult = MsgBoxResult.No Then
                                    DisplayBCScreen(BCSCREENS.BookInOrderSummaryOfContents)
                                    Exit Sub
                                End If
                            End If
                        End If

                    End If


                    '                    Dim bCheck As Boolean
                    '                    If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONITEMSCAN Then
                    '#If RF Then
                    '                        'RECONNECT to show message for Retry properly for each screen
                    '                        objAppContainer.m_ModScreen = AppContainer.ModScreen.ITEMSCAN
                    '#End If
                    '                        'Getting the details of scanned item
                    '                        bCheck = (objAppContainer.objDataEngine.GetItemDetails(strBarcode, AppContainer.ItemDetailType.Carton, m_BCItemInfo))
                    '                    Else
                    '                        bCheck = (objAppContainer.objDataEngine.GetItemDetails(strBarcode, AppContainer.ItemDetailType.OrderNo, m_BCItemInfo))

                    'End If

                    'Item not in file--Corresponding item scan screens are displayed
                    If Not bCheck Then
                        ' objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        'RECONNECT- not to show the message if connection fails
#If RF Then
                        'Timeout 
                        If objAppContainer.bTimeOut Then
                            Exit Sub
                        End If
                        If Not objAppContainer.bCommFailure And Not objAppContainer.bReconnectSuccess Then
                            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M67"), "Alert ", MessageBoxButtons.OK, _
                                         MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        End If
#ElseIf NRF Then

                        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M67"), "Alert ", MessageBoxButtons.OK, _
                                         MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
#End If


                        If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONITEMSCAN Then

                            DisplayBCScreen(BCSCREENS.ItemScan)
                            Exit Sub
                        ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARYOFCONTENTS Then
                            DisplayBCScreen(BCSCREENS.BookInOrderSummaryOfContents)
                            Exit Sub
                        ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINITEMFORNOORDERNUMBER Then
                            DisplayBCScreen(BCSCREENS.BookInItemforNoOrder)
                            Exit Sub

                        End If


                    Else

                        'checks whether the item is previously scanned and if yes sets the item quantity
                        For Each objItem As GIValueHolder.ScanDetails In m_ItemList
                            If m_ItemOrder = BookInOrder.Order Or m_ItemOrder = BookInOrder.NoOrder Then
                                If objItem.ProductCode = m_BCItemInfo.BootsCode Or _
                                         objItem.BootCode = m_BCItemInfo.BootsCode Then
                                    m_BCItemInfo.ItemQty = CInt(objItem.ItemQty)
                                    m_strScanCheck = True
                                    Exit For
                                End If

                            ElseIf objItem.ProductCode = m_BCItemInfo.ProductCode Or _
                              objItem.BootCode = m_BCItemInfo.BootsCode Then
                                m_BCItemInfo.ItemQty = CInt(objItem.ItemQty)
                                m_strScanCheck = True
                                Exit For
                            End If
                        Next
                        'For setting the expected qty for the scanned item 
                        'in case of book in order of item with order number
                        'so that the qty can be compared 
                        If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARYOFCONTENTS Then
                            m_iOrderExpectedQty = 0
                            For Each objItem As Item In m_ItemViewList
                                If objItem.strItemNo = m_BCItemInfo.BootsCode Then
                                    m_iOrderExpectedQty = objItem.iExpected
                                    Exit For
                                End If
                            Next
                        End If
                        'Start : 1.1    Archana Chandramathi  ISO 008-009 Changes
                        'Not to display the Qty entry screen in case of .com orders
                        If (m_bIsDotcomItem = False) Then
                            DisplayBCScreen(BCSCREENS.ItemInfo)
                        End If
                        'End : 1.1    Archana Chandramathi  ISO 008-009 Changes
                        Exit Sub

                    End If

                End If
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at BookIn carton Handle Item Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try

    End Sub
    ''' <summary>
    ''' Handles the Carton scan
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function HandleCarton(ByVal strBarcode As String) As Boolean
        Dim iResult As Integer = 0
        Dim bValidate As Boolean = True
        Dim iMsg As Integer = 0
        Dim sSupplierNum As String = Nothing
        Try
            'Validates Carton code
            'If carton code is invalid
            If Not objAppContainer.objHelper.ValidateASNBarcode(strBarcode, m_objASNCode) Then
                Return False
            End If

#If RF Then
            'O&C display message if Boots.com parcel is scanned.
            'This is an Order & Collect Parcel. Please use the Order & Collect menu to book in this parcel.
            If m_objASNCode.SupplierNumber = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DOTCOM_SUPPLIER_NUM) Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M138"), "Warning", MessageBoxButtons.OK, _
                                                                   MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                Return True
            End If
#End If

            If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONITEMSCAN Then
#If RF Then
                'RECONNECT to check the activity of current screen for reconnect logic
                objAppContainer.m_ModScreen = AppContainer.ModScreen.ITEMSCAN
#End If
                'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                'Cartons should not be scanned from item scanning screen
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                                                                                     MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                DisplayBCScreen(BCSCREENS.ItemScan)

                Return True

            ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARYOFCONTENTS Then
                ' objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                'Cartons should not be scanned from item scanning screen
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                                                                                    MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                DisplayBCScreen(BCSCREENS.BookInOrderSummaryOfContents)

                Return True

            ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINITEMFORNOORDERNUMBER Then
                'Cartons should not be scanned from item scanning screen
                ' objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), " Error", MessageBoxButtons.OK, _
                                                                                                    MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                DisplayBCScreen(BCSCREENS.BookInItemforNoOrder)

                Return True


            ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONEXPECTEDORDER Then
                Dim bTemp As Boolean = False
                Dim objSupData As New GIValueHolder.SupplierList
                '  m_SupplierName = m_CartonInfo.SupplierName
                For Each objSupplier As GIValueHolder.SupplierList In m_SupplierList
                    If objSupplier.SupplierNo = m_objASNCode.SupplierNumber Then
                        m_SupplierName = objSupplier.SupplierName
                        m_Outstanding = objSupplier.SupplierQty.ToString()
                        m_SupplierNo = objSupplier.SupplierNo
                        bTemp = True
                        Exit For
                    End If
                Next
                If Not bTemp Then
                    If objAppContainer.objDataEngine.GetSupplierData(m_objASNCode.SupplierNumber, _
                                                                     objSupData, AppContainer.DeliveryType.ASN, _
                                                                     AppContainer.FunctionType.BookIn) Then
                        m_SupplierName = objSupData.SupplierName
                        m_Outstanding = Quantity.Zero
                        m_SupplierNo = m_objASNCode.SupplierNumber
                    Else
                        m_SupplierName = "Unknown"
                        m_Outstanding = Quantity.Zero
                        m_SupplierNo = m_objASNCode.SupplierNumber
                    End If
                End If


            ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONSCAN Then
#If RF Then
                'RECONNECT to check the activity of current screen for reconnect logic
                objAppContainer.m_ModScreen = AppContainer.ModScreen.PREFINISH
#End If
                If Not m_objASNCode.SupplierNumber = m_SupplierNo Then
                    'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                    'different supplier
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M51"), "Alert ", MessageBoxButtons.OK, _
                                                                                                  MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    bValidate = False

                    DisplayBCScreen(BCSCREENS.CartonScan)
                    Return True

                End If

            End If



            'Carton details are fetched 


            If Not objAppContainer.objDataEngine.ValidateCartonScanned(strBarcode, m_CartonInfo, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.BookIn) Then

                '-----------Not in file---------------------------
                bValidate = False
#If RF Then
                'time out 
                If objAppContainer.bTimeOut Then
                    'on time out it should not enter HandleItem, if return is false
                    Return True
                End If
                If Not objAppContainer.bCommFailure AndAlso Not objAppContainer.bReconnectSuccess Then
#End If
                ' objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                'Start : 1.1    Archana Chandramathi  ISO 008-009 Changes
                'Take the first 6 character for supplier number
                sSupplierNum = strBarcode.Substring(0, 6)
                'When the carton is not on file check the supplier number is same as .com or not
                If sSupplierNum = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DOTCOM_SUPPLIER_NUM).ToString() Then
                    'objAppContainer.bIsDotComOrder = True
                    m_bIsDotcomItem = True
                End If
                'Check whether its .com order
                If m_bIsDotcomItem = True Then
                    m_bIsDotcomItem = False
                    'Display the message "Is it for your store?"
                    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M130"), "Carton Not On File ", MessageBoxButtons.YesNo, _
                                 MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    'When the user press Yes
                    If iResult = MsgBoxResult.Yes Then
                        'Display the message "Book in the parcel using the exceptions process"
                        iMsg = MessageBox.Show(MessageManager.GetInstance().GetMessage("M131"), " ", MessageBoxButtons.OK, _
                                                                          MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
                        'When click on OK button display the item entry screen with dummy barcode defaulted
                        If iMsg = MsgBoxResult.Ok Then
                            m_bIsDotcomItem = True
                        End If
                    Else
                        'Display the message "Book in the parcel using the exceptions process. Send the parcel back following the mis-direct process"
                        iMsg = MessageBox.Show(MessageManager.GetInstance().GetMessage("M132"), " ", MessageBoxButtons.OK, _
                                                                                                     MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
                        'When click on OK button display the item entry screen with dummy barcode defaulted
                        If iMsg = MsgBoxResult.Ok Then
                            m_bIsDotcomItem = True
                        End If
                    End If
                    'If the Item is Boots.com/ie save the details and display quantity entry screen
                    If m_bIsDotcomItem = True Then
                        'Saving the Not in file cartoncode to add along with the item details to send as export data
                        objAppContainer.strNIFCartonCode = strBarcode
                        'Set the active screen carton otem scan
                        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONITEMSCAN
                        'Default the dummy barcode and process it
                        HandleScanData(ConfigDataMgr.GetInstance.GetParam(ConfigKey.DUMMY_DOTCOM_ITEM).ToString(), BCType.ManualEntry)
                        'Set the quantity to 1 for boots.com/ie items
                        SetItemQty(Macros.DOTCOM_ITEM_QTY)
                        Return True
                    End If
                    'End : 1.1    Archana Chandramathi  ISO 008-009 Changes
                Else
                    'Is this the first attempt
                    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M52"), "Carton Not On File ", MessageBoxButtons.YesNo, _
                                 MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    If iResult = MsgBoxResult.Yes Then
                        'Put carton to one side and try again tomorrow
                        iMsg = MessageBox.Show(MessageManager.GetInstance().GetMessage("M45"), " ", MessageBoxButtons.OK, _
                                               MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
                        If iMsg = MsgBoxResult.Ok Then
                            If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONEXPECTEDORDER Then
                                DisplayBCScreen(BCSCREENS.ExpectedOrder)
                                Return True
                            ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONSCAN Then
                                DisplayBCScreen(BCSCREENS.CartonScan)
                                Return True
                            End If
                        End If
                        'If not the first attempt
                    ElseIf iResult = MsgBoxResult.No Then
                        iMsg = MessageBox.Show(MessageManager.GetInstance().GetMessage("M53"), " ", MessageBoxButtons.OK, _
                                                                           MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
                        If iMsg = MsgBoxResult.Ok Then
                            'Saving the Not in file cartoncode to add along with the item details to send as export data
                            objAppContainer.strNIFCartonCode = strBarcode
                            'Book In at item level
                            DisplayBCScreen(BCSCREENS.ItemScan)
                            Return True
                        End If
                    End If
                End If
#If RF Then
                    End If
                    objAppContainer.bReconnectSuccess = False
#End If

                '--------------------end of not in file---------------------------------


            ElseIf bValidate = True Then

                If Not m_CartonInfo.Status = Status.UnBooked Then
                    bValidate = False
                    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M47"), "Alert", MessageBoxButtons.OK, _
                                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    If iResult = MsgBoxResult.Ok Then
                        If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONSCAN Then
                            DisplayBCScreen(BCSCREENS.CartonScan)
                        End If
                        Return True
                    End If
                End If
                Dim bIsPresentInList As Boolean = False
                For Each objCartonPreviousItem As GIValueHolder.ScanDetails In m_CartonList
                    If objCartonPreviousItem.ScannedCode = m_CartonInfo.ASNNumber Then
                        bIsPresentInList = True
                        Exit For
                    End If
                Next
                If Not bIsPresentInList Then

                    Dim objCartonCurrentItem As New GIValueHolder.ScanDetails
                    objCartonCurrentItem.ScannedCode = strBarcode
                    objCartonCurrentItem.ScanDate = Format(DateTime.Now, "yyyyMMdd")
                    objCartonCurrentItem.ScanLevel = ScanLevel.Delivery
                    objCartonCurrentItem.ScanTime = Format(DateTime.Now, "HHmmss")
                    objCartonCurrentItem.ScanType = ScanType.Booked
                    m_CartonList.Add(objCartonCurrentItem)
                    m_CartonCount = m_CartonCount + 1

                    If Not CInt(m_Outstanding) = 0 Then
                        m_Outstanding = (CInt(m_Outstanding) - 1).ToString()
                    End If
                    DisplayBCScreen(BCSCREENS.CartonScan)
                Else
                    'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                    'already scanned
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M50"), "Alert ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    DisplayBCScreen(BCSCREENS.CartonScan)
                End If

            End If

            Return True
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at BookIn carton Handle carton Session: " + ex.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try

    End Function
    ''' <summary>
    ''' The Method handles the scan data returned form the barcode scanner.
    ''' This method implements the business logic to populate the data to the corresponding
    ''' UI element after validation.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="Type"></param>
    ''' <remarks></remarks>
    Public Sub HandleScanData(ByVal strBarcode As String, ByVal Type As BCType)
#If RF Then
        If objAppContainer.bReconnectSuccess Then
            objAppContainer.bReconnectSuccess = False
        End If
#End If
        'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
        Select Case Type
            Case BCType.CODE128
                HandleCarton(strBarcode)
            Case BCType.EAN
                If Not HandleCarton(strBarcode) Then
                    HandleItem(strBarcode)
                End If
            Case BCType.ManualEntry
                If Not HandleCarton(strBarcode) Then
                    HandleItem(strBarcode)
                End If
            Case BCType.UPC
                HandleItem(strBarcode)
            Case Else
                ' objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                                                                                MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                Exit Sub
        End Select
        '  objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
    End Sub
    ''' <summary>
    ''' This is called when a supplier is selected from the listview
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SupplierSelection()

        m_SupplierName = Nothing
        Dim iCounter As Integer = 0
        Try
            Dim arrTempSupplierList As New ArrayList
            For iCounter = 0 To m_BookInCartonExpectedOrder.lvwSuppliers.Items.Count - 1
                'The details of the selected supplier is stored in the corresponding variables
                If m_BookInCartonExpectedOrder.lvwSuppliers.Items(iCounter).Selected Then
                    m_SupplierName = m_BookInCartonExpectedOrder.lvwSuppliers.Items(iCounter).Text
                    m_Outstanding = m_BookInCartonExpectedOrder.lvwSuppliers.Items(iCounter).SubItems(1).Text

                    For Each objSupplier As GIValueHolder.SupplierList In m_SupplierList
                        If objSupplier.SupplierName = m_SupplierName Then

                            arrTempSupplierList.Add(objSupplier)
                        End If
                    Next
                    If arrTempSupplierList.Count > 0 Then
                        For Each objSupplierData As GIValueHolder.SupplierList In arrTempSupplierList
                            m_SupplierNo = objSupplierData.SupplierNo.ToString()
                            m_SupplierType = objSupplierData.SupplierType.ToString()
                            m_strSupplierSelectionCheck = "Y"
                        Next
                    End If
                    Exit For
                End If
            Next

            'For ASN suppliers
            If m_SupplierType = SupplierType.ASN Then
                DisplayBCScreen(BCSCREENS.CartonScan)

                'For Non ASN suppliers
            ElseIf m_SupplierType = SupplierType.Directs Then
                DisplayBCScreen(BCSCREENS.BookInOrderInitial)

            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at BookIn carton supplier selection Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try

    End Sub
    Public Sub SetQuantity(ByVal strQty As String)
        m_Qty = strQty
        DisplayBCScreen(BCSCREENS.ItemInfo)
        m_Qty = Nothing
    End Sub

    ''' <summary>
    ''' This is called when an order is selected from the listview
    ''' </summary>
    ''' <remarks></remarks>
    Public Function OrderSelection()
        m_OrderNo = Nothing
        Dim bTemp As Boolean = False
        Try
            Dim objTempItem As Item
            Dim iCounter As Integer = 0
            For iCounter = 0 To m_BookInOrderInitial.lvwOrders.Items.Count - 1
                If m_BookInOrderInitial.lvwOrders.Items(iCounter).Selected Then
                    'The selected order number is stored to a variable
                    m_OrderNo = m_BookInOrderInitial.lvwOrders.Items(iCounter).Text
                    bTemp = True
                    Exit For
                End If
            Next
            For Each objOrder As GIValueHolder.OrderList In m_OrderList
                If objOrder.OrderNo = m_OrderNo Then
                    m_OrderNumber = objOrder.Code.PadLeft(20, "0")
                    'The item list for the selected order is fetched
                    objAppContainer.objDataEngine.GetItemListForOrder(objOrder, m_ItemListForOrder)
#If RF Then
                    If Not objAppContainer.bCommFailure And Not objAppContainer.bReconnectSuccess Then
#End If
                    For Each objItemDetails As GIValueHolder.ItemListForOrder In m_ItemListForOrder
                        'The fetched item details are stored to the object ot Item() 
                        'and the objects are added to the arraylist
                        objTempItem = New Item
                        objTempItem.strItemNo = objItemDetails.BootsCode
                        objTempItem.strDesc = objItemDetails.ItemDesc
                        objTempItem.iExpected = CInt(objItemDetails.ExptdQty)
                        objTempItem.iRecd = 0
                        m_ItemViewList.Add(objTempItem)
                    Next
                    DisplayBCScreen(BCSCREENS.BookInOrderSummaryOfContents)
                    Exit For
                End If
#If RF Then
                    'Else
                    Exit For
                End If
#End If
            Next
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at BookIn carton order selection Session: " + ex.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' This is called when the supplier number is entered manually
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SupplierNoEntry(ByVal strSupplierNo As String)
        Dim bTemp As Boolean = False
        Try
            'The entered supplier number is checked and its details are stored in the variables
            Dim objSupplierdata As New GIValueHolder.SupplierList
            Dim arrSupplierNoEntryList As New ArrayList

            For Each objSupplier As GIValueHolder.SupplierList In m_SupplierList
                If objSupplier.SupplierNo = strSupplierNo Then
                    m_SupplierNo = objSupplier.SupplierNo
                    m_Outstanding = objSupplier.SupplierQty
                    m_SupplierName = objSupplier.SupplierName
                    m_SupplierType = objSupplier.SupplierType
                    arrSupplierNoEntryList.Add(objSupplier)
                    bTemp = True
                    Exit For
                End If
            Next

            If Not bTemp Then

                If objAppContainer.objDataEngine.GetSupplierData(strSupplierNo, objSupplierdata, AppContainer.DeliveryType.Directs, AppContainer.FunctionType.BookIn) Then
                    m_SupplierNo = strSupplierNo
                    m_Outstanding = Quantity.Zero
                    m_SupplierName = objSupplierdata.SupplierName
                    m_SupplierType = objSupplierdata.SupplierType

                Else

                    'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                    'If invalid supplier number is entered
#If RF Then

                    If Not objAppContainer.bCommFailure And Not objAppContainer.bReconnectSuccess Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M80"), "Alert ", MessageBoxButtons.OK, _
                                                                                                   MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        DisplayBCScreen(BCSCREENS.ExpectedOrder)
                    ElseIf objAppContainer.bCommFailure And Not objAppContainer.bReconnectSuccess Then
                        objAppContainer.objGoodsInMenu.Visible = True
                    ElseIf objAppContainer.bReconnectSuccess Then
                        DisplayBCScreen(BCSCREENS.ExpectedOrder)
                    End If
#ElseIf NRF Then
                    DisplayBCScreen(BCSCREENS.ExpectedOrder)
#End If
                    Exit Sub
                End If
            End If


#If RF Then 'O&C CSK 4/8/2016
            'O&C display message if Boots.com parcel is scanned.
            'This is an Order & Collect Parcel. Please use the Order & Collect menu to book in this parcel.
            If strSupplierNo = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DOTCOM_SUPPLIER_NUM) Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M138"), "Warning", _
                               MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)

                DisplayBCScreen(BCSCREENS.ExpectedOrder)
            Else

#End If
                'For ASN suppliers
            If m_SupplierType = SupplierType.ASN Then
                DisplayBCScreen(BCSCREENS.CartonScan)

                'For Non ASN suppliers
            ElseIf m_SupplierType = SupplierType.Directs Then
                DisplayBCScreen(BCSCREENS.BookInOrderInitial)

            End If

#If RF Then 'O&C CSK 4/8/2016
            End If
#End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occurred at BookIn carton supplier Number entry Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    ''' <summary>
    ''' To quit the session without saving anything and GoodsIn Main menu is displayed
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub QuitSession()
        Dim iResult As Integer = 0
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M84"), "Confirmation", MessageBoxButtons.YesNo, _
                                  MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        If iResult = MsgBoxResult.Yes Then
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            m_BCSessionMgr.EndSession(AppContainer.IsAbort.Yes)
        End If

    End Sub
    ''' <summary>
    ''' To quit the session session without saving anything and the summary screen is displayed
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub QuitBeforeCommit()
        Dim iResult As Integer = 0
        'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M48"), "Confirmation", MessageBoxButtons.YesNo, _
                                  MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        If iResult = MsgBoxResult.Yes Then
            If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARYOFCONTENTS Or _
            objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINITEMFORNOORDERNUMBER Or _
                          m_ItemOrder = BookInOrder.Order Or m_ItemOrder = BookInOrder.NoOrder Then
                DisplayBCScreen(BCSCREENS.BookInOrderSummary)
            Else
                m_strBookInCaronFinish = False
                DisplayBCScreen(BCSCREENS.Summary)
            End If
        End If
    End Sub
    ''' <summary>
    ''' Finishing the session
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub FinishSession()
        Dim iResult As Integer = 0
        Try
            ' objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M86"), "Alert", MessageBoxButtons.OKCancel, _
                                      MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            If iResult = MsgBoxResult.Ok Then

                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONITEMSCAN Or _
                     objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONSCAN Then
#If RF Then
                    'RECONNECT to check the activity of current screen for reconnect logic
                    objAppContainer.m_ModScreen = AppContainer.ModScreen.POSTFINISH
#End If

                    m_strBookInCaronFinish = True
                    m_strBookIncartonShowMsg = "Y"
                    If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONITEMSCAN Then
                        CartonCountIncrement()
                    End If
                    'sending the carton details as export data
                    If m_CartonList.Count > 0 Then
                        If objAppContainer.objDataEngine.SendCartonDetails(m_CartonList, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.BookIn) Then
                            'Clear the items after it is sent
                            m_CartonList.Clear()
                            DisplayBCScreen(BCSCREENS.Summary)
                        End If
                    Else
                        DisplayBCScreen(BCSCREENS.Summary)
                    End If
                    ''sending the item qty as export data
                    'If m_CartonNIFItemCount > 0 Then
                    '    objAppContainer.objDataEngine.SendItemQuantity(m_ItemList.Count, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.BookIn)
                    'End If

                ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINITEMFORNOORDERNUMBER Or _
                objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARYOFCONTENTS Then
                    'sending the item details as export data
                    If m_ItemList.Count > 0 Then
#If RF Then
                        Dim iItemCount As Integer = m_ItemList.Count
                        If objAppContainer.objDataEngine.SendItemDetails(m_ItemList, AppContainer.DeliveryType.Directs, AppContainer.FunctionType.BookIn) Then
                            m_ItemList.Clear()
                            objAppContainer.objDataEngine.SendItemQuantity(iItemCount, AppContainer.DeliveryType.Directs, AppContainer.FunctionType.BookIn)
                        End If
#ElseIf NRF Then
                        objAppContainer.objDataEngine.SendItemDetails(m_ItemList, AppContainer.DeliveryType.Directs, AppContainer.FunctionType.BookIn)
                        objAppContainer.objDataEngine.SendItemQuantity(m_ItemList.Count, AppContainer.DeliveryType.Directs, AppContainer.FunctionType.BookIn)
#End If

                    End If
#If RF Then
                    If Not objAppContainer.bCommFailure AndAlso Not objAppContainer.bReconnectSuccess Then
                        DisplayBCScreen(BCSCREENS.FinalBookInItemSummary)
                    End If
                    objAppContainer.bReconnectSuccess = False
#ElseIf NRF Then
                    DisplayBCScreen(BCSCREENS.FinalBookInItemSummary)
#End If

                End If

            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at BookIn carton finish Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try


    End Sub
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by BCSessionMgr
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EndSession(ByVal isAbort As AppContainer.IsAbort)
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)

#If RF Then
            'objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
            'RF RECONNECT
           ' If Not objAppContainer.bCommFailure Then
                'sending GIX for export Data writing
                If objAppContainer.objDataEngine.SendSessionExit(m_tyDeliveryType, AppContainer.FunctionType.Audit, isAbort) Then


#ElseIf NRF Then
            'sending GIX for export Data writing
            objAppContainer.objDataEngine.SendSessionExit(m_tyDeliveryType, AppContainer.FunctionType.Audit, isAbort)

#End If
            m_BookInCartonExpectedOrder.Close()
            m_BookInCartonExpectedOrder.Dispose()
            m_BookInCartonScanCarton.Close()
            m_BookInCartonScanCarton.Dispose()
            m_BookInCartonSummary.Close()
            m_BookInCartonSummary.Dispose()
            m_BookInCartonScanItem.Close()
            m_BookInCartonScanItem.Dispose()
            m_BookInCartonItemInfo.Close()
            m_BookInCartonItemInfo.Dispose()
            m_BookInOrderInitial.Close()
            m_BookInOrderSummaryOfContents.Close()
            m_BookInOrderSummary.Close()
            m_BookInOrderInitial.Dispose()
            m_BookInOrderSummaryOfContents.Dispose()
            m_BookInOrderSummary.Dispose()
            m_BookInItemForNoOrderNumber.Close()
            m_BookInItemForNoOrderNumber.Dispose()
            m_FinalBookInItemSummary.Close()
            m_FinalBookInItemSummary.Dispose()

            m_strScanCheck = False
            m_ItemViewList = Nothing
            m_SupplierList = Nothing
            m_CartonList = Nothing
            m_CartonInfo = Nothing
            m_OrderList = Nothing
            m_ItemListForOrder = Nothing
            m_ItemListForOrderDetails = Nothing
            m_BCItemInfo = Nothing
            m_CartonDetails = Nothing
            m_objASNCode = Nothing
            m_SupplierNo = Nothing
            m_objItem = Nothing
            m_Qty = Nothing
            'Fix for Book In Order not opening after auditting a carton not on file
            objAppContainer.strAuditCartonNotinFile = Nothing
#If RF Then
                objAppContainer.objGoodsInMenu.Visible = True
                objAppContainer.bReconnectSuccess = False
                objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
            End If
            'End If
#End If


            'CR for Forced Log off
#If NRF Then
            If objAppContainer.objPrevMod <> AppContainer.ACTIVEMODULE.AUDITCARTON Then
                If isAbort = AppContainer.IsAbort.No Then
                    objAppContainer.bForceLogOff = True
                    objAppContainer.ForcedLogOff()
                End If
            End If

#End If


            'End fix for Book In Order not opening after auditting a carton not on file
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at BookIn carton End Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try

    End Sub
#If RF Then
    Public Sub DisposeBookIn()
        m_BookInCartonExpectedOrder.Close()
        m_BookInCartonExpectedOrder.Dispose()
        m_BookInCartonScanCarton.Close()
        m_BookInCartonScanCarton.Dispose()
        m_BookInCartonSummary.Close()
        m_BookInCartonSummary.Dispose()
        m_BookInCartonScanItem.Close()
        m_BookInCartonScanItem.Dispose()
        m_BookInCartonItemInfo.Close()
        m_BookInCartonItemInfo.Dispose()
        m_BookInOrderInitial.Close()
        m_BookInOrderSummaryOfContents.Close()
        m_BookInOrderSummary.Close()
        m_BookInOrderInitial.Dispose()
        m_BookInOrderSummaryOfContents.Dispose()
        m_BookInOrderSummary.Dispose()
        m_BookInItemForNoOrderNumber.Close()
        m_BookInItemForNoOrderNumber.Dispose()
        m_FinalBookInItemSummary.Close()
        m_FinalBookInItemSummary.Dispose()

        m_strScanCheck = False
        m_ItemViewList = Nothing
        m_SupplierList = Nothing
        m_CartonList = Nothing
        m_CartonInfo = Nothing
        m_OrderList = Nothing
        m_ItemListForOrder = Nothing
        m_ItemListForOrderDetails = Nothing
        m_BCItemInfo = Nothing
        m_CartonDetails = Nothing
        m_objASNCode = Nothing
        m_SupplierNo = Nothing
        m_objItem = Nothing
        m_Qty = Nothing
        'Fix for Book In Order not opening after auditting a carton not on file
        objAppContainer.strAuditCartonNotinFile = Nothing
        objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
    End Sub

#End If

    Public Sub New()

    End Sub
End Class
''' <summary>
''' Screens in BookInOrder
''' </summary>
''' <remarks></remarks>
Public Enum BCSCREENS
    ExpectedOrder
    CartonScan
    Summary
    ItemScan
    ItemInfo
    BookInOrderInitial
    BookInOrderSummaryOfContents
    BookInOrderSummary
    BookInItemforNoOrder
    FinalBookInItemSummary
End Enum
''' <summary>
''' Class to store the details of items in an order
''' Its objects are used to populate the listview
''' </summary>
''' <remarks></remarks>
Public Class Item
    Public strItemNo As String
    Public strDesc As String
    Public iExpected As Integer
    Public iRecd As Integer
End Class
Public Class GenericComparer
    Implements IComparer
    Public Function Compare(ByVal objx As Object, ByVal objy As Object) As Integer Implements IComparer.Compare
        'Sort the arraylist based on Item code
        Dim objItemX As Item = DirectCast(objx, Item)
        Dim objItemY As Item = DirectCast(objy, Item)
        Return String.Compare(objItemX.strDesc, objItemY.strDesc)
    End Function
End Class
Public Class OrderComparer
    Implements IComparer
    Public Function Compare(ByVal objx As Object, ByVal objy As Object) As Integer Implements IComparer.Compare
        'Sort the arraylist based on order number
        Dim objItemX As GIValueHolder.OrderList = DirectCast(objx, GIValueHolder.OrderList)
        Dim objItemY As GIValueHolder.OrderList = DirectCast(objy, GIValueHolder.OrderList)
        Return String.Compare(objItemX.OrderNo, objItemY.OrderNo)
    End Function
End Class
Public Class SupplierComparer
    Implements IComparer
    Public Function Compare(ByVal objx As Object, ByVal objy As Object) As Integer Implements IComparer.Compare
        'Sort the arraylist based on Supplier name
        If TypeOf objx Is GIValueHolder.SupplierList Then
            Dim objItemX As GIValueHolder.SupplierList = DirectCast(objx, GIValueHolder.SupplierList)
            Dim objItemY As GIValueHolder.SupplierList = DirectCast(objy, GIValueHolder.SupplierList)
            Return String.Compare(objItemX.SupplierName, objItemY.SupplierName)
            'Sort the arraylist based on Expected Date
        ElseIf TypeOf objx Is GIValueHolder.SupplierDetails Then
            Dim objItemX As GIValueHolder.SupplierDetails = DirectCast(objx, GIValueHolder.SupplierDetails)
            Dim objItemY As GIValueHolder.SupplierDetails = DirectCast(objy, GIValueHolder.SupplierDetails)
            Dim iResult As Int16 = Date.Compare(DateTime.ParseExact(objItemX.ExptDate, "yyyyMMdd", CultureInfo.InvariantCulture), DateTime.ParseExact(objItemY.ExptDate, "yyyyMMdd", CultureInfo.InvariantCulture))
            If iResult = 0 Then
                Int64.Parse(objItemX.CartonNumber).CompareTo(Int64.Parse(objItemY.CartonNumber))
            Else
                Return iResult
            End If
            ' Return Date.Compare(DateTime.ParseExact(objItemX.ExptDate, "yyyyMMdd", CultureInfo.InvariantCulture), DateTime.ParseExact(objItemY.ExptDate, "yyyyMMdd", CultureInfo.InvariantCulture))
            'Sort the arraylist based on Item Description
        End If
    End Function
End Class
