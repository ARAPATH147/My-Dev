Public Class frmScan

    Private Sub frmScan_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        'BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
        BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.CODE128)
        BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.EAN128)
        BCReader.GetInstance().StartRead()
    End Sub

    Private Sub frmScan_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
        'BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
        BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.CODE128)
        BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.EAN128)
    End Sub

    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        Try
            FreezeControls()
            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M1"), "Confirm", _
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.OK Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
                'Fix for item not on file
                If GOSessionMgr.GetInstance().ValidateNotOnFile() Then
                    Select Case WorkflowMgr.GetInstance().objActiveFeature
                        Case WorkflowMgr.ACTIVEFEATURE.SEMICENTRALISED
                            If GOSessionMgr.GetInstance().SupplyRoute = "D" Then
                                GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.SupplierList)
                            Else
                                GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.ItemView)
                            End If
                        Case WorkflowMgr.ACTIVEFEATURE.DIRECTRETURNS
                            If GOSessionMgr.GetInstance().SupplyRoute = "D" Then
                                GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.SupplierList)
                            Else
                                GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.ItemView)
                            End If
                        Case Else
                            GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.ItemView)
                    End Select
                End If
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
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

    Private Sub btnFinish_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFinish.Click
        Try
            FreezeControls()
            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Confirm", _
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.OK Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
                If Not (WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.TRANSFERS) Then
                    'Fix for item not on file
                    If GOSessionMgr.GetInstance().ValidateNotOnFile() Then
                        Select Case WorkflowMgr.GetInstance().objActiveFeature
                            Case WorkflowMgr.ACTIVEFEATURE.SEMICENTRALISED
                                If GOSessionMgr.GetInstance().SupplyRoute = "D" Then
                                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.SupplierList)
                                Else
                                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.ItemView)
                                End If
                            Case WorkflowMgr.ACTIVEFEATURE.DIRECTRETURNS
                                If GOSessionMgr.GetInstance().SupplyRoute = "D" Then
                                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.SupplierList)
                                Else
                                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.ItemView)
                                End If
                            Case WorkflowMgr.ACTIVEFEATURE.TRANSFERS
                                '    GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTransferMgr.GOTRANSFER.ItemView)
                            Case Else
                                GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.ItemView)
                        End Select
                    End If
                ElseIf WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.TRANSFERS Then
                    If GOTransferMgr.GetInstance().ValidateNotOnFile() Then
                        GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTransferMgr.GOTRANSFER.ItemView)
                    End If
                End If
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
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
            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M3"), _
                "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.TRANSFERS Then
                    GOTransferMgr.GetInstance().EndSession()
                Else
                    GOSessionMgr.GetInstance().EndSession()
                End If
                WorkflowMgr.GetInstance().ExecQuit()
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub btnDestroy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDestroy.Click
        Try
            FreezeControls()
            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M57"), "Confirm", _
            MessageBoxButtons.OKCancel, Nothing, MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.OK Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
                'If GOSessionMgr.GetInstance().GetAuthorizationID() Is Nothing Then
                '    GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.Authorizationid)

                'Else
                'Fix for item not on file
                If GOSessionMgr.GetInstance().ValidateNotOnFile() Then
                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.ItemView)
                End If
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
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
            Me.btnFinish.Enabled = False
            Me.btnReturn.Enabled = False
            Me.btnDestroy.Enabled = False
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
            Me.btnFinish.Enabled = True
            Me.btnReturn.Enabled = True
            Me.btnDestroy.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Unfreeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class