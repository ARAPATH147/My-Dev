Imports System
Imports System.Data
Imports System.IO
Imports Microsoft.VisualBasic
''' <summary>
''' This class will be used for writing the application logs into a log file
''' </summary>
''' <remarks></remarks>
Public Class Logger

    Public Enum LogLevel As Integer
        RELEASE = 0
        [ERROR]
        DEBUG
        INFO
    End Enum
    'Initialising private variables as shared for use in shared function
    Private Shared strRelease As String = "False"
    Private Shared strError As String = "False"
    Private Shared strDebug As String = "False"
    Private Shared strInfo As String = "False"

    'Determine the log file name <TB Finalised>
    Private strLogFileName As String
    Public Sub New()

        Dim TimeStamp As String = Nothing
        Dim IPOctect As String = "txt"
        TimeStamp = DateTime.Now.ToString("MMddHHmmss")
        Dim temp As Integer = CInt(TimeStamp.Substring(0, 2))

        If (temp > 9) Then
            TimeStamp = TimeStamp.Remove(0, 2)
            Select Case temp
                Case 10
                    TimeStamp = TimeStamp.Insert(0, "A")
                Case 11
                    TimeStamp = TimeStamp.Insert(0, "B")
                Case 12
                    TimeStamp = TimeStamp.Insert(0, "C")
            End Select
        Else
            TimeStamp = TimeStamp.TrimStart("0")
        End If
        TimeStamp = TimeStamp.Remove(TimeStamp.Length - 1, 1)
        strLogFileName = TimeStamp + "." + IPOctect
#If NRF Then
        Dim DeviceMacAddress As String = ""
        DeviceMacAddress = objAppContainer.objHelper.GetSerialNumber()
        WriteAppLog(DeviceMacAddress + ":: ShelfManagement :: " + DateTime.Now.ToString("MMddyyyHHmmss"))
#ElseIf RF Then
         WriteAppLog(" Logging into ShelfManagement :: Mode-RF :: MAC ID INFO - NIL :: " + DateTime.Now.ToString("MMddyyyHHmmss"))
#End If
       End Sub
    Private WriteOnly Property LogFileName() As String
        Set(ByVal strValue As String)
            strLogFileName = strValue
        End Set
    End Property
    'Overloaded Sub implementing logging functionality based on level set
    Public Sub WriteAppLog(ByVal strMessage As String, ByVal eLoggingLevel As LogLevel)
        GetLogLevel()
        Try
            If strRelease Then
                If eLoggingLevel = LogLevel.RELEASE Then

                    Dim strLogFilePath As String = Path.Combine(ConfigDataMgr.GetInstance.GetParam(ConfigKey.LOG_FILE_PATH), strLogFileName)
                    'Dim strLogFilePath As String = Path.Combine("\application\IS\StoreApps", strLogFileName)
                    Dim strLogDateTime As String = "[ " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " ]"
                    Dim strLogData As String = " [" + eLoggingLevel.ToString() + "] :: " + strLogDateTime + " :: " + strMessage + vbCr & vbLf
                    FileIO.WriteDataIntoFile(strLogFilePath, strLogData, True)
                End If

            ElseIf strError Then
                If (eLoggingLevel = LogLevel.ERROR) Or (eLoggingLevel = LogLevel.RELEASE) Then

                    Dim strLogFilePath As String = Path.Combine(ConfigDataMgr.GetInstance.GetParam(ConfigKey.LOG_FILE_PATH), strLogFileName)
                    'Dim strLogFilePath As String = Path.Combine("\application\IS\StoreApps", strLogFileName)
                    Dim strLogDateTime As String = "[ " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " ]"
                    Dim strLogData As String = " [" + eLoggingLevel.ToString() + "] :: " + strLogDateTime + " :: " + strMessage + vbCr & vbLf
                    FileIO.WriteDataIntoFile(strLogFilePath, strLogData, True)
                End If
            ElseIf strDebug Then
                If (eLoggingLevel = LogLevel.DEBUG) Or (eLoggingLevel = LogLevel.ERROR) Or (eLoggingLevel = LogLevel.RELEASE) Then

                    Dim strLogFilePath As String = Path.Combine(ConfigDataMgr.GetInstance.GetParam(ConfigKey.LOG_FILE_PATH), strLogFileName)
                    'Dim strLogFilePath As String = Path.Combine("\application\IS\StoreApps", strLogFileName)
                    Dim strLogDateTime As String = "[ " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " ]"
                    Dim strLogData As String = " [" + eLoggingLevel.ToString() + "] :: " + strLogDateTime + " :: " + strMessage + vbCr & vbLf
                    FileIO.WriteDataIntoFile(strLogFilePath, strLogData, True)
                End If
            ElseIf strInfo Then
                If (eLoggingLevel = LogLevel.INFO) Or (eLoggingLevel = LogLevel.DEBUG) Or (eLoggingLevel = LogLevel.ERROR) Or (eLoggingLevel = LogLevel.RELEASE) Then

                    Dim strLogFilePath As String = Path.Combine(ConfigDataMgr.GetInstance.GetParam(ConfigKey.LOG_FILE_PATH), strLogFileName)
                    'Dim strLogFilePath As String = Path.Combine("\application\IS\StoreApps", sLogFileName)
                    Dim strLogDateTime As String = "[ " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " ]"
                    Dim strLogData As String = " [" + eLoggingLevel.ToString() + "] :: " + strLogDateTime + " :: " + strMessage + vbCr & vbLf
                    FileIO.WriteDataIntoFile(strLogFilePath, strLogData, True)
                End If
            End If
        Catch
            System.Windows.Forms.MessageBox.Show("UnHandled Exception in WriteAppLogs", "Error")
        End Try

    End Sub
    'Overloaded Sub for writing messages
    Public Sub WriteAppLog(ByVal strMessage As String)

        Try
            Dim strLogFilePath As String = Path.Combine(ConfigDataMgr.GetInstance.GetParam(ConfigKey.LOG_FILE_PATH), strLogFileName)
            Dim strLogDateTime As String = "[ " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " ]"
            Dim strLogData As String = " [" + "INFO" + "] :: " + strLogDateTime + " :: " + strMessage + vbCr & vbLf
            FileIO.WriteDataIntoFile(strLogFilePath, strLogData, True)
        Catch
            System.Windows.Forms.MessageBox.Show("UnHandled Exception in WriteAppLogs", "Error")
        End Try


    End Sub
    'Function reads the Log Level from the config file 
    Private Function GetLogLevel()
        Dim strLogLevel As String = ""

        strLogLevel = ConfigDataMgr.GetInstance.GetParam("LogLevel")

        If CInt(LogLevel.RELEASE) = strLogLevel Then
            strRelease = True

        ElseIf strLogLevel = CInt(LogLevel.ERROR) Then
            strError = True

        ElseIf strLogLevel = CInt(LogLevel.DEBUG) Then
            strDebug = True
        Else
            strInfo = True
        End If
        Return 0
    End Function
  
End Class


