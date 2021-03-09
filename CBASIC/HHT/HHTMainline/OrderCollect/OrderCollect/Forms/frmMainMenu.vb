Public Class frmMainMenu

    Public oFrmBookIn As frmBookInOrder = Nothing

    Private Sub pbBookInOnly_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbBookInOnly.Click
        oAppMain.enActiveModule = AppMain.ACTIVEMODULE.BOOKINORDER
        ParcelSession.GetInstance().startSession()

    End Sub

    Private Sub pbBookInPutAway_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbBookInPutAway.Click
        oAppMain.enActiveModule = AppMain.ACTIVEMODULE.BOOKINWITHLOCATION
        ParcelSession.GetInstance().startSession()
        'oFrmBookIn = New frmBookInOrder
        'oFrmBookIn.Visible = True
    End Sub

    Private Sub pbLogOff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbLogOff.Click
        If oAppMain.bConnect = True Then
            RFDataManager.GetInstance.LogOff()
            ConnectionManager.GetInstance.Disconnect()
        End If
        Application.Exit()
    End Sub

    Private Sub pbPutWayMove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbPutWayMove.Click
        oAppMain.enActiveModule = AppMain.ACTIVEMODULE.MOVEPUTAWAY
        ParcelSession.GetInstance().startSession()
    End Sub

    Private Sub pbOrderCollect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbOrderCollect.Click
        oAppMain.enActiveModule = AppMain.ACTIVEMODULE.QUERYCOLLECT
        ParcelSession.GetInstance().StartSession()
    End Sub

End Class
