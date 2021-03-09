<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmIPConfigure
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmIPConfigure))
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.pbQuitBtn = New System.Windows.Forms.PictureBox
        Me.pbSaveBtn = New System.Windows.Forms.PictureBox
        Me.lblSecondary = New System.Windows.Forms.Label
        Me.lblPrimary = New System.Windows.Forms.Label
        Me.lblSecondaryIp = New System.Windows.Forms.Label
        Me.lblPrimaryIP = New System.Windows.Forms.Label
        Me.rbtnSecondaryIp = New System.Windows.Forms.RadioButton
        Me.rbtnPrimaryIp = New System.Windows.Forms.RadioButton
        Me.lblActiveIP = New System.Windows.Forms.Label
        Me.objStatusBar = New McUtilities.CustomStatusBar
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.HighlightText
        Me.Panel1.Controls.Add(Me.pbQuitBtn)
        Me.Panel1.Controls.Add(Me.pbSaveBtn)
        Me.Panel1.Controls.Add(Me.lblSecondary)
        Me.Panel1.Controls.Add(Me.lblPrimary)
        Me.Panel1.Controls.Add(Me.lblSecondaryIp)
        Me.Panel1.Controls.Add(Me.lblPrimaryIP)
        Me.Panel1.Controls.Add(Me.rbtnSecondaryIp)
        Me.Panel1.Controls.Add(Me.rbtnPrimaryIp)
        Me.Panel1.Controls.Add(Me.lblActiveIP)
        Me.Panel1.Location = New System.Drawing.Point(3, 24)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(234, 224)
        '
        'pbQuitBtn
        '
        Me.pbQuitBtn.Image = CType(resources.GetObject("pbQuitBtn.Image"), System.Drawing.Image)
        Me.pbQuitBtn.Location = New System.Drawing.Point(116, 187)
        Me.pbQuitBtn.Name = "pbQuitBtn"
        Me.pbQuitBtn.Size = New System.Drawing.Size(50, 24)
        Me.pbQuitBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbSaveBtn
        '
        Me.pbSaveBtn.Image = CType(resources.GetObject("pbSaveBtn.Image"), System.Drawing.Image)
        Me.pbSaveBtn.Location = New System.Drawing.Point(44, 187)
        Me.pbSaveBtn.Name = "pbSaveBtn"
        Me.pbSaveBtn.Size = New System.Drawing.Size(50, 24)
        Me.pbSaveBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblSecondary
        '
        Me.lblSecondary.Location = New System.Drawing.Point(21, 115)
        Me.lblSecondary.Name = "lblSecondary"
        Me.lblSecondary.Size = New System.Drawing.Size(186, 22)
        Me.lblSecondary.Text = "Secondary Controller"
        '
        'lblPrimary
        '
        Me.lblPrimary.Location = New System.Drawing.Point(21, 58)
        Me.lblPrimary.Name = "lblPrimary"
        Me.lblPrimary.Size = New System.Drawing.Size(186, 22)
        Me.lblPrimary.Text = "Primary Controller"
        '
        'lblSecondaryIp
        '
        Me.lblSecondaryIp.Location = New System.Drawing.Point(66, 144)
        Me.lblSecondaryIp.Name = "lblSecondaryIp"
        Me.lblSecondaryIp.Size = New System.Drawing.Size(89, 22)
        Me.lblSecondaryIp.Text = "192.168.4.28"
        '
        'lblPrimaryIP
        '
        Me.lblPrimaryIP.Location = New System.Drawing.Point(66, 87)
        Me.lblPrimaryIP.Name = "lblPrimaryIP"
        Me.lblPrimaryIP.Size = New System.Drawing.Size(86, 22)
        Me.lblPrimaryIP.Text = "192.168.4.27"
        '
        'rbtnSecondaryIp
        '
        Me.rbtnSecondaryIp.Location = New System.Drawing.Point(36, 140)
        Me.rbtnSecondaryIp.Name = "rbtnSecondaryIp"
        Me.rbtnSecondaryIp.Size = New System.Drawing.Size(30, 20)
        Me.rbtnSecondaryIp.TabIndex = 4
        '
        'rbtnPrimaryIp
        '
        Me.rbtnPrimaryIp.Location = New System.Drawing.Point(36, 87)
        Me.rbtnPrimaryIp.Name = "rbtnPrimaryIp"
        Me.rbtnPrimaryIp.Size = New System.Drawing.Size(30, 20)
        Me.rbtnPrimaryIp.TabIndex = 3
        '
        'lblActiveIP
        '
        Me.lblActiveIP.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblActiveIP.Location = New System.Drawing.Point(7, 23)
        Me.lblActiveIP.Name = "lblActiveIP"
        Me.lblActiveIP.Size = New System.Drawing.Size(216, 23)
        Me.lblActiveIP.Text = "Set the Address of the Controller"
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 40
        '
        'frmIPConfigure
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "frmIPConfigure"
        Me.Text = "Airbeam IP Configure"
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents lblActiveIP As System.Windows.Forms.Label
    Friend WithEvents rbtnSecondaryIp As System.Windows.Forms.RadioButton
    Friend WithEvents rbtnPrimaryIp As System.Windows.Forms.RadioButton
    Friend WithEvents lblSecondary As System.Windows.Forms.Label
    Friend WithEvents lblPrimary As System.Windows.Forms.Label
    Friend WithEvents lblSecondaryIp As System.Windows.Forms.Label
    Friend WithEvents lblPrimaryIP As System.Windows.Forms.Label
    Friend WithEvents pbSaveBtn As System.Windows.Forms.PictureBox
    Friend WithEvents pbQuitBtn As System.Windows.Forms.PictureBox
    Public WithEvents objStatusBar As McUtilities.CustomStatusBar

End Class
