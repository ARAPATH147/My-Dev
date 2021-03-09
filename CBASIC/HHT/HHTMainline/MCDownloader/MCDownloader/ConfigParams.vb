Imports System.Xml
''' * Modification Log
''' 
'''******************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Added variables for Primary, Secondary and Active IP addresses
''' </Summary>
'''******************************************************************************
Public Class ConfigParams
    Public AppName As String
    Public FirstInvokeTime As String
    Public FirstDownloadTime As String
    Public AppExitTime As String
    Public DownloadInterval As Integer
    Public FirstInvokeForToday As String
    Public RestartRequired As String
    Public LastBuildBOOTCODE As DateTime
    Public LastBuildPGROUP As DateTime
    Public LastBuildSUPPLIER As DateTime
    Public LastBuildUSERS As DateTime
    Public LastBuildRECALL As DateTime
    Public LastBuildLIVEPOG As DateTime
    Public LastBuildBARCODE As DateTime
    Public LastBuildDEAL As DateTime
    Public LastBuildCATEGORY As DateTime
    Public LastBuildSHELFDES As DateTime
    Public LastBuildPOGMODULE As DateTime
    Public LogLevel As String
    Public LocalLogFilePath As String
    Public RemoteLogFilePath As String
    Public HomeDir As String
    Public StatusFileName As String
    Public DB As String
    Public TemplatDB As String
    Public ControlFile As ControlFile
    Public TFTP As TftpParams
    Public BatchProcessWindow As Integer
    Public ReferenceFileCount As Integer
    Public DownloadCount As Integer
    Public BuildInProgress As String
    Public BOOTCODE As String
    Public PGROUP As String
    Public SUPPLIER As String
    Public USERS As String
    Public RECALL As String
    Public LIVEPOG As String
    Public BARCODE As String
    Public DEAL As String
    Public CATEGORY As String
    Public SHELFDES As String
    Public POGMODULE As String
    Public UserIdleTimeOut As String
    Public BattSuspendTimeout As String
    Public SummaryScreenDisplayTime As Integer
    Public ConnectionLostCheckTime As Integer
    Public InitialisingDownloadTime As Integer
    Public ConnectionLostRestartTime As Integer
    Public ErrorInvokeTime As Integer
    Public IPCheckRetry As Integer
    Public IPCheckRetryWaitTime As Integer
    Public ControllerPort As String
End Class
Public Class ControlFile
    Public Name As String
    Public FileNameField As FieldProperty
    Public BuildStatus As FieldProperty
    Public LastBuild As FieldProperty
End Class
Public Class FieldProperty
    Public StartIndex As Integer
    Public Length As Integer
End Class
Public Class TftpParams
    Public Host As String
    Public Port As String
    Public RemoteFilePath As String
    Public LocalFilePath As String
End Class
Public Class IPParams
    'v1.1 Start - MCF Changes
    Public Const PrimaryIP As String = "primaryIP"
    Public Const SecondaryIP As String = "secondaryIP"
    Public Const ActiveIP As String = "activeIP"
    'v1.1 End - MCF Changes
End Class