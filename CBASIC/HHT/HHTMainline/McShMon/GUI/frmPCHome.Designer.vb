<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmPCHome
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
        Me.lblTrgt = New System.Windows.Forms.Label
        Me.lblCmplt = New System.Windows.Forms.Label
        Me.lblScanEnterPdtCode = New System.Windows.Forms.Label
        Me.lblTargetNum = New System.Windows.Forms.Label
        Me.lblCompletedNum = New System.Windows.Forms.Label
        Me.lblScanSEL = New System.Windows.Forms.Label
        Me.Btn_Next_small2 = New CustomButtons.btn_Next_small
        Me.Btn_Quit_small1 = New CustomButtons.btn_Quit_small
        Me.objProdSEL = New MCShMon.ProdSEL
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.SuspendLayout()
        '
        'lblTrgt
        '
        Me.lblTrgt.Location = New System.Drawing.Point(14, 155)
        Me.lblTrgt.Name = "lblTrgt"
        Me.lblTrgt.Size = New System.Drawing.Size(46, 20)
        Me.lblTrgt.Text = "Target:"
        '
        'lblCmplt
        '
        Me.lblCmplt.Location = New System.Drawing.Point(120, 155)
        Me.lblCmplt.Name = "lblCmplt"
        Me.lblCmplt.Size = New System.Drawing.Size(71, 20)
        Me.lblCmplt.Text = "Completed:"
        '
        'lblScanEnterPdtCode
        '
        Me.lblScanEnterPdtCode.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblScanEnterPdtCode.Location = New System.Drawing.Point(12, 196)
        Me.lblScanEnterPdtCode.Name = "lblScanEnterPdtCode"
        Me.lblScanEnterPdtCode.Size = New System.Drawing.Size(223, 20)
        Me.lblScanEnterPdtCode.Text = "Scan/Enter Product Code "
        Me.lblScanEnterPdtCode.Visible = False
        '
        'lblTargetNum
        '
        Me.lblTargetNum.Location = New System.Drawing.Point(60, 156)
        Me.lblTargetNum.Name = "lblTargetNum"
        Me.lblTargetNum.Size = New System.Drawing.Size(51, 20)
        Me.lblTargetNum.Text = "0"
        '
        'lblCompletedNum
        '
        Me.lblCompletedNum.Location = New System.Drawing.Point(186, 155)
        Me.lblCompletedNum.Name = "lblCompletedNum"
        Me.lblCompletedNum.Size = New System.Drawing.Size(45, 20)
        Me.lblCompletedNum.Text = "0"
        '
        'lblScanSEL
        '
        Me.lblScanSEL.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblScanSEL.Location = New System.Drawing.Point(14, 195)
        Me.lblScanSEL.Name = "lblScanSEL"
        Me.lblScanSEL.Size = New System.Drawing.Size(121, 20)
        Me.lblScanSEL.Text = "Scan SEL Code"
        Me.lblScanSEL.Visible = False
        '
        'Btn_Next_small2
        '
        Me.Btn_Next_small2.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Next_small2.Location = New System.Drawing.Point(15, 227)
        Me.Btn_Next_small2.Name = "Btn_Next_small2"
        Me.Btn_Next_small2.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Next_small2.TabIndex = 12
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(161, 227)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.TabIndex = 13
        '
        'objProdSEL
        '
        Me.objProdSEL.BackColor = System.Drawing.SystemColors.Window
        Me.objProdSEL.Location = New System.Drawing.Point(14, 18)
        Me.objProdSEL.Name = "objProdSEL"
        Me.objProdSEL.Size = New System.Drawing.Size(187, 117)
        Me.objProdSEL.TabIndex = 5
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 20
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(205, 3)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 27
        '
        'frmPCHome
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Btn_Next_small2)
        Me.Controls.Add(Me.lblScanSEL)
        Me.Controls.Add(Me.objProdSEL)
        Me.Controls.Add(Me.lblCompletedNum)
        Me.Controls.Add(Me.lblTargetNum)
        Me.Controls.Add(Me.lblScanEnterPdtCode)
        Me.Controls.Add(Me.lblCmplt)
        Me.Controls.Add(Me.lblTrgt)
        Me.Name = "frmPCHome"
        Me.Text = "Price Check"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblTrgt As System.Windows.Forms.Label
    Friend WithEvents lblCmplt As System.Windows.Forms.Label
    Friend WithEvents lblScanEnterPdtCode As System.Windows.Forms.Label
    Friend WithEvents lblTargetNum As System.Windows.Forms.Label
    Friend WithEvents lblCompletedNum As System.Windows.Forms.Label
    Friend WithEvents objProdSEL As MCShMon.ProdSEL
    Friend WithEvents lblScanSEL As System.Windows.Forms.Label
    Friend WithEvents Btn_Next_small2 As CustomButtons.btn_Next_small
    Friend WithEvents Btn_Quit_small1 As CustomButtons.btn_Quit_small
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
End Class
