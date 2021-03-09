'''***************************************************************
''' <FileName>ReferenceFileDownloader.vb</FileName>
''' <summary>
''' The Main application container class which will 
''' intialise all the applciation parameters.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''***************************************************************
#If NRF Then
Public Class RefernceFileDownload
    Private m_objTFTPSession As New TFTPClient.TFTPSession()
    Private m_tyOptions As New TFTPClient.TransferOptions()
    Private m_strFileName As String
    ''' <summary>
    ''' Constructor to initialize values for file download
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal strFileName As String)
        m_strFileName = strFileName
        'm_tyOptions.Host = objAppContainer.objConfigFileParams.TFTP.Host
        m_tyOptions.Host = objAppContainer.strActiveIP
        m_tyOptions.Port = objAppContainer.objConfigFileParams.TFTP.Port
        m_tyOptions.Action = TFTPClient.TransferType.Get
        If objAppContainer.objConfigFileParams.TFTP.RemoteFilePath = "" Then
            m_tyOptions.RemoteFilename = strFileName
        Else
            m_tyOptions.RemoteFilename = objAppContainer.objConfigFileParams.TFTP.RemoteFilePath.TrimEnd("\") + "\" + strFileName
        End If
        'file params are set
        m_tyOptions.LocalFilename = objAppContainer.objConfigFileParams.TFTP.LocalFilePath.TrimEnd("\") + "\" + strFileName
    End Sub
    ''' <summary>
    ''' Method to start download the file
    ''' Usually runs as a seperate thered for each file download
    ''' </summary>
    ''' <remarks></remarks>
    Public Function DownloadFile() As Status
        'file downloaded now
        Try
            m_objTFTPSession.Get(m_tyOptions)
            'if new files are downloaded update DB with details
            BatchConfigParser.GetInstance().AddToXml(m_strFileName)
            'Return the file download status
            Return Status.Completed
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("RefernceFileDownload:DownloadThread" + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    ''' Close the failed thread's socket and writer.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function KillDownloadThread() As Boolean
        Return m_objTFTPSession.CloseSocket()
    End Function
End Class
#End If