Imports System.Runtime.InteropServices
Public Class InstanceChecker
    Private m_hMutex As IntPtr
    Public Const NATIVE_ERROR_ALREADY_EXISTS As Int32 = 183
    ''' <summary>
    ''' Imports the CreateMutex system call
    ''' </summary>
    ''' <param name="lpMutexAttributes"></param>
    ''' <param name="InitialOwner"></param>
    ''' <param name="MutexName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DllImport("coredll.dll", EntryPoint:="CreateMutex", SetLastError:=True)> _
    Public Shared Function CreateMutex(ByVal lpMutexAttributes As IntPtr, ByVal InitialOwner As Boolean, ByVal MutexName As String) As IntPtr
    End Function
    ''' <summary>
    ''' Import ReleaseMutex system Call
    ''' </summary>
    ''' <param name="hMutex"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DllImport("coredll.dll", EntryPoint:="ReleaseMutex", SetLastError:=True)> _
    Public Shared Function ReleaseMutex(ByVal hMutex As IntPtr) As Boolean
    End Function
    ''' <summary>
    ''' Check the Mutex to find out if the application instance is already running
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsInstanceRunning() As Boolean
        m_hMutex = CreateMutex(IntPtr.Zero, True, "BTStoreApps")
        If m_hMutex = IntPtr.Zero Then
            'Throw New ApplicationException("Failure creating mutex: " + Marshal.GetLastWin32Error().ToString("X"))
            Return False
        End If
        If Marshal.GetLastWin32Error() = NATIVE_ERROR_ALREADY_EXISTS Then
            'MessageBox.Show(MessageManager.GetInstance().GetMessage("M67"), "Warning", _
            '                MessageBoxButtons.OK, _
            '                MessageBoxIcon.Exclamation, _
            '                MessageBoxDefaultButton.Button1)
            Return True
        Else
            Return False
        End If
    End Function
    ''' <summary>
    ''' Release the mutext at application exit.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ClearMutex() As Boolean
        Try
            Return ReleaseMutex(m_hMutex)
        Catch ex As Exception
            m_hMutex = Nothing
        End Try
    End Function
End Class
