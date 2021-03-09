<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmRLItemListScreen
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
        Me.lvRecallList = New System.Windows.Forms.ListView
        Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader2 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader3 = New System.Windows.Forms.ColumnHeader
        Me.lblItemInRecall = New System.Windows.Forms.Label
        Me.lblItemActioned = New System.Windows.Forms.Label
        Me.btnBack = New CustomButtons.btn_Back_sm
        Me.lblItemActioneddata = New System.Windows.Forms.Label
        Me.lblItemInRecalldata = New System.Windows.Forms.Label
        Me.lblRecallName = New System.Windows.Forms.Label
        Me.lblView = New System.Windows.Forms.Label
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.SuspendLayout()
        '
        'lvRecallList
        '
        Me.lvRecallList.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.lvRecallList.Columns.Add(Me.ColumnHeader1)
        Me.lvRecallList.Columns.Add(Me.ColumnHeader2)
        Me.lvRecallList.Columns.Add(Me.ColumnHeader3)
        Me.lvRecallList.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lvRecallList.FullRowSelect = True
        Me.lvRecallList.Location = New System.Drawing.Point(4, 38)
        Me.lvRecallList.Name = "lvRecallList"
        Me.lvRecallList.Size = New System.Drawing.Size(233, 145)
        Me.lvRecallList.TabIndex = 0
        Me.lvRecallList.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Items"
        Me.ColumnHeader1.Width = 55
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Start of Day Stock File"
        Me.ColumnHeader2.Width = 93
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Description"
        Me.ColumnHeader3.Width = 101
        '
        'lblItemInRecall
        '
        Me.lblItemInRecall.Location = New System.Drawing.Point(5, 186)
        Me.lblItemInRecall.Name = "lblItemInRecall"
        Me.lblItemInRecall.Size = New System.Drawing.Size(100, 20)
        Me.lblItemInRecall.Text = "Items In Recall :"
        '
        'lblItemActioned
        '
        Me.lblItemActioned.Location = New System.Drawing.Point(5, 206)
        Me.lblItemActioned.Name = "lblItemActioned"
        Me.lblItemActioned.Size = New System.Drawing.Size(100, 20)
        Me.lblItemActioned.Text = "Items Actioned :"
        '
        'btnBack
        '
        Me.btnBack.BackColor = System.Drawing.Color.Transparent
        Me.btnBack.Location = New System.Drawing.Point(180, 235)
        Me.btnBack.Name = "btnBack"
        Me.btnBack.Size = New System.Drawing.Size(50, 24)
        Me.btnBack.TabIndex = 4
        '
        'lblItemActioneddata
        '
        Me.lblItemActioneddata.Location = New System.Drawing.Point(177, 206)
        Me.lblItemActioneddata.Name = "lblItemActioneddata"
        Me.lblItemActioneddata.Size = New System.Drawing.Size(60, 20)
        Me.lblItemActioneddata.Text = "Label3"
        '
        'lblItemInRecalldata
        '
        Me.lblItemInRecalldata.Location = New System.Drawing.Point(177, 186)
        Me.lblItemInRecalldata.Name = "lblItemInRecalldata"
        Me.lblItemInRecalldata.Size = New System.Drawing.Size(60, 20)
        Me.lblItemInRecalldata.Text = "Label4"
        '
        'lblRecallName
        '
        Me.lblRecallName.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblRecallName.Location = New System.Drawing.Point(49, 15)
        Me.lblRecallName.Name = "lblRecallName"
        Me.lblRecallName.Size = New System.Drawing.Size(188, 20)
        Me.lblRecallName.Text = "AAAAAAAAAAAAAAAAAAAA"
        '
        'lblView
        '
        Me.lblView.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblView.Location = New System.Drawing.Point(4, 15)
        Me.lblView.Name = "lblView"
        Me.lblView.Size = New System.Drawing.Size(51, 20)
        Me.lblView.Text = "VIEW :"
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 7
        '
        'frmRLItemListScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblRecallName)
        Me.Controls.Add(Me.lblView)
        Me.Controls.Add(Me.lblItemActioneddata)
        Me.Controls.Add(Me.lblItemInRecalldata)
        Me.Controls.Add(Me.btnBack)
        Me.Controls.Add(Me.lblItemActioned)
        Me.Controls.Add(Me.lblItemInRecall)
        Me.Controls.Add(Me.lvRecallList)
        Me.Name = "frmRLItemListScreen"
        Me.Text = "Recalls"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lvRecallList As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents lblItemInRecall As System.Windows.Forms.Label
    Friend WithEvents lblItemActioned As System.Windows.Forms.Label
    Friend WithEvents btnBack As CustomButtons.btn_Back_sm
    Friend WithEvents lblItemActioneddata As System.Windows.Forms.Label
    Friend WithEvents lblItemInRecalldata As System.Windows.Forms.Label
    Friend WithEvents lblRecallName As System.Windows.Forms.Label
    Friend WithEvents lblView As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
End Class
