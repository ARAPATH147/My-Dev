Public Class frmRLDespatch

    Private Sub btnDespatch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDespatch.Click
        Try
            FreezeControls()
            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M5"), _
            "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, _
            MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.OK Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing Export data...")
                'Display the pop up message in case of excess sales plan and planner leaver recalls
                'to inform the user about any items pendings and the last date for recalls to be completed.
                If (((WorkflowMgr.GetInstance.objActiveFeature = WorkflowMgr.ACTIVEFEATURE.PLANNERLEAVER)) Or _
                     ((WorkflowMgr.GetInstance.objActiveFeature = WorkflowMgr.ACTIVEFEATURE.EXCESSSALESPLAN))) Then
                    RLSessionMgr.GetInstance().DisplaySummaryPopUp()
                End If
#If NRF Then
                RLSessionMgr.GetInstance().GenerateExportData()
                RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.Despatch
                RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Summary)
#ElseIf RF Then
                If RLSessionMgr.GetInstance().GenerateExportData() Then
                    RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.Despatch
                    RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Summary)
                End If
#End If
            End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#If NRF Then
            UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
        'RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.Despatch
        'RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Scan)
        'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

        'MessageBox.Show(MessageManager.GetInstance().GetMessage("M38"), _
        '"Caution", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
        'MessageBoxDefaultButton.Button1)
        'Check if all items in COMPANY HO recall is actioned or else don't allow the user to exit.
        'If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.COMPANYHORECALL Then
        '    If RLSessionMgr.GetInstance().m_ActionedItemsInRecall > 0 Then
        '        MessageBox.Show(MessageManager.GetInstance().GetMessage("M71"), _
        '                        "Alert", MessageBoxButtons.OK, MessageBoxIcon.Question, _
        '                        MessageBoxDefaultButton.Button1)
        '        Return
        '    End If
        'End If
        Try
            'If MessageBox.Show(MessageManager.GetInstance().GetMessage("M3"), _
            '            "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
            '            MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then

            '    RLSessionMgr.GetInstance().EndSession()
            '    WorkflowMgr.GetInstance().ExecQuit()

            'End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)

            'FIX FOR DEFCT 4990
            '    RLSessionMgr.GetInstance().RemoveActionsItemsInPrevUODs()

            RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.ItemList
            RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Scan)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.btnQuit.Enabled = False
            Me.btnDespatch.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Freeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' UnFreeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnFreezeControls()
        Try
            Me.btnQuit.Enabled = True
            Me.btnDespatch.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Unfreeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class