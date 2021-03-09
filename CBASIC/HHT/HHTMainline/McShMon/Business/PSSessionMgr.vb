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
Public Class PSSessionMgr
    Private m_PShome As frmPSHome
    Private m_PSItemDetails As frmPSItemDetails
    Private m_PSSummary As frmPSSummary

    Private Shared m_PSSessionMgr As PSSessionMgr = Nothing
    Private m_PSProductInfo As PSProductInfo = Nothing
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
    Public Shared Function GetInstance() As PSSessionMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PRINTSEL
        If m_PSSessionMgr Is Nothing Then
            m_PSSessionMgr = New PSSessionMgr()
            Return m_PSSessionMgr
        Else
            Return m_PSSessionMgr
        End If
    End Function
#If RF Then
    ''' <summary>
    ''' Updates the Status bar of all the forms in the session manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBarMessage()
        Try
            m_PShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_PSItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_PSSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured, Trace: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End If
    ''' <summary>
    ''' Initialises the Print SEL Session 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function StartSession() As Boolean
        Try
#If RF Then
            'Send INS transaction if in RF mode
            If Not objAppContainer.objExportDataManager.CreateINS() Then
                Return False
            End If
#End If
            'Do all print SEL realated Initialisations here.
            m_PShome = New frmPSHome()
            m_PSItemDetails = New frmPSItemDetails()
            m_PSSummary = New frmPSSummary()
            m_PSProductInfo = New PSProductInfo()
            'System Testing
            ' m_bIsFullPriceCheckRequired = False
            'IT Fixes
            ' m_QueuedSELList = New ArrayList()
            objArrayExportData = New ArrayList()
            ' m_strSEL = ""
            ' m_PreviousItem = ""
            'Added for mobile printing
            If objAppContainer.bMobilePrinterAttachedAtSignon Then
                With m_PSItemDetails
                    .Btn_Quit_small1.Visible = True
                    .Btn_Print1.Visible = False
                    .lblScanEnter.Text = "Scan/Enter next Item"
                End With
            Else
                With m_PSItemDetails
                    .Btn_Print1.Visible = True
                    .Btn_Quit_small1.Visible = False
                    '.lblScanEnter.Text = "Scan/Enter next Item or Print to print SELs"
                End With
            End If
            'Check if mobile printer is connected.
            If ConfigDataMgr.GetInstance().GetParam("ValidCurrency").ToString().Equals("S") Then
                strCurrency = Macros.POUND_SYMBOL.ToString()
                strSmallCurrency = Macros.PENCE_SYMBOL.ToString()
            ElseIf ConfigDataMgr.GetInstance().GetParam("ValidCurrency").ToString().Equals("E") Then
                strCurrency = Macros.EURO_SYMBOL.ToString()
                strSmallCurrency = Macros.CENTS_SYMBOL.ToString()
            End If
            'Set label type
            m_LabelType = MobilePrintSessionManager.LabelType.STD
            'set the location of the items for the labels and hide the clearance price lables
            'Me.DisplayPSScreen(PSSCREENS.Home)
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in PrintSEL:Print SEL Session cannot be started" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PSSessionMgr StartSession", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by PSSessionMgr.
    ''' </summary>
    ''' <returns>True if terminate is sucess else False</returns>
    ''' <remarks></remarks>
#If NRF Then
        Public Function EndSession() As Boolean 
#ElseIf RF Then
    Public Function EndSession(Optional ByVal isConnectivityLoss As Boolean = False) As Boolean
#End If
        'Save and data and perform all Exit Operations.
        'Close and Dispose all forms.
        Try
            'write export data for batch mode operation.
#If RF Then
            If Not isConnectivityLoss Then
                'Send INX transaction if in RF mode
                If Not objAppContainer.objExportDataManager.CreateINX() Then
                    Return False
                End If
            End If
#End If
#If NRF Then
            If Not objAppContainer.bMobilePrinterAttachedAtSignon Then
                WriteExportData()
                'Set active module to none after quitting the module
                objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.NONE
            End If
#End If
            m_PShome.Dispose()
            m_PSItemDetails.Dispose()
            m_PSSummary.Dispose()
            'Release all objects and Set to nothig.
            m_PSProductInfo = Nothing
            m_PSSessionMgr = Nothing
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
    ''' Writes the export data record
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteExportData() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter PSSessionMgr WriteExportData", Logger.LogLevel.RELEASE)
        'Integration Testing : Fix
        Dim iIndex As Integer
        Dim iTotal As Integer = 0
        Try
            m_PSSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PRT")
            'Dim objSMDataManager As SMTransactDataManager = New SMTransactDataManager()


            'System testing removed ElseIf and changed it to If
            If objArrayExportData.Count > 0 Then
                'Write INS record
                objAppContainer.objExportDataManager.CreateINS()
                'Write all PRTs
                For Each objPSData As PSExporDataReq In objArrayExportData
                    iTotal = objPSData.numCurrentSELs + objPSData.numSELs
                    For iIndex = 0 To iTotal - 1
                        'Dim objPRTData As PRTRecord = New PRTRecord()
                        'System Testing
                        'objPRTData.strBootscode = objAppContainer.objHelper.GeneratePCwithCDV(obj.pdtCode)
                        'objPRTData.strBootscode = objPSData.strBootsCode
                        'objPRTData.cIsMethod = Macros.PRINT_BATCH
                        objAppContainer.objExportDataManager.CreatePRT(objPSData.strBootsCode, SMTransactDataManager.ExFileType.EXData)
                    Next
                Next
                'Write INX record
                objAppContainer.objExportDataManager.CreateINX()
            End If
            'Return true to the calling function
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in PrintSEL:Print SEL WriteExportData failure" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PSSessionMgr WriteExportData", Logger.LogLevel.RELEASE)
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
        m_PShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        Try
            Select Case Type
                Case BCType.EAN
                    If Not (objAppContainer.objHelper.ValidateEAN(strBarcode)) Or _
                       Val(strBarcode) = 0 Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        m_PShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        Return
                    Else
                        strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                        If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_PSProductInfo)) Then
#If NRF Then  						
                            'DARWIN checking if the base barcode is present in Database/Conroller
                            If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                                'DARWIN converting database to Base Barcode
                                strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                                If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_PSProductInfo)) Then
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                                                  MessageBoxButtons.OK, _
                                                                  MessageBoxIcon.Asterisk, _
                                                                  MessageBoxDefaultButton.Button1)
                                    m_PShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                    Return
                                End If
                            Else
#End If
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                            m_PShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
#If NRF Then 
                            End If
#End If
                        End If
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
                            m_PShome.ProdSEL1.txtProduct.Text = ""
                            m_PShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
                        Else
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBarcode, m_PSProductInfo)) Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                                m_PShome.ProdSEL1.txtProduct.Text = ""
                                m_PShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                Return
                            End If
                        End If
                    Else
                        If (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                            strBarcode = strBarcode.PadLeft(13, "0")
                            strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_PSProductInfo)) Then
#If NRF Then
                                'DARWIN checking if the base barcode is present in Database/Conroller
                                If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                                    'DARWIN converting database to Base Barcode
                                    strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_PSProductInfo)) Then
                                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                           MessageBoxButtons.OK, _
                                           MessageBoxIcon.Asterisk, _
                                           MessageBoxDefaultButton.Button1)
                                        m_PShome.ProdSEL1.txtProduct.Text = ""
                                        m_PShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                        Return
                                    End If
                                Else
#End If
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                                m_PShome.ProdSEL1.txtProduct.Text = ""
                                m_PShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                Return
#If NRF Then
                                End If
#End If
                            End If
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                           MessageBoxButtons.OK, _
                           MessageBoxIcon.Asterisk, _
                           MessageBoxDefaultButton.Button1)
                            m_PShome.ProdSEL1.txtProduct.Text = ""
                            m_PShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
                        End If
                    End If
                Case BCType.SEL
                    If objAppContainer.objHelper.ValidateSEL(strBarcode) Then
                        Dim strBootsCode As String = ""
                        objAppContainer.objHelper.GetBootsCodeFromSEL(strBarcode, strBootsCode)
                        strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
                        If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBootsCode, m_PSProductInfo)) Then
                            objAppContainer.objLogger.WriteAppLog("Print SEL Cannot Obtain Boots code from SEL", Logger.LogLevel.DEBUG)
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                            m_PShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Exit Sub
                        End If
                    Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M4"), "Error", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                        m_PShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        Return
                    End If
            End Select
            'm_bIsFullPriceCheckRequired = False
            UpdateNumItemsScanned()
            'UT Changes
            PSSessionMgr.GetInstance().DisplayPSScreen(PSSessionMgr.PSSCREENS.ItemDetails)
            'Send PRT message and get LPR transacitons if mobile printer attached.
            'send the request and get the detials required to print label.
            If objAppContainer.bMobilePrinterAttachedAtSignon Or _
                objAppContainer.strPrintFlag = Macros.PRINT_LOCAL Then
#If RF Then
                objAppContainer.objDataEngine.GetLabelDetails(m_PSProductInfo)
#ElseIf NRF Then
                m_PSProductInfo.MSFlag = objAppContainer.objDataEngine.CheckMultisite(m_PSProductInfo.BootsCode)
#End If
                m_PSProductInfo.LabelQuantity = 1
                MobilePrintSessionManager.GetInstance.CreateLabels(m_PSProductInfo)
            Else
#If RF Then
                'Incase if in RF and mobile printer not attached send PRT request.
                objAppContainer.objExportDataManager.CreatePRT(m_PSProductInfo.BootsCode, _
                                                               SMTransactDataManager.ExFileType.EXData)
#End If
            End If
            'UAT commented as array need not be maintained
            Dim bIsExisting As Boolean = False

            For Each objPSData As PSExporDataReq In objArrayExportData
                If objPSData.strBootsCode.Equals(m_PSProductInfo.BootsCode) Then
                    bIsExisting = True
                    UpdateNumSELsQueued(True)
                    Exit For
                End If
            Next
            'Add it to export data array if it is a new item
            If Not bIsExisting Then
                Dim objPSExporDataReq As PSExporDataReq = New PSExporDataReq()
                objPSExporDataReq.numCurrentSELs = CInt(m_PSItemDetails.lblSELQty.Text.ToString())
                objPSExporDataReq.strBootsCode = m_PSProductInfo.BootsCode
                objArrayExportData.Add(objPSExporDataReq)
            End If
            'System Testing
            BCReader.GetInstance.StartRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in PrintSEL:Print SEL EndSession failure" + ex.StackTrace, Logger.LogLevel.RELEASE)
        Finally
            If Not m_PShome Is Nothing Then
                If Not m_PShome.ProdSEL1.IsDisposed AndAlso Not m_PShome.ProdSEL1.txtProduct.IsDisposed Then
                    m_PShome.ProdSEL1.txtSEL.Text = ""
                    m_PShome.ProdSEL1.txtProduct.Text = ""
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
    Public Function DisplayPSScreen(ByVal ScreenName As PSSCREENS)
        'Invoke method for other PS screens here.
        Try
            Select Case ScreenName
                Case PSSCREENS.Home
                    m_PShome.Invoke(New EventHandler(AddressOf DisplayPSScan))
                Case PSSCREENS.ItemDetails
                    m_PSItemDetails.Invoke(New EventHandler(AddressOf DisplayPSItemDetails))
                Case PSSCREENS.PSSummary
                    m_PSSummary.Invoke(New EventHandler(AddressOf DisplayPSSummary))
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at Print SEL Display Screen" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' To display the scan screen.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayPSScan(ByVal o As Object, ByVal e As EventArgs)
        With m_PShome
            .ProdSEL1.Show()
            .lblScanPCSEL.Show()
            'Sets the store id and active data time to the status bar
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

            .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' To display item details screen.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayPSItemDetails(ByVal o As Object, ByVal e As EventArgs)
        Dim iCount As Integer = 0
        Dim strBarcode As String = ""
        Try
            With m_PSItemDetails
                .lblBootsCode.Text = objAppContainer.objHelper.FormatBarcode(m_PSProductInfo.BootsCode)
                strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(m_PSProductInfo.ProductCode)
                .lblPdtCode.Text = objAppContainer.objHelper.FormatBarcode(strBarcode)
                arrItemDescr = objAppContainer.objHelper.GetFormattedDescription(m_PSProductInfo.Description)
                .lblItemDescr1.Text = arrItemDescr.Item(0)
                .lblItemDescr2.Text = arrItemDescr.Item(1)
                .lblItemDescr3.Text = arrItemDescr.Item(2)
                .lblStatus.Show()
                .lblStatusVal.Text = objAppContainer.objHelper.GetStatusDescription(m_PSProductInfo.Status)
                .lblEnterSELQty.Show()
                'For iCount = 0 To objArrayExportData.Count - 1
                '    Dim obj As PSExporDataReq = New PSExporDataReq()
                '    obj = objArrayExportData.Item(iCount)
                '    If obj.pdtCode.Equals(m_PSProductInfo.ProductCode) Then
                '        .lblSELQty.Text = obj.numSELs.ToString()
                '        Exit For
                '    Else
                .lblSELQty.Text = "1"
                '    End If
                'Next
                
                .lblScanEnter.Show()

                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("PSSessionMgr - Exception in full DisplayPSItemDetails", Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To display the summary screen.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayPSSummary(ByVal o As Object, ByVal e As EventArgs)
        With m_PSSummary
            .lblPrintSummary.Show()
            .lblScannedItems.Show()
            .lblScanNum.Text = GetNumItemsScanned()
            .lblSELQueued.Show()
            .lblSELNum.Text = CStr(GetSELQueuedCount())
#If RF Then
            .lblAction.Visible = False
            '.lblPrintSEL added for displaying the remaining test message-Anil
            .lblPrintSEL.Visible = True
#End If
#If NRF Then
            .lblAction.Show()
            If objAppContainer.bMobilePrinterAttachedAtSignon Then
                .lblAction.Text = "Action: Collect and display all new SELs"
            End If
#End If

            'Sets the store id and active data time to the status bar
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

            .Visible = True
            .Refresh()
            'TODO Fn to write PRT requests in the export data file
            'WriteTempData()
        End With
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
#If NRF Then
        If objAppContainer.bMobilePrinterAttachedAtSignon = False Then
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M43"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
        End If
#End If
        'display the summary screen.
        DisplayPSScreen(PSSessionMgr.PSSCREENS.PSSummary)
    End Sub
    ''' <summary>
    ''' Function to retrieve the total SELs queued in the particular session
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSELQueuedCount() As Integer
        Dim iTotalSELs As Integer = 0
        Try
            For Each obj As PSExporDataReq In objArrayExportData
                iTotalSELs += obj.numSELs + obj.numCurrentSELs
            Next
            Return iTotalSELs
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in PrintSEL: SEL Queued count cannot be retrieved " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    ''' Function to retrieve the total number of items scanned in the particular session
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetNumItemsScanned() As Integer
        Return iNumScanned
    End Function
    ''' <summary>
    ''' Sub to update if SELs queued has changed in the particular session
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateNumSELsQueued(Optional ByVal bIsItemScanned As Boolean = False)
        objAppContainer.objLogger.WriteAppLog("Enter PSSessionMgr UpdatedNumSElsQueued", Logger.LogLevel.RELEASE)
        Dim strPdtCode As String = ""
        Dim iIndexCount As Integer = 0
        Dim iCount As Integer = 0
        Dim bIsModify As Boolean = False
        Dim iTotalSEL As Integer = 0
        Dim objExpData As PSExporDataReq = New PSExporDataReq()
        'Dim objPRT As PRTRecord = New PRTRecord()
        Try
            For iCount = 0 To objArrayExportData.Count - 1
                Dim objPSData As PSExporDataReq = New PSExporDataReq()
                objPSData = objArrayExportData.Item(iCount)
                If objPSData.strBootsCode.Equals(m_PSProductInfo.BootsCode) And bIsItemScanned = True Then
                    'UAT to keep track of total SELs
                    'update the count from current to existing SELs count
                    objPSData.numSELs = objPSData.numSELs + objPSData.numCurrentSELs
                    objPSData.numCurrentSELs = CInt(m_PSItemDetails.lblSELQty.Text.ToString())
                    objExpData = objPSData
                    iIndexCount = iCount
                    'If Quit is selected from calcpad, the previous count needs to be maintained
                    bIsModify = True
                    Exit For
                ElseIf objPSData.strBootsCode.Equals(m_PSProductInfo.BootsCode) And bIsItemScanned = False Then
                    'If the SEL count is updated using Calc pad then update the current quantity.
                    objPSData.numCurrentSELs = CInt(m_PSItemDetails.lblSELQty.Text.ToString())
                    objExpData = objPSData
                    iIndexCount = iCount
                    'If Quit is selected from calcpad, the previous count needs to be maintained
                    bIsModify = True
                End If
            Next

            If bIsModify Then
                objArrayExportData.RemoveAt(iIndexCount)
                objArrayExportData.Insert(iIndexCount, objExpData)
                iTotalSEL = objExpData.numCurrentSELs - 1
                'Print the labels if mobile printer is attached.
                If objAppContainer.bMobilePrinterAttachedAtSignon Then
                    'print labels.
                    'update the number of labels reducing 1 label count as it is printed already.
                    m_PSProductInfo.LabelQuantity = iTotalSEL
                    'call printer session function to print SELs or clearance labels.
                    MobilePrintSessionManager.GetInstance.CreateLabels(m_PSProductInfo)
                Else
#If RF Then
                    For iIndex As Integer = 1 To iTotalSEL
                        objAppContainer.objExportDataManager.CreatePRT(objExpData.strBootsCode, _
                                                                       SMTransactDataManager.ExFileType.EXData)
                    Next
#End If
                End If
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("PSSessionMgr - Exception in full UpdateNumSELsQueued", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PSSessionMgr UpdatedNumSElsQueued", Logger.LogLevel.RELEASE)
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
    ''' Enum Class that defines all screens for Print SEL module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum PSSCREENS
        Home
        ItemDetails
        PSSummary
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
Public Class PSExporDataReq
    Public numCurrentSELs As Integer = 0
    Public numSELs As Integer = 0
    Public strBootsCode As String
End Class
''' <summary>
''' Value class to hold the values for each items scanned in Print SEL or clearance label.
''' </summary>
''' <remarks>*****Govindh Dec 2009 Clearance Label functionality changes *****</remarks>
Public Class PSProductInfo
    Inherits ProductInfo
    Private iClearancePrice As Integer
    Private iCurrentPrice As Integer
    Private m_Currency As String
    Private m_WasPrice1 As String
    Private m_WasPrice2 As String
    Private m_PHFType As MobilePrintSessionManager.LabelType
    Private m_UnitPriceFlag As String
    Private m_UnitMeasure As String
    Private m_UnitQuantity As String
    Private m_UnitType As String
    Private m_UnitPriceLine As String
    Private m_WEEEFlag As String
    Private m_WEEEPrfPrice As String
    'Private m_MSFlag As String
    Private m_PainKillerMessage As String
    Private m_MajorCurrencySymbol As String
    Private m_LabelQuantity As Integer
    Private m_OriginalPrice As String
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'Set the currency type.  ******* Govindh *** for clearance label printing.
        m_Currency = "S"
        If PSSessionMgr.GetInstance().CurrencySymbol.Equals("S") Then
            m_Currency = "S"
        ElseIf PSSessionMgr.GetInstance().CurrencySymbol.Equals("E") Then
            m_Currency = "E"
        End If
        'set major currency symbol.
        m_MajorCurrencySymbol = PSSessionMgr.GetInstance().CurrencySymbol
    End Sub
    ''' <summary>
    ''' Property to hold clerance price.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property ClearancePrice() As Integer
        Get
            Return iClearancePrice
        End Get
        Set(ByVal value As Integer)
            iClearancePrice = value
        End Set
    End Property
    ''' <summary>
    ''' Holds the current price of the item.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CurrentPrice() As Integer
        Get
            Return iCurrentPrice
        End Get
        Set(ByVal value As Integer)
            iCurrentPrice = value
        End Set
    End Property
    ''' <summary>
    ''' To hold the original price.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OriginalPrice() As String
        Get
            Return m_OriginalPrice
        End Get
        Set(ByVal value As String)
            m_OriginalPrice = value
        End Set
    End Property
    ''' <summary>
    ''' Currency Symbol
    ''' </summary>
    ''' <value>S = £/p   E = €/c (ascii value: 80h/A2h)</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property CurrencyType() As String
        Get
            Return m_Currency
        End Get
    End Property
    ''' <summary>
    ''' Previous price of the item.
    ''' </summary>
    ''' <value>6 ASC 2 dp assumed</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property WasPrice1() As String
        Get
            Return m_WasPrice1
        End Get
        Set(ByVal value As String)
            m_WasPrice1 = value
        End Set
    End Property
    ''' <summary>
    ''' Previous to last price of the item
    ''' </summary>
    ''' <value>6 ASC 2 dp assumed</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property WasPrice2() As String
        Get
            Return m_WasPrice2
        End Get
        Set(ByVal value As String)
            m_WasPrice2 = value
        End Set
    End Property
    ''' <summary>
    ''' Type of label to be printed.
    ''' </summary>
    ''' <value>0=STD, 1=WN, 2=WWN, 3=CLR</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property PHFType() As MobilePrintSessionManager.LabelType
        Get
            Return m_PHFType
        End Get
    End Property
    ''' <summary>
    ''' Property to set the SEL type.
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property SELLabelType() As String
        Set(ByVal value As String)
            Select Case value
                Case 0
                    m_PHFType = MobilePrintSessionManager.LabelType.STD
                Case 1
                    m_PHFType = MobilePrintSessionManager.LabelType.WN
                Case 2
                    m_PHFType = MobilePrintSessionManager.LabelType.WWN
                Case 3
                    m_PHFType = MobilePrintSessionManager.LabelType.CLR     'fix for 4257 - for CIP items
                Case 4
                    m_PHFType = MobilePrintSessionManager.LabelType.RCLR   'For normal clearance labels
                Case 5
                    m_PHFType = MobilePrintSessionManager.LabelType.CWCLR    'For catchweigth lines
            End Select
        End Set
    End Property
    ''' <summary>
    ''' Unit price flag of the item.
    ''' </summary>
    ''' <value>if Y then WEEE must be N</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UnitPriceFlag() As String
        Get
            Return m_UnitPriceFlag
        End Get
        Set(ByVal value As String)
            m_UnitPriceFlag = value
        End Set
    End Property
    ''' <summary>
    ''' Unit measure for the unit of the item.
    ''' </summary>
    ''' <value>6 ASC 0 - 999999</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UnitMeasure() As String
        Get
            Return m_UnitMeasure
        End Get
        Set(ByVal value As String)
            m_UnitMeasure = value
        End Set
    End Property
    ''' <summary>
    ''' Unit quantity for the item.
    ''' </summary>
    ''' <value>6 ASC 0 - 999999</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UnitQuantity() As String
        Get
            Return m_UnitQuantity
        End Get
        Set(ByVal value As String)
            m_UnitQuantity = value
        End Set
    End Property
    ''' <summary>
    ''' Unit type for measurement
    ''' </summary>
    ''' <value>Litre, Kg, g, ml, unit etc</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UnitType() As String
        Get
            Return m_UnitType
        End Get
        Set(ByVal value As String)
            m_UnitType = value
        End Set
    End Property
    ''' <summary>
    ''' Unit price line for the item.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UnitPriceLine() As String
        Get
            Return m_UnitPriceLine
        End Get
        Set(ByVal value As String)
            m_UnitPriceLine = value
        End Set
    End Property
    ''' <summary>
    ''' WEEE Flag
    ''' </summary>
    ''' <value>if Y then UnitPriceFlag must be N</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property WEEEFlag() As String
        Get
            Return m_WEEEFlag
        End Get
        Set(ByVal value As String)
            Dim iTemp As Integer = CType(value, Integer)
            Dim iCD As Integer = 6
            If (iTemp And iCD) <> 0 Then
                m_WEEEFlag = "Y"
            Else
                m_WEEEFlag = " "
            End If
        End Set
    End Property
    ''' <summary>
    ''' WEEE Price Fla
    ''' </summary>
    ''' <value>6 ASC 2 dp assumed</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property WEEEPrfPrice() As String
        Get
            Return m_WEEEPrfPrice
        End Get
        Set(ByVal value As String)
            m_WEEEPrfPrice = value
        End Set
    End Property
    '''' <summary>
    '''' Multisite Flag
    '''' </summary>
    '''' <value>Y/N or X not on active planner. Set to MS if Y</value>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Property MSFlag() As String
    '    Get
    '        Return m_MSFlag
    '    End Get
    '    Set(ByVal value As String)
    '        m_MSFlag = value
    '    End Set
    'End Property
    ''' <summary>
    ''' Pain killer message if the item is in pain killer category.
    ''' </summary>
    ''' <value>38-byte Painkiller Message eg. Incl. Paracetamol</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PainKillerMessage() As String
        Get
            Return m_PainKillerMessage
        End Get
        Set(ByVal value As String)
            m_PainKillerMessage = value
        End Set
    End Property
    ''' <summary>
    ''' Major currency symbol for the location of store.
    ''' </summary>
    ''' <value>Used to hold currency symbol for $25 template variable</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property MajorCurrencySymbol() As String
        Get
            Return m_MajorCurrencySymbol
        End Get
    End Property
    ''' <summary>
    ''' To store the quantity for label to the printed.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LabelQuantity() As Integer
        Get
            Return m_LabelQuantity
        End Get
        Set(ByVal value As Integer)
            m_LabelQuantity = value
        End Set
    End Property

End Class