''' ***************************************************************************
''' <fileName>frmPCHome.vb</fileName>
''' <summary>The Price Check Home Screen.This is where the barcode is scanned.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''*****************************************************************************
Public Class frmPCHome
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            'System Testing
            If (MessageBox.Show("Are you sure you wish to quit?", _
                                "Confirmation", _
                                MessageBoxButtons.YesNo, _
                                MessageBoxIcon.Question, _
                                MessageBoxDefaultButton.Button1) = (MsgBoxResult.Yes)) Then
                PCSessionMgr.GetInstance().DisplayPCScreen(PCSessionMgr.PCSCREENS.Summary)
            Else
                UnFreezeControls()
            End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
    Private Sub objProdSEL_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles objProdSEL.KeyDown
        Try
            objProdSEL.txtSEL.Focus()
            Me.lblScanEnterPdtCode.Hide()
            Me.lblScanSEL.Show()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub frmPCHome_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StartRead()
        'PCSessionMgr.GetInstance().DisplayPCScreen(PCSessionMgr.PCSCREENS.Home)
    End Sub
    Private Sub frmPCHome_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub
    Private Sub Btn_Next_small2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Next_small2.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            Me.objProdSEL.txtProduct.Text = ""
            Me.objProdSEL.txtSEL.Text = ""
            PCSessionMgr.GetInstance().DisplayPCScreen(PCSessionMgr.PCSCREENS.Home)
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
    Private Sub Info_button_i1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button_i1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.PRICECHK)
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
    'IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.objProdSEL.Btn_CalcPad_small1.Enabled = False
            Me.Btn_Next_small2.Enabled = False
            Me.Btn_Quit_small1.Enabled = False
            Me.Info_button_i1.Enabled = False
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
            Me.objProdSEL.Btn_CalcPad_small1.Enabled = True
            Me.Btn_Next_small2.Enabled = True
            Me.Btn_Quit_small1.Enabled = True
            Me.Info_button_i1.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class