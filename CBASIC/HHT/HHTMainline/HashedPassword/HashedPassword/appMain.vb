Module appMain
    Sub Main()
        Try
            Using objMainForm As New ClsHashedPassword
                objMainForm.ShowDialog()

            End Using
        Catch ex As Exception
            MessageBox.Show("Message:" + ex.Message, "Error")
        End Try
        Application.Exit()
    End Sub
End Module
