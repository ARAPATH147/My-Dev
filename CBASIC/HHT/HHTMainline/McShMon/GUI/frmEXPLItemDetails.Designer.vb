<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmEXPLItemDetails
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEXPLItemDetails))
        Me.lblCounted = New System.Windows.Forms.Label
        Me.lblCurrentQty = New System.Windows.Forms.Label
        Me.lblSalesFloorHeader = New System.Windows.Forms.Label
        Me.lblStockFigure = New System.Windows.Forms.Label
        Me.lblProductDesc3 = New System.Windows.Forms.Label
        Me.lblProductDesc2 = New System.Windows.Forms.Label
        Me.lblProductDesc1 = New System.Windows.Forms.Label
        Me.lblStatusDisplay = New System.Windows.Forms.Label
        Me.lblStatus = New System.Windows.Forms.Label
        Me.lblItemPosition = New System.Windows.Forms.Label
        Me.lblProductCodeDisplay = New System.Windows.Forms.Label
        Me.lblBootsCodeDisplay = New System.Windows.Forms.Label
        Me.lblStockFigureHeader = New System.Windows.Forms.Label
        Me.lblLocationHeader = New System.Windows.Forms.Label
        Me.cmbLocation = New System.Windows.Forms.ComboBox
        Me.btnSalesFloorCalcpad = New CustomButtons.btn_CalcPad_small
        Me.custCtrlBtnQuit = New CustomButtons.btn_Quit_small
        Me.custCtrlBtnBack = New CustomButtons.btn_Back_sm
        Me.custCtrlBtnNext = New CustomButtons.btn_Next_small
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.lblBackshopHeader = New System.Windows.Forms.Label
        Me.lblTotalItemQty = New System.Windows.Forms.Label
        Me.Btn_OSSRItem1 = New CustomButtons.btn_OSSRItem
        Me.lblOSSR = New System.Windows.Forms.Label
        Me.btnView = New System.Windows.Forms.PictureBox
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.btnZero = New CustomButtons.btn_Zero
        Me.SuspendLayout()
        '
        'lblCounted
        '
        Me.lblCounted.Location = New System.Drawing.Point(168, 15)
        Me.lblCounted.Name = "lblCounted"
        Me.lblCounted.Size = New System.Drawing.Size(68, 15)
        Me.lblCounted.Text = "COUNTED"
        '
        'lblCurrentQty
        '
        Me.lblCurrentQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblCurrentQty.Location = New System.Drawing.Point(163, 197)
        Me.lblCurrentQty.Name = "lblCurrentQty"
        Me.lblCurrentQty.Size = New System.Drawing.Size(38, 17)
        Me.lblCurrentQty.Text = "1111"
        '
        'lblSalesFloorHeader
        '
        Me.lblSalesFloorHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSalesFloorHeader.Location = New System.Drawing.Point(4, 197)
        Me.lblSalesFloorHeader.Name = "lblSalesFloorHeader"
        Me.lblSalesFloorHeader.Size = New System.Drawing.Size(157, 17)
        Me.lblSalesFloorHeader.Text = "Enter Sales Floor Qty:"
        '
        'lblStockFigure
        '
        Me.lblStockFigure.Location = New System.Drawing.Point(168, 115)
        Me.lblStockFigure.Name = "lblStockFigure"
        Me.lblStockFigure.Size = New System.Drawing.Size(38, 17)
        Me.lblStockFigure.Text = "1111"
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
        'lblStatusDisplay
        '
        Me.lblStatusDisplay.Location = New System.Drawing.Point(64, 99)
        Me.lblStatusDisplay.Name = "lblStatusDisplay"
        Me.lblStatusDisplay.Size = New System.Drawing.Size(165, 15)
        Me.lblStatusDisplay.Text = "Outstanding order cancelled"
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(4, 99)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(58, 17)
        Me.lblStatus.Text = "Status :"
        '
        'lblItemPosition
        '
        Me.lblItemPosition.Location = New System.Drawing.Point(187, 0)
        Me.lblItemPosition.Name = "lblItemPosition"
        Me.lblItemPosition.Size = New System.Drawing.Size(49, 15)
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
        'lblStockFigureHeader
        '
        Me.lblStockFigureHeader.Location = New System.Drawing.Point(4, 115)
        Me.lblStockFigureHeader.Name = "lblStockFigureHeader"
        Me.lblStockFigureHeader.Size = New System.Drawing.Size(149, 17)
        Me.lblStockFigureHeader.Text = "Total Stock File at 99:99 :"
        '
        'lblLocationHeader
        '
        Me.lblLocationHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblLocationHeader.Location = New System.Drawing.Point(3, 147)
        Me.lblLocationHeader.Name = "lblLocationHeader"
        Me.lblLocationHeader.Size = New System.Drawing.Size(111, 17)
        Me.lblLocationHeader.Text = "Select Site"
        '
        'cmbLocation
        '
        Me.cmbLocation.Location = New System.Drawing.Point(4, 167)
        Me.cmbLocation.Name = "cmbLocation"
        Me.cmbLocation.Size = New System.Drawing.Size(229, 22)
        Me.cmbLocation.TabIndex = 92
        '
        'btnSalesFloorCalcpad
        '
        Me.btnSalesFloorCalcpad.BackColor = System.Drawing.Color.Transparent
        Me.btnSalesFloorCalcpad.Location = New System.Drawing.Point(209, 193)
        Me.btnSalesFloorCalcpad.Name = "btnSalesFloorCalcpad"
        Me.btnSalesFloorCalcpad.Size = New System.Drawing.Size(24, 28)
        Me.btnSalesFloorCalcpad.TabIndex = 107
        '
        'custCtrlBtnQuit
        '
        Me.custCtrlBtnQuit.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnQuit.Location = New System.Drawing.Point(183, 246)
        Me.custCtrlBtnQuit.Name = "custCtrlBtnQuit"
        Me.custCtrlBtnQuit.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnQuit.TabIndex = 84
        '
        'custCtrlBtnBack
        '
        Me.custCtrlBtnBack.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnBack.Location = New System.Drawing.Point(123, 246)
        Me.custCtrlBtnBack.Name = "custCtrlBtnBack"
        Me.custCtrlBtnBack.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnBack.TabIndex = 83
        '
        'custCtrlBtnNext
        '
        Me.custCtrlBtnNext.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnNext.Location = New System.Drawing.Point(4, 246)
        Me.custCtrlBtnNext.Name = "custCtrlBtnNext"
        Me.custCtrlBtnNext.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnNext.TabIndex = 82
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(184, 47)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 81
        '
        'lblBackshopHeader
        '
        Me.lblBackshopHeader.Location = New System.Drawing.Point(4, 131)
        Me.lblBackshopHeader.Name = "lblBackshopHeader"
        Me.lblBackshopHeader.Size = New System.Drawing.Size(149, 17)
        Me.lblBackshopHeader.Text = "Total Item Count :"
        '
        'lblTotalItemQty
        '
        Me.lblTotalItemQty.Location = New System.Drawing.Point(168, 131)
        Me.lblTotalItemQty.Name = "lblTotalItemQty"
        Me.lblTotalItemQty.Size = New System.Drawing.Size(38, 17)
        Me.lblTotalItemQty.Text = "1111"
        '
        'Btn_OSSRItem1
        '
        Me.Btn_OSSRItem1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_OSSRItem1.Location = New System.Drawing.Point(109, 246)
        Me.Btn_OSSRItem1.Name = "Btn_OSSRItem1"
        Me.Btn_OSSRItem1.Size = New System.Drawing.Size(75, 24)
        Me.Btn_OSSRItem1.TabIndex = 146
        '
        'lblOSSR
        '
        Me.lblOSSR.Location = New System.Drawing.Point(178, 30)
        Me.lblOSSR.Name = "lblOSSR"
        Me.lblOSSR.Size = New System.Drawing.Size(35, 17)
        '
        'btnView
        '
        Me.btnView.Image = CType(resources.GetObject("btnView.Image"), System.Drawing.Image)
        Me.btnView.Location = New System.Drawing.Point(180, 80)
        Me.btnView.Name = "btnView"
        Me.btnView.Size = New System.Drawing.Size(40, 15)
        Me.btnView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 147
        '
        'btnZero
        '
        Me.btnZero.BackColor = System.Drawing.Color.Transparent
        Me.btnZero.Location = New System.Drawing.Point(64, 246)
        Me.btnZero.Name = "btnZero"
        Me.btnZero.Size = New System.Drawing.Size(50, 24)
        Me.btnZero.TabIndex = 171
        '
        'frmEXPLItemDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnZero)
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.Btn_OSSRItem1)
        Me.Controls.Add(Me.custCtrlBtnQuit)
        Me.Controls.Add(Me.custCtrlBtnBack)
        Me.Controls.Add(Me.lblBackshopHeader)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblOSSR)
        Me.Controls.Add(Me.lblTotalItemQty)
        Me.Controls.Add(Me.btnSalesFloorCalcpad)
        Me.Controls.Add(Me.cmbLocation)
        Me.Controls.Add(Me.lblLocationHeader)
        Me.Controls.Add(Me.lblCounted)
        Me.Controls.Add(Me.lblCurrentQty)
        Me.Controls.Add(Me.lblSalesFloorHeader)
        Me.Controls.Add(Me.lblStockFigure)
        Me.Controls.Add(Me.lblStockFigureHeader)
        Me.Controls.Add(Me.lblProductDesc3)
        Me.Controls.Add(Me.lblProductDesc2)
        Me.Controls.Add(Me.lblProductDesc1)
        Me.Controls.Add(Me.lblStatusDisplay)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.lblItemPosition)
        Me.Controls.Add(Me.lblProductCodeDisplay)
        Me.Controls.Add(Me.lblBootsCodeDisplay)
        Me.Controls.Add(Me.custCtrlBtnNext)
        Me.Name = "frmEXPLItemDetails"
        Me.Text = "Picking List"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblCounted As System.Windows.Forms.Label
    Friend WithEvents lblCurrentQty As System.Windows.Forms.Label
    Friend WithEvents lblSalesFloorHeader As System.Windows.Forms.Label
    Friend WithEvents lblStockFigure As System.Windows.Forms.Label
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
    Friend WithEvents lblStockFigureHeader As System.Windows.Forms.Label
    Friend WithEvents lblLocationHeader As System.Windows.Forms.Label
    Friend WithEvents cmbLocation As System.Windows.Forms.ComboBox
    Friend WithEvents btnSalesFloorCalcpad As CustomButtons.btn_CalcPad_small
    Friend WithEvents lblBackshopHeader As System.Windows.Forms.Label
    Friend WithEvents lblTotalItemQty As System.Windows.Forms.Label
    Friend WithEvents Btn_OSSRItem1 As CustomButtons.btn_OSSRItem
    Friend WithEvents lblOSSR As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents btnView As System.Windows.Forms.PictureBox
    Friend WithEvents btnZero As CustomButtons.btn_Zero
End Class
