<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSPLPHome
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
        Me.lblLivePlnr = New System.Windows.Forms.Label
        Me.lblSelectOption = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.Btn_CorePlanners1 = New CustomButtons.btn_CorePlanners
        Me.btnSalesPlan = New CustomButtons.Sales_Plan1
        Me.Btn_Quit_small1 = New CustomButtons.btn_Quit_small
        Me.SuspendLayout()
        '
        'lblLivePlnr
        '
        Me.lblLivePlnr.Location = New System.Drawing.Point(15, 18)
        Me.lblLivePlnr.Name = "lblLivePlnr"
        Me.lblLivePlnr.Size = New System.Drawing.Size(100, 20)
        Me.lblLivePlnr.Text = "Live Planner"
        '
        'lblSelectOption
        '
        Me.lblSelectOption.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelectOption.Location = New System.Drawing.Point(15, 207)
        Me.lblSelectOption.Name = "lblSelectOption"
        Me.lblSelectOption.Size = New System.Drawing.Size(100, 20)
        Me.lblSelectOption.Text = "Select Option"
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 9
        '
        'Btn_CorePlanners1
        '
        Me.Btn_CorePlanners1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CorePlanners1.Location = New System.Drawing.Point(62, 92)
        Me.Btn_CorePlanners1.Name = "Btn_CorePlanners1"
        Me.Btn_CorePlanners1.Size = New System.Drawing.Size(90, 24)
        Me.Btn_CorePlanners1.TabIndex = 15
        '
        'btnSalesPlan
        '
        Me.btnSalesPlan.BackColor = System.Drawing.Color.Transparent
        Me.btnSalesPlan.Location = New System.Drawing.Point(62, 137)
        Me.btnSalesPlan.Name = "btnSalesPlan"
        Me.btnSalesPlan.Size = New System.Drawing.Size(90, 24)
        Me.btnSalesPlan.TabIndex = 12
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(173, 235)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.TabIndex = 3
        '
        'frmSPLPHome
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_CorePlanners1)
        Me.Controls.Add(Me.btnSalesPlan)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblSelectOption)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.lblLivePlnr)
        Me.Name = "frmSPLPHome"
        Me.Text = "Planners"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblLivePlnr As System.Windows.Forms.Label
    Friend WithEvents Btn_Quit_small1 As CustomButtons.btn_Quit_small
    Friend WithEvents lblSelectOption As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents btnSalesPlan As CustomButtons.Sales_Plan1
    Friend WithEvents Btn_CorePlanners1 As CustomButtons.btn_CorePlanners
End Class
