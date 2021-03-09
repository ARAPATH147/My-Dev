Public Class frmBDUODInitialSummary
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblBookIn As System.Windows.Forms.Label
    Friend WithEvents lblExpected As System.Windows.Forms.Label

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
    Friend WithEvents lvwExpected As System.Windows.Forms.ListView
    Friend WithEvents lblOutstanding As System.Windows.Forms.Label
    Friend WithEvents lvwOutstanding As System.Windows.Forms.ListView
    Friend WithEvents lblNoExpected As System.Windows.Forms.Label
    Friend WithEvents lblNoOutstanding As System.Windows.Forms.Label
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Btn_Next_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Info As System.Windows.Forms.PictureBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBDUODInitialSummary))
        Me.lblBookIn = New System.Windows.Forms.Label
        Me.lblExpected = New System.Windows.Forms.Label
        Me.lvwExpected = New System.Windows.Forms.ListView
        Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader2 = New System.Windows.Forms.ColumnHeader
        Me.lblOutstanding = New System.Windows.Forms.Label
        Me.lvwOutstanding = New System.Windows.Forms.ListView
        Me.ColumnHeader3 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader4 = New System.Windows.Forms.ColumnHeader
        Me.Btn_Next_small1 = New System.Windows.Forms.PictureBox
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.lblNoExpected = New System.Windows.Forms.Label
        Me.lblNoOutstanding = New System.Windows.Forms.Label
        Me.Btn_Info = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'lblBookIn
        '
        Me.lblBookIn.Location = New System.Drawing.Point(24, 0)
        Me.lblBookIn.Name = "lblBookIn"
        Me.lblBookIn.Size = New System.Drawing.Size(160, 16)
        Me.lblBookIn.Text = "Book in Delivery"
        '
        'lblExpected
        '
        Me.lblExpected.Location = New System.Drawing.Point(24, 24)
        Me.lblExpected.Name = "lblExpected"
        Me.lblExpected.Size = New System.Drawing.Size(160, 16)
        Me.lblExpected.Text = "Expected Today"
        '
        'lvwExpected
        '
        Me.lvwExpected.Columns.Add(Me.ColumnHeader1)
        Me.lvwExpected.Columns.Add(Me.ColumnHeader2)
        Me.lvwExpected.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular)
        Me.lvwExpected.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lvwExpected.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.lvwExpected.Location = New System.Drawing.Point(24, 40)
        Me.lvwExpected.Name = "lvwExpected"
        Me.lvwExpected.Size = New System.Drawing.Size(184, 75)
        Me.lvwExpected.TabIndex = 7
        Me.lvwExpected.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "UOD Types"
        Me.ColumnHeader1.Width = 120
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Number"
        Me.ColumnHeader2.Width = 45
        '
        'lblOutstanding
        '
        Me.lblOutstanding.Location = New System.Drawing.Point(24, 120)
        Me.lblOutstanding.Name = "lblOutstanding"
        Me.lblOutstanding.Size = New System.Drawing.Size(216, 16)
        Me.lblOutstanding.Text = "Outstanding from Previous Deliveries"
        '
        'lvwOutstanding
        '
        Me.lvwOutstanding.Columns.Add(Me.ColumnHeader3)
        Me.lvwOutstanding.Columns.Add(Me.ColumnHeader4)
        Me.lvwOutstanding.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular)
        Me.lvwOutstanding.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lvwOutstanding.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.lvwOutstanding.Location = New System.Drawing.Point(24, 144)
        Me.lvwOutstanding.Name = "lvwOutstanding"
        Me.lvwOutstanding.Size = New System.Drawing.Size(184, 75)
        Me.lvwOutstanding.TabIndex = 5
        Me.lvwOutstanding.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "UOD Types"
        Me.ColumnHeader3.Width = 120
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Number"
        Me.ColumnHeader4.Width = 45
        '
        'Btn_Next_small1
        '
        Me.Btn_Next_small1.Image = CType(resources.GetObject("Btn_Next_small1.Image"), System.Drawing.Image)
        Me.Btn_Next_small1.Location = New System.Drawing.Point(24, 224)
        Me.Btn_Next_small1.Name = "Btn_Next_small1"
        Me.Btn_Next_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Next_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(160, 224)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblNoExpected
        '
        Me.lblNoExpected.Location = New System.Drawing.Point(24, 64)
        Me.lblNoExpected.Name = "lblNoExpected"
        Me.lblNoExpected.Size = New System.Drawing.Size(176, 20)
        Me.lblNoExpected.Text = "No Expected Deliveries Today"
        Me.lblNoExpected.Visible = False
        '
        'lblNoOutstanding
        '
        Me.lblNoOutstanding.Location = New System.Drawing.Point(24, 176)
        Me.lblNoOutstanding.Name = "lblNoOutstanding"
        Me.lblNoOutstanding.Size = New System.Drawing.Size(200, 16)
        Me.lblNoOutstanding.Text = "No Outstanding Previous Deliveries"
        Me.lblNoOutstanding.Visible = False
        '
        'Btn_Info
        '
        Me.Btn_Info.Image = CType(resources.GetObject("Btn_Info.Image"), System.Drawing.Image)
        Me.Btn_Info.Location = New System.Drawing.Point(192, 1)
        Me.Btn_Info.Name = "Btn_Info"
        Me.Btn_Info.Size = New System.Drawing.Size(32, 32)
        Me.Btn_Info.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmBDUODInitialSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Info)
        Me.Controls.Add(Me.lblNoOutstanding)
        Me.Controls.Add(Me.lblNoExpected)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Btn_Next_small1)
        Me.Controls.Add(Me.lvwOutstanding)
        Me.Controls.Add(Me.lblOutstanding)
        Me.Controls.Add(Me.lvwExpected)
        Me.Controls.Add(Me.lblExpected)
        Me.Controls.Add(Me.lblBookIn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmBDUODInitialSummary"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub Btn_Next_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Next_small1.Click
        FreezeControls()
        BDSessionMgr.GetInstance().DisplayBDScreen(BDSessionMgr.BDSCREENS.BDUODScan)
        Me.Hide()
        UnFreezeControls()
    End Sub
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        BDSessionMgr.GetInstance().QuitSession()
    End Sub
    Private Sub Btn_Info_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Info.Click
        FreezeControls()
        MessageBox.Show(MessageManager.GetInstance().GetMessage("M60"), "Help")
        UnFreezeControls()
    End Sub
    Public Sub FreezeControls()
        Me.Btn_Next_small1.Enabled = False
        Me.Btn_Quit_small1.Enabled = False
        Me.Btn_Info.Enabled = False
        Me.lvwExpected.Enabled = False
        Me.lvwOutstanding.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.Btn_Next_small1.Enabled = True
        Me.Btn_Quit_small1.Enabled = True
        Me.Btn_Info.Enabled = True
        Me.lvwExpected.Enabled = True
        Me.lvwOutstanding.Enabled = True
    End Sub
   
End Class
