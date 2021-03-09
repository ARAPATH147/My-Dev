Public Class frmBDUODDrvrConfrmation
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblDrvrConfrm As System.Windows.Forms.Label
    Friend WithEvents lblBookIn As System.Windows.Forms.Label

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents lblConfirmMessage As System.Windows.Forms.Label
    Friend WithEvents txtProductCode As System.Windows.Forms.TextBox
    Friend WithEvents RescanBatchvb1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblMsg As System.Windows.Forms.Label
    Friend WithEvents Btn_CalcPad_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents btnConfirm As System.Windows.Forms.PictureBox
    Friend WithEvents btnNoBadge As System.Windows.Forms.PictureBox
    Friend WithEvents lblStoreConfirmation As System.Windows.Forms.Label
    Friend WithEvents Btn_Info As System.Windows.Forms.PictureBox
    Friend WithEvents lstvwBatch As System.Windows.Forms.ListBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBDUODDrvrConfrmation))
        Me.lblBookIn = New System.Windows.Forms.Label
        Me.lblDrvrConfrm = New System.Windows.Forms.Label
        Me.lblMsg = New System.Windows.Forms.Label
        Me.lblConfirmMessage = New System.Windows.Forms.Label
        Me.btnNoBadge = New System.Windows.Forms.PictureBox
        Me.RescanBatchvb1 = New System.Windows.Forms.PictureBox
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.btnConfirm = New System.Windows.Forms.PictureBox
        Me.txtProductCode = New System.Windows.Forms.TextBox
        Me.Btn_CalcPad_small1 = New System.Windows.Forms.PictureBox
        Me.lblStoreConfirmation = New System.Windows.Forms.Label
        Me.Btn_Info = New System.Windows.Forms.PictureBox
        Me.lstvwBatch = New System.Windows.Forms.ListBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblBookIn
        '
        Me.lblBookIn.Location = New System.Drawing.Point(24, 12)
        Me.lblBookIn.Name = "lblBookIn"
        Me.lblBookIn.Size = New System.Drawing.Size(160, 16)
        Me.lblBookIn.Text = "Book in Delivery"
        '
        'lblDrvrConfrm
        '
        Me.lblDrvrConfrm.Location = New System.Drawing.Point(24, 40)
        Me.lblDrvrConfrm.Name = "lblDrvrConfrm"
        Me.lblDrvrConfrm.Size = New System.Drawing.Size(176, 16)
        Me.lblDrvrConfrm.Text = "Driver Confirmation of Receipt"
        '
        'lblMsg
        '
        Me.lblMsg.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg.Location = New System.Drawing.Point(24, 152)
        Me.lblMsg.Name = "lblMsg"
        Me.lblMsg.Size = New System.Drawing.Size(168, 20)
        Me.lblMsg.Text = "Scan/ Enter Driver Badge "
        '
        'lblConfirmMessage
        '
        Me.lblConfirmMessage.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblConfirmMessage.Location = New System.Drawing.Point(24, 152)
        Me.lblConfirmMessage.Name = "lblConfirmMessage"
        Me.lblConfirmMessage.Size = New System.Drawing.Size(160, 20)
        Me.lblConfirmMessage.Text = "Confirm Batch OK"
        '
        'btnNoBadge
        '
        Me.btnNoBadge.Image = CType(resources.GetObject("btnNoBadge.Image"), System.Drawing.Image)
        Me.btnNoBadge.Location = New System.Drawing.Point(18, 216)
        Me.btnNoBadge.Name = "btnNoBadge"
        Me.btnNoBadge.Size = New System.Drawing.Size(65, 24)
        Me.btnNoBadge.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'RescanBatchvb1
        '
        Me.RescanBatchvb1.Image = CType(resources.GetObject("RescanBatchvb1.Image"), System.Drawing.Image)
        Me.RescanBatchvb1.Location = New System.Drawing.Point(88, 216)
        Me.RescanBatchvb1.Name = "RescanBatchvb1"
        Me.RescanBatchvb1.Size = New System.Drawing.Size(88, 24)
        Me.RescanBatchvb1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(184, 216)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'btnConfirm
        '
        Me.btnConfirm.Image = CType(resources.GetObject("btnConfirm.Image"), System.Drawing.Image)
        Me.btnConfirm.Location = New System.Drawing.Point(18, 216)
        Me.btnConfirm.Name = "btnConfirm"
        Me.btnConfirm.Size = New System.Drawing.Size(65, 24)
        Me.btnConfirm.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'txtProductCode
        '
        Me.txtProductCode.Location = New System.Drawing.Point(24, 176)
        Me.txtProductCode.Name = "txtProductCode"
        Me.txtProductCode.Size = New System.Drawing.Size(160, 21)
        Me.txtProductCode.TabIndex = 3
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.Image = CType(resources.GetObject("Btn_CalcPad_small1.Image"), System.Drawing.Image)
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(196, 174)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(20, 23)
        Me.Btn_CalcPad_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblStoreConfirmation
        '
        Me.lblStoreConfirmation.Location = New System.Drawing.Point(24, 40)
        Me.lblStoreConfirmation.Name = "lblStoreConfirmation"
        Me.lblStoreConfirmation.Size = New System.Drawing.Size(172, 16)
        Me.lblStoreConfirmation.Text = "Store Confirmation of Receipt"
        Me.lblStoreConfirmation.Visible = False
        '
        'Btn_Info
        '
        Me.Btn_Info.Image = CType(resources.GetObject("Btn_Info.Image"), System.Drawing.Image)
        Me.Btn_Info.Location = New System.Drawing.Point(192, 8)
        Me.Btn_Info.Name = "Btn_Info"
        Me.Btn_Info.Size = New System.Drawing.Size(32, 32)
        Me.Btn_Info.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lstvwBatch
        '
        Me.lstvwBatch.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular)
        Me.lstvwBatch.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lstvwBatch.Location = New System.Drawing.Point(24, 80)
        Me.lstvwBatch.Name = "lstvwBatch"
        Me.lstvwBatch.Size = New System.Drawing.Size(192, 58)
        Me.lstvwBatch.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(24, 60)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(100, 16)
        Me.Label1.Text = "Scanned"
        '
        'frmBDUODDrvrConfrmation
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lstvwBatch)
        Me.Controls.Add(Me.Btn_Info)
        Me.Controls.Add(Me.txtProductCode)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.RescanBatchvb1)
        Me.Controls.Add(Me.lblBookIn)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.lblMsg)
        Me.Controls.Add(Me.lblConfirmMessage)
        Me.Controls.Add(Me.lblDrvrConfrm)
        Me.Controls.Add(Me.lblStoreConfirmation)
        Me.Controls.Add(Me.btnConfirm)
        Me.Controls.Add(Me.btnNoBadge)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmBDUODDrvrConfrmation"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub btnNoBadge_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNoBadge.Click
        FreezeControls()
#If RF Then
        objAppContainer.m_ModScreen = AppContainer.ModScreen.DRVRBDGESCAN
        objAppContainer.m_PreviousScreen = AppContainer.ModScreen.DRVRBDGESCAN
        BDSessionMgr.GetInstance().NoBadgeSelect(5)
#ElseIf NRF Then
        If MsgBx.DisplayMessage(MessageManager.GetInstance().GetMessage("M99"), "Alert", MsgBx.BUTTON_TYPE.CONTINE) = MsgBx.BUTTON_VALUE.CONTINUE Then
            BDSessionMgr.GetInstance().NoBadge()
        End If
#End If
        UnFreezeControls()

    End Sub

    Private Sub RescanBatchvb1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RescanBatchvb1.Click
        FreezeControls()
        BDSessionMgr.GetInstance().RescanBatch()
        UnFreezeControls()
    End Sub

    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        'set bool to indicate quit to be used in bookin summary--------------------------------


        Dim iResult As Integer = 0
        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M48"), "Confirmation", MessageBoxButtons.YesNo, _
                                  MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        If iResult = MsgBoxResult.Yes Then
            BDSessionMgr.GetInstance().DisplayBDScreen(BDSessionMgr.BDSCREENS.BDUODSummary)
        End If
    End Sub

    Private Sub btnConfirm_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConfirm.Click
        FreezeControls()
#If RF Then
        objAppContainer.m_ModScreen = AppContainer.ModScreen.DRVRBDGESCAN
        objAppContainer.m_PreviousScreen = AppContainer.ModScreen.DRVRBDGESCAN
#End If
        
        BDSessionMgr.GetInstance().OutOfHoursBatchConfirm()
        UnFreezeControls()
    End Sub

    Private Sub frmDrvrConfrmation_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        If BDSessionMgr.GetInstance().bIsInHours Then
            BCReader.GetInstance().StartRead()
        End If
    End Sub

    Private Sub frmDrvrConfrmation_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        If BDSessionMgr.GetInstance().bIsInHours Then
            BCReader.GetInstance().StopRead()
        End If
    End Sub
    Private Sub Btn_Info_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Info.Click
        FreezeControls()
        If BDSessionMgr.GetInstance().bIsInHours Then
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M59"), "Help")
        Else
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M110"), "Help")
        End If
        UnFreezeControls()

    End Sub
    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        FreezeControls()
#If RF Then
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
        CalcPadSessionMgr.GetInstance().StartSession(txtProductCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
#ElseIf NRF Then
        Dim objSftKeyPad As New frmCalcPad(txtProductCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        If objSftKeyPad.ShowDialog() = Windows.Forms.DialogResult.OK Then
            BDSessionMgr.GetInstance().HandleScanData(txtProductCode.Text, BCType.ManualEntry)
        End If
#End If
        UnFreezeControls()
    End Sub
    Public Sub FreezeControls()
        Me.Btn_Info.Enabled = False
        Me.Btn_Quit_small1.Enabled = False
        Me.RescanBatchvb1.Enabled = False
        Me.txtProductCode.Enabled = False
        Me.lstvwBatch.Enabled = False
        If BDSessionMgr.GetInstance().bIsInHours Then
            Me.btnNoBadge.Enabled = False
        Else
            Me.btnConfirm.Enabled = False
        End If

    End Sub
    Public Sub UnFreezeControls()
        Me.Btn_Info.Enabled = True
        Me.Btn_Quit_small1.Enabled = True
        Me.RescanBatchvb1.Enabled = True
        Me.txtProductCode.Enabled = True
        Me.lstvwBatch.Enabled = True
        If BDSessionMgr.GetInstance().bIsInHours Then
            Me.btnNoBadge.Enabled = True
        Else
            Me.btnConfirm.Enabled = True
        End If
    End Sub
End Class
