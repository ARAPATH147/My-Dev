Public Class frmGdsOutMenu
    Private Sub pnMenuLabelColourIndicator1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnMenuLabelColourIndicator1.Click
        Freeze()
        WorkflowMgr.GetInstance().NextScreen(1)
        Unfreeze()
    End Sub
    Private Sub pnlMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlMenuItem1.Click
        Freeze()
        WorkflowMgr.GetInstance().NextScreen(1)
        Unfreeze()
    End Sub
    Private Sub pnlMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlMenuItem2.Click
        Freeze()
        WorkflowMgr.GetInstance().NextScreen(2)
        Unfreeze()
    End Sub
    Private Sub pnMenuLabelColourIndicator2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnMenuLabelColourIndicator2.Click
        Freeze()
        WorkflowMgr.GetInstance().NextScreen(2)
        Unfreeze()
    End Sub
    Private Sub pnlMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlMenuItem3.Click
        Freeze()
        WorkflowMgr.GetInstance().NextScreen(3)
        Unfreeze()
    End Sub
    Private Sub pnMenuLabelColourIndicator3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnMenuLabelColourIndicator3.Click
        Freeze()
        WorkflowMgr.GetInstance().NextScreen(3)
        Unfreeze()
    End Sub
    Private Sub pnlMenuItem4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlMenuItem4.Click
        Freeze()
        WorkflowMgr.GetInstance().NextScreen(4)
        Unfreeze()
    End Sub

    Private Sub pnMenuLabelColourIndicator4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnMenuLabelColourIndicator4.Click
        Freeze()
        WorkflowMgr.GetInstance().NextScreen(4)
        Unfreeze()
    End Sub
    Private Sub pnlMenuItem5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlMenuItem5.Click
        Freeze()
        WorkflowMgr.GetInstance().NextScreen(5)
        Unfreeze()
    End Sub
    Private Sub pnMenuLabelColourIndicator5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnMenuLabelColourIndicator5.Click
        Freeze()
        WorkflowMgr.GetInstance().NextScreen(5)
        Unfreeze()
    End Sub
    Private Sub pnMenuLabelColourIndicator6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnMenuLabelColourIndicator6.Click
        Freeze()
        WorkflowMgr.GetInstance().NextScreen(6)
        Unfreeze()
    End Sub

    Private Sub pnlMenuItem6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlMenuItem6.Click
        Freeze()
        WorkflowMgr.GetInstance().NextScreen(6)
        Unfreeze()
    End Sub
    Private Sub pnlMenuItem7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlMenuItem7.Click
        Freeze()
        'Recalls CR
        'Create Recalls
        If WorkflowMgr.GetInstance().MenuName.Equals("Recall") Then
            Dim messageResult As MsgBx.ReturnData
            messageResult = MsgBx.ShowMessageDialog(MessageManager.GetInstance().GetMessage("M74").ToString())
            If messageResult = MsgBx.ReturnData.ContinueRecall Then
                WorkflowMgr.GetInstance().NextScreen(7)
            End If
        Else
            objAppContainer.bCreateRecall = False
            WorkflowMgr.GetInstance().NextScreen(7)
        End If

        Unfreeze()
    End Sub
    Private Sub pnMenuLabelColourIndicator7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnMenuLabelColourIndicator7.Click
        Freeze()
        WorkflowMgr.GetInstance().NextScreen(7)
        Unfreeze()
    End Sub
    Private Sub pbContextHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbContextHelp.Click
        Freeze()
        objAppContainer.objScreen.DisplayHelp()
        Unfreeze()
    End Sub
    'DARWIN Change
    Private Sub pnlMenuItem8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlMenuItem8.Click
        Freeze()
        WorkflowMgr.GetInstance().NextScreen(8)
        Unfreeze()
    End Sub
    Private Sub pnMenuLabelColourIndicator8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnMenuLabelColourIndicator8.Click
        Freeze()
        WorkflowMgr.GetInstance().NextScreen(8)
        Unfreeze()
    End Sub

    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Freeze()
        objAppContainer.bRecallStarted = False
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
        If WorkflowMgr.GetInstance().PrevStep = "0" Then

            If MsgBox("Are you sure you want to Quit?", _
                              MsgBoxStyle.YesNo, _
                    "Exit Application") = MsgBoxResult.Yes Then
                WorkflowMgr.GetInstance().ExecQuit()
            End If
        Else
            'If choosed to exit from recall select screen call end session to exit.
            If RLSessionMgr.GetInstance.bIsRecallStarted And Not objAppContainer.bIsCreateRecalls Then
                RLSessionMgr.GetInstance.EndSession()
#If RF Then
                objAppContainer.bRecallConnection = False
#End If
            End If
            WorkflowMgr.GetInstance().ExecPrev()
           
            
        End If
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Unfreeze()
    End Sub

    Private Sub btnLogoff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLogoff.Click
        If WorkflowMgr.GetInstance().PrevStep = "0" Then

            If MsgBox("Are you sure you want to Quit?", _
                              MsgBoxStyle.YesNo, _
                    "Exit Application") = MsgBoxResult.Yes Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Logging off....")
                WorkflowMgr.GetInstance().ExecQuit()
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End If
        Else
            WorkflowMgr.GetInstance().ExecPrev()
        End If
    End Sub
    Private Function Freeze() As Boolean
        RemoveHandler pnMenuLabelColourIndicator1.Click, AddressOf pnMenuLabelColourIndicator1_Click
        RemoveHandler pnlMenuItem1.Click, AddressOf pnlMenuItem1_Click
        RemoveHandler pnMenuLabelColourIndicator2.Click, AddressOf pnMenuLabelColourIndicator2_Click
        RemoveHandler pnlMenuItem2.Click, AddressOf pnlMenuItem2_Click
        RemoveHandler pnMenuLabelColourIndicator3.Click, AddressOf pnMenuLabelColourIndicator3_Click
        RemoveHandler pnlMenuItem3.Click, AddressOf pnlMenuItem3_Click
        RemoveHandler pnMenuLabelColourIndicator4.Click, AddressOf pnMenuLabelColourIndicator4_Click
        RemoveHandler pnlMenuItem4.Click, AddressOf pnlMenuItem4_Click
        RemoveHandler pnMenuLabelColourIndicator5.Click, AddressOf pnMenuLabelColourIndicator5_Click
        RemoveHandler pnlMenuItem5.Click, AddressOf pnlMenuItem5_Click
        RemoveHandler pnMenuLabelColourIndicator6.Click, AddressOf pnMenuLabelColourIndicator6_Click
        RemoveHandler pnlMenuItem6.Click, AddressOf pnlMenuItem6_Click
        RemoveHandler pnMenuLabelColourIndicator7.Click, AddressOf pnMenuLabelColourIndicator7_Click
        RemoveHandler pnlMenuItem7.Click, AddressOf pnlMenuItem7_Click
        RemoveHandler btnQuit.Click, AddressOf btnQuit_Click
        RemoveHandler pbContextHelp.Click, AddressOf pbContextHelp_Click
        RemoveHandler btnLogoff.Click, AddressOf btnLogoff_Click
    End Function
    Private Sub Unfreeze()
        'Emptying the event queue before adding handler to the panels and labels
        Application.DoEvents()
        AddHandler pnMenuLabelColourIndicator1.Click, AddressOf pnMenuLabelColourIndicator1_Click
        AddHandler pnlMenuItem1.Click, AddressOf pnlMenuItem1_Click
        AddHandler pnMenuLabelColourIndicator2.Click, AddressOf pnMenuLabelColourIndicator2_Click
        AddHandler pnlMenuItem2.Click, AddressOf pnlMenuItem2_Click
        AddHandler pnMenuLabelColourIndicator3.Click, AddressOf pnMenuLabelColourIndicator3_Click
        AddHandler pnlMenuItem3.Click, AddressOf pnlMenuItem3_Click
        AddHandler pnMenuLabelColourIndicator4.Click, AddressOf pnMenuLabelColourIndicator4_Click
        AddHandler pnlMenuItem4.Click, AddressOf pnlMenuItem4_Click
        AddHandler pnMenuLabelColourIndicator5.Click, AddressOf pnMenuLabelColourIndicator5_Click
        AddHandler pnlMenuItem5.Click, AddressOf pnlMenuItem5_Click
        AddHandler pnMenuLabelColourIndicator6.Click, AddressOf pnMenuLabelColourIndicator6_Click
        AddHandler pnlMenuItem6.Click, AddressOf pnlMenuItem6_Click
        AddHandler pnMenuLabelColourIndicator7.Click, AddressOf pnMenuLabelColourIndicator7_Click
        AddHandler pnlMenuItem7.Click, AddressOf pnlMenuItem7_Click
        AddHandler btnQuit.Click, AddressOf btnQuit_Click
        AddHandler pbContextHelp.Click, AddressOf pbContextHelp_Click
        AddHandler btnLogoff.Click, AddressOf btnLogoff_Click
    End Sub
End Class