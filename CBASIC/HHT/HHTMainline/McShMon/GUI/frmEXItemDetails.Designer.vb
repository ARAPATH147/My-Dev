<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmEXItemDetails
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
        Me.lblProdDescription3 = New System.Windows.Forms.Label
        Me.lblProdDescription2 = New System.Windows.Forms.Label
        Me.lblProdDescription1 = New System.Windows.Forms.Label
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.lblStatus = New System.Windows.Forms.Label
        Me.lblStatusText = New System.Windows.Forms.Label
        Me.lblBckShpQty = New System.Windows.Forms.Label
        Me.lblBackQtyText = New System.Windows.Forms.Label
        Me.Btn_CalcPad_small1 = New CustomButtons.btn_CalcPad_small
        Me.Btn_Next_small1 = New CustomButtons.btn_Next_small
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.btnView = New CustomButtons.btnView
        Me.lblOSSR = New System.Windows.Forms.Label
        Me.Btn_OSSRItem1 = New CustomButtons.btn_OSSRItem
        Me.lblSODStockFile = New System.Windows.Forms.Label
        Me.lblTotalItemCount = New System.Windows.Forms.Label
        Me.lblStockText = New System.Windows.Forms.Label
        Me.lblTotalItemText = New System.Windows.Forms.Label
        Me.cmbSite = New System.Windows.Forms.ComboBox
        Me.lblSite = New System.Windows.Forms.Label
        Me.Btn_CalcPad_small2 = New CustomButtons.btn_CalcPad_small
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblBootsCode
        '
        Me.lblBootsCode.Location = New System.Drawing.Point(7, 12)
        Me.lblBootsCode.Name = "lblBootsCode"
        Me.lblBootsCode.Size = New System.Drawing.Size(100, 20)
        Me.lblBootsCode.Text = "Bootscode"
        '
        'lblProductCode
        '
        Me.lblProductCode.Location = New System.Drawing.Point(7, 32)
        Me.lblProductCode.Name = "lblProductCode"
        Me.lblProductCode.Size = New System.Drawing.Size(107, 20)
        Me.lblProductCode.Text = "1234567891234"
        '
        'lblProdDescription3
        '
        Me.lblProdDescription3.Location = New System.Drawing.Point(7, 92)
        Me.lblProdDescription3.Name = "lblProdDescription3"
        Me.lblProdDescription3.Size = New System.Drawing.Size(140, 22)
        Me.lblProdDescription3.Text = "description3"
        '
        'lblProdDescription2
        '
        Me.lblProdDescription2.Location = New System.Drawing.Point(7, 74)
        Me.lblProdDescription2.Name = "lblProdDescription2"
        Me.lblProdDescription2.Size = New System.Drawing.Size(140, 23)
        Me.lblProdDescription2.Text = "description2"
        '
        'lblProdDescription1
        '
        Me.lblProdDescription1.Location = New System.Drawing.Point(7, 58)
        Me.lblProdDescription1.Name = "lblProdDescription1"
        Me.lblProdDescription1.Size = New System.Drawing.Size(140, 21)
        Me.lblProdDescription1.Text = "description1"
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(189, 19)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 10
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(7, 118)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(50, 20)
        Me.lblStatus.Text = "Status:"
        '
        'lblStatusText
        '
        Me.lblStatusText.Location = New System.Drawing.Point(60, 118)
        Me.lblStatusText.Name = "lblStatusText"
        Me.lblStatusText.Size = New System.Drawing.Size(152, 20)
        '
        'lblBckShpQty
        '
        Me.lblBckShpQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblBckShpQty.Location = New System.Drawing.Point(7, 220)
        Me.lblBckShpQty.Name = "lblBckShpQty"
        Me.lblBckShpQty.Size = New System.Drawing.Size(163, 15)
        '
        'lblBackQtyText
        '
        Me.lblBackQtyText.Location = New System.Drawing.Point(174, 220)
        Me.lblBackQtyText.Name = "lblBackQtyText"
        Me.lblBackQtyText.Size = New System.Drawing.Size(25, 15)
        Me.lblBackQtyText.Text = "000"
        Me.lblBackQtyText.Visible = False
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(203, 211)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small1.TabIndex = 18
        '
        'Btn_Next_small1
        '
        Me.Btn_Next_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Next_small1.Location = New System.Drawing.Point(7, 245)
        Me.Btn_Next_small1.Name = "Btn_Next_small1"
        Me.Btn_Next_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Next_small1.TabIndex = 19
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(184, 245)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 20
        '
        'btnView
        '
        Me.btnView.BackColor = System.Drawing.Color.Transparent
        Me.btnView.Location = New System.Drawing.Point(95, 245)
        Me.btnView.Name = "btnView"
        Me.btnView.Size = New System.Drawing.Size(50, 24)
        Me.btnView.TabIndex = 40
        '
        'lblOSSR
        '
        Me.lblOSSR.Location = New System.Drawing.Point(192, 58)
        Me.lblOSSR.Name = "lblOSSR"
        Me.lblOSSR.Size = New System.Drawing.Size(44, 20)
        '
        'Btn_OSSRItem1
        '
        Me.Btn_OSSRItem1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_OSSRItem1.Location = New System.Drawing.Point(158, 87)
        Me.Btn_OSSRItem1.Name = "Btn_OSSRItem1"
        Me.Btn_OSSRItem1.Size = New System.Drawing.Size(75, 21)
        Me.Btn_OSSRItem1.TabIndex = 70
        '
        'lblSODStockFile
        '
        Me.lblSODStockFile.Location = New System.Drawing.Point(7, 135)
        Me.lblSODStockFile.Name = "lblSODStockFile"
        Me.lblSODStockFile.Size = New System.Drawing.Size(140, 17)
        Me.lblSODStockFile.Text = "Start of Day Stock File:"
        '
        'lblTotalItemCount
        '
        Me.lblTotalItemCount.Location = New System.Drawing.Point(7, 152)
        Me.lblTotalItemCount.Name = "lblTotalItemCount"
        Me.lblTotalItemCount.Size = New System.Drawing.Size(107, 12)
        Me.lblTotalItemCount.Text = "Total Item Count:"
        '
        'lblStockText
        '
        Me.lblStockText.Location = New System.Drawing.Point(175, 135)
        Me.lblStockText.Name = "lblStockText"
        Me.lblStockText.Size = New System.Drawing.Size(57, 17)
        '
        'lblTotalItemText
        '
        Me.lblTotalItemText.Location = New System.Drawing.Point(175, 152)
        Me.lblTotalItemText.Name = "lblTotalItemText"
        Me.lblTotalItemText.Size = New System.Drawing.Size(58, 14)
        '
        'cmbSite
        '
        Me.cmbSite.Location = New System.Drawing.Point(7, 191)
        Me.cmbSite.Name = "cmbSite"
        Me.cmbSite.Size = New System.Drawing.Size(192, 22)
        Me.cmbSite.TabIndex = 80
        Me.cmbSite.Visible = False
        '
        'lblSite
        '
        Me.lblSite.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSite.Location = New System.Drawing.Point(7, 171)
        Me.lblSite.Name = "lblSite"
        Me.lblSite.Size = New System.Drawing.Size(93, 17)
        Me.lblSite.Text = "Select Site"
        Me.lblSite.Visible = False
        '
        'Btn_CalcPad_small2
        '
        Me.Btn_CalcPad_small2.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CalcPad_small2.Location = New System.Drawing.Point(128, 19)
        Me.Btn_CalcPad_small2.Name = "Btn_CalcPad_small2"
        Me.Btn_CalcPad_small2.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small2.TabIndex = 95
        Me.Btn_CalcPad_small2.Visible = False
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 111
        '
        'frmEXItemDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.Btn_CalcPad_small2)
        Me.Controls.Add(Me.lblSite)
        Me.Controls.Add(Me.cmbSite)
        Me.Controls.Add(Me.lblTotalItemText)
        Me.Controls.Add(Me.lblStockText)
        Me.Controls.Add(Me.lblTotalItemCount)
        Me.Controls.Add(Me.lblSODStockFile)
        Me.Controls.Add(Me.lblOSSR)
        Me.Controls.Add(Me.Btn_OSSRItem1)
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.Btn_Next_small1)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.lblBackQtyText)
        Me.Controls.Add(Me.lblStatusText)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.lblProdDescription3)
        Me.Controls.Add(Me.lblProdDescription2)
        Me.Controls.Add(Me.lblProdDescription1)
        Me.Controls.Add(Me.lblProductCode)
        Me.Controls.Add(Me.lblBootsCode)
        Me.Controls.Add(Me.lblBckShpQty)
        Me.Name = "frmEXItemDetails"
        Me.Text = "Excess Stock - BS"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblBootsCode As System.Windows.Forms.Label
    Friend WithEvents lblProductCode As System.Windows.Forms.Label
    Friend WithEvents lblProdDescription3 As System.Windows.Forms.Label
    Friend WithEvents lblProdDescription2 As System.Windows.Forms.Label
    Friend WithEvents lblProdDescription1 As System.Windows.Forms.Label
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lblStatusText As System.Windows.Forms.Label
    Friend WithEvents lblBckShpQty As System.Windows.Forms.Label
    Friend WithEvents lblBackQtyText As System.Windows.Forms.Label
    Friend WithEvents Btn_CalcPad_small1 As CustomButtons.btn_CalcPad_small
    Friend WithEvents Btn_Next_small1 As CustomButtons.btn_Next_small
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents btnView As CustomButtons.btnView
    Friend WithEvents lblOSSR As System.Windows.Forms.Label
    Friend WithEvents Btn_OSSRItem1 As CustomButtons.btn_OSSRItem
    Friend WithEvents lblSODStockFile As System.Windows.Forms.Label
    Friend WithEvents lblTotalItemCount As System.Windows.Forms.Label
    Friend WithEvents lblStockText As System.Windows.Forms.Label
    Friend WithEvents lblTotalItemText As System.Windows.Forms.Label
    Friend WithEvents cmbSite As System.Windows.Forms.ComboBox
    Friend WithEvents lblSite As System.Windows.Forms.Label
    Friend WithEvents Btn_CalcPad_small2 As CustomButtons.btn_CalcPad_small
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
End Class
