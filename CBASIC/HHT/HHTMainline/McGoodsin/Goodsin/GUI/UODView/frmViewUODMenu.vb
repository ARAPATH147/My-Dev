Public Class frmViewUODMenu
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblView As System.Windows.Forms.Label
    Friend WithEvents lblToday As System.Windows.Forms.Label
    Friend WithEvents lblNo1 As System.Windows.Forms.Label
    Friend WithEvents lblNo2 As System.Windows.Forms.Label
    Friend WithEvents pnlTodayUOD As System.Windows.Forms.Panel
    Friend WithEvents pnlFutureUOD As System.Windows.Forms.Panel
    Friend WithEvents lblFuture As System.Windows.Forms.Label
    Friend WithEvents lblSelect As System.Windows.Forms.Label

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
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmViewUODMenu))
        Me.lblView = New System.Windows.Forms.Label
        Me.lblToday = New System.Windows.Forms.Label
        Me.lblNo1 = New System.Windows.Forms.Label
        Me.lblNo2 = New System.Windows.Forms.Label
        Me.pnlTodayUOD = New System.Windows.Forms.Panel
        Me.pnlFutureUOD = New System.Windows.Forms.Panel
        Me.lblFuture = New System.Windows.Forms.Label
        Me.lblSelect = New System.Windows.Forms.Label
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.pnlTodayUOD.SuspendLayout()
        Me.pnlFutureUOD.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblView
        '
        Me.lblView.Location = New System.Drawing.Point(24, 24)
        Me.lblView.Name = "lblView"
        Me.lblView.Size = New System.Drawing.Size(100, 16)
        Me.lblView.Text = "View UOD"
        '
        'lblToday
        '
        Me.lblToday.Location = New System.Drawing.Point(8, 8)
        Me.lblToday.Name = "lblToday"
        Me.lblToday.Size = New System.Drawing.Size(64, 16)
        Me.lblToday.Text = "Today"
        '
        'lblNo1
        '
        Me.lblNo1.Location = New System.Drawing.Point(23, 70)
        Me.lblNo1.Name = "lblNo1"
        Me.lblNo1.Size = New System.Drawing.Size(16, 20)
        Me.lblNo1.Text = "1."
        '
        'lblNo2
        '
        Me.lblNo2.Location = New System.Drawing.Point(23, 104)
        Me.lblNo2.Name = "lblNo2"
        Me.lblNo2.Size = New System.Drawing.Size(16, 20)
        Me.lblNo2.Text = "2."
        '
        'pnlTodayUOD
        '
        Me.pnlTodayUOD.Controls.Add(Me.lblToday)
        Me.pnlTodayUOD.Location = New System.Drawing.Point(47, 62)
        Me.pnlTodayUOD.Name = "pnlTodayUOD"
        Me.pnlTodayUOD.Size = New System.Drawing.Size(73, 26)
        '
        'pnlFutureUOD
        '
        Me.pnlFutureUOD.Controls.Add(Me.lblFuture)
        Me.pnlFutureUOD.Location = New System.Drawing.Point(47, 96)
        Me.pnlFutureUOD.Name = "pnlFutureUOD"
        Me.pnlFutureUOD.Size = New System.Drawing.Size(65, 26)
        '
        'lblFuture
        '
        Me.lblFuture.Location = New System.Drawing.Point(8, 8)
        Me.lblFuture.Name = "lblFuture"
        Me.lblFuture.Size = New System.Drawing.Size(56, 16)
        Me.lblFuture.Text = "Future"
        '
        'lblSelect
        '
        Me.lblSelect.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelect.Location = New System.Drawing.Point(39, 160)
        Me.lblSelect.Name = "lblSelect"
        Me.lblSelect.Size = New System.Drawing.Size(121, 24)
        Me.lblSelect.Text = "Select from list"
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(160, 216)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmViewUODMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.lblSelect)
        Me.Controls.Add(Me.pnlTodayUOD)
        Me.Controls.Add(Me.lblNo2)
        Me.Controls.Add(Me.lblNo1)
        Me.Controls.Add(Me.lblView)
        Me.Controls.Add(Me.pnlFutureUOD)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmViewUODMenu"
        Me.Text = "Goods In"
        Me.pnlTodayUOD.ResumeLayout(False)
        Me.pnlFutureUOD.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub pnlTodayUOD_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlTodayUOD.Click
        Try

            FreezeControls()
            'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
            VUODSessionMgr.GetInstance().m_strPeriod = "T"
            VUODSessionMgr.GetInstance.DisplayVUODScreen(VUODSessionMgr.VUODSCREENS.ViewUOD)
            UnFreezeControls()
#If RF Then
            If objAppContainer.bCommFailure Then
                VUODSessionMgr.GetInstance.DisposeVUOD()
            End If
#End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        FreezeControls()
        VUODSessionMgr.GetInstance.QuitSession()
    End Sub

    Private Sub pnlFutureUOD_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlFutureUOD.Click
        Try

       
            FreezeControls()
            VUODSessionMgr.GetInstance().m_strPeriod = "F"
            VUODSessionMgr.GetInstance.DisplayVUODScreen(VUODSessionMgr.VUODSCREENS.ViewUOD)

            UnFreezeControls()
#If RF Then
            If objAppContainer.bCommFailure Then
                VUODSessionMgr.GetInstance.DisposeVUOD()
            End If
#End If
        Catch ex As Exception

        End Try

    End Sub
    Public Sub FreezeControls()
        RemoveHandler pnlFutureUOD.Click, AddressOf pnlFutureUOD_Click
        RemoveHandler pnlTodayUOD.Click, AddressOf pnlTodayUOD_Click
        Me.Btn_Quit_small1.Enabled = False

    End Sub
    Public Sub UnFreezeControls()
        Application.DoEvents()
        AddHandler pnlFutureUOD.Click, AddressOf pnlFutureUOD_Click
        AddHandler pnlTodayUOD.Click, AddressOf pnlTodayUOD_Click
        Me.Btn_Quit_small1.Enabled = True

    End Sub
    Private Sub frmViewUODMenu_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance.StopRead()
    End Sub

End Class
