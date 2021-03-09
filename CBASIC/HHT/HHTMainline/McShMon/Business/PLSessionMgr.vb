Imports System.Linq
'''***************************************************************
''' <FileName>PLSessionMgr.vb</FileName>
''' <summary>
''' The Picking List Container Class.
''' Implements all business logic and GUI navigation for Picking List.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>27-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
'''*********************************************************************
''' * Modification Log 
''' ******************************************************************** 
''' 1.1. Mathew Jerry Thomas. Defect Fast Fill Issue Fix. Release 14 A Interim
''' Uncommented  "m_OSSRGapFlag = objOSSRCurrentProductInfo.GapFlag" in DisplayPLItemConfirm

Public Class PLSessionMgr
    Private Shared m_PLSessionMgr As PLSessionMgr = Nothing
    Private m_PLHome As frmPLHome
    Private m_PLItemConfirm As frmPLItemConfirm
    Private m_SMPLItemDetails As frmSMPLItemDetails
    Private m_FFPLItemDetails As frmFFPLItemDetails
    Private m_EXPLItemDetails As frmEXPLItemDetails
    Private m_PLOSSRFinish As frmPLOSSRFinish
    'Auto Fast Fill PL CR
    Private m_Message As frmMessage
    Private m_PLView As frmView
    'nan Discrepancy Screen
    Private m_PLDiscrepancy As frmPLDiscrepancy
    'nan PSP discrepancy
    Private m_PLPSPPending As frmPLPSPpending
    'ambli
    'For OSSR
    Private m_OSSRPLItemDetails As frmOSSRPLItemDetails
    Private m_AFFPL As frmAFFPLItemDetails
    Private m_PLFinish As frmPLFinish
    Private m_PLSummary As frmPLSummary
    Private m_FillType As FastFillType = FastFillType.NULL
    Private m_ScannedCount As Integer = "0"
    Private m_PickedCount As Integer = "0"
    Private m_OSSRCount As Integer = "0"
    Private m_iCurScreen As Integer = 0
    'Fix for OOSR Toggling bug
    'Private bOSSRToggled As Boolean = "False"
    'Private m_OSSRPL As Boolean = False
    'Private m_PickingList As ArrayList = Nothing
    'Private m_PLMappingTable As Hashtable = Nothing
    Private m_CurrentPickingList As PickingList = Nothing
    'Private m_PickedDataList As ArrayList = Nothing

    'Declared arraylist to store Info about Multisited Items
    Private m_MultiSiteList As ArrayList = Nothing
    'CR for Repeat Count 
    Private m_tempMultiSiteList As ArrayList = Nothing
    Private m_iProductListCount As Integer
    'Private objAppContainer.objEXMultiSiteList As ArrayList = Nothing
    Private m_SelectedEXMSIndex As String = Nothing

    'Variables to implement partial price check
    'Support: Full Price Check Removed
    'Private m_bIsFullPriceCheckRequired As Boolean
    'Private m_PreviousItem As String
    Private m_strSEL As String
    Private m_ModulePriceCheck As ModulePriceCheck

    'Variable to keep track of whether the finish screen was invoked from which screen's Quit button select
    Private m_QuitInvokedScreen As String = Nothing
    Private SelectedItemPOG As String = Nothing
    Public m_IsMultisite As Boolean = False
    Private m_iTotalMultisiteCount As Integer = Nothing

    'Property to store the number of items checked
    Private m_iNumItemsChecked As Integer
    'Boolean variable to check if the item scanned is product code.
    Private bIsProductCode As Boolean = False
    'Variable to hold the list ID if teh picking list location is changed to OSSR.
    Private m_strConvertedList As String = ""
    'Private m_strListType As String = ""
    'String to hold the GAP Flag value associated with PLC transaction.
    'Defaulting this flag to 'Y' i.e, to count the stock and correct the TSF
    'For Fast Fill, Auto Fast Fill and related OSSR picking list this flag should be set to 'N'
    Private m_OSSRGapFlag As String = Macros.PLC_SM_FLAG
    'Fix for FF and EX PL
    Public bIsGap As Boolean = False
    Private OOSRStatus As New Hashtable()

    'Fix for defect 5059
    Private bHideCalcPad As Boolean = False

    'nan variable to track the site being counted now - set to default value of 0
    Private m_SelectedSite As Integer = 0
    'nan Store the form header
    Private m_FormHeader As String
    'stored products to be shown in discrepancy screen
    Private m_DiscrepancyList As ArrayList = Nothing
    Private bFromViewScreen As Boolean = False
    Private m_bIsProdCodeScanned As Boolean = False
    'to chk whether no selected from Finish screen or PSPPending screen
    Private m_iCancelQuit As Integer = 0
    'to chk whether zero selected from item confirm screen or SMPLitemdetailsscreen 
    'or EXPLItemDetails
    Private m_iZeroPressed As Integer = 0

    Public Property ZeroPressed() As Integer
        Get
            Return m_iZeroPressed
        End Get
        Set(ByVal value As Integer)
            m_iZeroPressed = value
        End Set
    End Property

    ''' <summary>
    ''' Getter and setter for the property m_bIsProdCodeScanned
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsScanned() As Boolean
        Get
            Return m_bIsProdCodeScanned
        End Get
        Set(ByVal value As Boolean)
            m_bIsProdCodeScanned = value
        End Set
    End Property

    Public Property SelectedSite() As Integer
        Get
            Return m_SelectedSite
        End Get
        Set(ByVal value As Integer)
            m_SelectedSite = value
        End Set
    End Property

    ''' <summary>
    ''' Getter and setter for the property m_iSalesFloorItemCount
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NumItemsChecked() As Integer
        Get
            Return m_iNumItemsChecked
        End Get
        Set(ByVal value As Integer)
            m_iNumItemsChecked = value
        End Set
    End Property
    'Property to store the number of items for which GAP is reported
    Private m_iNumGapItems As Integer
    Private Sub New()

    End Sub
    Public Property m_SelectedPOG() As String
        Get
            Return SelectedItemPOG
        End Get
        Set(ByVal value As String)
            SelectedItemPOG = value
        End Set
    End Property
    ''' <summary>
    ''' Functions for getting the object instance for the PLSessionMgr. 
    ''' Use this method to get the object refernce for the Singleton PLSessionMgr.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of CLContainer Class</remarks>
    Public Shared Function GetInstance() As PLSessionMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PICKGLST
        If m_PLSessionMgr Is Nothing Then
            m_PLSessionMgr = New PLSessionMgr()
            Return m_PLSessionMgr
        Else
            Return m_PLSessionMgr
        End If

    End Function
    Public Function CheckLocationSelection() As Boolean
        If m_IsMultisite Then
            If (m_SelectedPOG = "SELECT" Or m_SelectedPOG = Nothing) Then
                MessageBox.Show("Cannot Enter Sales Floor Quantity without Selecting a Location")
                Return False
            End If
        End If
        Return True
    End Function
    ''' <summary>
    ''' Does all the processing at the start of a session
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function StartSession() As Boolean
        Try
            objAppContainer.objLogger.WriteAppLog("Entered StartSession of PLSessionMgr", Logger.LogLevel.INFO)
            'Do all count list realated Initialisations here.
#If RF Then
            If objAppContainer.objExportDataManager.CreatePLO() Then
#End If
            'autofast fill PL CR
            m_Message = New frmMessage()
            m_PLHome = New frmPLHome()
            m_PLView = New frmView()
            m_PLItemConfirm = New frmPLItemConfirm()
            m_SMPLItemDetails = New frmSMPLItemDetails()
            m_FFPLItemDetails = New frmFFPLItemDetails()
            'ambli
            'For oSSR
            m_OSSRPLItemDetails = New frmOSSRPLItemDetails()
            m_AFFPL = New frmAFFPLItemDetails
            m_EXPLItemDetails = New frmEXPLItemDetails()
            m_PLFinish = New frmPLFinish()
            m_PLSummary = New frmPLSummary()
            m_PLOSSRFinish = New frmPLOSSRFinish()
            'nan
            m_PLDiscrepancy = New frmPLDiscrepancy()
            m_PLPSPPending = New frmPLPSPpending()

            'm_PickingList = New ArrayList()
            'm_PLMappingTable = New Hashtable()
            m_CurrentPickingList = New PickingList()
            'm_PickedDataList = New ArrayList()
            m_MultiSiteList = New ArrayList()
            m_tempMultiSiteList = New ArrayList()
            'objAppContainer.objEXMultiSiteList = New ArrayList()
            'Support: Full Price Check Removed
            'm_bIsFullPriceCheckRequired = False
            'm_PreviousItem = ""
            m_strSEL = ""
            m_ModulePriceCheck = New ModulePriceCheck()

            m_iProductListCount = 0


            m_iNumItemsChecked = 0
            m_iNumGapItems = 0
#If RF Then
            Else
                Return False
            End If
#End If
#If NRF Then
            'Retrieves the picking lists from DB
            If GetPickingListInfo() Then
                Return True
            Else
                Return False
            End If
#ElseIf RF Then
            PLSessionMgr.GetInstance().DisplayPLScreen(PLSessionMgr.PLSCREENS.Home)
            'return true
            Return True
#End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit StartSession of PLSessionMgr", Logger.LogLevel.INFO)
    End Function
#If RF Then
    ''' <summary>
    ''' Updates the Status bar of all the forms in the session manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBarMessage()
        Try
            m_PLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_PLItemConfirm.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SMPLItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_FFPLItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_OSSRPLItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_AFFPL.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_EXPLItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_PLFinish.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_PLSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_PLOSSRFinish.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured, Trace: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End If
    ''' <summary>
    ''' Does all processing that needs to be done when a session ends
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
#If NRF Then
  Public Function EndSession() As Boolean
#ElseIf RF Then
    Public Function EndSession(Optional ByVal isConnectivityLoss As Boolean = False) As Boolean
#End If
        Try
            objAppContainer.objLogger.WriteAppLog("Entered EndSession of PLSessionMgr", Logger.LogLevel.INFO)
            'Save and data and perform all Exit Operations.
            'Close and Dispose all forms.
#If NRF Then
            'Deleting The file
            objAppContainer.objExportDataManager.DeleteTempFiles(SMTransactDataManager.ExFileType.PLTemp)
            WriteTempData()
#ElseIf RF Then
            If Not isConnectivityLoss Then
                If Not objAppContainer.objExportDataManager.CreatePLF() Then
                    objAppContainer.objLogger.WriteAppLog("Cannot End Picking List Session." _
                                                          , Logger.LogLevel.RELEASE)
                    Return False
                End If
            End If
#End If
            m_PLHome.Dispose()
            m_PLItemConfirm.Dispose()
            'auto fast fill CR
            m_Message.Dispose()
            m_PLView.Dispose()
            'ambli
            'For OSSR
            m_OSSRPLItemDetails.Dispose()
            m_SMPLItemDetails.Dispose()
            m_AFFPL.Dispose()
            m_FFPLItemDetails.Dispose()
            m_EXPLItemDetails.Dispose()
            m_PLFinish.Dispose()
            m_PLSummary.Dispose()
            m_PLOSSRFinish.Dispose()
            'nan Discrepancy form dispose
            m_PLDiscrepancy.Dispose()
            m_PLPSPPending.Dispose()

            m_PLHome = Nothing
            m_PLItemConfirm = Nothing
            'ambli
            'For OSSR
            m_OSSRPLItemDetails = Nothing
            m_SMPLItemDetails = Nothing
            m_AFFPL = Nothing
            m_FFPLItemDetails = Nothing
            m_EXPLItemDetails = Nothing
            m_PLFinish = Nothing
            m_PLSummary = Nothing
            m_PLOSSRFinish = Nothing
            m_PLDiscrepancy = Nothing
            m_PLPSPPending = Nothing
            m_ModulePriceCheck = Nothing
            'auto fast fill PL CR
            m_PLView = Nothing
            'Release all objects and Set to nothing.
            'm_PickingList = Nothing
            'm_PLMappingTable = Nothing
            m_CurrentPickingList = Nothing
            m_MultiSiteList = Nothing
            m_tempMultiSiteList = Nothing
            'm_PickedDataList = Nothing
            'objAppContainer.objEXMultiSiteList = Nothing
            SelectedItemPOG = Nothing
            'Support: Full Price Check Removed
            'm_PreviousItem = Nothing
            m_PLSessionMgr = Nothing

            bIsGap = False

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in EndSession of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit EndSession of PLSessionMgr", Logger.LogLevel.INFO)
        Return True

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
        bIsProductCode = False
        objAppContainer.objLogger.WriteAppLog("Entered HandleScanData of PLSessionMgr", Logger.LogLevel.INFO)
        objAppContainer.objLogger.WriteAppLog("Barcode is :" + strBarcode + "|Type is: " + Type.ToString(), _
                                              Logger.LogLevel.INFO)
        m_PLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        'Set hide cal pad to false to display the calc pad in case of item details screen.
        bHideCalcPad = False
        Try
            Select Case Type
                Case BCType.EAN
                    m_bIsProdCodeScanned = True
                    If Not (objAppContainer.objHelper.ValidateEAN(strBarcode)) Or _
                       Val(strBarcode) = 0 Then
                        'Handle Invalid EAN here
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
                    Else
                        bIsProductCode = True
                        ProcessBarcodeEntered(strBarcode, True)
                    End If
                    m_PLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                Case BCType.ManualEntry
                    m_bIsProdCodeScanned = False
                    Dim strBootsCode As String = ""
                    If strBarcode.Length < 8 Then
                        'strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                        'Validate the Boots code entered for confirmation.
                        If objAppContainer.objHelper.ValidateBootsCode(strBarcode) Then
                            ProcessBarcodeEntered(strBarcode, False)
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                           MessageBoxButtons.OK, _
                           MessageBoxIcon.Asterisk, _
                           MessageBoxDefaultButton.Button1)
                            m_PLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
                        End If
                    Else
                        'handle manually entered valid product code
                        strBarcode = strBarcode.PadLeft(13, "0")    'To make EAN-8 to EAN-13
                        If (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                            bIsProductCode = True
                            ProcessBarcodeEntered(strBarcode, True)
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                           MessageBoxButtons.OK, _
                           MessageBoxIcon.Asterisk, _
                           MessageBoxDefaultButton.Button1)
                            m_PLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
                        End If
                    End If
                Case BCType.SEL
                    m_bIsProdCodeScanned = False
                    'Disable SEL for SM and FF PL
                    'If Not (m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL)) And _
                    'Not (m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_SF)) And _
                    'Not (m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_OSSR)) And _
                    'Not (m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_OSSR)) Then
                    '    MessageBox.Show(MessageManager.GetInstance().GetMessage("M69"), "Alert", _
                    '            MessageBoxButtons.OK, _
                    '            MessageBoxIcon.Hand, _
                    '            MessageBoxDefaultButton.Button1)
                    '    m_PLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                    '    Return
                    'End If
                    If objAppContainer.objHelper.ValidateSEL(strBarcode) Then
                        Dim strBootsCode As String = ""
                        objAppContainer.objHelper.GetBootsCodeFromSEL(strBarcode, strBootsCode)
                        strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)

                        m_strSEL = strBarcode
                        ProcessBarcodeEntered(strBootsCode, False)
                    Else
                        'handle invalid SEL here
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M4"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        m_PLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                    End If
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in HandleScanData of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit HandleScanData of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processes the product code or boots code entered
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="bIsProductCode"></param>
    ''' <remarks></remarks>
    Public Sub ProcessBarcodeEntered(ByVal strBarcode As String, ByVal bIsProductCode As Boolean)
        objAppContainer.objLogger.WriteAppLog("Entered ProcessBarcodeEntered of PLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim bIsProductValid As Boolean = False
            Dim objProductInfoList As ArrayList = New ArrayList()
            Dim strPrdCode As String = ""
            m_FillType = FastFillType.NULL
            'Removes the CDV digit if it is product code
            If bIsProductCode Then
                'strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                'To take the first 12 digit of product code.
                strBarcode = strBarcode.Substring(0, 12)
            End If

            'Retrieves the list id for which product info needs to be checked

            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If

            'Checks if the product code scanned and the product code of the product in the list are same
            'If true then item is valid else item is not valid

            Dim objSMPLProductInfo As PLProductInfo
            'nan Dim objSMPLProductInfo As SMPLProductInfo
            Dim objFFPLProductInfo As PLProductInfo 'nan FFPLProductInfo
            Dim objEXPLProductInfo As PLProductInfo 'nan- EXPLProductInfo 
            Dim objAFFPLProductInfo As PLProductInfo 'nan AFFPLProductInfo
#If RF Then
            Dim objOSSRPLProductInfo As PLProductInfo 'OSSRPLProductInfo changed to  PLProductInfo
#End If
            'Initialises the required value class based on the type of picking list selected
            'Validates the barcode entered
            If m_CurrentPickingList.ListType.Equals(Macros.SHELF_MONITOR_PL) Or _
               m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL) Then
                objSMPLProductInfo = New PLProductInfo()
                'nan objSMPLProductInfo = New SMPLProductInfo()
                objSMPLProductInfo = objProductInfoList.Item(m_iProductListCount)
                If bIsProductCode Then
                    'Include condition for checking hte product code against the first barcode.
                    'Fix for 2377
                    'If objSMPLProductInfo.ProductCode.Equals(strBarcode) Or _
                    '   objSMPLProductInfo.FirstBarcode.Equals(strBarcode) Then
                    If objAppContainer.objDataEngine.ValidateUsingPCAndBC(objSMPLProductInfo.BootsCode, _
                                                                       strBarcode) Then
                        bIsProductValid = True
#If NRF Then
                    Else
                        'DARWIN checking if the base barcode is present in Database/Conroller
                        If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                            'DARWIN converting database to Base Barcode
                            strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                            If objAppContainer.objDataEngine.ValidateUsingPCAndBC(objSMPLProductInfo.BootsCode, _
                                                                        strBarcode) Then
                                bIsProductValid = True
                            End If
                        End If
#End If
                    End If
                Else
                    If objSMPLProductInfo.BootsCode.Equals(strBarcode) Then
                        bIsProductValid = True
                    End If
                End If
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.FAST_FILL_PL) Then
                objFFPLProductInfo = New PLProductInfo() 'nan FFPLProductInfo()
                objFFPLProductInfo = objProductInfoList.Item(m_iProductListCount)
                If bIsProductCode Then
                    'Include condition for checking hte product code against the first barcode.
                    'Fix for 2377
                    'If objFFPLProductInfo.ProductCode.Equals(strBarcode) Or _
                    '    objFFPLProductInfo.FirstBarcode.Equals(strBarcode) Then
                    If objAppContainer.objDataEngine.ValidateUsingPCAndBC(objFFPLProductInfo.BootsCode, _
                                                                          strBarcode) Then
                        strPrdCode = objFFPLProductInfo.ProductCode
                        bIsProductValid = True
#If NRF Then
                    Else
                        'DARWIN checking if the base barcode is present in Database/Conroller
                        If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                            'DARWIN converting database to Base Barcode
                            strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                            If objAppContainer.objDataEngine.ValidateUsingPCAndBC(objFFPLProductInfo.BootsCode, _
                                                                          strBarcode) Then
                                strPrdCode = objFFPLProductInfo.ProductCode
                                bIsProductValid = True
                            End If
                        End If
#End If
                    End If
                Else
                    If objFFPLProductInfo.BootsCode.Equals(strBarcode) Then
                        strPrdCode = objFFPLProductInfo.ProductCode
                        bIsProductValid = True
                    End If
                End If
#If RF Then
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.OSSR_PL) Or _
                   m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_OSSR) Then
                objOSSRPLProductInfo = New PLProductInfo() 'OSSRPLProductInfo changed to  PLProductInfo
                objOSSRPLProductInfo = objProductInfoList.Item(m_iProductListCount)
                If bIsProductCode Then
                    'Include condition for checking hte product code against the first barcode.
                    'Fix for 2377
                    'If objFFPLProductInfo.ProductCode.Equals(strBarcode) Or _
                    '    objFFPLProductInfo.FirstBarcode.Equals(strBarcode) Then
                    If objAppContainer.objDataEngine.ValidateUsingPCAndBC(objOSSRPLProductInfo.BootsCode, _
                                                                          strBarcode) Then
                        strPrdCode = objOSSRPLProductInfo.ProductCode
                        bIsProductValid = True
                    End If
                Else
                    If objOSSRPLProductInfo.BootsCode.Equals(strBarcode) Then
                        strPrdCode = objOSSRPLProductInfo.ProductCode
                        bIsProductValid = True
                    End If
                End If
#End If
                'Check for Auto Fast Fill Added Here
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.AUTO_FAST_FILL_PL) Then
                objAFFPLProductInfo = New PLProductInfo() 'nan AFFPLProductInfo()
                objAFFPLProductInfo = objProductInfoList.Item(m_iProductListCount)
                If bIsProductCode Then
                    If objAppContainer.objDataEngine.ValidateUsingPCAndBC(objAFFPLProductInfo.BootsCode, _
                                                                          strBarcode) Then
                        strPrdCode = objAFFPLProductInfo.ProductCode
                        bIsProductValid = True
                        m_ScannedCount += 1
#If NRF Then
                    Else
                        'DARWIN checking if the base barcode is present in Database/Conroller
                        If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                            'DARWIN converting database to Base Barcode
                            strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                            If objAppContainer.objDataEngine.ValidateUsingPCAndBC(objAFFPLProductInfo.BootsCode, _
                                                                          strBarcode) Then
                                strPrdCode = objAFFPLProductInfo.ProductCode
                                bIsProductValid = True
                                m_ScannedCount += 1
                            End If
                        End If
#End If
                    End If
                Else
                    If objAFFPLProductInfo.BootsCode.Equals(strBarcode) Then
                        strPrdCode = objAFFPLProductInfo.ProductCode
                        bIsProductValid = True
                    End If
                End If
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_SF) Or _
                   m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_OSSR) Then
                objEXPLProductInfo = New PLProductInfo() 'nan- EXPLProductInfo ()
                objEXPLProductInfo = objProductInfoList.Item(m_iProductListCount)
                If bIsProductCode Then
                    'Fix for 2377
                    'If objEXPLProductInfo.ProductCode.Equals(strBarcode) Or _
                    '   objEXPLProductInfo.FirstBarcode.Equals(strBarcode) Then
                    If objAppContainer.objDataEngine.ValidateUsingPCAndBC(objEXPLProductInfo.BootsCode, _
                                                                          strBarcode) Then
                        strPrdCode = objEXPLProductInfo.ProductCode
                        bIsProductValid = True
#If NRF Then
                    Else
                        'DARWIN checking if the base barcode is present in Database/Conroller
                        If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                            'DARWIN converting database to Base Barcode
                            strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                            If objAppContainer.objDataEngine.ValidateUsingPCAndBC(objEXPLProductInfo.BootsCode, _
                                                                         strBarcode) Then
                                strPrdCode = objEXPLProductInfo.ProductCode
                                bIsProductValid = True
                            End If
                        End If
#End If
                    End If
                Else
                    If objEXPLProductInfo.BootsCode.Equals(strBarcode) Then
                        strPrdCode = objEXPLProductInfo.ProductCode
                        bIsProductValid = True
                        'Declare new variable for pritning labels.
                        Dim objPSProdInfo As PSProductInfo = New PSProductInfo()
                        'Setting app Module back to the original module
                        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PICKGLST
                        'Does the Price check for Picking List module. 
                        'If price check fails then prompts for full price check
                        'Dim bIsPriceCompareSuccess As Boolean = True
                        'Does the partial price check to determine whether SEL needs to be queued
                        If Not m_strSEL.Equals("") Then
                            Dim strTemp As String
#If NRF Then
                            strTemp= m_ModulePriceCheck.DoPartialPriceCheck(objEXPLProductInfo.ProductCode, _
                                                                            m_strSEL)
#ElseIf RF Then
                            strTemp = m_ModulePriceCheck.DoPartialPriceCheck(m_strSEL, objEXPLProductInfo.Price)
#End If
                            If strTemp = "0" Then
                                If objAppContainer.bMobilePrinterAttachedAtSignon Then
                                    objAppContainer.objDataEngine.GetProductInfoUsingPC(objEXPLProductInfo.ProductCode, _
                                                                                        objPSProdInfo)
#If RF Then
                                    objAppContainer.objDataEngine.GetLabelDetails(objPSProdInfo)
#End If
                                    objPSProdInfo.LabelQuantity = 1
                                    MobilePrintSessionManager.GetInstance.CreateLabels(objPSProdInfo)
                                Else
#If NRF Then
                                    'Identifies that the SEL needs to be queued
                                    objEXPLProductInfo.NumSELsQueued += 1
#ElseIf RF Then
                                    'Incase if in RF and mobile printer not attached send PRT request.
                                    objAppContainer.objExportDataManager.CreatePRT(objEXPLProductInfo.BootsCode, _
                                                                                   SMTransactDataManager.ExFileType.EXData)
#End If
                                End If
                                '#If RF Then
                                '                              'In RF Replacement SEL message is not shown in module price check.
                                '                              'This is because this has to be shown only after printing the SEL's
                                '                              'So , after printing the SEL using mobile printer or Sending PRT to the controller - showing the message in RF
                                '                              MessageBox.Show(MessageManager.GetInstance().GetMessage("M16"), "Replace SEL", _
                                '                              MessageBoxButtons.OK, _
                                '                              MessageBoxIcon.Asterisk, _
                                '                              MessageBoxDefaultButton.Button1)
                                '#End If

                                ' If Not bIsPriceCompareSuccess Then
                                objProductInfoList.RemoveAt(m_iProductListCount)
                                objProductInfoList.Insert(m_iProductListCount, objEXPLProductInfo)
                                objAppContainer.objGlobalPLMappingTable.Remove(m_CurrentPickingList.ListID)
                                objAppContainer.objGlobalPLMappingTable.Add(m_CurrentPickingList.ListID, objProductInfoList)
                                'm_bIsFullPriceCheckRequired = False
                                m_strSEL = ""
                            End If
                        End If
                    End If
                End If
            End If
            'If the Product is valid then displays the screen corresponding to the list type
            If Not bIsProductValid Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M10"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                m_strSEL = ""
                'MessageBox.Show("This is wrong item. Please try again.")
            Else
                If m_CurrentPickingList.ListType.Equals(Macros.SHELF_MONITOR_PL) Or _
                        m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL) Then
                    DisplayPLScreen(PLSCREENS.SMItemDetails)
                ElseIf m_CurrentPickingList.ListType.Equals(Macros.FAST_FILL_PL) Then
                    m_FillType = FastFillType.ManualFastFill
                    'Updates the FF Picking List
                    If UpdateFFProductInfo(strPrdCode) Then
                        DisplayPLScreen(PLSCREENS.FFItemDetails)
                    End If
                ElseIf m_CurrentPickingList.ListType.Equals(Macros.AUTO_FAST_FILL_PL) Then
                    m_FillType = FastFillType.AutoFastFill
                    'Updates the FF Picking List
                    If UpdateAFFProductInfo(strPrdCode) Then
                        DisplayPLScreen(PLSCREENS.AFFItemDetails)
                    End If
#If RF Then
                ElseIf m_CurrentPickingList.ListType.Equals(Macros.OSSR_PL) Or _
                        m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_OSSR) Then
                    'In case of FF and AFF list converted to OSSR list send the PLC transaction.
                    If m_OSSRGapFlag.Equals(Macros.PLC_FF_FLAG) Then
                        UpdateOSSRProductInfo(strPrdCode, 0)
                    End If
                    'To display any picking list converted to OSSR location or an OSSR list converted to 
                    'back shop  location.
                    DisplayPLScreen(PLSCREENS.OSSRItemDetails)
#End If
                ElseIf m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_SF) Or _
                       m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_OSSR) Then

                    'Dim bIsMultiSiteAlreadyDone As Boolean = False
                    'For Each objExMultiSiteData As EXMultiSiteInfo In objAppContainer.objEXMultiSiteList
                    '    If objExMultiSiteData.m_strListId.Equals(m_CurrentPickingList.ListID) AndAlso _
                    '        objExMultiSiteData.m_strProductCode.Equals(strPrdCode) Then
                    '        bIsMultiSiteAlreadyDone = True
                    '        Exit For
                    '    End If
                    'Next
                    'If Not bIsMultiSiteAlreadyDone Then
                    'Dim multiSiteList As ArrayList = New ArrayList()
                    'Retrives the multisite info
                    'If GetEXMultiSiteInfo(strPrdCode, multiSiteList) Then

                    'nan moved below code to DisplayPLItemConfirm
                    'If m_MultiSiteList.Count > 1 Then
                    '    If Not IsMultisiteTrackAdded(strPrdCode, m_CurrentPickingList.ListID) Then
                    '        Dim objEXPLData As EXMultiSiteInfo
                    '        Dim objSeqNum As Integer = 1
                    '        For Each strLocation As PlannerInfo In m_MultiSiteList
                    '            objEXPLData = New EXMultiSiteInfo
                    '            objEXPLData.m_SeqNum = objSeqNum.ToString.PadLeft(3, "0")
                    '            objEXPLData.m_strListId = m_CurrentPickingList.ListID
                    '            objEXPLData.m_strProductCode = strPrdCode
                    '            objEXPLData.m_strPlannerDesc = strLocation.POGDesc
                    '            objEXPLData.m_strPOGDescription = strLocation.Description
                    '            objEXPLData.m_iSalesFloorQty = 0
                    '            objEXPLData.m_bIsCounted = False

                    '            objAppContainer.objEXMultiSiteList.Add(objEXPLData)
                    '            objSeqNum = objSeqNum + 1
                    '        Next

                    '        Dim objEXPLOthersData As EXMultiSiteInfo = New EXMultiSiteInfo()
                    '        objEXPLOthersData.m_SeqNum = objSeqNum.ToString.PadLeft(3, "0")
                    '        objEXPLOthersData.m_strListId = m_CurrentPickingList.ListID
                    '        objEXPLOthersData.m_strPlannerDesc = ""
                    '        objEXPLOthersData.m_strProductCode = strPrdCode
                    '        objEXPLOthersData.m_strPOGDescription = "Other"
                    '        objEXPLOthersData.m_iSalesFloorQty = 0
                    '        objEXPLOthersData.m_bIsCounted = True
                    '        objAppContainer.objEXMultiSiteList.Add(objEXPLOthersData)
                    '    End If
                    'End If
                    'End If
                    ''Claering the arraylist which contains multisite information
                    'm_MultiSiteList.Clear()
                    'm_tempMultiSiteList.Clear()
                    'End If
                    DisplayPLScreen(PLSCREENS.EXItemDetails)
                End If
            End If

            Cursor.Current = Cursors.Default
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessBarcodeEntered of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessBarcodeEntered of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Displays the Picking List Home Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayPLHome(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Entered DisplayPLHome of PLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim ListType As String = Nothing
            Dim arrTempObjectList As New ArrayList()

            ' Change made for OSSR picking list visibility issue fix- Kiran Krishnan 13D release
#If RF Then
            Dim ElementList As Array = "S,E,A,F,N,O".Split(",")
#End If
#If NRF Then
            Dim ElementList As Array = "S,E,A,F,N".Split(",")
#End If
            Dim TempArrayList As New ArrayList()
            'Setting OSSRCount
            'm_OSSRCount = 0
            'Check for new items in case of rf mode
#If RF Then
            'Everytime the pickinglist is loaded clear the global array list and then create.
            objAppContainer.objGlobalPLMappingTable.Clear()
            objAppContainer.objGlobalPickingList.Clear()
            'Fix to instantly load all the picking list whenever the PL home is loaded.
            'Dim objLocalPickingList As ArrayList = New ArrayList()
            If Not objAppContainer.objDataEngine.GetPickingList(TempArrayList) Or _
               TempArrayList.Count = 0 Then
                'if no picking list is returned at any point in time during a session
                'clear the global array list and exit picking lis tmodule.
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M24"), "Info", _
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                'Call end session as there is no picking list available.
                EndSession()
                Exit Sub
            Else
                'SIT SFA -Sort picking list in RF mode
                For Each PickListItem As String In ElementList
                    For Each PickingListObject As PickingList In TempArrayList
                        If PickingListObject.ListType = PickListItem Then
                            'arrObjectList.Add(PickingListObject)
                            'TempArrayList.Remove(PickingListObject)
                            PickingListObject.ListTime = PickingListObject.ListTime.PadLeft(4, "0")
                            arrTempObjectList.Add(PickingListObject)
                        End If
                    Next
                    arrTempObjectList.Sort()
                    arrTempObjectList.Reverse()
                    For Each PickingListObject As PickingList In arrTempObjectList
                        objAppContainer.objGlobalPickingList.Add(PickingListObject)
                    Next
                    arrTempObjectList.Clear()
                Next
            End If
#End If
            'bOSSRToggled = False
            With m_PLHome
                .lstView.Clear()
                .lstView.Columns.Add("Time", 48 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                .lstView.Columns.Add("Type", 40 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                .lstView.Columns.Add("User", 96 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                .lstView.Columns.Add("Items", 48 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                'Fix for MultiUser
                '.lstView.Columns.Add("  ", 14 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                'Populate data from the list to list view
                For Each objPickingListItem As PickingList In objAppContainer.objGlobalPickingList
                    Dim lstItem As ListViewItem = New ListViewItem()
                    Select Case objPickingListItem.ListType
                        Case "S"
                            ListType = Macros.SHELF_MONITOR
                        Case "F"
                            ListType = Macros.FAST_FILL
                        Case "B"
                            ListType = Macros.EX_PL_BS
                        Case "C"
                            ListType = Macros.EX_PL_OSSR
                            'ListType = "EO"
                        Case "D"
                            ListType = Macros.EXCESS_STOCK
                        Case "E"
                            ListType = Macros.EXCESS_STOCK
                        Case "A"
                            ListType = Macros.AUTO_FAST_FILL
                        Case "O"
                            ListType = Macros.OSSR_Type
                        Case Else
                            ListType = "N"
                    End Select
                    Dim strTime As String = ""
#If NRF Then
                    If Not objPickingListItem.ListTime.Equals("") Then
                        'Padded zero to time
                        strTime = objPickingListItem.ListTime.PadLeft(4, "0")
                        strTime = strTime.Insert(2, ":")
                        'strTime = objPickingListItem.ListTime.Insert(2, ":")
                    End If
                    'lstItem.Text = strTime
                    'System Testing Bug Fix
                    lstItem.Text = strTime.Substring(0, 5)
#ElseIf RF Then
                    If Not objPickingListItem.ListTime.Equals("") Then
                        strTime = objPickingListItem.ListTime.Insert(10, ":")
                    End If
                    'lstItem.Text = strTime
                    'System Testing Bug Fix
                    lstItem.Text = strTime.Substring(8, 5)
#End If
                    'lstItem.Text = strTime.Substring(9, 12)
                    'lstItem.Text = objPickingListItem.ListTime

                    lstItem.SubItems.Add(ListType)
                    'To Do
                    lstItem.SubItems.Add(objPickingListItem.Creator.ToString.Trim())
                    'SFA ST - Trim the leading zeros
                    Dim item As String = objPickingListItem.TotalItems.ToString.TrimStart("0")
                    'CR for Multi Users
                    If objPickingListItem.ListStatus = "A" Then
                        'nan TODO inclure * as subscript
                        item = item + "*"
                        ' lstItem.SubItems.Add("*")
                    End If
                    lstItem.SubItems.Add(item)
                    'Allow normal user to access only FF and AFF list
                    'Remove other listtype for normal user.
                    If Not objAppContainer.bIsStockSpecialist Then
                        If (objPickingListItem.ListType <> "A") And (objPickingListItem.ListType <> "F") Then
                            lstItem.ForeColor = Color.Gray
                        End If
                    End If
                    .lstView.Items.Add(lstItem)
                    lstItem = Nothing
                Next
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayPLHome of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayPLHome of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Displays the Picking List Item Confirmation screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayPLItemConfirm(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Entered DisplayPLItemConfirm of PLSessionMgr", Logger.LogLevel.INFO)
        Try

            Dim objCurrentProductInfoList As ArrayList = New ArrayList()
            Dim objCurrentProductInfo As PLProductInfo 'nan- new common variable
            Dim objOSSRCurrentProductInfo As PLProductInfo
            Dim bPSPPending As Boolean = False
            

            m_IsMultisite = False
            'Gets the list of objects from the hashtable 

            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If
            'if no from Finish Screen
            If m_iCancelQuit = 1 Then
                m_iProductListCount = 0
                objCurrentProductInfo = objCurrentProductInfoList.Item(0)
                Dim bExit As Boolean = False
                For Each objProductInfo As PLProductInfo In objCurrentProductInfoList
                    If objProductInfo.MultiSiteList.Count > 1 Then
                        For Each objMultisiteData As EXMultiSiteInfo In objProductInfo.MultiSiteList
                            If objMultisiteData.m_bIsCounted = False And _
                            objMultisiteData.m_strPOGDescription <> "Other" And _
                            objMultisiteData.m_strPlannerDesc <> "Pending Sales Plan" Then
                                objCurrentProductInfo = objProductInfo
                                bExit = True
                                Exit For
                            End If
                        Next
                    ElseIf Not objProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                        objCurrentProductInfo = objProductInfo
                        bExit = True
                        Exit For
                    End If
                    If bExit Then
                        Exit For
                    End If
                    m_iProductListCount = m_iProductListCount + 1
                Next
                If Not bExit Then
                    m_iProductListCount = 0
                End If
                'if no from PSPPending Screen
            ElseIf m_iCancelQuit = 2 Then
                m_iProductListCount = 0
                Dim bExit As Boolean = False
                For Each objProductInfo As PLProductInfo In objCurrentProductInfoList
                    If objProductInfo.MultiSiteList.Count > 1 Then
                        Dim objMBSData As EXMultiSiteInfo = objProductInfo.MultiSiteList.Item(0)
                        Dim objPSPData As EXMultiSiteInfo = objProductInfo.MultiSiteList.Item(1)
                        If objPSPData.m_bIsCounted = False Or objMBSData.m_bIsCounted = False Then
                            objCurrentProductInfo = objProductInfo
                            bExit = True
                            Exit For
                        End If
                    ElseIf Not objProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                        objCurrentProductInfo = objProductInfo
                        bExit = True
                        Exit For
                    End If
                    If bExit Then
                        Exit For
                    End If
                    m_iProductListCount = m_iProductListCount + 1
                Next
                If Not bExit Then
                    m_iProductListCount = 0
                End If
            Else
                objCurrentProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)
            End If
            'nan set the global variable which has the current multisite information
            'objAppContainer.objEXMultiSiteList.Clear()
            objAppContainer.objEXMultiSiteList = objCurrentProductInfo.MultiSiteList

            With m_PLItemConfirm

                Dim objDescriptionArray As ArrayList = New ArrayList()
                Dim strProductCode As String = ""

                strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(objCurrentProductInfo.ProductCode)

                .lblBootsCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(objCurrentProductInfo.BootsCode)
                .lblProductCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(strProductCode)
                .lblStatusDisplay.Text = objAppContainer.objHelper.GetStatusDescription(objCurrentProductInfo.Status)
                If objCurrentProductInfo.TSF.Substring(0, 1).Equals("-") Then
                    .lblStockFigure.ForeColor = Color.Red
                Else
                    .lblStockFigure.ForeColor = .lblStatusDisplay.ForeColor
                End If
                .lblStockFigure.Text = objCurrentProductInfo.TSF
                .lblTotalItemCountHeader.Visible = True
                .lblTotalItemCount.Visible = True


                .Text = m_FormHeader

                If m_CurrentPickingList.ListType.Equals(Macros.FAST_FILL_PL) Or _
                 m_CurrentPickingList.ListType.Equals(Macros.AUTO_FAST_FILL_PL) Then

                    'nan removed check for negetive value
                    '.lblTotalItemCount.Text = objCurrentProductInfo.SalesFloorQuantity.ToString()
                    .lblTotalItemCountHeader.Visible = False
                    .lblTotalItemCount.Visible = False

                ElseIf (m_CurrentPickingList.ListType.Equals(Macros.OSSR_PL)) Or _
                 m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_OSSR) Then
                    objOSSRCurrentProductInfo = New PLProductInfo() 'Changed to PL Product Info
                    objOSSRCurrentProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)

                    'nan TODO tocheck Gap Flag

                    ''@@@@@@@@Set item GAP Flag for the picking list based on PLI@@@@@@@
#If RF Then
                    m_OSSRGapFlag = objOSSRCurrentProductInfo.GapFlag
#End If
                    ''@@@@@@Changes end@@@@@@

                End If
                If m_CurrentPickingList.ListType.Equals(Macros.OSSR_PL) Or _
               m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_OSSR) Or _
               m_CurrentPickingList.ListType = Macros.SHELF_MONITOR_PL Or _
               m_CurrentPickingList.ListType = Macros.EXCESS_STOCK_PL Then
                    'System Testing SFA - Fix for single site item
                    If objCurrentProductInfo.MultiSiteList.Count > 0 Then
                        Dim objMBSData As EXMultiSiteInfo = New EXMultiSiteInfo()
                        Dim objPSPData As EXMultiSiteInfo = New EXMultiSiteInfo()
                        objMBSData = objCurrentProductInfo.MultiSiteList.Item(0)
                        objPSPData = objCurrentProductInfo.MultiSiteList.Item(1)
                        If objMBSData.m_bIsCounted And objPSPData.m_bIsCounted = False Then
                            bPSPPending = True
                        End If
                    End If
                End If
                If objCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) And bPSPPending = False Then
                    If m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_SF) Or _
                       m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_OSSR) Then
                        .lblPicked.Text = "COUNTED"
                    Else
                        .lblPicked.Text = "PICKED"
                    End If
                    .lblPicked.Visible = True
                    'Fix for Gap button
                    .Btn_GAP1.Visible = False

                Else
                    'Gap button needs to be visible for FF type picking list only if item is
                    'not picked
                    .Btn_GAP1.Visible = True
                    .lblPicked.Visible = False
                End If
                bPSPPending = False
                'Gets the formatted description
                objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(objCurrentProductInfo.Description)


                .lblItemPosition.Text = (m_iProductListCount + 1).ToString() + "/" + objCurrentProductInfoList.Count.ToString()

                .lblProductDesc1.Text = objDescriptionArray.Item(0)
                .lblProductDesc2.Text = objDescriptionArray.Item(1)
                .lblProductDesc3.Text = objDescriptionArray.Item(2)

                .lblStockFigureHeader.Text = objAppContainer.objHelper.GetStockFileHeader()

                Dim multisiteList As ArrayList = New ArrayList()
                Dim iNumLocations As Integer = 0
                Dim bIsAllLocCounted As Boolean = True

                m_IsMultisite = False
                'nan If counting in SF
                If (IsMultisiteTrackAdded(objCurrentProductInfo.ProductCode, m_CurrentPickingList.ListID, objCurrentProductInfo.MultiSiteList)) Then
                    For Each objExMultisiteData As EXMultiSiteInfo In objCurrentProductInfo.MultiSiteList
                        'nan set multisite as true
                        m_IsMultisite = True
                        If objExMultisiteData.m_strListId.Equals(m_CurrentPickingList.ListID) AndAlso _
                            objExMultisiteData.m_strProductCode.Equals(objCurrentProductInfo.ProductCode) Then
                            If ((objExMultisiteData.m_bIsCounted = True) AndAlso _
                            (objExMultisiteData.m_strPOGDescription <> "Other")) Then
                                'SFA UAT DEF #820 - aligned the display
                                multisiteList.Add("*" & objExMultisiteData.m_strPlannerDesc.ToString().Trim() & " - " & objExMultisiteData.m_strPOGDescription.Trim() & " (Counted)")
                            ElseIf objCurrentProductInfo.PendingSalesFlag Then
                                'nan IF pending sales flag is true then count is in Back Shop
                                multisiteList.Add(objExMultisiteData.m_strPlannerDesc.ToString())
                            ElseIf objExMultisiteData.m_strPOGDescription = "Other" Then
                                multisiteList.Add(objExMultisiteData.m_strPOGDescription.Trim())
                            Else
                                multisiteList.Add(objExMultisiteData.m_strPlannerDesc.ToString().Trim() & " - " & objExMultisiteData.m_strPOGDescription.Trim())
                            End If

                            If bIsAllLocCounted Then
                                If Not objExMultisiteData.m_bIsCounted Then
                                    bIsAllLocCounted = False
                                End If
                            End If
                        End If
                    Next
                End If



                'Set Total Item Quantity
                'If m_CurrentPickingList.IsSalesFloorList Then
                '    If objCurrentProductInfo.SalesFloorQuantity = -1 Then
                '        .lblTotalItemCount.Text = "0"
                '    Else
                '        .lblTotalItemCount.Text = objCurrentProductInfo.SalesFloorQuantity
                '    End If

                'Else
                '    If objCurrentProductInfo.BackShopQuantity = -1 Then
                '        .lblTotalItemCount.Text = "0"
                '    Else
                '        .lblTotalItemCount.Text = objCurrentProductInfo.BackShopQuantity
                '    End If
                'End If


                Dim iTotalCount As Integer = 0
                Dim iBSQty As Integer = 0
                Dim iSFQty As Integer = 0
                Dim iOSSRQty As Integer = 0

                If objCurrentProductInfo.SalesFloorQuantity < 0 Then
                    iSFQty = 0
                Else
                    iSFQty = objCurrentProductInfo.SalesFloorQuantity
                End If
                If objCurrentProductInfo.BackShopQuantity < 0 Then
                    iBSQty = 0
                Else
                    iBSQty = objCurrentProductInfo.BackShopQuantity
                End If
                If objCurrentProductInfo.OSSRQuantity < 0 Then
                    iOSSRQty = 0
                Else
                    iOSSRQty = objCurrentProductInfo.OSSRQuantity
                End If

                iTotalCount = iBSQty + iSFQty + iOSSRQty
                .lblTotalItemCount.Text = iTotalCount

                If multisiteList.Count > 1 Then
                    .cmbLocation.Visible = True
                    .cmbLocation.Items.Clear()
                    .lblSite.Visible = True
                    'nan Remove Select
                    '.cmbLocation.Items.Add("Select")
                    For Each strLocation As String In multisiteList
                        .cmbLocation.Items.Add(strLocation)
                    Next
                    If bFromViewScreen Or m_iCancelQuit Then
                        Dim iIndex As Integer = 0
                        For Each objExMultisiteData As EXMultiSiteInfo In objCurrentProductInfo.MultiSiteList
                            If Not objExMultisiteData.m_bIsCounted Then
                                iIndex = Convert.ToInt16(objExMultisiteData.m_SeqNum)
                                Exit For
                            End If
                        Next
                        If iIndex = 0 Then
                            .cmbLocation.SelectedIndex() = iIndex
                        Else
                            .cmbLocation.SelectedIndex() = iIndex - 1
                        End If
                    Else
                        .cmbLocation.SelectedIndex() = m_SelectedSite
                    End If
                    bFromViewScreen = False
                    'nan Dispays form according to the site selected
                    CheckSelectedSite()

                Else
                    .cmbLocation.Visible = False
                    .lblSite.Visible = False
                End If
                m_iCancelQuit = 0

                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayPLItemConfirm of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayPLItemConfirm of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Go to site confirmation screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CheckSelectedSite()

        With m_PLItemConfirm
            ' If .cmbLocation.SelectedIndex() <> 0 Then
            .Label1.Visible = True
            .Btn_CalcPad_small1.Visible = True
            .Btn_CalcPad_small1.Enabled = True
            .lblSite.Text = "Select Site"
            .lblSite.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)

            'Else
            '.Label1.Visible = False
            '.Btn_CalcPad_small1.Visible = False
            '.Btn_CalcPad_small1.Enabled = False
            '.lblSite.Text = "Select Site"
            '.lblSite.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)


            'End If
        End With

    End Sub
    ''' <summary>
    ''' Go to site confirmation screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GetSiteConfirmation()

        DisplayPLScreen(PLSCREENS.ItemConfirm)

    End Sub



    ''' <summary>
    ''' Displays the Picking List SM item details
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplaySMPLItemDetails(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Entered DisplaySMPLItemDetails of PLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim objCurrentProductInfoList As ArrayList = New ArrayList()
            'nan Dim objSMCurrentProductInfo As SMPLProductInfo
            Dim objSMCurrentProductInfo As PLProductInfo
            'Gets the list of objects from the hashtable 
            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If

            With m_SMPLItemDetails
                .lblQtyReqHeader.Visible = False
                .lblQtyRequired.Visible = False
                .lblOf.Visible = False
                .lblTotQtyRequired.Visible = False
                Dim objDescriptionArray As ArrayList = New ArrayList()
                Dim bIsAlreadyPicked As Boolean = False
                'Checks the type of picking list and sets the values to the controls accordingly
                'The position of the item in the list is indicated by m_iProductListCount variable
                'Gets the value into an object of SMPLProductInfo and sets tghe values
                objSMCurrentProductInfo = New PLProductInfo() 'nan- SMPLProductInfo
                objSMCurrentProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)
                If objSMCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                    bIsAlreadyPicked = True
                End If

                .Text = m_FormHeader
                .lblSalesFloorQtyHeader.Text = objAppContainer.objHelper.GetStockFileHeader()
                'Gets the formatted description
                objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(objSMCurrentProductInfo.Description)
                Dim strProductCode As String = ""

                strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(objSMCurrentProductInfo.ProductCode)
                .lblItemPosition.Text = (m_iProductListCount + 1).ToString() + "/" + objCurrentProductInfoList.Count.ToString()
                .lblBootsCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(objSMCurrentProductInfo.BootsCode)
                .lblProductCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(strProductCode)
                .lblStatusDisplay.Text = objAppContainer.objHelper.GetStatusDescription(objSMCurrentProductInfo.Status)
                If objSMCurrentProductInfo.TSF.Substring(0, 1).Equals("-") Then
                    .lblStockFigure.ForeColor = Color.Red
                Else
                    .lblStockFigure.ForeColor = .lblStatusDisplay.ForeColor
                End If
                .lblStockFigure.Text = objSMCurrentProductInfo.TSF

                .lblProductDesc1.Text = objDescriptionArray.Item(0)
                .lblProductDesc2.Text = objDescriptionArray.Item(1)
                .lblProductDesc3.Text = objDescriptionArray.Item(2)

                Dim iTotalCount As Integer = 0
                Dim iBSQty As Integer = 0
                Dim iSFQty As Integer = 0
                Dim iOSSRQty As Integer = 0

                If objSMCurrentProductInfo.SalesFloorQuantity < 0 Then
                    iSFQty = 0
                Else
                    iSFQty = objSMCurrentProductInfo.SalesFloorQuantity
                End If
                If objSMCurrentProductInfo.BackShopQuantity < 0 Then
                    iBSQty = 0
                Else
                    iBSQty = objSMCurrentProductInfo.BackShopQuantity
                End If
                If objSMCurrentProductInfo.OSSRQuantity < 0 Then
                    iOSSRQty = 0
                Else
                    iOSSRQty = objSMCurrentProductInfo.OSSRQuantity
                End If
                iTotalCount = iBSQty + iSFQty + iOSSRQty

                'nan including total item count
                'If objSMCurrentProductInfo.BackShopQuantity = -1 Then
                '.lblTotalItemCount.Text = "0"
                'Else
                .lblTotalItemCount.Text = iTotalCount.ToString()
                'End If


                'ambli
                'For OSSR
#If RF Then
                'nan remove header
                '.lblStockFigureHeader.Text = "Stock Figure:"
                'Hide the cal pad button not to allow the users to
                're-enter item count without scanning the item again or 
                'selecting next or back button. fix for 5059
                If bHideCalcPad Then
                    .btnBackShopCalcpad.Visible = False
                Else
                    .btnBackShopCalcpad.Visible = True
                End If
                'Bug Fix for OSSR Visiblity
                'If Not bOSSRToggled Then
                If objAppContainer.OSSRStoreFlag = "Y" Then
                    If objSMCurrentProductInfo.OSSRFlag = "O" Then
                        'm_OSSRCount = 1
                        OOSRStatus(strProductCode) = "Y"
                        .lblOSSR.Text = "OSSR"
                        '.lblBackShopHeader.Text = "Enter Off Site Qty: "
                        If objSMCurrentProductInfo.BackShopQuantity < 0 Then
                            .lblBackShopQty.Text = 0
                        Else
                            .lblBackShopQty.Text = objSMCurrentProductInfo.BackShopQuantity.ToString()
                        End If
                    Else
                        OOSRStatus(strProductCode) = "N"
                        .lblOSSR.Text = " "
                    End If
                Else
                    .btn_OSSRItem.Visible = False
                    .lblOSSR.Visible = False
                End If
                'End If
#ElseIf NRF Then
                'nan changed value in form
                '.lblTotalItemCountHeader.Text = "Start of Day Stock Figure:"
                .btn_OSSRItem.Visible = False
                .lblOSSR.Visible = False
#End If
                'Fix for allignment for Back and OSSR button
#If RF Then
                If objAppContainer.OSSRStoreFlag <> "Y" Then
                    '.custCtrlBtnBack.Location = New Point(56 * objAppContainer.iOffSet, 246 * objAppContainer.iOffSet)
                    '.btn_OSSRItem.Location = New Point(113 * objAppContainer.iOffSet, 246 * objAppContainer.iOffSet)
                    'Else
                    .custCtrlBtnNext.Location = New Point(4 * objAppContainer.iOffSet, 246 * objAppContainer.iOffSet)
                    .btnZero.Location = New Point(64 * objAppContainer.iOffSet, 246 * objAppContainer.iOffSet)
                    .custCtrlBtnBack.Location = New Point(123 * objAppContainer.iOffSet, 246 * objAppContainer.iOffSet)
                    .custCtrlBtnQuit.Location = New Point(183 * objAppContainer.iOffSet, 246 * objAppContainer.iOffSet)
                    .btn_OSSRItem.Visible = False
                End If
#ElseIf NRF Then
                'nan removed
                '.custCtrlBtnBack.Location = New Point(93 * objAppContainer.iOffSet, 240 * objAppContainer.iOffSet)
                .custCtrlBtnNext.Location = New Point(4 * objAppContainer.iOffSet, 246 * objAppContainer.iOffSet)
                .btnZero.Location = New Point(64 * objAppContainer.iOffSet, 246 * objAppContainer.iOffSet)
                .custCtrlBtnBack.Location = New Point(123 * objAppContainer.iOffSet, 246 * objAppContainer.iOffSet)
                .custCtrlBtnQuit.Location = New Point(183 * objAppContainer.iOffSet, 246 * objAppContainer.iOffSet)
                .btn_OSSRItem.Visible = False
#End If
                'AFF PL CR
#If NRF Then
                .btnView.Visible = True
#ElseIf RF Then
                .btnView.Visible = False
#End If
                If bIsAlreadyPicked Then
                    .lblBackShopHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                    .lblBackShopQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                Else
                    .lblBackShopHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                    .lblBackShopQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                End If

                If objSMCurrentProductInfo.BackShopQuantity = -1 Then
                    .lblBackShopQty.Text = "0"
                Else
                    .lblBackShopQty.Text = objSMCurrentProductInfo.BackShopQuantity.ToString()
                End If


                Dim multisiteList As ArrayList = New ArrayList()
                Dim iNumLocations As Integer = 0
                Dim bIsAllLocCounted As Boolean = True

                If m_IsMultisite Then
                    For Each objSmMultisiteData As EXMultiSiteInfo In objSMCurrentProductInfo.MultiSiteList

                        If objSmMultisiteData.m_strListId.Equals(m_CurrentPickingList.ListID) AndAlso _
                            objSmMultisiteData.m_strProductCode.Equals(objSMCurrentProductInfo.ProductCode) Then

                            If objSmMultisiteData.m_SeqNum = m_SelectedSite + 1 Then
                                'nan Sales floor quantity will be following if multisite
                                .lblBackShopQty.Text = objSmMultisiteData.m_iBackShopQty
                                'nan If the site is the current selected site and if it has been counted.
                                If (objSmMultisiteData.m_bIsCounted = True) Then
                                    objSmMultisiteData.m_CountPicked = SMQuantityToPick(objSMCurrentProductInfo, objSmMultisiteData.m_SeqNum)
                                    .lblQtyReqHeader.Visible = True
                                    .lblQtyReqHeader.Text = "Qty to take to Sales Floor"
                                    .lblQtyReqHeader.Location = New Point(2 * objAppContainer.iOffSet, 212 * objAppContainer.iOffSet)
                                    .lblBackShopHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                                    .lblBackShopQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                                    .lblQtyRequired.Visible = True
                                    .lblOf.Visible = True
                                    .lblTotQtyRequired.Visible = True
                                    .lblTotQtyRequired.Location = New Point(207 * objAppContainer.iOffSet, 212 * objAppContainer.iOffSet)
                                    .lblQtyRequired.Text = objSmMultisiteData.m_CountPicked
                                    .lblTotQtyRequired.Text = objSMCurrentProductInfo.QuantityRequired

                                    Dim selectedSite As EXMultiSiteInfo
                                    selectedSite = objSMCurrentProductInfo.MultiSiteList.Item(m_SelectedSite)

                                    If objSmMultisiteData.m_CountPicked = 0 And _
                                    selectedSite.m_iBackShopQty <> 0 Then
                                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M108"), "Info", _
                                        MessageBoxButtons.OK, _
                                        MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
                                    End If
                                Else
                                    .lblBackShopHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                                    .lblBackShopQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                                    .lblQtyReqHeader.Visible = False
                                    .lblQtyRequired.Visible = False
                                    .lblOf.Visible = False
                                    .lblTotQtyRequired.Visible = False
                                End If
                            End If

                                If (objSmMultisiteData.m_bIsCounted = True) Then
                                    multisiteList.Add("*" & objSmMultisiteData.m_strPlannerDesc.ToString() & " (Counted)")
                                Else
                                    multisiteList.Add(objSmMultisiteData.m_strPlannerDesc.ToString())
                                End If

                                iNumLocations += 1
                                If bIsAllLocCounted Then
                                    If Not objSmMultisiteData.m_bIsCounted Then
                                        bIsAllLocCounted = False
                                    End If
                                End If

                            End If
                    Next
                Else  'System Testing SFA - Fix to display quantity to take to sales floor for single site item
                    If bIsAlreadyPicked Then
                        'System testing SFA - Msg included for single site tiem also
                        .lblQtyReqHeader.Visible = True
                        .lblQtyReqHeader.Text = "Qty to take to Sales Floor"
                        .lblQtyReqHeader.Location = New Point(2 * objAppContainer.iOffSet, 212 * objAppContainer.iOffSet)
                        .lblQtyRequired.Visible = True
                        .lblOf.Visible = True
                        .lblTotQtyRequired.Visible = True
                        .lblTotQtyRequired.Location = New Point(207 * objAppContainer.iOffSet, 212 * objAppContainer.iOffSet)
                        .lblTotQtyRequired.Text = objSMCurrentProductInfo.QuantityRequired
                        If objSMCurrentProductInfo.BackShopQuantity > objSMCurrentProductInfo.QuantityRequired Then
                            .lblQtyRequired.Text = objSMCurrentProductInfo.QuantityRequired
                        Else
                            .lblQtyRequired.Text = objSMCurrentProductInfo.BackShopQuantity
                        End If
                    End If
                End If

                If m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL) Then
                    .lblQtyReqHeader.Visible = False
                    .lblQtyRequired.Visible = False
                    .lblOf.Visible = False
                    .lblTotQtyRequired.Visible = False
                End If

                'Checking if all locations are handled before setting the item as counted
                If m_IsMultisite Then
                    If Not (iNumLocations > 1 And bIsAllLocCounted) Then
                        bIsAlreadyPicked = False
                    End If
                End If

                'nan check if item picked in all the sites
                If bIsAlreadyPicked Then
                    .lblPicked.Visible = True
                    .lblPicked.Text = "PICKED"
                Else
                    .lblPicked.Visible = False
                End If

                'nan check if item is counted in current site
                'If .lblBackShopQty.Text <> "0" Then
                '    .lblQtyReqHeader.Visible = True
                '    .lblQtyRequired.Visible = True
                '    .lblOf.Visible = True
                '    .lblTotQtyRequired.Visible = True
                '    .lblTotQtyRequired.Text = objSMCurrentProductInfo.QuantityRequired

                '    'nan returns the number of items to pick from site
                '    Dim quantityToPick As Integer
                '    'quantityToPick = SMQuantityToPick(objSMCurrentProductInfo.BackShopQuantity, _
                '    'objSMCurrentProductInfo.QuantityRequired, _
                '    'objSMCurrentProductInfo.SMQuantityPicked)

                '    'updates the number of items picked for product
                '    ' objSMCurrentProductInfo.QuantityPicked += quantityToPick

                'Else
                '    .lblQtyReqHeader.Visible = False
                '    .lblQtyRequired.Visible = False
                '    .lblOf.Visible = False
                '    .lblTotQtyRequired.Visible = False
                'End If

                'nan TODO .. remove ex pl code
                'ElseIf m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL) Then
                '.lblQtyReqHeader.Visible = False
                '.lblQtyRequired.Visible = False
                'End If

                If multisiteList.Count > 1 Then
                    .lblLocationHeader.Visible = True
                    .cmbLocation.Visible = True
                    .cmbLocation.Items.Clear()
                    'nan removed select as this wont be required
                    '.cmbLocation.Items.Add("Select")
                    For Each strLocation As String In multisiteList
                        .cmbLocation.Items.Add(strLocation)
                    Next

                    .cmbLocation.SelectedIndex = m_SelectedSite

                Else
                    'Code to adjust position of gap, next, back and quit button
                    .lblLocationHeader.Visible = False
                    .cmbLocation.Visible = False
                End If


                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplaySMPLItemDetails of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplaySMPLItemDetails of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub


    ''' <summary>
    ''' returns the number of items to pick from current site
    ''' </summary>
    ''' <remarks></remarks>
    Private Function SMQuantityToPick(ByRef product As PLProductInfo, ByVal site As Integer) As Integer
        Dim quantityToPick As Integer = 0
        Dim selectedSite As EXMultiSiteInfo
        Dim otherSite As EXMultiSiteInfo
        Dim quantityPicked As Integer = 0
        Dim quantityRequired As Integer = 0
        Dim quantityAvailable As Integer = 0

        'nan Check which is current site and find quantity picked in other site
        'nan site = 1 then current site is MBS else PSP
        If site = 1 Then
            selectedSite = product.MultiSiteList.Item(0)
            otherSite = product.MultiSiteList.Item(1)
        Else
            otherSite = product.MultiSiteList.Item(0)
            selectedSite = product.MultiSiteList.Item(1)
        End If

        quantityAvailable = selectedSite.m_iBackShopQty
        quantityPicked = otherSite.m_CountPicked
        quantityRequired = product.QuantityRequired - quantityPicked

        If quantityAvailable > quantityRequired Then
            quantityToPick = quantityRequired
        Else
            quantityToPick = quantityAvailable
        End If

        Return quantityToPick
        'If countPresent = -1 Then
        '    countPresent = 0
        'End If
        'Dim countPending As Integer
        'Dim countAvailable As Integer
        'If countRequired > countPicked Then
        '    countPending = countRequired - countPicked
        '    countAvailable = countPresent - countPicked
        '    If countAvailable > countPending Then
        '        Return countPending
        '    Else
        '        Return countAvailable
        '    End If
        'End If
    End Function


    ''' <summary>
    ''' Displays the Picking List FF item details
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayAFFPLItemDetails(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Entered DisplayAFFPLItemDetails of PLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim objCurrentProductInfoList As ArrayList = New ArrayList()
            Dim objAFFCurrentProductInfo As PLProductInfo 'nan AFFPLProductInfo

            'Gets the list of objects from the hashtable 

            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If

            With m_AFFPL

                Dim objDescriptionArray As ArrayList = New ArrayList()
                Dim bIsAlreadyPicked As Boolean = False

                .Text = m_FormHeader

                'Checks the type of picking list and sets the values to the controls accordingly
                'The position of the item in the list is indicated by m_iProductListCount variable

                'Gets the value into an object of SMPLProductInfo and sets tghe values
                objAFFCurrentProductInfo = New PLProductInfo() 'nan AFFPLProductInfo()
                objAFFCurrentProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)

                If objAFFCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                    bIsAlreadyPicked = True
                Else
                    m_PickedCount += 1
                End If

                'Gets the formatted description
                objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(objAFFCurrentProductInfo.Description)

                Dim strProductCode As String = ""
                strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(objAFFCurrentProductInfo.ProductCode)

                .lblItemPosition.Text = (m_iProductListCount + 1).ToString() + "/" + objCurrentProductInfoList.Count.ToString()
                .lblBootsCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(objAFFCurrentProductInfo.BootsCode)
                .lblProductCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(strProductCode)
                .lblStatusDisplay.Text = objAppContainer.objHelper.GetStatusDescription(objAFFCurrentProductInfo.Status)
                If objAFFCurrentProductInfo.TSF.Substring(0, 1).Equals("-") Then
                    .lblStockFigure.ForeColor = Color.Red
                Else
                    .lblStockFigure.ForeColor = .lblStatusDisplay.ForeColor
                End If
                .lblStockFigure.Text = objAFFCurrentProductInfo.TSF
                'nan Header Text taken from helper class function
                .lblStockFigureHeader.Text = objAppContainer.objHelper.GetStockFileHeader()
#If RF Then
                '.lblStockFigureHeader.Text = "Stock Figure:"
                'Fix for displaying OSSR
                'If objAppContainer.OSSRStoreFlag = "Y" Then
                '    .btn_OSSRItem.Visible = True
                '    .lblOSSR.Visible = True
                'Else
                '    .btn_OSSRItem.Visible = False
                '    .lblOSSR.Visible = False
                'End If
                If objAppContainer.OSSRStoreFlag = "Y" Then
                    If objAFFCurrentProductInfo.OSSRFlag = "O" Then
                        .lblOSSR.Text = "OSSR"
                        'm_OSSRCount += 1
                        OOSRStatus(strProductCode) = "Y"
                    Else
                        OOSRStatus(strProductCode) = "N"
                        .lblOSSR.Text = " "
                    End If
                Else
                    .btn_OSSRItem.Visible = False
                    .lblOSSR.Visible = False
                End If

#ElseIf NRF Then
                '.lblStockFigureHeader.Text = "Start of Day Stock Figure:"
                .btn_OSSRItem.Visible = False
                .lblOSSR.Visible = False
#End If
                'Fix for allignment for Back and OSSR button
#If RF Then
                If objAppContainer.OSSRStoreFlag = "Y" Then
                    .custCtrlBtnBack.Location = New Point(59 * objAppContainer.iOffSet, 242 * objAppContainer.iOffSet)
                    .btn_OSSRItem.Location = New Point(113 * objAppContainer.iOffSet, 242 * objAppContainer.iOffSet)
                Else
                    .custCtrlBtnBack.Location = New Point(93 * objAppContainer.iOffSet, 242 * objAppContainer.iOffSet)
                    .btn_OSSRItem.Visible = False
                End If
#ElseIf NRF Then
                .custCtrlBtnBack.Location = New Point(93 * objAppContainer.iOffSet, 242 * objAppContainer.iOffSet)
                .btn_OSSRItem.Visible = False
#End If

                If bIsAlreadyPicked Then
                    .lblPicked.Visible = True
                    .lblPicked.Text = "PICKED"
                Else
                    .lblPicked.Visible = False
                End If

                .lblProductDesc1.Text = objDescriptionArray.Item(0)
                .lblProductDesc2.Text = objDescriptionArray.Item(1)
                .lblProductDesc3.Text = objDescriptionArray.Item(2)


                If bIsAlreadyPicked Then
                    .lblQtyReqHeader.Visible = True
                    .lblQtyRequired.Visible = True
                    .lblQtyRequired.Text = objAFFCurrentProductInfo.QuantityRequired.ToString()
                Else
                    .lblQtyReqHeader.Visible = False
                    .lblQtyRequired.Visible = False
                End If
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayAFFPLItemDetails of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayAFFPLItemDetails of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Displays the Picking List FF item details
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayFFPLItemDetails(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Entered DisplayFFPLItemDetails of PLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim objCurrentProductInfoList As ArrayList = New ArrayList()
            Dim objFFCurrentProductInfo As PLProductInfo 'nan FFPLProductInfo

            'Gets the list of objects from the hashtable 

            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If

            With m_FFPLItemDetails

                Dim objDescriptionArray As ArrayList = New ArrayList()
                Dim bIsAlreadyPicked As Boolean = False

                'Checks the type of picking list and sets the values to the controls accordingly
                'The position of the item in the list is indicated by m_iProductListCount variable

                'Gets the value into an object of SMPLProductInfo and sets tghe values
                objFFCurrentProductInfo = New PLProductInfo() 'nan FFPLProductInfo()
                objFFCurrentProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)

                If objFFCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                    bIsAlreadyPicked = True
                End If

               .Text = m_FormHeader
                'nan Header Text taken from helper class function
                .lblStockFigureHeader.Text = objAppContainer.objHelper.GetStockFileHeader()

                'Gets the formatted description
                objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(objFFCurrentProductInfo.Description)

                Dim strProductCode As String = ""
                strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(objFFCurrentProductInfo.ProductCode)

                .lblItemPosition.Text = (m_iProductListCount + 1).ToString() + "/" + objCurrentProductInfoList.Count.ToString()
                .lblBootsCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(objFFCurrentProductInfo.BootsCode)
                .lblProductCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(strProductCode)
                .lblStatusDisplay.Text = objAppContainer.objHelper.GetStatusDescription(objFFCurrentProductInfo.Status)
                If objFFCurrentProductInfo.TSF.Substring(0, 1).Equals("-") Then
                    .lblStockFigure.ForeColor = Color.Red
                Else
                    .lblStockFigure.ForeColor = .lblStatusDisplay.ForeColor
                End If
                .lblStockFigure.Text = objFFCurrentProductInfo.TSF
#If RF Then
                '.lblStockFigureHeader.Text = "Stock Figure:"
                'Fix For Displaying OSSR only when the item is Off site
                'If objAppContainer.OSSRStoreFlag = "Y" Then
                '    .btn_OSSRItem.Visible = True
                '    .lblOSSR.Visible = True
                'Else
                '    .btn_OSSRItem.Visible = False
                '    .lblOSSR.Visible = False
                'End If
                If objAppContainer.OSSRStoreFlag = "Y" Then
                    If objFFCurrentProductInfo.OSSRFlag = "O" Then
                        .lblOSSR.Text = "OSSR"
                        'm_OSSRCount += 1
                        OOSRStatus(strProductCode) = "Y"
                    Else
                        OOSRStatus(strProductCode) = "N"
                        .lblOSSR.Text = " "
                    End If
                Else
                    .btn_OSSRItem.Visible = False
                    .lblOSSR.Visible = False
                End If
#ElseIf NRF Then
                '.lblStockFigureHeader.Text = "Start of Day Stock Figure:"
                .btn_OSSRItem.Visible = False
                .lblOSSR.Visible = False
#End If

                'Fix for allignment for Back and OSSR button
#If RF Then
                If objAppContainer.OSSRStoreFlag = "Y" Then
                    .custCtrlBtnBack.Location = New Point(59 * objAppContainer.iOffSet, 237 * objAppContainer.iOffSet)
                    .btn_OSSRItem.Location = New Point(113 * objAppContainer.iOffSet, 237 * objAppContainer.iOffSet)
                Else
                    .custCtrlBtnBack.Location = New Point(93 * objAppContainer.iOffSet, 237 * objAppContainer.iOffSet)
                    .btn_OSSRItem.Visible = False
                End If
#ElseIf NRF Then
                .custCtrlBtnBack.Location = New Point(93 * objAppContainer.iOffSet, 237 * objAppContainer.iOffSet)
                .btn_OSSRItem.Visible = False
#End If
                'AFF PL CR
#If NRF Then
                .btnView.Visible = True
#ElseIf RF Then
                .btnView.Visible = False
#End If

                If bIsAlreadyPicked Then
                    .lblPicked.Visible = True
                    .lblPicked.Text = "PICKED"
                Else
                    .lblPicked.Visible = False
                End If

                .lblProductDesc1.Text = objDescriptionArray.Item(0)
                .lblProductDesc2.Text = objDescriptionArray.Item(1)
                .lblProductDesc3.Text = objDescriptionArray.Item(2)
#If RF Then
                'Fix for displaying Sales Floor Qty
                .lblSalesQty.Text = objFFCurrentProductInfo.SalesFloorQuantity.ToString()
#ElseIf NRF Then
                .lblSalesFloor.Visible = False
                .lblSalesQty.Visible = False
#End If
                If bIsAlreadyPicked Then
                    .lblQtyReqHeader.Visible = True
                    .lblQtyRequired.Visible = True
                    .lblQtyRequired.Text = objFFCurrentProductInfo.QuantityRequired.ToString()
                Else
                    .lblQtyReqHeader.Visible = False
                    .lblQtyRequired.Visible = False
                End If

                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayFFPLItemDetails of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayFFPLItemDetails of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
#If RF Then
    ''' <summary>
    ''' Displays the Picking List OSSR item details
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayOSSRPLItemDetails(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Entered DisplayFFPLItemDetails of PLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim objCurrentProductInfoList As ArrayList = New ArrayList()
            Dim objOSSRCurrentProductInfo As PLProductInfo

            'Gets the list of objects from the hashtable 

            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If

            With m_OSSRPLItemDetails
                'System Testing SFA Added for OSSR PL
                .lblQtyReqHeader.Visible = False
                .lblQtyRequired.Visible = False
                .lblOf.Visible = False
                .lblTotQtyRequired.Visible = False

                Dim objDescriptionArray As ArrayList = New ArrayList()
                Dim bIsAlreadyPicked As Boolean = False
                'Checks the type of picking list and sets the values to the controls accordingly
                'The position of the item in the list is indicated by m_iProductListCount variable
                'Gets the value into an object of SMPLProductInfo and sets tghe values
                objOSSRCurrentProductInfo = New PLProductInfo() 'Changed to PLProductinfo
                objOSSRCurrentProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)
                If objOSSRCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                    bIsAlreadyPicked = True
                End If
                'Gets the formatted description
                objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(objOSSRCurrentProductInfo.Description)
                Dim strProductCode As String = ""
                strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(objOSSRCurrentProductInfo.ProductCode)
                .lblItemPosition.Text = (m_iProductListCount + 1).ToString() + "/" + objCurrentProductInfoList.Count.ToString()
                .lblBootsCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(objOSSRCurrentProductInfo.BootsCode)
                .lblProductCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(strProductCode)
                .lblStatusDisplay.Text = objAppContainer.objHelper.GetStatusDescription(objOSSRCurrentProductInfo.Status)
                If objOSSRCurrentProductInfo.TSF.Substring(0, 1).Equals("-") Then
                    .lblStockFigure.ForeColor = Color.Red
                Else
                    .lblStockFigure.ForeColor = .lblStatusDisplay.ForeColor
                End If
                .lblStockFigure.Text = objOSSRCurrentProductInfo.TSF

                .lblProductDesc1.Text = objDescriptionArray.Item(0)
                .lblProductDesc2.Text = objDescriptionArray.Item(1)
                .lblProductDesc3.Text = objDescriptionArray.Item(2)

                'Hide the cal pad button not to allow the users to
                're-enter item count without scanning the item again or 
                'selecting next or back button. fix for 5059
                If bHideCalcPad Then
                    .btnBackShopCalcpad.Visible = False
                Else
                    .btnBackShopCalcpad.Visible = True
                End If

                'nan Header Text taken from helper class function
                '.lblStockFigureHeader.Text = objAppContainer.objHelper.GetStockFileHeader()

                'Set the OSSR label
                If objAppContainer.OSSRStoreFlag = "Y" Then
                    If objOSSRCurrentProductInfo.OSSRFlag = "O" Then
                        .lblOSSR.Text = "OSSR"
                        OOSRStatus(strProductCode) = "Y"
                    Else
                        OOSRStatus(strProductCode) = "N"
                        .lblOSSR.Text = " "
                    End If
                Else
                    'System Testing Align back button if not ossr item
                    .custCtrlBtnBack.Location = New Point(93 * objAppContainer.iOffSet, 247 * objAppContainer.iOffSet)
                    .btn_OSSRItem.Visible = False
                    .lblOSSR.Visible = False
                End If
                'Set the OSSR quantity
                If objOSSRCurrentProductInfo.OSSRQuantity < 0 Then
                    .lblOSSRQty.Text = "0"
                Else
                    .lblOSSRQty.Text = objOSSRCurrentProductInfo.OSSRQuantity.ToString()
                End If
                'Set total quantity
                Dim iTotalCount As Integer = 0
                Dim iBSQty As Integer = 0
                Dim iSFQty As Integer = 0
                Dim iOSSRQty As Integer = 0

                If objOSSRCurrentProductInfo.SalesFloorQuantity < 0 Then
                    iSFQty = 0
                Else
                    iSFQty = objOSSRCurrentProductInfo.SalesFloorQuantity
                End If
                If objOSSRCurrentProductInfo.BackShopQuantity < 0 Then
                    iBSQty = 0
                Else
                    iBSQty = objOSSRCurrentProductInfo.BackShopQuantity
                End If
                If objOSSRCurrentProductInfo.OSSRQuantity < 0 Then
                    iOSSRQty = 0
                Else
                    iOSSRQty = objOSSRCurrentProductInfo.OSSRQuantity
                End If
                iTotalCount = iBSQty + iSFQty + iOSSRQty

                .lblTotalItemCount.Text = iTotalCount


                'Based on whether the item is already picked or not display the quantity required field.
                If m_CurrentPickingList.ListType.Equals(Macros.OSSR_PL) Then
                    'Check if the OOSR list is created from FF or AFF
                    If m_OSSRGapFlag.Equals(Macros.PLC_FF_FLAG) Then
                        .lblQtyReqHeader.Visible = True
                        .lblQtyReqHeader.Text = "Quantity Required:"
                        .lblQtyRequired.Visible = True
                        .lblQtyRequired.Text = objOSSRCurrentProductInfo.QuantityRequired.ToString()
                        .lblTotalItemCount.Visible = False
                        .lblTotalItemCountHeader.Visible = False
                        .lblOSSRQty.Visible = False
                        .lblOffSiteHeader.Visible = False
                        .btnBackShopCalcpad.Visible = False
                        .lblQtyReqHeader.Location = New Point(12 * objAppContainer.iOffSet, 158 * objAppContainer.iOffSet)
                        .lblQtyRequired.Location = New Point(159 * objAppContainer.iOffSet, 158 * objAppContainer.iOffSet)
                    Else
                        ''''''''''''
                        'System Testing SFA.
                        Dim multisiteList As ArrayList = New ArrayList()
                        Dim iNumLocations As Integer = 0
                        Dim bIsAllLocCounted As Boolean = True
                        If m_IsMultisite Then
                            For Each objOSSRMultisiteData As EXMultiSiteInfo In objOSSRCurrentProductInfo.MultiSiteList

                                If objOSSRMultisiteData.m_strListId.Equals(m_CurrentPickingList.ListID) AndAlso _
                                    objOSSRMultisiteData.m_strProductCode.Equals(objOSSRCurrentProductInfo.ProductCode) Then

                                    If objOSSRMultisiteData.m_SeqNum = m_SelectedSite + 1 Then
                                        'nan Sales floor quantity will be following if multisite
                                        .lblOSSRQty.Text = objOSSRMultisiteData.m_iBackShopQty
                                        'nan If the site is the current selected site and if it has been counted.
                                        If (objOSSRMultisiteData.m_bIsCounted = True) Then
                                            objOSSRMultisiteData.m_CountPicked = SMQuantityToPick(objOSSRCurrentProductInfo, objOSSRMultisiteData.m_SeqNum)
                                            .lblQtyReqHeader.Visible = True
                                            .lblQtyReqHeader.Text = "Qty to take to Sales Floor"
                                            .lblQtyReqHeader.Location = New Point(2 * objAppContainer.iOffSet, 220 * objAppContainer.iOffSet)
                                            .lblOffSiteHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                                            .lblOSSRQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                                            .lblQtyRequired.Visible = True
                                            .lblOf.Visible = True
                                            .lblTotQtyRequired.Visible = True
                                            .lblTotQtyRequired.Location = New Point(209 * objAppContainer.iOffSet, 220 * objAppContainer.iOffSet)
                                            .lblQtyRequired.Text = objOSSRMultisiteData.m_CountPicked
                                            .lblTotQtyRequired.Text = objOSSRCurrentProductInfo.QuantityRequired

                                            If objOSSRMultisiteData.m_CountPicked = 0 And _
                                            objOSSRMultisiteData.m_iBackShopQty <> 0 Then
                                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M108"), "Info", _
                                                MessageBoxButtons.OK, _
                                                MessageBoxIcon.Asterisk, _
                                                MessageBoxDefaultButton.Button1)
                                            End If
                                        Else
                                            .lblOffSiteHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                                            .lblOSSRQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                                            .lblQtyReqHeader.Visible = False
                                            .lblQtyRequired.Visible = False
                                            .lblOf.Visible = False
                                            .lblTotQtyRequired.Visible = False
                                        End If
                                    End If

                                    If (objOSSRMultisiteData.m_bIsCounted = True) Then
                                        multisiteList.Add("*" & objOSSRMultisiteData.m_strPlannerDesc.ToString() & " (Counted)")
                                    Else
                                        multisiteList.Add(objOSSRMultisiteData.m_strPlannerDesc.ToString())
                                    End If

                                    iNumLocations += 1
                                    If bIsAllLocCounted Then
                                        If Not objOSSRMultisiteData.m_bIsCounted Then
                                            bIsAllLocCounted = False
                                        End If
                                    End If

                                End If
                            Next
                        Else  'System Testing SFA - Fix to display quantity required for single site item
                            If bIsAlreadyPicked Then
                                .lblQtyReqHeader.Visible = True
                                .lblQtyReqHeader.Text = "Qty to take to Sales Floor"
                                .lblQtyReqHeader.Location = New Point(2 * objAppContainer.iOffSet, 220 * objAppContainer.iOffSet)
                                .lblQtyRequired.Visible = True
                                .lblOf.Visible = True
                                .lblTotQtyRequired.Visible = True
                                .lblTotQtyRequired.Location = New Point(209 * objAppContainer.iOffSet, 220 * objAppContainer.iOffSet)
                                .lblTotQtyRequired.Text = objOSSRCurrentProductInfo.QuantityRequired
                                .lblOffSiteHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                                .lblOSSRQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                                If objOSSRCurrentProductInfo.OSSRQuantity > objOSSRCurrentProductInfo.QuantityRequired Then
                                    .lblQtyRequired.Text = objOSSRCurrentProductInfo.QuantityRequired
                                Else
                                    .lblQtyRequired.Text = objOSSRCurrentProductInfo.OSSRQuantity
                                End If
                            Else
                                .lblOffSiteHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                                .lblOSSRQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                            End If
                        End If

                        'Checking if all locations are handled before setting the item as counted
                        If m_IsMultisite Then
                            If Not (iNumLocations > 1 And bIsAllLocCounted) Then
                                bIsAlreadyPicked = False
                            End If
                        End If

                        If multisiteList.Count > 1 Then
                            .lblLocationHeader.Visible = True
                            .cmbLocation.Visible = True
                            .cmbLocation.Items.Clear()
                            'nan removed select as this wont be required
                            '.cmbLocation.Items.Add("Select")
                            For Each strLocation As String In multisiteList
                                .cmbLocation.Items.Add(strLocation)
                            Next

                            .cmbLocation.SelectedIndex = m_SelectedSite

                        Else
                            'Code to adjust position of gap, next, back and quit button
                            .lblLocationHeader.Visible = False
                            .cmbLocation.Visible = False
                        End If

                        '''''''''''''
                        'Set the item picked status
                        'If bIsAlreadyPicked Then
                        '    .lblQtyReqHeader.Visible = True
                        '    .lblQtyRequired.Visible = True
                        '    .lblQtyRequired.Text = objOSSRCurrentProductInfo.QuantityRequired.ToString()
                        'Else
                        '    .lblQtyReqHeader.Visible = False
                        '    .lblQtyRequired.Visible = False
                        'End If

                    End If
                ElseIf m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_OSSR) Then
                    'System Testing SFA
                    .lblQtyReqHeader.Text = "Quantity Required:"
                    .lblQtyReqHeader.Location = New Point(4 * objAppContainer.iOffSet, 220 * objAppContainer.iOffSet)
                    .lblQtyReqHeader.Visible = True
                    .lblTotQtyRequired.Text = objOSSRCurrentProductInfo.BackShopQuantity
                    .lblTotQtyRequired.Location = New Point(163 * objAppContainer.iOffSet, 220 * objAppContainer.iOffSet)
                    .lblTotQtyRequired.Visible = True

                    'Set sales floor quantity and back shop quantity
                    '.lblQtyRequired.Text = objOSSRCurrentProductInfo.BackShopQuantity  - Assign it to total qty label instead of qty req label
                End If

                If bIsAlreadyPicked Then
                    .lblPicked.Visible = True
                    .lblPicked.Text = "PICKED"
                Else
                    .lblPicked.Visible = False
                End If

                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayFFPLItemDetails of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayFFPLItemDetails of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
#End If
    ''' <summary>
    ''' Displays the Picking List EX item details
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayEXPLItemDetails(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Entered DisplayEXPLItemDetails of PLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim objCurrentProductInfoList As ArrayList = New ArrayList()
            Dim objEXCurrentProductInfo As PLProductInfo 'nan- EXPLProductInfo 

            'Gets the list of objects from the hashtable 

            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If

            With m_EXPLItemDetails

                Dim objDescriptionArray As ArrayList = New ArrayList()
                Dim bIsAlreadyCounted As Boolean = False

                'Checks the type of picking list and sets the values to the controls accordingly
                'The position of the item in the list is indicated by m_iProductListCount variable

                'Gets the value into an object of SMPLProductInfo and sets tghe values
                objEXCurrentProductInfo = New PLProductInfo() 'nan- EXPLProductInfo 
                objEXCurrentProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)

                If objEXCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                    bIsAlreadyCounted = True
                End If
                Dim strProductCode As String = ""
                strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(objEXCurrentProductInfo.ProductCode)

                .Text = m_FormHeader
                'nan Header Text taken from helper class function
                .lblStockFigureHeader.Text = objAppContainer.objHelper.GetStockFileHeader()
#If RF Then
                'Hide the cal pad button not to allow the users to
                're-enter item count without scanning the item again or 
                'selecting next or back button.
                If bHideCalcPad Then
                    .btnSalesFloorCalcpad.Visible = False
                Else
                    .btnSalesFloorCalcpad.Visible = True
                End If
                '.lblStockFigureHeader.Text = "Stock Figure:"
                If objAppContainer.OSSRStoreFlag = "Y" Then
                    If objEXCurrentProductInfo.OSSRFlag = "O" Then
                        'm_OSSRCount = 1
                        OOSRStatus(strProductCode) = "Y"
                        .lblOSSR.Text = "OSSR"
                    Else
                        OOSRStatus(strProductCode) = "N"
                        .lblOSSR.Text = " "
                    End If
                Else
                    .Btn_OSSRItem1.Visible = False
                    .lblOSSR.Visible = False
                End If
#ElseIf NRF Then
                '.lblStockFigureHeader.Text = "Start of Day Stock Figure:"
                .Btn_OSSRItem1.Visible = False
                .lblOSSR.Visible = False
#End If
                'AFF PL CR
#If NRF Then
                .btnView.Visible = True
#ElseIf RF Then
                .btnView.Visible = False
#End If
                'm_IsMultisite = False
                'nan Sales floor quantity will be objEXCurrentProductInfo.SalesFloorQuantity if not multisite
                If objEXCurrentProductInfo.SalesFloorQuantity = -1 Then
                    .lblCurrentQty.Text = "0"
                Else
                    .lblCurrentQty.Text = objEXCurrentProductInfo.SalesFloorQuantity.ToString()
                End If


                Dim multisiteList As ArrayList = New ArrayList()
                Dim iNumLocations As Integer = 0
                Dim bIsAllLocCounted As Boolean = True


                If (IsMultisiteTrackAdded(objEXCurrentProductInfo.ProductCode, m_CurrentPickingList.ListID, objEXCurrentProductInfo.MultiSiteList)) Then
                    For Each objExMultisiteData As EXMultiSiteInfo In objEXCurrentProductInfo.MultiSiteList

                        If objExMultisiteData.m_strListId.Equals(m_CurrentPickingList.ListID) AndAlso _
                            objExMultisiteData.m_strProductCode.Equals(objEXCurrentProductInfo.ProductCode) Then

                            If objExMultisiteData.m_SeqNum = m_SelectedSite + 1 Then
                                'nan Sales floor quantity will be following if multisite
                                .lblCurrentQty.Text = objExMultisiteData.m_iSalesFloorQty
                            End If


                            If ((objExMultisiteData.m_bIsCounted = True) AndAlso _
                            (objExMultisiteData.m_strPOGDescription <> "Other")) Then
                                multisiteList.Add("*" & objExMultisiteData.m_strPlannerDesc.ToString().Trim() & "-" & objExMultisiteData.m_strPOGDescription.Trim() & " (Counted)")
                            Else
                                multisiteList.Add(objExMultisiteData.m_strPlannerDesc.ToString().Trim() & "-" & objExMultisiteData.m_strPOGDescription.Trim())
                            End If

                            iNumLocations += 1
                            If bIsAllLocCounted Then
                                If Not objExMultisiteData.m_bIsCounted Then
                                    bIsAllLocCounted = False
                                End If
                            End If
                        End If
                    Next
                End If

                Dim iTotal As Integer = 0
                Dim iSFCount As Integer = 0
                Dim iBSCount As Integer = 0
                Dim iOSSRCount As Integer = 0

                If objEXCurrentProductInfo.BackShopQuantity = -1 Then
                    iBSCount = 0
                Else
                    iBSCount = objEXCurrentProductInfo.BackShopQuantity
                End If

                If objEXCurrentProductInfo.SalesFloorQuantity = -1 Then
                    iSFCount = 0
                Else
                    iSFCount = objEXCurrentProductInfo.SalesFloorQuantity
                End If
                If objEXCurrentProductInfo.OSSRQuantity = -1 Then
                    iOSSRCount = 0
                Else
                    iOSSRCount = objEXCurrentProductInfo.OSSRQuantity
                End If


                iTotal = iBSCount + iSFCount + iOSSRCount
                'nan  the total count
                'If objEXCurrentProductInfo.SalesFloorQuantity = -1 Then
                '.lblTotalItemQty.Text = "0"
                'Else
                .lblTotalItemQty.Text = iTotal
                'End If


                'Integration testing
                If iNumLocations > 1 Then
                    m_IsMultisite = True
                End If
                'Checking if all locations are handled before setting the item as counted
                If m_IsMultisite Then
                    If Not (iNumLocations > 1 And bIsAllLocCounted) Then
                        bIsAlreadyCounted = False
                    End If
                End If

                'Dim strProductCode As String = ""
                strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(objEXCurrentProductInfo.ProductCode)

                'Gets the formatted description
                objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(objEXCurrentProductInfo.Description)

                .lblItemPosition.Text = (m_iProductListCount + 1).ToString() + "/" + objCurrentProductInfoList.Count.ToString()
                .lblBootsCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(objEXCurrentProductInfo.BootsCode)
                .lblProductCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(strProductCode)
                .lblStatusDisplay.Text = objAppContainer.objHelper.GetStatusDescription(objEXCurrentProductInfo.Status)
                If objEXCurrentProductInfo.TSF.Substring(0, 1).Equals("-") Then
                    .lblStockFigure.ForeColor = Color.Red
                Else
                    .lblStockFigure.ForeColor = .lblStatusDisplay.ForeColor
                End If
                .lblStockFigure.Text = objEXCurrentProductInfo.TSF

                If bIsAlreadyCounted Then
                    .lblCounted.Visible = True
                    .lblCounted.Text = "COUNTED"
                Else
                    .lblCounted.Visible = False
#If RF Then
                    .Btn_OSSRItem1.Enabled = False
                    .Btn_OSSRItem1.Visible = False
#End If

                End If
                .lblProductDesc1.Text = objDescriptionArray.Item(0)
                .lblProductDesc2.Text = objDescriptionArray.Item(1)
                .lblProductDesc3.Text = objDescriptionArray.Item(2)

                If multisiteList.Count > 1 Then
                    .lblLocationHeader.Visible = True
                    .cmbLocation.Visible = True
                    .cmbLocation.Items.Clear()
                    'nan removed select as this wont be required
                    '.cmbLocation.Items.Add("Select")
                    For Each strLocation As String In multisiteList
                        .cmbLocation.Items.Add(strLocation)
                    Next

                    .cmbLocation.SelectedIndex = m_SelectedSite

                    'Code to adjust the position of gap, next, back and quit button
                    .btnZero.Visible = True
                    .btnZero.Enabled = True
                    .btnZero.Location = New System.Drawing.Point(70 * objAppContainer.iOffSet, 247 * objAppContainer.iOffSet)
                    .custCtrlBtnNext.Location = New System.Drawing.Point(15 * objAppContainer.iOffSet, 247 * objAppContainer.iOffSet)
                    .custCtrlBtnBack.Location = New System.Drawing.Point(126 * objAppContainer.iOffSet, 247 * objAppContainer.iOffSet)
                    .custCtrlBtnQuit.Location = New System.Drawing.Point(182 * objAppContainer.iOffSet, 247 * objAppContainer.iOffSet)
                Else
                    'Code to adjust position of gap, next, back and quit button
                    .btnZero.Enabled = True   'Enabling zero button as part if SFA SIT defect466
                    .btnZero.Visible = True
                    .btnZero.Location = New System.Drawing.Point(70 * objAppContainer.iOffSet, 247 * objAppContainer.iOffSet)
                    .custCtrlBtnNext.Location = New System.Drawing.Point(15 * objAppContainer.iOffSet, 247 * objAppContainer.iOffSet)
                    .custCtrlBtnBack.Location = New System.Drawing.Point(126 * objAppContainer.iOffSet, 247 * objAppContainer.iOffSet)
                    .custCtrlBtnQuit.Location = New System.Drawing.Point(182 * objAppContainer.iOffSet, 247 * objAppContainer.iOffSet)

                    .lblLocationHeader.Visible = False
                    .cmbLocation.Visible = False
                End If
#If RF Then
                'Added for OSSR CR
                If bIsAlreadyCounted Then
                    If objAppContainer.OSSRStoreFlag = "Y" Then
                        .btnZero.Visible = False
                        .btnZero.Enabled = False
                        .Btn_OSSRItem1.Enabled = True
                        .Btn_OSSRItem1.Visible = True
                        .Btn_OSSRItem1.Location = New System.Drawing.Point(1 * objAppContainer.iOffSet, 247 * objAppContainer.iOffSet)
                        .custCtrlBtnNext.Location = New System.Drawing.Point(73 * objAppContainer.iOffSet, 247 * objAppContainer.iOffSet)
                        .custCtrlBtnBack.Location = New System.Drawing.Point(129 * objAppContainer.iOffSet, 247 * objAppContainer.iOffSet)
                        .custCtrlBtnQuit.Location = New System.Drawing.Point(185 * objAppContainer.iOffSet, 247 * objAppContainer.iOffSet)
                    End If
                End If
#End If
                '#If RF Then
                '                Dim objPLCRecord As PLCRecord = New PLCRecord()

                '                'Sets the values
                '                objPLCRecord.strListID = m_CurrentPickingList.ListID
                '                objPLCRecord.strNumberSEQ = objEXCurrentProductInfo.Sequence
                '                objPLCRecord.strBootscode = objEXCurrentProductInfo.BootsCode
                '                objPLCRecord.strStockCount = objEXCurrentProductInfo.SalesFloorQuantity
                '                objPLCRecord.cIsGAPFlag = Macros.PLC_SM_FLAG
                '                'After the new field addition.
                '                objPLCRecord.strPickListLocation = Macros.BACK_SHOP
                '                objPLCRecord.strOSSRCount = Macros.OSSR_COUNT
                '                objPLCRecord.strUpdateOSSRItem = Macros.UPDATE_OSSR_ITEM
                '                objPLCRecord.strLocationCounted = "  "  'To be updated
                '                objPLCRecord.strAllMSPicked = " "       'To be updated

                '                objAppContainer.objExportDataManager.CreatePLC(objPLCRecord)
                '#End If
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                'nan this used to be processlocationchange()
                ' as not location neednt be selected calling this function here
                ProcessLocation()

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayEXPLItemDetails of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayEXPLItemDetails of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Displays the Picking List Multisite discrepancy screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayPLDiscrepancy(ByVal o As Object, ByVal e As EventArgs)

        Dim b_IsDiscrepancy As Boolean = False
        Dim b_BackShop As Boolean = False
        Try

            With m_PLDiscrepancy
                .lstView.Clear()
                .Help1.Visible = False
                .lblHeading.Text = "Multisite Discrepancy"

                .lstView.Columns.Add("Item", 58 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                .lstView.Columns.Add("Item Description", 175 * objAppContainer.iOffSet, HorizontalAlignment.Left)

                If m_CurrentPickingList.ListType.Equals(Macros.OSSR_PL) Or _
                   m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_OSSR) Or _
                   m_CurrentPickingList.ListType = Macros.SHELF_MONITOR_PL Or _
                   m_CurrentPickingList.ListType = Macros.EXCESS_STOCK_PL Then

                    .Text = "Picking List - BS"
                    .lblHeading.Text = "Main Back Shop count must be completed for the following items"
                    'b_BackShop = True
                    '.lstView.Columns.Add("Item", 46, HorizontalAlignment.Center)
                    '.lstView.Columns.Add("Description", 122, HorizontalAlignment.Left)
                    '.lstView.Columns.Add("MBS", 32, HorizontalAlignment.Center)
                    ''nan including MBS and PSP columns
                    '.lstView.Columns.Add("PSP", 31, HorizontalAlignment.Center)
                ElseIf m_CurrentPickingList.ListType = Macros.EXCESS_STOCK_PL_OSSR Or _
                        m_CurrentPickingList.ListType = Macros.EXCESS_STOCK_PL_SF Then

                    .Text = "Picking List - SF"
                    .lblHeading.Text = "All Multisite count must be completed for the following items"
                    'b_BackShop = False
                    '.lstView.Columns.Add("Item", 70 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    '.lstView.Columns.Add("Description", 150 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                End If
                'Loop through all the items
                For Each objProductInfo As PLProductInfo In m_DiscrepancyList

                    .lstView.Items.Add( _
                   (New ListViewItem(New String() {objProductInfo.BootsCode, _
                                                   objProductInfo.ShortDescription})))

                Next
                .Help1.Visible = True
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                .Visible = True
                .Refresh()
            End With

            BCReader.GetInstance.StopRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayPLDiscrepancy of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try

        objAppContainer.objLogger.WriteAppLog("Exit PLSessionMgr DisplayPLDiscrepancy", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Displays the Picking List PSP Pending screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayPSPPending(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Entered DisplayPSPPending of PLSessionMgr", Logger.LogLevel.INFO)
        Try
            With m_PLPSPPending
                .Visible = True
                .Refresh()
            End With
            BCReader.GetInstance.StopRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayPLDiscrepancy of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try

        objAppContainer.objLogger.WriteAppLog("Exit PLSessionMgr DisplayPSPPending", Logger.LogLevel.RELEASE)
    End Sub

    ''' <summary>
    ''' Displays the Picking List Finish Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayPLFinish(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Entered DisplayPLFinish of PLSessionMgr", Logger.LogLevel.INFO)
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim iNumItemPicked As Integer = 0

        'System Testing
        Dim iTotalCount As Integer = 0
        Try
            With m_PLFinish

                'Dim strDataTable As Hashtable = New Hashtable()
                'For Each objCountedData As PLProductPickedData In m_PickedDataList
                '    Dim bIsUnique As Boolean = True
                '    If strDataTable.ContainsKey(objCountedData.m_strListId) Then
                '        bIsUnique = False
                '    End If
                '    If bIsUnique Then
                '        strDataTable.Add(objCountedData.m_strListId, objCountedData.m_strListType)
                '    End If
                'Next
                'For Each objCountedData As EXMultiSiteInfo In objAppContainer.objEXMultiSiteList
                '    Dim bIsUnique As Boolean = True
                '    If strDataTable.ContainsKey(objCountedData.m_strListId) Then
                '        bIsUnique = False
                '    End If
                '    If bIsUnique Then
                '        strDataTable.Add(objCountedData.m_strListId, Macros.EXCESS_STOCK_PL)
                '    End If
                'Next

                For Each objPickingList As PickingList In objAppContainer.objGlobalPickingList
                    If objPickingList.ListID = m_CurrentPickingList.ListID Then
                        Dim strListType As String = objPickingList.ListType
                        If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                            objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
                        End If
                        If strListType.Equals(Macros.SHELF_MONITOR_PL) Or _
                            strListType.Equals(Macros.EXCESS_STOCK_PL) Then

                            'nan For Each objProductData As SMPLProductInfo In objCurrentProductInfoList
                            For Each objProductData As PLProductInfo In objCurrentProductInfoList
                                If objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                    iNumItemPicked += 1
                                End If
                            Next
                        ElseIf (strListType.Equals(Macros.FAST_FILL_PL)) Then
                            'nan For Each objProductData As FFPLProductInfo In objCurrentProductInfoList
                            For Each objProductData As PLProductInfo In objCurrentProductInfoList
                                If objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                    iNumItemPicked += 1
                                End If
                            Next
                        ElseIf (strListType.Equals(Macros.AUTO_FAST_FILL_PL)) Then
                            'nan For Each objProductData As AFFPLProductInfo In objCurrentProductInfoList
                            For Each objProductData As PLProductInfo In objCurrentProductInfoList
                                If objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                    iNumItemPicked += 1
                                End If
                            Next
                        ElseIf strListType.Equals(Macros.EXCESS_STOCK_PL_SF) Or _
                                strListType.Equals(Macros.EXCESS_STOCK_PL_OSSR) Then

                            'nan For Each objProductData As EXPLProductInfo In objCurrentProductInfoList
                            For Each objProductData As PLProductInfo In objCurrentProductInfoList
                                If objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                    iNumItemPicked += 1
                                End If
                            Next
                        ElseIf strListType.Equals(Macros.OSSR_PL) Or _
                                strListType.Equals(Macros.EXCESS_STOCK_OSSR) Then
                            For Each objProductData As PLProductInfo In objCurrentProductInfoList 'System testing Changed to PLProductInfo
                                If objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                    iNumItemPicked += 1
                                End If
                            Next
                        End If
                    End If
                Next
                'System Testing
                'For Each objPickingList As PickingList In objAppContainer.objGlobalPickingList
                iTotalCount = m_CurrentPickingList.TotalItems
                ' Next

                Dim iUncheckedItems As Integer = 0
                If iNumItemPicked = iTotalCount Then
                    .lblPLStatDisplay.Text = "Picking List Complete."
                    .lblItemsChecked.Visible = False
                    .btnView.Visible = False
                    .lblSelectView.Visible = False
                ElseIf iNumItemPicked < iTotalCount Then
                    'nan form modified
                    .lblPLStatDisplay.Text = "Picking List Incomplete."
                    '.lblItemsChecked.Visible = True
                    iUncheckedItems = iTotalCount - iNumItemPicked
                    .btnView.Visible = True
                    .lblSelectView.Visible = True
                    '.lblItemsChecked.Text = iUncheckedItems.ToString() + " items not checked"
                End If
                'System Testing


                'Fix for Gap in FF and EX Picking List
                If m_CurrentPickingList.ListType.Equals(Macros.SHELF_MONITOR_PL) Or _
                   m_CurrentPickingList.ListType.Equals(Macros.FAST_FILL_PL) Or _
                   m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL) Or _
                   m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_SF) Or _
                   m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_OSSR) Or _
                   m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_OSSR) Then
                    'IT Internal: If count is entered for an item for which GAP is inserted already
                    'If bIsGap Then
                    Dim iTemp As Integer = 0
                    iTemp = GetNumGapItems()
                    '.lblGapData.Text = m_iNumGapItems.ToString() + " Gap Items sent to report"
                    .lblGapData.Text = iTemp.ToString() + " Gap items sent to report"
                    .lblGapData.Visible = True
                    'Else
                    '    .lblGapData.Visible = False
                    'End If
                End If

                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayPLFinish of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayPLFinish of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    Private Sub DisplayPLOSSRFinish(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Entered DisplayPLOSSRFinish of PLSessionMgr", Logger.LogLevel.INFO)
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim iNumItemPicked As Integer = 0
        Try
            'System Testing
            Dim iTotalCount As Integer = 0
            'm_OSSRCount = 0
            'For Each objPickingList As PickingList In objAppContainer.objGlobalPickingList
            'If objPickingList.ListID = m_CurrentPickingList.ListID Then
            Dim strListType As String = m_CurrentPickingList.ListType
            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If
            If strListType.Equals(Macros.SHELF_MONITOR_PL) Then
                'For Each objProductData As SMPLProductInfo In objCurrentProductInfoList
                '    If objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                '        iNumItemPicked += 1
                '    End If
                '    If OOSRStatus(objAppContainer.objHelper.GeneratePCwithCDV(objProductData.ProductCode)) = "Y" Then
                '        m_OSSRCount += 1
                '    End If
                'Next

                'nan Dim lstPicked = From objTemp As SMPLProductInfo In objCurrentProductInfoList Where _
                Dim lstPicked = From objTemp As PLProductInfo In objCurrentProductInfoList Where _
                                objTemp.ListItemStatus.Equals(Macros.STATUS_PICKED)
                iNumItemPicked = lstPicked.Count
            ElseIf (strListType.Equals(Macros.FAST_FILL_PL)) Then
                'For Each objProductData As FFPLProductInfo In objCurrentProductInfoList
                '    If objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                '        iNumItemPicked += 1
                '    End If
                '    If OOSRStatus(objAppContainer.objHelper.GeneratePCwithCDV(objProductData.ProductCode)) = "Y" Then
                '        m_OSSRCount += 1
                '    End If
                'Next
                'nan Dim lstPicked = From objTemp As FFPLProductInfo In objCurrentProductInfoList Where _
                Dim lstPicked = From objTemp As PLProductInfo In objCurrentProductInfoList Where _
                                objTemp.ListItemStatus.Equals(Macros.STATUS_PICKED)
                iNumItemPicked = lstPicked.Count
            ElseIf (strListType.Equals(Macros.AUTO_FAST_FILL_PL)) Then
                'For Each objProductData As AFFPLProductInfo In objCurrentProductInfoList
                '    If objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                '        iNumItemPicked += 1
                '    End If
                '    If OOSRStatus(objAppContainer.objHelper.GeneratePCwithCDV(objProductData.ProductCode)) = "Y" Then
                '        m_OSSRCount += 1
                '    End If
                'Next

                'nan Dim lstPicked = From objTemp As AFFPLProductInfo In objCurrentProductInfoList Where _
                Dim lstPicked = From objTemp As PLProductInfo In objCurrentProductInfoList Where _
                                objTemp.ListItemStatus.Equals(Macros.STATUS_PICKED)
                iNumItemPicked = lstPicked.Count
            End If
            'End If
            'Next
            iTotalCount = m_CurrentPickingList.TotalItems
            With m_PLOSSRFinish
                If iTotalCount = iNumItemPicked Then
                    .lblPLStatDisplay.Text = "Back shop picking list is now complete"
                Else
                    .lblPLStatDisplay.Text = "Back shop picking list is incomplete"
                End If
                .lblOSSRItems.Text = m_OSSRCount.ToString() + " OSSR Item(s)in List"
                Dim iTemp As Integer = 0
                iTemp = GetNumGapItems()
                .lblGAPItems.Text = iTemp.ToString() + " Gap Items sent to report"
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayPLFinish of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayPLFinish of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Displays the Picking List Summary Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayPLSummary(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Entered DisplayPLSummary of PLSessionMgr", Logger.LogLevel.INFO)
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim iItemCount As Integer = 0
        Dim iNumItemChecked As Integer = 0
        Dim bIsItemActioned As Boolean = False
        'to chk whether the actioned lists are completed or not
        Dim bIsListComplete As Boolean = True
        Dim objProductInfoList As ArrayList = New ArrayList()
        Try

            For Each objPickingList As PickingList In objAppContainer.objGlobalPickingList
                iItemCount = iItemCount + objPickingList.TotalItems
                If objAppContainer.objGlobalPLMappingTable.ContainsKey(objPickingList.ListID) Then
                    objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(objPickingList.ListID)

                    'If objPickingList.ListType.Equals(Macros.SHELF_MONITOR_PL) Then
                    'nan For Each objProductData As SMPLProductInfo In objCurrentProductInfoList
                    For Each objProductData As PLProductInfo In objCurrentProductInfoList
                        If objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                            'iNumItemChecked += 1
                            bIsItemActioned = True
                            Exit For
                        End If
                    Next
                    'ElseIf (objPickingList.ListType.Equals(Macros.FAST_FILL_PL)) Then
                    '    'nan For Each objProductData As FFPLProductInfo In objCurrentProductInfoList
                    '    For Each objProductData As PLProductInfo In objCurrentProductInfoList
                    '        If objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                    '            'iNumItemChecked += 1
                    '            bIsItemActioned = True
                    '            Exit For
                    '        End If
                    '    Next
                    'ElseIf (objPickingList.ListType.Equals(Macros.AUTO_FAST_FILL_PL)) Then
                    '    'nan For Each objProductData As AFFPLProductInfo In objCurrentProductInfoList
                    '    For Each objProductData As PLProductInfo In objCurrentProductInfoList
                    '        If objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                    '            bIsItemActioned = True
                    '            Exit For
                    '        End If
                    '    Next
                    'ElseIf objPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL) Or _
                    '    objPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_SF) Then
                    '    'nan For Each objProductData As EXPLProductInfo In objCurrentProductInfoList
                    '    For Each objProductData As PLProductInfo In objCurrentProductInfoList
                    '        If objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                    '            'iNumItemChecked += 1
                    '            bIsItemActioned = True
                    '            Exit For
                    '        End If
                    '    Next
                    'End If
                End If
            Next
            'If we  have aany itm actioned , only then display thye summary screen
#If RF Then
            m_PLSummary.lblMessage.Visible = False
#End If
            If bIsItemActioned Then
                For Each objPickingList As PickingList In objAppContainer.objGlobalPickingList
                    If objAppContainer.objGlobalPLMappingTable.ContainsKey(objPickingList.ListID) Then
                        objProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(objPickingList.ListID)

                        If objPickingList.ListType.Equals(Macros.OSSR_PL) Or _
                                                        objPickingList.ListType.Equals(Macros.EXCESS_STOCK_OSSR) Then
                            For Each objProductData As PLProductInfo In objProductInfoList 'OSSRPLProductInfo changed to  PLProductInfo
                                If Not objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                    bIsListComplete = False
                                    Exit For
                                End If
                            Next

                        Else

                            For Each objProductData As PLProductInfo In objProductInfoList
                                If Not objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                    bIsListComplete = False
                                    Exit For
                                End If
                            Next

                        End If
                    End If
                Next
                m_PLSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                If bIsListComplete Then
                    m_PLSummary.lblPLStatDisplay.Text = "Picking List Complete."
                Else
                    m_PLSummary.lblPLStatDisplay.Text = "Picking List Incomplete."
                End If
                m_PLSummary.Visible = True
                m_PLSummary.Refresh()
            Else
                'If there are no items actioned quit the picking list module.
                PLSessionMgr.GetInstance().EndSession()
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayPLSummary of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayPLSummary of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Screen Display method for Count List. 
    ''' All Count List sub screens will be displayed using this method. 
    ''' </summary>
    ''' <param name="ScreenName"></param>
    ''' <remarks></remarks>
    Public Sub DisplayPLScreen(ByVal ScreenName As PLSCREENS, Optional ByVal iCurScreen As Integer = 0)
        objAppContainer.objLogger.WriteAppLog("Entered DisplayPLScreen of PLSessionMgr", Logger.LogLevel.INFO)
        Try
            Select Case ScreenName
                Case PLSCREENS.Home
                    'Invoke method to display the home screen
                    m_PLHome.Invoke(New EventHandler(AddressOf DisplayPLHome))
                Case PLSCREENS.ItemConfirm
                    'Invoke method to display the item confirmation screen
                    m_PLItemConfirm.Invoke(New EventHandler(AddressOf DisplayPLItemConfirm))
#If RF Then
                Case PLSCREENS.OSSRItemDetails
                    'Invoke method to display the SM item details screens
                    m_SMPLItemDetails.Invoke(New EventHandler(AddressOf DisplayOSSRPLItemDetails))
#End If
                Case PLSCREENS.SMItemDetails
                    'Invoke method to display the SM item details screens
                    m_SMPLItemDetails.Invoke(New EventHandler(AddressOf DisplaySMPLItemDetails))
                Case PLSCREENS.FFItemDetails
                    'Invoke method to display the FF item details screens
                    m_FFPLItemDetails.Invoke(New EventHandler(AddressOf DisplayFFPLItemDetails))
                Case PLSCREENS.AFFItemDetails
                    'Invoke method to display the AFF item  details Screen.
                    m_AFFPL.Invoke(New EventHandler(AddressOf DisplayAFFPLItemDetails))
                Case PLSCREENS.EXItemDetails
                    'Invoke method to display the EX item details screens
                    m_EXPLItemDetails.Invoke(New EventHandler(AddressOf DisplayEXPLItemDetails))
                Case PLSCREENS.Finish
                    'Invoke method to display the finish screen
                    m_PLFinish.Invoke(New EventHandler(AddressOf DisplayPLFinish))
                Case PLSCREENS.Summary
                    'Invoke method to display the summary screen
                    m_PLSummary.Invoke(New EventHandler(AddressOf DisplayPLSummary))
                Case PLSCREENS.OSSRFinish
                    'Invoke method to display OSSR Final
                    m_PLOSSRFinish.Invoke(New EventHandler(AddressOf DisplayPLOSSRFinish))
                Case PLSCREENS.ItemView
                    m_iCurScreen = iCurScreen
                    m_PLView.Invoke(New EventHandler(AddressOf DisplayPLItemView))
                    'AFF PL CR
                Case PLSCREENS.AFFMessage
                    m_Message.Invoke(New EventHandler(AddressOf DisplayAFFMessage))
                    'nan Call discrepancy screen
                Case PLSCREENS.Discrepancy
                    m_Message.Invoke(New EventHandler(AddressOf DisplayPLDiscrepancy))
                Case PLSCREENS.PSPPending
                    m_Message.Invoke(New EventHandler(AddressOf DisplayPSPPending))

#If RF Then
                Case PLSCREENS.MoveEXPL
                    m_PLOSSRFinish.Invoke(New EventHandler(AddressOf DisplayMoveEXPL))
#End If

            End Select
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayPLScreen of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayPLScreen of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    'Auto Fast Fill PL CR
    ''' <summary>
    ''' Display Item View List screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayPLItemView(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Enter PLSessionMgr DisplayPLItemView", Logger.LogLevel.RELEASE)
        With m_PLView
            .lstView.Clear()
            .Help1.Location = New Point(400 / 2 * objAppContainer.iOffSet, 15 / 2 * objAppContainer.iOffSet)
            .lblHeading.Location = New Point(3 / 2 * objAppContainer.iOffSet, 20 / 2 * objAppContainer.iOffSet)
            .lblHeading.Text = "View Picking List"   'Changed Pick List to Picking List
            .lblHeading.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
            'nan Changed column sizes
            .Text = m_FormHeader
            .lblBottomText.Visible = True
            .lblBottomText.Text = "Select the item you want to pick or Quit to return to Picking List"  'Changed Pick List to Picking List
            Dim objCurrentProductInfoList As ArrayList = New ArrayList()
            'For Each objPickingList As PickingList In objAppContainer.objGlobalPickingList
            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
                'Next
                If m_CurrentPickingList.ListType = Macros.AUTO_FAST_FILL_PL Then
                    'SFA SIT DEF #568
                    .Help1.Visible = False
                    .lstView.Columns.Add("Item", 70 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    .lstView.Columns.Add("Description", 150 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    .lstView.Columns.Add("PG", 40 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    .lstView.Columns.Add("BC", 40 * objAppContainer.iOffSet, HorizontalAlignment.Center)

                    'nan For Each objItemInfo As AFFPLProductInfo In objCurrentProductInfoList
                    For Each objItemInfo As PLProductInfo In objCurrentProductInfoList
                        .lstView.Items.Add( _
                            (New ListViewItem(New String() {objItemInfo.BootsCode, _
                                                            objItemInfo.ShortDescription, _
                                                            objItemInfo.ProductGrp, _
                                                            objItemInfo.BCType})))
                    Next
                ElseIf m_CurrentPickingList.ListType = Macros.SHELF_MONITOR_PL Or _
                       m_CurrentPickingList.ListType = Macros.EXCESS_STOCK_PL Then
                    'SFA SIT DEF #568
                    .Help1.Visible = True
                    'nan Changed font size to 7
                    .lstView.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Regular)
                    '.lstView.Columns.Add("Item", 70 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    '.lstView.Columns.Add("Description", 150 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    ''nan including MBS and PSP columns
                    '.lstView.Columns.Add("MBS", 40 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    '.lstView.Columns.Add("PSP", 40 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    .lstView.Columns.Add("Item", 46 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    .lstView.Columns.Add("Description", 122 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstView.Columns.Add("MBS", 32 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    'nan including MBS and PSP columns
                    .lstView.Columns.Add("PSP", 31 * objAppContainer.iOffSet, HorizontalAlignment.Center)

                    Dim isMBS As String
                    Dim isPSP As String
                    Dim multiSite As EXMultiSiteInfo
                    'nan Populate Data for MBS and PSP
                    For Each objItemInfo As PLProductInfo In objCurrentProductInfoList

                        If objItemInfo.PendingSalesFlag Then
                            multiSite = objItemInfo.MultiSiteList.Item(0)
                            If multiSite.m_bIsCounted Then
                                isMBS = "Y"
                            Else
                                isMBS = "N"
                            End If

                            multiSite = objItemInfo.MultiSiteList.Item(1)
                            If multiSite.m_bIsCounted Then
                                isPSP = "Y"
                            Else
                                isPSP = "N"
                            End If
                        Else
                            If objItemInfo.IsBSCounted Then
                                isMBS = "Y"
                            Else
                                isMBS = "N"
                            End If
                            isPSP = "N/A"
                        End If

                        .lstView.Items.Add( _
                            (New ListViewItem(New String() {objItemInfo.BootsCode, _
                                                            LCase(objItemInfo.ShortDescription), isMBS, isPSP})))
                    Next

                ElseIf m_CurrentPickingList.ListType = Macros.EXCESS_STOCK_PL_OSSR Or _
                       m_CurrentPickingList.ListType = Macros.EXCESS_STOCK_PL_SF Then
                    .lstView.Columns.Add("Item", 70 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    .lstView.Columns.Add("Description", 150 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    'SFA SIT DEF #568
                    .Help1.Visible = False
                    'nan For Each objItemInfo As EXPLProductInfo In objCurrentProductInfoList
                    For Each objItemInfo As PLProductInfo In objCurrentProductInfoList
                        If m_iCurScreen = Macros.PL_FINISH Then
                            If objItemInfo.ListItemStatus = Macros.STATUS_PICKED Then
                                Continue For
                            Else
                                .lstView.Items.Add( _
                            (New ListViewItem(New String() {objItemInfo.BootsCode, _
                                                            objItemInfo.ShortDescription})))

                            End If
                        Else
                            .lstView.Items.Add( _
                            (New ListViewItem(New String() {objItemInfo.BootsCode, _
                                                            objItemInfo.ShortDescription})))
                        End If
                    Next
                ElseIf m_CurrentPickingList.ListType = Macros.FAST_FILL_PL Then
                    'SFA SIT DEF #568
                    .Help1.Visible = False
                    .lstView.Columns.Add("Item", 70 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    .lstView.Columns.Add("Description", 150 * objAppContainer.iOffSet, HorizontalAlignment.Center)


                    'nan For Each objItemInfo As FFPLProductInfo In objCurrentProductInfoList
                    For Each objItemInfo As PLProductInfo In objCurrentProductInfoList

                        .lstView.Items.Add( _
                            (New ListViewItem(New String() {objItemInfo.BootsCode, _
                                                           objItemInfo.ShortDescription})))
                    Next
                ElseIf m_CurrentPickingList.ListType.Equals(Macros.OSSR_PL) Or _
                       m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_OSSR) Then
                    'SFA SIT DEF #568
                    .Help1.Visible = False
                    .lstView.Columns.Add("Item", 70 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    .lstView.Columns.Add("Description", 150 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    For Each objItemInfo As PLProductInfo In objCurrentProductInfoList 'OSSRPLProductInfo changed to  PLProductInfo
                        If m_iCurScreen = Macros.PL_FINISH Then
                            If objItemInfo.ListItemStatus = Macros.STATUS_PICKED Then
                                Continue For
                            Else
                                .lstView.Items.Add( _
                            (New ListViewItem(New String() {objItemInfo.BootsCode, _
                                                            objItemInfo.ShortDescription})))
                            End If
                        Else
                            .lstView.Items.Add( _
                            (New ListViewItem(New String() {objItemInfo.BootsCode, _
                                                            objItemInfo.ShortDescription})))
                        End If
                    Next
                End If
            End If
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            .Visible = True
            .Refresh()
        End With
        BCReader.GetInstance.StopRead()
        objAppContainer.objLogger.WriteAppLog("Exit PLSessionMgr DisplayPLItemView", Logger.LogLevel.RELEASE)
    End Sub
    Public Function ProcessViewItemSelection(ByVal iSelectedIndex As Integer)
        Dim bTemp As Boolean = False
        Dim strBootsCode As String = Nothing
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim objCurrentProductInfo As PLProductInfo = New PLProductInfo()
        Dim iIndex As Integer = -1
        Try
            With m_PLView
                strBootsCode = .lstView.Items(iSelectedIndex).Text
            End With

            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If

            If Not strBootsCode Is Nothing Then
                For Each objItemInfo As PLProductInfo In objCurrentProductInfoList
                    iIndex = iIndex + 1
                    If objItemInfo.BootsCode.Equals(strBootsCode) Then
                        m_iProductListCount = iIndex
                        Exit For
                    End If
                Next
            End If
            'm_iProductListCount = iSelectedIndex
            'nan Reset the selected site index
            m_SelectedSite = 0
            bFromViewScreen = True
            DisplayPLScreen(PLSCREENS.ItemConfirm)
            bTemp = True
        Catch ex As Exception
            bTemp = False
        End Try
        Return bTemp
    End Function
    Private Sub DisplayAFFMessage(ByVal o As Object, ByVal e As EventArgs)
        With m_Message

            Me.m_Message.Location = New System.Drawing.Point(10, 105)
#If RF Then
       m_Message.lblMsg.Text = "Please wait, Loading List"
#ElseIf NRF Then
            m_Message.lblMsg.Text = "Please wait, Sorting picking List"
#End If

            m_Message.Visible = True
            Application.DoEvents()
            .Invalidate()
            .Refresh()
        End With
    End Sub
    Public Sub SetSelectedEXMSIndex(ByVal iIndex As Integer)
        m_SelectedEXMSIndex = iIndex.ToString.PadLeft(3, "0")
    End Sub
    ''' <summary>
    ''' Processes the location specific data
    '''     ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessLocation()
        objAppContainer.objLogger.WriteAppLog("Entered ProcessLocationChange of PLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim objCurrentProductInfoList As ArrayList = New ArrayList()
            Dim strLocation As String = ""

            With m_EXPLItemDetails
                'For multisited items set hide calc pad to false i.e., display calc pad.
                .btnSalesFloorCalcpad.Visible = True
                'nan removed as no select
                'If .cmbLocation.SelectedItem.ToString() = "Select" Then
                '    .lblSalesFloorQty.Text = 0
                '    Exit Sub
                'Else
                strLocation = .cmbLocation.SelectedItem.ToString()
                'End If

            End With

            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If

            'Gets the value into an object of EXPLProductInfo 
            Dim objEXCurrentProductInfo As PLProductInfo = New PLProductInfo() 'nan- EXPLProductInfo 
            objEXCurrentProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)

            'nan changed the list to new array in PLProductInfo
            For Each objPickedData As EXMultiSiteInfo In objEXCurrentProductInfo.MultiSiteList
                If objPickedData.m_strListId.Equals(m_CurrentPickingList.ListID) AndAlso _
                    objPickedData.m_strProductCode.Equals(objEXCurrentProductInfo.ProductCode) AndAlso _
                    objPickedData.m_SeqNum.Equals(m_SelectedSite + 1) Then
                    'nan removing negetive check
                    'If objPickedData.m_iSalesFloorQty < 0 Then
                    '    m_EXPLItemDetails.lblSalesFloorQty.Text = 0
                    'Else
                    m_EXPLItemDetails.lblCurrentQty.Text = objPickedData.m_iSalesFloorQty
                    'End If

                    Exit For

                End If
            Next
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Price PLSessionMgr - Exception in full processLocationChange", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessLocationChange of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Retrieves the data regarding the product groups from the database
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetPickingListInfo() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered GetPickingListInfo of PLSessionMgr", Logger.LogLevel.INFO)
        Try
            'Checks if the picking list is already available
            If objAppContainer.objGlobalPickingList.Count = 0 Then
                'Calls the GetPickingList method in DataEngine to update the m_PickingList
                If Not objAppContainer.objDataEngine.GetPickingList(objAppContainer.objGlobalPickingList) Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M24"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                    Return False
                Else
                    'nan sort
                    'objAppContainer.objGlobalPickingList.Sort()
                    'objAppContainer.objGlobalPickingList.Reverse()
                    PLSessionMgr.GetInstance().DisplayPLScreen(PLSessionMgr.PLSCREENS.Home)
                    Return True
                End If
            Else
                PLSessionMgr.GetInstance().DisplayPLScreen(PLSessionMgr.PLSCREENS.Home)
                Return True
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in GetPickingListInfo of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit GetPickingListInfo of PLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Retrieves the list of items corresponding to the picking list id from DB
    ''' </summary>
    ''' <param name="strListId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetItemList(ByVal strListId As String) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered GetItemList of PLSessionMgr", Logger.LogLevel.INFO)
        Dim objProductInfoList As ArrayList = Nothing
        'nan 
        Dim strProductCode As String
        Dim tempMultiSiteList As ArrayList
        Dim multiSiteList As ArrayList = New ArrayList()
        'CR for Repeat Count
        Dim RepeatCount As Integer = 0

        Try
            'Sets the currently selected product group to the m_CurrentPickingList variable
            For Each objPickingList As PickingList In objAppContainer.objGlobalPickingList
                If objPickingList.ListID.Equals(strListId) Then
                    'Checks if the picking list is completed. Shows the message if the list is already picked.
                    If objPickingList.ListStatus.Equals(Macros.STATUS_PICKED) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M44"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        Return False
                    ElseIf objPickingList.ListStatus.Equals(Macros.STATUS_LIST_ACTIVE) Then
#If RF Then
                        '@Service Fix
                        If (Not objAppContainer.strCurrentUserID.Equals(m_CurrentPickingList.PickerID)) Then
#End If
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M89"), "Info", _
                                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
                        Return False
#If RF Then
                        End If
#End If
                    End If
                    m_CurrentPickingList.ListID = objPickingList.ListID
                    m_CurrentPickingList.ListType = objPickingList.ListType
                    m_CurrentPickingList.ListTime = objPickingList.ListTime
                    m_CurrentPickingList.Creator = objPickingList.Creator
                    m_CurrentPickingList.TotalItems = objPickingList.TotalItems
                    m_CurrentPickingList.ListStatus = objPickingList.ListStatus
#If RF Then
                    m_CurrentPickingList.PickerID = objPickingList.PickerID
#End If
                    'Creates a arraylist with the count as total number of items in the selected list
                    objProductInfoList = New ArrayList()
                    Dim iCounter As Integer = 0
                    Exit For
                End If
            Next
#If RF Then

           'Auto Fast fill PL CR
            If m_CurrentPickingList.ListType = "A" Then
                Me.m_Message.Location = New System.Drawing.Point(10, 105)
                'm_Message.Visible = True
                'm_Message.lblMsg.Text = 
                m_Message.SetMessage("Please wait, Sorting picking List")
            End If
            'Auto Fast Fill PL CR

            Dim query = From obj1 As PickingList In objProductInfoList _
            Order By obj1.BuisnessCentre, obj1.ProductGroup, obj1.ItemDesc _
            Select obj1
#End If
            'Calls the GetItemList method in DataEngine to update the ProductInfo list
            If objAppContainer.objDataEngine.GetProductInfo(strListId, objProductInfoList, m_CurrentPickingList.ListType) Then

                'nan Checks if counting in Sales Floor
                If m_CurrentPickingList.ListType = Macros.EXCESS_STOCK_PL_OSSR Or _
                   m_CurrentPickingList.ListType = Macros.EXCESS_STOCK_PL_SF Then

                    'nan loops through the items and adds the multisite info 
                    For Each objProductInfo As PLProductInfo In objProductInfoList

                        strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(objProductInfo.ProductCode)
                        strProductCode = strProductCode.Substring(0, 12)
                        tempMultiSiteList = New ArrayList()  'System testing SFA - Refresh the palnner list
                        'If objAppContainer.objDataEngine.GetPlannerListUsingBC(objProductInfo.BootsCode, True, tempMultiSiteList) Then
                        If objAppContainer.objDataEngine.GetPickingListMultisiteList(objProductInfo.BootsCode, m_CurrentPickingList.ListID, tempMultiSiteList) Then
                            multiSiteList.Clear()
                            'According to the repeat count include the module info
                            For Each ObjPlanner As PlannerInfo In tempMultiSiteList
                                RepeatCount = Convert.ToInt16(ObjPlanner.RepeatCount.ToString().Trim())
                                For Counter As Integer = 1 To RepeatCount
                                    multiSiteList.Add(ObjPlanner)
                                Next
                            Next
                            If multiSiteList.Count > 1 Then
                                'm_IsMultisite = True
                                'nan
                                'objAppContainer.objEXMultiSiteList.Clear()
                                'objProductInfo.MultiSiteList = New ArrayList()
                                Dim objEXPLData As EXMultiSiteInfo
                                Dim objSeqNum As Integer = 1
                                For Each strLocation As PlannerInfo In multiSiteList
                                    objEXPLData = New EXMultiSiteInfo
                                    objEXPLData.m_SeqNum = objSeqNum.ToString.PadLeft(3, "0")
                                    objEXPLData.m_strListId = m_CurrentPickingList.ListID
                                    objEXPLData.m_strProductCode = strProductCode
                                    objEXPLData.m_strPlannerDesc = strLocation.POGDesc
                                    objEXPLData.m_strPOGDescription = strLocation.Description
                                    objEXPLData.m_iSalesFloorQty = 0
                                    objEXPLData.m_bIsCounted = False

                                    'objAppContainer.objEXMultiSiteList.Add(objEXPLData)
                                    'nan new list
                                    objProductInfo.MultiSiteList.Add(objEXPLData)
                                    objSeqNum = objSeqNum + 1
                                Next

                                Dim objEXPLOthersData As EXMultiSiteInfo = New EXMultiSiteInfo()
                                objEXPLOthersData.m_SeqNum = objSeqNum.ToString.PadLeft(3, "0")
                                objEXPLOthersData.m_strListId = m_CurrentPickingList.ListID
                                objEXPLOthersData.m_strPlannerDesc = ""
                                objEXPLOthersData.m_strProductCode = strProductCode
                                objEXPLOthersData.m_strPOGDescription = "Other"
                                objEXPLOthersData.m_iSalesFloorQty = 0
                                objEXPLOthersData.m_bIsCounted = True

                                'objAppContainer.objEXMultiSiteList.Add(objEXPLOthersData)
                                'nan new list
                                objProductInfo.MultiSiteList.Add(objEXPLOthersData)

                            End If
                        End If
                    Next
                ElseIf m_CurrentPickingList.ListType = Macros.OSSR_PL Or _
                   m_CurrentPickingList.ListType = Macros.SHELF_MONITOR_PL Then

                    For Each objProductInfo As PLProductInfo In objProductInfoList

#If RF Then
                        Dim objRFDataSource As New SMRFDataSource()
                        'System Testing Added ENQ to fetch PSP
                        If Not objRFDataSource.GetPendingSalesFlag(objProductInfo) Then
                            Return False
                        End If
                        objRFDataSource = Nothing
#End If
                        'nan TODO Remove hardcode of flag
                        'objProductInfo.PendingSalesFlag = True

                        'nan Check IF present in PSP
                        If objProductInfo.PendingSalesFlag Then
                            objProductInfo.MultiSiteList = New ArrayList()
                            strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(objProductInfo.ProductCode)
                            strProductCode = strProductCode.Substring(0, 12)

                            Dim objEXPLData As EXMultiSiteInfo
                            Dim objSeqNum As Integer = 1

                            objEXPLData = New EXMultiSiteInfo
                            objEXPLData.m_SeqNum = objSeqNum.ToString.PadLeft(3, "0")
                            objEXPLData.m_strListId = m_CurrentPickingList.ListID
                            objEXPLData.m_strProductCode = strProductCode
                            objEXPLData.m_strPlannerDesc = Macros.MAIN_BACK_SHOP
                            objEXPLData.m_strPOGDescription = ""
                            objEXPLData.m_iBackShopQty = 0
                            objEXPLData.m_bIsCounted = False
                            objProductInfo.MultiSiteList.Add(objEXPLData)

                            objSeqNum = objSeqNum + 1

                            objEXPLData = New EXMultiSiteInfo
                            objEXPLData.m_SeqNum = objSeqNum.ToString.PadLeft(3, "0")
                            objEXPLData.m_strListId = m_CurrentPickingList.ListID
                            objEXPLData.m_strProductCode = strProductCode
                            objEXPLData.m_strPlannerDesc = Macros.PEND_SALES_PLAN
                            objEXPLData.m_strPOGDescription = ""
                            objEXPLData.m_iBackShopQty = 0
                            objEXPLData.m_bIsCounted = False
                            objProductInfo.MultiSiteList.Add(objEXPLData)
                        End If
                    Next
                End If

                'Adds the Corresponding ProductInfo to the m_PLMapping hashtable
                objAppContainer.objGlobalPLMappingTable.Add(strListId, objProductInfoList)
                Return True
            Else
                objAppContainer.objLogger.WriteAppLog("PLSessionMgr. Unable to load PL item list.", _
                                                      Logger.LogLevel.DEBUG)
                Return False
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in GetItemList of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit GetItemList of PLSessionMgr", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Retrieves the multisite data for the product code
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="multiSiteList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetEXMultiSiteInfo(ByVal strProductCode As String, ByRef multiSiteList As ArrayList) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered GetEXMultiSiteInfo of PLSessionMgr", Logger.LogLevel.INFO)
#If NRF Then
        If objAppContainer.objDataEngine.GetPlannerListUsingPC(strProductCode, True, multiSiteList) Then
            Return True
        Else
            Return False
        End If
#ElseIf RF Then
        If objAppContainer.objDataEngine.GetPlannerListUsingBC(strProductCode, True, multiSiteList) Then
            Return True
        Else
            Return False
        End If
#End If
        objAppContainer.objLogger.WriteAppLog("Exit GetEXMultiSiteInfo of PLSessionMgr", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Processes the selection of any product from the list view
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessProductSelection() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ProcessProductSelection of PLSessionMgr", Logger.LogLevel.INFO)
        Dim strListId As String = Nothing
        ''Checks whether the PL is AFF type
        '' AFF PL CR
        Dim bIsAFFPL As Boolean = False
        m_iNumItemsChecked = 0
        'nan  Reset the site selection
        m_SelectedSite = 0

        With m_PLHome
            Dim iIndex As Integer = 0
            Dim iCounter As Integer = 0
            Dim strUserId As String = ""
            'Gets the selected list id from the listview
            Try
                'get the list selected by user
                iCounter = .lstView.SelectedIndices(0)
                Dim bIsDataAvailable As Boolean = True
                'nan Dim bIsDataAvailable As Boolean = False
                'nan If .lstView.SelectedIndices.Count > 0 Then
                'nan For iCounter = 0 To .lstView.Items.Count - 1
                'If .lstView.Items(iCounter).Selected Then
                'nan bIsDataAvailable = True
                Dim objPickingList As PickingList = New PickingList()
                objPickingList = objAppContainer.objGlobalPickingList.Item(iCounter)
                strListId = objPickingList.ListID
                If objPickingList.ListType = "A" Then
                    bIsAFFPL = True
                End If
#If NRF Then
                strUserId = objPickingList.UserID
#ElseIf RF Then
                strUserId = objPickingList.Creator
#End If
                m_CurrentPickingList.ListID = objPickingList.ListID
                m_CurrentPickingList.ListType = objPickingList.ListType
                m_CurrentPickingList.ListTime = objPickingList.ListTime
                m_CurrentPickingList.Creator = objPickingList.Creator
                m_CurrentPickingList.TotalItems = objPickingList.TotalItems
                m_CurrentPickingList.ListStatus = objPickingList.ListStatus
                m_CurrentPickingList.UserID = objPickingList.UserID
#If RF Then
                m_CurrentPickingList.PickerID = objPickingList.PickerID
#End If
                m_iProductListCount = 0
                'nan To check if picking list is in Sales Floor
                If objPickingList.ListType = Macros.EXCESS_STOCK_PL_SF Or _
                objPickingList.ListType = Macros.EXCESS_STOCK_PL_OSSR Then
                    m_CurrentPickingList.IsSalesFloorList = True
                    m_FormHeader = "Picking List - SF"
                Else
                    m_CurrentPickingList.IsSalesFloorList = False
                    m_FormHeader = "Picking List - BS"
                End If

                'nan Exit For
                'End If
                'nan Next
                'nan End If
                'Check if the selected list has undergone location change and display the message accordingly.
                If m_CurrentPickingList.ListID.Equals(m_strConvertedList) Then
                    If m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M94"), "Info")
                    ElseIf m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_OSSR) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M95"), "Info")
                    ElseIf m_CurrentPickingList.ListType.Equals(Macros.OSSR_PL) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M93"), "Info")
                    End If
                    'The user should quit picking list and then start again.
                    Return False
                End If
                If bIsDataAvailable Then
                    'Check the list status for active or picked list and restrict the access accordingly.
                    'Checks if the picking list is completed. Shows the message if the list is already picked.
                    If m_CurrentPickingList.ListStatus.Equals(Macros.STATUS_PICKED) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M44"), "Info", _
                                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
                        'if the list is already picked then don't continue
                        Return False
                    ElseIf m_CurrentPickingList.ListStatus.Equals(Macros.STATUS_LIST_ACTIVE) Then
                        'if the list type is autofastfill then provide option to use or not use the list.
                        If m_CurrentPickingList.ListType = Macros.AUTO_FAST_FILL_PL Then
                            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M88"), "Attention", _
                                               MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                                               MessageBoxDefaultButton.Button1) = MsgBoxResult.No Then
                                Return False
                            End If
                        Else
#If RF Then
                            '@Service Fix
                            If (Not objAppContainer.strCurrentUserID.Equals(m_CurrentPickingList.PickerID)) Then
#End If
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M89"), "Info", _
                                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                                MessageBoxDefaultButton.Button1)
                                'If someone else is picking the list then, don't contiue
                                Return False
#If RF Then
                            End If
#End If
                        End If
                    End If
                    'Checks if the user accessing the picking list and 
                    'the user who created the picking list are the same
                    'If different, then prompts the user whether to continue or not.
#If NRF Then
                    If (Not m_CurrentPickingList.UserID.Equals(objAppContainer.strCurrentUserID) And _
                        (CInt(m_CurrentPickingList.UserID) > 100)) Then
#ElseIf RF Then
                    If (Not objAppContainer.strCurrentUserName.Equals(m_CurrentPickingList.Creator) And _
                        Not (m_CurrentPickingList.ListType.Equals(Macros.AUTO_FAST_FILL_PL))) Then
#End If
                        Dim iRes As Integer
                        iRes = MessageBox.Show(MessageManager.GetInstance().GetMessage("M38"), "Attention", _
                                               MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                                               MessageBoxDefaultButton.Button1)
                        If iRes = MsgBoxResult.No Then
                            Return False
                        End If
                    End If
                    'If the list is already selected, then obtains the details from the hashtable
                    'else retrieves the details from DB
                    If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                        DisplayPLScreen(PLSCREENS.ItemConfirm)
                        Return bIsDataAvailable
                    Else
                        'AFF PL CR
                        If bIsAFFPL Then
                            DisplayPLScreen(PLSCREENS.AFFMessage)
                        End If

                        If Not GetItemList(strListId) Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                            If bIsAFFPL Then
                                'm_Message.Close()
                                m_Message.Hide()
                                'm_PLHome.lblMessage.Visible = False
                            End If
                            Return False
                        Else
                            If bIsAFFPL Then
                                'm_Message.Close()
                                m_Message.Hide()
                                'm_PLHome.lblMessage.Visible = False
                            End If

                            DisplayPLScreen(PLSCREENS.ItemConfirm)
                            Return bIsDataAvailable
                        End If
                    End If
                Else
                    Return bIsDataAvailable
                    'TODO: Log error here
                    objAppContainer.objLogger.WriteAppLog("PLSessionMgr. Unable to locate picking list.", _
                                                          Logger.LogLevel.DEBUG)
                End If
            Catch ex As Exception
#If RF Then
                If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                    Throw ex
                End If
#End If
                objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessProductSelection of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            End Try
            'End If
        End With
        objAppContainer.objLogger.WriteAppLog("Exit ProcessProductSelection of PLSessionMgr", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' To re-initailise the screens according to the picking list selected.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetScreenControls()
        Try
            If m_CurrentPickingList.ListType.Equals(Macros.OSSR_PL) Then
                m_OSSRPLItemDetails.Dispose()
                m_OSSRPLItemDetails = New frmOSSRPLItemDetails()
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_OSSR) Then
                With m_OSSRPLItemDetails
                    'Set the new location.
                    .lblOffSiteHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                    .lblOffSiteHeader.Location = New System.Drawing.Point(12, 210)
                    .lblOffSiteHeader.Size = New System.Drawing.Size(140, 17)
                    .lblOffSiteHeader.Text = "Enter Off Site Qty:"

                    .btnBackShopCalcpad.Location = New System.Drawing.Point(211, 202)

                    .lblOSSRQty.Location = New System.Drawing.Point(167, 210)

                    .lblQtyReqHeader.Visible = True
                    .lblQtyRequired.Visible = True
                    .lblQtyReqHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                    .lblQtyReqHeader.Location = New System.Drawing.Point(12, 185)
                    .lblQtyReqHeader.Size = New System.Drawing.Size(150, 17)
                    .lblQtyReqHeader.Text = "Back Shop Quantity:"

                    .lblQtyRequired.Location = New System.Drawing.Point(167, 185)
                    .lblQtyRequired.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                End With
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.SHELF_MONITOR_PL) Then
                m_SMPLItemDetails.Dispose()
                m_SMPLItemDetails = New frmSMPLItemDetails()
                'ElseIf m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL) Then
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_OSSR) Then
                m_EXPLItemDetails.Dispose()
                m_EXPLItemDetails = New frmEXPLItemDetails()
                m_EXPLItemDetails.lblBackshopHeader.Visible = False
                m_EXPLItemDetails.lblTotalItemQty.Visible = False
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_SF) Then
                m_EXPLItemDetails.Dispose()
                m_EXPLItemDetails = New frmEXPLItemDetails()
            End If
        Catch ex As Exception

        End Try
    End Sub
    ''' <summary>
    ''' Processes the picking of SM list
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessSMPLEntry()
        objAppContainer.objLogger.WriteAppLog("Entered ProcessSMPLEntry of PLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim strProductCode As String = m_SMPLItemDetails.lblProductCodeDisplay.Text.ToString()
            Dim iBackShopQty As String = Val(m_SMPLItemDetails.lblBackShopQty.Text.ToString())

            'Checks if the back shop quantity is zero. If non zero then updates the data
            If objAppContainer.objHelper.ValidateZeroQty(iBackShopQty) = False And bIsProductCode Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M1"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
            Else
                'To unformat the product code by removing "-" and then remove CDV from that value
                Dim strUnFormattedProductCode As String
                strUnFormattedProductCode = objAppContainer.objHelper.UnFormatBarcode(strProductCode)
                strUnFormattedProductCode = strUnFormattedProductCode.Remove(strUnFormattedProductCode.Length - 1, 1)

                'Updates the list with modified data
                UpdateSMProductInfo(strUnFormattedProductCode, iBackShopQty)
                'Set hid calc pad button to true to not display calc pad
                bHideCalcPad = True
                'Displays the SM Item Details screen
                '#If NRF Then
                DisplayPLScreen(PLSCREENS.SMItemDetails)
                '#ElseIf RF Then
                '                DisplayPLScreen(PLSCREENS.ItemConfirm)
                '#End If
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessSMPLEntry of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessSMPLEntry of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processes the picking of OSSR list
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessOSSRPLEntry()
        objAppContainer.objLogger.WriteAppLog("Entered ProcessSMPLEntry of PLSessionMgr", Logger.LogLevel.INFO)
        Try

            Dim strProductCode As String = m_OSSRPLItemDetails.lblProductCodeDisplay.Text.ToString()
            Dim iOSSRQty As String = Val(m_OSSRPLItemDetails.lblOSSRQty.Text.ToString())

            'Checks if the back shop quantity is zero. If non zero then updates the data
            If objAppContainer.objHelper.ValidateZeroQty(iOSSRQty) = False And bIsProductCode Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M1"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
            Else
                'To unformat the product code by removing "-" and then remove CDV from that value
                Dim strUnFormattedProductCode As String
                strUnFormattedProductCode = objAppContainer.objHelper.UnFormatBarcode(strProductCode)
                strUnFormattedProductCode = strUnFormattedProductCode.Remove(strUnFormattedProductCode.Length - 1, 1)

                'Updates the list with modified data
                UpdateOSSRProductInfo(strUnFormattedProductCode, iOSSRQty)
                'Set hid calc pad button to true to not display calc pad
                bHideCalcPad = True
                'Displays the SM Item Details screen
                DisplayPLScreen(PLSCREENS.OSSRItemDetails)
                'DisplayPLScreen(PLSCREENS.ItemConfirm)
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessSMPLEntry of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessSMPLEntry of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processes the picking of EX list
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessEXPLEntry()
        objAppContainer.objLogger.WriteAppLog("Entered ProcessEXPLEntry of PLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim strLocation As String = ""
            Dim multiSiteList As ArrayList = New ArrayList()

            'Gets the list of objects from the hashtable 

            Dim strProductCode As String = m_EXPLItemDetails.lblProductCodeDisplay.Text.ToString().Trim()
            Dim iSalesFloorQty As Integer = Val(m_EXPLItemDetails.lblCurrentQty.Text.ToString())
            Dim bIsValidEntry As Boolean = True

            Dim objCurrentProductList As New ArrayList()
            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If

            Dim objCurrentProductInfo As PLProductInfo = New PLProductInfo() 'nan- SMPLProductInfo
            objCurrentProductInfo = objCurrentProductList.Item(m_iProductListCount)

            If Not objAppContainer.objHelper.ValidateZeroQty(iSalesFloorQty) Then
                If m_iZeroPressed = 3 Then

                    If PLSessionMgr.GetInstance().IsScanned Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M99"), "Alert", _
                                        MessageBoxButtons.OK, _
                                        MessageBoxIcon.Hand, _
                                        MessageBoxDefaultButton.Button1)
                        bIsValidEntry = False
                    Else
                        bIsValidEntry = True

                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M100"), "Info", _
                                                                       MessageBoxButtons.OK, _
                                                                       MessageBoxIcon.Asterisk, _
                                                                       MessageBoxDefaultButton.Button1)
                    End If
                    m_iZeroPressed = 0
                Else
                    Dim iVal As Integer = 0
                    iVal = MessageBox.Show(MessageManager.GetInstance().GetMessage("M45"), "Confirmation", _
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                                            MessageBoxDefaultButton.Button1)
                    If iVal = MsgBoxResult.Yes Then
                        If objCurrentProductInfo.BackShopQuantity > 0 Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M23"), "Info", _
                                            MessageBoxButtons.OK, _
                                            MessageBoxIcon.Asterisk, _
                                            MessageBoxDefaultButton.Button1)
                        End If
                        bIsValidEntry = True
                    Else
                        bIsValidEntry = False
                    End If
                    End If
            End If

            If bIsValidEntry Then
                'To unformat the product code by removing "-" and then remove CDV from that value
                Dim strUnFormattedProductCode As String
                strUnFormattedProductCode = objAppContainer.objHelper.UnFormatBarcode(strProductCode)
                strUnFormattedProductCode = strUnFormattedProductCode.Remove(strUnFormattedProductCode.Length - 1, 1)


                'Update excess stock multisite list.
                'nan update is being done in UpdateEXProductInfo
                'UpdateEXMultiSiteList(strUnFormattedProductCode, iSalesFloorQty)

                For Each objPickedData As EXMultiSiteInfo In objAppContainer.objEXMultiSiteList
                    If objPickedData.m_strListId.Equals(m_CurrentPickingList.ListID) AndAlso _
                        objPickedData.m_strProductCode.Equals(strUnFormattedProductCode) Then
                        'Add item to multisite list.
                        multiSiteList.Add(objPickedData.m_strPOGDescription & " (Counted)")
                    End If
                Next

                If multiSiteList.Count = 1 Then
                    strLocation = multiSiteList.Item(0).ToString()
                Else
                    With m_EXPLItemDetails
                        If .cmbLocation.Visible Then
                            If .cmbLocation.SelectedText.Equals("Select") Then
                                Exit Sub
                            Else
                                strLocation = .cmbLocation.SelectedItem.ToString()
                            End If
                        End If
                    End With
                End If
                'Updates the list with modified data
                If UpdateEXProductInfo(strUnFormattedProductCode, iSalesFloorQty, strLocation) Then
                    'Set hid calc pad button to true to not display calc pad
                    bHideCalcPad = True
                    'Displays the EX Item Details screen
                    DisplayPLScreen(PLSCREENS.EXItemDetails)
                    '#If NRF Then
                    '                DisplayPLScreen(PLSCREENS.EXItemDetails)
                    '#ElseIf RF Then
                    '                    DisplayPLScreen(PLSCREENS.ItemConfirm)
                    '#End If
                End If
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessEXPLEntry of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessEXPLEntry of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    '''' <summary>
    '''' Function to update the multisite list.
    '''' </summary>
    '''' <param name="strProductCode"></param>
    '''' <param name="Quantity"></param>
    '''' <remarks></remarks>
    'Public Sub UpdateEXMultiSiteList(ByVal strProductCode As String, ByVal Quantity As String)
    '    For Each objPickedData As EXMultiSiteInfo In objAppContainer.objEXMultiSiteList
    '        If (objPickedData.m_strProductCode = strProductCode AndAlso _
    '            objPickedData.m_SeqNum = m_SelectedSite) Then
    '            'nan changed the condition to m_selectedsite
    '            objPickedData.m_iSalesFloorQty = Quantity
    '            objPickedData.m_bIsCounted = True
    '        End If
    '    Next
    'End Sub

    ''' <summary>
    ''' Checks if discrepancy screen is required.
    ''' </summary>
    ''' <remarks></remarks>
    Public Function DiscrepancyCheck() As Integer
        m_DiscrepancyList = New ArrayList()
        'Returns 2 if no discrepancy
        Dim b_IsDiscrepancy As Integer = 2
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim strListId = m_CurrentPickingList.ListID
        Dim b_BackShop As Boolean
        'Dim m_site As EXMultiSiteInfo
        Dim objMBSData As EXMultiSiteInfo = New EXMultiSiteInfo()
        Dim objPSPData As EXMultiSiteInfo = New EXMultiSiteInfo()

        If objAppContainer.objGlobalPLMappingTable.ContainsKey(strListId) Then
            objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(strListId)
        End If

        If m_CurrentPickingList.ListType.Equals(Macros.OSSR_PL) Or _
               m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_OSSR) Or _
               m_CurrentPickingList.ListType = Macros.SHELF_MONITOR_PL Or _
               m_CurrentPickingList.ListType = Macros.EXCESS_STOCK_PL Then

            b_BackShop = True

        ElseIf m_CurrentPickingList.ListType = Macros.EXCESS_STOCK_PL_OSSR Or _
                   m_CurrentPickingList.ListType = Macros.EXCESS_STOCK_PL_SF Then

            b_BackShop = False

        End If
        If b_BackShop Then

            'Loop through all the items
            For Each objProductInfo As PLProductInfo In objCurrentProductInfoList

                'If item is multisite
                If objProductInfo.MultiSiteList.Count > 1 Then
                  
                    objMBSData = objProductInfo.MultiSiteList.Item(0)
                    objPSPData = objProductInfo.MultiSiteList.Item(1)
                    If objPSPData.m_bIsCounted And objMBSData.m_bIsCounted = False Then
                        b_IsDiscrepancy = 0
                        m_DiscrepancyList.Add(objProductInfo)
                    ElseIf objMBSData.m_bIsCounted And objPSPData.m_bIsCounted = False _
                              And b_IsDiscrepancy <> 0 Then
                        b_IsDiscrepancy = 1
                    End If
                    'If atleast one site is counted and List is not completely counted
                    'If objProductInfo.SalesFloorQuantity > -1 _
                    '    And objProductInfo.BackShopQuantity > -1 _
                    '    And objProductInfo.ListItemStatus <> Macros.STATUS_PICKED Then

                    '    'If list is counted in back shop
                    '    If b_BackShop Then
                    '        m_site = objProductInfo.MultiSiteList.Item(0)
                    '        'if MBS is counted 
                    '        If m_site.m_bIsCounted Then
                    '            'site left to be counted is PSP so no need for item
                    '            ' to be shown in discrepancy screen
                    '            If b_IsDiscrepancy <> 0 Then
                    '                b_IsDiscrepancy = 1
                    '            End If
                    '            Continue For
                    '        End If
                    '    End If

                    '    'Set flag to show discrepancy screen is required
                    '    b_IsDiscrepancy = 0

                    '    'If condition is satisfied add item to discrepancy list
                    '    m_DiscrepancyList.Add(objProductInfo)
                    'End If
                End If
            Next
        End If
        Return b_IsDiscrepancy
    End Function


    ''' <summary>
    ''' Processes the quit selection on the item confirmation screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessItemConfirmQuit()
        objAppContainer.objLogger.WriteAppLog("Entered ProcessItemConfirmQuit of PLSessionMgr", Logger.LogLevel.INFO)
        m_QuitInvokedScreen = Macros.ITEM_CONFIRM_QUIT
        m_DiscrepancyList = Nothing

        'For Fast fill PLs discrepancy screen not required
        If m_CurrentPickingList.ListType = Macros.AUTO_FAST_FILL_PL Or _
            m_CurrentPickingList.ListType = Macros.FAST_FILL_PL Then

            DisplayPLScreen(PLSCREENS.Finish)
        Else
            Dim discCheck As Integer = DiscrepancyCheck()
            'Discrepancy return 0 if MBS discrepancy,1 if PSP pending,2 if no discrepancy
            If discCheck = 0 Then
                DisplayPLScreen(PLSCREENS.Discrepancy)
            ElseIf discCheck = 1 Then
                DisplayPLScreen(PLSCREENS.PSPPending)
            ElseIf discCheck = 2 Then
                'Displays the Picking List Home screen on selection of Quit 
                DisplayPLScreen(PLSCREENS.Finish)
            End If
        End If

        'Support: Full Price Check Removed
        'If full price check is required for the previous item then disable to check.
        'm_bIsFullPriceCheckRequired = False
        m_strSEL = ""
        objAppContainer.objLogger.WriteAppLog("Exit ProcessItemConfirmQuit of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processes the quit selection on the item Details screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessItemDetailsQuit()
        objAppContainer.objLogger.WriteAppLog("Entered ProcessItemDetailsQuit of PLSessionMgr", Logger.LogLevel.INFO)
        'Set hid calc pad button to false
        bHideCalcPad = False
        m_QuitInvokedScreen = Macros.ITEM_DETAILS_QUIT
        m_DiscrepancyList = Nothing
        'For Fast fill PLs discrepancy screen not required
        If m_CurrentPickingList.ListType = Macros.AUTO_FAST_FILL_PL Or _
            m_CurrentPickingList.ListType = Macros.FAST_FILL_PL Then
            DisplayPLScreen(PLSCREENS.Finish)
        Else
            Dim discCheck As Integer = DiscrepancyCheck()
            'Discrepancy return 0 if MBS discrepancy,1 if PSP pending,2 if no discrepancy
            If discCheck = 0 Then
                DisplayPLScreen(PLSCREENS.Discrepancy)
            ElseIf discCheck = 1 Then
                DisplayPLScreen(PLSCREENS.PSPPending)
            ElseIf discCheck = 2 Then
                'Displays the Picking List Home screen on selection of Quit 
                DisplayPLScreen(PLSCREENS.Finish)
            End If
        End If
        
        objAppContainer.objLogger.WriteAppLog("Exit ProcessItemDetailsQuit of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processes the selection of No on the Finish Screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessCancelFinish()
        objAppContainer.objLogger.WriteAppLog("Entered ProcessCancelFinish of PLSessionMgr", Logger.LogLevel.INFO)
        'Checks from which screen the finish screen was invoked. 
        'Displays the corresponding screen.
        'if No from Finish screen
        m_iCancelQuit = 1
        If m_QuitInvokedScreen.Equals(Macros.ITEM_CONFIRM_QUIT) Then
            DisplayPLScreen(PLSCREENS.ItemConfirm)
        ElseIf m_QuitInvokedScreen.Equals(Macros.ITEM_DETAILS_QUIT) Then
            If m_CurrentPickingList.ListType.Equals(Macros.SHELF_MONITOR_PL) Or _
                m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL) Then
                DisplayPLScreen(PLSCREENS.ItemConfirm)
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.FAST_FILL_PL) Then
                DisplayPLScreen(PLSCREENS.FFItemDetails)
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.AUTO_FAST_FILL_PL) Then
                DisplayPLScreen(PLSCREENS.AFFItemDetails)
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_SF) Or _
                m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_OSSR) Then
                DisplayPLScreen(PLSCREENS.ItemConfirm)
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.OSSR_PL) Or _
                m_CurrentPickingList.ListType.Equals(Macros.OSSR_PL) Then
                DisplayPLScreen(PLSCREENS.OSSRItemDetails)
            End If
        End If
        objAppContainer.objLogger.WriteAppLog("Exit ProcessCancelFinish of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processes the selection of No on the PSPPending Screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessCancelPSPPending()
        objAppContainer.objLogger.WriteAppLog("Entered ProcessCancelPSPPending of PLSessionMgr", Logger.LogLevel.INFO)
        'Checks from which screen the finish screen was invoked. 
        'Displays the corresponding screen.
        'if no from PSPPending Screen
        m_iCancelQuit = 2
        DisplayPLScreen(PLSCREENS.ItemConfirm)
        objAppContainer.objLogger.WriteAppLog("Exit ProcessCancelPSPPending of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processes the Yes button selection on the Finish Screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessFinish()
        objAppContainer.objLogger.WriteAppLog("Entered ProcessFinish of PLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim objCurrentProductInfoList As ArrayList = New ArrayList()
            'Dim strDataTable As Hashtable = New Hashtable()
            'Dim strPickedArray As ArrayList = New ArrayList()
            Dim strEXUnCountedMSList As ArrayList = New ArrayList()
            Dim iEXUncountedLocations As Integer = 0

            'Identifies the Listids that are processed during the current session for SM and FF
            'For Each objCountedData As PLProductPickedData In m_PickedDataList
            '    Dim bIsUnique As Boolean = True
            '    If strDataTable.ContainsKey(objCountedData.m_strListId) Then
            '        bIsUnique = False
            '    End If
            '    If bIsUnique Then
            '        strDataTable.Add(objCountedData.m_strListId, objCountedData.m_strListType)
            '    End If
            'Next
            ''Identifies the Listids that are processed during the current session for EX
            'For Each objCountedData As EXMultiSiteInfo In objAppContainer.objEXMultiSiteList
            '    Dim bIsUnique As Boolean = True
            '    If strDataTable.ContainsKey(objCountedData.m_strListId) Then
            '        bIsUnique = False
            '    End If
            '    If bIsUnique Then
            '        'strDataTable.Add(objCountedData.m_strListId, Macros.EXCESS_STOCK_PL)
            '        strDataTable.Add(objCountedData.m_strListId, Macros.EXCESS_STOCK_PL_SF)
            '    End If
            'Next

            'Iterates through the list of objects to identify which all lists are fully picked
            'For Each strListId As String In strDataTable.Keys
            Dim bIsFullyPicked As Boolean = True
            Dim strListId = m_CurrentPickingList.ListID
            Dim strListType As String = m_CurrentPickingList.ListType
            If objAppContainer.objGlobalPLMappingTable.ContainsKey(strListId) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(strListId)
            End If
            'To prevent the case where user just enters a pcikign list and come out without picking.
            If objCurrentProductInfoList.Count > 0 Then
                If strListType.Equals(Macros.SHELF_MONITOR_PL) Or _
                    strListType.Equals(Macros.EXCESS_STOCK_PL) Then
                    'nan For Each objProductInfo As SMPLProductInfo In objCurrentProductInfoList
                    For Each objProductInfo As PLProductInfo In objCurrentProductInfoList
                        If Not objProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                            bIsFullyPicked = False
                        End If
                    Next
                    If bIsFullyPicked Then
                        'strPickedArray.Add(strListId)
                        m_CurrentPickingList.ListStatus = Macros.STATUS_PICKED
                    End If
                ElseIf strListType.Equals(Macros.OSSR_PL) Or _
                    strListType.Equals(Macros.EXCESS_STOCK_OSSR) Then
                    'OSSRPLProductInfo changed to  PLProductInfo
                    For Each objProductInfo As PLProductInfo In objCurrentProductInfoList
                        If Not objProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                            bIsFullyPicked = False
                        End If
                    Next
                    If bIsFullyPicked Then
                        'strPickedArray.Add(strListId)
                        m_CurrentPickingList.ListStatus = Macros.STATUS_PICKED
                    End If
                ElseIf strListType.Equals(Macros.FAST_FILL_PL) Then
                    'nan For Each objProductInfo As FFPLProductInfo In objCurrentProductInfoList
                    For Each objProductInfo As PLProductInfo In objCurrentProductInfoList
                        If Not objProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                            bIsFullyPicked = False
                        End If
                    Next
                    If bIsFullyPicked Then
                        'strPickedArray.Add(strListId)
                        m_CurrentPickingList.ListStatus = Macros.STATUS_PICKED
                    End If
                ElseIf strListType.Equals(Macros.AUTO_FAST_FILL_PL) Then

                    'nan For Each objProductInfo As AFFPLProductInfo In objCurrentProductInfoList
                    For Each objProductInfo As PLProductInfo In objCurrentProductInfoList
                        If Not objProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                            bIsFullyPicked = False
                        End If
                    Next
                    If bIsFullyPicked Then
                        'strPickedArray.Add(strListId)
                        m_CurrentPickingList.ListStatus = Macros.STATUS_PICKED
                    End If
                ElseIf strListType.Equals(Macros.EXCESS_STOCK_PL_SF) Or _
                    strListType.Equals(Macros.EXCESS_STOCK_PL_OSSR) Then
                    Dim iTotalQuantity As Integer = 0
                    'Identifies if the product us counted in all the locations

                    'nan For Each objProductData As EXPLProductInfo In objCurrentProductInfoList
                    For Each objProductData As PLProductInfo In objCurrentProductInfoList
                        Dim iNumLocations As Integer = 0
                        Dim bIsAllLocCounted As Boolean = True
                        Dim iMSCounted As Integer = 0
                        For Each objPickedData As EXMultiSiteInfo In objAppContainer.objEXMultiSiteList
                            If objPickedData.m_strListId.Equals(strListId) AndAlso _
                                objPickedData.m_strProductCode.Equals(objProductData.ProductCode) Then
                                iNumLocations += 1
                                ' This keeps a count of how many items have been counted in case of 
                                'multisite database
                                If Not objPickedData.m_bIsCounted Then
                                    iMSCounted += 1
                                End If

                                If bIsAllLocCounted Then
                                    If Not objPickedData.m_bIsCounted Then
                                        If Not objPickedData.m_strPOGDescription = "Other" Then
                                            bIsAllLocCounted = False
                                        End If
                                    End If
                                End If
                            End If
                        Next
                        If Not iNumLocations = iMSCounted + 1 Then
                            If iNumLocations > 1 AndAlso Not bIsAllLocCounted Then
                                'strEXUnCountedMSList.Add(strListId)
                                iEXUncountedLocations += 1
                            End If
                        End If
                        If iNumLocations = iMSCounted + 1 Then
                            bIsFullyPicked = False
                        End If
                        If iNumLocations = 1 AndAlso _
                        Not objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                            bIsFullyPicked = False
                        End If
                        'IT Internal
                        If iNumLocations = 0 AndAlso _
                        Not objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                            bIsFullyPicked = False
                        End If
                    Next
                    If bIsFullyPicked Then
                        'strPickedArray.Add(strListId)
                        m_CurrentPickingList.ListStatus = Macros.STATUS_PICKED
                    End If
                End If
                'Next
            End If
            'Updates the status as picked in the global picking list
            'For Each strListId As String In strPickedArray
            For Each objPickingList As PickingList In objAppContainer.objGlobalPickingList
                If objPickingList.ListID.Equals(strListId) And m_CurrentPickingList.ListStatus.Equals(Macros.STATUS_PICKED) Then
                    objPickingList.ListStatus = Macros.STATUS_PICKED
                End If
                'If objPickingList.ListID.Equals(m_CurrentPickingList.ListID) Then
                '    m_CurrentPickingList.ListStatus = Macros.STATUS_PICKED
                'End If
            Next
            'Next
            'Displays the message to the user that there are uncounted locations
            If iEXUncountedLocations > 0 Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M9"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
            Else
#If RF Then
                ''Fix for partial picking list
                Dim objPLXRecord As PLXRecord = New PLXRecord()
                objPLXRecord.strListID = m_CurrentPickingList.ListID
                'Fix for PLX referring to OLD PPC Code
                objPLXRecord.strItems = m_CurrentPickingList.TotalItems
                'TODO: Confirm what is meant by lines actioned
                objPLXRecord.strLineActioned = m_iNumItemsChecked
                'Change for OSSR Picking list 
                If m_CurrentPickingList.Location.Trim() = Nothing Then
                    If m_CurrentPickingList.ListStatus.Equals(Macros.STATUS_PICKED) And m_OSSRCount = 0 Then
                        objPLXRecord.cIsComplete = Macros.PLX_COMPLETE_YES
                    Else
                        objPLXRecord.cIsComplete = Macros.PLX_COMPLETE_NO
                    End If
                Else
                    objPLXRecord.cIsComplete = m_CurrentPickingList.Location
                End If

                If Not objAppContainer.objExportDataManager.CreatePLX(objPLXRecord) Then
                    objAppContainer.objLogger.WriteAppLog("Could not UpdateSalesFloorProductInfo of PLSessionMgr.", _
                                                          Logger.LogLevel.RELEASE)
                End If
                'Next
#End If
                'Clear all the internal session variables used.
                m_CurrentPickingList = New PickingList()
                'DisplayPLScreen(PLSCREENS.Summary)
                DisplayPLScreen(PLSCREENS.Home)
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessFinish of PLSessionMgr. Exception is: " _
                                                  + ex.Message + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessFinish of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
#If RF Then
    Public Function GeneratePLX() As PLXRecord
        Dim objPLXRecord As PLXRecord
        Try
            objPLXRecord = New PLXRecord
            objPLXRecord.strListID = m_CurrentPickingList.ListID
            objPLXRecord.strItems = m_CurrentPickingList.TotalItems
            objPLXRecord.strLineActioned = m_iNumItemsChecked
            If m_CurrentPickingList.ListStatus.Equals(Macros.STATUS_PICKED) Then
                objPLXRecord.cIsComplete = Macros.PLX_COMPLETE_YES
            Else
                objPLXRecord.cIsComplete = Macros.PLX_COMPLETE_NO
            End If
            If m_CurrentPickingList.Location = Macros.OSSR_PL Then
                objPLXRecord.cIsComplete = Macros.PLX_COMPLETE_OSSR
            End If
            If m_CurrentPickingList.Location = Macros.EXCESS_STOCK_PL Then
                objPLXRecord.cIsComplete = Macros.EXCESS_STOCK_PL
            End If
            If m_CurrentPickingList.Location = Macros.EXCESS_STOCK_OSSR Then
                objPLXRecord.cIsComplete = Macros.EXCESS_STOCK_OSSR
            End If
            Return objPLXRecord
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in Genearting PLX during Connection Loss. Exception:" + ex.Message, Logger.LogLevel.RELEASE)
            Return Nothing
        Finally
            objPLXRecord = Nothing
        End Try
    End Function
#End If
    ''' <summary>
    ''' Processes the GAP entry
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessGap(ByVal strProductCode As String)
        objAppContainer.objLogger.WriteAppLog("Entered ProcessGap of PLSessionMgr", Logger.LogLevel.INFO)
        Try
            'Bcak shop qty is zero, so Gap is reported
            Dim iBackShopQty As Integer = 0
            Dim iSalesFloorQty As Integer = 0
#If RF Then
            Dim iOSSRrQty As Integer = 0
#End If
            Dim objCurrentProductList As New ArrayList()
            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If

            Dim objCurrentProductInfo As PLProductInfo = New PLProductInfo() 'nan- SMPLProductInfo
            objCurrentProductInfo = objCurrentProductList.Item(m_iProductListCount)

            MessageBox.Show(MessageManager.GetInstance().GetMessage("M100"), "Info", _
                                               MessageBoxButtons.OK, _
                                               MessageBoxIcon.Asterisk, _
                                               MessageBoxDefaultButton.Button1)
            'To unformat the product code by removing "-" and then remove CDV from that value
            Dim strUnFormattedProductCode As String
            strUnFormattedProductCode = objAppContainer.objHelper.UnFormatBarcode(strProductCode)
            strUnFormattedProductCode = strUnFormattedProductCode.Remove(strUnFormattedProductCode.Length - 1, 1)

            'Fix for gap button in FF and EX PL
            If m_CurrentPickingList.ListType.Equals(Macros.SHELF_MONITOR_PL) Or _
             m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL) Then
                UpdateSMProductInfo(strUnFormattedProductCode, iBackShopQty)
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.FAST_FILL_PL) Then
                UpdateFFProductInfo(strUnFormattedProductCode)
                'Fix for GAp button 
#If RF Then

            ElseIf m_CurrentPickingList.ListType.Equals(Macros.OSSR_PL) Or _
             m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_OSSR) Then
                UpdateOSSRProductInfo(strUnFormattedProductCode, iOSSRrQty)
#End If
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_SF) Or _
             m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_OSSR) Then
                Dim icountedSite As Integer = 0
                If m_IsMultisite Then
                    'MessageBox.Show(MessageManager.GetInstance().GetMessage("M77"), "Info", _
                    '          MessageBoxButtons.OK, _
                    '          MessageBoxIcon.Asterisk, _
                    '          MessageBoxDefaultButton.Button1)
                    'Return
                Else
                    If objCurrentProductInfo.BackShopQuantity > 0 Then
                        'Displaying message for single sited item.
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M23"), "Info", _
                                  MessageBoxButtons.OK, _
                                  MessageBoxIcon.Asterisk, _
                                  MessageBoxDefaultButton.Button1)
                    End If
                End If
                    'Updates the list with modified data
                    UpdateEXProductInfo(strUnFormattedProductCode, iSalesFloorQty, " ")
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.AUTO_FAST_FILL_PL) Then
                    'Getting the Picked Status
                    Dim objCurrentProductInfoList As New ArrayList()
                    If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                        objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
                    End If

                    Dim objAFFCurrentProductInfo As PLProductInfo 'nan AFFPLProductInfo
                    objAFFCurrentProductInfo = New PLProductInfo() 'nan AFFPLProductInfo()
                    objAFFCurrentProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)
                    UpdateAFFProductInfo(strUnFormattedProductCode)
                    If Not (objAFFCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED)) Then
                        m_PickedCount += 1
                    End If
                    objAFFCurrentProductInfo = Nothing
                    objCurrentProductInfoList = Nothing
            End If

            'IT Internal
            'm_iNumGapItems += 1
            'm_iProductListCount = m_iProductListCount + 1
            'If ValidateNextAndback() Then
            If m_iZeroPressed = 1 Then
                DisplayPLScreen(PLSCREENS.ItemConfirm)
            ElseIf m_iZeroPressed = 2 Then
                DisplayPLScreen(PLSCREENS.SMItemDetails)
            End If
            m_iZeroPressed = 0
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessGap of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessGap of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Updates the SMPLProductInfo object with the change in back shop qty
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="iBackShopQty"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateSMProductInfo(ByVal strProductCode As String, ByVal iBackShopQty As Integer) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered UpdateSMProductInfo of PLSessionMgr", Logger.LogLevel.INFO)
        ''Updates the list of products
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Try
            'Gets the current product info from hash table based on the list id and 
            'the position of the item in the list as indicated by 
            'm_iProductListCount variable
            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If

            Dim objCurrentProductInfo As PLProductInfo = New PLProductInfo() 'nan- SMPLProductInfo
            objCurrentProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)


            Dim iSalesFloorVal As Integer = 0
            Dim iBackShopVal As Integer = 0

            'nan Sales Floor Quantity not required
            'If objCurrentProductInfo.SalesFloorQuantity < 0 Then
            '    iSalesFloorVal = 0
            'Else
            '    iSalesFloorVal = objCurrentProductInfo.SalesFloorQuantity
            'End If
            'ambli
            'For OSSR

            'nan 
            'If objCurrentProductInfo.BackShopQuantity < 0 Then
            '    iBackShopVal = 0
            'Else
            'iBackShopVal = objCurrentProductInfo.BackShopQuantity
            'End If

            'objCurrentProductInfo.TSF = iSalesFloorVal + iBackShopVal

            'Checks if the product is already picked
            'If not picked then add the item to the picked Items count

            Dim bIsProductAlreadyPicked As Boolean = False

            If objCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                bIsProductAlreadyPicked = True
            End If

            If m_IsMultisite Then
                'nan Reset the Back Shop Count before recalculating
                objCurrentProductInfo.BackShopQuantity = 0

                ' Dim objUpdatedProductData As EXMultiSiteInfo = New EXMultiSiteInfo()
                Dim iIndexLength As Integer = objAppContainer.objEXMultiSiteList.Count - 1
                Dim iCount As Integer = 0
                'Update the product info for a particular location 
                For iCount = 0 To iIndexLength
                    Dim objCountedProductData As EXMultiSiteInfo = New EXMultiSiteInfo()
                    objCountedProductData = objCurrentProductInfo.MultiSiteList.Item(iCount)

                    If objCountedProductData.m_strListId.Equals(m_CurrentPickingList.ListID) AndAlso _
                        objCountedProductData.m_strProductCode.Equals(strProductCode) AndAlso _
                        objCountedProductData.m_SeqNum = m_SelectedSite + 1 Then
                        'nan changed condition to seq number instead of location
                        'objUpdatedProductData = objCountedProductData
                        objCountedProductData.m_bIsCounted = True
                        objCountedProductData.m_iBackShopQty = iBackShopQty
                        objCountedProductData.m_CountPicked = SMQuantityToPick(objCurrentProductInfo, objCountedProductData.m_SeqNum)
                        'iIndex = iCount
                    End If
                    'nan Get Total Back Shop Count
                    objCurrentProductInfo.BackShopQuantity += objCountedProductData.m_iBackShopQty
                Next

                'Update Global list with Total Multisite count of the item


                'objAppContainer.objEXMultiSiteList.RemoveAt(iIndex)
                'objAppContainer.objEXMultiSiteList.Insert(iIndex, objUpdatedProductData)
                'Check if product is already picked or not
                'For Each objCountedData As EXMultiSiteInfo In objAppContainer.objEXMultiSiteList
                '    If objCountedData.m_strListId.Equals(m_CurrentPickingList.ListID) AndAlso _
                '        objCountedData.m_strProductCode.Equals(strProductCode) Then

                '        If objCountedData.m_bIsCounted Then
                '            bIsProductAlreadyPicked = True
                '        Else
                '            bIsProductAlreadyPicked = False
                '            Exit For
                '        End If
                '    End If
                'Next
                Dim objProductData As EXMultiSiteInfo = New EXMultiSiteInfo()
                objProductData = objCurrentProductInfo.MultiSiteList.Item(0)
                If objProductData.m_bIsCounted Then
                    bIsProductAlreadyPicked = True
                Else
                    bIsProductAlreadyPicked = False
                End If
            Else   'System Testing SFA - Included fix for single site item
                objCurrentProductInfo.BackShopQuantity = iBackShopQty
                objCurrentProductInfo.IsBSCounted = True
                bIsProductAlreadyPicked = True
            End If

            If bIsProductAlreadyPicked Then
                m_iNumItemsChecked += 1
                objCurrentProductInfo.ListItemStatus = Macros.STATUS_PICKED
            End If

            'Gets the modified values for back shop quantity 
            'Updates the lists accordingly
            If objCurrentProductInfo.ProductCode.Equals(strProductCode) Then
                'IT Internal
                If (objCurrentProductInfo.BackShopQuantity = 0) And objCurrentProductInfo.SalesFloorQuantity = 0 Then
                    objCurrentProductInfo.IsGapSelected = True
                Else
                    objCurrentProductInfo.IsGapSelected = False
                End If
                'IT Internal - end
            End If

            objCurrentProductInfoList.RemoveAt(m_iProductListCount)
            objCurrentProductInfoList.Insert(m_iProductListCount, objCurrentProductInfo)

            objAppContainer.objGlobalPLMappingTable.Remove(m_CurrentPickingList.ListID)
            objAppContainer.objGlobalPLMappingTable.Add(m_CurrentPickingList.ListID, objCurrentProductInfoList)
#If RF Then
            'If objPickedData.m_strListType.Equals(Macros.SHELF_MONITOR_PL) Then
            'Iterates through the Product list to write data for each Product Info
            'For Each objSMPLProductInfo As SMPLProductInfo In objCurrentProductInfoList
            'System Testing
            If objCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                'Writes the CLC record to signify Count List Item Data entered
                Dim objPLCRecord As PLCRecord = New PLCRecord()
                Dim bIsPLXRequired As Boolean = False
                'Sets the values
                objPLCRecord.strListID = m_CurrentPickingList.ListID
                objPLCRecord.strNumberSEQ = objCurrentProductInfo.Sequence
                objPLCRecord.strBootscode = objCurrentProductInfo.BootsCode
                objPLCRecord.strStockCount = objCurrentProductInfo.BackShopQuantity
                objPLCRecord.cIsGAPFlag = Macros.PLC_SM_FLAG
                'After the new field addition.
                objPLCRecord.strPickListLocation = Macros.BACK_SHOP
                objPLCRecord.strOSSRCount = objCurrentProductInfo.OSSRQuantity
                objPLCRecord.strUpdateOSSRItem = objCurrentProductInfo.OSSRFlag
                objPLCRecord.strLocationCounted = "  "  'To be updated
                objPLCRecord.strAllMSPicked = objCurrentProductInfo.MSFlag     'To be updated
                'objPLCRecord.strLocationCounted = "  "  'To be updated
                'objPLCRecord.strAllMSPicked = " "       'To be updated
                If Not objAppContainer.objExportDataManager.CreatePLC(objPLCRecord) Then
                    objAppContainer.objLogger.WriteAppLog("Cannot  UpdateSMProductInfo of PLSessionMgr. " _
                                            , Logger.LogLevel.RELEASE)
                End If
                'Set boolean to write PLX records
            End If
            'Next
            'End If
#End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in UpdateSMProductInfo of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False

        End Try
        objAppContainer.objLogger.WriteAppLog("Exit UpdateSMProductInfo of PLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Updates the OSSRPLProductInfo object with the change in OSSR qty
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="iOSSRQty"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateOSSRProductInfo(ByVal strProductCode As String, ByVal iOSSRQty As Integer) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered UpdateOSSRProductInfo of PLSessionMgr", Logger.LogLevel.INFO)
        ''Updates the list of products
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Try
            'Gets the current product info from hash table based on the list id and 
            'the position of the item in the list as indicated by 
            'm_iProductListCount variable
            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If
            'System Testing OSSRPLProductInfo changed to  PLProductInfo
            Dim objCurrentProductInfo As PLProductInfo = New PLProductInfo()
            objCurrentProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)

            Dim iSalesFloorVal As Integer = 0
            Dim iBackShopVal As Integer = 0

            If objCurrentProductInfo.SalesFloorQuantity < 0 Then
                iSalesFloorVal = 0
            Else
                iSalesFloorVal = objCurrentProductInfo.SalesFloorQuantity
            End If

            If objCurrentProductInfo.OSSRQuantity < 0 Then
                iBackShopVal = 0
            Else
                iBackShopVal = objCurrentProductInfo.OSSRQuantity
            End If


            'Checks if the product is already picked
            'If not picked then add the item to the picked Items count

            Dim bIsProductAlreadyPicked As Boolean = False

            If objCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                bIsProductAlreadyPicked = True
            End If

            'System testing SFA
            If m_IsMultisite Then
                'Reset the Back Shop Count before recalculating
                objCurrentProductInfo.OSSRQuantity = 0

                ' Dim objUpdatedProductData As EXMultiSiteInfo = New EXMultiSiteInfo()
                Dim iIndexLength As Integer = objAppContainer.objEXMultiSiteList.Count - 1
                Dim iCount As Integer = 0
                'Update the product info for a particular location 
                For iCount = 0 To iIndexLength
                    Dim objCountedProductData As EXMultiSiteInfo = New EXMultiSiteInfo()
                    objCountedProductData = objCurrentProductInfo.MultiSiteList.Item(iCount)

                    If objCountedProductData.m_strListId.Equals(m_CurrentPickingList.ListID) AndAlso _
                        objCountedProductData.m_strProductCode.Equals(strProductCode) AndAlso _
                        objCountedProductData.m_SeqNum = m_SelectedSite + 1 Then
                        ' changed condition to seq number instead of location
                        'objUpdatedProductData = objCountedProductData
                        objCountedProductData.m_bIsCounted = True
                        objCountedProductData.m_iBackShopQty = iOSSRQty
                        objCountedProductData.m_CountPicked = SMQuantityToPick(objCurrentProductInfo, objCountedProductData.m_SeqNum)
                        'iIndex = iCount
                    End If
                    'Get Total Back Shop Count
                    objCurrentProductInfo.OSSRQuantity += objCountedProductData.m_iBackShopQty
                Next
                Dim objProductData As EXMultiSiteInfo = New EXMultiSiteInfo()
                objProductData = objCurrentProductInfo.MultiSiteList.Item(0)
                If objProductData.m_bIsCounted Then
                    bIsProductAlreadyPicked = True
                Else
                    bIsProductAlreadyPicked = False
                End If
            Else   'System Testing SFA - Included fix for single site item
                objCurrentProductInfo.OSSRQuantity = iOSSRQty
                objCurrentProductInfo.IsBSCounted = True
                bIsProductAlreadyPicked = True
            End If

            If bIsProductAlreadyPicked Then 'System tesing SFA Removed not condition from if
                m_iNumItemsChecked += 1
                objCurrentProductInfo.ListItemStatus = Macros.STATUS_PICKED

                'Dim objPickedData As PLProductPickedData = New PLProductPickedData()
                'objPickedData.m_strListId = m_CurrentPickingList.ListID
                'objPickedData.m_strListType = Macros.OSSR_PL
                'objPickedData.m_strProductCode = objCurrentProductInfo.ProductCode
                'm_PickedDataList.Add(objPickedData)

            End If

            'Gets the modified values for back shop quantity 
            'Updates the lists accordingly
            If objCurrentProductInfo.ProductCode.Equals(strProductCode) Then
                'IT Internal
                If (objCurrentProductInfo.OSSRQuantity = 0) And objCurrentProductInfo.SalesFloorQuantity = 0 Then
                    objCurrentProductInfo.IsGapSelected = True
                Else
                    objCurrentProductInfo.IsGapSelected = False
                End If
                'IT Internal - end
            End If

            objCurrentProductInfoList.RemoveAt(m_iProductListCount)
            objCurrentProductInfoList.Insert(m_iProductListCount, objCurrentProductInfo)

            objAppContainer.objGlobalPLMappingTable.Remove(m_CurrentPickingList.ListID)
            objAppContainer.objGlobalPLMappingTable.Add(m_CurrentPickingList.ListID, objCurrentProductInfoList)
#If RF Then
            If objCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                'Writes the CLC record to signify Count List Item Data entered
                Dim objPLCRecord As PLCRecord = New PLCRecord()
                Dim bIsPLXRequired As Boolean = False
                'Sets the values
                objPLCRecord.strListID = m_CurrentPickingList.ListID
                objPLCRecord.strNumberSEQ = objCurrentProductInfo.Sequence
                objPLCRecord.strBootscode = objCurrentProductInfo.BootsCode
                objPLCRecord.strStockCount = objCurrentProductInfo.BackShopQuantity
                objPLCRecord.cIsGAPFlag = m_OSSRGapFlag 'Set the GAP Flag read during picking list load.
                objPLCRecord.strPickListLocation = Macros.OSSR
                objPLCRecord.strOSSRCount = objCurrentProductInfo.OSSRQuantity
                objPLCRecord.strUpdateOSSRItem = objCurrentProductInfo.OSSRFlag
                objPLCRecord.strLocationCounted = "  "  'To be updated
                objPLCRecord.strAllMSPicked = objCurrentProductInfo.MSFlag     'To be updated
                If Not objAppContainer.objExportDataManager.CreatePLC(objPLCRecord) Then
                    objAppContainer.objLogger.WriteAppLog("Cannot  UpdateSMProductInfo of PLSessionMgr. " _
                                            , Logger.LogLevel.RELEASE)
                End If
            End If
            'Next
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in UpdateOSSRProductInfo of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False

        End Try
        objAppContainer.objLogger.WriteAppLog("Exit UpdateSMProductInfo of PLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Updates the AFFPLProductInfo 
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateAFFProductInfo(ByVal strProductCode As String) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered UpdateAFFProductInfo of PLSessionMgr", Logger.LogLevel.INFO)
        ''Updates the list of products
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Try

            'Gets the current product info from hash table based on the list id and 
            'the position of the item in the list as indicated by 
            'm_iProductListCount variable
            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If

            Dim objCurrentProductInfo As PLProductInfo = New PLProductInfo() 'nan AFFPLProductInfo()
            objCurrentProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)

            'Checks if the item is already picked by checking the PLPickedData object

            Dim bIsProductAlreadyPicked As Boolean = False
            If objCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                bIsProductAlreadyPicked = True
            End If
            'If item is not already picked then adds it to the pickeddata list
            If Not bIsProductAlreadyPicked Then
                m_iNumItemsChecked += 1
                objCurrentProductInfo.ListItemStatus = Macros.STATUS_PICKED

                Dim objPickedData As PLProductPickedData = New PLProductPickedData()
                objPickedData.m_strListId = m_CurrentPickingList.ListID

                objPickedData.m_strListType = Macros.AUTO_FAST_FILL_PL

                objPickedData.m_strProductCode = objCurrentProductInfo.ProductCode
                'm_PickedDataList.Add(objPickedData)

            End If

            objCurrentProductInfoList.RemoveAt(m_iProductListCount)
            objCurrentProductInfoList.Insert(m_iProductListCount, objCurrentProductInfo)

            objAppContainer.objGlobalPLMappingTable.Remove(m_CurrentPickingList.ListID)
            objAppContainer.objGlobalPLMappingTable.Add(m_CurrentPickingList.ListID, objCurrentProductInfoList)
#If RF Then
            'If objPickedData.m_strListType.Equals(Macros.SHELF_MONITOR_PL) Then
            'Iterates through the Product list to write data for each Product Info
            'For Each objAFFPLProductInfo As AFFPLProductInfo In objCurrentProductInfoList
            'System Testing
            If objCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                'Writes the CLC record to signify Count List Item Data entered
                Dim objPLCRecord As PLCRecord = New PLCRecord()
                Dim bIsPLXRequired As Boolean = False
                'Sets the values
                objPLCRecord.strListID = m_CurrentPickingList.ListID
                objPLCRecord.strNumberSEQ = objCurrentProductInfo.Sequence
                objPLCRecord.strBootscode = objCurrentProductInfo.BootsCode
                objPLCRecord.strStockCount = objCurrentProductInfo.QuantityRequired
                objPLCRecord.cIsGAPFlag = Macros.PLC_FF_FLAG
                'After the new field addition.
                objPLCRecord.strPickListLocation = Macros.BACK_SHOP
                objPLCRecord.strOSSRCount = objCurrentProductInfo.OSSRQuantity
                objPLCRecord.strUpdateOSSRItem = objCurrentProductInfo.OSSRFlag
                objPLCRecord.strLocationCounted = "  "  'To be updated
                objPLCRecord.strAllMSPicked = " "       'To be updated
                If Not objAppContainer.objExportDataManager.CreatePLC(objPLCRecord) Then
                    objAppContainer.objLogger.WriteAppLog("Cannot  UpdateSMProductInfo of PLSessionMgr. " _
                                            , Logger.LogLevel.RELEASE)
                End If


            End If
            'Next

#End If


        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in UpdateAFFProductInfo of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit UpdateAFFProductInfo of PLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function

    ''' <summary>
    ''' Updates the FFPLProductInfo 
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateFFProductInfo(ByVal strProductCode As String) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered UpdateFFProductInfo of PLSessionMgr", Logger.LogLevel.INFO)
        ''Updates the list of products
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Try

            'Gets the current product info from hash table based on the list id and 
            'the position of the item in the list as indicated by 
            'm_iProductListCount variable
            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If

            Dim objCurrentProductInfo As PLProductInfo = New PLProductInfo()
            'nan Dim objCurrentProductInfo As FFPLProductInfo = New FFPLProductInfo()
            objCurrentProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)


            'Checks if the item is already picked by checking the PLPickedData object

            Dim bIsProductAlreadyPicked As Boolean = False
            If objCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                bIsProductAlreadyPicked = True
            End If
            'If item is not already picked then adds it to the pickeddata list
            If Not bIsProductAlreadyPicked Then
                m_iNumItemsChecked += 1
                objCurrentProductInfo.ListItemStatus = Macros.STATUS_PICKED
                Dim objPickedData As PLProductPickedData = New PLProductPickedData()
                objPickedData.m_strListId = m_CurrentPickingList.ListID
                'Checking for Auto fill/ Manual Fill

                objPickedData.m_strListType = Macros.FAST_FILL_PL
                objPickedData.m_strProductCode = objCurrentProductInfo.ProductCode
                'm_PickedDataList.Add(objPickedData)

            End If

            objCurrentProductInfoList.RemoveAt(m_iProductListCount)
            objCurrentProductInfoList.Insert(m_iProductListCount, objCurrentProductInfo)

            objAppContainer.objGlobalPLMappingTable.Remove(m_CurrentPickingList.ListID)
            objAppContainer.objGlobalPLMappingTable.Add(m_CurrentPickingList.ListID, objCurrentProductInfoList)

#If RF Then
            'If objPickedData.m_strListType.Equals(Macros.SHELF_MONITOR_PL) Then
            'Iterates through the Product list to write data for each Product Info
            'For Each objFFPLProductInfo As FFPLProductInfo In objCurrentProductInfoList
            'System Testing
            If objCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                'Writes the PLC record to signify Count List Item Data entered
                Dim objPLCRecord As PLCRecord = New PLCRecord()
                Dim bIsPLXRequired As Boolean = False
                'Sets the values
                objPLCRecord.strListID = m_CurrentPickingList.ListID
                objPLCRecord.strNumberSEQ = objCurrentProductInfo.Sequence
                objPLCRecord.strBootscode = objCurrentProductInfo.BootsCode
                objPLCRecord.strStockCount = "0"
                objPLCRecord.cIsGAPFlag = Macros.PLC_FF_FLAG
                'After the new field addition.
                objPLCRecord.strPickListLocation = Macros.BACK_SHOP
                'objPLCRecord.strOSSRCount = objCurrentProductInfo.OSSRQuantity
                objPLCRecord.strOSSRCount = "0"
                objPLCRecord.strUpdateOSSRItem = objCurrentProductInfo.OSSRFlag
                objPLCRecord.strLocationCounted = "  "  'To be updated
                objPLCRecord.strAllMSPicked = objCurrentProductInfo.MSFlag     'To be updated
                If Not objAppContainer.objExportDataManager.CreatePLC(objPLCRecord) Then
                    objAppContainer.objLogger.WriteAppLog("Cannot  UpdateSMProductInfo of PLSessionMgr. " _
                                            , Logger.LogLevel.RELEASE)
                End If
            End If
            'Next
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in UpdateFFProductInfo of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit UpdateFFProductInfo of PLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Updates the EXPLProductInfo object with the changes
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="iSalesFloorQty"></param>
    ''' <param name="strLocation"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateEXProductInfo(ByVal strProductCode As String, ByVal iSalesFloorQty As Integer, ByVal strLocation As String) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered UpdateEXProductInfo of PLSessionMgr", Logger.LogLevel.INFO)
        ''Updates the list of products
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Try
            'Gets the current product info from hash table based on the list id and 
            'the position of the item in the list as indicated by 
            'm_iProductListCount variable
            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If

            Dim objCurrentProductInfo As PLProductInfo = New PLProductInfo() 'nan- EXPLProductInfo 
            objCurrentProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)

            'Gets the modified values for sales floor quantity 
            'Updates the lists accordingly
            If objCurrentProductInfo.ProductCode.Equals(strProductCode) Then
                objCurrentProductInfo.SalesFloorQuantity = iSalesFloorQty
            End If


            'Dim iSalesFloorVal As Integer = 0
            'Dim iBackShopVal As Integer = 0

            'If objCurrentProductInfo.SalesFloorQuantity < 0 Then
            '    iSalesFloorVal = 0
            'Else
            '    iSalesFloorVal = objCurrentProductInfo.SalesFloorQuantity
            'End If
            'If objCurrentProductInfo.BackShopQuantity < 0 Then
            '    iBackShopVal = 0
            'Else
            '    iBackShopVal = objCurrentProductInfo.BackShopQuantity
            'End If
            ' objCurrentProductInfo.TSF = iSalesFloorVal + iBackShopVal
            If Not m_IsMultisite Then
                objCurrentProductInfo.ListItemStatus = Macros.STATUS_PICKED

                'To update the list item status
                'If item is not already picked then adds it to the pickeddata list
                Dim objPickedData As PLProductPickedData = New PLProductPickedData()
                objPickedData.m_strListId = m_CurrentPickingList.ListID
                objPickedData.m_strListType = m_CurrentPickingList.ListType
                objPickedData.m_strProductCode = objCurrentProductInfo.ProductCode

                'm_PickedDataList.Add(objPickedData)
            End If


            'Checks if the product is already counted 
            'If not counted then add the item to the Counted Items list

            Dim bIsProductAlreadyPicked As Boolean = False
            If objCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                bIsProductAlreadyPicked = True
            End If

            'Only if its a Multisite product do the following
            If m_IsMultisite Then
                ' Dim objUpdatedProductData As EXMultiSiteInfo = New EXMultiSiteInfo()
                Dim iIndexLength As Integer = objAppContainer.objEXMultiSiteList.Count - 1
                Dim iCount As Integer = 0
                'Update the product info for a particular location 
                For iCount = 0 To iIndexLength
                    Dim objCountedProductData As EXMultiSiteInfo = New EXMultiSiteInfo()
                    objCountedProductData = objCurrentProductInfo.MultiSiteList.Item(iCount)

                    If objCountedProductData.m_strListId.Equals(m_CurrentPickingList.ListID) AndAlso _
                        objCountedProductData.m_strProductCode.Equals(strProductCode) AndAlso _
                        objCountedProductData.m_SeqNum = m_SelectedSite + 1 Then
                        'nan changed condition to seq number instead of location
                        'objUpdatedProductData = objCountedProductData
                        objCountedProductData.m_bIsCounted = True
                        objCountedProductData.m_iSalesFloorQty = iSalesFloorQty
                        'iIndex = iCount
                        Exit For
                    End If
                Next

                'Update Global list with Total Multisite count of the item
                objCurrentProductInfo.SalesFloorQuantity = GetTotalMultisiteCounts(objCurrentProductInfo.ProductCode)

                'objAppContainer.objEXMultiSiteList.RemoveAt(iIndex)
                'objAppContainer.objEXMultiSiteList.Insert(iIndex, objUpdatedProductData)
                'Check if product is already picked or not
                For Each objCountedData As EXMultiSiteInfo In objAppContainer.objEXMultiSiteList
                    If objCountedData.m_strListId.Equals(m_CurrentPickingList.ListID) AndAlso _
                        objCountedData.m_strProductCode.Equals(strProductCode) Then

                        If objCountedData.m_bIsCounted Then
                            bIsProductAlreadyPicked = True
                        Else
                            bIsProductAlreadyPicked = False
                            Exit For
                        End If
                    End If
                Next
            End If

            If bIsProductAlreadyPicked Then
                m_iNumItemsChecked += 1
                objCurrentProductInfo.ListItemStatus = Macros.STATUS_PICKED
            End If

            If objCurrentProductInfo.ProductCode.Equals(strProductCode) Then
                'IT Internal
                If (objCurrentProductInfo.BackShopQuantity = 0) And objCurrentProductInfo.SalesFloorQuantity = 0 Then
                    objCurrentProductInfo.IsGapSelected = True
                Else
                    objCurrentProductInfo.IsGapSelected = False
                End If

                'IT Internal - end
                'objCurrentProductInfo.OSSRQuantity = iOSSRQty
            End If
            objCurrentProductInfoList.RemoveAt(m_iProductListCount)
            objCurrentProductInfoList.Insert(m_iProductListCount, objCurrentProductInfo)

            objAppContainer.objGlobalPLMappingTable.Remove(m_CurrentPickingList.ListID)
            objAppContainer.objGlobalPLMappingTable.Add(m_CurrentPickingList.ListID, objCurrentProductInfoList)
#If RF Then
            'If objPickedData.m_strListType.Equals(Macros.SHELF_MONITOR_PL) Then
            'Iterates through the Product list to write data for each Product Info
            'For Each objEXPLProductInfo As EXPLProductInfo In objCurrentProductInfoList
            'System Testing
            If objCurrentProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                'Writes the CLC record to signify Count List Item Data entered
                Dim objPLCRecord As PLCRecord = New PLCRecord()
                Dim bIsPLXRequired As Boolean = False
                'Sets the values
                objPLCRecord.strListID = m_CurrentPickingList.ListID
                objPLCRecord.strNumberSEQ = objCurrentProductInfo.Sequence
                objPLCRecord.strBootscode = objCurrentProductInfo.BootsCode
                objPLCRecord.strStockCount = objCurrentProductInfo.SalesFloorQuantity
                objPLCRecord.cIsGAPFlag = Macros.PLC_SM_FLAG
                'After the new field addition.
                objPLCRecord.strPickListLocation = Macros.SHOP_FLOOR
                objPLCRecord.strOSSRCount = objCurrentProductInfo.OSSRQuantity
                objPLCRecord.strUpdateOSSRItem = objCurrentProductInfo.OSSRFlag
                If m_IsMultisite Then
                    objPLCRecord.strLocationCounted = GetMultisiteLocation(objCurrentProductInfo.ProductCode, m_CurrentPickingList.ListID)
                    objPLCRecord.strAllMSPicked = "Y"
                Else
                    objPLCRecord.strLocationCounted = "  "
                    objPLCRecord.strAllMSPicked = " "
                End If
                If Not objAppContainer.objExportDataManager.CreatePLC(objPLCRecord) Then
                    objAppContainer.objLogger.WriteAppLog("Cannot  UpdateSMProductInfo of PLSessionMgr. " _
                                            , Logger.LogLevel.RELEASE)
                End If
                'Set boolean to write PLX records
            End If
            'Next
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in UpdateEXProductInfo of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit UpdateEXProductInfo of PLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the next button click on the Item Details screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessItemConfirmNext()
        objAppContainer.objLogger.WriteAppLog("Entered ProcessItemConfirmNext of PLSessionMgr", Logger.LogLevel.INFO)
        'Set hid calc pad button to false
        bHideCalcPad = False
        m_iProductListCount = m_iProductListCount + 1
        If ValidateNextAndback() Then
            DisplayPLScreen(PLSCREENS.ItemConfirm)
            'Support: Full Price Check Removed
            'If full price check is required for the previous item then disable to check.
            'm_bIsFullPriceCheckRequired = False
            m_strSEL = ""

        End If
        objAppContainer.objLogger.WriteAppLog("Exit ProcessItemConfirmNext of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processes the next button click on the Product Count screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessItemDetailsNext(ByVal iSender As Integer)
        objAppContainer.objLogger.WriteAppLog("Entered ProcessItemDetailsNext of PLSessionMgr", Logger.LogLevel.INFO)
        'Set hid calc pad button to false
        bHideCalcPad = False
        m_iProductListCount = m_iProductListCount + 1
        If ValidateNextAndback(iSender) Then
            DisplayPLScreen(PLSCREENS.ItemConfirm)
        End If
        objAppContainer.objLogger.WriteAppLog("Exit ProcessItemDetailsNext of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processes the next button click on the Product Count screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessSMPLItemDetailsNext()
        objAppContainer.objLogger.WriteAppLog("Entered ProcessSMPLItemDetailsNext of PLSessionMgr", Logger.LogLevel.INFO)
        Dim strProductCode As String = m_SMPLItemDetails.lblProductCodeDisplay.Text.ToString()
        Dim iBackShopQty As String = Val(m_SMPLItemDetails.lblBackShopQty.Text.ToString())
        Dim strUnFormattedProductCode As String

        strUnFormattedProductCode = objAppContainer.objHelper.UnFormatBarcode(strProductCode)
        strUnFormattedProductCode = strUnFormattedProductCode.Remove(strUnFormattedProductCode.Length - 1, 1)
        bHideCalcPad = True
        'Updates the list with modified data
        UpdateSMProductInfo(strUnFormattedProductCode, iBackShopQty)

        m_iProductListCount = m_iProductListCount + 1
        If ValidateNextAndback(Macros.SENDER_FORM_ACTION) Then
            DisplayPLScreen(PLSCREENS.ItemConfirm)
        End If
        objAppContainer.objLogger.WriteAppLog("Exit ProcessSMPLItemDetailsNext of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processes the next button click on the Product Count screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessEXPLItemDetailsNext()
        objAppContainer.objLogger.WriteAppLog("Entered ProcessEXPLItemDetailsNext of PLSessionMgr", Logger.LogLevel.INFO)
        Dim strProductCode As String = m_EXPLItemDetails.lblProductCodeDisplay.Text.ToString().Trim()
        Dim iSalesFloorQty As Integer = Val(m_EXPLItemDetails.lblCurrentQty.Text.ToString())
        Dim strUnFormattedProductCode As String
        Dim strLocation As String = ""
        Dim multiSiteList As ArrayList = New ArrayList()

        strUnFormattedProductCode = objAppContainer.objHelper.UnFormatBarcode(strProductCode)
        strUnFormattedProductCode = strUnFormattedProductCode.Remove(strUnFormattedProductCode.Length - 1, 1)

        For Each objPickedData As EXMultiSiteInfo In objAppContainer.objEXMultiSiteList
            If objPickedData.m_strListId.Equals(m_CurrentPickingList.ListID) AndAlso _
                objPickedData.m_strProductCode.Equals(strUnFormattedProductCode) Then
                'Add item to multisite list.
                multiSiteList.Add(objPickedData.m_strPOGDescription & " (Counted)")
            End If
        Next

        If multiSiteList.Count = 1 Then
            strLocation = multiSiteList.Item(0).ToString()
        Else
            With m_EXPLItemDetails
                If .cmbLocation.Visible Then
                    If .cmbLocation.SelectedText.Equals("Select") Then
                        Exit Sub
                    Else
                        strLocation = .cmbLocation.SelectedItem.ToString()
                    End If
                End If
            End With
        End If
        'Updates the list with modified data
        UpdateEXProductInfo(strUnFormattedProductCode, iSalesFloorQty, strLocation)

        m_iProductListCount = m_iProductListCount + 1
        If ValidateNextAndback(Macros.SENDER_FORM_ACTION) Then
            DisplayPLScreen(PLSCREENS.ItemConfirm)
        End If
        objAppContainer.objLogger.WriteAppLog("Exit ProcessEXPLItemDetailsNext of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processes the back button click on the item details screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessItemConfirmBack()
        objAppContainer.objLogger.WriteAppLog("Entered ProcessItemConfirmBack of PLSessionMgr", Logger.LogLevel.INFO)
        'Set hid calc pad button to false
        bHideCalcPad = False
        m_iProductListCount = m_iProductListCount - 1
        If ValidateNextAndback() Then
            DisplayPLScreen(PLSCREENS.ItemConfirm)
            'If full price check is required for the previous item then disable to check.
            'm_bIsFullPriceCheckRequired = False
            m_strSEL = ""
        End If
        objAppContainer.objLogger.WriteAppLog("Exit ProcessItemConfirmBack of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processes the back button click on the Product Count screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessItemDetailsBack(ByVal iSender As Integer)
        objAppContainer.objLogger.WriteAppLog("Entered ProcessItemDetailsBack of PLSessionMgr", Logger.LogLevel.INFO)
        'Set hide calc pad button to false
        bHideCalcPad = False
        m_iProductListCount = m_iProductListCount - 1
        If ValidateNextAndback(iSender) Then
            DisplayPLScreen(PLSCREENS.ItemConfirm)
        End If
        objAppContainer.objLogger.WriteAppLog("Exit ProcessItemDetailsBack of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processes the item selection from discrepancy screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessDispcrepancyItemSelect(ByVal index As Integer)
        objAppContainer.objLogger.WriteAppLog("Entered ProcessDispcrepancyItemSelect of PLSessionMgr", Logger.LogLevel.INFO)

        Dim objProductInfo As PLProductInfo = m_DiscrepancyList.Item(index)
#If RF Then
        m_iProductListCount = objProductInfo.Sequence - 1
#ElseIf NRF Then
        m_iProductListCount = objProductInfo.SequenceNumber - 1
#End If
        'nan select MBS as site
        m_SelectedSite = 0
        If ValidateNextAndback() Then
            DisplayPLScreen(PLSCREENS.ItemConfirm)
        End If
        objAppContainer.objLogger.WriteAppLog("Exit ProcessDispcrepancyItemSelect of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' validates the selection of Next and Back Buttons
    ''' </summary>
    ''' <param name="iSender"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ValidateNextAndback(Optional ByVal iSender As Integer = 0) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ValidateNextAndback of PLSessionMgr", Logger.LogLevel.INFO)
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim bIsValidChoice As Boolean = True
        Try

            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If

            'Validates the next and back buttons
            If m_iProductListCount < 0 Then
                m_iProductListCount = m_iProductListCount + 1
                'TODO: Confirm message and move to message manager
                'If the sender is from next button click of process then isender val is 2
                If Not iSender = Macros.SENDER_PROCESS_ACTION Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M7"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                    'MessageBox.Show("This is the first item in the Picking List")
                End If
                bIsValidChoice = False
            End If
            If (m_iProductListCount > objCurrentProductInfoList.Count - 1) Then
                m_iProductListCount = m_iProductListCount - 1
                'TODO: Confirm message and move to message manager
                'If the sender is from next button click of process then isender val is 2
                If Not iSender = Macros.SENDER_PROCESS_ACTION Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M8"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                    'MessageBox.Show("There are no more items in the picking list")
                End If
                DisplayPLScreen(PLSCREENS.ItemConfirm)
                bIsValidChoice = False
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in ValidateNextAndback of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ValidateNextAndback of PLSessionMgr", Logger.LogLevel.INFO)
        'nan Reset Selected site if item change is true
        If bIsValidChoice Then
            m_SelectedSite = 0
        End If
        Return bIsValidChoice
    End Function
    ''' <summary>
    ''' Processes the Yes button click on the PSPPending screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessPSPPendingYes()
        objAppContainer.objLogger.WriteAppLog("Entered ProcessPSPPendingYes of PLSessionMgr", Logger.LogLevel.INFO)
        Dim strListId = m_CurrentPickingList.ListID
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim bAllCounted As Boolean = True

        If objAppContainer.objGlobalPLMappingTable.ContainsKey(strListId) Then
            objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(strListId)
        End If

        For Each objProductInfo As PLProductInfo In objCurrentProductInfoList
            If objProductInfo.BackShopQuantity <= -1 Then
                bAllCounted = False
                Exit For
            End If
        Next
        If bAllCounted Then
            DisplayPLScreen(PLSessionMgr.PLSCREENS.Home)
        Else
            DisplayPLScreen(PLSessionMgr.PLSCREENS.Finish)
        End If
        objAppContainer.objLogger.WriteAppLog("Exit ProcessPSPPendingYes of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
#If NRF Then
    ''' <summary>
    ''' Writes the required data to the temporary data file
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub WriteTempData()
        objAppContainer.objLogger.WriteAppLog("Entered WriteTempData of PLSessionMgr", Logger.LogLevel.INFO)
        'Dim objSMExportDataManager As SMTransactDataManager = New SMTransactDataManager()
        Dim arrQueuedSELs As ArrayList = Nothing
        Dim iItemCount As Integer = 0
        Try
            'To collect all SEL Print request.
            arrQueuedSELs = New ArrayList()
            Dim bIsDataToWrite As Boolean = False
            'Integration testing
            Dim iNumSELs As Integer = 0
            'Iterates through the Count lists to identify if there is data to be written
            For Each objTempPickingList As PickingList In objAppContainer.objGlobalPickingList

                If objAppContainer.objGlobalPLMappingTable.ContainsKey(objTempPickingList.ListID) Then
                    'Checking if items are picked in the picking list
                    Dim objPLProductList = New ArrayList()
                    objPLProductList = objAppContainer.objGlobalPLMappingTable.Item(objTempPickingList.ListID)

                    If objTempPickingList.ListType.Equals(Macros.SHELF_MONITOR_PL) Then
                        'nan For Each objSMPLProductInfo As SMPLProductInfo In objPLProductList
                        For Each objSMPLProductInfo As PLProductInfo In objPLProductList
                            If objSMPLProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                bIsDataToWrite = True
                                Exit For
                            End If
                        Next
                    ElseIf objTempPickingList.ListType.Equals(Macros.FAST_FILL_PL) Then
                        'nan For Each objSMPLProductInfo As FFPLProductInfo In objPLProductList
                        For Each objSMPLProductInfo As PLProductInfo In objPLProductList
                            If objSMPLProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                bIsDataToWrite = True
                                Exit For
                            End If
                        Next
                    ElseIf objTempPickingList.ListType.Equals(Macros.AUTO_FAST_FILL_PL) Then
                        'nan For Each objSMPLProductInfo As AFFPLProductInfo In objPLProductList
                        For Each objSMPLProductInfo As PLProductInfo In objPLProductList
                            If objSMPLProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                bIsDataToWrite = True
                                Exit For
                            End If
                        Next
                    ElseIf objTempPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL) Or _
                        objTempPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_SF) Then

                        'nan For Each objSMPLProductInfo As EXPLProductInfo In objPLProductList
                        For Each objSMPLProductInfo As PLProductInfo In objPLProductList
                            If objSMPLProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                bIsDataToWrite = True
                                Exit For
                            End If
                        Next
                    End If
                    If bIsDataToWrite Then
                        Exit For
                    End If
                End If
            Next

            If bIsDataToWrite Then

                'Writes the CLO record to signify Count Lists Start
                objAppContainer.objExportDataManager.CreatePLO(SMTransactDataManager.ExFileType.PLTemp)

                Dim objPLProductInfoList As ArrayList = Nothing
                Dim bIsPLXRequired As Boolean = False

                'Iterates through the Count lists to write data for each count list
                For Each objPickingList As PickingList In objAppContainer.objGlobalPickingList

                    objPLProductInfoList = New ArrayList()

                    If objAppContainer.objGlobalPLMappingTable.ContainsKey(objPickingList.ListID) Then

                        'Obtains the product list for a particular list id
                        objPLProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(objPickingList.ListID)
                        'Reset item count in a picking list.
                        iItemCount = 0
                        'Checks the type of picking list and does the processing with the corresponding object
                        If objPickingList.ListType.Equals(Macros.SHELF_MONITOR_PL) Then
                            'Iterates through the Product list to write data for each Product Info

                            'nan For Each objSMPLProductInfo As SMPLProductInfo In objPLProductInfoList
                            For Each objSMPLProductInfo As PLProductInfo In objPLProductInfoList
                                'System Testing
                                If objSMPLProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                    'Writes the CLC record to signify Count List Item Data entered
                                    Dim objPLCRecord As PLCRecord = New PLCRecord()

                                    'Sets the values
                                    objPLCRecord.strListID = objPickingList.ListID
                                    objPLCRecord.strNumberSEQ = objSMPLProductInfo.SequenceNumber
                                    objPLCRecord.strBootscode = objSMPLProductInfo.BootsCode
                                    objPLCRecord.strStockCount = objSMPLProductInfo.BackShopQuantity
                                    objPLCRecord.cIsGAPFlag = Macros.PLC_SM_FLAG
                                    'After the new field addition.
                                    objPLCRecord.strPickListLocation = Macros.BACK_SHOP
                                    objPLCRecord.strOSSRCount = Macros.OSSR_COUNT
                                    objPLCRecord.strUpdateOSSRItem = Macros.UPDATE_OSSR_ITEM
                                    objPLCRecord.strLocationCounted = "  "  'To be updated
                                    objPLCRecord.strAllMSPicked = " "       'To be updated

                                    objAppContainer.objExportDataManager.CreatePLC(objPLCRecord, SMTransactDataManager.ExFileType.PLTemp)
                                    'Increment the number of items
                                    iItemCount += 1
                                    'Set boolean to write PLX records
                                    bIsPLXRequired = True
                                End If
                            Next
                        ElseIf objPickingList.ListType.Equals(Macros.FAST_FILL_PL) Then
                            'Iterates through the Product list to write data for each Product Info
                            'nan For Each objFFPLProductInfo As FFPLProductInfo In objPLProductInfoList
                            For Each objFFPLProductInfo As PLProductInfo In objPLProductInfoList
                                'System Testing
                                If objFFPLProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                    'Writes the CLC record to signify Count List Item Data entered
                                    Dim objPLCRecord As PLCRecord = New PLCRecord()

                                    'Sets the values
                                    objPLCRecord.strListID = objPickingList.ListID

                                    objPLCRecord.strNumberSEQ = objFFPLProductInfo.SequenceNumber
                                    objPLCRecord.strBootscode = objFFPLProductInfo.BootsCode
                                    'objPLCRecord.strStockCount = objFFPLProductInfo.TSF
                                    'Defaulted to 0 as there no count entered.
                                    objPLCRecord.strStockCount = "0"
                                    objPLCRecord.cIsGAPFlag = Macros.PLC_FF_FLAG
                                    'After the new field addition.
                                    objPLCRecord.strPickListLocation = Macros.BACK_SHOP
                                    objPLCRecord.strOSSRCount = Macros.OSSR_COUNT
                                    objPLCRecord.strUpdateOSSRItem = Macros.UPDATE_OSSR_ITEM
                                    objPLCRecord.strLocationCounted = "  "  'To be updated
                                    objPLCRecord.strAllMSPicked = " "       'To be updated

                                    objAppContainer.objExportDataManager.CreatePLC(objPLCRecord, SMTransactDataManager.ExFileType.PLTemp)
                                    'Increment the number of items
                                    iItemCount += 1
                                    'Set boolean to write PLX records
                                    bIsPLXRequired = True
                                End If

                            Next
                        ElseIf objPickingList.ListType.Equals(Macros.AUTO_FAST_FILL_PL) Then

                            'Iterates through the Product list to write data for each Product Info
                            'nan For Each objAFFPLProductInfo As AFFPLProductInfo In objPLProductInfoList
                            For Each objAFFPLProductInfo As PLProductInfo In objPLProductInfoList
                                'System Testing
                                If objAFFPLProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                    'Writes the CLC record to signify Count List Item Data entered
                                    Dim objPLCRecord As PLCRecord = New PLCRecord()

                                    'Sets the values
                                    objPLCRecord.strListID = objPickingList.ListID

                                    objPLCRecord.strNumberSEQ = objAFFPLProductInfo.SequenceNumber
                                    objPLCRecord.strBootscode = objAFFPLProductInfo.BootsCode
                                    'objPLCRecord.strStockCount = objFFPLProductInfo.TSF
                                    'Defaulted to Quantity since there is no Quantity entry
                                    objPLCRecord.strStockCount = objAFFPLProductInfo.QuantityRequired
                                    objPLCRecord.cIsGAPFlag = Macros.PLC_FF_FLAG
                                    'After the new field addition.
                                    objPLCRecord.strPickListLocation = Macros.BACK_SHOP
                                    objPLCRecord.strOSSRCount = Macros.OSSR_COUNT
                                    objPLCRecord.strUpdateOSSRItem = Macros.UPDATE_OSSR_ITEM
                                    objPLCRecord.strLocationCounted = "  "  'To be updated
                                    objPLCRecord.strAllMSPicked = " "       'To be updated

                                    objAppContainer.objExportDataManager.CreatePLC(objPLCRecord, SMTransactDataManager.ExFileType.PLTemp)
                                    'Increment the number of items
                                    iItemCount += 1
                                    'Set boolean to write PLX records
                                    bIsPLXRequired = True
                                End If
                            Next
                        ElseIf objPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL) Or _
                        objPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_SF) Then
                            'Iterates through the Product list to write data for each Product Info

                            'nan For Each objEXPLProductInfo As EXPLProductInfo In objPLProductInfoList
                            For Each objEXPLProductInfo As PLProductInfo In objPLProductInfoList
                                'System Testing
                                If objEXPLProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                    'Writes the CLC record to signify Count List Item Data entered
                                    Dim objPLCRecord As PLCRecord = New PLCRecord()
                                    Dim iMSCount As Integer = 1
                                    'Sets the values
                                    objPLCRecord.strListID = objPickingList.ListID
                                    objPLCRecord.strNumberSEQ = objEXPLProductInfo.SequenceNumber
                                    objPLCRecord.strBootscode = objEXPLProductInfo.BootsCode
                                    For Each objCountedData As EXMultiSiteInfo In objEXPLProductInfo.MultiSiteList
                                        If objCountedData.m_bIsCounted Then
                                            m_iTotalMultisiteCount += objCountedData.m_iSalesFloorQty
                                            iMSCount += 1
                                        End If
                                    Next
                                    If Not m_iTotalMultisiteCount = Nothing Then
                                        objPLCRecord.strStockCount = m_iTotalMultisiteCount
                                        m_iTotalMultisiteCount = Nothing
                                    Else
                                        objPLCRecord.strStockCount = objEXPLProductInfo.SalesFloorQuantity
                                    End If

                                    objPLCRecord.cIsGAPFlag = Macros.PLC_SM_FLAG  '@Service Fix Replaced PLC_EX_Flag to SM
                                    'After the new field addition.
                                    objPLCRecord.strPickListLocation = Macros.SHOP_FLOOR
                                    objPLCRecord.strOSSRCount = Macros.OSSR_COUNT
                                    objPLCRecord.strUpdateOSSRItem = Macros.UPDATE_OSSR_ITEM
                                    If (iMSCount > 1) Then
                                        objPLCRecord.strLocationCounted = GetMultisiteLocation(objEXPLProductInfo.ProductCode, objPickingList.ListID)
                                        objPLCRecord.strAllMSPicked = "Y"
                                    Else
                                        objPLCRecord.strLocationCounted = "  "
                                        objPLCRecord.strAllMSPicked = " "
                                    End If
                                    'Reset Multisite count to 1.
                                    iMSCount = 1
                                    'Write export data record.
                                    objAppContainer.objExportDataManager.CreatePLC(objPLCRecord, SMTransactDataManager.ExFileType.PLTemp)
                                    'Increment the number of items
                                    iItemCount += 1
                                    'Set boolean to write PLX records
                                    bIsPLXRequired = True
                                End If

                                'Write PRTs if any
                                If objEXPLProductInfo.NumSELsQueued > 0 Then
                                    'Integration Testing
                                    iNumSELs += objEXPLProductInfo.NumSELsQueued
                                    Dim iCount As Integer = 0

                                    For iCount = 0 To objEXPLProductInfo.NumSELsQueued - 1
                                        'Write PRT record for the SEL requests Queued
                                        Dim objPRTRecord As PRTRecord = New PRTRecord()

                                        'Sets the values
                                        'objPRTRecord.strBootscode = objAppContainer.objHelper.GeneratePCwithCDV(objEXPLProductInfo.BootsCode)
                                        objPRTRecord.strBootscode = objEXPLProductInfo.BootsCode
                                        objPRTRecord.cIsMethod = Macros.PRINT_BATCH

                                        'objSMExportDataManager.CreatePRT(objPRTRecord, SMExportDataManager.ExFileType.PLTemp)
                                        arrQueuedSELs.Add(objPRTRecord)
                                    Next
                                End If
                            Next    'Move to next EXPL item in the list.
                        End If
                        'Write PLX record.
                        If bIsPLXRequired Then
                            'Writes the PLX record to Signify Picking List Exit. 
                            Dim objPLXRecord As PLXRecord = New PLXRecord()
                            objPLXRecord.strListID = objPickingList.ListID
                            objPLXRecord.strItems = iItemCount
                            'TODO: Confirm what is meant by lines actioned
                            objPLXRecord.strLineActioned = "0001"
                            If objPickingList.ListStatus.Equals(Macros.STATUS_PICKED) Then
                                objPLXRecord.cIsComplete = Macros.PLX_COMPLETE_YES
                            Else
                                objPLXRecord.cIsComplete = Macros.PLX_COMPLETE_NO
                            End If
                            objAppContainer.objExportDataManager.CreatePLX(objPLXRecord, SMTransactDataManager.ExFileType.PLTemp)
                            'Unset boolean to write PLX record.
                            bIsPLXRequired = False
                        End If
                    End If
                Next
                'Writes the PLF record to Signify Count List Finished. 
                objAppContainer.objExportDataManager.CreatePLF(SMTransactDataManager.ExFileType.PLTemp)
                'write Price check records if any
                m_ModulePriceCheck.WriteExportData(arrQueuedSELs)
            Else
                objAppContainer.objLogger.WriteAppLog("No temp picking list data to be written.", Logger.LogLevel.INFO)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured while processing WriteTempData of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit WriteTempData of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub

#End If

    Private Function GetTotalMultisiteCounts(ByVal strProductCode As String) As Integer
        Dim iTotalCount As Integer = 0
        Try
            For Each objCountedData As EXMultiSiteInfo In objAppContainer.objEXMultiSiteList
                If objCountedData.m_strListId.Equals(m_CurrentPickingList.ListID) AndAlso _
                    objCountedData.m_strProductCode.Equals(strProductCode) Then
                    iTotalCount += objCountedData.m_iSalesFloorQty
                End If
            Next
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured while processing GetTotalMultisiteCounts of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try

        Return iTotalCount
    End Function
    Private Function GetMultisiteLocation(ByVal strProductCode As String, ByVal strListID As String) As String
        Dim iCount As Integer = 0
        Try
            For Each objCountedData As EXMultiSiteInfo In objAppContainer.objEXMultiSiteList
                If objCountedData.m_strProductCode.Equals(strProductCode) AndAlso _
                objCountedData.m_strListId.Equals(strListID) Then
                    iCount = iCount + 1
                End If
            Next
        Catch ex As Exception
            Return "  "
        End Try
        If (iCount < 1) Then
            Return "  "
        Else
            Return (iCount - 1).ToString().PadLeft(2, "0")
        End If
    End Function
    Private Function IsMultisiteTrackAdded(ByVal strProductCode As String, ByVal strListID As String, ByRef MultiSiteList As ArrayList) As Boolean
        Dim iCount As Integer = 0
        Try
            'nan changed multisite list to new list in PLProductInfo
            For Each objCountedData As EXMultiSiteInfo In MultiSiteList
                If objCountedData.m_strProductCode.Equals(strProductCode) AndAlso _
                objCountedData.m_strListId.Equals(strListID) Then
                    Return True
                End If
            Next
        Catch ex As Exception
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Writes the final set of data identified to the export data file
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteExportData() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered WriteExportData of PLSessionMgr", Logger.LogLevel.INFO)
        'Dim objSMExportDataManager As SMTransactDataManager = New SMTransactDataManager()

        Dim iItemCount As Integer = 0
        Try
            Dim bIsDataToWrite As Boolean = False
            'Integration testing
            Dim iNumSELs As Integer = 0
            'Iterates through the pickign lists to identify if there is data to be written
            For Each objTempPickingList As PickingList In objAppContainer.objGlobalPickingList
                If objAppContainer.objGlobalPLMappingTable.ContainsKey(objTempPickingList.ListID) Then
                    'Checking if items are picked in the picking list
                    Dim objPLProductList = New ArrayList()
                    objPLProductList = objAppContainer.objGlobalPLMappingTable.Item(objTempPickingList.ListID)

                    If objTempPickingList.ListType.Equals(Macros.SHELF_MONITOR_PL) Then

                        'nan For Each objSMPLProductInfo As SMPLProductInfo In objPLProductList
                        For Each objSMPLProductInfo As PLProductInfo In objPLProductList
                            If objSMPLProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                bIsDataToWrite = True
                                Exit For
                            End If
                        Next
                    ElseIf objTempPickingList.ListType.Equals(Macros.FAST_FILL_PL) Then
                        'nan For Each objSMPLProductInfo As FFPLProductInfo In objPLProductList
                        For Each objSMPLProductInfo As PLProductInfo In objPLProductList
                            If objSMPLProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                bIsDataToWrite = True
                                Exit For
                            End If
                        Next
                    ElseIf objTempPickingList.ListType.Equals(Macros.AUTO_FAST_FILL_PL) Then
                        'nan For Each objSMPLProductInfo As AFFPLProductInfo In objPLProductList
                        For Each objSMPLProductInfo As PLProductInfo In objPLProductList
                            If objSMPLProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                bIsDataToWrite = True
                                Exit For
                            End If
                        Next
                    ElseIf objTempPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL) Or _
                        objTempPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_SF) Then

                        'nan For Each objSMPLProductInfo As EXPLProductInfo In objPLProductList
                        For Each objSMPLProductInfo As PLProductInfo In objPLProductList
                            If objSMPLProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                bIsDataToWrite = True
                                Exit For
                            End If
                        Next
                    End If
                    Exit For
                End If
            Next

            If bIsDataToWrite Then
                'm_PLSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PLO")
                'Writes the CLO record to signify Count Lists Start
                objAppContainer.objExportDataManager.CreatePLO(SMTransactDataManager.ExFileType.EXData)

                Dim objPLProductInfoList As ArrayList = Nothing
                Dim bIsPLXRequired As Boolean = False

                'Iterates through the Count lists to write data for each count list
                For Each objPickingList As PickingList In objAppContainer.objGlobalPickingList
                    'initialise arraylist.
                    objPLProductInfoList = New ArrayList()
                    If objAppContainer.objGlobalPLMappingTable.ContainsKey(objPickingList.ListID) Then
                        'Obtains the product list for a particular list id
                        objPLProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(objPickingList.ListID)
                        'Reset picking list item count.
                        iItemCount = 0
                        'Checks the type of picking list and does the processing with the corresponding object
                        If objPickingList.ListType.Equals(Macros.SHELF_MONITOR_PL) Then
                            'm_PLSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PLC")
                            'Iterates through the Product list to write data for each Product Info

                            'nan For Each objSMPLProductInfo As SMPLProductInfo In objPLProductInfoList
                            For Each objSMPLProductInfo As PLProductInfo In objPLProductInfoList
                                'System Testing
                                If objSMPLProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                    'Writes the CLC record to signify Count List Item Data entered
                                    Dim objPLCRecord As PLCRecord = New PLCRecord()

                                    'Sets the values
                                    objPLCRecord.strListID = objPickingList.ListID
                                    objPLCRecord.strNumberSEQ = objSMPLProductInfo.SequenceNumber
                                    objPLCRecord.strBootscode = objSMPLProductInfo.BootsCode
                                    objPLCRecord.strStockCount = objSMPLProductInfo.BackShopQuantity
                                    objPLCRecord.cIsGAPFlag = Macros.PLC_SM_FLAG
                                    'After the new field addition.
                                    objPLCRecord.strPickListLocation = Macros.BACK_SHOP
                                    objPLCRecord.strOSSRCount = Macros.OSSR_COUNT
                                    objPLCRecord.strUpdateOSSRItem = Macros.UPDATE_OSSR_ITEM
                                    objPLCRecord.strLocationCounted = "  "  'To be updated
                                    objPLCRecord.strAllMSPicked = " "       'To be updated

                                    objAppContainer.objExportDataManager.CreatePLC(objPLCRecord, SMTransactDataManager.ExFileType.EXData)
                                    'Increment the count
                                    iItemCount = iItemCount + 1
                                    'Set boolean to write PLX records
                                    bIsPLXRequired = True
                                End If
                            Next
                        ElseIf objPickingList.ListType.Equals(Macros.FAST_FILL_PL) Then
                            'Iterates through the Product list to write data for each Product Info
                            'nan For Each objFFPLProductInfo As FFPLProductInfo In objPLProductInfoList
                            For Each objFFPLProductInfo As PLProductInfo In objPLProductInfoList
                                'System Testing
                                If objFFPLProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                    'Writes the PLC record to signify Count List Item Data entered
                                    'm_PLSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PLC")
                                    Dim objPLCRecord As PLCRecord = New PLCRecord()

                                    'Sets the values
                                    objPLCRecord.strListID = objPickingList.ListID
                                    objPLCRecord.strNumberSEQ = objFFPLProductInfo.SequenceNumber
                                    objPLCRecord.strBootscode = objFFPLProductInfo.BootsCode
                                    'objPLCRecord.strStockCount = objFFPLProductInfo.TSF
                                    objPLCRecord.strStockCount = "0"
                                    objPLCRecord.cIsGAPFlag = Macros.PLC_FF_FLAG
                                    'After the new field addition.
                                    objPLCRecord.strPickListLocation = Macros.BACK_SHOP
                                    objPLCRecord.strOSSRCount = Macros.OSSR_COUNT
                                    objPLCRecord.strUpdateOSSRItem = Macros.UPDATE_OSSR_ITEM
                                    objPLCRecord.strLocationCounted = "  "  'To be updated
                                    objPLCRecord.strAllMSPicked = " "       'To be updated

                                    objAppContainer.objExportDataManager.CreatePLC(objPLCRecord, SMTransactDataManager.ExFileType.EXData)
                                    'Increment the count
                                    iItemCount = iItemCount + 1
                                    'Set boolean to write PLX records
                                    bIsPLXRequired = True
                                End If
                            Next
                        ElseIf objPickingList.ListType.Equals(Macros.AUTO_FAST_FILL_PL) Then
                            'Iterates through the Product list to write data for each Product Info

                            'nan For Each objAFFPLProductInfo As AFFPLProductInfo In objPLProductInfoList
                            For Each objAFFPLProductInfo As PLProductInfo In objPLProductInfoList
                                'System Testing
                                If objAFFPLProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                    'Writes the PLC record to signify Count List Item Data entered
                                    'm_PLSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PLC")
                                    Dim objPLCRecord As PLCRecord = New PLCRecord()

                                    'Sets the values
                                    objPLCRecord.strListID = objPickingList.ListID
                                    objPLCRecord.strNumberSEQ = objAFFPLProductInfo.SequenceNumber
                                    objPLCRecord.strBootscode = objAFFPLProductInfo.BootsCode
                                    'objPLCRecord.strStockCount = objFFPLProductInfo.TSF
                                    'Changing to Quantity required
                                    objPLCRecord.strStockCount = objAFFPLProductInfo.QuantityRequired
                                    objPLCRecord.cIsGAPFlag = Macros.PLC_FF_FLAG
                                    'After the new field addition.
                                    objPLCRecord.strPickListLocation = Macros.BACK_SHOP
                                    objPLCRecord.strOSSRCount = Macros.OSSR_COUNT
                                    objPLCRecord.strUpdateOSSRItem = Macros.UPDATE_OSSR_ITEM
                                    objPLCRecord.strLocationCounted = "  "  'To be updated
                                    objPLCRecord.strAllMSPicked = " "       'To be updated

                                    objAppContainer.objExportDataManager.CreatePLC(objPLCRecord, SMTransactDataManager.ExFileType.EXData)
                                    'Increment the count
                                    iItemCount = iItemCount + 1
                                    'Set boolean to write PLX records
                                    bIsPLXRequired = True
                                End If
                            Next
                        ElseIf objPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL) Or _
                        objPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_SF) Then
                            'Iterates through the Product list to write data for each Product Info

                            'nan For Each objEXPLProductInfo As EXPLProductInfo In objPLProductInfoList
                            For Each objEXPLProductInfo As PLProductInfo In objPLProductInfoList
                                'System Testing
                                If objEXPLProductInfo.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
                                    'Writes the CLC record to signify Count List Item Data entered
                                    'm_PLSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PLC")
                                    Dim objPLCRecord As PLCRecord = New PLCRecord()
                                    Dim iMSCount As Integer = 1
                                    'Sets the values
                                    objPLCRecord.strListID = objPickingList.ListID
                                    objPLCRecord.strNumberSEQ = objEXPLProductInfo.SequenceNumber
                                    objPLCRecord.strBootscode = objEXPLProductInfo.BootsCode
                                    For Each objCountedData As EXMultiSiteInfo In objEXPLProductInfo.MultiSiteList
                                        If objCountedData.m_bIsCounted Then
                                            m_iTotalMultisiteCount += objCountedData.m_iSalesFloorQty
                                            iMSCount += 1
                                        End If
                                    Next
                                    If Not m_iTotalMultisiteCount = Nothing Then
                                        objPLCRecord.strStockCount = m_iTotalMultisiteCount
                                        m_iTotalMultisiteCount = Nothing
                                    Else
                                        objPLCRecord.strStockCount = objEXPLProductInfo.SalesFloorQuantity
                                    End If

                                    objPLCRecord.cIsGAPFlag = Macros.PLC_SM_FLAG
                                    'After the new field addition.
                                    objPLCRecord.strPickListLocation = Macros.SHOP_FLOOR  '@Service Fix Replaced B with S
                                    objPLCRecord.strOSSRCount = Macros.OSSR_COUNT
                                    objPLCRecord.strUpdateOSSRItem = Macros.UPDATE_OSSR_ITEM
                                    If (iMSCount > 1) Then
                                        objPLCRecord.strLocationCounted = (iMSCount - 2).ToString().PadLeft(2, "0")
                                        objPLCRecord.strAllMSPicked = "Y"
                                    Else
                                        objPLCRecord.strLocationCounted = "  "
                                        objPLCRecord.strAllMSPicked = " "
                                    End If
                                    'Reset Multisite count to 1.
                                    iMSCount = 1

                                    objAppContainer.objExportDataManager.CreatePLC(objPLCRecord, SMTransactDataManager.ExFileType.EXData)
                                    'Increment the count
                                    iItemCount = iItemCount + 1
                                    'Set boolean to write PLX records
                                    bIsPLXRequired = True
                                End If
                            Next
                        End If
                        If bIsPLXRequired Then
                            'Writes the PLX record to Signify Picking List Exit. 
                            'm_PLSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PLX")
                            Dim objPLXRecord As PLXRecord = New PLXRecord()
                            objPLXRecord.strListID = objPickingList.ListID
                            objPLXRecord.strItems = iItemCount
                            objPLXRecord.strLineActioned = "0001"
                            If objPickingList.ListStatus.Equals(Macros.STATUS_PICKED) Then
                                objPLXRecord.cIsComplete = Macros.PLX_COMPLETE_YES
                            Else
                                objPLXRecord.cIsComplete = Macros.PLX_COMPLETE_NO
                            End If
                            objAppContainer.objExportDataManager.CreatePLX(objPLXRecord, SMTransactDataManager.ExFileType.EXData)
                            'Reset the item count to 0.
                            iItemCount = 0
                            'Unset boolean to write PLX records
                            bIsPLXRequired = False
                        End If
                    End If
                Next

                'Writes the CLF record to Signify Count List Finished. 
                objAppContainer.objExportDataManager.CreatePLF(SMTransactDataManager.ExFileType.EXData)
                'Integration testing
                'Fix for IT bug BOOTS00000902
                'm_ModulePriceCheck.WriteExportData(iNumSELs)

                'To avoid writing duplicate record in export data file when export data download
                'while logging off fails.
                objAppContainer.objGlobalPickingList.Clear()
                objAppContainer.objGlobalPLMappingTable.Clear()
            Else
                objAppContainer.objLogger.WriteAppLog("No Export Data to be written", Logger.LogLevel.INFO)
            End If
            objAppContainer.objExportDataManager.DeleteTempFiles(SMTransactDataManager.ExFileType.PLTemp)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured while processing WriteExportData of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit WriteExportData of PLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    'IT Internal
    ''' <summary>
    ''' Function to return the actual number of GAP items 
    ''' </summary>
    ''' <remarks>Gap Counted for FF and EX Picking List</remarks>
    Private Function GetNumGapItems() As Integer
        Dim iNumGapItems As Integer = 0
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()

        Try
            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If
            If m_CurrentPickingList.ListType.Equals(Macros.SHELF_MONITOR_PL) Or _
             m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL) Then
                Dim objCurrentProductInfo As PLProductInfo = New PLProductInfo() 'nan- SMPLProductInfo
                For Each objCurrentProductInfo In objCurrentProductInfoList
                    If objCurrentProductInfo.IsGapSelected = True Then
                        iNumGapItems += 1
                    End If
                Next
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.FAST_FILL_PL) Then
                Dim objCurrentProductInfo As PLProductInfo = New PLProductInfo() 'nan FFPLProductInfo()
                For Each objCurrentProductInfo In objCurrentProductInfoList
                    If objCurrentProductInfo.IsGapSelected = True Then
                        iNumGapItems += 1
                    End If
                Next
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_SF) Or _
             m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_PL_OSSR) Then
                Dim objCurrentProductInfo As PLProductInfo = New PLProductInfo() 'nan- EXPLProductInfo 
                For Each objCurrentProductInfo In objCurrentProductInfoList
                    If objCurrentProductInfo.IsGapSelected = True Then
                        iNumGapItems += 1
                    End If
                Next
            ElseIf m_CurrentPickingList.ListType.Equals(Macros.OSSR_PL) Or _
             m_CurrentPickingList.ListType.Equals(Macros.EXCESS_STOCK_OSSR) Then
                'System Testing OSSRPLProductInfo changed to  PLProductInfo
                Dim objCurrentProductInfo As PLProductInfo = New PLProductInfo()
                For Each objCurrentProductInfo In objCurrentProductInfoList
                    If objCurrentProductInfo.IsGapSelected = True Then
                        iNumGapItems += 1
                    End If
                Next
            End If
            'Dim objCurrentProductInfo As SMPLProductInfo = New SMPLProductInfo()
            'For Each objCurrentProductInfo In objCurrentProductInfoList
            '    If objCurrentProductInfo.IsGapSelected = True Then
            '        iNumGapItems += 1
            '    End If
            'Next
            ' objCurrentProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)
            Return iNumGapItems

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in GetNumGapItems of PLSessionMgr. Exception is: " _
                                                                      + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return 0
        End Try
    End Function

#Region "OSSR"
    'ambli
    'For OSSR
#If RF Then
    Public Sub DisplayOSSRToggle(ByRef lblToggleOSSR As Label, ByVal strBarcode As String, ByVal strProdCode As String)
        'Dim objENQ As ENQRecord = New ENQRecord()
        Dim bCurrentOSSR_FLAG As Boolean = False
        Dim bResponse As Boolean = False
        'Dim strProdCode As String
        If lblToggleOSSR.Text = " " Then
            bCurrentOSSR_FLAG = False
        Else
            bCurrentOSSR_FLAG = True
        End If
        'Set hid calc pad button to false
        bHideCalcPad = False
        'strProdCode = objAppContainer.objDataEngine.GetProductCode(strBarcode)
        'bOSSRToggled = True
        'objAppContainer.objExportDataManager.CreateENQ(objENQ)
        bResponse = objAppContainer.objExportDataManager.CreateENQ_ToggleOSSR(strBarcode, Not (bCurrentOSSR_FLAG))
        If (bResponse) Then
            If Not (bCurrentOSSR_FLAG) Then
                lblToggleOSSR.Text = "OSSR"
                OOSRStatus(strProdCode) = "Y"
                'While in back shop one cannot enter offsire quantity
                'If m_CurrentPickingList.ListType = Macros.SHELF_MONITOR_PL Then
                '    m_SMPLItemDetails.lblBackShopHeader.Text = "Enter Off Site Qty: "
                'End If
                MessageBox.Show("The location setting for this item has been changed to 'OSSR'.", "Info", _
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            Else
                'm_OSSRCount -= 1
                OOSRStatus(strProdCode) = "N"
                lblToggleOSSR.Text = " "
                'when in offsite one cannot enter back shop quantity
                'If m_CurrentPickingList.ListType = Macros.SHELF_MONITOR_PL Then
                '    m_SMPLItemDetails.lblBackShopHeader.Text = "Enter Back Shop Qty: "
                'End If
                MessageBox.Show("The location setting for this item has been changed to 'Back Shop'.", "Info", _
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            End If
        Else
            MessageBox.Show("Unable to Toggle OSSR Status.", "Info ", _
                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
        End If
    End Sub
    ''' <summary>
    ''' Check for OSSR item in the picking list.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckOSSRItem()
        Try
            Dim objCurrentProductInfoList As ArrayList = New ArrayList()
            Dim iNumItemPicked As Integer = 0
            Dim iTotalCount As Integer = 0
            Dim bIsOSSRPresent As Boolean = False
            'Dim iTemp As ArrayList = New ArrayList
            m_OSSRCount = 0
            m_CurrentPickingList.Location = " "
            'Fix for not able to quit from OSSR list. For lists moved to OSSR location.
            If m_CurrentPickingList.ListType.Equals(Macros.OSSR_PL) Then
                Return False
            End If
            'If the list type is other then check.
            'For Each objPickingList As PickingList In objAppContainer.objGlobalPickingList
            'If objPickingList.ListID = m_CurrentPickingList.ListID Then
            Dim strListType As String = m_CurrentPickingList.ListType
            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
                'End If
                If strListType.Equals(Macros.SHELF_MONITOR_PL) Then
                    'For Each objProductData As SMPLProductInfo In objCurrentProductInfoList
                    '    If OOSRStatus(objAppContainer.objHelper.GeneratePCwithCDV(objProductData.ProductCode)) = "Y" Then
                    '        m_OSSRCount += 1
                    '    End If
                    'Next

                    'nan Dim iTemp = From objTemp As SMPLProductInfo In objCurrentProductInfoList Select objTemp _
                    Dim iTemp = From objTemp As PLProductInfo In objCurrentProductInfoList Select objTemp _
                            Where objTemp.OSSRFlag.Equals("O")
                    m_OSSRCount = iTemp.Count
                ElseIf (strListType.Equals(Macros.FAST_FILL_PL)) Then
                    'For Each objProductData As FFPLProductInfo In objCurrentProductInfoList
                    '    If OOSRStatus(objAppContainer.objHelper.GeneratePCwithCDV(objProductData.ProductCode)) = "Y" Then
                    '        m_OSSRCount += 1
                    '    End If
                    'Next
                    'nan Dim iTemp = From objTemp As FFPLProductInfo In objCurrentProductInfoList Select objTemp _    
                    Dim iTemp = From objTemp As PLProductInfo In objCurrentProductInfoList Select objTemp _
                            Where objTemp.OSSRFlag.Equals("O")
                    m_OSSRCount = iTemp.Count
                ElseIf (strListType.Equals(Macros.AUTO_FAST_FILL_PL)) Then
                    'For Each objProductData As AFFPLProductInfo In objCurrentProductInfoList
                    '    If OOSRStatus(objAppContainer.objHelper.GeneratePCwithCDV(objProductData.ProductCode)) = "Y" Then
                    '        m_OSSRCount += 1
                    '    End If
                    'Next

                    'nan Dim iTemp = From objTemp As AFFPLProductInfo In objCurrentProductInfoList Select objTemp _
                    Dim iTemp = From objTemp As PLProductInfo In objCurrentProductInfoList Select objTemp _
                            Where objTemp.OSSRFlag.Equals("O")
                    m_OSSRCount = iTemp.Count
                ElseIf strListType.Equals(Macros.EXCESS_STOCK_PL_SF) Or _
                       strListType.Equals(Macros.EXCESS_STOCK_PL_OSSR) Then
                    'For Each objProductData As EXPLProductInfo In objCurrentProductInfoList
                    '    If OOSRStatus(objAppContainer.objHelper.GeneratePCwithCDV(objProductData.ProductCode)) = "Y" Then
                    '        m_OSSRCount += 1
                    '    End If
                    'Next
                    Dim iTemp = From objTemp As PLProductInfo In objCurrentProductInfoList Select objTemp _
                                 Where objTemp.OSSRFlag.Equals("O")
                    'nan Dim iTemp = From objTemp As EXPLProductInfo In objCurrentProductInfoList Select objTemp _
                    If iTemp.Count > 0 Then
                        m_OSSRCount = iTemp.Count
                        DisplayPLScreen(PLSCREENS.MoveEXPL)
                        Return True
                    Else
                        Return False
                    End If
                End If
            End If
            'm_OSSRCount = iTemp.Count
            If m_OSSRCount > 0 Then
                DisplayPLScreen(PLSCREENS.OSSRFinish)
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in PL Session Manager - Check OSSRItem", Logger.LogLevel.ERROR)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Displays summary screen to move the backshop picking list to OSSR.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayMoveEXPL()
        objAppContainer.objLogger.WriteAppLog("Entered DisplayMoveEXPL of PLSessionMgr", Logger.LogLevel.INFO)
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim iNumItemPicked As Integer = 0
        Dim iTotalCount As Integer = 0
        Dim iRemainingItem As Integer = 0
        'm_OSSRCount = 0
        Try
            iTotalCount = m_CurrentPickingList.TotalItems
            'For Each objPickingList As PickingList In objAppContainer.objGlobalPickingList
            'If objPickingList.ListID = m_CurrentPickingList.ListID Then
            Dim strListType As String = m_CurrentPickingList.ListType
            If objAppContainer.objGlobalPLMappingTable.ContainsKey(m_CurrentPickingList.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalPLMappingTable.Item(m_CurrentPickingList.ListID)
            End If
            'For Each objProductData As EXPLProductInfo In objCurrentProductInfoList
            '    If objProductData.ListItemStatus.Equals(Macros.STATUS_PICKED) Then
            '        iNumItemPicked += 1
            '    End If
            '    If OOSRStatus(objAppContainer.objHelper.GeneratePCwithCDV(objProductData.ProductCode)) = "Y" Then
            '        m_OSSRCount += 1
            '    End If
            'Next

    'nan Dim lstPicked = From objTemp As EXPLProductInfo In objCurrentProductInfoList Where _
            Dim lstPicked = From objTemp As PLProductInfo In objCurrentProductInfoList Where _
                        objTemp.ListItemStatus.Equals(Macros.STATUS_PICKED)
            iNumItemPicked = lstPicked.Count
            'End If
            'Next

            With m_PLOSSRFinish
                If m_CurrentPickingList.ListType = Macros.EXCESS_STOCK_PL_SF Then
                    If iTotalCount = iNumItemPicked Then
                        .lblPLStatDisplay.Text = "Picking List is now complete"
                    Else
                        .lblPLStatDisplay.Text = "List incomplete, some items not checked"
                    End If
                    .lblConfirm.Text = "Do you want to send this list to the OSSR?"
                    .lblGAPReport.Text = "**Excess Stock List**"
                    .lblGAPAcetated.Text = "OSSR WAN Store"
                End If
                If m_CurrentPickingList.ListType = Macros.EXCESS_STOCK_PL_OSSR Then
                    If iTotalCount = iNumItemPicked Then
                        .lblPLStatDisplay.Text = "Picking List is now complete"
                    Else
                        .lblPLStatDisplay.Text = "List incomplete, some items not checked"
                    End If
                    .lblConfirm.Text = "Do you want to send this list to the Backshop?"
                    .lblGAPReport.Text = "**Excess Stock List**"
                    .lblGAPAcetated.Text = "Stock Floor Counting"
                End If
                '.lblOSSRItems.Text = m_OSSRCount.ToString() + " OSSR Item(s) in list"
                .lblOSSRItems.Visible = False
                iRemainingItem = Val(iTotalCount) - Val(iNumItemPicked)
                .lblGAPItems.Text = iRemainingItem.ToString() + " Item(S) not checked"
                'Dim iTemp As Integer = 0
                'iTemp = GetNumGapItems()
                '.lblGAPItems.Text = iTemp.ToString() + " Gap Items sent to report"
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayMoveExPl of PLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        'objAppContainer.objLogger.WriteAppLog("Exit DisplayMoveExPl of PLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="bOSSR"></param>
    ''' <remarks></remarks>
    Public Sub SendOSSRList(ByVal bOSSR As Boolean)
        Try
            If (bOSSR) Then
                If m_CurrentPickingList.ListType = Macros.EXCESS_STOCK_PL_SF Then
                    m_CurrentPickingList.Location = Macros.EXCESS_STOCK_OSSR
                ElseIf m_CurrentPickingList.ListType = Macros.EXCESS_STOCK_PL_OSSR Then
                    m_CurrentPickingList.Location = Macros.EXCESS_STOCK_PL
                Else
                    m_CurrentPickingList.Location = Macros.OSSR_PL
                End If
                'Set the changed list ID
                m_strConvertedList = m_CurrentPickingList.ListID
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in SendOSSRList " + ex.Message())
        End Try
    End Sub
#End If
#End Region


    Public Class PLProductPickedData
        Public m_strProductCode As String
        Public m_strListId As String
        Public m_strListType As String
    End Class
    Public Class EXMultiSiteInfo
        Public m_strListId As String
        Public m_strProductCode As String
        Public m_strPlannerDesc As String
        Public m_strPOGDescription As String
        Public m_iSalesFloorQty As Integer
        Public m_bIsCounted As Boolean
        Public m_SeqNum As String
        'nan Including BS count
        Public m_iBackShopQty As Integer
        Public m_CountPicked As Integer
        Public m_iOSSRQty As Integer
    End Class
    ''' <summary>
    ''' Enum Class that defines all screens for Shelf Monitor module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum PLSCREENS
        Home
        ItemConfirm
        SMItemDetails
        FFItemDetails
        AFFItemDetails
        EXItemDetails
        'ambli
        OSSRItemDetails
        Finish
        OSSRFinish
        MoveEXPL
        Summary
        ItemView
        AFFMessage
        Discrepancy
        PSPPending
    End Enum
    Public Enum FastFillType
        NULL
        ManualFastFill
        AutoFastFill
    End Enum
End Class

''' ***************************************************************************
''' <fileName>SMPLProductInfo.vb</fileName>
''' <summary>Inherits ProductInfo class and defines its own members. Used as 
''' value class for holding values for SM type picking list.
''' </summary>
''' <author>Infosys Technologies Ltd.,</author>
''' <DateModified></DateModified>
''' <remarks></remarks>
''' ***************************************************************************
Public Class SMPLProductInfo
    Inherits ProductInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_FirstBarcode As String
    Private m_SalesFloorQty As Integer
    Private m_BackShopQty As Integer
    Private m_QuantityRequired As Integer
    Private m_ItemStatus As String
    Private m_NumSELsQueued As Integer
    'IT Internal
    Private m_IsGapSelected As Boolean
    'ambli
    'For OSSR
    'Holds the OSSR quantity
    Private m_iOSSRQuantity As Integer
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub
    'AFF PL CR
    Private BC_Type As String
    Public Property BCType() As String
        Get
            Return BC_Type
        End Get
        Set(ByVal value As String)
            BC_Type = value
        End Set
    End Property

    Private PG_Group As String
    Public Property ProductGrp() As String
        Get
            Return PG_Group
        End Get
        Set(ByVal value As String)
            PG_Group = value
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
    ''' Gets or sets Sales floor quantity.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SalesFloorQuantity() As Integer
        Get
            Return m_SalesFloorQty
        End Get
        Set(ByVal value As Integer)
            m_SalesFloorQty = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the OSSR quantity
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OSSRQuantity() As Integer
        Get
            Return m_iOSSRQuantity
        End Get
        Set(ByVal value As Integer)
            m_iOSSRQuantity = value
        End Set
    End Property
    'IT Internal
    ''' <summary>
    ''' Gets or sets Sales floor quantity.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsGapSelected() As Integer
        Get
            Return m_IsGapSelected
        End Get
        Set(ByVal value As Integer)
            m_IsGapSelected = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets Back shop quantity.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BackShopQuantity() As Integer
        Get
            Return m_BackShopQty
        End Get
        Set(ByVal value As Integer)
            m_BackShopQty = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the quantity required.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property QuantityRequired() As Integer
        Get
            Return m_QuantityRequired
        End Get
        Set(ByVal value As Integer)
            m_QuantityRequired = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets Item Status.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ListItemStatus() As String
        Get
            Return m_ItemStatus
        End Get
        Set(ByVal value As String)
            m_ItemStatus = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets number of SELs queued
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NumSELsQueued() As Integer
        Get
            Return m_NumSELsQueued
        End Get
        Set(ByVal value As Integer)
            m_NumSELsQueued = value
        End Set
    End Property
    Private m_SequenceNumber As String
    Public Property SequenceNumber() As String
        Get
            Return m_SequenceNumber
        End Get
        Set(ByVal value As String)
            m_SequenceNumber = value
        End Set
    End Property
End Class

'ambli
'For OSSR
Public Class OSSRPLProductInfo
    Inherits ProductInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_FirstBarcode As String
    Private m_QuantityRequired As Integer
    Private m_ItemStatus As String
    Private m_NumSELsQueued As Integer

    'Fix for Gap in FF Picking List
    Private m_SalesFloorQty As Integer
    Private m_BackShopQty As Integer

    Private m_IsGapSelected As Boolean

    'ambli
    'For OSSR
    'Holds the OSSR quantity
    Private m_iOSSRQuantity As Integer
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
    ''' Gets or sets the quantity required.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property QuantityRequired() As Integer
        Get
            Return m_QuantityRequired
        End Get
        Set(ByVal value As Integer)
            m_QuantityRequired = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets Item Status.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ListItemStatus() As String
        Get
            Return m_ItemStatus
        End Get
        Set(ByVal value As String)
            m_ItemStatus = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets number of SELs queued
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NumSELsQueued() As Integer
        Get
            Return m_NumSELsQueued
        End Get
        Set(ByVal value As Integer)
            m_NumSELsQueued = value
        End Set
    End Property
    Private m_SequenceNumber As String
    Public Property SequenceNumber() As String
        Get
            Return m_SequenceNumber
        End Get
        Set(ByVal value As String)
            m_SequenceNumber = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets Sales floor quantity.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SalesFloorQuantity() As Integer
        Get
            Return m_SalesFloorQty
        End Get
        Set(ByVal value As Integer)
            m_SalesFloorQty = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets Back shop quantity.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BackShopQuantity() As Integer
        Get
            Return m_BackShopQty
        End Get
        Set(ByVal value As Integer)
            m_BackShopQty = value
        End Set
    End Property
    'IT Internal
    ''' <summary>
    ''' Gets or Sets gap selection
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsGapSelected() As Boolean
        Get
            Return m_IsGapSelected
        End Get
        Set(ByVal value As Boolean)
            m_IsGapSelected = value
        End Set
    End Property
    '' <summary>
    ''' Gets/Sets the OSSR quantity
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OSSRQuantity() As Integer
        Get
            Return m_iOSSRQuantity
        End Get
        Set(ByVal value As Integer)
            m_iOSSRQuantity = value
        End Set
    End Property
    ''' <summary>
    ''' Business Centre for the item
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BCType() As String
        Get
            Return BC_Type
        End Get
        Set(ByVal value As String)
            BC_Type = value
        End Set
    End Property
    ''' <summary>
    ''' Product Group for the item.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ProductGrp() As String
        Get
            Return PG_Group
        End Get
        Set(ByVal value As String)
            PG_Group = value
        End Set
    End Property
End Class

Public Class AFFPLProductInfo
    Inherits ProductInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_FirstBarcode As String
    Private m_QuantityRequired As Integer
    Private m_ItemStatus As String
    Private m_NumSELsQueued As Integer
    'AFF PL CR
    Private BC_Type As String
    Public Property BCType() As String
        Get
            Return BC_Type
        End Get
        Set(ByVal value As String)
            BC_Type = value
        End Set
    End Property

    Private PG_Group As String
    Public Property ProductGrp() As String
        Get
            Return PG_Group
        End Get
        Set(ByVal value As String)
            PG_Group = value
        End Set
    End Property

    'ambli
    'For OSSR
    'Holds the OSSR quantity
    Private m_iOSSRQuantity As Integer

    Public iMultisiteCnt As String

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
    Public Property MutiSite() As String
        Get
            Return iMultisiteCnt
        End Get
        Set(ByVal value As String)
            iMultisiteCnt = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the quantity required.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property QuantityRequired() As Integer
        Get
            Return m_QuantityRequired
        End Get
        Set(ByVal value As Integer)
            m_QuantityRequired = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets Item Status.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ListItemStatus() As String
        Get
            Return m_ItemStatus
        End Get
        Set(ByVal value As String)
            m_ItemStatus = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets number of SELs queued
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NumSELsQueued() As Integer
        Get
            Return m_NumSELsQueued
        End Get
        Set(ByVal value As Integer)
            m_NumSELsQueued = value
        End Set
    End Property
    Private m_SequenceNumber As String
    Public Property SequenceNumber() As String
        Get
            Return m_SequenceNumber
        End Get
        Set(ByVal value As String)
            m_SequenceNumber = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the OSSR quantity
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OSSRQuantity() As Integer
        Get
            Return m_iOSSRQuantity
        End Get
        Set(ByVal value As Integer)
            m_iOSSRQuantity = value
        End Set
    End Property
End Class

''' ***************************************************************************
''' <fileName>EXPLProductInfo.vb</fileName>
''' <summary>Inherits ProductInfo class and defines its own members. Used as 
''' value class for holding values for EX type picking list.
''' </summary>
''' <author>Infosys Technologies Ltd.,</author>
''' <DateModified></DateModified>
''' <remarks></remarks>
''' ***************************************************************************
Public Class EXPLProductInfo
    Inherits ProductInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_FirstBarcode As String
    Private m_SalesFloorQty As Integer
    Private m_BackShopQty As Integer
    Private m_ItemStatus As String
    Private m_NumSELsQueued As Integer
    Private m_MultiSiteCount As Integer

    Private m_IsGapSelected As Boolean

    'ambli
    'For OSSR
    'Holds the OSSR quantity
    Private m_iOSSRQuantity As Integer

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub
    'AFF PL CR
    Private BC_Type As String
    Public Property BCType() As String
        Get
            Return BC_Type
        End Get
        Set(ByVal value As String)
            BC_Type = value
        End Set
    End Property

    Private PG_Group As String
    Public Property ProductGrp() As String
        Get
            Return PG_Group
        End Get
        Set(ByVal value As String)
            PG_Group = value
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

    Public Property MultiSiteCount() As Integer
        Get
            Return m_MultiSiteCount
        End Get
        Set(ByVal value As Integer)
            m_MultiSiteCount = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets Sales floor quantity.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SalesFloorQuantity() As Integer
        Get
            Return m_SalesFloorQty
        End Get
        Set(ByVal value As Integer)
            m_SalesFloorQty = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets back shop quantity.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BackShopQuantity() As Integer
        Get
            Return m_BackShopQty
        End Get
        Set(ByVal value As Integer)
            m_BackShopQty = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets Item Status.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ListItemStatus() As String
        Get
            Return m_ItemStatus
        End Get
        Set(ByVal value As String)
            m_ItemStatus = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the OSSR quantity
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OSSRQuantity() As Integer
        Get
            Return m_iOSSRQuantity
        End Get
        Set(ByVal value As Integer)
            m_iOSSRQuantity = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets number of SELs queued
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NumSELsQueued() As Integer
        Get
            Return m_NumSELsQueued
        End Get
        Set(ByVal value As Integer)
            m_NumSELsQueued = value
        End Set
    End Property

    Private m_SequenceNumber As String
    Public Property SequenceNumber() As String
        Get
            Return m_SequenceNumber
        End Get
        Set(ByVal value As String)
            m_SequenceNumber = value
        End Set
    End Property
    'IT Internal
    ''' <summary>
    ''' Gets or sets Gap Selection
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsGapSelected() As Boolean
        Get
            Return m_IsGapSelected
        End Get
        Set(ByVal value As Boolean)
            m_IsGapSelected = value
        End Set
    End Property
End Class

''' ***************************************************************************
''' <fileName>PickingList.vb</fileName>
''' <summary>Inherits ProductInfo class and defines its own members. Used as 
''' value class for holding values for EX type picking list.
''' </summary>
''' <author>Infosys Technologies Ltd.,</author>
''' <DateModified></DateModified>
''' <remarks></remarks>
''' ***************************************************************************
Public Class PickingList
    Implements IComparable
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private m_Creator As String
    Private m_ListTime As String
    Private m_ListType As String
    Private m_ListID As String
    Private m_TotalItems As String
    Private m_ListStatus As String
    Private m_UserID As String
    Private m_BuisnessCentreType As String
    Private m_ProductGroup As String
    Private m_ItemDesc As String
    Private m_PickerID As String
    'nan To track whether list is counted in sales floor or backshop
    Private m_IsSalesFloorList As Boolean
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub
    Public Property IsSalesFloorList() As Boolean
        Get
            Return m_IsSalesFloorList
        End Get
        Set(ByVal value As Boolean)
            m_IsSalesFloorList = value
        End Set
    End Property

#If RF Then
    Private m_Location As String
    ''' <summary>
    ''' Gets or sets Location for RF
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Location() As String
        Get
            Return m_Location
        End Get
        Set(ByVal value As String)
            m_Location = value
        End Set
    End Property
    Public Property PickerID() As String
        Get
            Return m_PickerID
        End Get
        Set(ByVal value As String)
            m_PickerID = value
        End Set
    End Property

#End If
    ''' <summary>
    ''' Gets or Sets the list status
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ListStatus() As String
        Get
            Return m_ListStatus
        End Get
        Set(ByVal value As String)
            m_ListStatus = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or Sets the User ID.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UserID() As String
        Get
            Return m_UserID
        End Get
        Set(ByVal value As String)
            m_UserID = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the Creator user.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Creator() As String
        Get
            Return m_Creator
        End Get
        Set(ByVal value As String)
            m_Creator = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the List time
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ListTime() As String
        Get
            Return m_ListTime
        End Get
        Set(ByVal value As String)
            m_ListTime = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the List type
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ListType() As String
        Get
            Return m_ListType
        End Get
        Set(ByVal value As String)
            m_ListType = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the List ID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ListID() As String
        Get
            Return m_ListID
        End Get
        Set(ByVal value As String)
            m_ListID = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets total item present in the list.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TotalItems() As String
        Get
            Return m_TotalItems
        End Get
        Set(ByVal value As String)
            m_TotalItems = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or Sets the BuisnessCentre
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BuisnessCentre() As String
        Get
            Return m_BuisnessCentreType
        End Get
        Set(ByVal value As String)
            m_BuisnessCentreType = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or Sets the ProductGroup
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ProductGroup() As String
        Get
            Return m_ProductGroup
        End Get
        Set(ByVal value As String)
            m_ProductGroup = value
        End Set

    End Property
    ''' <summary>
    ''' Gets or Sets the ItemDesc
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ItemDesc() As String
        Get
            Return m_ItemDesc
        End Get
        Set(ByVal value As String)
            m_ItemDesc = value
        End Set
    End Property

    Public Function CompareTo(ByVal obj As Object) As Integer _
      Implements System.IComparable.CompareTo
        If TypeOf obj Is PickingList Then
            Dim objPickListCompare As PickingList = CType(obj, PickingList)
            Dim iResult As Integer = Me.ListTime.CompareTo(objPickListCompare.ListTime)

            If iResult = 0 Then
                iResult = Me.ListTime.CompareTo(objPickListCompare.ListTime)
            End If
            Return iResult
        End If
    End Function

End Class
Public Class FFPLProductInfo
    Inherits ProductInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_FirstBarcode As String
    Private m_QuantityRequired As Integer
    Private m_ItemStatus As String
    Private m_NumSELsQueued As Integer

    'Fix for Gap in FF Picking List
    Private m_SalesFloorQty As Integer
    Private m_BackShopQty As Integer

    Private m_IsGapSelected As Boolean

    'ambli
    'For OSSR
    'Holds the OSSR quantity
    Private m_iOSSRQuantity As Integer

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub
    'AFF PL CR
    Private BC_Type As String
    Public Property BCType() As String
        Get
            Return BC_Type
        End Get
        Set(ByVal value As String)
            BC_Type = value
        End Set
    End Property

    Private PG_Group As String
    Public Property ProductGrp() As String
        Get
            Return PG_Group
        End Get
        Set(ByVal value As String)
            PG_Group = value
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
    ''' Gets or sets the quantity required.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property QuantityRequired() As Integer
        Get
            Return m_QuantityRequired
        End Get
        Set(ByVal value As Integer)
            m_QuantityRequired = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets Item Status.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ListItemStatus() As String
        Get
            Return m_ItemStatus
        End Get
        Set(ByVal value As String)
            m_ItemStatus = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets number of SELs queued
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NumSELsQueued() As Integer
        Get
            Return m_NumSELsQueued
        End Get
        Set(ByVal value As Integer)
            m_NumSELsQueued = value
        End Set
    End Property
    Private m_SequenceNumber As String
    Public Property SequenceNumber() As String
        Get
            Return m_SequenceNumber
        End Get
        Set(ByVal value As String)
            m_SequenceNumber = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets Sales floor quantity.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SalesFloorQuantity() As Integer
        Get
            Return m_SalesFloorQty
        End Get
        Set(ByVal value As Integer)
            m_SalesFloorQty = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets Back shop quantity.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BackShopQuantity() As Integer
        Get
            Return m_BackShopQty
        End Get
        Set(ByVal value As Integer)
            m_BackShopQty = value
        End Set
    End Property
    'IT Internal
    ''' <summary>
    ''' Gets or Sets gap selection
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsGapSelected() As Boolean
        Get
            Return m_IsGapSelected
        End Get
        Set(ByVal value As Boolean)
            m_IsGapSelected = value
        End Set
    End Property
    '' <summary>
    ''' Gets/Sets the OSSR quantity
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OSSRQuantity() As Integer
        Get
            Return m_iOSSRQuantity
        End Get
        Set(ByVal value As Integer)
            m_iOSSRQuantity = value
        End Set
    End Property
End Class


Public Class PLProductInfo
    Inherits ProductInfo

    Private m_FirstBarcode As String
    'nan by default setting the quantities to -1
    Private m_SalesFloorQty As Integer = -1
    Private m_BackShopQty As Integer = -1
    Private m_ItemStatus As String
    Private m_NumSELsQueued As Integer
    'nan Redundant
    'Private m_MultiSiteCount As Integer
    Private m_IsGapSelected As Boolean
    Private m_iOSSRQuantity As Integer
    Private BC_Type As String
    Private PG_Group As String
    Private m_SequenceNumber As String
    Private m_QuantityRequired As Integer
    Private m_bIsBSCounted As Boolean
    'nan Include the multisite information
    Private m_MultiSiteList As ArrayList = New ArrayList()
    Private m_IsUpdated As Boolean = False
    'nan Tracks the number of items picked
    Private m_SMQuantityPicked As Integer



    Sub New()

    End Sub
    Public Property SMQuantityPicked() As Integer
        Get
            Return m_SMQuantityPicked
        End Get
        Set(ByVal value As Integer)
            m_SMQuantityPicked = value
        End Set
    End Property
    Public Property MultiSiteList() As ArrayList
        Get
            Return m_MultiSiteList
        End Get
        Set(ByVal value As ArrayList)
            m_MultiSiteList = value
        End Set
    End Property
    Public Property QuantityRequired() As String
        Get
            Return m_QuantityRequired
        End Get
        Set(ByVal value As String)
            m_QuantityRequired = value
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

    Public Property FirstBarcode() As String
        Get
            Return m_FirstBarcode
        End Get
        Set(ByVal value As String)
            m_FirstBarcode = value
        End Set
    End Property

    Public Property SalesFloorQuantity() As Integer
        Get
            Return m_SalesFloorQty
        End Get
        Set(ByVal value As Integer)
            m_SalesFloorQty = value
        End Set
    End Property

    Public Property BackShopQuantity() As Integer
        Get
            Return m_BackShopQty
        End Get
        Set(ByVal value As Integer)
            m_BackShopQty = value
        End Set
    End Property

    Public Property ListItemStatus() As String
        Get
            Return m_ItemStatus
        End Get
        Set(ByVal value As String)
            m_ItemStatus = value
        End Set
    End Property

    Public Property OSSRQuantity() As Integer
        Get
            Return m_iOSSRQuantity
        End Get
        Set(ByVal value As Integer)
            m_iOSSRQuantity = value
        End Set
    End Property

    Public Property NumSELsQueued() As Integer
        Get
            Return m_NumSELsQueued
        End Get
        Set(ByVal value As Integer)
            m_NumSELsQueued = value
        End Set
    End Property

    Public Property SequenceNumber() As String
        Get
            Return m_SequenceNumber
        End Get
        Set(ByVal value As String)
            m_SequenceNumber = value
        End Set
    End Property

    Public Property IsGapSelected() As Boolean
        Get
            Return m_IsGapSelected
        End Get
        Set(ByVal value As Boolean)
            m_IsGapSelected = value
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
    Public Property IsBSCounted() As Boolean
        Get
            Return m_bIsBSCounted
        End Get
        Set(ByVal value As Boolean)
            m_bIsBSCounted = value
        End Set
    End Property
    Public Property ISUpdated() As Boolean
        Get
            Return m_IsUpdated
        End Get
        Set(ByVal value As Boolean)
            m_IsUpdated = value
        End Set
    End Property
End Class