<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmViewLogFileListScreen
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmViewLogFileListScreen))
        Me.lblSelect = New System.Windows.Forms.Label
        Me.lstvwLogFiles = New System.Windows.Forms.ListView
        Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader2 = New System.Windows.Forms.ColumnHeader
        Me.pgBar = New System.Windows.Forms.ProgressBar
        Me.lblCurrFile = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.lblTotalFiles = New System.Windows.Forms.Label
        Me.Quit = New System.Windows.Forms.PictureBox
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.pb_SendAll = New System.Windows.Forms.PictureBox
        Me.objStatusBar = New McUtilities.CustomStatusBar
        Me.lblStatus = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblSelect
        '
        Me.lblSelect.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelect.Location = New System.Drawing.Point(-1, 248)
        Me.lblSelect.Name = "lblSelect"
        Me.lblSelect.Size = New System.Drawing.Size(99, 20)
        Me.lblSelect.Text = "Select Log File"
        '
        'lstvwLogFiles
        '
        Me.lstvwLogFiles.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.lstvwLogFiles.Columns.Add(Me.ColumnHeader1)
        Me.lstvwLogFiles.Columns.Add(Me.ColumnHeader2)
        Me.lstvwLogFiles.FullRowSelect = True
        Me.lstvwLogFiles.Location = New System.Drawing.Point(0, 24)
        Me.lstvwLogFiles.Name = "lstvwLogFiles"
        Me.lstvwLogFiles.Size = New System.Drawing.Size(240, 208)
        Me.lstvwLogFiles.TabIndex = 1
        Me.lstvwLogFiles.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "File Name"
        Me.ColumnHeader1.Width = 143
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Date Modified"
        Me.ColumnHeader2.Width = 93
        '
        'pgBar
        '
        Me.pgBar.Location = New System.Drawing.Point(3, 6)
        Me.pgBar.Name = "pgBar"
        Me.pgBar.Size = New System.Drawing.Size(165, 12)
        '
        'lblCurrFile
        '
        Me.lblCurrFile.Location = New System.Drawing.Point(168, 1)
        Me.lblCurrFile.Name = "lblCurrFile"
        Me.lblCurrFile.Size = New System.Drawing.Size(33, 20)
        Me.lblCurrFile.Text = "0000"
        Me.lblCurrFile.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(199, 1)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(10, 20)
        Me.Label2.Text = "/"
        '
        'lblTotalFiles
        '
        Me.lblTotalFiles.Location = New System.Drawing.Point(207, 1)
        Me.lblTotalFiles.Name = "lblTotalFiles"
        Me.lblTotalFiles.Size = New System.Drawing.Size(33, 20)
        Me.lblTotalFiles.Text = "0000"
        '
        'Quit
        '
        Me.Quit.Image = CType(resources.GetObject("Quit.Image"), System.Drawing.Image)
        Me.Quit.Location = New System.Drawing.Point(183, 243)
        Me.Quit.Name = "Quit"
        Me.Quit.Size = New System.Drawing.Size(50, 24)
        Me.Quit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(93, 243)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(24, 21)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pb_SendAll
        '
        Me.pb_SendAll.Image = CType(resources.GetObject("pb_SendAll.Image"), System.Drawing.Image)
        Me.pb_SendAll.Location = New System.Drawing.Point(127, 243)
        Me.pb_SendAll.Name = "pb_SendAll"
        Me.pb_SendAll.Size = New System.Drawing.Size(50, 24)
        Me.pb_SendAll.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 6
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(5, 2)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(227, 18)
        Me.lblStatus.Visible = False
        '
        'frmViewLogFileListScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.pb_SendAll)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.Quit)
        Me.Controls.Add(Me.lblTotalFiles)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lblCurrFile)
        Me.Controls.Add(Me.pgBar)
        Me.Controls.Add(Me.lstvwLogFiles)
        Me.Controls.Add(Me.lblSelect)
        Me.KeyPreview = True
        Me.Name = "frmViewLogFileListScreen"
        Me.Text = "View Log File"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblSelect As System.Windows.Forms.Label
    Friend WithEvents lstvwLogFiles As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents pgBar As System.Windows.Forms.ProgressBar
    Friend WithEvents lblCurrFile As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblTotalFiles As System.Windows.Forms.Label
    Friend WithEvents Quit As System.Windows.Forms.PictureBox
    Public WithEvents objStatusBar As McUtilities.CustomStatusBar
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents pb_SendAll As System.Windows.Forms.PictureBox
    Friend WithEvents lblStatus As System.Windows.Forms.Label
End Class
