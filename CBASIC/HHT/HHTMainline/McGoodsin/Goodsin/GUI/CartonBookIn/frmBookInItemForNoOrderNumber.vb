Public Class frmBookInItemForNoOrderNumber
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblbookIn As System.Windows.Forms.Label
    Friend WithEvents lblSupplier As System.Windows.Forms.Label
    Friend WithEvents lblNoOrderNo As System.Windows.Forms.Label
    Friend WithEvents lblScan As System.Windows.Forms.Label
    Friend WithEvents lblCode As System.Windows.Forms.Label

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents Btn_Finish1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_CalcPad_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents txtBarcode As System.Windows.Forms.TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBookInItemForNoOrderNumber))
        Me.lblbookIn = New System.Windows.Forms.Label
        Me.lblSupplier = New System.Windows.Forms.Label
        Me.lblNoOrderNo = New System.Windows.Forms.Label
        Me.lblScan = New System.Windows.Forms.Label
        Me.lblCode = New System.Windows.Forms.Label
        Me.Btn_Finish1 = New System.Windows.Forms.PictureBox
        Me.Btn_CalcPad_small1 = New System.Windows.Forms.PictureBox
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.txtBarcode = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'lblbookIn
        '
        Me.lblbookIn.Location = New System.Drawing.Point(24, 24)
        Me.lblbookIn.Name = "lblbookIn"
        Me.lblbookIn.Size = New System.Drawing.Size(104, 16)
        Me.lblbookIn.Text = "Book In Order"
        '
        'lblSupplier
        '
        Me.lblSupplier.Location = New System.Drawing.Point(24, 48)
        Me.lblSupplier.Name = "lblSupplier"
        Me.lblSupplier.Size = New System.Drawing.Size(112, 16)
        Me.lblSupplier.Text = "Fuji"
        '
        'lblNoOrderNo
        '
        Me.lblNoOrderNo.Location = New System.Drawing.Point(24, 80)
        Me.lblNoOrderNo.Name = "lblNoOrderNo"
        Me.lblNoOrderNo.Size = New System.Drawing.Size(112, 16)
        Me.lblNoOrderNo.Text = "No Order Number"
        '
        'lblScan
        '
        Me.lblScan.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblScan.Location = New System.Drawing.Point(24, 168)
        Me.lblScan.Name = "lblScan"
        Me.lblScan.Size = New System.Drawing.Size(128, 20)
        Me.lblScan.Text = "Scan / Enter item"
        '
        'lblCode
        '
        Me.lblCode.Location = New System.Drawing.Point(24, 192)
        Me.lblCode.Name = "lblCode"
        Me.lblCode.Size = New System.Drawing.Size(100, 8)
        Me.lblCode.Visible = False
        '
        'Btn_Finish1
        '
        Me.Btn_Finish1.Image = CType(resources.GetObject("Btn_Finish1.Image"), System.Drawing.Image)
        Me.Btn_Finish1.Location = New System.Drawing.Point(16, 216)
        Me.Btn_Finish1.Name = "Btn_Finish1"
        Me.Btn_Finish1.Size = New System.Drawing.Size(65, 24)
        Me.Btn_Finish1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.Image = CType(resources.GetObject("Btn_CalcPad_small1.Image"), System.Drawing.Image)
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(176, 136)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(20, 23)
        Me.Btn_CalcPad_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(168, 216)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'txtBarcode
        '
        Me.txtBarcode.BackColor = System.Drawing.Color.White
        Me.txtBarcode.Location = New System.Drawing.Point(24, 136)
        Me.txtBarcode.MaxLength = 0
        Me.txtBarcode.Name = "txtBarcode"
        Me.txtBarcode.Size = New System.Drawing.Size(144, 21)
        Me.txtBarcode.TabIndex = 0
        '
        'frmBookInItemForNoOrderNumber
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.txtBarcode)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.Btn_Finish1)
        Me.Controls.Add(Me.lblCode)
        Me.Controls.Add(Me.lblScan)
        Me.Controls.Add(Me.lblNoOrderNo)
        Me.Controls.Add(Me.lblSupplier)
        Me.Controls.Add(Me.lblbookIn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmBookInItemForNoOrderNumber"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub frmBookInItemForNoOrderNumber_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StartRead()
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINITEMFORNOORDERNUMBER
    End Sub

    Private Sub frmBookInItemForNoOrderNumber_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub

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
            BCSessionMgr.GetInstance().HandleScanData(lblCode.Text, BCType.ManualEntry)
        End If
#End If

        UnFreezeControls()

    End Sub

    Private Sub Btn_Finish1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Finish1.Click
        FreezeCOntrols()
        BCSessionMgr.GetInstance().FinishSession()
        If objAppContainer.bAutoLogOffProcess = False Then
            UnFreezeControls()
        End If
    End Sub

    Public Sub FreezeControls()
        Me.Btn_CalcPad_small1.Enabled = False
        Me.Btn_Finish1.Enabled = False
        Me.Btn_Quit_small1.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.Btn_CalcPad_small1.Enabled = True
        Me.Btn_Finish1.Enabled = True
        Me.Btn_Quit_small1.Enabled = True
    End Sub
End Class
