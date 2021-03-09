#If NRF Then


Imports System.IO
Imports System.Data
Imports System.Data.SqlServerCe
Imports System.Globalization
'''****************************************************************************
''' <FileName>ActiveFileParser.vb</FileName>
''' <summary>
''' Responsible for parsing the Active files and populate the DB. This class is
''' also responsible for deleting the data present in the Active Tables before 
''' populating the data in the database.
''' TODO:Terminate the DB connection after use.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''****************************************************************************
''' 
'''****************************************************************************
'''* Modification Log 
'''**************************************************************************** 
'''* No:        Author             Date                 Description 
'''* 1.1     Christopher Kitto  14/04/2015  Modified as part of DALLAS project.
'''                  (CK)                   Amended ParseFile() and added new
'''                                         function ParseWHUODFile() to parse
'''                                         new active file - WHUOD.CSV
'''****************************************************************************


Public Class ActiveFileParser
    Private m_DBPath As String = ConfigDataMgr.GetInstance.GetParam(ConfigKey.CONN_STRING)
    Private m_ConnString As String = "Data Source = " & m_DBPath
    Private m_Delimiter As Char = Macros.DELIMITER
    Private m_RetryDelete As Integer = Macros.DELETERETRY
    'Create an instance of the SQL CE Connection.
    Dim sqlConn As SqlCeConnection = Nothing




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
                sqlConn = New SqlCeConnection(m_ConnString)
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
                'return the status
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
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
            sqlCommd.Connection = sqlConn
            sqlCompactEngine = New SqlCeEngine(m_ConnString)
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
                        Return False
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
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' To invoke function corresponding to the name of the file.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseFile(ByVal strFileName As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        Dim strLocalPath = ConfigDataMgr.GetInstance().GetParam(ConfigKey.ACTIVE_FILE_PATH)
        Select Case strFileName
            Case Macros.ASN
                If ConfigDataMgr.GetInstance.GetParam(ConfigKey.ASN_ACTIVE) = "Y" _
                AndAlso ConfigDataMgr.GetInstance.GetParam(ConfigKey.DIRECTS_ACTIVE) = "Y" Then
                    Return Me.ParseASNFile(strLocalPath, _
                                                strFileName, _
                                                iFileIndex)
                Else
                    Return True
                End If
            Case Macros.DIRECTS
                If ConfigDataMgr.GetInstance.GetParam(ConfigKey.DIRECTS_ACTIVE) = "Y" Then
                    Return Me.ParseDirectFile(strLocalPath, _
                                      strFileName, _
                                      iFileIndex)
                Else
                    Return True
                End If
            Case Macros.SSCUODOT
                If ConfigDataMgr.GetInstance.GetParam(ConfigKey.UOD_ACTIVE) = "Y" Then
                    Return Me.ParseSSCUODOTFile(strLocalPath, _
                                          strFileName, _
                                          iFileIndex)
                Else
                    Return True
                End If

                ' V1.1 - CK
                ' Parsing WHUOD.CSV file 
            Case Macros.WHUOD
                If objAppContainer.bDallasPosReceiptEnabled = True Then
                    Return Me.ParseWHUODFile(strLocalPath, _
                                             strFileName, _
                                             iFileIndex)
                Else
                    Return True
                End If
        End Select
    End Function
    ''' <summary>
    ''' To parse ASN.csv and update the DB
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function ParseASNFile(ByVal strFilePath As String, ByVal strFileName As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0

        Try
            Dim sqlCommd As New SqlCeCommand()
            'Assign SQL connection to the command.s
            sqlCommd.Connection = sqlConn

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
                        If (staData(iIndex) = " " _
                            Or staData(iIndex) = Nothing) Then

                            'Null values are replaced with '0'
                            staData(iIndex) = "0"
                        End If
                    Next

                    'Minu Removed single quotes and cint, cchar
                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO ASNList " _
                        & "(ASN_Number,Supplier_Ref,Carton_Number,Cartons_In_ASN," _
                        & "Status,Order_Number,Order_Suffix,BC,Exp_Delivery_Date," _
                        & "Exp_Delivery_Time,Total_Item_In_Carton)" _
                        & " VALUES ('" & staData(0) & "'," & staData(1) & "," _
                        & staData(2) & "," & staData(3) & ",'" & staData(4) _
                        & "'," & staData(5) & ",'" & staData(6) & "','" _
                        & staData(7) & "','" & staData(8) & "'," & staData(9) & "," _
                        & staData(10) & ")"
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
                ElseIf strLine.StartsWith("I") Then
                    strLine = strLine.Substring(2)
                    staData = strLine.Split(m_Delimiter)

                    ' So exception occurs.Clarify this.
                    For iIndex As Integer = 2 To staData.GetUpperBound(0) - 2 Step 3
                        'For inserting the parsed data to DB
                        If staData(iIndex).Trim() <> Nothing And _
                            staData(iIndex) <> "0" And _
                            staData(iIndex) <> "000000" Then

                            'Minu removed single quotes and cint
                            strInsertCmd = "INSERT INTO ASNListItems " _
                                & "(Carton_Number,Boots_Code,Supplier_Number," _
                                & "Despatched_Qty,Booked_Qty)" _
                                & " VALUES (" & staData(1) & "," _
                                & GenerateBCwithCDV(staData(iIndex)) & "," _
                                & staData(0) & "," & staData(iIndex + 1) & "," _
                                & staData(iIndex + 2) & ")"

                            sqlCommd.CommandText = strInsertCmd
                            'Executing the querry
                            Try
                                'Executing the querry
                                sqlCommd.ExecuteNonQuery()
                            Catch ex As Exception
                                If ex.Message.Contains("duplicate") Then
                                    objAppContainer.objLogger.WriteAppLog("ActiveFileParser: duplicate " _
                                                                          & "record found " + strLine, _
                                                                          Logger.LogLevel.DEBUG)
                                    'Read next iteration of the data.
                                    Continue For
                                Else
                                    'Counter to keep track of the record number inserted to the db.
                                    iCount = iCount + 1
                                    'throw exception to the next level.
                                    Throw ex
                                End If
                            End Try
                        End If
                    Next
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
            'Check iCount against the count in "T" record.
            strLine = strLine.Substring(2).Trim()
            'Check if all the records are inserted.
            If iCount = Val(strLine) Then
                'Close the file reader.
                rdrFileReader.Close()
                'Delete the reference file used by this function.
                File.Delete(strFilePath & strFileName)
            End If
        Catch ex As FileNotFoundException
            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: File" & strFileName & "not found.", _
                                                  Logger.LogLevel.RELEASE)
            'close the reader
            rdrFileReader.Close()
            'Return false in case of error
            Return False
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: Exception occurred" & _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            'Return false in case of error
            Return False
        Finally
            rdrFileReader.Close()
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        Return True
    End Function
    ''' <summary>
    ''' To parse directs.csv and update the DB
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function ParseDirectFile(ByVal strFilePath As String, ByVal strFileName As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0
        Dim strBC As String = Nothing
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Assign SQL connection to the command.s
            sqlCommd.Connection = sqlConn

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
                    'convert the business centre letter to hexa decimal value.
                    strBC = Conversion.Hex(Strings.Asc(staData(3).ToString()))
                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " _
                            Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = "0"
                        End If
                    Next

                    'Minu Removed single quotes and cint
                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO DirectList " _
                        & "(Order_No,Supplier,Order_Suffix,BC,Source," _
                        & "No_Of_Order_Items,No_Of_Singles,No_Of_Items_BookedIn," _
                        & "No_Of_Items_BkdLast,Order_Date,Exp_Delivery_Date," _
                        & "Confirm_Flag,Confirm_Date,Last_Updated_Start_Time," _
                        & "Last_Updated_End_Time,On_Sale_Date)" _
                        & " VALUES (" & staData(1) & "," & staData(0) & ",'" _
                        & staData(2) & "','" & strBC & "','" & staData(4) & "'," _
                        & staData(5) & "," & staData(6) & "," _
                        & staData(7) & "," & staData(8) & ",'" _
                        & staData(9) & "','" _
                        & staData(10) & "','" & staData(11) & "','" & staData(12) & "'," _
                        & staData(13) & "," & staData(14) & ",'" & staData(15) & "')"
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
                ElseIf strLine.StartsWith("I") Then
                    strLine = strLine.Substring(2)
                    staData = strLine.Split(m_Delimiter)

                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " _
                            Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = "0"
                        End If
                    Next

                    'Minu Removed single quotes
                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO DirectListItems " _
                        & "(Boots_Code,Order_No,List_ID,Supplier,Order_Suffix,BC," _
                        & "Source,Exp_Quantity,Qty_In_Good_Cond," _
                        & "Qty_In_Damaged_Cond,Qty_Stolen," _
                        & "Last_Qty_In_Good_Cond,Last_Qty_In_Damaged_cond," _
                        & "Last_Qty_In_Stolen_Cond)" _
                        & " VALUES (" & staData(6).TrimStart("0") & "," _
                        & staData(2) & "," _
                        & staData(0) & "," & staData(1) & ",'" & staData(3) & "','" _
                        & staData(4) & "','" & staData(5) & "'," & staData(7) & "," _
                        & staData(8) & "," & staData(9) & "," _
                        & staData(10) & "," _
                        & staData(11) & "," & staData(12) & "," _
                        & staData(13) & " )"

                    sqlCommd.CommandText = strInsertCmd
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
        Catch ex As FileNotFoundException
            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: File" & strFileName & "not found.", _
                                                  Logger.LogLevel.RELEASE)
            'close the reader
            rdrFileReader.Close()
            'Return false in case of error
            Return False
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: Exception occurred" & _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            'Return false in case of error
            Return False
        Finally
            rdrFileReader.Close()
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        Return True
    End Function
    ''' <summary>
    ''' To parse SSCUODOT.CSV.csv and update the DB
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function ParseSSCUODOTFile(ByVal strFilePath As String, ByVal strFileName As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strInsertCmd As String = ""
        Dim strLine As String = Nothing
        Dim iCount As Integer = 0
        Dim strUODNumber As String = Nothing
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Assign SQL connection to the command.s
            sqlCommd.Connection = sqlConn

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

                    'Store the UOD number in a variable.
                    strUODNumber = staData(0).ToString()

                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " _
                            Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = "0"
                        End If
                    Next

                    'Minu changed
                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO UODOUTDetail " _
                        & "(UOD_License_Number,Despatch_Date,Hierarchy_Level," _
                        & "Estimated_Delivery_Date,Delivery_Date,Delivery_Time," _
                        & "Outer_Type,Category,Reason,Warehouse_Area," _
                        & "Booked_In,Stock_Updated,Auto_Booked,Audited," _
                        & "Partial_Booked,GIT_Note_Match,Number_Childrens," _
                        & "Number_Of_Items,Ultimate_License_Number,Immediate_License_Number," _
                        & "Booked_In_Date,Booked_In_Time,Operator_ID,Audit_ID," _
                        & "Driver_ID,BookIn_Method,BookIn_Level)" _
                        & " VALUES (" & staData(0) & ",'" & staData(1) & "'," _
                        & staData(2) & ",'" & staData(3) & "','" & staData(4) & "'," _
                        & staData(5) & ",'" & staData(6) & "','" _
                        & staData(7) & "','" & staData(8) & "'," _
                        & staData(9) & ",'" & staData(10) & "','" _
                        & staData(11) & "','" & staData(12) & "','" _
                        & staData(13) & "','" & staData(14) & "','" _
                        & staData(15) & "','" & staData(16) & "','" _
                        & staData(17) & "'," & staData(18) & "," _
                        & staData(19) & ",'" & staData(20) & "'," _
                        & staData(21) & "," & staData(22) & "," _
                        & staData(23) & "," & staData(24) & ",'" _
                        & staData(25) & "','" & staData(26) & "')"

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
                ElseIf strLine.StartsWith("I") Then
                    strLine = strLine.Substring(2)
                    staData = strLine.Split(m_Delimiter)

                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " _
                            Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = "0"
                        End If
                    Next

                    'Minu changed
                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO UODOUTItems " _
                        & "(UOD_License_Plate,Boots_Code,Despatch_Quantity,Audit_Quantity)" _
                        & " VALUES (" & strUODNumber & "," & GenerateBCwithCDV(staData(0)) & "," _
                        & staData(1) & "," & staData(2) & ")"

                    sqlCommd.CommandText = strInsertCmd
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
                ElseIf strLine.StartsWith("S") Then
                    strLine = strLine.Substring(2)
                    staData = strLine.Split(m_Delimiter)

                    For iIndex As Integer = 0 To staData.Length - 1
                        If (staData(iIndex) = " " _
                            Or staData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            staData(iIndex) = "0"
                        End If
                    Next

                    'Minu Changed
                    'For inserting the parsed data to DB
                    strInsertCmd = "INSERT INTO UODOUTSummary " _
                        & "(Summary_Type,Container_Type,Container_Quantity)" _
                        & " VALUES ('" & staData(0) & "','" _
                        & staData(1) & "'," & staData(2) & ")"

                    sqlCommd.CommandText = strInsertCmd
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
            'Check iCount against the count in "T" record.
            strLine = strLine.Substring(2).Trim()
            'Check if all the records are inserted.
            If iCount = Val(strLine) Then
                'Close the file reader.
                rdrFileReader.Close()
                'Delete the reference file used by this function.
                File.Delete(strFilePath & strFileName)
            End If
        Catch ex As FileNotFoundException
            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: File" & strFileName & "not found.", _
                                                  Logger.LogLevel.RELEASE)
            'close the reader
            rdrFileReader.Close()
            'Return false in case of error
            Return False
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: Exception occurred" & _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            'Return false in case of error
            Return False
        Finally
            rdrFileReader.Close()
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
        Return True
    End Function
    ''' v1.1 - CK
    ''' Added new function ParseWHUODFile
    ''' <summary>
    ''' To parse WHUOD.CSV and update the DB
    ''' </summary>
    ''' <param name="cFilePath"></param>
    ''' <param name="cFileName"></param>
    ''' <param name="iFileIndex"></param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function ParseWHUODFile(ByVal cFilePath As String, ByVal cFileName As String, Optional ByVal iFileIndex As Integer = 0) As Boolean
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim caData() As String
        Dim cInsertCmd As String = ""
        Dim cLine As String = Nothing
        Dim iCount As Integer = 0
        Dim cExpectedDate As String = Nothing
        Try
            Dim sqlCommd As New SqlCeCommand()
            'Assign SQL connection to the command.s
            sqlCommd.Connection = sqlConn

            'Load the file for reading.
            rdrFileReader = New System.IO.StreamReader(cFilePath + cFileName)
            'Reading a line from the CSV file
            cLine = rdrFileReader.ReadLine()
            While Not (cLine.StartsWith("T"))
                cLine = cLine.Trim()
                If cLine.StartsWith("D") Then
                    cLine = cLine.Substring(2)
                    'Parsing the line
                    caData = cLine.Split(m_Delimiter)

                    For iIndex As Integer = 0 To caData.Length - 1
                        If (caData(iIndex) = " " _
                            Or caData(iIndex) = Nothing) Then
                            'Null values are replaced with '0'
                            caData(iIndex) = "0"
                        End If
                    Next

                    ' Changing expected date extracted from WHUOD.CSV into format of 'yyyy-MM-dd' to insert into db
                    cExpectedDate = DateTime.ParseExact(caData(1), "yyMMdd", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")

                    cInsertCmd = "INSERT INTO DALUODList " _
                        & "(DalUOD_Num,DalUOD_Exp_Date,DalUOD_Status)" _
                        & " VALUES ('" & Convert.ToInt64(caData(0)) & "','" & cExpectedDate & "','" _
                        & caData(2) & "')"
                    sqlCommd.CommandText = cInsertCmd
                    'Executing the querry
                    Try
                        'Executing the querry
                        sqlCommd.ExecuteNonQuery()
                        'Counter to keep track of the record number inserted to the db.
                        iCount = iCount + 1
                    Catch ex As Exception
                        If ex.Message.Contains("duplicate") Then
                            ' Log if duplicate record found
                            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: duplicate " _
                                                                  & "record found " + cLine, _
                                                                  Logger.LogLevel.DEBUG)
                            'Increment the line count.
                            iCount = iCount + 1
                            'Read next line
                            cLine = rdrFileReader.ReadLine()
                            Continue While
                        Else
                            Throw ex
                        End If
                    End Try

                ElseIf cLine.StartsWith("H") Then
                    If Not cLine.Contains(cFileName) Then
                        ' Log if the file name in the header record is not matching
                        objAppContainer.objLogger.WriteAppLog("ActiveFileParser: Invalid file format. The " _
                                                    & "file name in the header record is not matched.", _
                                                    Logger.LogLevel.RELEASE)
                        Return False
                    End If
                End If

                ' Read next line
                cLine = rdrFileReader.ReadLine()

            End While
            cLine = cLine.Substring(2)
            If iCount = Val(cLine) Then
                'Close the file reader.
                rdrFileReader.Close()
                'Delete the reference file used by this function.
                File.Delete(cFilePath & cFileName)
            End If
        Catch ex As FileNotFoundException
            ' Log if WHUOD.CSV file is not found
            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: File" & cFileName & "not found.", _
                                                  Logger.LogLevel.RELEASE)
            'close the reader
            rdrFileReader.Close()
            'Return false in case of error
            Return False
        Catch ex As Exception
            'Catch the exception and write the stack trace.
            objAppContainer.objLogger.WriteAppLog("ActiveFileParser: Exception occurred" & _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            'Return false in case of error
            Return False
        Finally
            rdrFileReader.Close()
        End Try
        'If successfully inserted the entire list of file contents to DB
        'Return true.
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
End Class
#End If
