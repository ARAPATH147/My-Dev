<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSPModuleListSP
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
        Me.lblSelectModule = New System.Windows.Forms.Label
        Me.Btn_Print1 = New CustomButtons.btn_Print
        Me.lblDate = New System.Windows.Forms.Label
        Me.lblRebuildDate = New System.Windows.Forms.Label
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.lblPlanner = New System.Windows.Forms.Label
        Me.custCtrlBtnBack = New CustomButtons.btn_Back_sm
        Me.lstView = New MCShMon.AdvancedListView
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblSelectModule
        '
        Me.lblSelectModule.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelectModule.Location = New System.Drawing.Point(9, 222)
        Me.lblSelectModule.Name = "lblSelectModule"
        Me.lblSelectModule.Size = New System.Drawing.Size(128, 17)
        Me.lblSelectModule.Text = "Select Module"
        '
        'Btn_Print1
        '
        Me.Btn_Print1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Print1.Location = New System.Drawing.Point(99, 243)
        Me.Btn_Print1.Name = "Btn_Print1"
        Me.Btn_Print1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Print1.TabIndex = 19
        '
        'lblDate
        '
        Me.lblDate.Location = New System.Drawing.Point(182, 39)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(56, 20)
        Me.lblDate.Text = "29/12/08"
        '
        'lblRebuildDate
        '
        Me.lblRebuildDate.Location = New System.Drawing.Point(129, 39)
        Me.lblRebuildDate.Name = "lblRebuildDate"
        Me.lblRebuildDate.Size = New System.Drawing.Size(50, 20)
        Me.lblRebuildDate.Text = "Rebuild"
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(199, 3)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 16
        '
        'lblPlanner
        '
        Me.lblPlanner.Font = New System.Drawing.Font("Tahoma", 11.0!, System.Drawing.FontStyle.Regular)
        Me.lblPlanner.Location = New System.Drawing.Point(3, 19)
        Me.lblPlanner.Name = "lblPlanner"
        Me.lblPlanner.Size = New System.Drawing.Size(174, 20)
        Me.lblPlanner.Text = "Planner"
        '
        'custCtrlBtnBack
        '
        Me.custCtrlBtnBack.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnBack.Location = New System.Drawing.Point(181, 243)
        Me.custCtrlBtnBack.Name = "custCtrlBtnBack"
        Me.custCtrlBtnBack.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnBack.TabIndex = 30
        '
        'lstView
        '
        Me.lstView.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.lstView.FullRowSelect = True
        Me.lstView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.lstView.Location = New System.Drawing.Point(9, 57)
        Me.lstView.Name = "lstView"
        Me.lstView.Size = New System.Drawing.Size(223, 158)
        Me.lstView.TabIndex = 17
        Me.lstView.View = System.Windows.Forms.View.Details
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 35
        '
        'frmSPModuleListSP
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.custCtrlBtnBack)
        Me.Controls.Add(Me.lblSelectModule)
        Me.Controls.Add(Me.Btn_Print1)
        Me.Controls.Add(Me.lstView)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.lblRebuildDate)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.lblPlanner)
        Me.Name = "frmSPModuleListSP"
        Me.Text = "Search Planner"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblSelectModule As System.Windows.Forms.Label
    Friend WithEvents Btn_Print1 As CustomButtons.btn_Print
    Friend WithEvents lstView As MCShMon.AdvancedListView
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblRebuildDate As System.Windows.Forms.Label
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents lblPlanner As System.Windows.Forms.Label
    Friend WithEvents custCtrlBtnBack As CustomButtons.btn_Back_sm
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
End Class
