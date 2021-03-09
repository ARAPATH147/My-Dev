''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Fixed defect #26 
''' </Summary>
'''****************************************************************************
Public Class frmUtilitiesMenu
#If NRF Then
    Private Sub pbIPMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbIPMenu.Click
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
        IPInfoMgr.GetInstance().StartSession()
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
    End Sub
    Private Sub pbMemStatusMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbMemStatusMenu.Click
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
        MemStatusMgr.GetInstance().StartSession()
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
    End Sub
    Private Sub pbViewLogMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbViewLogMenu.Click
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
        LogFileMgr.GetInstance().StartSession()
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
    End Sub
    Private Sub pbReloadRefMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbReloadRefMenu.Click
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
        ReferenceDataMgr.GetInstance.StartDownloadsession()
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
    End Sub
    Private Sub pbViewFileStatMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbViewFileStatMenu.Click
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
        FileStatusMgr.GetInstance.StartSession()
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
    End Sub
    Private Sub pbViewExpDataMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbViewExpDataMenu.Click
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
        ExportDataViewer.getinstance.startsession()
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
    End Sub
    Private Sub tbpgFileSysInfo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbpgFileSysInfo.Click
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
    End Sub
#End If
    Private Sub pbLogOff_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbLogOff.Click
        If MessageBox.Show("Do you really want to quit?", _
                                   "Alert", _
                                   MessageBoxButtons.YesNo, _
                                   MessageBoxIcon.Question, _
                                   MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
            Me.Dispose()
        End If
        'Check for the user select
    End Sub
    Private Sub pbAirbeamIPConfig_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbAirbeamIPConfig.Click
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
        AirbeamIPConfig.GetInstance().StartSession()
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
    End Sub
    Sub New()
        InitializeComponent()
#If RF Then
        pbIPMenu.Visible = False
        lblIPInfoMenu.Visible = False
        pbMemStatusMenu.Visible = False
        lblMemStatusMenu.Visible = False
        tbShlfMgmtMenu.Controls.Remove(tbpgFileSysInfo)
        pbAirbeamIPConfig.Visible = True
        'To make the coordinates matching for PPC.
        'v1.1 MCF Defect # 26
        If objAppContainer.iOffset = 2 Then
            pbAirbeamIPConfig.Location = New Point(50, 21)
            lblIPConfg.Location = New Point(13, 120)
        Else
            pbAirbeamIPConfig.Location = New Point(31, 21)
            lblIPConfg.Location = New Point(13, 69)
        End If
        tbShlfMgmtMenu.SelectedIndex = 0 'To make the SysInfo tab default
#End If
    End Sub
End Class