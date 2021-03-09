Public Class frmBookInCartonScanCarton
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblBookIn As System.Windows.Forms.Label
    Friend WithEvents lblCarton As System.Windows.Forms.Label
    Friend WithEvents lblScan As System.Windows.Forms.Label
    Friend WithEvents lblSupplier As System.Windows.Forms.Label
    Friend WithEvents lblBookedIn As System.Windows.Forms.Label
    Friend WithEvents lblBookedInCount As System.Windows.Forms.Label
    Friend WithEvents lblOutstanding As System.Windows.Forms.Label
    Friend WithEvents lblOutstandingCount As System.Windows.Forms.Label
    Friend WithEvents Btn_Finish1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_ViewGreen As System.Windows.Forms.PictureBox
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBookInCartonScanCarton))
        Me.lblBookIn = New System.Windows.Forms.Label
        Me.lblCarton = New System.Windows.Forms.Label
        Me.lblScan = New System.Windows.Forms.Label
        Me.lblSupplier = New System.Windows.Forms.Label
        Me.lblBookedIn = New System.Windows.Forms.Label
        Me.lblBookedInCount = New System.Windows.Forms.Label
        Me.lblOutstanding = New System.Windows.Forms.Label
        Me.lblOutstandingCount = New System.Windows.Forms.Label
        Me.Btn_Finish1 = New System.Windows.Forms.PictureBox
        Me.Btn_ViewGreen = New System.Windows.Forms.PictureBox
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.txtProductCode = New System.Windows.Forms.TextBox
        Me.Btn_CalcPad_small1 = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'lblBookIn
        '
        Me.lblBookIn.Location = New System.Drawing.Point(24, 16)
        Me.lblBookIn.Name = "lblBookIn"
        Me.lblBookIn.Size = New System.Drawing.Size(100, 16)
        Me.lblBookIn.Text = "Book in Carton"
        '
        'lblCarton
        '
        Me.lblCarton.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblCarton.Location = New System.Drawing.Point(24, 56)
        Me.lblCarton.Name = "lblCarton"
        Me.lblCarton.Size = New System.Drawing.Size(48, 16)
        Me.lblCarton.Text = "Carton"
        '
        'lblScan
        '
        Me.lblScan.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblScan.Location = New System.Drawing.Point(24, 96)
        Me.lblScan.Name = "lblScan"
        Me.lblScan.Size = New System.Drawing.Size(100, 32)
        Me.lblScan.Text = "Scan/ Enter Carton Barcode"
        '
        'lblSupplier
        '
        Me.lblSupplier.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSupplier.Location = New System.Drawing.Point(24, 128)
        Me.lblSupplier.Name = "lblSupplier"
        Me.lblSupplier.Size = New System.Drawing.Size(100, 16)
        Me.lblSupplier.Text = "Sup"
        '
        'lblBookedIn
        '
        Me.lblBookedIn.Location = New System.Drawing.Point(24, 144)
        Me.lblBookedIn.Name = "lblBookedIn"
        Me.lblBookedIn.Size = New System.Drawing.Size(72, 16)
        Me.lblBookedIn.Text = "Booked in"
        '
        'lblBookedInCount
        '
        Me.lblBookedInCount.Location = New System.Drawing.Point(136, 144)
        Me.lblBookedInCount.Name = "lblBookedInCount"
        Me.lblBookedInCount.Size = New System.Drawing.Size(72, 16)
        Me.lblBookedInCount.Text = "Label6"
        '
        'lblOutstanding
        '
        Me.lblOutstanding.Location = New System.Drawing.Point(24, 160)
        Me.lblOutstanding.Name = "lblOutstanding"
        Me.lblOutstanding.Size = New System.Drawing.Size(80, 16)
        Me.lblOutstanding.Text = "Outstanding"
        '
        'lblOutstandingCount
        '
        Me.lblOutstandingCount.Location = New System.Drawing.Point(136, 160)
        Me.lblOutstandingCount.Name = "lblOutstandingCount"
        Me.lblOutstandingCount.Size = New System.Drawing.Size(72, 16)
        Me.lblOutstandingCount.Text = "Label8"
        '
        'Btn_Finish1
        '
        Me.Btn_Finish1.Image = CType(resources.GetObject("Btn_Finish1.Image"), System.Drawing.Image)
        Me.Btn_Finish1.Location = New System.Drawing.Point(16, 232)
        Me.Btn_Finish1.Name = "Btn_Finish1"
        Me.Btn_Finish1.Size = New System.Drawing.Size(65, 24)
        Me.Btn_Finish1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_ViewGreen
        '
        Me.Btn_ViewGreen.Image = CType(resources.GetObject("Btn_ViewGreen.Image"), System.Drawing.Image)
        Me.Btn_ViewGreen.Location = New System.Drawing.Point(104, 232)
        Me.Btn_ViewGreen.Name = "Btn_ViewGreen"
        Me.Btn_ViewGreen.Size = New System.Drawing.Size(50, 24)
        Me.Btn_ViewGreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(176, 232)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'txtProductCode
        '
        Me.txtProductCode.Location = New System.Drawing.Point(72, 56)
        Me.txtProductCode.Name = "txtProductCode"
        Me.txtProductCode.Size = New System.Drawing.Size(120, 21)
        Me.txtProductCode.TabIndex = 0
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.Image = CType(resources.GetObject("Btn_CalcPad_small1.Image"), System.Drawing.Image)
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(200, 56)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(20, 23)
        Me.Btn_CalcPad_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmBookInCartonScanCarton
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.txtProductCode)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Btn_ViewGreen)
        Me.Controls.Add(Me.Btn_Finish1)
        Me.Controls.Add(Me.lblOutstandingCount)
        Me.Controls.Add(Me.lblOutstanding)
        Me.Controls.Add(Me.lblBookedInCount)
        Me.Controls.Add(Me.lblBookedIn)
        Me.Controls.Add(Me.lblSupplier)
        Me.Controls.Add(Me.lblScan)
        Me.Controls.Add(Me.lblCarton)
        Me.Controls.Add(Me.lblBookIn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmBookInCartonScanCarton"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
   
    Private Sub Btn_Finish1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Finish1.Click
        FreezeControls()

        BCSessionMgr.GetInstance().m_strBookIncartonShowMsg = "Y"

        BCSessionMgr.GetInstance().FinishSession()
#If RF Then
        If Not objAppContainer.bCommFailure Then
            UnFreezeControls()
        End If
#ElseIf NRF Then
        If objAppContainer.bAutoLogOffProcess = False Then
            UnFreezeControls()
        End If
#End If
       

    End Sub

    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click

        BCSessionMgr.GetInstance().QuitBeforeCommit()
    End Sub
    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        FreezeControls()
#If RF Then
        objAppContainer.m_ModScreen = AppContainer.ModScreen.CARTONSCAN
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

    Private Sub frmBookInCartonScanCarton_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StartRead()
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.BOOKINCARTON
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONSCAN
    End Sub

    Private Sub frmBookInCartonScanCarton_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub

    Private Sub Btn_ViewGreen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_ViewGreen.Click
        FreezeControls()
#If RF Then
        objAppContainer.objPrevMod = AppContainer.ACTIVEMODULE.BOOKINCARTON
#End If
        VCSessionManager.GetInstance.StartSession()
        UnFreezeControls()
        'objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.BOOKINCARTON
    End Sub
    Public Sub FreezeControls()
        Me.Btn_CalcPad_small1.Enabled = False
        Me.Btn_Finish1.Enabled = False
        Me.Btn_ViewGreen.Enabled = False
        Me.Btn_Quit_small1.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.Btn_CalcPad_small1.Enabled = True
        Me.Btn_Finish1.Enabled = True
        Me.Btn_ViewGreen.Enabled = True
        Me.Btn_Quit_small1.Enabled = True
    End Sub
End Class
