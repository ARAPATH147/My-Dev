Public Class frmSelectLocation

    Private Sub frmSelectLocation_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        oAppMain.enActiveScreen = AppMain.ACTIVESCREEN.SELECTLOCATION
        BCReader.GetInstance().StartRead()
    End Sub

    Private Sub frmSelectLocation_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub

    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click

        'If oAppMain.enActiveModule = AppMain.ACTIVEMODULE.BOOKINWITHLOCATION Then
        'Prevent user leaving screen without entering a location in Book In and Put Away and Put Away/Move
        MessageBox.Show(MessageManager.GetInstance.GetMessage("M25"), "WARNING", _
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        ' Else
        'ParcelSession.GetInstance.CancelCurrentBookIn()
        'End If

    End Sub

    Private Sub btnCalcPad1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCalcPad1.Click
        AppMain.displayCalcPadScreen(AppMain.CALCPADUSE.LOCATIONBARCODE)
    End Sub

    Private Sub pbHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbHelp.Click
        MessageBox.Show(MessageManager.GetInstance.GetMessage("M22"), "Help")
    End Sub
End Class