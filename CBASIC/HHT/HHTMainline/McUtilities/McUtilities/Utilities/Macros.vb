''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Added IPCONFG.XML for looking up controller IP address
''' </Summary>
'''****************************************************************************

Public Class Macros

    'Barcode Reader 
    Public Const MAX_I2OF5_LEN As Integer = 12
    Public Const MESSAGE_FILE_PATH = "\Utilities\ConfigFiles\MessageConfig.xml"
    Public Const LOG_FILE_PATH = "\Program Files\btstoreapps\Logs"
    Public Const CONFIG_FILE_PATH = "\Program Files\btstoreapps\McUtilities_Config.xml"
    Public Const REF_STATUS_FILE = "\Program Files\btstoreapps\BatchProcess.Xml"
    Public Const ACT_EXP_STATUSFILE = "\Program Files\btstoreapps\ActBuildProcess.xml"
    Public Const MCSHMON_CONFIG_FILE = "\Program Files\btstoreapps\McShMon_Config.xml"
    Public Const GOODSOUT_CONFIG_FILE = "\Program Files\btstoreapps\MCGdOut_Config.xml"
    Public Const GOODSIN_CONFIG_FILE = "\Program Files\btstoreapps\MCGdIn_Config.xml"
    Public Const NO_ACT_FILES As Integer = 7
    Public Const NO_REF_FILES As Integer = 10
    'Constants used by Data Access module
    Public Const MCSHMON_EXPORT_FILENAME = "McShMon_ExData.txt"
    Public Const GOODSOUT_EXPORT_FILENAME = "McGdOut_ExData.txt"
    Public Const GOODSIN_EXPORT_FILENAME = "McGdIn_ExData.txt"
    Public Const PL_EX_FILE_NAME = "TempPLExDataFile.txt"
    Public Const CL_EX_FILE_NAME = "TempCLExDataFile.txt"
    Public Const RECEIVE_BUFFER As Integer = 255
    Public Const WRITE_RETRY As Integer = 3
    Public Const MC70 = "M"
    Public Const RF = "R"

    'Constants used by ActiveFileParser
    Public Const DELIMITER = ","
    Public Const DELETERETRY = 3
    Public Const ACTIVETABLES = "ASNListItems,ASNList,CountListItems," _
                                & "CountList,CountsAndTargets,CreditClaimListItems," _
                                & "CreditClaimList,DirectListItems,DirectList," _
                                & "PickListItems,PickingList,RecallListItems," _
                                & "RecallList,UODOUTItems,UODOUTDetail"
    Public Const CONTROL = "CONTROL.CSV"
    Public Const PICKING = "PICKING.CSV"
    Public Const COUNT = "COUNT.CSV"
    Public Const CREDIT = "CREDIT.CSV"
    Public Const DIRECTS = "DIRECTS.CSV"
    Public Const ASN = "ASN.CSV"
    Public Const SSCUODOT = "SSCUODOT.CSV"
    Public Const ACTIVE_FILE_COUNT As Integer = 7


    'Constants to identify is the user is Supervisor or not
    Public Const SUPERVISOR_YES = "Y"
    Public Const SUPERVISOR_NO = "N"
    Public Const MAX_PASSWORD = 8  ' Maximum length of signon

    Public Const DEFAULT_USERID = "000"
    'v1.1 MCF Change
    Public Const IPCONFIG_FILE_PATH As String = "\Program Files\btstoreapps\IPConfg.xml"

End Class
