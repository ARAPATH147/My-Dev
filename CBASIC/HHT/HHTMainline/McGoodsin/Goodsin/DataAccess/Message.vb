'''***************************************************************
'''* Modification Log 
'''******************************************************************************* 
'''* No:      Author            Date            Description 
'''* 1.1     Christopher Kitto  09/04/2015   Modified as part of DALLAS project.
'''                  (CK)                 Added new classes DAL, DAD and DAE under
'''                                       the class Message
'''********************************************************************************
Public Class Message
    Public Const INITIALPOINTER As Integer = 0
    Public Const ENDTPOINTER As Integer = -1
    Public Const ENDPOINTER_STR As String = "-1    "
    Public Const ACK As String = "ACK"
    Public Const NAK As String = "NAK"
    Public Const GIR As String = "GIR"
    'Public Const GIB As String = "GIB"

    Public Class GIA

        Public Const TID As Integer = 3
        Public Const TID_OFFSET As Integer = 0

        Public Const ID As Integer = 3
        Public Const ID_OFFSET As Integer = 3

        Public Const DELIVERYTYPE As Integer = 1
        Public Const DELIVERYTYPE_OFFSET As Integer = 6

        Public Const [FUNCTION] As Integer = 1
        Public Const FUNCTION_OFFSET As Integer = 7

        Public Const REQUESTTYPE As Integer = 1
        Public Const REQUESTTYPE_OFFSET As Integer = 8

        Public Const PERIOD As Integer = 1
        Public Const PERIOD_OFFSET As Integer = 9

        Public Const POINTER As Integer = 6
        Public Const POINTER_OFFSET As Integer = 10

    End Class
    Public Class GIB
        Public Const TID As Integer = 3
        Public Const TID_OFFSET As Integer = 0

        Public Const ID As Integer = 3
        Public Const ID_OFFSET As Integer = 3

        Public Const RESPONSETYPE As Integer = 1
        Public Const RESPONSETYPE_OFFSET As Integer = 6


        'For Config GIB
        Public Const DIRECTS_ACTIVE_OFFSET As Integer = 7
        Public Const DIRECTS_ACTIVE As Integer = 1

        Public Const POS_UOD_ACTIVE_OFFSET As Integer = 8
        Public Const POS_UOD_ACTIVE As Integer = 1

        Public Const ASN_ACTIVE_OFFSET As Integer = 9
        Public Const ASN_ACTIVE As Integer = 1

        Public Const ONIGHT_DELIV_OFFSET As Integer = 10
        Public Const ONIGHT_DELIV As Integer = 1

        Public Const ONIGHT_SCAN_OFFSET As Integer = 11
        Public Const ONIGHT_SCAN As Integer = 1

        Public Const SCAN_BATCH_SIZE_OFFSET As Integer = 12
        Public Const SCAN_BATCH_SIZE As Integer = 3


        'For Other GIB
        Public Const COUNT_OFFSET As Integer = 7
        Public Const COUNT As Integer = 2

        Public Const POINTER_OFFSET As Integer = 9
        Public Const POINTER As Integer = 6

        'value to be used to interate through the rest of elements
        Public Const HEADERTOTAL As Integer = 15 '(52 chars x 20 = 1040 bytes) + 15
        Public Const COUNT_MAX As Integer = 20
        Public Const TRAILERTOTAL As Integer = 52


        'The GIB values start here

        Public Const IDENTIFIER_OFFSET As Integer = 15
        Public Const IDENTIFIER As Integer = 10

        Public Const SEQUENCE_OFFSET As Integer = 25
        Public Const SEQUENCE As Integer = 5
        Public Const NAME_OFFSET As Integer = 30
        Public Const NAME As Integer = 20

        Public Const SUPPLIERTYPE_OFFSET As Integer = 50
        Public Const SUPPLIERTYPE As Integer = 1
        Public Const CONTENTTYPE_OFFSET As Integer = 51
        Public Const CONTENTTYPE As Integer = 1
        Public Const EXPECTEDDATE_OFFSET As Integer = 52
        Public Const EXPECTEDDATE As Integer = 8
        Public Const BOOKEDIN_OFFSET As Integer = 60
        Public Const BOOKEDIN As Integer = 1
        Public Const QUANTITY_OFFSET As Integer = 61
        Public Const QUANTITY As Integer = 6

    End Class
    Public Class GIQ

        'Generic Details
        Public Const FIRSTPOINTER As Integer = 0
        'Generic details ends
        Public Const TID As Integer = 3
        Public Const TID_OFFSET As Integer = 0

        Public Const ID As Integer = 3
        Public Const ID_OFFSET As Integer = 3

        Public Const DELIVERYTYPE As Integer = 1
        Public Const DELIVERYTYPE_OFFSET As Integer = 6

        Public Const [FUNCTION] As Integer = 1
        Public Const FUNCTION_OFFSET As Integer = 7

        Public Const SELECTEDCODE_OFFSET As Integer = 8
        Public Const SELECTEDCODE As Integer = 20

        Public Const SEQUENCE_OFFSET As Integer = 28
        Public Const SEQUENCE As Integer = 5

        Public Const REQUESTTYPE_OFFSET As Integer = 33
        Public Const REQUESTTYPE As Integer = 1

        Public Const CONTENTTYPE_OFFSET As Integer = 34
        Public Const CONTENTTYPE As Integer = 1

        Public Const SUPPLIERTYPE_OFFSET As Integer = 35
        Public Const SUPPLIERTYPE As Integer = 1

        Public Const POINTER_OFFSET As Integer = 36
        Public Const POINTER As Integer = 6
    End Class
    Public Class GIR_B

        Public Const TID As Integer = 3
        Public Const TID_OFFSET As Integer = 0

        Public Const ID As Integer = 3
        Public Const ID_OFFSET As Integer = 3

        Public Const SELECTEDCODE_OFFSET As Integer = 6
        Public Const SELECTEDCODE As Integer = 20

        'splitting selected code to child and parent
        Public Const CHILDCODE_OFFSET As Integer = 6
        Public Const CHILDCODE As Integer = 10

        Public Const PARENTCODE_OFFSET As Integer = 16
        Public Const PARENTCODE As Integer = 10
        'splitting ends

        Public Const RESPONSETYPE_OFFSET As Integer = 26
        Public Const RESPONSETYPE As Integer = 1

        Public Const DESPATCHDATE_OFFSET As Integer = 27
        Public Const DESPATCHDATE As Integer = 6

        Public Const OUTERTYPE_OFFSET As Integer = 33
        Public Const OUTERTYPE As Integer = 1

        Public Const CONTENTTYPE_OFFSET As Integer = 34
        Public Const CONTENTTYPE As Integer = 1

        Public Const UODREASON_OFFSET As Integer = 35
        Public Const UODREASON As Integer = 1

        Public Const STATUS_OFFSET As Integer = 36
        Public Const STATUS As Integer = 1

        Public Const BOLUOD_OFFSET As Integer = 37
        Public Const BOLUOD As Integer = 1

        Public Const ORDERNUM_OFFSET As Integer = 38
        Public Const ORDERNUM As Integer = 5

        Public Const ORDERSUFFIX_OFFSET As Integer = 43
        Public Const ORDERSUFFIX As Integer = 1

        Public Const BUSCENTRE_OFFSET As Integer = 44
        Public Const BUSCENTRE As Integer = 1

    End Class
    Public Class GIR_S

        Public Const TID As Integer = 3
        Public Const TID_OFFSET As Integer = 0

        Public Const ID As Integer = 3
        Public Const ID_OFFSET As Integer = 3

        Public Const SELECTEDCODE_OFFSET As Integer = 6
        Public Const SELECTEDCODE As Integer = 20

        'splitting selected code to child and parent
        Public Const CHILDCODE_OFFSET As Integer = 6
        Public Const CHILDCODE As Integer = 10

        Public Const PARENTCODE_OFFSET As Integer = 16
        Public Const PARENTCODE As Integer = 10
        'splitting ends

        Public Const RESPONSETYPE_OFFSET As Integer = 26
        Public Const RESPONSETYPE As Integer = 1

        Public Const SUPPLIERNO_OFFSET As Integer = 27
        Public Const SUPPLIERNO As Integer = 6

        Public Const SUPPLIERNAME_OFFSET As Integer = 33
        Public Const SUPPLIERNAME As Integer = 10

        Public Const SUPPLIERTYPE_OFFSET As Integer = 43
        Public Const SUPPLIERTYPE As Integer = 1

    End Class
    Public Class GIR_F
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3


        Public Const SELECTEDCODE_OFFSET As Integer = 6
        Public Const SELECTEDCODE As Integer = 20

        'splitting selected code to child and parent
        Public Const CHILDCODE_OFFSET As Integer = 6
        Public Const CHILDCODE As Integer = 10

        Public Const PARENTCODE_OFFSET As Integer = 16
        Public Const PARENTCODE As Integer = 10
        'splitting ends

        Public Const RESPONSETYPE_OFFSET As Integer = 26
        Public Const RESPONSETYPE As Integer = 1

        Public Const DESPATCHDATE_OFFSET As Integer = 27
        Public Const DESPATCHDATE As Integer = 6

        Public Const OUTERTYPE_OFFSET As Integer = 33
        Public Const OUTERTYPE As Integer = 1

        Public Const CONTENTTYPE_OFFSET As Integer = 34
        Public Const CONTENTTYPE As Integer = 1

        Public Const UODREASON_PART1_OFFSET As Integer = 35
        Public Const UODREASON_PART1 As Integer = 1

        Public Const STATUS_OFFSET As Integer = 36
        Public Const STATUS As Integer = 1

        Public Const BOLUOD_OFFSET As Integer = 37
        Public Const BOLUOD As Integer = 1

        Public Const ORDERNUM_OFFSET As Integer = 38
        Public Const ORDERNUM As Integer = 5

        Public Const ORDERSUFFIX_OFFSET As Integer = 43
        Public Const ORDERSUFFIX As Integer = 1

        Public Const BUSCENTRE_OFFSET As Integer = 44
        Public Const BUSCENTRE As Integer = 1

        Public Const ESTDELIVERYDATE_OFFSET As Integer = 45
        Public Const ESTDELIVERYDATE As Integer = 6

        Public Const DRIVERBADGE_OFFSET As Integer = 51
        Public Const DRIVERBADGE As Integer = 8

        Public Const DRIVERCHECKINDATE_OFFSET As Integer = 59
        Public Const DRIVERCHECKINDATE As Integer = 6


        Public Const DRIVERCHECKINTIME_OFFSET As Integer = 65
        Public Const DRIVERCHECKINTIME As Integer = 4


        Public Const STOREOPID_OFFSET As Integer = 69
        Public Const STOREOPID As Integer = 8


        Public Const BOOKINDATE_OFFSET As Integer = 77
        Public Const BOOKINDATE As Integer = 6

        Public Const BOOKINTIME_OFFSET As Integer = 83
        Public Const BOOKINTIME As Integer = 4

        Public Const COUNT_OFFSET As Integer = 87
        Public Const COUNT As Integer = 2

        Public Const POINTER_OFFSET As Integer = 89
        Public Const POINTER As Integer = 6



        'value to be used to interate through the rest of elements
        Public Const HEADERTOTAL As Integer = 95 '(52 chars x 20 = 1040 bytes) + 15
        Public Const COUNT_MAX As Integer = 16
        Public Const TRAILERTOTAL As Integer = 53

        'Part2 starts here
        Public Const IDENTIFIER_OFFSET As Integer = 95
        Public Const IDENTIFIER As Integer = 10

        Public Const NAME_OFFSET As Integer = 105
        Public Const NAME As Integer = 13
        Public Const BOOKEDIN_OFFSET As Integer = 118
        Public Const BOOKEDIN As Integer = 1
        Public Const CONTENTTYPE_PART2_OFFSET As Integer = 119
        Public Const CONTENTTYPE_PART2 As Integer = 1

        Public Const DESCRIPTION_OFFSET As Integer = 120
        Public Const DESCRIPTION As Integer = 20

        Public Const QUANTITY_OFFSET As Integer = 140
        Public Const QUANTITY As Integer = 6

        Public Const SEQUENCE_OFFSET As Integer = 146
        Public Const SEQUENCE As Integer = 2


    End Class
    Public Class ENQ
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const SCANCODE_OFFSET As Integer = 13
        Public Const SCANCODE As Integer = 13

        Public Const STKFIGREQD_OFFSET As Integer = 1
        Public Const STKFIGREQD As Integer = 1

        Public Const OSSRFLAG_OFFSET As Integer = 1
        Public Const OSSRFLAG As Integer = 1

        Public Const PLANNER_OFFSET As Integer = 1
        Public Const PLANNER As Integer = 1

    End Class
    Public Class EQR
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const BCDE_OFFSET As Integer = 3
        Public Const BCDE As Integer = 7

        Public Const PARENT_OFFSET As Integer = 10
        Public Const PARENT As Integer = 7

        Public Const DESC_OFFSET As Integer = 17
        Public Const DESC As Integer = 20

        Public Const PRICE_OFFSET As Integer = 37
        Public Const PRICE As Integer = 6

        Public Const LONGDESC_OFFSET As Integer = 43
        Public Const LONGDESC As Integer = 45

        Public Const STATUS_OFFSET As Integer = 88
        Public Const STATUS As Integer = 1

        Public Const SUPPLY_OFFSET As Integer = 89
        Public Const SUPPLY As Integer = 1

        Public Const REDEEM_OFFSET As Integer = 90
        Public Const REDEEM As Integer = 1

        Public Const STOCKFIG_OFFSET As Integer = 91
        Public Const STOCKFIG As Integer = 6

        Public Const PCHKTARGET_OFFSET As Integer = 97
        Public Const PCHKTARGET As Integer = 4

        Public Const PCHKDONE_OFFSET As Integer = 101
        Public Const PCHKDONE As Integer = 4

        Public Const EMUPRICE_OFFSET As Integer = 105
        Public Const EMUPRICE As Integer = 6

        Public Const PRIMCURR_OFFSET As Integer = 111
        Public Const PRIMCURR As Integer = 1

        Public Const BARCODE_OFFSET As Integer = 112
        Public Const BARCODE As Integer = 13

        Public Const ACTVDEAL_OFFSET As Integer = 125
        Public Const ACTVDEAL As Integer = 1

        Public Const CHECKACCEPTED_OFFSET As Integer = 126
        Public Const CHECKACCEPTED As Integer = 1

        Public Const REJECTMESSAGE_OFFSET As Integer = 127
        Public Const REJECTMESSAGE As Integer = 14

        Public Const BC_OFFSET As Integer = 141
        Public Const BC As Integer = 1

        Public Const BCDESC_OFFSET As Integer = 142
        Public Const BCDESC As Integer = 14

        Public Const OSSRITEM_OFFSET As Integer = 156
        Public Const OSSRITEM As Integer = 1

        Public Const PLANLOC1_OFFSET As Integer = 217
        Public Const PLANLOC1 As Integer = 3

        Public Const PLANLOC2_OFFSET As Integer = 220
        Public Const PLANLOC2 As Integer = 3

        Public Const RECALLITEM_OFFSET As Integer = 223
        Public Const RECALLITEM As Integer = 1

        Public Const MARKDOWN_OFFSET As Integer = 224
        Public Const MARKDOWN As Integer = 1

        Public Const PGGROUP_OFFSET As Integer = 225
        Public Const PGGROUP As Integer = 6

        'Recalls CR
        Public Const RECALLTYPE_OFFSET As Integer = 231
        Public Const RECALLTYPE As Integer = 1

    End Class
    Public Class GIF
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const DELIVERTTYPE_OFFSET As Integer = 6
        Public Const DELIVERYTYPE As Integer = 1

        Public Const [FUNCTION] As Integer = 1
        Public Const FUNCTION_OFFSET As Integer = 7

        Public Const SCANCODE_OFFSET As Integer = 8
        Public Const SCANCODE As Integer = 20

        Public Const DESPATCHDATE_OFFSET As Integer = 28
        Public Const DESPATCHDATE As Integer = 6

        Public Const SCANTYPE_OFFSET As Integer = 34
        Public Const SCANTYPE As Integer = 1

        Public Const SCANLEVEL_OFFSET As Integer = 35
        Public Const SCANLEVEL As Integer = 1

        Public Const SCANDATE_OFFSET As Integer = 36
        Public Const SCANDATE As Integer = 6

        Public Const SCANTIME_OFFSET As Integer = 42
        Public Const SCANTIME As Integer = 4

        Public Const DRIVERBADGE_OFFSET As Integer = 46
        Public Const DRIVERBADGE As Integer = 8

        Public Const GITNOTE_OFFSET As Integer = 54
        Public Const GITNOTE As Integer = 1

        Public Const BATCHRESCAN_OFFSET As Integer = 55
        Public Const BATCHRESCAN As Integer = 1

        Public Const BARCODE_OFFSET As Integer = 56
        Public Const BARCODE As Integer = 13

        Public Const QUANTITY_OFFSET As Integer = 69
        Public Const QUANTITY As Integer = 6

        Public Const SEQUENCE_OFFSET As Integer = 75
        Public Const SEQUENCE As Integer = 2

    End Class
    Public Class GIX
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const DELIVERTTYPE_OFFSET As Integer = 6
        Public Const DELIVERYTYPE As Integer = 1

        Public Const FUNCTION_OFFSET As Integer = 7
        Public Const [FUNCTION] As Integer = 1

        Public Const ABORT_OFFSET As Integer = 8
        Public Const ABORT As Integer = 1
    End Class
    Public Class SOR
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const PSWD_OFFSET As Integer = 6
        Public Const PSWD As Integer = 3

        Public Const APPID_OFFSET As Integer = 9
        Public Const APPID As Integer = 3


    End Class
    Public Class SNR
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const AUTH_OFFSET As Integer = 6
        Public Const AUTH As Integer = 1

        Public Const UNAME_OFFSET As Integer = 7
        Public Const UNAME As Integer = 15

        Public Const DATETIME_OFFSET As Integer = 22
        Public Const DATETIME As Integer = 12

        Public Const PRTNUM_OFFSET As Integer = 30
        Public Const PRTNUM As Integer = 6

        Public Const PRTDESC_OFFSET As Integer = 40
        Public Const PRTDESC As Integer = 20 * 9


    End Class

    ' V1.1 - CK
    ' Added new class DAL

    ''' <summary>
    ''' Class with constant variables to hold the offset and length of 
    ''' DAL message fields
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DAL

        Public Const TID As Integer = 3
        Public Const TID_OFFSET As Integer = 0

        Public Const ID As Integer = 3
        Public Const ID_OFFSET As Integer = 3

        Public Const NEXTRECORDNO As Integer = 4
        Public Const NEXTRECORDNO_OFFSET As Integer = 6

    End Class

    ' V1.1 - CK
    ' Added new class DAD

    ''' <summary>
    ''' Class with constant variables to hold the offset and length of 
    ''' DAD message fields
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DAD

        Public Const TID As Integer = 3
        Public Const TID_OFFSET As Integer = 0

        Public Const NEXTRECORDNO As Integer = 4
        Public Const NEXTRECORDNO_OFFSET As Integer = 3

        Public Const DALLASBARCODE As Integer = 14
        Public Const DALLASBARCODE_OFFSET As Integer = 7

        Public Const EXPECTEDDELDATE As Integer = 6
        Public Const EXPECTEDDELDATE_OFFSET As Integer = 21

        Public Const STATUS As Integer = 1
        Public Const STATUS_OFFSET As Integer = 27

    End Class

    ' V1.1 - CK
    ' Added new class DAE

    ''' <summary>
    ''' Class with constant variables to hold the offset and length of 
    ''' DAE message fields
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DAE

        Public Const TID As Integer = 3
        Public Const TID_OFFSET As Integer = 0

    End Class

End Class
