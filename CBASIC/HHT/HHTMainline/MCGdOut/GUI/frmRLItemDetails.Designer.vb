<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmRLItemDetails
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
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.btnNext = New CustomButtons.btn_Next_small
        Me.btnView = New CustomButtons.btnView
        Me.lblTitle = New System.Windows.Forms.Label
        Me.lblDesc = New System.Windows.Forms.Label
        Me.lblBootsCode = New System.Windows.Forms.Label
        Me.lblBarcode = New System.Windows.Forms.Label
        Me.lblTSF = New System.Windows.Forms.Label
        Me.lblStatus = New System.Windows.Forms.Label
        Me.lblStockCount = New System.Windows.Forms.Label
        Me.btnProductCode = New CustomButtons.btn_CalcPad_small
        Me.btnQuantity = New CustomButtons.btn_CalcPad_small
        Me.lblStatusdata = New System.Windows.Forms.Label
        Me.lblTSFdata = New System.Windows.Forms.Label
        Me.lblStockCountdata = New System.Windows.Forms.Label
        Me.lblItemscan = New System.Windows.Forms.Label
        Me.btnConfirm = New CustomButtons.Confirm
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.SuspendLayout()
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(180, 235)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 0
        '
        'btnNext
        '
        Me.btnNext.BackColor = System.Drawing.Color.Transparent
        Me.btnNext.Location = New System.Drawing.Point(14, 235)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(50, 24)
        Me.btnNext.TabIndex = 1
        '
        'btnView
        '
        Me.btnView.BackColor = System.Drawing.Color.Transparent
        Me.btnView.Location = New System.Drawing.Point(94, 235)
        Me.btnView.Name = "btnView"
        Me.btnView.Size = New System.Drawing.Size(58, 24)
        Me.btnView.TabIndex = 2
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.Location = New System.Drawing.Point(6, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(227, 53)
        Me.lblTitle.Text = "The following items have not been scanned / entered into the recall or the quanti" & _
            "ty is different from Total Stock File." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Check and confirm this is correct"
        '
        'lblDesc
        '
        Me.lblDesc.Location = New System.Drawing.Point(3, 57)
        Me.lblDesc.Name = "lblDesc"
        Me.lblDesc.Size = New System.Drawing.Size(187, 20)
        Me.lblDesc.Text = "Product Desc"
        '
        'lblBootsCode
        '
        Me.lblBootsCode.Location = New System.Drawing.Point(3, 77)
        Me.lblBootsCode.Name = "lblBootsCode"
        Me.lblBootsCode.Size = New System.Drawing.Size(187, 20)
        Me.lblBootsCode.Text = "Boots code"
        '
        'lblBarcode
        '
        Me.lblBarcode.Location = New System.Drawing.Point(3, 97)
        Me.lblBarcode.Name = "lblBarcode"
        Me.lblBarcode.Size = New System.Drawing.Size(187, 20)
        Me.lblBarcode.Text = "Product Code"
        '
        'lblTSF
        '
        Me.lblTSF.Location = New System.Drawing.Point(3, 166)
        Me.lblTSF.Name = "lblTSF"
        Me.lblTSF.Size = New System.Drawing.Size(135, 20)
        Me.lblTSF.Text = "Start of Day Stock File :"
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(3, 138)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(100, 20)
        Me.lblStatus.Text = "Status :"
        '
        'lblStockCount
        '
        Me.lblStockCount.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblStockCount.Location = New System.Drawing.Point(3, 196)
        Me.lblStockCount.Name = "lblStockCount"
        Me.lblStockCount.Size = New System.Drawing.Size(127, 20)
        Me.lblStockCount.Text = "Enter Stock Count :"
        '
        'btnProductCode
        '
        Me.btnProductCode.BackColor = System.Drawing.Color.Transparent
        Me.btnProductCode.Location = New System.Drawing.Point(196, 55)
        Me.btnProductCode.Name = "btnProductCode"
        Me.btnProductCode.Size = New System.Drawing.Size(24, 28)
        Me.btnProductCode.TabIndex = 14
        '
        'btnQuantity
        '
        Me.btnQuantity.BackColor = System.Drawing.Color.Transparent
        Me.btnQuantity.Location = New System.Drawing.Point(196, 188)
        Me.btnQuantity.Name = "btnQuantity"
        Me.btnQuantity.Size = New System.Drawing.Size(24, 28)
        Me.btnQuantity.TabIndex = 15
        '
        'lblStatusdata
        '
        Me.lblStatusdata.Location = New System.Drawing.Point(136, 138)
        Me.lblStatusdata.Name = "lblStatusdata"
        Me.lblStatusdata.Size = New System.Drawing.Size(96, 20)
        Me.lblStatusdata.Text = "Unactioned"
        '
        'lblTSFdata
        '
        Me.lblTSFdata.Location = New System.Drawing.Point(136, 166)
        Me.lblTSFdata.Name = "lblTSFdata"
        Me.lblTSFdata.Size = New System.Drawing.Size(95, 20)
        Me.lblTSFdata.Text = "0"
        '
        'lblStockCountdata
        '
        Me.lblStockCountdata.Location = New System.Drawing.Point(136, 196)
        Me.lblStockCountdata.Name = "lblStockCountdata"
        Me.lblStockCountdata.Size = New System.Drawing.Size(49, 20)
        Me.lblStockCountdata.Text = "0"
        '
        'lblItemscan
        '
        Me.lblItemscan.Location = New System.Drawing.Point(167, 104)
        Me.lblItemscan.Name = "lblItemscan"
        Me.lblItemscan.Size = New System.Drawing.Size(70, 20)
        Me.lblItemscan.Text = "1000/1000"
        '
        'btnConfirm
        '
        Me.btnConfirm.BackColor = System.Drawing.Color.Transparent
        Me.btnConfirm.Location = New System.Drawing.Point(87, 235)
        Me.btnConfirm.Name = "btnConfirm"
        Me.btnConfirm.Size = New System.Drawing.Size(65, 24)
        Me.btnConfirm.TabIndex = 36
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 48
        '
        'frmRLItemDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnConfirm)
        Me.Controls.Add(Me.lblItemscan)
        Me.Controls.Add(Me.lblStockCountdata)
        Me.Controls.Add(Me.lblTSFdata)
        Me.Controls.Add(Me.lblStatusdata)
        Me.Controls.Add(Me.btnQuantity)
        Me.Controls.Add(Me.btnProductCode)
        Me.Controls.Add(Me.lblStockCount)
        Me.Controls.Add(Me.lblBarcode)
        Me.Controls.Add(Me.lblTSF)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.lblBootsCode)
        Me.Controls.Add(Me.lblDesc)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.btnNext)
        Me.Controls.Add(Me.btnQuit)
        Me.Name = "frmRLItemDetails"
        Me.Text = "Recalls"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents btnNext As CustomButtons.btn_Next_small
    Friend WithEvents btnView As CustomButtons.btnView
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblDesc As System.Windows.Forms.Label
    Friend WithEvents lblBootsCode As System.Windows.Forms.Label
    Friend WithEvents lblBarcode As System.Windows.Forms.Label
    Friend WithEvents lblTSF As System.Windows.Forms.Label
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lblStockCount As System.Windows.Forms.Label
    Friend WithEvents btnProductCode As CustomButtons.btn_CalcPad_small
    Friend WithEvents btnQuantity As CustomButtons.btn_CalcPad_small
    Friend WithEvents lblStatusdata As System.Windows.Forms.Label
    Friend WithEvents lblTSFdata As System.Windows.Forms.Label
    Friend WithEvents lblStockCountdata As System.Windows.Forms.Label
    Friend WithEvents lblItemscan As System.Windows.Forms.Label
    Friend WithEvents btnConfirm As CustomButtons.Confirm
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
End Class
