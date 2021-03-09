Public Class frmAuditCarton
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblMsg1 As System.Windows.Forms.Label
    Friend WithEvents txtProductCode As System.Windows.Forms.TextBox
    Friend WithEvents lblAuditCarton As System.Windows.Forms.Label

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
    Public WithEvents lblMsg2 As System.Windows.Forms.Label
    Friend WithEvents Btn_Quit_small As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_CalcPad_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Info As System.Windows.Forms.PictureBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAuditCarton))
        Me.Btn_Quit_small = New System.Windows.Forms.PictureBox
        Me.lblMsg2 = New System.Windows.Forms.Label
        Me.lblMsg1 = New System.Windows.Forms.Label
        Me.txtProductCode = New System.Windows.Forms.TextBox
        Me.Btn_CalcPad_small1 = New System.Windows.Forms.PictureBox
        Me.lblAuditCarton = New System.Windows.Forms.Label
        Me.Btn_Info = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'Btn_Quit_small
        '
        Me.Btn_Quit_small.Image = CType(resources.GetObject("Btn_Quit_small.Image"), System.Drawing.Image)
        Me.Btn_Quit_small.Location = New System.Drawing.Point(156, 211)
        Me.Btn_Quit_small.Name = "Btn_Quit_small"
        Me.Btn_Quit_small.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblMsg2
        '
        Me.lblMsg2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg2.Location = New System.Drawing.Point(20, 131)
        Me.lblMsg2.Name = "lblMsg2"
        Me.lblMsg2.Size = New System.Drawing.Size(100, 20)
        Me.lblMsg2.Text = "Carton Barcode"
        '
        'lblMsg1
        '
        Me.lblMsg1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg1.Location = New System.Drawing.Point(20, 115)
        Me.lblMsg1.Name = "lblMsg1"
        Me.lblMsg1.Size = New System.Drawing.Size(100, 20)
        Me.lblMsg1.Text = "Scan/ Enter"
        '
        'txtProductCode
        '
        Me.txtProductCode.Location = New System.Drawing.Point(20, 67)
        Me.txtProductCode.Name = "txtProductCode"
        Me.txtProductCode.Size = New System.Drawing.Size(160, 21)
        Me.txtProductCode.TabIndex = 4
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.Image = CType(resources.GetObject("Btn_CalcPad_small1.Image"), System.Drawing.Image)
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(192, 67)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(20, 23)
        Me.Btn_CalcPad_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblAuditCarton
        '
        Me.lblAuditCarton.Location = New System.Drawing.Point(20, 35)
        Me.lblAuditCarton.Name = "lblAuditCarton"
        Me.lblAuditCarton.Size = New System.Drawing.Size(100, 20)
        Me.lblAuditCarton.Text = "Audit Carton"
        '
        'Btn_Info
        '
        Me.Btn_Info.Image = CType(resources.GetObject("Btn_Info.Image"), System.Drawing.Image)
        Me.Btn_Info.Location = New System.Drawing.Point(184, 16)
        Me.Btn_Info.Name = "Btn_Info"
        Me.Btn_Info.Size = New System.Drawing.Size(32, 32)
        Me.Btn_Info.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmAuditCarton
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Info)
        Me.Controls.Add(Me.Btn_Quit_small)
        Me.Controls.Add(Me.lblMsg2)
        Me.Controls.Add(Me.lblMsg1)
        Me.Controls.Add(Me.txtProductCode)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.lblAuditCarton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmAuditCarton"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub Btn_Quit_small_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small.Click
        'Quits the session and goes to GoodsIn main menu
        ACSessionManager.GetInstance().QuitSession()
    End Sub
    Private Sub frmAuditCarton_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StartRead()
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.AUDITCARTON
    End Sub

    Private Sub frmAuditCarton_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub
    Private Sub Btn_Info_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Info.Click
        'Shows the help information
        FreezeControls()
        MessageBox.Show(MessageManager.GetInstance().GetMessage("M72"), "Help")
        UnFreezeControls()
    End Sub
    Private Sub Btn_CalcPad_small1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        FreezeControls()
#If RF Then
        objAppContainer.m_ModScreen = AppContainer.ModScreen.CARTONSCAN
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
        CalcPadSessionMgr.GetInstance().StartSession(txtProductCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
#ElseIf NRF Then
        Dim objSftKeyPad As New frmCalcPad(txtProductCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        If objSftKeyPad.ShowDialog() = Windows.Forms.DialogResult.OK Then
            ACSessionManager.GetInstance().HandleScanData(txtProductCode.Text, BCType.ManualEntry)
        End If
#End If
        UnFreezeControls()
    End Sub
    Public Sub FreezeControls()
        Me.Btn_Quit_small.Enabled = False
        Me.Btn_Info.Enabled = False
        Me.txtProductCode.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.Btn_Quit_small.Enabled = True
        Me.Btn_Info.Enabled = True
        Me.txtProductCode.Enabled = True
    End Sub

End Class
