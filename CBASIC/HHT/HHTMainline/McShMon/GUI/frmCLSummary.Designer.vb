<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmCLSummary
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
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblNumBackShop = New System.Windows.Forms.Label
        Me.lblNumSalesFloor = New System.Windows.Forms.Label
        Me.lblBackShop = New System.Windows.Forms.Label
        Me.lblSalesFloor = New System.Windows.Forms.Label
        Me.lblCountListDisplay = New System.Windows.Forms.Label
        Me.btnOK = New CustomButtons.btn_Ok
        Me.lblOSSRSite = New System.Windows.Forms.Label
        Me.lblOSSRSiteVal = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(13, 179)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(224, 31)
        Me.Label1.Text = "Action:Dock and Transmit or Please Continue with another list"
        '
        'lblNumBackShop
        '
        Me.lblNumBackShop.Location = New System.Drawing.Point(150, 99)
        Me.lblNumBackShop.Name = "lblNumBackShop"
        Me.lblNumBackShop.Size = New System.Drawing.Size(39, 20)
        Me.lblNumBackShop.Text = "000"
        '
        'lblNumSalesFloor
        '
        Me.lblNumSalesFloor.Location = New System.Drawing.Point(150, 60)
        Me.lblNumSalesFloor.Name = "lblNumSalesFloor"
        Me.lblNumSalesFloor.Size = New System.Drawing.Size(39, 20)
        Me.lblNumSalesFloor.Text = "000"
        '
        'lblBackShop
        '
        Me.lblBackShop.Location = New System.Drawing.Point(52, 100)
        Me.lblBackShop.Name = "lblBackShop"
        Me.lblBackShop.Size = New System.Drawing.Size(78, 20)
        Me.lblBackShop.Text = "Back Shop:"
        '
        'lblSalesFloor
        '
        Me.lblSalesFloor.Location = New System.Drawing.Point(52, 60)
        Me.lblSalesFloor.Name = "lblSalesFloor"
        Me.lblSalesFloor.Size = New System.Drawing.Size(78, 20)
        Me.lblSalesFloor.Text = "Sales Floor:"
        '
        'lblCountListDisplay
        '
        Me.lblCountListDisplay.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblCountListDisplay.Location = New System.Drawing.Point(13, 16)
        Me.lblCountListDisplay.Name = "lblCountListDisplay"
        Me.lblCountListDisplay.Size = New System.Drawing.Size(224, 20)
        Me.lblCountListDisplay.Text = "List is Complete"
        '
        'btnOK
        '
        Me.btnOK.BackColor = System.Drawing.Color.Transparent
        Me.btnOK.Location = New System.Drawing.Point(100, 223)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(40, 40)
        Me.btnOK.TabIndex = 34
        '
        'lblOSSRSite
        '
        Me.lblOSSRSite.Location = New System.Drawing.Point(52, 140)
        Me.lblOSSRSite.Name = "lblOSSRSite"
        Me.lblOSSRSite.Size = New System.Drawing.Size(78, 20)
        Me.lblOSSRSite.Text = "OSSR Site:"
        '
        'lblOSSRSiteVal
        '
        Me.lblOSSRSiteVal.Location = New System.Drawing.Point(150, 140)
        Me.lblOSSRSiteVal.Name = "lblOSSRSiteVal"
        Me.lblOSSRSiteVal.Size = New System.Drawing.Size(39, 20)
        Me.lblOSSRSiteVal.Text = "000"
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 41
        '
        'frmCLSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblOSSRSiteVal)
        Me.Controls.Add(Me.lblOSSRSite)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.lblNumBackShop)
        Me.Controls.Add(Me.lblNumSalesFloor)
        Me.Controls.Add(Me.lblBackShop)
        Me.Controls.Add(Me.lblSalesFloor)
        Me.Controls.Add(Me.lblCountListDisplay)
        Me.Controls.Add(Me.Label1)
        Me.Name = "frmCLSummary"
        Me.Text = "Count List"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblNumBackShop As System.Windows.Forms.Label
    Friend WithEvents lblNumSalesFloor As System.Windows.Forms.Label
    Friend WithEvents lblBackShop As System.Windows.Forms.Label
    Friend WithEvents lblSalesFloor As System.Windows.Forms.Label
    Friend WithEvents lblCountListDisplay As System.Windows.Forms.Label
    Friend WithEvents btnOK As CustomButtons.btn_Ok
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblOSSRSite As System.Windows.Forms.Label
    Friend WithEvents lblOSSRSiteVal As System.Windows.Forms.Label
End Class
