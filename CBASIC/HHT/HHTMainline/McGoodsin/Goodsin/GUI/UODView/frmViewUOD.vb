Public Class frmViewUOD
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblMsg As System.Windows.Forms.Label
    Friend WithEvents lblUOD As System.Windows.Forms.Label

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        Me.lvwUOD.Activation = ItemActivation.OneClick
        Me.lvwUOD.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular)
        'Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents lvwUOD As System.Windows.Forms.ListView
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Info1 As System.Windows.Forms.PictureBox
    Friend WithEvents txtProductCode As System.Windows.Forms.TextBox
    Friend WithEvents Btn_CalcPad_small1 As System.Windows.Forms.PictureBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmViewUOD))
        Me.lblUOD = New System.Windows.Forms.Label
        Me.Btn_Info1 = New System.Windows.Forms.PictureBox
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.lvwUOD = New System.Windows.Forms.ListView
        Me.lblMsg = New System.Windows.Forms.Label
        Me.txtProductCode = New System.Windows.Forms.TextBox
        Me.Btn_CalcPad_small1 = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'lblUOD
        '
        Me.lblUOD.Location = New System.Drawing.Point(16, 24)
        Me.lblUOD.Name = "lblUOD"
        Me.lblUOD.Size = New System.Drawing.Size(100, 20)
        Me.lblUOD.Text = "UODs"
        '
        'Btn_Info1
        '
        Me.Btn_Info1.Image = CType(resources.GetObject("Btn_Info1.Image"), System.Drawing.Image)
        Me.Btn_Info1.Location = New System.Drawing.Point(192, 12)
        Me.Btn_Info1.Name = "Btn_Info1"
        Me.Btn_Info1.Size = New System.Drawing.Size(32, 32)
        Me.Btn_Info1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(168, 232)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lvwUOD
        '
        Me.lvwUOD.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lvwUOD.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lvwUOD.Location = New System.Drawing.Point(16, 52)
        Me.lvwUOD.Name = "lvwUOD"
        Me.lvwUOD.Size = New System.Drawing.Size(210, 106)
        Me.lvwUOD.TabIndex = 3
        Me.lvwUOD.View = System.Windows.Forms.View.Details
        '
        'lblMsg
        '
        Me.lblMsg.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg.Location = New System.Drawing.Point(16, 162)
        Me.lblMsg.Name = "lblMsg"
        Me.lblMsg.Size = New System.Drawing.Size(128, 28)
        Me.lblMsg.Text = "Scan /Enter UOD or Select from list"
        '
        'txtProductCode
        '
        Me.txtProductCode.Location = New System.Drawing.Point(20, 200)
        Me.txtProductCode.Name = "txtProductCode"
        Me.txtProductCode.Size = New System.Drawing.Size(160, 21)
        Me.txtProductCode.TabIndex = 0
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.Image = CType(resources.GetObject("Btn_CalcPad_small1.Image"), System.Drawing.Image)
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(196, 198)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(20, 23)
        Me.Btn_CalcPad_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmViewUOD
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.txtProductCode)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.lblMsg)
        Me.Controls.Add(Me.lvwUOD)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Btn_Info1)
        Me.Controls.Add(Me.lblUOD)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmViewUOD"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        VUODSessionMgr.GetInstance.QuitSession()
    End Sub
    Private Sub lvwUOD_ItemActivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvwUOD.ItemActivate
        VUODSessionMgr.GetInstance.DisplayContainerInfo()

        If VUODSessionMgr.GetInstance().m_UODSelectionCheck = "Y" Then
            Me.Visible = False
            VUODSessionMgr.GetInstance().m_UODSelectionCheck = Nothing
        End If
    End Sub

    Private Sub frmViewUOD_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StartRead()
        Me.Focus()

    End Sub

    Private Sub frmViewUOD_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
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
            VUODSessionMgr.GetInstance().HandleScanData(txtProductCode.Text, BCType.ManualEntry)
        End If
#End If
        UnFreezeControls()
    End Sub

    Private Sub Btn_Info1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Info1.Click
        FreezeControls()
        MessageBox.Show(MessageManager.GetInstance().GetMessage("M69"), "Help")
        UnFreezeControls()
    End Sub
    Public Sub FreezeControls()
        Me.lvwUOD.Enabled = False
        Me.txtProductCode.Enabled = False
        Me.Btn_Info1.Enabled = False
        Me.Btn_Quit_small1.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.lvwUOD.Enabled = True
        Me.txtProductCode.Enabled = True
        Me.Btn_Info1.Enabled = True
        Me.Btn_Quit_small1.Enabled = True
    End Sub
End Class
