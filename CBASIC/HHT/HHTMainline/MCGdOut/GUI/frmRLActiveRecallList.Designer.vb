<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmRLActiveRecallList
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
        Me.lvRecallList = New System.Windows.Forms.ListView
        Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader2 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader3 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader4 = New System.Windows.Forms.ColumnHeader
        Me.lblTitle = New System.Windows.Forms.Label
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.btnHelp = New CustomButtons.help
        Me.Label1 = New System.Windows.Forms.Label
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.SuspendLayout()
        '
        'lvRecallList
        '
        Me.lvRecallList.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.lvRecallList.Columns.Add(Me.ColumnHeader1)
        Me.lvRecallList.Columns.Add(Me.ColumnHeader2)
        Me.lvRecallList.Columns.Add(Me.ColumnHeader3)
        Me.lvRecallList.Columns.Add(Me.ColumnHeader4)
        Me.lvRecallList.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lvRecallList.FullRowSelect = True
        Me.lvRecallList.Location = New System.Drawing.Point(3, 57)
        Me.lvRecallList.Name = "lvRecallList"
        Me.lvRecallList.Size = New System.Drawing.Size(234, 176)
        Me.lvRecallList.TabIndex = 0
        Me.lvRecallList.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Ref"
        Me.ColumnHeader1.Width = 60
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Description"
        Me.ColumnHeader2.Width = 150
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Items"
        Me.ColumnHeader3.Width = 50
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Progress"
        Me.ColumnHeader4.Width = 80
        '
        'lblTitle
        '
        Me.lblTitle.ForeColor = System.Drawing.Color.Black
        Me.lblTitle.Location = New System.Drawing.Point(3, 9)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(200, 44)
        Me.lblTitle.Text = "The following Planner Leaver Recalls are ready for completion"
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(180, 243)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 1
        '
        'btnHelp
        '
        Me.btnHelp.BackColor = System.Drawing.Color.Transparent
        Me.btnHelp.Location = New System.Drawing.Point(205, 3)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(32, 32)
        Me.btnHelp.TabIndex = 3
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(1, 246)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(174, 24)
        Me.Label1.Text = "Action: Select a Recall"
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 6
        '
        'frmRLActiveRecallList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnHelp)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.lvRecallList)
        Me.KeyPreview = True
        Me.Name = "frmRLActiveRecallList"
        Me.Text = "Recalls"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lvRecallList As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents btnHelp As CustomButtons.help
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
    Friend WithEvents Label1 As System.Windows.Forms.Label
End Class
