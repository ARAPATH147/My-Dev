Imports System.IO
Imports System.Data
Imports System.Xml
'''***************************************************************
''' <FileName>WorkflowMgr.vb</FileName>
''' <summary>
''' The Workflow Container Class which implements all business logic
''' and GUI navigation for Menu Population in Goods Outs.  
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Not to throw error when exiting from active controller
''' </Summary>
'''****************************************************************************
Public Class WorkflowMgr
    Private Shared m_objWorkflowMgr As WorkflowMgr
    Private m_icurWorkflowIndex As String = Nothing
    Private m_sDescription As String = Nothing
    Private m_sTitle As String = Nothing
    Private m_sPrevStep As String = Nothing
    Private m_sNextStep As String = Nothing
    Private m_sState As String = Nothing
    Private m_sScreen As String = Nothing
    Private m_bMenu As Boolean = Nothing
    Private m_iMenu As Integer = Nothing
    Private m_sBack As String = Nothing
    Private m_sQuit As String = Nothing
    Private m_sMenuItem As String = Nothing
    Private m_sType As String = Nothing
    Private m_sReasonCodeNum As String = Nothing
    Private m_sLabelColour As String = Nothing
    Private m_sHelpTextID As String = Nothing
    Private m_sHelpText As String = Nothing
    Private m_sMethodOfReturn As String = Nothing
    Private m_sCarrier As String = Nothing
    Private m_sDestination As String = Nothing
    Private m_sMenuName As String = Nothing
    Private m_sUODType As String = Nothing
    Private m_bDispMenuItemCol As Boolean = Nothing

    Public dsWorkflow As DataTable = Nothing
    Public dsHelpContext As DataTable = Nothing

    Public objActiveFeature As ACTIVEFEATURE
    ''' <summary>
    ''' Constructor initiates the data table and creates columns in it
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
        Try
            'Instantiate all the objects required
            Me.StartSession()
        Catch ex As Exception
            'Handle Goods out Init Exception here.
            Me.EndSession()
        End Try
    End Sub
    ''' <summary>
    ''' Initialises the Goods out Session 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartSession()
        'Creating a datatable and adding columns to it
        dsWorkflow = New DataTable
        dsWorkflow.Columns.Clear()
        dsWorkflow.Columns.Add("ID")
        dsWorkflow.Columns.Add("StepID")
        dsWorkflow.Columns.Add("Desc")
        dsWorkflow.Columns.Add("Type")
        dsWorkflow.Columns.Add("MenuItem")
        dsWorkflow.Columns.Add("Title")
        dsWorkflow.Columns.Add("PrevStep")
        dsWorkflow.Columns.Add("NextStep")
        dsWorkflow.Columns.Add("State")
        dsWorkflow.Columns.Add("Screen")
        dsWorkflow.Columns.Add("Menu")
        dsWorkflow.Columns.Add("Back")
        dsWorkflow.Columns.Add("Quit")
        dsWorkflow.Columns.Add("ReasonCodeNum")
        dsWorkflow.Columns.Add("DispMenuItemCol")
        dsWorkflow.Columns.Add("LabelColour")
        dsWorkflow.Columns.Add("ContextHelpID")
        dsWorkflow.Columns.Add("MethodOfReturn")
        dsWorkflow.Columns.Add("Carrier")
        dsWorkflow.Columns.Add("Destination")
        dsWorkflow.Columns.Add("UODTYPE")

        'Creating a datatable for Help Items
        dsHelpContext = New DataTable
        dsHelpContext.Columns.Clear()
        dsHelpContext.Columns.Add("HelpID")
        dsHelpContext.Columns.Add("HelpText")

    End Sub
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by GOSessionMgr.
    ''' </summary>
    ''' <returns>True if terminate is sucess else False</returns>
    ''' <remarks></remarks>
    Public Function EndSession()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered EndSession of  WorkflowMgr", Logger.LogLevel.INFO)
        Try

            m_icurWorkflowIndex = Nothing
            m_sDescription = Nothing
            m_sTitle = Nothing
            m_sPrevStep = Nothing
            m_sNextStep = Nothing
            m_sState = Nothing
            m_sScreen = Nothing
            m_bMenu = Nothing
            m_iMenu = Nothing
            m_sBack = Nothing
            m_sQuit = Nothing
            m_sMenuItem = Nothing
            m_sType = Nothing
            m_sReasonCodeNum = Nothing
            m_sLabelColour = Nothing
            m_sHelpTextID = Nothing
            m_sHelpText = Nothing
            m_sMethodOfReturn = Nothing
            m_sCarrier = Nothing
            m_sDestination = Nothing
            m_sMenuName = Nothing
            m_sUODType = Nothing
            m_bDispMenuItemCol = Nothing
            dsWorkflow = Nothing
            dsHelpContext = Nothing
            objActiveFeature = Nothing
            m_objWorkflowMgr = Nothing
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in EndSession of  WorkflowMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit EndSession of  WorkflowMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Function to make the class singleton
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As WorkflowMgr
        If m_objWorkflowMgr Is Nothing Then
            m_objWorkflowMgr = New WorkflowMgr
        End If
        Return m_objWorkflowMgr
    End Function

    ''' <summary>
    ''' Reads XML data into a dataset 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ReadXML()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ReadXML of  WorkflowMgr", Logger.LogLevel.INFO)

        Dim m_sAppPath As String
        m_sAppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)

        'Reads XML into a XML Text reader object
        If Not (System.IO.File.Exists(Macros.WORKFLOW_PATH)) Then
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M41"), "Workflow Error", MessageBoxButtons.OK, _
                                    Nothing, MessageBoxDefaultButton.Button1)
            'Exit Sub
        End If


        Dim objReader As New XmlTextReader(m_sAppPath + ConfigDataMgr.GetInstance.GetParam(ConfigKey.WORKFLOWXML))
        objReader.WhitespaceHandling = WhitespaceHandling.None
        objReader.MoveToFirstAttribute()

        'Parse the xml and add the data row by row into the data table
        While objReader.Read()
            If objReader.NodeType = XmlNodeType.Element And UCase(objReader.Name) = UCase("WorkflowStep") And objReader.AttributeCount() <> 0 Then
                objReader.MoveToElement()
                Dim m_newRow As DataRow = dsWorkflow.NewRow()

                Try
                    'Adding the read xml into the new row of Data Table
                    m_newRow("ID") = objReader.GetAttribute("ID")
                    m_newRow("StepID") = objReader.GetAttribute("StepID")
                    m_newRow("Desc") = objReader.GetAttribute("Desc")
                    m_newRow("Type") = objReader.GetAttribute("Type")
                    m_newRow("MenuItem") = objReader.GetAttribute("MenuItem")
                    m_newRow("Title") = objReader.GetAttribute("Title")
                    m_newRow("PrevStep") = objReader.GetAttribute("PrevStep")
                    m_newRow("NextStep") = objReader.GetAttribute("NextStep")
                    m_newRow("State") = objReader.GetAttribute("State")
                    m_newRow("Screen") = objReader.GetAttribute("Screen")
                    m_newRow("Menu") = objReader.GetAttribute("Menu")
                    m_newRow("Back") = objReader.GetAttribute("Back")
                    m_newRow("Quit") = objReader.GetAttribute("Quit")
                    m_newRow("ReasonCodeNum") = objReader.GetAttribute("ReasonCodeNum")
                    m_newRow("DispMenuItemCol") = objReader.GetAttribute("DispMenuItemCol")
                    m_newRow("LabelColour") = objReader.GetAttribute("LabelColour")
                    m_newRow("ContextHelpID") = objReader.GetAttribute("ContextHelpID")
                    m_newRow("MethodOfReturn") = objReader.GetAttribute("MethodOfReturn")
                    m_newRow("Carrier") = objReader.GetAttribute("Carrier")
                    m_newRow("Destination") = objReader.GetAttribute("Destination")
                    m_newRow("UODTYPE") = objReader.GetAttribute("UODTYPE")

                    dsWorkflow.Rows.Add(m_newRow)

                Catch ex As Exception

                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M41") & ex.Message, _
                                    "Workflow Error", MessageBoxButtons.OK, _
                                    Nothing, MessageBoxDefaultButton.Button1)

                    'Writing the Error to the Log File
                    objAppContainer.objLogger.WriteAppLog("Exception occured in ReadXML of  WorkflowMgr. Exception is: " _
                                                                      + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
                End Try
            ElseIf objReader.NodeType = XmlNodeType.Element And UCase(objReader.Name) = UCase("ContextHelp") And objReader.AttributeCount() <> 0 Then
                objReader.MoveToElement()
                Dim m_HelpnewRow As DataRow = dsHelpContext.NewRow()

                Try
                    'Adding the read xml into the new row of Data Table
                    m_HelpnewRow("HelpID") = objReader.GetAttribute("HelpID")
                    m_HelpnewRow("HelpText") = objReader.GetAttribute("HelpText")
                    dsHelpContext.Rows.Add(m_HelpnewRow)

                Catch ex As Exception
                    'Writing the Error to the Log File
                    objAppContainer.objLogger.WriteAppLog("Exception occured in Help Context ReadXML of  WorkflowMgr. Exception is: " _
                                                                      + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M49") & ex.Message, _
                    "Workflow Error", MessageBoxButtons.OK, _
                    Nothing, MessageBoxDefaultButton.Button1)
                End Try

            End If
        End While

        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit ReadXML of  WorkflowMgr", Logger.LogLevel.INFO)
    End Sub

    ''' <summary>
    '''  This subroutine is called to navigate to the next screen in the flow
    ''' </summary>
    ''' <param name="iMenuItem">The menu items selected from the screen</param>
    ''' <remarks></remarks>
    Public Sub NextScreen(ByVal iMenuItem As Integer)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered NextScreen of  WorkflowMgr", Logger.LogLevel.INFO)
        Try
            If WFIndex = "-1" Then NextStep = "1"
            CurMenuItem = iMenuItem.ToString

            'Appends the current step id with the menu item selected to get the step id 
            'of the next item
            If NextStep = "" And IsMenu = True And CurMenuItem > 0 Then
                NextStep = WFIndex.ToString & "." & iMenuItem.ToString
            End If

            'Set the current workflow index to the new workflow index
            WFIndex = NextStep

            'Read the dataset of the corresponding new step id
            If ReadWorkFlow(NextStep, "WORKFLOWSTEP") = True Then
                'Check for Help
                If HelpTextID <> "" Then
                    If ReadWorkFlow(HelpTextID, "CONTEXTHELP") = True Then
                        'Execute Workflow Step
                        SetFeatureName()
                        ExecCurWorkflow()
                    End If
                Else
                    'Execute Workflow Step
                    SetFeatureName()
                    ExecCurWorkflow()
                End If
            Else
                'To Do Message Box
                'Writing the Error to the Log File
                objAppContainer.objLogger.WriteAppLog("Exception occured: Reading work Flow data Failed in NextScreen of  WorkflowMgr")
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in NextScreen of  WorkflowMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit NextScreen of  WorkflowMgr", Logger.LogLevel.INFO)
    End Sub

    ''' <summary>
    ''' Loads the new screen with the corresponding step id details
    ''' </summary>
    ''' <returns>True for successful workflow execution else returns false</returns>
    ''' <remarks></remarks>

    Public Function ExecCurWorkflow() As Boolean
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ExecCurWorkflow of  WorkflowMgr", Logger.LogLevel.INFO)

        Try
            If Trim(Screen) <> "" Then
                'Call a function in the AppContainer Class to load the required screen
                objAppContainer.DisplayWorkflowScreen(Screen)
                If NextStep <> "" Then
                    WFIndex = NextStep
                End If
                'Writing to Log INFO File while exit
                objAppContainer.objLogger.WriteAppLog("Exit ExecCurWorkflow of  WorkflowMgr", Logger.LogLevel.INFO)
                ExecCurWorkflow = True
            Else
                'v1.1 MCF Change: Not to throw error when exiting from the active controller
                If Not objAppContainer.iConnectedToAlternate = -1 Then
                    ' if screen is empty it means the Workflow XML file was not read properly
                    MsgBox("Error in reading workflow xml", MsgBoxStyle.Critical, "WorkFlow Error")
                End If
                'Writing to Log INFO File while exit
                objAppContainer.objLogger.WriteAppLog("Exit ExecCurWorkflow of  WorkflowMgr", Logger.LogLevel.INFO)
                ExecCurWorkflow = False
                End If
        Catch ex As Exception
            MsgBox("Error in reading workflow xml", MsgBoxStyle.Critical, "WorkFlow Error")
            ExecCurWorkflow = False
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in ExecCurWorkflow of  WorkflowMgr. Exception is: " _
                                            + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Function

    ''' <summary>
    ''' Reads all attribute data from a corresponding step id
    ''' </summary>
    ''' <param name="sStepID">Step id</param>
    ''' <param name="sReaderName">If the data to be read in Help context or workflow</param>
    ''' <returns>True if read successfully / False item is not present in the dataset</returns>
    ''' <remarks></remarks>
    Private Function ReadWorkFlow(ByVal sStepID As String, ByVal sReaderName As String) As Boolean
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ReadWorkFlow of  WorkflowMgr", Logger.LogLevel.INFO)
        Try
            Dim m_objRow As DataRow
            'Read data from the dsWorkflow dataset
            If UCase(sReaderName) = "WORKFLOWSTEP" Then
                For Each m_objRow In dsWorkflow.Rows

                    If m_objRow("StepID") = sStepID Then
                        'Set current workflow properties
                        Description = m_objRow("Desc")
                        Title = m_objRow("Title")
                        PrevStep = m_objRow("PrevStep")
                        NextStep = m_objRow("NextStep")
                        State = m_objRow("State")
                        Screen = m_objRow("Screen")
                        SetMenu = m_objRow("Menu")
                        MenuName = m_objRow("MenuItem")
                        Back = m_objRow("Back")
                        Quit = m_objRow("Quit")
                        ReasonCodeNum = m_objRow("ReasonCodeNum")
                        Type = m_objRow("Type")
                        Labelcolour = m_objRow("LabelColour")
                        HelpTextID = m_objRow("ContextHelpID")
                        MethodOfReturn = m_objRow("MethodOfReturn")
                        Destination = m_objRow("Destination")
                        DisplayMenuColumn = CBool(m_objRow("DispMenuItemCol"))
                        UODType = m_objRow("UODTYPE")
                        Carrier = m_objRow("Carrier")
                        Destination = m_objRow("Destination")
                        Exit For
                    End If
                Next
                'Read data from the dsHelpContext dataset
            ElseIf UCase(sReaderName) = "CONTEXTHELP" Then
                For Each m_objRow In dsHelpContext.Rows
                    If m_objRow("HelpID") = sStepID Then
                        'Set Help context data
                        HelpText = m_objRow("HelpText")
                        Exit For
                    End If
                Next
            End If

            'Writing to Log INFO File while exit
            objAppContainer.objLogger.WriteAppLog("Exit ReadWorkFlow of  WorkflowMgr", Logger.LogLevel.INFO)
            ReadWorkFlow = True
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in ReadWorkFlow of  WorkflowMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            MessageBox.Show("ReadWorkFlowXML Memory Read Error. StepID='" & sStepID & "' " & Err.Description & ": Please contact the service desk.", "Attention")
            ReadWorkFlow = False
        End Try

    End Function
    Private Sub SetFeatureName()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered SetFeatureName of  WorkflowMgr", Logger.LogLevel.INFO)

        Try
            Select Case UCase(MenuName)
                Case UCase("Returns")
                    objActiveFeature = ACTIVEFEATURE.RETURNS
                Case UCase("Destroy")
                    objActiveFeature = ACTIVEFEATURE.DESTROY
                Case UCase("Fire/Flood damage")
                    If objActiveFeature = ACTIVEFEATURE.DESTROY Then
                        objActiveFeature = ACTIVEFEATURE.FIREFLOOD
                    End If
                Case UCase("Semi-Centralised Returns")
                    objActiveFeature = ACTIVEFEATURE.SEMICENTRALISED
                Case UCase("Direct Returns")
                    objActiveFeature = ACTIVEFEATURE.DIRECTRETURNS
                Case UCase("Centralised Returns")
                    objActiveFeature = ACTIVEFEATURE.CENTRALISED
                Case UCase("Goods Out Transfer")
                    objActiveFeature = ACTIVEFEATURE.TRANSFERS
                Case UCase("Stock Take")
                    objActiveFeature = ACTIVEFEATURE.STOCKTAKE
                Case UCase("Pharmacy Special Waste")
                    objActiveFeature = ACTIVEFEATURE.PHARMACYSPLWASTE
                Case UCase("Credit Claiming")
                    objActiveFeature = ACTIVEFEATURE.CREDITCLAIM
                Case UCase("Recall")
                    objActiveFeature = ACTIVEFEATURE.RECALL
                Case UCase("Create Recall")
                    objActiveFeature = ACTIVEFEATURE.CREATERECALL
                    objAppContainer.bIsCreateRecalls = True
                Case UCase("Returns and Recovery")
                    objActiveFeature = ACTIVEFEATURE.RETURNS
                Case UCase("Semi-Centralised Return")
                    objActiveFeature = ACTIVEFEATURE.SEMICENTRALISED
                Case UCase("Direct Return")
                    objActiveFeature = ACTIVEFEATURE.DIRECTRETURNS
                    'Case UCase("Company/HO Recalls")
                    '    objActiveFeature = ACTIVEFEATURE.COMPANYHORECALL
                    'Case UCase("Excess Salesplan Recalls")
                    '    objActiveFeature = ACTIVEFEATURE.EXCESSSALESPLAN
                    'Recalls CR
                Case UCase("Customer/Emergency")
                    objActiveFeature = ACTIVEFEATURE.CUSTOMEREMERGENCY
                Case UCase("Withdrawn")
                    objActiveFeature = ACTIVEFEATURE.WITHDRAWN
                Case UCase("100% Returns")
                    objActiveFeature = ACTIVEFEATURE.RECALLRETURNS
                Case UCase("Planner Leaver")
                    objActiveFeature = ACTIVEFEATURE.PLANNERLEAVER
                Case UCase("Excess Salesplan")
                    objActiveFeature = ACTIVEFEATURE.EXCESSSALESPLAN
            End Select
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in SetFeatureName of  WorkflowMgr. Exception is: " _
                                                            + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit SetFeatureName of  WorkflowMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Property to Get and set Display Menu Column variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DisplayMenuColumn() As String
        Get
            Return m_bDispMenuItemCol
        End Get
        Set(ByVal value As String)
            m_bDispMenuItemCol = value
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and set UODType variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UODType() As String
        Get
            Return m_sUODType
        End Get
        Set(ByVal value As String)
            If value <> "null" Then
                m_sUODType = value
            Else
                m_sUODType = ""
            End If
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and set Destination variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Destination() As String
        Get
            Return m_sDestination
        End Get
        Set(ByVal value As String)
            If value <> "" Then
                m_sDestination = value
            Else
                m_sDestination = ""
            End If
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and set Carrier variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Carrier() As String
        Get
            Return m_sCarrier
        End Get
        Set(ByVal value As String)
            m_sCarrier = value
        End Set
    End Property
    ''' <summary>
    '''  Property to Get and set MethodOfReturn variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MethodOfReturn() As String
        Get
            Return m_sMethodOfReturn
        End Get
        Set(ByVal value As String)
            m_sMethodOfReturn = value
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and Set Labelcolour variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Labelcolour() As String
        Get
            Return m_sLabelColour
        End Get
        Set(ByVal value As String)
            If value <> "null" Then
                m_sLabelColour = value
            End If
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and Set HelpTextID variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HelpTextID() As String
        Get
            Return m_sHelpTextID
        End Get
        Set(ByVal value As String)
            If value <> "null" Then
                m_sHelpTextID = value
            End If
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and Set HelpText variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HelpText() As String
        Get
            Return m_sHelpText
        End Get
        Set(ByVal value As String)
            If value <> "null" Then
                m_sHelpText = value
            End If
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and Set ReasonCodeNum variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ReasonCodeNum() As String
        Get
            Return System.Convert.ToInt64(Val(m_sReasonCodeNum))
        End Get
        Set(ByVal value As String)
            m_sReasonCodeNum = value
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and Set Type variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Type() As String
        Get
            Return m_sType
        End Get
        Set(ByVal value As String)
            If value <> "" Then 'if value is defined in the workflow.xml then save value
                m_sType = value
            End If
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and Set Quit variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Quit() As String
        Get
            Return m_sQuit
        End Get
        Set(ByVal value As String)
            m_sQuit = value
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and Set Back variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Back() As String
        Get
            Return m_sBack
        End Get
        Set(ByVal value As String)
            m_sBack = value
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and Set Screen variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Screen() As String
        Get
            Return m_sScreen
        End Get
        Set(ByVal value As String)
            m_sScreen = value
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and Set State variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property State() As String
        Get
            Return m_sState
        End Get
        Set(ByVal value As String)
            m_sState = value
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and Set NextStep variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NextStep() As String
        Get
            Return m_sNextStep
        End Get
        Set(ByVal value As String)
            m_sNextStep = value
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and Set PrevStep variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Property PrevStep() As String
        Get
            Return m_sPrevStep
        End Get
        Set(ByVal value As String)
            m_sPrevStep = value
        End Set
    End Property

    ''' <summary>
    ''' Property to Get and Set WFIndex variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property WFIndex() As String
        Get
            Return m_icurWorkflowIndex
        End Get
        Set(ByVal value As String)
            m_icurWorkflowIndex = value
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and Set Description variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Description() As String
        Get
            Return m_sDescription
        End Get
        Set(ByVal value As String)
            m_sDescription = value
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and Set Title variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Title() As String
        Get
            Return m_sTitle
        End Get
        Set(ByVal value As String)
            m_sTitle = value
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and Set CurMenuItem variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CurMenuItem() As String
        Get
            Return m_sMenuItem
        End Get
        Set(ByVal value As String)
            m_sMenuItem = value
        End Set
    End Property
    ''' <summary>
    ''' Property to Get and Set Menu Name variable
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MenuName() As String
        Get
            Return m_sMenuName
        End Get
        Set(ByVal value As String)
            m_sMenuName = value
        End Set
    End Property
    ''' <summary>
    ''' Resets the Workflow step index to -1 so that we can go back to the Main Menu screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Reset()
        WFIndex = "-1"
        DisplayMenuColumn = False
        Me.NextScreen(1)
    End Sub

    ''' <summary>
    ''' Property Returns the Colour to be displayed in the panel of the UOD scan screen
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property FetchLabelColourCode() As System.Drawing.Color
        Get
            Select Case UCase(m_sLabelColour)
                Case UCase("PURPLE")
                    Return Color.Purple
                Case UCase("ORANGE")
                    Return Color.Orange
                Case UCase("RED")
                    Return Color.Red
                Case UCase("BLACK")
                    Return Color.Black
                Case UCase("GREY")
                    Return Color.Gray
                Case UCase("YELLOW")
                    Return Color.Yellow
                Case UCase("WHITE")
                    Return Color.White
            End Select

        End Get
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sColour"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property CurrentMenuLabelColourCode(ByVal sColour As String) As System.Drawing.Color
        Get
            Select Case UCase(sColour)
                Case UCase("PURPLE")
                    Return Color.Purple
                Case UCase("ORANGE")
                    Return Color.Orange
                Case UCase("RED")
                    Return Color.Red
                Case UCase("BLACK")
                    Return Color.Black
                Case UCase("GREY")
                    Return Color.Gray
                Case UCase("YELLOW")
                    Return Color.Yellow
                Case UCase("WHITE")
                    Return Color.White
            End Select

        End Get
    End Property
    ''' <summary>
    ''' Property sets the Menu item
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    WriteOnly Property SetMenu() As Integer

        Set(ByVal Value As Integer)
            m_iMenu = Value
        End Set

    End Property
    ''' <summary>
    ''' Check if the screen to be populated has a menu or not
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsMenu() As Boolean
        Get
            If m_iMenu > 0 Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    ''' <summary>
    ''' Property returns the total menu items
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TotMenuItems() As Integer
        Get
            Return m_iMenu
        End Get
    End Property
    ''' <summary>
    ''' Terminate all the objects used in the WorkflowMgr class
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisposeWorkflowMgr()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisposeWorkflowMgr of  WorkflowMgr", _
                                              Logger.LogLevel.INFO)
        Try
            'Dispose everything else*************
            m_objWorkflowMgr = Nothing
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisposeWorkflowMgr of  WorkflowMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisposeWorkflowMgr of  WorkflowMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Sets the Menu Items on a Screen
    ''' </summary>
    ''' <param name="iCurMenuIndex">Menu index number</param>
    ''' <param name="objMenuItem">Object of the Panel that contains the label and panel</param>
    ''' <param name="objMenuItem2">object of the label</param>
    ''' <param name="objMenuItem3">Object of the Panel that displays color</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SetMenuItems(ByVal iCurMenuIndex As Integer, ByRef objMenuItem As Label, ByRef objMenuItem2 As Panel, ByRef objMenuItem3 As Panel, ByVal Type As String) As Boolean
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered SetMenuItems of  WorkflowMgr", Logger.LogLevel.INFO)
        Try
            'Obtain the Step id to be searched
            Dim sCurStepSearch As String : sCurStepSearch = (WFIndex.ToString & "." & iCurMenuIndex.ToString)
            Dim m_objRow As DataRow

            For Each m_objRow In dsWorkflow.Rows

                If m_objRow("StepID") = sCurStepSearch Then

                    If DisplayMenuColumn = True Then
                        'Set to new position and width and display labels
                        objMenuItem.Location = New System.Drawing.Point(25 * objAppContainer.iOffSet, objMenuItem.Location.Y)
                        objMenuItem3.BackColor = CurrentMenuLabelColourCode(m_objRow("LabelColour"))
                        objMenuItem3.Visible = True
                    Else
                        'Set back to original position and width and hide labels
                        objMenuItem.Location = New System.Drawing.Point(5 * objAppContainer.iOffSet, objMenuItem.Location.Y)
                        objMenuItem3.BackColor = System.Drawing.Color.White
                        objMenuItem3.Visible = False
                    End If
                    ''Set Page Title
                    'objDisplay.lblTitle.Text = WorkflowMgr.GetInstance().Title
                    If Type = "Recalls" Then
                        'Set current Menu Item workflow properties
                        objMenuItem.Text = m_objRow("MenuItem")
                    Else
                        'Set current Menu Item workflow properties
                        objMenuItem.Text = iCurMenuIndex.ToString & ". " & m_objRow("MenuItem")
                        'Fix for MC55 device to correct yellow colour.
                        If objMenuItem.Text = "4. Centralised Returns" And objAppContainer.iOffSet = 2 Then
                            objMenuItem3.BackColor = Color.FromArgb(255, 255, 180)
                        End If
                    End If
                

                    If objMenuItem.Text <> "" Then
                        objMenuItem.Visible = True
                        objMenuItem2.Visible = True
                    Else
                        objMenuItem.Visible = False
                        objMenuItem2.Visible = False
                    End If
                    Exit For
                End If
            Next
            'Writing to Log INFO File while exit
            objAppContainer.objLogger.WriteAppLog("Exit SetMenuItems of  WorkflowMgr", _
                                                  Logger.LogLevel.INFO)
            SetMenuItems = True

        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in SetMenuItems of  WorkflowMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            MessageBox.Show("ReadWorkFlowXML:SetMenuItems Error. Please contact the service desk.", "Attention")
            Dim elevateEx As New Exception("Workflow Configuration file error, verify file exists and valid.")
            Throw elevateEx
            SetMenuItems = False
        End Try

    End Function

    ''' <summary>
    ''' Populates the menu screen as per the Step id of Quit variable
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ExecQuit()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ExecQuit of WorkflowMgr", _
                                              Logger.LogLevel.INFO)
        Try
            'TODO : Session Completed code can come here*****************************
            Select Case Quit
                Case "0"
                    'Reset Workflow to the Beginning
                    Reset()
#If NRF Then
                    If objAppContainer.objUserSessionMgr.ValidateExData() Then
                        'For Logging off the user session 
                        If UserSessionManager.GetInstance.LogOutSession(True) Then
                            objAppContainer.objAutologOff.SetSleepTimeOut()
                            objAppContainer.bActiveform = False
                            WorkflowMgr.GetInstance().EndSession()
                        Else
                            'Do not logout from the main menu
                            Exit Sub
                        End If
                    Else
                        objAppContainer.objAutologOff.SetSleepTimeOut()
                        objAppContainer.bActiveform = False
                        WorkflowMgr.GetInstance().EndSession()
                    End If
#ElseIf RF Then
                    'For Logging off the user session 
                    If objAppContainer.objUserSessionMgr.LogOutSession(True) Then
                        objAppContainer.objAutologOff.ChangePowerState()
                        objAppContainer.bActiveform = False
                        WorkflowMgr.GetInstance().EndSession()
                    Else
                        'Do not logout from the main menu
                        Exit Sub
                    End If
#End If
                Case Else
                    WFIndex = Quit
                    'Navigate to defined quit step
                    If ReadWorkFlow(Quit, "WORKFLOWSTEP") = True Then

                        'Check for Help
                        If HelpTextID <> "" Then
                            If ReadWorkFlow(HelpTextID, "CONTEXTHELP") = True Then
                                'Execute Workflow Step
                                ExecCurWorkflow()
                            End If
                        Else
                            'Execute Workflow Step
                            ExecCurWorkflow()
                        End If
                    Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M43"), _
                                        "Exception", MessageBoxButtons.OK, _
                                        Nothing, MessageBoxDefaultButton.Button1)
                    End If
                    'ExecCurWorkflow()
            End Select
            ''To make the registry setting for the downloader
            'objAppContainer.objHelper.RegisterDownloader()
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in ExecQuit of  WorkflowMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit ExecQuit of  WorkflowMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Populates the menu screen as per the Step id of Prev variable
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ExecPrev()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ExecPrev of  WorkflowMgr", Logger.LogLevel.INFO)

        Try
            'Calculates PREVIOUS step and sets workflow properties
            WFIndex = PrevStep

            'Calculates Previous step and sets workflow properties
            If ReadWorkFlow(PrevStep, "WORKFLOWSTEP") = True Then

                'Check for Help
                If HelpTextID <> "" Then
                    If ReadWorkFlow(HelpTextID, "CONTEXTHELP") = True Then
                        'Execute Workflow Step
                        ExecCurWorkflow()
                    End If
                Else
                    'Execute Workflow Step
                    ExecCurWorkflow()
                End If
            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in ExecPrev of  WorkflowMgr. Exception is: " _
                                                             + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit ExecPrev of  WorkflowMgr", Logger.LogLevel.INFO)
    End Sub
    Public Enum ACTIVEFEATURE
        RETURNS
        DESTROY
        FIREFLOOD
        SEMICENTRALISED
        CENTRALISED
        DIRECTRETURNS
        TRANSFERS
        STOCKTAKE
        PHARMACYSPLWASTE
        CREDITCLAIM
        RECALL
        CREATERECALL
        COMPANYHORECALL
        EXCESSSALESPLAN
        PLANNERLEAVER
        'Recalls CR
        CUSTOMEREMERGENCY
        WITHDRAWN
        RECALLRETURNS

    End Enum
End Class


