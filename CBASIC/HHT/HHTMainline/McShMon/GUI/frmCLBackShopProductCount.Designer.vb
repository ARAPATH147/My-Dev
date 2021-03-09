<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmCLBackShopProductCount
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
        Me.lblProductDesc3 = New System.Windows.Forms.Label
        Me.lblProductDesc2 = New System.Windows.Forms.Label
        Me.lblProductDesc1 = New System.Windows.Forms.Label
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.Btn_CalcPad_small2 = New CustomButtons.btn_CalcPad_small
        Me.custCtrlBtnQuit = New CustomButtons.btn_Quit_small
        Me.custCtrlBtnNext = New CustomButtons.btn_Next_small
        Me.lblStatus = New System.Windows.Forms.Label
        Me.lblProductCodeDisplay = New System.Windows.Forms.Label
        Me.lblBootsCodeDisplay = New System.Windows.Forms.Label
        Me.lblShelfQty = New System.Windows.Forms.Label
        Me.lblEnterQty = New System.Windows.Forms.Label
        Me.cmbMultiSite = New System.Windows.Forms.ComboBox
        Me.lblTotalItemCountDisplay = New System.Windows.Forms.Label
        Me.lblTotalItemCount = New System.Windows.Forms.Label
        Me.lblSite = New System.Windows.Forms.Label
        Me.lblTotalStockFileDisplay = New System.Windows.Forms.Label
        Me.lblTotalStockFile = New System.Windows.Forms.Label
        Me.lblStatusDisplay = New System.Windows.Forms.Label
        Me.lblItemPosition = New System.Windows.Forms.Label
        Me.btnZero = New CustomButtons.btn_Zero
        Me.custCtrlBtnBack = New CustomButtons.btn_Back_sm
        Me.lblCounted = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblProductDesc3
        '
        Me.lblProductDesc3.Location = New System.Drawing.Point(6, 85)
        Me.lblProductDesc3.Name = "lblProductDesc3"
        Me.lblProductDesc3.Size = New System.Drawing.Size(160, 14)
        Me.lblProductDesc3.Text = "Product Description"
        '
        'lblProductDesc2
        '
        Me.lblProductDesc2.Location = New System.Drawing.Point(6, 67)
        Me.lblProductDesc2.Name = "lblProductDesc2"
        Me.lblProductDesc2.Size = New System.Drawing.Size(160, 15)
        Me.lblProductDesc2.Text = "Product Description"
        '
        'lblProductDesc1
        '
        Me.lblProductDesc1.Location = New System.Drawing.Point(6, 48)
        Me.lblProductDesc1.Name = "lblProductDesc1"
        Me.lblProductDesc1.Size = New System.Drawing.Size(160, 14)
        Me.lblProductDesc1.Text = "Product Description"
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(186, 54)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 85
        '
        'Btn_CalcPad_small2
        '
        Me.Btn_CalcPad_small2.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CalcPad_small2.Location = New System.Drawing.Point(207, 214)
        Me.Btn_CalcPad_small2.Name = "Btn_CalcPad_small2"
        Me.Btn_CalcPad_small2.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small2.TabIndex = 84
        '
        'custCtrlBtnQuit
        '
        Me.custCtrlBtnQuit.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnQuit.Location = New System.Drawing.Point(182, 246)
        Me.custCtrlBtnQuit.Name = "custCtrlBtnQuit"
        Me.custCtrlBtnQuit.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnQuit.TabIndex = 83
        '
        'custCtrlBtnNext
        '
        Me.custCtrlBtnNext.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnNext.Location = New System.Drawing.Point(6, 246)
        Me.custCtrlBtnNext.Name = "custCtrlBtnNext"
        Me.custCtrlBtnNext.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnNext.TabIndex = 81
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(6, 109)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(44, 18)
        Me.lblStatus.Text = "Status:"
        '
        'lblProductCodeDisplay
        '
        Me.lblProductCodeDisplay.Location = New System.Drawing.Point(6, 28)
        Me.lblProductCodeDisplay.Name = "lblProductCodeDisplay"
        Me.lblProductCodeDisplay.Size = New System.Drawing.Size(121, 20)
        Me.lblProductCodeDisplay.Text = "9-999999-999999"
        '
        'lblBootsCodeDisplay
        '
        Me.lblBootsCodeDisplay.Location = New System.Drawing.Point(6, 8)
        Me.lblBootsCodeDisplay.Name = "lblBootsCodeDisplay"
        Me.lblBootsCodeDisplay.Size = New System.Drawing.Size(100, 20)
        '
        'lblShelfQty
        '
        Me.lblShelfQty.Location = New System.Drawing.Point(160, 222)
        Me.lblShelfQty.Name = "lblShelfQty"
        Me.lblShelfQty.Size = New System.Drawing.Size(38, 15)
        '
        'lblEnterQty
        '
        Me.lblEnterQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblEnterQty.Location = New System.Drawing.Point(6, 222)
        Me.lblEnterQty.Name = "lblEnterQty"
        Me.lblEnterQty.Size = New System.Drawing.Size(150, 14)
        Me.lblEnterQty.Text = "Enter Quantity on Shelf"
        '
        'cmbMultiSite
        '
        Me.cmbMultiSite.Location = New System.Drawing.Point(6, 186)
        Me.cmbMultiSite.Name = "cmbMultiSite"
        Me.cmbMultiSite.Size = New System.Drawing.Size(224, 22)
        Me.cmbMultiSite.TabIndex = 114
        Me.cmbMultiSite.Visible = False
        '
        'lblTotalItemCountDisplay
        '
        Me.lblTotalItemCountDisplay.Location = New System.Drawing.Point(160, 146)
        Me.lblTotalItemCountDisplay.Name = "lblTotalItemCountDisplay"
        Me.lblTotalItemCountDisplay.Size = New System.Drawing.Size(55, 15)
        '
        'lblTotalItemCount
        '
        Me.lblTotalItemCount.Location = New System.Drawing.Point(6, 146)
        Me.lblTotalItemCount.Name = "lblTotalItemCount"
        Me.lblTotalItemCount.Size = New System.Drawing.Size(107, 15)
        Me.lblTotalItemCount.Text = "Total Item Count"
        '
        'lblSite
        '
        Me.lblSite.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSite.Location = New System.Drawing.Point(6, 167)
        Me.lblSite.Name = "lblSite"
        Me.lblSite.Size = New System.Drawing.Size(76, 15)
        Me.lblSite.Text = "Select Site"
        '
        'lblTotalStockFileDisplay
        '
        Me.lblTotalStockFileDisplay.Location = New System.Drawing.Point(160, 128)
        Me.lblTotalStockFileDisplay.Name = "lblTotalStockFileDisplay"
        Me.lblTotalStockFileDisplay.Size = New System.Drawing.Size(55, 15)
        '
        'lblTotalStockFile
        '
        Me.lblTotalStockFile.Location = New System.Drawing.Point(6, 128)
        Me.lblTotalStockFile.Name = "lblTotalStockFile"
        Me.lblTotalStockFile.Size = New System.Drawing.Size(150, 15)
        '
        'lblStatusDisplay
        '
        Me.lblStatusDisplay.Location = New System.Drawing.Point(69, 109)
        Me.lblStatusDisplay.Name = "lblStatusDisplay"
        Me.lblStatusDisplay.Size = New System.Drawing.Size(159, 15)
        '
        'lblItemPosition
        '
        Me.lblItemPosition.Location = New System.Drawing.Point(184, 8)
        Me.lblItemPosition.Name = "lblItemPosition"
        Me.lblItemPosition.Size = New System.Drawing.Size(38, 20)
        '
        'btnZero
        '
        Me.btnZero.BackColor = System.Drawing.Color.Transparent
        Me.btnZero.Location = New System.Drawing.Point(65, 246)
        Me.btnZero.Name = "btnZero"
        Me.btnZero.Size = New System.Drawing.Size(50, 24)
        Me.btnZero.TabIndex = 123
        '
        'custCtrlBtnBack
        '
        Me.custCtrlBtnBack.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnBack.Location = New System.Drawing.Point(123, 246)
        Me.custCtrlBtnBack.Name = "custCtrlBtnBack"
        Me.custCtrlBtnBack.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnBack.TabIndex = 140
        '
        'lblCounted
        '
        Me.lblCounted.Location = New System.Drawing.Point(173, 36)
        Me.lblCounted.Name = "lblCounted"
        Me.lblCounted.Size = New System.Drawing.Size(61, 18)
        Me.lblCounted.Text = "COUNTED"
        Me.lblCounted.Visible = False
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 113
        '
        'frmCLBackShopProductCount
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblCounted)
        Me.Controls.Add(Me.custCtrlBtnBack)
        Me.Controls.Add(Me.btnZero)
        Me.Controls.Add(Me.lblItemPosition)
        Me.Controls.Add(Me.lblTotalStockFileDisplay)
        Me.Controls.Add(Me.lblTotalStockFile)
        Me.Controls.Add(Me.lblStatusDisplay)
        Me.Controls.Add(Me.lblTotalItemCountDisplay)
        Me.Controls.Add(Me.lblTotalItemCount)
        Me.Controls.Add(Me.lblSite)
        Me.Controls.Add(Me.cmbMultiSite)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblShelfQty)
        Me.Controls.Add(Me.lblEnterQty)
        Me.Controls.Add(Me.lblProductDesc3)
        Me.Controls.Add(Me.lblProductDesc2)
        Me.Controls.Add(Me.lblProductDesc1)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.Btn_CalcPad_small2)
        Me.Controls.Add(Me.custCtrlBtnQuit)
        Me.Controls.Add(Me.custCtrlBtnNext)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.lblProductCodeDisplay)
        Me.Controls.Add(Me.lblBootsCodeDisplay)
        Me.Name = "frmCLBackShopProductCount"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblProductDesc3 As System.Windows.Forms.Label
    Friend WithEvents lblProductDesc2 As System.Windows.Forms.Label
    Friend WithEvents lblProductDesc1 As System.Windows.Forms.Label
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents Btn_CalcPad_small2 As CustomButtons.btn_CalcPad_small
    Friend WithEvents custCtrlBtnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents custCtrlBtnNext As CustomButtons.btn_Next_small
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lblProductCodeDisplay As System.Windows.Forms.Label
    Friend WithEvents lblBootsCodeDisplay As System.Windows.Forms.Label
    Friend WithEvents lblShelfQty As System.Windows.Forms.Label
    Friend WithEvents lblEnterQty As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents cmbMultiSite As System.Windows.Forms.ComboBox
    Friend WithEvents lblTotalItemCountDisplay As System.Windows.Forms.Label
    Friend WithEvents lblTotalItemCount As System.Windows.Forms.Label
    Friend WithEvents lblSite As System.Windows.Forms.Label
    Friend WithEvents lblTotalStockFileDisplay As System.Windows.Forms.Label
    Friend WithEvents lblTotalStockFile As System.Windows.Forms.Label
    Friend WithEvents lblStatusDisplay As System.Windows.Forms.Label
    Friend WithEvents lblItemPosition As System.Windows.Forms.Label
    Friend WithEvents btnZero As CustomButtons.btn_Zero
    Friend WithEvents custCtrlBtnBack As CustomButtons.btn_Back_sm
    Friend WithEvents lblCounted As System.Windows.Forms.Label
End Class
