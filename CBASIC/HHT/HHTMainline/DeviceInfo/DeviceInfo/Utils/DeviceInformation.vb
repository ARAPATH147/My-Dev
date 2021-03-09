Imports System.Runtime.InteropServices
Imports Symbol
Imports Microsoft.Win32
Imports System.Windows.Forms
'''****************************************************************************
''' <FileName> DeviceInformation.vb </FileName> 
''' <summary> Class gets the system information and create messge to be sent to the controller</summary> 
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

Public Class DeviceInformation

    <DllImport("IphlpApi", SetLastError:=True)> _
       Public Shared Function GetAdaptersInfo(ByVal pAdapterInfo As IntPtr, ByRef pOutBufLen As Integer) As Integer
    End Function
    <DllImport("coredll.dll")> _
    Public Shared Sub GetSystemTime(ByRef lpSystemTime As SYSTEMTIME)
    End Sub
    <DllImport("coredll.dll")> _
    Public Shared Function SetLocalTime(ByRef lpSystemTime As SYSTEMTIME) As Boolean
    End Function

    Private Shared oDeviceWLan As DeviceInformation
    Private oDeviceInfo As New DEVICE_INFO
    Private cData As String
    Private cSystemTime As String

    ''' <summary>
    ''' The shared function GetInstance will implement a check for the instantiation
    ''' of class DeviceWLan to make sure that the class has only one instance
    ''' </summary>
    Public Shared Function GetInstance() As DeviceInformation
        If oDeviceWLan Is Nothing Then
            oDeviceWLan = New DeviceInformation
        End If
        Return oDeviceWLan
    End Function

    Public Sub GetDeviceInfo()
        Dim type As String
        If oAppMain.cDeviceBuildType = "RF" Then
            type = "4" 'RF device 
        ElseIf oAppMain.cDeviceBuildType = "BH" Then
            type = "5" 'Batch device
        Else
            type = "0" 'Unknown
        End If

        With oDeviceInfo
            .cFiller = StrDup(17, " ")
            .cModelNumber = GetDeviceType()
            .cSerialNumber = GetSerialNumber()
            .cMACAddress = GetMacAddress()
            .cIPAddress = GetIPAddress()
            .cBuildType = type
        End With

        updateController()
    End Sub

    Private Sub updateController()
        Dim message As String = "DUM" + _
                                oDeviceInfo.cSerialNumber + _
                                oDeviceInfo.cMACAddress + _
                                oDeviceInfo.cIPAddress + _
                                oDeviceInfo.cModelNumber + _
                                oDeviceInfo.cBuildType + _
                                oDeviceInfo.cFiller

        If ConnectionMgr.GetInstance.Connected Then
            ConnectionMgr.GetInstance.Send(message)
            If ConnectionMgr.GetInstance.Receive(cData) Then
                Select Case cData.Substring(0, 3)
                    Case "DUR"
                        cSystemTime = cData.Substring(3, 12)
                        SetDeviceDateTime(cSystemTime)
                        oAppMain.cControllerSoftwareLevel = cData.Substring(15, 3)
                        oAppMain.cControllerBuildType = cData.Substring(19, 1)
                    Case "NAK"
                        oAppMain.cControllerSoftwareLevel = 0
                        oAppMain.cControllerBuildType = 0
                End Select
            End If
        End If
        ConnectionMgr.GetInstance.Disconnect()
    End Sub

    ''' <summary>
    ''' Returns the device model number 
    ''' </summary>
    ''' <returns>1=MC55, 2=MC70, 0=Unkown deivce</returns>
    ''' <remarks></remarks>
    Public Function GetDeviceType()
        Dim dtype As String = "0" 'Unknown deivce
        Dim info As New ResourceCoordination.ConfigData
        Dim devicelist As New ResourceCoordination.TerminalTypes
        devicelist = info.TERMINAL
        If InStr(devicelist.ToString, "MC55") > 0 Then
            dtype = "1" 'MC55
        ElseIf InStr(devicelist.ToString, "MC70") > 0 Then
            dtype = "2" 'MC70
        End If
        Return dtype
    End Function

    ''' <summary>
    ''' Returns the device serial number
    ''' </summary>
    ''' <returns>14 digit serial number</returns>
    Public Function GetSerialNumber()
        Dim terminalInfo As New ResourceCoordination.TerminalInfo
        Return terminalInfo.ESN.PadLeft(14, "0")
    End Function


    ''' <summary>
    ''' Returns the MAC Address of the network adapter
    ''' </summary>
    ''' <returns>returns 12 digit MAC Address</returns>
    ''' <remarks>000000000000 wil be returned if unable to get the deivce MAC address</remarks>
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
    ''' Returns the device IP address
    ''' </summary>
    ''' <returns>IP address in the format xxx.xxx.xxx.xxx</returns>
    ''' <remarks>127.000.000.1 will be returned if IP address is not found</remarks>    
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
    ''' To set device time to same as controller time.
    ''' </summary>
    ''' <param name="strDateTime">Datetime string recevived from controller</param>
    ''' <returns>
    ''' True - If successfully set the device time.
    ''' False - If error in setting the device time.
    ''' </returns>
    ''' <remarks></remarks>
    Private Function SetDeviceDateTime(ByVal strDateTime As String) As Boolean
        Dim SysTime As SYSTEMTIME
        'Get the device time.
        GetSystemTime(SysTime)
        'Populate structure to update the table.
        With SysTime
            .wYear = Convert.ToInt16(strDateTime.Substring(0, 4))
            .wMonth = Convert.ToInt16(strDateTime.Substring(4, 2))
            .wDay = Convert.ToInt16(strDateTime.Substring(6, 2))
            .wHour = Convert.ToInt16(strDateTime.Substring(8, 2))
            .wMinute = Convert.ToInt16(strDateTime.Substring(10, 2))
            .wSecond = Convert.ToInt16(0)
        End With

        'Set the new time`
        Return SetLocalTime(SysTime)
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

    Structure DEVICE_INFO               '60
        Public cSerialNumber As String  '14 
        Public cIPAddress As String     '15
        Public cMACAddress As String    '12
        Public cModelNumber As String   '1
        Public cBuildType As String     '1 
        Public cFiller As String        '17
    End Structure

    Public Structure SYSTEMTIME
        Public wYear As Short
        Public wMonth As Short
        Public wDayOfWeek As Short
        Public wDay As Short
        Public wHour As Short
        Public wMinute As Short
        Public wSecond As Short
        Public wMilliSeconds As Short
    End Structure

End Class
