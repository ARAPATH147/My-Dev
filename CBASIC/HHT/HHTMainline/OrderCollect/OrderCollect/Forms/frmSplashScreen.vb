Public Class frmSplashScreen

    Private Const MESSAGE_TEXT As String = "Please wait while the application loads..."
    Public Delegate Sub ChangeLabelTextCallback(ByVal strLabelText As String)

    Public Sub SetScreen()
        Dim aReleaseVersion() As String = Nothing
        Dim strAppVersion As String = Nothing

        lblStoreNo.Text = ConfigFileManager.GetInstance.GetParam(ConfigKey.STORE_NO)
        lblVersion.Text = ConfigFileManager.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
        strAppVersion = ConfigFileManager.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
        aReleaseVersion = strAppVersion.Split("-")
        lblVersion.Text = aReleaseVersion(0)
        lbIPAddress.Text = oAppMain.cActiveIP
        lblRelease.Text = ConfigFileManager.GetInstance.GetReleaseVersion()
        lblPort.Text = ConfigFileManager.GetInstance.GetParam(ConfigKey.IPPORT)
        lblMode.Text = ConfigFileManager.GetInstance.GetParam(ConfigKey.DEVICE_TYPE)
    End Sub

    Public Sub ChangeLabelText(Optional ByVal strLabelText As String = MESSAGE_TEXT)
        If Not Me.InvokeRequired Then
            'tmrDownloader.Interval = 1000
            'tmrDownloader.Enabled = True
            Me.Visible = True
            Label1.Text = ""
            Label1.Text = strLabelText
            lbIPAddress.Text = oAppMain.cActiveIP
        Else
            Me.Invoke(New ChangeLabelTextCallback(AddressOf ChangeLabelText), strLabelText)
        End If
    End Sub
End Class