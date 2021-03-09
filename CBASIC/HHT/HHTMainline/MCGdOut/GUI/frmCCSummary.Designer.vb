<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmCCSummary
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
        Me.btnOk = New CustomButtons.btn_Ok
        Me.lblTotSinglesData = New System.Windows.Forms.Label
        Me.lblTotValueData = New System.Windows.Forms.Label
        Me.lblTitle = New System.Windows.Forms.Label
        Me.lblTotalValue = New System.Windows.Forms.Label
        Me.lblTotSingles = New System.Windows.Forms.Label
        Me.lblCompleteInstruction = New System.Windows.Forms.Label
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.SuspendLayout()
        '
        'btnOk
        '
        Me.btnOk.BackColor = System.Drawing.Color.Transparent
        Me.btnOk.Location = New System.Drawing.Point(100, 220)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(40, 40)
        Me.btnOk.TabIndex = 24
        '
        'lblTotSinglesData
        '
        Me.lblTotSinglesData.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Regular)
        Me.lblTotSinglesData.Location = New System.Drawing.Point(132, 102)
        Me.lblTotSinglesData.Name = "lblTotSinglesData"
        Me.lblTotSinglesData.Size = New System.Drawing.Size(71, 16)
        '
        'lblTotValueData
        '
        Me.lblTotValueData.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Regular)
        Me.lblTotValueData.Location = New System.Drawing.Point(87, 74)
        Me.lblTotValueData.Name = "lblTotValueData"
        Me.lblTotValueData.Size = New System.Drawing.Size(143, 16)
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.ForeColor = System.Drawing.Color.Black
        Me.lblTitle.Location = New System.Drawing.Point(6, 9)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(231, 25)
        Me.lblTitle.Text = "Returns : Faulty"
        '
        'lblTotalValue
        '
        Me.lblTotalValue.Location = New System.Drawing.Point(8, 74)
        Me.lblTotalValue.Name = "lblTotalValue"
        Me.lblTotalValue.Size = New System.Drawing.Size(82, 16)
        Me.lblTotalValue.Text = "Total Value :"
        '
        'lblTotSingles
        '
        Me.lblTotSingles.Location = New System.Drawing.Point(8, 102)
        Me.lblTotSingles.Name = "lblTotSingles"
        Me.lblTotSingles.Size = New System.Drawing.Size(138, 16)
        Me.lblTotSingles.Text = "Number Of Singles : "
        '
        'lblCompleteInstruction
        '
        Me.lblCompleteInstruction.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblCompleteInstruction.Location = New System.Drawing.Point(8, 164)
        Me.lblCompleteInstruction.Name = "lblCompleteInstruction"
        Me.lblCompleteInstruction.Size = New System.Drawing.Size(224, 36)
        Me.lblCompleteInstruction.Text = "Action: Dock && Transmit." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Claim Completed"
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 31
        '
        'frmCCSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.lblTotSinglesData)
        Me.Controls.Add(Me.lblTotValueData)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.lblTotalValue)
        Me.Controls.Add(Me.lblTotSingles)
        Me.Controls.Add(Me.lblCompleteInstruction)
        Me.Name = "frmCCSummary"
        Me.Text = "Credit Claim"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnOk As CustomButtons.btn_Ok
    Friend WithEvents lblTotSinglesData As System.Windows.Forms.Label
    Friend WithEvents lblTotValueData As System.Windows.Forms.Label
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblTotalValue As System.Windows.Forms.Label
    Friend WithEvents lblTotSingles As System.Windows.Forms.Label
    Friend WithEvents lblCompleteInstruction As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
End Class
