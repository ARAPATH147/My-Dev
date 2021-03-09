<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmISItemDetails
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
        Me.lblStatusText = New System.Windows.Forms.Label
        Me.lblStatus = New System.Windows.Forms.Label
        Me.btn_Info_button_i = New CustomButtons.info_button_i
        Me.lblProdDescription3 = New System.Windows.Forms.Label
        Me.lblProdDescription2 = New System.Windows.Forms.Label
        Me.lblProdDescription1 = New System.Windows.Forms.Label
        Me.lblProductCode = New System.Windows.Forms.Label
        Me.lblBootsCode = New System.Windows.Forms.Label
        Me.btn_CalcPad_small = New CustomButtons.btn_CalcPad_small
        Me.lblUnits = New System.Windows.Forms.Label
        Me.lblValue = New System.Windows.Forms.Label
        Me.lblToday = New System.Windows.Forms.Label
        Me.lblTodayUnits = New System.Windows.Forms.Label
        Me.lblTodayValue = New System.Windows.Forms.Label
        Me.lblThisWeek = New System.Windows.Forms.Label
        Me.lblThisWeekUnits = New System.Windows.Forms.Label
        Me.lblThisWeekValue = New System.Windows.Forms.Label
        Me.lblStockFigure = New System.Windows.Forms.Label
        Me.lblStockFigureText = New System.Windows.Forms.Label
        Me.lblScanItem = New System.Windows.Forms.Label
        Me.btn_Quit_small = New CustomButtons.btn_Quit_small
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblStatusText
        '
        Me.lblStatusText.Location = New System.Drawing.Point(77, 110)
        Me.lblStatusText.Name = "lblStatusText"
        Me.lblStatusText.Size = New System.Drawing.Size(145, 20)
        Me.lblStatusText.Text = " "
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(12, 110)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(50, 20)
        Me.lblStatus.Text = "Status:"
        '
        'btn_Info_button_i
        '
        Me.btn_Info_button_i.BackColor = System.Drawing.Color.Transparent
        Me.btn_Info_button_i.Location = New System.Drawing.Point(186, 12)
        Me.btn_Info_button_i.Name = "btn_Info_button_i"
        Me.btn_Info_button_i.Size = New System.Drawing.Size(32, 32)
        Me.btn_Info_button_i.TabIndex = 20
        '
        'lblProdDescription3
        '
        Me.lblProdDescription3.Location = New System.Drawing.Point(12, 88)
        Me.lblProdDescription3.Name = "lblProdDescription3"
        Me.lblProdDescription3.Size = New System.Drawing.Size(140, 18)
        Me.lblProdDescription3.Text = "description3"
        '
        'lblProdDescription2
        '
        Me.lblProdDescription2.Location = New System.Drawing.Point(12, 70)
        Me.lblProdDescription2.Name = "lblProdDescription2"
        Me.lblProdDescription2.Size = New System.Drawing.Size(140, 18)
        Me.lblProdDescription2.Text = "description2"
        '
        'lblProdDescription1
        '
        Me.lblProdDescription1.Location = New System.Drawing.Point(12, 52)
        Me.lblProdDescription1.Name = "lblProdDescription1"
        Me.lblProdDescription1.Size = New System.Drawing.Size(140, 18)
        Me.lblProdDescription1.Text = "description1"
        '
        'lblProductCode
        '
        Me.lblProductCode.Location = New System.Drawing.Point(12, 26)
        Me.lblProductCode.Name = "lblProductCode"
        Me.lblProductCode.Size = New System.Drawing.Size(107, 20)
        Me.lblProductCode.Text = "1234567891234"
        '
        'lblBootsCode
        '
        Me.lblBootsCode.Location = New System.Drawing.Point(12, 7)
        Me.lblBootsCode.Name = "lblBootsCode"
        Me.lblBootsCode.Size = New System.Drawing.Size(100, 20)
        Me.lblBootsCode.Text = "Bootscode"
        '
        'btn_CalcPad_small
        '
        Me.btn_CalcPad_small.BackColor = System.Drawing.Color.Transparent
        Me.btn_CalcPad_small.Location = New System.Drawing.Point(143, 13)
        Me.btn_CalcPad_small.Name = "btn_CalcPad_small"
        Me.btn_CalcPad_small.Size = New System.Drawing.Size(24, 28)
        Me.btn_CalcPad_small.TabIndex = 28
        '
        'lblUnits
        '
        Me.lblUnits.Location = New System.Drawing.Point(99, 142)
        Me.lblUnits.Name = "lblUnits"
        Me.lblUnits.Size = New System.Drawing.Size(35, 20)
        Me.lblUnits.Text = "Units"
        '
        'lblValue
        '
        Me.lblValue.Location = New System.Drawing.Point(144, 142)
        Me.lblValue.Name = "lblValue"
        Me.lblValue.Size = New System.Drawing.Size(45, 20)
        Me.lblValue.Text = "Value"
        '
        'lblToday
        '
        Me.lblToday.Location = New System.Drawing.Point(43, 166)
        Me.lblToday.Name = "lblToday"
        Me.lblToday.Size = New System.Drawing.Size(50, 20)
        Me.lblToday.Text = "Today:"
        '
        'lblTodayUnits
        '
        Me.lblTodayUnits.Location = New System.Drawing.Point(99, 166)
        Me.lblTodayUnits.Name = "lblTodayUnits"
        Me.lblTodayUnits.Size = New System.Drawing.Size(39, 20)
        Me.lblTodayUnits.Text = "1,000"
        '
        'lblTodayValue
        '
        Me.lblTodayValue.Location = New System.Drawing.Point(144, 166)
        Me.lblTodayValue.Name = "lblTodayValue"
        Me.lblTodayValue.Size = New System.Drawing.Size(96, 20)
        '
        'lblThisWeek
        '
        Me.lblThisWeek.Location = New System.Drawing.Point(21, 192)
        Me.lblThisWeek.Name = "lblThisWeek"
        Me.lblThisWeek.Size = New System.Drawing.Size(70, 20)
        Me.lblThisWeek.Text = "This Week:"
        '
        'lblThisWeekUnits
        '
        Me.lblThisWeekUnits.Location = New System.Drawing.Point(99, 192)
        Me.lblThisWeekUnits.Name = "lblThisWeekUnits"
        Me.lblThisWeekUnits.Size = New System.Drawing.Size(39, 20)
        '
        'lblThisWeekValue
        '
        Me.lblThisWeekValue.Location = New System.Drawing.Point(144, 192)
        Me.lblThisWeekValue.Name = "lblThisWeekValue"
        Me.lblThisWeekValue.Size = New System.Drawing.Size(96, 20)
        '
        'lblStockFigure
        '
        Me.lblStockFigure.Location = New System.Drawing.Point(12, 216)
        Me.lblStockFigure.Name = "lblStockFigure"
        Me.lblStockFigure.Size = New System.Drawing.Size(77, 20)
        Me.lblStockFigure.Text = "Stock Figure:"
        '
        'lblStockFigureText
        '
        Me.lblStockFigureText.Location = New System.Drawing.Point(99, 216)
        Me.lblStockFigureText.Name = "lblStockFigureText"
        Me.lblStockFigureText.Size = New System.Drawing.Size(34, 20)
        '
        'lblScanItem
        '
        Me.lblScanItem.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblScanItem.Location = New System.Drawing.Point(12, 236)
        Me.lblScanItem.Name = "lblScanItem"
        Me.lblScanItem.Size = New System.Drawing.Size(140, 32)
        Me.lblScanItem.Text = "Scan/Enter next item or Quit" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'btn_Quit_small
        '
        Me.btn_Quit_small.BackColor = System.Drawing.Color.Transparent
        Me.btn_Quit_small.Location = New System.Drawing.Point(172, 240)
        Me.btn_Quit_small.Name = "btn_Quit_small"
        Me.btn_Quit_small.Size = New System.Drawing.Size(50, 24)
        Me.btn_Quit_small.TabIndex = 38
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 12
        '
        'frmISItemDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.btn_Quit_small)
        Me.Controls.Add(Me.lblScanItem)
        Me.Controls.Add(Me.lblStockFigureText)
        Me.Controls.Add(Me.lblStockFigure)
        Me.Controls.Add(Me.lblThisWeekValue)
        Me.Controls.Add(Me.lblThisWeekUnits)
        Me.Controls.Add(Me.lblThisWeek)
        Me.Controls.Add(Me.lblTodayValue)
        Me.Controls.Add(Me.lblTodayUnits)
        Me.Controls.Add(Me.lblToday)
        Me.Controls.Add(Me.lblValue)
        Me.Controls.Add(Me.lblUnits)
        Me.Controls.Add(Me.btn_CalcPad_small)
        Me.Controls.Add(Me.lblStatusText)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.btn_Info_button_i)
        Me.Controls.Add(Me.lblProdDescription3)
        Me.Controls.Add(Me.lblProdDescription2)
        Me.Controls.Add(Me.lblProdDescription1)
        Me.Controls.Add(Me.lblProductCode)
        Me.Controls.Add(Me.lblBootsCode)
        Me.Controls.Add(Me.objStatusBar)
        Me.Name = "frmISItemDetails"
        Me.Text = "Item Sales"
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblStatusText As System.Windows.Forms.Label
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents btn_Info_button_i As CustomButtons.info_button_i
    Friend WithEvents lblProdDescription3 As System.Windows.Forms.Label
    Friend WithEvents lblProdDescription2 As System.Windows.Forms.Label
    Friend WithEvents lblProdDescription1 As System.Windows.Forms.Label
    Friend WithEvents lblProductCode As System.Windows.Forms.Label
    Friend WithEvents lblBootsCode As System.Windows.Forms.Label
    Friend WithEvents btn_CalcPad_small As CustomButtons.btn_CalcPad_small
    Friend WithEvents lblUnits As System.Windows.Forms.Label
    Friend WithEvents lblValue As System.Windows.Forms.Label
    Friend WithEvents lblToday As System.Windows.Forms.Label
    Friend WithEvents lblTodayUnits As System.Windows.Forms.Label
    Friend WithEvents lblTodayValue As System.Windows.Forms.Label
    Friend WithEvents lblThisWeek As System.Windows.Forms.Label
    Friend WithEvents lblThisWeekUnits As System.Windows.Forms.Label
    Friend WithEvents lblThisWeekValue As System.Windows.Forms.Label
    Friend WithEvents lblStockFigure As System.Windows.Forms.Label
    Friend WithEvents lblStockFigureText As System.Windows.Forms.Label
    Friend WithEvents lblScanItem As System.Windows.Forms.Label
    Friend WithEvents btn_Quit_small As CustomButtons.btn_Quit_small
End Class
