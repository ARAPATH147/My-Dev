Public Class frmBDUODSummary
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblSummary As System.Windows.Forms.Label
    Friend WithEvents lblBookIn As System.Windows.Forms.Label

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
    Friend WithEvents lblNoDelivery As System.Windows.Forms.Label
    Friend WithEvents lblExpected As System.Windows.Forms.Label
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents lvwExpectedSummary As System.Windows.Forms.ListView
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBDUODSummary))
        Me.lblBookIn = New System.Windows.Forms.Label
        Me.lblSummary = New System.Windows.Forms.Label
        Me.lblNoDelivery = New System.Windows.Forms.Label
        Me.lvwExpectedSummary = New System.Windows.Forms.ListView
        Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader2 = New System.Windows.Forms.ColumnHeader
        Me.lblExpected = New System.Windows.Forms.Label
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'lblBookIn
        '
        Me.lblBookIn.Location = New System.Drawing.Point(24, 16)
        Me.lblBookIn.Name = "lblBookIn"
        Me.lblBookIn.Size = New System.Drawing.Size(160, 16)
        Me.lblBookIn.Text = "Book in Delivery"
        '
        'lblSummary
        '
        Me.lblSummary.Location = New System.Drawing.Point(24, 40)
        Me.lblSummary.Name = "lblSummary"
        Me.lblSummary.Size = New System.Drawing.Size(152, 16)
        Me.lblSummary.Text = "Summary"
        '
        'lblNoDelivery
        '
        Me.lblNoDelivery.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblNoDelivery.Location = New System.Drawing.Point(24, 64)
        Me.lblNoDelivery.Name = "lblNoDelivery"
        Me.lblNoDelivery.Size = New System.Drawing.Size(144, 16)
        Me.lblNoDelivery.Text = "No Delivery booked in"
        '
        'lvwExpectedSummary
        '
        Me.lvwExpectedSummary.Columns.Add(Me.ColumnHeader1)
        Me.lvwExpectedSummary.Columns.Add(Me.ColumnHeader2)
        Me.lvwExpectedSummary.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular)
        Me.lvwExpectedSummary.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lvwExpectedSummary.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.lvwExpectedSummary.Location = New System.Drawing.Point(24, 128)
        Me.lvwExpectedSummary.Name = "lvwExpectedSummary"
        Me.lvwExpectedSummary.Size = New System.Drawing.Size(184, 72)
        Me.lvwExpectedSummary.TabIndex = 1
        Me.lvwExpectedSummary.View = System.Windows.Forms.View.Details
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
        'lblExpected
        '
        Me.lblExpected.Location = New System.Drawing.Point(28, 95)
        Me.lblExpected.Name = "lblExpected"
        Me.lblExpected.Size = New System.Drawing.Size(160, 16)
        Me.lblExpected.Text = "Expected Today"
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(160, 216)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmBDUODSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.lvwExpectedSummary)
        Me.Controls.Add(Me.lblExpected)
        Me.Controls.Add(Me.lblNoDelivery)
        Me.Controls.Add(Me.lblSummary)
        Me.Controls.Add(Me.lblBookIn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmBDUODSummary"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub Btn_Quit_small1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        BDSessionMgr.GetInstance().EndSession(AppContainer.IsAbort.Yes)
    End Sub
    Private Sub frmBDUODSummary_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StopRead()
    End Sub
End Class
