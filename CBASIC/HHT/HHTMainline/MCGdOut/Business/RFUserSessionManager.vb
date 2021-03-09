
#If RF Then
Imports Microsoft.VisualBasic
Imports System.Net.Sockets
Imports System.Text
Imports System.Runtime.InteropServices
Imports MCGdOut.FileIO
Imports System.IO
Imports MCGdOut.GOExportDataManager

Public Class RFUserSessionManager
    ''' <summary>
    ''' Member variables.
    ''' </summary>
    ''' <remarks>Declaration of variables to be used in this class</remarks>
    Private m_UserAuthHome As frmUserAuthentication
    Private Shared m_RFUserSessionMgr As RFUserSessionManager = Nothing
    Private strTransactMessage As New System.Text.StringBuilder
    'Instantiate the UserInfo class
    Private objUserInfo As New UserInfo()
    ''' <summary>
    ''' To intialise the local variables
    ''' </summary>
    ''' <remarks>Intialize the session</remarks>
    Private Sub New()
        Try
            Me.StartSession()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("RFUserSessionMgr: Exception in UserSessionManager" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            Me.EndSession()
        End Try
    End Sub
    ''' <summary>
    ''' To get the instance of the UserSessionManager class
    ''' </summary>
    ''' <remarks>Instantiate the class</remarks>
    Public Shared Function GetInstance() As RFUserSessionManager
        ' Get the user auth active module 
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.USERAUTH
        If m_RFUserSessionMgr Is Nothing Then
            m_RFUserSessionMgr = New RFUserSessionManager()
            Return m_RFUserSessionMgr
        Else
            Return m_RFUserSessionMgr
        End If
    End Function
    ''' <summary>
    ''' To start the session and intialise all the variables 
    ''' </summary>
    ''' <remarks>Instantiate the User Authentication form</remarks>
    Public Sub StartSession()
        ' Instantiate the form User Authentication 
        m_UserAuthHome = New frmUserAuthentication()
    End Sub
    ''' <summary>
    ''' To end the session and release all the objects held by the 
    ''' UserSessionManager
    ''' </summary>
    ''' <remarks>Close and dispose all the objects used</remarks>
    Public Function EndSession() As Boolean
        Try
            m_UserAuthHome.Close()
            m_UserAuthHome.Dispose()
            m_RFUserSessionMgr = Nothing
            objUserInfo = Nothing
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("RFUserSessionMgr: Exception in EndSession" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function

    Public Sub UpdateStatusBar()
        Try
            m_UserAuthHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Handle user authentication with various business conditions
    ''' </summary>
    ''' <remarks></remarks>
    Public Function HandleUserAuthentication() As Boolean
        Return True 'anoop
    End Function


    ''' <summary>
    ''' To validate the user credentials login to the device
    ''' </summary>
    ''' <param name="strUserEnteredText"></param>
    ''' <remarks>The user entered text in the form</remarks>
    Public Function ValidateUser(ByVal strUserEnteredText As String) As Boolean
        'Declare local variable for storing validation 
        Dim bTemp As Boolean = False
        Dim bExData As Boolean = False
        Dim strUserEnteredID As String = Nothing
        'Dim strUserEnteredPwd As String = Nothing

        ' Removed padleft
        'Break the userid and password entered by the user
        strUserEnteredID = Mid(m_UserAuthHome.tbSignOn.Text, 1, 3)
        'strUserEnteredPwd = Mid(m_UserAuthHome.tbSignOn.Text, 4, 6).PadLeft(8, "0")

        'If the user entered userid and password is less than  MAX_PASSWORD do not accept it
        If strUserEnteredText.Length < Macros.MAX_PASSWORD Then
            MessageBox.Show(MessageManager.GetInstance.GetMessage("M26"), _
                            "Alert")
            m_UserAuthHome.tbSignOn.Text = ""
            Return False
        End If

        Try
            ' Call the Data Source class for validating the user id received
            If objAppContainer.objDataEngine.GetUserDetails(strUserEnteredText, objUserInfo) Then
                'Stock File Accuracy - Check for stock specialist
                If Not (objUserInfo.accessFlag = Macros.STOCK_SPECIALIST) Then
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M75"), _
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                MessageBoxDefaultButton.Button1)
                Else
                    objAppContainer.objLogger.WriteAppLog("RFUserSessionMgr: User validated successfully" _
                                                                  & strUserEnteredID, _
                                                              Logger.LogLevel.INFO)
                    'Update the current user's information
                    RFUserSessionManager.GetInstance().UpdateUserDetails() 'anoop

                    bTemp = True 'anoop
                End If
            Else
                'anoop:Changed the message to M27
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M27"), _
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                MessageBoxDefaultButton.Button1)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("RFUserSessionMgr: User validation failure" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' Launches the User Authentication form to the user
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LaunchUser()
        ' Make your form  visible
        m_UserAuthHome.Visible = True
        ' Display the User Authenticatioh form 
        m_UserAuthHome.ShowDialog()
    End Sub

    '''' <summary>
    '''' Handle user authentication with various business conditions
    '''' </summary>
    '''' <remarks></remarks>
    'Public Function HandleUserAuthentication() As Boolean
    '    'Set status bar
    '    m_UserAuthHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)

    '    'Declare local variable 
    '    Dim bTemp As Boolean = False
    '    Dim bIsUserSame As Boolean = False
    '    Dim bIsExDataAvailable As Boolean = False
    '    Dim bWriteIntoExData As Boolean = False
    '    Dim bIsCrashRecovered As Boolean = False
    '    Dim bCheckForActiveFile As Boolean = False
    '    Dim strDeviceIP As String = Nothing

    '    'Once user authentication is successful hide the authentication scree.
    '    m_UserAuthHome.Visible = False
    '    m_UserAuthHome.Refresh()

    'End Function

    ''' <summary>
    ''' To update the App container user details flag and update the config file
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateUserDetails()
        'Declare local variable 
        Dim bTemp As Boolean = False

        'Update the App container user details
        objAppContainer.strCurrentUserID = objUserInfo.user_ID
        If objUserInfo.authorityFlag = "S" Then
            objAppContainer.strSupervisorFlag = "Y"
        Else
            objAppContainer.strSupervisorFlag = "N"
        End If
        objAppContainer.strCurrentUserName = objUserInfo.user_Name
    End Sub

    ''' <summary>
    ''' This function will do condition handling for user log off session 
    ''' from shelf management and for autologoff
    ''' </summary>
    ''' <param name="bIsExportDataDownload"></param>
    ''' <remarks>Is the logout for autologoff or explicit logout</remarks>
    Public Function LogOutSession(ByVal bIsExportDataDownload As Boolean) As Boolean
        'Set the status bar
        Try
            objAppContainer.objLogger.WriteAppLog("Enter LogOutSession", Logger.LogLevel.RELEASE)
            If Not (objAppContainer.objExportDataManager.CreateOFF(False)) Then
                objAppContainer.objLogger.WriteAppLog("Cannot Create OFF record", Logger.LogLevel.RELEASE)
                Return False
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' To check whether the device has been docked into cradle or not
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CheckDeviceDocked() As Boolean
        'Declare local variable
        Dim strDeviceIP As String = Nothing
        Dim bTemp As Boolean = False
        Dim iCount As Integer = 3
        Do
            'Check for device IP if it has been docked 
            strDeviceIP = objAppContainer.objHelper.GetIPAddress()
            If strDeviceIP <> "127.000.000.001" And strDeviceIP <> "" And _
                                        strDeviceIP <> Nothing Then
                bTemp = True
            Else
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M33"), _
                                "Alert", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                MessageBoxDefaultButton.Button1)
                bTemp = False
                iCount = iCount - 1
                Threading.Thread.Sleep(Macros.WAIT_BEFORE_GET_IP)
            End If
        Loop Until bTemp = True Or iCount = 0
        Return bTemp
    End Function
End Class
''' <summary>
''' The class holds the values for the particular store.
''' </summary>
''' <remarks></remarks>
Public Class UserInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Public user_ID As String
    Public authorityFlag As String
    Public dateTime As String
    Public user_Name As String
    Public accessFlag As String
End Class

#End If