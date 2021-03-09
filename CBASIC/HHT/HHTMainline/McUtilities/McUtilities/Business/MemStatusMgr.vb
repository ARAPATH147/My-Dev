Imports System.Text

Public Class MemStatusMgr
    ''' <summary>
    ''' Member variables.
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Private m_MemStatusInfo As frmMemoryStatusInfo
    Private objMemStatusInfo As MemoryStatusInfo
    Private Shared m_MemStatusInfoMgr As MemStatusMgr = Nothing

    Public Structure MEMORYSTATUS
        Public dwLength As UInt32
        Public dwMemoryLoad As UInt32
        Public dwTotalPhys As UInt32
        Public dwAvailPhys As UInt32
        Public dwTotalPageFile As UInt32
        Public dwAvailPageFile As UInt32
        Public dwTotalVirtual As UInt32
        Public dwAvailVirtual As UInt32
    End Structure

    Public Declare Function GlobalMemoryStatus Lib "CoreDll.Dll" _
    (ByRef ms As MEMORYSTATUS) As Integer

    Public Declare Function GetSystemMemoryDivision Lib "CoreDll.Dll" _
    (ByRef lpdwStorePages As UInt32, ByRef ldpwRamPages As UInt32, _
     ByRef ldpwPageSize As UInt32) As Integer

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
    Public Shared Function GetInstance() As MemStatusMgr
        If m_MemStatusInfoMgr Is Nothing Then
            m_MemStatusInfoMgr = New MemStatusMgr()
            Return m_MemStatusInfoMgr
        Else
            Return m_MemStatusInfoMgr
        End If
    End Function
    ''' <summary>
    ''' To start the session and intialise all the variables 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartSession()
        objAppContainer.objLogger.WriteAppLog("Entered MemStatusMgr", Logger.LogLevel.INFO)
        m_MemStatusInfo = New frmMemoryStatusInfo()
        objMemStatusInfo = New MemoryStatusInfo()
        GetMemoryStatus()
        DisplayScreen(MEM_INFO_SCREENS.Home)
    End Sub
    ''' <summary>
    ''' To end the session and release all the objects held by the IPInfoMgr
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EndSession()
        objAppContainer.objLogger.WriteAppLog("Exiting MemStatusMgr", Logger.LogLevel.INFO)
        Try
            m_MemStatusInfo.Close()
            m_MemStatusInfo.Dispose()
            m_MemStatusInfoMgr = Nothing
            objMemStatusInfo = Nothing
        Catch ex As Exception

        End Try

    End Sub
    Private Function GetMemoryStatus() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered Get Memory Status", Logger.LogLevel.INFO)
        Dim memStatus As New MEMORYSTATUS

        Try

            Dim storePages As UInt32
            Dim ramPages As UInt32
            Dim pageSize As UInt32

            Dim res As Integer = GetSystemMemoryDivision(storePages, ramPages, pageSize)

            ' Call the native GlobalMemoryStatus method with the defined structure.
            GlobalMemoryStatus(memStatus)

            'divide the memory variables by 1024 (nkb) to obtain the size in kilobytes
            objMemStatusInfo.PercentUsedMemory = Format$(memStatus.dwMemoryLoad, "###,###,###,###")
            objMemStatusInfo.TotalPhysicalMemory = Format$(memStatus.dwTotalPhys / 1024, "###,###,###,###")
            objMemStatusInfo.FreePhysicalMemory = Format$(memStatus.dwAvailPhys / 1024, "###,###,###,###")
            objMemStatusInfo.TotalPageFileSize = Format$(memStatus.dwTotalPageFile / 1024, "###,###,###,###")
            objMemStatusInfo.FreePageFileSize = Format$(memStatus.dwAvailPageFile / 1024, "###,###,###,###")
            objMemStatusInfo.TotalVirtualMemory = Format$(memStatus.dwTotalVirtual / 1024, "###,###,###,###")
            objMemStatusInfo.FreeVirtualMemory = Format$(memStatus.dwAvailVirtual / 1024, "###,###,###,###")

            If objMemStatusInfo.PercentUsedMemory.Equals("") Then
                objMemStatusInfo.PercentUsedMemory = "0"
            End If
            If objMemStatusInfo.TotalPhysicalMemory.Equals("") Then
                objMemStatusInfo.TotalPhysicalMemory = "0"
            End If
            If objMemStatusInfo.FreePhysicalMemory.Equals("") Then
                objMemStatusInfo.FreePhysicalMemory = "0"
            End If
            If objMemStatusInfo.TotalPageFileSize.Equals("") Then
                objMemStatusInfo.TotalPageFileSize = "0"
            End If
            If objMemStatusInfo.FreePageFileSize.Equals("") Then
                objMemStatusInfo.FreePageFileSize = "0"
            End If
            If objMemStatusInfo.TotalVirtualMemory.Equals("") Then
                objMemStatusInfo.TotalVirtualMemory = "0"
            End If
            If objMemStatusInfo.FreeVirtualMemory.Equals("") Then
                objMemStatusInfo.FreeVirtualMemory = "0"
            End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting Getmemory status of MemStatMgr", Logger.LogLevel.INFO)

    End Function
    Public Sub DisplayMemStatInfo()
        With m_MemStatusInfo

            .lblPercentUsedMem.Text = objMemStatusInfo.PercentUsedMemory + "%"
            .lblTotalPhysicalMemory.Text = objMemStatusInfo.TotalPhysicalMemory + "KB"
            .lblFreePhysicalMem.Text = objMemStatusInfo.FreePhysicalMemory + "KB"
            .lblTotalPageFileSize.Text = objMemStatusInfo.TotalPageFileSize + "KB"
            .lblFreePageFileSize.Text = objMemStatusInfo.FreePageFileSize + "KB"
            .lblTotalVirtualMem.Text = objMemStatusInfo.TotalVirtualMemory + "KB"
            .lblFreeVirtualMem.Text = objMemStatusInfo.FreeVirtualMemory + "KB"

            .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' Screen Display method for Count List. 
    ''' All Count List sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName"></param>
    ''' <returns>True if success</returns>
    ''' <remarks></remarks>
    Public Function DisplayScreen(ByVal ScreenName As MEM_INFO_SCREENS)
        objAppContainer.objLogger.WriteAppLog("Entered Display Screen of MemStatMgr", Logger.LogLevel.INFO)
        Try

            Select Case ScreenName
                Case MEM_INFO_SCREENS.Home
                    'Invoke method to display the home screen
                    m_MemStatusInfo.Invoke(New EventHandler(AddressOf DisplayMemStatInfo))
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting Display Screen of MemStatMgr", Logger.LogLevel.INFO)

        Return True

    End Function
End Class
Public Enum MEM_INFO_SCREENS
    Home
End Enum
Public Class MemoryStatusInfo
    Public PercentUsedMemory As String
    Public TotalPhysicalMemory As String
    Public FreePhysicalMemory As String
    Public TotalPageFileSize As String
    Public FreePageFileSize As String
    Public TotalVirtualMemory As String
    Public FreeVirtualMemory As String
End Class
