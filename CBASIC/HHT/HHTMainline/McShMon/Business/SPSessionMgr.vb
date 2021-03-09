Imports System.Data
Imports System.Text
''' ***************************************************************************
''' <fileName>SPSessionMgr.vb</fileName>
''' <summary>The Space Planning Container Class.
''' Implements all business logic and GUI navigation for Space Planning. 
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''*****************************************************************************
Public Class SPSessionMgr
    Private m_SPLPhome As frmSPLPHome
    Private m_SPLineListLP As frmSPLineListLP
    Private m_SPLineListSP As frmSPLineListSP
    Private m_SPLPDeptCat As frmSPLPDeptCat
    Private m_SPModuleListLP As frmSPModuleListLP
    Private m_SPModuleListSP As frmSPModuleListSP
    Private m_SPPlannerListLP As frmSPPlannerListLP
    Private m_SPPlannerListSP As frmSPPlannerListSP
    Private m_SPPrintSEL As frmSPPrintSEL
    Private m_SPSearchPlnrHome As frmSPSearchPlnrHome
    Private m_PlannerItemInfo As frmPlannerItemInfo

    Private m_PSProductInfo As PSProductInfo = Nothing 'anoop
    Private Shared m_SPSessionMgr As SPSessionMgr = Nothing
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
    Public Shared Function GetInstance() As SPSessionMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.SPCEPLAN
        If m_SPSessionMgr Is Nothing Then
            m_SPSessionMgr = New SPSessionMgr()
            Return m_SPSessionMgr
        Else
            Return m_SPSessionMgr
        End If
    End Function
#If RF Then
    ''' <summary>
    ''' Updates the Status bar of all the forms in the session manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBarMessage()
        Try
            m_SPLPhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SPLineListLP.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SPLineListSP.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SPLPDeptCat.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SPModuleListLP.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SPModuleListSP.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SPPlannerListLP.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SPPlannerListSP.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SPPrintSEL.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_SPSearchPlnrHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_PlannerItemInfo.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured, Trace: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Function to hold boolean status when planner is invoked from item info module.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property isPlannerInvokedFromItemInfo() As Boolean
        Get
            Return m_bInvokedFromItemInfo
        End Get
    End Property

#End If
    ''' <summary>
    ''' Initialises the Space Planning Session 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function StartSession(ByVal PlannerType As PLANTYPE, Optional ByVal bInvokedFromItemInfo As Boolean = False) As Boolean
        Try
            'Do all price check realated Initialisations here.
            m_bInvokedFromItemInfo = bInvokedFromItemInfo
#If RF Then
            If objAppContainer.objExportDataManager.CreatePGS Then
#End If
                m_SPLPhome = New frmSPLPHome()
                m_SPLineListLP = New frmSPLineListLP()
                m_SPLineListSP = New frmSPLineListSP()
                m_SPLPDeptCat = New frmSPLPDeptCat()
                m_SPModuleListLP = New frmSPModuleListLP()
                m_SPModuleListSP = New frmSPModuleListSP()
                m_SPPlannerListLP = New frmSPPlannerListLP()
                m_SPPlannerListSP = New frmSPPlannerListSP()
                m_SPPrintSEL = New frmSPPrintSEL()
                m_SPSearchPlnrHome = New frmSPSearchPlnrHome()
                m_PlannerItemInfo = New frmPlannerItemInfo()
                m_PSProductInfo = New PSProductInfo() 'anoop
                'Setting app Module back to the original module
                objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.SPCEPLAN
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
                m_bIsSingleModule = False
                m_strSEL = ""
                m_PreviousItem = ""
                m_strPlannerType = PlannerType
                m_ModulePriceCheck = New ModulePriceCheck()
                'Set label type
                m_LabelType = MobilePrintSessionManager.LabelType.STD 'anoop
            Return True
#If RF Then
            Else
                Return False
            End If
#End If
        Catch ex As Exception
#If RF Then
            If bInvokedFromItemInfo Then
                objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.ITEMINFO
            End If
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            ElseIf ex.Message = Macros.CONNECTION_REGAINED Then
                Cursor.Current = Cursors.Default
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Space Planner Session cannot be started" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Space Palnner Start Session", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Does all processing that needs to be done when a session ends
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
#If NRF Then
 Public Function EndSession() As Boolean
#ElseIf RF Then
    Public Function EndSession(Optional ByVal isConnectionLost As Boolean = False) As Boolean
#End If
        Try
#If NRF Then
            WriteExportData()
            'Set active module to none after quitting the module
            objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.NONE
#ElseIf RF Then
            If Not isConnectionLost Then
                If Not objAppContainer.objExportDataManager.CreatePGX() Then
                    objAppContainer.objLogger.WriteAppLog("Failed Sending a PGX ")
                    Return False
                End If
            End If
#End If
            'Save and data and perform all Exit Operations.
            'Close and Dispose all forms.
            m_PlannerItemInfo.Dispose()
            m_SPLineListLP.Dispose()
            m_SPLineListSP.Dispose()
            m_SPPrintSEL.Dispose()
            m_SPModuleListLP.Dispose()
            m_SPModuleListSP.Dispose()
            m_SPPlannerListLP.Dispose()
            m_SPPlannerListSP.Dispose()
            m_SPLPDeptCat.Dispose()
            m_SPSearchPlnrHome.Dispose()
            m_SPLPhome.Dispose()

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
            m_SPSessionMgr = Nothing
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
    Public Function DisplaySPScreen(ByVal ScreenName As SPSCREENS)
        Try
            Select Case ScreenName
                Case SPSCREENS.LPHome
                    m_SPLPhome.Invoke(New EventHandler(AddressOf DisplayLPHome))
                Case SPSCREENS.SPHome
                    m_SPSearchPlnrHome.Invoke(New EventHandler(AddressOf DisplaySPHome))
                Case SPSCREENS.DeptCategory
                    m_SPLPDeptCat.Invoke(New EventHandler(AddressOf DisplaySPDeptCat))
                Case SPSCREENS.LineListLP
                    m_SPLineListLP.Invoke(New EventHandler(AddressOf DisplaySPLineListLP))
                Case SPSCREENS.LineListSP
                    m_SPLineListSP.Invoke(New EventHandler(AddressOf DisplaySPLineListSP))
                Case SPSCREENS.ModuleListLP
                    m_SPModuleListLP.Invoke(New EventHandler(AddressOf DisplaySPModuleListLP))
                Case SPSCREENS.ModuleListSP
                    m_SPModuleListSP.Invoke(New EventHandler(AddressOf DisplaySPModuleListSP))
                Case SPSCREENS.PlannerListLP
                    m_SPPlannerListLP.Invoke(New EventHandler(AddressOf DisplaySPPlannerListLP))
                Case SPSCREENS.PlannerListSP
                    m_SPPlannerListSP.Invoke(New EventHandler(AddressOf DisplaySPPlannerListSP))
                Case SPSCREENS.PrintSEL
                    m_SPPrintSEL.Invoke(New EventHandler(AddressOf DisplaySPPrintSEL))
                Case SPSCREENS.ItemDetails
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
    ''' The Method handles scan the scan data returned form the barcode scanner.
    ''' This method implements the business logic to populate the data to the corresponding
    ''' UI element after validation.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="Type"></param>
    ''' <remarks></remarks>
    Public Sub HandleScanData(ByVal strBarcode As String, ByVal Type As BCType)
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr HandleScanData", Logger.LogLevel.RELEASE)
        m_SPSearchPlnrHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        Try
            Select Case Type
                Case BCType.EAN
                    If Not (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)

                        m_SPSearchPlnrHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)


                        DisplaySPScreen(SPSCREENS.SPHome)
                        'System Testing
                        Exit Sub
                    Else
                        strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                        strSearchItemBC = objAppContainer.objDataEngine.GetBootsCode(strBarcode)
                        m_arrSPPlannerList.Clear()
                        If Not (objAppContainer.objDataEngine.GetPlannerListUsingPC(strBarcode, False, m_arrSPPlannerList)) Then
#If NRF Then
                            'DARWIN Changes
                            If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                                'DARWIN converting database to Base Barcode
                                strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                                If Not (objAppContainer.objDataEngine.GetPlannerListUsingPC(strBarcode, False, m_arrSPPlannerList)) Then
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                            MessageBoxButtons.OK, _
                                            MessageBoxIcon.Asterisk, _
                                            MessageBoxDefaultButton.Button1)
                                    m_SPSearchPlnrHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                    DisplaySPScreen(SPSCREENS.SPHome)
                                    Exit Sub
                                End If
                            Else
#End If
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                            DisplaySPScreen(SPSCREENS.SPHome)

                            m_SPSearchPlnrHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Exit Sub
#If NRF Then
                            End If
#End If
                        End If
                        m_strSEL = ""
                    End If
                Case BCType.ManualEntry
                    Dim strBootsCode As String = ""
                    If strBarcode.Length < 8 Then
                        If Not (objAppContainer.objHelper.ValidateBootsCode(strBarcode)) Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)

                            m_SPSearchPlnrHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            'Display search planner home screen if item code is not valid.
                            DisplaySPScreen(SPSCREENS.SPHome)
                            Exit Sub
                        Else
                            strSearchItemBC = strBarcode
                            m_arrSPPlannerList.Clear()
                            If Not (objAppContainer.objDataEngine.GetPlannerListUsingBC(strBarcode, False, m_arrSPPlannerList)) Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                                m_SPSearchPlnrHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                'Display search planner home screen in case of error in getting item details.
                                DisplaySPScreen(SPSCREENS.SPHome)
                                Exit Sub
                            End If
                        End If
                    Else
                        If (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                            strBarcode = strBarcode.PadLeft(13, "0")
                            strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                            strSearchItemBC = objAppContainer.objDataEngine.GetBootsCode(strBarcode)
                            m_arrSPPlannerList.Clear()
                            If Not (objAppContainer.objDataEngine.GetPlannerListUsingPC(strBarcode, False, m_arrSPPlannerList)) Then
#If NRF Then
                                'DARWIN Changes
                                If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                                    'DARWIN converting database to Base Barcode
                                    strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                                    If Not (objAppContainer.objDataEngine.GetPlannerListUsingPC(strBarcode, False, m_arrSPPlannerList)) Then
                                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                                MessageBoxButtons.OK, _
                                                MessageBoxIcon.Asterisk, _
                                                MessageBoxDefaultButton.Button1)
                                        m_SPSearchPlnrHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                        DisplaySPScreen(SPSCREENS.SPHome)
                                        Exit Sub
                                    End If
                                Else
#End If
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                        MessageBoxButtons.OK, _
                                        MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)

                                m_SPSearchPlnrHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                'Display search planner home screen in case of error in getting item details.
                                DisplaySPScreen(SPSCREENS.SPHome)
                                Exit Sub
                            End If
#If NRF Then
                            End If
#End If
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                            MessageBoxDefaultButton.Button1)

                            m_SPSearchPlnrHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            'Display search planner home screen if scanned EAN is invalid.
                            DisplaySPScreen(SPSCREENS.SPHome)
                            Exit Sub
                        End If
                    End If
                Case BCType.SEL
                    If objAppContainer.objHelper.ValidateSEL(strBarcode) Then
                        Dim strBootsCode As String = ""
                        Dim strPdtCode As String = ""
                        objAppContainer.objHelper.GetBootsCodeFromSEL(strBarcode, strBootsCode)
                        strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
                        strSearchItemBC = strBootsCode
                        m_arrSPPlannerList.Clear()
                        If Not (objAppContainer.objDataEngine.GetPlannerListUsingBC(strBootsCode, False, m_arrSPPlannerList)) Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)

                            m_SPSearchPlnrHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            'Display search planner home screen in case of error in getting item details.
                            DisplaySPScreen(SPSCREENS.SPHome)
                            Exit Sub
                        End If
                    Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M4"), "Error", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)

                        m_SPSearchPlnrHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        'Display search planner home screen if an invalid SEL is scanned.
                        DisplaySPScreen(SPSCREENS.SPHome)
                        Exit Sub
                    End If
            End Select
            'Scanning is possible only fron Search Planner Home screen. So pass screen ID as SP
            If m_arrSPPlannerList.Count > 0 Then
                'sort the plannet list
                Dim iComp As PogCompare = New PogCompare()
                m_arrSPPlannerList.Sort(iComp)
                'Display module list screen.
                DisplaySPScreen(SPSessionMgr.SPSCREENS.PlannerListSP)
            Else
                MessageBox.Show("Planners not available for the item scanned.")
                'Display search planner home screen if there is no planner information available
                'for the item scanned.
                DisplaySPScreen(SPSCREENS.SPHome)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("SPSessionMgr - Exception in full HandleScanData", Logger.LogLevel.RELEASE)
        Finally
            If Not m_SPSearchPlnrHome Is Nothing Then
                If Not m_SPSearchPlnrHome.objProdSEL.txtProduct.IsDisposed AndAlso Not m_SPSearchPlnrHome.objProdSEL.txtSEL.IsDisposed Then
                    m_SPSearchPlnrHome.objProdSEL.txtProduct.Text = ""
                    m_SPSearchPlnrHome.objProdSEL.txtSEL.Text = ""
                End If
            End If
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr HandleScanData", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' This subroutine sets the home screen display of Live Planner
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayLPHome(ByVal o As Object, ByVal e As EventArgs)
        With m_SPLPhome
            'Sets the store id and active data time to the status bar
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            If m_strPlannerType = PLANTYPE.PendingPlanner Then
                .lblLivePlnr.Text = "Pending Planner"
            End If
            .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' This subroutine sets the home screen display of Search Planner
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplaySPHome(ByVal o As Object, ByVal e As EventArgs)
        With m_SPSearchPlnrHome
            'Sets the store id and active data time to the status bar
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            .objProdSEL.txtProduct.Text = ""
            .objProdSEL.txtSEL.Text = ""
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
            With m_SPLPDeptCat
                .lstView.Columns.Clear()
                .lstView.Items.Clear()
                .lstView.Columns.Add("", 235 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                '.lstView.Items.Add((New ListViewItem(New String() {"Department/Category"})))
                'Populates the Department/Category List
                PopulateDeptList()
                If m_strPlannerType = PLANTYPE.PendingPlanner Then
                    .Label1.Text = "Pending Planner"
                    .Text = "Pending Planner"
                End If
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                .Visible = True
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
        With m_SPLPDeptCat
            Try
                If m_strPlannerType = PLANTYPE.PendingPlanner Then
                    .Label1.Text = "Pending Planner"
                    .Text = "Pending Planner"
                End If
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
                    'System Testing bug fix for item not on planner.
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                    Exit Sub
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
            objAppContainer.objLogger.WriteAppLog("SPSessionMgr - Exception in full DisplayPlannner", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr DisplayPlanner", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' This subroutine displays the Line List screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplaySPLineListLP(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr DisplaySPLineListLP", Logger.LogLevel.RELEASE)
        Dim iCount As Integer = 0
        Dim iIndex As Integer = 0
        Try
            With m_SPLineListLP
                .lstView.Columns.Clear()
                .lstView.Items.Clear()
                .lblModule.Text() = objAppContainer.objHelper.FormatEscapeSequence(m_SPPlannerListInfo.Description.ToString())
                If m_strPlannerType = PLANTYPE.PendingPlanner Then
                    .Text = "Pending Planner"
                End If
                'Defect:-117 START: Set the rebuild date for planner.
                'Formats the date to display in YY/MM/DD format
                Dim strRebuildDate As String = ""
                strRebuildDate = m_SPPlannerListInfo.RebuildDate.Split(" ")(0)
                'SFA DEF#827 -Commented 
                'Defect:-117
                'If strRebuildDate.Length = 8 Then
                'strRebuildDate = strRebuildDate.Substring(2, 6)
#If RF Then
                If Not strRebuildDate.Equals("") Then
                    Dim strBuilder As StringBuilder = New StringBuilder()
                    strBuilder.Append(strRebuildDate.Substring(6, 2))
                    strBuilder.Append("/")
                    strBuilder.Append(strRebuildDate.Substring(4, 2))
                    strBuilder.Append("/")
                    strBuilder.Append(strRebuildDate.Substring(2, 2))

                    strRebuildDate = strBuilder.ToString()
                End If
#End If
                    .Label1.Text = strRebuildDate
                    'End If
                    'Defect:-117: END
                    .lstView.Columns.Add("  Item       Description", 180 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstView.Columns.Add("Facing", 50 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    'Add the module description.
                    .lstView.Items.Add((New ListViewItem(New String() {m_SPModuleListInfo.Description, "FC"})))
                    While m_arrSPLineList.Count() > 0
                        Dim obj As SPLineListInfo = Nothing
                        obj = m_arrSPLineList(iCount)
                        If obj.NotchNumber = "" Then
                            obj.NotchNumber = "0"
                        End If
                        ' iCount += 1
                        Dim query = From obj1 As SPLineListInfo In m_arrSPLineList _
                                    Where obj1.ShelfNumber = obj.ShelfNumber _
                                    Order By obj1.ModuleSeqNumber Ascending, obj1.ShelfNumber Ascending _
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
                                                            obj1.ItemDescription, obj1.FaceCount})))

                            'm_arrSPLineList.Remove(obj1)
                            arrlist.Add(obj1)

                        Next
                        For Each obj1 As SPLineListInfo In arrlist
                            m_arrSPLineList.Remove(obj1)
                        Next
                    End While
                    'Sets the store id and active data time to the status bar
                    .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                    .Visible = True
                    .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured Space Planner Display line list Screen: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr DisplaySPLineListLP", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' This subroutine displays the Line List screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplaySPLineListSP(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr DisplaySPLineListSP", Logger.LogLevel.RELEASE)
        Dim iCount As Integer = 0
        Dim iIndex As Integer = 0
        Dim iSelectedRow As Integer = 0
        Dim bHighlighted As Boolean = False
        Try
            'Before adding the items to the list view sort the items in array.

            With m_SPLineListSP
                .lstView.Columns.Clear()
                .lstView.Items.Clear()
                .lblModule.Text() = objAppContainer.objHelper.FormatEscapeSequence(m_SPModuleListInfo.Description.ToString())
                .Btn_Next_small1.Visible = True
                'Defect:-117 START: Set the rebuild date for planner.
                'Formats the date to display in YY/MM/DD format
                Dim strRebuildDate As String = ""
                strRebuildDate = m_SPPlannerListInfo.RebuildDate.Split(" ")(0)
                'SFA DEF#827 -Commented 
                ''Defect:-117
                'If strRebuildDate.Length = 8 Then
                'strRebuildDate = strRebuildDate.Substring(2, 6)
#If RF Then
                If Not strRebuildDate.Equals("") Then
                    Dim strBuilder As StringBuilder = New StringBuilder()
                    strBuilder.Append(strRebuildDate.Substring(6, 2))
                    strBuilder.Append("/")
                    strBuilder.Append(strRebuildDate.Substring(4, 2))
                    strBuilder.Append("/")
                    strBuilder.Append(strRebuildDate.Substring(2, 2))
                    strRebuildDate = strBuilder.ToString()
                End If
#End If
                    .Label1.Text = strRebuildDate
                    'End If
                    'Defect:-117: END
                    .lstView.Columns.Add("  Item       Description", 180 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstView.Columns.Add("Facing", 50 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstView.Items.Add( _
                            (New ListViewItem(New String() {m_SPModuleListInfo.Description, _
                                                                    "FC"})))
                    'System Testing
                    iIndex += 1
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
                        'System Testing
                        iIndex += 1
                        Dim arrlist As New ArrayList
                        For Each obj1 As SPLineListInfo In query
                            .lstView.Items.Add( _
                            (New ListViewItem(New String() {"  " + obj1.ItemCode + " " + _
                                                            obj1.ItemDescription, obj1.FaceCount})))
                            If obj1.ItemCode = strSearchItemBC And bHighlighted = False Then
                                'TODO Automatic highlight of the scanned item
                                .lstView.Items(iIndex).BackColor = Color.Aquamarine
                                '.lstView.Items(iIndex).Focused = True
                                'IT Internal
                                '.lstView.Items(iIndex).Selected = True
                                'If (iIndex + 1) < .lstView.Items.Count Then
                                '    .lstView.EnsureVisible(iIndex + ((.lstView.Items.Count - iIndex) / 2))
                                'Else
                                '    .lstView.EnsureVisible(iIndex)
                                'End If
                                iSelectedRow = iIndex
                                '.lstView.EnsureVisible(iIndex + ((.lstView.Items.Count - iIndex) / 2))
                                bHighlighted = True
                                '    iIndex += 1
                                'Else

                            End If
                            iIndex += 1
                            arrlist.Add(obj1)

                        Next
                        For Each obj1 As SPLineListInfo In arrlist
                            m_arrSPLineList.Remove(obj1)
                        Next
                    End While
                    'If (iSelectedRow + 3) < .lstView.Items.Count Then
                    '    .lstView.EnsureVisible(iSelectedRow + 3)
                    'ElseIf (iSelectedRow + 2) < .lstView.Items.Count Then
                    If (iSelectedRow + 2) < .lstView.Items.Count Then
                        .lstView.EnsureVisible(iSelectedRow + 2)
                    ElseIf (iSelectedRow + 1) < .lstView.Items.Count Then
                        .lstView.EnsureVisible(iSelectedRow + 1)
                    Else
                        .lstView.EnsureVisible(iSelectedRow)
                    End If
                    'Sets the store id and active data time to the status bar

                    .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                    'DEFECT FIX -BTCPR00004154(RF Mode :: Planner :: 
                    'Rebuild date not required in case of search planner)
#If RF Then
                    .Label1.Visible = False
                    .lblRebuildDate.Visible = False

#End If

                    .Visible = True
                    .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("SPSessionMgr - Exception in full DisplaySPLineListSP", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr DisplaySPLineListSP", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' This subroutine displays the Module List screen for live planner
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplaySPModuleListLP(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr DisplaySPModuleListLP", Logger.LogLevel.RELEASE)
        Dim strCount As String = "01"
        Try
            With m_SPModuleListLP
                .lstView.Columns.Clear()
                .lstView.Items.Clear()
                .lblPlanner.Text() = objAppContainer.objHelper.FormatEscapeSequence(m_SPPlannerListInfo.Description.ToString())
                If m_strPlannerType = PLANTYPE.PendingPlanner Then
                    .Text = "Pending Planner"
                End If
                'Formats the date to display in YY/MM/DD format
                Dim strRebuildDate As String = ""

                strRebuildDate = m_SPPlannerListInfo.RebuildDate.Split(" ")(0)

                'SFA DEF#827 -Commented 
                'IT External - 834
                'If strRebuildDate.Length = 8 Then
#If RF Then
                If Not strRebuildDate.Equals("") Then
                    Dim strBuilder As StringBuilder = New StringBuilder()
                    strBuilder.Append(strRebuildDate.Substring(6, 2))
                    strBuilder.Append("/")
                    strBuilder.Append(strRebuildDate.Substring(4, 2))
                    strBuilder.Append("/")
                    strBuilder.Append(strRebuildDate.Substring(2, 2))

                    strRebuildDate = strBuilder.ToString()
                End If
#End If
                    .lblDate.Text = strRebuildDate
                    'End If
                    .lstView.Columns.Add("", 30 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstView.Columns.Add("", 150 * objAppContainer.iOffSet, HorizontalAlignment.Left)
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
                    .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                    .Visible = True
                    .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("SPSessionMgr - Exception in full DisplaySPModuleListLP", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr DisplaySPModuleListLP", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' This subroutine displays the Module List scren for search planner
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplaySPModuleListSP(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr DisplaySPModuleListSP", Logger.LogLevel.RELEASE)
        Dim strCount As String = "01"
        Try
            With m_SPModuleListSP
                .lstView.Columns.Clear()
                .lstView.Items.Clear()
                .lblPlanner.Text() = objAppContainer.objHelper.FormatEscapeSequence(m_SPPlannerListInfo.Description.ToString())

                'Formats the date to display in YY/MM/DD format
                Dim strRebuildDate As String = ""
                strRebuildDate = m_SPPlannerListInfo.RebuildDate.Split(" ")(0)
                'SFA DEF#827 -Commented 
                'IT External - 834
                'If strRebuildDate.Length = 8 Then

                '    strRebuildDate = strRebuildDate.Substring(2, 6)
#If RF Then
                Dim strBuilder As StringBuilder = New StringBuilder()
                strBuilder.Append(strRebuildDate.Substring(6, 2))
                strBuilder.Append("/")
                strBuilder.Append(strRebuildDate.Substring(4, 2))
                strBuilder.Append("/")
                strBuilder.Append(strRebuildDate.Substring(2, 2))

                strRebuildDate = strBuilder.ToString()
#End If
                .lblDate.Text = strRebuildDate
                'End If
                .lstView.Columns.Add("", 30 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                .lstView.Columns.Add("", 150 * objAppContainer.iOffSet, HorizontalAlignment.Left)
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
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("SPSessionMgr - Exception in full DisplaySPModuleListSP", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr DisplaySPModuleListSP", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' This subroutine displays the Planner List screen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DisplaySPPlannerListLP(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr DisplaySPPlannerListLP", Logger.LogLevel.RELEASE)
        Try
            With m_SPPlannerListLP
                .lblDept.Text() = objAppContainer.objHelper.FormatEscapeSequence(m_SPDeptInfo.Description.ToString())
                .lstView.Columns.Clear()
                .lstView.Items.Clear()
                .lstView.Columns.Add("", 120 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                .lstView.Columns.Add("", 80 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                .lstView.Columns.Add("", 20 * objAppContainer.iOffSet, HorizontalAlignment.Right)
                If m_strPlannerType = PLANTYPE.PendingPlanner Then
                    .Text = "Pending Planner"
                End If
                For Each objPlanner As PlannerInfo In m_arrSPPlannerList
                    'Formats the date to display in YY/MM/DD format
                    Dim strRebuildDate As String = ""
                    strRebuildDate = objPlanner.RebuildDate.Split(" ")(0)
                    'SFA DEF#827 -Commented 
                    'IT External - 834
                    'If strRebuildDate.Length = 8 Then
                    'strRebuildDate = strRebuildDate.Substring(2, 6)
#If RF Then
                    If Not strRebuildDate.Equals("") Then
                        Dim strBuilder As StringBuilder = New StringBuilder()
                        strBuilder.Append(strRebuildDate.Substring(6, 2))
                        strBuilder.Append("/")
                        strBuilder.Append(strRebuildDate.Substring(4, 2))
                        strBuilder.Append("/")
                        strBuilder.Append(strRebuildDate.Substring(2, 2))

                        strRebuildDate = strBuilder.ToString()
                    End If
#End If
                        'End If
                    .lstView.Items.Add( _
                    (New ListViewItem(New String() {objPlanner.Description.Trim(), strRebuildDate.Trim(), objPlanner.RepeatCount})))
                Next
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured Space Planner Display planner list Screen: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr DisplaySPPlanerListLP", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' This subroutine displays the Planner List screen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DisplaySPPlannerListSP(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr DisplaySPPlannerListSP", Logger.LogLevel.RELEASE)
        Try
            With m_SPPlannerListSP
                .lstView.Columns.Clear()
                .lstView.Items.Clear()
                .lstView.Columns.Add("", 120 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                .lstView.Columns.Add("", 80 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                .lstView.Columns.Add("", 20 * objAppContainer.iOffSet, HorizontalAlignment.Right)
                'TODO DB fn to get dept info for a boots code
                '  .lblDept.Text() = objAppContainer.objHelper.FormatEscapeSequence(m_SPDeptInfo.Description.ToString())
                For Each objPlanner As PlannerInfo In m_arrSPPlannerList
                    'Formats the date to display in YY/MM/DD format
                    Dim strRebuildDate As String = ""

                    strRebuildDate = objPlanner.RebuildDate.Split(" ")(0)
                    'SFA DEF#827 -Commented 
                    'If strRebuildDate.Length = 8 Then
                    '    strRebuildDate = strRebuildDate.Substring(2, 6)
#If RF Then
                    If Not strRebuildDate.Equals("") Then
                        Dim strBuilder As StringBuilder = New StringBuilder()
                        strBuilder.Append(strRebuildDate.Substring(6, 2))
                        strBuilder.Append("/")
                        strBuilder.Append(strRebuildDate.Substring(4, 2))
                        strBuilder.Append("/")
                        strBuilder.Append(strRebuildDate.Substring(2, 2))
                        strRebuildDate = strBuilder.ToString()
                    End If
#End If
                        'End If
                    .lstView.Items.Add( _
                    (New ListViewItem(New String() {objPlanner.Description.Trim(), strRebuildDate.Trim(), objPlanner.RepeatCount})))

                Next
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                'DEFECT FIX -BTCPR00004154(RF Mode :: Planner :: 
                'Rebuild date not required in case of search planner)
#If RF Then
                .lblRebuildDate.Visible = False
#End If
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("SPSessionMgr - Exception in full DisplaySPPlannerListSP", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr DisplaySPPlannerListSP", Logger.LogLevel.RELEASE)
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
                .lstView.Columns.Add("", 120 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                Dim tmpLst As ListViewItem = New ListViewItem(New String() {"Print All"})
                tmpLst.Tag = "FFF"
                .lstView.Items.Add(tmpLst)
                For Each objModule As ModuleInfo In m_arrSPModuleList
                    tmpLst = New ListViewItem(New String() {objModule.Description})
                    tmpLst.Tag = objModule.SequenceNumber
                    .lstView.Items.Add(tmpLst)
                Next
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("SPSessionMgr - Exception in full DisplaySPPrintSEL", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr DisplaySPPrintSEL", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Displays the Planner Item Details Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayPlannerItemDetails(ByVal o As Object, ByVal e As EventArgs)
        Dim objDescriptionArray As ArrayList = New ArrayList()
        Dim strCurrency As String = ""
        Dim strBarcode As String = ""
        'Dim strDealNo As String
        Try
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(m_IIProductInfo.Description)

            strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(m_IIProductInfo.ProductCode)
            If m_IIProductInfo.RedemptionFlag = Macros.Redemption_True Then
                m_PlannerItemInfo.lblRedeemableText.Text = "Yes"
            Else
                m_PlannerItemInfo.lblRedeemableText.Text = "No"
            End If

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
                'Fix for-When Item Info invoked from planner the label Start of day stock figure is displayed
#If RF Then
                .lblStockFig.Text = "Stock Figure:"
                'Fix for-The Stock fig value displayed is empty.
                If m_IIProductInfo.TSF = "      " Then
                    m_IIProductInfo.TSF = 0
                End If
#ElseIf NRF Then
                 .lblStockFig.Text = "Start of Stock Figure:"
#End If
                .lblStockText.Text = m_IIProductInfo.TSF
                Displaydeal(.cmbDeal)
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("SPSessionMgr - Exception in full DisplayPlannnerItemDetails", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr DisplayPlannerItemDetails", Logger.LogLevel.RELEASE)
    End Sub
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
            strDealNo = m_IIProductInfo.DealList
            Dim arrDealDetails() As String = Nothing
            Dim arrDealType As ArrayList = Nothing
            'If there is no deal present then hide the drop down list control.
            If strDealNo.Equals("") Then
                objDeal.Visible = False
                With m_PlannerItemInfo
                    .lblDealHeader.Visible = False
                End With
            Else
                'Split the deals and add to an array.
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
                'Get the deal details for each deal the item is linked.
                For iCounter = 0 To iLen - 1
                    Dim objDealDetails As DQRRECORD = New DQRRECORD()
                    If objAppContainer.objDataEngine.GetDealDetails(arrDealDetails(iCounter), objDealDetails) Then
                        Dim strTemp As String
                        strTemp = "DealMsg_" + objDealDetails.strDealType.PadLeft(2, "0")
                        objDeal.Items.Add(ConfigDataMgr.GetInstance.GetParam(strTemp))
                        m_arrDealDataList.Add(objDealDetails)
                    End If
                Next
                'System testing
                If objDeal.Items.Count() > 0 Then
                    objDeal.Items.Insert(0, "Select")
                    objDeal.Enabled = True
                    objDeal.SelectedIndex = 0
                ElseIf objDeal.Items.Count() = 0 Then
                    objDeal.Visible = False
                    m_PlannerItemInfo.lblDealHeader.Visible = False
                End If
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
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
        Dim objDealDetails As DQRRECORD = New DQRRECORD()
        objAppContainer.objLogger.WriteAppLog("Enter II DisplayDealInfo", Logger.LogLevel.RELEASE)
        Try
            With m_PlannerItemInfo
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
                If .cmbDeal.Enabled = True Then
                    If Not .cmbDeal.SelectedItem.Equals("Select") Then
                        If m_arrDealDataList.Count > 0 Then
                            objDealDetails = m_arrDealDataList.Item(.cmbDeal.SelectedIndex - 1)
                        Else
                            objDealDetails = m_arrDealDataList.Item(.cmbDeal.SelectedIndex)
                        End If

                        Dim strBuilder As StringBuilder = New StringBuilder()
                        Dim strDate As String
                        strBuilder.Append("This Item is on the following Active Promotion")
                        strBuilder.Append(Environment.NewLine + "---------------------------" + Environment.NewLine)
#If NRF Then
                        strBuilder.Append(.cmbDeal.SelectedItem.ToString())
#ElseIf RF Then
                        strBuilder.Append(objDealDetails.strDealDesc)
#End If
                        strDate = objDealDetails.strStartDate
                        strDate = strDate.Substring(6, 2) + "-" + strDate.Substring(4, 2) + "-" + strDate.Substring(0, 4)
                        strBuilder.Append(Environment.NewLine + "Start Date: " + strDate)
                        strDate = objDealDetails.strEndDate
                        strDate = strDate.Substring(6, 2) + "-" + strDate.Substring(4, 2) + "-" + strDate.Substring(0, 4)
                        strBuilder.Append(Environment.NewLine + "End Date: " + strDate)
                        strBuilder.Append(Environment.NewLine + "---------------------------" + Environment.NewLine)
                        strBuilder.Append("Check ALL Shelf Locations." + Environment.NewLine)
                        strBuilder.Append("Check Show Material.")
                        MessageBox.Show(strBuilder.ToString(), "Information")
                    End If
                End If
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured DisplaydealInfo. Exception is: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit II DisplayDealInfo", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Function to get item details when an item is selected in line list screen
    ''' and then procede to display the item info screen.
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
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
            DisplaySPScreen(SPSCREENS.ItemDetails)
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
        'FIX for Pending Planner not displayed
        Dim bIsPending As Boolean = False

        If m_strPlannerType = PLANTYPE.PendingPlanner Then
            bIsPending = True
        Else
            bIsPending = False
        End If
        With m_SPLPDeptCat
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
#If NRF Then
                        strListId = m_SPDeptInfo.CategoryID
#ElseIf RF Then
                            strListId = m_SPDeptInfo.POINTER.ToString()
#End If
                            Exit For
                        End If
                    Next
                End If

                'Processes the data to obtain the planner list
                If bIsDataAvailable Then
                    m_arrSPPlannerList.Clear()
                    If Not (objAppContainer.objDataEngine.GetPlannerListForCategory(strListId, m_arrSPPlannerList, bIsPending)) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M70"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                    Else
                        'Sort the planner list.
                        Dim iComp As PogCompare = New PogCompare()
                        m_arrSPPlannerList.Sort(iComp)
                        'Display the planner list screen.
                        DisplaySPScreen(SPSCREENS.PlannerListLP)
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
            If strPlannerType.Equals(Macros.SEARCH_PLANNER) Then
                With m_SPPlannerListSP

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
            ElseIf strPlannerType.Equals(Macros.LIVE_PLANNER) Then
                With m_SPPlannerListLP

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
            End If
            'gets the category id from the category array list
            'Dim strCatID As String = Nothing
            If Not bIsDataAvailable Then
                Return False
            End If
            ' If Not strPlannerID Is Nothing Then
            m_arrSPModuleList.Clear()
            If strPlannerType.Equals(Macros.SEARCH_PLANNER) Then
                bStatus = objAppContainer.objDataEngine.GetModuleList(m_SPPlannerListInfo.PlannerID, _
                                                                      strSearchItemBC, _
                                                                      m_arrSPModuleList)
            ElseIf strPlannerType.Equals(Macros.LIVE_PLANNER) Then
                bStatus = objAppContainer.objDataEngine.GetModuleList(m_SPPlannerListInfo.PlannerID, _
                                                                      m_arrSPModuleList)
            End If
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
                    If strPlannerType.Equals(Macros.LIVE_PLANNER) Then
                        DisplaySPScreen(SPSCREENS.ModuleListLP)
                    ElseIf strPlannerType.Equals(Macros.SEARCH_PLANNER) Then
                        DisplaySPScreen(SPSCREENS.ModuleListSP)
                    End If
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
                        If strPlannerType.Equals(Macros.LIVE_PLANNER) Then
                            DisplaySPScreen(SPSCREENS.LineListLP)
                        ElseIf strPlannerType.Equals(Macros.SEARCH_PLANNER) Then
                            DisplaySPScreen(SPSCREENS.LineListSP)
                        End If
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
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr ProcessProductSelectionForPlanner", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Processes the selection of any module from the list view
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessProductSelectionForModuleSP() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr ProcessProductSelectionForModuleSP", Logger.LogLevel.RELEASE)
        Dim iCounter As Integer = 0
        Dim bIsDataAvailable As Boolean = False
        With m_SPModuleListSP

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
                        DisplaySPScreen(SPSCREENS.LineListSP)
                        Return True
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
                objAppContainer.objLogger.WriteAppLog("Space Planner line list selection cannot be processed.Exception is:" + ex.StackTrace, Logger.LogLevel.RELEASE)
                Return False
            End Try

        End With
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr ProcessProductSelectionForModuleSP", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Processes the selection of any module from the list view
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessProductSelectionForModuleLP() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr ProcessProductSelectionForModuleLP", Logger.LogLevel.RELEASE)
        Dim iCounter As Integer = 0
        Dim bIsDataAvailable As Boolean = False
        With m_SPModuleListLP

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
                        'Display line list screen.
                        DisplaySPScreen(SPSCREENS.LineListLP)
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
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr ProcessProductSelectionForModuleLP", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Processes the selection of any line item from the list view
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessProductSelectionForLineItemLP() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr ProcessProductSelectionForLineItemLP", Logger.LogLevel.RELEASE)
        Dim strListId As String = Nothing
        With m_SPLineListLP

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
    ''' Processes the selection of any line item from the list view
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessProductSelectionForLineItemSP() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr ProcessProductSelectionForLineItemSP", Logger.LogLevel.RELEASE)
        Dim strListId As String = Nothing
        With m_SPLineListSP
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
                            Return True
                        Else
                            Return False
                        End If
                    Else
                        Return False
                    End If
                End If
            Catch ex As Exception
#If RF Then
                If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                    Throw ex
                End If
#End If
                objAppContainer.objLogger.WriteAppLog("Space Planner item selection cannot be processed" + ex.StackTrace, Logger.LogLevel.RELEASE)
                Return False
            End Try

        End With

        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr ProcessProductSelectionForLineItemSP", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Processes the selection of any line item from the Print screen
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessSelectionForPrint() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr ProcessProductSelectionForPrint", Logger.LogLevel.RELEASE)
        Dim strListId As String = Nothing
        Dim strSeqNumber As String = ""
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
                            strSeqNumber = .lstView.Items(iCounter).Tag.ToString()
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
                        strSeqNumber = strSeqNumber.PadLeft(3, "0")
                        If MessageBox.Show(MessageManager.GetInstance().GetMessage("M73"), "Info", _
                                MessageBoxButtons.YesNo, _
                                MessageBoxIcon.Question, _
                                MessageBoxDefaultButton.Button1) = DialogResult.Yes Then
                            If (PrintProcessPRP(strSeqNumber)) Then
                                'Integration testing
                                If (m_strPlannerType = PLANTYPE.LivePlanner) Then
                                    MessageBox.Show("SEL print request successfully queued for " + strListId.ToString(), "Info", _
                                   MessageBoxButtons.OK, _
                                   MessageBoxIcon.Asterisk, _
                                   MessageBoxDefaultButton.Button1)
                                ElseIf (m_strPlannerType = PLANTYPE.SearchPlanner) Then
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M42"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                                End If
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
        'FIX for Pending Planner not displayed
        Dim bIsPending As Boolean = False

        If m_strPlannerType = PLANTYPE.PendingPlanner Then
            bIsPending = True
        Else
            bIsPending = False
        End If
        Select Case strFrmType
            Case Macros.SP_CORE
                If (objAppContainer.objDataEngine.GetPOGCategoryList("Y", m_arrSPDeptCatList, bIsPending)) Then
                    DisplaySPScreen(SPSessionMgr.SPSCREENS.DeptCategory)
                Else
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M85"), "Info", _
                        MessageBoxButtons.OK, _
                        MessageBoxIcon.Question, _
                        MessageBoxDefaultButton.Button1)
                End If
            Case Macros.SP_SALESPLAN
                If (objAppContainer.objDataEngine.GetPOGCategoryList("N", m_arrSPDeptCatList, bIsPending)) Then
                    DisplaySPScreen(SPSessionMgr.SPSCREENS.DeptCategory)
                Else
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M85"), "Info", _
                        MessageBoxButtons.OK, _
                        MessageBoxIcon.Question, _
                        MessageBoxDefaultButton.Button1)
                End If
        End Select

    End Sub
    ''' <summary>
    ''' Function to process PRINT SEL request.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PrintProcessPRT()
        Dim strBootsCode As String
        strBootsCode = m_IIProductInfo.BootsCode
        'objPRTData.strBootscode = objAppContainer.objHelper.GeneratePCwithCDV(strPdtCode)
        If objAppContainer.bMobilePrinterAttachedAtSignon Then
#If NRF Then
            If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC (strBootsCode ,m_PSProductInfo )) Then
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
            'Whether its printed using mobile printer or not a PRT to be send
            'Confirm with govindh on the same
#If RF Then
            'Send PRT in the case of RF
            If Not (objAppContainer.objExportDataManager.CreatePRT(m_IIProductInfo.BootsCode)) Then
                objAppContainer.objLogger.WriteAppLog("Cannot Create PRT record at SP Start Session", Logger.LogLevel.RELEASE)
            End If
#End If
        End If
#If NRF Then
        'Changed this for a bug fix..
        'MC70 scenario has been removed - somehow.
        'Adding the same here
        Dim objPRTData As PRTRecord = New PRTRecord()
        'objPRTData.strBootscode = objAppContainer.objHelper.GeneratePCwithCDV(strPdtCode)
        objPRTData.strBootscode = m_IIProductInfo.BootsCode
        objPRTData.cIsMethod = Macros.PRINT_BATCH
        m_QueuedSELList.Add(objPRTData)
        objPRTData = Nothing
#End If
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr PrintProcessPRT", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Function to process PRP for printing SELs for a module.
    ''' </summary>
    ''' <param name="strModSequence"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
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
        If (m_strPlannerType = PLANTYPE.LivePlanner) Or (m_strPlannerType = PLANTYPE.PendingPlanner) Then
            m_SPLineListLP.Visible = True
        ElseIf (m_strPlannerType = PLANTYPE.SearchPlanner) Then
            m_SPLineListSP.Visible = True
        End If

    End Sub
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
        Dim iSelectedRow As Integer = 0
        Try
            With m_SPLineListSP
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
                            ' .lstView.Items(iCounter).Selected = True
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
            objAppContainer.objLogger.WriteAppLog("SPSessionMgr - Exception in full HighlightNextItem", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr HighlightNextItem", Logger.LogLevel.RELEASE)
    End Function
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
            With m_SPLineListSP
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
            objAppContainer.objLogger.WriteAppLog("SPSessionMgr - Exception in full HighlightPreviousItem", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr HighlightPreviousItem", Logger.LogLevel.RELEASE)
    End Function
#If NRF Then
    ''' <summary>
    ''' Writes the final set of data identified to the export data file
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteExportData() As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter SPSessionMgr WriteExportData", Logger.LogLevel.RELEASE)
        Try

            'Dim objSMDataManager As SMTransactDataManager = New SMTransactDataManager()

            If m_QueuedSELList.Count > 0 Then
                If m_strPlannerType.Equals(Macros.LIVE_PLANNER) Then
                    m_SPLPhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PRT")
                Else
                    m_SPSearchPlnrHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PRT")
                End If

                'If no price check records available write the PRTs between INS & INX
                If Not m_ModulePriceCheck.WriteExportData(m_QueuedSELList) Then
                    'Write INS
                    objAppContainer.objExportDataManager.CreateINS()
                    'Write all PRTs
                    For Each objPRT As PRTRecord In m_QueuedSELList
                        objAppContainer.objExportDataManager.CreatePRT(objPRT.strBootscode, SMTransactDataManager.ExFileType.EXData)
                    Next
                    'Write INX
                    objAppContainer.objExportDataManager.CreateINX()
                End If
            End If

            If m_QueuedPRPList.Count > 0 Then
                'Write PGS record
                objAppContainer.objExportDataManager.CreatePGS()
                'End If
                If m_strPlannerType.Equals(Macros.LIVE_PLANNER) Then
                    m_SPLPhome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PRP")
                Else
                    m_SPSearchPlnrHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PRP")
                End If
                For Each objPRP As PRPRecord In m_QueuedPRPList
                    objAppContainer.objExportDataManager.CreatePRP(objPRP)
                Next
                'Write PGX record
                objAppContainer.objExportDataManager.CreatePGX()

            End If
            'Clear price check session.
            m_ModulePriceCheck.EndSession()

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured Space Planner Write Export data: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit SPSessionMgr WriteExportData", Logger.LogLevel.RELEASE)
        Return True
    End Function
#End If

    ''' <summary>
    ''' Enum Class that defines all screens for Price Check module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum SPSCREENS
        LPHome
        SPHome
        LineListLP
        LineListSP
        ModuleListLP
        ModuleListSP
        DeptCategory
        PlannerListLP
        PlannerListSP
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
        LivePlanner
        PendingPlanner
        SearchPlanner
    End Enum

End Class
''' <summary>
''' Class to compare the objects in Module list and sort.
''' </summary>
''' <remarks></remarks>
Public Class PogCompare
    Implements IComparer
    ''' <summary>
    ''' Function to compare the objects.
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
        Dim tempMod1 As PlannerInfo = DirectCast(x, PlannerInfo)
        Dim tempMod2 As PlannerInfo = DirectCast(y, PlannerInfo)

        Return String.Compare(tempMod1.Description, tempMod2.Description)
    End Function
End Class
''' <summary>
''' Sort the items in the line list.
''' </summary>
''' <remarks></remarks>
Public Class LineListSorter
    Implements IComparer
    ''' <summary>
    ''' Function to compare the objects.
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
        Dim tmpShelf1 As SPLineListInfo = DirectCast(x, SPLineListInfo)
        Dim tmpShelf2 As SPLineListInfo = DirectCast(y, SPLineListInfo)

        Return String.Compare(tmpShelf1.ShelfNumber, tmpShelf2.ShelfNumber)
    End Function
End Class
''' ***************************************************************************
''' <fileName>CategoryList.vb</fileName>
''' <summary>Stores and retreives the list of categories available in the 
''' activ tables.
''' </summary>
''' <author>Infosys Technologies Ltd.,</author>
''' <DateModified></DateModified>
''' <remarks></remarks>
''' ***************************************************************************
Public Class CategoryInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_CategoryID As String
    Private m_Description As String
#If RF Then
    Private m_Pointer As String
    Public Property POINTER()
        Get
            Return m_Pointer
        End Get
        Set(ByVal value)
            m_Pointer = value
        End Set
    End Property
#End If
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub
    ''' <summary>
    ''' Gets or sets category ID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CategoryID() As String
        Get
            Return m_CategoryID
        End Get
        Set(ByVal value As String)
            m_CategoryID = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets category description.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Description() As String
        Get
            Return m_Description
        End Get
        Set(ByVal value As String)
            m_Description = value
        End Set
    End Property
End Class

''' ***************************************************************************
''' <fileName>ModuleInfo.vb</fileName>
''' <summary>To store and retreive the module details.
''' </summary>
''' <author>Infosys Technologies Ltd.,</author>
''' <DateModified></DateModified>
''' <remarks></remarks>
''' ***************************************************************************
Public Class ModuleInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_ModuleID As String
    Private m_Description As String
    Private m_SequenceNumber As String

#If RF Then
    Private m_ShelfQuantity As String
    Public Property SHELFCOUNT()
        Get
            Return m_ShelfQuantity
        End Get
        Set(ByVal value)
            m_ShelfQuantity = value
        End Set
    End Property
#End If
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub
    ''' <summary>
    ''' Gets or sets category ID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ModuleID() As String
        Get
            Return m_ModuleID
        End Get
        Set(ByVal value As String)
            m_ModuleID = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets category ID
    ''' For RF Scenario it stores the POGID for getting the info at later point of time
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SequenceNumber() As String
        Get
            Return m_SequenceNumber
        End Get
        Set(ByVal value As String)
            m_SequenceNumber = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets category description.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Description() As String
        Get
            Return m_Description
        End Get
        Set(ByVal value As String)
            m_Description = value
        End Set
    End Property
End Class
''' ***************************************************************************
''' <fileName>PlannerInfo.vb</fileName>
''' <summary>To store and retreive the planner details.
''' </summary>
''' <author>Infosys Technologies Ltd.,</author>
''' <DateModified></DateModified>
''' <remarks></remarks>
''' ***************************************************************************
Public Class PlannerInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_PlannerID As String
    Private m_Description As String
    Private m_RebuildDate As String
    Private m_SequenceNumber As String = Nothing
    Private m_ShelfNumber As String = Nothing
    Private m_PlannerDesc As String
    Private m_PSC As String = Nothing
    Private m_RepeatCount As String = Nothing
    'Stock File Accuracy  - Added new varibale
    Private m_IsCounted As String
    Private m_ItemCount As Integer
    Private m_SalesFloorQuantity As Integer
    Private m_BackShopQuantity As Integer
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        'Initialise the parameters here
    End Sub
    ''' <summary>
    ''' Gets or sets the item count in a site
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsCounted() As String
        Get
            Return m_IsCounted
        End Get
        Set(ByVal value As String)
            m_IsCounted = value
        End Set
    End Property
    Public Property ItemCount() As Integer
        Get
            Return m_ItemCount
        End Get
        Set(ByVal value As Integer)
            m_ItemCount = value
        End Set
    End Property
    Public Property SalesFloorQuantity() As Integer
        Get
            Return m_SalesFloorQuantity
        End Get
        Set(ByVal value As Integer)
            m_SalesFloorQuantity = value
        End Set
    End Property
    Public Property BackShopQuantity() As Integer
        Get
            Return m_BackShopQuantity
        End Get
        Set(ByVal value As Integer)
            m_BackShopQuantity = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the physical shelf quantity for an item
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PhysicalShelfQty() As String
        Get
            Return m_PSC
        End Get
        Set(ByVal value As String)
            m_PSC = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets category ID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PlannerID() As String
        Get
            Return m_PlannerID
        End Get
        Set(ByVal value As String)
            m_PlannerID = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets category description.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Description() As String
        Get
            Return m_Description
        End Get
        Set(ByVal value As String)
            m_Description = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the last rebuild date of the planner.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RebuildDate() As String
        Get
            Return m_RebuildDate
        End Get
        Set(ByVal value As String)
            m_RebuildDate = value
        End Set
    End Property
    ''' <summary>
    ''' Gets / Sets Sequence number for the module
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SequenceNumber() As String
        Get
            Return m_SequenceNumber
        End Get
        Set(ByVal value As String)
            m_SequenceNumber = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets Shelf number where the item is present.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ShelfNumber() As String
        Get
            Return m_ShelfNumber
        End Get
        Set(ByVal value As String)
            m_ShelfNumber = value
        End Set
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property POGDesc() As String
        Get
            Return m_PlannerDesc
        End Get
        Set(ByVal value As String)
            m_PlannerDesc = value
        End Set
    End Property
    'CR for Repeat Count
    Public Property RepeatCount() As String
        Get
            Return m_RepeatCount
        End Get
        Set(ByVal value As String)
            m_RepeatCount = value
        End Set
    End Property
End Class

Public Class SPLineListInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_ShelfDesc As String
    Private m_ShelfNumber As String
    Private m_BootsCode As String
    Private m_ItemDescription As String
    Private m_FaceCount As String
    Private m_NotchNumber As String
    Private m_ModuleSeqNumber As String
    ''' <summary>
    ''' gets or sets shelf description
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ShelfDesc() As String
        Get
            Return m_ShelfDesc
        End Get
        Set(ByVal value As String)
            m_ShelfDesc = value
        End Set
    End Property
    ''' <summary>
    ''' Getsor sets shelf number
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ShelfNumber() As String
        Get
            Return m_ShelfNumber
        End Get
        Set(ByVal value As String)
            m_ShelfNumber = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets face count
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FaceCount() As String
        Get
            Return m_FaceCount
        End Get
        Set(ByVal value As String)
            m_FaceCount = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or Sets item code
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ItemCode() As String
        Get
            Return m_BootsCode
        End Get
        Set(ByVal value As String)
            m_BootsCode = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets item description
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ItemDescription() As String
        Get
            Return m_ItemDescription
        End Get
        Set(ByVal value As String)
            m_ItemDescription = value
        End Set
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NotchNumber() As String
        Get
            Return m_NotchNumber
        End Get
        Set(ByVal value As String)
            m_NotchNumber = value
        End Set
    End Property

    Public Property ModuleSeqNumber()
        Get
            Return m_ModuleSeqNumber
        End Get
        Set(ByVal value)
            m_ModuleSeqNumber = value
        End Set
    End Property

End Class
