''' ***************************************************************************
''' <fileName>frmPCLItemDetails.vb</fileName>
''' <summary>The Print Clearance SEL Item Details Screen. The Item details are displayed here.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Dec-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''*****************************************************************************
''' * Modification Log
''' ********************************************************************
''' * 1.1   Archana Chandramathi    13 C Chilled Food Changes
''' Change the QUANTITY to 99 in case of clearance label printing
''' ********************************************************************/
Public Class frmPCLItemDetails
    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        Try
            'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            FreezeControls()
            Dim strPrdCode As String = lblBootsCode.Text.Trim()
            Dim objSftKeyPad As New frmCalcPad(lblBootsCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                Dim strData As String = lblBootsCode.Text
                lblBootsCode.Text = strPrdCode

                BCReader.GetInstance.EventBCScannedHandler(strData, BCType.ManualEntry)
            End If
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
    ''' <summary>
    ''' Handle the click event for Print button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Btn_Print1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Print1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            CLRSessionMgr.GetInstance().ProcessPrintSelection()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            'display home screen to scan the next item.
            CLRSessionMgr.GetInstance.DisplayPCLScreen(CLRSessionMgr.PCLSCREENS.Home)
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
    ''' Handle click event for calc pad button against SEL quantity.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Btn_CalcPad_small2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small2.Click
        Try
            FreezeControls()
            Dim strTemp As String = ""
            strTemp = lblSELQty.Text.ToString()
            Dim objCalcPad = New frmCalcPad(lblSELQty, CalcPadSessionMgr.EntryTypeEnum.PrintQuantity)
            'If Quit is selected from calcpad, the previous count needs to be maintained
            If (lblSELQty.Text.ToString() = "") Then
                lblSELQty.Text = strTemp
            End If
            If objCalcPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                objCalcPad = Nothing
                '1.1 Change the Qty to 99
                'Archana Chandramathi
                '13C Chilled Food project 
                Dim iMaxQty As Integer = 99
                If Not (CInt(lblSELQty.Text().ToString()) > iMaxQty) And (CInt(lblSELQty.Text().ToString()) <> 0) Then
                    lblSELQty.Text = lblSELQty.Text
                    'update the SEL quantity.
                    CLRSessionMgr.GetInstance.UpdateProductInfo()
                Else
                    lblSELQty.Text = strTemp
                End If
            End If
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
    ''' <summary>
    ''' Handle click event for calc pad button against clearance price.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Btn_CalcPad_small3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small3.Click
        Try
            FreezeControls()
            Dim strTemp As String = ""
            strTemp = lblClearancePrice.Text.ToString()
            Dim objCalcPad = New frmCalcPad(lblClearancePrice, CalcPadSessionMgr.EntryTypeEnum.ClearancePrice)
            'If Quit is selected from calcpad, the previous count needs to be maintained
            If objCalcPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                objCalcPad = Nothing
                CLRSessionMgr.GetInstance.UpdateClearancePrice(lblClearancePrice.Text.ToString())
                Me.Refresh()
            End If
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
    ''' <summary>
    ''' Handle form activate event.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub frmPSItemDetails_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StartRead()
    End Sub
    ''' <summary>
    ''' Handles form deactivate event.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub frmPSItemDetails_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.Btn_CalcPad_small1.Enabled = False
            Me.Btn_CalcPad_small2.Enabled = False
            Me.Btn_Print1.Enabled = False
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
            Me.Btn_CalcPad_small1.Enabled = True
            Me.Btn_CalcPad_small2.Enabled = True
            Me.Btn_Print1.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To quit from the clearance label print screen.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            CLRSessionMgr.GetInstance().ProcessQuitSelection()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            'display home screen to scan the next item.
            CLRSessionMgr.GetInstance.DisplayPCLScreen(CLRSessionMgr.PCLSCREENS.Home)
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