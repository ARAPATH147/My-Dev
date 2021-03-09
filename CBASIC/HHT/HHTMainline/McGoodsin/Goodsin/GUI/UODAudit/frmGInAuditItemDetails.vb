Public Class frmGInAuditItemDetails
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblContainer As System.Windows.Forms.Label
    Friend WithEvents lblPrdt As System.Windows.Forms.Label
    Friend WithEvents lblBootsCode As System.Windows.Forms.Label
    Friend WithEvents lblPrdtCode As System.Windows.Forms.Label
    Friend WithEvents lblUOD As System.Windows.Forms.Label
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
    Friend WithEvents lblEnterQty As System.Windows.Forms.Label
    Friend WithEvents lblQty As System.Windows.Forms.Label
    Friend WithEvents Btn_CalcPad_small As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Quit_small As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Next_small As System.Windows.Forms.PictureBox
    Friend WithEvents lblItem3 As System.Windows.Forms.Label
    Friend WithEvents lblItem2 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGInAuditItemDetails))
        Me.lblAuditUOD = New System.Windows.Forms.Label
        Me.lblContainer = New System.Windows.Forms.Label
        Me.lblPrdt = New System.Windows.Forms.Label
        Me.lblBootsCode = New System.Windows.Forms.Label
        Me.lblPrdtCode = New System.Windows.Forms.Label
        Me.lblUOD = New System.Windows.Forms.Label
        Me.lblEnterQty = New System.Windows.Forms.Label
        Me.lblQty = New System.Windows.Forms.Label
        Me.Btn_CalcPad_small = New System.Windows.Forms.PictureBox
        Me.Btn_Quit_small = New System.Windows.Forms.PictureBox
        Me.Btn_Next_small = New System.Windows.Forms.PictureBox
        Me.lblItem3 = New System.Windows.Forms.Label
        Me.lblItem2 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblAuditUOD
        '
        Me.lblAuditUOD.Location = New System.Drawing.Point(24, 16)
        Me.lblAuditUOD.Name = "lblAuditUOD"
        Me.lblAuditUOD.Size = New System.Drawing.Size(100, 20)
        Me.lblAuditUOD.Text = "Audit UOD"
        '
        'lblContainer
        '
        Me.lblContainer.Location = New System.Drawing.Point(24, 40)
        Me.lblContainer.Name = "lblContainer"
        Me.lblContainer.Size = New System.Drawing.Size(48, 20)
        Me.lblContainer.Text = "Crate"
        '
        'lblPrdt
        '
        Me.lblPrdt.Location = New System.Drawing.Point(24, 64)
        Me.lblPrdt.Name = "lblPrdt"
        Me.lblPrdt.Size = New System.Drawing.Size(200, 16)
        Me.lblPrdt.Text = "Desc1"
        '
        'lblBootsCode
        '
        Me.lblBootsCode.Location = New System.Drawing.Point(24, 136)
        Me.lblBootsCode.Name = "lblBootsCode"
        Me.lblBootsCode.Size = New System.Drawing.Size(152, 20)
        Me.lblBootsCode.Text = "11-40-930"
        '
        'lblPrdtCode
        '
        Me.lblPrdtCode.Location = New System.Drawing.Point(24, 160)
        Me.lblPrdtCode.Name = "lblPrdtCode"
        Me.lblPrdtCode.Size = New System.Drawing.Size(144, 20)
        Me.lblPrdtCode.Text = "5-014697-021021"
        '
        'lblUOD
        '
        Me.lblUOD.Location = New System.Drawing.Point(72, 40)
        Me.lblUOD.Name = "lblUOD"
        Me.lblUOD.Size = New System.Drawing.Size(96, 16)
        Me.lblUOD.Text = "300000000001"
        '
        'lblEnterQty
        '
        Me.lblEnterQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblEnterQty.Location = New System.Drawing.Point(24, 184)
        Me.lblEnterQty.Name = "lblEnterQty"
        Me.lblEnterQty.Size = New System.Drawing.Size(112, 20)
        Me.lblEnterQty.Text = "Enter Quantity"
        '
        'lblQty
        '
        Me.lblQty.Location = New System.Drawing.Point(136, 184)
        Me.lblQty.Name = "lblQty"
        Me.lblQty.Size = New System.Drawing.Size(48, 20)
        Me.lblQty.Text = "1"
        '
        'Btn_CalcPad_small
        '
        Me.Btn_CalcPad_small.Image = CType(resources.GetObject("Btn_CalcPad_small.Image"), System.Drawing.Image)
        Me.Btn_CalcPad_small.Location = New System.Drawing.Point(190, 181)
        Me.Btn_CalcPad_small.Name = "Btn_CalcPad_small"
        Me.Btn_CalcPad_small.Size = New System.Drawing.Size(20, 23)
        Me.Btn_CalcPad_small.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Quit_small
        '
        Me.Btn_Quit_small.Image = CType(resources.GetObject("Btn_Quit_small.Image"), System.Drawing.Image)
        Me.Btn_Quit_small.Location = New System.Drawing.Point(160, 224)
        Me.Btn_Quit_small.Name = "Btn_Quit_small"
        Me.Btn_Quit_small.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Next_small
        '
        Me.Btn_Next_small.Image = CType(resources.GetObject("Btn_Next_small.Image"), System.Drawing.Image)
        Me.Btn_Next_small.Location = New System.Drawing.Point(24, 224)
        Me.Btn_Next_small.Name = "Btn_Next_small"
        Me.Btn_Next_small.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Next_small.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblItem3
        '
        Me.lblItem3.Location = New System.Drawing.Point(24, 110)
        Me.lblItem3.Name = "lblItem3"
        Me.lblItem3.Size = New System.Drawing.Size(200, 16)
        Me.lblItem3.Text = "Desc3"
        '
        'lblItem2
        '
        Me.lblItem2.Location = New System.Drawing.Point(24, 87)
        Me.lblItem2.Name = "lblItem2"
        Me.lblItem2.Size = New System.Drawing.Size(200, 16)
        Me.lblItem2.Text = "Desc2"
        '
        'frmGInAuditItemDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblItem2)
        Me.Controls.Add(Me.lblItem3)
        Me.Controls.Add(Me.Btn_Quit_small)
        Me.Controls.Add(Me.Btn_Next_small)
        Me.Controls.Add(Me.Btn_CalcPad_small)
        Me.Controls.Add(Me.lblQty)
        Me.Controls.Add(Me.lblEnterQty)
        Me.Controls.Add(Me.lblUOD)
        Me.Controls.Add(Me.lblPrdtCode)
        Me.Controls.Add(Me.lblBootsCode)
        Me.Controls.Add(Me.lblPrdt)
        Me.Controls.Add(Me.lblContainer)
        Me.Controls.Add(Me.lblAuditUOD)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmGInAuditItemDetails"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub frmGINAuditItemDetails_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StopRead()
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINAUDITITEMDETAILS
    End Sub
    Private Sub Btn_Next_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Next_small.Click
        'Sets the quantity entered
        FreezeControls()
        AUODSessionManager.GetInstance().SetItemQuantity(lblQty.Text)
        AUODSessionManager.GetInstance().DisplayAUODScreen(AUODSessionManager.AUODSCREENS.AuditItem)
        UnFreezeControls()
    End Sub

    Private Sub Btn_Quit_small_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small.Click
        'Quits the session before saving
        AUODSessionManager.GetInstance().QuitSession()
    End Sub

    Private Sub Btn_CalcPad_small_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small.Click
        FreezeControls()
#If RF Then
  Dim iQty As Integer = CInt(lblQty.Text)

        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
        CalcPadSessionMgr.GetInstance().StartSession(lblQty, CalcPadSessionMgr.EntryTypeEnum.Quantity)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
#ElseIf NRF Then
        Dim iQty As Integer = CInt(lblQty.Text)
        Dim objSftKeyPad As New frmCalcPad(lblQty, CalcPadSessionMgr.EntryTypeEnum.Quantity)
        If objSftKeyPad.ShowDialog() = Windows.Forms.DialogResult.OK Then
            'Checks whether zero is entered
            If Not objAppContainer.objHelper.ValidateZeroQty(CInt(Me.lblQty.Text)) Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M1"), "Alert ", MessageBoxButtons.OK, _
                                                                                   MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                Me.lblQty.Text = iQty.ToString()
                Me.Refresh()
                'checks whether the quantity has changed
            ElseIf Not iQty = CInt(lblQty.Text) Then
                AUODSessionManager.GetInstance().SetItemQuantity(lblQty.Text)
                AUODSessionManager.GetInstance().DisplayAUODScreen(AUODSessionManager.AUODSCREENS.AuditItem)
            End If
        End If
#End If

        UnFreezeControls()
    End Sub
    Public Sub FreezeControls()
        Me.Btn_CalcPad_small.Enabled = False
        Me.Btn_Next_small.Enabled = False
        Me.Btn_Quit_small.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.Btn_CalcPad_small.Enabled = True
        Me.Btn_Next_small.Enabled = True
        Me.Btn_Quit_small.Enabled = True

    End Sub

End Class
