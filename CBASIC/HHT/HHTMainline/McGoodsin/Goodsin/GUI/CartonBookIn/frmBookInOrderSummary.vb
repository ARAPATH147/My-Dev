Public Class frmBookInOrderSummary
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblBookIn As System.Windows.Forms.Label
    Friend WithEvents lblSupplier As System.Windows.Forms.Label

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
    Friend WithEvents lblOrders As System.Windows.Forms.Label
    Friend WithEvents lblOrderCount As System.Windows.Forms.Label
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblSummary As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBookInOrderSummary))
        Me.lblBookIn = New System.Windows.Forms.Label
        Me.lblSupplier = New System.Windows.Forms.Label
        Me.lblOrders = New System.Windows.Forms.Label
        Me.lblOrderCount = New System.Windows.Forms.Label
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.lblSummary = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblBookIn
        '
        Me.lblBookIn.Location = New System.Drawing.Point(16, 16)
        Me.lblBookIn.Name = "lblBookIn"
        Me.lblBookIn.Size = New System.Drawing.Size(112, 16)
        Me.lblBookIn.Text = "Book In Order"
        '
        'lblSupplier
        '
        Me.lblSupplier.Location = New System.Drawing.Point(16, 40)
        Me.lblSupplier.Name = "lblSupplier"
        Me.lblSupplier.Size = New System.Drawing.Size(145, 16)
        Me.lblSupplier.Text = "Fuji"
        '
        'lblOrders
        '
        Me.lblOrders.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblOrders.Location = New System.Drawing.Point(16, 120)
        Me.lblOrders.Name = "lblOrders"
        Me.lblOrders.Size = New System.Drawing.Size(120, 16)
        Me.lblOrders.Text = "Orders Booked In"
        '
        'lblOrderCount
        '
        Me.lblOrderCount.Location = New System.Drawing.Point(160, 120)
        Me.lblOrderCount.Name = "lblOrderCount"
        Me.lblOrderCount.Size = New System.Drawing.Size(32, 20)
        Me.lblOrderCount.Text = "0"
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(160, 216)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblSummary
        '
        Me.lblSummary.Location = New System.Drawing.Point(16, 64)
        Me.lblSummary.Name = "lblSummary"
        Me.lblSummary.Size = New System.Drawing.Size(100, 16)
        '
        'frmBookInOrderSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblSummary)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.lblOrderCount)
        Me.Controls.Add(Me.lblOrders)
        Me.Controls.Add(Me.lblSupplier)
        Me.Controls.Add(Me.lblBookIn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmBookInOrderSummary"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub frmBookInOrderSummary_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StopRead()
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARY
    End Sub
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
#If RF Then
         objAppContainer.m_ModScreen = AppContainer.ModScreen.BCITEMFINISH
#End If
        BCSessionMgr.GetInstance().EndSession(AppContainer.IsAbort.Yes)
    End Sub
    Public Sub FreezeControls()
        Me.Btn_Quit_small1.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.Btn_Quit_small1.Enabled = True
    End Sub
End Class
