Imports System.Threading
Public Class MsgBx
    Private Shared Returnvalue As ReturnData
    Private Shared frmMessage As MsgBx
    'Private Shared Waiter As AutoResetEvent
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Private Sub btnContinue_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnContinue.Click
        Returnvalue = ReturnData.ContinueRecall
        'If Not Waiter Is Nothing Then
        '    Waiter.Reset()
        'End If
        frmMessage.Close()
    End Sub

    Private Sub btnquit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnquit.Click
        Returnvalue = ReturnData.QuitRecall
        'If Not Waiter Is Nothing Then
        '    Waiter.Reset()
        'End If
        frmMessage.Close()
    End Sub

    Public Enum ReturnData As Integer
        ContinueRecall = 0
        QuitRecall = 1
    End Enum
    Public Shared Function ShowMessageDialog(ByVal Message As String) As ReturnData
        frmMessage = New MsgBx
        frmMessage.Location = New System.Drawing.Point(10 * objAppContainer.iOffSet, 105 * objAppContainer.iOffSet)
        frmMessage.lblMsg.Text = Message
        frmMessage.Visible = True

        frmMessage.ShowDialog()

        frmMessage.Dispose()
        'Waiter = New AutoResetEvent(False)
        'Waiter.WaitOne()
        Return Returnvalue
    End Function
    Public Shared Sub CloseMessage()
        frmMessage.Close()
        frmMessage.Dispose()

    End Sub

    Public Shared Function ShowMessage(ByVal Message As String) As ReturnData
        frmMessage = New MsgBx
        'frmMessage.Panel1.Size = New Size(218, 58)
        frmMessage.Panel1.Size = New Size(210 * objAppContainer.iOffSet, 58 * objAppContainer.iOffSet)
        frmMessage.Panel1.Location = New System.Drawing.Point(1, 1)
        'frmMessage.Location = New System.Drawing.Point(10, 105)
        frmMessage.Location = New System.Drawing.Point(13, 105)
        'frmMessage.Size = New Size(220, 60)
        frmMessage.Size = New Size(212 * objAppContainer.iOffSet, 60 * objAppContainer.iOffSet)
        For Each Control As Control In frmMessage.Controls
            If TypeOf (Control) Is Button Then
                frmMessage.Controls.Remove(Control)
            End If
        Next
        frmMessage.lblMsg.Text = Message

        frmMessage.btnContinue.Visible = False
        frmMessage.btnQuit.Visible = False
        frmMessage.Visible = True

        frmMessage.Show()

        'frmMessage.Dispose()
        'Waiter = New AutoResetEvent(False)
        'Waiter.WaitOne()
        Return Returnvalue
    End Function

End Class