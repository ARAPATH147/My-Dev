<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmRLDespatch
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
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.btnDespatch = New CustomButtons.btn_Despatch
        Me.lblSingleData = New System.Windows.Forms.Label
        Me.lblUODData = New System.Windows.Forms.Label
        Me.lblCompleteInstruction = New System.Windows.Forms.Label
        Me.lblTotSingles = New System.Windows.Forms.Label
        Me.lblUOD = New System.Windows.Forms.Label
        Me.lblRecallDesc = New System.Windows.Forms.Label
        Me.lblTitle = New System.Windows.Forms.Label
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.SuspendLayout()
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(180, 235)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 41
        '
        'btnDespatch
        '
        Me.btnDespatch.BackColor = System.Drawing.Color.Transparent
        Me.btnDespatch.Location = New System.Drawing.Point(6, 235)
        Me.btnDespatch.Name = "btnDespatch"
        Me.btnDespatch.Size = New System.Drawing.Size(65, 24)
        Me.btnDespatch.TabIndex = 40
        '
        'lblSingleData
        '
        Me.lblSingleData.Location = New System.Drawing.Point(124, 102)
        Me.lblSingleData.Name = "lblSingleData"
        Me.lblSingleData.Size = New System.Drawing.Size(68, 24)
        '
        'lblUODData
        '
        Me.lblUODData.Location = New System.Drawing.Point(46, 74)
        Me.lblUODData.Name = "lblUODData"
        Me.lblUODData.Size = New System.Drawing.Size(181, 16)
        '
        'lblCompleteInstruction
        '
        Me.lblCompleteInstruction.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblCompleteInstruction.Location = New System.Drawing.Point(6, 149)
        Me.lblCompleteInstruction.Name = "lblCompleteInstruction"
        Me.lblCompleteInstruction.Size = New System.Drawing.Size(224, 56)
        Me.lblCompleteInstruction.Text = "Despatch UOD now by clicking the Despatch button below."
        '
        'lblTotSingles
        '
        Me.lblTotSingles.Location = New System.Drawing.Point(6, 102)
        Me.lblTotSingles.Name = "lblTotSingles"
        Me.lblTotSingles.Size = New System.Drawing.Size(125, 24)
        Me.lblTotSingles.Text = "Number of Singles :"
        '
        'lblUOD
        '
        Me.lblUOD.Location = New System.Drawing.Point(6, 73)
        Me.lblUOD.Name = "lblUOD"
        Me.lblUOD.Size = New System.Drawing.Size(45, 16)
        Me.lblUOD.Text = "UOD : "
        '
        'lblRecallDesc
        '
        Me.lblRecallDesc.ForeColor = System.Drawing.Color.Black
        Me.lblRecallDesc.Location = New System.Drawing.Point(6, 33)
        Me.lblRecallDesc.Name = "lblRecallDesc"
        Me.lblRecallDesc.Size = New System.Drawing.Size(224, 24)
        Me.lblRecallDesc.Text = "TEST RECALL12345678"
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
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 49
        '
        'frmRLDespatch
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.btnDespatch)
        Me.Controls.Add(Me.lblSingleData)
        Me.Controls.Add(Me.lblUODData)
        Me.Controls.Add(Me.lblCompleteInstruction)
        Me.Controls.Add(Me.lblTotSingles)
        Me.Controls.Add(Me.lblUOD)
        Me.Controls.Add(Me.lblRecallDesc)
        Me.Controls.Add(Me.lblTitle)
        Me.Name = "frmRLDespatch"
        Me.Text = "Recalls"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents btnDespatch As CustomButtons.btn_Despatch
    Friend WithEvents lblSingleData As System.Windows.Forms.Label
    Friend WithEvents lblUODData As System.Windows.Forms.Label
    Friend WithEvents lblCompleteInstruction As System.Windows.Forms.Label
    Friend WithEvents lblTotSingles As System.Windows.Forms.Label
    Friend WithEvents lblUOD As System.Windows.Forms.Label
    Friend WithEvents lblRecallDesc As System.Windows.Forms.Label
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
End Class
