Imports System.Windows.Forms
'''***************************************************************
''' <FileName>EXSessionMgr.vb</FileName>
''' <summary>
''' The Excess Stock Container Class.
''' Implements all business logic and GUI navigation for Excess Stock.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>25-Oct-2011</DateModified>
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
Public Class EXSessionMgr
    Private Shared objEXSessionMgr As EXSessionMgr
    Private m_EXhome As frmEXHome
    Private m_EXItemDetails As frmEXItemDetails
    Private m_EXLocationSelection As frmEXSelectLocation
    Private m_EXView As frmView
    Private m_EXSummary As frmEXSummary
    Private m_EXProductInfo As EXProductInfo = Nothing
    Private m_EXItemList As ArrayList = Nothing
    Private m_EXHelper As Helper
    Private m_iScannedProdCount As Integer
    Public bIsAlreadyScanned As Boolean = False
    Private m_bItemScanned As Boolean = False
    Private isNewItem As Boolean = False
    Private m_bIsProdCodeScanned As Boolean = False
    'Variable to keep track of sequence number
    Private m_iSequence As Integer = 0
    Public PreviousScreen As EXSCREENS
    'Added as part of SFA
    Public m_ItemScreen As String = Nothing
    Public m_IndexToSelect As Integer = 0
    Public m_IsCountPend As Boolean = False
    Public IsQtyUpdated As Boolean = False
    Private SelectedIndex As Integer
    Private strProductValid As String = "Y"
#If RF Then
    Private strPreviousBkShpQty As String = "0"
    Private bCurrentOSSR_FLAG As Boolean = False
    Private bOSSR_Toggled As Boolean = False
    Public Property SequenceNumber() As Integer
        Get
            Return m_iSequence
        End Get
        Set(ByVal value As Integer)
            m_iSequence = value
        End Set
    End Property
    'ambli
    'For OSSR
    Private IsShopFloorCounts As LocationType = LocationType.StockRoom
    ''' <summary>
    ''' Property to store and retrive the current location when the connection is lost
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CurrentLocation() As LocationType
        Get
            Return IsShopFloorCounts
        End Get
        Set(ByVal value As LocationType)
            IsShopFloorCounts = value
        End Set
    End Property
#End If
    Public Property m_SelectedIndex() As Integer
        Get
            Return SelectedIndex
        End Get
        Set(ByVal value As Integer)
            SelectedIndex = value
        End Set
    End Property

    ''' <summary>
    '''  Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()

    End Sub
    ''' <summary>
    ''' Functions for getting the object instance for the ESSessionMgr. 
    ''' Use this method to get the object refernce for the Singleton ESSessionMgr.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of ESSessionMgr Class</remarks>
    Public Shared Function GetInstance() As EXSessionMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.EXCSSTCK
        If objEXSessionMgr Is Nothing Then
            objEXSessionMgr = New EXSessionMgr()
            Return objEXSessionMgr
        Else
            Return objEXSessionMgr
        End If
        BCReader.GetInstance().DisableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
    End Function
#If RF Then
    ''' <summary>
    ''' Updates the Status bar of all the forms in the session manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBarMessage()
        Try
            m_EXhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_EXItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_EXLocationSelection.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_EXSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_EXView.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception Occured, Trace : " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Returns whether the current state of application is Pre-Next or Post Next
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsPostNext() As Boolean
        Get
            If (Not m_EXItemList Is Nothing) AndAlso (m_EXItemList.Count > 0) Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public ReadOnly Property IsFirstItemActioned() As Boolean
        Get
            If (m_EXItemList.Count >= 1) Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
#End If
    ''' <summary>
    ''' Initialises the Excess Stock Session 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function StartSession() As Boolean
        ''Do all Excess Stock related Initialisations here.
        objAppContainer.objLogger.WriteAppLog("Enter EX Start session", Logger.LogLevel.INFO)
        Try
            'Fix Excess stock. 
            'Start the session only when proper response recieved for Start Record
#If RF Then
            If Not (objAppContainer.objExportDataManager.CreateGAS()) Then
                'Fix - Message Corrected
                objAppContainer.objLogger.WriteAppLog("Cannot Create GAS record at Excess Stock Start Session", _
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
            m_EXhome = New frmEXHome()
            m_EXItemDetails = New frmEXItemDetails()
            m_EXLocationSelection = New frmEXSelectLocation()
            m_EXSummary = New frmEXSummary()
            m_EXView = New frmView()
            m_EXItemList = New ArrayList()
            'Disabling the SEL scanning
            'BCReader.GetInstance().DisableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
            m_iScannedProdCount = 0

#If RF Then
            If objAppContainer.OSSRStoreFlag = "Y" Then
                Me.DisplayEXScreen(EXSCREENS.LocationSelection)
            Else
                Me.DisplayEXScreen(EXSCREENS.Home)
            End If

#ElseIf NRF Then
              Me.DisplayEXScreen(EXSCREENS.Home)
#End If
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at  EX Start Session: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit EX Start Session", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by ExSessionMgr.
    ''' </summary>
    ''' <remarks></remarks>
#If NRF Then
        Public Sub EndSession() 
#ElseIf RF Then
    Public Function EndSession(Optional ByVal isConnectivityLoss As Boolean = False) As Boolean
#End If
        objAppContainer.objLogger.WriteAppLog("Enter EX End Session", Logger.LogLevel.INFO)
        Try
            'Save and data and perform all Exit Operations.
            'Close and Dispose all forms.

#If NRF Then
            If m_EXItemList.Count > 0 Then
                WriteExportData()
                'Set active module to none after quitting the module
                objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.NONE
            Else
                objAppContainer.objLogger.WriteAppLog("No Export data to be written", Logger.LogLevel.INFO)
            End If

#ElseIf RF Then
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
                    'v1.1 Commenting this for TRANSACT restart issue in MCF
                    'Return False
                End If
            End If
#End If

            m_EXhome.Dispose()
            m_EXItemDetails.Dispose()
            m_EXLocationSelection.Dispose()
            m_EXSummary.Dispose()
            m_EXView.Dispose()
            'Enabling the SEL scanning
            BCReader.GetInstance().EnableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
            'Release the objects and Set to nothing.
            m_bItemScanned = Nothing
            objEXSessionMgr = Nothing
#If RF Then
            Return True
#End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at  EX End Session:" + ex.StackTrace, Logger.LogLevel.RELEASE)
#If RF Then
            Return False
#End If
        End Try
        'objAppContainer.objLogger.WriteAppLog("Exit EX End Session", Logger.LogLevel.INFO)
        objAppContainer.objLogger.WriteAppLog("Exit EX End Session", Logger.LogLevel.INFO)
#If NRF Then
    End Sub
#ElseIf RF Then
    End Function
#End If

    ''' <summary>
    ''' GenerateGAX record generates the GAX for the currently active session
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GenerateGAX() As GAXRecord
        'Sent GAX record for RF mode
        Dim objGAXRecord As GAXRecord
        Try
            objGAXRecord = New GAXRecord()
#If RF Then
            objGAXRecord.strPickListItems = m_iSequence
#ElseIf NRF Then
        objGAXRecord.strPickListItems = m_EXItemList.Count
#End If

            objGAXRecord.strPriceChk = "0"
            objGAXRecord.strSELS = "0"
            Return objGAXRecord
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
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
        objAppContainer.objLogger.WriteAppLog("Enter EX HandleScanData", Logger.LogLevel.INFO)
        'SFA - System Testing - DEF#167 -Item Count not updated with a multisite item entry - commented code
        'isNewItem = False
        m_bIsProdCodeScanned = False
#If RF Then
        strPreviousBkShpQty = "0"
        bOSSR_Toggled = False
#End If
        'm_EXProductInfo = New EXProductInfo()
        Dim arrPogList As ArrayList = New ArrayList
        Dim bValidcode As Boolean = False
        m_EXhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        Try

            Select Case Type
                Case BCType.EAN
                    If Not (objAppContainer.objHelper.ValidateEAN(strBarcode)) Or _
                       Val(strBarcode) = 0 Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        m_EXhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                    Else
                        strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                        ProcessBarcodeEntry(strBarcode, True)

                        m_EXhome.ProdSEL1.txtProduct.Text = strBarcode
                        bValidcode = True
                        m_bIsProdCodeScanned = True
                    End If
                Case BCType.ManualEntry
                    '*******
                    'Bug No:62:System Testing:Boots code entry
                    Dim strBootsCode As String = ""
                    If strBarcode.Length < 8 Then
                        'strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                        'TO be confirmed whether to consider Boots code entered at this point.
                        'strBarcode = strBarcode.Substring(0, 6)
                        'strBarcode = strBarcode.PadLeft(12, "0")
                        'Integration Testing
                        If objAppContainer.objHelper.ValidateBootsCode(strBarcode) Then
                            ProcessBarcodeEntry(strBarcode, False)
                            m_EXhome.ProdSEL1.txtProduct.Text = strBootsCode
                            bValidcode = True
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                            MessageBoxDefaultButton.Button1)
                            m_EXhome.ProdSEL1.txtProduct.Text = ""
                            m_EXhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            DisplayEXScreen(EXSCREENS.Home)
                            Return
                        End If
                    Else
                        If (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                            strBarcode = strBarcode.PadLeft(13, "0")
                            strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                            ProcessBarcodeEntry(strBarcode, True)
                            m_EXhome.ProdSEL1.txtProduct.Text = strBarcode
                            bValidcode = True
                            m_bIsProdCodeScanned = True
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                           MessageBoxButtons.OK, _
                           MessageBoxIcon.Asterisk, _
                           MessageBoxDefaultButton.Button1)
                            m_EXhome.ProdSEL1.txtProduct.Text = ""
                            m_EXhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            DisplayEXScreen(EXSCREENS.Home)
                            Return
                        End If
                    End If

                Case BCType.SEL
                    'System testing bug Fix for SEL Scan
                    If objAppContainer.objHelper.ValidateSEL(strBarcode) Then
                        Dim strBootsCode As String = ""
                        objAppContainer.objHelper.GetBootsCodeFromSEL(strBarcode, strBootsCode)
                        strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
                        ProcessBarcodeEntry(strBootsCode, False)
                        bValidcode = True
                    Else
                        'handle invalid SEL here
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M4"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        m_EXhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        DisplayEXScreen(EXSCREENS.Home)
                        Return
                    End If
            End Select
            
            '294 : Added an extra condition to take the control to Item details screen only if the
            'Product/Boots code is valid
            m_EXhome.ProdSEL1.txtProduct.Text = ""
            If strProductValid = "Y" Then
                If bValidcode Then
                    'Integration Testing - Flag to indicate that an item was scanned
                    m_bItemScanned = True
                    'Added as per SFA - Decides the screen(Site selection/Item detail)
                    If m_EXProductInfo.PendingSalesFlag = True And m_ItemScreen <> Macros.SCREEN_ITEM_CONFIRM Then

                        EXSessionMgr.GetInstance().DisplayEXScreen(EXSessionMgr.EXSCREENS.ItemDetails, Macros.SCREEN_SITE_SELECT)
                    Else
                        EXSessionMgr.GetInstance().DisplayEXScreen(EXSessionMgr.EXSCREENS.ItemDetails, Macros.SCREEN_ITEM_DETAIL, m_SelectedIndex)
                    End If
                End If
                m_EXhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at  EX HandleScanData:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        Finally
            If Not m_EXhome Is Nothing Then
                'Fix for PPC - SMl - out of rf range - item Key in - failed retries - system returns to Main Icons screen not Model Day icons
                If Not m_EXhome.ProdSEL1.txtSEL.IsDisposed AndAlso Not m_EXhome.ProdSEL1.txtProduct.IsDisposed Then
                    m_EXhome.ProdSEL1.txtSEL.Text = ""
                    m_EXhome.ProdSEL1.txtProduct.Text = ""
                End If
            End If
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit EX HandleScanData", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Method to process the entry of barcode
    ''' </summary>
    ''' <param name="strBarCode"></param>
    ''' <param name="bIsProductCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ProcessBarcodeEntry(ByVal strBarCode As String, ByVal bIsProductCode As Boolean) As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter EX ProcessBarcodeEntry", Logger.LogLevel.RELEASE)
        Dim strTempCode As String = "000000000000"
        Dim arrPogList As ArrayList = New ArrayList
        bIsAlreadyScanned = False
        'Checks if the barcode entered is product code or boots code
        'If the item is already scanned then take the data from the list
        'else take the data from database
        'Added as part of SFA - verify the code entered during confirmation
        Try
            If m_ItemScreen = Macros.SCREEN_ITEM_CONFIRM Then
                If bIsProductCode Then
                    If objAppContainer.objDataEngine.ValidateUsingPCAndBC(m_EXProductInfo.BootsCode, _
                                                                          strBarCode) Then
                        strProductValid = "Y"
                    Else
                        strProductValid = "N"
                    End If
                Else
                    If m_EXProductInfo.BootsCode.Equals(strBarCode) Then
                        strProductValid = "Y"
                    Else
                        strProductValid = "N"
                    End If
                End If
                If strProductValid = "N" Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M10"), "Info", _
                   MessageBoxButtons.OK, _
                   MessageBoxIcon.Asterisk, _
                   MessageBoxDefaultButton.Button1)
                    objAppContainer.objLogger.WriteAppLog("Invalid Barcode Entry", Logger.LogLevel.INFO)
                End If
            Else
                m_EXProductInfo = New EXProductInfo()
                strProductValid = "Y"
                If bIsProductCode Then
                    For Each objEXProductInfo As EXProductInfo In m_EXItemList
                        'If objEXProductInfo.ProductCode.Equals(strBarCode) Then
                        If (strBarCode = objEXProductInfo.ProductCode) Or (strBarCode = objEXProductInfo.FirstBarcode) Then
                            m_EXProductInfo = objEXProductInfo
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
                            If (strTempCode = objEXProductInfo.ProductCode) Or (strTempCode = objEXProductInfo.FirstBarcode) Then
                                m_EXProductInfo = objEXProductInfo
                                bIsAlreadyScanned = True

                                Exit For
                            End If
#End If
                        End If

                    Next
                    If Not bIsAlreadyScanned Then

                        If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarCode, m_EXProductInfo)) Then
#If NRF Then
                            'DARWIN checking if the base barcode is present in Database/Conroller
                            If strBarCode.StartsWith("2") Or strBarCode.StartsWith("02") Then
                                'DARWIN converting database to Base Barcode
                                strBarCode = objAppContainer.objHelper.GetBaseBarcode(strBarCode)
                                If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarCode, m_EXProductInfo)) Then
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                           MessageBoxButtons.OK, _
                                           MessageBoxIcon.Asterisk, _
                                           MessageBoxDefaultButton.Button1)
                                Else
                                    GetMultiSiteLocations(m_EXProductInfo.ProductCode, arrPogList)
                                    m_EXProductInfo.MultiSiteCount = arrPogList.Count
                                End If
                            Else
#End If
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                        MessageBoxButtons.OK, _
                                        MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
#If NRF Then
                            End If
#End If
                        Else
                            isNewItem = True
                            IsQtyUpdated = False
                            m_EXProductInfo.Sequence = m_iSequence + 1
                            GetMultiSiteLocations(m_EXProductInfo.ProductCode, arrPogList)
                            m_EXProductInfo.MultiSiteCount = arrPogList.Count
                        End If

                    End If
                Else
                    For Each objEXProductInfo As EXProductInfo In m_EXItemList
                        If objEXProductInfo.BootsCode.Equals(strBarCode) Then
                            m_EXProductInfo = objEXProductInfo
                            bIsAlreadyScanned = True
#If RF Then
                        bOSSR_Toggled = True
#End If
                            Exit For
                        End If
                    Next
                    If Not bIsAlreadyScanned Then
                        If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBarCode, m_EXProductInfo)) Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                        Else
                            isNewItem = True
                            IsQtyUpdated = False
                            m_EXProductInfo.Sequence = m_iSequence + 1
                            GetMultiSiteLocations(m_EXProductInfo.ProductCode, arrPogList)
                            m_EXProductInfo.MultiSiteCount = arrPogList.Count
                        End If
                    End If
                End If
            End If
            Cursor.Current = Cursors.Default
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured at  EX ProcessBarcodeEntry:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit EX ProcessBarcodeEntry", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' The method Displays the Excess Stock Home Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayEXScan(ByVal o As Object, ByVal e As EventArgs)
        With m_EXhome
            'ambli
            'For OSSR
#If RF Then
            If IsShopFloorCounts = LocationType.StockRoom Then
                .lblBackshop.Text = "Back Shop"
            ElseIf IsShopFloorCounts = LocationType.OSSRSite Then
                .lblBackshop.Text = "Off Site"
            End If
#End If
            If m_EXItemList.Count = 0 Then
                m_EXhome.btnView.Visible = False
            Else
                m_EXhome.btnView.Visible = True
            End If
            'System testing
#If NRF Then
      .lblItemsText.Text = m_EXItemList.Count
#ElseIf RF Then
            .lblItemsText.Text = SequenceNumber

#End If
            'Refresh variables
            m_ItemScreen = Nothing
            m_IsCountPend = Nothing
            m_IndexToSelect = Nothing

            'Sets the store id and active data time to the status bar
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

            .Visible = True
            .Refresh()
        End With
    End Sub
#If RF Then
    Private Sub DisplayEXLocationSelection(ByVal o As Object, ByVal e As EventArgs)
        Try
            With m_EXLocationSelection
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                .Visible = True
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at  EX  DisplayEXLocationSelection:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit EX  DisplayEXLocationSelection", Logger.LogLevel.RELEASE)
    End Sub

    Public Sub DisplayOSSRToggle(ByRef lblToggleOSSR As Label, ByVal strBarcode As String)
        'Dim objENQ As ENQRecord = New ENQRecord()
        bOSSR_Toggled = True
        Dim bResponse As Boolean = False

        If lblToggleOSSR.Text = " " Then
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
    ''' Displays the Excess Stock Item Details Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayEXItemDetails(ByVal o As Object, ByVal e As EventArgs)
        Try
            With m_EXItemDetails
                Dim strBarcode As String = ""
#If RF Then
                strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(m_EXProductInfo.ProductCode)
                'System testing - DEF:54 - Added specific text to RF mode
                .lblSODStockFile.Text = "Total Stock File:"
#ElseIf NRF Then
                strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(m_EXProductInfo.SecondBarcode)
#End If

                If m_EXItemList.Count > 0 Then
                    .btnView.Visible = True
                Else
                    .btnView.Visible = False
                End If

                Dim objDescriptionArray As ArrayList = New ArrayList()
                objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(m_EXProductInfo.Description)
                .lblBootsCode.Text = objAppContainer.objHelper.FormatBarcode(m_EXProductInfo.BootsCode)
                .lblProductCode.Text = objAppContainer.objHelper.FormatBarcode(strBarcode)
                .lblProdDescription1.Text = objDescriptionArray.Item(0)
                .lblProdDescription2.Text = objDescriptionArray.Item(1)
                .lblProdDescription3.Text = objDescriptionArray.Item(2)
                .lblStatusText.Text = objAppContainer.objHelper.GetStatusDescription(m_EXProductInfo.Status)
                .lblStockText.Text = m_EXProductInfo.TSF
                .lblTotalItemText.Text = m_EXProductInfo.BackShopQty
                .lblBckShpQty.Text = "Enter Quantity on Shelf:"

                    'To highlight stock figure in red colour for a negative value.
                    If m_EXProductInfo.TSF.Substring(0, 1).Equals("-") Then
                        .lblStockText.ForeColor = Color.Red
                    Else
                        .lblStockText.ForeColor = .lblStatusText.ForeColor
                    End If
                    .Btn_CalcPad_small1.Visible = True
                    .lblBckShpQty.Visible = True
                    .lblBackQtyText.Visible = True
                    .Btn_CalcPad_small2.Visible = False

                    'Display controls according to the screen - Added as per SFA
                    If m_EXProductInfo.PendingSalesFlag Then
                        .lblSite.Visible = True
                        .cmbSite.Visible = True
                        PopulateSites()
                        .cmbSite.SelectedIndex = m_SelectedIndex
                    Else
                        .lblSite.Visible = False
                        .cmbSite.Visible = False
                        .cmbSite.Items.Clear()
                    End If
                    If m_ItemScreen = Macros.SCREEN_SITE_SELECT Then
                        'BCReader.GetInstance().StopRead()
                        If m_SelectedIndex <> Macros.SELECT_MBS Then
                            .lblBckShpQty.Visible = False
                            .lblBackQtyText.Visible = False
                            .Btn_CalcPad_small1.Visible = False
                        BCReader.GetInstance().StopRead()  'SFA  Testing
                        End If
                    ElseIf m_ItemScreen = Macros.SCREEN_ITEM_CONFIRM Then
                        .lblBckShpQty.Text = "Scan/Key to Confirm Item"
                        .Btn_CalcPad_small2.Visible = True
                        .Btn_CalcPad_small1.Visible = False
                        .lblBackQtyText.Visible = False
                        BCReader.GetInstance().StartRead()
                    ElseIf m_ItemScreen = Macros.SCREEN_ITEM_DETAIL Then
                        .Btn_CalcPad_small2.Visible = False
                    BCReader.GetInstance().StopRead()   'SFA  Testing
                    End If
                    'ambli
                    'For OSSR
#If RF Then

                    If objAppContainer.OSSRStoreFlag = "Y" Then
                        If Not bOSSR_Toggled Then
                            If m_EXProductInfo.OSSRFlag = "O" Then
                                .lblOSSR.Text = "OSSR"
                            Else
                                .lblOSSR.Text = " "
                            End If
                        End If
                    Else
                        .Btn_OSSRItem1.Visible = False
                        .lblOSSR.Visible = False
                    End If

#ElseIf NRF Then
                .Btn_OSSRItem1.Visible = False
                .lblOSSR.Visible = False
#End If

                    '#If RF Then

                    '                If IsShopFloorCounts = LocationType.StockRoom Then
                    '                    .lblBckShpQty.Text = "Enter Quantity on Shelf:"
                    '                Else
                    '                    .lblBckShpQty.Text = "Enter Off Site Qty:"
                    '                End If
                    '#End If
                    If .cmbSite.SelectedIndex = Macros.SELECT_PSP Then
                        .lblBackQtyText.Text = m_EXProductInfo.PSPCount
                    Else
                        .lblBackQtyText.Text = m_EXProductInfo.MBSCount
                    End If

                    'Sets the store id and active data time to the status bar
                    .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                    .Visible = True
                    .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at  EX DisplayEXItemDetails:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit EX DisplayEXItemDetails", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' To Intialize the View Screen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub IntialiseViewScreen()
        objAppContainer.objLogger.WriteAppLog("Enter EX InitialiseViewScreen", Logger.LogLevel.RELEASE)
        'Added as part of SFA - added 2 more columns
        With m_EXView
            .lblHeading.Text = "View Excess List"
            .lblHeading.Location = New Point(3 / 2 * objAppContainer.iOffSet, 20 / 2 * objAppContainer.iOffSet)
            .lblHeading.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
            .lstView.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Regular)
            .lstView.Columns.Add("Item", 46 * objAppContainer.iOffSet, HorizontalAlignment.Center)
            .lstView.Columns.Add("Description", 123 * objAppContainer.iOffSet, HorizontalAlignment.Left)
            .lstView.Columns.Add("MBS", 32 * objAppContainer.iOffSet, HorizontalAlignment.Center)
            .lstView.Columns.Add("PSP", 31 * objAppContainer.iOffSet, HorizontalAlignment.Center)
        End With
        objAppContainer.objLogger.WriteAppLog("Exit EX InitialiseViewScreen", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Displays the Excess Stock View Screen details
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayEXItemView(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Enter EX DisplayEXItemView", Logger.LogLevel.RELEASE)
        Try
            With m_EXView
                .Text = "Excess Stock - BS"
                .lblBottomText.Visible = True
                .Help1.Location = New Point(400 / 2 * objAppContainer.iOffSet, 15 / 2 * objAppContainer.iOffSet)
                .lstView.Clear()
                IntialiseViewScreen()
                PopulateEXViewList()
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                .Visible = True
                .Refresh()
            End With
            m_bItemScanned = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at  EX DisplayEXItemView:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit EX DisplayEXItemView", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Displays the Excess Stock Summary Screen details
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayEXSummary(ByVal o As Object, ByVal e As EventArgs)
        With m_EXSummary
#If RF Then
            .lblPickListText.Text = SequenceNumber.ToString()
#ElseIf NRF Then
                        .lblPickListText.Text = m_EXItemList.Count
#End If
            'Sets the store id and active data time to the status bar
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            'ambli
#If NRF Then
            If m_EXItemList.Count > 0 Then
                .lblUserMsg.Visible = True
                .lblDockTransmit.Visible = True
            Else
                .lblUserMsg.Visible = False
                .lblDockTransmit.Visible = False
            End If
#ElseIf RF Then
             If m_EXItemList.Count > 0 
            .lblUserMsg.Visible = True
            Else
            .lblUserMsg.Visible = False
            End If
            .lblDockTransmit.Visible = False
#End If
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
    '''  Function for Updating Product Info
    ''' </summary>
    ''' <remarks></remarks>
    Public Function UpdateProductInfo(ByVal IsCountUpdate As Boolean) As Boolean
        Try
#If RF Then
            strPreviousBkShpQty = m_EXProductInfo.BackShopQty
#End If
            objAppContainer.objLogger.WriteAppLog("Enter EX UpdateProductInfo", Logger.LogLevel.RELEASE)
            Dim iCount As Integer
            Dim strQty As String = Nothing
            Dim iMessageFlag As Boolean = False

            strQty = m_EXItemDetails.lblBackQtyText.Text

            If strQty.Trim().Equals("") Then
                Return False
            End If
            'Added as per SFA - Displays message on Next with quantity as zero
            If Not IsCountUpdate Then

                'If Not IsQtyUpdated Then

                'If m_EXProductInfo.PendingSalesFlag Then
                '    If m_EXItemDetails.cmbSite.SelectedItem = Macros.MAIN_BACK_SHOP Then
                '        If m_EXProductInfo.IsMBSCounted = "Y" Then
                '            iMessageFlag = True
                '        Else
                '            iMessageFlag = False
                '        End If
                '    ElseIf m_EXItemDetails.cmbSite.SelectedItem = Macros.PEND_SALES_PLAN Then
                '        If m_EXProductInfo.IsPSPCounted = "Y" Then
                '            iMessageFlag = True
                '        Else
                '            iMessageFlag = False
                '        End If
                '    End If
                'Else
                '    If m_EXProductInfo.IsMBSCounted = "Y" Then
                '        iMessageFlag = True
                '    Else
                '        iMessageFlag = False
                '    End If
                'End If

                'If Not iMessageFlag Then
                'If Not IsQtyUpdated Then
                If Not objAppContainer.objHelper.ValidateZeroQty(CInt(strQty)) Then
                    If m_bIsProdCodeScanned Then
                        'System Testing Commented Message and added new
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M65"))
                        Return False
                    Else
                        If Not IsQtyUpdated Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M102"))
                        End If
                    End If
                End If
                'End If
                'End If
            Else
                If Not objAppContainer.objHelper.ValidateZeroQty(CInt(strQty)) Then
                    If m_bIsProdCodeScanned Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M51"))
                        Return False
                    End If
                End If
            End If
                'Added as per SFA - Updates site's count status
                If IsCountUpdate Or strQty.Equals("0") Then
                    If m_EXProductInfo.PendingSalesFlag Then
                    If m_EXItemDetails.cmbSite.SelectedIndex = Macros.SELECT_MBS Then
                        m_EXProductInfo.IsMBSCounted = "Y"
                        m_EXProductInfo.MBSCount = strQty
                    ElseIf m_EXItemDetails.cmbSite.SelectedIndex = Macros.SELECT_PSP Then
                        m_EXProductInfo.IsPSPCounted = "Y"
                        m_EXProductInfo.PSPCount = strQty
                    End If

                        If m_EXProductInfo.IsMBSCounted = "N" Or m_EXProductInfo.IsPSPCounted = "N" Then
                            m_IsCountPend = True
                        Else
                            m_IsCountPend = False
                        End If
                    Else
                        m_EXProductInfo.IsMBSCounted = "Y"
                        m_EXProductInfo.IsPSPCounted = "N/A"
                        m_EXProductInfo.MBSCount = strQty
                    End If
                End If
                m_EXProductInfo.BackShopQty = m_EXProductInfo.MBSCount + m_EXProductInfo.PSPCount

                Dim bIsPresentInList As Boolean = False
                For iCount = 0 To m_EXItemList.Count - 1
                    Dim objEX As EXProductInfo = New EXProductInfo
                    objEX = m_EXItemList.Item(iCount)
                    If objEX.BootsCode.Equals(m_EXProductInfo.BootsCode) Then
                        m_EXItemList.RemoveAt(iCount)
                        m_EXItemList.Insert(iCount, m_EXProductInfo)
                        bIsPresentInList = True
                        Exit For
                    End If
                Next
#If RF Then
            'RFSTAB
            'Add code to sent GAP message here.
            Dim objGapRecord As GAPRecord = New GAPRecord()

            objGapRecord.strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(m_EXProductInfo.ProductCode)
            objGapRecord.strBootscode = m_EXProductInfo.BootsCode
            objGapRecord.strCurrentQty = m_EXProductInfo.BackShopQty
            objGapRecord.strNumberSEQ = m_EXProductInfo.Sequence
            ' Change to update Seq Number from GAR message
            'Setting the default values as zero
            objGapRecord.strFillQty = "0"
            objGapRecord.strStockFig = m_EXProductInfo.TSF
            'After the new message format.
            objGapRecord.strUpdateOssrItem = " "
            objGapRecord.strLocCounted = "  "

            'Added for Location change to OSSR
            If IsShopFloorCounts = LocationType.OSSRSite Then
                objGapRecord.cIsGAPFlag = Macros.PLC_OSSR_FLAG
            ElseIf IsShopFloorCounts = LocationType.StockRoom Then
                objGapRecord.cIsGAPFlag = Macros.PLC_EX_FLAG
            End If
            If objAppContainer.objExportDataManager.CreateGAP(objGapRecord) Then
                If isNewItem Then
                    m_iSequence = m_iSequence + 1
                    isNewItem = False
                End If
            End If
#End If
                If Not bIsPresentInList Then
                    m_EXItemList.Add(m_EXProductInfo)
                End If
                'System Testing - Set the scan count
                'Only if the item is scanned the Scanned count should increase
                If m_bItemScanned Then
                    m_iScannedProdCount = m_iScannedProdCount + 1
                    m_bItemScanned = False
                End If

                'Reset product code scanned boolean to false as we are ready to scan next item.
                m_bIsProdCodeScanned = False

                Return True

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at  EX UpdateProductInfo:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit EX UpdateProductInfo", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Subroutine for Populating List View
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateEXViewList()
        Try
            'Added as per SFA - Added 2 more fields to the list to display the count status of item in sites
            With m_EXView
                For Each objItemInfo As EXProductInfo In m_EXItemList
                    .lstView.Items.Add( _
                        (New ListViewItem(New String() {objItemInfo.BootsCode, _
                                                        LCase(objItemInfo.ShortDescription), _
                                                        objItemInfo.IsMBSCounted, _
                                                        objItemInfo.IsPSPCounted})))
                Next
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at  EX PopulateEXViewList:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit EX PopulateEXViewList", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Screen Display method for Excess Stock. 
    ''' All Excess Stock sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName"></param>
    ''' <param name="strScreen"></param>
    ''' <returns></returns>
    ''' <remarks>Modified for SFA to include 2 optional parameters</remarks>
    Public Function DisplayEXScreen(ByVal ScreenName As EXSCREENS, Optional ByVal strScreen As String = "", Optional ByVal iSelectedIndex As Integer = 0)
        objAppContainer.objLogger.WriteAppLog("Enter EX Display Screen", Logger.LogLevel.INFO)
        Try
            Select Case ScreenName
#If RF Then
                Case EXSCREENS.LocationSelection
                    m_EXLocationSelection.Invoke(New EventHandler(AddressOf DisplayEXLocationSelection))
#End If
                Case EXSCREENS.Home
                    PreviousScreen = EXSCREENS.Home
                    m_EXhome.Invoke(New EventHandler(AddressOf DisplayEXScan))
                Case EXSCREENS.ItemDetails
                    PreviousScreen = EXSCREENS.ItemDetails
                    m_ItemScreen = strScreen
                    m_SelectedIndex = iSelectedIndex
                    m_EXItemDetails.Invoke(New EventHandler(AddressOf DisplayEXItemDetails))
                Case EXSCREENS.ItemView
                    If m_EXItemList.Count = 0 Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M19"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                    Else
                        m_EXSummary.Invoke(New EventHandler(AddressOf DisplayEXItemView))
                    End If
                Case EXSCREENS.EXSummary
                    m_EXSummary.Invoke(New EventHandler(AddressOf DisplayEXSummary))

            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at Excess Stock Display Screen" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit EX Display Screen", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Populates the Site combobox - Added as part of SFA
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PopulateSites()
        Try
            m_EXItemDetails.cmbSite.Items.Clear()
            'SFA - ST - Added * counted
            If m_EXProductInfo.IsMBSCounted = "Y" Then
                m_EXItemDetails.cmbSite.Items.Add("* " + Macros.MAIN_BACK_SHOP + " (Counted)")
            Else
                m_EXItemDetails.cmbSite.Items.Add(Macros.MAIN_BACK_SHOP)
            End If
            If m_EXProductInfo.IsPSPCounted = "Y" Then
                m_EXItemDetails.cmbSite.Items.Add("* " + Macros.PEND_SALES_PLAN + " (Counted)")
            Else
                m_EXItemDetails.cmbSite.Items.Add(Macros.PEND_SALES_PLAN)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception in EXSessionMgr::" + ex.StackTrace(), Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit EXSessionMgr PopulateSites", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Processes the Item selection in View Screen to display item details
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessItemSelection() As Boolean
        Dim bIsDataAvailable As Boolean = False
        Dim strBootsCode As String = ""
        Dim strProductCode As String = ""
        'Obtains the boots code corresponding to the selected data
        Try
            With m_EXView
                Dim iCounter As Integer = 0
                If .lstView.SelectedIndices.Count > 0 Then
                    For iCounter = 0 To .lstView.Items.Count - 1
                        If .lstView.Items(iCounter).Selected Then
                            bIsDataAvailable = True
                            m_EXProductInfo = m_EXItemList.Item(iCounter)
                            Exit For
                        End If
                    Next
                End If
            End With
            'If data is available then obtains the Product code from the list
            If bIsDataAvailable Then
                If m_EXProductInfo.IsPSPCounted.Equals("N") Then
                    m_IndexToSelect = Macros.SELECT_PSP
                Else
                    m_IndexToSelect = Macros.SELECT_MBS
                End If
                DisplayEXScreen(EXSCREENS.ItemDetails, Macros.SCREEN_ITEM_CONFIRM, m_IndexToSelect)
            End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at Excess Stock ProcessItemSelection" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit EX ProcessItemSelection", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessItemDetailsQuit()
        'Integration Testing
        DisplayEXScreen(EXSCREENS.Home)
    End Sub
    ''' <summary>
    ''' Processes the quit selection on the EX Home screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessExcessStockQuit()
        Dim iResult As Integer = 0
        Dim bIsMSBCountPend As Boolean
        Dim bIsPSPCountPend As Boolean
        'Added as per SFA - Checks whether count pending in PSP/MBS
        For Each objItemInfo As EXProductInfo In m_EXItemList
            If objItemInfo.IsMBSCounted = "N" Then
                bIsMSBCountPend = True
                Exit For
            ElseIf objItemInfo.IsPSPCounted = "N" Then
                bIsPSPCountPend = True
            End If
        Next
        'Added as per SFA - Displays Quit messages according to the count status in PSP/MBS
        If bIsMSBCountPend Then
            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M98"), "Alert", MessageBoxButtons.OK, _
                                      MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)

        ElseIf bIsPSPCountPend Then
            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M97"), "Confirmation", MessageBoxButtons.YesNo, _
                                  MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        Else
            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M39"), "Confirmation", MessageBoxButtons.YesNo, _
                                          MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        End If

        If iResult = MsgBoxResult.Yes Then
            EXSessionMgr.GetInstance().DisplayEXScreen(EXSessionMgr.EXSCREENS.EXSummary)
        ElseIf iResult = MsgBoxResult.Ok Or iResult = MsgBoxResult.No Then
            EXSessionMgr.GetInstance().DisplayEXScreen(EXSessionMgr.EXSCREENS.Home)
        End If

    End Sub
    ''' <summary>
    ''' Sets the View form to visible state
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetViewVisible()
        m_EXView.Visible = True
    End Sub
    Public Sub LocationChosen(ByVal eLoc As LocationType)
#If RF Then
        Me.IsShopFloorCounts = eLoc
#End If
        Me.DisplayEXScreen(EXSCREENS.Home)
    End Sub
    Public Enum LocationType
        None
        StockRoom
        OSSRSite
    End Enum
    ''' <summary>
    '''  Writes the final set of data identified to the export data file
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteExportData() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter EX WriteExportData", Logger.LogLevel.RELEASE)
        Try
            Dim objGapRecord As GAPRecord = New GAPRecord()
            Dim objGAPCount As Integer = 0
            Dim objGAXRecord As GAXRecord = New GAXRecord()
            m_EXSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing GAS")
            objAppContainer.objExportDataManager.CreateGAS()
            m_EXSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing GAP")
            For Each objEXProductInfo As EXProductInfo In m_EXItemList
                objGapRecord.strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(objEXProductInfo.ProductCode)
                objGapRecord.strBootscode = objEXProductInfo.BootsCode
                objGapRecord.strCurrentQty = objEXProductInfo.BackShopQty
                objGapRecord.strNumberSEQ = objGAPCount + 1
                'Setting the default values as zero
                objGapRecord.strFillQty = "0"
                objGapRecord.strStockFig = "0"
                'After the new message format.
                objGapRecord.strUpdateOssrItem = " "
                objGapRecord.strLocCounted = "  "
                objGapRecord.cIsGAPFlag = Macros.PLC_EX_FLAG
                objAppContainer.objExportDataManager.CreateGAP(objGapRecord)
                'System Testing - increment the sequence number
                objGAPCount += 1
            Next
            m_EXSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing GAX")
            objGAXRecord.strPickListItems = m_EXItemList.Count
            objGAXRecord.strPriceChk = "0"
            objGAXRecord.strSELS = "0"
            objAppContainer.objExportDataManager.CreateGAX(objGAXRecord)

            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at WriteExportData in Excess Stock: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit EX WriteExportData", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Enumerates all scannable fields in Excess stock Module
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum SCNFIELDS
        ProductCode
    End Enum
    ''' <summary>
    ''' Enum Class that defines all screens for Excess stock module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum EXSCREENS
        LocationSelection
        Home
        ItemDetails
        ItemView
        EXSummary
    End Enum
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
''' <summary>
''' The value class for getting and managing excess stock.
''' </summary>
''' <remarks></remarks>
Public Class EXProductInfo
    Inherits ProductInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_backShopQty As Integer
    Private m_multiSiteCount As Integer
    Private m_FirstBarcode As String
    'Fix for Fetching Second barcode and displaying in Item Deatils Screen
    Private m_SecondBarcode As String
    'Added member variables as part of SFA
    Private m_isPSPCounted As String = "N"
    Private m_isMBSCounted As String = "N"
    Private m_PSPCount As Integer
    Private m_MBSCount As Integer

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub

    ''' <summary>
    ''' Gets or sets backshop quantity for a product
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BackShopQty() As Integer
        Get
            Return m_backShopQty
        End Get
        Set(ByVal value As Integer)
            m_backShopQty = value
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
    Public Property SecondBarcode() As String
        Get
            Return m_SecondBarcode
        End Get
        Set(ByVal value As String)
            m_SecondBarcode = value
        End Set
    End Property
    '''' <summary>
    '''' Gets or sets multisitecount for a product
    '''' </summary>
    '''' <value></value>
    '''' <returns></returns>
    '''' <remarks></remarks>
    Public Property MultiSiteCount() As Integer
        Get
            Return m_multiSiteCount
        End Get
        Set(ByVal value As Integer)
            m_multiSiteCount = value
        End Set
    End Property
    '''' <summary>
    '''' Gets or sets the flag which denotes the count status of item in MBS
    '''' Added as part of SFA
    '''' </summary>
    '''' <value></value>
    '''' <returns></returns>
    '''' <remarks></remarks>
    Public Property IsMBSCounted() As String
        Get
            Return m_isMBSCounted
        End Get
        Set(ByVal value As String)
            m_isMBSCounted = value
        End Set
    End Property
    '''' <summary>
    '''' Gets or sets the flag which denotes the count status of item in PSP 
    '''' Added as part of SFA
    '''' </summary>
    '''' <value></value>
    '''' <returns></returns>
    '''' <remarks></remarks>
    Public Property IsPSPCounted() As String
        Get
            Return m_isPSPCounted
        End Get
        Set(ByVal value As String)
            m_isPSPCounted = value
        End Set
    End Property
    '''' <summary>
    '''' Gets or sets the item  count in PSP 
    '''' Added as part of SFA
    '''' </summary>
    '''' <value></value>
    '''' <returns></returns>
    '''' <remarks></remarks>
    Public Property PSPCount() As Integer
        Get
            Return m_PSPCount
        End Get
        Set(ByVal value As Integer)
            m_PSPCount = value
        End Set
    End Property
    '''' <summary>
    '''' Gets or sets the item  count in MBS 
    '''' Added as part of SFA
    '''' </summary>
    '''' <value></value>
    '''' <returns></returns>
    '''' <remarks></remarks>
    Public Property MBSCount() As Integer
        Get
            Return m_MBSCount
        End Get
        Set(ByVal value As Integer)
            m_MBSCount = value
        End Set
    End Property
End Class