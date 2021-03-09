Imports System
Imports System.IO
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.Xml
Imports System.Collections.Specialized
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Loads controller IP address from IPCONFG.XML file
''' </Summary>
'''****************************************************************************
Public Class ConfigParser
    Private Shared m_objConfigParser As ConfigParser
    Private m_ConfigParams As ConfigParams
    Private m_strFilePath As String
    Private m_strFileName As String = ""
    Private m_objSerializer As XmlSerializer
    Private Shared m_settingsPath As String
    Private Shared m_settings As NameValueCollection
    'v1.1 MCF Change
    Private Shared m_IPsettings As NameValueCollection
    Private Shared m_IPsettingsPath As String
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
        'checks and loads the config xml
        If m_strFileName = "" Then
            m_strFilePath = System.IO.Path.GetDirectoryName( _
            System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
            m_strFileName = m_strFilePath + "\" + "McDownloader_Config.xml"
        End If
        'creates the serializer object
        If m_objSerializer Is Nothing Then
            m_objSerializer = New XmlSerializer(GetType(ConfigParams))
        End If
        LoadIPConfig()
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
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ReadXml()
        Try
            Dim fsReader As New FileStream(m_strFileName, FileMode.Open)
            ' Call the Deserialize method to restore the object's state.
            m_ConfigParams = CType(m_objSerializer.Deserialize(fsReader), ConfigParams)
            fsReader.Close()
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("ConfigParser::ReadXML:: Exception Occured, Message: " & ex.Message, Logger.LogLevel.ERROR)
        End Try
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub WriteXml()
        Try
            Dim fsWriter As New StreamWriter(m_strFileName)
            'm_ConfigParams = New ConfigParams()
            'Add the parameters to object
            ' Call the serialize method to put object to xml
            m_objSerializer.Serialize(fsWriter, m_ConfigParams)
            fsWriter.Close()
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("ConfigParser::WriteXML, exceptionOccured:" & ex.Message, Logger.LogLevel.ERROR)
        End Try
    End Sub
    ''' <summary>
    ''' This function implements the xml parser for the IP config file.
    ''' </summary>
    Private Shared Sub LoadIPConfig()

        m_IPsettingsPath = Macros.IPCONFIG_FILE_PATH

        Dim xdoc As System.Xml.XmlDocument = New XmlDocument

        xdoc.Load(m_IPsettingsPath)

        Dim root As XmlElement = xdoc.DocumentElement
        Dim nodeList As System.Xml.XmlNodeList = root.ChildNodes.Item(0).ChildNodes

        ' Add settings to the NameValueCollection. 
        m_IPsettings = New NameValueCollection
        For Each node As XmlNode In nodeList
            m_IPsettings.Add(node.Attributes("key").Value, node.Attributes("value").Value)
        Next
    End Sub
    ''' <summary>
    ''' Get the IP details from the IPCONFIG.XML file
    ''' </summary>
    ''' <param name="cKey">
    ''' key to be searched in the xml file
    ''' </param>
    Public Function GetIPParam(ByVal cKey As String) As String
        'xml document object
        Dim xd As New Xml.XmlDocument
        Dim value As String = Nothing
        Try
            If System.IO.File.Exists(Macros.IPCONFIG_FILE_PATH) Then

                'load the xml file
                xd.Load(Macros.IPCONFIG_FILE_PATH)

                'query for a value
                Dim Node As Xml.XmlNode = xd.DocumentElement.SelectSingleNode( _
                "/Configuration/IPDetails/add[@key=""" & cKey & """]")

                'return the value or nothing if it doesn't exist
                If Not Node Is Nothing Then
                    value = Node.Attributes.GetNamedItem("value").Value
                End If
            Else
                System.Windows.Forms.MessageBox.Show("IP Config File does not exist", "Error")
            End If
            Return value
        Catch ex As Exception
            MessageBox.Show("Invalid Config Key for IP Address")
            Return Nothing
        End Try
    End Function
    ''' <summary>
    ''' Set the activeIP key value
    ''' </summary>
    Public Sub SetActiveIP()
        'xml document object
        Dim xd As New Xml.XmlDocument
        Try
            If System.IO.File.Exists(Macros.IPCONFIG_FILE_PATH) Then
                'load the xml file
                xd.Load(Macros.IPCONFIG_FILE_PATH)
                'get the value
                Dim Node As Xml.XmlElement = CType(xd.DocumentElement.SelectSingleNode( _
                "/Configuration/IPDetails/add[@key=""" & _
                "activeIP" & """]"), Xml.XmlElement)
                If Not Node Is Nothing Then
                    'key found, set the value
                    Node.Attributes.GetNamedItem("value").Value = AppContainer.GetInstance.strActiveIP
                Else
                    'key not found, create it
                    MessageBox.Show("Could not set value")
                End If
                'finally, save the new version of the config file
                xd.Save(Macros.IPCONFIG_FILE_PATH)
            Else
                MessageBox.Show("Config File does not exist", "Error")
            End If
        Catch ex As Exception
            MessageBox.Show("Cannot set Active IP in xml")
        End Try
    End Sub
End Class
