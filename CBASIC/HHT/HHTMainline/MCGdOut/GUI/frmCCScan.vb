Public Class frmCCScan
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnFinish_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFinish.Click
        Try
            FreezeControls()
            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), _
            "Alert", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, _
             MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.OK Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
                Me.Refresh()
                'Fix for item not on file
                If CCSessionMgr.GetInstance().ValidateNotOnFile() Then
                    CCSessionMgr.GetInstance().DisplayCCScreen(CCSessionMgr.CCSCREENS.ItemList)
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
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
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
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub frmCCScan_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.CODE128)
            BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.EAN128)
            BCReader.GetInstance().StartRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub frmCCScan_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        Try
            BCReader.GetInstance().StopRead()
            BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.CODE128)
            BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.EAN128)
            'BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.UPCA)
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
            Me.btnFinish.Enabled = False
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
            Me.btnFinish.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Unfreeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class