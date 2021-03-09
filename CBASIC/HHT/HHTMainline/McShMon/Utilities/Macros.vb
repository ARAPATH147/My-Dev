''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - The controller IP information is stored in IPCONFG.XML
''' </Summary>
'''****************************************************************************

Public Class Macros

    'Barcode Reader 
    Public Const MAX_I2OF5_LEN As Integer = 12
    'Public Const MESSAGE_FILE_PATH = "\Program Files\BTStoreApps\MCShMon_Message.xml"
    'Public Const LOG_FILE_PATH = "\Program Files\BTStoreApps\Logs"
    Public Const CONFIG_FILE_PATH = "\Program Files\BTStoreApps\MCShMon_Config.xml"
    Public Const REF_FILE_STATUS_FILE = "\Program Files\btstoreapps\BatchProcess.Xml"
    Public Const ACT_FILE_STATUS_FILE = "\Program Files\btstoreapps\ActBuildProcess.Xml"
    'v1.1 MCF Change
    Public Const IPCONFIG_FILE_PATH As String = "\Program Files\btstoreapps\IPConfg.xml"
    Public Const MCDOWNLOADER_CONFIG = "\Program Files\btstoreapps\MCDownloader_Config.xml"
    'Constants used by Data Access module
    Public Const EXPORT_FILE_NAME = "McShMon_ExData.txt"
    Public Const PL_EX_FILE_NAME = "TempPLExData.txt"
    Public Const CL_EX_FILE_NAME = "TempCLExData.txt"
    Public Const RECEIVE_BUFFER As Integer = 255
    Public Const WRITE_RETRY As Integer = 3
    Public Const MC70 = "M"
    Public Const RF = "R"
    Public Const SNR_DATETIME_START_INDEX As Integer = 22
    Public Const SNR_DATETIME_LENGTH As Integer = 12
    Public Const GAR_LISTID_START_INDEX As Integer = 3
    Public Const GAR_LISTID_LENGTH As Integer = 3
    Public Const No_REF_FILES As Integer = 10

    'Constants used by ActiveFileParser
    Public Const DELIMITER = ","
    Public Const DELETERETRY = 3
    Public Const ACTIVETABLES = "CountListPlanograms,CountListItems,CountList,CountsAndTargets," _
                                & "PickingListPlanograms,PickListItems,PickingList"
    Public Const FILE_PARSER_STATUS_FILE = "ActiveAndExDataDetails.xml"
    'Public Const ACTIVETABLES = "ASNListItems,ASNList,CountListItems," _
    '                            & "CountList,CountsAndTargets,CreditClaimListItems," _
    '                            & "CreditClaimList,DirectListItems,DirectList," _
    '                            & "PickListItems,PickingList,RecallListItems," _
    '                            & "RecallList,UODOUTItems,UODOUTDetail"

    Public Const CONTROL = "CONTROL.CSV"
    Public Const PICKING = "PICKING.CSV"
    Public Const COUNT = "COUNT.CSV"
    Public Const ACTIVE_FILE_COUNT As Integer = 3
    Public Const WAIT_BEFORE_DOWNLOAD_RETRY As Integer = 1000
    Public Const WAIT_BEFORE_GET_IP As Integer = 4000

    'Constants for Count List Module
    Public Const SENDER_FORM_ACTION As Integer = 1
    Public Const SENDER_PROCESS_ACTION As Integer = 2
    Public Const COUNT_SALES_FLOOR As Integer = 1
    Public Const COUNT_BACK_SHOP As Integer = 2
   
    'Stock File Accuracy  - Added constant for cL
    Public Const COUNT_MBS As String = "MBS"
    Public Const COUNT_PSP As String = "PSP"
    Public Const COUNT_BS_UNKNOWN As String = "BS_UNKNOWN"
    Public Const COUNT_SF As String = "SF"
    Public Const COUNT_OSSR_BS As String = "OSSR_BS"
    Public Const COUNT_OSSR_PSP As String = "OSSR_PSP"
    Public Const SELECTED_INDEX As Integer = 0
    Public Const BS_SELECTED_INDEX As Integer = 0
    Public Const PSP_INDEX As Integer = 1
    Public Const COUNT_LIST_LOCSUMMARY = 1
    Public Const Count_LIST_DISCREPANCY = 2
    Public Const COUNT_LIST_ITEMSUMMARY = 3
    Public Const COUNT_LIST_ITEMDETAILS = 4
    Public Const COUNT_LIST_PROCESSZERO = 5
    Public Const COUNT_LIST_BACKSHOPSUMMARY = 6
    Public Const COUNT_LIST_ITEMSCAN = 7
    Public Const COUNT_LIST_FULLPRICECHECK = 8
    Public Const COUNT_LIST_PROCESSVIEWLIST = 9
    Public Const COUNT_LIST_PROCESSMULTISITESELECT = 10
    Public Const CREATECOUNTID As Integer = 3
    Public Const CREATECOUNTID_LENGTH As Integer = 3
    Public Const CREATEBOOTSCODE As Integer = 16
    Public Const CREATEBOOTSCODE_LENGTH As Integer = 7
    'ambli
    'OSSR
    Public Const COUNT_OSSR As Integer = 3
    'Naveen
    'Supervisor Tag
    Public Const SNR_SUPERVISOR_TAG = "S"

    'Constants to identify is the user is Supervisor or not
    Public Const SUPERVISOR_YES = "Y"
    Public Const SUPERVISOR_NO = "N"

    'Constants for picking list
    Public Const SHELF_MONITOR_PL = "S"
    Public Const FAST_FILL_PL = "F"
    Public Const PL_FINISH As Integer = 1
    'Cod for Auto Fill Quantity added
    Public Const AUTO_FAST_FILL_PL = "A"
    Public Const EXCESS_STOCK_PL = "B"
    Public Const EXCESS_STOCK_PL_SF = "E"
    Public Const EXCESS_STOCK_PL_OSSR = "D"
    Public Const EXCESS_STOCK_OSSR = "C"
    Public Const OSSR_PL = "O"
    Public Const EX_PL_BS = "EB"
    Public Const EX_PL_OSSR = "EO"


    'Constants added for SFA - Excess stock and Create own list
    Public Const SCREEN_SITE_SELECT = "SITE_SELECT"
    Public Const SCREEN_ITEM_CONFIRM = "ITEM_CONFIRM"
    Public Const SCREEN_ITEM_DETAIL = "ITEM_DETAIL"
    Public Const ITEM_DELIMITER = "|"
    Public Const MAIN_BACK_SHOP = "Main Back Shop"
    Public Const PEND_SALES_PLAN = "Pending Sales Plan"
    Public Const SELECT_MBS As Integer = 0
    Public Const SELECT_PSP As Integer = 1
    Public Const STRINGSIZE As Integer = 36
    Public Const SELECT_INDEX_ZERO As Integer = 0
    Public Const MAX_COUNT As Integer = 30
    Public Const START_USER_COUNTLIST As String = "S"
    Public Const END_USER_COUNTLIST As String = "X"
    Public Const USER_COUNT_LIST As String = "U"

    'Added after the new field addition
    Public Const SHOP_FLOOR As String = "S"
    Public Const BACK_SHOP As String = "B"
    Public Const PSP As String = "P"
    Public Const OSSR_PSP As String = "Q"
    'ambli
    'For OSSR
    Public Const OSSR As String = "O"

    Public Const OSSR_COUNT As String = "0000"
    Public Const UPDATE_OSSR_ITEM As String = " "

    'Constants to Display picking List on Picking List home screen
    Public Const SHELF_MONITOR = "SM"
    Public Const FAST_FILL = "FF"
    Public Const EXCESS_STOCK = "EX"
    Public Const AUTO_FAST_FILL = "AF"
    Public Const OSSR_Type = "OS"

    Public Const STATUS_PICKED = "P"
    Public Const STATUS_LIST_ACTIVE = "A"
    Public Const ITEM_CONFIRM_QUIT = "ITEM_CONFIRM_QUIT"
    Public Const ITEM_DETAILS_QUIT = "ITEM_DETAILS_QUIT"

    'Export Data Writing
    Public Const PRINT_BATCH = "B"
    Public Const PRINT_INSTANT = "I"
    Public Const PRINT_LOCAL = "L"

    Public Const CLX_COMMIT_YES = "Y"
    Public Const CLX_COMMIT_NO = "N"

    Public Const PLC_SM_FLAG = "Y"
    Public Const PLC_FF_FLAG = "N"
    Public Const PLC_EX_FLAG = "B"
    Public Const PLC_OSSR_FLAG = "O"

    Public Const PLX_COMPLETE_YES = "Y"

    Public Const PLX_COMPLETE_NO = "N"

    Public Const PLX_COMPLETE_OSSR = "O"

    'Constants for Space Planner
    Public Const SP_CORE = "Core"
    Public Const SP_SALESPLAN = "SalesPlan"
    Public Const LIVE_PLANNER = "LP"
    Public Const SEARCH_PLANNER = "SP"

    'Constants for Item Status
    Public Const STATUS_ACTIVE = "Active"
    Public Const STATUS_DELETED = "Deleted"
    Public Const STATUS_DISCONTINUED_OFFPLANNER = "Discontinued (off planner)"
    Public Const STATUS_RECALLED = "Recalled"
    Public Const STATUS_OUTSTANDING_CANCEL = "Outstanding order cancelled"
    Public Const STATUS_DISCONTINUED_ONPLANNER = "Discontinued (on planner)"
    Public Const STATUS_DISCONTINUED = "Discontinued"
    Public Const STATUS_SUSPENDED = "Suspended"
    Public Const STATUS_UNSUPPLIABLE = "Unsuppliable"

    'For Print SEL and Clearance label printing. *****Govindh Dec 2009
    Public Const SEL_STANDARD = "0"
    Public Const SEL_WAS_NOW = "1"
    Public Const SEL_WAS_WAS_NOW = "2"
    Public Const SEL_CIP_CLEARANCE = "3"
    Public Const SEL_CLEARANCE = "4"
    Public Const SEL_CATCH_WEIGHT_CLEARANCE = "5"
    Public Const POUND_SYMBOL = "£"
    Public Const EURO_SYMBOL = "€"
    Public Const PENCE_SYMBOL = "p"
    Public Const CENTS_SYMBOL = "c"
    Public Const CIP_MARKER As String = "CIP"
    Public Const FONTS_DIRECTORY = "\Program Files\BTStoreApps\"
    Public Const STANDARD_TEMPLATE_FILE = "\Program Files\BTStoreApps\SELPPCN0.BIN"
    Public Const WASNOW_TEMPLATE_FILE = "\Program Files\BTStoreApps\SELPPCN1.BIN"
    Public Const WASWASNOW_TEMPLATE_FILE = "\Program Files\BTStoreApps\SELPPCN2.BIN"
    Public Const CLEARANCE_TEMPLATE_FILE = "\Program Files\BTStoreApps\SELPPCN3.BIN"
    Public Const PAINKILLER_TEMPLATE_FILE = "\Program Files\BTStoreApps\SELPPCN4.BIN"
    Public Const WEEE_TEMPLATE_FILE = "\Program Files\BTStoreApps\SELPPCW0.BIN"
    Public Const RF_CLEARANCE_TEMPLATE_FILE = "\Program Files\BTStoreApps\CLEARL01.BIN"
    Public Const CW_CLEARANCE_TEMPLATE_FILE = "\Program Files\BTStoreApps\CATCHW01.BIN"
    Public Const APPLICATION_IDENTIFIER = "ACTBUILD"
    Public Const APPLICATION_OUTPIPE = "ACTBDOUT"
    Public Const APPLICATION_INPIPE = "ACTBDINB"
    Public Const MAX_PASSWORD = 6  ' Maximum length of signon

    'Constant for RedemptionFlag
    Public Const Redemption_True = "*"
    Public Const Redemption_False = ""
    Public Const RECONNECT_ATTEMPTS = 3
    Public Const CONNECTIVITY_MESSAGE = "Retry {0} of 3 to reconnect. " + vbCr + "Please wait until successful or select Cancel to quit. Last item scanned may not be checked."

#If RF Then
    Public Const CANCEL_SLEEP_TIME = 100
    Public Const CANCEL_CLICK = "\Windows\Alarm2.wav"
    Public Const CANCEL_TEXT = "Cancelling Reconnect Operation... Please Wait."
    Public Const DEFAULT_VALUE_PLC = "S0000    "
    Public Const MACID = "000000000000"
    Public Const USER_ABORT = "USER ABORTED THE CURRENT OPERATION"
    Public Const CONNECTION_LOST_EXCEPTION_MESSAGE = "LOSS OF CONNECTIVITY"
    Public Const CONNECTION_REGAINED = "CONNECTION REGAINED - ABORT CURRENT OPERATION"
#End If
End Class
