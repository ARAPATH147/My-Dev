Imports MCShMon.FileIO
Imports System.Threading
Imports MCShMon.Message
''' ***************************************************************************
''' <FileName>SMExportDataManager</FileName>
'''  <summary>
'''  The class is responsible for writing export data in to export data file
'''  by the shelf management modules
'''  </summary>
'''  <Author>Infosys Technologies Ltd.,</Author>
'''  <DateModified>22-Dec-2008</DateModified>
'''  <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
'''  <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>           
''' ***************************************************************************
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes for connecting to alternate controller while primary is down.
''' </Summary>
'''****************************************************************************
Public Class SMTransactDataManager
    ' Declaration of private member variable for reading export data file name
    Private strExFileName As String = Nothing
    Private strExFilePath As String = Nothing
    Private strExportFile As String = Nothing
    Private strPLListID As String = Nothing
    Private strCOLListID As String = Nothing
    Private m_TransactDataTransmitter As TransactDataTransmitter = Nothing
#If RF Then
    'Last Active Module Flag set to Distinguish between previous SM/FF/EX stock module
    Private m_LastActiveModule As AppContainer.ACTIVEMODULE
    Private m_ConnectionLostExit As Boolean = False
    Private m_BufferData As Object = Nothing
    Private m_IsEndRecordNeeded As Boolean = True
    Private m_TimeOutRetrySuccesStrtRcrd As Boolean = False
#End If

    'This Variable is used to hold the send Request in case of Multiple Response/Error Scenario's
    Private m_PreviousRequest As String = Nothing
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks>Intialize the export file path</remarks>
    Sub New()
        'Read the Export Data File path
        strExFilePath = ConfigDataMgr.GetInstance.GetParam(ConfigKey.EXPORT_FILE_PATH)

        ' Read the Export Data File name for actual export data
        strExFileName = GetFileName(ExFileType.EXData)

        'Preapare the full export data path appended with file name
        strExportFile = strExFilePath + strExFileName

        m_TransactDataTransmitter = New TransactDataTransmitter()
    End Sub
    Public Sub EndSession()
        If Not m_TransactDataTransmitter Is Nothing Then
            m_TransactDataTransmitter.EndSession()
            m_TransactDataTransmitter = Nothing
        End If
    End Sub
    ''' <summary>
    ''' To get the option to which file the data has to be written.
    ''' </summary>
    ''' <remarks>Sets the enum for Temporary files</remarks>
    Public Enum ExFileType
        PLTemp
        CLTemp
        EXData
    End Enum
    ''' <summary>
    ''' Returns the export data file name to which records are written.
    ''' </summary>
    ''' <param name="eFileType"></param>
    ''' <remarks>Sets the temp file name</remarks>
    Private Function GetFileName(ByVal eFileType As ExFileType) As String
        Select Case eFileType
            Case ExFileType.CLTemp
                ' Read the Export Data File name for count list
                Return Macros.CL_EX_FILE_NAME
            Case ExFileType.PLTemp
                ' Read the Export Data File name for picking list
                Return Macros.PL_EX_FILE_NAME
            Case ExFileType.EXData
                ' Read the Export Data File name for actual export data
                Return Macros.EXPORT_FILE_NAME
            Case Else
                ' Read the Export Data File name for actual export data
                Return Macros.EXPORT_FILE_NAME
        End Select
    End Function
    ''' <summary>
    ''' To reset export data file name to original after writing data for PL and CL.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ResetExFileName()
        ' Read the Export Data File name for actual export data
        strExFileName = GetFileName(ExFileType.EXData)

        'Preapare the full export data path appended with file name
        strExportFile = strExFilePath + strExFileName
    End Sub
#If RF Then
    Public Function CreateSYSStart() As Boolean
        If m_ConnectionLostExit Then
            objAppContainer.objLogger.WriteAppLog("Auto Stuff Your shelves: Connection not present and establishing " + _
                                                  "connection before session start", Logger.LogLevel.RELEASE)
            Reconnect(CurrentOperation.MODULE_START_RECORD)
        End If
        Return True
    End Function
#End If

#If RF Then
    ''' <summary>
    ''' The function creates then transact message for CLF(Count List Finished)
    ''' </summary>
    ''' <param name="AlreadyInReconnect">Whether End session is called from Recoonect</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateCLF(Optional ByVal AlreadyInReconnect As CurrentOperation = CurrentOperation.LIST_FINISH_xLF)
#ElseIf NRF Then
        ''' <summary>
    ''' The function creates then transact message for CLF(Count List Finished)
    ''' </summary>
    ''' <param name="eExFile"></param>
    '''<remarks>Sets the file type in which record has to be written</remarks>
    Public Function CreateCLF(Optional ByVal eExFile As ExFileType = ExFileType.CLTemp)
#End If

        'Local variable declaration 
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()

        ' Appending the details to the string 
        strExportDataString.Append("CLF")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)

#If NRF Then
         strExFileName = GetFileName(eExFile)
        'Preapare the full export data path appended with file name
        strExportFile = strExFilePath + strExFileName

     strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: CLF record write success", _
                                                  Logger.LogLevel.DEBUG)
            'Reset export data file name
            ResetExFileName()
        Catch ex As Exception
                   objAppContainer.objLogger.WriteAppLog("SMExportDataManager: CLF record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        'm_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())
        DATAPOOL.getInstance.ResetPoolData()
        If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), AlreadyInReconnect) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        bTemp = False
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            If AlreadyInReconnect <> CurrentOperation.SENDING_END_MESSAGE_AFTER_RECONNECT Then
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(AlreadyInReconnect)
                'End If
            End If

            bTemp = False
        End If
#End If
        Return bTemp
    End Function
#If NRF Then
    ''' <summary>
    ''' The function creates then transact message for CLO(Count List Start)
    ''' </summary>
    ''' <param name="eExFile"></param>
    ''' <remarks>Sets the file type in which record has to be written</remarks>
    Public Function CreateCLO(ByVal eExFile As ExFileType)
        ' Declaration of local variable
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()

        strExFileName = GetFileName(eExFile)
        'Preapare the full export data path appended with file name
        strExportFile = strExFilePath + strExFileName

        ' Appending the details to the  string 
        strExportDataString.Append("CLO")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(Environment.NewLine)

        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                                  strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: CLO record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: CLO record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try

        Return bTemp
    End Function
#End If

#If RF Then
    Public Function CreateCLO() As Boolean
        Dim bTemp As Boolean = False
        Try
            If m_ConnectionLostExit Then
                objAppContainer.objLogger.WriteAppLog("CLO : Connection not present and establishing connection " + _
                                                  "before session start", Logger.LogLevel.RELEASE)
                Reconnect(CurrentOperation.MODULE_START_RECORD)
            End If
            Dim strExportDataString As New System.Text.StringBuilder()
            ' Appending the details to the  string 
            strExportDataString.Append("CLO")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            'strExportDataString.Append(Environment.NewLine)
            DATAPOOL.getInstance.ResetPoolData()
            'bTemp = m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
                If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.MODULE_START_RECORD) Then
                    Dim objResponse As Object = Nothing
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is ACKRecord Then
                            bTemp = True
                        ElseIf TypeOf (objResponse) Is NAKRecord Then
                            bTemp = False
                        End If
                    End If
                    objResponse = Nothing
                End If
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(CurrentOperation.MODULE_START_RECORD)
                'End If
                bTemp = False
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Error in Creating CLO ::" + ex.Message, Logger.LogLevel.INFO)
        End Try
        Return bTemp
    End Function
#End If
 
#If RF Then
    Public Function CreateCLA(ByVal strStatus As String, Optional ByVal strListId As String = Nothing) As Boolean

        ' Declaration of local variable
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()
        If m_ConnectionLostExit Then
            objAppContainer.objLogger.WriteAppLog("CLA : Connection not present and establishing connection " + _
                                                  "before session start", Logger.LogLevel.RELEASE)
            Reconnect(CurrentOperation.MODULE_START_RECORD)
        End If

        If strListId = Nothing Then
            strListId = "   "
        End If
        ' Appending the details to the  string 
        strExportDataString.Append("CLA")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        strExportDataString.Append(strListId)
        strExportDataString.Append(",")
        strExportDataString.Append(strStatus)
        strExportDataString.Replace(",", "")
        m_PreviousRequest = strExportDataString.ToString()
        DATAPOOL.getInstance.ResetPoolData()
        If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
            bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.MODULE_START_RECORD)
        Else
            Reconnect(CurrentOperation.MODULE_START_RECORD)
            bTemp = False
        End If
        Return bTemp
    End Function
#End If
#If NRF Then
    Public Function CreateCLA(ByVal eExFile As ExFileType, ByVal strStatus As String) As Boolean

        ' Declaration of local variable
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()
        'strCOLListID = ConfigDataMgr.GetInstance.GetParam( _
        '                                       ConfigKey.CREATE_LIST_ID)
        'ConfigDataMgr.GetInstance.SetParam(ConfigKey.CREATE_LIST_ID, _
        '                                   (Val(strCOLListID) + 1).ToString())
        If strStatus.Equals("S") Then
            strCOLListID = "   "
        ElseIf strStatus.Equals("X") Then
            strCOLListID = ConfigDataMgr.GetInstance.GetParam( _
                                             ConfigKey.CREATE_LIST_ID)
            ConfigDataMgr.GetInstance.SetParam(ConfigKey.CREATE_LIST_ID, _
                                               (Val(strCOLListID) + 1).ToString())
        End If
       
        ' Appending the details to the  string 
        strExportDataString.Append("CLA")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        strExportDataString.Append("   ")
        strExportDataString.Append(",")
        strExportDataString.Append(strStatus)
        strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: CLA record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: CLA record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
#End If

    ''' <summary>
    ''' The function creates then transact message for CLD(Create List Item)
    ''' </summary>
    ''' <remarks></remarks>

    Public Function CreateCLD(ByVal objCLD As CLDRecord) As Boolean
        ' Declaration of local variable
        Dim bTemp As Boolean = False
        Dim strBootsCode As String = Nothing
        Dim strSequence As String = Nothing
#If RF Then
        If m_ConnectionLostExit Then
            objAppContainer.objLogger.WriteAppLog("CLD : Connection not present and establishing connection " + _
                                                  "before session start", Logger.LogLevel.RELEASE)
            Reconnect(CurrentOperation.MODULE_START_RECORD)
        End If

#End If

        Dim strExportDataString As New System.Text.StringBuilder()


        'Check for object which have not been intialized,justify it
        'If objCLD.strListID = String.Empty Then
        '    objCLD.strListID = ""
        '    strListID = objCLD.strListID.PadLeft(3, "0")
        'End If
        'If objCLD.strSequence = String.Empty Then
        '    objCLD.strSequence = ""
        '    strSequence = objCLD.strSequence.PadLeft(3, "0")
        'End If
        'If objCLD.strBootsCode = String.Empty Then
        '    objCLD.strBootsCode = ""
        '    strBootsCode = objCLD.strBootsCode.PadLeft(13, "0")
        'End If


        'Right justify zero filled
        'If objCLD.strListID <> "" Then
        '    strListID = objCLD.strListID.PadLeft(3, "0")
        'End If
        If objCLD.strSequence <> "" Then
            strSequence = objCLD.strSequence.PadLeft(3, "0")
        End If
        If objCLD.strBootsCode <> "" Then
            strBootsCode = objCLD.strBootsCode.PadLeft(13, "0")
        End If

        If strCOLListID <> "" Then
            strCOLListID = strCOLListID.PadLeft(3, "0")
        End If
        ' Appending the details to the  string 
        strExportDataString.Append("CLD")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
#If RF Then
        strExportDataString.Append(objCLD.strListID)
#Else
        strExportDataString.Append(strCOLListID)
#End If
        strExportDataString.Append(",")
        strExportDataString.Append(strSequence)
        strExportDataString.Append(",")
        strExportDataString.Append(objCLD.cSitetype)
        strExportDataString.Append(",")
        strExportDataString.Append(strBootsCode)
#If NRF Then
        strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: CLD record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: CLD record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        DATAPOOL.getInstance.ResetPoolData()
        If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
            bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.MODULE_START_RECORD)
        Else
            '            If Not DATAPOOL.getInstance.isConnected Then
            Reconnect(CurrentOperation.MODULE_START_RECORD)
            bTemp = False
            '            End If
        End If
#End If
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates then transact message for GAS(Shelf Monitor Start)
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CreateGAS() As Boolean
        ' Declaration of local variable
        Dim bTemp As Boolean = False
#If NRF Then
         Dim strExportDataString As New System.Text.StringBuilder()
        'Get the list ID to be used for the GAP record.
        strPLListID = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.LIST_ID)
        ConfigDataMgr.GetInstance.SetParam(ConfigKey.LIST_ID, _
                                           (Val(strPLListID) + 1).ToString())
        ' Appending the details to the  string 
        strExportDataString.Append("GAS")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: GAS record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: GAS record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        'if the connection lost and the user tries to access the same module 
        'he has to be established to the previous session
        If m_ConnectionLostExit Then
            Dim bJoiningPreviousSession As Boolean = False
            objAppContainer.objLogger.WriteAppLog("There was a connection lost in previous session", Logger.LogLevel.RELEASE)
            If Not strPLListID Is Nothing Then
                If (strPLListID.Trim("0") <> "") And _
                               (m_LastActiveModule = objAppContainer.objActiveModule) Then
                    objAppContainer.objLogger.WriteAppLog("Have to connect to previous session", Logger.LogLevel.RELEASE)
                    bJoiningPreviousSession = True
                End If
            End If
            'Try Reconnecting. No need to send a GAS when connection is lost
            If Reconnect(CurrentOperation.MODULE_START_RECORD) And bJoiningPreviousSession Then
                objAppContainer.objLogger.WriteAppLog("GAS : Connection not present and establishing connection " + _
                                                   "joining previous session", Logger.LogLevel.RELEASE)
                Return True
            Else
                objAppContainer.objLogger.WriteAppLog("Not Joining Previous Session, Starting a new session. Sending GAS", Logger.LogLevel.RELEASE)
            End If
        ElseIf (Not m_TransactDataTransmitter.ConnectionStatus) Then
            objAppContainer.objLogger.WriteAppLog("Connection Lost and trying to connect before restarting the module", Logger.LogLevel.RELEASE)
            strPLListID = "000"
            Reconnect(CurrentOperation.MODULE_START_RECORD)
        End If
        Dim strExportDataString As New System.Text.StringBuilder()
#If NRF Then
        'Get the list ID to be used for the GAP record.
        strPLListID = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.LIST_ID)
        ConfigDataMgr.GetInstance.SetParam(ConfigKey.LIST_ID, _
                                           (Val(strPLListID) + 1).ToString())
#End If
        ' Appending the details to the  string 
        strExportDataString.Append("GAS")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Replace(",", "")
        DATAPOOL.getInstance.ResetPoolData()
        'Setting the Previous Active Module Falg
        If (m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.MODULE_START_RECORD) Then
                Dim Returnobject As Object = Nothing
                DATAPOOL.getInstance.GetNextObject(Returnobject)
                If TypeOf (Returnobject) Is GARRecord Then
                    bTemp = True
                    strPLListID = CType(Returnobject, GARRecord).strListID
                ElseIf TypeOf (Returnobject) Is NAKRecord Then
                    bTemp = False
                End If
                Returnobject = Nothing
            End If
        Else
            'If Not DATAPOOL.getInstance.isConnected Then
            Reconnect(CurrentOperation.MODULE_START_RECORD)
            'End If
            bTemp = False
        End If
#End If
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates then transact message for PCS(Price Check Start)
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CreatePCS() As Boolean
        ' Declaration of local variable
        Dim bTemp As Boolean = False
#If RF Then
        If m_ConnectionLostExit Then
            objAppContainer.objLogger.WriteAppLog("PCS : Connection not present and establishing connection " + _
                                                  "before session start", Logger.LogLevel.RELEASE)
            Reconnect(CurrentOperation.MODULE_START_RECORD)
        End If
#End If
        Dim strExportDataString As New System.Text.StringBuilder()

        ' Appending the details to the  string 
        strExportDataString.Append("PCS")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)

#If NRF Then
         strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PCS record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PCS record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        DATAPOOL.getInstance.ResetPoolData()
        If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
            bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.MODULE_START_RECORD)
        Else
            '            If Not DATAPOOL.getInstance.isConnected Then
            Reconnect(CurrentOperation.MODULE_START_RECORD)
            bTemp = False
            '            End If
        End If
#End If
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates then transact message for PCX(Price Check Exit)
    ''' </summary>
    ''' <param name="objPCX"></param>
    ''' <remarks>object of record PCX</remarks>
    Public Function CreatePCX(ByRef objPCX As PCXRecord) As Boolean
        ' Declaration of local variable
        Dim bTemp As Boolean = False
        Dim strCheckItem As String = Nothing
        Dim strSEL As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()

        'Check for object which have not been intialized,justify it
        If objPCX.strCheckedItems = String.Empty Then
            objPCX.strCheckedItems = ""
            strCheckItem = objPCX.strCheckedItems.PadLeft(4, "0")
        End If
        If objPCX.strSELs = String.Empty Then
            objPCX.strSELs = ""
            strSEL = objPCX.strSELs.PadLeft(4, "0")
        End If
        'Right justify, zero filled
        If objPCX.strCheckedItems <> "" Then
            strCheckItem = objPCX.strCheckedItems.PadLeft(4, "0")
        End If
        If objPCX.strSELs <> "" Then
            strSEL = objPCX.strSELs.PadLeft(4, "0")
        End If

        ' Appending the details to the  string 
        strExportDataString.Append("PCX")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        strExportDataString.Append(strCheckItem)
        strExportDataString.Append(",")
        strExportDataString.Append(strSEL)

#If NRF Then
        strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PCX record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
             'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PCX record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        DATAPOOL.getInstance.ResetPoolData()
        If (m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification()) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        bTemp = False
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            'If Not DATAPOOL.getInstance.isConnected Then
            Reconnect()
            'End If
            bTemp = False
        End If
#End If
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates then transact message for PLO(Picking List Start)
    ''' </summary>
    ''' <param name="eExFile"></param>
    '''  <remarks>Sets the file type in which record has to be written</remarks>
    Public Function CreatePLO(Optional ByVal eExFile As ExFileType = ExFileType.PLTemp)
        ' Declaration of local variable
        Dim bTemp As Boolean = False
#If RF Then
        If m_ConnectionLostExit Then
            objAppContainer.objLogger.WriteAppLog("PLO : Connection not present and establishing connection " + _
                                                  "before session start", Logger.LogLevel.RELEASE)
            If Reconnect(CurrentOperation.MODULE_START_RECORD) AndAlso (m_LastActiveModule = objAppContainer.objActiveModule) Then
                'Joining Previous session
                objAppContainer.objLogger.WriteAppLog("Joining Previous Session", Logger.LogLevel.RELEASE)
            End If
        End If
#End If
        Dim strExportDataString As New System.Text.StringBuilder()

        'Get the export data file name to which the record has to be written
        strExFileName = GetFileName(eExFile)
        'Preapare the full export data path appended with file name
        strExportFile = strExFilePath + strExFileName

        ' Appending the details to the  string 
        strExportDataString.Append("PLO")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)

#If NRF Then
       strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PLO record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PLO record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        'bTemp = m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())
        DATAPOOL.getInstance.ResetPoolData()
        If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.MODULE_START_RECORD) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        bTemp = False
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            'If Not DATAPOOL.getInstance.isConnected Then
            Reconnect(CurrentOperation.MODULE_START_RECORD)
            bTemp = False
            'End If
        End If
#End If
        Return bTemp
    End Function
#If RF Then
    Public Function CreatePLF(Optional ByVal AlreadyInReconnect As CurrentOperation = CurrentOperation.LIST_FINISH_xLF)
#ElseIf NRF Then
    ''' <summary>
    ''' The function creates then transact message for PLF
    ''' (PickingLists Finished)
    ''' </summary>
    ''' <param name="eExFile"></param>
    ''' <remarks >Sets the file type in which record has to be written</remarks>
    Public Function CreatePLF(Optional ByVal eExFile As ExFileType = ExFileType.PLTemp)
#End If
        ' Declaration of local variable
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()
#If NRF Then
 'Get the export data filename to which the record has to be written.
        strExFileName = GetFileName(eExFile)
        'Preapare the full export data path appended with file name
        strExportFile = strExFilePath + strExFileName
#End If


        ' Appending the details to the  string 
        strExportDataString.Append("PLF")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)

#If NRF Then
            strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PLF record write success", _
                                                  Logger.LogLevel.DEBUG)
            'Reset export data file name
            ResetExFileName()
        Catch ex As Exception
              'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PLF record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        'm_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())
        DATAPOOL.getInstance.ResetPoolData()
        If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), AlreadyInReconnect) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        bTemp = False
                    ElseIf TypeOf (objResponse) Is NAKERRORRecord Then
                        bTemp = True
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            If AlreadyInReconnect <> CurrentOperation.SENDING_END_MESSAGE_AFTER_RECONNECT Then
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(AlreadyInReconnect)
                'End If
                bTemp = False
            End If
        End If
#End If
        Return bTemp
    End Function

    ''' <summary>
    ''' The function creates the transact message for CLC
    ''' (Count List Item Update/Confirm)
    ''' </summary>
    ''' <param name="objCLC"></param>
    ''' <param name="eExFile"></param>
    ''' <remarks >object of the record to be written and sets the file type</remarks>
    Public Function CreateCLC(ByRef objCLC As CLCRecord, Optional ByVal eExFile As ExFileType = ExFileType.CLTemp)
        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strListID As String = Nothing
        Dim strSeqeuencenum As String = Nothing
        Dim strCount As String = Nothing
        Dim strCountLocation As String = Nothing
        Dim strSiteID As String = Nothing
        Dim strSalesAtTimeOfUpload As String = Nothing

        Dim strExportDataString As New System.Text.StringBuilder()

        'Get the file name to which the export data has to be written
        strExFileName = GetFileName(eExFile)
        'Preapare the full export data path appended with file name
        strExportFile = strExFilePath + strExFileName

        'Check for object which have not been intialized,justify it
        'Added as part of SFA - If empty assuming as create own list
        If objCLC.strListID = String.Empty Then
            objCLC.strListID = strCOLListID
            'strListID = objCLC.strListID.PadLeft(3, "0")
        End If
        If objCLC.strNumberSEQ = String.Empty Then
            objCLC.strNumberSEQ = ""
            strSeqeuencenum = objCLC.strNumberSEQ.PadLeft(3, "0")
        End If
        If objCLC.strCountLocation = String.Empty Then
            objCLC.strCountLocation = ""
            strCountLocation = objCLC.strCountLocation.PadLeft(1, "0")
        End If


        'Right justify zero filled
        If objCLC.strListID <> "" Then
            strListID = objCLC.strListID.PadLeft(3, "0")
        End If
        If objCLC.strNumberSEQ <> "" Then
            strSeqeuencenum = objCLC.strNumberSEQ.PadLeft(3, "0")
        End If
        If objCLC.strCount <> "" Then
            strCount = objCLC.strCount.PadLeft(4, "0")
        End If

        If objCLC.strSalesAtTimeOfUpload <> "" Then
            strSalesAtTimeOfUpload = objCLC.strSalesAtTimeOfUpload.PadLeft(9, "0")
        End If

        'Appending the details to the string 
        strExportDataString.Append("CLC")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        strExportDataString.Append(strListID)
        strExportDataString.Append(",")
        strExportDataString.Append(strSeqeuencenum)
        strExportDataString.Append(",")
        strExportDataString.Append(objCLC.strBootscode)
        strExportDataString.Append(",")
        strExportDataString.Append(objCLC.strCountLocation)
        strExportDataString.Append(",")
        strExportDataString.Append(strCount)

#If NRF Then
        strExportDataString.Append(",")
        strExportDataString.Append(objCLC.strUpdateOSSR)
        strExportDataString.Append(",")
        strExportDataString.Append(strSalesAtTimeOfUpload) '@SFA
        strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: CLC record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: CLC record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Append(",")
        strExportDataString.Append(objCLC.strUpdateOSSR)
        strExportDataString.Append(",")
        strExportDataString.Append("XXXXXXXXX")
        strExportDataString.Replace(",", "")
        'bTemp = m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())
        DATAPOOL.getInstance.ResetPoolData()
        If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification()) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        bTemp = False
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            'If Not DATAPOOL.getInstance.isConnected Then
            Reconnect()
            'End If
            bTemp = False
        End If
#End If
        Return bTemp
    End Function

#If NRF Then
 ''' <summary>
    '''  The function creates the transact message for CLX(Count List Exit)
    ''' </summary>
    ''' <param name="objCLX"></param>
    ''' <param name="eExFile"></param>
    ''' <remarks >Structure object containing data for CLX record</remarks>
    Public Function CreateCLX(ByRef objCLX As CLXRecord, Optional ByVal eExFile As ExFileType = ExFileType.CLTemp)
#ElseIf RF Then
    Public Function CreateCLX(ByRef objCLX As CLXRecord, Optional ByVal AlreadyInReconnect As CurrentOperation = CurrentOperation.LIST_EXIT_xLX)
#End If

        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strListID As String = Nothing
        Dim strIsCommit As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()

#If NRF Then
 'Get the file name to which the export data record has to be written.
        strExFileName = GetFileName(eExFile)
        'Preapare the full export data path appended with file name
        strExportFile = strExFilePath + strExFileName
#End If


        'Check for object which have not been intialized,justify it
        'If objCLX.strListID = String.Empty Then
        '    objCLX.strListID = ""
        '    strListID = objCLX.strListID.PadLeft(3, "0")
        '    strListID = strCOLListID
        'End If
        If objCLX.cIsCommit = String.Empty Or objCLX.cIsCommit = "" Then
            objCLX.cIsCommit = "0"
        End If
        'Right Justify zero filled
        If objCLX.strListID <> "" Then
            strListID = objCLX.strListID.PadLeft(3, "0")
        End If

#If NRF Then
        'Assuming blank list id for create own list
        If objCLX.strListID = String.Empty Then
            strListID = strCOLListID.PadLeft(3, "0")
            ConfigDataMgr.GetInstance.SetParam(ConfigKey.CREATE_LIST_ID, _
                                                       (Val(strCOLListID) + 1).ToString())
        End If
#End If
        'Appending the details to the string 
        strExportDataString.Append("CLX")
        strExportDataString.Append(",")
        strExportDataString.Append(strListID)
        strExportDataString.Append(",")
        strExportDataString.Append(objCLX.cIsCommit)
        strExportDataString.Append(",")
        strExportDataString.Append(objCLX.strCountType)     'Added as per SFA

#If NRF Then
        strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: CLX record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: CLX record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        DATAPOOL.getInstance.ResetPoolData()
        'bTemp = m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())
        If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), AlreadyInReconnect) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        MessageBox.Show("Received error from controller " + _
                                        CType(objResponse, NAKRecord).strErrorMessage, _
                                        "Error")
                        bTemp = False
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            If AlreadyInReconnect <> CurrentOperation.SENDING_END_MESSAGE_AFTER_RECONNECT Then
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(AlreadyInReconnect)
                'End If
            End If

            bTemp = False
        End If
#End If
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates the transact message for GAP(Picking List Add/Replace)
    ''' </summary>
    ''' <param name="objGAP"></param>
    ''' <remarks >object of the record to be written</remarks>
    Public Function CreateGAP(ByRef objGAP As GAPRecord) As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strListID As String = Nothing
        Dim strSequencenum As String = Nothing
        Dim strBarcode As String = Nothing
        Dim strCurrentQty As String = Nothing
        Dim strFillQty As String = Nothing
        Dim strStockFig As String = Nothing
        Dim strLocCounted As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()

        'Check for object which have not been intialized,justify it
        If objGAP.strNumberSEQ = String.Empty Or objGAP.strBarcode = String.Empty Or _
        objGAP.cIsGAPFlag = String.Empty Or objGAP.cIsGAPFlag = "" Then
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GAP:Mandatory values not present", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End If
#If NRF Then
        'Create ENQ record. sample: ENQ111I 0000001616501N
        strExportDataString.Append("ENQ," & objAppContainer.strCurrentUserID _
                                   & ",I, ," & objGAP.strBootscode.PadLeft(13, "0") _
                                   & ",Y, ,N")
        strExportDataString.Append(Environment.NewLine)
#End If

        If objGAP.strCurrentQty = String.Empty Then
            objGAP.strCurrentQty = ""
            strCurrentQty = objGAP.strCurrentQty.PadLeft(4, "0")
        End If
        If objGAP.strFillQty = String.Empty Then
            objGAP.strFillQty = ""
            strFillQty = objGAP.strFillQty.PadLeft(4, "0")
        End If
        If objGAP.strStockFig = String.Empty Then
            objGAP.strStockFig = ""
            strStockFig = objGAP.strStockFig.PadLeft(5, "0")
            strStockFig = "+" & strStockFig
        End If
        'ambli
        'System Testing fix for GAP for both Btach and RF
        'If objGAP.strStockFig = String.Empty Then
        '    objGAP.strStockFig = ""
        '    strStockFig = objGAP.strStockFig.PadLeft(6, "0")
        'End If
        'Right justify zero filled
        strListID = strPLListID.PadLeft(3, "0")
        strSequencenum = objGAP.strNumberSEQ.PadLeft(3, "0")
        strBarcode = objGAP.strBarcode.PadLeft(13, "0")
        strCurrentQty = objGAP.strCurrentQty.PadLeft(4, "0")
        strFillQty = objGAP.strFillQty.PadLeft(4, "0")
        strLocCounted = objGAP.strLocCounted.PadLeft(2, "0")    'Fix to send "  " if an item is not multisited.
        'ambli
        'System Testing for GAP.No Sign Neds to be send.

        If Not objGAP.strStockFig.StartsWith("-") Then
            strStockFig = objGAP.strStockFig.PadLeft(6, "0")
            'strStockFig = "+" & strStockFig
        Else
            strStockFig = objGAP.strStockFig.PadLeft(6, "0")
            'strStockFig = "-" & strStockFig
        End If

        'Appending the details to the string 
        strExportDataString.Append("GAP")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        strExportDataString.Append(strListID)
        strExportDataString.Append(",")
        strExportDataString.Append(strSequencenum)
        strExportDataString.Append(",")
        strExportDataString.Append(strBarcode)
        strExportDataString.Append(",")
        strExportDataString.Append(objGAP.strBootscode)
        strExportDataString.Append(",")
        strExportDataString.Append(strCurrentQty)
        strExportDataString.Append(",")
        strExportDataString.Append(strFillQty)
        strExportDataString.Append(",")
        strExportDataString.Append(objGAP.cIsGAPFlag)
        strExportDataString.Append(",")
        strExportDataString.Append(strStockFig)
        strExportDataString.Append(",")
        strExportDataString.Append(objGAP.strUpdateOssrItem)
        strExportDataString.Append(",")
        strExportDataString.Append(strLocCounted)


#If NRF Then
        strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: GAP record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: GAP record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
        'Compiler Switch for RF
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        DATAPOOL.getInstance.ResetPoolData()
        If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification()) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        bTemp = False
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            'If Not DATAPOOL.getInstance.isConnected Then
            Reconnect()
            'End If
            bTemp = False
        End If
#End If
        Return bTemp
    End Function
#If RF Then
    ''' <summary>
    ''' The function creates the transact message for GAX(Shelf Monitor Exit)
    ''' </summary>
    ''' <param name="objGAX"></param>
    ''' <remarks >object of the record to be written</remarks>
    Public Function CreateGAX(ByRef objGAX As GAXRecord, Optional ByVal AlreadyInReconnect As CurrentOperation = CurrentOperation.OTHERS) As Boolean
#ElseIf NRF Then
           ''' <summary>
    ''' The function creates the transact message for GAX(Shelf Monitor Exit)
    ''' </summary>
    ''' <param name="objGAX"></param>
    ''' <remarks >object of the record to be written</remarks>
    Public Function CreateGAX(ByRef objGAX As GAXRecord) As Boolean
#End If

        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strListID As String = Nothing
        Dim strPickItem As String = Nothing
        Dim strSELs As String = Nothing
        Dim strPriceCheck As String = Nothing
        Dim strGAPitem As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()

        'Check for object which have not been intialized,justify it
        If objGAX.strPickListItems = String.Empty Or objGAX.strSELS = String.Empty Or _
        objGAX.strPriceChk = String.Empty Then
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("GAX:Mandatory values not present", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End If
        'Right justify zero filled
        strListID = strPLListID.PadLeft(3, "0")
        strPickItem = objGAX.strPickListItems.PadLeft(4, "0")
        strSELs = objGAX.strSELS.PadLeft(4, "0")
        strPriceCheck = objGAX.strPriceChk.PadLeft(4, "0")

        'Appending the details to the string 
        strExportDataString.Append("GAX")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        strExportDataString.Append(strListID)
        strExportDataString.Append(",")
        strExportDataString.Append(strPickItem)
        strExportDataString.Append(",")
        strExportDataString.Append(strSELs)
        strExportDataString.Append(",")
        strExportDataString.Append(strPriceCheck)

#If NRF Then
        strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: GAX record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: GAX record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
        'Compiler Switch for RF
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        'Reset Pool data Before sending a request 
        DATAPOOL.getInstance.ResetPoolData()
        'Send Data
        If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), AlreadyInReconnect) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        'Reset List ID after End of every session
                        strPLListID = "000"
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        bTemp = False
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            If AlreadyInReconnect <> CurrentOperation.SENDING_END_MESSAGE_AFTER_RECONNECT Then
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect()
                'End If
            End If

            bTemp = False
        End If
#End If
        Return bTemp
    End Function
#If NRF Then
    ''' <summary>
    ''' The function creates the transact message for PCM(Price Check Mismatch)
    ''' </summary>
    ''' <param name="objPCM"></param>
    ''' <remarks>object of the record to be written</remarks>
    Public Function CreatePCM(ByRef objPCM As PCMRecord) As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strVariance As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()
        Dim strPriceCheck As String = Nothing
        Dim strPrintType As String = " "
        'Check for object which have not been intialized,justify it
        If objPCM.strNumVariance = String.Empty Then
            objPCM.strNumVariance = ""
            strVariance = objPCM.strNumVariance.PadLeft(6, "0")
        End If
        If objAppContainer.bMobilePrinterAttachedAtSignon Then
            strPrintType = Macros.PRINT_LOCAL
        End If
        'Right justify zero filled
        If objPCM.strNumVariance <> "" Then
            'Check if the num variance is negative 
            If objPCM.strNumVariance < 0 Then
                strVariance = Str(CInt(objPCM.strNumVariance) * (-1))
                strVariance = LTrim(strVariance)
                strVariance = strVariance.PadLeft(5, "0")
                strVariance = strVariance.PadLeft(6, "-")
            Else
                strVariance = objPCM.strNumVariance.PadLeft(6, "0")
            End If
        End If
        strPriceCheck = objPCM.strPriceCheck
        'Create ENQ record. sample: "ENQ111IP0000001616501Y " for Price Check
        strExportDataString.Append("ENQ," & objAppContainer.strCurrentUserID _
                                   & ",I," & strPriceCheck & "," _
                                   & objPCM.strBootscode.PadLeft(13, "0") _
                                   & ",Y, ,N")
        strExportDataString.Append(Environment.NewLine)

        'Appending the details to the string 
        strExportDataString.Append("PCM")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        strExportDataString.Append(objPCM.strBootscode)
        strExportDataString.Append(",")
        strExportDataString.Append(strVariance)
        strExportDataString.Append(",")
        strExportDataString.Append(strPrintType)
        strExportDataString.Append(Environment.NewLine)

        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PCM record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PCM record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try

        Return bTemp
    End Function
#End If
#If RF Then
    Public Function CreatePCM(ByVal BootsCode As String, ByVal iVariance As Decimal)
        Dim bTemp As Boolean = False
        Dim strExportData As New System.Text.StringBuilder()
        Try
            strExportData.Append("PCM")
            strExportData.Append(objAppContainer.strCurrentUserID.PadLeft(3, "0"))
            strExportData.Append(BootsCode.PadLeft(7, "0"))
            If iVariance < 0 Then
                iVariance = iVariance * -1
                strExportData.Append("-" + iVariance.ToString().PadLeft(5, "0"))
            Else
                strExportData.Append(iVariance.ToString().PadLeft(6, "0"))
            End If
            'If MobilePrintSessionManager.GetInstance.MobilePrinterStatus Then
            '    strExportData.Append("L")
            'Else
            '    strExportData.Append(" ")
            'End If
            If objAppContainer.bMobilePrinterAttachedAtSignon Then
                strExportData.Append("L")
            Else
                strExportData.Append(" ")
            End If
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportData.ToString()) Then
                If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification()) Then
                    Dim objResponse As Object = Nothing
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is ACKRecord Then
                            bTemp = True
                        ElseIf TypeOf (objResponse) Is NAKRecord Then
                            bTemp = False
                        End If
                        objResponse = Nothing
                    End If
                End If
            Else
                If Not DATAPOOL.getInstance.isConnected Then
                    Reconnect()
                    bTemp = False
                End If
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
        End Try
        Return bTemp
    End Function
#End If

    'UAT - Creating ENQ for Price Check
    ''' <summary>
    ''' The function creates the transact message for ENQ(Price Check)
    ''' </summary>
    ''' <param name="objENQ"></param>
    ''' <remarks >object of the record to be written</remarks>
    Public Function CreateENQ(ByRef objENQ As ENQRecord) As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean = False
        Try
            Dim strBootsCode As String = Nothing
            Dim strPriceCheck As String = Nothing

            Dim strExportDataString As New System.Text.StringBuilder()


            'Right justify zero filled
            strPriceCheck = objENQ.strPriceCheck
            strBootsCode = objENQ.strBootsCode.PadLeft(13, "0")

            'Appending the details to the string 
            strExportDataString.Append("ENQ,")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            strExportDataString.Append(",I,")
            strExportDataString.Append(strPriceCheck)
            strExportDataString.Append(",")
            strExportDataString.Append(strBootsCode)
            strExportDataString.Append(",Y, ,N")

#If NRF Then
        strExportDataString.Append(Environment.NewLine)
            Try
                'Writing the received data into the export data file
                bTemp = WriteDataIntoFile(strExportFile, _
                                          strExportDataString.ToString(), True)
                'Write to log file
                objAppContainer.objLogger.WriteAppLog("SMExportDataManager: ENQ record write success", _
                                                      Logger.LogLevel.DEBUG)
            Catch ex As Exception
                 'Write to log file
                objAppContainer.objLogger.WriteAppLog("SMExportDataManager: ENQ record write failure", _
                                                      Logger.LogLevel.RELEASE)
                Return False
            End Try
'#ElseIf RF Then
'            strExportDataString.Replace(",", "")
'            DATAPOOL.getInstance.ResetPoolData()
'            Dim objResponse As Object = Nothing
'            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
'                If DATAPOOL.getInstance.GetNextObject(objResponse) Then

'                End If
'            End If
#End If
            Return bTemp
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception Occured while Sending an ENQ. Message: " + ex.Message, Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
#If RF Then
    Public Function CreateENQ(ByVal strBarcode As String, ByVal isPriceCheck As Boolean) As Boolean
        Dim bTemp As Boolean
        Try
            Dim strData As New System.Text.StringBuilder
            'Record Name
            strData.Append("ENQ")
            strData.Append(objAppContainer.strCurrentUserID)
            'I- for Item and p for parent
            strData.Append("I")
            'is the enquiry for price check or Fast fill or otherwise
            If isPriceCheck Then
                If objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.SHLFMNTR Then
                    strData.Append("C")
                ElseIf objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.CUNTLIST Then
                    strData.Append("C")
                ElseIf objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PRICECHK Then
                    strData.Append("P")
                Else
                    strData.Append(" ")
                End If
            Else
                strData.Append(" ")
            End If
            'The barcode or Boots code of the item
            strData.Append(strBarcode.PadLeft(13, "0"))
            strData.Append("Y")
            'Fix for Sending OSSR Flag to " " during a normal scan
            strData.Append(" ")
            'Planner Look up
            strData.Append("N")

            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strData.ToString()) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification())
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect()
                'End If
                bTemp = False
            End If
            Return bTemp
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception Occured while Sending an ENQ. Message: " + ex.Message, Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function

    Public Function CreateENQ_ToggleOSSR(ByVal strBarcode As String, ByVal isOSSR As Boolean) As Boolean
        Dim bTemp As Boolean = False
        Dim strData As New System.Text.StringBuilder
        'Record Names
        strData.Append("ENQ")
        strData.Append(objAppContainer.strCurrentUserID)
        'I- for Item and p for parent
        strData.Append("I")
        'is the enquiry for price check or Fast fill or otherwise
        strData.Append(" ")
        'The barcode or Boots code of the item
        strData.Append(strBarcode.PadLeft(13, "0"))
        strData.Append("Y")
        'Toggle OSSR
        If isOSSR Then
            strData.Append("O")
        Else
            strData.Append("N")
        End If

        'Planner Look up
        strData.Append("N")
        DATAPOOL.getInstance.ResetPoolData()
        If m_TransactDataTransmitter.SendRecordRF(strData.ToString()) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification()) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is EQRRecord Then
                        bTemp = True
                    Else
                        bTemp = False
                    End If
                End If
            End If
        Else
            'If Not DATAPOOL.getInstance.isConnected Then
            Reconnect()
            'End If
            bTemp = False
        End If
        Return bTemp
    End Function
#End If
    ''' <summary>
    ''' The function creates the transact message for PLC
    ''' (Picking List Item Update/Confirm)
    ''' </summary>
    ''' <param name="objPLC"></param>
    ''' <param name="eExFile"></param>
    ''' <remarks>Structure object containing data for PLC record</remarks>
    Public Function CreatePLC(ByRef objPLC As PLCRecord, Optional ByVal eExFile As ExFileType = ExFileType.PLTemp)
        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strListID As String = Nothing
        Dim strSequencenum As String = Nothing
        Dim strStockCount As String = Nothing
        Dim strOSSRCount As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()
#If NRF Then
        'Moved this to NRF because this part of code is not applicable in rf
      'Get the export data file name to which the record has to be writen.
        strExFileName = GetFileName(eExFile)
        'Preapare the full export data path appended with file name
        strExportFile = strExFilePath + strExFileName
#End If


        'Check for object which have not been intialized,justify it
        If objPLC.strListID = String.Empty Then
            objPLC.strListID = ""
            strListID = objPLC.strListID.PadLeft(3, "0")
        End If
        If objPLC.strNumberSEQ = String.Empty Then
            objPLC.strNumberSEQ = ""
            strSequencenum = objPLC.strNumberSEQ.PadLeft(3, "0")
        End If
        If objPLC.strStockCount = String.Empty Then
            objPLC.strStockCount = ""
            strStockCount = objPLC.strStockCount.PadLeft(4, "0")
        End If
        If objPLC.strOSSRCount = String.Empty Then
            objPLC.strOSSRCount = ""
            strOSSRCount = objPLC.strOSSRCount.PadLeft(4, "0")
        End If
        'Integration testing - 732
        If objPLC.cIsGAPFlag = "" Then
            objPLC.cIsGAPFlag = "".PadLeft("1", "0")
        End If


        'Right justify, zero filled
        If objPLC.strListID <> "" Then
            strListID = objPLC.strListID.PadLeft(3, "0")
        End If
        If objPLC.strNumberSEQ <> "" Then
            strSequencenum = objPLC.strNumberSEQ.PadLeft(3, "0")
        End If
        If objPLC.strStockCount <> "" Then
            strStockCount = objPLC.strStockCount.PadLeft(4, "0")
        End If

        'Appending the details to the string 
        strExportDataString.Append("PLC")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        strExportDataString.Append(strListID)
        strExportDataString.Append(",")
        strExportDataString.Append(strSequencenum)
        strExportDataString.Append(",")
        strExportDataString.Append(objPLC.strBootscode)
        strExportDataString.Append(",")
        strExportDataString.Append(strStockCount)
        strExportDataString.Append(",")
        strExportDataString.Append(objPLC.cIsGAPFlag)
        strExportDataString.Append(",")
        strExportDataString.Append(objPLC.strPickListLocation)
        strExportDataString.Append(",")
        If objPLC.strOSSRCount <> "" Then
            strOSSRCount = objPLC.strOSSRCount.PadLeft(4, "0")
        End If
        strExportDataString.Append(strOSSRCount)
        strExportDataString.Append(",")
        strExportDataString.Append(objPLC.strUpdateOSSRItem)
        strExportDataString.Append(",")
        strExportDataString.Append(objPLC.strLocationCounted)
        strExportDataString.Append(",")
        strExportDataString.Append(objPLC.strAllMSPicked)

#If NRF Then
        strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PLC record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PLC record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        'bTemp = m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())
        DATAPOOL.getInstance.ResetPoolData()
        If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification()) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        bTemp = False
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            'If Not DATAPOOL.getInstance.isConnected Then
            Reconnect()
            'End If
            bTemp = False
        End If
#End If
        Return bTemp
    End Function
#If RF Then

    ''' <summary>
    ''' PLC function to update stock figure
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <param name="strStockFigure"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreatePLC(ByVal strBootsCode As String, ByVal strStockFigure As String) As Boolean
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()

        'Appending the details to the string 
        strExportDataString.Append("PLC")
        strExportDataString.Append(",")
        'Appending User ID 
        strExportDataString.Append(objAppContainer.strCurrentUserID.PadLeft(3, "0"))
        strExportDataString.Append(",")
        'Appending LIST ID , Sequence Number as zero
        strExportDataString.Append("000000")
        strExportDataString.Append(",")
        'appending Boots Code for the item
        strExportDataString.Append(strBootsCode.PadLeft(7, "0"))
        strExportDataString.Append(",")
        'Appending the Count
        strExportDataString.Append(strStockFigure.PadLeft(4, "0"))
        strExportDataString.Append(",")
        'Appending GAP Falg as "Y"
        strExportDataString.Append("Y")

        strExportDataString.Append(Macros.DEFAULT_VALUE_PLC)
        ' strExportDataString.Append(Environment.NewLine)
        strExportDataString.Replace(",", "")
        'bTemp = m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())
        DATAPOOL.getInstance.ResetPoolData()
        If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.TSF_MODIFICATION) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        MessageBox.Show(CType(objResponse, NAKRecord).strErrorMessage, _
                                        "Count rejected", MessageBoxButtons.OK, _
                                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        bTemp = False
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            'If Not DATAPOOL.getInstance.isConnected Then
            Reconnect(CurrentOperation.TSF_MODIFICATION)
            'End If
            bTemp = False
        End If
        Return bTemp
    End Function
#End If

#If RF Then
    ''' <summary>
    ''' Create PLX Record
    ''' </summary>
    ''' <param name="objPLX"></param>
    ''' <param name="AlreadyinReconnect">Whether this call is from Reconnect</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreatePLX(ByRef objPLX As PLXRecord, Optional ByVal AlreadyinReconnect As CurrentOperation = CurrentOperation.LIST_EXIT_xLX)
#ElseIf NRF Then
    ''' <summary>
    '''  The function creates the transact message for PLX(Picking List Exit)
    ''' </summary>
    ''' <param name="objPLX"></param>
    ''' <param name="eExFile"></param>
    ''' <remarks>Structure object containing data for PLX record</remarks>
     Public Function CreatePLX(ByRef objPLX As PLXRecord,  ByVal eExFile As ExFileType )
#End If
        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strListID As String = Nothing
        Dim strLineActioned As String = Nothing
        Dim strItems As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()
#If NRF Then
    'Get the export data filename to which the record has to be written.
        strExFileName = GetFileName(eExFile)
        'Preapare the full export data path appended with file name
        strExportFile = strExFilePath + strExFileName
#End If
        'Check for object which have not been intialized,justify it
        If objPLX.strListID = String.Empty Then
            objPLX.strListID = ""
            strListID = objPLX.strListID.PadLeft(3, "0")
        End If
        If objPLX.strLineActioned = String.Empty Then
            objPLX.strLineActioned = ""
            strLineActioned = objPLX.strLineActioned.PadLeft(4, "0")
        End If
        If objPLX.strItems = String.Empty Then
            objPLX.strItems = ""
            strItems = objPLX.strItems.PadLeft(6, "0")
        End If
        'integration testing - 732
        If objPLX.cIsComplete = "" Then
            objPLX.cIsComplete = "".PadLeft("1", "0")
        End If
        'Right justify zero filled       
        If objPLX.strListID <> "" Then
            strListID = objPLX.strListID.PadLeft(3, "0")
        End If
        If objPLX.strLineActioned <> "" Then
            strLineActioned = objPLX.strLineActioned.PadLeft(4, "0")
        End If
        If objPLX.strItems <> "" Then
            strItems = objPLX.strItems.PadLeft(6, "0")
        End If

        'Appending the details to the string 
        strExportDataString.Append("PLX")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        strExportDataString.Append(strListID)
        strExportDataString.Append(",")
        strExportDataString.Append(strLineActioned)
        strExportDataString.Append(",")
        strExportDataString.Append(strItems)
        strExportDataString.Append(",")
        strExportDataString.Append(objPLX.cIsComplete)

#If NRF Then
        strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PLX record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PLX record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        'bTemp = m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())
        DATAPOOL.getInstance.ResetPoolData()
        If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), AlreadyinReconnect) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        bTemp = False
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            If AlreadyinReconnect <> CurrentOperation.SENDING_END_MESSAGE_AFTER_RECONNECT Then
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(AlreadyinReconnect)
                'End If
            End If
            bTemp = False
        End If
#End If
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates the transact message for PRT(SEL Print Request)
    ''' </summary>
    ''' <param name="eExFile"></param>
    ''' <remarks>Structure object containing data for PRT record</remarks>
    Public Function CreatePRT(ByVal strBootsCode As String, Optional ByVal eExFile As ExFileType = ExFileType.EXData) As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strBarcode As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()
#If NRF Then
        'Get the export data file name to which the record has to be written
        strExFileName = GetFileName(eExFile)
        'Preapare the full export data path appended with file name
        strExportFile = strExFilePath + strExFileName
#End If
        'Right justify zero filled       
        If strBootsCode <> "" Then
            strBarcode = strBootsCode.PadLeft(13, "0")
        End If

        'Appending the details to the string 
        strExportDataString.Append("PRT")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        strExportDataString.Append(strBarcode)
        strExportDataString.Append(",")
        strExportDataString.Append(objAppContainer.strPrintFlag)
#If NRF Then
        strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PRT record write success", _
                                                  Logger.LogLevel.RELEASE)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PRT record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        Try
            If (m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.PRINT_SELECTION)
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(CurrentOperation.PRINT_SELECTION)
                'End If
                bTemp = False
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: LPR Transmission failure.", _
                                                  Logger.LogLevel.RELEASE)
            bTemp = False
        End Try
#End If
        Return bTemp
    End Function
#If RF Then
    Public Function createPRT(ByVal strProductCode As String, ByVal cPrintFlag As String)
        Dim bTemp As Boolean = False
        Dim strExportData As New System.Text.StringBuilder()
        Try
            strExportData.Append("PRT")
            strExportData.Append(objAppContainer.strCurrentUserID.PadLeft(3, "0"))
            strExportData.Append(strProductCode.PadLeft(13, "0"))
            strExportData.Append(cPrintFlag)
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportData.ToString()) Then
                If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.PRINT_SELECTION) Then
                    Dim objResponse As Object = Nothing
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is ACKRecord Then
                            bTemp = True
                        ElseIf TypeOf (objResponse) Is NAKRecord Then
                            bTemp = False
                        ElseIf TypeOf (objResponse) Is LPRRecord Then
                            '''''''' to govindh

                        End If
                    End If
                    objResponse = Nothing
                End If
            Else
                If Not DATAPOOL.getInstance.isConnected Then
                    Reconnect(CurrentOperation.PRINT_SELECTION)
                End If
                bTemp = False
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
        End Try
        Return bTemp
    End Function
#End If
    ''' <summary>
    ''' The function creates the transact message for PRP
    ''' </summary>
    ''' <param name="objPRP"></param>
    ''' <remarks>object of the record to be written</remarks>
    Public Function CreatePRP(ByRef objPRP As PRPRecord) As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strKeyPOG As String = Nothing
        Dim strTypeMOD As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()

        ''Create PGS record 
        'strExportDataString.Append("PGS,")
        'strExportDataString.Append(objAppContainer.strCurrentUserID)
        'strExportDataString.Append(Environment.NewLine)

        'Check for object which have not been intialized,justify it
        If objPRP.strPOGKey = String.Empty Then
            objPRP.strPOGKey = ""
            strKeyPOG = objPRP.strPOGKey.PadLeft(6, "0")
        End If
        If objPRP.strMODSequence = String.Empty Then
            objPRP.strMODSequence = ""
            strTypeMOD = objPRP.strMODSequence.PadLeft(3, "0")
        End If
        If objPRP.strIsType = "" Or objPRP.strIsType <> "" Then
            objPRP.strIsType = "".PadLeft(1, "0")
        End If
        'Right justify zero filled       
        If objPRP.strPOGKey <> "" Then
            strKeyPOG = objPRP.strPOGKey.PadLeft(6, "0")
        End If
        If objPRP.strMODSequence <> "" Then
            strTypeMOD = objPRP.strMODSequence.PadLeft(3, "0")
        End If

        'Appending the details to the string 
        strExportDataString.Append("PRP")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        strExportDataString.Append(strKeyPOG)
        strExportDataString.Append(",")
        strExportDataString.Append(strTypeMOD)
        strExportDataString.Append(",")
        strExportDataString.Append(objPRP.strIsType)


        ''Create the end record.
        'strExportDataString.Append("PGX,")
        'strExportDataString.Append(objAppContainer.strCurrentUserID)
        'strExportDataString.Append(Environment.NewLine)
#If NRF Then
        strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PRP record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PRP record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        DATAPOOL.getInstance.ResetPoolData()
        If (m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification()) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        bTemp = False
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            'If Not DATAPOOL.getInstance.isConnected Then
            Reconnect(CurrentOperation.PRINT_SELECTION)
            'End If
            bTemp = False
        End If
#End If
        Return bTemp
    End Function
    'Integration Testing
    ''' <summary>
    ''' The function creates then transact message for PGS(Space Planner Start)
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CreatePGS() As Boolean
        ' Declaration of local variable
        Dim bTemp As Boolean = False
#If RF Then
        If m_ConnectionLostExit Then
            objAppContainer.objLogger.WriteAppLog("PGS : Connection not present and establishing connection " + _
                                                  "before session start", Logger.LogLevel.RELEASE)
            Reconnect(CurrentOperation.MODULE_START_RECORD)
        End If
#End If
        Dim strExportDataString As New System.Text.StringBuilder()

        'Create PGS record 
        strExportDataString.Append("PGS,")
        strExportDataString.Append(objAppContainer.strCurrentUserID)

#If NRF Then
                strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PGS record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
             'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PGS record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        DATAPOOL.getInstance.ResetPoolData()
        If (m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.MODULE_START_RECORD) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        bTemp = False
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            'If Not DATAPOOL.getInstance.isConnected Then
            Reconnect(CurrentOperation.MODULE_START_RECORD)
            'End If
            bTemp = False
        End If
#End If
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates then transact message for PGX(Space Planner End)
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CreatePGX() As Boolean
        ' Declaration of local variable
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()

        'Create the end record.
        strExportDataString.Append("PGX,")
        strExportDataString.Append(objAppContainer.strCurrentUserID)

#If NRF Then
        strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PGX record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: PGX record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        DATAPOOL.getInstance.ResetPoolData()
        If (m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification()) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        bTemp = False
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            'If Not DATAPOOL.getInstance.isConnected Then
            Reconnect()
            'End If
            bTemp = False
        End If
#End If
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates the transact message for SOR(Sign On Request)
    ''' </summary>
    ''' <param name="strPassword"></param>
    ''' <remarks>object of the record to be written</remarks>
    Public Function CreateSOR(ByVal strPassword As String) As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean = False
        Dim strExportDataString As String = Nothing
#If NRF Then

        Try
            strExportDataString = GenerateSOR(strPassword)
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)

            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: SOR record write success", _
                                                  Logger.LogLevel.RELEASE)
        Catch ex As Exception
                'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: SOR record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
#ElseIf RF Then
        Try
            strExportDataString = GenerateSOR(strPassword)
            strExportDataString = strExportDataString.Replace(",", "")
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.SIGN_ON)
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(CurrentOperation.SIGN_ON)
                'End If
                Return False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If

            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: SOR record transmission failure", _
                                                  Logger.LogLevel.RELEASE)
            bTemp = False
        End Try
        Return bTemp
#End If
    End Function
    ''' <summary>
    ''' The function creates the transact message for OFF(HHT Sign Off)
    ''' </summary>
    ''' <param name="IsCallForCrash"></param>
    ''' <remarks>If the OFF record is for crash recovery</remarks>
    Public Function CreateOFF(ByVal IsCallForCrash As Boolean) As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean
        Dim strExportDataString As New System.Text.StringBuilder()

        'Write INX before OFF if PRT records are written
        'If (objAppContainer.bIsPRTWritten = True) Then
        '    CreateINX()
        '    objAppContainer.bIsPRTWritten = False
        'End If

        'Appending the details to the string 
        strExportDataString.Append("OFF")
        strExportDataString.Append(",")

        'Check whether the OFF record is for crash recovery or not
        If Not IsCallForCrash Then
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
        Else
            'Take the Operation ID from the Config File
            strExportDataString.Append(ConfigDataMgr.GetInstance.GetParam( _
                                       ConfigKey.PREVIOUS_USER))
        End If

#If NRF Then
        strExportDataString.Append(Environment.NewLine)
        Try   

            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: OFF record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: OFF record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        DATAPOOL.getInstance.ResetPoolData()
        If (m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification()) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        bTemp = False
                    ElseIf TypeOf (objResponse) Is NAKERRORRecord Then
                        'Allow User to log off from the system when NAKERROR is Recieved
                        bTemp = True
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            'If Not DATAPOOL.getInstance.isConnected Then
            Reconnect()
            'End If
            bTemp = False
        End If
#End If
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
        Dim strVersion As String = Nothing
        'Read app version from config file
        'To split the appverion and release version. Actual app version should be send in SOR
        Dim aReleaseVersion() As String = Nothing

        strVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
        aReleaseVersion = strVersion.Split("-")
        strAppVersion = aReleaseVersion(1)
        'strAppVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION)
        strAppId = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_ID)
        strStoreType = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DEVICE_TYPE)
#If NRF Then
        strMACId = objAppContainer.objHelper.GetMacAddress()
        objAppContainer.objLogger.WriteAppLog("SMExportDataManager: Get MAC ID success" + strMACId, _
                                              Logger.LogLevel.RELEASE)
#ElseIf RF Then
        Try
            'Setting Default user ID to MAC address Revoked
            Dim iAttempt As Integer = 0
            Do
                strMACId = objAppContainer.objHelper.GetMacAddress()
                iAttempt = iAttempt + 1
                Threading.Thread.Sleep(1000)
            Loop Until (strMACId <> "000000000000" Or iAttempt = 2)
            'strMACId = objAppContainer.objHelper.GetSerialNumber()
            objAppContainer.strMACADDRESS = strMACId
        Catch ex As Exception
            strMACId = Macros.MACID
            objAppContainer.strMACADDRESS = Macros.MACID
            objAppContainer.objLogger.WriteAppLog("Exception occured While getting MAC ID" + strMACId, _
                                              Logger.LogLevel.RELEASE)
        End Try

#End If

        strFreeMem = objAppContainer.objHelper.CheckForFreeMemory("Program Files", lFreeMem)
        objAppContainer.objLogger.WriteAppLog("SMExportDataManager: Get Free Mem success" + strFreeMem, _
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
#If NRF Then
        ' Take the Operator ID from the Appcontainer
        If strUserID <> "" Then
            strExportDataString.Append(strUserID)
        Else
            strExportDataString.Append(objAppContainer.strCurrentUserID)
        End If
        strExportDataString.Append(",")
#End If
        'anoop
        'In RF the entire user entered text is appended.
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
#If NRF Then
        strExportDataString.Append(Environment.NewLine)
#End If

        'Return the SOR record generated.
        GenerateSOR = strExportDataString.ToString()
    End Function
    ''' <summary>
    ''' This function allows to delete the temporary files created in the 
    ''' application
    ''' </summary>
    ''' <param name="eExFile"></param>
    ''' <remarks>delete the temporary files from the device</remarks>
    Public Function DeleteTempFiles(ByVal eExFile As ExFileType) As Boolean
        'Declare a local variable
        Dim bTemp As Boolean = False
        Dim strTempExFilePath As String = Nothing
        'Read the temporary export file path from config file
        strTempExFilePath = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.TEMP_EXPORT_FILE_PATH)
        Try
            'Get the file name to which the export data has to be written
            strExFileName = GetFileName(eExFile)
            If Not strExFileName = Macros.EXPORT_FILE_NAME Then
                'Delete the file from the specified location
                If IO.File.Exists(strTempExFilePath & strExFileName) Then
                    IO.File.Delete(strTempExFilePath & strExFileName)
                    bTemp = True
                    objAppContainer.objLogger.WriteAppLog("SMExportDataManager: Temporary files deleted", _
                                                          Logger.LogLevel.DEBUG)
                End If
            End If
        Catch ex As Exception
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates the transact message for INS(Item Information Start)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateINS() As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean
#If RF Then
        If m_ConnectionLostExit Then
            objAppContainer.objLogger.WriteAppLog("INS: Connection not present and establishing " + _
                                                  "connection before session start", Logger.LogLevel.RELEASE)
            Reconnect(CurrentOperation.MODULE_START_RECORD)
        End If
#End If
        Dim strExportDataString As New System.Text.StringBuilder()

        'Appending the details to the string 
        strExportDataString.Append("INS")
        strExportDataString.Append(",")

        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)


#If NRF Then
        strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: INS record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: INS record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        DATAPOOL.getInstance.ResetPoolData()
        If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.MODULE_START_RECORD) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        bTemp = False
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            'If Not DATAPOOL.getInstance.isConnected Then
            Reconnect(CurrentOperation.MODULE_START_RECORD)
            'End If
            bTemp = False
        End If
#End If
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates the transact message for INX(Item Information End)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateINX() As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean
        Dim strExportDataString As New System.Text.StringBuilder()

        'Appending the details to the string 
        strExportDataString.Append("INX")
        strExportDataString.Append(",")

        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)


#If NRF Then
        strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: INX record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
                    'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: INX record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
        strExportDataString.Replace(",", "")
        DATAPOOL.getInstance.ResetPoolData()
        If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
            If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification()) Then
                Dim objResponse As Object = Nothing
                If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                    If TypeOf (objResponse) Is ACKRecord Then
                        bTemp = True
                    ElseIf TypeOf (objResponse) Is NAKRecord Then
                        bTemp = False
                    End If
                End If
                objResponse = Nothing
            End If
        Else
            'If Not DATAPOOL.getInstance.isConnected Then
            Reconnect()
            'End If
            bTemp = False
        End If
#End If
        Return bTemp
    End Function
    ''' <summary>
    '''  This Function is sent when a Count List is selected from CL home screen.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateCLS(ByRef objCLS As CLSRecord, Optional ByVal eExFile As ExFileType = ExFileType.CLTemp) As Boolean
        Dim bTemp As Boolean = False
        Dim strListId As String = Nothing
        Dim strSequencenum As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()

        'strListId = objCLS.strListID.PadLeft(3, "0")
        'strSequencenum = objCLS.strSequence.PadLeft(3, "0")
        'strExportDataString.Append("000")
        'strExportDataString.Append("000")

        ' Appending the details to the  string 
        strExportDataString.Append("CLS")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(",")
        strExportDataString.Append(objCLS.strListID.PadLeft(3, "0"))
        strExportDataString.Append(",")
        'SFA ST - Reduced from 4 to 3 asc
        strExportDataString.Append("001")
        'strExportDataString.Append(Environment.NewLine)

#If NRF Then
        strExportDataString.Append(Environment.NewLine)
        Try
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: INX record write success", _
                                                  Logger.LogLevel.DEBUG)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("SMExportDataManager: INX record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
#ElseIf RF Then
            strExportDataString.Replace(",", "")
            m_PreviousRequest = strExportDataString.ToString()
            DATAPOOL.getInstance.ResetPoolData()
            try
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.LIST_INITIALISE)
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(CurrentOperation.LIST_INITIALISE)
                'End If
                bTemp = False
            End If

        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: CLS record write failure", _
                                                         Logger.LogLevel.RELEASE)
            bTemp = False
        End Try
#End If
        Return bTemp
    End Function

#If RF Then
    ''' <summary>
    ''' The function creates the transact message for SSE And Sends it to the controller
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks>start of sales enquiry</remarks>
    Public Function CreateSSE() As Boolean
        ' Local variable declaration
        Dim bTemp As Boolean
        Dim strExportDataString As New System.Text.StringBuilder()

        'Appending the details to the string 
        strExportDataString.Append("SSE")
        strExportDataString.Append(",")

        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID)

        'strExportDataString.Append(Environment.NewLine)


        strExportDataString.Replace(",", "")
        DATAPOOL.getInstance.ResetPoolData()
        If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
            bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification())
        Else
            'If Not DATAPOOL.getInstance.isConnected Then
            Reconnect()
            'End If
            bTemp = False
        End If

        Return bTemp
    End Function

    ''' <summary>
    ''' Request Picking List Details from Controller 
    ''' </summary>
    ''' <returns>boolean</returns>
    ''' <remarks></remarks>
    Public Function CreatePLR() As Boolean
        ' Declaration of local variable
        Dim bTemp As Boolean = False
        Dim strSequencenum As String = Nothing
        Dim cAuthority As Char = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()
        If objAppContainer.strSupervisorFlag.Equals(Macros.SUPERVISOR_YES) Then
            cAuthority = "S"
        Else
            cAuthority = " "
        End If
        ' Appending the details to the  string 
        Try
            strExportDataString.Append("PLR")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            strExportDataString.Append("001")
            strExportDataString.Append(cAuthority)
            m_PreviousRequest = strExportDataString.ToString()
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.LIST_INITIALISE)
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(CurrentOperation.LIST_INITIALISE)
                'End If
                bTemp = False
            End If

            'Idea 1: we can hanle wait scenario in the Data Engine
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If

            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: PLR record write failure", _
                                                              Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function

    ''' <summary>
    '''  This Function is sent when a Picking List is selected from PL home screen.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreatePLS(ByRef objPLS As PLSRecord) As Boolean
        Dim bTemp As Boolean = False
        Dim strListId As String = Nothing
        Dim strSequencenum As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()

        Try
            strExportDataString.Append("PLS")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            strExportDataString.Append(objPLS.strListID)
            strExportDataString.Append("001")
            'strExportDataString.Append(Environment.NewLine)
            m_PreviousRequest = strExportDataString.ToString()
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.LIST_INITIALISE)
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(CurrentOperation.LIST_INITIALISE)
                'End If
                bTemp = False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: PLS record write failure", _
                                                         Logger.LogLevel.RELEASE)
            bTemp = False
        End Try
        Return bTemp
    End Function
    Public Sub SendRepeatedRequest(ByVal SequenceNumber As String)
        'Idea 2: Store the previous message and edit and send the new response
    End Sub


   

    ''' <summary>
    ''' This Function requests Count Lists from the controller
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateCLR() As Boolean
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()
        ' Appending the details to the  string 
        Try
            strExportDataString.Append("CLR")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            strExportDataString.Append("001")
            'strExportDataString.Append(Environment.NewLine)
            m_PreviousRequest = strExportDataString.ToString()
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.LIST_INITIALISE)
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(CurrentOperation.LIST_INITIALISE)
                'End If
                bTemp = False
            End If

        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: CLR record write failure", _
                                                             Logger.LogLevel.RELEASE)
            bTemp = False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateRPX() As Boolean
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()

        ' Appending the details to the  string 
        Try
            strExportDataString.Append("RPX")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            ' strExportDataString.Append(Environment.NewLine)
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
                If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification()) Then
                    Dim objResponse As Object = Nothing
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is ACKRecord Then
                            bTemp = True
                        ElseIf TypeOf (objResponse) Is NAKRecord Then
                            bTemp = False
                        End If
                    End If
                    objResponse = Nothing
                End If
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect()
                'End If
                bTemp = False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: RPX record write failure", _
                                                             Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateRPS(ByRef objRPS As RPSRecord) As Boolean
        Dim bTemp As Boolean = False
        'Dim strSequencenum As String = Nothing
        'Dim strReportID As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()
        objRPS.strReportID = objRPS.strReportID.PadLeft(12, "0")
        objRPS.strSequenceNo = objRPS.strSequenceNo.PadLeft(4, "0")
        ' Appending the details to the  string 
        Try
            strExportDataString.Append("RPS")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            strExportDataString.Append(objRPS.strReportID)
            strExportDataString.Append(objRPS.strSequenceNo)
            'strExportDataString.Append(Environment.NewLine)
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification())
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect()
                'End If
                bTemp = False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: RPS record write failure", _
                                                             Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objRLE"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateRLE(ByRef objRLE As RLERecord) As Boolean
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()
        ' Appending the details to the  string 
        Try
            strExportDataString.Append("RLE")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            strExportDataString.Append(objRLE.strSequenceNo)
            ' strExportDataString.Append(Environment.NewLine)
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
                'Threading.Thread.Sleep(10)
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification())
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect()
                'End If
                bTemp = False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: RLE record write failure", _
                                                             Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateRLS(ByRef objRLS As RLSRecord) As Boolean
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()
        objRLS.strReportID = objRLS.strReportID.PadLeft(12, "0")
        objRLS.strSequenceNo = objRLS.strSequenceNo.PadLeft(4, "0")
        ' Appending the details to the  string 
        Try
            strExportDataString.Append("RLS")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            strExportDataString.Append(objRLS.strReportID)
            strExportDataString.Append(objRLS.strSequenceNo)
            'strExportDataString.Append(Environment.NewLine)
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification())
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect()
                'End If
                bTemp = False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: RLS record write failure", _
                                                             Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateRPO() As Boolean
        Dim bTemp As Boolean = False
#If RF Then
        If m_ConnectionLostExit Then
            objAppContainer.objLogger.WriteAppLog("RPO : Connection not present and establishing connection " + _
                                                    "before session start", Logger.LogLevel.RELEASE)
            Reconnect(CurrentOperation.MODULE_START_RECORD)
        End If
#End If
        Dim strExportDataString As New System.Text.StringBuilder()
        Dim objResponse As Object = Nothing
        ' Appending the details to the  string 
        Try
            strExportDataString.Append("RPO")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            'strExportDataString.Append(Environment.NewLine)
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
                If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.MODULE_START_RECORD) Then
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is ACKRecord Then
                            bTemp = True
                        ElseIf TypeOf (objResponse) Is NAKRecord Then
                            bTemp = False
                        End If
                    End If
                End If
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect(CurrentOperation.MODULE_START_RECORD)
                'End If
                bTemp = False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: RPO record write failure", _
                                                             Logger.LogLevel.RELEASE)
            Return False
        Finally
            objResponse = Nothing
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateSSE(ByRef objSSE As SSERecord) As Boolean
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()
        ' Appending the details to the  string 
        Try
            strExportDataString.Append("SSE")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            ' strExportDataString.Append(Environment.NewLine)
            bTemp = m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: SSE record write failure", _
                                                             Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates the transact message for ISE(Item Sales Enquiry)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateISE(ByVal strBarcode As String) As Boolean
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()
        strBarcode = strBarcode.PadLeft(13, "0")
        'Appending the details to the  string 
        Try
            strExportDataString.Append("ISE")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            strExportDataString.Append(strBarcode)
            ' strExportDataString.Append(Environment.NewLine)
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
                'Threading.Thread.Sleep(10)
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification())
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect()
                'End If
                Return False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: ISE record write failure", _
                                                             Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateDNQ(ByVal strDealNumber As String) As Boolean
        Dim bTemp As Boolean = False
        Dim strDealNum As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()
        strDealNum = strDealNumber.PadLeft(4, "0")
        ' Appending the details to the  string 
        Try
            strExportDataString.Append("DNQ")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            strExportDataString.Append(strDealNum.Trim(" "))
            ' strExportDataString.Append(Environment.NewLine)
            'Threading.Thread.Sleep(10)
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification())
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect()
                'End If
                Return False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: DNQ record write failure", _
                                                             Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function

    ''' <summary>
    ''' The function creates then transact message for PGF.Requests for Request list of planner families
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreatePGF(ByRef objPGF As PGFRecord) As Boolean
        Dim bTemp As Boolean = False
        Dim strSequencenum As String = Nothing
        Dim cCoreflag As Char = Nothing
        Dim cPlannerType As Char = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()
        strSequencenum = objPGF.strSequenceNo.PadLeft(4, "0")
        cCoreflag = objPGF.cCoreFlag
        cPlannerType = objPGF.cPlannerType

        ' Appending the details to the  string 
        Try
            strExportDataString.Append("PGF")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            strExportDataString.Append(strSequencenum)
            strExportDataString.Append(cCoreflag)
            strExportDataString.Append(strSequencenum)
            'strExportDataString.Append(Environment.NewLine)
            bTemp = m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: PGF record write failure", _
                                                             Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    'anoop:Start
    ''' <summary>
    ''' The function creates then transact message for PGF.Requests for Request list of planner families
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreatePGA(ByVal strBarCode) As Boolean
        Dim bTemp As Boolean = False
        Dim strStartChain As String = Nothing
        Dim strStartMod As String = Nothing
        Dim strStartItem As String = Nothing
        Dim strFlag As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()

        strStartChain = "000"
        strStartMod = "000"
        strStartItem = "000"
        strFlag = "L"

        ' Appending the details to the  string 
        Try
            strExportDataString.Append("PGA")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            strExportDataString.Append(strBarCode)
            strExportDataString.Append(strStartChain)
            strExportDataString.Append(strStartMod)
            strExportDataString.Append(strStartItem)
            strExportDataString.Append(strFlag)
            'Store the previous request
            m_PreviousRequest = strExportDataString.ToString()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification())
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect()
                'End If
                bTemp = False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: PGA record write failure", _
                                                             Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    'anoop:end
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreatePGQ(ByVal strPOGIPTR As String, ByVal isPendingPlanner As Boolean) As Boolean
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()
        ' Appending the details to the  string 
        Try
            strExportDataString.Append("PGQ")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            strExportDataString.Append(strPOGIPTR.PadLeft(6, "0"))
            If isPendingPlanner Then
                strExportDataString.Append("P")
            Else
                strExportDataString.Append("L")
            End If

            m_PreviousRequest = strExportDataString.ToString()
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(m_PreviousRequest) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification())
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect()
                'End If
                bTemp = False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: PGQ record write failure", _
                                                             Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' The function creates then transact message for PGM. Request the module information for the selected planner.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreatePGM(ByVal strPOGID As String) As Boolean
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()

        ' Appending the details to the  string 
        Try
            'Appending Transaction Identifier
            strExportDataString.Append("PGM")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            strExportDataString.Append(strPOGID)
            strExportDataString.Append("000")
            strExportDataString.Append("FFFFFF")
            ' strExportDataString.Append(Environment.NewLine)
            m_PreviousRequest = strExportDataString.ToString()
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(m_PreviousRequest) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification())
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect()
                'End If
                bTemp = False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: PGM record write failure", _
                                                             Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    Public Function CreatePGM(ByVal strPOGID As String, ByVal strBootsCode As String) As Boolean
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()
        strBootsCode = strBootsCode.PadLeft(6, "0")
        ' Appending the details to the  string 
        Try
            'Appending Transaction Identifier
            strExportDataString.Append("PGM")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            strExportDataString.Append(strPOGID)
            strExportDataString.Append("000")
            strExportDataString.Append(strBootsCode.Substring(0, 6))
            'strExportDataString.Append(Environment.NewLine)
            m_PreviousRequest = strExportDataString.ToString()
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(m_PreviousRequest) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification())
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect()
                'End If
                bTemp = False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: PGM record write failure", _
                                                             Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreatePPL(ByRef objPPL As PPLRecord) As Boolean
        Dim bTemp As Boolean = False
        Dim strPOGDBList As String = Nothing

        Dim strExportDataString As New System.Text.StringBuilder()
        strPOGDBList = objPPL.POG_DB_List.PadLeft(100, "0")

        ' Appending the details to the  string 
        Try
            strExportDataString.Append("PPL")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            strExportDataString.Append(strPOGDBList)
            '  strExportDataString.Append(Environment.NewLine)
            bTemp = m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString())
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: PPL record write failure", _
                                                             Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreatePSL(ByVal strPOGID As String, ByVal strModuleSeqNo As String) As Boolean
        Dim bTemp As Boolean = False
        Dim strPOGDBKey As String = Nothing
        Dim strModSequence As String = Nothing
        Dim strShelfNum As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()
        strPOGDBKey = strPOGID.PadLeft(6, "0")
        strModSequence = strModuleSeqNo.PadLeft(3, "0")
        'DEFECT FIX - BTCPR00004153(RF Mode :: Planner :: 
        'Notch number missing in the line list screen in planner module.)
        'strShelfNum = "000"
        strShelfNum = "001"
        ' Appending the details to the  string 
        Try
            strExportDataString.Append("PSL")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            strExportDataString.Append(strPOGDBKey)
            strExportDataString.Append(strModSequence)
            strExportDataString.Append(strShelfNum)
            strExportDataString.Append("00")
            strExportDataString.Append("00")
            m_PreviousRequest = strExportDataString.ToString()
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportDataString.ToString()) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification())
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect()
                'End If
                bTemp = False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: PSL record write failure", _
                                                             Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="strSequenceNumber"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendNextSequence(ByVal strSequenceNumber As String)
        Dim bTemp As Boolean = False
        Dim strMessageType As String = m_PreviousRequest.Substring(0, 3)
        Try
            Select Case strMessageType
                Case "CLR"
                    m_PreviousRequest = m_PreviousRequest.Remove(CLR.LISTID_OFFSET, CLR.LISTID)
                    strSequenceNumber = strSequenceNumber.PadLeft(3, "0")
                    m_PreviousRequest = m_PreviousRequest.Insert(CLR.LISTID_OFFSET, strSequenceNumber)
                    If m_TransactDataTransmitter.SendRecordRF(m_PreviousRequest) Then
                        bTemp = True
                    End If
                    'Case "CLS"
                    '    m_PreviousRequest = m_PreviousRequest.Remove(CLS.SEQ_OFFSET, CLS.SEQ)
                    '    strSequenceNumber = strSequenceNumber.PadLeft(3, "0")
                    '    m_PreviousRequest = m_PreviousRequest.Insert(CLS.SEQ_OFFSET, strSequenceNumber)
                    '    If m_TransactDataTransmitter.SendRecordRF(m_PreviousRequest) Then
                    '        bTemp = True
                    '    End If
                Case "PLR"
                    m_PreviousRequest = m_PreviousRequest.Remove(PLR.SEQ_OFFSET, PLR.SEQ)
                    strSequenceNumber = strSequenceNumber.PadLeft(3, "0")
                    m_PreviousRequest = m_PreviousRequest.Insert(PLR.SEQ_OFFSET, strSequenceNumber)
                    If m_TransactDataTransmitter.SendRecordRF(m_PreviousRequest) Then
                        bTemp = True
                    End If
                Case "PLS"
                    m_PreviousRequest = m_PreviousRequest.Remove(PLS.SEQ_OFFSET, PLS.SEQ)
                    strSequenceNumber = strSequenceNumber.PadLeft(3, "0")
                    m_PreviousRequest = m_PreviousRequest.Insert(PLS.SEQ_OFFSET, strSequenceNumber)
                    If m_TransactDataTransmitter.SendRecordRF(m_PreviousRequest) Then
                        bTemp = True
                    End If
                Case "PGA"
                    m_PreviousRequest = m_PreviousRequest.Remove(PGA.STARTCHAIN_OFFSET, 9)
                    m_PreviousRequest = m_PreviousRequest.Insert(PGA.STARTCHAIN_OFFSET, strSequenceNumber)

                    If m_TransactDataTransmitter.SendRecordRF(m_PreviousRequest) Then
                        bTemp = True
                    End If
                Case "PGF"
                    m_PreviousRequest = m_PreviousRequest.Remove(PGF.SEQ_OFFSET, PGF.SEQ)
                    m_PreviousRequest = m_PreviousRequest.Insert(PGF.SEQ_OFFSET, strSequenceNumber.PadLeft(PGF.SEQ, "0"))
                    bTemp = m_TransactDataTransmitter.SendRecordRF(m_PreviousRequest)
                Case "PGQ"
                    m_PreviousRequest = m_PreviousRequest.Remove(PGQ.POGIPTR_OFFSET, PGQ.POGIPTR)
                    m_PreviousRequest = m_PreviousRequest.Insert(PGQ.POGIPTR_OFFSET, strSequenceNumber.PadLeft(PGQ.POGIPTR, "0"))
                    bTemp = m_TransactDataTransmitter.SendRecordRF(m_PreviousRequest)
                Case "PGM"
                    m_PreviousRequest = m_PreviousRequest.Remove(PGM.MODSEQ_OFFSET, PGM.MODSEQ)
                    m_PreviousRequest = m_PreviousRequest.Insert(PGM.MODSEQ_OFFSET, strSequenceNumber.PadLeft(PGM.MODSEQ, "0"))
                    bTemp = m_TransactDataTransmitter.SendRecordRF(m_PreviousRequest)
                Case "PSL"
                    Dim strPrevShelf As String = m_PreviousRequest.Substring(15, 3)
                    Dim strPrevChain As String = m_PreviousRequest.Substring(18, 2)
                    Dim strPrevItem As String = m_PreviousRequest.Substring(20, 2)
                    Dim strNewShelf As String = strSequenceNumber.Substring(0, 3)
                    Dim strNewChain As String = strSequenceNumber.Substring(3, 2)
                    Dim strNewItem As String = strSequenceNumber.Substring(5, 2)
                    m_PreviousRequest = m_PreviousRequest.Remove(PSL.PSL_NEXT_OFFSET, PSL.PSL_NEXT)
                    If strNewShelf.Equals("FFF") Then
                        strNewShelf = strPrevShelf
                    End If
                    If strNewChain.Equals("FF") Then
                        strNewChain = strPrevChain
                    End If
                    If strNewItem.Equals("FF") Then
                        strNewItem = strPrevItem
                    End If
                    m_PreviousRequest = m_PreviousRequest + strNewShelf + strNewChain + strNewItem
                    bTemp = m_TransactDataTransmitter.SendRecordRF(m_PreviousRequest)
                Case "PGL"
                    m_PreviousRequest = m_PreviousRequest.Remove(PGL.NEXTSEQ_OFFSET, PGL.NEXTSEQ)
                    m_PreviousRequest = m_PreviousRequest.Insert(PGL.NEXTSEQ_OFFSET, strSequenceNumber.PadLeft(PGL.NEXTSEQ, "0"))
                    bTemp = m_TransactDataTransmitter.SendRecordRF(m_PreviousRequest)
                Case "CLS"
                    m_PreviousRequest = m_PreviousRequest.Remove(CLS.SEQ_OFFSET, CLS.SEQ)
                    strSequenceNumber = strSequenceNumber.PadLeft(3, "0")
                    m_PreviousRequest = m_PreviousRequest.Insert(CLS.SEQ_OFFSET, strSequenceNumber)
                    If m_TransactDataTransmitter.SendRecordRF(m_PreviousRequest) Then
                        bTemp = True
                    End If
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Send Next Request for Sequence" + strSequenceNumber, Logger.LogLevel.ERROR)
        End Try
        Return bTemp
    End Function
#If RF Then
    Private Function ProcessDatarecived(ByRef m_Status As DATAPOOL.ConnectionStatus, Optional ByVal ConnectionScenario As CurrentOperation = CurrentOperation.OTHERS) As Boolean
        Select Case m_Status
            Case DATAPOOL.ConnectionStatus.MessageRecieved
                Return True
            Case DATAPOOL.ConnectionStatus.Disconnected
                If ConnectionScenario <> CurrentOperation.SENDING_END_MESSAGE_AFTER_RECONNECT Then
                    Reconnect(ConnectionScenario)
                End If
                Return False
            Case DATAPOOL.ConnectionStatus.Timeout
                Dim strException As String = ""
                If ConnectionScenario <> CurrentOperation.SENDING_END_MESSAGE_AFTER_RECONNECT Then
                    'v1.1 MCF Change
                    If objAppContainer.bMCFEnabled Then
                        If objAppContainer.objExportDataManager.m_TransactDataTransmitter.fConnectAlternateInRF() Then
                            'Invoke connection retry to connect and pass timeout parameter as true.
                            Reconnect(ConnectionScenario, False)
                        Else
                            objAppContainer.objExportDataManager.m_TransactDataTransmitter.fCloseSession(ConnectionScenario)
                            'Close shelfmanagement main menu
                            objAppContainer.objShlfMgmntMenu.Close()
                        End If
                    Else
                        While objAppContainer.objExportDataManager.m_TransactDataTransmitter.HandleTimeOut(ConnectionScenario)
                            Try
                                'Invoke connection retry to connect and pass timeout parameter as true.
                                Reconnect(ConnectionScenario, True)
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
                End If
                Return False
        End Select
    End Function
    ''' <summary>
    ''' Function that handles the reconnection procedures.
    ''' </summary>
    ''' <param name="enumConnectionLostScenatio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function Reconnect(Optional ByVal enumConnectionLostScenatio As CurrentOperation = CurrentOperation.OTHERS, Optional ByVal bIsReconnectingAfterDataTimeout As Boolean = False) As Boolean
        'Temperorily storing the previous connection loss scenario
        Dim bConnectionLostInPreviousSession As Boolean = m_ConnectionLostExit
        'Temporarily storing the previous Module
        Dim tempLastActiveModule As AppContainer.ACTIVEMODULE
        If bConnectionLostInPreviousSession Then
            'Temporarily storing the previous Module
            'if there was a previous connection loss and the currently module doesnt connect the previous module has to be reset
            tempLastActiveModule = m_LastActiveModule
        End If
        m_ConnectionLostExit = True
        objAppContainer.objLogger.WriteAppLog("Reconnecting... Setting status Bar message as disconnected ", Logger.LogLevel.RELEASE)
        objAppContainer.ConnectionStatus = False
        UpdateStatusBarMessage(enumConnectionLostScenatio)
        If m_TransactDataTransmitter.ModuleReconnect(enumConnectionLostScenatio, m_LastActiveModule, m_BufferData, bIsReconnectingAfterDataTimeout) Then
            If enumConnectionLostScenatio = CurrentOperation.SIGN_ON Then
                m_ConnectionLostExit = False
                objAppContainer.ConnectionStatus = True
                UpdateStatusBarMessage(CurrentOperation.SIGN_ON)
                Throw New Exception(Macros.CONNECTION_REGAINED)
            End If
            'No need to reset here --> because only wen the Reconnection fails the last active module will be reset
            'Check the functioning of the m_TransactDataTransmitter.ModuleReconnect --> Only in case of connection not established the last active module is changed
            'Here the last active module would remain unchanged
            'However to be cautious this is being checked and reset
            If bConnectionLostInPreviousSession Then
                'resetting the last active module
                m_LastActiveModule = tempLastActiveModule
            End If
            If CreateRCN() Then
                'v1.1 MCF Change 
                'If not connected to alternate
                If objAppContainer.iConnectedToAlternate <> 1 Then
                    If ((bConnectionLostInPreviousSession) And _
                        (enumConnectionLostScenatio = CurrentOperation.MODULE_START_RECORD)) Then
                        'Incase connection lost and previous session was closed, and wen a new session is started 
                        'then send a close message to enable controller to gracefully close the Previous session
                        ClosePreviousSession()
                    End If
                        objAppContainer.objLogger.WriteAppLog("Setting status Bar message as Connected ", Logger.LogLevel.RELEASE)
                    objAppContainer.ConnectionStatus = True
                    UpdateStatusBarMessage(enumConnectionLostScenatio)
                    'Reset Connection Lost exit Status and End Record Send status
                    'When Connection is established and Previous session is closed the End Record needed flag Has to be reset
                    m_ConnectionLostExit = False
                    m_IsEndRecordNeeded = False
                    If enumConnectionLostScenatio = CurrentOperation.MODULE_START_RECORD Then
                        If bConnectionLostInPreviousSession Then
                            Cursor.Current = Cursors.WaitCursor
                            m_TimeOutRetrySuccesStrtRcrd = True
                            Return True
                        Else
                            m_TimeOutRetrySuccesStrtRcrd = False
                            'If RCN is sent and the message is not start message
                            Throw New Exception(Macros.CONNECTION_REGAINED)
                        End If
                    Else
                        Throw New Exception(Macros.CONNECTION_REGAINED)
                    End If
                ElseIf objAppContainer.iConnectedToAlternate = 1 Then
                    'Archana
                    'v1.1 MCF Change
                    'Add code to xml change and reset variable
                    'objAppContainer.iConnectedToAlternate = 0
                    'When Connection is established and Previous session is closed the End Record needed flag Has to be reset
                    m_ConnectionLostExit = False
                    m_IsEndRecordNeeded = False
                    objAppContainer.objLogger.WriteAppLog("Setting status Bar message as Connected ", Logger.LogLevel.RELEASE)
                    objAppContainer.ConnectionStatus = True
                    UpdateStatusBarMessage(enumConnectionLostScenatio)
                Else
                    'If Connection established and RCN failed Then its equal to connection not regained
                    If bConnectionLostInPreviousSession Then
                        'Resetting the last active module
                        m_LastActiveModule = tempLastActiveModule
                    Else
                        'If Connection lost in the previous session and in this session 
                        'Conection not established leave the m_IsEndRecord Needed Boolean Untouched.
                        If enumConnectionLostScenatio = CurrentOperation.LIST_FINISH_xLF Or _
                       enumConnectionLostScenatio = CurrentOperation.MODULE_START_RECORD Then
                            'If Connection is lost in previous session then check for that
                            m_IsEndRecordNeeded = False
                            If ItemInfoSessionMgr.GetInstance.getItemInfoInvokingModule <> AppContainer.ACTIVEMODULE.ITEMINFO Then
                                'If Connection is lost in previous session then check for that
                                m_IsEndRecordNeeded = True
                            End If
                        Else
                            m_IsEndRecordNeeded = True
                        End If
                    End If
                    Throw New Exception(Macros.CONNECTION_LOST_EXCEPTION_MESSAGE.ToString())
                End If
            End If

        ElseIf objAppContainer.iConnectedToAlternate <> -1 Then 'Not reconnected from alternate
            'in case there was a connection loss in previous session and
            ' the next session is different and unable to reestablish connection then reset to the previous module
            'Setting Active module before trying to reconnect during Connection Lost
            If bConnectionLostInPreviousSession Then
                'Resetting the last active module
                m_LastActiveModule = tempLastActiveModule
            Else
                'If Connection lost in the previous session and in this session 
                'Conection not established leave the m_IsEndRecord Needed Boolean Untouched.
                If enumConnectionLostScenatio = CurrentOperation.LIST_FINISH_xLF Or _
               enumConnectionLostScenatio = CurrentOperation.MODULE_START_RECORD Then
                    'If Connection is lost in previous session then check for that
                    m_IsEndRecordNeeded = False
                    'Fix for PL not closing when item Info selected from PL (out of Range)
                    If ItemInfoSessionMgr.GetInstance.getItemInfoInvokingModule <> AppContainer.ACTIVEMODULE.ITEMINFO Then
                        'If Connection is lost in previous session then check for that
                        m_IsEndRecordNeeded = True
                    End If

                Else
                    m_IsEndRecordNeeded = True
                End If
            End If
            'Create and throw a new exception if reconnection fails.
            Throw New Exception(Macros.CONNECTION_LOST_EXCEPTION_MESSAGE.ToString())
        End If
    End Function
    ''' <summary>
    ''' Gracefully exit Previous session - send end session messages
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ClosePreviousSession() As Boolean
        If m_IsEndRecordNeeded Then
            Select Case m_LastActiveModule
                Case AppContainer.ACTIVEMODULE.PICKGLST
                    'Close the picking List
                    objAppContainer.objLogger.WriteAppLog("Last Active Module was picking List and thence a PLX is sent", Logger.LogLevel.RELEASE)
                    If (Not m_BufferData Is Nothing) AndAlso (TypeOf (m_BufferData) Is PLXRecord) Then
                        If CreatePLX(CType(m_BufferData, PLXRecord), CurrentOperation.SENDING_END_MESSAGE_AFTER_RECONNECT) Then
                            m_BufferData = Nothing
                            'Reset Picking List ID after sending End Record
                            strPLListID = "000"
                            'Resetting the Buffer Data
                            m_BufferData = Nothing
                            objAppContainer.objLogger.WriteAppLog("PLX Sent Successfully", Logger.LogLevel.RELEASE)
                            If CreatePLF(CurrentOperation.SENDING_END_MESSAGE_AFTER_RECONNECT) Then
                                objAppContainer.objLogger.WriteAppLog("PLF Sent Successfully", Logger.LogLevel.RELEASE)
                                Return True
                            Else
                                Return False
                            End If
                        Else
                            objAppContainer.objLogger.WriteAppLog("The BUFFER DATA is not of type PLX", Logger.LogLevel.RELEASE)
                            Return False
                        End If
                    End If
                Case AppContainer.ACTIVEMODULE.CUNTLIST
                    'close Count List
                    objAppContainer.objLogger.WriteAppLog("Last Active Module was Count List and hence a CLX is sent", Logger.LogLevel.RELEASE)
                    If (Not m_BufferData Is Nothing) AndAlso (TypeOf (m_BufferData) Is CLXRecord) Then
                        If CreateCLX(CType(m_BufferData, CLXRecord), CurrentOperation.SENDING_END_MESSAGE_AFTER_RECONNECT) Then
                            m_BufferData = Nothing
                            'Reset Picking List ID after sending End Record
                            strPLListID = "000"
                            'Resetting the Buffer Data
                            m_BufferData = Nothing
                            objAppContainer.objLogger.WriteAppLog("CLX Sent Successfully", Logger.LogLevel.RELEASE)
                            If CreateCLF(CurrentOperation.SENDING_END_MESSAGE_AFTER_RECONNECT) Then
                                objAppContainer.objLogger.WriteAppLog("CLF Sent Successfully", Logger.LogLevel.RELEASE)
                                Return True
                            Else
                                Return False
                            End If
                        Else
                            objAppContainer.objLogger.WriteAppLog("The BUFFER DATA is not of type CLX", Logger.LogLevel.RELEASE)
                            Return False
                        End If
                    Else
                        Return False
                    End If
                Case AppContainer.ACTIVEMODULE.EXCSSTCK
                    'If Not m_IsEndRecordNeeded Then
                    '    objAppContainer.objLogger.WriteAppLog("EXCESS STOCK: No End Record Needed", Logger.LogLevel.RELEASE)
                    '    Exit Select
                    'End If
                    If ((Not m_BufferData Is Nothing) AndAlso (TypeOf (m_BufferData) Is EX_GAX_Record)) Then
                        If (objAppContainer.objActiveModule <> AppContainer.ACTIVEMODULE.EXCSSTCK) Then
                            objAppContainer.objLogger.WriteAppLog("Last Active Module was Excess Stock and hence a GAX is sent", Logger.LogLevel.RELEASE)
                            If CreateGAX(CType(m_BufferData, EX_GAX_Record).exGAXRecord, CurrentOperation.SENDING_END_MESSAGE_AFTER_RECONNECT) Then
                                m_BufferData = Nothing
                                'Reset Picking List ID after sending End Record
                                strPLListID = "000"
                                objAppContainer.objLogger.WriteAppLog("GAX Sent Successfully", Logger.LogLevel.RELEASE)
                                Return True
                            Else
                                objAppContainer.objLogger.WriteAppLog("Sending GAX failed", Logger.LogLevel.RELEASE)
                                Return False
                            End If
                        Else
                            EXSessionMgr.GetInstance.SequenceNumber = CType(m_BufferData, EX_GAX_Record).exGAXRecord.strPickListItems
                            EXSessionMgr.GetInstance.CurrentLocation = CType(m_BufferData, EX_GAX_Record).currLocation
                            m_BufferData = Nothing
                            Return True
                        End If
                    End If

                Case AppContainer.ACTIVEMODULE.FASTFILL
                    'If Not m_IsEndRecordNeeded Then
                    '    objAppContainer.objLogger.WriteAppLog("FAST FILL: No End Record Needed", Logger.LogLevel.RELEASE)
                    '    Exit Select
                    'End If
                    If ((Not m_BufferData Is Nothing) AndAlso (TypeOf (m_BufferData) Is GAXRecord)) Then
                        If (objAppContainer.objActiveModule <> AppContainer.ACTIVEMODULE.FASTFILL) Then
                            objAppContainer.objLogger.WriteAppLog("Last Active Module was Fast Fill and hence a GAX is sent", Logger.LogLevel.RELEASE)
                            If CreateGAX(CType(m_BufferData, GAXRecord), CurrentOperation.SENDING_END_MESSAGE_AFTER_RECONNECT) Then
                                m_BufferData = Nothing
                                'Reset Picking List ID after sending End Record
                                strPLListID = "000"
                                objAppContainer.objLogger.WriteAppLog("GAX Sent Successfully", Logger.LogLevel.RELEASE)
                                Return True
                            Else
                                objAppContainer.objLogger.WriteAppLog("GAX sending Failed", Logger.LogLevel.RELEASE)
                                Return False
                            End If
                        Else
                            Dim objGAX As GAXRecord = CType(m_BufferData, GAXRecord)
                            FFSessionMgr.GetInstance.SequenceNumber = Val(objGAX.strPickListItems)
                            FFSessionMgr.GetInstance.SELS = Val(objGAX.strSELS)
                            ' FFSessionMgr.GetInstance.PriceCheck = Val(objGAX.strPickListItems)
                            objGAX = Nothing
                            m_BufferData = Nothing
                            Return True
                        End If
                    End If

                Case AppContainer.ACTIVEMODULE.SHLFMNTR
                    'If Not m_IsEndRecordNeeded Then
                    '    objAppContainer.objLogger.WriteAppLog("SHELF MONITOR: No End Record Needed", Logger.LogLevel.RELEASE)
                    '    Exit Select
                    'End If
                    If ((Not m_BufferData Is Nothing) AndAlso (TypeOf (m_BufferData) Is GAXRecord)) Then
                        If (objAppContainer.objActiveModule <> AppContainer.ACTIVEMODULE.SHLFMNTR) Then
                            objAppContainer.objLogger.WriteAppLog("Last Active Module was Shelf Monitor and hence a GAX is sent", Logger.LogLevel.RELEASE)
                            If CreateGAX(CType(m_BufferData, GAXRecord), CurrentOperation.SENDING_END_MESSAGE_AFTER_RECONNECT) Then
                                m_BufferData = Nothing
                                'Reset Picking List ID after sending End Record
                                strPLListID = "000"
                                objAppContainer.objLogger.WriteAppLog("GAX Sent Successfully", Logger.LogLevel.RELEASE)
                                Return True
                            Else
                                objAppContainer.objLogger.WriteAppLog("GAX sending Failed", Logger.LogLevel.RELEASE)
                                Return False
                            End If
                        Else
                            Dim objGAX As GAXRecord = CType(m_BufferData, GAXRecord)
                            SMSessionMgr.GetInstance.SequenceNumber = Val(objGAX.strPickListItems)
                            SMSessionMgr.GetInstance.SELS = Val(objGAX.strSELS)
                            'SMSessionMgr.GetInstance.PriceCheck = Val(objGAX.strPickListItems)
                            objGAX = Nothing
                            m_BufferData = Nothing
                            Return True
                        End If
                    End If
                Case Else
                    objAppContainer.objLogger.WriteAppLog("The previous session doesnt come in the List Modules, Not sending any end Record", Logger.LogLevel.RELEASE)
            End Select
        Else
            objAppContainer.objLogger.WriteAppLog("End Record Not Needed. ", Logger.LogLevel.RELEASE)
        End If
    End Function
    ''' <summary>
    ''' Updates Status Bar Message When connection is lost/ Gained
    ''' </summary>
    ''' <param name="enumConnectionLostScenatio"></param>
    ''' <remarks></remarks>
    Private Sub UpdateStatusBarMessage(ByVal enumConnectionLostScenatio As CurrentOperation)
        If enumConnectionLostScenatio <> CurrentOperation.SIGN_ON Then
            objAppContainer.objShlfMgmntMenu.UpdateStatusBar()
            If enumConnectionLostScenatio <> CurrentOperation.MODULE_START_RECORD Then
                Select Case objAppContainer.objActiveModule
                    Case AppContainer.ACTIVEMODULE.ASSIGNPRINTER
                        APSessionMgr.GetInstance.UpdateStatusBarMessage()
                    Case AppContainer.ACTIVEMODULE.AUTOSTUFFYOURSHELVES
                        AutoSYSSessionManager.GetInstance.UpdateStatusBarMessage()
                    Case AppContainer.ACTIVEMODULE.CUNTLIST
                        CLSessionMgr.GetInstance.UpdateStatusBarMessage()
                    Case AppContainer.ACTIVEMODULE.EXCSSTCK
                        EXSessionMgr.GetInstance.UpdateStatusBarMessage()
                    Case AppContainer.ACTIVEMODULE.FASTFILL
                        FFSessionMgr.GetInstance.UpdateStatusBarMessage()
                    Case AppContainer.ACTIVEMODULE.ITEMINFO
                        ItemInfoSessionMgr.GetInstance.UpdateStatusBarMessage()
                    Case AppContainer.ACTIVEMODULE.ITEMSALES
                        ISSessionManager.GetInstance.UpdateStatusBarMessage()
                    Case AppContainer.ACTIVEMODULE.PICKGLST
                        objAppContainer.objLogger.WriteAppLog("The Current Active module is Picking List", Logger.LogLevel.RELEASE)
                        PLSessionMgr.GetInstance.UpdateStatusBarMessage()
                    Case AppContainer.ACTIVEMODULE.PRICECHK
                        PCSessionMgr.GetInstance.UpdateStatusBarMessage()
                    Case AppContainer.ACTIVEMODULE.PRINTSEL
                        PSSessionMgr.GetInstance.UpdateStatusBarMessage()
                    Case AppContainer.ACTIVEMODULE.PRTCLEARANCE
                        CLRSessionMgr.GetInstance.UpdateStatusBarMessage()
                    Case AppContainer.ACTIVEMODULE.REPORTS
                        ReportsSessionManager.GetInstance.UpdateStatusBarMessage()
                    Case AppContainer.ACTIVEMODULE.SHLFMNTR
                        SMSessionMgr.GetInstance.UpdateStatusBarMessage()
                    Case AppContainer.ACTIVEMODULE.SPCEPLAN
                        SPSessionMgr.GetInstance.UpdateStatusBarMessage()
                    Case AppContainer.ACTIVEMODULE.SPCEPLAN_PENDING
                        SPSessionMgr.GetInstance.UpdateStatusBarMessage()
                    Case AppContainer.ACTIVEMODULE.STORESALES
                        SSSessionManager.GetInstance.UpdateStatusBarMessage()
                End Select
            End If
        Else
            RFUserSessionManager.GetInstance.UpdateStatusBar()
        End If
    End Sub
#End If
    ''' <summary>
    ''' The function creates then transact message for PGL. Requests extended item information details including planner data.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreatePGL(ByVal strBootsCode As String, Optional ByVal isPending As Boolean = False) As Boolean
        Dim bTemp As Boolean = False
        Dim cPlannerType As Char = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()

        strBootsCode = strBootsCode.PadLeft(6, "0")


        ' Appending the details to the  string 
        Try
            strExportDataString.Append("PGL")
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID)
            'Appending Boots Code
            strExportDataString.Append(strBootsCode.Substring(0, 6))
            'Appending the Start Chain Index
            strExportDataString.Append("000")
            'Appending the Start Planner Sequence
            strExportDataString.Append("000")
            'Appending Pending/Live Flag
            If isPending Then
                strExportDataString.Append("P")
            Else
                strExportDataString.Append("L")
            End If
            'strExportDataString.Append(Environment.NewLine)
            m_PreviousRequest = strExportDataString.ToString()
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(m_PreviousRequest) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification())
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect()
                'End If
                bTemp = False
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("SMTransactDataManager: PGL record write failure", _
                                                             Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function

    Public Function CreatePGF(ByVal strCore As String, ByVal isPendingPlanner As Boolean) As Boolean
        Dim bTemp As Boolean = False
        Dim strExportData As New System.Text.StringBuilder()
        Try
            'Appending the Trasaction Identifier
            strExportData.Append("PGF")
            'appending the User ID 
            strExportData.Append(objAppContainer.strCurrentUserID.PadLeft(3, "0"))
            'Appending the Sequence Number
            strExportData.Append("0000")
            If strCore.Equals("Y") Then
                strExportData.Append("C")
            Else
                strExportData.Append("N")
            End If
            If isPendingPlanner Then
                strExportData.Append("P")
            Else
                strExportData.Append("L")
            End If
            DATAPOOL.getInstance.ResetPoolData()
            m_PreviousRequest = strExportData.ToString()
            If m_TransactDataTransmitter.SendRecordRF(m_PreviousRequest) Then
                bTemp = ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification())
            Else
                'If Not DATAPOOL.getInstance.isConnected Then
                Reconnect()
                'End If
                bTemp = False
            End If

        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Create PGF:: Exception :: " + ex.Message, Logger.LogLevel.INFO)
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateRCN() As Boolean
        objAppContainer.objLogger.WriteAppLog("Sending an RCN", Logger.LogLevel.RELEASE)
        Dim bTemp As Boolean = False
        Dim strExportData As New System.Text.StringBuilder()
        Dim lFreeMem As Long = Nothing
        Dim strAppVersion As String = Nothing
        Dim aReleaseVersion() As String = Nothing
        Dim strDeviceType As String = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DEVICE_TYPE)
        'Dim strAppID As String '= ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_ID)
        'Assign the Mac ID which was retrieved when the application started
        Dim strMacID As String = objAppContainer.strMACADDRESS
        strAppVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
        aReleaseVersion = strAppVersion.Split("-")
        strAppVersion = aReleaseVersion(1)
        Try
            'Appending the Trasaction Identifier
            strExportData.Append("RCN")
            'appending the User ID 
            strExportData.Append(objAppContainer.strCurrentUserID.PadLeft(3, "0"))
            strExportData.Append(objAppContainer.strPassword.PadLeft(3, "0"))
            'Appending APPID 
            'App ID for Picking List Has to be 001 and other modules should be 000
            If objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PICKGLST Then
                strExportData.Append("001")
            Else
                strExportData.Append("000")
            End If
            'strExportData.Append(strAppID)
            'Appending APp version
            strExportData.Append(strAppVersion.PadLeft(4, "0"))
            'Appending the MACID
            If strMacID = "" Then
                strExportData.Append("000000000000")
            Else
                strExportData.Append(strMacID)
            End If
            'Append Device Type
            strExportData.Append(strDeviceType)

            'IPADDRESS
            strExportData.Append(objAppContainer.objHelper.GetIPAddress())

            'Free Memory
            strExportData.Append(objAppContainer.objHelper.CheckForFreeMemory("Program Files", lFreeMem).PadLeft(8, "0"))
            'Append List ID
            'If there was a connection loast exit in the previous session then check for previous module.
            'when connection lost during a session and connection is reestablished then no need to check for previous module
            'Just append the list ID
            If m_ConnectionLostExit Then
                If objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.SHLFMNTR Or _
                               objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.FASTFILL Or _
                               objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.EXCSSTCK Then
                    If (m_LastActiveModule = objAppContainer.objActiveModule) Then
                        'Only when last module and the currently selected module is same we have to send the 
                        'same list ID
                        If strPLListID Is Nothing Then
                            strPLListID = "000"
                        End If
                        strExportData.Append(strPLListID.PadLeft(3, "0"))
                    Else
                        objAppContainer.objLogger.WriteAppLog("Reset the list ID as the current module is not same as previous", Logger.LogLevel.RELEASE)
                        strExportData.Append("000")
                    End If
                Else
                    objAppContainer.objLogger.WriteAppLog("Reset the list ID as the current module is not same as previous", Logger.LogLevel.RELEASE)
                    'Give the list ID as 000 if the module is different
                    ' IF Reconnect succeds we actually have entered the module and only in that time the ID is reset
                    strExportData.Append("000")
                End If
            Else
                'If there is no connection lost exit then it means that we are starting a fresh session
                strPLListID = "000"
                'If objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.SHLFMNTR Or _
                '            objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.FASTFILL Or _
                '            objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.EXCSSTCK Then
                '    If strPLListID Is Nothing Then

                '    End If
                '    strExportData.Append(strPLListID.PadLeft(3, "0"))
                'Else
                strExportData.Append(strPLListID)
                'End If
            End If

            'Once the PLLID is sent in a RCN Reset it 
            'strPLListID = "000"
            DATAPOOL.getInstance.ResetPoolData()
            If m_TransactDataTransmitter.SendRecordRF(strExportData.ToString()) Then
                If ProcessDatarecived(DATAPOOL.getInstance.WaitForNotification(), CurrentOperation.SENDING_END_MESSAGE_AFTER_RECONNECT) Then
                    Dim objResponse As Object = Nothing
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is ACKRecord Then
                            bTemp = True
                            Dim objAck As ACKRecord = CType(objResponse, ACKRecord)
                            'v1.1 MCF Change
                            'If not connected to alternate
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
                            'v1.1 MCF Change
                            'If not connected to alternate
                            If objAppContainer.iConnectedToAlternate <> 1 Then
                                If objNAK.strErrorMessage <> "" Then
                                    MessageBox.Show(objNAK.strErrorMessage, "Error while Connecting", _
                                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                            MessageBoxDefaultButton.Button1)
                                Else
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M60"), "Error while Connecting", _
                                           MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                           MessageBoxDefaultButton.Button1)
                                End If
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
                'If Not DATAPOOL.getInstance.isConnected Then
                'Reconnect()
                'End If
                bTemp = False
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE OrElse _
                            ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Create RCN:: Exception :: " + ex.Message, Logger.LogLevel.INFO)
        End Try
        Return bTemp
    End Function
#End If

#If RF Then
    ''' <summary>
    ''' Enum Hadles Connection Lost Scenarios - During which operation the connection was lost 
    ''' To show appropriate message
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum CurrentOperation
        MODULE_START_RECORD
        TSF_MODIFICATION
        PRINT_SELECTION
        SENDING_END_MESSAGE_AFTER_RECONNECT
        SIGN_ON
        LIST_INITIALISE
        LIST_EXIT_xLX
        LIST_FINISH_xLF
        OTHERS
    End Enum
#End If
    'v1.1 MCF Change
    '' <summary>
    '' changes the current active IP to alternate controller IP
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
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
#Region "DATAPOOL"
#If RF Then
Class DATAPOOL
    Private bError As Boolean = False
    Private strErrMsg As String = ""
    Private areNotifier As AutoResetEvent
    Private m_DataArray As ArrayList
    Private m_Index As Integer = 0
    Private bWaitingForNotification As Boolean = False
    Private Shared objDataPool As DATAPOOL
    Private bWaitingforConnection As Boolean = False
    Private m_ConnectionStatus As ConnectionStatus
    ''' <summary>
    ''' Connection status
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum ConnectionStatus
        Connected
        Disconnected
        MessageRecieved
        Timeout
        Cancelled
    End Enum
    ''' <summary>
    ''' Whether a active socket available for sending and recieving data. true - available, false - not available
    ''' </summary>
    ''' <remarks></remarks>
    Private bConnectionStatus As Boolean = False
    ''' <summary>
    ''' Reset event which is used to connect to the socket
    ''' </summary>
    ''' <remarks></remarks>
    Private areConnect As AutoResetEvent
    ''' <summary>
    ''' Whether a active socket available for sending and recieving data. true - available, false - not available
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property isConnected() As Boolean
        Get
            Return bConnectionStatus
        End Get
        Set(ByVal value As Boolean)
            bConnectionStatus = value
        End Set
    End Property
    Public Property WaitingForNotification() As Boolean
        Get
            Return bWaitingForNotification
        End Get
        Set(ByVal value As Boolean)
            bWaitingForNotification = value
        End Set
    End Property

    Public Property WaitingForConnection() As Boolean
        Get
            Return bWaitingforConnection
        End Get
        Set(ByVal value As Boolean)
            bWaitingforConnection = value
        End Set
    End Property

    ''' <summary>
    ''' Function to wait for Connection. the execution stops untill a connection is completed
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WaitForConnection() As ConnectionStatus
        Try
            areConnect = New AutoResetEvent(False)
            objAppContainer.objLogger.WriteAppLog("Waiting for Connection", Logger.LogLevel.RELEASE)
            WaitingForConnection = True
            'Thread.CurrentThread.Priority = ThreadPriority.Lowest
            areConnect.WaitOne()
            Thread.CurrentThread.Priority = ThreadPriority.Lowest
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("DATAPOOL::Error Occured while waiting for a connection :: " _
                                                  + ex.Message, Logger.LogLevel.INFO)
        End Try
        Return m_ConnectionStatus
    End Function

    ''' <summary>
    ''' Notification for the Connection Status. 
    ''' </summary>
    ''' <param name="enumStauts"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function NotifyConnectionStatus(ByVal enumStauts As ConnectionStatus) As Boolean
        Try
            'Priority is set to lowest Becaause, if the object 
            'is not waiting then It might give some priority  for that thread
            'to execute and wait
            Thread.CurrentThread.Priority = ThreadPriority.Lowest
            If Not areConnect Is Nothing Then
                'Status denote connection status
                WaitingForConnection = False
                m_ConnectionStatus = enumStauts
                If m_ConnectionStatus = ConnectionStatus.Connected Then
                    isConnected = True
                End If
                areConnect.Set()
                Thread.CurrentThread.Priority = ThreadPriority.Highest
                'objAppContainer.objLogger.WriteAppLog("Connection Notification Recieved", Logger.LogLevel.RELEASE)
                If isConnected Then
                    objAppContainer.objLogger.WriteAppLog("Connection status : CONNECTED")
                Else
                    objAppContainer.objLogger.WriteAppLog("Connection status : NOTCONNECTED")
                End If
            Else
                objAppContainer.objLogger.WriteAppLog("Nothing to notify", Logger.LogLevel.RELEASE)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("DATAPOOL::Error Occured while Notifying the connection Status :: " _
                                                + ex.Message, Logger.LogLevel.INFO)
        End Try
    End Function


    Public Property IsError() As Boolean

        Get
            Return bError
        End Get
        Set(ByVal value As Boolean)
            bError = value
        End Set
    End Property
    Public Property ErrorMessage() As String
        Get
            Return strErrMsg
        End Get
        Set(ByVal value As String)
            strErrMsg = value
        End Set
    End Property
    Private Sub New()
        m_DataArray = New ArrayList()
    End Sub
    Public Shared Function getInstance() As DATAPOOL
        If objDataPool Is Nothing Then
            objDataPool = New DATAPOOL()
        End If
        Return objDataPool
    End Function
    ''' <summary>
    ''' Function to retreive the object stored in the data pool and pass it to the calling
    ''' datasource function.
    ''' </summary>
    ''' <param name="objData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetNextObject(ByRef objData As Object) As Boolean
        Dim bTemp As Boolean = False
        SyncLock (m_DataArray)
            If (Not (m_DataArray.Count = 0)) And (m_Index < m_DataArray.Count) Then
                objData = m_DataArray(m_Index)
                m_Index += 1
                bTemp = True
            Else
                bTemp = False
            End If
        End SyncLock
        Return bTemp
    End Function
    Public Sub addObject(ByVal objDataContainer As Object)
        m_DataArray.Add(objDataContainer)
    End Sub
    Public Sub ResetPoolData()
        bError = False
        strErrMsg = ""
        m_DataArray.Clear()
        m_Index = 0
    End Sub

    Public Property StoreData() As ArrayList
        Get
            Return m_DataArray
        End Get
        Set(ByVal value As ArrayList)
            m_DataArray = value
        End Set
    End Property
    Public Function WaitForNotification() As ConnectionStatus
        If isConnected Then
            objAppContainer.objLogger.WriteAppLog("DATAPOOL::Waiting for Response")
            WaitingForNotification = True
            Try
                If Not (areNotifier Is Nothing) Then
                    areNotifier = Nothing
                End If
                areNotifier = New AutoResetEvent(False)
                areNotifier.WaitOne()
                Thread.CurrentThread.Priority = ThreadPriority.Lowest
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("Error Occured In Data pool Wait for Notification" + _
                                                      ex.Message, Logger.LogLevel.RELEASE)
            End Try
            areNotifier = Nothing
        Else
            m_ConnectionStatus = ConnectionStatus.Disconnected
        End If
        Return m_ConnectionStatus
    End Function
    Public Sub NotifyDataEngine(ByVal Status As ConnectionStatus)
        Try
            If WaitingForNotification Then
                Thread.CurrentThread.Priority = ThreadPriority.Highest
                m_ConnectionStatus = Status
                areNotifier.Set()
                'objAppContainer.objLogger.WriteAppLog("DATAPOOL::Recieved Notification")
                areNotifier = Nothing
            End If
            WaitingForNotification = False
        Catch ex As Exception
            'Log the exception Here
        End Try
    End Sub
    ''' <summary>
    ''' Ends the DATA POOL
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks>Call datpool end session only  when the application ends</remarks>
    Public Function EndSession() As Boolean
        Try
            If Not (objDataPool Is Nothing) Then
                objDataPool = Nothing
            End If
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function
End Class
#End If
#End Region
