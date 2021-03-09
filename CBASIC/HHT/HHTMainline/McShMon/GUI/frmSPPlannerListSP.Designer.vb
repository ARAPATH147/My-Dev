<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSPPlannerListSP
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
        Me.lblSPHeading = New System.Windows.Forms.Label
        Me.lblSelectPlnr = New System.Windows.Forms.Label
        Me.lstView = New MCShMon.AdvancedListView
        Me.lblRebuildDate = New System.Windows.Forms.Label
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.custCtrlBtnBack = New CustomButtons.btn_Back_sm
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblSPHeading
        '
        Me.lblSPHeading.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblSPHeading.Location = New System.Drawing.Point(2, 33)
        Me.lblSPHeading.Name = "lblSPHeading"
        Me.lblSPHeading.Size = New System.Drawing.Size(196, 14)
        Me.lblSPHeading.Text = "Item located in the following Planner(s)"
        '
        'lblSelectPlnr
        '
        Me.lblSelectPlnr.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelectPlnr.Location = New System.Drawing.Point(9, 231)
        Me.lblSelectPlnr.Name = "lblSelectPlnr"
        Me.lblSelectPlnr.Size = New System.Drawing.Size(112, 17)
        Me.lblSelectPlnr.Text = "Select Planner"
        '
        'lstView
        '
        Me.lstView.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.lstView.FullRowSelect = True
        Me.lstView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.lstView.Location = New System.Drawing.Point(9, 66)
        Me.lstView.Name = "lstView"
        Me.lstView.Size = New System.Drawing.Size(223, 158)
        Me.lstView.TabIndex = 13
        Me.lstView.View = System.Windows.Forms.View.Details
        '
        'lblRebuildDate
        '
        Me.lblRebuildDate.Location = New System.Drawing.Point(157, 47)
        Me.lblRebuildDate.Name = "lblRebuildDate"
        Me.lblRebuildDate.Size = New System.Drawing.Size(75, 17)
        Me.lblRebuildDate.Text = "Rebuild Date"
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(194, 3)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 12
        '
        'custCtrlBtnBack
        '
        Me.custCtrlBtnBack.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnBack.Location = New System.Drawing.Point(182, 245)
        Me.custCtrlBtnBack.Name = "custCtrlBtnBack"
        Me.custCtrlBtnBack.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnBack.TabIndex = 30
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
        'frmSPPlannerListSP
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.custCtrlBtnBack)
        Me.Controls.Add(Me.lblSPHeading)
        Me.Controls.Add(Me.lblSelectPlnr)
        Me.Controls.Add(Me.lstView)
        Me.Controls.Add(Me.lblRebuildDate)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Name = "frmSPPlannerListSP"
        Me.Text = "Search Planner"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblSPHeading As System.Windows.Forms.Label
    Friend WithEvents lblSelectPlnr As System.Windows.Forms.Label
    Friend WithEvents lstView As MCShMon.AdvancedListView
    Friend WithEvents lblRebuildDate As System.Windows.Forms.Label
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents custCtrlBtnBack As CustomButtons.btn_Back_sm
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
End Class
