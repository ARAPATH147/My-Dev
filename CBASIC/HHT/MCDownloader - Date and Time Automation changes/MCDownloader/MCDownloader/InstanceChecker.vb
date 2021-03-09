Imports System.Runtime.InteropServices
Public Class InstanceChecker
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
    ''' Check the Mutex to findout if the application instance is already running
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function IsInstanceRunning() As Boolean
        Dim hMutex As IntPtr = CreateMutex(IntPtr.Zero, True, "MCD")
        If hMutex = IntPtr.Zero Then
            AppContainer.GetInstance.obLogger.WriteAppLog("InstanceChecker:: IsInstanceRunning:: CreateMutex failed - Exiting Application", Logger.LogLevel.RELEASE)
            Throw New ApplicationException("Failure creating mutex: " + Marshal.GetLastWin32Error().ToString("X"))
        End If
        If Marshal.GetLastWin32Error() = NATIVE_ERROR_ALREADY_EXISTS Then
            'MsgBox("Another instance active")
            Return True
        Else
            Return False
        End If
    End Function
End Class
