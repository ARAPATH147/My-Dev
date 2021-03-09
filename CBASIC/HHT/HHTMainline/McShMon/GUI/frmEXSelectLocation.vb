Public Class frmEXSelectLocation

    'Private Sub Btn_StockRoom1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    EXSessionMgr.GetInstance().LocationChosen(EXSessionMgr.LocationType.StockRoom)
    'End Sub

    Private Sub btn_OSSR_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OSSR.Click
        Try
            EXSessionMgr.GetInstance().LocationChosen(EXSessionMgr.LocationType.OSSRSite)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub Btn_Quit1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit1.Click
        Try
            'Naveen Fix: All Operations to be within TRY-CATCH Loop
            'ambli
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            objAppContainer.objLogger.WriteAppLog("Quit button click", Logger.LogLevel.INFO)
            If (MessageBox.Show("Are you sure you wish to quit?", _
                           "Confirmation", _
                           MessageBoxButtons.YesNo, _
                           MessageBoxIcon.Question, _
                           MessageBoxDefaultButton.Button1) = (MsgBoxResult.Yes)) Then
                'No Unfreeze needed because no freeze called
                EXSessionMgr.GetInstance().EndSession()
            End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at Quit button click: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
    End Sub

    Private Sub btnBackShop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBackShop.Click
        Try
            EXSessionMgr.GetInstance().LocationChosen(EXSessionMgr.LocationType.StockRoom)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class