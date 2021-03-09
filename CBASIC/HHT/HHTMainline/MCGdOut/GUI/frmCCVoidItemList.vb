Public Class frmCCVoidItemList

    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        Try
            FreezeControls()
            If lvItemList.Items.Count < 1 Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M56"), _
                "Caution", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                 MessageBoxDefaultButton.Button1)
            Else
                If MessageBox.Show(MessageManager.GetInstance().GetMessage("M4"), _
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                 MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                    'Generate an the export data
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing Export data...")
#If NRF Then
                 CCSessionMgr.GetInstance().GenerateExportData()
                CCSessionMgr.GetInstance().DisplayCCScreen(CCSessionMgr.CCSCREENS.Summary)
#ElseIf RF Then
                    If CCSessionMgr.GetInstance().GenerateExportData() Then
                        CCSessionMgr.GetInstance().DisplayCCScreen(CCSessionMgr.CCSCREENS.Summary)
                    End If
#End If
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                End If
            End If
#If NRF Then
  UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub

    Private Sub btnVoidItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnVoidItem.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Voiding item from the list")
            CCSessionMgr.GetInstance().VoidItemInfo()
            Me.lblTotalData.Text = CCSessionMgr.GetInstance().GetItemCount().ToString
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            'DEFECT FIX - BTCPR00004646 PPC - Goods Out - Returns & Destroy - Faulty - if all items are voided from Items in UOD summary screen return to item scan
            If CCSessionMgr.GetInstance().GetItemCount() = 0 Then
                CCSessionMgr.GetInstance().ClearData()
                CCSessionMgr.GetInstance().DisplayCCScreen(CCSessionMgr.CCSCREENS.Scan)
            End If
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
        Try
            FreezeControls()
            CCSessionMgr.GetInstance().ClearData()
            CCSessionMgr.GetInstance().DisplayCCScreen(CCSessionMgr.CCSCREENS.Scan)
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

    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.btnQuit.Enabled = False
            Me.btnNext.Enabled = False
            Me.btnVoidItem.Enabled = False
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
            Me.btnNext.Enabled = True
            Me.btnVoidItem.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Unfreeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class