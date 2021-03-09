<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmAutoSYSItemDetails
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
        Me.lblDesc1 = New System.Windows.Forms.Label
        Me.lblDesc2 = New System.Windows.Forms.Label
        Me.lblDesc3 = New System.Windows.Forms.Label
        Me.lblSt = New System.Windows.Forms.Label
        Me.lblshelfCap = New System.Windows.Forms.Label
        Me.lblstockFig = New System.Windows.Forms.Label
        Me.lblStatus = New System.Windows.Forms.Label
        Me.lblPSC = New System.Windows.Forms.Label
        Me.lblTSF = New System.Windows.Forms.Label
        Me.Btn_Quit_small1 = New CustomButtons.btn_Quit_small
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.lblMsg1 = New System.Windows.Forms.Label
        Me.lblPicked = New System.Windows.Forms.Label
        Me.BtnView1 = New CustomButtons.btnView
        Me.lblAction = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblBootsCode
        '
        Me.lblBootsCode.Location = New System.Drawing.Point(19, 13)
        Me.lblBootsCode.Name = "lblBootsCode"
        Me.lblBootsCode.Size = New System.Drawing.Size(100, 20)
        Me.lblBootsCode.Text = "11-34-965"
        '
        'lblProductCode
        '
        Me.lblProductCode.Location = New System.Drawing.Point(19, 29)
        Me.lblProductCode.Name = "lblProductCode"
        Me.lblProductCode.Size = New System.Drawing.Size(153, 20)
        Me.lblProductCode.Text = "1-234123-678545"
        '
        'lblDesc1
        '
        Me.lblDesc1.Location = New System.Drawing.Point(19, 52)
        Me.lblDesc1.Name = "lblDesc1"
        Me.lblDesc1.Size = New System.Drawing.Size(100, 20)
        Me.lblDesc1.Text = "Alcon Opti fibre"
        '
        'lblDesc2
        '
        Me.lblDesc2.Location = New System.Drawing.Point(19, 68)
        Me.lblDesc2.Name = "lblDesc2"
        Me.lblDesc2.Size = New System.Drawing.Size(100, 20)
        Me.lblDesc2.Text = "Express"
        '
        'lblDesc3
        '
        Me.lblDesc3.Location = New System.Drawing.Point(19, 84)
        Me.lblDesc3.Name = "lblDesc3"
        Me.lblDesc3.Size = New System.Drawing.Size(100, 20)
        Me.lblDesc3.Text = "335 ml"
        '
        'lblSt
        '
        Me.lblSt.Location = New System.Drawing.Point(19, 113)
        Me.lblSt.Name = "lblSt"
        Me.lblSt.Size = New System.Drawing.Size(58, 20)
        Me.lblSt.Text = "Status"
        '
        'lblshelfCap
        '
        Me.lblshelfCap.Location = New System.Drawing.Point(19, 132)
        Me.lblshelfCap.Name = "lblshelfCap"
        Me.lblshelfCap.Size = New System.Drawing.Size(133, 20)
        Me.lblshelfCap.Text = "Physical Shelf Capacity"
        '
        'lblstockFig
        '
        Me.lblstockFig.Location = New System.Drawing.Point(19, 151)
        Me.lblstockFig.Name = "lblstockFig"
        Me.lblstockFig.Size = New System.Drawing.Size(153, 20)
        Me.lblstockFig.Text = "Start of Day Stock File"
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(66, 113)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(174, 20)
        Me.lblStatus.Text = "Active"
        '
        'lblPSC
        '
        Me.lblPSC.Location = New System.Drawing.Point(165, 132)
        Me.lblPSC.Name = "lblPSC"
        Me.lblPSC.Size = New System.Drawing.Size(40, 20)
        Me.lblPSC.Text = "0"
        '
        'lblTSF
        '
        Me.lblTSF.Location = New System.Drawing.Point(165, 151)
        Me.lblTSF.Name = "lblTSF"
        Me.lblTSF.Size = New System.Drawing.Size(36, 20)
        Me.lblTSF.Text = "0"
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(13, 238)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.TabIndex = 1
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(198, 13)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 0
        '
        'lblMsg1
        '
        Me.lblMsg1.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg1.Location = New System.Drawing.Point(19, 175)
        Me.lblMsg1.Name = "lblMsg1"
        Me.lblMsg1.Size = New System.Drawing.Size(207, 20)
        '
        'lblPicked
        '
        Me.lblPicked.Location = New System.Drawing.Point(128, 14)
        Me.lblPicked.Name = "lblPicked"
        Me.lblPicked.Size = New System.Drawing.Size(51, 20)
        Me.lblPicked.Text = "PICKED"
        '
        'BtnView1
        '
        Me.BtnView1.BackColor = System.Drawing.Color.Transparent
        Me.BtnView1.Location = New System.Drawing.Point(172, 236)
        Me.BtnView1.Name = "BtnView1"
        Me.BtnView1.Size = New System.Drawing.Size(58, 24)
        Me.BtnView1.TabIndex = 50
        '
        'lblAction
        '
        Me.lblAction.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblAction.Location = New System.Drawing.Point(13, 213)
        Me.lblAction.Name = "lblAction"
        Me.lblAction.Size = New System.Drawing.Size(221, 20)
        Me.lblAction.Text = "Action: Scan next item to continue"
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 23
        '
        'frmAutoSYSItemDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblAction)
        Me.Controls.Add(Me.BtnView1)
        Me.Controls.Add(Me.lblPicked)
        Me.Controls.Add(Me.lblMsg1)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblTSF)
        Me.Controls.Add(Me.lblPSC)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.lblstockFig)
        Me.Controls.Add(Me.lblshelfCap)
        Me.Controls.Add(Me.lblSt)
        Me.Controls.Add(Me.lblDesc3)
        Me.Controls.Add(Me.lblDesc2)
        Me.Controls.Add(Me.lblDesc1)
        Me.Controls.Add(Me.lblProductCode)
        Me.Controls.Add(Me.lblBootsCode)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Name = "frmAutoSYSItemDetails"
        Me.Text = "Stuff Your Shelves"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents Btn_Quit_small1 As CustomButtons.btn_Quit_small
    Friend WithEvents lblBootsCode As System.Windows.Forms.Label
    Friend WithEvents lblProductCode As System.Windows.Forms.Label
    Friend WithEvents lblDesc1 As System.Windows.Forms.Label
    Friend WithEvents lblDesc2 As System.Windows.Forms.Label
    Friend WithEvents lblDesc3 As System.Windows.Forms.Label
    Friend WithEvents lblSt As System.Windows.Forms.Label
    Friend WithEvents lblshelfCap As System.Windows.Forms.Label
    Friend WithEvents lblstockFig As System.Windows.Forms.Label
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lblPSC As System.Windows.Forms.Label
    Friend WithEvents lblTSF As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblMsg1 As System.Windows.Forms.Label
    Friend WithEvents lblPicked As System.Windows.Forms.Label
    Friend WithEvents BtnView1 As CustomButtons.btnView
    Friend WithEvents lblAction As System.Windows.Forms.Label
End Class
