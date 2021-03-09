''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - The controller IP information is stored in IPCONFG.XML
''' </Summary>
'''****************************************************************************

Imports System.Xml
Imports System.IO
Imports System.Data
Imports Microsoft.VisualBasic
Imports System.Text
Imports System.Collections.Specialized
''' <summary>
''' This class implements the xml parser for the config file. The information extracted
''' are stored in a hash table in class ConfigDetails 
''' </summary>
''' <remarks></remarks>

Public Class ConfigDataMgr
    Private Shared m_settings As NameValueCollection
    Private Shared m_settingsPath As String
    Private Shared oConfigInstance As ConfigDataMgr
    'v1.1 MCF Change
    Private Shared m_IPsettings As NameValueCollection
    Private Shared m_IPsettingsPath As String
    Shared Sub New()
        Try
            LoadAppConfig()
            LoadIPConfig()
        Catch ex As Exception
            System.Windows.Forms.MessageBox.Show("Invalid config file format or data.", "Error")
            objAppContainer.bConfigFile = False
        End Try
    End Sub
    'The shared function GetInstance will implement a check for the instantiation
    'of class configDetails to make sure that the class has only one instance
    Public Shared Function GetInstance() As ConfigDataMgr
        If oConfigInstance Is Nothing Then
            oConfigInstance = New ConfigDataMgr
        End If
        Return oConfigInstance

    End Function

    'Public Function GetParam(ByVal cKey As String) As String
    '    'xml document object
    '    Dim xd As New Xml.XmlDocument
    '    Try
    '        If System.IO.File.Exists(Macros.CONFIG_FILE_PATH) Then

    '            'load the xml file
    '            xd.Load(Macros.CONFIG_FILE_PATH)

    '            'query for a value
    '            Dim Node As Xml.XmlNode = xd.DocumentElement.SelectSingleNode( _
    '            "/configuration/appSettings/add[@key=""" & cKey & """]")

    '            'return the value or nothing if it doesn't exist
    '            If Not Node Is Nothing Then
    '                Return Node.Attributes.GetNamedItem("value").Value
    '            Else
    '                Return Nothing
    '            End If
    '        Else
    '            System.Windows.Forms.MessageBox.Show("Config File does not exist", "Error")
    '            Return Nothing
    '        End If
    '    Catch ex As Exception
    '        MessageBox.Show("Invalid Config Key")
    '        Return Nothing
    '    End Try
    'End Function
    Public Function GetParam(ByVal Param As String) As String
        Dim appString As String = ""
        Try
            appString = m_settings.[Get](Param)
            If appString Is Nothing Then
                appString = ""
            End If
        Catch ex As Exception

        End Try
        Return appString
    End Function
    Private Shared Sub LoadAppConfig()
        m_settingsPath = Macros.CONFIG_FILE_PATH
        Dim xdoc As System.Xml.XmlDocument = New XmlDocument

        xdoc.Load(m_settingsPath)
        Dim root As XmlElement = xdoc.DocumentElement
        Dim nodeList As System.Xml.XmlNodeList = root.ChildNodes.Item(0).ChildNodes

        ' Add settings to the NameValueCollection. 
        m_settings = New NameValueCollection
        For Each node As XmlNode In nodeList
            m_settings.Add(node.Attributes("key").Value, node.Attributes("value").Value)
        Next
    End Sub

    'Public Sub SetParam(ByVal cKey As String, ByVal cValue As String)
    '    'xml document object
    '    Dim xd As New Xml.XmlDocument
    '    Try
    '        If System.IO.File.Exists(Macros.CONFIG_FILE_PATH) Then
    '            'load the xml file
    '            xd.Load(Macros.CONFIG_FILE_PATH)

    '            'get the value
    '            Dim Node As Xml.XmlElement = CType(xd.DocumentElement.SelectSingleNode( _
    '            "/configuration/appSettings/add[@key=""" & _
    '            cKey & """]"), Xml.XmlElement)
    '            If Not Node Is Nothing Then
    '                'key found, set the value
    '                Node.Attributes.GetNamedItem("value").Value = cValue
    '            Else
    '                'key not found, create it
    '                Node = xd.CreateElement("add")
    '                Node.SetAttribute("key", cKey)
    '                Node.SetAttribute("value", cValue)

    '                'look for the appsettings node
    '                Dim Root As Xml.XmlNode = xd.DocumentElement.SelectSingleNode("/configuration/appSettings")

    '                'add the new child node (this key)
    '                If Not Root Is Nothing Then
    '                    Root.AppendChild(Node)
    '                Else
    '                    Try
    '                        'appsettings node didn't exist, add it before adding the new child
    '                        Root = xd.DocumentElement.SelectSingleNode("/configuration")
    '                        Root.AppendChild(xd.CreateElement("appSettings"))
    '                        Root = xd.DocumentElement.SelectSingleNode("/configuration/appSettings")
    '                        Root.AppendChild(Node)
    '                    Catch ex As Exception
    '                        'failed adding node, throw an error
    '                        Throw New Exception("Could not set value", ex)
    '                    End Try
    '                End If
    '            End If

    '            'finally, save the new version of the config file
    '            xd.Save(Macros.CONFIG_FILE_PATH)
    '        Else
    '            System.Windows.Forms.MessageBox.Show("Config File does not exist", "Error")
    '        End If
    '    Catch ex As Exception
    '        MessageBox.Show("Cannot set config key attribute")
    '    End Try
    'End Sub
    Public Sub SetParam(ByVal Param As String, ByVal value As String)
        Try
            m_settings.[Set](Param, value)
            Update()
        Catch ex As Exception
            MessageBox.Show("Cannot set config key attribute")
            ' Logger.WriteLine(("Exception in SetConfigParam with update:" & Param.ToString()) + ex.ToString())
        End Try
    End Sub

    Private Shared Sub Update()
        Dim mxmlwriter As New XmlTextWriter(m_settingsPath, System.Text.UTF8Encoding.UTF8)
        mxmlwriter.WriteStartDocument()
        mxmlwriter.WriteStartElement("configuration")
        mxmlwriter.WriteStartElement("appSettings")

        For i As Integer = 0 To m_settings.Count - 1
            mxmlwriter.WriteStartElement("add")
            mxmlwriter.WriteStartAttribute("key", String.Empty)
            mxmlwriter.WriteRaw(m_settings.GetKey(i))
            mxmlwriter.WriteEndAttribute()

            mxmlwriter.WriteStartAttribute("value", String.Empty)
            mxmlwriter.WriteValue(m_settings.[Get](i).ToString())
            mxmlwriter.WriteEndAttribute()
            mxmlwriter.WriteEndElement()
        Next

        mxmlwriter.WriteEndElement()
        mxmlwriter.WriteEndElement()

        mxmlwriter.Close()
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
                ConfigKey.ACTIVE_IPADDRESS & """]"), Xml.XmlElement)
                If Not Node Is Nothing Then
                    'key found, set the value
                    Node.Attributes.GetNamedItem("value").Value = objAppContainer.strActiveIP
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
