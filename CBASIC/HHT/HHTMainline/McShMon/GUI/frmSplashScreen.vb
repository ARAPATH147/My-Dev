''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes for connecting to alternate controller while primary is down.
''' </Summary>
'''****************************************************************************

Public Class frmSplashScreen
    Private Const strText As String = "Please wait while the application loads..."
    Public Delegate Sub ChangeLabelTextCallback(ByVal strLabelText As String)
    Public Sub CloseSplash(ByVal Sender As System.Object, ByVal e As System.EventArgs)
        Me.Close()
    End Sub
    Public Sub ChangeLabelText(Optional ByVal strLabelText As String = strText)
        If Not Me.InvokeRequired Then
            tmrDownloader.Interval = 1000
            tmrDownloader.Enabled = True
            Me.Visible = True
            Label1.Text = ""
            Label1.Text = strLabelText
            lbIPAddress.Text = objAppContainer.strActiveIP 'v1.1 MCF Change to view changed IP
        Else
            Me.Invoke(New ChangeLabelTextCallback(AddressOf ChangeLabelText), strLabelText)
        End If
    End Sub
    Private Sub tmrDownloader_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrDownloader.Tick
        If (tmrDownloader.Interval > 0) Then
            tmrDownloader.Interval = tmrDownloader.Interval - 1
        End If
        tmrDownloader.Interval = 1000
    End Sub

    Private Sub frmSplashScreen_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim aReleaseVersion() As String = Nothing
        Dim strAppVersion As String = Nothing
        lblStoreNo.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.STORE_NO)
        strAppVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
        aReleaseVersion = strAppVersion.Split("-")
        lblVersion.Text = aReleaseVersion(0)
        lblRelease.Text = objAppContainer.objHelper.GetReleaseVersion()
        lbIPAddress.Text = objAppContainer.strActiveIP 'v1.1 MCF Change
        lblPort.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.IPPORT)
#If RF Then
        lblMode.Text = "RF"
#ElseIf NRF Then
        lblMode.Text = "Batch"
#End If
    End Sub
End Class