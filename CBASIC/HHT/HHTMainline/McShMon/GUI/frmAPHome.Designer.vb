<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmAPHome
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
        Me.lblCurrentPrinter = New System.Windows.Forms.Label
        Me.lblAvailablePrinter = New System.Windows.Forms.Label
        Me.lblSelectPrinter = New System.Windows.Forms.Label
        Me.lblCurPrinter = New System.Windows.Forms.Label
        Me.btn_Quit_small = New CustomButtons.btn_Quit_small
        Me.Btn_Save = New CustomButtons.btn_Save
        Me.Btn_Utilities = New CustomButtons.btn_Utilities
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.lstAvailablePrinter = New MCShMon.AdvancedListView
        Me.SuspendLayout()
        '
        'lblCurrentPrinter
        '
        Me.lblCurrentPrinter.Location = New System.Drawing.Point(21, 13)
        Me.lblCurrentPrinter.Name = "lblCurrentPrinter"
        Me.lblCurrentPrinter.Size = New System.Drawing.Size(100, 20)
        Me.lblCurrentPrinter.Text = "Current Printer"
        '
        'lblAvailablePrinter
        '
        Me.lblAvailablePrinter.Location = New System.Drawing.Point(21, 53)
        Me.lblAvailablePrinter.Name = "lblAvailablePrinter"
        Me.lblAvailablePrinter.Size = New System.Drawing.Size(100, 20)
        Me.lblAvailablePrinter.Text = "Available Printers"
        '
        'lblSelectPrinter
        '
        Me.lblSelectPrinter.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelectPrinter.Location = New System.Drawing.Point(32, 205)
        Me.lblSelectPrinter.Name = "lblSelectPrinter"
        Me.lblSelectPrinter.Size = New System.Drawing.Size(165, 33)
        Me.lblSelectPrinter.Text = "Select a SEL printer from the list above"
        '
        'lblCurPrinter
        '
        Me.lblCurPrinter.Location = New System.Drawing.Point(32, 33)
        Me.lblCurPrinter.Name = "lblCurPrinter"
        Me.lblCurPrinter.Size = New System.Drawing.Size(199, 20)
        Me.lblCurPrinter.Text = "<Printer Name>"
        '
        'btn_Quit_small
        '
        Me.btn_Quit_small.BackColor = System.Drawing.Color.Transparent
        Me.btn_Quit_small.Location = New System.Drawing.Point(181, 241)
        Me.btn_Quit_small.Name = "btn_Quit_small"
        Me.btn_Quit_small.Size = New System.Drawing.Size(50, 24)
        Me.btn_Quit_small.TabIndex = 6
        '
        'Btn_Save
        '
        Me.Btn_Save.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Save.Location = New System.Drawing.Point(9, 241)
        Me.Btn_Save.Name = "Btn_Save"
        Me.Btn_Save.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Save.TabIndex = 5
        '
        'Btn_Utilities
        '
        Me.Btn_Utilities.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Utilities.Location = New System.Drawing.Point(86, 241)
        Me.Btn_Utilities.Name = "Btn_Utilities"
        Me.Btn_Utilities.Size = New System.Drawing.Size(65, 24)
        Me.Btn_Utilities.TabIndex = 57
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 31
        '
        'lstAvailablePrinter
        '
        Me.lstAvailablePrinter.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.lstAvailablePrinter.FullRowSelect = True
        Me.lstAvailablePrinter.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.lstAvailablePrinter.Location = New System.Drawing.Point(32, 76)
        Me.lstAvailablePrinter.Name = "lstAvailablePrinter"
        Me.lstAvailablePrinter.Size = New System.Drawing.Size(165, 104)
        Me.lstAvailablePrinter.TabIndex = 3
        Me.lstAvailablePrinter.View = System.Windows.Forms.View.Details
        '
        'frmAPHome
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Utilities)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblCurPrinter)
        Me.Controls.Add(Me.btn_Quit_small)
        Me.Controls.Add(Me.Btn_Save)
        Me.Controls.Add(Me.lblSelectPrinter)
        Me.Controls.Add(Me.lstAvailablePrinter)
        Me.Controls.Add(Me.lblAvailablePrinter)
        Me.Controls.Add(Me.lblCurrentPrinter)
        Me.Name = "frmAPHome"
        Me.Text = "Assign Printer"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblCurrentPrinter As System.Windows.Forms.Label
    Friend WithEvents lblAvailablePrinter As System.Windows.Forms.Label
    Friend WithEvents lstAvailablePrinter As MCShMon.AdvancedListView
    Friend WithEvents lblSelectPrinter As System.Windows.Forms.Label
    Friend WithEvents Btn_Save As CustomButtons.btn_Save
    Friend WithEvents btn_Quit_small As CustomButtons.btn_Quit_small
    Friend WithEvents lblCurPrinter As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents Btn_Utilities As CustomButtons.btn_Utilities
End Class
