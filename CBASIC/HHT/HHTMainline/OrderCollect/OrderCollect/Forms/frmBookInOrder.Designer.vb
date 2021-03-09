<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmBookInOrder
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBookInOrder))
        Me.btnCalcPad1 = New System.Windows.Forms.PictureBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.lblCode = New System.Windows.Forms.TextBox
        Me.Btn_Finish1 = New System.Windows.Forms.PictureBox
        Me.lblSelectParcel = New System.Windows.Forms.Label
        Me.lblOutstanding = New System.Windows.Forms.Label
        Me.lblBookedIn = New System.Windows.Forms.Label
        Me.pbHelp = New System.Windows.Forms.PictureBox
        Me.lblBookedInCount = New System.Windows.Forms.Label
        Me.lblOutStandingCount = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'btnCalcPad1
        '
        Me.btnCalcPad1.Image = CType(resources.GetObject("btnCalcPad1.Image"), System.Drawing.Image)
        Me.btnCalcPad1.Location = New System.Drawing.Point(198, 40)
        Me.btnCalcPad1.Name = "btnCalcPad1"
        Me.btnCalcPad1.Size = New System.Drawing.Size(20, 23)
        Me.btnCalcPad1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(17, 68)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(125, 20)
        Me.Label2.Text = "Scan / enter parcel"
        '
        'lblCode
        '
        Me.lblCode.Location = New System.Drawing.Point(17, 41)
        Me.lblCode.Name = "lblCode"
        Me.lblCode.Size = New System.Drawing.Size(174, 21)
        Me.lblCode.TabIndex = 47
        '
        'Btn_Finish1
        '
        Me.Btn_Finish1.Image = CType(resources.GetObject("Btn_Finish1.Image"), System.Drawing.Image)
        Me.Btn_Finish1.Location = New System.Drawing.Point(154, 231)
        Me.Btn_Finish1.Name = "Btn_Finish1"
        Me.Btn_Finish1.Size = New System.Drawing.Size(65, 24)
        Me.Btn_Finish1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblSelectParcel
        '
        Me.lblSelectParcel.Location = New System.Drawing.Point(17, 12)
        Me.lblSelectParcel.Name = "lblSelectParcel"
        Me.lblSelectParcel.Size = New System.Drawing.Size(201, 16)
        Me.lblSelectParcel.Text = "Book In Order"
        '
        'lblOutstanding
        '
        Me.lblOutstanding.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblOutstanding.Location = New System.Drawing.Point(17, 190)
        Me.lblOutstanding.Name = "lblOutstanding"
        Me.lblOutstanding.Size = New System.Drawing.Size(86, 20)
        Me.lblOutstanding.Text = "Outstanding"
        '
        'lblBookedIn
        '
        Me.lblBookedIn.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblBookedIn.Location = New System.Drawing.Point(17, 160)
        Me.lblBookedIn.Name = "lblBookedIn"
        Me.lblBookedIn.Size = New System.Drawing.Size(86, 20)
        Me.lblBookedIn.Text = "Booked In"
        '
        'pbHelp
        '
        Me.pbHelp.Image = CType(resources.GetObject("pbHelp.Image"), System.Drawing.Image)
        Me.pbHelp.Location = New System.Drawing.Point(192, 3)
        Me.pbHelp.Name = "pbHelp"
        Me.pbHelp.Size = New System.Drawing.Size(32, 32)
        Me.pbHelp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblBookedInCount
        '
        Me.lblBookedInCount.Location = New System.Drawing.Point(110, 159)
        Me.lblBookedInCount.Name = "lblBookedInCount"
        Me.lblBookedInCount.Size = New System.Drawing.Size(43, 20)
        Me.lblBookedInCount.Text = "0"
        '
        'lblOutStandingCount
        '
        Me.lblOutStandingCount.Location = New System.Drawing.Point(109, 190)
        Me.lblOutStandingCount.Name = "lblOutStandingCount"
        Me.lblOutStandingCount.Size = New System.Drawing.Size(33, 20)
        Me.lblOutStandingCount.Text = "0"
        '
        'frmBookInOrder
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblOutStandingCount)
        Me.Controls.Add(Me.lblBookedInCount)
        Me.Controls.Add(Me.pbHelp)
        Me.Controls.Add(Me.lblOutstanding)
        Me.Controls.Add(Me.lblBookedIn)
        Me.Controls.Add(Me.lblSelectParcel)
        Me.Controls.Add(Me.Btn_Finish1)
        Me.Controls.Add(Me.lblCode)
        Me.Controls.Add(Me.btnCalcPad1)
        Me.Controls.Add(Me.Label2)
        Me.Name = "frmBookInOrder"
        Me.Text = "Order && Collect"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnCalcPad1 As System.Windows.Forms.PictureBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblCode As System.Windows.Forms.TextBox
    Friend WithEvents Btn_Finish1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblSelectParcel As System.Windows.Forms.Label
    Friend WithEvents lblOutstanding As System.Windows.Forms.Label
    Friend WithEvents lblBookedIn As System.Windows.Forms.Label
    Friend WithEvents pbHelp As System.Windows.Forms.PictureBox
    Friend WithEvents lblBookedInCount As System.Windows.Forms.Label
    Friend WithEvents lblOutStandingCount As System.Windows.Forms.Label
End Class
