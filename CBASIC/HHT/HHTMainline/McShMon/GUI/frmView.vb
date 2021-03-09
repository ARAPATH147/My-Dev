Public Class frmView

    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            'System Testing - changed the check from AppContainer.ACTIVEMODULE.SHLFMNTR to objAppContainer.objActiveModule.Equals
            If objAppContainer.objActiveModule.Equals(AppContainer.ACTIVEMODULE.SHLFMNTR) Then
                'DEFECT FIX - BTCPR00004226 (PPC - Shelf Monitor - item details displayed - <Quit> 
                'from <View> should return to displaying same item details)
                If SMSessionMgr.GetInstance().PreviousScreen = SMSessionMgr.SMSCREENS.Home Then
                    SMSessionMgr.GetInstance().DisplaySMScreen(SMSessionMgr.SMSCREENS.Home)
                ElseIf SMSessionMgr.GetInstance().PreviousScreen = SMSessionMgr.SMSCREENS.ItemDetails Then
                    SMSessionMgr.GetInstance().DisplaySMScreen(SMSessionMgr.SMSCREENS.ItemDetails)
                End If
            ElseIf objAppContainer.objActiveModule.Equals(AppContainer.ACTIVEMODULE.EXCSSTCK) Then
                If EXSessionMgr.GetInstance().PreviousScreen = EXSessionMgr.EXSCREENS.Home Then
                    EXSessionMgr.GetInstance().DisplayEXScreen(EXSessionMgr.EXSCREENS.Home)
                ElseIf EXSessionMgr.GetInstance().PreviousScreen = EXSessionMgr.EXSCREENS.ItemDetails Then
                    EXSessionMgr.GetInstance().DisplayEXScreen(EXSessionMgr.EXSCREENS.ItemDetails, _
                                                               EXSessionMgr.GetInstance().m_ItemScreen, _
                                                               EXSessionMgr.GetInstance().m_SelectedIndex)
                End If
                Me.Visible = False
            ElseIf objAppContainer.objActiveModule.Equals(AppContainer.ACTIVEMODULE.FASTFILL) Then
                If FFSessionMgr.GetInstance().PreviousScreen = FFSessionMgr.FFSCREENS.Home Then
                    FFSessionMgr.GetInstance().DisplayFFScreen(FFSessionMgr.FFSCREENS.Home)
                ElseIf FFSessionMgr.GetInstance().PreviousScreen = FFSessionMgr.FFSCREENS.ItemDetails Then
                    FFSessionMgr.GetInstance().DisplayFFScreen(FFSessionMgr.FFSCREENS.ItemDetails)
                End If

                Me.Visible = False
            ElseIf objAppContainer.objActiveModule.Equals(AppContainer.ACTIVEMODULE.AUTOSTUFFYOURSHELVES) Then
                AutoSYSSessionManager.GetInstance().DisplayASYSScreen(AutoSYSSessionManager.ASYSSCREENS.ItemDetails)
            ElseIf objAppContainer.objActiveModule.Equals(AppContainer.ACTIVEMODULE.PICKGLST) Then
                'PLSessionMgr.GetInstance().DisplayPLScreen(PLSessionMgr.PLSCREENS.ItemConfirm)
                Me.Visible = False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub Info_button_i1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button_i1.Click
        Try
            'System Testing
#If RF Then
            FreezeControls()
#End If
            Dim objCurrActiveModule As AppContainer.ACTIVEMODULE
            objCurrActiveModule = objAppContainer.objActiveModule
            ItemInfoSessionMgr.GetInstance().StartSession(objCurrActiveModule)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
            Cursor.Current = Cursors.Default
#If RF Then
        Finally
            UnfreezeControls()
#End If
        End Try
    End Sub
    Private Sub lstView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstView.ItemActivate
        Try
            If objAppContainer.objActiveModule.Equals(AppContainer.ACTIVEMODULE.EXCSSTCK) Then
                EXSessionMgr.GetInstance().ProcessItemSelection()
                Me.Visible = False
            ElseIf objAppContainer.objActiveModule.Equals(AppContainer.ACTIVEMODULE.FASTFILL) Then
                FFSessionMgr.GetInstance().ProcessItemSelection()
                Me.Visible = False
            ElseIf objAppContainer.objActiveModule.Equals(AppContainer.ACTIVEMODULE.SHLFMNTR) Then
                SMSessionMgr.GetInstance.DisplayViewItem()
                Me.Visible = False
            ElseIf objAppContainer.objActiveModule.Equals(AppContainer.ACTIVEMODULE.AUTOSTUFFYOURSHELVES) Then
                AutoSYSSessionManager.GetInstance().DisplayViewItem()
                'AFF PL CR
            ElseIf objAppContainer.objActiveModule.Equals(AppContainer.ACTIVEMODULE.PICKGLST) Then
                PLSessionMgr.GetInstance().ProcessViewItemSelection(Me.lstView.FocusedItem.Index)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    'System Testing
    Private Sub frmView_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance.StopRead()
        'If AppContainer.ACTIVEMODULE.EXCSSTCK Then
        '    objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.EXCSSTCK
        'ElseIf AppContainer.ACTIVEMODULE.FASTFILL Then
        '    objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.FASTFILL
        'ElseIf AppContainer.ACTIVEMODULE.SHLFMNTR Then
        '    objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.SHLFMNTR
        'End If
    End Sub

    Private Sub frmView_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate

    End Sub
    'System Testing - End

    Private Sub Help1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Help1.Click
        Try
            If objAppContainer.objActiveModule.Equals(AppContainer.ACTIVEMODULE.PICKGLST) Then
                MessageBox.Show("MBS= Main back Shop. Main back shop counts are compulsory " & ControlChars.CrLf & _
                                "PSP= Pending Sales Plan site. " & ControlChars.CrLf & _
                                "N= Not counted " & ControlChars.CrLf & _
                                "Y= Counted" & ControlChars.CrLf & _
                                "NA= Item is not on a pending sales plan" & ControlChars.CrLf & _
                                 ControlChars.CrLf, "Picking List Help")
            ElseIf objAppContainer.objActiveModule.Equals(AppContainer.ACTIVEMODULE.EXCSSTCK) Then
                MessageBox.Show("MBS= Main back Shop. Main back shop counts are compulsory " & ControlChars.CrLf & _
                               "PSP= Pending Sales Plan site. " & ControlChars.CrLf & _
                               "N= Not counted " & ControlChars.CrLf & _
                               "Y= Counted" & ControlChars.CrLf & _
                               "NA= Item is not on a pending sales plan" & ControlChars.CrLf & _
                                ControlChars.CrLf, "Excess Stock Help")
            ElseIf objAppContainer.objActiveModule.Equals(AppContainer.ACTIVEMODULE.SHLFMNTR) Then
                MessageBox.Show("* - Not all Multisites are Counted", "Information", _
                                     MessageBoxButtons.OK, _
                                     MessageBoxIcon.Exclamation, _
                                     MessageBoxDefaultButton.Button2)
            End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub FreezeControls()
        Try
            Help1.Enabled = False
            lstView.Enabled = False
            btnQuit.Enabled = False
            Info_button_i1.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception Occured while unfreezing controls", Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub UnfreezeControls()
        Try
            Help1.Enabled = True
            lstView.Enabled = True
            btnQuit.Enabled = True
            Info_button_i1.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception Occured while unfreezing controls", Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class