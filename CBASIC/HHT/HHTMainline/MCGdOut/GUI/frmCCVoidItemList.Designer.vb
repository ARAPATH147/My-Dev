<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmCCVoidItemList
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
        Me.btnVoidItem = New CustomButtons.btn_VoidItem
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.lvItemList = New System.Windows.Forms.ListView
        Me.lvNoOfItems = New System.Windows.Forms.ColumnHeader
        Me.lvItemDescription = New System.Windows.Forms.ColumnHeader
        Me.lblTotalData = New System.Windows.Forms.Label
        Me.lblTitle = New System.Windows.Forms.Label
        Me.lblList = New System.Windows.Forms.Label
        Me.lblTotal = New System.Windows.Forms.Label
        Me.btnNext = New CustomButtons.btn_Next_small
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.SuspendLayout()
        '
        'btnVoidItem
        '
        Me.btnVoidItem.BackColor = System.Drawing.Color.Transparent
        Me.btnVoidItem.Location = New System.Drawing.Point(88, 229)
        Me.btnVoidItem.Name = "btnVoidItem"
        Me.btnVoidItem.Size = New System.Drawing.Size(65, 24)
        Me.btnVoidItem.TabIndex = 94
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(180, 229)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 93
        '
        'lvItemList
        '
        Me.lvItemList.Columns.Add(Me.lvNoOfItems)
        Me.lvItemList.Columns.Add(Me.lvItemDescription)
        Me.lvItemList.FullRowSelect = True
        Me.lvItemList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lvItemList.Location = New System.Drawing.Point(8, 79)
        Me.lvItemList.Name = "lvItemList"
        Me.lvItemList.Size = New System.Drawing.Size(224, 127)
        Me.lvItemList.TabIndex = 92
        Me.lvItemList.View = System.Windows.Forms.View.Details
        '
        'lvNoOfItems
        '
        Me.lvNoOfItems.Text = "Count"
        Me.lvNoOfItems.Width = 50
        '
        'lvItemDescription
        '
        Me.lvItemDescription.Text = "Item Description"
        Me.lvItemDescription.Width = 170
        '
        'lblTotalData
        '
        Me.lblTotalData.Location = New System.Drawing.Point(180, 61)
        Me.lblTotalData.Name = "lblTotalData"
        Me.lblTotalData.Size = New System.Drawing.Size(50, 20)
        Me.lblTotalData.Text = "888888"
        Me.lblTotalData.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.ForeColor = System.Drawing.Color.Black
        Me.lblTitle.Location = New System.Drawing.Point(6, 9)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(231, 25)
        Me.lblTitle.Text = "Recall : Recalls and Recovery"
        '
        'lblList
        '
        Me.lblList.Location = New System.Drawing.Point(6, 60)
        Me.lblList.Name = "lblList"
        Me.lblList.Size = New System.Drawing.Size(120, 24)
        Me.lblList.Text = "Items In Claim"
        '
        'lblTotal
        '
        Me.lblTotal.Location = New System.Drawing.Point(128, 60)
        Me.lblTotal.Name = "lblTotal"
        Me.lblTotal.Size = New System.Drawing.Size(59, 20)
        Me.lblTotal.Text = "Total :"
        Me.lblTotal.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'btnNext
        '
        Me.btnNext.BackColor = System.Drawing.Color.Transparent
        Me.btnNext.Location = New System.Drawing.Point(11, 229)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(50, 24)
        Me.btnNext.TabIndex = 107
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 112
        '
        'frmCCVoidItemList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lvItemList)
        Me.Controls.Add(Me.lblTotal)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnNext)
        Me.Controls.Add(Me.btnVoidItem)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.lblTotalData)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.lblList)
        Me.Name = "frmCCVoidItemList"
        Me.Text = "Credit Claim"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnVoidItem As CustomButtons.btn_VoidItem
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents lvItemList As System.Windows.Forms.ListView
    Friend WithEvents lvNoOfItems As System.Windows.Forms.ColumnHeader
    Friend WithEvents lvItemDescription As System.Windows.Forms.ColumnHeader
    Friend WithEvents lblTotalData As System.Windows.Forms.Label
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblList As System.Windows.Forms.Label
    Friend WithEvents lblTotal As System.Windows.Forms.Label
    Friend WithEvents btnNext As CustomButtons.btn_Next_small
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
End Class
