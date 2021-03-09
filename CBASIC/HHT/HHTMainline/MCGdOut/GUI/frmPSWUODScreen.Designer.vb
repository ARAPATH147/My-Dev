<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmPSWUODScreen
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnCalcpad = New CustomButtons.btn_CalcPad_small
        Me.lblLabel = New System.Windows.Forms.Label
        Me.txtBarcode = New System.Windows.Forms.TextBox
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.lblTitle = New System.Windows.Forms.Label
        Me.lblScanColour = New System.Windows.Forms.Label
        Me.pnScanLabelColourIndicator = New System.Windows.Forms.Panel
        Me.lblInstruction = New System.Windows.Forms.Label
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.SuspendLayout()
        '
        'btnCalcpad
        '
        Me.btnCalcpad.BackColor = System.Drawing.Color.Transparent
        Me.btnCalcpad.Location = New System.Drawing.Point(205, 145)
        Me.btnCalcpad.Name = "btnCalcpad"
        Me.btnCalcpad.Size = New System.Drawing.Size(24, 28)
        Me.btnCalcpad.TabIndex = 105
        '
        'lblLabel
        '
        Me.lblLabel.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblLabel.Location = New System.Drawing.Point(9, 148)
        Me.lblLabel.Name = "lblLabel"
        Me.lblLabel.Size = New System.Drawing.Size(42, 24)
        Me.lblLabel.Text = "Label"
        '
        'txtBarcode
        '
        Me.txtBarcode.Location = New System.Drawing.Point(51, 148)
        Me.txtBarcode.MaxLength = 14
        Me.txtBarcode.Name = "txtBarcode"
        Me.txtBarcode.ReadOnly = True
        Me.txtBarcode.Size = New System.Drawing.Size(151, 21)
        Me.txtBarcode.TabIndex = 104
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(181, 235)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 102
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.ForeColor = System.Drawing.Color.Black
        Me.lblTitle.Location = New System.Drawing.Point(6, 9)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(231, 25)
        Me.lblTitle.Text = "Pharmacy Special Waste"
        '
        'lblScanColour
        '
        Me.lblScanColour.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblScanColour.Location = New System.Drawing.Point(62, 184)
        Me.lblScanColour.Name = "lblScanColour"
        Me.lblScanColour.Size = New System.Drawing.Size(168, 20)
        Me.lblScanColour.Text = "Scan Red Label"
        '
        'pnScanLabelColourIndicator
        '
        Me.pnScanLabelColourIndicator.BackColor = System.Drawing.Color.Red
        Me.pnScanLabelColourIndicator.Location = New System.Drawing.Point(9, 181)
        Me.pnScanLabelColourIndicator.Name = "pnScanLabelColourIndicator"
        Me.pnScanLabelColourIndicator.Size = New System.Drawing.Size(48, 24)
        '
        'lblInstruction
        '
        Me.lblInstruction.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblInstruction.Location = New System.Drawing.Point(9, 67)
        Me.lblInstruction.Name = "lblInstruction"
        Me.lblInstruction.Size = New System.Drawing.Size(220, 31)
        Me.lblInstruction.Text = "Place all items in UOD" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Do not scan items"
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 110
        '
        'frmPSWUODScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblInstruction)
        Me.Controls.Add(Me.btnCalcpad)
        Me.Controls.Add(Me.lblLabel)
        Me.Controls.Add(Me.txtBarcode)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.lblScanColour)
        Me.Controls.Add(Me.pnScanLabelColourIndicator)
        Me.Name = "frmPSWUODScreen"
        Me.Text = "Goods Out"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnCalcpad As CustomButtons.btn_CalcPad_small
    Friend WithEvents lblLabel As System.Windows.Forms.Label
    Friend WithEvents txtBarcode As System.Windows.Forms.TextBox
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblScanColour As System.Windows.Forms.Label
    Friend WithEvents pnScanLabelColourIndicator As System.Windows.Forms.Panel
    Friend WithEvents lblInstruction As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
End Class
