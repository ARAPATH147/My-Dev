Imports System.IO
Imports System.Data
Imports System.Data.SqlServerCe
Public Class ReferenceFileParser
    'Read the content of the configuariton file.
    Private m_ConfigParams As ConfigParams = Nothing
    Private m_RowCount As Integer
    Private m_PercentageValue As Integer

    'Read the Connection String and the delimiter from the configuration file.
    Private CONN_STRING As String = Nothing
    Private DELIMETER As String = ReferenceFileMacro.Delimiter

    'Create an instance of the SQL CE Connection.
    Dim sqlConn As SqlCeConnection = Nothing

    'Minu Added default date constant
    Private Const DEFAULT_DATE As String = "19000101"
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        m_ConfigParams = New ConfigParams()
        m_ConfigParams = ConfigParser.GetInstance().GetConfigParams()
        'Assign connection string.
        CONN_STRING = "Data Source = " & m_ConfigParams.DB & "; Max Database Size = 4088; Max Buffer Size = 2048; flush interval = 1; temp file max size = 4088"
        'initialise the DB connections.
        Initialise()
    End Sub
    ''' <summary>
    ''' Method to open connection to the device database.
    ''' </summary>
    ''' <returns>Bool
    ''' True - If successfully opened a connection to the database.
    ''' False - If failed to open a connection to the database.
    ''' </returns>
    ''' <remarks></remarks>
    Private Function Initialise() As Boolean
        Try
            'Assign the connection string to the SQL connection.
            sqlConn = New SqlCeConnection(CONN_STRING)
            'Open the connection to the DB.
            sqlConn.Open()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Method to close the connection after executing the query.
    ''' </summary>
    ''' <returns>Bool
    ''' True - If successfully closed the connection.
    ''' False - If any error occured during the connection close.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function Terminate() As Boolean
        If sqlConn.State = ConnectionState.Open Then
            Try
                'Close the connection to the DB.
                sqlConn.Close()
                Return True
            Catch ex As SqlCeException
                Return False
            End Try
        Else
            Return False
        End If
    End Function
    ''' <summary>
    ''' To invoke fucntion corresponding to the name of the file.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseFile(ByVal strFileName As String, Optional ByVal iIndex As Integer = 0) As Boolean
        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseFile:: Parsing file " & strFileName, _
                                           Logger.LogLevel.RELEASE)
        Dim rdrFileReader As IO.StreamReader = Nothing
        Try
            Dim strPath As String = m_ConfigParams.TFTP.LocalFilePath.TrimEnd("\") + "\"
            Dim strLastLine As String
            rdrFileReader = New IO.StreamReader(strPath + strFileName)
            strLastLine = rdrFileReader.ReadLine()
            While Not rdrFileReader.EndOfStream
                strLastLine = rdrFileReader.ReadLine()
            End While
            'Close the file reader.
            rdrFileReader.Close()
            'Calculate the number of records in file.
            m_RowCount = CInt(strLastLine.Split(",")(1))
            m_RowCount = CInt(m_RowCount / 10)
            m_PercentageValue = m_RowCount
        Catch ex As Exception
            'Close the file reader.
            rdrFileReader.Close()
        End Try
        Try
            Select Case strFileName
                Case m_ConfigParams.BOOTCODE
                    Return Me.ParseBootCodeFile(strFileName, _
                                                m_ConfigParams.TFTP.LocalFilePath, iIndex)
                    Exit Select
                Case m_ConfigParams.PGROUP
                    Return Me.ParsePGroupFile(strFileName, _
                                              m_ConfigParams.TFTP.LocalFilePath, iIndex)
                    Exit Select
                Case m_ConfigParams.SUPPLIER
                    Return Me.ParseSupplierFile(strFileName, _
                                                m_ConfigParams.TFTP.LocalFilePath, iIndex)
                    Exit Select
                Case m_ConfigParams.USERS
                    Return Me.ParseUsersFile(strFileName, _
                                             m_ConfigParams.TFTP.LocalFilePath, iIndex)
                    Exit Select
                Case m_ConfigParams.RECALL
                    Return Me.ParseRecallFile(strFileName, _
                                              m_ConfigParams.TFTP.LocalFilePath, iIndex)
                    Exit Select
                Case m_ConfigParams.LIVEPOG
                    Return Me.ParseLivePOGFile(strFileName, _
                                               m_ConfigParams.TFTP.LocalFilePath, iIndex)
                    Exit Select
                Case m_ConfigParams.BARCODE
                    Return Me.ParseBarCode(strFileName, _
                                           m_ConfigParams.TFTP.LocalFilePath, iIndex)
                    Exit Select
                Case m_ConfigParams.DEAL
                    Return Me.ParseDealFile(strFileName, _
                                            m_ConfigParams.TFTP.LocalFilePath, iIndex)
                    Exit Select
                Case m_ConfigParams.CATEGORY
                    Return Me.ParseCategoryFile(strFileName, _
                                                m_ConfigParams.TFTP.LocalFilePath, iIndex)
                    Exit Select
                Case m_ConfigParams.SHELFDES
                    Return Me.ParseShelFDescFile(strFileName, _
                                                m_ConfigParams.TFTP.LocalFilePath, iIndex)
                    Exit Select
                Case m_ConfigParams.POGMODULE
                    Return Me.ParseModuleFile(strFileName, _
                                              m_ConfigParams.TFTP.LocalFilePath, iIndex)
                    Exit Select
                Case Else
                    Exit Select
            End Select
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: Parsefile :: Parsing " & strFileName & _
                                                            "File Failed, Message:" & ex.Message, _
                                                             Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    ''' To clear the data present in the reference data table.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function PurgeReferenceData() As Boolean
        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: PurgeReferenceData::Reference Data Purging started ", _
                                                             Logger.LogLevel.RELEASE)
        Dim aReferenceTable() As String = Nothing
        Dim iRetryCount As Integer = ReferenceFileMacro.DeleteRetry
        Dim iCount As Integer = 0
        Dim strInsertCmd As String = ""
        Dim sqlCommd As New SqlCeCommand()
        Dim sqlCompactEngine As SqlCeEngine
        sqlCommd.Connection = sqlConn
        sqlCompactEngine = New SqlCeEngine(CONN_STRING)
        aReferenceTable = ReferenceFileMacro.ReferenceTables.Split(DELIMETER)
        iCount = aReferenceTable.Length
        While iCount > 0
            Try
                strInsertCmd = "DELETE FROM " & aReferenceTable(iCount).Trim()
                sqlCommd.CommandText = strInsertCmd
                sqlCommd.ExecuteNonQuery()

                'Shrink the database to truncate the empty space occupied by the DB file
                sqlCompactEngine.Shrink()
            Catch ex As Exception
                'Try
                If iRetryCount > 0 Then
                    iRetryCount = iRetryCount - 1
                    Continue While
                Else
                    'Update the details of the table whose data is not purged.
                    'TODO: Update the details of the tables that are not purged.
                End If
            End Try
            'Increment the array index counter.
            iCount = iCount + 1
        End While
        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: PurgeReferenceData::Reference Data Purging completed ", _
                                                            Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' To parse bootcode.csv and update the DB
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function ParseBootCodeFile(ByVal strFileName As String, ByVal strPath As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        'To monitor LOG message status
        Dim bIsErrorLOGSend As Boolean = False
        'For reading bootcode.csv file
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0
        Dim iPercentage As Integer = 10
        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseBootCodeFile::File parsing started for BOOTCODE.CSV.", _
                                                        Logger.LogLevel.RELEASE)
        AppContainer.GetInstance.objRefDownloadForm.DBUpdationStart()
        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.START, _
                                                                  LOGTransmitter.FileName.BOOTCODE, _
                                                                  LOGTransmitter.Reasons.Parse_File)
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Load the file for reading.
            rdrFileReader = New System.IO.StreamReader(strPath + strFileName)

            'iFileIndex is set to a value when iFileIndex number of records 
            'is already inserted to the database.
            If iFileIndex > 0 Then
                iCount = iFileIndex
                iFileIndex = iFileIndex + 2
                For iLoopIndex As Integer = 1 To iFileIndex
                    'Reading a line from the CSV file
                    strLine = rdrFileReader.ReadLine()
                Next
            End If
            'Assign SQL connection to the command
            sqlCommd.Connection = sqlConn
            'Reading a line from the CSV file
            strLine = rdrFileReader.ReadLine()

            While Not strLine.StartsWith("T")
                strLine = strLine.Trim()
                If iCount = m_PercentageValue Then
                    AppContainer.GetInstance.objRefDownloadForm.SetDBUpdationstatus(iPercentage)
                    m_PercentageValue = m_PercentageValue + m_RowCount
                    iPercentage = iPercentage + 10
                End If
                If strLine.StartsWith("D") Then
                    Try
                        strLine = strLine.Substring(2)
                        'Parsing the line
                        staData = strLine.Split(DELIMETER)
                        'Check for length of the data.
                        Dim iRecLength = staData.Length
                        If iRecLength <> 67 Then
                            iCount = iCount + 1
                            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseBootCodeFile::Error - Length mismatch for BOOTCODE.CSV.", _
                                                        Logger.LogLevel.RELEASE)
                            If Not bIsErrorLOGSend Then
                                'Length mismatch - send a LOG message
                                AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                      LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.BOOTCODE, LOGTransmitter.Reasons.Length_Mismatch)
                                bIsErrorLOGSend = True
                            End If
                            'Reading a line from the CSV file
                            strLine = rdrFileReader.ReadLine()
                            Continue While
                        End If
                        'If item status is blank, then the product is assumed to 
                        'be active.
                        If (staData(4) = " " Or staData(4) = Nothing) Then
                            staData(4) = "A"
                        End If
                        staData(3) = staData(3).Replace("'", "''")
                        staData(9) = staData(9).Replace("'", "''")

                        For iIndex As Integer = 0 To iRecLength - 1
                            If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                                'Null values are replaced with '0'
                                staData(iIndex) = 0
                            End If
                        Next
                        'Minu Convert zeros into default date
                        If Val(staData(13)) = 0 Then
                            staData(13) = DEFAULT_DATE
                        End If
                        If Val(staData(14)) = 0 Then
                            staData(14) = DEFAULT_DATE
                        End If
                        If Val(staData(15)) = 0 Then
                            staData(15) = DEFAULT_DATE
                        End If
                        If Val(staData(33)) = 0 Then
                            staData(33) = DEFAULT_DATE
                        End If
                        If Val(staData(35)) = 0 Then
                            staData(35) = DEFAULT_DATE
                        End If
                        If Val(staData(54)) = 0 Then
                            staData(54) = DEFAULT_DATE
                        End If
                        If Val(staData(57)) = 0 Then
                            staData(57) = DEFAULT_DATE
                        End If
                        If Val(staData(60)) = 0 Then
                            staData(60) = DEFAULT_DATE
                        End If
                        If Val(staData(62)) = 0 Then
                            staData(62) = DEFAULT_DATE
                        End If
                        If Val(staData(65)) = 0 Then
                            staData(65) = DEFAULT_DATE
                        End If

                        'Minu Removed single quotes from the query for integer values.
                        'Build the SQL command with the values to be inserted to the table.
                        'RECALLS CHANGE to be added to the code for Last_Stock_Movement_Date
                        'Build the SQL command with the values to be inserted to the table.
                        strInsertCmd = "INSERT INTO BootsCodeView" _
                             & "(Boots_Code,First_Barcode,Second_Barcode,Item_Desc," _
                             & "Item_Status,BC,Product_Group,Supply_Route,Markdown_Flag,SEL_Desc,Unit_Name," _
                             & "Unit_Item_Quantity,Unit_Measure,Date_Last_Delivered," _
                             & "Date_Last_Counted,Last_Stock_Movement_Date,SOD_TSF," _
                             & "Live_POG1,Live_POG2,Live_POG3,Live_POG4,Live_POG5," _
                             & "Live_POG6,Live_POG7,Live_POG8,Live_POG9,Live_POG10," _
                             & "Live_POG11,Live_POG12,Live_POG13," _
                             & "Live_POG14,Live_POG15,Live_POG16," _
                             & "Last_Price_Check_Date,Last_Price_Check_Price,Last_GAP_Date," _
                             & "OSSR_Flag,Label_Type," _
                             & "Current_Price, Deal_No1,Deal_No2," _
                             & "Deal_No3,Deal_No4,Deal_No5,Deal_No6,Deal_No7,Deal_No8," _
                             & "Deal_No9,Deal_No10,Item_Status0,Item_Status1," _
                             & "Item_Status3,Item_Status8,PH1_Price,PH1_Date," _
                             & "PH1_Type,PH2_Price,PH2_Date,PH2_Type,Pending_Price," _
                             & "Pending_Price_Date,Pending_Price_Type,Date_Last_Inc," _
                             & "Last_Deal,Last_Deal_Type,Last_Deal_Date,PSP_Flag) " _
                             & "VALUES (" & staData(0) & "," & staData(1) & "," _
                             & staData(2) & ",'" & staData(3).Trim() & "','" _
                             & staData(4) & "','" & staData(5) & "'," & staData(6) & ",'" _
                             & staData(7) & "','" & staData(8) & "','" & staData(9) & "','" _
                             & staData(10) & "'," & staData(11) & "," _
                             & staData(12) & ",'" & staData(13) & "','" _
                             & staData(14) & "','" & staData(15) & "'," _
                             & staData(16) & "," _
                             & staData(17) & "," & staData(18) & "," _
                             & staData(19) & "," & staData(20) & "," _
                             & staData(21) & "," & staData(22) & "," _
                             & staData(23) & "," & staData(24) & "," _
                             & staData(25) & "," & staData(26) & "," _
                             & staData(27) & "," & staData(28) & "," _
                             & staData(29) & "," & staData(30) & "," _
                             & staData(31) & "," & staData(32) & ",'" _
                             & staData(33) & "'," _
                             & staData(34) & ",'" & staData(35) & "','" _
                             & staData(36) & "','" & staData(37) & "'," _
                             & staData(38) & "," & staData(39) & "," _
                             & staData(40) & "," & staData(41) & "," _
                             & staData(42) & "," & staData(43) & "," _
                             & staData(44) & "," & staData(45) & "," _
                             & staData(46) & "," & staData(47) & "," _
                             & staData(48) & "," _
                             & staData(49) & "," & staData(50) & "," _
                             & staData(51) & "," & staData(52) & "," _
                             & staData(53) & ",'" & staData(54) & "','" _
                             & staData(55) & "'," & staData(56) & ",'" _
                             & staData(57) & "','" & staData(58) & "'," _
                             & staData(59) & ",'" & staData(60) & "','" _
                             & staData(61) & "','" & staData(62) & "'," _
                             & staData(63) & ",'" & staData(64) & "','" _
                             & staData(65) & "','" & staData(66) & "')"


                        'Assign SQL command text to the Sql Command
                        sqlCommd.CommandText = strInsertCmd

                        'Executing the querry
                        sqlCommd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1

                    Catch ex As Exception
                        'Log the duplicate record for reference.
                        If Not ex.Message.Contains("duplicate") Then
                            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseBootCodeFile:: Error:" _
                                                                  & "record found: " + strLine, _
                                                                  Logger.LogLevel.INFO)
                        End If
                        If Not bIsErrorLOGSend Then
                            'Duplicate DAta - Send a LOG Message
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, LOGTransmitter.Status.ABEND, _
                                                        LOGTransmitter.FileName.BOOTCODE, LOGTransmitter.Reasons.PKEY_Violation, iCount.ToString())

                            bIsErrorLOGSend = True
                        End If
                        'Increment the line count.
                        iCount = iCount + 1
                        'Read next line
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End Try
                ElseIf strLine.StartsWith("H") Then
                    If Not strLine.Contains(strFileName) Then
                        AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseBootCodeFile:: Invalid file format. The " _
                                                    & "file name in the header record is not matched.", _
                                                    Logger.LogLevel.RELEASE)
                        'Duplicate DATA - send a Log Message
                        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                               LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.BOOTCODE, LOGTransmitter.Reasons.Header_Name_Mismatch)
                        'return status
                        'Return False
                        bIsErrorLOGSend = True
                    End If
                End If
                'Read next line
                strLine = rdrFileReader.ReadLine()
            End While
            'close the reader
            rdrFileReader.Close()
            'Delete the reference file used by this function.
            File.Delete(strPath & strFileName)
            'End If
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseBootCodeFile::File parsing completed for BOOTCODE.CSV.", _
                                                            Logger.LogLevel.RELEASE)
            'AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
            '                                                     LOGTransmitter.Status.END_OK, LOGTransmitter.FileName.BOOTCODE, LOGTransmitter.Reasons.Parse_Complete)
            'Create index and statistics.
            CreateIndex("BootsCodeView", "Boots_Code")
            'CreateStatistics("BootsCodeView", "Boots_Code")
        Catch ex As FileNotFoundException
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseBootCodeFile::File BOOTCODE.CSV not found.", _
                                                            Logger.LogLevel.RELEASE)
            'File Not Found - send a LOG message
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, LOGTransmitter.Status.ABEND, _
                                    LOGTransmitter.FileName.BOOTCODE, LOGTransmitter.Reasons.File_Not_Found)
            'close the reader
            rdrFileReader.Close()
            'Return false in case of error
            Return False
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseBootCodeFile:: Exception occurred @ " & _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            'Error Occured - send a LOG message
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, LOGTransmitter.Status.ABEND, _
                                LOGTransmitter.FileName.BOOTCODE, LOGTransmitter.Reasons.Other_Errors)

            'Update the file index to the config file.
            BatchConfigParser.GetInstance().UpdateDBPopulationIndex(strFileName, iCount, ex.Message.ToString)
            Return False
        Finally
            rdrFileReader.Close()
            AppContainer.GetInstance.objRefDownloadForm.DBUpdationStop()
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        'If Successfull in parsing - send a LOG message
        If Not bIsErrorLOGSend Then
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseBootCodeFile::DB update successful for " + strFileName, _
                                                            Logger.LogLevel.RELEASE)
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                      LOGTransmitter.Status.END_OK, _
                                                                      LOGTransmitter.FileName.BOOTCODE, _
                                                                      LOGTransmitter.Reasons.Update_Success)
        Else
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.ABEND, _
                                                                  LOGTransmitter.FileName.BOOTCODE, _
                                                                  LOGTransmitter.Reasons.Update_Error)
        End If
        Return True
    End Function
    ''' <summary>
    '''  To parse barcode.csv and update the DB
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function ParseBarCode(ByVal strFileName As String, ByVal strPath As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        'To monitor LOG message status
        Dim bIsErrorLOGSend As Boolean = False
        'For reading barcode.csv file
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0
        Dim iPercentage As Integer = 10
        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseBarCode::File parsing started for BARCODE.CSV.", _
                                                        Logger.LogLevel.RELEASE)
        AppContainer.GetInstance.objRefDownloadForm.DBUpdationStart()
        'Sending a LOG message Barcode parse start
        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.START, _
                                                                  LOGTransmitter.FileName.BARCODE, _
                                                                  LOGTransmitter.Reasons.Parse_File)
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Load the file for reading.
            rdrFileReader = New System.IO.StreamReader(strPath + strFileName)

            'iFileIndex is set to a value when iFileIndex number of records 
            'is already inserted to the database.
            If iFileIndex > 0 Then
                iCount = iFileIndex
                iFileIndex = iFileIndex + 1
                For iLoopIndex As Integer = 1 To iFileIndex
                    'Reading a line from the CSV file
                    strLine = rdrFileReader.ReadLine()
                Next
            End If
            'Assign SQL connection to the command
            sqlCommd.Connection = sqlConn
            'Reading a line from the CSV file
            strLine = rdrFileReader.ReadLine()
            While Not strLine.StartsWith("T")
                strLine = strLine.Trim()
                If iCount = m_PercentageValue Then
                    AppContainer.GetInstance.objRefDownloadForm.SetDBUpdationstatus(iPercentage)
                    m_PercentageValue = m_PercentageValue + m_RowCount
                    iPercentage = iPercentage + 10
                End If
                If strLine.StartsWith("D") Then

                    strLine = strLine.Substring(2)
                    'Parsing the line
                    staData = strLine.Split(DELIMETER)
                    'Check for length of the data.
                    If staData.Length <> 2 Then
                        iCount = iCount + 1
                        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseBarCodeFile::Error - Length mismatch for BARCODE.CSV.", _
                                                    Logger.LogLevel.RELEASE)
                        If Not bIsErrorLOGSend Then
                            'Length Mismatch Error - send LOG message
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                               LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.BARCODE, LOGTransmitter.Reasons.Length_Mismatch)
                            bIsErrorLOGSend = True
                        End If
                        'Reading a line from the CSV file
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End If
                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = 0
                        End If
                    Next

                    'For inserting the parsed data to DB
                    'strInsertCmd = "INSERT INTO BarCodeView" _
                    '    & "(BarCode, Boots_Code, Current_Price, Deal_No1, Deal_No2, " _
                    '    & "Deal_No3, Deal_No4, Deal_No5, Deal_No6, Deal_No7, Deal_No8, " _
                    '    & "Deal_No9, Deal_No10, Item_Status0, Item_Status1," _
                    '    & "Item_Status3,Item_Status8, PH1_Price, PH1_Date, " _
                    '    & "PH1_Type, PH2_Price, PH2_Date,PH2_Type, Pending_Price," _
                    '    & "Pending_Price_Date, Pending_Price_Type,Date_Last_Inc," _
                    '    & " Last_Deal, Last_Deal_Type, Last_Deal_Date) " _
                    '    & "VALUES ('" & staData(0) & "','" & staData(12) & "','" _
                    '    & staData(1) & "','" & staData(2) & "','" & staData(3) & "','" _
                    '    & staData(4) & "','" & staData(5) & "','" & staData(6) & "','" _
                    '    & staData(7) & "','" & staData(8) & "','" & staData(9) & "','" _
                    '    & staData(10) & "','" & staData(11) & "','" & staData(13) & "','" _
                    '    & staData(14) & "','" & staData(15) & "','" & staData(16) & "','" _
                    '    & staData(17) & "','" & staData(18) & "','" & staData(19) & "','" _
                    '    & staData(20) & "','" & staData(21) & "','" & staData(22) & "','" _
                    '    & staData(23) & "','" & staData(24) & "','" & staData(25) & "','" _
                    '    & staData(26) & "','" & staData(27) & "','" & staData(28) & "','" _
                    '    & staData(29) & "')"

                    strInsertCmd = "INSERT INTO BarCodeView" _
                        & "(BarCode, Boots_Code) VALUES (" & staData(0) & "," & staData(1) & ")"
                    'Build the command text to the sql command
                    sqlCommd.CommandText = strInsertCmd
                    Try
                        'Executing the querry
                        sqlCommd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        'Log the duplicate record for reference.
                        If Not ex.Message.Contains("duplicate") Then
                            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseBarCodeFile:: Error:" _
                                                                  & "record found: " + strLine, _
                                                                  Logger.LogLevel.INFO)
                        End If
                        If Not bIsErrorLOGSend Then
                            'duplicate record send a LOG message
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                                     LOGTransmitter.Status.ABEND, _
                                                                                     LOGTransmitter.FileName.BARCODE, _
                                                                                     LOGTransmitter.Reasons.PKEY_Violation, _
                                                                                     iCount.ToString())

                            bIsErrorLOGSend = True
                        End If
                        'Increment the line count.
                        iCount = iCount + 1
                        'Read next line
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End Try
                ElseIf strLine.StartsWith("H") Then
                    If Not strLine.Contains(strFileName) Then
                        AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseBarCodeFile:: Invalid file format. The " _
                                                    & "file name in the header record is not matched.", _
                                                    Logger.LogLevel.RELEASE)
                        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                               LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.BARCODE, LOGTransmitter.Reasons.Header_Name_Mismatch)

                        'return status
                        'Return False
                        bIsErrorLOGSend = True
                    End If
                End If
                'Read next line
                strLine = rdrFileReader.ReadLine()
            End While
            'Close the file reader.
            rdrFileReader.Close()
            'Delete the reference file used by this function.
            File.Delete(strPath & strFileName)
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseBarCode::File parsing completed for BARCODE.CSV.", _
                                                            Logger.LogLevel.RELEASE)
            'AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
            '                                                     LOGTransmitter.Status.END_OK, LOGTransmitter.FileName.BARCODE, LOGTransmitter.Reasons.Parse_Complete)
            'Create index and statistics.
            'CreateIndex("BarCodeView", "BarCode,Boots_Code")
            'CreateStatistics("BarCodeView", "BarCode")
        Catch ex As FileNotFoundException
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseBarCode::File BARCODE.CSV not found.", _
                                                  Logger.LogLevel.RELEASE)
            'File Not found - send a LOG message
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                 LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.BARCODE, LOGTransmitter.Reasons.File_Not_Found)

            'close the reader
            rdrFileReader.Close()
            'Return false in case of error
            Return False
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParse:: ParseBarCodeFile:: Exception occurred @" & _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            'Exception occured - send LOG File
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                 LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.BARCODE, LOGTransmitter.Reasons.Other_Errors)

            'Update the file index to the config file.
            BatchConfigParser.GetInstance().UpdateDBPopulationIndex(strFileName, iCount, ex.Message.ToString)
            Return False
        Finally
            AppContainer.GetInstance.objRefDownloadForm.DBUpdationStop()
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        'Successfull, send a end LOG message
        If Not bIsErrorLOGSend Then
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseBarCode::DB update successful for " + strFileName, _
                                                            Logger.LogLevel.RELEASE)
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                      LOGTransmitter.Status.END_OK, _
                                                                      LOGTransmitter.FileName.BARCODE, _
                                                                      LOGTransmitter.Reasons.Update_Success)
        Else
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.ABEND, _
                                                                  LOGTransmitter.FileName.BARCODE, _
                                                                  LOGTransmitter.Reasons.Update_Error)
        End If
        Return True
    End Function
    ''' <summary>
    ''' To parse DEAL.CSV file and update DB
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function ParseDealFile(ByVal strFileName As String, ByVal strPath As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        'To monitor LOG message status
        Dim bIsErrorLOGSend As Boolean = False
        'For reading DEAL.csv file
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0
        Dim iPercentage As Integer = 10
        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseDealFile::File parsing started for DEAL.CSV.", _
                                                        Logger.LogLevel.RELEASE)
        AppContainer.GetInstance.objRefDownloadForm.DBUpdationStart()
        'Starting to parse - send a LOG message
        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.START, _
                                                                  LOGTransmitter.FileName.DEAL, _
                                                                  LOGTransmitter.Reasons.Parse_File)
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Load the file for reading.
            rdrFileReader = New System.IO.StreamReader(strPath + strFileName)

            'iFileIndex is set to a value when iFileIndex number of records 
            'is already inserted to the database.
            If iFileIndex > 0 Then
                iCount = iFileIndex
                iFileIndex = iFileIndex + 2
                For iLoopIndex As Integer = 1 To iFileIndex
                    'Reading a line from the CSV file
                    strLine = rdrFileReader.ReadLine()
                Next
            End If
            'Assign SQL connection to the command
            sqlCommd.Connection = sqlConn
            'Reading a line from the CSV file
            strLine = rdrFileReader.ReadLine()

            While Not strLine.StartsWith("T")
                strLine = strLine.Trim()
                If iCount = m_PercentageValue Then
                    AppContainer.GetInstance.objRefDownloadForm.SetDBUpdationstatus(iPercentage)
                    m_PercentageValue = m_PercentageValue + m_RowCount
                    iPercentage = iPercentage + 10
                End If
                If strLine.StartsWith("D") Then

                    strLine = strLine.Substring(2)
                    'Parsing the line
                    staData = strLine.Split(DELIMETER)

                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = 0
                        End If
                    Next

                    'Minu Removed single quotes from the query for integer values.
                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO DealList " _
                        & "(Deal_Number,Deal_Type,Start_Date,End_Date)" _
                        & " VALUES (" & staData(0) & ",'" & staData(1) & "','" _
                        & staData(2) & "','" & staData(3) & "')"

                    sqlCommd.CommandText = strInsertCmd
                    'Executing the querry
                    Try
                        'Executing the querry
                        sqlCommd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        'Log the duplicate record for reference.
                        If Not ex.Message.Contains("duplicate") Then
                            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseDealFile::Error:" _
                                                                  & "record found " + strLine, _
                                                                  Logger.LogLevel.INFO)
                        End If
                        If Not bIsErrorLOGSend Then
                            'error record found - send a LOG message
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                                      LOGTransmitter.Status.ABEND, _
                                                                                      LOGTransmitter.FileName.DEAL, _
                                                                                      LOGTransmitter.Reasons.PKEY_Violation, _
                                                                                     iCount.ToString())
                            bIsErrorLOGSend = True
                        End If
                        'Increment the line count.
                        iCount = iCount + 1

                        'Read next line
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End Try
                ElseIf strLine.StartsWith("H") Then
                    If Not strLine.Contains(strFileName) Then
                        AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseDealFile::Invalid file format. The " _
                                                    & "file name in the header record is not matched.", _
                                                    Logger.LogLevel.RELEASE)
                        'Header Name Mismatch - sending a LOG message
                        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                                 LOGTransmitter.Status.ABEND, _
                                                                                 LOGTransmitter.FileName.DEAL, _
                                                                                 LOGTransmitter.Reasons.Header_Name_Mismatch)
                        'return status
                        'Return False
                        bIsErrorLOGSend = True
                    End If
                End If
                'Read next line
                strLine = rdrFileReader.ReadLine()
            End While
            'Close the file reader.
            rdrFileReader.Close()
            'Delete the reference file used by this function.
            File.Delete(strPath & strFileName)
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseDealFile::File parsing completed for DEAL.CSV.", _
                                                            Logger.LogLevel.RELEASE)
            'AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
            '                                                    LOGTransmitter.Status.END_OK, LOGTransmitter.FileName.DEAL, LOGTransmitter.Reasons.Parse_Complete)
            'Create index and statistics.
            CreateIndex("DealList", "Deal_Number")
            'CreateStatistics("DealList", "Deal_Number")
        Catch ex As FileNotFoundException
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseDealFile::File DEAL.CSV not found.", _
                                                            Logger.LogLevel.RELEASE)
            'File Not Found exception - send a LOG message
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                         LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.DEAL, LOGTransmitter.Reasons.File_Not_Found)
            'close the reader
            rdrFileReader.Close()
            'Return false in case of error
            Return False
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseDealFile:: Exception occurred @ " & _
                                                            ex.StackTrace, Logger.LogLevel.RELEASE)
            'Error Occured - send a LOG Message 
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                         LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.DEAL, LOGTransmitter.Reasons.Other_Errors)
            'Update the file index to the config file.
            BatchConfigParser.GetInstance().UpdateDBPopulationIndex(strFileName, iCount, ex.Message.ToString)
            Return False
        Finally
            rdrFileReader.Close()
            AppContainer.GetInstance.objRefDownloadForm.DBUpdationStop()
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        'Success Send a Log Message
        If Not bIsErrorLOGSend Then
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseDealFile::DB update successful for " + strFileName, _
                                                            Logger.LogLevel.RELEASE)
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                      LOGTransmitter.Status.END_OK, _
                                                                      LOGTransmitter.FileName.DEAL, _
                                                                      LOGTransmitter.Reasons.Update_Success)
        Else
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.ABEND, _
                                                                  LOGTransmitter.FileName.DEAL, _
                                                                  LOGTransmitter.Reasons.Update_Error)
        End If
        Return True
    End Function
    ''' <summary>
    ''' To parse PGROUP.csv and update the DB
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ParsePGroupFile(ByVal strFileName As String, ByVal strPath As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        'To monitor LOG message status
        Dim bIsErrorLOGSend As Boolean = False
        'For reading bootcode.csv file
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0
        Dim iPercentage As Integer = 10
        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParsePGroupFile::File parsing started for PGROUP.CSV.", _
                                                        Logger.LogLevel.RELEASE)
        AppContainer.GetInstance.objRefDownloadForm.DBUpdationStart()
        'Parse start - send LOG message
        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.START, _
                                                                  LOGTransmitter.FileName.PGROUP, _
                                                                  LOGTransmitter.Reasons.Parse_File)
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Load the file for reading.
            rdrFileReader = New System.IO.StreamReader(strPath + strFileName)

            'iFileIndex is set to a value when iFileIndex number of records 
            'is already inserted to the database.
            If iFileIndex > 0 Then
                iCount = iFileIndex
                iFileIndex = iFileIndex + 2
                For iLoopIndex As Integer = 1 To iFileIndex
                    'Reading a line from the CSV file
                    strLine = rdrFileReader.ReadLine()
                Next
            End If

            'Assign SQL connection to the command
            sqlCommd.Connection = sqlConn
            'Reading a line from the CSV file
            strLine = rdrFileReader.ReadLine()

            While Not strLine.StartsWith("T")
                strLine = strLine.Trim()
                If iCount = m_PercentageValue Then
                    AppContainer.GetInstance.objRefDownloadForm.SetDBUpdationstatus(iPercentage)
                    m_PercentageValue = m_PercentageValue + m_RowCount
                    iPercentage = iPercentage + 10
                End If
                If strLine.StartsWith("D") Then

                    strLine = strLine.Substring(2)
                    'Parsing the line
                    staData = strLine.Split(DELIMETER)

                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = 0
                        End If
                    Next
                    'Escape the single quotes characters if any
                    staData(1) = staData(1).Replace("'", "''")
                    'Minu Removed single quotes from the query for integer values.
                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO ProductGroup " _
                        & "(Product_Grp_No,Product_Grp_Desc,Product_Grp_Flag)" _
                        & " VALUES (" & staData(0) & ",'" & staData(1) & "','" _
                        & staData(2) & "')"

                    sqlCommd.CommandText = strInsertCmd
                    'Executing the querry
                    Try
                        'Executing the querry
                        sqlCommd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        'Log the duplicate record for reference.
                        If Not ex.Message.Contains("duplicate") Then
                            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParsePGroupFile:: Error:" _
                                                                  & "record found " + strLine, _
                                                                  Logger.LogLevel.INFO)
                        End If
                        If Not bIsErrorLOGSend Then
                            'Duplicate records - send log filess
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                                     LOGTransmitter.Status.ABEND, _
                                                                                     LOGTransmitter.FileName.PGROUP, _
                                                                                     LOGTransmitter.Reasons.PKEY_Violation, _
                                                                                     iCount.ToString())
                            bIsErrorLOGSend = True
                        End If
                        'Increment the line count.
                        iCount = iCount + 1
                        'Read next line
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End Try
                ElseIf strLine.StartsWith("H") Then
                    If Not strLine.Contains(strFileName) Then
                        AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParsePGroupFile:: Invalid file format. The " _
                                                    & "file name in the header record is not matched.", _
                                                    Logger.LogLevel.RELEASE)
                        'Header Name Mismatch - send LOG Files
                        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                       LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.PGROUP, LOGTransmitter.Reasons.Header_Name_Mismatch)
                        bIsErrorLOGSend = True
                    End If
                End If
                'Read next line
                strLine = rdrFileReader.ReadLine()
            End While
            'Close the file reader.
            rdrFileReader.Close()
            'Delete the reference file used by this function.
            File.Delete(strPath & strFileName)
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParsePGroupFile::File parsing completed for PGROUP.CSV.", _
                                                            Logger.LogLevel.RELEASE)
            'AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
            '                                                    LOGTransmitter.Status.END_OK, LOGTransmitter.FileName.PGROUP, LOGTransmitter.Reasons.Parse_Complete)
            'Create index and statistics.
            CreateIndex("ProductGroup", "Product_Grp_No")
            'CreateStatistics("ProductGroup", "Product_Grp_No")
        Catch ex As FileNotFoundException
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParsePGroupFile::File" & strFileName & " - not found.", _
                                                  Logger.LogLevel.RELEASE)
            'File Not Found Exception - send LOG Message
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                         LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.PGROUP, LOGTransmitter.Reasons.File_Not_Found)
            'close the reader
            rdrFileReader.Close()
            'Return false in case of error
            Return False
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParsePGroupFile:: Exception occurred @ " & _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            'Error occured - send log message
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                         LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.PGROUP, LOGTransmitter.Reasons.Other_Errors)
            'Update the file index to the config file.
            BatchConfigParser.GetInstance().UpdateDBPopulationIndex(strFileName, iCount, ex.Message.ToString)
            Return False
        Finally
            rdrFileReader.Close()
            AppContainer.GetInstance.objRefDownloadForm.DBUpdationStop()
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        'Successful Send a Log Message
        If Not bIsErrorLOGSend Then
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParsePGroupFile::DB update successful for " + strFileName, _
                                                            Logger.LogLevel.RELEASE)
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                      LOGTransmitter.Status.END_OK, _
                                                                      LOGTransmitter.FileName.PGROUP, _
                                                                      LOGTransmitter.Reasons.Update_Success)
        Else
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.ABEND, _
                                                                  LOGTransmitter.FileName.PGROUP, _
                                                                  LOGTransmitter.Reasons.Update_Error)
        End If
        Return True
    End Function
    ''' <summary>
    '''  To parse SUPPLIER.csv and update the DB
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function ParseSupplierFile(ByVal strFileName As String, ByVal strPath As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        'To monitor LOG message status
        Dim bIsErrorLOGSend As Boolean = False
        'For reading bootcode.csv file
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0
        Dim iPercentage As Integer = 10
        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseSupplierFile::File parsing started for SUPPLIER.CSV.", _
                                                        Logger.LogLevel.RELEASE)
        AppContainer.GetInstance.objRefDownloadForm.DBUpdationStart()
        'Send  a log message
        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.START, _
                                                                  LOGTransmitter.FileName.SUPPLIER, _
                                                                  LOGTransmitter.Reasons.Parse_File)
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Load the file for reading.
            rdrFileReader = New System.IO.StreamReader(strPath + strFileName)

            'iFileIndex is set to a value when iFileIndex number of records 
            'is already inserted to the database.
            If iFileIndex > 0 Then
                iCount = iFileIndex
                iFileIndex = iFileIndex + 2
                For iLoopIndex As Integer = 1 To iFileIndex
                    'Reading a line from the CSV file
                    strLine = rdrFileReader.ReadLine()
                Next
            End If
            'Assign SQL connection to the command
            sqlCommd.Connection = sqlConn
            'Reading a line from the CSV file
            strLine = rdrFileReader.ReadLine()
            'Run through the entire file
            While Not strLine.StartsWith("T")
                strLine = strLine.Trim()
                If iCount = m_PercentageValue Then
                    AppContainer.GetInstance.objRefDownloadForm.SetDBUpdationstatus(iPercentage)
                    m_PercentageValue = m_PercentageValue + m_RowCount
                    iPercentage = iPercentage + 10
                End If
                If strLine.StartsWith("D") Then
                    strLine = strLine.Substring(2)
                    'Parsing the line
                    staData = strLine.Split(DELIMETER)

                    'To add 0s inplace of null values
                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = 0
                        End If
                    Next
                    'Escape the single quotes characters if any
                    staData(2) = staData(2).Replace("'", "''")
                    'Check for lenght of the data.
                    If staData.Length <> 5 Then
                        iCount = iCount + 1
                        If Not bIsErrorLOGSend Then
                            'Mismatch - send LOG
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                              LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.SUPPLIER, LOGTransmitter.Reasons.Length_Mismatch)
                            bIsErrorLOGSend = True
                        End If
                        'Reading a line from the CSV file
                        strLine = rdrFileReader.ReadLine()
                    End If
                    'Minu Removed single quotes from the query for integer values.
                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO SuppliersList " _
                        & "(Supplier_ID,Supplier_Name,Supplier_ASN_Flag,Supplier_Static_Flag)" _
                        & " VALUES (" & staData(1) & ",'" & staData(2) & "','" _
                        & staData(3) & "','" & staData(4) & "')"

                    sqlCommd.CommandText = strInsertCmd
                    'Executing the querry
                    Try
                        'Executing the querry
                        sqlCommd.ExecuteNonQuery()
                        'Check if supplier BC is not nothing
                        If Not staData(0).Equals("0") Then
                            UpdateSupplierBC(staData(1), staData(0), sqlCommd)
                        End If

                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        If ex.Message.Contains("duplicate") Then
                            'Check if supplier BC is not nothing
                            If Not staData(0).Equals("0") Then
                                UpdateSupplierBC(staData(1), staData(0), sqlCommd)
                            End If
                        End If
                        If Not (bIsErrorLOGSend) Then
                            'There is a duplicate item and no Supplier BC is updated
                            'So sending a  LOG message for PKEY Violation
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                               LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.SUPPLIER, LOGTransmitter.Reasons.PKEY_Violation, _
                                                                                     iCount.ToString())
                            bIsErrorLOGSend = True
                        End If
                        'Increment the line count.
                        iCount = iCount + 1
                        'Read next line
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End Try
                ElseIf strLine.StartsWith("H") Then
                    If Not strLine.Contains(strFileName) Then
                        AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseSupplierFile:: Invalid file format. The " _
                                                    & "file name in the header record is not matched.", _
                                                    Logger.LogLevel.RELEASE)
                        'Header Name Mismatch - sending LOG message
                        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                     LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.SUPPLIER, LOGTransmitter.Reasons.Header_Name_Mismatch)
                        'return status
                        'Return False
                        bIsErrorLOGSend = True
                    End If
                End If
                'Read next line
                strLine = rdrFileReader.ReadLine()
            End While
            'Close the file reader.
            rdrFileReader.Close()
            'Delete the reference file used by this function.
            File.Delete(strPath & strFileName)
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseSupplierFile::File parsing completed for SUPPLIER.CSV.", _
                                                            Logger.LogLevel.RELEASE)
            'Create index and statistics.
            CreateIndex("SuppliersList", "Supplier_ID")
            'CreateStatistics("SuppliersList", "Supplier_ID")
        Catch ex As FileNotFoundException
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseSupplierFile::File" & strFileName & " - not found.", _
                                                  Logger.LogLevel.RELEASE)
            'File Not Found - send LOG Message
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                          LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.SUPPLIER, LOGTransmitter.Reasons.File_Not_Found)
            'close the reader
            rdrFileReader.Close()
            'Return false in case of error
            Return False
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseSupplierFile:: Exception occurred @ " & _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            'Error - send a LOG
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                          LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.SUPPLIER, LOGTransmitter.Reasons.Other_Errors)
            'Update the file index to the config file.
            BatchConfigParser.GetInstance().UpdateDBPopulationIndex(strFileName, iCount, ex.Message.ToString)
            Return False
        Finally
            rdrFileReader.Close()
            AppContainer.GetInstance.objRefDownloadForm.DBUpdationStop()
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        'Successful send a LOG message
        If Not bIsErrorLOGSend Then
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseSupplierFile::DB update successful for " + strFileName, _
                                                            Logger.LogLevel.RELEASE)
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                      LOGTransmitter.Status.END_OK, _
                                                                      LOGTransmitter.FileName.SUPPLIER, _
                                                                      LOGTransmitter.Reasons.Update_Success)
        Else
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.ABEND, _
                                                                  LOGTransmitter.FileName.SUPPLIER, _
                                                                  LOGTransmitter.Reasons.Update_Error)
        End If
        Return True
    End Function
    Private Sub UpdateSupplierBC(ByVal strSupplierID As String, ByVal strSupplierBC As String, ByVal sqlCmd As SqlCeCommand)
        'Insert data into Supplier BC table if a Supplier 
        'row has BC.
        Dim strInsertCmd = "INSERT INTO SupplierBC " _
        & "(Supplier_ID,Supplier_BC)" _
        & " VALUES (" _
        & strSupplierID & ",'" & strSupplierBC & "')"
        'Assign the command to be executed.
        sqlCmd.CommandText = strInsertCmd
        'Executing the querry
        sqlCmd.ExecuteNonQuery()
    End Sub
    ''' <summary>
    ''' To parse USERS.csv and update the DB
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ParseUsersFile(ByVal strFileName As String, ByVal strPath As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        'To monitor LOG message status
        Dim bIsErrorLOGSend As Boolean = False
        'For reading USERS.csv file
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0
        Dim iPercentage As Integer = 10
        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseUsersFile::File parsing started for USERS.CSV.", _
                                                        Logger.LogLevel.RELEASE)
        AppContainer.GetInstance.objRefDownloadForm.DBUpdationStart()
        'Send a LOG message
        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                   LOGTransmitter.Status.START, _
                                                                   LOGTransmitter.FileName.USERS, _
                                                                   LOGTransmitter.Reasons.Parse_File)
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Load the file for reading.
            rdrFileReader = New System.IO.StreamReader(strPath + strFileName)

            'iFileIndex is set to a value when iFileIndex number of records 
            'is already inserted to the database.
            If iFileIndex > 0 Then
                iCount = iFileIndex
                iFileIndex = iFileIndex + 2
                For iLoopIndex As Integer = 1 To iFileIndex
                    'Reading a line from the CSV file
                    strLine = rdrFileReader.ReadLine()
                Next
            End If
            'Assign SQL connection to the command
            sqlCommd.Connection = sqlConn
            'Reading a line from the CSV file
            strLine = rdrFileReader.ReadLine()
            'Run through all the lines of the file.
            While Not strLine.StartsWith("T")
                strLine = strLine.Trim()
                If iCount = m_PercentageValue Then
                    AppContainer.GetInstance.objRefDownloadForm.SetDBUpdationstatus(iPercentage)
                    m_PercentageValue = m_PercentageValue + m_RowCount
                    iPercentage = iPercentage + 10
                End If
                If strLine.StartsWith("D") Then

                    strLine = strLine.Substring(2)
                    'Parsing the line
                    staData = strLine.Split(DELIMETER)
                    'To assign 0s inplace of null values.
                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = 0
                        End If
                    Next
                    'Escape the single quotes characters if any
                    staData(2) = staData(2).Replace("'", "''")

                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO UserList " _
                        & "(User_ID,Password,User_Name,Supervisor_Flag,Stock_Adjustment_Flag)" _
                        & " VALUES (" & staData(0) & "," & staData(1) & ",'" _
                        & staData(2) & "','" & staData(3) & "','" & staData(4) & "')"

                    sqlCommd.CommandText = strInsertCmd
                    'Executing the querry
                    Try
                        'Executing the querry
                        sqlCommd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        'Log the duplicate record for reference.
                        If Not ex.Message.Contains("duplicate") Then
                            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseUsersFile:: Error: " _
                                                                  & "record found " + strLine, _
                                                                  Logger.LogLevel.INFO)
                        End If
                        If Not bIsErrorLOGSend Then
                            'send a LOG message
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                    LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.USERS, LOGTransmitter.Reasons.PKEY_Violation, _
                                                                                     iCount.ToString())
                            bIsErrorLOGSend = True
                        End If
                        'Increment the line count.
                        iCount = iCount + 1
                        'Read next line
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End Try
                ElseIf strLine.StartsWith("H") Then
                    If Not strLine.Contains(strFileName) Then
                        AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseUsersFile:: Invalid file format. The " _
                                                    & "file name in the header record is not matched.", _
                                                    Logger.LogLevel.RELEASE)
                        'Send a LOG message
                        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                        LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.USERS, LOGTransmitter.Reasons.Header_Name_Mismatch)
                        bIsErrorLOGSend = True
                    End If
                End If

                'Read next line
                strLine = rdrFileReader.ReadLine()
            End While
            'Close the file reader.
            rdrFileReader.Close()
            'Delete the reference file used by this function.
            File.Delete(strPath & strFileName)
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseUsersFile:: File parsing completed for USERS.CSV.", _
                                                        Logger.LogLevel.RELEASE)
            'Create index and statistics.
            CreateIndex("UserList", "User_ID")
            'CreateStatistics("UserList", "User_ID")
        Catch ex As FileNotFoundException
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseUsersFile:: File" & strFileName & " - not found.", _
                                                  Logger.LogLevel.RELEASE)
            'File Not Found - send a Log message
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                      LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.USERS, LOGTransmitter.Reasons.File_Not_Found)
            'close the reader
            rdrFileReader.Close()
            'Return false in case of error
            Return False
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseUsersFile:: Exception occurred @ " & _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            'Error - send a LOG message
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                      LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.USERS, LOGTransmitter.Reasons.Other_Errors)
            'Update the file index to the config file.
            BatchConfigParser.GetInstance().UpdateDBPopulationIndex(strFileName, iCount, ex.Message.ToString)
            Return False
        Finally
            rdrFileReader.Close()
            AppContainer.GetInstance.objRefDownloadForm.DBUpdationStop()
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        If Not bIsErrorLOGSend Then
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseUsersFile::DB update successful for " + strFileName, _
                                                            Logger.LogLevel.RELEASE)
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                      LOGTransmitter.Status.END_OK, _
                                                                      LOGTransmitter.FileName.USERS, _
                                                                      LOGTransmitter.Reasons.Update_Success)
        Else
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.ABEND, _
                                                                  LOGTransmitter.FileName.USERS, _
                                                                  LOGTransmitter.Reasons.Update_Error)
        End If
        Return True
    End Function
    ''' <summary>
    '''  To parse RECALL.csv and update the DB
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ParseRecallFile(ByVal strFileName As String, ByVal strPath As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        'To monitor LOG message status
        Dim bIsErrorLOGSend As Boolean = False
        'For reading RECALL.csv file
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0
        Dim iPercentage As Integer = 10
        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseRecallFile::File parsing started for RECALL.CSV.", _
                                                        Logger.LogLevel.RELEASE)
        AppContainer.GetInstance.objRefDownloadForm.DBUpdationStart()
        'Parse started - Send LOG Message
        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.START, _
                                                                  LOGTransmitter.FileName.RECALL, _
                                                                  LOGTransmitter.Reasons.Parse_File)
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Load the file for reading.
            rdrFileReader = New System.IO.StreamReader(strPath + strFileName)

            'iFileIndex is set to a value when iFileIndex number of records 
            'is already inserted to the database.
            If iFileIndex > 0 Then
                iCount = iFileIndex
                iFileIndex = iFileIndex + 2
                For iLoopIndex As Integer = 1 To iFileIndex
                    'Reading a line from the CSV file
                    strLine = rdrFileReader.ReadLine()
                Next
            End If
            'Assign SQL connection to the command
            sqlCommd.Connection = sqlConn
            'Reading a line from the CSV file
            strLine = rdrFileReader.ReadLine()
            'Run through the entire file
            While Not strLine.StartsWith("T")
                strLine = strLine.Trim()
                If iCount = m_PercentageValue Then
                    AppContainer.GetInstance.objRefDownloadForm.SetDBUpdationstatus(iPercentage)
                    m_PercentageValue = m_PercentageValue + m_RowCount
                    iPercentage = iPercentage + 10
                End If
                If strLine.StartsWith("D") Then

                    strLine = strLine.Substring(2)
                    'Parsing the line
                    staData = strLine.Split(DELIMETER)
                    'Check for length of the data.
                    'CR for DEFECT 4982,2957 (batch nos and min Recall qty)
                    If staData.Length <> 12 Then
                        iCount = iCount + 1
                        If Not bIsErrorLOGSend Then
                            'Mismatch - send Log message
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                              LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.RECALL, LOGTransmitter.Reasons.Length_Mismatch)
                            bIsErrorLOGSend = True
                        End If
                        'Reading a line from the CSV file
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End If
                    'Escape the single quotes characters if any
                    staData(2) = staData(2).Replace("'", "''")
                    staData(7) = staData(7).Replace("'", "''")
                    If staData(11).Trim(" ") = "" Then
                        staData(11) = "0"
                    End If
                    'To replace the null fields with 0s
                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = 0
                        End If
                    Next
                    'Minu Replacing date with default date
                    If Val(staData(4)) = 0 Then
                        staData(4) = DEFAULT_DATE
                    End If
                    If Val(staData(6)) = 0 Then
                        staData(6) = DEFAULT_DATE
                    End If
                    'CR for DEFECT 4982,2957 (batch nos and min Recall qty)
                    'For inserting the parsed data to DB
                    'Minu Removed single quotes from the query for integer values.
                    strInsertCmd = "INSERT INTO RecallList " _
                        & "(Recall_Number,Recall_Type,Recall_Desc,Recall_Qty,Completion_Date," _
                        & "Business_Centre,Recall_Active_Date,Recall_Message,Status,UOD_Label_Type,Batch_Nos,Min_Return_Qty)" _
                        & " VALUES (" & staData(0) & ",'" & staData(1) & "','" _
                        & staData(2) & "'," & staData(3) & ",'" & staData(4) _
                        & "','" & staData(5) & "','" & staData(6) & "','" & staData(7) _
                        & "','" & staData(8) & "','" & staData(9) & "','" & staData(10) & "'," & staData(11) & ")"

                    sqlCommd.CommandText = strInsertCmd
                    'Executing the querry
                    Try
                        'Executing the querry
                        sqlCommd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        'Log the duplicate record for reference.
                        If Not ex.Message.Contains("duplicate") Then
                            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseRecallFile:: Error: " _
                                                                  & "record found " + strLine, _
                                                                  Logger.LogLevel.INFO)
                        End If
                        If Not bIsErrorLOGSend Then
                            'Duplicate records - send LOG
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                                       LOGTransmitter.Status.ABEND, _
                                                                                       LOGTransmitter.FileName.RECALL, _
                                                                                       LOGTransmitter.Reasons.PKEY_Violation, _
                                                                                     iCount.ToString())
                            bIsErrorLOGSend = True
                        End If
                        'Increment the line count.
                        iCount = iCount + 1
                        'Read next line
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End Try
                ElseIf strLine.StartsWith("I") Then
                    strLine = strLine.Substring(2)
                    staData = strLine.Split(DELIMETER)

                    'To insert recall items present in the same line.
                    'Updated the step count to insert item status as well.
                    For iIndex = 1 To staData.GetUpperBound(0) Step 2
                        Try
                            'For inserting the parsed data to DB
                            'Updated the query for change in the file structure.
                            If staData(iIndex).Trim() <> Nothing And _
                                                staData(iIndex).Trim() <> "" Then
                                strInsertCmd = "INSERT INTO RecallListItems " _
                                    & "(Recall_Number,Boots_Code,Recall_Status)" _
                                   & " VALUES (" & staData(0) & "," _
                                   & staData(iIndex) & ",'" & staData(iIndex + 1) & "')"
                                '& GenerateBCwithCDV(staData(iIndex)) & "')"

                                sqlCommd.CommandText = strInsertCmd
                                'Executing the querry

                                'Executing the querry
                                sqlCommd.ExecuteNonQuery()
                            End If
                        Catch ex As Exception
                            'Log the duplicate record for reference.
                            If Not ex.Message.Contains("duplicate") Then
                                AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseRecallFile::  Error:" _
                                                                      & "record found " + strLine, _
                                                                      Logger.LogLevel.INFO)
                            End If
                            If Not bIsErrorLOGSend Then
                                AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                                        LOGTransmitter.Status.ABEND, _
                                                                                        LOGTransmitter.FileName.RECALL, _
                                                                                        LOGTransmitter.Reasons.PKEY_Violation, _
                                                                                     iCount.ToString())
                                bIsErrorLOGSend = True
                            End If
                            'Read next line
                            'strLine = rdrFileReader.ReadLine()
                            Continue For
                        End Try
                    Next
                    'Counter to keep track of the record number inserted to the db.
                    iCount = iCount + 1
                ElseIf strLine.StartsWith("H") Then
                    If Not strLine.Contains(strFileName) Then
                        AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseRecallFile:: Invalid file format. The " _
                                                    & "file name in the header record is not matched.", _
                                                    Logger.LogLevel.RELEASE)
                        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                             LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.RECALL, LOGTransmitter.Reasons.Header_Name_Mismatch)
                        bIsErrorLOGSend = True
                    End If
                End If
                'Read next line
                strLine = rdrFileReader.ReadLine()
            End While
            'close the reader
            rdrFileReader.Close()
            'Delete the reference file used by this function.
            File.Delete(strPath & strFileName)
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseRecallFile::File parsing completed for RECALL.CSV.", _
                                                            Logger.LogLevel.RELEASE)
            'Create index and statistics.
            CreateIndex("RecallList", "Recall_Number")
            'CreateStatistics("RecallList", "Recall_Number")
            'Create index and statistics.
            CreateIndex("RecallListItems", "Recall_Number,Boots_Code")
            'CreateStatistics("RecallListItems", "Recall_Number,Boots_Code")
        Catch ex As FileNotFoundException
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseRecallFile::File" & strFileName & " - not found.", _
                                                  Logger.LogLevel.RELEASE)
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                           LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.RECALL, LOGTransmitter.Reasons.File_Not_Found)
            'close the reader
            rdrFileReader.Close()
            'Return false in case of error
            Return False
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseRecallFile::  Exception occurred @ " & _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                           LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.RECALL, LOGTransmitter.Reasons.Other_Errors)
            'Update the file index to the config file.
            BatchConfigParser.GetInstance().UpdateDBPopulationIndex(strFileName, iCount, ex.Message.ToString)
            Return False
        Finally
            rdrFileReader.Close()
            AppContainer.GetInstance.objRefDownloadForm.DBUpdationStop()
        End Try
        'true If successfully inserted the entire list of file contents to DB
        'Succesful - sendd a end LOg message
        If Not bIsErrorLOGSend Then
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseRecallFile::DB update successful for " + strFileName, _
                                                            Logger.LogLevel.RELEASE)
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                      LOGTransmitter.Status.END_OK, _
                                                                      LOGTransmitter.FileName.RECALL, _
                                                                      LOGTransmitter.Reasons.Update_Success)
        Else
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.ABEND, _
                                                                  LOGTransmitter.FileName.RECALL, _
                                                                  LOGTransmitter.Reasons.Update_Error)
        End If
        Return True
    End Function
    ''' <summary>
    ''' To parse LIVEPOG.csv and update the DB
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function ParseLivePOGFile(ByVal strFileName As String, ByVal strPath As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        'To monitor LOG message status
        Dim bIsErrorLOGSend As Boolean = False
        'For reading bootcode.csv file
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0
        Dim iPercentage As Integer = 10
        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseLivePOGFile::File parsing started for LIVEPOG.CSV.", _
                                                        Logger.LogLevel.RELEASE)
        AppContainer.GetInstance.objRefDownloadForm.DBUpdationStart()
        'Starting to parse - send a LOG message
        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.START, _
                                                                  LOGTransmitter.FileName.LIVEPOG, _
                                                                  LOGTransmitter.Reasons.Parse_File)
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Load the file for reading.
            rdrFileReader = New System.IO.StreamReader(strPath + strFileName)

            'iFileIndex is set to a value when iFileIndex number of records 
            'is already inserted to the database.
            If iFileIndex > 0 Then
                iCount = iFileIndex
                iFileIndex = iFileIndex + 2
                For iLoopIndex As Integer = 1 To iFileIndex
                    'Reading a line from the CSV file
                    strLine = rdrFileReader.ReadLine()
                Next
            End If
            'Assign SQL connection to the command
            sqlCommd.Connection = sqlConn
            'Reading a line from the CSV file
            strLine = rdrFileReader.ReadLine()
            'Run throuhg the entire file records.
            While Not strLine.StartsWith("T")
                strLine = strLine.Trim()
                If iCount = m_PercentageValue Then
                    AppContainer.GetInstance.objRefDownloadForm.SetDBUpdationstatus(iPercentage)
                    m_PercentageValue = m_PercentageValue + m_RowCount
                    iPercentage = iPercentage + 10
                End If
                If strLine.StartsWith("D") Then

                    strLine = strLine.Substring(2)
                    'Parsing the line
                    staData = strLine.Split(DELIMETER)
                    'Check for length of the data.
                    If staData.Length <> 8 Then
                        iCount = iCount + 1
                        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseLivePOGFile::Error - Length mismatch: " & strLine, _
                                                    Logger.LogLevel.RELEASE)
                        If Not bIsErrorLOGSend Then
                            'Length Mismatch - send LOG
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                         LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.LIVEPOG, LOGTransmitter.Reasons.Length_Mismatch)
                            bIsErrorLOGSend = True
                        End If
                        'Reading a line from the CSV file
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End If

                    'To replace the null values with 0s
                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = 0
                        End If
                    Next

                    'Replace ' with ''
                    staData(1) = staData(1).Replace("'", "''")

                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO LivePOG " _
                        & "(Module_ID,POG_Desc,Start_Date,End_Date," _
                        & "Category_ID1,Category_ID2,Category_ID3,POG_DB_No)" _
                        & " VALUES (" & staData(0) & ",'" & staData(1) & "','" _
                        & staData(2) & "','" & staData(3) & "'," _
                        & staData(4) & "," & staData(5) & "," _
                        & staData(6) & "," & staData(7) & ")"

                    sqlCommd.CommandText = strInsertCmd
                    'Executing the querry
                    Try
                        'Executing the querry
                        sqlCommd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        'Log the duplicate record for reference.
                        If Not ex.Message.Contains("duplicate") Then
                            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseLivePOGFile:: Error:" _
                                                                  & "record found " + strLine, _
                                                                  Logger.LogLevel.INFO)
                        End If
                        If Not bIsErrorLOGSend Then
                            'Send a LOG
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                   LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.LIVEPOG, LOGTransmitter.Reasons.PKEY_Violation, _
                                                                                     iCount.ToString())
                            bIsErrorLOGSend = True
                        End If
                        'Increment the line count.
                        iCount = iCount + 1
                        'Read next line
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End Try
                ElseIf strLine.StartsWith("H") Then
                    If Not strLine.Contains(strFileName) Then
                        AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseLivePOGFile::Invalid file format. The " _
                                                    & "file name in the header record is not matched.", _
                                                    Logger.LogLevel.RELEASE)
                        'Send a LOG
                        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                         LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.LIVEPOG, LOGTransmitter.Reasons.Header_Name_Mismatch)
                        'return status
                        'Return False
                        bIsErrorLOGSend = True
                    End If
                End If
                'Read next line
                strLine = rdrFileReader.ReadLine()
            End While
            'Close the file reader.
            rdrFileReader.Close()
            'Delete the reference file used by this function.
            File.Delete(strPath & strFileName)
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseLivePOGFile::File parsing completed for LIVEPOG.CSV.", _
                                                            Logger.LogLevel.RELEASE)
            'Create index and statistics.
            CreateIndex("LivePOG", "Module_ID")
            'CreateStatistics("LivePOG", "Module_ID")
        Catch ex As FileNotFoundException
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseLivePOGFile::File" & strFileName & " - not found.", _
                                                  Logger.LogLevel.RELEASE)
            'Send a LOG - File Not Found
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                   LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.LIVEPOG, LOGTransmitter.Reasons.File_Not_Found)
            'close the reader
            rdrFileReader.Close()
            'Return false in case of error
            Return False
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseLivePOGFile:: Exception occurred @ " & _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            'Send a LOG - Exception
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                   LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.LIVEPOG, LOGTransmitter.Reasons.Other_Errors)
            'Update the file index to the config file.
            BatchConfigParser.GetInstance().UpdateDBPopulationIndex(strFileName, iCount, ex.Message.ToString)
            Return False
        Finally
            rdrFileReader.Close()
            AppContainer.GetInstance.objRefDownloadForm.DBUpdationStop()
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        'Successful - send a LOG message
        If Not bIsErrorLOGSend Then
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseLivePOGFile:: DB update successful for " + strFileName, _
                                                            Logger.LogLevel.RELEASE)
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                      LOGTransmitter.Status.END_OK, _
                                                                      LOGTransmitter.FileName.LIVEPOG, _
                                                                      LOGTransmitter.Reasons.Update_Success)
        Else
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.ABEND, _
                                                                  LOGTransmitter.FileName.LIVEPOG, _
                                                                  LOGTransmitter.Reasons.Update_Error)
        End If
        Return True
    End Function
    ''' <summary>
    '''  To parse CATEGORY.csv and update the DB
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function ParseCategoryFile(ByVal strFileName As String, ByVal strPath As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        'To monitor LOG message status
        Dim bIsErrorLOGSend As Boolean = False
        'For reading CATEGORY.csv file
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0
        Dim iPercentage As Integer = 10
        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseCategoryFile::File parsing started for CATEGORY.CSV.", _
                                                        Logger.LogLevel.RELEASE)
        AppContainer.GetInstance.objRefDownloadForm.DBUpdationStart()
        'Starting to Parse - send LOG message
        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.START, _
                                                                  LOGTransmitter.FileName.CATEGORY, _
                                                                  LOGTransmitter.Reasons.Parse_File)
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Load the file for reading.
            rdrFileReader = New System.IO.StreamReader(strPath + strFileName)

            'iFileIndex is set to a value when iFileIndex number of records 
            'is already inserted to the database.
            If iFileIndex > 0 Then
                iCount = iFileIndex
                iFileIndex = iFileIndex + 2
                For iLoopIndex As Integer = 1 To iFileIndex
                    'Reading a line from the CSV file
                    strLine = rdrFileReader.ReadLine()
                Next
            End If
            'Assign SQL connection to the command
            sqlCommd.Connection = sqlConn
            'Reading a line from the CSV file
            strLine = rdrFileReader.ReadLine()
            'Run through the entire file records.
            While Not strLine.StartsWith("T")
                strLine = strLine.Trim()
                If iCount = m_PercentageValue Then
                    AppContainer.GetInstance.objRefDownloadForm.SetDBUpdationstatus(iPercentage)
                    m_PercentageValue = m_PercentageValue + m_RowCount
                    iPercentage = iPercentage + 10
                End If
                If strLine.StartsWith("D") Then

                    strLine = strLine.Substring(2)
                    'Parsing the line
                    staData = strLine.Split(DELIMETER)
                    'Check for length of the data.
                    If staData.Length <> 4 Then
                        iCount = iCount + 1
                        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseCategoryFile::Error - Length mismatch: " & strLine, _
                                                    Logger.LogLevel.RELEASE)
                        If Not bIsErrorLOGSend Then
                            'Mismatch - send a log message
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                               LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.CATEGORY, LOGTransmitter.Reasons.Length_Mismatch)
                            bIsErrorLOGSend = True
                        End If
                        'Reading a line from the CSV file
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End If
                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = 0
                        End If
                    Next

                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO POGCategory " _
                        & "(Category_ID,Category_Desc,Category_Type,Core_Non_Core)" _
                        & " VALUES (" & staData(0) & ",'" & staData(1) & "'," _
                        & staData(2) & ",'" & staData(3) & "')"

                    sqlCommd.CommandText = strInsertCmd
                    'Executing the querry
                    Try
                        'Executing the querry
                        sqlCommd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        'Log the duplicate record for reference.
                        If Not ex.Message.Contains("duplicate") Then
                            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseCategoryFile:: Error:" _
                                                                  & "record found " + strLine, _
                                                                  Logger.LogLevel.INFO)
                        End If
                        If Not bIsErrorLOGSend Then
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                                          LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.CATEGORY, LOGTransmitter.Reasons.PKEY_Violation, _
                                                                                     iCount.ToString())
                            bIsErrorLOGSend = True
                        End If
                        'Increment the line count.
                        iCount = iCount + 1
                        'Read next line
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End Try
                ElseIf strLine.StartsWith("H") Then
                    If Not strLine.Contains(strFileName) Then
                        AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseCategoryFile:: Invalid file format. The " _
                                                    & "file name in the header record is not matched.", _
                                                    Logger.LogLevel.RELEASE)
                        'Header record is not matched - send a log file 
                        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                               LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.CATEGORY, LOGTransmitter.Reasons.Header_Name_Mismatch)
                        'return status
                        'Return False
                        bIsErrorLOGSend = True
                    End If
                End If
                'Read next line
                strLine = rdrFileReader.ReadLine()
            End While
            'Close the file reader.
            rdrFileReader.Close()
            'Delete the reference file used by this function.
            File.Delete(strPath & strFileName)
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseCategoryFile::File parsing completed for CATEGORY.CSV.", _
                                                            Logger.LogLevel.RELEASE)
            'Create index and statistics.
            CreateIndex("POGCategory", "Category_ID")
            'CreateStatistics("POGCategory", "Category_ID")
        Catch ex As FileNotFoundException
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseCategoryFile::File" & strFileName & " - not found.", _
                                                  Logger.LogLevel.RELEASE)
            ' File not found - send a LOG message
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                               LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.CATEGORY, LOGTransmitter.Reasons.File_Not_Found)
            'close the reader
            rdrFileReader.Close()
            'Return false in case of error
            Return False
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseCategoryFile:: Exception occurred @ " & _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            'Error Occured - send a LOG message
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                               LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.CATEGORY, LOGTransmitter.Reasons.Other_Errors)
            'Update the file index to the config file.
            BatchConfigParser.GetInstance().UpdateDBPopulationIndex(strFileName, iCount, ex.Message.ToString)
            Return False
        Finally
            rdrFileReader.Close()
            AppContainer.GetInstance.objRefDownloadForm.DBUpdationStop()
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        'Success full send a LOG message
        If Not bIsErrorLOGSend Then
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseCategoryFile::DB update successful for " + strFileName, _
                                                            Logger.LogLevel.RELEASE)
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                      LOGTransmitter.Status.END_OK, _
                                                                      LOGTransmitter.FileName.CATEGORY, _
                                                                      LOGTransmitter.Reasons.Update_Success)
        Else
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.ABEND, _
                                                                  LOGTransmitter.FileName.CATEGORY, _
                                                                  LOGTransmitter.Reasons.Update_Error)
        End If
        Return True
    End Function
    ''' <summary>
    '''  To parse MODULE.csv and update the DB
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function ParseModuleFile(ByVal strFileName As String, ByVal strPath As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        'To monitor LOG message status
        Dim bIsErrorLOGSend As Boolean = False
        'For reading MODULE.csv file
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0
        Dim iPercentage As Integer = 10
        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseModuleFile::File parsing started for MODULE.CSV.", _
                                                        Logger.LogLevel.RELEASE)
        AppContainer.GetInstance.objRefDownloadForm.DBUpdationStart()
        'Starting to parse Module File - send corresponding LOG
        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.START, _
                                                                  LOGTransmitter.FileName.MODULES, _
                                                                  LOGTransmitter.Reasons.Parse_File)
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Load the file for reading.
            rdrFileReader = New System.IO.StreamReader(strPath + strFileName)

            'iFileIndex is set to a value when iFileIndex number of records 
            'is already inserted to the database.

            If iFileIndex > 0 Then
                iFileIndex = iFileIndex + 2
                iCount = iFileIndex
                For iLoopIndex As Integer = 1 To iFileIndex
                    'Reading a line from the CSV file
                    strLine = rdrFileReader.ReadLine()
                Next
            End If
            'Assign SQL connection to the command
            sqlCommd.Connection = sqlConn
            'Reading a line from the CSV file
            strLine = rdrFileReader.ReadLine()
            'Run through the entire list of records in the file.
            While Not strLine.StartsWith("T")
                strLine = strLine.Trim()
                If iCount = m_PercentageValue Then
                    AppContainer.GetInstance.objRefDownloadForm.SetDBUpdationstatus(iPercentage)
                    m_PercentageValue = m_PercentageValue + m_RowCount
                    iPercentage = iPercentage + 10
                End If
                If strLine.StartsWith("D") Then
                    Try
                        strLine = strLine.Substring(2)
                        'Parsing the line
                        staData = strLine.Split(DELIMETER)
                        'Repeat Count CR Changes
                        'Change in Length OF the data when Repeat Count Added, Data length -6
                        'Check for length of the data.
                        If staData.Length <> 6 Then
                            iCount = iCount + 1
                            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseModuleFile::Error - Length mismatch: " & strLine, _
                                                        Logger.LogLevel.RELEASE)
                            If Not bIsErrorLOGSend Then
                                'Length Mismatch - Send LOG
                                AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                    LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.MODULES, LOGTransmitter.Reasons.Length_Mismatch)
                                bIsErrorLOGSend = True
                            End If
                            'Reading a line from the CSV file
                            strLine = rdrFileReader.ReadLine()
                            Continue While
                        End If

                        For iIndex As Integer = 0 To staData.Length - 1
                            If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                                'Null values are replaced with '0'
                                staData(iIndex) = 0
                            End If
                        Next
                        ' Repeat Count CR Changes
                        'Inserting Repeat Count Field into ModuleListTable
                        'For inserting the parsed data to DB
                    'Minu Removed single quotes from the query for integer values.
                    strInsertCmd = "INSERT INTO ModuleList " _
                        & "(Module_ID,Module_Seq,Module_Desc,Shelf_Count,Item_Count,Repeat_Count)" _
                        & " VALUES (" & staData(0) & "," & staData(1) & ",'" _
                        & staData(2).Trim() & "'," & staData(3) & "," _
                        & staData(4) & "," & staData(5) & ")"


                        sqlCommd.CommandText = strInsertCmd
                        'Executing the querry

                        'Executing the querry
                        sqlCommd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        'Log the duplicate record for reference.
                        If Not ex.Message.Contains("duplicate") Then
                            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseModuleFile:: Error:" _
                                                                  & "record found " + strLine, _
                                                                  Logger.LogLevel.INFO)
                        End If
                        If Not bIsErrorLOGSend Then
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                                    LOGTransmitter.Status.ABEND, _
                                                                                    LOGTransmitter.FileName.MODULES, _
                                                                                    LOGTransmitter.Reasons.PKEY_Violation, _
                                                                                     iCount.ToString())
                            bIsErrorLOGSend = True
                        End If
                        'Increment the line count.
                        iCount = iCount + 1
                        'Read next line
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End Try
                ElseIf strLine.StartsWith("I") Then
                    Try
                        strLine = strLine.Substring(2)
                        'Parsing the line
                        staData = strLine.Split(DELIMETER)
                        'Check for length of the data.
                        If staData.Length <> 10 Then
                            iCount = iCount + 1
                            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseCategoryFile::Error - Length mismatch: " & strLine, _
                                                        Logger.LogLevel.RELEASE)
                            If Not bIsErrorLOGSend Then
                                'Length Mismatch -Send LOG
                                AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                    LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.MODULES, LOGTransmitter.Reasons.Length_Mismatch)
                                bIsErrorLOGSend = True
                            End If
                            'Reading a line from the CSV file
                            strLine = rdrFileReader.ReadLine()
                            Continue While
                        End If
                        For iIndex As Integer = 0 To staData.Length - 1
                            If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                                'Null values are replaced with '0'
                                staData(iIndex) = 0
                            End If
                        Next
                        'Escape the single quotes characters if any
                    'staData(3) = staData(3).Trim()
                    'staData(3) = staData(3).Replace("'", "''")

                    'Minu Removed single quotes from the query for integer values.
                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO ModuleListItems " _
                        & "(Module_ID,Module_Seq,Shelf_Numb," _
                        & "Shelf_Desc_Index,Notch_No,Facings,Boots_Code,MDQ,PSC,Sequence_Number)" _
                        & " VALUES (" & staData(0) & "," & staData(1) & "," _
                        & staData(2) & "," & staData(3) & "," & staData(4) & "," _
                        & staData(5) & "," & staData(6) & "," _
                        & staData(7) & "," & staData(8) & "," _
                        & staData(9) & ")"
                        '& GenerateBCwithCDV(staData(5)) & "','" & staData(2) & "','" & staData(3) & "'," _
                        sqlCommd.CommandText = strInsertCmd
                        'Executing the querry

                        'Executing the querry
                        sqlCommd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        If Not ex.Message.Contains("duplicate") Then
                            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseModuleFile::Error:" _
                                                                      & "record found " + strLine, _
                                                                      Logger.LogLevel.INFO)
                        End If
                        If Not bIsErrorLOGSend Then
                            'Duplicate records Found - send LOG
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                                     LOGTransmitter.Status.ABEND, _
                                                                                     LOGTransmitter.FileName.MODULES, _
                                                                                     LOGTransmitter.Reasons.PKEY_Violation, _
                                                                                     iCount.ToString())
                            bIsErrorLOGSend = True
                        End If
                        'Increment the line count.
                        iCount = iCount + 1
                        'Read next line
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End Try
                ElseIf strLine.StartsWith("H") Then
                    If Not strLine.Contains(strFileName) Then
                        AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseModuleFile:: Invalid file format. The " _
                                                    & "file name in the header record is not matched.", _
                                                    Logger.LogLevel.RELEASE)
                        'Header Mismatch - log message
                        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.MODULES, LOGTransmitter.Reasons.Header_Name_Mismatch)
                        'return status
                        'Return False
                        bIsErrorLOGSend = True
                    End If
                End If

                'Read next line
                strLine = rdrFileReader.ReadLine()
            End While
            'Close file reader.
            rdrFileReader.Close()
            'Delete the reference file used by this function.
            File.Delete(strPath & strFileName)
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseModuleFile::File parsing completed for MODULE.CSV.", _
                                                        Logger.LogLevel.RELEASE)
            'Create index and statistics.
            CreateIndex("ModuleList", "Module_ID,Module_Seq")
            'CreateStatistics("ModuleList", "Module_ID,Module_Seq")
            'Create index and statistics.
            CreateIndex("ModuleListItems", "Module_ID,Boots_Code")
            CreateIndex("ModuleListItems", "Module_ID,Module_Seq", "ModuleListSeq")
            'CreateStatistics("ModuleListItems", "Module_ID,Module_Seq,Boots_Code,Shelf_Numb,Sequence_Number")
        Catch ex As FileNotFoundException
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseModuleFile::File " & strFileName & " - not found.", _
                                                          Logger.LogLevel.RELEASE)
            'File Not found - exception - send a LOG message
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                 LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.MODULES, LOGTransmitter.Reasons.File_Not_Found)
            'close the reader
            rdrFileReader.Close()
            'Return false in case of error
            Return False
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ParseModuleFile:: Exception occurred @ " & _
                                                          ex.StackTrace, Logger.LogLevel.RELEASE)
            'Error - send a LOG
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                 LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.MODULES, LOGTransmitter.Reasons.Other_Errors)
            'Update the file index to the config file.
            BatchConfigParser.GetInstance().UpdateDBPopulationIndex(strFileName, iCount, ex.Message.ToString)
            Return False
        Finally
            rdrFileReader.Close()
            AppContainer.GetInstance.objRefDownloadForm.DBUpdationStop()
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        'SuccesFul Send LOG
        If Not bIsErrorLOGSend Then
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseModuleFile::DB update successful for " + strFileName, _
                                                            Logger.LogLevel.RELEASE)
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                      LOGTransmitter.Status.END_OK, _
                                                                      LOGTransmitter.FileName.MODULES, _
                                                                      LOGTransmitter.Reasons.Update_Success)
        Else
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.ABEND, _
                                                                  LOGTransmitter.FileName.MODULES, _
                                                                  LOGTransmitter.Reasons.Update_Error)
        End If
        Return True
    End Function
    ''' <summary>
    '''  To parse SHELFDES.csv and update the DB
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function ParseShelFDescFile(ByVal strFileName As String, ByVal strPath As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        'To monitor LOG message status
        Dim bIsErrorLOGSend As Boolean = False
        'For reading SHELFDES.csv file
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0
        Dim iPercentage As Integer = 10
        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseModuleFile::File parsing started for MODULE.CSV.", _
                                                        Logger.LogLevel.RELEASE)
        'Starting to parse Module File - send corresponding LOG
        AppContainer.GetInstance().objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.START, _
                                                                  LOGTransmitter.FileName.SHELFDES, _
                                                                  LOGTransmitter.Reasons.Parse_File)
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Load the file for reading.
            rdrFileReader = New System.IO.StreamReader(strPath + strFileName)

            'iFileIndex is set to a value when iFileIndex number of records 
            'is already inserted to the database.

            If iFileIndex > 0 Then
                iFileIndex = iFileIndex + 2
                iCount = iFileIndex
                For iLoopIndex As Integer = 1 To iFileIndex
                    'Reading a line from the CSV file
                    strLine = rdrFileReader.ReadLine()
                Next
            End If
            'Assign SQL connection to the command
            sqlCommd.Connection = sqlConn
            'Reading a line from the CSV file
            strLine = rdrFileReader.ReadLine()
            'Run through the entire list of records in the file.
            While Not strLine.StartsWith("T")
                strLine = strLine.Trim()
                If iCount = m_PercentageValue Then
                    AppContainer.GetInstance.objRefDownloadForm.SetDBUpdationstatus(iPercentage)
                    m_PercentageValue = m_PercentageValue + m_RowCount
                    iPercentage = iPercentage + 10
                End If
                If strLine.StartsWith("D") Then
                    strLine = strLine.Substring(2)
                    'Parsing the line
                    staData = strLine.Split(DELIMETER)
                    'Check for length of the data.
                    If staData.Length <> 2 Then
                        iCount = iCount + 1
                        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseShelFDescFile::Error - Length mismatch: " & strLine, _
                                                        Logger.LogLevel.RELEASE)
                        If Not bIsErrorLOGSend Then
                            'Length Mismatch - Send LOG
                            AppContainer.GetInstance().objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.SHELFDES, LOGTransmitter.Reasons.Length_Mismatch)
                            bIsErrorLOGSend = True
                        End If
                        'Reading a line from the CSV file
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End If

                    'Escape single quotes 
                    staData(1) = staData(1).Replace("'", "''")

                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = 0
                        End If
                    Next

                    'Stock File Accuracy Minu - Handle null value in description field
                    If (staData(1)) = "0" Then
                        staData(1) = "Not Available"
                    End If

                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO ShelfDesc " _
                        & "(Shelf_Desc_Index,Shelf_Desc)" _
                        & " VALUES (" & staData(0) & ",'" & staData(1) & "')"

                    sqlCommd.CommandText = strInsertCmd
                    'Executing the querry
                    Try
                        'Executing the querry
                        sqlCommd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        'Log the duplicate record for reference.
                        If Not ex.Message.Contains("duplicate") Then
                            'Log the duplicate record for reference.
                            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser: duplicate " _
                                                                 & "record found " + strLine, _
                                                                 Logger.LogLevel.INFO)
                        End If
                        If Not bIsErrorLOGSend Then
                            AppContainer.GetInstance().objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                            LOGTransmitter.Status.ABEND, _
                                                                            LOGTransmitter.FileName.SHELFDES, _
                                                                            LOGTransmitter.Reasons.PKEY_Violation, iCount.ToString())
                            bIsErrorLOGSend = True
                        End If
                        'Increment the line count.
                        iCount = iCount + 1
                        'Read next line
                        strLine = rdrFileReader.ReadLine()
                        Continue While
                    End Try
                ElseIf strLine.StartsWith("H") Then
                    If Not strLine.Contains(strFileName) Then
                        AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser: Invalid file format. The " _
                                                   & "file name in the header record is not matched.", _
                                                   Logger.LogLevel.RELEASE)
                        'Header Mismatch - log message
                        AppContainer.GetInstance().objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.MODULES, LOGTransmitter.Reasons.Header_Name_Mismatch)
                        'return status
                        Return False
                        bIsErrorLOGSend = True
                    End If
                End If

                'Read next line
                strLine = rdrFileReader.ReadLine()
            End While
            'Check iCount against the count in "T" record.
            staData = Nothing
            staData = strLine.Split(DELIMETER)
            'Check if all the records are inserted.
            ' If iCount = Val(staData(1)) Then
            'Close file reader.
            rdrFileReader.Close()
            'Delete the reference file used by this function.
            File.Delete(strPath & strFileName)
            'End If
            BatchConfigParser.GetInstance().UpdateDBPopulationIndex(strFileName, iCount)

        Catch ex As FileNotFoundException
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser: File" & strFileName & "not found.", _
                                                 Logger.LogLevel.RELEASE)
            'File Not found - exception - send a LOG message
            AppContainer.GetInstance().objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                 LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.SHELFDES, LOGTransmitter.Reasons.File_Not_Found)
            'close the reader
            rdrFileReader.Close()
            BatchConfigParser.GetInstance().UpdateDBPopulationIndex(strFileName, iCount, ex.Message.ToString)
            'Return false in case of error
            Return False
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser: Exception occurred" & _
                                                 ex.StackTrace, Logger.LogLevel.RELEASE)
            'Error - send a LOG
            AppContainer.GetInstance().objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                 LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.SHELFDES, LOGTransmitter.Reasons.Other_Errors)
            'Update the file index to the config file.
            BatchConfigParser.GetInstance().UpdateDBPopulationIndex(strFileName, iCount, ex.Message.ToString)
            Return False
        Finally
            rdrFileReader.Close()
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        'SuccesFul Send LOG
        If Not bIsErrorLOGSend Then
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ParseModuleFile::DB update successful for " + strFileName, _
                                                            Logger.LogLevel.RELEASE)
            AppContainer.GetInstance().objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                      LOGTransmitter.Status.END_OK, _
                                                                      LOGTransmitter.FileName.SHELFDES, _
                                                                      LOGTransmitter.Reasons.Update_Success)
        Else
            AppContainer.GetInstance().objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                  LOGTransmitter.Status.ABEND, _
                                                                  LOGTransmitter.FileName.SHELFDES, _
                                                                  LOGTransmitter.Reasons.Update_Error)
        End If
        Return True
    End Function

    ''' <summary>
    ''' Calculates the Boots Code based on CDV
    ''' </summary>
    ''' <param name="bootsCode">Boots code without CDV</param>
    ''' <returns>Boots code with CDV</returns>
    ''' <remarks></remarks>
    Public Function GenerateBCwithCDV(ByVal bootsCode As String) As String
        Dim validBootsCode As String = ""
        Dim total As Int32 = 0
        Dim counter As Int32 = 1
        Dim factor As Int32 = 7

        While counter < 7
            total = total + (Val(Mid(bootsCode, counter, 1)) * factor)
            counter = counter + 1
            factor = factor - 1
        End While

        total = 11 - (total Mod 11)
        If total > 9 Then
            total = 0
        End If
        validBootsCode = bootsCode & total
        GenerateBCwithCDV = validBootsCode
    End Function
    ''' <summary>
    ''' To run sample queries to ease the Db data fetching 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ExecuteSampleQuery(ByVal strFile As String)
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0
        Dim sqlQuery As String = ""
        Try
            Dim sqlCommd As New SqlCeCommand()
            Dim sqlRdr As SqlCeDataReader
            'Run the query create statistics.
            Select Case strFile
                Case "Module"
                    sqlQuery = "SELECT ModuleList.Module_Seq, ModuleListItems.Shelf_Numb, " _
                        & "ModuleList.Module_Desc, ModuleList.Module_ID, LivePOG.POG_Desc " _
                        & "FROM ModuleList INNER JOIN ModuleListItems  WITH(INDEX (ModuleListItems_IDX)) " _
                        & "ON ModuleList.Module_ID = ModuleListItems.Module_ID " _
                        & "AND ModuleList.Module_Seq = ModuleListItems.Module_Seq " _
                        & "INNER JOIN LivePOG ON LivePOG.Module_ID = ModuleList.Module_ID " _
                        & "WHERE ModuleListItems.Module_ID IN ('221342') AND (ModuleListItems.Boots_Code = '1234567')"
                Case Else
                    sqlQuery = "SELECT BarCodeView.Boots_Code, BootsCodeView.Item_Status, " _
                        & "BootsCodeView.SEL_Desc, BootsCodeView.SOD_TSF, BarCodeView.Current_Price, " _
                        & "BarCodeView.Item_Status3, BarCodeView.Deal_No1, " _
                        & "BarCodeView.Deal_No2, BarCodeView.Deal_No3, BarCodeView.Deal_No4, " _
                        & "BarCodeView.Deal_No5, BarCodeView.Deal_No6, BarCodeView.Deal_No7, " _
                        & "BarCodeView.Deal_No8, BarCodeView.Deal_No9, BarCodeView.Deal_No10 " _
                        & "FROM BarCodeView, BootsCodeView " _
                        & "WHERE (BarCodeView.BarCode = '123456789012') And " _
                        & "BootsCodeView.Boots_Code = BarCodeView.Boots_Code"
            End Select
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ExecuteSampleQuery::Executing sample query started.", _
                                                            Logger.LogLevel.RELEASE)
            'Executing the querry
            sqlCommd = New SqlCeCommand(sqlQuery, sqlConn)
            sqlRdr = sqlCommd.ExecuteReader()
            AppContainer.GetInstance().obLogger.WriteAppLog("RefFileParser:: ExecuteSampleQuery::Executing sample query completed.", _
                                                            Logger.LogLevel.RELEASE)
        Catch ex As Exception
            'Log the duplicate record for reference.
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: ExecutingSampleQuery :: Error " _
                                                  & "record found " + strLine, _
                                                  Logger.LogLevel.INFO)
        End Try
    End Sub
    ''' <summary>
    ''' Function to delete index on the tables after populating the data.
    ''' </summary>
    ''' <param name="strTableName"></param>
    ''' <remarks></remarks>
    Private Sub DeleteIndex(ByVal strTableName As String)
        Dim strDeleteIndex As String = Nothing
        Dim strIndex As String = Nothing
        Dim iVal As Integer = 0
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Dim sqlRdr As SqlCeDataReader
            strIndex = strTableName & "_IDX"
            strDeleteIndex = String.Format("DROP INDEX {0}.{1}", strTableName, strIndex)
            'Executing the querry
            sqlCommd = New SqlCeCommand(strDeleteIndex, sqlConn)
            iVal = sqlCommd.ExecuteNonQuery()
        Catch ex As Exception
            'Log the duplicate record for reference.
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: DeleteIndex:: Error Occured, Message:  " + ex.Message, _
                                                          Logger.LogLevel.ERROR)
        End Try
    End Sub
    ''' <summary>
    ''' Function to create index on the tables after populating the data.
    ''' </summary>
    ''' <param name="strTableName"></param>
    ''' <remarks></remarks>
    Private Sub CreateIndex(ByRef strTableName As String, ByVal strColumns As String, Optional ByVal strIndex As String = "NA")
        Dim strCreateIndex As String = Nothing
        Dim iVal As Integer = 0
        Try
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: CreateIndex:: start Time - " + Now.ToString(), Logger.LogLevel.INFO)
            Dim sqlCommd As New SqlCeCommand()
            'Dim sqlRdr As SqlCeDataReader
            If strIndex.Equals("NA") Then
                strIndex = strTableName & "_IDX"
            End If
            strCreateIndex = String.Format("CREATE INDEX {0} ON {1} ({2})", strIndex, strTableName, strColumns)
            'Executing the querry
            sqlCommd = New SqlCeCommand(strCreateIndex, sqlConn)
            iVal = sqlCommd.ExecuteNonQuery()
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: CreateIndex:: Completed Time - " + Now.ToString(), Logger.LogLevel.INFO)
        Catch ex As Exception
            'Log the duplicate record for reference.
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser:: CreateIndex:: Error ccured, Message: " + ex.Message, _
                                                          Logger.LogLevel.ERROR)
        End Try
    End Sub
    ''' <summary>
    ''' Function to create statistics on the tables after populating the data.
    ''' </summary>
    ''' <param name="strTableName"></param>
    ''' <remarks></remarks>
    Private Sub CreateStatistics(ByVal strTableName As String, ByVal strColumns As String)
        Dim strCreateStats As String = Nothing
        Dim strStats As String = Nothing
        Dim iVal As Integer = 0
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Dim sqlRdr As SqlCeDataReader
            strStats = strTableName & "_STATS"
            strCreateStats = String.Format("CREATE STATISTICS {0} ON {1}({2}) WITH FULLSCAN", strStats, strTableName, strColumns)
            'Executing the querry
            sqlCommd = New SqlCeCommand(strCreateStats, sqlConn)
            iVal = sqlCommd.ExecuteNonQuery()
        Catch ex As Exception
            'Log the duplicate record for reference.
            AppContainer.GetInstance.obLogger.WriteAppLog("RefFileParser: Create Statistics: " + ex.Message, _
                                                          Logger.LogLevel.ERROR)
        End Try
    End Sub
End Class