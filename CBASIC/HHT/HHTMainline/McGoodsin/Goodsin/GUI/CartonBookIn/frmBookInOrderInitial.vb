Public Class frmBookInOrderInitial
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        Me.lvwOrders.Activation = ItemActivation.OneClick
        Me.lvwOrders.Font = New System.Drawing.Font("Courier New", 9.0!, FontStyle.Regular)
        'Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents lblBookIn As System.Windows.Forms.Label
    Friend WithEvents lblSupplier As System.Windows.Forms.Label
    Friend WithEvents lvwOrders As System.Windows.Forms.ListView
    Friend WithEvents lblSelect As System.Windows.Forms.Label
    Friend WithEvents lblMsg As System.Windows.Forms.Label
    Friend WithEvents btnNoOrderNo As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents OrderNumber As System.Windows.Forms.ColumnHeader
    Friend WithEvents Expt As System.Windows.Forms.ColumnHeader
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBookInOrderInitial))
        Me.lblBookIn = New System.Windows.Forms.Label
        Me.lblSupplier = New System.Windows.Forms.Label
        Me.lvwOrders = New System.Windows.Forms.ListView
        Me.OrderNumber = New System.Windows.Forms.ColumnHeader
        Me.Expt = New System.Windows.Forms.ColumnHeader
        Me.lblSelect = New System.Windows.Forms.Label
        Me.lblMsg = New System.Windows.Forms.Label
        Me.btnNoOrderNo = New System.Windows.Forms.PictureBox
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
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
        Me.lblSupplier.Size = New System.Drawing.Size(143, 17)
        Me.lblSupplier.Text = "Fuji"
        '
        'lvwOrders
        '
        Me.lvwOrders.Columns.Add(Me.OrderNumber)
        Me.lvwOrders.Columns.Add(Me.Expt)
        Me.lvwOrders.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lvwOrders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lvwOrders.Location = New System.Drawing.Point(16, 52)
        Me.lvwOrders.Name = "lvwOrders"
        Me.lvwOrders.Size = New System.Drawing.Size(208, 88)
        Me.lvwOrders.TabIndex = 4
        Me.lvwOrders.View = System.Windows.Forms.View.Details
        '
        'OrderNumber
        '
        Me.OrderNumber.Text = "Order Number"
        Me.OrderNumber.Width = 125
        '
        'Expt
        '
        Me.Expt.Text = "Expt"
        Me.Expt.Width = 70
        '
        'lblSelect
        '
        Me.lblSelect.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelect.Location = New System.Drawing.Point(16, 152)
        Me.lblSelect.Name = "lblSelect"
        Me.lblSelect.Size = New System.Drawing.Size(152, 16)
        Me.lblSelect.Text = "Select Order Number"
        '
        'lblMsg
        '
        Me.lblMsg.Location = New System.Drawing.Point(16, 176)
        Me.lblMsg.Name = "lblMsg"
        Me.lblMsg.Size = New System.Drawing.Size(200, 48)
        Me.lblMsg.Text = "If order number not present 48 hours after expected date then use no order number" & _
            ""
        '
        'btnNoOrderNo
        '
        Me.btnNoOrderNo.Image = CType(resources.GetObject("btnNoOrderNo.Image"), System.Drawing.Image)
        Me.btnNoOrderNo.Location = New System.Drawing.Point(16, 232)
        Me.btnNoOrderNo.Name = "btnNoOrderNo"
        Me.btnNoOrderNo.Size = New System.Drawing.Size(125, 24)
        Me.btnNoOrderNo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(168, 232)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmBookInOrderInitial
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.btnNoOrderNo)
        Me.Controls.Add(Me.lblMsg)
        Me.Controls.Add(Me.lblSelect)
        Me.Controls.Add(Me.lvwOrders)
        Me.Controls.Add(Me.lblSupplier)
        Me.Controls.Add(Me.lblBookIn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmBookInOrderInitial"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        BCSessionMgr.GetInstance().QuitSession()
    End Sub

    Private Sub lvwOrders_ItemActivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvwOrders.SelectedIndexChanged
        If BCSessionMgr.GetInstance().OrderSelection() Then
#If RF Then
            If Not objAppContainer.bCommFailure And Not objAppContainer.bReconnectSuccess Then
                Me.Hide()
                Me.Dispose()
            End If
            objAppContainer.bReconnectSuccess = False
#ElseIf NRF Then
        Me.Hide()
        Me.Dispose()
#End If
        End If

    End Sub
    Private Sub frmBookInOrderInitial_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated

        BCReader.GetInstance().StopRead()
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERINITIAL

        
    End Sub
    Private Sub btnNoOrderNo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNoOrderNo.Click
        FreezeControls()
        BCSessionMgr.GetInstance().DisplayBCScreen(BCSCREENS.BookInItemforNoOrder)
        UnFreezeControls()
    End Sub
    Public Sub FreezeControls()
        Me.lvwOrders.Enabled = False
        Me.Btn_Quit_small1.Enabled = False
        Me.btnNoOrderNo.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.lvwOrders.Enabled = True
        Me.Btn_Quit_small1.Enabled = True
        Me.btnNoOrderNo.Enabled = True
    End Sub
End Class
