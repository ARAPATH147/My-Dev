Public Class frmCLLocationSelection

    Private Sub custCtrlBtnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnQuit.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            If CLSessionMgr.GetInstance().ProcessLocationQuit() Then
                Me.Visible = False
            End If
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

    Private Sub btnSalesFloor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSalesFloor.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            If CLSessionMgr.GetInstance().ProcessSalesFloorSelect() Then
                Me.Visible = False
            End If

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

    Private Sub btnBackShop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBackShop.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            If CLSessionMgr.GetInstance().ProcessBackShopSelect() Then
                Me.Visible = False
            End If

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
#Region "OSSR"
    'ambli
    'For OSSR
#If RF Then
    Private Sub btn_OSSR_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OSSR.Click
        Try
        FreezeControls()
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
        CLSessionMgr.GetInstance().ProcessOSSRSelect()
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        UnFreezeControls()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End If
#End Region
    'IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.custCtrlBtnQuit.Enabled = False
            Me.btnSalesFloor.Enabled = False
            Me.btnBackShop.Enabled = False
            Me.Info_button_i1.Enabled = False
            'ambli
            'For OSSR
            Me.btn_OSSR.Enabled = False
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
            Me.custCtrlBtnQuit.Enabled = True
            Me.btnSalesFloor.Enabled = True
            Me.btnBackShop.Enabled = True
            Me.Info_button_i1.Enabled = True
            'ambli
            'For OSSR
            Me.btn_OSSR.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try

    End Sub

    
    Private Sub Info_button_i1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button_i1.Click
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
End Class