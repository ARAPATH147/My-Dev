<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmPLItemConfirm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPLItemConfirm))
        Me.lblProductDesc3 = New System.Windows.Forms.Label
        Me.lblProductDesc2 = New System.Windows.Forms.Label
        Me.lblProductDesc1 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblStatusDisplay = New System.Windows.Forms.Label
        Me.lblStatus = New System.Windows.Forms.Label
        Me.lblItemPosition = New System.Windows.Forms.Label
        Me.lblProductCodeDisplay = New System.Windows.Forms.Label
        Me.lblBootsCodeDisplay = New System.Windows.Forms.Label
        Me.lblTotalItemCount = New System.Windows.Forms.Label
        Me.lblTotalItemCountHeader = New System.Windows.Forms.Label
        Me.lblStockFigure = New System.Windows.Forms.Label
        Me.lblStockFigureHeader = New System.Windows.Forms.Label
        Me.lblPicked = New System.Windows.Forms.Label
        Me.custCtrlBtnQuit = New CustomButtons.btn_Quit_small
        Me.custCtrlBtnBack = New CustomButtons.btn_Back_sm
        Me.custCtrlBtnNext = New CustomButtons.btn_Next_small
        Me.Info_button = New CustomButtons.info_button_i
        Me.Btn_CalcPad_small1 = New CustomButtons.btn_CalcPad_small
        Me.btn_View = New System.Windows.Forms.PictureBox
        Me.Btn_GAP1 = New CustomButtons.btn_Zero
        Me.cmbLocation = New System.Windows.Forms.ComboBox
        Me.lblSite = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
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
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(4, 207)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(206, 16)
        Me.Label1.Text = "Scan/Key to confirm item"
        '
        'lblStatusDisplay
        '
        Me.lblStatusDisplay.Location = New System.Drawing.Point(64, 108)
        Me.lblStatusDisplay.Name = "lblStatusDisplay"
        Me.lblStatusDisplay.Size = New System.Drawing.Size(109, 17)
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(4, 108)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(58, 17)
        Me.lblStatus.Text = "Status :"
        '
        'lblItemPosition
        '
        Me.lblItemPosition.Location = New System.Drawing.Point(187, 0)
        Me.lblItemPosition.Name = "lblItemPosition"
        Me.lblItemPosition.Size = New System.Drawing.Size(49, 15)
        Me.lblItemPosition.Text = "9/100"
        '
        'lblProductCodeDisplay
        '
        Me.lblProductCodeDisplay.Location = New System.Drawing.Point(4, 21)
        Me.lblProductCodeDisplay.Name = "lblProductCodeDisplay"
        Me.lblProductCodeDisplay.Size = New System.Drawing.Size(116, 20)
        Me.lblProductCodeDisplay.Text = "9-999999-999999"
        '
        'lblBootsCodeDisplay
        '
        Me.lblBootsCodeDisplay.Location = New System.Drawing.Point(4, 3)
        Me.lblBootsCodeDisplay.Name = "lblBootsCodeDisplay"
        Me.lblBootsCodeDisplay.Size = New System.Drawing.Size(100, 18)
        Me.lblBootsCodeDisplay.Text = "11-999-999"
        '
        'lblTotalItemCount
        '
        Me.lblTotalItemCount.Location = New System.Drawing.Point(168, 140)
        Me.lblTotalItemCount.Name = "lblTotalItemCount"
        Me.lblTotalItemCount.Size = New System.Drawing.Size(38, 17)
        Me.lblTotalItemCount.Text = "1111"
        '
        'lblTotalItemCountHeader
        '
        Me.lblTotalItemCountHeader.Location = New System.Drawing.Point(4, 140)
        Me.lblTotalItemCountHeader.Name = "lblTotalItemCountHeader"
        Me.lblTotalItemCountHeader.Size = New System.Drawing.Size(130, 17)
        Me.lblTotalItemCountHeader.Text = "Total Item Count :"
        '
        'lblStockFigure
        '
        Me.lblStockFigure.Location = New System.Drawing.Point(168, 124)
        Me.lblStockFigure.Name = "lblStockFigure"
        Me.lblStockFigure.Size = New System.Drawing.Size(47, 17)
        Me.lblStockFigure.Text = "1111"
        '
        'lblStockFigureHeader
        '
        Me.lblStockFigureHeader.Location = New System.Drawing.Point(4, 124)
        Me.lblStockFigureHeader.Name = "lblStockFigureHeader"
        Me.lblStockFigureHeader.Size = New System.Drawing.Size(152, 17)
        Me.lblStockFigureHeader.Text = "Total Stock File at 99:99 :"
        '
        'lblPicked
        '
        Me.lblPicked.Location = New System.Drawing.Point(172, 15)
        Me.lblPicked.Name = "lblPicked"
        Me.lblPicked.Size = New System.Drawing.Size(64, 15)
        Me.lblPicked.Text = "PICKED"
        '
        'custCtrlBtnQuit
        '
        Me.custCtrlBtnQuit.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnQuit.Location = New System.Drawing.Point(183, 246)
        Me.custCtrlBtnQuit.Name = "custCtrlBtnQuit"
        Me.custCtrlBtnQuit.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnQuit.TabIndex = 30
        '
        'custCtrlBtnBack
        '
        Me.custCtrlBtnBack.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnBack.Location = New System.Drawing.Point(123, 246)
        Me.custCtrlBtnBack.Name = "custCtrlBtnBack"
        Me.custCtrlBtnBack.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnBack.TabIndex = 29
        '
        'custCtrlBtnNext
        '
        Me.custCtrlBtnNext.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnNext.Location = New System.Drawing.Point(4, 246)
        Me.custCtrlBtnNext.Name = "custCtrlBtnNext"
        Me.custCtrlBtnNext.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnNext.TabIndex = 28
        '
        'Info_button
        '
        Me.Info_button.BackColor = System.Drawing.Color.Transparent
        Me.Info_button.Location = New System.Drawing.Point(184, 47)
        Me.Info_button.Name = "Info_button"
        Me.Info_button.Size = New System.Drawing.Size(32, 32)
        Me.Info_button.TabIndex = 27
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(142, 6)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small1.TabIndex = 25
        '
        'btn_View
        '
        Me.btn_View.Image = CType(resources.GetObject("btn_View.Image"), System.Drawing.Image)
        Me.btn_View.Location = New System.Drawing.Point(180, 80)
        Me.btn_View.Name = "btn_View"
        Me.btn_View.Size = New System.Drawing.Size(40, 15)
        Me.btn_View.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_GAP1
        '
        Me.Btn_GAP1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_GAP1.Location = New System.Drawing.Point(64, 246)
        Me.Btn_GAP1.Name = "Btn_GAP1"
        Me.Btn_GAP1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_GAP1.TabIndex = 160
        '
        'cmbLocation
        '
        Me.cmbLocation.Location = New System.Drawing.Point(4, 176)
        Me.cmbLocation.Name = "cmbLocation"
        Me.cmbLocation.Size = New System.Drawing.Size(229, 22)
        Me.cmbLocation.TabIndex = 162
        '
        'lblSite
        '
        Me.lblSite.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSite.Location = New System.Drawing.Point(3, 156)
        Me.lblSite.Name = "lblSite"
        Me.lblSite.Size = New System.Drawing.Size(100, 16)
        Me.lblSite.Text = "Select Site"
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 59
        '
        'frmPLItemConfirm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.cmbLocation)
        Me.Controls.Add(Me.lblSite)
        Me.Controls.Add(Me.Btn_GAP1)
        Me.Controls.Add(Me.btn_View)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblPicked)
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
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblStatusDisplay)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.Info_button)
        Me.Controls.Add(Me.lblItemPosition)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.lblProductCodeDisplay)
        Me.Controls.Add(Me.lblBootsCodeDisplay)
        Me.Name = "frmPLItemConfirm"
        Me.Text = "Picking List"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblProductDesc3 As System.Windows.Forms.Label
    Friend WithEvents lblProductDesc2 As System.Windows.Forms.Label
    Friend WithEvents lblProductDesc1 As System.Windows.Forms.Label
    Friend WithEvents custCtrlBtnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents custCtrlBtnBack As CustomButtons.btn_Back_sm
    Friend WithEvents custCtrlBtnNext As CustomButtons.btn_Next_small
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblStatusDisplay As System.Windows.Forms.Label
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents Info_button As CustomButtons.info_button_i
    Friend WithEvents lblItemPosition As System.Windows.Forms.Label
    Friend WithEvents Btn_CalcPad_small1 As CustomButtons.btn_CalcPad_small
    Friend WithEvents lblProductCodeDisplay As System.Windows.Forms.Label
    Friend WithEvents lblBootsCodeDisplay As System.Windows.Forms.Label
    Friend WithEvents lblTotalItemCount As System.Windows.Forms.Label
    Friend WithEvents lblTotalItemCountHeader As System.Windows.Forms.Label
    Friend WithEvents lblStockFigure As System.Windows.Forms.Label
    Friend WithEvents lblStockFigureHeader As System.Windows.Forms.Label
    Friend WithEvents lblPicked As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents btn_View As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_GAP1 As CustomButtons.btn_Zero
    Friend WithEvents cmbLocation As System.Windows.Forms.ComboBox
    Friend WithEvents lblSite As System.Windows.Forms.Label
End Class
