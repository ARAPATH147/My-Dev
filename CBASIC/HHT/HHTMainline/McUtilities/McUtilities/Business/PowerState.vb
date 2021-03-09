Imports System.Runtime.InteropServices
Imports Microsoft.Win32
''' <summary>
''' Class to set the system powerstate
''' </summary>
''' <remarks></remarks>
Public Class PowerState
    Private regKeyName As String = "System\CurrentControlSet\Control\Power\Timeouts"
    Private regKeyName2 As String = "SYSTEM\CurrentControlSet\Control\Power"
    <DllImport("coredll.dll")> _
    Private Shared Function NotifyWinUserSystem(ByVal uiChange As UInteger) As Boolean
    End Function
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

    End Sub
    Public Sub UnSetSleepTimeOut()
        Dim regKey As RegistryKey
        Dim regKey1 As RegistryKey
        Dim iTimeOut As Integer = 0
        Try
            'set the user idle time out in the registry.
            regKey = Registry.LocalMachine.OpenSubKey(regKeyName, True)
            regKey1 = Registry.LocalMachine.OpenSubKey(regKeyName2, True)
            'Idle time when device is on battery
            'regKey.SetValue("BattUserIdle", userIdleTime)
            'Set 0 to sleep timer to prevent the device from entering
            'sleep state.
            regKey.SetValue("BattSuspendTimeout", iTimeOut)
            regKey.SetValue("ACSuspendTimeout", iTimeOut)
            regKey1.SetValue("ExtPowerOff", iTimeOut)
            'Close the registry key.
            regKey.Close()
            regKey1.Close()
            'notify win usersystem function with argument 
            '3 - NWUS_MAX_IDLE_TIME_CHANGED
            NotifyWinUserSystem(3)
            objAppContainer.objLogger.WriteAppLog("PowerState:: UnSetSleepTimeOut:: changing the power settings key value in registry:", _
                                                  Logger.LogLevel.RELEASE)
        Catch ex As Exception
            'Add the exception to the device log.
            objAppContainer.objLogger.WriteAppLog("PowerState:: UnSetSleepTimeOut:: Error in changing the power settings key value in registry:" & _
                                                              ex.Message.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To set/reset the sleep time out for the device.
    ''' </summary>
    ''' <param name="iTimeOut"></param>
    ''' <remarks></remarks>
    Public Sub SetSleepTimeOut(ByVal iTimeOut As Integer)
        Dim regKey As RegistryKey
        Dim regKey1 As RegistryKey
        Try
            'set the user idle time out in the registry.
            regKey = Registry.LocalMachine.OpenSubKey(regKeyName, True)
            regKey1 = Registry.LocalMachine.OpenSubKey(regKeyName2, True)
            'Idle time when device is on battery
            'regKey.SetValue("BattUserIdle", userIdleTime)
            'revert sleep timer to default value sleep state.
            regKey.SetValue("BattSuspendTimeout", iTimeOut)
            regKey.SetValue("ACSuspendTimeout", iTimeOut)
            regKey1.SetValue("ExtPowerOff", iTimeOut)
            'Close the registry key.
            regKey.Close()
            regKey1.Close()
            'notify win usersystem function with argument 3 - NWUS_MAX_IDLE_TIME_CHANGED
            NotifyWinUserSystem(3)
            objAppContainer.objLogger.WriteAppLog("PowerState:: SetSleepTimeOut:: changing the power settings key value in registry:", _
                                                  Logger.LogLevel.RELEASE)
        Catch ex As Exception
            'Add the exception to the device log.
            objAppContainer.objLogger.WriteAppLog("PowerState:: SetSleepTimeOut:: Error in changing the power settings Key in registry:" & _
                                                              ex.Message.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class
