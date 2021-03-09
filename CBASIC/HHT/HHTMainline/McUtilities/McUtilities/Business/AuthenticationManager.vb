Imports Microsoft.VisualBasic
Imports System.Net.Sockets
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.IO
''' <summary>
''' This class handles the User Session Manager for User Authentication 
''' </summary>
''' <remarks></remarks>
Public Class AuthenticationManager
    ''' <summary>
    ''' Member variables.
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Private m_UserAuthHome As frmUserAuthentication
    Private Shared m_AuthenticationMgr As AuthenticationManager = Nothing
    'Instantiate the UserInfo class
    Private objUserInfo As New UserInfo()
    ''' <summary>
    ''' To intialise the local variables
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
        Try
            Me.StartSession()
        Catch ex As Exception
            Me.EndSession()
        End Try
    End Sub
    ''' <summary>
    ''' To get the instance of the UserSessionManager class
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As AuthenticationManager
        ' Get the user auth active module 
        If m_AuthenticationMgr Is Nothing Then
            m_AuthenticationMgr = New AuthenticationManager()
            Return m_AuthenticationMgr
        Else
            Return m_AuthenticationMgr
        End If
    End Function
    ''' <summary>
    ''' To start the session and intialise all the variables 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartSession()
        ' Instantiate the form User Authentication 
        m_UserAuthHome = New frmUserAuthentication()

    End Sub
    ''' <summary>
    ''' To end the session and release all the objects held by the UserSessionManager
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EndSession() As Boolean
        Try
            m_UserAuthHome.Close()
            m_UserAuthHome.Dispose()
            m_AuthenticationMgr = Nothing
            objUserInfo = Nothing

        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function
  
    ''' <summary>
    ''' To validate the user credentials login to the device
    ''' </summary>
    ''' <param name="strUserEnteredText"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateUser(ByVal strUserEnteredText As String) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ValidateUser of AuthenticationManager", Logger.LogLevel.INFO)
        'Declare local variable for storing validation 
        Dim bTemp As Boolean = False
        Dim bExData As Boolean = False
        Dim strUserEnteredID As String = Nothing
        Dim strUserEnteredPwd As String = Nothing
        objAppContainer.objLogger.WriteAppLog("Validating User", Logger.LogLevel.RELEASE)
        'Break the userid and password entered by the user
        strUserEnteredID = Mid(strUserEnteredText, 1, 3).PadLeft(8, "0")
        strUserEnteredPwd = Mid(strUserEnteredText, 4, 6).PadLeft(8, "0")

        'If the user entered userid and password is less than  MAX_PASSWORD do not accept it
        If strUserEnteredText.Length < Macros.MAX_PASSWORD Then
            MessageBox.Show("Please enter three digit user ID and Password", "ALERT")
            m_UserAuthHome.tbSignOn.Text = ""
            objAppContainer.objLogger.WriteAppLog("Invalid UserID", Logger.LogLevel.INFO)
            Return False
        End If

        Try
            ' Call the Data Source class for validating the user id received
            If objAppContainer.objDataEngine.GetUserDetails(strUserEnteredID, objUserInfo) Then
                'Assign the retreived values from DB to the User Info 
                If Not objUserInfo Is Nothing Then
                    ' Validate the password entered from the user
                    If (strUserEnteredPwd.Equals(objUserInfo.Password)) Then
                        bTemp = True
                    Else
                        objAppContainer.objLogger.WriteAppLog("Invalid UserID", Logger.LogLevel.INFO)
                        MessageBox.Show("Invalid User ID/Password", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                End If
            Else
                objAppContainer.objLogger.WriteAppLog("Invalid UserID", Logger.LogLevel.INFO)
                MessageBox.Show("Invalid User ID/Password", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting ValidateUser of AuthenticationManager", Logger.LogLevel.INFO)
        Return bTemp
    End Function
End Class

''' <summary>
''' The class holds the values for the User Info who has been validated against the database data
''' </summary>
''' <remarks></remarks>
Public Class UserInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Public user_ID As String
    Public userFlag As String
    Public userPassword As String
    Public user_Name As String
    ''' <summary>
    ''' To set or get User ID of a User.
    ''' </summary>
    ''' <remarks></remarks>
    Public Property UserID() As String
        Get
            Return user_ID
        End Get
        Set(ByVal value As String)
            user_ID = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get the password.
    ''' </summary>
    ''' <remarks></remarks>
    Public Property Password() As String
        Get
            Return userPassword
        End Get
        Set(ByVal value As String)
            userPassword = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get the Supervisor flag.
    ''' </summary>
    ''' <remarks></remarks>
    Public Property SupervisorFlag() As String
        Get
            Return userFlag
        End Get
        Set(ByVal value As String)
            userFlag = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get the User Name
    ''' </summary>
    ''' <remarks></remarks>
    Public Property UserName() As String
        Get
            Return user_Name
        End Get
        Set(ByVal value As String)
            user_Name = value
        End Set
    End Property
End Class








