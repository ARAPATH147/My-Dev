Imports System.Text
'''***************************************************************
''' <FileName>ItemInfoSessionMgr.vb</FileName>
''' <summary>
''' The Item Info Container Class.
''' Implements all business logic and GUI navigation for Item Info.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>27-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
Public Class ItemInfoSessionMgr
    Private Shared m_IISessionMgr As ItemInfoSessionMgr
    Private m_IIhome As frmIIHome
    Private m_IIItemDetails As frmIIItemDetails
    Private m_IIProductInfo As ItemInfo
    Private m_IIItemList As ArrayList = Nothing
    Private m_arrDealDataList As ArrayList = Nothing
    Private m_InvokingModule As AppContainer.ACTIVEMODULE
    Private m_ModulePriceCheck As ModulePriceCheck
    'Private m_bIsFullPriceCheckRequired As Boolean
    Private m_PreviousItem As String
    'Private m_FullPriceCheckCount As Integer = 0
    Private m_strSEL As String
    Private m_bIsInvokingFormPlanner As Boolean = False
    Private m_bInvokedFromPlanner As Boolean = False
    Private m_QueuedSELList As ArrayList = Nothing
    Private objPSProductInfo As PSProductInfo = New PSProductInfo()
#If RF Then
    Private m_PostDetailsSent As Boolean = False
    Public Property PostDetailsSent() As Boolean
        Get
            Return m_PostDetailsSent
        End Get
        Set(ByVal value As Boolean)
            m_PostDetailsSent = value
        End Set
    End Property
#End If
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()

    End Sub
#If RF Then
    ''' <summary>
    ''' Updates the Status bar of all the forms in the session manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBarMessage()
        Try
            m_IIhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_IIItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception Occured, Trace : " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Gets the module details from which Item Details is being invoked
    ''' </summary>
    ''' <value></value>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property getItemInfoInvokingModule() As AppContainer.ACTIVEMODULE
        Get
            Return m_InvokingModule
        End Get
    End Property
#End If
    ''' <summary>
    ''' Initialises the Item Info Session when invoked from another module 
    ''' where the item info details screen needs to be directly displayed.
    ''' </summary>
    ''' <param name="objInvokeModule"></param>
    ''' <remarks></remarks>
    Public Function StartSession(ByVal objInvokeModule As AppContainer.ACTIVEMODULE, Optional ByVal bInvokedFromPlanner As Boolean = False) As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter Item Info Start session", Logger.LogLevel.INFO)
        Try
#If RF Then
            m_InvokingModule = objInvokeModule
            If Not objAppContainer.objExportDataManager.CreateINS() Then
                objAppContainer.objLogger.WriteAppLog("Cannot create INS Record in Item info", Logger.LogLevel.RELEASE)
                objAppContainer.objActiveModule = objInvokeModule
                Cursor.Current = Cursors.Default
                Return False
            End If
#End If

            'Do all Item Info related Initialisations here.
            m_IIhome = New frmIIHome()
            m_IIItemDetails = New frmIIItemDetails()

            m_IIItemList = New ArrayList()
            m_IIProductInfo = New ItemInfo()
            m_arrDealDataList = New ArrayList()
            m_QueuedSELList = New ArrayList()
#If NRF Then
            m_InvokingModule = objInvokeModule
#End If
            m_ModulePriceCheck = New ModulePriceCheck()
            'm_bIsFullPriceCheckRequired = False
            m_strSEL = ""
            m_PreviousItem = ""
            m_bInvokedFromPlanner = bInvokedFromPlanner
            Me.DisplayIIScreen(IISCREENS.Home)
            Return True
        Catch ex As Exception
            'TODO
            objAppContainer.objLogger.WriteAppLog("Exception occured at Item Info Start Session: " + ex.StackTrace, Logger.LogLevel.RELEASE)
#If RF Then
            objAppContainer.objActiveModule = objInvokeModule
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            ElseIf ex.Message = Macros.CONNECTION_REGAINED Then
                Cursor.Current = Cursors.Default
                Return False
            End If
#End If
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Item Info Start Session", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Initialises the Item Info Session when invoked from another module 
    ''' where the item info details screen needs to be directly displayed.
    ''' </summary>
    ''' <param name="objInvokeModule"></param>
    ''' <param name="strBootsCode"></param>
    ''' <remarks></remarks>
    Public Function StartSession(ByVal objInvokeModule As AppContainer.ACTIVEMODULE, ByVal strBootsCode As String, ByVal bIsInvokingFormPlanner As Boolean) As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter Item Info Start session", Logger.LogLevel.INFO)
        Try
#If RF Then
            If Not objAppContainer.objExportDataManager.CreateINS() Then
                objAppContainer.objLogger.WriteAppLog("Cannot create INS Record in Item info", Logger.LogLevel.RELEASE)
                Return False
            End If
#End If
            'Do all Item Info related Initialisations here.
            m_IIhome = New frmIIHome()
            m_IIItemDetails = New frmIIItemDetails()


            m_IIItemList = New ArrayList()
            m_IIProductInfo = New ItemInfo()
            m_arrDealDataList = New ArrayList()
            m_QueuedSELList = New ArrayList()
            m_InvokingModule = objInvokeModule
            m_ModulePriceCheck = New ModulePriceCheck()
            'm_bIsFullPriceCheckRequired = False
            m_strSEL = ""
            m_PreviousItem = ""
            m_bIsInvokingFormPlanner = bIsInvokingFormPlanner
            If (ProcessDataRetrieval(strBootsCode)) Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            ElseIf ex.Message = Macros.CONNECTION_REGAINED Then
                Cursor.Current = Cursors.Default
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured at Item Info Start Session: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Item Info Start Session", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by ItemInfoSessionMgr.
    ''' </summary>
    ''' <remarks></remarks>
#If NRF Then
    Public Sub EndSession()
#ElseIf RF Then
    Public Function EndSession(Optional ByVal isConnectivityLoss As Boolean = False) As Boolean
#End If
        objAppContainer.objLogger.WriteAppLog("Enter EX End Session", Logger.LogLevel.INFO)
        Try
#If NRF Then
             If m_QueuedSELList.Count > 0 Then
                WriteExportData()
            Else
                objAppContainer.objLogger.WriteAppLog("No Export data to be written", Logger.LogLevel.INFO)
            End If
#ElseIf RF Then
            If Not isConnectivityLoss Then
                If Not objAppContainer.objExportDataManager.CreateINX() Then
                    objAppContainer.objLogger.WriteAppLog("Cannot create INX record in Item Info End Session", Logger.LogLevel.RELEASE)
                    Return False
                End If
            End If
#End If
            'Save and data and perform all Exit Operations.
            'Close and Dispose all forms.
            'Integration testing
            objAppContainer.objActiveModule = m_InvokingModule

            If m_bInvokedFromPlanner Then
                m_IIItemDetails.Visible = False
                m_IIhome.Visible = False
                m_bInvokedFromPlanner = False
#If NRF Then
                Exit sub 
#ElseIf RF Then
                Exit Function
#End If
            End If
            m_IIItemDetails.Dispose()
            m_IIhome.Dispose()

            m_IIItemList = Nothing
            m_IIProductInfo = Nothing
            m_arrDealDataList = Nothing
            m_QueuedSELList = Nothing
            m_ModulePriceCheck = Nothing
            m_PreviousItem = Nothing
            'Release all objects and Set to nothing.
            m_IISessionMgr = Nothing
#If RF Then
            Return True
#End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at Item Info End Session", Logger.LogLevel.RELEASE)
#If RF Then
            Return False
#End If

        End Try
        objAppContainer.objLogger.WriteAppLog("Exit EX End Session", Logger.LogLevel.INFO)
#If RF Then
    End Function
#ElseIf NRF Then
    End Sub
#End If


    ''' <summary>
    ''' Functions for getting the object instance for the ItemInfoSessionMgr. 
    ''' Use this method to get the object reference for the Singleton ItemInfoSessionMgr.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of ItemInfoSessionMgr Class</remarks>
    Public Shared Function GetInstance() As ItemInfoSessionMgr
        If m_IISessionMgr Is Nothing Then
            m_IISessionMgr = New ItemInfoSessionMgr()
            'Set active module.
            objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.ITEMINFO
            Return m_IISessionMgr
        Else
            'Set active module.
            objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.ITEMINFO
            Return m_IISessionMgr
        End If
    End Function
    ''' <summary>
    ''' The Method handles scan the scan data returned form the barcode scanner.
    ''' This method implements the business logic to populate the data to the corresponding
    ''' UI element after validation.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="Type"></param>
    ''' <remarks></remarks>
    Public Sub HandleScanData(ByVal strBarcode As String, ByVal Type As BCType)
        objAppContainer.objLogger.WriteAppLog("Enter II HandleScanData", Logger.LogLevel.RELEASE)
        m_IIhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        'To display the wait cursor in order to inform user
        Threading.Thread.Sleep(1000)
        objPSProductInfo = New PSProductInfo()
        'Setting app Module back to the original module
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.ITEMINFO
        Try
            Select Case Type
                Case BCType.EAN
                    If Not (objAppContainer.objHelper.ValidateEAN(strBarcode)) Or _
                       Val(strBarcode) = 0 Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                              MessageBoxDefaultButton.Button1)
                        'Added for RF Mode -Lakshmi
                        m_IIhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        Return
                    Else
                        If Not (objAppContainer.objDataEngine.GetItemDetailsAllUsingPC(strBarcode, m_IIProductInfo)) Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                            'Added for RF Mode -Lakshmi
                            m_IIhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Exit Sub
                        End If
                        'System testing  - commented 
                        m_strSEL = ""
                        m_IIItemList.Add(m_IIProductInfo)
                    End If
                Case BCType.ManualEntry
                    Dim strBootsCode As String = ""
                    If strBarcode.Length < 8 Then
                        'Integration Testing
                        If objAppContainer.objHelper.ValidateBootsCode(strBarcode) Then
                            If Not (objAppContainer.objDataEngine.GetItemDetailsAllUsingBC(strBarcode, m_IIProductInfo)) Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                                m_IIhome.ProdSEL1.txtProduct.Text = ""
                                'Added for RF Mode -Lakshmi
                                m_IIhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                Exit Sub
                            End If
                            'System testing  - commented 
                            m_IIItemList.Add(m_IIProductInfo)
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                           MessageBoxButtons.OK, _
                           MessageBoxIcon.Asterisk, _
                           MessageBoxDefaultButton.Button1)
                            m_IIhome.ProdSEL1.txtProduct.Text = ""
                            'Added for RF Mode -Lakshmi
                            m_IIhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
                        End If
                    Else
                        If (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                            If Not (objAppContainer.objDataEngine.GetItemDetailsAllUsingPC(strBarcode, m_IIProductInfo)) Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                                m_IIhome.ProdSEL1.txtProduct.Text = ""
                                'Added for RF Mode -Lakshmi
                                m_IIhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                Exit Sub
                            End If
                            m_IIItemList.Add(m_IIProductInfo)
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                           MessageBoxButtons.OK, _
                                           MessageBoxIcon.Asterisk, _
                                           MessageBoxDefaultButton.Button1)
                            m_IIhome.ProdSEL1.txtProduct.Text = ""
                            'Added for RF Mode -Lakshmi
                            m_IIhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
                        End If
                    End If
                    'Reset SEL variable.
                    m_strSEL = ""
                Case BCType.SEL
                    If objAppContainer.objHelper.ValidateSEL(strBarcode) Then
                        Dim strBootsCode As String = ""
                        objAppContainer.objHelper.GetBootsCodeFromSEL(strBarcode, strBootsCode)
                        strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
                        If Not (objAppContainer.objDataEngine.GetItemDetailsAllUsingBC(strBootsCode, m_IIProductInfo)) Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                            'Added for RF Mode 
                            m_IIhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            'Ends
                            Exit Sub
                        Else
                            Dim strTemp As String
#If NRF Then
                            strTemp = m_ModulePriceCheck.DoPartialPriceCheck(m_IIProductInfo.ProductCode, strBarcode)
#ElseIf RF Then
                            strTemp = m_ModulePriceCheck.DoPartialPriceCheck(strBarcode, m_IIProductInfo.Price)
                            With objPSProductInfo
                                .BootsCode = m_IIProductInfo.BootsCode
                                .ProductCode = m_IIProductInfo.ProductCode
                                .Description = m_IIProductInfo.Description
                                .Status = m_IIProductInfo.Status
                                .CurrentPrice = CInt(m_IIProductInfo.Price)
                                .CIPFlag = m_IIProductInfo.CIPFlag
                                .Advantage = m_IIProductInfo.Advantage
                                .SupplyRoute = m_IIProductInfo.SupplyRoute
                            End With
#End If
                            If strTemp.Equals("0") Then
                                If objAppContainer.bMobilePrinterAttachedAtSignon Then
#If NRF Then
                                    objAppContainer.objDataEngine.GetProductInfoUsingPC(m_IIProductInfo.ProductCode, _
                                                                                        objPSProductInfo)
#ElseIf RF Then
                                    objAppContainer.objDataEngine.GetLabelDetails(objPSProductInfo)
#End If
                                    objPSProductInfo.LabelQuantity = 1
                                    MobilePrintSessionManager.GetInstance.CreateLabels(objPSProductInfo)
                                Else
#If NRF Then
                                    Dim objPRTData As PRTRecord = New PRTRecord()
                                    'objPRTData.strBootscode = (m_FFProductInfo.BootsCode).PadLeft(13, "0")
                                    objPRTData.strBootscode = m_IIProductInfo.BootsCode
                                    objPRTData.cIsMethod = Macros.PRINT_BATCH

                                    m_QueuedSELList.Add(objPRTData)
#ElseIf RF Then
                                    'Incase if in RF and mobile printer not attached send PRT request.
                                    objAppContainer.objExportDataManager.CreatePRT(m_IIProductInfo.BootsCode, _
                                                                                   SMTransactDataManager.ExFileType.EXData)
#End If
                                End If
                                'Changes for fixing Defect  BTCPR00004707
                                '#If RF Then
                                '                                'In RF Replacement SEL message is not shown in module price check.
                                '                                'This is because this has to be shown only after printing the SEL's
                                '                                'So , after printing the SEL using mobile printer or Sending PRT to the controller - showing the message in RF
                                '                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M16"), "Replace SEL", _
                                '                                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                '                                                MessageBoxDefaultButton.Button1)
                                '#End If
                                'Changes end for Defect  BTCPR00004707
                                'Clear the SEL value scanned.
                                m_strSEL = ""
                            End If
                            'Adds the obtained data
                            m_IIItemList.Add(m_IIProductInfo)
                        End If
                    Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M4"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                        'Added for RF Mode -Lakshmi
                        m_IIhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                    End If
            End Select
            'To update with item details in case of SEL not scanned.
            With objPSProductInfo
                .BootsCode = m_IIProductInfo.BootsCode
                .ProductCode = m_IIProductInfo.ProductCode
                .Description = m_IIProductInfo.Description
                .Status = m_IIProductInfo.Status
                .CurrentPrice = CInt(m_IIProductInfo.Price)
                .CIPFlag = m_IIProductInfo.CIPFlag
                .Advantage = m_IIProductInfo.Advantage
                .SupplyRoute = m_IIProductInfo.SupplyRoute
            End With
            'm_bIsFullPriceCheckRequired = False
            ItemInfoSessionMgr.GetInstance().DisplayIIScreen(ItemInfoSessionMgr.IISCREENS.ItemDetails)
            'Added for RF Mode -Lakshmi
            m_IIhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            'start to read barcodes.
            BCReader.GetInstance.StartRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at Item Info HandleScanData:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        Finally
#If RF Then
            If Not Me Is Nothing Then
                Me.ClearProductInfo()
            End If
            If Not m_IIhome Is Nothing Then
                If Not m_IIhome.ProdSEL1.txtProduct.IsDisposed AndAlso m_IIhome.ProdSEL1.txtSEL.IsDisposed Then
                m_IIhome.ProdSEL1.txtSEL.Text = ""
                m_IIhome.ProdSEL1.txtProduct.Text = ""
                End If
            End If
#End If
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit II HandleScanData", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' The method Displays the Item Info Home Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayIIScan(ByVal o As Object, ByVal e As EventArgs)
        With m_IIhome
            'Sets the store id and active data time to the status bar
            .ProdSEL1.txtProduct.Text = ""
            .ProdSEL1.txtSEL.Text = ""
            'Added for RF Mode -Lakshmi

            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

            'Ends
            .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' To print SELs when 'Print' button is pressed.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PrintProcess()
        If objAppContainer.bMobilePrinterAttachedAtSignon Then
#If NRF Then
            objAppContainer.objDataEngine.GetProductInfoUsingPC(m_IIProductInfo.ProductCode, _
                                                                objPSProductInfo)
#ElseIf RF Then
            'Used in case of Connection loss
            PostDetailsSent = True
            'Setting app Module back to the original module
            objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.ITEMINFO
            objAppContainer.objDataEngine.GetLabelDetails(objPSProductInfo)
#End If
            objPSProductInfo.LabelQuantity = 1
            MobilePrintSessionManager.GetInstance.CreateLabels(objPSProductInfo)
#If RF Then
            'Used in case of Connection loss
            PostDetailsSent = False
#End If
        Else
#If NRF Then
            Dim objPRTData As PRTRecord = New PRTRecord()
            'objPRTData.strBootscode = (m_FFProductInfo.BootsCode).PadLeft(13, "0")
            objPRTData.strBootscode = m_IIProductInfo.BootsCode
            objPRTData.cIsMethod = Macros.PRINT_BATCH

            m_QueuedSELList.Add(objPRTData)
#ElseIf RF Then
            'Used in case of Connection loss
            PostDetailsSent = True
            'Setting app Module back to the original module
            objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.ITEMINFO
            Dim objData As Object = Nothing
            'Incase if in RF and mobile printer not attached send PRT request.
            objAppContainer.objExportDataManager.CreatePRT(objPSProductInfo.BootsCode, _
                                                           SMTransactDataManager.ExFileType.EXData)
            DATAPOOL.getInstance.GetNextObject(objData)
            'Used in case of Connection loss
            PostDetailsSent = False
#End If
        End If
    End Sub
    ''' <summary>
    ''' Displays the Item Info Item Details Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayIIItemDetails(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Enter II DisplayIIItemDetails", Logger.LogLevel.RELEASE)
        Try
            Dim objDescriptionArray As ArrayList = New ArrayList()
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(m_IIProductInfo.Description)

            If m_IIProductInfo.RedemptionFlag = Macros.Redemption_True Then
                m_IIItemDetails.lblRedeemableText.Text = "Yes"
            Else
                m_IIItemDetails.lblRedeemableText.Text = "No"
            End If
            If m_IIProductInfo.TSF = "      " Then
                m_IIProductInfo.TSF = 0
            End If

            'Ends-Lakshmi
            Dim strCurrency As String = ""
            Dim strBarcode As String = ""
            strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(m_IIProductInfo.ProductCode)
            strCurrency = objAppContainer.objHelper.GetCurrency()
            With m_IIItemDetails
                .lblBootsCode.Text = objAppContainer.objHelper.FormatBarcode(m_IIProductInfo.BootsCode)
                .lblProductCode.Text = objAppContainer.objHelper.FormatBarcode(strBarcode)
                .lblProdDescription1.Text = objDescriptionArray.Item(0)
                .lblProdDescription2.Text = objDescriptionArray.Item(1)
                .lblProdDescription3.Text = objDescriptionArray.Item(2)
                .lblStatusText.Text = objAppContainer.objHelper.GetStatusDescription(m_IIProductInfo.Status)
#If RF Then
                If objAppContainer.OSSRStoreFlag = "Y" Then
                    If m_IIProductInfo.OSSRFlag = "O" Then
                        .lblOSSR.Text = "OSSR"
                    Else
                        .lblOSSR.Text = " "
                    End If
                Else
                    .btn_OSSRItem.Visible = False
                    .lblOSSR.Visible = False
                End If

                'System testing - DEF:54 - Added specific text to RF mode
                .lblStockFig.Text = "Total Stock File:"

                '.lblStockFig.Text = "Stock Figure:"
                 'Removed as part of SFA
                '.Btn_TSF1.Visible = True
#ElseIf NRF Then
                .btn_OSSRItem.Visible = False
                .btn_OSSRItem.Enabled = False
                .lblOSSR.Visible = False
                .lblOSSR.Enabled = False
                '.lblStockFig.Text = "Start of Day Stock Figure:"
                'Removed as part of SFA
                '.Btn_TSF1.Visible = False
                .PlannerNew1.Location = New System.Drawing.Point(90 * objAppContainer.iOffSet, 248 * objAppContainer.iOffSet)
#End If
                .lblPriceText.Text = objAppContainer.objHelper.FormatPrice(m_IIProductInfo.Price)
                .lblCurrencySymbol.Text = strCurrency
                'Stock File Accuracy  added TSF label
                If m_IIProductInfo.TSF.Substring(0, 1).Equals("-") Then
                    .lblStockText.ForeColor = Color.Red
                Else
                    .lblStockText.ForeColor = .lblStatusText.ForeColor
                End If
                ' end
                .lblStockText.Text = m_IIProductInfo.TSF
                'Check if any active deals available for the item and then call the function to display the deal.
                Displaydeal(.cmbDeal)
                'Sets the store id and active data time to the status bar
#If NRF Then
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#ElseIf RF Then
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
#End If
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured at II DispalyIIItemDetails:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit II DisplayIIItemDetails", Logger.LogLevel.RELEASE)
    End Sub

    ''' <summary>
    ''' Screen Display method for Item Info. 
    ''' All Item Info sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName">Enum IISCREENS</param>
    ''' <returns>True if display is success else False</returns>
    ''' <remarks></remarks>
    Public Function DisplayIIScreen(ByVal ScreenName As IISCREENS)
        objAppContainer.objLogger.WriteAppLog("Enter Item Info Display Screen", Logger.LogLevel.INFO)
        Try
            Select Case ScreenName
                Case IISCREENS.Home
                    m_IIhome.Invoke(New EventHandler(AddressOf DisplayIIScan))
                Case IISCREENS.ItemDetails
                    m_IIItemDetails.Invoke(New EventHandler(AddressOf DisplayIIItemDetails))
            End Select
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured Item Info Display Screen: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Item Info Display Screen", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Method retieves the data to be displayed in the item details screen
    ''' when called from another screen
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ProcessDataRetrieval(ByVal strProductCode As String) As Boolean
        Try
            If Not (objAppContainer.objDataEngine.GetItemDetailsAllUsingBC(strProductCode, m_IIProductInfo)) Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"))
                'Exit Function
                Return False
            End If
            m_IIItemList.Add(m_IIProductInfo)
            'System Testing
            ItemInfoSessionMgr.GetInstance().DisplayIIScreen(ItemInfoSessionMgr.IISCREENS.PlannerItemDetails)
            Return True
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured at II ProcessDataRetrieval:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    '''  Enum Class that defines all screens for Item Info module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum IISCREENS
        Home
        ItemDetails
        PlannerItemDetails
    End Enum
    ''' <summary>
    ''' enumerates all scannable fields in Item Info Module
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum SCNFIELDS
        ProductCode
        SELCode
    End Enum
    ''' <summary>
    ''' Method for populating deal info in the combo box.
    ''' </summary>
    ''' <param name="objDeal"></param>
    ''' <remarks></remarks>
    Public Sub Displaydeal(ByRef objDeal As ComboBox)
        Dim strDealNo As String
        Dim arrTemp() As String = Nothing
        objAppContainer.objLogger.WriteAppLog("Enter II DispalyDeal", Logger.LogLevel.RELEASE)
        Try
            strDealNo = m_IIProductInfo.DealList
            Dim arrDealDetails() As String = Nothing
            Dim objDealDetails As DQRRECORD
            'If there is no deal present then hide the drop down list control.
            If strDealNo.Equals("") Then
                objDeal.Visible = False
                With m_IIItemDetails
                    .lblDealHeader.Visible = False
                End With
            Else
                'Split the deals and add to an array.
                Dim iLen As Integer = 0
                arrDealDetails = strDealNo.Split(",")
                iLen = arrDealDetails.Length

                Dim iCounter As Integer = 0
                objDeal.Visible = True
                With m_IIItemDetails
                    .lblDealHeader.Visible = True
                End With
                objDeal.Enabled = False
                objDeal.Items.Clear()
                m_arrDealDataList.Clear()
                'Get the deal details for each deal the item is linked.
                For iCounter = 0 To iLen - 1
                    objDealDetails = New DQRRECORD()
                    If objAppContainer.objDataEngine.GetDealDetails(arrDealDetails(iCounter), objDealDetails) Then
                        Dim strTemp As String
                        strTemp = "DealMsg_" + objDealDetails.strDealType.PadLeft(2, "0")
                        objDeal.Items.Add(ConfigDataMgr.GetInstance.GetParam(strTemp))
                        m_arrDealDataList.Add(objDealDetails)
                    End If
                Next
                If objDeal.Items.Count() > 0 Then
                    objDeal.Items.Insert(0, "Select")
                    objDeal.Enabled = True
                    objDeal.SelectedIndex = 0
                ElseIf objDeal.Items.Count() = 0 Then
                    objDeal.Visible = False
                    m_IIItemDetails.lblDealHeader.Visible = False
                End If
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured Displaydeal. Exception is: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit II DispalyDeal", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Displays the deal information in the message for the deal selected
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DisplayDealInfo() As Boolean
        Dim arrDealData As ArrayList = New ArrayList()
        Dim objDealDetails As DQRRECORD = New DQRRECORD()
        objAppContainer.objLogger.WriteAppLog("Enter II DisplayDealInfo", Logger.LogLevel.RELEASE)
        Try
            With m_IIItemDetails
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
                If .cmbDeal.Enabled = True Then
                    If Not .cmbDeal.SelectedItem.Equals("Select") Then
                        If m_arrDealDataList.Count > 0 Then
                            objDealDetails = m_arrDealDataList.Item(.cmbDeal.SelectedIndex - 1)
                        Else
                            objDealDetails = m_arrDealDataList.Item(.cmbDeal.SelectedIndex)
                        End If

                        Dim strBuilder As StringBuilder = New StringBuilder()
                        Dim strDate As String
                        strBuilder.Append("This Item is on the following Active Promotion")
                        strBuilder.Append(Environment.NewLine + "---------------------------" + Environment.NewLine)
#If NRF Then
                        strBuilder.Append(.cmbDeal.SelectedItem.ToString())
#ElseIf RF Then
                        strBuilder.Append(objDealDetails.strDealDesc)
#End If
                        strDate = objDealDetails.strStartDate
                        strDate = strDate.Substring(6, 2) + "-" + strDate.Substring(4, 2) + "-" + strDate.Substring(0, 4)
                        strBuilder.Append(Environment.NewLine + "Start Date: " + strDate)
                        strDate = objDealDetails.strEndDate
                        strDate = strDate.Substring(6, 2) + "-" + strDate.Substring(4, 2) + "-" + strDate.Substring(0, 4)
                        strBuilder.Append(Environment.NewLine + "End Date: " + strDate)
                        strBuilder.Append(Environment.NewLine + "---------------------------" + Environment.NewLine)
                        strBuilder.Append("Check ALL Shelf Locations." + Environment.NewLine)
                        strBuilder.Append("Check Show Material.")
                        MessageBox.Show(strBuilder.ToString(), "Information")
                    End If
                End If

                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured DisplaydealInfo. Exception is: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit II DisplayDealInfo", Logger.LogLevel.RELEASE)
    End Function
#If NRF Then
    ''' <summary>
    ''' Writes the final set of data identified to the export data file
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteExportData() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter II WriteExportData", Logger.LogLevel.RELEASE)
        m_IIItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PRT")
        Try
            'If no price check records available write the PRTs between INS & INX
            If Not m_ModulePriceCheck.WriteExportData(m_QueuedSELList) Then
                'Fix for export data writing time consuming
                'Dim objSMDataManager As SMTransactDataManager = New SMTransactDataManager()
                'Write INS
                objAppContainer.objExportDataManager.CreateINS()
                'Write all PRTs
                For Each objPRT As PRTRecord In m_QueuedSELList
                    objAppContainer.objExportDataManager.CreatePRT(objPRT.strBootscode, SMTransactDataManager.ExFileType.EXData)
                Next
                'Write INX
                objAppContainer.objExportDataManager.CreateINX()
            End If
            'Clear price check module session
            m_ModulePriceCheck.EndSession()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at Item Info Write Export Data: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit II WriteExportData", Logger.LogLevel.RELEASE)
        Return True
    End Function

#End If
    ''' <summary>
    ''' Checks whether the module was invoked from planner
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsInvokedFromPlanner() As Boolean
        If m_bInvokedFromPlanner Or m_bIsInvokingFormPlanner Then
            Return True
        Else
            Return False
        End If

    End Function
#If RF Then
    Public Function AmendTSF(ByVal strBootsCode As String, ByVal strStockFigure As String) As Boolean
        Dim bTemp As Boolean = False
        Try
            'used in case of connection loss
            PostDetailsSent = True
            bTemp = objAppContainer.objExportDataManager.CreatePLC(strBootsCode, strStockFigure)
            PostDetailsSent = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error in Amend TSF function :: " + ex.Message)
        End Try
        Return bTemp
    End Function
#End If

    Public Sub ClearProductInfo()
        Try
            'm_IIProductInfo.BootsCode = ""
            m_IIProductInfo.DealDescription = ""
            m_IIProductInfo.DealList = ""
            m_IIProductInfo.Description = ""
            m_IIProductInfo.FirstBarcode = ""
            m_IIProductInfo.Price = ""
            m_IIProductInfo.ProductCode = ""
            m_IIProductInfo.RedemptionFlag = ""
            m_IIProductInfo.ShortDescription = ""
            m_IIProductInfo.Status = ""
            m_IIProductInfo.TSF = ""
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#Region "OSSR"
    'ambli
    'For OSSR
#If RF Then
    Public Sub DisplayOSSRToggle(ByRef lblToggleOSSR As Label, ByVal strBarcode As String)
        Dim bCurrentOSSR_FLAG As Boolean = False
        Dim bResponse As Boolean = False

        If lblToggleOSSR.Text = " " Then
            bCurrentOSSR_FLAG = False
        Else
            bCurrentOSSR_FLAG = True
        End If
        bResponse = objAppContainer.objExportDataManager.CreateENQ_ToggleOSSR(strBarcode, Not (bCurrentOSSR_FLAG))

        If (bResponse) Then
            If Not (bCurrentOSSR_FLAG) Then
                lblToggleOSSR.Text = "OSSR"
                MessageBox.Show("The location setting for this item has been changed to 'OSSR'.", "Info", _
                       MessageBoxButtons.OK, _
                       MessageBoxIcon.Asterisk, _
                       MessageBoxDefaultButton.Button1)

            Else
                lblToggleOSSR.Text = ""
                MessageBox.Show("The location setting for this item has been changed to 'Back Shop'.", "Info", _
                MessageBoxButtons.OK, _
                MessageBoxIcon.Asterisk, _
                MessageBoxDefaultButton.Button1)
            End If
        Else
            MessageBox.Show("Unable to Toggle OSSR Status.", "Info ", _
              MessageBoxButtons.OK, _
                MessageBoxIcon.Asterisk, _
                MessageBoxDefaultButton.Button1)
        End If
    End Sub
#End If
#End Region
End Class
''' <summary>
''' The value class for getting and managing Item Info Items.
''' </summary>
''' <remarks></remarks>
Public Class ItemInfo
    Inherits ProductInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_RedemptionFlag As String
    Private m_DealList As String
    Private m_DealDescription As String
    Public m_FirstBarcode As String
    Private BC_Type As String
    Private PG_Group As String
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub
    ''' <summary>
    ''' To store first barcode of  an item.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FirstBarcode() As String
        Get
            Return m_FirstBarcode
        End Get
        Set(ByVal value As String)
            m_FirstBarcode = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets List item status.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RedemptionFlag() As String
        Get
            Return m_RedemptionFlag
        End Get
        Set(ByVal value As String)
            m_RedemptionFlag = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets list of deals available for a product.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DealList() As String
        Get
            Return m_DealList
        End Get
        Set(ByVal value As String)
            m_DealList = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets list of deal list available for a product.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DealDescription() As String
        Get
            Return m_DealDescription
        End Get
        Set(ByVal value As String)
            m_DealDescription = value
        End Set
    End Property
    Public Property BCType() As String
        Get
            Return BC_Type
        End Get
        Set(ByVal value As String)
            BC_Type = value
        End Set
    End Property
    Public Property ProductGrp() As String
        Get
            Return PG_Group
        End Get
        Set(ByVal value As String)
            PG_Group = value
        End Set
    End Property
End Class