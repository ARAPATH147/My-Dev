'''***************************************************************
''' <FileName>GOSessionMgr.vb</FileName>
''' <summary>
''' The Goods out feature class which will 
''' intialise all the parameters with respect to Goods Out.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
Public Class GOSessionMgr
    Inherits GoodsOutManager

    'Declaring the Form objects
    Private m_frmAuthorizationId As frmAuthorizationId
    Private m_frmRecallId As frmRecallid
    Private m_frmSuppliersList As frmSuppliersList

    'Declaring the objects that store items in a list
    Private Shared m_objGOSessionMgr As GOSessionMgr = Nothing
    Private m_SupplierList As ArrayList = Nothing

    'Stores the Transaction Data held within the session
    Private m_Authorizationid As String = Nothing
    Private m_Supplier As Supplier = Nothing
    Private m_Recallid As String = Nothing

    'Recalls CR
    'Create Recalls
    Public m_Message As MsgBx = Nothing

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
    Public Shared Function GetInstance() As GOSessionMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.GDSOUT
        If m_objGOSessionMgr Is Nothing Then
            m_objGOSessionMgr = New GOSessionMgr
        End If
        Return m_objGOSessionMgr
    End Function
    ReadOnly Property BusinessCentre() As String
        Get
            Return m_BCType
        End Get
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property SupplyRoute() As String
        Get
            Return m_SupplyRoute
        End Get
    End Property
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
            m_frmAuthorizationId = New frmAuthorizationId
            m_frmRecallId = New frmRecallid
            'Instantiating a class to store the GOItemInfo objects
            m_frmSuppliersList = New frmSuppliersList
            m_Message = New MsgBx
            Return True
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_REGAINED Or ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Then
                Throw ex
            End If
#End If
            Return False
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Function

    ''' <summary>
    ''' Updates the status bar message in all forms
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBar()
        Try
            m_frmAuthorizationId.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_frmRecallId.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_frmSuppliersList.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
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
        objAppContainer.objLogger.WriteAppLog("Entered HandleScanData of  GOSessionMgr", Logger.LogLevel.INFO)
        GOSessionMgr.GetInstance().m_frmGOScan.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
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
                    GOSessionMgr.GetInstance().ProcessScanItem(strBootsCode, True)
                Case BCType.EAN
                    GOSessionMgr.GetInstance().ProcessScanItem(strBarcode, False)
                Case BCType.ManualEntry
                    'strBarcode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                    GOSessionMgr.GetInstance().ProcessScanItem(strBarcode, True)
                Case BCType.UOD
                    GOSessionMgr.GetInstance().ProcessScanUOD(strBarcode, False)
                Case BCType.UODManualEntry
                    GOSessionMgr.GetInstance().ProcessScanUOD(strBarcode, True)
                Case BCType.UPC
                    GOSessionMgr.GetInstance().ProcessScanItem(strBarcode, False)
            End Select
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in HandleScanData of  GOSessionMgr. Exception is: " _
                                                            + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
#If RF Then
            If (Not m_frmGOScan Is Nothing) AndAlso (Not m_frmGOScan.IsDisposed) Then
                GOSessionMgr.GetInstance().m_frmGOScan.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End If
#ElseIf NRF Then
            GOSessionMgr.GetInstance().m_frmGOScan.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#End If

              End Try

        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit HandleScanData of  GOSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processes the scanned or handkeyed Product Code
    ''' </summary>
    ''' <param name="strBarcode">Scanned Item</param>
    ''' <param name="bIsManual">Manual or scanned</param>
    ''' <remarks></remarks>
    Private Sub ProcessScanItem(ByVal strBarcode As String, ByVal bIsManual As Boolean)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ProcessScanItem of  GOSessionMgr", Logger.LogLevel.INFO)
        Try
            'Check if the entry is manual or scanned
            If bIsManual Then
                'Check if the entered data is a valid Boots Code
                If (objAppContainer.objHelper.ValidateBootsCode(strBarcode)) Then
                    'strBarcode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                    objAppContainer.objLogger.WriteAppLog("GOSessionMgr::ProcessScanItem:BootsCode validated= " & strBarcode, Logger.LogLevel.RELEASE)
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
                            If CBool(ConfigDataMgr.GetInstance.GetParam(ConfigKey.VALIDATESUPPROUTE)) And _
                            m_GOItemList.Count > 0 Then
                                If m_SupplyRoute = "D" Then
                                    If ValidateSupplyRoute() Then
                                        GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                                    Else
                                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M52"), _
                                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                             MessageBoxDefaultButton.Button1)
                                        GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.Scan)
                                    End If
                                ElseIf m_objGOItemInfo.SupplyRoute <> "D" Then
                                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                                Else
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M52"), _
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                         MessageBoxDefaultButton.Button1)
                                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.Scan)
                                End If
                            Else
                                'Recalls CR
                                'Create recalls
                                If objAppContainer.bCreateRecall Then
                                    'Tailoring
                                    'To check tailoring flag too
#If RF Then
                                    If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_objGOItemInfo.BusinessCentreType = "D" Then
#ElseIf NRF Then
                            If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_objGOItemInfo.BusinessCentreType = "D" Or m_objGOItemInfo.IsTillBarSet Then
#End If
                                        GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                                    Else
                                        'beep sound
                                        RLSessionMgr.GetInstance().Beep()
                                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M73"), _
                                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                                MessageBoxDefaultButton.Button1)
                                    End If
                                Else
                                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                                End If
                                'GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                            End If
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M12"), _
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                         MessageBoxDefaultButton.Button1)
                            GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.Scan)
                        End If
                    Else
                        'Recalls CR Create recalls
                        If objAppContainer.bCreateRecall Then
                            'Tailoring
                            'To check tailoring flag too
#If RF Then
                            If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_objGOItemInfo.BusinessCentreType = "D" Then
#ElseIf NRF Then
                            If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_objGOItemInfo.BusinessCentreType = "D" Or m_objGOItemInfo.IsTillBarSet Then
#End If
                                'If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_BCType = "D" Then
                                GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                            Else
                                'beep sound
                                RLSessionMgr.GetInstance().Beep()
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M73"), _
                                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                                MessageBoxDefaultButton.Button1)
                            End If
                        Else
                            GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                        End If
                        'GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                    End If
                    'Check if the entered data is a valid EAN/Product Code
                ElseIf (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                    objAppContainer.objLogger.WriteAppLog("GOSessionMgr::ProcessScanItem:Barcode validated= " & strBarcode, Logger.LogLevel.RELEASE)
                    'Removing the last digit from the Barcode since its used only for check digit validation
                    strBarcode = strBarcode.PadLeft(13, "0")
                    strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                    m_objGOItemInfo = New GOItemInfo()
                    'Get the product info from the Data Access Layer
                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_objGOItemInfo)) Then
#If NRF Then                       
					   ' DARWIN CHANGE converting Price Barcode to Base Barcode
                        If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                            'Get the product info from the Data Access Layer
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
                            strBarcode = objAppContainer.objHelper.GetBaseBarcode(strBarcode)
                            'DARWIN if Catch wt Barcode not in file check db/Controller using base barcode
                            'Get the product info from the Data Access Layer
                            If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_objGOItemInfo)) Then
                                ' DARWIN CHANGE converting Price Barcode to Base Barcode
                                If strBarcode.StartsWith("2") Or strBarcode.StartsWith("02") Then
                                    'Get the product info from the Data Access Layer
                                    If Not (objAppContainer.objDataEngine.GetProductInfoUsingPC(strBarcode, m_objGOItemInfo)) Then
                                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M6"), _
                                                           "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                                            MessageBoxDefaultButton.Button1)
                                        Exit Sub
                                    End If
                                Else
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M6"), _
                                                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                                             MessageBoxDefaultButton.Button1)
                                    Exit Sub
                                End If

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
                                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                                Else
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M52"), _
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                         MessageBoxDefaultButton.Button1)
                                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.Scan)
                                End If
                            Else
                                'Recalls CR
                                'Create recalls
                                If objAppContainer.bCreateRecall Then
                                    'Tailoring
                                    'To check tailoring flag too
#If RF Then
                                    If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_objGOItemInfo.BusinessCentreType = "D" Then
#ElseIf NRF Then
                                    If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_objGOItemInfo.BusinessCentreType = "D" Or m_objGOItemInfo.IsTillBarSet Then
#End If
                                        GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                                    Else
                                        'beep sound
                                        RLSessionMgr.GetInstance().Beep()
                                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M73"), _
                                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                                MessageBoxDefaultButton.Button1)
                                    End If
                                Else
                                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                                End If

                                'GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                            End If
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M12"), _
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                         MessageBoxDefaultButton.Button1)
                            GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.Scan)
                        End If
                    Else
                        'Recalls CR
                        'Create recalls
                        If objAppContainer.bCreateRecall Then
                            'Tailoring
                            'To check tailoring flag too
#If RF Then
                            If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_objGOItemInfo.BusinessCentreType = "D" Then
#ElseIf NRF Then
                            If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_objGOItemInfo.BusinessCentreType = "D" Or m_objGOItemInfo.IsTillBarSet Then
#End If
                                'If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_BCType = "D" Or m_objGOItemInfo.Tailored Then
                                GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                            Else
                                'beep sound
                                RLSessionMgr.GetInstance().Beep()
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M73"), _
                                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                                 MessageBoxDefaultButton.Button1)
                            End If
                        Else
                            GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                        End If
                        'GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                    End If
                Else
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M9"), _
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
                    objAppContainer.objLogger.WriteAppLog("GOSessionMgr::ProcessScanItem:Barcode validated= " & strBarcode, Logger.LogLevel.RELEASE)
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
                                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                                Else
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M52"), _
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                         MessageBoxDefaultButton.Button1)
                                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.Scan)
                                End If
                            Else
                                'GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                                'Recalls CR
                                'Create recalls
                                If objAppContainer.bCreateRecall Then
#If RF Then
                                    If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_objGOItemInfo.BusinessCentreType = "D" Then
#ElseIf NRF Then
                                    If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_objGOItemInfo.BusinessCentreType = "D" Or m_objGOItemInfo.IsTillBarSet Then
#End If
                                        'If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_BCType = "D" Then
                                        GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                                    Else
                                        'beep sound
                                        RLSessionMgr.GetInstance().Beep()
                                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M73"), _
                                                 "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                                  MessageBoxDefaultButton.Button1)
                                    End If
                                Else
                                    'Recalls CR
                                    'Create recalls
                                    If objAppContainer.bIsCreateRecalls Then
                                        'Tailoring
                                        'To check tailoring flag too
#If RF Then
                                        If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_objGOItemInfo.BusinessCentreType = "D" Then
#ElseIf NRF Then
                                        If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_objGOItemInfo.BusinessCentreType = "D" Or m_objGOItemInfo.IsTillBarSet Then
#End If
                                            'If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_BCType = "D" Then
                                            GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                                        Else
                                            'beep sound
                                            RLSessionMgr.GetInstance().Beep()
                                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M73"), _
                                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                                 MessageBoxDefaultButton.Button1)
                                        End If
                                    Else
                                        GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                                    End If
                                    'GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                                End If
                                ''''''''''''''end create recalls


                            End If


                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M12"), _
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                         MessageBoxDefaultButton.Button1)
                            GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.Scan)
                        End If
                    Else
                        'Recalls CR
                        'Create recalls
                        If objAppContainer.bCreateRecall Then
                            'Tailoring
                            'To check tailoring flag too
#If RF Then
                            If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_objGOItemInfo.BusinessCentreType = "D" Then
#ElseIf NRF Then
                            If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_objGOItemInfo.BusinessCentreType = "D" Or m_objGOItemInfo.IsTillBarSet Then
#End If
                                'If m_objGOItemInfo.strRecallType = "E" Or m_objGOItemInfo.strRecallType = "W" Or m_BCType = "D" Then
                                GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                            Else
                                'beep sound
                                RLSessionMgr.GetInstance().Beep()
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M73"), _
                                                 "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                                  MessageBoxDefaultButton.Button1)
                            End If
                        Else
                            GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                        End If
                        'GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.ItemDetails)
                    End If
                End If
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or _
            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessScanItem of  GOSessionMgr. Exception is: " _
                                                  + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit ProcessScanItem of  GOSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Process the hand keyed or scanned UOD and store it in class Members
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="bIsManual"></param>
    ''' <remarks></remarks>
    Private Sub ProcessScanUOD(ByVal strBarcode As String, ByVal bIsManual As Boolean)
        Dim bErrorMessagePrompted As Boolean = False
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ProcessScanUOD of  GOSessionMgr", Logger.LogLevel.INFO)
        Try
            If m_GOItemList.Count > 0 Then
                'Check if the UOD is valid or not
                If (objAppContainer.objHelper.ValidateUOD(strBarcode, bErrorMessagePrompted)) Then
                    'Set the UOD data to the class member
                    GOSessionMgr.GetInstance().SetUOD(strBarcode)
                    'Go to the next screen
                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSCREENS.GODespatch)
                    objAppContainer.objLogger.WriteAppLog("GOSessionMgr::ProcessScanUOD:UOD Scanned =" & strBarcode, Logger.LogLevel.RELEASE)
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
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessScanUOD of  GOSessionMgr. Exception is: " _
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
        objAppContainer.objLogger.WriteAppLog("Exit ProcessScanUOD of  GOSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by GOSessionMgr.
    ''' </summary>
    ''' <returns>True if terminate is sucess else False</returns>
    ''' <remarks></remarks>
    Public Function EndSession(Optional ByVal bIsConnectFailed As Boolean = False)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered EndSession of  GOSessionMgr", Logger.LogLevel.INFO)
        Try
#If NRF Then
            m_GOItemList.Clear()
#End If

            'Save and data and perform all Exit Operations.
            'Close and Dispose all forms.
            'm_frmSuppliersList.Close()
            m_frmSuppliersList.Dispose()
            'm_frmAuthorizationId.Close()
            m_frmAuthorizationId.Dispose()
            'm_frmRecallId.Close()
            m_frmRecallId.Dispose()
            'If objAppContainer.bCreateRecall = True Then
            '    objAppContainer.bCreateRecall = False
            'End If
            TerminateGoodsOutManager(bIsConnectFailed)
            'In case of connection failure set the session variable to temp variable
#If RF Then
            If bIsConnectFailed Then
                objAppContainer.stSession.m_Authorizationid = m_Authorizationid
                objAppContainer.stSession.m_Supplier = m_Supplier
                objAppContainer.stSession.m_BCType = m_BCType
                objAppContainer.stSession.m_SupplierType = m_SupplierType
            End If
#End If
            'Release all objects and Set to nothig.
            'Closing the class object
            m_SupplierList = Nothing
            m_Authorizationid = Nothing
            m_Supplier = Nothing
            m_objGOSessionMgr = Nothing
            'Fix for item not on file
            m_BCType = Nothing
            m_SupplierType = Nothing
            'Recalls CR
            'Create Recalls
            m_Message.Dispose()
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in EndSession of  GOSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit EndSession of  GOSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function

    ''' <summary>
    ''' Screen Display method for Goods Out. 
    ''' All Goods Out sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName">Enum SMSCREENS</param>
    ''' <returns>True if display is sucess else False</returns>
    ''' <remarks></remarks>
    Public Function DisplayGOScreen(ByVal ScreenName As GOSCREENS)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayGOScreen of  GOSessionMgr", Logger.LogLevel.INFO)

        Try
            Select Case ScreenName
                Case GOSCREENS.Scan
                    m_frmGOScan.Invoke(New EventHandler(AddressOf DisplayGOScan))
                Case GOSCREENS.ItemDetails
                    m_frmGOItemDetails.Invoke(New EventHandler(AddressOf DisplayGOItemDetails))
                Case GOSCREENS.ItemView
                    m_frmScanUOD.Invoke(New EventHandler(AddressOf DisplayScanUODScreen))
                Case GOSCREENS.GOSummary
                    m_frmGOSummary.Invoke(New EventHandler(AddressOf DisplayGOSummary))
                Case GOSCREENS.Authorizationid
                    m_frmAuthorizationId.Invoke(New EventHandler(AddressOf DisplayGOAuthorizationid))
                Case GOSCREENS.Recallid
                    m_frmRecallId.Invoke(New EventHandler(AddressOf DisplayRecallid))
                Case GOSCREENS.GODespatch
                    m_frmDespatch.Invoke(New EventHandler(AddressOf DisplayGODespatch))
                Case GOSCREENS.SupplierList
                    m_frmSuppliersList.Invoke(New EventHandler(AddressOf DisplaySupplierList))
                    'Recalls CR
                    'Create Recalls
                Case GOSCREENS.Message
                    m_Message.Invoke(New EventHandler(AddressOf DisplayMessage))
            End Select
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayGOScreen of  GOSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try

        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayGOScreen of  GOSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Display the Authorization screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayGOAuthorizationid()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayGOAuthorizationid of  GOSessionMgr", Logger.LogLevel.INFO)
        Try
            With m_frmAuthorizationId
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                .objNumeric.lblNumeric.Text = "Enter Authorization Number :"
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M15"), _
            "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
             MessageBoxDefaultButton.Button1)
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayGOAuthorizationid of  GOSessionMgr. Exception is: " _
                                                             + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmAuthorizationId.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayGOAuthorizationid of  GOSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Display the Recall screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayRecallid()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayRecallid of  GOSessionMgr", Logger.LogLevel.INFO)
        Try
            With m_frmRecallId
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                .objNumeric.lblNumeric.Text = "Enter Recall Number"
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M16"), _
           "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
            MessageBoxDefaultButton.Button1)
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayRecallid of  GOSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmRecallId.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayRecallid of  GOSessionMgr", Logger.LogLevel.INFO)

    End Sub
    'Recalls Cr
    'Create Recalls
    Private Sub DisplayMessage(ByVal o As Object, ByVal e As EventArgs)
        With m_Message
            m_Message.lblMsg.Text = MessageManager.GetInstance().GetMessage("M74").ToString()
            m_Message.Visible = True
            Application.DoEvents()
            .Invalidate()
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' Displays supplier list
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplaySupplierList()
        'Write to Log INFO File while entry
        m_frmSuppliersList.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Fetching the Supplier List")
        objAppContainer.objLogger.WriteAppLog("Entered DisplaySupplierList of  GOSessionMgr", Logger.LogLevel.INFO)
        Try
            Me.GetSuppliersList()
            With m_frmSuppliersList
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                .lblBusCentreDesc.Text = objAppContainer.objHelper.FormatEscapeSequence _
                                        (ConfigDataMgr.GetInstance().GetParam(m_BusinessCentreType))
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M19"), _
           "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
            MessageBoxDefaultButton.Button1)
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplaySupplierList of  GOSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmSuppliersList.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplaySupplierList of  GOSessionMgr", Logger.LogLevel.INFO)
    End Sub
    Public Sub GetSuppliersList()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered GetSuppliersList of  GOSessionMgr", Logger.LogLevel.INFO)
        Try
            'Load the supplier list only if its not prepopulated
            m_SupplierList = New ArrayList
            'If objAppContainer.objDataEngine.GetSupplierList(m_SupplierList) Then
            If objAppContainer.objDataEngine.GetSupplierList(m_SupplierList, m_BusinessCentreType) Then
                If m_SupplierList.Count > m_frmSuppliersList.lvSuppliersList.Items.Count Then
                    Dim arrindex = m_frmSuppliersList.lvSuppliersList.Items.Count
                    For idx As Integer = arrindex To m_SupplierList.Count - 1
                        Dim obj As SupplierList = m_SupplierList.Item(idx)
                        Dim objListItem As ListViewItem
                        objListItem = m_frmSuppliersList.lvSuppliersList.Items.Add(New ListViewItem(obj.SupplierID))
                        objListItem.SubItems.Add(obj.SupplierName)
                    Next
                ElseIf m_SupplierList.Count < m_frmSuppliersList.lvSuppliersList.Items.Count Then
                    m_frmSuppliersList.lvSuppliersList.Items.Clear()
                    For Each m_Item As SupplierList In m_SupplierList
                        Dim objListItem As ListViewItem
                        objListItem = m_frmSuppliersList.lvSuppliersList.Items.Add(New ListViewItem(m_Item.SupplierID))
                        objListItem.SubItems.Add(m_Item.SupplierName)
                    Next
                End If
            Else
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M18"), _
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                MessageBoxDefaultButton.Button1)
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in GetSuppliersList of  GOSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M21"), _
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                MessageBoxDefaultButton.Button1)
        Finally
            m_SupplierList = Nothing
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit GetSuppliersList of  GOSessionMgr", Logger.LogLevel.INFO)
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
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered GenerateExportData of  GOSessionMgr", Logger.LogLevel.INFO)
        Try
#If NRF Then
  'Creating a variable of the type CreateUOS
            Dim objUOS As UOSRecord = Nothing
            Dim objUOA As UOARecord = Nothing
#End If

            Dim objUOX As UOXRecord = Nothing
            Dim objSTQ As STQRecord = Nothing
            Dim totalPrice As Double = 0.0

            'Handling Autologoff scenario for no data
            If m_UODNumber = Nothing Or m_GOItemList.Count < 1 Then
#If NRF Then
                Exit Sub
#ElseIf RF Then
                Exit Function
#End If
            End If
            m_frmDespatch.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing UOS")
#If NRF Then
            objUOS.strIsListType = "G"
            'TODO : Call UOS in DAL
            objAppContainer.objExportDataManager.CreateUOS(objUOS)
            objAppContainer.objLogger.WriteAppLog("Goods Out : UOS written successfully", Logger.LogLevel.RELEASE)
'#ElseIf RF Then
'            If Not objAppContainer.objExportDataManager.CreateUOS(objUOS) Then
'                Return False
'            End If
'            objAppContainer.objLogger.WriteAppLog("Goods Out : UOS written successfully", Logger.LogLevel.RELEASE)
'#End If

            'objAppContainer.objExportDataManager.CreateUOS(objUOS)

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
'#If NRF Then
                objAppContainer.objExportDataManager.CreateUOA(objUOA)
'#ElseIf RF Then
'                If Not objAppContainer.objExportDataManager.CreateUOA(objUOA) Then
'                    Return False
'                End If
                objAppContainer.objLogger.WriteAppLog("Goods Out : UOA written successfully", Logger.LogLevel.RELEASE)
            Next
#End If
            'Sending a STQ request to the Controller only in case of Stock Take
            If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.STOCKTAKE Then
                m_frmDespatch.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Sending STQ")
                objSTQ.strUODNumber = Macros.STQ_UOD_NUMBER
#If NRF Then
                objAppContainer.objExportDataManager.CreateSTQ(objSTQ)
#ElseIf RF Then
                Dim strUODNumber As String = ""
                If Not objAppContainer.objExportDataManager.CreateSTQ(objSTQ, strUODNumber) OrElse _
                                            strUODNumber.Trim() = "" Then
                    Return False
                End If
                'In case of NRF setting the UOD number to this value
                SetUOD(strUODNumber)
#End If
                objAppContainer.objLogger.WriteAppLog("Goods Out : STQ written successfully", Logger.LogLevel.RELEASE)
            End If
            m_frmDespatch.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing UOX")
            'Creating a varible of the type CreateUOX to be sent to DAL
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
            If m_Recallid <> "" Then
                objUOX.strRecall = m_Recallid
            Else
                objUOX.strRecall = ""
            End If
            If m_Authorizationid <> "" Then
                objUOX.strAuthCode = m_Authorizationid
            Else
                objUOX.strAuthCode = ""
            End If
            If m_Supplier.SupplierNo <> Nothing Then
                objUOX.strSupplier = m_Supplier.SupplierNo
            Else
                objUOX.strSupplier = ""
            End If
            objUOX.strMethod = WorkflowMgr.GetInstance().MethodOfReturn
            objUOX.strCarrier = WorkflowMgr.GetInstance().Carrier
            objUOX.strNumbird = "" 'Hardcoded as empty always
            objUOX.strNumReason = WorkflowMgr.GetInstance().ReasonCodeNum
            'TODO: While writing data for Store transfer put a check condition
            objUOX.strRecStore = ""
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
            'm_GOItemList.Clear() - The alternative change is done in displaysummaryscreen()
#ElseIf RF Then
            Try
                If Not objAppContainer.objExportDataManager.CreateUOX(objUOX) Then
                    Return False
                End If
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog(ex.Message + " Occured in Generate Export data - " + _
                                                      "GOSession Manager", Logger.LogLevel.RELEASE)
                If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                    Throw ex
                End If
            Finally
                objUOX = Nothing
            End Try

#End If
            objAppContainer.objLogger.WriteAppLog("Goods Out : UOX written successfully", Logger.LogLevel.RELEASE)
            'Update the UOD collection
            objAppContainer.objUODCollection.Add(m_UODNumber)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured in GenerateExportdata of  GOSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit GenerateExportdata of  GOSessionMgr", Logger.LogLevel.INFO)
#If NRF Then
        End sub
#ElseIf RF Then
        Return True
    End Function
#End If
#End Region

    Public Sub SetAuthorizationID(ByVal sAuthid As String)
        objAppContainer.objLogger.WriteAppLog("Auth id set to " & sAuthid, Logger.LogLevel.RELEASE)
        m_Authorizationid = sAuthid
    End Sub
    Public Function GetAuthorizationID() As String
        Return m_Authorizationid
    End Function
    Public Sub SetRecallid(ByVal sRecallid As String)
        objAppContainer.objLogger.WriteAppLog("Recall id set to " & sRecallid, Logger.LogLevel.RELEASE)
        m_Recallid = sRecallid
    End Sub
    Public Function GetRecallid() As String
        Return m_Recallid
    End Function
    Public Sub SetSupplierName()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered SetSupplierName of  GOSessionMgr", Logger.LogLevel.INFO)
        Try
            Dim indexes As ListView.SelectedIndexCollection = m_frmSuppliersList.lvSuppliersList.SelectedIndices
            Dim index As Integer
            For Each index In indexes
                m_Supplier.SupplierNo = m_frmSuppliersList.lvSuppliersList.Items(index).Text
                m_Supplier.SupplierName = m_frmSuppliersList.lvSuppliersList.Items(index).SubItems(1).Text
                objAppContainer.objLogger.WriteAppLog("Supplier Name set successfully to " & m_Supplier.SupplierNo, Logger.LogLevel.RELEASE)
                Exit For
            Next
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in SetSupplierName of  GOSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit SetSupplierName of  GOSessionMgr", Logger.LogLevel.INFO)
    End Sub

    ''' <summary>
    ''' Enum Class that defines all screens for Shelf Monitor module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum GOSCREENS
        Scan
        ItemDetails
        ItemView
        GOSummary
        Authorizationid
        GODespatch
        SupplierList
        Recallid
        'Recalls CR
        'Create Recalls
        Message
    End Enum
    Public Function ValidateNotOnFile() As Boolean
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


''' <summary>
''' The value class for getting and managing Goods Out Items.
''' </summary>
''' <remarks></remarks>
Public Class GOItemInfo
    Inherits ProductInfo
    Private m_FirstBarcode As String
    Private m_SecondBarcode As String
    Private m_sBusinessCenterType As String
    Private m_sQuantity As String
    'Recall CR
    Private m_strRecallType As String
    'Recalls CR
    'Create Recalls
    Private m_strItemStatus_0 As String
    Private m_strItemStatus_8 As String
    Private m_Tailored As Boolean
    Private m_IsTillBarSet As Boolean
    Private m_StockMoveDate As String
    Private m_SeqNo As String
    Public Property StockMoveDate() As String
        Get
            Return m_StockMoveDate
        End Get
        Set(ByVal value As String)
            m_StockMoveDate = value
        End Set
    End Property
    Public Property Tailored() As Boolean
        Get
            Return m_Tailored
        End Get
        Set(ByVal value As Boolean)
            m_Tailored = value
        End Set
    End Property
    Public Property IsTillBarSet() As Boolean
        Get
            Return m_IsTillBarSet
        End Get
        Set(ByVal value As Boolean)
            m_IsTillBarSet = value
        End Set
    End Property
    Public Property strItemStatus_0() As String
        Get
            Return m_strItemStatus_0
        End Get
        Set(ByVal value As String)
            m_strItemStatus_0 = value
        End Set
    End Property
    Public Property strItemStatus_8() As String
        Get
            Return m_strItemStatus_8
        End Get
        Set(ByVal value As String)
            m_strItemStatus_8 = value
        End Set
    End Property
    Public Property strRecallType() As String
        Get
            Return m_strRecallType
        End Get
        Set(ByVal value As String)
            m_strRecallType = value
        End Set
    End Property
    Public Property FirstBarcode() As String
        Get
            Return m_FirstBarcode
        End Get
        Set(ByVal value As String)
            m_FirstBarcode = value
        End Set
    End Property
    Public Property SecondBarcode() As String
        Get
            Return m_SecondBarcode
        End Get
        Set(ByVal value As String)
            m_SecondBarcode = value
        End Set
    End Property

    Public Property BusinessCentreType() As String
        Get
            Return m_sBusinessCenterType
        End Get
        Set(ByVal value As String)
            m_sBusinessCenterType = value
        End Set
    End Property
    Public Property Quantity() As String
        Get
            Return m_sQuantity
        End Get
        Set(ByVal value As String)
            m_sQuantity = value
        End Set
    End Property
    Public Property SequenceNumber() As String
        Get
            Return m_SeqNo
        End Get
        Set(ByVal value As String)
            m_SeqNo = value
        End Set
    End Property

End Class