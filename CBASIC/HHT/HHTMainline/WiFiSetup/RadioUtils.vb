Imports Symbol.Fusion
Imports Symbol.Fusion.WLAN
Imports System.IO
Imports System.Net.NetworkCredential


Public Class RadioUtils
    Implements IDisposable
    'Private objSymbolRadio As Symbol.WirelessLAN.Radio
    'Private objSymbolMACAddress As Symbol.WirelessLAN.MACAddress

    'Private objSymbolRadio As Symbol.Fusion.WLAN.Adapter.SSIDInfo

    ''Private objSymbolMACAddress As Symbol.Fusion.WLAN.NDISInfoBSSID
    ' Private objSymbolMACAddress As Symbol.Fusion.WLAN.Adapter.SSIDInfo


#Region "Disposing"

    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: free other state (managed objects).
            End If

            ' TODO: free your own state (unmanaged objects).
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
#End Region
    '****************************************************************************************
    '* This method will set Registry keys directly into the Registry.
    '* No longer used due to ID/Password being removed on a cold boot.
    '****************************************************************************************
    ''' <summary>
    ''' Creats the Registry Keys in the Directory
    ''' </summary>
    ''' <param name="StoreID">The Four digit store ID entered by the user</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Function CreateRegistryFile(ByVal StoreID As String, ByVal StoreSetting As String)
    Public Function CreateRegistryFile(ByVal StoreID As String)
        Dim bTemp As Boolean = False



        Try
            Dim wlan As New WLAN()

            Dim intProfileCount As Integer
            intProfileCount = wlan.Profiles.Length


            If (intProfileCount = 0) Then

                CreateProfile()

            ElseIf (intProfileCount > 1) Then

                CreateProfile()

            ElseIf (intProfileCount = 1) Then
                If Not wlan.Profiles(0).Name = "BUK-Stores" Then
                    CreateProfile()
                End If
            End If


            wlan.Dispose()


        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message.ToString(), "Error")
        End Try
        Return bTemp
    End Function
    ''' <summary>
    '''  
    ''' </summary>
    ''' <param name="strMACAddress"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function HashMACAddress(ByVal strMACAddress As String) As String
        Dim strHashedPassword As String = ""
        Dim strContainer1 As String = "" ' To hold Two Digit
        Dim strContainer2 As String = "" 'to Hold Two digit value
        For Each PasswordChar In strMACAddress.ToCharArray()
            If (strContainer1.Length >= 2) Then
                If (strContainer2.Length >= 2) Then
                    'Both Containers are full, so dump in the reverse order to the Hashed password
                    ' the below line commented  by Anil since we are providing the password more hashed
                    'strHashedPassword += strContainer2 + strContainer1
                    strContainer2 = strContainer2.Substring(0, 2)
                    strContainer1 = strContainer1.Substring(0, 1)
                    strHashedPassword += "0#" + strContainer2 + "&%" + strContainer1
                    strContainer1 = ""
                    strContainer1 = PasswordChar    'substitute the value read from array in this iteration.
                    strContainer2 = ""
                Else
                    strContainer2 += PasswordChar
                End If
            Else
                strContainer1 += PasswordChar
            End If
        Next
        If (strContainer1.Length > 0) Then
            If strContainer2.Length > 0 Then
                strHashedPassword += strContainer2
            End If
            strHashedPassword += strContainer1
        End If
        Return strHashedPassword
    End Function

    'This function is used for creating a profile for WiFi Set up

    Private Sub CreateProfile()


        Dim wlan As New WLAN()
        ' the below line will delete all the profile already present.
        wlan.Profiles.DeleteAll()

        Dim strUserName As String = Nothing
        Dim strPassword As String = Nothing


        'Get the MAC address of the device to use it as user name.
        'Remove all the :'s to make is tohave only alphanumerics.


        strUserName = wlan.Adapters(0).MacAddress.Replace(":", "")

        'Disposing the MacAddress Object

        ' objSymbolMACAddress = Nothing
        '  objSymbolRadio = Nothing

        'Generate Password using the user name i.e., using the MAC address.
        strPassword = HashMACAddress(strUserName)

        '*** Set Default Profile name to BTC[StoreID]K
        'commented this line of code by Anil for changing the store id. 

        'StoreID = "BTC" & StoreID
        'If StoreSetting = "A" Then
        '    StoreID += "K"
        'ElseIf StoreSetting = "B" Then
        '    StoreID = "BTCOSSRK"
        'Else
        '    StoreID = "BTC0006"
        'End If

        'StoreID = "boylnuzm"


        ' the code is commented by Anil
        'Dim myAdhocProfileData As New Symbol.Fusion.WLAN.AdhocProfileData(StoreID, StoreID)
        Dim myAdhocProfileData As New Symbol.Fusion.WLAN.AdhocProfileData("BUK-Stores", "BUK-Stores")

        'create a profile with default ProfileData object 

        '   wlan.Profiles.CreateAdhocProfile(myAdhocProfileData)

        'Set powerMode


        myAdhocProfileData.PowerMode = PowerMode.FAST_POWER_SAVE

        'set to full transmission power

        myAdhocProfileData.TransmissionPower = AdhocProfileData.TransmitPowerLevel.IBSS_FULL

        myAdhocProfileData.IPSettings.AddressingMode = IPSettings.AddressingModes.DHCP

        myAdhocProfileData.Encryption.EncryptionType = Symbol.Fusion.WLAN.Encryption.EncryptionTypes.AES

        'set the communication channel

        myAdhocProfileData.Channel = Symbol.Fusion.WLAN.AdhocProfileData.CommunicationChannel.FREQUENCY_2412_MHZ

        'create new ProfileData object 

        ' Dim myInfrastructureProfileData As New Symbol.Fusion.WLAN.InfrastructureProfileData(StoreID, StoreID)
        Dim myInfrastructureProfileData As New Symbol.Fusion.WLAN.InfrastructureProfileData("BUK-Stores", "BUK-Stores")

        'create a profile with default ProfileData object 

        '  wlan.Profiles.CreateInfrastructureProfile(myInfrastructureProfileData)

        'Set powerMode


        myInfrastructureProfileData.PowerMode = PowerMode.FAST_POWER_SAVE

        'set to full transmission power

        myInfrastructureProfileData.TransmissionPower = InfrastructureProfileData.TransmitPowerLevel.BSS_AUTO



        'set security type to SECURITY_WPA2_ENTERPRISE

        myInfrastructureProfileData.SecurityType = WLANSecurityType.SECURITY_WPA2_ENTERPRISE

        'set authentication to PEAP_MSCHAPV2

        myInfrastructureProfileData.Authentication.AuthenticationType = Symbol.Fusion.WLAN.Authentication.AuthenticationTypes.PEAP_MSCHAPV2

        myInfrastructureProfileData.Authentication.ServerCertificate.Validate = False

        'Select a local server certificate

        myInfrastructureProfileData.Authentication.ServerCertificate.CertificateType = Authentication.CertificateTypes.LOCAL
        myInfrastructureProfileData.Authentication.LoginOperation = Authentication.ProfileLoginOperation.AUTO
        myInfrastructureProfileData.Authentication.UserCertificate.CertificateType = Authentication.CertificateTypes.LOCAL

        'provide the certificate as none which is installed in the device (in this case)

        myInfrastructureProfileData.Authentication.ServerCertificate.Name = ""

        myInfrastructureProfileData.Authentication.CredentialPrompt.CacheOptions.AtConnect = False
        myInfrastructureProfileData.Authentication.CredentialPrompt.CacheOptions.AtGivenTime = False
        myInfrastructureProfileData.Authentication.CredentialPrompt.CacheOptions.AtResume = False

        myInfrastructureProfileData.Authentication.UserCredentials.Domain = ""
        myInfrastructureProfileData.Authentication.UserCredentials.PassWord = strPassword
        myInfrastructureProfileData.Authentication.UserCredentials.UserName = strUserName

        'set the encryption to AES

        myInfrastructureProfileData.Encryption.EncryptionType = Symbol.Fusion.WLAN.Encryption.EncryptionTypes.AES

        'Select DHCP IP mode

        ' the country code for UK  is GB.
        myInfrastructureProfileData.CountryCode = "GB"
        myInfrastructureProfileData.IPSettings.AddressingMode = IPSettings.AddressingModes.DHCP
        ' myInfrastructureProfileData.IPSettings.AddressingMode = Symbol.Fusion.WLAN.IPSettings.AddressingModes.DHCP

        wlan.Profiles.CreateInfrastructureProfile(myInfrastructureProfileData)


    End Sub

End Class
