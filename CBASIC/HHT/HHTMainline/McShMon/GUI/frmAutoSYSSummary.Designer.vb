<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmAutoSYSSummary
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
        Me.Btn_Ok1 = New CustomButtons.btn_Ok
        Me.lblHeading = New System.Windows.Forms.Label
        Me.lblItems = New System.Windows.Forms.Label
        Me.lblPicked = New System.Windows.Forms.Label
        Me.lblScannedItems = New System.Windows.Forms.Label
        Me.lblItemsPicked = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'Btn_Ok1
        '
        Me.Btn_Ok1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Ok1.Location = New System.Drawing.Point(97, 212)
        Me.Btn_Ok1.Name = "Btn_Ok1"
        Me.Btn_Ok1.Size = New System.Drawing.Size(40, 40)
        Me.Btn_Ok1.TabIndex = 0
        '
        'lblHeading
        '
        Me.lblHeading.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblHeading.Location = New System.Drawing.Point(15, 20)
        Me.lblHeading.Name = "lblHeading"
        Me.lblHeading.Size = New System.Drawing.Size(194, 20)
        Me.lblHeading.Text = "Stuff Your Shelves Summary   " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'lblItems
        '
        Me.lblItems.Location = New System.Drawing.Point(15, 58)
        Me.lblItems.Name = "lblItems"
        Me.lblItems.Size = New System.Drawing.Size(100, 20)
        Me.lblItems.Text = "Scanned Items:"
        '
        'lblPicked
        '
        Me.lblPicked.Location = New System.Drawing.Point(15, 94)
        Me.lblPicked.Name = "lblPicked"
        Me.lblPicked.Size = New System.Drawing.Size(100, 20)
        Me.lblPicked.Text = "Items Picked:"
        '
        'lblScannedItems
        '
        Me.lblScannedItems.Location = New System.Drawing.Point(153, 58)
        Me.lblScannedItems.Name = "lblScannedItems"
        Me.lblScannedItems.Size = New System.Drawing.Size(59, 20)
        Me.lblScannedItems.Text = "0"
        '
        'lblItemsPicked
        '
        Me.lblItemsPicked.Location = New System.Drawing.Point(153, 94)
        Me.lblItemsPicked.Name = "lblItemsPicked"
        Me.lblItemsPicked.Size = New System.Drawing.Size(46, 20)
        Me.lblItemsPicked.Text = "0"
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 23
        '
        'frmAutoSYSSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblItemsPicked)
        Me.Controls.Add(Me.lblScannedItems)
        Me.Controls.Add(Me.lblPicked)
        Me.Controls.Add(Me.lblItems)
        Me.Controls.Add(Me.lblHeading)
        Me.Controls.Add(Me.Btn_Ok1)
        Me.Name = "frmAutoSYSSummary"
        Me.Text = "Stuff Your Shelves"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Btn_Ok1 As CustomButtons.btn_Ok
    Friend WithEvents lblHeading As System.Windows.Forms.Label
    Friend WithEvents lblItems As System.Windows.Forms.Label
    Friend WithEvents lblPicked As System.Windows.Forms.Label
    Friend WithEvents lblScannedItems As System.Windows.Forms.Label
    Friend WithEvents lblItemsPicked As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
End Class
