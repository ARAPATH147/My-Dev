''' """""""""""""""""""""""""""""""""""""""""""
''' Generic Messages Sent from POD
''' """""""""""""""""""""""""""""""""""""""""""
''' <summary>
''' SOR Record Structure
''' </summary>
''' <remarks>Sign on Request Message Format</remarks>

Public Enum ENQType
    Item
    Parent
End Enum

Public Enum ENQFunction
    PriceCheck
    FastFill
    Other
End Enum

Public Structure SORRecord
    Dim strOperatorID As String
    Dim strPassword As String
    Dim strApplicationID As String
    Dim strAppVersion As String
    Dim strMacAddress As String
    Dim strDeviceType As String
    Dim strIP As String
    Dim strFreeMemory As String
End Structure
''' <summary>
''' OFF Record Structure
''' </summary>
''' <remarks>sign off record format</remarks>
Public Structure OFFRecord
    Dim strOperatorID As String
End Structure
''' <summary>
''' ALR Record Structure
''' </summary>
''' <remarks>Application Launch Request Format</remarks>
Public Structure ALRRecord
    Dim strOperatorID As String
    Dim strApplication As String
End Structure


'''' <summary>Generic Messages - Recieved</summary>
'''' """""""""""""""""""""""""""""""""""""""""""
'''' Generic Messages Recieved
'''' """""""""""""""""""""""""""""""""""""""""""

'''' <summary>
'''' SNR Record Format
'''' </summary>
'''' <remarks>Sign On Response - Different for MC70 and PPC</remarks>
'''' <include > Printer Configuration is not applicable to MC70 Devices</include>
'Public Structure SNRRecord
'    Dim strOperatorID As String
'    Dim cAuthorityFlag As Char
'    Dim strUserName As String
'    Dim dDateTime As Date
'    Dim cOSSRFlag As Char
'    '' This Printer Configuration is not applicable to MC70 Devices
'    Dim strPrinterNumber As String
'    Dim strPrinterDescription As String
'End Structure
''' <summary>
''' NAK Record Format 
''' </summary>
''' <remarks>Negative Acknowledgement format</remarks>
Public Structure NAKRecord
    Dim strErrorMessage As String
End Structure

Public Structure NAKERRORRecord
    Dim strErrorMessage As String
End Structure
''' <summary>
''' ACK Record Structure
''' </summary>
''' <remarks>Acknowledgement Message Format</remarks>
Public Structure ACKRecord
    Dim strAckMessage As String
End Structure
''' <summary>
''' EQR Record Format
''' </summary>
''' <remarks>Enquiry Response Message Format</remarks>
Public Structure EQRRecord
    Dim strBootsCode As String              'Boots code
    Dim strParentBC As String               'Parent Boots code
    Dim strItemDesc As String               'Item Description
    Dim strPrice As String                  'Current Price of the item
    Dim strSELDesc As String                'SEL description
    Dim cStatus As Char                     'Item status ' ' - active, X, Z - discontinued
    Dim cSupply As Char                     'Supply route
    Dim cReedemable As Char                 'Advantage Redeeemable
    Dim strStockFigure As String            'Theoritical stock figure
    Dim strPriceCHKTarget As String         'Price check target for the week for the store
    Dim strPriceCHKDone As String           'Price check done so far in the store.
    Dim strEMUPrice As String               'EMU price
    Dim cPrimaryCurrency As String          'Primary currency S - Sterlin, E - Euro
    Dim strBarcode As String                'Product barcode of the item
    Dim cActiveDeal As Char                 'Is the item on active deal.
    Dim cFlagPriceCheck As Char             'Is price check accepted
    Dim strRejectMessage As String          'If price check rejected then, message contaisn reason.
    Dim cBusinessCentre As Char             'Business centre letter for the item
    Dim strBCDescription As String          'Business centere description
    Dim cOSSRFlag As Char                   'OSSR flag for the item.
    Dim arrDealSum As ArrayList             'Summary of first 10 deals for the item scanned.
    Dim strCorePlannerLoc As String         'Core planner 1
    Dim strSalesPlanerLoc As String         'Sales planner 1
    Dim cRecallItem As Char                 'Is item on recall
    Dim cMarkDown As Char                   'Is item on mark down.
    Dim cRecallType As Char
    Dim strPGGroup As String
    Dim cPendSale As Char                   'Added as per SFA - Pending Sales Plan
End Structure

'''' <summary> Shelf Monitor Messages</summary>
'''' """"""""""""""""""""""""""""""""""""""""""
'''' SM Sent Message Structures
'''' """"""""""""""""""""""""""""""""""""""""""

''' <summary>
''' GAS Record Structure
''' </summary>
''' <remarks>Shelf Monitor Start Record format</remarks>
Public Structure GASRecord
    Dim strOperatorID As String
End Structure



''' <summary>
''' GAR Record Structure
''' </summary>
''' <remarks>Shelf Monitor Start Response Format</remarks>
Public Structure GARRecord
    Dim strListID As String
    Dim strPriceCHK_Target As String
    Dim strPriceCHK_Done As String
End Structure



Public Structure PLORecord
    Dim strOperatorID As String
End Structure

Public Structure PLFrecord
    Dim strOperatorID As String
End Structure

Public Structure PLSRecord
    Dim strOperatorID As String
    Dim strListID As String
    Dim strSequence As String
End Structure

Public Structure PLRRecord
    Dim strOperatorID As String
    Dim strSequence As String
    Dim cAuthorityFlag As Char
End Structure

Public Structure PLIRecord
    Dim strListID As String
    Dim strSequence As String
    Dim strBootsCode As String
    Dim strParentBootsCode As String
    Dim strDescription As String
    Dim strRequired As String
    Dim cStatus As Char
    Dim cGapFlags As Char
    Dim cActiveDeal As Char
    Dim strStockFigure As String
    Dim strSEL_Desc As String
    Dim strBarcode As String
    Dim strShelfQuantity As String
    Dim strBackShopQuantity As String
    Dim cOSSR_Flag As Char
    Dim cMS_Flag As Char
   
End Structure
Public Structure PLERecord
    Dim strOperatorID As String
End Structure
Public Structure PLLRecord
    Dim strListID As String
    Dim strSeqNumber As String
    Dim cStatus As Char
    Dim DateTime As String
    Dim strLines As String
    Dim strDisplayName As String
    Dim cStockLocation As Char
    Dim strPickerID As String
End Structure
Public Structure PCSRecord
    Dim strOperatorID As String
End Structure
''Public Structure PCMRecord
''    Dim strOperatorID As String
''    Dim strBootsCode As String
''    Dim strVariance As String
''End Structure

'Public Structure PCXRecord
'    Dim strOperatorID As String
'    Dim strItemsChecked As String
'    Dim strSELs As String
'End Structure

Public Structure PCRRecord
    Dim strPriceChkTarget As String
    Dim strPriceChkDone As String
End Structure

' ''Public Structure PRTRecord
' ''    Dim strOperatorID As String
' ''    Dim strBC As String
' ''    Dim cMethod As Char
' ''End Structure

Public Structure INSRecord
    Dim strOperatorID As String
End Structure

Public Structure INXRecord
    Dim strOperatorID As String
End Structure

'Public Structure PRPRecord
'    Dim strOperatorID As String
'    Dim strPOG_Key As String
'    Dim strSequence As String
'    Dim cMethod As String
'End Structure
Public Structure CLORecord
    Dim strOperatorID As String
End Structure

Public Structure CLFRecord
    Dim strOperatorID As String
End Structure

Public Structure CLSRecord
    Dim strOperatorID As String
    Dim strListID As String
End Structure

Public Structure CLRRecord
    Dim strOperatorID As String
    Dim strSequence As String
End Structure
Public Structure CLIRecord
    Dim strListID As String
    Dim strItemInList As String             'Number of Items in list - Added as per SFA 
    Dim strItemSeq As String
    Dim strMoreToCome As String
    Dim strSequence As String
    Dim strBootsCode As String
    Dim strParentBootsCode As String
    Dim strBarcode As String
    Dim strSEL_Desc As String
    Dim cActiveDeal As Char
    Dim strProdGroup As String
    Dim iSF_Count As Integer
    Dim iBSMBS_Count As Integer
    Dim iBSPSP_Count As Integer
    Dim iOSSRMBS_Count As Integer
    Dim iOSSRPSP_Count As Integer
    Dim cStatus As Char
    Dim cOSSR_Flag As Char
    Dim strDateLastCount As String
    Dim strPendSale_Flag As Char
    Dim strStockFigure As String
    Dim strNextItem As String               'Delimiter to mark end of item info - Added as per SFA
End Structure
Public Structure CLLRecord
    Dim strReturnedListID As String
    Dim strSeqNumber As String
    Dim strTotalItems As String
    Dim strNo_SF_Items As String
    Dim strNo_BS_Items As String
    Dim cListType As Char
    Dim strBusinessGroup As String
    Dim strOSSR_Item As String
    Dim cActiveFlag As Char
    Dim strLastCntDate As String            'Added as per SFA
    Dim strCounterID As String              'Added as per SFA CR08
End Structure

Public Structure CLERecord
    Dim strListID As String
End Structure
Public Structure CLBRecord
    Dim strListID As String
    'Dim strSequence As String
End Structure
'Public Structure CLGRecord
'    Dim strListID As String
'    Dim strSequence As String
'End Structure

Public Structure RPORecord
    Dim strOperatorID As String
End Structure

Public Structure RUPInnerStructure
    Dim strData As String
    Dim cLevel As Char
    Dim cFunction As Char
End Structure
Public Structure RUPRecord
    Dim strCount As String
    Dim arrRUPInnerStucture As ArrayList
End Structure

Public Structure RLDInnerStructure
    Dim strSequence As String
    Dim strData As String
End Structure
Public Structure RLDRecord
    Dim strCount As String
    Dim arrRLDInnerStructure As ArrayList
End Structure
Public Structure RPXRecord
    Dim strOperatorID As String
End Structure

Public Structure RPSRecord
    Dim strOperatorID As String
    Dim strReportID As String
    Dim strSequenceNo As String
End Structure

Public Structure RLSRecord
    Dim strOperatorId As String
    Dim strReportID As String
    Dim strSequenceNo As String
End Structure
Public Structure RLERecord
    Dim strOperatorId As String
    Dim strSequenceNo As String
End Structure

Public Structure SSERecord
    Dim strOperatorID As String
End Structure

Public Structure ISERecord
    Dim strOperatorID As String
    Dim strBarcode As String
End Structure

Public Structure SSRRecord
    Dim strTodaysSales As String
    Dim strWeeklySales As String
End Structure

Public Structure PGSRecord
    Dim strOperatorID As String
End Structure


Public Structure PGXRecord
    Dim strOperatorID As String
End Structure

Public Structure PGFRecord
    Dim strOperatorID As String
    Dim strSequenceNo As String
    Dim cCoreFlag As Char
    Dim cPlannerType As Char
End Structure

Public Structure PGQRecord
    Dim strOperatorID As String
    Dim strPOG_IndexPtr As String
    Dim cPlannerType As Char
End Structure

Public Structure PGMRecord
    Dim strOperatorID As String
    Dim strPOG_Key As String
    Dim strModSeq As String
    Dim cPlannerType As Char
End Structure


Public Structure PPLRecord
    Dim strOperatorID As String
    Dim POG_DB_List As String
End Structure

Public Structure PSLRecord
    Dim strOperatorID As String
    Dim strPOG_DB_Key As String
    Dim strModSeq As String
    Dim strShelfNumber As String
    Dim cChainRecordSeq As Char
    Dim strChainRecordItem As String
End Structure

Public Structure PGLRecord
    Dim strOperatorID As String
    Dim strBootsCode As String
    Dim strSR_Index As String
    Dim cPlannerType As Char
End Structure

Public Structure PGGRecord
    Dim strSequence As String
    Dim strDescription As String
    Dim strPOG_StartPtr As String
    Dim strFamilyType As String
    Dim strHierachy As String
End Structure



Public Structure PGRRecord
    Dim strPOG_IndexPtr As String
    Dim strPOGDesc As String
    Dim strActDate As String
    Dim strDeacDate As String
    Dim strMODCount As String
End Structure


Public Structure PGNRecord
    Dim strMODSeq As String
    Dim strMODDesc As String
    Dim strShelfCount As String
    Dim cFilter As Char
End Structure
Public Structure ItemsOnShelf
    Dim strBootsCode As String
    Dim strItemDesc As String
    Dim strFacings As String
End Structure
Public Structure PSRRecord
    Dim strNotchNo As String
    Dim strShelfDesc As String
    Dim strModuleID As String
    Dim strShelfNumber As String
    Dim strBootsCode As String
    Dim strItemDesc As String
    Dim strFacings As String
End Structure

Public Structure PGIRecord
    Dim strPOGKEY As String
    Dim strPOGDESC As String
    Dim strMODCNT As String
End Structure
'anoop:start
Public Structure PGBRecord
    Dim strPOGKey As String
    Dim strModuleCount As String
    Dim strRepeatCount As String
    Dim strPOGDesc As String
    Dim strModuleDesc As String
    Dim strMDQ As String
    Dim strPSC As String
End Structure
'anoop:end
Public Structure LPRRecord
    Dim WASPrice1 As String
    Dim WASPrice2 As String
    Dim cPHF_SEL_Type As Char
    Dim cUnitPriceFlag As Char
    Dim strUnitMeasure As String
    Dim strUnitQuantity As String
    Dim strUnittype As String
    Dim cWEEE_Flag As String
    Dim strWEE_Price As String
    Dim charMS_Flag As Char
    Dim strPainKillerMessage As String
End Structure


''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''' ***************************************************************************
''' <FileName>SMExportDataManager</FileName>
'''  <summary>
'''  Declaration of all the structure which are required for the export data
'''  </summary>
'''  <Author>Infosys Technologies Ltd.,</Author>
'''  <DateModified>22-Dec-2008</DateModified>
'''  <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
'''  <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>           
''' ***************************************************************************
Public Structure CLCRecord
    ''' <summary>
    ''' Structure to create export data for Count List CLC
    ''' </summary>
    Dim strOprtrId As String
    Dim strListID As String
    Dim strNumberSEQ As String
    Dim strBootscode As String
    Dim strCountLocation As String
    Dim strCount As String
    Dim strUpdateOSSR As String
    Dim strSalesAtTimeOfUpload As String
End Structure
''' <summary>
''' Structure to create export data for Count List CLX
''' </summary>
Public Structure CLXRecord
    Dim strListID As String
    Dim cIsCommit As Char
    Dim strCountType As String
End Structure
''' <summary>
''' Structure to create export data for Count List CLA
''' </summary>
Public Structure CLARecord
    Dim strStatus As String
End Structure
''' <summary>
''' Structure to create export data for Count List CLD
''' </summary>
Public Structure CLDRecord
    Dim strListID As String
    Dim strSequence As String
    Dim cSitetype As Char
    Dim strBootsCode As String
End Structure
''' <summary>
''' Structure to create export data for GAP
''' </summary>
Public Structure GAPRecord
    Dim strNumberSEQ As String
    Dim strBarcode As String
    Dim strBootscode As String
    Dim strCurrentQty As String
    Dim strFillQty As String
    Dim cIsGAPFlag As Char
    Dim strStockFig As String
    'New fields added after new message format
    Dim strUpdateOssrItem As String     'value should be " " for SM, FF and EX
    Dim strLocCounted As String         'Multisite location.
End Structure
''' <summary>
''' Structure to create export data for GAX
''' </summary>
Public Structure GAXRecord
    Dim strPickListItems As String
    Dim strSELS As String
    Dim strPriceChk As String
End Structure
''' <summary>
''' This is for keeping the shop location
''' Not to be used with any other GAX Record
''' </summary>
''' <remarks>HAs to be used only in case of Excess Stock.</remarks>
Public Structure EX_GAX_Record
    Dim exGAXRecord As GAXRecord
    Dim currLocation As EXSessionMgr.LocationType
End Structure
''' <summary>
''' Structure to create export data for Price Check Mismatch PCM
''' </summary>
Public Structure PCMRecord
    Dim strBootscode As String
    Dim strNumVariance As String
    Dim strPriceCheck As String
End Structure
''' <summary>
''' Structure to create export data for Piking List PLC
''' </summary>
Public Structure PLCRecord
    Dim strOprtrID As String
    Dim strListID As String
    Dim strNumberSEQ As String
    Dim strBootscode As String
    Dim strStockCount As String
    Dim cIsGAPFlag As Char
    'Newly added fields on 01-Mar-2009
    Dim strPickListLocation As String
    Dim strOSSRCount As String
    Dim strUpdateOSSRItem As String
    Dim strLocationCounted As String
    Dim strAllMSPicked As String
End Structure
''' <summary>
''' Structure to create export data for Picking List PLX
''' </summary>
Public Structure PLXRecord
    Dim strListID As String
    Dim strLineActioned As String
    Dim strItems As String
    Dim cIsComplete As Char
End Structure
''' <summary>
''' Structure to create export data for Print SEL PRT
''' </summary>
Public Structure PRTRecord
    Dim strBootscode As String
    'System Testing - changed char to string
    Dim cIsMethod As String
End Structure
''' <summary>
''' Structure to create export data for PRP
''' </summary>
Public Structure PRPRecord
    Dim strPOGKey As String
    Dim strMODSequence As String
    Dim strIsType As String
End Structure
''' <summary>
''' Structure to create export data for PCX
''' </summary>
Public Structure PCXRecord
    Dim strCheckedItems As String
    Dim strSELs As String
End Structure
'UAT - ENQ for Price Check
''' <summary>
''' Structure to create export data for ENQ in price Check
''' </summary>

Public Structure ENQRecord
    Dim strOperatorId As String
    Dim cEnqType As ENQType
    Dim cFunction As ENQFunction
    Dim strBarCode As String
    Dim cStockFigReq As Char
    Dim cOSSRFlag As Char
    Dim cPlanner As Char
    'For NRF
    Dim strBootsCode As String
    Dim strPriceCheck As String
End Structure





'<TBD>
'----------------------------------------------------------------------------------
'''' """""""""""""""""""""""""""""""""""""""""""
'''' Generic Messages Sent from POD
'''' """""""""""""""""""""""""""""""""""""""""""
'''' <summary>
'''' SOR Record Structure
'''' </summary>
'''' <remarks>Sign on Request Message Format</remarks>
'Public Structure SORRecord
'    Dim strOperatorID As String
'    Dim strPassword As String
'    Dim strApplicationID As String
'    Dim strAppVersion As String
'    Dim strMacAddress As String
'    Dim cDeviceType As Char
'    Dim strIP As String
'    Dim strFreeMemory As String
'End Structure
''UAT - ENQ for Price Check
'''' <summary>
'''' Structure to create export data for ENQ in price Check
'''' </summary>
'Public Structure ENQRecord
'    Dim strOperatorId As String
'    Dim cEnqType As Char
'    Dim cFunction As Char
'    Dim strBarCode As String
'    Dim cStockFigReq As Char
'    Dim cOSSRFlag As Char
'    Dim cPlanner As Char
'End Structure
'''' <summary>
'''' OFF Record Structure
'''' </summary>
'''' <remarks>sign off record format</remarks>
'''' 
'Public Structure OFFRecord
'    Dim strOperatorID As String
'End Structure
'''' <summary>
'''' ALR Record Structure
'''' </summary>
'''' <remarks>Application Launch Request Format</remarks>
'Public Structure ALRRecord
'    Dim strOperatorID As String
'    Dim Application As String
'End Structure


'''' <summary>Generic Messages - Recieved</summary>
'''' """""""""""""""""""""""""""""""""""""""""""
'''' Generic Messages Recieved
'''' """""""""""""""""""""""""""""""""""""""""""

'''' <summary>
'''' SNR Record Format
'''' </summary>
'''' <remarks>Sign On Response - Different for MC70 and PPC</remarks>
'''' <include > Printer Configuration is not applicable to MC70 Devices</include>
Public Structure SNRRecord
    Dim strOperatorID As String
    Dim cAuthorityFlag As Char
    Dim strUserName As String
    Dim strDateTime As String
    Dim cOSSRFlag As Char
    '' This Printer Configuration is not applicable to MC70 Devices
    Dim strPrinterNumber As String
    Dim strPrinterDescription As String
    Dim cStockAccess As Char    'Added as per SFA requirement
End Structure
'''' <summary>
'''' NAK Record Format 
'''' </summary>
'''' <remarks>Negative Acknowledgement format</remarks>
'Public Structure NAKRecord
'    Dim strErrorMessage As String
'End Structure
'''' <summary>
'''' ACK Record Structure
'''' </summary>
'''' <remarks>Acknowledgement Message Format</remarks>
'Public Structure ACKRecord
'    Dim strAckMessage As String
'End Structure
'''' <summary>
'''' EQR Record Format
'''' </summary>
'''' <remarks>Enquiry Response Message Format</remarks>
'Public Structure EQRRecord
'    Dim strBootsCode As String
'    Dim strParentBC As String
'    Dim strItemDescription As String
'    Dim strPrice As String
'    Dim strSelDescription As String
'    Dim cStatus As Char
'    Dim cSupply As Char
'    Dim cReedemable As Char
'    Dim strStockFigure As String
'    Dim strPriceCHKTarget As String
'    Dim strPriceCHKDone As String
'    Dim strEMUPrice As String
'    Dim cPrimaryCurrency As Char
'    Dim strBarcode As String
'    Dim cActiveDeal As Char
'    Dim cFlagPriceCheck As Char
'    Dim strRejectMessage As String
'    Dim cBusinessCentre As Char
'    Dim strBCDescription As String
'    Dim cOSSRFlag As Char
'    Dim strDealSum As String
'    Dim strCorePlannerLoc As String
'    Dim strSalesPlanerLoc As String
'    Dim cRecallItem As Char
'    Dim cMarkDown As Char
'End Structure




'''' <summary> Shelf Monitor Messages</summary>
'''' """"""""""""""""""""""""""""""""""""""""""
'''' SM Sent Message Structures
'''' """"""""""""""""""""""""""""""""""""""""""

'''' <summary>
'''' GAS Record Structure
'''' </summary>
'''' <remarks>Shelf Monitor Start Record format</remarks>
'Public Structure GASRecord
'    Dim strOperatorID As String
'End Structure

'''' <summary>
'''' GAR Record Structure
'''' </summary>
'''' <remarks>Shelf Monitor Start Response Format</remarks>
'Public Structure GARRecord
'    Dim strListID As String
'    Dim strPriceCHK_Target As String
'    Dim strPriceCHK_Done As String
'End Structure

''''<Summary>Message Structure for Picking List </Summary>

'Public Structure PLORecord
'    Dim strOperatorID As String
'End Structure

'Public Structure PLFrecord
'    Dim strOperatorID As String
'End Structure

'Public Structure PLSRecord
'    Dim strOperatorID As String
'    Dim strListID As String
'    Dim strSequence As String
'End Structure

'Public Structure PLRRecord
'    Dim strOperatorID As String
'    Dim strSequence As String
'    Dim cAuthorityFlag As Char
'End Structure

'Public Structure PLIRecord
'    Dim strListID As String
'    Dim strSequence As String
'    Dim strBootsCode As String
'    Dim strParentBootsCode As String
'    Dim strDescription As String
'    Dim strRequired As String
'    Dim cStatus As Char
'    Dim cGapFlags As Char
'    Dim cActiveDeal As Char
'    Dim strStockFigure As String
'    Dim strSEL_Desc As String
'    Dim strBarcode As String
'    Dim strShelfQuantity As String
'    Dim strBackShopQuantity As String
'    Dim cOSSR_Flag As Char
'    Dim cMS_Flag As Char
'End Structure
'Public Structure MSARecord
'    Dim strOperatorId As String
'    Dim strListId As String
'    Dim strSeqNumber As String
'End Structure
'Public Structure PLERecord
'    Dim strListID As String
'End Structure


''''<summary>Message Structure for Price Check </summary>
'Public Structure PCSRecord
'    Dim strOperatorID As String
'End Structure
''Public Structure PCMRecord
''    Dim strOperatorID As String
''    Dim strBootsCode As String
''    Dim strVariance As String
''End Structure

''Public Structure PCXRecord
''    Dim strOperatorID As String
''    Dim strItemsChecked As String
''    Dim strSELs As String
''End Structure

'Public Structure PCRRecord
'    Dim strPriceChkTarget As String
'    Dim strPriceChkDone As String
'End Structure

'' ''Public Structure PRTRecord
'' ''    Dim strOperatorID As String
'' ''    Dim strBC As String
'' ''    Dim cMethod As Char
'' ''End Structure

''<Summary> Message Structure for Print SEL </Summary>

'Public Structure INSRecord
'    Dim strOperatorID As String
'End Structure

'Public Structure INXRecord
'    Dim strOperatorID As String
'End Structure

'Public Structure LPRRecord
'    Dim strWASPrice1 As String
'    Dim strWASPrice2 As String
'    Dim cPHF_SEL_Type As Char
'    Dim cUnitPriceFlag As Char
'    Dim strUnitMeasure As String
'    Dim strUnitQuantity As String
'    Dim strUnittype As String
'    Dim cWEEE_Flag As Char
'    Dim strWEE_Price As String
'    Dim cMS_Flag As Char
'    Dim strPainKillerMsg As String
'End Structure

''Public Structure PRPRecord
''    Dim strOperatorID As String
''    Dim strPOG_Key As String
''    Dim strSequence As String
''    Dim cMethod As String
''End Structure

''<Summary>Message Structure for Count List </Summary>

'Public Structure CLORecord
'    Dim strOperatorID As String
'End Structure


'Public Structure CLFRecord
'    Dim strOperatorID As String
'End Structure



'Public Structure CLSRecord
'    Dim strOperatorID As String
'    Dim strListID As String
'    Dim strSequence As String
'End Structure

'Public Structure CLRRecord
'    Dim strOperatorID As String
'    Dim strListId As String
'End Structure

'Public Structure CLIRecord
'    Dim strListID As String
'    Dim strSequence As String
'    Dim strBootsCode As String
'    Dim strParentBootsCode As String
'    Dim strBarcode As String
'    Dim strSEL_Desc As String
'    Dim cActiveDeal As Char
'    Dim strProdGroup As String
'    Dim strBS_Count As String
'    Dim strSF_Count As String
'    Dim cStatus As Char
'    Dim strOSSR_Count As String
'    Dim cOSSR_Flag As Char
'End Structure

'Public Structure CLLRecord
'    Dim strReturnedList As String
'    Dim strSeqNumber As String
'    Dim strTotalItems As String
'    Dim strNo_SF_Items As String
'    Dim strNo_BS_Items As String
'    Dim cListType As Char
'    Dim strBusinessGroup As String
'    Dim strOSSR_Item As String
'    Dim cActiveFlag As Char
'End Structure

'Public Structure CLERecord
'    Dim strListID As String
'End Structure
''<Summary> Message Structure for reports </Summary>

'Public Structure RPORecord
'    Dim strOperatorID As String
'End Structure

'Public Structure RLERecord
'    Dim strOperatorId As String
'    Dim strSequenceSize As String
'End Structure
Public Structure RLRRecord
    Dim strSequence As String
    Dim strTitle As String
    Dim strReportId As String
End Structure

'Public Structure RUPStructure
'    Dim cLevel As Char
'    Dim cFunction As Char
'    Dim strHeaderText As String
'End Structure
'Public Structure RUPRecord
'    Dim strCount As String
'    Dim RUPStructure1 As RUPStructure
'    Dim RUPStructure2 As RUPStructure
'    Dim RUPStructure3 As RUPStructure
'    Dim RUPStructure4 As RUPStructure
'    Dim RUPStructure5 As RUPStructure
'    Dim RUPStructure6 As RUPStructure
'    Dim RUPStructure7 As RUPStructure
'    Dim RUPStructure8 As RUPStructure
'    Dim RUPStructure9 As RUPStructure
'    Dim RUPStructure10 As RUPStructure
'End Structure
'Public Structure RLDStructure
'    Dim strSequence As String
'    Dim strData As String
'End Structure

'Public Structure RLDRecord
'    Dim strCount As String
'    Dim RLDStructure1 As RLDStructure
'    Dim RLDStructure2 As RLDStructure
'    Dim RLDStructure3 As RLDStructure
'    Dim RLDStructure4 As RLDStructure
'    Dim RLDStructure5 As RLDStructure
'    Dim RLDStructure6 As RLDStructure
'End Structure
'Public Structure RPXRecord
'    Dim strOperatorID As String
'End Structure

'Public Structure RPSRecord
'    Dim strOperatorID As String
'    Dim strReportID As String
'    Dim strSequenceNo As String
'End Structure

'Public Structure RLSRecord
'    Dim strOperatorId As String
'    Dim strReportID As String
'    Dim strSequenceNo As String
'End Structure



''<Summary>Message Structure for store sales </summary>


'Public Structure SSERecord
'    Dim strOperatorID As String
'End Structure


'Public Structure ISERecord
'    Dim strOperatorID As String
'    Dim strBarcode As String
'End Structure

'Public Structure SSRRecord
'    Dim strTodaysSales As String
'    Dim strWeeklySales As String
'End Structure
Public Structure ISRRecord
    Dim strItemDescription As String
    Dim strItemSoldTody As String
    Dim strValueItemSoldToday As String
    Dim strItemSoldWeek As String
    Dim strValueItemSoldWeek As String
End Structure

Public Structure DQRRECORD
    Dim strDealNumber As String
    Dim strStartDate As String
    Dim strEndDate As String
    Dim strDealDesc As String
    Dim strDealType As String
End Structure

''<Summary> Message Structure for Item Info </Summary>

'Public Structure DNQRecord
'    Dim strOperatorID As String
'    Dim strDealNumber As String
'End Structure

'Public Structure DQRRecord
'    Dim strDealNum As String
'    Dim strStartDate As Date
'    Dim strEndDate As Date
'    Dim strDealDescription As String
'End Structure

''<Summary> Message Structure for Planners </Summary>

'Public Structure PGSRecord
'    Dim strOperatorID As String
'End Structure


'Public Structure PGXRecord
'    Dim strOperatorID As String
'End Structure

'Public Structure PGFRecord
'    Dim strOperatorID As String
'    Dim strSequenceNo As String
'    Dim cCoreFlag As Char
'    Dim cPlannerType As Char
'End Structure

'Public Structure PGQRecord
'    Dim strOperatorID As String
'    Dim strPOG_IndexPtr As String
'    Dim cPlannerType As Char
'End Structure

'Public Structure PGMRecord
'    Dim strOperatorID As String
'    Dim strPOG_Key As String
'    Dim strModSeq As String
'    Dim strFilter As String
'End Structure


'Public Structure PPLRecord
'    Dim strOperatorID As String
'    Dim POG_DB_List As String
'End Structure

'Public Structure PSLRecord
'    Dim strOperatorID As String
'    Dim strPOG_DB_Key As String
'    Dim strModSeq As String
'    Dim strShelfNumber As String
'    Dim strChainRecordSeq As String
'    Dim strChainRecordItem As String
'End Structure

'Public Structure PGLRecord
'    Dim strOperatorID As String
'    Dim strBootsCode As String
'    'Dim strSR_Index As String
'    Dim strStartChain As String
'    Dim strStartPlan As String
'    Dim cPlannerType As Char
'End Structure

Public Structure PGARecord
    Dim strOperatorId As String
    Dim strBootsCode As String
    Dim strStartChain As String
    Dim strStartMod As String
    Dim strStartItem As String
    Dim cPlannerType As Char
End Structure

'Public Structure PGGRecord
'    'Dim strOperatorID As String
'    Dim strSequence As String
'    Dim strDescription As String
'    Dim strPOG_StartPtr As String
'    Dim strFamilyType As String
'    Dim strHierachy As String
'    Dim cMoreToCome As Char
'End Structure

'Public Structure PGRStruct
'    Dim strPOG_IndexPtr As String
'    'Dim strPOG_Key As String
'    'Dim strPOGID As String
'    Dim strPOGDesc As String
'    Dim strActDate As Date
'    Dim strDeacDate As Date
'    Dim strMODCount As String

'End Structure
'Public Structure PGRRecord
'    'Dim strOperatorID As String
'    Dim pgrStruct1 As PGRStruct
'    Dim pgrStruct2 As PGRStruct
'    Dim pgrStruct3 As PGRStruct
'    Dim pgrStruct4 As PGRStruct
'    Dim strNxtPOGIndexPtr As String
'    ''Below Repeats FourTimes
'End Structure

'Public Structure PGNStruct
'    'Dim strPOGKey As String
'    Dim strMODSeq As String
'    Dim cFilter As Char
'    Dim strMODDesc As String
'    Dim strShelfCount As String
'End Structure
'Public Structure PGNRecord
'    'Dim strOperatorID As String
'    Dim pgnStruct1 As PGNStruct
'    Dim pgnStruct2 As PGNStruct
'    Dim pgnStruct3 As PGNStruct
'    Dim pgnStruct4 As PGNStruct
'    'Dim cMoreItems As Char
'End Structure

'Public Structure ItemsOnShelf
'    Dim strBootsCode As String
'    Dim strItemDesc As String
'    Dim strFacings As String
'End Structure
'Public Structure PSRRecord
'    'Dim strOperatorID As String
'    'Dim strPOGKey As String
'    Dim strMODSeq As String
'    Dim strShelfNo As String
'    Dim strNotchNo As String
'    Dim strShelfDesc As String
'    Dim strRecSeq As String
'    Dim structItemList As ItemsOnShelf()
'End Structure

''Public Structure PGIRecord
''    'Dim strOperatorID As String
''    Dim strBootsCode As String
''    Dim strItemSRIndex As String
''    Dim cPlannerFlag As String
''End Structure
'Public Structure PGIStruct
'    Dim strPlannerKey As String
'    Dim strPlannerDesc As String
'    Dim strModuleCount As String
'End Structure
'Public Structure PGIRecord
'    Dim PGIStruct1 As PGIStruct
'    Dim PGIStruct2 As PGIStruct
'    Dim PGIStruct3 As PGIStruct
'    Dim PGIStruct4 As PGIStruct
'    Dim strNextChain As String
'    Dim strNextModule As String
'End Structure

'Public Structure PPRRecord
'    Dim strOperatorID As String
'    Dim strPOGArray() As String
'    Dim strPOGDB As String
'    Dim strPOGDesc As String
'End Structure

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''' ***************************************************************************
'''' <FileName>SMExportDataManager</FileName>
''''  <summary>
''''  Declaration of all the structure which are required for the export data
''''  </summary>
''''  <Author>Infosys Technologies Ltd.,</Author>
''''  <DateModified>22-Dec-2008</DateModified>
''''  <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''''  <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>           
'''' ***************************************************************************
'Public Structure CLCRecord
'    ''' <summary>
'    ''' Structure to create export data for Count List CLC
'    ''' </summary>
'    Dim strOperatorId As String
'    Dim strListID As String
'    Dim strNumberSEQ As String
'    Dim strBootscode As String
'    Dim strBackShopCount As String
'    Dim strSalesFloorCount As String
'    'Dim strOssrCount As String
'    'Dim strUpdateOssrItem As String
'End Structure
'''' <summary>
'''' Structure to create export data for Count List CLX
'''' </summary>
'Public Structure CLXRecord
'    Dim strListID As String
'    Dim cIsCommit As Char
'End Structure
'''' <summary>
'''' Structure to create export data for GAP
'''' </summary>
'Public Structure GAPRecord
'    Dim strOperatorId As String
'    Dim strListId As String
'    Dim strNumberSEQ As String
'    Dim strBarcode As String
'    Dim strBootscode As String
'    Dim strCurrentQty As String
'    Dim strFillQty As String
'    Dim cIsGAPFlag As Char
'    Dim strStockFig As String
'    'New fields added after new message format
'    Dim cUpdateOssrItem As Char     'value should be " " for SM, FF and EX
'    Dim strLocCounted As String         'Multisite location.
'End Structure
'''' <summary>
'''' Structure to create export data for GAX
'''' </summary>
'Public Structure GAXRecord
'    Dim strOperatorId As String
'    Dim strListId As String
'    Dim strPickListItems As String
'    Dim strSELS As String
'    Dim strPriceChk As String
'End Structure
'''' <summary>
'''' Structure to create export data for Price Check Mismatch PCM
'''' </summary>
'Public Structure PCMRecord
'    Dim strOperatorId As String
'    Dim strBootscode As String
'    Dim strNumVariance As String
'    Dim cPrinterType As Char
'End Structure
'''' <summary>
'''' Structure to create export data for Piking List PLC
'''' </summary>
'Public Structure PLCRecord
'    Dim strOperatorId As String
'    Dim strListID As String
'    Dim strNumberSEQ As String
'    Dim strBootscode As String
'    Dim strStockCount As String
'    Dim cIsGAPFlag As Char
'    'Newly added fields on 01-Mar-2009
'    Dim cPickListLocation As Char
'    Dim strOSSRCount As String
'    Dim cUpdateOSSRItem As Char
'    Dim strLocationCounted As String
'    Dim cAllMSPicked As Char
'End Structure
'''' <summary>
'''' Structure to create export data for Picking List PLX
'''' </summary>
'Public Structure PLXRecord
'    Dim strOperatorId As String
'    Dim strListID As String
'    Dim strLineActioned As String
'    Dim strItems As String
'    Dim cIsComplete As Char
'End Structure
'''' <summary>
'''' Structure to create export data for Print SEL PRT
'''' </summary>
'Public Structure PRTRecord
'    Dim strOperatorId As String
'    Dim strBootscode As String
'    'System Testing - changed char to string
'    Dim cIsMethod As Char
'End Structure
'''' <summary>
'''' Structure to create export data for PRP
'''' </summary>
'Public Structure PRPRecord
'    Dim strOperatorId As String
'    Dim strPOGKey As String
'    Dim strMODSequence As String
'    Dim cIsType As Char
'End Structure
'''' <summary>
'''' Structure to create export data for PCX
'''' </summary>
'Public Structure PCXRecord
'    Dim strOperatorId As String
'    Dim strCheckedItems As String
'    Dim strSELs As String
'End Structure
'<TBD>
'----------------------------------------------------------------------------------





