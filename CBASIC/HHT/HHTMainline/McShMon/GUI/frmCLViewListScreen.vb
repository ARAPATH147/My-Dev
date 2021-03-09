Public Class frmCLViewListScreen


    Private Sub lstvwItemDetails_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstvwItemDetails.ItemActivate
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            If CLSessionMgr.GetInstance().ProcessViewListItemSelect() Then
                Me.Visible = False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        Finally
            UnFreezeControls()

        End Try
    End Sub
    'IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.custCtrlBtnQuit.Enabled = False
            Me.Info_button_i1.Enabled = False
            Me.Btn_Help.Enabled = False
            Me.lstvwItemDetails.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception Occured @ Freeze Controls " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' UnFreeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnFreezeControls()
        Try
            Me.custCtrlBtnQuit.Enabled = True
            Me.Info_button_i1.Enabled = True
            Me.Btn_Help.Enabled = True
            Me.lstvwItemDetails.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception Occured @ Unfreeze Controls " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub custCtrlBtnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnQuit.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            If CLSessionMgr.GetInstance().ProcessViewListQuit() Then
                Me.Visible = False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        Finally
            UnFreezeControls()
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

    Private Sub Btn_Help_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Help.Click
        Try
            FreezeControls()
            If CLSessionMgr.GetInstance.CurrentScreen = Macros.Count_LIST_DISCREPANCY Then
                If CLSessionMgr.GetInstance.CurrentLocation = Macros.COUNT_SALES_FLOOR Then
                    MessageBox.Show("Items displayed have been partially counted. All sites must be counted ", "Count List Help")
                ElseIf CLSessionMgr.GetInstance.CurrentLocation = Macros.COUNT_BACK_SHOP Then
                    MessageBox.Show("MBS= Main back shop" & ControlChars.NewLine & ControlChars.NewLine & _
                           "Items displayed have only been partially counted. All main back shop sites must be counted.", "Count List Help")
                ElseIf CLSessionMgr.GetInstance.CurrentLocation = Macros.COUNT_OSSR Then
                    MessageBox.Show("OSSR= Offsite Stock Room" & ControlChars.NewLine & ControlChars.NewLine & _
                           "Items displayed have only been partially counted. All ossr sites must be counted.", "Count List Help")
                End If
            ElseIf CLSessionMgr.GetInstance.CurrentScreen = Macros.COUNT_LIST_BACKSHOPSUMMARY Then
                If CLSessionMgr.GetInstance.CurrentLocation = Macros.COUNT_BACK_SHOP Then
                    MessageBox.Show("MBS= Main back Shop. Main back shop counts are compulsory" & ControlChars.NewLine & ControlChars.NewLine & _
                                    "PSP= Pending Sales Plan site." & ControlChars.NewLine & ControlChars.NewLine & _
                                    "N= Not counted" & ControlChars.NewLine & ControlChars.NewLine & _
                                    "Y= Counted" & ControlChars.NewLine & ControlChars.NewLine & _
                                    "NA= Item is not on a pending sales plan", "Count List Help")
                ElseIf CLSessionMgr.GetInstance.CurrentLocation = Macros.COUNT_OSSR Then
                    MessageBox.Show("OSSR= Offsite Stock Room. OSSR counts are compulsory" & ControlChars.NewLine & ControlChars.NewLine & _
                                    "PSP= Pending Sales Plan site." & ControlChars.NewLine & ControlChars.NewLine & _
                                    "N= Not counted" & ControlChars.NewLine & ControlChars.NewLine & _
                                    "Y= Counted" & ControlChars.NewLine & ControlChars.NewLine & _
                                    "NA= Item is not on a pending sales plan", "Count List Help")
                End If
            ElseIf CLSessionMgr.GetInstance.CurrentScreen = Macros.COUNT_LIST_ITEMSUMMARY Then
#If RF Then
                MessageBox.Show("SF= Sales Floor" & ControlChars.NewLine & ControlChars.NewLine & _
                                "BS= Back Shop" & ControlChars.NewLine & ControlChars.NewLine & _
                                "OS= Offsite Stock Room" & ControlChars.NewLine & ControlChars.NewLine & _
                                "N= Not counted" & ControlChars.NewLine & ControlChars.NewLine & _
                                "Y= Counted", "Count List Help")
#ElseIf NRF Then
                MessageBox.Show("SF= Sales Floor" & ControlChars.NewLine & ControlChars.NewLine & _
                                "BS= Back Shop" & ControlChars.NewLine & ControlChars.NewLine & _
                                "N= Not counted" & ControlChars.NewLine & ControlChars.NewLine & _
                                "Y= Counted", "Count List Help")
#End If
                
            Else
                If CLSessionMgr.GetInstance.CurrentLocation = Macros.COUNT_SALES_FLOOR Then
                    MessageBox.Show("*= All sites not counted for this item", "Count List Help")
                ElseIf CLSessionMgr.GetInstance.CurrentLocation = Macros.COUNT_BACK_SHOP Then
                    MessageBox.Show("MBS= Main back Shop. Main back shop counts are compulsory" & ControlChars.NewLine & ControlChars.NewLine & _
                            "PSP= Pending Sales Plan site." & ControlChars.NewLine & ControlChars.NewLine & _
                            "N= Not counted" & ControlChars.NewLine & ControlChars.NewLine & _
                            "Y= Counted" & ControlChars.NewLine & ControlChars.NewLine & _
                            "NA= Item is not on a pending sales plan", "Count List Help")
                ElseIf CLSessionMgr.GetInstance.CurrentLocation = Macros.COUNT_OSSR Then
                    MessageBox.Show("OSSR= Offsite Stock Room. OSSR counts are compulsory" & ControlChars.NewLine & ControlChars.NewLine & _
                            "PSP= Pending Sales Plan site." & ControlChars.NewLine & ControlChars.NewLine & _
                            "N= Not counted" & ControlChars.NewLine & ControlChars.NewLine & _
                            "Y= Counted" & ControlChars.NewLine & ControlChars.NewLine & _
                            "NA= Item is not on a pending sales plan", "Count List Help")
                End If
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
End Class