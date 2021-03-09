<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmUtilitiesMenu
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUtilitiesMenu))
        Me.lblLogFile = New System.Windows.Forms.Label
        Me.tbpgSysInfo = New System.Windows.Forms.TabPage
        Me.lblIPConfg = New System.Windows.Forms.Label
        Me.pbAirbeamIPConfig = New System.Windows.Forms.PictureBox
        Me.lblMemStatusMenu = New System.Windows.Forms.Label
        Me.pbMemStatusMenu = New System.Windows.Forms.PictureBox
        Me.lblIPInfoMenu = New System.Windows.Forms.Label
        Me.pbIPMenu = New System.Windows.Forms.PictureBox
        Me.pbViewLogMenu = New System.Windows.Forms.PictureBox
        Me.tbpgFileSysInfo = New System.Windows.Forms.TabPage
        Me.lblViewFileStatus = New System.Windows.Forms.Label
        Me.pbViewFileStatMenu = New System.Windows.Forms.PictureBox
        Me.lblReloadRefData = New System.Windows.Forms.Label
        Me.lblViewExpData = New System.Windows.Forms.Label
        Me.pbReloadRefMenu = New System.Windows.Forms.PictureBox
        Me.pbViewExpDataMenu = New System.Windows.Forms.PictureBox
        Me.tbShlfMgmtMenu = New System.Windows.Forms.TabControl
        Me.Quit = New System.Windows.Forms.TabPage
        Me.lblLogOff = New System.Windows.Forms.Label
        Me.pbLogOff = New System.Windows.Forms.PictureBox
        Me.objStatusBar = New McUtilities.CustomStatusBar
        Me.tbpgSysInfo.SuspendLayout()
        Me.tbpgFileSysInfo.SuspendLayout()
        Me.tbShlfMgmtMenu.SuspendLayout()
        Me.Quit.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblLogFile
        '
        Me.lblLogFile.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblLogFile.Location = New System.Drawing.Point(7, 69)
        Me.lblLogFile.Name = "lblLogFile"
        Me.lblLogFile.Size = New System.Drawing.Size(88, 17)
        Me.lblLogFile.Text = "Log File Info"
        Me.lblLogFile.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'tbpgSysInfo
        '
        Me.tbpgSysInfo.Controls.Add(Me.lblIPConfg)
        Me.tbpgSysInfo.Controls.Add(Me.pbAirbeamIPConfig)
        Me.tbpgSysInfo.Controls.Add(Me.lblMemStatusMenu)
        Me.tbpgSysInfo.Controls.Add(Me.pbMemStatusMenu)
        Me.tbpgSysInfo.Controls.Add(Me.lblIPInfoMenu)
        Me.tbpgSysInfo.Controls.Add(Me.pbIPMenu)
        Me.tbpgSysInfo.Location = New System.Drawing.Point(0, 0)
        Me.tbpgSysInfo.Name = "tbpgSysInfo"
        Me.tbpgSysInfo.Size = New System.Drawing.Size(240, 256)
        Me.tbpgSysInfo.Text = "SysInfo"
        '
        'lblIPConfg
        '
        Me.lblIPConfg.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblIPConfg.Location = New System.Drawing.Point(13, 164)
        Me.lblIPConfg.Name = "lblIPConfg"
        Me.lblIPConfg.Size = New System.Drawing.Size(97, 33)
        Me.lblIPConfg.Text = "AirBEAM IP" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Configuration"
        Me.lblIPConfg.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbAirbeamIPConfig
        '
        Me.pbAirbeamIPConfig.Image = CType(resources.GetObject("pbAirbeamIPConfig.Image"), System.Drawing.Image)
        Me.pbAirbeamIPConfig.Location = New System.Drawing.Point(31, 116)
        Me.pbAirbeamIPConfig.Name = "pbAirbeamIPConfig"
        Me.pbAirbeamIPConfig.Size = New System.Drawing.Size(46, 45)
        Me.pbAirbeamIPConfig.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblMemStatusMenu
        '
        Me.lblMemStatusMenu.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMemStatusMenu.Location = New System.Drawing.Point(116, 69)
        Me.lblMemStatusMenu.Name = "lblMemStatusMenu"
        Me.lblMemStatusMenu.Size = New System.Drawing.Size(111, 15)
        Me.lblMemStatusMenu.Text = "Memory Status"
        Me.lblMemStatusMenu.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbMemStatusMenu
        '
        Me.pbMemStatusMenu.Image = CType(resources.GetObject("pbMemStatusMenu.Image"), System.Drawing.Image)
        Me.pbMemStatusMenu.Location = New System.Drawing.Point(151, 21)
        Me.pbMemStatusMenu.Name = "pbMemStatusMenu"
        Me.pbMemStatusMenu.Size = New System.Drawing.Size(46, 45)
        Me.pbMemStatusMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblIPInfoMenu
        '
        Me.lblIPInfoMenu.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblIPInfoMenu.Location = New System.Drawing.Point(13, 69)
        Me.lblIPInfoMenu.Name = "lblIPInfoMenu"
        Me.lblIPInfoMenu.Size = New System.Drawing.Size(97, 15)
        Me.lblIPInfoMenu.Text = "IP Details"
        Me.lblIPInfoMenu.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbIPMenu
        '
        Me.pbIPMenu.Image = CType(resources.GetObject("pbIPMenu.Image"), System.Drawing.Image)
        Me.pbIPMenu.Location = New System.Drawing.Point(31, 21)
        Me.pbIPMenu.Name = "pbIPMenu"
        Me.pbIPMenu.Size = New System.Drawing.Size(46, 45)
        Me.pbIPMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbViewLogMenu
        '
        Me.pbViewLogMenu.Image = CType(resources.GetObject("pbViewLogMenu.Image"), System.Drawing.Image)
        Me.pbViewLogMenu.Location = New System.Drawing.Point(17, 16)
        Me.pbViewLogMenu.Name = "pbViewLogMenu"
        Me.pbViewLogMenu.Size = New System.Drawing.Size(60, 50)
        Me.pbViewLogMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'tbpgFileSysInfo
        '
        Me.tbpgFileSysInfo.Controls.Add(Me.lblViewFileStatus)
        Me.tbpgFileSysInfo.Controls.Add(Me.pbViewFileStatMenu)
        Me.tbpgFileSysInfo.Controls.Add(Me.lblReloadRefData)
        Me.tbpgFileSysInfo.Controls.Add(Me.lblViewExpData)
        Me.tbpgFileSysInfo.Controls.Add(Me.pbReloadRefMenu)
        Me.tbpgFileSysInfo.Controls.Add(Me.pbViewExpDataMenu)
        Me.tbpgFileSysInfo.Controls.Add(Me.lblLogFile)
        Me.tbpgFileSysInfo.Controls.Add(Me.pbViewLogMenu)
        Me.tbpgFileSysInfo.Location = New System.Drawing.Point(0, 0)
        Me.tbpgFileSysInfo.Name = "tbpgFileSysInfo"
        Me.tbpgFileSysInfo.Size = New System.Drawing.Size(232, 253)
        Me.tbpgFileSysInfo.Text = "FileSysInfo"
        '
        'lblViewFileStatus
        '
        Me.lblViewFileStatus.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblViewFileStatus.Location = New System.Drawing.Point(140, 69)
        Me.lblViewFileStatus.Name = "lblViewFileStatus"
        Me.lblViewFileStatus.Size = New System.Drawing.Size(88, 31)
        Me.lblViewFileStatus.Text = "View File Status"
        Me.lblViewFileStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbViewFileStatMenu
        '
        Me.pbViewFileStatMenu.Image = CType(resources.GetObject("pbViewFileStatMenu.Image"), System.Drawing.Image)
        Me.pbViewFileStatMenu.Location = New System.Drawing.Point(150, 16)
        Me.pbViewFileStatMenu.Name = "pbViewFileStatMenu"
        Me.pbViewFileStatMenu.Size = New System.Drawing.Size(60, 50)
        Me.pbViewFileStatMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblReloadRefData
        '
        Me.lblReloadRefData.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblReloadRefData.Location = New System.Drawing.Point(129, 153)
        Me.lblReloadRefData.Name = "lblReloadRefData"
        Me.lblReloadRefData.Size = New System.Drawing.Size(104, 31)
        Me.lblReloadRefData.Text = "Reload Reference Data"
        Me.lblReloadRefData.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblViewExpData
        '
        Me.lblViewExpData.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblViewExpData.Location = New System.Drawing.Point(7, 153)
        Me.lblViewExpData.Name = "lblViewExpData"
        Me.lblViewExpData.Size = New System.Drawing.Size(88, 31)
        Me.lblViewExpData.Text = "View Export Data"
        Me.lblViewExpData.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbReloadRefMenu
        '
        Me.pbReloadRefMenu.Image = CType(resources.GetObject("pbReloadRefMenu.Image"), System.Drawing.Image)
        Me.pbReloadRefMenu.Location = New System.Drawing.Point(150, 100)
        Me.pbReloadRefMenu.Name = "pbReloadRefMenu"
        Me.pbReloadRefMenu.Size = New System.Drawing.Size(60, 50)
        Me.pbReloadRefMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbViewExpDataMenu
        '
        Me.pbViewExpDataMenu.Image = CType(resources.GetObject("pbViewExpDataMenu.Image"), System.Drawing.Image)
        Me.pbViewExpDataMenu.Location = New System.Drawing.Point(17, 100)
        Me.pbViewExpDataMenu.Name = "pbViewExpDataMenu"
        Me.pbViewExpDataMenu.Size = New System.Drawing.Size(60, 50)
        Me.pbViewExpDataMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'tbShlfMgmtMenu
        '
        Me.tbShlfMgmtMenu.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.tbShlfMgmtMenu.Controls.Add(Me.tbpgSysInfo)
        Me.tbShlfMgmtMenu.Controls.Add(Me.tbpgFileSysInfo)
        Me.tbShlfMgmtMenu.Controls.Add(Me.Quit)
        Me.tbShlfMgmtMenu.Dock = System.Windows.Forms.DockStyle.None
        Me.tbShlfMgmtMenu.Location = New System.Drawing.Point(0, 0)
        Me.tbShlfMgmtMenu.Name = "tbShlfMgmtMenu"
        Me.tbShlfMgmtMenu.SelectedIndex = 0
        Me.tbShlfMgmtMenu.Size = New System.Drawing.Size(240, 279)
        Me.tbShlfMgmtMenu.TabIndex = 14
        '
        'Quit
        '
        Me.Quit.Controls.Add(Me.lblLogOff)
        Me.Quit.Controls.Add(Me.pbLogOff)
        Me.Quit.Location = New System.Drawing.Point(0, 0)
        Me.Quit.Name = "Quit"
        Me.Quit.Size = New System.Drawing.Size(232, 253)
        Me.Quit.Text = "Quit"
        '
        'lblLogOff
        '
        Me.lblLogOff.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblLogOff.Location = New System.Drawing.Point(31, 80)
        Me.lblLogOff.Name = "lblLogOff"
        Me.lblLogOff.Size = New System.Drawing.Size(70, 18)
        Me.lblLogOff.Text = "Log Off"
        Me.lblLogOff.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbLogOff
        '
        Me.pbLogOff.Image = CType(resources.GetObject("pbLogOff.Image"), System.Drawing.Image)
        Me.pbLogOff.Location = New System.Drawing.Point(31, 17)
        Me.pbLogOff.Name = "pbLogOff"
        Me.pbLogOff.Size = New System.Drawing.Size(60, 60)
        Me.pbLogOff.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 15
        '
        'frmUtilitiesMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.tbShlfMgmtMenu)
        Me.Name = "frmUtilitiesMenu"
        Me.Text = "Utilities"
        Me.tbpgSysInfo.ResumeLayout(False)
        Me.tbpgFileSysInfo.ResumeLayout(False)
        Me.tbShlfMgmtMenu.ResumeLayout(False)
        Me.Quit.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblLogFile As System.Windows.Forms.Label
    Friend WithEvents tbpgSysInfo As System.Windows.Forms.TabPage
    Friend WithEvents lblMemStatusMenu As System.Windows.Forms.Label
    Friend WithEvents pbMemStatusMenu As System.Windows.Forms.PictureBox
    Friend WithEvents lblIPInfoMenu As System.Windows.Forms.Label
    Friend WithEvents pbViewLogMenu As System.Windows.Forms.PictureBox
    Friend WithEvents tbpgFileSysInfo As System.Windows.Forms.TabPage
    Friend WithEvents pbIPMenu As System.Windows.Forms.PictureBox
    Friend WithEvents lblReloadRefData As System.Windows.Forms.Label
    Friend WithEvents lblViewExpData As System.Windows.Forms.Label
    Friend WithEvents pbReloadRefMenu As System.Windows.Forms.PictureBox
    Friend WithEvents pbViewExpDataMenu As System.Windows.Forms.PictureBox
    Friend WithEvents lblViewFileStatus As System.Windows.Forms.Label
    Friend WithEvents pbViewFileStatMenu As System.Windows.Forms.PictureBox
    Friend WithEvents Quit As System.Windows.Forms.TabPage
    Friend WithEvents lblLogOff As System.Windows.Forms.Label
    Friend WithEvents pbLogOff As System.Windows.Forms.PictureBox
    Public WithEvents objStatusBar As McUtilities.CustomStatusBar
    Protected WithEvents tbShlfMgmtMenu As System.Windows.Forms.TabControl
    Friend WithEvents lblIPConfg As System.Windows.Forms.Label
    Friend WithEvents pbAirbeamIPConfig As System.Windows.Forms.PictureBox
End Class
