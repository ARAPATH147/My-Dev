Imports System
Imports System.Data
Imports System.Text.RegularExpressions
Imports System.Data.SqlServerCe
'''******************************************************************************
''' <FileName>SMNRFDataSource.vb</FileName>
''' <summary>
''' This class is the data source class for the Shelf Management application.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''******************************************************************************
Public Class UtilNRFDataSource
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub
    ''' <summary>
    ''' Gets the user details using User ID.
    ''' </summary>
    ''' <param name="strUserID">User ID</param>
    ''' <param name="objUserInfo">Object to be updated.</param>
    ''' <returns>Bool
    ''' True - If successfully updated the results.
    ''' False - If anyy error occurred while updating the results or
    ''' there is no such User Id present in the database.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetUserDetails(ByVal strUserID As String, ByRef objUserInfo As UserInfo) As Boolean
        Dim strSqlCmd As String = QueryMacro.GET_USER_DETAILS
        strSqlCmd = String.Format(strSqlCmd, strUserID)
        Dim sqlResultSet As SqlCeDataReader
        Try
            Dim objDBConnection As DBConnections = New DBConnections()
            'execute the command to get the user details.
            sqlResultSet = objDBConnection.ExecuteSQLQuery(strSqlCmd)
            'process dataset and retreive the details in it.
            If sqlResultSet.Read Then
                With objUserInfo
                    .UserID = sqlResultSet("User_ID").ToString()
                    .Password = sqlResultSet("Password").ToString()
                    .SupervisorFlag = sqlResultSet("Supervisor_Flag").ToString()
                End With
            Else
                'If error occured while reading the data reader.
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            Return False
        Finally
            'Clear the memory occupied by the variable.
            sqlResultSet = Nothing
        End Try
        'If successfully updated the details.
        Return True
    End Function

End Class