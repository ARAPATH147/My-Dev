<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmShlfMgmntMenu
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmShlfMgmntMenu))
        Me.pbSMMenu = New System.Windows.Forms.PictureBox
        Me.lblSMMenu = New System.Windows.Forms.Label
        Me.lblPLMenu = New System.Windows.Forms.Label
        Me.pbPLMenu = New System.Windows.Forms.PictureBox
        Me.lblFstFillMenu = New System.Windows.Forms.Label
        Me.pbFstFillMenu = New System.Windows.Forms.PictureBox
        Me.lblPriceCheckMenu = New System.Windows.Forms.Label
        Me.pbPriceCheckMenu = New System.Windows.Forms.PictureBox
        Me.tbShlfMgmtMenu = New System.Windows.Forms.TabControl
        Me.tbpgMdlDay = New System.Windows.Forms.TabPage
        Me.pbExsStck_Gray = New System.Windows.Forms.PictureBox
        Me.pbCntlst_Gray = New System.Windows.Forms.PictureBox
        Me.pbSMMenu_Gray = New System.Windows.Forms.PictureBox
        Me.lblAutoSYS = New System.Windows.Forms.Label
        Me.pbCntLst = New System.Windows.Forms.PictureBox
        Me.lblExsStck = New System.Windows.Forms.Label
        Me.pbxAutoSYS = New System.Windows.Forms.PictureBox
        Me.lblCntLst = New System.Windows.Forms.Label
        Me.pbExsStck = New System.Windows.Forms.PictureBox
        Me.tbpgPlans = New System.Windows.Forms.TabPage
        Me.lblPendingPlanner = New System.Windows.Forms.Label
        Me.pbPendingPlanner = New System.Windows.Forms.PictureBox
        Me.lblSPMenu = New System.Windows.Forms.Label
        Me.lblLPMenu = New System.Windows.Forms.Label
        Me.pbSPMenu = New System.Windows.Forms.PictureBox
        Me.pbLPMenu = New System.Windows.Forms.PictureBox
        Me.tbpgInfo = New System.Windows.Forms.TabPage
        Me.lblReports = New System.Windows.Forms.Label
        Me.pbReports = New System.Windows.Forms.PictureBox
        Me.lblItemSales = New System.Windows.Forms.Label
        Me.pbItemSales = New System.Windows.Forms.PictureBox
        Me.lblStoreSales = New System.Windows.Forms.Label
        Me.pbStoreSales = New System.Windows.Forms.PictureBox
        Me.lblItemInfo = New System.Windows.Forms.Label
        Me.pbxItemInfo = New System.Windows.Forms.PictureBox
        Me.tbpgPrint = New System.Windows.Forms.TabPage
        Me.lblPrtClrLbl = New System.Windows.Forms.Label
        Me.PBPrtClrLbl = New System.Windows.Forms.PictureBox
        Me.lblAssignPrinter = New System.Windows.Forms.Label
        Me.pbAPMenu = New System.Windows.Forms.PictureBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.pbPSMenu = New System.Windows.Forms.PictureBox
        Me.tbpgLogoff = New System.Windows.Forms.TabPage
        Me.lblLogOff = New System.Windows.Forms.Label
        Me.pbLogOff = New System.Windows.Forms.PictureBox
        Me.ImageList1 = New System.Windows.Forms.ImageList
        Me.tmrAlarm = New System.Windows.Forms.Timer
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.tbShlfMgmtMenu.SuspendLayout()
        Me.tbpgMdlDay.SuspendLayout()
        Me.tbpgPlans.SuspendLayout()
        Me.tbpgInfo.SuspendLayout()
        Me.tbpgPrint.SuspendLayout()
        Me.tbpgLogoff.SuspendLayout()
        Me.SuspendLayout()
        '
        'pbSMMenu
        '
        Me.pbSMMenu.Image = CType(resources.GetObject("pbSMMenu.Image"), System.Drawing.Image)
        Me.pbSMMenu.Location = New System.Drawing.Point(31, 11)
        Me.pbSMMenu.Name = "pbSMMenu"
        Me.pbSMMenu.Size = New System.Drawing.Size(60, 60)
        Me.pbSMMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbSMMenu.Visible = False
        '
        'lblSMMenu
        '
        Me.lblSMMenu.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSMMenu.Location = New System.Drawing.Point(15, 73)
        Me.lblSMMenu.Name = "lblSMMenu"
        Me.lblSMMenu.Size = New System.Drawing.Size(88, 15)
        Me.lblSMMenu.Text = "Shelf Monitor"
        Me.lblSMMenu.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblPLMenu
        '
        Me.lblPLMenu.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblPLMenu.Location = New System.Drawing.Point(16, 157)
        Me.lblPLMenu.Name = "lblPLMenu"
        Me.lblPLMenu.Size = New System.Drawing.Size(88, 17)
        Me.lblPLMenu.Text = "Picking List"
        Me.lblPLMenu.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbPLMenu
        '
        Me.pbPLMenu.Image = CType(resources.GetObject("pbPLMenu.Image"), System.Drawing.Image)
        Me.pbPLMenu.Location = New System.Drawing.Point(31, 96)
        Me.pbPLMenu.Name = "pbPLMenu"
        Me.pbPLMenu.Size = New System.Drawing.Size(60, 60)
        Me.pbPLMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblFstFillMenu
        '
        Me.lblFstFillMenu.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblFstFillMenu.Location = New System.Drawing.Point(136, 73)
        Me.lblFstFillMenu.Name = "lblFstFillMenu"
        Me.lblFstFillMenu.Size = New System.Drawing.Size(88, 15)
        Me.lblFstFillMenu.Text = "Fast Fill"
        Me.lblFstFillMenu.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbFstFillMenu
        '
        Me.pbFstFillMenu.Image = CType(resources.GetObject("pbFstFillMenu.Image"), System.Drawing.Image)
        Me.pbFstFillMenu.Location = New System.Drawing.Point(156, 11)
        Me.pbFstFillMenu.Name = "pbFstFillMenu"
        Me.pbFstFillMenu.Size = New System.Drawing.Size(60, 60)
        Me.pbFstFillMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblPriceCheckMenu
        '
        Me.lblPriceCheckMenu.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblPriceCheckMenu.Location = New System.Drawing.Point(138, 157)
        Me.lblPriceCheckMenu.Name = "lblPriceCheckMenu"
        Me.lblPriceCheckMenu.Size = New System.Drawing.Size(88, 17)
        Me.lblPriceCheckMenu.Text = "Price Check"
        Me.lblPriceCheckMenu.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbPriceCheckMenu
        '
        Me.pbPriceCheckMenu.Image = CType(resources.GetObject("pbPriceCheckMenu.Image"), System.Drawing.Image)
        Me.pbPriceCheckMenu.Location = New System.Drawing.Point(156, 96)
        Me.pbPriceCheckMenu.Name = "pbPriceCheckMenu"
        Me.pbPriceCheckMenu.Size = New System.Drawing.Size(60, 60)
        Me.pbPriceCheckMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'tbShlfMgmtMenu
        '
        Me.tbShlfMgmtMenu.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.tbShlfMgmtMenu.Controls.Add(Me.tbpgMdlDay)
        Me.tbShlfMgmtMenu.Controls.Add(Me.tbpgPlans)
        Me.tbShlfMgmtMenu.Controls.Add(Me.tbpgInfo)
        Me.tbShlfMgmtMenu.Controls.Add(Me.tbpgPrint)
        Me.tbShlfMgmtMenu.Controls.Add(Me.tbpgLogoff)
        Me.tbShlfMgmtMenu.Dock = System.Windows.Forms.DockStyle.None
        Me.tbShlfMgmtMenu.Location = New System.Drawing.Point(0, 0)
        Me.tbShlfMgmtMenu.Name = "tbShlfMgmtMenu"
        Me.tbShlfMgmtMenu.SelectedIndex = 0
        Me.tbShlfMgmtMenu.Size = New System.Drawing.Size(240, 279)
        Me.tbShlfMgmtMenu.TabIndex = 13
        '
        'tbpgMdlDay
        '
        Me.tbpgMdlDay.AutoScroll = True
        Me.tbpgMdlDay.Controls.Add(Me.pbExsStck_Gray)
        Me.tbpgMdlDay.Controls.Add(Me.pbCntlst_Gray)
        Me.tbpgMdlDay.Controls.Add(Me.pbSMMenu_Gray)
        Me.tbpgMdlDay.Controls.Add(Me.lblAutoSYS)
        Me.tbpgMdlDay.Controls.Add(Me.pbCntLst)
        Me.tbpgMdlDay.Controls.Add(Me.lblExsStck)
        Me.tbpgMdlDay.Controls.Add(Me.pbxAutoSYS)
        Me.tbpgMdlDay.Controls.Add(Me.lblCntLst)
        Me.tbpgMdlDay.Controls.Add(Me.pbExsStck)
        Me.tbpgMdlDay.Controls.Add(Me.pbPLMenu)
        Me.tbpgMdlDay.Controls.Add(Me.lblFstFillMenu)
        Me.tbpgMdlDay.Controls.Add(Me.lblPriceCheckMenu)
        Me.tbpgMdlDay.Controls.Add(Me.pbFstFillMenu)
        Me.tbpgMdlDay.Controls.Add(Me.lblPLMenu)
        Me.tbpgMdlDay.Controls.Add(Me.lblSMMenu)
        Me.tbpgMdlDay.Controls.Add(Me.pbPriceCheckMenu)
        Me.tbpgMdlDay.Controls.Add(Me.pbSMMenu)
        Me.tbpgMdlDay.Location = New System.Drawing.Point(0, 0)
        Me.tbpgMdlDay.Name = "tbpgMdlDay"
        Me.tbpgMdlDay.Size = New System.Drawing.Size(240, 256)
        Me.tbpgMdlDay.Text = "Model Day"
        '
        'pbExsStck_Gray
        '
        Me.pbExsStck_Gray.Enabled = False
        Me.pbExsStck_Gray.Image = CType(resources.GetObject("pbExsStck_Gray.Image"), System.Drawing.Image)
        Me.pbExsStck_Gray.Location = New System.Drawing.Point(156, 182)
        Me.pbExsStck_Gray.Name = "pbExsStck_Gray"
        Me.pbExsStck_Gray.Size = New System.Drawing.Size(60, 60)
        Me.pbExsStck_Gray.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbExsStck_Gray.Visible = False
        '
        'pbCntlst_Gray
        '
        Me.pbCntlst_Gray.Enabled = False
        Me.pbCntlst_Gray.Image = CType(resources.GetObject("pbCntlst_Gray.Image"), System.Drawing.Image)
        Me.pbCntlst_Gray.Location = New System.Drawing.Point(31, 182)
        Me.pbCntlst_Gray.Name = "pbCntlst_Gray"
        Me.pbCntlst_Gray.Size = New System.Drawing.Size(60, 60)
        Me.pbCntlst_Gray.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbCntlst_Gray.Visible = False
        '
        'pbSMMenu_Gray
        '
        Me.pbSMMenu_Gray.Enabled = False
        Me.pbSMMenu_Gray.Image = CType(resources.GetObject("pbSMMenu_Gray.Image"), System.Drawing.Image)
        Me.pbSMMenu_Gray.Location = New System.Drawing.Point(31, 11)
        Me.pbSMMenu_Gray.Name = "pbSMMenu_Gray"
        Me.pbSMMenu_Gray.Size = New System.Drawing.Size(60, 60)
        Me.pbSMMenu_Gray.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbSMMenu_Gray.Visible = False
        '
        'lblAutoSYS
        '
        Me.lblAutoSYS.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblAutoSYS.Location = New System.Drawing.Point(18, 334)
        Me.lblAutoSYS.Name = "lblAutoSYS"
        Me.lblAutoSYS.Size = New System.Drawing.Size(88, 30)
        Me.lblAutoSYS.Text = "Stuff Your Shelves"
        Me.lblAutoSYS.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbCntLst
        '
        Me.pbCntLst.Image = CType(resources.GetObject("pbCntLst.Image"), System.Drawing.Image)
        Me.pbCntLst.Location = New System.Drawing.Point(31, 182)
        Me.pbCntLst.Name = "pbCntLst"
        Me.pbCntLst.Size = New System.Drawing.Size(60, 60)
        Me.pbCntLst.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbCntLst.Visible = False
        '
        'lblExsStck
        '
        Me.lblExsStck.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblExsStck.Location = New System.Drawing.Point(138, 244)
        Me.lblExsStck.Name = "lblExsStck"
        Me.lblExsStck.Size = New System.Drawing.Size(88, 17)
        Me.lblExsStck.Text = "Excess Stock"
        Me.lblExsStck.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbxAutoSYS
        '
        Me.pbxAutoSYS.Image = CType(resources.GetObject("pbxAutoSYS.Image"), System.Drawing.Image)
        Me.pbxAutoSYS.Location = New System.Drawing.Point(31, 271)
        Me.pbxAutoSYS.Name = "pbxAutoSYS"
        Me.pbxAutoSYS.Size = New System.Drawing.Size(60, 60)
        Me.pbxAutoSYS.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblCntLst
        '
        Me.lblCntLst.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblCntLst.Location = New System.Drawing.Point(15, 244)
        Me.lblCntLst.Name = "lblCntLst"
        Me.lblCntLst.Size = New System.Drawing.Size(88, 17)
        Me.lblCntLst.Text = "Count List"
        Me.lblCntLst.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbExsStck
        '
        Me.pbExsStck.Image = CType(resources.GetObject("pbExsStck.Image"), System.Drawing.Image)
        Me.pbExsStck.Location = New System.Drawing.Point(156, 182)
        Me.pbExsStck.Name = "pbExsStck"
        Me.pbExsStck.Size = New System.Drawing.Size(60, 60)
        Me.pbExsStck.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pbExsStck.Visible = False
        '
        'tbpgPlans
        '
        Me.tbpgPlans.Controls.Add(Me.lblPendingPlanner)
        Me.tbpgPlans.Controls.Add(Me.pbPendingPlanner)
        Me.tbpgPlans.Controls.Add(Me.lblSPMenu)
        Me.tbpgPlans.Controls.Add(Me.lblLPMenu)
        Me.tbpgPlans.Controls.Add(Me.pbSPMenu)
        Me.tbpgPlans.Controls.Add(Me.pbLPMenu)
        Me.tbpgPlans.Location = New System.Drawing.Point(0, 0)
        Me.tbpgPlans.Name = "tbpgPlans"
        Me.tbpgPlans.Size = New System.Drawing.Size(232, 253)
        Me.tbpgPlans.Text = "Plan"
        '
        'lblPendingPlanner
        '
        Me.lblPendingPlanner.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblPendingPlanner.Location = New System.Drawing.Point(16, 174)
        Me.lblPendingPlanner.Name = "lblPendingPlanner"
        Me.lblPendingPlanner.Size = New System.Drawing.Size(106, 20)
        Me.lblPendingPlanner.Text = "Pending Planner"
        '
        'pbPendingPlanner
        '
        Me.pbPendingPlanner.Image = CType(resources.GetObject("pbPendingPlanner.Image"), System.Drawing.Image)
        Me.pbPendingPlanner.Location = New System.Drawing.Point(31, 110)
        Me.pbPendingPlanner.Name = "pbPendingPlanner"
        Me.pbPendingPlanner.Size = New System.Drawing.Size(60, 60)
        Me.pbPendingPlanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblSPMenu
        '
        Me.lblSPMenu.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSPMenu.Location = New System.Drawing.Point(124, 79)
        Me.lblSPMenu.Name = "lblSPMenu"
        Me.lblSPMenu.Size = New System.Drawing.Size(109, 17)
        Me.lblSPMenu.Text = "Search Planners"
        Me.lblSPMenu.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblLPMenu
        '
        Me.lblLPMenu.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblLPMenu.Location = New System.Drawing.Point(16, 79)
        Me.lblLPMenu.Name = "lblLPMenu"
        Me.lblLPMenu.Size = New System.Drawing.Size(88, 17)
        Me.lblLPMenu.Text = "Live Planners"
        Me.lblLPMenu.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbSPMenu
        '
        Me.pbSPMenu.Image = CType(resources.GetObject("pbSPMenu.Image"), System.Drawing.Image)
        Me.pbSPMenu.Location = New System.Drawing.Point(150, 16)
        Me.pbSPMenu.Name = "pbSPMenu"
        Me.pbSPMenu.Size = New System.Drawing.Size(60, 60)
        Me.pbSPMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbLPMenu
        '
        Me.pbLPMenu.Image = CType(resources.GetObject("pbLPMenu.Image"), System.Drawing.Image)
        Me.pbLPMenu.Location = New System.Drawing.Point(31, 16)
        Me.pbLPMenu.Name = "pbLPMenu"
        Me.pbLPMenu.Size = New System.Drawing.Size(60, 60)
        Me.pbLPMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'tbpgInfo
        '
        Me.tbpgInfo.Controls.Add(Me.lblReports)
        Me.tbpgInfo.Controls.Add(Me.pbReports)
        Me.tbpgInfo.Controls.Add(Me.lblItemSales)
        Me.tbpgInfo.Controls.Add(Me.pbItemSales)
        Me.tbpgInfo.Controls.Add(Me.lblStoreSales)
        Me.tbpgInfo.Controls.Add(Me.pbStoreSales)
        Me.tbpgInfo.Controls.Add(Me.lblItemInfo)
        Me.tbpgInfo.Controls.Add(Me.pbxItemInfo)
        Me.tbpgInfo.Location = New System.Drawing.Point(0, 0)
        Me.tbpgInfo.Name = "tbpgInfo"
        Me.tbpgInfo.Size = New System.Drawing.Size(232, 253)
        Me.tbpgInfo.Text = "Info"
        '
        'lblReports
        '
        Me.lblReports.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblReports.Location = New System.Drawing.Point(153, 175)
        Me.lblReports.Name = "lblReports"
        Me.lblReports.Size = New System.Drawing.Size(87, 20)
        Me.lblReports.Text = "Reports"
        '
        'pbReports
        '
        Me.pbReports.Image = CType(resources.GetObject("pbReports.Image"), System.Drawing.Image)
        Me.pbReports.Location = New System.Drawing.Point(153, 112)
        Me.pbReports.Name = "pbReports"
        Me.pbReports.Size = New System.Drawing.Size(60, 60)
        Me.pbReports.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblItemSales
        '
        Me.lblItemSales.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblItemSales.Location = New System.Drawing.Point(153, 89)
        Me.lblItemSales.Name = "lblItemSales"
        Me.lblItemSales.Size = New System.Drawing.Size(100, 20)
        Me.lblItemSales.Text = "Item Sales"
        '
        'pbItemSales
        '
        Me.pbItemSales.Image = CType(resources.GetObject("pbItemSales.Image"), System.Drawing.Image)
        Me.pbItemSales.Location = New System.Drawing.Point(153, 16)
        Me.pbItemSales.Name = "pbItemSales"
        Me.pbItemSales.Size = New System.Drawing.Size(60, 60)
        Me.pbItemSales.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblStoreSales
        '
        Me.lblStoreSales.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblStoreSales.Location = New System.Drawing.Point(21, 79)
        Me.lblStoreSales.Name = "lblStoreSales"
        Me.lblStoreSales.Size = New System.Drawing.Size(100, 20)
        Me.lblStoreSales.Text = "Store Sales"
        '
        'pbStoreSales
        '
        Me.pbStoreSales.Image = CType(resources.GetObject("pbStoreSales.Image"), System.Drawing.Image)
        Me.pbStoreSales.Location = New System.Drawing.Point(21, 16)
        Me.pbStoreSales.Name = "pbStoreSales"
        Me.pbStoreSales.Size = New System.Drawing.Size(60, 60)
        Me.pbStoreSales.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblItemInfo
        '
        Me.lblItemInfo.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblItemInfo.Location = New System.Drawing.Point(21, 180)
        Me.lblItemInfo.Name = "lblItemInfo"
        Me.lblItemInfo.Size = New System.Drawing.Size(88, 15)
        Me.lblItemInfo.Text = "Item Info"
        '
        'pbxItemInfo
        '
        Me.pbxItemInfo.Image = CType(resources.GetObject("pbxItemInfo.Image"), System.Drawing.Image)
        Me.pbxItemInfo.Location = New System.Drawing.Point(21, 112)
        Me.pbxItemInfo.Name = "pbxItemInfo"
        Me.pbxItemInfo.Size = New System.Drawing.Size(60, 60)
        Me.pbxItemInfo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'tbpgPrint
        '
        Me.tbpgPrint.Controls.Add(Me.lblPrtClrLbl)
        Me.tbpgPrint.Controls.Add(Me.PBPrtClrLbl)
        Me.tbpgPrint.Controls.Add(Me.lblAssignPrinter)
        Me.tbpgPrint.Controls.Add(Me.pbAPMenu)
        Me.tbpgPrint.Controls.Add(Me.Label1)
        Me.tbpgPrint.Controls.Add(Me.pbPSMenu)
        Me.tbpgPrint.Location = New System.Drawing.Point(0, 0)
        Me.tbpgPrint.Name = "tbpgPrint"
        Me.tbpgPrint.Size = New System.Drawing.Size(232, 253)
        Me.tbpgPrint.Text = "Print"
        '
        'lblPrtClrLbl
        '
        Me.lblPrtClrLbl.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblPrtClrLbl.Location = New System.Drawing.Point(15, 184)
        Me.lblPrtClrLbl.Name = "lblPrtClrLbl"
        Me.lblPrtClrLbl.Size = New System.Drawing.Size(100, 32)
        Me.lblPrtClrLbl.Text = "Print Clearance Label"
        Me.lblPrtClrLbl.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'PBPrtClrLbl
        '
        Me.PBPrtClrLbl.Image = CType(resources.GetObject("PBPrtClrLbl.Image"), System.Drawing.Image)
        Me.PBPrtClrLbl.Location = New System.Drawing.Point(37, 121)
        Me.PBPrtClrLbl.Name = "PBPrtClrLbl"
        Me.PBPrtClrLbl.Size = New System.Drawing.Size(60, 60)
        Me.PBPrtClrLbl.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblAssignPrinter
        '
        Me.lblAssignPrinter.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblAssignPrinter.Location = New System.Drawing.Point(23, 83)
        Me.lblAssignPrinter.Name = "lblAssignPrinter"
        Me.lblAssignPrinter.Size = New System.Drawing.Size(94, 20)
        Me.lblAssignPrinter.Text = "Assign Printer"
        '
        'pbAPMenu
        '
        Me.pbAPMenu.Image = CType(resources.GetObject("pbAPMenu.Image"), System.Drawing.Image)
        Me.pbAPMenu.Location = New System.Drawing.Point(37, 20)
        Me.pbAPMenu.Name = "pbAPMenu"
        Me.pbAPMenu.Size = New System.Drawing.Size(60, 60)
        Me.pbAPMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(144, 83)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(88, 17)
        Me.Label1.Text = "Print SEL"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbPSMenu
        '
        Me.pbPSMenu.Image = CType(resources.GetObject("pbPSMenu.Image"), System.Drawing.Image)
        Me.pbPSMenu.Location = New System.Drawing.Point(156, 20)
        Me.pbPSMenu.Name = "pbPSMenu"
        Me.pbPSMenu.Size = New System.Drawing.Size(60, 60)
        Me.pbPSMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'tbpgLogoff
        '
        Me.tbpgLogoff.Controls.Add(Me.lblLogOff)
        Me.tbpgLogoff.Controls.Add(Me.pbLogOff)
        Me.tbpgLogoff.Location = New System.Drawing.Point(0, 0)
        Me.tbpgLogoff.Name = "tbpgLogoff"
        Me.tbpgLogoff.Size = New System.Drawing.Size(232, 253)
        Me.tbpgLogoff.Text = "Log Off"
        '
        'lblLogOff
        '
        Me.lblLogOff.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblLogOff.Location = New System.Drawing.Point(18, 79)
        Me.lblLogOff.Name = "lblLogOff"
        Me.lblLogOff.Size = New System.Drawing.Size(85, 18)
        Me.lblLogOff.Text = "Log Off"
        Me.lblLogOff.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'pbLogOff
        '
        Me.pbLogOff.Image = CType(resources.GetObject("pbLogOff.Image"), System.Drawing.Image)
        Me.pbLogOff.Location = New System.Drawing.Point(31, 17)
        Me.pbLogOff.Name = "pbLogOff"
        Me.pbLogOff.Size = New System.Drawing.Size(60, 60)
        Me.pbLogOff.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'ImageList1
        '
        Me.ImageList1.ImageSize = New System.Drawing.Size(16, 16)
        '
        'tmrAlarm
        '
        Me.tmrAlarm.Interval = 30000
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.ScrollBar
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 274)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 20)
        Me.objStatusBar.TabIndex = 129
        '
        'frmShlfMgmntMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.BackColor = System.Drawing.SystemColors.ScrollBar
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.tbShlfMgmtMenu)
        Me.KeyPreview = True
        Me.Name = "frmShlfMgmntMenu"
        Me.Text = "Shelf Management"
        Me.tbShlfMgmtMenu.ResumeLayout(False)
        Me.tbpgMdlDay.ResumeLayout(False)
        Me.tbpgPlans.ResumeLayout(False)
        Me.tbpgInfo.ResumeLayout(False)
        Me.tbpgPrint.ResumeLayout(False)
        Me.tbpgLogoff.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pbSMMenu As System.Windows.Forms.PictureBox
    Friend WithEvents lblSMMenu As System.Windows.Forms.Label
    Friend WithEvents lblPLMenu As System.Windows.Forms.Label
    Friend WithEvents pbPLMenu As System.Windows.Forms.PictureBox
    Friend WithEvents lblFstFillMenu As System.Windows.Forms.Label
    Friend WithEvents pbFstFillMenu As System.Windows.Forms.PictureBox
    Friend WithEvents lblPriceCheckMenu As System.Windows.Forms.Label
    Friend WithEvents pbPriceCheckMenu As System.Windows.Forms.PictureBox
    Friend WithEvents tbpgMdlDay As System.Windows.Forms.TabPage
    Friend WithEvents tbpgPlans As System.Windows.Forms.TabPage
    Friend WithEvents tbpgInfo As System.Windows.Forms.TabPage
    Friend WithEvents tbpgPrint As System.Windows.Forms.TabPage
    Friend WithEvents tbpgLogoff As System.Windows.Forms.TabPage
    Friend WithEvents pbCntLst As System.Windows.Forms.PictureBox
    Friend WithEvents lblExsStck As System.Windows.Forms.Label
    Friend WithEvents lblCntLst As System.Windows.Forms.Label
    Friend WithEvents pbExsStck As System.Windows.Forms.PictureBox
    Friend WithEvents lblLogOff As System.Windows.Forms.Label
    Friend WithEvents pbLogOff As System.Windows.Forms.PictureBox
    Friend WithEvents lblItemInfo As System.Windows.Forms.Label
    Friend WithEvents pbxItemInfo As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents pbPSMenu As System.Windows.Forms.PictureBox
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents lblSPMenu As System.Windows.Forms.Label
    Friend WithEvents lblLPMenu As System.Windows.Forms.Label
    Friend WithEvents pbSPMenu As System.Windows.Forms.PictureBox
    Friend WithEvents pbLPMenu As System.Windows.Forms.PictureBox
    Public WithEvents tmrAlarm As System.Windows.Forms.Timer
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Protected WithEvents tbShlfMgmtMenu As System.Windows.Forms.TabControl
    Friend WithEvents lblAutoSYS As System.Windows.Forms.Label
    Friend WithEvents pbxAutoSYS As System.Windows.Forms.PictureBox
    Friend WithEvents pbAPMenu As System.Windows.Forms.PictureBox
    Friend WithEvents lblAssignPrinter As System.Windows.Forms.Label
    Friend WithEvents PBPrtClrLbl As System.Windows.Forms.PictureBox
    Friend WithEvents lblPrtClrLbl As System.Windows.Forms.Label
    Friend WithEvents pbStoreSales As System.Windows.Forms.PictureBox
    Friend WithEvents lblItemSales As System.Windows.Forms.Label
    Friend WithEvents pbItemSales As System.Windows.Forms.PictureBox
    Friend WithEvents lblStoreSales As System.Windows.Forms.Label
    Friend WithEvents lblReports As System.Windows.Forms.Label
    Friend WithEvents pbReports As System.Windows.Forms.PictureBox
    Friend WithEvents pbPendingPlanner As System.Windows.Forms.PictureBox
    Friend WithEvents lblPendingPlanner As System.Windows.Forms.Label
    Friend WithEvents pbSMMenu_Gray As System.Windows.Forms.PictureBox
    Friend WithEvents pbCntlst_Gray As System.Windows.Forms.PictureBox
    Friend WithEvents pbExsStck_Gray As System.Windows.Forms.PictureBox
End Class
