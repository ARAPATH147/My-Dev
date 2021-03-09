#If RF Then
Public Class RFUserSessionManager
    ''' <summary>
    ''' Member variables.
    ''' </summary>
    ''' <remarks>Declaration of variables to be used in this class</remarks>
    Private m_UserAuthHome As frmUserAuthentication
    Private Shared m_UserSessionMgr As RFUserSessionManager = Nothing
    Private m_ActBuildTime As String = Nothing
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
            objAppContainer.objLogger.WriteAppLog("UserSessionManager: Exception in UserSessionManager" + _
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
        If m_UserSessionMgr Is Nothing Then
            m_UserSessionMgr = New RFUserSessionManager()
            Return m_UserSessionMgr
        Else
            Return m_UserSessionMgr
        End If
    End Function
    ''' <summary>
    ''' To start the session and intialise all the variables 
    ''' </summary>
    ''' <remarks>Instantiate the User Authentication form</remarks>
    Public Sub StartSession()
        ' Instantiate the form User Authentication 
        m_UserAuthHome = New frmUserAuthentication()
        'Instantiate the object for active file parser.

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
            m_UserSessionMgr = Nothing
            objUserInfo = Nothing
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionManager: Exception in EndSession" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
    Public Sub UpdateStatusBar()
        Try
            m_UserAuthHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured while updating the Sign on form status Bar", Logger.LogLevel.RELEASE)
        End Try
    End Sub
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

        'Break the userid and password entered by the user
        strUserEnteredID = Mid(m_UserAuthHome.tbSignOn.Text, 1, 3).PadLeft(8, "0")
        'strUserEnteredPwd = Mid(m_UserAuthHome.tbSignOn.Text, 4, 3).PadLeft(8, "0")

        'If the user entered userid and password is less than  MAX_PASSWORD do not accept it
        If strUserEnteredText.Length < Macros.MAX_PASSWORD Then
            MessageBox.Show(MessageManager.GetInstance.GetMessage("M26"), _
                            "Alert")
            m_UserAuthHome.tbSignOn.Text = ""
            Return False
        End If

        If CInt(strUserEnteredID) < 100 Then
            MessageBox.Show(MessageManager.GetInstance.GetMessage("M27"), _
                                         "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                         MessageBoxDefaultButton.Button1)
            Return False

        End If

        Try
            ' Call the Data Source class for validating the user id received
            If objAppContainer.objDataEngine.GetUserDetails(strUserEnteredText, objUserInfo) Then
                objAppContainer.objLogger.WriteAppLog("RFUserSessionMgr: User validated successfully" _
                                                              & strUserEnteredID, _
                                                          Logger.LogLevel.INFO)
                'Update the current user's information
                RFUserSessionManager.GetInstance().UpdateUserDetails() 'anoop

                bTemp = True 'anoop

            Else
                'anoop:Changed the message to M27
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M27"), _
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                MessageBoxDefaultButton.Button1)
            End If
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr: User validation failure" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
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
    ''' <summary>
    ''' Handle user authentication with various business conditions
    ''' </summary>
    ''' <remarks></remarks>
    Public Function HandleUserAuthentication() As Boolean
        Return True 'anoop
    End Function
    ''' <summary>
    ''' To update the App container user details flag and update the config file
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateUserDetails()
        Dim strPrinterDesc As String = ""
        Dim strPrintNum As String = ""
        'Update the App container user details
        'anoop
        objAppContainer.strCurrentUserID = objUserInfo.user_ID
        If objUserInfo.authorityFlag = Macros.SNR_SUPERVISOR_TAG Then
            objAppContainer.strSupervisorFlag = Macros.SUPERVISOR_YES
        Else
            objAppContainer.strSupervisorFlag = Macros.SUPERVISOR_NO
        End If
        objAppContainer.OSSRStoreFlag = objUserInfo.ossrFlag
        strPrinterDesc = objUserInfo.prtDesc.Trim().Replace(vbCrLf, ",")
        strPrinterDesc = strPrinterDesc.Replace(vbCr, "")
        strPrinterDesc = strPrinterDesc.Replace(vbLf, "")
        objAppContainer.aPrinterList = strPrinterDesc.Split(",")
        objAppContainer.aPrintNos = objUserInfo.prtNum.ToCharArray()
        'FIX for PRINT NUMBER ASSIGN
        Dim iPrinterCount As Integer = 0
        While iPrinterCount < 10
            If objUserInfo.prtNum.Substring(iPrinterCount, 1) = "0" Then
                objAppContainer.aPrintNos(iPrinterCount) = iPrinterCount.ToString()
            End If
            iPrinterCount += 1
        End While
        objAppContainer.strCurrentUserName = objUserInfo.user_Name
        'Stock File Accuracy -Set stock specialist varible to true if stock access flag is "Y"
        If objUserInfo.stockAccess = "Y" Then
            objAppContainer.bIsStockSpecialist = True
        End If
    End Sub
    ''' <summary>
    ''' This function will do condition handling for user log off session 
    ''' from shelf management and for autologoff
    ''' </summary>
    ''' <param name="bIsExportDataDownload"></param>
    ''' <remarks>Is the logout for autologoff or explicit logout</remarks>
    Public Function LogOutSession(ByVal bIsExportDataDownload As Boolean) As Boolean
        If Not (objAppContainer.objExportDataManager.CreateOFF(False)) Then
            objAppContainer.objLogger.WriteAppLog("Cannot Create OFF record", Logger.LogLevel.RELEASE)
            Return False
        End If
        Return True


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
'anoop:Added new userInfo class
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
    Public prtNum As String
    Public prtDesc As String
    Public ossrFlag As String
    Public stockAccess As String

End Class
'anoop
#End If
