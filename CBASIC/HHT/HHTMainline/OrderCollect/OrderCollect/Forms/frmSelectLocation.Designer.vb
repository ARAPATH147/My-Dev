<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSelectLocation
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSelectLocation))
        Me.btnQuit = New System.Windows.Forms.PictureBox
        Me.btnCalcPad1 = New System.Windows.Forms.PictureBox
        Me.lblScanSelectLocation = New System.Windows.Forms.Label
        Me.lblSelectLocation = New System.Windows.Forms.Label
        Me.lstParcelLocation = New System.Windows.Forms.ListBox
        Me.lblParcelLocationList = New System.Windows.Forms.Label
        Me.lblOrderNo = New System.Windows.Forms.Label
        Me.TextBox1 = New System.Windows.Forms.TextBox
        Me.lblOrderNumber = New System.Windows.Forms.Label
        Me.lblBookedInMessage = New System.Windows.Forms.Label
        Me.pbHelp = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'btnQuit
        '
        Me.btnQuit.Image = CType(resources.GetObject("btnQuit.Image"), System.Drawing.Image)
        Me.btnQuit.Location = New System.Drawing.Point(176, 264)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'btnCalcPad1
        '
        Me.btnCalcPad1.Image = CType(resources.GetObject("btnCalcPad1.Image"), System.Drawing.Image)
        Me.btnCalcPad1.Location = New System.Drawing.Point(203, 54)
        Me.btnCalcPad1.Name = "btnCalcPad1"
        Me.btnCalcPad1.Size = New System.Drawing.Size(20, 23)
        Me.btnCalcPad1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblScanSelectLocation
        '
        Me.lblScanSelectLocation.Location = New System.Drawing.Point(6, 82)
        Me.lblScanSelectLocation.Name = "lblScanSelectLocation"
        Me.lblScanSelectLocation.Size = New System.Drawing.Size(220, 20)
        Me.lblScanSelectLocation.Text = "Scan / Enter Location"
        '
        'lblSelectLocation
        '
        Me.lblSelectLocation.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelectLocation.Location = New System.Drawing.Point(6, 35)
        Me.lblSelectLocation.Name = "lblSelectLocation"
        Me.lblSelectLocation.Size = New System.Drawing.Size(226, 16)
        Me.lblSelectLocation.Text = "Scan Location for parcel "
        '
        'lstParcelLocation
        '
        Me.lstParcelLocation.Location = New System.Drawing.Point(6, 168)
        Me.lstParcelLocation.Name = "lstParcelLocation"
        Me.lstParcelLocation.Size = New System.Drawing.Size(220, 86)
        Me.lstParcelLocation.TabIndex = 42
        '
        'lblParcelLocationList
        '
        Me.lblParcelLocationList.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblParcelLocationList.Location = New System.Drawing.Point(6, 148)
        Me.lblParcelLocationList.Name = "lblParcelLocationList"
        Me.lblParcelLocationList.Size = New System.Drawing.Size(201, 20)
        Me.lblParcelLocationList.Text = "Parcel No.         Location"
        '
        'lblOrderNo
        '
        Me.lblOrderNo.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblOrderNo.Location = New System.Drawing.Point(6, 110)
        Me.lblOrderNo.Name = "lblOrderNo"
        Me.lblOrderNo.Size = New System.Drawing.Size(70, 20)
        Me.lblOrderNo.Text = "Order No."
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(6, 54)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(194, 21)
        Me.TextBox1.TabIndex = 49
        '
        'lblOrderNumber
        '
        Me.lblOrderNumber.Location = New System.Drawing.Point(79, 110)
        Me.lblOrderNumber.Name = "lblOrderNumber"
        Me.lblOrderNumber.Size = New System.Drawing.Size(144, 20)
        Me.lblOrderNumber.Text = "1234567890"
        '
        'lblBookedInMessage
        '
        Me.lblBookedInMessage.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblBookedInMessage.Location = New System.Drawing.Point(6, 133)
        Me.lblBookedInMessage.Name = "lblBookedInMessage"
        Me.lblBookedInMessage.Size = New System.Drawing.Size(226, 20)
        Me.lblBookedInMessage.Text = "Parcels already booked in for this order"
        '
        'pbHelp
        '
        Me.pbHelp.Image = CType(resources.GetObject("pbHelp.Image"), System.Drawing.Image)
        Me.pbHelp.Location = New System.Drawing.Point(205, 1)
        Me.pbHelp.Name = "pbHelp"
        Me.pbHelp.Size = New System.Drawing.Size(32, 32)
        Me.pbHelp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmSelectLocation
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.pbHelp)
        Me.Controls.Add(Me.lblParcelLocationList)
        Me.Controls.Add(Me.lblBookedInMessage)
        Me.Controls.Add(Me.lblOrderNumber)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.lstParcelLocation)
        Me.Controls.Add(Me.lblOrderNo)
        Me.Controls.Add(Me.lblSelectLocation)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.btnCalcPad1)
        Me.Controls.Add(Me.lblScanSelectLocation)
        Me.Name = "frmSelectLocation"
        Me.Text = "Order && Collect"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnQuit As System.Windows.Forms.PictureBox
    Friend WithEvents btnCalcPad1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblScanSelectLocation As System.Windows.Forms.Label
    Friend WithEvents lblSelectLocation As System.Windows.Forms.Label
    Friend WithEvents lstParcelLocation As System.Windows.Forms.ListBox
    Friend WithEvents lblParcelLocationList As System.Windows.Forms.Label
    Friend WithEvents lblOrderNo As System.Windows.Forms.Label
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents lblOrderNumber As System.Windows.Forms.Label
    Friend WithEvents lblBookedInMessage As System.Windows.Forms.Label
    Friend WithEvents pbHelp As System.Windows.Forms.PictureBox
End Class
