Imports System.IO
Imports System.Data
#If NRF Then
Imports System.Data.SqlServerCe
#End If
Imports System.Xml
Imports System.Text

#If NRF Then

'''****************************************************************************
''' <FileName>ActiveFileParser.vb</FileName>
''' <summary>
''' Responsible for parsing the Active files and populate the DB. This class is
''' also responsible for deleting the data present in the Active Tables before 
''' populating the data in the database.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''****************************************************************************
Public Class ActiveFileParser
    Private m_DBPath As String = ConfigDataMgr.GetInstance.GetParam(ConfigKey.CONN_STRING)
    Private m_ConnectionString As String = "Data Source = " & _
                        ConfigDataMgr.GetInstance.GetParam(ConfigKey.CONN_STRING)
    Private m_Delimiter As String = Macros.DELIMITER

    Private m_RetryDelete As Integer = Macros.DELETERETRY

    Private Const DEFAULT_DATE As String = "19000101"
    'Create an instance of the SQL CE Connection.
    Dim m_SqlConn As SqlCeConnection = Nothing

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub
    ''' <summary>
    ''' Method to open connection to the device database.
    ''' </summary>
    ''' <returns>Bool
    ''' True - If successfully opened a connection to the database.
    ''' False - If failed to open a connection to the database.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function Initialise() As Boolean
        Try
            If m_DBPath <> Nothing And File.Exists(m_DBPath) Then
                m_SqlConn = New SqlCeConnection(m_ConnectionString)
                m_SqlConn.Open()
                'Add the exception to the application log.
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                                "Opened connection to database", _
                                                Logger.LogLevel.RELEASE)
            Else
                'Throw error message box.
                MessageBox.Show("Database file not found", "Warning", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, _
                                MessageBoxDefaultButton.Button1)
                'Add the exception to the application log.
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            "DB file does not exists or path " _
                                            & "not available in config file", _
                                            Logger.LogLevel.RELEASE)
                'return the status
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.StackTrace.ToString(), _
                                            Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
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
        If m_SqlConn.State = ConnectionState.Open Then
            Try
                'Close the connection to the DB.
                m_SqlConn.Close()
                Return True
            Catch ex As SqlCeException
                Return False
            End Try
        Else
            Return False
        End If
    End Function
    ''' <summary>
    ''' To clear the data present in the reference data table.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function PurgeActiveData() As Boolean
        Dim aActiveTable() As String = Nothing
        Dim iRetryCount As Integer = m_RetryDelete
        Dim iCount As Integer = 0
        Dim strInsertCmd As String = ""
        Try
            Dim sqlCommd As New SqlCeCommand()
            Dim sqlCompactEngine As SqlCeEngine
            sqlCommd.Connection = m_SqlConn
            sqlCompactEngine = New SqlCeEngine(m_ConnectionString)
            aActiveTable = Macros.ACTIVETABLES.Split(m_Delimiter)
            While iCount >= 0 And iCount < aActiveTable.Length
                Try
                    strInsertCmd = "DELETE FROM " & aActiveTable(iCount).Trim()
                    sqlCommd.CommandText = strInsertCmd
                    sqlCommd.ExecuteNonQuery()

                    'Shrink the database to truncate the empty space occupied by the DB file
                    sqlCompactEngine.Shrink()

                    'Log the process compeltion.
                    objAppContainer.objLogger.WriteAppLog("UserSessionManager: Active data " _
                                                          & "in table " & aActiveTable(iCount) _
                                                          & " is cleared", Logger.LogLevel.RELEASE)
                Catch ex As Exception
                    'Try
                    If iRetryCount > 0 Then
                        iRetryCount = iRetryCount - 1
                        Continue While
                    Else
                        'Add the exception to the device log.
                        objAppContainer.objLogger.WriteAppLog(ex.StackTrace, _
                                                              Logger.LogLevel.RELEASE)
                        'Update the details of the table whose data is not purged.
                        'Modified to return true for handling defect  in pilot : 65
                        Return True
                    End If
                End Try
                'Increment the array index counter and reset the retry count
                iCount = iCount + 1
                iRetryCount = m_RetryDelete
            End While
            'update the active data availability in the config file.
            ConfigDataMgr.GetInstance.SetParam(ConfigKey.ACTIVE_DATA_AVAILABILITY, "False")
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: Exception occurred" & _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            'return status
            'Modified to return true for handling defect  in pilot : 65
            Return True
        End Try
        Return True
    End Function
    ''' <summary>
    ''' To invoke fucntion corresponding to the name of the file.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseFile(ByVal strFileName As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        Dim strLocalPath = ConfigDataMgr.GetInstance().GetParam(ConfigKey.ACTIVE_FILE_PATH)
        Select Case strFileName
            Case Macros.COUNT
                Return Me.ParseCountFile(strLocalPath, _
                                            strFileName, _
                                            iFileIndex)
            Case Macros.PICKING
                Return Me.ParsePickingFile(strLocalPath, _
                                          strFileName, _
                                          iFileIndex)
            Case Macros.CONTROL
                Return Me.ParseControlFile(strLocalPath, _
                                            strFileName, _
                                            iFileIndex)
        End Select
    End Function
    ''' <summary>
    ''' To parse CONTROL.csv and update the DB
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function ParseControlFile(ByVal strFilePath As String, ByVal strFileName As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0

        Try
            Dim sqlCommd As New SqlCeCommand()
            'Assign SQL connection to the command.s
            sqlCommd.Connection = m_SqlConn

            'Load the file for reading.
            rdrFileReader = New System.IO.StreamReader(strFilePath + strFileName)
            'Reading a line from the CSV file
            strLine = rdrFileReader.ReadLine()

            While Not (strLine.StartsWith("T"))
                strLine = strLine.Trim()
                If strLine.StartsWith("D") Then
                    strLine = strLine.Substring(2)
                    'Parsing the line
                    staData = strLine.Split(m_Delimiter)

                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = 0
                        End If
                    Next

                    'Set the value to the global variable in appcontiner that holds the 
                    'price check completed count tracked.
                    objAppContainer.iCompletedCount = CInt(staData(8))
                    'Removed cint char
                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO CountsAndTargets " _
                    & "(Store_Status,Store_Type,Price_Inc_Applied,LDCPARM1," _
                    & "LDCPARM2,LDCPARM3,PC_Date_Threshold,PC_Weekly_Target," _
                    & "PC_Count_Current_Week,PC_Upper_Limit,PC_Lower_Limit," _
                    & "PC_Increments,PC_Default_Target,PC_Error_Count1," _
                    & "PC_Error_Count2,EMU_Status,Currency_Indicator," _
                    & "Emu_Conversation_Factor) VALUES ('" _
                    & staData(0) & "','" & staData(1) & "','" _
                    & staData(2) & "','" & staData(3) & "','" _
                    & staData(4) & "','" & staData(5) & "','" _
                    & staData(6) & "'," & staData(7) & "," _
                    & staData(8) & "," & staData(9) & "," _
                    & staData(10) & "," & staData(11) & "," _
                    & staData(12) & "," & staData(13) & "," _
                    & staData(14) & ",'" & staData(15) & "','" _
                    & staData(16) & "','" & staData(17) & "')"

                    sqlCommd.CommandText = strInsertCmd
                    'Executing the querry
                    sqlCommd.ExecuteNonQuery()
                    'Counter to keep track of the record number inserted to the db.
                    iCount = iCount + 1
                ElseIf strLine.StartsWith("H") Then
                    If Not strLine.Contains(strFileName) Then
                        objAppContainer.objLogger.WriteAppLog("ActiveFileParser: Invalid file format. The " _
                                                    & "file name in the header record is not matched.", _
                                                    Logger.LogLevel.RELEASE)
                        Return False
                    End If
                End If
                'Read next line
                strLine = rdrFileReader.ReadLine()
            End While
            'Close the reader
            rdrFileReader.Close()
            'Check iCount against the count in "T" record.
            staData = strLine.Split(",")
            'Check if all the records are inserted.
            If iCount = Val(staData(1)) Then
                'Delete the reference file used by this function.
                File.Delete(strFilePath & strFileName)
            End If
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus(Macros.CONTROL, "P", "NA")
        Catch ex As FileNotFoundException
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            strFileName & " file not found", _
                                            Logger.LogLevel.RELEASE)
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus(Macros.CONTROL, "F", "File not found.")
            'return false in case of any exception.
            Return False
        Catch ex As Exception
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus(Macros.CONTROL, "F", ex.Message.Trim())
            'return false in case of any exception.
            Return False
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        Return True
    End Function
    ''' <summary>
    ''' To parse PICKING.csv and update the DB
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function ParsePickingFile(ByVal strFilePath As String, ByVal strFileName As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        'For Insert
        Dim strListInsertCmd As String = ""
        Dim strItemsInsertCmd As String = ""
        Dim strPlanogramInsertCmd As String = ""
        'For Update
        Dim strUpdateCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0
        Dim iNumItems As Integer = 0
        Dim strListID As String = Nothing
        Dim sqlListCommd As SqlCeCommand
        Dim sqlItemsCommd As SqlCeCommand
        Dim sqlPlanogramCommd As SqlCeCommand
        Dim iIndx As Integer = 0

        Try
            Dim sqlCommd As New SqlCeCommand()

            'Assign SQL connection to the command.s
            'sqlListCommd.Connection = m_SqlConn
            'sqlItemsCommd.Connection = m_SqlConn
            sqlCommd.Connection = m_SqlConn

            'Load the file for reading.
            rdrFileReader = New System.IO.StreamReader(strFilePath + strFileName)
            'Reading a line from the CSV file
            strLine = rdrFileReader.ReadLine()
            'Support :Defining the Query
            'Parametrised query
            strListInsertCmd = "INSERT INTO PickingList (List_ID,List_Date,List_Time,Number_Of_Items,Creator_ID,Picker_ID,List_Type,List_Status)" _
            & "VALUES(@List_ID,@List_Date,@List_Time,@Number_Of_Items,@Creator_ID,@Picker_ID,@List_Type,@List_Status)"

            strItemsInsertCmd = "INSERT INTO PickListItems(List_ID,Sequence_Number,Boots_Code,Shelf_Qty," _
            & "Fill_Qty,Sales_At_Shelf_Count,Back_Shop_Count,Sales_At_Pick," _
            & "Item_Status,Stock_Figure) " _
            & " VALUES (@List_ID,@Sequence_Number,@Boots_Code,@Shelf_Qty,@Fill_Qty, " _
            & " @Sales_At_Shelf_Count,@Back_Shop_Count,@Sales_At_Pick," _
            & " @Item_Status,@Stock_Figure) "

            strPlanogramInsertCmd = "INSERT INTO PickingListPlanograms (List_ID,Boots_Code,Module_ID,Module_Seq,Repeat_Count," _
            & " Fill_Qty)" _
            & " VALUES(@List_ID,@Boots_Code,@Module_ID,@Module_Seq,@Repeat_Count,@Fill_Qty)"

            'initialising the command
            sqlListCommd = New SqlCeCommand(strListInsertCmd, m_SqlConn)
            sqlItemsCommd = New SqlCeCommand(strItemsInsertCmd, m_SqlConn)
            sqlPlanogramCommd = New SqlCeCommand(strPlanogramInsertCmd, m_SqlConn)
            'Changed data types.
            'Defining the Parameters
            With sqlListCommd.Parameters
                .Add("@List_ID", SqlDbType.SmallInt)
                .Add("@List_Date", SqlDbType.NVarChar)
                .Add("@List_Time", SqlDbType.SmallInt)
                .Add("@Number_Of_Items", SqlDbType.SmallInt)
                .Add("@Creator_ID", SqlDbType.SmallInt)
                .Add("@Picker_ID", SqlDbType.SmallInt)
                .Add("@List_Type", SqlDbType.NVarChar)
                .Add("@List_Status", SqlDbType.NVarChar)
            End With

            With sqlItemsCommd.Parameters
                .Add("@List_ID", SqlDbType.SmallInt)
                .Add("@Sequence_Number", SqlDbType.SmallInt)
                .Add("@Boots_Code", SqlDbType.Int)
                .Add("@Shelf_Qty", SqlDbType.SmallInt)
                .Add("@Fill_Qty", SqlDbType.SmallInt)
                .Add("@Sales_At_Shelf_Count", SqlDbType.SmallInt)
                .Add("@Back_Shop_Count", SqlDbType.SmallInt)
                .Add("@Sales_At_Pick", SqlDbType.SmallInt)
                .Add("@Item_Status", SqlDbType.NVarChar)
                .Add("@Stock_Figure", SqlDbType.SmallInt)
            End With

            With sqlPlanogramCommd.Parameters
                .Add("@List_ID", SqlDbType.SmallInt)
                .Add("@Boots_Code", SqlDbType.Int)
                .Add("@Module_ID", SqlDbType.BigInt)
                .Add("@Module_Seq", SqlDbType.SmallInt)
                .Add("@Repeat_Count", SqlDbType.SmallInt)
                .Add("@Fill_Qty", SqlDbType.SmallInt)

            End With
            'Support : Initialising Parameterised Query
            sqlListCommd.Prepare()
            sqlItemsCommd.Prepare()
            sqlPlanogramCommd.Prepare()

            'Preparing the queries
            'sqlListCommd.Prepare()
            'sqlItemsCommd.Prepare()

            While Not (strLine.StartsWith("T"))
                strLine = strLine.Trim()
                If strLine.StartsWith("D") Then
                    strLine = strLine.Substring(2)
                    'Parsing the line
                    staData = strLine.Split(m_Delimiter)

                    'Handling for partial picked picking list
                    If staData(7) = Nothing Then
                        staData(7) = "U"
                    End If

                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = 0
                        End If
                    Next
                    'Store list id
                    If staData(0) <> strListID And strListID <> Nothing Then
                        'Update the picking list detail record with new count.
                        'For inserting the parsed data to DB
                        ' REmoved single quotes for integer values
                        strUpdateCmd = "UPDATE PickingList SET Number_Of_Items =" _
                                        & iNumItems & " WHERE List_ID=" & strListID
                        'Set command text in the sql command
                        sqlCommd.CommandText = strUpdateCmd
                        Try
                            'Executing the querry
                            sqlCommd.ExecuteNonQuery()
                        Catch ex As Exception
                            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: Updating the no. of items" _
                                                                  + strLine, Logger.LogLevel.RELEASE)
                        Finally
                            iNumItems = 0
                        End Try
                        'update the new list id inthe variable.
                        strListID = staData(0)
                    Else
                        'update the new list id inthe variable.
                        strListID = staData(0)
                    End If

                    'For inserting the parsed data to DB

                    'strInsertCmd = "INSERT INTO PickingList " _
                    '    & "(List_ID,List_Date,List_Time,Number_Of_Items," _
                    '    & "Creator_ID,Picker_ID,List_Type,List_Status)" _
                    '    & " VALUES ('" & staData(0) & "','" & staData(1) & "','" _
                    '    & staData(2) & "'," & CInt(staData(3)) & ",'" _
                    '    & staData(4) & "','" & staData(5) & "','" _
                    '    & staData(6) & "','" & staData(7) & "')"

                    ' Chnaged data type
                    'Support: Assigning the values for the parameters
                    With sqlListCommd
                        .Parameters(0).Value = CInt(staData(0))
                        .Parameters(1).Value = staData(1)
                        .Parameters(2).Value = CInt(staData(2))
                        .Parameters(3).Value = CInt(staData(3))
                        .Parameters(4).Value = CInt(staData(4))
                        .Parameters(5).Value = CInt(staData(5))
                        .Parameters(6).Value = staData(6)
                        .Parameters(7).Value = staData(7)
                    End With

                    'sqlCommd.CommandText = strListInsertCmd
                    'Executing the querry
                    Try
                        'Executing the querry
                        sqlListCommd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        If ex.Message.Contains("duplicate") Then
                            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: duplicate " _
                                                                  & "record found " + strLine, _
                                                                  Logger.LogLevel.RELEASE)
                            'Increment the line count.
                            iCount = iCount + 1
                            'Read next line
                            strLine = rdrFileReader.ReadLine()
                            Continue While
                        Else
                            Throw ex
                        End If
                    End Try
                    'Parameterised Query

                ElseIf strLine.StartsWith("I") Then
                    'To insert the record starting with I to a different table.
                    strLine = strLine.Substring(2)
                    staData = strLine.Split(m_Delimiter)

                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = 0
                        End If
                    Next
                    'For inserting the parsed data to DB
                    'strItemsInsertCmd = "INSERT INTO PickListItems " _
                    '    & "(List_ID,Sequence_Number,Boots_Code,Shelf_Qty," _
                    '    & " Fill_Qty,Sales_At_Shelf_Count,Back_Shop_Count," _
                    '    & "Sales_At_Pick,Item_Status)" _
                    '    & " VALUES ('" & staData(0) & "','" _
                    '    & staData(1) & "','" & staData(2) & "'," _
                    '    & CInt(staData(3)) & "," & CInt(staData(4)) & "," _
                    '    & CInt(staData(5)) & "," & CInt(staData(6)) & "," _
                    '    & CInt(staData(7)) & ",'" & staData(8) & "')"


                    'Support :Changed to Parameterised Query
                    'Support: Assigning the values for the parameters
                    'Changed
                    With sqlItemsCommd
                        .Parameters(0).Value = CInt(staData(0))
                        .Parameters(1).Value = CInt(staData(1))
                        .Parameters(2).Value = CInt(staData(2))
                        .Parameters(3).Value = CInt(staData(3))
                        .Parameters(4).Value = CInt(staData(4))
                        .Parameters(5).Value = CInt(staData(5))
                        .Parameters(6).Value = CInt(staData(6))
                        .Parameters(7).Value = CInt(staData(7))
                        .Parameters(8).Value = staData(8)
                        .Parameters(9).Value = CInt(staData(9))
                    End With
                    Try
                        'Executing the querry
                        sqlItemsCommd.ExecuteNonQuery()
                        If staData(8) = "U" Then
                            'increment item details number.
                            iNumItems = iNumItems + 1
                        End If
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        If ex.Message.Contains("duplicate") Then
                            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: duplicate " _
                                                                  & "record found " + strLine, _
                                                                  Logger.LogLevel.RELEASE)
                            'Increment the line count.
                            iCount = iCount + 1
                            'Read next line
                            strLine = rdrFileReader.ReadLine()
                            Continue While
                        Else
                            Throw ex
                        End If
                    End Try
                    iIndx = 10
                    While (iIndx < staData.Length)
                        If CInt(staData(iIndx)) = 0 Then
                            Exit While
                        End If
                        With sqlPlanogramCommd
                            .Parameters(0).Value = CInt(staData(0))
                            .Parameters(1).Value = CInt(staData(2))
                            .Parameters(2).Value = CInt(staData(iIndx))
                            .Parameters(3).Value = CInt(staData(iIndx + 1))
                            .Parameters(4).Value = CInt(staData(iIndx + 2))
                            .Parameters(5).Value = CInt(staData(iIndx + 3))
                        End With
                        iIndx += 4
                        Try
                            sqlPlanogramCommd.ExecuteNonQuery()

                        Catch ex As Exception
                            If ex.Message.Contains("duplicate") Then
                                objAppContainer.objLogger.WriteAppLog("ActiveFileParser: duplicate " _
                                                                      & "record found " + strLine, _
                                                                      Logger.LogLevel.RELEASE)
                                'Increment the line count.
                                'iCount = iCount + 1
                                Continue While
                            Else
                                Throw ex
                            End If
                        End Try
                    End While
                   
                    'Read next line
                    'strLine = rdrFileReader.ReadLine()
                    'sqlCommd.CommandText = strItemsInsertCmd
                    'Executing the querry
                    
                ElseIf strLine.StartsWith("H") Then
                    If Not strLine.Contains(strFileName) Then
                        objAppContainer.objLogger.WriteAppLog("ActiveFileParser: Invalid file format. The " _
                                                    & "file name in the header record is not matched.", _
                                                    Logger.LogLevel.RELEASE)
                        Return False
                    End If
                End If
                'Read next line
                strLine = rdrFileReader.ReadLine()
            End While
            'To update the count for the last pickign list.
            If strListID <> Nothing Then
                'Update the picking list detail record with new count.
                'For inserting the parsed data to DB
                ' Removed single quotes
                strUpdateCmd = "UPDATE PickingList SET Number_Of_Items =" _
                                & iNumItems & " WHERE List_ID=" & strListID
                'Set command text in the sql command
                sqlCommd.CommandText = strUpdateCmd
                Try
                    'Executing the querry
                    sqlCommd.ExecuteNonQuery()
                Catch ex As Exception
                    objAppContainer.objLogger.WriteAppLog("ActiveFileParser: Updating the no. of items" _
                                                          + strLine, Logger.LogLevel.RELEASE)
                Finally
                    iNumItems = 0
                End Try
            End If
            'Close the reader
            rdrFileReader.Close()
            'Check iCount against the count in "T" record.
            staData = strLine.Split(",")
            'Check if all the records are inserted.
            If iCount = Val(staData(1)) Then
                'Delete the reference file used by this function.
                File.Delete(strFilePath & strFileName)
            End If
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus(Macros.PICKING, "P", "NA")
        Catch ex As FileNotFoundException
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog(strFileName & " file not found", _
                                                                Logger.LogLevel.RELEASE)
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus(Macros.PICKING, "F", "File not found.")
            'return false in case of any exception.
            Return False
        Catch ex As Exception
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog(ex.Message.ToString(), _
                                                                Logger.LogLevel.RELEASE)
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus(Macros.PICKING, "F", ex.Message.Trim())
            'return false in case of any exception.
            Return False
        Finally
            sqlListCommd = Nothing
            sqlItemsCommd = Nothing
            sqlPlanogramCommd = Nothing
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        Return True
    End Function
    ''' <summary>
    ''' To parse COUNT.csv and update the DB
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function ParseCountFile(ByVal strFilePath As String, ByVal strFileName As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0
        Dim StrListCommand As String
        Dim strItemsCommand As String
        Dim strPlannersCommand As String
        Dim sqlListCmd As SqlCeCommand
        Dim sqlItemsCommand As SqlCeCommand
        Dim sqlPlannersCommand As SqlCeCommand
        Dim iIndx As Integer

        Try
            'Dim sqlCommd As New SqlCeCommand()
            'Assign SQL connection to the command.s
            'sqlCommd.Connection = m_SqlConn

            'Load the file for reading.
            rdrFileReader = New System.IO.StreamReader(strFilePath + strFileName)
            'Reading a line from the CSV file
            strLine = rdrFileReader.ReadLine()
            'Support : Defining the Text of the insert query
            StrListCommand = "INSERT INTO CountList (List_ID, List_Count_Date, " _
            & "Number_Of_Items, List_Name, Status, List_Type, Legacy_List_ID, OutStanding_SFCount, " _
            & "OutStanding_BSCount, OutStanding_OSSRCount) " _
            & "VALUES(@List_ID, @List_Count_Date, @Number_Of_Items, @List_Name, @Status, @List_Type, " _
            & "@Legacy_List_ID, @OutStanding_SFCount, @OutStanding_BSCount, @OutStanding_OSSRCount)"

            strItemsCommand = "Insert Into CountlistItems (List_ID, Sequence_Number, " _
            & "Boots_Code, Back_Shop_Count, Sales_Floor_Count, OSSR_Count, Sales_At_POD_Dock, " _
            & "Back_Shop_PSP_Count, OSSR_PSP_Count, Stock_Figure, Date_Of_Last_Count) " _
            & " VALUES (@List_ID, @Sequence_Number, @Boots_Code, @Back_Shop_Count, @Sales_Floor_Count, " _
            & "@OSSR_Count, @Sales_At_POD_Dock, @Back_Shop_PSP_Count,@OSSR_PSP_Count, " _
            & "@Stock_Figure, @Date_Of_Last_Count)"

            strPlannersCommand = "INSERT INTO CountListPlanograms (List_ID, Boots_Code, Module_ID, " _
            & "Module_Seq, Repeat_Count, Fill_Quantity) " _
            & "VALUES(@List_ID, @Boots_Code, @Module_ID, @Module_Seq, @Repeat_Count, " _
            & "@Fill_Quantity)"

            'Support: Initialising the Command
            sqlListCmd = New SqlCeCommand(StrListCommand, m_SqlConn)
            sqlItemsCommand = New SqlCeCommand(strItemsCommand, m_SqlConn)
            sqlPlannersCommand = New SqlCeCommand(strPlannersCommand, m_SqlConn)

            'Support: Defining the Parameters
            ' Changed data type
            With sqlListCmd.Parameters
                .Add("@List_ID", SqlDbType.SmallInt)
                .Add("@List_Count_Date", SqlDbType.NVarChar)
                .Add("@Number_Of_Items", SqlDbType.SmallInt)
                .Add("@List_Name", SqlDbType.NVarChar)
                .Add("@Status", SqlDbType.NVarChar)
                .Add("@List_Type", SqlDbType.NVarChar)
                .Add("@Legacy_List_ID", SqlDbType.SmallInt)
                .Add("@OutStanding_SFCount", SqlDbType.SmallInt)
                .Add("@OutStanding_BSCount", SqlDbType.SmallInt)
                .Add("@OutStanding_OSSRCount", SqlDbType.SmallInt)
            End With

            With sqlItemsCommand.Parameters
                .Add("@List_ID", SqlDbType.SmallInt)
                .Add("@Sequence_Number", SqlDbType.SmallInt)
                .Add("@Boots_Code", SqlDbType.Int)
                .Add("@Back_Shop_Count", SqlDbType.Int)
                .Add("@Sales_Floor_Count", SqlDbType.Int)
                .Add("@OSSR_Count", SqlDbType.Int)
                .Add("@Sales_At_POD_Dock", SqlDbType.Int)
                .Add("@Back_Shop_PSP_Count", SqlDbType.Int)
                .Add("@OSSR_PSP_Count", SqlDbType.Int)
                .Add("@Stock_Figure", SqlDbType.Int)
                .Add("@Date_Of_Last_Count", SqlDbType.NVarChar)

            End With

            With sqlPlannersCommand.Parameters
                .Add("@List_ID", SqlDbType.SmallInt)
                .Add("@Boots_Code", SqlDbType.Int)
                .Add("@Module_ID", SqlDbType.Int)
                .Add("@Module_Seq", SqlDbType.SmallInt)
                .Add("@Repeat_Count", SqlDbType.SmallInt)
                .Add("@Fill_Quantity", SqlDbType.SmallInt)
            End With

            sqlListCmd.Prepare()
            sqlItemsCommand.Prepare()
            sqlPlannersCommand.Prepare()

            While Not (strLine.StartsWith("T"))
                strLine = strLine.Trim()
                If strLine.StartsWith("D") Then
                    strLine = strLine.Substring(2)
                    'Parsing the line
                    staData = strLine.Split(m_Delimiter)

                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = 0
                        End If
                    Next

                    If Val(staData(1)) = 0 Then
                        staData(1) = DEFAULT_DATE
                    End If

                    'For inserting the parsed data to DB
                    'strInsertCmd = "INSERT INTO CountList " _
                    '    & "(List_ID,List_Count_Date," _
                    '    & "Number_Of_Items,BC,Status,List_Type," _
                    '    & "Legacy_List_ID,Outstanding_SFCount," _
                    '    & "OutStanding_BSCount,Business_Unit_Name)" _
                    '    & " VALUES ('" & staData(0) & "','" _
                    '    & staData(1) & "'," & CInt(staData(2)) & ",'" _
                    '    & staData(3) & "','" & staData(5) _
                    '    & "','" & staData(6) & "','" & staData(7) & "'," _
                    '    & CInt(staData(8)) & "," & CInt(staData(9)) & ",'" & _
                    '    staData(4).Trim() & "')"

                    'Support
                    'Changed the query to parameterised query
                   
                    'Support: Assigning the values for the parameters
                    With sqlListCmd
                        .Parameters(0).Value = CInt(staData(0))
                        .Parameters(1).Value = staData(1)
                        .Parameters(2).Value = CInt(staData(2))
                        .Parameters(3).Value = staData(3).Trim()
                        .Parameters(4).Value = staData(4)
                        .Parameters(5).Value = staData(5)
                        .Parameters(6).Value = CInt(staData(6))
                        .Parameters(7).Value = CInt(staData(7))
                        .Parameters(8).Value = CInt(staData(8))
                        .Parameters(9).Value = CInt(staData(9))

                    End With


                    'sqlCommd.CommandText = strInsertCmd
                    'Executing the querry
                    Try
                        'Executing the querry
                        'sqlCommd.ExecuteNonQuery()
                        sqlListCmd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        If ex.Message.Contains("duplicate") Then
                            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: duplicate " _
                                                                  & "record found " + strLine, _
                                                                  Logger.LogLevel.RELEASE)
                            'Increment the line count.
                            iCount = iCount + 1
                            'Read next line
                            strLine = rdrFileReader.ReadLine()
                            Continue While
                        Else
                            Throw ex
                        End If
                    End Try
                ElseIf strLine.StartsWith("I") Then
                    strLine = strLine.Substring(2)
                    staData = strLine.Split(m_Delimiter)

                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = 0
                        End If
                    Next

                    ' Convert zeros into default date
                    If Val(staData(10)) = 0 Then
                        staData(10) = DEFAULT_DATE
                    End If
                    'For inserting the parsed data to DB
                    'strInsertCmd = "INSERT INTO CountListItems " _
                    '    & "(Boots_Code,List_ID,Sequence_Number,Back_Shop_Count," _
                    '    & "Sales_Floor_Count,Sales_At_SFCount," _
                    '    & "Sales_At_BSCount)" _
                    '    & " VALUES ('" & staData(2) & "','" & staData(0) & "','" _
                    '    & staData(1) & "'," & CInt(staData(3)) & "," _
                    '    & CInt(staData(4)) & "," & CInt(staData(5)) & "," _
                    '    & CInt(staData(6)) & ")"
                  
                    'Support: Assigning the values for the parameters
                    ' Changed
                    With sqlItemsCommand
                        .Parameters(0).Value = CInt(staData(0))
                        .Parameters(1).Value = CInt(staData(1))
                        .Parameters(2).Value = CInt(staData(2))
                        .Parameters(3).Value = CInt(staData(3))
                        .Parameters(4).Value = CInt(staData(4))
                        .Parameters(5).Value = CInt(staData(5))
                        .Parameters(6).Value = CInt(staData(6))
                        .Parameters(7).Value = CInt(staData(7))
                        .Parameters(8).Value = CInt(staData(8))
                        .Parameters(9).Value = CInt(staData(9))
                        .Parameters(10).Value = staData(10)
                    End With
                    Try
                        'Executing the querry
                        'sqlCommd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        sqlItemsCommand.ExecuteNonQuery()
                        iCount = iCount + 1
                    Catch ex As Exception
                        If ex.Message.Contains("duplicate") Then
                            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: duplicate " _
                                                                  & "record found " + strLine, _
                                                                  Logger.LogLevel.RELEASE)
                            'Increment the line count.
                            iCount = iCount + 1
                            'Read next line
                            strLine = rdrFileReader.ReadLine()
                            Continue While
                        Else
                            'Throw the exception outside.
                            Throw ex
                        End If
                    End Try
                    iIndx = 11
                    While (iIndx < staData.Length)
                        If CInt(staData(iIndx)) = 0 Then
                            Exit While
                        End If
                        With sqlPlannersCommand
                            .Parameters(0).Value = CInt(staData(0))
                            .Parameters(1).Value = CInt(staData(2))
                            .Parameters(2).Value = CInt(staData(iIndx))
                            .Parameters(3).Value = CInt(staData(iIndx + 1))
                            .Parameters(4).Value = CInt(staData(iIndx + 2))
                            .Parameters(5).Value = CInt(staData(iIndx + 3))
                        End With
                        iIndx += 4
                        Try
                            sqlPlannersCommand.ExecuteNonQuery()
                        Catch ex As Exception
                            If ex.Message.Contains("duplicate") Then
                                objAppContainer.objLogger.WriteAppLog("ActiveFileParser: duplicate " _
                                                                      & "record found " + strLine, _
                                                                      Logger.LogLevel.RELEASE)
                                'Increment the line count.
                                'iCount = iCount + 1
                                Continue While
                            Else
                                'Throw the exception outside.
                                Throw ex
                            End If
                        End Try
                    End While
                   
                    'Read next line
                    'strLine = rdrFileReader.ReadLine()
                    
                ElseIf strLine.StartsWith("H") Then
                    If Not strLine.Contains(strFileName) Then
                        objAppContainer.objLogger.WriteAppLog("ActiveFileParser: Invalid file format. The " _
                                                    & "file name in the header record is not matched.", _
                                                    Logger.LogLevel.RELEASE)
                        Return False
                    End If
                End If


                'Read next line
                strLine = rdrFileReader.ReadLine()
            End While
            'Close the reader
            rdrFileReader.Close()
            'Check iCount against the count in "T" record.
            staData = strLine.Split(",")
            'Check if all the records are inserted.
            If iCount = Val(staData(1)) Then
                'Delete the reference file used by this function.
                File.Delete(strFilePath & strFileName)
            End If
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus(Macros.COUNT, "P", "NA")
        Catch ex As FileNotFoundException
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            strFileName & " file not found", _
                                            Logger.LogLevel.RELEASE)
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus(Macros.COUNT, "F", "File not found.")
            'return false in case of any exception.
            Return False
        Catch ex As Exception

            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog(ex.Message.ToString(), _
                                                                Logger.LogLevel.RELEASE)
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus(Macros.COUNT, "F", ex.Message.Trim())
            'return false in case of any exception.
            Return False
        Finally
            sqlItemsCommand = Nothing
            sqlListCmd = Nothing
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        Return True
    End Function
End Class
#End If