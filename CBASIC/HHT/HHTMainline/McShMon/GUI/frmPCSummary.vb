''' ***************************************************************************
''' <fileName>frmPCSumamry.vb</fileName>
''' <summary>The Price Check Summary Screen. The number of SELs queued is displayed here.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''*****************************************************************************
Public Class frmPCSummary
    Private Sub Btn_Ok1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Ok1.Click
        Try
            FreezeControls()
#If RF Then
            If Not PCSessionMgr.GetInstance.EndSession() Then
                UnFreezeControls()
            End If
#ElseIf NRF Then
            PCSessionMgr.GetInstance.EndSession()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at OK button click: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit OK button Session", Logger.LogLevel.INFO)
    End Sub
    'IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.Btn_Ok1.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub UnFreezeControls()
        Try
            Me.Btn_Ok1.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class