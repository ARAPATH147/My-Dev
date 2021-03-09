<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmPCSummary
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
        Me.lblTitleSum = New System.Windows.Forms.Label
        Me.lblSELsQueued = New System.Windows.Forms.Label
        Me.lblDockAndTransmit = New System.Windows.Forms.Label
        Me.lblNumSEL = New System.Windows.Forms.Label
        Me.Btn_Ok1 = New CustomButtons.btn_Ok
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.lblSOD = New System.Windows.Forms.Label
        Me.lblPriceCheckMessage = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblTitleSum
        '
        Me.lblTitleSum.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitleSum.Location = New System.Drawing.Point(14, 32)
        Me.lblTitleSum.Name = "lblTitleSum"
        Me.lblTitleSum.Size = New System.Drawing.Size(177, 20)
        Me.lblTitleSum.Text = "Price Check Summary"
        '
        'lblSELsQueued
        '
        Me.lblSELsQueued.Location = New System.Drawing.Point(14, 105)
        Me.lblSELsQueued.Name = "lblSELsQueued"
        Me.lblSELsQueued.Size = New System.Drawing.Size(106, 20)
        Me.lblSELsQueued.Text = "SELs Generated:"
        '
        'lblDockAndTransmit
        '
        Me.lblDockAndTransmit.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblDockAndTransmit.Location = New System.Drawing.Point(14, 161)
        Me.lblDockAndTransmit.Name = "lblDockAndTransmit"
        Me.lblDockAndTransmit.Size = New System.Drawing.Size(212, 51)
        Me.lblDockAndTransmit.Text = "Action: Dock and Transmit.     Collect and display all new SELS"
        '
        'lblNumSEL
        '
        Me.lblNumSEL.Location = New System.Drawing.Point(137, 106)
        Me.lblNumSEL.Name = "lblNumSEL"
        Me.lblNumSEL.Size = New System.Drawing.Size(79, 20)
        Me.lblNumSEL.Text = "numSEL"
        '
        'Btn_Ok1
        '
        Me.Btn_Ok1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Ok1.Location = New System.Drawing.Point(98, 220)
        Me.Btn_Ok1.Name = "Btn_Ok1"
        Me.Btn_Ok1.Size = New System.Drawing.Size(40, 40)
        Me.Btn_Ok1.TabIndex = 5
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 10
        '
        'lblSOD
        '
        Me.lblSOD.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblSOD.Location = New System.Drawing.Point(14, 56)
        Me.lblSOD.Name = "lblSOD"
        Me.lblSOD.Size = New System.Drawing.Size(202, 31)
        Me.lblSOD.Text = "Start of Day tickets were not put out correctly"
        Me.lblSOD.Visible = False
        '
        'lblPriceCheckMessage
        '
        Me.lblPriceCheckMessage.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblPriceCheckMessage.Location = New System.Drawing.Point(14, 161)
        Me.lblPriceCheckMessage.Name = "lblPriceCheckMessage"
        Me.lblPriceCheckMessage.Size = New System.Drawing.Size(212, 39)
        Me.lblPriceCheckMessage.Text = "Collect and display all new SELS"
        'frmPCSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblPriceCheckMessage)
        Me.Controls.Add(Me.lblSOD)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.Btn_Ok1)
        Me.Controls.Add(Me.lblNumSEL)
        Me.Controls.Add(Me.lblDockAndTransmit)
        Me.Controls.Add(Me.lblSELsQueued)
        Me.Controls.Add(Me.lblTitleSum)
        Me.Name = "frmPCSummary"
        Me.Text = "Price Check"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblTitleSum As System.Windows.Forms.Label
    Friend WithEvents lblSELsQueued As System.Windows.Forms.Label
    Friend WithEvents lblDockAndTransmit As System.Windows.Forms.Label
    Friend WithEvents lblNumSEL As System.Windows.Forms.Label
    Friend WithEvents Btn_Ok1 As CustomButtons.btn_Ok
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblSOD As System.Windows.Forms.Label
    Friend WithEvents lblPriceCheckMessage As System.Windows.Forms.Label
End Class
