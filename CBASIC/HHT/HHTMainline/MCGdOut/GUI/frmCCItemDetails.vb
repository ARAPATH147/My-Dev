Public Class frmCCItemDetails

    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        Try
            FreezeControls()
            'Check if the quantity is greater than 0
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Updating the Product Information...")
            If CCSessionMgr.GetInstance().ValidateQuantity(Me.lblQuantityData.Text) Then
                'Set the product data 
                CCSessionMgr.GetInstance().SetProductInfo()
                'Clear the item detail form
                CCSessionMgr.GetInstance().ClearData()
                CCSessionMgr.GetInstance().DisplayCCScreen(CCSessionMgr.CCSCREENS.Scan)
            End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#If NRF Then
            UnFreezeControls ()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            'Quit the module
            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M3"), _
                "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                CCSessionMgr.GetInstance().EndSession()
                WorkflowMgr.GetInstance().ExecQuit()
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub Btn_CalcPad_small_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small.Click
        Try
            FreezeControls()
            'Prompt the calcpad for manual entry
            Dim m_objcalpad As New frmCalcPad(lblQuantityData, CalcPadSessionMgr.EntryTypeEnum.Quantity)
            m_objcalpad.ShowDialog()
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
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.btnQuit.Enabled = False
            Me.btnNext.Enabled = False
            Me.Btn_CalcPad_small.Enabled = False
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
            Me.btnNext.Enabled = True
            Me.Btn_CalcPad_small.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Unfreeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class