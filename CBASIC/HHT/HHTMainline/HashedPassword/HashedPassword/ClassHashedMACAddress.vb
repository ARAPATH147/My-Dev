
Imports System.IO
Imports System.Net.NetworkCredential


Public Class ClassHashedMACAddress

    Implements IDisposable



#Region "Disposing"

    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: free other state (managed objects).
            End If

            ' TODO: free your own state (unmanaged objects).
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
#End Region
    '****************************************************************************************
    '* This method will get the hashed pasword for any MAC Address.

    '****************************************************************************************
    ''' <summary>
    ''' Function for hashed password
    ''' </summary>
    ''' <param name="MACAddress"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateHashedPassword(ByVal MACAddress As String) As String
        Dim bTemp As Boolean = False

        Try
            'Check whether a Registry File is already Present
            'Initailising the Symbol Wireless objects


            Dim strUserName As String = Nothing
            Dim strPassword As String = Nothing

            strUserName = MACAddress.Replace(":", "")
            'Generate Password using the user name i.e., using the MAC address.
            strPassword = HashMACAddress(strUserName)

            'MessageBox.Show("Hashed Password is: " + strPassword)

            Return strPassword

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message.ToString(), "Error")
        End Try
        Return bTemp
    End Function
    ''' <summary>
    '''  
    ''' </summary>
    ''' <param name="strMACAddress"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function HashMACAddress(ByVal strMACAddress As String) As String

        Dim strHashedPassword As String = ""
        Dim strContainer1 As String = "" ' To hold Two Digit
        Dim strContainer2 As String = "" 'to Hold Two digit value
        For Each PasswordChar In strMACAddress.ToCharArray()
            If (strContainer1.Length >= 2) Then
                If (strContainer2.Length >= 2) Then
                    'Both Containers are full, so dump in the reverse order to the Hashed password

                    strContainer2 = strContainer2.Substring(0, 2)
                    strContainer1 = strContainer1.Substring(0, 1)
                    strHashedPassword += "0#" + strContainer2 + "&%" + strContainer1
                    strContainer1 = ""
                    strContainer1 = PasswordChar    'substitute the value read from array in this iteration.
                    strContainer2 = ""
                Else
                    strContainer2 += PasswordChar
                End If
            Else
                strContainer1 += PasswordChar
            End If
        Next
        If (strContainer1.Length > 0) Then
            If strContainer2.Length > 0 Then
                strHashedPassword += strContainer2
            End If
            strHashedPassword += strContainer1

        End If

        Return strHashedPassword

    End Function
End Class
