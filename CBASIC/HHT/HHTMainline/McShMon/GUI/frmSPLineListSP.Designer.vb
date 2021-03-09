<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSPLineListSP
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
        Me.lblSelectOption = New System.Windows.Forms.Label
        Me.Btn_Print1 = New CustomButtons.btn_Print
        Me.Btn_Quit_small1 = New CustomButtons.btn_Quit_small
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblRebuildDate = New System.Windows.Forms.Label
        Me.lblModule = New System.Windows.Forms.Label
        Me.Btn_Next_small1 = New CustomButtons.btn_Next_small
        Me.custCtrlBtnBack = New CustomButtons.btn_Back_sm
        Me.lstView = New MCShMon.AdvancedListView
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblSelectOption
        '
        Me.lblSelectOption.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelectOption.Location = New System.Drawing.Point(5, 221)
        Me.lblSelectOption.Name = "lblSelectOption"
        Me.lblSelectOption.Size = New System.Drawing.Size(128, 17)
        Me.lblSelectOption.Text = "Select Option"
        '
        'Btn_Print1
        '
        Me.Btn_Print1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Print1.Location = New System.Drawing.Point(65, 244)
        Me.Btn_Print1.Name = "Btn_Print1"
        Me.Btn_Print1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Print1.TabIndex = 29
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(180, 244)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.TabIndex = 28
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(180, 40)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(56, 20)
        Me.Label1.Text = "29/12/08"
        '
        'lblRebuildDate
        '
        Me.lblRebuildDate.Location = New System.Drawing.Point(97, 41)
        Me.lblRebuildDate.Name = "lblRebuildDate"
        Me.lblRebuildDate.Size = New System.Drawing.Size(82, 17)
        Me.lblRebuildDate.Text = "Rebuild Date"
        '
        'lblModule
        '
        Me.lblModule.Font = New System.Drawing.Font("Tahoma", 11.0!, System.Drawing.FontStyle.Regular)
        Me.lblModule.Location = New System.Drawing.Point(12, 20)
        Me.lblModule.Name = "lblModule"
        Me.lblModule.Size = New System.Drawing.Size(174, 20)
        Me.lblModule.Text = "Module"
        '
        'Btn_Next_small1
        '
        Me.Btn_Next_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Next_small1.Location = New System.Drawing.Point(9, 244)
        Me.Btn_Next_small1.Name = "Btn_Next_small1"
        Me.Btn_Next_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Next_small1.TabIndex = 30
        '
        'custCtrlBtnBack
        '
        Me.custCtrlBtnBack.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnBack.Location = New System.Drawing.Point(121, 243)
        Me.custCtrlBtnBack.Name = "custCtrlBtnBack"
        Me.custCtrlBtnBack.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnBack.TabIndex = 35
        '
        'lstView
        '
        Me.lstView.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.lstView.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lstView.FullRowSelect = True
        Me.lstView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lstView.Location = New System.Drawing.Point(7, 58)
        Me.lstView.Name = "lstView"
        Me.lstView.Size = New System.Drawing.Size(223, 158)
        Me.lstView.TabIndex = 27
        Me.lstView.View = System.Windows.Forms.View.Details
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 40
        '
        'frmSPLineListSP
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.custCtrlBtnBack)
        Me.Controls.Add(Me.Btn_Next_small1)
        Me.Controls.Add(Me.lblSelectOption)
        Me.Controls.Add(Me.Btn_Print1)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.lstView)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblRebuildDate)
        Me.Controls.Add(Me.lblModule)
        Me.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.Name = "frmSPLineListSP"
        Me.Text = "Search Planner"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblSelectOption As System.Windows.Forms.Label
    Friend WithEvents Btn_Print1 As CustomButtons.btn_Print
    Friend WithEvents Btn_Quit_small1 As CustomButtons.btn_Quit_small
    Friend WithEvents lstView As MCShMon.AdvancedListView
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblRebuildDate As System.Windows.Forms.Label
    Friend WithEvents lblModule As System.Windows.Forms.Label
    Friend WithEvents Btn_Next_small1 As CustomButtons.btn_Next_small
    Friend WithEvents custCtrlBtnBack As CustomButtons.btn_Back_sm
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
End Class
