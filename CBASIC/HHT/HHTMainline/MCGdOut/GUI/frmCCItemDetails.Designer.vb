<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmCCItemDetails
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
        Me.lblItemDesc3 = New System.Windows.Forms.Label
        Me.lblItemDesc2 = New System.Windows.Forms.Label
        Me.Btn_CalcPad_small = New CustomButtons.btn_CalcPad_small
        Me.lblTitle = New System.Windows.Forms.Label
        Me.lblItemDesc1 = New System.Windows.Forms.Label
        Me.lblBusCentreDesc = New System.Windows.Forms.Label
        Me.lblEAN = New System.Windows.Forms.Label
        Me.lblBootsCode = New System.Windows.Forms.Label
        Me.lblQuantityData = New System.Windows.Forms.Label
        Me.lblQuantity = New System.Windows.Forms.Label
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.btnNext = New CustomButtons.btn_Next_small
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblItemDesc3
        '
        Me.lblItemDesc3.Location = New System.Drawing.Point(6, 141)
        Me.lblItemDesc3.Name = "lblItemDesc3"
        Me.lblItemDesc3.Size = New System.Drawing.Size(175, 17)
        Me.lblItemDesc3.Text = "Instr3"
        '
        'lblItemDesc2
        '
        Me.lblItemDesc2.Location = New System.Drawing.Point(6, 124)
        Me.lblItemDesc2.Name = "lblItemDesc2"
        Me.lblItemDesc2.Size = New System.Drawing.Size(175, 17)
        Me.lblItemDesc2.Text = "Instr2"
        '
        'Btn_CalcPad_small
        '
        Me.Btn_CalcPad_small.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CalcPad_small.Location = New System.Drawing.Point(187, 165)
        Me.Btn_CalcPad_small.Name = "Btn_CalcPad_small"
        Me.Btn_CalcPad_small.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small.TabIndex = 70
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.ForeColor = System.Drawing.Color.Black
        Me.lblTitle.Location = New System.Drawing.Point(6, 9)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(231, 25)
        Me.lblTitle.Text = "Recall : Recalls and Recovery"
        '
        'lblItemDesc1
        '
        Me.lblItemDesc1.Location = New System.Drawing.Point(6, 107)
        Me.lblItemDesc1.Name = "lblItemDesc1"
        Me.lblItemDesc1.Size = New System.Drawing.Size(175, 17)
        Me.lblItemDesc1.Text = "Instr1"
        '
        'lblBusCentreDesc
        '
        Me.lblBusCentreDesc.ForeColor = System.Drawing.Color.Black
        Me.lblBusCentreDesc.Location = New System.Drawing.Point(6, 33)
        Me.lblBusCentreDesc.Name = "lblBusCentreDesc"
        Me.lblBusCentreDesc.Size = New System.Drawing.Size(224, 22)
        Me.lblBusCentreDesc.Text = "Beauty n Care"
        '
        'lblEAN
        '
        Me.lblEAN.Location = New System.Drawing.Point(6, 83)
        Me.lblEAN.Name = "lblEAN"
        Me.lblEAN.Size = New System.Drawing.Size(120, 20)
        Me.lblEAN.Text = "0-000004-020202"
        '
        'lblBootsCode
        '
        Me.lblBootsCode.Location = New System.Drawing.Point(6, 59)
        Me.lblBootsCode.Name = "lblBootsCode"
        Me.lblBootsCode.Size = New System.Drawing.Size(120, 20)
        Me.lblBootsCode.Text = "40-20-200"
        '
        'lblQuantityData
        '
        Me.lblQuantityData.Location = New System.Drawing.Point(131, 172)
        Me.lblQuantityData.Name = "lblQuantityData"
        Me.lblQuantityData.Size = New System.Drawing.Size(50, 16)
        Me.lblQuantityData.Text = "1"
        '
        'lblQuantity
        '
        Me.lblQuantity.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblQuantity.Location = New System.Drawing.Point(6, 172)
        Me.lblQuantity.Name = "lblQuantity"
        Me.lblQuantity.Size = New System.Drawing.Size(107, 16)
        Me.lblQuantity.Text = "Enter Quantity:"
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(176, 236)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 69
        '
        'btnNext
        '
        Me.btnNext.BackColor = System.Drawing.Color.Transparent
        Me.btnNext.Location = New System.Drawing.Point(6, 236)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(50, 24)
        Me.btnNext.TabIndex = 68
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 78
        '
        'frmCCItemDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblBusCentreDesc)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblItemDesc3)
        Me.Controls.Add(Me.lblItemDesc2)
        Me.Controls.Add(Me.Btn_CalcPad_small)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.lblItemDesc1)
        Me.Controls.Add(Me.lblEAN)
        Me.Controls.Add(Me.lblBootsCode)
        Me.Controls.Add(Me.lblQuantityData)
        Me.Controls.Add(Me.lblQuantity)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.btnNext)
        Me.KeyPreview = True
        Me.Name = "frmCCItemDetails"
        Me.Text = "Credit Claim"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblItemDesc3 As System.Windows.Forms.Label
    Friend WithEvents lblItemDesc2 As System.Windows.Forms.Label
    Friend WithEvents Btn_CalcPad_small As CustomButtons.btn_CalcPad_small
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblItemDesc1 As System.Windows.Forms.Label
    Friend WithEvents lblBusCentreDesc As System.Windows.Forms.Label
    Friend WithEvents lblEAN As System.Windows.Forms.Label
    Friend WithEvents lblBootsCode As System.Windows.Forms.Label
    Friend WithEvents lblQuantityData As System.Windows.Forms.Label
    Friend WithEvents lblQuantity As System.Windows.Forms.Label
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents btnNext As CustomButtons.btn_Next_small
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
End Class
