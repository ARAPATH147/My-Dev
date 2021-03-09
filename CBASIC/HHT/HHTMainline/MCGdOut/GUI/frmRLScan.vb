Public Class frmRLScan

    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
#If RF Then
            objAppContainer.bRecallConnection = False
#End If
            'Check if all items in COMPANY HO recall is actioned or else don't allow the user to exit.
            'If (WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.CUSTOMEREMERGENCY Or _
            '    WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.WITHDRAWN) Then
            'For Change 4988 100% recalls to be non- flexible
            If (WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.CUSTOMEREMERGENCY Or _
                WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.WITHDRAWN Or _
                WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.RECALLRETURNS) Then
                If RLSessionMgr.GetInstance().m_ActionedItemsInRecall > 0 And _
                   RLSessionMgr.GetInstance().m_ActionedItemsInRecall < RLSessionMgr.GetInstance().m_TotalItemsInRecall Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M71"), _
                                    "Alert", MessageBoxButtons.OK, MessageBoxIcon.Question, _
                                    MessageBoxDefaultButton.Button1)
                    Return
                ElseIf RLSessionMgr.GetInstance().m_ActionedItemsInRecall = RLSessionMgr.GetInstance().m_TotalItemsInRecall Then
                    ' If Not (RLSessionMgr.GetInstance().bRecallActionCompleted And RLSessionMgr.GetInstance().bCreateAnotherRecallUOD) Then
                    If RLSessionMgr.GetInstance().GetActionedItemCount = 0 Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M71"), _
                                   "Alert", MessageBoxButtons.OK, MessageBoxIcon.Question, _
                                   MessageBoxDefaultButton.Button1)
                        Return
                    Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M72"), _
                             "Alert", MessageBoxButtons.OK, MessageBoxIcon.Question, _
                             MessageBoxDefaultButton.Button1)
                        Return
                    End If


                    'End If

                End If
            End If
            '#End If
            'MessageBox.Show(MessageManager.GetInstance().GetMessage("M38"), _
            '"Caution", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
            'MessageBoxDefaultButton.Button1)
            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M3"), _
                "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                RLSessionMgr.GetInstance().bCreateAnotherRecallUOD = False
                RLSessionMgr.GetInstance().bRecallActionCompleted = False
                'RLSessionMgr.GetInstance().EndSession()
                'WorkflowMgr.GetInstance().ExecQuit()
                RLSessionMgr.GetInstance().bIsRecallReturns = False
#If RF Then
                'Send recall exit message and return to active recall list screen.
                RLSessionMgr.GetInstance().SendRecallExitForQuit()
#End If
                RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.ActiveRecallList)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub btnView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnView.Click
        Try
            FreezeControls()
            'Make the form disabled
            'BCReader.GetInstance().StopRead()
            'BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
            'BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.CODE128)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Loading Recall Item List...")
            RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.Scan
            RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.ItemList)
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

    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        Try
            FreezeControls()
            RLSessionMgr.GetInstance().bCreateAnotherRecallUOD = False
            'Check if all items in COMPANY HO recall is actioned or else don't allow the user to exit.
            'If (WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.CUSTOMEREMERGENCY Or _
            '    WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.WITHDRAWN) Then
            'For Change 4988 100% recalls to be non- flexible
            If (WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.CUSTOMEREMERGENCY Or _
                WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.WITHDRAWN Or _
                WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.RECALLRETURNS) Then
                If RLSessionMgr.GetInstance().m_ActionedItemsInRecall = RLSessionMgr.GetInstance().m_TotalItemsInRecall Then
                    RLSessionMgr.GetInstance().bRecallActionCompleted = True
                Else
                    RLSessionMgr.GetInstance().bRecallActionCompleted = False
                End If


                'If RLSessionMgr.GetInstance().m_ActionedItemsInRecall > 0 And _
                '   RLSessionMgr.GetInstance().m_ActionedItemsInRecall < RLSessionMgr.GetInstance().m_TotalItemsInRecall Then
                '    MessageBox.Show(MessageManager.GetInstance().GetMessage("M71"), _
                '                    "Alert", MessageBoxButtons.OK, MessageBoxIcon.Question, _
                '                    MessageBoxDefaultButton.Button1)
                '    Return
                'End If
            End If
            '  RLSessionMgr.GetInstance().bIsRecallReturns = False
            'If RLSessionMgr.GetInstance().m_ActionedItemsInRecall > 0 And _
            '      RLSessionMgr.GetInstance().m_ActionedItemsInRecall < RLSessionMgr.GetInstance().m_TotalItemsInRecall Then



            Dim iResult As DialogResult = MessageBox.Show("If you require another UOD to complete " + _
                                                          "the recall please Select YES now.", "New UOD", _
                                                          MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                                                          MessageBoxDefaultButton.Button1)

            If iResult = Windows.Forms.DialogResult.No Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Closing Active Recall List")
                If RLSessionMgr.GetInstance().IsDiscrepancy() Then
                    RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.Scan
                    RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Discrepancy)
                Else
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Loading UOD Screen")
                    RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.Scan
                    RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.ScanUOD)
                End If
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            ElseIf iResult = Windows.Forms.DialogResult.Yes Then
                'DEFECT FIX for 4990
                '  RLSessionMgr.GetInstance().SetActionsItemsInPrevUODs()
                RLSessionMgr.GetInstance().bCreateAnotherRecallUOD = True
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Loading UOD Screen")
                RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.Scan
                RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.ScanUOD)
            End If
            'Else
            '    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Closing Active Recall List")
            '    If RLSessionMgr.GetInstance().IsDiscrepancy() Then
            '        RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.Scan
            '        RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Discrepancy)
            '    Else
            '        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Loading UOD Screen")
            '        RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.Scan
            '        RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.ScanUOD)
            '    End If
            '    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            'End If
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

    Private Sub frmRLScan_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
            BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.CODE128)
            BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.EAN128)
            BCReader.GetInstance.StartRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub frmRLScan_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        Try
            'Close the Scan Reader
            BCReader.GetInstance.StopRead()
            BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
            BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.CODE128)
            BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.EAN128)
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
            Me.btnReturn.Enabled = False
            Me.btnView.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ freeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' UnFreeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnFreezeControls()
        Try
            Me.btnQuit.Enabled = True
            Me.btnReturn.Enabled = True
            Me.btnView.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Unfreeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class