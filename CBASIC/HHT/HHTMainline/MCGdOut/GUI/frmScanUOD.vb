Imports System.Reflection
Public Class frmScanUOD
    Private SpaceOK As Boolean = False

    Private Sub Btn_VoidItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnVoidItem.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Deleting Item from the list")
            If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.TRANSFERS Then
                GOTransferMgr.GetInstance().VoidItemInfo()
                Me.lblTotalData.Text = GOTransferMgr.GetInstance().GetUODItemCount.ToString
            Else
                GOSessionMgr.GetInstance().VoidItemInfo()
                Me.lblTotalData.Text = GOSessionMgr.GetInstance().GetUODItemCount.ToString
            End If
            'DEFECT FIX - BTCPR00004646 PPC - Goods Out - Returns & Destroy - Faulty - if all items are voided from Items in UOD summary screen return to item scan
            If GOSessionMgr.GetInstance().GetUODItemCount = 0 Then
                If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.DESTROY Then
                    GOSessionMgr.GetInstance().SetAuthorizationID(Nothing)
                End If
                If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.TRANSFERS Then
                    GOTransferMgr.GetInstance().ClearData()
                    GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTransferMgr.GOTRANSFER.Scan)
                Else
                    GOSessionMgr.GetInstance().ClearData()
                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.Scan)
                End If
            End If
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

    Private Sub frmScanUOD_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            'Not enabling scanning 
            If Not (WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.DESTROY _
             Or WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.STOCKTAKE) Then
                'Disabling scanning of product code 
                BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
                BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.EAN13)
                BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.UPCA)
                BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.EAN8)
                'Fix for scanning EAN128 UOD symbology
                'If Not objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PHSLWT Then
                '    BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.EAN128)
                'End If                
                BCReader.GetInstance().StartRead()
            End If
            'To disable UOD scanning
            'Fix for white label
            If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.FIREFLOOD Then
                BCReader.GetInstance().StopRead()
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub frmScanUOD_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        Try
            If Not (WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.DESTROY _
                  Or WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.STOCKTAKE) Then
                'Re-enabling scanning of product code 
                BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
                BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.EAN13)
                BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.UPCA)
                BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.EAN8)
                'Fix for scanning EAN128 UOD symbology
                'If Not objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PHSLWT Then
                '    BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.EAN128)
                'End If
                BCReader.GetInstance().StopRead()
            End If
            'Fix for white label
            If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.FIREFLOOD Then
                BCReader.GetInstance().StartRead()
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub btnCalcpad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCalcpad.Click
        Try
            Dim objSftKeyPad As New frmCalcPad(txtBarcode, CalcPadSessionMgr.EntryTypeEnum.UOD)
            objSftKeyPad.ShowDialog()
            objSftKeyPad.Close()

            If txtBarcode.Text.Trim().Length > 0 Then
                BCReader.GetInstance().RaiseScanEvent(txtBarcode.Text.ToString().Trim(), BCType.UODManualEntry)
                txtBarcode.Text = ""
            End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub txtBarcode_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtBarcode.KeyDown
        If e.KeyValue = Keys.Enter Then
            If txtBarcode.Text.Trim().Length > 0 Then
                BCReader.GetInstance().RaiseScanEvent(txtBarcode.Text.ToString().Trim(), BCType.UODManualEntry)
            End If
        End If
    End Sub


    Private Sub txtBarcode_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtBarcode.KeyPress
        Dim numberFormatInfo As Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat
        Dim decimalSeparator As String = numberFormatInfo.NumberDecimalSeparator
        Dim groupSeparator As String = numberFormatInfo.NumberGroupSeparator
        Dim negativeSign As String = numberFormatInfo.NegativeSign

        Dim keyInput As String = e.KeyChar.ToString()

        If [Char].IsDigit(e.KeyChar) Then
            ' Digits are OK
        ElseIf keyInput.Equals(decimalSeparator) OrElse keyInput.Equals(groupSeparator) OrElse keyInput.Equals(negativeSign) Then
            ' Decimal separator is OK
        ElseIf e.KeyChar = vbBack Then
            ' Backspace key is OK
            '    else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
            '    {
            '     // Let the edit control handle control and alt key combinations
            '    }
        ElseIf Me.SpaceOK AndAlso e.KeyChar = " "c Then

        Else
            ' Consume this invalid key and beep.
            e.Handled = True
        End If

    End Sub
    Public ReadOnly Property IntValue() As Integer
        Get
            Return Int32.Parse(Me.Text)
        End Get
    End Property
    Public ReadOnly Property DecimalValue() As Decimal
        Get
            Return [Decimal].Parse(Me.Text)
        End Get
    End Property
    Public Property AllowSpace() As Boolean

        Get
            Return Me.SpaceOK
        End Get
        Set(ByVal value As Boolean)
            Me.SpaceOK = value
        End Set
    End Property

    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            FreezeControls()
            If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.DESTROY Then
                GOSessionMgr.GetInstance().SetAuthorizationID(Nothing)
            End If
            If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.TRANSFERS Then
                GOTransferMgr.GetInstance().ClearData()
                GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTransferMgr.GOTRANSFER.Scan)
            Else
                GOSessionMgr.GetInstance().ClearData()
                GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.Scan)
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

    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Updating Product Information...")
            Me.Refresh()
            If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.STOCKTAKE Then
                'TODO: Default UOD value for Destroy
                GOSessionMgr.GetInstance().SetUOD("00000000001000")
                'Call the Despatch screen
                GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.GODespatch)
            ElseIf WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.FIREFLOOD Then
                'GOSessionMgr.GetInstance().SetUOD("00000000000000")
                'FIX for DEFECTID-3862 - Destroy Fire/Flood writes uod number of all 0 causing Stock Support to abend
                GOSessionMgr.GetInstance().SetUOD("00000000001000")
                'Call the Despatch screen
                GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.GODespatch)
            Else
                'TODO: Default UOD value for Destroy
                GOSessionMgr.GetInstance().SetUOD("00000000001000")
                'Call the Supplier List screen
                If GOSessionMgr.GetInstance().SupplyRoute = "D" Then
                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.SupplierList)
                Else
                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.GODespatch)
                End If
            End If
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
            Me.btnQuit.Enabled = False
            Me.btnNext.Enabled = False
            Me.btnVoidItem.Enabled = False
            Me.btnCalcpad.Enabled = False
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
            Me.btnNext.Enabled = True
            Me.btnVoidItem.Enabled = True
            Me.btnCalcpad.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Unfreeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class