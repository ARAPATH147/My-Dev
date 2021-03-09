Imports System.Runtime.InteropServices
Imports Microsoft.Win32
'''***************************************************************
''' <FileName>AutoSYSSessionMgr.vb</FileName>
''' <summary>
''' The Auto Stuff Your Shelves Container Class.
''' Implements all business logic and GUI navigation for Auto Stuff Your Shelves.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>01-June-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
Public Class AutoSYSSessionManager
    Private Shared m_AuotSYSSessionManager As AutoSYSSessionManager = Nothing
    Private m_AutoSYSHome As frmAutoSYSHomeScreen
    Private m_AutoSYSItemDetails As frmAutoSYSItemDetails
    Private m_AutoSYSView As frmView
    Private m_AutoSYSSummary As frmAutoSYSSummary
    Private m_ASYSItemList As ArrayList = Nothing
    Private m_ASYSItemsPicked As ArrayList = Nothing
    Private m_ASYSProductInfo As ASYSProductInfo = Nothing
    Private m_iIndex As Integer
    ''' <summary>
    ''' Functions for getting the object instance for the AutoSYSSessionManager.
    ''' </summary>
    ''' <returns>Object reference of AutoSYSSessionManager Class</returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As AutoSYSSessionManager
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.AUTOSTUFFYOURSHELVES
        If m_AuotSYSSessionManager Is Nothing Then
            m_AuotSYSSessionManager = New AutoSYSSessionManager()
            Return m_AuotSYSSessionManager
        Else
            Return m_AuotSYSSessionManager
        End If
    End Function
    ''' <summary>
    ''' Initialises the Auto Stuff Your Shelves Session
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartSession()
#If RF Then
        'Create SYS start is to check whether there was a conection loss in previous session
        'And try to establish a connection before proceeding
        If objAppContainer.objExportDataManager.CreateSYSStart() Then
#End If
            m_AutoSYSHome = New frmAutoSYSHomeScreen
            m_AutoSYSItemDetails = New frmAutoSYSItemDetails
            m_AutoSYSView = New frmView
            m_AutoSYSSummary = New frmAutoSYSSummary
            m_iIndex = 0
            m_ASYSItemList = New ArrayList()
            m_ASYSItemsPicked = New ArrayList()
            m_ASYSProductInfo = New ASYSProductInfo()
            m_AutoSYSHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            Me.DisplayASYSScreen(ASYSSCREENS.Home)
#If RF Then
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
            m_AutoSYSHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_AutoSYSItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_AutoSYSView.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_AutoSYSSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured, Trace: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End If
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by Auto Stuff Your Shelves Session
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EndSession()
        Try
            m_AutoSYSHome.Dispose()
            m_AutoSYSItemDetails.Dispose()
            m_AutoSYSView.Dispose()
            m_AutoSYSSummary.Dispose()
            m_ASYSItemList = Nothing
            m_ASYSItemsPicked = Nothing
            m_ASYSProductInfo = Nothing
            m_AuotSYSSessionManager = Nothing
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at AYS End Session " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Quits the Session and displays the summary screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub QuitSession()
        If (MessageBox.Show("Are you sure you wish to quit?", _
                            "Confirmation", _
                            MessageBoxButtons.YesNo, _
                            MessageBoxIcon.Question, _
                            MessageBoxDefaultButton.Button1) = (MsgBoxResult.Yes)) Then
            DisplayASYSScreen(ASYSSCREENS.Summary)
        End If
    End Sub

    ''' <summary>
    '''  Screen Display method for Auto Stuff Your Shelves. 
    ''' All Auto Stuff Your Shelves sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DisplayASYSScreen(ByVal ScreenName As ASYSSCREENS)
        ' objAppContainer.objLogger.WriteAppLog("Enter ASYS Display Screen", Logger.LogLevel.INFO)
        Try
            Select Case ScreenName
                Case ASYSSCREENS.Home
                    m_AutoSYSHome.Invoke(New EventHandler(AddressOf DisplayASYSHomeScreen))
                Case ASYSSCREENS.ItemDetails
                    m_AutoSYSItemDetails.Invoke(New EventHandler(AddressOf DisplayASYSItemDetails))
                Case ASYSSCREENS.View
                    m_AutoSYSView.Invoke(New EventHandler(AddressOf DisplayASYSPickList))
                Case ASYSSCREENS.Summary
                    m_AutoSYSSummary.Invoke(New EventHandler(AddressOf DisplayASYSSummary))
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at Auto Stuff Your Shelves Display Screen" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        '  objAppContainer.objLogger.WriteAppLog("Exit ASYS Display Screen", Logger.LogLevel.INFO)
        Return True
    End Function

    ''' <summary>
    ''' Displays Auto Stuff Your Shelves Home screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayASYSHomeScreen(ByVal o As Object, ByVal e As EventArgs)
        Try
            With m_AutoSYSHome
                'Sets the store id and active data time to the status bar
                '.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                '                'Added for RF Mode -Lakshmi
                '#If NRF Then
                '                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                '#ElseIf RF Then
                '                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                '#End If
                'Ends
                .objProdSEL.txtProduct.Text = ""
                .objProdSEL.txtSEL.Text = ""
                .lblItemsScanned.Text = m_ASYSItemList.Count

                .Visible = True
                .Refresh()
                .Activate()
            End With

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at ASYS Home screen:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Displays Auto Stuff Your Shelves Item Details screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayASYSItemDetails(ByVal o As Object, ByVal e As EventArgs)
        Try
            Dim strBarcode As String = ""
            strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(m_ASYSProductInfo.ProductCode)
            Dim objDescriptionArray As ArrayList = New ArrayList()
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(m_ASYSProductInfo.Description)
            With m_AutoSYSItemDetails
                'Sets the store id and active data time to the status bar
                'Added for RF Mode -Lakshmi

                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)


                'Ends
                .lblBootsCode.Text = objAppContainer.objHelper.FormatBarcode(m_ASYSProductInfo.BootsCode)
                .lblProductCode.Text = objAppContainer.objHelper.FormatBarcode(strBarcode)
                .lblDesc1.Text = objDescriptionArray.Item(0)
                .lblDesc2.Text = objDescriptionArray.Item(1)
                .lblDesc3.Text = objDescriptionArray.Item(2)
                .lblStatus.Text = objAppContainer.objHelper.GetStatusDescription(m_ASYSProductInfo.Status)
                .lblPSC.Text = m_ASYSProductInfo.PSC
                'Stock File Accuracy - added TSF label
                If m_ASYSProductInfo.TSF.Substring(0, 1).Equals("-") Then
                    .lblTSF.ForeColor = Color.Red
                Else
                    .lblTSF.ForeColor = .lblStatus.ForeColor
                End If
                'End
                .lblTSF.Text = m_ASYSProductInfo.TSF
                .lblMsg1.Text = "Take up to " + m_ASYSProductInfo.PSC + " to the sales floor"
                If m_ASYSItemsPicked.Count > 0 Then
                    .BtnView1.Visible = True
                Else
                    .BtnView1.Visible = False
                End If
#If RF Then
                'System testing - DEF:54 - Added specific text to RF mode
                .lblstockFig.Text = "Total Stock File:"
                '.lblstockFig.Text = "Stock Figure"
#End If
                'If m_iIndex > 0 Then
                '    .Btn_Previous31.Visible = True
                'Else
                '    .Btn_Previous31.Visible = False
                'End If
                'If m_iIndex = m_ASYSItemsPicked.Count - 1 Then

                '    .Btn_Next_small1.Visible = False
                'Else
                '    .Btn_Next_small1.Visible = True
                'End If
                .Visible = True
                .Refresh()
                .Activate()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at ASYSItemDetails:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Displays Auto Stuff Your Shelves Summary screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayASYSSummary(ByVal o As Object, ByVal e As EventArgs)
        Try
            With m_AutoSYSSummary
                'Sets the store id and active data time to the status bar
                'Added for RF Mode -Lakshmi
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                'Ends
                .lblScannedItems.Text = m_ASYSItemList.Count
                .lblItemsPicked.Text = m_ASYSItemsPicked.Count
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at ASYSSummary:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>

    Private Sub DisplayASYSPickList(ByVal o As Object, ByVal e As EventArgs)
        Try
            With m_AutoSYSView
                .objStatusBar.SetMessage("Select item for information")
                .lstView.Clear()
                .Text = "Stuff Your Shelves"
                .lblHeading.Text = "View Stuff Your Shelves List"
                .lstView.Columns.Add("Item", 70 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                .lstView.Columns.Add("Description", 122 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                .lstView.Columns.Add("MS", 33 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                For Each objItemInfo As ASYSProductInfo In m_ASYSItemsPicked
                    .lstView.Items.Add( _
                       (New ListViewItem(New String() {objItemInfo.BootsCode, _
                                                       objItemInfo.ShortDescription.Trim(), _
                                                     objItemInfo.MutiSite})))


                Next
                '    'Sets the store id and active data time to the status bar
                '    .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                '    .lblScannedItems.Text = m_ASYSItemList.Count
                '    .lblItemsPicked.Text = m_ASYSItemsPicked.Count
                .Help1.Visible = False
                .lblTot.Visible = True
                .lblTotal.Visible = True
                .lblTotal.Text = m_ASYSItemsPicked.Count.ToString()
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at ASYSVIEW:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    ''' <summary>
    ''' For making beep sound
    ''' </summary>
    ''' <param name="szSound"></param>
    ''' <param name="hModule"></param>
    ''' <param name="flags"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    <DllImport("coredll.dll")> _
Public Shared Function PlaySound(ByVal szSound As String, ByVal hModule As IntPtr, ByVal flags As Integer) As Integer
    End Function
    Public Enum PlaySoundFlags As Integer
        SND_SYNC = &H0
        ' play synchronously (default) 
        SND_ASYNC = &H1
        ' play asynchronously 
        SND_NODEFAULT = &H2
        ' silence (!default) if sound not found 
        SND_MEMORY = &H4
        ' pszSound points to a memory file 
        SND_LOOP = &H8
        ' loop the sound until next sndPlaySound 
        SND_NOSTOP = &H10
        ' don't stop any currently playing sound 
        SND_NOWAIT = &H2000
        ' don't wait if the driver is busy 
        SND_ALIAS = &H10000
        ' name is a registry alias 
        SND_ALIAS_ID = &H110000
        ' alias is a predefined ID 
        SND_FILENAME = &H20000
        ' name is file name 
        SND_RESOURCE = &H40004
        ' name is resource name or atom 
    End Enum

    Public Sub Beep()

        Play(ConfigDataMgr.GetInstance.GetParam(ConfigKey.BEEP_PATH).ToString())
    End Sub

    Public Shared Sub Play(ByVal fileName As String)
        Try
            PlaySound(fileName, IntPtr.Zero, CInt(PlaySoundFlags.SND_FILENAME Or PlaySoundFlags.SND_SYNC))
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at Playing Sound:" + ex.StackTrace, Logger.LogLevel.RELEASE)
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

        ' objAppContainer.objLogger.WriteAppLog("Enter ASYS HandleScanData", Logger.LogLevel.INFO)
        Try
            Cursor.Current = Cursors.WaitCursor
            'm_AutoSYSHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            Select Case Type
                Case BCType.EAN
                    If Not (objAppContainer.objHelper.ValidateEAN(strBarcode)) Or _
                          Val(strBarcode) = 0 Then
                        'Invalid Product Code
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        'Added for RF Mode -Lakshmi

                        m_AutoSYSHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)


                        'Ends
                        ' DisplayASYSScreen(ASYSSCREENS.Home)
                        Exit Sub
                    Else
                        strBarcode = strBarcode.PadLeft(13, "0")
                        '#If NRF Then
                        strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                        '#End If

                        If ProcessBarcodeEntry(strBarcode, True) Then
                            m_AutoSYSHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            'If TSF is greater than PSC then Home screen is displayed
                            If CInt(m_ASYSProductInfo.TSF) > CInt(m_ASYSProductInfo.PSC) Then
                                BCReader.GetInstance().StartRead()
                                m_AutoSYSHome.objStatusBar.SetMessage(m_ASYSProductInfo.BootsCode + "- No Action Required")
                                DisplayASYSScreen(ASYSSCREENS.Home)
                                Exit Sub
                                'If TSF is less than or equal to PSC then the Item details screen is displayed
                            Else


                                m_iIndex = m_ASYSItemsPicked.Count - 1
                                BCReader.GetInstance().StartRead()

                                'beep sound
                                Beep()
                                DisplayASYSScreen(ASYSSCREENS.ItemDetails)
                                Exit Sub
                            End If
                        Else
                            'Added for RF Mode -Lakshmi

                            m_AutoSYSHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                            'Ends
                            Exit Sub
                        End If

                    End If
                Case BCType.ManualEntry
                    Dim strBootsCode As String = ""
                    'If bootscode is entered manually
                    If strBarcode.Length < 8 Then
                        'strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                        'strBarcode = strBarcode.Substring(0, 6)
                        'strBarcode = strBarcode.PadLeft(12, "0")
                        If objAppContainer.objHelper.ValidateBootsCode(strBarcode) Then
                            If ProcessBarcodeEntry(strBarcode, False) Then
                                'If TSF is greater than PSC then Home screen is displayed
                                If CInt(m_ASYSProductInfo.TSF) > CInt(m_ASYSProductInfo.PSC) Then
                                    BCReader.GetInstance().StartRead()
                                    m_AutoSYSHome.objStatusBar.SetMessage(m_ASYSProductInfo.BootsCode + "- No Action Required")
                                    DisplayASYSScreen(ASYSSCREENS.Home)
                                    Exit Sub
                                Else
                                    'If TSF is less than or equal to PSC then the Item details screen is displayed
                                    m_iIndex = m_ASYSItemsPicked.Count - 1
                                    BCReader.GetInstance().StartRead()
                                    'beep sound
                                    Beep()
                                    DisplayASYSScreen(ASYSSCREENS.ItemDetails)
                                    Exit Sub
                                End If
                            Else
                                'Added for RF Mode 
                                m_AutoSYSHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                'Ends
                                m_AutoSYSHome.objProdSEL.txtProduct.Text = ""
                                Exit Sub
                            End If
                        Else
                            'Invalid product code
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                            MessageBoxDefaultButton.Button1)
                            'Added for RF Mode 
                            m_AutoSYSHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            'Ends
                            DisplayASYSScreen(ASYSSCREENS.Home)
                            Exit Sub
                        End If
                    Else
                        If (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                            strBarcode = strBarcode.PadLeft(13, "0")
                            strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)

                            If ProcessBarcodeEntry(strBarcode, True) Then
                                'If TSF is greater than PSC then Home screen is displayed
                                If CInt(m_ASYSProductInfo.TSF) > CInt(m_ASYSProductInfo.PSC) Then
                                    BCReader.GetInstance().StartRead()
                                    m_AutoSYSHome.objStatusBar.SetMessage(m_ASYSProductInfo.BootsCode + "- No Action Required")
                                    DisplayASYSScreen(ASYSSCREENS.Home)
                                    Exit Sub
                                Else
                                    'If TSF is less than or equal to PSC then the Item details screen is displayed
                                    m_iIndex = m_ASYSItemsPicked.Count - 1
                                    BCReader.GetInstance().StartRead()
                                    'beep sound
                                    Beep()
                                    DisplayASYSScreen(ASYSSCREENS.ItemDetails)
                                    Exit Sub
                                End If
                            Else
                                'Added for RF Mode
                                m_AutoSYSHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                'Ends
                                m_AutoSYSHome.objProdSEL.txtProduct.Text = ""
                                Exit Sub
                            End If
                        Else
                            'Invalid Product Code
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                            'Added for RF Mode -Lakshmi
                            m_AutoSYSHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            'Ends
                            DisplayASYSScreen(ASYSSCREENS.Home)
                            Exit Sub
                        End If
                    End If
                Case BCType.SEL
                    If objAppContainer.objHelper.ValidateSEL(strBarcode) Then
                        Dim strBootsCode As String = ""
                        objAppContainer.objHelper.GetBootsCodeFromSEL(strBarcode, strBootsCode)
                        strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)

                        If ProcessBarcodeEntry(strBootsCode, False) Then
                            'If TSF is greater than PSC then Home screen is displayed
                            If CInt(m_ASYSProductInfo.TSF) > CInt(m_ASYSProductInfo.PSC) Then
                                BCReader.GetInstance().StartRead()
                                m_AutoSYSHome.objStatusBar.SetMessage(m_ASYSProductInfo.BootsCode + "- No Action Required")
                                DisplayASYSScreen(ASYSSCREENS.Home)
                                Exit Sub
                            Else
                                'If TSF is less than or equal to PSC then the Item details screen is displayed
                                m_iIndex = m_ASYSItemsPicked.Count - 1
                                BCReader.GetInstance().StartRead()
                                'beep sound
                                Beep()
                                DisplayASYSScreen(ASYSSCREENS.ItemDetails)
                                Exit Sub
                            End If
                        Else
                            'Added for RF Mode -Lakshmi
                            m_AutoSYSHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            'Ends
                            Exit Sub
                        End If
                    Else
                        'Invalid SEL
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M4"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                        'Added for RF Mode -Lakshmi
                        m_AutoSYSHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        'Ends
                    End If
                Case Else
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at FF HandleScanData:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        Finally
            If Not m_AutoSYSHome Is Nothing Then
                If Not m_AutoSYSHome.objProdSEL.txtProduct.IsDisposed AndAlso Not m_AutoSYSHome.objProdSEL.txtSEL.IsDisposed Then
                    m_AutoSYSHome.objProdSEL.txtProduct.Text = ""
                    m_AutoSYSHome.objProdSEL.txtSEL.Text = ""
                End If
            End If
            Cursor.Current = Cursors.Default
        End Try
        ' objAppContainer.objLogger.WriteAppLog("Exit ASYS HandleScanData", Logger.LogLevel.RELEASE)
    End Sub


    ''' <summary>
    ''' Processes the entry of barcode
    ''' </summary>
    ''' <param name="strBarCode"></param>
    ''' <param name="bIsProductCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessBarcodeEntry(ByVal strBarCode As String, ByVal bIsProductCode As Boolean) As Boolean
       
        ' objAppContainer.objLogger.WriteAppLog("Enter ASYS ProcessBarcodeEntry", Logger.LogLevel.RELEASE)
        Dim bIsAlreadyScanned As Boolean = False
        Dim strTempCode As String = "000000000000"
        Dim objCurrentItem As New ASYSProductInfo
        Try
            'for product codes
            If bIsProductCode Then
                For Each objASYSProductInfo As ASYSProductInfo In m_ASYSItemList
                    'checks whether the item was previously scanned
                    If (strBarCode = objASYSProductInfo.ProductCode) Or (strBarCode = objASYSProductInfo.FirstBarcode) Then
                        m_ASYSProductInfo = objASYSProductInfo
                        bIsAlreadyScanned = True
                        Exit For
#If NRF Then 
                    ElseIf strBarCode.StartsWith("2") Or strBarCode.StartsWith("02") Then
                        'to check if already scanned Catch Weight Barcode is scanned again
                        strTempCode = objAppContainer.objHelper.GetBaseBarcode(strBarCode)
                        If (strTempCode = m_ASYSProductInfo.ProductCode) Or (strTempCode = m_ASYSProductInfo.FirstBarcode) Then
                            m_ASYSProductInfo = objASYSProductInfo
                            bIsAlreadyScanned = True
                            Exit For
                        End If
#End If						

                    End If
                Next
                'if scanned for the first time
                If Not bIsAlreadyScanned Then
                    '  objAppContainer.objLogger.WriteAppLog("1", Logger.LogLevel.RELEASE)
                    'if item details are not available
                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarCode, objCurrentItem)) Then
#If NRF Then                       
					  'DARWIN checking if the base barcode is present in Database/Conroller
                        If strBarCode.StartsWith("2") Or strBarCode.StartsWith("02") Then
                            'DARWIN converting database to Base Barcode
                            strBarCode = objAppContainer.objHelper.GetBaseBarcode(strBarCode)
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarCode, objCurrentItem)) Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                       MessageBoxButtons.OK, _
                                       MessageBoxIcon.Asterisk, _
                                       MessageBoxDefaultButton.Button1)

                                Return False
							Else
								m_ASYSProductInfo = objCurrentItem
								'The item is added to the itemlist
								m_ASYSItemList.Add(objCurrentItem)
								If Not CInt(m_ASYSProductInfo.TSF) > CInt(m_ASYSProductInfo.PSC) Then
								'the item is added to the pickeditems list
								m_ASYSItemsPicked.Add(objCurrentItem)

								End If
								' objAppContainer.objLogger.WriteAppLog("2", Logger.LogLevel.RELEASE)
								Return True							
                            End If
                        Else
#End If
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)

                        Return False
#If NRF Then  
                        End If
#End If
                    Else
                        m_ASYSProductInfo = objCurrentItem
                        'The item is added to the itemlist
                        m_ASYSItemList.Add(objCurrentItem)
                        If Not CInt(m_ASYSProductInfo.TSF) > CInt(m_ASYSProductInfo.PSC) Then
                            'the item is added to the pickeditems list
                            m_ASYSItemsPicked.Add(objCurrentItem)

                        End If
                        ' objAppContainer.objLogger.WriteAppLog("2", Logger.LogLevel.RELEASE)
                        Return True

                    End If

                Else
                    'already scanned
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M78"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                    Return True

                End If

                'for bootscode
            Else
                For Each objASYSProductInfo As ASYSProductInfo In m_ASYSItemList
                    'checks whether the item was previously scanned
                    If objASYSProductInfo.BootsCode.Equals(strBarCode) Then
                        m_ASYSProductInfo = objASYSProductInfo
                        bIsAlreadyScanned = True
                        Exit For
                    End If
                Next
                'if scanned for the first time
                If Not bIsAlreadyScanned Then
                    'if item details are not available
                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBarCode, objCurrentItem)) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)

                        Return False
                    Else
                        m_ASYSProductInfo = objCurrentItem
                        'The item is added to the itemlist
                        m_ASYSItemList.Add(objCurrentItem)
                        If Not CInt(m_ASYSProductInfo.TSF) > CInt(m_ASYSProductInfo.PSC) Then
                            'the item is added to the pickeditems list
                            m_ASYSItemsPicked.Add(objCurrentItem)
                        End If

                        Return True
                    End If
                Else
                    'already scanned
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M78"), "Info", _
                               MessageBoxButtons.OK, _
                               MessageBoxIcon.Asterisk, _
                               MessageBoxDefaultButton.Button1)
                    Return True
                End If

            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured at ASYS ProcessBarcodeEntry:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        ' objAppContainer.objLogger.WriteAppLog("Enter ASYS ProcessBarcodeEntry", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Display Items details when an item is selected from View List
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayViewItem()
        Dim iIndex As Integer = 0

        With m_AutoSYSView
            For iIndex = 0 To .lstView.Items.Count - 1
                If .lstView.Items(iIndex).Selected Then
                    m_ASYSProductInfo = m_ASYSItemsPicked.Item(iIndex)
                    DisplayASYSScreen(ASYSSCREENS.ItemDetails)
                    Exit For
                End If
            Next
        End With


    End Sub
    ''' <summary>
    ''' To get the list of items picked
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GetProductList()

    End Sub
    ''' <summary>
    ''' Enum Class that defines all screens for Auto Stuff Your Shelves module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum ASYSSCREENS
        Home
        ItemDetails
        View
        Summary
    End Enum
End Class
''' <summary>
''' The value class for getting and managing Auto Stuff Your Shelves
''' </summary>
''' <remarks></remarks>
Public Class ASYSProductInfo
    Inherits ProductInfo

    Private physicalShelfCapacity As String
    Public Property PSC() As String
        Get
            Return physicalShelfCapacity
        End Get
        Set(ByVal value As String)
            physicalShelfCapacity = value
        End Set
    End Property

    Private m_FirstBarcode As String
    Public Property FirstBarcode() As String
        Get
            Return m_FirstBarcode
        End Get
        Set(ByVal value As String)
            m_FirstBarcode = value
        End Set
    End Property


    Private iMultisiteCnt As String
    Public Property MutiSite() As String
        Get
            Return iMultisiteCnt
        End Get
        Set(ByVal value As String)
            iMultisiteCnt = value
        End Set
    End Property

End Class

