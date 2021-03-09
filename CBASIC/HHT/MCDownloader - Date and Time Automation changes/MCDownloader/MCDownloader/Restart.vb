Public Class Restart
    ''' <summary>
    ''' Pricete variables.
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared m_Restart As Restart
    Private Declare Function KernelIoControl Lib "coredll.dll" (ByVal dwIoControlCode As Integer, ByVal lpInBuf As IntPtr, ByVal nInBufSize As Integer, ByVal lpOutBuf As IntPtr, ByVal nOutBufSize As Integer, ByRef lpBytesReturned As Integer) As Integer
    ''' <summary>
    ''' To Get unique instance of the class object.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As Restart
        If m_Restart Is Nothing Then
            m_Restart = New Restart
            Return m_Restart
        Else
            Return m_Restart
        End If
    End Function
    Private Function CTL_CODE(ByVal iDeviceType As Integer, ByVal iFunc As Integer, ByVal iMethod As Integer, ByVal iAccess As Integer) As Integer
        Return (iDeviceType << 16) Or (iAccess << 14) Or (iFunc << 2) Or iMethod
    End Function
    ''' <summary>
    ''' Function to reset pocket PC.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ResetPocketPC() As Integer
        Const FILE_DEVICE_HAL As Integer = &H101
        Const METHOD_BUFFERED As Integer = 0
        Const FILE_ANY_ACCESS As Integer = 0
        Dim bytesReturned As Integer = 0
        Dim IOCTL_HAL_REBOOT As Integer
        IOCTL_HAL_REBOOT = m_Restart.CTL_CODE(FILE_DEVICE_HAL, 15, METHOD_BUFFERED, FILE_ANY_ACCESS)
        Return KernelIoControl(IOCTL_HAL_REBOOT, IntPtr.Zero, 0, IntPtr.Zero, 0, bytesReturned)
    End Function
End Class
