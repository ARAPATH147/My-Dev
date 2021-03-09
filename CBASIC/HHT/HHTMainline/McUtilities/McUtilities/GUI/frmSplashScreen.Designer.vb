<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSplashScreen
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSplashScreen))
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblVersion = New System.Windows.Forms.Label
        Me.lblProdVer = New System.Windows.Forms.Label
        Me.lblPortNo = New System.Windows.Forms.Label
        Me.lbIPAddress = New System.Windows.Forms.Label
        Me.lblIP = New System.Windows.Forms.Label
        Me.lblPort = New System.Windows.Forms.Label
        Me.lblStoreNo = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(240, 240)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.Color.White
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.Label1.Location = New System.Drawing.Point(0, 239)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(240, 25)
        Me.Label1.Text = "Please wait while the application loads..."
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblVersion
        '
        Me.lblVersion.BackColor = System.Drawing.Color.White
        Me.lblVersion.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lblVersion.Location = New System.Drawing.Point(185, 11)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(40, 15)
        Me.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblProdVer
        '
        Me.lblProdVer.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Regular)
        Me.lblProdVer.ForeColor = System.Drawing.Color.MediumBlue
        Me.lblProdVer.Location = New System.Drawing.Point(201, 283)
        Me.lblProdVer.Name = "lblProdVer"
        Me.lblProdVer.Size = New System.Drawing.Size(37, 10)
        Me.lblProdVer.Text = "v2.05"
        '
        'lblPortNo
        '
        Me.lblPortNo.BackColor = System.Drawing.Color.White
        Me.lblPortNo.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblPortNo.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lblPortNo.Location = New System.Drawing.Point(154, 270)
        Me.lblPortNo.Name = "lblPortNo"
        Me.lblPortNo.Size = New System.Drawing.Size(35, 15)
        Me.lblPortNo.Text = "Port :"
        Me.lblPortNo.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lbIPAddress
        '
        Me.lbIPAddress.BackColor = System.Drawing.Color.White
        Me.lbIPAddress.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lbIPAddress.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lbIPAddress.Location = New System.Drawing.Point(45, 270)
        Me.lbIPAddress.Name = "lbIPAddress"
        Me.lbIPAddress.Size = New System.Drawing.Size(100, 15)
        Me.lbIPAddress.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblIP
        '
        Me.lblIP.BackColor = System.Drawing.Color.White
        Me.lblIP.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblIP.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lblIP.Location = New System.Drawing.Point(20, 270)
        Me.lblIP.Name = "lblIP"
        Me.lblIP.Size = New System.Drawing.Size(25, 15)
        Me.lblIP.Text = "IP :"
        Me.lblIP.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblPort
        '
        Me.lblPort.BackColor = System.Drawing.Color.White
        Me.lblPort.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblPort.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lblPort.Location = New System.Drawing.Point(192, 270)
        Me.lblPort.Name = "lblPort"
        Me.lblPort.Size = New System.Drawing.Size(25, 15)
        Me.lblPort.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblStoreNo
        '
        Me.lblStoreNo.BackColor = System.Drawing.Color.White
        Me.lblStoreNo.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblStoreNo.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lblStoreNo.Location = New System.Drawing.Point(3, 11)
        Me.lblStoreNo.Name = "lblStoreNo"
        Me.lblStoreNo.Size = New System.Drawing.Size(40, 15)
        Me.lblStoreNo.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'frmSplashScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblStoreNo)
        Me.Controls.Add(Me.lblProdVer)
        Me.Controls.Add(Me.lblPortNo)
        Me.Controls.Add(Me.lbIPAddress)
        Me.Controls.Add(Me.lblIP)
        Me.Controls.Add(Me.lblPort)
        Me.Controls.Add(Me.lblVersion)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Regular)
        Me.KeyPreview = True
        Me.Name = "frmSplashScreen"
        Me.Text = "Boots Store Application"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblVersion As System.Windows.Forms.Label
    Friend WithEvents lblProdVer As System.Windows.Forms.Label
    Friend WithEvents lblPortNo As System.Windows.Forms.Label
    Friend WithEvents lbIPAddress As System.Windows.Forms.Label
    Friend WithEvents lblIP As System.Windows.Forms.Label
    Friend WithEvents lblPort As System.Windows.Forms.Label
    Friend WithEvents lblStoreNo As System.Windows.Forms.Label
End Class
