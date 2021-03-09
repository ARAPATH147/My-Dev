Public Class frmAuditCartonItem
    Inherits System.Windows.Forms.Form
    Public WithEvents lblMsg2 As System.Windows.Forms.Label
    Friend WithEvents lblMsg1 As System.Windows.Forms.Label
    Friend WithEvents txtProductCode As System.Windows.Forms.TextBox
    Friend WithEvents lblAuditCarton As System.Windows.Forms.Label
    Friend WithEvents lblItem As System.Windows.Forms.Label

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
    Friend WithEvents Btn_Finish As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Quit_small As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_CalcPad_small1 As System.Windows.Forms.PictureBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAuditCartonItem))
        Me.Btn_Quit_small = New System.Windows.Forms.PictureBox
        Me.lblMsg2 = New System.Windows.Forms.Label
        Me.lblMsg1 = New System.Windows.Forms.Label
        Me.txtProductCode = New System.Windows.Forms.TextBox
        Me.Btn_CalcPad_small1 = New System.Windows.Forms.PictureBox
        Me.lblAuditCarton = New System.Windows.Forms.Label
        Me.lblItem = New System.Windows.Forms.Label
        Me.Btn_Finish = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'Btn_Quit_small
        '
        Me.Btn_Quit_small.Image = CType(resources.GetObject("Btn_Quit_small.Image"), System.Drawing.Image)
        Me.Btn_Quit_small.Location = New System.Drawing.Point(160, 216)
        Me.Btn_Quit_small.Name = "Btn_Quit_small"
        Me.Btn_Quit_small.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblMsg2
        '
        Me.lblMsg2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg2.Location = New System.Drawing.Point(20, 120)
        Me.lblMsg2.Name = "lblMsg2"
        Me.lblMsg2.Size = New System.Drawing.Size(100, 16)
        Me.lblMsg2.Text = "Item"
        '
        'lblMsg1
        '
        Me.lblMsg1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg1.Location = New System.Drawing.Point(20, 104)
        Me.lblMsg1.Name = "lblMsg1"
        Me.lblMsg1.Size = New System.Drawing.Size(100, 16)
        Me.lblMsg1.Text = "Scan/ Enter"
        '
        'txtProductCode
        '
        Me.txtProductCode.Location = New System.Drawing.Point(64, 48)
        Me.txtProductCode.Name = "txtProductCode"
        Me.txtProductCode.Size = New System.Drawing.Size(128, 21)
        Me.txtProductCode.TabIndex = 5
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.Image = CType(resources.GetObject("Btn_CalcPad_small1.Image"), System.Drawing.Image)
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(196, 48)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(20, 23)
        Me.Btn_CalcPad_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblAuditCarton
        '
        Me.lblAuditCarton.Location = New System.Drawing.Point(20, 24)
        Me.lblAuditCarton.Name = "lblAuditCarton"
        Me.lblAuditCarton.Size = New System.Drawing.Size(100, 20)
        Me.lblAuditCarton.Text = "Audit Carton"
        '
        'lblItem
        '
        Me.lblItem.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblItem.Location = New System.Drawing.Point(24, 56)
        Me.lblItem.Name = "lblItem"
        Me.lblItem.Size = New System.Drawing.Size(32, 20)
        Me.lblItem.Text = "Item"
        '
        'Btn_Finish
        '
        Me.Btn_Finish.Image = CType(resources.GetObject("Btn_Finish.Image"), System.Drawing.Image)
        Me.Btn_Finish.Location = New System.Drawing.Point(24, 216)
        Me.Btn_Finish.Name = "Btn_Finish"
        Me.Btn_Finish.Size = New System.Drawing.Size(65, 24)
        Me.Btn_Finish.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmAuditCartonItem
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Finish)
        Me.Controls.Add(Me.lblItem)
        Me.Controls.Add(Me.Btn_Quit_small)
        Me.Controls.Add(Me.lblMsg2)
        Me.Controls.Add(Me.lblMsg1)
        Me.Controls.Add(Me.txtProductCode)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.lblAuditCarton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmAuditCartonItem"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub Btn_Quit_small_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small.Click
        'Quitting the session without saving
        ACSessionManager.GetInstance().QuitBeforeCommit()
    End Sub

    Private Sub Btn_Finish_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Finish.Click
        'FreezeControls()
        'Finishing the session
        'To show the message 'Dock and Transmit'
        ACSessionManager.GetInstance().m_bShowMsg = True

        ACSessionManager.GetInstance().FinishSession()
        'UnFreezeControls()
    End Sub

    Private Sub frmAuditCartonItem_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StartRead()
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.AUDITCARTONITEM
    End Sub

    Private Sub frmAuditCartonItem_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
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
            ACSessionManager.GetInstance().HandleScanData(txtProductCode.Text, BCType.ManualEntry)
        End If
#End If
        UnFreezeControls()
    End Sub
    Public Sub FreezeControls()
        Me.Btn_Quit_small.Enabled = False
        Me.Btn_Finish.Enabled = False
        Me.txtProductCode.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.Btn_Quit_small.Enabled = True
        Me.Btn_Finish.Enabled = True
        Me.txtProductCode.Enabled = True
    End Sub
End Class
