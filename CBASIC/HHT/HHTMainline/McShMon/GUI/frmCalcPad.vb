Imports System
Imports System.IO
Public Class frmCalcPad
    Inherits System.Windows.Forms.Form
    'Declare required variable
    Private RefControl As Control
    Private EnumValue As CalcPadSessionMgr.EntryTypeEnum
    Private EnumCheckValue As Boolean = False
    Private MAX_PASSWORD As Integer  ' Maximum length of EAN
    Private iCurrentPrice As Integer ' To get the current price of the item in Pence.
    ''' <summary>
    ''' Constructor for Calc pad form class.
    ''' </summary>
    ''' <param name="ctrlrefValue"></param>
    ''' <param name="EntryTypeTemp"></param>
    ''' <remarks></remarks>
    Public Sub New(ByRef ctrlrefValue As Control, ByVal EntryTypeTemp As CalcPadSessionMgr.EntryTypeEnum, Optional ByVal iCurrentPrice As Integer = 0)
        ' This call is required by the Windows Form Designer.
        ' Add any initialization after the InitializeComponent() call.
        InitializeComponent()
        EnumValue = EntryTypeTemp
        If (EntryTypeTemp.ToString = "Quantity" Or EntryTypeTemp.ToString = "PrintQuantity" Or EntryTypeTemp.ToString = "TSF") Then
            EnumCheckValue = True
        End If
        'Get the current price of the item in case of Printing clearance label.
        iCurrentPrice = iCurrentPrice
        'calcPad = New CalcPadSessionMgr
        CalcPadSessionMgr.GetInstance().cEntryType = EntryTypeTemp
        RefControl = ctrlrefValue
        tbValue.TextAlign = HorizontalAlignment.Left
        Select Case EntryTypeTemp
            Case 0
                If EntryTypeTemp = CalcPadSessionMgr.EntryTypeEnum.Quantity Then
                    MAX_PASSWORD = 20
                ElseIf EntryTypeTemp = CalcPadSessionMgr.EntryTypeEnum.PrintQuantity Then
                    MAX_PASSWORD = 20
                End If
                'Maximum Value For Quantity Entry is 20

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
            Case 1
                'For UOD the Text Box Has to Be Empty
                tbValue.Text = ""
                'The Maximum Value That A UOD Can Have is 14
                MAX_PASSWORD = 14
            Case 2
                'for Bar Code  The Text Box Has To BE Empty
                tbValue.Text = ""
                'The MAximum Value That a Barcode can Have is 13
                MAX_PASSWORD = 13

            Case 3
                If EntryTypeTemp = CalcPadSessionMgr.EntryTypeEnum.Quantity Then
                    MAX_PASSWORD = 20
                ElseIf EntryTypeTemp = CalcPadSessionMgr.EntryTypeEnum.PrintQuantity Then
                    MAX_PASSWORD = 20
                End If
                'Maximum Value For Quantity Entry is 20

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
            Case 4
                'for Price the Text Box Has To BE Empty
                tbValue.Text = ""
                'The MAximum length of the Price digit.
                MAX_PASSWORD = 13
            Case 5
                MAX_PASSWORD = 20
                tbValue.Text = ""
                CalcPadSessionMgr.GetInstance().strAvailableData = True
        End Select
        'setting the maxmimum lenth of the text pad
        tbValue.MaxLength = MAX_PASSWORD
        tbValue.SelectionStart = tbValue.TextLength
    End Sub
    ''' <summary>
    ''' Handle 0 button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbZero.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "0"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub
    ''' <summary>
    ''' Handle 1 button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbOne_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbOne.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "1"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub
    ''' <summary>
    ''' Handle 2 button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbTwo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbTwo.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Text += "2"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub
    ''' <summary>
    ''' Handle 3 button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbThree_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbThree.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "3"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub
    ''' <summary>
    ''' Handle 4 button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbFour_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbFour.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "4"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub
    ''' <summary>
    ''' Handle 5 button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbFive_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbFive.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "5"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub
    ''' <summary>
    ''' Handle 6 button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbSix_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSix.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "6"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub
    ''' <summary>
    ''' Handle 7 button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbSeven_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSeven.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "7"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub
    ''' <summary>
    ''' Handle 8 button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbEight_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbEight.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "8"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub
    ''' <summary>
    ''' Handle 9 button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbNine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNine.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "9"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
    End Sub
    ''' <summary>
    ''' Handle Delete button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbDelete.Click
        tbValue.Focus()
        CalcPadSessionMgr.GetInstance().ProcessDelete(tbValue)
        tbValue.SelectionStart = Len(tbValue.Text)
    End Sub
    ''' <summary>
    ''' Handle Quit button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbQuit.Click
        'RefControl.Text = ""
        DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub
    ''' <summary>
    ''' Handle + button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbPlus_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbPlus.Click
        If EnumCheckValue Then
            'Checks Whether the length is not equal to maximum length
            If Not (Len(tbValue.Text) < MAX_PASSWORD) Then
                MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            ElseIf Len(tbValue.Text) <> 0 Then
                tbValue.Focus()
                tbValue.Text += "+"
                tbValue.SelectionStart = Len(tbValue.Text)
            Else
                MessageBox.Show("Invalid Operation", EnumValue.ToString + " Entry", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
            End If
        Else
            MessageBox.Show("Invalid Operation", EnumValue.ToString + " Entry", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
        End If
    End Sub
    ''' <summary>
    ''' Handle - button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbMinus_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbMinus.Click
        If EnumCheckValue Then
            If Not (Len(tbValue.Text) < MAX_PASSWORD) Then

                'Checks Whether the length is not equal to maximum length
                MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            ElseIf Len(tbValue.Text) <> 0 Then
                tbValue.Focus()
                tbValue.Text += "-"
                tbValue.SelectionStart = Len(tbValue.Text)
            Else
                MessageBox.Show("Invalid Operation", EnumValue.ToString + " Entry", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
            End If
        Else
            MessageBox.Show("Invalid Operation", EnumValue.ToString + " Entry", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
        End If
    End Sub
    ''' <summary>
    ''' Handle X button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbMultiply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbMultiply.Click
        If EnumCheckValue Then
            'Checks Whether the length is not equal to maximum length
            If Not (Len(tbValue.Text) < MAX_PASSWORD) Then
                MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            ElseIf Len(tbValue.Text) <> 0 Then
                tbValue.Focus()
                tbValue.Text += "x"
                tbValue.SelectionStart = Len(tbValue.Text)
            Else
                MessageBox.Show("Invalid Operation", EnumValue.ToString + " Entry", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
            End If
        Else
            MessageBox.Show("Invalid Operation", EnumValue.ToString + " Entry", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
        End If
    End Sub
    ''' <summary>
    ''' Handle key down event in calc pad.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub tbValue_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles tbValue.KeyDown
        If Not e.KeyValue = Keys.F23 Then
            If e.KeyValue = Keys.Enter Then
                If CalcPadSessionMgr.GetInstance().ProcessOK(tbValue) Then
                    If tbValue.Text.Length > 0 Then
                        'If EnumValue = CalcPadSessionMgr.EntryTypeEnum.ClearancePrice Then
                        '    CLRSessionMgr.GetInstance().UpdateClearancePrice(Convert.ToInt32(tbValue.Text))
                        'Else
                        Me.RefControl.Text = tbValue.Text
                        'End If
                        DialogResult = Windows.Forms.DialogResult.OK
                    Else
                        Me.RefControl.Text = ""
                    End If
                End If
            ElseIf tbValue.TextLength >= MAX_PASSWORD Then
                MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + _
                                (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + _
                                " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
            End If
        End If
    End Sub
    ''' <summary>
    ''' Handle the click on ? button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbQMark_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbQMark.Click
        If EnumValue.ToString = "Barcode" Then
            MessageBox.Show("Use the calc pad to type in a Barcode. " & _
            "If you make a mistake press 'Del' to delete last key. To return without entering a barcode press 'Quit', or press 'Ok' to confirm.", "Help")
        ElseIf EnumValue.ToString = "Quantity" Then
            MessageBox.Show("Use the calc pad to type in a value. " & _
            "If you make a mistake press 'Del' to delete last key. To return without entering a value press 'Quit', or press 'Ok' to confirm.", "Help")
        ElseIf EnumValue.ToString = "UOD" Then
            MessageBox.Show("Use the calc pad to type in a UOD. " & _
            "If you make a mistake press 'Del' to delete last key. " & _
            "To return without entering a barcode press 'Quit', or press 'Ok' to confirm.", "Help")
        ElseIf EnumValue.ToString = "PrintQuantity" Then
            MessageBox.Show("Use the calc pad to type in a Print Quantity. " & _
            "If you make a mistake press 'Del' to delete last key. " & _
            "To return without entering a barcode press 'Quit', or " & _
            "press 'Ok' to confirm. The print quantity should be from 1-9 ", "Help")
            '*******Govindh Nov 09 ******changed for including clearance label functionality
        ElseIf EnumValue.ToString = "ClearancePrice" Then
            MessageBox.Show("Use the calc pad to type in the Clearance Price in Pence/Cents. " & _
            "E.G. €12.50 enter as 1250 or £12.50 enter as 1250. " & _
            "To return without entering a barcode press 'Quit', or " & _
            "press 'Ok' to confirm.", "Help")
            '********end change
        End If
    End Sub
    ''' <summary>
    ''' To handle the click event on OK button. 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Btn_Ok1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        If CalcPadSessionMgr.GetInstance().ProcessOK(tbValue) Then
            If tbValue.Text.Length > 0 Then
                'If EnumValue = CalcPadSessionMgr.EntryTypeEnum.ClearancePrice Then
                '    CLRSessionMgr.GetInstance().UpdateClearancePrice(Convert.ToInt32(tbValue.Text))
                'Else
                Me.RefControl.Text = tbValue.Text
                'End If
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
    Private Sub UpdatePrice()

    End Sub
End Class

