Public Class frmGInAuditSummary
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
    Friend WithEvents lblContainer As System.Windows.Forms.Label
    Friend WithEvents lblItemsScanned As System.Windows.Forms.Label
    Friend WithEvents lblMsg As System.Windows.Forms.Label
    Friend WithEvents lblContainerUOD As System.Windows.Forms.Label
    Friend WithEvents lblNo As System.Windows.Forms.Label
    Friend WithEvents Btn_Quit_small As System.Windows.Forms.PictureBox
    Friend WithEvents Help1 As System.Windows.Forms.PictureBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGInAuditSummary))
        Me.lblAuditUOD = New System.Windows.Forms.Label
        Me.lblSummary = New System.Windows.Forms.Label
        Me.lblContainer = New System.Windows.Forms.Label
        Me.lblItemsScanned = New System.Windows.Forms.Label
        Me.lblMsg = New System.Windows.Forms.Label
        Me.Btn_Quit_small = New System.Windows.Forms.PictureBox
        Me.lblContainerUOD = New System.Windows.Forms.Label
        Me.lblNo = New System.Windows.Forms.Label
        Me.Help1 = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'lblAuditUOD
        '
        Me.lblAuditUOD.Location = New System.Drawing.Point(24, 24)
        Me.lblAuditUOD.Name = "lblAuditUOD"
        Me.lblAuditUOD.Size = New System.Drawing.Size(100, 16)
        Me.lblAuditUOD.Text = "Audit UOD"
        '
        'lblSummary
        '
        Me.lblSummary.Location = New System.Drawing.Point(24, 48)
        Me.lblSummary.Name = "lblSummary"
        Me.lblSummary.Size = New System.Drawing.Size(100, 16)
        Me.lblSummary.Text = "Summary"
        '
        'lblContainer
        '
        Me.lblContainer.Location = New System.Drawing.Point(24, 72)
        Me.lblContainer.Name = "lblContainer"
        Me.lblContainer.Size = New System.Drawing.Size(34, 17)
        Me.lblContainer.Text = "Crate"
        '
        'lblItemsScanned
        '
        Me.lblItemsScanned.Location = New System.Drawing.Point(24, 96)
        Me.lblItemsScanned.Name = "lblItemsScanned"
        Me.lblItemsScanned.Size = New System.Drawing.Size(100, 16)
        Me.lblItemsScanned.Text = "Items Scanned"
        '
        'lblMsg
        '
        Me.lblMsg.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg.Location = New System.Drawing.Point(24, 120)
        Me.lblMsg.Name = "lblMsg"
        Me.lblMsg.Size = New System.Drawing.Size(184, 48)
        Me.lblMsg.Text = "Collect Stores Service Centre Receiving Exception Report. Stockfile not adjusted." & _
            ""
        '
        'Btn_Quit_small
        '
        Me.Btn_Quit_small.Image = CType(resources.GetObject("Btn_Quit_small.Image"), System.Drawing.Image)
        Me.Btn_Quit_small.Location = New System.Drawing.Point(160, 192)
        Me.Btn_Quit_small.Name = "Btn_Quit_small"
        Me.Btn_Quit_small.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblContainerUOD
        '
        Me.lblContainerUOD.Location = New System.Drawing.Point(64, 72)
        Me.lblContainerUOD.Name = "lblContainerUOD"
        Me.lblContainerUOD.Size = New System.Drawing.Size(88, 16)
        '
        'lblNo
        '
        Me.lblNo.Location = New System.Drawing.Point(136, 96)
        Me.lblNo.Name = "lblNo"
        Me.lblNo.Size = New System.Drawing.Size(88, 16)
        '
        'Help1
        '
        Me.Help1.Image = CType(resources.GetObject("Help1.Image"), System.Drawing.Image)
        Me.Help1.Location = New System.Drawing.Point(176, 24)
        Me.Help1.Name = "Help1"
        Me.Help1.Size = New System.Drawing.Size(32, 32)
        Me.Help1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmGInAuditSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.Help1)
        Me.Controls.Add(Me.lblNo)
        Me.Controls.Add(Me.lblContainerUOD)
        Me.Controls.Add(Me.Btn_Quit_small)
        Me.Controls.Add(Me.lblMsg)
        Me.Controls.Add(Me.lblItemsScanned)
        Me.Controls.Add(Me.lblContainer)
        Me.Controls.Add(Me.lblSummary)
        Me.Controls.Add(Me.lblAuditUOD)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmGInAuditSummary"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub frmGInAuditSummary_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StopRead()
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINAUDITSUMMARY
    End Sub
    Private Sub Btn_Quit_small_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small.Click
        AUODSessionManager.GetInstance().FinalQuitSession()
    End Sub
    Private Sub Help1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Help1.Click
        'Shows the help information
        FreezeControls()
        MessageBox.Show(MessageManager.GetInstance().GetMessage("M63"), "Help")
        UnFreezeControls()
    End Sub
    Public Sub FreezeControls()
        Me.Help1.Enabled = False
        Me.Btn_Quit_small.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.Help1.Enabled = True
        Me.Btn_Quit_small.Enabled = True
    End Sub

End Class
