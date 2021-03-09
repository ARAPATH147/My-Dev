'''***************************************************************
''' <FileName>GoodsOutManager.vb</FileName>
''' <summary>
''' This is a generalized base class which contains implementation
''' of commonly used goods out functionalities
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
Public Class GoodsOutManager
    'Declaring the Form objects
    Protected m_frmGOScan As frmScan
    Protected m_frmGOItemDetails As frmGOItemDetails
    Protected m_frmScanUOD As frmScanUOD
    Protected m_frmDespatch As frmGODespatch
    Protected m_frmGOSummary As frmSummary

    'Declaring the objects that store items in a list
    Protected m_objGOItemInfo As GOItemInfo = Nothing
    Protected m_GOItemList As ArrayList = Nothing


    'Stores the Transaction Data held within the session
    Protected m_BusinessCentreType As String = Nothing
    Protected m_SupplyRoute As String = Nothing
    Protected m_UODNumber As String = Nothing
    Protected m_SequenceNumber As Integer = 1
    ''' <summary>
    ''' Initialises the Goods out manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Function InitGoodsOutManager() As Boolean
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered InitGoodsOutManager of GoodsOutManager", Logger.LogLevel.INFO)
        Try
#If RF Then
            If objAppContainer.eLastUsedModule = "None" Or _
              (Not objAppContainer.eLastUsedModule.Equals("None") And Not _
              objAppContainer.eLastUsedModule.Equals(WorkflowMgr.GetInstance().WFIndex)) Then
                Dim objUOS As UOSRecord = New UOSRecord()
                objUOS.strIsListType = "G"      'D - denoted it is sent for goods out module.
                'If RF Mode then send UOS right here.
                If Not objAppContainer.objExportDataManager.CreateUOS(objUOS) Then
                    objUOS = Nothing
                    Return False
                End If
                'Update the work flow index to public variable
                objAppContainer.eLastUsedModule = WorkflowMgr.GetInstance().WFIndex
                objUOS = Nothing
                objAppContainer.objLogger.WriteAppLog("Succeeded UOS sending", Logger.LogLevel.RELEASE)
                m_GOItemList = New ArrayList()
            Else
                If objAppContainer.eLastUsedModule.Equals(WorkflowMgr.GetInstance().WFIndex) Then
                    'Instantiating a class to store the CCItemInfo objects
                    'Instantiating a class to store the CCItemInfo objects
                    m_GOItemList = New ArrayList()
                    m_GOItemList = objAppContainer.objGlobalItemList
                    m_SupplyRoute = objAppContainer.stSession.m_SupplierType
                    m_BusinessCentreType = objAppContainer.stSession.m_BCType
                End If
            End If
#End If
            'All the Goods out related forms are instantiated.
            m_frmGOScan = New frmScan
            m_frmGOItemDetails = New frmGOItemDetails
            m_frmGOSummary = New frmSummary
            m_frmScanUOD = New frmScanUOD
            m_frmDespatch = New frmGODespatch
#If NRF Then
            'Instantiating a class to store the GOItemInfo objects
            m_GOItemList = New ArrayList()
#End If
            Return True
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_REGAINED Or ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Then
                Throw ex
            End If
#End If
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in InitGoodsOutManager of GoodsOutManager. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        Finally
            'Writing to Log INFO File while exit
            objAppContainer.objLogger.WriteAppLog("Exit InitGoodsOutManager of GoodsOutManager", Logger.LogLevel.INFO)
        End Try
    End Function

    ''' <summary>
    ''' Sets the Product info in GOItemInfo into the array list
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetProductInfo(Optional ByVal strItemStatus As String = "A")
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered SetProductInfo of GoodsOutManager", Logger.LogLevel.INFO)
        Try
            Dim bAlreadyScanned As Boolean = False
            Dim objTemp As GOItemInfo
            'Read the item quantity form the label
            m_objGOItemInfo.Quantity = m_frmGOItemDetails.lblQuantityData.Text
#If NRF Then
            'If item is already scanned remove the item object and add a new item
            ' object with appended quantity data
            For Each objItemInfo As GOItemInfo In m_GOItemList
                If objItemInfo.ProductCode = m_objGOItemInfo.ProductCode Or _
                objItemInfo.FirstBarcode = m_objGOItemInfo.FirstBarcode Then
                    'Read the sequence number to update the same item in the list CLLOL
                    m_objGOItemInfo.SequenceNumber = objItemInfo.SequenceNumber
                    If strItemStatus = "X" Or objItemInfo.Quantity <> m_objGOItemInfo.Quantity Then
                        m_GOItemList.Remove(objItemInfo)
                        Exit For
                    Else
                        Exit Sub
                    End If
                End If
            Next

            If strItemStatus = "A" Then
                objAppContainer.objLogger.WriteAppLog("GoodsOutManager::SetProductInfo: Quantity Entered =" & m_frmGOItemDetails.lblQuantityData.Text, Logger.LogLevel.RELEASE)
                '@Service Fix
                'Fix for item not on file
                If m_GOItemList.Count > 0 Then
                    objTemp = m_GOItemList.Item(0)
                    If objTemp.BusinessCentreType = "" And m_objGOItemInfo.BusinessCentreType <> "" Then
                        objTemp.BusinessCentreType = m_objGOItemInfo.BusinessCentreType
                        m_BusinessCentreType = m_objGOItemInfo.BusinessCentreType
                        'Fix
                        m_GOItemList.RemoveAt(0)
                        m_GOItemList.Insert(0, objTemp)
                    ElseIf m_objGOItemInfo.BusinessCentreType = "" And objTemp.BusinessCentreType <> "" Then
                        m_objGOItemInfo.BusinessCentreType = objTemp.BusinessCentreType
                        m_BusinessCentreType = m_objGOItemInfo.BusinessCentreType
                    End If
                End If
                '@Service Fix  End
                m_GOItemList.Add(m_objGOItemInfo)
            End If
#End If
#If RF Then
            'Set the sequence number in order track the item in case of rf mode.
            If m_objGOItemInfo.SequenceNumber = "" Then
                m_objGOItemInfo.SequenceNumber = m_SequenceNumber.ToString()
                'm_SequenceNumber += 1
            End If
            Dim objUOA As UOARecord = New UOARecord()
            Try
                'In case of RF mode send the UOA message right here.
                Dim totalPrice As Double = 0.0
                m_frmGOItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing UOA: " + _
                                                            m_objGOItemInfo.BootsCode)
                objUOA.strsequencenumber = CStr(m_objGOItemInfo.SequenceNumber)
                objUOA.strbootscode = m_objGOItemInfo.BootsCode
                objUOA.strquanity = m_objGOItemInfo.Quantity
                objUOA.strsdescription = m_objGOItemInfo.ShortDescription
                objUOA.strdescSEL = m_objGOItemInfo.Description
                objUOA.strnumPrice = m_objGOItemInfo.ItemPrice
                totalPrice = CDbl(m_objGOItemInfo.ItemPrice) * CDbl(m_objGOItemInfo.Quantity)
                objUOA.strnumTotalPrice = CStr(totalPrice)
                objUOA.stritembc = m_objGOItemInfo.BusinessCentreType
                objUOA.strbcname = ConfigDataMgr.GetInstance().GetParam(m_objGOItemInfo.BusinessCentreType)
                objUOA.strbarcode = m_objGOItemInfo.ProductCode
                'TODO: this status will always be A since the we dont send a UOA for cancelling an item
                objUOA.strIsStatus = strItemStatus

                'Get the data from the DAL and then add the supply route to this variable
                If objAppContainer.objExportDataManager.CreateUOA(objUOA) Then
                    'If item is already scanned remove the item object and add a new item
                    ' object with appended quantity data
                    For Each objItemInfo As GOItemInfo In m_GOItemList
                        If objItemInfo.ProductCode = m_objGOItemInfo.ProductCode Or _
                        objItemInfo.FirstBarcode = m_objGOItemInfo.FirstBarcode Then
                            bAlreadyScanned = True
                            'Read the sequence number to update the same item in the list CLLOL
                            m_objGOItemInfo.SequenceNumber = objItemInfo.SequenceNumber
                            If strItemStatus = "X" Or objItemInfo.Quantity <> m_objGOItemInfo.Quantity Then
                                m_GOItemList.Remove(objItemInfo)
                                Exit For
                            Else
                                Exit Sub
                            End If
                        End If
                    Next
                    If strItemStatus = "A" Then
                        objAppContainer.objLogger.WriteAppLog("GoodsOutManager::SetProductInfo: Quantity Entered =" & m_frmGOItemDetails.lblQuantityData.Text, Logger.LogLevel.RELEASE)
                        '@Service Fix
                        'Fix for item not on file
                        If m_GOItemList.Count > 0 Then
                            objTemp = m_GOItemList.Item(0)
                            If objTemp.BusinessCentreType = "" And m_objGOItemInfo.BusinessCentreType <> "" Then
                                objTemp.BusinessCentreType = m_objGOItemInfo.BusinessCentreType
                                m_BusinessCentreType = m_objGOItemInfo.BusinessCentreType
                                 'Fix
                                m_GOItemList.RemoveAt(0)
                                m_GOItemList.Insert(0, objTemp)
                            ElseIf m_objGOItemInfo.BusinessCentreType = "" And objTemp.BusinessCentreType <> "" Then
                                m_objGOItemInfo.BusinessCentreType = objTemp.BusinessCentreType
                                m_BusinessCentreType = m_objGOItemInfo.BusinessCentreType
                            End If
                         End If
                        '@Service Fix  End
                        m_GOItemList.Add(m_objGOItemInfo)
                    End If
                    If strItemStatus <> "X" And Not bAlreadyScanned Then
                        m_SequenceNumber += 1
                        'Return False
                    End If
                End If
                objAppContainer.objLogger.WriteAppLog("Credit Claim : UOA written successfully", Logger.LogLevel.RELEASE)
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("Credit Claim : UOA writing Failed " + ex.Message, Logger.LogLevel.RELEASE)
                If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                    Throw ex
                End If
            Finally
                objUOA = Nothing
            End Try

#End If
            'This condition will set the business centre type of the first item 
            'scanned as the supply route and the business centre type
            If m_GOItemList.Count = 1 Then
                If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.TRANSFERS Then
                    m_BusinessCentreType = GOTransferMgr.GetInstance().m_objGOItemInfo.BusinessCentreType
                    m_SupplyRoute = GOTransferMgr.GetInstance().m_objGOItemInfo.SupplyRoute
                Else
                    m_BusinessCentreType = GOSessionMgr.GetInstance().m_objGOItemInfo.BusinessCentreType
                    m_SupplyRoute = GOSessionMgr.GetInstance().m_objGOItemInfo.SupplyRoute
                End If
            End If
            m_objGOItemInfo = Nothing
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in SetProductInfo of GoodsOutManager. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit SetProductInfo of GoodsOutManager", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by the class.
    ''' </summary>
    ''' <returns>True if terminate is sucess else False</returns>
    ''' <remarks></remarks>
    Public Function TerminateGoodsOutManager(Optional ByVal bIsConnectFailed As Boolean = False)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered TerminateGoodsOutManager of GoodsOutManager", Logger.LogLevel.INFO)
        Try
            'Save and data and perform all Exit Operations.
            'Close and Dispose all forms.

            'm_frmGOScan.Close()
            m_frmGOScan.Dispose()

            'm_frmGOItemDetails.Close()
            m_frmGOItemDetails.Dispose()

            ' m_frmGOSummary.Close()
            m_frmGOSummary.Dispose()

            'm_frmScanUOD.Close()
            m_frmScanUOD.Dispose()

            'm_frmDespatch.Close()
            m_frmDespatch.Dispose()
#If RF Then
            If bIsConnectFailed Then
                objAppContainer.objGlobalItemList = New ArrayList()
                objAppContainer.objGlobalItemList = m_GOItemList
                objAppContainer.eLastUsedModule = WorkflowMgr.GetInstance().WFIndex
                objAppContainer.stSession.m_BCType = m_BusinessCentreType
                objAppContainer.stSession.m_SupplierType = m_SupplyRoute
            Else
                objAppContainer.eLastUsedModule = "None"
                objAppContainer.stSession = New AppContainer.SessionVariables()
                objAppContainer.objGlobalItemList = Nothing
                'Closing the item list array object
                m_GOItemList = Nothing
            End If
#ElseIf NRF Then
            'Closing the item list array object
            m_GOItemList = Nothing
#End If
            'Release all objects and Set to nothig.
            m_objGOItemInfo = Nothing
            m_BusinessCentreType = Nothing
            m_UODNumber = Nothing
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in TerminateGoodsOutManager of GoodsOutManager. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit TerminateGoodsOutManager of GoodsOutManager", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Populate and display the Product Scan screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub DisplayGOScan(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayGOScan of GoodsOutManager", Logger.LogLevel.INFO)
        Try
            With m_frmGOScan
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                .objProduct.txtProductCode.Text = ""
                .btnReturn.Visible = False
                .btnFinish.Visible = False
                .btnDestroy.Visible = False
                If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.RETURNS Then
                    'Retrun Button is only visible if atleast one item is scanned
                    If m_GOItemList.Count > 0 Then
                        .btnReturn.Visible = True
                    End If
                ElseIf WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.DESTROY Or _
                WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.FIREFLOOD Then
                    'Destroy Button is only visible if atleast one item is scanned
                    If m_GOItemList.Count > 0 Then
                        .btnDestroy.Visible = True
                    End If
                ElseIf WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.SEMICENTRALISED Then
                    If m_GOItemList.Count > 0 Then
                        '.btnFinish.Visible = True
                        .btnReturn.Visible = True
                    End If
                Else
                    'Retrun Finish is only visible if atleast one item is scanned
                    If m_GOItemList.Count > 0 Then
                        .btnFinish.Visible = True
                    End If
                End If
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayGOScan of GoodsOutManager. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmGOScan.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayGOScan of GoodsOutManager", Logger.LogLevel.INFO)

    End Sub
    ''' <summary>
    ''' Populate and display the Item details screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub DisplayGOItemDetails(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayGOItemDetails of GoodsOutManager", Logger.LogLevel.INFO)
        Try
            'Creating an object of the Class that stores the item info
            m_frmGOItemDetails.lblQuantityData.Text = "1"
            'Check if the item is already scanned if yes then show the message
            For Each objItemInfo As GOItemInfo In m_GOItemList
                If (objItemInfo.ProductCode = m_objGOItemInfo.ProductCode) Or (objItemInfo.FirstBarcode = m_objGOItemInfo.FirstBarcode) Then
                    'If objItemInfo.ProductCode = m_objGOItemInfo.ProductCode Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M59"), _
                    "Item Added", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                    MessageBoxDefaultButton.Button1)
                    'Add a count of 1 to the existing item count.
                    If Val(objItemInfo.Quantity) < 999 Then
                        m_frmGOItemDetails.lblQuantityData.Text = objItemInfo.Quantity + 1
                    Else
                        'Display message here to inform user that count is not incremented.
                        MessageBox.Show("Quantity cannot be more than 999. Previous count will be displayed.", _
                                        "Count Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                        MessageBoxDefaultButton.Button1)
                        m_frmGOItemDetails.lblQuantityData.Text = objItemInfo.Quantity
                    End If
                    m_objGOItemInfo.SequenceNumber = objItemInfo.SequenceNumber
                    Exit For
                End If
            Next
            Dim objDescriptionArray As ArrayList = New ArrayList()
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(m_objGOItemInfo.Description)
            With m_frmGOItemDetails
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                'Fix for handling item not on file case
                If m_objGOItemInfo.BusinessCentreType = "" Then
                    .lblBusCentreDesc.Text = ""
                Else
                    .lblBusCentreDesc.Text = objAppContainer.objHelper.FormatEscapeSequence _
                                        (ConfigDataMgr.GetInstance().GetParam(m_objGOItemInfo.BusinessCentreType))
                End If
                .lblBootsCode.Text = objAppContainer.objHelper.FormatBarcode(m_objGOItemInfo.BootsCode)
                .lblEAN.Text = objAppContainer.objHelper.FormatBarcode(objAppContainer.objHelper.GeneratePCwithCDV(m_objGOItemInfo.ProductCode))
                .lblItemDesc1.Text = objAppContainer.objHelper.FormatEscapeSequence(objDescriptionArray.Item(0))
                .lblItemDesc2.Text = objAppContainer.objHelper.FormatEscapeSequence(objDescriptionArray.Item(1))
                .lblItemDesc3.Text = objAppContainer.objHelper.FormatEscapeSequence(objDescriptionArray.Item(2))
                If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.DESTROY Or _
                WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.FIREFLOOD Then
                    .lblInstruction.Text = "Items to be Destroyed"
                Else
                    .lblInstruction.Text = "Items to be Returned|Place in UOD"
                End If
                .Visible = True
                .Refresh()
            End With
            objDescriptionArray = Nothing
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayGOItemDetails of GoodsOutManager. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmGOItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayGOItemDetails of GoodsOutManager", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Display the UOD scan screen with the item scan list and the total items in UOD
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub DisplayScanUODScreen(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayScanUODScreen of GoodsOutManager", Logger.LogLevel.INFO)
        Try
            'TODO: Write code in loop
            m_frmScanUOD.lwItemList.Items.Clear()
            For Each m_Item As GOItemInfo In m_GOItemList
                Dim objListItem As ListViewItem
                objListItem = m_frmScanUOD.lwItemList.Items.Add(New ListViewItem(m_Item.Quantity))
                objListItem.SubItems.Add(m_Item.ShortDescription)
            Next

            'Populating the controls
            With m_frmScanUOD
                .btnNext.Visible = False
                'Altering the display of controls for Destory scenario
                If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.DESTROY _
                Or WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.STOCKTAKE _
                Or WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.FIREFLOOD Then
                    .btnNext.Visible = True
                    .lblLabel.Visible = False
                    .txtBarcode.Enabled = False
                    .txtBarcode.Visible = False
                    .btnCalcpad.Visible = False
                    .pnScanLabelColourIndicator.Visible = False
                    .lblScanColour.Visible = False
                End If
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                If m_BusinessCentreType = "" Then
                    .lblBusCentreDesc.Text = ""
                Else
                    .lblBusCentreDesc.Text = objAppContainer.objHelper.FormatEscapeSequence _
                                                           (ConfigDataMgr.GetInstance().GetParam(m_BusinessCentreType))
                End If
                .lblTotalData.Text = Me.GetUODItemCount().ToString
                .pnScanLabelColourIndicator.BackColor = WorkflowMgr.GetInstance().FetchLabelColourCode
                .lblScanColour.Text = "Scan " & WorkflowMgr.GetInstance().Labelcolour & " Label"
                .Visible = True
                .Refresh()

            End With
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayScanUODScreen of GoodsOutManager. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmScanUOD.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try

        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayScanUODScreen of GoodsOutManager", Logger.LogLevel.INFO)

    End Sub
    ''' <summary>
    ''' Populate and display the Summary Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub DisplayGOSummary(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayGOSummary of GoodsOutManager", Logger.LogLevel.INFO)
        Try
            If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.DESTROY _
                    Or WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.STOCKTAKE Then
                m_frmGOSummary.lblUOD.Visible = False
                m_frmGOSummary.lblUODData.Visible = False
            End If
            With m_frmGOSummary
                .btnOk.Enabled = True
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                .lblBusCentreDesc.Text = objAppContainer.objHelper.FormatEscapeSequence _
                                        (ConfigDataMgr.GetInstance().GetParam(m_BusinessCentreType))
                .lblUODData.Text = m_UODNumber
                .lblTotSinglesData.Text = GetUODItemCount().ToString
                .lblTotValueData.Text = CalculateTotalValue().ToString("0.00")
                If ConfigDataMgr.GetInstance().GetParam(ConfigKey.VALID_CURRENCY).Equals("S") Then
                    .lblCurrSymbol.Text = Macros.POUND_SYMBOL
                Else
                    .lblCurrSymbol.Text = Macros.EURO_SYMBOL
                End If
#If RF Then
                'FIX for BTCPR00004783(Collect Advice of Content wording does not appear on PPC)
                '.lblCompleteInstruction.Visible = False
                .lblCompleteInstruction.Text = "Collect Advice of Content"
#End If
                .Visible = True
                .Refresh()
                'Set active module to none so that in case of auto logoff
                'export data is not written for the second time.
                objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.NONE
            End With
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayGOSummary of GoodsOutManager. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmGOSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayGOSummary of GoodsOutManager", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Populate and display the Despatch Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub DisplayGODespatch(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayGODespatch of GoodsOutManager", Logger.LogLevel.INFO)
        Try
            'UOD not displayed in Despatch form
            If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.DESTROY _
            Or WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.STOCKTAKE Then
                m_frmDespatch.lblUOD.Visible = False
                m_frmDespatch.lblUODData.Visible = False
            End If
            'Fix for item not on file
            With m_frmDespatch
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                If m_BusinessCentreType = "" Then
                    .lblBusCentreDesc.Text = ""
                Else
                    .lblBusCentreDesc.Text = objAppContainer.objHelper.FormatEscapeSequence _
                                                            (ConfigDataMgr.GetInstance().GetParam(m_BusinessCentreType))
                End If
                .lblUODData.Text = m_UODNumber
                .lblSingleData.Text = GetUODItemCount().ToString
                'If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.DESTROY Then
                '    .lblCompleteInstruction.Text = "Destroy UOD now by clicking the Despatch button below."

                'Else
                .lblCompleteInstruction.Text = "Despatch UOD now by clicking the Despatch button below."
                'End If
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayGODespatch of GoodsOutManager. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmDespatch.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayGODespatch of GoodsOutManager", Logger.LogLevel.INFO)

    End Sub
    ''' <summary>
    ''' Clear the Form controls
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ClearData()
        With m_frmGOItemDetails
            .lblBootsCode.Text = ""
            .lblBusCentreDesc.Text = ""
            .lblEAN.Text = ""
            .lblItemDesc1.Text = ""
            .lblItemDesc2.Text = ""
            .lblItemDesc3.Text = ""
            .lblQuantityData.Text = "1"
        End With
    End Sub
    ''' <summary>
    ''' Fetches the items scanned and returns it in the form of an arraylist
    ''' </summary>
    ''' <returns>List of items scanned</returns>
    ''' <remarks></remarks>
    Public Function GetUODItemList() As ArrayList
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered GetUODItemList of GoodsOutManager", Logger.LogLevel.INFO)

        Try
            If m_GOItemList IsNot Nothing Then
                'Writing to Log INFO File while exit
                objAppContainer.objLogger.WriteAppLog("Exit GetUODItemList of GoodsOutManager", Logger.LogLevel.INFO)
                Return m_GOItemList
            Else
                'Writing to Log INFO File while exit
                objAppContainer.objLogger.WriteAppLog("Exit GetUODItemList of GoodsOutManager", Logger.LogLevel.INFO)
                Return Nothing
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in GetUODItemList of GoodsOutManager. Exception is: " _
                                                             + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return Nothing
        End Try
        
    End Function
   
    Public Function GetUODItemCount() As Integer
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered GetUODItemCount of GoodsOutManager", Logger.LogLevel.INFO)
        Dim m_TempUODCount As Integer
        Try


            For Each m_TempList As GOItemInfo In m_GOItemList
                m_TempUODCount += m_TempList.Quantity
            Next
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in GetUODItemCount of GoodsOutManager. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit GetUODItemCount of GoodsOutManager", Logger.LogLevel.INFO)
        Return m_TempUODCount
    End Function
    ''' <summary>
    ''' Calculate the total price of all the items in UOD
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CalculateTotalValue() As Double
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered CalculateTotalValue of GoodsOutManager", Logger.LogLevel.INFO)
        Dim dTotal As Double = 0.0
        Try
            For Each m_Item As GOItemInfo In m_GOItemList
                dTotal = dTotal + (CDbl(m_Item.ItemPrice) * CDbl(m_Item.Quantity))
            Next
            If Not dTotal = 0 Then
                dTotal = dTotal / 100
            End If
            objAppContainer.objLogger.WriteAppLog("GoodsOutManager::CalculateTotalValue: Total Price calculated =" & dTotal, Logger.LogLevel.RELEASE)
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in CalculateTotalValue of GoodsOutManager. Exception is: " _
                                                        + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit CalculateTotalValue of GoodsOutManager", Logger.LogLevel.INFO)
        Return dTotal

    End Function
    ''' <summary>
    ''' Function deletes a selected item from the GO item list
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub VoidItemInfo()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered VoidItemInfo of GoodsOutManager", Logger.LogLevel.INFO)
        Try
            Dim indexes As ListView.SelectedIndexCollection = m_frmScanUOD.lwItemList.SelectedIndices
            Dim index As Integer
            For Each index In indexes
                For Each m_Item As GOItemInfo In m_GOItemList
                    If m_Item.ShortDescription = m_frmScanUOD.lwItemList.Items(index).SubItems(1).Text Then
                        If MsgBox("Are you sure?", MsgBoxStyle.OkCancel, "Void Selected Item") = MsgBoxResult.Ok Then
                            objAppContainer.objLogger.WriteAppLog("GoodsOutManager::Voided items=" & m_Item.ShortDescription, Logger.LogLevel.RELEASE)
                            m_frmScanUOD.lwItemList.Items.Remove(m_frmScanUOD.lwItemList.Items(index))
                            'm_GOItemList.Remove(m_Item)
                            m_objGOItemInfo = m_Item    'Set the item details to current item
                            SetProductInfo("X")         'Delete from the array list and send UOA cancel message.
                            'If all items are voided then hide the NEXT Button
                            If m_GOItemList.Count < 1 Then
                                m_frmScanUOD.btnNext.Visible = False
                            End If
                        End If
                        Exit For
                    End If
                Next
            Next
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in VoidItemInfo of GoodsOutManager. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit VoidItemInfo of GoodsOutManager", Logger.LogLevel.INFO)

    End Sub
    ''' <summary>
    ''' Stores the UOD number so that it can be sent to export data later
    ''' </summary>
    ''' <param name="sUODNumber">UOD number</param>
    ''' <remarks></remarks>
    Public Sub SetUOD(ByVal sUODNumber As String)
        m_UODNumber = sUODNumber
    End Sub
    ''' <summary>
    ''' ValidateBusinessCentreType 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateBusinessCentreType() As Boolean
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ValidateBusinessCentreType of GoodsOutManager", Logger.LogLevel.INFO)
        Dim objTemp As GOItemInfo
        Try
            If m_GOItemList.Count > 0 Then
                objTemp = m_GOItemList.Item(0)
                If objTemp.BusinessCentreType <> "" And m_objGOItemInfo.BusinessCentreType <> "" Then
                    If Not objTemp.BusinessCentreType = m_objGOItemInfo.BusinessCentreType Then
                        objAppContainer.objLogger.WriteAppLog("Exit ValidateBusinessCentreType of GoodsOutManager", Logger.LogLevel.INFO)
                        Return False
                    End If
                ElseIf objTemp.BusinessCentreType = "" And m_objGOItemInfo.BusinessCentreType <> "" Then
                    objTemp.BusinessCentreType = m_objGOItemInfo.BusinessCentreType
                    'Fix
                    m_BusinessCentreType = m_objGOItemInfo.BusinessCentreType
                    m_GOItemList.RemoveAt(0)
                    m_GOItemList.Insert(0, objTemp)
                ElseIf m_objGOItemInfo.BusinessCentreType = "" And objTemp.BusinessCentreType <> "" Then
                    m_objGOItemInfo.BusinessCentreType = objTemp.BusinessCentreType
                    'Fix
                    m_BusinessCentreType = objTemp.BusinessCentreType
                End If
            End If
            'Writing to Log INFO File while exit
            objAppContainer.objLogger.WriteAppLog("Exit ValidateBusinessCentreType of GoodsOutManager", Logger.LogLevel.INFO)
            Return True
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in ValidateBusinessCentreType of GoodsOutManager. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateSupplyRoute() As Boolean
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ValidateSupplyRoute of GoodsOutManager", Logger.LogLevel.INFO)
        Dim objTemp As GOItemInfo
        Try
            If m_GOItemList.Count > 0 Then
                objTemp = m_GOItemList.Item(0)
                'Fix for Mixed supply routes in Goods Out
                If objTemp.SupplyRoute <> "" And m_objGOItemInfo.SupplyRoute <> "" Then
                    If Not objTemp.SupplyRoute = m_objGOItemInfo.SupplyRoute Then
                        objAppContainer.objLogger.WriteAppLog("Exit ValidateSupplyRoute of GoodsOutManager", Logger.LogLevel.INFO)
                        Return False
                        'objAppContainer.objLogger.WriteAppLog("ValidateSupplyRoute of GoodsOutManager", Logger.LogLevel.INFO)
                        'Return True
                    End If
                ElseIf objTemp.SupplyRoute = "" And m_objGOItemInfo.SupplyRoute <> "" Then
                    objTemp.SupplyRoute = m_objGOItemInfo.SupplyRoute
                    m_SupplyRoute = m_objGOItemInfo.SupplyRoute
                    m_GOItemList.RemoveAt(0)
                    m_GOItemList.Insert(0, objTemp)
                ElseIf m_objGOItemInfo.SupplyRoute = "" And objTemp.SupplyRoute <> "" Then
                    m_objGOItemInfo.SupplyRoute = objTemp.SupplyRoute
                    m_SupplyRoute = objTemp.SupplyRoute
                End If
            End If
            'Writing to Log INFO File while exit
            objAppContainer.objLogger.WriteAppLog("Exit ValidateSupplyRoute of GoodsOutManager", Logger.LogLevel.INFO)
            Return True
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in ValidateSupplyRoute of GoodsOutManager. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Validates the quantity entered by the user
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateQuantity(ByVal strQuantity As String) As Boolean
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ValidateQuantity of GoodsOutManager", Logger.LogLevel.INFO)
        Dim lQuantity As Long
        lQuantity = CLng(strQuantity)
        Try
            If objAppContainer.objHelper.ValidateZeroQty(lQuantity) Then
                'Writing to Log INFO File while exit
                objAppContainer.objLogger.WriteAppLog("Exit ValidateQuantity of GoodsOutManager", Logger.LogLevel.INFO)
                Return True
            Else
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M10"), _
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                 MessageBoxDefaultButton.Button1)
                'Writing to Log INFO File while exit
                objAppContainer.objLogger.WriteAppLog("Exit ValidateQuantity of GoodsOutManager", Logger.LogLevel.INFO)
                Return False
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in ValidateQuantity of GoodsOutManager. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try

    End Function
    Structure Supplier
        Dim SupplierNo As Integer
        Dim SupplierName As String
    End Structure
    'Structure to store the export data
    Structure UOXData
        Dim sListType As String
        Dim sUOD As String
        Dim sStatus As String
        Dim sItemCount As String
        Dim sStockFig As String
        Dim sSupplierRoute As String
        Dim sDisplayLocation As String
        Dim sBC As String
        Dim sBusinessCenterDesc As String
        Dim sRecallNumber As String
        Dim sAuthNumber As String
        Dim sSupplier As String
        Dim sMethodOfReturn As String
        Dim sCarrier As String
        Dim sBird As String             'This is blank
        Dim sReasonNumber As String
        Dim sReceivingStore As String
        Dim sDestination As String
        Dim sWareHouseRoute As String   ' This is blank
        Dim sUODType As String
        Dim sDamageReason As String
    End Structure
    Structure UOAData
        Dim SupplierNo As Integer
        Dim SupplierName As String
    End Structure
    Structure UOSData
        Dim SupplierNo As Integer
        Dim SupplierName As String
    End Structure
End Class
