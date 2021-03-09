
Public Class UOR
    Public Const TID_OFFSET As Integer = 0
    Public Const TID As Integer = 3

    Public Const OP_ID_OFFSET As Integer = 3
    Public Const OP_ID As Integer = 6

    Public Const LIST_ID_OFFSET As Integer = 6
    Public Const List_ID As Integer = 4

    Public Const VALID_BC_OFFSET As Integer = 10
    Public Const VALID_BC As Integer = 30
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
End Class

Public Class DSR
    Public Const TID_OFFSET As Integer = 0
    Public Const TID As Integer = 3

    Public Const BUSINESS_CENTRE_OFFSET As Integer = 3
    Public Const BUSINESS_CENTRE As Integer = 1

    Public Const SEQUENCE_NUMBER_OFFSET As Integer = 4
    Public Const SEQUENCE_NUMBER As Integer = 4

    Public Const SUPPLIER_NUMBER_OFFSET As Integer = 8
    Public Const SUPPLIER_NUMBER As Integer = 6

    Public Const SUPP_NAME_OFFSET As Integer = 14
    Public Const SUPP_NAME As Integer = 10
End Class

Public Class DSE
    Public Const TID_OFFSET As Integer = 0
    Public Const TID As Integer = 3
End Class

Public Class STR
    Public Const TID_OFFSET As Integer = 0
    Public Const TID As Integer = 3

    Public Const UOD_NUMBER_OFFSET As Integer = 3
    Public Const UOD_NUMBER As Integer = 8

    Public Const UOD_SUFFIX_OFFSET As Integer = 11
    Public Const UOD_SUFFIX As Integer = 6
End Class

Public Class RCC
    Public Const TID_OFFSET As Integer = 0
    Public Const TID As Integer = 3

    Public Const OP_ID_OFFSET As Integer = 3
    Public Const OP_ID As Integer = 6

    Public Const INDEX_OFFSET As Integer = 6
    Public Const INDEX As Integer = 4

    Public Const RECALL_REF_OFFSET As Integer = 10
    Public Const RECALL_REF As Integer = 8

    Public Const RECALL_TYPE_OFFSET As Integer = 18
    Public Const RECALL_TYPE As Integer = 1

    Public Const RECALL_DESC_OFFSET As Integer = 19
    Public Const RECALL_DESC As Integer = 20

    Public Const RECALL_COUNT_OFFSET As Integer = 39
    Public Const RECALL_COUNT As Integer = 4

    Public Const ACTIVE_DATE_OFFSET As Integer = 43
    Public Const ACTIVE_DATE As Integer = 8

    Public Const SPECIAL_INSTRUCTION_OFFSET As Integer = 51
    Public Const SPECIAL_INSTRUCTION As Integer = 1

    Public Const LAB_TYPE_OFFSET As Integer = 52
    Public Const LAB_TYPE As Integer = 2

    Public Const MRQ_OFFSET As Integer = 54
    Public Const MRQ As Integer = 2
    'Tailoring
    Public Const TAILORED_OFFSET As Integer = 57
    Public Const TAILORED As Integer = 1

    Public Const RECALL_LIST_STATUS As Integer = 56
    Public Const RECALL_STATUS As Integer = 1
    'BATCH NOS for SPECIAL INSTRUCTION MESSAGE
    Public Const BATCH_NOS_OFFSET As Integer = 58
    Public Const BATCH_NOS As Integer = 30
End Class

Public Class RCE
    Public Const TID_OFFSET As Integer = 0
    Public Const TID As Integer = 3

    Public Const OP_ID_OFFSET As Integer = 3
    Public Const OP_ID As Integer = 6

End Class

Public Class RCF
    Public Const TID_OFFSET As Integer = 0
    Public Const TID As Integer = 3

    Public Const OP_ID_OFFSET As Integer = 3
    Public Const OP_ID As Integer = 6

    Public Const RECALL_REF_NUMBER_OFFSET As Integer = 6
    Public Const RECALL_REF_NUM As Integer = 8

    Public Const MORE_ITEMS_OFFSET As Integer = 14
    Public Const MORE_ITEMS As Integer = 1

    Public Const RECALL_STATUS_OFFSET As Integer = 15
    Public Const RECALL_STATUS As Integer = 1


    Public Const HEADERTOTAL As Integer = 16
    Public Const MAX_COUNT As Integer = 10
    Public Const TRAILERTOTAL As Integer = 36

    Public Const RECALL_ITEM_OFFSET As Integer = 0
    Public Const RECALL_ITEM As Integer = 6

    Public Const ITEM_DESCRIPTION_OFFSET As Integer = 6
    Public Const ITEM_DESCRIPTION As Integer = 20

    Public Const RECALL_COUNT_OFFSET As Integer = 30
    Public Const RECALL_COUNT As Integer = 4

    Public Const TSF_OFFSET As Integer = 26
    Public Const TSF As Integer = 4

    Public Const ITEM_FLAG_OFFSET As Integer = 34
    Public Const ITEM_FLAG As Integer = 1
    Public Const VISIBLE_OFFSET As Integer = 35
    Public Const VISIBLE As Integer = 1

End Class

Public Class RCJ
    Public Const TID_OFFSET As Integer = 0
    Public Const TID As Integer = 3

    Public Const OP_ID_OFFSET As Integer = 3
    Public Const OP_ID As Integer = 6

    Public Const RECALL_REF_OFFSET As Integer = 6
    Public Const RECALL_REF As Integer = 8

    Public Const SPECIAL_INST_OFFSET As Integer = 14
    Public Const SPECIAL_INST As Integer = 160
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

    'AFF PL CR 
    'Public Const RECALLTYPE_OFFSET As Integer = 225
    'Public Const RECALLTYPE As Integer = 1

    Public Const PGGROUP_OFFSET As Integer = 225
    Public Const PGGROUP As Integer = 6

    'Recalls CR
    Public Const RECALLTYPE_OFFSET As Integer = 231
    Public Const RECALLTYPE As Integer = 1


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

Public Class DSG
    Public Const LISTNUM_OFFSET As Integer = 7
    Public Const LISTNUM As Integer = 4
End Class

Public Class RCD
    Public Const INDEX_OFFSET As Integer = 6
    Public Const INDEX As Integer = 4
End Class
Public Class RCH
    Public Const INDEX_OFFSET As Integer = 14
    Public Const INDEX As Integer = 4
End Class