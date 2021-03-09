<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSPPrintSEL
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
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.lblItem = New System.Windows.Forms.Label
        Me.Btn_Quit_small1 = New CustomButtons.btn_Quit_small
        Me.lblSelectModule = New System.Windows.Forms.Label
        Me.lstView = New MCShMon.AdvancedListView
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(199, 9)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 4
        '
        'lblItem
        '
        Me.lblItem.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblItem.Location = New System.Drawing.Point(3, 9)
        Me.lblItem.Name = "lblItem"
        Me.lblItem.Size = New System.Drawing.Size(190, 20)
        Me.lblItem.Text = "Item"
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(181, 219)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.TabIndex = 11
        '
        'lblSelectModule
        '
        Me.lblSelectModule.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelectModule.Location = New System.Drawing.Point(9, 247)
        Me.lblSelectModule.Name = "lblSelectModule"
        Me.lblSelectModule.Size = New System.Drawing.Size(201, 17)
        Me.lblSelectModule.Text = "Select Print Option"
        '
        'lstView
        '
        Me.lstView.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.lstView.FullRowSelect = True
        Me.lstView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.lstView.Location = New System.Drawing.Point(9, 55)
        Me.lstView.Name = "lstView"
        Me.lstView.Size = New System.Drawing.Size(223, 158)
        Me.lstView.TabIndex = 9
        Me.lstView.View = System.Windows.Forms.View.Details
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 13
        '
        'frmSPPrintSEL
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblSelectModule)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.lstView)
        Me.Controls.Add(Me.lblItem)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Name = "frmSPPrintSEL"
        Me.Text = "Planners"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents lblItem As System.Windows.Forms.Label
    Friend WithEvents lstView As MCShMon.AdvancedListView
    Friend WithEvents Btn_Quit_small1 As CustomButtons.btn_Quit_small
    Friend WithEvents lblSelectModule As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
End Class
