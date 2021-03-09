Public Class frmAPLocalPrinterUtilities
    ''' <summary>
    ''' To send fonts to the mobile printer.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbSendFonts_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSendFonts.Click
        Try
            FreezeControls()
            APSessionMgr.GetInstance.SendFonts()
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
    ''' <summary>
    ''' To perform a test print to check the printer status.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbTestPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbTestPrint.Click
        Try
            FreezeControls()
            APSessionMgr.GetInstance.TestPrint()
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
    ''' <summary>
    ''' To test clearance label printing.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbTestClearancePrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbTestClearancePrint.Click
        Try
            FreezeControls()
            APSessionMgr.GetInstance.TestClearancePrint()
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
    ''' <summary>
    ''' To quit from printer utilities screen.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btn_Quit_small_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Quit_small.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            APSessionMgr.GetInstance.DisplayAPScreen(APSessionMgr.APSCREENS.Home)
#If NRF Then
        UnFreezeControls()
#End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
    Private Sub FreezeControls()
        Try
            Me.btn_Quit_small.Enabled = False
            Me.pbSendFonts.Enabled = False
            Me.pbTestPrint.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub UnFreezeControls()
        Try
            Me.btn_Quit_small.Enabled = True
            Me.pbSendFonts.Enabled = True
            Me.pbTestPrint.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class