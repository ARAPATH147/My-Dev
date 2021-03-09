#If NRF Then
Imports System.IO
Imports System.Data
Imports System.Data.SqlServerCe
Imports System.Text.RegularExpressions

'''****************************************************************************
''' <FileName>ActiveFileParser.vb</FileName>
''' <summary>
''' Responsible for parsing the Active files and populate the DB. This class is
''' also responsible for deleting the data present in the Active Tables before 
''' populating the data in the database.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>08-Dec-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''****************************************************************************
Public Class ActiveFileParser
    Private strDBPath As String = ConfigDataMgr.GetInstance.GetParam(ConfigKey.CONN_STRING)
    Private strConnectionString As String = "Data Source = " & strDBPath
    Private DELIMETER As String = Macros.DELIMITER

    Private iRetryDelete As Integer = Macros.DELETERETRY

    'Create an instance of the SQL CE Connection.
    Dim sqlConn As SqlCeConnection = Nothing

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
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
            If strDBPath <> Nothing And File.Exists(strDBPath) Then
                sqlConn = New SqlCeConnection(strConnectionString)
                sqlConn.Open()
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
                'Exit the application.
                objAppContainer.AppTerminate()
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
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
                'Catch the exception and write the stack trace.
                objAppContainer.objLogger.WriteAppLog("ActiveFileParser: Exception occurred" & _
                                                      ex.StackTrace, Logger.LogLevel.RELEASE)
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
        Dim iRetryCount As Integer = iRetryDelete
        Dim iCount As Integer = 0
        Dim strInsertCmd As String = ""
        Try
            Dim sqlCommd As New SqlCeCommand()
            Dim sqlCompactEngine As SqlCeEngine
            sqlCommd.Connection = sqlConn
            sqlCompactEngine = New SqlCeEngine(strConnectionString)
            aActiveTable = Macros.ACTIVETABLES.Split(DELIMETER)
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
                iRetryCount = iRetryDelete
            End While
            'update the active data availability in the config file.
            ConfigDataMgr.GetInstance.SetParam(ConfigKey.ACTIVE_DATA_AVAILABILITY, "False")

        Catch ex As Exception
            'Catch the exception and write the stack trace.
            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: Exception occurred" & _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            'return status. Modified to return true for handling defect  in pilot : 65
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
            Case Macros.CREDIT
                Return Me.ParseCreditFile(strLocalPath, strFileName, iFileIndex)
        End Select
    End Function
    ''' <summary>
    ''' To parse CREDIT.csv and update the DB
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function ParseCreditFile(ByVal strFilePath As String, ByVal strFileName As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        'For reading CREDIT.csv file
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim iCount As Integer = 0
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim strRegExp1 As Regex
        Dim strRegExp2 As Regex
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Assign SQL connection to the command.
            sqlCommd.Connection = sqlConn
            'load the file to the reader.
            rdrFileReader = New System.IO.StreamReader(strFilePath & strFileName, True)
            'Reading a line from the CSV file
            strLine = rdrFileReader.ReadLine()

            While Not (strLine.StartsWith("T"))
                strLine = strLine.Trim()
                If strLine.StartsWith("D") Then
                    strLine = strLine.Substring(2)
                    'Parsing the line
                    staData = strLine.Split(DELIMETER)

                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex).Trim() = "" _
                            Or staData(iIndex).Trim() = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = 0
                        End If
                    Next
                    'Check for the items that might contain all 0s instead of value.
                    'Replace such instances by a single 0.
                    strRegExp1 = New Regex("^[1-9]$")
                    strRegExp2 = New Regex("^\s*0*\s*0*\s*$")
                    If Not strRegExp1.IsMatch(staData(2)) And strRegExp2.IsMatch(staData(2)) Then
                        staData(2) = "0"
                    End If
                    If Not strRegExp1.IsMatch(staData(10)) And strRegExp2.IsMatch(staData(10)) Then
                        staData(10) = "0"
                    End If
                    If Not strRegExp1.IsMatch(staData(15)) And strRegExp2.IsMatch(staData(15)) Then
                        staData(15) = "0"
                    End If
                    If Not strRegExp1.IsMatch(staData(11)) And strRegExp2.IsMatch(staData(11)) Then
                        staData(11) = "0"
                    End If
                    If Not strRegExp1.IsMatch(staData(12)) And strRegExp2.IsMatch(staData(12)) Then
                        staData(12) = "0"
                    End If

                    ' Changed
                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO CreditClaimList " _
                        & "(List_ID,Claim_Type,UOD_Number,Claim_Status," _
                        & "Item_Count,UOD_Qty,Stock_Adjusted,Supply_Route," _
                        & "Display_Location,ClaimBC,Recall_Number," _
                        & "Auth_Code,Supplier,Return_Method," _
                        & "Carrier_Code,Bird_Number,Reason_Code," _
                        & "Receiving_Store_No,Destination,Warehouse_Type," _
                        & "UOD_Type,Damage_Reason," _
                        & "UOD_Open_Date,UOD_Despatch_Date,UOD_Open_Time," _
                        & "Creator_ID,RF_Status)" _
                        & " VALUES ('" & staData(0) & "','" _
                        & staData(1) & "','" & staData(2) & "','" _
                        & staData(3) & "'," & staData(4) _
                        & "," & staData(5) & ",'" & staData(6) & "','" _
                        & staData(7) & "','" & staData(8) & "','" _
                        & staData(9) & "','" & staData(10) & "','" _
                        & staData(11) & "','" & staData(12) & "','" _
                        & staData(13) & "','" & staData(14) & "','" _
                        & staData(15) & "','" & staData(16) & "','" _
                        & staData(17) & "','" & staData(18) & "','" _
                        & staData(19) & "','" & staData(20) & "','" _
                        & staData(21) & "','" & staData(22) & "','" _
                        & staData(23) & "','" & staData(24) & "','" _
                        & staData(25) & "','" & staData(26) & "')"
                    sqlCommd.CommandText = strInsertCmd
                    'Executing the querry
                    Try
                        'Executing the querry
                        sqlCommd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        'If record is duplicate one ignore this and continue with next.
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
                    'Counter to keep track of the record number inserted to the db.
                    iCount = iCount + 1
                ElseIf strLine.StartsWith("I") Then
                    strLine = strLine.Substring(2)
                    staData = strLine.Split(DELIMETER)

                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " _
                            Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = 0
                        End If
                    Next

                    ' changed
                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO CreditClaimListItems " _
                        & "(List_ID,Boots_Code,Claim_Qty,Item_Status)" _
                        & " VALUES ('" & staData(0) & "'," _
                        & staData(1) & "," _
                        & staData(2) & ",'" & staData(3) & "')"

                    sqlCommd.CommandText = strInsertCmd
                    'Executing the querry
                    Try
                        'Executing the querry
                        sqlCommd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        If ex.Message.Contains("duplicate") Then
                            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: duplicate " _
                                                                  & "record found " + strLine, _
                                                                  Logger.LogLevel.DEBUG)
                            'Increment the line count.
                            iCount = iCount + 1
                            'Read next line
                            strLine = rdrFileReader.ReadLine()
                            Continue While
                        Else
                            Throw ex
                        End If
                    End Try
                    'Counter to keep track of the record number inserted to the db.
                    iCount = iCount + 1
                ElseIf strLine.StartsWith("H") Then
                    If Not strLine.Contains(strFileName) Then
                        objAppContainer.objLogger.WriteAppLog("ActiveFileParser: Invalid file format. The " _
                                                    & "file name in the header record is not matched.", _
                                                    Logger.LogLevel.RELEASE)
                        'return status
                        Return False
                    End If
                End If
                'Read next line
                strLine = rdrFileReader.ReadLine()
            End While
            'Check iCount against the count in "T" record.
            strLine = strLine.Substring(2).Trim()
            'Check if all the records are inserted.
            If iCount = Val(strLine) Then
                'Close the file reader.
                rdrFileReader.Close()
                'Delete the reference file used by this function.
                File.Delete(strFilePath & strFileName)
            End If
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus(Macros.CREDIT, "P", "NA")
        Catch ex As FileNotFoundException
            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: File" & strFileName & "not found.", _
                                                  Logger.LogLevel.RELEASE)
            'close the reader
            rdrFileReader.Close()
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus(Macros.CREDIT, "F", "File not found.")
            'Return false in case of error
            Return False
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: Exception occurred" & _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus(Macros.CREDIT, "F", ex.Message.Trim())
            'Return false in case of error
            Return False
        Finally
            'close the reader
            rdrFileReader.Close()
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        Return True
    End Function
End Class
#End If
