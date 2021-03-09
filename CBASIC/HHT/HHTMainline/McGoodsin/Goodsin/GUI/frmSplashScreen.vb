''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Application version looked up from app configuration file
''' </Summary>
'''****************************************************************************
Public Class frmSplashScreen
    Inherits System.Windows.Forms.Form
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblRelease As System.Windows.Forms.Label
    Friend WithEvents lblPort As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents lbIPAddress As System.Windows.Forms.Label
    Friend WithEvents lblStoreNo As System.Windows.Forms.Label
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblVersion As System.Windows.Forms.Label
    Friend WithEvents lblMode As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label

    Public Event Event_Kill_Splash(ByVal Sender As System.Object, ByVal e As System.EventArgs)
#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

#If RF Then
  SetIPAddress()
#End If


        'Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
#If NRF Then
    Friend WithEvents tmrDownloader As System.Windows.Forms.Timer
#End If

    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSplashScreen))
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblRelease = New System.Windows.Forms.Label
        Me.lblPort = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.lbIPAddress = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.lblStoreNo = New System.Windows.Forms.Label
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.lblVersion = New System.Windows.Forms.Label
        Me.lblMode = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.Color.White
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.Label1.Location = New System.Drawing.Point(0, 242)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(240, 22)
        Me.Label1.Text = "Please wait while the application loads..."
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblRelease
        '
        Me.lblRelease.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Regular)
        Me.lblRelease.ForeColor = System.Drawing.Color.MediumBlue
        Me.lblRelease.Location = New System.Drawing.Point(135, 280)
        Me.lblRelease.Name = "lblRelease"
        Me.lblRelease.Size = New System.Drawing.Size(64, 10)
        Me.lblRelease.Text = "RCL v6.00"
        '
        'lblPort
        '
        Me.lblPort.BackColor = System.Drawing.Color.White
        Me.lblPort.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblPort.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lblPort.Location = New System.Drawing.Point(172, 265)
        Me.lblPort.Name = "lblPort"
        Me.lblPort.Size = New System.Drawing.Size(25, 13)
        Me.lblPort.Text = "800"
        Me.lblPort.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label7
        '
        Me.Label7.BackColor = System.Drawing.Color.White
        Me.Label7.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label7.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.Label7.Location = New System.Drawing.Point(135, 265)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(35, 15)
        Me.Label7.Text = "Port :"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lbIPAddress
        '
        Me.lbIPAddress.BackColor = System.Drawing.Color.White
        Me.lbIPAddress.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Bold)
        Me.lbIPAddress.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lbIPAddress.Location = New System.Drawing.Point(29, 265)
        Me.lbIPAddress.Name = "lbIPAddress"
        Me.lbIPAddress.Size = New System.Drawing.Size(100, 11)
        Me.lbIPAddress.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.Color.White
        Me.Label3.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Bold)
        Me.Label3.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.Label3.Location = New System.Drawing.Point(1, 265)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(28, 11)
        Me.Label3.Text = "IP :"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblStoreNo
        '
        Me.lblStoreNo.BackColor = System.Drawing.Color.White
        Me.lblStoreNo.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblStoreNo.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lblStoreNo.Location = New System.Drawing.Point(10, 7)
        Me.lblStoreNo.Name = "lblStoreNo"
        Me.lblStoreNo.Size = New System.Drawing.Size(40, 15)
        Me.lblStoreNo.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(6, 4)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(229, 229)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblVersion
        '
        Me.lblVersion.BackColor = System.Drawing.Color.White
        Me.lblVersion.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblVersion.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lblVersion.Location = New System.Drawing.Point(175, 7)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(57, 15)
        Me.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblMode
        '
        Me.lblMode.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Regular)
        Me.lblMode.ForeColor = System.Drawing.Color.MediumBlue
        Me.lblMode.Location = New System.Drawing.Point(44, 279)
        Me.lblMode.Name = "lblMode"
        Me.lblMode.Size = New System.Drawing.Size(30, 10)
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Regular)
        Me.Label4.ForeColor = System.Drawing.Color.MediumBlue
        Me.Label4.Location = New System.Drawing.Point(4, 279)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(34, 10)
        Me.Label4.Text = "MODE:"
        '
        'frmSplashScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.lblRelease)
        Me.Controls.Add(Me.lblPort)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.lbIPAddress)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblMode)
        Me.Controls.Add(Me.lblVersion)
        Me.Controls.Add(Me.lblStoreNo)
        Me.Controls.Add(Me.PictureBox1)
        Me.MaximizeBox = False
        Me.Name = "frmSplashScreen"
        Me.Text = "Boots Store Application"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub frmSplash_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            AddHandler Event_Kill_Splash, AddressOf CloseSplash


            'UAT - Change in splash screen
            Dim aReleaseVersion() As String = Nothing
            Dim strAppVersion As String = Nothing
            lblStoreNo.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.STORE_NO)
            'lblVersion.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION)
            strAppVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
            aReleaseVersion = strAppVersion.Split("-")
            lblVersion.Text = aReleaseVersion(0)
            'lbIPAddress.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.SERVER_IPADDRESS)
            'v1.1 MCF Change
            lbIPAddress.Text = objAppContainer.strActiveIP
            lblRelease.Text = objAppContainer.objHelper.GetReleaseVersion()
            lblPort.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.IPPORT)
#If RF Then
            lblMode.Text = "RF"
#ElseIf NRF Then
            lblMode.Text = "Batch"
#End If

            ' Show Port connection info on the Splash Screen
            'ShowSplashMessage()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Splashscreen:Exception" _
                                                     & ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try

    End Sub
#If RF Then
 Public Sub SetIPAddress()
        lblVersion.Text = ConfigDataMgr.GetInstance().GetParam(ConfigKey.APP_VERSION).ToString()
        'Info.Text = "Addr: " + ConfigDataMgr.GetInstance().GetParam(ConfigKey.SERVER_IPADDRESS).ToString() _
        '+ ", Port " + ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT).ToString()
    End Sub
    Public Sub ShowSplashMessage(ByVal sMsg As String)
        'Me.labMessage.Text = sMsg.ToString()


    End Sub
#End If




    Public Sub CloseSplash(ByVal Sender As System.Object, ByVal e As System.EventArgs)
        Close()
    End Sub
    Private Const strText As String = "Please wait while the application loads..."
    Public Delegate Sub ChangeLabelTextCallback(ByVal strLabelText As String)
    Public Sub ChangeLabelText(Optional ByVal strLabelText As String = strText)
        If Not Me.InvokeRequired Then
            'tmrDownloader.Interval = 1000
            'tmrDownloader.Enabled = True
            Me.Visible = True
            Label1.Text = ""
            Label1.Text = strLabelText
            lbIPAddress.Text = objAppContainer.strActiveIP
        Else
            Me.Invoke(New ChangeLabelTextCallback(AddressOf ChangeLabelText), strLabelText)
        End If
    End Sub
#Region "NRF"
#If NRF Then
    Private Sub tmrDownloader_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrDownloader.Tick
        If (tmrDownloader.Interval > 0) Then
            tmrDownloader.Interval = tmrDownloader.Interval - 1
        End If
        tmrDownloader.Interval = 1000
    End Sub
#End If
#End Region


End Class
