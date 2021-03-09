'''***************************************************************
''' <FileName>FFSessionMgr.vb</FileName>
''' <summary>
''' The Fast Fill Container Class.
''' Implements all business logic and GUI navigation for Fast Fill.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>27-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' Fix for transact restart issue in MCF.
''' </Summary>
'''****************************************************************************
Public Class FFSessionMgr
    Private m_FFhome As frmFFHome
    Private m_FFItemDetails As frmFFItemDetails
    Private m_FFSummary As frmFFSummary
    Private m_FFView As frmView
    Private Shared m_FFSessionMgr As FFSessionMgr = Nothing
    Private m_FFProductInfo As FFProductInfo = Nothing
    Private m_FFItemList As ArrayList = Nothing
    Private m_ItemCount As Integer = 0
    Private m_SELQueuedCount As Integer = 0
    Private m_SELCount As Integer = 0
    Private m_iScannedProdCount As Integer
    Public bIsAlreadyScanned As Boolean = False
    'Private m_PreviousItem As String
    'Private m_bIsFullPriceCheckRequired As Boolean
    Private m_QueuedSELList As ArrayList = Nothing
    'Private m_FullPriceCheckCount As Integer = 0
    Private m_strSEL As String
    Private m_ModulePriceCheck As ModulePriceCheck = Nothing
    Private m_bItemScanned As Boolean = False
    Private m_GapCount As Integer = 0
    Private isNewItem As Boolean = False
#If RF Then
    'Private strPreviousFillQty As String = "0"
    Private bCurrentOSSR_FLAG As Boolean = False
    Private bOSSR_Toggled As Boolean = False
    Public Property SequenceNumber() As Integer
        Get
            Return m_GapCount
        End Get
        Set(ByVal value As Integer)
            m_GapCount = value
        End Set
    End Property

    Public Property SELS() As Integer
        Get
            Return m_SELCount
        End Get
        Set(ByVal value As Integer)
            m_SELCount = value
        End Set
    End Property

    'Public Property PriceCheck() As Integer
    '    Get
    '        Return m_ModulePriceCheck.GetPCCountForCurrentSession()
    '    End Get
    '    Set(ByVal value As Integer)
    '        m_ModulePriceCheck.SetPCCOunt(value)
    '    End Set
    'End Property
#End If
    Public PreviousScreen As FFSCREENS
    'Private m_SMDataManager As SMTransactDataManager
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()

    End Sub
    ''' <summary>
    ''' Functions for getting the object instance for the FFSessionMgr. 
    ''' Use this method to get the object refernce for the Singleton FFSessionMgr.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of FFSessionMgr Class</remarks>
    Public Shared Function GetInstance() As FFSessionMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.FASTFILL
        If m_FFSessionMgr Is Nothing Then
            m_FFSessionMgr = New FFSessionMgr()
            Return m_FFSessionMgr
        Else
            Return m_FFSessionMgr
        End If
    End Function
#If RF Then
    ''' <summary>
    ''' Updates the Status bar of all the forms in the session manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBarMessage()
        Try
            m_FFhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_FFItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_FFSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_FFView.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception Occured, Trace : " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Public ReadOnly Property IsPostNext() As Boolean
        Get
            If (Not m_FFItemList Is Nothing) AndAlso (m_FFItemList.Count > 0) Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public ReadOnly Property IsFirstItemActioned() As Boolean
        Get
            If (m_FFItemList.Count >= 1) Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
#End If
    ''' <summary>
    ''' Initialises the Fast Fill Session 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function StartSession() As Boolean
        'Do all Fast Fill related Initialisations here.
        objAppContainer.objLogger.WriteAppLog("Enter FF Start session", Logger.LogLevel.INFO)
        Try
#If RF Then
            If Not (objAppContainer.objExportDataManager.CreateGAS()) Then
                objAppContainer.objLogger.WriteAppLog("Cannot Create GAS record at FF Start Session", _
                                                      Logger.LogLevel.RELEASE)
                Return False
            Else
                'Send a PGS message
                If Not (objAppContainer.objExportDataManager.CreatePGS()) Then
                    objAppContainer.objLogger.WriteAppLog("Cannot Create PGS record at SM Start Session", _
                                                          Logger.LogLevel.RELEASE)
                    Return False
                End If
            End If
#End If
            m_FFhome = New frmFFHome()
            m_FFItemDetails = New frmFFItemDetails()
            m_FFSummary = New frmFFSummary()
            m_FFView = New frmView()
            m_QueuedSELList = New ArrayList()
            'Support: Full Price Check Removed
            'm_bIsFullPriceCheckRequired = False
            ' m_PreviousItem = ""
            m_strSEL = ""
            m_FFProductInfo = New FFProductInfo()
            m_FFItemList = New ArrayList()
            m_ModulePriceCheck = New ModulePriceCheck()
            Me.DisplayFFScreen(FFSCREENS.Home)
            'm_SMDataManager = New SMTransactDataManager()
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at FF Start Session: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit FF Start Session", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by FFSessionMgr.
    ''' </summary>
    ''' <remarks></remarks>
#If NRF Then
    Public Sub EndSession()
#ElseIf RF Then
    Public Function EndSession(Optional ByVal isConnectivityLoss As Boolean = False) As Boolean
#End If
        objAppContainer.objLogger.WriteAppLog("Enter FF End Session", Logger.LogLevel.INFO)
        Try
#If NRF Then
            'Write Export Data for NRF mode
            If Not (WriteExportData()) Then
                objAppContainer.objLogger.WriteAppLog("No Export data to be written", Logger.LogLevel.INFO)
            End If
            'Set active module to none after quitting the module
            objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.NONE
#ElseIf RF Then
            'Send GAX only if Connectivity is not lost
            If Not isConnectivityLoss Then
                Dim objGAXRecord As GAXRecord = GenerateGAX()
                'Send PGX to end planner session
                If Not (objAppContainer.objExportDataManager.CreatePGX()) Then
                    objAppContainer.objLogger.WriteAppLog("Cannot Create PGX record at SM Start Session", _
                                                          Logger.LogLevel.RELEASE)
                    'v1.1 Commenting this for TRANSACT restart issue in MCF
                    'Return False
                End If
                If Not (objAppContainer.objExportDataManager.CreateGAX(objGAXRecord)) Then
                    objAppContainer.objLogger.WriteAppLog("Cannot Create GAX record at FF Start Session", Logger.LogLevel.RELEASE)
                    objGAXRecord = Nothing
                    'v1.1 Commenting this for TRANSACT restart issue in MCF
                    'Return False
                End If
                objGAXRecord = Nothing
            End If
#End If
            m_FFhome.Dispose()
            m_FFItemDetails.Dispose()
            m_FFView.Dispose()
            m_FFSummary.Dispose()
            m_FFProductInfo = Nothing
            m_FFItemList = Nothing
            m_QueuedSELList = Nothing
            m_FFSessionMgr = Nothing
            m_ModulePriceCheck = Nothing
            'System Testing
            m_ItemCount = Nothing
            m_bItemScanned = Nothing
            m_SELQueuedCount = Nothing
            m_SELCount = Nothing
            m_iScannedProdCount = Nothing
            bIsAlreadyScanned = Nothing
            m_strSEL = Nothing
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at FF End Session:" + ex.StackTrace + " Message : " + ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
            Return False
#End If
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit FF End Session", Logger.LogLevel.INFO)
#If RF Then
    End Function
#ElseIf NRF Then
    End Sub
#End If

    ''' <summary>
    ''' GenerateGAX record generates the GAX for the currently active session
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GenerateGAX() As GAXRecord
        Dim objGAXRecord As GAXRecord
        Try
            'Sent GAX record for RF mode
            objGAXRecord = New GAXRecord()
#If NRF Then
       objGAXRecord.strPickListItems = m_FFItemList.Count
#ElseIf RF Then
            objGAXRecord.strPickListItems = m_GapCount
#End If

            objGAXRecord.strSELS = m_SELQueuedCount
            objGAXRecord.strPriceChk = m_ModulePriceCheck.GetPCCountForCurrentSession()
            Return objGAXRecord
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " OCcured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return Nothing
        Finally
            objGAXRecord = Nothing
        End Try
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
        objAppContainer.objLogger.WriteAppLog("Enter FF HandleScanData", Logger.LogLevel.INFO)
        Dim objPSProductInfo As PSProductInfo = New PSProductInfo()
        ''''Bug HAs to be fixed
        'Setting app Module back to the original module
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.FASTFILL
        isNewItem = False
#If RF Then
        bOSSR_Toggled = False
        'Reset the previous quantity fields.
#End If
        m_FFProductInfo = New FFProductInfo()
        m_FFhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        Try

            Select Case Type
                Case BCType.EAN
                    If Not (objAppContainer.objHelper.ValidateEAN(strBarcode)) Or _
                       Val(strBarcode) = 0 Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        m_FFhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        DisplayFFScreen(FFSCREENS.Home)
                        Return
                    Else
                        strBarcode = strBarcode.PadLeft(13, "0")
                        strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                        ProcessBarcodeEntry(strBarcode, True)

                    End If
                Case BCType.ManualEntry
                    Dim strBootsCode As String = ""
                    If strBarcode.Length < 8 Then
                        'strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                        'TO be confirmed whether to consider Boots code entered at this point.
                        'strBarcode = strBarcode.Substring(0, 6)
                        'strBarcode = strBarcode.PadLeft(12, "0")
                        'Integration Testing
                        If objAppContainer.objHelper.ValidateBootsCode(strBarcode) Then
                            ProcessBarcodeEntry(strBarcode, False)
                            'System testing - commented
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                            m_FFhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            DisplayFFScreen(FFSCREENS.Home)
                            Return
                        End If
                    Else
                        If (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                            strBarcode = strBarcode.PadLeft(13, "0")
                            strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                            ProcessBarcodeEntry(strBarcode, True)
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                            m_FFhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            DisplayFFScreen(FFSCREENS.Home)
                            Return
                        End If
                    End If
                Case BCType.SEL
                    If objAppContainer.objHelper.ValidateSEL(strBarcode) Then
                        m_SELCount = m_SELCount + 1
                        Dim strBootsCode As String = ""
                        Dim bIsPriceCompareSuccess As Boolean = True
                        objAppContainer.objHelper.GetBootsCodeFromSEL(strBarcode, strBootsCode)
                        strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
                        If ProcessBarcodeEntry(strBootsCode, False) Then
                            'Do the Price check for FF module
                            Dim strTemp As String
                            'Price check is different for RF and NRF
#If NRF Then
                           strTemp = m_ModulePriceCheck.DoPartialPriceCheck(m_FFProductInfo.ProductCode, strBarcode)
#ElseIf RF Then
                            strTemp = m_ModulePriceCheck.DoPartialPriceCheck(strBarcode, m_FFProductInfo.Price)
#End If
                            If strTemp.Equals("0") Then
                                m_SELQueuedCount = m_SELQueuedCount + 1

                                If objAppContainer.bMobilePrinterAttachedAtSignon Then
#If NRF Then
                                    objAppContainer.objDataEngine.GetProductInfoUsingPC(m_FFProductInfo.ProductCode, _
                                                                                        objPSProductInfo)
#ElseIf RF Then
                                    With objPSProductInfo
                                        .BootsCode = m_FFProductInfo.BootsCode
                                        .ProductCode = m_FFProductInfo.ProductCode
                                        .Description = m_FFProductInfo.Description
                                        .Status = m_FFProductInfo.Status
                                        .CurrentPrice = m_FFProductInfo.Price
                                        .CIPFlag = m_FFProductInfo.CIPFlag
                                        .Advantage = m_FFProductInfo.Advantage
                                        .SupplyRoute = m_FFProductInfo.SupplyRoute
                                    End With
                                    objAppContainer.objDataEngine.GetLabelDetails(objPSProductInfo)
#End If
                                    objPSProductInfo.LabelQuantity = 1
                                    MobilePrintSessionManager.GetInstance.CreateLabels(objPSProductInfo)
                                Else
#If NRF Then
                                    Dim objPRTData As PRTRecord = New PRTRecord()
                                    'objPRTData.strBootscode = (m_FFProductInfo.BootsCode).PadLeft(13, "0")
                                    objPRTData.strBootscode = m_FFProductInfo.BootsCode
                                    objPRTData.cIsMethod = Macros.PRINT_BATCH

                                    m_QueuedSELList.Add(objPRTData)
#ElseIf RF Then
                                    'Incase if in RF and mobile printer not attached send PRT request.
                                    objAppContainer.objExportDataManager.CreatePRT(m_FFProductInfo.BootsCode, _
                                                                                   SMTransactDataManager.ExFileType.EXData)
#End If

                                End If
                                '#If RF Then
                                '                                'In RF Replacement SEL message is not shown in module price check.
                                '                                'This is because, the message has to be shown only after printing the SEL's
                                '                                'So , after printing the SEL using mobile printer or Sending PRT to the controller - showing the message in RF
                                '                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M16"), "Replace SEL", _
                                '                          MessageBoxButtons.OK, _
                                '                              MessageBoxIcon.Asterisk, _
                                '                              MessageBoxDefaultButton.Button1)
                                '#End If
                                'Clear the value stores in the SEL variable.
                                m_strSEL = ""

                            End If
                        End If
                    Else
                        MessageBox.Show(MessageManager.GetInstance.GetMessage("M4"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        m_FFhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        DisplayFFScreen(FFSCREENS.Home)
                        Return
                    End If
            End Select
            'Support Full Price Check Removed
            'm_bIsFullPriceCheckRequired = False
            'System testing - commented
            'm_FFhome.ProdSEL1.txtProduct.Text = ""
            'm_FFhome.ProdSEL1.txtSEL.Text = ""
            'Integration Testing - Flag to indicate that an item was scanned
            m_bItemScanned = True
            FFSessionMgr.GetInstance().DisplayFFScreen(FFSessionMgr.FFSCREENS.ItemDetails)
            m_FFhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at FF HandleScanData:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        Finally
            If Not m_FFhome Is Nothing Then
                If Not m_FFhome.ProdSEL1.txtSEL.IsDisposed AndAlso Not m_FFhome.ProdSEL1.txtProduct.IsDisposed Then
                    m_FFhome.ProdSEL1.txtSEL.Text = ""
                    m_FFhome.ProdSEL1.txtProduct.Text = ""
                End If


            End If
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit FF HandleScanData", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Processes the entry of barcode
    ''' </summary>
    ''' <param name="strBarCode"></param>
    ''' <param name="bIsProductCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ProcessBarcodeEntry(ByVal strBarCode As String, ByVal bIsProductCode As Boolean) As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter FF ProcessBarcodeEntry", Logger.LogLevel.RELEASE)
        Dim strTempCode As String = "000000000000"
        Dim arrPogList As ArrayList = New ArrayList
        bIsAlreadyScanned = False
        Dim bIsValid As Boolean = False
        'Checks if the barcode entered is product code or boots code
        'If the item is already scanned then take the data from the list
        'else take the data from database
        Try

            If bIsProductCode Then
                For Each objFFProductInfo As FFProductInfo In m_FFItemList
                    If (strBarCode = objFFProductInfo.ProductCode) Or (strBarCode = objFFProductInfo.FirstBarcode) Then
                        m_FFProductInfo = objFFProductInfo
                        bIsAlreadyScanned = True
                        'Fix for OSSR Toggle
#If RF Then
                        bOSSR_Toggled = True
#End If
                        Exit For
#If NRF Then
                    ElseIf strBarCode.StartsWith("2") Or strBarCode.StartsWith("02") Then
                        'to check if already scanned Catch Weight Barcode is scanned again
                        strTempCode = objAppContainer.objHelper.GetBaseBarcode(strBarCode)
                        If (strTempCode = objFFProductInfo.ProductCode) Or (strTempCode = objFFProductInfo.FirstBarcode) Then
                            m_FFProductInfo = objFFProductInfo
                            bIsAlreadyScanned = True
                            Exit For
                        End If
#End If	
                    End If
                Next
                If Not bIsAlreadyScanned Then
                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarCode, m_FFProductInfo)) Then
#If NRF Then
                        'DARWIN checking if the base barcode is present in Database/Conroller
                        If strBarCode.StartsWith("2") Or strBarCode.StartsWith("02") Then
                            'DARWIN converting database to Base Barcode
                            strBarCode = objAppContainer.objHelper.GetBaseBarcode(strBarCode)
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarCode, m_FFProductInfo)) Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                      MessageBoxButtons.OK, _
                                       MessageBoxIcon.Asterisk, _
                                       MessageBoxDefaultButton.Button1)
                                DisplayFFScreen(FFSCREENS.Home)
                                Return False
                            Else
                                GetMultiSiteLocations(m_FFProductInfo.ProductCode, arrPogList)
                                m_FFProductInfo.MultiSiteCount = arrPogList.Count
                                Return True
                            End If
                        Else
#End If	
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        DisplayFFScreen(FFSCREENS.Home)
                        Return False
#If NRF Then
                        End If
#End If	
                    Else
                        isNewItem = True
                        GetMultiSiteLocations(m_FFProductInfo.ProductCode, arrPogList)
                        m_FFProductInfo.MultiSiteCount = arrPogList.Count
                        'Sequence Number Fix
                        m_FFProductInfo.Sequence = (m_GapCount + 1).ToString().PadLeft(3, "0")
                        Return True
                    End If

                End If
            Else
                For Each objFFProductInfo As FFProductInfo In m_FFItemList
                    If objFFProductInfo.BootsCode.Equals(strBarCode) Then
                        m_FFProductInfo = objFFProductInfo
                        bIsAlreadyScanned = True
#If RF Then
                        bOSSR_Toggled = True
#End If
                        Exit For
                    End If
                Next
                If Not bIsAlreadyScanned Then
                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBarCode, m_FFProductInfo)) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        'System Testing
                        DisplayFFScreen(FFSCREENS.Home)
                        Return False
                    Else
                        isNewItem = True
                        GetMultiSiteLocations(m_FFProductInfo.ProductCode, arrPogList)
                        m_FFProductInfo.MultiSiteCount = arrPogList.Count
                        'Sequence Number Fix
                        m_FFProductInfo.Sequence = (m_GapCount + 1).ToString().PadLeft(3, "0")
                        'System Testing
                        Return True
                    End If
                End If
                Return True
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured at FF ProcessBarcodeEntry:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Enter FF ProcessBarcodeEntry", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Sets the View form to visible state
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetViewVisible()
        m_FFView.Visible = True
    End Sub
    ''' <summary>
    ''' The method Displays the Fast Fill Home Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayFFScan(ByVal o As Object, ByVal e As EventArgs)
        With m_FFhome
            .ProdSEL1.txtProduct.Text = ""
            .ProdSEL1.txtSEL.Text = ""
            'If m_bIsFullPriceCheckRequired And m_strSEL <> "" And m_strSEL <> Nothing Then
            '    .ProdSEL1.txtSEL.Text = m_strSEL
            'End If
            'Support : Full Price Check Removed

            If m_FFItemList.Count = 0 Then
                m_FFhome.btnView.Visible = False
            Else
                m_FFhome.btnView.Visible = True
            End If
            'System Testing
#If RF Then
            .lblItemVal.Text = SequenceNumber.ToString()
#ElseIf NRF Then
                .lblItemVal.Text = m_FFItemList.Count
#End If

            ' .lblItemVal.Text = m_iScannedProdCount
            'System Testing - SEL count should be incremented only if there is a PRT queued
            ' .lblSELVal.Text = m_SELCount
            .lblSELVal.Text = m_SELQueuedCount
            'Sets the store id and active data time to the status bar
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    '''  Displays the Fast Fill Item Details Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayFFItemDetails(ByVal o As Object, ByVal e As EventArgs)
        Try
            With m_FFItemDetails
                Dim strBarcode As String = ""
                '#If RF Then
                '                strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(m_FFProductInfo.ProductCode)
                '#ElseIf NRF Then
                strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(m_FFProductInfo.ProductCode)
                '#End If

                'TODO
                If m_FFItemList.Count > 0 Then
                    .btnView.Visible = True
                Else
                    .btnView.Visible = False
                End If

                Dim objDescriptionArray As ArrayList = New ArrayList()
                objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(m_FFProductInfo.Description)
                .lblBootsCode.Text = objAppContainer.objHelper.FormatBarcode(m_FFProductInfo.BootsCode)
                .lblProductCode.Text = objAppContainer.objHelper.FormatBarcode(strBarcode)
                .lblProdDescription1.Text = objDescriptionArray.Item(0)
                .lblProdDescription2.Text = objDescriptionArray.Item(1)
                .lblProdDescription3.Text = objDescriptionArray.Item(2)

                .lblStatusText.Text = objAppContainer.objHelper.GetStatusDescription(m_FFProductInfo.Status)

                'Stock File Accuracy  added TSF label
                If m_FFProductInfo.TSF.Substring(0, 1).Equals("-") Then
                    .lblStockText.ForeColor = Color.Red
                Else
                    .lblStockText.ForeColor = .lblStatusText.ForeColor
                End If
                .lblStockText.Text = m_FFProductInfo.TSF
                'IT Bug fix: The quantity entered cannot be negative. So commented the check
                'If m_FFProductInfo.strFillQuantiy < 0 Then
                'Added code to display blank in the label if quantity enterd is zero.
                If m_FFProductInfo.strFillQuantiy = 0 Then
                    'Defect 115 START
                    .lblFillQtyText.Text = "0"
                    'Defect 115 END
                Else
                    .lblFillQtyText.Text = m_FFProductInfo.strFillQuantiy
                End If
                'ambli
                'For OSSR
#If RF Then
                'If objAppContainer.OSSRStoreFlag = "Y" Then
                '    .btn_OSSRItem.Visible = True
                '    .lblOSSR.Visible = True
                'Else
                '    .btn_OSSRItem.Visible = False
                '    .lblOSSR.Visible = False
                'End If

                'System testing - DEF:54 - Added specific text to RF mode
                .lblSODStockFile.Text = "Total Stock File:"

                If objAppContainer.OSSRStoreFlag = "Y" Then
                    If m_FFItemDetails.btnView.Visible Then
                        .btn_OSSRItem.Location = New Point(112 * objAppContainer.iOffSet, 241 * objAppContainer.iOffSet)
                        .btnView.Location = New Point(59 * objAppContainer.iOffSet, 241 * objAppContainer.iOffSet)
                    Else
                        .btn_OSSRItem.Location = New Point(112 * objAppContainer.iOffSet, 241 * objAppContainer.iOffSet)
                    End If
                    If Not bOSSR_Toggled Then
                        If m_FFProductInfo.OSSRFlag = "O" Then
                            .lblOSSR.Text = "OSSR"
                        Else
                            .lblOSSR.Text = " "
                        End If
                    End If
                Else
                    .btnView.Location = New Point(88 * objAppContainer.iOffSet, 241 * objAppContainer.iOffSet)
                    .btn_OSSRItem.Visible = False
                    .lblOSSR.Visible = False
                End If
#End If
#If NRF Then
                .btnView.Location = New Point(88 * objAppContainer.iOffSet,241 * objAppContainer.iOffSet)
                .btn_OSSRItem.Visible = False
                .btn_OSSRItem.Enabled = False
                .lblOSSR.Visible = False
                .lblOSSR.Enabled = False
#End If
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at FF DisplayFFItemDetails:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#If RF Then
    Public Sub DisplayOSSRToggle(ByRef lblToggleOSSR As Label, ByVal strBarcode As String)
        bOSSR_Toggled = True
        'Dim objENQ As ENQRecord = New ENQRecord()

        Dim bResponse As Boolean = False

        If lblToggleOSSR.Text = "" Or lblToggleOSSR.Text = " " Then
            bCurrentOSSR_FLAG = False
        Else
            bCurrentOSSR_FLAG = True
        End If
        'objAppContainer.objExportDataManager.CreateENQ(objENQ)
        bResponse = objAppContainer.objExportDataManager.CreateENQ_ToggleOSSR(strBarcode, Not (bCurrentOSSR_FLAG))

        If (bResponse) Then
            If Not (bCurrentOSSR_FLAG) Then
                lblToggleOSSR.Text = "OSSR"
                MessageBox.Show("The location setting for this item has been changed to 'OSSR'.", "Info", _
                       MessageBoxButtons.OK, _
                       MessageBoxIcon.Asterisk, _
                       MessageBoxDefaultButton.Button1)

            Else
                lblToggleOSSR.Text = " "
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
    ''' <summary>
    ''' To Intialize the View Screen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub IntialiseViewScreen()
        objAppContainer.objLogger.WriteAppLog("Enter FF InitialiseViewScreen", Logger.LogLevel.RELEASE)
        With m_FFView
            .Help1.Visible = False
            .lblHeading.Text = "View Fast Fill List"
            .lstView.Columns.Add("Item", 70 * objAppContainer.iOffSet, HorizontalAlignment.Center)
            .lstView.Columns.Add("Item Description", 128 * objAppContainer.iOffSet, HorizontalAlignment.Center)
            .lstView.Columns.Add("MS", 30 * objAppContainer.iOffSet, HorizontalAlignment.Center)
            '.Visible = True
        End With
        objAppContainer.objLogger.WriteAppLog("Exit FF InitialiseViewScreen", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    '''  Displays the Fast Fill View Screen details
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayFFItemView(ByVal o As Object, ByVal e As EventArgs)
        Try
            With m_FFView
                .Text = "Fast Fill"
                .lstView.Clear()
                IntialiseViewScreen()
                PopulateFFViewList()
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                .Visible = True
                .Refresh()
            End With
            m_bItemScanned = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at FF DisplayFFItemView:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Subroutine for Populating List View
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateFFViewList()
        objAppContainer.objLogger.WriteAppLog("Enter FF PopulateFFViewList", Logger.LogLevel.RELEASE)
        Dim strTempMS As String = ""
        With m_FFView
            
            For Each objItemInfo As FFProductInfo In m_FFItemList
                'Integration Testing
                If Microsoft.VisualBasic.Val(objItemInfo.MultiSiteCount()) > 0 Then
                    strTempMS = objItemInfo.MultiSiteCount.ToString()
                Else
                    strTempMS = "1"
                End If

                .lstView.Items.Add( _
                    (New ListViewItem(New String() {objItemInfo.BootsCode, _
                                                    objItemInfo.ShortDescription, _
                                                    strTempMS})))
            Next
        End With
        objAppContainer.objLogger.WriteAppLog("Exit FF PopulateFFViewList", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Screen Display method for Fast Fill. 
    ''' All Fast Fill sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DisplayFFScreen(ByVal ScreenName As FFSCREENS)
        objAppContainer.objLogger.WriteAppLog("Enter FF Display Screen", Logger.LogLevel.INFO)
        Try
            Select Case ScreenName
                Case FFSCREENS.Home
                    PreviousScreen = FFSCREENS.Home
                    m_FFhome.Invoke(New EventHandler(AddressOf DisplayFFScan))
                Case FFSCREENS.ItemDetails
                    PreviousScreen = FFSCREENS.ItemDetails
                    m_FFItemDetails.Invoke(New EventHandler(AddressOf DisplayFFItemDetails))
                Case FFSCREENS.ItemView
                    If m_FFItemList.Count = 0 Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M53"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                    Else
                        m_FFSummary.Invoke(New EventHandler(AddressOf DisplayFFItemView))
                    End If
                Case FFSCREENS.FFSummary
                    m_FFSummary.Invoke(New EventHandler(AddressOf DisplayFFSummary))
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at Fast Fill Display Screen" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit FF Display Screen", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Displays the Fast Fill Summary Screen details
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayFFSummary(ByVal o As Object, ByVal e As EventArgs)

        With m_FFSummary
            .lblScndItmsVal.Text = m_iScannedProdCount.ToString()
#If NRF Then
  .lblPLItmsVal.Text = m_FFItemList.Count
#ElseIf RF Then
            .lblPLItmsVal.Text = m_GapCount
#End If

            .lblSELQdVal.Text = m_SELQueuedCount
            'IT External 888 Boots 1212
            'ambli
#If RF Then
            .lblUserMsg.Visible = True
            .lblActionDockTransmit.Visible = False
#ElseIf NRF Then
            If m_SELQueuedCount > 0 Then
                .lblUserMsg.Visible = True
                .lblActionDockTransmit.Visible = True
            Else
                .lblUserMsg.Visible = False
            End If
                 'The message Dock and Tramsmit need not be displayed if there
            'are no Picking List items

            If m_FFItemList.Count > 0 Then
                .lblActionDockTransmit.Visible = True
            Else
                .lblActionDockTransmit.Visible = False
            End If
#End If


       
            'Sets the store id and active data time to the status bar
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            .Visible = True
            .Refresh()
        End With

    End Sub
    ''' <summary>
    ''' Function for displaying Multisite Locations
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="arrPlannerList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetMultiSiteLocations(ByVal strProductCode As String, ByRef arrPlannerList As ArrayList) As Boolean
        objAppContainer.objDataEngine.GetPlannerListUsingPC(strProductCode, _
                                                            True, arrPlannerList)
    End Function
    ''' <summary>
    ''' Function for Updating Product Info. This method updates the item info to the 
    ''' internal array list for NRF world and for RF, it will send the GAP record to the 
    ''' TRANSACT
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateProductInfo() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter FF UpdateProductInfo", Logger.LogLevel.RELEASE)
        'Return Varaiable
#If RF Then
        Dim bTemp As Boolean = False
        'Set current quantity to the private variables.
        'strPreviousFillQty = m_FFProductInfo.strFillQuantiy
#End If
        Try
            Dim iCount As Integer
            If m_FFItemDetails.lblFillQtyText.Text.Trim().Equals("") Then
                Return False
            End If
            If Not objAppContainer.objHelper.ValidateZeroQty(CInt(m_FFItemDetails.lblFillQtyText.Text)) Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M1"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                'IT bug fix: The fill quantity label should display blank if zero is entered.
                'System testing - commented
                ' m_FFItemDetails.lblFillQtyText.Text = ""
                'DisplayFFScreen(FFSCREENS.ItemDetails)
                'm_FFProductInfo.strFillQuantiy = 0
                Return False
            Else
                m_FFProductInfo.strFillQuantiy = m_FFItemDetails.lblFillQtyText.Text
            End If

            Dim bIsPresentInList As Boolean = False
            For iCount = 0 To m_FFItemList.Count - 1
                Dim objFF As FFProductInfo = New FFProductInfo
                objFF = m_FFItemList.Item(iCount)
                If objFF.BootsCode.Equals(m_FFProductInfo.BootsCode) Then
                    m_FFItemList.RemoveAt(iCount)
                    m_FFItemList.Insert(iCount, m_FFProductInfo)
                    bIsPresentInList = True
                    Exit For
                End If
            Next
#If RF Then
            'RFSTAB
            'Add code to sent GAP message here.
            Dim objGapRecord As GAPRecord = New GAPRecord()
            objGapRecord.strNumberSEQ = m_FFProductInfo.Sequence
            objGapRecord.strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(m_FFProductInfo.ProductCode)
            objGapRecord.strBootscode = m_FFProductInfo.BootsCode
            ' objGapRecord.strCurrentQty = objFFProductInfo.strFillQuantiy
            objGapRecord.strCurrentQty = "0"
            objGapRecord.cIsGAPFlag = Macros.PLC_FF_FLAG
            ' objGapRecord.strFillQty = "0"
            objGapRecord.strStockFig = "0"
            objGapRecord.strUpdateOssrItem = " "
            objGapRecord.strLocCounted = "  "
            objGapRecord.strFillQty = m_FFProductInfo.strFillQuantiy
            'Check for fill quantity                                        'Fix 4416
            'Dim iQty1 As Integer = CInt(m_FFProductInfo.strFillQuantiy)     'Fix 4416
            'Dim iQty2 As Integer = CInt(strPreviousFillQty)                 'Fix 4416
            'If iQty1 > iQty2 Then                               'Fix 4416
            '    objGapRecord.strFillQty = iQty1 - iQty2         'Fix 4416
            'ElseIf iQty1 < iQty2 Then                           'Fix 4416
            '    objGapRecord.strFillQty = iQty2 - iQty1         'Fix 4416
            'Else
            '    If iQty1 = iQty2 And bIsAlreadyScanned = False Then 'Fix 4472
            '        objGapRecord.strFillQty = iQty1.ToString()      'Fix 4472
            '    Else                                                'Fix 4472
            '        objGapRecord.strFillQty = "0"                   'Fix 4472
            '    End If
            'End If                                              'Fix 4416

            If objAppContainer.objExportDataManager.CreateGAP(objGapRecord) Then
                'Wen The quantity is entered, Updaate only wen ack is recieved for the GAP record.
                bTemp = True
                If isNewItem Then
                    m_GapCount = m_GapCount + 1
                    isNewItem = False
                End If
            End If
#End If
            If Not bIsPresentInList Then
                m_FFItemList.Add(m_FFProductInfo)
            End If
            'Only if the item is scanned the Scanned count should increase
            If m_bItemScanned Then
                m_iScannedProdCount += 1
                m_bItemScanned = False
            End If
#If RF Then
            Return bTemp
#ElseIf NRF Then
            'Not changing the NRf code
            Return True
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at Fast Fill Update Product Info" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try

        objAppContainer.objLogger.WriteAppLog("Exit FF UpdateProductInfo", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Function to Select an item from the List View
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessItemSelection() As Boolean
        Dim bIsDataAvailable As Boolean = False
        Dim strBootsCode As String = ""
        Dim strProductCode As String = ""
        'Obtains the boots code corresponding to the selected data
        objAppContainer.objLogger.WriteAppLog("Enter FF ProcessItemSelection", Logger.LogLevel.RELEASE)
        With m_FFView
            Dim iCounter As Integer = 0
            If .lstView.SelectedIndices.Count > 0 Then
                For iCounter = 0 To .lstView.Items.Count - 1
                    If .lstView.Items(iCounter).Selected Then
                        bIsDataAvailable = True
                        'strBootsCode = .lstView.Items(iCounter).Text
                        m_FFProductInfo = m_FFItemList.Item(iCounter)
                        Exit For
                    End If
                Next
            End If
        End With
        'If data is available then obtains the Product code from the list
        If bIsDataAvailable Then

            'calls the Display method to display the FF item details screen
            DisplayFFScreen(FFSCREENS.ItemDetails)
            'ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.EXCSSTCK, strBootsCode, True)
        End If
        objAppContainer.objLogger.WriteAppLog("Exit FF ProcessItemSelection", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Processes the quit selection on the FF Home screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessFastFillQuit()
        Dim iResult As Integer = 0
        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M39"), "Confirmation", MessageBoxButtons.YesNo, _
                                  MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        If iResult = MsgBoxResult.Yes Then
            FFSessionMgr.GetInstance().DisplayFFScreen(FFSessionMgr.FFSCREENS.FFSummary)
        End If

    End Sub
    ''' <summary>
    ''' Processes the selection of Quit button in the FFItemDetails screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessItemDetailsQuit()
        'System Testing - Control brought back to home screen rather than summary screen.
        DisplayFFScreen(FFSCREENS.Home)
    End Sub
#If NRF Then

    ''' <summary>
    ''' Function to Write Export Data
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteExportData() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter Export Data Writing Session", Logger.LogLevel.INFO)
        Try
            'Integration Testing
            If m_FFItemList.Count > 0 Then
                'Fix for export data writing time consuming
                'Dim objSMDataManager As SMTransactDataManager = New SMTransactDataManager()

                Dim objGAPCount As Integer = 0
                Dim iCount As Integer = 0
                Dim objPRTRecord As PRTRecord
                m_FFSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing GAS")
                objAppContainer.objExportDataManager.CreateGAS()
                For Each objFFProductInfo As FFProductInfo In m_FFItemList
                    m_FFSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing GAP")
                    Dim objGapRecord As GAPRecord = New GAPRecord()
                    objGapRecord.strNumberSEQ = objGAPCount + 1
                    objGapRecord.strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(objFFProductInfo.ProductCode)
                    objGapRecord.strBootscode = objFFProductInfo.BootsCode
                    ' objGapRecord.strCurrentQty = objFFProductInfo.strFillQuantiy
                    objGapRecord.strCurrentQty = "0"
                    objGapRecord.cIsGAPFlag = Macros.PLC_FF_FLAG
                    ' objGapRecord.strFillQty = "0"
                    objGapRecord.strFillQty = objFFProductInfo.strFillQuantiy
                    objGapRecord.strStockFig = "0"
                    objGapRecord.strUpdateOssrItem = " "
                    objGapRecord.strLocCounted = "  "
                    objAppContainer.objExportDataManager.CreateGAP(objGapRecord)
                    'System Testing - increment the sequence number
                    objGAPCount += 1

                    'Write price check records.
                    'm_ModulePriceCheck.WriteExportData(objFFProductInfo.BootsCode)

                    'Write PRT records.
                    'Dim linQuery = From objPRT As PRTRecord In m_QueuedSELList _
                    '       Select objPRT Where objPRT.strBootscode = objGapRecord.strBootscode
                    'For Each objPRT As PRTRecord In linQuery
                    '    objSMDataManager.CreatePRT(objPRT, SMExportDataManager.ExFileType.EXData)
                    'Next
                    'Writing SEL Print Request Adjacent to the GAP Record.
                    'Fix for PRT record - Support Bug Fix
                    If m_QueuedSELList.Count <> 0 Then
                        While (iCount < m_QueuedSELList.Count)
                            objPRTRecord = m_QueuedSELList(iCount)
                            If (objPRTRecord.strBootscode = objGapRecord.strBootscode) Then
                                'Insert PRT recoreds where required
                                m_FFSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PRT")
                                'Dim PRTRecordData As PRTRecord = New PRTRecord()
                                ''System Testing
                                'PRTRecordData.strBootscode = objPRTRecord.strBootscode
                                'PRTRecordData.cIsMethod = Macros.PRINT_BATCH
                                objAppContainer.objExportDataManager.CreatePRT(objPRTRecord.strBootscode, SMTransactDataManager.ExFileType.EXData)
                                m_QueuedSELList.RemoveAt(iCount)
                                'PRTRecordData = Nothing
                            Else
                                iCount = iCount + 1
                            End If
                        End While
                    End If
                    'reset counter value
                    iCount = 0
                Next
                'Fix for PRT record - Support Bug Fix
                If (m_QueuedSELList.Count <> 0) Then
                    'For SELs without Corresponding SM Session
                    For Each objPRTRecord In m_QueuedSELList
                        'Insert PRT recoreds where required
                        m_FFSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PRT")
                        'Dim PRTRecordData As PRTRecord = New PRTRecord()
                        ''System Testing
                        'PRTRecordData.strBootscode = objPRTRecord.strBootscode
                        'PRTRecordData.cIsMethod = Macros.PRINT_BATCH
                        objAppContainer.objExportDataManager.CreatePRT(objPRTRecord.strBootscode, _
                                                                       SMTransactDataManager.ExFileType.EXData)
                    Next
                End If

                m_FFSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing GAX")
                Dim objGAXRecord As GAXRecord = New GAXRecord()
                objGAXRecord.strPickListItems = m_FFItemList.Count
                objGAXRecord.strSELS = m_SELQueuedCount
                objGAXRecord.strPriceChk = m_ModulePriceCheck.GetPCCountForCurrentSession()
                objAppContainer.objExportDataManager.CreateGAX(objGAXRecord)
            ElseIf ((m_FFItemList.Count = 0) And (m_QueuedSELList.Count <> 0)) Then
                m_FFSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing INS")
                objAppContainer.objExportDataManager.CreateINS()

                For Each objPRTRecord As PRTRecord In m_QueuedSELList
                    'Insert PRT recoreds as required
                    m_FFSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PRT")
                    'Dim PRTRecordData As PRTRecord = New PRTRecord()
                    ''System Testing
                    'PRTRecordData.strBootscode = objPRTRecord.strBootscode
                    'PRTRecordData.cIsMethod = Macros.PRINT_BATCH
                    objAppContainer.objExportDataManager.CreatePRT(objPRTRecord.strBootscode, _
                                                                   SMTransactDataManager.ExFileType.EXData)
                Next
                objAppContainer.objExportDataManager.CreateINX()
                m_FFSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing INX")
            End If
            'If m_QueuedSELList.Count > 0 Then

            '    m_FFSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PRT")
            '    Dim objSMDataManager As SMExportDataManager = New SMExportDataManager()
            '    For Each objPRT As PRTRecord In m_QueuedSELList

            '        objSMDataManager.CreatePRT(objPRT, SMExportDataManager.ExFileType.EXData)
            '    Next

            'End If

            'Clear price check module session.
            m_ModulePriceCheck.EndSession()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at Export Data Writing: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Export Data Session", Logger.LogLevel.INFO)
        Return True
    End Function
#End If
    ''' <summary>
    ''' Enumerates all scannable fields in Excess stock Module
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum SCNFIELDS
        ProductCode
        SELCode
    End Enum
    ''' <summary>
    ''' Enum Class that defines all screens for Fast Fill module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum FFSCREENS
        Home
        ItemDetails
        ItemView
        FFSummary
    End Enum
End Class
''' <summary>
''' The value class for getting and managing Fast Fill.
''' </summary>
''' <remarks></remarks>
Public Class FFProductInfo
    Inherits ProductInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_FirstBarcode As String
    'To set Second Barcode
    Private m_SecondBarcode As String
    Public imultiSiteCount As Integer
    Public strFillQuantiy As String



    ''' <summary>
    ''' Gets or sets multisitecount for a product
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MultiSiteCount() As Integer
        Get
            Return imultiSiteCount
        End Get
        Set(ByVal value As Integer)
            imultiSiteCount = value
        End Set
    End Property
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
    ''' To store first barcode of  an item.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SecondBarcode() As String
        Get
            Return m_SecondBarcode
        End Get
        Set(ByVal value As String)
            m_SecondBarcode = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets FillQuantity for a product
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FillQuantity() As String
        Get
            Return strFillQuantiy
        End Get
        Set(ByVal value As String)
            strFillQuantiy = value
        End Set
    End Property
End Class