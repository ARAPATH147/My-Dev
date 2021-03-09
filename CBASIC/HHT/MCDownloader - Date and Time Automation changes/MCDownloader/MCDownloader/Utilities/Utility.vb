Imports System.Runtime.InteropServices
Imports System.Net
Imports Symbol.ResourceCoordination
Public Class Utility
    <DllImport("IphlpApi", SetLastError:=True)> _
  Private Shared Function GetAdaptersInfo(ByVal pAdapterInfo As IntPtr, ByRef pOutBufLen As Integer) As Integer
    End Function

#Region "Mac Address"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Structure IP_ADDR_STRING
        Private [Next] As IntPtr
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)> _
        Friend IpAddress As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)> _
        Friend IpMask As String
        Private Context As Integer
    End Structure '_IP_ADDR_STRING
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    <StructLayout(LayoutKind.Sequential)> Structure IP_ADAPTER_INFO
        Friend [Next] As IntPtr
        Private ComboIndex As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=260)> _
        Friend AdapterName() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=132)> _
        Friend Description() As Byte
        Friend AddressLength As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> _
        Friend Address() As Byte
        Private Index As System.UInt32
        Private Type As System.UInt32
        Private DhcpEnabled As Integer
        Private CurrentIpAddress As IntPtr
        Friend IpAddressList As IP_ADDR_STRING
        Friend GatewayList As IP_ADDR_STRING
        Friend DhcpServer As IP_ADDR_STRING
        Private HaveWins As Boolean
        Friend PrimaryWinsServer As IP_ADDR_STRING
        Friend SecondaryWinsServer As IP_ADDR_STRING
        Private LeaseObtained As Integer
        Private LeaseExpires As Integer
    End Structure
    ''' <summary>
    ''' Gets the MAC address of the device to be sent in SOR
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetMacAddress() As String
        Dim pAdapterInfo As IntPtr = IntPtr.Zero
        Dim pOutBufLen As Integer = 0
        Dim bTemp() As Byte = Nothing
        Dim strMac As String = Nothing
        Dim strTemp As String = Nothing
        Dim ipInfo As New IP_ADAPTER_INFO
        Try
            Dim ret As Integer = GetAdaptersInfo(pAdapterInfo, pOutBufLen)
            pAdapterInfo = Marshal.AllocHGlobal(pOutBufLen)
            ret = GetAdaptersInfo(pAdapterInfo, pOutBufLen)
            ipInfo = CType(Marshal.PtrToStructure(pAdapterInfo, GetType(IP_ADAPTER_INFO)), IP_ADAPTER_INFO)
            strTemp = strTemp & System.Text.ASCIIEncoding.ASCII.GetString(ipInfo.AdapterName, 0, ipInfo.AdapterName.Length - 1)
            'ad.Add(ipinfo.Address)
            bTemp = ipInfo.Address
            For iLoop As Integer = 0 To 5
                strMac = strMac & Hex(bTemp(iLoop)).PadLeft(2, "0")
            Next
            strMac = strMac.Trim()
            Marshal.FreeHGlobal(pAdapterInfo)
            AppContainer.GetInstance.obLogger.WriteAppLog("Device MAC Address :" + strMac, Logger.LogLevel.RELEASE)
            'Return MAC address obtained.
            Return strMac
        Catch ex As Exception
            'return nothing if MAC address is not found or failed to retreive
            'MAC address.
            Return ""
        End Try
    End Function 'GetMacAddress
    

#End Region
#Region "IP ADDRESS"
    ''' <summary>
    ''' To check for the dynamic IP generated when the device is docked
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Function GetIPAddress() As String
        'Declare the local variable and the get the host name
        Dim sDnsName As String = Nothing
        Dim m_IpHostEntry As System.Net.IPHostEntry = Nothing
        Dim m_aIPAddressArray As System.Net.IPAddress() = Nothing
        Dim strIP As String = ""
        Dim iIndex As Integer = 0
        Dim aIPSubnet() As String = Nothing
        Try
            sDnsName = System.Net.Dns.GetHostName()
            AppContainer.GetInstance().obLogger.WriteAppLog("Getting DNS name" + _
                                                  sDnsName, Logger.LogLevel.INFO)
            m_IpHostEntry = System.Net.Dns.GetHostEntry(sDnsName)
            AppContainer.GetInstance().obLogger.WriteAppLog("Getting IP Host", _
                                                  Logger.LogLevel.INFO)
            m_aIPAddressArray = m_IpHostEntry.AddressList()
            AppContainer.GetInstance().obLogger.WriteAppLog("Getting IP address" + _
                                                  m_IpHostEntry.AddressList(0).ToString(), _
                                                  Logger.LogLevel.INFO)
            ' Check if the address array has a default value
            If m_aIPAddressArray.Length > 0 Then
                ' Check within a loop whether the IP is else then 127.0.0.1
                For iIndex = 0 To m_aIPAddressArray.Length - 1
                    ' If the address IP is else then convert it into string 
                    If m_aIPAddressArray(iIndex).ToString() = "127.0.0.1" And _
                       m_aIPAddressArray.Length = 1 Then
                        strIP = m_aIPAddressArray(iIndex).ToString()
                        AppContainer.GetInstance().obLogger.WriteAppLog("IP of device is" _
                                                              & strIP, _
                                                              Logger.LogLevel.RELEASE)
                    ElseIf m_aIPAddressArray(iIndex).ToString() <> "127.0.0.1" Then
                        strIP = m_aIPAddressArray(iIndex).ToString()
                        AppContainer.GetInstance().obLogger.WriteAppLog("IP of device is" _
                                                              & strIP, _
                                                              Logger.LogLevel.RELEASE)
                    End If
                Next
                ' Return the new IP generated when the device is docked into 
                'the(cradle)
                'format the IP address to have 3 digits in all the three subnets
                aIPSubnet = strIP.Split(".")
                aIPSubnet(0) = aIPSubnet(0).PadLeft(3, "0")
                aIPSubnet(1) = aIPSubnet(1).PadLeft(3, "0")
                aIPSubnet(2) = aIPSubnet(2).PadLeft(3, "0")
                aIPSubnet(3) = aIPSubnet(3).PadLeft(3, "0")
                strIP = aIPSubnet(0) & "." & aIPSubnet(1) & "." & _
                           aIPSubnet(2) & "." & aIPSubnet(3)

                'returnt he IP address to the calling function
                Return strIP
            Else
                'Return the default IP of the device when the device is 
                'not docked into the cradle
                Return "127.000.000.001"
            End If
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("Utility:: GetIpAddress:: Device IP retreival failed" + _
                                      ex.StackTrace, Logger.LogLevel.RELEASE)
            Return "127.000.000.001"
        End Try
    End Function
#End Region
#Region "DEVICE SERIAL NO"
    Public Shared Function GetSerialNumber() As String
        Try

            Dim objTerminalInfo As TerminalInfo = New TerminalInfo()
            AppContainer.GetInstance().obLogger.WriteAppLog("Serial Number of Device : " + _
                                          objTerminalInfo.ESN.ToString(), Logger.LogLevel.RELEASE)
            If objTerminalInfo.ESN.Length > 12 Then
                Return objTerminalInfo.ESN.Substring(objTerminalInfo.ESN.Length - 12, 12).ToString()
            Else
                Return objTerminalInfo.ESN.ToString().PadLeft(12, "0")
            End If

        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("Device Serial Number retreival failed" + _
                                      ex.Message + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return "000000000000"
        End Try
    End Function
#End Region

End Class
