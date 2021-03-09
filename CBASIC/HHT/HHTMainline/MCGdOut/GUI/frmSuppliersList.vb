Public Class frmSuppliersList


    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            FreezeControls()
            If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.DESTROY Then
                GOSessionMgr.GetInstance().SetAuthorizationID(Nothing)
            End If
            GOSessionMgr.GetInstance().ClearData()
            GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.Scan)
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

    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        Try
            FreezeControls()
            If lvSuppliersList.SelectedIndices.Count > 0 Then

                GOSessionMgr.GetInstance().SetSupplierName()
                'Fix for Defect 4095(Direct Returns/Semi Centralised Returns-do not offer the scan UOD label option)
                If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.DESTROY Then
                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.GODespatch)
                Else
                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.ItemView)
                End If
            Else
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M39"), _
                "Caution", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                MessageBoxDefaultButton.Button1)
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

    Private Sub btnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBack.Click
        Try
            FreezeControls()
            If WorkflowMgr.GetInstance.objActiveFeature = WorkflowMgr.ACTIVEFEATURE.DESTROY Then
                GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.ItemView)
            Else
                GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.Scan)
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
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.btnQuit.Enabled = False
            Me.btnNext.Enabled = False
            Me.btnBack.Enabled = False
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
            Me.btnBack.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Unfreeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class