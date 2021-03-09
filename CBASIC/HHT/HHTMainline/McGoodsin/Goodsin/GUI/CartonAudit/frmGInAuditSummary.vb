Public Class frmGInAuditSummary1
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblSummary As System.Windows.Forms.Label
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
    Friend WithEvents lblCartonQty As System.Windows.Forms.Label
    Friend WithEvents lblQty As System.Windows.Forms.Label
    Friend WithEvents lblMsg As System.Windows.Forms.Label
    Friend WithEvents PicBox_Quit As System.Windows.Forms.PictureBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmGInAuditSummary1))
        Me.lblSummary = New System.Windows.Forms.Label
        Me.lblAuditUOD = New System.Windows.Forms.Label
        Me.lblCartonQty = New System.Windows.Forms.Label
        Me.lblQty = New System.Windows.Forms.Label
        Me.lblMsg = New System.Windows.Forms.Label
        Me.PicBox_Quit = New System.Windows.Forms.PictureBox
        '
        'lblSummary
        '
        Me.lblSummary.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
        Me.lblSummary.Location = New System.Drawing.Point(24, 40)
        Me.lblSummary.Text = "Summary"
        '
        'lblAuditUOD
        '
        Me.lblAuditUOD.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
        Me.lblAuditUOD.Location = New System.Drawing.Point(24, 16)
        Me.lblAuditUOD.Text = "Audit UOD"
        '
        'lblCartonQty
        '
        Me.lblCartonQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblCartonQty.Location = New System.Drawing.Point(24, 88)
        Me.lblCartonQty.Size = New System.Drawing.Size(120, 24)
        Me.lblCartonQty.Text = "Cartons Booked In"
        '
        'lblQty
        '
        Me.lblQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
        Me.lblQty.Location = New System.Drawing.Point(152, 88)
        Me.lblQty.Size = New System.Drawing.Size(40, 20)
        Me.lblQty.Text = "1"
        '
        'lblMsg
        '
        Me.lblMsg.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg.Location = New System.Drawing.Point(24, 128)
        Me.lblMsg.Size = New System.Drawing.Size(144, 32)
        Me.lblMsg.Text = "Collect and file carton Book In Report"
        Me.lblMsg.Visible = False
        '
        'PicBox_Quit
        '
        Me.PicBox_Quit.Image = CType(resources.GetObject("PicBox_Quit.Image"), System.Drawing.Image)
        Me.PicBox_Quit.Location = New System.Drawing.Point(152, 216)
        Me.PicBox_Quit.Size = New System.Drawing.Size(48, 21)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        '
        'frmGInAuditSummary1
        '
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.Controls.Add(Me.PicBox_Quit)
        Me.Controls.Add(Me.lblMsg)
        Me.Controls.Add(Me.lblQty)
        Me.Controls.Add(Me.lblCartonQty)
        Me.Controls.Add(Me.lblSummary)
        Me.Controls.Add(Me.lblAuditUOD)
        Me.Text = "Goods In"

    End Sub

#End Region

End Class
