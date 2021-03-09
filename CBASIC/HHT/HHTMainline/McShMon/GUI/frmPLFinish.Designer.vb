<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmPLFinish
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
        Me.custCtrlBtnNo = New CustomButtons.btn_No_sm_red
        Me.custCtrlBtnYes = New CustomButtons.btn_Yes_sm
        Me.lblConfirm = New System.Windows.Forms.Label
        Me.lblGapData = New System.Windows.Forms.Label
        Me.lblPLStatDisplay = New System.Windows.Forms.Label
        Me.lblItemsChecked = New System.Windows.Forms.Label
        Me.Btn_Help = New CustomButtons.btn_Info
        Me.lblSelectView = New System.Windows.Forms.Label
        Me.btnView = New CustomButtons.btnView
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'custCtrlBtnNo
        '
        Me.custCtrlBtnNo.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnNo.Location = New System.Drawing.Point(177, 233)
        Me.custCtrlBtnNo.Name = "custCtrlBtnNo"
        Me.custCtrlBtnNo.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnNo.TabIndex = 20
        '
        'custCtrlBtnYes
        '
        Me.custCtrlBtnYes.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnYes.Location = New System.Drawing.Point(12, 232)
        Me.custCtrlBtnYes.Name = "custCtrlBtnYes"
        Me.custCtrlBtnYes.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnYes.TabIndex = 19
        '
        'lblConfirm
        '
        Me.lblConfirm.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblConfirm.Location = New System.Drawing.Point(12, 193)
        Me.lblConfirm.Name = "lblConfirm"
        Me.lblConfirm.Size = New System.Drawing.Size(215, 20)
        Me.lblConfirm.Text = "Are you sure you want to Quit?"
        '
        'lblGapData
        '
        Me.lblGapData.Location = New System.Drawing.Point(21, 82)
        Me.lblGapData.Name = "lblGapData"
        Me.lblGapData.Size = New System.Drawing.Size(188, 20)
        '
        'lblPLStatDisplay
        '
        Me.lblPLStatDisplay.Location = New System.Drawing.Point(12, 16)
        Me.lblPLStatDisplay.Name = "lblPLStatDisplay"
        Me.lblPLStatDisplay.Size = New System.Drawing.Size(152, 36)
        Me.lblPLStatDisplay.Text = "Picking List Incomplete"
        '
        'lblItemsChecked
        '
        Me.lblItemsChecked.Enabled = False
        Me.lblItemsChecked.Location = New System.Drawing.Point(12, 43)
        Me.lblItemsChecked.Name = "lblItemsChecked"
        Me.lblItemsChecked.Size = New System.Drawing.Size(170, 20)
        Me.lblItemsChecked.Text = "<n> items not checked"
        Me.lblItemsChecked.Visible = False
        '
        'Btn_Help
        '
        Me.Btn_Help.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Help.Location = New System.Drawing.Point(195, 16)
        Me.Btn_Help.Name = "Btn_Help"
        Me.Btn_Help.Size = New System.Drawing.Size(32, 32)
        Me.Btn_Help.TabIndex = 161
        '
        'lblSelectView
        '
        Me.lblSelectView.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelectView.Location = New System.Drawing.Point(12, 129)
        Me.lblSelectView.Name = "lblSelectView"
        Me.lblSelectView.Size = New System.Drawing.Size(197, 34)
        Me.lblSelectView.Text = "To check items not completed, select View"
        Me.lblSelectView.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'btnView
        '
        Me.btnView.BackColor = System.Drawing.Color.Transparent
        Me.btnView.Location = New System.Drawing.Point(92, 233)
        Me.btnView.Name = "btnView"
        Me.btnView.Size = New System.Drawing.Size(58, 24)
        Me.btnView.TabIndex = 163
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 25
        '
        'frmPLFinish
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.lblSelectView)
        Me.Controls.Add(Me.Btn_Help)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.custCtrlBtnNo)
        Me.Controls.Add(Me.custCtrlBtnYes)
        Me.Controls.Add(Me.lblConfirm)
        Me.Controls.Add(Me.lblGapData)
        Me.Controls.Add(Me.lblItemsChecked)
        Me.Controls.Add(Me.lblPLStatDisplay)
        Me.Name = "frmPLFinish"
        Me.Text = "Picking List"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents custCtrlBtnNo As CustomButtons.btn_No_sm_red
    Friend WithEvents custCtrlBtnYes As CustomButtons.btn_Yes_sm
    Friend WithEvents lblConfirm As System.Windows.Forms.Label
    Friend WithEvents lblGapData As System.Windows.Forms.Label
    Friend WithEvents lblPLStatDisplay As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblItemsChecked As System.Windows.Forms.Label
    Friend WithEvents Btn_Help As CustomButtons.btn_Info
    Friend WithEvents lblSelectView As System.Windows.Forms.Label
    Friend WithEvents btnView As CustomButtons.btnView
End Class
