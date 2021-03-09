<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmPLOSSRFinish
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
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.custCtrlBtnNo = New CustomButtons.btn_No_sm_red
        Me.custCtrlBtnYes = New CustomButtons.btn_Yes_sm
        Me.lblConfirm = New System.Windows.Forms.Label
        Me.lblOSSRItems = New System.Windows.Forms.Label
        Me.lblPLStatDisplay = New System.Windows.Forms.Label
        Me.lblGAPItems = New System.Windows.Forms.Label
        Me.lblGAPReport = New System.Windows.Forms.Label
        Me.lblGAPAcetated = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 31
        '
        'custCtrlBtnNo
        '
        Me.custCtrlBtnNo.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnNo.Location = New System.Drawing.Point(177, 241)
        Me.custCtrlBtnNo.Name = "custCtrlBtnNo"
        Me.custCtrlBtnNo.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnNo.TabIndex = 30
        '
        'custCtrlBtnYes
        '
        Me.custCtrlBtnYes.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnYes.Location = New System.Drawing.Point(12, 241)
        Me.custCtrlBtnYes.Name = "custCtrlBtnYes"
        Me.custCtrlBtnYes.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnYes.TabIndex = 29
        '
        'lblConfirm
        '
        Me.lblConfirm.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblConfirm.Location = New System.Drawing.Point(12, 199)
        Me.lblConfirm.Name = "lblConfirm"
        Me.lblConfirm.Size = New System.Drawing.Size(166, 32)
        Me.lblConfirm.Text = "Do you want to send this list to OSSR?"
        '
        'lblOSSRItems
        '
        Me.lblOSSRItems.Location = New System.Drawing.Point(39, 68)
        Me.lblOSSRItems.Name = "lblOSSRItems"
        Me.lblOSSRItems.Size = New System.Drawing.Size(162, 20)
        Me.lblOSSRItems.Text = "<n>OSSR Item(s)  in List"
        '
        'lblPLStatDisplay
        '
        Me.lblPLStatDisplay.Location = New System.Drawing.Point(22, 15)
        Me.lblPLStatDisplay.Name = "lblPLStatDisplay"
        Me.lblPLStatDisplay.Size = New System.Drawing.Size(203, 36)
        Me.lblPLStatDisplay.Text = "The Backshop Picking List is" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & " now complete"
        '
        'lblGAPItems
        '
        Me.lblGAPItems.Location = New System.Drawing.Point(27, 88)
        Me.lblGAPItems.Name = "lblGAPItems"
        Me.lblGAPItems.Size = New System.Drawing.Size(170, 20)
        Me.lblGAPItems.Text = "<n>GAP Items sent to report"
        '
        'lblGAPReport
        '
        Me.lblGAPReport.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblGAPReport.Location = New System.Drawing.Point(33, 127)
        Me.lblGAPReport.Name = "lblGAPReport"
        Me.lblGAPReport.Size = New System.Drawing.Size(161, 20)
        Me.lblGAPReport.Text = "** Check GAP Report**"
        Me.lblGAPReport.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblGAPAcetated
        '
        Me.lblGAPAcetated.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblGAPAcetated.Location = New System.Drawing.Point(39, 156)
        Me.lblGAPAcetated.Name = "lblGAPAcetated"
        Me.lblGAPAcetated.Size = New System.Drawing.Size(151, 20)
        Me.lblGAPAcetated.Text = "Put out GAP Acetates"
        Me.lblGAPAcetated.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'frmPLOSSRFinish
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblGAPAcetated)
        Me.Controls.Add(Me.lblGAPReport)
        Me.Controls.Add(Me.lblGAPItems)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.custCtrlBtnNo)
        Me.Controls.Add(Me.custCtrlBtnYes)
        Me.Controls.Add(Me.lblConfirm)
        Me.Controls.Add(Me.lblOSSRItems)
        Me.Controls.Add(Me.lblPLStatDisplay)
        Me.Name = "frmPLOSSRFinish"
        Me.Text = "Picking List"
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents custCtrlBtnNo As CustomButtons.btn_No_sm_red
    Friend WithEvents custCtrlBtnYes As CustomButtons.btn_Yes_sm
    Friend WithEvents lblConfirm As System.Windows.Forms.Label
    Friend WithEvents lblOSSRItems As System.Windows.Forms.Label
    Friend WithEvents lblPLStatDisplay As System.Windows.Forms.Label
    Friend WithEvents lblGAPItems As System.Windows.Forms.Label
    Friend WithEvents lblGAPReport As System.Windows.Forms.Label
    Friend WithEvents lblGAPAcetated As System.Windows.Forms.Label
End Class
