<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmOSSRPLItemDetails
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
        Me.lblOSSR = New System.Windows.Forms.Label
        Me.btnBackShopCalcpad = New CustomButtons.btn_CalcPad_small
        Me.lblPicked = New System.Windows.Forms.Label
        Me.lblOSSRQty = New System.Windows.Forms.Label
        Me.lblOffSiteHeader = New System.Windows.Forms.Label
        Me.lblStockFigure = New System.Windows.Forms.Label
        Me.lblStockFigureHeader = New System.Windows.Forms.Label
        Me.lblTotalItemCount = New System.Windows.Forms.Label
        Me.lblTotalItemCountHeader = New System.Windows.Forms.Label
        Me.lblProductDesc3 = New System.Windows.Forms.Label
        Me.lblProductDesc2 = New System.Windows.Forms.Label
        Me.lblProductDesc1 = New System.Windows.Forms.Label
        Me.custCtrlBtnQuit = New CustomButtons.btn_Quit_small
        Me.custCtrlBtnBack = New CustomButtons.btn_Back_sm
        Me.custCtrlBtnNext = New CustomButtons.btn_Next_small
        Me.lblStatusDisplay = New System.Windows.Forms.Label
        Me.lblStatus = New System.Windows.Forms.Label
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.lblItemPosition = New System.Windows.Forms.Label
        Me.lblProductCodeDisplay = New System.Windows.Forms.Label
        Me.lblBootsCodeDisplay = New System.Windows.Forms.Label
        Me.btn_OSSRItem = New CustomButtons.btn_OSSRItem
        Me.cmbLocation = New System.Windows.Forms.ComboBox
        Me.lblLocationHeader = New System.Windows.Forms.Label
        Me.lblOf = New System.Windows.Forms.Label
        Me.lblTotQtyRequired = New System.Windows.Forms.Label
        Me.lblQtyRequired = New System.Windows.Forms.Label
        Me.lblQtyReqHeader = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblOSSR
        '
        Me.lblOSSR.Location = New System.Drawing.Point(178, 30)
        Me.lblOSSR.Name = "lblOSSR"
        Me.lblOSSR.Size = New System.Drawing.Size(49, 16)
        Me.lblOSSR.Text = "OSSR"
        '
        'btnBackShopCalcpad
        '
        Me.btnBackShopCalcpad.BackColor = System.Drawing.Color.Transparent
        Me.btnBackShopCalcpad.Location = New System.Drawing.Point(208, 186)
        Me.btnBackShopCalcpad.Name = "btnBackShopCalcpad"
        Me.btnBackShopCalcpad.Size = New System.Drawing.Size(24, 28)
        Me.btnBackShopCalcpad.TabIndex = 110
        '
        'lblPicked
        '
        Me.lblPicked.Location = New System.Drawing.Point(177, 15)
        Me.lblPicked.Name = "lblPicked"
        Me.lblPicked.Size = New System.Drawing.Size(47, 18)
        Me.lblPicked.Text = "PICKED"
        '
        'lblOSSRQty
        '
        Me.lblOSSRQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblOSSRQty.Location = New System.Drawing.Point(163, 193)
        Me.lblOSSRQty.Name = "lblOSSRQty"
        Me.lblOSSRQty.Size = New System.Drawing.Size(39, 17)
        '
        'lblOffSiteHeader
        '
        Me.lblOffSiteHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblOffSiteHeader.Location = New System.Drawing.Point(4, 193)
        Me.lblOffSiteHeader.Name = "lblOffSiteHeader"
        Me.lblOffSiteHeader.Size = New System.Drawing.Size(160, 17)
        Me.lblOffSiteHeader.Text = "Enter Quantity on Shelf:"
        '
        'lblStockFigure
        '
        Me.lblStockFigure.Location = New System.Drawing.Point(163, 115)
        Me.lblStockFigure.Name = "lblStockFigure"
        Me.lblStockFigure.Size = New System.Drawing.Size(39, 17)
        '
        'lblStockFigureHeader
        '
        Me.lblStockFigureHeader.Location = New System.Drawing.Point(4, 115)
        Me.lblStockFigureHeader.Name = "lblStockFigureHeader"
        Me.lblStockFigureHeader.Size = New System.Drawing.Size(140, 17)
        Me.lblStockFigureHeader.Text = "Total Stock File:"
        '
        'lblTotalItemCount
        '
        Me.lblTotalItemCount.Location = New System.Drawing.Point(163, 131)
        Me.lblTotalItemCount.Name = "lblTotalItemCount"
        Me.lblTotalItemCount.Size = New System.Drawing.Size(39, 17)
        '
        'lblTotalItemCountHeader
        '
        Me.lblTotalItemCountHeader.Location = New System.Drawing.Point(4, 131)
        Me.lblTotalItemCountHeader.Name = "lblTotalItemCountHeader"
        Me.lblTotalItemCountHeader.Size = New System.Drawing.Size(140, 17)
        Me.lblTotalItemCountHeader.Text = "Total Item Count :"
        '
        'lblProductDesc3
        '
        Me.lblProductDesc3.Location = New System.Drawing.Point(4, 82)
        Me.lblProductDesc3.Name = "lblProductDesc3"
        Me.lblProductDesc3.Size = New System.Drawing.Size(160, 14)
        Me.lblProductDesc3.Text = "Product Description"
        '
        'lblProductDesc2
        '
        Me.lblProductDesc2.Location = New System.Drawing.Point(4, 65)
        Me.lblProductDesc2.Name = "lblProductDesc2"
        Me.lblProductDesc2.Size = New System.Drawing.Size(160, 15)
        Me.lblProductDesc2.Text = "Product Description"
        '
        'lblProductDesc1
        '
        Me.lblProductDesc1.Location = New System.Drawing.Point(4, 47)
        Me.lblProductDesc1.Name = "lblProductDesc1"
        Me.lblProductDesc1.Size = New System.Drawing.Size(160, 14)
        Me.lblProductDesc1.Text = "Product Description"
        '
        'custCtrlBtnQuit
        '
        Me.custCtrlBtnQuit.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnQuit.Location = New System.Drawing.Point(187, 247)
        Me.custCtrlBtnQuit.Name = "custCtrlBtnQuit"
        Me.custCtrlBtnQuit.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnQuit.TabIndex = 109
        '
        'custCtrlBtnBack
        '
        Me.custCtrlBtnBack.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnBack.Location = New System.Drawing.Point(56, 247)
        Me.custCtrlBtnBack.Name = "custCtrlBtnBack"
        Me.custCtrlBtnBack.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnBack.TabIndex = 108
        '
        'custCtrlBtnNext
        '
        Me.custCtrlBtnNext.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnNext.Location = New System.Drawing.Point(3, 247)
        Me.custCtrlBtnNext.Name = "custCtrlBtnNext"
        Me.custCtrlBtnNext.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnNext.TabIndex = 107
        '
        'lblStatusDisplay
        '
        Me.lblStatusDisplay.Location = New System.Drawing.Point(62, 99)
        Me.lblStatusDisplay.Name = "lblStatusDisplay"
        Me.lblStatusDisplay.Size = New System.Drawing.Size(162, 14)
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(4, 99)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(44, 14)
        Me.lblStatus.Text = "Status:"
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(184, 47)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 106
        '
        'lblItemPosition
        '
        Me.lblItemPosition.Location = New System.Drawing.Point(187, 0)
        Me.lblItemPosition.Name = "lblItemPosition"
        Me.lblItemPosition.Size = New System.Drawing.Size(45, 20)
        '
        'lblProductCodeDisplay
        '
        Me.lblProductCodeDisplay.Location = New System.Drawing.Point(4, 21)
        Me.lblProductCodeDisplay.Name = "lblProductCodeDisplay"
        Me.lblProductCodeDisplay.Size = New System.Drawing.Size(130, 20)
        Me.lblProductCodeDisplay.Text = "9-999999-999999"
        '
        'lblBootsCodeDisplay
        '
        Me.lblBootsCodeDisplay.Location = New System.Drawing.Point(4, 3)
        Me.lblBootsCodeDisplay.Name = "lblBootsCodeDisplay"
        Me.lblBootsCodeDisplay.Size = New System.Drawing.Size(100, 18)
        Me.lblBootsCodeDisplay.Text = "11-999-999"
        '
        'btn_OSSRItem
        '
        Me.btn_OSSRItem.BackColor = System.Drawing.Color.Transparent
        Me.btn_OSSRItem.Location = New System.Drawing.Point(110, 247)
        Me.btn_OSSRItem.Name = "btn_OSSRItem"
        Me.btn_OSSRItem.Size = New System.Drawing.Size(75, 24)
        Me.btn_OSSRItem.TabIndex = 148
        '
        'cmbLocation
        '
        Me.cmbLocation.Location = New System.Drawing.Point(4, 161)
        Me.cmbLocation.Name = "cmbLocation"
        Me.cmbLocation.Size = New System.Drawing.Size(229, 22)
        Me.cmbLocation.TabIndex = 168
        Me.cmbLocation.Visible = False
        '
        'lblLocationHeader
        '
        Me.lblLocationHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblLocationHeader.Location = New System.Drawing.Point(3, 147)
        Me.lblLocationHeader.Name = "lblLocationHeader"
        Me.lblLocationHeader.Size = New System.Drawing.Size(111, 17)
        Me.lblLocationHeader.Text = "Select Site"
        Me.lblLocationHeader.Visible = False
        '
        'lblOf
        '
        Me.lblOf.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblOf.Location = New System.Drawing.Point(193, 220)
        Me.lblOf.Name = "lblOf"
        Me.lblOf.Size = New System.Drawing.Size(17, 17)
        Me.lblOf.Text = "of"
        '
        'lblTotQtyRequired
        '
        Me.lblTotQtyRequired.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTotQtyRequired.Location = New System.Drawing.Point(209, 220)
        Me.lblTotQtyRequired.Name = "lblTotQtyRequired"
        Me.lblTotQtyRequired.Size = New System.Drawing.Size(28, 17)
        Me.lblTotQtyRequired.Text = "999"
        '
        'lblQtyRequired
        '
        Me.lblQtyRequired.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblQtyRequired.Location = New System.Drawing.Point(163, 220)
        Me.lblQtyRequired.Name = "lblQtyRequired"
        Me.lblQtyRequired.Size = New System.Drawing.Size(28, 17)
        Me.lblQtyRequired.Text = "999"
        Me.lblQtyRequired.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblQtyReqHeader
        '
        Me.lblQtyReqHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblQtyReqHeader.Location = New System.Drawing.Point(2, 220)
        Me.lblQtyReqHeader.Name = "lblQtyReqHeader"
        Me.lblQtyReqHeader.Size = New System.Drawing.Size(162, 17)
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 111
        '
        'frmOSSRPLItemDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblOf)
        Me.Controls.Add(Me.lblTotQtyRequired)
        Me.Controls.Add(Me.lblQtyRequired)
        Me.Controls.Add(Me.lblQtyReqHeader)
        Me.Controls.Add(Me.cmbLocation)
        Me.Controls.Add(Me.lblLocationHeader)
        Me.Controls.Add(Me.btn_OSSRItem)
        Me.Controls.Add(Me.lblOSSR)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnBackShopCalcpad)
        Me.Controls.Add(Me.lblPicked)
        Me.Controls.Add(Me.lblOSSRQty)
        Me.Controls.Add(Me.lblOffSiteHeader)
        Me.Controls.Add(Me.lblStockFigure)
        Me.Controls.Add(Me.lblStockFigureHeader)
        Me.Controls.Add(Me.lblTotalItemCount)
        Me.Controls.Add(Me.lblTotalItemCountHeader)
        Me.Controls.Add(Me.lblProductDesc3)
        Me.Controls.Add(Me.lblProductDesc2)
        Me.Controls.Add(Me.lblProductDesc1)
        Me.Controls.Add(Me.custCtrlBtnQuit)
        Me.Controls.Add(Me.custCtrlBtnBack)
        Me.Controls.Add(Me.custCtrlBtnNext)
        Me.Controls.Add(Me.lblStatusDisplay)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.lblItemPosition)
        Me.Controls.Add(Me.lblProductCodeDisplay)
        Me.Controls.Add(Me.lblBootsCodeDisplay)
        Me.Name = "frmOSSRPLItemDetails"
        Me.Text = "Picking List"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblOSSR As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents btnBackShopCalcpad As CustomButtons.btn_CalcPad_small
    Friend WithEvents lblPicked As System.Windows.Forms.Label
    Friend WithEvents lblOSSRQty As System.Windows.Forms.Label
    Friend WithEvents lblOffSiteHeader As System.Windows.Forms.Label
    Friend WithEvents lblStockFigure As System.Windows.Forms.Label
    Friend WithEvents lblStockFigureHeader As System.Windows.Forms.Label
    Friend WithEvents lblTotalItemCount As System.Windows.Forms.Label
    Friend WithEvents lblTotalItemCountHeader As System.Windows.Forms.Label
    Friend WithEvents lblProductDesc3 As System.Windows.Forms.Label
    Friend WithEvents lblProductDesc2 As System.Windows.Forms.Label
    Friend WithEvents lblProductDesc1 As System.Windows.Forms.Label
    Friend WithEvents custCtrlBtnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents custCtrlBtnBack As CustomButtons.btn_Back_sm
    Friend WithEvents custCtrlBtnNext As CustomButtons.btn_Next_small
    Friend WithEvents lblStatusDisplay As System.Windows.Forms.Label
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents lblItemPosition As System.Windows.Forms.Label
    Friend WithEvents lblProductCodeDisplay As System.Windows.Forms.Label
    Friend WithEvents lblBootsCodeDisplay As System.Windows.Forms.Label
    Friend WithEvents btn_OSSRItem As CustomButtons.btn_OSSRItem
    Friend WithEvents cmbLocation As System.Windows.Forms.ComboBox
    Friend WithEvents lblLocationHeader As System.Windows.Forms.Label
    Friend WithEvents lblOf As System.Windows.Forms.Label
    Friend WithEvents lblTotQtyRequired As System.Windows.Forms.Label
    Friend WithEvents lblQtyRequired As System.Windows.Forms.Label
    Friend WithEvents lblQtyReqHeader As System.Windows.Forms.Label
End Class
