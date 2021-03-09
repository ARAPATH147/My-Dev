<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSMSummary
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
        Me.lblSumryHeading = New System.Windows.Forms.Label
        Me.lblScndItms = New System.Windows.Forms.Label
        Me.lblPLItms = New System.Windows.Forms.Label
        Me.lblSELQd = New System.Windows.Forms.Label
        Me.lblScndItmsVal = New System.Windows.Forms.Label
        Me.lblPLItmsVal = New System.Windows.Forms.Label
        Me.lblSELQdVal = New System.Windows.Forms.Label
        Me.lblUserMsg = New System.Windows.Forms.Label
        Me.btnOK = New CustomButtons.btn_Ok
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.lblActionDockTransmit = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblSumryHeading
        '
        Me.lblSumryHeading.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblSumryHeading.Location = New System.Drawing.Point(18, 15)
        Me.lblSumryHeading.Name = "lblSumryHeading"
        Me.lblSumryHeading.Size = New System.Drawing.Size(170, 20)
        Me.lblSumryHeading.Text = "Shelf Monitor Summary"
        '
        'lblScndItms
        '
        Me.lblScndItms.Location = New System.Drawing.Point(18, 48)
        Me.lblScndItms.Name = "lblScndItms"
        Me.lblScndItms.Size = New System.Drawing.Size(95, 15)
        Me.lblScndItms.Text = "Scanned Items:"
        '
        'lblPLItms
        '
        Me.lblPLItms.Location = New System.Drawing.Point(18, 78)
        Me.lblPLItms.Name = "lblPLItms"
        Me.lblPLItms.Size = New System.Drawing.Size(111, 15)
        Me.lblPLItms.Text = "Picking List Items:"
        '
        'lblSELQd
        '
        Me.lblSELQd.Location = New System.Drawing.Point(18, 108)
        Me.lblSELQd.Name = "lblSELQd"
        Me.lblSELQd.Size = New System.Drawing.Size(95, 15)
        Me.lblSELQd.Text = "SELs Generated:"
        '
        'lblScndItmsVal
        '
        Me.lblScndItmsVal.Location = New System.Drawing.Point(151, 48)
        Me.lblScndItmsVal.Name = "lblScndItmsVal"
        Me.lblScndItmsVal.Size = New System.Drawing.Size(37, 15)
        '
        'lblPLItmsVal
        '
        Me.lblPLItmsVal.Location = New System.Drawing.Point(151, 78)
        Me.lblPLItmsVal.Name = "lblPLItmsVal"
        Me.lblPLItmsVal.Size = New System.Drawing.Size(37, 15)
        '
        'lblSELQdVal
        '
        Me.lblSELQdVal.Location = New System.Drawing.Point(151, 108)
        Me.lblSELQdVal.Name = "lblSELQdVal"
        Me.lblSELQdVal.Size = New System.Drawing.Size(37, 15)
        '
        'lblUserMsg
        '
        Me.lblUserMsg.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblUserMsg.Location = New System.Drawing.Point(18, 155)
        Me.lblUserMsg.Name = "lblUserMsg"
        Me.lblUserMsg.Size = New System.Drawing.Size(192, 50)
        Me.lblUserMsg.Text = "Collect and display all new SELs OR start another session"
        '
        'btnOK
        '
        Me.btnOK.BackColor = System.Drawing.Color.Transparent
        Me.btnOK.Location = New System.Drawing.Point(89, 221)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(40, 40)
        Me.btnOK.TabIndex = 21
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 30
        '
        'lblActionDockTransmit
        '
        Me.lblActionDockTransmit.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblActionDockTransmit.Location = New System.Drawing.Point(18, 137)
        Me.lblActionDockTransmit.Name = "lblActionDockTransmit"
        Me.lblActionDockTransmit.Size = New System.Drawing.Size(192, 19)
        Me.lblActionDockTransmit.Text = "Action: Dock and Transmit."
        '
        'frmSMSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblActionDockTransmit)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.lblUserMsg)
        Me.Controls.Add(Me.lblSELQdVal)
        Me.Controls.Add(Me.lblPLItmsVal)
        Me.Controls.Add(Me.lblScndItmsVal)
        Me.Controls.Add(Me.lblSELQd)
        Me.Controls.Add(Me.lblPLItms)
        Me.Controls.Add(Me.lblScndItms)
        Me.Controls.Add(Me.lblSumryHeading)
        Me.Name = "frmSMSummary"
        Me.Text = "Shelf Monitor"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblSumryHeading As System.Windows.Forms.Label
    Friend WithEvents lblScndItms As System.Windows.Forms.Label
    Friend WithEvents lblPLItms As System.Windows.Forms.Label
    Friend WithEvents lblSELQd As System.Windows.Forms.Label
    Friend WithEvents lblScndItmsVal As System.Windows.Forms.Label
    Friend WithEvents lblPLItmsVal As System.Windows.Forms.Label
    Friend WithEvents lblSELQdVal As System.Windows.Forms.Label
    Friend WithEvents lblUserMsg As System.Windows.Forms.Label
    Friend WithEvents btnOK As CustomButtons.btn_Ok
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblActionDockTransmit As System.Windows.Forms.Label
End Class
