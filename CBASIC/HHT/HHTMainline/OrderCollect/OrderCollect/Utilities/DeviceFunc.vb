Imports System.Runtime.InteropServices

Public Class DeviceFunc

    <DllImport("coredll.dll")> _
    Public Shared Function GetDiskFreeSpaceEx(ByVal directory As String, ByRef lpFreeBytesAvailableToCaller As Integer, ByRef lpTotalNumberOfBytes As Integer, ByRef lpTotalNumberOfFreeBytes As Integer) As Boolean
    End Function
    <DllImport("IphlpApi", SetLastError:=True)> _
       Public Shared Function GetAdaptersInfo(ByVal pAdapterInfo As IntPtr, ByRef pOutBufLen As Integer) As Integer
    End Function

    Private Shared oDeviceFunc As DeviceFunc

    ''' <summary>
    ''' The shared function GetInstance will implement a check for the instantiation
    ''' of class DeviceFunc to make sure that the class has only one instance
    ''' </summary>
    Public Shared Function GetInstance() As DeviceFunc
        If oDeviceFunc Is Nothing Then
            oDeviceFunc = New DeviceFunc
        End If
        Return oDeviceFunc

    End Function


    ''' <summary>
    ''' Gets the MAC address of the device to be sent in SOR
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMacAddress() As String

        Dim structSize As Int32 = Marshal.SizeOf(GetType(IP_ADAPTER_INFO))
        Dim pArray As IntPtr = Marshal.AllocHGlobal(structSize)
        Dim len As UInt64 = Convert.ToUInt64(structSize)
        Dim ret As Int32 = GetAdaptersInfo(pArray, len)
        Dim strMacAddress As String = "000000000000"
        Dim setppc As Boolean = False

        'Check the OS version to identify between PPC and MC55 device.

        If Not Environment.OSVersion.Version.ToString().StartsWith("5.2.") Then
            setppc = True
        End If

        'Dotnet CF returns 6 as the adapter type for ethernet and wireless
        Dim uintAdapterType As System.UInt32 = 6
        Dim adapterIndex As Integer = Nothing

        If setppc = True Then
            'For PPC adapter index will be 1
            adapterIndex = 1
        Else
            'For MC55RF the adapter index will be 2
            adapterIndex = 2
        End If

        Try
            If ret = 111 Then
                ' Buffer was too small, reallocate the correct size for the buffer.
                pArray = Marshal.ReAllocHGlobal(pArray, New IntPtr(Convert.ToInt64(len)))
                ret = GetAdaptersInfo(pArray, len)
            End If

            If ret = 0 Then
                Dim pAdapterInfo As IntPtr = pArray
                Dim loopIndex As Integer = 1

                Do
                    ' Retrieve the adapter info from the memory address.
                    Dim currAdapterInfo As IP_ADAPTER_INFO = _
                    CType(Marshal.PtrToStructure(pAdapterInfo, GetType(IP_ADAPTER_INFO)), IP_ADAPTER_INFO)
                    'Check for valid adapter type - ethernet/wireless
                    If currAdapterInfo.Type = uintAdapterType And _
                        loopIndex = adapterIndex Then
                        ' Loop through and extract MAC address
                        Dim StrMac As String = Nothing
                        For iLoop As Integer = 0 To 5
                            StrMac = StrMac & Hex(currAdapterInfo.Address(iLoop)).PadLeft(2, "0")
                        Next
                        strMacAddress = StrMac.Trim
                        Marshal.FreeHGlobal(pArray)
                        Return strMacAddress
                    End If
                    loopIndex = loopIndex + 1
                    pAdapterInfo = currAdapterInfo.Next
                Loop Until IntPtr.op_Equality(pAdapterInfo, IntPtr.Zero)
            End If
        Catch ex As Exception
            ' Always return something...
            Marshal.FreeHGlobal(pArray)
            Return strMacAddress
        End Try

        ' Always return something...
        Marshal.FreeHGlobal(pArray)

        Return strMacAddress
    End Function

   
    ''' <summary>
    ''' To check for the dynamic IP generated when the device is docked
    ''' </summary>
    ''' <remarks></remarks>    
    Public Function GetIPAddress() As String
        'Declare the local variable and the get the host name
        Dim sDnsName As String = Nothing
        Dim m_IpHostEntry As System.Net.IPHostEntry = Nothing
        Dim m_aIPAddressArray As System.Net.IPAddress() = Nothing
        Dim strIP As String = ""
        Dim iIndex As Integer = 0
        Dim aIPSubnet() As String = Nothing
        Try
            sDnsName = System.Net.Dns.GetHostName()
            m_IpHostEntry = System.Net.Dns.GetHostEntry(sDnsName)
            m_aIPAddressArray = m_IpHostEntry.AddressList()
            ' Check if the address array has a default value
            If m_aIPAddressArray.Length > 0 Then
                ' Check within a loop whether the IP is else then 127.0.0.1
                For iIndex = 0 To m_aIPAddressArray.Length - 1
                    ' If the address IP is else then convert it into string 
                    If m_aIPAddressArray(iIndex).ToString() = "127.0.0.1" And _
                       m_aIPAddressArray.Length = 1 Then
                        strIP = m_aIPAddressArray(iIndex).ToString()
                    ElseIf m_aIPAddressArray(iIndex).ToString() <> "127.0.0.1" Then
                        strIP = m_aIPAddressArray(iIndex).ToString()
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
            Return "127.000.000.001"
        End Try
    End Function

    ''' <summary>
    ''' To check for free memory in the device
    ''' </summary>
    ''' <param name="folder"></param>
    ''' <param name="iFreemem"></param>
    ''' <remarks>The folder of which the free memory needs to be calculated</remarks>
    Public Function CheckForFreeMemory(ByVal folder As String, ByRef iFreemem As Long) As String
        'Declare the local variables
        Dim folderName As String = folder
        Dim freeBytesAvailableToCaller As UInteger = 0
        Dim totalNumberOfBytes As UInteger = 0
        Dim totalNumberOfFreeBytes As UInteger = 0
        Try
            'Call GetDiskFreeSpaceEx for getting the free memory space available 
            'in the specified device
            GetDiskFreeSpaceEx(folderName, freeBytesAvailableToCaller, _
                               totalNumberOfBytes, totalNumberOfFreeBytes)
            Dim totalFreeMemoryinMB As Long = totalNumberOfFreeBytes / (1024 * 1024)
            iFreemem = totalFreeMemoryinMB * 8
        Catch ex As Exception
            Return False
        End Try
        Return iFreemem
    End Function

    Structure IP_ADAPTER_INFO
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
        'Private Type As System.UInt32
        Friend Type As System.UInt32
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

    Structure IP_ADDR_STRING
        Private [Next] As IntPtr
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)> _
        Friend IpAddress As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)> _
        Friend IpMask As String
        Private Context As Integer
    End Structure
End Class
