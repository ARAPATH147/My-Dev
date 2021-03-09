'''***************************************************************
''' <FileName>AUODSessionManager.vb</FileName>
''' <summary>
''' The Audit UOD Container Class.
''' Implements all business logic and GUI navigation for Audit UOD.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>08-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 1.1 for PPC</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
'''* Modification Log 
'''******************************************************************************* 
'''* No:       Author            Date            Description 
'''* 1.1     Christopher Kitto  09/04/2015   Modified as part of DALLAS project.
'''                  (CK)                    Amended the function HandleScanData to
'''                                          show message if Dallas UOD is scanned
'''                                          or entered.                      
'''********************************************************************************

Public Class AUODSessionManager
    Private Shared m_AUODSessionMgr As AUODSessionManager
    Private m_GInAudit As frmGInAudit
    Private m_GInAuditItem As frmGInAuditItem
    Private m_GInAuditItemDetails As frmGINAuditItemDetails
    Private m_GInAuditSummary As frmGInAuditSummary


    Private m_AUODItemInfo As ItemInfo
    Private m_AUODInfo As UODInfo
    Private m_UODList As ArrayList = Nothing
    ' Private m_ItemList As ArrayList = Nothing
    'CHANGE
    Public m_ItemList As ArrayList = Nothing
    Private m_UODContentsList As ArrayList = Nothing
    Private m_objUOD As UOD
    Private m_strScanCheck As Boolean
    Private m_arrItemDetails As New ArrayList
    ' V1.1 - CK
    ' Declared m_MsgBox
    Private m_MsgBox As MsgBx
    Private m_BCType As BCType
    Private m_bUnfreeze As Boolean = False
    Private bIsNewSessionAfterConnectionLoss As Boolean = False
    Private iItemCount As Integer = 0
    ''' <summary>
    ''' Functions for getting the object instance for the AUODSessionManager. 
    ''' Use this method to get the object refernce for the Singleton AUODSessionManager.
    ''' </summary>
    ''' <returns>Object reference of AUODSessionManager Class</returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As AUODSessionManager


        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.AUDITUOD
        If m_AUODSessionMgr Is Nothing Then
            m_AUODSessionMgr = New AUODSessionManager
            Return m_AUODSessionMgr
        Else
            Return m_AUODSessionMgr
        End If
    End Function
    ''' <summary>
    ''' Initialises the Audit UOD Session 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartSession()
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
           m_GInAudit = New frmGInAudit
            m_GInAuditItem = New frmGInAuditItem
            m_GInAuditItemDetails = New frmGInAuditItemDetails
            m_GInAuditSummary = New frmGInAuditSummary
            ' V1.1 _ CK
            ' Initialised m_Msgbox
#If RF Then
            m_MsgBox = New MsgBx
#End If
            m_UODList = New ArrayList
            m_ItemList = New ArrayList
            m_UODContentsList = New ArrayList
            m_AUODItemInfo = New ItemInfo
            m_AUODInfo = New UODInfo
            m_objUOD = New UOD
            m_objUOD.iCountItems = 0
            m_strScanCheck = False
            iItemCount = 0
#If RF Then
            objAppContainer.objDataEngine.GetAuditSummary()
            If Not objAppContainer.bCommFailure And Not objAppContainer.bReconnectSuccess Then
                Me.DisplayAUODScreen(AUODSCREENS.Audit)
            ElseIf objAppContainer.bReconnectSuccess Then
                DisplayAUODScreen(AUODSCREENS.Audit)
                bIsNewSessionAfterConnectionLoss = True
                '  objAppContainer.objSSCReceivingMainMenu.Visible = True
            End If
            objAppContainer.bReconnectSuccess = False

#ElseIf NRF Then
            objAppContainer.objDataEngine.GetAuditSummary()
            Me.DisplayAUODScreen(AUODSCREENS.Audit)
#End If


        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit UOD Start Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try

    End Sub
    ''' <summary>
    ''' To set the quantity of audited item and store the details to an arraylist
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetItemQuantity(ByVal strQty As String)
        Try
            m_AUODItemInfo.ItemQty = CInt(strQty)
            ' m_AUODItemInfo.ItemQty = CInt(m_GInAuditItemDetails.lblQty.Text)
            Dim bIsPresentInList As Boolean = False

            'Checks whether the item is scanned previously
            For Each objUODPreviousItem As GIValueHolder.ScanDetails In m_ItemList
                If objUODPreviousItem.ProductCode = m_AUODItemInfo.ProductCode Or _
                objUODPreviousItem.BootCode = m_AUODItemInfo.BootsCode Then
                    'sets the qty
                    objUODPreviousItem.ItemQty = m_AUODItemInfo.ItemQty.ToString()
                    bIsPresentInList = True
                    Exit For
                End If
            Next
            'If scanned for the first time added to the arraylist
            If Not bIsPresentInList Then
                Dim objUODCurrentItem As New GIValueHolder.ScanDetails
                objUODCurrentItem.ScannedCode = m_objUOD.strUODNumber.Substring(0, 10)
                objUODCurrentItem.ProductCode = m_AUODItemInfo.ProductCode
                objUODCurrentItem.ItemQty = m_AUODItemInfo.ItemQty.ToString()
                objUODCurrentItem.ScanType = ScanType.Audited
                objUODCurrentItem.ScanLevel = ScanLevel.Item
                objUODCurrentItem.ScanDate = Format(DateTime.Now, "yyyyMMdd")
                objUODCurrentItem.ScanTime = Format(DateTime.Now, "HHmmss")
                objUODCurrentItem.DespatchDate = m_AUODInfo.DespatchDate
                m_ItemList.Add(objUODCurrentItem)
                m_objUOD.iCountItems = m_objUOD.iCountItems + 1
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit UOD Set Item Quantity Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub


    ''' <summary>
    ''' Screen Display method for Audit UOD. 
    ''' All Audit UOD sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName"></param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function DisplayAUODScreen(ByVal ScreenName As AUODSCREENS) As Boolean
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
            Select Case ScreenName
                Case AUODSCREENS.Audit
                    m_GInAudit.Invoke(New EventHandler(AddressOf DisplayAUODScan))
                Case AUODSCREENS.AuditItem
                    m_GInAuditItem.Invoke(New EventHandler(AddressOf DisplayAuditItem))
                Case AUODSCREENS.AuditItemDetails
                    m_GInAuditItemDetails.Invoke(New EventHandler(AddressOf DisplayAuditItemDetails))
                Case AUODSCREENS.AuditSummary
                    m_GInAuditSummary.Invoke(New EventHandler(AddressOf DisplayAuditSummary))
            End Select
        Catch ex As Exception
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit UOD Display Screen Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function

    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by AUODSessionManager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EndSession(ByVal isAbort As AppContainer.IsAbort, Optional ByVal iSwitching As Boolean = False)
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
            
#If RF Then
            objAppContainer.m_ModScreen = AppContainer.ModScreen.QUIT
            'RF RECONNECT
            If Not objAppContainer.bCommFailure Then
                'sending GIX for export Data writing
                If objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.SSC, AppContainer.FunctionType.Audit, isAbort) Then
#ElseIf NRF Then
            'sending GIX for export Data writing
            objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.SSC, AppContainer.FunctionType.Audit, isAbort)

#End If
                    m_GInAudit.Close()
                    m_GInAuditItem.Close()
                    m_GInAuditItemDetails.Close()
                    m_GInAuditSummary.Close()
                    m_GInAudit.Dispose()
                    m_GInAuditItem.Dispose()
                    m_GInAuditItemDetails.Dispose()
                    m_GInAuditSummary.Dispose()
                    m_UODList = Nothing
                    m_UODContentsList = Nothing
                    m_AUODItemInfo = Nothing
                    m_AUODInfo = Nothing
                    m_objUOD = Nothing
                    m_strScanCheck = False
#If RF Then
                    objAppContainer.bReconnectSuccess = False
                    objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
                End If
            End If
#End If

            'CR for Forced Log off
            If Not iSwitching Then
#If NRF Then
                ' objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit, isAbort)
                'CR for Forced Log off - Check this when it goes to book in Carton from Audit carton
                If isAbort = AppContainer.IsAbort.No Then
                    objAppContainer.bForceLogOff = True
                    objAppContainer.ForcedLogOff()
                End If

#End If
            End If
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit UOD End Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try

    End Sub
#If RF Then
    Public Sub DisposeAuditUOD()
        m_GInAudit.Close()
        m_GInAuditItem.Close()
        m_GInAuditItemDetails.Close()
        m_GInAuditSummary.Close()
        m_GInAudit.Dispose()
        m_GInAuditItem.Dispose()
        m_GInAuditItemDetails.Dispose()
        m_GInAuditSummary.Dispose()
        m_UODList = Nothing
        m_UODContentsList = Nothing
        m_AUODItemInfo = Nothing
        m_AUODInfo = Nothing
        m_objUOD = Nothing
        m_strScanCheck = False
    End Sub
#End If

    ''' <summary>
    ''' The Method handles the scan data returned form the barcode scanner.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <remarks></remarks>
    Public Sub HandleData(ByVal strBarcode As String)
        Dim iResult As Integer = 0
        Dim bValidate As Boolean = True
        Dim strUODBarcode As String = ""

        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
            'Scanning UOD
            If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINAUDIT Then

                If Not objAppContainer.objHelper.ValidateUODBarcode(strBarcode, strUODBarcode) Then

                    'Validating IST
                    If objAppContainer.objHelper.ValidateISTCode(strBarcode, strUODBarcode) Then
                        'getting the details of items in UOD
                        If Not objAppContainer.objDataEngine.GetUODDetails(strUODBarcode, m_AUODInfo, m_UODContentsList) Then
                            'UOD not in file
                            bValidate = False
                            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M40"), "Alert", MessageBoxButtons.OK, _
                                         MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            If iResult = MsgBoxResult.OK Then
                                DisplayAUODScreen(AUODSCREENS.Audit)
                                Exit Sub
                            End If
                            'Check whether UOD is booked in or not
                        ElseIf m_AUODInfo.UODStatus = Status.UnBooked Then

                            bValidate = False
                            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M41"), "Alert ", MessageBoxButtons.OK, _
                                         MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            If iResult = MsgBoxResult.OK Then

                                Dim objSSCReceive As New frmSSCReceivingMainMenu
                                objSSCReceive.Show()
                                m_bUnfreeze = True
                                EndSession(AppContainer.IsAbort.No)
                                '  objAppContainer.objSSCReceivingMainMenu.Show()
                                Exit Sub
                            End If

                            'check for previous audit
                        ElseIf m_AUODInfo.UODStatus = Status.Audited Then
                            bValidate = False
                            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M42"), "Alert ", MessageBoxButtons.OK, _
                                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            If iResult = MsgBoxResult.OK Then
                                m_objUOD.strUODNumber = strUODBarcode
                                DisplayAUODScreen(AUODSCREENS.AuditItem)
                            End If
                        End If


                        If bValidate = True Then
                            m_objUOD.strUODNumber = strUODBarcode
                            DisplayAUODScreen(AUODSCREENS.AuditItem)
                        End If
                        Exit Sub
                        'Barcode not recognised
                    Else
                        objAppContainer.objStatusBar.SetMessage("")
                        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                           MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                        If iResult = MsgBoxResult.OK Then
                            DisplayAUODScreen(AUODSCREENS.Audit)
                        End If
                    End If


                Else
                    'Cant audit Dolly
                    If strBarcode.Substring(0, 1) = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DOLLYID) Then
                        bValidate = False
                        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M43"), "Alert ", MessageBoxButtons.OK, _
                                   MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        If iResult = MsgBoxResult.OK Then
                            DisplayAUODScreen(AUODSCREENS.Audit)
                            Exit Sub
                        End If

                    Else
#If RF Then
                        'RECONNECT - Checking whether UOD is scanned
                        objAppContainer.m_ModScreen = AppContainer.ModScreen.CARTONSCAN

#End If
                        'getting the details of items in UOD
                        If Not objAppContainer.objDataEngine.GetUODDetails(strUODBarcode, m_AUODInfo, m_UODContentsList) Then

                            'UOD not in file
                            bValidate = False
#If RF Then
                            'TIMEOUT- if timeout do not do any processing
                            If objAppContainer.bTimeOut Then
                                Exit Sub
                            End If
                            If Not objAppContainer.bCommFailure Then
                                If Not objAppContainer.bReconnectSuccess Then
#End If
                                    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M40"), "Alert", MessageBoxButtons.OK, _
                                                 MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                                    If iResult = MsgBoxResult.Ok Then
                                        DisplayAUODScreen(AUODSCREENS.Audit)
                                        Exit Sub
                                    End If
#If RF Then
                                Else
                                    DisplayAUODScreen(AUODSCREENS.Audit)
                                    objAppContainer.bReconnectSuccess = False
                                    Exit Sub
                                End If

                            End If

#End If
                                'Check whether UOD is booked in or not
                            ElseIf m_AUODInfo.UODStatus = Status.UnBooked Then

                                bValidate = False
                                iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M41"), "Alert ", MessageBoxButtons.OK, _
                                             MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                                If iResult = MsgBoxResult.Ok Then

                                    Dim objSSCReceive As New frmSSCReceivingMainMenu
                                    objSSCReceive.Show()
                                    m_bUnfreeze = True
                                    'EndSession(AppContainer.IsAbort.No)
                                    EndSession(AppContainer.IsAbort.Yes)
                                    '  objAppContainer.objSSCReceivingMainMenu.Show()
                                    Exit Sub
                                End If

                                'check for previous audit
                            ElseIf m_AUODInfo.UODStatus = Status.Audited Then
                                bValidate = False
                                iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M42"), "Alert ", MessageBoxButtons.OK, _
                                            MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                                If iResult = MsgBoxResult.Ok Then
                                    m_objUOD.strUODNumber = strBarcode
                                    DisplayAUODScreen(AUODSCREENS.AuditItem)
                                End If
                            End If


                        If bValidate = True Then
                            m_objUOD.strUODNumber = strBarcode
                            DisplayAUODScreen(AUODSCREENS.AuditItem)
                        End If

                    End If

                End If

                'Scanning Item
            ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINAUDITITEM Then
                If m_BCType = BCType.CODE128 Or m_BCType = BCType.I2O5 Then

                    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M39"), "Error ", MessageBoxButtons.OK, _
                                                             MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                    m_objUOD.strUODNumber = strBarcode
                    DisplayAUODScreen(AUODSCREENS.AuditItem)

                Else
                    Dim bTemp As Boolean = False
                    If strBarcode.Length < 8 Then

                        'strBarcode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                        ' strBarcode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)

                        bTemp = objAppContainer.objHelper.ValidateBootsCode(strBarcode)
                        If Not bTemp Then
                            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Error ", MessageBoxButtons.OK, _
                                        MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                            m_objUOD.strUODNumber = strBarcode
                            DisplayAUODScreen(AUODSCREENS.AuditItem)
                            Exit Sub
                        End If
                       
                    Else
                        bTemp = objAppContainer.objHelper.ValidateEAN(strBarcode)
                    End If
                    'Validates Item code
                    If Not bTemp Then

                        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M39"), "Error ", MessageBoxButtons.OK, _
                                         MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                        m_objUOD.strUODNumber = strBarcode
                        DisplayAUODScreen(AUODSCREENS.AuditItem)

                    Else

                        'Checking whether the scanned item belongs to the parent UOD
                        'Dim bIsPresent As Boolean = False
                        'For Each objPresentItem As GIValueHolder.ItemList In m_UODContentsList
                        '    If objPresentItem.ProductCode = strBarcode Then
                        '        bIsPresent = True
                        '        Exit For
                        '    End If
                        'Next

                        'If Not bIsPresent Then
                        '    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M108"), "Alert ", MessageBoxButtons.OK, _
                        '                                                                      MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        '    If iResult = MsgBoxResult.OK Then
                        '        DisplayAUODScreen(AUODSCREENS.AuditItem)
                        '        Exit Sub
                        '    End If
                        'End If

#If RF Then
                        'RECONNECT - Checking whether UOD is scanned
                        objAppContainer.m_ModScreen = AppContainer.ModScreen.ITEMSCAN

#End If

                        'Getting the details of the scanned item
                        If Not (objAppContainer.objDataEngine.GetItemDetails(strBarcode, AppContainer.ItemDetailType.UOD, m_AUODItemInfo)) Then
#If RF Then
                            'TIMEOUT - if timeout do not do any processing
                            If objAppContainer.bTimeOut Then
                                Exit Sub
                            End If
                            'RECONNECT

                            If Not objAppContainer.bCommFailure And Not objAppContainer.bReconnectSuccess Then
#End If
                                'Item not in file
                                iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M67"), "Alert ", MessageBoxButtons.OK, _
                                             MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                                DisplayAUODScreen(AUODSCREENS.AuditItem)
#If RF Then
                            End If
                            objAppContainer.bReconnectSuccess = False
#End If

                            Else

                                'checks whether the item is previously scanned and if yes sets the item quantity
                                For Each objUODItem As GIValueHolder.ScanDetails In m_ItemList
                                If objUODItem.ProductCode = m_AUODItemInfo.ProductCode Or _
                                objUODItem.BootCode = m_AUODItemInfo.BootsCode Then
                                        m_AUODItemInfo.ItemQty = CInt(objUODItem.ItemQty)
                                        m_strScanCheck = True
                                        Exit For
                                    End If
                                Next
                                DisplayAUODScreen(AUODSCREENS.AuditItemDetails)
                            End If
                    End If
                End If
            End If
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit UOD Handle Data Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try

    End Sub
    ' V1.1 - CK
    ' Added new function DisplayAuditScreen

    ''' <summary>
    ''' The Method calls DisplayAUODScreen to switch the screen to
    ''' Audit screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayAuditScreen()
        DisplayAUODScreen(AUODSCREENS.Audit)
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
        Try
#If RF Then
            If objAppContainer.bReconnectSuccess Then
                objAppContainer.bReconnectSuccess = False
            End If
#End If

            ' V1.1 - CK
            ' For stores enabled for dallas positive receiving, if dallas barcode is scanned then
            ' show message
            If objAppContainer.bDallasPosReceiptEnabled Then
                If strBarcode.StartsWith("0501") And strBarcode.Length = 14 Then
#If RF Then
                    DisplayMsgBox(MessageManager.GetInstance().GetMessage("M137"), "AUDIT NOT AVAILABLE", MsgBx.BUTTON_TYPE.CONTINE, 9)
#End If
#If NRF Then

                    MsgBx.DisplayMessage(MessageManager.GetInstance().GetMessage("M137"), _
                                         "AUDIT NOT AVAILABLE", MsgBx.BUTTON_TYPE.CONTINE)
                    m_GInAudit.txtProductCode.Text = ""
#End If

                    Exit Sub
                End If
            End If

            m_GInAuditItem.FreezeControls()
            m_BCType = Type
            Select Case Type
                Case BCType.I2O5
                    HandleData(strBarcode)
                Case BCType.ManualEntry
                    HandleData(strBarcode)
                Case BCType.EAN
                    HandleData(strBarcode)
                Case BCType.CODE128
                    HandleData(strBarcode)
                Case BCType.UPC
                    If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINAUDITITEM Then
                        HandleData(strBarcode)
                    Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                                                                                                          MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                        m_GInAuditItem.UnFreezeControls()
                        Exit Sub
                    End If
                Case Else
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                                                                                    MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                    m_GInAuditItem.UnFreezeControls()
                    Exit Sub
            End Select
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit UOD Handle Scan Data Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        Finally
            If Not m_bUnfreeze Then
#If RF Then
                If Not objAppContainer.bCommFailure Then
                    m_GInAuditItem.UnFreezeControls()
                End If
#ElseIf NRF Then
                m_GInAuditItem.UnFreezeControls()

#End If


            End If
            m_bUnfreeze = False
        End Try

    End Sub

    ''' <summary>
    ''' Displays the screen to scan UOD
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayAUODScan(ByVal o As Object, ByVal e As EventArgs)
        Try
            objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINAUDIT
            objAppContainer.objStatusBar.SetMessage("")
            With m_GInAudit
                'To display status bar message
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                .Visible = True
                '.objProduct.txtProductCode.Text = ""
                .txtProductCode.Text = ""
                .txtProductCode.Focus()
                .Refresh()
            End With
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit UOD Scan Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Displays the screen to scan item
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayAuditItem(ByVal o As Object, ByVal e As EventArgs)
        Try
            'To display status bar message
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINAUDITITEM
            objAppContainer.objStatusBar.SetMessage("")
            With m_GInAuditItem
                .Visible = True
                .txtProductCode.Text = ""
                .txtProductCode.Focus()
                'If m_ItemList.Count > 0 Then
                '    .Btn_Finish.Visible = True
                'Else

                '    .Btn_Finish.Visible = False
                'End If
                '.objProduct.txtProductCode.Text = ""
                .Refresh()
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            End With
            'Check if there is any data store in global array due to reconnect.
            Try
#If RF Then


                If bIsNewSessionAfterConnectionLoss And objAppContainer.m_SavedDetails.Count > 0 And _
                objAppContainer.eDeliveryType = AppContainer.DeliveryType.SSC And _
                objAppContainer.eFunctionType = AppContainer.FunctionType.Audit Then
                    Dim objCartonItem As GIValueHolder.ScanDetails = objAppContainer.m_SavedDetails(0)
                    If objCartonItem.ScannedCode = m_objUOD.strUODNumber.Substring(0, 10) Then
                        m_ItemList = objAppContainer.m_SavedDetails
                        m_objUOD.iCountItems = m_ItemList.Count
                    Else
                        objAppContainer.eDeliveryType = AppContainer.DeliveryType.SSC
                        objAppContainer.eFunctionType = AppContainer.FunctionType.Audit
                        objAppContainer.m_SavedDetails = New ArrayList()
                    End If
                Else
                    objAppContainer.eDeliveryType = AppContainer.DeliveryType.SSC
                    objAppContainer.eFunctionType = AppContainer.FunctionType.Audit
                    objAppContainer.m_SavedDetails = New ArrayList()
                End If
                'Set the boolean session state to false.
                bIsNewSessionAfterConnectionLoss = False
#End If
            Catch ex As Exception
                AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit UOD Display UOD Item Scan: " + ex.Message + ex.ToString(), _
                                                                    Logger.LogLevel.RELEASE)
            End Try
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit UOD Display Item Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Displays the Item Details screen 
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayAuditItemDetails(ByVal o As Object, ByVal e As EventArgs)
        Try
            objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINAUDITITEMDETAILS
            'To display the item description in 3 lines
            Dim objDescriptionArray As ArrayList = New ArrayList
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(m_AUODItemInfo.ItemDesc)

            With m_GInAuditItemDetails

                'To display status bar message
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                .lblUOD.Text = m_objUOD.strUODNumber.Substring(0, 10)
                .lblPrdt.Text = objDescriptionArray.Item(0).ToString()
                .lblItem2.Text = objDescriptionArray.Item(1).ToString()
                .lblItem3.Text = objDescriptionArray.Item(2).ToString()
                If m_AUODItemInfo.BootsCode.Length <= 7 Then
                .lblBootsCode.Text = objAppContainer.objHelper.FormatBarcode(m_AUODItemInfo.BootsCode)
                Else
                    .lblBootsCode.Text = ""
                End If

                If m_AUODItemInfo.ProductCode.Length <= 7 Then
                    .lblPrdtCode.Text = ""
                Else
                .lblPrdtCode.Text = objAppContainer.objHelper.FormatBarcode(m_AUODItemInfo.ProductCode)
                End If
                'If the item is previously scanned then the corresponding quantity is displayed
                'else the quantity is prepopulated with value 1
                If m_strScanCheck Then
                    .lblQty.Text = m_AUODItemInfo.ItemQty.ToString()
                    m_strScanCheck = False
                Else
                    .lblQty.Text = Quantity.One
                End If
                objAppContainer.objStatusBar.SetMessage("")
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit UOD Item Details Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Displays the Audit Summary screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayAuditSummary(ByVal o As Object, ByVal e As EventArgs)
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
            objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINAUDITSUMMARY
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            With m_GInAuditSummary
                .lblContainerUOD.Text = m_objUOD.strUODNumber.Substring(0, 10)
                .lblNo.Text = m_objUOD.iCountItems.ToString()
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit UOD Summary Screen Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ' V1.1 - CK
    ' Added new function DisplayMsgBox

    ''' <summary>
    ''' Custom message box called by BDSessionMgr.
    ''' </summary>
    ''' <param name="cText"></param>
    ''' <param name="cCaption"></param>
    ''' <param name="btnType"></param>
    ''' <param name="iMessageType"></param>
    ''' <remarks></remarks>
    Public Function DisplayMsgBox(ByVal cText As String, ByVal cCaption As String, _
                                  ByVal btnType As MsgBx.BUTTON_TYPE, Optional ByVal iMessageType As Integer = 0) As Boolean
        Try
            m_MsgBox.MsgBoxInitialize(cText, cCaption, btnType, iMessageType)
            m_MsgBox.Invoke(New EventHandler(AddressOf DisplayMsgBoxScreen))
        Catch ex As Exception
            Return False

        End Try
        Return True

    End Function
    ' V1.1 - CK
    ' Added new function DisplayMsgBoxScreen

    ''' <summary>
    ''' Custom message box shown
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub DisplayMsgBoxScreen(ByVal o As Object, ByVal e As EventArgs)
        With m_MsgBox
            .Show()
            ' .Visible = True
            .Refresh()
        End With
    End Sub


    ''' <summary>
    ''' To quit the session without saving anything
    ''' </summary>
    ''' <remarks></remarks>

    Public Sub QuitSession()
        Dim iResult As Integer = 0
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M84"), "Confirmation", MessageBoxButtons.YesNo, _
                                      MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
            If iResult = MsgBoxResult.Yes Then
                m_AUODSessionMgr.EndSession(AppContainer.IsAbort.Yes)
                'objAppContainer.objSSCReceivingMainMenu.Hide()
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit UOD Quit Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To quit the session after saving all audits
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub FinalQuitSession()
        Dim iResult As Integer = 0
        Try
            'Checking count is added in order to make the menu visible in case No items are scanned
            ' If Not objAppContainer.strDeviceType = Macros.RF Then
#If NRF Then
                'For Non Rf Device
                iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M85"), "Alert", MessageBoxButtons.OK, _
                                      MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                If iResult = MsgBoxResult.Ok Then
                    If m_objUOD.iCountItems.ToString() Then
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                        m_AUODSessionMgr.EndSession(AppContainer.IsAbort.No)
                    Else
                        m_AUODSessionMgr.EndSession(AppContainer.IsAbort.Yes)
                    End If
                End If
#ElseIf RF Then
            If m_objUOD.iCountItems.ToString() Then
                m_AUODSessionMgr.EndSession(AppContainer.IsAbort.No)
            Else
                m_AUODSessionMgr.EndSession(AppContainer.IsAbort.Yes)
            End If
#End If

            '   Else

            '  End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit UOD Final Quit Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Finishing the session
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub FinishSession()
        Dim iResult As Integer = 0
        Try

            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M111"), "Alert", MessageBoxButtons.OKCancel, _
                                      MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            If iResult = MsgBoxResult.OK Then
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
                'sending the item details as export data
                If m_ItemList.Count > 0 Then
                    iItemCount = m_ItemList.Count
#If RF Then
                    'RECONNECT
                    objAppContainer.m_ModScreen = AppContainer.ModScreen.POSTFINISH
                    If objAppContainer.objDataEngine.SendItemDetails(m_ItemList, AppContainer.DeliveryType.SSC, AppContainer.FunctionType.Audit) Then
                        m_ItemList.Clear()
                        objAppContainer.objDataEngine.SendItemQuantity(iItemCount, AppContainer.DeliveryType.SSC, AppContainer.FunctionType.Audit)
                    End If
#ElseIf NRF Then
                    objAppContainer.objDataEngine.SendItemDetails(m_ItemList, AppContainer.DeliveryType.SSC, AppContainer.FunctionType.Audit)
                    objAppContainer.objDataEngine.SendItemQuantity(m_ItemList.Count, AppContainer.DeliveryType.SSC, AppContainer.FunctionType.Audit)
#End If

                End If
#If RF Then
                'RECONNECT
                If Not objAppContainer.bCommFailure AndAlso Not objAppContainer.bReconnectSuccess Then
                    DisplayAUODScreen(AUODSessionManager.AUODSCREENS.AuditSummary)
                End If
                objAppContainer.bReconnectSuccess = False

#ElseIf NRF Then
                 DisplayAUODScreen(AUODSessionManager.AUODSCREENS.AuditSummary)
#End If

            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Audit UOD Finish Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try

    End Sub

    ''' <summary>
    ''' Screens in Audit UOD
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum AUODSCREENS
        Audit
        AuditItem
        AuditItemDetails
        AuditSummary

    End Enum
End Class
''' <summary>
''' Class to maintain the count of items scanned
''' </summary>
''' <remarks></remarks>
Public Class UOD
    Public strUODNumber As String
    Public iCountItems As Integer
End Class

