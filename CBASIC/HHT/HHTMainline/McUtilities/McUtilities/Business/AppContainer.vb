Imports System.Threading
Imports System.Diagnostics
''' <summary>
''' This is the Applicaiton Container Class.
''' This class initialises the generic application paramaeters and brings up the business modules.
''' </summary>
''' <remarks></remarks>
'''
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Check added to verify whether MCF is enabled or not
''' </Summary>
'''****************************************************************************
Public Class AppContainer
    'Create all genereic objects here. 
    ''' <summary>
    ''' Instance of Utilities main menu
    ''' </summary>
    ''' <remarks></remarks>
    Private objUtilitiesMenu As frmUtilitiesMenu = Nothing
    'Decalre variables for storing the user information
    ''' <summary>
    ''' Instance of Logger class to write log information.
    ''' </summary>
    ''' <remarks></remarks>
    Public objLogger As Logger
    ''' <summary>
    ''' Clas responsible for uploading log file to controller.
    ''' </summary>
    ''' <remarks></remarks>
    Public objLogMessageTransmitter As LOGTransmitter
    ''' <summary>
    ''' Object to access configuration file parameters.
    ''' </summary>
    ''' <remarks></remarks>
    Public objConfigFileParams As ConfigParams
    ''' <summary>
    ''' Object to access IP configuration file parameters.
    ''' </summary>
    ''' <remarks></remarks>
    Public objIPConfigFileParams As IPParams
    ''' <summary>
    ''' Variable to set the location of controls. New MC55 device has double
    ''' the resolution of MC70. Set this variable to dynamically set the location.
    ''' </summary>
    ''' <remarks></remarks>
    Public iOffset As Integer = 1
    ''' <summary>
    ''' MCF v1.1 Variable to set the IP Address of the active controller: Implemented as part of MCF Phase 1
    ''' </summary>
    ''' <remarks></remarks>
    Public strActiveIP As String = Nothing
    'MCF v1.1 Declare variable for whether mcf enabled.
    Public bMCFEnabled As Boolean = False
    'MCF v1.1 Declare variable whether connected to alternate IP
    Public bConnectedToAlternate As Boolean = False
    ''' <summary>
    ''' Application initialisation.
    ''' Most of this will be performed when the Splash screen is displayed at application startup.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub AppInitialise()
        Try
            'Do all Initialisations here
            objConfigFileParams = ConfigParser.GetInstance().GetConfigParams()
            ConfigDataMgr.GetInstance.LoadIPConfig()
            'MCF Change
            strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.ACTIVE_IPADDRESS).ToString()
            If Not ConfigDataMgr.GetInstance.GetIPParam _
                   (ConfigKey.PRIMARY_IPADDRESS).ToString() = ConfigDataMgr.GetInstance.GetIPParam _
                                                            (ConfigKey.SECONDARY_IPADDRESS).ToString() Then
                bMCFEnabled = True
            End If
            objLogger = New Logger()
            objLogMessageTransmitter = New LOGTransmitter()
            'Check if the device is mc55 then set offset to 2
            If System.Environment.OSVersion.Version.ToString.StartsWith("5.2.") Then
                iOffset = 2
            End If
            objUtilitiesMenu = New frmUtilitiesMenu()
            objUtilitiesMenu.ShowDialog()
            objConfigFileParams = Nothing
            objLogger = Nothing
            objLogMessageTransmitter = Nothing

            'TerminateSplash()
            AppTerminate()
        Catch ex As Exception
            MessageBox.Show("Cannot Initialise Application | " & Err.Description & " : " & Err.Number, "Exception", _
                                       MessageBoxButtons.OK, _
                                       MessageBoxIcon.Exclamation, _
                                       MessageBoxDefaultButton.Button1)
            objConfigFileParams = Nothing
            objLogger = Nothing
            objLogMessageTransmitter = Nothing
            'Exit Applicaiton if Initialisation fails.
            Application.Exit()
        End Try
    End Sub
    ''' <summary>
    ''' Sub Routine to perform all operations while the application is terminated.
    ''' AppTerminate will release all objects created by the Container, dispose al forms
    ''' and gracefully kill the application.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub AppTerminate()
        'Perform all Terminate functions here
        Try
            objUtilitiesMenu = Nothing
            objAppContainer = Nothing
            Application.Exit()
        Catch ex As Exception
            'Handle Application Terminate Exception here
            Application.Exit()
            Return
        End Try
    End Sub
End Class
