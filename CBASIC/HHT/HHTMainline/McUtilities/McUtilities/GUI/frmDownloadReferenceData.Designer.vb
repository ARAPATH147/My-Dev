<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmDownloadReferenceData
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
    Private mainMenu1 As System.Windows.Forms.MainMenu

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDownloadReferenceData))
        Me.mainMenu1 = New System.Windows.Forms.MainMenu
        Me.lblProcess = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblProcessindicator = New System.Windows.Forms.Label
        Me.ConnectionTimer = New System.Windows.Forms.Timer
        Me.lblConnectionLost1 = New System.Windows.Forms.Label
        Me.pgBar = New System.Windows.Forms.ProgressBar
        Me.lblDBUpdation = New System.Windows.Forms.Label
        Me.lblPercentage = New System.Windows.Forms.Label
        Me.lblConnectionLost2 = New System.Windows.Forms.Label
        Me.pbFileTransfer = New System.Windows.Forms.PictureBox
        Me.cmdDownloadOptions = New System.Windows.Forms.ComboBox
        Me.btnOk = New System.Windows.Forms.PictureBox
        Me.btnQuit = New System.Windows.Forms.PictureBox
        Me.objStatusBar = New McUtilities.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblProcess
        '
        Me.lblProcess.BackColor = System.Drawing.Color.PaleTurquoise
        Me.lblProcess.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblProcess.Location = New System.Drawing.Point(0, 14)
        Me.lblProcess.Name = "lblProcess"
        Me.lblProcess.Size = New System.Drawing.Size(240, 18)
        Me.lblProcess.Text = "Uploading Reference Data"
        Me.lblProcess.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(8, 35)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(127, 20)
        Me.Label1.Text = "Current Operation:"
        '
        'lblProcessindicator
        '
        Me.lblProcessindicator.Location = New System.Drawing.Point(8, 55)
        Me.lblProcessindicator.Name = "lblProcessindicator"
        Me.lblProcessindicator.Size = New System.Drawing.Size(229, 36)
        Me.lblProcessindicator.Text = "Operation Status"
        '
        'lblConnectionLost1
        '
        Me.lblConnectionLost1.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblConnectionLost1.ForeColor = System.Drawing.Color.Red
        Me.lblConnectionLost1.Location = New System.Drawing.Point(3, 79)
        Me.lblConnectionLost1.Name = "lblConnectionLost1"
        Me.lblConnectionLost1.Size = New System.Drawing.Size(236, 23)
        '
        'pgBar
        '
        Me.pgBar.Location = New System.Drawing.Point(3, 240)
        Me.pgBar.Name = "pgBar"
        Me.pgBar.Size = New System.Drawing.Size(234, 16)
        '
        'lblDBUpdation
        '
        Me.lblDBUpdation.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblDBUpdation.Location = New System.Drawing.Point(4, 225)
        Me.lblDBUpdation.Name = "lblDBUpdation"
        Me.lblDBUpdation.Size = New System.Drawing.Size(157, 17)
        Me.lblDBUpdation.Text = "DB Update Status"
        '
        'lblPercentage
        '
        Me.lblPercentage.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblPercentage.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lblPercentage.Location = New System.Drawing.Point(177, 225)
        Me.lblPercentage.Name = "lblPercentage"
        Me.lblPercentage.Size = New System.Drawing.Size(49, 20)
        Me.lblPercentage.Text = "%"
        Me.lblPercentage.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblConnectionLost2
        '
        Me.lblConnectionLost2.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblConnectionLost2.ForeColor = System.Drawing.Color.Red
        Me.lblConnectionLost2.Location = New System.Drawing.Point(4, 105)
        Me.lblConnectionLost2.Name = "lblConnectionLost2"
        Me.lblConnectionLost2.Size = New System.Drawing.Size(235, 40)
        '
        'pbFileTransfer
        '
        Me.pbFileTransfer.Image = CType(resources.GetObject("pbFileTransfer.Image"), System.Drawing.Image)
        Me.pbFileTransfer.Location = New System.Drawing.Point(30, 105)
        Me.pbFileTransfer.Name = "pbFileTransfer"
        Me.pbFileTransfer.Size = New System.Drawing.Size(180, 70)
        Me.pbFileTransfer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'cmdDownloadOptions
        '
        Me.cmdDownloadOptions.Location = New System.Drawing.Point(30, 35)
        Me.cmdDownloadOptions.Name = "cmdDownloadOptions"
        Me.cmdDownloadOptions.Size = New System.Drawing.Size(180, 22)
        Me.cmdDownloadOptions.TabIndex = 9
        '
        'btnOk
        '
        Me.btnOk.Image = CType(resources.GetObject("btnOk.Image"), System.Drawing.Image)
        Me.btnOk.Location = New System.Drawing.Point(89, 225)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(40, 40)
        Me.btnOk.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'btnQuit
        '
        Me.btnQuit.Image = CType(resources.GetObject("btnQuit.Image"), System.Drawing.Image)
        Me.btnQuit.Location = New System.Drawing.Point(89, 225)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(40, 40)
        Me.btnQuit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 5
        '
        'frmDownloadReferenceData
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.cmdDownloadOptions)
        Me.Controls.Add(Me.pbFileTransfer)
        Me.Controls.Add(Me.pgBar)
        Me.Controls.Add(Me.lblPercentage)
        Me.Controls.Add(Me.lblDBUpdation)
        Me.Controls.Add(Me.lblProcessindicator)
        Me.Controls.Add(Me.lblConnectionLost2)
        Me.Controls.Add(Me.lblConnectionLost1)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblProcess)
        Me.Controls.Add(Me.btnOk)
        Me.KeyPreview = True
        Me.MinimizeBox = False
        Me.Name = "frmDownloadReferenceData"
        Me.Text = "Reference Data Upload"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblProcess As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblProcessindicator As System.Windows.Forms.Label
    Friend WithEvents objStatusBar As CustomStatusBar
    Friend WithEvents ConnectionTimer As System.Windows.Forms.Timer
    Friend WithEvents lblConnectionLost1 As System.Windows.Forms.Label
    Friend WithEvents pgBar As System.Windows.Forms.ProgressBar
    Friend WithEvents lblDBUpdation As System.Windows.Forms.Label
    Friend WithEvents lblPercentage As System.Windows.Forms.Label
    Friend WithEvents lblConnectionLost2 As System.Windows.Forms.Label
    Friend WithEvents pbFileTransfer As System.Windows.Forms.PictureBox
    Friend WithEvents cmdDownloadOptions As System.Windows.Forms.ComboBox
    Friend WithEvents btnOk As System.Windows.Forms.PictureBox
    Friend WithEvents btnQuit As System.Windows.Forms.PictureBox
End Class
