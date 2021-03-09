''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class frmPLHome
    Private Sub btnInfo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInfo.Click
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
    Private Sub custCtrlBtnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnQuit.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
#If NRF Then
            PLSessionMgr.GetInstance().DisplayPLScreen(PLSessionMgr.PLSCREENS.Summary)
#ElseIf RF Then
            If Not PLSessionMgr.GetInstance().EndSession() Then
                UnFreezeControls()
            End If
#End If
            'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        Finally
            UnFreezeControls()
        End Try
    End Sub
    'Private Sub lstView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstView.ItemActivate
    'Try
    '    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
    '    'SFA SIT - Enable FF and AFF Pl for normal users
    '    Dim iIndex As Integer = lstView.SelectedIndices(0)
    '    'Dim index As Integer = CInt(lstView.SelectedIndices(0).ToString)
    '    'If (index = 1) Then
    '    'lstView.Items(iIndex).Selected = False
    '    'lstView.Items(iIndex).Focused = False
    '    'lstView.Invalidate()
    '    'lstView.Items(index + 1).Focused = False
    '    'lstView.Invalidate()
    '    'End If

    '    If lstView.Items(iIndex).ForeColor = Color.Gray Then
    '        lstView.Items(iIndex).Selected = False
    '        lstView.Items(iIndex).Focused = False
    '        Return
    '    End If
    '    If PLSessionMgr.GetInstance().ProcessProductSelection() Then
    '        Me.Visible = False
    '    End If
    '    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
    'Catch ex As Exception
    '    objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)

    'Finally
    '    UnFreezeControls()
    'End Try
    'End Sub
    Private Sub btnHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHelp.Click
        Try
            FreezeControls()
            'System Testing bug Fix for displaying OSSR PL details in PL.
#If RF Then
            If objAppContainer.OSSRStoreFlag = "Y" Then
                MessageBox.Show("Picking List types:" & ControlChars.CrLf & ControlChars.CrLf & _
                              "OS - Offsite WAN " & ControlChars.CrLf & _
                              "SM - Shelf Monitor " & ControlChars.CrLf & _
                              "EX - Excess Stock " & ControlChars.CrLf & _
                              "EB - Excess Stock in BS" & ControlChars.CrLf & _
                              "EO - Excess Stock in OSSR" & ControlChars.CrLf & _
                              "AF - Auto Fast Fill " & ControlChars.CrLf & _
                              "   (AUTO-ALL)-items from all BC" & ControlChars.CrLf & _
                              "   (AUTO-BC)-items from BC only" & ControlChars.CrLf & _
                              "    e.g.(AUTO-BABY)" & ControlChars.CrLf & _
                              "FF -  Fast Fill " & ControlChars.CrLf & _
                                                ControlChars.CrLf, "Picking List Type Help")
            Else
                MessageBox.Show("Picking List types:" & ControlChars.CrLf & ControlChars.CrLf & _
                               "SM - Shelf Monitor " & ControlChars.CrLf & _
                               "EX - Excess Stock " & ControlChars.CrLf & _
                               "AF - Auto Fast Fill " & ControlChars.CrLf & _
                               "   (AUTO-ALL)-items from all BC" & ControlChars.CrLf & _
                               "   (AUTO-BC)-items from BC only" & ControlChars.CrLf & _
                               "    e.g.(AUTO-BABY)" & ControlChars.CrLf & _
                               "FF -  Fast Fill " & ControlChars.CrLf & _
                                                 ControlChars.CrLf, "Picking List Type Help")
            End If
#ElseIf NRF Then
            MessageBox.Show("Picking List types:" & ControlChars.CrLf & ControlChars.CrLf & _
                         "SM - Shelf Monitor " & ControlChars.CrLf & _
                         "EX - Excess Stock " & ControlChars.CrLf & _
                         "AF - Auto Fast Fill " & ControlChars.CrLf & _
                         "   (AUTO-ALL)-items from all BC" & ControlChars.CrLf & _
                         "   (AUTO-BC)-items from BC only" & ControlChars.CrLf & _
                         "    e.g.(AUTO-BABY)" & ControlChars.CrLf & _
                         "FF -  Fast Fill " & ControlChars.CrLf & _
                                           ControlChars.CrLf, "Picking List Type Help")
#End If
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
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.custCtrlBtnQuit.Enabled = False
            Me.btnHelp.Enabled = False
            Me.btnInfo.Enabled = False
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
            Me.btnHelp.Enabled = True
            Me.btnInfo.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub lstView_SelectedIndexChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstView.SelectedIndexChanged
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            'SFA SIT - Enable FF and AFF Pl for normal users
            Dim iIndex As Integer = lstView.SelectedIndices(0)
            'Dim index As Integer = CInt(lstView.SelectedIndices(0).ToString)
            'If (index = 1) Then
            'lstView.Items(iIndex).Selected = False
            'lstView.Items(iIndex).Focused = False
            'lstView.Invalidate()
            'lstView.Items(index + 1).Focused = False
            'lstView.Invalidate()
            'End If

            If lstView.Items(iIndex).ForeColor = Color.Gray Then
                lstView.Items(iIndex).Selected = False
                lstView.Items(iIndex).Focused = False
                Return
            End If
            If PLSessionMgr.GetInstance().ProcessProductSelection() Then
                Me.Visible = False
            End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)

        Finally
            UnFreezeControls()
        End Try
    End Sub
End Class