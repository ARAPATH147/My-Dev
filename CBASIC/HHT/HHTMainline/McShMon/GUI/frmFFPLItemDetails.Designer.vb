<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmFFPLItemDetails
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFFPLItemDetails))
        Me.lblPicked = New System.Windows.Forms.Label
        Me.lblQtyRequired = New System.Windows.Forms.Label
        Me.lblQtyReqHeader = New System.Windows.Forms.Label
        Me.lblStockFigure = New System.Windows.Forms.Label
        Me.lblStockFigureHeader = New System.Windows.Forms.Label
        Me.lblProductDesc3 = New System.Windows.Forms.Label
        Me.lblProductDesc2 = New System.Windows.Forms.Label
        Me.lblProductDesc1 = New System.Windows.Forms.Label
        Me.lblStatusDisplay = New System.Windows.Forms.Label
        Me.lblStatus = New System.Windows.Forms.Label
        Me.lblItemPosition = New System.Windows.Forms.Label
        Me.lblProductCodeDisplay = New System.Windows.Forms.Label
        Me.lblBootsCodeDisplay = New System.Windows.Forms.Label
        Me.custCtrlBtnQuit = New CustomButtons.btn_Quit_small
        Me.custCtrlBtnBack = New CustomButtons.btn_Back_sm
        Me.custCtrlBtnNext = New CustomButtons.btn_Next_small
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.lblOSSR = New System.Windows.Forms.Label
        Me.btn_OSSRItem = New CustomButtons.btn_OSSRItem
        Me.lblSalesFloor = New System.Windows.Forms.Label
        Me.lblSalesQty = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.btnView = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'lblPicked
        '
        Me.lblPicked.Location = New System.Drawing.Point(177, 18)
        Me.lblPicked.Name = "lblPicked"
        Me.lblPicked.Size = New System.Drawing.Size(59, 18)
        Me.lblPicked.Text = "PICKED"
        '
        'lblQtyRequired
        '
        Me.lblQtyRequired.Location = New System.Drawing.Point(164, 180)
        Me.lblQtyRequired.Name = "lblQtyRequired"
        Me.lblQtyRequired.Size = New System.Drawing.Size(38, 17)
        Me.lblQtyRequired.Text = "25"
        '
        'lblQtyReqHeader
        '
        Me.lblQtyReqHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblQtyReqHeader.Location = New System.Drawing.Point(4, 180)
        Me.lblQtyReqHeader.Name = "lblQtyReqHeader"
        Me.lblQtyReqHeader.Size = New System.Drawing.Size(150, 17)
        Me.lblQtyReqHeader.Text = "Quantity Required:"
        '
        'lblStockFigure
        '
        Me.lblStockFigure.Location = New System.Drawing.Point(168, 146)
        Me.lblStockFigure.Name = "lblStockFigure"
        Me.lblStockFigure.Size = New System.Drawing.Size(38, 17)
        Me.lblStockFigure.Text = "100"
        '
        'lblStockFigureHeader
        '
        Me.lblStockFigureHeader.Location = New System.Drawing.Point(4, 146)
        Me.lblStockFigureHeader.Name = "lblStockFigureHeader"
        Me.lblStockFigureHeader.Size = New System.Drawing.Size(148, 17)
        Me.lblStockFigureHeader.Text = "Start of Day Stock File:"
        '
        'lblProductDesc3
        '
        Me.lblProductDesc3.Location = New System.Drawing.Point(4, 84)
        Me.lblProductDesc3.Name = "lblProductDesc3"
        Me.lblProductDesc3.Size = New System.Drawing.Size(160, 14)
        Me.lblProductDesc3.Text = "Express 335ml" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'lblProductDesc2
        '
        Me.lblProductDesc2.Location = New System.Drawing.Point(4, 67)
        Me.lblProductDesc2.Name = "lblProductDesc2"
        Me.lblProductDesc2.Size = New System.Drawing.Size(160, 15)
        Me.lblProductDesc2.Text = "Boots Lemon" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'lblProductDesc1
        '
        Me.lblProductDesc1.Location = New System.Drawing.Point(4, 49)
        Me.lblProductDesc1.Name = "lblProductDesc1"
        Me.lblProductDesc1.Size = New System.Drawing.Size(160, 14)
        Me.lblProductDesc1.Text = "Alcon Opti Free" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'lblStatusDisplay
        '
        Me.lblStatusDisplay.Location = New System.Drawing.Point(64, 110)
        Me.lblStatusDisplay.Name = "lblStatusDisplay"
        Me.lblStatusDisplay.Size = New System.Drawing.Size(160, 22)
        Me.lblStatusDisplay.Text = "Active"
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(4, 110)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(44, 22)
        Me.lblStatus.Text = "Status:"
        '
        'lblItemPosition
        '
        Me.lblItemPosition.Location = New System.Drawing.Point(179, 1)
        Me.lblItemPosition.Name = "lblItemPosition"
        Me.lblItemPosition.Size = New System.Drawing.Size(45, 20)
        Me.lblItemPosition.Text = "1/1001"
        '
        'lblProductCodeDisplay
        '
        Me.lblProductCodeDisplay.Location = New System.Drawing.Point(4, 23)
        Me.lblProductCodeDisplay.Name = "lblProductCodeDisplay"
        Me.lblProductCodeDisplay.Size = New System.Drawing.Size(130, 20)
        Me.lblProductCodeDisplay.Text = "5-901425-038253" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'lblBootsCodeDisplay
        '
        Me.lblBootsCodeDisplay.Location = New System.Drawing.Point(4, 6)
        Me.lblBootsCodeDisplay.Name = "lblBootsCodeDisplay"
        Me.lblBootsCodeDisplay.Size = New System.Drawing.Size(100, 18)
        Me.lblBootsCodeDisplay.Text = "11-34-965"
        '
        'custCtrlBtnQuit
        '
        Me.custCtrlBtnQuit.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnQuit.Location = New System.Drawing.Point(186, 237)
        Me.custCtrlBtnQuit.Name = "custCtrlBtnQuit"
        Me.custCtrlBtnQuit.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnQuit.TabIndex = 84
        '
        'custCtrlBtnBack
        '
        Me.custCtrlBtnBack.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnBack.Location = New System.Drawing.Point(55, 237)
        Me.custCtrlBtnBack.Name = "custCtrlBtnBack"
        Me.custCtrlBtnBack.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnBack.TabIndex = 83
        '
        'custCtrlBtnNext
        '
        Me.custCtrlBtnNext.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnNext.Location = New System.Drawing.Point(2, 237)
        Me.custCtrlBtnNext.Name = "custCtrlBtnNext"
        Me.custCtrlBtnNext.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnNext.TabIndex = 82
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(184, 52)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 81
        '
        'lblOSSR
        '
        Me.lblOSSR.Location = New System.Drawing.Point(182, 36)
        Me.lblOSSR.Name = "lblOSSR"
        Me.lblOSSR.Size = New System.Drawing.Size(49, 15)
        Me.lblOSSR.Text = "OSSR"
        '
        'btn_OSSRItem
        '
        Me.btn_OSSRItem.BackColor = System.Drawing.Color.Transparent
        Me.btn_OSSRItem.Location = New System.Drawing.Point(108, 237)
        Me.btn_OSSRItem.Name = "btn_OSSRItem"
        Me.btn_OSSRItem.Size = New System.Drawing.Size(75, 24)
        Me.btn_OSSRItem.TabIndex = 120
        '
        'lblSalesFloor
        '
        Me.lblSalesFloor.Location = New System.Drawing.Point(4, 128)
        Me.lblSalesFloor.Name = "lblSalesFloor"
        Me.lblSalesFloor.Size = New System.Drawing.Size(148, 20)
        Me.lblSalesFloor.Text = "Sales Floor Quantity:"
        '
        'lblSalesQty
        '
        Me.lblSalesQty.Location = New System.Drawing.Point(168, 128)
        Me.lblSalesQty.Name = "lblSalesQty"
        Me.lblSalesQty.Size = New System.Drawing.Size(61, 20)
        Me.lblSalesQty.Text = "30"
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 90
        '
        'btnView
        '
        Me.btnView.Image = CType(resources.GetObject("btnView.Image"), System.Drawing.Image)
        Me.btnView.Location = New System.Drawing.Point(180, 89)
        Me.btnView.Name = "btnView"
        Me.btnView.Size = New System.Drawing.Size(40, 15)
        Me.btnView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmFFPLItemDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.lblSalesQty)
        Me.Controls.Add(Me.lblSalesFloor)
        Me.Controls.Add(Me.btn_OSSRItem)
        Me.Controls.Add(Me.lblOSSR)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblPicked)
        Me.Controls.Add(Me.lblQtyRequired)
        Me.Controls.Add(Me.lblQtyReqHeader)
        Me.Controls.Add(Me.lblStockFigure)
        Me.Controls.Add(Me.lblStockFigureHeader)
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
        Me.Name = "frmFFPLItemDetails"
        Me.Text = "Picking List"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblPicked As System.Windows.Forms.Label
    Friend WithEvents lblQtyRequired As System.Windows.Forms.Label
    Friend WithEvents lblQtyReqHeader As System.Windows.Forms.Label
    Friend WithEvents lblStockFigure As System.Windows.Forms.Label
    Friend WithEvents lblStockFigureHeader As System.Windows.Forms.Label
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
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblOSSR As System.Windows.Forms.Label
    Friend WithEvents btn_OSSRItem As CustomButtons.btn_OSSRItem
    Friend WithEvents lblSalesFloor As System.Windows.Forms.Label
    Friend WithEvents lblSalesQty As System.Windows.Forms.Label
    Friend WithEvents btnView As System.Windows.Forms.PictureBox
End Class
