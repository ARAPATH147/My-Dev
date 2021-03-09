Public Class Message
    Public Const INITIALPOINTER As Integer = 0
    Public Const ENDTPOINTER As Integer = -1
    Public Const ENDPOINTER_STR As String = "-1"

    Public Class SOR
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const PSWD_OFFSET As Integer = 6
        Public Const PSWD As Integer = 3

        Public Const APPID_OFFSET As Integer = 9
        Public Const APPID As Integer = 3

        Public Const APPVER_OFFSET As Integer = 12
        Public Const APPVER As Integer = 4

        Public Const MAC_OFFSET As Integer = 16
        Public Const MAC As Integer = 12

        Public Const TYPE_OFFSET As Integer = 28
        Public Const TYPE As Integer = 1

        Public Const IP_OFFSET As Integer = 29
        Public Const IP As Integer = 8


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

        Public Const PRTNUM_OFFSET As Integer = 34
        Public Const PRTNUM As Integer = 10

        Public Const OSSR_OFFSET As Integer = 44
        Public Const OSSR As Integer = 1

        Public Const STKACCESS_OFFSET As Integer = 45
        Public Const STKACCESS As Integer = 1

        Public Const PRTDESC_OFFSET As Integer = 46
        Public Const PRTDESC As Integer = 200

    End Class
    Public Class DQR
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const DEAL_NUMBER_OFFSET As Integer = 3
        Public Const DEAL_NUMBER As Integer = 4

        Public Const START_DATE_OFFSET As Integer = 7
        Public Const START_DATE As Integer = 8

        Public Const END_DATE_OFFSET As Integer = 15
        Public Const END_DATE As Integer = 8

        Public Const DEAL_DESC_OFFSET As Integer = 23
        Public Const DEAL_DESC As Integer = 35
    End Class
    Public Class ENQ
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const SCANCODE_OFFSET As Integer = 8
        Public Const SCANCODE As Integer = 13

        Public Const STKFIGREQD_OFFSET As Integer = 21
        Public Const STKFIGREQD As Integer = 1

        Public Const OSSRFLAG_OFFSET As Integer = 22
        Public Const OSSRFLAG As Integer = 1

        Public Const PLANNER_OFFSET As Integer = 23
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

        Public Const SELDesc_OFFSET As Integer = 43
        Public Const SELDesc As Integer = 45

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

        Public Const DEALSUM_OFFSET As Integer = 157
        Public Const DEALSUM As Integer = 6
        Public Const MAX_Count_DEALSUM As Integer = 10
       

        Public Const PLANLOC1_OFFSET As Integer = 217
        Public Const PLANLOC1 As Integer = 3

        Public Const PLANLOC2_OFFSET As Integer = 220
        Public Const PLANLOC2 As Integer = 3

        Public Const RECALLITEM_OFFSET As Integer = 223
        Public Const RECALLITEM As Integer = 1

        Public Const MARKDOWN_OFFSET As Integer = 224
        Public Const MARKDOWN As Integer = 1

        Public Const RECALLTYPE_OFFSET As Integer = 231
        Public Const RECALLTYPE As Integer = 1

        Public Const PGGRP_OFFSET As Integer = 225
        Public Const PGGRP As Integer = 6

        Public Const PENDSALE_OFFSET As Integer = 232
        Public Const PENDSALE As Integer = 1


    End Class
    Public Class ACK
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const MESSAGE_OFFSET As Integer = 3
        Public Const MESSAGE As Integer = 3 * 21

    End Class
    Public Class NAK
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const MESSAGE_OFFSET As Integer = 3
        Public Const MESSAGE As Integer = 150

    End Class

    Public Class NAKERROR
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 8

        Public Const MESSAGE_OFFSET As Integer = 8
        Public Const MESSAGE As Integer = 150

    End Class
    Public Class OFF
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3
    End Class
    Public Class GAS
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3
    End Class
    Public Class GAR
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const PRCHKTGT_OFFSET As Integer = 3
        Public Const PRCHKTGT As Integer = 4

        Public Const PRCHKDNE_OFFSET As Integer = 7
        Public Const PRCHKDNE As Integer = 4
    End Class
    Public Class GAP
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 3
        Public Const LISTID As Integer = 3

        Public Const SEQ_OFFSET As Integer = 6
        Public Const SEQ As Integer = 3

        Public Const BARCODE_OFFSET As Integer = 9
        Public Const BARCODE As Integer = 13

        Public Const BCODE_OFFSET As Integer = 22
        Public Const BCODE As Integer = 7

        Public Const CURRENT_OFFSET As Integer = 29
        Public Const CURRENT As Integer = 4

        Public Const FILL_OFFSET As Integer = 33
        Public Const FILL As Integer = 4

        Public Const GAPFLAG_OFFSET As Integer = 37
        Public Const GAPFLAG As Integer = 1

        Public Const STOCKFIG_OFFSET As Integer = 38
        Public Const STOCKFIG As Integer = 6

        Public Const UPDOSSR_OFFSET As Integer = 44
        Public Const UPDOSSR As Integer = 1

        Public Const LOCCNT_OFFSET As Integer = 45
        Public Const LOCCNT As Integer = 2

    End Class
    Public Class GAX
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 6
        Public Const LISTID As Integer = 3

        Public Const PICKS_OFFSET As Integer = 9
        Public Const PICKS As Integer = 4

        Public Const SELS_OFFSET As Integer = 13
        Public Const SELS As Integer = 4

        Public Const PCHKS_OFFSET As Integer = 17
        Public Const PCHKS As Integer = 4
    End Class
    Public Class PLO
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3
    End Class
    Public Class PLR
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const SEQ_OFFSET As Integer = 6
        Public Const SEQ As Integer = 3

        Public Const AUTH_OFFSET As Integer = 9
        Public Const AUTH As Integer = 1

    End Class
    Public Class PLL
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const SEQ_OFFSET As Integer = 6
        Public Const SEQ As Integer = 3

        Public Const STATUS_OFFSET As Integer = 9
        Public Const STATUS As Integer = 1

        Public Const DATETIME_OFFSET As Integer = 10
        Public Const DATETIME As Integer = 12

        Public Const LINES_OFFSET As Integer = 22
        Public Const LINES As Integer = 4

        Public Const UNAME_OFFSET As Integer = 26
        Public Const UNAME As Integer = 15

        Public Const LOC_OFFSET As Integer = 41
        Public Const LOC As Integer = 1

        Public Const PICKER_OFFSET As Integer = 42
        Public Const PICKER As Integer = 3
    End Class
    Public Class PLS
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 6
        Public Const LISTID As Integer = 3

        Public Const SEQ_OFFSET As Integer = 9
        Public Const SEQ As Integer = 3

    End Class
    Public Class PLI
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 3
        Public Const LISTID As Integer = 3

        Public Const SEQ_OFFSET As Integer = 6
        Public Const SEQ As Integer = 3

        Public Const BCDE_OFFSET As Integer = 9
        Public Const BCDE As Integer = 7

        Public Const PARENT_OFFSET As Integer = 16
        Public Const PARENT As Integer = 7

        Public Const DESC_OFFSET As Integer = 23
        Public Const DESC As Integer = 20

        Public Const REQUIRED_OFFSET As Integer = 43
        Public Const REQUIRED As Integer = 4

        Public Const STATUS_OFFSET As Integer = 47
        Public Const STATUS As Integer = 1

        Public Const GAPFLAG_OFFSET As Integer = 48
        Public Const GAPFLAG As Integer = 1

        Public Const ACTDEAL_OFFSET As Integer = 49
        Public Const ACTDEAL As Integer = 1

        Public Const STOCKFIG_OFFSET As Integer = 50
        Public Const STOCKFIG As Integer = 6

        Public Const SELD_OFFSET As Integer = 56
        Public Const SELD As Integer = 45

        Public Const BARCODE_OFFSET As Integer = 101
        Public Const BARCODE As Integer = 13

        Public Const QTYSHELF_OFFSET As Integer = 114
        Public Const QTYSHELF As Integer = 4

        Public Const BKSHP_OFFSET As Integer = 118
        Public Const BKSHP As Integer = 4

        Public Const OSSRITEM_OFFSET As Integer = 122
        Public Const OSSRITEM As Integer = 1

        Public Const MSFLAG_OFFSET As Integer = 123
        Public Const MSFLAG As Integer = 1

    End Class
    Public Class PLC
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 6
        Public Const LISTID As Integer = 3

        Public Const SEQ_OFFSET As Integer = 9
        Public Const SEQ As Integer = 3

        Public Const BCDE_OFFSET As Integer = 12
        Public Const BCDE As Integer = 7

        Public Const COUNT_OFFSET As Integer = 19
        Public Const COUNT As Integer = 4

        Public Const GAPFLAG_OFFSET As Integer = 23
        Public Const GAPFLAG As Integer = 1

        Public Const PICKLOC_OFFSET As Integer = 24
        Public Const PICKLOC As Integer = 1

        Public Const OSSRCNT_OFFSET As Integer = 25
        Public Const OSSRCNT As Integer = 4

        Public Const UPDOSSR_OFFSET As Integer = 29
        Public Const UPDOSSR As Integer = 1

        Public Const LOCCNT_OFFSET As Integer = 30
        Public Const LOCCNT As Integer = 2

        Public Const MSPICK_OFFSET As Integer = 32
        Public Const MSPICK As Integer = 1

    End Class
    Public Class PLX
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 6
        Public Const LISTID As Integer = 3

        Public Const LINES_OFFSET As Integer = 9
        Public Const LINES As Integer = 4

        Public Const ITEMS_OFFSET As Integer = 13
        Public Const ITEMS As Integer = 6

        Public Const COMPLETE_OFFSET As Integer = 19
        Public Const COMPLETE As Integer = 1

    End Class
    Public Class PLF
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

    End Class
    Public Class PLE
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

    End Class
    Public Class MSA
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 6
        Public Const LISTID As Integer = 3

        Public Const SEQ_OFFSET As Integer = 9
        Public Const SEQ As Integer = 3

    End Class
    Public Class MSB
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 6
        Public Const LISTID As Integer = 3

        Public Const SEQ_OFFSET As Integer = 9
        Public Const SEQ As Integer = 3

        Public Const SHELFCNT_OFFSET As Integer = 12
        Public Const SHELFCNT As Integer = 4

        Public Const FILLQTY_OFFSET As Integer = 16
        Public Const FILLQTY As Integer = 4

    End Class
    Public Class PCS
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

    End Class
    Public Class PCR
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const PRCHKTGT_OFFSET As Integer = 3
        Public Const PRCHKTGT As Integer = 4

        Public Const PRCHKDNE_OFFSET As Integer = 7
        Public Const PRCHKDNE As Integer = 4

    End Class
    Public Class PCM
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const BCDE_OFFSET As Integer = 6
        Public Const BCDE As Integer = 7

        Public Const VAR_OFFSET As Integer = 13
        Public Const VAR As Integer = 6

        Public Const PRINTTYPE_OFFSET As Integer = 19
        Public Const PRINTYPE As Integer = 1

    End Class
    Public Class PCX
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const ITEMS_OFFSET As Integer = 6
        Public Const ITEMS As Integer = 4

        Public Const SELS_OFFSET As Integer = 10
        Public Const SELS As Integer = 4

    End Class
    Public Class PRT
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const BARCODE_OFFSET As Integer = 6
        Public Const BARCODE As Integer = 13

        Public Const METHOD_OFFSET As Integer = 19
        Public Const METHOD As Integer = 1

    End Class
    Public Class SSE
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

    End Class
    Public Class SSR
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const SVT_OFFSET As Integer = 3
        Public Const SVT As Integer = 10

        Public Const SVPREV_OFFSET As Integer = 13
        Public Const SVPREV As Integer = 10

    End Class
    Public Class ISE
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const BARCODE_OFFSET As Integer = 6
        Public Const BARCODE As Integer = 13

    End Class
    Public Class ISR
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const DESC_OFFSET As Integer = 3
        Public Const DESC As Integer = 20

        Public Const CQTY_OFFSET As Integer = 23
        Public Const CQTY As Integer = 4

        Public Const CVAL_OFFSET As Integer = 27
        Public Const CVAL As Integer = 8

        Public Const PQTY_OFFSET As Integer = 35
        Public Const PQTY As Integer = 4

        Public Const PVAL_OFFSET As Integer = 39
        Public Const PVAL As Integer = 8

    End Class
    Public Class CLO
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

    End Class
    Public Class CLR
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 6
        Public Const LISTID As Integer = 3

    End Class
    Public Class CLL
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const RETLISTID_OFFSET As Integer = 3
        Public Const RETLISTID As Integer = 3

        Public Const SEQ_OFFSET As Integer = 6
        Public Const SEQ As Integer = 3

        Public Const TOTITEMS_OFFSET As Integer = 9
        Public Const TOTITEMS As Integer = 3

        Public Const SFITEMS_OFFSET As Integer = 12
        Public Const SFITEMS As Integer = 3

        Public Const BSITEMS_OFFSET As Integer = 15
        Public Const BSITEMS As Integer = 3

        Public Const LSTYPE_OFFSET As Integer = 18
        Public Const LSTYPE As Integer = 1

        Public Const LSTCNTDT_OFFSET As Integer = 19
        Public Const LSTCNTDT As Integer = 8

        Public Const BUNAME_OFFSET As Integer = 27
        Public Const BUNAME As Integer = 30

        Public Const ACTIVE_OFFSET As Integer = 57
        Public Const ACTIVE As Integer = 1

        Public Const OSSRITEMS_OFFSET As Integer = 58
        Public Const OSSRITEMS As Integer = 3

        Public Const COUNTERID_OFFSET As Integer = 61
        Public Const COUNTERID As Integer = 3
    End Class
    Public Class CLS
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 6
        Public Const LISTID As Integer = 3

        Public Const SEQ_OFFSET As Integer = 9
        Public Const SEQ As Integer = 3

    End Class
    Public Class CLI
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 3
        Public Const LISTID As Integer = 3

        Public Const ITEMINLIST_OFFSET As Integer = 6
        Public Const ITEMINLIST As Integer = 3

        Public Const SEQID_OFFSET As Integer = 9
        Public Const SEQID As Integer = 3

        Public Const MOREITEM_OFFSET As Integer = 12
        Public Const MOREITEM As Integer = 1

        Public Const SEQ_OFFSET As Integer = 13
        Public Const SEQ As Integer = 3

        Public Const BCDE_OFFSET As Integer = 16
        Public Const BCDE As Integer = 7

        Public Const PARENT_OFFSET As Integer = 23
        Public Const PARENT As Integer = 7

        Public Const BARCODE_OFFSET As Integer = 30
        Public Const BARCODE As Integer = 13

        Public Const SELD_OFFSET As Integer = 43
        Public Const SELD As Integer = 45

        Public Const ACTDEAL_OFFSET As Integer = 88
        Public Const ACTDEAL As Integer = 1

        Public Const PRODGP_OFFSET As Integer = 89
        Public Const PRODGP As Integer = 6

        Public Const BSCNT_OFFSET As Integer = 95
        Public Const BSCNT As Integer = 4

        Public Const BSPSPCNT_OFFSET As Integer = 99
        Public Const BSPSPCNT As Integer = 4

        Public Const SFCNT_OFFSET As Integer = 103
        Public Const SFCNT As Integer = 4

        Public Const STATUS_OFFSET As Integer = 107
        Public Const STATUS As Integer = 1

        Public Const OSSRBSCNT_OFFSET As Integer = 108
        Public Const OSSRBSCNT As Integer = 4

        Public Const OSSRPSPCNT_OFFSET As Integer = 112
        Public Const OSSRPSPCNT As Integer = 4

        Public Const OSSRITEM_OFFSET As Integer = 116
        Public Const OSSRITEM As Integer = 1

        Public Const LSTCNTDATE_OFFSET As Integer = 117
        Public Const LSTCNTDATE As Integer = 8
        Public Const PENDSALE_OFFSET As Integer = 125
        Public Const PENDSALE As Integer = 1
        Public Const STOCKFIG_OFFSET As Integer = 126
        Public Const STOCKFIG As Integer = 6
        Public Const NEXTITEM As Integer = 119
        'Public Const NEXTITEM_OFFSET As Integer = 133
        'Public Const NEXTITEM As Integer = 1

    End Class
    Public Class CLC
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 6
        Public Const LISTID As Integer = 3

        Public Const SEQ_OFFSET As Integer = 9
        Public Const SEQ As Integer = 3

        Public Const BSCNT_OFFSET As Integer = 12
        Public Const BSCNT As Integer = 13

        Public Const CNTLOCN_OFFSET As Integer = 25
        Public Const CNTLOCN As Integer = 1

        Public Const COUNT_OFFSET As Integer = 26
        Public Const COUNT As Integer = 4

        Public Const SITEID_OFFSET As Integer = 30
        Public Const SITEID As Integer = 3

        Public Const UPDOSSR_OFFSET As Integer = 33
        Public Const UPDOSSR As Integer = 1

    End Class
    Public Class CLX
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 3
        Public Const LISTID As Integer = 3

        Public Const COMMIT_OFFSET As Integer = 6
        Public Const COMMIT As Integer = 1

        Public Const COUNTTYPE_OFFSET As Integer = 7
        Public Const COUNTTYPE As Integer = 1

    End Class
    Public Class CLF
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3
    End Class
    Public Class CLA
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const STATUS_OFFSET As Integer = 6
        Public Const STATUS As Integer = 3
    End Class
    Public Class CLB
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 3
        Public Const LISTID As Integer = 3
        Public Const SEQ_OFFSET As Integer = 6
        Public Const SEQ As Integer = 3

    End Class
    Public Class CLD
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 3
        Public Const LISTID As Integer = 3

        Public Const SEQ_OFFSET As Integer = 6
        Public Const SEQ As Integer = 3

        Public Const SITETYPE_OFFSET As Integer = 9
        Public Const SITETYPE As Integer = 1

        Public Const BCDE_OFFSET As Integer = 10
        Public Const BCDE As Integer = 13

    End Class
    Public Class CLG
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 3
        Public Const LISTID As Integer = 3

        Public Const SEQ_OFFSET As Integer = 6
        Public Const SEQ As Integer = 3

    End Class
    Public Class CLE
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const LISTID_OFFSET As Integer = 3
        Public Const LISTID As Integer = 3
    End Class
    Public Class RPO
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3
    End Class
    Public Class RLE
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const SEQ_OFFSET As Integer = 6
        Public Const SEQ As Integer = 4
    End Class
    Public Class RLR
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const SEQ_OFFSET As Integer = 3
        Public Const SEQ As Integer = 4

        Public Const TITLE_OFFSET As Integer = 7
        Public Const TITLE As Integer = 20

        Public Const REPORTID_OFFSET As Integer = 27
        Public Const REPORTID As Integer = 12
    End Class
    Public Class RLS
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const REPORTID_OFFSET As Integer = 6
        Public Const REPORTID As Integer = 12

        Public Const SEQ_OFFSET As Integer = 18
        Public Const SEQ As Integer = 4

    End Class
    Public Class RLD
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const COUNT_OFFSET As Integer = 3
        Public Const COUNT As Integer = 3

        Public Const SEQ_OFFSET As Integer = 6
        Public Const SEQ As Integer = 4

        Public Const DATA_OFFSET As Integer = 10
        Public Const DATA As Integer = 20
    End Class
    Public Class RPS
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const COUNT_OFFSET As Integer = 3
        Public Const COUNT As Integer = 3

        Public Const REPORTID_OFFSET As Integer = 6
        Public Const REPORTID As Integer = 12

        Public Const SEQ_OFFSET As Integer = 18
        Public Const SEQ As Integer = 20
    End Class
    Public Class RUP
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const COUNT_OFFSET As Integer = 3
        Public Const COUNT As Integer = 3

        Public Const LEVEL_OFFSET As Integer = 6
        Public Const LEVEL As Integer = 1

        Public Const FUNC_OFFSET As Integer = 7
        Public Const FUNC As Integer = 1

        Public Const DATA_OFFSET As Integer = 8
        Public Const DATA As Integer = 20


        Public Const HEADERTOTAL As Integer = 6
        Public Const MAX_COUNT As Integer = 10
        Public Const TRAILERTOTAL As Integer = 22
    End Class
    Public Class RLF
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3
    End Class
    Public Class RPX
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3
    End Class
    Public Class PGS
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3
    End Class
    Public Class PGX
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3
    End Class
    Public Class PGF
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const SEQ_OFFSET As Integer = 6
        Public Const SEQ As Integer = 4

        Public Const COREFLG_OFFSET As Integer = 10
        Public Const COREFLG As Integer = 1

        Public Const LIVEPEND_OFFSET As Integer = 11
        Public Const LIVEPEND As Integer = 1

     
    End Class
    Public Class PGG
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const HEADERTOTAL As Integer = 3
        Public Const TRAILERTOTAL As Integer = 64
        Public Const MAX_COUNT As Integer = 4

        Public Const SEQ_OFFSET As Integer = 0
        Public Const SEQ As Integer = 4

        Public Const DESC_OFFSET As Integer = 4
        Public Const DESC As Integer = 50

        Public Const STRTPTR_OFFSET As Integer = 54
        Public Const STRTPTR As Integer = 6

        Public Const FAMTYPE_OFFSET As Integer = 60
        Public Const FAMTYPE As Integer = 2

        Public Const HIREARCHY_OFFSET As Integer = 62
        Public Const HIREARCHY As Integer = 2

        Public Const MORE2COME_OFFSET As Integer = 259
        Public Const MORE2COME As Integer = 1
    End Class

    Public Class PGN
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const HEADERTOTAL As Integer = 3
        Public Const TRAILERTOTAL As Integer = 56
        Public Const MAX_COUNT As Integer = 4

        Public Const MODULESEQ_OFFSET As Integer = 0
        Public Const MODULESEQ As Integer = 3

        Public Const MOD_DESC_OFFSET As Integer = 3
        Public Const MOD_DESC As Integer = 50

        Public Const SHELF_COUNT_OFFSET As Integer = 53
        Public Const SHELF_COUNT As Integer = 2

        Public Const FILTER_OFFSET As Integer = 55
        Public Const FILTER As Integer = 1
    End Class
    Public Class PGQ
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const POGIPTR_OFFSET As Integer = 6
        Public Const POGIPTR As Integer = 6

        Public Const LIVEPEND_OFFSET As Integer = 12
        Public Const LIVEPEND As Integer = 1

    End Class
    Public Class PGR
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3


        Public Const HEADERTOTAL As Integer = 3 '(64 chars x 4 = 1040 bytes) + 15
        Public Const COUNT_MAX As Integer = 4
        Public Const TRAILERTOTAL As Integer = 75


        Public Const POGIPTR_OFFSET As Integer = 0
        Public Const POGIPTR As Integer = 6

        Public Const POGDESC_OFFSET As Integer = 6
        Public Const POGDESC As Integer = 50

        Public Const ACTDATE_OFFSET As Integer = 56
        Public Const ACTDATE As Integer = 8

        Public Const DACTDATE_OFFSET As Integer = 64
        Public Const DACTDATE As Integer = 8

        Public Const MODECNT_OFFSET As Integer = 72
        Public Const MODECNT As Integer = 3

        Public Const NXTPOGIREC_OFFSET As Integer = 303
        Public Const NXTPOGIREC As Integer = 6

        

    End Class

    Public Class PSR
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3


        Public Const NOTCH_NO_OFFSET As Integer = 3
        Public Const NOTCH_NO As Integer = 3

        Public Const SHELF_DESC_OFFSET As Integer = 6
        Public Const SHELF_DESC As Integer = 50


        'These details are used to calculate repeated items 
        Public Const HEADERTOTAL As Integer = 56 '3+3+50 = 56
        Public Const COUNT_MAX As Integer = 15
        Public Const TRAILERTOTAL As Integer = 32

        Public Const BOOTSCODE_OFFSET As Integer = 0
        Public Const BOOTSCODE As Integer = 6

        Public Const ITEM_DESC_OFFSET As Integer = 6
        Public Const ITEM_DESC As Integer = 24

        Public Const FACINGS_OFFSET As Integer = 30
        Public Const FACINGS As Integer = 2

        Public Const NEXT_SHELF_OFFSET As Integer = 536
        Public Const NEXT_SHELF As Integer = 3

        Public Const NEXT_CHAIN_OFFSET As Integer = 539
        Public Const NEXT_CHAIN As Integer = 2

        Public Const NEXT_ITEM_OFFSET As Integer = 541
        Public Const NEXT_ITEM As Integer = 2
    End Class

    Public Class PSL
        Public Const PSL_NEXT_OFFSET As Integer = 15
        Public Const PSL_NEXT As Integer = 7


        Public Const MODULE_ID_OFFSET As Integer = 12
        Public Const MODULE_ID As Integer = 3

        Public Const SHELF_NUMBER_OFFSET As Integer = 15
        Public Const SHELF_NUMBER As Integer = 3 '2

    End Class

    Public Class PGL
        Public Const NEXTSEQ_OFFSET As Integer = 12
        Public Const NEXTSEQ As Integer = 6

    End Class
    'anoop:Start
    Public Class PGA
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const BOOTSCODE_OFFSET As Integer = 6
        Public Const BOOTSCODE As Integer = 6

        Public Const STARTCHAIN_OFFSET As Integer = 12
        Public Const STARTCHAIN As Integer = 3

        Public Const STARTMOD_OFFSET As Integer = 15
        Public Const STARTMOD As Integer = 3

        Public Const STARTITEM_OFFSET As Integer = 18
        Public Const STARTITEM As Integer = 3

        Public Const FLAG_OFFSET As Integer = 21
        Public Const FLAG As Integer = 1

       
    End Class
    Public Class PGB
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const POGKEY_OFFSET As Integer = 3
        Public Const POGKEY As Integer = 6

        Public Const MODCOUNT_OFFSET As Integer = 9
        Public Const MODCOUNT As Integer = 3

        Public Const REPEATCOUNT_OFFSET As Integer = 12
        Public Const RPTCOUNT As Integer = 3

        Public Const POGDESC_OFFSET As Integer = 15
        Public Const POGDESC As Integer = 30

        Public Const MODDESC_OFFSET As Integer = 45
        Public Const MODEDESC As Integer = 30

        Public Const MDQ_OFFSET As Integer = 75
        Public Const MDQ As Integer = 4

        Public Const PSC_OFFSET As Integer = 79
        Public Const PSC As Integer = 4

        Public Const NEXTCHAIN_OFFSET As Integer = 323
        Public Const NEXTCHAIN As Integer = 3
        Public Const NEXTMOD_OFFSET As Integer = 326
        Public Const NEXTMOD As Integer = 3
        Public Const NEXTITEM_OFFSET As Integer = 329
        Public Const NEXTITEM As Integer = 3

        Public Const COUNT_MAX As Integer = 4
        Public Const TRAILERTOTAL As Integer = 80

    End Class
    'anoop:end
    Public Class PGM
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const ID_OFFSET As Integer = 3
        Public Const ID As Integer = 3

        Public Const POGDB_OFFSET As Integer = 6
        Public Const POGDB As Integer = 6

        Public Const MODSEQ_OFFSET As Integer = 12
        Public Const MODSEQ As Integer = 3

        Public Const FILTER_OFFSET As Integer = 15
        Public Const FILTER As Integer = 6


    End Class
    Public Class LPR
        Public Const TID_OFFSET As Integer = 0
        Public Const TID As Integer = 3

        Public Const WASPRICE1_OFFSET As Integer = 3
        Public Const WASPRICE1 As Integer = 6

        Public Const WASPRICE2_OFFSET As Integer = 9
        Public Const WASPRICE2 As Integer = 6

        Public Const PHFTYPE_OFFSET As Integer = 15
        Public Const PHFTYPE As Integer = 1

        Public Const UNITPRICEFLAG_OFFSET As Integer = 16
        Public Const UNITPRICEFLAG As Integer = 1

        Public Const UNITMEASURE_OFFSET As Integer = 17
        Public Const UNITMEASURE As Integer = 6

        Public Const UNITQTY_OFFSET As Integer = 23
        Public Const UNITQTY As Integer = 8

        Public Const UNITTYPE_OFFSET As Integer = 31
        Public Const UNITTYPE As Integer = 10

        Public Const WEEEFLAG_OFFSET As Integer = 41
        Public Const WEEEFLAG As Integer = 1

        Public Const WEEEPRFPRICE_OFFSET As Integer = 42
        Public Const WEEEPRFPRICE As Integer = 6

        Public Const MSFLAG_OFFSET As Integer = 48
        Public Const MSFLAG As Integer = 1

        Public Const PAINKILLERMSG_OFFSET As Integer = 49
        Public Const PAINKILLERMSG As Integer = 40


    End Class

    Public Class PGI
        Public Const HEADERTOTAL As Integer = 3
        Public Const MAX_COUNT As Integer = 4
        Public Const TRAILERTOTAL As Integer = 59

        Public Const POGKEY_OFFSET As Integer = 0
        Public Const POGKEY As Integer = 6

        Public Const POGDESC_OFFSET As Integer = 6
        Public Const POGDESC As Integer = 50

        Public Const MODULE_COUNT_OFFSET As Integer = 56
        Public Const MODULE_COUNT As Integer = 3


        Public Const NEXTCHAIN_OFFSET As Integer = 239
        Public Const NEXTCHAIN As Integer = 3

        Public Const NEXT_MOD_OFFSET As Integer = 241
        Public Const NEXT_MOD As Integer = 3

        Public Const NEXTSEQ_OFFSET As Integer = 239
        Public Const NEXTSEQ As Integer = 6
    End Class
End Class
