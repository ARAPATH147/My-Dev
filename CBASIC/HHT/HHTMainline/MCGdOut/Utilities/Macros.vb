'''***************************************************************
''' <FileName>Macros.vb</FileName>
''' <summary>
''' Stores the Macros
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Controller IP looked up from IPCONFG.XML file
''' </Summary>
'''****************************************************************************
Public Class Macros

    Public Const CONFIG_FILE_PATH = "\Program Files\btstoreapps\MCGdOut_Config.xml"
    Public Const REF_FILE_STATUS_FILE = "\Program Files\btstoreapps\BatchProcess.Xml"
    Public Const ACT_FILE_STATUS_FILE = "\Program Files\btstoreapps\ActBuildProcess.Xml"
    Public Const MCDOWNLOADER_CONFIG = "\Program Files\btstoreapps\MCDownloader_Config.xml"
    'v1.1 MCF Change
    Public Const IPCONFIG_FILE_PATH As String = "\Program Files\btstoreapps\IPConfg.xml"

    'System Testng Workflow xml
    Public Const WORKFLOW_PATH = "\Program Files\btstoreapps\McGdOut_WorkFlow.xml"

    Public Const No_REF_FILES As Integer = 10
    Public Const AUTHMAXSIZE = 8
    Public Const DESTSTOREMAXSIZE = 4

    'Constants used by Data Access module
    Public Const EXPORT_FILE_NAME = "MCGdOut_ExData.txt"
    Public Const PL_EX_FILE_NAME = "TempPLExDataFile.txt"
    Public Const CL_EX_FILE_NAME = "TempCLExDataFile.txt"
    Public Const RECEIVE_BUFFER As Integer = 255
    Public Const WRITE_RETRY As Integer = 3
    Public Const MC70 = "M"
    Public Const RF = "R"
    Public Const SNR_DATETIME_START_INDEX As Integer = 22
    Public Const SNR_DATETIME_LENGTH As Integer = 12
    Public Const GAR_LISTID_START_INDEX As Integer = 3
    Public Const GAR_LISTID_LENGTH As Integer = 3

    'Constants used by ActiveFileParser
    Public Const DELIMITER = ","
    Public Const DELETERETRY = 3
    Public Const ACTIVETABLES = "CreditClaimListItems, CreditClaimList"
    Public Const CREDIT = "CREDIT.CSV"
    Public Const ACTIVE_FILE_COUNT As Integer = 1
    Public Const WAIT_BEFORE_DOWNLOAD_RETRY As Integer = 1000
    Public Const WAIT_BEFORE_GET_IP As Integer = 4000


    'Barcode Reader
    ' Public Const MAX_I2OF5_LEN As Integer = 12
    Public Const MIN_I2OF5_LEN As Integer = 12
    Public Const MAX_I2OF5_LEN As Integer = 22 'for Clearance Label

    'Constants for Item Status
    Public Const STATUS_ACTIVE = "Active"
    Public Const STATUS_DELETED = "Deleted"
    Public Const STATUS_DISCONTINUED = "Discontinued"
    Public Const STATUS_RECALLED = "Recalled"

    'Recall item status
    Public Const RECALL_ITEM_PICKED = "Y"
    Public Const RECALL_ITEM_UNPICKED = "N"
    Public Const RECALL_ITEM_ACTIONED = "A"
    Public Const RECALL_ITEM_UNACTIONED = "P"
    Public Const RECALL_RCF_ITEM_COUNT As Integer = 10

    'System Testing SFA -Authentiction - DEF #43
    Public Const STOCK_SPECIALIST = "Y"
    'Recall Types
    Public Const RECALL_EMERGENCY = "E"
    Public Const RECALL_WITHDRAWN = "W"
    Public Const RECALL_100RETURN = "R"

    'Currency Symbols
    Public Const POUND_SYMBOL = "£"
    Public Const EURO_SYMBOL = "€"

    'Public Constants used by User Authentication
    Public Const MAX_PASSWORD = 6  ' Maximum length of signon
    Public Const STQ_UOD_NUMBER = "99998888"
    Public Const RECONNECT_ATTEMPTS = 3
    Public Const TIMEOUT_RECONNECT_MESSAGE = "Retry {0} of 3 to reconnect. Please wait until successful."
    Public Const CONNECTIVITY_MESSAGE_START = "Retry {0} of 3 to reconnect. " + vbCr + "Please wait until successful or select Cancel to quit."
    Public Const CONNECTIVITY_MESSAGE_MIDDLE = "Retry {0} of 3 to reconnect. " + vbCr + "Please wait until successful or select Cancel to quit. The last item may not be saved."
    Public Const CONNECTIVITY_MESSAGE_END = "Retry {0} of 3 to reconnect. " + vbCr + "Please wait until successful or select Cancel to quit. Your list has been saved."
    Public Const CONNECTIVITY_ITEM_ADD = "Retry {0} of 3 to reconnect. " + vbCr + "Please wait until successful or select Cancel to quit. The last item may not be saved."
    Public Const CONNECTIVITY_ITEM_VOID = "Retry {0} of 3 to reconnect. " + vbCr + "Please wait until successful or select Cancel to quit. The last void item may not be saved."
    Public Const CONNECTION_LOSS_EXCEPTION = "Connection Lost, Close Module"
    Public Const CONNECTION_REGAINED = "Connection Lost and Reestablished, Abort Currrent Operation"
    Public Const CANCEL_TEXT = "Cancelling Reconnect Operation... Please Wait."
    Public Const CANCEL_CLICK = "\Windows\Alarm2.wav"
    Public Const CANCEL_SLEEP_TIME = 300
    Public Const CONNECTIVITY_TIMEOUTCANCEL = "Connection Time Out, Close Module"
End Class
