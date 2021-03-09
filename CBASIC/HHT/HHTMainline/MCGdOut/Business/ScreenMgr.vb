'''***************************************************************
''' <FileName>ScreenMgr.vb</FileName>
''' <summary>
''' The class manages the population and display of screen during 
''' menu population
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
Public Class ScreenMgr
    Private Shared m_objScreenMgr As ScreenMgr
    Private objMenu As frmGdsOutMenu = Nothing
    Private objHelp As frmHelp = Nothing

    Private Sub New()
        objMenu = New frmGdsOutMenu
        objHelp = New frmHelp
    End Sub
    ''' <summary>
    ''' Function to make the class singleton
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As ScreenMgr
        If m_objScreenMgr Is Nothing Then
            m_objScreenMgr = New ScreenMgr
        End If
        Return m_objScreenMgr
    End Function
    ''' <summary>
    ''' Displays the Sign on Screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplaySignon()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplaySignon of  ScreenMgr", Logger.LogLevel.INFO)
        Try
            If objMenu Is Nothing Then
                objMenu = New frmGdsOutMenu
            End If

            ConfigureMenuDisplay(objMenu)


            objMenu.Visible = True
            objMenu.Show()

            'objLastForm = objMenu
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplaySignon of  ScreenMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplaySignon of  ScreenMgr", Logger.LogLevel.INFO)
    End Sub
    Public Sub UpdateStatusBar()
        Try
            objMenu.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            objHelp.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Instantiates the Goods Out or Credit claim business class and then 
    ''' display the Product Scan screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayProductScan()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayProductScan of  ScreenMgr", Logger.LogLevel.INFO)
        objMenu.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Opening " & WorkflowMgr.GetInstance().Title.ToString())
        Try
            Select Case WorkflowMgr.GetInstance().Description

                Case "Goods Out"
                    If GOSessionMgr.GetInstance.StartSession() Then
                        GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.Scan)
                    Else
                        WorkflowMgr.GetInstance().ExecQuit()
                    End If

                Case "Credit Claim"
                    If CCSessionMgr.GetInstance.StartSession() Then
                        CCSessionMgr.GetInstance().DisplayCCScreen(GOSessionMgr.GOSCREENS.Scan)
                    Else
                        WorkflowMgr.GetInstance().ExecQuit()
                    End If
            End Select
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayProductScan of  ScreenMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_REGAINED Then
                WorkflowMgr.GetInstance.ExecPrev()
            End If
#End If
        Finally
            objMenu.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayProductScan of  ScreenMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Display the Goods out Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayGOMenu()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayGOMenu of  ScreenMgr", Logger.LogLevel.INFO)

        Try
            If ConfigureMenuDisplay(objMenu) Then

                objMenu.Visible = True
                objMenu.Refresh()
            Else
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M40"), _
                        "Display Screen Error", MessageBoxButtons.OK, _
                        Nothing, MessageBoxDefaultButton.Button1)
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayGOMenu of  ScreenMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayGOMenu of  ScreenMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Display the Main Menu screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayMainMenu()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayMainMenu of  ScreenMgr", Logger.LogLevel.INFO)
        Try
            If ConfigureMenuDisplay(objMenu) Then
                objMenu.Visible = True
            Else
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M40"), _
                        "Display Screen Error", MessageBoxButtons.OK, _
                        Nothing, MessageBoxDefaultButton.Button1)
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayMainMenu of  ScreenMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayMainMenu of  ScreenMgr", Logger.LogLevel.INFO)
    End Sub

    ''' <summary>
    ''' Configures the menu items on the screen
    ''' </summary>
    ''' <param name="objDisplay">Object of the Goods out Menu form</param>
    ''' <returns>True if Successful / False if Failure</returns>
    ''' <remarks></remarks>
    Private Function ConfigureMenuDisplay(ByRef objDisplay As frmGdsOutMenu) As Boolean
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ConfigureMenuDisplay of  ScreenMgr", Logger.LogLevel.INFO)
        Try
            Dim m_iCurMenuItem As Integer
            Dim m_objCurMenuLabel As Label
            Dim m_objCurMenuPanel As Panel
            Dim m_objCurMenuPanel2 As Panel

            'Configure Array to hold the data in each control of the form
            ' Dim m_arrMenuItems As Array = Array.CreateInstance(GetType(Object), 7, 3)
            Dim m_arrMenuItems As Array = Array.CreateInstance(GetType(Object), 8, 3)
            m_arrMenuItems.SetValue(objDisplay.lblMenuItem1, 0, 0)
            m_arrMenuItems.SetValue(objDisplay.pnlMenuItem1, 0, 1)
            m_arrMenuItems.SetValue(objDisplay.pnMenuLabelColourIndicator1, 0, 2)

            m_arrMenuItems.SetValue(objDisplay.lblMenuItem2, 1, 0)
            m_arrMenuItems.SetValue(objDisplay.pnlMenuItem2, 1, 1)
            m_arrMenuItems.SetValue(objDisplay.pnMenuLabelColourIndicator2, 1, 2)

            m_arrMenuItems.SetValue(objDisplay.lblMenuItem3, 2, 0)
            m_arrMenuItems.SetValue(objDisplay.pnlMenuItem3, 2, 1)
            m_arrMenuItems.SetValue(objDisplay.pnMenuLabelColourIndicator3, 2, 2)

            m_arrMenuItems.SetValue(objDisplay.lblMenuItem4, 3, 0)
            m_arrMenuItems.SetValue(objDisplay.pnlMenuItem4, 3, 1)
            m_arrMenuItems.SetValue(objDisplay.pnMenuLabelColourIndicator4, 3, 2)

            m_arrMenuItems.SetValue(objDisplay.lblMenuItem5, 4, 0)
            m_arrMenuItems.SetValue(objDisplay.pnlMenuItem5, 4, 1)
            m_arrMenuItems.SetValue(objDisplay.pnMenuLabelColourIndicator5, 4, 2)

            m_arrMenuItems.SetValue(objDisplay.lblMenuItem6, 5, 0)
            m_arrMenuItems.SetValue(objDisplay.pnlMenuItem6, 5, 1)
            m_arrMenuItems.SetValue(objDisplay.pnMenuLabelColourIndicator6, 5, 2)

            m_arrMenuItems.SetValue(objDisplay.lblMenuItem7, 6, 0)
            m_arrMenuItems.SetValue(objDisplay.pnlMenuItem7, 6, 1)
            m_arrMenuItems.SetValue(objDisplay.pnMenuLabelColourIndicator7, 6, 2)
            m_arrMenuItems.SetValue(objDisplay.lblMenuItem8, 7, 0)
            m_arrMenuItems.SetValue(objDisplay.pnlMenuItem8, 7, 1)
            m_arrMenuItems.SetValue(objDisplay.pnMenuLabelColourIndicator8, 7, 2)

            'Clearing all the controls on the form and reseting the screen
            objDisplay.Text = ""

            For m_iCurMenuItem = 0 To m_arrMenuItems.GetUpperBound(0) 'Length - 1 'WorkflowMgr.GetInstance().TotMenuItems - 1
                m_objCurMenuLabel = m_arrMenuItems.GetValue(m_iCurMenuItem, 0)
                m_objCurMenuPanel = m_arrMenuItems.GetValue(m_iCurMenuItem, 1)
                m_objCurMenuPanel2 = m_arrMenuItems.GetValue(m_iCurMenuItem, 2)
                m_objCurMenuLabel.Text = ""
                m_objCurMenuLabel.Visible = False
                m_objCurMenuPanel.Visible = False
            Next m_iCurMenuItem
            Dim Display As String = objDisplay.lblMenuItem1.Text
            'Set Form Title
            objDisplay.Text = WorkflowMgr.GetInstance().Description

            'Set Page Title
            objDisplay.lblTitle.Text = WorkflowMgr.GetInstance().Title

            'Hide or show the Quit and logoff button
            objDisplay.btnLogoff.Visible = False

            'CR For Credit Claiming - Unsaleable Chilled Food & Damage- 
            If objDisplay.lblTitle.Text = "Credit Claiming" Then
                objDisplay.lblMenuItem1.Font = New System.Drawing.Font("Tahoma", 8.5!, System.Drawing.FontStyle.Bold)
                objDisplay.lblMenuItem2.Font = New System.Drawing.Font("Tahoma", 8.5!, System.Drawing.FontStyle.Bold)
                objDisplay.lblMenuItem3.Font = New System.Drawing.Font("Tahoma", 8.5!, System.Drawing.FontStyle.Bold)
                objDisplay.lblMenuItem4.Font = New System.Drawing.Font("Tahoma", 8.5!, System.Drawing.FontStyle.Bold)
                objDisplay.lblMenuItem5.Font = New System.Drawing.Font("Tahoma", 8.5!, System.Drawing.FontStyle.Bold)
                objDisplay.lblMenuItem6.Font = New System.Drawing.Font("Tahoma", 8.5!, System.Drawing.FontStyle.Bold)
                objDisplay.lblMenuItem7.Font = New System.Drawing.Font("Tahoma", 8.5!, System.Drawing.FontStyle.Bold)
                objDisplay.lblMenuItem8.Font = New System.Drawing.Font("Tahoma", 8.5!, System.Drawing.FontStyle.Bold)
                objDisplay.lblRecallNo1.Visible = False
                objDisplay.lblRecallNo2.Visible = False
                objDisplay.lblRecallNo3.Visible = False
                objDisplay.lblRecallNo4.Visible = False
                objDisplay.lblRecallNo5.Visible = False
                objDisplay.lblRecallType.Visible = False
                objDisplay.lblNumRecalls.Visible = False
                objDisplay.lblText.Visible = False
            ElseIf objDisplay.lblTitle.Text = "Recalls" Then
                MsgBx.ShowMessage("Please wait loading active recall list.")
                objMenu.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
                If RLSessionMgr.GetInstance.bIsRecallStarted = False Then
                    If RLSessionMgr.GetInstance().StartSession(objAppContainer.bRecallStarted) Then
                        objAppContainer.bIsCreateRecalls = False
                        'If RLSessionMgr.GetInstance().StartSession(False) Then
                        objAppContainer.bRecallStarted = True
                        RLSessionMgr.GetInstance().DisplayRecallCount()
                        objDisplay.lblMenuItem2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                        objDisplay.lblMenuItem3.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                        objDisplay.lblMenuItem4.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                        objDisplay.lblMenuItem5.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                        objDisplay.lblMenuItem6.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                        objDisplay.lblMenuItem7.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                        objDisplay.lblMenuItem8.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                        objDisplay.lblRecallNo1.Visible = True
                        objDisplay.lblRecallNo2.Visible = True
                        objDisplay.lblRecallNo3.Visible = True
                        objDisplay.lblRecallNo4.Visible = True
                        objDisplay.lblRecallNo5.Visible = True
                        objDisplay.lblRecallType.Visible = True
                        objDisplay.lblNumRecalls.Visible = True
                        objDisplay.lblText.Visible = True
                        objDisplay.lblRecallNo1.Text = RLSessionMgr.GetInstance().m_RecallCount.Customer
                        objDisplay.lblRecallNo2.Text = RLSessionMgr.GetInstance().m_RecallCount.Withdrawn
                        objDisplay.lblRecallNo3.Text = RLSessionMgr.GetInstance().m_RecallCount.Returns
                        objDisplay.lblRecallNo5.Text = RLSessionMgr.GetInstance().m_RecallCount.PlannerLeaver
                        objDisplay.lblRecallNo4.Text = RLSessionMgr.GetInstance().m_RecallCount.ExcessSalesPlan
                    Else
                        objAppContainer.bIsCreateRecalls = False

                        objDisplay.lblMenuItem2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                        objDisplay.lblMenuItem3.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                        objDisplay.lblMenuItem4.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                        objDisplay.lblMenuItem5.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                        objDisplay.lblMenuItem6.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                        objDisplay.lblMenuItem7.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                        objDisplay.lblMenuItem8.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                        objDisplay.lblRecallNo1.Visible = True
                        objDisplay.lblRecallNo2.Visible = True
                        objDisplay.lblRecallNo3.Visible = True
                        objDisplay.lblRecallNo4.Visible = True
                        objDisplay.lblRecallNo5.Visible = True
                        objDisplay.lblRecallType.Visible = True
                        objDisplay.lblNumRecalls.Visible = True
                        objDisplay.lblText.Visible = True
                        objDisplay.lblRecallNo1.Text = 0
                        objDisplay.lblRecallNo2.Text = 0
                        objDisplay.lblRecallNo3.Text = 0
                        objDisplay.lblRecallNo4.Text = 0
                        objDisplay.lblRecallNo5.Text = 0
                    End If
                Else
                    objAppContainer.bIsCreateRecalls = False
                    objAppContainer.bRecallStarted = True
                    RLSessionMgr.GetInstance().DisplayRecallCount()
                    objDisplay.lblMenuItem2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                    objDisplay.lblMenuItem3.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                    objDisplay.lblMenuItem4.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                    objDisplay.lblMenuItem5.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                    objDisplay.lblMenuItem6.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                    objDisplay.lblMenuItem7.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                    objDisplay.lblMenuItem8.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
                    objDisplay.lblRecallNo1.Visible = True
                    objDisplay.lblRecallNo2.Visible = True
                    objDisplay.lblRecallNo3.Visible = True
                    objDisplay.lblRecallNo4.Visible = True
                    objDisplay.lblRecallNo5.Visible = True
                    objDisplay.lblRecallType.Visible = True
                    objDisplay.lblNumRecalls.Visible = True
                    objDisplay.lblText.Visible = True
                    objDisplay.lblRecallNo1.Text = RLSessionMgr.GetInstance().m_RecallCount.Customer
                    objDisplay.lblRecallNo2.Text = RLSessionMgr.GetInstance().m_RecallCount.Withdrawn
                    objDisplay.lblRecallNo3.Text = RLSessionMgr.GetInstance().m_RecallCount.Returns
                    objDisplay.lblRecallNo5.Text = RLSessionMgr.GetInstance().m_RecallCount.PlannerLeaver
                    objDisplay.lblRecallNo4.Text = RLSessionMgr.GetInstance().m_RecallCount.ExcessSalesPlan
                End If
                'Setting the status bar with active data time
                objDisplay.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                MsgBx.CloseMessage()
            Else
                If objDisplay.lblTitle.Text = "Create Recall" And Not objAppContainer.bCreateRecall Then
                    objAppContainer.bCreateRecall = True
                    objAppContainer.bIsCreateRecalls = True
                End If
                
                objDisplay.lblMenuItem1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                objDisplay.lblMenuItem2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                objDisplay.lblMenuItem3.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                objDisplay.lblMenuItem4.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                objDisplay.lblMenuItem5.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                objDisplay.lblMenuItem6.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                objDisplay.lblMenuItem7.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                objDisplay.lblMenuItem8.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
                objDisplay.lblRecallNo1.Visible = False
                objDisplay.lblRecallNo2.Visible = False
                objDisplay.lblRecallNo3.Visible = False
                objDisplay.lblRecallNo4.Visible = False
                objDisplay.lblRecallNo5.Visible = False
                objDisplay.lblRecallType.Visible = False
                objDisplay.lblNumRecalls.Visible = False
                objDisplay.lblText.Visible = False
            End If
            'Setting menu items by iterating through the dataset
            For m_iCurMenuItem = 0 To WorkflowMgr.GetInstance().TotMenuItems - 1
                WorkflowMgr.GetInstance().SetMenuItems(m_iCurMenuItem + 1, m_arrMenuItems.GetValue(m_iCurMenuItem, 0), m_arrMenuItems.GetValue(m_iCurMenuItem, 1), m_arrMenuItems.GetValue(m_iCurMenuItem, 2), objDisplay.lblTitle.Text)
            Next m_iCurMenuItem

            'Hide/Show Buttons
            If WorkflowMgr.GetInstance().HelpTextID <> "" Then
                objDisplay.pbContextHelp.Visible = True
            Else
                objDisplay.pbContextHelp.Visible = False
            End If

            If WorkflowMgr.GetInstance().Quit <> "" Then
                objDisplay.btnQuit.Visible = True
                'If its the first screen display the logoff button instead of the 
                'Quit button
                If WorkflowMgr.GetInstance.WFIndex = "1" Then
                    objDisplay.btnQuit.Visible = False
                    objDisplay.btnLogoff.Visible = True
                    objDisplay.btnLogoff.Location = New System.Drawing.Point(145 * objAppContainer.iOffSet, 235 * objAppContainer.iOffSet) 'Darwin changes
                    'Setting the status bar with active data time
                    objDisplay.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                End If
            Else
                objDisplay.btnQuit.Visible = False
            End If
            'Writing to Log INFO File while exit
            objAppContainer.objLogger.WriteAppLog("Exit ConfigureMenuDisplay of  ScreenMgr", Logger.LogLevel.INFO)
            Return True
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in ConfigureMenuDisplay of  ScreenMgr. Exception is: " + _
                                                  ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_REGAINED And objAppContainer.bRecallConnection Then
                WorkflowMgr.GetInstance().ExecPrev()
                Return True
            ElseIf ex.Message = Macros.CONNECTION_LOSS_EXCEPTION And objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.RECALL Then
                Return True
            ElseIf ex.Message = Macros.CONNECTIVITY_TIMEOUTCANCEL And objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.RECALL Then
                Return True
            End If
#End If
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Display the help screen with data from the dsHelpContext data table
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayHelp(Optional ByVal bWorkFlow As Boolean = True, Optional ByVal strHelpText As String = "")
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayHelp of  ScreenMgr", Logger.LogLevel.INFO)
        Dim m_sTempString As String = ""
        Try
            If bWorkFlow Then
                If WorkflowMgr.GetInstance().Title <> vbNullString Then
                    objHelp.lblTitle.Text = "Help | " & WorkflowMgr.GetInstance().Title
                Else
                    objHelp.lblTitle.Text = "Help"
                End If
                m_sTempString = WorkflowMgr.GetInstance().HelpText
            Else
                objHelp.lblTitle.Text = "Help"
                m_sTempString = strHelpText
            End If



            'Convert the #### in the text to carriage return 
            objHelp.txtHelpText.Text = m_sTempString.Replace("####", vbNewLine)
            'Setting the status bar with active data time
            objHelp.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            objHelp.Visible = True
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayHelp of  ScreenMgr. Exception is: " _
                                                          + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        objMenu.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayHelp of  ScreenMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Terminate the objects and data in the screen manager class
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisposeScreenMgr()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisposeScreenMgr of  ScreenMgr", Logger.LogLevel.INFO)
        Try
            objMenu.Close()
            objMenu.Dispose()
            objHelp.Close()
            objHelp.Dispose()

            objMenu = Nothing
            objHelp = Nothing

            m_objScreenMgr = Nothing
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisposeScreenMgr of  ScreenMgr. Exception is: " _
                                                             + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisposeScreenMgr of  ScreenMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Create an instance of GOSessionManager and show the auth id
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayAuthorizationid()
        'Write to Log INFO File while entry
        objMenu.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Opening " & WorkflowMgr.GetInstance().Title.ToString())
        objAppContainer.objLogger.WriteAppLog("Entered DisplayAuthorizationid of  ScreenMgr", Logger.LogLevel.INFO)
        Try
            If GOSessionMgr.GetInstance.StartSession Then
                GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.Authorizationid)
            Else
                WorkflowMgr.GetInstance().ExecQuit()
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayAuthorizationid of  ScreenMgr. Exception is: " _
                                                             + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_REGAINED Then
                WorkflowMgr.GetInstance.ExecPrev()
            End If
#End If
        End Try
        objMenu.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayAuthorizationid of  ScreenMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Create an instance of GOSessionManager and show the recall id
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayRecallid()

        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayRecallid of  ScreenMgr", Logger.LogLevel.INFO)
        Try
            objMenu.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Opening " & WorkflowMgr.GetInstance().Title.ToString())
            If GOSessionMgr.GetInstance.StartSession() Then
                GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.Recallid)
            Else
                WorkflowMgr.GetInstance().ExecQuit()
            End If
            objMenu.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayRecallid of  ScreenMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_REGAINED Then
                WorkflowMgr.GetInstance.ExecPrev()
            End If
#End If
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayRecallid of  ScreenMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Create an instance of GOTransferMgr and show the Destination store id
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayDesinationStoreid()

        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayDesinationStoreid of  ScreenMgr", Logger.LogLevel.INFO)
        Try
            objMenu.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Opening " & WorkflowMgr.GetInstance().Title.ToString())
            If GOTransferMgr.GetInstance.StartSession() Then
                GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTransferMgr.GOTRANSFER.DestinationStoreId)
            Else
                WorkflowMgr.GetInstance().ExecQuit()
            End If
            objMenu.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayDesinationStoreid of  ScreenMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_REGAINED Then
                WorkflowMgr.GetInstance.ExecPrev()
            End If
#End If
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayDesinationStoreid of  ScreenMgr", Logger.LogLevel.INFO)
    End Sub

    ''' <summary>
    ''' Create an instance of PSWSessionManager and show the Scan UOD screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayUOD()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayUOD of  ScreenMgr", Logger.LogLevel.INFO)
        Try
            objMenu.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Opening " & WorkflowMgr.GetInstance().Title.ToString())
            If PSWSessionMgr.GetInstance.StartSession() Then
                PSWSessionMgr.GetInstance().DisplayPSWScreen(PSWSessionMgr.PSWSCREENS.ScanUOD)
            Else
                WorkflowMgr.GetInstance().ExecQuit()
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayUOD of  ScreenMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_REGAINED Then
                WorkflowMgr.GetInstance.ExecPrev()
            End If
#End If
        End Try
        objMenu.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayUOD of  ScreenMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Instantiate the Recall Session Manager and display the Recall List
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayRecallList()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayRecallList of  ScreenMgr", Logger.LogLevel.INFO)
        Try
            'Select Case UCase(WorkflowMgr.GetInstance.MenuName)
            '    Case UCase("Company/HO Recalls")
            '        WorkflowMgr.GetInstance.objActiveFeature = WorkflowMgr.ACTIVEFEATURE.COMPANYHORECALL
            '    Case UCase("Excess Salesplan Recalls")
            '        WorkflowMgr.GetInstance.objActiveFeature = WorkflowMgr.ACTIVEFEATURE.EXCESSSALESPLAN
            '    Case UCase("Planner Leaver Recalls")
            '        WorkflowMgr.GetInstance.objActiveFeature = WorkflowMgr.ACTIVEFEATURE.PLANNERLEAVER
            'End Select
            'MsgBx.ShowMessage("Please wait loading active recall list.")
            objMenu.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Loading Active Recall List")
            RLSessionMgr.GetInstance().CallingScreen = Nothing
            'Removed start session from constructor 
            'First start the session if succesful then initialise session variables.
            'If RLSessionMgr.GetInstance.StartSession() Then
            Dim isRecallAvailable As Boolean = False
            Select Case WorkflowMgr.GetInstance().CurMenuItem
                Case "2"
                    If RLSessionMgr.GetInstance().m_RecallCount.Customer > 0 Then
                        isRecallAvailable = True
                    End If
                Case "3"
                    If RLSessionMgr.GetInstance().m_RecallCount.Withdrawn > 0 Then
                        isRecallAvailable = True
                    End If
                Case "4"
                    If RLSessionMgr.GetInstance().m_RecallCount.Returns > 0 Then
                        isRecallAvailable = True
                    End If
                Case "6"
                    If RLSessionMgr.GetInstance().m_RecallCount.PlannerLeaver > 0 Then
                        isRecallAvailable = True
                    End If
                Case "5"
                    If RLSessionMgr.GetInstance().m_RecallCount.ExcessSalesPlan > 0 Then
                        isRecallAvailable = True
                    End If
            End Select
            If isRecallAvailable Then
                RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.ActiveRecallList)
            Else
                MessageBox.Show("No active recall list available", "Info", _
                                   MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                   MessageBoxDefaultButton.Button1)
                RLSessionMgr.GetInstance().bNoRecallsInList = True
                WorkflowMgr.GetInstance().ExecQuit()
            End If
            isRecallAvailable = False
            'End If

            'MsgBx.CloseMessage()


            objMenu.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayRecallList of  ScreenMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
#If RF Then
            If ex.Message = Macros.CONNECTION_REGAINED Then
                WorkflowMgr.GetInstance.ExecPrev()
            End If
#End If
        End Try
        objMenu.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayRecallList of  ScreenMgr", Logger.LogLevel.INFO)
    End Sub
End Class

