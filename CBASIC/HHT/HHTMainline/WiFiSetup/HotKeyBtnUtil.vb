Imports System.Runtime.InteropServices
Imports Microsoft.Win32

Public Class HotKeyBtnUtil
    Implements IDisposable

    Private regKeyName As String = "SOFTWARE\Microsoft\Shell\Keys\40C5"

    Public Sub disableActionButton()
        Dim regKey As RegistryKey
        Dim applicationName As String = ""
        Dim noneFlag As Integer = 9
        Try
            'set the user idle time out in the registry.
            regKey = Registry.LocalMachine.OpenSubKey(regKeyName, True)
            'Setting the value for Default key
            regKey.SetValue("", applicationName)
            'Flag 9 corresponds to <None> Value
            regKey.SetValue("Flags", noneFlag)

            'Commit all the registry changes to the registry
            regKey.Close()

        Catch ex As Exception
            'Add the exception to the device log.
        End Try
    End Sub

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

End Class
