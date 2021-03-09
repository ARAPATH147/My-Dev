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

Public Class RefernceFileDownload
    Public m_objTFTPSession As New TFTPClient.TFTPSession()
    Private m_tyOptions As New TFTPClient.TransferOptions()
    Private m_objConfigParams As ConfigParams
    Private m_strFileName As String
    ''' <summary>
    ''' Constructor to initialize values for file download
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal strFileName As String)
        m_strFileName = strFileName
        m_objConfigParams = ConfigParser.GetInstance().GetConfigParams()
        'm_tyOptions.Host = m_objConfigParams.TFTP.Host
        m_tyOptions.Host = AppContainer.GetInstance.strActiveIP
        m_tyOptions.Action = TFTPClient.TransferType.Get
        m_tyOptions.Port = m_objConfigParams.TFTP.Port
        If m_objConfigParams.TFTP.RemoteFilePath = "" Then
            m_tyOptions.RemoteFilename = strFileName
        Else
            m_tyOptions.RemoteFilename = m_objConfigParams.TFTP.RemoteFilePath.TrimEnd("\") + "\" + strFileName
        End If
        'file params are set
        m_tyOptions.LocalFilename = m_objConfigParams.TFTP.LocalFilePath.TrimEnd("\") + "\" + strFileName
    End Sub
    ''' <summary>
    ''' Method to start download the file
    ''' Usually runs as a seperate thered for each file download
    ''' </summary>
    ''' <remarks></remarks>
    Public Function DownloadThread() As Status
        'file downloaded now
        Try
            m_objTFTPSession.Get(m_tyOptions)
            'Update the xml file with the details of files downloaded
            BatchConfigParser.GetInstance().AddToXml(m_strFileName)
            'Return the file download status
            Return Status.Completed
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("RefernceFileDownload::DownloadThread:: Exception Occured, Message:" + ex.Message, Logger.LogLevel.ERROR)
            Return Status.Terminated
        End Try

    End Function
End Class
