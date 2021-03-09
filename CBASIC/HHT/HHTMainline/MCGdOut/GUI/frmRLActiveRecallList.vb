'''****************************************************************************
''' <FileName>frmRLActiveRecallList.vb</FileName>
''' <summary>
''' The Recall feature class to display active recall list based on the recall
''' type choosen.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''****************************************************************************
Public Class frmRLActiveRecallList
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHelp.Click
        'MessageBox.Show(ControlChars.Tab & _
        '                '"Recall types" & ControlChars.CrLf & ControlChars.CrLf & _
        '"C -  Customer " & ControlChars.CrLf & _
        '"W -  Withdrawn " & ControlChars.CrLf & _
        '"R -  100% Returns " & ControlChars.CrLf & _
        '                  ControlChars.CrLf & _
        '"BC -  Batch Customer" & ControlChars.CrLf & _
        '"BW - Batch Withdrawn" & ControlChars.CrLf & _
        '"BR -  Batch Returns" & ControlChars.CrLf & _
        '                        ControlChars.CrLf & _
        '"EX -  Excess Salesplan" & ControlChars.CrLf & _
        '"PL -  Planner Leaver" & _
        'ControlChars.CrLf & ControlChars.CrLf, "Recall Type Help")

        If RLSessionMgr.GetInstance().bIsRecallReturns Then
            MessageBox.Show("Item" & ControlChars.CrLf & ControlChars.CrLf & _
                     "TL -  Tailored List " & ControlChars.CrLf & _
             ControlChars.CrLf & ControlChars.CrLf, "Recall Help")
        End If
    End Sub
    ''' <summary>
    ''' Handle Quit button click event for the active recall list form.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
#If RF Then
            objAppContainer.bRecallConnection = False
#End If

            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M3"), _
                   "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                   MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
#If RF Then
                objAppContainer.bIsActiveRecallListSCreen = False
#End If
                RLSessionMgr.GetInstance().CallingScreen = Nothing
                ' RLSessionMgr.GetInstance().EndSession()
                WorkflowMgr.GetInstance().ExecQuit()
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Function to handle recall list selection and load the items for
    ''' corresponding list.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' 25-June-09: Fix for recall CR to include item status: Added message box prompt
    ''' when a user selects a recall list that is already actioned.
    ''' </remarks>
    Private Sub lvRecallList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvRecallList.ItemActivate
        Try
            FreezeControls()
            'Load the item list and display the scan screen
            Dim iCtr As Integer = 0
            Dim indexes As ListView.SelectedIndexCollection = lvRecallList.SelectedIndices
            Dim bSelected As Boolean = False
            Dim isTailored As Boolean = False
            Dim isActioned As Boolean = False
            Dim iMinRetQty As Integer = 0
            For Each iCtr In indexes
                'Check if the first item starts with * to check for list is actioned or not.
                If lvRecallList.Items(iCtr).SubItems(3).Text.Equals("Completed") Then
                    If MessageBox.Show(MessageManager.GetInstance().GetMessage("M69"), _
                        "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                        MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.No Then
                        'Unfreeze the controls before rolling back.
                        UnFreezeControls()
                        'Exit the for loop and retain the same screen.
                        Exit For
                    End If
                    isActioned = True
                    RLSessionMgr.GetInstance.bActionedRecall = True
                Else
                    RLSessionMgr.GetInstance.bActionedRecall = False
                End If
                MsgBx.ShowMessage("Please wait loading recall list items.")
                'Load the recall list item.
                Me.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Loading Recall List Items...")
                If lvRecallList.Items(iCtr).SubItems(2).Text.Equals("TL") Then
                    isTailored = True
                End If
                Dim strRecallCount As String = lvRecallList.Items(iCtr).SubItems(2).Text
                If strRecallCount = "TL" Then
                    strRecallCount = RLSessionMgr.GetInstance.GetTLItemCount(lvRecallList.Items(iCtr).SubItems(0).Text)
                End If
                RLSessionMgr.GetInstance().m_RecallDesc = lvRecallList.Items(iCtr).SubItems(1).Text.ToString()
                If RLSessionMgr.GetInstance().SetRecallItemList(lvRecallList.Items(iCtr).SubItems(0).Text, _
                                                                CInt(strRecallCount), _
                                                                isActioned, isTailored) Then
                    RLSessionMgr.GetInstance().SetCurrentList(lvRecallList.Items(iCtr).SubItems(0).Text)

                    RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.ActiveRecallList
                    bSelected = True
                End If
                Exit For
            Next
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            'Close the message box.
            MsgBx.CloseMessage()
            'If a recall list is selected from the active recall list screen
            'then display the corresponding special instrution screen or load the recall list items.
            If bSelected Then
#If RF Then
                objAppContainer.bIsActiveRecallListSCreen = False
#End If
                'CR FOR MRQ DEFECT 4957
                If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.EXCESSSALESPLAN Then
                    'Send the recall number to get minimum return quantity.
                    If RLSessionMgr.GetInstance().iMinReturnQty >= 0 Then
                        MessageBox.Show("Only return significant backshop excesses on this Recall Min Qty: " & _
                                    RLSessionMgr.GetInstance().iMinReturnQty.ToString() & " units per item." & vbCrLf & vbCrLf & _
                                    "Salesfloor must be full.", _
                        "Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                        MessageBoxDefaultButton.Button1)
                    End If
                End If
                'If there is no tailored items in a tailored recall then ask if the user
                'wish to complete the recall straight away or continue adding the items in recall.
                If RLSessionMgr.GetInstance().bNoTailoredItemsinReturns Then
                    If (MessageBox.Show("No tailored items for this recall. Select ‘Yes’ to continue or ‘No’ to complete the recall.", _
                    "Confirmation", _
                        MessageBoxButtons.YesNo, _
                        MessageBoxIcon.Question, _
                        MessageBoxDefaultButton.Button1) = (MsgBoxResult.Yes)) Then
                        'If special instruction is present
#If RF Then
                        If (RLSessionMgr.GetInstance.m_CurrentList.RecallMessage = "Y") Or _
                           (RLSessionMgr.GetInstance.m_CurrentList.BatchNos.Trim(" ") <> Nothing) Then
#ElseIf NRF Then
                        If Not (RLSessionMgr.GetInstance.m_CurrentList.RecallMessage.Trim("0") = "") Or _
                           Not (RLSessionMgr.GetInstance.m_CurrentList.BatchNos.Trim("0") = "") Then
#End If
                            RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.SpclInstructions)
                        Else
                            RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Scan)
                        End If
                    Else
                        'if there are no tailored item in 100% Returns
                        RLSessionMgr.GetInstance().bNoTailoredRtrnItem = True
                        RLSessionMgr.GetInstance().m_UODNumber = "00000000001000"
                        RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Despatch)
                        'RLSessionMgr.GetInstance().bNoTailoredRtrnItem = False
                    End If
                Else
                    'Recalls Cr,Special Instructions
                    'For a non tailore 100% returns recall and other recall types
#If RF Then
                    If (RLSessionMgr.GetInstance.m_CurrentList.RecallMessage.Equals("Y")) Or _
                       (RLSessionMgr.GetInstance.m_CurrentList.BatchNos.Trim(" ") <> Nothing) Then
#ElseIf NRF Then
                    If Not (RLSessionMgr.GetInstance.m_CurrentList.RecallMessage.Trim("0") = "") Or _
                       Not (RLSessionMgr.GetInstance.m_CurrentList.BatchNos.Trim("0") = "") Then
#End If
                        RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.SpclInstructions)
                    Else
                        RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Scan)
                    End If
                End If
#If NRF Then
                UnFreezeControls()
#End If
            Else
#If NRF Then
                UnFreezeControls()
#End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                MsgBx.CloseMessage()
                ' Throw ex
            End If
#End If
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
            Me.btnQuit.Enabled = False
            Me.btnHelp.Enabled = False
            Me.lvRecallList.Enabled = False
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
            Me.btnHelp.Enabled = True
            Me.lvRecallList.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Unfreeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class