<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmPLHome
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
        Me.custCtrlBtnQuit = New CustomButtons.btn_Quit_small
        Me.btnHelp = New CustomButtons.btn_Info
        Me.btnInfo = New CustomButtons.info_button_i
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.lstView = New MCShMon.AdvancedListView
        Me.lblSelectList = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'custCtrlBtnQuit
        '
        Me.custCtrlBtnQuit.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnQuit.Location = New System.Drawing.Point(177, 242)
        Me.custCtrlBtnQuit.Name = "custCtrlBtnQuit"
        Me.custCtrlBtnQuit.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnQuit.TabIndex = 7
        '
        'btnHelp
        '
        Me.btnHelp.BackColor = System.Drawing.Color.Transparent
        Me.btnHelp.Location = New System.Drawing.Point(74, 235)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(32, 32)
        Me.btnHelp.TabIndex = 6
        '
        'btnInfo
        '
        Me.btnInfo.BackColor = System.Drawing.Color.Transparent
        Me.btnInfo.Location = New System.Drawing.Point(13, 235)
        Me.btnInfo.Name = "btnInfo"
        Me.btnInfo.Size = New System.Drawing.Size(32, 32)
        Me.btnInfo.TabIndex = 5
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 8
        '
        'lstView
        '
        Me.lstView.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.lstView.FullRowSelect = True
        Me.lstView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lstView.Location = New System.Drawing.Point(2, 2)
        Me.lstView.Name = "lstView"
        Me.lstView.Size = New System.Drawing.Size(235, 207)
        Me.lstView.TabIndex = 4
        Me.lstView.View = System.Windows.Forms.View.Details
        '
        'lblSelectList
        '
        Me.lblSelectList.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelectList.Location = New System.Drawing.Point(3, 213)
        Me.lblSelectList.Name = "lblSelectList"
        Me.lblSelectList.Size = New System.Drawing.Size(145, 20)
        Me.lblSelectList.Text = "Select a List"
        '
        'frmPLHome
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblSelectList)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.custCtrlBtnQuit)
        Me.Controls.Add(Me.btnHelp)
        Me.Controls.Add(Me.btnInfo)
        Me.Controls.Add(Me.lstView)
        Me.Name = "frmPLHome"
        Me.Text = "Picking List"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lstView As MCShMon.AdvancedListView
    Friend WithEvents custCtrlBtnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents btnHelp As CustomButtons.btn_Info
    Friend WithEvents btnInfo As CustomButtons.info_button_i
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblSelectList As System.Windows.Forms.Label
End Class
