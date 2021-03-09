Imports System.Collections.Specialized
Imports System.IO

'''****************************************************************************
''' <FileName> ConfigFileManager.vb </FileName>
''' <summary>
''' Class to handle the reading of the CSV configuration file and the IPCONFIG.XML fle
''' </summary> 
''' <Version>1.0</Version> 
''' <Author>Andrew Paton</Author> 
''' <DateModified>11-05-2016</DateModified> 
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC55RF</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK </CopyRight>
'''****************************************************************************
'''* Modification Log 
'''**************************************************************************** 
'''  1.0    Andrew Paton                             20/10/2016        
'''         Inital Version.
''' 

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
            'Not needed as background application
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
    ''' Get the value for the passed parameter from the IPCONFIG xml file
    ''' </summary>
    ''' <param name="cKey">String containing perameter Id</param>
    ''' <returns>String containing an IP address</returns>
    Public Function GetIPParam(ByVal cKey As String) As String
        'xml document object
        Dim xd As New Xml.XmlDocument
        Dim value As String = Nothing
        Try
            If System.IO.File.Exists(ConfigKey.IPCONFIG_FILE_PATH) Then

                'load the xml file
                xd.Load(ConfigKey.IPCONFIG_FILE_PATH)

                'query for a value
                Dim Node As Xml.XmlNode = xd.DocumentElement.SelectSingleNode( _
                "/Configuration/IPDetails/add[@key=""" & cKey & """]")

                'return the value or nothing if it doesn't exist
                If Not Node Is Nothing Then
                    value = Node.Attributes.GetNamedItem("value").Value
                End If
            End If
            Return value
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

End Class
