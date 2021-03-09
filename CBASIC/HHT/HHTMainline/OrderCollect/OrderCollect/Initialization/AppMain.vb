Imports System.Threading

'''****************************************************************************
''' <FileName> AppMain.vb </FileName> 
''' <summary> Main Application Class Module </summary> 
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

Public Class AppMain

    'Screen variables
    Public frmAppMenu As frmMainMenu
    Public frmLoadScreen As frmSplashScreen
    Public Shared frmCalculator As frmCalcPad

    'Class variables
    Public oMsgManager As MessageManager = Nothing
    Public oConfigManager As ConfigFileManager = Nothing

    Public iOffSet As Integer = 1
    'Public UserInfo As New UserSession.UserInfo

    'Enum Variables
    Public enActiveModule As ACTIVEMODULE
    Public enActiveScreen As ACTIVESCREEN
    Public mSNRMessage As New SNRMessage

    'Variables used for network connection
    Public cActiveIP As String = Nothing
    Public bConnect As Boolean = False
    Public bMCFEnabled As Boolean = False
    Public iConnectedToAlternate As Integer = 0

    'User information variables
    Public cCurrentUserID As String = Nothing
    Public bUserSession As Boolean = False

    'Connection Status
    Public bReconnectSuccess As Boolean = False
    Public bCommFailure As Boolean = False
    Public bTimeOut As Boolean = False
    Public bRetryAtTimeout As Boolean = False

    ''' <summary>
    ''' Application initialisation.
    ''' Most of this will be performed when the Splash screen is displayed at application startup.
    ''' </summary>
    Public Sub AppInitialise()
        Try
            oConfigManager = ConfigFileManager.GetInstance()
            oMsgManager = MessageManager.GetInstance()
            frmCalculator = New frmCalcPad

            cActiveIP = ConfigFileManager.GetInstance.GetParam(ConfigKey.ACTIVE_IPADDRESS)

            Dim thrSplashScreen As New Thread(AddressOf displaySplash)
            thrSplashScreen.Start()
            Thread.Sleep(1000)

            If Not ConfigFileManager.GetInstance.GetParam(ConfigKey.PRIMARY_IPADDRESS).ToString() = _
                      ConfigFileManager.GetInstance.GetParam(ConfigKey.SECONDARY_IPADDRESS).ToString() Then
                bMCFEnabled = True
            End If

            ConnectionManager.GetInstance()

            If bConnect Then

                UserSession.GetInstance.LaunchUser()
                UserSession.GetInstance.EndSession()

                If cCurrentUserID <> Nothing Then

                    'If RFDataManager.GetInstance.GetUserDetails(cCurrentUserID, UserInfo) = True Then

                    AddHandler BCReader.GetInstance().evtBCScanned, AddressOf BCReader.GetInstance().EventBCScannedHandler

                    'Check if the OS version in WM6.5 for MC55 device.
                    If Environment.OSVersion.Version.ToString().StartsWith("5.2.") Then
                        'Set barcode reader to user the laser bar beam rather than using image.
                        iOffSet = 2
                    End If

                    frmAppMenu = New frmMainMenu()
                    frmAppMenu.ShowDialog()

                    'End If
                End If
            Else
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Error", _
                                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If
            AppTerminate()
        Catch ex As Exception
            MessageBox.Show("Cannot Initialise Application | " & Err.Description & " : " & Err.Number, "Exception", _
                                       MessageBoxButtons.OK, _
                                       MessageBoxIcon.Exclamation, _
                                       MessageBoxDefaultButton.Button1)
            'Exit Applicaiton if Initialisation fails.
            Application.Exit()
        End Try
    End Sub

    ''' <summary>
    ''' Sub Routine to perform all operations while the application is terminated.
    ''' AppTerminate will release all objects created by the Container, dispose 
    ''' all forms and gracefully kill the application.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub AppTerminate()
        'Perform all Terminate functions here

        Try
            'frmMainMenu = Nothing
            ConnectionManager.GetInstance.CloseSession()
            oMsgManager.CloseSession()
            frmLoadScreen = Nothing
            frmSplashScreen = Nothing
            oMsgManager = Nothing
            oConfigManager = Nothing
        Catch ex As Exception
            'Handle Application Terminate Exception here
            Application.Exit()
            Return
        End Try
    End Sub

    ''' <summary>
    ''' Display the CalcPadScreen
    ''' </summary>
    Public Shared Sub displayCalcPadScreen(ByVal callType As CALCPADUSE)
        frmCalculator.setMaxLength(callType)
        frmCalculator.Visible = True
    End Sub

    ''' <summary>
    ''' Display the splash screen while the application loads
    ''' </summary>
    Private Sub displaySplash()
        frmLoadScreen = New frmSplashScreen
        With frmLoadScreen
            .SetScreen()
        End With
        Application.Run(frmLoadScreen)
    End Sub

    Public Enum ACTIVEMODULE
        CALCPAD
        BOOKINORDER
        BOOKINWITHLOCATION
        MOVEPUTAWAY
        QUERYCOLLECT
        USERAUTH
    End Enum

    Public Enum ACTIVESCREEN
        BOOKINPARCEL
        QUERYPARCEL
        SELECTLOCATION
    End Enum

    Public Enum CALCPADUSE
        ASNBARCODE
        LOCATIONBARCODE
        PARCELORDERNUMBER
    End Enum

    Public Structure SNRMessage
        Public cUserID As String
        Public cAuthorityFlag As Char
        Public cUserName As String
        Public cDateTime As String
        Public cPrtNum As String
        Public cPrtDesc As String
        Public cUserPassword As String
    End Structure

End Class

