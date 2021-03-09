''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Loads controller IP address from IPCONFG.XML file
''' </Summary>
'''****************************************************************************

Public Class Macros
    Public Const LOG_FILE_PATH = "\Program Files\btstoreapps\Logs"
    Public Const REF_DOWNLOAD_SCREEN = 1
    Public Const SUMMARY_SCREEN = 2
    Public Const REF_STATUS_FILE = "\Program Files\btstoreapps\BatchProcess.Xml"
    Public Const NO_REF_FILES As Integer = 10
    Public Const DATA_SLOT_NAME = "Status"
    Public Const OPERATORID = "000"
    'v1.1 MCF Change
    Public Const IPCONFIG_FILE_PATH As String = "\Program Files\btstoreapps\IPConfg.xml"
End Class
