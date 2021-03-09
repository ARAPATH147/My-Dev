// ****************************************************************************
// v2.1
// v3.0 - 2004 RF Core Rollout January - March 2004 Paul Bowers
// v4.0 - 2004 OSSR (Off site StockRoom Support) 31st August 2004 Paul Bowers
// v5.0 - 2004 Credit Claiming - 16th Nov 2004 - Stuart Highley
// v6.0 - 2007 Recalls 22nd May 2007 - Paul Bowers
// v6.1 - 17-10-2007 - Minor fix for Mobile printing - Brian Greenfield BMG
// v6.2 - 14-08-2008 - Changes to PLLDB for Multi-Site Counts - Brian Greenfield BMG
// v6.3 - 10-09-2008 - Added ACTBUILD semaphore pipe definitions for use with MC70's.
// v6.4 - 12-08-2011 - Stuart Highley - Mult UOD Rcll - Converted single element array.
// v7.0 - 12-03-2012 - Stock File Accuracy - Charles Skadorwa (CSk)
//                     Updated PLLDB structure with new pending counts and times.
//                     Increased multi-sites from 11 to 33.
//                     PLLDB_RECL increased from 164 to 384.
// ****************************************************************************

#if !defined RFSFILE_H
    #define RFSFILE_H 1

//Flags for Boots read and write interfaces
    #define B_OPENFILE          0x01
    #define B_CLOSEFILE         0x02

    #define LRTLG               "LRTLG"
    #define LRTLGBKP            "ADXLXACN::D:\\ADX_UDT1\\LRTLG.BKP"
    #define LRTLG_RECL          25L                                         //SDH 19-01-2005 OSSR WAN
    #define LRTLG_REP           548                                         //SDH 19-01-2005 OSSR WAN
    #define LRTLG_OFLAGS        0x2014
    #define LRTLG_CFLAGS        0x2014

// LRTLG record layout
typedef struct LRTLG_Record_Layout {
    BYTE type;
    BYTE user[3];
    WORD year;
    BYTE month;
    BYTE day;
    LONG time_ms;
    BYTE bLocation;                                                         //SDH 19-01-2005 OSSR WAN
    BYTE details[12];
} LRTLG_REC;


    #define SELBF               "SELBF"
    #define SELBF_RECL          20L
    #define SELBF_REP           0
    #define SELBF_OFLAGS        0x201C
    #define SELBF_CFLAGS        0x243C

// SELBF record layout
typedef struct SELBF_Record_Layout {
    BYTE item_code[13];
    BYTE info[6];
    BYTE printerid[1];
} SELBF_REC;

    #define GAPBF               "GAPBF"
    #define GAPBF_RECL          1L         // Variable
    #define GAPBF_REP           0
    #define GAPBF_OFLAGS        0x203C
    #define GAPBF_CFLAGS        0x243C

//// GAPBF record layout
//typedef struct GAPBF_Record_Layout {
//    BYTE item_code[13];
//    BYTE check_price[6];
//    BYTE filler[1];
//} GAPBF_REC;

    // ASN Carton Buffer Definition                                 // BMG 17-10-2008 ASN/Directs
    #define CBBF               "CB:"                                // BMG 17-10-2008 ASN/Directs
    #define CBBFTMP            "WC"       // Temp file name         // BMG 17-10-2008 ASN/Directs
    #define CBBF_RECL          1L         // Variable               // BMG 17-10-2008 ASN/Directs
    #define CBBF_REP           0                                    // BMG 17-10-2008 ASN/Directs
    #define CBBF_OFLAGS        0x203C                               // BMG 17-10-2008 ASN/Directs
    #define CBBF_CFLAGS        0x243C                               // BMG 17-10-2008 ASN/Directs

    // Positive UOD Buffer Definition                               // BMG 17-10-2008 ASN/Directs
    #define PUBBF               "PUB:"                              // BMG 17-10-2008 ASN/Directs
    #define PUBBFTMP            "WU"       // Temp file name        // BMG 17-10-2008 ASN/Directs
    #define PUBBF_RECL          1L         // Variable              // BMG 17-10-2008 ASN/Directs
    #define PUBBF_REP           0                                   // BMG 17-10-2008 ASN/Directs
    #define PUBBF_OFLAGS        0x203C                              // BMG 17-10-2008 ASN/Directs
    #define PUBBF_CFLAGS        0x243C                              // BMG 17-10-2008 ASN/Directs

    // Directs Buffer Definition                                    // BMG 17-10-2008 ASN/Directs
    #define DIRBF               "DIB:"                              // BMG 17-10-2008 ASN/Directs
    #define DIRBFTMP            "WD"       // Temp file name        // BMG 17-10-2008 ASN/Directs
    #define DIRBF_RECL          1L         // Variable              // BMG 17-10-2008 ASN/Directs
    #define DIRBF_REP           0                                   // BMG 17-10-2008 ASN/Directs
    #define DIRBF_OFLAGS        0x203C                              // BMG 17-10-2008 ASN/Directs
    #define DIRBF_CFLAGS        0x243C                              // BMG 17-10-2008 ASN/Directs

    #define STOCK               "STOCK"
    #define STOCK_RECL          30L
    #define STOCK_KEYL          4
    #define STOCK_REP           108
    #define STOCK_OFLAGS        0x2018

typedef struct STOCK_Record_Layout {
    BYTE boots_code[4];
    WORD stock_fig;
    WORD last_count;
    BYTE date_last_count[3];
    BYTE date_last_move[3];
    WORD last_rec;
    BYTE date_last_rec[3];
    BYTE filler[11];
} STOCK_REC;

    #define IMSTC               "IMSTC"
    #define IMSTC_RECL          40L
    #define IMSTC_KEYL          11
    #define IMSTC_REP           31
    #define IMSTC_OFLAGS        0x2018

typedef struct IMSTC_Record_Layout {
    BYTE bar_code[11];
    LONG restart;
    LONG numitems;
    LONG amtsale;
    LONG reserved;
    LONG stkmq_restart;
    BYTE status_flag[1];
    WORD stock_figure;
    BYTE reason_item_removed[1];
    BYTE filler[5];
} IMSTC_REC;

    #define CIMF                "CIMFI"
    #define CIMF_RECL           16L
    #define CIMF_KEYL           4
    #define CIMF_REP            54
    #define CIMF_OFLAGS         0x2018

typedef struct CIMF_Record_Layout {
    BYTE boots_code[4];
    LONG restart;
    LONG numitem;
    BYTE trans_date[3];
    BYTE space[1];
} CIMF_REC;

    #define CITEM               "CITEM"
    #define CITEM_RECL          64L
    #define CITEM_KEYL          4
    #define CITEM_REP           184
    #define CITEM_OFLAGS        0x2018

typedef struct CI_Record_Layout {
    BYTE boots_code[4];
    BYTE shelf_allocation[2];
    BYTE vulnerable_esa[2];
    BYTE special_order_esa[2];
    BYTE total_esa[2];
    BYTE initial_display_stock[2];
    BYTE on_order_in_this_pdt[2];
    BYTE on_order_today[2];
    BYTE total_on_order[2];
    BYTE date_of_last_manual_count[3];
    BYTE unit[1];
    BYTE vulnerable_report_flag[1];
    BYTE sales_sign_flag[1];
    BYTE week_4_sales[4];
    BYTE week_3_sales[4];
    BYTE week_2_sales[4];
    BYTE week_1_sales[4];
    WORD alter_sales_quantity;
    BYTE list_frequency[1];
    BYTE yesterdays_sales[2];
    BYTE count_request_flag[1];
    WORD previous_theoretical_stock;
    BYTE filler[14];
} CITEM_REC;

    #define GAPMES              "GAPMES"
    #define GAPMES_RECL         1L
    #define GAPMES_REP          0
    #define GAPMES_OFLAGS      0x201C
// No layout required - semaphore file

    #define PLLOL           "PLLOL"
    #define PLLOL_RECL      34L                                             // 02-2-05 SDH OSSR WAN
    #define PLLOL_REP       510
    #define PLLOL_OFLAGS    0x201C

//  record layout
typedef struct {
    BYTE list_id[3];
    BYTE creator[3];
    BYTE picker[3];
    BYTE list_status[1];
    BYTE create_date[6];
    BYTE create_time[4];
    BYTE pick_start_time[4];
    BYTE pick_end_time[4];
    BYTE item_count[4];
    BYTE cOssrPicking;                                                      // 02-2-05 SDH OSSR WAN
    BYTE cLocation; // [S]helf Monitor, [F]ast fill, [O]ssr, [E]xcess stock (shop floor),
                    // [B]ack shop Excess stock, Excess stock [C]ount in OSSR
} PLLOL_REC;

    #define PLLDB           "PLLDB"
    #define PLLDB_KEYL      6L
//  #define PLLDB_RECL      28L                                               // 31-8-04 PAB OSSR
//  #define PLLDB_RECL      54L                                               // 31-8-04 PAB OSSR
//  #define PLLDB_RECL      164L                                              // BMG 6.2 14-08-2008 //CSk 12-03-2012 SFA
    #define PLLDB_RECL      384L                                              //CSk 12-03-2012 SFA
    #define PLLDB_REP       511
    #define PLLDB_OFLAGS    0x201C

typedef struct {                                                            // BMG 6.2 14-08-2008 Multi-Sited sub record
    BYTE ms_loc_count[2];                                                   // BMG 6.2 14-08-2008
    BYTE ms_sales_fig[2];                                                   // BMG 6.2 14-08-2008
    BYTE ms_time_of_count[2];                                               // BMG 6.2 14-08-2008
    BYTE ms_fill_qty[2];                                                    // BMG 6.2 14-08-2008
    BYTE ms_filler[2];                                                      // BMG 6.2 14-08-2008
} PLLDB_MS_REC;                                                             // BMG 6.2 14-08-2008

//  record layout
typedef struct {
    BYTE list_id[3];
    BYTE seq[3];
    BYTE boots_code[4];
    BYTE qty_on_shelf[4];
    BYTE fill_qty[4];
    BYTE gap_flag[1];
    BYTE item_status[1];
    BYTE stock_room_count[4];
    BYTE sales_figure[4];
    BYTE abPendingBSCnt[2];                                                 //CSk 12-03-2012 SFA
    BYTE abPendingBSTime[2];                                                //CSk 12-03-2012 SFA
    BYTE abPendingOSSRCnt[2];                                               //CSk 12-03-2012 SFA
    BYTE abPendingOSSRTime[2];                                              //CSk 12-03-2012 SFA
    BYTE ossr_count[4];                                                     // PAB 31-8-04 OSSR
    BYTE time_sf_count[2];                                                  // PAB 31-8-04 OSSR
    BYTE time_bs_count[2];                                                  // PAB 31-8-04 OSSR
    BYTE time_ossr_count[2];                                                // PAB 31-8-04 OSSR
    BYTE sales_bs_count[4];                                                 // PAB 31-8-04 OSSR
    BYTE sales_ossr_count[4];                                               // PAB 31-8-04 OSSR
    PLLDB_MS_REC aMS_details[33];                                           //CSk 12-03-2012 SFA
} PLLDB_REC;

    #define STC                 "STCXX"
    #define STC_RECL            128L
    #define STC_KEYL            2
    #define STC_REP             38
    #define STC_OFLAGS          0x2018

    #define STP                 "STPXX"
    #define STP_RECL            128L
    #define STP_KEYL            2
    #define STP_REP             39
    #define STP_OFLAGS          0x2018

typedef struct {
    LONG cash;
    LONG credit;
    BYTE cr_time[3];
    LONG discount;
    BYTE resv[3];
} ST_REC_SUB;

typedef struct {
    BYTE workgroup[2];
    ST_REC_SUB details[7];
} ST_REC;

    #define IMFP                "IMFP"
    #define IMFP_RECL           27L
    #define IMFP_KEYL           11
    #define IMFP_REP            32
    #define IMFP_OFLAGS         0x2018

typedef struct {
    BYTE bar_code[11];
    LONG restart;
    LONG numitems;
    LONG amtsale;
    LONG resv;
} IMF_REC;

    #define STKMQ               "STKMQ"
    #define STKMQ_RECL          1L      // Variable length
    #define STKMQ_REP           83
    #define STKMQ_OFLAGS        0x2014
// Type 13 record
//  o/s   lth  description          data ( []=binary )
//    0     3  record ident         "[13];
//    3     3  date now (YYMMDD)    PPP
//    6     3  time now (HHMMSS)    PPP
//    9     4  list number          AAAA
//    13    2  item number          AA
//    15    1  item status          A
//    16    4  item code (0ccccccd) PPPP
//    20    3  count date (YYMMDD)  PPP
//    23    2  count time (HHMM)    PP
//    25    6  price                PPPPP;
//    31    5  count                AAAA;
//    36    4  backroom count       AAAA     "XXXX" = RP Count.
//    40    3  record terminator    "[0D0A]
    #define STKMQ_T13_LTH 43L

// Type 17 record
//  o/s   lth  description          data ( []=binary )
//     0    3  record ident         "[14];
//     3    3  date (YYMMDD)        PPP
//     6    3  time (HHMMSS)        PPP
//     9    4  list number          AAAA
//    13    1  list status          A   (from PILST list Header record)
//    14    3  record terminator    "[0D0A]
    #define STKMQ_T14_LTH 17L

// Variable length record - create dynamically

    #define RFRDESC             "RFRDESC"
    #define RFRDESC_RECL        51L
    #define RFRDESC_REP         0
    #define RFRDESC_OFLAGS     0x201C

typedef struct {
    BYTE title[20];
    BYTE fname[12];
    BYTE days[3];
    BYTE action[1];
    BYTE arcfname[12];
    BYTE reptype[1];                                                        // v3.0
    BYTE term[2];
} RFRDESC_REC;

    #define RFREP_RECL         26L
    #define RFREP_REP       0
    #define RFREP_OFLAGS    0x201C

typedef struct {
    BYTE q1[1];
    BYTE level[1];
    BYTE exp[1];
    BYTE data[20];
    BYTE q2[1];
    BYTE term[2];
} RFREP_REC;

    #define TSF                   "EALTERMS"
    #define TSF_RECL              63L
    #define TSF_KEYL              2
    #define TSF_REP               29
    #define TSF_OFLAGS            0x2018
    #define TSF_BUFFER            0x8000L

    #define PSBT                  "PSBTERMS"
    #define PSBT_RECL             63L
    #define PSBT_KEYL             2
    #define PSBT_REP              43
    #define PSBT_OFLAGS           0x2018

typedef struct {
    BYTE till[2];                                                           // UPD (= 0x9999)
    BYTE indicat0[1];
    BYTE indicat1[1];
    BYTE tslname[8];
    BYTE monitor[2];
    BYTE tlogflag[1];
    BYTE resvd[48]
} TSF_STORE_REC;

typedef struct {
    BYTE till[2];                                                           // UPD (not = 0x9999)
    BYTE indicat0[1];
    BYTE indicat1[1];
    LONG grosspos;
    LONG grossneg;
    LONG netcash;
    LONG netncash;
    LONG amtloan;
    LONG amtpicku;
    LONG amtcashc;
    BYTE operator[4];                                                       // ?
    BYTE transnum[2];                                                       // ?
    BYTE user[9];                                                           // ?
    BYTE controller[2];                                                     // ?
    BYTE resvd[14]
} TSF_TILL_REC;

    #define WRF                   "EALWORKG"
    #define WRF_RECL              112L
    #define WRF_KEYL     2
    #define WRF_REP               11
    #define WRF_OFLAGS            0x2018
    #define WRF_BUFFER      0x8000L

typedef struct {
    BYTE wrkgp[2];                                                          // UPD
    BYTE name[20];
    BYTE till[20][2];                                                       // UPD
} WRF_REC;

// External communications pipe
    #define CPIPE           "pi:rfscomms"
    #define CPIPE_RECL         6L
    #define CPIPE_REP       0
//#define CPIPE_CFLAGS     0x2519      // v4.0
    #define CPIPE_CFLAGS    0x3519      // v4.0
    #define CPIPE_TIMEOUT      100L     // Read timeout (mS)

//ACTBUUILD Semaphore pipe                                                  // BMG 10-09-2008 6.3 MC70
    #define SPIPE           "pi:actbuild"                                   // BMG 10-09-2008 6.3 MC70
    #define SPIPE_RECL      20L                                             // BMG 10-09-2008 6.3 MC70
    #define SPIPE_REP       0                                               // BMG 10-09-2008 6.3 MC70
    #define SPIPE_CFLAGS    0x3519                                          // BMG 10-09-2008 6.3 MC70
    #define SPIPE_TIMEOUT   100L     // Read timeout (mS)                   // BMG 10-09-2008 6.3 MC70

    #define DBG          "ADXLXACN::D:\\ADX_UDT1\\RFDEBUG.DAT"
    #define DBGBKP          "ADXLXACN::D:\\ADX_UDT1\\RFDEBUG.BKP"
    #define DBG_RECL        1L
    #define DBG_REP            0
    #define DBG_OFLAGS         0x2014
    #define DBG_CFLAGS         0x2014


    #define PST          "ADXLXACN::D:\\ADX_UDT1\\PATRACE.DAT"
    #define PSTBKP          "ADXLXACN::D:\\ADX_UDT1\\PATRACE.BKP"
    #define PST_RECL        1L
    #define PST_REP         0
    #define PST_OFLAGS         0x2014
    #define PST_CFLAGS         0x2014

    #define MINLS               "MINLS"
    #define MINLS_RECL          11L
    #define MINLS_KEYL          4
    #define MINLS_REP           547
    #define MINLS_OFLAGS        0x201C

typedef struct MINLS_Record_Layout {
    BYTE boots_code[4];                                                     // UPD
    BYTE recount_date[3];                                                   // UPD YYMMDD (windowed)
    BYTE discrepancy[3];                                                    // UPD
    BYTE count_status[1];
} MINLS_REC;

    #define SUSPT              "EALSUSPT"
    #define SUSPT_RECL         508L
    #define SUSPT_KEYL         6
    #define SUSPT_REP          81
    #define SUSPT_OFLAGS       0x201C

typedef struct SUSPT_Record_layout {
    BYTE terminal[2];                                                       // UPD
    BYTE transnum[2];                                                       // UPD
    WORD sequence;                                                          // UPD
    BYTE reason[1];                                                         // ascii
    BYTE day_time[6];                                                       // UPD ddmmyyhhmmss
    BYTE operator_id[3];                                                    // ascii
    BYTE EAN_data[481];                                                     // Ascii 37*13EAN
    BYTE filler[11];
} SUSPT_REC;

    #define PIITM              "PIITM"
    #define PIITM_RECL         30L
    #define PIITM_KEYL         6
    #define PIITM_REP          121
    #define PIITM_OFLAGS       0x2018

typedef struct PIITM_Record_Layout {
    BYTE list_no[4];                                                        // ASCII "0000"
    BYTE list_seq[2];                                                       // ASCII
    BYTE item_code[4];                                                      // UPD YYMMDD (windowed)
    BYTE on_idf[1];                                                         // ASCII
    BYTE activity[1];                                                       // ASCII
    BYTE cycle_length[2];
    BYTE family_marker[2];
    BYTE members[2];
    BYTE eligibility_flag[1];
    BYTE status[1];
    BYTE potential_recount[1];
    BYTE discrepancy_qty[2];
    BYTE discrepancy_amt[2];
    BYTE filler[5];
} PIITM_REC;


    #define PILST              "PILST"
    #define PILST_RECL         40L
    #define PILST_KEYL         4
    #define PILST_REP          120
    #define PILST_OFLAGS       0x2018

typedef struct PILST_Record_Layout_H {
    BYTE list_no[4];                                                        // ASCII "0000"
    BYTE list_no_spare[4];                                                  // ASCII
    BYTE cpm_run_date[3];                                                   // UPD YYMMDD (windowed)
    BYTE pipln_run_ok[1];                                                   // ASCII
    BYTE list_no_highest[4];                                                // ASCII
    BYTE resvd[24];
} PILST_HEADER_REC;

typedef struct PILST_Record_Layout_D {
    BYTE list_no[4];                                                        // ASCII not "0000"
    BYTE name[12];                                                          // ASCII
    BYTE bc_letter[1];                                                      // ASCII
    BYTE type[1];                                                           // ASCII
    BYTE count_by_date[3];                                                  // UPD YYMMDD (windowed)
    BYTE product_group[3];                                                  // UPD GGGGSS
    BYTE items[1];                                                          // UPD
    BYTE to_be_counted[1];                                                  // UPD
    BYTE count_date[3];                                                     // UPD YYMMDD (windowed)
    BYTE status[1];                                                         // ASCII
    BYTE recount_date[3];                                                   // UPD YYMMDD (windowed)
    BYTE recount_allowed[1];                                                // ASCII
    BYTE resvd[6];
} PILST_DETAIL_REC;

// Shared memory buffer, for interprocess communications
    #define SM_BUFFER_NAME      "sm:rfsstat"
    #define SM_BUFFER_SIZE      1024L

    #define RFOK           "ADXLXACN::D:\\ADX_UDT1\\RFOK.BIN"               // 24-8-2004 PAB
    #define RFOK_RECL      80L                                              // 24-8-2004 PAB
    #define RFOK_REP       517                                              // 24-8-2004 PAB
    #define RFOK_OFLAGS    0x201C                                           // 24-8-2004 PAB
// 24-8-2004 PAB
typedef struct RFOK_Record_Layout {                                         // 24-08-2004 PAB
    BYTE rfaudit;                                                           // 24-08-2004 PAB
    BYTE rfmaint;                                                           // 24-08-2004 PAB
    BYTE rfpikmnt;                                                          // 24-08-2004 PAB
    BYTE rfccmnt;                                                           // 07-12-2004 SDH
    BYTE rfscc;                                                             // 07-12-2004 SDH
    BYTE filler[75];                                                        // 24-8-2004 PAB
} RFOK_REC;                                                                 // 24-8-2004 PAB


//---- DEAL ----                                                            // SDH 17-11-04 CREDIT CLAIM
    #define DEAL             "$EAL"                                         // SDH 17-11-04 CREDIT CLAIM
    #define DEAL_REP         311                                            // SDH 17-11-04 CREDIT CLAIM
    #define DEAL_OFLAGS      A_READ | A_SHARE                               // SDH 17-11-04 CREDIT CLAIM
    #define DEAL_KEYL        2                                              // SDH 26-11-04 PROMOTIONS
typedef struct DEAL_QLFY_Layout {                                           // SDH 26-11-04 PROMOTIONS
    BYTE bFlag;                                                             // SDH 26-11-04 PROMOTIONS
    BYTE bCode;                                                             // SDH 26-11-04 PROMOTIONS
    LONG lAmnt;                                                             // SDH 26-11-04 PROMOTIONS
    BYTE bList;                                                             // SDH 26-11-04 PROMOTIONS
    BYTE bFiller;                                                           // SDH 26-11-04 PROMOTIONS
} DEAL_QLFY;                                                                // SDH 26-11-04 PROMOTIONS
typedef struct DEAL_REWD_Layout {                                           // SDH 26-11-04 PROMOTIONS
    BYTE bCode;                                                             // SDH 26-11-04 PROMOTIONS
    BYTE bQty;                                                              // SDH 26-11-04 PROMOTIONS
    LONG lAmt;                                                              // SDH 26-11-04 PROMOTIONS
    BYTE bList;                                                             // SDH 26-11-04 PROMOTIONS
    BYTE bQualMsg;                                                          // SDH 26-11-04 PROMOTIONS
    BYTE abQlfyMsgRcpt[38];                                                 // SDH 26-11-04 PROMOTIONS
    BYTE abQlfyMsgDisp[20];                                                 // SDH 26-11-04 PROMOTIONS
    BYTE bRewdMsg;                                                          // SDH 26-11-04 PROMOTIONS
    BYTE abRewdMsgRcpt[27];                                                 // SDH 26-11-04 PROMOTIONS
    WORD wMaxQlfy;                                                          // SDH 26-11-04 PROMOTIONS
    WORD wNumRewdQlfy;                                                      // SDH 26-11-04 PROMOTIONS
    BYTE abFiller[2];                                                       // SDH 26-11-04 PROMOTIONS
} DEAL_REWD;                                                                // SDH 26-11-04 PROMOTIONS
typedef struct DEAL_REC_Layout {                                            // SDH 26-11-04 PROMOTIONS
    BYTE abDealNumPD[2];                                                    // SDH 26-11-04 PROMOTIONS
    BYTE abPrevDealPD[2];                                                   // SDH 26-11-04 PROMOTIONS
    BYTE abNextDealPD[2];                                                   // SDH 26-11-04 PROMOTIONS
    BYTE abStartDatePD[4];                                                  // SDH 26-11-04 PROMOTIONS
    WORD wStartTime;                                                        // SDH 26-11-04 PROMOTIONS
    BYTE abEndDatePD[4];                                                    // SDH 26-11-04 PROMOTIONS
    WORD wEndTime;                                                          // SDH 26-11-04 PROMOTIONS
    BYTE cBusCentre;                                                        // SDH 26-11-04 PROMOTIONS
    BYTE abDealDesc[35];                                                    // SDH 26-11-04 PROMOTIONS
    BYTE bNumLists;                                                         // SDH 26-11-04 PROMOTIONS
    BYTE bRunDay;                                                           // SDH 26-11-04 PROMOTIONS
    BYTE bPriority;                                                         // SDH 26-11-04 PROMOTIONS
    WORD wExemptTills;                                                      // SDH 26-11-04 PROMOTIONS
    BYTE bFlags1;                                                           // SDH 26-11-04 PROMOTIONS
    BYTE bSalesPrompt;                                                      // SDH 26-11-04 PROMOTIONS
    BYTE bExclMsg;                                                          // SDH 26-11-04 PROMOTIONS
    BYTE bFlags2;                                                           // SDH 26-11-04 PROMOTIONS
    BYTE bNumQlfns;                                                         // SDH 26-11-04 PROMOTIONS
    BYTE bNumRewds;                                                         // SDH 26-11-04 PROMOTIONS
    BYTE abFiller[3];                                                       // SDH 26-11-04 PROMOTIONS
    DEAL_QLFY Qual[4];                                                      // SDH 26-11-04 PROMOTIONS
    DEAL_REWD Rewd[3];                                                      // SDH 26-11-04 PROMOTIONS
} DEAL_REC;                                                                 // SDH 26-11-04 PROMOTIONS
    #define DEAL_RECL         sizeof(DEAL_REC)                              // SDH 26-11-04 PROMOTIONS
//---- TDTFF (Deal trickle file) ----                                       // SDH 26-11-04 PROMOTIONS
    #define TDTFF            "$DTFF"                                        // SDH 26-11-04 PROMOTIONS
    #define TDTFF_REP        650                                            // SDH 26-11-04 PROMOTIONS
    #define TDTFF_OFLAGS     A_READ | A_SHARE                               // SDH 26-11-04 PROMOTIONS
typedef struct TDTFF_HEADER_Layout {                                        // SDH 26-11-04 PROMOTIONS
    LONG lPtr;                                                              // SDH 26-11-04 PROMOTIONS
    LONG lMaxPtr;                                                           // SDH 26-11-04 PROMOTIONS
} TDTFF_HEADER;                                                             // SDH 26-11-04 PROMOTIONS
typedef struct TDTFF_REC_Layout {                                           // SDH 26-11-04 PROMOTIONS
    UWORD uwDealNumPD;                                                      // SDH 26-11-04 PROMOTIONS
} TDTFF_REC;                                                                // SDH 26-11-04 PROMOTIONS
    #define TDTFF_RECL      sizeof(TDTFF_REC)                               // SDH 26-11-04 PROMOTIONS

    #define EPSOM           "D:\\ADX_UDT1\\EPSOM.BIN"                       // v3.0
    #define EPSOM_RECL      1L                                              // v3.0
    #define EPSOM_REP       186                                             // v3.0
    #define EPSOM_OFLAGS    0x200F  // Exclusive lock required              // v3.0

    #define PCHK            "D:\\ADX_UDT1\\PCHECK.BIN"                      // v3.0
    #define PCHK_RECL       1L                                              // v3.0
    #define PCHK_REP        190                                             // v3.0
    #define PCHK_OFLAGS     0x200F  // Exclusive lock required              // v3.0

#define JOBOK           "JOBOK"                                              // 13-12-2004 PAB
#define JOBOK_RECL      84L                                                  // 13-12-2004 PAB
#define JOBOK_REP       14                                                   // 13-12-2004 PAB
#define JOBOK_OFLAGS    0x201C                                               // 13-12-2004 PAB
                                                                             // 13-12-2004 PAB
typedef struct JOBOK_Record_Layout {                                         // 13-12-2004 PAB
   BYTE delim1[1];
   BYTE psb21[1];          // Started,Ended,Xabend                           // 13-12-2004 PAB
   BYTE psb22[1];                                                            // 13-12-2004 PAB
   BYTE psb23[1];                                                            // 13-12-2004 PAB
   BYTE psb24[1];                                                            // 13-12-2004 PAB
   BYTE psb25[1];                                                            // 13-12-2004 PAB
   BYTE filler[78];                                                          // 13-12-2004 PAB
} JOBOK_REC;                                                                 // 13-12-2004 PAB

#define PDTASSET         "PDTASSET"
#define PDTASSET_KEYL    12L
#define PDTASSET_RECL    91L
#define PDTASSET_REP     780
#define PDTASSET_OFLAGS  0x201C

typedef struct {
    BYTE AppVer[4];
} APP_VER_REC;

typedef struct PDTASSET_Record_layout {
    BYTE MAC[12];           //Key
    BYTE Dev_Type[1];       //Device type, M= MC70, P = PPC
    BYTE IP_Addr[15];       //IP Address xxx.xxx.xxx.xxx
    BYTE Date_Last_Dock[8]; //Date of last dock CCYYMMDD
    BYTE Time_Last_Dock[4]; //Time of last dock HHMM
    BYTE User_Last_Dock[3]; //user ID of last dock
    BYTE Mem_Free[8];       //Free memory when last docked
    APP_VER_REC AppVer[10]; //Application version for up to 10 applications (4 bytes per application)
} PDTASSET_REC;

#define RCINDX           "RCINDX"                              // Recalls 22/5/2007 PAB
#define RCINDX_RECL      48L                                   // Recalls 22/5/2007 PAB // 14/04/2008 1.0 BMG
#define RCINDX_REP       741                                   // Recalls 22/5/2007 PAB
#define RCINDX_OFLAGS    A_READ | A_SHARE                      // Recalls 22/5/2007 PAB
typedef struct RCINDX_Record_Layout {                          // Recalls 22/5/2007 PAB
    BYTE abIndexNum[2];                                        // Recalls 22/5/2007 PAB
    BYTE acRecallID[4];                                 // Recalls 22/5/2007 PAB
    BYTE abRecallDesc[20];                                     // Recalls 22/5/2007 PAB
    BYTE abActiveDate[4];                                      // Recalls 22/5/2007 PAB
    BYTE cRecallType;                                          // Mult UOD Rcll 12-8-2011 SDH
    BYTE abItemCount[2];                                       // Recalls 22/5/2007 PAB
    BYTE cRecallStatus;                                        // Mult UOD Rcll 12-8-2011 SDH
    BYTE cSpecialIns;                                          // Mult UOD Rcll 12-8-2011 SDH
    BYTE abLabelType[2];
    BYTE abExpiryDate[4];                                                               // 14/04/2008 1.0 BMG
    BYTE cMRQ[2];                                                                       // 06/05/2008 1.0 BMG
    BYTE abFiller[5];                                          // Recalls 22/5/2007 PAB // 14/04/2008 1.0 BMG
} RCINDX_REC;                                                  // Recalls 22/5/2007 PAB
                                                               // Recalls 22/5/2007 PAB
#define RECOK           "RECOK"                                // Recalls 22/5/2007 PAB
#define RECOK_RECL      80L                                    // Recalls 22/5/2007 PAB
#define RECOK_REP       742                                    // Recalls 22/5/2007 PAB
#define RECOK_OFLAGS    A_READ | A_SHARE                       // Recalls 22/5/2007 PAB
typedef struct RECOK_Record_Layout {                           // Recalls 22/5/2007 PAB
    BYTE cPSB70Processing;                                     // Recalls 22/5/2007 PAB
    BYTE cIRFRefrsh;                                           // Recalls 22/5/2007 PAB
    BYTE cFlag2;                                               // Recalls 22/5/2007 PAB
    BYTE cFlag3;                                               // Recalls 22/5/2007 PAB
    BYTE cDate[8];                                             // Recalls 22/5/2007 PAB
    BYTE cTime[6];                                             // Recalls 22/5/2007 PAB
    BYTE abFiller[62];                                         // Recalls 22/5/2007 PAB
} RECOK_REC;                                                   // Recalls 22/5/2007 PAB
                                                               // Recalls 22/5/2007 PAB
#define RCSPI           "RCSPI"                                // Recalls 22/5/2007 PAB
#define RCSPI_RECL      168L                                   // Recalls 22/5/2007 PAB
#define RCSPI_KEYL      8                                      // Recalls 22/5/2007 PAB
#define RCSPI_REP       743                                    // Recalls 22/5/2007 PAB
#define RCSPI_OFLAGS    A_READ | A_SHARE                       // Recalls 22/5/2007 PAB
typedef struct {                                               // Recalls 22/5/2007 PAB
    BYTE acRecallID[8];                                 // Recalls 22/5/2007 PAB
    BYTE abMessage[160];                                       // Recalls 22/5/2007 PAB
} RCSPI_REC;                                                   // Recalls 22/5/2007 PAB
                                                               // Recalls 22/5/2007 PAB
#define RECALL           "RECALLS"                             // Recalls 22/5/2007 PAB
#define RECALL_RECL      508L                                  // Recalls 22/5/2007 PAB
#define RECALL_KEYL      9                                     // Recalls 22/5/2007 PAB
#define RECALL_REP       740                                   // Recalls 22/5/2007 PAB
#define RECALL_OFLAGS    A_READ | A_WRITE| A_SHARE             // Recalls 22/5/2007 PAB
typedef struct {                                               // Recalls 22/5/2007 PAB
    BYTE abItemCode[3];                                        // Recalls 22/5/2007 PAB
    BYTE anCountTSF[2];                                        // Recalls 20/6/2007 PAB
    BYTE anSessionCount[2];                                    // Recalls 20/6/2007 PAB
    BYTE cCountUpdatedToday;                                   // Mult UOD Rcll 12-8-2011 SDH
} RecallItem;                                                 // Recalls 22/5/2007 PAB
typedef struct {                                               // Recalls 22/5/2007 PAB
    BYTE acRecallID[8];                                 // Recalls 22/5/2007 PAB
    UBYTE ubChain;                                             // Recalls 22/5/2007 PAB
    BYTE cRecallType;                                          // Recalls 22/5/2007 PAB
    BYTE abDescription[20];                                    // Recalls 22/5/2007 PAB
    BYTE anLabelType[8];                                       // Recalls 22/5/2007 PAB
    BYTE cSupplyRoute;                                         // Mult UOD Rcll 12-8-2011 SDH
    BYTE cReasonCode;                                          // Mult UOD Rcll 12-8-2011 SDH
    BYTE cBusinessCentre;                                      // Mult UOD Rcll 12-8-2011 SDH
    BYTE abActiveDate[8];                                      // Recalls 22/5/2007 PAB
    BYTE abDueDate[8];                                         // Recalls 22/5/2007 PAB
    BYTE abCompleteDate[8];                                    // Recalls 22/5/2007 PAB
    BYTE cRecallStatus;                                        // Mult UOD Rcll 12-8-2011 SDH
    BYTE abBatchNumbers[30];                                   // Recalls 22/5/2007 PAB
    BYTE anItemCount[4];                                       // Recalls 22/5/2007 PAB
    RecallItem aRecallItems[50];                              // Recalls 22/5/2007 PAB
    BYTE abFiller[8];                                          // Recalls 22/5/2007 PAB
} RECALL_REC;                                                  // Recalls 22/5/2007 PAB


#endif /* RFSFILE_H not defined */

