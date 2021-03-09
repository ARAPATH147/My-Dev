Public Class frmCOLItemScan
    Private Sub btnQuit_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            objAppContainer.objLogger.WriteAppLog("Quit button click", Logger.LogLevel.INFO)
            Try
                'Added as per SFA - Processes Quit
                If CLSessionMgr.GetInstance().ProcessCOLItemScanQuit() Then
                    Me.Visible = False
                End If
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("Exception occured at Quit button click: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            End Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            objAppContainer.objLogger.WriteAppLog("Exit Quit button Session", Logger.LogLevel.INFO)
#If NRF Then
            UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
    Private Sub Info_button_i1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button_i1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.CUNTLIST)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#If NRF Then
            UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
    Private Sub frmCOLHome_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            BCReader.GetInstance().StartRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub frmCOLHome_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        Try
            BCReader.GetInstance().StopRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub btnView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnView.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            objAppContainer.objLogger.WriteAppLog("View button click", Logger.LogLevel.INFO)


            If Not CLSessionMgr.GetInstance().ProcessViewList() Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End If

            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            objAppContainer.objLogger.WriteAppLog("Exit View button Session", Logger.LogLevel.INFO)
#If NRF Then
            UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
    'IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.ProdSEL1.Btn_CalcPad_small1.Enabled = False
            Me.btnQuit.Enabled = False
            Me.btnView.Enabled = False
            Me.Info_button_i1.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' UnFreeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnFreezeControls()
        Try
            Me.ProdSEL1.Btn_CalcPad_small1.Enabled = True
            Me.btnQuit.Enabled = True
            Me.btnView.Enabled = True
            Me.Info_button_i1.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class