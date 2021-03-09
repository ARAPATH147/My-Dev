<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmFileDetails
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFileDetails))
        Me.lblSMActDwTimeVal = New System.Windows.Forms.Label
        Me.lblActDwTime = New System.Windows.Forms.Label
        Me.lblRefDataDwTimeVal = New System.Windows.Forms.Label
        Me.lblRefDataDwTime = New System.Windows.Forms.Label
        Me.lblLogFileSizeVal = New System.Windows.Forms.Label
        Me.lblLogFileSize = New System.Windows.Forms.Label
        Me.lblExpShmonDataSizeVal = New System.Windows.Forms.Label
        Me.lblExpDataSize = New System.Windows.Forms.Label
        Me.lblSMExpDwTimeVal = New System.Windows.Forms.Label
        Me.lblExpDataDwTime = New System.Windows.Forms.Label
        Me.lblLogSize = New System.Windows.Forms.Label
        Me.lbl_SM_Ex_Size = New System.Windows.Forms.Label
        Me.lblOf = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.lbl_GO_Ex_Size = New System.Windows.Forms.Label
        Me.lblExpGODataSizeVal = New System.Windows.Forms.Label
        Me.lbl_GI_Ex_Size = New System.Windows.Forms.Label
        Me.lblExpGIDataSizeVal = New System.Windows.Forms.Label
        Me.btnOk = New System.Windows.Forms.PictureBox
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.PictureBox2 = New System.Windows.Forms.PictureBox
        Me.PictureBox3 = New System.Windows.Forms.PictureBox
        Me.lblReferenceStatus = New System.Windows.Forms.Label
        Me.lblActdataStatus = New System.Windows.Forms.Label
        Me.lblExpStatus = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.lblGOExpDwTimeVal = New System.Windows.Forms.Label
        Me.lblGIExpDwTimeVal = New System.Windows.Forms.Label
        Me.lblGOActDwTimeVal = New System.Windows.Forms.Label
        Me.lblGIActDwTimeVal = New System.Windows.Forms.Label
        Me.lblProcessInfo = New System.Windows.Forms.Label
        Me.lblTotalFiles = New System.Windows.Forms.Label
        Me.lblCurrFile = New System.Windows.Forms.Label
        Me.pgBar = New System.Windows.Forms.ProgressBar
        Me.objStatusBar = New McUtilities.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblSMActDwTimeVal
        '
        Me.lblSMActDwTimeVal.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblSMActDwTimeVal.Location = New System.Drawing.Point(163, 48)
        Me.lblSMActDwTimeVal.Name = "lblSMActDwTimeVal"
        Me.lblSMActDwTimeVal.Size = New System.Drawing.Size(42, 18)
        Me.lblSMActDwTimeVal.Text = "XX:XX"
        Me.lblSMActDwTimeVal.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblActDwTime
        '
        Me.lblActDwTime.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblActDwTime.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblActDwTime.ForeColor = System.Drawing.SystemColors.Desktop
        Me.lblActDwTime.Location = New System.Drawing.Point(0, 31)
        Me.lblActDwTime.Name = "lblActDwTime"
        Me.lblActDwTime.Size = New System.Drawing.Size(240, 17)
        Me.lblActDwTime.Text = "Active Data Upload Time:"
        '
        'lblRefDataDwTimeVal
        '
        Me.lblRefDataDwTimeVal.BackColor = System.Drawing.Color.White
        Me.lblRefDataDwTimeVal.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblRefDataDwTimeVal.Location = New System.Drawing.Point(160, 16)
        Me.lblRefDataDwTimeVal.Name = "lblRefDataDwTimeVal"
        Me.lblRefDataDwTimeVal.Size = New System.Drawing.Size(45, 16)
        Me.lblRefDataDwTimeVal.Text = "XX:XX"
        Me.lblRefDataDwTimeVal.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblRefDataDwTime
        '
        Me.lblRefDataDwTime.BackColor = System.Drawing.Color.White
        Me.lblRefDataDwTime.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblRefDataDwTime.Location = New System.Drawing.Point(2, 1)
        Me.lblRefDataDwTime.Name = "lblRefDataDwTime"
        Me.lblRefDataDwTime.Size = New System.Drawing.Size(158, 28)
        Me.lblRefDataDwTime.Text = "Reference Data Upload Time:"
        '
        'lblLogFileSizeVal
        '
        Me.lblLogFileSizeVal.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblLogFileSizeVal.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblLogFileSizeVal.Location = New System.Drawing.Point(143, 157)
        Me.lblLogFileSizeVal.Name = "lblLogFileSizeVal"
        Me.lblLogFileSizeVal.Size = New System.Drawing.Size(62, 19)
        Me.lblLogFileSizeVal.Text = "0"
        Me.lblLogFileSizeVal.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblLogFileSize
        '
        Me.lblLogFileSize.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblLogFileSize.Location = New System.Drawing.Point(0, 155)
        Me.lblLogFileSize.Name = "lblLogFileSize"
        Me.lblLogFileSize.Size = New System.Drawing.Size(133, 20)
        Me.lblLogFileSize.Text = "Total Log File Size     :"
        '
        'lblExpShmonDataSizeVal
        '
        Me.lblExpShmonDataSizeVal.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblExpShmonDataSizeVal.Location = New System.Drawing.Point(144, 187)
        Me.lblExpShmonDataSizeVal.Name = "lblExpShmonDataSizeVal"
        Me.lblExpShmonDataSizeVal.Size = New System.Drawing.Size(62, 20)
        Me.lblExpShmonDataSizeVal.Text = "0"
        Me.lblExpShmonDataSizeVal.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblExpDataSize
        '
        Me.lblExpDataSize.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblExpDataSize.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblExpDataSize.ForeColor = System.Drawing.SystemColors.Desktop
        Me.lblExpDataSize.Location = New System.Drawing.Point(0, 171)
        Me.lblExpDataSize.Name = "lblExpDataSize"
        Me.lblExpDataSize.Size = New System.Drawing.Size(240, 14)
        Me.lblExpDataSize.Text = "Export Data Size"
        '
        'lblSMExpDwTimeVal
        '
        Me.lblSMExpDwTimeVal.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblSMExpDwTimeVal.Location = New System.Drawing.Point(161, 110)
        Me.lblSMExpDwTimeVal.Name = "lblSMExpDwTimeVal"
        Me.lblSMExpDwTimeVal.Size = New System.Drawing.Size(45, 18)
        Me.lblSMExpDwTimeVal.Text = "XX:XX"
        Me.lblSMExpDwTimeVal.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblExpDataDwTime
        '
        Me.lblExpDataDwTime.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblExpDataDwTime.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblExpDataDwTime.ForeColor = System.Drawing.SystemColors.Desktop
        Me.lblExpDataDwTime.Location = New System.Drawing.Point(-3, 93)
        Me.lblExpDataDwTime.Name = "lblExpDataDwTime"
        Me.lblExpDataDwTime.Size = New System.Drawing.Size(240, 16)
        Me.lblExpDataDwTime.Text = " Export Data Download Time:"
        '
        'lblLogSize
        '
        Me.lblLogSize.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblLogSize.Location = New System.Drawing.Point(205, 157)
        Me.lblLogSize.Name = "lblLogSize"
        Me.lblLogSize.Size = New System.Drawing.Size(24, 17)
        Me.lblLogSize.Text = "B"
        Me.lblLogSize.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lbl_SM_Ex_Size
        '
        Me.lbl_SM_Ex_Size.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lbl_SM_Ex_Size.Location = New System.Drawing.Point(205, 187)
        Me.lbl_SM_Ex_Size.Name = "lbl_SM_Ex_Size"
        Me.lbl_SM_Ex_Size.Size = New System.Drawing.Size(24, 20)
        Me.lbl_SM_Ex_Size.Text = "B"
        Me.lbl_SM_Ex_Size.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblOf
        '
        Me.lblOf.Location = New System.Drawing.Point(197, 253)
        Me.lblOf.Name = "lblOf"
        Me.lblOf.Size = New System.Drawing.Size(10, 20)
        Me.lblOf.Text = "/"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label3.Location = New System.Drawing.Point(31, 187)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(129, 18)
        Me.Label3.Text = "Shelf Management"
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label4.Location = New System.Drawing.Point(31, 202)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(80, 18)
        Me.Label4.Text = "Goods Out"
        '
        'Label6
        '
        Me.Label6.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label6.Location = New System.Drawing.Point(31, 218)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(82, 19)
        Me.Label6.Text = "Goods In"
        '
        'lbl_GO_Ex_Size
        '
        Me.lbl_GO_Ex_Size.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lbl_GO_Ex_Size.Location = New System.Drawing.Point(205, 202)
        Me.lbl_GO_Ex_Size.Name = "lbl_GO_Ex_Size"
        Me.lbl_GO_Ex_Size.Size = New System.Drawing.Size(24, 18)
        Me.lbl_GO_Ex_Size.Text = "B"
        Me.lbl_GO_Ex_Size.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblExpGODataSizeVal
        '
        Me.lblExpGODataSizeVal.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblExpGODataSizeVal.Location = New System.Drawing.Point(144, 202)
        Me.lblExpGODataSizeVal.Name = "lblExpGODataSizeVal"
        Me.lblExpGODataSizeVal.Size = New System.Drawing.Size(62, 18)
        Me.lblExpGODataSizeVal.Text = "0"
        Me.lblExpGODataSizeVal.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lbl_GI_Ex_Size
        '
        Me.lbl_GI_Ex_Size.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lbl_GI_Ex_Size.Location = New System.Drawing.Point(205, 218)
        Me.lbl_GI_Ex_Size.Name = "lbl_GI_Ex_Size"
        Me.lbl_GI_Ex_Size.Size = New System.Drawing.Size(24, 19)
        Me.lbl_GI_Ex_Size.Text = "B"
        Me.lbl_GI_Ex_Size.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblExpGIDataSizeVal
        '
        Me.lblExpGIDataSizeVal.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblExpGIDataSizeVal.Location = New System.Drawing.Point(143, 218)
        Me.lblExpGIDataSizeVal.Name = "lblExpGIDataSizeVal"
        Me.lblExpGIDataSizeVal.Size = New System.Drawing.Size(62, 19)
        Me.lblExpGIDataSizeVal.Text = "0"
        Me.lblExpGIDataSizeVal.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'btnOk
        '
        Me.btnOk.Image = CType(resources.GetObject("btnOk.Image"), System.Drawing.Image)
        Me.btnOk.Location = New System.Drawing.Point(190, 234)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(40, 40)
        Me.btnOk.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(217, 1)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(22, 20)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'PictureBox2
        '
        Me.PictureBox2.Image = CType(resources.GetObject("PictureBox2.Image"), System.Drawing.Image)
        Me.PictureBox2.Location = New System.Drawing.Point(217, 28)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(22, 20)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'PictureBox3
        '
        Me.PictureBox3.Image = CType(resources.GetObject("PictureBox3.Image"), System.Drawing.Image)
        Me.PictureBox3.Location = New System.Drawing.Point(216, 91)
        Me.PictureBox3.Name = "PictureBox3"
        Me.PictureBox3.Size = New System.Drawing.Size(22, 20)
        Me.PictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblReferenceStatus
        '
        Me.lblReferenceStatus.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblReferenceStatus.Location = New System.Drawing.Point(164, 1)
        Me.lblReferenceStatus.Name = "lblReferenceStatus"
        Me.lblReferenceStatus.Size = New System.Drawing.Size(50, 16)
        Me.lblReferenceStatus.Text = "Failed"
        Me.lblReferenceStatus.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblActdataStatus
        '
        Me.lblActdataStatus.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblActdataStatus.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblActdataStatus.Location = New System.Drawing.Point(164, 32)
        Me.lblActdataStatus.Name = "lblActdataStatus"
        Me.lblActdataStatus.Size = New System.Drawing.Size(50, 16)
        Me.lblActdataStatus.Text = "Failed"
        Me.lblActdataStatus.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblExpStatus
        '
        Me.lblExpStatus.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblExpStatus.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblExpStatus.ForeColor = System.Drawing.SystemColors.Desktop
        Me.lblExpStatus.Location = New System.Drawing.Point(164, 93)
        Me.lblExpStatus.Name = "lblExpStatus"
        Me.lblExpStatus.Size = New System.Drawing.Size(50, 16)
        Me.lblExpStatus.Text = "Failed"
        Me.lblExpStatus.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(31, 138)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(82, 19)
        Me.Label1.Text = "Goods In"
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.Location = New System.Drawing.Point(31, 124)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(80, 18)
        Me.Label2.Text = "Goods Out"
        '
        'Label5
        '
        Me.Label5.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label5.Location = New System.Drawing.Point(31, 110)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(129, 18)
        Me.Label5.Text = "Shelf Management"
        '
        'Label7
        '
        Me.Label7.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label7.Location = New System.Drawing.Point(31, 79)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(82, 19)
        Me.Label7.Text = "Goods In"
        '
        'Label8
        '
        Me.Label8.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label8.Location = New System.Drawing.Point(31, 63)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(80, 18)
        Me.Label8.Text = "Goods Out"
        '
        'Label9
        '
        Me.Label9.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label9.Location = New System.Drawing.Point(31, 48)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(129, 18)
        Me.Label9.Text = "Shelf Management"
        '
        'lblGOExpDwTimeVal
        '
        Me.lblGOExpDwTimeVal.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblGOExpDwTimeVal.Location = New System.Drawing.Point(160, 124)
        Me.lblGOExpDwTimeVal.Name = "lblGOExpDwTimeVal"
        Me.lblGOExpDwTimeVal.Size = New System.Drawing.Size(45, 18)
        Me.lblGOExpDwTimeVal.Text = "XX:XX"
        Me.lblGOExpDwTimeVal.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblGIExpDwTimeVal
        '
        Me.lblGIExpDwTimeVal.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblGIExpDwTimeVal.Location = New System.Drawing.Point(160, 138)
        Me.lblGIExpDwTimeVal.Name = "lblGIExpDwTimeVal"
        Me.lblGIExpDwTimeVal.Size = New System.Drawing.Size(45, 18)
        Me.lblGIExpDwTimeVal.Text = "XX:XX"
        Me.lblGIExpDwTimeVal.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblGOActDwTimeVal
        '
        Me.lblGOActDwTimeVal.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblGOActDwTimeVal.Location = New System.Drawing.Point(163, 63)
        Me.lblGOActDwTimeVal.Name = "lblGOActDwTimeVal"
        Me.lblGOActDwTimeVal.Size = New System.Drawing.Size(42, 18)
        Me.lblGOActDwTimeVal.Text = "XX:XX"
        Me.lblGOActDwTimeVal.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblGIActDwTimeVal
        '
        Me.lblGIActDwTimeVal.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblGIActDwTimeVal.Location = New System.Drawing.Point(163, 76)
        Me.lblGIActDwTimeVal.Name = "lblGIActDwTimeVal"
        Me.lblGIActDwTimeVal.Size = New System.Drawing.Size(42, 18)
        Me.lblGIActDwTimeVal.Text = "XX:XX"
        Me.lblGIActDwTimeVal.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblProcessInfo
        '
        Me.lblProcessInfo.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblProcessInfo.Location = New System.Drawing.Point(0, 232)
        Me.lblProcessInfo.Name = "lblProcessInfo"
        Me.lblProcessInfo.Size = New System.Drawing.Size(236, 23)
        Me.lblProcessInfo.Text = "Process Info Displayed Here"
        '
        'lblTotalFiles
        '
        Me.lblTotalFiles.Location = New System.Drawing.Point(204, 254)
        Me.lblTotalFiles.Name = "lblTotalFiles"
        Me.lblTotalFiles.Size = New System.Drawing.Size(33, 20)
        Me.lblTotalFiles.Text = "0000"
        '
        'lblCurrFile
        '
        Me.lblCurrFile.Location = New System.Drawing.Point(165, 254)
        Me.lblCurrFile.Name = "lblCurrFile"
        Me.lblCurrFile.Size = New System.Drawing.Size(33, 20)
        Me.lblCurrFile.Text = "0000"
        Me.lblCurrFile.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'pgBar
        '
        Me.pgBar.Location = New System.Drawing.Point(3, 256)
        Me.pgBar.Name = "pgBar"
        Me.pgBar.Size = New System.Drawing.Size(154, 10)
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 96
        '
        'frmFileDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.lblExpStatus)
        Me.Controls.Add(Me.PictureBox3)
        Me.Controls.Add(Me.lblExpDataDwTime)
        Me.Controls.Add(Me.lblGIExpDwTimeVal)
        Me.Controls.Add(Me.lblGOExpDwTimeVal)
        Me.Controls.Add(Me.lblExpDataSize)
        Me.Controls.Add(Me.lblLogFileSize)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblActdataStatus)
        Me.Controls.Add(Me.lblReferenceStatus)
        Me.Controls.Add(Me.PictureBox2)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.lbl_GI_Ex_Size)
        Me.Controls.Add(Me.lblExpGIDataSizeVal)
        Me.Controls.Add(Me.lbl_GO_Ex_Size)
        Me.Controls.Add(Me.lblExpGODataSizeVal)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.lblTotalFiles)
        Me.Controls.Add(Me.lblOf)
        Me.Controls.Add(Me.lblCurrFile)
        Me.Controls.Add(Me.lbl_SM_Ex_Size)
        Me.Controls.Add(Me.lblLogSize)
        Me.Controls.Add(Me.pgBar)
        Me.Controls.Add(Me.lblSMExpDwTimeVal)
        Me.Controls.Add(Me.lblExpShmonDataSizeVal)
        Me.Controls.Add(Me.lblLogFileSizeVal)
        Me.Controls.Add(Me.lblSMActDwTimeVal)
        Me.Controls.Add(Me.lblActDwTime)
        Me.Controls.Add(Me.lblRefDataDwTimeVal)
        Me.Controls.Add(Me.lblRefDataDwTime)
        Me.Controls.Add(Me.lblGIActDwTimeVal)
        Me.Controls.Add(Me.lblGOActDwTimeVal)
        Me.Controls.Add(Me.lblProcessInfo)
        Me.Controls.Add(Me.Label6)
        Me.ForeColor = System.Drawing.Color.White
        Me.Name = "frmFileDetails"
        Me.Text = "File Details"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblSMActDwTimeVal As System.Windows.Forms.Label
    Friend WithEvents lblActDwTime As System.Windows.Forms.Label
    Friend WithEvents lblRefDataDwTimeVal As System.Windows.Forms.Label
    Friend WithEvents lblRefDataDwTime As System.Windows.Forms.Label
    Friend WithEvents lblLogFileSizeVal As System.Windows.Forms.Label
    Friend WithEvents lblLogFileSize As System.Windows.Forms.Label
    Friend WithEvents lblExpShmonDataSizeVal As System.Windows.Forms.Label
    Friend WithEvents lblExpDataSize As System.Windows.Forms.Label
    Friend WithEvents lblSMExpDwTimeVal As System.Windows.Forms.Label
    Friend WithEvents lblExpDataDwTime As System.Windows.Forms.Label
    Friend WithEvents lblLogSize As System.Windows.Forms.Label
    Friend WithEvents lbl_SM_Ex_Size As System.Windows.Forms.Label
    Friend WithEvents lblOf As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents lbl_GO_Ex_Size As System.Windows.Forms.Label
    Friend WithEvents lblExpGODataSizeVal As System.Windows.Forms.Label
    Friend WithEvents lbl_GI_Ex_Size As System.Windows.Forms.Label
    Friend WithEvents lblExpGIDataSizeVal As System.Windows.Forms.Label
    Friend WithEvents btnOk As System.Windows.Forms.PictureBox
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
    Friend WithEvents PictureBox3 As System.Windows.Forms.PictureBox
    Friend WithEvents lblReferenceStatus As System.Windows.Forms.Label
    Friend WithEvents lblActdataStatus As System.Windows.Forms.Label
    Friend WithEvents lblExpStatus As System.Windows.Forms.Label
    Public WithEvents objStatusBar As McUtilities.CustomStatusBar
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents lblGOExpDwTimeVal As System.Windows.Forms.Label
    Friend WithEvents lblGIExpDwTimeVal As System.Windows.Forms.Label
    Friend WithEvents lblGOActDwTimeVal As System.Windows.Forms.Label
    Friend WithEvents lblGIActDwTimeVal As System.Windows.Forms.Label
    Friend WithEvents lblProcessInfo As System.Windows.Forms.Label
    Friend WithEvents lblTotalFiles As System.Windows.Forms.Label
    Friend WithEvents lblCurrFile As System.Windows.Forms.Label
    Friend WithEvents pgBar As System.Windows.Forms.ProgressBar
End Class
