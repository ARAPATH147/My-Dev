Public Class frmGInAuditItem
    Inherits System.Windows.Forms.Form
    Friend WithEvents txtProductCode As System.Windows.Forms.TextBox
    Friend WithEvents lblAuditUOD As System.Windows.Forms.Label

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
    Friend WithEvents lblMsg2 As System.Windows.Forms.Label
    Friend WithEvents lblMsg1 As System.Windows.Forms.Label
    Friend WithEvents Btn_Quit_small As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Finish As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_CalcPad_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblUOD As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGInAuditItem))
        Me.lblAuditUOD = New System.Windows.Forms.Label
        Me.txtProductCode = New System.Windows.Forms.TextBox
        Me.Btn_CalcPad_small1 = New System.Windows.Forms.PictureBox
        Me.lblMsg2 = New System.Windows.Forms.Label
        Me.lblMsg1 = New System.Windows.Forms.Label
        Me.Btn_Quit_small = New System.Windows.Forms.PictureBox
        Me.Btn_Finish = New System.Windows.Forms.PictureBox
        Me.lblUOD = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblAuditUOD
        '
        Me.lblAuditUOD.Location = New System.Drawing.Point(24, 16)
        Me.lblAuditUOD.Name = "lblAuditUOD"
        Me.lblAuditUOD.Size = New System.Drawing.Size(100, 16)
        Me.lblAuditUOD.Text = "Audit UOD"
        '
        'txtProductCode
        '
        Me.txtProductCode.Location = New System.Drawing.Point(64, 48)
        Me.txtProductCode.Name = "txtProductCode"
        Me.txtProductCode.Size = New System.Drawing.Size(120, 21)
        Me.txtProductCode.TabIndex = 4
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.Image = CType(resources.GetObject("Btn_CalcPad_small1.Image"), System.Drawing.Image)
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(200, 48)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(20, 23)
        Me.Btn_CalcPad_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblMsg2
        '
        Me.lblMsg2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg2.Location = New System.Drawing.Point(24, 104)
        Me.lblMsg2.Name = "lblMsg2"
        Me.lblMsg2.Size = New System.Drawing.Size(100, 16)
        Me.lblMsg2.Text = "Item"
        '
        'lblMsg1
        '
        Me.lblMsg1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg1.Location = New System.Drawing.Point(24, 88)
        Me.lblMsg1.Name = "lblMsg1"
        Me.lblMsg1.Size = New System.Drawing.Size(100, 16)
        Me.lblMsg1.Text = "Scan/ Enter"
        '
        'Btn_Quit_small
        '
        Me.Btn_Quit_small.Image = CType(resources.GetObject("Btn_Quit_small.Image"), System.Drawing.Image)
        Me.Btn_Quit_small.Location = New System.Drawing.Point(168, 184)
        Me.Btn_Quit_small.Name = "Btn_Quit_small"
        Me.Btn_Quit_small.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Finish
        '
        Me.Btn_Finish.Image = CType(resources.GetObject("Btn_Finish.Image"), System.Drawing.Image)
        Me.Btn_Finish.Location = New System.Drawing.Point(24, 184)
        Me.Btn_Finish.Name = "Btn_Finish"
        Me.Btn_Finish.Size = New System.Drawing.Size(65, 24)
        Me.Btn_Finish.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblUOD
        '
        Me.lblUOD.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblUOD.Location = New System.Drawing.Point(24, 48)
        Me.lblUOD.Name = "lblUOD"
        Me.lblUOD.Size = New System.Drawing.Size(40, 16)
        Me.lblUOD.Text = "Item"
        '
        'frmGInAuditItem
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblUOD)
        Me.Controls.Add(Me.Btn_Quit_small)
        Me.Controls.Add(Me.lblMsg2)
        Me.Controls.Add(Me.lblMsg1)
        Me.Controls.Add(Me.txtProductCode)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.lblAuditUOD)
        Me.Controls.Add(Me.Btn_Finish)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmGInAuditItem"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub Btn_Quit_small_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small.Click
        'Quitting the session before saving
        AUODSessionManager.GetInstance().QuitSession()
    End Sub

    Private Sub Btn_Finish_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Finish.Click
        AUODSessionManager.GetInstance().FinishSession()
    End Sub

    Private Sub frmGInAuditItem_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StartRead()
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINAUDITITEM
    End Sub

    Private Sub frmGInAuditItem_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub
    Private Sub Btn_CalcPad_small1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        FreezeControls()
#If RF Then
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
        CalcPadSessionMgr.GetInstance().StartSession(txtProductCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
#ElseIf NRF Then
        Dim objSftKeyPad As New frmCalcPad(txtProductCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        If objSftKeyPad.ShowDialog() = Windows.Forms.DialogResult.OK Then
            AUODSessionManager.GetInstance().HandleScanData(txtProductCode.Text, BCType.ManualEntry)
        End If
#End If
        UnFreezeControls()
    End Sub
    Public Sub FreezeControls()
        Me.txtProductCode.Enabled = False
        Me.Btn_Finish.Enabled = False
        Me.Btn_Quit_small.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.txtProductCode.Enabled = True
        Me.Btn_Finish.Enabled = True
        Me.Btn_Quit_small.Enabled = True
    End Sub
End Class
