Public Class frmBookInOrderSummaryOfContents
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblBookIn As System.Windows.Forms.Label
    Friend WithEvents lblSupplier As System.Windows.Forms.Label
    Friend WithEvents lblOrder As System.Windows.Forms.Label
    Friend WithEvents lblSummary As System.Windows.Forms.Label
    Friend WithEvents lblScan As System.Windows.Forms.Label
    Friend WithEvents lblOrderNo As System.Windows.Forms.Label
    Friend WithEvents lblCode As System.Windows.Forms.Label

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        Me.lvwSumOfContents.Activation = ItemActivation.OneClick
        Me.lvwSumOfContents.Font = New System.Drawing.Font("Courier New", 9.0!, FontStyle.Regular)
        'Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents Btn_CalcPad_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Finish1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents lvwSumOfContents As System.Windows.Forms.ListView
    Friend WithEvents Item As System.Windows.Forms.ColumnHeader
    Friend WithEvents Desc As System.Windows.Forms.ColumnHeader
    Friend WithEvents Expt As System.Windows.Forms.ColumnHeader
    Friend WithEvents Recd As System.Windows.Forms.ColumnHeader
    Friend WithEvents txtBarcode As System.Windows.Forms.TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBookInOrderSummaryOfContents))
        Me.lblBookIn = New System.Windows.Forms.Label
        Me.lblSupplier = New System.Windows.Forms.Label
        Me.lblOrder = New System.Windows.Forms.Label
        Me.lblSummary = New System.Windows.Forms.Label
        Me.lblScan = New System.Windows.Forms.Label
        Me.lblOrderNo = New System.Windows.Forms.Label
        Me.lvwSumOfContents = New System.Windows.Forms.ListView
        Me.Item = New System.Windows.Forms.ColumnHeader
        Me.Desc = New System.Windows.Forms.ColumnHeader
        Me.Expt = New System.Windows.Forms.ColumnHeader
        Me.Recd = New System.Windows.Forms.ColumnHeader
        Me.lblCode = New System.Windows.Forms.Label
        Me.Btn_CalcPad_small1 = New System.Windows.Forms.PictureBox
        Me.Btn_Finish1 = New System.Windows.Forms.PictureBox
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.txtBarcode = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'lblBookIn
        '
        Me.lblBookIn.Location = New System.Drawing.Point(16, 8)
        Me.lblBookIn.Name = "lblBookIn"
        Me.lblBookIn.Size = New System.Drawing.Size(100, 16)
        Me.lblBookIn.Text = "Book In Order"
        '
        'lblSupplier
        '
        Me.lblSupplier.Location = New System.Drawing.Point(16, 32)
        Me.lblSupplier.Name = "lblSupplier"
        Me.lblSupplier.Size = New System.Drawing.Size(160, 20)
        Me.lblSupplier.Text = "Fuji"
        '
        'lblOrder
        '
        Me.lblOrder.Location = New System.Drawing.Point(16, 52)
        Me.lblOrder.Name = "lblOrder"
        Me.lblOrder.Size = New System.Drawing.Size(56, 16)
        Me.lblOrder.Text = "Order "
        '
        'lblSummary
        '
        Me.lblSummary.Location = New System.Drawing.Point(16, 72)
        Me.lblSummary.Name = "lblSummary"
        Me.lblSummary.Size = New System.Drawing.Size(144, 16)
        Me.lblSummary.Text = "Summary of contents"
        '
        'lblScan
        '
        Me.lblScan.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblScan.Location = New System.Drawing.Point(16, 216)
        Me.lblScan.Name = "lblScan"
        Me.lblScan.Size = New System.Drawing.Size(120, 16)
        Me.lblScan.Text = "Scan / Enter item"
        '
        'lblOrderNo
        '
        Me.lblOrderNo.Location = New System.Drawing.Point(76, 52)
        Me.lblOrderNo.Name = "lblOrderNo"
        Me.lblOrderNo.Size = New System.Drawing.Size(64, 16)
        Me.lblOrderNo.Text = "Label6"
        '
        'lvwSumOfContents
        '
        Me.lvwSumOfContents.Columns.Add(Me.Item)
        Me.lvwSumOfContents.Columns.Add(Me.Desc)
        Me.lvwSumOfContents.Columns.Add(Me.Expt)
        Me.lvwSumOfContents.Columns.Add(Me.Recd)
        Me.lvwSumOfContents.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lvwSumOfContents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lvwSumOfContents.Location = New System.Drawing.Point(16, 100)
        Me.lvwSumOfContents.Name = "lvwSumOfContents"
        Me.lvwSumOfContents.Size = New System.Drawing.Size(208, 80)
        Me.lvwSumOfContents.TabIndex = 5
        Me.lvwSumOfContents.View = System.Windows.Forms.View.Details
        '
        'Item
        '
        Me.Item.Text = "Item Code"
        Me.Item.Width = 80
        '
        'Desc
        '
        Me.Desc.Text = "Desc"
        Me.Desc.Width = 180
        '
        'Expt
        '
        Me.Expt.Text = "Expt"
        Me.Expt.Width = 40
        '
        'Recd
        '
        Me.Recd.Text = "Recd"
        Me.Recd.Width = 40
        '
        'lblCode
        '
        Me.lblCode.Location = New System.Drawing.Point(144, 216)
        Me.lblCode.Name = "lblCode"
        Me.lblCode.Size = New System.Drawing.Size(32, 8)
        Me.lblCode.Visible = False
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.Image = CType(resources.GetObject("Btn_CalcPad_small1.Image"), System.Drawing.Image)
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(192, 192)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Finish1
        '
        Me.Btn_Finish1.Image = CType(resources.GetObject("Btn_Finish1.Image"), System.Drawing.Image)
        Me.Btn_Finish1.Location = New System.Drawing.Point(8, 232)
        Me.Btn_Finish1.Name = "Btn_Finish1"
        Me.Btn_Finish1.Size = New System.Drawing.Size(65, 24)
        Me.Btn_Finish1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(160, 232)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'txtBarcode
        '
        Me.txtBarcode.BackColor = System.Drawing.Color.White
        Me.txtBarcode.Location = New System.Drawing.Point(16, 192)
        Me.txtBarcode.MaxLength = 0
        Me.txtBarcode.Name = "txtBarcode"
        Me.txtBarcode.Size = New System.Drawing.Size(168, 21)
        Me.txtBarcode.TabIndex = 0
        '
        'frmBookInOrderSummaryOfContents
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.txtBarcode)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Btn_Finish1)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.lblCode)
        Me.Controls.Add(Me.lvwSumOfContents)
        Me.Controls.Add(Me.lblOrderNo)
        Me.Controls.Add(Me.lblScan)
        Me.Controls.Add(Me.lblSummary)
        Me.Controls.Add(Me.lblOrder)
        Me.Controls.Add(Me.lblSupplier)
        Me.Controls.Add(Me.lblBookIn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmBookInOrderSummaryOfContents"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        BCSessionMgr.GetInstance().QuitBeforeCommit()
    End Sub

    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        FreezeControls()
#If RF Then
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
        CalcPadSessionMgr.GetInstance().StartSession(lblCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
#ElseIf NRF Then
        Dim objSftKeyPad As New frmCalcPad(lblCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        If objSftKeyPad.ShowDialog() = Windows.Forms.DialogResult.OK Then
            'BCSessionMgr.GetInstance().SupplierNoEntry(lblCode.Text)
            BCSessionMgr.GetInstance().HandleScanData(lblCode.Text, BCType.ManualEntry)
        End If
#End If
        UnFreezeControls()
    End Sub

    Private Sub frmBookInOrderSummaryOfContents_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StartRead()
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARYOFCONTENTS
    End Sub

    Private Sub frmBookInOrderSummaryOfContents_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub

    Private Sub Btn_Finish1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Finish1.Click
        BCSessionMgr.GetInstance().FinishSession()
    End Sub
    Public Sub FreezeControls()
        Me.lvwSumOfContents.Enabled = False
        Me.Btn_CalcPad_small1.Enabled = False
        Me.Btn_Finish1.Enabled = False
        Me.Btn_Quit_small1.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.lvwSumOfContents.Enabled = True
        Me.Btn_CalcPad_small1.Enabled = True
        Me.Btn_Finish1.Enabled = True
        Me.Btn_Quit_small1.Enabled = True
    End Sub
End Class
