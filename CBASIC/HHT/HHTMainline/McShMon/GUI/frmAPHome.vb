Public Class frmAPHome
    ''' <summary>
    ''' To handle click on save button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Btn_Save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Save.Click
        Try
            FreezeControls()
            If lstAvailablePrinter.SelectedIndices.Count >= 1 Then
                'set the selected pritner.
                objAppContainer.strCurrentPrinter = lstAvailablePrinter.SelectedIndices(0).ToString()

                If lstAvailablePrinter.SelectedIndices.Count > 0 Then
                    For iCounter = 0 To lstAvailablePrinter.Items.Count - 1
                        If lstAvailablePrinter.Items(iCounter).Selected Then
                            'update the selected printer.
                            objAppContainer.strCurrentPrinter = lstAvailablePrinter.Items(iCounter).Text.Trim()
                            If lstAvailablePrinter.Items(iCounter).Text.Contains("Mobile") Then
                                objAppContainer.bMobilePrinterAttachedAtSignon = True
                                objAppContainer.strPrintFlag = Macros.PRINT_LOCAL
                            Else
                                objAppContainer.bMobilePrinterAttachedAtSignon = False
                                If lstAvailablePrinter.Items(iCounter).Text.Contains("Controller") Then
                                    objAppContainer.strPrintFlag = Macros.PRINT_BATCH
                                Else
                                    objAppContainer.strPrintFlag = objAppContainer.strCurrentPrinter.Substring(0, 1)
                                End If
                            End If
                            Exit For
                        End If
                    Next
                End If
                'Exit Assign printer module.
                APSessionMgr.GetInstance.EndSession()
            Else
                MessageBox.Show("Please select a printer from the list box.", "Assign Printer", MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                UnFreezeControls()
            End If
            'UnFreezeControls()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
    ''' <summary>
    ''' Function to freeze controls.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.Btn_Save.Enabled = False
            Me.btn_Quit_small.Enabled = False
            Me.Btn_Utilities.Enabled = False
            Me.lstAvailablePrinter.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Function to unfreeze controls.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnFreezeControls()
        Try
            Me.Btn_Utilities.Enabled = True
            Me.Btn_Save.Enabled = True
            Me.btn_Quit_small.Enabled = True
            Me.lstAvailablePrinter.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To handle click on Quit button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btn_Quit_small_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Quit_small.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            APSessionMgr.GetInstance.EndSession()
            'UnFreezeControls()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To handle click on utilities button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Btn_Utilities_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Utilities.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            If MobilePrintSessionManager.GetInstance() IsNot Nothing Then
                If MobilePrintSessionManager.GetInstance().MobilePrinterStatus Then
                    APSessionMgr.GetInstance.DisplayAPScreen(APSessionMgr.APSCREENS.Utilities)
                Else
                    MessageBox.Show("This option is for use by stores with Mobile printers ONLY." & vbCrLf & vbCrLf & _
                    "If you have attached a Mobile printer to your device then please Sign Off, " & _
                    "re-attach printer, Sign On and try again", "Printer Warning", MessageBoxButtons.OK, _
                    MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
                End If
            Else
                MessageBox.Show("This option is for use by stores with Mobile printers ONLY." & vbCrLf & vbCrLf & _
                    "If you have attached a Mobile printer to your device then please Sign Off, " & _
                    "re-attach printer, Sign On and try again", "Printer Warning", MessageBoxButtons.OK, _
                    MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            End If
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
End Class