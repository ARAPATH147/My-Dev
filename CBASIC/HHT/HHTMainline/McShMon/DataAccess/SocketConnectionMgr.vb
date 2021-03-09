Imports System.Text
#If NRF Then

Imports System.Net.Sockets
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
    Dim m_Port As String = Nothing
    Dim m_RecBufferSize As Integer = Macros.RECEIVE_BUFFER

    Dim m_TcpClient As TcpClient = Nothing
    Dim m_NetworkStream As NetworkStream = Nothing

    'Dim m_RFClient As CClientSocket = Nothing
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
        'InitialiseConnection1()
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
    'Private Sub InitialiseConnection1()

    '    Try
    '        Dim OnConnectNotifyHandler As Global.System.EventHandler = Nothing
    '        OnConnectNotifyHandler = New CClientSocket.ConnectionDelegate(AddressOf (Cliente_A_OnConnect()))

    '        m_RFClient = New CClientSocket(m_HostName, 80)

    '        AddHandler m_RFClient.OnConnect, OnConnectNotifyHandler
    '        m_RFClient.OnConnect()

    '        'AddHandler hndlReader.ReadNotify, Me.ReadNotifyHandler
    '        m_RFClient.OnConnect += New CClientSocket.ConnectionDelegate(AddressOf (Cliente_A_OnConnect(soc As Socket)))
    '        m_RFClient.OnDisconnect += New CClientSocket.ConnectionDelegate(Cliente_A_OnDisconnect)
    '        m_RFClient.OnError += New CClientSocket.ErrorDelegate(Cliente_A_OnError)
    '        m_RFClient.OnRead += New CClientSocket.ConnectionDelegate(Cliente_A_OnRead)
    '        m_RFClient.OnWrite += New CClientSocket.ConnectionDelegate(Cliente_A_OnWrite)
    '        m_RFClient.Connect()
    '    Catch ex As Exception
    '        'Add the exception to the device log.
    '        objAppContainer.objLogger.WriteAppLog(ex.Message.ToString(), Logger.LogLevel.RELEASE)
    '    End Try
    'End Sub

    'Private Sub Cliente_A_OnConnect(ByVal soc As Socket)

    'End Sub
    'Private Sub Cliente_A_OnDisconnect(ByVal soc As Socket)

    'End Sub
    'Private Sub Cliente_A_OnError(ByVal soc As Socket)

    'End Sub
    'Private Sub Cliente_A_OnRead(ByVal soc As Socket)

    'End Sub
    'Private Sub Cliente_A_OnWrite(ByVal soc As Socket)

    'End Sub
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
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
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
                m_NetworkStream.Read(bRespData, 0, 1024)
            Catch ex As Exception
                'Add the exception to the device log.
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
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
        m_HostName = objAppContainer.strActiveIP.ToString()
        m_Port = ConfigDataMgr.GetInstance().GetParam( _
                                                ConfigKey.IPPORT).ToString()
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
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
End Class

#End If