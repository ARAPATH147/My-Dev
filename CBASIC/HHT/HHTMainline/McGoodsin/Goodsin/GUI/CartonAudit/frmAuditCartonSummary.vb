Public Class frmAuditCartonSummary
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
    Friend WithEvents Btn_Quit_small As System.Windows.Forms.PictureBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAuditCartonSummary))
        Me.lblSummary = New System.Windows.Forms.Label
        Me.lblAuditUOD = New System.Windows.Forms.Label
        Me.lblCartonQty = New System.Windows.Forms.Label
        Me.lblQty = New System.Windows.Forms.Label
        Me.lblMsg = New System.Windows.Forms.Label
        Me.Btn_Quit_small = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'lblSummary
        '
        Me.lblSummary.Location = New System.Drawing.Point(24, 48)
        Me.lblSummary.Name = "lblSummary"
        Me.lblSummary.Size = New System.Drawing.Size(100, 16)
        Me.lblSummary.Text = "Summary"
        '
        'lblAuditUOD
        '
        Me.lblAuditUOD.Location = New System.Drawing.Point(24, 24)
        Me.lblAuditUOD.Name = "lblAuditUOD"
        Me.lblAuditUOD.Size = New System.Drawing.Size(100, 16)
        Me.lblAuditUOD.Text = "Audit Carton"
        '
        'lblCartonQty
        '
        Me.lblCartonQty.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblCartonQty.Location = New System.Drawing.Point(24, 88)
        Me.lblCartonQty.Name = "lblCartonQty"
        Me.lblCartonQty.Size = New System.Drawing.Size(120, 16)
        Me.lblCartonQty.Text = "Cartons Booked In"
        '
        'lblQty
        '
        Me.lblQty.Location = New System.Drawing.Point(152, 88)
        Me.lblQty.Name = "lblQty"
        Me.lblQty.Size = New System.Drawing.Size(40, 16)
        Me.lblQty.Text = "1"
        '
        'lblMsg
        '
        Me.lblMsg.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg.Location = New System.Drawing.Point(24, 144)
        Me.lblMsg.Name = "lblMsg"
        Me.lblMsg.Size = New System.Drawing.Size(168, 32)
        Me.lblMsg.Text = "Collect and file carton Book In Report"
        Me.lblMsg.Visible = False
        '
        'Btn_Quit_small
        '
        Me.Btn_Quit_small.Image = CType(resources.GetObject("Btn_Quit_small.Image"), System.Drawing.Image)
        Me.Btn_Quit_small.Location = New System.Drawing.Point(160, 208)
        Me.Btn_Quit_small.Name = "Btn_Quit_small"
        Me.Btn_Quit_small.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmAuditCartonSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Quit_small)
        Me.Controls.Add(Me.lblMsg)
        Me.Controls.Add(Me.lblQty)
        Me.Controls.Add(Me.lblCartonQty)
        Me.Controls.Add(Me.lblSummary)
        Me.Controls.Add(Me.lblAuditUOD)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmAuditCartonSummary"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub frmAuditCartonSummary_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StopRead()
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.AUDITCARTONSUM
    End Sub
    Private Sub Btn_Quit_small_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small.Click
        ' If Not objAppContainer.strDeviceType = Macros.RF Then
#If NRF Then
             'For Non Rf Device
            If ACSessionManager.GetInstance().m_bShowMsg Then
                'If summary sreen is exited after Finishing the session
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M85"), "Alert", MessageBoxButtons.OK, _
                                          MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                ACSessionManager.GetInstance().m_bShowMsg = False
                ACSessionManager.GetInstance().EndSession(AppContainer.IsAbort.No)
            Else
                ACSessionManager.GetInstance().EndSession(AppContainer.IsAbort.Yes)
            End If
#ElseIf RF Then
        If ACSessionManager.GetInstance().m_bShowMsg Then
            'If summary sreen is exited after Finishing the session
            ACSessionManager.GetInstance().EndSession(AppContainer.IsAbort.No)
        Else

            ACSessionManager.GetInstance().EndSession(AppContainer.IsAbort.Yes)
        End If
#End If

        ' Else

        'End If
    End Sub
    Public Sub FreezeControls()
        Me.Btn_Quit_small.Enabled = False

    End Sub

    Public Sub UnFreezeControls()
        Me.Btn_Quit_small.Enabled = True
    End Sub

End Class
