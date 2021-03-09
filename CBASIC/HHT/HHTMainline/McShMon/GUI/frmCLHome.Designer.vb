<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmCLHome
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
        Me.Btn_Help = New CustomButtons.btn_Info
        Me.custCtrlBtnQuit = New CustomButtons.btn_Quit_small
        Me.btn_CreateCountList = New CustomButtons.btn_CreateCountList
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.lstvwProductGroup = New MCShMon.AdvancedListView
        Me.SuspendLayout()
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(12, 223)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 0
        '
        'Btn_Help
        '
        Me.Btn_Help.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Help.Location = New System.Drawing.Point(53, 223)
        Me.Btn_Help.Name = "Btn_Help"
        Me.Btn_Help.Size = New System.Drawing.Size(32, 32)
        Me.Btn_Help.TabIndex = 1
        '
        'custCtrlBtnQuit
        '
        Me.custCtrlBtnQuit.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnQuit.Location = New System.Drawing.Point(176, 228)
        Me.custCtrlBtnQuit.Name = "custCtrlBtnQuit"
        Me.custCtrlBtnQuit.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnQuit.TabIndex = 2
        '
        'btn_CreateCountList
        '
        Me.btn_CreateCountList.BackColor = System.Drawing.Color.Transparent
        Me.btn_CreateCountList.Location = New System.Drawing.Point(94, 228)
        Me.btn_CreateCountList.Name = "btn_CreateCountList"
        Me.btn_CreateCountList.Size = New System.Drawing.Size(73, 24)
        Me.btn_CreateCountList.TabIndex = 14
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 6
        '
        'lstvwProductGroup
        '
        Me.lstvwProductGroup.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.lstvwProductGroup.FullRowSelect = True
        Me.lstvwProductGroup.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lstvwProductGroup.Location = New System.Drawing.Point(12, 15)
        Me.lstvwProductGroup.Name = "lstvwProductGroup"
        Me.lstvwProductGroup.Size = New System.Drawing.Size(214, 200)
        Me.lstvwProductGroup.TabIndex = 3
        Me.lstvwProductGroup.View = System.Windows.Forms.View.Details
        '
        'frmCLHome
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.btn_CreateCountList)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lstvwProductGroup)
        Me.Controls.Add(Me.custCtrlBtnQuit)
        Me.Controls.Add(Me.Btn_Help)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Name = "frmCLHome"
        Me.Text = "Count List"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents Btn_Help As CustomButtons.btn_Info
    Friend WithEvents custCtrlBtnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents lstvwProductGroup As MCShMon.AdvancedListView
    Friend WithEvents btn_CreateCountList As CustomButtons.btn_CreateCountList
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
End Class
