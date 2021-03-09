Public Class Dummy_Form

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim scannerKill As BCReader = BCReader.GetInstance()
        scannerKill.TerminateBCReader()
    End Sub
End Class