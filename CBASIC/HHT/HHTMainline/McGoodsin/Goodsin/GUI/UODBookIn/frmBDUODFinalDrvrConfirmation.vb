Public Class frmBDUODFinalDrvrConfirmation
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblFinalConfrmtn As System.Windows.Forms.Label
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
    Friend WithEvents lblBook As System.Windows.Forms.Label
    Friend WithEvents lblRemaining As System.Windows.Forms.Label
    Friend WithEvents txtProductCode As System.Windows.Forms.TextBox
    Friend WithEvents Btn_Back_sm1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblConfirmMessage As System.Windows.Forms.Label
    Friend WithEvents lblMsg As System.Windows.Forms.Label
    Friend WithEvents Btn_CalcPad_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents btnConfirm As System.Windows.Forms.PictureBox
    Friend WithEvents btnNoBadge As System.Windows.Forms.PictureBox
    Friend WithEvents lblConfirmation As System.Windows.Forms.Label
    Friend WithEvents lvwBookedIn As System.Windows.Forms.ListBox
    Friend WithEvents lvwRemainingToday As System.Windows.Forms.ListBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBDUODFinalDrvrConfirmation))
        Me.lblBookIn = New System.Windows.Forms.Label
        Me.lblFinalConfrmtn = New System.Windows.Forms.Label
        Me.lblBook = New System.Windows.Forms.Label
        Me.lblRemaining = New System.Windows.Forms.Label
        Me.Btn_CalcPad_small1 = New System.Windows.Forms.PictureBox
        Me.lblMsg = New System.Windows.Forms.Label
        Me.Btn_Back_sm1 = New System.Windows.Forms.PictureBox
        Me.btnNoBadge = New System.Windows.Forms.PictureBox
        Me.btnConfirm = New System.Windows.Forms.PictureBox
        Me.lblConfirmMessage = New System.Windows.Forms.Label
        Me.txtProductCode = New System.Windows.Forms.TextBox
        Me.lblConfirmation = New System.Windows.Forms.Label
        Me.lvwBookedIn = New System.Windows.Forms.ListBox
        Me.lvwRemainingToday = New System.Windows.Forms.ListBox
        Me.SuspendLayout()
        '
        'lblBookIn
        '
        Me.lblBookIn.Location = New System.Drawing.Point(24, 8)
        Me.lblBookIn.Name = "lblBookIn"
        Me.lblBookIn.Size = New System.Drawing.Size(160, 16)
        Me.lblBookIn.Text = "Book in Delivery"
        '
        'lblFinalConfrmtn
        '
        Me.lblFinalConfrmtn.Location = New System.Drawing.Point(24, 24)
        Me.lblFinalConfrmtn.Name = "lblFinalConfrmtn"
        Me.lblFinalConfrmtn.Size = New System.Drawing.Size(160, 16)
        Me.lblFinalConfrmtn.Text = "Driver Final Confirmation"
        '
        'lblBook
        '
        Me.lblBook.Location = New System.Drawing.Point(24, 40)
        Me.lblBook.Name = "lblBook"
        Me.lblBook.Size = New System.Drawing.Size(136, 16)
        Me.lblBook.Text = "Booked In"
        '
        'lblRemaining
        '
        Me.lblRemaining.Location = New System.Drawing.Point(24, 114)
        Me.lblRemaining.Name = "lblRemaining"
        Me.lblRemaining.Size = New System.Drawing.Size(136, 16)
        Me.lblRemaining.Text = "Remaining Today"
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.Image = CType(resources.GetObject("Btn_CalcPad_small1.Image"), System.Drawing.Image)
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(192, 200)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(20, 23)
        Me.Btn_CalcPad_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblMsg
        '
        Me.lblMsg.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg.Location = New System.Drawing.Point(24, 184)
        Me.lblMsg.Name = "lblMsg"
        Me.lblMsg.Size = New System.Drawing.Size(176, 20)
        Me.lblMsg.Text = "Scan/ Enter Driver Badge "
        '
        'Btn_Back_sm1
        '
        Me.Btn_Back_sm1.Image = CType(resources.GetObject("Btn_Back_sm1.Image"), System.Drawing.Image)
        Me.Btn_Back_sm1.Location = New System.Drawing.Point(162, 229)
        Me.Btn_Back_sm1.Name = "Btn_Back_sm1"
        Me.Btn_Back_sm1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Back_sm1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'btnNoBadge
        '
        Me.btnNoBadge.Image = CType(resources.GetObject("btnNoBadge.Image"), System.Drawing.Image)
        Me.btnNoBadge.Location = New System.Drawing.Point(24, 229)
        Me.btnNoBadge.Name = "btnNoBadge"
        Me.btnNoBadge.Size = New System.Drawing.Size(65, 24)
        Me.btnNoBadge.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'btnConfirm
        '
        Me.btnConfirm.Image = CType(resources.GetObject("btnConfirm.Image"), System.Drawing.Image)
        Me.btnConfirm.Location = New System.Drawing.Point(24, 227)
        Me.btnConfirm.Name = "btnConfirm"
        Me.btnConfirm.Size = New System.Drawing.Size(65, 24)
        Me.btnConfirm.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblConfirmMessage
        '
        Me.lblConfirmMessage.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblConfirmMessage.Location = New System.Drawing.Point(24, 184)
        Me.lblConfirmMessage.Name = "lblConfirmMessage"
        Me.lblConfirmMessage.Size = New System.Drawing.Size(176, 20)
        Me.lblConfirmMessage.Text = "Confirm Delivery to Finish"
        '
        'txtProductCode
        '
        Me.txtProductCode.Location = New System.Drawing.Point(24, 200)
        Me.txtProductCode.Name = "txtProductCode"
        Me.txtProductCode.Size = New System.Drawing.Size(160, 21)
        Me.txtProductCode.TabIndex = 2
        '
        'lblConfirmation
        '
        Me.lblConfirmation.Location = New System.Drawing.Point(24, 24)
        Me.lblConfirmation.Name = "lblConfirmation"
        Me.lblConfirmation.Size = New System.Drawing.Size(160, 16)
        Me.lblConfirmation.Text = "Final Confirmation"
        Me.lblConfirmation.Visible = False
        '
        'lvwBookedIn
        '
        Me.lvwBookedIn.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular)
        Me.lvwBookedIn.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lvwBookedIn.Location = New System.Drawing.Point(24, 60)
        Me.lvwBookedIn.Name = "lvwBookedIn"
        Me.lvwBookedIn.Size = New System.Drawing.Size(184, 44)
        Me.lvwBookedIn.TabIndex = 1
        '
        'lvwRemainingToday
        '
        Me.lvwRemainingToday.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular)
        Me.lvwRemainingToday.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lvwRemainingToday.Location = New System.Drawing.Point(24, 132)
        Me.lvwRemainingToday.Name = "lvwRemainingToday"
        Me.lvwRemainingToday.Size = New System.Drawing.Size(184, 44)
        Me.lvwRemainingToday.TabIndex = 0
        '
        'frmBDUODFinalDrvrConfirmation
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.lvwRemainingToday)
        Me.Controls.Add(Me.lvwBookedIn)
        Me.Controls.Add(Me.txtProductCode)
        Me.Controls.Add(Me.Btn_Back_sm1)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.lblRemaining)
        Me.Controls.Add(Me.lblBook)
        Me.Controls.Add(Me.lblBookIn)
        Me.Controls.Add(Me.btnNoBadge)
        Me.Controls.Add(Me.btnConfirm)
        Me.Controls.Add(Me.lblConfirmMessage)
        Me.Controls.Add(Me.lblMsg)
        Me.Controls.Add(Me.lblConfirmation)
        Me.Controls.Add(Me.lblFinalConfrmtn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmBDUODFinalDrvrConfirmation"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub btnNoBadge_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNoBadge.Click

#If RF Then
        FreezeControls()
        BDSessionMgr.GetInstance().NoBadgeSelect(6)
        UnFreezeControls()
#ElseIf NRF Then
        If MsgBx.DisplayMessage(MessageManager.GetInstance().GetMessage("M99"), "Alert", MsgBx.BUTTON_TYPE.CONTINE) = MsgBx.BUTTON_VALUE.CONTINUE Then
            BDSessionMgr.GetInstance().NoBadgeSessionConfirm()
        End If
#End If

    End Sub

    Private Sub Btn_Back_sm1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Back_sm1.Click
        FreezeControls()
        BDSessionMgr.GetInstance().bIsFinished = False
        BDSessionMgr.GetInstance().DisplayBDScreen(BDSessionMgr.BDSCREENS.BDUODScan)
        UnFreezeControls()
    End Sub

    Private Sub btnConfirm_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConfirm.Click
        Dim iResult As Integer = 0

        FreezeControls()
        iResult = MessageBox.Show("Complete GIT note. Does delivery match driver's column?", "Confirmation", MessageBoxButtons.YesNo, _
                                       MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        UnFreezeControls()
        If iResult = MsgBoxResult.Yes Then
            BDSessionMgr.GetInstance().bGITNote = True
        Else
            BDSessionMgr.GetInstance().bGITNote = False
        End If
        BDSessionMgr.GetInstance().OutOfHoursFinalConfirm()

    End Sub

    Private Sub frmFinalDrvrConfirmation_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        If BDSessionMgr.GetInstance().bIsInHours Then
            BCReader.GetInstance().StartRead()
        End If
    End Sub

    Private Sub frmFinalDrvrConfirmation_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        If BDSessionMgr.GetInstance().bIsInHours Then
            BCReader.GetInstance().StopRead()
        End If
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
        Else
            UnFreezeControls()
        End If
#End If
    End Sub
    Public Sub FreezeControls()
        Me.Btn_Back_sm1.Enabled = False
        Me.txtProductCode.Enabled = False
        Me.lvwBookedIn.Enabled = False
        Me.lvwRemainingToday.Enabled = False
        If BDSessionMgr.GetInstance().bIsInHours Then
            Me.btnNoBadge.Enabled = False
        Else
            Me.btnConfirm.Enabled = False
        End If

    End Sub
    Public Sub UnFreezeControls()
        Me.Btn_Back_sm1.Enabled = True
        Me.txtProductCode.Enabled = True
        Me.lvwBookedIn.Enabled = True
        Me.lvwRemainingToday.Enabled = True
        If BDSessionMgr.GetInstance().bIsInHours Then
            Me.btnNoBadge.Enabled = True
        Else
            Me.btnConfirm.Enabled = True
        End If
    End Sub
End Class
