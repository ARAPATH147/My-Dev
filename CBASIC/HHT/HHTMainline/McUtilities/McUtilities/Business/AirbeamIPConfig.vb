Imports System.Net

''' <summary>
''' MCF v1.1    Kiran Sajeev    Jan 2013
''' This new class implements the Airbeam IP configuration module in the utilities application. 
''' The Airbeam IP is stored in the Airbeam registry which is lookedup by the Airbeam client
''' while synchronizing the application image with the controller.
''' </summary>

Public Class AirbeamIPConfig

    ''' Member variables.
    Private m_AirbeamIPConfigInfo As frmIPConfigure
    Private Shared m_AirbeamIPConfig As AirbeamIPConfig = Nothing

    ''' To get the instance of the AirbeamIPConfig class
    Public Shared Function GetInstance() As AirbeamIPConfig
        If m_AirbeamIPConfig Is Nothing Then
            m_AirbeamIPConfig = New AirbeamIPConfig()
            Return m_AirbeamIPConfig
        Else
            Return m_AirbeamIPConfig
        End If
    End Function
    ''' <summary>
    ''' To start the session and intialise all the variables 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartSession()
        objAppContainer.objLogger.WriteAppLog("Initialising AirbeamIPConfig", Logger.LogLevel.INFO)
        m_AirbeamIPConfigInfo = New frmIPConfigure()
        DisplayScreen(AIRBEAM_IP_CONFIG_SCREENS.Home)
    End Sub
    ''' <summary>
    ''' To end the session and release all the objects held by the AirbeamIPConfig
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EndSession()
        Try
            m_AirbeamIPConfigInfo.Close()
            m_AirbeamIPConfigInfo.Dispose()
            m_AirbeamIPConfig = Nothing
            objAppContainer.objLogger.WriteAppLog("Exited from AirbeamIPConfig", Logger.LogLevel.INFO)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception while EndSession in AirbeamIPConfig" + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try

    End Sub
    ''' <summary>
    ''' To display the IP value in the label
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayAirbeamConfigInfo()
        Dim objUtils As Utility = New Utility()
        With m_AirbeamIPConfigInfo
            .lblPrimaryIP.Text = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString()
            .lblSecondaryIp.Text = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.SECONDARY_IPADDRESS).ToString()
            .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' Screen Display method 
    ''' All Count List sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName"></param>
    ''' <returns>True if success</returns>
    Public Function DisplayScreen(ByVal ScreenName As IP_SCREENS)
        Try
            Select Case ScreenName
                Case AIRBEAM_IP_CONFIG_SCREENS.Home
                    'Invoke method to display the home screen
                    m_AirbeamIPConfigInfo.Invoke(New EventHandler(AddressOf DisplayAirbeamConfigInfo))
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in DisplayScreen: " + ex.Message.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
End Class
Public Enum AIRBEAM_IP_CONFIG_SCREENS
    Home
End Enum

