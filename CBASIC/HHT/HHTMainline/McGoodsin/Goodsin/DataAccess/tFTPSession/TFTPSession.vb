Namespace TFTPClient
    Partial Public Class TFTPSession
        ' Delegates
        Public Delegate Sub ConnectedHandler()
        Public Delegate Sub TransferringHandler(ByVal BytesTransferred As Long, ByVal BytesTotal As Long)
        Public Delegate Sub TransferFailedHandler(ByVal ErrorCode As Short, ByVal ErrorMessage As String)
        Public Delegate Sub TransferFinishedHandler()
        Public Delegate Sub DisconnectedHandler()

        ' Events
        Public Event Connected As ConnectedHandler
        Public Event Transferring As TransferringHandler
        Public Event TransferFailed As TransferFailedHandler
        Public Event TransferFinished As TransferFinishedHandler
        Public Event Disconnected As DisconnectedHandler

        ' Enumerations
        Public Enum Modes
            NETASCII = 0
            OCTET = 1
        End Enum

        Public Enum OpCodes
            RRQ = 1
            ' Read Request
            WRQ = 2
            ' Write Request
            DATA = 3
            ' Data
            ACK = 4
            ' Acknowledge
            [ERROR] = 5
            ' Error
            OACK = 6
            ' Option Acknowledge
        End Enum

        ' Properties
        Private _blockSize As Integer

        Public Property BlockSize() As Integer
            Get
                Return _blockSize
            End Get
            Set(ByVal value As Integer)
                _blockSize = value
            End Set
        End Property

        Private _timeout As Integer

        Public Property Timeout() As Integer
            Get
                Return _timeout
            End Get
            Set(ByVal value As Integer)
                _timeout = value
            End Set
        End Property

        Private _host As String

        Public Property Host() As String
            Get
                Return _host
            End Get
            Set(ByVal value As String)
                _host = value
            End Set
        End Property

        Private _mode As Modes

        Public Property Mode() As Modes
            Get
                Return _mode
            End Get
            Set(ByVal value As Modes)
                _mode = value
            End Set
        End Property

        Private _packetReader As New PacketReader()

        Private _packetBuilder As New PacketBuilder()

        ' Constructor
        Public Sub New()
            ' Default property values
            _mode = Modes.OCTET
            _blockSize = 512
            _timeout = 10
        End Sub
    End Class
End Namespace


