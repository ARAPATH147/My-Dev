
Imports System.Runtime.InteropServices
Imports Microsoft.Win32

#If RF Then
'''***************************************************************
''' <FileName>ReportsSessionManager.vb</FileName>
''' <summary>
''' The Reports Container Class.
''' Implements all business logic and GUI navigation for Reports.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>26-Nov-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
Public Class ReportsSessionManager
    Implements IReports

#Region "Variable Initialization"
    Private Shared m_ReportsSessionMgr As ReportsSessionManager
    Private m_ReportsHome As frmReportList
    Private m_NoReports As frmNoReports
    Private m_ReportDetails As frmReportDetails
    Private m_ReportId As String
    Private m_SeqNum As Hashtable
    Public objReportListArray As New System.Collections.ArrayList
#End Region

#Region "Functions"
    ''' <summary>
    ''' To display the details screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub DisplayDetailsScreen(ByVal o As Object, ByVal e As System.EventArgs) Implements IReports.DisplayDetailsScreen
        Dim arrReportHeaders As New System.Collections.ArrayList
        Dim Header As New TreeNode
        Dim Child As TreeNode
        Try
            'To fetch the Report Headers
            GetReportHeaders(m_ReportId, arrReportHeaders)
            If arrReportHeaders.Count > 0 Then
                With m_ReportDetails
                    .tvReports.Nodes.Clear()
                    .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                    For Each objRLD As RLDRecord In arrReportHeaders
                        For Each objRLDinnerRecord As RLDInnerStructure In objRLD.arrRLDInnerStructure
                            While m_SeqNum.Contains(objRLDinnerRecord.strData)
                                objRLDinnerRecord.strData = objRLDinnerRecord.strData + " "
                            End While

                            Header = .tvReports.Nodes.Add(objRLDinnerRecord.strData)
                            'Adding empty child node
                            Child = Header.Nodes.Add(" ")

                            m_SeqNum.Add(objRLDinnerRecord.strData, objRLDinnerRecord.strSequence)
                        Next
                    Next
                    .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                    .Visible = True
                    m_ReportsHome.Visible = False
                    '.Text = m_ReportId
                    .Refresh()
                End With
            Else
                'System Testing bug fix.
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M79"), "Info", _
                          MessageBoxButtons.OK, _
                          MessageBoxIcon.Asterisk, _
                          MessageBoxDefaultButton.Button1)
                Me.DisplayReportScreen(REPORTSCREENS.Home)
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception in DisplayDetailsScreen" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Screen display method for Reports. 
    ''' All Reports sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName"></param>
    ''' <remarks></remarks>
    Public Sub DisplayReportScreen(ByVal ScreenName As REPORTSCREENS) Implements IReports.DisplayReportScreen
        objAppContainer.objLogger.WriteAppLog("Enter Reports Display Report Screen", Logger.LogLevel.INFO)
        Try
            Select Case ScreenName
                Case REPORTSCREENS.Home
                    m_ReportsHome.Invoke(New EventHandler(AddressOf DisplayHomeScreen))
                Case REPORTSCREENS.NoReports
                    m_NoReports.Invoke(New EventHandler(AddressOf DisplayNoReports))
                Case REPORTSCREENS.DetailsScreen
                    m_ReportDetails.Invoke(New EventHandler(AddressOf DisplayDetailsScreen))
            End Select
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured Reports Display Screen: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Reports Display Screen", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Method to display NoReports Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub DisplayNoReports(ByVal o As Object, ByVal e As System.EventArgs) Implements IReports.DisplayNoReports
        With m_NoReports
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' Method to get the Details of the report
    ''' </summary>
    ''' <param name="strReportID"></param>
    ''' <param name="arrReportDetails"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetReportDetails(ByVal strReportID As String, ByVal strSequenceNum As String, ByRef arrReportDetails As System.Collections.ArrayList) As Boolean Implements IReports.GetReportDetails
        Try
            objAppContainer.objDataEngine.GetReportDetails(strReportID, strSequenceNum, arrReportDetails)
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Method to get the report headers
    ''' </summary>
    ''' <param name="strReportID"></param>
    ''' <param name="arrReportHeaders"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetReportHeaders(ByVal strReportID As String, ByRef arrReportHeaders As System.Collections.ArrayList) As Boolean Implements IReports.GetReportHeaders
        Try
            objAppContainer.objDataEngine.GetReportHeaders(strReportID, arrReportHeaders)
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Method to get the list of reports
    ''' </summary>
    ''' <param name="objReports"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetReportList(ByRef arrReports As System.Collections.ArrayList) As Boolean Implements IReports.GetReportList
        Try
            objAppContainer.objDataEngine.GetReportList(arrReports)
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Method to display home screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub DisplayHomeScreen(ByVal o As Object, ByVal e As System.EventArgs) Implements ISMInterface.DisplayHomeScreen
        m_SeqNum = New Hashtable()
        Dim strItem As String = ""
        With m_ReportsHome
            If .lstReports.Items.Count = 0 Then
                For Each objRLR As RLRRecord In objReportListArray
                    strItem = objRLR.strTitle.TrimEnd(" ")
                    .lstReports.Items.Add(New ListViewItem(strItem))
                Next
            End If
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    '''  Gracefully Terminate all forms and objects created by ReportsSessionManager.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
#If NRF Then
        Public Function EndSession()as Boolean 
#ElseIf RF Then
    Public Function EndSession(Optional ByVal isConnectivityLoss As Boolean = False) As Boolean
#End If
        objAppContainer.objLogger.WriteAppLog("Enter Reports End Session", Logger.LogLevel.INFO)
        Try
            If Not isConnectivityLoss Then
                If Not objAppContainer.objExportDataManager.CreateRPX() Then
                    objAppContainer.objLogger.WriteAppLog("Cannot create RPX record in Reports End Session", Logger.LogLevel.RELEASE)
                    Return False
                End If
            End If
            m_NoReports.Dispose()
            m_ReportsHome.Dispose()
            m_ReportDetails.Dispose()
            m_ReportsSessionMgr = Nothing
            m_ReportId = ""
            objReportListArray.Clear()
            objReportListArray = Nothing
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at Reports End Session", Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Reports end session", Logger.LogLevel.INFO)
    End Function
#If RF Then
    ''' <summary>
    ''' Updates the Status bar of all the forms in the session manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBarMessage()
        Try
            m_ReportsHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_NoReports.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_ReportDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured, Trace: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End If
    ''' <summary>
    ''' Initialises the Reports Session when invoked 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function StartSession() As Boolean Implements ISMInterface.StartSession
        objAppContainer.objLogger.WriteAppLog("Enter Reports start session", Logger.LogLevel.INFO)
        Try
            'Initialization of all report details
            If objAppContainer.objExportDataManager.CreateRPO() Then
                m_ReportsHome = New frmReportList()
                m_NoReports = New frmNoReports()
                m_ReportDetails = New frmReportDetails()
                Me.GetReportList(objReportListArray)
                If (objReportListArray.Count = 0) Then
                    Me.DisplayReportScreen(REPORTSCREENS.NoReports)
                Else
                    Me.DisplayReportScreen(REPORTSCREENS.Home)
                End If
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured in Reports module start session")
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Reports start session", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Functions for getting the object instance for the ReportsSessionMgr. 
    ''' Use this method to get the object reference for the Singleton ReportsSalesSessionMgr.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of ReportsSessionMgr Class</remarks>
    Public Shared Function GetInstance() As ReportsSessionManager
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.REPORTS
        If m_ReportsSessionMgr Is Nothing Then
            m_ReportsSessionMgr = New ReportsSessionManager()
            Return m_ReportsSessionMgr
        Else
            Return m_ReportsSessionMgr
        End If
    End Function
    ''' <summary>
    ''' Function to get selected report id and displaying corresponding details
    ''' </summary>
    ''' <param name="ReportID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Details(ByVal strIndex As String)
        Try
            Dim objRLR As RLRRecord
            objRLR = objReportListArray.Item(strIndex)
            m_ReportId = objRLR.strReportId
            Me.DisplayReportScreen(REPORTSCREENS.DetailsScreen)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    ''' <summary>
    ''' To display the child node and its details
    ''' </summary>
    ''' <param name="header"></param>
    ''' <remarks></remarks>
    Public Sub DisplayChildDetails(ByRef header As TreeNode)
        'Dim iDetailsCount As Integer
        Dim Child As New TreeNode
        Dim arrReportDetails As New System.Collections.ArrayList
        Dim iParentLevel As Integer = 0
        Dim Parent As TreeNode = New TreeNode()
        Try
            Parent = Nothing
            Parent = header.Parent
            'iDetailsCount = 0
            If Parent Is Nothing Then
                GetReportDetails(Me.m_ReportId, m_SeqNum(header.Text), arrReportDetails)
                header.Nodes.Clear()
                For Each objRUP As RUPRecord In arrReportDetails
                    For Each objRUPInnerRecord As RUPInnerStructure In objRUP.arrRUPInnerStucture
                        If Val(objRUPInnerRecord.cLevel) > iParentLevel Then
                            Child = header.Nodes.Add(objRUPInnerRecord.strData)
                            iParentLevel = Val(objRUPInnerRecord.cLevel)
                            header = Child
                        ElseIf Val(objRUPInnerRecord.cLevel) = iParentLevel Then
                            header = header.Parent
                            Child = header.Nodes.Add(objRUPInnerRecord.strData)
                            iParentLevel = Val(objRUPInnerRecord.cLevel)
                            header = Child
                        ElseIf Val(objRUPInnerRecord.cLevel) < iParentLevel Then
                            'Add for loop
                            While iParentLevel >= Val(objRUPInnerRecord.cLevel)
                                Child = header
                                header = header.Parent

                                If header Is Nothing And iParentLevel = 1 Then
                                    header = Child
                                End If
                                iParentLevel = iParentLevel - 1
                            End While
                            Child = header.Nodes.Add(objRUPInnerRecord.strData)
                            iParentLevel = Val(objRUPInnerRecord.cLevel)
                            header = Child
                        End If
                        If objRUPInnerRecord.cFunction = "E" Then
                            Child.Expand()
                        End If
                    Next
                Next
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            'Exception message
        End Try

        'While iDetailsCount < arrReportDetails.Count
        '    ParseDetails(arrReportDetails(iDetailsCount).ToString())
        '    Child = header.Nodes.Add(objReportsData.Data)
        '    If (objReportsData.Level > 0) Then
        '        Child.Nodes.Add(" ")
        '    End If
        '    If objReportsData.AutoExpand = "E" Then
        '        Child.Expand()
        '    End If
        '    iDetailsCount = iDetailsCount + 1
        'End While
    End Sub
    'Public Sub ParseDetails(ByVal ReportDetails As String)
    '    Dim iLength As Integer
    '    iLength = ReportDetails.Length
    '    objReportsData.Data = ReportDetails.Substring(0, iLength - 2)
    '    objReportsData.Level = ReportDetails.Substring(iLength - 2, 1)
    '    objReportsData.AutoExpand = ReportDetails.Substring(iLength - 1)
    'End Sub
    ''' <summary>
    ''' For making beep sound
    ''' </summary>
    ''' <param name="szSound"></param>
    ''' <param name="hModule"></param>
    ''' <param name="flags"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    <DllImport("coredll.dll")> _
Public Shared Function PlaySound(ByVal szSound As String, ByVal hModule As IntPtr, ByVal flags As Integer) As Integer
    End Function
    Public Sub Beep()
        Play(ConfigDataMgr.GetInstance().GetParam(ConfigKey.BEEP_PATH).ToString())
    End Sub
    Public Shared Sub Play(ByVal fileName As String)
        Try
            PlaySound(fileName, IntPtr.Zero, CInt(PlaySoundFlags.SND_FILENAME Or PlaySoundFlags.SND_SYNC))
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at Playing Sound:" + ex.StackTrace, Logger.LogLevel.RELEASE)
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
#End Region
End Class
#End If
