<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmRLScanUOD
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
        Me.btnCalcpad = New CustomButtons.btn_CalcPad_small
        Me.lblLabel = New System.Windows.Forms.Label
        Me.txtBarcode = New System.Windows.Forms.TextBox
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.lwItemList = New System.Windows.Forms.ListView
        Me.lwNoOfItems = New System.Windows.Forms.ColumnHeader
        Me.lvItemDescription = New System.Windows.Forms.ColumnHeader
        Me.lblTotalData = New System.Windows.Forms.Label
        Me.lblTitle = New System.Windows.Forms.Label
        Me.lblScanColour = New System.Windows.Forms.Label
        Me.lblListDesc = New System.Windows.Forms.Label
        Me.lblList = New System.Windows.Forms.Label
        Me.lblTotal = New System.Windows.Forms.Label
        Me.pnScanLabelColourIndicator = New System.Windows.Forms.TextBox
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.SuspendLayout()
        '
        'btnCalcpad
        '
        Me.btnCalcpad.BackColor = System.Drawing.Color.Transparent
        Me.btnCalcpad.Location = New System.Drawing.Point(204, 178)
        Me.btnCalcpad.Name = "btnCalcpad"
        Me.btnCalcpad.Size = New System.Drawing.Size(24, 28)
        Me.btnCalcpad.TabIndex = 105
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
        Me.txtBarcode.TabIndex = 104
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(180, 235)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 102
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
        Me.lwItemList.TabIndex = 101
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
        'lblTotalData
        '
        Me.lblTotalData.Location = New System.Drawing.Point(180, 62)
        Me.lblTotalData.Name = "lblTotalData"
        Me.lblTotalData.Size = New System.Drawing.Size(52, 20)
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
        'lblScanColour
        '
        Me.lblScanColour.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblScanColour.Location = New System.Drawing.Point(61, 211)
        Me.lblScanColour.Name = "lblScanColour"
        Me.lblScanColour.Size = New System.Drawing.Size(176, 20)
        Me.lblScanColour.Text = "Scan Purple Label"
        '
        'lblListDesc
        '
        Me.lblListDesc.ForeColor = System.Drawing.Color.Black
        Me.lblListDesc.Location = New System.Drawing.Point(6, 33)
        Me.lblListDesc.Name = "lblListDesc"
        Me.lblListDesc.Size = New System.Drawing.Size(224, 24)
        Me.lblListDesc.Text = "Beauty n Care"
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
        Me.lblTotal.Location = New System.Drawing.Point(128, 62)
        Me.lblTotal.Name = "lblTotal"
        Me.lblTotal.Size = New System.Drawing.Size(59, 20)
        Me.lblTotal.Text = "Total :"
        Me.lblTotal.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'pnScanLabelColourIndicator
        '
        Me.pnScanLabelColourIndicator.BackColor = System.Drawing.Color.Purple
        Me.pnScanLabelColourIndicator.Enabled = False
        Me.pnScanLabelColourIndicator.Location = New System.Drawing.Point(8, 208)
        Me.pnScanLabelColourIndicator.Multiline = True
        Me.pnScanLabelColourIndicator.Name = "pnScanLabelColourIndicator"
        Me.pnScanLabelColourIndicator.ReadOnly = True
        Me.pnScanLabelColourIndicator.Size = New System.Drawing.Size(48, 24)
        Me.pnScanLabelColourIndicator.TabIndex = 122
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 113
        '
        'frmRLScanUOD
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.pnScanLabelColourIndicator)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lwItemList)
        Me.Controls.Add(Me.lblTotal)
        Me.Controls.Add(Me.btnCalcpad)
        Me.Controls.Add(Me.lblLabel)
        Me.Controls.Add(Me.txtBarcode)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.lblTotalData)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.lblScanColour)
        Me.Controls.Add(Me.lblListDesc)
        Me.Controls.Add(Me.lblList)
        Me.Name = "frmRLScanUOD"
        Me.Text = "Recalls"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnCalcpad As CustomButtons.btn_CalcPad_small
    Friend WithEvents lblLabel As System.Windows.Forms.Label
    Friend WithEvents txtBarcode As System.Windows.Forms.TextBox
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents lwItemList As System.Windows.Forms.ListView
    Friend WithEvents lwNoOfItems As System.Windows.Forms.ColumnHeader
    Friend WithEvents lvItemDescription As System.Windows.Forms.ColumnHeader
    Friend WithEvents lblTotalData As System.Windows.Forms.Label
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblScanColour As System.Windows.Forms.Label
    Friend WithEvents lblListDesc As System.Windows.Forms.Label
    Friend WithEvents lblList As System.Windows.Forms.Label
    Friend WithEvents lblTotal As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
    Friend WithEvents pnScanLabelColourIndicator As System.Windows.Forms.TextBox
End Class
