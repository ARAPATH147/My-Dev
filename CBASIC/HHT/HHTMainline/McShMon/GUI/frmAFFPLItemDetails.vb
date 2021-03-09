Public Class frmAFFPLItemDetails

    Private Sub custCtrlBtnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnNext.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            PLSessionMgr.GetInstance().ProcessItemDetailsNext(Macros.SENDER_FORM_ACTION)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            UnFreezeControls()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.custCtrlBtnBack.Enabled = False
            Me.custCtrlBtnNext.Enabled = False
            Me.Info_button_i1.Enabled = False
            Me.custCtrlBtnQuit.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' UnFreeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnFreezeControls()
        Try
            Me.custCtrlBtnBack.Enabled = True
            Me.custCtrlBtnNext.Enabled = True
            Me.Info_button_i1.Enabled = True
            Me.custCtrlBtnQuit.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub custCtrlBtnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnBack.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            PLSessionMgr.GetInstance().ProcessItemDetailsBack(Macros.SENDER_FORM_ACTION)
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

    Private Sub custCtrlBtnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnQuit.Click
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
        PLSessionMgr.GetInstance().ProcessItemDetailsQuit()
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try

    End Sub

    Private Sub Info_button_i1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button_i1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.PICKGLST)
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
#If RF Then
    Private Sub btn_OSSRItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OSSRItem.Click
        Try
            PLSessionMgr.GetInstance().DisplayOSSRToggle(lblOSSR, _
                     objAppContainer.objHelper.UnFormatBarcode(lblBootsCodeDisplay.Text.ToString()), _
                     objAppContainer.objHelper.UnFormatBarcode(lblProductCodeDisplay.Text.ToString()))
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End If
    'Auto Fast Fill CR
    Private Sub btnView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnView.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            PLSessionMgr.GetInstance().DisplayPLScreen(PLSessionMgr.PLSCREENS.ItemView)

            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

            UnFreezeControls()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class