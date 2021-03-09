Imports System.Collections.Specialized
Imports System.IO

'''****************************************************************************
''' <FileName> ConfigFileManager.vb </FileName>
''' <summary>
''' Class to handle the reading of the Order and Collect CSV configuration file
''' </summary> 
''' <Version>1.0</Version> 
''' <Author>Andrew Paton</Author> 
''' <DateModified>11-05-2016</DateModified> 
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC55RF</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK </CopyRight>
'''****************************************************************************
'''* Modification Log 
'''**************************************************************************** 
'''  1.0    Andrew Paton                             11/05/2016        
'''         Inital Version.
''' 
'''**************************************************************************** 

Public Class ConfigFileManager

    Private Shared configList As New List(Of String)

    Private Shared oConfigInstance As ConfigFileManager

    ''' <summary>
    ''' Read configfile to configlist when class is initialised
    ''' </summary>
    Private Sub New()
        Try
            Using readConfigFile As New StreamReader(ConfigKey.CONFIG_FILE_PATH)
                While Not readConfigFile.EndOfStream
                    configList.Add(readConfigFile.ReadLine)
                End While
            End Using
        Catch
            MessageBox.Show("Could not open " + ConfigKey.CONFIG_FILE_PATH, "Exception")
        End Try

    End Sub

    ''' <summary>
    ''' The shared function GetInstance will implement a check for the instantiation
    ''' of class ConfigFileManager to make sure that the class has only one instance
    ''' </summary>
    Public Shared Function GetInstance() As ConfigFileManager
        If oConfigInstance Is Nothing Then
            oConfigInstance = New ConfigFileManager
        End If
        Return oConfigInstance

    End Function

    ''' <summary>
    ''' Get the value for the passed parameter from the configlist
    ''' </summary>
    ''' <param name="cParam">String containing Message Id,Message 
    '''                      read from file eg. M1,Error</param>
    ''' <returns>String</returns>

    Public Function GetParam(ByVal cParam As String) As String
        Dim appString As String = ""
        For Each record In configList
            Dim sParam As String() = record.Split(",")
            If sParam(0) = cParam Then
                appString = sParam(1)
            End If
        Next
        Return appString
    End Function

    ''' <summary>
    ''' Set the value for the passed parameter from the configuration list
    ''' </summary>
    Public Sub SetParam(ByVal cParam As String, ByVal value As String)
        For index As Integer = 0 To (configList.Count - 1)
            Dim sParam As String() = configList(index).Split(",")
            If sParam(0) = cParam Then
                configList(index) = cParam & "," & value
                index = configList.Count
            End If
        Next
        Update()
    End Sub

    ''' <summary>
    ''' Update the configuration gfile with all records in the configuration list
    ''' </summary>
    Public Shared Sub Update()
        Using oWriter As New StreamWriter(ConfigKey.CONFIG_FILE_PATH)
            For Each record In configList
                oWriter.WriteLine(record)
            Next
            oWriter.Close()
        End Using
    End Sub

    ''' <summary>
    ''' Function to get the release version from the configuration list 
    ''' </summary>
    ''' <returns>String</returns>
    Public Function GetReleaseVersion() As String
        Dim cReleaseVersion() As String = Nothing
        Dim cFinalVersion As String = "RCL v"
        Try
            cReleaseVersion = GetParam(ConfigKey.APP_VERSION).ToString().Split("-")
            cFinalVersion += CInt(cReleaseVersion(1).Substring(0, 2)) & "." _
                                    & CInt(cReleaseVersion(1).Substring(2, 2))
        Catch ex As Exception

        End Try
        Return cFinalVersion

    End Function

End Class
