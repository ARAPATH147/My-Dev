
Public Class frmFileDataViewer
#If NRF Then
    Public m_Invokingform As String = Nothing

    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        If m_Invokingform = "ExportDataViewer" Then
            ExportDataViewer.getinstance.EndDisplay()
        ElseIf m_Invokingform = "LogFileMgr" Then
            LogFileMgr.GetInstance.EndDisplay()
        End If
    End Sub

    Private Sub frmFileDataViewer_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
    End Sub
#End If
End Class