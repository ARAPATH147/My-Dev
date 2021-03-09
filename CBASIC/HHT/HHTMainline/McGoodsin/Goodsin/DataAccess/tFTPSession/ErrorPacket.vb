Imports System
Imports System.Collections.Generic
Imports System.Text
Namespace TFTPClient
    Public Class ErrorPacket
        Private _code As Short

        Public Property Code() As Short
            Get
                Return _code
            End Get
            Set(ByVal value As Short)
                _code = value
            End Set
        End Property

        Private _message As String

        Public Property Message() As String
            Get
                Return _message
            End Get
            Set(ByVal value As String)
                _message = value
            End Set
        End Property

        Public Sub New(ByVal Code As Short, ByVal Message As String)
            Code = _code
            Message = _message
        End Sub
    End Class
End Namespace

