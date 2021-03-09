<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmPLPSPpending
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
        Me.custCtrlBtnNo = New CustomButtons.btn_No_sm_red
        Me.custCtrlBtnYes = New CustomButtons.btn_Yes_sm
        Me.btnView = New CustomButtons.btnView
        Me.Label1 = New System.Windows.Forms.Label
        Me.Btn_Help = New CustomButtons.btn_Info
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'custCtrlBtnNo
        '
        Me.custCtrlBtnNo.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnNo.Location = New System.Drawing.Point(173, 238)
        Me.custCtrlBtnNo.Name = "custCtrlBtnNo"
        Me.custCtrlBtnNo.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnNo.TabIndex = 22
        '
        'custCtrlBtnYes
        '
        Me.custCtrlBtnYes.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnYes.Location = New System.Drawing.Point(12, 238)
        Me.custCtrlBtnYes.Name = "custCtrlBtnYes"
        Me.custCtrlBtnYes.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnYes.TabIndex = 21
        '
        'btnView
        '
        Me.btnView.BackColor = System.Drawing.Color.Transparent
        Me.btnView.Location = New System.Drawing.Point(92, 238)
        Me.btnView.Name = "btnView"
        Me.btnView.Size = New System.Drawing.Size(58, 24)
        Me.btnView.TabIndex = 23
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(22, 91)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(215, 63)
        Me.Label1.Text = "Not all Pending Sales Plan items have been counted. To check items not completed," & _
            " select View. Are you sure you want to quit the picking list?"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Btn_Help
        '
        Me.Btn_Help.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Help.Location = New System.Drawing.Point(191, 14)
        Me.Btn_Help.Name = "Btn_Help"
        Me.Btn_Help.Size = New System.Drawing.Size(32, 32)
        Me.Btn_Help.TabIndex = 160
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 9
        '
        'frmPLPSPpending
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Help)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.custCtrlBtnNo)
        Me.Controls.Add(Me.custCtrlBtnYes)
        Me.Controls.Add(Me.objStatusBar)
        Me.Name = "frmPLPSPpending"
        Me.Text = "Picking List - BS"
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents custCtrlBtnNo As CustomButtons.btn_No_sm_red
    Friend WithEvents custCtrlBtnYes As CustomButtons.btn_Yes_sm
    Friend WithEvents btnView As CustomButtons.btnView
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Btn_Help As CustomButtons.btn_Info
End Class
