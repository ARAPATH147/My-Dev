''' ***************************************************************************
''' <fileName>PSSessionMgr.vb</fileName>
''' <summary>The Print SEL Container Class.
''' Implements all business logic and GUI navigation for Print SEL. 
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''*****************************************************************************
''' * Modification Log
''' ********************************************************************
''' * 1.1   Archana Chandramathi    13 C Chilled Food Changes
''' Remove the message "Please make sure that wider stationery is used 
''' for printing labels for food items" from the device
''' ********************************************************************/
Public Class CLRSessionMgr
    Private m_PCLhome As frmPCLHome
    Private m_PCLItemDetails As frmPCLItemDetails
    Private m_PSSummary As frmPSSummary
    Private m_PCLHelp As frmPCLHelp   'Added for mobile printing.********** Govindh

    Private Shared m_CLRSessionMgr As CLRSessionMgr = Nothing
    Private m_PCLProductInfo As PSProductInfo = Nothing
    Private m_PCLItemList As ArrayList = Nothing
    Private iNumScanned As Integer = 0
    Private iNumSELQueued As Integer = 0
    Private arrItemDescr As ArrayList = New ArrayList()
    Private objArrayExportData As ArrayList = Nothing
    'Variable added to implement clearance label.Added for mobile printing.********** Govindh
    Private strCurrency As String = ""
    Private strSmallCurrency As String = ""
    Private bFirstPrint As Boolean = True
    Private m_CurrencySymbol As String
    Private m_LabelType As MobilePrintSessionManager.LabelType
    Private m_MobilePrinterStatus As Boolean = False
    Private bIsPrinted As Boolean = False
    Private Sub New()
        'get the currency symbol applicable for the store.
        m_CurrencySymbol = ConfigDataMgr.GetInstance().GetParam("ValidCurrency").ToString()
    End Sub
    ''' <summary>
    ''' Property to read and hold the currency symbol for the store.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property CurrencySymbol() As String
        Get
            Return m_CurrencySymbol
        End Get
    End Property
    ''' <summary>
    ''' Functions for getting the object instance for the PSSessionMgr. 
    ''' Use this method to get the object refernce for the Singleton PSSessionMgr.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of PSSessionMgr Class</remarks>
    Public Shared Function GetInstance() As CLRSessionMgr
        'set the active module name.
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PRTCLEARANCE
        If m_CLRSessionMgr Is Nothing Then
            m_CLRSessionMgr = New CLRSessionMgr()
            Return m_CLRSessionMgr
        Else
            Return m_CLRSessionMgr
        End If
    End Function
    ''' <summary>
    ''' Initialises the Print SEL Session 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function StartSession() As Boolean
        Try
#If RF Then
            'Send INS transaction if in RF mode
            If objAppContainer.objExportDataManager.CreateINS() Then
#End If
                'Do all print SEL realated Initialisations here.
                m_PCLhome = New frmPCLHome()
                m_PCLItemDetails = New frmPCLItemDetails()
                'm_PSSummary = New frmPSSummary()
                m_PCLProductInfo = New PSProductInfo()
                ''''Bug HAs to be fixed
                'Setting app Module back to the original module
                objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PRTCLEARANCE
                'Added for print clearance label function
                m_PCLHelp = New frmPCLHelp()
                m_PCLItemList = New ArrayList()

                'Check if mobile printer is connected.
                If ConfigDataMgr.GetInstance().GetParam("ValidCurrency").ToString().Equals("S") Then
                    strCurrency = Macros.POUND_SYMBOL.ToString()
                    strSmallCurrency = Macros.PENCE_SYMBOL.ToString()
                ElseIf ConfigDataMgr.GetInstance().GetParam("ValidCurrency").ToString().Equals("E") Then
                    strCurrency = Macros.EURO_SYMBOL.ToString()
                    strSmallCurrency = Macros.CENTS_SYMBOL.ToString()
                End If
                'Set label type
                m_LabelType = MobilePrintSessionManager.LabelType.RCLR
                m_PCLItemDetails.lblClearancePrice.Text = "0.00"
                'Set first print flag to true.
                bFirstPrint = True
                Return True
#If RF Then
            Else
                Return False
            End If
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in PrintSEL:Print SEL Session cannot be started" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PSSessionMgr StartSession", Logger.LogLevel.RELEASE)
    End Function
#If RF Then
    ''' <summary>
    ''' Updates the Status bar of all the forms in the session manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBarMessage()
        Try
            m_PCLhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_PCLItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            'Never used Screen
            'm_PSSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_PCLHelp.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception Occured, Trace : " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End If
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by PSSessionMgr.
    ''' </summary>
    ''' <returns>True if terminate is sucess else False</returns>
    ''' <remarks></remarks>
#If NRF Then
    Public Function EndSession() As Boolean
#ElseIf RF Then
    Public Function EndSession(Optional ByVal isConnectivityLost As Boolean = False) As Boolean
#End If
        'Save and data and perform all Exit Operations.
        'Close and Dispose all forms.
        Try
#If RF Then
            If Not isConnectivityLost Then
                'Send INX message in case of RF mode.
                If Not objAppContainer.objExportDataManager.CreateINX() Then
                    Return False
                End If
            End If
#End If
            'Disable the decoder to handle only SELs
            BCReader.GetInstance().DisableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
            'write export data for batch mode operation.
            m_PCLhome.Dispose()
            m_PCLItemDetails.Dispose()
            m_PCLHelp.Dispose()
            'Release all objects and Set to nothig.
            m_PCLProductInfo = Nothing
            m_CLRSessionMgr = Nothing
            'IT Fixes
            ' m_QueuedSELList = Nothing
            objArrayExportData = Nothing
            ' m_PreviousItem = Nothing
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in PrintSEL:Print SEL EndSession failure" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
        objAppContainer.objLogger.WriteAppLog("Exit PSSessionMgr EndSession", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' The Method handles scan the scane data returned form the barcode scanner.
    ''' This method implements the business logic to populate the data to the corresponding
    ''' UI element after validation.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="Type"></param>
    ''' <remarks></remarks>
    Public Sub HandleScanData(ByVal strBarcode As String, ByVal Type As BCType)
        objAppContainer.objLogger.WriteAppLog("Enter PSSessionMgr HandleScanData", Logger.LogLevel.RELEASE)
        m_PCLhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        Dim strBaseCode As String = ""
        Dim strPrice As String = ""
        Dim enLabelType As String = Nothing
        Try
            'reset the value class so that the data is stored properly for the next item scanned.
            m_PCLProductInfo = New PSProductInfo()
            'By default set to clearance label. set it to catch weight clearance only
            'when a corresponding line item is scanned or clearance label is scanned.
            'm_PCLProductInfo.SELLabelType = Macros.SEL_CLEARANCE
            enLabelType = Macros.SEL_CLEARANCE
            ''''Bug HAs to be fixed
            'Setting app Module back to the original module
            objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PRTCLEARANCE
            Select Case Type
                Case BCType.EAN
                    If Not (objAppContainer.objHelper.ValidateEAN(strBarcode)) Or _
                       Val(strBarcode) = 0 Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        m_PCLhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        Return
                    Else
                        strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                        If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_PCLProductInfo)) Then
#If NRF Then
                            'create the base barcode for the item scanned.
                            If strBarcode.StartsWith("02") Or strBarcode.StartsWith("2") Then
                                objAppContainer.objHelper.GetBaseBarcodeWithPrice(strBarcode, strPrice, strBaseCode)
                                If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_PCLProductInfo)) Then
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                                    MessageBoxButtons.OK, _
                                                    MessageBoxIcon.Asterisk, _
                                                    MessageBoxDefaultButton.Button1)
                                    m_PCLhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                    Return
                                End If
                                'Update the price from barcode to the oject.
                                m_PCLProductInfo.CurrentPrice = Val(strPrice)
                                m_PCLProductInfo.OriginalPrice = Val(strPrice)
                                'm_PCLProductInfo.SELLabelType = Macros.SEL_CATCH_WEIGHT_CLEARANCE
                                enLabelType = Macros.SEL_CATCH_WEIGHT_CLEARANCE
                            Else
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                                MessageBoxButtons.OK, _
                                                MessageBoxIcon.Asterisk, _
                                                MessageBoxDefaultButton.Button1)
                                m_PCLhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                Return
                            End If
#End If
#If RF Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                                MessageBoxButtons.OK, _
                                                MessageBoxIcon.Asterisk, _
                                                MessageBoxDefaultButton.Button1)
                            m_PCLhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
#End If
                        End If
#If RF Then
                        'Check the product code in case of rf and set the label type
                        If strBarcode.StartsWith("02") Or strBarcode.StartsWith("2") Then
                            'm_PCLProductInfo.SELLabelType = Macros.SEL_CATCH_WEIGHT_CLEARANCE
                            enLabelType = Macros.SEL_CATCH_WEIGHT_CLEARANCE
                        End If
#End If
                    End If
                Case BCType.ManualEntry
                    Dim strBootsCode As String = ""
                    If strBarcode.Length < 8 Then
                        'strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                        If Not (objAppContainer.objHelper.ValidateBootsCode(strBarcode)) Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                            m_PCLhome.ProdSEL1.txtProduct.Text = ""
                            m_PCLhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
                        Else
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBarcode, m_PCLProductInfo)) Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                                m_PCLhome.ProdSEL1.txtProduct.Text = ""
                                m_PCLhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                Return
                            End If
                        End If
                    Else
                        If (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                            strBarcode = strBarcode.PadLeft(13, "0")
                            strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_PCLProductInfo)) Then
#If NRF Then
                                'create the base barcode for the item scanned.
                                If strBarcode.StartsWith("02") Or strBarcode.StartsWith("2") Then
                                    objAppContainer.objHelper.GetBaseBarcodeWithPrice(strBarcode, strPrice, strBaseCode)
                                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_PCLProductInfo)) Then
                                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                                        MessageBoxButtons.OK, _
                                                        MessageBoxIcon.Asterisk, _
                                                        MessageBoxDefaultButton.Button1)
                                        m_PCLhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                        Return
                                    End If
                                    'Update the price from barcode to the oject.
                                    m_PCLProductInfo.CurrentPrice = Val(strPrice)
                                    m_PCLProductInfo.OriginalPrice = Val(strPrice)
                                    'm_PCLProductInfo.SELLabelType = Macros.SEL_CATCH_WEIGHT_CLEARANCE
                                    enLabelType = Macros.SEL_CATCH_WEIGHT_CLEARANCE
                                Else
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                                    MessageBoxButtons.OK, _
                                                    MessageBoxIcon.Asterisk, _
                                                    MessageBoxDefaultButton.Button1)
                                    m_PCLhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                    Return
                                End If
#End If
#If RF Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                                    MessageBoxButtons.OK, _
                                                    MessageBoxIcon.Asterisk, _
                                                    MessageBoxDefaultButton.Button1)
                                m_PCLhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                Return
#End If
                            End If
#If RF Then
                            'Check the product code in case of rf and set the label type
                            If strBarcode.StartsWith("02") Or strBarcode.StartsWith("2") Then
                                'm_PCLProductInfo.SELLabelType = Macros.SEL_CATCH_WEIGHT_CLEARANCE
                                enLabelType = Macros.SEL_CATCH_WEIGHT_CLEARANCE
                            End If
#End If
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                           MessageBoxButtons.OK, _
                           MessageBoxIcon.Asterisk, _
                           MessageBoxDefaultButton.Button1)
                            m_PCLhome.ProdSEL1.txtProduct.Text = ""
                            m_PCLhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
                        End If
                    End If
                Case BCType.SEL
                    Dim strBootsCode As String = ""
                    If objAppContainer.objHelper.ValidateSEL(strBarcode) Then
                        objAppContainer.objHelper.GetBootsCodeFromSEL(strBarcode, strBootsCode)
                        strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
                        If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBootsCode, m_PCLProductInfo)) Then
                            objAppContainer.objLogger.WriteAppLog("Print Clearance cannot obtain Boots code from SEL", Logger.LogLevel.DEBUG)
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                            m_PCLhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Exit Sub
                        End If
                    Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M4"), "Error", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                        m_PCLhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        Return
                    End If
                Case BCType.CLRLBL
                    'Check if the label scanned is clearance label.
                    Dim strBootsCode As String = ""
                    Dim strClrPrice As String = ""
                    Dim strOrigPrice As String = ""
                    objAppContainer.objHelper.ParseClearanceBarcode(strBarcode, strBootsCode, strClrPrice, strOrigPrice)
                    strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBootsCode, m_PCLProductInfo)) Then
                        objAppContainer.objLogger.WriteAppLog("Print Clearance cannot obtain Boots code from clearance label.", _
                                                              Logger.LogLevel.DEBUG)
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
                        m_PCLhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        Exit Sub
                    End If
                    'Update the original price and clearance price of the item.
                    m_PCLProductInfo.CurrentPrice = strClrPrice
                    m_PCLProductInfo.OriginalPrice = strOrigPrice
                    If strBarcode.Length = 22 Then
                        'Set label type as catch weight clearance label.
                        'm_PCLProductInfo.SELLabelType = Macros.SEL_CATCH_WEIGHT_CLEARANCE
                        enLabelType = Macros.SEL_CATCH_WEIGHT_CLEARANCE
                    End If
            End Select
            'Set item printed to false
            bIsPrinted = False
            'm_bIsFullPriceCheckRequired = False
            UpdateNumItemsScanned()

            'set the label type in the message to clearance label.
            m_PCLProductInfo.SELLabelType = enLabelType
            'UT Changes
            CLRSessionMgr.GetInstance().DisplayPCLScreen(PCLSCREENS.ItemDetails)

            'if in RF mode get the SEL print details by sending PRT.
#If RF Then
            'Send PRT message and get LPR transacitons if mobile printer attached.
            'send the request and get the detials required to print label.
            objAppContainer.objDataEngine.GetLabelDetails(m_PCLProductInfo)
#End If
            'set the label type in the message to clearance label.
            m_PCLProductInfo.SELLabelType = enLabelType
            'UAT commented as array need not be maintained
            Dim strProductCode As String = ""
            Dim iIndexCount As Integer = 0
            Dim iCount As Integer = 0
            'System Testing
            BCReader.GetInstance.StartRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in PrintSEL:Print SEL EndSession failure" + ex.StackTrace, Logger.LogLevel.RELEASE)
        Finally
            If Not m_PCLhome Is Nothing Then
                If Not m_PCLhome.ProdSEL1.txtProduct.IsDisposed AndAlso Not m_PCLhome.ProdSEL1.txtSEL.IsDisposed Then
                    m_PCLhome.ProdSEL1.txtProduct.Text = ""
                    m_PCLhome.ProdSEL1.txtSEL.Text = ""
                End If

            End If
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PSSessionMgr HandleScanData", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Screen Display method for Print SEL. 
    ''' All print SEL sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName">Enum PSSCREENS</param>
    ''' <returns>True if display is success else False</returns>
    ''' <remarks></remarks>
    Public Function DisplayPCLScreen(ByVal ScreenName As PCLSCREENS)
        'Invoke method for other PS screens here.
        BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
        Try
            Select Case ScreenName
                Case PCLSCREENS.Home
                    m_PCLhome.Invoke(New EventHandler(AddressOf DisplayPCLScan))
                Case PCLSCREENS.ItemDetails
                    m_PCLItemDetails.Invoke(New EventHandler(AddressOf DisplayItemDetails))
                Case PCLSCREENS.PCLHelp
                    m_PCLHelp.Invoke(New EventHandler(AddressOf DisplayPCLHelp))
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at Print SEL Display Screen" + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' Display Print Clearance label help screen.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayPCLHelp(ByVal o As Object, ByVal e As EventArgs)
        With m_PCLHelp
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' To display the scan screen.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayPCLScan(ByVal o As Object, ByVal e As EventArgs)
        Try
            With m_PCLhome
                .ProdSEL1.lblSEL.Text = "SEL/Clearance Label"
                .ProdSEL1.txtProduct.Text = ""
                .ProdSEL1.txtSEL.Text = ""
                .ProdSEL1.Show()
                .lblScanPCSEL.Show()
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
    ''' <summary>
    ''' To display item details screen.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayItemDetails(ByVal o As Object, ByVal e As EventArgs)
        Dim iCount As Integer = 0
        Dim strBarcode As String = ""
        Try
            With m_PCLItemDetails
                .lblBootsCode.Text = objAppContainer.objHelper.FormatBarcode(m_PCLProductInfo.BootsCode)
                strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(m_PCLProductInfo.ProductCode)
                .lblPdtCode.Text = objAppContainer.objHelper.FormatBarcode(strBarcode)
                arrItemDescr = objAppContainer.objHelper.GetFormattedDescription(m_PCLProductInfo.Description)
                .lblItemDescr1.Text = arrItemDescr.Item(0)
                .lblItemDescr2.Text = arrItemDescr.Item(1)
                .lblItemDescr3.Text = arrItemDescr.Item(2)
                .lblStatus.Show()
                .lblStatusVal.Text = objAppContainer.objHelper.GetStatusDescription(m_PCLProductInfo.Status)
                .lblEnterSELQty.Show()
                .lblSELQty.Text = "1"
                .lblScanEnter.Show()
                .lblClearance.Font = New System.Drawing.Font("Tahoma", 9.0!, FontStyle.Bold)
                .lblEnterSELQty.Font = New System.Drawing.Font("Tahoma", 9.0!, FontStyle.Regular)
                'Set btn to visible in case of non catch weight items.
                If m_PCLProductInfo.PHFType = MobilePrintSessionManager.LabelType.CWCLR Then
                    .Btn_CalcPad_small2.Visible = False
                Else
                    .Btn_CalcPad_small2.Visible = True
                    .Btn_CalcPad_small2.Enabled = True
                End If
                'If clearance price is present dsplay clearance price or 
                'else display current price until clearance price is available.
                'MessageBox.Show(m_PCLProductInfo.ClearancePrice)
                If m_PCLProductInfo.ClearancePrice > 0 Then
                    .lblClearance.Text = "Clearance Price:"
                    UpdateClearancePrice(m_PCLProductInfo.ClearancePrice)
                Else
                    .lblClearance.Text = "Current Price:"
                    UpdateClearancePrice(m_PCLProductInfo.CurrentPrice, False)
                End If
                .lblEnterSELQty.Text = "Label Quantity:"
                'Set the bottom label text to ask user to enter clearance price.
                .lblScanEnter.Text = "Enter Clearance price"
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                'Make the print button invisible until the clearance price is entered.
                .Btn_Print1.Visible = False
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("PSSessionMgr - Exception in full DisplayPSItemDetails", _
                                                  Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Subroutine to keep count of the total number of scanned items
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateNumItemsScanned()
        iNumScanned += 1
    End Sub
    ''' <summary>
    ''' Processes the selction of Print button click
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessPrintSelection()
        'print labels.call printer session function to print SELs or clearance labels.
        '1.1 Remove the message box
        'Archana Chandramathi
        '13C Chilled Food project 
        'If bFirstPrint Then
        '    'Display the messgae to alert the user to user correct stationary.
        '    MessageBox.Show(MessageManager.GetInstance().GetMessage("M81"), "Printer Warning", _
        '                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        'End If
        'set label quantity
        m_PCLProductInfo.LabelQuantity = CInt(m_PCLItemDetails.lblSELQty.Text)
        'set the current price and clearance price.
        m_PCLProductInfo.OriginalPrice = m_PCLProductInfo.CurrentPrice
        m_PCLProductInfo.CurrentPrice = m_PCLProductInfo.ClearancePrice
        'print the label
        If MobilePrintSessionManager.GetInstance.MobilePrinterStatus Then
            MobilePrintSessionManager.GetInstance.CreateLabels(m_PCLProductInfo)
        End If
        'After successful first print set the flag to false.
        bFirstPrint = False
    End Sub
    ''' <summary>
    ''' To process when the user selects Quit button.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessQuitSelection()
        'reset the value class object.
        'Object is still in memory
        m_PCLProductInfo = New PSProductInfo()
        ''''Bug HAs to be fixed
        'Setting app Module back to the original module
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PRTCLEARANCE
        'After successful first print set the flag to false.
        bFirstPrint = True
    End Sub
    ''' <summary>
    ''' Function to retrieve the total number of items scanned in the particular session
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetNumItemsScanned() As Integer
        Return iNumScanned
    End Function
    ''' <summary>
    ''' Update the clearance price as per the requirement in the display.
    ''' </summary>
    ''' <param name="iClearancePrice"></param>
    ''' <remarks></remarks>
    Public Function UpdateClearancePrice(ByVal iClearancePrice As Integer, Optional ByVal bIsClrPrice As Boolean = True) As Boolean
        Dim dPrice As Double = 0.0
        Try
            If iClearancePrice < 100 Then
                With m_PCLItemDetails
                    .lblClearancePrice.TextAlign = ContentAlignment.TopRight
                    .lblClearancePrice.Location = New System.Drawing.Point(130 * objAppContainer.iOffSet, 131 * objAppContainer.iOffSet)
                    .lblClearancePrice.Text = iClearancePrice.ToString()
                    .lblCurrency.Location = New System.Drawing.Point(181 * objAppContainer.iOffSet, 130 * objAppContainer.iOffSet)
                    .lblCurrency.Text = strSmallCurrency
                    .Btn_Print1.Visible = True
                    'Set the bottom label text to default text.
                    .lblScanEnter.Text = "Scan/Enter item or print labels"
                    If bIsClrPrice Then
                        .lblClearance.Text = "Clearance Price:"
                        .lblClearance.Font = New System.Drawing.Font("Tahoma", 9.0!, FontStyle.Regular)
                        .lblEnterSELQty.Font = New System.Drawing.Font("Tahoma", 9.0!, FontStyle.Bold)
                        .lblEnterSELQty.Text = "Label Quantity:"
                    End If
                End With
            ElseIf iClearancePrice >= 100 Then
                With m_PCLItemDetails
                    .lblClearancePrice.TextAlign = ContentAlignment.TopLeft
                    .lblClearancePrice.Location = New System.Drawing.Point(146 * objAppContainer.iOffSet, 131 * objAppContainer.iOffSet)
                    .lblClearancePrice.Text = String.Format("{0:n}", Convert.ToDouble(iClearancePrice / 100))
                    .lblCurrency.Location = New System.Drawing.Point(132 * objAppContainer.iOffSet, 130 * objAppContainer.iOffSet)
                    .lblCurrency.Text = strCurrency
                    .Btn_Print1.Visible = True
                    'Set the bottom label text to default text.
                    .lblScanEnter.Text = "Scan/Enter item or print labels"
                    If bIsClrPrice Then
                        .lblClearance.Text = "Clearance Price:"
                        .lblClearance.Font = New System.Drawing.Font("Tahoma", 9.0!, FontStyle.Regular)
                        .lblEnterSELQty.Font = New System.Drawing.Font("Tahoma", 9.0!, FontStyle.Bold)
                        .lblEnterSELQty.Text = "Label Quantity:"
                    End If
                End With
            End If
            'If bIsClrPrice Then
            '    'set current price to WasPrice1 to be used while creating wider clearance label.
            '    m_PCLProductInfo.WasPrice1 = m_PCLProductInfo.CurrentPrice
            '    'Update clearance price into current price.
            '    m_PCLProductInfo.CurrentPrice = iClearancePrice
            'End If
            'Set the clearance price entered in the object.
            m_PCLProductInfo.ClearancePrice = iClearancePrice
            objAppContainer.objLogger.WriteAppLog("PSSessionMgr - Updated clearanace price", _
                                                  Logger.LogLevel.RELEASE)
            'update clearance price in the object array
            'Return UpdateClearancePrice(iClearancePrice)
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("PSSessionMgr - Exception in full UpdateClearancePrice", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' To update the item details into array.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateProductInfo(Optional ByVal iClrPrice As Integer = 0) As Boolean
        Dim iIndex As Integer = 0
        Try
            If iClrPrice > 0 Then
                m_PCLProductInfo.ClearancePrice = iClrPrice
            End If
            m_PCLProductInfo.LabelQuantity = CInt(m_PCLItemDetails.lblSELQty.Text)
            'now add the object to the array list.
            For Each objItems As PSProductInfo In m_PCLItemList
                If objItems.ProductCode = m_PCLProductInfo.ProductCode Then
                    m_PCLItemList.RemoveAt(iIndex)
                    m_PCLItemList.Insert(iIndex, m_PCLProductInfo)
                    'return true after updating the array.
                    Return True
                End If
                iIndex += 1
            Next
            'if control reach here then it means that the item is not present.
            m_PCLItemList.Add(m_PCLProductInfo)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("PSSessionMgr - Exception in full UpdateProductInfo", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' To send PRT record.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SendPRT(ByVal cPrintType As String)
        Try
            'Dim objPRT As PRTRecord = New PRTRecord()
            'objPRT.strBootscode = m_PCLProductInfo.BootsCode.Trim()
            'objPRT.cIsMethod = cPrintType.Trim()
            objAppContainer.objExportDataManager.CreatePRT(m_PCLProductInfo.BootsCode.Trim(), _
                                                           SMTransactDataManager.ExFileType.EXData)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("PSSessionMgr - Exception in full SendPRT", _
                                                  Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Retrieves the lists of products from the Export Data Requirements list.
    ''' </summary>
    ''' <returns>Export Data Requirements  List on Success and NULL on error</returns>
    ''' <remarks></remarks>
    Private Function GetProductList() As ArrayList
        Return objArrayExportData
    End Function
    ''' <summary>
    ''' Returns the current price of the item scanned.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCurrentPrice() As Integer
        Return CInt(m_PCLProductInfo.CurrentPrice)
    End Function
    ''' <summary>
    ''' Enum Class that defines all screens for Print SEL module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum PCLSCREENS
        Home
        ItemDetails
        PCLHelp
    End Enum
    ''' <summary>
    ''' Enumerates all scanable fields in PS Module
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum SCNFIELDS
        ProductCode
        SELCode
    End Enum
End Class