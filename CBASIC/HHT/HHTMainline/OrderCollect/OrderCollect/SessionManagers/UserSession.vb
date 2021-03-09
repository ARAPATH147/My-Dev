'''****************************************************************************
''' <FileName> UserSession.vb </FileName> 
''' <summary> Session manager for handling user signon</summary> 
''' <Version>1.0</Version> 
''' <Author>Andrew Paton</Author> 
''' <DateModified>11-05-2016</DateModified> 
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC55RF</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK </CopyRight>
'''****************************************************************************
'''* Modification Log 
'''**************************************************************************** 
'''  1.0    Andrew Paton                             11/05/2016        
'''         Inital Version.
''' 
'''**************************************************************************** 

Public Class UserSession

    Public Shared mfrmUserAuthHome As frmUserLogon
    Private Shared moUserSession As UserSession
    'Public mUserInfo As New UserInfo
    'Public mSNRMessage As New SNRMessage

    ''' <summary>
    ''' The shared function GetInstance will implement a check for the instantiation
    ''' of class UserSession to make sure that the class has only one instance
    ''' </summary>
    Public Shared Function GetInstance() As UserSession
        ' Get the user auth active module 
        oAppMain.enActiveModule = AppMain.ACTIVEMODULE.USERAUTH
        If moUserSession Is Nothing Then
            moUserSession = New UserSession()
            Return moUserSession
        Else
            Return moUserSession
        End If
    End Function

    ''' <summary>
    ''' To intialise the local variables
    ''' </summary>
    Private Sub New()
        Try
            mfrmUserAuthHome = New frmUserLogon
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Launches the User Logon form to the user
    ''' </summary>
    Public Sub LaunchUser()
        ' Make your form  visible
        mfrmUserAuthHome.Visible = True
        ' Display the User Authenticatioh form 
        mfrmUserAuthHome.ShowDialog()
    End Sub

    ''' <summary>
    ''' To end the session and release all the objects held by the UserSessionManager
    ''' </summary>
    Public Sub EndSession()
        Try
            mfrmUserAuthHome.Close()
            mfrmUserAuthHome.Dispose()
            moUserSession = Nothing
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' To validate the user credentials login to the device
    ''' </summary>
    ''' <param name="cUserEnteredText">3-digit User ID and 3 digit Password</param>
    ''' <returns>Boolean</returns>
    Public Function ValidateUser(ByVal cUserEnteredText As String) As Boolean
        Try
            If cUserEnteredText.Length < 6 Then
                'Please enter 3 digit User Id and 3 digit Password.
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M1"), "ALERT", _
                                                                MessageBoxButtons.OK, _
                                                          MessageBoxIcon.Exclamation, _
                                                        MessageBoxDefaultButton.Button1)

                mfrmUserAuthHome.txtSignOn.Text = ""
                Return False

            End If

            ' Call the Data manger class for validating the user id received
            If RFDataManager.GetInstance.GetUserDetails(cUserEnteredText) Then

                oAppMain.cCurrentUserID = cUserEnteredText.Substring(0, 3)
                UpdateUserDetails()
                oAppMain.bUserSession = False
                Return True

            Else
                oAppMain.bReconnectSuccess = False
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' To update user details in the config file
    ''' </summary>
    Private Function UpdateUserDetails() As Boolean
        Try
            ConfigFileManager.GetInstance.SetParam(ConfigKey.PREVIOUS_USER, _
                                      oAppMain.cCurrentUserID.Substring(0, 3))
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

End Class
