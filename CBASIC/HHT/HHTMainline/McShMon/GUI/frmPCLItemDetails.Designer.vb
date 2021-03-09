<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmPCLItemDetails
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
        Me.Btn_CalcPad_small2 = New CustomButtons.btn_CalcPad_small
        Me.lblItemDescr2 = New System.Windows.Forms.Label
        Me.lblItemDescr3 = New System.Windows.Forms.Label
        Me.lblClearance = New System.Windows.Forms.Label
        Me.lblClearancePrice = New System.Windows.Forms.Label
        Me.Btn_CalcPad_small3 = New CustomButtons.btn_CalcPad_small
        Me.lblCurrency = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.Btn_Quit_small1 = New CustomButtons.btn_Quit_small
        Me.SuspendLayout()
        '
        'lblBootsCode
        '
        Me.lblBootsCode.Location = New System.Drawing.Point(21, 13)
        Me.lblBootsCode.Name = "lblBootsCode"
        Me.lblBootsCode.Size = New System.Drawing.Size(100, 20)
        Me.lblBootsCode.Text = "BootsCode"
        '
        'lblPdtCode
        '
        Me.lblPdtCode.Location = New System.Drawing.Point(21, 30)
        Me.lblPdtCode.Name = "lblPdtCode"
        Me.lblPdtCode.Size = New System.Drawing.Size(142, 20)
        Me.lblPdtCode.Text = "PdtCode"
        '
        'lblItemDescr1
        '
        Me.lblItemDescr1.Location = New System.Drawing.Point(21, 48)
        Me.lblItemDescr1.Name = "lblItemDescr1"
        Me.lblItemDescr1.Size = New System.Drawing.Size(175, 16)
        Me.lblItemDescr1.Text = "Item Descr"
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(170, 13)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small1.TabIndex = 3
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(21, 103)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(54, 20)
        Me.lblStatus.Text = "Status :"
        '
        'lblStatusVal
        '
        Me.lblStatusVal.Location = New System.Drawing.Point(76, 103)
        Me.lblStatusVal.Name = "lblStatusVal"
        Me.lblStatusVal.Size = New System.Drawing.Size(161, 21)
        Me.lblStatusVal.Text = "statusVal"
        '
        'lblEnterSELQty
        '
        Me.lblEnterSELQty.Location = New System.Drawing.Point(21, 164)
        Me.lblEnterSELQty.Name = "lblEnterSELQty"
        Me.lblEnterSELQty.Size = New System.Drawing.Size(137, 21)
        Me.lblEnterSELQty.Text = "Label Quantity:"
        '
        'lblSELQty
        '
        Me.lblSELQty.Location = New System.Drawing.Point(159, 164)
        Me.lblSELQty.Name = "lblSELQty"
        Me.lblSELQty.Size = New System.Drawing.Size(37, 20)
        Me.lblSELQty.Text = "1"
        '
        'lblScanEnter
        '
        Me.lblScanEnter.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblScanEnter.Location = New System.Drawing.Point(21, 195)
        Me.lblScanEnter.Name = "lblScanEnter"
        Me.lblScanEnter.Size = New System.Drawing.Size(216, 28)
        Me.lblScanEnter.Text = "Scan/Enter next item or print labels"
        '
        'Btn_Print1
        '
        Me.Btn_Print1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Print1.Location = New System.Drawing.Point(170, 233)
        Me.Btn_Print1.Name = "Btn_Print1"
        Me.Btn_Print1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Print1.TabIndex = 15
        '
        'Btn_CalcPad_small2
        '
        Me.Btn_CalcPad_small2.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CalcPad_small2.Location = New System.Drawing.Point(205, 158)
        Me.Btn_CalcPad_small2.Name = "Btn_CalcPad_small2"
        Me.Btn_CalcPad_small2.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small2.TabIndex = 25
        Me.Btn_CalcPad_small2.Visible = False
        '
        'lblItemDescr2
        '
        Me.lblItemDescr2.Location = New System.Drawing.Point(21, 65)
        Me.lblItemDescr2.Name = "lblItemDescr2"
        Me.lblItemDescr2.Size = New System.Drawing.Size(175, 16)
        Me.lblItemDescr2.Text = "Item Descr"
        '
        'lblItemDescr3
        '
        Me.lblItemDescr3.Location = New System.Drawing.Point(21, 81)
        Me.lblItemDescr3.Name = "lblItemDescr3"
        Me.lblItemDescr3.Size = New System.Drawing.Size(175, 16)
        Me.lblItemDescr3.Text = "Item Descr"
        '
        'lblClearance
        '
        Me.lblClearance.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblClearance.Location = New System.Drawing.Point(21, 130)
        Me.lblClearance.Name = "lblClearance"
        Me.lblClearance.Size = New System.Drawing.Size(105, 21)
        Me.lblClearance.Text = "Clearance price:"
        '
        'lblClearancePrice
        '
        Me.lblClearancePrice.Location = New System.Drawing.Point(146, 131)
        Me.lblClearancePrice.Name = "lblClearancePrice"
        Me.lblClearancePrice.Size = New System.Drawing.Size(53, 20)
        Me.lblClearancePrice.Text = "9999.99"
        '
        'Btn_CalcPad_small3
        '
        Me.Btn_CalcPad_small3.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CalcPad_small3.Location = New System.Drawing.Point(205, 124)
        Me.Btn_CalcPad_small3.Name = "Btn_CalcPad_small3"
        Me.Btn_CalcPad_small3.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small3.TabIndex = 25
        '
        'lblCurrency
        '
        Me.lblCurrency.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Regular)
        Me.lblCurrency.Location = New System.Drawing.Point(132, 130)
        Me.lblCurrency.Name = "lblCurrency"
        Me.lblCurrency.Size = New System.Drawing.Size(15, 20)
        Me.lblCurrency.Text = "£"
        Me.lblCurrency.TextAlign = System.Drawing.ContentAlignment.TopCenter
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
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(21, 233)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.TabIndex = 48
        '
        'frmPCLItemDetails
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
        Me.Controls.Add(Me.Btn_CalcPad_small3)
        Me.Controls.Add(Me.Btn_CalcPad_small2)
        Me.Controls.Add(Me.Btn_Print1)
        Me.Controls.Add(Me.lblScanEnter)
        Me.Controls.Add(Me.lblClearancePrice)
        Me.Controls.Add(Me.lblCurrency)
        Me.Controls.Add(Me.lblSELQty)
        Me.Controls.Add(Me.lblClearance)
        Me.Controls.Add(Me.lblEnterSELQty)
        Me.Controls.Add(Me.lblStatusVal)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.lblItemDescr1)
        Me.Controls.Add(Me.lblPdtCode)
        Me.Controls.Add(Me.lblBootsCode)
        Me.Name = "frmPCLItemDetails"
        Me.Text = "Print Clearance"
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
    Friend WithEvents Btn_CalcPad_small2 As CustomButtons.btn_CalcPad_small
    Friend WithEvents lblItemDescr2 As System.Windows.Forms.Label
    Friend WithEvents lblItemDescr3 As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblClearance As System.Windows.Forms.Label
    Friend WithEvents lblClearancePrice As System.Windows.Forms.Label
    Friend WithEvents Btn_CalcPad_small3 As CustomButtons.btn_CalcPad_small
    Friend WithEvents lblCurrency As System.Windows.Forms.Label
    Friend WithEvents Btn_Quit_small1 As CustomButtons.btn_Quit_small
End Class
