Public Class PROCESSENTRY32


    ''Constants for structure Defenition
    Private Const SizeOffset As Integer = 0
    Private Const UsageOffset As Integer = 4
    Private Const ProcessIDOffset As Integer = 8
    Private Const DefaultHeapIDOffset As Integer = 12
    Private Const ModuleIDOffset As Integer = 16
    Private Const ThreadsOffset As Integer = 20
    Private Const ParentProcessIDOffset As Integer = 24
    Private Const PriClassBaseOffset As Integer = 28
    Private Const dwFlagsOffset As Integer = 32
    Private Const ExeFileOffset As Integer = 36
    Private Const MemoryBaseOffset As Integer = 556
    Private Const AccessKeyOffset As Integer = 560
    Private Const Size As Integer = 564
    Private Const MAX_PATH As Integer = 260

    'Defining Data Types
    Public dwSize As UInt32
    Public cntUsage As UInt32
    Public th32ProcessID As UInt32
    Public th32DefaultHeapID As UInt32
    Public th32ModuleID As UInt32
    Public cntThreads As UInt32
    Public th32ParentProcessID As UInt32
    Public pcPriClassBase As UInt32
    Public dwFlags As UInt32
    Public szExeFile As String
    Public th32MemoryBase As UInt32
    Public th32AccessKey As UInt32

    Public Sub New()

    End Sub
    ''' <summary>
    ''' Initialises new instance of the class
    ''' </summary>
    ''' <param name="aData"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal aData() As Byte)
        Me.dwSize = GetUInt(aData, SizeOffset)
        Me.cntUsage = GetUInt(aData, UsageOffset)
        Me.th32ProcessID = GetUInt(aData, ProcessIDOffset)
        Me.th32DefaultHeapID = GetUInt(aData, DefaultHeapIDOffset)
        Me.th32ModuleID = GetUInt(aData, ModuleIDOffset)
        Me.cntThreads = GetUInt(aData, ThreadsOffset)
        Me.th32ParentProcessID = GetUInt(aData, ParentProcessIDOffset)
        Me.pcPriClassBase = CLng(GetUInt(aData, PriClassBaseOffset))
        Me.dwFlags = GetUInt(aData, dwFlagsOffset)
        Me.szExeFile = GetString(aData, ExeFileOffset, MAX_PATH)
        Me.th32MemoryBase = GetUInt(aData, MemoryBaseOffset)
        Me.th32AccessKey = GetUInt(aData, AccessKeyOffset)
    End Sub
    Public Function ToByteArray() As Byte()
        Dim aData(Size) As Byte
        SetUInt(aData, SizeOffset, Size)
        Return aData
    End Function
    Public Function GetUInt(ByVal aData() As Byte, ByVal Offset As Integer) As UInt16
        Return BitConverter.ToUInt16(aData, Offset)
    End Function

    Public Sub SetUInt(ByVal aData() As Byte, ByVal Offset As Integer, ByVal Value As Integer)
        Dim buint() As Byte = BitConverter.GetBytes(Value)
        Buffer.BlockCopy(buint, 0, aData, Offset, buint.Length)
    End Sub
    Public Function GetUShort(ByVal aData() As Byte, ByVal Offset As Integer) As UShort
        Return BitConverter.ToUInt16(aData, Offset)
    End Function
    Public Sub SetUShort(ByVal aData() As Byte, ByVal Offset As Integer, ByVal Value As Integer)
        Dim bushort() As Byte = BitConverter.GetBytes(CShort(Value))
        Buffer.BlockCopy(bushort, 0, aData, Offset, bushort.Length)
    End Sub
    Public Function GetString(ByVal aData() As Byte, ByVal Offset As Integer, ByVal Length As Integer) As String
        Dim strReturn As String = System.Text.Encoding.Unicode.GetString(aData, Offset, Length)
        Return strReturn
    End Function
    Public Sub SetString(ByVal aData() As Byte, ByVal Offset As Integer, ByVal Value As String)
        Dim arr() As Byte = System.Text.Encoding.ASCII.GetBytes(Value)
        Buffer.BlockCopy(arr, 0, aData, Offset, arr.Length)
    End Sub
    Public ReadOnly Property Name() As String
        Get
            Return szExeFile.Substring(0, szExeFile.IndexOf(vbNullChar))
        End Get
    End Property
    Public ReadOnly Property PID() As UInt32
        Get
            Return th32ProcessID
        End Get
    End Property
    Public ReadOnly Property BaseAddress() As Long
        Get
            Return th32MemoryBase
        End Get
    End Property
    Public ReadOnly Property ThreadCount() As ULong
        Get
            Return cntThreads
        End Get
    End Property
End Class
