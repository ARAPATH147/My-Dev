''' * Modification Log
''' 
'''****************************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Avoid displaying error message while reconnect fail to primary controller
''' </Summary>
'''****************************************************************************************
Public Class frmGoodsInMenu
    Inherits System.Windows.Forms.Form
    Friend WithEvents lbl1 As System.Windows.Forms.Label
    Friend WithEvents lbl2 As System.Windows.Forms.Label
    Friend WithEvents tmrAlarm As System.Windows.Forms.Timer
    Friend WithEvents pnlSSCReceiving As System.Windows.Forms.Panel
    Public Delegate Sub StartTimerCallback()
    Public Delegate Sub StopTimerCallback()
    Private iInterval As Integer = 0
#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'Add event handler for getting the auto logoff timer tick.
        AddHandler objAppContainer.objAutologOff.evtPowerOn, AddressOf Me.StopTimer
        AddHandler objAppContainer.objAutologOff.evtUserIdle, AddressOf Me.StartTimer
#If RF Then
 'RECONNECT 

        objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
#End If


        'This call is required by the Windows Form Designer.
        InitializeComponent()
        If objAppContainer.objConfigValues.UODActive = "N" Then
            Me.lblSSCReceiving.ForeColor = Color.Gray
            Me.pnlSSCReceiving.Enabled = False
        End If
        'No need to check ASNactive flag
        If objAppContainer.objConfigValues.DirectsActive = "N" Then
            Me.lblDirectsReceiving.ForeColor = Color.Gray
            Me.pnlDirectsReceiving.Enabled = False
        End If
        'Add any initialization after the InitializeComponent() call
        ' Add any initialization after the InitializeComponent() call.
        'iInterval = CInt(ConfigDataMgr.GetInstance().GetParam(ConfigKey.AUTO_LOGOFF_TIMEOUT))
        'Convert minutes to milliseconds.
        'tmrAlarm.Interval = iInterval * 60 * 1000
        'Start the thread for the auto logoff.
        objAppContainer.objAutologOff.Start()

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents lbl3 As System.Windows.Forms.Label
    Friend WithEvents pbLogOff As System.Windows.Forms.PictureBox
    Friend WithEvents lblDirectsReceiving As System.Windows.Forms.Label
    Friend WithEvents pnlDirectsReceiving As System.Windows.Forms.Panel
    Friend WithEvents lblSSCReceiving As System.Windows.Forms.Label
    Friend WithEvents Help1 As System.Windows.Forms.PictureBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGoodsInMenu))
        Me.lbl1 = New System.Windows.Forms.Label
        Me.lbl2 = New System.Windows.Forms.Label
        Me.pnlSSCReceiving = New System.Windows.Forms.Panel
        Me.lblSSCReceiving = New System.Windows.Forms.Label
        Me.pnlDirectsReceiving = New System.Windows.Forms.Panel
        Me.lblDirectsReceiving = New System.Windows.Forms.Label
        Me.lbl3 = New System.Windows.Forms.Label
        Me.Help1 = New System.Windows.Forms.PictureBox
        Me.pbLogOff = New System.Windows.Forms.PictureBox
        Me.tmrAlarm = New System.Windows.Forms.Timer
        Me.pnlSSCReceiving.SuspendLayout()
        Me.pnlDirectsReceiving.SuspendLayout()
        Me.SuspendLayout()
        '
        'lbl1
        '
        Me.lbl1.Location = New System.Drawing.Point(24, 64)
        Me.lbl1.Name = "lbl1"
        Me.lbl1.Size = New System.Drawing.Size(16, 16)
        Me.lbl1.Text = "1."
        '
        'lbl2
        '
        Me.lbl2.Location = New System.Drawing.Point(24, 109)
        Me.lbl2.Name = "lbl2"
        Me.lbl2.Size = New System.Drawing.Size(16, 20)
        Me.lbl2.Text = "2."
        '
        'pnlSSCReceiving
        '
        Me.pnlSSCReceiving.Controls.Add(Me.lblSSCReceiving)
        Me.pnlSSCReceiving.Location = New System.Drawing.Point(40, 56)
        Me.pnlSSCReceiving.Name = "pnlSSCReceiving"
        Me.pnlSSCReceiving.Size = New System.Drawing.Size(144, 40)
        '
        'lblSSCReceiving
        '
        Me.lblSSCReceiving.Location = New System.Drawing.Point(8, 7)
        Me.lblSSCReceiving.Name = "lblSSCReceiving"
        Me.lblSSCReceiving.Size = New System.Drawing.Size(128, 28)
        Me.lblSSCReceiving.Text = "Stores Service Centre Receiving"
        '
        'pnlDirectsReceiving
        '
        Me.pnlDirectsReceiving.Controls.Add(Me.lblDirectsReceiving)
        Me.pnlDirectsReceiving.Location = New System.Drawing.Point(40, 104)
        Me.pnlDirectsReceiving.Name = "pnlDirectsReceiving"
        Me.pnlDirectsReceiving.Size = New System.Drawing.Size(120, 24)
        '
        'lblDirectsReceiving
        '
        Me.lblDirectsReceiving.Location = New System.Drawing.Point(8, 4)
        Me.lblDirectsReceiving.Name = "lblDirectsReceiving"
        Me.lblDirectsReceiving.Size = New System.Drawing.Size(104, 16)
        Me.lblDirectsReceiving.Text = "Directs Receiving"
        '
        'lbl3
        '
        Me.lbl3.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lbl3.Location = New System.Drawing.Point(24, 168)
        Me.lbl3.Name = "lbl3"
        Me.lbl3.Size = New System.Drawing.Size(100, 20)
        Me.lbl3.Text = "Select from list"
        '
        'Help1
        '
        Me.Help1.Image = CType(resources.GetObject("Help1.Image"), System.Drawing.Image)
        Me.Help1.Location = New System.Drawing.Point(176, 8)
        Me.Help1.Name = "Help1"
        Me.Help1.Size = New System.Drawing.Size(32, 32)
        Me.Help1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbLogOff
        '
        Me.pbLogOff.Image = CType(resources.GetObject("pbLogOff.Image"), System.Drawing.Image)
        Me.pbLogOff.Location = New System.Drawing.Point(24, 208)
        Me.pbLogOff.Name = "pbLogOff"
        Me.pbLogOff.Size = New System.Drawing.Size(88, 24)
        Me.pbLogOff.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'tmrAlarm
        '
        '
        'frmGoodsInMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.pbLogOff)
        Me.Controls.Add(Me.Help1)
        Me.Controls.Add(Me.lbl3)
        Me.Controls.Add(Me.pnlDirectsReceiving)
        Me.Controls.Add(Me.pnlSSCReceiving)
        Me.Controls.Add(Me.lbl2)
        Me.Controls.Add(Me.lbl1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmGoodsInMenu"
        Me.Text = "Goods In"
        Me.pnlSSCReceiving.ResumeLayout(False)
        Me.pnlDirectsReceiving.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region
    ''' <summary>
    ''' Function to handle click event for Direct Receiving option from main menu.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pnlDirectsReceiving_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlDirectsReceiving.Click
        FreezeControls()
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        Dim objDirectsRecv As New frmDirReceive
        objDirectsRecv.Show()
        '  objAppContainer.objDirReceive.Show()
        UnfreezeControls()
    End Sub
    ''' <summary>
    ''' Function to handle click event for SSC Receiving option from main menu.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pnlSSCReceiving_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlSSCReceiving.Click
        FreezeControls()
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        Dim objSSCRecv As New frmSSCReceivingMainMenu
        objSSCRecv.Show()
        ' objAppContainer.objSSCReceivingMainMenu.Show()
        UnfreezeControls()
    End Sub
    ''' <summary>
    ''' Function to handle Logoff option from main menu.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbLogOff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbLogOff.Click
        Dim diaResult As New DialogResult
        FreezeControls()
#If RF Then
        diaResult = MessageBox.Show(MessageManager.GetInstance.GetMessage("M105"), _
                                    "Confirmation", _
                                    MessageBoxButtons.YesNo, _
                                    MessageBoxIcon.Question, _
                                    MessageBoxDefaultButton.Button1)
        UnfreezeControls()
        If diaResult = Windows.Forms.DialogResult.Yes Then

            If Not objAppContainer.objDataEngine.LogOff() Then
                Exit Sub
            Else
                Me.Dispose()
            End If

            'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        End If
#ElseIf NRF Then

        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.CUSTOM, "Logging Off...")
        If UserSessionManager.GetInstance.ValidateExData() Then
            'Check if device is docked
            If UserSessionManager.GetInstance.CheckDeviceDocked() Then
                'Bring up splash screen
                objAppContainer.objSplashScreen.ChangeLabelText("Downloading export data...")
                objAppContainer.objLogger.WriteAppLog("Application Logg off initiated", _
                                                      Logger.LogLevel.RELEASE)
                'Call logoutSession 
                If UserSessionManager.GetInstance.LogOutSession(True) Then
                    objAppContainer.objLogger.WriteAppLog("Application Logg off complete", _
                                                          Logger.LogLevel.RELEASE)
                Else
                    objAppContainer.objLogger.WriteAppLog("Application Logoff.Connection " _
                                                          & "failure with controller", _
                                                          Logger.LogLevel.RELEASE)
                    'Bring up splash screen
                    'objAppContainer.objSplashScreen.ChangeLabelText()
                    'Allow the user to try again.
                    'v1.1 Start: MCF Change to avoid getting error message in case of reconnect
                    If objAppContainer.iConnectedToAlternate <> 1 Then
                        UnfreezeControls()
                        Me.Visible = True
                        objAppContainer.objStatusBar.Visible = True
                        objAppContainer.objStatusBar.Refresh()
                        Me.Visible = True
                        Me.Refresh()
                        'objAppContainer.objStatusBar.Refresh()
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                        Exit Sub
                    End If
                    'v1.1 End: MCF Change to avoid getting error message in case of reconnect
                End If
            Else
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M29"), _
                                "Alert", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, _
                                MessageBoxDefaultButton.Button1)
                'Allow the user to try again.
                UnfreezeControls()
                objAppContainer.objStatusBar.Visible = True
                objAppContainer.objStatusBar.Refresh()
                Me.Visible = True
                Me.Refresh()
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                Exit Sub
            End If
        Else
            diaResult = MessageBox.Show(MessageManager.GetInstance.GetMessage("M105"), _
                                    "Confirmation", _
                                        MessageBoxButtons.YesNo, _
                                        MessageBoxIcon.Question, _
                                        MessageBoxDefaultButton.Button1)
            If diaResult = Windows.Forms.DialogResult.No Then
                UnfreezeControls()
                objAppContainer.objStatusBar.Visible = True
                objAppContainer.objStatusBar.Location = New Drawing.Point(0, 26 * objAppContainer.iOffSet)
                objAppContainer.objStatusBar.Refresh()
                Me.Visible = True
                Me.Refresh()
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                Exit Sub
            End If
        End If
        objAppContainer.objAutologOff.SetSleepTimeOut()
        'Bring up splash screen
        objAppContainer.objSplashScreen.ChangeLabelText("Application Logging off. Please wait... ")
        Me.Dispose()
#End If
    End Sub
    ''' <summary>
    ''' Function to handle Help option from main menu.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Help1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Help1.Click
        FreezeControls()
        MessageBox.Show(MessageManager.GetInstance().GetMessage("M81"), "Help")
        UnfreezeControls()
    End Sub
    ''' <summary>
    ''' Function to freeze controls
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub FreezeControls()
        RemoveHandler pnlDirectsReceiving.Click, AddressOf pnlDirectsReceiving_Click
        RemoveHandler pnlSSCReceiving.Click, AddressOf pnlSSCReceiving_Click
        Me.Help1.Enabled = False
        Me.pbLogOff.Enabled = False
    End Sub
    ''' <summary>
    ''' Function to unfreeze controls
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UnfreezeControls()
        Application.DoEvents()
        AddHandler pnlDirectsReceiving.Click, AddressOf pnlDirectsReceiving_Click
        AddHandler pnlSSCReceiving.Click, AddressOf pnlSSCReceiving_Click
        Me.Help1.Enabled = True
        Me.pbLogOff.Enabled = True
    End Sub
    ''' <summary>
    ''' Timer to handle auto logoff timeout.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub tmrAlarm_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrAlarm.Tick
        'Disable the timer
        StopTimer()
        'Call autologoff function to perform the logoff action.
        If objAppContainer.AutoLogOffSession() Then
            objAppContainer.objLogger.WriteAppLog("Auto logoff successfully competed.", _
                                                  Logger.LogLevel.RELEASE)
        Else
            objAppContainer.objLogger.WriteAppLog("Auto logoff failed.", _
                                                  Logger.LogLevel.RELEASE)
        End If
        'close the shelf management menu display.
        Me.Dispose()
    End Sub
    ''' <summary>
    ''' The timer paramers are set 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartTimer()
        ' Runs the timer, and raises the event.
        If Not (Me.InvokeRequired) Then
            'tmrAlarm.Enabled = True
            'Call autologoff function to perform the logoff action.
            If objAppContainer.AutoLogOffSession() Then
                objAppContainer.objLogger.WriteAppLog("Auto logoff successfully competed.", _
                                                      Logger.LogLevel.RELEASE)
            Else
                objAppContainer.objLogger.WriteAppLog("Auto logoff failed.", _
                                                      Logger.LogLevel.RELEASE)
            End If
            ''Bring up splash screen
            'objAppContainer.objSplashScreen.ChangeLabelText("Application Logging off. Please wait... ")
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            Try
                'Threading.Thread.Sleep(3000)
                'objAppContainer.objAutologOff.Stop()
                'objAppContainer.objAutologOff = Nothing
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("Stopping Auto logoff thread.", _
                                                      Logger.LogLevel.RELEASE)
            End Try
            Me.Dispose()
        Else
            Me.Invoke(New StartTimerCallback(AddressOf StartTimer))
        End If
    End Sub
    ''' <summary>
    ''' The timer paramers are unset 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StopTimer()
        ' Runs the timer, and raises the event.
        If Not (Me.InvokeRequired) Then
            tmrAlarm.Enabled = False
        Else
            Me.Invoke(New StopTimerCallback(AddressOf StopTimer))
        End If
    End Sub
End Class
