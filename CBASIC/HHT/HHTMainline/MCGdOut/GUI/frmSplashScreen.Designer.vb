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
        Me.pbBackground = New System.Windows.Forms.PictureBox
        Me.tmrDownloader = New System.Windows.Forms.Timer
        Me.lblVersion = New System.Windows.Forms.Label
        Me.labMessage = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.lbIPAddress = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.lblPort = New System.Windows.Forms.Label
        Me.lblStoreNo = New System.Windows.Forms.Label
        Me.lblProdVer = New System.Windows.Forms.Label
        Me.lblMode = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'pbBackground
        '
        Me.pbBackground.Image = CType(resources.GetObject("pbBackground.Image"), System.Drawing.Image)
        Me.pbBackground.Location = New System.Drawing.Point(5, 4)
        Me.pbBackground.Name = "pbBackground"
        Me.pbBackground.Size = New System.Drawing.Size(229, 229)
        Me.pbBackground.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'tmrDownloader
        '
        '
        'lblVersion
        '
        Me.lblVersion.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lblVersion.Location = New System.Drawing.Point(190, 8)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(40, 15)
        Me.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'labMessage
        '
        Me.labMessage.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.labMessage.ForeColor = System.Drawing.Color.Navy
        Me.labMessage.Location = New System.Drawing.Point(0, 239)
        Me.labMessage.Name = "labMessage"
        Me.labMessage.Size = New System.Drawing.Size(240, 25)
        Me.labMessage.Text = "Please wait while the application loads..."
        Me.labMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label7
        '
        Me.Label7.BackColor = System.Drawing.Color.White
        Me.Label7.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label7.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.Label7.Location = New System.Drawing.Point(142, 262)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(35, 15)
        Me.Label7.Text = "Port :"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lbIPAddress
        '
        Me.lbIPAddress.BackColor = System.Drawing.Color.White
        Me.lbIPAddress.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lbIPAddress.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lbIPAddress.Location = New System.Drawing.Point(33, 262)
        Me.lbIPAddress.Name = "lbIPAddress"
        Me.lbIPAddress.Size = New System.Drawing.Size(100, 15)
        Me.lbIPAddress.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.Color.White
        Me.Label2.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.Label2.Location = New System.Drawing.Point(8, 262)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(25, 15)
        Me.Label2.Text = "IP :"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblPort
        '
        Me.lblPort.BackColor = System.Drawing.Color.White
        Me.lblPort.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblPort.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lblPort.Location = New System.Drawing.Point(180, 262)
        Me.lblPort.Name = "lblPort"
        Me.lblPort.Size = New System.Drawing.Size(25, 15)
        Me.lblPort.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblStoreNo
        '
        Me.lblStoreNo.BackColor = System.Drawing.Color.White
        Me.lblStoreNo.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblStoreNo.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lblStoreNo.Location = New System.Drawing.Point(9, 8)
        Me.lblStoreNo.Name = "lblStoreNo"
        Me.lblStoreNo.Size = New System.Drawing.Size(40, 15)
        Me.lblStoreNo.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblProdVer
        '
        Me.lblProdVer.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Regular)
        Me.lblProdVer.ForeColor = System.Drawing.Color.MediumBlue
        Me.lblProdVer.Location = New System.Drawing.Point(175, 282)
        Me.lblProdVer.Name = "lblProdVer"
        Me.lblProdVer.Size = New System.Drawing.Size(62, 10)
        Me.lblProdVer.Text = "RCL v6.05"
        '
        'lblMode
        '
        Me.lblMode.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Regular)
        Me.lblMode.ForeColor = System.Drawing.Color.MediumBlue
        Me.lblMode.Location = New System.Drawing.Point(49, 278)
        Me.lblMode.Name = "lblMode"
        Me.lblMode.Size = New System.Drawing.Size(34, 13)
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Regular)
        Me.Label4.ForeColor = System.Drawing.Color.MediumBlue
        Me.Label4.Location = New System.Drawing.Point(9, 278)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(34, 13)
        Me.Label4.Text = "MODE:"
        '
        'frmSplashScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblMode)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.lblProdVer)
        Me.Controls.Add(Me.lblStoreNo)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.lbIPAddress)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lblPort)
        Me.Controls.Add(Me.lblVersion)
        Me.Controls.Add(Me.labMessage)
        Me.Controls.Add(Me.pbBackground)
        Me.Name = "frmSplashScreen"
        Me.Text = "Boots Store Application"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pbBackground As System.Windows.Forms.PictureBox
    Friend WithEvents tmrDownloader As System.Windows.Forms.Timer
    Friend WithEvents lblVersion As System.Windows.Forms.Label
    Friend WithEvents labMessage As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents lbIPAddress As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblPort As System.Windows.Forms.Label
    Friend WithEvents lblStoreNo As System.Windows.Forms.Label
    Friend WithEvents lblProdVer As System.Windows.Forms.Label
    Friend WithEvents lblMode As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
End Class
