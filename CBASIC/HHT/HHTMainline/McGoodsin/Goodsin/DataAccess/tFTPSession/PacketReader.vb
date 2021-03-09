Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace TFTPClient
    Public Class PacketReader
        Public Function ReadOpCode(ByVal ReceivedData As Byte()) As TFTPSession.OpCodes
            Return CType(ReceivedData(1), TFTPSession.OpCodes)
        End Function

        Public Function ReadTransferSize(ByVal ReceivedData As Byte()) As Integer
            Dim h As Integer, tSize As Integer = 0
            'string searchStr, decPacket = Encoding.ASCII.GetString(ReceivedData);
            Dim searchStr As String, decPacket As String = Encoding.ASCII.GetString(ReceivedData, 0, ReceivedData.Length - 1)
            Dim splitChar As Char() = {Chr(0)}
            Dim splitPacket As String() = decPacket.Split(splitChar)

            h = 0
            While h < splitPacket.Length - 1
                searchStr = splitPacket(h).ToLower()
                If searchStr = "tsize" Then
                    tSize = Integer.Parse(splitPacket(h + 1))
                End If
                System.Math.Max(System.Threading.Interlocked.Increment(h), h - 1)
            End While
            Return tSize
        End Function

        Public Function ReadError(ByVal ReceivedData As Byte()) As ErrorPacket
            Dim codeStr As String = ReceivedData(2).ToString() + ReceivedData(3).ToString()

            Dim code As Short = Short.Parse(codeStr)
            Dim message As String = ""

            Dim h As Integer = 4
            While h < ReceivedData.Length
                If ReceivedData(h) = 0 Then
                    Exit While
                End If

                'message += CType(ReceivedData(h), Char)
                message += Chr(ReceivedData(h))
                h += 1
            End While

            Return New ErrorPacket(code, message)
        End Function

        Public Function CompareBlocks(ByVal SentData As Byte(), ByVal ReceivedData As Byte()) As Boolean
            If ReceivedData(2) = SentData(2) AndAlso ReceivedData(3) = SentData(3) Then
                Return True
            End If

            Return False
        End Function
    End Class
End Namespace

