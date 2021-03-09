Public Class frmRLItemListScreen

    Private Sub lvRecallList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvRecallList.ItemActivate
        Try
            FreezeControls()
            Dim indexes As ListView.SelectedIndexCollection = lvRecallList.SelectedIndices
            Dim iCtr As Integer = 0
            Dim bItemSelected As Boolean = False
            Dim strBarcode As String = "0000000"
            For Each iCtr In indexes
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
                'Fix for  In a completed recall list -> go to view -> choose an item -> displays scan screen instead of item details screen.
                'The boots Code was send without trimming the *
                RLSessionMgr.GetInstance().SetCurrentItem(lvRecallList.Items(iCtr).Text.Trim("*"))
                RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.ItemList
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                strBarcode = lvRecallList.Items(iCtr).SubItems(0).Text.Replace("*", "")

                bItemSelected = True
                Exit For
            Next
            If bItemSelected Then
                RLSessionMgr.GetInstance().HandleScanData(strBarcode, BCType.EAN)
                ' RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.ItemDetails)
                'Me.Visible = False
                'Me.Visible = True
            End If
#If NRF Then
UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub

    Private Sub btnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBack.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.ItemList
            RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Scan)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#If NRF Then
UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
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
            Me.btnBack.Enabled = False
            Me.lvRecallList.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ freeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' UnFreeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnFreezeControls()
        Try
            Me.btnBack.Enabled = True
            Me.lvRecallList.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Unfreeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class