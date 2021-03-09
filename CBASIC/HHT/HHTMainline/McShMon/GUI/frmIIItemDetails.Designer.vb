<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmIIItemDetails
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
        Me.lblProductCode = New System.Windows.Forms.Label
        Me.lblProdDescription1 = New System.Windows.Forms.Label
        Me.lblStatus = New System.Windows.Forms.Label
        Me.lblPrice = New System.Windows.Forms.Label
        Me.lblStockFig = New System.Windows.Forms.Label
        Me.lblRedeemable = New System.Windows.Forms.Label
        Me.lblDealHeader = New System.Windows.Forms.Label
        Me.cmbDeal = New System.Windows.Forms.ComboBox
        Me.lblStatusText = New System.Windows.Forms.Label
        Me.lblPriceText = New System.Windows.Forms.Label
        Me.lblStockText = New System.Windows.Forms.Label
        Me.lblRedeemableText = New System.Windows.Forms.Label
        Me.lblProdDescription2 = New System.Windows.Forms.Label
        Me.lblProdDescription3 = New System.Windows.Forms.Label
        Me.lblCurrencySymbol = New System.Windows.Forms.Label
        Me.Btn_CalcPad_small1 = New CustomButtons.btn_CalcPad_small
        Me.Btn_Quit_small1 = New CustomButtons.btn_Quit_small
        Me.Btn_Print1 = New CustomButtons.btn_Print
        Me.PlannerNew1 = New CustomButtons.plannerNew
        Me.lblOSSR = New System.Windows.Forms.Label
        Me.btn_OSSRItem = New CustomButtons.btn_OSSRItem
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblBootsCode
        '
        Me.lblBootsCode.Location = New System.Drawing.Point(12, 7)
        Me.lblBootsCode.Name = "lblBootsCode"
        Me.lblBootsCode.Size = New System.Drawing.Size(110, 15)
        Me.lblBootsCode.Text = "99-99-9999"
        '
        'lblProductCode
        '
        Me.lblProductCode.Location = New System.Drawing.Point(12, 26)
        Me.lblProductCode.Name = "lblProductCode"
        Me.lblProductCode.Size = New System.Drawing.Size(120, 17)
        Me.lblProductCode.Text = "9-999999-999998"
        '
        'lblProdDescription1
        '
        Me.lblProdDescription1.Location = New System.Drawing.Point(12, 49)
        Me.lblProdDescription1.Name = "lblProdDescription1"
        Me.lblProdDescription1.Size = New System.Drawing.Size(140, 18)
        Me.lblProdDescription1.Text = "description 1"
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(12, 114)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(50, 15)
        Me.lblStatus.Text = "Status :"
        '
        'lblPrice
        '
        Me.lblPrice.Location = New System.Drawing.Point(12, 132)
        Me.lblPrice.Name = "lblPrice"
        Me.lblPrice.Size = New System.Drawing.Size(83, 15)
        Me.lblPrice.Text = "Price :"
        '
        'lblStockFig
        '
        Me.lblStockFig.Location = New System.Drawing.Point(12, 150)
        Me.lblStockFig.Name = "lblStockFig"
        Me.lblStockFig.Size = New System.Drawing.Size(147, 16)
        Me.lblStockFig.Text = "Start of Day Stock File:"
        '
        'lblRedeemable
        '
        Me.lblRedeemable.Location = New System.Drawing.Point(12, 175)
        Me.lblRedeemable.Name = "lblRedeemable"
        Me.lblRedeemable.Size = New System.Drawing.Size(100, 32)
        Me.lblRedeemable.Text = "Advantage" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Redeemable :" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'lblDealHeader
        '
        Me.lblDealHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblDealHeader.Location = New System.Drawing.Point(12, 207)
        Me.lblDealHeader.Name = "lblDealHeader"
        Me.lblDealHeader.Size = New System.Drawing.Size(165, 14)
        Me.lblDealHeader.Text = "Item Deal Information"
        '
        'cmbDeal
        '
        Me.cmbDeal.Location = New System.Drawing.Point(12, 224)
        Me.cmbDeal.Name = "cmbDeal"
        Me.cmbDeal.Size = New System.Drawing.Size(216, 22)
        Me.cmbDeal.TabIndex = 8
        '
        'lblStatusText
        '
        Me.lblStatusText.Location = New System.Drawing.Point(65, 114)
        Me.lblStatusText.Name = "lblStatusText"
        Me.lblStatusText.Size = New System.Drawing.Size(161, 14)
        '
        'lblPriceText
        '
        Me.lblPriceText.Location = New System.Drawing.Point(168, 132)
        Me.lblPriceText.Name = "lblPriceText"
        Me.lblPriceText.Size = New System.Drawing.Size(43, 15)
        Me.lblPriceText.Text = "000.00"
        '
        'lblStockText
        '
        Me.lblStockText.Location = New System.Drawing.Point(151, 151)
        Me.lblStockText.Name = "lblStockText"
        Me.lblStockText.Size = New System.Drawing.Size(70, 15)
        '
        'lblRedeemableText
        '
        Me.lblRedeemableText.Location = New System.Drawing.Point(151, 185)
        Me.lblRedeemableText.Name = "lblRedeemableText"
        Me.lblRedeemableText.Size = New System.Drawing.Size(75, 22)
        '
        'lblProdDescription2
        '
        Me.lblProdDescription2.Location = New System.Drawing.Point(12, 67)
        Me.lblProdDescription2.Name = "lblProdDescription2"
        Me.lblProdDescription2.Size = New System.Drawing.Size(140, 19)
        Me.lblProdDescription2.Text = "description2"
        '
        'lblProdDescription3
        '
        Me.lblProdDescription3.Location = New System.Drawing.Point(12, 86)
        Me.lblProdDescription3.Name = "lblProdDescription3"
        Me.lblProdDescription3.Size = New System.Drawing.Size(140, 22)
        Me.lblProdDescription3.Text = "description3"
        '
        'lblCurrencySymbol
        '
        Me.lblCurrencySymbol.Location = New System.Drawing.Point(151, 132)
        Me.lblCurrencySymbol.Name = "lblCurrencySymbol"
        Me.lblCurrencySymbol.Size = New System.Drawing.Size(11, 15)
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(143, 13)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small1.TabIndex = 13
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(187, 248)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.TabIndex = 12
        '
        'Btn_Print1
        '
        Me.Btn_Print1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Print1.Location = New System.Drawing.Point(4, 248)
        Me.Btn_Print1.Name = "Btn_Print1"
        Me.Btn_Print1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Print1.TabIndex = 10
        '
        'PlannerNew1
        '
        Me.PlannerNew1.BackColor = System.Drawing.Color.Transparent
        Me.PlannerNew1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular)
        Me.PlannerNew1.Location = New System.Drawing.Point(58, 248)
        Me.PlannerNew1.Name = "PlannerNew1"
        Me.PlannerNew1.Size = New System.Drawing.Size(75, 24)
        Me.PlannerNew1.TabIndex = 71
        '
        'lblOSSR
        '
        Me.lblOSSR.Location = New System.Drawing.Point(178, 19)
        Me.lblOSSR.Name = "lblOSSR"
        Me.lblOSSR.Size = New System.Drawing.Size(48, 20)
        '
        'btn_OSSRItem
        '
        Me.btn_OSSRItem.BackColor = System.Drawing.Color.Transparent
        Me.btn_OSSRItem.Location = New System.Drawing.Point(143, 47)
        Me.btn_OSSRItem.Name = "btn_OSSRItem"
        Me.btn_OSSRItem.Size = New System.Drawing.Size(75, 24)
        Me.btn_OSSRItem.TabIndex = 105
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 37
        '
        'frmIIItemDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.btn_OSSRItem)
        Me.Controls.Add(Me.lblOSSR)
        Me.Controls.Add(Me.PlannerNew1)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblCurrencySymbol)
        Me.Controls.Add(Me.lblProdDescription3)
        Me.Controls.Add(Me.lblProdDescription2)
        Me.Controls.Add(Me.lblRedeemableText)
        Me.Controls.Add(Me.lblStockText)
        Me.Controls.Add(Me.lblPriceText)
        Me.Controls.Add(Me.lblStatusText)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Btn_Print1)
        Me.Controls.Add(Me.cmbDeal)
        Me.Controls.Add(Me.lblDealHeader)
        Me.Controls.Add(Me.lblRedeemable)
        Me.Controls.Add(Me.lblPrice)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.lblProdDescription1)
        Me.Controls.Add(Me.lblProductCode)
        Me.Controls.Add(Me.lblBootsCode)
        Me.Controls.Add(Me.lblStockFig)
        Me.Name = "frmIIItemDetails"
        Me.Text = "Item Info"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblBootsCode As System.Windows.Forms.Label
    Friend WithEvents lblProductCode As System.Windows.Forms.Label
    Friend WithEvents lblProdDescription1 As System.Windows.Forms.Label
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lblPrice As System.Windows.Forms.Label
    Friend WithEvents lblStockFig As System.Windows.Forms.Label
    Friend WithEvents lblRedeemable As System.Windows.Forms.Label
    Friend WithEvents lblDealHeader As System.Windows.Forms.Label
    Friend WithEvents cmbDeal As System.Windows.Forms.ComboBox
    Friend WithEvents Btn_Print1 As CustomButtons.btn_Print
    Friend WithEvents Btn_Quit_small1 As CustomButtons.btn_Quit_small
    Friend WithEvents Btn_CalcPad_small1 As CustomButtons.btn_CalcPad_small
    Friend WithEvents lblStatusText As System.Windows.Forms.Label
    Friend WithEvents lblPriceText As System.Windows.Forms.Label
    Friend WithEvents lblStockText As System.Windows.Forms.Label
    Friend WithEvents lblRedeemableText As System.Windows.Forms.Label
    Friend WithEvents lblProdDescription2 As System.Windows.Forms.Label
    Friend WithEvents lblProdDescription3 As System.Windows.Forms.Label
    Friend WithEvents lblCurrencySymbol As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents PlannerNew1 As CustomButtons.plannerNew
    Friend WithEvents lblOSSR As System.Windows.Forms.Label
    Friend WithEvents btn_OSSRItem As CustomButtons.btn_OSSRItem

End Class
