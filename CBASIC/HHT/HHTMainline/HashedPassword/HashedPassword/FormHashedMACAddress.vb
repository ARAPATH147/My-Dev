Imports System
Public Class ClsHashedPassword

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        txtMACAddress.Text = ""
        txtMACAddress.MaxLength = 17
        txtMACAddress.SelectionLength = Len(txtMACAddress.Text)
        txtPassword.Visible = False
        lblPass.Visible = False
        txtPassword.Text = ""
        'If txtMACAddress.SelectionLength > 17 Then
        '    MessageBox.Show("test")

        'End If
    End Sub


    'Private Sub txtMACAddress_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtMACAddress.KeyPress

    '    e.Handled = Not (Char.IsDigit(e.KeyChar) Or Asc(e.KeyChar) < 32)
    '    MessageBox.Show("Allowed values not more than 18 characters")
    'End Sub

    ' commenting the following function  for checking the Text Changed events
    'Private Sub txtMACAddress_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtMACAddress.KeyPress
    '    txtMACAddress.MaxLength = 17
    '    If Len(txtMACAddress.Text) = txtMACAddress.MaxLength Then
    '        txtMACAddress.Text = ""
    '        MessageBox.Show("Allowed values not more than 18 characters")
    '        txtMACAddress.SelectAll()

    '        txtMACAddress.Text = ""

    '        txtMACAddress.SelectionStart = 0
    '        ' txtMACAddress.Focus()

    '    End If
    'End Sub




    '''' <summary>
    '''' Generating Hashed Password
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    '''' <remarks></remarks>
    'Private Sub pbSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSave.Click


    '    'Dim macid As String
    '    'macid = txtMACAddress.Text

    '    'If macid = String.Empty Or macid.Length = 0 Or String.IsNullOrEmpty(macid) Or Trim(macid) = Nothing Then
    '    '    MessageBox.Show("Please enter MAC Address")
    '    '    txtMACAddress.Focus()

    '    'Else


    '    '    If macid.Length > 1 Then
    '    '        Try


    '    '            Using objHashedMACAddress As ClassHashedMACAddress = New ClassHashedMACAddress

    '    '                objHashedMACAddress.CreateHashedPassword(macid)

    '    '            End Using
    '    '        Catch ex As Exception
    '    '            MessageBox.Show("Error in generating Hashed password", "Error Occured")
    '    '        End Try



    '    '    Else
    '    '        MessageBox.Show("Please Verify the MAC ID.", "Enter MAC ID")
    '    '    End If

    '    'End If
    '    'exit 
    'End Sub

#Region "Lock"
    '    ''' <summary>
    '    ''' Check Whether the Text box has 4 numbers and if so disable the Buttons
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Private Sub CheckToLockKeys()
    '        txtStoreNumber.SelectionStart = txtStoreNumber.Text.Length
    '        If (txtStoreNumber.Text.Length = 4) Then
    '            LockButtons(True)
    '        End If
    '    End Sub
    ''' <summary>
    ''' Lock/Unloack the buttons
    ''' </summary>
    ''' <param name="Has_To_BE_Locked">True to Disable the button and False to Enable</param>
    ''' <remarks></remarks>
    Private Sub LockButtons(ByVal Has_To_BE_Locked As Boolean)
        If Has_To_BE_Locked Then
            '    btnOne.Enabled = False
            '    btnTwo.Enabled = False
            '    btnThree.Enabled = False
            '    btnFour.Enabled = False
            '    btnFive.Enabled = False
            '    btnSix.Enabled = False
            '    btnSeven.Enabled = False
            '    btnEight.Enabled = False
            '    btnNine.Enabled = False
            '    btnZero.Enabled = False
            'Else
            '    btnOne.Enabled = True
            '    btnTwo.Enabled = True
            '    btnThree.Enabled = True
            '    btnFour.Enabled = True
            '    btnFive.Enabled = True
            '    btnSix.Enabled = True
            '    btnSeven.Enabled = True
            '    btnEight.Enabled = True
            '    btnNine.Enabled = True
            '    btnZero.Enabled = True
        End If
    End Sub
#End Region

    Private Sub WLANSetup_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        If (e.KeyCode = System.Windows.Forms.Keys.Up) Then
            'Up
        End If
        If (e.KeyCode = System.Windows.Forms.Keys.Down) Then
            'Down
        End If
        If (e.KeyCode = System.Windows.Forms.Keys.Left) Then
            'Left
        End If
        If (e.KeyCode = System.Windows.Forms.Keys.Right) Then
            'Right
        End If
        If (e.KeyCode = System.Windows.Forms.Keys.Enter) Then
            'Enter
        End If

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub btnDel_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If txtMACAddress.Text.Length > 1 Then
            txtMACAddress.Text = txtMACAddress.Text.Substring(0, txtMACAddress.Text.Length - 1)
            txtMACAddress.SelectionStart = txtMACAddress.Text.Length
        ElseIf txtMACAddress.Text.Length = 1 Then
            txtMACAddress.Text = ""
            txtMACAddress.SelectionStart = 0
        End If
        LockButtons(False)
    End Sub
    ''' <summary>
    ''' To quit from the form.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Dispose()
    End Sub


    Private Sub btnHashedPassword_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHashedPassword.Click

        Dim macid As String

        macid = txtMACAddress.Text

        


        If macid = String.Empty Or macid.Length = 0 Or String.IsNullOrEmpty(macid) Or Trim(macid) = Nothing Then
            MessageBox.Show("Please enter MAC Address")
            txtMACAddress.Focus()

        Else

            If macid.Length > 1 Then
                
                Try
                    Dim objHashedMACAddress As ClassHashedMACAddress = New ClassHashedMACAddress

                    txtPassword.Text = objHashedMACAddress.CreateHashedPassword(macid)
                    lblPass.Visible = True
                    txtPassword.Visible = True

                Catch ex As Exception
                    MessageBox.Show("Error in generating Hashed password", "Error Occured")
                End Try

            Else
                MessageBox.Show("Please Verify the MAC ID.", "Enter MAC ID")
            End If

        End If
    End Sub

    ''' <summary>
    ''' To quit from the form.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Me.Dispose()
    End Sub
    'for deleting the content
    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If txtMACAddress.Text.Length > 1 Then
            txtMACAddress.Text = txtMACAddress.Text.Substring(0, txtMACAddress.Text.Length - 1)
            txtMACAddress.SelectionStart = txtMACAddress.Text.Length
        ElseIf txtMACAddress.Text.Length = 1 Then
            txtMACAddress.Text = ""
            txtMACAddress.SelectionStart = 0
        End If
    End Sub

    
    Private Sub txtMACAddress_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtMACAddress.TextChanged
        Dim macHash As String

        macHash = txtMACAddress.Text
        If macHash.Contains(":") Then
            txtMACAddress.MaxLength = 17
        Else
            If Not macHash.Contains(":") Then
                txtMACAddress.MaxLength = 12
            End If
        End If
    End Sub
End Class
