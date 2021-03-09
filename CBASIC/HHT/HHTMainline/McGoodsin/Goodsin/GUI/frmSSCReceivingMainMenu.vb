Public Class frmSSCReceivingMainMenu
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblRecv As System.Windows.Forms.Label
    Friend WithEvents lblSSCRecv As System.Windows.Forms.Label

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
    Friend WithEvents lblNo1 As System.Windows.Forms.Label
    Friend WithEvents lblNo2 As System.Windows.Forms.Label
    Friend WithEvents lblNo3 As System.Windows.Forms.Label
    Friend WithEvents pnlSSC As System.Windows.Forms.Panel
    Friend WithEvents lblBookIn As System.Windows.Forms.Label
    Friend WithEvents lblbook As System.Windows.Forms.Label
    Friend WithEvents pnlAudit As System.Windows.Forms.Panel
    Friend WithEvents pnlView As System.Windows.Forms.Panel
    Friend WithEvents lblAuditUOD As System.Windows.Forms.Label
    Friend WithEvents lblView As System.Windows.Forms.Label
    Friend WithEvents lblSelect As System.Windows.Forms.Label
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents btn_Info As System.Windows.Forms.PictureBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSSCReceivingMainMenu))
        Me.lblSSCRecv = New System.Windows.Forms.Label
        Me.lblRecv = New System.Windows.Forms.Label
        Me.lblNo1 = New System.Windows.Forms.Label
        Me.lblNo2 = New System.Windows.Forms.Label
        Me.lblNo3 = New System.Windows.Forms.Label
        Me.pnlSSC = New System.Windows.Forms.Panel
        Me.lblbook = New System.Windows.Forms.Label
        Me.lblBookIn = New System.Windows.Forms.Label
        Me.pnlAudit = New System.Windows.Forms.Panel
        Me.lblAuditUOD = New System.Windows.Forms.Label
        Me.pnlView = New System.Windows.Forms.Panel
        Me.lblView = New System.Windows.Forms.Label
        Me.lblSelect = New System.Windows.Forms.Label
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.btn_Info = New System.Windows.Forms.PictureBox
        Me.pnlSSC.SuspendLayout()
        Me.pnlAudit.SuspendLayout()
        Me.pnlView.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblSSCRecv
        '
        Me.lblSSCRecv.Location = New System.Drawing.Point(32, 24)
        Me.lblSSCRecv.Name = "lblSSCRecv"
        Me.lblSSCRecv.Size = New System.Drawing.Size(128, 20)
        Me.lblSSCRecv.Text = "Stores Service Centre "
        '
        'lblRecv
        '
        Me.lblRecv.Location = New System.Drawing.Point(32, 40)
        Me.lblRecv.Name = "lblRecv"
        Me.lblRecv.Size = New System.Drawing.Size(100, 20)
        Me.lblRecv.Text = "Receiving"
        '
        'lblNo1
        '
        Me.lblNo1.Location = New System.Drawing.Point(32, 80)
        Me.lblNo1.Name = "lblNo1"
        Me.lblNo1.Size = New System.Drawing.Size(24, 16)
        Me.lblNo1.Text = "1."
        '
        'lblNo2
        '
        Me.lblNo2.Location = New System.Drawing.Point(32, 112)
        Me.lblNo2.Name = "lblNo2"
        Me.lblNo2.Size = New System.Drawing.Size(24, 16)
        Me.lblNo2.Text = "2."
        '
        'lblNo3
        '
        Me.lblNo3.Location = New System.Drawing.Point(32, 144)
        Me.lblNo3.Name = "lblNo3"
        Me.lblNo3.Size = New System.Drawing.Size(24, 16)
        Me.lblNo3.Text = "3."
        '
        'pnlSSC
        '
        Me.pnlSSC.Controls.Add(Me.lblbook)
        Me.pnlSSC.Location = New System.Drawing.Point(48, 72)
        Me.pnlSSC.Name = "pnlSSC"
        Me.pnlSSC.Size = New System.Drawing.Size(112, 24)
        '
        'lblbook
        '
        Me.lblbook.Location = New System.Drawing.Point(8, 8)
        Me.lblbook.Name = "lblbook"
        Me.lblbook.Size = New System.Drawing.Size(96, 16)
        Me.lblbook.Text = "Book In Delivery"
        '
        'lblBookIn
        '
        Me.lblBookIn.Location = New System.Drawing.Point(48, 80)
        Me.lblBookIn.Name = "lblBookIn"
        Me.lblBookIn.Size = New System.Drawing.Size(96, 16)
        Me.lblBookIn.Text = "Book In Delivery"
        '
        'pnlAudit
        '
        Me.pnlAudit.Controls.Add(Me.lblAuditUOD)
        Me.pnlAudit.Location = New System.Drawing.Point(48, 104)
        Me.pnlAudit.Name = "pnlAudit"
        Me.pnlAudit.Size = New System.Drawing.Size(80, 24)
        '
        'lblAuditUOD
        '
        Me.lblAuditUOD.Location = New System.Drawing.Point(8, 8)
        Me.lblAuditUOD.Name = "lblAuditUOD"
        Me.lblAuditUOD.Size = New System.Drawing.Size(64, 16)
        Me.lblAuditUOD.Text = "Audit UOD"
        '
        'pnlView
        '
        Me.pnlView.Controls.Add(Me.lblView)
        Me.pnlView.Location = New System.Drawing.Point(48, 136)
        Me.pnlView.Name = "pnlView"
        Me.pnlView.Size = New System.Drawing.Size(80, 24)
        '
        'lblView
        '
        Me.lblView.Location = New System.Drawing.Point(8, 8)
        Me.lblView.Name = "lblView"
        Me.lblView.Size = New System.Drawing.Size(64, 16)
        Me.lblView.Text = "View UOD"
        '
        'lblSelect
        '
        Me.lblSelect.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelect.Location = New System.Drawing.Point(40, 176)
        Me.lblSelect.Name = "lblSelect"
        Me.lblSelect.Size = New System.Drawing.Size(120, 20)
        Me.lblSelect.Text = "Select from list"
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(160, 200)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'btn_Info
        '
        Me.btn_Info.Image = CType(resources.GetObject("btn_Info.Image"), System.Drawing.Image)
        Me.btn_Info.Location = New System.Drawing.Point(184, 16)
        Me.btn_Info.Name = "btn_Info"
        Me.btn_Info.Size = New System.Drawing.Size(32, 32)
        Me.btn_Info.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmSSCReceivingMainMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(238, 267)
        Me.ControlBox = False
        Me.Controls.Add(Me.btn_Info)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.lblSelect)
        Me.Controls.Add(Me.pnlView)
        Me.Controls.Add(Me.pnlAudit)
        Me.Controls.Add(Me.pnlSSC)
        Me.Controls.Add(Me.lblNo3)
        Me.Controls.Add(Me.lblNo2)
        Me.Controls.Add(Me.lblNo1)
        Me.Controls.Add(Me.lblRecv)
        Me.Controls.Add(Me.lblSSCRecv)
        Me.Controls.Add(Me.lblBookIn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmSSCReceivingMainMenu"
        Me.Text = "Goods In"
        Me.pnlSSC.ResumeLayout(False)
        Me.pnlAudit.ResumeLayout(False)
        Me.pnlView.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub pnlAudit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlAudit.Click
        FreezeControls()
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        AUODSessionManager.GetInstance().StartSession()
        Me.Hide()
        UnFreezeControls()
    End Sub

    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        Dim iResult As Integer
        FreezeControls()

        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M79"), "Confirmation", MessageBoxButtons.YesNo, _
                                  MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        If iResult = MsgBoxResult.Yes Then
            UnFreezeControls()
            Me.Hide()
        Else
            UnFreezeControls()
        End If
    End Sub

    Private Sub pnlSSC_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlSSC.Click
        FreezeControls()
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        BDSessionMgr.GetInstance().StartSession()
        If Not BDSessionMgr.GetInstance().bReconnected Then
            Me.Hide()
            BDSessionMgr.GetInstance().bReconnected = True
        End If

        UnFreezeControls()
    End Sub

    Private Sub pnlView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlView.Click
        FreezeControls()
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        VUODSessionMgr.GetInstance.StartSession()
        Me.Hide()
        UnFreezeControls()
    End Sub

    Private Sub btn_Info_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Info.Click
        FreezeControls()
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        MessageBox.Show(MessageManager.GetInstance().GetMessage("M82"), "Help")
        UnFreezeControls()
    End Sub
    Public Sub FreezeControls()
        RemoveHandler pnlAudit.Click, AddressOf pnlAudit_Click
        RemoveHandler pnlSSC.Click, AddressOf pnlSSC_Click
        RemoveHandler pnlView.Click, AddressOf pnlView_Click
        Me.btn_Info.Enabled = False
        Me.Btn_Quit_small1.Enabled = False

    End Sub
    Public Sub UnFreezeControls()
        Application.DoEvents()
        AddHandler pnlAudit.Click, AddressOf pnlAudit_Click
        AddHandler pnlSSC.Click, AddressOf pnlSSC_Click
        AddHandler pnlView.Click, AddressOf pnlView_Click
        Me.btn_Info.Enabled = True
        Me.Btn_Quit_small1.Enabled = True
    End Sub
End Class
