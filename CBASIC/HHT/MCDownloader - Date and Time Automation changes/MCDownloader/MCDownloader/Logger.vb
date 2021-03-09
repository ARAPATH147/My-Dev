Imports System
Imports System.Data
Imports System.IO
''' <summary>
''' This class will be used for writing the application logs into a log file
''' </summary>
''' <remarks></remarks>
Public Class Logger
    ''' <summary>
    ''' Enum to set log level.
    ''' </summary>
    ''' <remarks></remarks>
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

    'Determine the log file name
    Private strLogFileName As String = Nothing
    ''' <summary>
    ''' Constructor for LogFileUploader class
    ''' </summary>
    ''' <remarks></remarks>
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
        'AppContainer.GetInstance().m_LogFilename = strLogFileName
        WriteAppLog(":: MCDownloader :: " + DateTime.Now.ToString("MMddyyyHHmmss"))
    End Sub
    ''' <summary>
    ''' Function that returns the current log file that is in use by the downloader
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetLogFileName() As String
        Return strLogFileName
    End Function
    ''' <summary>
    ''' Overloaded Sub implementing logging functionality based on level set
    ''' </summary>
    ''' <param name="strMessage"></param>
    ''' <param name="eLoggingLevel"></param>
    ''' <remarks></remarks>
    Public Sub WriteAppLog(ByVal strMessage As String, ByVal eLoggingLevel As LogLevel)
        GetLogLevel()
        Try
            If strRelease Then
                If eLoggingLevel = LogLevel.RELEASE Then

                    Dim strLogFilePath As String = Path.Combine(Macros.LOG_FILE_PATH, strLogFileName)
                    'Dim strLogFilePath As String = Path.Combine("\application\IS\StoreApps", strLogFileName)
                    Dim strLogDateTime As String = "[ " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " ]"
                    Dim strLogData As String = " [" + eLoggingLevel.ToString() + "] :: " + strLogDateTime + " :: " + strMessage + vbCr & vbLf
                    FileIO.WriteDataIntoFile(strLogFilePath, strLogData, True)
                End If

            ElseIf strError Then
                If (eLoggingLevel = LogLevel.ERROR) Or (eLoggingLevel = LogLevel.RELEASE) Then

                    Dim strLogFilePath As String = Path.Combine(Macros.LOG_FILE_PATH, strLogFileName)
                    'Dim strLogFilePath As String = Path.Combine("\application\IS\StoreApps", strLogFileName)
                    Dim strLogDateTime As String = "[ " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " ]"
                    Dim strLogData As String = " [" + eLoggingLevel.ToString() + "] :: " + strLogDateTime + " :: " + strMessage + vbCr & vbLf
                    FileIO.WriteDataIntoFile(strLogFilePath, strLogData, True)
                End If
            ElseIf strDebug Then
                If (eLoggingLevel = LogLevel.DEBUG) Or (eLoggingLevel = LogLevel.ERROR) Or (eLoggingLevel = LogLevel.RELEASE) Then

                    Dim strLogFilePath As String = Path.Combine(Macros.LOG_FILE_PATH, strLogFileName)
                    'Dim strLogFilePath As String = Path.Combine("\application\IS\StoreApps", strLogFileName)
                    Dim strLogDateTime As String = "[ " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " ]"
                    Dim strLogData As String = " [" + eLoggingLevel.ToString() + "] :: " + strLogDateTime + " :: " + strMessage + vbCr & vbLf
                    FileIO.WriteDataIntoFile(strLogFilePath, strLogData, True)
                End If
            ElseIf strInfo Then
                'If (eLoggingLevel = LogLevel.INFO) Or (eLoggingLevel = LogLevel.DEBUG) Or (eLoggingLevel = LogLevel.ERROR) Or (eLoggingLevel = LogLevel.RELEASE) Then

                Dim strLogFilePath As String = Path.Combine(Macros.LOG_FILE_PATH, strLogFileName)
                'Dim strLogFilePath As String = Path.Combine("\application\IS\StoreApps", sLogFileName)
                Dim strLogDateTime As String = "[ " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " ]"
                Dim strLogData As String = " [" + eLoggingLevel.ToString() + "] :: " + strLogDateTime + " :: " + strMessage + vbCr & vbLf
                FileIO.WriteDataIntoFile(strLogFilePath, strLogData, True)
                'End If
            End If
        Catch
            'System.Windows.Forms.MessageBox.Show("UnHandled Exception in WriteAppLogs", "Error")
        End Try

    End Sub
    ''' <summary>
    ''' Overloaded Sub for writing messages
    ''' </summary>
    ''' <param name="strMessage"></param>
    ''' <remarks></remarks>
    Public Sub WriteAppLog(ByVal strMessage As String)

        Try
            Dim strLogFilePath As String = Path.Combine(Macros.LOG_FILE_PATH, strLogFileName)
            Dim strLogDateTime As String = "[ " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " ]"
            Dim strLogData As String = " [" + "INFO" + "] :: " + strLogDateTime + " :: " + strMessage + vbCr & vbLf
            FileIO.WriteDataIntoFile(strLogFilePath, strLogData, True)
        Catch
            'System.Windows.Forms.MessageBox.Show("UnHandled Exception in WriteAppLogs", "Error")
        End Try


    End Sub
    ''' <summary>
    ''' Function reads the Log Level from the config file 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetLogLevel()
        Dim strLogLevel As String = ""
        'ConfigDetails.GetInstance.GetConfigParam("LogLevel", cLogLevel)
        strLogLevel = ConfigParser.GetInstance().GetConfigParams().LogLevel

        If LogLevel.RELEASE = CInt(strLogLevel) Then
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


