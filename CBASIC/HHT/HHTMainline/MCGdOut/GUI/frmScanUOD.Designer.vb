<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmScanUOD
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
        Me.lblTitle = New System.Windows.Forms.Label
        Me.lblScanColour = New System.Windows.Forms.Label
        Me.lblBusCentreDesc = New System.Windows.Forms.Label
        Me.lblList = New System.Windows.Forms.Label
        Me.lblTotal = New System.Windows.Forms.Label
        Me.lblTotalData = New System.Windows.Forms.Label
        Me.lwItemList = New System.Windows.Forms.ListView
        Me.lwNoOfItems = New System.Windows.Forms.ColumnHeader
        Me.lvItemDescription = New System.Windows.Forms.ColumnHeader
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.btnVoidItem = New CustomButtons.btn_VoidItem
        Me.lblLabel = New System.Windows.Forms.Label
        Me.txtBarcode = New System.Windows.Forms.TextBox
        Me.btnCalcpad = New CustomButtons.btn_CalcPad_small
        Me.btnNext = New CustomButtons.btn_Next_small
        Me.pnScanLabelColourIndicator = New System.Windows.Forms.TextBox
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.ForeColor = System.Drawing.Color.Black
        Me.lblTitle.Location = New System.Drawing.Point(6, 9)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(231, 25)
        Me.lblTitle.Text = "Recall : Returns and Recovery"
        '
        'lblScanColour
        '
        Me.lblScanColour.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblScanColour.Location = New System.Drawing.Point(61, 211)
        Me.lblScanColour.Name = "lblScanColour"
        Me.lblScanColour.Size = New System.Drawing.Size(168, 20)
        Me.lblScanColour.Text = "Scan Purple Label"
        '
        'lblBusCentreDesc
        '
        Me.lblBusCentreDesc.ForeColor = System.Drawing.Color.Black
        Me.lblBusCentreDesc.Location = New System.Drawing.Point(8, 33)
        Me.lblBusCentreDesc.Name = "lblBusCentreDesc"
        Me.lblBusCentreDesc.Size = New System.Drawing.Size(224, 24)
        Me.lblBusCentreDesc.Text = "Beauty n Care"
        '
        'lblList
        '
        Me.lblList.Location = New System.Drawing.Point(3, 61)
        Me.lblList.Name = "lblList"
        Me.lblList.Size = New System.Drawing.Size(120, 24)
        Me.lblList.Text = "Items In UOD"
        '
        'lblTotal
        '
        Me.lblTotal.Location = New System.Drawing.Point(135, 61)
        Me.lblTotal.Name = "lblTotal"
        Me.lblTotal.Size = New System.Drawing.Size(59, 20)
        Me.lblTotal.Text = "Total :"
        Me.lblTotal.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblTotalData
        '
        Me.lblTotalData.Location = New System.Drawing.Point(193, 61)
        Me.lblTotalData.Name = "lblTotalData"
        Me.lblTotalData.Size = New System.Drawing.Size(39, 16)
        Me.lblTotalData.Text = "33333"
        Me.lblTotalData.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lwItemList
        '
        Me.lwItemList.Columns.Add(Me.lwNoOfItems)
        Me.lwItemList.Columns.Add(Me.lvItemDescription)
        Me.lwItemList.FullRowSelect = True
        Me.lwItemList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lwItemList.Location = New System.Drawing.Point(8, 80)
        Me.lwItemList.Name = "lwItemList"
        Me.lwItemList.Size = New System.Drawing.Size(224, 86)
        Me.lwItemList.TabIndex = 50
        Me.lwItemList.View = System.Windows.Forms.View.Details
        '
        'lwNoOfItems
        '
        Me.lwNoOfItems.Text = "Count"
        Me.lwNoOfItems.Width = 50
        '
        'lvItemDescription
        '
        Me.lvItemDescription.Text = "Item Description"
        Me.lvItemDescription.Width = 170
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(180, 235)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 52
        '
        'btnVoidItem
        '
        Me.btnVoidItem.BackColor = System.Drawing.Color.Transparent
        Me.btnVoidItem.Location = New System.Drawing.Point(90, 235)
        Me.btnVoidItem.Name = "btnVoidItem"
        Me.btnVoidItem.Size = New System.Drawing.Size(65, 24)
        Me.btnVoidItem.TabIndex = 53
        '
        'lblLabel
        '
        Me.lblLabel.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Bold)
        Me.lblLabel.Location = New System.Drawing.Point(8, 181)
        Me.lblLabel.Name = "lblLabel"
        Me.lblLabel.Size = New System.Drawing.Size(42, 24)
        Me.lblLabel.Text = "Label"
        '
        'txtBarcode
        '
        Me.txtBarcode.Location = New System.Drawing.Point(50, 181)
        Me.txtBarcode.MaxLength = 14
        Me.txtBarcode.Name = "txtBarcode"
        Me.txtBarcode.ReadOnly = True
        Me.txtBarcode.Size = New System.Drawing.Size(151, 21)
        Me.txtBarcode.TabIndex = 79
        '
        'btnCalcpad
        '
        Me.btnCalcpad.BackColor = System.Drawing.Color.Transparent
        Me.btnCalcpad.Location = New System.Drawing.Point(204, 178)
        Me.btnCalcpad.Name = "btnCalcpad"
        Me.btnCalcpad.Size = New System.Drawing.Size(24, 28)
        Me.btnCalcpad.TabIndex = 80
        '
        'btnNext
        '
        Me.btnNext.BackColor = System.Drawing.Color.Transparent
        Me.btnNext.Location = New System.Drawing.Point(8, 235)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(50, 24)
        Me.btnNext.TabIndex = 92
        '
        'pnScanLabelColourIndicator
        '
        Me.pnScanLabelColourIndicator.BackColor = System.Drawing.Color.Black
        Me.pnScanLabelColourIndicator.Enabled = False
        Me.pnScanLabelColourIndicator.Location = New System.Drawing.Point(8, 207)
        Me.pnScanLabelColourIndicator.Multiline = True
        Me.pnScanLabelColourIndicator.Name = "pnScanLabelColourIndicator"
        Me.pnScanLabelColourIndicator.ReadOnly = True
        Me.pnScanLabelColourIndicator.Size = New System.Drawing.Size(48, 24)
        Me.pnScanLabelColourIndicator.TabIndex = 110
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 101
        '
        'frmScanUOD
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.pnScanLabelColourIndicator)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnNext)
        Me.Controls.Add(Me.btnCalcpad)
        Me.Controls.Add(Me.lblLabel)
        Me.Controls.Add(Me.txtBarcode)
        Me.Controls.Add(Me.btnVoidItem)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.lwItemList)
        Me.Controls.Add(Me.lblTotalData)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.lblScanColour)
        Me.Controls.Add(Me.lblBusCentreDesc)
        Me.Controls.Add(Me.lblList)
        Me.Controls.Add(Me.lblTotal)
        Me.Name = "frmScanUOD"
        Me.Text = "Goods Out"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblScanColour As System.Windows.Forms.Label
    Friend WithEvents lblBusCentreDesc As System.Windows.Forms.Label
    Friend WithEvents lblList As System.Windows.Forms.Label
    Friend WithEvents lblTotal As System.Windows.Forms.Label
    Friend WithEvents lblTotalData As System.Windows.Forms.Label
    Friend WithEvents lwItemList As System.Windows.Forms.ListView
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents btnVoidItem As CustomButtons.btn_VoidItem
    Friend WithEvents lwNoOfItems As System.Windows.Forms.ColumnHeader
    Friend WithEvents lvItemDescription As System.Windows.Forms.ColumnHeader
    Friend WithEvents lblLabel As System.Windows.Forms.Label
    Friend WithEvents txtBarcode As System.Windows.Forms.TextBox
    Friend WithEvents btnCalcpad As CustomButtons.btn_CalcPad_small
    Friend WithEvents btnNext As CustomButtons.btn_Next_small
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
    Friend WithEvents pnScanLabelColourIndicator As System.Windows.Forms.TextBox
End Class
