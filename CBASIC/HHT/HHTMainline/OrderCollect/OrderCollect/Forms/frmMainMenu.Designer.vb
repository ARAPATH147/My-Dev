<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmMainMenu
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMainMenu))
        Me.tcMainMenu = New System.Windows.Forms.TabControl
        Me.tpBookIn = New System.Windows.Forms.TabPage
        Me.pbOrderCollect = New System.Windows.Forms.PictureBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.pbBookInPutAway = New System.Windows.Forms.PictureBox
        Me.lblBookInPutAway = New System.Windows.Forms.Label
        Me.PictureBox4 = New System.Windows.Forms.PictureBox
        Me.pbPutWayMove = New System.Windows.Forms.PictureBox
        Me.lblPutWayMove = New System.Windows.Forms.Label
        Me.PictureBox2 = New System.Windows.Forms.PictureBox
        Me.lblBookInOnly = New System.Windows.Forms.Label
        Me.pbBookInOnly = New System.Windows.Forms.PictureBox
        Me.tpLogOff = New System.Windows.Forms.TabPage
        Me.lblLogOff = New System.Windows.Forms.Label
        Me.pbLogOff = New System.Windows.Forms.PictureBox
        Me.tcMainMenu.SuspendLayout()
        Me.tpBookIn.SuspendLayout()
        Me.tpLogOff.SuspendLayout()
        Me.SuspendLayout()
        '
        'tcMainMenu
        '
        Me.tcMainMenu.Controls.Add(Me.tpBookIn)
        Me.tcMainMenu.Controls.Add(Me.tpLogOff)
        Me.tcMainMenu.Font = New System.Drawing.Font("Tahoma", 13.5!, System.Drawing.FontStyle.Bold)
        Me.tcMainMenu.Location = New System.Drawing.Point(0, 0)
        Me.tcMainMenu.Name = "tcMainMenu"
        Me.tcMainMenu.SelectedIndex = 0
        Me.tcMainMenu.Size = New System.Drawing.Size(240, 300)
        Me.tcMainMenu.TabIndex = 0
        '
        'tpBookIn
        '
        Me.tpBookIn.AutoScroll = True
        Me.tpBookIn.Controls.Add(Me.pbOrderCollect)
        Me.tpBookIn.Controls.Add(Me.Label3)
        Me.tpBookIn.Controls.Add(Me.pbBookInPutAway)
        Me.tpBookIn.Controls.Add(Me.lblBookInPutAway)
        Me.tpBookIn.Controls.Add(Me.PictureBox4)
        Me.tpBookIn.Controls.Add(Me.pbPutWayMove)
        Me.tpBookIn.Controls.Add(Me.lblPutWayMove)
        Me.tpBookIn.Controls.Add(Me.PictureBox2)
        Me.tpBookIn.Controls.Add(Me.lblBookInOnly)
        Me.tpBookIn.Controls.Add(Me.pbBookInOnly)
        Me.tpBookIn.Location = New System.Drawing.Point(0, 0)
        Me.tpBookIn.Name = "tpBookIn"
        Me.tpBookIn.Size = New System.Drawing.Size(240, 268)
        Me.tpBookIn.Text = " Book In "
        '
        'pbOrderCollect
        '
        Me.pbOrderCollect.Image = CType(resources.GetObject("pbOrderCollect.Image"), System.Drawing.Image)
        Me.pbOrderCollect.Location = New System.Drawing.Point(138, 146)
        Me.pbOrderCollect.Name = "pbOrderCollect"
        Me.pbOrderCollect.Size = New System.Drawing.Size(60, 60)
        Me.pbOrderCollect.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbOrderCollect.Visible = False
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label3.Location = New System.Drawing.Point(112, 209)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(107, 33)
        Me.Label3.Text = "Query / Collect an order"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.Label3.Visible = False
        '
        'pbBookInPutAway
        '
        Me.pbBookInPutAway.Image = CType(resources.GetObject("pbBookInPutAway.Image"), System.Drawing.Image)
        Me.pbBookInPutAway.Location = New System.Drawing.Point(31, 24)
        Me.pbBookInPutAway.Name = "pbBookInPutAway"
        Me.pbBookInPutAway.Size = New System.Drawing.Size(60, 60)
        Me.pbBookInPutAway.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblBookInPutAway
        '
        Me.lblBookInPutAway.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblBookInPutAway.Location = New System.Drawing.Point(7, 87)
        Me.lblBookInPutAway.Name = "lblBookInPutAway"
        Me.lblBookInPutAway.Size = New System.Drawing.Size(111, 49)
        Me.lblBookInPutAway.Text = "Book in and put away customer order"
        Me.lblBookInPutAway.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'PictureBox4
        '
        Me.PictureBox4.Image = CType(resources.GetObject("PictureBox4.Image"), System.Drawing.Image)
        Me.PictureBox4.Location = New System.Drawing.Point(31, 24)
        Me.PictureBox4.Name = "PictureBox4"
        Me.PictureBox4.Size = New System.Drawing.Size(60, 60)
        Me.PictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox4.Visible = False
        '
        'pbPutWayMove
        '
        Me.pbPutWayMove.Image = CType(resources.GetObject("pbPutWayMove.Image"), System.Drawing.Image)
        Me.pbPutWayMove.Location = New System.Drawing.Point(31, 146)
        Me.pbPutWayMove.Name = "pbPutWayMove"
        Me.pbPutWayMove.Size = New System.Drawing.Size(60, 60)
        Me.pbPutWayMove.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblPutWayMove
        '
        Me.lblPutWayMove.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblPutWayMove.Location = New System.Drawing.Point(10, 209)
        Me.lblPutWayMove.Name = "lblPutWayMove"
        Me.lblPutWayMove.Size = New System.Drawing.Size(101, 48)
        Me.lblPutWayMove.Text = "Put away / Move customer order"
        Me.lblPutWayMove.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'PictureBox2
        '
        Me.PictureBox2.Image = CType(resources.GetObject("PictureBox2.Image"), System.Drawing.Image)
        Me.PictureBox2.Location = New System.Drawing.Point(31, 146)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(60, 60)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox2.Visible = False
        '
        'lblBookInOnly
        '
        Me.lblBookInOnly.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblBookInOnly.Location = New System.Drawing.Point(114, 87)
        Me.lblBookInOnly.Name = "lblBookInOnly"
        Me.lblBookInOnly.Size = New System.Drawing.Size(109, 48)
        Me.lblBookInOnly.Text = "Book in customer order only"
        Me.lblBookInOnly.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbBookInOnly
        '
        Me.pbBookInOnly.Image = CType(resources.GetObject("pbBookInOnly.Image"), System.Drawing.Image)
        Me.pbBookInOnly.Location = New System.Drawing.Point(138, 24)
        Me.pbBookInOnly.Name = "pbBookInOnly"
        Me.pbBookInOnly.Size = New System.Drawing.Size(60, 60)
        Me.pbBookInOnly.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'tpLogOff
        '
        Me.tpLogOff.Controls.Add(Me.lblLogOff)
        Me.tpLogOff.Controls.Add(Me.pbLogOff)
        Me.tpLogOff.Location = New System.Drawing.Point(0, 0)
        Me.tpLogOff.Name = "tpLogOff"
        Me.tpLogOff.Size = New System.Drawing.Size(232, 265)
        Me.tpLogOff.Text = " Log Off "
        '
        'lblLogOff
        '
        Me.lblLogOff.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblLogOff.Location = New System.Drawing.Point(20, 87)
        Me.lblLogOff.Name = "lblLogOff"
        Me.lblLogOff.Size = New System.Drawing.Size(85, 18)
        Me.lblLogOff.Text = "Log Off"
        Me.lblLogOff.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbLogOff
        '
        Me.pbLogOff.Image = CType(resources.GetObject("pbLogOff.Image"), System.Drawing.Image)
        Me.pbLogOff.Location = New System.Drawing.Point(31, 24)
        Me.pbLogOff.Name = "pbLogOff"
        Me.pbLogOff.Size = New System.Drawing.Size(60, 60)
        Me.pbLogOff.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmMainMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 300)
        Me.ControlBox = False
        Me.Controls.Add(Me.tcMainMenu)
        Me.Name = "frmMainMenu"
        Me.Text = "Order && Collect"
        Me.tcMainMenu.ResumeLayout(False)
        Me.tpBookIn.ResumeLayout(False)
        Me.tpLogOff.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tcMainMenu As System.Windows.Forms.TabControl
    Friend WithEvents tpBookIn As System.Windows.Forms.TabPage
    Friend WithEvents pbBookInPutAway As System.Windows.Forms.PictureBox
    Friend WithEvents lblBookInPutAway As System.Windows.Forms.Label
    Friend WithEvents PictureBox4 As System.Windows.Forms.PictureBox
    Friend WithEvents pbPutWayMove As System.Windows.Forms.PictureBox
    Friend WithEvents lblPutWayMove As System.Windows.Forms.Label
    Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
    Friend WithEvents lblBookInOnly As System.Windows.Forms.Label
    Friend WithEvents pbBookInOnly As System.Windows.Forms.PictureBox
    Friend WithEvents tpLogOff As System.Windows.Forms.TabPage
    Friend WithEvents lblLogOff As System.Windows.Forms.Label
    Friend WithEvents pbLogOff As System.Windows.Forms.PictureBox
    Friend WithEvents pbOrderCollect As System.Windows.Forms.PictureBox
    Friend WithEvents Label3 As System.Windows.Forms.Label

End Class
