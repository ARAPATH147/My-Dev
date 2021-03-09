<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmPSItemDetails
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
        Me.lblBootsCode = New System.Windows.Forms.Label
        Me.lblPdtCode = New System.Windows.Forms.Label
        Me.lblItemDescr1 = New System.Windows.Forms.Label
        Me.Btn_CalcPad_small1 = New CustomButtons.btn_CalcPad_small
        Me.lblStatus = New System.Windows.Forms.Label
        Me.lblStatusVal = New System.Windows.Forms.Label
        Me.lblEnterSELQty = New System.Windows.Forms.Label
        Me.lblSELQty = New System.Windows.Forms.Label
        Me.lblScanEnter = New System.Windows.Forms.Label
        Me.Btn_Print1 = New CustomButtons.btn_Print
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.Btn_CalcPad_small2 = New CustomButtons.btn_CalcPad_small
        Me.lblItemDescr2 = New System.Windows.Forms.Label
        Me.lblItemDescr3 = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.Btn_Quit_small1 = New CustomButtons.btn_Quit_small
        Me.SuspendLayout()
        '
        'lblBootsCode
        '
        Me.lblBootsCode.Location = New System.Drawing.Point(21, 23)
        Me.lblBootsCode.Name = "lblBootsCode"
        Me.lblBootsCode.Size = New System.Drawing.Size(100, 20)
        Me.lblBootsCode.Text = "BootsCode"
        '
        'lblPdtCode
        '
        Me.lblPdtCode.Location = New System.Drawing.Point(21, 40)
        Me.lblPdtCode.Name = "lblPdtCode"
        Me.lblPdtCode.Size = New System.Drawing.Size(142, 20)
        Me.lblPdtCode.Text = "PdtCode"
        '
        'lblItemDescr1
        '
        Me.lblItemDescr1.Location = New System.Drawing.Point(21, 60)
        Me.lblItemDescr1.Name = "lblItemDescr1"
        Me.lblItemDescr1.Size = New System.Drawing.Size(175, 16)
        Me.lblItemDescr1.Text = "Item Descr"
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(170, 23)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small1.TabIndex = 3
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(21, 121)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(54, 20)
        Me.lblStatus.Text = "Status :"
        '
        'lblStatusVal
        '
        Me.lblStatusVal.Location = New System.Drawing.Point(76, 121)
        Me.lblStatusVal.Name = "lblStatusVal"
        Me.lblStatusVal.Size = New System.Drawing.Size(161, 21)
        Me.lblStatusVal.Text = "statusVal"
        '
        'lblEnterSELQty
        '
        Me.lblEnterSELQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblEnterSELQty.Location = New System.Drawing.Point(21, 156)
        Me.lblEnterSELQty.Name = "lblEnterSELQty"
        Me.lblEnterSELQty.Size = New System.Drawing.Size(126, 21)
        Me.lblEnterSELQty.Text = "Enter SEL Quantity:"
        '
        'lblSELQty
        '
        Me.lblSELQty.Location = New System.Drawing.Point(153, 156)
        Me.lblSELQty.Name = "lblSELQty"
        Me.lblSELQty.Size = New System.Drawing.Size(37, 20)
        Me.lblSELQty.Text = "1"
        '
        'lblScanEnter
        '
        Me.lblScanEnter.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblScanEnter.Location = New System.Drawing.Point(21, 183)
        Me.lblScanEnter.Name = "lblScanEnter"
        Me.lblScanEnter.Size = New System.Drawing.Size(216, 28)
        Me.lblScanEnter.Text = "Scan/Enter next Item or Print to print SELs"
        '
        'Btn_Print1
        '
        Me.Btn_Print1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Print1.Location = New System.Drawing.Point(170, 233)
        Me.Btn_Print1.Name = "Btn_Print1"
        Me.Btn_Print1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Print1.TabIndex = 15
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(25, 228)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 16
        '
        'Btn_CalcPad_small2
        '
        Me.Btn_CalcPad_small2.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CalcPad_small2.Location = New System.Drawing.Point(196, 148)
        Me.Btn_CalcPad_small2.Name = "Btn_CalcPad_small2"
        Me.Btn_CalcPad_small2.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small2.TabIndex = 25
        '
        'lblItemDescr2
        '
        Me.lblItemDescr2.Location = New System.Drawing.Point(21, 77)
        Me.lblItemDescr2.Name = "lblItemDescr2"
        Me.lblItemDescr2.Size = New System.Drawing.Size(175, 16)
        Me.lblItemDescr2.Text = "Item Descr"
        '
        'lblItemDescr3
        '
        Me.lblItemDescr3.Location = New System.Drawing.Point(21, 93)
        Me.lblItemDescr3.Name = "lblItemDescr3"
        Me.lblItemDescr3.Size = New System.Drawing.Size(175, 16)
        Me.lblItemDescr3.Text = "Item Descr"
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 34
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(170, 233)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.TabIndex = 45
        '
        'frmPSItemDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblItemDescr3)
        Me.Controls.Add(Me.lblItemDescr2)
        Me.Controls.Add(Me.Btn_CalcPad_small2)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.Btn_Print1)
        Me.Controls.Add(Me.lblScanEnter)
        Me.Controls.Add(Me.lblSELQty)
        Me.Controls.Add(Me.lblEnterSELQty)
        Me.Controls.Add(Me.lblStatusVal)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.lblItemDescr1)
        Me.Controls.Add(Me.lblPdtCode)
        Me.Controls.Add(Me.lblBootsCode)
        Me.Name = "frmPSItemDetails"
        Me.Text = "Print SELs"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblBootsCode As System.Windows.Forms.Label
    Friend WithEvents lblPdtCode As System.Windows.Forms.Label
    Friend WithEvents lblItemDescr1 As System.Windows.Forms.Label
    Friend WithEvents Btn_CalcPad_small1 As CustomButtons.btn_CalcPad_small
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lblStatusVal As System.Windows.Forms.Label
    Friend WithEvents lblEnterSELQty As System.Windows.Forms.Label
    Friend WithEvents lblSELQty As System.Windows.Forms.Label
    Friend WithEvents lblScanEnter As System.Windows.Forms.Label
    Friend WithEvents Btn_Print1 As CustomButtons.btn_Print
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents Btn_CalcPad_small2 As CustomButtons.btn_CalcPad_small
    Friend WithEvents lblItemDescr2 As System.Windows.Forms.Label
    Friend WithEvents lblItemDescr3 As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents Btn_Quit_small1 As CustomButtons.btn_Quit_small
End Class
