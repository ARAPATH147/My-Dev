Public Class frmBDUODHome
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblMsg As System.Windows.Forms.Label
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
    Friend WithEvents Btn_Next_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblMsg2 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBDUODHome))
        Me.lblMsg = New System.Windows.Forms.Label
        Me.lblBookIn = New System.Windows.Forms.Label
        Me.Btn_Next_small1 = New System.Windows.Forms.PictureBox
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.lblMsg2 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblMsg
        '
        Me.lblMsg.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg.Location = New System.Drawing.Point(24, 48)
        Me.lblMsg.Name = "lblMsg"
        Me.lblMsg.Size = New System.Drawing.Size(168, 48)
        Me.lblMsg.Text = "Ensure  you are the only user  booking in"
        '
        'lblBookIn
        '
        Me.lblBookIn.Location = New System.Drawing.Point(24, 16)
        Me.lblBookIn.Name = "lblBookIn"
        Me.lblBookIn.Size = New System.Drawing.Size(160, 20)
        Me.lblBookIn.Text = "Book in Delivery"
        '
        'Btn_Next_small1
        '
        Me.Btn_Next_small1.Image = CType(resources.GetObject("Btn_Next_small1.Image"), System.Drawing.Image)
        Me.Btn_Next_small1.Location = New System.Drawing.Point(24, 216)
        Me.Btn_Next_small1.Name = "Btn_Next_small1"
        Me.Btn_Next_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Next_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(160, 216)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblMsg2
        '
        Me.lblMsg2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg2.Location = New System.Drawing.Point(24, 120)
        Me.lblMsg2.Name = "lblMsg2"
        Me.lblMsg2.Size = New System.Drawing.Size(176, 64)
        Me.lblMsg2.Text = "When booking in dollys if security band is black scan dolly, any other colour sca" & _
            "n crates"
        '
        'frmBDUODHome
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblMsg2)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Btn_Next_small1)
        Me.Controls.Add(Me.lblBookIn)
        Me.Controls.Add(Me.lblMsg)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmBDUODHome"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub Btn_Next_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Next_small1.Click
        FreezeControls()
        BDSessionMgr.GetInstance().DisplayBDScreen(BDSessionMgr.BDSCREENS.BDUODInitialSummary)
        Me.Hide()
        UnFreezeControls()
    End Sub
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        BDSessionMgr.GetInstance().QuitSession()     
    End Sub
    Public Sub FreezeControls()
        Me.Btn_Next_small1.Enabled = False
        Me.Btn_Quit_small1.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.Btn_Next_small1.Enabled = True
        Me.Btn_Quit_small1.Enabled = True
    End Sub
End Class
