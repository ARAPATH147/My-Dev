Imports System.Text

Namespace TFTPClient
    Friend Class PacketBuilder
        Public Function Request(ByVal OpCode As TFTPSession.OpCodes, ByVal RemoteFileName As String, ByVal Mode As TFTPSession.Modes, ByVal iBlockSize As Integer, ByVal TransferSize As Long, ByVal Timeout As Integer) As Byte()
            ' Request packet structure
            ' -----------------------------------------------------------------------------
            ' |OpCode|FileName|0|Mode|0|BlkSize|0|BSVal|0|TSize|0|TSVal|0|Timeout|0|TVal|0|
            ' -----------------------------------------------------------------------------
            Dim len As Integer

            Dim packetStr As String = ""
            Dim strMode As String = Mode.ToString().ToLower()
            Dim blockSize As String = iBlockSize.ToString()
            Dim nullChar As String = "" & Chr(0) & ""

            Dim packet As Byte()

            ' Create packet as a string
            Select Case OpCode
                Case TFTPSession.OpCodes.RRQ
                    'modifited ---------
                    packetStr = nullChar + DirectCast(ChrW(1), Char)
                    Exit Select
                Case TFTPSession.OpCodes.WRQ
                    'modifited ---------
                    packetStr = nullChar + CType(ChrW(2), Char)
                    Exit Select
            End Select

            packetStr += RemoteFileName + nullChar + strMode + nullChar + "blksize" + nullChar + blockSize.ToString() + nullChar + "tsize" + nullChar + TransferSize.ToString() + nullChar + "timeout" + nullChar + Timeout.ToString() + nullChar

            len = packetStr.Length
            'modified -------------------
            packet = New Byte(len - 1) {}

            ' Encode packet as ASCII bytes
            packet = System.Text.Encoding.ASCII.GetBytes(packetStr)
            Return packet
        End Function

        Public Function Ack(ByVal Block1 As Integer, ByVal Block2 As Integer) As Byte()
            ' ACK packet structure
            ' ----------
            ' |04|Block|
            ' ----------
            'modified--------------
            'Dim packet As Byte() = New Byte(4) {}
            Dim packet As Byte() = New Byte(3) {}
            packet(0) = 0
            packet(1) = CType(TFTPSession.OpCodes.ACK, Byte)
            packet(2) = CType(Block1, Byte)
            packet(3) = CType(Block2, Byte)
            Return packet
        End Function

        Public Function Data(ByVal SendData As Byte(), ByVal Block1 As Integer, ByVal Block2 As Integer) As Byte()
            ' DATA packet structure
            ' ----------
            ' |03|Block|
            ' ----------
            '---modified
            'Dim packet As Byte() = New Byte(SendData.Length + 4) {}
            Dim packet As Byte() = New Byte(SendData.Length + 3) {}
            'packet[0] = 0;
            packet(1) = CType(TFTPSession.OpCodes.DATA, Byte)
            packet(2) = CType(Block1, Byte)
            packet(3) = CType(Block2, Byte)
            Dim h As Integer = 4
            While h < SendData.Length + 4
                packet(h) = SendData(h - 4)
                h += 1
            End While
            Return packet
        End Function

        Public Function IncrementBock(ByVal ReceivedData As Byte(), ByVal Block As Integer()) As Integer()
            If ReceivedData(3) = 255 Then
                If ReceivedData(2) < 255 Then
                    Block(0) = CType(ReceivedData(2), Integer) + 1
                    Block(1) = 0
                Else
                    Block(0) = 0
                    Block(1) = 0
                End If
            Else
                Block(1) = CType(ReceivedData(3), Integer) + 1
            End If
            Return Block
        End Function
    End Class
End Namespace


