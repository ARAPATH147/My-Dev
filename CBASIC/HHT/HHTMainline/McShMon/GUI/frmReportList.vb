#If RF Then
Public Class frmReportList
    Private Sub btn_Quit_small_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Quit_small.Click
        Try
            FreezeControl()
            objAppContainer.objLogger.WriteAppLog("Quit button click", Logger.LogLevel.INFO)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            Try
                'UAT DEFECT FIX - Item sales and reports - do not need the 'are you sure you wish to quit' message 
                'If (MessageBox.Show(MessageManager.GetInstance().GetMessage("M39"), _
                '               "Confirmation", _
                '               MessageBoxButtons.YesNo, _
                '               MessageBoxIcon.Question, _
                '               MessageBoxDefaultButton.Button1) = (MsgBoxResult.Yes)) Then
                If Not ReportsSessionManager.GetInstance().EndSession() Then
                    UnFreezeControl()
                End If
                'Else
                'UnFreezeControl()
                'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                'End If
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("Exception occured at Quit button click: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            End Try
            objAppContainer.objLogger.WriteAppLog("Exit Quit button Session", Logger.LogLevel.INFO)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControl()
#End If
        End Try
    End Sub
    Private Sub FreezeControl()
        Try
            lstReports.Enabled = False
            btn_Info_button_i.Enabled = False
            btn_Quit_small.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub UnFreezeControl()
        Try
            lstReports.Enabled = True
            btn_Info_button_i.Enabled = True
            btn_Quit_small.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub btn_Info_button_i_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Info_button_i.Click
        Try
            FreezeControl()
            objAppContainer.objLogger.WriteAppLog("Info button click", Logger.LogLevel.INFO)
            Try
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
                ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.REPORTS)
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                UnFreezeControl()
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("Exception in Info Button Click of Reports", Logger.LogLevel.RELEASE)
            End Try
            objAppContainer.objLogger.WriteAppLog("Exits Info Button Click", Logger.LogLevel.INFO)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControl()
#End If
        End Try
    End Sub

    Private Sub lstReports_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstReports.SelectedIndexChanged
        Try
            FreezeControl()
            'Dim strSelectedReport As String
            Dim iSelectedIndex As Integer
            objAppContainer.objLogger.WriteAppLog("On Selecting List", Logger.LogLevel.INFO)

            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            'DEFECT FIX - BTCPR00004149(RF Mode :: Reports :: Application 
            'hangs when tapped on empty space in reports module)
            If lstReports.SelectedIndices.Count <= 0 Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                UnFreezeControl()
                Return
            End If
            iSelectedIndex = lstReports.SelectedIndices(0)
            'If iSelectedIndex >= 0 Then
            '    strSelectedReport = lstReports.Items(iSelectedIndex).Text
            'If (ReportsSessionManager.GetInstance().Details(iSelectedIndex)) Then
            '    'Me.lstReports.Clear()
            '    'Me.Visible = False
            'End If
            'End If
            ReportsSessionManager.GetInstance().Details(iSelectedIndex)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            UnFreezeControl()
            objAppContainer.objLogger.WriteAppLog("Exit Selecting List", Logger.LogLevel.INFO)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in  Reports", Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControl()
#End If
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Selecting List", Logger.LogLevel.INFO)
    End Sub
End Class
#End If