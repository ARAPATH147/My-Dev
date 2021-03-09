'''***************************************************************
''' <FileName>SSSessionMgr.vb</FileName>
''' <summary>
''' The Store Sales Container Class.
''' Implements all business logic and GUI navigation for Store Sales.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>19-Nov-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
''' 
#If RF Then
Public Class SSSessionManager
    Implements ISSInterface
#Region "Variables"
    Private Shared m_SSSessionMgr As SSSessionManager
    Private m_SShome As frmSSSummary
    Private m_StoreSalesInfo As StoreSalesInfo
#End Region
#Region "Constructor"

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
    End Sub
#End Region
#Region "Methods"
    ''' <summary>
    ''' Updates the Status bar of all the forms in the session manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBarMessage()
        Try
            m_SShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured, Trace: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Initialises the Item Sales session when invoked from info tab of 
    ''' Shelf Management main menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Function StartSession() As Boolean Implements ISMInterface.StartSession
        objAppContainer.objLogger.WriteAppLog("Enter Store Sales Start session", Logger.LogLevel.INFO)
#If RF Then
        Dim objData As Object = Nothing
        If Not (objAppContainer.objExportDataManager.CreateINS()) Then
            'If (DATAPOOL.getInstance.GetNextObject(objData)) Then
            objAppContainer.objLogger.WriteAppLog("Cannot Create INS record at Store Sales Start Session", Logger.LogLevel.INFO)
            Return False
            'End If
        End If
#End If
        Try
            'Initialisation of all Item sales details
            m_SShome = New frmSSSummary()
            m_StoreSalesInfo = New StoreSalesInfo
            Me.DisplaySSScreen(SSSCREENS.Home)
            Return True
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured at Store Sales Start Session: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Store Sales Start Session", Logger.LogLevel.INFO)

    End Function
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by StoreSalesSessionMgr.
    ''' </summary>
    ''' <remarks></remarks>
#If NRF Then
     Public Function EndSession() As Boolean 
#ElseIf RF Then
    Public Function EndSession(Optional ByVal isConnectionLost As Boolean = False) As Boolean
#End If
        objAppContainer.objLogger.WriteAppLog("Enter Store Sales End Session", Logger.LogLevel.INFO)
        Try
#If RF Then
            If Not isConnectionLost Then
                If Not objAppContainer.objExportDataManager.CreateINX() Then
                    objAppContainer.objLogger.WriteAppLog("Cannot create INX record in Itemsales End Session", Logger.LogLevel.RELEASE)
                    Return False
                End If
            End If
#End If
            m_SShome.Dispose()
            m_SSSessionMgr = Nothing
            m_StoreSalesInfo = Nothing
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at Store Sales End Session", Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Store Sales End Session", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Functions for getting the object instance for the StoreSalesSessionMgr. 
    ''' Use this method to get the object reference for the Singleton ItemSalesSessionMgr.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of StoreSalesSessionMgr Class</remarks>
    Public Shared Function GetInstance() As SSSessionManager
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.STORESALES
        If m_SSSessionMgr Is Nothing Then
            m_SSSessionMgr = New SSSessionManager()
            Return m_SSSessionMgr
        Else
            Return m_SSSessionMgr
        End If
    End Function
    ''' <summary>
    ''' The method Displays the Store Sales Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub DisplayHomeScreen(ByVal o As Object, ByVal e As System.EventArgs) Implements ISMInterface.DisplayHomeScreen
        GetStoreSalesInfo(m_StoreSalesInfo.Today, m_StoreSalesInfo.ThisWeek)
        With m_SShome
            '.lblThisWeekValue.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.CURRENCY_KEY) + " " + objAppContainer.objHelper.FormatSalesValue(m_StoreSalesInfo.ThisWeek)
            '.lblTodayValue.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.CURRENCY_KEY) + " " + objAppContainer.objHelper.FormatSalesValue(m_StoreSalesInfo.Today)

            'Fix for displaying the value along with decimal
            .lblThisWeekValue.Text = objAppContainer.objHelper.GetCurrency() + " " + Helper.FormatMoney(m_StoreSalesInfo.ThisWeek)
            .lblTodayValue.Text = objAppContainer.objHelper.GetCurrency() + " " + Helper.FormatMoney(m_StoreSalesInfo.Today)
            'Fix for Status Bar  display "Connected" for RF Mode
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' Screen display method for Store Sales. 
    ''' All Store Sales sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName">Enum ISSCREENS</param>
    ''' <remarks></remarks>
    Public Sub DisplaySSScreen(ByVal ScreenName As SSSCREENS) Implements ISSInterface.DisplaySSScreen
        objAppContainer.objLogger.WriteAppLog("Enter Store Sales Display Screen", Logger.LogLevel.INFO)
        Try
            Select Case ScreenName
                Case SSSCREENS.Home
                    m_SShome.Invoke(New EventHandler(AddressOf DisplayHomeScreen))
            End Select
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("Exception occured Store Sales Display Screen: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Store Sales Display Screen", Logger.LogLevel.INFO)
        Return
    End Sub
    ''' <summary>
    ''' To fetch the Store Sales information 
    ''' </summary>
    ''' <param name="strToday"></param>
    ''' <param name="strWeeks"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetStoreSalesInfo(ByRef strToday As String, ByRef strWeeks As String) As Boolean Implements ISSInterface.GetStoreSalesInfo
        Try
            objAppContainer.objDataEngine.GetStoreSalesInfo(m_StoreSalesInfo)
            strToday = m_StoreSalesInfo.Today
            strWeeks = m_StoreSalesInfo.ThisWeek
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
#End Region
End Class
''' <summary>
''' The value class for getting and managing Store Sales Details.
''' </summary>
''' <remarks></remarks>
Public Class StoreSalesInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
#Region "Variables"
    Private m_Today As String
    Private m_ThisWeek As String
#End Region
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
#Region "constructor"
    Sub New()
    End Sub
#End Region
#Region "Methods"
    ''' <summary>
    ''' Gets or sets the value of Store Sales today
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    Public Property Today() As String
        Get
            Return m_Today
        End Get
        Set(ByVal value As String)
            m_Today = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the value of Store Sales this week
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    Public Property ThisWeek() As String
        Get
            Return m_ThisWeek
        End Get
        Set(ByVal value As String)
            m_ThisWeek = value
        End Set
    End Property
#End Region
End Class

#End If