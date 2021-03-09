<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSMItemDetails
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
        Me.lblBtsCode = New System.Windows.Forms.Label
        Me.lblEAN = New System.Windows.Forms.Label
        Me.lblDesc1 = New System.Windows.Forms.Label
        Me.lblDesc2 = New System.Windows.Forms.Label
        Me.lblDesc3 = New System.Windows.Forms.Label
        Me.lblStatus = New System.Windows.Forms.Label
        Me.lblSlsFlrQnty = New System.Windows.Forms.Label
        Me.lblSlsflrVal = New System.Windows.Forms.Label
        Me.lblStsVal = New System.Windows.Forms.Label
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.lblMltSiteCnt = New System.Windows.Forms.Label
        Me.cmbbxMltSites = New System.Windows.Forms.ComboBox
        Me.btnCalcPadSlsFlr = New CustomButtons.btn_CalcPad_small
        Me.btnView = New CustomButtons.btnView
        Me.btnNext = New CustomButtons.btn_Next_small
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.lblOSSR = New System.Windows.Forms.Label
        Me.btn_OSSRItem = New CustomButtons.btn_OSSRItem
        Me.lblTotalItemText = New System.Windows.Forms.Label
        Me.lblStockText = New System.Windows.Forms.Label
        Me.lblTotalItemCount = New System.Windows.Forms.Label
        Me.lblSODStockFile = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.btnZero = New CustomButtons.btn_Zero
        Me.SuspendLayout()
        '
        'lblBtsCode
        '
        Me.lblBtsCode.Location = New System.Drawing.Point(12, 5)
        Me.lblBtsCode.Name = "lblBtsCode"
        Me.lblBtsCode.Size = New System.Drawing.Size(126, 15)
        '
        'lblEAN
        '
        Me.lblEAN.Location = New System.Drawing.Point(12, 21)
        Me.lblEAN.Name = "lblEAN"
        Me.lblEAN.Size = New System.Drawing.Size(152, 15)
        '
        'lblDesc1
        '
        Me.lblDesc1.Location = New System.Drawing.Point(12, 45)
        Me.lblDesc1.Name = "lblDesc1"
        Me.lblDesc1.Size = New System.Drawing.Size(97, 15)
        '
        'lblDesc2
        '
        Me.lblDesc2.Location = New System.Drawing.Point(12, 64)
        Me.lblDesc2.Name = "lblDesc2"
        Me.lblDesc2.Size = New System.Drawing.Size(97, 15)
        '
        'lblDesc3
        '
        Me.lblDesc3.Location = New System.Drawing.Point(12, 81)
        Me.lblDesc3.Name = "lblDesc3"
        Me.lblDesc3.Size = New System.Drawing.Size(97, 15)
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(12, 104)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(44, 15)
        Me.lblStatus.Text = "Status:"
        '
        'lblSlsFlrQnty
        '
        Me.lblSlsFlrQnty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSlsFlrQnty.Location = New System.Drawing.Point(9, 206)
        Me.lblSlsFlrQnty.Name = "lblSlsFlrQnty"
        Me.lblSlsFlrQnty.Size = New System.Drawing.Size(171, 15)
        Me.lblSlsFlrQnty.Text = "Enter Quantity on Shelf:"
        '
        'lblSlsflrVal
        '
        Me.lblSlsflrVal.Location = New System.Drawing.Point(177, 206)
        Me.lblSlsflrVal.Name = "lblSlsflrVal"
        Me.lblSlsflrVal.Size = New System.Drawing.Size(25, 15)
        Me.lblSlsflrVal.Tag = ""
        Me.lblSlsflrVal.Text = "0"
        '
        'lblStsVal
        '
        Me.lblStsVal.Location = New System.Drawing.Point(65, 104)
        Me.lblStsVal.Name = "lblStsVal"
        Me.lblStsVal.Size = New System.Drawing.Size(166, 15)
        Me.lblStsVal.Text = "Active"
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(187, 237)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 45
        '
        'lblMltSiteCnt
        '
        Me.lblMltSiteCnt.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMltSiteCnt.Location = New System.Drawing.Point(12, 158)
        Me.lblMltSiteCnt.Name = "lblMltSiteCnt"
        Me.lblMltSiteCnt.Size = New System.Drawing.Size(106, 15)
        Me.lblMltSiteCnt.Text = "Select Location:"
        '
        'cmbbxMltSites
        '
        Me.cmbbxMltSites.Location = New System.Drawing.Point(12, 171)
        Me.cmbbxMltSites.Name = "cmbbxMltSites"
        Me.cmbbxMltSites.Size = New System.Drawing.Size(218, 22)
        Me.cmbbxMltSites.TabIndex = 77
        '
        'btnCalcPadSlsFlr
        '
        Me.btnCalcPadSlsFlr.BackColor = System.Drawing.Color.Transparent
        Me.btnCalcPadSlsFlr.Location = New System.Drawing.Point(207, 200)
        Me.btnCalcPadSlsFlr.Name = "btnCalcPadSlsFlr"
        Me.btnCalcPadSlsFlr.Size = New System.Drawing.Size(24, 28)
        Me.btnCalcPadSlsFlr.TabIndex = 92
        '
        'btnView
        '
        Me.btnView.BackColor = System.Drawing.Color.Transparent
        Me.btnView.Location = New System.Drawing.Point(122, 237)
        Me.btnView.Name = "btnView"
        Me.btnView.Size = New System.Drawing.Size(58, 24)
        Me.btnView.TabIndex = 107
        '
        'btnNext
        '
        Me.btnNext.BackColor = System.Drawing.Color.Transparent
        Me.btnNext.Location = New System.Drawing.Point(6, 237)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(50, 24)
        Me.btnNext.TabIndex = 61
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(199, 8)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 131
        '
        'lblOSSR
        '
        Me.lblOSSR.Location = New System.Drawing.Point(195, 47)
        Me.lblOSSR.Name = "lblOSSR"
        Me.lblOSSR.Size = New System.Drawing.Size(39, 20)
        '
        'btn_OSSRItem
        '
        Me.btn_OSSRItem.BackColor = System.Drawing.Color.Transparent
        Me.btn_OSSRItem.Location = New System.Drawing.Point(162, 72)
        Me.btn_OSSRItem.Name = "btn_OSSRItem"
        Me.btn_OSSRItem.Size = New System.Drawing.Size(75, 24)
        Me.btn_OSSRItem.TabIndex = 165
        '
        'lblTotalItemText
        '
        Me.lblTotalItemText.Location = New System.Drawing.Point(175, 138)
        Me.lblTotalItemText.Name = "lblTotalItemText"
        Me.lblTotalItemText.Size = New System.Drawing.Size(58, 14)
        Me.lblTotalItemText.Text = "0"
        '
        'lblStockText
        '
        Me.lblStockText.Location = New System.Drawing.Point(175, 121)
        Me.lblStockText.Name = "lblStockText"
        Me.lblStockText.Size = New System.Drawing.Size(57, 17)
        '
        'lblTotalItemCount
        '
        Me.lblTotalItemCount.Location = New System.Drawing.Point(11, 138)
        Me.lblTotalItemCount.Name = "lblTotalItemCount"
        Me.lblTotalItemCount.Size = New System.Drawing.Size(107, 12)
        Me.lblTotalItemCount.Text = "Total Item Count:"
        '
        'lblSODStockFile
        '
        Me.lblSODStockFile.Location = New System.Drawing.Point(11, 121)
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
        Me.objStatusBar.TabIndex = 120
        '
        'btnZero
        '
        Me.btnZero.BackColor = System.Drawing.Color.Transparent
        Me.btnZero.Location = New System.Drawing.Point(65, 237)
        Me.btnZero.Name = "btnZero"
        Me.btnZero.Size = New System.Drawing.Size(50, 24)
        Me.btnZero.TabIndex = 177
        '
        'frmSMItemDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnZero)
        Me.Controls.Add(Me.lblTotalItemText)
        Me.Controls.Add(Me.lblStockText)
        Me.Controls.Add(Me.lblTotalItemCount)
        Me.Controls.Add(Me.lblSODStockFile)
        Me.Controls.Add(Me.btn_OSSRItem)
        Me.Controls.Add(Me.lblOSSR)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.btnCalcPadSlsFlr)
        Me.Controls.Add(Me.cmbbxMltSites)
        Me.Controls.Add(Me.lblMltSiteCnt)
        Me.Controls.Add(Me.btnNext)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.lblStsVal)
        Me.Controls.Add(Me.lblSlsflrVal)
        Me.Controls.Add(Me.lblSlsFlrQnty)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.lblDesc3)
        Me.Controls.Add(Me.lblDesc2)
        Me.Controls.Add(Me.lblDesc1)
        Me.Controls.Add(Me.lblEAN)
        Me.Controls.Add(Me.lblBtsCode)
        Me.Name = "frmSMItemDetails"
        Me.Text = "Shelf Monitor"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblBtsCode As System.Windows.Forms.Label
    Friend WithEvents lblEAN As System.Windows.Forms.Label
    Friend WithEvents lblDesc1 As System.Windows.Forms.Label
    Friend WithEvents lblDesc2 As System.Windows.Forms.Label
    Friend WithEvents lblDesc3 As System.Windows.Forms.Label
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lblSlsFlrQnty As System.Windows.Forms.Label
    Friend WithEvents lblSlsflrVal As System.Windows.Forms.Label
    Friend WithEvents lblStsVal As System.Windows.Forms.Label
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents lblMltSiteCnt As System.Windows.Forms.Label
    Friend WithEvents cmbbxMltSites As System.Windows.Forms.ComboBox
    Friend WithEvents btnCalcPadSlsFlr As CustomButtons.btn_CalcPad_small
    Friend WithEvents btnView As CustomButtons.btnView
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents btnNext As CustomButtons.btn_Next_small
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents lblOSSR As System.Windows.Forms.Label
    Friend WithEvents btn_OSSRItem As CustomButtons.btn_OSSRItem
    Friend WithEvents lblTotalItemText As System.Windows.Forms.Label
    Friend WithEvents lblStockText As System.Windows.Forms.Label
    Friend WithEvents lblTotalItemCount As System.Windows.Forms.Label
    Friend WithEvents lblSODStockFile As System.Windows.Forms.Label
    Friend WithEvents btnZero As CustomButtons.btn_Zero
End Class
