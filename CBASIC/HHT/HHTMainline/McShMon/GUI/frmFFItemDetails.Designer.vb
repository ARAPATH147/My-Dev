<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmFFItemDetails
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
        Me.btnView = New CustomButtons.btnView
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.Btn_Next_small1 = New CustomButtons.btn_Next_small
        Me.Btn_CalcPad_small1 = New CustomButtons.btn_CalcPad_small
        Me.lblFillQtyText = New System.Windows.Forms.Label
        Me.lblFillQty = New System.Windows.Forms.Label
        Me.lblStatusText = New System.Windows.Forms.Label
        Me.lblStatus = New System.Windows.Forms.Label
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.lblProdDescription3 = New System.Windows.Forms.Label
        Me.lblProdDescription2 = New System.Windows.Forms.Label
        Me.lblProdDescription1 = New System.Windows.Forms.Label
        Me.lblProductCode = New System.Windows.Forms.Label
        Me.lblBootsCode = New System.Windows.Forms.Label
        Me.lblOSSR = New System.Windows.Forms.Label
        Me.btn_OSSRItem = New CustomButtons.btn_OSSRItem
        Me.lblStockText = New System.Windows.Forms.Label
        Me.lblSODStockFile = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'btnView
        '
        Me.btnView.BackColor = System.Drawing.Color.Transparent
        Me.btnView.Location = New System.Drawing.Point(99, 241)
        Me.btnView.Name = "btnView"
        Me.btnView.Size = New System.Drawing.Size(58, 24)
        Me.btnView.TabIndex = 64
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(187, 241)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 63
        '
        'Btn_Next_small1
        '
        Me.Btn_Next_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Next_small1.Location = New System.Drawing.Point(11, 241)
        Me.Btn_Next_small1.Name = "Btn_Next_small1"
        Me.Btn_Next_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Next_small1.TabIndex = 62
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(201, 196)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small1.TabIndex = 61
        '
        'lblFillQtyText
        '
        Me.lblFillQtyText.Location = New System.Drawing.Point(167, 204)
        Me.lblFillQtyText.Name = "lblFillQtyText"
        Me.lblFillQtyText.Size = New System.Drawing.Size(28, 15)
        Me.lblFillQtyText.Text = "0"
        '
        'lblFillQty
        '
        Me.lblFillQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblFillQty.Location = New System.Drawing.Point(12, 204)
        Me.lblFillQty.Name = "lblFillQty"
        Me.lblFillQty.Size = New System.Drawing.Size(140, 15)
        Me.lblFillQty.Text = "Enter Fill Quantity:"
        '
        'lblStatusText
        '
        Me.lblStatusText.Location = New System.Drawing.Point(68, 142)
        Me.lblStatusText.Name = "lblStatusText"
        Me.lblStatusText.Size = New System.Drawing.Size(157, 18)
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(12, 142)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(50, 18)
        Me.lblStatus.Text = "Status:"
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(193, 29)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 60
        '
        'lblProdDescription3
        '
        Me.lblProdDescription3.Location = New System.Drawing.Point(12, 115)
        Me.lblProdDescription3.Name = "lblProdDescription3"
        Me.lblProdDescription3.Size = New System.Drawing.Size(140, 22)
        Me.lblProdDescription3.Text = "description3"
        '
        'lblProdDescription2
        '
        Me.lblProdDescription2.Location = New System.Drawing.Point(12, 99)
        Me.lblProdDescription2.Name = "lblProdDescription2"
        Me.lblProdDescription2.Size = New System.Drawing.Size(140, 21)
        Me.lblProdDescription2.Text = "description2"
        '
        'lblProdDescription1
        '
        Me.lblProdDescription1.Location = New System.Drawing.Point(12, 83)
        Me.lblProdDescription1.Name = "lblProdDescription1"
        Me.lblProdDescription1.Size = New System.Drawing.Size(140, 22)
        Me.lblProdDescription1.Text = "description1"
        '
        'lblProductCode
        '
        Me.lblProductCode.Location = New System.Drawing.Point(12, 49)
        Me.lblProductCode.Name = "lblProductCode"
        Me.lblProductCode.Size = New System.Drawing.Size(110, 20)
        Me.lblProductCode.Text = "1234567891234"
        '
        'lblBootsCode
        '
        Me.lblBootsCode.Location = New System.Drawing.Point(12, 29)
        Me.lblBootsCode.Name = "lblBootsCode"
        Me.lblBootsCode.Size = New System.Drawing.Size(100, 20)
        Me.lblBootsCode.Text = "Bootscode"
        '
        'lblOSSR
        '
        Me.lblOSSR.Location = New System.Drawing.Point(189, 64)
        Me.lblOSSR.Name = "lblOSSR"
        Me.lblOSSR.Size = New System.Drawing.Size(48, 20)
        '
        'btn_OSSRItem
        '
        Me.btn_OSSRItem.BackColor = System.Drawing.Color.Transparent
        Me.btn_OSSRItem.Location = New System.Drawing.Point(162, 87)
        Me.btn_OSSRItem.Name = "btn_OSSRItem"
        Me.btn_OSSRItem.Size = New System.Drawing.Size(75, 24)
        Me.btn_OSSRItem.TabIndex = 95
        '
        'lblStockText
        '
        Me.lblStockText.Location = New System.Drawing.Point(166, 160)
        Me.lblStockText.Name = "lblStockText"
        Me.lblStockText.Size = New System.Drawing.Size(57, 17)
        '
        'lblSODStockFile
        '
        Me.lblSODStockFile.Location = New System.Drawing.Point(12, 160)
        Me.lblSODStockFile.Name = "lblSODStockFile"
        Me.lblSODStockFile.Size = New System.Drawing.Size(140, 17)
        Me.lblSODStockFile.Text = "Start Of Day Stock File:"
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 75
        '
        'frmFFItemDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblStockText)
        Me.Controls.Add(Me.lblSODStockFile)
        Me.Controls.Add(Me.btn_OSSRItem)
        Me.Controls.Add(Me.lblOSSR)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.Btn_Next_small1)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.lblFillQtyText)
        Me.Controls.Add(Me.lblFillQty)
        Me.Controls.Add(Me.lblStatusText)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.lblProdDescription3)
        Me.Controls.Add(Me.lblProdDescription2)
        Me.Controls.Add(Me.lblProdDescription1)
        Me.Controls.Add(Me.lblProductCode)
        Me.Controls.Add(Me.lblBootsCode)
        Me.Name = "frmFFItemDetails"
        Me.Text = "Fast Fill"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnView As CustomButtons.btnView
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents Btn_Next_small1 As CustomButtons.btn_Next_small
    Friend WithEvents Btn_CalcPad_small1 As CustomButtons.btn_CalcPad_small
    Friend WithEvents lblFillQtyText As System.Windows.Forms.Label
    Friend WithEvents lblFillQty As System.Windows.Forms.Label
    Friend WithEvents lblStatusText As System.Windows.Forms.Label
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents lblProdDescription3 As System.Windows.Forms.Label
    Friend WithEvents lblProdDescription2 As System.Windows.Forms.Label
    Friend WithEvents lblProdDescription1 As System.Windows.Forms.Label
    Friend WithEvents lblProductCode As System.Windows.Forms.Label
    Friend WithEvents lblBootsCode As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblOSSR As System.Windows.Forms.Label
    Friend WithEvents btn_OSSRItem As CustomButtons.btn_OSSRItem
    Friend WithEvents lblStockText As System.Windows.Forms.Label
    Friend WithEvents lblSODStockFile As System.Windows.Forms.Label
End Class
