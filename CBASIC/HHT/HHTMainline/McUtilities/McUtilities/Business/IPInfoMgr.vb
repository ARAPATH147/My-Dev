Imports System.Net

Public Class IPInfoMgr

    ''' <summary>
    ''' Member variables.
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Private m_IPInfo As frmIPData
    Private objIPInfo As IPInfo
    Private Shared m_IPInfoMgr As IPInfoMgr = Nothing

    ''' <summary>
    ''' To intialise the local variables
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()

    End Sub
    ''' <summary>
    ''' To get the instance of the IPInfoMgr class
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As IPInfoMgr
        If m_IPInfoMgr Is Nothing Then
            m_IPInfoMgr = New IPInfoMgr()
            Return m_IPInfoMgr
        Else
            Return m_IPInfoMgr
        End If
    End Function
    ''' <summary>
    ''' To start the session and intialise all the variables 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartSession()
        objAppContainer.objLogger.WriteAppLog("Initialising IPInfoMgr", Logger.LogLevel.INFO)
        m_IPInfo = New frmIPData()
        objIPInfo = New IPInfo()
        GetIP()
        DisplayScreen(IP_SCREENS.Home)
    End Sub
    ''' <summary>
    ''' To end the session and release all the objects held by the IPInfoMgr
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EndSession()
        Try
            m_IPInfo.Close()
            m_IPInfo.Dispose()
            m_IPInfoMgr = Nothing
            objIPInfo = Nothing
            objAppContainer.objLogger.WriteAppLog("Exited from IPInfoMgr", Logger.LogLevel.INFO)
        Catch ex As Exception

        End Try

    End Sub
    Private Function GetIP() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered Get IP", Logger.LogLevel.INFO)
        Try
            Dim strHostName = Dns.GetHostName()

            'Then using host name, get the IP address list..
            Dim ipEntry As IPHostEntry = Dns.GetHostEntry(strHostName)
            Dim ipAddr As IPAddress() = ipEntry.AddressList

            Dim iCounter As Integer = 0
            For iCounter = 0 To ipAddr.Length - 1

                objIPInfo.IPAddress = ipAddr(iCounter).ToString()

            Next
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Function
    Public Sub DisplayIPInfo()
        Dim objUtils As Utility = New Utility()
        With m_IPInfo
            .lblIPVal.Text = objIPInfo.IPAddress
            .lblSerialNumber.Text = objUtils.GetSerialNumber(False)
            .lblMACAddr.Text = Utility.GetMacAddress()
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
    ''' <remarks></remarks>
    Public Function DisplayScreen(ByVal ScreenName As IP_SCREENS)
        objAppContainer.objLogger.WriteAppLog("Entered Get IP", Logger.LogLevel.INFO)

        Try
            Select Case ScreenName
                Case IP_SCREENS.Home
                    'Invoke method to display the home screen
                    m_IPInfo.Invoke(New EventHandler(AddressOf DisplayIPInfo))
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try

        Return True

    End Function

End Class
Public Enum IP_SCREENS
    Home
End Enum
Public Class IPInfo
    Public IPAddress As String
End Class
