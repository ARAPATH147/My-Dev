Public Class frmSPLPHome
    ''' <summary>
    ''' To Quit from the product scan screen i.e., Quit from search planner module.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        Try
            FreezeControls()
#If RF Then
            'Invoke function to end planner module session.
            If Not SPSessionMgr.GetInstance.EndSession() Then
                UnFreezeControls()
            End If
#Else
        SPSessionMgr.GetInstance.EndSession()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Loads list of sales planners available.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnSalesPlan_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSalesPlan.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            SPSessionMgr.GetInstance.GetFormType(Macros.SP_SALESPLAN)
#If RF Then
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
#Else
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
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
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.btnSalesPlan.Enabled = False
            Me.Btn_CorePlanners1.Enabled = False
            Me.Btn_Quit_small1.Enabled = False
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
            Me.btnSalesPlan.Enabled = True
            Me.Btn_CorePlanners1.Enabled = True
            Me.Btn_Quit_small1.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Loads the list of core planners available for the store.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Btn_CorePlanners1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CorePlanners1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            SPSessionMgr.GetInstance.GetFormType(Macros.SP_CORE)
#If RF Then
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
#Else
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
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