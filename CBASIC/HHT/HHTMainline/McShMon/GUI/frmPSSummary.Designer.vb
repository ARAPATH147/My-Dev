<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmPSSummary
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
        Me.lblPrintSummary = New System.Windows.Forms.Label
        Me.lblScannedItems = New System.Windows.Forms.Label
        Me.lblSELQueued = New System.Windows.Forms.Label
        Me.lblScanNum = New System.Windows.Forms.Label
        Me.lblSELNum = New System.Windows.Forms.Label
        Me.lblAction = New System.Windows.Forms.Label
        Me.Btn_Ok1 = New CustomButtons.btn_Ok
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.lblPrintSEL = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblPrintSummary
        '
        Me.lblPrintSummary.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblPrintSummary.Location = New System.Drawing.Point(16, 28)
        Me.lblPrintSummary.Name = "lblPrintSummary"
        Me.lblPrintSummary.Size = New System.Drawing.Size(140, 20)
        Me.lblPrintSummary.Text = "Print SEL Summary"
        '
        'lblScannedItems
        '
        Me.lblScannedItems.Location = New System.Drawing.Point(16, 77)
        Me.lblScannedItems.Name = "lblScannedItems"
        Me.lblScannedItems.Size = New System.Drawing.Size(100, 20)
        Me.lblScannedItems.Text = "Scanned Items:"
        '
        'lblSELQueued
        '
        Me.lblSELQueued.Location = New System.Drawing.Point(16, 109)
        Me.lblSELQueued.Name = "lblSELQueued"
        Me.lblSELQueued.Size = New System.Drawing.Size(105, 20)
        Me.lblSELQueued.Text = "SELs Generated:"
        '
        'lblScanNum
        '
        Me.lblScanNum.Location = New System.Drawing.Point(122, 77)
        Me.lblScanNum.Name = "lblScanNum"
        Me.lblScanNum.Size = New System.Drawing.Size(57, 20)
        Me.lblScanNum.Text = "ScanNum"
        '
        'lblSELNum
        '
        Me.lblSELNum.Location = New System.Drawing.Point(122, 109)
        Me.lblSELNum.Name = "lblSELNum"
        Me.lblSELNum.Size = New System.Drawing.Size(57, 20)
        Me.lblSELNum.Text = "SELNum"
        '
        'lblAction
        '
        Me.lblAction.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblAction.Location = New System.Drawing.Point(16, 170)
        Me.lblAction.Name = "lblAction"
        Me.lblAction.Size = New System.Drawing.Size(206, 37)
        Me.lblAction.Text = "Action: Dock && Transmit.      Collect and display all new SELs"
        '
        'Btn_Ok1
        '
        Me.Btn_Ok1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Ok1.Location = New System.Drawing.Point(89, 221)
        Me.Btn_Ok1.Name = "Btn_Ok1"
        Me.Btn_Ok1.Size = New System.Drawing.Size(40, 40)
        Me.Btn_Ok1.TabIndex = 7
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 14
        '
        'lblPrintSEL
        '
        Me.lblPrintSEL.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblPrintSEL.Location = New System.Drawing.Point(16, 170)
        Me.lblPrintSEL.Name = "lblPrintSEL"
        Me.lblPrintSEL.Size = New System.Drawing.Size(206, 37)
        Me.lblPrintSEL.Text = "Collect and display all new SELs"
        'frmPSSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblPrintSEL)
        Me.Controls.Add(Me.lblSELNum)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.Btn_Ok1)
        Me.Controls.Add(Me.lblAction)
        Me.Controls.Add(Me.lblScanNum)
        Me.Controls.Add(Me.lblScannedItems)
        Me.Controls.Add(Me.lblPrintSummary)
        Me.Controls.Add(Me.lblSELQueued)
        Me.Name = "frmPSSummary"
        Me.Text = "Print SELs"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblPrintSummary As System.Windows.Forms.Label
    Friend WithEvents lblScannedItems As System.Windows.Forms.Label
    Friend WithEvents lblSELQueued As System.Windows.Forms.Label
    Friend WithEvents lblScanNum As System.Windows.Forms.Label
    Friend WithEvents lblSELNum As System.Windows.Forms.Label
    Friend WithEvents lblAction As System.Windows.Forms.Label
    Friend WithEvents Btn_Ok1 As CustomButtons.btn_Ok
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblPrintSEL As System.Windows.Forms.Label
End Class
