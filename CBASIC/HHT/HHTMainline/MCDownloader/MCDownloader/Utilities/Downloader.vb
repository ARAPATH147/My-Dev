Public Class Downloader
    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        Timer.Interval = 550
        Timer.Enabled = True
        ' Add any initialization after the InitializeComponent() call.
    End Sub
    Private Sub Timer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer.Tick
        If (Timer.Interval = 550) Then
            pbArrow.Visible = False
            pbArrow.Refresh()
            Timer.Interval = 500
        Else
            pbArrow.Visible = True
            pbArrow.Refresh()
            Timer.Interval = 550
        End If
    End Sub
End Class
