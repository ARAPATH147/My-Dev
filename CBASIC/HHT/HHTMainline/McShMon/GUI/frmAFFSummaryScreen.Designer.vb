<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmAFFSummaryScreen
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
        Me.lblsummary = New System.Windows.Forms.Label
        Me.lblScanned = New System.Windows.Forms.Label
        Me.lblScannedCount = New System.Windows.Forms.Label
        Me.lblPicked = New System.Windows.Forms.Label
        Me.lblPickedCount = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.Btn_Ok1 = New CustomButtons.btn_Ok
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblsummary
        '
        Me.lblsummary.Location = New System.Drawing.Point(15, 28)
        Me.lblsummary.Name = "lblsummary"
        Me.lblsummary.Size = New System.Drawing.Size(206, 20)
        Me.lblsummary.Text = "Auto Fast Fill Picking List Summary"
        '
        'lblScanned
        '
        Me.lblScanned.Location = New System.Drawing.Point(15, 68)
        Me.lblScanned.Name = "lblScanned"
        Me.lblScanned.Size = New System.Drawing.Size(100, 20)
        Me.lblScanned.Text = "Scanned Items"
        '
        'lblScannedCount
        '
        Me.lblScannedCount.Location = New System.Drawing.Point(112, 68)
        Me.lblScannedCount.Name = "lblScannedCount"
        Me.lblScannedCount.Size = New System.Drawing.Size(48, 20)
        Me.lblScannedCount.Text = "00"
        Me.lblScannedCount.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblPicked
        '
        Me.lblPicked.Location = New System.Drawing.Point(15, 102)
        Me.lblPicked.Name = "lblPicked"
        Me.lblPicked.Size = New System.Drawing.Size(100, 20)
        Me.lblPicked.Text = "Items Picked"
        '
        'lblPickedCount
        '
        Me.lblPickedCount.Location = New System.Drawing.Point(112, 102)
        Me.lblPickedCount.Name = "lblPickedCount"
        Me.lblPickedCount.Size = New System.Drawing.Size(48, 20)
        Me.lblPickedCount.Text = "00"
        Me.lblPickedCount.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(15, 195)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(199, 20)
        Me.Label1.Text = "Action: Dock and Transmit"
        '
        'Btn_Ok1
        '
        Me.Btn_Ok1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Ok1.Location = New System.Drawing.Point(102, 218)
        Me.Btn_Ok1.Name = "Btn_Ok1"
        Me.Btn_Ok1.Size = New System.Drawing.Size(38, 40)
        Me.Btn_Ok1.TabIndex = 35
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 36
        '
        'frmAFFSummaryScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.Btn_Ok1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblPickedCount)
        Me.Controls.Add(Me.lblPicked)
        Me.Controls.Add(Me.lblScannedCount)
        Me.Controls.Add(Me.lblScanned)
        Me.Controls.Add(Me.lblsummary)
        Me.Name = "frmAFFSummaryScreen"
        Me.Text = "Picking List"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblsummary As System.Windows.Forms.Label
    Friend WithEvents lblScanned As System.Windows.Forms.Label
    Friend WithEvents lblScannedCount As System.Windows.Forms.Label
    Friend WithEvents lblPicked As System.Windows.Forms.Label
    Friend WithEvents lblPickedCount As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Btn_Ok1 As CustomButtons.btn_Ok
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
End Class
