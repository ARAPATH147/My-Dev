Imports System.Reflection
Imports System.Runtime.InteropServices
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Check added to verify whether MCF is enabled or not
''' </Summary>
'''****************************************************************************
Public Class frmConnector
    Public Delegate Sub SetCurrentStatusCallback(ByVal strTextToSet As String)
    Public Delegate Sub showConnectorCallBack()
    Public Delegate Sub hideConnectorCallBack()
    Private m_TimeoutRetry As Boolean = False
    Private m_TimeoutCancel As Boolean = False
    'v1.1 MCF Change
    Private m_ConnectAlternate As Integer = 0
    <DllImport("coredll.dll")> _
    Private Shared Function PlaySound(ByVal szSound As String, ByVal hModule As IntPtr, ByVal flags As Integer) As Integer
    End Function

    Private m_Cancelled As Boolean = False
    Public Property cancelled() As Boolean
        Get
            Return m_Cancelled
        End Get
        Set(ByVal value As Boolean)
            m_Cancelled = value
        End Set
    End Property
    ''' <summary>
    ''' Property to hold timeout cancel click select
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TimeoutCancel() As String
        Get
            Return m_TimeoutCancel
        End Get
        Set(ByVal value As String)
            m_TimeoutCancel = value
        End Set
    End Property
    ''' <summary>
    ''' Property to hold timeout retry click select
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TimeoutRetry() As Boolean
        Get
            Return m_TimeoutRetry
        End Get
        Set(ByVal value As Boolean)
            m_TimeoutRetry = value
        End Set
    End Property
    ''' <summary>
    ''' Property to hold alternate controller connect
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ConnectToAlternate() As Integer
        Get
            Return m_ConnectAlternate
        End Get
        Set(ByVal value As Integer)
            m_ConnectAlternate = value
        End Set
    End Property
    Public Sub HideConnector()
        Try
            If Me.InvokeRequired Then
                Me.BeginInvoke(New hideConnectorCallBack(AddressOf HideConnector))
            Else
                Me.Visible = False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Public Sub ShowConnector()
        Try
            If Me.InvokeRequired Then
                Me.BeginInvoke(New showConnectorCallBack(AddressOf ShowConnector))
            Else
                Me.Visible = True
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Public Enum PlaySoundFlags As Integer
        SND_SYNC = &H0
        ' play synchronously (default) 
        SND_ASYNC = &H1
        ' play asynchronously 
        SND_NODEFAULT = &H2
        ' silence (!default) if sound not found 
        SND_MEMORY = &H4
        ' pszSound points to a memory file 
        SND_LOOP = &H8
        ' loop the sound until next sndPlaySound 
        SND_NOSTOP = &H10
        ' don't stop any currently playing sound 
        SND_NOWAIT = &H2000
        ' don't wait if the driver is busy 
        SND_ALIAS = &H10000
        ' name is a registry alias 
        SND_ALIAS_ID = &H110000
        ' alias is a predefined ID 
        SND_FILENAME = &H20000
        ' name is file name 
        SND_RESOURCE = &H40004
        ' name is resource name or atom 
    End Enum
    Public Sub setStatus(ByVal strStatus As String)
        Me.lblMessage.Text = strStatus
        Me.lblMessage.Invalidate()
        Me.lblMessage.Refresh()
        Me.Refresh()
    End Sub
    ''' <summary>
    ''' new - initializes the data pool object
    ''' </summary>
    ''' <remarks>Initialises the data pool</remarks>
    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Me.Close()
        '  Me.Hide()
    End Sub
    Public Shared Sub Play(ByVal fileName As String)
        Try
            PlaySound(fileName, IntPtr.Zero, CInt(PlaySoundFlags.SND_FILENAME Or PlaySoundFlags.SND_SYNC))
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at Playing Sound:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click

        Try
            m_ConnectAlternate = 0
            Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Highest
            Cursor.Current = Cursors.WaitCursor
            m_Cancelled = True
            btnCancel.Enabled = False
#If RF Then
            Play(Macros.CANCEL_CLICK)
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try

        Application.DoEvents()
    End Sub

    Private Sub btnTimeoutRetry_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTimeoutRetry.Click
        Try
            objAppContainer.objLogger.WriteAppLog("Timeout occured: RETRY Selected", Logger.LogLevel.INFO)
            m_TimeoutRetry = True
            Me.Close()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub btnTimeoutCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTimeoutCancel.Click
        Try
            objAppContainer.objLogger.WriteAppLog("Timeout occured: CANCEL selected", Logger.LogLevel.INFO)
            m_TimeoutCancel = True
            Me.Close()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Function is invoked when user select OK on connect to Alternate message box.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnCancelAlternate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancelAlternate.Click
        Try
            m_ConnectAlternate = -1
            Me.Close()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Function is invoked when user select Alternate on connect to Alternate message box.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnConnectAlternate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnectAlternate.Click
        Try
            m_ConnectAlternate = 1
            Me.Close()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class