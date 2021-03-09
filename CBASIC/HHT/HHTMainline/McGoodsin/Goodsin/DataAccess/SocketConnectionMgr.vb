Imports System.Text
Imports System.Net.Sockets
#If NRF Then
'''****************************************************************************
''' <FileName>SocketConnectionMgr.vb</FileName>
''' <summary>
''' Responsible for initialis and terminate connection with the TRANSACT 
''' service running on the EPOS controller. Send and receive TRANSACT messages.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''****************************************************************************
Public Class SocketConnectionMgr
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Dim m_HostName As String = Nothing
    Dim m_Port As Integer = Nothing
    Dim m_RecBufferSize As Integer = Macros.RECEIVE_BUFFER

    Public m_TcpClient As TcpClient = Nothing
    Dim m_NetworkStream As NetworkStream = Nothing

    ''' <summary>
    ''' Gets or sets the connection status
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ConnectionStatus() As Boolean
        Get
            'Return m_ConnStatus
            Return m_TcpClient.Client.Connected
        End Get
    End Property
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        GetConnectionDetails()
        InitialiseConnection()
    End Sub
    ''' <summary>
    ''' Initialise a connection to the TRANSACT.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitialiseConnection()
        m_TcpClient = New TcpClient()
        Try
            m_TcpClient.Connect(m_HostName, m_Port)
            m_NetworkStream = m_TcpClient.GetStream()
        Catch ex As Exception
            'Add the exception to the device log.
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Transmits record to the controller in byte stream.
    ''' </summary>
    ''' <param name="bExportData"></param>
    ''' <returns>Bool
    ''' True - If successfully transmitted the data to the controller.
    ''' False - If error in transmitting the data to the controller.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function TransmitData(ByVal bExportData As Byte()) As Boolean
        'Check if the network stream is capable of writing.
        If m_NetworkStream.CanWrite Then
            Try
                'm_NetworkStream.WriteTimeout = m_TimeOut
                m_NetworkStream.Write(bExportData, 0, bExportData.Length)
            Catch ex As Exception
                'Add the exception to the device log.
                objAppContainer.objLogger.WriteAppLog( _
                                                ex.Message.ToString(), _
                                                Logger.LogLevel.RELEASE)
                Return False
            End Try
            Return True
        Else
            Return False
        End If
    End Function
    ''' <summary>
    ''' Read record from the stream.
    ''' </summary>
    ''' <param name="bRespData"></param>
    ''' <returns>Bool
    ''' True - If successfully read the data from the stream.
    ''' False - If failed to read the data from the stream.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function ReadData(ByRef bRespData As Byte()) As Boolean
        'Check if the network stream is capable of writing.
        ReDim bRespData(m_TcpClient.ReceiveBufferSize)

        'Chec if network stream can read.
        If m_NetworkStream.CanRead Then
            Try
                'm_NetworkStream.ReadTimeout = m_TimeOut
                m_NetworkStream.Read(bRespData, 0, m_RecBufferSize)
            Catch ex As Exception
                'Add the exception to the device log.
                objAppContainer.objLogger.WriteAppLog( _
                                                ex.Message.ToString(), _
                                                Logger.LogLevel.RELEASE)
                Return False
            End Try
            Return True
        Else
            Return False
        End If

    End Function
    ''' <summary>
    ''' Gets the controller credentials from the config file.
    ''' </summary>
    ''' <returns>Bool
    ''' True - If successfully obtained the setting from the config file.
    ''' False - Any error occured in obtaining the config file.
    ''' </returns>
    ''' <remarks></remarks>
    Private Function GetConnectionDetails() As Boolean
        'm_HostName = ConfigDataMgr.GetInstance().GetParam( _
        '                                        ConfigKey.SERVER_IPADDRESS).ToString()
        m_HostName = objAppContainer.strActiveIP
        m_Port = CInt(ConfigDataMgr.GetInstance().GetParam( _
                                                ConfigKey.IPPORT).ToString())
    End Function
    ''' <summary>
    ''' Close the connection established with the TRANSACT.
    ''' </summary>
    ''' <returns>Bool
    ''' True - If successfully transmitted all the data.
    ''' False - Any failure in transmitting the export data.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function TerminateConnection() As Boolean
        Try
            If Not m_TcpClient Is Nothing Then m_TcpClient.Close()
            If Not m_NetworkStream Is Nothing Then m_NetworkStream.Close()
        Catch ex As Exception
            'Add the exception to the device log.
            objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
End Class
#End If