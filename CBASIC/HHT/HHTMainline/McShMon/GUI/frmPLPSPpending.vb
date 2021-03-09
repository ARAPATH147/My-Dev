Public Class frmPLPSPpending


    Private Sub custCtrlBtnYes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnYes.Click
        'PLSessionMgr.GetInstance().ProcessPSPPendingYes()
        'PLSessionMgr.GetInstance().PSPPendingQuit() = True
        PLSessionMgr.GetInstance().DisplayPLScreen(PLSessionMgr.PLSCREENS.Finish)
    End Sub

    Private Sub btnView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnView.Click
        Try
            'FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            PLSessionMgr.GetInstance().DisplayPLScreen(PLSessionMgr.PLSCREENS.ItemView)

            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

            'UnFreezeControls()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub custCtrlBtnNo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnNo.Click
        'Me.Visible = False
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            PLSessionMgr.GetInstance().ProcessCancelPSPPending()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try

    End Sub

    Private Sub Btn_Help_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Help.Click
        MessageBox.Show("YES= Selecting YES will exit you from the picking list as Pending Sales Plan sites are not compulsory" & ControlChars.CrLf & ControlChars.CrLf & _
                        "VIEW= Selecting VIEW will display the status of all items in the picking list" & ControlChars.CrLf & ControlChars.CrLf & _
                        "NO= Selecting NO will return the user to the first item which has not been completed" & ControlChars.CrLf & _
                                 ControlChars.CrLf, "Picking List Help")
    End Sub

    Private Sub Label1_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label1.ParentChanged

    End Sub
End Class