Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace TFTPClient
    Public Structure TransferOptions
        Private _action As TransferType

        Public Property Action() As TransferType
            Get
                Return _action
            End Get
            Set(ByVal value As TransferType)
                _action = value
            End Set
        End Property

        Private _localFilename As String

        Public Property LocalFilename() As String
            Get
                Return _localFilename
            End Get
            Set(ByVal value As String)
                _localFilename = value
            End Set
        End Property

        Private _remoteFilename As String

        Public Property RemoteFilename() As String
            Get
                Return _remoteFilename
            End Get
            Set(ByVal value As String)
                _remoteFilename = value
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

        Private _port As Integer
        Public Property Port() As Integer
            Get
                Return _port
            End Get
            Set(ByVal value As Integer)
                _port = value
            End Set
        End Property
    End Structure
End Namespace


