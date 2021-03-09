Public Class frmSMPLItemConfirm

    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
    Private Sub Info_button_i1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
    Private Sub custCtrlBtnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
    Private Sub custCtrlBtnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
    Private Sub custCtrlBtnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
    Private Sub frmSMPLItemConfirm_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StartRead()
    End Sub

    Private Sub frmSMPLItemConfirm_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub
End Class