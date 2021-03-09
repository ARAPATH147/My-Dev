'''***************************************************************
''' <FileName>ACSessionManager.vb</FileName>
''' <summary>
''' The Audit Carton Container Class.
''' Implements all business logic and GUI navigation for Audit Carton.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>08-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 1.1 for PPC</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''******************************************************************************
''' Modification Log 
'''******************************************************************************* 
''' No:      Author            Date            Description 
''' 1.1     Charles Skadorwa   04/08/2016   Modified as part of Order & Collect project.
'''           (CS)                          Suppress Boots.com supplier from displaying  
'''                                         - see function HandleData(). 
'''********************************************************************************


Public Class ACSessionManager

    Private Shared m_ACSessionManager As ACSessionManager
    Private m_AuditCarton As frmAuditCarton
    Private m_AuditCartonItem As frmAuditCartonItem
    Private m_AuditCartonItemDetails As frmAuditCartonItemdetails
    Private m_AuditCartonSummary As frmAuditCartonSummary
#If RF Then
    Private m_CalcPad As AuditUODCalcPad
    Private bIsNewSessionAfterConnectionLoss As Boolean = False
#End If

    Private m_ACItemInfo As ItemInfo
    Private m_ACCartonInfo As GIValueHolder.CartonInfo
    ' Private m_CartonList As ArrayList = Nothing
    ' Private m_ItemList As ArrayList = Nothing
    'CHANGE
    Public m_CartonList As ArrayList = Nothing
    Public m_ItemList As ArrayList = Nothing
    Private m_CartonContentsList As ArrayList = Nothing
    Private m_objCarton As Carton
    Private m_objASNCode As GIValueHolder.ASNCode
    Private m_strScanCheck As Boolean
    Private m_strAuditFinishCheck As Boolean
    Public m_bShowMsg As Boolean = False
    Private iItemCount As Integer = 0
    ''' <summary>
    ''' Functions for getting the object instance for the ACSessionManager. 
    ''' Use this method to get the object refernce for the Singleton ACSessionManager.
    ''' </summary>
    ''' <returns>Object reference of ACSessionManager Class</returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As ACSessionManager

        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.AUDITCARTON


        If m_ACSessionManager Is Nothing Then
            m_ACSessionManager = New ACSessionManager()
            Return m_ACSessionManager
        Else
            Return m_ACSessionManager
        End If
    End Function


    ''' <summary>
    ''' Initialises the Audit Carton Session 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartSession()
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
            m_AuditCarton = New frmAuditCarton
            m_AuditCartonItem = New frmAuditCartonItem
            m_AuditCartonItemDetails = New frmAuditCartonItemDetails
            m_AuditCartonSummary = New frmAuditCartonSummary
            m_bShowMsg = False
            m_CartonList = New ArrayList
            m_ItemList = New ArrayList
            m_CartonContentsList = New ArrayList
            m_ACItemInfo = New ItemInfo
            m_ACCartonInfo = New GIValueHolder.CartonInfo
            m_objCarton = New Carton
            m_objCarton.iCountItems = 0
            m_objASNCode = New GIValueHolder.ASNCode
            m_strScanCheck = False
            m_strAuditFinishCheck = False
            iItemCount = 0
            '----------
#If RF Then
            m_CalcPad = New AuditUODCalcPad
            objAppContainer.objDataEngine.SendAuditSession()
            If Not objAppContainer.bCommFailure And Not objAppContainer.bReconnectSuccess Then
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                Me.DisplayACScreen(ACSCREENS.Audit)
            ElseIf objAppContainer.bReconnectSuccess Then
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                Me.DisplayACScreen(ACSCREENS.Audit)
                ' objAppContainer.objDirReceive.Visible = True
                bIsNewSessionAfterConnectionLoss = True
            End If

            objAppContainer.bReconnectSuccess = False

#ElseIf NRF Then
             objAppContainer.objDataEngine.SendAuditSession()
            'End If
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
            Me.DisplayACScreen(ACSCREENS.Audit)
#End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occurred at Audit Carton Start Session: " + ex.ToString, Logger.LogLevel.RELEASE)
        End Try

    End Sub
    ''' <summary>
    ''' To set the quantity of audited item and store the details to an arraylist
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetItemQty(ByVal strQty As String)

        Try
            m_ACItemInfo.ItemQty = CInt(strQty)
            '   m_ACItemInfo.ItemQty = CInt(m_AuditCartonItemDetails.lblQty.Text)
            Dim bIsPresentInList As Boolean = False
            'Checks whether the item is scanned previously
            For Each objCartonPreviousItem As GIValueHolder.ScanDetails In m_ItemList
                If objCartonPreviousItem.ProductCode = m_ACItemInfo.ProductCode Then
                    objCartonPreviousItem.ItemQty = m_ACItemInfo.ItemQty.ToString()
                    bIsPresentInList = True
                    Exit For
                End If
            Next
            'If scanned for the first time added to the arraylist
            If Not bIsPresentInList Then
                Dim objCartonCurrentItem As New GIValueHolder.ScanDetails
                objCartonCurrentItem.ScannedCode = m_objCarton.strCartonNo
                objCartonCurrentItem.ProductCode = m_ACItemInfo.ProductCode
                objCartonCurrentItem.ItemQty = m_ACItemInfo.ItemQty.ToString()
                objCartonCurrentItem.ScanType = ScanType.Audited
                objCartonCurrentItem.ScanLevel = ScanLevel.Item
                objCartonCurrentItem.ScanDate = Format(DateTime.Now, "yyyyMMdd")
                objCartonCurrentItem.ScanTime = Format(DateTime.Now, "HHmmss")
                m_ItemList.Add(objCartonCurrentItem)
                m_objCarton.iCountItems = m_objCarton.iCountItems + 1
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit Carton Set Item Quantity Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try


    End Sub

    ''' <summary>
    ''' Screen Display method for Audit Carton. 
    ''' All Audit Carton sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName"></param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function DisplayACScreen(ByVal ScreenName As ACSCREENS) As Boolean

        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
            Select Case ScreenName
                Case ACSCREENS.Audit
                    m_AuditCarton.Invoke(New EventHandler(AddressOf DisplayACScan))
                Case ACSCREENS.AuditItem
                    m_AuditCartonItem.Invoke(New EventHandler(AddressOf DisplayAuditCartonItem))
                Case ACSCREENS.AuditItemDetails
                    m_AuditCartonItemDetails.Invoke(New EventHandler(AddressOf DisplayAuditCartonItemDetails))
                Case ACSCREENS.AuditSummary
                    m_AuditCartonSummary.Invoke(New EventHandler(AddressOf DisplayAuditCartonSummary))
            End Select
        Catch ex As Exception
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit Carton Display Screen Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' Displays the screen to scan Carton
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayACScan(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.AUDITCARTON
        objAppContainer.objStatusBar.SetMessage("")
        With m_AuditCarton
            .txtProductCode.Text = ""
            .txtProductCode.Focus()
            '.objScanableField.txtProduct.Text = ""
            .Visible = True
            .Refresh()
        End With
        'To display status bar message
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
    End Sub
    ''' <summary>
    ''' Displays the screen to scan Item
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayAuditCartonItem(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.AUDITCARTONITEM
        objAppContainer.objStatusBar.SetMessage("")
        With m_AuditCartonItem
            .Visible = True
            '.objScanableField.txtProduct.Text = ""
            .txtProductCode.Text = ""
            .txtProductCode.Focus()
            'If m_ItemList.Count > 0 Then
            '    .Btn_Finish.Visible = True
            'Else

            '    .Btn_Finish.Visible = False
            'End If
            .Refresh()
            'To display status bar message
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        End With
        'Check in case of reconnect, if the carton and supplier are same then set the item details.
        'else clear the items.
        Try
#If RF Then


            If bIsNewSessionAfterConnectionLoss And objAppContainer.m_SavedDetails.Count > 0 And _
                objAppContainer.eDeliveryType = AppContainer.DeliveryType.ASN And _
                objAppContainer.eFunctionType = AppContainer.FunctionType.Audit Then
                Dim objCartonItem As GIValueHolder.ScanDetails = objAppContainer.m_SavedDetails(0)
                If objCartonItem.ScannedCode = m_objCarton.strCartonNo Then
                    m_ItemList = objAppContainer.m_SavedDetails
                    m_objCarton.iCountItems = m_ItemList.Count
                Else
                    objAppContainer.eDeliveryType = AppContainer.DeliveryType.ASN
                    objAppContainer.eFunctionType = AppContainer.FunctionType.Audit
                    objAppContainer.m_SavedDetails = New ArrayList()
                End If
            Else
                objAppContainer.eDeliveryType = AppContainer.DeliveryType.ASN
                objAppContainer.eFunctionType = AppContainer.FunctionType.Audit
                objAppContainer.m_SavedDetails = New ArrayList()
            End If
            'Set the boolean session state to false.
            bIsNewSessionAfterConnectionLoss = False
#End If
        Catch ex As Exception
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit Carton Display Carton Item Scan: " + ex.Message + ex.ToString(), _
                                                                Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Displays the Summary screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayAuditCartonSummary(ByVal o As Object, ByVal e As EventArgs)
        Try
            objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.AUDITCARTONSUM
#If NRF Then
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
#ElseIf RF Then
               objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
#End If

            'If summary screen is displayed after clicking finish then carton quantity and message is shown
            If m_strAuditFinishCheck Then
                m_AuditCartonSummary.lblMsg.Visible = True
                If m_ItemList.Count > 0 Or iItemCount > 0 Then
                    m_AuditCartonSummary.lblQty.Text = Quantity.One
                ElseIf m_ItemList.Count = 0 Then
                    m_AuditCartonSummary.lblQty.Text = Quantity.Zero
                End If

                m_strAuditFinishCheck = False

                'If summary screen is displayed by quitting from intermediate
                ' screens then the quantity will be zero and message wont be shown
            ElseIf Not m_strAuditFinishCheck Then
                m_AuditCartonSummary.lblQty.Text = Quantity.Zero

            End If
            With m_AuditCartonSummary
                .Visible = True
                .Refresh()
            End With
            'To display status bar message
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit Carton Display Summary Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub

    ''' <summary>
    ''' Displays the Item Details screen 
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayAuditCartonItemDetails(ByVal o As Object, ByVal e As EventArgs)
        Try
            objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.AUDITCARTONITEMDETAILS
            Dim objDescriptionArray As ArrayList = New ArrayList
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(m_ACItemInfo.ItemDesc)

            With m_AuditCartonItemDetails
                .lblItem.Text = objDescriptionArray.Item(0).ToString()
                .lblItem2.Text = objDescriptionArray.Item(1).ToString()
                .lblItem3.Text = objDescriptionArray.Item(2).ToString()
                If m_ACItemInfo.BootsCode.Length <= 7 Then
                .lblBootsCode.Text = objAppContainer.objHelper.FormatBarcode(m_ACItemInfo.BootsCode)
                Else
                    .lblBootsCode.Text = ""
                End If
                If m_ACItemInfo.ProductCode.Length <= 7 Then
                    .lblItemcode.Text = ""
                Else
                .lblItemcode.Text = objAppContainer.objHelper.FormatBarcode(m_ACItemInfo.ProductCode)
                End If

                'If the item is previously scanned then the corresponding quantity is displayed
                'else the quantity is prepopulated with value 1
                If m_strScanCheck Then
                    .lblQty.Text = m_ACItemInfo.ItemQty.ToString()
                    m_strScanCheck = False

                Else
                    .lblQty.Text = Quantity.One
                End If
                objAppContainer.objStatusBar.SetMessage("")
                .Visible = True
                .Refresh()
                'To display status bar message
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            End With
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit Carton Item Details Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To quit the session without saving anything and GoodsIn Main menu is displayed
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub QuitSession()
        Dim iResult As Integer = 0
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M84"), "Confirmation", MessageBoxButtons.YesNo, _
                                      MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
            If iResult = MsgBoxResult.Yes Then
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
                m_ACSessionManager.EndSession(AppContainer.IsAbort.Yes)
                'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit Carton Quit Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub

    ''' <summary>
    ''' To quit the session session without saving anything and the summary screen is displayed
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub QuitBeforeCommit()
        Dim iResult As Integer = 0
        Try
            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M48"), "Confirmation", MessageBoxButtons.YesNo, _
                                      MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
            If iResult = MsgBoxResult.Yes Then
                m_strAuditFinishCheck = False
                DisplayACScreen(ACSCREENS.AuditSummary)

            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit Carton Quit Before Commit Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Finishing the session
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub FinishSession()
        Dim iResult As Integer = 0
        Try
            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M86"), "Alert", MessageBoxButtons.OKCancel, _
                                      MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            If iResult = MsgBoxResult.OK Then
                'Modified to display dock and transmit only when the sesion is completed
                m_bShowMsg = True
                'sending the item details as export data
                If m_ItemList.Count > 0 Then
                    'To show the message 'Dock and Transmit'
                    m_bShowMsg = True
                    iItemCount = m_ItemList.Count
#If RF Then
                    If objAppContainer.objDataEngine.SendItemDetails(m_ItemList, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit) Then
                        m_ItemList.Clear()
                        objAppContainer.objDataEngine.SendItemQuantity(iItemCount, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit)
                    End If
#ElseIf NRF Then
                    objAppContainer.objDataEngine.SendItemDetails(m_ItemList, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit)
                    objAppContainer.objDataEngine.SendItemQuantity(m_ItemList.Count, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit)
#End If
                Else
                    m_bShowMsg = False
                End If

                m_strAuditFinishCheck = True
#If RF Then
                If Not objAppContainer.bCommFailure AndAlso Not objAppContainer.bReconnectSuccess Then
                    DisplayACScreen(ACSCREENS.AuditSummary)
                End If
                objAppContainer.bReconnectSuccess = False
#ElseIf NRF Then
                DisplayACScreen(ACSCREENS.AuditSummary)
#End If
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit Carton Finish Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by ACSessionManager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EndSession(ByVal isAbort As AppContainer.IsAbort, Optional ByVal iSwitching As Boolean = False)
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
            
            'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
            'sending GIX for export Data writing
            If Not iSwitching Then
                objAppContainer.objLogger.WriteAppLog("Audit Carton End Session", Logger.LogLevel.INFO)
#If RF Then
                objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
                If Not objAppContainer.bCommFailure Then
                    objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit, isAbort)


#ElseIf NRF Then
                objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit, isAbort)

#End If
                    m_AuditCarton.Close()
                    m_AuditCartonItem.Close()
                    m_AuditCartonItemDetails.Close()
                    m_AuditCartonSummary.Close()
                    m_AuditCarton.Dispose()
                    m_AuditCartonItem.Dispose()
                    m_AuditCartonItemDetails.Dispose()
                    m_AuditCartonSummary.Dispose()
                    m_CartonContentsList = Nothing
                    m_CartonList = Nothing
                    m_ACItemInfo = Nothing
                    m_ACCartonInfo = Nothing
                    m_objCarton = Nothing
                    m_objASNCode = Nothing
                    m_strScanCheck = False
#If RF Then
                    objAppContainer.bReconnectSuccess = False
                    objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
                End If
#End If
                'CR for Forced Log off - Check this when it goes to book in Carton from Audit carton
#If NRF Then
                If isAbort = AppContainer.IsAbort.No Then
                    objAppContainer.bForceLogOff = True
                    objAppContainer.ForcedLogOff()
                End If
#End If
            End If
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit Carton End Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
#If RF Then
    Public Sub DisposeAuditCarton()
        Try
            m_AuditCarton.Close()
            m_AuditCartonItem.Close()
            m_AuditCartonItemDetails.Close()
            m_AuditCartonSummary.Close()
            m_AuditCarton.Dispose()
            m_AuditCartonItem.Dispose()
            m_AuditCartonItemDetails.Dispose()
            m_AuditCartonSummary.Dispose()
            m_CartonContentsList = Nothing
            m_CartonList = Nothing
            m_ACItemInfo = Nothing
            m_ACCartonInfo = Nothing
            m_objCarton = Nothing
            m_objASNCode = Nothing
            m_strScanCheck = False
        Catch ex As Exception

        End Try
    End Sub
#End If
    ''' <summary>
    ''' The Method handles the scan data returned from the barcode scanner.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <remarks></remarks>
    Public Sub HandleData(ByVal strBarcode As String)
        Dim iResult As Integer = 0
        Dim bValidate As Boolean = True
        Dim iMsg As Integer = 0
        Try
            'Scanning Carton
            If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.AUDITCARTON Then
                Cursor.Current = Cursors.WaitCursor
                'Barcode not recognised
                If Not objAppContainer.objHelper.ValidateASNBarcode(strBarcode, m_objASNCode) Then
                    ' objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                         MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                    If iResult = MsgBoxResult.OK Then
                        Cursor.Current = Cursors.Default
                        DisplayACScreen(ACSCREENS.Audit)
                    End If
                Else


#If RF Then
                    ' O&C Prevent booking in of Boots.Com orders (CS)
                    If Left(strBarcode, 6) = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DOTCOM_SUPPLIER_NUM) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M138"), "Warning", _
                                       MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                        bValidate = False
                        DisplayACScreen(ACSCREENS.Audit)

                    Else
#End If

                        ' objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
                        'Carton Not In File Condition

                        If Not (objAppContainer.objDataEngine.ValidateCartonScanned(strBarcode, m_ACCartonInfo, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit)) Then
#If RF Then
                            objAppContainer.m_ModScreen = AppContainer.ModScreen.CARTONSCAN
                            bValidate = False
                            'time out
                            If objAppContainer.bTimeOut Then
                                Exit Sub
                            End If
                            If Not objAppContainer.bCommFailure AndAlso Not objAppContainer.bReconnectSuccess Then
                                Cursor.Current = Cursors.Default
                                bValidate = False
                                'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                                'Is this the first attempt
                                iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M44"), "Carton Not On File ", MessageBoxButtons.YesNo, _
                                             MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                                If iResult = MsgBoxResult.Yes Then
                                    'Put carton to one side and try again tomorrow
                                    iMsg = MessageBox.Show(MessageManager.GetInstance().GetMessage("M45"), " ", MessageBoxButtons.OK, _
                                                           MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
                                    If iMsg = MsgBoxResult.Ok Then
                                        DisplayACScreen(ACSCREENS.Audit)
                                    End If
                                    'If not the first attempt
                                ElseIf iResult = MsgBoxResult.No Then
                                    'Book Carton at item level
                                    iMsg = MessageBox.Show(MessageManager.GetInstance().GetMessage("M46"), " ", MessageBoxButtons.OK, _
                                                                                       MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
                                    If iMsg = MsgBoxResult.Ok Then
                                        objAppContainer.strAuditCartonNotinFile = "NIF"
                                        objAppContainer.strNIFCartonCode = strBarcode
                                        'Ends the audit session and starts Book In session
                                        objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit, AppContainer.IsAbort.Yes)
                                        BCSessionMgr.GetInstance().StartSession()
                                        objAppContainer.objPrevMod = AppContainer.ACTIVEMODULE.AUDITCARTON
                                        BCSessionMgr.GetInstance().DisplayBCScreen(BCSCREENS.ItemScan)
                                        EndSession(AppContainer.IsAbort.Yes, True)
                                    End If
                                End If
                            Else
                                Exit Sub
                            End If

#ElseIf NRF Then
                        Cursor.Current = Cursors.Default
                        bValidate = False
                        'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        'Is this the first attempt
                        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M44"), "Carton Not On File ", MessageBoxButtons.YesNo, _
                                     MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        If iResult = MsgBoxResult.Yes Then
                            'Put carton to one side and try again tomorrow
                            iMsg = MessageBox.Show(MessageManager.GetInstance().GetMessage("M45"), " ", MessageBoxButtons.OK, _
                                                   MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
                            If iMsg = MsgBoxResult.Ok Then
                                DisplayACScreen(ACSCREENS.Audit)
                            End If



                            'If not the first attempt
                        ElseIf iResult = MsgBoxResult.No Then
                            'Book Carton at item level
                            iMsg = MessageBox.Show(MessageManager.GetInstance().GetMessage("M46"), " ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
                            If iMsg = MsgBoxResult.Ok Then
                                objAppContainer.strAuditCartonNotinFile = "NIF"
                                objAppContainer.strNIFCartonCode = strBarcode
                                'Ends the audit session and starts Book In session
                                objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit, AppContainer.IsAbort.Yes)
                                BCSessionMgr.GetInstance().StartSession()
                                objAppContainer.objPrevMod = AppContainer.ACTIVEMODULE.AUDITCARTON
                                BCSessionMgr.GetInstance().DisplayBCScreen(BCSCREENS.ItemScan)
                                EndSession(AppContainer.IsAbort.Yes, True)
                            End If
                        End If
#End If
                            'Check whether the carton has been booked in
                        ElseIf Not m_ACCartonInfo.Status = Status.UnBooked Then
                            Cursor.Current = Cursors.Default
                            bValidate = False
                            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M47"), "Alert", MessageBoxButtons.OK, _
                                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            If iResult = MsgBoxResult.Ok Then
                                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                                DisplayACScreen(ACSCREENS.Audit)
                            End If
                        End If

#If RF Then
                        ' O&C End If (CS)
                    End If
#End If

                    'If validations are right item scan screen is displayed
                    If bValidate = True Then
#If RF Then
                        If Not objAppContainer.bCommFailure Then
#End If
                            m_objCarton.strCartonNo = strBarcode
                            'getting the details of items in carton
#If NRF Then
                        objAppContainer.objDataEngine.GetCartonDetails(strBarcode, m_CartonContentsList, AppContainer.DeliveryType.ASN, False, AppContainer.FunctionType.Audit)
#End If
#If RF Then

                            objAppContainer.objDataEngine.GetCartonDetails(strBarcode, m_CartonContentsList, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit)
#End If

                            'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)

#If RF Then
                            Cursor.Current = Cursors.Default
                            If Not objAppContainer.bCommFailure Then

                                DisplayACScreen(ACSCREENS.AuditItem)
                            End If

#ElseIf NRF Then
                        Cursor.Current = Cursors.Default
                        DisplayACScreen(ACSCREENS.AuditItem)
#End If
#If RF Then
                        End If
#End If
                    End If
                End If

                'Scanning Item
            Else
                Dim bTemp As Boolean = False

                If strBarcode.Length < 8 Then
                    'Validate the Boots code entered for confirmation.
                    ' strBarcode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                    bTemp = objAppContainer.objHelper.ValidateBootsCode(strBarcode)
                    If Not bTemp Then
                        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Error ", MessageBoxButtons.OK, _
                                   MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)

                        DisplayACScreen(ACSCREENS.AuditItem)
                        Exit Sub
                    End If


                Else
                    bTemp = objAppContainer.objHelper.ValidateEAN(strBarcode)
                End If
                'Validates Item code
                If Not bTemp Then
                    ' objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M66"), "Error ", MessageBoxButtons.OK, _
                                     MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)

                    DisplayACScreen(ACSCREENS.AuditItem)

                Else
                    Dim strTempCode As String = ""
                    'Checking whether the scanned item belongs to the parent Carton
                    Dim strBootsCode As String = ""
                    Dim bIsPresent As Boolean = False
#If RF Then

                    objAppContainer.m_ModScreen = AppContainer.ModScreen.ITEMSCAN
                    'For Each objPresentItem As GIValueHolder.ItemDetails In m_CartonContentsList
                    '    If objPresentItem.ProductCode = strBarcode OrElse objPresentItem.ItemCode = strBarcode Then
                    '        bIsPresent = True
                    '        Exit For
                    '        'DARWIN CR to handle Catch weight barcodes
                    '    ElseIf (strBarcode.StartsWith("2") OrElse strBarcode.StartsWith("02")) AndAlso strBarcode.Length > 8 Then
                    '        strTempCode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                    '        If objPresentItem.ProductCode = strTempCode Then
                    '            bIsPresent = True
                    '            Exit For
                    '        End If
                    '    End If
                    '    If strBarcode.Substring(0, 6) = "000000" Then
                    '        strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode.Substring(6, 6))
                    '        If objPresentItem.ItemCode = strBootsCode Then
                    '            bIsPresent = True
                    '            Exit For
                    '        End If

                    '    End If

                    'Next
#ElseIf NRF Then
                    Dim strTemItemCode As String = " "
                    If strBarcode.Length <= 7 Then
                        strTemItemCode = strBarcode
                    Else
                        strTemItemCode = objAppContainer.objDataEngine.GetBootsCode(strBarcode)
                    End If

                    For Each objPresentItem As GIValueHolder.ItemDetails In m_CartonContentsList
                        If (objPresentItem.ItemCode = strTemItemCode) Then
                            bIsPresent = True
                            Exit For
                        End If

                    Next
#End If


                    If Not bIsPresent Then
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M107"), "Alert ", MessageBoxButtons.YesNo, _
                                                                                          MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        If iResult = MsgBoxResult.No Then
                            DisplayACScreen(ACSCREENS.AuditItem)
                            Exit Sub
                        End If
                    End If

#If NRF Then
                    objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
#End If

                    'Getting the details of the scanned item
                    If Not (objAppContainer.objDataEngine.GetItemDetails(strBarcode, AppContainer.ItemDetailType.Carton, m_ACItemInfo)) Then
                        ' objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        'Item Not in file
#If RF Then
                        'TIMEOUT -  if timeout do not do any processing
                        If objAppContainer.bTimeOut Then
                            Exit Sub
                        End If
                        If Not objAppContainer.bCommFailure And Not objAppContainer.bReconnectSuccess Then
#End If
                            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M67"), "Alert ", MessageBoxButtons.OK, _
                                         MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            DisplayACScreen(ACSCREENS.AuditItem)
#If RF Then
                        End If
                        objAppContainer.bReconnectSuccess = False
#End If

                    Else
#If RF Then


                        '+ve UOD CR
                        For Each objPresentItem As GIValueHolder.ItemDetails In m_CartonContentsList
                            If objPresentItem.ProductCode = m_ACItemInfo.ProductCode OrElse objPresentItem.ItemCode = m_ACItemInfo.BootsCode Then
                                bIsPresent = True
                                Exit For
                                'DARWIN CR to handle Catch weight barcodes
                            ElseIf (strBarcode.StartsWith("2") OrElse strBarcode.StartsWith("02")) AndAlso strBarcode.Length > 8 Then
                                strTempCode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                                If objPresentItem.ProductCode = strTempCode Then
                                    bIsPresent = True
                                    Exit For
                                End If
                            End If
                            If strBarcode.Substring(0, 6) = "000000" Then
                                strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode.Substring(6, 6))
                                If objPresentItem.ItemCode = m_ACItemInfo.BootsCode Then
                                    bIsPresent = True
                                    Exit For
                                End If

                            End If

                        Next
#End If
                        'checks whether the item is previously scanned and if yes sets the item quantity
                        For Each objCartonItem As GIValueHolder.ScanDetails In m_ItemList
                            If objCartonItem.ProductCode = m_ACItemInfo.ProductCode Or _
                            objCartonItem.BootCode = m_ACItemInfo.BootsCode Then
                                m_ACItemInfo.ItemQty = CInt(objCartonItem.ItemQty)
                                m_strScanCheck = True
                                Exit For
                            End If

                        Next
                        Cursor.Current = Cursors.Default
                        DisplayACScreen(ACSCREENS.AuditItemDetails)
                    End If
                End If

            End If
#If NRF Then
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
#End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit Carton Handle Data Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try


    End Sub

    ''' <summary>
    ''' The Method handles the scan data returned form the barcode scanner.
    ''' This method implements the business logic to populate the data to the corresponding
    ''' UI element after validation.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="Type"></param>
    ''' <remarks></remarks>
    Public Sub HandleScanData(ByVal strBarcode As String, ByVal Type As BCType)
#If RF Then
        If objAppContainer.bReconnectSuccess Then
            objAppContainer.bReconnectSuccess = False
        End If
#End If
        Select Case Type
            Case BCType.CODE128
                HandleData(strBarcode)
            Case BCType.EAN
                HandleData(strBarcode)
            Case BCType.ManualEntry
                HandleData(strBarcode)
            Case BCType.UPC
                If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.AUDITCARTONITEM Then
                    HandleData(strBarcode)
                Else
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                                                                                                   MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                    Exit Sub
                End If
            Case Else
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                                                                                MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                Exit Sub
        End Select
    End Sub
#If RF Then
   Public Sub ShowCalcPadForQuantity(ByVal quantity As String)
        '***Display the calc pad in order to enter a quantity
        '***if we already have a non zero quantity then display this as default
        'Mark Goode 17/03/2005 - quantity <> "" or quantity <> "0"
        If quantity <> "0" And quantity <> "" Then
            m_CalcPad.InitialValue = quantity & " + "
        Else
            m_CalcPad.InitialValue = ""
        End If
        objAppContainer.objCalcpad.Invoke(New EventHandler(AddressOf PopulateCalcQuantity))
        objAppContainer.DisplayHelpScreens(AppContainer.HSCREENS.Calcpad)
    End Sub
    Private Sub PopulateCalcQuantity(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objCalcpad.tbValue.Text = m_CalcPad.InitialValue
    End Sub
    Private Class AuditUODCalcPad
        Public InitialValue As String
    End Class
#End If
End Class

''' <summary>
''' Screens in Audit Carton
''' </summary>
''' <remarks></remarks>
Public Enum ACSCREENS
    Audit
    AuditItem
    AuditItemDetails
    AuditSummary
End Enum
''' <summary>
''' Class to maintain the count of items scanned
''' </summary>
''' <remarks></remarks>
Public Class Carton
    Public strCartonNo As String
    Public iCountItems As Integer
End Class
