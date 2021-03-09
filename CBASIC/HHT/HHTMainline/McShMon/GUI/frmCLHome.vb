Public Class frmCLHome
    Private Sub custCtrlBtnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnQuit.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            'SFA DEF #812 - Commenting for flow change 
            'If CLSessionMgr.GetInstance().ProcessQuitListsScreen() Then
            If CLSessionMgr.GetInstance().EndSession() Then
                Me.Visible = False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        Finally
            UnFreezeControls()
        End Try
    End Sub
    Private Sub Info_button_i1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button_i1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.CUNTLIST)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#If NRF Then
        UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
   
    Private Sub Btn_Help_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Help.Click
        Try
            FreezeControls()
            MessageBox.Show("SO   - Support Office" & ControlChars.CrLf & ControlChars.CrLf & _
                      "NEG - Negative" & ControlChars.CrLf & ControlChars.CrLf & _
                      "U    - User Generated" & ControlChars.CrLf & ControlChars.CrLf & _
                                  ControlChars.CrLf, "Count List Type Help")
           
#If NRF Then
        UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
    'IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.custCtrlBtnQuit.Enabled = False
            Me.Btn_Help.Enabled = False
            Me.Info_button_i1.Enabled = False
            Me.btn_CreateCountList.Enabled = False
            'Me.lstvwProductGroup.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception Occured @ Freeze Controls " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' UnFreeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnFreezeControls()
        Try
            Me.custCtrlBtnQuit.Enabled = True
            Me.Btn_Help.Enabled = True
            Me.Info_button_i1.Enabled = True
            Me.btn_CreateCountList.Enabled = True
            'Me.lstvwProductGroup.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured @ Unfreeze Controls " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub lstvwProductGroup_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstvwProductGroup.ItemActivate
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            If CLSessionMgr.GetInstance().ProcessProductSelection() Then
                Me.Visible = False
            End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        Finally
            UnFreezeControls()
        End Try
    End Sub
    Private Sub btn_CreateCountList_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_CreateCountList.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            objAppContainer.objLogger.WriteAppLog("Create Own List button click", Logger.LogLevel.INFO)
            Try
                CLSessionMgr.GetInstance().StartSessionCOL()
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("Exception occured at Create Own List button click: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            End Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            objAppContainer.objLogger.WriteAppLog("Exit View button Session", Logger.LogLevel.INFO)
#If NRF Then
            UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
End Class