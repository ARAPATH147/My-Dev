Imports System.Math
''' ***************************************************************************
''' <fileName>PCSessionMgr.vb</fileName>
''' <summary>The Price Check Session Manager Class.
''' Implements all business logic and GUI navigation for Price Check.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''*****************************************************************************
Public Class PCSessionMgr
    Private m_PChome As frmPCHome
    Private m_PCSummary As frmPCSummary

    Private Shared m_PCSessionMgr As PCSessionMgr = Nothing
    Private m_PriceCheckInfo As PriceCheckInfo = Nothing
    Private m_PCTargetDetails As PCTargetDetails = Nothing

    'PCItemList maintains all the items scanned in a price check session
    Private m_PCItemList As ArrayList = Nothing
    Private SELQueuedCount As Integer = 0
    Private m_QueuedSELList As ArrayList = Nothing
    Private m_bPCTargetAvailable As Boolean
    Private bFlag As Boolean
    Private iTBFlag As Integer = 0
    'IT Internal
    Private bIsPriceIncrease As Boolean
    'UAT - Array to store ENQ objects
    Private m_QueuedPCMList As ArrayList = Nothing
    Private iPriceCheckCount As Integer = 0
    Private cCurencySymb As String = ""
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()

    End Sub
    ''' <summary>
    ''' Functions for getting the object instance for the PCSessionMgr. 
    ''' Use this method to get the object reference for the Singleton PCSessionMgr.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of PCSessionMgr Class</remarks>
    Public Shared Function GetInstance() As PCSessionMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PRICECHK
        If m_PCSessionMgr Is Nothing Then
            m_PCSessionMgr = New PCSessionMgr()
            Return m_PCSessionMgr
        Else
            Return m_PCSessionMgr
        End If
    End Function
#If RF Then
    ''' <summary>
    ''' Updates the Status bar of all the forms in the session manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBarMessage()
        Try
            m_PChome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_PCSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception Occured, Trace : " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End If
    ''' <summary>
    ''' Initialises the Price Check Session 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function StartSession() As Boolean
        'Do all price check realated Initialisations here.
        Dim bTemp As Boolean = False
        Try
#If RF Then
            If objAppContainer.objExportDataManager.CreatePCS() Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is PCRRecord Then
                        'Check price check target and completed count and display warnign message.
                        'if the completed count is greater than or equal to target then ask confirmation
                        'from user.
                        If CType(objResponse, PCRRecord).strPriceChkDone >= CType(objResponse, PCRRecord).strPriceChkTarget Then
                            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M91"), _
                                            "Information", MessageBoxButtons.YesNo, _
                                            MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) = DialogResult.No Then
                                Dim objPCX As New PCXRecord()
                                objPCX.strSELs = "0000"
                                objPCX.strCheckedItems = "0000"
                                If Not (objAppContainer.objExportDataManager.CreatePCX(objPCX)) Then
                                    objAppContainer.objLogger.WriteAppLog("Cannot Create PCX record at PC End Session", Logger.LogLevel.RELEASE)
                                    Throw New Exception(Macros.USER_ABORT)
                                End If
                            End If
                        Else
                        End If
#End If
                        m_PChome = New frmPCHome()
                        m_PCSummary = New frmPCSummary()

                        m_PriceCheckInfo = New PriceCheckInfo()
                        m_PCTargetDetails = New PCTargetDetails()
                        m_QueuedSELList = New ArrayList()
#If NRF Then
                     m_bPCTargetAvailable = False
#ElseIf RF Then
                        Dim objPCRRecord As PCRRecord = Nothing
                        objPCRRecord = CType(objResponse, PCRRecord)
                        'bug fix
                        'no need to show zero padded values in target and completed count
                        m_PCTargetDetails.PriceCheckCompleted = (Convert.ToDouble(objPCRRecord.strPriceChkDone)).ToString()
                        m_PCTargetDetails.PriceCheckTarget = (Convert.ToDouble(objPCRRecord.strPriceChkTarget)).ToString()
                        m_bPCTargetAvailable = True
                        objPCRRecord = Nothing
#End If
                        'Assign the currency symbol
                        Dim strTemp As String = ConfigDataMgr.GetInstance.GetParam(ConfigKey.VALID_CURRENCY).ToString()
                        If strTemp = "S" Then
                            cCurencySymb = Macros.POUND_SYMBOL
                        ElseIf strTemp = "E" Then
                            cCurencySymb = Macros.EURO_SYMBOL
                        End If
                        bFlag = True
                        bIsPriceIncrease = False
                        m_QueuedPCMList = New ArrayList()
                        m_PCItemList = New ArrayList()
                        Me.DisplayPCScreen(PCSCREENS.Home)
                        bTemp = True
#If RF Then
                    End If
                End If
            End If
#End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or _
                        ex.Message = Macros.USER_ABORT Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Price Check Session cannot be started" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
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
        objAppContainer.objLogger.WriteAppLog("Enter PCSessionMgr HandleScanData", Logger.LogLevel.RELEASE)
        m_PChome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        Dim strTemp As String = ""
        Try
            Select Case Type
                Case BCType.EAN
                    If Not (objAppContainer.objHelper.ValidateEAN(strBarcode)) Or _
                       Val(strBarcode) = 0 Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        m_PChome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                    Else
                        m_PChome.objProdSEL.txtProduct.Text = strBarcode
                    End If
                Case BCType.ManualEntry
                    If strBarcode.Length < 8 Then
                        'strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                        If Not (objAppContainer.objHelper.ValidateBootsCode(strBarcode)) Then
                            'Handle invalid Boots Code
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                            m_PChome.objProdSEL.txtProduct.Text = ""
                            m_PChome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
                        Else
                            m_PChome.objProdSEL.txtProduct.Text = strBarcode
                        End If
                    Else
                        If (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                            m_PChome.objProdSEL.txtProduct.Text = strBarcode
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                           MessageBoxButtons.OK, _
                           MessageBoxIcon.Asterisk, _
                           MessageBoxDefaultButton.Button1)
                            m_PChome.objProdSEL.txtProduct.Text = ""
                            m_PChome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
                        End If
                    End If
                Case BCType.SEL
                    If Not (objAppContainer.objHelper.ValidateSEL(strBarcode)) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M4"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        m_PChome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                    Else
                        m_PChome.objProdSEL.txtSEL.Text = strBarcode
                    End If
            End Select
            BCReader.GetInstance.StartRead()
            PCSessionMgr.GetInstance.DisplayPCScreen(PCSessionMgr.PCSCREENS.Home)
            m_PChome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Price Check Handle Scan Data Failiure" + ex.StackTrace, Logger.LogLevel.RELEASE)
#If RF Then
             If ex.Message = Macros.CONNECTION_REGAINED Then
                If Not m_PChome Is Nothing Then
                    If Not m_PChome.objProdSEL.txtProduct.IsDisposed AndAlso Not m_PChome.objProdSEL.txtSEL.IsDisposed Then
                        m_PChome.objProdSEL.txtProduct.Text = ""
                        m_PChome.objProdSEL.txtSEL.Text = ""
                    End If

                End If
            End If
#End If
           
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PCSessionMgr HandleScanData", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Screen Display method for Price Check. 
    ''' All Price Check sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName">Enum PCSCREENS</param>
    ''' <returns>True if display is sucess else False</returns>
    ''' <remarks></remarks>
    Public Function DisplayPCScreen(ByVal ScreenName As PCSCREENS)
        Try
            Select Case ScreenName
                Case PCSCREENS.Home
                    m_PChome.Invoke(New EventHandler(AddressOf DisplayPCScan))
                Case PCSCREENS.Summary
                    m_PCSummary.Invoke(New EventHandler(AddressOf DisplayPCSummary))
            End Select
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Price Check Session Exception at DisplayPCScreen" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' This subroutine sets the home screen display
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayPCScan(ByVal o As Object, ByVal e As EventArgs)
        Dim strPdtCode As String = ""
        Dim strSELCode As String = ""
        Dim strBootsCode As String = ""
        Dim bResult As Boolean = False
        Try
            With m_PChome
                If Not (m_bPCTargetAvailable) Then
                    'If (GetPriceCheckTargetData()) Then
                    bFlag = GetPriceCheckTargetData()
                End If
                'When PC module is launched for the first time, iCompleted COunt will be 0
                'If objAppContainer.iCompletedCount = 0 Then
                'this will change in case of RF module
                If bFlag Then
                    'Integration testing
                    If m_PCTargetDetails.PriceCheckTarget() Is Nothing Then
                        .lblTargetNum.Text = "0"
                    Else
                        .lblTargetNum.Text = m_PCTargetDetails.PriceCheckTarget().ToString()
                    End If
                Else
                    MessageBox.Show("Target Price Check count not available", _
                              "Info", _
                              MessageBoxButtons.OK, _
                              MessageBoxIcon.Asterisk, _
                              MessageBoxDefaultButton.Button1)
                    'Integration Testing
                    bFlag = True
                End If
#If NRF Then
                .lblCompletedNum.Text = objAppContainer.iCompletedCount
#ElseIf RF Then
                .lblCompletedNum.Text = m_PCTargetDetails.PriceCheckCompleted.ToString()
#End If
                'Set local variables for further processing
                strPdtCode = .objProdSEL.txtProduct.Text.ToString()
                strSELCode = .objProdSEL.txtSEL.Text.ToString()
                .Refresh()
                'The label to scan SEL or Pdt code should be displayed alternatively depending
                'on which was sacnned earlier
                If strPdtCode = Nothing Then
                    .lblScanSEL.Hide()
                    .lblScanEnterPdtCode.Show()
                    iTBFlag = 1
                    .objProdSEL.txtProduct.Focus()
                    .Visible = True
                    .Refresh()
                ElseIf strSELCode = Nothing Then
                    .lblScanEnterPdtCode.Hide()
                    .lblScanSEL.Show()
                    iTBFlag = 0
                    .objProdSEL.txtSEL.Focus()
                    .Visible = True
                    .Refresh()
                Else
#If NRF Then
                    'Gets the 7  digit Boots code of the Pdt scanned
                    strBootsCode = CheckSELProductMatch(strSELCode, strPdtCode)
                    'IT Intenal - Display message if Item is not on file
                    If strBootsCode = "0" Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                        .objProdSEL.txtSEL.Text = ""
                        .objProdSEL.txtProduct.Text = ""
                        Return
                    End If
                    If strBootsCode.Trim(" ") <> "" Then
                        'If the user has entered Boots code, Pdtcode needs to be fetched
                        If Len(strPdtCode) <= 7 Then
                            strPdtCode = objAppContainer.objDataEngine.GetProductCode(strBootsCode)
                        Else
                            'remove the check digit in the product code scanned to get details
                            'from database. Trim off the last digit in the product code string.
                            strPdtCode = strPdtCode.PadLeft(13, "0")
                            strPdtCode = Mid(strPdtCode, 1, 12)
                        End If
                        If (GetPriceCheckInfo(strBootsCode, strPdtCode)) Then
                            'Changes to incorporate price check based on SEL 
                            'print flag in product group.
                            If m_PriceCheckInfo.SELPrintFlag.Equals("Y") Then
                                'Actual price check
                                If (DoFullPriceCheck(strBootsCode, strPdtCode)) Then
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M16"), "Info", _
                                                    MessageBoxButtons.OK, _
                                                    MessageBoxIcon.Asterisk, _
                                                    MessageBoxDefaultButton.Button1)
                                    SELQueuedCount += 1
                                    GetSetSELQueuedCount = SELQueuedCount
                                    If objAppContainer.bMobilePrinterAttachedAtSignon Then
                                        Dim objPSProductInfo As PSProductInfo = New PSProductInfo()
                                        objAppContainer.objDataEngine.GetProductInfoUsingPC(strPdtCode, objPSProductInfo)
                                        objPSProductInfo.LabelQuantity = 1
                                        MobilePrintSessionManager.GetInstance.CreateLabels(objPSProductInfo)
                                    Else
                                        PrintProcess(strBootsCode)
                                    End If
                                End If
                                    .lblCompletedNum.Text = objAppContainer.iCompletedCount
                                Else
                                    objAppContainer.objLogger.WriteAppLog("SEL printing not permitted for the scanned item's product groupt.", _
                                                                          Logger.LogLevel.RELEASE)
                                End If
                            Else
                                objAppContainer.objLogger.WriteAppLog("Price Check Info cannot be retrieved", Logger.LogLevel.DEBUG)
                            End If
                            'Clear all fields
                            .objProdSEL.txtSEL.Text = ""
                            .objProdSEL.txtProduct.Text = ""
                            .lblScanSEL.Hide()
                            .lblScanEnterPdtCode.Show()
                            .objProdSEL.txtProduct.Focus()
                            .Refresh()
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M21"), "Info", _
                                        MessageBoxButtons.OK, _
                                        MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
                            'Handles the label dispaly depending on what is scanned first
                            If iTBFlag = 0 Then
                                .objProdSEL.txtSEL.Text = ""
                                .objProdSEL.txtSEL.Focus()
                            Else
                                .objProdSEL.txtProduct.Text = ""
                                .objProdSEL.txtProduct.Focus()
                            End If
                        End If
#ElseIf RF Then
                    Dim objItemInfo As New ItemInfo()
                    objAppContainer.objHelper.GetBootsCodeFromSEL(strSELCode, strBootsCode)
                    strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
                    If objAppContainer.objDataEngine.GetItemDetailsAllUsingPC(strPdtCode, objItemInfo) Then
                        If (objItemInfo.BootsCode.Equals(strBootsCode)) Then
                            Dim strSELPrice As String = ""
                            objAppContainer.objHelper.GetPriceFromSEL(strSELCode, strSELPrice)
                            Dim dVariance As Decimal = ComparePrice(objItemInfo.Price, strSELPrice)
                            If (dVariance = 0) Then
                                'Price matches and already we have send a ENQ with "P"
                                'So now check the price acceptance flag
                                DoFullPriceCheck(strBootsCode, strPdtCode)
                                'Set the display variable
                                'Bug Fix 
                                'The price check count get reset when we scan a wrong SEL/ PC Combination
                                m_PCTargetDetails.PriceCheckCompleted = m_PriceCheckInfo.PCComplete
                                m_PCTargetDetails.PriceCheckTarget = m_PriceCheckInfo.PCTarget
                                .lblCompletedNum.Text = m_PCTargetDetails.PriceCheckCompleted
                                .lblTargetNum.Text = m_PCTargetDetails.PriceCheckTarget

                                .objProdSEL.txtSEL.Text = ""
                                .objProdSEL.txtProduct.Text = ""
                                .lblScanSEL.Hide()
                                .lblScanEnterPdtCode.Show()
                                .objProdSEL.txtProduct.Focus()
                                .Refresh()
                            Else
                                'Create and send PCM message here
                                'Only if PCM succeeds Go to Print record
                                If objAppContainer.objExportDataManager.CreatePCM(strBootsCode, dVariance) Then
                                    'Show message box here
                                    'price check error
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M48") + "    Current Price : " + _
                                                    cCurencySymb + " " + objItemInfo.Price.Insert(4, ".").TrimStart("0"), _
                                                    "SEL Price Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                                    MessageBoxDefaultButton.Button1)
                                    'Increasing the Print SEL Count Here
                                    'Bug Fix System testing RF STAB
                                    'The SEL Printed count is zero in the summary screen
                                    SELQueuedCount += 1
                                    GetSetSELQueuedCount = SELQueuedCount

                                    'Print SEL Here
                                    '''''to be verified
                                    'Ask Govindh to verify this 
                                    If objAppContainer.bMobilePrinterAttachedAtSignon Then
                                        Dim objPSProductInfo As PSProductInfo = New PSProductInfo()
                                        With objPSProductInfo
                                            .BootsCode = objItemInfo.BootsCode
                                            .ProductCode = objItemInfo.ProductCode
                                            .Description = objItemInfo.Description
                                            .Status = objItemInfo.Status
                                            .CurrentPrice = objItemInfo.Price
                                            .CIPFlag = objItemInfo.CIPFlag
                                            .Advantage = objItemInfo.Advantage
                                            .SupplyRoute = objItemInfo.SupplyRoute
                                        End With
                                        objAppContainer.objDataEngine.GetLabelDetails(objPSProductInfo)
                                        objPSProductInfo.LabelQuantity = 1
                                        MobilePrintSessionManager.GetInstance.CreateLabels(objPSProductInfo)
                                        'Showing the Message only after a print
                                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M16"), "Info", _
                                                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                                        MessageBoxDefaultButton.Button1)
                                    Else
                                        'Incase if in RF and mobile printer not attached send PRT request.
                                        If objAppContainer.objExportDataManager.CreatePRT(strBootsCode, _
                                                                     SMTransactDataManager.ExFileType.EXData) Then
                                            'Showing the Message only after a print
                                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M16"), "Info", _
                                                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                                            MessageBoxDefaultButton.Button1)
                                        End If
                                    End If
                                End If
                                .objProdSEL.txtSEL.Text = ""
                                .objProdSEL.txtProduct.Text = ""
                            End If
                        Else
                            'Product code and Sel Doesnot match
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M21"), "Info", _
                                           MessageBoxButtons.OK, _
                                           MessageBoxIcon.Asterisk, _
                                           MessageBoxDefaultButton.Button1)
                            'Handles the label dispaly depending on what is scanned first
                            If iTBFlag = 0 Then
                                .objProdSEL.txtSEL.Text = ""
                                .objProdSEL.txtSEL.Focus()
                            Else
                                .objProdSEL.txtProduct.Text = ""
                                .objProdSEL.txtProduct.Focus()
                            End If
                        End If
                    Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                        MessageBoxButtons.OK, _
                                        MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
                        .objProdSEL.txtSEL.Text = ""
                        .objProdSEL.txtProduct.Text = ""
                    End If
                    objItemInfo = Nothing
#End If
                    End If
                    'Sets the store id and active data time to the status bar
                    .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                    .Visible = True
                    .Refresh()
            End With
            objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PRICECHK
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            ElseIf ex.Message = Macros.CONNECTION_REGAINED Then
                If Not m_PChome Is Nothing Then
                    m_PChome.objProdSEL.txtProduct.Text = ""
                    m_PChome.objProdSEL.txtSEL.Text = ""
                End If
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Price Check - Exception in DisplayPCScan", Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' This subroutine sets the summary screen display
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayPCSummary(ByVal o As Object, ByVal e As EventArgs)
        With m_PCSummary
            .lblTitleSum.Show()
            .lblSELsQueued.Show()
            .lblNumSEL.Text = m_PCSessionMgr.GetSetSELQueuedCount()
#If NRF Then
                .lblDockAndTransmit.Show()
#ElseIf RF Then
            .lblDockAndTransmit.Visible = False
            .lblPriceCheckMessage.Visible = True
#End If


            'Sets the store id and active data time to the status bar
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by PCSessionMgr.
    ''' </summary>
    ''' <returns>True if terminate is sucess else False</returns>
    ''' <remarks></remarks>
#If NRF Then
        Public Function EndSession() As Boolean
#ElseIf RF Then
    Public Function EndSession(Optional ByVal isConnectivityLoss As Boolean = False) As Boolean
#End If
        objAppContainer.objLogger.WriteAppLog("Enter Price Check End Session", Logger.LogLevel.INFO)
        Try
#If NRF Then
            WriteExportData()
            'Set active module to none after quitting the module
            objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.NONE
#ElseIf RF Then
            If Not isConnectivityLoss Then
                'Sending PCX -Price Check END
                Dim objPCX As New PCXRecord()
                objPCX.strSELs = GetSetSELQueuedCount().ToString()
                objPCX.strCheckedItems = iPriceCheckCount.ToString()
                If Not (objAppContainer.objExportDataManager.CreatePCX(objPCX)) Then
                    objAppContainer.objLogger.WriteAppLog("Cannot Create PCX record at PC End Session", Logger.LogLevel.RELEASE)
                    Return False
                End If
            End If
#End If
            m_PChome.Dispose()
            m_PCSummary.Dispose()
            m_PCTargetDetails = Nothing
            'Release all objects and Set to nothing.
            m_PCItemList = Nothing
            m_QueuedSELList = Nothing
            m_bPCTargetAvailable = Nothing
            'Save data and perform all Exit Operations.
            'Dispose all forms.
            bIsPriceIncrease = Nothing
            m_QueuedPCMList = Nothing
            iPriceCheckCount = Nothing
            m_PCSessionMgr = Nothing
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Price Check Session End failure" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
        objAppContainer.objLogger.WriteAppLog("Exit PCSessionMgr End Session", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Retrieves the Price Check target data from database
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPriceCheckTargetData() As Boolean
        Try
            m_bPCTargetAvailable = True
            If (objAppContainer.objDataEngine.GetPCTargetDetails(m_PCTargetDetails)) Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Price Check Getting PCTarget from DB failure" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Function to retrieve Price Check Info
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <param name="strPdtCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPriceCheckInfo(ByVal strBootsCode, ByVal strPdtCode) As Boolean
        Try
            If (objAppContainer.objDataEngine.GetPriceCheckInfo(strBootsCode, strPdtCode, m_PriceCheckInfo)) Then
                Return True
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Price Check Getting PCInfo from DB failure" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    Public Property GetSetSELQueuedCount() As Integer
        Get
            Return SELQueuedCount
        End Get
        Set(ByVal value As Integer)
            SELQueuedCount = value
        End Set
    End Property
    ''' <summary>
    ''' Compares the Boots Code from Pdt Code and SEL
    ''' </summary>
    ''' <param name="strSEL"></param>
    ''' <param name="strPdtCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckSELProductMatch(ByVal strSEL As String, ByVal strPdtCode As String) As String
        'System Testing - Changed variable name
        Dim strBootsCodeFromPdtCode As String = ""
        Dim strBootsCodeFromSEL As String = ""
        Try
            'Return boots code if the user has enterd the boots code
            If Len(strPdtCode) > 7 Then
                'Enable EAN - 8 barcode check
                strPdtCode = strPdtCode.PadLeft(13, "0")
                strPdtCode = Trim(Mid(strPdtCode, 1, 12))
                strBootsCodeFromPdtCode = objAppContainer.objDataEngine.GetBootsCode(strPdtCode)
                If strBootsCodeFromPdtCode = 0 Then
                    'return nothing if fails
                    Return "0"
                End If
            Else
                strBootsCodeFromPdtCode = strPdtCode
            End If

            objAppContainer.objHelper.GetBootsCodeFromSEL(strSEL, strBootsCodeFromSEL)
            strBootsCodeFromSEL = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCodeFromSEL)
            If (String.Compare(strBootsCodeFromPdtCode, strBootsCodeFromSEL, True) = 0) Then
                Return strBootsCodeFromPdtCode
            Else
                'return nothing if fails
                Return Nothing
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Price Check SEL- Product Match" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return Nothing
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PCSessionMgr CheckSELProductMatch", Logger.LogLevel.RELEASE)
    End Function
#If NRF Then
    ''' <summary>
    ''' This function compares the SEL price with the Current price from data base / Controller
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ComparePrice() As Boolean
        Dim dcCurrentPrice As Decimal
        Dim dcSELPrice As Decimal
        Dim strSELPrice As String = ""

        dcCurrentPrice = CDec(m_PriceCheckInfo.CurrentPrice())

        objAppContainer.objHelper.GetPriceFromSEL(m_PChome.objProdSEL.txtSEL.Text.ToString(), strSELPrice)
        If Not (strSELPrice = "") Then
            dcSELPrice = CDec(strSELPrice)
        Else
            dcSELPrice = 0D
        End If
        If dcCurrentPrice = dcSELPrice Then
             Return True
        Else
             Return False
        End If
    End Function
#ElseIf RF Then
    Private Function ComparePrice(ByVal strCurrentPrice As String, ByVal strSELPrice As String) As Decimal
        Dim dcCurrentPrice As Decimal
        Dim dcSELPrice As Decimal
        'Converts the current price and SEL price to decimal
        dcCurrentPrice = CDec(strCurrentPrice)
        dcSELPrice = CDec(strSELPrice)
        'Checks if the current price is equal to price obtained from SEL
        Return (dcSELPrice - dcCurrentPrice)
    End Function
#End If
#If RF Then
    Public Function DoFullPriceCheck(ByVal strBootsCode As String, ByVal strProductCode As String) As Boolean
        Dim bTemp As Boolean = False
        Try
            'Now the product info lies in m_PC Product Info
            If GetPriceCheckInfo(strBootsCode, strProductCode) Then
                'Removed the date
                'As per the call with Govindh on 17/02/2009 
                'Remove the date because in few cases we dont get a date from the application
                If m_PriceCheckInfo.PriceAcceptedFlag = "Y" Or _
                m_PriceCheckInfo.PriceAcceptedFlag.Trim() = Nothing Then
                    bTemp = True
                    'MessageBox.Show(MessageManager.GetInstance().GetMessage("M17"), _
                    '                 "Info", _
                    '              MessageBoxButtons.OK, _
                    '              MessageBoxIcon.Asterisk, _
                    '              MessageBoxDefaultButton.Button1)
                ElseIf m_PriceCheckInfo.PriceAcceptedFlag = "N" Then
                    'Display the reject message here if the price chekc is rejected.
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M17") & " This item was last checked on " & m_PriceCheckInfo.RejectMessage.ToString(), _
                                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                    bTemp = False
                End If
            End If
            'Count the number of items.
            iPriceCheckCount += 1
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
            bTemp = False
            objAppContainer.objLogger.WriteAppLog("Error : " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
        Return bTemp
    End Function
#End If
#If NRF Then
''' <summary>
    ''' This function performs the full price check for the PC module
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DoFullPriceCheck(ByVal strBootsCode As String, ByVal strPdtCode As String) As Boolean
        Dim dcCurrentPrice As Decimal
        Dim dcPendingPrice As Decimal
        Dim dcSELPrice As Decimal
        Dim strSELPrice As String = ""
        Dim bComparePrice As Boolean = False
        Dim iCount As Integer = 0
        Try
            'Converts the values to decimal
            Try
                If Not (m_PriceCheckInfo.CurrentPrice = "") Then
                    dcCurrentPrice = CDec(m_PriceCheckInfo.CurrentPrice)
                Else
                    dcCurrentPrice = 0D
                End If
                If Not (m_PriceCheckInfo.PendingPrice = "") Then
                    dcPendingPrice = CDec(m_PriceCheckInfo.PendingPrice)
                Else
                    dcPendingPrice = 0D
                End If
                objAppContainer.objHelper.GetPriceFromSEL(m_PChome.objProdSEL.txtSEL.Text.ToString(), strSELPrice)
                If Not (strSELPrice = "") Then
                    dcSELPrice = CDec(strSELPrice)
                Else
                    dcSELPrice = 0D
                End If
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("DoFullPriceCheck - Cannot convert Price to Decimal", Logger.LogLevel.RELEASE)
            End Try

            'Comparing SEL price and current price
            bComparePrice = ComparePrice()

            If bComparePrice Then
                If dcPendingPrice = 0D Then
                    If (IsPriceCheckPeriodValid()) Then
                        'If the product is not price check in the last threshold no. of days
                        'then count it towards target and queue a ENQ record with PC flag.
                        CreateENQ(strBootsCode, "P")
                    Else
                        'If the product is price  checked then queue only ENQ without PC flag.
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M17"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        CreateENQ(strBootsCode, " ")
                    End If
                    Return False
                ElseIf dcPendingPrice > dcCurrentPrice Then
                    bIsPriceIncrease = True
                    '@Service fix - pending price check date included
                    If m_PriceCheckInfo.PendingPriceDate.Date <= Date.Now.Date Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M17"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                        If Not (IsSupervisor()) Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M18"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                        End If
                        'Create PCM, ENQ and PRT records.
                        CreatePCM(strBootsCode, dcPendingPrice, dcCurrentPrice, " ")
                        Return True
                    Else
                        CreateENQ(strBootsCode, "P")
                        Return False
                    End If   '@Service fix  end
                ElseIf dcPendingPrice < dcCurrentPrice Then
                    'This is counted towards weekly target, write PCM, ENQ set to Price Check flag
                    'and PRT records
                    '@Service fix - pending price check date included
                    If m_PriceCheckInfo.PendingPriceDate.Date <= Date.Now.Date Then
                        CreatePCM(strBootsCode, dcPendingPrice, dcCurrentPrice, "P")
                        Return True
                    Else
                        CreateENQ(strBootsCode, "P")
                        Return False
                    End If
                End If
            Else
                'PRT shhould be queued if SEL has incorrect price
                'PCM and ENQ with price check flag set. Counted towards weekly target.
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M48") + "    Current Price : " + _
                               cCurencySymb + " " + m_PriceCheckInfo.CurrentPrice.Insert(6, ".").TrimStart("0"), _
                               "SEL Price Error", MessageBoxButtons.OK, _
                               MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                'Create PCM record.
                CreatePCM(strBootsCode, strSELPrice, dcCurrentPrice, " ")
                Return True
                End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Price Check Session Exception at DoFullPriceCheck" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PCSessionMgr DoFullPriceCheck", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Function to check whether a price check has been performed within the last 28 days(configurable)
    ''' </summary>
    ''' <returns>True if period is valid and false otherwise</returns>
    ''' <remarks></remarks>
    Public Function IsPriceCheckPeriodValid() As Boolean
        Dim dtLastPC As Date
        Dim iThreshold As Integer
        Dim iDateDiff As Integer
        Dim tsSpan As TimeSpan
        Try
            iThreshold = m_PCTargetDetails.PCThreshold()
            'UT Changes: Convert date from database to required format 
            'System Testing
            'Removed if else block as the date already in correct format in database
            'If CInt(m_PriceCheckInfo.LastPriceCheckDate) = 0 Then
            '    dtLastPC = DateTime.ParseExact(ConfigDataMgr.GetInstance.GetParam( _
            '                                   ConfigKey.DEFAULT_LAST_PC_DATE).ToString(), _
            '                                   "yyyyMMdd", _
            '                                   System.Globalization.CultureInfo.CurrentCulture)
            'Else
            '    dtLastPC = DateTime.ParseExact(m_PriceCheckInfo.LastPriceCheckDate, _
            '                                   "yyyyMMdd", _
            '                                   System.Globalization.CultureInfo.CurrentCulture)
            'End If
            'Added new line
            dtLastPC = m_PriceCheckInfo.LastPriceCheckDate
            tsSpan = Now.Subtract(dtLastPC)
            iDateDiff = tsSpan.Days

            If (iDateDiff) > iThreshold Then
                Return True
            ElseIf (iDateDiff < 0) Then
                objAppContainer.objLogger.WriteAppLog("Price Check- checking PC Valid Period failure", Logger.LogLevel.RELEASE)
                Return False
            Else
                Return False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Price Check - Exception in full IsPriceCheckPeriodValid", Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PCSessionMgr IsPriceCheckPeriodvalid", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' This function creates the PCM records and queue it in the array
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreatePCM(ByVal strBootsCode As String, ByVal dcPendingPrice As Decimal, ByVal dcCurrentPrice As Decimal, ByVal strPC As String) As Boolean
        Dim objPCMData As PCMRecord = New PCMRecord()
        Dim iVariance As Integer = 0
        objPCMData.strBootscode = strBootsCode
        iVariance = dcPendingPrice - dcCurrentPrice
        objPCMData.strNumVariance = iVariance.ToString()
        objPCMData.strPriceCheck = strPC
        UpdatePCMRecordList(objPCMData)
    End Function
    ''' <summary>
    ''' This function creates the ENQ records and queue it in the array
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateENQ(ByVal strBootsCode As String, ByVal strPriceCheck As String) As Boolean
        Dim objENQData As ENQRecord = New ENQRecord()
        objENQData.strBootsCode = strBootsCode
        objENQData.strPriceCheck = strPriceCheck
        UpdatePCItemList(objENQData)
    End Function
    ''' <summary>
    ''' This function returns true if the user who has logged in is the superviser
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsSupervisor() As Boolean
        If objAppContainer.strSupervisorFlag.Equals(Macros.SUPERVISOR_YES) Then
            Return True
        Else
            Return False
        End If
    End Function

#End If

    ''' <summary>
    ''' System Testing - Export data writing for Price Check
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <remarks></remarks>
    Public Sub PrintProcess(ByVal strBootsCode As String)
        Try
            Dim objPRTData As PRTRecord = New PRTRecord()
            objPRTData.strBootscode = strBootsCode
            objPRTData.cIsMethod = Macros.PRINT_BATCH
            m_QueuedSELList.Add(objPRTData)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(" Price Check - Exception in PrintProcess ", Logger.LogLevel.RELEASE)
        End Try
    End Sub
#If NRF Then

    ''' <summary>
    ''' Update the item list for already price checked Items
    ''' </summary>
    ''' <param name="objENQData">Current Product</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdatePCItemList(ByVal objENQData As ENQRecord) As Boolean
        Try
            Dim bIsPresent As Boolean = False
            For iCount = 0 To m_PCItemList.Count - 1
                Dim objENQRecord As ENQRecord = New ENQRecord
                objENQRecord = m_PCItemList.Item(iCount)
                If objENQRecord.strBootsCode.Equals(objENQData.strBootsCode) Then
                    m_PCItemList.RemoveAt(iCount)
                    m_PCItemList.Insert(iCount, objENQData)
                    bIsPresent = True
                    Exit For
                End If
            Next
            If Not bIsPresent Then
                m_PCItemList.Add(objENQData)
                If objENQData.strPriceCheck = "P" Then
                     objAppContainer.iCompletedCount += 1
                     iPriceCheckCount += 1
                End If
            End If
            Return bIsPresent
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Price Check Session Exception at UpdateItemList" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Function


    ''' <summary>
    ''' Update the item list for already price checked Items
    ''' </summary>
    ''' <param name="objPCMData">Current Product</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdatePCMRecordList(ByVal objPCMData As PCMRecord) As Boolean
        Try
            Dim bIsPresent As Boolean = False
            For iCount = 0 To m_QueuedPCMList.Count - 1
                Dim objPCMRecord As PCMRecord = New PCMRecord
                objPCMRecord = m_QueuedPCMList.Item(iCount)
                If objPCMRecord.strBootscode.Equals(objPCMData.strBootscode) Then
                    m_QueuedPCMList.RemoveAt(iCount)
                    m_QueuedPCMList.Insert(iCount, objPCMData)
                    bIsPresent = True
                    Exit For
                End If
            Next
            If Not bIsPresent Then
                m_QueuedPCMList.Add(objPCMData)
                If objPCMData.strPriceCheck = "P" Then

                    objAppContainer.iCompletedCount += 1


                    iPriceCheckCount += 1
                End If
            End If
            Return bIsPresent
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Price Check Session Exception at UpdateItemList" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Function


    ''' <summary>
    ''' Writes the export data
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteExportData() As Boolean
        Try
            'Integration testing
            Dim objSMDataManager As SMTransactDataManager = New SMTransactDataManager()
            'Write PCS and PCX only if there is atleast one PCM record.
            If m_PCItemList.Count > 0 Or m_QueuedPCMList.Count > 0 Then
                'Write PCS record
                m_PCSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PCS")
                objAppContainer.objExportDataManager.CreatePCS()
            End If
            'List that contains only the ENQs created as part of Price check.
            If m_PCItemList.Count() > 0 Then
                'Write ENQ record
                m_PCSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing ENQ")
                For Each objENQ As ENQRecord In m_PCItemList
                    objAppContainer.objExportDataManager.CreateENQ(objENQ)
                Next
            End If
            'Write PCMs for which there is an ENQ message
            If m_QueuedPCMList.Count() > 0 Then
                For Each objPCM As PCMRecord In m_QueuedPCMList
                    m_PCSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PCM")
                    objSMDataManager.CreatePCM(objPCM)
                    Dim iReqNos As Integer = m_QueuedSELList.Count
                    For iCounter As Integer = 0 To iReqNos - 1
                        Dim objPRT As PRTRecord = m_QueuedSELList(iCounter)
                        If objPRT.strBootscode = objPCM.strBootscode Then
                            m_PCSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PRT")
                            objSMDataManager.CreatePRT(objPRT.strBootscode, SMTransactDataManager.ExFileType.EXData)
                            'Remove the item at location iCounter which is written.
                            m_QueuedSELList.RemoveAt(iCounter)
                            'exit for loop.
                            Exit For
                        End If
                    Next
                Next
            End If
            'To Queue the SELs
            If m_QueuedSELList.Count > 0 Then
                m_PCSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PRT")
                For Each objPRT As PRTRecord In m_QueuedSELList
                    objSMDataManager.CreatePRT(objPRT.strBootscode, SMTransactDataManager.ExFileType.EXData)
                Next
            End If
            'To write PCX records.
            If m_PCItemList.Count() > 0 Or m_QueuedPCMList.Count() > 0 Then
                'Create PCX record
                m_PCSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PCX")
                Dim objPCXData As PCXRecord = New PCXRecord()
                objPCXData.strSELs = GetSetSELQueuedCount().ToString()
                objPCXData.strCheckedItems = iPriceCheckCount.ToString()

                objSMDataManager.CreatePCX(objPCXData)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Price Check Session Exception at WriteExportData" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PCSessionMgr WriteExportData", Logger.LogLevel.RELEASE)
        Return True
    End Function
#End If
    ''' <summary>
    ''' Enum Class that defines all screens for Price Check module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum PCSCREENS
        Home
        Summary
    End Enum

    ''' <summary>
    ''' enumerates all scanable fields in PC Module
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum SCNFIELDS
        ProductCode
        SELCode
    End Enum
End Class
''' ***************************************************************************
''' <className>PriceCheckInfo</className>
''' <summary>Inherits ProductInfo class and defines its own members. Used as 
''' value class for holding values for Price Check.
''' </summary>
''' <author>Infosys Technologies Ltd.</author>
''' <DateModified></DateModified>
''' <remarks></remarks>
''' ***************************************************************************
Public Class PriceCheckInfo
    Inherits ProductInfo
#If NRF Then
       ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_CurrentPrice As String
    Private m_PendingPriceToday As String
    Private m_LastPriceCheckDate As String
    Private m_SELPrintFlag As String
    Private m_PendingPriceDate As Date  '@Service Fix
      ''' <summary>
    ''' Gets or sets current price of a product.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CurrentPrice() As String
        Get
            Return m_CurrentPrice
        End Get
        Set(ByVal value As String)
            m_CurrentPrice = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets pending price of a product.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PendingPrice() As String
        Get
            Return m_PendingPriceToday
        End Get
        Set(ByVal value As String)
            m_PendingPriceToday = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets Last Price Check Date of a product.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LastPriceCheckDate() As String
        Get
            Return m_LastPriceCheckDate
        End Get
        Set(ByVal value As String)
            m_LastPriceCheckDate = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or Sets SEL printing flag for the product group to which the item scanned belongs.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SELPrintFlag() As String
        Get
            Return m_SELPrintFlag
        End Get
        Set(ByVal value As String)
            m_SELPrintFlag = value
        End Set
    End Property
    Public Property PendingPriceDate() As Date
        Get
            Return m_PendingPriceDate
        End Get
        Set(ByVal value As Date)
            m_PendingPriceDate = value
        End Set
    End Property
#ElseIf RF Then
#If RF Then
    Private m_PriceAcceptedFlag As String
    Private m_Target As Integer
    Private m_completed As Integer
    Private m_RejectMessage As String

    Public Property RejectMessage() As String
        Get
            Return m_RejectMessage
        End Get
        Set(ByVal value As String)
            m_RejectMessage = value
        End Set
    End Property

    Public Property PriceAcceptedFlag() As String
        Get
            Return m_PriceAcceptedFlag
        End Get
        Set(ByVal value As String)
            m_PriceAcceptedFlag = value
        End Set
    End Property
    Public Property PCTarget() As Integer
        Get
            Return m_completed
        End Get
        Set(ByVal value As Integer)
            m_completed = value
        End Set
    End Property
    Public Property PCComplete() As Integer
        Get
            Return m_Target
        End Get
        Set(ByVal value As Integer)
            m_Target = value
        End Set
    End Property

#End If
#End If
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub


End Class
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class PCTargetDetails
    ''' <summary>
    ''' Member Variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_PCTarget As String
    Private m_PCCompleted As String
    Private m_PCThreshold As String
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub
    ''' <summary>
    ''' Gets or sets the Price Check target for a week.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PriceCheckTarget() As String
        Get
            Return m_PCTarget
        End Get
        Set(ByVal value As String)
            m_PCTarget = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the Price Check completed for the week.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PriceCheckCompleted() As String
        Get
            Return m_PCCompleted
        End Get
        Set(ByVal value As String)
            m_PCCompleted = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the threshold for the price check.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PCThreshold() As String
        Get
            Return m_PCThreshold
        End Get
        Set(ByVal value As String)
            m_PCThreshold = value
        End Set
    End Property
End Class
