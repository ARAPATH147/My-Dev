<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmCLSiteInformation
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
        Me.custCtrlBtnQuit = New CustomButtons.btn_Quit_small
        Me.Btn_Help = New CustomButtons.btn_Info
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.lstvwSiteInfo = New MCShMon.AdvancedListView
        Me.lblSiteSelect = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'custCtrlBtnQuit
        '
        Me.custCtrlBtnQuit.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnQuit.Location = New System.Drawing.Point(187, 242)
        Me.custCtrlBtnQuit.Name = "custCtrlBtnQuit"
        Me.custCtrlBtnQuit.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnQuit.TabIndex = 5
        '
        'Btn_Help
        '
        Me.Btn_Help.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Help.Location = New System.Drawing.Point(194, 10)
        Me.Btn_Help.Name = "Btn_Help"
        Me.Btn_Help.Size = New System.Drawing.Size(32, 32)
        Me.Btn_Help.TabIndex = 4
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(133, 10)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 3
        '
        'lstvwSiteInfo
        '
        Me.lstvwSiteInfo.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.lstvwSiteInfo.FullRowSelect = True
        Me.lstvwSiteInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lstvwSiteInfo.Location = New System.Drawing.Point(3, 51)
        Me.lstvwSiteInfo.Name = "lstvwSiteInfo"
        Me.lstvwSiteInfo.Size = New System.Drawing.Size(234, 170)
        Me.lstvwSiteInfo.TabIndex = 6
        Me.lstvwSiteInfo.View = System.Windows.Forms.View.Details
        '
        'lblSiteSelect
        '
        Me.lblSiteSelect.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSiteSelect.Location = New System.Drawing.Point(3, 234)
        Me.lblSiteSelect.Name = "lblSiteSelect"
        Me.lblSiteSelect.Size = New System.Drawing.Size(100, 20)
        Me.lblSiteSelect.Text = "Select a Site"
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 272)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 22)
        Me.objStatusBar.TabIndex = 7
        '
        'frmCLSiteInformation
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblSiteSelect)
        Me.Controls.Add(Me.lstvwSiteInfo)
        Me.Controls.Add(Me.custCtrlBtnQuit)
        Me.Controls.Add(Me.Btn_Help)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Name = "frmCLSiteInformation"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents custCtrlBtnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents Btn_Help As CustomButtons.btn_Info
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents lstvwSiteInfo As MCShMon.AdvancedListView
    Friend WithEvents lblSiteSelect As System.Windows.Forms.Label
    Friend WithEvents objStatusBar As MCShMon.CustomStatusBar
End Class
