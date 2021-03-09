Public Class frmViewUODDolly
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblUODText As System.Windows.Forms.Label
    Friend WithEvents lblUODValue As System.Windows.Forms.Label
    Friend WithEvents lblTypeText As System.Windows.Forms.Label
    Friend WithEvents lblBkdDateValue As System.Windows.Forms.Label
    Friend WithEvents lblTypeValue As System.Windows.Forms.Label
    Friend WithEvents lblStatusText As System.Windows.Forms.Label
    Friend WithEvents lblExptdDateValue As System.Windows.Forms.Label
    Friend WithEvents lblExptdDateText As System.Windows.Forms.Label
    Friend WithEvents lblBkdDateText As System.Windows.Forms.Label
    Friend WithEvents lblMsgSelect As System.Windows.Forms.Label
    Public bDollySelected As Boolean = False
    Public bListChangeEnables As Boolean = False
#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        Me.lvwUODDolly.Activation = ItemActivation.OneClick
        Me.lvwUODDolly.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular)
        'Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents Btn_Back_sm1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Info1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblBkdInValue As System.Windows.Forms.Label
    Public WithEvents lvwUODDolly As System.Windows.Forms.ListView
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmViewUODDolly))
        Me.lblUODText = New System.Windows.Forms.Label
        Me.lblUODValue = New System.Windows.Forms.Label
        Me.lblTypeText = New System.Windows.Forms.Label
        Me.lblBkdDateValue = New System.Windows.Forms.Label
        Me.lblTypeValue = New System.Windows.Forms.Label
        Me.lblStatusText = New System.Windows.Forms.Label
        Me.lblBkdInValue = New System.Windows.Forms.Label
        Me.lblExptdDateValue = New System.Windows.Forms.Label
        Me.lblExptdDateText = New System.Windows.Forms.Label
        Me.lblBkdDateText = New System.Windows.Forms.Label
        Me.lvwUODDolly = New System.Windows.Forms.ListView
        Me.lblMsgSelect = New System.Windows.Forms.Label
        Me.Btn_Back_sm1 = New System.Windows.Forms.PictureBox
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.Btn_Info1 = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'lblUODText
        '
        Me.lblUODText.Location = New System.Drawing.Point(16, 24)
        Me.lblUODText.Name = "lblUODText"
        Me.lblUODText.Size = New System.Drawing.Size(40, 20)
        Me.lblUODText.Text = "UOD"
        '
        'lblUODValue
        '
        Me.lblUODValue.Location = New System.Drawing.Point(56, 24)
        Me.lblUODValue.Name = "lblUODValue"
        Me.lblUODValue.Size = New System.Drawing.Size(112, 16)
        Me.lblUODValue.Text = "Label2"
        '
        'lblTypeText
        '
        Me.lblTypeText.Location = New System.Drawing.Point(16, 48)
        Me.lblTypeText.Name = "lblTypeText"
        Me.lblTypeText.Size = New System.Drawing.Size(40, 16)
        Me.lblTypeText.Text = "Type:"
        '
        'lblBkdDateValue
        '
        Me.lblBkdDateValue.Location = New System.Drawing.Point(160, 72)
        Me.lblBkdDateValue.Name = "lblBkdDateValue"
        Me.lblBkdDateValue.Size = New System.Drawing.Size(64, 16)
        Me.lblBkdDateValue.Text = "Label4"
        '
        'lblTypeValue
        '
        Me.lblTypeValue.Location = New System.Drawing.Point(48, 48)
        Me.lblTypeValue.Name = "lblTypeValue"
        Me.lblTypeValue.Size = New System.Drawing.Size(56, 16)
        Me.lblTypeValue.Text = "Label5"
        '
        'lblStatusText
        '
        Me.lblStatusText.Location = New System.Drawing.Point(125, 48)
        Me.lblStatusText.Name = "lblStatusText"
        Me.lblStatusText.Size = New System.Drawing.Size(51, 16)
        Me.lblStatusText.Text = "Bkd In:"
        '
        'lblBkdInValue
        '
        Me.lblBkdInValue.Location = New System.Drawing.Point(168, 48)
        Me.lblBkdInValue.Name = "lblBkdInValue"
        Me.lblBkdInValue.Size = New System.Drawing.Size(56, 16)
        Me.lblBkdInValue.Text = "Label7"
        '
        'lblExptdDateValue
        '
        Me.lblExptdDateValue.Location = New System.Drawing.Point(56, 72)
        Me.lblExptdDateValue.Name = "lblExptdDateValue"
        Me.lblExptdDateValue.Size = New System.Drawing.Size(62, 16)
        Me.lblExptdDateValue.Text = "Label8"
        '
        'lblExptdDateText
        '
        Me.lblExptdDateText.Location = New System.Drawing.Point(16, 72)
        Me.lblExptdDateText.Name = "lblExptdDateText"
        Me.lblExptdDateText.Size = New System.Drawing.Size(40, 16)
        Me.lblExptdDateText.Text = "Exptd:"
        '
        'lblBkdDateText
        '
        Me.lblBkdDateText.Location = New System.Drawing.Point(128, 72)
        Me.lblBkdDateText.Name = "lblBkdDateText"
        Me.lblBkdDateText.Size = New System.Drawing.Size(32, 16)
        Me.lblBkdDateText.Text = "Bkd:"
        '
        'lvwUODDolly
        '
        Me.lvwUODDolly.BackColor = System.Drawing.Color.White
        Me.lvwUODDolly.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lvwUODDolly.Location = New System.Drawing.Point(16, 96)
        Me.lvwUODDolly.Name = "lvwUODDolly"
        Me.lvwUODDolly.Size = New System.Drawing.Size(208, 88)
        Me.lvwUODDolly.TabIndex = 4
        Me.lvwUODDolly.View = System.Windows.Forms.View.Details
        '
        'lblMsgSelect
        '
        Me.lblMsgSelect.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsgSelect.Location = New System.Drawing.Point(16, 192)
        Me.lblMsgSelect.Name = "lblMsgSelect"
        Me.lblMsgSelect.Size = New System.Drawing.Size(88, 20)
        Me.lblMsgSelect.Text = "Select UOD"
        '
        'Btn_Back_sm1
        '
        Me.Btn_Back_sm1.Image = CType(resources.GetObject("Btn_Back_sm1.Image"), System.Drawing.Image)
        Me.Btn_Back_sm1.Location = New System.Drawing.Point(16, 224)
        Me.Btn_Back_sm1.Name = "Btn_Back_sm1"
        Me.Btn_Back_sm1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Back_sm1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(176, 224)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Info1
        '
        Me.Btn_Info1.Image = CType(resources.GetObject("Btn_Info1.Image"), System.Drawing.Image)
        Me.Btn_Info1.Location = New System.Drawing.Point(192, 12)
        Me.Btn_Info1.Name = "Btn_Info1"
        Me.Btn_Info1.Size = New System.Drawing.Size(32, 32)
        Me.Btn_Info1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmViewUODDolly
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Info1)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Btn_Back_sm1)
        Me.Controls.Add(Me.lblMsgSelect)
        Me.Controls.Add(Me.lvwUODDolly)
        Me.Controls.Add(Me.lblBkdDateText)
        Me.Controls.Add(Me.lblExptdDateText)
        Me.Controls.Add(Me.lblExptdDateValue)
        Me.Controls.Add(Me.lblBkdInValue)
        Me.Controls.Add(Me.lblStatusText)
        Me.Controls.Add(Me.lblTypeValue)
        Me.Controls.Add(Me.lblBkdDateValue)
        Me.Controls.Add(Me.lblTypeText)
        Me.Controls.Add(Me.lblUODValue)
        Me.Controls.Add(Me.lblUODText)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmViewUODDolly"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        VUODSessionMgr.GetInstance.QuitSession()
    End Sub

    Private Sub Btn_Back_sm1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Back_sm1.Click
        FreezeControls()
        VUODSessionMgr.GetInstance.DisplayVUODScreen(VUODSessionMgr.VUODSCREENS.ViewUOD)
        UnFreezeControls()
    End Sub
    'Private Sub lvwUODDolly_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvwUODDolly.SelectedIndexChanged
    '    bDollySelected = True
    '    'Invoke the GetItemList function
    '    'VUODSessionMgr.GetInstance().GetItemList()
    '    VUODSessionMgr.GetInstance.DisplayVUODScreen(VUODSessionMgr.VUODSCREENS.UODNonDolly)
    'End Sub
    Private Sub lvwUODDolly_ItemActivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvwUODDolly.ItemActivate
        bDollySelected = True
#If RF Then
        objAppContainer.m_ModScreen = AppContainer.ModScreen.ITEMSELECT
#End If
        'Invoke the GetItemList function
        VUODSessionMgr.GetInstance.DisplayVUODScreen(VUODSessionMgr.VUODSCREENS.UODNonDolly)

    End Sub

    Private Sub Btn_Info1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Info1.Click
        FreezeControls()
        MessageBox.Show(MessageManager.GetInstance().GetMessage("M70"), "Help")
        UnFreezeControls()
    End Sub
    Private Sub frmViewUODDolly_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance.StopRead()
    End Sub
    Public Sub FreezeControls()
        Me.lvwUODDolly.Enabled = False
        Me.lvwUODDolly.Enabled = False
        Me.Btn_Back_sm1.Enabled = False
        Me.Btn_Info1.Enabled = False
        Me.Btn_Quit_small1.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.lvwUODDolly.Enabled = True
        Me.lvwUODDolly.Enabled = True
        Me.Btn_Back_sm1.Enabled = True
        Me.Btn_Info1.Enabled = True
        Me.Btn_Quit_small1.Enabled = True
    End Sub
End Class
