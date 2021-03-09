Imports System.Globalization
'''***************************************************************
''' <FileName>VCSessionManager.vb</FileName>
''' <summary>
''' The View Carton Container Class.
''' Implements all business logic and GUI navigation for View Carton.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>08-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 1.1 for PPC</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''******************************************************************************
''' Modification Log 
'''******************************************************************************* 
''' No:      Author            Date            Description 
''' 1.1     Charles Skadorwa   04/08/2016   Modified as part of Order & Collect project.
'''           (CS)                          Suppress Boots.com supplier from displaying  
'''                                         - see function HandleData(). 
'''********************************************************************************
'''***************************************************************
Public Class VCSessionManager
    Private Shared m_VCSessionManager As VCSessionManager
    Private m_viewCarton As frmViewCarton
    Private m_ViewItems As frmViewItems
    Private objASNCode As GIValueHolder.ASNCode
    Private m_ViewSuppliers As frmViewSuppliers

    Public m_SupplierList As ArrayList
    Public m_CartonList As ArrayList
    Public m_ItemList As ArrayList
    Public m_VCartonInfo As GIValueHolder.CartonInfo
    Private strSupplierName As String = ""
    Public strCartonId As String = ""
    Public bScanned As Boolean = False
    Public strASNCode As String = ""
    Public bReturn As Boolean = False
    Private strSupplierNo As String = ""
    Private strExptDate As String = ""
    Private strBkdInStatus As String = ""
    Private strNoOfCartonsinASN As String = "0000"
    ''' <summary>
    ''' Initialises the View Carton session
    ''' </summary>
    '''<remarks></remarks>
    Public Sub StartSession()
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
            m_viewCarton = New frmViewCarton
            m_ViewItems = New frmViewItems
            m_ViewSuppliers = New frmViewSuppliers
            m_SupplierList = New ArrayList
            m_CartonList = New ArrayList
            m_ItemList = New ArrayList
            m_VCartonInfo = New GIValueHolder.CartonInfo
            'Set the column width for each columns.
            'objAppContainer.objHelper.SetColumnWidth(m_viewCarton.lvwCartonList)
            Me.DisplayViewCarton(VCARTONSCREENS.ViewSuppliers)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("View Carton session cannot be started", Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Functions for getting the object instance for the VCSessionMgr. 
    ''' Use this method to get the object reference for the  VCSessionMgr.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of VCSessionMgr
    '''  Class</remarks>
    Public Function EndSession() As Boolean
        Try
#If NRF Then
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
            objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.ASN, AppContainer.FunctionType.View, AppContainer.IsAbort.No)
#ElseIf RF Then
            'RF RECONNECT
            If Not objAppContainer.bCommFailure Then
                objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.ASN, AppContainer.FunctionType.View, AppContainer.IsAbort.No)
            End If
#End If

            m_viewCarton.Close()
            m_ViewItems.Close()
            m_ViewSuppliers.Close()
            m_viewCarton.Dispose()
            m_ViewItems.Dispose()
            m_ViewSuppliers.Dispose()
            m_SupplierList = Nothing
            m_CartonList = Nothing
            m_ItemList = Nothing
            m_VCartonInfo = Nothing
            objASNCode = Nothing
            'm_VCSessionManager = Nothing

#If RF Then
            objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
#End If
#If NRF Then
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
#End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("VC Session Manager EndSession failure", Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    Public Sub DisposeViewCarton()
        m_viewCarton.Close()
        m_ViewItems.Close()
        m_ViewSuppliers.Close()
        m_viewCarton.Dispose()
        m_ViewItems.Dispose()
        m_ViewSuppliers.Dispose()
        m_SupplierList = Nothing
        m_CartonList = Nothing
        m_ItemList = Nothing
        m_VCartonInfo = Nothing
        objASNCode = Nothing
    End Sub
    ''' <summary>
    ''' Function for getting the object instance for the VCSessionManager. 
    ''' Use this method to get the object reference for the  VCSessionManager.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of VCSessionManager Class</remarks>
    Public Shared Function GetInstance() As VCSessionManager
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.VCARTON
        If m_VCSessionManager Is Nothing Then
            m_VCSessionManager = New VCSessionManager
            Return m_VCSessionManager
        Else
            Return m_VCSessionManager
        End If
    End Function
    ''' <summary>
    ''' To retrieve the Item details based on the Carton selected
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Sub DisplayItems(ByVal o As Object, ByVal e As EventArgs)
        Try
#If RF Then
  objAppContainer.objStatusBar.SetMessage("")
#ElseIf NRF Then
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
#End If

            Dim objItemComparer As New GenericComparer
            With m_ViewItems
                .lvwItemDetails.BeginUpdate()
                .lvwItemDetails.Items.Clear()
                'Sorting the itemlist based on the Item description
                m_ItemList.Sort(0, m_ItemList.Count, objItemComparer)
                'Adding items  to the Item List view
                For Each objItem As GIValueHolder.ItemDetails In m_ItemList
                    Dim objItemListView As ListViewItem = New ListViewItem
                    objItemListView = .lvwItemDetails.Items.Add(New ListViewItem(objItem.ItemCode))
                    objItemListView.SubItems.Add(objItem.ItemDesc)
                    objItemListView.SubItems.Add(objItem.ItemQty)
                Next
                'invoking the GetSupplierInfo method to retrieve the supplier name
#If NRF Then
                GetSupplierInfo()
#End If

                'Displaying labels of the View Item details screen
                .lblSupplierName.Text = strSupplierName.ToString()
                .lblCartonIdValue.Text = strCartonId.ToString
                .lblExptDateValue.Text = strExptDate.ToString
                If (strBkdInStatus = Macros.N) Then
                    .lblBkdInStatusValue.Text = "Unbooked"
                ElseIf strBkdInStatus = Macros.AUDITED Then
                    .lblBkdInStatusValue.Text = "Audited"
                Else
                    .lblBkdInStatusValue.Text = "Booked"
                End If
                m_ItemList.Clear()
                .lvwItemDetails.EndUpdate()
                objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINVIEWITEMS
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at view carton Displayitems Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To Display the Supplier List screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplaySuppliers(ByVal o As Object, ByVal e As EventArgs)
        'Invoke the PopulateSupplierList function
        VCSessionManager.GetInstance.PopulateSupplierList()
        objAppContainer.objStatusBar.SetMessage("")
#If RF Then
        If Not objAppContainer.bCommFailure AndAlso (Not objAppContainer.bReconnectSuccess) Then
#End If
            With m_ViewSuppliers
                objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINVIEWSUPPLIERS
#If NRF Then
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
#ElseIf RF Then
                objAppContainer.objStatusBar.SetMessage("")
#End If
                .Visible = True
                .Refresh()
            End With

#If RF Then
        End If
#End If
    End Sub
    ''' <summary>
    ''' To display the Carton of each supplier
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayCarton(ByVal o As Object, ByVal e As EventArgs)
        'Display the View Carton screen if the GetCartonList returns true else Display the view Suppliers screen
        'GetCartonList function is invoked to get the Carton list of a supplier
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        If VCSessionManager.GetInstance.GetCartonList() Then
            objAppContainer.objStatusBar.SetMessage("")
            With m_viewCarton
                objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINVIEWCARTON
                .Visible = True
                .Refresh()
            End With
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        End If
    End Sub
    Public Enum VCARTONSCREENS
        ViewCarton
        ViewItems
        ViewSuppliers
    End Enum
    ''' <summary>
    ''' Screen Display method for View Carton
    ''' All View Carton sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName">Enum VCARTONSCREENS</param>
    ''' <returns>True if display is success else False</returns>
    ''' <remarks></remarks>
    Public Function DisplayViewCarton(ByVal ScreenName As VCARTONSCREENS) As Boolean
        Try
            Select Case ScreenName
                Case VCARTONSCREENS.ViewSuppliers
                    m_viewCarton.Invoke(New EventHandler(AddressOf DisplaySuppliers))
                Case VCARTONSCREENS.ViewCarton
                    m_viewCarton.Invoke(New EventHandler(AddressOf DisplayCarton))
                Case VCARTONSCREENS.ViewItems
                    m_ViewItems.Invoke(New EventHandler(AddressOf DisplayItems))
            End Select
        Catch ex As Exception

            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at view carton Display Session: " + ex.ToString(), Logger.LogLevel.RELEASE)

            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' The Method handles scan the scan data returned form the barcode scanner
    ''' This method implements the business logic to populate the data to the corresponding
    ''' UI element after validation.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="Type"></param>
    ''' <remarks></remarks
    Public Sub HandleScanData(ByVal strCartonCode As String, ByVal Type As BCType)
        Dim iResult As Integer = 0
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
        Try
        Select Case Type
            'To get the Carton code entered manually
        Case BCType.ManualEntry
                HandleData(strCartonCode)
                    'To get the carton code scanned
                Case BCType.CODE128
                    HandleData(strCartonCode)
                Case BCType.EAN
                    HandleData(strCartonCode)
                Case Else
                    'If carton code is invalid display barcode not recognised
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", _
                                            MessageBoxButtons.OK, _
                                                 MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
            End Select
        'Clearing the text field

            m_ViewSuppliers.txtProductCode.Text = ""
        ' m_ViewSuppliers.objScanableField.txtProduct.Text = ""
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at view carton Handlescandata Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Main function to handle all scan data and manual entries as input.
    ''' </summary>
    ''' <param name="strCartonCode"></param>
    ''' <remarks></remarks>
    Public Sub HandleData(ByVal strCartonCode As String)
        bScanned = True
        Dim bTemp As Boolean = False
        Dim dtExpDeliveryDate As DateTime   'Minu added
        Try
            'Validating the ASN Barcode Entered
            If (objAppContainer.objHelper.ValidateASNBarcode(strCartonCode, objASNCode)) Then
                strASNCode = strCartonCode.ToString()

                'XXXCSK------------------------------------------
#If RF Then
                If Left(strASNCode, 6) = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DOTCOM_SUPPLIER_NUM) Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M138"), "Warning", _
                                   MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                    bTemp = True

                Else

#End If
                    'XXXCSK------------------------------------------


                    'invoke the GetSupplierInfo to get the Supplier details
                    If GetSupplierInfo() Then
                        If objAppContainer.objDataEngine.ValidateCartonScanned(strCartonCode, m_VCartonInfo, _
                                                           AppContainer.DeliveryType.ASN, AppContainer.FunctionType.View) Then
#If RF Then
                            If objAppContainer.objDataEngine.GetCartonDetails(strASNCode, m_ItemList, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.View) Then
#End If
#If NRF Then
                        Dim bBkd As Boolean
                        If objAppContainer.objDataEngine.GetCartonDetails(strASNCode, m_ItemList, AppContainer.DeliveryType.ASN, bBkd, AppContainer.FunctionType.View, ) Then
#End If
                                With m_VCartonInfo
                                    Select Case .Status.ToString()
                                        Case Status.Booked
                                            strBkdInStatus = Macros.Y
                                        Case Status.UnBooked
                                            strBkdInStatus = Macros.N
                                    End Select

                                    strCartonId = .CartonNumber.ToString()
                                    'strExptDate = FormatLongstrDate(.ExpDeliveryDate.ToString())
                                    'SFA DEF #834 - Date format correction
#If RF Then
                                    strExptDate = FormatLongstrDate(.ExpDeliveryDate.ToString())
#Else
                                'Minu Added
                                dtExpDeliveryDate = .ExpDeliveryDate
                                strExptDate = Format(dtExpDeliveryDate, "dd/MM/yy")
                                'Minu end
#End If

                                End With
                                bTemp = True
                                VCSessionManager.GetInstance.DisplayViewCarton(VCARTONSCREENS.ViewItems)
                            End If
                        End If
                    End If
                    'XXXCSK------------------------------------------
#If RF Then
                End If


#End If
                'XXXCSK------------------------------------------



                    If Not bTemp Then
#If RF Then
                        'TIMEOUT
                        If objAppContainer.bTimeOut Then
                            Exit Sub
                        End If
                        If Not objAppContainer.bCommFailure And Not objAppContainer.bReconnectSuccess Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M109"), _
                                            "Alert ", MessageBoxButtons.OK, _
                                            MessageBoxIcon.Exclamation, _
                                            MessageBoxDefaultButton.Button1)
                        End If

#ElseIf NRF Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M109"), _
                                    "Alert ", MessageBoxButtons.OK, _
                                    MessageBoxIcon.Exclamation, _
                                    MessageBoxDefaultButton.Button1)
#End If
                    End If
                Else
                    'Display appropriate message box if Carton code entered is invalid
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), _
                                    "Error ", MessageBoxButtons.OK, _
                                    MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                    objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at view carton handledata Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Quiting from the form and display the Goods In main Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub QuitSession()
        Dim iResult As Integer = 0
        ' Dim objGoodsInMainMenu As New frmGoodsInMenu
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M103"), "Confirmation", MessageBoxButtons.YesNo, _
                                       MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
            m_ViewSuppliers.UnFreezeControls()
            If iResult = MsgBoxResult.Yes Then
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
#If RF Then
                objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
                If objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.ASN, _
                                                AppContainer.FunctionType.View, AppContainer.IsAbort.No) Then
#ElseIf NRF Then
                objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.ASN, _
                                                AppContainer.FunctionType.View, AppContainer.IsAbort.No)
#End If
                m_viewCarton.Close()
                m_ViewItems.Close()
                m_ViewSuppliers.Close()
                m_viewCarton.Dispose()
                m_ViewItems.Dispose()
                m_ViewSuppliers.Dispose()
                m_SupplierList = Nothing
                m_CartonList = Nothing
                m_ItemList = Nothing
                m_VCartonInfo = Nothing
                objASNCode = Nothing
                '     m_VCSessionManager = Nothing
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
#If RF Then
                    objAppContainer.bReconnectSuccess = False
                End If
#End If
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at view carton quit Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Quiting from the Item details screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub QuitItemDetailsScreen()
        Dim iResult As Integer = 0
        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M103"), "Confirmation", MessageBoxButtons.YesNo, _
                                 MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        If iResult = MsgBoxResult.Yes Then
            DisplayViewCarton(VCSessionManager.VCARTONSCREENS.ViewSuppliers)
            m_viewCarton.Close()
            m_ViewItems.Close()
        End If
    End Sub
    ''' <summary>
    ''' To get the supplier name selected from Supplier list screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GetSupplierListSelectedIndex()
        'Dim strSupplierName As String = ""
        Dim iCounter As Integer
        With m_ViewSuppliers
            If .lvwSupplerList.SelectedIndices.Count > 0 Then
                For iCounter = 0 To .lvwSupplerList.Items.Count - 1
                    If .lvwSupplerList.Items(iCounter).Selected Then
                        strSupplierName = .lvwSupplerList.Items(iCounter).Text
                        Exit For
                    End If
                Next
            End If
        End With
        If strSupplierName = "" Then
            Exit Sub
        End If
        m_CartonList.Clear()
    End Sub
    ''' <summary>
    ''' Populating the Carton list screen based on the Supplier selected
    ''' </summary>
    ''' <remarks></remarks>
    Public Function GetCartonList() As Boolean
        Dim strExptDate As String = ""
        Dim dtExptDate As DateTime  'Minu added
        Try
            Dim objCartonComparer As New GenericComparer
            m_viewCarton.lvwCartonList.BeginUpdate()
            m_viewCarton.bDisableSelected = False
            If m_ViewItems.lvwItemDetails.Items.Count <> 0 Then
                m_ViewItems.lvwItemDetails.Items.Clear()
            End If
            'when returning from item details screen the value of m_carrton list wont be zero. so no need to retrieve from DB
            If m_CartonList.Count = 0 Then
                For Each objSupplier As GIValueHolder.SupplierList In m_SupplierList
                    If objSupplier.SupplierName = strSupplierName Then
                        strSupplierNo = objSupplier.SupplierNo
                    End If
                Next
                If m_CartonList.Count = 0 Then
                    'invoking the GetSupplierDetails method if m_cartonlist is empty
                    objAppContainer.objDataEngine.GetSupplierDetails(strSupplierNo, m_CartonList)
                    strNoOfCartonsinASN = m_CartonList.Count
                    'If there is no cartons for a particular supplier then display a message
                    If m_CartonList.Count = 0 Then
                        VCSessionManager.GetInstance.DisplayViewCarton(VCARTONSCREENS.ViewSuppliers)
#If RF Then
                        If Not objAppContainer.bCommFailure AndAlso Not objAppContainer.bReconnectSuccess Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M104"), "Alert", MessageBoxButtons.OK, _
                                       MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        End If
                        objAppContainer.bReconnectSuccess = False
#ElseIf NRF Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M104"), "Alert", MessageBoxButtons.OK, _
                                       MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
#End If
                        Return False
                    End If
                End If
                With m_viewCarton
                    'Adding columns  to the Carton List view
                    .lblSupplier.Text = strSupplierName.ToString()
                End With
            End If
            'Clearing the carton List view items
            m_viewCarton.lvwCartonList.Items.Clear()

            If m_CartonList.Count > 0 Then
                'Sorting the m_cartonlist arraylist based on the Expected date
                m_CartonList.Sort(0, m_CartonList.Count, objCartonComparer)
                'Adding items to the list view
                For Each objCarton As GIValueHolder.SupplierDetails In m_CartonList
                    Dim objCartonListView As ListViewItem = New ListViewItem
                    'Adding items to the list view
                    objCartonListView = m_viewCarton.lvwCartonList.Items.Add(New ListViewItem(objCarton.CartonNumber))
                    'strExptDate = FormatShortstrDate(objCarton.ExptDate)
                    'SFA DEF #834 - Date format correction
#If RF Then
                    strExptDate = FormatShortstrDate(objCarton.ExptDate)
#Else
                    'Minu Commented above line
                    dtExptDate = objCarton.ExptDate
                    strExptDate = Format(dtExptDate, "dd/MM")
                    'Minu end
#End If
                    objCartonListView.SubItems.Add(strExptDate.ToString())
                    objCartonListView.SubItems.Add(objCarton.Status.ToString())
                    objCartonListView.SubItems.Add(objCarton.TotalItemsInCarton)
                Next
            End If
            m_viewCarton.bDisableSelected = True
            m_viewCarton.lvwCartonList.EndUpdate()
            Return True
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at view carton getCartonList Session: " + ex.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' To get the Supplier information by querying the supplier list
    ''' </summary>
    ''' <remarks></remarks>
    Public Function GetSupplierInfo() As Boolean
        Dim bTemp As Boolean = False
        Dim arrSupplierList As New ArrayList
        Dim m_SupplierDataList As GIValueHolder.SupplierList
        Try
        If bScanned = True Then
            strSupplierNo = objASNCode.SupplierNumber.ToString()
            'retrieving the supplier details based on the Supplier selected
            For Each ObjSupplierInfo As GIValueHolder.SupplierList In m_SupplierList
                If ObjSupplierInfo.SupplierNo = strSupplierNo Then
                    strSupplierName = ObjSupplierInfo.SupplierName
                    arrSupplierList.Add(ObjSupplierInfo)
                    bTemp = True
                    Exit For
                End If
            Next
            If Not bTemp Then
                If objAppContainer.objDataEngine.GetSupplierData(strSupplierNo, m_SupplierDataList, AppContainer.DeliveryType.Directs, AppContainer.FunctionType.View) Then
                    strSupplierName = m_SupplierDataList.SupplierName.ToString()
                    bTemp = True
                End If

            End If
            'With m_VCartonInfo
            '    strBkdInStatus = .Status.ToString()
            '    strCartonId = .CartonNumber.ToString()
            '    strExptDate = FormatLongstrDate(.ExpDeliveryDate.ToString())
                'End With
        Return bTemp

            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occurred at view carton getsupplierinfo Session: " + ex.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' To retrieve the carton details based on the Carton selected
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCartonInfo() As Boolean
        strCartonId = ""
        Dim iCounter As Integer
        Dim dtExptDate As DateTime  'Minu added
        Try
            Dim arrCartonDetailList As New ArrayList
            Dim arrCartonDataList As New ArrayList
            With m_viewCarton
                'retrieving details of the carton selected
                If .lvwCartonList.SelectedIndices.Count > 0 Then
                    For iCounter = 0 To .lvwCartonList.Items.Count - 1
                        If .lvwCartonList.Items(iCounter).Selected Then
                            strCartonId = .lvwCartonList.Items(iCounter).Text
                            Exit For
                        End If
                    Next
                End If
            End With
            If strCartonId = "" Then
                Return False
            End If
            'Adding details of carton selected to a arraylist
            For Each objCarton As GIValueHolder.SupplierDetails In m_CartonList
                If objCarton.CartonNumber = strCartonId Then
                    strBkdInStatus = objCarton.Status.ToString()
                    strCartonId = objCarton.CartonNumber.ToString()
                    strASNCode = strSupplierNo + strCartonId + strNoOfCartonsinASN.PadLeft(4, "0")
                    'strExptDate = FormatLongstrDate(objCarton.ExptDate.ToString())
                    'SFA DEF #834 - Date format correction
#If RF Then
                    strExptDate = FormatLongstrDate(objCarton.ExptDate.ToString())
#Else
                    'Minu Added
                    dtExptDate = objCarton.ExptDate
                    strExptDate = Format(dtExptDate, "dd/MM/yy")
                    'Minu end
#End If
                    Exit For
                End If
            Next
           'Clearing the itemlist arraylist
            m_ItemList.Clear()
            'Invoking the GetCartonDetails to populate the ItemList 
#If RF Then
 If objAppContainer.objDataEngine.GetCartonDetails(strASNCode, m_ItemList, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.View) Then
                Return True
            Else
                Return False
            End If
#End If
#If NRF Then
            Dim bBkd As Boolean
            If objAppContainer.objDataEngine.GetCartonDetails(strASNCode, m_ItemList, AppContainer.DeliveryType.ASN, bBkd, AppContainer.FunctionType.View) Then
                Return True
            Else
                Return False
            End If
#End If


        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at view carton getcartonInfo Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try

    End Function
    ''' <summary>
    ''' 'FormatShortstrDate is used to convert the yymmdd string data
    '''  into ddmm date format
    ''' into ddmmyy date format
    ''' </summary>
    ''' <param name="strDate"></param>
    ''' <returns>strDate</returns>
    ''' <remarks></remarks>
    Public Function FormatShortstrDate(ByRef strDate As String) As String
        Dim mm As String = strDate.Substring(4, 2)
        Dim dd As String = strDate.Substring(6, 2)
        'strDate = dd + "/" + mm
        Return dd + "/" + mm
    End Function
    ''' <summary>
    ''' FormatLongstrDate is used to convert the yymmdd string data 
    ''' into ddmmyy date format
    ''' </summary>
    ''' <param name="strdate"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FormatLongstrDate(ByRef strdate As String) As String
        Dim yy As String = strdate.Substring(2, 2)
        Dim mm As String = strdate.Substring(4, 2)
        Dim dd As String = strdate.Substring(6, 2)
        'strDate = dd + "/" + mm
        Return dd + "/" + mm + "/" + yy
    End Function
    ''' <summary>
    ''' To populate the Supplier List screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PopulateSupplierList()
        Dim objItemComparer As New GenericComparer
        Dim strSupplierNo As String = ""
        m_ViewSuppliers.bOverrideSelected = False
        'code to over ride selected index change -----------------------------------------------------------
        With m_ViewSuppliers
            .lvwSupplerList.BeginUpdate()
            'Clearing the items of the listview
            .lvwSupplerList.Items.Clear()
            'Populate the arraylist if the arraylist count is zero
            If (m_SupplierList.Count = 0) Then
#If RF Then
                If Not objAppContainer.objDataEngine.GetSupplierListForView(m_SupplierList) Then
                    Exit Sub
                End If
#ElseIf NRF Then
                 objAppContainer.objDataEngine.GetSupplierListForView(m_SupplierList)
#End If

                m_SupplierList.Sort(0, m_SupplierList.Count, objItemComparer)
            End If
            'Adding items to the Supplier List view
            For Each objSupplier As GIValueHolder.SupplierList In m_SupplierList
                Dim objSupplierListView As ListViewItem = New ListViewItem
                objSupplierListView = m_ViewSuppliers.lvwSupplerList.Items.Add(New ListViewItem(objSupplier.SupplierName))
                objSupplierListView.SubItems.Add(objSupplier.SupplierQty)
            Next
            .lvwSupplerList.EndUpdate()
            .bOverrideSelected = True
        End With
    End Sub

    Public Class GenericComparer
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
                Dim iResult As Int16
                'SFA DEF #834 - Date format correction
#If RF Then
                iResult = Date.Compare(DateTime.ParseExact(objItemX.ExptDate, "yyyyMMdd", CultureInfo.InvariantCulture), DateTime.ParseExact(objItemY.ExptDate, "yyyyMMdd", CultureInfo.InvariantCulture))
#Else
                'Minu Removed parseexact
                 iResult = Date.Compare(objItemX.ExptDate, objItemY.ExptDate)
#End If
                
                If iResult = 0 Then
                    Return Int64.Parse(objItemX.CartonNumber).CompareTo(Int64.Parse(objItemY.CartonNumber))
                Else
                    Return iResult
                End If

                ' Return Date.Compare(DateTime.ParseExact(objItemX.ExptDate, "yyyyMMdd", CultureInfo.InvariantCulture), DateTime.ParseExact(objItemY.ExptDate, "yyyyMMdd", CultureInfo.InvariantCulture))
                'Sort the arraylist based on Item Description
            ElseIf TypeOf objx Is GIValueHolder.ItemDetails Then
                Dim objItemX As GIValueHolder.ItemDetails = DirectCast(objx, GIValueHolder.ItemDetails)
                Dim objItemY As GIValueHolder.ItemDetails = DirectCast(objy, GIValueHolder.ItemDetails)
                Return String.Compare(objItemX.ItemDesc, objItemY.ItemDesc)
            End If
        End Function
    End Class
End Class
