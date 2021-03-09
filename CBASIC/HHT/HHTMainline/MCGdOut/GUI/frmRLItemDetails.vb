'''****************************************************************************
''' <FileName>frmRLItemDetails.vb</FileName>
''' <summary>
''' Form to display the recall list item details.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''****************************************************************************
Public Class frmRLItemDetails
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            'Check if all items in COMPANY HO recall is actioned or else don't allow the user to exit.
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.ItemList
            RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Scan)
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
    Private Sub btnConfirm_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConfirm.Click
        Try
            'Updating the stock figure
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Updating Stock Figure...")
            RLSessionMgr.GetInstance().ConfirmDiscrepancy()
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
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnQuantity_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuantity.Click
        Try
            FreezeControls()
            Dim objSftKeyPad As New frmCalcPad(lblStockCountdata, CalcPadSessionMgr.EntryTypeEnum.Quantity)
            If objSftKeyPad.ShowDialog() = Windows.Forms.DialogResult.Cancel Then
                'FIX for Defect 5056
                UnFreezeControls()
                Exit Sub
            End If

            If Not lblStockCountdata.Text.Trim().Length > 0 Then
                MessageBox.Show("Please enter a valid number as Stock Count.", "Invalid Stock Count")
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
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnView.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Loading Recall Item List...")
            RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.ItemDetails
            RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.ItemList)
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
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnProductCode_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProductCode.Click
        Try
            FreezeControls()
            Dim strPrdCode As String = lblBootsCode.Text.Trim()
            Dim objSftKeyPad As New frmCalcPad(lblBootsCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
            objSftKeyPad.ShowDialog()
            Dim strData As String = lblBootsCode.Text
            lblBootsCode.Text = strPrdCode
            If Not (strData.Equals("") Or strPrdCode.Equals(strData)) Then
                BCReader.GetInstance().RaiseScanEvent(strData, BCType.ManualEntry)
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
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
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.ItemList
            If Trim(lblStockCountdata.Text) = "0" And lblStockCountdata.Visible = True Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M58"), _
                "Zero Count", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                 MessageBoxDefaultButton.Button1)
            End If
            RLSessionMgr.GetInstance().SetProductInfo()
            RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Scan)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            'End If
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
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub frmRLItemDetails_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            If Not RLSessionMgr.GetInstance().bIsDiscrepancy Then
                BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
                BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.CODE128)
                BCReader.GetInstance.StartRead()
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
    Private Sub frmRLItemDetails_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        If Not RLSessionMgr.GetInstance().bIsDiscrepancy Then
            BCReader.GetInstance().StopRead()
            BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
            BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.CODE128)
        End If
    End Sub
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.btnQuit.Enabled = False
            Me.btnNext.Enabled = False
            Me.btnView.Enabled = False
            Me.btnProductCode.Enabled = False
            Me.btnQuantity.Enabled = False
            Me.btnConfirm.Enabled = False
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
            Me.btnQuit.Enabled = True
            Me.btnNext.Enabled = True
            Me.btnView.Enabled = True
            Me.btnProductCode.Enabled = True
            Me.btnQuantity.Enabled = True
            Me.btnConfirm.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Unfreeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub lblBarcode_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblBarcode.ParentChanged

    End Sub

    Private Sub lblBootsCode_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblBootsCode.ParentChanged

    End Sub
End Class