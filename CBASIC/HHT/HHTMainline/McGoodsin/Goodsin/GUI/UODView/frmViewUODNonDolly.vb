Public Class frmViewUODNonDolly
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblUODText As System.Windows.Forms.Label
    Friend WithEvents lblTypeText As System.Windows.Forms.Label
    Friend WithEvents lblTypeValue As System.Windows.Forms.Label
    Friend WithEvents lblStatusText As System.Windows.Forms.Label
    Friend WithEvents lblExptdDateText As System.Windows.Forms.Label
    Friend WithEvents lblExptdDateValue As System.Windows.Forms.Label
    Friend WithEvents lblBkdDateText As System.Windows.Forms.Label
    Friend WithEvents lblBkdDateValue As System.Windows.Forms.Label
    Friend WithEvents lblStatusValue As System.Windows.Forms.Label

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        Me.lvwUODNotDolly.Activation = ItemActivation.OneClick
        Me.lvwUODNotDolly.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular)
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
    Friend WithEvents lblUODValue As System.Windows.Forms.Label
    Public WithEvents lvwUODNotDolly As System.Windows.Forms.ListView
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmViewUODNonDolly))
        Me.lblUODText = New System.Windows.Forms.Label
        Me.lblTypeText = New System.Windows.Forms.Label
        Me.lblTypeValue = New System.Windows.Forms.Label
        Me.lblStatusText = New System.Windows.Forms.Label
        Me.lblExptdDateText = New System.Windows.Forms.Label
        Me.lblExptdDateValue = New System.Windows.Forms.Label
        Me.lblBkdDateText = New System.Windows.Forms.Label
        Me.lblBkdDateValue = New System.Windows.Forms.Label
        Me.lblStatusValue = New System.Windows.Forms.Label
        Me.Btn_Back_sm1 = New System.Windows.Forms.PictureBox
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.lvwUODNotDolly = New System.Windows.Forms.ListView
        Me.lblUODValue = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblUODText
        '
        Me.lblUODText.Location = New System.Drawing.Point(16, 24)
        Me.lblUODText.Name = "lblUODText"
        Me.lblUODText.Size = New System.Drawing.Size(40, 16)
        Me.lblUODText.Text = "UOD"
        '
        'lblTypeText
        '
        Me.lblTypeText.Location = New System.Drawing.Point(16, 48)
        Me.lblTypeText.Name = "lblTypeText"
        Me.lblTypeText.Size = New System.Drawing.Size(40, 16)
        Me.lblTypeText.Text = "Type:"
        '
        'lblTypeValue
        '
        Me.lblTypeValue.Location = New System.Drawing.Point(52, 48)
        Me.lblTypeValue.Name = "lblTypeValue"
        Me.lblTypeValue.Size = New System.Drawing.Size(72, 16)
        Me.lblTypeValue.Text = "Label3"
        '
        'lblStatusText
        '
        Me.lblStatusText.Location = New System.Drawing.Point(126, 48)
        Me.lblStatusText.Name = "lblStatusText"
        Me.lblStatusText.Size = New System.Drawing.Size(46, 16)
        Me.lblStatusText.Text = "Bkd In:"
        '
        'lblExptdDateText
        '
        Me.lblExptdDateText.Location = New System.Drawing.Point(16, 72)
        Me.lblExptdDateText.Name = "lblExptdDateText"
        Me.lblExptdDateText.Size = New System.Drawing.Size(40, 16)
        Me.lblExptdDateText.Text = "Exptd:"
        '
        'lblExptdDateValue
        '
        Me.lblExptdDateValue.Location = New System.Drawing.Point(56, 72)
        Me.lblExptdDateValue.Name = "lblExptdDateValue"
        Me.lblExptdDateValue.Size = New System.Drawing.Size(64, 16)
        Me.lblExptdDateValue.Text = "Label6"
        '
        'lblBkdDateText
        '
        Me.lblBkdDateText.Location = New System.Drawing.Point(128, 72)
        Me.lblBkdDateText.Name = "lblBkdDateText"
        Me.lblBkdDateText.Size = New System.Drawing.Size(32, 16)
        Me.lblBkdDateText.Text = "Bkd:"
        '
        'lblBkdDateValue
        '
        Me.lblBkdDateValue.Location = New System.Drawing.Point(163, 72)
        Me.lblBkdDateValue.Name = "lblBkdDateValue"
        Me.lblBkdDateValue.Size = New System.Drawing.Size(64, 16)
        Me.lblBkdDateValue.Text = "Label8"
        '
        'lblStatusValue
        '
        Me.lblStatusValue.Location = New System.Drawing.Point(171, 48)
        Me.lblStatusValue.Name = "lblStatusValue"
        Me.lblStatusValue.Size = New System.Drawing.Size(56, 16)
        Me.lblStatusValue.Text = "Label9"
        '
        'Btn_Back_sm1
        '
        Me.Btn_Back_sm1.Image = CType(resources.GetObject("Btn_Back_sm1.Image"), System.Drawing.Image)
        Me.Btn_Back_sm1.Location = New System.Drawing.Point(16, 216)
        Me.Btn_Back_sm1.Name = "Btn_Back_sm1"
        Me.Btn_Back_sm1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Back_sm1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(176, 216)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lvwUODNotDolly
        '
        Me.lvwUODNotDolly.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lvwUODNotDolly.Location = New System.Drawing.Point(16, 96)
        Me.lvwUODNotDolly.Name = "lvwUODNotDolly"
        Me.lvwUODNotDolly.Size = New System.Drawing.Size(210, 104)
        Me.lvwUODNotDolly.TabIndex = 1
        Me.lvwUODNotDolly.View = System.Windows.Forms.View.Details
        '
        'lblUODValue
        '
        Me.lblUODValue.Location = New System.Drawing.Point(56, 24)
        Me.lblUODValue.Name = "lblUODValue"
        Me.lblUODValue.Size = New System.Drawing.Size(96, 16)
        Me.lblUODValue.Text = "Label1"
        '
        'frmViewUODNonDolly
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblUODValue)
        Me.Controls.Add(Me.lvwUODNotDolly)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Btn_Back_sm1)
        Me.Controls.Add(Me.lblStatusValue)
        Me.Controls.Add(Me.lblBkdDateValue)
        Me.Controls.Add(Me.lblBkdDateText)
        Me.Controls.Add(Me.lblExptdDateValue)
        Me.Controls.Add(Me.lblExptdDateText)
        Me.Controls.Add(Me.lblStatusText)
        Me.Controls.Add(Me.lblTypeValue)
        Me.Controls.Add(Me.lblTypeText)
        Me.Controls.Add(Me.lblUODText)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmViewUODNonDolly"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        VUODSessionMgr.GetInstance.QuitSession()
    End Sub

    Private Sub Btn_Back_sm1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Back_sm1.Click
        FreezeControls()
        VUODSessionMgr.GetInstance.Transition()
        UnFreezeControls()
    End Sub

    Private Sub frmViewUODNonDolly_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance.StopRead()
    End Sub
    Public Sub FreezeControls()
        Me.lvwUODNotDolly.Enabled = False
        Me.Btn_Back_sm1.Enabled = False
        Me.Btn_Quit_small1.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.lvwUODNotDolly.Enabled = True
        Me.Btn_Back_sm1.Enabled = True
        Me.Btn_Quit_small1.Enabled = True
    End Sub

End Class
