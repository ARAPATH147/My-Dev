Imports MCGdOut.FileIO
'''****************************************************************************
''' <FileName>UserSessionManager.vb</FileName>
''' <summary>
''' Create the TRANSACT records before writing to the export data file.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>08-Dec-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''****************************************************************************
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Connects to alternate controller while primary is down
''' </Summary>
'''****************************************************************************
Public Class GOExportDataManager

    ' Declaration of private member variable for reading export data file name
#If NRF Then
    Private strExFileName As String = Nothing
    Private strExFilePath As String = Nothing
    Private strExportFile As String = Nothing

#End If
#If RF Then
    Private strListID As String = Nothing
    Private strBusinessCentre As String = Nothing
    Private m_PreviousMessage As String = Nothing
    Private m_SeqNumber As Integer = 0
    'In NRF Scenario the socket is needed ony wen download happens 
    'So a Local Socket manager is declared and initialised in Download Export Data() function
    Private m_TransactDataTransmitter As ExDataTransmitter = Nothing
    Private m_ConnectionLostExit As Boolean = False
    Private m_TimeOutRetrySuccesStrtRcrd As Boolean = False
#End If

    Public Sub EndSession()
#If RF Then
        If Not m_TransactDataTransmitter Is Nothing Then
            m_TransactDataTransmitter.EndSession()
            m_TransactDataTransmitter = Nothing
        End If
#ElseIf NRF Then
        'Nothing to dispose in NRF
        'MAinly because ExData Transmitter is not used 
#End If
    End Sub
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
#If NRF Then
        'Read the Export Data File path
        strExFilePath = ConfigDataMgr.GetInstance.GetParam(ConfigKey.EXPORT_FILE_PATH)

        ' Read the Export Data File name for actual export data
        strExFileName = Macros.EXPORT_FILE_NAME

        'Preapare the full export data path appended with file name
        strExportFile = strExFilePath + strExFileName
#ElseIf RF Then
        m_TransactDataTransmitter = New ExDataTransmitter()
#End If
    End Sub
#If RF Then
#Region "Connection Lost Scenario"
    Public Enum ConnectionLostScenario
        While_Sending_End_Record
        While_Sending_Start_Record
        While_Sending_ENQ
        While_Sending_Void
        While_Sending_Item_Add
        While_Scanning_UOD_Label
        While_Retrieving_the_List
        While_Log_OFF
        While_Others
        While_Login
        While_Retreiving_Act_Recall_List
    End Enum
#End Region
#Region "CreateENQ"
    Public Function CreateENQ(ByVal strBarcode As String) As Boolean
        Dim bTemp As Boolean
        Dim strData As New System.Text.StringBuilder
        'Record Name
        strData.Append("ENQ")
        strData.Append(objAppContainer.strCurrentUserID)
        'I- for Item and p for parent
        strData.Append("I")
        'Module is not PriceCheck So append Zero
        strData.Append(" ")

        'The barcode or Boots code of the item
        strData.Append(strBarcode.PadLeft(13, "0"))
        'Stock Figure Needed?
        strData.Append("Y")
        'Ossr No Change
        strData.Append(" ")
        'Planner Look up
        strData.Append("N")
        DATAPOOL.getInstance.ResetPoolData()
        If m_TransactDataTransmitter.SendRecordRF(strData.ToString().Replace(",", "")) Then
            bTemp = ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification())
        Else
            ' If Not DATAPOOL.getInstance.isConnected Then
            If m_SeqNumber > 0 Then
                Reconnect(ConnectionLostScenario.While_Sending_Item_Add)
            Else
                Reconnect()
            End If
            'End If
            bTemp = False
        End If
        Return bTemp
    End Function
#End Region
#Region "ProcessDataRecieved"
    Private Function ProcessDataRecieved(ByRef m_Status As DATAPOOL.ConnectionStatus, Optional ByVal ConnectionLostScenario As ConnectionLostScenario = GOExportDataManager.ConnectionLostScenario.While_Others) As Boolean
        Select Case m_Status
            Case DATAPOOL.ConnectionStatus.MessageRecieved
                Return True
            Case DATAPOOL.ConnectionStatus.Disconnected
                Reconnect(ConnectionLostScenario)
                Return False
            Case DATAPOOL.ConnectionStatus.TimeOut
                'objAppContainer.objExportDataManager.m_TransactDataTransmitter.HandleTimeOut(ConnectionLostScenario)
                Dim strException As String = ""
                'If ConnectionLostScenario <> ConnectionLostScenario.SENDING_END_MESSAGE_AFTER_RECONNECT Then
                'v1.1 MCF Change
                If objAppContainer.bMCFEnabled Then
                    If objAppContainer.objExportDataManager.m_TransactDataTransmitter.fConnectAlternateInRF() Then
                        Reconnect(ConnectionLostScenario, False)
                    Else
                        objAppContainer.objExportDataManager.m_TransactDataTransmitter.sCloseSession()
                    End If
                Else
                    While objAppContainer.objExportDataManager.m_TransactDataTransmitter.HandleTimeOut(ConnectionLostScenario, strException)
                        Try
                            'Invoke connection retry to connect and pass timeout parameter as true.
                            Reconnect(ConnectionLostScenario, True)
                            If m_TimeOutRetrySuccesStrtRcrd Then
                                Throw (New Exception(Macros.CONNECTION_REGAINED))
                            End If
                            m_TimeOutRetrySuccesStrtRcrd = False
                        Catch ex As Exception
                            strException = ex.Message
                            If ex.Message.Equals(Macros.CONNECTION_REGAINED) Then
                                Exit While
                            End If
                        End Try
                    End While
                    Throw New Exception(strException)
                End If
                Return False
        End Select
    End Function
#End Region
#Region "Reconnect"
    Private Sub Reconnect(Optional ByVal ConnectionLostScenario As ConnectionLostScenario = GOExportDataManager.ConnectionLostScenario.While_Others, Optional ByVal bIsReconnectingAfterDataTimeout As Boolean = False)
        objAppContainer.objLogger.WriteAppLog("Reconnecting ", Logger.LogLevel.RELEASE)
        Dim bCncnLostExitPrSsn As Boolean = m_ConnectionLostExit
        m_ConnectionLostExit = True
        objAppContainer.objLogger.WriteAppLog("Setting status Bar message as disconnected ", Logger.LogLevel.RELEASE)
        objAppContainer.isConnected = False
        UpdateStatusBarMessage(ConnectionLostScenario)
        If objAppContainer.objExportDataManager.m_TransactDataTransmitter.EstablishConnection(ConnectionLostScenario, bIsReconnectingAfterDataTimeout) Then
            'In case of SOR no need to send an RCN
            If ConnectionLostScenario <> GOExportDataManager.ConnectionLostScenario.While_Login Then
                If CreateRCN(strListID) Then
                    'v1.1 MCF Change
                    If objAppContainer.iConnectedToAlternate = 1 Then
                        ConfigDataMgr.GetInstance.SetActiveIP()
                    End If
                    'Getting the stack trace
                    objAppContainer.objLogger.WriteAppLog("Setting status Bar message as Connected ", Logger.LogLevel.RELEASE)
                    objAppContainer.isConnected = True
                    UpdateStatusBarMessage(ConnectionLostScenario)
                    m_ConnectionLostExit = False
                    If ((ConnectionLostScenario = GOExportDataManager.ConnectionLostScenario.While_Sending_Start_Record) Or _
                    (ConnectionLostScenario = GOExportDataManager.ConnectionLostScenario.While_Log_OFF)) Then
                       
                        If bCncnLostExitPrSsn Then
                           
                            Cursor.Current = Cursors.WaitCursor
                            m_TimeOutRetrySuccesStrtRcrd = True
                            Return
                        Else
                            
                            m_TimeOutRetrySuccesStrtRcrd = False
                            'If RCN is sent and the message is not start message
                            Throw (New Exception(Macros.CONNECTION_REGAINED))
                        End If
                    Else
                        If objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.RECALL And _
                        ConnectionLostScenario = GOExportDataManager.ConnectionLostScenario.While_Retreiving_Act_Recall_List Then
                            RLSessionMgr.GetInstance().EndSession()
                            ' WorkflowMgr.GetInstance().ExecPrev()
                        ElseIf objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.RECALL And _
                        (ConnectionLostScenario = GOExportDataManager.ConnectionLostScenario.While_Sending_Item_Add Or _
                         ConnectionLostScenario = GOExportDataManager.ConnectionLostScenario.While_Others) Then
                            'DEFECT FIX for 4907
                            'sending RCA
                            CreateRCA()
                        End If
                        objAppContainer.objLogger.WriteAppLog("Connection Regained Exception Thrown", Logger.LogLevel.RELEASE)
                        'If RCN is sent and the message is not start message
                        Throw (New Exception(Macros.CONNECTION_REGAINED))
                    End If
                Else
                    'Reset the List ID if Session is closed
                    strListID = "000"
                    Throw (New Exception(Macros.CONNECTION_LOSS_EXCEPTION))
                End If
            Else
                'Else for Login
                objAppContainer.isConnected = True
                UpdateStatusBarMessage(ConnectionLostScenario)
                m_ConnectionLostExit = False
                Throw (New Exception(Macros.CONNECTION_REGAINED))
            End If
        ElseIf objAppContainer.iConnectedToAlternate = 0 Then
            'Reset the List ID if Session is closed
            Throw New Exception(Macros.CONNECTION_LOSS_EXCEPTION)
        End If
    End Sub
#End Region
#Region "UpdateStatusBarMessage"
    ''' <summary>
    ''' updates the status bar as not connectecd
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBarMessage(Optional ByVal ConnectionLostScenario As ConnectionLostScenario = ConnectionLostScenario.While_Others)
        Try
            ScreenMgr.GetInstance.UpdateStatusBar()
            If ConnectionLostScenario <> GOExportDataManager.ConnectionLostScenario.While_Sending_Start_Record Then
                Select Case objAppContainer.objActiveModule
                    Case AppContainer.ACTIVEMODULE.CRDCLM
                        'credit claim
                        CCSessionMgr.GetInstance().UpdateStatusBar()
                    Case AppContainer.ACTIVEMODULE.CRTRCL
                        'create recall
                        GOSessionMgr.GetInstance().UpdateStatusBar()
                    Case AppContainer.ACTIVEMODULE.GDSOUT
                        'Goods Out
                        GOSessionMgr.GetInstance().UpdateStatusBar()
                    Case AppContainer.ACTIVEMODULE.GDSTFR
                        'goods out transfer
                        GOTransferMgr.GetInstance().UpdateStatusBar()
                    Case AppContainer.ACTIVEMODULE.PHSLWT
                        'pharmacy special waste.
                        PSWSessionMgr.GetInstance().UpdateStatusBar()
                    Case AppContainer.ACTIVEMODULE.RECALL
                        'create recall.
                        RLSessionMgr.GetInstance().UpdateStatusBar()
                    Case AppContainer.ACTIVEMODULE.USERAUTH
                        RFUserSessionManager.GetInstance.UpdateStatusBar()
                End Select
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occured @:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End Region
#Region "CreateRCN"
    Public Function CreateRCN(ByVal strListID As String) As Boolean
        objAppContainer.objLogger.WriteAppLog("Sending an RCN", Logger.LogLevel.RELEASE)
        Dim bTemp As Boolean = False
        Dim lFreeMem As Long = Nothing
        Dim strExportData As New System.Text.StringBuilder()
        Dim strAppID As String = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_ID)
        Dim strMacID As String = objAppContainer.strMacAddress
        Dim strAppVersion As String = Nothing
        Dim aReleaseVersion() As String = Nothing
        Dim strDeviceType As String = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DEVICE_TYPE)
        strAppVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
        aReleaseVersion = strAppVersion.Split("-")
        strAppVersion = aReleaseVersion(1)
        Try
            'Appending the Trasaction Identifier
            strExportData.Append("RCN")
            'appending the User ID 
            strExportData.Append(objAppContainer.strCurrentUserID.PadLeft(3, "0"))
            'Appending Password
            strExportData.Append(objAppContainer.strPassword.PadLeft(3, "0"))
            'Appending APP ID 
            strExportData.Append(strAppID)
            'Appending APp version
            strExportData.Append(strAppVersion.PadLeft(4, "0"))
            'Appending the MACID
            If strMacID = "" Then
                strExportData.Append("000000000000")
            Else
                strExportData.Append(strMacID)
            End If
            'strExportData.Append(strAppID)
            strExportData.Append(strDeviceType)
            strExportData.Append(objAppContainer.objHelper.GetIPAddress())
            strExportData.Append(objAppContainer.objHelper.CheckForFreeMemory("Program Files", lFreeMem).PadLeft(8, "0"))
            'Time being commented out
            'Because List ID in RCN is 3 digit and In UOR is 4 digit
            'If strListID Is Nothing Then
            'Appending list id
            strExportData.Append("000")
            'Else
            '    strExportData.Append(strListID.PadLeft(3, "0"))
            'End If
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportData.ToString()) Then
                If ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification()) Then
                    Dim objResponse As Object = Nothing
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is ACKRecord Then
                            bTemp = True
                            Dim objAck As ACKRecord = CType(objResponse, ACKRecord)
                            'v1.1 MCF Change
                            If objAppContainer.iConnectedToAlternate <> 1 Then
                                If objAck.strAckMessage <> "" Then
                                    MessageBox.Show(objAck.strAckMessage, "Reconnect Successful", _
                                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                            MessageBoxDefaultButton.Button1)
                                End If
                            Else
                                objAppContainer.objLogger.WriteAppLog("Successfully auto logged in to the Alternate Controller", Logger.LogLevel.RELEASE)
                            End If
                            objAck = Nothing
                        ElseIf TypeOf (objResponse) Is NAKRecord Then
                            'While Reconnecting if any error comes 
                            'it has to be displayed to  the USer
                            bTemp = False
                            Dim objNAK As NAKRecord = CType(objResponse, NAKRecord)
                            If objNAK.strErrorMessage <> "" Then
                                MessageBox.Show(objNAK.strErrorMessage, "Error while Connecting", _
                                        MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
                            Else
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M60"), "Error while Connecting", _
                                       MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                       MessageBoxDefaultButton.Button1)
                            End If
                            objNAK = Nothing
                        ElseIf TypeOf (objResponse) Is NAKERROR Then
                            bTemp = False
                            'Donothing - Becoz already message would be displayed in NAKERROR Parsing
                        End If
                    End If
                    objResponse = Nothing
                End If
            Else
                ' If Not DATAPOOL.getInstance.isConnected Then
                'Reconnect()
                ''End If
                'bTemp = False
                'If again reconnection is called, this will go into a loop
                Return False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Create PGF:: Exception :: " + ex.Message, Logger.LogLevel.INFO)
        End Try
        Return bTemp
    End Function
#End Region
#Region "CreateDSS"
    Public Function CreateDSS() As Boolean
        Dim bTemp As Boolean = True
        Try
            Dim strExportData As String = "DSS" + objAppContainer.strPreviousUserID.PadLeft(3, "0")
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportData) Then
                If ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification()) Then
                    Dim objResponse As Object = Nothing
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is ACKRecord Then
                            bTemp = True
                        End If
                    End If
                End If
            Else
                ' If Not DATAPOOL.getInstance.isConnected Then
                Reconnect()
                'End If
                bTemp = False
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("CreateDSS:: Error ::" + ex.Message, Logger.LogLevel.RELEASE)
        End Try
        Return bTemp
    End Function
#End Region
#Region "CreateDSG"
    Public Function CreateDSG(ByVal cBusinessCentre As Char) As Boolean
        Dim bTemp As Boolean = False
        Try
            Dim strExportData As New System.Text.StringBuilder()
            strExportData.Append("DSG")
            strExportData.Append(objAppContainer.strPreviousUserID.PadLeft(3, "0"))
            strExportData.Append(cBusinessCentre)
            strExportData.Append("0001")
            DATAPOOL.getInstance.ResetPoolData()
            m_PreviousMessage = strExportData.ToString()
            If m_TransactDataTransmitter.SendRecordRF(m_PreviousMessage) Then
                'm_PreviousMessage = ""         'Fix for 4290
                bTemp = ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification())
            Else
                If Not DATAPOOL.getInstance.isConnected Then
                    Reconnect()
                End If
                bTemp = False
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            m_PreviousMessage = ""
        End Try
        Return bTemp
    End Function
#End Region
#Region "CreateRCI"
    Public Function CreateRCI(ByVal strRecallRefNo As String, ByRef strInstructions As String) As Boolean
        Dim bTemp As Boolean = False
        Dim strExportData As New System.Text.StringBuilder()
        Dim objResponse As Object = Nothing
        Dim objRCJRecord As RCJRecord
        Try
            'Transaction Identifier
            strExportData.Append("RCI")
            'OperatorID
            strExportData.Append(objAppContainer.strCurrentUserID.PadLeft(3, "0"))
            'Reference Number 
            strExportData.Append(strRecallRefNo.PadLeft(8, "0"))
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportData.ToString()) Then
                If ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification(), ConnectionLostScenario.While_Retrieving_the_List) Then
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is RCJRecord Then
                            objRCJRecord = CType(objResponse, RCJRecord)
                            If objRCJRecord.strRecallRefNo.Trim.Equals(strRecallRefNo) Then
                                strInstructions = objRCJRecord.strSpecialInst
                                bTemp = True
                            End If
                            objRCJRecord = Nothing
                        End If
                    End If
                    objResponse = Nothing
                End If
            Else
                ' If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(ConnectionLostScenario.While_Retrieving_the_List)
                'End If
                bTemp = False
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Error :: Create RCI :: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
        Return bTemp
    End Function
#End Region
#Region "SendNextSequence"
    Public Function SendNextSequence(ByVal strSequenceNumber As String) As Boolean
        Dim bTemp As Boolean = False
        Dim iIndex As Integer = 0
        Try
            If m_PreviousMessage = "" Then
                DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.Disconnected)
            Else
                Select Case m_PreviousMessage.Substring(0, 3)
                    Case "DSG"
                        m_PreviousMessage = m_PreviousMessage.Remove(DSG.LISTNUM_OFFSET, DSG.LISTNUM)
                        m_PreviousMessage = m_PreviousMessage.Insert(DSG.LISTNUM_OFFSET, strSequenceNumber.PadLeft(4, "0"))
                        bTemp = m_TransactDataTransmitter.SendRecordRF(m_PreviousMessage)
                    Case "RCD"
                        m_PreviousMessage = m_PreviousMessage.Remove(RCD.INDEX_OFFSET, RCD.INDEX)
                        m_PreviousMessage = m_PreviousMessage.Insert(RCD.INDEX_OFFSET, strSequenceNumber.PadLeft(4, "0"))
                        bTemp = m_TransactDataTransmitter.SendRecordRF(m_PreviousMessage)
                    Case "RCH"
                        iIndex = CInt(m_PreviousMessage.Substring(RCH.INDEX_OFFSET, RCH.INDEX)) + Macros.RECALL_RCF_ITEM_COUNT
                        strSequenceNumber = iIndex.ToString.PadLeft(4, strSequenceNumber)
                        m_PreviousMessage = m_PreviousMessage.Remove(RCH.INDEX_OFFSET, RCH.INDEX)
                        m_PreviousMessage = m_PreviousMessage.Insert(RCH.INDEX_OFFSET, strSequenceNumber)
                        bTemp = m_TransactDataTransmitter.SendRecordRF(m_PreviousMessage)
                End Select
            End If
        Catch ex As Exception

        End Try
        Return bTemp
    End Function
#End Region
#End If
    ''' <summary>
    ''' The function creates the  export data record for UOS
    ''' </summary>
    ''' <param name="objCreateUOS"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateUOS(ByVal objCreateUOS As UOSRecord) As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strListType As String = Nothing
        Dim strOperatorName As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()

        'Right Justify ,zero filled
        If objCreateUOS.strIsListType = String.Empty Then
            objCreateUOS.strIsListType = ""
            strListType = objCreateUOS.strIsListType.PadLeft(3, "0")
        End If
        If objCreateUOS.strIsListType <> "" Then
            strListType = objCreateUOS.strIsListType.PadLeft(3, "0")
        End If
        strOperatorName = objAppContainer.strCurrentUserName.Trim().PadRight(15, " ")
        'Appending the details to the string 
        strExportDataString.Append("UOS")
        strExportDataString.Append(",")
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        'strExportDataString.Append(strOperatorName)
        'strExportDataString.Append(",")
        'strExportDataString.Append(DateAndTime.Now().ToString("yyyyMMddhhmm"))
        'strExportDataString.Append(",")
        strExportDataString.Append(objCreateUOS.strIsListType)
        Try
#If NRF Then
             strExportDataString.Append(Environment.NewLine)
 'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
#ElseIf RF Then
            If m_ConnectionLostExit Then
                'Though RCN is send the next session start has to be send...
                Reconnect(ConnectionLostScenario.While_Sending_Start_Record)
            End If
            Dim strExportData As String = strExportDataString.ToString().Replace(",", "")
            Dim objResponse As Object = Nothing
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportData) Then
                If ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification(), ConnectionLostScenario.While_Sending_Start_Record) Then
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is UORRecord Then
                            Dim objUOR As UORRecord
                            objUOR = CType(objResponse, UORRecord)
                            strBusinessCentre = objUOR.strBusinessCentre
                            strListID = objUOR.strListNumber
                            bTemp = True
                            objUOR = Nothing
                        End If
                    End If
                    objResponse = Nothing
                End If
            Else
                'checking whether record send error is because of connection loss
                ' If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(ConnectionLostScenario.While_Sending_Start_Record)
                'End If
                bTemp = False
            End If
#End If

            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: UOS record write success", _
                                                  Logger.LogLevel.INFO)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: UOS record write failure" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_REGAINED Or ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Then
                Throw ex
            End If
#End If

            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates the  export data record for STQ
    ''' </summary>
    ''' <param name="objSTQRecord"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateSTQ(ByVal objSTQRecord As STQRecord, Optional ByRef UODNumber As String = "") As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strUODNumber As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()

        'Appending the details to the string 
        strExportDataString.Append("STQ")
        strExportDataString.Append(",")
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        strUODNumber = objSTQRecord.strUODNumber.PadLeft(8, "0")
        strExportDataString.Append(strUODNumber)

        Try
#If NRF Then
             strExportDataString.Append(Environment.NewLine)
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
#ElseIf RF Then
            'check for this message
            Dim strExportData As String = strExportDataString.ToString().Replace(",", "")
            Dim objResponse As Object = Nothing
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportData) Then
                If ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification()) Then
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is STRRecord Then
                            Dim objSTRRecord As STRRecord
                            objSTRRecord = CType(objResponse, STRRecord)
                            UODNumber = objSTRRecord.strUODNo + objSTRRecord.strUODSuffix
                            bTemp = True
                        Else
                            bTemp = False
                        End If
                        objSTQRecord = Nothing
                    End If
                    objResponse = Nothing
                End If
            Else
                'checking whether record send error is because of connection loss
                ' If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(ConnectionLostScenario.While_Scanning_UOD_Label)
                'End If
                bTemp = False
            End If
#End If

            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: STQ record write success", _
                                                  Logger.LogLevel.INFO)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: STQ record write failure" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            'for send messages no exception need to be thrown. Only for get data message it is necessary
            '#If RF Then
            '            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION or ex.Message = Macros.CONNECTION_REGAINED Then
            '                Throw ex
            '            End If
            '#End If
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates the  export data record for UOA
    ''' </summary>
    ''' <param name="objCreateUOA"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateUOA(ByVal objCreateUOA As UOARecord) As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strListNum As String = Nothing
        Dim strSeqNumber As String = Nothing
        Dim strBootscode As String = Nothing
        Dim strNumQuantity As String = Nothing
        Dim strShortDesc As String = Nothing
        Dim strDescSEL As String = Nothing
        Dim strPrice As String = Nothing
        Dim strTotalPrice As String = Nothing
        Dim strItemBC As String = Nothing
        Dim strBCName As String = Nothing
        Dim strBarcode As String = Nothing
        Dim strIsStatus As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()

        ' Check for padding for object field with values
        Dim iListId As Integer = CInt(ConfigDataMgr.GetInstance().GetParam(ConfigKey.LIST_ID))
        iListId = iListId + 1
        strListNum = CStr(iListId).Trim().PadLeft(4, "0")
#If RF Then
        strListNum = strListID.Trim().PadLeft(4, "0")
#End If
        '#If NRF Then
         strSeqNumber = objCreateUOA.strsequencenumber.Trim().PadLeft(4, "0")
'#ElseIf RF Then
'        strSeqNumber = strListID.Trim().PadLeft(4, "0")
'#End If
        'strSeqNumber = objCreateUOA.strsequencenumber.Trim().PadLeft(4, "0")
        strBootscode = objCreateUOA.strbootscode.Trim().PadLeft(7, "0")
        strNumQuantity = objCreateUOA.strquanity.Trim().PadLeft(4, "0")

        If objCreateUOA.strsdescription.Length > 20 Then
            objCreateUOA.strsdescription = objCreateUOA.strsdescription.Substring(0, 20)
        End If
        strShortDesc = objCreateUOA.strsdescription.Trim().PadRight(20, " ")
        strDescSEL = objCreateUOA.strdescSEL.Trim().PadRight(45, " ")
        If objCreateUOA.strnumPrice.Length > 6 Then
            objCreateUOA.strnumPrice = objCreateUOA.strnumPrice.Substring(objCreateUOA.strnumPrice.Length - 6, 6)
        End If
        strPrice = objCreateUOA.strnumPrice.Trim().PadLeft(6, "0")
        strTotalPrice = objCreateUOA.strnumTotalPrice.Trim().PadLeft(6, "0")
        strItemBC = objCreateUOA.stritembc.Trim().PadLeft(1, " ")
        If objCreateUOA.strbcname.Length > 14 Then
            objCreateUOA.strbcname = objCreateUOA.strbcname.Substring(0, 14)
        End If
        strBCName = objCreateUOA.strbcname.Trim().PadRight(14, " ")
        'DEFECT FIX - BTCPR00004162
        ' strBarcode = objCreateUOA.strbarcode.Trim().PadLeft(13, "0")
        strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(objCreateUOA.strbarcode.Trim()).PadLeft(13, "0")
        strIsStatus = objCreateUOA.strIsStatus.Trim().PadLeft(1, " ")


        'Appending the details to the string 
        strExportDataString.Append("UOA")
        strExportDataString.Append(",")
        strExportDataString.Append(objAppContainer.strCurrentUserID)    'Added as per message document.
        strExportDataString.Append(",")
        strExportDataString.Append(strListNum)
        strExportDataString.Append(",")
        strExportDataString.Append(strSeqNumber)
        strExportDataString.Append(",")
        strExportDataString.Append(strBootscode)
        strExportDataString.Append(",")
        strExportDataString.Append(strNumQuantity)
        strExportDataString.Append(",")
        strExportDataString.Append(strShortDesc)
        strExportDataString.Append(",")
        strExportDataString.Append(strDescSEL)
        strExportDataString.Append(",")
        strExportDataString.Append(strPrice)
        strExportDataString.Append(",")
        strExportDataString.Append(strTotalPrice)
        strExportDataString.Append(",")
        strExportDataString.Append(strItemBC)
        strExportDataString.Append(",")
        strExportDataString.Append(strBCName)
        strExportDataString.Append(",")
        strExportDataString.Append(strBarcode)
        strExportDataString.Append(",")
        strExportDataString.Append(strIsStatus)


        Try
#If NRF Then
            strExportDataString.Append(Environment.NewLine)
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
#ElseIf RF Then
            Dim objResponse As Object = Nothing
            Dim objTempConnScenario As ConnectionLostScenario = ConnectionLostScenario.While_Others
            If strIsStatus.Equals("A") And Val(strSeqNumber) = 1 Then
                'Reconnect(ConnectionLostScenario.While_Others)
                objTempConnScenario = ConnectionLostScenario.While_Others
            ElseIf strIsStatus.Equals("A") And Val(strSeqNumber) > 1 Then
                'Reconnect(ConnectionLostScenario.While_Sending_Item_Add)
                objTempConnScenario = ConnectionLostScenario.While_Sending_Item_Add
            ElseIf strIsStatus.Equals("X") Then
                'Reconnect(ConnectionLostScenario.While_Sending_Void)
                objTempConnScenario = ConnectionLostScenario.While_Sending_Void
            End If
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString().Replace(",", "")) Then
                If ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification(), objTempConnScenario) Then
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is ACKRecord Then
                            m_SeqNumber = Val(strSeqNumber)
                            bTemp = True
                        End If
                    End If
                    objResponse = Nothing
                End If
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                'If strIsStatus.Equals("A") And Val(strSeqNumber) = 1 Then
                '    Reconnect(ConnectionLostScenario.While_Others)
                'ElseIf strIsStatus.Equals("A") And Val(strSeqNumber) > 1 Then
                '    Reconnect(ConnectionLostScenario.While_Sending_Item_Add)
                'ElseIf strIsStatus.Equals("X") Then
                '    Reconnect(ConnectionLostScenario.While_Sending_Void)
                'End If
                'End If
                Reconnect(objTempConnScenario)
                bTemp = False
            End If

#End If

            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: POA record write success", _
                                                  Logger.LogLevel.INFO)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: POA record write failure" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            'for send messages no exception need to be thrown. Only for get data message it is necessary
            '#If RF Then
            '            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION or ex.Message = Macros.CONNECTION_REGAINED Then
            '                Throw ex
            '            End If
            '#End If
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates the  export data record for UOX
    ''' </summary>
    ''' <param name="objCreateUOX"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateUOX(ByVal objCreateUOX As UOXRecord) As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strListNumber As String = Nothing
        Dim strIsListType As String = Nothing
        Dim strUOD As String = Nothing
        Dim strIsStatus As String = Nothing
        Dim strItemCount As String = Nothing
        Dim strStockFigure As String = Nothing
        Dim strSupplierRoute As String = Nothing
        Dim strDisplayLocation As String = Nothing
        Dim strBCName As String = Nothing
        Dim strBCDescription As String = Nothing
        Dim strRecallNumber As String = Nothing
        Dim strAuthCode As String = Nothing
        Dim strSupplier As String = Nothing
        Dim strMethod As String = Nothing
        Dim strCarrier As String = Nothing
        Dim strNumBird As String = Nothing
        Dim strNumReason As String = Nothing
        Dim strRecallStore As String = Nothing
        Dim strDestination As String = Nothing
        Dim strWRoute As String = Nothing
        Dim strIsUODType As String = Nothing
        Dim strReasonDamage As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()

        'Condition handling for various senario
        If objCreateUOX.strIsUODType = "03" Or objCreateUOX.strIsUODType = "3" Then
            objCreateUOX.strReasonDamage = "02"
            If objCreateUOX.strSupplierRoute = "C" Then
                objCreateUOX.strWroute = "C"
            Else
                objCreateUOX.strWroute = "R"
            End If
        End If

        'Right justify and zero filled
        Dim iListId As Integer = CInt(ConfigDataMgr.GetInstance().GetParam(ConfigKey.LIST_ID))
        iListId = iListId + 1
        strListNumber = CStr(iListId).Trim().PadLeft(4, "0")
        strIsListType = objCreateUOX.strisListType.ToString().PadLeft(1, "0")
        strUOD = objCreateUOX.strUOD.Trim().PadLeft(14, "0")
        strIsStatus = objCreateUOX.strIsStatus.PadLeft(1, " ")
        strItemCount = objCreateUOX.strItemCount.Trim().PadLeft(4, "0")
        strStockFigure = objCreateUOX.strIsStockFigure.PadLeft(1, "0")
        If objCreateUOX.strSupplierRoute <> "" And objCreateUOX.strSupplierRoute <> "W" Then
            strSupplierRoute = "O" 'Set to letter 0 if Direct
        Else
            strSupplierRoute = objCreateUOX.strSupplierRoute.ToString().PadLeft(1, " ")
        End If
        strDisplayLocation = objCreateUOX.strDisplayLoc.PadLeft(1, " ")

        'Fix for unknown item
        'If strBCName = "" Then
        '    strBCName = ""
        'Else
        strBCName = objCreateUOX.strBCname.PadLeft(1, " ")
        'End If
        'If strBCDescription = "" Then
        'strBCDescription = ""
        'Else
        strBCDescription = objCreateUOX.strBCdesc.Trim().PadRight(14, " ")
        'End If
        strRecallNumber = objCreateUOX.strRecall.Trim().PadLeft(8, " ")
        strAuthCode = objCreateUOX.strAuthCode.Trim().PadLeft(15, " ")
        strSupplier = objCreateUOX.strSupplier.Trim().PadLeft(15, " ")
        strMethod = objCreateUOX.strMethod.Trim().PadLeft(2, "0")
        strCarrier = objCreateUOX.strCarrier.Trim().PadLeft(2, "0")
        strNumBird = objCreateUOX.strNumbird.Trim().PadLeft(8, " ")
        strNumReason = objCreateUOX.strNumReason.Trim().PadLeft(2, "0")
        strRecallStore = objCreateUOX.strRecStore.Trim().PadLeft(4, "0")
        strDestination = objCreateUOX.strDestination.Trim().PadLeft(2, "0")
        strWRoute = objCreateUOX.strWroute.PadLeft(1, " ")
        strIsUODType = objCreateUOX.strIsUODType.Trim().PadLeft(2, "0")
        strReasonDamage = objCreateUOX.strReasonDamage.Trim().PadLeft(2, "0")


        'Appending the details to the string 
        strExportDataString.Append("UOX")
        strExportDataString.Append(",")
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
#If NRF Then
        strExportDataString.Append(strListNumber)
#ElseIf RF Then
        strExportDataString.Append(strListID.PadLeft(4, "0"))
#End If

        strExportDataString.Append(",")
        strExportDataString.Append(strIsListType)
        strExportDataString.Append(",")
        strExportDataString.Append(strUOD)
        strExportDataString.Append(",")
        strExportDataString.Append(strIsStatus)
        strExportDataString.Append(",")
        strExportDataString.Append(strItemCount)
        strExportDataString.Append(",")
        strExportDataString.Append(strStockFigure)
        strExportDataString.Append(",")
        strExportDataString.Append(strSupplierRoute)
        strExportDataString.Append(",")
        strExportDataString.Append(strDisplayLocation)
        strExportDataString.Append(",")
        strExportDataString.Append(strBCName)
        strExportDataString.Append(",")
        strExportDataString.Append(strBCDescription)
        strExportDataString.Append(",")
        strExportDataString.Append(strRecallNumber)
        strExportDataString.Append(",")
        strExportDataString.Append(strAuthCode)
        strExportDataString.Append(",")
        strExportDataString.Append(strSupplier)
        strExportDataString.Append(",")
        strExportDataString.Append(strMethod)
        strExportDataString.Append(",")
        strExportDataString.Append(strCarrier)
        strExportDataString.Append(",")
        strExportDataString.Append(strNumBird)
        strExportDataString.Append(",")
        strExportDataString.Append(strNumReason)
        strExportDataString.Append(",")
        strExportDataString.Append(strRecallStore)
        strExportDataString.Append(",")
        strExportDataString.Append(strDestination)
        strExportDataString.Append(",")
        strExportDataString.Append(strWRoute)
        strExportDataString.Append(",")
        strExportDataString.Append(strIsUODType)
        strExportDataString.Append(",")
        strExportDataString.Append(strReasonDamage)


        Try
#If NRF Then
             strExportDataString.Append(Environment.NewLine)
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                     strExportDataString.ToString(), True)
#ElseIf RF Then
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString().Replace(",", "")) Then
                'UOX Send is successful
                'Since this is the last record , connection cut in betwee nUOX send and ACK recieved should have diff msg
                'So the isEndRecord flag is set to true
                If ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification(), ConnectionLostScenario.While_Sending_End_Record) Then
                    Dim objResponse As Object = Nothing
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is ACKRecord Then
                            m_SeqNumber = 0
                            bTemp = True
                        End If
                    End If
                    objResponse = Nothing
                End If
            Else
                'Not setting the end record flag becoz UOX is not successfully send
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(ConnectionLostScenario.While_Sending_End_Record)
                'End If
                bTemp = False
            End If
#End If

            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: UOX record write success", _
                                                  Logger.LogLevel.INFO)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: UOX record write failure" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            '#If RF Then
            '            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION or ex.Message = Macros.CONNECTION_REGAINED Then
            '                Throw ex
            '            End If
            '#End If
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates the transact message for RCA
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateRCA() As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()

        'Appending the details to the string 
        strExportDataString.Append("RCA")
        strExportDataString.Append(",")
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        Try
#If NRF Then
             strExportDataString.Append(Environment.NewLine)
    'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
#ElseIf RF Then
            If m_ConnectionLostExit Then
                Reconnect(ConnectionLostScenario.While_Sending_Start_Record)
            End If
            Dim objResponse As Object = Nothing
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString.Replace(",", "")) Then
                If ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification(), ConnectionLostScenario.While_Sending_Start_Record) Then
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is ACKRecord Then
                            bTemp = True
                        ElseIf TypeOf (objResponse) Is NAKRecord Then
                            bTemp = False
                            'Dim objNAK As NAKRecord = CType(objResponse, NAKRecord)
                            'If objNAK.strErrorMessage.Trim(" ") <> "" Then
                            'MessageBox.Show(objNAK.strErrorMessage.Trim(" "), "Info", _
                            '                MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
                            'End If
                        End If
                    End If
                    DATAPOOL.getInstance.ResetPoolData()
                    objResponse = Nothing
                End If
            Else
                '   If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(ConnectionLostScenario.While_Sending_Start_Record)
                'End If
                bTemp = False
            End If
#End If
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: RCA record write success", _
                                                  Logger.LogLevel.INFO)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: RCA record write failure" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            If ex.Message = Macros.CONNECTIVITY_TIMEOUTCANCEL And objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.RECALL Then
                Throw ex
            End If
#End If
            Return False
        End Try
        Return bTemp
    End Function

    ''' <summary>
    ''' The function creates the transact message for RCB
    ''' </summary>
    ''' <param name="objCreateRCB"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateRCB(ByVal objCreateRCB As RCBRecord) As Boolean
        'Local variable declaration
        Dim bTemp As Boolean = False
        Dim strRecallReference As String = Nothing
        Dim strUOD As String = Nothing
        Dim strRecallStatus As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()

        'Right justify zero filled
        If objCreateRCB.strRecallref <> "" Then
            strRecallReference = objCreateRCB.strRecallref.PadLeft(8, "0")
        End If
        If objCreateRCB.strnumUOD <> "" Then
            strUOD = objCreateRCB.strnumUOD.PadLeft(14, "0")
        End If
        If objCreateRCB.strStateCall <> "" Then
            strRecallStatus = objCreateRCB.strStateCall.PadLeft(1, " ")
        End If

        'Appending the details to the string 
        strExportDataString.Append("RCB")
        strExportDataString.Append(",")
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        strExportDataString.Append(strRecallReference)
        strExportDataString.Append(",")
        strExportDataString.Append(strUOD)
        strExportDataString.Append(",")
        strExportDataString.Append(strRecallStatus)


        Try
#If NRF Then
             strExportDataString.Append(Environment.NewLine)
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
#ElseIf RF Then

            Dim objResponse As Object = Nothing
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString.Replace(",", "")) Then
                'RCB is send succesfully
                'Since this is the last record and has been successfully send hence we set the flag as true
                If ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification(), ConnectionLostScenario.While_Sending_End_Record) Then
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is ACKRecord Then
                            bTemp = True
                        End If
                    End If
                    DATAPOOL.getInstance.ResetPoolData()
                    objResponse = Nothing
                End If
            Else
                'Here last record sending failed. hence the flag is not set appropriately
                '    If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(ConnectionLostScenario.While_Sending_End_Record)
                'End If
                bTemp = False
            End If

#End If
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: RCB record write success", _
                                                  Logger.LogLevel.INFO)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: RCB record write failure" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            '#If RF Then
            '            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION or ex.Message = Macros.CONNECTION_REGAINED Then
            '                Throw ex
            '            End If
            '#End If
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates the transact message for RCG
    ''' </summary>
    ''' <param name="objCreateRCG"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateRCG(ByVal objCreateRCG As RCGRecord) As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strRecallReference As String = Nothing
        Dim strRecallItem As String = Nothing
        Dim strRecallCount As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()

        'Right justify zero filled
        If objCreateRCG.strRecallRef <> "" Then
            strRecallReference = objCreateRCG.strRecallRef.PadLeft(8, "0")
        End If
        If objCreateRCG.strRecallItem <> "" Then
            strRecallItem = objCreateRCG.strRecallItem.PadLeft(6, "0")
        End If
        If objCreateRCG.strRecallCount <> "" Then
            strRecallCount = objCreateRCG.strRecallCount.PadLeft(4, "0")
        End If

        'Appending the details to the string 
        strExportDataString.Append("RCG")
        strExportDataString.Append(",")
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        strExportDataString.Append(strRecallReference)
        strExportDataString.Append(",")
        strExportDataString.Append(strRecallItem)
        strExportDataString.Append(",")
        strExportDataString.Append(strRecallCount)


        Try
#If NRF Then
             strExportDataString.Append(Environment.NewLine)
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                       strExportDataString.ToString(), True)
#ElseIf RF Then
            Dim objResponse As Object = Nothing
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString().Replace(",", "")) Then
                If ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification(), ConnectionLostScenario.While_Sending_Item_Add) Then
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is ACKRecord Then
                            m_SeqNumber = m_SeqNumber + 1 'To mention that first item is updated.
                            bTemp = True
                        End If
                        objResponse = Nothing
                    End If
                End If
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(ConnectionLostScenario.While_Sending_Item_Add)
                'End If
                bTemp = False
            End If

#End If
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: RCG record write success", _
                                                  Logger.LogLevel.INFO)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: RCG record write failure" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)

            '#If RF Then
            '            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION or ex.Message = Macros.CONNECTION_REGAINED Then
            '                Throw ex
            '            End If
            '#End If
            Return False
        End Try
        Return bTemp
    End Function
#If RF Then
    Public Function CreateUOQ(ByVal strUODNumber As String) As Boolean
        Dim bTemp As Boolean = False
        Dim strExportData As New System.Text.StringBuilder
        Dim objResponse As Object = Nothing
        Try
            'Transaction Identifier
            strExportData.Append("UOQ")
            'Appending the opertaor ID
            strExportData.Append(objAppContainer.strCurrentUserID)
            'Appending the UOD Number
            strExportData.Append(strUODNumber.PadLeft(14, "0"))
            'Resetting the DATA Pool for the transaction start
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportData.ToString()) Then
                If ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification(), ConnectionLostScenario.While_Scanning_UOD_Label) Then
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is ACKRecord Then
                            bTemp = True
                        End If
                        objResponse = Nothing
                    End If
                End If
            Else
                ' If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(ConnectionLostScenario.While_Scanning_UOD_Label)
                'End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception Occured @ Create UOQ", Logger.LogLevel.RELEASE)
            bTemp = False
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
        Finally
            strExportData = Nothing
            objResponse = Nothing
        End Try
        Return bTemp
    End Function
    Public Function CreateRCD(ByVal strRecallType As String) As Boolean
        Dim bTemp As Boolean = False
        Try
            Dim strExportData As New System.Text.StringBuilder
            'Transaction ID is Appended
            strExportData.Append("RCD")
            'The USer Is is appended
            strExportData.Append(objAppContainer.strCurrentUserID.PadLeft(3, "0"))
            'The Default Sequence Number / Index - 000 is appended
            strExportData.Append("0000")
            strExportData.Append(strRecallType)
            m_PreviousMessage = strExportData.ToString()
            strExportData = Nothing
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(m_PreviousMessage) Then
                ' bTemp = ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification(), ConnectionLostScenario.While_Retrieving_the_List)
                bTemp = ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification(), ConnectionLostScenario.While_Retreiving_Act_Recall_List)
            Else
                ' If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(ConnectionLostScenario.While_Retreiving_Act_Recall_List)
                ' Reconnect(ConnectionLostScenario.While_Retrieving_the_List)
                'End If
                bTemp = False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occurred @ " + ex.StackTrace, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            If ex.Message = Macros.CONNECTIVITY_TIMEOUTCANCEL And objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.RECALL Then
                Throw ex
            End If
#End If
        End Try
        Return bTemp
    End Function


    Public Function CreateRCH(ByVal strRecallRefNumber As String) As Boolean
        Dim bTemp As Boolean = False
        Try
            Dim strExportData As New System.Text.StringBuilder
            'Transaction ID is Appended
            strExportData.Append("RCH")
            'The USer Is is appended
            strExportData.Append(objAppContainer.strCurrentUserID.PadLeft(3, "0"))
            'Appendinf Recall Reference Number
            strExportData.Append(strRecallRefNumber)
            'Index 
            strExportData.Append("0000")
            m_PreviousMessage = strExportData.ToString()
            strExportData = Nothing
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(m_PreviousMessage) Then
                bTemp = ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification(), ConnectionLostScenario.While_Retrieving_the_List)
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(ConnectionLostScenario.While_Retrieving_the_List)
                '  End If
                bTemp = False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occured @ " + ex.StackTrace, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
        End Try
        Return bTemp
    End Function
#End If
    ''' <summary>
    ''' The function creates the transact message for SOR(Sign On Request)
    ''' </summary>
    ''' <param name="strPassword"></param>
    ''' <remarks>object of the record to be written</remarks>
    Public Function CreateSOR(ByVal strPassword As String) As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strFreeMem As String = Nothing  'Free memory in Mega Bits (Mb)
        Dim strAppId As String = Nothing
        'Dim strMACId As String = Nothing
        Dim strIPAdd As String = Nothing
        Dim strAppVersion As String = Nothing
        Dim strStoreType As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()
        Dim lFreeMem As Long = Nothing
        Dim strVersion As String = Nothing
        'Read app version from config file
        'To split the appverion and release version. Actual app version should be send in SOR
        Dim aReleaseVersion() As String = Nothing
        Dim iAttempt As Integer = 0
        strVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
        aReleaseVersion = strVersion.Split("-")
        strAppVersion = aReleaseVersion(1)
        'Read app version from config file
        'strAppVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION)
        strAppId = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_ID)
        strStoreType = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DEVICE_TYPE)

        Do
            objAppContainer.strMacAddress = objAppContainer.objHelper.GetMacAddress()
            iAttempt = iAttempt + 1
            Threading.Thread.Sleep(2000)
        Loop Until (objAppContainer.strMacAddress <> "000000000000" Or iAttempt = 2)

        objAppContainer.objLogger.WriteAppLog("Get MAC ID success" + objAppContainer.strMacAddress, _
                                              Logger.LogLevel.RELEASE)
        strFreeMem = objAppContainer.objHelper.CheckForFreeMemory("Program Files", lFreeMem)
        objAppContainer.objLogger.WriteAppLog("Get Free Mem success" + _
                                              strFreeMem, Logger.LogLevel.RELEASE)

        'Right justify,zero-filled
        strAppVersion = strAppVersion.PadLeft(4, "0")
        strAppVersion = strAppVersion.Replace(".", "0")
        strFreeMem = strFreeMem.PadLeft(8, "0")

        'Split each subnet and pad left with 0s
        strIPAdd = objAppContainer.objHelper.GetIPAddress()
        objAppContainer.objLogger.WriteAppLog("Get IP success" + strIPAdd, _
                                              Logger.LogLevel.INFO)

        'Appending the details to the string 
        strExportDataString.Append("SOR")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        strExportDataString.Append(strPassword)
        strExportDataString.Append(",")
        strExportDataString.Append(strAppId)
        strExportDataString.Append(",")
        strExportDataString.Append(strAppVersion)
        strExportDataString.Append(",")
        strExportDataString.Append(objAppContainer.strMacAddress)
        strExportDataString.Append(",")
        strExportDataString.Append(strStoreType)
        strExportDataString.Append(",")
        strExportDataString.Append(strIPAdd)
        strExportDataString.Append(",")
        strExportDataString.Append(strFreeMem)

        Try
#If NRF Then
            strExportDataString.Append(Environment.NewLine)
              'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
#ElseIf RF Then
            strExportDataString = strExportDataString.Replace(",", "")
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
                bTemp = ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification(), ConnectionLostScenario.While_Login)
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(ConnectionLostScenario.While_Login)
                'End If
                bTemp = False
            End If
#End If
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: SOR record write success", _
                                                  Logger.LogLevel.INFO)
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: SOR record write failure" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' To add for User Auth
    ''' </summary>
    ''' <param name="IsCallForCrash"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateOFF(ByVal IsCallForCrash As Boolean) As Boolean

        ' Local variable declaration
        Dim bTemp As Boolean
        Dim strExportDataString As New System.Text.StringBuilder()

        'Appending the details to the string 
        strExportDataString.Append("OFF")
        strExportDataString.Append(",")

        'Check whether the OFF record is for crash recovery or not
        If IsCallForCrash Then
            'Take the Operation ID from the Config File
            strExportDataString.Append(ConfigDataMgr.GetInstance.GetParam(ConfigKey.PREVIOUS_USER))
        Else
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
        End If


        Try
#If NRF Then
             strExportDataString.Append(Environment.NewLine)
               'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
#ElseIf RF Then
            If m_ConnectionLostExit Then
                Reconnect(ConnectionLostScenario.While_Log_OFF)
            End If
            strExportDataString.Replace(",", "")
            DATAPOOL.getInstance.ResetPoolData()
            If (m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())) Then
                If ProcessDataRecieved(DATAPOOL.getInstance.WaitForNotification(), ConnectionLostScenario.While_Log_OFF) Then
                    Dim Returnobject As Object = Nothing
                    If DATAPOOL.getInstance.GetNextObject(Returnobject) Then
                        If TypeOf (Returnobject) Is ACKRecord Then
                            bTemp = True
                        ElseIf TypeOf (Returnobject) Is NAKRecord Then
                            bTemp = False
                        ElseIf TypeOf (Returnobject) Is NAKERRORRecord Then
                            'Allow user to gracefully log of when error record is recieved
                            bTemp = True
                        End If
                    End If
                    Returnobject = Nothing
                End If
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(ConnectionLostScenario.While_Log_OFF)
                'End If
                bTemp = False
            End If
#End If
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: OFF record write success", _
                                                  Logger.LogLevel.INFO)
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GOExportDataManager: OFF record write failure" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' Generate SOR export
    ''' </summary>
    ''' <param name="strPassword"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GenerateSOR(ByVal strPassword As String, Optional ByVal strUserID As String = "") As String
        Dim strFreeMem As String = Nothing  'Free memory in Mega Bits (Mb)
        Dim strAppId As String = Nothing
        Dim strMACId As String = Nothing
        Dim strIPAdd As String = Nothing
        Dim strAppVersion As String = Nothing
        Dim strStoreType As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()
        Dim lFreeMem As Long = Nothing

        'Read app version from config file
        Dim strVersion As String = Nothing
        'Read app version from config file
        'To split the appverion and release version. Actual app version should be send in SOR
        Dim aReleaseVersion() As String = Nothing
        Dim iAttempt As Integer = 0
        strVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
        aReleaseVersion = strVersion.Split("-")
        strAppVersion = aReleaseVersion(1)
        strAppId = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_ID)
        strStoreType = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DEVICE_TYPE)

        Do
            strMACId = objAppContainer.objHelper.GetMacAddress()
            iAttempt = iAttempt + 1
            Threading.Thread.Sleep(2000)
        Loop Until (strMACId <> "000000000000" Or iAttempt = 2)

        objAppContainer.objLogger.WriteAppLog("GOExportDataManager: Get MAC ID success" + strMACId, _
                                              Logger.LogLevel.RELEASE)
        strFreeMem = objAppContainer.objHelper.CheckForFreeMemory("Program Files", lFreeMem)
        objAppContainer.objLogger.WriteAppLog("GOExportDataManager: Get Free Mem success" + strFreeMem, _
                                              Logger.LogLevel.RELEASE)

        'Right justify,zero-filled
        strAppVersion = strAppVersion.PadLeft(4, "0")
        strAppVersion = strAppVersion.Replace(".", "0")
        strFreeMem = strFreeMem.PadLeft(8, "0")

        'Split each subnet and pad left with 0s
        strIPAdd = objAppContainer.objHelper.GetIPAddress()
        objAppContainer.objLogger.WriteAppLog("SMExportDataManager: Get IP success" + strIPAdd, _
                                              Logger.LogLevel.INFO)

        'Appending the details to the string 
        strExportDataString.Append("SOR")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        If strUserID <> "" Then
            strExportDataString.Append(strUserID)
        Else
            strExportDataString.Append(objAppContainer.strCurrentUserID)
        End If
        strExportDataString.Append(",")
        strExportDataString.Append(strPassword)
        strExportDataString.Append(",")
        strExportDataString.Append(strAppId)
        strExportDataString.Append(",")
        strExportDataString.Append(strAppVersion)
        strExportDataString.Append(",")
        strExportDataString.Append(strMACId)
        strExportDataString.Append(",")
        strExportDataString.Append(strStoreType)
        strExportDataString.Append(",")
        strExportDataString.Append(strIPAdd)
        strExportDataString.Append(",")
        strExportDataString.Append(strFreeMem)
        strExportDataString.Append(Environment.NewLine)

        'Return the SOR record generated.
        GenerateSOR = strExportDataString.ToString()
    End Function
    'v1.1 MCF Change
    '' <summary>
    '' change the IP to alternate controller IP
    '' </summary>
    '' <remarks>none</remarks>
    Public Sub sConnectAlternateInBatch()
        Try
            objAppContainer.iConnectedToAlternate = 1
            If objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString() Then
                objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.SECONDARY_IPADDRESS).ToString()
            Else
                objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString()
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in sConnectAlternateInBatch " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class
