Imports System.IO
'''****************************************************************************
''' <FileName>frmShlfMgmntMenu.vb</FileName>
''' <summary>
''' Displays Shelf Management main menu with tabs and buttons to choose various
''' modules present under Shelf Management application. Auto logoff process is 
''' started from this form. if the user is idle for a configurable time period
''' then the application is logged off automatically.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>03-Feb-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''****************************************************************************
Public Class frmShlfMgmntMenu
    Private iInterval As Integer = 0
    Public Delegate Sub StartTimerCallback()
    Public Delegate Sub StopTimerCallback()
    ''' <summary>
    ''' Constructor for this class.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'Add event handler for getting the auto logoff timer tick.
        AddHandler objAppContainer.objAutologOff.evtPowerOn, AddressOf Me.StopTimer
        AddHandler objAppContainer.objAutologOff.evtUserIdle, AddressOf Me.StartTimer
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        '#If NRF Then
        'Call to hide menu icons like Item Sales, Store Sales, Reports etc
        HideRFIcons()
        ' Add any initialization after the InitializeComponent() call.
        'iInterval = CInt(ConfigDataMgr.GetInstance().GetParam(ConfigKey.AUTO_LOGOFF_TIMEOUT))
        'Convert minutes to milliseconds.
        'tmrAlarm.Interval = iInterval * 60 * 1000
        'Start the thread for the auto logoff.
        objAppContainer.objAutologOff.Start()
        '#End If
    End Sub
    ''' <summary>
    ''' Shelf Monitor Module
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbxSMMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSMMenu.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Shelf Monitor...")
            SMSessionMgr.GetInstance().StartSession()
            UnFreezeControls()
        Catch ex As Exception
            UnFreezeControls()
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Picking List Menu
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbxPLMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbPLMenu.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Picking List...")
            If Not PLSessionMgr.GetInstance().StartSession() Then
                PLSessionMgr.GetInstance().EndSession()
            End If
            UnFreezeControls()
        Catch ex As Exception
#If NRF Then
            UnFreezeControls()
            PLSessionMgr.GetInstance().EndSession()
#ElseIf RF Then
            UnFreezeControls()
            If Not ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                PLSessionMgr.GetInstance().EndSession()
            End If
#End If
        End Try
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
    End Sub
#If RF Then
    ''' <summary>
    ''' Sets the status Bar Message as Disconnected
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBar()
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
    End Sub
#End If
    ''' <summary>
    ''' Log off module
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbLogOff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbLogOff.Click
        FreezeControls()
#If NRF Then
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Logging Off...")
        If NRFUserSessionManager.GetInstance.ValidateExData() Then
            'Check if device is docked
            If NRFUserSessionManager.GetInstance.CheckDeviceDocked() Then
                'Bring up splash screen
                objAppContainer.objSplashScreen. _
                                ChangeLabelText("Downloading export data...")
                objAppContainer.objLogger.WriteAppLog("Application Logg off initiated", _
                                                      Logger.LogLevel.RELEASE)
                'Call logoutSession 
                If NRFUserSessionManager.GetInstance.LogOutSession(True) Then
                    objAppContainer.objLogger.WriteAppLog("Application Logg off complete", _
                                                          Logger.LogLevel.RELEASE)
                Else
                    objAppContainer.objLogger.WriteAppLog("Application Logoff.Connection " _
                                                          & "failure with controller", _
                                                          Logger.LogLevel.RELEASE)
                    'Bring up splash screen
                    'objAppContainer.objSplashScreen.ChangeLabelText()
                    'Allow the user to try again.
                    UnFreezeControls()
                    Me.Visible = True
                    Me.Refresh()
                    Exit Sub
                End If
            Else
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M29"), _
                                "Alert", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, _
                                MessageBoxDefaultButton.Button1)
                'Unfreeze the controls and display the shelf management menu screen.
                UnFreezeControls()
                Me.Refresh()
                'IT Internal
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                Exit Sub
            End If
        Else
            Dim diaResult As New DialogResult
            diaResult = MessageBox.Show(MessageManager.GetInstance.GetMessage("M72"), _
                                    "Confirmation", _
                                        MessageBoxButtons.YesNo, _
                                        MessageBoxIcon.Question, _
                                        MessageBoxDefaultButton.Button1)
            If diaResult = Windows.Forms.DialogResult.No Then

                UnFreezeControls()
                Me.Visible = True
                Me.Refresh()
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                Exit Sub
            End If
        End If
        objAppContainer.objAutologOff.SetSleepTimeOut()
        UnFreezeControls()
        'Bring up splash screen
        objAppContainer.objSplashScreen.ChangeLabelText("Application Logging off. Please wait... ")
        Me.Dispose()
#ElseIf RF Then
        'Add Applicaiton Logoff Routine for RF world here.
        'Show user confirmation message
        Dim diaResult As New DialogResult
        diaResult = MessageBox.Show(MessageManager.GetInstance.GetMessage("M72"), _
                                "Confirmation", _
                                    MessageBoxButtons.YesNo, _
                                    MessageBoxIcon.Question, _
                                    MessageBoxDefaultButton.Button1)
        If diaResult = Windows.Forms.DialogResult.No Then
            UnFreezeControls()
            Me.Visible = True
            Me.Refresh()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY )
            Exit Sub
            'If user chooses "yes" from the screen
        ElseIf diaResult = Windows.Forms.DialogResult.Yes Then
            'Send OFF record to the controller
            If RFUserSessionManager.GetInstance.LogOutSession(False) Then
                'Got ACK from controller
                objAppContainer.objAutologOff.ChangePowerState()
                UnFreezeControls()
                'Bring up splash screen
                objAppContainer.objSplashScreen.ChangeLabelText("Application Logging off. Please wait... ")
                Me.Dispose()
            End If
        End If
#End If
    End Sub
    ''' <summary>
    ''' Fast Fill Module
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbFstFillMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbFstFillMenu.Click
        FreezeControls()
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Fast Fill...")
            FFSessionMgr.GetInstance().StartSession()
            'Already a call for Home screen display exists in Start Session.
            'FFSessionMgr.GetInstance().DisplayFFScreen(FFSessionMgr.FFSCREENS.Home)
        Catch ex As Exception
#If NRF Then
            FFSessionMgr.GetInstance().EndSession()
#ElseIf RF Then
            If Not ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                FFSessionMgr.GetInstance().EndSession()
                objAppContainer.objLogger.WriteAppLog("Excpetion occured while starting Fast fill ", Logger.LogLevel.RELEASE)
            End If
#End If
        End Try
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        UnFreezeControls()
    End Sub
    ''' <summary>
    ''' Price Check Module
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbPriceCheckMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbPriceCheckMenu.Click
        FreezeControls()
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Price Check...")
            If Not PCSessionMgr.GetInstance().StartSession() Then
                PCSessionMgr.GetInstance().EndSession()
            End If
        Catch ex As Exception
#If NRF Then
            'Handle PC Init Exception here.
            PCSessionMgr.GetInstance().EndSession()
#ElseIf RF Then
            If Not ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE And Not ex.Message = Macros.USER_ABORT Then
                'Handle PC Init Exception here.
                PCSessionMgr.GetInstance().EndSession()
                objAppContainer.objLogger.WriteAppLog("Exception Occured while starting Price Check Module", Logger.LogLevel.RELEASE)
            End If
#End If
        End Try
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        UnFreezeControls()
    End Sub
    ''' <summary>
    ''' Count List Module
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbCntLst_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbCntLst.Click
        FreezeControls()
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Count List...")
#If NRF Then
            If Not CLSessionMgr.GetInstance().StartSession() Then
                CLSessionMgr.GetInstance().EndSession()
            End If
#ElseIf RF Then
            If Not CLSessionMgr.GetInstance().StartSession() Then
                CLSessionMgr.GetInstance().EndSession()
            End If
#End If
        Catch ex As Exception
#If NRF Then
            'Handle CL Init Exception here.
            CLSessionMgr.GetInstance().EndSession()
#ElseIf RF Then
            If Not ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                'Handle CL Init Exception here.
                CLSessionMgr.GetInstance().EndSession()
                objAppContainer.objLogger.WriteAppLog("Connection loss in the CL start  session", Logger.LogLevel.RELEASE)
            End If
#End If
        End Try
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        UnFreezeControls()
    End Sub
    ''' <summary>
    ''' Excess Stock Module
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbExsStck_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbExsStck.Click
        FreezeControls()
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Excess Stock...")
            EXSessionMgr.GetInstance().StartSession()
        Catch ex As Exception
#If NRF Then
            'Handle EX Init Exception here.
            EXSessionMgr.GetInstance().EndSession()
#ElseIf RF Then
            If Not ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                'Handle EX Init Exception here.
                EXSessionMgr.GetInstance().EndSession()
                objAppContainer.objLogger.WriteAppLog("Connection loss in the EX start  session", Logger.LogLevel.RELEASE)
            End If
#End If
        End Try
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        UnFreezeControls()
    End Sub
    ''' <summary>
    ''' Item Info Module
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbxItemInfo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbxItemInfo.Click
        FreezeControls()
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Item Info...")
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.ITEMINFO)
        Catch ex As Exception
#If NRF Then
            'Handle ItemInfo Init Exception here.
            ItemInfoSessionMgr.GetInstance().EndSession()
#ElseIf RF Then
            If Not ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                'Handle ItemInfo Init Exception here.
                ItemInfoSessionMgr.GetInstance().EndSession()
            End If
#End If
        End Try
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        UnFreezeControls()
    End Sub
    ''' <summary>
    ''' Auto Stuff Your Shelves
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbxAutoSYS_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbxAutoSYS.Click
        FreezeControls()
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Auto Stuff Your Shelves...")
            AutoSYSSessionManager.GetInstance().StartSession()
        Catch ex As Exception
#If NRF Then
            'Handle AutoSys Init Exception here.
            AutoSYSSessionManager.GetInstance().EndSession()
#End If
        End Try
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        UnFreezeControls()
    End Sub
    ''' <summary>
    ''' Print SEL module to be started.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbPrintSEL_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbPSMenu.Click
        FreezeControls()
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Print SEL...")
            PSSessionMgr.GetInstance().StartSession()
            PSSessionMgr.GetInstance().DisplayPSScreen(PSSessionMgr.PSSCREENS.Home)
        Catch ex As Exception
#If NRF Then
            'Handle Print SEL Init Exception here.
            PSSessionMgr.GetInstance().EndSession()
#ElseIf RF Then
            If Not ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                'Handle Print SEL Init Exception here.
                PSSessionMgr.GetInstance().EndSession()
            End If
#End If
        End Try
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        UnFreezeControls()
    End Sub
    ''' <summary>
    ''' To start print clearance label function.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub PBPrtClrLbl_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PBPrtClrLbl.Click
        FreezeControls()
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Print Clearance Label...")
            If MobilePrintSessionManager.GetInstance() IsNot Nothing Then
                If MobilePrintSessionManager.GetInstance().MobilePrinterStatus Then
                    If CLRSessionMgr.GetInstance().StartSession() Then
                        'Set the print type/method.
                        objAppContainer.bMobilePrinterAttachedAtSignon = True
                        objAppContainer.strPrintFlag = Macros.PRINT_LOCAL
                        CLRSessionMgr.GetInstance().DisplayPCLScreen(CLRSessionMgr.PCLSCREENS.PCLHelp)
                    End If
                Else
                    objAppContainer.bMobilePrinterAttachedAtSignon = False
                    objAppContainer.strCurrentPrinter = "0     Controller"
                    objAppContainer.strPrintFlag = Macros.PRINT_BATCH
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M80"), "Printer Warning", _
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                End If
            Else
                objAppContainer.bMobilePrinterAttachedAtSignon = False
                objAppContainer.strCurrentPrinter = "0     Controller"
                objAppContainer.strPrintFlag = Macros.PRINT_BATCH
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M80"), "Printer Warning", _
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If
        Catch ex As Exception
#If NRF Then
            'Handle Print SEL Init Exception here.
            CLRSessionMgr.GetInstance().EndSession()
#ElseIf RF Then
            If Not ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                'Handle Print SEL Init Exception here.
                CLRSessionMgr.GetInstance().EndSession()
            End If
#End If
        End Try
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        UnFreezeControls()
    End Sub
    ''' <summary>
    ''' To assign printer and send fonts for current session.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbAPMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbAPMenu.Click
        FreezeControls()
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Assign Printer...")
            APSessionMgr.GetInstance().StartSession()
            APSessionMgr.GetInstance().DisplayAPScreen(APSessionMgr.APSCREENS.Home)
        Catch ex As Exception
#If NRF Then
            'Handle Print SEL Init Exception here.
            APSessionMgr.GetInstance().EndSession()
#ElseIf RF Then
            If Not ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                'Handle Print SEL Init Exception here.
                APSessionMgr.GetInstance().EndSession()
            End If
#End If
        End Try
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        UnFreezeControls()
    End Sub
    ''' <summary>
    ''' Live Planner Module
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbLPMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbLPMenu.Click
        FreezeControls()
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Live Planners...")
            If SPSessionMgr.GetInstance().StartSession(SPSessionMgr.PLANTYPE.LivePlanner) Then
                SPSessionMgr.GetInstance().DisplaySPScreen(SPSessionMgr.SPSCREENS.LPHome)
            End If
        Catch ex As Exception
#If NRF Then
            'Handle Live Planner Init Exception here.
            'Fix: PS changed to SP
            SPSessionMgr.GetInstance().EndSession()
#ElseIf RF Then
            If Not ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                'Handle Live Planner Init Exception here.
                'Fix: PS changed to SP
                SPSessionMgr.GetInstance().EndSession()
            End If
#End If
        End Try
#If NRF Then
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#Else
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
#End If
        UnFreezeControls()
    End Sub
    ''' <summary>
    ''' Search Planner Module
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbSPMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSPMenu.Click
        FreezeControls()
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Search Planners...")
            If SPSessionMgr.GetInstance().StartSession(SPSessionMgr.PLANTYPE.SearchPlanner) Then
                SPSessionMgr.GetInstance().DisplaySPScreen(SPSessionMgr.SPSCREENS.SPHome)
            End If
        Catch ex As Exception
#If NRF Then
            'Handle Search Planner Init Exception here.
            'The Handling changed from PS to SP
            SPSessionMgr.GetInstance().EndSession()
#ElseIf RF Then
            If Not ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                'Handle Search Planner Init Exception here.
                'The Handling changed from PS to SP
                SPSessionMgr.GetInstance().EndSession()
            End If
#End If
        End Try
#If NRF Then
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#Else
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
#End If
        UnFreezeControls()
    End Sub
#If RF Then
    ''' <summary>
    ''' Pending Planners
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbPendingPlanner_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbPendingPlanner.Click
        FreezeControls()
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Pending Planners...")
            'If PPSessionMgr.GetInstance().StartSession(PPSessionMgr.PLANTYPE.PendingPlanner) Then
            '    PPSessionMgr.GetInstance().DisplaySPScreen(PPSessionMgr.PPSCREENS.LPHome)
            If SPSessionMgr.GetInstance().StartSession(SPSessionMgr.PLANTYPE.PendingPlanner) Then
                SPSessionMgr.GetInstance().DisplaySPScreen(SPSessionMgr.SPSCREENS.LPHome)
            Else
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M86"), "Planner Error", _
                               MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If
        Catch ex As Exception
#If NRF Then
            'Handle Live Planner Init Exception here.
            PSSessionMgr.GetInstance().EndSession()
#ElseIf RF Then
            If Not ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                'Handle Live Planner Init Exception here.
                PSSessionMgr.GetInstance().EndSession()
                objAppContainer.objLogger.WriteAppLog("Error while starting the PP session", Logger.LogLevel.RELEASE)
            End If
#End If
        End Try
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
        UnFreezeControls()
    End Sub
#End If
    ''' <summary>
    ''' The timer paramers are set 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartTimer()
        ' Runs the timer, and raises the event.
        If Not (Me.InvokeRequired) Then
            'tmrAlarm.Enabled = True
            'Perform Autologoff.
            'Call autologoff function to perform the logoff action.
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Auto Log Off...")
            If objAppContainer.AutoLogOffSession() Then
                objAppContainer.objLogger.WriteAppLog("Auto logoff successfully completed.", _
                                                      Logger.LogLevel.RELEASE)
            Else
                objAppContainer.objLogger.WriteAppLog("Auto logoff failed.", _
                                                      Logger.LogLevel.RELEASE)
            End If
            'close the shelf management menu display.
            Me.Dispose()
        Else
            Me.Invoke(New StartTimerCallback(AddressOf StartTimer))
        End If
    End Sub
    ''' <summary>
    ''' The timer paramers are set 
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
    ''' <summary>
    ''' To handle when the timer strikes
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub tmrAlarm_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrAlarm.Tick
        'Disable the timer
        StopTimer()
        'Call autologoff function to perform the logoff action.
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Auto LogOff...")
        If objAppContainer.AutoLogOffSession() Then
            objAppContainer.objLogger.WriteAppLog("Auto logoff successfully completed.", _
                                                  Logger.LogLevel.RELEASE)
        Else
            objAppContainer.objLogger.WriteAppLog("Auto logoff failed.", _
                                                  Logger.LogLevel.RELEASE)
        End If
        'close the shelf management menu display.
        Me.Dispose()
#If RF Then
        'anoop
        frmAutoLogOffMsg.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
        frmAutoLogOffMsg.ShowDialog()
        Exit Sub
#End If
    End Sub
    ''' <summary>
    ''' Executes when the form is loaded
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub frmShlfMgmntMenu_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Cursor.Current = Cursors.Default
        UnFreezeControls()
    End Sub
    ''' <summary>
    ''' To freeze the controls
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.pbCntLst.Enabled = False
            Me.pbExsStck.Enabled = False
            Me.pbFstFillMenu.Enabled = False
            Me.pbLogOff.Enabled = False
            Me.pbLPMenu.Enabled = False
            Me.pbPLMenu.Enabled = False
            Me.pbPriceCheckMenu.Enabled = False
            Me.pbPSMenu.Enabled = False
            Me.pbSMMenu.Enabled = False
            Me.pbSPMenu.Enabled = False
            Me.pbxItemInfo.Enabled = False
            Me.pbxAutoSYS.Enabled = False
            Me.pbStoreSales.Enabled = False
            Me.pbItemSales.Enabled = False
            Me.pbReports.Enabled = False
            Me.lblReports.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To unfreeze the control
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnFreezeControls()
        Try
            Me.pbCntLst.Enabled = True
            Me.pbExsStck.Enabled = True
            Me.pbFstFillMenu.Enabled = True
            Me.pbLogOff.Enabled = True
            Me.pbLPMenu.Enabled = True
            Me.pbPLMenu.Enabled = True
            Me.pbPriceCheckMenu.Enabled = True
            Me.pbPSMenu.Enabled = True
            Me.pbSMMenu.Enabled = True
            Me.pbSPMenu.Enabled = True
            Me.pbxItemInfo.Enabled = True
            Me.pbxAutoSYS.Enabled = True
            Me.pbItemSales.Enabled = True
            Me.pbStoreSales.Enabled = True
            Me.pbReports.Enabled = True
            Me.lblReports.Enabled = True
            'Stock File Accuracy - added new function to disable certain icons for normal staff
            DisableShlfMgmntIcons()   'Uncomment for release
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Hide the Icons which are not in 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub HideRFIcons()
        ' Commented for fixing the defect No: 9 MC55RF Observation log. So not deleting the code.
        'Uncommented for SFA SIT
#If NRF Then
        Me.pbItemSales.Visible = False
        Me.pbItemSales.Enabled = False
        Me.lblItemSales.Visible = False

        Me.lblStoreSales.Visible = False
        Me.pbStoreSales.Visible = False
        Me.pbStoreSales.Enabled = False

        Me.pbPendingPlanner.Visible = False
        Me.pbPendingPlanner.Enabled = False
        Me.lblPendingPlanner.Visible = False

        Me.pbReports.Enabled = False
        Me.pbReports.Visible = False
        Me.lblReports.Visible = False
        Me.lblReports.Enabled = False

        Me.pbxItemInfo.Location = Me.pbStoreSales.Location
        Me.lblItemInfo.Location = Me.lblStoreSales.Location
#End If
    End Sub
    ''' <summary>
    ''' disable the Icons for normal staff
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DisableShlfMgmntIcons()
        If Not objAppContainer.bIsStockSpecialist Then
            Me.pbSMMenu.Enabled = False
            Me.lblSMMenu.Enabled = False
            ''Me.pbPLMenu.Enabled = False  'SIT SFA Defect 349- enable FF and AFF pl for normal user
            ''Me.lblPLMenu.Enabled = False
            Me.pbCntLst.Enabled = False
            Me.lblCntLst.Enabled = False
            Me.pbExsStck.Enabled = False
            Me.lblExsStck.Enabled = False

            'SFA UAT
            Me.pbSMMenu.Visible = False
            Me.pbSMMenu_Gray.Visible = True
            Me.pbCntLst.Visible = False
            Me.pbCntlst_Gray.Visible = True
            Me.pbExsStck.Visible = False
            Me.pbExsStck_Gray.Visible = True
        Else
            Me.pbSMMenu.Visible = True
            Me.pbSMMenu.Enabled = True
            Me.pbSMMenu_Gray.Visible = False
            Me.pbCntLst.Visible = True
            Me.pbCntLst.Enabled = True
            Me.pbCntlst_Gray.Visible = False
            Me.pbExsStck.Visible = True
            Me.pbExsStck.Enabled = True
            Me.pbExsStck_Gray.Visible = False
        End If
    End Sub
    ''' <summary>
    ''' Executes when the shelf management main menu form loads.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub frmShlfMgmntMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Stock File Accuracy - added new function to disable certain icons for normal staff
        DisableShlfMgmntIcons()   'Uncomment for release
        ' end
#If NRF Then
        HideRFIcons()
        'pbItemSales.Visible = False
        'pbStoreSales.Visible=False
        'pbItemSales.Enabled=False
        'pbStoreSales.Enabled=False
        'lblItemSales.Visible=False
        'lblStoreSales.Visible=False
        'pbReports.Visible=False
        'lblReports.Visible=False
#End If
    End Sub
#If RF Then
    Private Sub pbItemSales_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbItemSales.Click
        FreezeControls()
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Item Sales...")
            ISSessionManager.GetInstance().StartSession()
        Catch ex As Exception
            If Not ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                'ISSessionManager.GetInstance().EndSession()
                objAppContainer.objLogger.WriteAppLog("exception occured during start session of item sales", Logger.LogLevel.RELEASE)
            End If
        End Try
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
        UnFreezeControls()
    End Sub

    Private Sub pbStoreSales_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbStoreSales.Click
        FreezeControls()
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Store Sales...")
            SSSessionManager.GetInstance().StartSession()
        Catch ex As Exception
            If Not ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                'Handle ItemSales Init Exception here.
                SSSessionManager.GetInstance().EndSession()
            End If
        End Try
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
        UnFreezeControls()
    End Sub

    Private Sub pbReports_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbReports.Click
        FreezeControls()
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Starting Reports")
            ReportsSessionManager.GetInstance().StartSession()
        Catch ex As Exception
            If Not ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                ReportsSessionManager.GetInstance().EndSession()
            End If
        End Try
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
        UnFreezeControls()
    End Sub
#End If
End Class
