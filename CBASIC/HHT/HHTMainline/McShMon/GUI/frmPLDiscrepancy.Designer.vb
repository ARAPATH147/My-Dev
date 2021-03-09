<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmPLDiscrepancy
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
        Me.Help1 = New CustomButtons.help
        Me.lblHeading = New System.Windows.Forms.Label
        Me.lblTask = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.lstView = New MCShMon.AdvancedListView
        Me.SuspendLayout()
        '
        'Help1
        '
        Me.Help1.BackColor = System.Drawing.Color.Transparent
        Me.Help1.Location = New System.Drawing.Point(204, 5)
        Me.Help1.Name = "Help1"
        Me.Help1.Size = New System.Drawing.Size(32, 32)
        Me.Help1.TabIndex = 20
        '
        'lblHeading
        '
        Me.lblHeading.Location = New System.Drawing.Point(3, 4)
        Me.lblHeading.Name = "lblHeading"
        Me.lblHeading.Size = New System.Drawing.Size(194, 48)
        Me.lblHeading.Text = "<Heading>"
        '
        'lblTask
        '
        Me.lblTask.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTask.Location = New System.Drawing.Point(3, 221)
        Me.lblTask.Name = "lblTask"
        Me.lblTask.Size = New System.Drawing.Size(233, 20)
        Me.lblTask.Text = "Select the item you want to count"
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 19
        '
        'lstView
        '
        Me.lstView.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.lstView.FullRowSelect = True
        Me.lstView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lstView.Location = New System.Drawing.Point(2, 55)
        Me.lstView.Name = "lstView"
        Me.lstView.Size = New System.Drawing.Size(235, 147)
        Me.lstView.TabIndex = 4
        Me.lstView.View = System.Windows.Forms.View.Details
        '
        'frmPLDiscrepancy
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblTask)
        Me.Controls.Add(Me.Help1)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lstView)
        Me.Controls.Add(Me.lblHeading)
        Me.Name = "frmPLDiscrepancy"
        Me.Text = "Picking List"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Help1 As CustomButtons.help
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lstView As MCShMon.AdvancedListView
    Friend WithEvents lblHeading As System.Windows.Forms.Label
    Friend WithEvents lblTask As System.Windows.Forms.Label
End Class
