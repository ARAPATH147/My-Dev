<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmConnector
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
    Private mainMenu1 As System.Windows.Forms.MainMenu

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmConnector))
        Me.mainMenu1 = New System.Windows.Forms.MainMenu
        Me.Label1 = New System.Windows.Forms.Label
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.btnTimeoutCancel = New System.Windows.Forms.Button
        Me.btnTimeoutRetry = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnOK = New System.Windows.Forms.Button
        Me.lblMessage = New System.Windows.Forms.Label
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.btnCancelAlternate = New System.Windows.Forms.Button
        Me.btnConnectAlternate = New System.Windows.Forms.Button
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.Color.Red
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label1.Name = "Label1"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.HighlightText
        Me.Panel1.Controls.Add(Me.btnConnectAlternate)
        Me.Panel1.Controls.Add(Me.btnCancelAlternate)
        Me.Panel1.Controls.Add(Me.btnTimeoutCancel)
        Me.Panel1.Controls.Add(Me.btnTimeoutRetry)
        Me.Panel1.Controls.Add(Me.btnCancel)
        Me.Panel1.Controls.Add(Me.btnOK)
        Me.Panel1.Controls.Add(Me.lblMessage)
        Me.Panel1.Controls.Add(Me.PictureBox1)
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Name = "Panel1"
        '
        'btnTimeoutCancel
        '
        Me.btnTimeoutCancel.BackColor = System.Drawing.Color.MidnightBlue
        Me.btnTimeoutCancel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        resources.ApplyResources(Me.btnTimeoutCancel, "btnTimeoutCancel")
        Me.btnTimeoutCancel.Name = "btnTimeoutCancel"
        '
        'btnTimeoutRetry
        '
        Me.btnTimeoutRetry.BackColor = System.Drawing.Color.MidnightBlue
        Me.btnTimeoutRetry.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        resources.ApplyResources(Me.btnTimeoutRetry, "btnTimeoutRetry")
        Me.btnTimeoutRetry.Name = "btnTimeoutRetry"
        '
        'btnCancel
        '
        Me.btnCancel.BackColor = System.Drawing.Color.MidnightBlue
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.btnCancel.Name = "btnCancel"
        '
        'btnOK
        '
        Me.btnOK.BackColor = System.Drawing.Color.MidnightBlue
        resources.ApplyResources(Me.btnOK, "btnOK")
        Me.btnOK.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.btnOK.Name = "btnOK"
        '
        'lblMessage
        '
        resources.ApplyResources(Me.lblMessage, "lblMessage")
        Me.lblMessage.Name = "lblMessage"
        '
        'PictureBox1
        '
        resources.ApplyResources(Me.PictureBox1, "PictureBox1")
        Me.PictureBox1.Name = "PictureBox1"
        '
        'btnCancelAlternate
        '
        Me.btnCancelAlternate.BackColor = System.Drawing.Color.MidnightBlue
        Me.btnCancelAlternate.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        resources.ApplyResources(Me.btnCancelAlternate, "btnCancelAlternate")
        Me.btnCancelAlternate.Name = "btnCancelAlternate"
        '
        'btnConnectAlternate
        '
        Me.btnConnectAlternate.BackColor = System.Drawing.Color.MidnightBlue
        Me.btnConnectAlternate.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        resources.ApplyResources(Me.btnConnectAlternate, "btnConnectAlternate")
        Me.btnConnectAlternate.Name = "btnConnectAlternate"
        '
        'frmConnector
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        resources.ApplyResources(Me, "$this")
        Me.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange
        Me.BackColor = System.Drawing.Color.DimGray
        Me.ControlBox = False
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.MinimizeBox = False
        Me.Name = "frmConnector"
        Me.TopMost = True
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents lblMessage As System.Windows.Forms.Label
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents btnTimeoutCancel As System.Windows.Forms.Button
    Friend WithEvents btnTimeoutRetry As System.Windows.Forms.Button
    Friend WithEvents btnCancelAlternate As System.Windows.Forms.Button
    Friend WithEvents btnConnectAlternate As System.Windows.Forms.Button
End Class
