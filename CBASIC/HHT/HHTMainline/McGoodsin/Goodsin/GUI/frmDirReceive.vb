Public Class frmDirReceive
    Inherits System.Windows.Forms.Form
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents pnlView As System.Windows.Forms.Panel
    Friend WithEvents lblView As System.Windows.Forms.Label
    Friend WithEvents lblbook As System.Windows.Forms.Label
    Friend WithEvents lblNo3 As System.Windows.Forms.Label
    Friend WithEvents lblNo2 As System.Windows.Forms.Label
    Friend WithEvents lblDirRecv As System.Windows.Forms.Label
    Friend WithEvents lblSelect As System.Windows.Forms.Label
    Friend WithEvents lblNo1 As System.Windows.Forms.Label
    Friend WithEvents pnlAudit As System.Windows.Forms.Panel
    Friend WithEvents lblBookIn As System.Windows.Forms.Label

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        If objAppContainer.objConfigValues.DirectsActive = Macros.Y AndAlso _
                     objAppContainer.objConfigValues.ASNActive = Macros.N Then
            Me.lblAudit.ForeColor = Color.Gray
            Me.pnlAudit.Enabled = False
            Me.lblView.ForeColor = Color.Gray
            Me.pnlView.Enabled = False
        End If
        'Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents pnlBookIn As System.Windows.Forms.Panel
    Friend WithEvents lblAudit As System.Windows.Forms.Label
    Friend WithEvents btn_Info As System.Windows.Forms.PictureBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDirReceive))
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.pnlView = New System.Windows.Forms.Panel
        Me.lblView = New System.Windows.Forms.Label
        Me.pnlBookIn = New System.Windows.Forms.Panel
        Me.lblbook = New System.Windows.Forms.Label
        Me.lblNo3 = New System.Windows.Forms.Label
        Me.lblNo2 = New System.Windows.Forms.Label
        Me.lblDirRecv = New System.Windows.Forms.Label
        Me.lblSelect = New System.Windows.Forms.Label
        Me.lblNo1 = New System.Windows.Forms.Label
        Me.btn_Info = New System.Windows.Forms.PictureBox
        Me.pnlAudit = New System.Windows.Forms.Panel
        Me.lblAudit = New System.Windows.Forms.Label
        Me.lblBookIn = New System.Windows.Forms.Label
        Me.pnlView.SuspendLayout()
        Me.pnlBookIn.SuspendLayout()
        Me.pnlAudit.SuspendLayout()
        Me.SuspendLayout()
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(152, 200)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pnlView
        '
        Me.pnlView.Controls.Add(Me.lblView)
        Me.pnlView.Location = New System.Drawing.Point(48, 126)
        Me.pnlView.Name = "pnlView"
        Me.pnlView.Size = New System.Drawing.Size(88, 30)
        '
        'lblView
        '
        Me.lblView.Location = New System.Drawing.Point(8, 8)
        Me.lblView.Name = "lblView"
        Me.lblView.Size = New System.Drawing.Size(72, 16)
        Me.lblView.Text = "View Carton"
        '
        'pnlBookIn
        '
        Me.pnlBookIn.Controls.Add(Me.lblbook)
        Me.pnlBookIn.Location = New System.Drawing.Point(48, 56)
        Me.pnlBookIn.Name = "pnlBookIn"
        Me.pnlBookIn.Size = New System.Drawing.Size(104, 29)
        '
        'lblbook
        '
        Me.lblbook.Location = New System.Drawing.Point(8, 8)
        Me.lblbook.Name = "lblbook"
        Me.lblbook.Size = New System.Drawing.Size(88, 16)
        Me.lblbook.Text = "Book In Order"
        '
        'lblNo3
        '
        Me.lblNo3.Location = New System.Drawing.Point(24, 134)
        Me.lblNo3.Name = "lblNo3"
        Me.lblNo3.Size = New System.Drawing.Size(24, 16)
        Me.lblNo3.Text = "3."
        '
        'lblNo2
        '
        Me.lblNo2.Location = New System.Drawing.Point(24, 97)
        Me.lblNo2.Name = "lblNo2"
        Me.lblNo2.Size = New System.Drawing.Size(24, 16)
        Me.lblNo2.Text = "2."
        '
        'lblDirRecv
        '
        Me.lblDirRecv.Location = New System.Drawing.Point(24, 24)
        Me.lblDirRecv.Name = "lblDirRecv"
        Me.lblDirRecv.Size = New System.Drawing.Size(104, 20)
        Me.lblDirRecv.Text = "Directs Receiving"
        '
        'lblSelect
        '
        Me.lblSelect.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelect.Location = New System.Drawing.Point(24, 176)
        Me.lblSelect.Name = "lblSelect"
        Me.lblSelect.Size = New System.Drawing.Size(120, 20)
        Me.lblSelect.Text = "Select from list"
        '
        'lblNo1
        '
        Me.lblNo1.Location = New System.Drawing.Point(24, 64)
        Me.lblNo1.Name = "lblNo1"
        Me.lblNo1.Size = New System.Drawing.Size(24, 16)
        Me.lblNo1.Text = "1."
        '
        'btn_Info
        '
        Me.btn_Info.Image = CType(resources.GetObject("btn_Info.Image"), System.Drawing.Image)
        Me.btn_Info.Location = New System.Drawing.Point(176, 8)
        Me.btn_Info.Name = "btn_Info"
        Me.btn_Info.Size = New System.Drawing.Size(32, 32)
        Me.btn_Info.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pnlAudit
        '
        Me.pnlAudit.Controls.Add(Me.lblAudit)
        Me.pnlAudit.Location = New System.Drawing.Point(48, 89)
        Me.pnlAudit.Name = "pnlAudit"
        Me.pnlAudit.Size = New System.Drawing.Size(96, 29)
        '
        'lblAudit
        '
        Me.lblAudit.Location = New System.Drawing.Point(8, 8)
        Me.lblAudit.Name = "lblAudit"
        Me.lblAudit.Size = New System.Drawing.Size(80, 16)
        Me.lblAudit.Text = "Audit Carton"
        '
        'lblBookIn
        '
        Me.lblBookIn.Location = New System.Drawing.Point(48, 56)
        Me.lblBookIn.Name = "lblBookIn"
        Me.lblBookIn.Size = New System.Drawing.Size(88, 24)
        Me.lblBookIn.Text = "Book In Delivery"
        '
        'frmDirReceive
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.pnlView)
        Me.Controls.Add(Me.pnlBookIn)
        Me.Controls.Add(Me.lblNo3)
        Me.Controls.Add(Me.lblNo2)
        Me.Controls.Add(Me.lblDirRecv)
        Me.Controls.Add(Me.lblSelect)
        Me.Controls.Add(Me.lblNo1)
        Me.Controls.Add(Me.btn_Info)
        Me.Controls.Add(Me.pnlAudit)
        Me.Controls.Add(Me.lblBookIn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmDirReceive"
        Me.Text = "Goods In"
        Me.pnlView.ResumeLayout(False)
        Me.pnlBookIn.ResumeLayout(False)
        Me.pnlAudit.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub pnlAudit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlAudit.Click
        FreezeControls()
        ACSessionManager.GetInstance().StartSession()
#If RF Then
        If Not objAppContainer.bReconnectSuccess Then
            Me.Hide()
        End If
        objAppContainer.bReconnectSuccess = False
#ElseIf NRF Then
        Me.Hide()
#End If
        UnFreezeControls()
    End Sub

    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        Dim iResult As Integer
        FreezeControls()
        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M79"), "Confirmation", MessageBoxButtons.YesNo, _
                                  MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        If iResult = MsgBoxResult.Yes Then
            UnFreezeControls()
            Me.Hide()
        Else
            UnFreezeControls()

        End If

    End Sub
    Private Sub pnlBookIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlBookIn.Click
        FreezeControls()
        BCSessionMgr.GetInstance().StartSession()
#If RF Then
        If Not objAppContainer.bReconnectSuccess Then
            Me.Hide()
        End If
        objAppContainer.bReconnectSuccess = False
#ElseIf NRF Then
        Me.Hide()
#End If
        UnFreezeControls()

    End Sub

    Private Sub pnlView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlView.Click
        FreezeControls()
        VCSessionManager.GetInstance.StartSession()
#If RF Then
        If Not objAppContainer.bReconnectSuccess Then
            Me.Hide()
        End If
        objAppContainer.bReconnectSuccess = False
#ElseIf NRF Then
        Me.Hide()
#End If
        
        UnFreezeControls()
    End Sub
    Private Sub btn_Info_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Info.Click
        FreezeControls()
        MessageBox.Show(MessageManager.GetInstance().GetMessage("M83"), "Help")
        UnFreezeControls()
    End Sub
    Public Sub FreezeControls()
        RemoveHandler pnlAudit.Click, AddressOf pnlAudit_Click
        RemoveHandler pnlBookIn.Click, AddressOf pnlBookIn_Click
        RemoveHandler pnlView.Click, AddressOf pnlView_Click
        Me.btn_Info.Enabled = False
        Me.Btn_Quit_small1.Enabled = False

    End Sub
    Public Sub UnFreezeControls()
        Application.DoEvents()
        AddHandler pnlAudit.Click, AddressOf pnlAudit_Click
        AddHandler pnlBookIn.Click, AddressOf pnlBookIn_Click
        AddHandler pnlView.Click, AddressOf pnlView_Click
        Me.btn_Info.Enabled = True
        Me.Btn_Quit_small1.Enabled = True
    End Sub
End Class
