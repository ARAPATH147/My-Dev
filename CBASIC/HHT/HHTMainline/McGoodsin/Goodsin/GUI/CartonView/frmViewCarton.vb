Public Class frmViewCarton
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblMessage As System.Windows.Forms.Label
    Friend WithEvents lvwCartonList As System.Windows.Forms.ListView
    Friend WithEvents lblMessage2 As System.Windows.Forms.Label
    Friend WithEvents lblSupplier As System.Windows.Forms.Label

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        Me.lvwCartonList.Activation = ItemActivation.OneClick
        Me.lvwCartonList.Font = New System.Drawing.Font("Courier New", 9.0!, FontStyle.Regular)
        'Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents Btn_Back_sm1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Info1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmViewCarton))
        Me.lblMessage = New System.Windows.Forms.Label
        Me.lvwCartonList = New System.Windows.Forms.ListView
        Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader2 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader3 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader4 = New System.Windows.Forms.ColumnHeader
        Me.lblMessage2 = New System.Windows.Forms.Label
        Me.lblSupplier = New System.Windows.Forms.Label
        Me.Btn_Back_sm1 = New System.Windows.Forms.PictureBox
        Me.Btn_Info1 = New System.Windows.Forms.PictureBox
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'lblMessage
        '
        Me.lblMessage.Location = New System.Drawing.Point(16, 16)
        Me.lblMessage.Name = "lblMessage"
        Me.lblMessage.Size = New System.Drawing.Size(162, 16)
        Me.lblMessage.Text = "View Carton"
        '
        'lvwCartonList
        '
        Me.lvwCartonList.Columns.Add(Me.ColumnHeader1)
        Me.lvwCartonList.Columns.Add(Me.ColumnHeader2)
        Me.lvwCartonList.Columns.Add(Me.ColumnHeader3)
        Me.lvwCartonList.Columns.Add(Me.ColumnHeader4)
        Me.lvwCartonList.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lvwCartonList.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lvwCartonList.FullRowSelect = True
        Me.lvwCartonList.Location = New System.Drawing.Point(12, 56)
        Me.lvwCartonList.Name = "lvwCartonList"
        Me.lvwCartonList.Size = New System.Drawing.Size(218, 120)
        Me.lvwCartonList.TabIndex = 5
        Me.lvwCartonList.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Carton"
        Me.ColumnHeader1.Width = 67
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Expt"
        Me.ColumnHeader2.Width = 48
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Bkd In"
        Me.ColumnHeader3.Width = 54
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Num"
        Me.ColumnHeader4.Width = 33
        '
        'lblMessage2
        '
        Me.lblMessage2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMessage2.Location = New System.Drawing.Point(12, 188)
        Me.lblMessage2.Name = "lblMessage2"
        Me.lblMessage2.Size = New System.Drawing.Size(96, 16)
        Me.lblMessage2.Text = "Select Carton"
        '
        'lblSupplier
        '
        Me.lblSupplier.Location = New System.Drawing.Point(16, 32)
        Me.lblSupplier.Name = "lblSupplier"
        Me.lblSupplier.Size = New System.Drawing.Size(162, 21)
        Me.lblSupplier.Text = "Clarins"
        '
        'Btn_Back_sm1
        '
        Me.Btn_Back_sm1.Image = CType(resources.GetObject("Btn_Back_sm1.Image"), System.Drawing.Image)
        Me.Btn_Back_sm1.Location = New System.Drawing.Point(12, 216)
        Me.Btn_Back_sm1.Name = "Btn_Back_sm1"
        Me.Btn_Back_sm1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Back_sm1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Info1
        '
        Me.Btn_Info1.Image = CType(resources.GetObject("Btn_Info1.Image"), System.Drawing.Image)
        Me.Btn_Info1.Location = New System.Drawing.Point(184, 8)
        Me.Btn_Info1.Name = "Btn_Info1"
        Me.Btn_Info1.Size = New System.Drawing.Size(32, 32)
        Me.Btn_Info1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(180, 216)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmViewCarton
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Btn_Info1)
        Me.Controls.Add(Me.Btn_Back_sm1)
        Me.Controls.Add(Me.lblSupplier)
        Me.Controls.Add(Me.lblMessage2)
        Me.Controls.Add(Me.lvwCartonList)
        Me.Controls.Add(Me.lblMessage)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmViewCarton"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Public bCartonListPurge As Boolean = False
    Public bDisableSelected As Boolean = False
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        'VCSessionManager.GetInstance.QuitItemDetailsScreen()
        VCSessionManager.GetInstance().QuitSession()
    End Sub

    Private Sub Btn_Back_sm1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Back_sm1.Click
        FreezeControls()
        bCartonListPurge = True
        VCSessionManager.GetInstance.DisplayViewCarton(VCSessionManager.VCARTONSCREENS.ViewSuppliers)
        UnFreezeControls()
    End Sub


    Private Sub lvwCartonList_ItemActivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvwCartonList.SelectedIndexChanged
        If bDisableSelected Then
            'VCSessionManager.GetInstance.DisplayViewCarton(VCSessionManager.VCARTONSCREENS.ViewItems)
            If VCSessionManager.GetInstance.GetCartonInfo() Then
                VCSessionManager.GetInstance.DisplayViewCarton(VCSessionManager.VCARTONSCREENS.ViewItems)
                Me.Hide()
            End If
        End If
    End Sub

    Private Sub Btn_Info1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Info1.Click
        FreezeControls()
        MessageBox.Show(MessageManager.GetInstance().GetMessage("M74"), "Help")
        UnFreezeControls()
    End Sub
    Private Sub frmViewCarton_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance.StopRead()
    End Sub
    Public Sub FreezeControls()
        Me.lvwCartonList.Enabled = False
        Me.Btn_Back_sm1.Enabled = False
        Me.Btn_Info1.Enabled = False
        Me.Btn_Quit_small1.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.lvwCartonList.Enabled = True
        Me.Btn_Back_sm1.Enabled = True
        Me.Btn_Info1.Enabled = True
        Me.Btn_Quit_small1.Enabled = True
    End Sub
End Class
