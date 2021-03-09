Imports System.Runtime.InteropServices
Imports Microsoft.Win32
'''****************************************************************************
''' <FileName>RLSessionMgr.vb</FileName>
''' <summary>
''' The Recall feature class which will 
''' intialise all the parameters with respect to Active Recalls.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''****************************************************************************
Public Class RLSessionMgr
    'Declaring the Form objects
    Private m_frmRLActiveRecallList As frmRLActiveRecallList
    Private m_frmRLDespatch As frmRLDespatch
    Public m_frmRLItemDetails As frmRLItemDetails
    Private m_frmRLItemListScreen As frmRLItemListScreen
    Public m_frmRLScan As frmRLScan
    Private m_frmRLScanUOD As frmRLScanUOD
    Private m_frmRLSummary As frmRLSummary
    'Recalls CR,Specail Instruction
    Private m_frmSpclInstruction As frmSpclInstructions
    'Declaring the objects that store items in a list
    Public Shared objRLSessionMgr As RLSessionMgr = Nothing
    Private m_RecallList As ArrayList = Nothing
    Private m_RecallItemList As Hashtable = Nothing
    Private m_ActionedItemList As Hashtable = Nothing
    Private m_ActionedItemListinPrevUODs As Hashtable = Nothing
    Public m_RecallCountForTL As Hashtable = Nothing
    'RECALLS CR

    Public StockCount As String = ""
    Private m_RecallStatus As Hashtable = Nothing
    Public m_DiscrepancyList As ArrayList = Nothing
    Private m_objRLItemInfo As GOItemInfo = Nothing
    'Private m_RecallInfo As RLRecallInfo = Nothing
    'Private m_RecallItemInfo As RLItemInfo = Nothing
    Private m_CurrentItem As RLItemInfo = Nothing
    Public m_CurrentList As RLRecallInfo = Nothing
    Public m_RecallDesc As String = ""
    'Tailoring
    Public bTailoring As Boolean = False
    Public bLoadRecalls As Boolean = False
    'Recall CR
    Public m_RecallCount As RecallCount = Nothing
    Public CallingScreen As RECALLSCREENS
    Public bIsDiscrepancy As Boolean = False
    'RECALLS DEFECT FIX
    Public m_TotalItemsInRecall As Integer = 0
    Public m_ActionedItemsInRecall As Integer = 0
    Public bCreateAnotherRecallUOD As Boolean = False
    Public bActionedRecall As Boolean = False
    'Stores the Transaction Data held within the session
    Public m_UODNumber As String = Nothing
    'For storing boolean value that denotes whether to show / hide return button.
    Private m_bIsReturnVisible As Boolean = False
    'Used for discrepancy item number starts from 1
    Public m_DiscItemNumber As Integer = 0
    'For Loading active recall list.
    Private m_Message As frmMessage
    'For Label Type for UOD scanning
    Public strLabelType As String = ""
    Public iMinReturnQty As Integer = 0
    Public bIsRecallStarted As Boolean = False
    Private m_DiscItemCount As Integer = 0
    Public strRecallDescription As String = ""
    Public bNoTailoredRtrnItem As Boolean = False
    Public bIsRecallReturns As Boolean = False
    Public bNoRecallsInList As Boolean = False
    Public bIsRCBSent As Boolean = False
    Public bNoTailoredItemsinReturns As Boolean = False
    Public bRecallActionCompleted As Boolean = False

    ''' <summary>
    ''' Constructor initiates data 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.RECALL
        Try
            'Instantiate all the objects required
            ' Me.StartSession()
        Catch ex As Exception
            'Handle Recall Init Exception here.
            Me.EndSession()
        End Try
    End Sub
    'Recalls CR
    'Beep sound
    <DllImport("coredll.dll")> _
Public Shared Function PlaySound(ByVal szSound As String, ByVal hModule As IntPtr, ByVal flags As Integer) As Integer
    End Function
    Public Enum PlaySoundFlags As Integer
        SND_SYNC = &H0
        ' play synchronously (default) 
        SND_ASYNC = &H1
        ' play asynchronously 
        SND_NODEFAULT = &H2
        ' silence (!default) if sound not found 
        SND_MEMORY = &H4
        ' pszSound points to a memory file 
        SND_LOOP = &H8
        ' loop the sound until next sndPlaySound 
        SND_NOSTOP = &H10
        ' don't stop any currently playing sound 
        SND_NOWAIT = &H2000
        ' don't wait if the driver is busy 
        SND_ALIAS = &H10000
        ' name is a registry alias 
        SND_ALIAS_ID = &H110000
        ' alias is a predefined ID 
        SND_FILENAME = &H20000
        ' name is file name 
        SND_RESOURCE = &H40004
        ' name is resource name or atom 
    End Enum

    Public Sub Beep()

        Play(ConfigDataMgr.GetInstance.GetParam(ConfigKey.BEEP_PATH).ToString())
    End Sub
    Public Shared Sub Play(ByVal fileName As String)
        Try
            PlaySound(fileName, IntPtr.Zero, CInt(PlaySoundFlags.SND_FILENAME Or PlaySoundFlags.SND_SYNC))
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at Playing Sound:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Function to make the class singleton
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As RLSessionMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.RECALL
        If objRLSessionMgr Is Nothing Then
            objRLSessionMgr = New RLSessionMgr
        End If
        Return objRLSessionMgr
    End Function
    ''' <summary>
    ''' Property to set recall status for the currently loaded list.
    ''' </summary>
    ''' <remarks></remarks>
    Private m_CurrentListStatus As String
    Public WriteOnly Property CurrentRecallListStatus() As String
        Set(ByVal value As String)
            m_CurrentListStatus = value
            'm_CurrentList.ListStatus = value
        End Set
    End Property
    ''' <summary>
    ''' Initialises the Active Recall Session 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function StartSession(ByVal bSessionStarted As Boolean) As Boolean
        Try
            If Not bSessionStarted Then
#If RF Then
                objAppContainer.bRecallConnection = True
                If Not objAppContainer.objExportDataManager.CreateRCA() Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M50"), "Info", _
                                               MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                               MessageBoxDefaultButton.Button1)
                    objAppContainer.bRecallConnection = False
                    Return False
                Else
                    objAppContainer.bRecallConnection = False
                    bIsRCBSent = False
                End If
#End If
                'All the Active Recall related forms are instantiated.
                m_frmRLActiveRecallList = New frmRLActiveRecallList()
                m_frmRLDespatch = New frmRLDespatch()
                m_frmRLItemDetails = New frmRLItemDetails()
                m_frmRLItemListScreen = New frmRLItemListScreen()
                m_frmRLScan = New frmRLScan()
                m_frmRLScanUOD = New frmRLScanUOD()
                m_frmRLSummary = New frmRLSummary()
                'Recalls CR,Specail Instruction
                m_frmSpclInstruction = New frmSpclInstructions()
                'Instantiating a class to store the Recall Item  objects
                m_RecallList = New ArrayList
                m_RecallItemList = New Hashtable
                m_ActionedItemList = New Hashtable
                m_ActionedItemListinPrevUODs = New Hashtable
                'RECALLS CR - to hold Recall Status
                m_RecallStatus = New Hashtable
                m_DiscrepancyList = New ArrayList
                'For loading active recall list
                m_Message = New frmMessage()
                bIsRecallStarted = True
                bCreateAnotherRecallUOD = False
                bRecallActionCompleted = False
                Return True
            Else
                Return True
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " occured at RLSessionMgr:StartSession() ", Logger.LogLevel.INFO)
#If RF Then
            If ex.Message = Macros.CONNECTION_REGAINED Or ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Then
                Throw ex
            End If
            If ex.Message = Macros.CONNECTIVITY_TIMEOUTCANCEL And objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.RECALL Then
                Throw ex
            End If
#End If
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Updates the status bar message in all forms
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBar()
        Try
            m_frmRLActiveRecallList.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_frmRLDespatch.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_frmRLItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_frmRLItemListScreen.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_frmRLScan.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_frmRLScanUOD.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_frmRLSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Eoccured @: " + ex.StackTrace, Logger.LogLevel.INFO)
        End Try
    End Sub
    ''' <summary>
    ''' Send RCB message when Quit option is selected.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SendRecallExitForQuit()
        Try
            Dim objRCB As RCBRecord = New RCBRecord()
            objRCB.strnumUOD = "00000000000000"
            objRCB.strRecallref = m_CurrentList.RecallNumber
            objRCB.strStateCall = "X"
            objAppContainer.objExportDataManager.CreateRCB(objRCB)
            'Following this send a recall session start message so that it will fetch the items
            objAppContainer.objExportDataManager.CreateRCA()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Eoccured @: " + ex.StackTrace, Logger.LogLevel.INFO)
        End Try
    End Sub
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by RLSessionMgr.
    ''' </summary>
    ''' <param name="bIsReconnect">Parameter to mention if EndSession function is not invoked
    ''' automatically inorder to perform the recall type check. If the EndSession is invoked
    ''' due to connection loss, reconnect or time out then don't perform the check for recall type as it 
    ''' will not dispose the forms and might cause problems.</param>
    ''' <returns>True if all the forms are cleared graefully.</returns>
    ''' <remarks></remarks>
    Public Function EndSession() As Boolean
        'Public Function EndSession(Optional ByVal bIsReconnect As Boolean = False) As Boolean
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered EndSession of  RLSessionMgr", _
                                              Logger.LogLevel.INFO)
        Try
            'Check if the recall type choosen is in Emergency / 100% returns / Withdrawn
            'and restrict the user not to action the list partially.
            'If (m_CurrentList.RecallType.Equals(Macros.RECALL_EMERGENCY) Or _
            '   m_CurrentList.RecallType.Equals(Macros.RECALL_WITHDRAWN) Or _
            '   m_CurrentList.RecallType.Equals(Macros.RECALL_100RETURN)) Then
            '    If m_CurrentList.ListStatus = Macros.RECALL_ITEM_UNACTIONED Then
            '        Return False
            '    End If
            '    'Return False
            'End If
            objAppContainer.bRecallStarted = False
            'Close and Dispose all forms.
            m_frmRLActiveRecallList.Close()
            m_frmRLActiveRecallList.Dispose()

            m_frmRLScanUOD.Close()
            m_frmRLScanUOD.Dispose()

            m_frmRLDespatch.Close()
            m_frmRLDespatch.Dispose()

            m_frmRLItemDetails.Close()
            m_frmRLItemDetails.Dispose()

            m_frmRLItemListScreen.Close()
            m_frmRLItemListScreen.Dispose()

            m_frmRLScan.Close()
            m_frmRLScan.Dispose()

            m_frmRLSummary.Close()
            m_frmRLSummary.Dispose()
            'Recalls CR
            m_frmSpclInstruction.Close()
            m_frmSpclInstruction.Dispose()
            'For loading active recall list
            m_Message.Close()
            m_Message.Dispose()

            'Release all objects and Set to nothig.
            m_RecallList = Nothing
            m_RecallItemList = Nothing
            'Recalls CR
            m_RecallStatus = Nothing
            m_RecallCount = Nothing
            'm_RecallInfo = Nothing
            'm_RecallItemInfo = Nothing
            m_UODNumber = Nothing
            m_ActionedItemList = Nothing
            m_ActionedItemListinPrevUODs = Nothing
            m_DiscrepancyList = Nothing
            m_CurrentItem = Nothing
            m_CurrentList = Nothing
            CallingScreen = Nothing
            m_DiscItemNumber = Nothing

            objRLSessionMgr = Nothing
            'Recalls Cr
            'Create Recalls
            objAppContainer.bIsCreateRecalls = False
#If RF Then
            objAppContainer.bIsActiveRecallListSCreen = False
#End If
            bIsRecallStarted = False
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in EndSession of  RLSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit EndSession of  RLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    'To get Recall Type
    'Recalls Cr
    'Create Recalls
    Public Sub GetRecallType(ByVal strItemStatus0 As String, ByVal strItemStatus8 As String, ByRef cRecallType As Char)
        'Dim itemStatus0 As Integer
        'Dim itemStatus8 As Integer
        'itemStatus0 = Convert.ToDecimal(strItemStatus0)
        'itemStatus8 = Convert.ToDecimal(strItemStatus8)
        'itemStatus0 = Convert.ToString(itemStatus0, 2)
        'itemStatus8 = Convert.ToString(itemStatus8, 2)
        'strItemStatus0 = itemStatus0.ToString().PadLeft(8, "0")
        'strItemStatus8 = itemStatus8.ToString().PadLeft(8, "0")
        ' IRF INDICATOR 0 - 5th bit from right = strItemStatus0.Substring(3, 1)
        ' IRF INDICATOR 8 - 6th Bit from Right = strItemStatus8.Substring(2, 1)
        ' IRF INDICATOR 8 - 7th Bit from Right = strItemStatus8.Substring(1, 1)

        If strItemStatus0.Substring(3, 1) = "1" And strItemStatus8.Substring(2, 1) = "0" And strItemStatus8.Substring(1, 1) = "0" Then
            cRecallType = "E"
        ElseIf strItemStatus0.Substring(3, 1) = "1" And strItemStatus8.Substring(2, 1) = "1" And strItemStatus8.Substring(1, 1) = "0" Then
            cRecallType = "W"
        ElseIf strItemStatus0.Substring(3, 1) = "1" And strItemStatus8.Substring(2, 1) = "0" And strItemStatus8.Substring(1, 1) = "1" Then
            cRecallType = "R"
        ElseIf strItemStatus8.Substring(2, 1) = "1" And strItemStatus8.Substring(1, 1) = "1" Then
            cRecallType = "P"
        Else
            cRecallType = " "
        End If

    End Sub
    ''' <summary>
    ''' The Method handles the scan data returned form the barcode scanner.
    ''' This method implements the business logic to populate the data to the corresponding
    ''' UI element after validation.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="Type"></param>
    ''' <remarks></remarks>
    Public Sub HandleScanData(ByVal strBarcode As String, ByVal Type As BCType)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered HandleScanData of  RLSessionMgr", Logger.LogLevel.INFO)
        Dim strBootsCode As String = ""
        Try
            Select Case Type
                Case BCType.SEL
                    'Change to handle SEL scanning and clearance label scanning.
                    If strBarcode.StartsWith("8270") And (strBarcode.Length > 12) Then
                        strBootsCode = strBarcode.Substring(4, 6)
                    ElseIf objAppContainer.objHelper.ValidateSEL(strBarcode) Then
                        objAppContainer.objHelper.GetBootsCodeFromSEL(strBarcode, strBootsCode)
                    End If
                    strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
                    'call function to process the scanned item.
                    'Set manual entry flag to true to handle the boots code accepted by scanning.
                    RLSessionMgr.GetInstance().ProcessScanItem(strBootsCode)
                Case BCType.EAN
                    RLSessionMgr.GetInstance().ProcessScanItem(strBarcode)
                Case BCType.UPC
                    RLSessionMgr.GetInstance().ProcessScanItem(strBarcode)
                Case BCType.ManualEntry
                    'strBarcode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                    RLSessionMgr.GetInstance().ProcessScanItem(strBarcode)
                Case BCType.UOD
                    RLSessionMgr.GetInstance().ProcessScanUOD(strBarcode, False)
                Case BCType.UODManualEntry
                    RLSessionMgr.GetInstance().ProcessScanUOD(strBarcode, True)
            End Select
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in HandleScanData of  RLSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit HandleScanData of  RLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Screen Display method for Active Recall. 
    ''' All Recall sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName">Enum SMSCREENS</param>
    ''' <returns>True if display is sucess else False</returns>
    ''' <remarks></remarks>
    Public Function DisplayRecallScreen(ByVal ScreenName As RECALLSCREENS)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayRecallScreen of  RLSessionMgr", Logger.LogLevel.INFO)
        Try
            Select Case ScreenName
                Case RECALLSCREENS.ActiveRecallList
                    m_frmRLActiveRecallList.Invoke(New EventHandler(AddressOf DisplayActiveRecallList))
                Case RECALLSCREENS.Despatch
                    m_frmRLDespatch.Invoke(New EventHandler(AddressOf DisplayDespatch))
                Case RECALLSCREENS.ItemDetails
                    m_frmRLItemDetails.Invoke(New EventHandler(AddressOf DisplayItemDetails))
                Case RECALLSCREENS.ItemList
                    m_frmRLItemListScreen.Invoke(New EventHandler(AddressOf DisplayItemList))
                Case RECALLSCREENS.Scan
                    m_frmRLScan.Invoke(New EventHandler(AddressOf DisplayScan))
                Case RECALLSCREENS.ScanUOD
                    m_frmRLScanUOD.Invoke(New EventHandler(AddressOf DisplayScanUOD))
                Case RECALLSCREENS.Summary
                    m_frmRLSummary.Invoke(New EventHandler(AddressOf DisplaySummary))
                Case RECALLSCREENS.Discrepancy
                    m_frmRLItemDetails.Invoke(New EventHandler(AddressOf DisplayDiscrepancy))
                    'Recalls CR,Special Instructions
                Case RECALLSCREENS.SpclInstructions
                    m_frmSpclInstruction.Invoke(New EventHandler(AddressOf DisplaySpclInstruction))
                Case RECALLSCREENS.Message
                    m_Message.Invoke(New EventHandler(AddressOf DisplayMessage))
            End Select
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in DisplayRecallScreen of  RLSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayRecallScreen of  RLSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Populate and display the Product Scan screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayActiveRecallList(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayActiveRecallList of  RLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim objComparer As New SortComparer
            'for Recall List Comparison
            Dim objStatusComparer As New RecallStatusComparer
            'System Testing - Lakshmi
            Dim strRecallStatus As String = "Unactioned"
            Dim strRecallType As String = Nothing
            Dim strRecallName As String = "100% Returns"
            Dim iQuantity As Integer = 0
            Select Case WorkflowMgr.GetInstance.objActiveFeature
                'Recall CR
                Case WorkflowMgr.ACTIVEFEATURE.CUSTOMEREMERGENCY
                    strRecallType = "E"
                    strRecallName = "Customer/Emergency"
                Case WorkflowMgr.ACTIVEFEATURE.WITHDRAWN
                    strRecallType = "W"
                    strRecallName = "Withdrawn"
                Case WorkflowMgr.ACTIVEFEATURE.RECALLRETURNS
                    strRecallType = "R"
                    strRecallName = "100% Returns"
                Case WorkflowMgr.ACTIVEFEATURE.EXCESSSALESPLAN
                    strRecallType = "C"
                    strRecallName = "Excess Salesplan"
                Case WorkflowMgr.ACTIVEFEATURE.PLANNERLEAVER
                    strRecallType = "I"
                    strRecallName = "Planner Leaver"
            End Select
            'While loading the recall list clear all lists and list items in case of RF mode.
            m_RecallItemList.Clear()
            m_RecallItemList = New Hashtable()
            m_ActionedItemList.Clear()
            m_ActionedItemList = New Hashtable()
            m_ActionedItemsInRecall = 0
            m_ActionedItemListinPrevUODs.Clear()
            m_ActionedItemListinPrevUODs = New Hashtable()
            m_RecallCountForTL = New Hashtable()
            m_RecallCountForTL.Clear()
            bIsDiscrepancy = False  'Fix for 4807: Issue where scan UOD screen is displayed after entering recall count.
            objAppContainer.objLogger.WriteAppLog("Loading Active Recall", Logger.LogLevel.RELEASE)
            'Set the column width for MC55 device.
            frmRLActiveRecallList.ColumnHeader1.Width = 60 * objAppContainer.iOffSet
            frmRLActiveRecallList.ColumnHeader2.Width = 150 * objAppContainer.iOffSet
            frmRLActiveRecallList.ColumnHeader3.Width = 50 * objAppContainer.iOffSet
            frmRLActiveRecallList.ColumnHeader4.Width = 80 * objAppContainer.iOffSet
            With frmRLActiveRecallList
                .lblTitle.Text = "The following " + strRecallName + " Recalls are ready for completion"
                'if m_Recalllist count is 0 then reload the list
                If m_RecallList.Count < 1 Then
                    If objAppContainer.objDataEngine.GetRecallList(m_RecallList, m_RecallCount) Then
                        'm_RecallList.Sort(0, m_RecallList.Count, objStatusComparer)
                        'Load the Recall List to list view
                        .lvRecallList.Items.Clear()
                        For Each objRecallInfo As RLRecallInfo In m_RecallList
                            Dim objListItem As ListViewItem
                            If objRecallInfo.RecallType = strRecallType Then
                                If strRecallName = "100% Returns" Then
                                    bIsRecallReturns = True
                                Else
                                    bIsRecallReturns = False
                                End If

                                objListItem = New ListViewItem(objRecallInfo.RecallNumber)
                                objListItem.SubItems.Add(objRecallInfo.RecallDescription)
                                iQuantity = CInt(objRecallInfo.RecallQuantity)
                                'Tailoring
                                If objRecallInfo.Tailored Then
                                    objListItem.SubItems.Add("TL")
                                    'Store the quantity in a hastable
                                    m_RecallCountForTL.Add(objRecallInfo.RecallNumber, iQuantity)
                                Else
                                    objListItem.SubItems.Add(iQuantity.ToString())
                                End If
                                'objListItem.SubItems.Add(objRecallInfo.ActiveDate)

                                objListItem.SubItems.Add(objRecallInfo.ListStatus)

                                'For Label Type for UOD scanning
                                .lvRecallList.Items.Add(objListItem)

                            End If
                        Next

                    Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M50"), "Info", _
                                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
                        WorkflowMgr.GetInstance.ExecPrev()
                        Exit Sub
                    End If
                Else
                    ' Sort Recall List according to Recall Number and Recall Status
                    '   m_RecallList.Sort(0, m_RecallList.Count, objStatusComparer)
                    'Load the Recall List to list view
                    .lvRecallList.Items.Clear()

                    For Each objRecallInfo As RLRecallInfo In m_RecallList
                        Dim objListItem As ListViewItem
                        If objRecallInfo.RecallType = strRecallType Then
                            If strRecallName = "100% Returns" Then
                                bIsRecallReturns = True
                            Else
                                bIsRecallReturns = False
                            End If
                            objListItem = New ListViewItem(objRecallInfo.RecallNumber)
                            objListItem.SubItems.Add(objRecallInfo.RecallDescription)
                            iQuantity = CInt(objRecallInfo.RecallQuantity)

                            'Tailoring
                            If objRecallInfo.Tailored Then
                                objListItem.SubItems.Add("TL")
                                'Store the quantity for a tailored list in a has table.
                                m_RecallCountForTL.Add(objRecallInfo.RecallNumber, iQuantity)
                            Else
                                objListItem.SubItems.Add(iQuantity.ToString())
                            End If
                            ' objListItem.SubItems.Add(objRecallInfo.ActiveDate)
                            strRecallStatus = objRecallInfo.ListStatus
                            'If m_RecallStatus.Count > 0 Then
                            '    For Each obj As DictionaryEntry In m_RecallStatus
                            '        If objRecallInfo.RecallNumber = obj.Key Then
                            '            objRecallInfo.ListStatus = obj.Value
                            '            strRecallStatus = obj.Value
                            '        End If
                            '    Next

                            'End If
                            objListItem.SubItems.Add(strRecallStatus)


                            .lvRecallList.Items.Add(objListItem)
                        End If
                    Next

                End If
                If bIsRecallReturns Then
                    .btnHelp.Visible = True
                    .lblTitle.Width = 196 * objAppContainer.iOffSet
                Else
                    'When help button it not displayed increase the width of title lable.
                    .btnHelp.Visible = False
                    .lblTitle.Width = 234 * objAppContainer.iOffSet
                End If

                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                .Visible = True
                .Refresh()
#If RF Then
                objAppContainer.bIsActiveRecallListSCreen = True
#End If
            End With
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in DisplayActiveRecallList of  RLSessionMgr. Exception is: " _
                                                  + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            ' m_frmRLActiveRecallList.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayActiveRecallList of  RLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Function to add actioned items for a UOD.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetActionsItemsInPrevUODs()
        For Each objActioned As DictionaryEntry In m_ActionedItemList
            Dim objRecall As RLItemInfo = objActioned.Value
            If Not m_ActionedItemListinPrevUODs.Contains(objActioned.Key) Then
                m_ActionedItemListinPrevUODs.Add(objActioned.Key, objRecall)
            End If
        Next
    End Sub
    ''' <summary>
    ''' Function to remove actioned items for a UOD on Quit
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RemoveActionsItemsInPrevUODs()
        For Each objActioned As DictionaryEntry In m_ActionedItemList
            Dim objRecall As RLItemInfo = objActioned.Value
            If Not m_ActionedItemListinPrevUODs.Contains(objActioned.Key) Then
                m_ActionedItemListinPrevUODs.Remove(objActioned.Key)
            End If
        Next
    End Sub
    ''' <summary>
    ''' Function to get the recall count for each recall type to display the recall home screen.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayRecallCount()
        'Recalls CR
        ' If m_RecallList.Count = 0 Then
        If Not bNoRecallsInList Then

#If RF Then
            objAppContainer.bRecallConnection = True
            If bIsRCBSent Then
                objAppContainer.objExportDataManager.CreateRCA()
            End If
            bIsRCBSent = False
#End If
            m_RecallList.Clear()
            m_RecallCount = New RecallCount()
            objAppContainer.objDataEngine.GetRecallList(m_RecallList, m_RecallCount)
            objAppContainer.objRecallCount = m_RecallCount
#If RF Then
            objAppContainer.bRecallConnection = False
#End If
        End If
        bNoRecallsInList = False
    End Sub
    ''' <summary>
    ''' Function to fetch the recall item count for tailored recall list.
    ''' </summary>
    ''' <param name="strRecallNumber"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTLItemCount(ByVal strRecallNumber As String) As String
        Try
            Return m_RecallCountForTL(strRecallNumber).ToString()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error in fetching recall item count for tailored list", _
                                                  Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    ''' Function to display temporary message box while loading recall list.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayMessage(ByVal o As Object, ByVal e As EventArgs)
        With m_Message

            Me.m_Message.Location = New System.Drawing.Point(10, 105)
            m_Message.lblMsg.Text = "Please wait, Loading active List"
            m_Message.Visible = True
            Application.DoEvents()
            .Invalidate()
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' Function to display recalls special instruction screen.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplaySpclInstruction(ByVal o As Object, ByVal e As EventArgs)
#If RF Then
        If m_CurrentList.RecallMessage.Trim(" ").Equals("Y") Then
            If objAppContainer.objExportDataManager.CreateRCI(m_CurrentList.RecallNumber, m_CurrentList.RecallMessage) Then
#End If
                With m_frmSpclInstruction
                    .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                    .lblRecallNoText.Text = m_CurrentList.RecallNumber
                    Dim strTempText As String = objAppContainer.objHelper.TrimSpaces(m_CurrentList.RecallMessage)
                    .lblSpcl.Visible = True
                    'FIX for BTCPR00004774 (missing '&' character)
#If RF Then
                    If m_CurrentList.RecallMessage.Trim(" ") = "N" Then
                        m_CurrentList.RecallMessage = m_CurrentList.RecallMessage.Replace("N", " ")
                        .lblSpcl.Visible = False
                    End If
#ElseIf NRF Then
                    If m_CurrentList.RecallMessage.Trim(" ") = "0" Then
                        m_CurrentList.RecallMessage = m_CurrentList.RecallMessage.Replace("0", " ")
                        .lblSpcl.Visible = False
                    End If
#End If

                    If m_CurrentList.RecallMessage.Length < 160 Then
                        m_CurrentList.RecallMessage = m_CurrentList.RecallMessage.PadRight(160, " ")
                    End If
                    .lblSpclText.Text = objAppContainer.objHelper.FormatEscapeSequence(m_CurrentList.RecallMessage.Substring(0, 77))
                    .lblSpclText1.Text = objAppContainer.objHelper.FormatEscapeSequence(m_CurrentList.RecallMessage.Substring(77, 77))
                    .lblSpclText2.Text = objAppContainer.objHelper.FormatEscapeSequence(m_CurrentList.RecallMessage.Substring(154, 6))
                    'BATCH NOS
                    If (WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.CUSTOMEREMERGENCY Or _
                       WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.WITHDRAWN Or _
                       WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.RECALLRETURNS) Then

                        .lblBatchNos.Text = objAppContainer.objHelper.FormatEscapeSequence( _
                                               objAppContainer.objHelper.TrimSpaces(m_CurrentList.BatchNos))
                        'FIX for DEFECT (POD - where recall has special instructions but no batch number don't show reference to batch number, as per PPC)
#If RF Then
                        If m_CurrentList.BatchNos.Trim(" ") = "" Then
#ElseIf NRF Then
                        If m_CurrentList.BatchNos.Trim("0") = "" Then
#End If
                            .lblBatchTitle.Visible = False
                            .lblBatchNos.Visible = False
                        Else
                            .lblBatchNos.Visible = True
                            .lblBatchTitle.Visible = True
                        End If
                    Else
                        .lblBatchTitle.Visible = False
                        .lblBatchNos.Visible = False
                    End If

                    .btnHelp.Visible = False
                    .Visible = True
                    .Refresh()
                End With
#If RF Then
            Else
                DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Scan)
            End If
        Else
            If Not m_CurrentList.BatchNos.Trim(" ") = "" Then
                With m_frmSpclInstruction
                    .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                    .lblRecallNoText.Text = m_CurrentList.RecallNumber
                    Dim strTempText As String = " "

                    'FIX for BTCPR00004774 (missing '&' character)
                    .lblSpcl.Visible = False
                    .lblSpclText.Text = " "
                    .lblSpclText1.Text = " "
                    .lblSpclText2.Text = " "
                    'BATCH NOS
                    If (WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.CUSTOMEREMERGENCY Or _
                                   WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.WITHDRAWN Or _
                                   WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.RECALLRETURNS) Then
                        .lblBatchNos.Text = objAppContainer.objHelper.FormatEscapeSequence( _
                                               objAppContainer.objHelper.TrimSpaces(m_CurrentList.BatchNos))
                        'FIX for DEFECT (POD - where recall has special instructions but no batch number don't show reference to batch number, as per PPC)

                        If m_CurrentList.BatchNos.Trim(" ") = "" Then
                            .lblBatchTitle.Visible = False
                            .lblBatchNos.Visible = False
                        Else
                            .lblBatchNos.Visible = True
                            .lblBatchTitle.Visible = True
                        End If
                    Else
                        .lblBatchTitle.Visible = False
                        .lblBatchNos.Visible = False
                    End If

                    .btnHelp.Visible = False
                    .Visible = True
                    .Refresh()
                End With
            Else
                DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Scan)
            End If
        End If
#End If
    End Sub
    ''' <summary>
    ''' Populate and display the Despatch Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayDespatch(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayDespatch of  RLSessionMgr", Logger.LogLevel.INFO)
        Try
            With m_frmRLDespatch
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                .lblRecallDesc.Text = objAppContainer.objHelper.FormatEscapeSequence _
                                        (m_CurrentList.RecallDescription)
                .lblUODData.Text = m_UODNumber
                .lblSingleData.Text = GetUODItemCount().ToString
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in DisplayDespatch of  RLSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmRLDespatch.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayDespatch of  RLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Populate and display the Item List screen when we click on VIEW
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' 25-June-09: Fix for recall CR to include item status: Change to include a * infront
    ''' of the item that is actioned.
    ''' </remarks>
    Private Sub DisplayItemList(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayItemList of  RLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim iItemCount As Integer = 0
            'Loading the list view with recall items
            m_frmRLItemListScreen.lvRecallList.Items.Clear()
#If RF Then
            'Changed the width - MC55RF Test Fix
            m_frmRLItemListScreen.ColumnHeader2.Width = 83
            m_frmRLItemListScreen.ColumnHeader3.Width = 265
            m_frmRLItemListScreen.ColumnHeader2.Text = "TSF"
#End If
            'Copy the key list (Boots item code list) to an array and then sort it in ascending order.
            Dim arrKeys() As String
            ReDim arrKeys(m_RecallItemList.Keys.Count - 1)
            m_RecallItemList.Keys.CopyTo(arrKeys, 0)
            Array.Sort(arrKeys)
            If m_CurrentList.Tailored Then
                'Populate the list view items.
                For Each strBootsCode As String In arrKeys
                    'Next
                    'For Each obj As DictionaryEntry In m_RecallItemList
                    Dim objRecallItem As RLItemInfo = m_RecallItemList.Item(strBootsCode)
                    Dim objListItem As ListViewItem
                    'Tailoring
                    objRecallItem.IsRecallTailored = True
                    'If the item is tailored then only display the items in the view list screen.
                    If objRecallItem.TailoringFlag Then

                        'Pilot support: CR to associate status to each item in the list.
                        If objRecallItem.RecallItemStatus.Equals(Macros.RECALL_ITEM_PICKED) Or CInt(objRecallItem.UODCount) >= 0 Then
                            objListItem = m_frmRLItemListScreen.lvRecallList.Items.Add(New ListViewItem("*" & objRecallItem.BootsCode))
                        Else
                            objListItem = m_frmRLItemListScreen.lvRecallList.Items.Add(New ListViewItem(objRecallItem.BootsCode))
                        End If
                        objListItem.SubItems.Add(objRecallItem.TSF)
                        objListItem.SubItems.Add(objRecallItem.Description)

                        'To get the actioned item count.
                        If objRecallItem.RecallItemStatus.Equals(Macros.RECALL_ITEM_PICKED) Or CInt(objRecallItem.UODCount) >= 0 Then
                            iItemCount = iItemCount + 1
                        End If
                    End If

                Next
            Else
                'Populate the list view items.
                For Each strBootsCode As String In arrKeys
                    'Next
                    'For Each obj As DictionaryEntry In m_RecallItemList
                    Dim objRecallItem As RLItemInfo = m_RecallItemList.Item(strBootsCode)
                    objRecallItem.IsRecallTailored = False
                    Dim objListItem As ListViewItem
                    'Pilot support: CR to associate status to each item in the list.
                    If objRecallItem.RecallItemStatus.Equals(Macros.RECALL_ITEM_PICKED) Or CInt(objRecallItem.UODCount) >= 0 Then
                        objListItem = m_frmRLItemListScreen.lvRecallList.Items.Add(New ListViewItem("*" & objRecallItem.BootsCode))
                    Else
                        objListItem = m_frmRLItemListScreen.lvRecallList.Items.Add(New ListViewItem(objRecallItem.BootsCode))
                    End If
                    objListItem.SubItems.Add(objRecallItem.TSF)
                    objListItem.SubItems.Add(objRecallItem.Description)

                    'To get the actioned item count.
                    If objRecallItem.RecallItemStatus.Equals(Macros.RECALL_ITEM_PICKED) Or CInt(objRecallItem.UODCount) >= 0 Then
                        iItemCount = iItemCount + 1
                    End If
                Next
            End If

            With m_frmRLItemListScreen
                .lblRecallName.Text = objAppContainer.objHelper.FormatEscapeSequence(m_CurrentList.RecallDescription)
                'If m_CurrentList.ListStatus.Equals("A") And (WorkflowMgr.GetInstance.objActiveFeature = WorkflowMgr.ACTIVEFEATURE.CUSTOMEREMERGENCY Or _
                '                                             WorkflowMgr.GetInstance.objActiveFeature = WorkflowMgr.ACTIVEFEATURE.WITHDRAWN) Then
                'For Change 4988 100% recalls to be non- flexible
                If m_CurrentList.ListStatus.Equals("A") And (WorkflowMgr.GetInstance.objActiveFeature = WorkflowMgr.ACTIVEFEATURE.CUSTOMEREMERGENCY Or _
                                                           WorkflowMgr.GetInstance.objActiveFeature = WorkflowMgr.ACTIVEFEATURE.WITHDRAWN Or _
                                                           WorkflowMgr.GetInstance.objActiveFeature = WorkflowMgr.ACTIVEFEATURE.RECALLRETURNS) Then
                    .lblItemActioneddata.Text = m_TotalItemsInRecall
                Else
                    .lblItemActioneddata.Text = iItemCount
                End If
                .lblItemInRecalldata.Text = GetRecallItemCount()
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in DisplayItemList of  RLSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmRLItemListScreen.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayItemList of  RLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Populate and display the scan screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayScan(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayScan of  RLSessionMgr", Logger.LogLevel.INFO)
        ''Make the form scann enabled
        'BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
        'BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.CODE128)
        'BCReader.GetInstance().StartRead()
        Try
            With m_frmRLScan
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                .objProduct.txtProductCode.Text = ""

                'Get total number of items in the recall.
                m_TotalItemsInRecall = GetRecallItemCount()

                .btnReturn.Visible = False
                .lblRclDesc.Text = objAppContainer.objHelper.FormatEscapeSequence(m_RecallDesc)
                'Retrun Button is only visible if atleast one item is scanned
                If m_ActionedItemList.Count > 0 And m_bIsReturnVisible Then
                    'DEFECT FIX RECALLS
                    m_ActionedItemsInRecall = GetActionedItemCount()
                    .btnReturn.Visible = True
                End If
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in DisplayScan of  RLSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmRLScan.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayScan of  RLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Populate and display the scan screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayDiscrepancy(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayDiscrepancy of  RLSessionMgr", Logger.LogLevel.INFO)
        objAppContainer.objLogger.WriteAppLog("Display Discrepancy", Logger.LogLevel.RELEASE)
        Try
            m_CurrentItem = m_DiscrepancyList.Item(m_DiscItemNumber)
            If m_CurrentItem.ProductCode = Nothing Then
                Dim objItemInfo As New GOItemInfo
                If objAppContainer.objDataEngine.GetProductInfoUsingBC(m_CurrentItem.BootsCode, objItemInfo) Then
                    m_CurrentItem.ProductCode = objItemInfo.SecondBarcode
                Else
                    m_CurrentItem.ProductCode = m_CurrentItem.BootsCode.Substring(0, 6).PadLeft(12, "0")
                End If
            End If
            With m_frmRLItemDetails
                .lblItemscan.Visible = True
                .lblTitle.Text = "The following items have not been scanned / entered into the recall or the quantity is different from Total Stock File." + ControlChars.CrLf + _
                                 "Check and confirm this is correct."
                .lblDesc.Text = objAppContainer.objHelper.FormatEscapeSequence(m_CurrentItem.Description)
                .lblBootsCode.Text = objAppContainer.objHelper.FormatBarcode(m_CurrentItem.BootsCode)
                'DEFECT FIX for BTCPR00005018 (PPC - Discrepancy screen - device is not showing item barcodes)
#If NRF Then
                 .lblBarcode.Text = objAppContainer.objHelper.FormatBarcode(objAppContainer.objHelper.GeneratePCwithCDV(m_CurrentItem.ProductCode))
#ElseIf RF Then
                If m_CurrentItem.ProductCode.Length < 13 Then
                    m_CurrentItem.ProductCode = objAppContainer.objHelper.GeneratePCwithCDV(m_CurrentItem.ProductCode)
                End If
                .lblBarcode.Text = objAppContainer.objHelper.FormatBarcode(m_CurrentItem.ProductCode)
#End If




                .lblStatusdata.Text = m_CurrentItem.Status
                .lblStockCountdata.Text = m_CurrentItem.StockCount
                If m_CurrentItem.TSF.Substring(0, 1).Equals("-") Then
                    .lblTSFdata.ForeColor = Color.Red
                Else
                    .lblTSFdata.ForeColor = .lblStatusdata.ForeColor
                End If
                .lblTSFdata.Text = m_CurrentItem.TSF
                .lblItemscan.Text = CStr(m_DiscItemNumber + 1 & "/" & m_DiscItemCount)
                .btnNext.Visible = False
                .btnConfirm.Visible = True
                .btnQuit.Visible = True
                .btnView.Visible = False
                .btnProductCode.Visible = False
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in DisplayDiscrepancy of  RLSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmRLItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayDiscrepancy of  RLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Populate and display the UOD screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayScanUOD(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayScanUOD of  RLSessionMgr", Logger.LogLevel.INFO)
        Try
            'TODO: Write code in loop
            m_frmRLScanUOD.lwItemList.Items.Clear()
            For Each obj As DictionaryEntry In m_ActionedItemList
                Dim m_Item As RLItemInfo = obj.Value
                'Filter the item that are already. Pilot defect : 121. Display only
                'the items actioned during that session.
                If m_Item.ScanStatus = True Then
                    Dim objListItem As ListViewItem
                    ' objListItem = m_frmRLScanUOD.lwItemList.Items.Add(New ListViewItem(m_Item.StockCount))
                    objListItem = m_frmRLScanUOD.lwItemList.Items.Add(New ListViewItem(m_Item.StockCount))
                    objListItem.SubItems.Add(m_Item.Description)
                End If
            Next
            'Populating the controls
            With m_frmRLScanUOD
                .lblTitle.Text = WorkflowMgr.GetInstance().Title

                .lblListDesc.Text = objAppContainer.objHelper.FormatEscapeSequence _
                                        (m_CurrentList.RecallDescription)
                .lblTotalData.Text = Me.GetUODItemCount().ToString
                'Recall CR: allow black, grey and purple for EX and COM.HO recalls.
                .pnScanLabelColourIndicator.Visible = True
                .pnScanLabelColourIndicator.BackColor = m_CurrentList.LabelColourCode
                '.lblScanColour.Location = New System.Drawing.Point(61, 211)
                '.lblScanColour.Size = New System.Drawing.Size(168, 20)
                .lblScanColour.Text = "Scan " & m_CurrentList.LabelColourName & " Label"
                'End Changes
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in DisplayScanUOD of  RLSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmRLScanUOD.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayScanUOD of  RLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Populate and display the summary screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplaySummary(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplaySummary of  RLSessionMgr", Logger.LogLevel.INFO)
        Try
            objAppContainer.bIsAutoLogOffatSummary = True
            With m_frmRLSummary
                .btnOk.Enabled = True
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                .lblRecallDesc.Text = objAppContainer.objHelper.FormatEscapeSequence _
                                        (m_CurrentList.RecallDescription)
                .lblUODData.Text = m_UODNumber
                .lblTotSinglesData.Text = GetUODItemCount().ToString
                .lblTotValueData.Text = objAppContainer.objHelper.GetCurrency() + " " + CalculateTotalValue().ToString("0.00")
                'FIX for BTCPR00004783(Collect Advice of Content wording does not appear on PPC)
#If RF Then
                ' .lblCompleteInstruction.Visible = False
                .lblCompleteInstruction.Text = "Collect Advice of Content"
#End If
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in DisplaySummary of  RLSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmRLSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplaySummary of  RLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Populate and display the Item details screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayItemDetails(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayItemDetails of  RLSessionMgr", Logger.LogLevel.INFO)
        Try
            'Set default value in label to space.
            m_frmRLItemDetails.lblStockCountdata.Text = " "

            m_CurrentItem.ItemPrice = m_objRLItemInfo.ItemPrice
            m_CurrentItem.ProductCode = m_objRLItemInfo.ProductCode
            m_CurrentItem.FirstBarcode = m_objRLItemInfo.FirstBarcode

            'Check if the connection is lost and re-connected to recall session then
            'display the item count or else don't display a count instead display empty label.
            If m_ActionedItemList.ContainsKey(m_CurrentItem.BootsCode) And m_CurrentItem.ScanStatus Then
                Dim objItemInfo As RLItemInfo = m_ActionedItemList(m_CurrentItem.BootsCode)
                If objItemInfo.StockCount = "" Then
                    m_frmRLItemDetails.lblStockCountdata.Text = " "
                Else
                    m_frmRLItemDetails.lblStockCountdata.Text = CInt(objItemInfo.StockCount).ToString()
                End If
            Else
                m_frmRLItemDetails.lblStockCountdata.Text = " "
            End If
            'DEFECT FIX FOR COUNTING STOCK COUNT IN DIFFERENT UODS
            StockCount = m_frmRLItemDetails.lblStockCountdata.Text
            With m_frmRLItemDetails
#If RF Then
                .lblTSF.Text = "Total Stock File :"
#End If
                'Set required controls in the screen to visible state.
                SetControlVisible()
                .lblItemscan.Visible = False
                .btnConfirm.Visible = False
                .lblTitle.Text = objAppContainer.objHelper.FormatEscapeSequence(m_CurrentList.RecallDescription)
                .lblDesc.Text = m_CurrentItem.Description
                .lblBootsCode.Text = objAppContainer.objHelper.FormatBarcode(m_CurrentItem.BootsCode)
                .lblBarcode.Text = objAppContainer.objHelper.FormatBarcode(objAppContainer.objHelper.GeneratePCwithCDV(m_CurrentItem.ProductCode))
                .lblStatusdata.Text = m_CurrentItem.Status
                'To highlight stock figure in red colour for a negative value.
                If m_CurrentItem.TSF.Substring(0, 1).Equals("-") Then
                    .lblTSFdata.ForeColor = Color.Red
                Else
                    .lblTSFdata.ForeColor = .lblStatusdata.ForeColor
                End If
                .lblTSFdata.Text = m_CurrentItem.TSF
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in DisplayItemDetails of  RLSessionMgr. Exception is: " _
                                                             + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmRLItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayItemDetails of  RLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Display the pop up message in case of excess sales plan and planner leaver recalls
    ''' to inform the user about any items pendings and the last date for recalls to be completed.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplaySummaryPopUp()
        Try
            Dim iCompleted As Integer = m_ActionedItemList.Count

            Dim iActionedReturnsItemCount As Integer = 0
            For Each obj As DictionaryEntry In m_RecallItemList
                Dim objRecallitem As RLItemInfo = obj.Value
                If objRecallitem.ScanStatus Or objRecallitem.Status = "Actioned" Then
                    iActionedReturnsItemCount = iActionedReturnsItemCount + 1
                End If
            Next
            Dim iTotal As Integer = m_RecallItemList.Count
            Dim strActiveDate As String = m_CurrentList.ActiveDate.Insert(4, "/")
            strActiveDate = strActiveDate.Insert(7, "/")
            If iActionedReturnsItemCount <> iTotal Then
                'If iCompleted <> iTotal Then
                Dim strMsg As String = iActionedReturnsItemCount & " Recall item(s) scanned out of " & iTotal & ". " + _
                                       vbCrLf & "Recall will remain open until: " + _
                                       Convert.ToDateTime(strActiveDate).ToString("d")

                'Display the pop up message.
                MessageBox.Show(strMsg, "Recall Summary", MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString() + " occured in DsiplaySummaryPopUp() of  RLSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetControlVisible()
        With m_frmRLItemDetails
            .btnQuantity.Visible = True
            .btnView.Visible = True
            .btnNext.Visible = True
            .lblStockCount.Visible = True
            .lblStockCountdata.Visible = True
            .lblDesc.Visible = True
            .lblBootsCode.Visible = True
            .lblBarcode.Visible = True
            .btnProductCode.Visible = True
            .lblTSF.Visible = True
            .lblTSFdata.Visible = True
            .lblStockCount.Visible = True
            .lblStockCountdata.Visible = True
        End With
    End Sub
    ''' <summary>
    ''' Calculate the total price of all the items in UOD
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CalculateTotalValue() As Decimal
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered CalculateTotalValue of  RLSessionMgr", Logger.LogLevel.INFO)
        Dim dTotal As Decimal = 0.0
        Try
            For Each obj As DictionaryEntry In m_ActionedItemList
                Dim m_Item As RLItemInfo = obj.Value
                If m_Item.ScanStatus = True Then
                    dTotal = dTotal + (CDec(m_Item.ItemPrice) * CDec(m_Item.StockCount))
                End If
            Next
            If Not dTotal = 0 Then
                dTotal = dTotal / 100
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in CalculateTotalValue of  RLSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit CalculateTotalValue of  RLSessionMgr", Logger.LogLevel.INFO)
        Return dTotal
    End Function
    ''' <summary>
    ''' Calculate the total items that are actioned
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetActionedItemCount() As Integer
        'Return m_ActionedItemList.Count
        Dim strVar = From objTemp As RLItemInfo In m_ActionedItemList.Values Where objTemp.RecallItemStatus.Equals(Macros.RECALL_ITEM_PICKED) And objTemp.ScanStatus = True Select objTemp
        Return strVar.Count
    End Function
    ''' <summary>
    ''' Function to get the minimum return quantity for the selected recall list.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMinReturnQuantity(ByVal strRecallNo As String) As Integer
        Try
            Dim objTempRecall As RLRecallInfo = New RLRecallInfo()
            For Each objTempRecall In m_RecallList
                If objTempRecall.RecallNumber.Equals(strRecallNo) Then
                    Return objTempRecall.MinRecallQty
                End If
            Next
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in GetMinReturnQuantity", Logger.LogLevel.RELEASE)
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' Calculate the total items that are actioned
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsDiscrepancy() As Integer
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered IsDiscrepancy of  RLSessionMgr", Logger.LogLevel.INFO)
        Try
            'Clear the array list conaining the discrepency items.
            m_DiscrepancyList.Clear()
            'Get the list of discrepency list.
            'If WorkflowMgr.GetInstance.objActiveFeature.Equals(WorkflowMgr.ACTIVEFEATURE.CUSTOMEREMERGENCY) Or _
            '    WorkflowMgr.GetInstance.objActiveFeature.Equals(WorkflowMgr.ACTIVEFEATURE.WITHDRAWN) Then
            'For Change 4988 100% recalls to be non- flexible
            If WorkflowMgr.GetInstance.objActiveFeature.Equals(WorkflowMgr.ACTIVEFEATURE.CUSTOMEREMERGENCY) Or _
                WorkflowMgr.GetInstance.objActiveFeature.Equals(WorkflowMgr.ACTIVEFEATURE.WITHDRAWN) Or _
                WorkflowMgr.GetInstance.objActiveFeature.Equals(WorkflowMgr.ACTIVEFEATURE.RECALLRETURNS) Then
                IsActionedininCurrentUOD()
                For Each obj As DictionaryEntry In m_RecallItemList
                    Dim objRecallitem As RLItemInfo = obj.Value

                    objRecallitem.StockCount = objRecallitem.StockCount.Trim(" ")

                    'FIX FOR DEFECT BTCPR00004989(POD/PPC - If selected another UOD required=Yes 
                    'the Discrepancy screen should add together any item counts from previous UOD's)
                    Dim iRecallStockCount As Integer
                    If objRecallitem.StockCount <> "" Then
                        iRecallStockCount = Convert.ToInt32(objRecallitem.StockCount)
                    Else
                        iRecallStockCount = 0
                    End If
                    If objRecallitem.AddedStockCount > 0 Then
                        If objRecallitem.AddedStockCount.ToString() = objRecallitem.TSF Then
                            objRecallitem.Status = "Actioned"
                        End If
                    End If

                    If (objRecallitem.Status = "Discrepancy" Or objRecallitem.StockCount = "") And _
                        objRecallitem.Status <> "Confirmed" And Not objRecallitem.ActionedinPreviousUOD Then
                        'Adding on the Tailored items to discrepancy list if the 100% Returns Recall List is tailored
                        If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.RECALLRETURNS Then
                            If objRecallitem.IsRecallTailored And objRecallitem.TailoringFlag Then
                                m_DiscrepancyList.Add(objRecallitem)
                            ElseIf Not objRecallitem.IsRecallTailored Then
                                m_DiscrepancyList.Add(objRecallitem)
                            End If
                        Else
                            m_DiscrepancyList.Add(objRecallitem)
                        End If
                        ' m_DiscrepancyList.Add(objRecallitem)
                    End If
                Next
                'Set number of discrepency items to a private variable.
                m_DiscItemCount = m_DiscrepancyList.Count
                'Reset discrepency item list number
                m_DiscItemNumber = 0
                If m_DiscItemCount > 0 Then
                    'Set the global variable to mention that there are dicrepencies
                    bIsDiscrepancy = True
                    'Writing to Log INFO File while exit
                    objAppContainer.objLogger.WriteAppLog("Exit IsDiscrepancy of  RLSessionMgr", Logger.LogLevel.INFO)
                    Return True
                Else
                    'Writing to Log INFO File while exit
                    objAppContainer.objLogger.WriteAppLog("Exit IsDiscrepancy of  RLSessionMgr", Logger.LogLevel.INFO)
                    Return False
                End If
            Else
                'If the recall list type is not HO/COMPANY reccal then, don't display descrepency screens.
                Return False
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in IsDiscrepancy of  RLSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    ''' Mark confirmed items in discrepancy list, actioned list and recall list.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ConfirmDiscrepancy()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ConfirmDiscrepancy of  RLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim strStockCount As String = m_frmRLItemDetails.lblStockCountdata.Text.Trim()
            If strStockCount.Equals("") Then
                m_CurrentItem.StockCount = "0"
            Else
                m_CurrentItem.StockCount = strStockCount
            End If

            'Pilot Support: Recall CR to include status.
            m_CurrentItem.RecallItemStatus = Macros.RECALL_ITEM_PICKED

#If RF Then
            Dim objRCG As RCGRecord = New RCGRecord()
            Try
                objRCG.strRecallRef = m_CurrentList.RecallNumber
                objRCG.strRecallItem = m_CurrentItem.BootsCode.Substring(0, 6)
                objRCG.strRecallCount = m_CurrentItem.StockCount
                'Send RCG in case of RF mode to update the controller.
                If objAppContainer.objExportDataManager.CreateRCG(objRCG) Then
                    'Add new item object with appended quantity data
                    If m_ActionedItemList.ContainsKey(m_CurrentItem.BootsCode) Then
                        Dim objItem As RLItemInfo
                        objItem = m_ActionedItemList(m_CurrentItem.BootsCode)
                        objItem.ScanStatus = True
                        objItem.Status = "Actioned"
                    Else
                        m_CurrentItem.ScanStatus = True
                        m_CurrentItem.Status = "Actioned"
                        m_ActionedItemList.Add(m_CurrentItem.BootsCode, m_CurrentItem)
                    End If
                    'set the private variable to enable Return button.
                    m_bIsReturnVisible = True

                    'Update the status in Recall Item list as well
                    Dim objUpdate As RLItemInfo = m_RecallItemList(m_CurrentItem.BootsCode)
                    objUpdate.Status = "Confirmed"
                Else
                    objAppContainer.objLogger.WriteAppLog("Error occured in sending RCG of  RLSessionMgr.", _
                                                          Logger.LogLevel.RELEASE)
                    Exit Sub
                End If
            Catch ex As Exception
                If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                    Throw ex
                End If
            Finally
                objRCG = Nothing
            End Try
#ElseIf NRF Then
            If m_ActionedItemList.ContainsKey(m_CurrentItem.BootsCode) Then
                Dim objItem As RLItemInfo
                objItem = m_ActionedItemList(m_CurrentItem.BootsCode)
                objItem.ScanStatus = True
                objItem.Status = "Actioned"
            Else
                m_CurrentItem.ScanStatus = True
                m_CurrentItem.Status = "Actioned"
                m_ActionedItemList.Add(m_CurrentItem.BootsCode, m_CurrentItem)
            End If
            'set the private variable to enable Return button.
            m_bIsReturnVisible = True
            'Update the status in Recall Item list as well
            Dim objUpdate As RLItemInfo = m_RecallItemList(m_CurrentItem.BootsCode)
            objUpdate.Status = "Confirmed"
#End If
            'Increment the discrepency item counter.
            m_DiscItemNumber = m_DiscItemNumber + 1
            'if there are still items to be confirmed then display dicrepency screen or
            'else display UOD scan screen
            RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.ItemDetails
            If m_DiscItemCount = m_DiscItemNumber Then
                RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.ScanUOD)
            Else
                RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Discrepancy)
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in SetProductInfo of  RLSessionMgr. Exception is: " _
                                                             + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit ConfirmDiscrepancy of  RLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Calculate the total items that are actioned
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUODItemCount() As Integer
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered GetUODItemCount of  RLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim m_TempClaimCount As Integer
            For Each obj As DictionaryEntry In m_ActionedItemList
                Dim m_TempList As RLItemInfo = obj.Value
                If m_TempList.ScanStatus = True Then
                    m_TempClaimCount += CInt(m_TempList.StockCount)
                End If
            Next
            'Writing to Log INFO File while exit
            objAppContainer.objLogger.WriteAppLog("Exit GetUODItemCount of  RLSessionMgr", Logger.LogLevel.INFO)
            Return m_TempClaimCount
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in GetUODItemCount of  RLSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    ''' Get the Total items in a recall Item list
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRecallItemCount() As Integer
        If WorkflowMgr.GetInstance.objActiveFeature = WorkflowMgr.ACTIVEFEATURE.RECALLRETURNS And m_CurrentList.Tailored = True Then
            'get the number of items in tailored list.
            Dim objTailoredItemCount = From objItem As RLItemInfo In m_RecallItemList.Values Where _
                                       objItem.TailoringFlag = True Select objItem
            Return objTailoredItemCount.Count
        Else
            Return m_RecallItemList.Count
        End If
    End Function
    ''' <summary>
    ''' Fetches the all the items that are actioned and return a list
    ''' </summary>
    ''' <returns>List of items scanned</returns>
    ''' <remarks></remarks>
    Public Function GetActionedItemList() As Hashtable
        If m_ActionedItemList IsNot Nothing Then
            Return m_ActionedItemList
        Else
            Return Nothing
        End If

    End Function
    ''' <summary>
    ''' Sets the item selected as the current item
    ''' </summary>
    ''' <param name="strBootsCode">Boots code to fetch the object from hashtable</param>
    ''' <remarks></remarks>
    Public Sub SetCurrentItem(ByVal strBootsCode As String)
        m_CurrentItem = Nothing
        m_CurrentItem = m_RecallItemList(strBootsCode)
    End Sub

    ''' <summary>
    ''' Sets the Recall List selected as the current List
    ''' </summary>
    ''' <param name="strRecallNo">Recall number to identify the objects</param>
    ''' <remarks></remarks>
    Public Sub SetCurrentList(ByVal strRecallNo As String)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered SetCurrentList of  RLSessionMgr", Logger.LogLevel.INFO)
        Try
            m_CurrentList = Nothing
            For Each objRecall As RLRecallInfo In m_RecallList
                If objRecall.RecallNumber = strRecallNo Then
                    m_CurrentList = objRecall
                    'm_CurrentList.ListStatus = m_CurrentListStatus
                    strLabelType = m_CurrentList.LabelType
                    iMinReturnQty = m_CurrentList.MinRecallQty
                    Exit For
                End If
            Next
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in SetCurrentList of  RLSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit SetCurrentList of  RLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Checks if the Scanned/Entered Product code is present in the Recall List
    ''' </summary>
    ''' <returns>True if success else false</returns>
    ''' <remarks></remarks>
    Public Function IsRecallListItem(ByVal strBarcode As String) As Boolean
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered IsRecallListItem of  RLSessionMgr", Logger.LogLevel.INFO)
        Try
            m_objRLItemInfo = New GOItemInfo()
            'Check if the scanned item is valid Boots Code
            If (objAppContainer.objHelper.ValidateBootsCode(strBarcode)) Then
                'strBarcode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                If Not objAppContainer.objDataEngine.GetProductInfoUsingBC(strBarcode, m_objRLItemInfo) Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M11"), _
                                  "Item Not Found", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                   MessageBoxDefaultButton.Button1)
                    Return False
                End If
                'strTempBootsCode = m_objRLItemInfo.BootsCode
                'If the item belongs to the same recall list then Recall Item Validation passes
                '                For Each obj As DictionaryEntry In m_RecallItemList
                '                    Dim objRecallItem As RLItemInfo = obj.Value
                '                    'If strBarcode = objRecallItem.ProductCode Or _
                '                    'strTempBootsCode = objRecallItem.BootsCode Or _
                '                    'strBarcode = objRecallItem.FirstBarcode Then
                '                    If strTempBootsCode = objRecallItem.BootsCode Then
                '                        strBootsCode = objRecallItem.BootsCode
                '#If NRF Then
                '                        'DARWIN
                '                        bBarcode = True
                '#End If
                '                        Exit For
                '                    End If
                '                Next
                'Check if the scanned item is valid EAN
            ElseIf (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                Dim bBarcode As Boolean = False
                'Removing the last digit from the Barcode since its used only for check digit validation
                strBarcode = strBarcode.PadLeft(13, "0")
                strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                objAppContainer.objLogger.WriteAppLog("Entered IsRecallListItem of  RLSessionMgr", Logger.LogLevel.RELEASE)
                If Not objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_objRLItemInfo) Then
#If NRF Then
                    'DARWIN
                    If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                        strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                        If Not objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_objRLItemInfo) Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M11"), _
                                            "Item Not Found", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                             MessageBoxDefaultButton.Button1)
                            Return False
                        End If
                        'strTempBootsCode = m_objRLItemInfo.BootsCode
                        ''If the item belongs to the same recall list then Recall Item Validation passes
                        'For Each obj As DictionaryEntry In m_RecallItemList
                        '    Dim objRecallItem As RLItemInfo = obj.Value
                        '    'If strBarcode = objRecallItem.ProductCode Or _
                        '    'strTempBootsCode = objRecallItem.BootsCode Or _
                        '    'strBarcode = objRecallItem.FirstBarcode Then
                        '    If strTempBootsCode = objRecallItem.BootsCode Then
                        '        strBootsCode = objRecallItem.BootsCode
                        '        Exit For
                        '    End If
                        'Next
                    Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M11"), _
                                        "Item Not Found", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
                        Return False
                    End If
#ElseIf RF Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M11"), _
                                    "Item Not Found", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                    Return False
#End If
                End If
                'END DARWIN CHANGE

                'End If
                '                strTempBootsCode = m_objRLItemInfo.BootsCode
                '                'If the item belongs to the same recall list then Recall Item Validation passes
                '                For Each obj As DictionaryEntry In m_RecallItemList
                '                    Dim objRecallItem As RLItemInfo = obj.Value
                '                    'If strBarcode = objRecallItem.ProductCode Or _
                '                    'strTempBootsCode = objRecallItem.BootsCode Or _
                '                    'strBarcode = objRecallItem.FirstBarcode Then
                '                    If strTempBootsCode = objRecallItem.BootsCode Then
                '                        strBootsCode = objRecallItem.BootsCode
                '#If NRF Then
                '                        'DARWIN
                '                        bBarcode = True
                '#End If
                '                        Exit For
                '                    End If
                '                Next


            Else
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M24"), _
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                MessageBoxDefaultButton.Button1)
                'Writing to Log INFO File while exit
                objAppContainer.objLogger.WriteAppLog("Exit IsRecallListItem of  RLSessionMgr", Logger.LogLevel.INFO)
                Return False
            End If

            If m_RecallItemList.ContainsKey(m_objRLItemInfo.BootsCode) Then
                'Get the item in from the recall list so that it can be displayed
                objAppContainer.objLogger.WriteAppLog("Scanned Valid Product/Boots code", Logger.LogLevel.RELEASE)
                SetCurrentItem(m_objRLItemInfo.BootsCode)
                'Writing to Log INFO File while exit
                objAppContainer.objLogger.WriteAppLog("Exit IsRecallListItem of  RLSessionMgr", Logger.LogLevel.INFO)
                Return True
            Else
                'Writing to Log INFO File while exit
                objAppContainer.objLogger.WriteAppLog("Exit IsRecallListItem of  RLSessionMgr", Logger.LogLevel.INFO)
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M25"), _
                                 "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                  MessageBoxDefaultButton.Button1)
                Return False
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in IsRecallListItem of  RLSessionMgr. Exception is: " _
                                                         + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If

        End Try
    End Function
    ''' <summary>
    ''' Gets the data fetched by Data Access Layer and inserts the data into RecallItemList
    ''' </summary>
    ''' <returns>True if success else false</returns>
    ''' <remarks>
    ''' 25-June-09: Fix for recall CR to include item status. If an item in a list selected by
    ''' the user is already actioned then that item will be added to actioned list by means of
    ''' m_ActionedItemList hash table.
    ''' </remarks>
    Public Function SetRecallItemList(ByVal strRecallNo As String, ByVal iRecallQty As Integer, ByVal isListActioned As Boolean, ByVal isTailored As Boolean) As Boolean
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered SetRecallItemList of  RLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim RecallItemList As New ArrayList
            'reset the boolean variable that allows to display return button.
            m_bIsReturnVisible = False
            'Initially set bTailoring flag to false to check if the list has ay tailored items or not.
            bTailoring = False
#If NRF Then
                'Fetching data from the data access layer
            If objAppContainer.objDataEngine.GetRecallItemDetails(strRecallNo, iRecallQty, RecallItemList, isTailored) Then
#ElseIf RF Then
            'Fetching data from the data access layer
            If objAppContainer.objDataEngine.GetRecallItemDetails(strRecallNo, iRecallQty, RecallItemList) Then
#End If
                For Each obj As RLItemInfo In RecallItemList
                    'System Testing 
#If RF Then
                    obj.BootsCode = objAppContainer.objHelper.GenerateBCwithCDV(obj.BootsCode)
                    If isTailored Then
                        'If Not bTailoring Then
                        If obj.TailoringFlag Then
                            bTailoring = True
                        End If
                        'End If
                    End If
#ElseIf NRF Then
                    If Not isTailored Then
                        'Setting the tailoring falg to explicity true for other recall types other than 100% returns.
                        'For non-Tailored list all the items should be shown in the list view so explicitly setting tailored flag as true
                        'Here True means tailoring is NA.
                        obj.TailoringFlag = True
                    End If
#End If
                    'Add the item to hash table.
                    m_RecallItemList.Add(obj.BootsCode, obj)
                    'DEFECT FIX FOR DEFECT 5015
#If RF Then
                    If obj.RecallItemStatus.Equals(Macros.RECALL_ITEM_PICKED) Then
                        'FIX TO DEFECT - actioned items in a previous session being displayed in SCAN UOD Screen
                        'obj.ScanStatus = True
                        obj.ScanStatus = False
                        'DEFECT FIX for PPC - non flexible recalls - completed - if further items are actioned discrepancy screen should only show newly action items
                        obj.ActionedinPreviousUOD = True
                        If obj.ItemPrice Is Nothing Then
                            Dim objItemDetail As New GOItemInfo
                            objAppContainer.objDataEngine.GetProductInfoUsingBC(obj.BootsCode, objItemDetail)
                            obj.ItemPrice = objItemDetail.ItemPrice
                            m_ActionedItemList.Add(obj.BootsCode, obj)
                            objItemDetail = Nothing
                        Else
                            m_ActionedItemList.Add(obj.BootsCode, obj)
                        End If
                    End If
#End If
                Next
                'Tailoring
                'For 100% Returns
                If ((Not bTailoring) And (isTailored)) And bIsRecallReturns Then
                    bNoTailoredItemsinReturns = True
                Else
                    bNoTailoredItemsinReturns = False
                End If

                'Writing to Log INFO File while exit
                objAppContainer.objLogger.WriteAppLog("Exit SetRecallItemList of  RLSessionMgr", Logger.LogLevel.INFO)
                Return True
            Else
                If Not bActionedRecall Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M42"), "Error", _
                    MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                    MessageBoxDefaultButton.Button1)
                End If

                'Writing to Log INFO File while exit
                objAppContainer.objLogger.WriteAppLog("Exit SetRecallItemList of  RLSessionMgr", Logger.LogLevel.INFO)
                Return False
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(" Exception in SetRecallItemList of  RLSessionMgr. Exception is: " _
                                                  + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Function
#If NRF Then
    ''' <summary>
    ''' Function to set Tailoringflag
    ''' </summary>
    ''' <param name="TSF"></param>
    ''' <param name="StockDate"></param>
    ''' <param name="Tailoredflag"></param>
    ''' <param name="strLivePog1"></param>
    ''' <remarks></remarks>
    Public Sub SetTailoringFlag(ByVal TSF As Integer, ByVal StockDate As DateTime, _
                                ByRef Tailoredflag As Boolean, ByVal strLivePog1 As String)
        If TSF > 0 Then
            bTailoring = True
            'This is for adding tailored item in the view list
            Tailoredflag = True
        ElseIf DateDiff(DateInterval.Day, StockDate, DateTime.Today) <= objAppContainer.iStockMovementValidityDays Then
            bTailoring = True
            'This is for adding tailored item in the view list
            Tailoredflag = True
        ElseIf strLivePog1 <> "0" Then
            bTailoring = True
            'This is for adding tailored item in the view list
            Tailoredflag = True
        Else
            'This is for adding tailored item in the view list
            Tailoredflag = False
        End If
    End Sub

#End If
    ''' <summary>
    ''' Stores the UOD number so that it can be sent to export data later
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetUOD(ByVal sUODNumber As String)
        m_UODNumber = sUODNumber
    End Sub
    Public Sub IsActionedinPreviousUOD()
        For Each objActioned As DictionaryEntry In m_ActionedItemList
            Dim objRecallItemScanned As RLItemInfo = objActioned.Value
            objRecallItemScanned.ActionedinPreviousUOD = True
            objRecallItemScanned.ActionedNow = False
        Next
    End Sub
    Public Sub IsActionedininCurrentUOD()
        For Each objActioned As DictionaryEntry In m_ActionedItemList
            Dim objRecallItemScanned As RLItemInfo = objActioned.Value
            If objRecallItemScanned.ScanStatus Then
                objRecallItemScanned.ActionedNow = True
                objRecallItemScanned.ActionedinPreviousUOD = False
            End If

        Next
    End Sub
    ''' <summary>
    ''' Sets the Product info kept in the RLItemInfo object into the array list
    ''' </summary>
    ''' <remarks>
    ''' 25-June-09: Fix for recall CR to include item status: To include item status
    ''' as picked when an item is actioned by the user.
    ''' </remarks>
    Public Sub SetProductInfo()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered SetProductInfo of  RLSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim strStatus As String
            'If item is already scanned remove the Actioned Item list 
            For Each obj As DictionaryEntry In m_ActionedItemList
                Dim objItemInfo As RLItemInfo = obj.Value
                If objItemInfo.ProductCode = m_CurrentItem.ProductCode Then
                    m_ActionedItemList.Remove(objItemInfo.BootsCode)
                    Exit For
                End If
            Next
            m_CurrentItem.StockCount = m_frmRLItemDetails.lblStockCountdata.Text
            'FIX FOR DEFECT BTCPR00004989(POD/PPC - If selected another UOD required=Yes the Discrepancy screen should add together any item counts from previous UOD's)
            ' Not adding the stock count, if no stock count is changed in item details screen
            If StockCount <> m_frmRLItemDetails.lblStockCountdata.Text Then
                m_CurrentItem.AddedStockCount += Convert.ToInt32(m_CurrentItem.StockCount)
            End If

            ' End If
            'Checking for Discrepancy
            If (CInt(m_CurrentItem.TSF) <> CInt(m_CurrentItem.StockCount) Or m_CurrentItem.StockCount = "") And _
                m_CurrentItem.Status <> "Confirmed" Then
                strStatus = "Discrepancy"
            Else
                strStatus = "Actioned"
            End If
            m_CurrentItem.Status = strStatus    'Set Item status after comapring the item count and TSF.
            m_CurrentItem.ScanStatus = True 'Set scan status to true once the item is processed.
#If RF Then
            Dim objRCG As RCGRecord = New RCGRecord()
            Try
                objRCG.strRecallRef = m_CurrentList.RecallNumber
                objRCG.strRecallItem = m_CurrentItem.BootsCode.Substring(0, 6)
                objRCG.strRecallCount = m_CurrentItem.StockCount
                'Send RCG in case of RF mode to update the controller.
                If objAppContainer.objExportDataManager.CreateRCG(objRCG) Then
                    'Add new item object with appended quantity data
                    m_ActionedItemList.Add(m_CurrentItem.BootsCode, m_CurrentItem)

                    'Pilot Support: Recall CR to include status.
                    m_CurrentItem.RecallItemStatus = Macros.RECALL_ITEM_PICKED

                    'set the private variable to enable Return button.
                    m_bIsReturnVisible = True

                    'Update the status in Recall Item list as well
                    Dim objUpdate As RLItemInfo = m_RecallItemList(m_CurrentItem.BootsCode)
                    objUpdate.Status = strStatus
                End If
            Catch ex As Exception
                If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                    Throw ex
                End If
            Finally
                objRCG = Nothing
            End Try
#ElseIf NRF Then
            'Add new item object with appended quantity data
            m_ActionedItemList.Add(m_CurrentItem.BootsCode, m_CurrentItem)
            'set the private variable to enable Return button.
            m_bIsReturnVisible = True
            'Pilot Support: Recall CR to include status.
            m_CurrentItem.RecallItemStatus = Macros.RECALL_ITEM_PICKED
            'Update the status in Recall Item list as well
            Dim objUpdate As RLItemInfo = m_RecallItemList(m_CurrentItem.BootsCode)
            objUpdate.Status = strStatus
#End If
            strStatus = Nothing
            'objUpdate = Nothing
            'm_CurrentItem = Nothing
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in SetProductInfo of  RLSessionMgr. Exception is: " _
                                                             + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit SetProductInfo of  RLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Generate the Export data file by calling the API in Data access layer
    ''' </summary>
    ''' <remarks></remarks>
#If NRF Then
     Public Sub GenerateExportData()
#ElseIf RF Then
    Public Function GenerateExportData() As Boolean
#End If

        'Creating a variable of the type CreateUOS
        Dim objRCB As RCBRecord = Nothing
        Dim objRCG As RCGRecord = Nothing
        m_frmRLDespatch.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing RCA")
        'Handling Autologoff scenario for no data
        If (m_UODNumber = Nothing Or m_ActionedItemList.Count < 1) And Not bNoTailoredRtrnItem Then
#If NRF Then
            Exit Sub
#ElseIf RF Then
            Exit Function
#End If
        End If
#If NRF Then
         objAppContainer.objExportDataManager.CreateRCA()
'#ElseIf RF Then
'        If objAppContainer.objExportDataManager.CreateRCA() Then
'            Return False
'        End If
#End If

        objAppContainer.objLogger.WriteAppLog("RCA written Successfully", Logger.LogLevel.RELEASE)
        'Variable to define a sequence number
        Dim iSequenceNo As Integer = 1
#If NRF Then
        For Each obj As DictionaryEntry In m_ActionedItemList
            Dim objItem As RLItemInfo = obj.Value
            'Filter the item that are already
            If objItem.ScanStatus = True Then
                m_frmRLDespatch.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing RCG :" + objItem.BootsCode)
                objRCG.strRecallRef = m_CurrentList.RecallNumber
                objRCG.strRecallItem = objAppContainer.objHelper.RemoveBootsCodeCDV(objItem.BootsCode)
                objRCG.strRecallCount = objItem.StockCount

                objAppContainer.objExportDataManager.CreateRCG(objRCG)

                objAppContainer.objLogger.WriteAppLog("RCG written Successfully", Logger.LogLevel.RELEASE)
            End If
        Next
#End If
        m_frmRLDespatch.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing RCB")
        'Creating a varible of the type CreateUOX to be sent to DAL
        objRCB.strRecallref = m_CurrentList.RecallNumber
        objRCB.strnumUOD = m_UODNumber
        If RLSessionMgr.GetInstance.bCreateAnotherRecallUOD Then
            'If bActionedRecall Then
            '    'Using another UOD
            '    objRCB.strStateCall = "A"
            'Else
            '    'Using another UOD
            '    objRCB.strStateCall = "P"
            'End If
            'Using another UOD
            objRCB.strStateCall = "P"
        ElseIf bActionedRecall Then
            'Actioning partially a completed recall should 
            'set the recall status as actioned
            objRCB.strStateCall = "A"
        ElseIf (m_CurrentList.RecallType = "R" Or m_CurrentList.RecallType = "S") And _
                m_CurrentList.Tailored Then
            'for 100% Returns check if all Tailored items in the list are
            'actioned or not
            Dim iActionedReturnsItemCount As Integer = 0
            Dim iTotalReturnsItemCount As Integer = 0
            Dim iActionedNoTailored As Integer = 0
            For Each obj As DictionaryEntry In m_RecallItemList
                Dim objRecallitem As RLItemInfo = obj.Value
                If objRecallitem.ScanStatus And objRecallitem.TailoringFlag Then
                    iActionedReturnsItemCount = iActionedReturnsItemCount + 1
                ElseIf objRecallitem.ScanStatus And Not objRecallitem.TailoringFlag Then
                    iActionedNoTailored = iActionedNoTailored + 1
                End If
                If objRecallitem.TailoringFlag Then
                    iTotalReturnsItemCount = iTotalReturnsItemCount + 1
                End If
            Next

            If iActionedReturnsItemCount = iTotalReturnsItemCount And iTotalReturnsItemCount > 0 Then
                'if all Tailored items in the list are actioned then set Recall Status as Actioned
                objRCB.strStateCall = "A"
            ElseIf RLSessionMgr.GetInstance().bNoTailoredItemsinReturns Then
                'if there are no tailored Items in the Recall List
                'and if any of the non tailored items in the list is actioned
                ' then set recall status as Actioned
                objRCB.strStateCall = "A"
            ElseIf m_ActionedItemList.Count = m_RecallItemList.Count Then
                'if all items in the Recalls list is actioned
                objRCB.strStateCall = "A"
            Else
                'Dim itActionedReturnsItemCount As Integer = 0
                'For Each obj As DictionaryEntry In m_RecallItemList
                '    Dim objRecallitem As RLItemInfo = obj.Value
                '    If objRecallitem.ScanStatus Then
                '        itActionedReturnsItemCount = itActionedReturnsItemCount + 1
                '    End If
                'Next
                'If itActionedReturnsItemCount = m_RecallItemList.Count Then
                '    objRCB.strStateCall = "A"
                'Else
                '    objRCB.strStateCall = "P"
                'End If
                objRCB.strStateCall = "P"
                'End If
            End If
        Else
            If m_ActionedItemList.Count = m_RecallItemList.Count Then
                'if all items in the recalls are actioned
                objRCB.strStateCall = "A"
            Else
                Dim iActionedReturnsItemCount As Integer = 0
                For Each obj As DictionaryEntry In m_RecallItemList
                    Dim objRecallitem As RLItemInfo = obj.Value
                    If objRecallitem.ScanStatus Or objRecallitem.Status = "Actioned" Then
                        iActionedReturnsItemCount = iActionedReturnsItemCount + 1
                    End If
                Next
                If iActionedReturnsItemCount = m_RecallItemList.Count Then
                    objRCB.strStateCall = "A"
                Else
                    objRCB.strStateCall = "P"
                End If
                ' objRCB.strStateCall = "P"
                ' End If
            End If
        End If

        'End If
#If NRF Then
        objAppContainer.objExportDataManager.CreateRCB(objRCB)
#ElseIf RF Then
        Try
            If Not objAppContainer.objExportDataManager.CreateRCB(objRCB) Then
                Return False
            Else
                bIsRCBSent = True
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occured in Generate Export data - " + _
                                                               "RLSession Manager", Logger.LogLevel.RELEASE)
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
        Finally
            objRCB = Nothing
        End Try
#End If
        objAppContainer.objLogger.WriteAppLog("RCB written Successfully", Logger.LogLevel.RELEASE)
        objAppContainer.objUODCollection.Add(m_UODNumber)
#If NRF Then
    End Sub
#ElseIf RF Then
        Return True
    End Function
#End If
    ''' <summary>
    ''' Check if the product code scanned/entered same 
    ''' as the product code of the item, when item is selected 
    ''' from View Item List screen
    ''' </summary>
    ''' <returns>True if success else false</returns>
    ''' <remarks></remarks>
    Public Function IsProductCodeSame() As Boolean

    End Function

    ''' <summary>
    ''' Processes the scanned or handkeyed Product Code
    ''' </summary>
    ''' <param name="strBarcode">Scanned Item</param>
    ''' <remarks></remarks>
    Private Sub ProcessScanItem(ByVal strBarcode As String)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ProcessScanItem of  RLSessionMgr", Logger.LogLevel.INFO)
        Try
            'Validate the item against the recall item list.
            If IsRecallListItem(strBarcode) Then
                RLSessionMgr.GetInstance().CallingScreen = RECALLSCREENS.Scan
                RLSessionMgr.GetInstance().DisplayRecallScreen(RECALLSCREENS.ItemDetails)
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in ProcessScanItem of  RLSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit ProcessScanItem of  RLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Process the hand keyed or scanned UOD and store it in class Members
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="bIsManual"></param>
    ''' <remarks></remarks>
    Private Sub ProcessScanUOD(ByVal strBarcode As String, ByVal bIsManual As Boolean)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ProcessScanItem of  RLSessionMgr", Logger.LogLevel.INFO)
        Dim bErrorMessagePrompted As Boolean = False
        Try
            If m_ActionedItemList.Count > 0 Then
                ''pilot support: Recall CR to allow black and grey label
                'If m_CurrentList.RecallType.Equals("PL") Then
                '    'For Planner leaver recalls.
                '    WorkflowMgr.GetInstance().Labelcolour = "PURPLE"
                'Else
                '    'For Company/HO Recalls and Excess sales plan recalls.
                '    'Setting label colour to RLCOLOUR allows to validate for purple, black and grey.
                '    WorkflowMgr.GetInstance().Labelcolour = "RLCOLOUR"
                'End If
                ''Check if the UOD is valid or not
                If (objAppContainer.objHelper.ValidateUOD(strBarcode, bErrorMessagePrompted)) Then
                    'Set the UOD data to the class member
                    objAppContainer.objLogger.WriteAppLog("Recall::UOD scanned = " & strBarcode, Logger.LogLevel.RELEASE)
                    RLSessionMgr.GetInstance().SetUOD(strBarcode)
                    'Go to the next screen
                    RLSessionMgr.GetInstance().CallingScreen = RECALLSCREENS.ScanUOD
                    RLSessionMgr.GetInstance().DisplayRecallScreen(RECALLSCREENS.Despatch)
                Else
                    If Not bErrorMessagePrompted Then
                        If bIsManual Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M13"), _
                             "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                              MessageBoxDefaultButton.Button1)
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), _
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                     MessageBoxDefaultButton.Button1)
                        End If
                    End If

                End If
            Else
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M55"), _
                "Caution", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                MessageBoxDefaultButton.Button1)
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in ProcessScanItem of  RLSessionMgr. Exception is: " _
                                                             + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
#If RF Then
            'This code block clears the barcode text from the text box when there is a connection loss 
            'and connection regained during the retry attempts
            If ex.Message = Macros.CONNECTION_REGAINED Then
                If ((Not m_frmRLScanUOD Is Nothing) AndAlso (Not m_frmRLScanUOD.IsDisposed)) Then
                    m_frmRLScanUOD.txtBarcode.Text = ""
                End If
            End If
#End If
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit ProcessScanItem of  RLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' To reset the hash table containing the actioned item list.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ClearActionedItems() As Boolean
        m_ActionedItemList.Clear()
    End Function
    ''' <summary>
    ''' Clears all the member variables
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ClearData()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ClearData of  RLSessionMgr", Logger.LogLevel.INFO)
        Try
            'Release all objects and Set to nothig.
            m_RecallItemList = Nothing
            'm_RecallInfo = Nothing
            'm_RecallItemInfo = Nothing
            m_UODNumber = Nothing

            m_ActionedItemList = Nothing
            m_DiscrepancyList = Nothing
            m_CurrentItem = Nothing
            m_CurrentList = Nothing
            'Recall CR
            'm_RecallCount = Nothing
            CallingScreen = Nothing
            m_DiscItemNumber = Nothing
            RLSessionMgr.GetInstance().CallingScreen = Nothing

            'Reinstantiating the data
            m_RecallItemList = New Hashtable
            m_ActionedItemList = New Hashtable
            m_DiscrepancyList = New ArrayList
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in ClearData of  RLSessionMgr. Exception is: " _
                                                             + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit ClearData of  RLSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Enum Class that defines all screens for Shelf Monitor module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum RECALLSCREENS
        Scan
        ItemDetails
        ActiveRecallList
        ItemList
        Summary
        Despatch
        ScanUOD
        Discrepancy
        'For Recalls CR,Special Instructution.
        SpclInstructions
        'For loading Active recall list
        Message
    End Enum
End Class
''' <summary>
''' Class to compare the objects in recall list and sort.
''' </summary>
''' <remarks></remarks>
Public Class RecallSorter
    Implements IComparer
    ''' <summary>
    ''' Function to compare the objects.
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
        Dim tempMod1 As RLItemInfo = DirectCast(x, RLItemInfo)
        Dim tempMod2 As RLItemInfo = DirectCast(y, RLItemInfo)

        Return String.Compare(tempMod1.BootsCode, tempMod2.BootsCode)
    End Function
End Class
''' <summary>
''' Class to store the each item in the recall list
''' </summary>
''' <remarks>
''' 25-June-09: Fix for recall CR to include item status: Added new property 
''' for tracking recall status.
''' </remarks>
Public Class RLRecallInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_CompletionDate As String
    Private m_BusinessCentre As String
    Private m_RecallType As String
    Private m_ActiveDate As String
    Private m_RecallDescription As String
    Private m_RecallNumber As String
    'Tailoring
    'Changed to string from integer since TL should be dispalyed
    Private m_RecallQuantity As String
    Private m_ListStatus As String
    'For Special Instruction
    Private m_RecallMessage As String
    Private m_MinRecallQty As Integer
    'Tailoring
    Private m_Tailored As Boolean
    'For UOD Label in Recalls
    Private m_LabelType As String
    'BATCH NOD
    Private m_BatchNos As String
    Public Property BatchNos() As String
        Get
            Return m_BatchNos
        End Get
        Set(ByVal value As String)
            m_BatchNos = value
        End Set
    End Property
    ''' <summary>
    ''' For dispalying Recall List as TL or number of items
    ''' For Batch Mode is determined by Item description (starting with NT* or not)
    ''' For RF Mode it is received from RCC Message Tailored flag 
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns>Boolean</returns>
    ''' <remarks>NIL</remarks>
    Public Property Tailored() As Boolean
        Get
            Return m_Tailored
        End Get
        Set(ByVal value As Boolean)
            m_Tailored = value
        End Set
    End Property
    Public Property LabelType() As String
        Get
            Return m_LabelType
        End Get
        Set(ByVal value As String)
            m_LabelType = value
        End Set
    End Property
    Public ReadOnly Property LabelColourCode() As System.Drawing.Color
        Get
            Select Case m_LabelType
                Case "01"
                    '"BLACK"
                    Return Color.Black
                Case "02"
                    '"GREY"
                    Return Color.Gray
                Case "03"
                    '"YELLOW"
                    Return Color.Yellow
                Case "04"
                    '"ORANGE"
                    Return Color.Orange
                Case "05"
                    '"RED"
                    Return Color.Red
                Case "06"
                    '"WHITE"
                    Return Color.White
                Case "07"
                    '"PURPLE"
                    Return Color.Purple
            End Select
        End Get
    End Property
    Public ReadOnly Property LabelColourName() As String
        Get
            Select Case m_LabelType
                Case "01"
                    '"BLACK"
                    Return "Black"
                Case "02"
                    '"GREY"
                    Return "Gray"
                Case "03"
                    '"YELLOW"
                    Return "Yellow"
                Case "04"
                    '"ORANGE"
                    Return "Orange"
                Case "05"
                    '"RED"
                    Return "Red"
                Case "06"
                    '"WHITE"
                    Return "White"
                Case "07"
                    '"PURPLE"
                    Return "Purple"
                Case Else
                    Return "appropriate"
            End Select
        End Get
    End Property
    ''' <summary>
    ''' Constructor for the class ProductInfo
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub
    ''' <summary>
    ''' To set or get the Boots code.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BusinessCentre() As String
        Get
            Return m_BusinessCentre
        End Get
        Set(ByVal value As String)
            m_BusinessCentre = value
        End Set
    End Property
    ''' <summary>
    ''' Recall Message of an item
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RecallMessage() As String
        Get
            Return m_RecallMessage
        End Get
        Set(ByVal value As String)
            m_RecallMessage = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get Product code.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RecallDescription() As String
        Get
            Return m_RecallDescription
        End Get
        Set(ByVal value As String)
            m_RecallDescription = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get the Product description.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RecallNumber() As String
        Get
            Return m_RecallNumber
        End Get
        Set(ByVal value As String)
            m_RecallNumber = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get the Product status.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RecallQuantity() As String
        Get
            Return m_RecallQuantity
        End Get
        Set(ByVal value As String)
            m_RecallQuantity = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get the Active Type.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RecallType() As String
        Get
            Return m_RecallType
        End Get
        Set(ByVal value As String)
            Select Case value
                Case "S"
                    m_RecallType = "R"
                Case "F"
                    m_RecallType = "E"
                Case "X"
                    m_RecallType = "W"
                Case Else
                    m_RecallType = value
            End Select
        End Set
    End Property
    ''' <summary>
    ''' To set or get the Active date.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ActiveDate() As String
        Get
            Return m_ActiveDate
        End Get
        Set(ByVal value As String)
            m_ActiveDate = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get the Completion date.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CompletionDate() As String
        Get
            Return m_CompletionDate
        End Get
        Set(ByVal value As String)
            m_CompletionDate = value
        End Set
    End Property
    ''' <summary>
    ''' Recall Item status.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ListStatus() As String
        Get
            'Return m_ListStatus
            Select Case m_ListStatus
                Case "A"
                    Return "Completed"
                Case "P"
                    Return "Partial"
                Case Else
                    Return "Unactioned"
            End Select
        End Get
        Set(ByVal value As String)
            m_ListStatus = value
        End Set
    End Property
    ''' <summary>
    ''' Minimum Recall Quantity.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MinRecallQty() As Integer
        Get
            Return m_MinRecallQty
        End Get
        Set(ByVal value As Integer)
            m_MinRecallQty = value
        End Set
    End Property
End Class
''' <summary>
''' Class to store the Item info of each item in the Recall Item list
''' </summary>
''' <remarks>
''' 25-June-09: Fix for recall CR to include item status: Added new property to hold Recall
''' list item status.
''' </remarks>
Public Class RLItemInfo
    Inherits ProductInfo

    Private m_FirstBarcode As String
    Private m_strRecallNumber As String
    Private m_strStockCount As String
    Private m_bScanStatus As Boolean = False
    Private m_RecallStatus As String
    'For Tailoring
    Private m_StockDateMove As String
    Private m_UODCount As String
    'To display the tailored items in the list view
    Private bTailoringflag As Boolean
    ' to store the StockCount added for different UODs 
    Private iAddedStockCount As Integer
    Private bActionedinPreviousUOD As Boolean
    Private bActionedNow As Boolean
    Private bIsRecallTailored As Boolean
    Private bIsPresentInLivePlanner As Boolean
    Public Property IsPresentInLivePlanner() As Boolean
        Get
            Return bIsPresentInLivePlanner
        End Get
        Set(ByVal value As Boolean)
            bIsPresentInLivePlanner = value
        End Set
    End Property
    Public Property IsRecallTailored() As Boolean
        Get
            Return bIsRecallTailored
        End Get
        Set(ByVal value As Boolean)
            bIsRecallTailored = value
        End Set
    End Property
    Public Property ActionedNow() As Boolean
        Get
            Return bActionedNow
        End Get
        Set(ByVal value As Boolean)
            bActionedNow = value
        End Set
    End Property
    Public Property ActionedinPreviousUOD() As Boolean
        Get
            Return bActionedinPreviousUOD
        End Get
        Set(ByVal value As Boolean)
            bActionedinPreviousUOD = value
        End Set
    End Property
    Public Property AddedStockCount() As Integer
        Get
            Return iAddedStockCount
        End Get
        Set(ByVal value As Integer)
            iAddedStockCount = value
        End Set
    End Property
    ''' <summary>
    ''' This is used to check whether the item has to be displayed in the 
    ''' Item View list screen or not
    '''  For Batch mode: it is calculated using the 3 rules
    '''  For RF mode:It is received from Visible flag of RCF message
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns>Boolean</returns>
    ''' <remarks>nil</remarks>
    Public Property TailoringFlag() As Boolean
        Get
            Return bTailoringflag
        End Get
        Set(ByVal value As Boolean)
            bTailoringflag = value
        End Set
    End Property

    Public Property StockDateMove() As String
        Get
            Return m_StockDateMove
        End Get
        Set(ByVal value As String)
            m_StockDateMove = value
        End Set
    End Property
    ''' <summary>
    ''' First barcode of an item.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FirstBarcode() As String
        Get
            Return m_FirstBarcode
        End Get
        Set(ByVal value As String)
            m_FirstBarcode = value
        End Set
    End Property
    ''' <summary>
    ''' Recall list number.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RecallNumber() As String
        Get
            Return m_strRecallNumber
        End Get
        Set(ByVal value As String)
            m_strRecallNumber = value
        End Set
    End Property
    ''' <summary>
    ''' Stock count for a recall list.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StockCount() As String
        Get
            Return m_strStockCount
        End Get
        Set(ByVal value As String)
            m_strStockCount = value
        End Set
    End Property
    ''' <summary>
    ''' Whether an item is scanned or not.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ScanStatus() As Boolean
        Get
            Return m_bScanStatus
        End Get
        Set(ByVal value As Boolean)
            m_bScanStatus = value
        End Set
    End Property
    ''' <summary>
    ''' Recall Item status.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RecallItemStatus() As String
        Get
            Return m_RecallStatus
        End Get
        Set(ByVal value As String)
            m_RecallStatus = value
        End Set
    End Property
    ''' <summary>
    ''' Proeprty to hold UOD Count for an item.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UODCount() As String
        Get
            If m_UODCount.Trim.Equals("") Then
                m_UODCount = "-1"
            End If
            Return m_UODCount
        End Get
        Set(ByVal value As String)
            m_UODCount = value
        End Set
    End Property

End Class
''' <summary>
''' Recall CR
''' Class to store the RecallCount for Each Type
''' </summary>
''' <remarks>
'''
''' </remarks>
Public Class RecallCount
    Private i_Customer As Integer
    Private i_Withdrawn As Integer
    Private i_Returns As Integer
    Private i_PlannerLeaver As Integer
    Private i_ExcessSalesPlan As Integer
    Public Property Customer() As Integer
        Get
            Return i_Customer
        End Get
        Set(ByVal value As Integer)
            i_Customer = value
        End Set
    End Property
    Public Property Withdrawn() As Integer
        Get
            Return i_Withdrawn
        End Get
        Set(ByVal value As Integer)
            i_Withdrawn = value
        End Set
    End Property
    Public Property Returns() As Integer
        Get
            Return i_Returns
        End Get
        Set(ByVal value As Integer)
            i_Returns = value
        End Set
    End Property
    Public Property PlannerLeaver() As Integer
        Get
            Return i_PlannerLeaver
        End Get
        Set(ByVal value As Integer)
            i_PlannerLeaver = value
        End Set
    End Property
    Public Property ExcessSalesPlan() As Integer
        Get
            Return i_ExcessSalesPlan
        End Get
        Set(ByVal value As Integer)
            i_ExcessSalesPlan = value
        End Set
    End Property
End Class
Public Class RecallStatusComparer
    Implements IComparer
    Private bSorting As Boolean = False
    ''' <summary>
    ''' To sort the Recall List according to Recall Status and Recall Number
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
        Dim objRecallX As RLRecallInfo = DirectCast(x, RLRecallInfo)
        Dim objRecallY As RLRecallInfo = DirectCast(y, RLRecallInfo)
        Dim iReturn As Integer = String.Compare(objRecallY.ListStatus, objRecallX.ListStatus)
        If iReturn = 0 Then
            iReturn = IIf(True, Compare(objRecallX, objRecallY), Compare(objRecallY, objRecallX))
        End If

        Return iReturn

    End Function
    Public Function Compare(ByVal objRecallA As RLRecallInfo, ByVal objRecallB As RLRecallInfo) As Integer
        Return Int64.Parse(objRecallA.RecallNumber).CompareTo(Int64.Parse(objRecallB.RecallNumber))
    End Function
End Class
' Implementation of the IComparer 
' interface for sorting ArrayList items.
Public Class SortComparer
    Implements IComparer
    Private bAscending As Boolean


    ' Constructor requires the sort order;
    ' true if ascending, otherwise descending.
    'Public Sub New(ByVal asc As Boolean)
    '    bAscending = asc
    'End Sub


    ' Implemnentation of the IComparer:Compare 
    ' method for comparing two objects.
    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
        Dim xItem As ListViewItem = DirectCast(x, ListViewItem)
        Dim yItem As ListViewItem = DirectCast(y, ListViewItem)
        Dim iReturn As Integer
        Dim xTextStatus As String = xItem.SubItems(3).Text
        Dim yTextStatus As String = yItem.SubItems(3).Text
        iReturn = String.Compare(yTextStatus, xTextStatus)
        If iReturn = 0 Then
            Dim xTextRclNmbr As String = xItem.SubItems(0).Text
            Dim yTextRclNmbr As String = yItem.SubItems(0).Text
            iReturn = IIf(True, Compare(xTextRclNmbr, yTextRclNmbr), Compare(yTextRclNmbr, xTextRclNmbr))
        End If
        Return iReturn
    End Function
    Public Function Compare(ByVal x As String, ByVal y As String) As Integer
        Return Int64.Parse(x).CompareTo(Int64.Parse(y))
    End Function
End Class




