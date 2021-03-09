Imports System.Runtime.InteropServices
Imports System.Threading
Imports Microsoft.Win32
'''****************************************************************************
''' <FileName>AutoLogOff.vb</FileName>
''' <summary>
''' Implements the auto logoff functionality to enable automatic log off if the
''' devic applicaiton is idle for s configurable time period.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>05-Dec-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''****************************************************************************
Public Class AutoLogOff
    Public Event PowerStatusChanged As EventHandler
    Public Delegate Sub StopThreadCallback()
    Public Delegate Sub ChangePowerStateCallback()

    'Create an even for getting the auto logoff timer hit.
    Public Event evtUserIdle()
    Public Event evtPowerOn()
    'Private variables
    Private m_DeviceSleepTimeout As Integer = 0
    Private userIdleTime As Integer = 0
    Private m_objPowerState As PowerState
    Private m_MsgQueueHandle As IntPtr = IntPtr.Zero
    Private m_PowerNotificationHandle As IntPtr = IntPtr.Zero
    Private m_MessageLoopThread As Thread = Nothing
    'Varibale to store the status of the thread.
    Private m_Started As Boolean = False
    Private m_Stopped As Boolean = False
    Private m_Busy As Boolean = False
    Private m_Control As Control = Nothing
    ''' <summary>
    ''' Structure to receive the power notification
    ''' </summary>
    ''' <remarks></remarks>
    <StructLayout(LayoutKind.Sequential)> _
    Public Structure MsgQOptions
        Public dwSize As Integer
        Public dwFlags As Integer
        Public dwMaxMessages As Integer
        Public cbMaxMessage As Integer
        Public bReadAccess As Boolean
    End Structure
    <DllImport("coredll.dll")> _
    Private Shared Function RequestPowerNotifications(ByVal hMsgQ As IntPtr, ByVal Flags As UInteger) As IntPtr
    End Function
    <DllImport("coredll")> _
    Private Shared Function SetSystemPowerState(ByVal psState As String, ByVal StateFlags As Integer, ByVal Options As Integer) As Integer
    End Function
    <DllImport("coredll.dll")> _
    Private Shared Function StopPowerNotifications(ByVal h As IntPtr) As Boolean
    End Function
    <DllImport("coredll.dll")> _
    Private Shared Function WaitForSingleObject(ByVal hHandle As IntPtr, ByVal wait As Integer) As UInteger
    End Function
    <DllImport("coredll.dll")> _
    Private Shared Function CreateMsgQueue(ByVal name As String, ByRef options As MsgQOptions) As IntPtr
    End Function
    <DllImport("coredll.dll")> _
    Private Shared Function CloseMsgQueue(ByVal hMsgQ As IntPtr) As Boolean
    End Function
    <DllImport("coredll.dll")> _
    Private Shared Function ReadMsgQueue(ByVal hMsgQ As IntPtr, ByVal lpBuffer As Byte(), ByVal cbBufSize As UInteger, ByRef lpNumRead As UInteger, ByVal dwTimeout As Integer, ByRef pdwFlags As UInteger) As Boolean
    End Function
    <DllImport("coredll.dll")> _
    Private Shared Function NotifyWinUserSystem(ByVal uiChange As UInteger) As Boolean
    End Function
    ''' <summary>
    ''' Define the values for the power states that device returns
    ''' </summary>
    ''' <remarks></remarks>
    Const PBT_TRANSITION As UInt32 = 1
    ' broadcast specifying system power state transition
    Const PBT_RESUME As UInt32 = 2
    ' broadcast notifying a resume, specifies previous state
    Const PBT_POWERSTATUSCHANGE As UInt32 = 4
    ' power supply switched to/from AC/DC
    Const PBT_POWERINFOCHANGE As UInt32 = 8
    ' some system power status field has changed
    Const POWER_FORCE As UInt32 = 4096
    'For forcefully setting the transition.
    Const POWER_STATE_ON As UInt32 = 302055424
    ' on state
    Const POWER_STATE_OFF As UInt32 = 131072
    ' no power, full off
    Const POWER_STATE_CRITICA As UInt32 = 262144
    ' critical off
    Const POWER_STATE_BOOT As UInt32 = 524288
    ' boot state
    Const POWER_STATE_IDLE As UInt32 = 1048576
    ' idle state

    Const POWER_STATE_USERIDLE As UInt32 = 16777216
    ' idle user state

    Const POWER_STATE_SUSPEND As UInt32 = 2097152
    ' suspend state
    Const POWER_STATE_RESET As UInt32 = 8388608
    ' reset state
    Const POWER_STATE_BACKLIGHTON As UInt32 = 33554432
    ' device scree backlight on
    Const POWER_STATE_BACKLIGHTOFF As UInt32 = 268500992
    'back light off 268500992   268435456
    Const POWER_STATES_MASK As UInt32 = (POWER_STATE_USERIDLE Or POWER_STATE_BACKLIGHTON)

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'Initialise Barcode Reader
        m_Control = New Control()
    End Sub
    ''' <summary>
    ''' Initialises a thread and starts the thread execution.
    ''' </summary>
    ''' <remarks></remarks>
    Public Function Start() As Boolean
        m_Stopped = False
        Dim msgOptions As New MsgQOptions()
        Dim regKeyName As String = "System\CurrentControlSet\Control\Power\Timeouts"
        Dim regKeyName2 As String = "ControlPanel\BackLight"
        Dim regKey As RegistryKey
        Dim regKey2 As RegistryKey
        Dim autoLogOffTime As Integer = 0
        msgOptions.dwFlags = 0
        msgOptions.dwMaxMessages = 20
        msgOptions.cbMaxMessage = 50
        msgOptions.bReadAccess = True
        msgOptions.dwSize = CUInt(Marshal.SizeOf(msgOptions))

        Try
            objAppContainer.objLogger.WriteAppLog( _
                      "Starting AutoLogOff", Logger.LogLevel.INFO)
                   
            'Read user idle time from the config file.
            userIdleTime = CInt(ConfigDataMgr.GetInstance().GetParam(ConfigKey.USER_IDLE_TIMEOUT).ToString()) * 60
            autoLogOffTime = CInt(ConfigDataMgr.GetInstance().GetParam(ConfigKey.AUTO_LOGOFF_TIMEOUT).ToString()) * 60
            'set the user idle time out in the registry.
            regKey = Registry.LocalMachine.OpenSubKey(regKeyName, True)
            'Idle time when device is on battery : Value in seconds.
            regKey.SetValue("BattUserIdle", autoLogOffTime)
            'Idle time when device is on AC : Value in seconds
            regKey.SetValue("ACUserIdle", autoLogOffTime)
            'Read the suspend time out currently present in the device.
            m_DeviceSleepTimeout = userIdleTime + 60     'Value in seconds.
            regKey.SetValue("BattSuspendTimeout", autoLogOffTime + 240)
            regKey.SetValue("ACSuspendTimeout", autoLogOffTime + 240)
            'Close the registry key.
            regKey.Close()
            regKey2 = Registry.CurrentUser.OpenSubKey(regKeyName2, True)
            regKey2.SetValue("ACTimeout", autoLogOffTime)
            regKey2.SetValue("BatteryTimeout", autoLogOffTime)
            regKey2.Close()
            'notify win usersystem function with argument 
            '3 - NWUS_MAX_IDLE_TIME_CHANGED
            NotifyWinUserSystem(3)
            'Update the battery suspend timeout in config file.
            ConfigDataMgr.GetInstance.SetParam(ConfigKey.BATTERY_SUSPEND_TIMEOUT, _
                                               m_DeviceSleepTimeout.ToString())
            'Create message handle and get the value to the structure object.
            m_MsgQueueHandle = CreateMsgQueue("BionessPowerNotificationQueue", _
                                          msgOptions)
            m_PowerNotificationHandle = RequestPowerNotifications(m_MsgQueueHandle, _
                                                                  PBT_TRANSITION Or _
                                                                  POWER_STATES_MASK)
            'Create thread object.
            m_MessageLoopThread = New Thread(New ThreadStart(AddressOf CheckTransition))
            'Start the thread execution.
            m_MessageLoopThread.Start()
        Catch ex As Exception
            'Add the exception to the device log.
            objAppContainer.objLogger.WriteAppLog( _
                        "Error in starting thread for listening to PBT_TRANSITION:" & _
                        ex.Message.ToString(), _
                        Logger.LogLevel.RELEASE)
            'Return false
            Return False
        End Try
        'Return true if successfully created the thread.
        Return True
    End Function
    ''' <summary>
    ''' Function to stop the thread execution
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub [Stop]()
        Try
            If Not m_Control.InvokeRequired Then
                If m_Busy = False Then
                    'If thread exists.
                    If m_MessageLoopThread IsNot Nothing Then
                        m_Started = False
                        'Abort the thread execution.
                        m_MessageLoopThread.Abort()
                        m_MessageLoopThread.Join(5000)
                        'Clear power notification handle.
                        If m_PowerNotificationHandle <> IntPtr.Zero Then
                            StopPowerNotifications(m_PowerNotificationHandle)
                        End If
                        'Clear message structure object.
                        If m_MsgQueueHandle <> IntPtr.Zero Then
                            CloseMsgQueue(m_MsgQueueHandle)
                        End If
                    End If
                Else
                    m_Stopped = True
                End If
                'set the sleep timer back to the original value.
                'SetSleepTimeOut(1)
            Else
                m_Control.Invoke(New StopThreadCallback(AddressOf [Stop]))
            End If
        Catch ex As Exception
            'Add the exception to the device log.
            objAppContainer.objLogger.WriteAppLog("Thread is not killed or does not exists.", _
                                                  Logger.LogLevel.RELEASE)
        Finally
            m_PowerNotificationHandle = IntPtr.Zero
            m_MsgQueueHandle = IntPtr.Zero
            'Clear thread object.
            m_MessageLoopThread = Nothing
        End Try
    End Sub
    ''' <summary>
    ''' Function to keep Check on the Power State Transition
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CheckTransition()
        Dim nRead As UInteger = 0
        Dim uiFlags As UInteger = 0
        Dim uiResult As UInteger = 0
        m_Started = True
        m_Busy = False
        Try
            'Set thread priority
            Thread.CurrentThread.Priority = ThreadPriority.Highest
            While m_Started
                'Wait till next event occurs.
                uiResult = WaitForSingleObject(m_MsgQueueHandle, -1)
                m_Busy = True
                If uiResult = 0 Then
                    Dim buf As Byte() = New Byte(50) {}
                    'Read the data from the message handle.
                    ReadMsgQueue(m_MsgQueueHandle, buf, CUInt(buf.Length), nRead, -1, uiFlags)
                    Dim iTemp As Int32 = ConvertByteArray(buf, 4)
                    Dim uiFlag As UInteger = iTemp And POWER_STATES_MASK
                    objAppContainer.objLogger.WriteAppLog("Power Transition Received " + uiFlag.ToString() + " and " + iTemp.ToString(), _
                                                          Logger.LogLevel.RELEASE)
                    Select Case uiFlag
                        Case POWER_STATE_ON
                            'When user interrupt occurs or screen recevies a tap event
                            'SetSleepTimeOut(1)
                            'If m_Started Then
                            '    RaiseEvent evtPowerOn()
                            'End If
                            Exit Select
                        Case POWER_STATE_OFF
                            Exit Select
                        Case POWER_STATE_CRITICA
                            Exit Select
                        Case POWER_STATE_BOOT
                            Exit Select
                        Case POWER_STATE_IDLE
                            Exit Select
                        Case POWER_STATE_USERIDLE
                            'When the device enters idle state.
                            'SetSleepTimeOut(0)
                            If m_Started Then
                                RaiseEvent evtUserIdle()
                            End If
                            m_Stopped = True
                            Exit Select
                        Case POWER_STATE_SUSPEND
                            'MsgBox(uiFlag.ToString())
                            Exit Select
                        Case POWER_STATE_RESET
                            Exit Select
                        Case Else
                            Exit Select
                    End Select
                End If
                m_Busy = False
                If m_Stopped Then
                    [Stop]()
                End If
            End While
        Catch ex As Exception
            'Add the exception to the device log.
            objAppContainer.objLogger.WriteAppLog("Waiting for next power transition.", _
                                                  Logger.LogLevel.RELEASE)
            'Add the exception to the device log.
            objAppContainer.objLogger.WriteAppLog("Waiting for next power transition." + ex.Message, _
                                                  Logger.LogLevel.DEBUG)
        Finally
            'Clear power notification handle.
            StopPowerNotifications(m_PowerNotificationHandle)
            'Clear message structure object.
            CloseMsgQueue(m_MsgQueueHandle)
            m_PowerNotificationHandle = IntPtr.Zero
            m_MsgQueueHandle = IntPtr.Zero
            'Clear thread object.
            m_MessageLoopThread = Nothing
        End Try
    End Sub
    ''' <summary>
    ''' Function to convert he byte array to UInteger
    ''' </summary>
    ''' <param name="arrBytes">Byte Array</param>
    ''' <param name="iOffset">Offset to read the value</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ConvertByteArray(ByVal arrBytes As Byte(), ByVal iOffset As Integer) As UInteger
        Dim uiRes As UInteger = 0
        uiRes += arrBytes(iOffset)
        uiRes += arrBytes(iOffset + 1) * CUInt(256)
        uiRes += arrBytes(iOffset + 2) * CUInt(65536)
        uiRes += arrBytes(iOffset + 3) * CUInt(16777216)
        'Return the converted value.
        Return uiRes
    End Function
    ''' <summary>
    ''' To set/reset the sleep time out for the device.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetSleepTimeOut()
        Dim regKeyName1 As String = "System\CurrentControlSet\Control\Power\Timeouts"
        Dim regKeyName2 As String = "ControlPanel\BackLight"
        Dim regKey1 As RegistryKey
        Dim regKey2 As RegistryKey
        Try
            'set the user idle time out in the registry.
            regKey1 = Registry.LocalMachine.OpenSubKey(regKeyName1, True)
            'Get the device sleep time to a variable.
            'Idle time when device is on battery : Value in seconds.
            regKey1.SetValue("BattUserIdle", userIdleTime)
            'Idle time when device is on AC : Value in seconds
            regKey1.SetValue("ACUserIdle", userIdleTime)
            'revert sleep timer to default value.
            'sleep state.
            regKey1.SetValue("BattSuspendTimeout", m_DeviceSleepTimeout)
            regKey1.SetValue("ACSuspendTimeout", m_DeviceSleepTimeout)
            'Close the key
            regKey1.Close()

            regKey2 = Registry.CurrentUser.OpenSubKey(regKeyName2, True)
            'For reg key 2 to set back light timing.
            regKey2.SetValue("ACTimeout", userIdleTime)
            regKey2.SetValue("BatteryTimeout", userIdleTime)
            'Close the registry key.
            regKey2.Close()

            'notify win usersystem function with argument 3 - NWUS_MAX_IDLE_TIME_CHANGED
            NotifyWinUserSystem(3)
            ChangePowerState()
        Catch ex As Exception
            'Add the exception to the device log.
            objAppContainer.objLogger.WriteAppLog( _
                                            "Error in changing the power setting in registry:" & _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To change from current power state to a different power state.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ChangePowerState()
        Try
            'm_Stopped = True
            m_Started = False
            If Not m_Control.InvokeRequired Then
                SetSystemPowerState(Nothing, POWER_STATE_ON, POWER_FORCE)
            Else
                m_Control.Invoke(New ChangePowerStateCallback(AddressOf ChangePowerState))
            End If
        Catch ex As Exception
            'Add the exception to the device log.
            objAppContainer.objLogger.WriteAppLog( _
                                            "Error in changing the power state:" & _
                                            ex.Message.ToString(), _
                                            Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class