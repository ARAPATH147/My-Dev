<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmEXSummary
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
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.lblPickListText = New System.Windows.Forms.Label
        Me.lblUserMsg = New System.Windows.Forms.Label
        Me.Btn_Ok1 = New CustomButtons.btn_Ok
        Me.lblDockTransmit = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(16, 23)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(166, 20)
        Me.Label1.Text = "Excess Stock Summary"
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(16, 66)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(100, 20)
        Me.Label2.Text = "Items in List:"
        '
        'lblPickListText
        '
        Me.lblPickListText.Location = New System.Drawing.Point(148, 66)
        Me.lblPickListText.Name = "lblPickListText"
        Me.lblPickListText.Size = New System.Drawing.Size(67, 20)
        '
        'lblUserMsg
        '
        Me.lblUserMsg.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblUserMsg.Location = New System.Drawing.Point(14, 111)
        Me.lblUserMsg.Name = "lblUserMsg"
        Me.lblUserMsg.Size = New System.Drawing.Size(201, 42)
        Me.lblUserMsg.Text = "Now go to Picking List to count the Sales Floor."
        '
        'Btn_Ok1
        '
        Me.Btn_Ok1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Ok1.Location = New System.Drawing.Point(93, 220)
        Me.Btn_Ok1.Name = "Btn_Ok1"
        Me.Btn_Ok1.Size = New System.Drawing.Size(40, 40)
        Me.Btn_Ok1.TabIndex = 7
        '
        'lblDockTransmit
        '
        Me.lblDockTransmit.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblDockTransmit.Location = New System.Drawing.Point(14, 176)
        Me.lblDockTransmit.Name = "lblDockTransmit"
        Me.lblDockTransmit.Size = New System.Drawing.Size(140, 22)
        Me.lblDockTransmit.Text = "Dock and Transmit"
        Me.lblDockTransmit.Visible = False
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 14
        '
        'frmEXSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblDockTransmit)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.Btn_Ok1)
        Me.Controls.Add(Me.lblUserMsg)
        Me.Controls.Add(Me.lblPickListText)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Name = "frmEXSummary"
        Me.Text = "Excess Stock"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblPickListText As System.Windows.Forms.Label
    Friend WithEvents lblUserMsg As System.Windows.Forms.Label
    Friend WithEvents Btn_Ok1 As CustomButtons.btn_Ok
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblDockTransmit As System.Windows.Forms.Label
End Class
