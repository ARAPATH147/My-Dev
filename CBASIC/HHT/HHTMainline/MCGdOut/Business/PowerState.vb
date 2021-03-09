Imports System.Runtime.InteropServices
''' <summary>
''' Class to set the system powerstate
''' </summary>
''' <remarks></remarks>
Public Class PowerState
    Private Const SETUNATTENED As Integer = 1
    Private Const LEAVEUNATTENED As Integer = 0
    <DllImport("coredll.dll", CharSet:=CharSet.Unicode)> _
        Private Shared Function PowerPolicyNotify(ByVal dwMessage As Integer, ByVal dwData As Integer) As Boolean
    End Function
    ''' <summary>
    ''' Enum value to hold system power state values
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum PPN_Message
        PPN_REEVALUATESTATE = 1
        PPN_POWERCHANGE = 2
        PPN_SUSPENDKEYPRESSED = 4
        PPN_SUSPENDKEYRELEASED = 5
        PPN_APPBUTTONPRESSED = 6
        PPN_UNATTENDEDMODE = 3
        PPN_OEMBASE = 65536
    End Enum
    ''' <summary>
    ''' Method to set the undattended mode
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetUnAttendedMode()
        'PowerPolicyNotify(PPN_Message.PPN_UNATTENDEDMODE, 1)
        PowerPolicyNotify(PPN_Message.PPN_UNATTENDEDMODE, SETUNATTENED)
    End Sub
    ''' <summary>
    ''' Method to leave unattended Mode
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LeaveUnAttenedMode()
        'PowerPolicyNotify(PPN_Message.PPN_UNATTENDEDMODE, 0)
        PowerPolicyNotify(PPN_Message.PPN_UNATTENDEDMODE, LEAVEUNATTENED)
    End Sub
End Class
