<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSuppliersList
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
        Me.lblBusCentreDesc = New System.Windows.Forms.Label
        Me.lblTitle = New System.Windows.Forms.Label
        Me.lblSupplierslist = New System.Windows.Forms.Label
        Me.lvSuppliersList = New System.Windows.Forms.ListView
        Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader2 = New System.Windows.Forms.ColumnHeader
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.btnNext = New CustomButtons.btn_Next_small
        Me.btnBack = New CustomButtons.btn_Back_sm
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblBusCentreDesc
        '
        Me.lblBusCentreDesc.ForeColor = System.Drawing.Color.Black
        Me.lblBusCentreDesc.Location = New System.Drawing.Point(6, 28)
        Me.lblBusCentreDesc.Name = "lblBusCentreDesc"
        Me.lblBusCentreDesc.Size = New System.Drawing.Size(224, 24)
        Me.lblBusCentreDesc.Text = "TEST RECALL12345678"
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.ForeColor = System.Drawing.Color.Black
        Me.lblTitle.Location = New System.Drawing.Point(6, 9)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(224, 24)
        Me.lblTitle.Text = "Returns : Faulty"
        '
        'lblSupplierslist
        '
        Me.lblSupplierslist.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSupplierslist.ForeColor = System.Drawing.Color.Black
        Me.lblSupplierslist.Location = New System.Drawing.Point(10, 52)
        Me.lblSupplierslist.Name = "lblSupplierslist"
        Me.lblSupplierslist.Size = New System.Drawing.Size(117, 24)
        Me.lblSupplierslist.Text = "Suppliers List"
        '
        'lvSuppliersList
        '
        Me.lvSuppliersList.Columns.Add(Me.ColumnHeader1)
        Me.lvSuppliersList.Columns.Add(Me.ColumnHeader2)
        Me.lvSuppliersList.FullRowSelect = True
        Me.lvSuppliersList.Location = New System.Drawing.Point(10, 73)
        Me.lvSuppliersList.Name = "lvSuppliersList"
        Me.lvSuppliersList.Size = New System.Drawing.Size(220, 135)
        Me.lvSuppliersList.TabIndex = 6
        Me.lvSuppliersList.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Supplier Code"
        Me.ColumnHeader1.Width = 101
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Supplier Name"
        Me.ColumnHeader2.Width = 116
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(5, 211)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(233, 24)
        Me.Label1.Text = "Select a SupplierCode and click Next"
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(180, 235)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 9
        '
        'btnNext
        '
        Me.btnNext.BackColor = System.Drawing.Color.Transparent
        Me.btnNext.Location = New System.Drawing.Point(10, 235)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(50, 24)
        Me.btnNext.TabIndex = 10
        '
        'btnBack
        '
        Me.btnBack.BackColor = System.Drawing.Color.Transparent
        Me.btnBack.Location = New System.Drawing.Point(95, 235)
        Me.btnBack.Name = "btnBack"
        Me.btnBack.Size = New System.Drawing.Size(50, 24)
        Me.btnBack.TabIndex = 11
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 16
        '
        'frmSuppliersList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnBack)
        Me.Controls.Add(Me.btnNext)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lvSuppliersList)
        Me.Controls.Add(Me.lblSupplierslist)
        Me.Controls.Add(Me.lblBusCentreDesc)
        Me.Controls.Add(Me.lblTitle)
        Me.Name = "frmSuppliersList"
        Me.Text = "Goods Out"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblBusCentreDesc As System.Windows.Forms.Label
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblSupplierslist As System.Windows.Forms.Label
    Friend WithEvents lvSuppliersList As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents btnNext As CustomButtons.btn_Next_small
    Friend WithEvents btnBack As CustomButtons.btn_Back_sm
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
End Class
