<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSSSummary
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
        Me.lblStoreSalesSummaryDetails = New System.Windows.Forms.Label
        Me.lblToday = New System.Windows.Forms.Label
        Me.lblThisWeek = New System.Windows.Forms.Label
        Me.lblThisWeekValue = New System.Windows.Forms.Label
        Me.lblTodayValue = New System.Windows.Forms.Label
        Me.btn_Ok = New CustomButtons.btn_Ok
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblStoreSalesSummaryDetails
        '
        Me.lblStoreSalesSummaryDetails.Location = New System.Drawing.Point(66, 30)
        Me.lblStoreSalesSummaryDetails.Name = "lblStoreSalesSummaryDetails"
        Me.lblStoreSalesSummaryDetails.Size = New System.Drawing.Size(127, 43)
        Me.lblStoreSalesSummaryDetails.Text = "Store sales value    " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "      summary"
        '
        'lblToday
        '
        Me.lblToday.Location = New System.Drawing.Point(21, 104)
        Me.lblToday.Name = "lblToday"
        Me.lblToday.Size = New System.Drawing.Size(100, 20)
        Me.lblToday.Text = "Today"
        '
        'lblThisWeek
        '
        Me.lblThisWeek.Location = New System.Drawing.Point(21, 149)
        Me.lblThisWeek.Name = "lblThisWeek"
        Me.lblThisWeek.Size = New System.Drawing.Size(100, 20)
        Me.lblThisWeek.Text = "This Week"
        '
        'lblThisWeekValue
        '
        Me.lblThisWeekValue.Location = New System.Drawing.Point(127, 149)
        Me.lblThisWeekValue.Name = "lblThisWeekValue"
        Me.lblThisWeekValue.Size = New System.Drawing.Size(100, 20)
        '
        'lblTodayValue
        '
        Me.lblTodayValue.Location = New System.Drawing.Point(127, 104)
        Me.lblTodayValue.Name = "lblTodayValue"
        Me.lblTodayValue.Size = New System.Drawing.Size(100, 20)
        '
        'btn_Ok
        '
        Me.btn_Ok.BackColor = System.Drawing.Color.Transparent
        Me.btn_Ok.Location = New System.Drawing.Point(89, 221)
        Me.btn_Ok.Name = "btn_Ok"
        Me.btn_Ok.Size = New System.Drawing.Size(40, 40)
        Me.btn_Ok.TabIndex = 10
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 13
        '
        'frmSSSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btn_Ok)
        Me.Controls.Add(Me.lblTodayValue)
        Me.Controls.Add(Me.lblThisWeekValue)
        Me.Controls.Add(Me.lblThisWeek)
        Me.Controls.Add(Me.lblToday)
        Me.Controls.Add(Me.lblStoreSalesSummaryDetails)
        Me.Name = "frmSSSummary"
        Me.Text = "Store Sales"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblStoreSalesSummaryDetails As System.Windows.Forms.Label
    Friend WithEvents lblToday As System.Windows.Forms.Label
    Public WithEvents lblThisWeek As System.Windows.Forms.Label
    Friend WithEvents lblThisWeekValue As System.Windows.Forms.Label
    Friend WithEvents lblTodayValue As System.Windows.Forms.Label
    Friend WithEvents btn_Ok As CustomButtons.btn_Ok
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
End Class
