<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmPlannerItemInfo
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
        Me.lblProdDescription3 = New System.Windows.Forms.Label
        Me.lblProdDescription2 = New System.Windows.Forms.Label
        Me.lblRedeemableText = New System.Windows.Forms.Label
        Me.lblStockText = New System.Windows.Forms.Label
        Me.lblPriceText = New System.Windows.Forms.Label
        Me.lblStatusText = New System.Windows.Forms.Label
        Me.cmbDeal = New System.Windows.Forms.ComboBox
        Me.lblDealHeader = New System.Windows.Forms.Label
        Me.lblRedeemable = New System.Windows.Forms.Label
        Me.lblStockFig = New System.Windows.Forms.Label
        Me.lblPrice = New System.Windows.Forms.Label
        Me.lblStatus = New System.Windows.Forms.Label
        Me.lblProdDescription1 = New System.Windows.Forms.Label
        Me.lblProductCode = New System.Windows.Forms.Label
        Me.lblBootsCode = New System.Windows.Forms.Label
        Me.Btn_Quit_small1 = New CustomButtons.btn_Quit_small
        Me.Btn_Print1 = New CustomButtons.btn_Print
        Me.PlannerNew1 = New CustomButtons.plannerNew
        Me.lblCurrencySymbol = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblProdDescription3
        '
        Me.lblProdDescription3.Location = New System.Drawing.Point(11, 89)
        Me.lblProdDescription3.Name = "lblProdDescription3"
        Me.lblProdDescription3.Size = New System.Drawing.Size(140, 13)
        Me.lblProdDescription3.Text = "description3"
        '
        'lblProdDescription2
        '
        Me.lblProdDescription2.Location = New System.Drawing.Point(11, 70)
        Me.lblProdDescription2.Name = "lblProdDescription2"
        Me.lblProdDescription2.Size = New System.Drawing.Size(140, 13)
        Me.lblProdDescription2.Text = "description2"
        '
        'lblRedeemableText
        '
        Me.lblRedeemableText.Location = New System.Drawing.Point(127, 184)
        Me.lblRedeemableText.Name = "lblRedeemableText"
        Me.lblRedeemableText.Size = New System.Drawing.Size(100, 20)
        '
        'lblStockText
        '
        Me.lblStockText.Location = New System.Drawing.Point(159, 156)
        Me.lblStockText.Name = "lblStockText"
        Me.lblStockText.Size = New System.Drawing.Size(71, 15)
        '
        'lblPriceText
        '
        Me.lblPriceText.Location = New System.Drawing.Point(144, 137)
        Me.lblPriceText.Name = "lblPriceText"
        Me.lblPriceText.Size = New System.Drawing.Size(46, 15)
        '
        'lblStatusText
        '
        Me.lblStatusText.Location = New System.Drawing.Point(75, 111)
        Me.lblStatusText.Name = "lblStatusText"
        Me.lblStatusText.Size = New System.Drawing.Size(161, 21)
        '
        'cmbDeal
        '
        Me.cmbDeal.Location = New System.Drawing.Point(11, 224)
        Me.cmbDeal.Name = "cmbDeal"
        Me.cmbDeal.Size = New System.Drawing.Size(216, 22)
        Me.cmbDeal.TabIndex = 53
        Me.cmbDeal.Visible = False
        '
        'lblDealHeader
        '
        Me.lblDealHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblDealHeader.Location = New System.Drawing.Point(11, 207)
        Me.lblDealHeader.Name = "lblDealHeader"
        Me.lblDealHeader.Size = New System.Drawing.Size(165, 14)
        Me.lblDealHeader.Text = "Item Deal Information"
        Me.lblDealHeader.Visible = False
        '
        'lblRedeemable
        '
        Me.lblRedeemable.Location = New System.Drawing.Point(11, 175)
        Me.lblRedeemable.Name = "lblRedeemable"
        Me.lblRedeemable.Size = New System.Drawing.Size(100, 32)
        Me.lblRedeemable.Text = "Advantage" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Redeemable :" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'lblStockFig
        '
        Me.lblStockFig.Location = New System.Drawing.Point(11, 156)
        Me.lblStockFig.Name = "lblStockFig"
        Me.lblStockFig.Size = New System.Drawing.Size(156, 15)
        Me.lblStockFig.Text = "Start of Day Stock Figure:"
        '
        'lblPrice
        '
        Me.lblPrice.Location = New System.Drawing.Point(11, 138)
        Me.lblPrice.Name = "lblPrice"
        Me.lblPrice.Size = New System.Drawing.Size(83, 15)
        Me.lblPrice.Text = "Price :"
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(11, 111)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(58, 15)
        Me.lblStatus.Text = "Status :"
        '
        'lblProdDescription1
        '
        Me.lblProdDescription1.Location = New System.Drawing.Point(11, 52)
        Me.lblProdDescription1.Name = "lblProdDescription1"
        Me.lblProdDescription1.Size = New System.Drawing.Size(140, 13)
        Me.lblProdDescription1.Text = "description 1"
        '
        'lblProductCode
        '
        Me.lblProductCode.Location = New System.Drawing.Point(11, 26)
        Me.lblProductCode.Name = "lblProductCode"
        Me.lblProductCode.Size = New System.Drawing.Size(120, 17)
        Me.lblProductCode.Text = "9-999999-999998"
        '
        'lblBootsCode
        '
        Me.lblBootsCode.Location = New System.Drawing.Point(11, 7)
        Me.lblBootsCode.Name = "lblBootsCode"
        Me.lblBootsCode.Size = New System.Drawing.Size(110, 15)
        Me.lblBootsCode.Text = "99-99-9999"
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(177, 248)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.TabIndex = 55
        '
        'Btn_Print1
        '
        Me.Btn_Print1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Print1.Location = New System.Drawing.Point(11, 248)
        Me.Btn_Print1.Name = "Btn_Print1"
        Me.Btn_Print1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Print1.TabIndex = 54
        '
        'PlannerNew1
        '
        Me.PlannerNew1.BackColor = System.Drawing.Color.Transparent
        Me.PlannerNew1.Location = New System.Drawing.Point(86, 248)
        Me.PlannerNew1.Name = "PlannerNew1"
        Me.PlannerNew1.Size = New System.Drawing.Size(70, 24)
        Me.PlannerNew1.TabIndex = 90
        '
        'lblCurrencySymbol
        '
        Me.lblCurrencySymbol.Location = New System.Drawing.Point(131, 137)
        Me.lblCurrencySymbol.Name = "lblCurrencySymbol"
        Me.lblCurrencySymbol.Size = New System.Drawing.Size(11, 15)
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 58
        '
        'frmPlannerItemInfo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblCurrencySymbol)
        Me.Controls.Add(Me.PlannerNew1)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblProdDescription3)
        Me.Controls.Add(Me.lblProdDescription2)
        Me.Controls.Add(Me.lblRedeemableText)
        Me.Controls.Add(Me.lblStockText)
        Me.Controls.Add(Me.lblPriceText)
        Me.Controls.Add(Me.lblStatusText)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Btn_Print1)
        Me.Controls.Add(Me.cmbDeal)
        Me.Controls.Add(Me.lblDealHeader)
        Me.Controls.Add(Me.lblRedeemable)
        Me.Controls.Add(Me.lblStockFig)
        Me.Controls.Add(Me.lblPrice)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.lblProdDescription1)
        Me.Controls.Add(Me.lblProductCode)
        Me.Controls.Add(Me.lblBootsCode)
        Me.Name = "frmPlannerItemInfo"
        Me.Text = "Planner"
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblProdDescription3 As System.Windows.Forms.Label
    Friend WithEvents lblProdDescription2 As System.Windows.Forms.Label
    Friend WithEvents lblRedeemableText As System.Windows.Forms.Label
    Friend WithEvents lblStockText As System.Windows.Forms.Label
    Friend WithEvents lblPriceText As System.Windows.Forms.Label
    Friend WithEvents lblStatusText As System.Windows.Forms.Label
    Friend WithEvents Btn_Quit_small1 As CustomButtons.btn_Quit_small
    Friend WithEvents Btn_Print1 As CustomButtons.btn_Print
    Friend WithEvents cmbDeal As System.Windows.Forms.ComboBox
    Friend WithEvents lblDealHeader As System.Windows.Forms.Label
    Friend WithEvents lblRedeemable As System.Windows.Forms.Label
    Friend WithEvents lblStockFig As System.Windows.Forms.Label
    Friend WithEvents lblPrice As System.Windows.Forms.Label
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lblProdDescription1 As System.Windows.Forms.Label
    Friend WithEvents lblProductCode As System.Windows.Forms.Label
    Friend WithEvents lblBootsCode As System.Windows.Forms.Label
    Friend WithEvents PlannerNew1 As CustomButtons.plannerNew
    Friend WithEvents lblCurrencySymbol As System.Windows.Forms.Label
End Class
