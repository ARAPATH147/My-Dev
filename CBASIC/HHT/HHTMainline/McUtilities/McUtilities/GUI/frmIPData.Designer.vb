<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmIPData
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmIPData))
        Me.lblIP = New System.Windows.Forms.Label
        Me.lblIPVal = New System.Windows.Forms.Label
        Me.btnOk = New System.Windows.Forms.PictureBox
        Me.objStatusBar = New McUtilities.CustomStatusBar
        Me.lblSerial = New System.Windows.Forms.Label
        Me.lblSerialNumber = New System.Windows.Forms.Label
        Me.lblMAC = New System.Windows.Forms.Label
        Me.lblMACAddr = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblIP
        '
        Me.lblIP.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblIP.Location = New System.Drawing.Point(15, 32)
        Me.lblIP.Name = "lblIP"
        Me.lblIP.Size = New System.Drawing.Size(80, 20)
        Me.lblIP.Text = "IP Address:"
        '
        'lblIPVal
        '
        Me.lblIPVal.Location = New System.Drawing.Point(110, 32)
        Me.lblIPVal.Name = "lblIPVal"
        Me.lblIPVal.Size = New System.Drawing.Size(110, 20)
        Me.lblIPVal.Text = "127.0.0.1"
        '
        'btnOk
        '
        Me.btnOk.Image = CType(resources.GetObject("btnOk.Image"), System.Drawing.Image)
        Me.btnOk.Location = New System.Drawing.Point(110, 217)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(40, 40)
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
        'lblSerial
        '
        Me.lblSerial.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSerial.Location = New System.Drawing.Point(15, 70)
        Me.lblSerial.Name = "lblSerial"
        Me.lblSerial.Size = New System.Drawing.Size(80, 20)
        Me.lblSerial.Text = "Serial No.:"
        '
        'lblSerialNumber
        '
        Me.lblSerialNumber.Location = New System.Drawing.Point(110, 70)
        Me.lblSerialNumber.Name = "lblSerialNumber"
        Me.lblSerialNumber.Size = New System.Drawing.Size(127, 20)
        '
        'lblMAC
        '
        Me.lblMAC.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMAC.Location = New System.Drawing.Point(15, 107)
        Me.lblMAC.Name = "lblMAC"
        Me.lblMAC.Size = New System.Drawing.Size(80, 20)
        Me.lblMAC.Text = "MAC Addr:"
        '
        'lblMACAddr
        '
        Me.lblMACAddr.Location = New System.Drawing.Point(110, 107)
        Me.lblMACAddr.Name = "lblMACAddr"
        Me.lblMACAddr.Size = New System.Drawing.Size(127, 20)
        '
        'frmIPData
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.lblMACAddr)
        Me.Controls.Add(Me.lblSerialNumber)
        Me.Controls.Add(Me.lblIPVal)
        Me.Controls.Add(Me.lblMAC)
        Me.Controls.Add(Me.lblSerial)
        Me.Controls.Add(Me.lblIP)
        Me.KeyPreview = True
        Me.Name = "frmIPData"
        Me.Text = "IP Information"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblIP As System.Windows.Forms.Label
    Friend WithEvents lblIPVal As System.Windows.Forms.Label
    Friend WithEvents btnOk As System.Windows.Forms.PictureBox
    Public WithEvents objStatusBar As McUtilities.CustomStatusBar
    Friend WithEvents lblSerial As System.Windows.Forms.Label
    Friend WithEvents lblSerialNumber As System.Windows.Forms.Label
    Friend WithEvents lblMAC As System.Windows.Forms.Label
    Friend WithEvents lblMACAddr As System.Windows.Forms.Label
End Class
