Imports System
Imports System.IO
Public Class frmCalcPad
    Inherits System.Windows.Forms.Form

    'Public calcPad As CalcPadSessionMgr
    Private RefControl As Control
    Public NoofDigit As Integer
    Private EnumValue As CalcPadSessionMgr.EntryTypeEnum
    Public EnumCheckValue As Boolean = False
    Public MAX_PASSWORD As Integer  ' Maximum length of EAN
    Private m_EnumText As String = ""
    Public Sub New(ByRef ctrlrefValue As Control, ByVal EntryTypeTemp As CalcPadSessionMgr.EntryTypeEnum)

        ' This call is required by the Windows Form Designer.
        ' Add any initialization after the InitializeComponent() call.
        InitializeComponent()
        EnumValue = EntryTypeTemp
        If (EntryTypeTemp.ToString = "Quantity") Then
            EnumCheckValue = True
        End If
        'calcPad = New CalcPadSessionMgr
        CalcPadSessionMgr.GetInstance().cEntryType = EntryTypeTemp
        RefControl = ctrlrefValue
        tbValue.TextAlign = HorizontalAlignment.Left

        Select Case EntryTypeTemp
            Case CalcPadSessionMgr.EntryTypeEnum.Quantity
                'Maximum Value For Quantity Entry is 20
                MAX_PASSWORD = 20
                'Set the text to be displayed in the message erro box.
                m_EnumText = "Quantity"
                'Checks if there is value in the control. 
                'If there is value then append "+" sign.
                If ctrlrefValue.Text.Trim().Equals("") Then
                    tbValue.Text = ""
                    CalcPadSessionMgr.GetInstance().strAvailableData = False
                ElseIf ctrlrefValue.Text.Trim().Equals("0") Then
                    tbValue.Text = ""
                    CalcPadSessionMgr.GetInstance().strAvailableData = False
                Else
                    If ctrlrefValue.Text.Trim().Last.Equals("+") Then
                        tbValue.Text = ctrlrefValue.Text.Trim()
                        '  CalcPadSessionMgr.GetInstance().strAvailableData = Mid(ctrlrefValue.Text.Trim(), 1, ctrlrefValue.Text.Length)
                    Else
                        tbValue.Text = ctrlrefValue.Text + "+"
                        CalcPadSessionMgr.GetInstance().strAvailableData = True
                    End If
                End If
                tbValue.MaxLength = MAX_PASSWORD
            Case CalcPadSessionMgr.EntryTypeEnum.UOD
                'For UOD the Text Box Has to Be Empty
                tbValue.Text = ""
                'Set the text to be displayed in the message erro box.
                m_EnumText = "UOD"
                'The Maximum Value That A UOD Can Have is 14
                MAX_PASSWORD = 14
                tbValue.MaxLength = MAX_PASSWORD
            Case CalcPadSessionMgr.EntryTypeEnum.Barcode
                'for Bar Code  The Text Box Has To BE Empty
                tbValue.Text = ""
                'Set the text to be displayed in the message erro box.
                m_EnumText = "Barcode"
                'The MAximum Value That a Barcode can Have is 13
                MAX_PASSWORD = 13
                tbValue.MaxLength = MAX_PASSWORD
            Case CalcPadSessionMgr.EntryTypeEnum.Authorization_Id
                'for Auth id entry  The Text Box Has To BE Empty
                tbValue.Text = ""
                'Set the text to be displayed in the message erro box.
                m_EnumText = "Authorization ID"
                'The MAximum Value That a Barcode can Have is 13
                MAX_PASSWORD = 8
                tbValue.MaxLength = MAX_PASSWORD
            Case CalcPadSessionMgr.EntryTypeEnum.Destination_Store_Id
                'for Store Entry  The Text Box Has To BE Empty
                tbValue.Text = ""
                'Set the text to be displayed in the message erro box.
                m_EnumText = "Destination Store ID"
                'The MAximum Value That a Barcode can Have is 13
                MAX_PASSWORD = 4
                tbValue.MaxLength = MAX_PASSWORD
            Case CalcPadSessionMgr.EntryTypeEnum.Recall_Id
                'for Auth id entry  The Text Box Has To BE Empty
                tbValue.Text = ""
                'Set the text to be displayed in the message erro box.
                m_EnumText = "Recall ID"
                'The MAximum Value That a Barcode can Have is 13
                MAX_PASSWORD = 8
                tbValue.MaxLength = MAX_PASSWORD
        End Select
        'Integration Testing- Removed to avoid the crash in keyboard dll
        'KeyBoard.GetInstance().KeyBoard_NumericMode()

        tbValue.SelectionStart = tbValue.TextLength
    End Sub


    Private Sub pbZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbZero.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "0"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The maximum length for " + m_EnumText + " entry is " + (MAX_PASSWORD.ToString) + " characters.", _
                            m_EnumText + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub pbOne_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbOne.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "1"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The maximum length for " + m_EnumText + " entry is " + (MAX_PASSWORD.ToString) + " characters.", _
                            m_EnumText + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub pbTwo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbTwo.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Text += "2"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The maximum length for " + m_EnumText + " entry is " + (MAX_PASSWORD.ToString) + " characters.", _
                            m_EnumText + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub pbThree_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbThree.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "3"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The maximum length for " + m_EnumText + " entry is " + (MAX_PASSWORD.ToString) + " characters.", _
                            m_EnumText + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub pbFour_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbFour.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "4"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The maximum length for " + m_EnumText + " entry is " + (MAX_PASSWORD.ToString) + " characters.", _
                            m_EnumText + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub pbFive_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbFive.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "5"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The maximum length for " + m_EnumText + " entry is " + (MAX_PASSWORD.ToString) + " characters.", _
                            m_EnumText + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub pbSix_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSix.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "6"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The maximum length for " + m_EnumText + " entry is " + (MAX_PASSWORD.ToString) + " characters.", _
                            m_EnumText + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub pbSeven_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSeven.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "7"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The maximum length for " + m_EnumText + " entry is " + (MAX_PASSWORD.ToString) + " characters.", _
                            m_EnumText + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub pbEight_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbEight.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "8"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The maximum length for " + m_EnumText + " entry is " + (MAX_PASSWORD.ToString) + " characters.", _
                            m_EnumText + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub pbNine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNine.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "9"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The maximum length for " + m_EnumText + " entry is " + (MAX_PASSWORD.ToString) + " characters.", _
                            m_EnumText + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub pbDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbDelete.Click
        tbValue.Focus()
        CalcPadSessionMgr.GetInstance().ProcessDelete(tbValue)
        tbValue.SelectionStart = Len(tbValue.Text)
    End Sub


    Private Sub pbQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbQuit.Click
        'RefControl.Text = ""
        DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub


    Private Sub pbPlus_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbPlus.Click
        If EnumCheckValue Then
            'Checks Whether the length is not equal to maximum length
            If Not (Len(tbValue.Text) < MAX_PASSWORD) Then
                MessageBox.Show("The maximum length for " + m_EnumText + " entry is " + (MAX_PASSWORD.ToString) + " characters.", _
                                m_EnumText + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            ElseIf Len(tbValue.Text) <> 0 Then
                tbValue.Focus()
                tbValue.Text += "+"
                tbValue.SelectionStart = Len(tbValue.Text)
            Else
                MessageBox.Show("Invalid operation", m_EnumText + " Entry", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)

            End If
        Else
            MessageBox.Show("Invalid operation", m_EnumText + " Entry", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
        End If
    End Sub
    Private Sub pbMinus_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbMinus.Click
        If EnumCheckValue Then
            If Not (Len(tbValue.Text) < MAX_PASSWORD) Then

                'Checks Whether the length is not equal to maximum length
                MessageBox.Show("The maximum length for " + m_EnumText + " entry is " + (MAX_PASSWORD.ToString) + " characters.", _
                                m_EnumText + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            ElseIf Len(tbValue.Text) <> 0 Then
                tbValue.Focus()
                tbValue.Text += "-"
                tbValue.SelectionStart = Len(tbValue.Text)
            Else
                MessageBox.Show("Invalid operation", m_EnumText + " Entry", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
            End If
        Else
            MessageBox.Show("Invalid operation", m_EnumText + " Entry", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
        End If
    End Sub
    Private Sub pbMultiply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbMultiply.Click
        If EnumCheckValue Then
            'Checks Whether the length is not equal to maximum length
            If Not (Len(tbValue.Text) < MAX_PASSWORD) Then
                MessageBox.Show("The maximum length for " + EnumValue.ToString + " entry is " + (MAX_PASSWORD.ToString) + " characters.", _
                                EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            ElseIf Len(tbValue.Text) <> 0 Then
                tbValue.Focus()
                tbValue.Text += "x"
                tbValue.SelectionStart = Len(tbValue.Text)
            Else
                MessageBox.Show("Invalid operation", m_EnumText + " Entry", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
            End If
        Else
            MessageBox.Show("Invalid operation", m_EnumText + " Entry", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
        End If

    End Sub


    Private Sub tbValue_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles tbValue.KeyDown
        If Not e.KeyValue = Keys.F23 Then
            If e.KeyValue = Keys.Enter Then
                If CalcPadSessionMgr.GetInstance().ProcessOK(tbValue) Then
                    If tbValue.Text.Length > 0 Then
                        Me.RefControl.Text = tbValue.Text.Trim
                    Else
                        Me.RefControl.Text = ""
                    End If
                    Me.Close()
                End If
            ElseIf tbValue.TextLength >= MAX_PASSWORD Then
                MessageBox.Show("The maximum length for " + m_EnumText + " entry is " + (MAX_PASSWORD.ToString) + " characters.", _
                                m_EnumText + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            End If
        End If

    End Sub

    Private Sub pbQMark_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbQMark.Click
        If EnumValue.ToString = "Barcode" Then
            MessageBox.Show("Use the calc pad to type in a Barcode. " & _
            "If you make a mistake press 'Del' to delete last key. To return without entering a barcode press 'Quit', or press 'Ok' to confirm.", "Help")
        ElseIf EnumValue.ToString = "Quantity" Then
            MessageBox.Show("Use the calc pad to type in a value. " & _
            "If you make a mistake press 'Del' to delete last key. To return without entering a value press 'Quit', or press 'Ok' to confirm.", "Help")
        ElseIf EnumValue.ToString = "UOD" Then
            MessageBox.Show("Use the calc pad to type in a UOD. " & _
            "If you make a mistake press 'Del' to delete last key. To return without entering a barcode press 'Quit', or press 'Ok' to confirm.", "Help")
        ElseIf EnumValue.ToString = "Authorization_Id" Then
            MessageBox.Show("Use the calc pad to type in a Authorisation Id. " & _
            "If you make a mistake press 'Del' to delete last key. To return without entering a Authorisation id press 'Quit', or press 'Ok' to confirm.", "Help")
        ElseIf EnumValue.ToString = "Destination_Store_Id" Then
            MessageBox.Show("Use the calc pad to type in a Destination Store Id. " & _
            "If you make a mistake press 'Del' to delete last key. To return without entering a Destination Store id press 'Quit', or press 'Ok' to confirm.", "Help")
        ElseIf EnumValue.ToString = "Recall_Id" Then
            MessageBox.Show("Use the calc pad to type in a Recall Id. " & _
            "If you make a mistake press 'Del' to delete last key. To return without entering a Recall id press 'Quit', or press 'Ok' to confirm.", "Help")
        End If
    End Sub

    Private Sub Btn_Ok1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Ok1.Click
        If CalcPadSessionMgr.GetInstance().ProcessOK(tbValue) Then
            If tbValue.Text.Length > 0 Then
                Me.RefControl.Text = tbValue.Text
            Else
                Me.RefControl.Text = ""
            End If
            DialogResult = Windows.Forms.DialogResult.OK
        End If
    End Sub
    ''' <summary>
    ''' Timer to use inorder to close the calc pad automatically before 
    ''' auto logoff.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub frmCalcPad_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Add any initialization after the InitializeComponent() call.
        Dim iInterval As Integer = CInt(ConfigDataMgr.GetInstance().GetParam(ConfigKey.AUTO_LOGOFF_TIMEOUT))
        'Convert minutes to milliseconds.
        tmrChecker.Interval = (iInterval * 60 * 1000) - 30000
        tmrChecker.Enabled = True
    End Sub
    ''' <summary>
    ''' Timer tick event handler to close the form.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub tmrChecker_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrChecker.Tick
        tmrChecker.Enabled = False
        'Close the form.
        Me.Close()
    End Sub

End Class


