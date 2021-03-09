#If RF Then
Imports System.Data
Imports System.Text
Public Class PPSessionMgr
    Private m_SPPPhome As frmSPLPHome
    Private m_SPLineListPP As frmSPLineListLP
    Private m_SPPPDeptCat As frmSPLPDeptCat
    Private m_SPModuleListPP As frmSPModuleListLP
    Private m_SPPlannerListPP As frmSPPlannerListLP
    Private m_SPPrintSEL As frmSPPrintSEL
    Private m_SPSearchPlnrHome As frmSPSearchPlnrHome
    Private m_PlannerItemInfo As frmPlannerItemInfo
    Private m_PSProductInfo As PSProductInfo = Nothing 'anoop
    Private Shared m_PPSessionMgr As PPSessionMgr = Nothing
    Private m_SPDeptInfo As CategoryInfo = Nothing
    Private m_SPPlannerListInfo As PlannerInfo = Nothing
    Private m_SPModuleListInfo As ModuleInfo = Nothing
    Private m_SPLinelistInfo As SPLineListInfo = Nothing
    Private m_SPProductInfo As ProductInfo = Nothing
    Private m_IIProductInfo As ItemInfo = Nothing
    Public m_arrSPDeptCatList As ArrayList = Nothing
    Private m_arrSPPlannerList As ArrayList = Nothing
    Private m_arrSPModuleList As ArrayList = Nothing
    Private m_arrSPLineList As ArrayList = Nothing
    Private m_arrItemDescr As ArrayList = Nothing
    Private m_arrDealDataList As ArrayList = Nothing

    'Variable to store the Boots code of the search item in search planner
    Private strSearchItemBC As String = ""
    Private m_ScannedItem As String = ""
    Private m_strPlannerType As String = ""

    Private m_ModulePriceCheck As ModulePriceCheck = New ModulePriceCheck()
    Private m_PreviousItem As String
    'Private m_bIsFullPriceCheckRequired As Boolean
    'System Testing - Flag to identify single module
    Private m_bIsSingleModule As Boolean
    Private m_bInvokedFromItemInfo As Boolean
    Private m_strSEL As String
    Private m_QueuedSELList As ArrayList = Nothing
    Private m_FullPriceCheckCount As Integer = 0
    Private m_QueuedPRPList As ArrayList = Nothing
    Private m_LabelType As MobilePrintSessionManager.LabelType 'anoop

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()

    End Sub
    ''' <summary>
    ''' Functions for getting the object instance for the SPSessionMgr. 
    ''' Use this method to get the object reference for the Singleton SPSessionMgr.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of SPSessionMgr Class</remarks>
    Public Shared Function GetInstance() As PPSessionMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.SPCEPLAN_PENDING
        If m_PPSessionMgr Is Nothing Then
            m_PPSessionMgr = New PPSessionMgr()
            Return m_PPSessionMgr
        Else
            Return m_PPSessionMgr
        End If
    End Function
#If RF Then
    ''' <summary>
    ''' Updates the Status bar of all the forms in the session manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBarMessage()
        Try
            m_SPPPhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SPLineListPP.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SPPPDeptCat.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SPModuleListPP.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SPPlannerListPP.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SPPrintSEL.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SPSearchPlnrHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_PlannerItemInfo.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured, Trace: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End If
    ''' <summary>
    ''' Initialises the Space Planning Session 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function StartSession(ByVal PlannerType As PLANTYPE, Optional ByVal bInvokedFromItemInfo As Boolean = False) As Boolean
        'Do all price check realated Initialisations here.
        Try
            'anoop: Send a PGS message
            If Not (objAppContainer.objExportDataManager.CreatePGS()) Then
                objAppContainer.objLogger.WriteAppLog("Cannot Create PGS record at PP Start Session", Logger.LogLevel.RELEASE)
                'TODO: Donot proceed further if PGS message response is not successful
                'Naveen 
                'Fixed the false scenario
                Return False
            End If
            m_SPPPhome = New frmSPLPHome()
            m_SPLineListPP = New frmSPLineListLP()
            m_SPPPDeptCat = New frmSPLPDeptCat()
            m_SPModuleListPP = New frmSPModuleListLP()
            m_SPPlannerListPP = New frmSPPlannerListLP()
            m_SPPrintSEL = New frmSPPrintSEL()
            m_SPSearchPlnrHome = New frmSPSearchPlnrHome()
            m_PlannerItemInfo = New frmPlannerItemInfo()
            m_PSProductInfo = New PSProductInfo() 'anoop
            'Naveen
            ''''Bug HAs to be fixed
            'Setting app Module back to the original module
            objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.SPCEPLAN_PENDING
            m_SPDeptInfo = New CategoryInfo
            m_SPPlannerListInfo = New PlannerInfo
            m_SPModuleListInfo = New ModuleInfo
            m_SPLinelistInfo = New SPLineListInfo
            m_SPProductInfo = New ProductInfo
            m_IIProductInfo = New ItemInfo

            m_arrSPDeptCatList = New ArrayList()
            m_arrSPPlannerList = New ArrayList()
            m_arrSPModuleList = New ArrayList()
            m_arrSPLineList = New ArrayList()
            m_arrItemDescr = New ArrayList()
            m_arrDealDataList = New ArrayList()

            m_QueuedSELList = New ArrayList()
            m_QueuedPRPList = New ArrayList()
            'm_bIsFullPriceCheckRequired = False
            'System testing
            m_bIsSingleModule = False
            m_bInvokedFromItemInfo = bInvokedFromItemInfo
            m_strSEL = ""
            m_PreviousItem = ""
            m_strPlannerType = PlannerType
            m_ModulePriceCheck = New ModulePriceCheck()
            m_LabelType = MobilePrintSessionManager.LabelType.STD 'anoop
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Space Planner Session cannot be started" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Space Planner Start Session", Logger.LogLevel.INFO)

    End Function
    ''' <summary>
    ''' Does all processing that needs to be done when a session ends
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function EndSession(Optional ByVal isConnectivityLoss As Boolean = False) As Boolean

        Try
            If Not isConnectivityLoss Then
                'anoop: Send a PGX message
                If Not (objAppContainer.objExportDataManager.CreatePGX()) Then
                    objAppContainer.objLogger.WriteAppLog("Cannot Create PGX record at PP Start Session", Logger.LogLevel.RELEASE)
                    Return False
                End If
            End If


            'If full price check is performed, PCM data needs to be written.

            'Save and data and perform all Exit Operations.
            'Close and Dispose all forms.
            m_PlannerItemInfo.Dispose()
            m_SPLineListPP.Dispose()
            m_SPPrintSEL.Dispose()
            m_SPModuleListPP.Dispose()
            m_SPPlannerListPP.Dispose()
            m_SPPPDeptCat.Dispose()
            m_SPSearchPlnrHome.Dispose()
            m_SPPPhome.Dispose()

            'Release all objects and Set to nothing.
            m_SPDeptInfo = Nothing
            m_SPPlannerListInfo = Nothing
            m_SPModuleListInfo = Nothing
            m_SPLinelistInfo = Nothing
            m_SPProductInfo = Nothing
            m_IIProductInfo = Nothing

            m_arrDealDataList = Nothing
            m_arrSPDeptCatList = Nothing
            m_arrSPPlannerList = Nothing
            m_arrSPModuleList = Nothing
            m_arrSPLineList = Nothing
            m_arrItemDescr = Nothing
            m_PreviousItem = Nothing
            m_QueuedSELList = Nothing
            m_QueuedPRPList = Nothing
            m_ModulePriceCheck = Nothing
            'System testing
            m_bIsSingleModule = Nothing
            'Clear the class variable
            m_PPSessionMgr = Nothing
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at Space Palnner End Session" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Space Planner End Session", Logger.LogLevel.INFO)
    End Function


    ''' <summary>
    ''' Screen Display method for Space Planning. 
    ''' All Space Planner sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName">Enum SPSCREENS</param>
    ''' <returns>True if display is success else False</returns>
    ''' <remarks></remarks>
    Public Function DisplaySPScreen(ByVal ScreenName As PPSCREENS)
        Try
            Select Case ScreenName
                Case PPSCREENS.LPHome
                    m_SPPPhome.Invoke(New EventHandler(AddressOf DisplayPPHome))
                Case PPSCREENS.DeptCategory
                    m_SPPPDeptCat.Invoke(New EventHandler(AddressOf DisplaySPDeptCat))
                Case PPSCREENS.LineListPP
                    m_SPLineListPP.Invoke(New EventHandler(AddressOf DisplaySPLineListPP))
                Case PPSCREENS.ModuleListPP
                    m_SPModuleListPP.Invoke(New EventHandler(AddressOf DisplaySPModuleListPP))
                Case PPSCREENS.PlannerListPP
                    m_SPPlannerListPP.Invoke(New EventHandler(AddressOf DisplaySPPlannerListPP))
                Case PPSCREENS.PrintSEL
                    m_SPPrintSEL.Invoke(New EventHandler(AddressOf DisplaySPPrintSEL))
                Case PPSCREENS.ItemDetails
                    m_PlannerItemInfo.Invoke(New EventHandler(AddressOf DisplayPlannerItemDetails))
            End Select
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Space Planner Screen cannot be launched" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function

    ''' <summary>
    ''' This subroutine sets the home screen display of Live Planner
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>

    Private Sub DisplayPPHome(ByVal o As Object, ByVal e As EventArgs)
        With m_SPPPhome
            'Sets the store id and active data time to the status bar
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
            .lblLivePlnr.Text = "Pending Planner" 'anoop To be moved to Macro list
            .Visible = True
            .Refresh()
        End With

    End Sub

    ''' <summary>
    ''' This subroutine displays the Department/Category screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>

    Private Sub DisplaySPDeptCat(ByVal o As Object, ByVal e As EventArgs)
        Try
            With m_SPPPDeptCat
                .lstView.Columns.Clear()
                .lstView.Items.Clear()
                .lstView.Columns.Add("", 235, HorizontalAlignment.Left)

                '.lstView.Items.Add((New ListViewItem(New String() {"Department/Category"})))
                'Populates the Department/Category List
                PopulateDeptList()
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                .Label1.Text = "Pending Planners" 'anoop
                .Visible = True
                .Text = "Pending Planners"
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured Space Plan Display Department Screen: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try

    End Sub
    ''' <summary>
    ''' Populates the Department/category list view
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateDeptList()

        With m_SPPPDeptCat

            Try
                For Each objDeptCat As CategoryInfo In m_arrSPDeptCatList
                    ''UAT - To remove 'Boots, 'Core' and 'Sales Plan' categories from the list
                    'If Trim(objDeptCat.Description) <> "Boots" And Trim(objDeptCat.Description) <> "Sales Plan" And _
                    'Trim(objDeptCat.Description) <> "Core" Then
                    .lstView.Items.Add( _
                    (New ListViewItem(New String() {Trim(objDeptCat.Description)})))
                    'End If
                Next
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("Exception occured Space Planner Populate department list: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            End Try
        End With

    End Sub

    'System Testing
    ''' <summary>
    ''' This subroutine invokes Planner from item details screen
    ''' </summary>
    ''' <remarks></remarks>
    '''anoop:Is this required for pending planners?
    Public Sub DisplayPlanner(ByVal strBootsCode As String)
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr DisplayPlanner", Logger.LogLevel.RELEASE)
        Try
            If Not (objAppContainer.objHelper.ValidateBootsCode(strBootsCode)) Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)

            Else
                strSearchItemBC = strBootsCode
                m_arrSPPlannerList.Clear()
                If Not (objAppContainer.objDataEngine.GetPlannerListUsingBC(strBootsCode, False, m_arrSPPlannerList)) Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                Else
                    If m_arrSPPlannerList.Count > 0 Then
                        DisplaySPScreen(SPSessionMgr.SPSCREENS.PlannerListSP)
                        Exit Sub
                    Else
                        MessageBox.Show("Planners not available for the item scanned")
                    End If
                End If

            End If
            ItemInfoSessionMgr.GetInstance.DisplayIIScreen(ItemInfoSessionMgr.IISCREENS.ItemDetails)
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("PPSessionMgr - Exception in full DisplayPlannner", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PPSessionMgr DisplayPlanner", Logger.LogLevel.RELEASE)
    End Sub

    ''' <summary>
    ''' This subroutine displays the Line List screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>

    Private Sub DisplaySPLineListPP(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr DisplaySPLineListPP", Logger.LogLevel.RELEASE)
        Dim iCount As Integer = 0
        Dim iIndex As Integer = 0
        Try
            With m_SPLineListPP
                .lstView.Columns.Clear()
                .lstView.Items.Clear()
                .lblModule.Text() = objAppContainer.objHelper.FormatEscapeSequence(m_SPPlannerListInfo.Description.ToString())
                'Defect:-117 START: Set the rebuild date for planner.
                'Formats the date to display in YY/MM/DD format
                Dim strRebuildDate As String = ""
                strRebuildDate = m_SPPlannerListInfo.RebuildDate
                'Defect:-117
                If strRebuildDate.Length = 8 Then
                    strRebuildDate = strRebuildDate.Substring(2, 6)

                    Dim strBuilder As StringBuilder = New StringBuilder()
                    strBuilder.Append(strRebuildDate.Substring(4, 2))
                    strBuilder.Append("/")
                    strBuilder.Append(strRebuildDate.Substring(2, 2))
                    strBuilder.Append("/")
                    strBuilder.Append(strRebuildDate.Substring(0, 2))

                    strRebuildDate = strBuilder.ToString()

                    .Label1.Text() = strRebuildDate
                    .Text = "Pending Planner"
                End If
                'Defect:-117: END

                .lstView.Columns.Add("  Item       Description", 180, HorizontalAlignment.Left)
                .lstView.Columns.Add("Facing", 50, HorizontalAlignment.Left)

                .lstView.Items.Add((New ListViewItem(New String() {m_SPModuleListInfo.Description, "FC"})))
                While m_arrSPLineList.Count() > 0

                    Dim obj As SPLineListInfo = Nothing
                    obj = m_arrSPLineList(iCount)
                    Dim query = From obj1 As SPLineListInfo In m_arrSPLineList _
                                Where obj1.ShelfNumber = obj.ShelfNumber _
                                Select obj1

                    .lstView.Items.Add( _
                        (New ListViewItem(New String() {obj.ModuleSeqNumber + "." + obj.ShelfNumber + _
                                                        " Notch " + _
                                                        obj.NotchNumber + " " + _
                                                                 obj.ShelfDesc})))
                    'TODO check if shelf number needs to be appended
                    Dim arrlist As New ArrayList
                    For Each obj1 As SPLineListInfo In query
                        .lstView.Items.Add( _
                        (New ListViewItem(New String() {"  " + obj1.ItemCode + " " + _
                                                                 obj1.ItemDescription, _
                                                                obj1.FaceCount})))

                        'm_arrSPLineList.Remove(obj1)
                        arrlist.Add(obj1)

                    Next
                    For Each obj1 As SPLineListInfo In arrlist
                        m_arrSPLineList.Remove(obj1)
                    Next
                End While
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured Space Planner Display line list Screen: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr DisplaySPLineListLP", Logger.LogLevel.RELEASE)
    End Sub

    ''' <summary>
    ''' This subroutine displays the Module List screen for live planner
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>

    Private Sub DisplaySPModuleListPP(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr DisplaySPModuleListLP", Logger.LogLevel.RELEASE)
        Dim strCount As String = "01"
        Try
            With m_SPModuleListPP
                .Text = "Pending Planner"
                .lstView.Columns.Clear()
                .lstView.Items.Clear()
                .lblPlanner.Text() = objAppContainer.objHelper.FormatEscapeSequence(m_SPPlannerListInfo.Description.ToString())

                'Formats the date to display in YY/MM/DD format
                Dim strRebuildDate As String = ""
                strRebuildDate = m_SPPlannerListInfo.RebuildDate
                'IT External - 834
                If strRebuildDate.Length = 8 Then
                    strRebuildDate = strRebuildDate.Substring(2, 6)

                    Dim strBuilder As StringBuilder = New StringBuilder()
                    strBuilder.Append(strRebuildDate.Substring(4, 2))
                    strBuilder.Append("/")
                    strBuilder.Append(strRebuildDate.Substring(2, 2))
                    strBuilder.Append("/")
                    strBuilder.Append(strRebuildDate.Substring(0, 2))

                    strRebuildDate = strBuilder.ToString()

                    .lblDate.Text() = strRebuildDate
                End If
                .lstView.Columns.Add("", 30, HorizontalAlignment.Left)

                .lstView.Columns.Add("", 150, HorizontalAlignment.Left)
                Dim query = From obj1 As ModuleInfo In m_arrSPModuleList _
                            Order By obj1.Description.Split(" ")(1)
                For Each objModule As ModuleInfo In query
                    .lstView.Items.Add( _
                    (New ListViewItem(New String() {strCount, objModule.Description})))
                    If (CInt(strCount) < 9) Then
                        strCount = "0" + CStr(CInt(strCount) + 1)
                    Else
                        strCount = CStr(CInt(strCount) + 1)
                    End If
                Next
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("PPSessionMgr - Exception in full DisplaySPModuleListLP", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PPSessionMgr DisplaySPModuleListLP", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' This subroutine displays the Planner List screen
    ''' </summary>
    ''' <remarks></remarks>

    Private Sub DisplaySPPlannerListPP(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr DisplaySPPlannerListLP", Logger.LogLevel.RELEASE)
        Try


            With m_SPPlannerListPP
                .Text = "Pending Planner"
                .lblDept.Text() = objAppContainer.objHelper.FormatEscapeSequence(m_SPDeptInfo.Description.ToString())
                .lstView.Columns.Clear()
                .lstView.Items.Clear()
                '.lstView.Columns.Add("", 140, HorizontalAlignment.Left)
                '.lstView.Columns.Add("", 70, HorizontalAlignment.Right)

                .lstView.Columns.Add("", 100, HorizontalAlignment.Left)
                .lstView.Columns.Add("", 80, HorizontalAlignment.Center)
                .lstView.Columns.Add("", 20, HorizontalAlignment.Right)


                For Each objPlanner As PlannerInfo In m_arrSPPlannerList

                    'Formats the date to display in YY/MM/DD format
                    Dim strRebuildDate As String = ""
                    strRebuildDate = objPlanner.RebuildDate
                    'IT External - 834
                    If strRebuildDate.Length = 8 Then
                        strRebuildDate = strRebuildDate.Substring(2, 6)

                        Dim strBuilder As StringBuilder = New StringBuilder()
                        strBuilder.Append(strRebuildDate.Substring(4, 2))
                        strBuilder.Append("/")
                        strBuilder.Append(strRebuildDate.Substring(2, 2))
                        strBuilder.Append("/")
                        strBuilder.Append(strRebuildDate.Substring(0, 2))

                        strRebuildDate = strBuilder.ToString()
                    End If
                    .lstView.Items.Add( _
                    (New ListViewItem(New String() {objPlanner.Description, strRebuildDate, objPlanner.RepeatCount})))

                Next

                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured Space Planner Display planner list Screen: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr DisplaySPPlanerListLP", Logger.LogLevel.RELEASE)
    End Sub

    ''' <summary>
    ''' This subroutine displays the Print SEL screen for Space Planner module
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>

    Private Sub DisplaySPPrintSEL(ByVal o As Object, ByVal e As EventArgs)
        Try
            With m_SPPrintSEL
                .lblItem.Text() = m_SPPlannerListInfo.Description.ToString()
                .lstView.Columns.Clear()
                .lstView.Items.Clear()
                .lstView.Columns.Add("", 120, HorizontalAlignment.Left)
                .lstView.Items.Add( _
                   (New ListViewItem(New String() {"Print All"})))
                For Each objModule As ModuleInfo In m_arrSPModuleList
                    .lstView.Items.Add( _
                    (New ListViewItem(New String() {objModule.Description})))
                Next

                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("SPSessionMgr - Exception in full DisplaySPPrintSEL", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr DisplaySPPrintSEL", Logger.LogLevel.RELEASE)
    End Sub
    'System Testing
    ''' <summary>
    ''' Displays the Planner Item Details Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayPlannerItemDetails(ByVal o As Object, ByVal e As EventArgs)
        Dim objDescriptionArray As ArrayList = New ArrayList()
        'Dim strDealNo As String
        Try
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(m_IIProductInfo.Description)
            '#If NRF Then

            '            Dim iRedemption As Integer
            '            Dim iCheckBit As Integer = 4

            '            iRedemption = CType(m_IIProductInfo.RedemptionFlag, Integer)
            '            If (iRedemption And iCheckBit) <> 0 Then
            '                m_PlannerItemInfo.lblRedeemableText.Text = "Yes"
            '            Else
            '                m_PlannerItemInfo.lblRedeemableText.Text = "No"
            '            End If
            '#End If
            'Code added for RF Mode -Lakshmi
#If RF Then
            If m_IIProductInfo.RedemptionFlag = Macros.Redemption_True Then
                m_PlannerItemInfo.lblRedeemableText.Text = "Yes"
            Else
                m_PlannerItemInfo.lblRedeemableText.Text = "No"
            End If
            If m_IIProductInfo.TSF = "      " Then
                m_IIProductInfo.TSF = 0
            End If
#End If
            'Ends-Lakshmi
            Dim strCurrency As String = ""
            Dim strBarcode As String = ""
            strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(m_IIProductInfo.ProductCode)
            strCurrency = objAppContainer.objHelper.GetCurrency()
            With m_PlannerItemInfo
                .lblBootsCode.Text = objAppContainer.objHelper.FormatBarcode(m_IIProductInfo.BootsCode)
                .lblProductCode.Text = objAppContainer.objHelper.FormatBarcode(strBarcode)
                .lblProdDescription1.Text = objDescriptionArray.Item(0)
                .lblProdDescription2.Text = objDescriptionArray.Item(1)
                .lblProdDescription3.Text = objDescriptionArray.Item(2)
                .lblStatusText.Text = objAppContainer.objHelper.GetStatusDescription(m_IIProductInfo.Status)
                'IT - Formatted Price to be displayed
                '.lblPriceText.Text = Val(m_IIProductInfo.Price)
                .lblPriceText.Text = objAppContainer.objHelper.FormatPrice(m_IIProductInfo.Price)
                .lblCurrencySymbol.Text = strCurrency
                .lblStockText.Text = m_IIProductInfo.TSF
                'IT - Internal
                Displaydeal(.cmbDeal)
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                .Visible = True
                .Refresh()

            End With
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("PPSessionMgr - Exception in full DisplayPlannnerItemDetails", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PPSessionMgr DisplayPlannerItemDetails", Logger.LogLevel.RELEASE)
    End Sub
    'IT Internal

    ''' <summary>
    ''' Method for populating deal info in the combo box.
    ''' </summary>
    ''' <param name="objDeal"></param>
    ''' <remarks></remarks>
    Public Sub Displaydeal(ByRef objDeal As ComboBox)
        Dim strDealNo As String
        Dim arrTemp() As String = Nothing
        objAppContainer.objLogger.WriteAppLog("Enter II DispalyDeal", Logger.LogLevel.RELEASE)
        Try
            'Dim iIndex As Integer
            'Dim arrList As ArrayList = New ArrayList
            strDealNo = m_IIProductInfo.DealList
            Dim arrDealDetails() As String = Nothing
            Dim arrDealType As ArrayList = Nothing

            If strDealNo.Equals("") Then
                objDeal.Visible = False
                With m_PlannerItemInfo
                    .lblDealHeader.Visible = False
                End With
            Else
                Dim iLen As Integer = 0
                arrDealDetails = strDealNo.Split(",")
                iLen = arrDealDetails.Length

                Dim iCounter As Integer = 0
                objDeal.Visible = True
                With m_PlannerItemInfo
                    .lblDealHeader.Visible = True
                End With
                objDeal.Enabled = False
                objDeal.Items.Clear()
                m_arrDealDataList.Clear()
                'If iLen > 1 Then
                '    objDeal.Items.Add("Select")
                'End If
                For iCounter = 0 To iLen - 1
                    Dim objDealDetails As DQRRECORD = New DQRRECORD()
                    objAppContainer.objDataEngine.GetDealDetails(arrDealDetails(iCounter), objDealDetails)

                    Dim strTemp As String
                    ' strTemp = "DealMsg_" + arrDealType(0)
                    'System Testing - 0 padding
                    'If arrDealType.Count > 0 Then
                    strTemp = "DealMsg_" + arrDealType(0).ToString().PadLeft(2, "0")
                    objDeal.Items.Add(ConfigDataMgr.GetInstance.GetParam(strTemp))
                    m_arrDealDataList.Add(arrDealType)
                    'End If
                Next
                'System testing
                'IT - Internal
                If objDeal.Items.Count() > 0 Then
                    objDeal.Items.Insert(0, "Select")
                    objDeal.Enabled = True
                    objDeal.SelectedIndex = 0

                ElseIf objDeal.Items.Count() = 0 Then
                    objDeal.Visible = False
                    m_PlannerItemInfo.lblDealHeader.Visible = False
                    'Else
                    '    objDeal.Enabled = True
                    'integration testing - if only deal is present leave it unselected.
                    'objDeal.SelectedIndex = 0
                    'objDeal.Text = objDeal.Items(0).ToString()
                End If
                'Systen testing - end

                'Systen testing - commented
                'objDeal.Enabled = True
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            'MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"))
            objAppContainer.objLogger.WriteAppLog("Exception occured Displaydeal. Exception is: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit II DispalyDeal", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Displays the deal information in the message for the deal selected
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DisplayDealInfo() As Boolean
        Dim arrDealData As ArrayList = New ArrayList()
        objAppContainer.objLogger.WriteAppLog("Enter II DisplayDealInfo", Logger.LogLevel.RELEASE)
        Try


            With m_PlannerItemInfo
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
                If .cmbDeal.Enabled = True Then
                    If Not .cmbDeal.SelectedItem.Equals("Select") Then
                        If m_arrDealDataList.Count > 0 Then
                            arrDealData = m_arrDealDataList.Item(.cmbDeal.SelectedIndex - 1)
                        Else
                            arrDealData = m_arrDealDataList.Item(.cmbDeal.SelectedIndex)
                        End If

                        Dim strBuilder As StringBuilder = New StringBuilder()
                        Dim strDate As String
                        strBuilder.Append("This Item is on the following Active Promotion")
                        strBuilder.Append(Environment.NewLine + "---------------------------" + Environment.NewLine)
                        strBuilder.Append(.cmbDeal.SelectedItem.ToString())
                        strDate = arrDealData.Item(1).ToString
                        strDate = strDate.Substring(6, 2) + "-" + strDate.Substring(4, 2) + "-" + strDate.Substring(0, 4)
                        strBuilder.Append(Environment.NewLine + "Start Date: " + strDate)
                        strDate = arrDealData.Item(2).ToString
                        strDate = strDate.Substring(6, 2) + "-" + strDate.Substring(4, 2) + "-" + strDate.Substring(0, 4)
                        strBuilder.Append(Environment.NewLine + "End Date: " + strDate)
                        strBuilder.Append(Environment.NewLine + "---------------------------" + Environment.NewLine)
                        strBuilder.Append("Check ALL Shelf Locations." + Environment.NewLine)
                        strBuilder.Append("Check Show Material.")
                        MessageBox.Show(strBuilder.ToString(), "Information")
                    End If
                End If
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured DisplaydealInfo. Exception is: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit II DisplayDealInfo", Logger.LogLevel.RELEASE)
    End Function

    Private Function ProcessDataRetrieval(ByVal strBootsCode As String) As Boolean
        Try
            If Not (objAppContainer.objDataEngine.GetItemDetailsAllUsingBC(strBootsCode, m_IIProductInfo)) Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                'Exit Function
                Return False
            End If
            DisplaySPScreen(PPSCREENS.ItemDetails)
            Return True
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Processes the selection of any product from the list view
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessProductSelectionForCategory() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr ProcessProductSelectionForCategory", Logger.LogLevel.RELEASE)
        Dim strListId As String = Nothing
        With m_SPPPDeptCat
            Dim iIndex As Integer = 0
            Dim iCounter As Integer = 0
            'Gets the selected list id from the listview
            Try
                Dim bIsDataAvailable As Boolean = False
                If .lstView.SelectedIndices.Count > 0 Then
                    For iCounter = 0 To .lstView.Items.Count - 1
                        If .lstView.Items(iCounter).Selected Then
                            'Retrives the selected Category info from the list
                            bIsDataAvailable = True

                            m_SPDeptInfo = m_arrSPDeptCatList.Item(iCounter)
                            'strListId = .lstView.Items(iCounter).Text
                            strListId = m_SPDeptInfo.POINTER.ToString()
                            Exit For
                        End If
                    Next
                End If

                'Processes the data to obtain the planner list
                If bIsDataAvailable Then
                    m_arrSPPlannerList.Clear()
                    If Not (objAppContainer.objDataEngine.GetPlannerListForCategory(strListId, m_arrSPPlannerList, True)) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M70"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                    Else
                        DisplaySPScreen(PPSCREENS.PlannerListPP)
                        Return True
                    End If
                End If
                Return False
            Catch ex As Exception
#If RF Then
                If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                    Throw ex
                End If
#End If
                objAppContainer.objLogger.WriteAppLog("Space Planner item selection cannot be processed. Exception is: " + ex.StackTrace, Logger.LogLevel.RELEASE)
                Return False
            End Try

        End With
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr ProcessProductSelectionForCategory", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Processes the selection of any product from the list view
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessProductSelectionForPlanner(ByVal strPlannerType As String) As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr ProcessProductSelectionForPlanner", Logger.LogLevel.RELEASE)
        Dim iIndex As Integer = 0
        Dim iCounter As Integer = 0
        Dim bStatus As Boolean = False
        Dim bIsDataAvailable As Boolean = False
        Try
            With m_SPPlannerListPP

                'Gets the selected list id from the listview
                If .lstView.SelectedIndices.Count > 0 Then
                    For iCounter = 0 To .lstView.Items.Count - 1
                        If .lstView.Items(iCounter).Selected Then
                            bIsDataAvailable = True
                            m_SPPlannerListInfo = m_arrSPPlannerList.Item(iCounter)
                            Exit For
                        End If
                    Next
                End If
            End With

            'gets the category id from the category array list
            'Dim strCatID As String = Nothing
            If Not bIsDataAvailable Then
                Return False
            End If
            ' If Not strPlannerID Is Nothing Then
            m_arrSPModuleList.Clear()

            bStatus = objAppContainer.objDataEngine.GetModuleList(m_SPPlannerListInfo.PlannerID, _
                                                                  m_arrSPModuleList)
            If Not bStatus Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
            Else
                'Display Line List screen directly if there is only one module.
                If m_arrSPModuleList.Count() > 1 Then
                    'System Testing
                    m_bIsSingleModule = False

                    DisplaySPScreen(PPSCREENS.ModuleListPP)

                    Return True
                ElseIf m_arrSPModuleList.Count() = 1 Then
                    'System Testing
                    m_bIsSingleModule = True
                    m_arrSPLineList.Clear()
                    m_SPModuleListInfo = m_arrSPModuleList.Item(0)
                    If Not (objAppContainer.objDataEngine.GetLineList(m_SPModuleListInfo.ModuleID, m_SPModuleListInfo.SequenceNumber, m_arrSPLineList)) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                    Else
                        DisplaySPScreen(PPSCREENS.LineListPP)
                        Return True
                    End If
                End If
            End If
            ' End If
            Return False
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Space Planner planner selection cannot be processed. Exception is: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PPSessionMgr ProcessProductSelectionForPlanner", Logger.LogLevel.RELEASE)
    End Function

    ''' <summary>
    ''' Processes the selection of any module from the list view
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessProductSelectionForModulePP() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter PPSessionMgr ProcessProductSelectionForModulePP", Logger.LogLevel.RELEASE)
        Dim iCounter As Integer = 0
        Dim bIsDataAvailable As Boolean = False
        With m_SPModuleListPP

            'Gets the selected list id from the listview
            Try

                If .lstView.SelectedIndices.Count > 0 Then
                    For iCounter = 0 To .lstView.Items.Count - 1
                        If .lstView.Items(iCounter).Selected Then
                            bIsDataAvailable = True
                            m_SPModuleListInfo = m_arrSPModuleList.Item(iCounter)
                            Exit For
                        End If
                    Next
                End If

                If bIsDataAvailable Then
                    m_arrSPLineList.Clear()
                    If Not (objAppContainer.objDataEngine.GetLineList(m_SPModuleListInfo.ModuleID, m_SPModuleListInfo.SequenceNumber, m_arrSPLineList)) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        Return False
                    Else
                        'Calculate CDV and append it with Boots code. This Boots Code is used to fetch the item description
                        'For Each obj As SPLineListInfo In m_arrSPLineList
                        '    obj.ItemCode = objAppContainer.objHelper.GenerateBCwithCDV(obj.ItemCode)
                        '    Dim strItemDescr = objAppContainer.objDataEngine.GetItemDescription(obj.ItemCode)
                        '    obj.ItemDescription = strItemDescr
                        'Next
                        DisplaySPScreen(PPSCREENS.LineListPP)
                        'System Testing
                        Return True
                    End If
                End If
                Return False
            Catch ex As Exception
#If RF Then
                If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                    Throw ex
                End If
#End If
                objAppContainer.objLogger.WriteAppLog("Space Planner line list selection cannot be processed. Exception is:" + ex.StackTrace, Logger.LogLevel.RELEASE)
                Return False
            End Try

        End With
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr ProcessProductSelectionForModulePP", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Processes the selection of any line item from the list view
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessProductSelectionForLineItemPP() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr ProcessProductSelectionForLineItemLP", Logger.LogLevel.RELEASE)
        Dim strListId As String = Nothing
        With m_SPLineListPP

            Dim iCounter As Integer = 0
            'Gets the selected list id from the listview
            Try
                Dim bIsDataAvailable As Boolean = False
                If .lstView.SelectedIndices.Count > 0 Then
                    For iCounter = 0 To .lstView.Items.Count - 1
                        If .lstView.Items(iCounter).Selected Then
                            bIsDataAvailable = True
                            strListId = .lstView.Items(iCounter).Text.Trim()
                            Exit For
                        End If
                    Next
                End If
                Dim strtemp As String = ""
                If bIsDataAvailable Then

                    strtemp = Trim(Mid(strListId, 1, 7))
                    'System Testing
                    If (objAppContainer.objHelper.ValidateBootsCode(strtemp)) Then
                        'IT - Shelf type sometimes succeeds boots code validation

                        If InStr(1, strtemp, " ") > 0 Then
                            Return False
                        End If
                        If ProcessDataRetrieval(strtemp) Then

                            'objAppContainer.objDataEngine.GetProductInfo(m_SPProductInfo)
                            'TODO Call Item Info module here
                            'If data is available then obtains the Product code from the list
                            'calls the overloaded method of the StartSession to display the item details
                            'ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.SPCEPLAN, strtemp, True)
                            'System Testing
                            Return True
                        Else

                            Return False
                        End If
                    Else
                        'MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"))
                        Return False
                    End If
                End If
            Catch ex As Exception
#If RF Then
                If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                    Throw ex
                End If
#End If
                objAppContainer.objLogger.WriteAppLog("Space Planner item selection cannot be processed. Exception is:" + ex.StackTrace, Logger.LogLevel.RELEASE)
                Return False
            End Try

        End With
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr ProcessProductSelectionForLineItemLP", Logger.LogLevel.RELEASE)

    End Function

    ''' <summary>
    ''' Processes the selection of any line item from the Print screen
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessSelectionForPrint() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr ProcessProductSelectionForPrint", Logger.LogLevel.RELEASE)
        Dim strListId As String = Nothing
        Dim bTemp As Boolean = False
        Try
            With m_SPPrintSEL
                Dim iCounter As Integer = 0
                'Gets the selected list id from the listview
                Dim bIsDataAvailable As Boolean = False
                If .lstView.SelectedIndices.Count > 0 Then
                    For iCounter = 0 To .lstView.Items.Count - 1
                        If .lstView.Items(iCounter).Selected Then
                            bIsDataAvailable = True
                            strListId = .lstView.Items(iCounter).Text
                            Exit For
                        End If
                    Next
                End If
                Dim strtemp As String = ""

                If bIsDataAvailable Then
                    'System Testing
                    strtemp = Trim(Mid(strListId, 1, 9))
                    If (String.Compare(strtemp, "Print All").Equals(0)) Then
                        If MessageBox.Show(MessageManager.GetInstance().GetMessage("M74"), "Info", _
                                MessageBoxButtons.YesNo, _
                                MessageBoxIcon.Question, _
                                MessageBoxDefaultButton.Button1) = DialogResult.Yes Then
                            If PrintProcessPRPForModules() Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M41"), "Info", _
                                                        MessageBoxButtons.OK, _
                                                        MessageBoxIcon.Asterisk, _
                                                        MessageBoxDefaultButton.Button1)
                                bTemp = True
                            End If
                        Else
                            bTemp = False
                        End If
                    Else
                        'IT Fixes : Passing seq no: instead of module ID
                        strtemp = m_SPModuleListInfo.SequenceNumber
                        strtemp.PadLeft(3, "0")
                        If MessageBox.Show(MessageManager.GetInstance().GetMessage("M73"), "Info", _
                                MessageBoxButtons.YesNo, _
                                MessageBoxIcon.Question, _
                                MessageBoxDefaultButton.Button1) = DialogResult.Yes Then
                            If (PrintProcessPRP(strtemp)) Then
                                'Integration testing

                                MessageBox.Show("SEL print request successfully queued for " + strListId.ToString(), "Info", _
                               MessageBoxButtons.OK, _
                               MessageBoxIcon.Asterisk, _
                               MessageBoxDefaultButton.Button1)

                            End If
                        End If

                    End If
                End If
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured Space Planner Process selection for Print: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr ProcessProductSelectionForPrint", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' This subroutine decides the form to be displayed based on the parametor passed.
    ''' </summary>
    ''' <param name="strFrmType"></param>
    ''' <remarks></remarks>
    Public Sub GetFormType(ByVal strFrmType As String)
        m_arrSPDeptCatList.Clear()
        Select Case strFrmType
            Case Macros.SP_CORE
                If (objAppContainer.objDataEngine.GetPOGCategoryList("Y", m_arrSPDeptCatList, True)) Then
                    DisplaySPScreen(PPSessionMgr.PPSCREENS.DeptCategory)
                Else
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M85"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Question, _
                                MessageBoxDefaultButton.Button1)
                End If
            Case Macros.SP_SALESPLAN
                If (objAppContainer.objDataEngine.GetPOGCategoryList("N", m_arrSPDeptCatList, True)) Then
                    DisplaySPScreen(PPSessionMgr.PPSCREENS.DeptCategory)
                Else
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M85"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Question, _
                                MessageBoxDefaultButton.Button1)
                End If
        End Select

    End Sub
    Public Sub PrintProcessPRT()
        Dim strBootsCode As String
        strBootsCode = m_IIProductInfo.BootsCode
        Dim objPRTData As PRTRecord = New PRTRecord()
        'objPRTData.strBootscode = objAppContainer.objHelper.GeneratePCwithCDV(strPdtCode)
        objPRTData.strBootscode = m_IIProductInfo.BootsCode
        If objAppContainer.bMobilePrinterAttachedAtSignon Then
            objPRTData.cIsMethod = Macros.PRINT_LOCAL
#If NRF Then
        If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBootsCode ,m_PSProductInfo )) Then
            objAppContainer.objLogger.WriteAppLog("Unable to retrieve label details from database", Logger.LogLevel.RELEASE)
        Else
        'call printer session function to print SELs or clearance labels.
        'Test:MobilePrintSessionManager.GetInstance.PrintLabels(1, m_LabelType, _m_PSProductInfo)
        End If

#ElseIf RF Then
            'send the request and get the detials required to print label.
            If Not objAppContainer.objDataEngine.GetLabelDetails(m_PSProductInfo) Then
                objAppContainer.objLogger.WriteAppLog("Unable to retrieve label details", Logger.LogLevel.RELEASE)
            Else
                'call printer session function to print SELs or clearance labels.
                'Test:MobilePrintSessionManager.GetInstance.PrintLabels(1, m_LabelType, _m_PSProductInfo)
            End If
#End If
        Else
            'Set print method to batch, if Mobile printer is not available
            objPRTData.cIsMethod = Macros.PRINT_BATCH
#If RF Then
            'Send PRT in the case of RF
            If Not (objAppContainer.objExportDataManager.CreatePRT(objPRTData.strBootscode)) Then
                objAppContainer.objLogger.WriteAppLog("Cannot Create PRT record at SP Start Session", Logger.LogLevel.RELEASE)
            End If
#End If
        End If
#If NRF Then
        m_QueuedSELList.Add(objPRTData)
#End If
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr PrintProcessPRT", Logger.LogLevel.RELEASE)
    End Sub
    Public Function PrintProcessPRP(ByVal strModSequence As String)
        Try
            Dim objPRPData As PRPRecord = New PRPRecord()
            'TODO Check whether all the values set are correct
            objPRPData.strPOGKey = m_SPPlannerListInfo.PlannerID
            objPRPData.strIsType = "0"
            objPRPData.strMODSequence = strModSequence
#If RF Then
            'anoop: Send a PRP message
            If Not (objAppContainer.objExportDataManager.CreatePRP(objPRPData)) Then
                objAppContainer.objLogger.WriteAppLog("Cannot Create PRP record at SP Start Session", Logger.LogLevel.RELEASE)
            End If
#ElseIf NRF Then
            m_QueuedPRPList.Add(objPRPData)
#End If
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured Space Planner Print Process PRP: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr PrintProcessPRP", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' To queue PRP request from module list screen for all modules
    ''' </summary>
    ''' <remarks></remarks>
    Public Function PrintProcessPRPForModules() As Boolean
        Try
            PrintProcessPRP("FFF")
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured Space Planner Print Process PRP for modules: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr PrintProcessPRPForModules", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Sets the View form to visible state
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetPrevScreenVisible()
        m_SPLineListPP.Visible = True
    End Sub

    'System Testing -
    ''' <summary>
    ''' Returns true if there is only onemodule for the planner
    ''' </summary>
    ''' <remarks></remarks>
    Public Function IsSingleModule() As Boolean
        Return m_bIsSingleModule
    End Function
    ''' <summary>
    ''' Checks whether the module was invoked from Item Info
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsInvokedFromItemInfo() As Boolean
        Return m_bInvokedFromItemInfo
    End Function

    'System Testing -
    ''' <summary>
    ''' Highlight the next instance of the same item in the line list screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function HighlightNextItem() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr HighlightNextItem", Logger.LogLevel.RELEASE)
        Dim iCount As Integer = 0
        Dim iCounter As Integer = 0
        Dim iIndex As Integer = 0
        Dim strtemp As String = ""
        Dim bRef As Boolean = False

        Try
            With m_SPLineListPP
                iCount = .lstView.Items.Count()

                For iTemp As Integer = 0 To iCount - 1
                    If (.lstView.Items(iTemp).BackColor = Color.Aquamarine) Then
                        iIndex = iTemp

                        Exit For
                    End If
                Next
                If (iIndex = iCount - 1) Then
                    'Integration testing - 720
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M64"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                    'Need not go to the next item as it is the last item
                    Exit Function
                Else

                    For iCounter = iIndex + 1 To .lstView.Items.Count - 1
                        strtemp = Trim(Mid(.lstView.Items(iCounter).Text, 1, 7))
                        If strtemp = strSearchItemBC Then
                            .lstView.Items(iIndex).BackColor = Color.White
                            .lstView.Items(iCounter).BackColor = Color.Aquamarine
                            'IT Internal
                            .lstView.EnsureVisible(iCounter)
                            'Integration testing

                            bRef = True
                            Exit For
                        End If
                    Next

                    'Integration Testing
                    If Not bRef Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M64"), "Info", _
                        MessageBoxButtons.OK, _
                        MessageBoxIcon.Asterisk, _
                        MessageBoxDefaultButton.Button1)
                    End If
                End If
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("PPSessionMgr - Exception in full HighlightNextItem", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PPSessionMgr HighlightNextItem", Logger.LogLevel.RELEASE)
    End Function
    'System Testing -
    ''' <summary>
    ''' Highlight the previous instance of the same item in the line list screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function HighlightPreviousItem() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr HighlightPreviousItem", Logger.LogLevel.RELEASE)
        Dim iCount As Integer = 0
        Dim iCounter As Integer = 0
        Dim iIndex As Integer = 0
        Dim strtemp As String = ""
        Dim bRef As Boolean = False
        Try
            With m_SPLineListPP
                iCount = .lstView.Items.Count()

                For iTemp As Integer = 0 To iCount - 1
                    If (.lstView.Items(iTemp).BackColor = Color.Aquamarine) Then
                        iIndex = iTemp
                        Exit For
                    End If
                Next
                If (iIndex = 0) Then
                    'Integration testing - 720
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M67"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                    'Need not go to the next item as it is the last item
                    Exit Function
                Else

                    For iCounter = iIndex - 1 To 0 Step -1
                        strtemp = Trim(Mid(.lstView.Items(iCounter).Text, 1, 7))
                        If strtemp = strSearchItemBC Then
                            .lstView.Items(iIndex).BackColor = Color.White
                            .lstView.Items(iCounter).BackColor = Color.Aquamarine
                            'IT Internal
                            .lstView.EnsureVisible(iCounter)
                            'Integration testing
                            bRef = True
                            Exit For
                        End If
                    Next
                    'Integration Testing
                    If Not bRef Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M67"), "Info", _
                        MessageBoxButtons.OK, _
                        MessageBoxIcon.Asterisk, _
                        MessageBoxDefaultButton.Button1)
                    End If
                End If
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("PPSessionMgr - Exception in full HighlightPreviousItem", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PPSessionMgr HighlightPreviousItem", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Writes the final set of data identified to the export data file
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteExportData() As Boolean

        Return True
    End Function

    ''' <summary>
    ''' Enum Class that defines all screens for Price Check module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum PPSCREENS
        LPHome
        LineListPP
        ModuleListPP
        DeptCategory
        PlannerListPP
        PrintSEL
        ItemDetails
    End Enum

    ''' <summary>
    ''' enumerates all scanable fields in PC Module
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum SCNFIELDS
        ProductCode
        SELCode
    End Enum


    ''' <summary>
    ''' enumerates the different Planner types in the Space Planning Module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum PLANTYPE
        PendingPlanner
    End Enum

End Class
#End If
