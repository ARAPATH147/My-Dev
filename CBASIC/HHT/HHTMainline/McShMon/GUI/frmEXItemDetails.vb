'''***************************************************************
''' <FileName>frmEXItemDetails.vb</FileName>
''' <summary>
''' This form displays the Item details screen for excess stock where the user can enter the backshop quantity.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> ''' <DateModified>21-Oct-2011</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> ''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
Public Class frmEXItemDetails

    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            FreezeControls()
            objAppContainer.objLogger.WriteAppLog("Quit button click", Logger.LogLevel.INFO)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            Try
                EXSessionMgr.GetInstance().ProcessItemDetailsQuit()
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("Exception occured at Quit button click: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            End Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            objAppContainer.objLogger.WriteAppLog("Exit Quit button Session", Logger.LogLevel.INFO)
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

    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        Try
            FreezeControls()

            Dim objSftKeyPad As New frmCalcPad(lblBackQtyText, CalcPadSessionMgr.EntryTypeEnum.Quantity)
            'Updates the entered backshop quantity
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then

                If (EXSessionMgr.GetInstance().UpdateProductInfo(True)) Then
                    EXSessionMgr.GetInstance.IsQtyUpdated = True
                    'Added as per SFA - Display ItemDetails screen if count pending in sites
                    If EXSessionMgr.GetInstance().m_IsCountPend = False Then
                        EXSessionMgr.GetInstance.DisplayEXScreen(EXSessionMgr.EXSCREENS.Home)
                    Else
                        EXSessionMgr.GetInstance.DisplayEXScreen(EXSessionMgr.EXSCREENS.ItemDetails, Macros.SCREEN_ITEM_DETAIL, _
                        EXSessionMgr.GetInstance().m_SelectedIndex)
                    End If
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

    Private Sub Btn_Next_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Next_small1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            objAppContainer.objLogger.WriteAppLog("Next button click", Logger.LogLevel.INFO)
            Try
                'Integration Testing
                If (EXSessionMgr.GetInstance().UpdateProductInfo(False)) Then
                    EXSessionMgr.GetInstance.DisplayEXScreen(EXSessionMgr.EXSCREENS.Home)
                Else
                    EXSessionMgr.GetInstance.DisplayEXScreen(EXSessionMgr.EXSCREENS.ItemDetails, Nothing, EXSessionMgr.GetInstance.m_SelectedIndex)
                End If
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("Exception occured at Next button click: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            End Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            objAppContainer.objLogger.WriteAppLog("Exit Next button Session", Logger.LogLevel.INFO)
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

    Private Sub Info_button_i1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button_i1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            'Enable SEL scanning for Item Info
            BCReader.GetInstance().EnableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.EXCSSTCK)
            'Disable SEL Scanning
            BCReader.GetInstance().DisableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
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
    Private Sub btnView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnView.Click
        Try
            FreezeControls()
            objAppContainer.objLogger.WriteAppLog("View button click", Logger.LogLevel.INFO)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            Try
                EXSessionMgr.GetInstance().DisplayEXScreen(EXSessionMgr.EXSCREENS.ItemView)
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("Exception occured at View button click: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            End Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            objAppContainer.objLogger.WriteAppLog("Exit View button Session", Logger.LogLevel.INFO)
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
            Me.btnQuit.Enabled = False
            Me.btnView.Enabled = False
            Me.Info_button_i1.Enabled = False
            Me.Btn_CalcPad_small1.Enabled = False
            Me.Btn_Next_small1.Enabled = False
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
            Me.btnQuit.Enabled = True
            Me.btnView.Enabled = True
            Me.Info_button_i1.Enabled = True
            Me.Btn_CalcPad_small1.Enabled = True
            Me.Btn_Next_small1.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
  
    Private Sub frmEXItemDetails_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            'SFA  Testing
            If lblBckShpQty.Text.Equals("Enter Quantity on Shelf:") Then
                BCReader.GetInstance().StopRead()
            Else
            BCReader.GetInstance().StartRead()
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
        'UnFreezeControls()
    End Sub
    Private Sub frmEXItemDetails_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        Try
            BCReader.GetInstance().StopRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#If RF Then
    Private Sub Btn_OSSRItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_OSSRItem1.Click
        Try
            EXSessionMgr.GetInstance().DisplayOSSRToggle(lblOSSR, _
                          objAppContainer.objHelper.UnFormatBarcode(lblBootsCode.Text.ToString()))
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
#End If

    Private Sub frmEXItemDetails_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Public Sub cmbSite_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbSite.SelectedIndexChanged
        Try
            If Not EXSessionMgr.GetInstance.m_SelectedIndex.Equals(cmbSite.SelectedIndex) Then
                EXSessionMgr.GetInstance.m_SelectedIndex = cmbSite.SelectedIndex
                EXSessionMgr.GetInstance.DisplayEXScreen(EXSessionMgr.EXSCREENS.ItemDetails, Macros.SCREEN_ITEM_CONFIRM, cmbSite.SelectedIndex)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    'Added as per SFA - To confirm the product code on change of site
    Private Sub Btn_CalcPad_small2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small2.Click
        Try
            FreezeControls()
            Dim strPrdCode As String = lblProductCode.Text.Trim()
            'Dim bIsQuit As Boolean = True
            Dim objSftKeyPad As New frmCalcPad(lblProductCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
            ' objSftKeyPad.ShowDialog()
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                Dim strData As String = lblProductCode.Text
                lblProductCode.Text = strPrdCode
                If strData.Equals("") Or strPrdCode.Equals(strData) Then
                Else
                    EXSessionMgr.GetInstance().HandleScanData(strData, BCType.ManualEntry)
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

    Private Sub objStatusBar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
End Class