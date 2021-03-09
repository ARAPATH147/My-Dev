Imports System.Data
'''***************************************************************
''' <FileName>CLSessionMgr.vb</FileName>
''' <summary>
''' The Count List Container Class.
''' Implements all business logic and GUI navigation for Count List.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>27-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''*********************************************************************
''' * Modification Log 
''' ******************************************************************** 
''' * 1.1. Vishnu Balachandran.
''' Merged from service fix for the issue “Invalid Message Length” appearing in
''' CLD messages MC70/MC55BH batch devices".
''' ********************************************************************
Public Class CLSessionMgr

    Private Shared m_CLSessionMgr As CLSessionMgr = Nothing
    Private m_CLHome As frmCLHome
    Private m_CLLocSelection As frmCLLocationSelection
    Private m_CLItemDetails As frmCLItemDetails
    Private m_CLSalesFloorProductCount As frmCLSalesFloorProductCount
    Private m_CLBackShopProductCount As frmCLBackShopProductCount
    'ambli
    'For OSSR
    Private m_CLOSSRProductCount As frmCLOSSRProductCount
    'Private m_CLFinish As frmCLFinish
    Private m_CLSummary As frmCLSummary
    'Stock File Accuracy  - declared new form variables
    Private m_CLSiteInfo As frmCLSiteInformation
    Private m_CLViewListScreen As frmCLViewListScreen
    Private m_CLFullPriceCheck As frmCLFullPriceCheck
    Private m_EntryType As BCType = BCType.None
    Private m_ModulePriceCheck As ModulePriceCheck
    Private m_PreviousItem As String
    Private m_SELQueued As Integer = 0
    Private m_bIsFullPriceCheckRequired As Boolean = False
    Private m_SELForFullPriceCheck As String
    Private m_CLPriceCheckProductInfo As CLProductInfo = Nothing
    Private objCountListDataTable As New DataTable()

    Private m_CCLScan As frmCLSummary
    'Support : Full Price Check Removed
    'Private m_bIsFullPriceCheckRequired As Boolean
    'Private m_PreviousItem As String
    'Private m_ModulePriceCheck As ModulePriceCheck
    Private m_strSEL As String


    'Stock File Accuracy  

    Private m_CLCurrentSiteInfo As CLMultiSiteInfo = Nothing
    Private m_CLCurrentItemInfo As CLProductInfo = Nothing
    Private m_SelectedPOGSeqNum As String = Nothing
    'Private m_bIsNotPlannerItem As Boolean = False
    Private m_bIsMultisited As Boolean = False
    Private m_strCurrentBootsCode As String = Nothing
    Private m_arrItemList As ArrayList = Nothing
    Private m_iLocation As Integer = 0
    Private m_CLCurrentProductGroup As CLProductGroupInfo = Nothing
    Private m_ProductGroup As CLProductGroupInfo = Nothing
    Private m_iProductListCount As Integer
    'Indicates the location on which currently counting is done. 1 for sales floor and 2 for back shop
    Private m_iCountedLocation As Integer
   

    'Added as part of SFA - For Create Own List
    Private m_COLItemScan As frmCOLItemScan
    Private m_CLItemList As ArrayList
    Private isNewItem As Boolean = False
    Private m_iSequence As Integer = 0
    Public m_bIsCreateOwnList As Boolean
    Public bIsAlreadyScanned As Boolean = False
    Public m_COLLocation As String
    Private m_CLProductInfo As CLProductInfo = Nothing
    Private m_arrPogList As ArrayList = Nothing
    Private objCLProductInfoList As ArrayList = Nothing
    Private rowData As DataRow
    Private m_bIsItemsInPSP As Boolean = False
    Private m_bISItemsInOSSRPSP As Boolean = False
    Private m_bIsNewList As Boolean = False
    Private m_iListCreatedLoc As Integer = 0
    Private strProductValid As String = "Y"
    Private m_ItemScreen As String = Nothing
    Private m_bIsSiteInfo As Boolean = False
    Private strListId As String
    'Private iListCount As Integer = 10
    Private strCOLListId As String = ""
    'Private m_iCreateListCount As Integer
    Private m_iBackNextCount As Integer
    Private m_bIsBackNext As Boolean = False
    Private m_bNavigation As Boolean = False
    Private m_bIsItemNotInPlanner As Boolean = False
    Private m_bPlannerFlag As Boolean = False
    Private arrTemp As ArrayList = Nothing
    Private arr_NOPItemList As ArrayList = Nothing
    Private arr_UnItemList As ArrayList = Nothing
    Private bIsItemScan As Boolean = False
#If RF Then
    Private strListId_RF As String
    Private strSeq_RF As String
    Private objCreateCountList As New CLProductGroupInfo()
    Private iNakErrorFlag As Boolean = False
#End If
    'ambli
    'For OSSR
   
    'Property to store the number of items counted in sales floor
    Private m_iSalesFloorItemCount As Integer
    ''' <summary>
    ''' Getter and setter for the property m_iSalesFloorItemCount
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property GetSetSalesFloorItemCount() As Integer
        Get
            Return m_iSalesFloorItemCount
        End Get
        Set(ByVal value As Integer)
            m_iSalesFloorItemCount = value
        End Set
    End Property
    'Property to store the number of items counted in back shop
    Private m_iBackShopItemCount As Integer
    ''' <summary>
    ''' Getter and setter for the property m_iBackShopItemCount
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property GetSetBackShopItemCount() As Integer
        Get
            Return m_iBackShopItemCount
        End Get
        Set(ByVal value As Integer)
            m_iBackShopItemCount = value
        End Set
    End Property
    Public Property SelectedPOGSeqNum() As String
        Get
            Return m_SelectedPOGSeqNum
        End Get
        Set(ByVal value As String)
            m_SelectedPOGSeqNum = value
        End Set
    End Property

    'Public Property IsNotPlannerItem() As Boolean
    '    Get
    '        Return m_bIsNotPlannerItem
    '    End Get
    '    Set(ByVal value As Boolean)
    '        m_bIsNotPlannerItem = value
    '    End Set
    'End Property
    Public Property CurrentScreen() As Integer
        Get
            Return m_iLocation
        End Get
        Set(ByVal value As Integer)
            m_iLocation = value
        End Set
    End Property
    Public Property CurrentLocation() As Integer
        Get
            Return m_iCountedLocation
        End Get
        Set(ByVal value As Integer)
            m_iCountedLocation = value
        End Set
    End Property

    Private Sub New()

    End Sub
    ''' <summary>
    ''' Functions for getting the object instance for the CLContainer. 
    ''' Use this method to get the object refernce for the Singleton CLContainer.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of CLContainer Class</remarks>
    Public Shared Function GetInstance() As CLSessionMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.CUNTLIST
        If m_CLSessionMgr Is Nothing Then
            m_CLSessionMgr = New CLSessionMgr()
            Return m_CLSessionMgr
        Else
            Return m_CLSessionMgr
        End If

    End Function
#If RF Then
    ''' <summary>
    ''' Updates the Status bar of all the forms in the session manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBarMessage()
        Try
            m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_CLLocSelection.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_CLItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_CLSalesFloorProductCount.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_CLBackShopProductCount.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_CLOSSRProductCount.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            'm_CLFinish.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_CLSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_CLSiteInfo.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_CLViewListScreen.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME) 
            m_CLFullPriceCheck.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME) 
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception Occured, Trace : " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    ''' <summary>
    ''' Getting the Current Status of the Count List
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsItTheFirstItemToBeActioned() As Boolean
        Try
            ' This should be the first scan
            'Means in one location the count has to be one and others it should be zero
            If objAppContainer.m_CLBackShopCountedInfoList.Count = 1 Then
                If objAppContainer.m_CLSalesFloorCountedInfoList.Count = 0 And objAppContainer.m_CLOSSRCountedInfoList.Count = 0 Then
                    Return True
                Else
                    Return False
                End If
            ElseIf objAppContainer.m_CLSalesFloorCountedInfoList.Count = 1 Then
                If objAppContainer.m_CLBackShopCountedInfoList.Count = 0 And objAppContainer.m_CLOSSRCountedInfoList.Count = 0 Then
                    Return True
                Else
                    Return False
                End If
            ElseIf objAppContainer.m_CLOSSRCountedInfoList.Count = 1 Then
                If objAppContainer.m_CLSalesFloorCountedInfoList.Count = 0 And objAppContainer.m_CLBackShopCountedInfoList.Count = 0 Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Getting the Status info from CLSessionManager" + ex.StackTrace + _
                                                  " Message: " + ex.Message, Logger.LogLevel.RELEASE)
            Return True
        End Try
    End Function
#End If
    ''' <summary>
    ''' Does all the processing at the start of a session
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function StartSession() As Boolean
        Try
            objAppContainer.objLogger.WriteAppLog("Entered StartSession of CLSessionMgr", Logger.LogLevel.INFO)
            'Do all count list realated Initialisations here.
#If RF Then
            If objAppContainer.objExportDataManager.CreateCLO() Then
#End If
                m_CLHome = New frmCLHome()


                m_CLLocSelection = New frmCLLocationSelection()
                m_CLItemDetails = New frmCLItemDetails()
                m_CLSalesFloorProductCount = New frmCLSalesFloorProductCount()
                m_CLBackShopProductCount = New frmCLBackShopProductCount()
                'ambli
                'For OSSR
                m_CLOSSRProductCount = New frmCLOSSRProductCount()
                'm_CLFinish = New frmCLFinish()
                m_CLSummary = New frmCLSummary()

                'Stock File Accuracy  - Added new forms
                m_CLSiteInfo = New frmCLSiteInformation()
                m_CLViewListScreen = New frmCLViewListScreen()
                m_CLFullPriceCheck = New frmCLFullPriceCheck()


                m_CLCurrentSiteInfo = New CLMultiSiteInfo()
                m_CLCurrentItemInfo = New CLProductInfo()

                m_CLPriceCheckProductInfo = New CLProductInfo()
                m_ModulePriceCheck = New ModulePriceCheck()
                m_CLCurrentProductGroup = New CLProductGroupInfo()
            'm_CLSalesFloorCountedInfoList = New ArrayList()
            'm_CLBackShopCountedInfoList = New ArrayList()
                m_ProductGroup = New CLProductGroupInfo()
                'ambli
                'For OSSR
            'm_CLOSSRCountedInfoList = New ArrayList()
                'Support : full price check removed
                'm_bIsFullPriceCheckRequired = False
                'm_PreviousItem = ""
                m_strSEL = ""
                'm_ModulePriceCheck = New ModulePriceCheck()
                'Stock File Accuracy 

                'Stock File Accuracy - COL
           
                m_iProductListCount = 0
                m_strCurrentBootsCode = Nothing

                m_iBackShopItemCount = 0
                m_iSalesFloorItemCount = 0

                'For OSSR
                m_iOSSRItemCount = 0
                m_iCountedLocation = 0
                m_CLProductInfo = New CLProductInfo()
                m_arrPogList = New ArrayList()
                m_CLItemList = New ArrayList() '@@@@@@@@M

#If RF Then
            objAppContainer.m_CLSalesFloorCountedInfoList.Clear()
            objAppContainer.m_CLBackShopCountedInfoList.Clear()
            objAppContainer.m_CLOSSRCountedInfoList.Clear()
            objAppContainer.objCLSummary = New CLSummary()
            objAppContainer.objGlobalCreateCountList.Clear()
#End If
            'Retrieves the product group info from DB
#If NRF Then
                 If GetProductGroupInfo() Then
                    Return True
                Else
                    Return False
                End If
#End If
               
                'Refresh the list
#If RF Then

                CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.Home)
                Return True

#End If

#If RF Then
            Else
                Return False
            End If
#End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit StartSession of CLSessionMgr", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Does all processing that needs to be done when a session ends
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
#If NRF Then
        Public Function EndSession() As Boolean
#ElseIf RF Then
    Public Function EndSession(Optional ByVal isConnectivityLoss As Boolean = False) As Boolean
#End If
        Try
            'Save and data and perform all Exit Operations.
            'Close and Dispose all forms.
            objAppContainer.objLogger.WriteAppLog("Entered EndSession of CLSessionMgr", Logger.LogLevel.INFO)

            'Writes data to the temporary file
#If NRF Then
            WriteTempData()
#ElseIf RF Then
            If Not isConnectivityLoss Then
                If Not objAppContainer.objExportDataManager.CreateCLF() Then
                    objAppContainer.objLogger.WriteAppLog("Cannot End Count List Session." _
                                                    , Logger.LogLevel.RELEASE)
                    Return False
                End If
            End If
#End If
            If Not m_CLHome Is Nothing Then
                m_CLHome.Dispose()
                m_CLHome = Nothing
            End If
            If Not m_CLLocSelection Is Nothing Then
                m_CLLocSelection.Dispose()
                m_CLLocSelection = Nothing
            End If
            If Not m_CLItemDetails Is Nothing Then
                m_CLItemDetails.Dispose()
                m_CLItemDetails = Nothing
            End If
            If Not m_CLSalesFloorProductCount Is Nothing Then
                m_CLSalesFloorProductCount.Dispose()
                m_CLSalesFloorProductCount = Nothing
            End If
            If Not m_CLBackShopProductCount Is Nothing Then
                m_CLBackShopProductCount.Dispose()
                m_CLBackShopProductCount = Nothing
            End If
            If Not m_CLOSSRProductCount Is Nothing Then
                m_CLOSSRProductCount.Dispose()
                m_CLOSSRProductCount = Nothing
            End If
            If Not m_CLSummary Is Nothing Then
                m_CLSummary.Dispose()
                m_CLSummary = Nothing
            End If
            If Not m_CLSiteInfo Is Nothing Then
                m_CLSiteInfo.Dispose()
                m_CLSiteInfo = Nothing
            End If
            If Not m_CLViewListScreen Is Nothing Then
                m_CLViewListScreen.Dispose()
                m_CLViewListScreen = Nothing
            End If
            If Not m_CLFullPriceCheck Is Nothing Then
                m_CLFullPriceCheck.Dispose()
                m_CLFullPriceCheck = Nothing
            End If
            If Not m_COLItemScan Is Nothing Then
                m_COLItemScan.Dispose()
                m_COLItemScan = Nothing
            End If
            'Commented for performance check
            'm_CLHome.Dispose()
            'm_CLLocSelection.Dispose()
            'm_CLItemDetails.Dispose()
            'm_CLSalesFloorProductCount.Dispose()
            'm_CLBackShopProductCount.Dispose()
            ''ambli
            ''For OSSR
            'm_CLOSSRProductCount.Dispose()
            '' m_CLFinish.Dispose()
            'm_CLSummary.Dispose()
            ''Stock File Accuracy 
            'm_CLSiteInfo.Dispose()
            'm_CLViewListScreen.Dispose()
            'm_CLFullPriceCheck.Dispose()
            ''Added as part of SFA -  Create Own List
            'm_COLItemScan.Dispose()
            'Comment end

            m_CLPriceCheckProductInfo = Nothing
            m_CLCurrentSiteInfo = Nothing
            m_CLCurrentItemInfo = Nothing
            m_SelectedPOGSeqNum = Nothing
            'Support : Price Check Removed
            m_ModulePriceCheck = Nothing
            'Release all objects and Set to nothing.
            m_CLCurrentProductGroup = Nothing
            'm_CLSalesFloorCountedInfoList = Nothing
            'm_CLBackShopCountedInfoList = Nothing
            m_ProductGroup = Nothing
            'ambli
            'For OSSR
            ' m_CLOSSRCountedInfoList = Nothing
            'm_PreviousItem = Nothing
            m_CLSessionMgr = Nothing


            'm_CLItemList.Clear()
            objAppContainer.bIsCreateOwnList = Nothing
        Catch ex As Exception
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit EndSession of CLSessionMgr", Logger.LogLevel.INFO)
        Return True

    End Function
    ''' <summary>
    ''' The Method handles scan the scane data returned form the barcode scanner.
    ''' This method implements the business logic to populate the data to the corresponding
    ''' UI element after validation.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="Type"></param>
    ''' <remarks></remarks>
    Public Sub HandleScanData(ByVal strBarcode As String, ByVal Type As BCType)
        objAppContainer.objLogger.WriteAppLog("Barcode is :" + strBarcode + "|Type is: " + Type.ToString(), _
                                              Logger.LogLevel.INFO)
        m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        m_EntryType = Type
        Dim objProductInfo As New CLProductInfo()
        Dim objCurrentCountListInfoTable As New Hashtable()
        Dim objCurrentProductInfoList As New ArrayList()
        Dim objCurrentProduct As New CLProductInfo()
        'Dim objPSProductInfo As PSProductInfo = New PSProductInfo()

       

        Try
            'Added as part of SFA - Create own list - checks the total number items added 
            If m_bIsCreateOwnList Then
                'SFA SIT - Allows to update already scanned item even after adding 30 items
                If m_iLocation = Macros.COUNT_LIST_ITEMSCAN Then
                    If Not ((m_iCountedLocation = Macros.COUNT_BACK_SHOP Or m_iCountedLocation = Macros.COUNT_OSSR) And Type = BCType.SEL) Then
                        Dim bCheckForMaxCount As Boolean = True
                        Dim strBarcodeTemp As String = ""
                        Dim strBootsCodeTemp As String = ""
                        If Not m_CLItemList.Count = 0 Then
                            If Type = BCType.EAN Then
                                '#If NRF Then
                                strBarcodeTemp = strBarcode.Remove(strBarcode.Length - 1, 1)
                                '#End If
                            ElseIf Type = BCType.SEL Then
                                'If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                                objAppContainer.objHelper.GetBootsCodeFromSEL(strBarcode, strBootsCodeTemp)
                                strBootsCodeTemp = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCodeTemp)
                                'Else
                                '    bCheckForMaxCount = False
                                'End If
                            End If

                            'If bCheckForMaxCount Then
                            For Each objCL As CLProductInfo In m_CLItemList
                                If objCL.BootsCode.Equals(strBarcode) Or objCL.ProductCode.Equals(strBarcodeTemp) Or _
                                     objCL.BootsCode.Equals(strBootsCodeTemp) Then
                                    bCheckForMaxCount = False
                                    Exit For
                                End If
                            Next
                            'End If
                        End If
                        If bCheckForMaxCount Then
                            If m_CLItemList.Count = Macros.MAX_COUNT Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M106"), "Info", _
                                        MessageBoxButtons.OK, _
                                        MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
                                m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                Return
                            End If
                        End If
                    End If
                End If
            Else
                If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    objCurrentCountListInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                    objCurrentProductInfoList = objCurrentCountListInfoTable.Item(m_CLCurrentSiteInfo.strPlannerDesc)
                End If
                objCurrentProduct = objCurrentProductInfoList.Item(m_iProductListCount)
            End If
            Select Case Type
                Case BCType.EAN
                    If Not (objAppContainer.objHelper.ValidateEAN(strBarcode)) Or _
                       Val(strBarcode) = 0 Then
                        'Handle Invalid EAN here
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                        m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                    Else
                        '' ''#If NRF Then
                        '' ''                        strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                        '' ''#End If
                        'Stock File Accuracy  
                        'executes if full price check required
                        If (m_bIsFullPriceCheckRequired) Then
#If NRF Then
                            strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)

#End If
                            '                            'anoop:End
                            'Fix for the PRT record with all 0s as boots code.
                            'If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, objProductInfo)) Then
#If RF Then
                            If Not (objAppContainer.objDataEngine.GetCLProductInfo(strBarcode, objProductInfo)) Then
#ElseIf NRF Then
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, objProductInfo)) Then
#End If
                                'DARWIN checking if the base barcode is present in Database/Conroller
                                If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                                    'DARWIN converting database to Base Barcode
                                    strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                                    'If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, objProductInfo)) Then
#If RF Then
                                    If Not (objAppContainer.objDataEngine.GetCLProductInfo(strBarcode, objProductInfo)) Then
#ElseIf NRF Then
                                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, objProductInfo)) Then
#End If
                                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                                        MessageBoxButtons.OK, _
                                                        MessageBoxIcon.Asterisk, _
                                                        MessageBoxDefaultButton.Button1)
                                        m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                        Return
                                    End If
                                Else
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                                    MessageBoxButtons.OK, _
                                                    MessageBoxIcon.Asterisk, _
                                                    MessageBoxDefaultButton.Button1)
                                    m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                    Return
                                End If
                            End If
                            'End fix for PRT record with all 0s as boots code.

                            If ProcessFullPriceCheck(strBarcode, objProductInfo) Then
                                If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                                    CLSessionMgr.GetInstance().DisplayCLScreen(CLSCREENS.SalesFloorProductCount)
                                End If
                            End If
                        Else
                            ProcessBarcodeEntered(strBarcode, True)
                            Cursor.Current = Cursors.Default
                            m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        End If

                    End If
                Case BCType.ManualEntry
                    Dim strBootsCode As String = ""
                    If strBarcode.Length < 8 Then
                        'strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                        'Integration Testing
                        If objAppContainer.objHelper.ValidateBootsCode(strBarcode) Then
                            'Support : Price Check Removed
                            'To perfomr full price check if Boots Code is hand keyed after SEL scan.
                            'If m_bIsFullPriceCheckRequired Then
                            '    'TO be confirmed whether to consider Boots code entered at this point.
                            '    strBarcode = strBarcode.Substring(0, 6)
                            '    strBarcode = strBarcode.PadLeft(12, "0")
                            '    ProcessBarcodeEntered(strBarcode, True)
                            'Else


                            'Stock File Accuracy 
                            'If Not CLSessionMgr.GetInstance.IsNotPlannerItem Then
                            If Not m_bIsCreateOwnList Then
                                'If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBarcode, objProductInfo)) Then
                                If Not objCurrentProduct.IsUnknownItem Then
#If RF Then
                                    If Not (objAppContainer.objDataEngine.GetCLProductInfo(strBarcode, objProductInfo)) Then
#ElseIf NRF Then

                                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBarcode, objProductInfo)) Then
#End If
                                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                                        MessageBoxButtons.OK, _
                                                        MessageBoxIcon.Asterisk, _
                                                        MessageBoxDefaultButton.Button1)
#If NRF Then
                                        m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#ElseIf RF Then
                                        m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
#End If
                                        Return
                                    End If
                                End If
                            End If


                            'End If

                            m_strSEL = ""

                            If Not m_bIsFullPriceCheckRequired Then
                                ProcessBarcodeEntered(strBarcode, False)
                                'System Testing - Lakshmi
                                m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Else
                                'full price check for an item.
                                strBarcode = strBarcode.Substring(0, 6)
                                strBarcode = strBarcode.PadLeft(12, "0")
                            End If
                            'End If
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                           MessageBoxButtons.OK, _
                           MessageBoxIcon.Asterisk, _
                           MessageBoxDefaultButton.Button1)
                            m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
                        End If
                    Else
                        If (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                            'handle valid product code
                            strBarcode = strBarcode.PadLeft(13, "0")
                            ''#If NRF Then
                            '' strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                            ''#End If

                            'Stock File Accuracy  
                            If Not m_bIsFullPriceCheckRequired Then
                                ProcessBarcodeEntered(strBarcode, True)
                                'System Testing - Lakshmi
                                m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Else
#If NRF Then
                                strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
#End If
                                'If Not CLSessionMgr.GetInstance.IsNotPlannerItem Then
                                'anoop:End
                                'If Not objCurrentProduct.IsUnknownItem Then
                                If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, objProductInfo)) Then
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                        MessageBoxButtons.OK, _
                                        MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
                                    m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                    Return
                                End If
                                'End If

                                'End If
                            End If

                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                           MessageBoxButtons.OK, _
                           MessageBoxIcon.Asterisk, _
                           MessageBoxDefaultButton.Button1)
                            m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
                        End If
                    End If
                    If m_bIsFullPriceCheckRequired Then
                        If ProcessFullPriceCheck(strBarcode, objProductInfo) Then
                            If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                                CLSessionMgr.GetInstance().DisplayCLScreen(CLSCREENS.SalesFloorProductCount)
                            End If
                        End If
                    End If
                Case BCType.SEL
                    Dim isPriceCheck As Boolean = False
                    Dim isValid As Boolean = True
                    If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                        If objAppContainer.objHelper.ValidateSEL(strBarcode) Then
                            Dim strBootsCode As String = ""
                            objAppContainer.objHelper.GetBootsCodeFromSEL(strBarcode, strBootsCode)
                            strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
                            m_strSEL = strBarcode

                            'Stock File Accuracy  
                            'Price check feature included
                            isPriceCheck = ProcessBarcodeEntered(strBootsCode, False, isValid)
                            If isPriceCheck Then
                                If Not objCurrentProduct.IsUnknownItem Then
#If RF Then
                                    If Not (objAppContainer.objDataEngine.GetCLProductInfo(strBootsCode, objProductInfo)) Then
#ElseIf NRF Then
                                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBootsCode, objProductInfo)) Then
#End If
                                        'if product info is not obtained from database/ Controller.
                                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                                        MessageBoxButtons.OK, _
                                                        MessageBoxIcon.Asterisk, _
                                                        MessageBoxDefaultButton.Button1)
                                        m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                        Return
                                    Else
                                        Dim strTemp As String
#If NRF Then
                                        strTemp = m_ModulePriceCheck.DoPartialPriceCheck(objProductInfo.ProductCode, strBarcode)
#ElseIf RF Then
                                        strTemp = m_ModulePriceCheck.DoPartialPriceCheck(strBarcode, objProductInfo.Price)
#End If
                                        If strTemp.Equals("1") Then
#If NRF Then
                                            'No Price Change in Partial Price check. Check for Full Price Check
                                            If (m_ModulePriceCheck.IsFullPriceCheckRequired(False)) Then
#ElseIf RF Then
                                            'In case of RF a Full price check screen has to be displayed only if 
                                            '1. Price accepted flag is "Y" and the price check completed count should be less that taget
                                            If (m_ModulePriceCheck.IsFullPriceCheckRequired(objProductInfo.BootsCode, objProductInfo.ProductCode)) Then
#End If
                                                'Do Full Price check
                                                'Here the objective of setting full price flag to true is to show the 
                                                'Full price check screen
                                                'Validation of SEL against product code is alone done in
                                                ' full price check screen
                                                m_bIsFullPriceCheckRequired = True
                                                objProductInfo.NumSELsQueued = 0
                                                m_PreviousItem = strBootsCode
                                                m_SELForFullPriceCheck = strBarcode
                                                m_CLPriceCheckProductInfo = objProductInfo
                                                'UAT fix for incrementing the scanned items if
                                                'Full price check is not performed in FPC screen.
                                                'm_bItemScanned = True
                                                'UAT Fix End.
                                            Else
                                                'Price change in Partial Price check
                                                objProductInfo.NumSELsQueued = 0
                                            End If
                                        ElseIf strTemp.Equals("0") Then
                                            objProductInfo.NumSELsQueued += 1

                                            'Queue SEL
                                            If objAppContainer.bMobilePrinterAttachedAtSignon Then
                                                Dim objPSProductInfo As PSProductInfo = New PSProductInfo()
#If NRF Then
                                                objAppContainer.objDataEngine.GetProductInfoUsingPC(objProductInfo.ProductCode, _
                                                                                                    objPSProductInfo)
#ElseIf RF Then
                                                With objPSProductInfo
                                                    .BootsCode = objProductInfo.BootsCode
                                                    .ProductCode = objProductInfo.ProductCode
                                                    .Description = objProductInfo.Description
                                                    .Status = objProductInfo.Status
                                                    .CurrentPrice = objProductInfo.Price
                                                    .CIPFlag = objProductInfo.CIPFlag
                                                    .Advantage = objProductInfo.Advantage
                                                    .SupplyRoute = objProductInfo.SupplyRoute
                                                End With
                                                objAppContainer.objDataEngine.GetLabelDetails(objPSProductInfo)
#End If
                                                objPSProductInfo.LabelQuantity = 1
                                                MobilePrintSessionManager.GetInstance.CreateLabels(objPSProductInfo)
                                                objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.CUNTLIST
                                            Else
                                                '#If NRF Then
                                                UpdateProductSELCount(objProductInfo)
#If RF Then
                                                'Incase if in RF and mobile printer not attached send PRT request.
                                                objAppContainer.objExportDataManager.CreatePRT(objProductInfo.BootsCode, _
                                                                                               SMTransactDataManager.ExFileType.EXData)
#End If
                                            End If
                                            '#If RF Then
                                            'In RF Replacement SEL message is not shown in module price check.
                                            'This is because this has to be shown only after printing the SEL's
                                            'So , after printing the SEL using mobile printer or Sending PRT to the controller - showing the message in RF
                                            'MessageBox.Show(MessageManager.GetInstance().GetMessage("M16"), "Replace SEL", _
                                            'MessageBoxButtons.OK, _
                                            'MessageBoxIcon.Asterisk, _
                                            'MessageBoxDefaultButton.Button1)
                                            '#End If
                                            'Update SEL Queued Count or printed count
                                            'm_SELQueued = m_SELQueued + 1
                                        End If
                                    End If
                                End If  'unknown
                            End If    'ispricechek
                            If m_bIsFullPriceCheckRequired Then
                                CLSessionMgr.GetInstance().DisplayCLScreen(CLSCREENS.FullPriceCheck)
                            Else
                                If isValid Then
                                    'Normal flow  if  full price check not required
                                    If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                                        CLSessionMgr.GetInstance().DisplayCLScreen(CLSCREENS.SalesFloorProductCount)
                                    End If
                                End If
                            End If
                        Else
                            'handle invalid SEL here
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M4"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        End If
                        m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                    Else
                        'if BS, sel not used.. Need to scan product code
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M69"), "Alert", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Hand, _
                                MessageBoxDefaultButton.Button1)
                        m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        'Cursor.Current = Cursors.Default
                    End If
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in HandleScanData of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        Finally
            If Not m_COLItemScan Is Nothing Then
                If Not m_COLItemScan.ProdSEL1.txtSEL.IsDisposed AndAlso Not m_COLItemScan.ProdSEL1.txtProduct.IsDisposed Then
                    m_COLItemScan.ProdSEL1.txtSEL.Text = ""
                    m_COLItemScan.ProdSEL1.txtProduct.Text = ""
                End If
            End If
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit HandleScanData of CLSessionMgr", Logger.LogLevel.INFO)
    End Sub

    Private Function ProcessFullPriceCheck(ByVal strBarcode As String, ByRef objProductInfo As CLProductInfo) As Boolean
        If objAppContainer.objDataEngine.ValidateUsingPCAndBC(m_PreviousItem, strBarcode) Then
#If NRF Then
            If Not (m_ModulePriceCheck.DoFullPriceCheck(m_SELForFullPriceCheck, objProductInfo.ProductCode)) Then
                'Price change in Full Price check
                objProductInfo.NumSELsQueued += 1
                'Update SEL Queued Count
                'm_SELQueued = m_SELQueued + 1
                'Support Bug Fix.
                'Queue SEL
                UpdateProductSELCount(objProductInfo)
            End If
            '#ElseIf RF Then
            '*****Only valiation of SEL against product code is needed
            '*****No other verification is needed because an "ENQ " with "C" is already sent.
            'm_ModulePriceCheck.DoFullPriceCheck(strBarcode)
#End If
            m_bIsFullPriceCheckRequired = False
        Else
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M47"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
            m_CLHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            'Reset the Barcode Symbology Type.
            ' m_EntryType = BCType.SEL
            Return False
        End If
        m_SELForFullPriceCheck = ""
        Return True
    End Function
    '#If NRF Then
    Private Function UpdateProductSELCount(ByVal objProductInfo As CLProductInfo) As Boolean
        'if MobilePrintSessionManager.GetInstance.MobilePrinterStatus Then
        'Dim objPRTData As PRTRecord = New PRTRecord()
        Dim arrProductList As New ArrayList()
        Dim isSELQueued As Boolean = False
        Try
            'objPRTData.strBootscode = objAppContainer.objHelper.GeneratePCwithCDV(m_IIProductInfo.ProductCode)
            'objPRTData.strBootscode = objProductInfo.BootsCode
            'objPRTData.cIsMethod = Macros.PRINT_BATCH
            'm_CLSELQueued.Add(objPRTData)

            'Update the global structure if SEL queued
            If m_bIsCreateOwnList Then
                For Each objCurrentProductInfo As CLProductInfo In m_CLItemList
                    If objCurrentProductInfo.BootsCode.Equals(objProductInfo.BootsCode) Then
                        objCurrentProductInfo.NumSELsQueued += 1
                        m_SELQueued = m_SELQueued + 1
                        isSELQueued = True
                        Exit For
                    End If
                Next
                If Not isSELQueued Then
                    m_CLProductInfo.NumSELsQueued += 1
                    m_SELQueued = m_SELQueued + 1
                End If
            Else
                If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    arrProductList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                    For Each objCurrentProductInfo As CLProductInfo In arrProductList
                        If objCurrentProductInfo.BootsCode.Equals(objProductInfo.BootsCode) Then
                            objCurrentProductInfo.NumSELsQueued += 1
                            Exit For
                        End If
                    Next
                End If
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    '#End If

    ''' <summary>
    ''' Processes the product code or boots code entered
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="bIsProductCode"></param>
    ''' <remarks></remarks>
    Public Function ProcessBarcodeEntered(ByVal strBarcode As String, ByVal bIsProductCode As Boolean, Optional ByRef isValid As Boolean = True) As Boolean
        Try
            objAppContainer.objLogger.WriteAppLog("Entered ProcessBarcodeEntered of CLSessionMgr", Logger.LogLevel.INFO)
            Dim bIsProductValid As Boolean = False
            Dim objProductInfoList As ArrayList = New ArrayList()
            Dim strPrdCode As String = ""
            Dim objCurrentCountListInfoTable As Hashtable = New Hashtable()
            Dim objItemList As New ArrayList()
            Dim iIndex As Integer = -1
            bIsAlreadyScanned = False
            'Added as part of SFA - for Create own List - Start
            If m_bIsCreateOwnList Then
#If NRF Then
                If bIsProductCode Then
                    strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                End If

#End If
                If m_ItemScreen = Macros.SCREEN_ITEM_CONFIRM Then
                    bIsItemScan = False
                    If m_CLProductInfo.IsUnknownItem Then
                        If bIsProductCode Then
                            'strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                            If m_CLProductInfo.ProductCode.Equals(strBarcode) Then
                                strProductValid = "Y"
                            Else
                                strProductValid = "N"
                            End If
                        Else
                            If m_CLProductInfo.BootsCode.Equals(strBarcode) Then
                                strProductValid = "Y"
                            Else
                                strProductValid = "N"
                            End If
                        End If

                    Else
                        If bIsProductCode Then
                            '#If NRF Then
                            '                            strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                            '#End If
                            If objAppContainer.objDataEngine.ValidateUsingPCAndBC(m_CLProductInfo.BootsCode, _
                                                                                  strBarcode) Then
                                strProductValid = "Y"
                            Else
                                strProductValid = "N"
                            End If
                        Else
                            If m_CLProductInfo.BootsCode.Equals(strBarcode) Then
                                strProductValid = "Y"
                            Else
                                strProductValid = "N"
                            End If
                        End If
                    End If
                    If strProductValid = "N" Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M10"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                        objAppContainer.objLogger.WriteAppLog("Invalid Barcode Entry", Logger.LogLevel.INFO)
                        isValid = False
                        'DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation)
                        Return False
                    End If
                    'm_ItemScreen = Nothing
                Else
                    'Dim m_CLProductInfo As CLProductInfo = New CLProductInfo()
                    Dim strTempCode As String = "000000000000"
                    If bIsProductCode Then
                        For Each objCLProductInfo As CLProductInfo In m_CLItemList
                            'If objEXProductInfo.ProductCode.Equals(strBarCode) Then
                            iIndex = iIndex + 1
                            If (strBarcode = objCLProductInfo.ProductCode) Or (strBarcode = objCLProductInfo.FirstBarcode) Then
                                m_CLProductInfo = objCLProductInfo
                                bIsAlreadyScanned = True
                                m_bIsBackNext = False
                                m_iBackNextCount = iIndex
                                'Fix for OSSR Toggle
                                '#If RF Then
                                'bOSSR_Toggled = True
                                '#End If
                                Exit For
#If NRF Then
                            ElseIf strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                                'to check if already scanned Catch Weight Barcode is scanned again
                                strTempCode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                                iIndex = -1
                                If (strTempCode = objCLProductInfo.ProductCode) Or (strTempCode = objCLProductInfo.FirstBarcode) Then
                                    iIndex = iIndex + 1
                                    m_CLProductInfo = objCLProductInfo
                                    bIsAlreadyScanned = True
                                    m_bIsBackNext = False
                                    m_iBackNextCount = iIndex
                                    Exit For
                                End If
#End If
                            End If

                        Next
                        If Not bIsAlreadyScanned Then
                            m_CLProductInfo = New CLProductInfo()
#If RF Then
                            objCreateCountList.SeqNumber = strSeq_RF
                            'strSeq_RF = strSeq_RF + 1
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_CLProductInfo, objCreateCountList, Nothing, iNakErrorFlag)) Then
                                isValid = False
                                If Not iNakErrorFlag Then
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M110"), "Info", _
                                                MessageBoxButtons.OK, _
                                                MessageBoxIcon.Asterisk, _
                                                MessageBoxDefaultButton.Button1)
                                Else
                                    iNakErrorFlag = False
                                    BCReader.GetInstance().StartRead()
                                End If
                                Return False
#Else

                            'strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)

                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_CLProductInfo)) Then
#End If
#If NRF Then
                                'DARWIN checking if the base barcode is present in Database/Conroller
                                If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                                    'DARWIN converting database to Base Barcode
                                    strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                                    m_CLProductInfo = New CLProductInfo()
                                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_CLProductInfo)) Then
                                        If m_EntryType = BCType.ManualEntry Then
                                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M109"), "Info", _
                                                MessageBoxButtons.OK, _
                                                MessageBoxIcon.Asterisk, _
                                                MessageBoxDefaultButton.Button1)
                                            Return True
                                        Else
                                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M105"), "Info", _
                                                    MessageBoxButtons.OK, _
                                                    MessageBoxIcon.Asterisk, _
                                                    MessageBoxDefaultButton.Button1)

                                            GetUnknownItemDetails(True)
                                        End If
                                    Else
                                        'GetMultiSiteLocations(m_EXProductInfo.ProductCode, arrPogList)
                                        'm_CLProductInfo.MultiSiteCount = arrPogList.Count
                                    End If
                                Else
#End If
                                If m_EntryType = BCType.ManualEntry Then
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M109"), "Info", _
                                        MessageBoxButtons.OK, _
                                        MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
                                    Return True
                                Else
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M105"), "Info", _
                                            MessageBoxButtons.OK, _
                                            MessageBoxIcon.Asterisk, _
                                            MessageBoxDefaultButton.Button1)

                                    GetUnknownItemDetails(True)
                                End If
#If NRF Then
                                End If
#End If
                            Else
                                isNewItem = True
                                m_bIsBackNext = False
                                '#If RF Then
                                '                                strSeq_RF = strSeq_RF + 1
                                '#End If
                                '                                m_CLProductInfo.Sequence = m_iSequence + 1
                                m_iBackNextCount = m_iBackNextCount + 1
                                m_arrPogList.Clear()
                                GetMultiSiteLocations(m_CLProductInfo.BootsCode, m_arrPogList)
                                'Check if the item is mulltisited. 
                                If (m_arrPogList.Count > 1) Then
                                    m_CLProductInfo.MultiSiteCount = m_arrPogList.Count
                                    m_CLProductInfo.MSFlag = "Y"
                                    m_bIsMultisited = True
                                    'PopulateMultiSites(m_arrPogList, m_CLProductInfo.BootsCode)

                                ElseIf m_arrPogList.Count = 1 Then
                                    m_CLProductInfo.MultiSiteCount = 0
                                    m_CLProductInfo.MSFlag = "  "
                                    m_bIsMultisited = False
                                Else
                                    Dim objOtherSiteInfo As New CLSessionMgr.CLMultiSiteInfo()
                                    Dim arrNotPlannerList As New ArrayList()

                                    objOtherSiteInfo.strPlannerID = "Not On Planner"
                                    objOtherSiteInfo.strPOGDescription = "Not On Planner"
                                    objOtherSiteInfo.strPlannerDesc = "Not On Planner"
                                    objOtherSiteInfo.IsCounted = "N"
                                    objOtherSiteInfo.strSalesFloorQuantiy = -1
                                    objOtherSiteInfo.strSeqNumber = 0
                                    arrNotPlannerList.Add(objOtherSiteInfo)

                                    m_CLProductInfo.SFMultiSiteList = arrNotPlannerList
                                    m_CLProductInfo.MultiSiteCount = 0
                                    m_CLProductInfo.MSFlag = "  "
                                    m_bIsMultisited = False

                                    arr_NOPItemList.Add(m_CLProductInfo)
                                    m_CLCurrentProductGroup.NotOnPlannerItemCount = m_CLCurrentProductGroup.NotOnPlannerItemCount + 1
                                    '''''''''''
                                    '#If NRF Then
                                    '                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                    '                                               MessageBoxButtons.OK, _
                                    '                                               MessageBoxIcon.Asterisk, _
                                    '                                               MessageBoxDefaultButton.Button1)

                                    '                                m_COLItemScan.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                    '                                Return False
                                    '#End If
                                End If
                            End If
                        End If
                    Else
                        iIndex = -1
                        For Each objCLProductInfo As CLProductInfo In m_CLItemList
                            iIndex = iIndex + 1
                            If objCLProductInfo.BootsCode.Equals(strBarcode) Then
                                m_CLProductInfo = objCLProductInfo
                                bIsAlreadyScanned = True
                                m_bIsBackNext = False
                                m_iBackNextCount = iIndex
#If RF Then
                                'bOSSR_Toggled = True
#End If
                                Exit For
                            End If
                        Next
                        If Not bIsAlreadyScanned Then
                            m_CLProductInfo = New CLProductInfo()
#If RF Then
                            objCreateCountList.SeqNumber = strSeq_RF
                            'strSeq_RF = strSeq_RF + 1
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBarcode, m_CLProductInfo, objCreateCountList, Nothing, iNakErrorFlag)) Then
                                If Not iNakErrorFlag Then
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M110"), "Info", _
                                                MessageBoxButtons.OK, _
                                                MessageBoxIcon.Asterisk, _
                                                MessageBoxDefaultButton.Button1)
                                Else
                                    iNakErrorFlag = False
                                    BCReader.GetInstance().StartRead()
                                End If
                                isValid = False   'Change valid flag to false
                                Return False
#Else
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBarcode, m_CLProductInfo)) Then
                                If m_EntryType = BCType.ManualEntry Then
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M109"), "Info", _
                                        MessageBoxButtons.OK, _
                                        MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
                                    Return True
                                Else
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M105"), "Info", _
                                            MessageBoxButtons.OK, _
                                            MessageBoxIcon.Asterisk, _
                                            MessageBoxDefaultButton.Button1)

                                    GetUnknownItemDetails(False)
                                End If
#End If
                            Else
                                isNewItem = True
                                m_bIsBackNext = False
                                '#If RF Then
                                '                                strSeq_RF = strSeq_RF + 1
                                '#End If
                                m_bIsItemNotInPlanner = False
                                m_CLProductInfo.Sequence = m_iSequence
                                m_iSequence += 1
                                m_iBackNextCount = m_iBackNextCount + 1
                                m_arrPogList.Clear()
                                GetMultiSiteLocations(m_CLProductInfo.BootsCode, m_arrPogList)
                                'Check if the item is mulltisited. 
                                If (m_arrPogList.Count > 1) Then
                                    m_CLProductInfo.MultiSiteCount = m_arrPogList.Count
                                    m_CLProductInfo.MSFlag = "Y"
                                    m_bIsMultisited = True
                                    'PopulateMultiSites(m_arrPogList, m_CLProductInfo.ProductCode)

                                ElseIf m_arrPogList.Count = 1 Then
                                    m_CLProductInfo.MultiSiteCount = 0
                                    m_CLProductInfo.MSFlag = "  "
                                    m_bIsMultisited = False
                                Else
                                    Dim objOtherSiteInfo As New CLSessionMgr.CLMultiSiteInfo()
                                    Dim arrNotPlannerList As New ArrayList()
                                    objOtherSiteInfo.strPlannerID = "Not On Planner"
                                    objOtherSiteInfo.strPOGDescription = "Not On Planner"
                                    objOtherSiteInfo.strPlannerDesc = "Not On Planner"
                                    objOtherSiteInfo.IsCounted = "N"
                                    objOtherSiteInfo.strSalesFloorQuantiy = -1
                                    objOtherSiteInfo.strSeqNumber = 0
                                    arrNotPlannerList.Add(objOtherSiteInfo)

                                    m_CLProductInfo.SFMultiSiteList = arrNotPlannerList
                                    m_CLProductInfo.MultiSiteCount = 0
                                    m_CLProductInfo.MSFlag = "  "
                                    m_bIsMultisited = False

                                    arr_NOPItemList.Add(m_CLProductInfo)
                                    m_CLCurrentProductGroup.NotOnPlannerItemCount = m_CLCurrentProductGroup.NotOnPlannerItemCount + 1
                                    '''''''''''
                                    '#If NRF Then
                                    '                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                    '                                                   MessageBoxButtons.OK, _
                                    '                                                   MessageBoxIcon.Asterisk, _
                                    '                                                   MessageBoxDefaultButton.Button1)

                                    '                                    m_COLItemScan.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                    '                                    Return False
                                    '#End If

                                End If
                            End If
                        End If
                    End If
                End If
                'Added as part of SFA - Create Own List,

                If Not m_ItemScreen = Macros.SCREEN_ITEM_CONFIRM Then
'#If NRF Then
                    'if difference is not greater than 2, display message
                    Dim dtLastCounted As Date
                    Dim tsSpan As TimeSpan
                    Dim iDateDiff As Integer
                    Dim dateToDisplay As Date
                    Dim iConfirm As Integer = 0
                    Dim strMessage As String
                    Dim strDate As String = ""
                    Try
#If RF Then
                        strDate = m_CLProductInfo.LastCountDate
                        dtLastCounted = New Date(CInt(strDate.Substring(0, 4)), CInt(strDate.Substring(4, 2)), CInt(strDate.Substring(6, 2)))
#ElseIf NRF Then
                    dtLastCounted = m_CLProductInfo.LastCountDate
#End If
                    Catch
                        dtLastCounted = "01/01/1900"
                    End Try
                    tsSpan = Now.Subtract(dtLastCounted)
                    iDateDiff = tsSpan.Days
                    dateToDisplay = dtLastCounted
                    If Not iDateDiff > 2 Then
                        strMessage = MessageManager.GetInstance().GetMessage("M103").ToString()
                        strMessage = strMessage.Replace("$", dateToDisplay.Date)
                        iConfirm = MessageBox.Show(strMessage, "Confirmation", _
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                    End If
                    If iConfirm = MsgBoxResult.No Then
                        isValid = False
                        DisplayCLScreen(CLSCREENS.COLItemScan)
                    Else
                        'For price check
                        If m_EntryType = BCType.SEL And Not m_CLProductInfo.IsUnknownItem Then
                            Return True
                        Else
                            If m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                                CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.BackShopProductCount)
                            ElseIf m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                                CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.SalesFloorProductCount)
                            ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                                CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.OSSRProductCount)
                            End If
                        End If
                    End If
                Else
                    'For price check
                    If m_EntryType = BCType.SEL And Not m_CLProductInfo.IsUnknownItem Then
                        Return True
                    Else
                        If m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                            CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.BackShopProductCount)
                        ElseIf m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                            CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.SalesFloorProductCount)
                        ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                            CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.OSSRProductCount)
                        End If
                    End If
                End If
                'Added as part of SFA - for Create own List - End

                'Normal count list
            Else
                Dim strbtcode As String = Nothing
                If bIsProductCode Then
                    'strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                    strBarcode = strBarcode.Substring(0, 12)
                End If

                'Retrieves the list id for which product info needs to be checked
                If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    objCurrentCountListInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                    objProductInfoList = objCurrentCountListInfoTable.Item(m_CLCurrentSiteInfo.strPlannerDesc)
                End If

                'Checks if the product code scanned and the product code of the product in the list are same
                'If true then item is valid else item is not valid
                Dim objProductInfo As CLProductInfo = New CLProductInfo()
                objProductInfo = objProductInfoList.Item(m_iProductListCount)
                If bIsProductCode Then
                    If Not objProductInfo.IsUnknownItem Then
                        If objAppContainer.objDataEngine.ValidateUsingPCAndBC(objProductInfo.BootsCode, _
                                                                         strBarcode) Then
                            strPrdCode = objProductInfo.ProductCode
                            bIsProductValid = True
                        End If
                    Else
                        'strbtcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                        If objProductInfo.ProductCode.Equals(strBarcode) Then
                            bIsProductValid = True
                        End If
                    End If
                Else
                    If objProductInfo.BootsCode.Equals(strBarcode) Then
                        bIsProductValid = True
                    End If
                End If
                If Not bIsProductValid Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M10"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                    objAppContainer.objLogger.WriteAppLog("Invalid Barcode Entry", Logger.LogLevel.INFO)
                    'MessageBox.Show("This is wrong item. Please try again.")
                    isValid = False
                    Return False
                Else
                    Dim iResult As Integer
                    Dim arrTempMultiSiteList As New ArrayList()
                    Dim clMultiSiteData As CLMultiSiteInfo
                    Dim arrMultiSiteList As New ArrayList()
                    Dim iCount As Integer
                    Dim objProductInfoTable As New Hashtable()
                    Dim iBSQty As Integer
                    Dim iBSPSPQty As Integer
                    Dim iOSSR_BSQty As Integer
                    Dim iOSSR_PSPQty As Integer

                    'To return to handle scan data if SEL scanned to implement pricecheck
                    If m_EntryType = BCType.SEL And Not m_CLCurrentItemInfo.IsSFDifferentSession Then
                        Return True
                    Else
                        If m_iCountedLocation = Macros.COUNT_SALES_FLOOR And m_CLCurrentItemInfo.IsSFDifferentSession Then
                            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M107"), "Confirmation", _
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                                    MessageBoxDefaultButton.Button1)
                            If iResult = MsgBoxResult.Yes Then

                                arrTempMultiSiteList = m_CLCurrentItemInfo.SFMultiSiteList
                                iCount = 0
                                For Each PlannerInfo As CLMultiSiteInfo In arrTempMultiSiteList
                                    iCount = iCount + 1
                                    clMultiSiteData = New CLMultiSiteInfo
                                    clMultiSiteData.strSeqNumber = PlannerInfo.strSeqNumber
                                    clMultiSiteData.strSalesFloorQuantiy = -1
                                    clMultiSiteData.strPlannerDesc = PlannerInfo.strPlannerDesc
                                    clMultiSiteData.strPlannerID = PlannerInfo.strPlannerID
                                    If arrTempMultiSiteList.Count > 1 And iCount = arrTempMultiSiteList.Count Then
                                        clMultiSiteData.IsCounted = "Y"
                                        clMultiSiteData.strPOGDescription = "Other"
                                    Else
                                        clMultiSiteData.strPOGDescription = PlannerInfo.strPlannerDesc + " - " + PlannerInfo.strModuleDesc
                                        clMultiSiteData.IsCounted = "N"
                                    End If
                                    arrMultiSiteList.Add(clMultiSiteData)
                                Next
                                'Reset the multisitelist for an item
                                m_CLCurrentItemInfo.SFMultiSiteList = arrMultiSiteList
#If RF Then
                                If m_CLCurrentItemInfo.OSSRFlag = "O" Then
                                    If m_CLCurrentItemInfo.OSSRMBSQuantity < 0 Then
                                        iOSSR_BSQty = 0
                                    Else
                                        iOSSR_BSQty = m_CLCurrentItemInfo.OSSRMBSQuantity
                                    End If
                                    If m_CLCurrentItemInfo.OSSRPSPQuantity < 0 Then
                                        iOSSR_PSPQty = 0
                                    Else
                                        iOSSR_PSPQty = m_CLCurrentItemInfo.OSSRPSPQuantity
                                    End If
                                Else
                                    If m_CLCurrentItemInfo.BackShopMBSQuantity < 0 Then
                                        iBSQty = 0
                                    Else
                                        iBSQty = m_CLCurrentItemInfo.BackShopMBSQuantity
                                    End If
                                    If m_CLCurrentItemInfo.BackShopPSPQuantity < 0 Then
                                        iBSPSPQty = 0
                                    Else
                                        iBSPSPQty = m_CLCurrentItemInfo.BackShopPSPQuantity
                                    End If
                                End If
#ElseIf NRF Then
                                If m_CLCurrentItemInfo.BackShopMBSQuantity < 0 Then
                                    iBSQty = 0
                                Else
                                    iBSQty = m_CLCurrentItemInfo.BackShopMBSQuantity
                                End If
                                If m_CLCurrentItemInfo.BackShopPSPQuantity < 0 Then
                                    iBSPSPQty = 0
                                Else
                                    iBSPSPQty = m_CLCurrentItemInfo.BackShopPSPQuantity
                                End If
#End If


                                ''''''''''
#If RF Then
                                If objAppContainer.OSSRStoreFlag = "Y" Then
                                    If m_CLCurrentItemInfo.OSSRFlag = "O" Then
                                        If m_CLCurrentItemInfo.BackShopQuantity < 0 And m_CLCurrentItemInfo.BackShopPSPQuantity < 0 And _
                                                                       m_CLCurrentItemInfo.OSSRQuantity < 0 And m_CLCurrentItemInfo.OSSRPSPQuantity < 0 Then
                                            m_CLCurrentItemInfo.TotalQuantity = m_CLCurrentItemInfo.BackShopQuantity + m_CLCurrentItemInfo.BackShopPSPQuantity + _
                                                             m_CLCurrentItemInfo.OSSRQuantity + m_CLCurrentItemInfo.OSSRPSPQuantity
                                        Else
                                            m_CLCurrentItemInfo.TotalQuantity = iBSQty + iBSPSPQty + iOSSR_BSQty + iOSSR_PSPQty
                                        End If
                                    Else
                                        If m_CLCurrentItemInfo.BackShopQuantity < 0 And m_CLCurrentItemInfo.BackShopPSPQuantity < 0 Then
                                            m_CLCurrentItemInfo.TotalQuantity = m_CLCurrentItemInfo.BackShopQuantity + m_CLCurrentItemInfo.BackShopPSPQuantity
                                        Else
                                            m_CLCurrentItemInfo.TotalQuantity = iBSQty + iBSPSPQty
                                        End If
                                    End If
                                Else
                                    If m_CLCurrentItemInfo.BackShopQuantity < 0 And m_CLCurrentItemInfo.BackShopPSPQuantity < 0 Then
                                        m_CLCurrentItemInfo.TotalQuantity = m_CLCurrentItemInfo.BackShopQuantity + m_CLCurrentItemInfo.BackShopPSPQuantity
                                    Else
                                        m_CLCurrentItemInfo.TotalQuantity = iBSQty + iBSPSPQty
                                    End If
                                End If
#ElseIf NRF Then
                                If m_CLCurrentItemInfo.BackShopQuantity < 0 And m_CLCurrentItemInfo.BackShopPSPQuantity < 0 Then
                                    m_CLCurrentItemInfo.TotalQuantity = m_CLCurrentItemInfo.BackShopQuantity + m_CLCurrentItemInfo.BackShopPSPQuantity
                                Else
                                    m_CLCurrentItemInfo.TotalQuantity = iBSQty + iBSPSPQty
                                End If
#End If


                                ''''''''''''
                                m_CLCurrentItemInfo.SalesFloorQuantity = -1

                                'Reset the site level count status for those items
                                For Each strItem As String In m_CLCurrentItemInfo.DistinctPOGList
                                    If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                                        objProductInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                                        objItemList = objProductInfoTable.Item(strItem)
                                        For Each objItemInfo As CLProductInfo In objItemList
                                            If objItemInfo.BootsCode = m_CLCurrentItemInfo.BootsCode Then
                                                objItemInfo.CountStatus = "N"
                                            End If
                                        Next
                                    End If
                                Next

                                'm_CLCurrentItemInfo.IsSFItemCounted = True
                                m_CLCurrentItemInfo.IsSFDifferentSession = False

                                '''''''''
                                'Checks if the product is already counted in sales floor
                                'If not counted then add the item to the Counted Items list
                                'Dim bIsProductAlreadyCounted As Boolean = False

                                'For Each objCountedProductData As CLProductCountedData In objAppContainer.m_CLSalesFloorCountedInfoList
                                '    If objCountedProductData.m_strListId.Equals(m_CLCurrentProductGroup.ListID) AndAlso _
                                '    objCountedProductData.m_strProductCode.Equals(m_CLCurrentItemInfo.ProductCode) Then
                                '        bIsProductAlreadyCounted = True
                                '        Exit For
                                '    End If
                                'Next

                                'If Not bIsProductAlreadyCounted Then
                                '    objAppContainer.objCLSummary.iSFListCounted += 1
                                '    m_iSalesFloorItemCount = m_iSalesFloorItemCount + 1
                                '    Dim objSalesFloorCountedProduct As CLProductCountedData = New CLProductCountedData()
                                '    objSalesFloorCountedProduct.m_strProductCode = m_CLCurrentItemInfo.ProductCode
                                '    objSalesFloorCountedProduct.m_strListId = m_CLCurrentProductGroup.ListID

                                '    objAppContainer.m_CLSalesFloorCountedInfoList.Add(objSalesFloorCountedProduct)
                                'End If

                                'Send CLC once count is reset
                                '#If RF Then
                                '                                Dim objCLCRecord As CLCRecord = New CLCRecord()
                                '                                'Sets the values
                                '                                objCLCRecord.strListID = m_CLCurrentProductGroup.ListID
                                '                                objCLCRecord.strNumberSEQ = m_CLCurrentItemInfo.SequenceNumber
                                '                                objCLCRecord.strBootscode = m_CLCurrentItemInfo.BootsCode
                                '                                objCLCRecord.strCountLocation = Macros.SHOP_FLOOR
                                '                                objCLCRecord.strCount = m_CLCurrentItemInfo.SalesFloorQuantity
                                '                                objCLCRecord.strUpdateOSSR = " "

                                '                                If Not objAppContainer.objExportDataManager.CreateCLC(objCLCRecord) Then
                                '                                    objAppContainer.objLogger.WriteAppLog("Could not UpdateSalesFloorProductInfo of CLSessionMgr." _
                                '                                         , Logger.LogLevel.RELEASE)
                                '                                    Return False

                                '                                End If
                                '#End If
                                'If Not m_CLCurrentProductGroup.IsActive Then
                                '    m_CLCurrentProductGroup.IsActive = True
                                'End If
                                '''''''''
                                If m_EntryType = BCType.SEL Then
                                    Return True
                                Else
                                    CLSessionMgr.GetInstance().DisplayCLScreen(CLSCREENS.SalesFloorProductCount)
                                End If
                            Else
                                ProcessItemDetailsNext()
                            End If
                        Else

                            'If the count location is sales floor, display sales floor product count screen
                            If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                                CLSessionMgr.GetInstance().DisplayCLScreen(CLSCREENS.SalesFloorProductCount)
                                'If the count location is back shop, display sales floor product count screen
                            ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                                CLSessionMgr.GetInstance().DisplayCLScreen(CLSCREENS.BackShopProductCount)
                                'ambli
                                'For OSSR
#If RF Then
                                'If objAppContainer.OSSRStoreFlag = "Y" Then
                            ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                                CLSessionMgr.GetInstance().DisplayCLScreen(CLSCREENS.OSSRProductCount)
                                'End If
#End If
                            End If
                        End If
                    End If
                End If
            End If
            ' End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessBarcodeEntered of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessBarcodeEntered of CLSessionMgr", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Displays the Count List Home Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayCLHome(ByVal o As Object, ByVal e As EventArgs)
        arrTemp = New ArrayList()
        Try
            objAppContainer.objLogger.WriteAppLog("Entered DisplayCLHome of CLSessionMgr", Logger.LogLevel.INFO)
            '&&&&&&&&&&&&&&&&&&&&&&&
            objAppContainer.objCLSummary.iBSListCounted = 0
            objAppContainer.objCLSummary.iSFListCounted = 0
#If RF Then
            objAppContainer.objGlobalCLProductGroupList.Clear()
            objAppContainer.objGlobalCLProductInfoTable.Clear()
            objAppContainer.objGlobalCLSiteInfoTable.Clear()
            objAppContainer.objGlobalCLInfoTable.Clear()
            objAppContainer.objCLSummary.iOSSRListCounted = 0
            objAppContainer.objDataEngine.GetCountList(objAppContainer.objGlobalCLProductGroupList)
#End If
            '&&&&&&&&&&&&&&&&&&&&&&&
            If m_bIsCreateOwnList Then
                If Not m_CLItemList.Count = 0 Then
                    arrTemp = m_CLItemList.Clone()
                    objAppContainer.objGlobalCreateCountList.Add(m_CLCurrentProductGroup.ListID, arrTemp)
                End If
                m_CLCurrentProductGroup = m_ProductGroup
            End If

            m_bIsCreateOwnList = False
            objAppContainer.bIsCreateOwnList = False
            'm_CLCurrentProductGroup = New CLProductGroupInfo()
            With m_CLHome
                'Check if product group list is empty
                'If so, display home screen with create count list and other buttons
                If objAppContainer.objGlobalCLProductGroupList.Count() = 0 Then
                    .lstvwProductGroup.Visible = False
                    .lstvwProductGroup.Clear()
                Else
                    .lstvwProductGroup.Visible = True
                    .lstvwProductGroup.Clear()
                    'Stock File Accuracy  - Change Heading
                    .lstvwProductGroup.Columns.Add("ID", 49 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    .lstvwProductGroup.Columns.Add("List Name", 106 * objAppContainer.iOffSet, HorizontalAlignment.Center)
                    .lstvwProductGroup.Columns.Add("Items", 55 * objAppContainer.iOffSet, HorizontalAlignment.Center)

                    'Populate data from the list to list view
                    For Each objProductGroup As CLProductGroupInfo In objAppContainer.objGlobalCLProductGroupList
                        'If Not m_CLCurrentProductGroup.ListID Is Nothing Then
                        '    If m_CLCurrentProductGroup.ListID.Equals(objProductGroup.ListID) Then
                        If objProductGroup.IsComplete Then
                            Continue For
                        End If
                        '    End If
                        'End If
                        Dim strListType As String = Nothing
                        Dim lstItem As ListViewItem = New ListViewItem()
                        If objProductGroup.ListType.Equals("H") Then
                            strListType = "SO"
                        ElseIf objProductGroup.ListType.Equals("R") Then
                            strListType = "R"
                        ElseIf objProductGroup.ListType.Equals("U") Then
                            strListType = "U"
                        ElseIf objProductGroup.ListType.Equals("N") Then
                            strListType = "NEG"
                        End If
                        lstItem.Text = Convert.ToInt16(objProductGroup.ListID).ToString() + strListType
                        lstItem.SubItems.Add(objProductGroup.ListDescription)
#If NRF Then
                        lstItem.SubItems.Add(objProductGroup.NumberOfItems)
#ElseIf RF Then
                        If objProductGroup.ActiveType = "A" Then
                            lstItem.SubItems.Add(objProductGroup.NumberOfItems.ToString() + "*")
                        Else
                            lstItem.SubItems.Add(objProductGroup.NumberOfItems)
                        End If
#End If
                        'If Not m_CLCurrentProductGroup.ListID Is Nothing Then
                        '    If m_CLCurrentProductGroup.ListID.Equals(objProductGroup.ListID) Then
                        '        If m_CLCurrentProductGroup.IsActive Then
                        '            lstItem.SubItems.Add(objProductGroup.NumberOfItems.ToString() + "*")
                        '        Else
                        '            lstItem.SubItems.Add(objProductGroup.NumberOfItems)
                        '        End If
                        '    Else
                        '        lstItem.SubItems.Add(objProductGroup.NumberOfItems)
                        '    End If
                        'Else
                        '    lstItem.SubItems.Add(objProductGroup.NumberOfItems)
                        'End If
                        .lstvwProductGroup.Items.Add(lstItem)
                        lstItem = Nothing
                    Next
                End If

                If .lstvwProductGroup.Items.Count = 0 Then
                    .lstvwProductGroup.Visible = False
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M11"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                    objAppContainer.objLogger.WriteAppLog(MessageManager.GetInstance().GetMessage("M11"), Logger.LogLevel.INFO)
                End If

                'Retrieves the store id and sets it to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayCLHome of CLSessionMgr. Exception is:" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCLHome of CLSessionMgr", Logger.LogLevel.INFO)
    End Sub
#Region "Create Own List"
    ''' <summary>
    ''' Initialize variables for create own list
    ''' </summary>

    Public Function StartSessionCOL() As Boolean
        'Send CLA transact for starting create own list
#If RF Then

        If Not objAppContainer.objDataEngine.GetCreateCountListID(Macros.START_USER_COUNTLIST, objCreateCountList) Then
            objAppContainer.objLogger.WriteAppLog("Cannot Create CLA record at Create Count Start Session", _
                                                      Logger.LogLevel.RELEASE)
            Return False
        Else
            strListId_RF = objCreateCountList.ListID
            strSeq_RF = "001"
        End If
        strListId = strListId_RF
#Else
        strListId = "U" + objAppContainer.iListCount.ToString()
#End If
        'm_ProductGroup = m_CLCurrentProductGroup
        m_COLItemScan = New frmCOLItemScan()
        m_CLProductInfo = New CLProductInfo()
        m_SelectedPOGSeqNum = Nothing
        m_ItemScreen = Nothing
        m_bIsSiteInfo = False
        'strListId = "U" + iListCount.ToString()
        m_CLItemList = New ArrayList()
        'm_CLCurrentProductGroup.ListID = ""
        m_CLCurrentProductGroup = New CLProductGroupInfo()
        m_iBackNextCount = -1
        m_bIsBackNext = False
        m_bNavigation = False
        m_bPlannerFlag = False
        m_bIsItemsInPSP = False
        m_bISItemsInOSSRPSP = False
        m_iListCreatedLoc = Nothing
        strCOLListId = ""
        m_SELQueued = 0
        CLSessionMgr.GetInstance().m_bIsCreateOwnList = True
        objAppContainer.bIsCreateOwnList = True
        arr_NOPItemList = New ArrayList()
        arr_UnItemList = New ArrayList()
        CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.COLLocationSelection)
        Return True

    End Function
    ''' <summary>
    ''' Populates  data for site information screen for Create own list
    ''' </summary>
    Public Sub PopulateCOLSiteInfo()
        Dim objSiteInfo As New CLMultiSiteInfo()
        Dim arrMultiSite As New ArrayList()
        Dim iNotOnPlannerCount As Integer = 0
        Dim arrNotOnPlannerList As New ArrayList()
        Dim iUnknownCount As Integer = 0
        Dim arrDescriptionList As ArrayList
        iNotOnPlannerCount = m_CLCurrentProductGroup.NotOnPlannerItemCount
        iUnknownCount = m_CLCurrentProductGroup.UnknownItemCount

        If Not m_CLCurrentProductGroup.ListID = strListId Then
            'strCOLListId = strListId
            CreateDataTable()
            m_CLCurrentProductGroup = New CLProductGroupInfo()
            m_CLCurrentProductGroup.ListID = strListId
            m_CLCurrentProductGroup.ListDescription = "User Generated Count List"
            m_CLCurrentProductGroup.ListType = "U"
            m_CLCurrentProductGroup.NotOnPlannerItemCount = iNotOnPlannerCount
            m_CLCurrentProductGroup.NotOnPlannerItemList = arr_NOPItemList
            m_CLCurrentProductGroup.UnknownItemCount = iUnknownCount
            m_CLCurrentProductGroup.UnknownItemList = arr_UnItemList
            'm_CLCurrentProductGroup.NumberOfItems = m_CLItemList.Count()
            'If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
            For Each objProductInfo As CLProductInfo In m_CLItemList
                arrDescriptionList = New ArrayList()
                If Not objProductInfo.IsUnknownItem Then
                    arrMultiSite = objProductInfo.SFMultiSiteList
                    For Each objSiteInfo In arrMultiSite
                        If objSiteInfo.strPlannerDesc <> "Other" Then
                            If objSiteInfo.strPlannerDesc <> "Not On Planner" Then
                                If Not arrDescriptionList.Contains(objSiteInfo.strPOGDescription.Trim()) Then
                                    rowData = objCountListDataTable.NewRow()
                                    rowData.Item("List_ID") = m_CLCurrentProductGroup.ListID
                                    rowData.Item("Boots_Code") = objProductInfo.BootsCode
                                    rowData.Item("Planner_Desc") = objSiteInfo.strPlannerDesc
                                    rowData.Item("Repeat_Count") = objSiteInfo.iRepeatCount
                                    rowData.Item("POG_Description") = objSiteInfo.strPOGDescription.Trim()
                                    objCountListDataTable.Rows.Add(rowData)
                                    arrDescriptionList.Add(objSiteInfo.strPOGDescription.Trim())
                                End If
                            End If
                        End If
                    Next
                End If
            Next
            'End If
            PopulateRFSiteInfoData(objCountListDataTable)
            objAppContainer.objGlobalCLProductInfoTable.Add(m_CLCurrentProductGroup.ListID, m_CLItemList)
            objAppContainer.iListCount = objAppContainer.iListCount + 1
        End If
    End Sub
    ''' <summary>
    ''' Displays the Create Own List Location Selection Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayCOLLocationSelection(ByVal o As Object, ByVal e As EventArgs)
        Dim iItemBackShopCount As Integer = 0
        Dim iItemSalesFloorCount As Integer = 0
        Dim iTotalOSSRItemCount As Integer = 0
        Dim iTotalItemInList As Integer = 0
        Dim iItemOSSRCount As Integer = 0
        Try
            With m_CLLocSelection

                m_SelectedPOGSeqNum = Nothing
                .Text = "Create Count"
                If m_CLItemList.Count <> 0 Then
                    For Each objCurrentProductInfo As CLProductInfo In m_CLItemList
                        'If Not objCurrentProductInfo.IsNotPlannerItem Then
                        If objCurrentProductInfo.SalesFloorQuantity >= 0 Then
                            iItemSalesFloorCount = iItemSalesFloorCount + 1
                        End If
                        If objCurrentProductInfo.BackShopQuantity >= 0 Then
                            iItemBackShopCount = iItemBackShopCount + 1
                        End If
#If RF Then
                            If objAppContainer.OSSRStoreFlag = "Y" Then
                                m_CLLocSelection.btn_OSSR.Visible = True
                                m_CLLocSelection.btn_OSSR.Enabled = True
                                m_CLLocSelection.lblNumOSSRItems.Visible = True
                                If objCurrentProductInfo.OSSRQuantity >= 0 Then
                                    iItemOSSRCount = iItemOSSRCount + 1
                                End If
                                If objCurrentProductInfo.OSSRFlag = "O" Then
                                    iTotalOSSRItemCount = iTotalOSSRItemCount + 1
                                End If
                            Else
                                m_CLLocSelection.btn_OSSR.Visible = False
                                m_CLLocSelection.btn_OSSR.Enabled = False
                                m_CLLocSelection.lblNumOSSRItems.Visible = False
                            End If
#End If
                        'End If
                    Next
                    iTotalItemInList = m_CLItemList.Count

                    If m_iListCreatedLoc = Macros.COUNT_SALES_FLOOR Then
                        .lblNumSalesFloorItems.Text = iTotalItemInList.ToString() + " Items"
#If NRF Then
                        .lblNumBackShopItems.Text = iItemBackShopCount.ToString() + "/" + iTotalItemInList.ToString() + " Items"
#ElseIf RF Then
                        .lblNumBackShopItems.Text = iItemBackShopCount.ToString() + "/" + (iTotalItemInList - iTotalOSSRItemCount).ToString() + " Items"
                        .lblNumOSSRItems.Text = iItemOSSRCount.ToString() + "/" + iTotalOSSRItemCount.ToString() + " Items"
#End If
                    End If

                    If m_iListCreatedLoc = Macros.COUNT_BACK_SHOP Then
                        .lblNumBackShopItems.Text = iTotalItemInList.ToString() + " Items"
                        .lblNumSalesFloorItems.Text = iItemSalesFloorCount.ToString() + "/" + iTotalItemInList.ToString() + " Items"
                        .lblNumOSSRItems.Text = iItemOSSRCount.ToString() + "/" + iTotalOSSRItemCount.ToString() + " Items"
                    End If

                    If m_iListCreatedLoc = Macros.COUNT_OSSR Then
                        .lblNumOSSRItems.Text = iTotalItemInList.ToString() + " Items"
                        .lblNumSalesFloorItems.Text = iItemSalesFloorCount.ToString() + "/" + iTotalItemInList.ToString() + " Items"
                        .lblNumBackShopItems.Text = iItemBackShopCount.ToString() + "/" + (iTotalItemInList - iTotalOSSRItemCount).ToString() + " Items"
                    End If
                    .lblNumSalesFloorItems.Visible = True
                    .lblNumBackShopItems.Visible = True

                Else
#If RF Then
                    If objAppContainer.OSSRStoreFlag = "Y" Then
                        m_CLLocSelection.btn_OSSR.Visible = True
                        m_CLLocSelection.btn_OSSR.Enabled = True
                        m_CLLocSelection.lblNumOSSRItems.Visible = True
                    Else
                        m_CLLocSelection.btn_OSSR.Visible = False
                        m_CLLocSelection.btn_OSSR.Enabled = False
                        m_CLLocSelection.lblNumOSSRItems.Visible = False
                    End If
#End If
                    .lblNumBackShopItems.Visible = False
                    .lblNumSalesFloorItems.Visible = False
                    .lblNumOSSRItems.Visible = False
                    .lblProductName.Visible = False
                    .lblStatus.Visible = False

                End If
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception ocurred in DisplayCOLLocationSelection of CLSessionMgr. Exception is:" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCOLLocationSelection of CLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Displays the Create Own List Item scan Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayCOLItemScanScreen(ByVal o As Object, ByVal e As EventArgs)
        Try
            Dim iFlag As Boolean = False
            With m_COLItemScan
                If m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                    .Text = "Create Count - BS"
                ElseIf m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                    .Text = "Create Count - SF"
                End If
                If m_CLItemList.Count > 0 Then
                    .btnView.Visible = True
                    .lblSELtxt.Text = m_SELQueued
                Else
                    .btnView.Visible = False
                    '.lblItemtxt.Text = 0
                    .lblSELtxt.Text = 0
                End If
                m_iLocation = Macros.COUNT_LIST_ITEMSCAN
                bIsItemScan = True
                m_ItemScreen = Nothing
                m_SelectedPOGSeqNum = Nothing
                'Display 
                'For Each objProductInfo As CLProductInfo In m_CLItemList
                '    If objProductInfo.IsNotPlannerItem Then
                '        iFlag = True
                '        Exit For
                '    End If
                'Next
                'If iFlag Then
                '    .lblItemtxt.Text = m_CLItemList.Count - 1
                'Else
                .lblItemtxt.Text = m_CLItemList.Count
                'End If
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception ocurred in DisplayCOLHome of CLSessionMgr. Exception is:" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCOLHome of CLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Get all Multisite locations for an item
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <param name="arrPlannerList"></param>
    ''' <remarks></remarks>
    Private Sub GetMultiSiteLocations(ByVal strBootsCode As String, ByVal arrPlannerList As ArrayList)
        Dim multiSiteList As ArrayList = New ArrayList()
        Dim objCLData As CLMultiSiteInfo
        Dim RepeatCount As Integer = 0
        Try
            'MultiSite for Sales floor
            If objAppContainer.objDataEngine.GetPlannerListUsingBC(strBootsCode, True, arrPlannerList) Then

                'objAppContainer.objDataEngine.GetMultiSiteDetails(strListId_RF, strBootsCode, arrPlannerList)
                For Each ObjPlanner As PlannerInfo In arrPlannerList
                    RepeatCount = Convert.ToInt16(ObjPlanner.RepeatCount.ToString().Trim())
                    For Counter As Integer = 1 To RepeatCount
                        multiSiteList.Add(ObjPlanner)
                    Next
                Next
                'If multiSiteList.Count > 1 Then
                m_CLProductInfo.SFMultiSiteList = New ArrayList()
                objCLData = New CLMultiSiteInfo()
                Dim objSeqNum As Integer = -1
                For Each strLocation As PlannerInfo In multiSiteList
                    objSeqNum = objSeqNum + 1
                    objCLData = New CLMultiSiteInfo
                    objCLData.strSeqNumber = objSeqNum
                    objCLData.strPOGDescription = strLocation.POGDesc.Trim() + " - " + strLocation.Description.Trim()
                    objCLData.strPlannerDesc = strLocation.POGDesc.Trim()
                    'objCLData.strPOGDescription = strLocation.POGDesc
                    objCLData.strSalesFloorQuantiy = -1
                    objCLData.IsCounted = "N"
                    objCLData.strPlannerID = strLocation.PlannerID
                    objCLData.iRepeatCount = strLocation.RepeatCount
                    m_CLProductInfo.SFMultiSiteList.Add(objCLData)
                Next
                If multiSiteList.Count > 1 Then
                    Dim objOtherSiteInfo As New CLSessionMgr.CLMultiSiteInfo()
                    objOtherSiteInfo.strPlannerID = "Other"
                    objOtherSiteInfo.strPOGDescription = "Other"
                    objOtherSiteInfo.strPlannerDesc = "Other"
                    objOtherSiteInfo.IsCounted = "Y"
                    objOtherSiteInfo.strSalesFloorQuantiy = -1
                    objOtherSiteInfo.strSeqNumber = objSeqNum + 1
                    m_CLProductInfo.SFMultiSiteList.Add(objOtherSiteInfo)
                End If
            End If
            'm_CLProductInfo.SFMultiSiteList = multiSiteList
            'End If
            'MultiSite for Back Shop
            multiSiteList = New ArrayList()
            objCLData = New CLMultiSiteInfo()
            objCLData.strPOGDescription = ("Main Back Shop")
            objCLData.strPlannerDesc = Macros.COUNT_MBS
            objCLData.strPlannerID = Macros.COUNT_MBS
            objCLData.strSeqNumber = 0
            objCLData.IsCounted = "N"
            objCLData.strBackShopQuantiy = -1
            multiSiteList.Add(objCLData)
            If m_CLProductInfo.PendingSalesFlag Then
                objCLData = New CLMultiSiteInfo()
                objCLData.strPOGDescription = ("Pending Sales Plan")
                objCLData.strPlannerID = Macros.COUNT_PSP
                objCLData.strPlannerDesc = Macros.COUNT_PSP
                objCLData.IsCounted = "N"
                objCLData.strBackShopQuantiy = -1
                objCLData.strSeqNumber = 1
                multiSiteList.Add(objCLData)
            End If
            m_CLProductInfo.BSMultiSiteList = multiSiteList
#If RF Then
            'multisitelist for OSSR
            multiSiteList = New ArrayList()
            objCLData = New CLMultiSiteInfo()
            objCLData.strPOGDescription = ("OSSR")
            objCLData.strPlannerDesc = Macros.COUNT_OSSR_BS
            objCLData.strPlannerID = Macros.COUNT_OSSR_BS
            objCLData.strSeqNumber = 0
            objCLData.IsCounted = "N"
            objCLData.strOSSRQuantiy = -1
            multiSiteList.Add(objCLData)
            If m_CLProductInfo.PendingSalesFlag Then
                objCLData = New CLMultiSiteInfo()
                objCLData.strPOGDescription = ("Pending Sales Plan")
                objCLData.strPlannerID = Macros.COUNT_OSSR_PSP
                objCLData.strPlannerDesc = Macros.COUNT_OSSR_PSP
                objCLData.IsCounted = "N"
                objCLData.strOSSRQuantiy = -1
                objCLData.strSeqNumber = 1
                multiSiteList.Add(objCLData)
            End If
            m_CLProductInfo.OSSRMultiSiteList = multiSiteList
#End If
            m_CLProductInfo.SalesFloorQuantity = -1
            m_CLProductInfo.BackShopQuantity = -1
            m_CLProductInfo.BackShopMBSQuantity = -1
            m_CLProductInfo.OSSRQuantity = -1
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in GetMultiSiteLocations of CLSessionMgr" + ex.StackTrace(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Gets the non planner item details
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetUnknownItemDetails(ByVal iProductFlag As Boolean)

        'Dim objCLData As CLMultiSiteInfo = New CLMultiSiteInfo()
        'Dim arrMultisite As ArrayList = New ArrayList()
        'objCLData.strSeqNumber = 0
        'objCLData.strPOGDescription = "Not on Planner"
        'objCLData.strPlannerDesc = "Not on Planner"
        'objCLData.strSalesFloorQuantiy = -1
        'objCLData.IsCounted = "N"
        'objCLData.strPlannerID = "Not on Planner"
        'objCLData.strBackShopQuantiy = -1
        'arrMultisite.Add(objCLData)
        'm_CLProductInfo.SFMultiSiteList = arrMultisite
        'arrMultisite = New ArrayList()
        'objCLData.strSeqNumber = 0
        'objCLData.strPOGDescription = "Not on Planner"
        'objCLData.strPlannerDesc = "Not on Planner"
        'objCLData.strSalesFloorQuantiy = -1
        'objCLData.IsCounted = "N"
        'objCLData.strPlannerID = "Not on Planner"
        'objCLData.strBackShopQuantiy = -1
        'arrMultisite.Add(objCLData)
        'm_CLProductInfo.BSMultiSiteList = arrMultisite
        'm_bIsItemNotInPlanner = True
        'm_bPlannerFlag = True
        'm_bIsMultisited = False

        isNewItem = True
        m_bIsItemNotInPlanner = False
        m_CLProductInfo.Sequence = m_iSequence
        m_iSequence += 1
        m_iBackNextCount = m_iBackNextCount + 1
        m_bIsBackNext = False

        Dim multiSiteList As New ArrayList()
        Dim objCLData As CLMultiSiteInfo
        Dim objOtherSiteInfo As New CLSessionMgr.CLMultiSiteInfo()
        Dim arrUnKnownPlannerList As New ArrayList()
        objOtherSiteInfo.strPlannerID = "Unknown"
        objOtherSiteInfo.strPOGDescription = "Unknown"
        objOtherSiteInfo.strPlannerDesc = "Unknown"
        objOtherSiteInfo.IsCounted = "N"
        objOtherSiteInfo.strSalesFloorQuantiy = -1
        objOtherSiteInfo.strSeqNumber = 0

        arrUnKnownPlannerList.Add(objOtherSiteInfo)

        m_CLProductInfo.SFMultiSiteList = arrUnKnownPlannerList

        objCLData = New CLMultiSiteInfo()
        objCLData.strPOGDescription = ("Main Back Shop")
        objCLData.strPlannerDesc = Macros.COUNT_MBS
        objCLData.strPlannerID = Macros.COUNT_MBS
        objCLData.strSeqNumber = 0
        objCLData.IsCounted = "N"
        objCLData.strBackShopQuantiy = -1
        multiSiteList.Add(objCLData)

        m_CLProductInfo.BSMultiSiteList = multiSiteList
        If iProductFlag Then
            m_CLProductInfo.BootsCode = objAppContainer.objHelper.GeneratePCwithCDV(m_CLProductInfo.BootsCode.TrimStart("0"))
        End If

        'm_CLProductInfo.BootsCode = objAppContainer.objHelper.GeneratePCwithCDV(m_CLProductInfo.BootsCode.TrimStart("0"))

        m_CLProductInfo.MultiSiteCount = 0
        m_CLProductInfo.MSFlag = "  "
        m_bIsMultisited = False
        m_CLProductInfo.IsUnknownItem = True

        'arr_UnItemList.Add(m_CLProductInfo)
        'm_CLCurrentProductGroup.UnknownItemCount = m_CLCurrentProductGroup.UnknownItemCount + 1

        m_CLProductInfo.SalesFloorQuantity = -1
        m_CLProductInfo.BackShopQuantity = -1
        m_CLProductInfo.BackShopMBSQuantity = -1
    End Sub
    ''' <summary>
    ''' Updates the Multisites combo box with POG descriptions
    ''' Add the product and the POG descriptions to the MS tracker
    ''' </summary>
    ''' <param name="PogList">the List of POG where the product is present</param>
    ''' <param name="strProdCode">Product code of the product</param>
    ''' <remarks></remarks>
    Private Sub PopulateMultiSites(ByVal PogList As ArrayList, ByVal strProdCode As String)
        Try
            Dim iCnt As Integer = New Integer()
            m_CLItemDetails.cmbMultiSite.Items.Clear()
            m_CLItemDetails.cmbMultiSite.Items.Add("SELECT")

            For iCnt = 0 To PogList.Count - 1
                Dim objPlannerInfo As PlannerInfo = New PlannerInfo()
                Dim objCLMultiSiteInfo As CLMultiSiteInfo = New CLMultiSiteInfo
                objPlannerInfo = PogList.Item(iCnt)
                m_CLItemDetails.cmbMultiSite.Items.Add(objPlannerInfo.POGDesc.ToString.Trim() _
                                                        & " - " & objPlannerInfo.Description.ToString.Trim())
            Next iCnt

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " Exception in CLSessionMgr::" + ex.StackTrace(), Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit CLSessionMgr PopulateMultiSites", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Processes the counting done on sales floor
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="iCOLQty"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessCOLItemCounted(ByVal strProductCode As String, ByVal iCOLQty As Integer, _
                                                 ByVal strBootsCode As String) As Boolean
        Try
            objAppContainer.objLogger.WriteAppLog("Entered ProcessSalesFloorItemCounted of CLSessionMgr", Logger.LogLevel.INFO)

            'To unformat the product code by removing "-" and then remove CDV from that value
            Dim strUnFormattedProductCode As String
            Dim strUnFormatBootsCode As String
            Dim iSalesFloorQty As Integer = 0
            Dim iBackShopQty As Integer = 0
            Dim iOSSRQty As Integer = 0

            strUnFormattedProductCode = objAppContainer.objHelper.UnFormatBarcode(strProductCode)
            strUnFormattedProductCode = strUnFormattedProductCode.Remove(strUnFormattedProductCode.Length - 1, 1)
            strUnFormatBootsCode = objAppContainer.objHelper.UnFormatBarcode(strBootsCode)
            If iCOLQty = "0" Then
                If (m_EntryType = BCType.EAN) Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M51"), "Invalid Data", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Exclamation, _
                                    MessageBoxDefaultButton.Button2)
                    Return True
                End If
            End If
            
            'Updates the list with modified data
            If m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                iBackShopQty = iCOLQty
            ElseIf m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                iSalesFloorQty = iCOLQty
            ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                iOSSRQty = iCOLQty
            End If
            If UpdateCOLProductInfo(strUnFormattedProductCode, iSalesFloorQty, iBackShopQty, iOSSRQty, strUnFormatBootsCode) Then
                If m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                    DisplayCLScreen(CLSCREENS.BackShopProductCount)
                ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                    DisplayCLScreen(CLSCREENS.OSSRProductCount)
                ElseIf m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                    'Displays the Sales Floor Count screen
                    DisplayCLScreen(CLSCREENS.SalesFloorProductCount)
                End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessSalesFloorItemCounted of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try

        objAppContainer.objLogger.WriteAppLog("Exit ProcessSalesFloorItemCounted of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Updates the CLProductInfo object with the change in qty in salesfloor/backshop/ossr
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="iSalesFloorQty"></param>
    ''' <param name="iBackShopQty"></param>
    ''' <param name="iOSSRQty"></param>
    ''' <param name="strBootsCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateCOLProductInfo(ByVal strProductCode As String, ByVal iSalesFloorQty As Integer, _
                                                   ByVal iBackShopQty As Integer, ByVal iOSSRQty As Integer, ByVal strBootsCode As String) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered UpdateCOLProductInfo of CLSessionMgr", Logger.LogLevel.INFO)
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim objCurrentCLSiteInfoTable As Hashtable = New Hashtable()
        Dim objCurrentSFSiteInfoList As ArrayList = New ArrayList()
        Dim objCurrentBSSiteInfoList As ArrayList = New ArrayList()
        Dim objCurrentCountListInfoTable As New Hashtable()
        Dim objProductInfo As New CLProductInfo()
        Dim objCurrentProductList As New ArrayList()
        Dim objCurrentProductInfo As New CLProductInfo()
        Dim objSiteInfo As New CLMultiSiteInfo()

        Dim strPlannerDesc As String = Nothing
        Dim iItemIndex As Integer = -1
        Dim iSiteIndex As Integer = -1
        Dim iInfoIndex As Integer = -1
        Dim iSalesFloorVal As Integer = -1
        Dim iBackShopVal As Integer = -1
        Dim iItemQty As Integer = -1
        Dim iTotalSalesFloorQty As Integer = 0
        Dim iTotalBackShopQty As Integer = 0
        Dim iTotalOSSRQty As Integer = 0
        'Dim objCurrentCountListInfoTable As New Hashtable()
        'Dim objCurrentProductInfoList As New ArrayList()
        Try

            'objCurrentSFSiteInfoList = m_CLProductInfo.SFMultiSiteList
            'objCurrentBSSiteInfoList = m_CLProductInfo.BSMultiSiteList

            'm_CLProductInfo.BackShopQuantity = -1
            'm_CLProductInfo.SalesFloorQuantity = -1
            If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                For Each objSiteInfo In m_CLProductInfo.SFMultiSiteList
                    If m_CLProductInfo.IsUnknownItem Or m_SelectedPOGSeqNum = Nothing Then
                        m_SelectedPOGSeqNum = 0
                    End If
                    'If Not objSiteInfo.strPlannerDesc.Equals("Other") Then
                    If objSiteInfo.strSeqNumber.Equals(m_SelectedPOGSeqNum) Then
                        If objSiteInfo.IsCounted.Equals("N") Or objSiteInfo.strPOGDescription.Equals("Other") Then
                            objSiteInfo.strPOGDescription = "* " & objSiteInfo.strPOGDescription & "  (Counted)"
                            objSiteInfo.IsCounted = "Y"
                        End If
                        objSiteInfo.strSalesFloorQuantiy = iSalesFloorQty
                        strPlannerDesc = objSiteInfo.strPlannerDesc
                        iSiteIndex = iSiteIndex + 1
                        Exit For
                    End If
                    iSiteIndex = iSiteIndex + 1
                    'End If
                Next

            ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                For Each objSiteInfo In m_CLProductInfo.BSMultiSiteList
                    If m_CLProductInfo.IsUnknownItem Then
                        m_SelectedPOGSeqNum = 0
                    End If
                    If objSiteInfo.strSeqNumber.Equals(m_SelectedPOGSeqNum) Then
                        If objSiteInfo.IsCounted.Equals("N") Then
                            objSiteInfo.strPOGDescription = "* " & objSiteInfo.strPOGDescription & "  (Counted)"
                            objSiteInfo.IsCounted = "Y"
                            m_CLProductInfo.TotalBSSiteCount = m_CLProductInfo.TotalBSSiteCount + 1
                        End If
                        objSiteInfo.strBackShopQuantiy = iBackShopQty
                        strPlannerDesc = objSiteInfo.strPlannerDesc
                        If m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                            If m_SelectedPOGSeqNum = Macros.BS_SELECTED_INDEX Then
                                m_CLProductInfo.BackShopMBSQuantity = iBackShopQty
                            Else
                                m_CLProductInfo.BackShopPSPQuantity = iBackShopQty
                            End If
                        End If
                        iSiteIndex = iSiteIndex + 1
                        Exit For
                    End If
                    iSiteIndex = iSiteIndex + 1
                Next
                If m_CLProductInfo.IsUnknownItem Then
                    strPlannerDesc = Macros.COUNT_BS_UNKNOWN
                End If
            ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                For Each objSiteInfo In m_CLProductInfo.OSSRMultiSiteList
                    If m_CLProductInfo.IsUnknownItem Then
                        m_SelectedPOGSeqNum = 0
                    End If
                    If objSiteInfo.strSeqNumber.Equals(m_SelectedPOGSeqNum) Then
                        If objSiteInfo.IsCounted.Equals("N") Then
                            objSiteInfo.strPOGDescription = "* " & objSiteInfo.strPOGDescription & "  (Counted)"
                            objSiteInfo.IsCounted = "Y"
                            m_CLProductInfo.TotalOSSRSiteCount = m_CLProductInfo.TotalOSSRSiteCount + 1
                        End If
                        objSiteInfo.strOSSRQuantiy = iOSSRQty
                        strPlannerDesc = objSiteInfo.strPlannerDesc
                        If m_iCountedLocation = Macros.COUNT_OSSR Then
                            If m_SelectedPOGSeqNum = Macros.BS_SELECTED_INDEX Then
                                m_CLProductInfo.OSSRMBSQuantity = iOSSRQty
                            Else
                                m_CLProductInfo.OSSRPSPQuantity = iOSSRQty
                            End If
                        End If
                        iSiteIndex = iSiteIndex + 1
                        Exit For
                    End If
                    iSiteIndex = iSiteIndex + 1
                Next
            End If
            'objProductInfo.CountStatus = "Y"

            'retrieves the total count of item in all sites for SalesFloor


            Dim iTempSFQty As Integer
            For Each objSiteInfo In m_CLProductInfo.SFMultiSiteList
                If objSiteInfo.strSalesFloorQuantiy < 0 Then
                    iTempSFQty = 0
                Else
                    iTempSFQty = objSiteInfo.strSalesFloorQuantiy
                End If
                iTotalSalesFloorQty = iTotalSalesFloorQty + iTempSFQty
            Next


            'retrieves the total count of item in all sites for BackShop

            If Not m_CLProductInfo.BSMultiSiteList Is Nothing Then
                Dim iTempBSQty As Integer
                For Each objSiteInfo In m_CLProductInfo.BSMultiSiteList
                    If objSiteInfo.strBackShopQuantiy < 0 Then
                        iTempBSQty = 0
                    Else
                        iTempBSQty = objSiteInfo.strBackShopQuantiy
                    End If
                    iTotalBackShopQty = iTotalBackShopQty + iTempBSQty
                Next
            End If

            'Update values tosend CLC request
            If Not m_CLProductInfo.OSSRMultiSiteList Is Nothing Then
                Dim iTempOSSRQty As Integer
                For Each objSiteInfo In m_CLProductInfo.OSSRMultiSiteList
                    If objSiteInfo.strOSSRQuantiy < 0 Then
                        iTempOSSRQty = 0
                    Else
                        iTempOSSRQty = objSiteInfo.strOSSRQuantiy
                    End If
                    iTotalOSSRQty = iTotalOSSRQty + iTempOSSRQty
                Next
            End If

            'update the total quantity to be displayed
            If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                m_CLProductInfo.IsSFItemCounted = True
                m_CLProductInfo.SalesFloorQuantity = iTotalSalesFloorQty
                iItemQty = iSalesFloorQty
            ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                If m_SelectedPOGSeqNum = Macros.BS_SELECTED_INDEX Then
                    m_CLProductInfo.IsMBSItemCounted = True
                Else
                    m_CLProductInfo.IsBSPSPItemCounted = True
                End If
                m_CLProductInfo.BackShopQuantity = iTotalBackShopQty
                iItemQty = iBackShopQty
            ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                m_CLProductInfo.OSSRQuantity = iOSSRQty
                iItemQty = iOSSRQty
            End If
#If RF Then

            'Dim iOSSRVal As Integer = 0
            'If objAppContainer.OSSRStoreFlag = "Y" Then
            '    iOSSRVal = -1
            'End If
            m_CLProductInfo.TotalQuantity = iTotalSalesFloorQty + iTotalBackShopQty + iTotalOSSRQty
            'If m_CLProductInfo.MultiSiteCount = 0 Then
            '    If m_iListCreatedLoc = Macros.COUNT_SALES_FLOOR Then
            '        iItemQty = iSalesFloorQty
            '    End If
            'End If
            'End if

#ElseIf NRF Then
            m_CLProductInfo.TotalQuantity = iTotalSalesFloorQty + iTotalBackShopQty
#End If
            'To update unknown item count to 0 in next location
            'Also will update the unknown category
#If NRF Then
            If m_bIsNewList Then
                If m_CLProductInfo.IsUnknownItem Then
                    If m_iListCreatedLoc = Macros.COUNT_SALES_FLOOR Then
                        m_CLProductInfo.BackShopMBSQuantity = 0
                        m_CLProductInfo.BackShopQuantity = 0
                        m_CLProductInfo.IsMBSItemCounted = True
                        For Each objSiteInfo In m_CLProductInfo.BSMultiSiteList
                            objSiteInfo.IsCounted = "Y"
                        Next
                        m_CLProductInfo.TotalBSSiteCount = m_CLProductInfo.TotalBSSiteCount + 1
                    ElseIf m_iListCreatedLoc = Macros.COUNT_BACK_SHOP Then
                        m_CLProductInfo.SalesFloorQuantity = 0
                        m_CLProductInfo.IsSFItemCounted = True
                        For Each objSiteInfo In m_CLProductInfo.SFMultiSiteList
                            objSiteInfo.IsCounted = "Y"
                        Next
                    End If
                    'update unknown item into separate list
                    Dim bIsUnknownPresent As Boolean = False
                    If arr_UnItemList.Count() > 0 Then
                        For iCount = 0 To arr_UnItemList.Count - 1
                            Dim objCL As CLProductInfo = New CLProductInfo
                            objCL = arr_UnItemList.Item(iCount)
                            If objCL.ProductCode.Equals(m_CLProductInfo.ProductCode) Then
                                arr_UnItemList.RemoveAt(iCount)
                                arr_UnItemList.Insert(iCount, m_CLProductInfo)
                                bIsUnknownPresent = True
                                Exit For
                            End If
                        Next
                    End If
                    If Not bIsUnknownPresent Then
                        arr_UnItemList.Add(m_CLProductInfo)
                        m_CLCurrentProductGroup.UnknownItemCount = m_CLCurrentProductGroup.UnknownItemCount + 1
                    End If

                End If
            End If
#End If

            Dim bIsPresentInList As Boolean = False
            For iCount = 0 To m_CLItemList.Count - 1
                Dim objCL As CLProductInfo = New CLProductInfo
                objCL = m_CLItemList.Item(iCount)
                If objCL.BootsCode.Equals(m_CLProductInfo.BootsCode) Then
                    m_CLItemList.RemoveAt(iCount)
                    m_CLItemList.Insert(iCount, m_CLProductInfo)
                    bIsPresentInList = True
                    Exit For
                End If
            Next
            '%%%%%%%%%
#If RF Then
            'Checks if the product is already counted in sales floor
            'If not counted then add the item to the Counted Items list
            If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                Dim bIsProductAlreadyCounted As Boolean = False
                For Each objCountedProductData As CLProductCountedData In objAppContainer.m_CLSalesFloorCountedInfoList
                    If objCountedProductData.m_strListId.Equals(strListId_RF.TrimStart("0")) AndAlso _
                    objCountedProductData.m_strProductCode.Equals(m_CLProductInfo.ProductCode) Then
                        bIsProductAlreadyCounted = True
                        Exit For
                    End If
                Next
                If Not bIsProductAlreadyCounted Then
                    objAppContainer.objCLSummary.iSFListCounted += 1
                    m_iSalesFloorItemCount = m_iSalesFloorItemCount + 1
                    Dim objSalesFloorCountedProduct As CLProductCountedData = New CLProductCountedData()
                    objSalesFloorCountedProduct.m_strProductCode = m_CLProductInfo.ProductCode
                    objSalesFloorCountedProduct.m_strListId = strListId_RF.TrimStart("0")

                    objAppContainer.m_CLSalesFloorCountedInfoList.Add(objSalesFloorCountedProduct)
                End If
            ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                Dim bIsProductAlreadyCounted As Boolean = False
                For Each objCountedProductData As CLProductCountedData In objAppContainer.m_CLBackShopCountedInfoList
                    If objCountedProductData.m_strListId.Equals(strListId_RF.TrimStart("0")) AndAlso _
                    objCountedProductData.m_strProductCode.Equals(m_CLProductInfo.ProductCode) Then
                        bIsProductAlreadyCounted = True
                        Exit For
                    End If
                Next
                If Not bIsProductAlreadyCounted Then
                    objAppContainer.objCLSummary.iBSListCounted += 1
                    m_iBackShopItemCount = m_iBackShopItemCount + 1
                    Dim objBackShopCountedProduct As CLProductCountedData = New CLProductCountedData()
                    objBackShopCountedProduct.m_strProductCode = m_CLProductInfo.ProductCode
                    objBackShopCountedProduct.m_strListId = strListId_RF.TrimStart("0")

                    objAppContainer.m_CLBackShopCountedInfoList.Add(objBackShopCountedProduct)
                End If
            ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                Dim bIsProductAlreadyCounted As Boolean = False
                For Each objCountedProductData As CLProductCountedData In objAppContainer.m_CLOSSRCountedInfoList
                    If objCountedProductData.m_strListId.Equals(strListId_RF.TrimStart("0")) AndAlso _
                    objCountedProductData.m_strProductCode.Equals(m_CLProductInfo.ProductCode) Then
                        bIsProductAlreadyCounted = True
                        Exit For
                    End If
                Next

                If Not bIsProductAlreadyCounted Then
                    objAppContainer.objCLSummary.iOSSRListCounted += 1
                    m_iOSSRItemCount = m_iOSSRItemCount + 1
                    Dim objOSSRCountedProduct As CLProductCountedData = New CLProductCountedData()
                    objOSSRCountedProduct.m_strProductCode = m_CLProductInfo.ProductCode
                    objOSSRCountedProduct.m_strListId = strListId_RF.TrimStart("0")
                    objAppContainer.m_CLOSSRCountedInfoList.Add(objOSSRCountedProduct)
                End If
            End If
#End If
            '%%%%%%%%%

#If RF Then

            Dim objCLCRecord As CLCRecord = New CLCRecord()
            'Sets the values
            objCLCRecord.strListID = strListId_RF
            objCLCRecord.strNumberSEQ = m_CLProductInfo.SequenceNumber 'strSeq_RF - 1
            objCLCRecord.strBootscode = m_CLProductInfo.BootsCode
            If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                objCLCRecord.strCountLocation = Macros.SHOP_FLOOR
                objCLCRecord.strCount = m_CLProductInfo.SalesFloorQuantity
            ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                objCLCRecord.strCountLocation = Macros.BACK_SHOP
                If m_SelectedPOGSeqNum = Macros.BS_SELECTED_INDEX Then
                    objCLCRecord.strCountLocation = Macros.BACK_SHOP
                Else
                    objCLCRecord.strCountLocation = Macros.PSP
                End If
                objCLCRecord.strCount = iItemQty
            ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                If m_SelectedPOGSeqNum = Macros.BS_SELECTED_INDEX Then
                    objCLCRecord.strCountLocation = Macros.OSSR
                Else
                    objCLCRecord.strCountLocation = Macros.OSSR_PSP
                End If
                objCLCRecord.strCount = iItemQty
            End If
            objCLCRecord.strUpdateOSSR = " "
            If Not objAppContainer.objExportDataManager.CreateCLC(objCLCRecord) Then
                objAppContainer.objLogger.WriteAppLog("Could not UpdateCOLProductInfo of CLSessionMgr." _
                                             , Logger.LogLevel.RELEASE)
                Return False
            
            End If
#End If
            If Not bIsPresentInList Then
                m_CLItemList.Add(m_CLProductInfo)
				'SFA DEF#837 - Increment the seq only if the item is updated with count
#If RF Then
                strSeq_RF = strSeq_RF + 1
#End If
                If Not m_CLItemList.Count = m_iBackNextCount Then
                    m_iBackNextCount = m_CLItemList.Count - 1
                End If

            End If
            'If m_bIsSiteInfo Then
            If Not m_CLCurrentProductGroup.ListID = Nothing Then   '@@@@@@@M
                If Not m_bNavigation Then
                    If Not strPlannerDesc.Equals("Other") Then
                        If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                            objCurrentCountListInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                            objCurrentProductInfoList = objCurrentCountListInfoTable.Item(strPlannerDesc)
                        End If

                        If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                            'Update count status if all sites counted for an item in one planner
                            For Each objProductInfo In objCurrentProductInfoList
                                If objProductInfo.BootsCode.Equals(strBootsCode) Then
                                    objProductInfo.CurrentSiteCount = objProductInfo.CurrentSiteCount + 1
                                    If objProductInfo.CurrentSiteCount = objProductInfo.TotalSiteCount Then
                                        objProductInfo.CountStatus = "Y"
                                        iInfoIndex = iInfoIndex + 1
                                        Exit For
                                    End If
                                End If
                                iInfoIndex = iInfoIndex + 1
                            Next
                        Else
                            For Each objProductInfo In objCurrentProductInfoList
                                If objProductInfo.BootsCode.Equals(strBootsCode) Then
                                    objProductInfo.CountStatus = "Y"
                                    iInfoIndex = iInfoIndex + 1
                                    Exit For
                                End If
                                iInfoIndex = iInfoIndex + 1
                            Next
                        End If

                        objCurrentProductInfoList.RemoveAt(iInfoIndex)
                        objCurrentProductInfoList.Insert(iInfoIndex, objProductInfo)
                        objCurrentCountListInfoTable.Remove(strPlannerDesc)
                        objCurrentCountListInfoTable.Add(strPlannerDesc, objCurrentProductInfoList)
                        objAppContainer.objGlobalCLInfoTable.Remove(m_CLCurrentProductGroup.ListID)
                        objAppContainer.objGlobalCLInfoTable.Add(m_CLCurrentProductGroup.ListID, objCurrentCountListInfoTable)
                        'm_bIsSiteInfo = False
                    End If
                End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in UpdateCOLProductInfo of CLSessionMgr. Exception is:" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False

        End Try
        objAppContainer.objLogger.WriteAppLog("Exit UpdateCOLProductInfo of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Displays the View List screen for Create Own List
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks>Added as part of SFA</remarks>
    Private Sub DisplayCOLViewList(ByVal o As Object, ByVal e As EventArgs)
        Dim lstItem As ListViewItem
        Dim iCount As Integer
        Try
            objAppContainer.objLogger.WriteAppLog("Entered DisplayCLViewList of CLSessionMgr", Logger.LogLevel.INFO)
            With m_CLViewListScreen
                .lstvwItemDetails.Clear()
                .lblViewList.Visible = True
                .lblViewListSiteDisplay.Visible = False
                .lblViewListSite.Visible = False
                .lblDiscepancy.Visible = False
                .Info_button_i1.Visible = True
                .custCtrlBtnQuit.Visible = True
                .lblViewList.Text = "View Count List"
                .lblItemSelect.Visible = True
                .lblItemSelect.Text = "Select an item to Count or Quit"
                .lstvwItemDetails.Font = New System.Drawing.Font("Tahoma", 7.0!, FontStyle.Regular)

                If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                    .Text = "Create Count - SF"
                    .lblViewListSite.Visible = False
                    .lstvwItemDetails.Columns.Add("Item", 48 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("Description", 130 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("Multisites", 53 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    'Populate site info details into list view
                    For Each objProductInfo As CLProductInfo In m_CLItemList
                        'If Not objProductInfo.IsNotPlannerItem Then
                        lstItem = New ListViewItem()
                        iCount = 0
                        lstItem.Text = objProductInfo.BootsCode
                        lstItem.SubItems.Add(LCase(objProductInfo.Description))
                        For Each objPlannerInfo As CLMultiSiteInfo In objProductInfo.SFMultiSiteList
                            If objPlannerInfo.IsCounted.Equals("Y") Then
                                iCount = iCount + 1
                            End If
                        Next
                        If objProductInfo.SFMultiSiteList.Count() > 1 Then
                            If iCount < objProductInfo.SFMultiSiteList.Count() Then
                                lstItem.SubItems.Add((objProductInfo.SFMultiSiteList.Count() - 1).ToString() + "*")
                            Else
                                lstItem.SubItems.Add(objProductInfo.SFMultiSiteList.Count() - 1)
                            End If
                        Else
                            If iCount < objProductInfo.SFMultiSiteList.Count() Then
                                lstItem.SubItems.Add(objProductInfo.SFMultiSiteList.Count().ToString() + "*")
                            Else
                                lstItem.SubItems.Add(objProductInfo.SFMultiSiteList.Count())
                            End If
                        End If
                        .lstvwItemDetails.Items.Add(lstItem)
                        lstItem = Nothing
                        'End If
                    Next
                ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                    .Text = "Create Count - BS"
                    .lblViewListSite.Visible = True
                    .lblViewListSiteDisplay.Visible = True
                    .lblViewListSiteDisplay.Text = "Main Back Shop"
                    .lstvwItemDetails.Columns.Add("Item", 46 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("Description", 122 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("MBS", 32 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("PSP", 31 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    'Populate item info for backshop for NRF
                    For Each objProductInfo As CLProductInfo In m_CLItemList
                        'If Not objProductInfo.IsNotPlannerItem Then
                        lstItem = New ListViewItem()
                        lstItem.Text = objProductInfo.BootsCode
                        lstItem.SubItems.Add(objProductInfo.Description)
                        For Each objSiteInfo As CLMultiSiteInfo In objProductInfo.BSMultiSiteList
                            lstItem.SubItems.Add(objSiteInfo.IsCounted)
                        Next
                        If Not objProductInfo.PendingSalesFlag Then
                            lstItem.SubItems.Add("N/A")
                        End If
                        .lstvwItemDetails.Items.Add(lstItem)
                        lstItem = Nothing
                        'End If
                    Next
                ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                    .Text = "Create Count - OS"
                    .lblViewListSite.Visible = True
                    .lblViewListSiteDisplay.Visible = True
                    .lblViewListSiteDisplay.Text = "OSSR"
                    .lstvwItemDetails.Columns.Add("Item", 46 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("Description", 115 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("OSSR", 40 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("PSP", 30 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    'Populate item info for backshop for NRF
                    For Each objProductInfo As CLProductInfo In m_CLItemList
                        'If Not objProductInfo.IsNotPlannerItem Then
                        lstItem = New ListViewItem()
                        lstItem.Text = objProductInfo.BootsCode
                        lstItem.SubItems.Add(objProductInfo.Description)
                        For Each objSiteInfo As CLMultiSiteInfo In objProductInfo.OSSRMultiSiteList
                            lstItem.SubItems.Add(objSiteInfo.IsCounted)
                        Next
                        If Not objProductInfo.PendingSalesFlag Then
                            lstItem.SubItems.Add("N/A")
                        End If
                        .lstvwItemDetails.Items.Add(lstItem)
                        lstItem = Nothing
                        'End If
                    Next
                End If
                'Retrieves the store id and sets it to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayCLViewList of CLSessionMgr. Exception is:" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCLViewList of CLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' To do the processing while quiting the scan screen for Create Own List
    ''' </summary>
    ''' <remarks>Added as part of SFA</remarks>
    Public Function ProcessCOLItemScanQuit() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ProcessCOLQuit of CLSessionMgr", Logger.LogLevel.INFO)
        Dim iResult As Integer = 0
        Dim arrCheckMultiSiteList As ArrayList = New ArrayList
        Dim arrDiscrepancyList As ArrayList = New ArrayList
        Dim objMBSPlannerInfo As New CLMultiSiteInfo()
        Dim objPSPPlannerInfo As New CLMultiSiteInfo()
        Dim bCheck As Boolean = False
        Dim arrItemSummaryLsit As New ArrayList()
        Dim iRes As Integer = 0
        'If m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
        '    For Each objProductInfo As CLProductInfo In m_CLItemList
        '        If Not objProductInfo.BackShopQuantity < 0 Then
        '            arrCheckMultiSiteList = objProductInfo.BSMultiSiteList
        '            If Not arrCheckMultiSiteList Is Nothing Then
        '                If arrCheckMultiSiteList.Count > 1 Then
        '                    objMBSPlannerInfo = arrCheckMultiSiteList.Item(0)
        '                    objPSPPlannerInfo = arrCheckMultiSiteList.Item(1)
        '                    If objMBSPlannerInfo.IsCounted.Equals("Y") And _
        '                                objPSPPlannerInfo.IsCounted.Equals("N") Then
        '                        arrItemSummaryLsit.Add(objProductInfo)
        '                    End If
        '                End If
        '            End If
        '        End If
        '    Next
        'ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
        '    For Each objProductInfo As CLProductInfo In m_CLItemList
        '        If Not objProductInfo.OSSRQuantity < 0 Then
        '            arrCheckMultiSiteList = objProductInfo.OSSRMultiSiteList
        '            If Not arrCheckMultiSiteList Is Nothing Then
        '                If arrCheckMultiSiteList.Count > 1 Then
        '                    objMBSPlannerInfo = arrCheckMultiSiteList.Item(0)
        '                    objPSPPlannerInfo = arrCheckMultiSiteList.Item(1)
        '                    If objMBSPlannerInfo.IsCounted.Equals("Y") And _
        '                                objPSPPlannerInfo.IsCounted.Equals("N") Then
        '                        arrItemSummaryLsit.Add(objProductInfo)
        '                    End If
        '                End If
        '            End If
        '        End If
        '    Next
        'End If

        'If bCheck Then
        '    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M97"), "Confirmation", _
        '                          MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
        '                          MessageBoxDefaultButton.Button1)
        'Else
        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M104"), "Confirmation", _
                                  MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                                  MessageBoxDefaultButton.Button1)
        'End If
        If iResult = MsgBoxResult.Yes Then
            If m_CLItemList.Count = 0 Then
                DisplayCLScreen(CLSCREENS.COLLocationSelection)
            Else
                For Each objProductInfo As CLProductInfo In m_CLItemList
                    If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                        arrCheckMultiSiteList = objProductInfo.SFMultiSiteList
                        For Each objPlannerInfo As CLMultiSiteInfo In arrCheckMultiSiteList
                            If objPlannerInfo.IsCounted.Equals("N") Then
                                arrDiscrepancyList.Add(objProductInfo)
                                Exit For
                            End If
                        Next
                    ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                        If Not objProductInfo.BackShopQuantity < 0 Then
                            arrCheckMultiSiteList = objProductInfo.BSMultiSiteList
                            If Not arrCheckMultiSiteList Is Nothing Then
                                If arrCheckMultiSiteList.Count > 1 Then
                                    objMBSPlannerInfo = arrCheckMultiSiteList.Item(0)
                                    objPSPPlannerInfo = arrCheckMultiSiteList.Item(1)
                                    If objPSPPlannerInfo.IsCounted.Equals("Y") And _
                                                objMBSPlannerInfo.IsCounted.Equals("N") Then
                                        arrDiscrepancyList.Add(objProductInfo)
                                    End If
                                End If
                            End If
                        End If
                    ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                        If Not objProductInfo.OSSRQuantity < 0 Then
                            arrCheckMultiSiteList = objProductInfo.OSSRMultiSiteList
                            If Not arrCheckMultiSiteList Is Nothing Then
                                If arrCheckMultiSiteList.Count > 1 Then
                                    objMBSPlannerInfo = arrCheckMultiSiteList.Item(0)
                                    objPSPPlannerInfo = arrCheckMultiSiteList.Item(1)
                                    If objPSPPlannerInfo.IsCounted.Equals("Y") And _
                                                objMBSPlannerInfo.IsCounted.Equals("N") Then
                                        arrDiscrepancyList.Add(objProductInfo)
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next
                If Not arrDiscrepancyList.Count = 0 Then
                    If m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M98"), "Alert", MessageBoxButtons.OK, _
                                      MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                    ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M111"), "Alert", MessageBoxButtons.OK, _
                                      MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                    End If
                    DisplayCLScreen(CLSCREENS.ViewListScreen, m_iCountedLocation, 0, Nothing, arrDiscrepancyList, Macros.Count_LIST_DISCREPANCY)
                    'ElseIf arrItemSummaryLsit.Count <> 0 Then
                    '    'Display warning msg that item counted in mbs and not in psp
                    '    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M97"), "Confirmation", _
                    '                              MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                    '                              MessageBoxDefaultButton.Button1)
                    '    If iResult = MsgBoxResult.Yes Then
                    '        'display item summary screen
                    '        DisplayCLScreen(CLSCREENS.ViewListScreen, m_iCountedLocation, 0, Nothing, arrItemSummaryLsit, Macros.COUNT_LIST_BACKSHOPSUMMARY)
                    '    Else
                    '        'Display col item scan if selected no from warning msg
                    '        DisplayCLScreen(CLSCREENS.COLItemScan)
                    '        Return False
                    '    End If
                Else
                    DisplayCLScreen(CLSCREENS.COLLocationSelection)
                End If
            End If
        Else
            Return False
        End If
        objAppContainer.objLogger.WriteAppLog("Exit ProcessItemDetailsQuit of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    Private Sub CreateDataTable()
        objCountListDataTable = New DataTable
        Dim List_ID As DataColumn = New DataColumn("List_ID")
        Dim Boots_Code As DataColumn = New DataColumn("Boots_Code")
        Dim Planner_ID As DataColumn = New DataColumn("Planner_ID")
        Dim Repeat_Count As DataColumn = New DataColumn("Repeat_Count")
        Dim POG_Description As DataColumn = New DataColumn("POG_Description")
        Dim Planner_Desc As DataColumn = New DataColumn("Planner_Desc")
        List_ID.DataType = System.Type.GetType("System.String")
        Boots_Code.DataType = System.Type.GetType("System.String")
        Planner_ID.DataType = System.Type.GetType("System.String")
        Repeat_Count.DataType = System.Type.GetType("System.String")
        POG_Description.DataType = System.Type.GetType("System.String")
        Planner_Desc.DataType = System.Type.GetType("System.String")
        objCountListDataTable.Columns.Add(List_ID)
        objCountListDataTable.Columns.Add(Boots_Code)
        objCountListDataTable.Columns.Add(Planner_Desc)
        objCountListDataTable.Columns.Add(Repeat_Count)
        objCountListDataTable.Columns.Add(POG_Description)
        objCountListDataTable.Columns.Add(Planner_ID)
    End Sub
    ''' <summary>
    ''' Processes the back button click on the Product Count screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessCOLBack() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ProcessProductCountBack of CLSessionMgr", Logger.LogLevel.INFO)
        m_bIsBackNext = True
        m_iBackNextCount = m_iBackNextCount - 1

        If ValidateCOLNextAndback() Then
            m_EntryType = BCType.None
            If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.SELECTED_INDEX)
            Else
                DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.BS_SELECTED_INDEX)
            End If
        Else
            Return False
        End If
        objAppContainer.objLogger.WriteAppLog("Exit ProcessProductCountBack of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' validates the selection of Next and Back Buttons
    ''' </summary>
    ''' <param name="iSender"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ValidateCOLNextAndback(Optional ByVal iSender As Integer = 0) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ValidateNextAndback of CLSessionMgr", Logger.LogLevel.INFO)
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim objCurrentCountListInfoTable As Hashtable = New Hashtable()
        Dim bIsValidChoice As Boolean = True
        Try
            'Gets the current product info from hash table based on the list id 
            'and the position of the item in the list as indicated by 
            'm_iProductListCount variable

            objCurrentProductInfoList = m_CLItemList

            'Validates the next and back buttons
            If m_iBackNextCount < 0 Then
                m_iBackNextCount = m_iBackNextCount + 1
                'If the sender is from next button click of process then isender val is 2
                If Not iSender = Macros.SENDER_PROCESS_ACTION Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M5"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                    'MessageBox.Show("This is the first item")
                End If
                bIsValidChoice = False
            End If
            If (m_iBackNextCount > objCurrentProductInfoList.Count - 1) Then
                m_iBackNextCount = m_iBackNextCount - 1
                'If the sender is from next button click of process then isender val is 2
                If Not iSender = Macros.SENDER_PROCESS_ACTION Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M6"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                    'MessageBox.Show("This is the last item")
                End If
                bIsValidChoice = False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured while processing ValidateNextAndback of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ValidateNextAndback of CLSessionMgr", Logger.LogLevel.INFO)
        Return bIsValidChoice
    End Function
    Public Function ProcessCOLItemDetailsNext() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ProcessItemDetailsNext of CLSessionMgr", Logger.LogLevel.INFO)
        'Added as part of SFA - On click of Next display the Item scan screen for COL 
        Dim iCount As Integer = 0
        m_bIsBackNext = True
        m_iBackNextCount = m_iBackNextCount + 1
        'If m_bPlannerFlag Then
        '    iCount = m_CLItemList.Count - 1
        'Else
        iCount = m_CLItemList.Count
        'End If
        If iCount <= m_iBackNextCount Then
            If m_bNavigation Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M6"), "Info", _
                                          MessageBoxButtons.OK, _
                                          MessageBoxIcon.Asterisk, _
                                          MessageBoxDefaultButton.Button1)
                objAppContainer.objLogger.WriteAppLog(MessageManager.GetInstance().GetMessage("M6"), Logger.LogLevel.INFO)
            Else
                DisplayCLScreen(CLSCREENS.COLItemScan)
            End If
            m_iBackNextCount = m_iBackNextCount - 1
        Else
            m_EntryType = BCType.None
            If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.SELECTED_INDEX)
            Else
                DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.BS_SELECTED_INDEX)
            End If
            m_strSEL = ""
        End If
        objAppContainer.objLogger.WriteAppLog("Exit ProcessItemDetailsNext of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    Public Function ProcessCOLProductCountNext(ByVal strPrdCode As String, _
                    ByVal strBootsCode As String, ByVal iShelfQuantity As Integer) As Boolean

        objAppContainer.objLogger.WriteAppLog("Entered ProcessProductCountNext of CLSessionMgr", Logger.LogLevel.INFO)

        Dim strUnFormatBootsCode As String
        Dim iCurrentQuantity As String = Nothing
        Dim objCurrentProductInfo As New CLProductInfo()
        Dim objCurrentBSSiteInfoList As New ArrayList()
        Dim objCurrentOSSRSiteInfoList As New ArrayList()
        Dim objCurrentSFSiteInfoList As New ArrayList()
        Dim objSiteInfo As New CLMultiSiteInfo()

        'To unformat the product code by removing "-" and then remove CDV from that value
        Dim strProductCode As String = ""
        strProductCode = objAppContainer.objHelper.UnFormatBarcode(strPrdCode)
        strProductCode = strProductCode.Remove(strProductCode.Length - 1, 1)
        strUnFormatBootsCode = objAppContainer.objHelper.UnFormatBarcode(strBootsCode)
        '$$$$$$$$$$$$
        'If m_CLItemList.Count = 0 Then
        '    objCurrentProductInfo = m_CLProductInfo
        'Else
        '    objCurrentProductInfo = m_CLItemList.Item(m_iBackNextCount)
        'End If
        objCurrentProductInfo = m_CLProductInfo

        'If objCurrentProductInfo.BootsCode.Equals(strUnFormatBootsCode) Then
        objCurrentSFSiteInfoList = objCurrentProductInfo.SFMultiSiteList
#If NRF Then
                        objCurrentBSSiteInfoList = objCurrentProductInfo.BSMultiSiteList
#ElseIf RF Then
        If objAppContainer.OSSRStoreFlag = "Y" Then
            If objCurrentProductInfo.OSSRFlag = "O" Then
                objCurrentOSSRSiteInfoList = objCurrentProductInfo.OSSRMultiSiteList
            Else
                objCurrentBSSiteInfoList = objCurrentProductInfo.BSMultiSiteList
            End If
        Else
            objCurrentBSSiteInfoList = objCurrentProductInfo.BSMultiSiteList
        End If
#End If

        'End If

        '$$$$$$$$$$$$
        If m_EntryType = BCType.EAN Then
            If iShelfQuantity = 0 Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M99"), "Info", _
                                               MessageBoxButtons.OK, _
                                               MessageBoxIcon.Asterisk, _
                                               MessageBoxDefaultButton.Button1)
                objAppContainer.objLogger.WriteAppLog(MessageManager.GetInstance().GetMessage("M99"), Logger.LogLevel.INFO)
            Else
                ProcessCOLItemDetailsNext()
            End If

        Else
            If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                'If item key entered and quantity not entered, a count of zero assumed
                'Move to next item if present
                If iShelfQuantity = 0 Then

                    For Each objSiteInfo In objCurrentSFSiteInfoList
                        If objSiteInfo.strSeqNumber.Equals(m_SelectedPOGSeqNum) Then
                            iCurrentQuantity = objSiteInfo.strSalesFloorQuantiy
                            Exit For
                        End If
                    Next
                    If iCurrentQuantity.Equals("-1") Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M102"), "Info", _
                                                                  MessageBoxButtons.OK, _
                                                                  MessageBoxIcon.Asterisk, _
                                                                  MessageBoxDefaultButton.Button1)
                        objAppContainer.objLogger.WriteAppLog(MessageManager.GetInstance().GetMessage("M102"), Logger.LogLevel.INFO)
                    End If
                    'Updates the list with modified data
                    If UpdateCOLProductInfo(strProductCode, iShelfQuantity, 0, 0, strUnFormatBootsCode) Then
                        ProcessCOLItemDetailsNext()
                    End If
                Else
                    'If a valid quantity is enterd, move to next item on pressing next button
                    ProcessCOLItemDetailsNext()
                End If
            ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                If iShelfQuantity = 0 Then
                    For Each objSiteInfo In objCurrentBSSiteInfoList
                        If objSiteInfo.strSeqNumber.Equals(m_SelectedPOGSeqNum) Then
                            iCurrentQuantity = objSiteInfo.strBackShopQuantiy
                            Exit For
                        End If
                    Next
                    If iCurrentQuantity.Equals("-1") Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M102"), "Info", _
                                                                  MessageBoxButtons.OK, _
                                                                  MessageBoxIcon.Asterisk, _
                                                                  MessageBoxDefaultButton.Button1)
                        objAppContainer.objLogger.WriteAppLog(MessageManager.GetInstance().GetMessage("M102"), Logger.LogLevel.INFO)
                    End If

                    'Updates the list with modified data
                    If UpdateCOLProductInfo(strProductCode, 0, iShelfQuantity, 0, strUnFormatBootsCode) Then
                        ProcessCOLItemDetailsNext()
                    End If
                Else
                    'If a valid quantity is enterd, move to next item on pressing next button
                    ProcessCOLItemDetailsNext()
                End If

                'ambli
                'For OSSR
            ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
#If RF Then
                If iShelfQuantity = 0 Then
                    For Each objSiteInfo In objCurrentOSSRSiteInfoList
                        If objSiteInfo.strSeqNumber.Equals(m_SelectedPOGSeqNum) Then
                            iCurrentQuantity = objSiteInfo.strOSSRQuantiy
                            Exit For
                        End If
                    Next
                    If iCurrentQuantity.Equals("-1") Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M102"), "Info", _
                                                                  MessageBoxButtons.OK, _
                                                                  MessageBoxIcon.Asterisk, _
                                                                  MessageBoxDefaultButton.Button1)
                        objAppContainer.objLogger.WriteAppLog(MessageManager.GetInstance().GetMessage("M102"), Logger.LogLevel.INFO)
                    End If

                    'Updates the list with modified data
                    If UpdateCOLProductInfo(strProductCode, 0, 0, iShelfQuantity, strUnFormatBootsCode) Then
                        ProcessCOLItemDetailsNext()
                    End If
                Else
                    'If a valid quantity is enterd, move to next item on pressing next button
                    ProcessCOLItemDetailsNext()
                End If

#End If
            End If
        End If

        objAppContainer.objLogger.WriteAppLog("Entered ProcessProductCountNext of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
#End Region
    ''' <summary>
    ''' Displays the Count List Location Selection Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayCLLocationSelection(ByVal o As Object, ByVal e As EventArgs)
        Try
            objAppContainer.objLogger.WriteAppLog("Entered DisplayCLLocationSelection of CLSessionMgr", Logger.LogLevel.INFO)

            Dim arrSFList As New ArrayList()
            Dim arrBSList As New ArrayList()
            Dim arrBootsCodeList As ArrayList
            Dim arrPlannerList As ArrayList = New ArrayList()
            Dim objCLProductInfoTable As Hashtable = New Hashtable()

            Dim iItemBackShopCount As Integer = 0
            Dim iItemSalesFloorCount As Integer = 0
            Dim iTotalOSSRItemCount As Integer = 0
            'Stock File Accuracy  - Declared new variable to hold count list type
            Dim strListType As String = ""
            'For OSSR 
            Dim iItemOSSRCount As Integer = 0
            Dim objProductInfo As CLProductInfo = New CLProductInfo()
            Dim objCurrentProductInfoList As ArrayList = New ArrayList()

            If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                objCurrentProductInfoList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
            End If

            For Each objCurrentProductInfo As CLProductInfo In objCurrentProductInfoList
                If objCurrentProductInfo.SalesFloorQuantity >= 0 Then
                    iItemSalesFloorCount = iItemSalesFloorCount + 1
                End If
#If NRF Then
                If objCurrentProductInfo.BackShopQuantity >= 0 Then
                    iItemBackShopCount = iItemBackShopCount + 1
                End If
#End If
                

#If RF Then
                If objAppContainer.OSSRStoreFlag = "Y" Then
                    m_CLLocSelection.btn_OSSR.Visible = True
                    m_CLLocSelection.btn_OSSR.Enabled = True
                    m_CLLocSelection.lblNumOSSRItems.Visible = True
                    If objCurrentProductInfo.OSSRFlag = "O" Then
                        iTotalOSSRItemCount = iTotalOSSRItemCount + 1
                        If objCurrentProductInfo.OSSRQuantity >= 0 Then
                            iItemOSSRCount = iItemOSSRCount + 1
                        End If
                    Else
                        If objCurrentProductInfo.BackShopQuantity >= 0 Then
                            iItemBackShopCount = iItemBackShopCount + 1
                        End If
                    End If
                Else
                    If objCurrentProductInfo.BackShopQuantity >= 0 Then
                        iItemBackShopCount = iItemBackShopCount + 1
                    End If
                    m_CLLocSelection.btn_OSSR.Visible = False
                    m_CLLocSelection.btn_OSSR.Enabled = False
                    m_CLLocSelection.lblNumOSSRItems.Visible = False
                End If
#End If
            Next
#If NRF Then
                    m_CLLocSelection.btn_OSSR.Visible = False
                    m_CLLocSelection.btn_OSSR.Enabled = False
                    m_CLLocSelection.lblNumOSSRItems.Visible = False
#End If

            'Populates data in the location selection screen
            With m_CLLocSelection
                .Text = "Count List"
                .lblProductName.Text = m_CLCurrentProductGroup.ListDescription
                strListType = m_CLCurrentProductGroup.ListType
                'Stock File Accuracy  - Expand count list type and display 
                If strListType.Equals("H") Then
                    .lblStatus.Text = "Support Office"
                ElseIf strListType.Equals("R") Then
                    .lblStatus.Text = "Recount"
                ElseIf strListType.Equals("U") Then
                    .lblStatus.Text = "User generated count list"
                ElseIf strListType.Equals("N") Then
                    .lblStatus.Text = "Negative Count List"
                End If


                .lblNumSalesFloorItems.Text = iItemSalesFloorCount.ToString() + "/" + (m_CLCurrentProductGroup.CurrentItemCount).ToString() + " Items"
                .lblNumSalesFloorItems.Visible = True
                m_CLCurrentProductGroup.SFItemCount = m_CLCurrentProductGroup.CurrentItemCount
                m_CLCurrentProductGroup.SFCountedItems = iItemSalesFloorCount
#If NRF Then
                .lblNumBackShopItems.Text = iItemBackShopCount.ToString() + "/" + (m_CLCurrentProductGroup.CurrentItemCount).ToString() + " Items"
                .lblNumBackShopItems.Visible =True 
                m_CLCurrentProductGroup.BSItemCount = m_CLCurrentProductGroup.CurrentItemCount
                m_CLCurrentProductGroup.BSCountedItems = iItemBackShopCount
                If m_CLCurrentProductGroup.UnknownItemCount > 0 Then
                    m_CLCurrentProductGroup.BSItemCount = m_CLCurrentProductGroup.BSItemCount - m_CLCurrentProductGroup.UnknownItemCount
                End If

#End If
                'For OSSR
#If RF Then
                .lblNumBackShopItems.Text = iItemBackShopCount.ToString() + "/" + (m_CLCurrentProductGroup.CurrentItemCount - iTotalOSSRItemCount).ToString() + " Items"
                .lblNumOSSRItems.Text = iItemOSSRCount.ToString() + "/" + iTotalOSSRItemCount.ToString() + " Items"
                .lblNumBackShopItems.Visible = True
                '.lblNumOSSRItems.Visible = True
                m_CLCurrentProductGroup.BSItemCount = m_CLCurrentProductGroup.CurrentItemCount - iTotalOSSRItemCount
                m_CLCurrentProductGroup.OSSRItemCount = iTotalOSSRItemCount
                m_CLCurrentProductGroup.BSCountedItems = iItemBackShopCount
                m_CLCurrentProductGroup.OSSRCountedItems = iItemOSSRCount
#End If
                'Stock File Accuracy  -  retrieve site info list
                'Check whether the site detials present global structure.
                'If the list id is present ,but planner not present, then iterate through the planners
                'availabe in list and get the item detaiis in each site.

                'Populate global structures initially.
#If NRF Then
                If Not objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    If Not objAppContainer.objGlobalCLSiteInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                        If objAppContainer.objDataEngine.GetPlannerListDetails(m_CLCurrentProductGroup.ListID, arrPlannerList) Then
                             'Not on planner
                             If m_CLCurrentProductGroup.NotOnPlannerItemCount > 0 Then
                                Dim objSiteInfo As New CLMultiSiteInfo()
                                objSiteInfo.strPlannerDesc = "Not On Planner"
                                'objSiteInfo.strSeqNumber = "1"
                                objSiteInfo.iItemCount = m_CLCurrentProductGroup.NotOnPlannerItemCount
                                arrPlannerList.Add(objSiteInfo)
                            End If
                            If m_CLCurrentProductGroup.UnknownItemCount > 0 Then
                                Dim objUnSiteInfo As New CLMultiSiteInfo()
                                objUnSiteInfo.strPlannerDesc = "Unknown"
                                'objSiteInfo.strSeqNumber = "1"
                                objUnSiteInfo.iItemCount = m_CLCurrentProductGroup.UnknownItemCount
                                arrPlannerList.Add(objUnSiteInfo)
                            End If
                            objAppContainer.objGlobalCLSiteInfoTable.Add(m_CLCurrentProductGroup.ListID, arrPlannerList)
                        Else
                            'Not on planner
                            If m_CLCurrentProductGroup.NotOnPlannerItemCount > 0 Then
                                Dim objSiteInfo As New CLMultiSiteInfo()
                                objSiteInfo.strPlannerDesc = "Not On Planner"
                                'objSiteInfo.strSeqNumber = "1"
                                objSiteInfo.iItemCount = m_CLCurrentProductGroup.NotOnPlannerItemCount
                                arrPlannerList.Add(objSiteInfo)
                            End If
                            If m_CLCurrentProductGroup.UnknownItemCount > 0 Then
                                Dim objUnSiteInfo As New CLMultiSiteInfo()
                                objUnSiteInfo.strPlannerDesc = "Unknown"
                                'objSiteInfo.strSeqNumber = "1"
                                objUnSiteInfo.iItemCount = m_CLCurrentProductGroup.UnknownItemCount
                                arrPlannerList.Add(objUnSiteInfo)
                            End If
                            objAppContainer.objGlobalCLSiteInfoTable.Add(m_CLCurrentProductGroup.ListID, arrPlannerList)
                        End If
                    End If

                    If m_CLCurrentProductGroup.UnknownItemCount > 0 Then
                        arrSFList = m_CLCurrentProductGroup.UnknownItemList
                        arrBSList = m_CLCurrentProductGroup.UnknownItemList
                    End If
                    For Each objPlannerInfo As CLMultiSiteInfo In objAppContainer.objGlobalCLSiteInfoTable.Item(m_CLCurrentProductGroup.ListID)
                        arrBootsCodeList = New ArrayList()
                        Dim arrItemList As New ArrayList()
                        If objPlannerInfo.strPlannerDesc = "Not On Planner" Then
                            arrItemList = m_CLCurrentProductGroup.NotOnPlannerItemList
                            For Each objItemInfo As CLProductInfo In arrItemList
                                If objItemInfo.SalesFloorQuantity < 0 Then
                                    objItemInfo.CountStatus = "N"
                                Else
                                    objItemInfo.CountStatus = "Y"
                                End If
                                objItemInfo.TotalSiteCount = 1
                            Next
                            arrBootsCodeList = arrItemList
                        ElseIf objPlannerInfo.strPlannerDesc = "Unknown" Then
                            arrItemList = arrSFList
                            For Each objItemInfo As CLProductInfo In arrItemList
                                If objItemInfo.SalesFloorQuantity < 0 Then
                                    objItemInfo.CountStatus = "N"
                                Else
                                    objItemInfo.CountStatus = "Y"
                                End If
                                objItemInfo.TotalSiteCount = 1
                            Next
                            arrBootsCodeList = arrItemList
                        Else
                            'Retrieve the items in each planner
                            GetBootsCodeList(Macros.COUNT_SF, objPlannerInfo.strPlannerDesc, m_CLCurrentProductGroup.ListID, arrBootsCodeList)

                            'Retrieves the site count for each item in a planner
                            objAppContainer.objDataEngine.GetItemSiteCount(objPlannerInfo.strPlannerDesc, m_CLCurrentProductGroup.ListID, arrBootsCodeList)
                        End If
                        objCLProductInfoTable.Add(objPlannerInfo.strPlannerDesc.Trim(), arrBootsCodeList)
                    Next

                    Dim arrMBSItemList As New ArrayList()
                    arrBootsCodeList = New ArrayList()
                    GetBootsCodeList(Macros.COUNT_MBS, Macros.COUNT_MBS, m_CLCurrentProductGroup.ListID, arrBootsCodeList)
                    objCLProductInfoTable.Add(Macros.COUNT_MBS, arrBootsCodeList)

                    'Dim ListCount As Integer = 0
                    'ListCount = arrBootsCodeList.Count()

                    'Populate unknown items into Unknown
                    arrBootsCodeList = New ArrayList()
                    Dim arrUItemList As New ArrayList()
                    If m_CLCurrentProductGroup.UnknownItemCount > 0 Then
                        arrUItemList = arrBSList
                        For Each objItemInfo As CLProductInfo In arrUItemList
                            If objItemInfo.BackShopQuantity < 0 Then
                                objItemInfo.CountStatus = "N"
                            Else
                                objItemInfo.CountStatus = "Y"
                            End If
                            objItemInfo.TotalSiteCount = 1
                            arrBootsCodeList.Add(objItemInfo)
                        Next
                    End If
                    objCLProductInfoTable.Add(Macros.COUNT_BS_UNKNOWN, arrBootsCodeList)

                    If Not m_CLCurrentProductGroup.BackShopPSPCount = 0 Then
                        arrBootsCodeList = New ArrayList()
                        GetBootsCodeList(Macros.COUNT_PSP, Macros.COUNT_PSP, m_CLCurrentProductGroup.ListID, arrBootsCodeList)
                        objCLProductInfoTable.Add(Macros.COUNT_PSP, arrBootsCodeList)
                    End If
                    objAppContainer.objGlobalCLInfoTable.Add(m_CLCurrentProductGroup.ListID, objCLProductInfoTable)
                End If

#End If

                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception ocurred in DisplayCLLocationSelection of CLSessionMgr. Exception is:" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCLLocationSelection of CLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Displays the Count List Sales Floor Product Counting Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayCLSalesFloorProductCount(ByVal o As Object, ByVal e As EventArgs)
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim objCurrentCountListInfoTable As Hashtable = New Hashtable()
        Dim arrMultiSiteList As New ArrayList()
        Dim objMultiSite As New CLMultiSiteInfo()
        Dim objSiteInfo As New CLMultiSiteInfo()
        Dim arrProductList As New ArrayList()

        Dim strBootsCode As String
        Dim objCurrentProductInfo As New CLProductInfo()
        Dim iCount As Integer = 0

        Dim strLastActBuildTime As String = Nothing
        Dim strActiveDataTime As DateTime = Nothing
        Dim strtempActiveDataTime As String = Nothing


        objAppContainer.objLogger.WriteAppLog("Entered DisplayCLSalesFloorProductCount of CLSessionMgr", Logger.LogLevel.INFO)
        Try
            'Gets the current product info from hash table based on the list id 
            'and the position of the item in the list as indicated by 
            'm_iProductListCount variable
            objCurrentProductInfo = New CLProductInfo()
            m_bIsFullPriceCheckRequired = False
            If m_bIsCreateOwnList Then
                If Not m_bIsNewList Then
                    If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                        objCurrentCountListInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                        objCurrentProductInfoList = objCurrentCountListInfoTable.Item(m_CLCurrentSiteInfo.strPlannerDesc)
                        objCurrentProductInfo = m_CLProductInfo
                    End If
                Else
                    If m_bNavigation Then
                        objCurrentProductInfoList = m_CLItemList
                        objCurrentProductInfo = m_CLItemList(m_iBackNextCount)
                    Else
                        objCurrentProductInfo = m_CLProductInfo
                        m_CLProductInfo.CreatedLocation = Macros.SHOP_FLOOR
                    End If
                End If
                m_ItemScreen = Nothing
                bIsItemScan = False
                arrMultiSiteList = objCurrentProductInfo.SFMultiSiteList
                If m_SelectedPOGSeqNum Is Nothing Then
                    m_SelectedPOGSeqNum = Macros.SELECTED_INDEX
                End If
            Else

                If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    objCurrentCountListInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                    objCurrentProductInfoList = objCurrentCountListInfoTable.Item(m_CLCurrentSiteInfo.strPlannerDesc)
                End If

                Dim objProductInfo As New CLProductInfo()
                objProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)
                strBootsCode = objProductInfo.BootsCode

                If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    arrProductList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                    For Each objCurrentProductInfo In arrProductList
                        If objCurrentProductInfo.BootsCode.Equals(strBootsCode) Then
                            objCurrentProductInfo = m_CLCurrentItemInfo
                            Exit For
                        End If
                    Next
                End If
            End If

            'Sets the values
            Dim objDescriptionArray As ArrayList = New ArrayList()
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(objCurrentProductInfo.Description)

            'Obtains the 13 digit product code
            Dim strProductCode As String = ""
            strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(objCurrentProductInfo.ProductCode)


            With m_CLSalesFloorProductCount
                If m_bIsCreateOwnList Then
                    .Text = "Create Count - SF"
                    If m_CLItemList.Count = 0 Then
                        .custCtrlBtnBack.Visible = False
                    Else
                        .custCtrlBtnBack.Visible = True
                    End If
                    If Not m_bIsNewList Then
                        .lblItemPosition.Text = (m_iProductListCount + 1).ToString() + "/" + objCurrentProductInfoList.Count.ToString()
                        .lblItemPosition.Visible = True
                    Else
                        'Disable controls for Create Own List
                        If m_bNavigation Then
                            .lblItemPosition.Text = (m_iBackNextCount + 1).ToString() + "/" + objCurrentProductInfoList.Count.ToString()
                            .lblItemPosition.Visible = True
                        Else
                            .lblItemPosition.Visible = False
                        End If
                    End If
                Else
                    .Text = "Count List - SF"
                    .lblItemPosition.Text = (m_iProductListCount + 1).ToString() + "/" + (objCurrentProductInfoList.Count).ToString()
                End If

                'If bIsItemCounted Then
                '    .lblCounted.Visible = True
                '    .lblCounted.Text = "COUNTED"
                'Else
                '    .lblCounted.Visible = False
                '    .lblCounted.Text = ""
                'End If
                .lblBootsCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(objCurrentProductInfo.BootsCode)
                .lblProductCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(strProductCode)
                .lblProductDesc1.Text = objDescriptionArray.Item(0)
                .lblProductDesc2.Text = objDescriptionArray.Item(1)
                .lblProductDesc3.Text = objDescriptionArray.Item(2)
                If objCurrentProductInfo.IsUnknownItem Then
                    .lblStatusDisplay.Text = "N/A"
                    If m_bIsCreateOwnList Then
                        If Not objCurrentProductInfo.IsSEL Then
                            .lblBootsCodeDisplay.Text = "00-00-000"
                        End If
                    End If
                Else
                    .lblStatusDisplay.Text = objAppContainer.objHelper.GetStatusDescription(objCurrentProductInfo.Status)
                End If
                If objCurrentProductInfo.TSF.Substring(0, 1).Equals("-") Then
                    .lblTotalStockFileDisplay.ForeColor = Color.Red
                Else
                    .lblTotalStockFileDisplay.ForeColor = .lblStatusDisplay.ForeColor
                End If
                .lblTotalStockFileDisplay.Text = objCurrentProductInfo.TSF

#If NRF Then
                If m_bIsCreateOwnList Then
                    .lblTotalStockFile.Text = "Start of Day Stock File:"
                Else
                    strLastActBuildTime = ConfigDataMgr.GetInstance.GetParam(ConfigKey.LAST_ACTBUILD_TIME)
                    strActiveDataTime = DateTime.ParseExact(strLastActBuildTime, "yyyy-MM-dd HH:mm:ss", Nothing)
                    strtempActiveDataTime = strActiveDataTime.Hour.ToString().PadLeft(2, "0") + ":" + _
                                        strActiveDataTime.Minute.ToString().PadLeft(2, "0")
                    .lblTotalStockFile.Text = ""
                    .lblTotalStockFile.Text = "Total Stock File at " + strtempActiveDataTime + " "
                End If
#ElseIf RF Then
                .lblTotalStockFile.Text = ""
                .lblTotalStockFile.Text = "Total Stock File"
#End If

                    'If objCurrentProductInfo.BackShopQuantity < 0 Then
                    '    .lblBackShopVal.Text = 0
                    'Else
                    '    .lblBackShopVal.Text = objCurrentProductInfo.BackShopQuantity
                    'End If
                    'If objCurrentProductInfo.SalesFloorQuantity < 0 Then
                    '    .lblShelfQty.Text = 0
                    'Else
                    '    .lblShelfQty.Text = objCurrentProductInfo.SalesFloorQuantity
                    'End If

                    'Gets the sales floor quanitity to be displayed
                    arrMultiSiteList = objCurrentProductInfo.SFMultiSiteList

                    'Enable counted label if item counted in all sites
                    .lblCounted.Visible = False
                    If m_bIsCreateOwnList Then
                        If objCurrentProductInfo.IsUnknownItem Then
                            .lblStatusDisplay.Text = "N/A"
                            m_SelectedPOGSeqNum = 0
                        End If
                    End If
                    For Each objPlannerInfo As CLMultiSiteInfo In arrMultiSiteList
                        If objPlannerInfo.IsCounted.Equals("N") Then
                            iCount = iCount + 1
                            Exit For
                        End If
                    Next
                    '#If NRF Then 'Remove the check
                    If iCount = 0 Then
                        .lblCounted.Visible = True
                    End If
                    '#End If
                    If arrMultiSiteList.Count <= 1 Then
                        If objCurrentProductInfo.SalesFloorQuantity < 0 Then
                            .lblShelfQty.Text = 0
                        Else
                            .lblShelfQty.Text = objCurrentProductInfo.SalesFloorQuantity
                        End If
                    Else
                        For Each objSiteInfo In arrMultiSiteList
                            If objSiteInfo.strSeqNumber.Equals(m_SelectedPOGSeqNum) Then
                                If objSiteInfo.strSalesFloorQuantiy < 0 Then
                                    .lblShelfQty.Text = 0
                                Else
                                    .lblShelfQty.Text = objSiteInfo.strSalesFloorQuantiy
                                End If
                                Exit For
                            End If
                        Next
                    End If

                    ' populating multi sites
                    If arrMultiSiteList.Count <= 1 Then
                        .lblSite.Visible = False
                        .cmbMultiSite.Visible = False
                    Else
                        .lblSite.Visible = True
                        .cmbMultiSite.Visible = True
                        .cmbMultiSite.Items.Clear()
                        '.cmbMultiSite.Items.Add("Select")
                        For Each objMultiSite In arrMultiSiteList
                            .cmbMultiSite.Items.Add(objMultiSite.strPOGDescription)
                        Next
                        .cmbMultiSite.SelectedIndex = m_SelectedPOGSeqNum

                    End If
                    'For OSSR
                    '#If RF Then
                    'If objAppContainer.OSSRStoreFlag = "Y" Then
                    '    .btn_OSSRItem.Visible = True
                    '    .lblOSSR.Visible = True
                    '    If objCurrentProductInfo.OSSRStatus = Macros.OSSR Then
                    '        .lblOSSR.Text = "OSSR"
                    '    Else
                    '        .lblOSSR.Text = ""
                    '    End If
                    '    If objCurrentProductInfo.OSSRQuantity < 0 Then
                    '        .lblOSSRVal.Text = 0
                    '    Else
                    '        .lblOSSRVal.Text = objCurrentProductInfo.OSSRQuantity
                    '    End If
                    'Else
                    '    .btn_OSSRItem.Visible = False
                    '    .lblOSSR.Visible = False
                    '    .lblOffsite.Visible = False
                    '    .lblOSSRVal.Visible = False
                    'End If
                    '#ElseIf NRF Then
                    '.btn_OSSRItem.Visible = False
                    '.lblOSSR.Visible = False
                    '.lblOffsite.Visible = False
                    '.lblOSSRVal.Visible = False
                    '#End If
                    'Fix for allignment for Back and OSSR button
                    '#If RF Then

                    'If objAppContainer.OSSRStoreFlag = "Y" Then
                    '    .custCtrlBtnBack.Location = New Point(59 * objAppContainer.iOffSet, 241 * objAppContainer.iOffSet)
                    '    .btn_OSSRItem.Location = New Point(113 * objAppContainer.iOffSet, 241 * objAppContainer.iOffSet)
                    'Else
                    '    .custCtrlBtnBack.Location = New Point(93 * objAppContainer.iOffSet, 241 * objAppContainer.iOffSet)
                    '    .btn_OSSRItem.Visible = False
                    'End If
                    '#ElseIf NRF Then
                    '                .custCtrlBtnBack.Location = New Point(93 * objAppContainer.iOffSet, 241 * objAppContainer.iOffSet)
                    '                .btn_OSSRItem.Visible = False

                    '#End If
                    'Displays the total quantity

                    If objCurrentProductInfo.TotalQuantity < 0 Then
                        .lblTotalItemCountDisplay.Text = 0
                    Else
                        .lblTotalItemCountDisplay.Text = objCurrentProductInfo.TotalQuantity
                    End If

                    'Sets the store id and active data time to the status bar
                    .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                    .Visible = True
                    .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayCLSalesFloorProductCount of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCLSalesFloorProductCount of CLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Displays the Count List Back Shop Product Counting Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayCLBackShopProductCount(ByVal o As Object, ByVal e As EventArgs)
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim objCurrentCountListInfoTable As Hashtable = New Hashtable()
        Dim arrProductList As New ArrayList()
        Dim arrMultiSiteList As New ArrayList()
        Dim objMultiSite As New CLMultiSiteInfo()
        Dim objSiteInfo As New CLMultiSiteInfo()
        Dim objCurrentProductInfo As New CLProductInfo()
        Dim strBootsCode As String

        Dim strLastActBuildTime As String = Nothing
        Dim strActiveDataTime As DateTime = Nothing
        Dim strtempActiveDataTime As String = Nothing

        objAppContainer.objLogger.WriteAppLog("Entered DisplayCLBackShopProductCount of CLSessionMgr", Logger.LogLevel.INFO)
        Try

            'Gets the current product info from hash table based on the list id 
            'and the position of the item in the list as indicated by 
            'm_iProductListCount variable
            objCurrentProductInfo = New CLProductInfo()
            If m_bIsCreateOwnList Then
                If Not m_bIsNewList Then
                    If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                        objCurrentCountListInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                        objCurrentProductInfoList = objCurrentCountListInfoTable.Item(m_CLCurrentSiteInfo.strPlannerDesc)
                        objCurrentProductInfo = m_CLProductInfo
                    End If
                Else
                    If m_bNavigation Then
                        objCurrentProductInfoList = m_CLItemList
                        objCurrentProductInfo = m_CLItemList(m_iBackNextCount)
                    Else
                        objCurrentProductInfo = m_CLProductInfo
                        m_CLProductInfo.CreatedLocation = Macros.BACK_SHOP
                    End If
                End If

                m_ItemScreen = Nothing
                arrMultiSiteList = m_CLProductInfo.BSMultiSiteList
                If m_SelectedPOGSeqNum Is Nothing Then
                    m_SelectedPOGSeqNum = Macros.SELECT_INDEX_ZERO
                End If
            Else

                If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    objCurrentCountListInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                    objCurrentProductInfoList = objCurrentCountListInfoTable.Item(m_CLCurrentSiteInfo.strPlannerDesc)
                End If

                Dim objProductInfo As New CLProductInfo()
                objProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)
                strBootsCode = objProductInfo.BootsCode

                If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    arrProductList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                    For Each objCurrentProductInfo In arrProductList
                        If objCurrentProductInfo.BootsCode.Equals(strBootsCode) Then
                            Exit For
                        End If
                    Next
                End If
            End If

            'Sets the values
            Dim objDescriptionArray As ArrayList = New ArrayList()
            Dim iCount As Integer = 0
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(objCurrentProductInfo.Description)

            Dim strProductCode As String = ""
            strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(objCurrentProductInfo.ProductCode)

            With m_CLBackShopProductCount
                If m_bIsCreateOwnList Then
                    .Text = "Create Count - BS"
                    If m_bPlannerFlag Then
                        iCount = m_CLItemList.Count - 1
                    Else
                        iCount = m_CLItemList.Count
                    End If
                    If m_CLItemList.Count = 0 Then
                        .custCtrlBtnBack.Visible = False
                    Else
                        .custCtrlBtnBack.Visible = True
                    End If
                    If Not m_bIsNewList Then
                        .lblItemPosition.Text = (m_iProductListCount + 1).ToString() + "/" + objCurrentProductInfoList.Count.ToString()
                        .lblItemPosition.Visible = True
                    Else
                        'Disable controls for Create Own List
                        If m_bNavigation Then
                            .lblItemPosition.Text = (m_iBackNextCount + 1).ToString() + "/" + objCurrentProductInfoList.Count.ToString()
                            .lblItemPosition.Visible = True
                        Else
                            .lblItemPosition.Visible = False
                        End If
                    End If
                Else
                    .Text = "Count List - BS"
                    .lblItemPosition.Text = (m_iProductListCount + 1).ToString() + "/" + (objCurrentProductInfoList.Count).ToString()
                End If
                'If bIsItemCounted Then
                '    .lblCounted.Visible = True
                '    .lblCounted.Text = "COUNTED"
                'Else
                '    .lblCounted.Visible = False
                '    .lblCounted.Text = ""
                'End If
                .lblBootsCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(objCurrentProductInfo.BootsCode)

                .lblProductCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(strProductCode)
                .lblProductDesc1.Text = objDescriptionArray.Item(0)
                .lblProductDesc2.Text = objDescriptionArray.Item(1)
                .lblProductDesc3.Text = objDescriptionArray.Item(2)
                If objCurrentProductInfo.IsUnknownItem Then
                    .lblStatusDisplay.Text = "N/A"
                    If m_bIsCreateOwnList Then
                        If Not objCurrentProductInfo.IsSEL Then
                            .lblBootsCodeDisplay.Text = "00-00-000"
                        End If
                    End If
                Else
                    .lblStatusDisplay.Text = objAppContainer.objHelper.GetStatusDescription(objCurrentProductInfo.Status)
                End If
                If objCurrentProductInfo.TSF.Substring(0, 1).Equals("-") Then
                    .lblTotalStockFileDisplay.ForeColor = Color.Red
                Else
                    .lblTotalStockFileDisplay.ForeColor = .lblStatusDisplay.ForeColor
                End If
                .lblTotalStockFileDisplay.Text = objCurrentProductInfo.TSF

#If NRF Then
                If m_bIsCreateOwnList Then
                    .lblTotalStockFile.Text = "Start of Day Stock File:"
                Else
                    strLastActBuildTime = ConfigDataMgr.GetInstance.GetParam(ConfigKey.LAST_ACTBUILD_TIME)
                    strActiveDataTime = DateTime.ParseExact(strLastActBuildTime, "yyyy-MM-dd HH:mm:ss", Nothing)
                    strtempActiveDataTime = strActiveDataTime.Hour.ToString().PadLeft(2, "0") + ":" + _
                                        strActiveDataTime.Minute.ToString().PadLeft(2, "0")
                    .lblTotalStockFile.Text = ""
                    .lblTotalStockFile.Text = "Total Stock File at " + strtempActiveDataTime + " "
                End If
#ElseIf RF Then
                .lblTotalStockFile.Text = ""
                .lblTotalStockFile.Text = "Total Stock File"
#End If



                    'If objCurrentProductInfo.BackShopQuantity < 0 Then
                    '    .lblBackShopVal.Text = 0
                    'Else
                    '    .lblBackShopVal.Text = objCurrentProductInfo.BackShopQuantity
                    'End If
                    'If objCurrentProductInfo.SalesFloorQuantity < 0 Then
                    '    .lblShelfQty.Text = 0
                    'Else
                    '    .lblShelfQty.Text = objCurrentProductInfo.SalesFloorQuantity
                    'End If

                    'Gets the back shop quantity to be displayed
                    arrMultiSiteList = objCurrentProductInfo.BSMultiSiteList
                    If m_bIsCreateOwnList Then
                        If objCurrentProductInfo.IsUnknownItem Then
                            .lblStatusDisplay.Text = "N/A"
                            m_SelectedPOGSeqNum = 0
                        End If
                    End If

                    .lblCounted.Visible = False
                    If objCurrentProductInfo.TotalBSSiteCount = arrMultiSiteList.Count Then
                        .lblCounted.Visible = True
                    End If
                    For Each objSiteInfo In arrMultiSiteList
                        If objSiteInfo.strSeqNumber.Equals(m_SelectedPOGSeqNum) Then
                            If objSiteInfo.strBackShopQuantiy < 0 Then
                                .lblShelfQty.Text = 0
                            Else
                                .lblShelfQty.Text = objSiteInfo.strBackShopQuantiy
                            End If
                            Exit For
                        End If
                    Next

                    'populating multi sites
                    If arrMultiSiteList.Count = 1 Then
                        .lblSite.Visible = False
                        .cmbMultiSite.Visible = False
                    Else
                        .lblSite.Visible = True
                        .cmbMultiSite.Visible = True
                        .cmbMultiSite.Items.Clear()
                        For Each objMultiSite In arrMultiSiteList
                            .cmbMultiSite.Items.Add(objMultiSite.strPOGDescription)
                        Next
                        .cmbMultiSite.SelectedIndex = m_SelectedPOGSeqNum
                    End If
                'Stock File Accuracy  commented 
                    'ambli
                    'For OSSR

                    '#If RF Then
                    'If objAppContainer.OSSRStoreFlag = "Y" Then
                    '    .btn_OSSRItem.Visible = True
                    '    .lblOSSR.Visible = True
                    '    If objCurrentProductInfo.OSSRStatus = Macros.OSSR Then
                    '        .lblOSSR.Text = "OSSR"
                    '    Else
                    '        .lblOSSR.Text = ""
                    '    End If
                    '    If objCurrentProductInfo.OSSRQuantity < 0 Then
                    '        .lblOSSRVal.Text = 0
                    '    Else
                    '        .lblOSSRVal.Text = objCurrentProductInfo.OSSRQuantity
                    '    End If
                    'Else
                    '    .btn_OSSRItem.Visible = False
                    '    .lblOSSR.Visible = False
                    '    .lblOffSite.Visible = False
                    '    .lblOSSRVal.Visible = False
                    'End If
                    '#ElseIf NRF Then

                    '.btn_OSSRItem.Visible = False
                    '.lblOSSR.Visible = False
                    '.lblOffSite.Visible = False
                    '.lblOSSRVal.Visible = False
                    '#End If
                    'Fix for allignment for Back and OSSR button
                    '#If RF Then
                    '                If objAppContainer.OSSRStoreFlag = "Y" Then
                    '                    .custCtrlBtnBack.Location = New Point(59 * objAppContainer.iOffSet, 244* objAppContainer.iOffSet)
                    '                    .btn_OSSRItem.Location = New Point(111* objAppContainer.iOffSet, 244* objAppContainer.iOffSet)
                    '                Else
                    '                    .custCtrlBtnBack.Location = New Point(93* objAppContainer.iOffSet, 244* objAppContainer.iOffSet)
                    '                    .btn_OSSRItem.Visible = False
                    '                End If
                    '#ElseIf NRF Then
                    '.custCtrlBtnBack.Location = New Point(93 * objAppContainer.iOffSet, 244 * objAppContainer.iOffSet)
                    ' .btn_OSSRItem.Visible = False
                    '#End If
                    'Displays the total quantity

                    If objCurrentProductInfo.TotalQuantity < 0 Then
                        .lblTotalItemCountDisplay.Text = 0
                    Else
                        .lblTotalItemCountDisplay.Text = objCurrentProductInfo.TotalQuantity
                    End If

                    'Sets the store id and active data time to the status bar
                    .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                    .Visible = True
                    .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayCLBackShopProductCount of CLSessionMgr. Exception is:" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCLBackShopProductCount of CLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Displays the Count List Item Details Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayCLItemDetails(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objLogger.WriteAppLog("Entered DisplayCLItemDetails of CLSessionMgr", Logger.LogLevel.INFO)
        Dim objCurrentCountListInfoTable As Hashtable = New Hashtable()
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim arrMultiSiteList As ArrayList = New ArrayList()
        Dim arrItemList As New ArrayList()
        Dim objMultiSite As New CLMultiSiteInfo()
        Dim arrProductList As New ArrayList()
        Dim iIndex As Integer = -1
        Dim iSFSiteIndex As Integer = -1
        Dim iBSSiteIndex As Integer = -1
        Dim strBootsCode As String  
        Dim strLastActBuildTime As String = Nothing
        Dim strActiveDataTime As DateTime = Nothing
        Dim strtempActiveDataTime As String = Nothing
        Dim iCount As Integer = 0
        Dim objProductInfo As New CLProductInfo()
        Dim arrTempItemList As New ArrayList()
        Dim iFlag As Boolean = False

        'IsNotPlannerItem = False

        Try
            BCReader.GetInstance().StartRead()
            Dim objCurrentProductInfo As CLProductInfo = New CLProductInfo()
            With m_CLItemDetails
                'Added as part of SFA - Create own List
                If m_bIsCreateOwnList Then
                    'm_iLocation = Macros.COUNT_LIST_ITEMDETAILS
                    If m_CLItemList.Count = 0 Then
                        .custCtrlBtnBack.Visible = False
                    Else
                        .custCtrlBtnBack.Visible = True
                    End If
                    If Not m_strCurrentBootsCode Is Nothing Then
                        If m_bIsNewList Then
                            arrTempItemList = m_CLItemList
                        ElseIf Not m_CLCurrentProductGroup.ListID = Nothing Then '@@@@@@@M
                            objCurrentCountListInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                            objCurrentProductInfoList = objCurrentCountListInfoTable.Item(m_CLCurrentSiteInfo.strPlannerDesc)
                            arrTempItemList = objCurrentProductInfoList
                            'Else
                            'For Each objProductInfo In m_CLItemList
                            '    If objProductInfo.BootsCode.Equals(m_strCurrentBootsCode) Then
                            '        objCurrentProductInfo = objProductInfo
                            '        Exit For
                            '    End If
                            'Next
                        End If

                            For Each objItemInfo As CLProductInfo In arrTempItemList
                                iIndex = iIndex + 1
                                If objItemInfo.BootsCode.Equals(m_strCurrentBootsCode) Then
                                    strBootsCode = m_strCurrentBootsCode
                                m_iProductListCount = iIndex
                                m_iBackNextCount = iIndex
                                    Exit For
                                End If
                            Next

                            For Each objProductInfo In m_CLItemList
                                If objProductInfo.BootsCode.Equals(m_strCurrentBootsCode) Then
                                    objCurrentProductInfo = objProductInfo
                                    Exit For
                                End If
                            Next

                    Else
                        If m_bIsSiteInfo And Not m_bNavigation Then
                                If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                                    objCurrentCountListInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                                objCurrentProductInfoList = objCurrentCountListInfoTable.Item(m_CLCurrentSiteInfo.strPlannerDesc)
                                End If
                                'Dim objProductInfo As New CLProductInfo()
                                objProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)
                                strBootsCode = objProductInfo.BootsCode
                                For Each objProductInfo In m_CLItemList
                                    If objProductInfo.BootsCode.Equals(strBootsCode) Then
                                        objCurrentProductInfo = objProductInfo
                                        Exit For
                                    End If
                                Next
                            If Not m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                                'Default to PSP if PSP selected from site information screen
                                If Not m_iLocation = Macros.COUNT_LIST_PROCESSZERO And Not m_iLocation = Macros.COUNT_LIST_PROCESSVIEWLIST _
                                And Not m_iLocation = Macros.COUNT_LIST_PROCESSMULTISITESELECT Then
                                    If m_CLCurrentSiteInfo.strPlannerDesc = Macros.COUNT_PSP Or _
                                    m_CLCurrentSiteInfo.strPlannerDesc = Macros.COUNT_OSSR_PSP Then
                                        m_SelectedPOGSeqNum = Macros.PSP_INDEX
                                    End If
                                End If
                            End If
                            'ElseIf m_bIsBackNext Then
                        ElseIf ((m_bNavigation Or m_bIsBackNext) And Not m_bIsNewList) Then
                            objCurrentProductInfo = m_CLItemList.Item(m_iBackNextCount)
                            m_bIsBackNext = False
                            objCurrentProductInfoList = m_CLItemList
                        ElseIf m_bIsBackNext And m_bIsNewList Then
                            objCurrentProductInfo = m_CLItemList.Item(m_iBackNextCount)
                            m_bIsBackNext = False
                            objCurrentProductInfoList = m_CLItemList
                        ElseIf m_bIsNewList Then
                            objCurrentProductInfo = m_CLProductInfo
                        Else
                            objCurrentProductInfo = m_CLItemList.Item(m_iProductListCount)
                            objCurrentProductInfoList = m_CLItemList
                        End If
                        End If

                        m_ItemScreen = Macros.SCREEN_ITEM_CONFIRM
                        m_iLocation = Macros.COUNT_LIST_ITEMDETAILS
                        If m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                            .Text = "Create Count - BS"
                            arrMultiSiteList = objCurrentProductInfo.BSMultiSiteList
                            If Not m_strCurrentBootsCode Is Nothing Then
                                For Each objSiteInfo As CLMultiSiteInfo In arrMultiSiteList
                                    iBSSiteIndex = iBSSiteIndex + 1
                                    If objSiteInfo.IsCounted.Equals("N") Then
                                        m_SelectedPOGSeqNum = iBSSiteIndex
                                        Exit For
                                    End If
                                Next
                            End If
                        ElseIf m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                            .Text = "Create Count - SF"
                            arrMultiSiteList = objCurrentProductInfo.SFMultiSiteList
                            If Not m_strCurrentBootsCode Is Nothing Then
                                For Each objSiteInfo As CLMultiSiteInfo In arrMultiSiteList
                                    iSFSiteIndex = iSFSiteIndex + 1
                                    If objSiteInfo.IsCounted.Equals("N") Then
                                        m_SelectedPOGSeqNum = iSFSiteIndex
                                        Exit For
                                    End If
                                Next
                            End If
#If RF Then
                        ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                            .Text = "Create Count - OS"
                            arrMultiSiteList = objCurrentProductInfo.OSSRMultiSiteList
                            If Not m_strCurrentBootsCode Is Nothing Then
                                For Each objSiteInfo As CLMultiSiteInfo In arrMultiSiteList
                                    iBSSiteIndex = iBSSiteIndex + 1
                                    If objSiteInfo.IsCounted.Equals("N") Then
                                        m_SelectedPOGSeqNum = iBSSiteIndex
                                        Exit For
                                    End If
                                Next
                            End If
#End If
                        End If

                        .lblCounted.Visible = False
                        For Each objPlannerInfo As CLMultiSiteInfo In arrMultiSiteList
                            If objPlannerInfo.IsCounted.Equals("N") Then
                                iCount = iCount + 1
                            End If
                        Next
                        If iCount = 0 Then
                            .lblCounted.Visible = True
                        End If


                        'If normal count list
                Else

                        If Not m_iLocation = Macros.COUNT_SALES_FLOOR Then
                            'Default to PSP if PSP selected from site information screen
                        If Not m_iLocation = Macros.COUNT_LIST_PROCESSZERO And Not m_iLocation = Macros.COUNT_LIST_PROCESSVIEWLIST _
                        And Not m_iLocation = Macros.COUNT_LIST_PROCESSMULTISITESELECT Then
                            If m_CLCurrentSiteInfo.strPlannerDesc = Macros.COUNT_PSP Or _
                                m_CLCurrentSiteInfo.strPlannerDesc = Macros.COUNT_OSSR_PSP Then
                                m_SelectedPOGSeqNum = Macros.PSP_INDEX
                            End If
                        End If
                        End If

                        'Gets the current product info from hash table based on the list id 
                        'and the position of the item in the list as indicated by 
                        'm_iProductListCount variable
                        If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                            objCurrentCountListInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                            objCurrentProductInfoList = objCurrentCountListInfoTable.Item(m_CLCurrentSiteInfo.strPlannerDesc)
                        End If



                        'If from view list/discrepancy/item summary screen
                        If Not m_strCurrentBootsCode Is Nothing Then
                            For Each objItemInfo As CLProductInfo In objCurrentProductInfoList
                                iIndex = iIndex + 1
                                If objItemInfo.BootsCode.Equals(m_strCurrentBootsCode) Then
                                    strBootsCode = m_strCurrentBootsCode
                                    m_iProductListCount = iIndex
                                    Exit For
                                End If
                            Next
                        Else
                            objProductInfo = New CLProductInfo()
                            objProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)
                            strBootsCode = objProductInfo.BootsCode
                        End If

                        If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                            arrProductList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                            For Each objCurrentProductInfo In arrProductList
                                If objCurrentProductInfo.BootsCode.Equals(strBootsCode) Then
                                    m_CLCurrentItemInfo = objCurrentProductInfo
                                    Exit For
                                End If
                            Next
                        End If

                        'Check for not on planner item
                        'if so, skip confiramtion for the item
                        'If objCurrentProductInfo.IsNotPlannerItem Then
                        '    IsNotPlannerItem = True
                        'End If

                        Dim bIsItemCounted As Boolean = False
                        If m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                            .Text = "Count List - BS"
                            'Retrieves the multi site list for the item
                            arrMultiSiteList = objCurrentProductInfo.BSMultiSiteList
                            .lblCounted.Visible = False
                            'Enable counted label if item counted in all sites
                            If objCurrentProductInfo.TotalBSSiteCount = arrMultiSiteList.Count Then
                                .lblCounted.Visible = True
                            End If
                            If Not m_strCurrentBootsCode Is Nothing Then
                                For Each objSiteInfo As CLMultiSiteInfo In arrMultiSiteList
                                    iBSSiteIndex = iBSSiteIndex + 1
                                    If objSiteInfo.IsCounted.Equals("N") Then
                                        m_SelectedPOGSeqNum = iBSSiteIndex
                                        Exit For
                                    End If
                                Next
                            End If
                        ElseIf m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                            .Text = "Count List - SF"
                            'Retrieves the multi site list for the item
                            arrMultiSiteList = objCurrentProductInfo.SFMultiSiteList

                            'Enable counted label if item counted in all sites
                            .lblCounted.Visible = False
                            For Each objPlannerInfo As CLMultiSiteInfo In arrMultiSiteList
                                If objPlannerInfo.IsCounted.Equals("N") Then
                                    iCount = iCount + 1
                                End If
                            Next
                            If iCount = 0 Then
                                .lblCounted.Visible = True
                            End If
                            If Not m_strCurrentBootsCode Is Nothing Then
                                For Each objSiteInfo As CLMultiSiteInfo In arrMultiSiteList
                                    iSFSiteIndex = iSFSiteIndex + 1
                                    If objSiteInfo.IsCounted.Equals("N") Then
                                        m_SelectedPOGSeqNum = iSFSiteIndex
                                        Exit For
                                    End If
                                Next
                            End If
                            'For OSSR
#If RF Then
                        ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                            .Text = "Count List - OS"
                            'Retrieves the multi site list for the item
                            arrMultiSiteList = objCurrentProductInfo.OSSRMultiSiteList
                            .lblCounted.Visible = False
                            'Enable counted label if item counted in all sites
                            If objCurrentProductInfo.TotalOSSRSiteCount = arrMultiSiteList.Count Then
                                .lblCounted.Visible = True
                            End If
                            If Not m_strCurrentBootsCode Is Nothing Then
                                For Each objSiteInfo As CLMultiSiteInfo In arrMultiSiteList
                                    iBSSiteIndex = iBSSiteIndex + 1
                                    If objSiteInfo.IsCounted.Equals("N") Then
                                        m_SelectedPOGSeqNum = iBSSiteIndex
                                        Exit For
                                    End If
                                Next
                            End If
#End If

                        End If
                End If

                'populating multi sites
                If arrMultiSiteList.Count = 1 Then
                    .lblSite.Visible = False
                    .cmbMultiSite.Visible = False
                    m_bIsMultisited = False
                Else
                    .lblSite.Visible = True
                    .cmbMultiSite.Visible = True
                    m_bIsMultisited = True
                    .cmbMultiSite.Items.Clear()
                    'If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then 'Commented
                    '    .cmbMultiSite.Items.Add("Select")
                    'End If
                    For Each objMultiSite In arrMultiSiteList
                        .cmbMultiSite.Items.Add(objMultiSite.strPOGDescription)
                    Next
                    .cmbMultiSite.SelectedIndex = m_SelectedPOGSeqNum
                End If


                'Sets the values
                Dim objDescriptionArray As ArrayList = New ArrayList()
                objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(objCurrentProductInfo.Description)

                'Obtains the 13 digit product code
                Dim strProductCode As String = ""
                strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(objCurrentProductInfo.ProductCode)

                If Not m_bIsCreateOwnList Then
                    .lblItemPosition.Text = (m_iProductListCount + 1).ToString() + "/" + objCurrentProductInfoList.Count.ToString()
                    .lblItemPosition.Visible = True
                Else
                    If Not m_bIsNewList Then
                        .lblItemPosition.Text = (m_iProductListCount + 1).ToString() + "/" + objCurrentProductInfoList.Count.ToString()
                        .lblItemPosition.Visible = True
                    Else
                        If m_bNavigation Then
                            .lblItemPosition.Text = (m_iBackNextCount + 1).ToString() + "/" + m_CLItemList.Count.ToString()
                            .lblItemPosition.Visible = True
                        Else
                            .lblItemPosition.Visible = False
                        End If
                    End If
                End If

                'If bIsItemCounted Then
                '    .lblCounted.Visible = True
                '    .lblCounted.Text = "COUNTED"
                'Else
                '    .lblCounted.Visible = False
                'End If

                .lblBootsCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(objCurrentProductInfo.BootsCode)
                .lblProductCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(strProductCode)
                .lblProductDesc1.Text = objDescriptionArray.Item(0)
                .lblProductDesc2.Text = objDescriptionArray.Item(1)
                .lblProductDesc3.Text = objDescriptionArray.Item(2)

                If  objCurrentProductInfo.IsUnknownItem Then
                    .lblStatusDisplay.Text = "N/A"
                    If m_bIsCreateOwnList Then
                        If Not objCurrentProductInfo.IsSEL Then
                            .lblBootsCodeDisplay.Text = "00-00-000"
                        End If
                    End If
                Else
                    .lblStatusDisplay.Text = objAppContainer.objHelper.GetStatusDescription(objCurrentProductInfo.Status)
                End If

                'Displays the total quantity
                If objCurrentProductInfo.TotalQuantity < 0 Then
                    .lblTotalItemCountDisplay.Text = 0
                Else
                    .lblTotalItemCountDisplay.Text = objCurrentProductInfo.TotalQuantity
                End If
                If objCurrentProductInfo.TSF.Substring(0, 1).Equals("-") Then
                    .lblTotalStockFileDisplay.ForeColor = Color.Red
                Else
                    .lblTotalStockFileDisplay.ForeColor = .lblStatusDisplay.ForeColor
                End If
                .lblTotalStockFileDisplay.Text = objCurrentProductInfo.TSF

                'Display time along with total stock file label in NRF mode
#If NRF Then
                If m_bIsCreateOwnList Then
                    .lblTotalStockFile.Text = "Start of Day Stock File:"
                Else
                    strLastActBuildTime = ConfigDataMgr.GetInstance.GetParam(ConfigKey.LAST_ACTBUILD_TIME)
                    strActiveDataTime = DateTime.ParseExact(strLastActBuildTime, "yyyy-MM-dd HH:mm:ss", Nothing)
                    strtempActiveDataTime = strActiveDataTime.Hour.ToString().PadLeft(2, "0") + ":" + _
                                        strActiveDataTime.Minute.ToString().PadLeft(2, "0")
                    .lblTotalStockFile.Text = ""
                    .lblTotalStockFile.Text = "Total Stock File at " + strtempActiveDataTime + " "
                End If
#ElseIf RF Then
                .lblTotalStockFile.Text = ""
                .lblTotalStockFile.Text = "Total Stock File"
#End If
                m_CLProductInfo = objCurrentProductInfo

                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()

            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayCLItemDetails of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCLItemDetails of CLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Displays the Count List Summary Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
	'''SFA - Modified the function as part of CR11
    ''' <remarks></remarks>
    Private Sub DisplayCLSummary(ByVal o As Object, ByVal e As EventArgs)
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim iTotalItems As Integer = 0
        Dim objArrayList As ArrayList = New ArrayList()
        Dim bIsNotComplete As Boolean = False
        Dim iSalesFloorTemp As Integer = 0
        Dim iBackShopTemp As Integer = 0
        Dim iOSSRTemp As Integer = 0

        objAppContainer.objLogger.WriteAppLog("Entered DisplayCLSummary of CLSessionMgr", Logger.LogLevel.INFO)
        Try
            'Retrieves the total number of items count listed in this session
            If Not m_bIsCreateOwnList Then        
                If m_CLCurrentProductGroup.IsComplete Then
                    bIsNotComplete = True
                End If
                'SFA DEF#843 - Shows the summary of the selected count list
                If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    objCurrentProductInfoList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                End If

                For Each objCurrentProductInfo As CLProductInfo In objCurrentProductInfoList
                    If objCurrentProductInfo.SalesFloorQuantity >= 0 Then
                        iSalesFloorTemp = iSalesFloorTemp + 1
                    End If
#If NRF Then
                If objCurrentProductInfo.BackShopQuantity >= 0 Then
                    iBackShopTemp = iBackShopTemp + 1
                End If
#Else
                    If objAppContainer.OSSRStoreFlag = "Y" Then
                        If objCurrentProductInfo.OSSRFlag = "O" Then
                            If objCurrentProductInfo.OSSRQuantity >= 0 Then
                                iOSSRTemp = iOSSRTemp + 1
                            End If
                        Else
                            If objCurrentProductInfo.BackShopQuantity >= 0 Then
                                iBackShopTemp = iBackShopTemp + 1
                            End If
                        End If
                    Else
                        If objCurrentProductInfo.BackShopQuantity >= 0 Then
                            iBackShopTemp = iBackShopTemp + 1
                        End If

                    End If
#End If
                Next
                'Retrieves the total number of items in create own list
            Else
                Dim iCount As Integer = m_CLItemList.Count
                If Not iCount = 0 Then
                    For Each objProductInfo As CLProductInfo In m_CLItemList
                        If Not objProductInfo.SalesFloorQuantity < 0 Then
                            iSalesFloorTemp = iSalesFloorTemp + 1
                        End If
#If NRF Then
                        If Not objProductInfo.BackShopQuantity < 0 Then
                            iBackShopTemp = iBackShopTemp + 1
                        End If
                        If (iCount = iSalesFloorTemp And iCount = iBackShopTemp) Then
                            bIsNotComplete = True
                        End If

#Else
                        'SFA DEF#845 - Item considered as counted in OSSR only if its an OSSR item
                        If objProductInfo.OSSRFlag.Equals(Macros.OSSR) Then
                            If Not objProductInfo.OSSRQuantity < 0 Then
                                iOSSRTemp = iOSSRTemp + 1
                            End If
                        Else
                            If Not objProductInfo.BackShopQuantity < 0 Then
                                iBackShopTemp = iBackShopTemp + 1
                            End If
                        End If
                        If (iCount = iSalesFloorTemp And iCount = iBackShopTemp + iOSSRTemp) Then
                            bIsNotComplete = True
                        End If
#End If
                    Next
                End If
            End If

            With m_CLSummary
                If bIsNotComplete Then
                    .lblCountListDisplay.Text = "List is Complete"
                Else
                    .lblCountListDisplay.Text = "List is Incomplete"
                End If
                .lblNumBackShop.Text = iBackShopTemp
                .lblNumSalesFloor.Text = iSalesFloorTemp
#If RF Then
                If objAppContainer.OSSRStoreFlag = "Y" Then
                    .lblOSSRSiteVal.Text = iOSSRTemp
                Else
                    .lblOSSRSite.Visible = False
                    .lblOSSRSiteVal.Visible = False
                End If
                .Label1.Visible = false
#ElseIf NRF Then
                .Label1.Visible = True
                .lblOSSRSite.Visible = False
                .lblOSSRSiteVal.Visible = False
#End If
                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayCLSummary of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCLSummary of CLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Displays the Count List Site Information Screen.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayCLSiteInformation(ByVal o As Object, ByVal e As EventArgs)
        'Stock File Accuracy  - added new function to display site info
        Dim lstItem As ListViewItem
        Dim iItemCount As Integer
        Dim iMBSItemCount As Integer = 0
        Dim iPSPItemCount As Integer = 0
        Dim iBSUnknownItemCount As Integer = 0
        Dim iOSSRMBSItemCount As Integer = 0
        Dim iOSSRPSPItemCount As Integer = 0
        Dim objProductInfoTable As New Hashtable()
        Dim objUnknownProductInfoTable As New Hashtable()
        Dim objItemList As New ArrayList()
        Dim bIsPSPPresent As Boolean = False
        Dim bIsBSItemsPresent As Boolean = False
        Try
            objAppContainer.objLogger.WriteAppLog("Entered DisplayCLSiteInformation of CLSessionMgr", Logger.LogLevel.INFO)
            With m_CLSiteInfo

                .lstvwSiteInfo.Clear()
                .lstvwSiteInfo.Columns.Add("Site", 110 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                .lstvwSiteInfo.Columns.Add("Items", 60 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                .lstvwSiteInfo.Columns.Add("Counted", 60 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                'Populate site info details into list view
                If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                    If m_bIsCreateOwnList Then
                        .Text = "Create Count - SF"
                    Else
                    .Text = "Count List - SF"
                    End If
                    For Each objPlannerInfo As CLMultiSiteInfo In objAppContainer.objGlobalCLSiteInfoTable.Item(m_CLCurrentProductGroup.ListID)
                        If Not objPlannerInfo.strPlannerDesc.Trim().Equals("Unknown") Then
                            If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                                objProductInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                                objItemList = objProductInfoTable.Item(objPlannerInfo.strPlannerDesc.Trim())
                                For Each objProductInfo As CLProductInfo In objItemList
                                    If objProductInfo.CountStatus = "Y" Then
                                        iItemCount = iItemCount + 1
                                    End If
                                Next
                            End If
                            lstItem = New ListViewItem()
                            lstItem.Text = objPlannerInfo.strPlannerDesc.Trim()
                            lstItem.SubItems.Add(objPlannerInfo.iItemCount)
                            lstItem.SubItems.Add(iItemCount)
                            .lstvwSiteInfo.Items.Add(lstItem)
                            iItemCount = 0
                            lstItem = Nothing
                        Else
                            '###########
                            objUnknownProductInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                            Dim objUnknownItemList As ArrayList = objUnknownProductInfoTable.Item(objPlannerInfo.strPlannerDesc.Trim())
                            For Each objProductInfo As CLProductInfo In objUnknownItemList
                                If objProductInfo.SalesFloorQuantity > -1 Then
                                    iItemCount = iItemCount + 1
                                End If
                            Next

                            lstItem = New ListViewItem()
                            lstItem.Text = objPlannerInfo.strPlannerDesc.Trim()
                            lstItem.SubItems.Add(objPlannerInfo.iItemCount)
                            lstItem.SubItems.Add(iItemCount)
                            .lstvwSiteInfo.Items.Add(lstItem)
                            iItemCount = 0
                            lstItem = Nothing
                            '###########
                        End If
                    Next

                ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                    If m_bIsCreateOwnList Then
                        .Text = "Create Count - BS"
                    Else
                    .Text = "Count List - BS"
                    End If
                    'Populate site info for backshop
                    If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                        objProductInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                        objItemList = objProductInfoTable.Item(Macros.COUNT_MBS)
                        If Not objItemList Is Nothing Then
                            If objItemList.Count() > 0 Then
                                For Each objProductInfo As CLProductInfo In objItemList
                                    If Not objProductInfo.IsUnknownItem Then
                                        If objProductInfo.CountStatus = "Y" Then
                                            iMBSItemCount = iMBSItemCount + 1
                                        End If
                                    Else
                                        If objProductInfo.BackShopMBSQuantity > -1 Then
                                            iMBSItemCount = iMBSItemCount + 1
                                        End If
                                    End If
                                Next
                                If Not bIsBSItemsPresent Then
                                    bIsBSItemsPresent = True
                                End If
                            End If
                        End If
                    End If
                    If bIsBSItemsPresent Then
                        lstItem = New ListViewItem()
                        lstItem.Text = "Main Back Shop"
                        lstItem.SubItems.Add(objItemList.Count())
                        lstItem.SubItems.Add(iMBSItemCount)
                        .lstvwSiteInfo.Items.Add(lstItem)
                    End If
                    'Check whether PSP items present in CL or COL
#If NRF Then
                    If m_bIsCreateOwnList Then
                        If m_bIsItemsInPSP Then
                            bIsPSPPresent = True
                        End If
                    Else
                        If m_CLCurrentProductGroup.BackShopPSPCount > 0 Then
                            bIsPSPPresent = True
                        End If
                    End If
#ElseIf RF Then
                    bIsPSPPresent =True 
#End If
                    
                    If bIsPSPPresent Then
                        If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                            objProductInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                            objItemList = objProductInfoTable.Item(Macros.COUNT_PSP)
                            For Each objProductInfo As CLProductInfo In objItemList
                                If objProductInfo.CountStatus = "Y" Then
                                    iPSPItemCount = iPSPItemCount + 1
                                End If
                            Next
                        End If
                        lstItem = New ListViewItem()
                        lstItem.Text = "Pending Sales Plan"
                        lstItem.SubItems.Add(objItemList.Count())
                        lstItem.SubItems.Add(iPSPItemCount)
                        .lstvwSiteInfo.Items.Add(lstItem)
                    End If
#If NRF Then
                    If m_CLCurrentProductGroup.UnknownItemCount > 0 Then
                        'To dispaly unknown category
                        If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                            objProductInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                            objItemList = objProductInfoTable.Item(Macros.COUNT_BS_UNKNOWN)
                            For Each objProductInfo As CLProductInfo In objItemList
                                If objProductInfo.BackShopMBSQuantity > -1 Then
                                    iBSUnknownItemCount = iBSUnknownItemCount + 1
                                End If
                            Next
                        End If
                        lstItem = New ListViewItem()
                        lstItem.Text = "Unknown"
                        lstItem.SubItems.Add(objItemList.Count())
                        lstItem.SubItems.Add(iBSUnknownItemCount)
                        .lstvwSiteInfo.Items.Add(lstItem)
                    End If
#End If

                    ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                    If m_bIsCreateOwnList Then
                        .Text = "Create Count - OS"
                    Else
                        .Text = "Count List - OS"
                    End If
                    'Populate site info for OSSR
                    If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                        objProductInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                        objItemList = objProductInfoTable.Item(Macros.COUNT_OSSR_BS)
                        For Each objProductInfo As CLProductInfo In objItemList
                            If objProductInfo.CountStatus = "Y" Then
                                iOSSRMBSItemCount = iOSSRMBSItemCount + 1
                            End If
                        Next
                    End If
                    lstItem = New ListViewItem()
                    lstItem.Text = "OSSR"
                    lstItem.SubItems.Add(objItemList.Count())
                    lstItem.SubItems.Add(iOSSRMBSItemCount)
                    .lstvwSiteInfo.Items.Add(lstItem)

                    If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                        objProductInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                        objItemList = objProductInfoTable.Item(Macros.COUNT_OSSR_PSP)
                        For Each objProductInfo As CLProductInfo In objItemList
                            If objProductInfo.CountStatus = "Y" Then
                                iOSSRPSPItemCount = iOSSRPSPItemCount + 1
                            End If
                        Next
                    End If
                        lstItem = New ListViewItem()
                        lstItem.Text = "Pending Sales Plan"
                        lstItem.SubItems.Add(objItemList.Count())
                        lstItem.SubItems.Add(iOSSRPSPItemCount)
                        .lstvwSiteInfo.Items.Add(lstItem)

                End If
                    'Retrieves the store id and sets it to the status bar

                    .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                    .Visible = True
                    .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayCLSiteInformation of CLSessionMgr. Exception is:" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCLSiteInformation of CLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Displays the View List screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayCLViewList(ByVal o As Object, ByVal e As EventArgs)
        'Stock File Accuracy  - added new function to display view lsit
        Dim lstItem As ListViewItem
        Dim objProductInfoTable As New Hashtable()
        Dim objSiteList As ArrayList
        Dim objItemList As New ArrayList
        Dim objMBSItemList As New ArrayList()
        Dim objPSPItemList As New ArrayList()
        Dim objBSUNKNOWNItemList As New ArrayList()
        Dim objOSSRMBSItemList As New ArrayList()
        Dim objOSSRPSPItemList As New ArrayList()
        Dim objSiteInfoTable As New Hashtable()
        Dim objProductList As New ArrayList()
        Dim iCount As Integer
        Try
            objAppContainer.objLogger.WriteAppLog("Entered DisplayCLViewList of CLSessionMgr", Logger.LogLevel.INFO)
            With m_CLViewListScreen

                .lblViewList.Visible = True

                .lblDiscepancy.Visible = False
                .Info_button_i1.Visible = True
                .custCtrlBtnQuit.Visible = True
                .lblItemSelect.Text = "Select an item to Count or Quit"
                .lblViewList.Text = "View Count List"
                'Populate site info details into list view

                If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                    If m_bIsCreateOwnList Then
                        .Text = "Create Count - SF"
                    Else
                        .Text = "Count List - SF"
                    End If
                    .lblViewListSite.Visible = True
                    .lblViewListSiteDisplay.Visible = True

                    'Display the site name
                    .lblViewListSiteDisplay.Text = m_CLCurrentSiteInfo.strPlannerDesc

                    .lstvwItemDetails.Clear()
                    .lstvwItemDetails.Font = New System.Drawing.Font("Tahoma", 7.0!, FontStyle.Regular)
                    .lstvwItemDetails.Columns.Add("Item", 48 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("Description", 130 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("Multisites", 53 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .Text = "Count List - SF"
                    If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                        objProductInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                        objItemList = objProductInfoTable.Item(m_CLCurrentSiteInfo.strPlannerDesc)
                        For Each objItemInfo As CLProductInfo In objItemList
                            objProductList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                            iCount = 0
                            lstItem = New ListViewItem()

                            For Each objProductInfo As CLProductInfo In objProductList
                                If objProductInfo.BootsCode.Equals(objItemInfo.BootsCode) Then
                                    lstItem.Text = objProductInfo.BootsCode
                                    lstItem.SubItems.Add(LCase(objProductInfo.Description))
                                    For Each objPlannerInfo As CLMultiSiteInfo In objProductInfo.SFMultiSiteList
                                        If objPlannerInfo.IsCounted.Equals("Y") Then
                                            iCount = iCount + 1
                                        End If
                                    Next
									'Check whether the item is multisited
                                    If objProductInfo.SFMultiSiteList.Count() > 1 Then
									    'Defect Fix: 850
                                        If iCount > 1 And iCount < objProductInfo.SFMultiSiteList.Count() Then
                                            lstItem.SubItems.Add((objProductInfo.SFMultiSiteList.Count() - 1).ToString() + "*")
                                        Else
                                            lstItem.SubItems.Add(objProductInfo.SFMultiSiteList.Count() - 1)
                                        End If
                                    Else
										'Defect Fix: 850
                                        'If iCount < objProductInfo.SFMultiSiteList.Count() Then
                                        '    lstItem.SubItems.Add(objProductInfo.SFMultiSiteList.Count().ToString() + "*")
                                        'Else
                                            lstItem.SubItems.Add(objProductInfo.SFMultiSiteList.Count())
                                        'End If
                                    End If
                                    .lstvwItemDetails.Items.Add(lstItem)
                                    lstItem = Nothing
                                    Exit For
                                End If
                            Next

                        Next
                    End If
                ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                    If m_bIsCreateOwnList Then
                        .Text = "Create Count - BS"
                    Else
                        .Text = "Count List - BS"
                    End If
                    .lblViewListSite.Visible = True
                    .lblViewListSiteDisplay.Visible = True
                    If m_CLCurrentSiteInfo.strPlannerDesc = Macros.COUNT_MBS Then
                        .lblViewListSiteDisplay.Text = "Main Back Shop"
                    ElseIf m_CLCurrentSiteInfo.strPlannerDesc = Macros.COUNT_PSP Then
                        .lblViewListSiteDisplay.Text = "Pending Sales Plan"
                    ElseIf m_CLCurrentSiteInfo.strPlannerDesc = Macros.COUNT_BS_UNKNOWN Then
                        .lblViewListSiteDisplay.Text = "Unknown"
                    End If
                    .lstvwItemDetails.Clear()
                    .lstvwItemDetails.Font = New System.Drawing.Font("Tahoma", 7.0!, FontStyle.Regular)
                    .lstvwItemDetails.Columns.Add("Item", 46 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("Description", 122 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("MBS", 32 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("PSP", 31 * objAppContainer.iOffSet, HorizontalAlignment.Left)


                    'Fetch item list in mbs and psp to display the corresponding items
                    objSiteInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                    If objSiteInfoTable.ContainsKey(Macros.COUNT_MBS) Then
                        objMBSItemList = objSiteInfoTable.Item(Macros.COUNT_MBS)
                    End If
                    If objSiteInfoTable.ContainsKey(Macros.COUNT_PSP) Then
                        objPSPItemList = objSiteInfoTable.Item(Macros.COUNT_PSP)
                    End If
                    If objSiteInfoTable.ContainsKey(Macros.COUNT_BS_UNKNOWN) Then
                        objBSUNKNOWNItemList = objSiteInfoTable.Item(Macros.COUNT_BS_UNKNOWN)
                    End If
                    'Populate item info for backshop 

                    If m_CLCurrentSiteInfo.strPlannerDesc.Equals(Macros.COUNT_MBS) Then
                        For Each objItem As CLProductInfo In objMBSItemList
                            objItemList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                            objSiteList = New ArrayList()
                            lstItem = New ListViewItem()
                            For Each objProductInfo As CLProductInfo In objItemList
                                If objProductInfo.BootsCode.Equals(objItem.BootsCode) Then
                                    lstItem.Text = objProductInfo.BootsCode
                                    lstItem.SubItems.Add(LCase(objProductInfo.Description))
                                    objSiteList = objProductInfo.BSMultiSiteList
                                    If Not objSiteList Is Nothing Then
                                        For Each objPlannerInfo As CLMultiSiteInfo In objSiteList
                                            lstItem.SubItems.Add(objPlannerInfo.IsCounted)
                                        Next
                                    End If
                                    If Not objProductInfo.PendingSalesFlag Then
                                        lstItem.SubItems.Add("N/A")
                                    End If
                                    .lstvwItemDetails.Items.Add(lstItem)
                                    Exit For
                                End If
                            Next
                        Next
                        'if current site is psp, iterate through pspitemlist and display item detials
                    ElseIf m_CLCurrentSiteInfo.strPlannerDesc.Equals(Macros.COUNT_PSP) Then
                        For Each objItem As CLProductInfo In objPSPItemList
                            objItemList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                            objSiteList = New ArrayList()
                            lstItem = New ListViewItem()
                            For Each objProductInfo As CLProductInfo In objItemList
                                If objProductInfo.BootsCode.Equals(objItem.BootsCode) Then
                                    lstItem.Text = objProductInfo.BootsCode
                                    lstItem.SubItems.Add(LCase(objProductInfo.Description))
                                    objSiteList = objProductInfo.BSMultiSiteList
                                    If Not objSiteList Is Nothing Then
                                        For Each objPlannerInfo As CLMultiSiteInfo In objSiteList
                                            lstItem.SubItems.Add(objPlannerInfo.IsCounted)
                                        Next
                                    End If
                                    If Not objProductInfo.PendingSalesFlag Then
                                        lstItem.SubItems.Add("N/A")
                                    End If
                                    .lstvwItemDetails.Items.Add(lstItem)
                                    Exit For
                                End If
                            Next
                        Next
                        'if current site is BS unknown, iterate through bs unknownitemlist and display item detials
                    ElseIf m_CLCurrentSiteInfo.strPlannerDesc.Equals(Macros.COUNT_BS_UNKNOWN) Then
                        For Each objItem As CLProductInfo In objBSUNKNOWNItemList
                            objItemList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                            objSiteList = New ArrayList()
                            lstItem = New ListViewItem()
                            For Each objProductInfo As CLProductInfo In objItemList
                                If objProductInfo.BootsCode.Equals(objItem.BootsCode) Then
                                    lstItem.Text = objProductInfo.BootsCode
                                    lstItem.SubItems.Add(LCase(objProductInfo.Description))
                                    objSiteList = objProductInfo.BSMultiSiteList
                                    If Not objSiteList Is Nothing Then
                                        For Each objPlannerInfo As CLMultiSiteInfo In objSiteList
                                            lstItem.SubItems.Add(objPlannerInfo.IsCounted)
                                        Next
                                    End If
                                    If Not objProductInfo.PendingSalesFlag Then
                                        lstItem.SubItems.Add("N/A")
                                    End If
                                    .lstvwItemDetails.Items.Add(lstItem)
                                    Exit For
                                End If
                            Next
                        Next
                    End If
                ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                    If m_bIsCreateOwnList Then
                        .Text = "Create Count - OS"
                    Else
                        .Text = "Count List - OS"
                    End If
                    .lblViewListSite.Visible = True
                    .lblViewListSiteDisplay.Visible = True
                    If m_CLCurrentSiteInfo.strPlannerDesc = Macros.COUNT_OSSR_BS Then
                        .lblViewListSiteDisplay.Text = "OSSR"
                    ElseIf m_CLCurrentSiteInfo.strPlannerDesc = Macros.COUNT_OSSR_PSP Then
                        .lblViewListSiteDisplay.Text = "Pending Sales Plan"
                    End If
                    .lstvwItemDetails.Clear()
                    .lstvwItemDetails.Font = New System.Drawing.Font("Tahoma", 7.0!, FontStyle.Regular)
                    .lstvwItemDetails.Columns.Add("Item", 46 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("Description", 115 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("OSSR", 40 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("PSP", 30 * objAppContainer.iOffSet, HorizontalAlignment.Left)

                    'Fetch item list in mbs and psp to display the corresponding items
                    objSiteInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                    If objSiteInfoTable.ContainsKey(Macros.COUNT_OSSR_BS) Then
                        objOSSRMBSItemList = objSiteInfoTable.Item(Macros.COUNT_OSSR_BS)
                    End If
                    If objSiteInfoTable.ContainsKey(Macros.COUNT_OSSR_PSP) Then
                        objOSSRPSPItemList = objSiteInfoTable.Item(Macros.COUNT_OSSR_PSP)
                    End If

                    'Populate item info for backshop 
                    If m_CLCurrentSiteInfo.strPlannerDesc.Equals(Macros.COUNT_OSSR_BS) Then
                        For Each objItem As CLProductInfo In objOSSRMBSItemList
                            objItemList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                            objSiteList = New ArrayList()
                            lstItem = New ListViewItem()
                            For Each objProductInfo As CLProductInfo In objItemList
                                If objProductInfo.BootsCode.Equals(objItem.BootsCode) Then
                                    lstItem.Text = objProductInfo.BootsCode
                                    lstItem.SubItems.Add(LCase(objProductInfo.Description))
                                    objSiteList = objProductInfo.OSSRMultiSiteList
                                    If Not objSiteList Is Nothing Then
                                        For Each objPlannerInfo As CLMultiSiteInfo In objSiteList
                                            lstItem.SubItems.Add(objPlannerInfo.IsCounted)
                                        Next
                                    End If
                                    If Not objProductInfo.PendingSalesFlag Then
                                        lstItem.SubItems.Add("N/A")
                                    End If
                                    .lstvwItemDetails.Items.Add(lstItem)
                                    Exit For
                                End If
                            Next
                        Next
                        'if current site is psp, iterate through pspitemlist and display item detials
                    ElseIf m_CLCurrentSiteInfo.strPlannerDesc.Equals(Macros.COUNT_OSSR_PSP) Then
                        For Each objItem As CLProductInfo In objOSSRPSPItemList
                            objItemList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)

                            objSiteList = New ArrayList()
                            lstItem = New ListViewItem()

                            For Each objProductInfo As CLProductInfo In objItemList
                                If objProductInfo.BootsCode.Equals(objItem.BootsCode) Then
                                    lstItem.Text = objProductInfo.BootsCode
                                    lstItem.SubItems.Add(LCase(objProductInfo.Description))
                                    objSiteList = objProductInfo.OSSRMultiSiteList
                                    If Not objSiteList Is Nothing Then
                                        For Each objPlannerInfo As CLMultiSiteInfo In objSiteList
                                            lstItem.SubItems.Add(objPlannerInfo.IsCounted)
                                        Next
                                    End If
                                    If Not objProductInfo.PendingSalesFlag Then
                                        lstItem.SubItems.Add("N/A")
                                    End If
                                    .lstvwItemDetails.Items.Add(lstItem)
                                    Exit For

                                End If

                            Next

                        Next
                    End If
                End If

                'Retrieves the store id and sets it to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayCLViewList of CLSessionMgr. Exception is:" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCLViewList of CLSessionMgr", Logger.LogLevel.INFO)
    End Sub


    Private Sub DisplayCLFullPriceCheck(ByVal o As Object, ByVal e As EventArgs)
        BCReader.GetInstance().DisableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
        With m_CLFullPriceCheck
            .lblBtsCode.Text = objAppContainer.objHelper.FormatBarcode(m_CLPriceCheckProductInfo.BootsCode)
            'Fix for displaying Barcode in Full Price Check screen
            .lblEAN.Text = objAppContainer.objHelper.FormatBarcode(objAppContainer.objHelper.GeneratePCwithCDV(m_CLPriceCheckProductInfo.ProductCode))
            Dim objDescriptionArray As ArrayList = New ArrayList()
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(m_CLPriceCheckProductInfo.Description)

            .lblDesc1.Text = objDescriptionArray.Item(0)
            .lblDesc2.Text = objDescriptionArray.Item(1)
            .lblDesc3.Text = objDescriptionArray.Item(2)

            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)


            .Visible = True
            .Refresh()
        End With
    End Sub

    ''' <summary>
    ''' Displays the View List screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayCLMultiSiteDiscrepancy(ByVal o As Object, ByVal e As EventArgs)
        'Stock File Accuracy  - added new function to display view lsit
        Dim lstItem As ListViewItem
        Dim objItemList As New ArrayList()
        Try
            objAppContainer.objLogger.WriteAppLog("Entered DisplayCLMultiSiteDiscrepancy of CLSessionMgr", Logger.LogLevel.INFO)
            With m_CLViewListScreen
                .lblDiscepancy.Visible = True
                .lblDiscepancy.Text = ""
                .Info_button_i1.Visible = False
                .custCtrlBtnQuit.Visible = False
                .lblViewList.Visible = False
                .lblViewListSite.Visible = False
                .lblViewListSiteDisplay.Visible = False
                .lblItemSelect.Text = "Select the item you want to count"
                .lstvwItemDetails.Clear()
                .lstvwItemDetails.Columns.Add("Item", 60 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                .lstvwItemDetails.Columns.Add("Item Description", 170 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                For Each objProductInfo As CLProductInfo In m_arrItemList
                    lstItem = New ListViewItem()
                    lstItem.Text = objProductInfo.BootsCode
                    lstItem.SubItems.Add(objProductInfo.Description)
                    .lstvwItemDetails.Items.Add(lstItem)
                Next
                'Populate item details into list view
                If m_bIsCreateOwnList Then
                    If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                        .Text = "Create Count - SF"
                        .lblDiscepancy.Text = "All multisites must be completed for the following items:"
                    ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                        .Text = "Create Count - BS"
                        .lblDiscepancy.Text = "Main Back Shop count must be completed for the following items:"
                    ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                        .Text = "Create Count - OS"
                        .lblDiscepancy.Text = "OSSR count must be completed for the following items:"
                    End If
                Else
                    If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                        .Text = "Count List - SF"
                        .lblDiscepancy.Text = "All multisites must be completed for the following items:"
                    ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                        .Text = "Count List - BS"
                        .lblDiscepancy.Text = "Main Back Shop count must be completed for the following items:"
                    ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                        .Text = "Count List - OS"
                        .lblDiscepancy.Text = "OSSR count must be completed for the following items:"
                    End If
                End If
                'Retrieves the store id and sets it to the status bar

                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayCLMultiSiteDiscrepancy of CLSessionMgr. Exception is:" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        Finally
            m_arrItemList.Clear()
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCLMultiSiteDiscrepancy of CLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Displays the Count List Location Summary screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    'Included in inital req of SFA. But later removed
    '    Private Sub DisplayCLLocationSummary(ByVal o As Object, ByVal e As EventArgs)
    '        objAppContainer.objLogger.WriteAppLog("Entered DisplayCLLocationSummary of CLSessionMgr", Logger.LogLevel.INFO)
    '        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
    '        Dim iItemBackShopCount As Integer = 0
    '        Dim iItemOSSRCount As Integer = 0
    '        Dim iTotalOSSRItemCount As Integer = 0
    '        Try
    '            If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
    '                objCurrentProductInfoList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
    '            End If
    '            With m_CLFinish
    '                For Each objCurrentProductInfo As CLProductInfo In objCurrentProductInfoList

    '                    If objCurrentProductInfo.BackShopQuantity >= 0 Then
    '                        iItemBackShopCount = iItemBackShopCount + 1
    '                    End If

    '#If RF Then
    '                If objAppContainer.OSSRStoreFlag = "Y" Then
    '                    m_CLFinish.lblOSSRSite.Visible = True 
    '                    If objCurrentProductInfo.OSSRQuantity >= 0 Then
    '                        iItemOSSRCount = iItemOSSRCount + 1
    '                    End If
    '                    If objCurrentProductInfo.OSSRFlag = "O" Then
    '                        iTotalOSSRItemCount = iTotalOSSRItemCount + 1
    '                    End If
    '                Else
    '                    m_CLLocSelection.btn_OSSR.Visible = False
    '                    m_CLLocSelection.btn_OSSR.Enabled = False
    '                    m_CLLocSelection.lblNumOSSRItems.Visible = False
    '                End If
    '#End If
    '                Next
    '                'Sets the values

    '                .Btn_Help.Visible = True
    '                .lblCountListDisplay.Text = "Count List is incomplete"
    '                .Info_button_i1.Visible = True
    '                .lblNumSalesFloor.Text = m_arrItemList.Count.ToString() + "/" + objCurrentProductInfoList.Count.ToString()
    '                .lblNumBackShop.Text = iItemBackShopCount.ToString() + "/" + objCurrentProductInfoList.Count.ToString()

    '                'For OSSR
    '#If RF Then
    '                .lblNumBackShop.Text = iItemBackShopCount.ToString() + "/" + (m_CLCurrentProductGroup.NumberOfItems - iTotalOSSRItemCount).ToString()
    '                .lblNumOSSR.Text = iItemOSSRCount.ToString() + "/" + iTotalOSSRItemCount.ToString()
    '#End If
    '                .lblConfirmMessage.Text = "Do you want to quit out of the list?"
    '                'Sets the store id and active data time to the status bar
    '                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)


    '                .Visible = True
    '                .Refresh()
    '            End With

    '        Catch ex As Exception
    '            objAppContainer.objLogger.WriteAppLog("Exception occured DisplayCLLocationSummary of CLSessionMgr. Exception is: " _
    '                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
    '        Finally
    '            m_arrItemList.Clear()
    '        End Try
    '        objAppContainer.objLogger.WriteAppLog("Exit DisplayCLLocationSummary of CLSessionMgr", Logger.LogLevel.INFO)
    '    End Sub
    ''' <summary>
    ''' Displays the Item Summary screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayCLItemSummary(ByVal o As Object, ByVal e As EventArgs)
        'Stock File Accuracy  - added new function to display view lsit
        Dim lstItem As ListViewItem
        Dim objProductInfoTable As New Hashtable()
        Dim objSiteList As New ArrayList()
        Dim objItemList As New ArrayList()
        Dim bIsOSSR As Boolean = False
        Dim bIsBSOS As Boolean = False
        Try
            objAppContainer.objLogger.WriteAppLog("Entered DisplayCLItemSummary of CLSessionMgr", Logger.LogLevel.INFO)
#If RF Then
            If objAppContainer.OSSRStoreFlag = "Y" Then
                bIsBSOS = True
            End If
#End If
            With m_CLViewListScreen

                If m_bIsCreateOwnList Then
                    .Text = "Create Count"
                Else
                    .Text = "Count List"
                End If

                .lblViewList.Visible = True
                .lblDiscepancy.Visible = False
                .lblViewListSite.Visible = False
                .lblViewListSiteDisplay.Visible = False
                .Info_button_i1.Visible = True
                .custCtrlBtnQuit.Visible = True
                .lblItemSelect.Text = "Select an item to Count or Quit"
                'Populate site info details into list view

                'Display the label text
                .lblViewList.Text = "Location Summary Screen"
                .lstvwItemDetails.Clear()
                .lstvwItemDetails.Font = New System.Drawing.Font("Tahoma", 7.0!, FontStyle.Regular)
                .lstvwItemDetails.Columns.Add("Item", 46 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                If bIsBSOS Then
                    .lstvwItemDetails.Columns.Add("Description", 105 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("SF", 31 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("BS/OS", 50 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                Else
                    .lstvwItemDetails.Columns.Add("Description", 122 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("SF", 32 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("BS", 31 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                End If

                For Each objProductInfo As CLProductInfo In m_arrItemList
                    If Not objProductInfo.TotalQuantity < 0 Then
#If NRF Then
                        If m_bIsCreateOwnList Then
                            lstItem = New ListViewItem()
                            lstItem.Text = objProductInfo.BootsCode
                            lstItem.SubItems.Add(LCase(objProductInfo.Description))
                            If Not objProductInfo.SalesFloorQuantity < 0 Then
                                lstItem.SubItems.Add("Y")
                            Else
                                lstItem.SubItems.Add("N")
                            End If
                            If Not objProductInfo.BackShopQuantity < 0 Then
                                lstItem.SubItems.Add("Y")
                            Else
                                lstItem.SubItems.Add("N")
                            End If
                            .lstvwItemDetails.Items.Add(lstItem)
                        Else
                            If objProductInfo.BackShopQuantity < 0 Or objProductInfo.SalesFloorQuantity < 0 Then
                                lstItem = New ListViewItem()
                                lstItem.Text = objProductInfo.BootsCode
                                lstItem.SubItems.Add(LCase(objProductInfo.Description))
                                If Not objProductInfo.SalesFloorQuantity < 0 Then
                                    lstItem.SubItems.Add("Y")
                                Else
                                    lstItem.SubItems.Add("N")
                                End If

                                If Not objProductInfo.BackShopQuantity < 0 Then
                                    lstItem.SubItems.Add("Y")
                                Else
                                    lstItem.SubItems.Add("N")
                                End If
                                .lstvwItemDetails.Items.Add(lstItem)
                            End If
                        End If
#ElseIf RF Then
                        'If RF mode, Check whether item is OSSR item or BS item
                        bIsOSSR = False
                        If objAppContainer.OSSRStoreFlag = "Y" Then
                            If objProductInfo.OSSRFlag = "O" Then
                                bIsOSSR = True
                            End If
                        End If
                        If bIsOSSR Then
                            'If OSSR display OSSR count status ,but under the same column header BS
                            If objProductInfo.OSSRQuantity < 0 Or objProductInfo.SalesFloorQuantity < 0 Then
                                lstItem = New ListViewItem()
                                lstItem.Text = objProductInfo.BootsCode
                                lstItem.SubItems.Add(LCase(objProductInfo.Description))
                                If Not objProductInfo.SalesFloorQuantity < 0 Then
                                    lstItem.SubItems.Add("Y")
                                Else
                                    lstItem.SubItems.Add("N")
                                End If

                                If Not objProductInfo.OSSRQuantity < 0 Then
                                    lstItem.SubItems.Add("Y")
                                Else
                                    lstItem.SubItems.Add("N")
                                End If
                                .lstvwItemDetails.Items.Add(lstItem)
                            End If
                        Else
                            If objProductInfo.BackShopQuantity < 0 Or objProductInfo.SalesFloorQuantity < 0 Then
                                lstItem = New ListViewItem()
                                lstItem.Text = objProductInfo.BootsCode
                                lstItem.SubItems.Add(LCase(objProductInfo.Description))
                                If Not objProductInfo.SalesFloorQuantity < 0 Then
                                    lstItem.SubItems.Add("Y")
                                Else
                                    lstItem.SubItems.Add("N")
                                End If

                                If Not objProductInfo.BackShopQuantity < 0 Then
                                    lstItem.SubItems.Add("Y")
                                Else
                                    lstItem.SubItems.Add("N")
                                End If
                                .lstvwItemDetails.Items.Add(lstItem)
                            End If
                        End If
#End If
                    End If
                Next

                'Retrieves the store id and sets it to the status bar

                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayCLItemSummary of CLSessionMgr. Exception is:" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCLItemSummary of CLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Displays the Item Summary screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayCLBackShopSummary(ByVal o As Object, ByVal e As EventArgs)
        'Stock File Accuracy  - added new function to display view lsit
        Dim lstItem As ListViewItem
        Dim objProductInfoTable As New Hashtable()
        Dim objSiteList As New ArrayList()
        Dim objItemList As New ArrayList()
        Dim bIsOSSR As Boolean = False
        Try
            objAppContainer.objLogger.WriteAppLog("Entered DisplayCLBackShopSummary of CLSessionMgr", Logger.LogLevel.INFO)
            With m_CLViewListScreen

                .lblViewList.Visible = True
                .lblDiscepancy.Visible = False
                .lblViewListSite.Visible = False
                .lblViewListSiteDisplay.Visible = False
                .Info_button_i1.Visible = True
                .custCtrlBtnQuit.Visible = True
                .lblItemSelect.Text = "Select an item to Count or Quit"
                'Populate item info details into list view
                If m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                    .lblViewList.Text = "Backshop Summary"
                    .lstvwItemDetails.Clear()
                    .lstvwItemDetails.Font = New System.Drawing.Font("Tahoma", 7.0!, FontStyle.Regular)
                    .lstvwItemDetails.Columns.Add("Item", 46 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("Description", 122 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("MBS", 32 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("PSP", 31 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    If m_bIsCreateOwnList Then
                        .Text = "Create Count - BS"
                    Else
                        .Text = "Count List - BS"
                    End If


                    'Populate item info for backshop from item summary list
                    ' populated while quitting from site info page
                    For Each objProductInfo As CLProductInfo In m_arrItemList
                        lstItem = New ListViewItem()
                        lstItem.Text = objProductInfo.BootsCode
                        lstItem.SubItems.Add(LCase(objProductInfo.Description))
                        objSiteList = objProductInfo.BSMultiSiteList
                        For Each objPlannerInfo As CLMultiSiteInfo In objSiteList
                            lstItem.SubItems.Add(objPlannerInfo.IsCounted)
                        Next
                        .lstvwItemDetails.Items.Add(lstItem)
                    Next
                ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                    .lblViewList.Text = "Backshop Summary"
                    .lstvwItemDetails.Clear()
                    .lstvwItemDetails.Font = New System.Drawing.Font("Tahoma", 7.0!, FontStyle.Regular)
                    .lstvwItemDetails.Columns.Add("Item", 46 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("Description", 115 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("OSSR", 40 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lstvwItemDetails.Columns.Add("PSP", 30 * objAppContainer.iOffSet, HorizontalAlignment.Left)

                    If m_bIsCreateOwnList Then
                        .Text = "Create Count - OS"
                    Else
                        .Text = "Count List - OS"
                    End If

                    'Populate item info for OSSR from item summary list
                    ' populated while quitting from site info page
                    For Each objProductInfo As CLProductInfo In m_arrItemList
                        lstItem = New ListViewItem()
                        lstItem.Text = objProductInfo.BootsCode
                        lstItem.SubItems.Add(LCase(objProductInfo.Description))
                        objSiteList = objProductInfo.OSSRMultiSiteList
                        For Each objPlannerInfo As CLMultiSiteInfo In objSiteList
                            lstItem.SubItems.Add(objPlannerInfo.IsCounted)
                        Next
                        .lstvwItemDetails.Items.Add(lstItem)
                    Next
                End If
                'Retrieves the store id and sets it to the status bar

                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayCLBackShopSummary of CLSessionMgr. Exception is:" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCLBackShopSummary of CLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Screen Display method for Count List. 
    ''' All Count List sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName"></param>
    ''' <param name="iLocationCounted"></param>
    ''' <returns>True if success</returns>
    ''' <remarks></remarks>
    Public Function DisplayCLScreen(ByVal ScreenName As CLSCREENS, Optional ByVal iLocationCounted As Integer = 0, _
                  Optional ByVal iSelectedIndex As Integer = 0, Optional ByVal strBootsCode As String = Nothing, _
                  Optional ByVal arrItemList As ArrayList = Nothing, Optional ByVal iLocation As Integer = 0)
        Try
            objAppContainer.objLogger.WriteAppLog("Entered DisplayCLScreen of CLSessionMgr", Logger.LogLevel.INFO)
            BCReader.GetInstance().EnableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
            CurrentScreen = ScreenName
            Select Case ScreenName
                Case CLSCREENS.Home
                    'Invoke method to display the home screen
                    m_CLHome.Invoke(New EventHandler(AddressOf DisplayCLHome))
                Case CLSCREENS.LocationSelection
                    'Invoke method to display the location selection screen
                    m_CLLocSelection.Invoke(New EventHandler(AddressOf DisplayCLLocationSelection))
                Case CLSCREENS.SalesFloorProductCount
                    'Invoke method to display the product counting screen for back shop
                    'm_iProductListCount = 0
                    m_CLSalesFloorProductCount.Invoke(New EventHandler(AddressOf DisplayCLSalesFloorProductCount))
                Case CLSCREENS.BackShopProductCount
                    'Invoke method to display the product counting screen for back shop
                    'm_iProductListCount = 0
                    m_CLBackShopProductCount.Invoke(New EventHandler(AddressOf DisplayCLBackShopProductCount))
                Case CLSCREENS.ItemDetails
                    m_iCountedLocation = iLocationCounted
                    m_SelectedPOGSeqNum = iSelectedIndex
                    m_strCurrentBootsCode = strBootsCode
                    m_iLocation = iLocation
                    'Invoke method to display the item details screen for sales floor
                    m_CLItemDetails.Invoke(New EventHandler(AddressOf DisplayCLItemDetails))
                    'Case CLSCREENS.Finish
                    '    m_iCountedLocation = iLocationCounted
                    '    m_arrItemList = arrItemList
                    '    m_iLocation = iLocation
                    '    If iLocation = Macros.COUNT_LIST_LOCSUMMARY Then
                    '        m_CLFinish.Invoke(New EventHandler(AddressOf DisplayCLLocationSummary))
                    '    Else
                    '        'Invoke method to display the finish screen
                    '        m_CLFinish.Invoke(New EventHandler(AddressOf DisplayCLFinish))
                    '    End If
                Case CLSCREENS.Summary
                    'Invoke method to display the summary screen
                    m_CLSummary.Invoke(New EventHandler(AddressOf DisplayCLSummary))
                    'Stock File Accuracy  - Added new screens
                Case CLSCREENS.SiteInformation
                    m_iCountedLocation = iLocationCounted
                    'Invoke method to display the site info screen
                    m_CLSiteInfo.Invoke(New EventHandler(AddressOf DisplayCLSiteInformation))
                Case CLSCREENS.ViewListScreen
                    m_iCountedLocation = iLocationCounted
                    m_iLocation = iLocation
                    m_arrItemList = arrItemList
                    If iLocation = Macros.Count_LIST_DISCREPANCY Then
                        m_arrItemList = arrItemList
                        'Invoke method to display the multiSiteDiscrepancy screen
                        m_CLViewListScreen.Invoke(New EventHandler(AddressOf DisplayCLMultiSiteDiscrepancy))
                    ElseIf iLocation = Macros.COUNT_LIST_BACKSHOPSUMMARY Then
                        m_CLViewListScreen.Invoke(New EventHandler(AddressOf DisplayCLBackShopSummary))
                    ElseIf iLocation = Macros.COUNT_LIST_ITEMSUMMARY Then
                        m_CLViewListScreen.Invoke(New EventHandler(AddressOf DisplayCLItemSummary))
                    Else
                        'Invoke method to display the view list screen
                        m_iLocation = iLocation
                        m_CLViewListScreen.Invoke(New EventHandler(AddressOf DisplayCLViewList))
                    End If

                Case CLSCREENS.FullPriceCheck
                    'Invoke method to display the COL location selection screen
                    m_CLFullPriceCheck.Invoke(New EventHandler(AddressOf DisplayCLFullPriceCheck))

                    'Added as part of SFA - Create Own List
                Case CLSCREENS.COLLocationSelection
                    'Invoke method to display the COL location selection screen
                    m_CLLocSelection.Invoke(New EventHandler(AddressOf DisplayCOLLocationSelection))
                Case CLSCREENS.COLItemScan
                    'Invoke method to display the COL location selection screen
                    m_CLLocSelection.Invoke(New EventHandler(AddressOf DisplayCOLItemScanScreen))
                Case CLSCREENS.ViewCOLListScreen
                    'Invoke method to display the COL view list screen
                    m_CLViewListScreen.Invoke(New EventHandler(AddressOf DisplayCOLViewList))
                    'ambli
                    'For OSSR
#If RF Then
                Case CLSCREENS.OSSRProductCount
                    'Invoke method to display the product counting screen for OSSR
                    'm_iProductListCount = 0
                    m_CLOSSRProductCount.Invoke(New EventHandler(AddressOf DisplayCLOSSRProductCount))
#End If
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayCLScreen of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCLScreen of CLSessionMgr", Logger.LogLevel.INFO)
        Return True

    End Function
    ''' <summary>
    ''' Retrieves the data regarding the product groups from the database
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetProductGroupInfo() As Boolean
        Try
            objAppContainer.objLogger.WriteAppLog("Entered GetProductGroupInfo of CLSessionMgr", Logger.LogLevel.INFO)
            If objAppContainer.objGlobalCLProductGroupList.Count = 0 Then

                'Calls the GetCountList method in DataEngine to update the ProductGroupList
                If Not objAppContainer.objDataEngine.GetCountList(objAppContainer.objGlobalCLProductGroupList) Then
                    'MessageBox.Show(MessageManager.GetInstance().GetMessage("M11"), "Info", _
                    '        MessageBoxButtons.OK, _
                    '        MessageBoxIcon.Asterisk, _
                    '        MessageBoxDefaultButton.Button1)
                    'objAppContainer.objLogger.WriteAppLog(MessageManager.GetInstance().GetMessage("M11"), Logger.LogLevel.INFO)
                    'MessageBox.Show("There are no count lists to be counted")

                    'Stock File Accuraacy  updated
                    'Comment return statement so that it will display home screen to select create count list
                    CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.Home)
                    Return True
                    'Return False
                Else
                    CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.Home)
                    Return True
                End If
            Else
                CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.Home)
                Return True
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in GetProductGroupInfo of CLSessionMgr. Exception is:" _
                                                              + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit GetProductGroupInfo of CLSessionMgr", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Retrieves the list of items corresponding to the product group id from DB
    ''' </summary>
    ''' <param name="strListId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetItemList(ByVal strListId As String, ByRef strErrMsg As String) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered GetItemList of CLSessionMgr", Logger.LogLevel.INFO)

        Dim iCountListItemCount As Integer = 0
        Dim objCLProductInfoList As New ArrayList()
        Dim arrTempSFSiteList As ArrayList
        Dim arrPlanogramList As ArrayList
        Dim objNotOnPlannerItemList As New ArrayList()
        Dim objUnknownItemList As New ArrayList()
        Dim iIndex As Integer = 0
        Dim objProductGroupInfo As New CLProductGroupInfo()

        Dim objProductGroup As New CLProductGroupInfo()
        Dim m_iBS_PSP As Integer = 0
        Dim m_iOSSR_PSP As Integer = 0

        Dim objCountListDataTable As New DataTable()
        Dim List_ID As DataColumn = New DataColumn("List_ID")
        Dim Boots_Code As DataColumn = New DataColumn("Boots_Code")
        Dim Planner_Desc As DataColumn = New DataColumn("Planner_Desc")
        Dim Repeat_Count As DataColumn = New DataColumn("Repeat_Count")
        Dim POG_Description As DataColumn = New DataColumn("POG_Description")
        Dim DiffSession As DataColumn = New DataColumn("DiffSession")
        Dim rowData As DataRow
#If RF Then
        Dim objRFDataSource As New SMRFDataSource()
#End If

        List_ID.DataType = System.Type.GetType("System.String")
        Boots_Code.DataType = System.Type.GetType("System.String")
        Planner_Desc.DataType = System.Type.GetType("System.String")
        Repeat_Count.DataType = System.Type.GetType("System.String")
        POG_Description.DataType = System.Type.GetType("System.String")
        DiffSession.DataType = System.Type.GetType("System.String")
        objCountListDataTable.Columns.Add(List_ID)
        objCountListDataTable.Columns.Add(Boots_Code)
        objCountListDataTable.Columns.Add(Planner_Desc)
        objCountListDataTable.Columns.Add(Repeat_Count)
        objCountListDataTable.Columns.Add(POG_Description)
        objCountListDataTable.Columns.Add(DiffSession)

        Try
            
            'Calls the GetItemList method in DataEngine to update the CLProductInfo list
            If objAppContainer.objDataEngine.GetCountListItemDetails(strListId, objCLProductInfoList, strErrMsg) Then

                m_CLCurrentProductGroup.CurrentItemCount = objCLProductInfoList.Count()
                Dim iIndexArray As ArrayList = New ArrayList()
                Dim iItemIndex As Integer = 0
                If objCLProductInfoList.Count() > 0 Then
                    'Stores the multisite info for each item

                    For Each objProductInfo As CLProductInfo In objCLProductInfoList
                        'Handle unknown items
#If NRF Then
                        If objProductInfo.IsUnknownItem Then
                            Dim objUnknownSFSiteInfo As New CLSessionMgr.CLMultiSiteInfo()
                            Dim objUnknownSFSiteList As New ArrayList()
                            'objOtherSiteInfo.strPlannerID = "Other"
                            objUnknownSFSiteInfo.strModuleDesc = ""
                            objUnknownSFSiteInfo.strPlannerDesc = "Unknown"
                            objUnknownSFSiteInfo.strSalesFloorQuantiy = objProductInfo.SalesFloorQuantity
                            If objProductInfo.SalesFloorQuantity < 0 Then
                                objUnknownSFSiteInfo.strPOGDescription = "Unknown"
                                objUnknownSFSiteInfo.IsCounted = "N"
                            Else
                                objUnknownSFSiteInfo.strPOGDescription = "* Unknown (Counted)"
                                objUnknownSFSiteInfo.IsCounted = "Y"
                            End If
                            objUnknownSFSiteInfo.strSeqNumber = 0
                            objUnknownSFSiteList.Add(objUnknownSFSiteInfo)
                            objProductInfo.SFMultiSiteList = objUnknownSFSiteList

                            GetBSMultiSiteList(m_CLCurrentProductGroup.ListID, objProductInfo, m_iBS_PSP)
                            objUnknownItemList.Add(objProductInfo)
                            Continue For
                        End If
#End If
                        'Handle unknown items end
                        arrPlanogramList = New ArrayList()
                        If GetSFMultiSiteList(m_CLCurrentProductGroup.ListID, objProductInfo, arrPlanogramList) Then
#If RF Then
                            'Insert each row of data into a datatable
                            For Each objSiteInfo As CLMultiSiteInfo In arrPlanogramList
                                rowData = objCountListDataTable.NewRow()
                                rowData.Item("List_ID") = m_CLCurrentProductGroup.ListID
                                rowData.Item("Boots_Code") = objProductInfo.BootsCode
                                rowData.Item("Planner_Desc") = objSiteInfo.strPlannerDesc
                                rowData.Item("Repeat_Count") = objSiteInfo.iRepeatCount
                                rowData.Item("POG_Description") = objSiteInfo.strPOGDescription
                                If objProductInfo.SalesFloorQuantity > -1 Then
                                    rowData.Item("DiffSession") = "Y"
                                Else
                                    rowData.Item("DiffSession") = "N"
                                End If
                                objCountListDataTable.Rows.Add(rowData)
                            Next
#End If
                            'Sets the field indicating number of SELs queued to zero in the beginning
                            objProductInfo.NumSELsQueued = 0
#If NRF Then
                            GetBSMultiSiteList(m_CLCurrentProductGroup.ListID, objProductInfo, m_iBS_PSP)
#End If
                            'Stock File Accuracy  updated for RF site list
#If RF Then
                            'If Not objRFDataSource.GetPendingSalesFlag(objProductInfo) Then
                            '    Return False
                            'End If
                            If objAppContainer.OSSRStoreFlag = "Y" Then
                                If objProductInfo.OSSRFlag = "O" Then
                                    GetOSSRMultiSiteList(m_CLCurrentProductGroup.ListID, objProductInfo, m_iOSSR_PSP)
                                Else
                                    GetBSMultiSiteList(m_CLCurrentProductGroup.ListID, objProductInfo, m_iBS_PSP)
                                End If
                            Else
                                GetBSMultiSiteList(m_CLCurrentProductGroup.ListID, objProductInfo, m_iBS_PSP)
                            End If
#End If
                        Else
                            '''''''''
                            Dim objOtherSFSiteInfo As New CLSessionMgr.CLMultiSiteInfo()
                            Dim objSFSiteList As New ArrayList()
                            'objOtherSiteInfo.strPlannerID = "Other"
                            objOtherSFSiteInfo.strModuleDesc = ""
                            objOtherSFSiteInfo.strPlannerDesc = "Not On Planner"
                            objOtherSFSiteInfo.strSalesFloorQuantiy = objProductInfo.SalesFloorQuantity
                            If objProductInfo.SalesFloorQuantity < 0 Then
                                objOtherSFSiteInfo.strPOGDescription = "Not On Planner"
                                objOtherSFSiteInfo.IsCounted = "N"
                                'objProductInfo.CountStatus = "N"
                            Else
                                objOtherSFSiteInfo.strPOGDescription = "* Not On Planner (Counted)"
                                objOtherSFSiteInfo.IsCounted = "Y"
                                'objProductInfo.CountStatus = "Y"
                                'objProductInfo.IsSFDifferentSession = True
                            End If
                            objOtherSFSiteInfo.strSeqNumber = 0
                            objSFSiteList.Add(objOtherSFSiteInfo)
                            objProductInfo.SFMultiSiteList = objSFSiteList

#If NRF Then
                            GetBSMultiSiteList(m_CLCurrentProductGroup.ListID, objProductInfo, m_iBS_PSP)
#End If
                            'Stock File Accuracy  updated for RF site list
#If RF Then
                            'If Not objRFDataSource.GetPendingSalesFlag(objProductInfo) Then
                            '    Return False
                            'End If
                            If objAppContainer.OSSRStoreFlag = "Y" Then
                                If objProductInfo.OSSRFlag = "O" Then
                                    GetOSSRMultiSiteList(m_CLCurrentProductGroup.ListID, objProductInfo, m_iOSSR_PSP)
                                Else
                                    GetBSMultiSiteList(m_CLCurrentProductGroup.ListID, objProductInfo, m_iBS_PSP)
                                End If
                            Else
                                GetBSMultiSiteList(m_CLCurrentProductGroup.ListID, objProductInfo, m_iBS_PSP)
                            End If
#End If
                            objNotOnPlannerItemList.Add(objProductInfo)
                            '''''''''
                        End If
                    Next
                    objAppContainer.objGlobalCLProductInfoTable.Add(strListId, objCLProductInfoList)
                Else
                    Return False
                End If

                If m_CLCurrentProductGroup.CurrentItemCount > 0 Then
                    'Sets the PSP item count for the product group
                    For Each objProductGroup In objAppContainer.objGlobalCLProductGroupList
                        If objProductGroup.ListID.Equals(strListId) Then
                            objProductGroup.BackShopPSPCount = m_iBS_PSP
                            objProductGroup.OSSRPSPCount = m_iOSSR_PSP
                            objProductGroup.NotOnPlannerItemList = objNotOnPlannerItemList
                            objProductGroup.NotOnPlannerItemCount = objNotOnPlannerItemList.Count()
                            objProductGroup.UnknownItemList = objUnknownItemList
                            objProductGroup.UnknownItemCount = objUnknownItemList.Count()
                            Exit For
                        End If
                    Next

                    m_CLCurrentProductGroup.BackShopPSPCount = m_iBS_PSP
                    m_CLCurrentProductGroup.OSSRPSPCount = m_iOSSR_PSP
                    m_CLCurrentProductGroup.NotOnPlannerItemList = objNotOnPlannerItemList
                    m_CLCurrentProductGroup.NotOnPlannerItemCount = objNotOnPlannerItemList.Count()
                    m_CLCurrentProductGroup.UnknownItemList = objUnknownItemList
                    m_CLCurrentProductGroup.UnknownItemCount = objUnknownItemList.Count()

                    'Stock File Accuracy  Updated
                    'Call Function to populate the global structure  for RF alone
#If RF Then
                    If Not PopulateRFSiteInfoData(objCountListDataTable) Then
                        Return False
                    End If

#End If
                    Return True
                Else
                    Return False
                End If
            Else     'End getcountlistitems
                m_CLCurrentProductGroup.CurrentItemCount = 0
                Return False
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or _
                                ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in GetItemList of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit GetItemList of CLSessionMgr", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Processes the selection of any product from the list view
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessProductSelection() As Boolean
        Dim strListId As String = Nothing
        Dim strErrMsg As String = Nothing
        Dim strCreatorID As String = Nothing
        Dim iResult As Integer = 0
        Dim objCurrentProductGroup As CLProductGroupInfo = Nothing
        objAppContainer.objLogger.WriteAppLog("Entered ProcessProductSelection of CLSessionMgr", Logger.LogLevel.INFO)
        With m_CLHome
            Dim iIndex As Integer = -1
            Dim iCounter As Integer = 0
            Dim arrListType() As Char = {"S", "O", "U", "R", "N", "E", "G"}
            'Gets the selected list id from the listview
            Try
                objCurrentProductGroup = New CLProductGroupInfo()
                Dim bIsDataAvailable As Boolean = False
                If .lstvwProductGroup.SelectedIndices.Count > 0 Then
                    For iCounter = 0 To .lstvwProductGroup.Items.Count - 1
                        If .lstvwProductGroup.Items(iCounter).Selected Then
                            bIsDataAvailable = True
                            strListId = .lstvwProductGroup.Items(iCounter).Text
                            'Fixed in Integration Testing. This has to be modified if the list 
                            'type is going to have more than 1 character.
                            'strListId = strListId.Remove(strListId.Length - 1, 1)
                            'Stock File Accuracy  changed as length of type exceeded 1 character
                            strListId = strListId.TrimEnd(arrListType)
                            'SFA CR08 : Creator Id is extracted from list name (User ID XXX)
                            strCreatorID = .lstvwProductGroup.Items(iCounter).SubItems(1).Text
                            If strCreatorID.Contains("User Id") Then
                                strCreatorID = strCreatorID.Substring(strCreatorID.LastIndexOf(" "c) + 1, 3)
                            End If

                            Exit For
                        End If
                    Next
                End If

                For Each objCLProductGroup As CLProductGroupInfo In objAppContainer.objGlobalCLProductGroupList
                    iIndex = iIndex + 1
                    If objCLProductGroup.ListID.Equals(strListId) Then
                        'Get the Count List header details from array list.
                        objCurrentProductGroup = objAppContainer.objGlobalCLProductGroupList.Item(iIndex)
                        m_CLCurrentProductGroup = objCurrentProductGroup
                        m_ProductGroup = objCurrentProductGroup
                        objCurrentProductGroup = Nothing
                        'End Fixed in Integration Testing. 
                        m_iProductListCount = 0
                        Exit For
                    End If
                Next
                'Stock File Accuracy  - Lock feature no longer available in app.
                'Check status of the selected list or if the list is locked for today.
                'If m_CLCurrentProductGroup.IsLocked Then
                '    MessageBox.Show(MessageManager.GetInstance().GetMessage("M13"), "Info", _
                '                    MessageBoxButtons.OK, _
                '                    MessageBoxIcon.Asterisk, _
                '                    MessageBoxDefaultButton.Button1)
                '    Return False
#If RF Then
                If m_CLCurrentProductGroup.ActiveType.Equals("A") Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M96"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                    Return False
                End If
#End If
                'CR08 Change for SFA                                
                If m_CLCurrentProductGroup.ListType.Equals("U") AndAlso _
                    Not strCreatorID.Equals(objAppContainer.strCurrentUserID) Then
                    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M112"), "Confirmation", _
                          MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                          MessageBoxDefaultButton.Button1)

                    If iResult = MsgBoxResult.No Then
                        Return False
                    End If
                End If

                If bIsDataAvailable Then
                    If m_CLCurrentProductGroup.NumberOfItems = 0 Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                   MessageBoxButtons.OK, _
                                   MessageBoxIcon.Asterisk, _
                                   MessageBoxDefaultButton.Button1)
                        CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.Home)
                        Return False
                    End If
                    'If the list is already selected, then obtains the details from the hashtable
                    'else retrieves the details from DB
                    If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(strListId) Then
                        'For Each objCLProductGroup As CLProductGroupInfo In objAppContainer.objGlobalCLProductGroupList
                        '    If objCLProductGroup.ListID.Equals(strListId) Then
                        '        If objCLProductGroup.IsLocked Then
                        '            MessageBox.Show(MessageManager.GetInstance().GetMessage("M13"), "Info", _
                        '                            MessageBoxButtons.OK, _
                        '                            MessageBoxIcon.Asterisk, _
                        '                            MessageBoxDefaultButton.Button1)
                        '            Return False
                        '        End If
                        '        m_CLCurrentProductGroup.ListID = objCLProductGroup.ListID
                        '        m_CLCurrentProductGroup.ListDescription = objCLProductGroup.ListDescription
                        '        m_CLCurrentProductGroup.ListType = objCLProductGroup.ListType
                        '        m_CLCurrentProductGroup.IsLocked = objCLProductGroup.IsLocked
                        '        Exit For
                        '    End If
                        'Next
                        DisplayCLScreen(CLSCREENS.LocationSelection)
                        Return bIsDataAvailable
                    Else
                        If Not GetItemList(strListId, strErrMsg) Then
                            'IT-Integration bug fix-Message added
                            If Not strErrMsg Is Nothing Then
                                MessageBox.Show(strErrMsg, "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                            Else
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                            End If
                            'SFA DEF#824 - Refresh not needed on nakerror
                            'CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.Home)
                            Return False
                        Else
                            DisplayCLScreen(CLSCREENS.LocationSelection)
                            Return bIsDataAvailable
                        End If
                    End If
                Else
                    Return bIsDataAvailable
                    'TODO: Log error here
                End If

            Catch ex As Exception
#If RF Then
                If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or _
                            Macros.CONNECTION_REGAINED Then
                    Throw ex
                End If
#End If
                objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessProductSelection of CLSessionMgr. Exception is: " _
                                                      + ex.StackTrace, Logger.LogLevel.RELEASE)
                Return False
            End Try
            'End If
        End With
        objAppContainer.objLogger.WriteAppLog("Exit ProcessProductSelection of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the counting done on sales floor
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="iSalesFloorQty"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessSalesFloorItemCounted(ByVal strProductCode As String, ByVal iSalesFloorQty As Integer, _
                                                 ByVal strBootsCode As String) As Boolean
        Try
            objAppContainer.objLogger.WriteAppLog("Entered ProcessSalesFloorItemCounted of CLSessionMgr", Logger.LogLevel.INFO)

            'To unformat the product code by removing "-" and then remove CDV from that value
            Dim strUnFormattedProductCode As String
            Dim strUnFormatBootsCode As String
            strUnFormattedProductCode = objAppContainer.objHelper.UnFormatBarcode(strProductCode)
            strUnFormattedProductCode = strUnFormattedProductCode.Remove(strUnFormattedProductCode.Length - 1, 1)
            strUnFormatBootsCode = objAppContainer.objHelper.UnFormatBarcode(strBootsCode)
            'Updates the list with modified data
            If (iSalesFloorQty = "0") Then
                If (m_EntryType = BCType.EAN) Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M51"), "Invalid Data", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Exclamation, _
                                    MessageBoxDefaultButton.Button2)
                    Return True
                Else
            If UpdateSalesFloorProductInfo(strUnFormattedProductCode, iSalesFloorQty, strUnFormatBootsCode) Then

                'Displays the Sales Floor Count screen
                DisplayCLScreen(CLSCREENS.SalesFloorProductCount)
            End If
                End If
            Else
                If UpdateSalesFloorProductInfo(strUnFormattedProductCode, iSalesFloorQty, strUnFormatBootsCode) Then

                    'Displays the Sales Floor Count screen
                    DisplayCLScreen(CLSCREENS.SalesFloorProductCount)
                End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessSalesFloorItemCounted of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try

        objAppContainer.objLogger.WriteAppLog("Exit ProcessSalesFloorItemCounted of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the counting done on back shop
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="iBackShopQty"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessBackShopItemCounted(ByVal strProductCode As String, ByVal iBackShopQty As Integer, _
                                               ByVal strBootsCode As String) As Boolean
        Try
            objAppContainer.objLogger.WriteAppLog("Entered ProcessBackShopItemCounted of CLSessionMgr", Logger.LogLevel.INFO)

            'To unformat the product code by removing "-" and then remove CDV from that value
            Dim strUnFormattedProductCode As String
            Dim strUnFormatBootsCode As String
            strUnFormattedProductCode = objAppContainer.objHelper.UnFormatBarcode(strProductCode)
            strUnFormattedProductCode = strUnFormattedProductCode.Remove(strUnFormattedProductCode.Length - 1, 1)
            strUnFormatBootsCode = objAppContainer.objHelper.UnFormatBarcode(strBootsCode)

            If (iBackShopQty = "0") Then
                If (m_EntryType = BCType.EAN) Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M51"), "Invalid Data", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Exclamation, _
                                    MessageBoxDefaultButton.Button2)
                    Return True
                Else
            If UpdateBackShopProductInfo(strUnFormattedProductCode, iBackShopQty, strUnFormatBootsCode) Then
                        'Displays the Back Shop Count screen
                        DisplayCLScreen(CLSCREENS.BackShopProductCount)
                    End If
                End If
            Else
                If UpdateBackShopProductInfo(strUnFormattedProductCode, iBackShopQty, strUnFormatBootsCode) Then
                'Displays the Back Shop Count screen
                DisplayCLScreen(CLSCREENS.BackShopProductCount)
            End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessBackShopItemCounted of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessBackShopItemCounted of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the selection of Quit button on the Home screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessQuitListsScreen() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ProcessQuitListsScreen of CLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim bIsCounted As Boolean = False
            'Checks if any of the Lists are accessed

#If NRF Then
            If objAppContainer.m_CLBackShopCountedInfoList.Count > 0 Or objAppContainer.m_CLSalesFloorCountedInfoList.Count > 0 Or m_CLItemList.Count > 0 Then
                bIsCounted = True
            End If
#End If
            'ambli
            'For OSSR
#If RF Then
            If objAppContainer.m_CLBackShopCountedInfoList.Count > 0 Or objAppContainer.m_CLSalesFloorCountedInfoList.Count > 0 Or objAppContainer.m_CLOSSRCountedInfoList.Count > 0 Or m_CLItemList.Count > 0 Then
                bIsCounted = True
            End If
#End If

            If objAppContainer.objGlobalCreateCountList.Count > 0 Then
                bIsCounted = True
            End If
            'If counting is done for any of the lists then Summary screen is displayed
            'else quits the Count List to shelf management main menu by calling the end session
            If bIsCounted Then
                DisplayCLScreen(CLSessionMgr.CLSCREENS.Summary)
            Else
                EndSession()
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessQuitListsScreen of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessQuitListsScreen of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the OK selection on the CL Summary  screen.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessCLSummaryOK() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ProcessCLSummaryOK of CLSessionMgr", Logger.LogLevel.INFO)
        Dim objCLXRecord As CLXRecord = New CLXRecord()
        Try
#If RF Then
            'User generated list
            If m_bIsCreateOwnList Then

                'CLA update msg 
                Dim objCLA As CLARecord = New CLARecord()
                If Not objAppContainer.objExportDataManager.CreateCLA(Macros.END_USER_COUNTLIST, strListId_RF) Then
                    objAppContainer.objLogger.WriteAppLog("Cannot Create CLA record at Create Count Start Session", _
                                                      Logger.LogLevel.RELEASE)

                End If

                objCLXRecord.strListID = strListId_RF
                objCLXRecord.cIsCommit = Macros.CLX_COMMIT_YES
                objCLXRecord.strCountType = Macros.USER_COUNT_LIST
                If Not objAppContainer.objExportDataManager.CreateCLX(objCLXRecord) Then
                    objAppContainer.objLogger.WriteAppLog("Could not UpdateSalesFloorProductInfo of CLSessionMgr." _
                                                , Logger.LogLevel.RELEASE)
                End If
            Else
                'Normal Count list
                objCLXRecord.strListID = m_CLCurrentProductGroup.ListID
                objCLXRecord.cIsCommit = Macros.CLX_COMMIT_YES
                objCLXRecord.strCountType = m_CLCurrentProductGroup.ListType
                'Else
                '    objCLXRecord.cIsCommit = Macros.CLX_COMMIT_NO
                'End If

                'If m_CLCurrentProductGroup.ListType.Equals("H") Then
                '    objCLXRecord.strCountType = "H"
                'ElseIf m_CLCurrentProductGroup.ListType.Equals("R") Then
                '    objCLXRecord.strCountType = "R"
                'ElseIf m_CLCurrentProductGroup.ListType.Equals("U") Then
                '    objCLXRecord.strCountType = "U"
                'ElseIf m_CLCurrentProductGroup.ListType.Equals("N") Then
                '    objCLXRecord.strCountType = "N"
                'End If


                If Not objAppContainer.objExportDataManager.CreateCLX(objCLXRecord) Then
                    objAppContainer.objLogger.WriteAppLog("Could not UpdateSalesFloorProductInfo of CLSessionMgr." _
                                                , Logger.LogLevel.RELEASE)
                    'Return False
                End If

            End If
#End If
            DisplayCLScreen(CLSCREENS.Home)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessCLSummaryOK of CLSessionMgr. Exception is: " _
                                                              + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Processes the quit selection on the location selecetion screen.
    ''' Gets user confirmation as required.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessLocationQuit() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ProcessLocationQuit of CLSessionMgr", Logger.LogLevel.INFO)
        Dim arrItemSummaryLsit As New ArrayList()
        Dim objProductGroup As CLProductGroupInfo = New CLProductGroupInfo()
        Dim bIsOSSR As Boolean = False

        Try
            If m_bIsCreateOwnList Then
                m_ItemScreen = Nothing
                m_bIsNewList = False
                If m_CLItemList.Count = 0 Then
#If RF Then
                    Dim objCLA As CLARecord = New CLARecord()
                    If Not objAppContainer.objExportDataManager.CreateCLA(Macros.END_USER_COUNTLIST, strListId_RF) Then
                        objAppContainer.objLogger.WriteAppLog("Cannot Create CLA record at Create Count Start Session", _
                                                          Logger.LogLevel.RELEASE)
                        
                    End If
                    Dim objCLXRecord As CLXRecord = New CLXRecord()
                    objCLXRecord.strListID = strListId_RF
                    objCLXRecord.cIsCommit = Macros.CLX_COMMIT_YES
                    objCLXRecord.strCountType = Macros.USER_COUNT_LIST
                    If Not objAppContainer.objExportDataManager.CreateCLX(objCLXRecord) Then
                        objAppContainer.objLogger.WriteAppLog("Could not UpdateSalesFloorProductInfo of CLSessionMgr." _
                                                    , Logger.LogLevel.RELEASE)

                    End If
#End If
                    DisplayCLScreen(CLSessionMgr.CLSCREENS.Home)
                Else
                    If m_CLCurrentProductGroup.ListID = Nothing Then '@@@@@@@M .equals("")
                        If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                            m_iCountedLocation = Macros.COUNT_BACK_SHOP
                        Else
                            m_iCountedLocation = Macros.COUNT_SALES_FLOOR
                        End If
                        PopulateCOLSiteInfo()
                    End If
                    For Each objProductInfo As CLProductInfo In m_CLItemList
                        bIsOSSR = False
                        If Not objProductInfo.TotalQuantity < 0 Then
#If NRF Then
                            If objProductInfo.SalesFloorQuantity < 0 Or objProductInfo.BackShopQuantity < 0 Then
                                arrItemSummaryLsit.Add(objProductInfo)
                            End If
#ElseIf RF Then
                            If objAppContainer.OSSRStoreFlag = "Y" Then
                                If objProductInfo.OSSRFlag = "O" Then
                                    bIsOSSR = True
                                End If
                            End If

                            If bIsOSSR Then
                                If objProductInfo.SalesFloorQuantity < 0 Or objProductInfo.OSSRQuantity < 0 Then
                                    arrItemSummaryLsit.Add(objProductInfo)
                                End If
                            Else
                                If objProductInfo.SalesFloorQuantity < 0 Or objProductInfo.BackShopQuantity < 0 Then
                                    arrItemSummaryLsit.Add(objProductInfo)
                                End If
                            End If
#End If
                        End If
                    Next
                    If arrItemSummaryLsit.Count <> 0 Then
                        DisplayCLScreen(CLSCREENS.ViewListScreen, m_iCountedLocation, 0, Nothing, arrItemSummaryLsit, Macros.COUNT_LIST_ITEMSUMMARY)
                    ElseIf arrItemSummaryLsit.Count = 0 Then
                        'Directing to summary screen
                        CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.Summary)
                    End If
                End If
            Else

                'Flow change
                'Quit from location selection screen takes user to item summary screen,
                'if list contains any item for which action pending
                For Each objProductInfo As CLProductInfo In objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                    bIsOSSR = False
                    If Not objProductInfo.TotalQuantity < 0 Then
#If NRF Then
                        If objProductInfo.SalesFloorQuantity < 0 Or objProductInfo.BackShopQuantity < 0 Then
                            arrItemSummaryLsit.Add(objProductInfo)
                        End If
#ElseIf RF Then
                        'If RF mode, Check whether item is OSSR item or BS item
                        If objAppContainer.OSSRStoreFlag = "Y" Then
                            If objProductInfo.OSSRFlag = "O" Then
                                bIsOSSR = True
                            End If
                        End If

                        If bIsOSSR Then
                            If objProductInfo.SalesFloorQuantity < 0 Or objProductInfo.OSSRQuantity < 0 Then
                                arrItemSummaryLsit.Add(objProductInfo)
                            End If
                        Else
                            If objProductInfo.SalesFloorQuantity < 0 Or objProductInfo.BackShopQuantity < 0 Then
                                arrItemSummaryLsit.Add(objProductInfo)
                            End If
                        End If
#End If
                    End If
                Next

                'If item with action pending in list, move to item summary
                'or else to countlist home screen
                If arrItemSummaryLsit.Count <> 0 Then
                    DisplayCLScreen(CLSCREENS.ViewListScreen, m_iCountedLocation, 0, Nothing, arrItemSummaryLsit, Macros.COUNT_LIST_ITEMSUMMARY)
                ElseIf arrItemSummaryLsit.Count = 0 Then
                    'Check if all items counted in a alist. then remove it from home screen
#If NRF Then
                    If (m_CLCurrentProductGroup.SFCountedItems  = m_CLCurrentProductGroup.CurrentItemCount) And _
                       (m_CLCurrentProductGroup.BSCountedItems = m_CLCurrentProductGroup.CurrentItemCount) Then
                        m_CLCurrentProductGroup.IsComplete = True
                    End If
#ElseIf RF Then
                    If m_CLCurrentProductGroup.SFCountedItems = m_CLCurrentProductGroup.CurrentItemCount Then
                        Dim BSOS As Integer = 0
                        BSOS = m_CLCurrentProductGroup.BSCountedItems + m_CLCurrentProductGroup.OSSRCountedItems
                        If BSOS = m_CLCurrentProductGroup.CurrentItemCount Then
                            m_CLCurrentProductGroup.IsComplete = True
                        End If
                    End If
#End If
                   
                    'Update teh global array only when CLX is sent successfully.

                    Dim iIndexArray As Integer = 0
                    Dim iCounter As Integer = 0
                    'Dim objProductGroup As CLProductGroupInfo = New CLProductGroupInfo()
                    For iCounter = 0 To objAppContainer.objGlobalCLProductGroupList.Count - 1
                        Dim objTempProductGroup As CLProductGroupInfo = New CLProductGroupInfo()
                        objTempProductGroup = objAppContainer.objGlobalCLProductGroupList.Item(iCounter)
                        If objTempProductGroup.ListID.Equals(m_CLCurrentProductGroup.ListID) Then
                            iIndexArray = iCounter
                            objTempProductGroup.IsUpdate = True
                            objProductGroup = objTempProductGroup
                            Exit For
                        End If
                    Next
                    objAppContainer.objGlobalCLProductGroupList.RemoveAt(iIndexArray)
                    objAppContainer.objGlobalCLProductGroupList.Insert(iIndexArray, objProductGroup)

                    '#End If
                    'Directing to summary screen
                    CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.Summary)
                    End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessLocationQuit of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Entered ProcessLocationQuit of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    Public Function HandleConnectionLossGenerateCLX() As CLXRecord
        Try
            Dim objCLXRecord As CLXRecord = New CLXRecord()
            'SFA DEF #816 - handling connection loss for create own list 
            If m_bIsCreateOwnList Then
#If RF Then
                objCLXRecord.strListID = strListId_RF
                objCLXRecord.strCountType = Macros.USER_COUNT_LIST
#End If
            Else
                objCLXRecord.strListID = m_CLCurrentProductGroup.ListID
                'SFA DEF #811 - Included list type in CLX
                objCLXRecord.strCountType = m_CLCurrentProductGroup.ListType
            End If
            objCLXRecord.cIsCommit = Macros.CLX_COMMIT_NO
            Return objCLXRecord
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " occured @:" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return Nothing
        End Try
    End Function
    ''' <summary>
    ''' Processes the selection of sales floor button click on location selection screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessSalesFloorSelect() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ProcessSalesFloorSelect of CLSessionMgr", Logger.LogLevel.INFO)

        'Added as part of SFA-Create Own List-Display Item scan screen if creating a new list else
        'display Site info screen
        Try
            m_EntryType = BCType.None
            If m_bIsCreateOwnList Then
                m_iCountedLocation = Macros.COUNT_SALES_FLOOR
                If m_CLItemList.Count = 0 Then
                    m_bIsNewList = True
                    m_bNavigation = False
                    m_iListCreatedLoc = Macros.COUNT_SALES_FLOOR
#If RF Then
                    objCreateCountList.SiteType = Macros.SHOP_FLOOR
#End If
                    DisplayCLScreen(CLSessionMgr.CLSCREENS.COLItemScan)
                ElseIf m_iListCreatedLoc = Macros.COUNT_SALES_FLOOR Then
                    m_bNavigation = True
                    m_bIsNewList = True
                    m_iBackNextCount = 0
                    m_bIsBackNext = True
                    DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.SELECTED_INDEX)
                Else
                    m_bIsNewList = False
                    m_bNavigation = False
                    PopulateCOLSiteInfo()
                    m_iProductListCount = 0
                    DisplayCLScreen(CLSessionMgr.CLSCREENS.SiteInformation, Macros.COUNT_SALES_FLOOR)
                End If
            Else
                If m_CLCurrentProductGroup.SFItemCount <> 0 Then
                    m_iProductListCount = 0
                    CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.SiteInformation, Macros.COUNT_SALES_FLOOR)
                Else
                    Return False
                End If
            End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessSalesFloorSelect of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessSalesFloorSelect of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the selection of back shop button click on location selection screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessBackShopSelect() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ProcessBackShopSelect of CLSessionMgr", Logger.LogLevel.INFO)
        m_iProductListCount = 0
        Dim arrBootsCodeList As ArrayList
        Dim objCLProductInfoTable As Hashtable = New Hashtable()
        Dim strLocation As String = Nothing
        Dim strPlannerDesc As String = Nothing
        Dim objTempTable As New Hashtable()
        Dim arrPlannerList As New ArrayList()
        'Added as part of SFA - Create Own List - Display Item scan screen if creating a new list else
        'display Site info screen
        Try
            m_EntryType = BCType.None
            If m_bIsCreateOwnList Then
                m_iCountedLocation = Macros.COUNT_BACK_SHOP
                'If m_CLItemList.Count = 0 Then
                '    m_bIsNewList = True
                '    m_iListCreatedLoc = Macros.COUNT_BACK_SHOP
                '    DisplayCLScreen(CLSessionMgr.CLSCREENS.COLItemScan)
                'ElseIf m_bIsItemsInPSP Then
                '    m_bIsNewList = False
                '    PopulateCOLSiteInfo()
                '    DisplayCLScreen(CLSessionMgr.CLSCREENS.SiteInformation, Macros.COUNT_BACK_SHOP)
                'Else
                '    m_bIsNewList = False
                '    DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation)

                'End If
                If Not m_CLItemList.Count = 0 Then
                    If m_iListCreatedLoc = Macros.COUNT_BACK_SHOP Then
                        m_bNavigation = True
                        m_bIsNewList = True
                        m_iBackNextCount = 0
                        m_bIsBackNext = True
                        DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.BS_SELECTED_INDEX)
                    Else
                        PopulateCOLSiteInfo()
                        If m_CLCurrentProductGroup.BSItemCount <> 0 Or m_CLCurrentProductGroup.UnknownItemCount <> 0 Then
                            m_bNavigation = False
                            m_bIsNewList = False
                            'SFA - DEF 778
                            If m_CLCurrentProductGroup.BSItemCount > 0 And (m_CLCurrentProductGroup.UnknownItemCount > 0 Or _
                                                   m_bIsItemsInPSP) Then
                                DisplayCLScreen(CLSessionMgr.CLSCREENS.SiteInformation, Macros.COUNT_BACK_SHOP)
                            Else
                                'SFA - DEF 778
                                If m_CLCurrentProductGroup.UnknownItemCount > 0 Then
                                    strPlannerDesc = Macros.COUNT_BS_UNKNOWN
                                ElseIf m_CLCurrentProductGroup.BSItemCount > 0 Then
                                    strPlannerDesc = Macros.COUNT_MBS 'SIT SFA
                                End If
                                m_bIsSiteInfo = True  '&&Test
                                strLocation = strPlannerDesc
                                m_CLCurrentSiteInfo.strPlannerDesc = strPlannerDesc
                                DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation)
                            End If
                        Else
                                Return False  'To disable btn
                        End If
                    End If
                Else
                        m_bIsNewList = True
                        m_bNavigation = False
                        m_iListCreatedLoc = Macros.COUNT_BACK_SHOP
#If RF Then
                        objCreateCountList.SiteType = Macros.BACK_SHOP
#End If
                        DisplayCLScreen(CLSessionMgr.CLSCREENS.COLItemScan)
                End If
            Else
                If m_CLCurrentProductGroup.BSItemCount <> 0 Or m_CLCurrentProductGroup.UnknownItemCount <> 0 Then
                    'SFA -DEF 778
                    If m_CLCurrentProductGroup.BSItemCount > 0 And (m_CLCurrentProductGroup.UnknownItemCount > 0 Or _
                                                   m_CLCurrentProductGroup.BackShopPSPCount > 0) Then
                        'redirect to site information instead of itemdetails
                        CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.SiteInformation, Macros.COUNT_BACK_SHOP)
                    Else
                        If m_CLCurrentProductGroup.BSItemCount > 0 Then
                            strPlannerDesc = Macros.COUNT_MBS
                        ElseIf m_CLCurrentProductGroup.UnknownItemCount > 0 Then
                            strPlannerDesc = Macros.COUNT_BS_UNKNOWN
                        End If
                        strLocation = strPlannerDesc
                        m_CLCurrentSiteInfo.strPlannerDesc = strPlannerDesc
                        CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.ItemDetails, Macros.COUNT_BACK_SHOP, Macros.BS_SELECTED_INDEX)
                    End If
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessBackShopSelect of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessBackShopSelect of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the selection of site from site information screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessSiteSelect() As Boolean
        'Stock File Accuracy  - Added new function to process site select
        objAppContainer.objLogger.WriteAppLog("Entered ProcessSiteSelect of CLSessionMgr", Logger.LogLevel.INFO)
        m_iProductListCount = 0
        Dim strPlannerDesc As String = Nothing
        Dim objTempTable As New Hashtable()
        Dim arrPlannerList As New ArrayList()
        Try
            m_EntryType = BCType.None
            With m_CLSiteInfo

                'Set the plannerId and location id corresponding to the site selected
                'Location id is to identify the query to be called
                If m_bIsCreateOwnList Then
                    m_bIsSiteInfo = True
                End If
                If .lstvwSiteInfo.FocusedItem.Text.Equals("Main Back Shop") Then

                    strPlannerDesc = Macros.COUNT_MBS

                ElseIf .lstvwSiteInfo.FocusedItem.Text.Equals("OSSR") Then

                    strPlannerDesc = Macros.COUNT_OSSR_BS

                ElseIf .lstvwSiteInfo.FocusedItem.Text.Equals("Pending Sales Plan") Then

                    If m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                        strPlannerDesc = Macros.COUNT_PSP
                    ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                        strPlannerDesc = Macros.COUNT_OSSR_PSP
                    End If

                ElseIf .lstvwSiteInfo.FocusedItem.Text.Equals("Not On Planner") Then

                    strPlannerDesc = "Not On Planner"

                ElseIf .lstvwSiteInfo.FocusedItem.Text.Equals("Unknown") Then

                    If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                        strPlannerDesc = "Unknown"
                    ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                        strPlannerDesc = Macros.COUNT_BS_UNKNOWN
                    End If

                Else
                    Dim iIndex As Integer = 0
                    Dim iCounter As Integer = 0
                    'Gets the selected list id from the listview

                    Dim bIsDataAvailable As Boolean = False
                    If .lstvwSiteInfo.SelectedIndices.Count > 0 Then
                        For iCounter = 0 To .lstvwSiteInfo.Items.Count - 1
                            If .lstvwSiteInfo.Items(iCounter).Selected Then
                                bIsDataAvailable = True
                                strPlannerDesc = .lstvwSiteInfo.Items(iCounter).Text
                                Exit For
                            End If
                        Next
                    End If

                End If
            End With
            m_CLCurrentSiteInfo.strPlannerDesc = strPlannerDesc

            'Display item details when a site is selectes from the list
            If strPlannerDesc = Macros.COUNT_PSP Or strPlannerDesc = Macros.COUNT_MBS Or strPlannerDesc = Macros.COUNT_BS_UNKNOWN Then
                DisplayCLScreen(CLSCREENS.ItemDetails, Macros.COUNT_BACK_SHOP, Macros.BS_SELECTED_INDEX)
            ElseIf strPlannerDesc = Macros.COUNT_OSSR_PSP Or strPlannerDesc = Macros.COUNT_OSSR_BS Then
                DisplayCLScreen(CLSCREENS.ItemDetails, Macros.COUNT_OSSR, Macros.BS_SELECTED_INDEX)
            Else
                DisplayCLScreen(CLSCREENS.ItemDetails, Macros.COUNT_SALES_FLOOR, Macros.SELECTED_INDEX)
            End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessSiteSelect of CLSessionMgr. Exception is: " _
                                                          + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try

        objAppContainer.objLogger.WriteAppLog("Exit ProcessSiteSelect of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the selection of view lsit screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessViewList() As Boolean
        'Stock File Accuracy  - Added new function to process view lsit
        objAppContainer.objLogger.WriteAppLog("Entered ProcessViewListItemSelect of CLSessionMgr", Logger.LogLevel.INFO)
        Try
            'Dispaly view list screen
            If m_bIsCreateOwnList Then
                If m_bIsNewList Then
                    DisplayCLScreen(CLSCREENS.ViewCOLListScreen, m_iCountedLocation)
                Else
                    DisplayCLScreen(CLSCREENS.ViewListScreen, m_iCountedLocation, 0, Nothing, Nothing, Macros.COUNT_LIST_ITEMDETAILS)
                End If
            Else
                DisplayCLScreen(CLSCREENS.ViewListScreen, m_iCountedLocation)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessViewListItemSelect of CLSessionMgr. Exception is: " _
                                                          + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessViewList of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function

    ''' <summary>
    ''' Processes the selection of item from view lsit/discrepancy/item summary screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessViewListItemSelect() As Boolean
        'Stock File Accuracy  - Added new function 
        objAppContainer.objLogger.WriteAppLog("Entered ProcessViewListItemSelect of CLSessionMgr", Logger.LogLevel.INFO)
        Dim strBootsCode As String = Nothing
        Dim arrProductList As New ArrayList()
        Dim arrMultiSiteList As New ArrayList()
        Try
            m_EntryType = BCType.None
            With m_CLViewListScreen
                'Iterate through the list to get selected item
                If .lstvwItemDetails.SelectedIndices.Count > 0 Then
                    For iCounter = 0 To .lstvwItemDetails.Items.Count - 1
                        If .lstvwItemDetails.Items(iCounter).Selected Then
                            strBootsCode = .lstvwItemDetails.Items(iCounter).Text
                            Exit For
                        End If
                    Next
                End If

            End With

            If Not strBootsCode Is Nothing Then

                'Retrieves the multisite list for the item to set the current plannerid for the selected item
                If m_bIsCreateOwnList Then
                    If Not m_CLItemList.Count = 0 Then
                        For Each objProductInfo As CLProductInfo In m_CLItemList
                            If objProductInfo.BootsCode.Equals(strBootsCode) Then
                                If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                                    arrMultiSiteList = objProductInfo.SFMultiSiteList
                                ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                                    arrMultiSiteList = objProductInfo.BSMultiSiteList
                                ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                                    arrMultiSiteList = objProductInfo.OSSRMultiSiteList
                                End If
                                Exit For
                            End If
                        Next
                    End If
                Else
                    If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                        arrProductList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                        For Each objProductInfo As CLProductInfo In arrProductList
                            If objProductInfo.BootsCode.Equals(strBootsCode) Then
                                If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                                    arrMultiSiteList = objProductInfo.SFMultiSiteList
                                ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                                    arrMultiSiteList = objProductInfo.BSMultiSiteList
                                ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                                    arrMultiSiteList = objProductInfo.OSSRMultiSiteList
                                End If
                                Exit For
                            End If
                        Next
                    End If
                End If
                If m_iLocation = Macros.Count_LIST_DISCREPANCY Then
                    For Each objSiteInfo As CLMultiSiteInfo In arrMultiSiteList
                        m_CLCurrentSiteInfo.strPlannerDesc = objSiteInfo.strPlannerDesc
                        Exit For
                    Next
                    DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.SELECTED_INDEX, strBootsCode)
                ElseIf m_iLocation = Macros.COUNT_LIST_BACKSHOPSUMMARY Then
                    ProcessBackShopSummarySelectItem(strBootsCode)
                ElseIf m_iLocation = Macros.COUNT_LIST_ITEMSUMMARY Then
                    ProcessItemSummarySelectItem(strBootsCode)
                Else
                    'For sales floor, will display only items in one particular site.
                    'So no need to change current siteid.
                    'But for Back shop and OSSR set the current siteid
                    If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                        DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.SELECTED_INDEX, strBootsCode)
                    Else
                        DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.BS_SELECTED_INDEX, strBootsCode, Nothing, Macros.COUNT_LIST_PROCESSVIEWLIST)
                    End If
                End If
            End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessViewListItemSelect of CLSessionMgr. Exception is: " _
                                                          + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessViewListItemSelect of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the selection of item from back shop summary screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessBackShopSummarySelectItem(ByVal strBootsCode As String) As Boolean
        'Stock File Accuracy  - Added new function 
        Dim arrMultiSiteList As New ArrayList()
        Dim arrProductList As New ArrayList()
        Dim objCurrentProductInfo As New CLProductInfo()

        Dim bIsOSSR As Boolean = False


        objAppContainer.objLogger.WriteAppLog("Entered ProcessBackShopSummarySelectItem of CLSessionMgr", Logger.LogLevel.INFO)
        Try
            'Assign the back shop summary item list
            arrProductList = m_arrItemList
            'Retrieves the multisite list for the item to set the current plannerId
            For Each objCurrentProductInfo In arrProductList
                If objCurrentProductInfo.BootsCode.Equals(strBootsCode) Then
                    If m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                        arrMultiSiteList = objCurrentProductInfo.BSMultiSiteList
                    ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                        arrMultiSiteList = objCurrentProductInfo.OSSRMultiSiteList
                    End If
                    Exit For
                End If
            Next

            For Each objSiteInfo As CLMultiSiteInfo In arrMultiSiteList
                m_CLCurrentSiteInfo.strPlannerDesc = objSiteInfo.strPlannerDesc
                Exit For
            Next

            DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.BS_SELECTED_INDEX, strBootsCode)

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessBackShopSummarySelectItem of CLSessionMgr. Exception is: " _
                                                          + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessBackShopSummarySelectItem of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the selection of item from item summary screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessItemSummarySelectItem(ByVal strBootsCode As String) As Boolean
        'Stock File Accuracy  - Added new function 
        Dim arrMultiSiteList As New ArrayList()
        Dim arrProductList As New ArrayList()
        Dim objCurrentProductInfo As New CLProductInfo()
        m_iProductListCount = 0
        Dim bIsOSSR As Boolean = False


        objAppContainer.objLogger.WriteAppLog("Entered ProcessItemSummarySelectItem of CLSessionMgr", Logger.LogLevel.INFO)
        Try
            'On selecting item from item summary screen direct the user to the uncounted location
            'to count the item

            'Added as part of SFA - Create Own List
            If m_bIsCreateOwnList Then
                m_arrItemList = m_CLItemList
            End If

            For Each objProductInfo As CLProductInfo In m_arrItemList
                    If objProductInfo.BootsCode.Equals(strBootsCode) Then
                        If Not objProductInfo.TotalQuantity < 0 Then

#If NRF Then
                             If objProductInfo.SalesFloorQuantity < 0 Then
                                    m_iCountedLocation = Macros.COUNT_SALES_FLOOR
                             ElseIf objProductInfo.BackShopQuantity < 0 Then
                                    m_iCountedLocation = Macros.COUNT_BACK_SHOP
                             End If
#End If
#If RF Then
                            If objAppContainer.OSSRStoreFlag = "Y" Then
                                If objProductInfo.OSSRFlag = "O" Then
                                    bIsOSSR = True
                                End If
                            End If

                            'In RF mode, if not OSSR item the location will be either SF or BS
                            If Not bIsOSSR Then
                                If objProductInfo.SalesFloorQuantity < 0 Then
                                    m_iCountedLocation = Macros.COUNT_SALES_FLOOR

                                ElseIf objProductInfo.BackShopQuantity < 0 Then
                                    m_iCountedLocation = Macros.COUNT_BACK_SHOP
                                End If
                            Else
                                'In RF mode,ie: if bIsOSSR is true, then location will be SF and OSSR
                                If objProductInfo.SalesFloorQuantity < 0 Then
                                    m_iCountedLocation = Macros.COUNT_SALES_FLOOR

                                ElseIf objProductInfo.OSSRQuantity < 0 Then
                                    m_iCountedLocation = Macros.COUNT_OSSR
                                End If

                            End If
#End If
                    End If
                    Exit For
                End If
            Next
            Dim bIsUnknown As Boolean = False
            'Retrieves the multisite list for the item to set the current plannerId
            If m_bIsCreateOwnList Then
                For Each objCurrentProductInfo In m_CLItemList
                    If objCurrentProductInfo.BootsCode.Equals(strBootsCode) Then
                        m_bIsSiteInfo = True  '&&Test
                        m_bNavigation = False
                        If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                            arrMultiSiteList = objCurrentProductInfo.SFMultiSiteList
                        ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                            bIsUnknown = objCurrentProductInfo.IsUnknownItem
                            arrMultiSiteList = objCurrentProductInfo.BSMultiSiteList
                        ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                            arrMultiSiteList = objCurrentProductInfo.OSSRMultiSiteList
                        End If
                        Exit For
                    End If
                Next
            Else
                If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    arrProductList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                    For Each objCurrentProductInfo In arrProductList
                        If objCurrentProductInfo.BootsCode.Equals(strBootsCode) Then
                            If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                                arrMultiSiteList = objCurrentProductInfo.SFMultiSiteList
                            ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                                bIsUnknown = objCurrentProductInfo.IsUnknownItem
                                arrMultiSiteList = objCurrentProductInfo.BSMultiSiteList
                            ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                                arrMultiSiteList = objCurrentProductInfo.OSSRMultiSiteList
                            End If
                            Exit For
                        End If
                    Next
                End If
            End If
            For Each objSiteInfo As CLMultiSiteInfo In arrMultiSiteList
                If bIsUnknown Then
                    m_CLCurrentSiteInfo.strPlannerDesc = Macros.COUNT_BS_UNKNOWN
                Else
                    m_CLCurrentSiteInfo.strPlannerDesc = objSiteInfo.strPlannerDesc
                End If
                Exit For
            Next
                If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                    DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.SELECTED_INDEX, strBootsCode)
                Else
                    DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.BS_SELECTED_INDEX, strBootsCode)
                End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessItemSummarySelectItem of CLSessionMgr. Exception is: " _
                                                          + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessItemSummarySelectItem of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function

    ''' <summary>
    ''' Processes the selection of Quit from view lsit/ItemSummary screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessViewListQuit() As Boolean
        Dim iConfirm As Integer
        Dim objProductGroup As CLProductGroupInfo = New CLProductGroupInfo()
        'Stock File Accuracy  - Added new function 
        objAppContainer.objLogger.WriteAppLog("Entered ProcessViewListItemSelect of CLSessionMgr", Logger.LogLevel.INFO)
        Try
            m_EntryType = BCType.None
            'Quit from item summary displays home screen.
            'Quit from view list displays item detaisl screen
            If m_iLocation = Macros.COUNT_LIST_ITEMSUMMARY Then

                iConfirm = MessageBox.Show(MessageManager.GetInstance().GetMessage("M101"), "Info", _
                                                       MessageBoxButtons.OK, _
                                                       MessageBoxIcon.Asterisk, _
                                                       MessageBoxDefaultButton.Button1)

                'Update CLX
#If RF Then
                If Not m_bIsCreateOwnList Then
                    'SFA DEF #812 - Commenting for flow change - Start
                    'Dim objCLXRecord As CLXRecord = New CLXRecord()
                    'objCLXRecord.strListID = m_CLCurrentProductGroup.ListID
                    'If iConfirm = MsgBoxResult.Ok Then
                    '    objCLXRecord.cIsCommit = Macros.CLX_COMMIT_YES
                    'Else
                    '    objCLXRecord.cIsCommit = Macros.CLX_COMMIT_NO
                    'End If
                    'If m_CLCurrentProductGroup.ListType.Equals("H") Then
                    '    objCLXRecord.strCountType = "H"
                    'ElseIf m_CLCurrentProductGroup.ListType.Equals("R") Then
                    '    objCLXRecord.strCountType = "R"
                    'ElseIf m_CLCurrentProductGroup.ListType.Equals("U") Then
                    '    objCLXRecord.strCountType = "U"
                    'ElseIf m_CLCurrentProductGroup.ListType.Equals("N") Then
                    '    objCLXRecord.strCountType = "N"
                    'End If
                    'If Not objAppContainer.objExportDataManager.CreateCLX(objCLXRecord) Then
                    '    objAppContainer.objLogger.WriteAppLog("Could not UpdateSalesFloorProductInfo of CLSessionMgr." _
                    '                                , Logger.LogLevel.RELEASE)
                    '    Return False
                    'Else

                    'Update teh global array only when CLX is sent successfully.
                    If iConfirm = MsgBoxResult.Ok Then
                        Dim iIndexArray As Integer = 0
                        Dim iCounter As Integer = 0
                        'Dim objProductGroup As CLProductGroupInfo = New CLProductGroupInfo()
                        For iCounter = 0 To objAppContainer.objGlobalCLProductGroupList.Count - 1
                            Dim objTempProductGroup As CLProductGroupInfo = New CLProductGroupInfo()
                            objTempProductGroup = objAppContainer.objGlobalCLProductGroupList.Item(iCounter)
                            If objTempProductGroup.ListID.Equals(m_CLCurrentProductGroup.ListID) Then
                                iIndexArray = iCounter
                                objTempProductGroup.IsUpdate = True
                                objProductGroup = objTempProductGroup
                                Exit For
                            End If
                        Next
                        objAppContainer.objGlobalCLProductGroupList.RemoveAt(iIndexArray)
                        objAppContainer.objGlobalCLProductGroupList.Insert(iIndexArray, objProductGroup)
                    End If
                    'End If
                    'Else

                    '    Dim objCLA As CLARecord = New CLARecord()
                    '    If Not objAppContainer.objExportDataManager.CreateCLA(Macros.END_USER_COUNTLIST, strListId_RF) Then
                    '        objAppContainer.objLogger.WriteAppLog("Cannot Create CLA record at Create Count Start Session", _
                    '                                          Logger.LogLevel.RELEASE)
                    '        Return False
                    '    End If

                    '    Dim objCLXRecord As CLXRecord = New CLXRecord()
                    '    objCLXRecord.strListID = m_CLCurrentProductGroup.ListID
                    '    If iConfirm = MsgBoxResult.Ok Then
                    '        objCLXRecord.cIsCommit = Macros.CLX_COMMIT_YES
                    '    Else
                    '        objCLXRecord.cIsCommit = Macros.CLX_COMMIT_NO
                    '    End If
                    '    objCLXRecord.strCountType = Macros.USER_COUNT_LIST

                    '    If Not objAppContainer.objExportDataManager.CreateCLX(objCLXRecord) Then
                    '        objAppContainer.objLogger.WriteAppLog("Could not UpdateSalesFloorProductInfo of CLSessionMgr." _
                    '                                    , Logger.LogLevel.RELEASE)
                    '        Return False
                    '    End If
                    'SFA DEF #812 - Commenting for flow change - End
                End If
#End If
#If NRF Then
                  If Not m_bIsCreateOwnList Then
                'Update teh global array only when CLX is sent successfully.
                If iConfirm = MsgBoxResult.Ok Then
                    If Not m_bIsCreateOwnList Then
                    Dim iIndexArray As Integer = 0
                    Dim iCounter As Integer = 0
                    'Dim objProductGroup As CLProductGroupInfo = New CLProductGroupInfo()
                    For iCounter = 0 To objAppContainer.objGlobalCLProductGroupList.Count - 1
                        Dim objTempProductGroup As CLProductGroupInfo = New CLProductGroupInfo()
                        objTempProductGroup = objAppContainer.objGlobalCLProductGroupList.Item(iCounter)
                        If objTempProductGroup.ListID.Equals(m_CLCurrentProductGroup.ListID) Then
                            iIndexArray = iCounter
                            objTempProductGroup.IsUpdate = True 
                            objProductGroup = objTempProductGroup
                            Exit For
                        End If
                    Next
                    objAppContainer.objGlobalCLProductGroupList.RemoveAt(iIndexArray)
                    objAppContainer.objGlobalCLProductGroupList.Insert(iIndexArray, objProductGroup)
                End If
                 End If
                End If
#End If
                m_SelectedPOGSeqNum = Nothing
                'SFA DEF#812 
                'Direct the user to Summary
                DisplayCLScreen(CLSCREENS.Summary)
            ElseIf m_iLocation = Macros.COUNT_LIST_BACKSHOPSUMMARY Then
            If m_bIsCreateOwnList Then
                DisplayCLScreen(CLSCREENS.COLLocationSelection, m_iCountedLocation)
            Else
                DisplayCLScreen(CLSCREENS.LocationSelection, m_iCountedLocation)
            End If
            Else
            If m_bIsCreateOwnList Then
                    If m_ItemScreen = Macros.SCREEN_ITEM_CONFIRM Then
                        DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, m_SelectedPOGSeqNum)
                    Else
                        DisplayCLScreen(CLSCREENS.COLItemScan)
                    End If
            Else
                DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, m_SelectedPOGSeqNum)
            End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessViewListItemSelect of CLSessionMgr. Exception is: " _
                                                          + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessViewListItemSelect of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the selection of site from multi site list
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessMultiSiteSelect() As Boolean
        'Stock File Accuracy  - Added new function
        objAppContainer.objLogger.WriteAppLog("Entered ProcessMultiSiteSelect of CLSessionMgr", Logger.LogLevel.INFO)
        Try
            m_EntryType = BCType.None
            DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, m_SelectedPOGSeqNum, Nothing, Nothing, Macros.COUNT_LIST_PROCESSMULTISITESELECT)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessMultiSiteSelect of CLSessionMgr. Exception is: " _
                                                          + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessMultiSiteSelect of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the selection of No  button from finish/Location Selection screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessFinishNo() As Boolean
        'Stock File Accuracy  - Added new function
        objAppContainer.objLogger.WriteAppLog("Entered ProcessFinishNo of CLSessionMgr", Logger.LogLevel.INFO)
        Try
            If m_iLocation = Macros.COUNT_LIST_LOCSUMMARY Then
                DisplayCLScreen(CLSCREENS.SiteInformation, m_iCountedLocation)
            Else
                CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.Summary)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessFinishNo of CLSessionMgr. Exception is: " _
                                                          + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessFinishNo of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the selection of Yes button from finish/Location Selection screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessFinishYes() As Boolean
        'Stock File Accuracy  - Added new function
        objAppContainer.objLogger.WriteAppLog("Entered ProcessFinishYes of CLSessionMgr", Logger.LogLevel.INFO)
        Try
            If m_iLocation = Macros.COUNT_LIST_LOCSUMMARY Then
                DisplayCLScreen(CLSCREENS.ViewListScreen, m_iCountedLocation, 0, Nothing, Nothing, Macros.COUNT_LIST_ITEMSUMMARY)
            Else
                CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.Home)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessFinishYes of CLSessionMgr. Exception is: " _
                                                          + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessFinishYes of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Retrieves the list of sites corresponding to the product group id from DB
    ''' </summary>
    ''' <param name="strListId"></param>D:\BOOTS\Sep20_Code\Recalls_2010\McShMon\GUI\frmEXSummary.vb
    ''' <param name="arrPlannerList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetPlannerList(ByVal strListId As String, ByRef arrPlannerList As ArrayList) As Boolean
        'Stock File Accuracy  - Defined new function to retrieve planner list
        objAppContainer.objLogger.WriteAppLog("Entered GetPlannerList of CLSessionMgr", Logger.LogLevel.INFO)
        Dim objCLProductInfoTable As New Hashtable()
        Dim arrBootsCodeList As New ArrayList()
        Dim objPlannerInfo As New PlannerInfo()
        Try
            'Sets the currently selected product group to the m_CLCurrentProductGroup variable
            'For Each objCLProductGroup As CLProductGroupInfo In objAppContainer.objGlobalCLProductGroupList
            '    If objCLProductGroup.ListID.Equals(strListId) Then
            'Stock File Accuracy  -Lock feature removed.
            'If objCLProductGroup.IsLocked Then
            '    MessageBox.Show(MessageManager.GetInstance().GetMessage("M13"), "Info", _
            '        MessageBoxButtons.OK, _
            '        MessageBoxIcon.Asterisk, _
            '        MessageBoxDefaultButton.Button1)
            '    Return False
            'End If
            'm_CLCurrentProductGroup.ListID = objCLProductGroup.ListID
            'm_CLCurrentProductGroup.ListDescription = objCLProductGroup.ListDescription
            'm_CLCurrentProductGroup.ListType = objCLProductGroup.ListType
            'Stock File Accuracy  - commented
            'm_CLCurrentProductGroup.IsLocked = objCLProductGroup.IsLocked
            '                    objCLProductInfoList = New ArrayList()
            '                    Dim iCounter As Integer = 0
            '#If NRF Then
            '                    For iCounter = 0 To objCLProductGroup.NumberOfItems - 1
            '                        Dim objCLProductInfo As CLProductInfo = New CLProductInfo()
            '                        objCLProductInfoList.Add(objCLProductInfo)
            '                    Next
            '#End If
            'Exit For
            '    End If
            'Next

            'Calls the GetPlannerListDetails method in DataEngine to update the CLProductInfo list
            If Not objAppContainer.objGlobalCLSiteInfoTable.ContainsKey(strListId) Then
                If objAppContainer.objDataEngine.GetPlannerListDetails(strListId, arrPlannerList) Then
                    If m_CLCurrentProductGroup.NotOnPlannerItemCount > 0 Then
                        Dim objSiteInfo As New CLMultiSiteInfo()
                        objSiteInfo.strPlannerDesc = "Not On Planner"
                        objSiteInfo.strSeqNumber = "1"
                        objSiteInfo.iItemCount = m_CLCurrentProductGroup.NotOnPlannerItemCount
                        arrPlannerList.Add(objSiteInfo)
                    End If
                    objAppContainer.objGlobalCLSiteInfoTable.Add(strListId, arrPlannerList)
                    Return True
                Else
                    Return False
                End If
                Else
                    CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.SiteInformation, Macros.COUNT_SALES_FLOOR)
                End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or _
                                ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in GetPlannerList of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit GetPlannerList of CLSessionMgr", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Retrieves all multisites for item in Sales floor
    ''' </summary>
    ''' <param name="strListId"></param>
    ''' <param name="objProductInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetSFMultiSiteList(ByVal strListId As String, _
                                 ByRef objProductInfo As CLProductInfo, ByRef arrPlanogramList As ArrayList) As Boolean
        'Stock File Accuracy  - Defined new function to retrieve multisite list
        Dim arrPlannerList As New ArrayList()
        Dim arrTempSFMultiSiteList As New ArrayList()
        Dim arrSFMultiSiteList As New ArrayList()
        Dim objSFSiteInfo As CLMultiSiteInfo

        Dim arrSiteList As New ArrayList()
        Dim iRepeatCount As Integer
        Dim iSequence As Integer = -1
        Dim clMultiSiteData As CLMultiSiteInfo
        Dim arrMultiSiteList As New ArrayList()
        Dim iIndex As Integer = -1
        Dim arrPOGList As New ArrayList()


        objAppContainer.objLogger.WriteAppLog("Entered GetSFMultiSiteList of CLSessionMgr", Logger.LogLevel.INFO)
        Try
            'Calls the GetMultiSiteListDetails method in DataEngine to update the multisite list


            If objAppContainer.objDataEngine.GetMultiSiteDetails(strListId, objProductInfo.BootsCode, _
                                               arrPlannerList) Then

#If RF Then
                'In RF mode PGB response gives date of plannerType in arrplannerList
                'Convert it into clmultisiteinfo type 
                For Each objPlannerInfo As PlannerInfo In arrPlannerList
                    objSFSiteInfo = New CLMultiSiteInfo
                    objSFSiteInfo.strPlannerDesc = objPlannerInfo.POGDesc.Trim()
                    objSFSiteInfo.strPlannerID = objPlannerInfo.PlannerID
                    objSFSiteInfo.strPOGDescription = objPlannerInfo.POGDesc.Trim() + " - " + objPlannerInfo.Description.Trim()
                    objSFSiteInfo.strSalesFloorQuantiy = objProductInfo.SalesFloorQuantity
                    objSFSiteInfo.iRepeatCount = objPlannerInfo.RepeatCount
                    objSFSiteInfo.IsCounted = "N"
                    objSFSiteInfo.strModuleDesc = objPlannerInfo.Description.Trim()
                    arrTempSFMultiSiteList.Add(objSFSiteInfo)
                Next
                arrPlanogramList = arrTempSFMultiSiteList
#ElseIf NRF Then
            arrTempSFMultiSiteList = arrPlannerList
            'Comment in RF mode
            'arrPlanogramList = arrPlannerList  'To test
#End If

                'Expanding the repeat count for each site
                For Each objSiteInfo As CLMultiSiteInfo In arrTempSFMultiSiteList
                    arrPOGList.Add(objSiteInfo.strPlannerDesc.Trim())  ' Get the unique pog
                    iRepeatCount = objSiteInfo.iRepeatCount
                    For counter As Integer = 1 To iRepeatCount
                        arrSiteList.Add(objSiteInfo)
                    Next
                Next

                'Assign the unique set of planograms to the product
                objProductInfo.DistinctPOGList = arrPOGList

                'Check whether in same session or different
                If arrSiteList.Count > 1 And objProductInfo.SalesFloorQuantity > -1 Then
                    objProductInfo.IsSFDifferentSession = True
                End If

                'Assigning the sequence number to each site and populating multisitelist
                For Each PlannerInfo As CLMultiSiteInfo In arrSiteList
                    clMultiSiteData = New CLMultiSiteInfo
                    iSequence = iSequence + 1
                    clMultiSiteData.strSeqNumber = iSequence
                    If arrSiteList.Count = 1 Then
                        clMultiSiteData.strSalesFloorQuantiy = objProductInfo.SalesFloorQuantity
                    Else
                        clMultiSiteData.strSalesFloorQuantiy = -1
                    End If
                    clMultiSiteData.strPlannerDesc = PlannerInfo.strPlannerDesc
                    clMultiSiteData.strPlannerID = PlannerInfo.strPlannerID
                    clMultiSiteData.strModuleDesc = PlannerInfo.strModuleDesc
                    If Not objProductInfo.SalesFloorQuantity < 0 Then
                        clMultiSiteData.strPOGDescription = "* " & PlannerInfo.strPOGDescription & " (Counted)"
                        clMultiSiteData.IsCounted = "Y"
                    Else
                        clMultiSiteData.strPOGDescription = PlannerInfo.strPOGDescription
                        clMultiSiteData.IsCounted = PlannerInfo.IsCounted
                    End If

                    'clMultiSiteData.iSiteCount = PlannerInfo.iSiteCount
                    arrSFMultiSiteList.Add(clMultiSiteData)
                Next
                If arrSiteList.Count > 1 Then
                    Dim objOtherSiteInfo As New CLSessionMgr.CLMultiSiteInfo()
                    objOtherSiteInfo.strPOGDescription = "Other"
                    objOtherSiteInfo.strPlannerDesc = "Other"
                    objOtherSiteInfo.strPlannerID = "Other"
                    objOtherSiteInfo.IsCounted = "Y"
                    objOtherSiteInfo.strSalesFloorQuantiy = -1
                    objOtherSiteInfo.strSeqNumber = iSequence + 1
                    arrSFMultiSiteList.Add(objOtherSiteInfo)
                End If

                'To update new itemdetails
                objProductInfo.SFMultiSiteList = arrSFMultiSiteList
                Return True
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or _
                                ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in GetSFMultiSiteList of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit GetMultiSiteList of CLSessionMgr", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Retrieves all multisites for item in Back Shop
    ''' </summary>
    ''' <param name="strListId"></param>
    ''' <param name="objProductInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetBSMultiSiteList(ByVal strListId As String, _
                    ByRef objProductInfo As CLProductInfo, ByRef m_iBS_PSP As Integer) As Boolean
        'Stock File Accuracy  - Defined new function to populate multisite list for back shop
        Dim objCLProductInfoTable As New Hashtable()
        Dim arrBSMultiSiteList As New ArrayList()
        Dim objmultiSite As CLMultiSiteInfo = Nothing
        Dim arrProductList As New ArrayList()

        objAppContainer.objLogger.WriteAppLog("Entered GetBSMultiSiteList of CLSessionMgr", Logger.LogLevel.INFO)
        Try
            'Updates the multisite list for backshop
            objmultiSite = New CLMultiSiteInfo()
            objmultiSite.strPlannerID = Macros.COUNT_MBS
            objmultiSite.strPlannerDesc = Macros.COUNT_MBS
            objmultiSite.strBackShopQuantiy = objProductInfo.BackShopMBSQuantity
            If Not objProductInfo.BackShopMBSQuantity < 0 Then
                objmultiSite.strPOGDescription = "* Main Back Shop (Counted)"
                objmultiSite.IsCounted = "Y"
                objProductInfo.TotalBSSiteCount = objProductInfo.TotalBSSiteCount + 1
            Else
                objmultiSite.strPOGDescription = ("Main Back Shop")
                objmultiSite.IsCounted = "N"
            End If
            objmultiSite.strSeqNumber = "0"
            arrBSMultiSiteList.Add(objmultiSite)

            If objProductInfo.PendingSalesFlag Then
                m_iBS_PSP = m_iBS_PSP + 1
                objmultiSite = New CLMultiSiteInfo()
                objmultiSite.strPlannerDesc = Macros.COUNT_PSP
                objmultiSite.strPlannerID = Macros.COUNT_PSP
                objmultiSite.strBackShopQuantiy = objProductInfo.BackShopPSPQuantity
                If Not objProductInfo.BackShopPSPQuantity < 0 Then
                    objmultiSite.strPOGDescription = "* Pending Sales Plan (Counted)"
                    objmultiSite.IsCounted = "Y"
                    objProductInfo.TotalBSSiteCount = objProductInfo.TotalBSSiteCount + 1
                Else
                    objmultiSite.strPOGDescription = ("Pending Sales Plan")
                    objmultiSite.IsCounted = "N"
                End If
                objmultiSite.strSeqNumber = "1"
                arrBSMultiSiteList.Add(objmultiSite)

            End If

            objProductInfo.BSMultiSiteList = arrBSMultiSiteList

        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or _
                                ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in GetBSMultiSiteList of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit GetMultiSiteList of CLSessionMgr", Logger.LogLevel.INFO)
    End Function

    ''' <summary>
    ''' Retrieves all multisites for item in OSSR
    ''' </summary>
    ''' <param name="strListId"></param>
    ''' <param name="objProductInfo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetOSSRMultiSiteList(ByVal strListId As String, _
                    ByRef objProductInfo As CLProductInfo, ByRef m_iOSSR_PSP As Integer) As Boolean
        'Stock File Accuracy  - Defined new function to populate multisite list for OSSR
        Dim objCLProductInfoTable As New Hashtable()
        Dim arrOSSRMultiSiteList As New ArrayList()
        Dim objmultiSite As CLMultiSiteInfo = Nothing
        Dim arrProductList As New ArrayList()

        objAppContainer.objLogger.WriteAppLog("Entered GetOSSRMultiSiteList of CLSessionMgr", Logger.LogLevel.INFO)
        Try
            'Updates the multisite list for backshop
            objmultiSite = New CLMultiSiteInfo()
            objmultiSite.strPlannerID = Macros.COUNT_OSSR_BS
            objmultiSite.strPlannerDesc = Macros.COUNT_OSSR_BS
            objmultiSite.strOSSRQuantiy = objProductInfo.OSSRMBSQuantity
            If Not objProductInfo.OSSRMBSQuantity < 0 Then
                objmultiSite.strPOGDescription = "* OSSR (Counted)"
                objmultiSite.IsCounted = "Y"
                objProductInfo.TotalOSSRSiteCount = objProductInfo.TotalOSSRSiteCount + 1
            Else
                objmultiSite.strPOGDescription = ("OSSR")
                objmultiSite.IsCounted = "N"
            End If
            objmultiSite.strSeqNumber = "0"
            arrOSSRMultiSiteList.Add(objmultiSite)

            If objProductInfo.PendingSalesFlag Then
                m_iOSSR_PSP = m_iOSSR_PSP + 1
                objmultiSite = New CLMultiSiteInfo()
                objmultiSite.strPlannerDesc = Macros.COUNT_OSSR_PSP
                objmultiSite.strPlannerID = Macros.COUNT_OSSR_PSP
                objmultiSite.strOSSRQuantiy = objProductInfo.OSSRPSPQuantity
                If Not objProductInfo.OSSRPSPQuantity < 0 Then
                    objmultiSite.strPOGDescription = "* Pending Sales Plan (Counted)"
                    objmultiSite.IsCounted = "Y"
                    objProductInfo.TotalOSSRSiteCount = objProductInfo.TotalOSSRSiteCount + 1
                Else
                    objmultiSite.strPOGDescription = ("Pending Sales Plan")
                    objmultiSite.IsCounted = "N"
                End If
                objmultiSite.strSeqNumber = "1"
                arrOSSRMultiSiteList.Add(objmultiSite)

            End If

            objProductInfo.OSSRMultiSiteList = arrOSSRMultiSiteList


        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Or _
                                ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in GetOSSRSMultiSiteList of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit GetOSSRSMultiSiteList of CLSessionMgr", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Retrieves the list of item  corresponding to the site in product group id from DB
    ''' </summary>
    ''' <param name="strListId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Private Function GetBootsCodeList(ByVal strLocation As String, ByVal strPlannerDesc As String, _
                                      ByVal strListId As String, ByRef arrBootsCodeList As ArrayList) As Boolean
        'Stock File Accuracy  - Defined new function to retrieve planner list
        objAppContainer.objLogger.WriteAppLog("Entered GetBootsCodeList of CLSessionMgr", Logger.LogLevel.INFO)
        Try


            'Calls the GetBootsCodeItemList method in DataEngine to update the CLProductInfo list
            If objAppContainer.objDataEngine.GetBootsCodeItemList(strLocation, strPlannerDesc, strListId, arrBootsCodeList) Then

                Return True
            Else
                Return False
            End If
        Catch ex As Exception

            objAppContainer.objLogger.WriteAppLog("Exception occured in GetBootsCodeList of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit GetBootsCodeList of CLSessionMgr", Logger.LogLevel.INFO)
    End Function


    ''' <summary>
    ''' Updates the CLProductInfo object with the change in sales floor qty
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="iSalesFloorQty"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateSalesFloorProductInfo(ByVal strProductCode As String, ByVal iSalesFloorQty As Integer, _
                                                    ByVal strBootsCode As String) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered UpdateSalesFloorProductInfo of CLSessionMgr", Logger.LogLevel.INFO)
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim objCurrentCLSiteInfoTable As Hashtable = New Hashtable()
        Dim objCurrentSFSiteInfoList As ArrayList = New ArrayList()
        Dim objCurrentBSSiteInfoList As ArrayList = New ArrayList()
        Dim objCurrentOSSRSiteInfoList As ArrayList = New ArrayList()
        Dim objCurrentCountListInfoTable As New Hashtable()
        Dim objProductInfo As New CLProductInfo()
        Dim objCurrentProductList As New ArrayList()
        Dim objCurrentProductInfo As New CLProductInfo()
        Dim objSiteInfo As New CLMultiSiteInfo()

        Dim strPlannerDesc As String = Nothing
        Dim iItemIndex As Integer = -1
        Dim iSiteIndex As Integer = -1
        Dim iInfoIndex As Integer = -1
        Dim iSalesFloorVal As Integer = -1
        Dim iBackShopVal As Integer = -1
        Dim iOSSRVal As Integer = -1
        Dim iTotalSalesFloorQty As Integer = 0
        Dim iTotalBackShopQty As Integer = 0
        Dim iTotalOSSRQty As Integer = 0
        Try

            'Updates the counted item in site with "Y"
            If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                objCurrentProductList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                For Each objCurrentProductInfo In objCurrentProductList
                    iItemIndex = iItemIndex + 1
                    If objCurrentProductInfo.BootsCode.Equals(strBootsCode) Then
                        objCurrentSFSiteInfoList = objCurrentProductInfo.SFMultiSiteList
#If NRF Then
                        objCurrentBSSiteInfoList = objCurrentProductInfo.BSMultiSiteList
#ElseIf RF Then
                        If objAppContainer.OSSRStoreFlag = "Y" Then
                            If objCurrentProductInfo.OSSRFlag = "O" Then
                                objCurrentOSSRSiteInfoList = objCurrentProductInfo.OSSRMultiSiteList
                            Else
                                objCurrentBSSiteInfoList = objCurrentProductInfo.BSMultiSiteList
                            End If
                        Else
                            objCurrentBSSiteInfoList = objCurrentProductInfo.BSMultiSiteList
                        End If
#End If
                        Exit For
                    End If
                Next
            End If

            For Each objSiteInfo In objCurrentSFSiteInfoList
                If objSiteInfo.strSeqNumber.Equals(m_SelectedPOGSeqNum) Then
                    If objSiteInfo.IsCounted.Equals("N") Or objSiteInfo.strPOGDescription.Equals("Other") Then
                        objSiteInfo.strPOGDescription = "* " & objSiteInfo.strPOGDescription.Trim() & " (Counted)"
                        objSiteInfo.IsCounted = "Y"
                        'objCurrentProductInfo.TotalSFSiteCount = objCurrentProductInfo.TotalSFSiteCount + 1
                    End If
                    objSiteInfo.strSalesFloorQuantiy = iSalesFloorQty
                    strPlannerDesc = objSiteInfo.strPlannerDesc.Trim()
                    iSiteIndex = iSiteIndex + 1
                    Exit For
                End If
                iSiteIndex = iSiteIndex + 1
            Next

            'Updates CLSFMultiSiteDetails with new count
            objCurrentSFSiteInfoList.RemoveAt(iSiteIndex)
            objCurrentSFSiteInfoList.Insert(iSiteIndex, objSiteInfo)
            objCurrentProductInfo.SFMultiSiteList = objCurrentSFSiteInfoList


            'Updates the count status of item in corresponding site except 'Other' site
            If Not strPlannerDesc.Equals("Other") Then
                If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    objCurrentCountListInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                    objCurrentProductInfoList = objCurrentCountListInfoTable.Item(strPlannerDesc)
                End If

                'Update count status if all sites counted for an item in one planner
                For Each objProductInfo In objCurrentProductInfoList
                    If objProductInfo.BootsCode.Equals(strBootsCode) Then
                        objProductInfo.CurrentSiteCount = objProductInfo.CurrentSiteCount + 1
                        If objProductInfo.CurrentSiteCount = objProductInfo.TotalSiteCount Then
                            objProductInfo.CountStatus = "Y"
                            iInfoIndex = iInfoIndex + 1
                        Exit For
                        End If
                    End If
                    iInfoIndex = iInfoIndex + 1
                Next

                'Updates objGlobalCLInfotable
                objCurrentProductInfoList.RemoveAt(iInfoIndex)
                objCurrentProductInfoList.Insert(iInfoIndex, objProductInfo)
                objCurrentCountListInfoTable.Remove(strPlannerDesc)
                objCurrentCountListInfoTable.Add(strPlannerDesc, objCurrentProductInfoList)
                objAppContainer.objGlobalCLInfoTable.Remove(m_CLCurrentProductGroup.ListID)
                objAppContainer.objGlobalCLInfoTable.Add(m_CLCurrentProductGroup.ListID, objCurrentCountListInfoTable)

            End If
            'retrieves the total count of item in all sites for SalesFloor

            If objCurrentSFSiteInfoList.Count() > 0 Then
                Dim iTempSFQty As Integer
                For Each objSiteInfo In objCurrentSFSiteInfoList
                    If objSiteInfo.strSalesFloorQuantiy < 0 Then
                        iTempSFQty = 0
                    Else
                        iTempSFQty = objSiteInfo.strSalesFloorQuantiy
                    End If
                    iTotalSalesFloorQty = iTotalSalesFloorQty + iTempSFQty
                Next
            End If

            'retrieves the total count of item in all sites for BackShop

            If objCurrentBSSiteInfoList.Count() > 0 Then

                Dim iTempBSQty As Integer
                For Each objSiteInfo In objCurrentBSSiteInfoList
                    If objSiteInfo.strBackShopQuantiy < 0 Then
                        iTempBSQty = 0
                    Else
                        iTempBSQty = objSiteInfo.strBackShopQuantiy
                    End If
                    iTotalBackShopQty = iTotalBackShopQty + iTempBSQty
                Next
            End If

            'Update values tosend CLC request
            'iSalesFloorVal = iSalesFloorQty



            'update the sf quantity to be displayed
            objCurrentProductInfo.SalesFloorQuantity = iTotalSalesFloorQty

#If RF Then

            'retrieves the total count of item in all sites for OSSR
            If objCurrentOSSRSiteInfoList.Count() > 0 Then
                Dim iTempOSSRQty As Integer
                For Each objSiteInfo In objCurrentOSSRSiteInfoList
                    If objSiteInfo.strOSSRQuantiy < 0 Then
                        iTempOSSRQty = 0
                    Else
                        iTempOSSRQty = objSiteInfo.strOSSRQuantiy
                    End If
                    iTotalOSSRQty = iTotalOSSRQty + iTempOSSRQty
                Next
            End If
            objCurrentProductInfo.TotalQuantity = iTotalSalesFloorQty + iTotalBackShopQty + iTotalOSSRQty

            'End if
#End If
#If NRF Then
                objCurrentProductInfo.TotalQuantity = iTotalSalesFloorQty + iTotalBackShopQty
#End If
            objCurrentProductInfo.IsSFItemCounted = True

            objCurrentProductList.RemoveAt(iItemIndex)
            objCurrentProductList.Insert(iItemIndex, objCurrentProductInfo)
            objAppContainer.objGlobalCLProductInfoTable.Remove(m_CLCurrentProductGroup.ListID)
            objAppContainer.objGlobalCLProductInfoTable.Add(m_CLCurrentProductGroup.ListID, objCurrentProductList)

            'Checks if the product is already counted in sales floor
            'If not counted then add the item to the Counted Items list
            Dim bIsProductAlreadyCounted As Boolean = False

            For Each objCountedProductData As CLProductCountedData In objAppContainer.m_CLSalesFloorCountedInfoList
                If objCountedProductData.m_strListId.Equals(m_CLCurrentProductGroup.ListID) AndAlso _
                objCountedProductData.m_strProductCode.Equals(objCurrentProductInfo.ProductCode) Then
                    bIsProductAlreadyCounted = True
                    Exit For
                End If
            Next

            'If m_CLSalesFloorProductCount.lblCounted.Visible AndAlso _
            'm_CLSalesFloorProductCount.lblCounted.Text.Equals("COUNTED") Then
            '    bIsProductAlreadyCounted = True
            'End If

            If Not bIsProductAlreadyCounted Then
                objAppContainer.objCLSummary.iSFListCounted += 1
                m_iSalesFloorItemCount = m_iSalesFloorItemCount + 1
                Dim objSalesFloorCountedProduct As CLProductCountedData = New CLProductCountedData()
                objSalesFloorCountedProduct.m_strProductCode = objCurrentProductInfo.ProductCode
                objSalesFloorCountedProduct.m_strListId = m_CLCurrentProductGroup.ListID

                objAppContainer.m_CLSalesFloorCountedInfoList.Add(objSalesFloorCountedProduct)
            End If

            If Not m_CLCurrentProductGroup.IsActive Then
                m_CLCurrentProductGroup.IsActive = True
            End If

#If RF Then

            Dim objCLCRecord As CLCRecord = New CLCRecord()
            'Sets the values
            objCLCRecord.strListID = m_CLCurrentProductGroup.ListID
            objCLCRecord.strNumberSEQ = objCurrentProductInfo.SequenceNumber
            objCLCRecord.strBootscode = objCurrentProductInfo.BootsCode
            objCLCRecord.strCountLocation = Macros.SHOP_FLOOR
            objCLCRecord.strCount = objCurrentProductInfo.SalesFloorQuantity
            objCLCRecord.strUpdateOSSR = " "

            If Not objAppContainer.objExportDataManager.CreateCLC(objCLCRecord) Then
                objAppContainer.objLogger.WriteAppLog("Could not UpdateSalesFloorProductInfo of CLSessionMgr." _
                                             , Logger.LogLevel.RELEASE)
                Return False

            End If
#End If


        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in UpdateSalesFloorProductInfo of CLSessionMgr. Exception is:" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False

        End Try
        objAppContainer.objLogger.WriteAppLog("Exit UpdateSalesFloorProductInfo of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Updates the CLProductInfo object with the change in back shop qty 
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="iBackShopQty"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateBackShopProductInfo(ByVal strProductCode As String, ByVal iBackShopQty As Integer, _
                                                ByVal strBootsCode As String) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered UpdateBackShopProductInfo of CLSessionMgr", Logger.LogLevel.INFO)
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim objCurrentCLSiteInfoTable As Hashtable = New Hashtable()
        Dim objCurrentSFSiteInfoList As ArrayList = New ArrayList()
        Dim objCurrentBSSiteInfoList As ArrayList = New ArrayList()
        Dim objCurrentOSSRSiteInfoList As ArrayList = New ArrayList()
        Dim objCurrentCountListInfoTable As New Hashtable()
        Dim objProductInfo As New CLProductInfo()
        Dim objCurrentProductList As New ArrayList()
        Dim objCurrentProductInfo As New CLProductInfo()
        Dim objSiteInfo As New CLMultiSiteInfo()

        Dim strPlannerDesc As String = Nothing
        Dim iItemIndex As Integer = -1
        Dim iSiteIndex As Integer = -1
        Dim iInfoIndex As Integer = -1
        Dim iSalesFloorVal As Integer = -1
        Dim iBackShopVal As Integer = -1
        Dim iOSSRVal As Integer = -1
        Dim iTotalSalesFloorQty As Integer = 0
        Dim iTotalBackShopQty As Integer = 0
        Dim iTotalOSSRQty As Integer = 0
        Try

            'Updates the counted item in site with "Y"
            If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                objCurrentProductList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                For Each objCurrentProductInfo In objCurrentProductList
                    iItemIndex = iItemIndex + 1
                    If objCurrentProductInfo.BootsCode.Equals(strBootsCode) Then
                        objCurrentSFSiteInfoList = objCurrentProductInfo.SFMultiSiteList
#If NRF Then
                        objCurrentBSSiteInfoList = objCurrentProductInfo.BSMultiSiteList
#ElseIf RF Then
                        If objAppContainer.OSSRStoreFlag = "Y" Then
                            If objCurrentProductInfo.OSSRFlag = "O" Then
                                objCurrentOSSRSiteInfoList = objCurrentProductInfo.OSSRMultiSiteList
                            Else
                                objCurrentBSSiteInfoList = objCurrentProductInfo.BSMultiSiteList
                            End If
                        Else
                            objCurrentBSSiteInfoList = objCurrentProductInfo.BSMultiSiteList
                        End If
#End If
                        Exit For
                    End If
                Next
            End If

            For Each objSiteInfo In objCurrentBSSiteInfoList
                If objSiteInfo.strSeqNumber.Equals(m_SelectedPOGSeqNum) Then
                    If objSiteInfo.IsCounted.Equals("N") Then
                        objSiteInfo.strPOGDescription = "* " & objSiteInfo.strPOGDescription.Trim & " (Counted)"
                        objSiteInfo.IsCounted = "Y"
                        objCurrentProductInfo.TotalBSSiteCount = objCurrentProductInfo.TotalBSSiteCount + 1
                    End If
                    objSiteInfo.strBackShopQuantiy = iBackShopQty
                    strPlannerDesc = objSiteInfo.strPlannerDesc
                    'Updating BS - MBS and  PSP quantity separately 
                    If m_SelectedPOGSeqNum = Macros.BS_SELECTED_INDEX Then
                        objCurrentProductInfo.BackShopMBSQuantity = iBackShopQty
                        objCurrentProductInfo.IsMBSItemCounted = True
                    Else
                        objCurrentProductInfo.BackShopPSPQuantity = iBackShopQty
                        objCurrentProductInfo.IsBSPSPItemCounted = True
                    End If
                    iSiteIndex = iSiteIndex + 1
                    Exit For
                End If
                iSiteIndex = iSiteIndex + 1
            Next

            'Updates CLSFMultiSiteDetails with new count
            objCurrentBSSiteInfoList.RemoveAt(iSiteIndex)
            objCurrentBSSiteInfoList.Insert(iSiteIndex, objSiteInfo)
            objCurrentProductInfo.BSMultiSiteList = objCurrentBSSiteInfoList

            If m_CLCurrentSiteInfo.strPlannerDesc.Equals(Macros.COUNT_BS_UNKNOWN) Then
                strPlannerDesc = Macros.COUNT_BS_UNKNOWN
            End If

            'Updates the count status of item in corresponding site
            If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                objCurrentCountListInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                objCurrentProductInfoList = objCurrentCountListInfoTable.Item(strPlannerDesc)
            End If

            For Each objProductInfo In objCurrentProductInfoList
                If objProductInfo.BootsCode.Equals(strBootsCode) Then
                    objProductInfo.CountStatus = "Y"
                    iInfoIndex = iInfoIndex + 1
                    Exit For
                End If
                iInfoIndex = iInfoIndex + 1
            Next

            ''''''
            'Updates objGlobalCLInfotable
            objCurrentProductInfoList.RemoveAt(iInfoIndex)
            objCurrentProductInfoList.Insert(iInfoIndex, objProductInfo)
            objCurrentCountListInfoTable.Remove(strPlannerDesc)
            objCurrentCountListInfoTable.Add(strPlannerDesc, objCurrentProductInfoList)
            objAppContainer.objGlobalCLInfoTable.Remove(m_CLCurrentProductGroup.ListID)
            objAppContainer.objGlobalCLInfoTable.Add(m_CLCurrentProductGroup.ListID, objCurrentCountListInfoTable)

            ''''''
            'Retrieve sales floor quantity
            If objCurrentProductInfo.SalesFloorQuantity < 0 Then
                iTotalSalesFloorQty = 0
            Else
                iTotalSalesFloorQty = objCurrentProductInfo.SalesFloorQuantity
            End If


            'retrieves the total count of item in all sites for BackShop

            If objCurrentBSSiteInfoList.Count() > 0 Then

                Dim iTempBSQty As Integer
                For Each objSiteInfo In objCurrentBSSiteInfoList
                    If objSiteInfo.strBackShopQuantiy < 0 Then
                        iTempBSQty = 0
                    Else
                        iTempBSQty = objSiteInfo.strBackShopQuantiy
                    End If
                    iTotalBackShopQty = iTotalBackShopQty + iTempBSQty
                Next
            End If

            'update the bs quantity to be displayed
            objCurrentProductInfo.BackShopQuantity = iTotalBackShopQty

#If RF Then

            'retrieves the total count of item in all sites for OSSR
            If objCurrentOSSRSiteInfoList.Count() > 0 Then
                Dim iTempOSSRQty As Integer
                For Each objSiteInfo In objCurrentOSSRSiteInfoList
                    If objSiteInfo.strOSSRQuantiy < 0 Then
                        iTempOSSRQty = 0
                    Else
                        iTempOSSRQty = objSiteInfo.strOSSRQuantiy
                    End If
                    iTotalOSSRQty = iTotalOSSRQty + iTempOSSRQty
                Next
            End If
            objCurrentProductInfo.TotalQuantity = iTotalSalesFloorQty + iTotalBackShopQty + iTotalOSSRQty

            'End if
#End If
#If NRF Then
            objCurrentProductInfo.TotalQuantity = iTotalSalesFloorQty + iTotalBackShopQty
#End If


            objCurrentProductList.RemoveAt(iItemIndex)
            objCurrentProductList.Insert(iItemIndex, objCurrentProductInfo)
            objAppContainer.objGlobalCLProductInfoTable.Remove(m_CLCurrentProductGroup.ListID)
            objAppContainer.objGlobalCLProductInfoTable.Add(m_CLCurrentProductGroup.ListID, objCurrentProductList)


            'Checks if the product is already counted in sales floor
            'If not counted then add the item to the Counted Items list
            Dim bIsProductAlreadyCounted As Boolean = False

            For Each objCountedProductData As CLProductCountedData In objAppContainer.m_CLBackShopCountedInfoList
                If objCountedProductData.m_strListId.Equals(m_CLCurrentProductGroup.ListID) AndAlso _
                objCountedProductData.m_strProductCode.Equals(objCurrentProductInfo.ProductCode) Then
                    bIsProductAlreadyCounted = True
                    Exit For
                End If
            Next

            'If m_CLSalesFloorProductCount.lblCounted.Visible AndAlso _
            'm_CLSalesFloorProductCount.lblCounted.Text.Equals("COUNTED") Then
            '    bIsProductAlreadyCounted = True
            'End If

            If Not bIsProductAlreadyCounted Then
                objAppContainer.objCLSummary.iBSListCounted += 1
                m_iBackShopItemCount = m_iBackShopItemCount + 1
                Dim objBackShopCountedProduct As CLProductCountedData = New CLProductCountedData()
                objBackShopCountedProduct.m_strProductCode = objCurrentProductInfo.ProductCode
                objBackShopCountedProduct.m_strListId = m_CLCurrentProductGroup.ListID

                objAppContainer.m_CLBackShopCountedInfoList.Add(objBackShopCountedProduct)
            End If

            If Not m_CLCurrentProductGroup.IsActive Then
                m_CLCurrentProductGroup.IsActive = True
            End If

#If RF Then
            'For Each objProductInfo As CLProductInfo In objCurrentProductInfoList
            '    Dim objCLCRecord As CLCRecord = New CLCRecord()
            '    'Sets the values
            '    objCLCRecord.strListID = m_CLCurrentProductGroup.ListID
            '    objCLCRecord.strNumberSEQ = objProductInfo.SequenceNumber
            '    objCLCRecord.strBootscode = objProductInfo.BootsCode
            '    objCLCRecord.strBackShopCount = objProductInfo.BackShopQuantity
            '    objCLCRecord.strSalesFloorCount = objProductInfo.SalesFloorQuantity
            '    objCLCRecord.strOssrCount = objProductInfo.OSSRQuantity
            '    objCLCRecord.strUpdateOssrItem = " "
            '    If Not objAppContainer.objExportDataManager.CreateCLC(objCLCRecord) Then
            '        objAppContainer.objLogger.WriteAppLog("Could not UpdateSalesFloorProductInfo of CLSessionMgr." _
            '                                     , Logger.LogLevel.RELEASE)
            '        Return False

            '    End If
            'Next

            Dim objCLCRecord As CLCRecord = New CLCRecord()
            'Sets the values
            objCLCRecord.strListID = m_CLCurrentProductGroup.ListID
            objCLCRecord.strNumberSEQ = objCurrentProductInfo.SequenceNumber
            objCLCRecord.strBootscode = objCurrentProductInfo.BootsCode
            If m_SelectedPOGSeqNum = Macros.BS_SELECTED_INDEX Then
                objCLCRecord.strCountLocation = Macros.BACK_SHOP
                objCLCRecord.strCount = objCurrentProductInfo.BackShopMBSQuantity
            Else
                objCLCRecord.strCountLocation = Macros.PSP
                objCLCRecord.strCount = objCurrentProductInfo.BackShopPSPQuantity
            End If
            objCLCRecord.strUpdateOSSR = " "


            If Not objAppContainer.objExportDataManager.CreateCLC(objCLCRecord) Then
                objAppContainer.objLogger.WriteAppLog("Could not UpdateBackShopProductInfo of CLSessionMgr." _
                                             , Logger.LogLevel.RELEASE)
                Return False

            End If

#End If


        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in UpdateBackShopProductInfo of CLSessionMgr. Exception is:" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False

        End Try
        objAppContainer.objLogger.WriteAppLog("Exit UpdateBackShopProductInfo of CLSessionMgr", Logger.LogLevel.INFO)
        Return True

    End Function
    ''' <summary>
    ''' Processes the next button click on the Item Details screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessItemDetailsNext() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ProcessItemDetailsNext of CLSessionMgr", Logger.LogLevel.INFO)
        'Added as part of SFA - On click of Next display the Item scan screen for COL
        If m_bIsCreateOwnList And m_bIsNewList Then
            ProcessCOLItemDetailsNext()
        Else
            m_iProductListCount = m_iProductListCount + 1
            If ValidateNextAndback() Then
                m_EntryType = BCType.None
                If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                    DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.SELECTED_INDEX)
                Else
                    DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.BS_SELECTED_INDEX)
                End If
                'Support: Full Price Check Removed
                'If full price check is required for the previous item then disable to check.
                'm_bIsFullPriceCheckRequired = False
                m_strSEL = ""
            Else
                Return False
            End If
        End If
        objAppContainer.objLogger.WriteAppLog("Exit ProcessItemDetailsNext of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the Quit button click on the Item Details and counting screens
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessItemDetailsQuit() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ProcessItemDetailsQuit of CLSessionMgr", Logger.LogLevel.INFO)
        Dim iResult As Integer = 0
        If Not m_bIsCreateOwnList Then
            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M40"), "Confirmation", _
                                      MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                                      MessageBoxDefaultButton.Button1)
            'SFA SIT DEF #644
        Else
            iResult = 6

            '    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M104"), "Confirmation", _
            '                              MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
            '                              MessageBoxDefaultButton.Button1)
        End If
        If iResult = MsgBoxResult.Yes Then
            If m_bIsCreateOwnList Then
                If m_bIsNewList Then
                    If m_bNavigation Then
                        DisplayCLScreen(CLSCREENS.COLLocationSelection)
                    Else
                        'If m_bPlannerFlag Then
                        '    If Not m_CLItemList.Count - 1 = m_iBackNextCount Then
                        '        m_iBackNextCount = m_CLItemList.Count - 2
                        '    End If
                        'Else
                        If Not m_CLItemList.Count = m_iBackNextCount - 1 Then
                            m_iBackNextCount = m_CLItemList.Count - 1
                        End If
                        'End If
                        DisplayCLScreen(CLSCREENS.COLItemScan)
                    End If
                ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                    'SFA - DEF 778
                    If m_CLCurrentProductGroup.BSItemCount > 0 And (m_CLCurrentProductGroup.UnknownItemCount > 0 Or _
                    m_bIsItemsInPSP) Then
                        DisplayCLScreen(CLSCREENS.SiteInformation, m_iCountedLocation)
                    Else
                        DisplayCLScreen(CLSCREENS.COLLocationSelection)
                    End If
                ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                    If m_bISItemsInOSSRPSP Then
                        DisplayCLScreen(CLSCREENS.SiteInformation, m_iCountedLocation)
                    Else
                        DisplayCLScreen(CLSCREENS.COLLocationSelection)
                    End If
                ElseIf m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                    DisplayCLScreen(CLSCREENS.SiteInformation, m_iCountedLocation)
                End If
            Else
                If m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                    'SFA - DEF 778
                    If m_CLCurrentProductGroup.BSItemCount > 0 And (m_CLCurrentProductGroup.UnknownItemCount > 0 Or _
                    m_CLCurrentProductGroup.BackShopPSPCount > 0) Then
                        'changed to site info screen
                        DisplayCLScreen(CLSCREENS.SiteInformation, m_iCountedLocation)
                    Else
                        DisplayCLScreen(CLSCREENS.LocationSelection, m_iCountedLocation)
                    End If
                ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                    If m_CLCurrentProductGroup.OSSRPSPCount <> 0 Then

                        'changed to site info screen
                        DisplayCLScreen(CLSCREENS.SiteInformation, m_iCountedLocation)
                    Else
                        DisplayCLScreen(CLSCREENS.LocationSelection, m_iCountedLocation)
                    End If
                ElseIf m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                    DisplayCLScreen(CLSCREENS.SiteInformation, m_iCountedLocation)

                End If
            End If
            'Support: Full Price Check Removed
            'If full price check is required for the previous item then disable to check.
            'm_bIsFullPriceCheckRequired = False
            m_strSEL = ""
        Else
            'Remain in same screen
            DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, m_SelectedPOGSeqNum)
            Return False
        End If

        objAppContainer.objLogger.WriteAppLog("Exit ProcessItemDetailsQuit of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the next button click on the Product Count screen
    ''' </summary>
    ''' <param name="iSender"></param>
    ''' <remarks></remarks>
    Public Function ProcessProductCountNext(ByVal iSender As Integer, ByVal strPrdCode As String, _
                        ByVal strBootsCode As String, ByVal iShelfQuantity As Integer) As Boolean

        objAppContainer.objLogger.WriteAppLog("Entered ProcessProductCountNext of CLSessionMgr", Logger.LogLevel.INFO)

        Dim strUnFormatBootsCode As String
        Dim objCurrentProductList As New ArrayList()
        Dim objCurrentSFSiteInfoList As New ArrayList()
        Dim iCurrentQuantity As String = Nothing
        Dim objCurrentProductInfo As New CLProductInfo()
        Dim objCurrentBSSiteInfoList As New ArrayList()
        Dim objCurrentOSSRSiteInfoList As New ArrayList()
        Dim objSiteInfo As New CLMultiSiteInfo()

        If m_bIsCreateOwnList And m_bIsNewList Then
            ProcessCOLProductCountNext(strPrdCode, strBootsCode, iShelfQuantity)
        Else

            'To unformat the product code by removing "-" and then remove CDV from that value
            Dim strProductCode As String = ""
            strProductCode = objAppContainer.objHelper.UnFormatBarcode(strPrdCode)
            strProductCode = strProductCode.Remove(strProductCode.Length - 1, 1)
            strUnFormatBootsCode = objAppContainer.objHelper.UnFormatBarcode(strBootsCode)

            '&&&&&&&&&&&&&&&&&&
            If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                objCurrentProductList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                For Each objCurrentProductInfo In objCurrentProductList
                    If objCurrentProductInfo.BootsCode.Equals(strUnFormatBootsCode) Then
                        objCurrentSFSiteInfoList = objCurrentProductInfo.SFMultiSiteList
#If NRF Then
                        objCurrentBSSiteInfoList = objCurrentProductInfo.BSMultiSiteList
#ElseIf RF Then
                        If objAppContainer.OSSRStoreFlag = "Y" Then
                            If objCurrentProductInfo.OSSRFlag = "O" Then
                                objCurrentOSSRSiteInfoList = objCurrentProductInfo.OSSRMultiSiteList
                            Else
                                objCurrentBSSiteInfoList = objCurrentProductInfo.BSMultiSiteList
                            End If
                        Else
                            objCurrentBSSiteInfoList = objCurrentProductInfo.BSMultiSiteList
                        End If
#End If
                        Exit For
                    End If
                Next
            End If

            '&&&&&&&&&&&&&&&&&&
            If m_EntryType = BCType.EAN Then
                If iShelfQuantity = 0 Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M99"), "Info", _
                                                   MessageBoxButtons.OK, _
                                                   MessageBoxIcon.Asterisk, _
                                                   MessageBoxDefaultButton.Button1)
                    objAppContainer.objLogger.WriteAppLog(MessageManager.GetInstance().GetMessage("M99"), Logger.LogLevel.INFO)
                Else
                    m_iProductListCount = m_iProductListCount + 1
                    If ValidateNextAndback(iSender) Then
                        m_EntryType = BCType.None
                        DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.SELECTED_INDEX)
                    End If
                End If
            Else
                If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                    'If item key entered and quantity not entered, a count of zero assumed
                    'Move to next item if present
                    If iShelfQuantity = 0 Then
                        '##############
                        For Each objSiteInfo In objCurrentSFSiteInfoList
                            If objSiteInfo.strSeqNumber.Equals(m_SelectedPOGSeqNum) Then
                                iCurrentQuantity = objSiteInfo.strSalesFloorQuantiy
                                Exit For
                            End If
                        Next

                        If iCurrentQuantity.Equals("-1") Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M102"), "Info", _
                                               MessageBoxButtons.OK, _
                                               MessageBoxIcon.Asterisk, _
                                               MessageBoxDefaultButton.Button1)
                            objAppContainer.objLogger.WriteAppLog(MessageManager.GetInstance().GetMessage("M102"), Logger.LogLevel.INFO)
                        End If
                        '##############

                        'Updates the list with modified data
                        If UpdateSalesFloorProductInfo(strProductCode, iShelfQuantity, strUnFormatBootsCode) Then
                            m_iProductListCount = m_iProductListCount + 1
                            If ValidateNextAndback(iSender) Then
                                m_EntryType = BCType.None
                                DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.SELECTED_INDEX)
                            Else
                                DisplayCLScreen(CLSCREENS.SalesFloorProductCount)
                            End If
                        End If
                    Else
                        'If a valid quantity is enterd, move to next item on pressing next button
                        m_iProductListCount = m_iProductListCount + 1
                        If ValidateNextAndback(iSender) Then
                            m_EntryType = BCType.None
                            DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.SELECTED_INDEX)
                        Else
                            DisplayCLScreen(CLSCREENS.SalesFloorProductCount)
                        End If
                    End If
                ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                    If iShelfQuantity = 0 Then
                        '##############

                        For Each objSiteInfo In objCurrentBSSiteInfoList
                            If objSiteInfo.strSeqNumber.Equals(m_SelectedPOGSeqNum) Then
                                iCurrentQuantity = objSiteInfo.strBackShopQuantiy
                                Exit For
                            End If
                        Next

                        If iCurrentQuantity.Equals("-1") Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M102"), "Info", _
                                               MessageBoxButtons.OK, _
                                               MessageBoxIcon.Asterisk, _
                                               MessageBoxDefaultButton.Button1)
                            objAppContainer.objLogger.WriteAppLog(MessageManager.GetInstance().GetMessage("M102"), Logger.LogLevel.INFO)
                        End If
                        '##############

                        'Updates the list with modified data
                        If UpdateBackShopProductInfo(strProductCode, iShelfQuantity, strUnFormatBootsCode) Then
                            m_iProductListCount = m_iProductListCount + 1
                            If ValidateNextAndback(iSender) Then
                                m_EntryType = BCType.None
                                DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.BS_SELECTED_INDEX)
                            Else
                                DisplayCLScreen(CLSCREENS.BackShopProductCount)
                            End If
                        End If
                    Else
                        'If a valid quantity is enterd, move to next item on pressing next button
                        m_iProductListCount = m_iProductListCount + 1
                        If ValidateNextAndback(iSender) Then
                            m_EntryType = BCType.None
                            DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.BS_SELECTED_INDEX)
                        Else
                            DisplayCLScreen(CLSCREENS.BackShopProductCount)
                        End If
                    End If

                    'ambli
                    'For OSSR
                ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
#If RF Then
                    'Updates the list with modified data

                    If iShelfQuantity = 0 Then
                        '##############
                        For Each objSiteInfo  In objCurrentOSSRSiteInfoList
                            If objSiteInfo.strSeqNumber.Equals(m_SelectedPOGSeqNum) Then
                                iCurrentQuantity = objSiteInfo.strOSSRQuantiy
                                Exit For
                            End If
                        Next

                        If iCurrentQuantity.Equals("-1") Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M102"), "Info", _
                                               MessageBoxButtons.OK, _
                                               MessageBoxIcon.Asterisk, _
                                               MessageBoxDefaultButton.Button1)
                            objAppContainer.objLogger.WriteAppLog(MessageManager.GetInstance().GetMessage("M102"), Logger.LogLevel.INFO)
                        End If
                        '##############
                        'Updates the list with modified data
                        If UpdateOSSRProductInfo(strProductCode, iShelfQuantity, strUnFormatBootsCode) Then
                            m_iProductListCount = m_iProductListCount + 1
                            If ValidateNextAndback(iSender) Then
                                m_EntryType = BCType.None
                                DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.BS_SELECTED_INDEX)
                            Else
                                DisplayCLScreen(CLSCREENS.OSSRProductCount)
                            End If
                        End If
                    Else
                        'If a valid quantity is enterd, move to next item on pressing next button
                        m_iProductListCount = m_iProductListCount + 1
                        If ValidateNextAndback(iSender) Then
                            m_EntryType = BCType.None
                            DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.BS_SELECTED_INDEX)
                        Else
                            DisplayCLScreen(CLSCREENS.OSSRProductCount)
                        End If
                    End If

#End If
                End If
            End If


        End If
        objAppContainer.objLogger.WriteAppLog("Entered ProcessProductCountNext of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the back button click on the item details screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessItemDetailsBack() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ProcessItemDetailsBack of CLSessionMgr", Logger.LogLevel.INFO)
        If m_bIsCreateOwnList And m_bIsNewList Then
            ProcessCOLBack()
        Else
        m_iProductListCount = m_iProductListCount - 1
            If ValidateNextAndback() Then
                m_EntryType = BCType.None
                If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                    DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.SELECTED_INDEX)
                Else
                    DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.BS_SELECTED_INDEX)
                End If
                'Support: Full Price Check Removed
                'If full price check is required for the previous item then disable to check.
                'm_bIsFullPriceCheckRequired = False
                m_strSEL = ""
            Else
                Return False
            End If
        End If
        objAppContainer.objLogger.WriteAppLog("Exit ProcessItemDetailsBack of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the back button click on the Product Count screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessProductCountBack() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ProcessProductCountBack of CLSessionMgr", Logger.LogLevel.INFO)
        If m_bIsCreateOwnList And m_bIsNewList Then
            ProcessCOLBack()
        Else
            m_iProductListCount = m_iProductListCount - 1

            If ValidateNextAndback() Then
                m_EntryType = BCType.None
                If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                    DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.SELECTED_INDEX)
                Else
                    DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.BS_SELECTED_INDEX)
                End If
            Else
                Return False
            End If
        End If
        objAppContainer.objLogger.WriteAppLog("Exit ProcessProductCountBack of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the selection of Zero button
    ''' </summary>
    ''' <param name="strPrdCode"></param>
    ''' <remarks></remarks>
    Public Function ProcessZeroSelection(ByVal strPrdCode As String, ByVal strBootsCode As String, _
                                          Optional ByVal strLocation As Integer = 0) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ProcessZeroSelection of CLSessionMgr", Logger.LogLevel.INFO)
        Try

            Dim iQuantity As Integer = 0
            Dim strUnFormatBootsCode As String
            Dim iResult As Integer
            Dim arrTempMultiSiteList As New ArrayList()
            Dim clMultiSiteData As CLMultiSiteInfo
            Dim arrMultiSiteList As New ArrayList()
            Dim iCount As Integer
            Dim objProductInfoTable As New Hashtable()
            Dim iBSQty As Integer
            Dim iBSPSPQty As Integer
            Dim iOSSR_BSQty As Integer
            Dim iOSSR_PSPQty As Integer
            Dim objItemList As ArrayList
            Dim arrProductList As New ArrayList()
            Dim objCurrentProductInfo As New CLProductInfo()
            Dim strCurrentSite As String = Nothing
            'To unformat the product code by removing "-" and then remove CDV from that value
            Dim strProductCode As String = ""
            strProductCode = objAppContainer.objHelper.UnFormatBarcode(strPrdCode)
            strProductCode = strProductCode.Remove(strProductCode.Length - 1, 1)
            strUnFormatBootsCode = objAppContainer.objHelper.UnFormatBarcode(strBootsCode)
            m_bIsFullPriceCheckRequired = False
            If m_EntryType = BCType.EAN Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M99"), "Info", _
                                               MessageBoxButtons.OK, _
                                               MessageBoxIcon.Asterisk, _
                                               MessageBoxDefaultButton.Button1)
                objAppContainer.objLogger.WriteAppLog(MessageManager.GetInstance().GetMessage("M99"), Logger.LogLevel.INFO)

            Else
                If m_bIsCreateOwnList Then
                    If strLocation = Macros.COUNT_LIST_FULLPRICECHECK And (m_CLProductInfo.MultiSiteCount > 1 _
                                        And bIsItemScan) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M76"), "Info", _
                                              MessageBoxButtons.OK, _
                                              MessageBoxIcon.Asterisk, _
                                              MessageBoxDefaultButton.Button1)
                        objAppContainer.objLogger.WriteAppLog(MessageManager.GetInstance().GetMessage("M76"))
                        DisplayCLScreen(CLSCREENS.SalesFloorProductCount, m_iCountedLocation, m_SelectedPOGSeqNum)
                        bIsItemScan = False
                        Return True
                    Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M100"), "Info", _
                                                  MessageBoxButtons.OK, _
                                                  MessageBoxIcon.Asterisk, _
                                                  MessageBoxDefaultButton.Button1)
                        objAppContainer.objLogger.WriteAppLog(MessageManager.GetInstance().GetMessage("M100"), Logger.LogLevel.INFO)
                    End If
                Else
                    If m_iCountedLocation = Macros.COUNT_SALES_FLOOR And m_CLCurrentItemInfo.IsSFDifferentSession And strLocation = Macros.COUNT_LIST_ITEMDETAILS Then
                        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M107"), "Confirmation", _
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                                   MessageBoxDefaultButton.Button1)
                    Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M100"), "Info", _
                                                  MessageBoxButtons.OK, _
                                                  MessageBoxIcon.Asterisk, _
                                                  MessageBoxDefaultButton.Button1)
                        objAppContainer.objLogger.WriteAppLog(MessageManager.GetInstance().GetMessage("M100"), Logger.LogLevel.INFO)
                    End If
                End If


                If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                    'Updates the list with modified data
                    If m_bIsCreateOwnList Then
                        If UpdateCOLProductInfo(strProductCode, iQuantity, 0, 0, strUnFormatBootsCode) Then
                            If strLocation = Macros.COUNT_LIST_ITEMDETAILS Then
                                DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, m_SelectedPOGSeqNum)
                            ElseIf strLocation = Macros.COUNT_LIST_FULLPRICECHECK Then
                                ' 1.1 Fix from Service team for Invalid CLD Message Length  Issue in POD Starts
                                If m_CLProductInfo.CreatedLocation <> Macros.BACK_SHOP Then
                                    objCurrentProductInfo = m_CLProductInfo
                                    m_CLProductInfo.CreatedLocation = Macros.SHOP_FLOOR
                                End If
                                ' 1.1 Fix from Service team for Invalid CLD  Message Length Issue in POD Ends
                                If m_CLProductInfo.MultiSiteCount > 1 Then
                                    DisplayCLScreen(CLSCREENS.SalesFloorProductCount, m_iCountedLocation, m_SelectedPOGSeqNum)
                                Else
                                    DisplayCLScreen(CLSCREENS.COLItemScan)
                                End If
                            Else
                                DisplayCLScreen(CLSCREENS.SalesFloorProductCount, m_iCountedLocation, m_SelectedPOGSeqNum)
                            End If
                        End If
                    Else   'ee
                        '''''''''''''''''''

                        If m_CLCurrentItemInfo.IsSFDifferentSession And strLocation = Macros.COUNT_LIST_ITEMDETAILS Then
                            'iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M107"), "Confirmation", _
                            '    MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                            '    MessageBoxDefaultButton.Button1)
                            If iResult = MsgBoxResult.Yes Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M100"), "Info", _
                                             MessageBoxButtons.OK, _
                                             MessageBoxIcon.Asterisk, _
                                             MessageBoxDefaultButton.Button1)
                                arrTempMultiSiteList = m_CLCurrentItemInfo.SFMultiSiteList
                                iCount = 0
                                For Each PlannerInfo As CLMultiSiteInfo In arrTempMultiSiteList
                                    If iCount = m_SelectedPOGSeqNum Then
                                        strCurrentSite = PlannerInfo.strPlannerDesc
                                        clMultiSiteData = New CLMultiSiteInfo
                                        clMultiSiteData.strSeqNumber = PlannerInfo.strSeqNumber
                                        clMultiSiteData.strSalesFloorQuantiy = 0
                                        clMultiSiteData.strPlannerDesc = PlannerInfo.strPlannerDesc
                                        clMultiSiteData.strPlannerID = PlannerInfo.strPlannerID
                                        If PlannerInfo.strPlannerDesc.Equals("Other") Then
                                            clMultiSiteData.strPOGDescription = "* Other (Counted)"
                                        Else
                                            clMultiSiteData.strPOGDescription = PlannerInfo.strPOGDescription
                                        End If
                                        clMultiSiteData.IsCounted = "Y"
                                    Else
                                        clMultiSiteData = New CLMultiSiteInfo
                                        clMultiSiteData.strSeqNumber = PlannerInfo.strSeqNumber
                                        clMultiSiteData.strSalesFloorQuantiy = -1
                                        clMultiSiteData.strPlannerDesc = PlannerInfo.strPlannerDesc
                                        clMultiSiteData.strPlannerID = PlannerInfo.strPlannerID
                                        If PlannerInfo.strPlannerDesc.Equals("Other") Then
                                            clMultiSiteData.strPOGDescription = "Other"
                                            clMultiSiteData.IsCounted = "Y"
                                        Else
                                            clMultiSiteData.strPOGDescription = PlannerInfo.strPlannerDesc + " - " + PlannerInfo.strModuleDesc
                                            clMultiSiteData.IsCounted = "N"
                                        End If
                                    End If
                                    arrMultiSiteList.Add(clMultiSiteData)
                                    iCount = iCount + 1
                                Next
                                'Reset the multisitelist for an item
                                m_CLCurrentItemInfo.SFMultiSiteList = arrMultiSiteList

#If RF Then
                                If m_CLCurrentItemInfo.OSSRFlag = "O" Then
                                    If m_CLCurrentItemInfo.OSSRMBSQuantity < 0 Then
                                        iOSSR_BSQty = 0
                                    Else
                                        iOSSR_BSQty = m_CLCurrentItemInfo.OSSRMBSQuantity
                                    End If
                                    If m_CLCurrentItemInfo.OSSRPSPQuantity < 0 Then
                                        iOSSR_PSPQty = 0
                                    Else
                                        iOSSR_PSPQty = m_CLCurrentItemInfo.OSSRPSPQuantity
                                    End If
                                Else
                                    If m_CLCurrentItemInfo.BackShopMBSQuantity < 0 Then
                                        iBSQty = 0
                                    Else
                                        iBSQty = m_CLCurrentItemInfo.BackShopMBSQuantity
                                    End If
                                    If m_CLCurrentItemInfo.BackShopPSPQuantity < 0 Then
                                        iBSPSPQty = 0
                                    Else
                                        iBSPSPQty = m_CLCurrentItemInfo.BackShopPSPQuantity
                                    End If
                                End If
#ElseIf NRF Then
                                If m_CLCurrentItemInfo.BackShopMBSQuantity < 0 Then
                                    iBSQty = 0
                                Else
                                    iBSQty = m_CLCurrentItemInfo.BackShopMBSQuantity
                                End If
                                If m_CLCurrentItemInfo.BackShopPSPQuantity < 0 Then
                                    iBSPSPQty = 0
                                Else
                                    iBSPSPQty = m_CLCurrentItemInfo.BackShopPSPQuantity
                                End If
#End If



                                ''''''''''
#If RF Then
                                If objAppContainer.OSSRStoreFlag = "Y" Then
                                    If m_CLCurrentItemInfo.OSSRFlag = "O" Then
                                        If m_CLCurrentItemInfo.BackShopQuantity < 0 And m_CLCurrentItemInfo.BackShopPSPQuantity < 0 And _
                                                                       m_CLCurrentItemInfo.OSSRQuantity < 0 And m_CLCurrentItemInfo.OSSRPSPQuantity < 0 Then
                                            m_CLCurrentItemInfo.TotalQuantity = m_CLCurrentItemInfo.BackShopQuantity + m_CLCurrentItemInfo.BackShopPSPQuantity + _
                                                             m_CLCurrentItemInfo.OSSRQuantity + m_CLCurrentItemInfo.OSSRPSPQuantity
                                        Else
                                            m_CLCurrentItemInfo.TotalQuantity = iBSQty + iBSPSPQty + iOSSR_BSQty + iOSSR_PSPQty
                                        End If
                                    Else
                                        If m_CLCurrentItemInfo.BackShopQuantity < 0 And m_CLCurrentItemInfo.BackShopPSPQuantity < 0 Then
                                            m_CLCurrentItemInfo.TotalQuantity = m_CLCurrentItemInfo.BackShopQuantity + m_CLCurrentItemInfo.BackShopPSPQuantity
                                        Else
                                            m_CLCurrentItemInfo.TotalQuantity = iBSQty + iBSPSPQty
                                        End If
                                    End If
                                Else
                                    If m_CLCurrentItemInfo.BackShopQuantity < 0 And m_CLCurrentItemInfo.BackShopPSPQuantity < 0 Then
                                        m_CLCurrentItemInfo.TotalQuantity = m_CLCurrentItemInfo.BackShopQuantity + m_CLCurrentItemInfo.BackShopPSPQuantity
                                    Else
                                        m_CLCurrentItemInfo.TotalQuantity = iBSQty + iBSPSPQty
                                    End If
                                End If
#ElseIf NRF Then
                                If m_CLCurrentItemInfo.BackShopQuantity < 0 And m_CLCurrentItemInfo.BackShopPSPQuantity < 0 Then
                                    m_CLCurrentItemInfo.TotalQuantity = m_CLCurrentItemInfo.BackShopQuantity + m_CLCurrentItemInfo.BackShopPSPQuantity
                                Else
                                    m_CLCurrentItemInfo.TotalQuantity = iBSQty + iBSPSPQty
                                End If
#End If

                                m_CLCurrentItemInfo.SalesFloorQuantity = 0
#If RF Then
                                Dim objCLCRecord As CLCRecord = New CLCRecord()
                                'Sets the values
                                objCLCRecord.strListID = m_CLCurrentProductGroup.ListID
                                objCLCRecord.strNumberSEQ = m_CLCurrentItemInfo.SequenceNumber
                                objCLCRecord.strBootscode = m_CLCurrentItemInfo.BootsCode
                                objCLCRecord.strCountLocation = Macros.SHOP_FLOOR
                                objCLCRecord.strCount = m_CLCurrentItemInfo.SalesFloorQuantity
                                objCLCRecord.strUpdateOSSR = " "

                                If Not objAppContainer.objExportDataManager.CreateCLC(objCLCRecord) Then
                                    objAppContainer.objLogger.WriteAppLog("Could not UpdateSalesFloorProductInfo of CLSessionMgr." _
                                         , Logger.LogLevel.RELEASE)
                                    Return False

                                End If
#End If

                                'Reset the site level count status for those items
                                For Each strItem As String In m_CLCurrentItemInfo.DistinctPOGList
                                    'If Not strItem.Equals(strCurrentSite) Then
                                    objItemList = New ArrayList()
                                    If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                                        objProductInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                                        objItemList = objProductInfoTable.Item(strItem)
                                        For Each objItemInfo As CLProductInfo In objItemList
                                            If objItemInfo.BootsCode = m_CLCurrentItemInfo.BootsCode Then
                                                objItemInfo.CountStatus = "N"
                                            End If
                                        Next
                                    End If
                                    'End If
                                Next
                                '''''''''''''''''''
                                'Updates the count status of item in corresponding site except 'Other' site
                                If Not strCurrentSite.Equals("Other") Then
                                    Dim objCurrentCountInfoTable As New Hashtable()
                                    Dim objCurrentProductList As New ArrayList()
                                    If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                                        objCurrentCountInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                                        objCurrentProductList = objCurrentCountInfoTable.Item(strCurrentSite)
                                    End If

                                    'Update count status if all sites counted for an item in one planner
                                    For Each objProduct As CLProductInfo In objCurrentProductList
                                        If objProduct.BootsCode.Equals(m_CLCurrentItemInfo.BootsCode) Then
                                            objProduct.CurrentSiteCount = objProduct.CurrentSiteCount + 1
                                            If objProduct.CurrentSiteCount = objProduct.TotalSiteCount Then
                                                objProduct.CountStatus = "Y"
                                                Exit For
                                            End If
                                        End If
                                    Next
                                End If
                                '''''''''''''''''''

                                m_CLCurrentItemInfo.IsSFItemCounted = True
                                m_CLCurrentItemInfo.IsSFDifferentSession = False

                                'Checks if the product is already counted in sales floor
                                'If not counted then add the item to the Counted Items list
                                Dim bIsProductAlreadyCounted As Boolean = False

                                For Each objCountedProductData As CLProductCountedData In objAppContainer.m_CLSalesFloorCountedInfoList
                                    If objCountedProductData.m_strListId.Equals(m_CLCurrentProductGroup.ListID) AndAlso _
                                    objCountedProductData.m_strProductCode.Equals(m_CLCurrentItemInfo.ProductCode) Then
                                        bIsProductAlreadyCounted = True
                                        Exit For
                                    End If
                                Next

                                If Not bIsProductAlreadyCounted Then
                                    objAppContainer.objCLSummary.iSFListCounted += 1
                                    m_iSalesFloorItemCount = m_iSalesFloorItemCount + 1
                                    Dim objSalesFloorCountedProduct As CLProductCountedData = New CLProductCountedData()
                                    objSalesFloorCountedProduct.m_strProductCode = m_CLCurrentItemInfo.ProductCode
                                    objSalesFloorCountedProduct.m_strListId = m_CLCurrentProductGroup.ListID

                                    objAppContainer.m_CLSalesFloorCountedInfoList.Add(objSalesFloorCountedProduct)
                                End If

                                If Not m_CLCurrentProductGroup.IsActive Then
                                    m_CLCurrentProductGroup.IsActive = True
                                End If
                                If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                                    arrProductList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                                    For Each objCurrentProductInfo In arrProductList
                                        If objCurrentProductInfo.BootsCode.Equals(strBootsCode) Then
                                            objCurrentProductInfo = m_CLCurrentItemInfo
                                            Exit For
                                        End If
                                    Next
                                End If

                                '''''''''
                                DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, m_SelectedPOGSeqNum)
                            Else
                                DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, m_SelectedPOGSeqNum)
                            End If
                        Else
                            '''''''''''''''''
                            If UpdateSalesFloorProductInfo(strProductCode, iQuantity, strUnFormatBootsCode) Then
                                If strLocation = Macros.COUNT_LIST_ITEMDETAILS Then
                                    DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, m_SelectedPOGSeqNum)
                                Else
                                    DisplayCLScreen(CLSCREENS.SalesFloorProductCount, m_iCountedLocation, m_SelectedPOGSeqNum)
                                End If
                            End If
                        End If
                    End If 'ee
                ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                    'Updates the list with modified data
                    If m_bIsCreateOwnList Then
                        If UpdateCOLProductInfo(strProductCode, 0, iQuantity, 0, strUnFormatBootsCode) Then
                            If strLocation = Macros.COUNT_LIST_ITEMDETAILS Then
                                DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, m_SelectedPOGSeqNum, Nothing, Nothing, Macros.COUNT_LIST_PROCESSZERO)
                            Else
                                DisplayCLScreen(CLSCREENS.BackShopProductCount, m_iCountedLocation, m_SelectedPOGSeqNum)
                            End If
                        End If
                    Else 'ee
                        If UpdateBackShopProductInfo(strProductCode, iQuantity, strUnFormatBootsCode) Then
                            If strLocation = Macros.COUNT_LIST_ITEMDETAILS Then
                                DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, m_SelectedPOGSeqNum, Nothing, Nothing, Macros.COUNT_LIST_PROCESSZERO)
                            Else
                                DisplayCLScreen(CLSCREENS.BackShopProductCount, m_iCountedLocation, m_SelectedPOGSeqNum)
                            End If
                        End If
                    End If
                    'ambli
                    'For OSSR
                ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
#If RF Then
                    If m_bIsCreateOwnList Then
                        If UpdateCOLProductInfo(strProductCode, 0, 0, iQuantity, strUnFormatBootsCode) Then
                            If strLocation = Macros.COUNT_LIST_ITEMDETAILS Then
                                DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, m_SelectedPOGSeqNum, Nothing, Nothing, Macros.COUNT_LIST_PROCESSZERO)
                            Else
                                DisplayCLScreen(CLSCREENS.OSSRProductCount, m_iCountedLocation, m_SelectedPOGSeqNum)
                            End If
                        End If
                    Else 'ee
                        If UpdateOSSRProductInfo(strProductCode, iQuantity, strUnFormatBootsCode) Then
                            If strLocation = Macros.COUNT_LIST_ITEMDETAILS Then
                                DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, m_SelectedPOGSeqNum, Nothing, Nothing, Macros.COUNT_LIST_PROCESSZERO)
                            Else
                                DisplayCLScreen(CLSCREENS.OSSRProductCount, m_iCountedLocation, m_SelectedPOGSeqNum)
                            End If
                        End If
                    End If
#End If
                End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessZeroSelection of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessZeroSelection of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Processes the Quit button click on the Site Info screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Function ProcessSiteInformationQuit() As Boolean

        objAppContainer.objLogger.WriteAppLog("Entered ProcessSiteInformationQuit of CLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim iResult As Integer = 0
            Dim iCount As Integer
            Dim multiSiteCountTable As New Hashtable()
            Dim arrDiscrepancyList As New ArrayList()
            Dim arrMultiSiteList As ArrayList
            Dim arrObjItemList As New ArrayList()
            Dim objMBSPlannerInfo As New CLMultiSiteInfo()
            Dim objPSPPlannerInfo As New CLMultiSiteInfo()
            Dim arrCompleteCountList As New ArrayList()
            Dim arrInCompleteCountList As New ArrayList()
            Dim arrProductList As New ArrayList()
            Dim arrItemSummaryLsit As New ArrayList()
            Dim bIsOSSR As Boolean = False
            Dim arrObjOSSRItemList As New ArrayList()
            Dim arrObjBSItemList As New ArrayList()
#If NRF Then
            'arrObjItemList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
            arrObjBSItemList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
#ElseIf RF Then
            If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                arrProductList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                For Each objProductInfo As CLProductInfo In arrProductList
                    'If RF mode, Check whether item is OSSR item or BS item
                    If objAppContainer.OSSRStoreFlag = "Y" Then
                        If objProductInfo.OSSRFlag = "O" Then
                            arrObjOSSRItemList.add(objProductInfo)
                        else
                            arrObjBSItemList.add(objProductInfo)
                        End If
                    else
                         arrObjBSItemList.add(objProductInfo)
                    End If
                Next
            End if
#End If
            If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                'Iterate items in structure to invoke discrepancy and location summary screens
                If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    arrProductList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                    For Each objProductInfo As CLProductInfo In arrProductList
                        iCount = 0
                        'Check for counted items
                        If Not objProductInfo.SalesFloorQuantity < 0 Then
                            arrMultiSiteList = New ArrayList()
                            arrMultiSiteList = objProductInfo.SFMultiSiteList
                            'Get the counted site count
                            If Not arrMultiSiteList Is Nothing Then
                                For Each objPlannerInfo As CLMultiSiteInfo In arrMultiSiteList
                                    If objPlannerInfo.IsCounted.Equals("Y") Then
                                        iCount = iCount + 1
                                    End If
                                Next
                                'If item not counted in all sites, populate it into discrepancy list
                                If iCount < arrMultiSiteList.Count Then
                                    arrDiscrepancyList.Add(objProductInfo)
                                End If
                            End If
                        End If
                    Next
                End If

                'Display screens according to whther item present in discrepancy list
                'If no discrepancy, diplay locationselection screen
                If arrDiscrepancyList.Count <> 0 Then
                    DisplayCLScreen(CLSCREENS.ViewListScreen, m_iCountedLocation, 0, Nothing, arrDiscrepancyList, Macros.Count_LIST_DISCREPANCY)
                Else
                    If m_bIsCreateOwnList Then
                        DisplayCLScreen(CLSCREENS.COLLocationSelection, m_iCountedLocation)
                    Else
                        DisplayCLScreen(CLSCREENS.LocationSelection, m_iCountedLocation)
                    End If
                End If

                'If quit from back shop
            ElseIf m_iCountedLocation = Macros.COUNT_BACK_SHOP Then
                If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    For Each objProductInfo As CLProductInfo In arrObjBSItemList
                        iCount = 0
                        'Check for counted items
                        If Not objProductInfo.BackShopQuantity < 0 Then
                            arrMultiSiteList = New ArrayList()
                            arrMultiSiteList = objProductInfo.BSMultiSiteList
                            If Not arrMultiSiteList Is Nothing Then
                                If arrMultiSiteList.Count > 1 Then
                                    objMBSPlannerInfo = arrMultiSiteList.Item(0)
                                    objPSPPlannerInfo = arrMultiSiteList.Item(1)
                                    'item counted in psp and not in mbs, Then display discrepancy
                                    If objPSPPlannerInfo.IsCounted.Equals("Y") And _
                                                objMBSPlannerInfo.IsCounted.Equals("N") Then
                                        arrDiscrepancyList.Add(objProductInfo)
                                    ElseIf objPSPPlannerInfo.IsCounted.Equals("N") And _
                                                objMBSPlannerInfo.IsCounted.Equals("Y") Then
                                        arrItemSummaryLsit.Add(objProductInfo)
                                        'Item counted in both sites,populate into coplete count list
                                    ElseIf objMBSPlannerInfo.IsCounted.Equals("Y") And _
                                            objPSPPlannerInfo.IsCounted.Equals("Y") Then
                                        arrCompleteCountList.Add(objProductInfo)
                                    End If
                                    'Item present in MBS alone. If counted in mbs, populate into completed list
                                ElseIf arrMultiSiteList.Count = 1 Then
                                    objMBSPlannerInfo = arrMultiSiteList.Item(0)
                                    If objMBSPlannerInfo.IsCounted.Equals("Y") Then
                                        arrCompleteCountList.Add(objProductInfo)
                                    End If
                                End If
                            End If
                        Else
                            arrInCompleteCountList.Add(objProductInfo)
                        End If
                    Next
                End If

                If arrDiscrepancyList.Count <> 0 Then
                    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M98"), "Alert", MessageBoxButtons.OK, _
                                      MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                    If iResult = MsgBoxResult.Ok Then
                        DisplayCLScreen(CLSCREENS.ViewListScreen, m_iCountedLocation, 0, Nothing, arrDiscrepancyList, Macros.Count_LIST_DISCREPANCY)
                    End If
                    'if all/none of the items present in complte list, display location selection
                ElseIf arrInCompleteCountList.Count = arrObjBSItemList.Count Or arrCompleteCountList.Count = arrObjBSItemList.Count Then
                    If m_bIsCreateOwnList Then
                        DisplayCLScreen(CLSCREENS.COLLocationSelection, m_iCountedLocation)
                    Else
                    DisplayCLScreen(CLSCREENS.LocationSelection, m_iCountedLocation)
                    End If
                    ' If items remaining in list to be counted, display item summary
                ElseIf arrItemSummaryLsit.Count <> 0 Then
                    'Display warning msg that item counted in mbs and not in psp
                    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M97"), "Confirmation", _
                                              MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                                              MessageBoxDefaultButton.Button1)
                    If iResult = MsgBoxResult.Yes Then
                        'display item summary screen
                        DisplayCLScreen(CLSCREENS.ViewListScreen, m_iCountedLocation, 0, Nothing, arrItemSummaryLsit, Macros.COUNT_LIST_BACKSHOPSUMMARY)
                    Else
                        'Display site information screeen if selected no from warning msg
                        DisplayCLScreen(CLSCREENS.SiteInformation, m_iCountedLocation)
                        Return False
                    End If
                ElseIf arrItemSummaryLsit.Count = 0 Then
                    If m_bIsCreateOwnList Then
                        DisplayCLScreen(CLSCREENS.COLLocationSelection, m_iCountedLocation)
                    Else
                    DisplayCLScreen(CLSCREENS.LocationSelection, m_iCountedLocation)
                    End If
                End If
#If RF Then
            ElseIf m_iCountedLocation = Macros.COUNT_OSSR Then
                If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    For Each objProductInfo As CLProductInfo In arrObjOSSRItemList
                        iCount = 0
                        'Check for counted items
                        If Not objProductInfo.OSSRQuantity < 0 Then
                            arrMultiSiteList = New ArrayList()
                            arrMultiSiteList = objProductInfo.OSSRMultiSiteList
                            If Not arrMultiSiteList Is Nothing Then
                                If arrMultiSiteList.Count > 1 Then
                                    objMBSPlannerInfo = arrMultiSiteList.Item(0)
                                    objPSPPlannerInfo = arrMultiSiteList.Item(1)
                                    'item counted in psp and not in mbs, Then display discrepancy
                                    If objPSPPlannerInfo.IsCounted.Equals("Y") And _
                                                objMBSPlannerInfo.IsCounted.Equals("N") Then
                                        arrDiscrepancyList.Add(objProductInfo)
                                    ElseIf objPSPPlannerInfo.IsCounted.Equals("N") And _
                                                objMBSPlannerInfo.IsCounted.Equals("Y") Then
                                        arrItemSummaryLsit.Add(objProductInfo)
                                        'Item counted in both sites,populate into coplete count list
                                    ElseIf objMBSPlannerInfo.IsCounted.Equals("Y") And _
                                            objPSPPlannerInfo.IsCounted.Equals("Y") Then
                                        arrCompleteCountList.Add(objProductInfo)
                                    End If
                                    'Item present in MBS alone. If counted in mbs, populate into completed list
                                ElseIf arrMultiSiteList.Count = 1 Then
                                    objMBSPlannerInfo = arrMultiSiteList.Item(0)
                                    If objMBSPlannerInfo.IsCounted.Equals("Y") Then
                                        arrCompleteCountList.Add(objProductInfo)
                                    End If
                                End If
                            End If
                        Else
                            arrInCompleteCountList.Add(objProductInfo)
                        End If
                    Next
                End If

                If arrDiscrepancyList.Count <> 0 Then
                    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M111"), "Alert", MessageBoxButtons.OK, _
                                      MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                    If iResult = MsgBoxResult.Ok Then
                        DisplayCLScreen(CLSCREENS.ViewListScreen, m_iCountedLocation, 0, Nothing, arrDiscrepancyList, Macros.Count_LIST_DISCREPANCY)
                    End If
                    'if all/none of the items present in complte list, display location selection
                ElseIf arrCompleteCountList.Count = 0 Or arrCompleteCountList.Count = arrObjOSSRItemList.Count Then
                    If m_bIsCreateOwnList Then
                        DisplayCLScreen(CLSCREENS.COLLocationSelection, m_iCountedLocation)
                    Else
                    DisplayCLScreen(CLSCREENS.LocationSelection, m_iCountedLocation)
                    End If
                    'If items remaining in list to be counted, display item summary
                ElseIf arrItemSummaryLsit.Count <> 0 Then
                    'Display warning msg that item counted in mbs and not in psp
                    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M97"), "Confirmation", _
                                              MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                                              MessageBoxDefaultButton.Button1)
                    If iResult = MsgBoxResult.Yes Then
                        'display item summary screen
                        DisplayCLScreen(CLSCREENS.ViewListScreen, m_iCountedLocation, 0, Nothing, arrItemSummaryLsit, Macros.COUNT_LIST_BACKSHOPSUMMARY)
                    Else
                        'Display site information screeen if selected no from warning msg
                        DisplayCLScreen(CLSCREENS.SiteInformation, m_iCountedLocation)
                        Return False
                    End If
                ElseIf arrItemSummaryLsit.Count = 0 Then
                    If m_bIsCreateOwnList Then
                        DisplayCLScreen(CLSCREENS.COLLocationSelection, m_iCountedLocation)
                    Else
                    DisplayCLScreen(CLSCREENS.LocationSelection, m_iCountedLocation)
                    End If
                End If
#End If
            End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured while processing ProcessSiteInformationQuit of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ProcessSiteInformationQuit of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Populate the RF site information  from datatable 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function PopulateRFSiteInfoData(ByVal objCountListDataTable As DataTable) As Boolean
        'Stock File Accuracy  added new function 
        'Tp populate the productinfo structure with relevant details
        objAppContainer.objLogger.WriteAppLog("Entered PopulateRFSiteInfoData of CLSessionMgr", Logger.LogLevel.INFO)

        Dim objCLProductInfoTable As New Hashtable()
        Dim objItemInfoList As New ArrayList()
        Dim arrSiteList As New ArrayList()
        Dim arrTempSiteList As New ArrayList()
        Dim arrBootsCodeList As ArrayList
        Dim arrPlannerList As New ArrayList()
        Dim arrItemList As ArrayList = Nothing
        Dim iSiteCount As Integer
        Dim iRepeatCount As Integer
        Dim iCount As Integer
        Dim objPlannerInfo As CLMultiSiteInfo
        Dim strPlannerDesc As String
        Dim strBootsCode As String = Nothing
        Dim strItem As String = Nothing
        Dim objBSItem As CLProductInfo

        Dim objProductInfo As CLProductInfo
        Dim objSiteInfo As CLMultiSiteInfo
        Dim objItem As CLProductInfo
        Dim arrTempMBSItemList As New ArrayList()
        Dim arrTempPSPItemList As New ArrayList()
        Dim arrTempOSSR_BSItemList As New ArrayList()
        Dim arrTempOSSR_PSPItemList As New ArrayList()
        Dim arrMBSItemList As New ArrayList()
        Dim arrPSPItemList As New ArrayList()
        Dim arrOSSR_BSItemList As New ArrayList()
        Dim arrOSSR_PSPItemList As New ArrayList()
        Dim isDiffSession As String = Nothing
        Try
            iCount = objCountListDataTable.Rows.Count()
            'Iterate through the items in datatable and retrieve unique siteids to a list
            For i As Integer = 0 To iCount - 1
                strPlannerDesc = objCountListDataTable.Select()(i).ItemArray(2)
                If Not arrTempSiteList.Contains(strPlannerDesc) Then
                    arrTempSiteList.Add(strPlannerDesc)

                    objSiteInfo = New CLMultiSiteInfo()
                    objSiteInfo.strPlannerDesc = strPlannerDesc
                    objSiteInfo.strPOGDescription = objCountListDataTable.Select()(i).ItemArray(4)
                    arrSiteList.Add(objSiteInfo)
                End If
            Next

            'Iterate through the unique sites in the selected list and
            'fetch the items present in each site.
            If Not objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                For Each objCurrentSiteInfo As CLMultiSiteInfo In arrSiteList
                    arrBootsCodeList = New ArrayList()
                    For i As Integer = 0 To iCount - 1
                        objProductInfo = New CLProductInfo()
                        strBootsCode = objCountListDataTable.Select()(i).ItemArray(1)
                        strPlannerDesc = objCountListDataTable.Select()(i).ItemArray(2)
                        If (strPlannerDesc = objCurrentSiteInfo.strPlannerDesc) Then
                            If Not arrBootsCodeList.Contains(strBootsCode) Then
                                arrBootsCodeList.Add(strBootsCode)
                            End If
                        End If
                    Next

                    objPlannerInfo = New CLMultiSiteInfo()
                    objPlannerInfo.strPlannerDesc = objCurrentSiteInfo.strPlannerDesc
                    objPlannerInfo.iItemCount = arrBootsCodeList.Count()
                    arrPlannerList.Add(objPlannerInfo)

                    'Fetch the site count for an item in arrBootsCodeList for current planner
                    arrItemList = New ArrayList()
                    For Each strItemCode As String In arrBootsCodeList
                        iSiteCount = 0
                        objProductInfo = New CLProductInfo()
                        isDiffSession = Nothing
                        For i As Integer = 0 To iCount - 1
                            strItem = objCountListDataTable.Select()(i).ItemArray(1)
                            strPlannerDesc = objCountListDataTable.Select()(i).ItemArray(2)
                            iRepeatCount = objCountListDataTable.Select()(i).ItemArray(3)
                            If (strItemCode = strItem) And (strPlannerDesc = objCurrentSiteInfo.strPlannerDesc) Then
                                If m_bIsCreateOwnList Then
                                    isDiffSession = "N"
                                Else
                                    isDiffSession = objCountListDataTable.Select()(i).ItemArray(5)
                                End If
                                For counter As Integer = 1 To iRepeatCount
                                    iSiteCount = iSiteCount + 1
                                Next
                            End If
                        Next
                        objProductInfo.BootsCode = strItemCode
                        objProductInfo.TotalSiteCount = iSiteCount
                        If isDiffSession.Equals("Y") Then
                            objProductInfo.CountStatus = "Y"
                        Else
                            objProductInfo.CountStatus = "N"
                        End If
                        arrItemList.Add(objProductInfo)
                    Next
                    objCLProductInfoTable.Add(objCurrentSiteInfo.strPlannerDesc, arrItemList)
                Next

                'Not on planner 

                If m_CLCurrentProductGroup.NotOnPlannerItemCount > 0 Then
                    Dim objOtherSiteInfo As New CLMultiSiteInfo()
                    objOtherSiteInfo.strPlannerDesc = "Not On Planner"
                    'objSiteInfo.strSeqNumber = "1"
                    objOtherSiteInfo.iItemCount = m_CLCurrentProductGroup.NotOnPlannerItemCount
                    arrPlannerList.Add(objOtherSiteInfo)

                    Dim arrBootsCodeItemList As New ArrayList()
                    Dim arrNotPlannerItemList As New ArrayList()
                    arrNotPlannerItemList = m_CLCurrentProductGroup.NotOnPlannerItemList
                    For Each objItemInfo As CLProductInfo In arrNotPlannerItemList
                        If objItemInfo.SalesFloorQuantity < 0 Then
                            objItemInfo.CountStatus = "N"
                        Else
                            objItemInfo.CountStatus = "Y"
                        End If
                        objItemInfo.TotalSiteCount = 1
                    Next
                    arrBootsCodeItemList = arrNotPlannerItemList
                    objCLProductInfoTable.Add(objOtherSiteInfo.strPlannerDesc.Trim(), arrBootsCodeItemList)
                End If

                If m_bIsCreateOwnList Then
                    If m_CLCurrentProductGroup.UnknownItemCount > 0 Then
                        'Populte unknown category in SF
                        Dim objOtherSiteInfo As New CLMultiSiteInfo()
                        objOtherSiteInfo.strPlannerDesc = "Unknown"
                        'objSiteInfo.strSeqNumber = "1"
                        objOtherSiteInfo.iItemCount = m_CLCurrentProductGroup.UnknownItemCount
                        arrPlannerList.Add(objOtherSiteInfo)

                        Dim arrBootsCodeItemList As New ArrayList()
                        Dim arrUnknownItemList As New ArrayList()
                        arrUnknownItemList = m_CLCurrentProductGroup.UnknownItemList
                        For Each objItemInfo As CLProductInfo In arrUnknownItemList
                            If objItemInfo.SalesFloorQuantity < 0 Then
                                objItemInfo.CountStatus = "N"
                            Else
                                objItemInfo.CountStatus = "Y"
                            End If
                            objItemInfo.TotalSiteCount = 1
                        Next
                        arrBootsCodeItemList = arrUnknownItemList
                        objCLProductInfoTable.Add(objOtherSiteInfo.strPlannerDesc.Trim(), arrBootsCodeItemList)
                    End If
                End If

                'Unique sites populated to a global structure
                If Not objAppContainer.objGlobalCLSiteInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    objAppContainer.objGlobalCLSiteInfoTable.Add(m_CLCurrentProductGroup.ListID, arrPlannerList)
                End If


                If m_bIsCreateOwnList Then
                    objItemInfoList = m_CLItemList
                Else
                    objItemInfoList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                End If

                For Each objItemInfo As CLProductInfo In objItemInfoList
                    'If Not objItemInfo.IsNotPlannerItem Then
                    strItem = objItemInfo.BootsCode
#If RF Then
                        If objAppContainer.OSSRStoreFlag = "Y" Then
                            If objItemInfo.OSSRFlag = "O" Then
                                arrTempOSSR_BSItemList.Add(objItemInfo)
                                If objItemInfo.PendingSalesFlag Then
                                    arrTempOSSR_PSPItemList.Add(objItemInfo)
                                    m_bISItemsInOSSRPSP = True
                                End If
                            Else
                                arrTempMBSItemList.Add(objItemInfo)
                                If objItemInfo.PendingSalesFlag Then
                                    arrTempPSPItemList.Add(objItemInfo)
                                    m_bIsItemsInPSP = True
                                End If
                            End If
                        Else
                            arrTempMBSItemList.Add(objItemInfo)
                            If objItemInfo.PendingSalesFlag Then
                                arrTempPSPItemList.Add(objItemInfo)
                                m_bIsItemsInPSP = True
                            End If
                        End If
#ElseIf NRF Then
                    If Not objItemInfo.IsUnknownItem Then
                        arrTempMBSItemList.Add(objItemInfo)
                        If objItemInfo.PendingSalesFlag Then
                            arrTempPSPItemList.Add(objItemInfo)
                            m_bIsItemsInPSP = True
                        End If
                    End If
#End If

                        'End If
                Next

                'Assign counts
                If m_bIsCreateOwnList Then
                    m_CLCurrentProductGroup.BSItemCount = arrTempMBSItemList.Count()
                    m_CLCurrentProductGroup.OSSRItemCount = arrTempOSSR_BSItemList.Count()
                End If

                If arrTempMBSItemList.Count() > 0 Then
                    For Each item As CLProductInfo In arrTempMBSItemList
                        objItem = New CLProductInfo()
                        objItem.BootsCode = item.BootsCode
                        If item.BackShopMBSQuantity < 0 Then
                            objItem.CountStatus = "N"
                        Else
                            objItem.CountStatus = "Y"
                        End If
                        arrMBSItemList.Add(objItem)
                    Next
                    objCLProductInfoTable.Add(Macros.COUNT_MBS, arrMBSItemList)
                End If
                '&&&&&&&&&&&&
                'Populte unknown category in Backshop
                If m_CLCurrentProductGroup.UnknownItemCount > 0 Then
                    Dim arrBSUnknownItemList As New ArrayList()
                    Dim arrUItemList As New ArrayList()
                    arrUItemList = m_CLCurrentProductGroup.UnknownItemList
                    For Each objItemInfo As CLProductInfo In arrUItemList
                        If objItemInfo.BackShopMBSQuantity < 0 Then
                            objItemInfo.CountStatus = "N"
                        Else
                            objItemInfo.CountStatus = "Y"
                        End If
                        objItemInfo.TotalSiteCount = 1
                        arrBSUnknownItemList.Add(objItemInfo)
                    Next
                    objCLProductInfoTable.Add(Macros.COUNT_BS_UNKNOWN, arrBSUnknownItemList)
                End If
                '&&&&&&&&&&&&
                If arrTempPSPItemList.Count() > 0 Then
                    For Each item As CLProductInfo In arrTempPSPItemList
                        objItem = New CLProductInfo()
                        objItem.BootsCode = item.BootsCode
                        If m_bIsCreateOwnList Then
                            objItem.CountStatus = "N"
                        Else
                            If item.BackShopPSPQuantity < 0 Then
                                objItem.CountStatus = "N"
                            Else
                                objItem.CountStatus = "Y"
                            End If
                        End If
                        arrPSPItemList.Add(objItem)
                    Next
                    objCLProductInfoTable.Add(Macros.COUNT_PSP, arrPSPItemList)
                End If
                If arrTempOSSR_BSItemList.Count() > 0 Then
                    For Each item As CLProductInfo In arrTempOSSR_BSItemList
                        objItem = New CLProductInfo()
                        objItem.BootsCode = item.BootsCode
                        If m_bIsCreateOwnList Then
                            objItem.CountStatus = "N"
                        Else
                            If item.OSSRMBSQuantity < 0 Then
                                objItem.CountStatus = "N"
                            Else
                                objItem.CountStatus = "Y"
                            End If
                        End If
                        arrOSSR_BSItemList.Add(objItem)
                    Next
                    objCLProductInfoTable.Add(Macros.COUNT_OSSR_BS, arrOSSR_BSItemList)
                End If
                If arrTempOSSR_PSPItemList.Count() > 0 Then
                    For Each item As CLProductInfo In arrTempOSSR_PSPItemList
                        objItem = New CLProductInfo()
                        objItem.BootsCode = item.BootsCode
                        If m_bIsCreateOwnList Then
                            objItem.CountStatus = "N"
                        Else
                            If item.OSSRPSPQuantity < 0 Then
                                objItem.CountStatus = "N"
                            Else
                                objItem.CountStatus = "Y"
                            End If
                        End If
                        arrOSSR_PSPItemList.Add(objItem)
                    Next
                    objCLProductInfoTable.Add(Macros.COUNT_OSSR_PSP, arrOSSR_PSPItemList)
                End If
                'End If
                'Populate the global structure
                objAppContainer.objGlobalCLInfoTable.Add(m_CLCurrentProductGroup.ListID, objCLProductInfoTable)

            End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured while processing PopulateRFSiteInfoData of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PopulateRFSiteInfoData of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function

    ''' <summary>
    ''' Check if a valid location is selected for a multisited item
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckLocationSelection() As Boolean
        'Stock File Accuracy  - Added to check location selction
        If m_bIsMultisited Then
            If m_iCountedLocation = Macros.COUNT_SALES_FLOOR Then
                If (m_SelectedPOGSeqNum = Nothing) Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M52"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                    Return False
                End If
            End If
        End If
        Return True
    End Function
    ''' <summary>
    ''' validates the selection of Next and Back Buttons
    ''' </summary>
    ''' <param name="iSender"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ValidateNextAndback(Optional ByVal iSender As Integer = 0) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ValidateNextAndback of CLSessionMgr", Logger.LogLevel.INFO)
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim objCurrentCountListInfoTable As Hashtable = New Hashtable()
        Dim bIsValidChoice As Boolean = True
        Try
            'Gets the current product info from hash table based on the list id 
            'and the position of the item in the list as indicated by 
            'm_iProductListCount variable
            'If m_bIsCreateOwnList Then
            '    objCurrentProductInfoList = m_CLItemList
            'Else
            If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                objCurrentCountListInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                objCurrentProductInfoList = objCurrentCountListInfoTable.Item(m_CLCurrentSiteInfo.strPlannerDesc)
            End If
            'End If
            'Validates the next and back buttons
            If m_iProductListCount < 0 Then
                m_iProductListCount = m_iProductListCount + 1
                'If the sender is from next button click of process then isender val is 2
                If Not iSender = Macros.SENDER_PROCESS_ACTION Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M5"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                    'MessageBox.Show("This is the first item")
                End If
                bIsValidChoice = False
            End If
            If (m_iProductListCount > objCurrentProductInfoList.Count - 1) Then
                m_iProductListCount = m_iProductListCount - 1
                'If the sender is from next button click of process then isender val is 2
                If Not iSender = Macros.SENDER_PROCESS_ACTION Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M6"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                    'MessageBox.Show("This is the last item")
                End If
                bIsValidChoice = False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured while processing ValidateNextAndback of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ValidateNextAndback of CLSessionMgr", Logger.LogLevel.INFO)
        Return bIsValidChoice
    End Function
#If NRF Then

    ''' <summary>
    ''' Writes the required data to the temporary data file
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub WriteTempData()
        objAppContainer.objLogger.WriteAppLog("Entered WriteTempData of CLSessionMgr", Logger.LogLevel.INFO)
        'Dim objSMExportDataManager As SMTransactDataManager = New SMTransactDataManager()
        Dim arrQueuedSELs As ArrayList = Nothing
        Dim strStatus As String
        Dim bCheckCountListData As Boolean = False
        Dim strSeq As String = Nothing
        Dim objCOLArray As ArrayList = New ArrayList()
        Dim objCLCRecord As CLCRecord
        Dim objCLSRecord As CLSRecord
        Try
            'To collect all SEL Print request.
            arrQueuedSELs = New ArrayList()
            Dim bIsDataToWrite As Boolean = False
            'Integration testing
            Dim iNumSELs As Integer = 0
            objAppContainer.objExportDataManager.DeleteTempFiles(SMTransactDataManager.ExFileType.CLTemp)

            'Checks if any of the Lists are accessed/Created
            If objAppContainer.m_CLBackShopCountedInfoList.Count > 0 Or objAppContainer.m_CLSalesFloorCountedInfoList.Count > 0 Then
                bIsDataToWrite = True
            End If

            'Added as part of SFA-Create Own List-Checks if there is a list created by user to be written

            If Not objAppContainer.objGlobalCreateCountList.Count = 0 Then
                bCheckCountListData = True
            End If
            If bIsDataToWrite Or bCheckCountListData Then
#If NRF Then
                objAppContainer.objExportDataManager.CreateCLO(SMTransactDataManager.ExFileType.CLTemp)
#End If
            Else
                objAppContainer.objLogger.WriteAppLog("No Temp Data to be written", Logger.LogLevel.INFO)
            End If

            If bCheckCountListData Then

                'objAppContainer.objExportDataManager.CreateCLO(SMTransactDataManager.ExFileType.CLTemp)
                'If bCheckCountListData Then
     For Each sKey As String In objAppContainer.objGlobalCreateCountList.Keys
#If NRF Then
                objAppContainer.objExportDataManager.CreateCLA(SMTransactDataManager.ExFileType.CLTemp, Macros.START_USER_COUNTLIST)
#End If
                'Iterates through the Product list to write data for each Product Info


                    objCOLArray = objAppContainer.objGlobalCreateCountList.Item(sKey)
                    strSeq = "000"
                    For Each objCLProductInfo As CLProductInfo In objCOLArray
                        Dim objCLDRecord As CLDRecord = New CLDRecord()
                        strSeq = strSeq + 1
                        objCLDRecord.strBootsCode = objCLProductInfo.BootsCode
                        objCLDRecord.strSequence = strSeq
                        objCLDRecord.cSitetype = objCLProductInfo.CreatedLocation
                        objAppContainer.objExportDataManager.CreateCLD(objCLDRecord)

                        'Writes the CLC record to signify Create own List Data entered

                        
                        'Sets the values
                        If objCLProductInfo.IsSFItemCounted Then
                            objCLCRecord = New CLCRecord()
                            objCLCRecord.strNumberSEQ = strSeq
                            If objCLProductInfo.IsUnknownItem Then
                                objCLCRecord.strBootscode = "0000000"
                            Else
                                objCLCRecord.strBootscode = objCLProductInfo.BootsCode
                            End If
                            objCLCRecord.strCount = objCLProductInfo.SalesFloorQuantity
                            objCLCRecord.strCountLocation = Macros.SHOP_FLOOR
                            objCLCRecord.strUpdateOSSR = " "
                            objCLCRecord.strSalesAtTimeOfUpload = "XXXXXXXXX"
                            objAppContainer.objExportDataManager.CreateCLC(objCLCRecord, SMTransactDataManager.ExFileType.CLTemp)
                        End If
                        If objCLProductInfo.IsMBSItemCounted Then
                            objCLCRecord = New CLCRecord()
                            objCLCRecord.strNumberSEQ = strSeq
                            If objCLProductInfo.IsUnknownItem Then
                                objCLCRecord.strBootscode = "0000000"
                            Else
                                objCLCRecord.strBootscode = objCLProductInfo.BootsCode
                            End If
                            objCLCRecord.strCountLocation = Macros.BACK_SHOP
                            objCLCRecord.strCount = objCLProductInfo.BackShopMBSQuantity
                            objCLCRecord.strUpdateOSSR = " "
                            objCLCRecord.strSalesAtTimeOfUpload = "XXXXXXXXX"
                            objAppContainer.objExportDataManager.CreateCLC(objCLCRecord, SMTransactDataManager.ExFileType.CLTemp)
                        End If
                        If objCLProductInfo.IsBSPSPItemCounted Then
                            objCLCRecord = New CLCRecord()
                            objCLCRecord.strNumberSEQ = strSeq
                            objCLCRecord.strBootscode = objCLProductInfo.BootsCode
                            objCLCRecord.strCountLocation = Macros.PSP
                            objCLCRecord.strCount = objCLProductInfo.BackShopPSPQuantity
                            objCLCRecord.strUpdateOSSR = " "
                            objCLCRecord.strSalesAtTimeOfUpload = "XXXXXXXXX"
                            objAppContainer.objExportDataManager.CreateCLC(objCLCRecord, SMTransactDataManager.ExFileType.CLTemp)
                        End If
                        If objCLProductInfo.NumSELsQueued > 0 Then
                            'Integration Testing
                            iNumSELs += objCLProductInfo.NumSELsQueued
                            Dim iCount As Integer = 0
                            For iCount = 0 To objCLProductInfo.NumSELsQueued - 1
                                'Write PRT record for the SEL requests Queued
                                Dim objPRTRecord As PRTRecord = New PRTRecord()
                                'Sets the values
                                objPRTRecord.strBootscode = objCLProductInfo.BootsCode
                                objPRTRecord.cIsMethod = Macros.PRINT_BATCH
                                'objAppContainer.objExportDataManager.CreatePRT(objPRTRecord.strBootscode, _
                                '                                        SMTransactDataManager.ExFileType.CLTemp)
                                arrQueuedSELs.Add(objPRTRecord)
                            Next
                        End If
                    Next

                    'm_ModulePriceCheck.WriteExportData(arrQueuedSELs)

                'Writes the CLA record to Signify Create own List End. 

                objAppContainer.objExportDataManager.CreateCLA(SMTransactDataManager.ExFileType.CLTemp, Macros.END_USER_COUNTLIST)
                Dim objCLXRecord As CLXRecord = New CLXRecord()
                objCLXRecord.cIsCommit = Macros.CLX_COMMIT_YES
                objCLXRecord.strCountType = Macros.USER_COUNT_LIST
                objAppContainer.objExportDataManager.CreateCLX(objCLXRecord, SMTransactDataManager.ExFileType.CLTemp)
Next
 End If

            'Write temp data for Count List
            If bIsDataToWrite Then

                'Writes the CLO record to signify Count Lists Start
                Dim objCLProductInfoList As ArrayList = Nothing

                'Iterates through the Count lists to write data for each count list
                For Each objCountList As CLProductGroupInfo In objAppContainer.objGlobalCLProductGroupList

                    objCLProductInfoList = New ArrayList()
                    'If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(objCountList.ListID) Then
                    If objCountList.IsActive Then

                        'SFA DEF #804 - Send CLS to track the list in report
                        objCLSRecord = New CLSRecord()
                        objCLSRecord.strListID = objCountList.ListID
                        objAppContainer.objExportDataManager.CreateCLS(objCLSRecord, SMTransactDataManager.ExFileType.CLTemp)
                        'Obtains the product list for a particular list id
                        objCLProductInfoList = objAppContainer.objGlobalCLProductInfoTable.Item(objCountList.ListID)

                        'Iterates through the Product list to write data for each Product Info
                        For Each objCLProductInfo As CLProductInfo In objCLProductInfoList

                            'Checks if the particular item is counted atleast in one location
                            If (Not objCLProductInfo.IsSFItemCounted) And (Not objCLProductInfo.IsMBSItemCounted) And (Not objCLProductInfo.IsBSPSPItemCounted) Then
                                objAppContainer.objLogger.WriteAppLog("Not counted in any location. ProductCode is:" + objCLProductInfo.ProductCode, Logger.LogLevel.INFO)
                            Else
                                'Writes the CLC record to signify Count List Item Data entered

                                'Send CLC forSales floor
                                If objCLProductInfo.IsSFItemCounted Then
                                    objCLCRecord = New CLCRecord()
                                    'Sets the values
                                    objCLCRecord.strListID = objCountList.ListID
                                    objCLCRecord.strNumberSEQ = objCLProductInfo.SequenceNumber
                                    objCLCRecord.strBootscode = objCLProductInfo.BootsCode
                                    objCLCRecord.strCountLocation = Macros.SHOP_FLOOR
                                    objCLCRecord.strCount = objCLProductInfo.SalesFloorQuantity
                                    objCLCRecord.strUpdateOSSR = " "
                                    objCLCRecord.strSalesAtTimeOfUpload = objCLProductInfo.SalesAtPODDock
                                    objAppContainer.objExportDataManager.CreateCLC(objCLCRecord, SMTransactDataManager.ExFileType.CLTemp)
                                End If

                                'Send CLC for Back Shop
                                If objCLProductInfo.IsMBSItemCounted Then
                                    objCLCRecord = New CLCRecord()
                                    'Sets the values
                                    objCLCRecord.strListID = objCountList.ListID
                                    objCLCRecord.strNumberSEQ = objCLProductInfo.SequenceNumber
                                    objCLCRecord.strBootscode = objCLProductInfo.BootsCode
                                    objCLCRecord.strCountLocation = Macros.BACK_SHOP
                                    objCLCRecord.strCount = objCLProductInfo.BackShopMBSQuantity
                                    objCLCRecord.strUpdateOSSR = " "
                                    objCLCRecord.strSalesAtTimeOfUpload = "XXXXXXXXX"
                                    objAppContainer.objExportDataManager.CreateCLC(objCLCRecord, SMTransactDataManager.ExFileType.CLTemp)
                                End If

                                If objCLProductInfo.IsBSPSPItemCounted Then
                                    'Send CLC for BS PSP
                                    objCLCRecord = New CLCRecord()
                                    'Sets the values
                                    objCLCRecord.strListID = objCountList.ListID
                                    objCLCRecord.strNumberSEQ = objCLProductInfo.SequenceNumber
                                    objCLCRecord.strBootscode = objCLProductInfo.BootsCode
                                    objCLCRecord.strCountLocation = Macros.PSP
                                    objCLCRecord.strCount = objCLProductInfo.BackShopPSPQuantity
                                    objCLCRecord.strUpdateOSSR = " "
                                    objCLCRecord.strSalesAtTimeOfUpload = "XXXXXXXXX"
                                    objAppContainer.objExportDataManager.CreateCLC(objCLCRecord, SMTransactDataManager.ExFileType.CLTemp)
                                End If

                            End If

                            'Insert PRT records if any.
                            If objCLProductInfo.NumSELsQueued > 0 Then
                                'Integration Testing
                                iNumSELs += objCLProductInfo.NumSELsQueued
                                Dim iCount As Integer = 0

                                For iCount = 0 To objCLProductInfo.NumSELsQueued - 1
                                    'Write PRT record for the SEL requests Queued
                                    Dim objPRTRecord As PRTRecord = New PRTRecord()
                                    'Sets the values
                                    'objPRTRecord.strBootscode = objAppContainer.objHelper.GeneratePCwithCDV(objCLProductInfo.BootsCode)
                                    objPRTRecord.strBootscode = objCLProductInfo.BootsCode
                                    objPRTRecord.cIsMethod = Macros.PRINT_BATCH
                                    'Write the record to export data file.
                                    'objAppContainer.objExportDataManager.CreatePRT(objPRTRecord.strBootscode, _
                                    '                                   SMTransactDataManager.ExFileType.CLTemp)
                                    arrQueuedSELs.Add(objPRTRecord)
                                Next
                            End If
                        Next

                        'Writes the CLX record to Signify Count List Exit. 
                        'It is specified whether Count List is Locked for the day or Not
                        Dim objCLXRecord As CLXRecord = New CLXRecord()
                        objCLXRecord.strListID = objCountList.ListID
                        If objCountList.IsUpdate Then
                            objCLXRecord.cIsCommit = Macros.CLX_COMMIT_YES
                        Else
                            objCLXRecord.cIsCommit = Macros.CLX_COMMIT_NO

                            'objPRTRecord.strBootscode = objAppContainer.objHelper.GeneratePCwithCDV(objCLProductInfo.BootsCode)
                            'Write the record to export data file.


                        End If

                        If objCountList.ListType.Equals("H") Then
                            objCLXRecord.strCountType = "H"
                        ElseIf objCountList.ListType.Equals("R") Then
                            objCLXRecord.strCountType = "R"
                        ElseIf objCountList.ListType.Equals("U") Then
                            objCLXRecord.strCountType = "U"
                        ElseIf objCountList.ListType.Equals("N") Then
                            objCLXRecord.strCountType = "N"
                        End If
                        'Writes the CLF record to Signify Count List Finished. 
                        'Insert Price check records if any

                        objAppContainer.objExportDataManager.CreateCLX(objCLXRecord, SMTransactDataManager.ExFileType.CLTemp)

                    End If
                Next
            End If
            'Writes the CLF record to Signify Count List Finished. 
            If bIsDataToWrite Or bCheckCountListData Then
                objAppContainer.objExportDataManager.CreateCLF(SMTransactDataManager.ExFileType.CLTemp)
            End If
            'Insert Price check records if any
            m_ModulePriceCheck.WriteExportData(arrQueuedSELs)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured while processing WriteTempData of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit WriteTempData of CLSessionMgr", Logger.LogLevel.INFO)
    End Sub
#End If
    ''' <summary>
    ''' Writes the final set of data identified to the export data file
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteExportData(Optional ByVal bLockList As Boolean = True) As Boolean
        Dim arrQueuedSELs As ArrayList = Nothing
        Try
            objAppContainer.objLogger.WriteAppLog("Entered WriteExportData of CLSessionMgr", Logger.LogLevel.INFO)
            'To collect all SEL Print request.
            arrQueuedSELs = New ArrayList()
            Dim bIsDataToWrite As Boolean = False
            Dim bCheckCountListData As Boolean = False
            Dim strStatus As String
            Dim strSeq As String = Nothing
            'Integration testing
            Dim iNumSELs As Integer = 0
            Dim objCOLArray As ArrayList = New ArrayList()
            Dim objCLCRecord As CLCRecord
            Dim objCLSRecord As CLSRecord
            'Iterates through the Count lists to identify if there is data to be written
            'For Each objTempCountList As CLProductGroupInfo In objAppContainer.objGlobalCLProductGroupList
            '    If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(objTempCountList.ListID) Then
            '        bIsDataToWrite = True
            '        Exit For
            '    End If
            'Next

            'Checks if any of the Lists are accessed/Created
            If objAppContainer.m_CLBackShopCountedInfoList.Count > 0 Or objAppContainer.m_CLSalesFloorCountedInfoList.Count > 0 Then
                bIsDataToWrite = True
            End If

            'Added as part of SFA-Create Own List-Checks if there is a list created by user to be written

            If Not objAppContainer.objGlobalCreateCountList.Count = 0 Then
                bCheckCountListData = True
            End If

            If bIsDataToWrite Or bCheckCountListData Then
#If NRF Then
                objAppContainer.objExportDataManager.CreateCLO(SMTransactDataManager.ExFileType.EXData)
#End If
            Else
                objAppContainer.objLogger.WriteAppLog("No ExportData to be written", Logger.LogLevel.INFO)
            End If

            If bCheckCountListData Then
                '#If NRF Then

                '                objAppContainer.objExportDataManager.CreateCLO(SMTransactDataManager.ExFileType.EXData)
                '#End If
                '                If bCheckCountListData Then
                For Each sKey As String In objAppContainer.objGlobalCreateCountList.Keys
#If NRF Then
                objAppContainer.objExportDataManager.CreateCLA(SMTransactDataManager.ExFileType.EXData, Macros.START_USER_COUNTLIST)
#End If
                'Iterates through the Product list to write data for each Product Info


                    objCOLArray = objAppContainer.objGlobalCreateCountList.Item(sKey)
                    strSeq = "000"
                    For Each objCLProductInfo As CLProductInfo In objCOLArray
                        Dim objCLDRecord As CLDRecord = New CLDRecord()
                        strSeq = strSeq + 1
                        objCLDRecord.strBootsCode = objCLProductInfo.BootsCode
                        objCLDRecord.strSequence = strSeq
                        objCLDRecord.cSitetype = objCLProductInfo.CreatedLocation
                        objAppContainer.objExportDataManager.CreateCLD(objCLDRecord)

                        'Writes the CLC record to signify Create own List Data entered


                        'Sets the values
                        If objCLProductInfo.IsSFItemCounted Then
                            objCLCRecord = New CLCRecord()
                            objCLCRecord.strNumberSEQ = strSeq
                            If objCLProductInfo.IsUnknownItem Then
                                objCLCRecord.strBootscode = "0000000"
                            Else
                                objCLCRecord.strBootscode = objCLProductInfo.BootsCode
                            End If
                            objCLCRecord.strCount = objCLProductInfo.SalesFloorQuantity
                            objCLCRecord.strCountLocation = Macros.SHOP_FLOOR
                            objCLCRecord.strUpdateOSSR = " "
                            objCLCRecord.strSalesAtTimeOfUpload = "XXXXXXXXX"
                            objAppContainer.objExportDataManager.CreateCLC(objCLCRecord, SMTransactDataManager.ExFileType.EXData)
                        End If
                        If objCLProductInfo.IsMBSItemCounted Then
                            objCLCRecord = New CLCRecord()
                            objCLCRecord.strNumberSEQ = strSeq
                            If objCLProductInfo.IsUnknownItem Then
                                objCLCRecord.strBootscode = "0000000"
                            Else
                                objCLCRecord.strBootscode = objCLProductInfo.BootsCode
                            End If
                            objCLCRecord.strCountLocation = Macros.BACK_SHOP
                            objCLCRecord.strCount = objCLProductInfo.BackShopMBSQuantity
                            objCLCRecord.strUpdateOSSR = " "
                            objCLCRecord.strSalesAtTimeOfUpload = "XXXXXXXXX"
                            objAppContainer.objExportDataManager.CreateCLC(objCLCRecord, SMTransactDataManager.ExFileType.EXData)
                        End If
                        If objCLProductInfo.IsBSPSPItemCounted Then
                            objCLCRecord = New CLCRecord()
                            objCLCRecord.strNumberSEQ = strSeq
                            objCLCRecord.strBootscode = objCLProductInfo.BootsCode
                            objCLCRecord.strCountLocation = Macros.PSP
                            objCLCRecord.strCount = objCLProductInfo.BackShopPSPQuantity
                            objCLCRecord.strUpdateOSSR = " "
                            objCLCRecord.strSalesAtTimeOfUpload = "XXXXXXXXX"
                            objAppContainer.objExportDataManager.CreateCLC(objCLCRecord, SMTransactDataManager.ExFileType.EXData)
                        End If
                        If objCLProductInfo.NumSELsQueued > 0 Then
                            'Integration Testing
                            iNumSELs += objCLProductInfo.NumSELsQueued
                            Dim iCount As Integer = 0
                            For iCount = 0 To objCLProductInfo.NumSELsQueued - 1
                                'Write PRT record for the SEL requests Queued
                                Dim objPRTRecord As PRTRecord = New PRTRecord()
                                'Sets the values
                                objPRTRecord.strBootscode = objCLProductInfo.BootsCode
                                objPRTRecord.cIsMethod = Macros.PRINT_BATCH
                                'objAppContainer.objExportDataManager.CreatePRT(objPRTRecord.strBootscode, _
                                '                                       SMTransactDataManager.ExFileType.EXData)
                                arrQueuedSELs.Add(objPRTRecord)
                            Next
                        End If
                    Next

                    'Writes the CLA record to Signify Create own List End. 
#If NRF Then
                    objAppContainer.objExportDataManager.CreateCLA(SMTransactDataManager.ExFileType.EXData, Macros.END_USER_COUNTLIST)
#End If

                    Dim objCLXRecord As CLXRecord = New CLXRecord()
                    objCLXRecord.cIsCommit = Macros.CLX_COMMIT_YES
                    objCLXRecord.strCountType = Macros.USER_COUNT_LIST
                    objAppContainer.objExportDataManager.CreateCLX(objCLXRecord, SMTransactDataManager.ExFileType.EXData)
                Next
                objAppContainer.objGlobalCreateCountList.Clear()
            End If

            'Writes Export data for count list
            If bIsDataToWrite Then
                'Writes the CLO record to signify Count Lists Start
                '#If NRF Then

                '                objAppContainer.objExportDataManager.CreateCLO(SMTransactDataManager.ExFileType.EXData)
                '#End If

                Dim objCLProductInfoList As ArrayList = Nothing

                'Iterates through the Count lists to write data for each count list
                For Each objCountList As CLProductGroupInfo In objAppContainer.objGlobalCLProductGroupList

                    objCLProductInfoList = New ArrayList()

                    If objCountList.IsActive Then
                        'If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(objCountList.ListID) Then

                        'SFA DEF #804 - Send CLS to track the list in report
                        objCLSRecord = New CLSRecord()
                        objCLSRecord.strListID = objCountList.ListID
                        objAppContainer.objExportDataManager.CreateCLS(objCLSRecord, SMTransactDataManager.ExFileType.EXData)
                        'Obtains the product list for a particular list id
                        objCLProductInfoList = objAppContainer.objGlobalCLProductInfoTable.Item(objCountList.ListID)

                        'Iterates through the Product list to write data for each Product Info
                        For Each objCLProductInfo As CLProductInfo In objCLProductInfoList

                            'Checks if the particular item is counted atleast in one location
                            If (Not objCLProductInfo.IsSFItemCounted) And (Not objCLProductInfo.IsMBSItemCounted) And (Not objCLProductInfo.IsBSPSPItemCounted) Then
                                objAppContainer.objLogger.WriteAppLog("Not counted in any location. ProductCode is:" + objCLProductInfo.ProductCode, Logger.LogLevel.INFO)
                            Else
                                'Writes the CLC record to signify Count List Item Data entered

                                'Send CLC forSales floor
                                If objCLProductInfo.IsSFItemCounted Then
                                    objCLCRecord = New CLCRecord()
                                    'Sets the values
                                    objCLCRecord.strListID = objCountList.ListID
                                    objCLCRecord.strNumberSEQ = objCLProductInfo.SequenceNumber
                                    objCLCRecord.strBootscode = objCLProductInfo.BootsCode
                                    objCLCRecord.strCountLocation = Macros.SHOP_FLOOR
                                    objCLCRecord.strCount = objCLProductInfo.SalesFloorQuantity
                                    objCLCRecord.strUpdateOSSR = " "
                                    objCLCRecord.strSalesAtTimeOfUpload = objCLProductInfo.SalesAtPODDock
                                    objAppContainer.objExportDataManager.CreateCLC(objCLCRecord, SMTransactDataManager.ExFileType.EXData)
                                End If

                                'Send CLC for Back Shop
                                If objCLProductInfo.IsMBSItemCounted Then
                                    objCLCRecord = New CLCRecord()
                                    'Sets the values
                                    objCLCRecord.strListID = objCountList.ListID
                                    objCLCRecord.strNumberSEQ = objCLProductInfo.SequenceNumber
                                    objCLCRecord.strBootscode = objCLProductInfo.BootsCode
                                    objCLCRecord.strCountLocation = Macros.BACK_SHOP
                                    objCLCRecord.strCount = objCLProductInfo.BackShopMBSQuantity
                                    objCLCRecord.strUpdateOSSR = " "
                                    objCLCRecord.strSalesAtTimeOfUpload = "XXXXXXXXX"
                                    objAppContainer.objExportDataManager.CreateCLC(objCLCRecord, SMTransactDataManager.ExFileType.EXData)
                                End If

                                If objCLProductInfo.IsBSPSPItemCounted Then
                                    'Send CLC for BS PSP
                                    objCLCRecord = New CLCRecord()
                                    'Sets the values
                                    objCLCRecord.strListID = objCountList.ListID
                                    objCLCRecord.strNumberSEQ = objCLProductInfo.SequenceNumber
                                    objCLCRecord.strBootscode = objCLProductInfo.BootsCode
                                    objCLCRecord.strCountLocation = Macros.PSP
                                    objCLCRecord.strCount = objCLProductInfo.BackShopPSPQuantity
                                    objCLCRecord.strUpdateOSSR = " "
                                    objCLCRecord.strSalesAtTimeOfUpload = "XXXXXXXXX"
                                    objAppContainer.objExportDataManager.CreateCLC(objCLCRecord, SMTransactDataManager.ExFileType.EXData)
                                End If

                            End If

                            If objCLProductInfo.NumSELsQueued > 0 Then
                                'Integration Testing
                                iNumSELs += objCLProductInfo.NumSELsQueued
                                'm_CLSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing PRT")
                                Dim iCount As Integer = 0

                                For iCount = 0 To objCLProductInfo.NumSELsQueued - 1
                                    'Write PRT record for the SEL requests Queued
                                    Dim objPRTRecord As PRTRecord = New PRTRecord()

                                    'Sets the values
                                    'objPRTRecord.strBootscode = objAppContainer.objHelper.GeneratePCwithCDV(objCLProductInfo.BootsCode)
                                    objPRTRecord.strBootscode = objCLProductInfo.BootsCode
                                    objPRTRecord.cIsMethod = Macros.PRINT_BATCH

                                    'm_CLSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing CLX")
                                    'Writes the CLX record to Signify Count List Exit. 
                                    'It is specified whether Count List is Locked for the day or Not


                                    'objAppContainer.objExportDataManager.CreatePRT(objPRTRecord.strBootscode, _
                                    '                                   SMTransactDataManager.ExFileType.EXData)
                                    arrQueuedSELs.Add(objPRTRecord)
                                Next
                            End If
                        Next
                        'm_CLSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing CLX")
                        'Writes the CLX record to Signify Count List Exit. 
                        'It is specified whether Count List is Locked for the day or Not
                        Dim objCLXRecord As CLXRecord = New CLXRecord()
                        objCLXRecord.strListID = objCountList.ListID
                        If objCountList.IsUpdate Then ' And bLockList Then
                            objCLXRecord.cIsCommit = Macros.CLX_COMMIT_YES
                        Else
                            objCLXRecord.cIsCommit = Macros.CLX_COMMIT_NO
                        End If
                        If objCountList.ListType.Equals("H") Then
                            objCLXRecord.strCountType = "H"
                        ElseIf objCountList.ListType.Equals("R") Then
                            objCLXRecord.strCountType = "R"
                        ElseIf objCountList.ListType.Equals("U") Then
                            objCLXRecord.strCountType = "U"
                        ElseIf objCountList.ListType.Equals("N") Then
                            objCLXRecord.strCountType = "N"
                        End If
                        objAppContainer.objExportDataManager.CreateCLX(objCLXRecord, SMTransactDataManager.ExFileType.EXData)

                    End If
                Next
            End If
            If bIsDataToWrite Or bCheckCountListData Then
                'Writes the CLF record to Signify Count List Finished. 
                objAppContainer.objExportDataManager.CreateCLF(SMTransactDataManager.ExFileType.EXData)
            End If
            'Integration testing
            'Fix for IT bug BOOTS00000902
            'write Price check records if any
            'm_ModulePriceCheck.WriteExportData(arrQueuedSELs)

            'To avoid writing duplicate record in export data file when export data download
            'while logging off fails.
            objAppContainer.objGlobalCLProductGroupList.Clear()
            objAppContainer.objGlobalCLProductInfoTable.Clear()
            'Stock File Accuracy -  clearing global varibles
            objAppContainer.objGlobalCLInfoTable.Clear()
            objAppContainer.objGlobalCLSiteInfoTable.Clear()
            'Delete the temp file after writing export data
            objAppContainer.objExportDataManager.DeleteTempFiles(SMTransactDataManager.ExFileType.CLTemp)
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured while processing WriteExportData of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit WriteExportData of CLSessionMgr", Logger.LogLevel.INFO)
    End Function
    Public Class CLProductCountedData
        Public m_strProductCode As String
        Public m_strListId As String
        'Public m_iCountedLocation As Integer
        'Public m_bIsCounted As Boolean
    End Class
    Public Class CLMultiSiteInfo
        Public strSeqNumber As String
        Public strPOGDescription As String
        Public strSalesFloorQuantiy As String
        Public strBackShopQuantiy As String
        Public strOSSRQuantiy As String
        Public IsCounted As String
        Public strPlannerDesc As String
        Public strPlannerID As String
        Public iItemCount As Integer
        Public iRepeatCount As Integer
        Public strModuleID As String
        Public iSiteCount As Integer
        Public strModuleDesc As String
        Public test As String
    End Class
    ''' <summary>
    ''' Enum Class that defines all screens for Count List module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum CLSCREENS
        Home
        LocationSelection
        ItemDetails
        SalesFloorProductCount
        BackShopProductCount
        'ambli
        'For OSSR
        OSSRProductCount
        Finish
        Summary
        'Stock File Accuracy  - Added new screens
        SiteInformation
        ViewListScreen
        MultiSiteDiscrepancy
        ItemSummary
        FullPriceCheck
        'Added as part of SFA - For Create Own list
        COLLocationSelection
        COLItemScan
        COLItemDetails
        ViewCOLListScreen
    End Enum
    ''' <summary>
    ''' enumerates all scanable fields in Count List Module
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum SCNFIELDS
        ProductCode
        SELCode
    End Enum
#Region "OSSR"
    'ambli
    'For OSSR
#If RF Then
    'ambli
    'For OSSR

    Public Sub DisplayOSSRToggle(ByRef lblToggleOSSR As Label, ByVal strBarcode As String)
        'Dim objENQ As ENQRecord = New ENQRecord()
        Dim bCurrentOSSR_FLAG As Boolean = False
        Dim bResponse As Boolean = False

        If lblToggleOSSR.Text = "" Then
            bCurrentOSSR_FLAG = False
        Else
            bCurrentOSSR_FLAG = True
        End If
        'objAppContainer.objExportDataManager.CreateENQ(objENQ)
        bResponse = objAppContainer.objExportDataManager.CreateENQ_ToggleOSSR(strBarcode, Not (bCurrentOSSR_FLAG))

        If (bResponse) Then
            If Not (bCurrentOSSR_FLAG) Then
                lblToggleOSSR.Text = "OSSR"
                MessageBox.Show("The location setting for this item has been changed to 'OSSR'.", "Info", _
                       MessageBoxButtons.OK, _
                       MessageBoxIcon.Asterisk, _
                       MessageBoxDefaultButton.Button1)

            Else
                lblToggleOSSR.Text = ""
                MessageBox.Show("The location setting for this item has been changed to 'Back Shop'.", "Info", _
                MessageBoxButtons.OK, _
                MessageBoxIcon.Asterisk, _
                MessageBoxDefaultButton.Button1)
            End If
        Else
            MessageBox.Show("Unable to Toggle OSSR Status.", "Info ", _
              MessageBoxButtons.OK, _
                MessageBoxIcon.Asterisk, _
                MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub DisplayCLOSSRProductCount(ByVal o As Object, ByVal e As EventArgs)
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim objCurrentCountListInfoTable As Hashtable = New Hashtable()
        Dim arrProductList As New ArrayList()
        Dim arrMultiSiteList As New ArrayList()
        Dim objMultiSite As New CLMultiSiteInfo()
        Dim objSiteInfo As New CLMultiSiteInfo()
        Dim objCurrentProductInfo As New CLProductInfo()
        Dim strBootsCode As String

        objAppContainer.objLogger.WriteAppLog("Entered DisplayCLOSSRProductCount of CLSessionMgr", Logger.LogLevel.INFO)
        Try
            'Gets the current product info from hash table based on the list id 
            'and the position of the item in the list as indicated by 
            'm_iProductListCount variable
            objCurrentProductInfo = New CLProductInfo()
            If m_bIsCreateOwnList Then
                If Not m_bIsNewList Then
                    If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                        objCurrentCountListInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                        objCurrentProductInfoList = objCurrentCountListInfoTable.Item(m_CLCurrentSiteInfo.strPlannerDesc)
                        objCurrentProductInfo = m_CLProductInfo
                    End If
                Else
                    If m_bNavigation Then
                        objCurrentProductInfoList = m_CLItemList
                        objCurrentProductInfo = m_CLItemList(m_iBackNextCount)
                    Else
                        objCurrentProductInfo = m_CLProductInfo
                        m_CLProductInfo.CreatedLocation = Macros.OSSR
                    End If
                End If
                m_ItemScreen = Nothing
                arrMultiSiteList = m_CLProductInfo.BSMultiSiteList
                If m_SelectedPOGSeqNum Is Nothing Then
                    m_SelectedPOGSeqNum = Macros.SELECT_INDEX_ZERO
                End If
            Else
                If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    objCurrentCountListInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                    objCurrentProductInfoList = objCurrentCountListInfoTable.Item(m_CLCurrentSiteInfo.strPlannerDesc)
                End If

                Dim objProductInfo As New CLProductInfo()
                objProductInfo = objCurrentProductInfoList.Item(m_iProductListCount)
                strBootsCode = objProductInfo.BootsCode

                If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                    arrProductList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                    For Each objCurrentProductInfo In arrProductList
                        If objCurrentProductInfo.BootsCode.Equals(strBootsCode) Then
                            Exit For
                        End If
                    Next

                End If
            End If
            'Sets the values
            Dim objDescriptionArray As ArrayList = New ArrayList()
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(objCurrentProductInfo.Description)

            Dim strProductCode As String = ""
            strProductCode = objAppContainer.objHelper.GeneratePCwithCDV(objCurrentProductInfo.ProductCode)

            With m_CLOSSRProductCount
                Dim iCount As Integer = 0
                If m_bIsCreateOwnList Then
                    .Text = "Create Count - BS"
                    If m_bPlannerFlag Then
                        iCount = m_CLItemList.Count - 1
                    Else
                        iCount = m_CLItemList.Count
                    End If
                    If m_CLItemList.Count = 0 Then
                        .custCtrlBtnBack.Visible = False
                    Else
                        .custCtrlBtnBack.Visible = True
                    End If
                    If Not m_bIsNewList Then
                        .lblItemPosition.Text = (m_iProductListCount + 1).ToString() + "/" + objCurrentProductInfoList.Count.ToString()
                        .lblItemPosition.Visible = True
                    Else
                        'Disable controls for Create Own List
                        If m_bNavigation Then
                            .lblItemPosition.Text = (m_iBackNextCount + 1).ToString() + "/" + objCurrentProductInfoList.Count.ToString()
                            .lblItemPosition.Visible = True
                        Else
                            .lblItemPosition.Visible = False
                        End If
                    End If
                Else
                    .Text = "Count List - OS"
                    .lblItemPosition.Text = (m_iProductListCount + 1).ToString() + "/" + (objCurrentProductInfoList.Count).ToString()

                End If
                'If bIsItemCounted Then
                '    .lblCounted.Visible = True
                '    .lblCounted.Text = "COUNTED"
                'Else
                '    .lblCounted.Visible = False
                '    .lblCounted.Text = ""
                'End If
                .lblBootsCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(objCurrentProductInfo.BootsCode)

                .lblProductCodeDisplay.Text = objAppContainer.objHelper.FormatBarcode(strProductCode)
                .lblProductDesc1.Text = objDescriptionArray.Item(0)
                .lblProductDesc2.Text = objDescriptionArray.Item(1)
                .lblProductDesc3.Text = objDescriptionArray.Item(2)
    If objCurrentProductInfo.IsUnknownItem Then
                    .lblStatusDisplay.Text = "N/A"
                Else
                .lblStatusDisplay.Text = objAppContainer.objHelper.GetStatusDescription(objCurrentProductInfo.Status)
    end if
                If objCurrentProductInfo.TSF.Substring(0, 1).Equals("-") Then
                    .lblTotalStockFileDisplay.ForeColor = Color.Red
                Else
                    .lblTotalStockFileDisplay.ForeColor = .lblStatusDisplay.ForeColor
                End If
                .lblTotalStockFileDisplay.Text = objCurrentProductInfo.TSF


                .lblTotalStockFile.Text = ""
                .lblTotalStockFile.Text = "Total Stock File"



                'If objCurrentProductInfo.BackShopQuantity < 0 Then
                '    .lblBackShopVal.Text = 0
                'Else
                '    .lblBackShopVal.Text = objCurrentProductInfo.BackShopQuantity
                'End If
                'If objCurrentProductInfo.SalesFloorQuantity < 0 Then
                '    .lblShelfQty.Text = 0
                'Else
                '    .lblShelfQty.Text = objCurrentProductInfo.SalesFloorQuantity
                'End If

                'Gets the back shop quantity to be displayed
                arrMultiSiteList = objCurrentProductInfo.OSSRMultiSiteList
                If m_bIsCreateOwnList Then
                    If objCurrentProductInfo.IsUnknownItem Then
                        .lblStatusDisplay.Text = "N/A"
                        m_SelectedPOGSeqNum = 0
                    End If
                End If
                'Enable Counted lable if item counted in all sites
                .lblCounted.Visible = False
                If objCurrentProductInfo.TotalOSSRSiteCount = arrMultiSiteList.Count Then
                    .lblCounted.Visible = True
                End If
                For Each objSiteInfo In arrMultiSiteList
                    If objSiteInfo.strSeqNumber.Equals(m_SelectedPOGSeqNum) Then
                        If objSiteInfo.strOSSRQuantiy < 0 Then
                            .lblShelfQty.Text = 0
                        Else
                            .lblShelfQty.Text = objSiteInfo.strOSSRQuantiy
                        End If
                        Exit For

                    End If
                Next

                'populating multi sites
                If arrMultiSiteList.Count = 1 Then
                    .lblSite.Visible = False
                    .cmbMultiSite.Visible = False
                Else
                    .lblSite.Visible = True
                    .cmbMultiSite.Visible = True
                    .cmbMultiSite.Items.Clear()
                    '.cmbMultiSite.Items.Add("Select")
                    For Each objMultiSite In arrMultiSiteList
                        .cmbMultiSite.Items.Add(objMultiSite.strPOGDescription)
                    Next
                    .cmbMultiSite.SelectedIndex = m_SelectedPOGSeqNum
                End If

                'Displays the total quantity
                If objCurrentProductInfo.TotalQuantity < 0 Then
                    .lblTotalItemCountDisplay.Text = 0
                Else
                    .lblTotalItemCountDisplay.Text = objCurrentProductInfo.TotalQuantity
                End If

                '.lblItemPosition.Text = (m_iProductListCount + 1).ToString() + "/" + (objCurrentProductInfoList.Count).ToString()


                'Sets the store id and active data time to the status bar
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayCLOSSRProductCount of CLSessionMgr. Exception is:" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayCLOSSRProductCount of CLSessionMgr", Logger.LogLevel.INFO)

    End Sub
#End If
    'ambli
    'For OSSR
    'Property to store the number of items counted in OSSR
    Private m_iOSSRItemCount As Integer

    ''' <summary>
    ''' Getter and setter for the property m_iOSSRItemCount
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property GetSetOSSRItemCount() As Integer
        Get
            Return m_iOSSRItemCount
        End Get
        Set(ByVal value As Integer)
            m_iOSSRItemCount = value
        End Set
    End Property

    'ambli
    'For OSSR
    ''' <summary>
    ''' Processes the selection of OSSR button click on location selection screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessOSSRSelect()
#If RF Then
        'Stock File Accuracy  updated for OSSR location selection changes
        objAppContainer.objLogger.WriteAppLog("Entered ProcessOSSRSelect of CLSessionMgr", Logger.LogLevel.INFO)
        Dim strPlannerDesc As String = Nothing
        Dim strLocation As String = Nothing
        m_iProductListCount = 0
        m_EntryType = BCType.None
        If m_bIsCreateOwnList Then
            m_iCountedLocation = Macros.COUNT_OSSR
            If Not m_CLItemList.Count = 0 Then
                If m_iListCreatedLoc = Macros.COUNT_OSSR Then
                    m_bNavigation = True
                    m_bIsNewList = True
                    m_iBackNextCount = 0
                    m_bIsBackNext = True
                    DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation, Macros.BS_SELECTED_INDEX)
                Else
                    PopulateCOLSiteInfo()
                    If m_CLCurrentProductGroup.OSSRItemCount <> 0 Then
                        m_bNavigation = False
                        m_bIsNewList = False
                        If m_bISItemsInOSSRPSP Then
                            DisplayCLScreen(CLSessionMgr.CLSCREENS.SiteInformation, Macros.COUNT_OSSR)
                        Else
                            m_bIsSiteInfo = True  '&&Test
                            strPlannerDesc = Macros.COUNT_OSSR_BS 'SIT SFA
                            strLocation = strPlannerDesc
                            m_CLCurrentSiteInfo.strPlannerDesc = strPlannerDesc
                            DisplayCLScreen(CLSCREENS.ItemDetails, m_iCountedLocation)
                        End If
                    End If
                End If
            Else
                    m_bIsNewList = True
                    m_bNavigation = False
                    m_iListCreatedLoc = Macros.COUNT_OSSR
                    objCreateCountList.SiteType = Macros.OSSR
                    DisplayCLScreen(CLSessionMgr.CLSCREENS.COLItemScan)
            End If
        Else
            If m_CLCurrentProductGroup.OSSRItemCount <> 0 Then
                If m_CLCurrentProductGroup.OSSRPSPCount = 0 Then
                    m_CLCurrentSiteInfo.strPlannerDesc = Macros.COUNT_OSSR_BS
                    CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.ItemDetails, Macros.COUNT_OSSR, Macros.SELECTED_INDEX)
                Else
                    'redirect to site information instead of itemdetails
                    CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.SiteInformation, Macros.COUNT_OSSR)
                End If
            End If
        End If

        objAppContainer.objLogger.WriteAppLog("Exit ProcessOSSRSelect of CLSessionMgr", Logger.LogLevel.INFO)
#End If
    End Sub
    'ambli
    'For OSSR

#If RF Then
    Private Function UpdateOSSRProductInfo(ByVal strProductCode As String, ByVal iOSSRQty As Integer, _
                                                ByVal strBootsCode As String) As Boolean

        objAppContainer.objLogger.WriteAppLog("Entered UpdateOSSRProductInfo of CLSessionMgr", Logger.LogLevel.INFO)
        Dim objCurrentProductInfoList As ArrayList = New ArrayList()
        Dim objCurrentCLSiteInfoTable As Hashtable = New Hashtable()
        Dim objCurrentSFSiteInfoList As ArrayList = New ArrayList()
        Dim objCurrentBSSiteInfoList As ArrayList = New ArrayList()
        Dim objCurrentOSSRSiteInfoList As ArrayList = New ArrayList()
        Dim objCurrentCountListInfoTable As New Hashtable()
        Dim objProductInfo As New CLProductInfo()
        Dim objCurrentProductList As New ArrayList()
        Dim objCurrentProductInfo As New CLProductInfo()
        Dim objSiteInfo As New CLMultiSiteInfo()
        Dim strPlannerDesc As String = Nothing
        Dim iItemIndex As Integer = -1
        Dim iSiteIndex As Integer = -1
        Dim iInfoIndex As Integer = -1
        Dim iSalesFloorVal As Integer = -1
        Dim iBackShopVal As Integer = -1
        Dim iOSSRVal As Integer = -1
        Dim iTotalSalesFloorQty As Integer = 0
        Dim iTotalBackShopQty As Integer = 0
        Dim iTotalOSSRQty As Integer = 0

        Try

            'Updates the counted item in site with "Y"
            If objAppContainer.objGlobalCLProductInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                objCurrentProductList = objAppContainer.objGlobalCLProductInfoTable.Item(m_CLCurrentProductGroup.ListID)
                For Each objCurrentProductInfo In objCurrentProductList
                    iItemIndex = iItemIndex + 1
                    If objCurrentProductInfo.BootsCode.Equals(strBootsCode) Then
                        objCurrentSFSiteInfoList = objCurrentProductInfo.SFMultiSiteList
#If NRF Then
                        objCurrentBSSiteInfoList = objCurrentProductInfo.BSMultiSiteList
#ElseIf RF Then
                        If objAppContainer.OSSRStoreFlag = "Y" Then
                            If objCurrentProductInfo.OSSRFlag = "O" Then
                                objCurrentOSSRSiteInfoList = objCurrentProductInfo.OSSRMultiSiteList
                            Else
                                objCurrentBSSiteInfoList = objCurrentProductInfo.BSMultiSiteList
                            End If
                        Else
                            objCurrentBSSiteInfoList = objCurrentProductInfo.BSMultiSiteList
                        End If
#End If
                        Exit For
                    End If
                Next

            End If


            For Each objSiteInfo In objCurrentOSSRSiteInfoList
                If objSiteInfo.strSeqNumber.Equals(m_SelectedPOGSeqNum) Then
                    If objSiteInfo.IsCounted.Equals("N") Then
                        objSiteInfo.strPOGDescription = "* " & objSiteInfo.strPOGDescription.Trim & " (Counted)"
                        objSiteInfo.IsCounted = "Y"
                        objCurrentProductInfo.TotalOSSRSiteCount = objCurrentProductInfo.TotalOSSRSiteCount + 1
                    End If
                    objSiteInfo.strOSSRQuantiy = iOSSRQty
                    strPlannerDesc = objSiteInfo.strPlannerDesc
                    'Updating BS - MBS and  PSP quantity separately 
                    If m_SelectedPOGSeqNum = Macros.BS_SELECTED_INDEX Then
                        objCurrentProductInfo.OSSRMBSQuantity = iOSSRQty
                    Else
                        objCurrentProductInfo.OSSRPSPQuantity = iOSSRQty
                    End If
                    iSiteIndex = iSiteIndex + 1
                    Exit For
                End If
                iSiteIndex = iSiteIndex + 1
            Next

            'Updates objCurrentOSSRSiteInfoList with new count
            objCurrentOSSRSiteInfoList.RemoveAt(iSiteIndex)
            objCurrentOSSRSiteInfoList.Insert(iSiteIndex, objSiteInfo)
            objCurrentProductInfo.OSSRMultiSiteList = objCurrentOSSRSiteInfoList

            'Updates the count status of item in corresponding site
            If objAppContainer.objGlobalCLInfoTable.ContainsKey(m_CLCurrentProductGroup.ListID) Then
                objCurrentCountListInfoTable = objAppContainer.objGlobalCLInfoTable.Item(m_CLCurrentProductGroup.ListID)
                objCurrentProductInfoList = objCurrentCountListInfoTable.Item(strPlannerDesc)
            End If

            For Each objProductInfo In objCurrentProductInfoList
                If objProductInfo.BootsCode.Equals(strBootsCode) Then
                    objProductInfo.CountStatus = "Y"
                    iInfoIndex = iInfoIndex + 1
                    Exit For
                End If
                iInfoIndex = iInfoIndex + 1
            Next

            'retrieves the total count of item in all sites for SalesFloor
            If objCurrentSFSiteInfoList.Count() > 0 Then
                Dim iTempSFQty As Integer
                For Each objSiteInfo In objCurrentSFSiteInfoList
                    If objSiteInfo.strSalesFloorQuantiy < 0 Then
                        iTempSFQty = 0
                    Else
                        iTempSFQty = objSiteInfo.strSalesFloorQuantiy
                    End If
                    iTotalSalesFloorQty = iTotalSalesFloorQty + iTempSFQty
                Next
            End If

            'retrieves the total count of item in all sites for BackShop
            If objCurrentBSSiteInfoList.Count() > 0 Then
                Dim iTempBSQty As Integer
                For Each objSiteInfo In objCurrentBSSiteInfoList
                    If objSiteInfo.strBackShopQuantiy < 0 Then
                        iTempBSQty = 0
                    Else
                        iTempBSQty = objSiteInfo.strBackShopQuantiy
                    End If
                    iTotalBackShopQty = iTotalBackShopQty + iTempBSQty
                Next
            End If

            'update the ossr quantity to be displayed
            objCurrentProductInfo.OSSRQuantity = iTotalOSSRQty

#If RF Then
            'retrieves the total count of item in all sites for OSSR
            If objCurrentOSSRSiteInfoList.Count() > 0 Then
                Dim iTempOSSRQty As Integer
                For Each objSiteInfo In objCurrentOSSRSiteInfoList
                    If objSiteInfo.strOSSRQuantiy < 0 Then
                        iTempOSSRQty = 0
                    Else
                        iTempOSSRQty = objSiteInfo.strOSSRQuantiy
                    End If
                    iTotalOSSRQty = iTotalOSSRQty + iTempOSSRQty
                Next
            End If
            objCurrentProductInfo.TotalQuantity = iTotalSalesFloorQty + iTotalBackShopQty + iTotalOSSRQty
            'End if
#End If
#If NRF Then
            objCurrentProductInfo.TotalQuantity = iTotalSalesFloorQty + iTotalBackShopQty
#End If
            objCurrentProductList.RemoveAt(iItemIndex)
            objCurrentProductList.Insert(iItemIndex, objCurrentProductInfo)

            objAppContainer.objGlobalCLProductInfoTable.Remove(m_CLCurrentProductGroup.ListID)
            objAppContainer.objGlobalCLProductInfoTable.Add(m_CLCurrentProductGroup.ListID, objCurrentProductList)

            'Checks if the product is already counted in OSSR
            'If not counted then add the item to the Counted Items list
            Dim bIsProductAlreadyCounted As Boolean = False

            For Each objCountedProductData As CLProductCountedData In objAppContainer.m_CLOSSRCountedInfoList
                If objCountedProductData.m_strListId.Equals(m_CLCurrentProductGroup.ListID) AndAlso _
                objCountedProductData.m_strProductCode.Equals(objCurrentProductInfo.ProductCode) Then
                    bIsProductAlreadyCounted = True
                    Exit For
                End If
            Next

            'If m_CLSalesFloorProductCount.lblCounted.Visible AndAlso _
            'm_CLSalesFloorProductCount.lblCounted.Text.Equals("COUNTED") Then
            '    bIsProductAlreadyCounted = True
            'End If

            If Not bIsProductAlreadyCounted Then
                objAppContainer.objCLSummary.iOSSRListCounted += 1
                m_iOSSRItemCount = m_iOSSRItemCount + 1
                Dim objOSSRCountedProduct As CLProductCountedData = New CLProductCountedData()
                objOSSRCountedProduct.m_strProductCode = objCurrentProductInfo.ProductCode
                objOSSRCountedProduct.m_strListId = m_CLCurrentProductGroup.ListID

                objAppContainer.m_CLOSSRCountedInfoList.Add(objOSSRCountedProduct)
            End If

            If Not m_CLCurrentProductGroup.IsActive Then
                m_CLCurrentProductGroup.IsActive = True
            End If

#If RF Then
            'For Each objProductInfo As CLProductInfo In objCurrentProductInfoList
            '    Dim objCLCRecord As CLCRecord = New CLCRecord()
            '    'Sets the values
            '    objCLCRecord.strListID = m_CLCurrentProductGroup.ListID
            '    objCLCRecord.strNumberSEQ = objProductInfo.SequenceNumber
            '    objCLCRecord.strBootscode = objProductInfo.BootsCode
            '    objCLCRecord.strBackShopCount = objProductInfo.BackShopQuantity
            '    objCLCRecord.strSalesFloorCount = objProductInfo.SalesFloorQuantity
            '    objCLCRecord.strOssrCount = objProductInfo.OSSRQuantity
            '    objCLCRecord.strUpdateOssrItem = " "
            '    If Not objAppContainer.objExportDataManager.CreateCLC(objCLCRecord) Then
            '        objAppContainer.objLogger.WriteAppLog("Could not UpdateSalesFloorProductInfo of CLSessionMgr." _
            '                                     , Logger.LogLevel.RELEASE)
            '        Return False

            '    End If
            'Next


            Dim objCLCRecord As CLCRecord = New CLCRecord()
            'Sets the values
            objCLCRecord.strListID = m_CLCurrentProductGroup.ListID
            objCLCRecord.strNumberSEQ = objCurrentProductInfo.SequenceNumber
            objCLCRecord.strBootscode = objCurrentProductInfo.BootsCode
            If m_SelectedPOGSeqNum = Macros.BS_SELECTED_INDEX Then
                objCLCRecord.strCountLocation = Macros.OSSR
                objCLCRecord.strCount = objCurrentProductInfo.OSSRMBSQuantity
            Else
                objCLCRecord.strCountLocation = Macros.OSSR_PSP
                objCLCRecord.strCount = objCurrentProductInfo.OSSRPSPQuantity
            End If
            objCLCRecord.strUpdateOSSR = " "

            If Not objAppContainer.objExportDataManager.CreateCLC(objCLCRecord) Then
                objAppContainer.objLogger.WriteAppLog("Could not UpdateBackShopProductInfo of CLSessionMgr." _
                                             , Logger.LogLevel.RELEASE)
                Return False

            End If

#End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in UpdateOSSRProductInfo of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit UpdateOSSRProductInfo of CLSessionMgr", Logger.LogLevel.INFO)
        Return True

    End Function

    ''' <summary>
    ''' Processes the counting done on OSSR
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="iOSSRQty"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessOSSRItemCounted(ByVal strProductCode As String, ByVal iOSSRQty As Integer, _
                                           ByVal strBootsCode As String) As Boolean
        Try
            objAppContainer.objLogger.WriteAppLog("Entered ProcessOSSRItemCounted of CLSessionMgr", Logger.LogLevel.INFO)

            'To unformat the product code by removing "-" and then remove CDV from that value
            Dim strUnFormattedProductCode As String
            Dim strUnFormatBootsCode As String
            strUnFormattedProductCode = objAppContainer.objHelper.UnFormatBarcode(strProductCode)
            strUnFormattedProductCode = strUnFormattedProductCode.Remove(strUnFormattedProductCode.Length - 1, 1)
            strUnFormatBootsCode = objAppContainer.objHelper.UnFormatBarcode(strBootsCode)

            'Updates the list with modified data

            If (iOSSRQty = "0") Then
                If (m_EntryType = BCType.EAN) Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M51"), "Invalid Data", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Exclamation, _
                                    MessageBoxDefaultButton.Button2)
                    Return True
                Else
                    If UpdateOSSRProductInfo(strUnFormattedProductCode, iOSSRQty, strUnFormatBootsCode) Then
                        'Displays the OSSR Count screen
                        DisplayCLScreen(CLSCREENS.OSSRProductCount)
                    End If
                End If
            Else
                If UpdateOSSRProductInfo(strUnFormattedProductCode, iOSSRQty, strUnFormatBootsCode) Then
                    'Displays the OSSR Count screen
                    DisplayCLScreen(CLSCREENS.OSSRProductCount)
                End If
            End If


        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessOSSRItemCounted of CLSessionMgr. Exception is: " _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try

        objAppContainer.objLogger.WriteAppLog("Exit ProcessOSSRCounted of CLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
#End If
#End Region

End Class
''' <summary>
''' To store the details of the count list displayed in the summary screen.
''' </summary>
''' <remarks></remarks>
Public Class CLSummary
    Public iListCounted As Integer = 0
    Public iSFListCounted As Integer = 0
    Public iBSListCounted As Integer = 0
    'Stock File Accuracy added array to hold unique count lists counted
    Public strListIDArray As ArrayList = New ArrayList()
    Public strarrKey As New ArrayList()
    Public isDataToWrite As Boolean = False
    'ambli
    'For OSSR
    Public iOSSRListCounted As Integer = 0
End Class
