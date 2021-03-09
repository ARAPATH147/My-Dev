Public Class frmBookInCartonScanItem
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblBookIn As System.Windows.Forms.Label
    Friend WithEvents lblItem As System.Windows.Forms.Label
    Friend WithEvents lblScan As System.Windows.Forms.Label
    Friend WithEvents Btn_Finish1 As System.Windows.Forms.PictureBox
    Friend WithEvents btnNextCarton As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox

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
    Friend WithEvents txtProductCode As System.Windows.Forms.TextBox
    Friend WithEvents Btn_CalcPad_small1 As System.Windows.Forms.PictureBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBookInCartonScanItem))
        Me.lblBookIn = New System.Windows.Forms.Label
        Me.lblItem = New System.Windows.Forms.Label
        Me.lblScan = New System.Windows.Forms.Label
        Me.Btn_Finish1 = New System.Windows.Forms.PictureBox
        Me.btnNextCarton = New System.Windows.Forms.PictureBox
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.txtProductCode = New System.Windows.Forms.TextBox
        Me.Btn_CalcPad_small1 = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'lblBookIn
        '
        Me.lblBookIn.Location = New System.Drawing.Point(16, 16)
        Me.lblBookIn.Name = "lblBookIn"
        Me.lblBookIn.Size = New System.Drawing.Size(100, 20)
        Me.lblBookIn.Text = "Book in Carton"
        '
        'lblItem
        '
        Me.lblItem.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblItem.Location = New System.Drawing.Point(16, 40)
        Me.lblItem.Name = "lblItem"
        Me.lblItem.Size = New System.Drawing.Size(40, 20)
        Me.lblItem.Text = "Item"
        '
        'lblScan
        '
        Me.lblScan.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblScan.Location = New System.Drawing.Point(16, 72)
        Me.lblScan.Name = "lblScan"
        Me.lblScan.Size = New System.Drawing.Size(104, 32)
        Me.lblScan.Text = "Scan/ Enter Item"
        '
        'Btn_Finish1
        '
        Me.Btn_Finish1.Image = CType(resources.GetObject("Btn_Finish1.Image"), System.Drawing.Image)
        Me.Btn_Finish1.Location = New System.Drawing.Point(9, 200)
        Me.Btn_Finish1.Name = "Btn_Finish1"
        Me.Btn_Finish1.Size = New System.Drawing.Size(65, 24)
        Me.Btn_Finish1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'btnNextCarton
        '
        Me.btnNextCarton.Image = CType(resources.GetObject("btnNextCarton.Image"), System.Drawing.Image)
        Me.btnNextCarton.Location = New System.Drawing.Point(80, 200)
        Me.btnNextCarton.Name = "btnNextCarton"
        Me.btnNextCarton.Size = New System.Drawing.Size(91, 24)
        Me.btnNextCarton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(176, 200)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'txtProductCode
        '
        Me.txtProductCode.Location = New System.Drawing.Point(64, 40)
        Me.txtProductCode.Name = "txtProductCode"
        Me.txtProductCode.Size = New System.Drawing.Size(124, 21)
        Me.txtProductCode.TabIndex = 0
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.Image = CType(resources.GetObject("Btn_CalcPad_small1.Image"), System.Drawing.Image)
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(197, 39)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(20, 23)
        Me.Btn_CalcPad_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmBookInCartonScanItem
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.txtProductCode)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.btnNextCarton)
        Me.Controls.Add(Me.Btn_Finish1)
        Me.Controls.Add(Me.lblScan)
        Me.Controls.Add(Me.lblItem)
        Me.Controls.Add(Me.lblBookIn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmBookInCartonScanItem"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub frmBookInCartonScanItem_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StartRead()
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONITEMSCAN
    End Sub

    Private Sub frmBookInCartonScanItem_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub

    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        BCSessionMgr.GetInstance().QuitBeforeCommit()
    End Sub

    Private Sub Btn_Finish1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Finish1.Click
        FreezeControls()
        objAppContainer.strAuditCartonNotinFile = Nothing
#If RF Then
        BCSessionMgr.GetInstance().m_strBookIncartonShowMsg = "Y"
#End If
        BCSessionMgr.GetInstance().FinishSession()
#If RF Then
        'RECONNECT - 
        If Not objAppContainer.bCommFailure Then
            UnFreezeControls()
        End If

#ElseIf NRF Then
        If objAppContainer.bAutoLogOffProcess = False Then
            UnFreezeControls()
        End If
#End If

    End Sub
    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        FreezeControls()
#If RF Then
        objAppContainer.m_ModScreen = AppContainer.ModScreen.ITEMSCAN
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
        CalcPadSessionMgr.GetInstance().StartSession(txtProductCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
#ElseIf NRF Then
        Dim objSftKeyPad As New frmCalcPad(txtProductCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        If objSftKeyPad.ShowDialog() = Windows.Forms.DialogResult.OK Then
            BCSessionMgr.GetInstance().HandleScanData(txtProductCode.Text, BCType.ManualEntry)
        End If
#End If
        UnFreezeControls()
    End Sub

    Private Sub btnNextCarton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNextCarton.Click
        FreezeControls()
        BCSessionMgr.GetInstance().CartonCountIncrement()
        BCSessionMgr.GetInstance().DisplayBCScreen(BCSCREENS.CartonScan)
        UnFreezeControls()
    End Sub
    Public Sub FreezeControls()
        Me.Btn_CalcPad_small1.Enabled = False
        Me.Btn_Finish1.Enabled = False
        Me.btnNextCarton.Enabled = False
        Me.Btn_Quit_small1.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.Btn_CalcPad_small1.Enabled = True
        Me.Btn_Finish1.Enabled = True
        Me.btnNextCarton.Enabled = True
        Me.Btn_Quit_small1.Enabled = True

    End Sub
End Class
