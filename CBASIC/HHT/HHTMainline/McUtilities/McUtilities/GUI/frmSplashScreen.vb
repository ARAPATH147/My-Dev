Public Class frmSplashScreen

    Private Sub frmSplashScreen_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'UAT - Change in splash screen
        Dim aReleaseVersion() As String = Nothing
        Dim strAppVersion As String = Nothing
        lblStoreNo.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.STORE_NO)
        'lblVersion.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION)
        strAppVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
        aReleaseVersion = strAppVersion.Split("-")
        lblVersion.Text = aReleaseVersion(0)
        lbIPAddress.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.SERVER_IPADDRESS)
        lblPort.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.IPPORT)
        'AddHandler Event_Kill_Splash, AddressOf KillSplash
    End Sub
    Public Sub CloseSplash(ByVal Sender As System.Object, ByVal e As System.EventArgs)
        Me.Close()
    End Sub
End Class