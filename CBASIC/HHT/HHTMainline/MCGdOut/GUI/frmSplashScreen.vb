''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Refers application config file for displaying release version
''' </Summary>
'''****************************************************************************
Public Class frmSplashScreen
    Private Const strText As String = "Please wait while the application loads..."
    Public Delegate Sub ChangeLabelTextCallback(ByVal strLabelText As String)
    Public Event Event_Kill_Splash(ByVal Sender As System.Object, ByVal e As System.EventArgs)
    Public Sub ChangeLabelText(Optional ByVal strLabelText As String = strText)
        If Not Me.InvokeRequired Then
            tmrDownloader.Interval = 1000
            tmrDownloader.Enabled = True
            Me.Visible = True
            labMessage.Text = ""
            labMessage.Text = strLabelText
            'v1.1 MCF Change
            lbIPAddress.Text = objAppContainer.strActiveIP
        Else
            Me.Invoke(New ChangeLabelTextCallback(AddressOf ChangeLabelText), strLabelText)
        End If
    End Sub
    Private Sub frmSplashScreen_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'UAT - Change in splash screen
            Dim aReleaseVersion() As String = Nothing
            Dim strAppVersion As String = Nothing
            lblStoreNo.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.STORE_NO)
            'lblVersion.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION)
            strAppVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
            aReleaseVersion = strAppVersion.Split("-")
            lblVersion.Text = aReleaseVersion(0)
            'v1.1 MCF Change
            'lbIPAddress.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.SERVER_IPADDRESS)
            lbIPAddress.Text = objAppContainer.strActiveIP
            lblProdVer.Text = objAppContainer.objHelper.GetReleaseVersion()
            lblPort.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.IPPORT)
#If RF Then
            lblMode.Text = "RF"
#ElseIf NRF Then
            lblMode.Text = "Batch"
#End If
            AddHandler Event_Kill_Splash, AddressOf KillSplash

        Catch ex As Exception
            MessageBox.Show("Error in Loading splash screen", "Exception")
        End Try
    End Sub
    Public Sub KillSplash(ByVal Sender As System.Object, ByVal e As System.EventArgs)
        Close()
    End Sub

    Private Sub tmrDownloader_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrDownloader.Tick
        If (tmrDownloader.Interval > 0) Then
            tmrDownloader.Interval = tmrDownloader.Interval - 1
        End If
        tmrDownloader.Interval = 1000
    End Sub
End Class