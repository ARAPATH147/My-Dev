Public Class WLANSetup
    Private Sub btnOne_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOne.Click
        txtStoreNumber.Text = txtStoreNumber.Text + "1"
        CheckToLockKeys()
    End Sub

    Private Sub btnTwo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTwo.Click
        txtStoreNumber.Text = txtStoreNumber.Text + "2"
        CheckToLockKeys()
    End Sub

    Private Sub btnThree_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnThree.Click
        txtStoreNumber.Text = txtStoreNumber.Text + "3"
        CheckToLockKeys()
    End Sub

    Private Sub btnZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnZero.Click
        txtStoreNumber.Text = txtStoreNumber.Text + "0"
        CheckToLockKeys()
    End Sub

    Private Sub btnEight_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEight.Click
        txtStoreNumber.Text = txtStoreNumber.Text + "8"
        CheckToLockKeys()
    End Sub

    Private Sub btnNine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNine.Click
        txtStoreNumber.Text = txtStoreNumber.Text + "9"
        CheckToLockKeys()
    End Sub

    Private Sub btnSix_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSix.Click
        txtStoreNumber.Text = txtStoreNumber.Text + "6"
        CheckToLockKeys()
    End Sub

    Private Sub btnFive_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFive.Click
        txtStoreNumber.Text = txtStoreNumber.Text + "5"
        CheckToLockKeys()
    End Sub

    Private Sub btnFour_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFour.Click
        txtStoreNumber.Text = txtStoreNumber.Text + "4"
        CheckToLockKeys()
    End Sub

    Private Sub btnSeven_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSeven.Click
        txtStoreNumber.Text = txtStoreNumber.Text + "7"
        CheckToLockKeys()
    End Sub

    Private Sub btnDel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDel.Click
        If txtStoreNumber.Text.Length > 1 Then
            txtStoreNumber.Text = txtStoreNumber.Text.Substring(0, txtStoreNumber.Text.Length - 1)
            txtStoreNumber.SelectionStart = txtStoreNumber.Text.Length
        ElseIf txtStoreNumber.Text.Length = 1 Then
            txtStoreNumber.Text = ""
            txtStoreNumber.SelectionStart = 0
        End If
        LockButtons(False)
    End Sub
    ''' <summary>
    ''' To save the settings in the registry and enable connectivity to the Wi-Fi network.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSave.Click
        'Check Whether the 

        If txtStoreNumber.Text.Length = 4 Then
            Try
                Me.btnDel.Enabled = False
                Me.btnEight.Enabled = False
                Me.btnFive.Enabled = False
                Me.btnFour.Enabled = False
                Me.btnNine.Enabled = False
                Me.btnOne.Enabled = False
                Me.btnSeven.Enabled = False
                Me.btnSix.Enabled = False
                Me.btnThree.Enabled = False
                Me.btnTwo.Enabled = False
                Me.btnZero.Enabled = False
                Me.txtStoreNumber.Enabled = False
                Me.pbQuit.Enabled = False
                Me.pbSave.Enabled = False
                Me.rbStore.Enabled = False
                Me.rbStore.Enabled = False
                Me.rbTestLab.Enabled = False
                Me.rbOSSR.Enabled = False

                'Using objRadiusUtils As RadioUtils = New RadioUtils
                '    If rbStore.Checked Then
                '        objRadiusUtils.CreateRegistryFile(txtStoreNumber.Text, Macro.STORE_CONTROLLER)
                '    ElseIf rbOSSR.Checked Then
                '        objRadiusUtils.CreateRegistryFile(txtStoreNumber.Text, Macro.OSSR_STORE)
                '    ElseIf rbTestLab.Checked Then
                '        objRadiusUtils.CreateRegistryFile(txtStoreNumber.Text, Macro.TEST_LAB)
                '    End If
                'End Using
            Catch ex As Exception
                MessageBox.Show("Error While writing the RADIUS settings to the registry", "Error Occured")
            End Try


            Me.btnDel.Enabled = True
            Me.btnEight.Enabled = True
            Me.btnFive.Enabled = True
            Me.btnFour.Enabled = True
            Me.btnNine.Enabled = True
            Me.btnOne.Enabled = True
            Me.btnSeven.Enabled = True
            Me.btnSix.Enabled = True
            Me.btnThree.Enabled = True
            Me.btnTwo.Enabled = True
            Me.btnZero.Enabled = True
            Me.txtStoreNumber.Enabled = True
            Me.pbQuit.Enabled = True
            Me.pbSave.Enabled = True
            Me.rbStore.Enabled = True
            Me.rbStore.Enabled = True
            Me.rbTestLab.Enabled = True
            Me.rbOSSR.Enabled = True
        Else
            MessageBox.Show("Please Verify the Store ID. (Store ID should be in four digits)", "Enter Store ID")
        End If
        'exit 
    End Sub
    ''' <summary>
    ''' To quit from the form.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbQuit.Click
        Me.Dispose()
    End Sub
#Region "Lock"
    ''' <summary>
    ''' Check Whether the Text box has 4 numbers and if so disable the Buttons
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CheckToLockKeys()
        txtStoreNumber.SelectionStart = txtStoreNumber.Text.Length
        If (txtStoreNumber.Text.Length = 4) Then
            LockButtons(True)
        End If
    End Sub
    ''' <summary>
    ''' Lock/Unloack the buttons
    ''' </summary>
    ''' <param name="Has_To_BE_Locked">True to Disable the button and False to Enable</param>
    ''' <remarks></remarks>
    Private Sub LockButtons(ByVal Has_To_BE_Locked As Boolean)
        If Has_To_BE_Locked Then
            btnOne.Enabled = False
            btnTwo.Enabled = False
            btnThree.Enabled = False
            btnFour.Enabled = False
            btnFive.Enabled = False
            btnSix.Enabled = False
            btnSeven.Enabled = False
            btnEight.Enabled = False
            btnNine.Enabled = False
            btnZero.Enabled = False
        Else
            btnOne.Enabled = True
            btnTwo.Enabled = True
            btnThree.Enabled = True
            btnFour.Enabled = True
            btnFive.Enabled = True
            btnSix.Enabled = True
            btnSeven.Enabled = True
            btnEight.Enabled = True
            btnNine.Enabled = True
            btnZero.Enabled = True
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
End Class
