Public Class frmMessage

    Public Delegate Sub SetMessageCallBack(ByVal Message As String)

    Public Sub SetMessage(ByVal Message As String)
        Try
            If Me.InvokeRequired Then
                Me.Invoke(New SetMessageCallBack(AddressOf SetMessage), Message)
            Else
                lblMsg.Text = Message
                Me.Refresh()
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception while Setting Message, Trace :" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class