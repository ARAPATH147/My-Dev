Public Class frmBookInOrder

    Private Sub frmBookInOrder_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StartRead()
    End Sub

    Private Sub frmBookInOrder_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub

    Private Sub Btn_Finish1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Finish1.Click
        ParcelSession.GetInstance.closeSession()
    End Sub

    Private Sub btnCalcPad1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCalcPad1.Click
        AppMain.displayCalcPadScreen(AppMain.CALCPADUSE.ASNBARCODE)
    End Sub

    Private Sub pbHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbHelp.Click
        MessageBox.Show(MessageManager.GetInstance.GetMessage("M21"), "Help")
    End Sub

    
End Class