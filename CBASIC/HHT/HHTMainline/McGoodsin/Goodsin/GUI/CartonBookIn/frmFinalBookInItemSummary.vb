Public Class frmFinalBookInItemSummary
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblBookIn As System.Windows.Forms.Label
    Friend WithEvents lblSupplier As System.Windows.Forms.Label
    Friend WithEvents lblOrder As System.Windows.Forms.Label
    Friend WithEvents lblDiscr As System.Windows.Forms.Label
    Friend WithEvents lblMsg As System.Windows.Forms.Label
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        Me.lvwDiscrepancies.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular)
        Me.lvwDiscrepancies.Activation = ItemActivation.OneClick
        'Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents lvwDiscrepancies As System.Windows.Forms.ListView
    Friend WithEvents lblMsg2 As System.Windows.Forms.Label
    Friend WithEvents Item As System.Windows.Forms.ColumnHeader
    Friend WithEvents Desc As System.Windows.Forms.ColumnHeader
    Friend WithEvents Expt As System.Windows.Forms.ColumnHeader
    Friend WithEvents Recd As System.Windows.Forms.ColumnHeader
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFinalBookInItemSummary))
        Me.lblBookIn = New System.Windows.Forms.Label
        Me.lblSupplier = New System.Windows.Forms.Label
        Me.lblOrder = New System.Windows.Forms.Label
        Me.lblDiscr = New System.Windows.Forms.Label
        Me.lblMsg = New System.Windows.Forms.Label
        Me.lvwDiscrepancies = New System.Windows.Forms.ListView
        Me.Item = New System.Windows.Forms.ColumnHeader
        Me.Desc = New System.Windows.Forms.ColumnHeader
        Me.Expt = New System.Windows.Forms.ColumnHeader
        Me.Recd = New System.Windows.Forms.ColumnHeader
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.lblMsg2 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblBookIn
        '
        Me.lblBookIn.Location = New System.Drawing.Point(24, 16)
        Me.lblBookIn.Name = "lblBookIn"
        Me.lblBookIn.Size = New System.Drawing.Size(100, 16)
        Me.lblBookIn.Text = "Book In Order"
        '
        'lblSupplier
        '
        Me.lblSupplier.Location = New System.Drawing.Point(24, 40)
        Me.lblSupplier.Name = "lblSupplier"
        Me.lblSupplier.Size = New System.Drawing.Size(100, 16)
        Me.lblSupplier.Text = "Fuji"
        '
        'lblOrder
        '
        Me.lblOrder.Location = New System.Drawing.Point(24, 58)
        Me.lblOrder.Name = "lblOrder"
        Me.lblOrder.Size = New System.Drawing.Size(100, 16)
        Me.lblOrder.Text = "Order 123456"
        '
        'lblDiscr
        '
        Me.lblDiscr.Location = New System.Drawing.Point(24, 78)
        Me.lblDiscr.Name = "lblDiscr"
        Me.lblDiscr.Size = New System.Drawing.Size(100, 16)
        Me.lblDiscr.Text = "Discrepancies"
        '
        'lblMsg
        '
        Me.lblMsg.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg.Location = New System.Drawing.Point(16, 196)
        Me.lblMsg.Name = "lblMsg"
        Me.lblMsg.Size = New System.Drawing.Size(192, 32)
        Me.lblMsg.Text = "Collect and file Confirmation of Receipts Report"
        '
        'lvwDiscrepancies
        '
        Me.lvwDiscrepancies.Columns.Add(Me.Item)
        Me.lvwDiscrepancies.Columns.Add(Me.Desc)
        Me.lvwDiscrepancies.Columns.Add(Me.Expt)
        Me.lvwDiscrepancies.Columns.Add(Me.Recd)
        Me.lvwDiscrepancies.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lvwDiscrepancies.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lvwDiscrepancies.Location = New System.Drawing.Point(16, 100)
        Me.lvwDiscrepancies.Name = "lvwDiscrepancies"
        Me.lvwDiscrepancies.Size = New System.Drawing.Size(208, 90)
        Me.lvwDiscrepancies.TabIndex = 2
        Me.lvwDiscrepancies.View = System.Windows.Forms.View.Details
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
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(168, 232)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblMsg2
        '
        Me.lblMsg2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg2.Location = New System.Drawing.Point(24, 120)
        Me.lblMsg2.Name = "lblMsg2"
        Me.lblMsg2.Size = New System.Drawing.Size(160, 48)
        Me.lblMsg2.Text = "Collect and file Confirmation of Receipts Report"
        '
        'frmFinalBookInItemSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblMsg2)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.lvwDiscrepancies)
        Me.Controls.Add(Me.lblMsg)
        Me.Controls.Add(Me.lblDiscr)
        Me.Controls.Add(Me.lblOrder)
        Me.Controls.Add(Me.lblSupplier)
        Me.Controls.Add(Me.lblBookIn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmFinalBookInItemSummary"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub frmFinalBookInItemSummary_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StopRead()
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.FINALBOOKINITEMSUMMARY
    End Sub
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click

#If NRF Then
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M85"), "Alert", MessageBoxButtons.OK, _
                                      MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)

            BCSessionMgr.GetInstance().EndSession(AppContainer.IsAbort.No)
#ElseIf RF Then
        objAppContainer.m_ModScreen = AppContainer.ModScreen.BCITEMFINISH
        BCSessionMgr.GetInstance().EndSession(AppContainer.IsAbort.No)
#End If

    End Sub
End Class
