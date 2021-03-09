''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Controller IP looked up from IPCONFG.XML file
''' </Summary>
'''****************************************************************************
''' No:      Author            Date            Description 
''' 1.2  Christopher Kitto  14/04/2015      Modified as part of DALLAS project.
'''           (CK)                          Added a new constant WHUOD for the active 
'''                                         file WHUOD.CSV under class Macros. 
'''                                         Added new constant BURTON under class
'''                                         ContainerName. Changed active file count.
'''                                        'Pallets' name replaced with 'SSC Pallets' 
'''                                         Added the table name DALUODList in the constant
'''                                         string variable ACTIVETABLES. Added constant
'''                                         for Burton in class ContainerType                             
'''      Kiran Krishnan     05/05/2015      Added new constants for Dallas Project
'''                                         to Class Macros                          
''' ********************************************************************************
''' No:      Author            Date            Description 
''' 1.3  Kiran Krishnan      15/04/2016     The 13C release change to add Boots.com
'''                                         related varaiable DOTCOM_ITEM_QTY was 
'''                                         missing in HHT Main. Merged it back.
''' ********************************************************************************
Public Class Macros

    'Barcode Reader 
    Public Const MAX_I2OF5_LEN As Integer = 12

   
    Public Const APPCONFIG_FILE_PATH As String = "\Program Files\btstoreapps"
    'v1.1 MCF Change
    Public Const IPCONFIG_FILE_PATH As String = "\Program Files\btstoreapps\IPConfg.xml"
    'Constants used by Data Access module
    Public Const CONFIG_FILE_PATH As String = "\Program Files\btstoreapps\McGdIn_Config.xml"
    Public Const MESSAGE_FILE_PATH As String = "Program Files\btstoreapps\McGdIn_Message.xml"
#If RF Then
    Public Const CANCEL_CLICK = "\Windows\Alarm2.wav"
    Public Const RECEIVE_BUFFER As Integer = 255
    Public Const MACID = "000000000000"
    Public Const Redemption_True = "*"
    Public Const Redemption_False = ""
    Public Const RECONNECT_ATTEMPTS = 3
    Public Const CANCEL_SLEEP_TIME = 100
    Public Const CONNECTIVITY_MESSAGE = "Retry {0} of 3 to reconnect. " + vbCr + "Please wait until successful or select Cancel to quit."
#End If


    Public Const LOG_FILE_PATH As String = "\Program Files\btstoreapps\Logs\"

    Public Const EXPORT_FILE_NAME As String = "McGdIn_ExData.txt"

    Public Const WRITE_RETRY As Integer = 3
    Public Const MC70 As String = "M"
    Public Const RF As String = "R"
    Public Const MAX_PASSWORD As Integer = 6  ' Maximum length of signon

    Public Const MIN_DRIVERID As Integer = 8

    Public Const CRATEID As String = "CrateId"
    Public Const DOLLYID As String = "DollyId"
    Public Const ROLLYCAGEID As String = "RollyCageId"
    Public Const PALLETSID As String = "PalletsId"
    Public Const OUTERSID As String = "OutersId"
    Public Const Y As String = "Y"
    Public Const N As String = "N"
    Public Const NOBADGE As String = "99999999"
    Public Const OUTOFHOURS As String = "00000000"
    Public Const EXPECTED As String = "E"
    Public Const OUTSTANDING As String = "O"
    'CR for Future UOD
    Public Const FUTURE As String = "F"
    Public Const TODAY As String = "T"
    Public Const BOOKEDIN As String = "B"
    Public Const UNBOOKED As String = "U"
    Public Const AUDITED As String = "A"
    Public Const ZEROUOD As String = "0000000000"
    Public Const BLEN As Integer = 15
    Public Const RLEN As Integer = 7
    'V1.2 KK
    'Added as part of Dallas Positive Receiving Project
    Public Const RECEIPTED As String = "R"
    Public Const UNRECEIPTED As String = "U"
    Public Const BANKED As String = "B"
    'V1.3 KK
    'Merged back missing Boots.com not on file related variable
    Public Const DOTCOM_ITEM_QTY As String = "001"

#Region "NRF"
#If NRF Then


    'Public Const MESSAGE_FILE_PATH As String = "\Program Files\Goodsin\Goodsin_Message.xml"
    'Public Const LOG_FILE_PATH As String = "\Program Files\Goodsin\Logs\"
    'Public Const CONFIG_FILE_PATH As String = "\Program Files\Goodsin\Goodsin_Config"
    Public Const MCDOWNLOADER_CONFIG = "\Program Files\btstoreapps\MCDownloader_Config.xml"
    'Public Const EXPORT_FILE_NAME As String = "McGdIn_ExData.txt"
    Public Const SNR_DATETIME_START_INDEX As Integer = 22
    Public Const SNR_DATETIME_LENGTH As Integer = 12
    Public Const WAIT_BEFORE_DOWNLOAD_RETRY As Integer = 1000
    Public Const WAIT_BEFORE_GET_IP As Integer = 4000

    Public Const RECEIVE_BUFFER As Integer = 1056



    'Constants used by ActiveFileParser
    Public Const DELIMITER As Char = ","
    Public Const DELETERETRY As Integer = 3

    Public Const ConnectionString As String = "\Program Files\MCGoodsIn\BTCDB.sdf"
    ' v1.2 - CK
    ' Added the name of table DALUODList in the constant variable ACTIVETABLES
    ' Public Const ACTIVETABLES As String = "ASNListItems,ASNList,DirectListItems,DirectList," _
    '                            & "UODOUTItems,UODOUTDetail,UODOUTSummary"
    Public Const ACTIVETABLES As String = "ASNListItems,ASNList,DirectListItems,DirectList," _
                                & "UODOUTItems,UODOUTDetail,UODOUTSummary,DALUODList"
    Public Const DIRECTS As String = "DIRECTS.CSV"
    Public Const ASN As String = "ASN.CSV"
    Public Const SSCUODOT As String = "SSCUODOT.CSV"
    Public Const CONTROL As String = "CONTROL.CSV"
    ' v1.2 - CK
    ' Added a constant variable for WHUOD.CSV
    Public Const WHUOD As String = "WHUOD.CSV"
    ' v1.2 - CK
    ' Active file count changed to 5 since new WHUOD.CSV introduced
    ' Public Const ACTIVE_FILE_COUNT As Integer = 4
    Public Const ACTIVE_FILE_COUNT As Integer = 5

    Public Const APPLICATION_IDENTIFIER As String = "ACTBUILD"
    Public Const APPLICATION_OUTPIPE As String = "ACTBDOUT"
    Public Const APPLICATION_INPIPE As String = "ACTBDINB"

#End If
#End Region
End Class
''' <summary>
''' Class for getting Container Type
''' </summary>
''' <remarks></remarks>
Public Class ContainerType
    Public Const Dolly As String = "D"
    Public Const Crate As String = "C"
    Public Const Pallet As String = "P"
    'V1.2 - KK
    Public Const Burton As String = "B"
    Public Const RollCage As String = "R"
    Public Const Outer As String = "O"
    Public Const IST As String = "I"
End Class
''' <summary>
'''  Class for getting Container Name
''' </summary>
''' <remarks></remarks>
Public Class ContainerName
    Public Const Dolly As String = "Dollys"
    Public Const Crate As String = "Crates"
    Public Const Rollcage As String = "RoCos"
    ' v1.2 - CK
    ' "Pallets" name replaced with "SSC Pallets" 
    ' Public Const Pallet As String = "Pallets"
    Public Const Pallet As String = "SSC Pallets"
    Public Const Outer As String = "Travel Outers"
    Public Const IST As String = "ISTs"
    ' v1.2 - CK
    ' Active file count changed to 5 since new WHUOD.CSV introduced
    Public Const Burton As String = "Burton Pallets"
End Class
Public Class BookInOrder
    Public Const Order As String = "Order"
    Public Const NoOrder As String = "NoOrder"
End Class
Public Class Status
    Public Const UnBooked As String = "U"
    Public Const Booked As String = "B"
    Public Const Audited As String = "A"
End Class
Public Class ScanType
    Public Const Booked As String = "B"
    Public Const Audited As String = "A"

    Public Const ReturnedMisdirect As String = "N"

    Public Const LateUOD As String = "L"
End Class
Public Class ScanLevel
    Public Const Item As String = "I"
    Public Const Delivery As String = "D"

End Class
Public Class SupplierType
    Public Const ASN As String = "A"
    Public Const Directs As String = "D"
End Class
Public Class Quantity
    Public Const Zero As String = "0"
    Public Const One As String = "1"
End Class
Public Class ContainerNam
    Public Const Dolly As String = "Dolly"
    Public Const Crate As String = "Crate"
    Public Const Rollcage As String = "RoCo"
    ' V1.2 - CK
    ' "Pallet" name replaced with "SSC Pallet"
    ' Public Const Pallet As String = "Pallet"
    Public Const Pallet As String = "SSC Pallet"
    Public Const Outer As String = " Travel Outer"
    Public Const IST As String = "IST"
End Class
