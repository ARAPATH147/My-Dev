#If NRF Then
Imports System
Imports System.Data
Imports System.IO
Imports System.Data.SqlServerCe
'''****************************************************************************
''' <FileName>DBConnections.vb</FileName>
''' <summary>
''' This class is responsible for creating connection to the local database,
''' execute the query and return the results to the calling fucntion.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''****************************************************************************
Public Class DBConnections
    Dim strConnString As String
    Dim strDBFile As String = Nothing
    Dim sqlConn As SqlCeConnection = Nothing
    'Dim sqlReader As SqlCeDataReader
    ''' <summary>
    ''' Constructor responsible for retreiving the connection string from the 
    ''' configuration file.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'Get the connection string while creating the object.
        GetConnectionString()
        Me.Initialise()
    End Sub
    ''' <summary>
    ''' Method to open connection to the device database.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Initialise()
        Try
            If strDBFile <> Nothing And File.Exists(strDBFile) Then
                sqlConn = New SqlCeConnection(strConnString)
                sqlConn.Open()
                'Add the exception to the application log.
                'objAppContainer.objLogger.WriteAppLog( _
                '                                "Opened connection to database", _
                '                                Logger.LogLevel.RELEASE)
            Else
                'Throw error message box.
                MessageBox.Show("Database file not found", "Warning", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, _
                                MessageBoxDefaultButton.Button1)
                'Add the exception to the application log.
                'objAppContainer.objLogger.WriteAppLog( _
                '                            "DB file does not exists or path " _
                '                            & "not available in config file", _
                '                            Logger.LogLevel.RELEASE)
                'Exit the application.
                objAppContainer.AppTerminate()
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            'objAppContainer.objLogger.WriteAppLog( _
            '                                ex.Message.ToString(), _
            '                                Logger.LogLevel.RELEASE)
        End Try
    End Sub
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
                sqlConn.Close()
                Return True
            Catch ex As SqlCeException
                'Add the exception to the application log.
                'objAppContainer.objLogger.WriteAppLog( _
                '                                ex.Message.ToString(), _
                '                                Logger.LogLevel.RELEASE)
                Return False
            End Try
        Else
            Return False
        End If
    End Function
    ''' <summary>
    ''' To execute the query passed to the function.
    ''' </summary>
    ''' <param name="sqlQuery"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExecuteSQLQuery(ByVal sqlQuery As String) As SqlCeDataReader
        'Dim cmdSQLQuery As New SqlCeCommand(sqlCommand, sqlConnection)
        Dim sqlRdr As SqlCeDataReader
        Dim sqlCmd As SqlCeCommand = Nothing
        Try
            sqlCmd = New SqlCeCommand(sqlQuery, sqlConn)
            sqlRdr = sqlCmd.ExecuteReader()
            Return sqlRdr
        Catch ex As SqlCeException
            'Add the exception to the application log.
            objAppContainer.objLogger.WriteAppLog( _
                                            sqlQuery, _
                                            Logger.LogLevel.RELEASE)
            objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            Return Nothing
        Finally
            sqlCmd = Nothing
        End Try
    End Function
    ''' <summary>
    ''' To get the connection string present in the configuration file.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetConnectionString()
        strDBFile = ConfigDataMgr.GetInstance() _
                        .GetParam(ConfigKey.CONN_STRING).ToString()
        strConnString = "Data Source = " & strDBFile
    End Sub
End Class
#End If