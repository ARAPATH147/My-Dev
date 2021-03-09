Imports System
Imports System.IO
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Public Class ConfigParser
    Private Shared m_objConfigParser As ConfigParser
    Private m_ConfigParams As ConfigParams
    Private m_strFilePath As String
    Private m_strFileName As String = ""
    Private m_objSerializer As XmlSerializer
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
        'checks and loads the config xml
        If m_strFileName = "" Then
            m_strFilePath = System.IO.Path.GetDirectoryName( _
            System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
            m_strFileName = m_strFilePath + "\" + "MCDownloader_Config.xml"
        End If
        'creates the serializer object
        If m_objSerializer Is Nothing Then
            m_objSerializer = New XmlSerializer(GetType(ConfigParams))
        End If
        If Not File.Exists(m_strFileName) Then
            'write code to create the defual config xml
            WriteXml()
        End If
    End Sub
    ''' <summary>
    ''' GetInstance to get the instance of singleton class
    ''' </summary>
    ''' <returns>ConfigParser</returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As ConfigParser
        If m_objConfigParser Is Nothing Then
            m_objConfigParser = New ConfigParser()
            Return m_objConfigParser
        Else
            Return m_objConfigParser
        End If
    End Function
    Public Function GetConfigParams() As ConfigParams
        If m_ConfigParams Is Nothing Then
            ReadXml()
        End If
        Return m_ConfigParams
    End Function
    Public Sub UpdateConfig()
        WriteXml()
    End Sub
    Private Sub ReadXml()
        Try
            Dim fsReader As New FileStream(m_strFileName, FileMode.Open)
            ' Call the Deserialize method to restore the object's state.
            m_ConfigParams = CType(m_objSerializer.Deserialize(fsReader), ConfigParams)
            fsReader.Close()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub WriteXml()
        Try
            Dim fsWriter As New StreamWriter(m_strFileName)
            'm_ConfigParams = New ConfigParams()
            'Add the parameters to object
            'Call the serialize method to put object to xml
            m_objSerializer.Serialize(fsWriter, m_ConfigParams)
            fsWriter.Close()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class
