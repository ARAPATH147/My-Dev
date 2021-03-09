Public Class frmCalcPad
    Inherits System.Windows.Forms.Form
    'Public calcPad As CalcPadSessionMgr
    Private RefControl As Control
    Public NoofDigit As Integer
    Private EnumValue As CalcPadSessionMgr.EntryTypeEnum
    Public EnumCheckValue As Boolean = False
    Friend WithEvents pbSeven As System.Windows.Forms.Button
    Friend WithEvents pbEight As System.Windows.Forms.Button
    Friend WithEvents pbNine As System.Windows.Forms.Button
    Friend WithEvents pbFour As System.Windows.Forms.Button
    Friend WithEvents pbFive As System.Windows.Forms.Button
    Friend WithEvents pbSix As System.Windows.Forms.Button
    Friend WithEvents pbOne As System.Windows.Forms.Button
    Friend WithEvents pbTwo As System.Windows.Forms.Button
    Friend WithEvents pbThree As System.Windows.Forms.Button
    Friend WithEvents pbZero As System.Windows.Forms.Button
    Friend WithEvents pbOk As System.Windows.Forms.PictureBox
    Friend WithEvents tmrChecker As System.Windows.Forms.Timer
    Public MAX_PASSWORD As Integer  ' Maximum length of EAN



#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents pbHelp As System.Windows.Forms.PictureBox
    Friend WithEvents pbQuit As System.Windows.Forms.PictureBox
    Friend WithEvents pbPlus As System.Windows.Forms.PictureBox
    Friend WithEvents pbMinus As System.Windows.Forms.PictureBox
    Friend WithEvents lblInfo As System.Windows.Forms.Label
    Friend WithEvents tbStNo As System.Windows.Forms.TextBox
    Friend WithEvents status As System.Windows.Forms.TextBox
    Friend WithEvents tbValue As System.Windows.Forms.TextBox
    Friend WithEvents pbDelete As System.Windows.Forms.PictureBox
    Friend WithEvents pbMultiply As System.Windows.Forms.PictureBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCalcPad))
        Me.tbValue = New System.Windows.Forms.TextBox
        Me.pbHelp = New System.Windows.Forms.PictureBox
        Me.pbDelete = New System.Windows.Forms.PictureBox
        Me.pbQuit = New System.Windows.Forms.PictureBox
        Me.pbPlus = New System.Windows.Forms.PictureBox
        Me.pbMinus = New System.Windows.Forms.PictureBox
        Me.pbMultiply = New System.Windows.Forms.PictureBox
        Me.lblInfo = New System.Windows.Forms.Label
        Me.tbStNo = New System.Windows.Forms.TextBox
        Me.status = New System.Windows.Forms.TextBox
        Me.pbSeven = New System.Windows.Forms.Button
        Me.pbEight = New System.Windows.Forms.Button
        Me.pbNine = New System.Windows.Forms.Button
        Me.pbFour = New System.Windows.Forms.Button
        Me.pbFive = New System.Windows.Forms.Button
        Me.pbSix = New System.Windows.Forms.Button
        Me.pbOne = New System.Windows.Forms.Button
        Me.pbTwo = New System.Windows.Forms.Button
        Me.pbThree = New System.Windows.Forms.Button
        Me.pbZero = New System.Windows.Forms.Button
        Me.pbOk = New System.Windows.Forms.PictureBox
        Me.tmrChecker = New System.Windows.Forms.Timer
        Me.SuspendLayout()
        '
        'tbValue
        '
        Me.tbValue.Font = New System.Drawing.Font("Arial", 11.0!, System.Drawing.FontStyle.Regular)
        Me.tbValue.Location = New System.Drawing.Point(25, 42)
        Me.tbValue.Name = "tbValue"
        Me.tbValue.Size = New System.Drawing.Size(191, 29)
        Me.tbValue.TabIndex = 20
        '
        'pbHelp
        '
        Me.pbHelp.Image = CType(resources.GetObject("pbHelp.Image"), System.Drawing.Image)
        Me.pbHelp.Location = New System.Drawing.Point(184, 5)
        Me.pbHelp.Name = "pbHelp"
        Me.pbHelp.Size = New System.Drawing.Size(32, 32)
        Me.pbHelp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbDelete
        '
        Me.pbDelete.Image = CType(resources.GetObject("pbDelete.Image"), System.Drawing.Image)
        Me.pbDelete.Location = New System.Drawing.Point(75, 220)
        Me.pbDelete.Name = "pbDelete"
        Me.pbDelete.Size = New System.Drawing.Size(40, 40)
        Me.pbDelete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbQuit
        '
        Me.pbQuit.Image = CType(resources.GetObject("pbQuit.Image"), System.Drawing.Image)
        Me.pbQuit.Location = New System.Drawing.Point(125, 220)
        Me.pbQuit.Name = "pbQuit"
        Me.pbQuit.Size = New System.Drawing.Size(40, 40)
        Me.pbQuit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbPlus
        '
        Me.pbPlus.Image = CType(resources.GetObject("pbPlus.Image"), System.Drawing.Image)
        Me.pbPlus.Location = New System.Drawing.Point(175, 77)
        Me.pbPlus.Name = "pbPlus"
        Me.pbPlus.Size = New System.Drawing.Size(40, 40)
        Me.pbPlus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbMinus
        '
        Me.pbMinus.Image = CType(resources.GetObject("pbMinus.Image"), System.Drawing.Image)
        Me.pbMinus.Location = New System.Drawing.Point(175, 125)
        Me.pbMinus.Name = "pbMinus"
        Me.pbMinus.Size = New System.Drawing.Size(40, 40)
        Me.pbMinus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbMultiply
        '
        Me.pbMultiply.Image = CType(resources.GetObject("pbMultiply.Image"), System.Drawing.Image)
        Me.pbMultiply.Location = New System.Drawing.Point(175, 172)
        Me.pbMultiply.Name = "pbMultiply"
        Me.pbMultiply.Size = New System.Drawing.Size(40, 40)
        Me.pbMultiply.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblInfo
        '
        Me.lblInfo.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Bold)
        Me.lblInfo.Location = New System.Drawing.Point(25, 19)
        Me.lblInfo.Name = "lblInfo"
        Me.lblInfo.Size = New System.Drawing.Size(176, 20)
        Me.lblInfo.Text = "Please enter a value"
        '
        'tbStNo
        '
        Me.tbStNo.BackColor = System.Drawing.Color.LightGray
        Me.tbStNo.Location = New System.Drawing.Point(200, 288)
        Me.tbStNo.Name = "tbStNo"
        Me.tbStNo.ReadOnly = True
        Me.tbStNo.Size = New System.Drawing.Size(40, 21)
        Me.tbStNo.TabIndex = 0
        '
        'status
        '
        Me.status.BackColor = System.Drawing.Color.LightGray
        Me.status.Location = New System.Drawing.Point(0, 288)
        Me.status.Name = "status"
        Me.status.ReadOnly = True
        Me.status.Size = New System.Drawing.Size(200, 21)
        Me.status.TabIndex = 1
        '
        'pbSeven
        '
        Me.pbSeven.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbSeven.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbSeven.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbSeven.Location = New System.Drawing.Point(25, 77)
        Me.pbSeven.Name = "pbSeven"
        Me.pbSeven.Size = New System.Drawing.Size(40, 40)
        Me.pbSeven.TabIndex = 21
        Me.pbSeven.Text = "7"
        '
        'pbEight
        '
        Me.pbEight.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbEight.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbEight.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbEight.Location = New System.Drawing.Point(75, 77)
        Me.pbEight.Name = "pbEight"
        Me.pbEight.Size = New System.Drawing.Size(40, 40)
        Me.pbEight.TabIndex = 21
        Me.pbEight.Text = "8"
        '
        'pbNine
        '
        Me.pbNine.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNine.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNine.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNine.Location = New System.Drawing.Point(125, 77)
        Me.pbNine.Name = "pbNine"
        Me.pbNine.Size = New System.Drawing.Size(40, 40)
        Me.pbNine.TabIndex = 21
        Me.pbNine.Text = "9"
        '
        'pbFour
        '
        Me.pbFour.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbFour.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbFour.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbFour.Location = New System.Drawing.Point(25, 125)
        Me.pbFour.Name = "pbFour"
        Me.pbFour.Size = New System.Drawing.Size(40, 40)
        Me.pbFour.TabIndex = 21
        Me.pbFour.Text = "4"
        '
        'pbFive
        '
        Me.pbFive.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbFive.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbFive.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbFive.Location = New System.Drawing.Point(75, 125)
        Me.pbFive.Name = "pbFive"
        Me.pbFive.Size = New System.Drawing.Size(40, 40)
        Me.pbFive.TabIndex = 21
        Me.pbFive.Text = "5"
        '
        'pbSix
        '
        Me.pbSix.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbSix.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbSix.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbSix.Location = New System.Drawing.Point(125, 125)
        Me.pbSix.Name = "pbSix"
        Me.pbSix.Size = New System.Drawing.Size(40, 40)
        Me.pbSix.TabIndex = 21
        Me.pbSix.Text = "6"
        '
        'pbOne
        '
        Me.pbOne.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbOne.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbOne.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbOne.Location = New System.Drawing.Point(24, 172)
        Me.pbOne.Name = "pbOne"
        Me.pbOne.Size = New System.Drawing.Size(40, 40)
        Me.pbOne.TabIndex = 21
        Me.pbOne.Text = "1"
        '
        'pbTwo
        '
        Me.pbTwo.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbTwo.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbTwo.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbTwo.Location = New System.Drawing.Point(75, 172)
        Me.pbTwo.Name = "pbTwo"
        Me.pbTwo.Size = New System.Drawing.Size(40, 40)
        Me.pbTwo.TabIndex = 21
        Me.pbTwo.Text = "2"
        '
        'pbThree
        '
        Me.pbThree.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbThree.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbThree.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbThree.Location = New System.Drawing.Point(125, 172)
        Me.pbThree.Name = "pbThree"
        Me.pbThree.Size = New System.Drawing.Size(40, 40)
        Me.pbThree.TabIndex = 21
        Me.pbThree.Text = "3"
        '
        'pbZero
        '
        Me.pbZero.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbZero.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbZero.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbZero.Location = New System.Drawing.Point(25, 220)
        Me.pbZero.Name = "pbZero"
        Me.pbZero.Size = New System.Drawing.Size(40, 40)
        Me.pbZero.TabIndex = 21
        Me.pbZero.Text = "0"
        '
        'pbOk
        '
        Me.pbOk.Image = CType(resources.GetObject("pbOk.Image"), System.Drawing.Image)
        Me.pbOk.Location = New System.Drawing.Point(175, 220)
        Me.pbOk.Name = "pbOk"
        Me.pbOk.Size = New System.Drawing.Size(40, 40)
        Me.pbOk.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'tmrChecker
        '
        '
        'frmCalcPad
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.pbHelp)
        Me.Controls.Add(Me.pbThree)
        Me.Controls.Add(Me.pbTwo)
        Me.Controls.Add(Me.pbSix)
        Me.Controls.Add(Me.pbFive)
        Me.Controls.Add(Me.pbZero)
        Me.Controls.Add(Me.pbOne)
        Me.Controls.Add(Me.pbNine)
        Me.Controls.Add(Me.pbFour)
        Me.Controls.Add(Me.pbEight)
        Me.Controls.Add(Me.pbSeven)
        Me.Controls.Add(Me.tbStNo)
        Me.Controls.Add(Me.status)
        Me.Controls.Add(Me.lblInfo)
        Me.Controls.Add(Me.pbMultiply)
        Me.Controls.Add(Me.pbMinus)
        Me.Controls.Add(Me.pbPlus)
        Me.Controls.Add(Me.pbOk)
        Me.Controls.Add(Me.pbQuit)
        Me.Controls.Add(Me.pbDelete)
        Me.Controls.Add(Me.tbValue)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmCalcPad"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region

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
                'Checks if there is value in the control. 
                'If there is value then append "+" sign.
                If ctrlrefValue.Text.Trim().Equals("") Then
                    tbValue.Text = ""
                    CalcPadSessionMgr.GetInstance().strAvailableData = False
                ElseIf ctrlrefValue.Text.Trim().Equals("0") Then
                    tbValue.Text = ""
                    CalcPadSessionMgr.GetInstance().strAvailableData = False
                Else
                    If ctrlrefValue.Text.Trim().EndsWith("+") Then
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
                'The Maximum Value That A UOD Can Have is 14
                MAX_PASSWORD = 14
                tbValue.MaxLength = MAX_PASSWORD
            Case CalcPadSessionMgr.EntryTypeEnum.Barcode
                'for Bar Code  The Text Box Has To BE Empty
                tbValue.Text = ""
                'The MAximum Value That a Barcode can Have is 18
                MAX_PASSWORD = 18
                tbValue.MaxLength = MAX_PASSWORD
            Case CalcPadSessionMgr.EntryTypeEnum.Supplier
                'for Supplier  The Text Box Has To BE Empty
                tbValue.Text = ""
                'The MAximum Value That a Barcode can Have is 18
                MAX_PASSWORD = 6
                tbValue.MaxLength = MAX_PASSWORD
        End Select
        tbValue.SelectionStart = tbValue.TextLength

    End Sub

    Public Sub CalPadInitialize(ByRef ctrlrefValue As Control, ByVal EntryTypeTemp As Integer)
        'calcPad = New CalcPadSessionMgr
        CalcPadSessionMgr.GetInstance().cEntryType = EntryTypeTemp
        RefControl = ctrlrefValue
        Select Case EntryTypeTemp
            Case 0
                'Checks if there is value in the control. 
                'If there is value then append "+" sign.
                If ctrlrefValue.Text.Trim().Equals("") Then
                    tbValue.Text = ""
                    CalcPadSessionMgr.GetInstance().strAvailableData = ""
                ElseIf ctrlrefValue.Text.Trim().Equals("0") Then
                    tbValue.Text = ""
                    CalcPadSessionMgr.GetInstance().strAvailableData = ""
                Else
                    If ctrlrefValue.Text.Trim().EndsWith("+") Then 'ctrlrefValue.Text.Trim().Last.Equals("+") Then
                        tbValue.Text = ctrlrefValue.Text.Trim()
                        CalcPadSessionMgr.GetInstance().strAvailableData = Mid(ctrlrefValue.Text.Trim(), 1, ctrlrefValue.Text.Length)
                    Else
                        tbValue.Text = ctrlrefValue.Text + " + "
                        CalcPadSessionMgr.GetInstance().strAvailableData = ctrlrefValue.Text.Trim()
                    End If


                End If
            Case 1
                tbValue.Text = ctrlrefValue.Text
            Case 2
                'If barcode the the field should be empty
                tbValue.Text = ""
                'tbValue.Text = objAppContainer.objHelper.UnFormatBarcode(ctrlrefValue.Text)

        End Select
    End Sub
    Private Sub pbZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbZero.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "0"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
        tbValue.Focus()
    End Sub

    Private Sub pbOne_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbOne.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "1"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
        tbValue.Focus()
    End Sub

    Private Sub pbTwo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbTwo.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Text += "2"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
        tbValue.Focus()
    End Sub

    Private Sub pbThree_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbThree.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "3"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
        tbValue.Focus()
    End Sub

    Private Sub pbFour_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbFour.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "4"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
        tbValue.Focus()
    End Sub

    Private Sub pbFive_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbFive.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "5"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
        tbValue.Focus()
    End Sub

    Private Sub pbSix_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSix.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "6"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
        tbValue.Focus()
    End Sub

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

    Private Sub pbNine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNine.Click
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < MAX_PASSWORD Then
            tbValue.Focus()
            tbValue.Text += "9"
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            MessageBox.Show("The Maximum length for " + EnumValue.ToString + " Entry is " + (MAX_PASSWORD.ToString) + " Characters", EnumValue.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
        tbValue.Focus()
    End Sub

    Private Sub pbClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        tbValue.Text = ""
        tbValue.Focus()
    End Sub

    Private Sub pbDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbDelete.Click
        tbValue.Focus()
        CalcPadSessionMgr.GetInstance().ProcessDelete(tbValue)
        tbValue.SelectionStart = Len(tbValue.Text)
    End Sub


    Private Sub pbQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbQuit.Click
#If RF Then
        tbValue.Text = ""
        tbValue.Focus()
        '  CalcPadSessionMgr.GetInstance().EndSession()
        Me.Hide()
        'Me.DialogResult = Windows.Forms.DialogResult.Cancel
#End If

#If NRF Then
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        'Me.Close()
        ' Application.DoEvents()
#End If
    End Sub
    Private Sub pbHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbHelp.Click

        Select Case EnumValue
            Case CalcPadSessionMgr.EntryTypeEnum.Barcode
                MessageBox.Show("Use the calc pad to type in a Barcode. " & _
                  "If you make a mistake press 'Del' to delete last key. To return without entering a barcode press 'Quit', or press 'Ok' to confirm.", "Help")
            Case CalcPadSessionMgr.EntryTypeEnum.Quantity
                MessageBox.Show("Use the calc pad to type in a value. " & _
            "If you make a mistake press 'Del' to delete last key. To return without entering a value press 'Quit', or press 'Ok' to confirm.", "Help")
            Case CalcPadSessionMgr.EntryTypeEnum.Supplier
                MessageBox.Show("Use the calc pad to type in a Supplier Number. " & _
          "If you make a mistake press 'Del' to delete last key. To return without entering a barcode press 'Quit', or press 'Ok' to confirm.", "Help")
            Case CalcPadSessionMgr.EntryTypeEnum.UOD
                MessageBox.Show("Use the calc pad to type in a UOD. " & _
            "If you make a mistake press 'Del' to delete last key. To return without entering a barcode press 'Quit', or press 'Ok' to confirm.", "Help")

        End Select
    End Sub

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


    Public Sub pbOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbOk.Click
#If RF Then
        pbOk.Enabled = False
        If CalcPadSessionMgr.GetInstance().ProcessOK(tbValue) Then
            If tbValue.Text.Length > 0 Then
                Me.RefControl.Text = tbValue.Text
            Else
                Me.RefControl.Text = ""
            End If
            If EnumValue = CalcPadSessionMgr.EntryTypeEnum.Barcode Or _
             EnumValue = CalcPadSessionMgr.EntryTypeEnum.Supplier Then
                Me.RefControl.Text = ""
                CalcPadSessionMgr.GetInstance().ProcessEnteredValue(tbValue, True)

            End If
            pbOk.Enabled = True
            Me.Hide()
        Else
            pbOk.Enabled = True
            If objAppContainer.bCalcpad Then
                Me.Hide()
                objAppContainer.bCalcpad = False
            End If
        End If
#ElseIf NRF Then
        If CalcPadSessionMgr.GetInstance().ProcessOK(tbValue) Then
            If tbValue.Text.Length > 0 Then
                Me.RefControl.Text = tbValue.Text
            Else
                Me.RefControl.Text = ""
            End If
            DialogResult = Windows.Forms.DialogResult.OK
        End If
#End If
    End Sub
    ''' <summary>
    ''' To set the timer at load event close calcpad before autologoff.
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
    ''' Handle the tick event to close calcpad form.
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
