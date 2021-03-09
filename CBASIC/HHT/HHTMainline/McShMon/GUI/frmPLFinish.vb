Public Class frmPLFinish
    Private Sub custCtrlBtnNo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnNo.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            PLSessionMgr.GetInstance().ProcessCancelFinish()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#If NRF Then
        UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnfreezeControls()
#End If
        End Try
    End Sub
    Private Sub custCtrlBtnYes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnYes.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
#If RF Then
            If Not (PLSessionMgr.GetInstance().CheckOSSRItem()) Then
                PLSessionMgr.GetInstance().ProcessFinish()
            End If
#ElseIf NRF Then
        PLSessionMgr.GetInstance().ProcessFinish()
#End If
            'IT External
            'PLSessionMgr.GetInstance().DisplayPLScreen(PLSessionMgr.PLSCREENS.Home)
            'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#If NRF Then
        UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnfreezeControls()
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
            Me.custCtrlBtnNo.Enabled = False
            Me.custCtrlBtnYes.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Unfreeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnfreezeControls()
        Try
            Me.custCtrlBtnNo.Enabled = True
            Me.custCtrlBtnYes.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub btnView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnView.Click
        PLSessionMgr.GetInstance().DisplayPLScreen(PLSessionMgr.PLSCREENS.ItemView, Macros.PL_FINISH)
    End Sub

    Private Sub Btn_Help_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Help.Click
        MessageBox.Show("YES= Selecting YES will exit you from the picking list " & ControlChars.CrLf & ControlChars.CrLf & _
                         "VIEW= Selecting VIEW will display the status of all items in the picking list" & ControlChars.CrLf & ControlChars.CrLf & _
                         "NO= Selecting NO will return the user to the first item which has not been completed" & ControlChars.CrLf & _
                                 ControlChars.CrLf, "Picking List Help")
    End Sub
End Class