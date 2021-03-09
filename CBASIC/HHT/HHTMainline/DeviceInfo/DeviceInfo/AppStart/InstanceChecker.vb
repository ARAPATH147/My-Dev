Imports System.Runtime.InteropServices

'''****************************************************************************
''' <FileName> InstanceChecker.vb </FileName> 
''' <summary> Prevents multiple instances of the application running</summary> 
''' <Version>1.0</Version> 
''' <Author>Andrew Paton</Author> 
''' <DateModified>11-05-2016</DateModified> 
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC55RF</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK </CopyRight>
'''****************************************************************************
'''* Modification Log 
'''**************************************************************************** 
'''  1.0    Andrew Paton                             11/05/2016        
'''         Inital Version.
''' 
'''**************************************************************************** 

Public Class InstanceChecker

    Private m_hMutex As IntPtr
    Public Const NATIVE_ERROR_ALREADY_EXISTS As Int32 = 183

    <DllImport("coredll.dll", EntryPoint:="CreateMutex", SetLastError:=True)> _
    Public Shared Function fCreateMutex(ByVal lpMutexAttributes As IntPtr, ByVal InitialOwner As Boolean, ByVal MutexName As String) As IntPtr
    End Function

    <DllImport("coredll.dll", EntryPoint:="ReleaseMutex", SetLastError:=True)> _
    Public Shared Function fReleaseMutex(ByVal hMutex As IntPtr) As Boolean
    End Function

    ''' <summary>
    ''' Function to check if an instance of the application is already running 
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function fIsInstanceRunning() As Boolean
        m_hMutex = fCreateMutex(IntPtr.Zero, True, "BTStoreApps")
        If m_hMutex = IntPtr.Zero Then
            Return False
        End If
        If Marshal.GetLastWin32Error() = NATIVE_ERROR_ALREADY_EXISTS Then
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
    Public Function fClearMutex() As Boolean
        Try
            Return fReleaseMutex(m_hMutex)
        Catch ex As Exception
            m_hMutex = Nothing
        End Try
    End Function
End Class
