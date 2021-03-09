Imports System.Linq
'''***************************************************************
''' <FileName>PLSessionMgr.vb</FileName>
''' <summary>
''' The Shelf Monitor Container Class.
''' Implements all business logic and GUI navigation for Shlef Monitor. 
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
''' MCF fix for transact restart issue in MCF.
''' </Summary>
'''****************************************************************************
Public Class SMSessionMgr
    Private m_SMhome As frmSMHome
    Private m_SMFullPriceCheck As frmSMFullPriceCheck
    Private m_SMItemDetails As frmSMItemDetails
    Private m_SMFillQuantity As frmSMFillQuantity
    Private m_SMSummary As frmSMSummary
    Private m_SMView As frmView
    Public m_ItemActioned As Boolean = False
    Private Shared m_SMSessionMgr As SMSessionMgr = Nothing
    Public PreviousScreen As SMSCREENS
    'SMProductInfo maintains the latest scanned/entered item details
    Private m_SMProductInfo As SMProductInfo = Nothing
    'SMItemList maintains all the items scanned in a shel monitor session
    Private m_SMItemList As ArrayList = Nothing
    'SMMultiSiteTracker maintains if all locations for a multi sited item is counted.
    Private m_SMMultiSiteTracker As ArrayList = Nothing
    'Private m_MSInfoForCurrentItem As ArrayList = Nothing
    Private m_SMSELQueued As ArrayList = Nothing
    Private m_arrPogList As ArrayList = Nothing
    'Handling repeat Count CR
    Private m_arrtempPOGLIst As ArrayList = Nothing
    Public m_SalesFloorQty As String = ""
    Private m_ModulePriceCheck As ModulePriceCheck
    Private m_EntryType As BCType = BCType.None
    'To Track the GAP Count
    'Private m_GapCount As Integer = 0
    'To Track the GAP Count
    Private m_SELQueued As Integer = 0
    Private m_ScannedItemCount As Integer = 0
    Private m_PreviousItem As String

    Private m_bIsFullPriceCheckRequired As Boolean = False
    Private m_SELForFullPriceCheck As String
    Private m_bIsMultisited As Boolean = False
    Private m_bIsViewed As Boolean = False
    Private m_iSeqNum As Integer = 0
    Private m_bItemScanned As Boolean = False
    'Flag to check and give a unique sequence number
    Private isNewItem As Boolean = False
    'Total item count for multisited items - SFA
    Private m_iItemCount As Integer = 0
#If RF Then
    Private bCurrentOSSR_FLAG As Boolean = False
    'Private strPreviousShelfQty As String = "0"
    'Private strPreviousFillQty As String = "0"
    ''' <summary>
    ''' Used to correct sequence number when the connection is lost, session exited
    ''' Same session is started again and the connection is regained
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SequenceNumber() As Integer
        Get
            Return m_iSeqNum
        End Get
        Set(ByVal value As Integer)
            m_iSeqNum = value
        End Set
    End Property

    Public Property SELS() As Integer
        Get
            Return m_SELQueued
        End Get
        Set(ByVal value As Integer)
            m_SELQueued = value
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

    Private SelectedPOGSeqNum As String
    Public Property m_SelectedPOGSeqNum() As String
        Get
            Return SelectedPOGSeqNum
        End Get
        Set(ByVal value As String)
            SelectedPOGSeqNum = value
        End Set
    End Property
    'Total item count for multisited items - SFA
    Public Property TotalItemCount() As Integer
        Get
            Return m_iItemCount
        End Get
        Set(ByVal value As Integer)
            m_iItemCount = value
        End Set
    End Property

    Private Sub New()
        Try
            'StartSession is called when module starts and not from constructor
            'Me.StartSession()
        Catch ex As Exception
            'Handle SM Init Exception here.
            'Me.EndSession()
            objAppContainer.objLogger.WriteAppLog("Cannot Initialise SM Session Manager" + ex.StackTrace, _
                                                 Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Functions for getting the object instance for the SMSessionMgr. 
    ''' Use this method to get the object refernce for the Singleton SMSessionMgr.
    ''' </summary>
    ''' <returns>An Object of SMSessionMgr</returns>
    ''' <remarks>Object reference of SMSessionMgr Class</remarks>
    Public Shared Function GetInstance() As SMSessionMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.SHLFMNTR
        If m_SMSessionMgr Is Nothing Then
            m_SMSessionMgr = New SMSessionMgr()
            Return m_SMSessionMgr
        Else
            Return m_SMSessionMgr
        End If

    End Function
#If RF Then
    Public ReadOnly Property ISPostNext() As Boolean
        Get
            If (Not m_SMItemList Is Nothing) AndAlso (m_SMItemList.Count > 0) Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    Public ReadOnly Property IsFirstItemActioned() As Boolean
        Get
            If (m_SMItemList.Count >= 1) Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
#End If


    ''' <summary>
    ''' Initialises a Shelf Monitor Session 
    ''' Creates all associated objects and Forms
    ''' </summary>
    ''' <remarks>Executed at the start of every SM session</remarks>
    Public Sub StartSession()
#If NRF Then
        InitialiseSM ()
        'Display the home screen
        DisplaySMScreen(SMSessionMgr.SMSCREENS.Home)
#ElseIf RF Then
        'anoop: Send a GAS message
        If Not (objAppContainer.objExportDataManager.CreateGAS()) Then
            objAppContainer.objLogger.WriteAppLog("Cannot Create GAS record at SM Start Session", _
                                                  Logger.LogLevel.RELEASE)
            Exit Sub
        Else
            'Send a PGS message
            If Not (objAppContainer.objExportDataManager.CreatePGS()) Then
                objAppContainer.objLogger.WriteAppLog("Cannot Create PGS record at SM Start Session", _
                                                      Logger.LogLevel.RELEASE)
                Exit Sub
            End If
            'Initialise the SM session after getting a GAR 
            InitialiseSM()
            'Display the home screen
            DisplaySMScreen(SMSessionMgr.SMSCREENS.Home)
        End If
#End If
    End Sub
#If RF Then
    ''' <summary>
    ''' Updates the Status bar of all the forms in the session manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBarMessage()
        Try
            m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SMFullPriceCheck.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SMItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SMFillQuantity.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SMSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SMView.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured, Trace: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

#End If
    ''' <summary>
    ''' Initialises the SM session
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub InitialiseSM()
        'Do all shelf monitor realated Initialisations here.
        m_SMhome = New frmSMHome()
        m_SMFullPriceCheck = New frmSMFullPriceCheck()
        m_SMItemDetails = New frmSMItemDetails()
        m_SMFillQuantity = New frmSMFillQuantity()
        m_SMSummary = New frmSMSummary()
        m_SMView = New frmView()
        m_ModulePriceCheck = New ModulePriceCheck()
        m_PreviousItem = ""
        m_SMSELQueued = New ArrayList()
        m_SMProductInfo = New SMProductInfo()
        m_SMItemList = New ArrayList()
        m_SMMultiSiteTracker = New ArrayList()
        m_arrPogList = New ArrayList()
        m_arrtempPOGLIst = New ArrayList()
    End Sub
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by SMSessionMgr.
    ''' </summary>
    ''' <returns>True if terminate is sucess else False</returns>
    ''' <remarks></remarks>
#If NRF Then
        Public Function EndSession() As Boolean
#ElseIf RF Then
    Public Function EndSession(Optional ByVal isConnectivityLoss As Boolean = False) As Boolean
#End If
        Try
            'Added condition for RF & NRF.
#If NRF Then
            If (WriteExportData()) Then
                'Set active module to none after quitting the module
                objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.NONE
                objAppContainer.objLogger.WriteAppLog("Export data for SM written succesfully.", _
                                                      Logger.LogLevel.RELEASE)
            Else
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M75"), _
                                "Data Write Error", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
            End If
#ElseIf RF Then
            'Send GAX only if Connectivity is not lost
            If Not isConnectivityLoss Then
                'Sent GAX record for RF mode

                Dim objGAXRecord As GAXRecord = GenerateGAX()
                'Send PGX to end planner session
                If Not (objAppContainer.objExportDataManager.CreatePGX()) Then
                    objAppContainer.objLogger.WriteAppLog("Cannot Create PGX record at SM Start Session", _
                                                          Logger.LogLevel.RELEASE)
                    'v1.1 Commenting this for TRANSACT restart issue in MCF
                    'Return False
                End If
                If Not (objAppContainer.objExportDataManager.CreateGAX(objGAXRecord)) Then
                    objAppContainer.objLogger.WriteAppLog("Cannot Create GAX record at SM End Session", _
                                                          Logger.LogLevel.RELEASE)
                    'v1.1 Commenting this for TRANSACT restart issue in MCF
                    'Return False
                End If
            End If
            m_ModulePriceCheck = Nothing
#End If

            'Close and Dispose all forms.
            m_SMView.Dispose()
            m_SMFullPriceCheck.Dispose()
            m_SMItemDetails.Dispose()
            m_SMFillQuantity.Dispose()
            m_SMhome.Dispose()
            m_SMSummary.Dispose()

            'Release all objects and Set to nothig.
            m_SMSELQueued = Nothing
            m_bItemScanned = Nothing
            m_SMhome = Nothing
            m_SMItemDetails = Nothing
            m_SMFillQuantity = Nothing
            m_SMSummary = Nothing
            m_SMView = Nothing
            m_SMProductInfo = Nothing
            m_SMItemList = Nothing
            m_SMMultiSiteTracker = Nothing
            m_arrPogList = Nothing
            m_arrtempPOGLIst = Nothing
            m_ModulePriceCheck = Nothing
            m_SELQueued = Nothing
            m_ScannedItemCount = Nothing
            SelectedPOGSeqNum = Nothing
            m_bIsFullPriceCheckRequired = Nothing
            m_bIsMultisited = Nothing
            m_SMSessionMgr = Nothing
            m_iSeqNum = Nothing
            m_PreviousItem = Nothing
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("SM End Session failed" + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' GenerateGAX record generates the GAX for the currently active session
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GenerateGAX() As GAXRecord
        Dim objGAXRecord As GAXRecord
        Try
            objGAXRecord = New GAXRecord()
            objGAXRecord.strPickListItems = GetPickListCount()
            objGAXRecord.strSELS = m_SELQueued
            objGAXRecord.strPriceChk = m_ModulePriceCheck.GetPCCountForCurrentSession()
            Return objGAXRecord
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occured @:" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return Nothing
        Finally
            objGAXRecord = Nothing
        End Try
    End Function
    ''' <summary>
    ''' The Method handles scan the scane data returned form the barcode scanner.
    ''' This method implements the business logic to populate the data to the corresponding
    ''' UI element after validation.
    ''' </summary>
    ''' <param name="strBarcode">Raw Barocode returned by the BCReader Class</param>
    ''' <param name="Type">Symbology of the barocde mapped to the Boots BC types</param>
    ''' <remarks></remarks>
    Public Sub HandleScanData(ByVal strBarcode As String, ByVal Type As BCType)
        Try
            Dim objProductInfo As SMProductInfo = New SMProductInfo
            Dim objPSProductInfo As PSProductInfo = New PSProductInfo()
            'Naveen
            ''''Bug HAs to be fixed
            'Setting app Module back to the original module
            objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.SHLFMNTR
            isNewItem = False
            m_ItemActioned = False
            m_EntryType = Type
            m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            m_bIsMultisited = False
            'Total item count for multisited items - SFA
            m_iItemCount = 0
            Select Case Type
                Case BCType.ManualEntry
                    Dim strBootsCode As String = ""

                    If strBarcode.Length < 8 Then
                        'strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                        ''TO be confirmed whether to consider Boots code entered for performing
                        ''full price check for an item.
                        'strBarcode = strBarcode.Substring(0, 6)
                        'strBarcode = strBarcode.PadLeft(12, "0")
                        'Integration Testing
                        If objAppContainer.objHelper.ValidateBootsCode(strBarcode) Then
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBarcode, objProductInfo)) Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                                MessageBoxButtons.OK, _
                                                MessageBoxIcon.Asterisk, _
                                                MessageBoxDefaultButton.Button1)
#If NRF Then
                                m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#ElseIf RF Then
                                m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
#End If
                                Return
                            End If
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                           MessageBoxButtons.OK, _
                                           MessageBoxIcon.Asterisk, _
                                           MessageBoxDefaultButton.Button1)
#If NRF Then
                            m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#ElseIf RF Then
                            m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
#End If
                            Return
                        End If
                        'full price check for an item.
                        strBarcode = strBarcode.Substring(0, 6)
                        strBarcode = strBarcode.PadLeft(12, "0")
                    Else
                        If (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                            strBarcode = strBarcode.PadLeft(13, "0")
                            'anoop:Start
#If NRF Then
                        strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
#End If
                            'anoop:End
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, objProductInfo)) Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                                m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                Return
                            End If
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                           MessageBoxButtons.OK, _
                           MessageBoxIcon.Asterisk, _
                           MessageBoxDefaultButton.Button1)
                            m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
                        End If
                    End If
                    'Integration Testing
                    If (m_bIsFullPriceCheckRequired) Then
                        'If (strBarcode = m_SMProductInfo.ProductCode) Or (strBootsCode = m_SMProductInfo.BootsCode) Or (strBarcode = m_SMProductInfo.FirstBarcode) Then
                        If objAppContainer.objDataEngine.ValidateUsingPCAndBC(m_PreviousItem, strBarcode) Then
#If NRF Then
                        If Not (m_ModulePriceCheck.DoFullPriceCheck(m_SELForFullPriceCheck, objProductInfo.ProductCode)) Then
                            'Price change in Full Price check
                            objProductInfo.iSELQueued += 1
                            'Update SEL Queued Count
                            m_SELQueued = m_SELQueued + 1
                            QueueSELPrintRequest(objProductInfo.BootsCode)
                        End If
#ElseIf RF Then
                            '*****Only valiation of SEL against product code is needed
                            '*****No other verification is needed because an "ENQ " with "C" is already send.

                            ''Full Price check to be done.
                            ''But no print is needed
                            'm_ModulePriceCheck.DoFullPriceCheck(objProductInfo.ProductCode)
                            ''    'Price change in Full Price check
                            ''    objProductInfo.iSELQueued += 1
                            ''    'Update SEL Queued Count
                            ''    m_SELQueued = m_SELQueued + 1
                            ''    QueueSELPrintRequest(objProductInfo.BootsCode)
                            ''    'Work on Queing the SEL print request
                            ''End If
#End If

                            'Fix for manual entry of bootscode in fullprice check screen
                            m_bIsFullPriceCheckRequired = False
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M47"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                            m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
                        End If
                    End If
                Case BCType.EAN
                    If Not (objAppContainer.objHelper.ValidateEAN(strBarcode)) Or _
                       Val(strBarcode) = 0 Then
                        'TODO : Change message
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        Return
                    Else
                        'anoop:Start
#If NRF Then
                    strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
#End If
                        'anoop:End
                        'Fix for the PRT record with all 0s as boots code.
                        If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, objProductInfo)) Then
                            'DARWIN checking if the base barcode is present in Database/Conroller
                            If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                                'DARWIN converting database to Base Barcode
                                strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                                If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, objProductInfo)) Then
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                                    MessageBoxButtons.OK, _
                                                    MessageBoxIcon.Asterisk, _
                                                    MessageBoxDefaultButton.Button1)
                                    m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                    Return
                                End If
                            Else
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                                MessageBoxButtons.OK, _
                                                MessageBoxIcon.Asterisk, _
                                                MessageBoxDefaultButton.Button1)
                                m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                Return
                            End If
                        End If
                        'End fix for PRT record with all 0s as boots code.
                        If (m_bIsFullPriceCheckRequired) Then
                            'If (strBarcode = m_SMProductInfo.ProductCode) Or (strBarcode = m_SMProductInfo.FirstBarcode) Then
                            If objAppContainer.objDataEngine.ValidateUsingPCAndBC(m_PreviousItem, strBarcode) Then
#If NRF Then
                                If Not (m_ModulePriceCheck.DoFullPriceCheck(m_SELForFullPriceCheck, objProductInfo.ProductCode)) Then
                                    'Price change in Full Price check
                                    objProductInfo.iSELQueued += 1
                                    'Update SEL Queued Count
                                    m_SELQueued = m_SELQueued + 1
                                    'Support Bug Fix.
                                    'Queue SEL
                                    QueueSELPrintRequest(objProductInfo.BootsCode)
                                End If
                                '#ElseIf RF Then
                                '*****Only valiation of SEL against product code is needed
                                '*****No other verification is needed because an "ENQ " with "C" is already sent.
                                'm_ModulePriceCheck.DoFullPriceCheck(strBarcode)
#End If
                                m_bIsFullPriceCheckRequired = False
                            Else
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M47"), "Info", _
                                                MessageBoxButtons.OK, _
                                                MessageBoxIcon.Asterisk, _
                                                MessageBoxDefaultButton.Button1)
                                m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                'Reset the Barcode Symbology Type.
                                m_EntryType = BCType.SEL
                                Return
                            End If
                        End If
                    End If
                    m_SELForFullPriceCheck = ""
                Case BCType.SEL
                    Dim strBootsCode As String = Nothing
                    'Validate the SEL for CDV
                    If Not (objAppContainer.objHelper.ValidateSEL(strBarcode)) Then
                        'if SEL is not valid.
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M4"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        Return
                    End If
                    'Get the Boots Code from SEL
                    Try
                        objAppContainer.objHelper.GetBootsCodeFromSEL(strBarcode, strBootsCode)
                    Catch ex As Exception
                        objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception occured in GetBootsCodeFromSEL of SMSessionMgr. Exception is: " _
                                                      + ex.StackTrace, Logger.LogLevel.RELEASE)
                    End Try
                    'Generate the Boots Code with CDV and proceed with functionalities if ture
                    strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBootsCode, objProductInfo)) Then
                        'if product info is not obtained from database/ Controller.
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                        MessageBoxButtons.OK, _
                                        MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
                        m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        Return
                    Else
                        Dim strTemp As String
#If NRF Then
                        strTemp = m_ModulePriceCheck.DoPartialPriceCheck(objProductInfo.ProductCode, strBarcode)
#ElseIf RF Then
                        strTemp = m_ModulePriceCheck.DoPartialPriceCheck(strBarcode, objProductInfo.Price)
#End If
                        If strTemp.Equals("1") Then
#If NRF Then
                            'No Price Change in Partial Price check. Check for Full Price Check
                            If (m_ModulePriceCheck.IsFullPriceCheckRequired(False)) Then
#ElseIf RF Then
                            'In case of RF a Full price check screen has to be displayed only if 
                            '1. Price accepted flag is "Y" and the price check completed count should be less that taget
                            If (m_ModulePriceCheck.IsFullPriceCheckRequired(objProductInfo.BootsCode, objProductInfo.ProductCode)) Then
#End If
                                'Do Full Price check
                                'Here the objective of setting full price flag to true is to show the 
                                'Full price check screen
                                'Validation of SEL against product code is alone done in
                                ' full price check screen
                                m_bIsFullPriceCheckRequired = True
                                objProductInfo.iSELQueued = 0
                                m_PreviousItem = strBootsCode
                                m_SELForFullPriceCheck = strBarcode
                                m_SMProductInfo = objProductInfo
                                'UAT fix for incrementing the scanned items if
                                'Full price check is not performed in FPC screen.
                                m_bItemScanned = True
                                'UAT Fix End.
                            Else
                                'Price change in Partial Price check
                                objProductInfo.iSELQueued = 0
                            End If
                        ElseIf strTemp.Equals("0") Then
                            objProductInfo.iSELQueued += 1
                            'Support Fix
                            'Queue SEL
                            If objAppContainer.bMobilePrinterAttachedAtSignon Then
#If NRF Then
                                objAppContainer.objDataEngine.GetProductInfoUsingPC(objProductInfo.ProductCode, _
                                                                                    objPSProductInfo)
#ElseIf RF Then
                                With objPSProductInfo
                                    .BootsCode = objProductInfo.BootsCode
                                    .ProductCode = objProductInfo.ProductCode
                                    .Description = objProductInfo.Description
                                    .Status = objProductInfo.Status
                                    .CurrentPrice = objProductInfo.Price
                                    .CIPFlag = objProductInfo.CIPFlag
                                    .Advantage = objProductInfo.Advantage
                                    .SupplyRoute = objProductInfo.SupplyRoute
                                End With
                                objAppContainer.objDataEngine.GetLabelDetails(objPSProductInfo)
#End If
                                objPSProductInfo.LabelQuantity = 1
                                MobilePrintSessionManager.GetInstance.CreateLabels(objPSProductInfo)
                                objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.SHLFMNTR
                            Else
#If NRF Then
                                QueueSELPrintRequest(objProductInfo.BootsCode)
#ElseIf RF Then
                                'Incase if in RF and mobile printer not attached send PRT request.
                                objAppContainer.objExportDataManager.CreatePRT(objProductInfo.BootsCode, _
                                                                               SMTransactDataManager.ExFileType.EXData)
#End If
                            End If
                            '#If RF Then
                            'In RF Replacement SEL message is not shown in module price check.
                            'This is because this has to be shown only after printing the SEL's
                            'So , after printing the SEL using mobile printer or Sending PRT to the controller - showing the message in RF
                            'MessageBox.Show(MessageManager.GetInstance().GetMessage("M16"), "Replace SEL", _
                            'MessageBoxButtons.OK, _
                            'MessageBoxIcon.Asterisk, _
                            'MessageBoxDefaultButton.Button1)
                            '#End If
                            'Update SEL Queued Count or printed count
                            m_SELQueued = m_SELQueued + 1
                        End If
                    End If
            End Select
            Try
                'Check if the item is already scanned in this session
                Dim objExistingItem As SMProductInfo = Nothing
                If (GetProductFromList(objProductInfo.BootsCode, objExistingItem)) Then
                    'Check if the item is Multisited
                    m_ItemActioned = True
                    If (objExistingItem.iMultisiteCnt > 0) Then
                        ProcessExistingItem(objExistingItem)
                        objProductInfo = objExistingItem
                    Else
                        objProductInfo = objExistingItem
                        m_arrPogList.Clear()
                    End If
                Else
                    'Item not already scanned
                    'Get the site count for the Item from the DB
                    m_arrPogList.Clear()
                    isNewItem = True
                    'CR for Repeat Count
                    m_arrtempPOGLIst.Clear()
                    Dim RepeatCount As Integer = 0
                    GetMultiSiteLocations(objProductInfo.BootsCode, m_arrtempPOGLIst)
                    objProductInfo.Sequence = m_iSeqNum + 1
                    For Each ObjPlanner As PlannerInfo In m_arrtempPOGLIst
                        RepeatCount = Convert.ToInt16(ObjPlanner.RepeatCount.ToString().Trim())
                        For Counter As Integer = 1 To RepeatCount
                            m_arrPogList.Add(ObjPlanner)
                        Next
                    Next
                    'Check if the item is mulltisited. 
                    If (m_arrPogList.Count > 1) Then
                        objProductInfo.iMultisiteCnt = m_arrPogList.Count
                        objProductInfo.MSFlag = "Y"
                        m_bIsMultisited = True
                        PopulateMultiSites(m_arrPogList, m_SMProductInfo.ProductCode)
                        ''''Auto Fill Quantity
                        'The Following Code is added to handle an item present in boots code table and no corresponding module Id present.
                        'This Condition differentiates a single sited item from item with no Module ID
                    ElseIf m_arrPogList.Count = 1 Then
                        objProductInfo.iMultisiteCnt = 0
                        objProductInfo.MSFlag = "  "
                        m_bIsMultisited = False
                    Else
                        'DEFECT FIX - BTCPR00004135 PPC - item not on planner - system should not 
                        'present the Information message 'Item Not On Planner'
#If NRF Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                   MessageBoxButtons.OK, _
                                   MessageBoxIcon.Asterisk, _
                                   MessageBoxDefaultButton.Button1)

                    m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                    Return
#End If
                    End If
                End If
                m_SMProductInfo = objProductInfo
                If (m_SMProductInfo.strFillQuantiy = "0") Then
                    m_SMProductInfo.strFillQuantiy = ""
                End If
                'Disposing the Local variables
                objProductInfo = Nothing
                'Integration Testing - Flag to indicate that an item was scanned
                m_bItemScanned = True
                If m_bIsFullPriceCheckRequired Then
                    DisplaySMScreen(SMSCREENS.FullPriceCheck)
                Else
                    DisplaySMScreen(SMSCREENS.ItemDetails)
                End If
                m_SMhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            Catch ex As Exception
#If RF Then
                If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                    Throw ex
                End If
#End If
                objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception occured in HandleScanData of SMSessionMgr. Exception is: " _
                                                     + ex.StackTrace, Logger.LogLevel.RELEASE)
            End Try
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception occured in HandleScanData of SMSessionMgr. Exception is: " _
                                                      + ex.Message + " Connection Lost", Logger.LogLevel.RELEASE)
            Else
                objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception occured in HandleScanData of SMSessionMgr. Exception is: " _
                                                      + ex.Message, Logger.LogLevel.RELEASE)
            End If
#ElseIf NRF Then
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception occured in HandleScanData of SMSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
#End If
        Finally
            If Not m_SMhome Is Nothing Then
                If Not m_SMhome.cuscntrlProdSEL.txtSEL.IsDisposed AndAlso Not m_SMhome.cuscntrlProdSEL.txtProduct.IsDisposed Then
                    m_SMhome.cuscntrlProdSEL.txtSEL.Text = ""
                    m_SMhome.cuscntrlProdSEL.txtProduct.Text = ""
                End If
            End If
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SMSessionMgr HandleScanData", Logger.LogLevel.RELEASE)
    End Sub
#If NRF Then
     Private Function QueueSELPrintRequest(ByVal strBootsCode As String) As Boolean
    'if MobilePrintSessionManager.GetInstance.MobilePrinterStatus Then

        Dim objPRTData As PRTRecord = New PRTRecord()
        Try
            'objPRTData.strBootscode = objAppContainer.objHelper.GeneratePCwithCDV(m_IIProductInfo.ProductCode)
            objPRTData.strBootscode = strBootsCode
            objPRTData.cIsMethod = Macros.PRINT_BATCH
            m_SMSELQueued.Add(objPRTData)
            Return True
        Catch ex As Exception
            Return False
        Finally
            objPRTData = Nothing
        End Try
    End Function
#End If
    ''' <summary>
    ''' Process the item if its already scanned item
    ''' </summary>
    ''' <param name="objProductInfo">Current Item Object</param>
    ''' <remarks></remarks>
    Private Sub ProcessExistingItem(ByVal objProductInfo As SMProductInfo)
        objAppContainer.objLogger.WriteAppLog("Enter SMSessionMgr ProcessExistingItem", Logger.LogLevel.RELEASE)
        Try
            'If (objProductInfo.iMultisiteCnt > 0) Then
            m_bIsMultisited = True
            m_SelectedPOGSeqNum = Nothing
            'Clear the POG list
            m_SMItemDetails.cmbbxMltSites.Items.Clear()
            m_SMFillQuantity.cmbbxMltSites.Items.Clear()
            'Get Multisite list from tracker.
            Dim objMSList As ArrayList = New ArrayList
            GetExistingMultiSiteLocations(objProductInfo.ProductCode, objMSList)

            If (m_arrPogList.Count = 0) Then
                m_arrPogList = objMSList
            End If
            'objProductInfo.iMultisiteCnt = objMSList.Count

            'Get Existing shelf and Fill Quantity for selected Module
            m_SMItemDetails.cmbbxMltSites.Items.Add("SELECT")
            m_SMFillQuantity.cmbbxMltSites.Items.Add("SELECT")
            For Each objPOG As SMMultiSiteInfo In objMSList
                'Populate existing POGs to list.
                If ((objPOG.bIsCounted = True) And (objPOG.strPOGDescription <> "OTHER")) Then
                    m_SMItemDetails.cmbbxMltSites.Items.Add("*" & (objPOG.strPlannerDesc.ToString.Trim() & "-" & objPOG.strPOGDescription.ToString.Trim() & " (Counted)"))
                    m_SMFillQuantity.cmbbxMltSites.Items.Add("*" & (objPOG.strPlannerDesc.ToString.Trim() & "-" & objPOG.strPOGDescription.ToString.Trim() & " (Counted)"))
                Else
                    m_SMItemDetails.cmbbxMltSites.Items.Add(objPOG.strPlannerDesc.ToString.Trim() & "-" & objPOG.strPOGDescription.ToString.Trim())
                    m_SMFillQuantity.cmbbxMltSites.Items.Add(objPOG.strPlannerDesc.ToString.Trim() & "-" & objPOG.strPOGDescription.ToString.Trim())
                End If
            Next
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception occured in ProcessExistingItem of SMSessionMgr." + ex.StackTrace(), _
                                                  Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SMSessionMgr ProcessExistingItem", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Retrieves the lists of products from the SMProductInfo list.
    ''' </summary>
    ''' <returns>SMproductInfo List on Success and NULL on error</returns>
    ''' <remarks></remarks>
    Private Function GetProductList() As ArrayList
        Return m_SMItemList
    End Function
    ''' <summary>
    ''' Retrives the product Info from the SMsessionMgr List
    ''' </summary>
    ''' <param name="strBootsCode">Boots Code</param>
    ''' <param name="objSMProductInfo">Reference of SMproductInfo object</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetProductFromList(ByVal strBootsCode As String, ByRef objSMProductInfo As SMProductInfo) As Boolean
        Try
            For Each objItemInfo As SMProductInfo In m_SMItemList
                If (strBootsCode = objItemInfo.BootsCode) Then
                    objSMProductInfo = objItemInfo
                    Return True
                End If
            Next
            Return False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception occured in GetProductFromList of SMSessionMgr." + ex.StackTrace(), _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Retrives the product Info from the SMsessionMgr List
    ''' </summary>
    ''' <param name="strBootsCode">Boots Code</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProductFromList(ByVal strBootsCode As String) As Boolean
        Try
            For Each objItemInfo As SMProductInfo In m_SMItemList
                If (strBootsCode = objItemInfo.BootsCode) Then
                    'Check if the item is Multisited
                    If (objItemInfo.iMultisiteCnt > 0) Then
                        ProcessExistingItem(objItemInfo)
                        m_SMProductInfo = objItemInfo
                    Else
                        m_SMProductInfo = objItemInfo
                        m_arrPogList.Clear()
                    End If
                    Return True
                End If
            Next
            Return False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception occured in GetProductFromList of SMSessionMgr." + ex.StackTrace(), _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    Private Function IsItemAlreadyScanned(ByVal strProductCode As String) As Boolean
        Try
            For Each objItemInfo As SMProductInfo In m_SMItemList
                If (strProductCode = objItemInfo.ProductCode) Then
                    Return True
                End If
            Next
            Return False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception occured in IsItemAlreadyScanned of SMSessionMgr." + ex.StackTrace(), _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Update the item list for already scanned Items
    ''' </summary>
    ''' <param name="objProductInfo">Current Product</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateItemList(ByVal objProductInfo As SMProductInfo) As Boolean
        'Check if the item is already existing in the array list. 
        'If YES update the object else add new item to list.
        objAppContainer.objLogger.WriteAppLog("Enter SMSessionMgr UpdateItemList", Logger.LogLevel.RELEASE)
        Dim iIndex As Integer = 0
        Dim isActioned As Boolean = False
        Try
            'Check if the item scanned is already present in the scanned item list.
            For Each objExistingItem As SMProductInfo In m_SMItemList
                If (objExistingItem.ProductCode = objProductInfo.ProductCode) Then
                    If (m_bIsMultisited) Then
                        objProductInfo.strFillQuantiy = GetTotalFillQnty(objExistingItem.ProductCode)
                        objProductInfo.strShelfQuantiy = GetTotalShelfQnty(objExistingItem.ProductCode)
                        'Add the item to array list.
                        m_SMItemList.RemoveAt(iIndex)
                        m_SMItemList.Insert(iIndex, objProductInfo)
                    Else
                        m_SMItemList.RemoveAt(iIndex)
                        m_SMItemList.Insert(iIndex, objProductInfo)
                        'objExistingItem = objProductInfo
                    End If
                    '#If RF Then
                    '                    'Anoop: Send Gap Start 
                    '                    'Naveen: Removed this code from PRocess fill quantity to here
                    '                    'Naveen:Send GAP - Have added call to process gap to send gap for items which are updated
                    '                    'Done as a fix for sequence number
                    '                    'if the item is added newly or updated
                    '                    ProcessGAP(objProductInfo, True)
                    '#End If
                    Return True
                End If
                iIndex = iIndex + 1
            Next
            'Add new item if not already in list.
            m_SMItemList.Add(objProductInfo)
            m_SalesFloorQty = ""
            '#If RF Then
            '            'Anoop: Send Gap Start 
            '            'Naveen: Removed this code from PRocess fill quantity to here
            '            'Naveen:Send GAP - Have added call to process gap to send gap for 
            '            'Done as a fix for sequence number
            '            'if the item is added newly or updated
            '            ProcessGAP(objProductInfo, True)
            '#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception occured in UpdateItemList of SMSessionMgr." + ex.StackTrace(), _
                                                  Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
          ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return 0
            End If
#End If
            Return False
        Finally

        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SMSessionMgr UpdateItemList", Logger.LogLevel.RELEASE)
    End Function
#If NRF Then
    ''' <summary>
    ''' Write the export data to the export data file at end session.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function WriteExportData() As Boolean
        '@Service Fix To check valid items in list
        Dim m_bIsValidItemPresent As Boolean = False
        Dim iInvalidItemCount As Integer = 0
        Try
            ''@Service Fix for autologoff
            ''Set IsAllSitesCounted to true if all sites counted for an item
            For Each objItemInfo As SMProductInfo In m_SMItemList
                If Microsoft.VisualBasic.Val(objItemInfo.iMultisiteCnt) > 0 Then
                    If (IsAllSitesCounted(objItemInfo.ProductCode)) Then
                        objItemInfo.IsAllSitesCounted = True
                        m_bIsValidItemPresent = True
                    Else
                        iInvalidItemCount = iInvalidItemCount + 1
                        objItemInfo.IsAllSitesCounted = False
                    End If
                Else
                    objItemInfo.IsAllSitesCounted = True
                    m_bIsValidItemPresent = True
                End If
            Next
            'Set the current item sctioned flag to False.
            m_ItemActioned = False
            'Write GAS and GAX only if there is atleast one GAP record.
            If m_SMItemList.Count > 0 Then
                'Write GAS record
                '@Service Fix - SEnd GAS only if valid item present
                If m_bIsValidItemPresent Then
                    m_SMSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing GAS")
                    objAppContainer.objExportDataManager.CreateGAS()
                End If

                Dim iCount As Integer = 0
                Dim objPRTRecord As PRTRecord
                Dim dforTimeStamp As Date = DateAndTime.Now
                'Reset sequence number variable to 0.
                m_iSeqNum = 1
                For Each objExistingItem As SMProductInfo In m_SMItemList
                    If objExistingItem.IsAllSitesCounted Then '@Service Fix
                        'Write GAP records
                        m_SMSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing GAP")
                        'Write GAP record to export data.
                        ProcessGAP(objExistingItem)
                    End If  '@Service Fix
                    'Insert price check records.
                    m_ModulePriceCheck.WriteExportData(objExistingItem.BootsCode)
                    iCount = 0
                    'Writing SEL Print Request Adjacent to the GAP Record.
                    'Fix for PRT record - Support Bug Fix
                    If m_SMSELQueued.Count > 0 Then
                        While (iCount < m_SMSELQueued.Count)
                            objPRTRecord = m_SMSELQueued(iCount)
                            If (objPRTRecord.strBootscode = objExistingItem.BootsCode) Then
                                'Insert PRT recoreds where required
                                m_SMSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PRT")
                                objAppContainer.objExportDataManager.CreatePRT(objPRTRecord.strBootscode, _
                                                                               SMTransactDataManager.ExFileType.EXData)
                                m_SMSELQueued.RemoveAt(iCount)
                            Else
                                iCount = iCount + 1
                            End If
                        End While
                    End If
                Next
                'Fix for PRT record - Support Bug Fix
                If (m_SMSELQueued.Count > 0) Then
                    'For SELs without Corresponding SM Session
                    For Each objPRTRecord In m_SMSELQueued
                        'Insert PRT recoreds where required
                        m_SMSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PRT")
                        objAppContainer.objExportDataManager.CreatePRT(objPRTRecord.strBootscode, _
                                                                       SMTransactDataManager.ExFileType.EXData)
                    Next
                End If
                objAppContainer.objLogger.WriteAppLog("The Time For Export Data Writing is " + (DateAndTime.Now - dforTimeStamp).Minutes.ToString() + "Minutes", Logger.LogLevel.INFO)

                'Write tthe GAX record here
                If m_bIsValidItemPresent Then '@Service Fix
                    m_SMSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing GAX")
                    Dim GAXRecordData As GAXRecord = New GAXRecord()
                    GAXRecordData.strPickListItems = (GetPickListCount() - iInvalidItemCount)
                    GAXRecordData.strPriceChk = m_ModulePriceCheck.GetPCCountForCurrentSession()
                    GAXRecordData.strSELS = GetSELQueuedCount()

                    objAppContainer.objExportDataManager.CreateGAX(GAXRecordData)
                End If
            ElseIf ((m_SMItemList.Count = 0) And (m_SMSELQueued.Count > 0)) Then
                m_SMSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing INS")
                objAppContainer.objExportDataManager.CreateINS()
                For Each objPRTRecord As PRTRecord In m_SMSELQueued
                    'Insert PRT recoreds where required
                    m_SMSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PRT")
                    objAppContainer.objExportDataManager.CreatePRT(objPRTRecord.strBootscode, _
                                                                   SMTransactDataManager.ExFileType.EXData)
                Next
                objAppContainer.objExportDataManager.CreateINX()
                m_SMSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing INX")
            End If
            'Integration testing
            m_ModulePriceCheck.EndSession()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Processing Shelf Monitor Session Data Failed." + ex.StackTrace(), _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
#End If
    ''' <summary>
    ''' Retrieves the total items scanned during the shelf monitor session
    ''' </summary>
    ''' <returns>Number of Scanned Items </returns>
    ''' <remarks></remarks>
    Private Function GetScannedProductCount() As Integer
        Return m_ScannedItemCount
    End Function
    ''' <summary>
    ''' Retrieves the total number of SELs queued for this session.
    ''' </summary>
    ''' <returns>Number of queued SELs</returns>
    ''' <remarks></remarks>
    Private Function GetSELQueuedCount() As Integer
        Return m_SELQueued
    End Function
    ''' <summary>
    ''' Retrieves the total number of items with non zero Fill Quantity.
    ''' </summary>
    ''' <returns>Number of items with non zero fill quantity.</returns>
    ''' <remarks></remarks>
    Private Function GetPickListCount() As Integer
#If NRF Then
        Return m_SMItemList.Count
#ElseIf RF Then
        Return m_iSeqNum
#End If

    End Function
    Public Function IsMultisited() As Boolean
        Return m_bIsMultisited
    End Function
    ''' <summary>
    ''' Display Items details when an item is selected from View List
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayViewItem()
        Dim iIndex As Integer = 0
        Dim objProductInfo As SMProductInfo
        With m_SMView
            For iIndex = 0 To .lstView.Items.Count - 1
                If .lstView.Items(iIndex).Selected Then
                    objProductInfo = m_SMItemList.Item(iIndex)
                    HandleScanData(objAppContainer.objHelper.GeneratePCwithCDV(objProductInfo.ProductCode), BCType.ManualEntry)
                    Exit For
                End If
            Next
        End With
        m_bItemScanned = False

    End Sub

    ''' <summary>
    ''' Process the Sales floor Quantity entry
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function ProcessSalesFlrQnty(Optional ByVal bIsFPCGapCall As Boolean = False) As Boolean
        'Check if in case the item is already scanned and has sales floor quantity
        If bIsFPCGapCall Then
            'Dim dialogResult As New DialogResult
            'If Val(m_SMProductInfo.strShelfQuantiy) > 0 Then
            '    dialogResult = MessageBox.Show("You have selected Gap. Do you really want to continue.", _
            '                                    "Warning", MessageBoxButtons.YesNo, _
            '                                    MessageBoxIcon.Asterisk, _
            '                                    MessageBoxDefaultButton.Button2)
            '    'Check the user input.
            '    If dialogResult = Windows.Forms.DialogResult.No Then
            '        'If user chooses No. return to FPC screen.
            '        Return False
            '    End If
            'if user confirms for GAP selection then set label text to "0"
            m_SMItemDetails.lblSlsflrVal.Text = "0"
            'End If
        End If
        'For call from item details screen.
        'm_SMProductInfo.strShelfQuantiy = m_SMItemDetails.lblSlsflrVal.Text
        m_SalesFloorQty = m_SMItemDetails.lblSlsflrVal.Text
        If ((m_SMItemDetails.lblSlsflrVal.Text = "0") Or (m_SMItemDetails.lblSlsflrVal.Text = Nothing)) Then
            If (m_EntryType = BCType.EAN) Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M51"), "Invalid Data", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, _
                                MessageBoxDefaultButton.Button2)
                Return True
            Else
                'in case if the value is nothing the same can be set to 0.
                ' m_SMProductInfo.strShelfQuantiy = "0"
                m_SalesFloorQty = "0"
                'MessageBox.Show(MessageManager.GetInstance().GetMessage("M50"), "GAP", _
                '                MessageBoxButtons.OK, _
                '                MessageBoxIcon.Exclamation, _
                '                MessageBoxDefaultButton.Button3)
            End If
        End If

        'UAT Bug Fix - If GAP or Quit is selected from FPC screen clear all price chk related variables
        m_SELForFullPriceCheck = ""
        m_bIsFullPriceCheckRequired = False
        'UAT Bug Fix End

        DisplaySMScreen(SMSCREENS.FillQuantity)

        'Clear the text after updating the sales floor quantity.
        m_SMItemDetails.lblSlsflrVal.Text = "0"

    End Function
    ''' <summary>
    ''' Process the Fill Quantity Entry
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessFillQnty() As Integer
        'Read the quantity from labels.
        m_SMProductInfo.strShelfQuantiy = m_SMFillQuantity.lblSlsflrVal.Text
        m_SMProductInfo.strFillQuantiy = m_SMFillQuantity.lblFilQntyVal.Text
        'Check if the fill quantity entered is 0 or left nothing.
        If ((m_SMProductInfo.strFillQuantiy = "0") Or (m_SMProductInfo.strFillQuantiy = Nothing)) Then
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M49"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
        End If
        'Check for multisited items and update the tracker.
        If m_bIsMultisited Then
            UpdateMultiSiteTracker()
            m_arrPogList.Clear()
            If ((m_SMFillQuantity.cmbbxMltSites.SelectedItem.Equals("Other")) Or _
                (m_SMFillQuantity.cmbbxMltSites.SelectedItem.Equals("OTHER")) Or _
                (m_SMFillQuantity.cmbbxMltSites.SelectedItem.Equals("-Other")) Or _
                (m_SMFillQuantity.cmbbxMltSites.SelectedItem.Equals("-OTHER"))) Then
                'Set the other item actioned flag to true.
                m_SMProductInfo.isOtherActioned = True
            End If
        End If
#If RF Then
        'Update the item details in the array.
        If ProcessGAP(m_SMProductInfo) Then
            UpdateItemList(m_SMProductInfo)
        End If
#ElseIf NRF Then
        UpdateItemList(m_SMProductInfo)
#End If
        'If item scanned then increment the count.
        If m_bItemScanned Then
            m_ScannedItemCount = m_ScannedItemCount + 1
            m_bItemScanned = False
        End If

        'Clearing all session variables for current item.
        m_arrPogList.Clear()
        m_SelectedPOGSeqNum = ""
        If (m_bIsMultisited) Then
            If (SMSessionMgr.GetInstance.IsAllMSCounted) Then
                m_bIsMultisited = False
            End If
            ProcessExistingItem(m_SMProductInfo)
        Else
            m_bIsMultisited = False
        End If
        'Set barcode entry type to manual to allow GAPing second location of a
        'multisite item whose product code is scanned.
        m_EntryType = BCType.ManualEntry
    End Function
    ''' <summary>
    ''' Processes the GAP request when there is a GAP
    ''' </summary>
    ''' <returns>True is succefull Else False</returns>
    ''' <remarks></remarks>
    Public Function ProcessGAP(ByVal objExportDataItem As SMProductInfo) As Boolean
        'Write GAP record
        Dim GAPRecord As GAPRecord = Nothing
        Try
            GAPRecord.cIsGAPFlag = "Y"
            GAPRecord.strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(objExportDataItem.ProductCode)
            GAPRecord.strBootscode = objExportDataItem.BootsCode
            GAPRecord.strNumberSEQ = objExportDataItem.Sequence
            GAPRecord.strStockFig = objExportDataItem.TSF
            GAPRecord.strCurrentQty = objExportDataItem.strShelfQuantiy
            GAPRecord.strFillQty = objExportDataItem.strFillQuantiy
            GAPRecord.strUpdateOssrItem = " "
            GAPRecord.strLocCounted = objExportDataItem.strMSLocation
            'End after the change in the gap record.
#If NRF Then
            If objExportDataItem.MSFlag = "Y" Then
                GAPRecord.strLocCounted = "00"
            Else
                GAPRecord.strLocCounted = "  "
            End If
            GAPRecord.strNumberSEQ = m_iSeqNum.ToString()
            objAppContainer.objExportDataManager.CreateGAP(GAPRecord)
            m_iSeqNum = m_iSeqNum + 1
#ElseIf RF Then
            If objAppContainer.objExportDataManager.CreateGAP(GAPRecord) Then
                If isNewItem Then
                    m_iSeqNum = m_iSeqNum + 1
                    isNewItem = False
                End If
            Else
                Return False
            End If
#End If
            Return True
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
          ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            Else
                Return 0
            End If
#End If
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception in ProcessGAP of SMSessionMgr " + ex.StackTrace(), Logger.LogLevel.RELEASE)
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Get all Multisite locations for an item
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <param name="arrPlannerList"></param>
    ''' <remarks></remarks>
    Private Sub GetMultiSiteLocations(ByVal strBootsCode As String, ByRef arrPlannerList As ArrayList)
        Try
            objAppContainer.objDataEngine.GetPlannerListUsingBC(strBootsCode, True, arrPlannerList)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in GetMultiSiteLocations of SMSessionMgr" + ex.StackTrace(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Get the Total fill Quantity for a scanned item
    ''' </summary>
    ''' <param name="strProductCode">Product code of the scanned item</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetTotalFillQnty(ByVal strProductCode As String) As String
        Try
            Dim strTotalFillQnty As Integer = 0
            For Each objSMMultiSiteInfo As SMMultiSiteInfo In m_SMMultiSiteTracker
                If (objSMMultiSiteInfo.strProductCode = strProductCode) Then
                    strTotalFillQnty = strTotalFillQnty + Microsoft.VisualBasic.Val(objSMMultiSiteInfo.strFillQuantiy)
                End If
            Next
            Return strTotalFillQnty.ToString()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception in SMSessionMgr::" + ex.StackTrace(), Logger.LogLevel.RELEASE)
            Return "0"
        End Try
    End Function
    ''' <summary>
    ''' Get the Total Shelf Quantity for a scanned item
    ''' </summary>
    ''' <param name="strProductCode">Product code of the scanned item</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetTotalShelfQnty(ByVal strProductCode As String) As String
        Try
            Dim strTotalShelfQnty As Integer = 0
            For Each objSMMultiSiteInfo As SMMultiSiteInfo In m_SMMultiSiteTracker
                If (objSMMultiSiteInfo.strProductCode = strProductCode) Then
                    strTotalShelfQnty = strTotalShelfQnty + Microsoft.VisualBasic.Val(objSMMultiSiteInfo.strShelfQuantiy)
                End If
            Next
            Return strTotalShelfQnty.ToString()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception in SMSessionMgr::" + ex.StackTrace(), Logger.LogLevel.RELEASE)
            Return "0"
        End Try
    End Function
    ''' <summary>
    ''' Get multisite locations for an already scanned item from the multisite tracker
    ''' </summary>
    ''' <param name="strProductCode">Product code of the already scanned item</param>
    ''' <param name="arrPlannerList">Ref to arrraylist to return the multisite locations</param>
    ''' <remarks></remarks>
    Private Sub GetExistingMultiSiteLocations(ByVal strProductCode As String, ByRef arrPlannerList As ArrayList)
        Try
            For Each objSMMultiSiteInfo As SMMultiSiteInfo In m_SMMultiSiteTracker
                If (objSMMultiSiteInfo.strProductCode = strProductCode) Then
                    If (objSMMultiSiteInfo.strFillQuantiy = "0" And objSMMultiSiteInfo.bIsCounted = False) Then
                        objSMMultiSiteInfo.strFillQuantiy = ""
                    End If
                    arrPlannerList.Add(objSMMultiSiteInfo)
                End If
            Next
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception in SMSessionMgr::" + ex.StackTrace(), Logger.LogLevel.RELEASE)
        End Try
    End Sub

    ''' <summary>
    ''' Initialise the View Scanned Items Screen 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub IntialiseViewScreen()
        With m_SMView
            .lblHeading.Text = "View Shelf Monitor List"
            .lstView.Columns.Add("Item", 70 * objAppContainer.iOffSet, HorizontalAlignment.Center)
            .lstView.Columns.Add("Description", 122 * objAppContainer.iOffSet, HorizontalAlignment.Center)
            .lstView.Columns.Add("MS", 30 * objAppContainer.iOffSet, HorizontalAlignment.Center)
        End With
    End Sub
    ''' <summary>
    ''' Populate the View List
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateSMViewList()
        objAppContainer.objLogger.WriteAppLog("Enter SMSessionMgr PoplateSMViewList", Logger.LogLevel.RELEASE)
        Try
            With m_SMView
                Dim strTempMS As String
                Dim objMSData As ArrayList = New ArrayList

                For Each objItemInfo As SMProductInfo In m_SMItemList

                    If Microsoft.VisualBasic.Val(objItemInfo.iMultisiteCnt) > 0 Then
                        If (IsAllSitesCounted(objItemInfo.ProductCode)) Then
                            strTempMS = objItemInfo.iMultisiteCnt.ToString()
                        Else
                            strTempMS = objItemInfo.iMultisiteCnt.ToString() + "*"
                        End If
                    Else
                        strTempMS = "1"
                    End If

                    .lstView.Items.Add( _
                        (New ListViewItem(New String() {objItemInfo.BootsCode, _
                                                        objItemInfo.ShortDescription, _
                                                        strTempMS})))
                Next
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception in SMSessionMgr::" + ex.StackTrace(), Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SMSessionMgr PopulateSMViewList", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Check if all Multi sites are counted for a product
    ''' </summary>
    ''' <param name="CurrentProductCode">Product code of the item to be checked</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IsAllSitesCounted(ByVal CurrentProductCode As String) As Boolean
        Dim CurrentItem As ArrayList = New ArrayList
        GetExistingMultiSiteLocations(CurrentProductCode, CurrentItem)
        For Each objPOG As SMMultiSiteInfo In CurrentItem
            If (objPOG.bIsCounted = False) Then
                Return False
            End If
        Next
        Return True
    End Function
    ''' <summary>
    ''' Updates the Multisites combo box with POG descriptions
    ''' Add the product and the POG descriptions to the MS tracker
    ''' </summary>
    ''' <param name="PogList">the List of POG where the product is present</param>
    ''' <param name="strProdCode">Product code of the product</param>
    ''' <remarks></remarks>
    Private Sub PopulateMultiSites(ByVal PogList As ArrayList, ByVal strProdCode As String)
        Try
            Dim iCnt As Integer = New Integer()
            m_SMItemDetails.cmbbxMltSites.Items.Clear()
            m_SMFillQuantity.cmbbxMltSites.Items.Clear()

            m_SMItemDetails.cmbbxMltSites.Items.Add("SELECT")
            m_SMFillQuantity.cmbbxMltSites.Items.Add("SELECT")

            For iCnt = 0 To PogList.Count - 1
                Dim objPlannerInfo As PlannerInfo = New PlannerInfo()
                Dim objSMMultiSiteInfo As SMMultiSiteInfo = New SMMultiSiteInfo
                objPlannerInfo = PogList.Item(iCnt)
                m_SMItemDetails.cmbbxMltSites.Items.Add(objPlannerInfo.POGDesc.ToString.Trim() _
                                                        & " - " & objPlannerInfo.Description.ToString.Trim())
                m_SMFillQuantity.cmbbxMltSites.Items.Add(objPlannerInfo.POGDesc.ToString.Trim() _
                                                        & " - " & objPlannerInfo.Description.ToString.Trim())

            Next iCnt

            m_SMItemDetails.cmbbxMltSites.Items.Add("OTHER")
            m_SMFillQuantity.cmbbxMltSites.Items.Add("OTHER")
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception in SMSessionMgr::" + ex.StackTrace(), Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SMSessionMgr PopulateMultiSites", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Add an Multisited item to the Multisite tracker
    ''' </summary>
    ''' <param name="strPOGdesc">POG description</param>
    ''' <param name="strProdCode">Product Code</param>
    ''' <remarks></remarks>
    Private Sub AddtoMultiSiteTracker(ByVal strPlannerdesc As String, ByVal strPOGdesc As String, ByVal strProdCode As String, ByVal strSeqNum As String, ByVal PSC As String)
        Try
            Dim objSMMultiSiteInfo As SMMultiSiteInfo = New SMMultiSiteInfo
            objSMMultiSiteInfo.strProductCode = strProdCode
            objSMMultiSiteInfo.strPOGDescription = strPOGdesc
            objSMMultiSiteInfo.strPlannerDesc = strPlannerdesc
            'TODO Change this to set as False by default
            objSMMultiSiteInfo.strShelfQuantiy = "0"
            objSMMultiSiteInfo.strFillQuantiy = "0"
            objSMMultiSiteInfo.PSC = PSC
            'For Other locations, set default counted as True
            If (strPOGdesc = "OTHER") Then
                objSMMultiSiteInfo.bIsCounted = True
            Else
                objSMMultiSiteInfo.bIsCounted = False
            End If
            objSMMultiSiteInfo.strSeqNumber = strSeqNum
            m_SMMultiSiteTracker.Add(objSMMultiSiteInfo)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception in SMSessionMgr::" + ex.StackTrace(), Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SMSessionMgr AddtoMultiSiteTracker", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsAllMSCounted() As Boolean
        Dim objUncntdMS As ArrayList = SMSessionMgr.GetInstance.GetUncountedMSites()
        If objUncntdMS.Count > 0 Then
            For Each obj As SMMultiSiteInfo In objUncntdMS
                If (IsItemAlreadyScanned(obj.strProductCode)) Then
                    If Not obj.strPOGDescription = "OTHER" Then
                        Return False
                    End If
                End If
            Next
        Else
            Return True
        End If
    End Function
    ''' <summary>
    ''' Update the Multisite tracker with the counts
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateMultiSiteTracker()
        Try
            If Not (IsItemAlreadyScanned(m_SMProductInfo.ProductCode)) Then
                'Item not already added. Add all locations for this item to tracker
                Dim icnt As Integer = 0
                Dim iTotalSites As Integer = m_arrPogList.Count - 1
                For icnt = 0 To iTotalSites
                    Dim objPlannerInfo As PlannerInfo = New PlannerInfo()
                    objPlannerInfo = m_arrPogList.Item(icnt)
                    'Add item to Tracker
                    AddtoMultiSiteTracker(objPlannerInfo.POGDesc.ToString(), _
                                          objPlannerInfo.Description.ToString(), _
                                          m_SMProductInfo.ProductCode, _
                                          (icnt + 1).ToString().PadLeft(3, "0"), objPlannerInfo.PhysicalShelfQty)
                Next icnt
                'Add Other Locations for tracking
                AddtoMultiSiteTracker("", "OTHER", m_SMProductInfo.ProductCode, (icnt + 1).ToString().PadLeft(3, "0"), "0")
            End If
            'Item Already added. Update new counts for this item.
            For Each objSMMultiSiteInfo As SMMultiSiteInfo In m_SMMultiSiteTracker
                If (objSMMultiSiteInfo.strSeqNumber = m_SelectedPOGSeqNum And objSMMultiSiteInfo.strProductCode = m_SMProductInfo.ProductCode) Then
                    objSMMultiSiteInfo.strShelfQuantiy = m_SMProductInfo.strShelfQuantiy
                    objSMMultiSiteInfo.strFillQuantiy = m_SMProductInfo.strFillQuantiy
                    objSMMultiSiteInfo.bIsCounted = True
                    m_SMProductInfo.strMSLocation = (Val(objSMMultiSiteInfo.strSeqNumber) - 1).ToString().PadLeft(2, "0")
                    Return
                End If
            Next
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString() + " Exception in SMSessionMgr::" + ex.StackTrace(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Get Multisite data for a lready scanned item
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GetMultiSiteData()
        Try
            'Get Existing shelf and Fill Quantity for selected Module
            Dim objPOGList As ArrayList = New ArrayList
            GetExistingMultiSiteLocations(m_SMProductInfo.ProductCode, objPOGList)
            For Each objPOG As SMMultiSiteInfo In objPOGList
                If (m_SelectedPOGSeqNum = objPOG.strSeqNumber) Then
                    m_SMProductInfo.strFillQuantiy = objPOG.strFillQuantiy
                    ''Auto Fill Quantity
                    'In Case of Item which is added to multisite tracker and not actioned 
                    'The Fill quantity is set to null to enable for auto fill quantity calculation
                    If (m_SMProductInfo.strFillQuantiy = "0" And Not (objPOG.bIsCounted)) Then
                        m_SMProductInfo.strFillQuantiy = ""
                    End If
                    m_SMProductInfo.strShelfQuantiy = objPOG.strShelfQuantiy
                    m_SMItemDetails.lblSlsflrVal.Text = m_SMProductInfo.strShelfQuantiy
                    m_SMFillQuantity.lblFilQntyVal.Text = m_SMProductInfo.strFillQuantiy
                End If
            Next
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception in SMSessionMgr::" + ex.StackTrace(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Get all uncounted sites for a multisited item
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUncountedMSites() As ArrayList
        Try
            Dim objSMMultiSiteInfo As SMMultiSiteInfo = New SMMultiSiteInfo
            Dim UncntdMSites As ArrayList = New ArrayList()
            For Each objSMMultiSiteInfo In m_SMMultiSiteTracker
                If objSMMultiSiteInfo.bIsCounted = False Then
                    UncntdMSites.Add(objSMMultiSiteInfo)
                End If
            Next
            Return UncntdMSites
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception in SMSessionMgr::" + ex.StackTrace(), Logger.LogLevel.RELEASE)
            Return Nothing
        End Try
    End Function
    ''' <summary>
    ''' Check if a valid location is selected for a multisited item
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckLocationSelection() As Boolean
        If m_bIsMultisited Then
            If (m_SelectedPOGSeqNum = "000" Or m_SelectedPOGSeqNum = Nothing) Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M52"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                Return False
            End If
        End If
        Return True
    End Function
    ''' <summary>
    ''' Display the default screen for Item Details
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetDefaultState()
        With m_SMItemDetails
            .lblSlsFlrQnty.Visible = True
            .lblSlsflrVal.Visible = True
            .lblSlsflrVal.Text = ""
            .btnCalcPadSlsFlr.Visible = True
            .lblMltSiteCnt.Visible = False
            .cmbbxMltSites.Visible = False
            .btnZero.Visible = True
        End With
    End Sub
    ''' <summary>
    ''' Display SM home screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplaySMScan(ByVal o As Object, ByVal e As EventArgs)
        With m_SMhome
            If m_SMItemList.Count = 0 Then
                m_SMhome.btnView.Visible = False
            Else
                m_SMhome.btnView.Visible = True
            End If
#If NRF Then
            .lblItemVal.Text = m_SMItemList.Count.ToString()
#ElseIf RF Then
            .lblItemVal.Text = GetPickListCount()
#End If

            .lblSELVal.Text = GetSELQueuedCount().ToString()

            'Sets the store id and active data time to the status bar
            Dim strStoreId As String = ""
            strStoreId = ConfigDataMgr.GetInstance().GetParam(ConfigKey.STORE_NO)
            .objStatusBar.lblStoreId.Text = strStoreId

            'Fix for the status bar doesnot display "Connected"
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)


            .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' Display SM Item Details Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplaySMItemDetails(ByVal o As Object, ByVal e As EventArgs)
        With m_SMItemDetails
            GetDefaultState()
            .lblBtsCode.Text = objAppContainer.objHelper.FormatBarcode(m_SMProductInfo.BootsCode)
            .lblEAN.Text = objAppContainer.objHelper.FormatBarcode(objAppContainer.objHelper.GeneratePCwithCDV(m_SMProductInfo.ProductCode))

            Dim objDescriptionArray As ArrayList = New ArrayList()
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(m_SMProductInfo.Description)

            .lblDesc1.Text = objDescriptionArray.Item(0)
            .lblDesc2.Text = objDescriptionArray.Item(1)
            .lblDesc3.Text = objDescriptionArray.Item(2)

            If m_SMItemList.Count = 0 Then
                m_SMItemDetails.btnView.Visible = False

            Else
                m_SMItemDetails.btnView.Visible = True
            End If

            .lblStsVal.Text = objAppContainer.objHelper.GetStatusDescription(m_SMProductInfo.Status)
            'Stock File Accuracy  added TSF  an total item count label
            If m_SMProductInfo.TSF.Substring(0, 1).Equals("-") Then
                .lblStockText.ForeColor = Color.Red
            Else
                .lblStockText.ForeColor = .lblStsVal.ForeColor
            End If
            .lblStockText.Text = m_SMProductInfo.TSF
            'Total item count for multisited items - SFA
            If m_bIsMultisited Then
                Dim objPOGList As ArrayList = New ArrayList
                Dim iItemCount As Integer = 0

                GetExistingMultiSiteLocations(m_SMProductInfo.ProductCode, objPOGList)

                For Each objPOG As SMMultiSiteInfo In objPOGList
                    iItemCount += objPOG.strShelfQuantiy
                Next

                .lblTotalItemText.Text = iItemCount.ToString()
            Else
                .lblTotalItemText.Text = m_SMProductInfo.strShelfQuantiy
            End If
            'For OSSR
#If RF Then
            'System testing - DEF:54 - Added specific text to RF mode
            .lblSODStockFile.Text = "Total Stock File:"

            If objAppContainer.OSSRStoreFlag = "Y" Then
                If m_SMItemDetails.btnView.Visible Then
                    'Fix for OSSR Button alignmnent not proper in SM Item Details screen - MC55RF testing
                    .btnNext.Location = New Point(6 * objAppContainer.iOffSet, 223 * objAppContainer.iOffSet)
                    .btnZero.Location = New Point(58 * objAppContainer.iOffSet, 223 * objAppContainer.iOffSet)
                    .btnView.Location = New Point(111 * objAppContainer.iOffSet, 223 * objAppContainer.iOffSet)
                    .btn_OSSRItem.Location = New Point(162 * objAppContainer.iOffSet, 223 * objAppContainer.iOffSet)
                    .btnQuit.Location = New Point(187 * objAppContainer.iOffSet, 250 * objAppContainer.iOffSet)

                Else
                    .btn_OSSRItem.Location = New Point(115 * objAppContainer.iOffSet, 237 * objAppContainer.iOffSet)
                    .btnQuit.Location = New Point(187 * objAppContainer.iOffSet, 237 * objAppContainer.iOffSet)
                    .btnZero.Location = New Point(60 * objAppContainer.iOffSet, 237 * objAppContainer.iOffSet)
                End If
                .btn_OSSRItem.Visible = True
                .lblOSSR.Visible = True
                If m_SMProductInfo.OSSRFlag = "O" Then
                    .lblOSSR.Text = "OSSR"
                Else
                    .lblOSSR.Text = " "
                End If

            Else
                'Fix for OSSR Button alignmnent not proper in SM Item Details screen - MC55RF testing
                .btn_OSSRItem.Visible = False
                .lblOSSR.Visible = False
                .btnNext.Location = New Point(6 * objAppContainer.iOffSet, 237 * objAppContainer.iOffSet)
                .btnZero.Location = New Point(65 * objAppContainer.iOffSet, 237 * objAppContainer.iOffSet)
                .btnQuit.Location = New Point(187 * objAppContainer.iOffSet, 237 * objAppContainer.iOffSet)
                .btnView.Location = New Point(121 * objAppContainer.iOffSet, 237 * objAppContainer.iOffSet)
            End If
#ElseIf NRF Then
            'Fix for OSSR Button alignmnent not proper in SM Item Details screen - MC55RF testing
             .btnNext.Location = New Point(6 * objAppContainer.iOffSet, 237 * objAppContainer.iOffSet)
             .btnZero.Location = New Point(65 * objAppContainer.iOffSet, 237 * objAppContainer.iOffSet)
             .btnQuit.Location = New Point(187 * objAppContainer.iOffSet, 237 * objAppContainer.iOffSet)
             .btnView.Location = New Point(121 * objAppContainer.iOffSet, 237 * objAppContainer.iOffSet)
            .btn_OSSRItem.Visible = False
            .btn_OSSRItem.Enabled = False
            .lblOSSR.Visible = False
            .lblOSSR.Enabled = False
#End If

            If (m_bIsMultisited) Then
                .lblMltSiteCnt.Visible = True
                .cmbbxMltSites.Visible = True
                .cmbbxMltSites.SelectedItem = "SELECT"
                .lblSlsflrVal.Text = "0"
            Else
                .lblMltSiteCnt.Visible = False
                .cmbbxMltSites.Visible = False
                .lblSlsflrVal.Text = m_SMProductInfo.strShelfQuantiy
            End If
            'UAT Bug Fix - If GAP or Quit is selected from FPC screen clear all price chk related variables
            m_SELForFullPriceCheck = ""
            m_bIsFullPriceCheckRequired = False
            'UAT Bug Fix End

            'Sets the store id and active data time to the status bar
            Dim strStoreId As String = ""
            strStoreId = ConfigDataMgr.GetInstance().GetParam(ConfigKey.STORE_NO)
            .objStatusBar.lblStoreId.Text = strStoreId

            'Fix for the status bar doesnot display "Connected"
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

            .Visible = True
            .Refresh()
        End With
        BCReader.GetInstance.StopRead()
    End Sub
    'For OSSR
#If RF Then
    Public Sub DisplayOSSRToggle(ByRef lblToggleOSSR As Label, ByVal strBarcode As String)
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
    ''' <summary>
    ''' Display SM Fill Quantity screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplaySMFillQuantity(ByVal o As Object, ByVal e As EventArgs)
        With m_SMFillQuantity
#If RF Then
            'UAT - DEF:860 - Added specific text to RF mode : UST Global
            .lblSODStockFile.Text = "Total Stock File:"
#End If
            .lblFilQntyVal.Text = ""
            .lblBtsCode.Text = objAppContainer.objHelper.FormatBarcode(m_SMProductInfo.BootsCode)
            .lblEAN.Text = objAppContainer.objHelper.FormatBarcode(objAppContainer.objHelper.GeneratePCwithCDV(m_SMProductInfo.ProductCode))

            Dim objDescriptionArray As ArrayList = New ArrayList()
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(m_SMProductInfo.Description)

            .lblDesc1.Text = objDescriptionArray.Item(0)
            .lblDesc2.Text = objDescriptionArray.Item(1)
            .lblDesc3.Text = objDescriptionArray.Item(2)

            If m_SMItemList.Count = 0 Then
                m_SMFillQuantity.btnView.Visible = False
            Else
                m_SMFillQuantity.btnView.Visible = True
            End If

            .lblStsVal.Text = objAppContainer.objHelper.GetStatusDescription(m_SMProductInfo.Status)
            'Stock File Accuracy  added TSF  an total item count label
            If m_SMProductInfo.TSF.Substring(0, 1).Equals("-") Then
                .lblStockText.ForeColor = Color.Red
            Else
                .lblStockText.ForeColor = .lblStsVal.ForeColor
            End If
            .lblStockText.Text = m_SMProductInfo.TSF
            'Total item count for multisited items - SFA
            .lblTotalItemText.Text = TotalItemCount + m_SalesFloorQty

            .lblFilQnty.Visible = True
            .lblFilQntyVal.Visible = True

            .lblSlsflrVal.Text = m_SalesFloorQty

            .lblFilQntyVal.Text = m_SMProductInfo.strFillQuantiy
            'DEFECT FIX - BTCPR00004156(After "0" is assumed as fill 
            'quantity it is not displayed when the same item is scanned again)
            If m_SMProductInfo.strFillQuantiy = "" Then
                .lblFilQntyVal.Text = "0"
            End If
            Dim temp As Integer = 0
            If (m_bIsMultisited) Then
                .lblMltSiteCnt.Visible = True
                .cmbbxMltSites.Visible = True
                temp = (Val(m_SelectedPOGSeqNum))
                .cmbbxMltSites.SelectedIndex = temp
            Else
                temp = 1
                .lblMltSiteCnt.Visible = False
                .cmbbxMltSites.Visible = False
            End If
            Dim tempFillQty As Integer
            ''Auto Fill Quantityt
            'The Following If clause displays the Fil Quantity
            IntialiseViewScreen()
            Try
                If Not (m_bIsMultisited) Then
                    If (m_SMProductInfo.strFillQuantiy = "") Then
                        If (Not (m_ItemActioned)) Then
                            .lblFilQnty.Text = "Fill Quantity (Auto):"
                            Dim objPlannerInfo As New PlannerInfo()
                            objPlannerInfo = m_arrPogList(temp - 1)
                            tempFillQty = Val(objPlannerInfo.PhysicalShelfQty.Trim) - Val(.lblSlsflrVal.Text.Trim())
                            If (tempFillQty <= 0) Then
                                .lblFilQntyVal.Text = "0"
                            Else
                                .lblFilQntyVal.Text = tempFillQty.ToString
                            End If
                        End If
                    Else
                        .lblFilQnty.Text = "Fill Quantity :"
                    End If
                ElseIf ((m_SMFillQuantity.cmbbxMltSites.SelectedItem.Equals("Other")) Or _
                     (m_SMFillQuantity.cmbbxMltSites.SelectedItem.Equals("OTHER"))) Then
                    .lblFilQnty.Text = "Fill Quantity (Auto):"
                    .lblFilQntyVal.Text = "0"
                Else
                    If (m_SMProductInfo.strFillQuantiy = "") Then
                        .lblFilQnty.Text = "Fill Quantity (Auto):"
                        If (Not (m_ItemActioned)) And (temp <= m_arrPogList.Count) Then
                            Dim objPlannerInfo As New PlannerInfo()
                            objPlannerInfo = m_arrPogList(temp - 1)
                            tempFillQty = Val(objPlannerInfo.PhysicalShelfQty.Trim) - Val(.lblSlsflrVal.Text.Trim())
                        ElseIf m_bIsMultisited And m_ItemActioned Then
                            Dim objPlannerInfo As New SMMultiSiteInfo()
                            objPlannerInfo = m_arrPogList(temp - 1)
                            tempFillQty = Val(objPlannerInfo.PSC.Trim) - Val(.lblSlsflrVal.Text.Trim())
                        End If
                        If (tempFillQty <= 0) Then
                            .lblFilQntyVal.Text = "0"
                        Else
                            .lblFilQntyVal.Text = tempFillQty.ToString
                        End If
                    Else
                        If ((m_SMFillQuantity.cmbbxMltSites.SelectedItem.Equals("Other")) Or _
                            (m_SMFillQuantity.cmbbxMltSites.SelectedItem.Equals("OTHER")) Or _
                            (m_SMFillQuantity.cmbbxMltSites.SelectedItem.Equals("-Other")) Or _
                            (m_SMFillQuantity.cmbbxMltSites.SelectedItem.Equals("-OTHER"))) Then

                            If Not m_SMProductInfo.isOtherActioned Then
                                .lblFilQnty.Text = "Fill Quantity (Auto):"
                            Else
                                .lblFilQnty.Text = "Fill Quantity :"
                            End If
                        Else
                            .lblFilQnty.Text = "Fill Quantity :"
                        End If
                    End If
                End If
            Catch ex As Exception
                .lblFilQntyVal.Text = "0"
                objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
            End Try
            'Sets the store id and active data time to the status bar
            Dim strStoreId As String = ""
            strStoreId = ConfigDataMgr.GetInstance().GetParam(ConfigKey.STORE_NO)
            .objStatusBar.lblStoreId.Text = strStoreId
            'Fix for the status bar doesnot display "Connected"
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            .Visible = True
            .Refresh()
        End With
        BCReader.GetInstance.StopRead()
    End Sub
    ''' <summary>
    ''' Display Summary screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplaySMSummary(ByVal o As Object, ByVal e As EventArgs)
        With m_SMSummary
            .lblScndItmsVal.Text = GetScannedProductCount()
            .lblPLItmsVal.Text = GetPickListCount()
            .lblSELQdVal.Text = GetSELQueuedCount()

            If .lblSELQdVal.Text = "0" Then
                .lblUserMsg.Visible = False
#If RF Then
                .lblActionDockTransmit.Visible = False
                .lblUserMsg.Visible = True
#End If
            Else
                .lblUserMsg.Visible = True
#If NRF Then
                .lblActionDockTransmit.Visible = True
#ElseIf RF Then
                .lblActionDockTransmit.Visible = False
                .lblUserMsg.Text = "Confirm all new SELs have been collected and displayed"
#End If
            End If
#If NRF Then
            'The message Dock and Tramsmit need not be displayed if there
            'are no Picking List items
            If .lblPLItmsVal.Text = "0" Then
                .lblActionDockTransmit.Visible = False
            Else
                .lblActionDockTransmit.Visible = True
            End If
#End If
            'Sets the store id and active data time to the status bar
            Dim strStoreId As String = ""
            strStoreId = ConfigDataMgr.GetInstance().GetParam(ConfigKey.STORE_NO)
            .objStatusBar.lblStoreId.Text = strStoreId

            'Fix for the status bar doesnot display "Connected"
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)




            .Visible = True
            .Refresh()
        End With
        BCReader.GetInstance.StopRead()
    End Sub
    ''' <summary>
    ''' Display Item View List screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplaySMItemView(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Enter SMSessionMgr DisplaySMItemView", Logger.LogLevel.RELEASE)
        With m_SMView
            .lstView.Clear()
            m_bIsFullPriceCheckRequired = False
            IntialiseViewScreen()
            PopulateSMViewList()

            'Sets the store id and active data time to the status bar
#If NRF Then
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#ElseIf RF Then
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
#End If

            .Visible = True
            .Refresh()
        End With
        BCReader.GetInstance.StopRead()
        objAppContainer.objLogger.WriteAppLog("Exit SMSessionMgr DisplaySMItemView", Logger.LogLevel.RELEASE)
    End Sub
    Private Sub DisplayFullPriceCheck(ByVal o As Object, ByVal e As EventArgs)
        BCReader.GetInstance().DisableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
        With m_SMFullPriceCheck
            .lblBtsCode.Text = objAppContainer.objHelper.FormatBarcode(m_SMProductInfo.BootsCode)
            'Fix for displaying Barcode in Full Price Check screen
            .lblEAN.Text = objAppContainer.objHelper.FormatBarcode(objAppContainer.objHelper.GeneratePCwithCDV(m_SMProductInfo.ProductCode))
            Dim objDescriptionArray As ArrayList = New ArrayList()
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(m_SMProductInfo.Description)

            .lblDesc1.Text = objDescriptionArray.Item(0)
            .lblDesc2.Text = objDescriptionArray.Item(1)
            .lblDesc3.Text = objDescriptionArray.Item(2)

            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)


            .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' Screen Display method for Shelf Monitor. 
    ''' All shelf monitor sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName">Enum SMSCREENS</param>
    ''' <returns>True if display is sucess else False</returns>
    ''' <remarks></remarks>
    Public Function DisplaySMScreen(ByVal ScreenName As SMSCREENS)
        BCReader.GetInstance().EnableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
        Try
            Select Case ScreenName
                Case SMSCREENS.Home
                    PreviousScreen = SMSCREENS.Home
                    m_SMhome.Invoke(New EventHandler(AddressOf DisplaySMScan))
                Case SMSCREENS.ItemDetails
                    PreviousScreen = SMSCREENS.ItemDetails
                    m_SMItemDetails.Invoke(New EventHandler(AddressOf DisplaySMItemDetails))
                Case SMSCREENS.FillQuantity
                    m_SMFillQuantity.Invoke(New EventHandler(AddressOf DisplaySMFillQuantity))
                Case SMSCREENS.ItemView
                    If m_SMItemList.Count = 0 Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M53"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                    Else
                        m_SMSummary.Invoke(New EventHandler(AddressOf DisplaySMItemView))
                    End If
                Case SMSCREENS.SMSummary
                    m_SMSummary.Invoke(New EventHandler(AddressOf DisplaySMSummary))
                Case SMSCREENS.FullPriceCheck
                    m_SMSummary.Invoke(New EventHandler(AddressOf DisplayFullPriceCheck))
                    'Write Inoke method for other SM screens here.
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception occured in DisplaySMScreen of SMSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' Enum Class that defines all screens for Shelf Monitor module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum SMSCREENS
        Home
        FullPriceCheck
        ItemDetails
        FillQuantity
        ItemView
        SMSummary
    End Enum
    ''' <summary>
    ''' enumerates all scanable fields in SM Module
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum SCNFIELDS
        ProductCode
        SELCode
    End Enum

    Public Class SMMultiSiteInfo
        Public strSeqNumber As String
        Public strProductCode As String
        Public strPOGDescription As String
        Public strPlannerDesc As String
        Public strShelfQuantiy As String
        Public strFillQuantiy As String
        Public PSC As String
        Public bIsCounted As Boolean
    End Class
End Class
''' <summary>
''' The value class for getting and managing Shelf Monitor Items.
''' </summary>
''' <remarks></remarks>
Public Class SMProductInfo
    Inherits ProductInfo

    Private m_FirstBarcode As String = ""
    'To set Second barcode 
    Private m_SecondBarcode As String = ""
    Public iMultisiteCnt As Integer = "0"
    Public strShelfQuantiy As String = "0"
    Public strFillQuantiy As String = "0"
    Public iSELQueued As Integer = 0
    Public isOtherActioned As Boolean = False
    Public strMSLocation As String = "  "
#If RF Then
    'Public m_OSSRFlag As String = ""
    Public m_PriceAcceptedFlag As String
    Public m_Target As Integer
    Public m_completed As Integer
#End If

    Public Property FirstBarcode() As String
        Get
            Return m_FirstBarcode
        End Get
        Set(ByVal value As String)
            m_FirstBarcode = value
        End Set
    End Property

    Public Property SecondBarcode() As String
        Get
            Return m_SecondBarcode
        End Get
        Set(ByVal value As String)
            m_SecondBarcode = value
        End Set
    End Property
End Class




