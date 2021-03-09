<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmView
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
        Me.lblHeading = New System.Windows.Forms.Label
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.Help1 = New CustomButtons.help
        Me.lblTot = New System.Windows.Forms.Label
        Me.lblTotal = New System.Windows.Forms.Label
        Me.lblBottomText = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.lstView = New MCShMon.AdvancedListView
        Me.SuspendLayout()
        '
        'lblHeading
        '
        Me.lblHeading.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblHeading.Location = New System.Drawing.Point(3, 1)
        Me.lblHeading.Name = "lblHeading"
        Me.lblHeading.Size = New System.Drawing.Size(191, 19)
        Me.lblHeading.Text = "View <Module> List"
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(168, 240)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 4
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(14, 233)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 8
        '
        'Help1
        '
        Me.Help1.BackColor = System.Drawing.Color.Transparent
        Me.Help1.Location = New System.Drawing.Point(63, 233)
        Me.Help1.Name = "Help1"
        Me.Help1.Size = New System.Drawing.Size(32, 32)
        Me.Help1.TabIndex = 12
        '
        'lblTot
        '
        Me.lblTot.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTot.Location = New System.Drawing.Point(110, 20)
        Me.lblTot.Name = "lblTot"
        Me.lblTot.Size = New System.Drawing.Size(84, 20)
        Me.lblTot.Text = "Total in list"
        Me.lblTot.Visible = False
        '
        'lblTotal
        '
        Me.lblTotal.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTotal.Location = New System.Drawing.Point(189, 20)
        Me.lblTotal.Name = "lblTotal"
        Me.lblTotal.Size = New System.Drawing.Size(48, 20)
        Me.lblTotal.Text = "Label1"
        Me.lblTotal.Visible = False
        '
        'lblBottomText
        '
        Me.lblBottomText.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblBottomText.Location = New System.Drawing.Point(3, 195)
        Me.lblBottomText.Name = "lblBottomText"
        Me.lblBottomText.Size = New System.Drawing.Size(205, 35)
        Me.lblBottomText.Text = "Select an item or Quit to return"
        Me.lblBottomText.Visible = False
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 10
        '
        'lstView
        '
        Me.lstView.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.lstView.FullRowSelect = True
        Me.lstView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lstView.Location = New System.Drawing.Point(3, 41)
        Me.lstView.Name = "lstView"
        Me.lstView.Size = New System.Drawing.Size(235, 151)
        Me.lstView.TabIndex = 3
        Me.lstView.View = System.Windows.Forms.View.Details
        '
        'frmView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblBottomText)
        Me.Controls.Add(Me.lblTotal)
        Me.Controls.Add(Me.lblTot)
        Me.Controls.Add(Me.Help1)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.lstView)
        Me.Controls.Add(Me.lblHeading)
        Me.Name = "frmView"
        Me.Text = "Item View"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblHeading As System.Windows.Forms.Label
    Friend WithEvents lstView As MCShMon.AdvancedListView
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents Help1 As CustomButtons.help
    Friend WithEvents lblTot As System.Windows.Forms.Label
    Friend WithEvents lblTotal As System.Windows.Forms.Label
    Friend WithEvents lblBottomText As System.Windows.Forms.Label
End Class
