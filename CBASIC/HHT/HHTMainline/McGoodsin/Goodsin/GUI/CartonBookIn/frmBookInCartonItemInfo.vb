Public Class frmBookInCartonItemInfo
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblBookin As System.Windows.Forms.Label
    Friend WithEvents lblSupplier As System.Windows.Forms.Label
    Friend WithEvents lblItem2 As System.Windows.Forms.Label
    Friend WithEvents lblItem3 As System.Windows.Forms.Label
    Friend WithEvents lblItemDesc As System.Windows.Forms.Label
    Friend WithEvents lblBootscode As System.Windows.Forms.Label
    Friend WithEvents lblEAN As System.Windows.Forms.Label
    Friend WithEvents lblQty As System.Windows.Forms.Label
    Friend WithEvents Btn_CalcPad_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Next_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblEnter As System.Windows.Forms.Label

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
    Friend WithEvents lblOrder As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBookInCartonItemInfo))
        Me.lblBookin = New System.Windows.Forms.Label
        Me.lblSupplier = New System.Windows.Forms.Label
        Me.lblItem2 = New System.Windows.Forms.Label
        Me.lblItem3 = New System.Windows.Forms.Label
        Me.lblItemDesc = New System.Windows.Forms.Label
        Me.lblBootscode = New System.Windows.Forms.Label
        Me.lblEAN = New System.Windows.Forms.Label
        Me.lblQty = New System.Windows.Forms.Label
        Me.Btn_CalcPad_small1 = New System.Windows.Forms.PictureBox
        Me.Btn_Next_small1 = New System.Windows.Forms.PictureBox
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.lblEnter = New System.Windows.Forms.Label
        Me.lblOrder = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblBookin
        '
        Me.lblBookin.Location = New System.Drawing.Point(24, 10)
        Me.lblBookin.Name = "lblBookin"
        Me.lblBookin.Size = New System.Drawing.Size(100, 16)
        Me.lblBookin.Text = "Book In Carton"
        '
        'lblSupplier
        '
        Me.lblSupplier.Location = New System.Drawing.Point(24, 26)
        Me.lblSupplier.Name = "lblSupplier"
        Me.lblSupplier.Size = New System.Drawing.Size(100, 16)
        Me.lblSupplier.Text = "Clarins"
        '
        'lblItem2
        '
        Me.lblItem2.Location = New System.Drawing.Point(24, 87)
        Me.lblItem2.Name = "lblItem2"
        Me.lblItem2.Size = New System.Drawing.Size(200, 16)
        Me.lblItem2.Text = "Desc2"
        '
        'lblItem3
        '
        Me.lblItem3.Location = New System.Drawing.Point(24, 110)
        Me.lblItem3.Name = "lblItem3"
        Me.lblItem3.Size = New System.Drawing.Size(200, 16)
        Me.lblItem3.Text = "Desc3"
        '
        'lblItemDesc
        '
        Me.lblItemDesc.Location = New System.Drawing.Point(24, 64)
        Me.lblItemDesc.Name = "lblItemDesc"
        Me.lblItemDesc.Size = New System.Drawing.Size(200, 16)
        Me.lblItemDesc.Text = "Desc1"
        '
        'lblBootscode
        '
        Me.lblBootscode.Location = New System.Drawing.Point(24, 136)
        Me.lblBootscode.Name = "lblBootscode"
        Me.lblBootscode.Size = New System.Drawing.Size(100, 20)
        Me.lblBootscode.Text = "11-40-930"
        '
        'lblEAN
        '
        Me.lblEAN.Location = New System.Drawing.Point(24, 160)
        Me.lblEAN.Name = "lblEAN"
        Me.lblEAN.Size = New System.Drawing.Size(120, 16)
        Me.lblEAN.Text = "5-014697-021021"
        '
        'lblQty
        '
        Me.lblQty.Location = New System.Drawing.Point(136, 184)
        Me.lblQty.Name = "lblQty"
        Me.lblQty.Size = New System.Drawing.Size(40, 20)
        Me.lblQty.Text = "1"
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.Image = CType(resources.GetObject("Btn_CalcPad_small1.Image"), System.Drawing.Image)
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(182, 181)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(20, 23)
        Me.Btn_CalcPad_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Next_small1
        '
        Me.Btn_Next_small1.Image = CType(resources.GetObject("Btn_Next_small1.Image"), System.Drawing.Image)
        Me.Btn_Next_small1.Location = New System.Drawing.Point(24, 224)
        Me.Btn_Next_small1.Name = "Btn_Next_small1"
        Me.Btn_Next_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Next_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(174, 224)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblEnter
        '
        Me.lblEnter.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblEnter.Location = New System.Drawing.Point(24, 184)
        Me.lblEnter.Name = "lblEnter"
        Me.lblEnter.Size = New System.Drawing.Size(100, 20)
        Me.lblEnter.Text = "Enter Quantity"
        '
        'lblOrder
        '
        Me.lblOrder.Location = New System.Drawing.Point(24, 42)
        Me.lblOrder.Name = "lblOrder"
        Me.lblOrder.Size = New System.Drawing.Size(100, 16)
        '
        'frmBookInCartonItemInfo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblOrder)
        Me.Controls.Add(Me.lblEnter)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Btn_Next_small1)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.lblQty)
        Me.Controls.Add(Me.lblEAN)
        Me.Controls.Add(Me.lblBootscode)
        Me.Controls.Add(Me.lblItemDesc)
        Me.Controls.Add(Me.lblItem3)
        Me.Controls.Add(Me.lblItem2)
        Me.Controls.Add(Me.lblSupplier)
        Me.Controls.Add(Me.lblBookin)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmBookInCartonItemInfo"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        BCSessionMgr.GetInstance().QuitBeforeCommit()
    End Sub

    Private Sub frmBookInCartonItemInfo_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StopRead()
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONITEMINFO
    End Sub

    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click

        FreezeControls()
#If RF Then
  objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
        CalcPadSessionMgr.GetInstance().StartSession(lblQty, CalcPadSessionMgr.EntryTypeEnum.Quantity)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
#ElseIf NRF Then
        Dim iQty As Integer = CInt(lblQty.Text)
        Dim objSftKeyPad As New frmCalcPad(lblQty, CalcPadSessionMgr.EntryTypeEnum.Quantity)
        'If objSftKeyPad.ShowDialog() = Windows.Forms.DialogResult.OK Then
        '    'Checks whether zero is entered
        '    If Not objAppContainer.objHelper.ValidateZeroQty(CInt(Me.lblQty.Text)) Then
        '        MessageBox.Show(MessageManager.GetInstance().GetMessage("M1"), "Alert ", MessageBoxButtons.OK, _
        '                                                                           MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        '        Me.lblQty.Text = iQty.ToString()
        '        Me.Refresh()
        '        'Check whether the qty entered is higher than expected qty
        '    ElseIf Not BCSessionMgr.GetInstance().CheckExpectedOrderQty(iQty) Then
        '        MessageBox.Show(MessageManager.GetInstance().GetMessage("M68"), "Alert ", MessageBoxButtons.OK, _
        '                                                                           MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)

        '        'checks whether the quantity has changed
        '    ElseIf Not iQty = CInt(lblQty.Text) Then
        '        BCSessionMgr.GetInstance().SetItemQty(iQty)
        '    End If
        'End If
        objSftKeyPad.ShowDialog()
#End If

        UnFreezeControls()


    End Sub

    Private Sub Btn_Next_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Next_small1.Click
        FreezeControls()
        BCSessionMgr.GetInstance().SetItemQty(lblQty.Text)

        UnFreezeControls()
    End Sub
    Public Sub FreezeControls()
        Me.Btn_CalcPad_small1.Enabled = False
        Me.Btn_Next_small1.Enabled = False
        Me.Btn_Quit_small1.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.Btn_CalcPad_small1.Enabled = True
        Me.Btn_Next_small1.Enabled = True
        Me.Btn_Quit_small1.Enabled = True

    End Sub
End Class
