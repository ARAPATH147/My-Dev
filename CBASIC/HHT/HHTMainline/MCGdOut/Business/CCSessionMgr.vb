'''***************************************************************
''' <FileName>CCSessionMgr.vb</FileName>
''' <summary>
''' The Credit Claim feature class which will 
''' intialise all the parameters with respect to Credit Claim.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
Public Class CCSessionMgr
    Public Shared objCCSessionMgr As CCSessionMgr = Nothing


    'Declaring the Form objects
    Private m_frmCCScan As frmCCScan = Nothing
    Private m_frmItemDetails As frmCCItemDetails = Nothing
    Private m_frmCCVoidItemList As frmCCVoidItemList = Nothing
    Private m_frmCCSummary As frmCCSummary = Nothing

    'Declaring the an object to store item info
    Private m_objCCItemInfo As GOItemInfo = Nothing

    'Declare array list to store Item info
    Private m_CCItemList As ArrayList = Nothing

    Private m_BCType As String = Nothing
    'Private m_BCDesc As String = Nothing
    Private m_SupplyRoute As String = Nothing
    Private m_SequenceNumber As Integer = 1
    ''' <summary>
    ''' Constructor initiates data and forms
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.CRDCLM
        Try
            'Instantiate all the objects required
            ' Me.StartSession()
        Catch ex As Exception
            'Handle Goods out Init Exception here.
            objAppContainer.objLogger.WriteAppLog("Exception occured in Constructor of CCSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Me.EndSession()
        End Try
    End Sub
    ''' <summary>
    ''' Function to make the class singleton
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As CCSessionMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.CRDCLM
        If objCCSessionMgr Is Nothing Then
            objAppContainer.objLogger.WriteAppLog("Creating New Instance of CC-Session Manager", Logger.LogLevel.RELEASE)
            objCCSessionMgr = New CCSessionMgr
        End If
        Return objCCSessionMgr
    End Function

    ''' <summary>
    ''' Initialises the Credit Claim Session  
    ''' </summary>
    ''' <remarks></remarks>
    Public Function StartSession() As Boolean
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered StartSession of CCSessionMgr", Logger.LogLevel.INFO)
        Try
#If RF Then
            If objAppContainer.eLastUsedModule = "None" Or _
              (Not objAppContainer.eLastUsedModule.Equals("None") And Not _
              objAppContainer.eLastUsedModule.Equals(WorkflowMgr.GetInstance().WFIndex)) Then
                Dim objUOS As UOSRecord = New UOSRecord()
                objUOS.strIsListType = "C"      'C - denoted it is sent for credit claiming module.
                'If RF Mode then send UOS right here.
                If Not objAppContainer.objExportDataManager.CreateUOS(objUOS) Then
                    objUOS = Nothing
                    Return False
                End If
                'Update the work flow index to public variable
                objAppContainer.eLastUsedModule = WorkflowMgr.GetInstance().WFIndex
                objUOS = Nothing
                objAppContainer.objLogger.WriteAppLog("Succeeded UOS sending", Logger.LogLevel.RELEASE)
                'Instantiating a class to store the CCItemInfo objects
                m_CCItemList = New ArrayList()
            Else
                If objAppContainer.eLastUsedModule.Equals(WorkflowMgr.GetInstance().WFIndex) Then
                    'Instantiating a class to store the CCItemInfo objects
                    m_CCItemList = New ArrayList()
                    m_CCItemList = objAppContainer.objGlobalItemList
                End If
            End If
#ElseIf NRF Then
            'Instantiating a class to store the CCItemInfo objects
            m_CCItemList = New ArrayList()
#End If
            'All the Goods out related forms are instantiated.
            m_frmCCScan = New frmCCScan
            m_frmItemDetails = New frmCCItemDetails
            m_frmCCVoidItemList = New frmCCVoidItemList
            m_frmCCSummary = New frmCCSummary
            Return True
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in StartSession of CCSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_REGAINED Or ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Then
                Throw ex
            End If
#End If
            Return False
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit StartSession of CCSessionMgr", Logger.LogLevel.INFO)

    End Function
    ''' <summary>
    ''' Updates the status bar message in all forms
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBar()
        Try
            m_frmCCScan.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_frmItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_frmCCVoidItemList.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_frmCCSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Eoccured @: " + ex.StackTrace, Logger.LogLevel.INFO)
        End Try
    End Sub
    ''' <summary>
    ''' The Method handles scan the scan data returned form the barcode scanner.
    ''' This method implements the business logic to populate the data to the corresponding
    ''' UI element after validation.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="Type"></param>
    ''' <remarks></remarks>
    Public Sub HandleScanData(ByVal strBarcode As String, ByVal Type As BCType)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered HandleScanData of CCSessionMgr", Logger.LogLevel.INFO)
        Try
            CCSessionMgr.GetInstance().m_frmCCScan.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            CCSessionMgr.GetInstance().m_frmCCScan.Refresh()
            Select Case Type
                Case BCType.EAN
                    CCSessionMgr.GetInstance().ProcessScanItem(strBarcode, False)
                Case BCType.UPC
                    CCSessionMgr.GetInstance().ProcessScanItem(strBarcode, False)
                Case BCType.ManualEntry
                    'strBarcode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                    CCSessionMgr.GetInstance().ProcessScanItem(strBarcode, True)
                Case BCType.SEL
                    'DARWIN CHANGE to Handle Clearance Label
                    If strBarcode.StartsWith("8270") And (strBarcode.Length > 12) Then
                        strBarcode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode.Substring(4, 6))
                        m_objCCItemInfo = New GOItemInfo()
                        'Get the product info from the Data Access Layer
                        If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBarcode, m_objCCItemInfo)) Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M11"), _
                                            "Item not found", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                            MessageBoxDefaultButton.Button1)
                        Else
                            'Fix for item not on file
                            If m_BCType = Nothing Then
                                If Not m_objCCItemInfo.BusinessCentreType = "" Then
                                    m_BCType = m_objCCItemInfo.BusinessCentreType
                                    m_SupplyRoute = m_objCCItemInfo.SupplyRoute
                                End If
                            End If
                            'If item details are successfully retrieved show the Item details screen
                            CCSessionMgr.GetInstance().DisplayCCScreen(CCSCREENS.ItemDetails)
                        End If
                        'Check if the SEL is valid
                    ElseIf objAppContainer.objHelper.ValidateSEL(strBarcode) Then
                        Dim strBootsCode As String = ""
                        objAppContainer.objHelper.GetBootsCodeFromSEL(strBarcode, strBootsCode)
                        strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
                        m_objCCItemInfo = New GOItemInfo()
                        'Get the product info from the Data Access Layer
                        If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBootsCode, m_objCCItemInfo)) Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M11"), _
                                            "Item not found", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                            MessageBoxDefaultButton.Button1)
                        Else
                            'Fix for item not on file
                            If m_BCType = Nothing Then
                                If Not m_objCCItemInfo.BusinessCentreType = "" Then
                                    m_BCType = m_objCCItemInfo.BusinessCentreType
                                    m_SupplyRoute = m_objCCItemInfo.SupplyRoute
                                End If
                            End If
                            'If item details are successfully retrieved show the Item details screen
                            CCSessionMgr.GetInstance().DisplayCCScreen(CCSCREENS.ItemDetails)
                        End If
                    Else
                        'Handle invalid SEL here
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M7"), _
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                        MessageBoxDefaultButton.Button1)
                    End If
            End Select
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in HandleScanData of CCSessionMgr. Exception is: " _
                                                  + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
#If RF Then
            If (Not m_frmCCScan Is Nothing) AndAlso (Not m_frmCCScan.IsDisposed) Then
                CCSessionMgr.GetInstance().m_frmCCScan.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End If
#ElseIf NRF Then
                CCSessionMgr.GetInstance().m_frmCCScan.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#End If

        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit HandleScanData of CCSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Screen Display method for Goods Out. 
    ''' All Goods Out sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName">Enum SMSCREENS</param>
    ''' <returns>True if display is sucess else False</returns>
    ''' <remarks></remarks>
    Public Function DisplayCCScreen(ByVal ScreenName As CCSCREENS)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayCCScreen of CCSessionMgr", Logger.LogLevel.INFO)
        Try
            Select Case ScreenName
                Case CCSCREENS.Scan
                    m_frmCCScan.Invoke(New EventHandler(AddressOf DisplayScan))
                Case CCSCREENS.ItemDetails
                    m_frmItemDetails.Invoke(New EventHandler(AddressOf DisplayItemDetails))
                Case CCSCREENS.ItemList
                    m_frmCCVoidItemList.Invoke(New EventHandler(AddressOf DisplayItemListScreen))
                Case CCSCREENS.Summary
                    m_frmCCSummary.Invoke(New EventHandler(AddressOf DisplaySummary))
            End Select
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayCCScreen of CCSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try

        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCCScreen of CCSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Populate and display the Product Scan screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayScan(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayScan of CCSessionMgr", Logger.LogLevel.INFO)

        Try
            With m_frmCCScan
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                .objProdSEL.txtProduct.Text = ""
                .btnFinish.Visible = False
                'Retrun Finish is only visible if atleast one item is scanned
                If m_CCItemList.Count > 0 Then
                    .btnFinish.Visible = True
                End If
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception

            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayScan of CCSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            If Not m_frmCCScan Is Nothing Then
                CCSessionMgr.GetInstance().m_frmCCScan.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End If
        End Try

        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayScan of CCSessionMgr", Logger.LogLevel.INFO)

    End Sub
    ''' <summary>
    ''' Populate and display the Item details screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayItemDetails(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayItemDetails of CCSessionMgr", Logger.LogLevel.INFO)
        Try
            'If the item is already scanned then the quantity is fetched for display
            m_frmItemDetails.lblQuantityData.Text = "1"
            For Each objItemInfo As GOItemInfo In m_CCItemList
                If objItemInfo.ProductCode = m_objCCItemInfo.ProductCode Or _
                objItemInfo.FirstBarcode = m_objCCItemInfo.FirstBarcode Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M59"), _
                    "Item Added", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                    MessageBoxDefaultButton.Button1)
                    'Dim iTemp As Integer = CInt(objItemInfo.Quantity) + 1
                    'm_frmItemDetails.lblQuantityData.Text = iTemp.ToString()
                    If Val(objItemInfo.Quantity) < 999 Then
                        m_frmItemDetails.lblQuantityData.Text = objItemInfo.Quantity + 1
                    Else
                        'Display message here to inform user that count is not incremented.
                        MessageBox.Show("Quantity cannot be more than 999. Previous count will be displayed.", _
                                        "Count Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                        MessageBoxDefaultButton.Button1)
                        m_frmItemDetails.lblQuantityData.Text = objItemInfo.Quantity
                    End If
                    m_objCCItemInfo.SequenceNumber = objItemInfo.SequenceNumber
                    Exit For
                End If
            Next
            Dim objDescriptionArray As ArrayList = New ArrayList()
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(m_objCCItemInfo.Description)
            'Populate the form controls and format them for user display
            With m_frmItemDetails
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                'Fix for item not on file
                If m_objCCItemInfo.BusinessCentreType = "" Then
                    .lblBusCentreDesc.Text = ""
                Else
                    .lblBusCentreDesc.Text = objAppContainer.objHelper.FormatEscapeSequence _
                                                           (ConfigDataMgr.GetInstance.GetParam(m_objCCItemInfo.BusinessCentreType))
                End If
                .lblBootsCode.Text = objAppContainer.objHelper.FormatBarcode(m_objCCItemInfo.BootsCode)
                .lblEAN.Text = objAppContainer.objHelper.FormatBarcode(objAppContainer.objHelper.GeneratePCwithCDV(m_objCCItemInfo.ProductCode))
                'The item description is formatted to be displayed on 3 lines
                .lblItemDesc1.Text = objAppContainer.objHelper.FormatEscapeSequence(objDescriptionArray.Item(0))
                .lblItemDesc2.Text = objAppContainer.objHelper.FormatEscapeSequence(objDescriptionArray.Item(1))
                .lblItemDesc3.Text = objAppContainer.objHelper.FormatEscapeSequence(objDescriptionArray.Item(2))
                .Visible = True
                .Refresh()
            End With
            objDescriptionArray = Nothing
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayItemDetails of CCSessionMgr. Exception is: " _
                                                           + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            CCSessionMgr.GetInstance().m_frmItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayItemDetails of CCSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Display the item scan list and the total items in claim
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayItemListScreen(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayItemListScreen of CCSessionMgr", Logger.LogLevel.INFO)
        Try
            'Populate the list view control with data from the CC item list
            m_frmCCVoidItemList.lvItemList.Items.Clear()
            For Each m_Item As GOItemInfo In m_CCItemList
                Dim objListItem As ListViewItem
                objListItem = m_frmCCVoidItemList.lvItemList.Items.Add(New ListViewItem(m_Item.Quantity))
                objListItem.SubItems.Add(m_Item.ShortDescription)
            Next

            'Populating the controls
            With m_frmCCVoidItemList
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                .lblTotalData.Text = Me.GetItemCount().ToString
                .Visible = True
                .Refresh()

            End With
        Catch ex As Exception

            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayItemListScreen of CCSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            CCSessionMgr.GetInstance().m_frmCCVoidItemList.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try

        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayItemListScreen of CCSessionMgr", Logger.LogLevel.INFO)

    End Sub
    ''' <summary>
    ''' Populate and display the Summary Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplaySummary(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplaySummary of CCSessionMgr", Logger.LogLevel.INFO)
        Try
            'Populating the controls
            With m_frmCCSummary
                .btnOk.Enabled = True
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
#If RF Then
                'Removed "Dock and Transmit" from the text for RF - MC55RF testing
                .lblCompleteInstruction.Text = "Claim Completed"
#End If
                .lblTotSinglesData.Text = CCSessionMgr.GetInstance().GetItemCount().ToString
                .lblTotValueData.Text = CCSessionMgr.GetInstance().CalculateTotalValue().ToString("0.00")
                .Visible = True
                .Refresh()
                'Set active module to none so that in case of auto logoff
                'export data is not written for the second time.
                objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.NONE
            End With
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplaySummary of CCSessionMgr. Exception is: " _
                                                  + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            CCSessionMgr.GetInstance().m_frmCCSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplaySummary of CCSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Fetches the items scanned and returns it in the form of an arraylist
    ''' </summary>
    ''' <returns>List of items scanned</returns>
    ''' <remarks></remarks>
    Public Function GetItemList() As ArrayList
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered GetItemList of CCSessionMgr", Logger.LogLevel.INFO)
        Try
            If m_CCItemList IsNot Nothing Then
                'Writing to Log INFO File while exit
                objAppContainer.objLogger.WriteAppLog("Exit GetItemList of CCSessionMgr", Logger.LogLevel.INFO)
                Return m_CCItemList
            Else
                'Writing to Log INFO File while exit
                objAppContainer.objLogger.WriteAppLog("Exit GetItemList of CCSessionMgr", Logger.LogLevel.INFO)
                Return Nothing
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in GetItemList of CCSessionMgr. Exception is: " _
                                                            + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return Nothing
        End Try


    End Function
    ''' <summary>
    ''' Sets the Product info in GOItemInfo into the array list
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetProductInfo(Optional ByVal strItemStatus As String = "A")
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered SetProductInfo of CCSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim bAlreadyScanned As Boolean = False
            'Fix for item not on file
            Dim objTemp As GOItemInfo
            'get the data from the qualitydata label and enter it in the Item info object
            m_objCCItemInfo.Quantity = m_frmItemDetails.lblQuantityData.Text
            'Set the sequence number in order track the item in case of rf mode.
            If m_objCCItemInfo.SequenceNumber = "" Then
                m_objCCItemInfo.SequenceNumber = m_SequenceNumber.ToString()
                'm_SequenceNumber += 1
            End If
#If NRF Then
            'If item is already scanned remove the item object and add a new item object with appended quantity data
            For Each objItemInfo As GOItemInfo In m_CCItemList
                If objItemInfo.ProductCode = m_objCCItemInfo.ProductCode Or _
                 objItemInfo.FirstBarcode = m_objCCItemInfo.FirstBarcode Then
                    'Read the sequence number to update the same item in the list CLLOL
                    m_objCCItemInfo.SequenceNumber = objItemInfo.SequenceNumber
                    If strItemStatus = "X" Or objItemInfo.Quantity <> m_objCCItemInfo.Quantity Then
                        m_CCItemList.Remove(objItemInfo)
                        Exit For
                    Else
                        Exit Sub
                    End If
                End If
            Next
            'Add updated item object to array.
            If strItemStatus = "A" Then
                'Add if the call is not for Void item.
                objAppContainer.objLogger.WriteAppLog("CCSessionMgr::SetProductInfo: Quantity entered = " & m_frmItemDetails.lblQuantityData.Text, Logger.LogLevel.RELEASE)
                'Fix for item not on file
                If m_CCItemList.Count > 0 Then
                    objTemp = m_CCItemList.Item(0)
                    If objTemp.BusinessCentreType = "" And m_objCCItemInfo.BusinessCentreType <> "" Then
                        objTemp.BusinessCentreType = m_objCCItemInfo.BusinessCentreType
                        m_BCType = m_objCCItemInfo.BusinessCentreType
                        'Fix
                        m_CCItemList.RemoveAt(0)
                        m_CCItemList.Insert(0, objTemp)
                    ElseIf m_objCCItemInfo.BusinessCentreType = "" And objTemp.BusinessCentreType <> "" Then
                        m_objCCItemInfo.BusinessCentreType = objTemp.BusinessCentreType
                        m_BCType = m_objCCItemInfo.BusinessCentreType
                    End If
                End If
                'Fix for item not on file
                If m_CCItemList.Count > 0 Then
                    objTemp = m_CCItemList.Item(0)
                    If objTemp.SupplyRoute = "" And m_objCCItemInfo.SupplyRoute <> "" Then
                        objTemp.SupplyRoute = m_objCCItemInfo.SupplyRoute
                        m_SupplyRoute = m_objCCItemInfo.SupplyRoute
                        'Fix
                        m_CCItemList.RemoveAt(0)
                        m_CCItemList.Insert(0, objTemp)
                    ElseIf m_objCCItemInfo.SupplyRoute = "" And objTemp.SupplyRoute <> "" Then
                        m_objCCItemInfo.SupplyRoute = objTemp.SupplyRoute
                        m_SupplyRoute = m_objCCItemInfo.SupplyRoute
                    End If
                End If
                'Add the Item info object to the array list
                m_CCItemList.Add(m_objCCItemInfo)
            End If
#End If
#If RF Then
            'In case of RF mode send the UOA message right here.
            Dim objUOA As UOARecord = New UOARecord()
            Try
                Dim totalPrice As Double = 0.0
                m_frmCCVoidItemList.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing UOA: " + _
                                                            m_objCCItemInfo.BootsCode)
                objUOA.strsequencenumber = CStr(m_objCCItemInfo.SequenceNumber)
                objUOA.strbootscode = m_objCCItemInfo.BootsCode
                objUOA.strquanity = m_objCCItemInfo.Quantity
                objUOA.strsdescription = m_objCCItemInfo.ShortDescription
                objUOA.strdescSEL = m_objCCItemInfo.Description
                objUOA.strnumPrice = m_objCCItemInfo.ItemPrice
                totalPrice = CDbl(m_objCCItemInfo.ItemPrice) * CDbl(m_objCCItemInfo.Quantity)
                objUOA.strnumTotalPrice = CStr(totalPrice)
                objUOA.stritembc = m_objCCItemInfo.BusinessCentreType
                objUOA.strbcname = ConfigDataMgr.GetInstance().GetParam(m_objCCItemInfo.BusinessCentreType)
                objUOA.strbarcode = m_objCCItemInfo.ProductCode
                'TODO: this status will always be A since the we dont send a UOA for cancelling an item
                objUOA.strIsStatus = strItemStatus

                'Get the data from the DAL and then add the supply route to this variable
                If objAppContainer.objExportDataManager.CreateUOA(objUOA) Then
                    'If item is already scanned remove the item object and add a new item object with appended quantity data
                    For Each objItemInfo As GOItemInfo In m_CCItemList
                        If objItemInfo.ProductCode = m_objCCItemInfo.ProductCode Or _
                         objItemInfo.FirstBarcode = m_objCCItemInfo.FirstBarcode Then
                            bAlreadyScanned = True
                            'Read the sequence number to update the same item in the list CLLOL
                            m_objCCItemInfo.SequenceNumber = objItemInfo.SequenceNumber
                            If strItemStatus = "X" Or objItemInfo.Quantity <> m_objCCItemInfo.Quantity Then
                                m_CCItemList.Remove(objItemInfo)
                                Exit For
                            Else
                                Exit Sub
                            End If
                        End If
                    Next
                    'Add the updated item to the list.
                    If strItemStatus = "A" Then
                        'Add if the call is not for Void item.
                        objAppContainer.objLogger.WriteAppLog("CCSessionMgr::SetProductInfo: Quantity entered = " & m_frmItemDetails.lblQuantityData.Text, Logger.LogLevel.RELEASE)
                        'Fix for item not on file
                        If m_CCItemList.Count > 0 Then
                            objTemp = m_CCItemList.Item(0)
                            If objTemp.BusinessCentreType = "" And m_objCCItemInfo.BusinessCentreType <> "" Then
                                objTemp.BusinessCentreType = m_objCCItemInfo.BusinessCentreType
                                m_BCType = m_objCCItemInfo.BusinessCentreType
                                'Fix
                                m_CCItemList.RemoveAt(0)
                                m_CCItemList.Insert(0, objTemp)
                            ElseIf m_objCCItemInfo.BusinessCentreType = "" And objTemp.BusinessCentreType <> "" Then
                                m_objCCItemInfo.BusinessCentreType = objTemp.BusinessCentreType
                                m_BCType = m_objCCItemInfo.BusinessCentreType
                            End If
                        End If
                        'Fix for item not on file
                        If m_CCItemList.Count > 0 Then
                            objTemp = m_CCItemList.Item(0)
                            If objTemp.SupplyRoute = "" And m_objCCItemInfo.SupplyRoute <> "" Then
                                objTemp.SupplyRoute = m_objCCItemInfo.SupplyRoute
                                m_SupplyRoute = m_objCCItemInfo.SupplyRoute
                                'Fix
                                m_CCItemList.RemoveAt(0)
                                m_CCItemList.Insert(0, objTemp)
                            ElseIf m_objCCItemInfo.SupplyRoute = "" And objTemp.SupplyRoute <> "" Then
                                m_objCCItemInfo.SupplyRoute = objTemp.SupplyRoute
                                m_SupplyRoute = m_objCCItemInfo.SupplyRoute
                            End If
                        End If
                        'Add the Item info object to the array list
                        m_CCItemList.Add(m_objCCItemInfo)
                    End If
                    If strItemStatus <> "X" And Not bAlreadyScanned Then
                        m_SequenceNumber += 1
                        'Return False
                    End If
                End If
                objAppContainer.objLogger.WriteAppLog("Credit Claim : UOA written successfully", Logger.LogLevel.RELEASE)
            Catch ex As Exception
                If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Or _
                                ex.Message = Macros.CONNECTION_REGAINED Then
                    Throw ex
                End If
            Finally
                objUOA = Nothing
            End Try
#End If
            m_objCCItemInfo = Nothing
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in SetProductInfo of CCSessionMgr. Exception is: " _
                                                    + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit SetProductInfo of CCSessionMgr", Logger.LogLevel.INFO)

    End Sub
    ''' <summary>
    ''' Call the Data access layer functions to generate export data files
    ''' </summary>
    ''' <remarks></remarks>
#If NRF Then
        Public Sub GenerateExportData()
#ElseIf RF Then
    Public Function GenerateExportData() As Boolean
#End If

        'Creating a variable of the type CreateUOS
        Dim objUOS As UOSRecord = Nothing
        Dim objUOA As UOARecord = Nothing
        Dim objUOX As UOXRecord = Nothing
        Dim totalPrice As Double = 0.0

        'Handling Autologoff scenario for no data
#If NRF Then
        If m_CCItemList Is Nothing Then
            Exit Sub
        End If
#End If
        
        If m_CCItemList.Count < 1 Then
#If NRF Then
            Exit SUB
#ElseIf RF Then
            Exit Function
#End If
        End If
        m_frmCCVoidItemList.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing UOS")
        objUOS.strIsListType = "C"
        'TODO : Call UOS in DAL
#If NRF Then
        objAppContainer.objExportDataManager.CreateUOS(objUOS)
#End If
        objAppContainer.objLogger.WriteAppLog("Credit Claim : UOS written successfully", Logger.LogLevel.RELEASE)
#If NRF Then
        'Variable to define a sequence number
        Dim iSequenceNo As Integer = 1
        For Each objItem As GOItemInfo In m_CCItemList
            m_frmCCVoidItemList.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing UOA: " + objItem.BootsCode)
            objUOA.strsequencenumber = CStr(iSequenceNo)
            objUOA.strbootscode = objItem.BootsCode
            objUOA.strquanity = objItem.Quantity
            objUOA.strsdescription = objItem.ShortDescription
            objUOA.strdescSEL = objItem.Description
            objUOA.strnumPrice = objItem.ItemPrice
            totalPrice = CDbl(objItem.ItemPrice) * CDbl(objItem.Quantity)
            objUOA.strnumTotalPrice = CStr(totalPrice)
            objUOA.stritembc = objItem.BusinessCentreType
            objUOA.strbcname = ConfigDataMgr.GetInstance().GetParam(objItem.BusinessCentreType)
            objUOA.strbarcode = objItem.ProductCode
            'TODO: this status will always be A since the we dont send a UOA for cancelling an item
            objUOA.strIsStatus = "A"
            iSequenceNo += 1
            'TODO : Get the data from the DAL and then add the supply route to this variable

            objAppContainer.objExportDataManager.CreateUOA(objUOA)
            '#ElseIf RF Then
            '            If Not objAppContainer.objExportDataManager.CreateUOA(objUOA) Then
            '                Return False
            '            End If
            objAppContainer.objLogger.WriteAppLog("Credit Claim : UOA written successfully", Logger.LogLevel.RELEASE)
        Next
#End If

        'Creating a varible of the type CreateUOX to be sent to DAL
        m_frmCCVoidItemList.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing UOX")
        objUOX.strisListType = "C"
        'Since credit claim does not have a UOD this field is left blank
        objUOX.strUOD = ""
        'TODO : Status can be cancel
        objUOX.strIsStatus = "D"
        objUOX.strItemCount = GetItemCount().ToString
        objUOX.strIsStockFigure = "Y" 'Hardcoded to Y as per the RF Goods out Credit claim DD
        'TODO : Get the data from the DAL and then add the supply route to this variable
        objUOX.strSupplierRoute = m_SupplyRoute
        objUOX.strDisplayLoc = "" 'Empty as per PPC
        'Since Credit claim is not specific to a since BC type these fields are left blank
        objUOX.strBCname = m_BCType
        objUOX.strBCdesc = ConfigDataMgr.GetInstance().GetParam(m_BCType)
        objUOX.strRecall = ""
        objUOX.strAuthCode = ""
        objUOX.strSupplier = ""
        objUOX.strMethod = WorkflowMgr.GetInstance().MethodOfReturn
        objUOX.strCarrier = WorkflowMgr.GetInstance().Carrier
        objUOX.strNumbird = "" 'Hardcoded as empty always
        objUOX.strNumReason = WorkflowMgr.GetInstance().ReasonCodeNum
        'TODO: While writing data for Store transfer put a check condition
        objUOX.strRecStore = ""
        objUOX.strDestination = WorkflowMgr.GetInstance().Destination
        'Depends on the supplier route if supplier route is C then warehouse is C else its R
        objUOX.strWroute = ""
        objUOX.strIsUODType = WorkflowMgr.GetInstance().UODType
        If objUOX.strIsUODType = "03" Or objUOX.strIsUODType = "3" Then
            objUOX.strReasonDamage = "02"
        Else
            objUOX.strReasonDamage = ""
        End If
        'TODO : Call UOX in DAL
#If NRF Then
        objAppContainer.objExportDataManager.CreateUOX(objUOX)
        'Clear the item list once the export data is written to the file.
        'm_CCItemList.Clear() - The alternative change is done in displaysummaryscreen()
#ElseIf RF Then
        Try
            If Not objAppContainer.objExportDataManager.CreateUOX(objUOX) Then
                Return False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occured in Generate Export data - " + _
                                                      "CCSession Manager", Logger.LogLevel.RELEASE)
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
        Finally
            objUOX = Nothing
        End Try
#End If
        objAppContainer.objLogger.WriteAppLog("Credit Claim : UOX written successfully", Logger.LogLevel.RELEASE)
#If NRF Then
    End Sub
#ElseIf RF Then
        Return True
    End Function
#End If


    ''' <summary>
    ''' Gets the total items in the CC item list
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetItemCount() As Integer
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered GetItemCount of CCSessionMgr", Logger.LogLevel.INFO)
        Dim m_TempClaimCount As Integer
        Try
            For Each m_TempList As GOItemInfo In m_CCItemList
                m_TempClaimCount += CInt(m_TempList.Quantity)
            Next
        Catch ex As Exception

            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in GetItemCount of CCSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit GetItemCount of CCSessionMgr", Logger.LogLevel.INFO)
        Return m_TempClaimCount
    End Function
    ''' <summary>
    ''' Calculate the total price of all the items in UOD
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CalculateTotalValue() As Double
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered CalculateTotalValue of CCSessionMgr", Logger.LogLevel.INFO)
        Dim dTotal As Double = 0.0
        Try
            For Each m_Item As GOItemInfo In m_CCItemList
                dTotal = dTotal + (CDbl(m_Item.ItemPrice) * CDbl(m_Item.Quantity))
            Next
            If Not dTotal = 0.0 Then
                dTotal = dTotal / 100.0
            End If
            'Writing to Log INFO File while exit
            objAppContainer.objLogger.WriteAppLog("Exit CalculateTotalValue of CCSessionMgr", Logger.LogLevel.INFO)
            Return dTotal
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in CalculateTotalValue of CCSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    ''' Function deletes a selected item from the CCItemList list
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub VoidItemInfo()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered VoidItemInfo of CCSessionMgr", Logger.LogLevel.INFO)

        Try
            'Get index of the items selected 
            Dim indexes As ListView.SelectedIndexCollection = m_frmCCVoidItemList.lvItemList.SelectedIndices
            For Each idx As Integer In indexes
                For Each m_Item As GOItemInfo In m_CCItemList
                    'Get the object of the selected item and delete it
                    If m_Item.ShortDescription = m_frmCCVoidItemList.lvItemList.Items(idx).SubItems(1).Text Then
                        If MsgBox("Are you sure?", MsgBoxStyle.OkCancel, "Void Selected Item") = MsgBoxResult.Ok Then
                            m_frmCCVoidItemList.lvItemList.Items.Remove(m_frmCCVoidItemList.lvItemList.Items(idx))
                            'm_CCItemList.Remove(m_Item)
                            'Set current item to what the user selected to remove the item form the array list
                            'send an UOD update message to inform it is cancelled.
                            'Status X is to cancel the item added to the UOD.
                            m_objCCItemInfo = m_Item
                            SetProductInfo("X")
                        End If
                        Exit For
                    End If
                Next
            Next
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in VoidItemInfo of CCSessionMgr. Exception is: " _
                                                             + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try

        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit VoidItemInfo of CCSessionMgr", Logger.LogLevel.INFO)

    End Sub
    ''' <summary>
    ''' Processes the scanned or handkeyed Product Code
    ''' </summary>
    ''' <param name="strBarcode">Scanned Item</param>
    ''' <param name="bIsManual">Manual or scanned</param>
    ''' <remarks></remarks>
    Private Sub ProcessScanItem(ByVal strBarcode As String, ByVal bIsManual As Boolean)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ProcessScanItem of CCSessionMgr", Logger.LogLevel.INFO)
        Try
            'Check if the entry is manual or scanned
            If bIsManual Then
                'DARWIN getting boots code from Clearance Label
                If strBarcode.StartsWith("8270") Then
                    strBarcode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode.Substring(4, 6))
                End If
                If (objAppContainer.objHelper.ValidateBootsCode(strBarcode)) Then
                    objAppContainer.objLogger.WriteAppLog("CCSessionMgr::ProcessScanItem:Boots code validated= " & strBarcode, Logger.LogLevel.RELEASE)
                    'strBarcode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                    m_objCCItemInfo = New GOItemInfo()
                    'Get the product info from the Data Access Layer
                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBarcode, m_objCCItemInfo)) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M11"), _
                        "Item Not Found", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                        MessageBoxDefaultButton.Button1)
                        Exit Sub
                    End If
                    'Fix for item not on file
                    If m_BCType = Nothing Then
                        If Not m_objCCItemInfo.BusinessCentreType = "" Then
                            m_BCType = m_objCCItemInfo.BusinessCentreType
                            m_SupplyRoute = m_objCCItemInfo.SupplyRoute
                        End If
                    End If
                    CCSessionMgr.GetInstance().DisplayCCScreen(CCSCREENS.ItemDetails)
                ElseIf (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                    objAppContainer.objLogger.WriteAppLog("CCSessionMgr::ProcessScanItem:Barcode validated= " & strBarcode, Logger.LogLevel.RELEASE)
                    'Removing the last digit from the Barcode since its used only for check digit validation
                    strBarcode = strBarcode.PadLeft(13, "0")
                    strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                    m_objCCItemInfo = New GOItemInfo()
                    'Get the product info from the Data Access Layer
                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_objCCItemInfo)) Then
#If NRF Then
					   'DARWIN checking if the Item code is a catch weight barcode or not
                        If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                            strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                            'DARWIN if Catch wt Barcode not in file check db/Controller using base barcode
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_objCCItemInfo)) Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M11"), _
                               "Item Not Found", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                                Exit Sub
                            End If
                        Else
#End If
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M11"), _
                            "Item Not Found", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                             MessageBoxDefaultButton.Button1)
                        Exit Sub
#If NRF Then
                        End If
#End If
                    End If
#If NRF Then
                    'DARWIN if item code not in DB and is a Catch wt Barcde then chk db/cntrllr using base barcode
                    If (m_objCCItemInfo.Description = "Unknown Item") Then
                        'DARWIN checking if the Item code is a catch weight barcode or not
                        If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then

                            'DARWIN CHANGE converting Price Barcode to Base Barcode as Catch wt Barcode not
                            'in controller
                            strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                            'DARWIN if Catch wt Barcode not in file check db/Controller using base barcode
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_objCCItemInfo)) Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M11"), _
                               "Item Not Found", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                                Exit Sub
                            End If
                        End If
                    End If
#End If
                    'Fix for item not on file
                    If m_BCType = Nothing Then
                        If Not m_objCCItemInfo.BusinessCentreType = "" Then
                            m_BCType = m_objCCItemInfo.BusinessCentreType
                            m_SupplyRoute = m_objCCItemInfo.SupplyRoute
                        End If
                    End If
                    CCSessionMgr.GetInstance().DisplayCCScreen(CCSCREENS.ItemDetails)
                Else
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M8"), _
                      "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                       MessageBoxDefaultButton.Button1)
                    Exit Sub
                End If
            Else
                'Check if the Scanned item has a valid EAN code
                If objAppContainer.objHelper.ValidateEAN(strBarcode) Then
                    objAppContainer.objLogger.WriteAppLog("CCSessionMgr::ProcessScanItem:Barcode validated= " & strBarcode, Logger.LogLevel.RELEASE)
                    ''Removing the last digit from the Barcode since its used only for check digit validation
                    strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                    m_objCCItemInfo = New GOItemInfo()
                    'Get the product info from the Data Access Layer
                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_objCCItemInfo)) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M11"), _
                        "Item Not Found", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                         MessageBoxDefaultButton.Button1)
                        Exit Sub
                    End If
                    'Fix for item not on file
                    If m_BCType = Nothing Then
                        If Not m_objCCItemInfo.BusinessCentreType = "" Then
                            m_BCType = m_objCCItemInfo.BusinessCentreType
                            m_SupplyRoute = m_objCCItemInfo.SupplyRoute
                        End If
                    End If
                    CCSessionMgr.GetInstance().DisplayCCScreen(CCSCREENS.ItemDetails)
                Else
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M9"), _
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                     MessageBoxDefaultButton.Button1)
                End If
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessScanItem of CCSessionMgr. Exception is: " _
                                                             + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit ProcessScanItem of CCSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Validates the quantity entered by the user
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateQuantity(ByVal strQuantity As String) As Boolean
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ValidateQuantity of CCSessionMgr", Logger.LogLevel.INFO)

        Dim lQuantity As Long
        lQuantity = CLng(strQuantity)
        Try
            'Validate if 0 quantity is entered
            If objAppContainer.objHelper.ValidateZeroQty(lQuantity) Then
                'Writing to Log INFO File while exit
                objAppContainer.objLogger.WriteAppLog("Exit ValidateQuantity of CCSessionMgr", Logger.LogLevel.INFO)
                Return True
            Else
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M10"), _
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                 MessageBoxDefaultButton.Button1)
                'Writing to Log INFO File while exit
                objAppContainer.objLogger.WriteAppLog("Exit ValidateQuantity of CCSessionMgr", Logger.LogLevel.INFO)
                Return False
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in ValidateQuantity of CCSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Clear the Form controls
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ClearData()
        With m_frmItemDetails
            .lblBootsCode.Text = ""
            .lblBusCentreDesc.Text = ""
            .lblEAN.Text = ""
            .lblItemDesc1.Text = ""
            .lblItemDesc2.Text = ""
            .lblItemDesc3.Text = ""
            .lblQuantityData.Text = "1"
        End With
    End Sub
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by CCSessionMgr
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EndSession(Optional ByVal bIsConnectFailed As Boolean = False)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered EndSessionof CCSessionMgr", Logger.LogLevel.INFO)
        Try
            'Save and data and perform all Exit Operations.
            'Close and Dispose all forms.
            m_frmCCScan.Close()
            m_frmCCScan.Dispose()
            m_frmItemDetails.Close()
            m_frmItemDetails.Dispose()
            m_frmCCVoidItemList.Close()
            m_frmCCVoidItemList.Dispose()
            m_frmCCSummary.Close()
            m_frmCCSummary.Dispose()
            'Release all objects and Set to nothing.
#If RF Then
            If bIsConnectFailed Then
                objAppContainer.objGlobalItemList = New ArrayList()
                objAppContainer.objGlobalItemList = m_CCItemList
                objAppContainer.eLastUsedModule = WorkflowMgr.GetInstance().WFIndex
            Else
                objAppContainer.eLastUsedModule = "None"
                objAppContainer.objGlobalItemList = Nothing
                m_CCItemList = Nothing
            End If
#ElseIf NRF Then
            m_CCItemList = Nothing
#End If
            'Closing the class object
            objCCSessionMgr = Nothing
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in EndSession of CCSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit EndSession of CCSessionMgr", Logger.LogLevel.INFO)

        Return True
    End Function
    Public Function ValidateNotOnFile() As Boolean
        If m_BCType = Nothing Then
            m_BCType = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DEFAULT_BUISNESSCENTRE_TYPE)
            m_SupplyRoute = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DEFAULT_SUPPLY_ROUTE)
        End If
        'Pilot support: User BC of Baby's if all item scanned are unknown items.
        For Each objItemInfo As GOItemInfo In m_CCItemList
            If objItemInfo.BusinessCentreType = "" Then
                objItemInfo.BusinessCentreType = m_BCType
                'objItemInfo.ShortDescription = m_BCDesc
                objItemInfo.SupplyRoute = m_SupplyRoute
            End If
        Next
        Return True
    End Function

    ''' <summary>
    ''' Enum Class that defines all screens for Shelf Monitor module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum CCSCREENS
        Scan
        ItemDetails
        ItemList
        Summary

    End Enum
End Class
