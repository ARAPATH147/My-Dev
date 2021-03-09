'***************************************************************
' <FileName>RefDownloadMgr.vb</FileName>
' <summary>
' This class manages the Reference data download
' </summary>
' <Version>1.0</Version>
' <Author>Infosys Technologies Ltd.</Author>
' <DateModified>21-Dec-2008</DateModified>
' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'***************************************************************

Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Runtime.InteropServices
''' <summary>
''' AppContainer Class to initialize and control the
''' program exection
''' </summary>
''' <remarks></remarks>
Public Class RefDownloadMgr
#If NRF Then
    Private m_tmrAlarm As System.Windows.Forms.Timer
    Private m_iAlarmCounter As Integer = 1
    Private m_bExitFlag As Boolean = False
    Private m_objPwrState As PowerState
    Private Shared m_objRefDownloadMgr As RefDownloadMgr


    Private m_bFirstInvoke As Boolean = False


    Dim m_dtFirstInvokeToday As DateTime
    Dim m_dtFirstDownloadToday As DateTime
    Dim m_dtAppExitToday As DateTime
    ''' <summary>
    ''' Contructor, retrieves information from configuration xml
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
        'get config params from xml
    End Sub
    ''' <summary>
    ''' GetInstance to get the instance of singleton class
    ''' </summary>
    ''' <returns>AppContainer</returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As RefDownloadMgr
        If m_objRefDownloadMgr Is Nothing Then
            m_objRefDownloadMgr = New RefDownloadMgr()
            Return m_objRefDownloadMgr
        Else
            Return m_objRefDownloadMgr
        End If
    End Function
    ''' <summary>
    ''' The Start Method to enter into the processing logic
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Start()


        'check if application has exited abruptly - then system is in unattened mode...
        'release it and set 
        'sets the power state to Unattended Mode to prevend system sleep till 6.00 AM
        Try
            m_objPwrState = New PowerState
            m_objPwrState.UnSetSleepTimeOut()
            'sets timer to raise events at particular intervals
            StartTimer()

            'continue waiting till the other threads finish processing
            While m_bExitFlag = False
                ' Processes all the events in the queue.
                Application.DoEvents()
            End While
        Catch ex As Exception
            'log the exception here
        Finally
            'Leave unattended mode
            m_objPwrState.SetSleepTimeOut(CInt(objAppContainer.objConfigFileParams.BattSuspendTimeout))
        End Try

    End Sub
    ''' <summary>
    ''' The timer paramers are set 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartTimer()
        ' Adds the event and the event handler for the method that will
        ' process the timer event to the timer.
        m_tmrAlarm = New System.Windows.Forms.Timer()
        AddHandler m_tmrAlarm.Tick, AddressOf TimerEventProcessor


        m_tmrAlarm.Interval = 10000




        ''Set timer based on time of invokation
        'm_tmrAlarm.Interval = objAppContainer.objConfigFileParams.DownloadInterval * 60 * 1000

        ' Runs the timer, and raises the event.
        m_tmrAlarm.Enabled = True
    End Sub

    ''' <summary>
    ''' Function to get the timedifference in milliseconds between time.now and the timepassed
    ''' </summary>
    ''' <param name="dtTime"></param>
    ''' <returns>tsDifference</returns>
    ''' <remarks></remarks>
    Private Function IntervalTill(ByVal dtTime As DateTime) As Integer
        Dim tsDifference As TimeSpan
        If DateTime.op_LessThan(dtTime, Now) Then
            dtTime = dtTime.AddDays(1)
        End If
        tsDifference = dtTime.Subtract(Now)
        Return tsDifference.TotalMilliseconds
    End Function
    ''' <summary>
    ''' Function to set the next auto Inovoke time
    ''' Returns true if the function is invoked inbetween startime and endtime for 
    ''' daily download
    ''' Returns false if invoked at any other time - assumes that the application is 
    ''' invoked to set the first autoinvoke
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function SetNextInvoke() As Boolean

        Dim bReturn As Boolean = False
        Dim tySystemTime As SystemTime
        Dim intptrTime As IntPtr

        m_dtFirstInvokeToday = New DateTime(Today.Year, Today.Month, _
                                            Today.Day, _
                                            Hour(objAppContainer.objConfigFileParams.FirstInvokeTime), _
                                            Minute(objAppContainer.objConfigFileParams.FirstInvokeTime), 0)

        m_dtFirstDownloadToday = New DateTime(Today.Year, Today.Month, _
                                            Today.Day, _
                                            Hour(objAppContainer.objConfigFileParams.FirstDownloadTime), _
                                            Minute(objAppContainer.objConfigFileParams.FirstDownloadTime), 0)

        m_dtAppExitToday = New DateTime(Today.Year, Today.Month, _
                                           Today.Day, _
                                           Hour(objAppContainer.objConfigFileParams.AppExitTime), _
                                           Minute(objAppContainer.objConfigFileParams.AppExitTime), 0)

        If DateTime.op_LessThan(m_dtFirstInvokeToday, Now) Then
            m_bFirstInvoke = True
            bReturn = True
        ElseIf DateTime.op_LessThan(m_dtAppExitToday, Now) Then
            bReturn = False
            'register application to get invoked at around night 11:45
            'convert the invoke time to Unmanages system time struct
            tySystemTime = ConvertDateTime.FromDateTime(m_dtFirstInvokeToday)
            intptrTime = Marshal.AllocHGlobal(Marshal.SizeOf(tySystemTime))
            Marshal.StructureToPtr(tySystemTime, intptrTime, False)

            'register the application to run
            CeRunAppAtTime(objAppContainer.objConfigFileParams.AppName, intptrTime)
        Else
            m_bFirstInvoke = False
            bReturn = True
        End If
        Return bReturn
    End Function
    ''' <summary>
    ''' This is the method to run when the timer is raised.
    ''' </summary>
    ''' <param name="myObject"></param>
    ''' <param name="myEventArgs"></param>
    ''' <remarks></remarks>
    Private Sub TimerEventProcessor(ByVal myObject As Object, _
                                           ByVal myEventArgs As EventArgs)
        m_tmrAlarm.Enabled = False
        RegisterAppFromTimer()
        'spown thread to handle index file download and the rest
        Dim objBatchProcessor As New BatchProcessor()
        objBatchProcessor.BatchProcess()
        m_bExitFlag = True

    End Sub

    Private Sub RegisterAppFromTimer()
        Dim dtNextInterval As DateTime
        Dim tySystemTime As SystemTime
        Dim intptrTime As IntPtr

        If DateTime.op_LessThan(Now, m_dtAppExitToday) Then
            dtNextInterval = m_dtFirstDownloadToday.AddMinutes(objAppContainer.objConfigFileParams.DownloadInterval)

            While DateTime.op_LessThan(dtNextInterval, Now)
                dtNextInterval = dtNextInterval.AddMinutes(objAppContainer.objConfigFileParams.DownloadInterval)
            End While
        Else
            If DateTime.op_LessThan(m_dtFirstInvokeToday, Now) Then
                dtNextInterval = m_dtFirstInvokeToday.AddDays(1)
            Else
                dtNextInterval = m_dtFirstInvokeToday
            End If
        End If

        'convert the invoke time to Unmanages system time struct
        tySystemTime = ConvertDateTime.FromDateTime(dtNextInterval)
        intptrTime = Marshal.AllocHGlobal(Marshal.SizeOf(tySystemTime))
        Marshal.StructureToPtr(tySystemTime, intptrTime, False)

        'register the application to run
        CeRunAppAtTime(objAppContainer.objConfigFileParams.AppName, intptrTime)
    End Sub

    ''' <summary>
    ''' Imports CeRunappTime of Coredll to register an application to run at a specific time
    ''' </summary>
    ''' <param name="AppName"></param>
    ''' <param name="ExecTime"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DllImport("CoreDLL.dll")> _
    Private Shared Function CeRunAppAtTime(ByVal AppName As String, ByVal ExecTime As IntPtr) As Boolean
    End Function
#End If
End Class
