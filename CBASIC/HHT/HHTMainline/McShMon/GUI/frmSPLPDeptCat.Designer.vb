<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSPLPDeptCat
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
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.custCtrlBtnBack = New CustomButtons.btn_Back_sm
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.lstView = New MCShMon.AdvancedListView
        Me.lblDeptCat = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(13, 11)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(141, 20)
        Me.Label1.Text = "Live Planners"
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.Location = New System.Drawing.Point(3, 226)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(223, 20)
        Me.Label2.Text = "Select Department/Category"
        '
        'custCtrlBtnBack
        '
        Me.custCtrlBtnBack.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnBack.Location = New System.Drawing.Point(176, 244)
        Me.custCtrlBtnBack.Name = "custCtrlBtnBack"
        Me.custCtrlBtnBack.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnBack.TabIndex = 30
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(194, 8)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 1
        '
        'lstView
        '
        Me.lstView.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.lstView.FullRowSelect = True
        Me.lstView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.lstView.Location = New System.Drawing.Point(3, 56)
        Me.lstView.Name = "lstView"
        Me.lstView.Size = New System.Drawing.Size(223, 158)
        Me.lstView.TabIndex = 5
        Me.lstView.View = System.Windows.Forms.View.Details
        '
        'lblDeptCat
        '
        Me.lblDeptCat.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblDeptCat.Location = New System.Drawing.Point(12, 35)
        Me.lblDeptCat.Name = "lblDeptCat"
        Me.lblDeptCat.Size = New System.Drawing.Size(158, 18)
        Me.lblDeptCat.Text = "Department/Category"
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 33
        '
        'frmSPLPDeptCat
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblDeptCat)
        Me.Controls.Add(Me.custCtrlBtnBack)
        Me.Controls.Add(Me.lstView)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.Label1)
        Me.Name = "frmSPLPDeptCat"
        Me.Text = "Live Planner"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lstView As MCShMon.AdvancedListView
    Friend WithEvents custCtrlBtnBack As CustomButtons.btn_Back_sm
    Friend WithEvents lblDeptCat As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
End Class
