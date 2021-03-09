<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmEXSelectLocation
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEXSelectLocation))
        Me.lblOffSiteStackRoom = New System.Windows.Forms.Label
        Me.btn_OSSR = New CustomButtons.btn_OSSR
        Me.lblBackShop = New System.Windows.Forms.Label
        Me.lblOffSite = New System.Windows.Forms.Label
        Me.lblSelectLoc = New System.Windows.Forms.Label
        Me.Btn_Quit1 = New CustomButtons.btn_Quit
        Me.btnBackShop = New CustomButtons.BackShop
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblOffSiteStackRoom
        '
        Me.lblOffSiteStackRoom.Location = New System.Drawing.Point(36, 28)
        Me.lblOffSiteStackRoom.Name = "lblOffSiteStackRoom"
        Me.lblOffSiteStackRoom.Size = New System.Drawing.Size(143, 32)
        Me.lblOffSiteStackRoom.Text = "This store has an Offsite Stock room"
        '
        'btn_OSSR
        '
        Me.btn_OSSR.BackColor = System.Drawing.Color.Transparent
        Me.btn_OSSR.Location = New System.Drawing.Point(35, 138)
        Me.btn_OSSR.Name = "btn_OSSR"
        Me.btn_OSSR.Size = New System.Drawing.Size(76, 20)
        Me.btn_OSSR.TabIndex = 2
        '
        'lblBackShop
        '
        Me.lblBackShop.Location = New System.Drawing.Point(127, 94)
        Me.lblBackShop.Name = "lblBackShop"
        Me.lblBackShop.Size = New System.Drawing.Size(75, 20)
        Me.lblBackShop.Text = "Back Shop"
        '
        'lblOffSite
        '
        Me.lblOffSite.Location = New System.Drawing.Point(127, 140)
        Me.lblOffSite.Name = "lblOffSite"
        Me.lblOffSite.Size = New System.Drawing.Size(76, 20)
        Me.lblOffSite.Text = "Off Site"
        '
        'lblSelectLoc
        '
        Me.lblSelectLoc.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelectLoc.Location = New System.Drawing.Point(35, 198)
        Me.lblSelectLoc.Name = "lblSelectLoc"
        Me.lblSelectLoc.Size = New System.Drawing.Size(144, 20)
        Me.lblSelectLoc.Text = "Select Count Location"
        '
        'Btn_Quit1
        '
        Me.Btn_Quit1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Quit1.Location = New System.Drawing.Point(175, 221)
        Me.Btn_Quit1.Name = "Btn_Quit1"
        Me.Btn_Quit1.Size = New System.Drawing.Size(40, 40)
        Me.Btn_Quit1.TabIndex = 4
        '
        'btnBackShop
        '
        Me.btnBackShop.BackColor = System.Drawing.Color.Transparent
        Me.btnBackShop.Location = New System.Drawing.Point(35, 94)
        Me.btnBackShop.Name = "btnBackShop"
        Me.btnBackShop.Size = New System.Drawing.Size(75, 21)
        Me.btnBackShop.TabIndex = 36
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 31
        '
        'frmEXSelectLocation
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnBackShop)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.Btn_Quit1)
        Me.Controls.Add(Me.lblSelectLoc)
        Me.Controls.Add(Me.lblOffSite)
        Me.Controls.Add(Me.lblBackShop)
        Me.Controls.Add(Me.btn_OSSR)
        Me.Controls.Add(Me.lblOffSiteStackRoom)
        Me.Name = "frmEXSelectLocation"
        Me.Text = "Excess Stock"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblOffSiteStackRoom As System.Windows.Forms.Label
    Friend WithEvents btn_OSSR As CustomButtons.btn_OSSR
    Friend WithEvents lblBackShop As System.Windows.Forms.Label
    Friend WithEvents lblOffSite As System.Windows.Forms.Label
    Friend WithEvents lblSelectLoc As System.Windows.Forms.Label
    Friend WithEvents Btn_Quit1 As CustomButtons.btn_Quit
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents btnBackShop As CustomButtons.BackShop
End Class
