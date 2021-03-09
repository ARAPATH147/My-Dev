Imports System.Runtime.InteropServices
'''***************************************************************
''' <FileName>SystemTime.vb</FileName>
''' <summary>
''' SystemTime structure to hold the system time as its represented
''' by sytem OS
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''***************************************************************
<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
Public Structure SystemTime
    Public wYear As Int16
    Public wMonth As Int16
    Public wDayOfWeek As Int16
    Public wDay As Int16
    Public wHour As Int16
    Public wMinute As Int16
    Public wSecond As Int16
    Public wMilliseconds As Int16
End Structure
''' <summary>
''' Class ConvertDateTime helping to convert the system date time to
''' a sequential SystemTime structure.
''' </summary>
''' <remarks></remarks>
Public Class ConvertDateTime
    Public Shared Function FromDateTime(ByVal dtTargetTime As DateTime) As SystemTime
        Dim lTargetTimeAsFileTimeUTC As Long = dtTargetTime.ToFileTime()

        Dim lTargetTimeAsFileTimeLocal As Long = 0
        FileTimeToLocalFileTime(lTargetTimeAsFileTimeUTC, lTargetTimeAsFileTimeLocal)

        Dim tyTargetTimeAsSystemTime As New SystemTime

        FileTimeToSystemTime(lTargetTimeAsFileTimeLocal, tyTargetTimeAsSystemTime)

        Return tyTargetTimeAsSystemTime
    End Function

#Region "FileTime Imports"
    <DllImport("CoreDLL.dll")> _
    Private Shared Function FileTimeToSystemTime(ByRef lpFileTime As Long, ByRef lpSystemTime As SystemTime) As Integer
    End Function
    <DllImport("CoreDLL.dll")> _
    Private Shared Function FileTimeToLocalFileTime(ByRef lpFileTime As Long, ByRef lpLocalFileTime As Long) As Integer
    End Function
#End Region
End Class