Public Class frmUserLogon

    Private Const MAX_LENGTH As Integer = 6 'Max length of password

    Private Sub pbExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbExit.Click
        Me.Close()
    End Sub

    Private Sub pbDel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbDel.Click
        If Len(txtSignOn.Text) <> 0 Then
            txtSignOn.Text = Strings.Left(txtSignOn.Text, (Len(txtSignOn.Text) - 1))
        End If
    End Sub
    Private Sub pbClr_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbClr.Click
        txtSignOn.Text = ""
    End Sub
    Private Sub pbEnter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbEnter.Click
        pbEnter.Enabled = False
        If UserSession.GetInstance.ValidateUser(txtSignOn.Text) = False Then
            txtSignOn.Text = ""
            pbEnter.Enabled = True
        Else
            Me.Close()
        End If
    End Sub

    Private Sub inputText(ByVal value As String)
        'Checks Whether the length is not equal to maximum length
        If Len(txtSignOn.Text) < MAX_LENGTH Then
            txtSignOn.Text += value
        End If
    End Sub

    Private Sub pbZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbZero.Click
        inputText("0")
    End Sub

    Private Sub pbOne_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbOne.Click
        inputText("1")
    End Sub

    Private Sub pbTwo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbTwo.Click
        inputText("2")
    End Sub

    Private Sub pbThree_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbThree.Click
        inputText("3")
    End Sub

    Private Sub pbFour_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbFour.Click
        inputText("4")
    End Sub

    Private Sub pbFive_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbFive.Click
        inputText("5")
    End Sub

    Private Sub pbSix_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSix.Click
        inputText("6")
    End Sub

    Private Sub pbSeven_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSeven.Click
        inputText("7")
    End Sub

    Private Sub pbEight_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbEight.Click
        inputText("8")
    End Sub

    Private Sub pbNine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNine.Click
        inputText("9")
    End Sub

    Private Sub pbHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbHelp.Click
        MessageBox.Show(MessageManager.GetInstance.GetMessage("M18"), "Help")
    End Sub

End Class