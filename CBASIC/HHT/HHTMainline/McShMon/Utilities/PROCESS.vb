Imports System.Runtime.InteropServices
Public Class PROCESS
    Private Const TH32CS_SNAPPROCESS As Integer = &H2
    <DllImport("toolhelp.dll")> _
    Public Shared Function CreateToolhelp32Snapshot(ByVal flags As UInteger, ByVal processid As UInteger) As IntPtr
    End Function
    <DllImport("toolhelp.dll")> _
Public Shared Function CloseToolhelp32Snapshot(ByVal handle As IntPtr) As Integer
    End Function
    <DllImport("toolhelp.dll")> _
    Public Shared Function Process32First(ByVal handle As IntPtr, ByVal pe As Byte()) As Integer
    End Function
    <DllImport("toolhelp.dll")> _
    Public Shared Function Process32Next(ByVal handle As IntPtr, ByVal pe As Byte()) As Integer
    End Function
    <DllImport("coredll.dll")> _
    Private Shared Function OpenProcess(ByVal flags As Integer, ByVal fInherit As Boolean, ByVal PID As Integer) As IntPtr
    End Function
    Private Const PROCESS_TERMINATE As Integer = 1
    <DllImport("coredll.dll")> _
    Private Shared Function TerminateProcess(ByVal hProcess As IntPtr, ByVal ExitCode As UInteger) As Boolean
    End Function
    <DllImport("coredll.dll")> _
    Private Shared Function CloseHandle(ByVal handle As IntPtr) As Boolean
    End Function
    Private Const INVALID_HANDLE_VALUE As Integer = -1


    Private processName As String
    Private handle As IntPtr
    Private threadCount As Integer
    Private baseAddress As Long


    Public Sub New()
        ''Nothing in Default Constructor
    End Sub

    Public Sub New(ByVal id As IntPtr, ByVal procname As String, ByVal threadcount As Integer, ByVal baseaddress As Long)
        handle = id
        processName = procname
        threadcount = threadcount
        baseaddress = baseaddress
    End Sub

    Public Function GetProcess() As ArrayList
        Dim ProcList As New ArrayList()
        handle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0)
        If CInt(handle) > 32 Then
            Try
                Dim peCurrent As PROCESSENTRY32
                Dim pe32 As New PROCESSENTRY32()
                Dim peBytes() As Byte = pe32.ToByteArray()
                Dim retval As Integer = Process32First(handle, peBytes)
                While Not (retval <> 1)
                    peCurrent = New PROCESSENTRY32(peBytes)
                    Dim proc As PROCESS = New PROCESS(New IntPtr(CLng(peCurrent.PID)), _
                                  peCurrent.Name.ToString(), CInt(peCurrent.ThreadCount), _
                                  CLng(peCurrent.BaseAddress))
                    ProcList.Add(proc)
                    retval = Process32Next(handle, peBytes)
                End While
                CloseToolhelp32Snapshot(handle)
                Return ProcList
            Catch ex As Exception
                Return ProcList
            End Try
        Else
            Return Nothing
        End If
    End Function
    Public Overrides Function ToString() As String
        Return processName
    End Function
    Public Function GetProcessName() As String
        Return ProcessName
    End Function
End Class
