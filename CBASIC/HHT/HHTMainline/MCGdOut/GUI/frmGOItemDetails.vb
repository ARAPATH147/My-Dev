Public Class frmGOItemDetails

    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M3"), _
               "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
               MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.TRANSFERS Then
                    GOTransferMgr.GetInstance().EndSession()
                    WorkflowMgr.GetInstance().ExecQuit()
                Else
                    'GOSessionMgr.GetInstance().EndSession()
                    GOSessionMgr.GetInstance().ClearData()
                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.Scan)
                End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Updating Item Information")
            Me.Refresh()
            If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.TRANSFERS Then
                If GOTransferMgr.GetInstance().ValidateQuantity(Me.lblQuantityData.Text) Then
                    GOTransferMgr.GetInstance().SetProductInfo()
                    GOTransferMgr.GetInstance().ClearData()
                    GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTransferMgr.GOTRANSFER.Scan)
                End If
            Else
                If GOSessionMgr.GetInstance().ValidateQuantity(Me.lblQuantityData.Text) Then
                    GOSessionMgr.GetInstance().SetProductInfo()
                    GOSessionMgr.GetInstance().ClearData()
                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.Scan)
                End If
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

    Private Sub Btn_CalcPad_small_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small.Click
        Try
            FreezeControls()
            Dim m_objcalpad As New frmCalcPad(lblQuantityData, CalcPadSessionMgr.EntryTypeEnum.Quantity)
            m_objcalpad.ShowDialog()
            m_objcalpad.Close()
#If NRF Then
            objAppContainer.objCurrentModule = objAppContainer.objActiveModule
#End If

            'UAT
            GOTransferMgr.GetInstance().ValidateQuantity(Me.lblQuantityData.Text)

#If NRF Then
            If objAppContainer.objCurrentModule = AppContainer.ACTIVEMODULE.GDSOUT Then
                objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.GDSOUT
            End If
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

    Private Sub frmGOItemDetails_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            BCReader.GetInstance().StopRead()
            BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.CODE128)
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
            Me.btnNext.Enabled = False
            Me.Btn_CalcPad_small.Enabled = False
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
            Me.Btn_CalcPad_small.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Unreeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class