<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmMemoryStatusInfo
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMemoryStatusInfo))
        Me.lblPercentUsedMemHeader = New System.Windows.Forms.Label
        Me.lblPercentUsedMem = New System.Windows.Forms.Label
        Me.lblTotalMemoryHeader = New System.Windows.Forms.Label
        Me.lblFreePhysicalMemHeader = New System.Windows.Forms.Label
        Me.lblTotalPageFileSizeHeader = New System.Windows.Forms.Label
        Me.lblFreePageFileSizeHeader = New System.Windows.Forms.Label
        Me.lblTotalVirtualMemHeader = New System.Windows.Forms.Label
        Me.lblFreeVirtualMemHeader = New System.Windows.Forms.Label
        Me.lblTotalPhysicalMemory = New System.Windows.Forms.Label
        Me.lblFreePhysicalMem = New System.Windows.Forms.Label
        Me.lblTotalPageFileSize = New System.Windows.Forms.Label
        Me.lblFreePageFileSize = New System.Windows.Forms.Label
        Me.lblTotalVirtualMem = New System.Windows.Forms.Label
        Me.lblFreeVirtualMem = New System.Windows.Forms.Label
        Me.btnOk = New System.Windows.Forms.PictureBox
        Me.objStatusBar = New McUtilities.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblPercentUsedMemHeader
        '
        Me.lblPercentUsedMemHeader.BackColor = System.Drawing.Color.WhiteSmoke
        Me.lblPercentUsedMemHeader.Location = New System.Drawing.Point(13, 15)
        Me.lblPercentUsedMemHeader.Name = "lblPercentUsedMemHeader"
        Me.lblPercentUsedMemHeader.Size = New System.Drawing.Size(88, 20)
        Me.lblPercentUsedMemHeader.Text = "Used Memory:"
        '
        'lblPercentUsedMem
        '
        Me.lblPercentUsedMem.BackColor = System.Drawing.Color.WhiteSmoke
        Me.lblPercentUsedMem.Location = New System.Drawing.Point(152, 15)
        Me.lblPercentUsedMem.Name = "lblPercentUsedMem"
        Me.lblPercentUsedMem.Size = New System.Drawing.Size(67, 20)
        Me.lblPercentUsedMem.Text = "100%"
        '
        'lblTotalMemoryHeader
        '
        Me.lblTotalMemoryHeader.Location = New System.Drawing.Point(13, 44)
        Me.lblTotalMemoryHeader.Name = "lblTotalMemoryHeader"
        Me.lblTotalMemoryHeader.Size = New System.Drawing.Size(133, 20)
        Me.lblTotalMemoryHeader.Text = "Total Physical Memory:"
        '
        'lblFreePhysicalMemHeader
        '
        Me.lblFreePhysicalMemHeader.Location = New System.Drawing.Point(13, 76)
        Me.lblFreePhysicalMemHeader.Name = "lblFreePhysicalMemHeader"
        Me.lblFreePhysicalMemHeader.Size = New System.Drawing.Size(133, 20)
        Me.lblFreePhysicalMemHeader.Text = "Free Physical Memory:"
        '
        'lblTotalPageFileSizeHeader
        '
        Me.lblTotalPageFileSizeHeader.Location = New System.Drawing.Point(13, 109)
        Me.lblTotalPageFileSizeHeader.Name = "lblTotalPageFileSizeHeader"
        Me.lblTotalPageFileSizeHeader.Size = New System.Drawing.Size(122, 20)
        Me.lblTotalPageFileSizeHeader.Text = "Total Page File Size:"
        '
        'lblFreePageFileSizeHeader
        '
        Me.lblFreePageFileSizeHeader.Location = New System.Drawing.Point(13, 140)
        Me.lblFreePageFileSizeHeader.Name = "lblFreePageFileSizeHeader"
        Me.lblFreePageFileSizeHeader.Size = New System.Drawing.Size(122, 20)
        Me.lblFreePageFileSizeHeader.Text = "Free Page File Size:"
        '
        'lblTotalVirtualMemHeader
        '
        Me.lblTotalVirtualMemHeader.Location = New System.Drawing.Point(13, 170)
        Me.lblTotalVirtualMemHeader.Name = "lblTotalVirtualMemHeader"
        Me.lblTotalVirtualMemHeader.Size = New System.Drawing.Size(133, 20)
        Me.lblTotalVirtualMemHeader.Text = "Total Virtual Memory:"
        '
        'lblFreeVirtualMemHeader
        '
        Me.lblFreeVirtualMemHeader.Location = New System.Drawing.Point(13, 202)
        Me.lblFreeVirtualMemHeader.Name = "lblFreeVirtualMemHeader"
        Me.lblFreeVirtualMemHeader.Size = New System.Drawing.Size(122, 20)
        Me.lblFreeVirtualMemHeader.Text = "Free Virtual Memory:"
        '
        'lblTotalPhysicalMemory
        '
        Me.lblTotalPhysicalMemory.Location = New System.Drawing.Point(152, 44)
        Me.lblTotalPhysicalMemory.Name = "lblTotalPhysicalMemory"
        Me.lblTotalPhysicalMemory.Size = New System.Drawing.Size(85, 20)
        Me.lblTotalPhysicalMemory.Text = "100KB"
        '
        'lblFreePhysicalMem
        '
        Me.lblFreePhysicalMem.Location = New System.Drawing.Point(152, 76)
        Me.lblFreePhysicalMem.Name = "lblFreePhysicalMem"
        Me.lblFreePhysicalMem.Size = New System.Drawing.Size(85, 20)
        Me.lblFreePhysicalMem.Text = "100KB"
        '
        'lblTotalPageFileSize
        '
        Me.lblTotalPageFileSize.Location = New System.Drawing.Point(152, 109)
        Me.lblTotalPageFileSize.Name = "lblTotalPageFileSize"
        Me.lblTotalPageFileSize.Size = New System.Drawing.Size(85, 20)
        Me.lblTotalPageFileSize.Text = "100KB"
        '
        'lblFreePageFileSize
        '
        Me.lblFreePageFileSize.Location = New System.Drawing.Point(152, 140)
        Me.lblFreePageFileSize.Name = "lblFreePageFileSize"
        Me.lblFreePageFileSize.Size = New System.Drawing.Size(85, 20)
        Me.lblFreePageFileSize.Text = "100KB"
        '
        'lblTotalVirtualMem
        '
        Me.lblTotalVirtualMem.Location = New System.Drawing.Point(152, 170)
        Me.lblTotalVirtualMem.Name = "lblTotalVirtualMem"
        Me.lblTotalVirtualMem.Size = New System.Drawing.Size(85, 20)
        Me.lblTotalVirtualMem.Text = "100KB"
        '
        'lblFreeVirtualMem
        '
        Me.lblFreeVirtualMem.Location = New System.Drawing.Point(152, 202)
        Me.lblFreeVirtualMem.Name = "lblFreeVirtualMem"
        Me.lblFreeVirtualMem.Size = New System.Drawing.Size(85, 20)
        Me.lblFreeVirtualMem.Text = "100KB"
        '
        'btnOk
        '
        Me.btnOk.Image = CType(resources.GetObject("btnOk.Image"), System.Drawing.Image)
        Me.btnOk.Location = New System.Drawing.Point(105, 229)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(41, 40)
        Me.btnOk.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 39
        '
        'frmMemoryStatusInfo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.lblFreeVirtualMem)
        Me.Controls.Add(Me.lblTotalVirtualMem)
        Me.Controls.Add(Me.lblFreePageFileSize)
        Me.Controls.Add(Me.lblTotalPageFileSize)
        Me.Controls.Add(Me.lblFreePhysicalMem)
        Me.Controls.Add(Me.lblTotalPhysicalMemory)
        Me.Controls.Add(Me.lblFreeVirtualMemHeader)
        Me.Controls.Add(Me.lblTotalVirtualMemHeader)
        Me.Controls.Add(Me.lblFreePageFileSizeHeader)
        Me.Controls.Add(Me.lblTotalPageFileSizeHeader)
        Me.Controls.Add(Me.lblFreePhysicalMemHeader)
        Me.Controls.Add(Me.lblTotalMemoryHeader)
        Me.Controls.Add(Me.lblPercentUsedMem)
        Me.Controls.Add(Me.lblPercentUsedMemHeader)
        Me.Name = "frmMemoryStatusInfo"
        Me.Text = "Memory Status"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblPercentUsedMemHeader As System.Windows.Forms.Label
    Friend WithEvents lblPercentUsedMem As System.Windows.Forms.Label
    Friend WithEvents lblTotalMemoryHeader As System.Windows.Forms.Label
    Friend WithEvents lblFreePhysicalMemHeader As System.Windows.Forms.Label
    Friend WithEvents lblTotalPageFileSizeHeader As System.Windows.Forms.Label
    Friend WithEvents lblFreePageFileSizeHeader As System.Windows.Forms.Label
    Friend WithEvents lblTotalVirtualMemHeader As System.Windows.Forms.Label
    Friend WithEvents lblFreeVirtualMemHeader As System.Windows.Forms.Label
    Friend WithEvents lblTotalPhysicalMemory As System.Windows.Forms.Label
    Friend WithEvents lblFreePhysicalMem As System.Windows.Forms.Label
    Friend WithEvents lblTotalPageFileSize As System.Windows.Forms.Label
    Friend WithEvents lblFreePageFileSize As System.Windows.Forms.Label
    Friend WithEvents lblTotalVirtualMem As System.Windows.Forms.Label
    Friend WithEvents lblFreeVirtualMem As System.Windows.Forms.Label
    Friend WithEvents btnOk As System.Windows.Forms.PictureBox
    Public WithEvents objStatusBar As McUtilities.CustomStatusBar
End Class
