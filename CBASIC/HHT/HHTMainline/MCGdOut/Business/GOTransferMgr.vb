'''***************************************************************
''' <FileName>GOTransferMgr.vb</FileName>
''' <summary>
''' This class will implement the functionality for  
''' Goods out Transfers sub module.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>01-Dec-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************

Public Class GOTransferMgr
    Inherits GoodsOutManager
    'Declaring the Form objects
    Private m_frmDestinationStore As frmDestinationStore

    'Declaring the objects that store items in a list
    Private Shared m_objGOTransferMgr As GOTransferMgr = Nothing

    'Stores the Transaction Data held within the session
    Private m_Storeid As String = Nothing

    'Fix for item not on file
    Private m_BCType As String = Nothing
    'Private m_BCDesc As String = Nothing
    Private m_SupplierType As String = Nothing



    ''' <summary>
    ''' Constructor initiates data 
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()
        Try
            'Instantiate all the objects required
            ' Me.StartSession()
        Catch ex As Exception
            'Handle Goods out Init Exception here.
            Me.EndSession()
        End Try


    End Sub
    ''' <summary>
    ''' Shared Function to return the object of the class singleton
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As GOTransferMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.GDSTFR
        If m_objGOTransferMgr Is Nothing Then
            m_objGOTransferMgr = New GOTransferMgr
        End If
        Return m_objGOTransferMgr
    End Function
    ''' <summary>
    ''' Initialises the Goods out Session 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function StartSession() As Boolean
        Try
            If Not Me.InitGoodsOutManager() Then
                Return False
            End If
            'All the Goods out related forms are instantiated.
            m_frmDestinationStore = New frmDestinationStore
            Return True
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_REGAINED Or ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Then
                Throw ex
            End If
#End If
            Return False
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occured @ :" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Function

    ''' <summary>
    ''' Updates the status bar message in all forms
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBar()
        Try
            m_frmDestinationStore.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Eoccured @: " + ex.StackTrace, Logger.LogLevel.INFO)
        End Try
    End Sub
    ''' <summary>
    ''' The Method handles scan the scan data returned form the barcode scanner.
    ''' This method implements the business logic to populate the data to the corresponding
    ''' UI element after validation.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="Type"></param>
    ''' <remarks></remarks>
    Public Sub HandleScanData(ByVal strBarcode As String, ByVal Type As BCType)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered HandleScanData of  GOTransferMgr", Logger.LogLevel.INFO)
        GOTransferMgr.GetInstance().m_frmGOScan.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        Try
            Select Case Type

                Case BCType.EAN
                    GOTransferMgr.GetInstance().ProcessScanItem(strBarcode, False)
                Case BCType.UPC
                    GOTransferMgr.GetInstance().ProcessScanItem(strBarcode, False)
                Case BCType.ManualEntry
                    'strBarcode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                    GOTransferMgr.GetInstance().ProcessScanItem(strBarcode, True)
                Case BCType.UOD
                    GOTransferMgr.GetInstance().ProcessScanUOD(strBarcode, False)
                Case BCType.UODManualEntry
                    GOTransferMgr.GetInstance().ProcessScanUOD(strBarcode, True)
            End Select
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in HandleScanData of  GOTransferMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
#If RF Then
            If (Not m_frmGOScan Is Nothing) AndAlso (Not m_frmGOScan.IsDisposed) Then
                GOTransferMgr.GetInstance().m_frmGOScan.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End If
#ElseIf NRF Then
            GOTransferMgr.GetInstance().m_frmGOScan.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#End If
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit HandleScanData of  GOTransferMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processes the scanned or handkeyed Product Code
    ''' </summary>
    ''' <param name="strBarcode">Scanned Item</param>
    ''' <param name="bIsManual">Manual or scanned</param>
    ''' <remarks></remarks>
    Private Sub ProcessScanItem(ByVal strBarcode As String, ByVal bIsManual As Boolean)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ProcessScanItem of  GOTransferMgr", Logger.LogLevel.INFO)
        Try
            'Check if the entry is manual or scanned
            If bIsManual Then
                'Check if the Boots code entered is valid or not
                If (objAppContainer.objHelper.ValidateBootsCode(strBarcode)) Then
                    'strBarcode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                    m_objGOItemInfo = New GOItemInfo()
                    'Get the product info from the Data Access Layer
                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingBC(strBarcode, m_objGOItemInfo)) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M6"), _
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                        MessageBoxDefaultButton.Button1)
                        Exit Sub
                    End If

                    'Fix for item not in file
                    If m_BCType = Nothing Then
                        If Not m_objGOItemInfo.BusinessCentreType = "" Then
                            m_BCType = m_objGOItemInfo.BusinessCentreType
                            'm_BCDesc = m_objGOItemInfo.ShortDescription
                            m_SupplierType = m_objGOItemInfo.SupplyRoute
                        End If
                    End If


                    'Data is available so now check if validation is required before moving ahead
                    If CBool(ConfigDataMgr.GetInstance.GetParam(ConfigKey.VALIDATEBC)) Then
                        'Check if the items entered belong to the same Business Centre type or not
                        If ValidateBusinessCentreType() Then
                            If CBool(ConfigDataMgr.GetInstance.GetParam(ConfigKey.VALIDATESUPPROUTE)) Then
                                If ValidateSupplyRoute() Then
                                    GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTRANSFER.ItemDetails)
                                Else
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M52"), _
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                         MessageBoxDefaultButton.Button1)
                                    GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTRANSFER.Scan)
                                End If
                            Else
                                GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTRANSFER.ItemDetails)
                            End If
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M12"), _
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                            MessageBoxDefaultButton.Button1)
                            GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTRANSFER.Scan)
                        End If
                    Else
                        GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTRANSFER.ItemDetails)
                    End If
                    'Check if the item has a valid EAN code
                ElseIf (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then

                    'Removing the last digit from the Barcode since its used only for check digit validation
                    strBarcode = strBarcode.PadLeft(13, "0")
                    strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                    m_objGOItemInfo = New GOItemInfo()
                    'Get the product info from the Data Access Layer
                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_objGOItemInfo)) Then
#If NRF Then
						'DARWIN checking if the Item code is a catch weight barcode or not
                        If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                            strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                            'DARWIN if Catch wt Barcode not in file check db/Controller using base barcode
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_objGOItemInfo)) Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M6"), _
                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                  MessageBoxDefaultButton.Button1)
                                Exit Sub
                            End If
                        Else
#End If
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M6"), _
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                        MessageBoxDefaultButton.Button1)
                        Exit Sub
#If NRF Then
                        End If
                    End If
                    'DARWIN if item code not in DB and is a Catch wt Barcde then chk db/cntrllr using base barcode
                    If (m_objGOItemInfo.Description = "Unknown Item") Then
                        'DARWIN checking if the Item code is a catch weight barcode or not
                        If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then

                            'DARWIN CHANGE converting Price Barcode to Base Barcode as Catch wt Barcode not
                            'in controller
                            strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                            'DARWIN if Catch wt Barcode not in file check db/Controller using base barcode
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_objGOItemInfo)) Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M6"), _
                             "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                              MessageBoxDefaultButton.Button1)
                                Exit Sub
                            End If
                        End If
#End If
                    End If

                    'Fix for item not in file
                    If m_BCType = Nothing Then
                        If Not m_objGOItemInfo.BusinessCentreType = "" Then
                            m_BCType = m_objGOItemInfo.BusinessCentreType
                            'm_BCDesc = m_objGOItemInfo.ShortDescription
                            m_SupplierType = m_objGOItemInfo.SupplyRoute
                        End If
                    End If

                    'Data is available so now check if validation is required before moving ahead
                    If CBool(ConfigDataMgr.GetInstance.GetParam(ConfigKey.VALIDATEBC)) Then
                        'Check if the items entered belong to the same Business Centre type or not
                        If ValidateBusinessCentreType() Then
                            If CBool(ConfigDataMgr.GetInstance.GetParam(ConfigKey.VALIDATESUPPROUTE)) Then
                                If ValidateSupplyRoute() Then
                                    GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTRANSFER.ItemDetails)
                                Else
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M52"), _
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                         MessageBoxDefaultButton.Button1)
                                    GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTRANSFER.Scan)
                                End If
                            Else
                                GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTRANSFER.ItemDetails)
                            End If
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M12"), _
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                            MessageBoxDefaultButton.Button1)
                            GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTRANSFER.Scan)
                        End If
                    Else
                        GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTRANSFER.ItemDetails)
                    End If
                Else
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M8"), _
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                        MessageBoxDefaultButton.Button1)
                    Exit Sub

                End If

            Else
                'Check if the Scanned item has a valid EAN code
                If Not (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M9"), _
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                    MessageBoxDefaultButton.Button1)
                Else
                    ''Removing the last digit from the Barcode since its used only for check digit validation
                    strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                    m_objGOItemInfo = New GOItemInfo()
                    'Get the product info from the Data Access Layer
                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_objGOItemInfo)) Then
                     
#If NRF Then   
					 If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                            strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                            'DARWIN if Catch wt Barcode not in file check db/Controller using base barcode
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_objGOItemInfo)) Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M6"), _
                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                  MessageBoxDefaultButton.Button1)
                                Exit Sub
                            End If
                        Else
#End If
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M6"), _
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                        MessageBoxDefaultButton.Button1)
                        Exit Sub
                        End If
#If NRF Then  						
                    End If
#End If

                    'Fix for item not in file
                    If m_BCType = Nothing Then
                        If Not m_objGOItemInfo.BusinessCentreType = "" Then
                            m_BCType = m_objGOItemInfo.BusinessCentreType
                            'm_BCDesc = m_objGOItemInfo.ShortDescription
                            m_SupplierType = m_objGOItemInfo.SupplyRoute
                        End If
                    End If

                    'Data is available so now check if validation is required before moving ahead
                    If CBool(ConfigDataMgr.GetInstance.GetParam(ConfigKey.VALIDATEBC)) Then
                        'Check if the items entered belong to the same Business Centre type or not
                        If ValidateBusinessCentreType() Then
                            If CBool(ConfigDataMgr.GetInstance.GetParam(ConfigKey.VALIDATESUPPROUTE)) Then
                                If ValidateSupplyRoute() Then
                                    GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTRANSFER.ItemDetails)
                                Else
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M52"), _
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                         MessageBoxDefaultButton.Button1)
                                    GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTRANSFER.Scan)
                                End If
                            Else
                                GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTRANSFER.ItemDetails)
                            End If
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M12"), _
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                            MessageBoxDefaultButton.Button1)
                            GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTRANSFER.Scan)
                        End If
                    Else
                        GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTRANSFER.ItemDetails)
                    End If
                End If
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessScanItem of  GOTransferMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit ProcessScanItem of  GOTransferMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Process the hand keyed or scanned UOD and store it in class Members
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="bIsManual"></param>
    ''' <remarks></remarks>
    Private Sub ProcessScanUOD(ByVal strBarcode As String, ByVal bIsManual As Boolean)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ProcessScanUOD of  GOTransferMgr", Logger.LogLevel.INFO)
        Dim bErrorMessagePrompted As Boolean = False
        Try
            If m_GOItemList.Count > 0 Then
                'Check if the UOD is valid or not
                If (objAppContainer.objHelper.ValidateUOD(strBarcode, bErrorMessagePrompted)) Then
                    'Set the UOD data to the class member
                    GOTransferMgr.GetInstance().SetUOD(strBarcode)
                    'Go to the next screen
                    GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTRANSFER.Despatch)
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
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessScanUOD of  GOTransferMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
#If RF Then
            'This code block clears the barcode text from the text box when there is a connection loss 
            'and connection regained during the retry attempts
            If ex.Message = Macros.CONNECTION_REGAINED Then
                If ((Not m_frmScanUOD Is Nothing) AndAlso (Not m_frmScanUOD.IsDisposed)) Then
                    m_frmScanUOD.txtBarcode.Text = ""
                End If
            End If
#End If
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit ProcessScanUOD of  GOTransferMgr", Logger.LogLevel.INFO)
    End Sub
#Region "End Session"
    ''' <summary>
    ''' Generates the Export data using Data Access Layer API
    ''' </summary> 
    ''' <remarks></remarks>
#If NRF Then
     Public Sub GenerateExportData()
#ElseIf RF Then
    Public Function GenerateExportData() As Boolean
#End If

#If NRF Then
 'Creating a variable of the type CreateUOS
        Dim objUOS As UOSRecord = Nothing
        Dim objUOA As UOARecord = Nothing
#End If

        Dim objUOX As UOXRecord = Nothing
        Dim totalPrice As Double = 0.0

        m_frmDespatch.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing UOS")
        'Handling Autologoff scenario for no data
        If m_UODNumber = Nothing Or m_GOItemList.Count < 1 Then
#If NRF Then
            Exit Sub
#ElseIf RF Then
            Exit Function
#End If
        End If
#If NRF Then
        objUOS.strIsListType = "G"
        objAppContainer.objExportDataManager.CreateUOS(objUOS)

        objAppContainer.objLogger.WriteAppLog("GoodsOut Transfer : UOS written successfully", Logger.LogLevel.RELEASE)
        'Variable to define a sequence number
        Dim iSequenceNo As Integer = 1

        For Each objItem As GOItemInfo In m_GOItemList
            m_frmDespatch.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing UOA: " + objItem.BootsCode)
            objUOA.strsequencenumber = CStr(iSequenceNo)
            objUOA.strbootscode = objItem.BootsCode
            objUOA.strquanity = objItem.Quantity
            objUOA.strsdescription = objItem.ShortDescription
            objUOA.strdescSEL = objItem.Description
            objUOA.strnumPrice = objItem.ItemPrice
            totalPrice = CDbl(objItem.ItemPrice) * CDbl(objItem.Quantity)
            objUOA.strnumTotalPrice = CStr(totalPrice)
            objUOA.stritembc = objItem.BusinessCentreType
            objUOA.strbcname = ConfigDataMgr.GetInstance().GetParam(objItem.BusinessCentreType)
            objUOA.strbarcode = objItem.ProductCode
            'TODO: this status will be x for cancelled items
            objUOA.strIsStatus = "A"
            iSequenceNo += 1
            'TODO : Get the data from the DAL and then add the supply route to this variable
            objAppContainer.objExportDataManager.CreateUOA(objUOA)
            objAppContainer.objLogger.WriteAppLog("GoodsOut Transfer : UOA written successfully", Logger.LogLevel.RELEASE)
        Next
#End If
        'Creating a varible of the type CreateUOX to be sent to DAL
        m_frmDespatch.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing UOX")
        objUOX.strisListType = "G"
        objUOX.strUOD = m_UODNumber
        'TODO : Status can be cancel
        objUOX.strIsStatus = "D"
        objUOX.strItemCount = GetUODItemCount().ToString
        objUOX.strIsStockFigure = "Y" 'Hardcoded to Y as per the RF Goods out Credit claim DD
        'TODO : Get the data from the DAL and then add the supply route to this variable
        objUOX.strSupplierRoute = m_SupplyRoute
        objUOX.strDisplayLoc = "" 'Empty as per PPC
        objUOX.strBCname = m_BusinessCentreType
        objUOX.strBCdesc = ConfigDataMgr.GetInstance().GetParam(m_BusinessCentreType)
        objUOX.strRecall = ""
        objUOX.strAuthCode = ""
        objUOX.strSupplier = ""
        objUOX.strMethod = WorkflowMgr.GetInstance().MethodOfReturn
        objUOX.strCarrier = WorkflowMgr.GetInstance().Carrier
        objUOX.strNumbird = "" 'Hardcoded as empty always
        objUOX.strNumReason = WorkflowMgr.GetInstance().ReasonCodeNum
        'TODO: While writing data for Store transfer put a check condition
        If m_Storeid <> "" Then
            objUOX.strRecStore = m_Storeid
        Else
            objUOX.strRecStore = ""
        End If
        objUOX.strDestination = WorkflowMgr.GetInstance().Destination
        'Depends on the supplier route if supplier route is C then warehouse is C else its R
        objUOX.strWroute = ""
        objUOX.strIsUODType = WorkflowMgr.GetInstance().UODType
        If objUOX.strIsUODType = "03" Or objUOX.strIsUODType = "3" Then
            objUOX.strReasonDamage = "02"
        Else
            objUOX.strReasonDamage = ""
        End If
        'TODO : Call UOX in DAL
#If NRF Then
        objAppContainer.objExportDataManager.CreateUOX(objUOX)
        'Clear the item list once the export data is written to the file.
        'm_GOItemList.Clear()
#ElseIf RF Then
        Try
            If Not objAppContainer.objExportDataManager.CreateUOX(objUOX) Then
                Return False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occured in Generate Export data - " + _
                                                               "GOTransfer Manager", Logger.LogLevel.RELEASE)
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
        Finally
            objUOX = Nothing
        End Try
#End If
        objAppContainer.objLogger.WriteAppLog("GoodsOut Transfer : UOX written successfully", Logger.LogLevel.RELEASE)
        objAppContainer.objUODCollection.Add(m_UODNumber)
#If NRF Then
        End Sub
#ElseIf RF Then
        Return True
    End Function
#End If
#End Region
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by GOTransferMgr.
    ''' </summary>
    ''' <returns>True if terminate is sucess else False</returns>
    ''' <remarks></remarks>
    Public Function EndSession(Optional ByVal bIsConnectFailed As Boolean = False)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered EndSession of  GOTransferMgr", Logger.LogLevel.INFO)
        Try
            'Save and data and perform all Exit Operations.
            'Close and Dispose all forms.

            m_frmDestinationStore.Close()
            m_frmDestinationStore.Dispose()
            TerminateGoodsOutManager(bIsConnectFailed)
            'Release all objects and Set to nothig.
#If RF Then
            If bIsConnectFailed Then
                objAppContainer.stSession.m_StoreID = m_Storeid
            End If
#End If
            'Closing the class object
            m_Storeid = Nothing
            m_objGOTransferMgr = Nothing
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in EndSession of  GOTransferMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try

        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit EndSession of  GOTransferMgr", Logger.LogLevel.INFO)
        Return True
    End Function

    ''' <summary>
    ''' Screen Display method for Goods Out. 
    ''' All Goods Out sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName">Enum SMSCREENS</param>
    ''' <returns>True if display is sucess else False</returns>
    ''' <remarks></remarks>
    Public Function DisplayGOTransferScreen(ByVal ScreenName As GOTRANSFER)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayGOTransferScreen of  GOTransferMgr", Logger.LogLevel.INFO)
        Try
            Select Case ScreenName
                Case GOTRANSFER.Scan
                    m_frmGOScan.Invoke(New EventHandler(AddressOf DisplayGOScan))
                Case GOTRANSFER.ItemDetails
                    m_frmGOItemDetails.Invoke(New EventHandler(AddressOf DisplayGOItemDetails))
                Case GOTRANSFER.ItemView
                    m_frmScanUOD.Invoke(New EventHandler(AddressOf DisplayScanUODScreen))
                Case GOTRANSFER.Summary
                    m_frmGOSummary.Invoke(New EventHandler(AddressOf DisplayGOSummary))
                Case GOTRANSFER.Despatch
                    m_frmDespatch.Invoke(New EventHandler(AddressOf DisplayGODespatch))
                Case GOTRANSFER.DestinationStoreId
                    m_frmDestinationStore.Invoke(New EventHandler(AddressOf DisplayDestinationStore))
            End Select
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayGOTransferScreen of  GOTransferMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayGOTransferScreen of  GOTransferMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Display the Authorization screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayDestinationStore()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayDestinationStore of  GOTransferMgr", Logger.LogLevel.INFO)
        Try
            With m_frmDestinationStore
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                .objNumeric.lblNumeric.Text = "Enter Destination Store Number"
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M22"), _
            "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
             MessageBoxDefaultButton.Button1)
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayDestinationStore of  GOTransferMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmDestinationStore.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayDestinationStore of  GOTransferMgr", Logger.LogLevel.INFO)
    End Sub

    Public Sub SetStoreID(ByVal sStoreid As String)
        m_Storeid = sStoreid
    End Sub
    Public Function GetDestinationStoreID() As String
        Return m_Storeid
    End Function
    Public Function ValidateStoreid(ByVal sAuthid As String) As Boolean
        If (sAuthid.Length = Macros.DESTSTOREMAXSIZE And sAuthid <> "" And sAuthid <> "0" And Val(sAuthid) > 0) Then
            Return True
        End If
        Return False
    End Function
    ''' <summary>
    ''' Enum Class that defines all screens for Shelf Monitor module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum GOTRANSFER
        Scan
        ItemDetails
        ItemView
        Summary
        DestinationStoreId
        Despatch
    End Enum
    Public Function ValidateNotOnFile() As Boolean
        'If m_BCType = Nothing Then
        '    MessageBox.Show(MessageManager.GetInstance.GetMessage("M68"), _
        '                   "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
        '                   MessageBoxDefaultButton.Button1)
        '    Return False
        'Else
        '    For Each objItemInfo As GOItemInfo In m_GOItemList
        '        If objItemInfo.BusinessCentreType = "" Then
        '            objItemInfo.BusinessCentreType = m_BCType
        '            'objItemInfo.ShortDescription = m_BCDesc
        '            objItemInfo.SupplyRoute = m_SupplierType
        '        End If
        '    Next
        'End If
        'Return True
        If m_BCType = Nothing Then
            m_BusinessCentreType = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DEFAULT_BUISNESSCENTRE_TYPE)
            m_SupplyRoute = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DEFAULT_SUPPLY_ROUTE)
            m_BCType = m_BusinessCentreType
            m_SupplierType = m_SupplyRoute
        End If

        For Each objItemInfo As GOItemInfo In m_GOItemList
            If objItemInfo.BusinessCentreType = "" Then
                'Pilot Support: If all items in the list are unknown items then add BC and SR for Baby.
                objItemInfo.BusinessCentreType = m_BusinessCentreType
                objItemInfo.SupplyRoute = m_SupplyRoute
                'end change
            End If
        Next

        Return True
    End Function

End Class
