<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmCLViewListScreen
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
        Me.lblItemSelect = New System.Windows.Forms.Label
        Me.custCtrlBtnQuit = New CustomButtons.btn_Quit_small
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.lblViewList = New System.Windows.Forms.Label
        Me.lblViewListSite = New System.Windows.Forms.Label
        Me.lblViewListSiteDisplay = New System.Windows.Forms.Label
        Me.lblDiscepancy = New System.Windows.Forms.Label
        Me.Btn_Help = New CustomButtons.btn_Info
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.lstvwItemDetails = New MCShMon.AdvancedListView
        Me.SuspendLayout()
        '
        'lblItemSelect
        '
        Me.lblItemSelect.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblItemSelect.Location = New System.Drawing.Point(3, 211)
        Me.lblItemSelect.Name = "lblItemSelect"
        Me.lblItemSelect.Size = New System.Drawing.Size(234, 20)
        '
        'custCtrlBtnQuit
        '
        Me.custCtrlBtnQuit.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnQuit.Location = New System.Drawing.Point(178, 243)
        Me.custCtrlBtnQuit.Name = "custCtrlBtnQuit"
        Me.custCtrlBtnQuit.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnQuit.TabIndex = 11
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(3, 235)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 9
        '
        'lblViewList
        '
        Me.lblViewList.Location = New System.Drawing.Point(3, 17)
        Me.lblViewList.Name = "lblViewList"
        Me.lblViewList.Size = New System.Drawing.Size(187, 20)
        Me.lblViewList.Visible = False
        '
        'lblViewListSite
        '
        Me.lblViewListSite.Location = New System.Drawing.Point(3, 37)
        Me.lblViewListSite.Name = "lblViewListSite"
        Me.lblViewListSite.Size = New System.Drawing.Size(46, 20)
        Me.lblViewListSite.Text = "Site :"
        '
        'lblViewListSiteDisplay
        '
        Me.lblViewListSiteDisplay.Location = New System.Drawing.Point(45, 37)
        Me.lblViewListSiteDisplay.Name = "lblViewListSiteDisplay"
        Me.lblViewListSiteDisplay.Size = New System.Drawing.Size(165, 20)
        '
        'lblDiscepancy
        '
        Me.lblDiscepancy.Location = New System.Drawing.Point(3, 10)
        Me.lblDiscepancy.Name = "lblDiscepancy"
        Me.lblDiscepancy.Size = New System.Drawing.Size(188, 47)
        '
        'Btn_Help
        '
        Me.Btn_Help.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Help.Location = New System.Drawing.Point(196, 10)
        Me.Btn_Help.Name = "Btn_Help"
        Me.Btn_Help.Size = New System.Drawing.Size(32, 32)
        Me.Btn_Help.TabIndex = 15
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 272)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 22)
        Me.objStatusBar.TabIndex = 13
        '
        'lstvwItemDetails
        '
        Me.lstvwItemDetails.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.lstvwItemDetails.FullRowSelect = True
        Me.lstvwItemDetails.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lstvwItemDetails.Location = New System.Drawing.Point(3, 60)
        Me.lstvwItemDetails.Name = "lstvwItemDetails"
        Me.lstvwItemDetails.Size = New System.Drawing.Size(234, 146)
        Me.lstvwItemDetails.TabIndex = 12
        Me.lstvwItemDetails.View = System.Windows.Forms.View.Details
        '
        'frmCLViewListScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Help)
        Me.Controls.Add(Me.lblDiscepancy)
        Me.Controls.Add(Me.lblViewListSiteDisplay)
        Me.Controls.Add(Me.lblViewListSite)
        Me.Controls.Add(Me.lblViewList)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblItemSelect)
        Me.Controls.Add(Me.lstvwItemDetails)
        Me.Controls.Add(Me.custCtrlBtnQuit)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Name = "frmCLViewListScreen"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblItemSelect As System.Windows.Forms.Label
    Friend WithEvents lstvwItemDetails As MCShMon.AdvancedListView
    Friend WithEvents custCtrlBtnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents lblViewList As System.Windows.Forms.Label
    Friend WithEvents lblViewListSite As System.Windows.Forms.Label
    Friend WithEvents lblViewListSiteDisplay As System.Windows.Forms.Label
    Friend WithEvents lblDiscepancy As System.Windows.Forms.Label
    Friend WithEvents Btn_Help As CustomButtons.btn_Info
End Class
