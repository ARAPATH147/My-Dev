<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmAPLocalPrinterUtilities
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAPLocalPrinterUtilities))
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.btn_Quit_small = New CustomButtons.btn_Quit_small
        Me.pbSendFonts = New System.Windows.Forms.PictureBox
        Me.pbTestPrint = New System.Windows.Forms.PictureBox
        Me.lblSendFonts = New System.Windows.Forms.Label
        Me.lblTestPrint = New System.Windows.Forms.Label
        Me.pbTestClearancePrint = New System.Windows.Forms.PictureBox
        Me.lblTestClPrint = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 31
        '
        'btn_Quit_small
        '
        Me.btn_Quit_small.BackColor = System.Drawing.Color.Transparent
        Me.btn_Quit_small.Location = New System.Drawing.Point(152, 245)
        Me.btn_Quit_small.Name = "btn_Quit_small"
        Me.btn_Quit_small.Size = New System.Drawing.Size(50, 24)
        Me.btn_Quit_small.TabIndex = 32
        '
        'pbSendFonts
        '
        Me.pbSendFonts.Image = CType(resources.GetObject("pbSendFonts.Image"), System.Drawing.Image)
        Me.pbSendFonts.Location = New System.Drawing.Point(36, 17)
        Me.pbSendFonts.Name = "pbSendFonts"
        Me.pbSendFonts.Size = New System.Drawing.Size(60, 72)
        Me.pbSendFonts.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbTestPrint
        '
        Me.pbTestPrint.Image = CType(resources.GetObject("pbTestPrint.Image"), System.Drawing.Image)
        Me.pbTestPrint.Location = New System.Drawing.Point(133, 17)
        Me.pbTestPrint.Name = "pbTestPrint"
        Me.pbTestPrint.Size = New System.Drawing.Size(69, 72)
        Me.pbTestPrint.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblSendFonts
        '
        Me.lblSendFonts.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSendFonts.Location = New System.Drawing.Point(36, 98)
        Me.lblSendFonts.Name = "lblSendFonts"
        Me.lblSendFonts.Size = New System.Drawing.Size(79, 20)
        Me.lblSendFonts.Text = "Send Fonts"
        '
        'lblTestPrint
        '
        Me.lblTestPrint.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTestPrint.Location = New System.Drawing.Point(133, 98)
        Me.lblTestPrint.Name = "lblTestPrint"
        Me.lblTestPrint.Size = New System.Drawing.Size(100, 20)
        Me.lblTestPrint.Text = "SEL Test Print"
        '
        'pbTestClearancePrint
        '
        Me.pbTestClearancePrint.Image = CType(resources.GetObject("pbTestClearancePrint.Image"), System.Drawing.Image)
        Me.pbTestClearancePrint.Location = New System.Drawing.Point(36, 121)
        Me.pbTestClearancePrint.Name = "pbTestClearancePrint"
        Me.pbTestClearancePrint.Size = New System.Drawing.Size(96, 87)
        Me.pbTestClearancePrint.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblTestClPrint
        '
        Me.lblTestClPrint.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTestClPrint.Location = New System.Drawing.Point(36, 218)
        Me.lblTestClPrint.Name = "lblTestClPrint"
        Me.lblTestClPrint.Size = New System.Drawing.Size(130, 22)
        Me.lblTestClPrint.Text = "Clearance Test Print"
        '
        'frmAPLocalPrinterUtilities
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.pbTestClearancePrint)
        Me.Controls.Add(Me.lblTestPrint)
        Me.Controls.Add(Me.lblTestClPrint)
        Me.Controls.Add(Me.lblSendFonts)
        Me.Controls.Add(Me.pbTestPrint)
        Me.Controls.Add(Me.pbSendFonts)
        Me.Controls.Add(Me.btn_Quit_small)
        Me.Controls.Add(Me.objStatusBar)
        Me.Name = "frmAPLocalPrinterUtilities"
        Me.Text = "Local Printer Utilities"
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents btn_Quit_small As CustomButtons.btn_Quit_small
    Friend WithEvents pbSendFonts As System.Windows.Forms.PictureBox
    Friend WithEvents pbTestPrint As System.Windows.Forms.PictureBox
    Friend WithEvents lblSendFonts As System.Windows.Forms.Label
    Friend WithEvents lblTestPrint As System.Windows.Forms.Label
    Friend WithEvents pbTestClearancePrint As System.Windows.Forms.PictureBox
    Friend WithEvents lblTestClPrint As System.Windows.Forms.Label
End Class
