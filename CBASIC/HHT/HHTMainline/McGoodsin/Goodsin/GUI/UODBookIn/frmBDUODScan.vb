Public Class frmBDUODScan
    Inherits System.Windows.Forms.Form
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
    Friend WithEvents lblMsg1 As System.Windows.Forms.Label
    Friend WithEvents lblMsg2 As System.Windows.Forms.Label
    Friend WithEvents txtProductCode As System.Windows.Forms.TextBox
    Friend WithEvents Btn_Finish1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_CalcPad_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblType As System.Windows.Forms.Label
    Friend WithEvents lblScanned As System.Windows.Forms.Label
    Friend WithEvents lblRemaining As System.Windows.Forms.Label
    Friend WithEvents lstUODScan As System.Windows.Forms.ListBox
    Friend WithEvents Btn_Info As System.Windows.Forms.PictureBox
    Friend WithEvents lblUOD As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBDUODScan))
        Me.lblBookIn = New System.Windows.Forms.Label
        Me.lblMsg1 = New System.Windows.Forms.Label
        Me.lblMsg2 = New System.Windows.Forms.Label
        Me.Btn_Finish1 = New System.Windows.Forms.PictureBox
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.txtProductCode = New System.Windows.Forms.TextBox
        Me.Btn_CalcPad_small1 = New System.Windows.Forms.PictureBox
        Me.lstUODScan = New System.Windows.Forms.ListBox
        Me.lblType = New System.Windows.Forms.Label
        Me.lblScanned = New System.Windows.Forms.Label
        Me.lblRemaining = New System.Windows.Forms.Label
        Me.Btn_Info = New System.Windows.Forms.PictureBox
        Me.lblUOD = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblBookIn
        '
        Me.lblBookIn.Location = New System.Drawing.Point(24, 20)
        Me.lblBookIn.Name = "lblBookIn"
        Me.lblBookIn.Size = New System.Drawing.Size(160, 16)
        Me.lblBookIn.Text = "Book in Delivery"
        '
        'lblMsg1
        '
        Me.lblMsg1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg1.Location = New System.Drawing.Point(24, 72)
        Me.lblMsg1.Name = "lblMsg1"
        Me.lblMsg1.Size = New System.Drawing.Size(100, 16)
        Me.lblMsg1.Text = "Scan/ Enter"
        '
        'lblMsg2
        '
        Me.lblMsg2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg2.Location = New System.Drawing.Point(24, 88)
        Me.lblMsg2.Name = "lblMsg2"
        Me.lblMsg2.Size = New System.Drawing.Size(100, 16)
        Me.lblMsg2.Text = "UOD Barcode"
        '
        'Btn_Finish1
        '
        Me.Btn_Finish1.Image = CType(resources.GetObject("Btn_Finish1.Image"), System.Drawing.Image)
        Me.Btn_Finish1.Location = New System.Drawing.Point(24, 216)
        Me.Btn_Finish1.Name = "Btn_Finish1"
        Me.Btn_Finish1.Size = New System.Drawing.Size(65, 24)
        Me.Btn_Finish1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(168, 216)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'txtProductCode
        '
        Me.txtProductCode.Location = New System.Drawing.Point(64, 48)
        Me.txtProductCode.Name = "txtProductCode"
        Me.txtProductCode.Size = New System.Drawing.Size(128, 21)
        Me.txtProductCode.TabIndex = 6
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.Image = CType(resources.GetObject("Btn_CalcPad_small1.Image"), System.Drawing.Image)
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(200, 48)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(20, 23)
        Me.Btn_CalcPad_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lstUODScan
        '
        Me.lstUODScan.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular)
        Me.lstUODScan.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lstUODScan.Location = New System.Drawing.Point(24, 128)
        Me.lstUODScan.Name = "lstUODScan"
        Me.lstUODScan.Size = New System.Drawing.Size(192, 72)
        Me.lstUODScan.TabIndex = 5
        '
        'lblType
        '
        Me.lblType.Location = New System.Drawing.Point(24, 104)
        Me.lblType.Name = "lblType"
        Me.lblType.Size = New System.Drawing.Size(56, 16)
        Me.lblType.Text = "Type"
        '
        'lblScanned
        '
        Me.lblScanned.Location = New System.Drawing.Point(104, 104)
        Me.lblScanned.Name = "lblScanned"
        Me.lblScanned.Size = New System.Drawing.Size(56, 16)
        Me.lblScanned.Text = "Scanned"
        '
        'lblRemaining
        '
        Me.lblRemaining.Location = New System.Drawing.Point(160, 104)
        Me.lblRemaining.Name = "lblRemaining"
        Me.lblRemaining.Size = New System.Drawing.Size(64, 16)
        Me.lblRemaining.Text = "Remaining"
        '
        'Btn_Info
        '
        Me.Btn_Info.Image = CType(resources.GetObject("Btn_Info.Image"), System.Drawing.Image)
        Me.Btn_Info.Location = New System.Drawing.Point(192, 8)
        Me.Btn_Info.Name = "Btn_Info"
        Me.Btn_Info.Size = New System.Drawing.Size(32, 32)
        Me.Btn_Info.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblUOD
        '
        Me.lblUOD.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblUOD.Location = New System.Drawing.Point(24, 52)
        Me.lblUOD.Name = "lblUOD"
        Me.lblUOD.Size = New System.Drawing.Size(35, 15)
        Me.lblUOD.Text = "UOD"
        '
        'frmBDUODScan
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblUOD)
        Me.Controls.Add(Me.Btn_Info)
        Me.Controls.Add(Me.lblRemaining)
        Me.Controls.Add(Me.lblScanned)
        Me.Controls.Add(Me.lblType)
        Me.Controls.Add(Me.lstUODScan)
        Me.Controls.Add(Me.txtProductCode)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.lblMsg2)
        Me.Controls.Add(Me.lblMsg1)
        Me.Controls.Add(Me.lblBookIn)
        Me.Controls.Add(Me.Btn_Finish1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmBDUODScan"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub Btn_Finish1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Finish1.Click

        FreezeControls()
        BDSessionMgr.GetInstance().bIsFinished = True
        If BDSessionMgr.GetInstance().m_arrListBatch.Count = 0 And BDSessionMgr.GetInstance().m_arrDallasConfirmedUOD.Count = 0 Then
            BDSessionMgr.GetInstance().DisplayBDScreen(BDSessionMgr.BDSCREENS.BDUODFinalDrvrCnfrm)
        Else
            BDSessionMgr.GetInstance().DisplayBDScreen(BDSessionMgr.BDSCREENS.BDUODBatchDrvrConfrm)
        End If
        UnfreezeControls()
    End Sub

    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        Dim iResult As Integer = 0
        FreezeControls()
        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M48"), "Confirmation", MessageBoxButtons.YesNo, _
                                  MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)

        'When quit is pressed all the data need to be purged.. and information of quit need to be send to EPOS ---------------------------------
        If iResult = MsgBoxResult.Yes Then
            BDSessionMgr.GetInstance().DisplayBDScreen(BDSessionMgr.BDSCREENS.BDUODSummary)
        End If
        UnfreezeControls()

    End Sub

    Private Sub frmBDUODScan_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StartRead()
    End Sub

    Private Sub frmBDUODScan_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
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
        UnfreezeControls()
    End Sub
    Private Sub Btn_Info_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Info.Click
        FreezeControls()
        MessageBox.Show(MessageManager.GetInstance().GetMessage("M58"), "Help")

        UnFreezeControls()
    End Sub

    Public Sub FreezeControls()
        Me.Btn_Finish1.Enabled = False
        Me.Btn_Info.Enabled = False
        Me.Btn_Quit_small1.Enabled = False
        Me.Btn_CalcPad_small1.Enabled = False
    End Sub
    Public Sub UnfreezeControls()
        Me.Btn_Finish1.Enabled = True
        Me.Btn_Info.Enabled = True
        Me.Btn_Quit_small1.Enabled = True
        Me.Btn_CalcPad_small1.Enabled = True
    End Sub

End Class
