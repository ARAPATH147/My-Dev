Imports System.Xml
'''***************************************************************
''' <FileName>AppContainer.vb</FileName>
''' <summary>
''' This is a Singlton utility class which reads a Message xml file 
''' and returns the desired message
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
    Public Class MessageManager
        Private Shared m_Instance As MessageManager
        Private objReader As XmlDocument
        Private bFileExist As Boolean

        'Constructor
        Private Sub New()
            If Not OpenXML() Then
                MessageBox.Show("Could not open the XML file ", "Exception")
            End If
        End Sub
        ''' <summary>
        ''' Create instance of the class
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetInstance() As MessageManager
            'Create only a single instance of MessageManager class
            If m_Instance Is Nothing Then
                m_Instance = New MessageManager
            End If

            Return m_Instance

        End Function
        ''' <summary>
        ''' Reads and Loads the messageconfig xml 
        ''' </summary>
        ''' <returns>True if load succeeds or else false</returns>
        ''' <remarks></remarks>
        Private Function OpenXML() As Boolean
            'Function loads the xml into an XMLDocument object when the application is loaded
            Try
                Dim sAppPath As String
                'Get the Applicaton path
                sAppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
                'Load the XML document into a XML Document object
                objReader = New XmlDocument()

                'Check if the file is present or not
            If System.IO.File.Exists(sAppPath + ConfigDataMgr.GetInstance.GetParam(ConfigKey.MESSAGE_FILE_PATH)) Then
                objReader.Load(sAppPath + ConfigDataMgr.GetInstance.GetParam(ConfigKey.MESSAGE_FILE_PATH))
            Else
                MessageBox.Show("The XML file does not exist")
                bFileExist = False
            End If

                'Returns true if XML loaded correcly
                bFileExist = True
                Return True

            Catch ex As Exception
                'Returns false if error in loading XML
                Return False
            End Try
        End Function
        ''' <summary>
        ''' Reads XML text and returns it to the calling function
        ''' </summary>
        ''' <param name="nodeId">Node name</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMessage(ByVal nodeId As String) As String

            Try
                If bFileExist Then
                    'Identify the Root node in the xml file
                    Dim objRootElement As XmlElement
                    objRootElement = objReader.FirstChild

                    'Reading the message of the corresponding nodeId from the XML File
                    GetMessage = objRootElement.GetElementsByTagName(nodeId).Item(0).InnerText
                Else
                    GetMessage = "File does not exist"
                End If

            Catch ex As Exception
                GetMessage = "Specified node not preset in XML"
            End Try

    End Function
    ''' <summary>
    ''' Overloaded method to reads XML text and returns it to the calling function
    ''' </summary>
    ''' <param name="nodeId">Node name</param>
    ''' <param name="param">Parameter to be passed</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMessage(ByVal nodeId As String, ByVal param As String) As String
        Dim strTempMsg As String
        Try
            If bFileExist Then
                'Identify the Root node in the xml file
                Dim objRootElement As XmlElement
                objRootElement = objReader.FirstChild

                'Reading the message of the corresponding nodeId from the XML File
                strTempMsg = objRootElement.GetElementsByTagName(nodeId).Item(0).InnerText

                GetMessage = Replace(strTempMsg, "{0}", param)
            Else
                GetMessage = "File does not exist"
            End If

        Catch ex As Exception
            GetMessage = "Specified node not preset in XML"
        End Try

    End Function

        Public Sub TerminateMsgMgr()
            objReader = Nothing
            m_Instance = Nothing
        End Sub
    End Class

