<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSMFillQuantity
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
        Me.btnCalcPadFillQnty = New CustomButtons.btn_CalcPad_small
        Me.cmbbxMltSites = New System.Windows.Forms.ComboBox
        Me.lblMltSiteCnt = New System.Windows.Forms.Label
        Me.btnNext = New CustomButtons.btn_Next_small
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.lblFilQntyVal = New System.Windows.Forms.Label
        Me.lblFilQnty = New System.Windows.Forms.Label
        Me.lblStsVal = New System.Windows.Forms.Label
        Me.lblStatus = New System.Windows.Forms.Label
        Me.lblDesc1 = New System.Windows.Forms.Label
        Me.lblDesc3 = New System.Windows.Forms.Label
        Me.lblDesc2 = New System.Windows.Forms.Label
        Me.lblEAN = New System.Windows.Forms.Label
        Me.lblBtsCode = New System.Windows.Forms.Label
        Me.lblSlsflrVal = New System.Windows.Forms.Label
        Me.lblSlsFlrQnty = New System.Windows.Forms.Label
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.lblTotalItemText = New System.Windows.Forms.Label
        Me.lblStockText = New System.Windows.Forms.Label
        Me.lblTotalItemCount = New System.Windows.Forms.Label
        Me.lblSODStockFile = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'btnView
        '
        Me.btnView.BackColor = System.Drawing.Color.Transparent
        Me.btnView.Location = New System.Drawing.Point(120, 237)
        Me.btnView.Name = "btnView"
        Me.btnView.Size = New System.Drawing.Size(58, 24)
        Me.btnView.TabIndex = 121
        '
        'btnCalcPadFillQnty
        '
        Me.btnCalcPadFillQnty.BackColor = System.Drawing.Color.Transparent
        Me.btnCalcPadFillQnty.Location = New System.Drawing.Point(207, 206)
        Me.btnCalcPadFillQnty.Name = "btnCalcPadFillQnty"
        Me.btnCalcPadFillQnty.Size = New System.Drawing.Size(24, 28)
        Me.btnCalcPadFillQnty.TabIndex = 119
        '
        'cmbbxMltSites
        '
        Me.cmbbxMltSites.DisplayMember = "SELECT"
        Me.cmbbxMltSites.Enabled = False
        Me.cmbbxMltSites.Location = New System.Drawing.Point(12, 160)
        Me.cmbbxMltSites.Name = "cmbbxMltSites"
        Me.cmbbxMltSites.Size = New System.Drawing.Size(224, 22)
        Me.cmbbxMltSites.TabIndex = 118
        '
        'lblMltSiteCnt
        '
        Me.lblMltSiteCnt.Location = New System.Drawing.Point(12, 143)
        Me.lblMltSiteCnt.Name = "lblMltSiteCnt"
        Me.lblMltSiteCnt.Size = New System.Drawing.Size(106, 15)
        Me.lblMltSiteCnt.Text = "Location:"
        '
        'btnNext
        '
        Me.btnNext.BackColor = System.Drawing.Color.Transparent
        Me.btnNext.Location = New System.Drawing.Point(5, 237)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(50, 24)
        Me.btnNext.TabIndex = 117
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(186, 237)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 116
        '
        'lblFilQntyVal
        '
        Me.lblFilQntyVal.Location = New System.Drawing.Point(176, 212)
        Me.lblFilQntyVal.Name = "lblFilQntyVal"
        Me.lblFilQntyVal.Size = New System.Drawing.Size(25, 15)
        Me.lblFilQntyVal.Tag = ""
        Me.lblFilQntyVal.Text = "0"
        '
        'lblFilQnty
        '
        Me.lblFilQnty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblFilQnty.Location = New System.Drawing.Point(9, 212)
        Me.lblFilQnty.Name = "lblFilQnty"
        Me.lblFilQnty.Size = New System.Drawing.Size(171, 15)
        Me.lblFilQnty.Text = "Fill Quantity :"
        '
        'lblStsVal
        '
        Me.lblStsVal.Location = New System.Drawing.Point(65, 91)
        Me.lblStsVal.Name = "lblStsVal"
        Me.lblStsVal.Size = New System.Drawing.Size(99, 15)
        Me.lblStsVal.Text = "Active"
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(12, 91)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(44, 15)
        Me.lblStatus.Text = "Status:"
        '
        'lblDesc1
        '
        Me.lblDesc1.Location = New System.Drawing.Point(12, 37)
        Me.lblDesc1.Name = "lblDesc1"
        Me.lblDesc1.Size = New System.Drawing.Size(97, 15)
        '
        'lblDesc3
        '
        Me.lblDesc3.Location = New System.Drawing.Point(12, 72)
        Me.lblDesc3.Name = "lblDesc3"
        Me.lblDesc3.Size = New System.Drawing.Size(97, 15)
        '
        'lblDesc2
        '
        Me.lblDesc2.Location = New System.Drawing.Point(12, 54)
        Me.lblDesc2.Name = "lblDesc2"
        Me.lblDesc2.Size = New System.Drawing.Size(97, 15)
        '
        'lblEAN
        '
        Me.lblEAN.Location = New System.Drawing.Point(12, 20)
        Me.lblEAN.Name = "lblEAN"
        Me.lblEAN.Size = New System.Drawing.Size(152, 15)
        '
        'lblBtsCode
        '
        Me.lblBtsCode.Location = New System.Drawing.Point(12, 4)
        Me.lblBtsCode.Name = "lblBtsCode"
        Me.lblBtsCode.Size = New System.Drawing.Size(126, 15)
        '
        'lblSlsflrVal
        '
        Me.lblSlsflrVal.Location = New System.Drawing.Point(177, 186)
        Me.lblSlsflrVal.Name = "lblSlsflrVal"
        Me.lblSlsflrVal.Size = New System.Drawing.Size(25, 15)
        Me.lblSlsflrVal.Tag = ""
        '
        'lblSlsFlrQnty
        '
        Me.lblSlsFlrQnty.Location = New System.Drawing.Point(9, 186)
        Me.lblSlsFlrQnty.Name = "lblSlsFlrQnty"
        Me.lblSlsFlrQnty.Size = New System.Drawing.Size(171, 15)
        Me.lblSlsFlrQnty.Text = "Quantity on Shelf:"
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(198, 4)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 140
        '
        'lblTotalItemText
        '
        Me.lblTotalItemText.Location = New System.Drawing.Point(175, 125)
        Me.lblTotalItemText.Name = "lblTotalItemText"
        Me.lblTotalItemText.Size = New System.Drawing.Size(58, 14)
        Me.lblTotalItemText.Text = "0"
        '
        'lblStockText
        '
        Me.lblStockText.Location = New System.Drawing.Point(175, 109)
        Me.lblStockText.Name = "lblStockText"
        Me.lblStockText.Size = New System.Drawing.Size(57, 17)
        '
        'lblTotalItemCount
        '
        Me.lblTotalItemCount.Location = New System.Drawing.Point(11, 125)
        Me.lblTotalItemCount.Name = "lblTotalItemCount"
        Me.lblTotalItemCount.Size = New System.Drawing.Size(107, 12)
        Me.lblTotalItemCount.Text = "Total Item Count:"
        '
        'lblSODStockFile
        '
        Me.lblSODStockFile.Location = New System.Drawing.Point(11, 109)
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
        Me.objStatusBar.TabIndex = 128
        '
        'frmSMFillQuantity
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblTotalItemText)
        Me.Controls.Add(Me.lblStockText)
        Me.Controls.Add(Me.lblTotalItemCount)
        Me.Controls.Add(Me.lblSODStockFile)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.lblSlsflrVal)
        Me.Controls.Add(Me.lblSlsFlrQnty)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblDesc3)
        Me.Controls.Add(Me.lblDesc2)
        Me.Controls.Add(Me.lblEAN)
        Me.Controls.Add(Me.lblBtsCode)
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.btnCalcPadFillQnty)
        Me.Controls.Add(Me.cmbbxMltSites)
        Me.Controls.Add(Me.lblMltSiteCnt)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.lblFilQntyVal)
        Me.Controls.Add(Me.lblFilQnty)
        Me.Controls.Add(Me.lblStsVal)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.lblDesc1)
        Me.Controls.Add(Me.btnNext)
        Me.Name = "frmSMFillQuantity"
        Me.Text = "Shelf Monitor"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnView As CustomButtons.btnView
    Friend WithEvents btnCalcPadFillQnty As CustomButtons.btn_CalcPad_small
    Friend WithEvents cmbbxMltSites As System.Windows.Forms.ComboBox
    Friend WithEvents lblMltSiteCnt As System.Windows.Forms.Label
    Friend WithEvents btnNext As CustomButtons.btn_Next_small
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents lblFilQntyVal As System.Windows.Forms.Label
    Friend WithEvents lblFilQnty As System.Windows.Forms.Label
    Friend WithEvents lblStsVal As System.Windows.Forms.Label
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lblDesc1 As System.Windows.Forms.Label
    Friend WithEvents lblDesc3 As System.Windows.Forms.Label
    Friend WithEvents lblDesc2 As System.Windows.Forms.Label
    Friend WithEvents lblEAN As System.Windows.Forms.Label
    Friend WithEvents lblBtsCode As System.Windows.Forms.Label

    Friend WithEvents lblSlsflrVal As System.Windows.Forms.Label
    Friend WithEvents lblSlsFlrQnty As System.Windows.Forms.Label

    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents lblTotalItemText As System.Windows.Forms.Label
    Friend WithEvents lblStockText As System.Windows.Forms.Label
    Friend WithEvents lblTotalItemCount As System.Windows.Forms.Label
    Friend WithEvents lblSODStockFile As System.Windows.Forms.Label

End Class
