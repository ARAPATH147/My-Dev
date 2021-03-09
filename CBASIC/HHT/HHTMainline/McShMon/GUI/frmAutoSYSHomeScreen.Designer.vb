<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmAutoSYSHomeScreen
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
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.Btn_Quit_small1 = New CustomButtons.btn_Quit_small
        Me.lblItems = New System.Windows.Forms.Label
        Me.lblItemsScanned = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.objProdSEL = New MCShMon.ProdSEL
        Me.lblScanPCSEL = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(184, 5)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 0
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(164, 232)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.TabIndex = 1
        '
        'lblItems
        '
        Me.lblItems.Location = New System.Drawing.Point(25, 176)
        Me.lblItems.Name = "lblItems"
        Me.lblItems.Size = New System.Drawing.Size(61, 20)
        Me.lblItems.Text = "Items:"
        '
        'lblItemsScanned
        '
        Me.lblItemsScanned.Location = New System.Drawing.Point(93, 176)
        Me.lblItemsScanned.Name = "lblItemsScanned"
        Me.lblItemsScanned.Size = New System.Drawing.Size(100, 20)
        Me.lblItemsScanned.Text = "0"
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 23
        '
        'objProdSEL
        '
        Me.objProdSEL.Location = New System.Drawing.Point(25, 39)
        Me.objProdSEL.Name = "objProdSEL"
        Me.objProdSEL.Size = New System.Drawing.Size(187, 117)
        Me.objProdSEL.TabIndex = 27
        '
        'lblScanPCSEL
        '
        Me.lblScanPCSEL.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblScanPCSEL.Location = New System.Drawing.Point(22, 196)
        Me.lblScanPCSEL.Name = "lblScanPCSEL"
        Me.lblScanPCSEL.Size = New System.Drawing.Size(214, 33)
        Me.lblScanPCSEL.Text = "Scan/Enter Product Code or Scan SEL"
        '
        'frmAutoSYSHomeScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblScanPCSEL)
        Me.Controls.Add(Me.objProdSEL)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblItemsScanned)
        Me.Controls.Add(Me.lblItems)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Name = "frmAutoSYSHomeScreen"
        Me.Text = "Stuff Your Shelves"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents Btn_Quit_small1 As CustomButtons.btn_Quit_small
    Friend WithEvents lblItems As System.Windows.Forms.Label
    Friend WithEvents lblItemsScanned As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents objProdSEL As MCShMon.ProdSEL
    Friend WithEvents lblScanPCSEL As System.Windows.Forms.Label
End Class
