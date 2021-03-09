<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmAFFPLItemDetails
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAFFPLItemDetails))
        Me.lblBootsCodeDisplay = New System.Windows.Forms.Label
        Me.lblProductCodeDisplay = New System.Windows.Forms.Label
        Me.lblItemPosition = New System.Windows.Forms.Label
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.lblStatus = New System.Windows.Forms.Label
        Me.custCtrlBtnNext = New CustomButtons.btn_Next_small
        Me.custCtrlBtnBack = New CustomButtons.btn_Back_sm
        Me.custCtrlBtnQuit = New CustomButtons.btn_Quit_small
        Me.lblProductDesc1 = New System.Windows.Forms.Label
        Me.lblProductDesc2 = New System.Windows.Forms.Label
        Me.lblProductDesc3 = New System.Windows.Forms.Label
        Me.lblStockFigureHeader = New System.Windows.Forms.Label
        Me.lblStockFigure = New System.Windows.Forms.Label
        Me.lblQtyReqHeader = New System.Windows.Forms.Label
        Me.lblQtyRequired = New System.Windows.Forms.Label
        Me.lblPicked = New System.Windows.Forms.Label
        Me.lblStatusDisplay = New System.Windows.Forms.Label
        Me.lblOSSR = New System.Windows.Forms.Label
        Me.btn_OSSRItem = New CustomButtons.btn_OSSRItem
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.btnView = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'lblBootsCodeDisplay
        '
        Me.lblBootsCodeDisplay.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblBootsCodeDisplay.Location = New System.Drawing.Point(16, 9)
        Me.lblBootsCodeDisplay.Name = "lblBootsCodeDisplay"
        Me.lblBootsCodeDisplay.Size = New System.Drawing.Size(100, 18)
        Me.lblBootsCodeDisplay.Text = "11-999-999"
        '
        'lblProductCodeDisplay
        '
        Me.lblProductCodeDisplay.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblProductCodeDisplay.Location = New System.Drawing.Point(16, 29)
        Me.lblProductCodeDisplay.Name = "lblProductCodeDisplay"
        Me.lblProductCodeDisplay.Size = New System.Drawing.Size(130, 20)
        Me.lblProductCodeDisplay.Text = "9-999999-999999"
        '
        'lblItemPosition
        '
        Me.lblItemPosition.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblItemPosition.Location = New System.Drawing.Point(179, 11)
        Me.lblItemPosition.Name = "lblItemPosition"
        Me.lblItemPosition.Size = New System.Drawing.Size(45, 20)
        Me.lblItemPosition.Text = "1/1001"
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.Info_button_i1.Location = New System.Drawing.Point(182, 65)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 81
        '
        'lblStatus
        '
        Me.lblStatus.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblStatus.Location = New System.Drawing.Point(16, 125)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(44, 22)
        Me.lblStatus.Text = "Status:"
        '
        'custCtrlBtnNext
        '
        Me.custCtrlBtnNext.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnNext.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.custCtrlBtnNext.Location = New System.Drawing.Point(1, 242)
        Me.custCtrlBtnNext.Name = "custCtrlBtnNext"
        Me.custCtrlBtnNext.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnNext.TabIndex = 82
        '
        'custCtrlBtnBack
        '
        Me.custCtrlBtnBack.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnBack.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.custCtrlBtnBack.Location = New System.Drawing.Point(54, 242)
        Me.custCtrlBtnBack.Name = "custCtrlBtnBack"
        Me.custCtrlBtnBack.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnBack.TabIndex = 83
        '
        'custCtrlBtnQuit
        '
        Me.custCtrlBtnQuit.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnQuit.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.custCtrlBtnQuit.Location = New System.Drawing.Point(187, 242)
        Me.custCtrlBtnQuit.Name = "custCtrlBtnQuit"
        Me.custCtrlBtnQuit.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnQuit.TabIndex = 84
        '
        'lblProductDesc1
        '
        Me.lblProductDesc1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblProductDesc1.Location = New System.Drawing.Point(16, 61)
        Me.lblProductDesc1.Name = "lblProductDesc1"
        Me.lblProductDesc1.Size = New System.Drawing.Size(160, 14)
        Me.lblProductDesc1.Text = "Product Description"
        '
        'lblProductDesc2
        '
        Me.lblProductDesc2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblProductDesc2.Location = New System.Drawing.Point(16, 78)
        Me.lblProductDesc2.Name = "lblProductDesc2"
        Me.lblProductDesc2.Size = New System.Drawing.Size(160, 15)
        Me.lblProductDesc2.Text = "Product Description"
        '
        'lblProductDesc3
        '
        Me.lblProductDesc3.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblProductDesc3.Location = New System.Drawing.Point(16, 93)
        Me.lblProductDesc3.Name = "lblProductDesc3"
        Me.lblProductDesc3.Size = New System.Drawing.Size(160, 14)
        Me.lblProductDesc3.Text = "Product Description"
        '
        'lblStockFigureHeader
        '
        Me.lblStockFigureHeader.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblStockFigureHeader.Location = New System.Drawing.Point(16, 161)
        Me.lblStockFigureHeader.Name = "lblStockFigureHeader"
        Me.lblStockFigureHeader.Size = New System.Drawing.Size(148, 17)
        Me.lblStockFigureHeader.Text = "Start of Day Stock File:"
        '
        'lblStockFigure
        '
        Me.lblStockFigure.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblStockFigure.Location = New System.Drawing.Point(170, 161)
        Me.lblStockFigure.Name = "lblStockFigure"
        Me.lblStockFigure.Size = New System.Drawing.Size(38, 17)
        Me.lblStockFigure.Text = "1111"
        '
        'lblQtyReqHeader
        '
        Me.lblQtyReqHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblQtyReqHeader.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblQtyReqHeader.Location = New System.Drawing.Point(16, 193)
        Me.lblQtyReqHeader.Name = "lblQtyReqHeader"
        Me.lblQtyReqHeader.Size = New System.Drawing.Size(150, 17)
        Me.lblQtyReqHeader.Text = "Quantity Required:"
        '
        'lblQtyRequired
        '
        Me.lblQtyRequired.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblQtyRequired.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblQtyRequired.Location = New System.Drawing.Point(170, 192)
        Me.lblQtyRequired.Name = "lblQtyRequired"
        Me.lblQtyRequired.Size = New System.Drawing.Size(38, 17)
        Me.lblQtyRequired.Text = "1111"
        '
        'lblPicked
        '
        Me.lblPicked.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblPicked.Location = New System.Drawing.Point(176, 31)
        Me.lblPicked.Name = "lblPicked"
        Me.lblPicked.Size = New System.Drawing.Size(59, 15)
        Me.lblPicked.Text = "PICKED"
        '
        'lblStatusDisplay
        '
        Me.lblStatusDisplay.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblStatusDisplay.Location = New System.Drawing.Point(76, 125)
        Me.lblStatusDisplay.Name = "lblStatusDisplay"
        Me.lblStatusDisplay.Size = New System.Drawing.Size(160, 22)
        '
        'lblOSSR
        '
        Me.lblOSSR.Location = New System.Drawing.Point(176, 47)
        Me.lblOSSR.Name = "lblOSSR"
        Me.lblOSSR.Size = New System.Drawing.Size(48, 14)
        Me.lblOSSR.Text = "OSSR"
        '
        'btn_OSSRItem
        '
        Me.btn_OSSRItem.BackColor = System.Drawing.Color.Transparent
        Me.btn_OSSRItem.Location = New System.Drawing.Point(108, 242)
        Me.btn_OSSRItem.Name = "btn_OSSRItem"
        Me.btn_OSSRItem.Size = New System.Drawing.Size(75, 24)
        Me.btn_OSSRItem.TabIndex = 121
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.Color.FromArgb(CType(CType(201, Byte), Integer), CType(CType(220, Byte), Integer), CType(CType(233, Byte), Integer))
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.objStatusBar.Location = New System.Drawing.Point(0, 277)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 17)
        Me.objStatusBar.TabIndex = 91
        '
        'btnView
        '
        Me.btnView.Image = CType(resources.GetObject("btnView.Image"), System.Drawing.Image)
        Me.btnView.Location = New System.Drawing.Point(183, 101)
        Me.btnView.Name = "btnView"
        Me.btnView.Size = New System.Drawing.Size(40, 15)
        Me.btnView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmAFFPLItemDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.btn_OSSRItem)
        Me.Controls.Add(Me.lblOSSR)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblStatusDisplay)
        Me.Controls.Add(Me.lblBootsCodeDisplay)
        Me.Controls.Add(Me.lblPicked)
        Me.Controls.Add(Me.lblProductCodeDisplay)
        Me.Controls.Add(Me.lblQtyRequired)
        Me.Controls.Add(Me.lblItemPosition)
        Me.Controls.Add(Me.lblQtyReqHeader)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.lblStockFigure)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.lblStockFigureHeader)
        Me.Controls.Add(Me.custCtrlBtnNext)
        Me.Controls.Add(Me.lblProductDesc3)
        Me.Controls.Add(Me.custCtrlBtnBack)
        Me.Controls.Add(Me.lblProductDesc2)
        Me.Controls.Add(Me.custCtrlBtnQuit)
        Me.Controls.Add(Me.lblProductDesc1)
        Me.Name = "frmAFFPLItemDetails"
        Me.Text = "Picking List"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblBootsCodeDisplay As System.Windows.Forms.Label
    Friend WithEvents lblProductCodeDisplay As System.Windows.Forms.Label
    Friend WithEvents lblItemPosition As System.Windows.Forms.Label
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents custCtrlBtnNext As CustomButtons.btn_Next_small
    Friend WithEvents custCtrlBtnBack As CustomButtons.btn_Back_sm
    Friend WithEvents custCtrlBtnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents lblProductDesc1 As System.Windows.Forms.Label
    Friend WithEvents lblProductDesc2 As System.Windows.Forms.Label
    Friend WithEvents lblProductDesc3 As System.Windows.Forms.Label
    Friend WithEvents lblStockFigureHeader As System.Windows.Forms.Label
    Friend WithEvents lblStockFigure As System.Windows.Forms.Label
    Friend WithEvents lblQtyReqHeader As System.Windows.Forms.Label
    Friend WithEvents lblQtyRequired As System.Windows.Forms.Label
    Friend WithEvents lblPicked As System.Windows.Forms.Label
    Friend WithEvents lblStatusDisplay As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblOSSR As System.Windows.Forms.Label
    Friend WithEvents btn_OSSRItem As CustomButtons.btn_OSSRItem
    Friend WithEvents btnView As System.Windows.Forms.PictureBox
End Class
