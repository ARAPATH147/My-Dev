'''****************************************************************************
''' <FileName>GOExportDataStructure.vb</FileName>
''' <summary>
''' Declaration of the structure of the transact message required for export 
''' data file
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>08-Dec-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''****************************************************************************

Public Structure UOSRecord
    'Structure to create UOS
    Dim strIsListType As String
End Structure
''' <summary>
''' Structure to create export data for UOA 
''' </summary>
''' <remarks></remarks>
Public Structure UOARecord
    Dim strListnumber As String
    Dim strsequencenumber As String
    Dim strbootscode As String
    Dim strquanity As String
    Dim strsdescription As String
    Dim strdescSEL As String
    Dim strnumPrice As String
    Dim strnumTotalPrice As String
    Dim stritembc As String
    Dim strbcname As String
    Dim strbarcode As String
    Dim strIsStatus As String
End Structure
''' <summary>
''' Structure to create export data for UOX 
''' </summary>
''' <remarks></remarks>
Public Structure UOXRecord
    Dim strlistnumber As String
    Dim strisListType As String
    Dim strUOD As String
    Dim strIsStatus As String
    Dim strItemCount As String
    Dim strIsStockFigure As String
    Dim strSupplierRoute As String
    Dim strDisplayLoc As String
    Dim strBCname As String
    Dim strBCdesc As String
    Dim strRecall As String
    Dim strAuthCode As String
    Dim strSupplier As String
    Dim strMethod As String
    Dim strCarrier As String
    Dim strNumbird As String
    Dim strNumReason As String
    Dim strRecStore As String
    Dim strDestination As String
    Dim strWroute As String
    Dim strIsUODType As String
    Dim strReasonDamage As String
End Structure
''' <summary>
''' Structure to create export data for RCB
''' </summary>
''' <remarks></remarks>
Public Structure RCBRecord
    Dim strRecallref As String
    Dim strnumUOD As String
    Dim strStateCall As String
End Structure
''' <summary>
''' Structure to create export data for RCG
''' </summary>
''' <remarks></remarks>
Public Structure RCGRecord
    Dim strRecallRef As String
    Dim strRecallItem As String
    Dim strRecallCount As String
End Structure

''' <summary>
''' To add for User Auth
''' </summary>
''' <remarks></remarks>
Public Structure SORRecord
    Dim strpassword As String
    Dim strAppID As String
    Dim strAddressMAC As String
    Dim strIsType As String
    Dim stAddressIP As String
    Dim strFreeMem As String
End Structure
''' <summary>
''' Structure to get the required values for STQ message.
''' </summary>
''' <remarks></remarks>
Public Structure STQRecord
    Dim strUODNumber As String
End Structure

Public Structure UORRecord
    Dim strOperatorId As String
    Dim strListNumber As String
    Dim strBusinessCentre As String
End Structure

Public Structure DSRRecord
    Dim cBusinessCentre As Char
    Dim strSequenceNo As String
    Dim strSupplierNo As String
    Dim strSupplierName As String
End Structure

Public Structure DSERecord
    Dim strTID As String
End Structure

Public Structure STRRecord
    Dim strUODNo As String
    Dim strUODSuffix As String
End Structure

Public Structure RCCRecord
    Dim strOperatorID As String
    Dim strIndex As String
    Dim strRecallRefNo As String
    Dim cRecallType As Char
    Dim strRecallDesc As String
    Dim strRecallCount As String
    Dim strActiveDate As String
    Dim cSpecialInst As Char '"Y" then Special Instructions available
    Dim strLabType As String
    Dim strMRQ As String
    'Tailoring
    Dim strTailored As Boolean
    Dim strListStatus As String
    Dim strBatchNos As String
End Structure

Public Structure RCERecord
    Dim strOperatorId As String
End Structure

Public Structure RCFInnerRecord
    Dim strRecallItem As String
    Dim strItemDesc As String
    Dim strTSF As String
    Dim strRecallCount As String
    Dim cItemFlag As Char
    'Recalls CR
    Dim cVisible As Char
End Structure

Public Structure RCFRecord
    Dim strRecallRef As String
    Dim cRecallStatus As Char
    Dim InnerRecordArrays As ArrayList
End Structure


Public Structure RCJRecord
    Dim strOperatorID As String
    Dim strRecallRefNo As String
    Dim strSpecialInst As String
End Structure

Public Structure SNRRecord
    Dim strOperatorID As String
    Dim cAuthorityFlag As Char
    Dim strUserName As String
    Dim strDateTime As String
    Dim cOSSRFlag As Char
    '' This Printer Configuration is not applicable to MC70 Devices
    Dim strPrinterNumber As String
    Dim strPrinterDescription As String
    Dim cFiller As Char
    Dim strAccessStock As String
End Structure

''' <summary>
''' NAK Record Format 
''' </summary>
''' <remarks>Negative Acknowledgement format</remarks>
Public Structure NAKRecord
    Dim strErrorMessage As String
    Dim isNAKERROR As Boolean
End Structure

Public Structure NAKERRORRecord
    Dim strErrorMessage As String
    Dim isNAKERROR As Boolean
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
    ''' <summary>
    ''' Boots Code of the PRoduct
    ''' </summary>
    ''' <remarks></remarks>
    Dim strBootsCode As String
    ''' <summary>
    ''' Parent Barcode. Need Not be Used. Use strBOOTSCODE Instead
    ''' </summary>
    ''' <remarks></remarks>
    Dim strParentBC As String
    ''' <summary>
    ''' 45 Character Item Description of the Product
    ''' </summary>
    ''' <remarks></remarks>
    Dim strItemDesc As String
    ''' <summary>
    ''' The Price of the Item
    ''' </summary>
    ''' <remarks></remarks>
    Dim strPrice As String
    ''' <summary>
    ''' one line description, that is displyed in the SEL. also refered as short description
    ''' </summary>
    ''' <remarks></remarks>
    Dim strSELDesc As String
    ''' <summary>
    ''' Status of the item " "-active, "X/Z" - Discontinued
    ''' </summary>
    ''' <remarks></remarks>
    Dim cStatus As Char
    ''' <summary>
    ''' Supply Method " " - Unknown, "D" - Direct, "C" CSR, "W" Warehouse
    ''' </summary>
    ''' <remarks></remarks>
    Dim cSupply As Char
    ''' <summary>
    ''' Reedemable Flag "*" - Customer can Reedeme the points on this product and " " - not possible
    ''' </summary>
    ''' <remarks></remarks>
    Dim cReedemable As Char
    ''' <summary>
    ''' The Theoritical Stock Figure of the Item
    ''' </summary>
    ''' <remarks></remarks>
    Dim strStockFigure As String
    ''' <summary>
    ''' Price Check Target 
    ''' </summary>
    ''' <remarks></remarks>
    Dim strPriceCHKTarget As String
    ''' <summary>
    ''' Price check cmpleted
    ''' </summary>
    ''' <remarks></remarks>
    Dim strPriceCHKDone As String
    ''' <summary>
    ''' Price in ECU, unsigned left right justified, space zero filled , all zeroes if EMU not active
    ''' </summary>
    ''' <remarks></remarks>
    Dim strEMUPrice As String
    ''' <summary>
    ''' Primary Currency "S"- sterling , "E" -Euro
    ''' </summary>
    ''' <remarks></remarks>
    Dim cPrimaryCurrency As String
    ''' <summary>
    ''' Barcode or Boots code, right justified, zero filled eg. 5045095680114
    ''' </summary>
    ''' <remarks></remarks>
    Dim strBarcode As String
    ''' <summary>
    ''' Active Deal "Y" - deal Present , "N" - No Deal
    ''' </summary>
    ''' <remarks></remarks>
    Dim cActiveDeal As Char
    ''' <summary>
    ''' Price Check Status "Y" - if Price accepted, "N" -Not Accepted
    ''' </summary>
    ''' <remarks></remarks>
    Dim cFlagPriceCheck As Char
    ''' <summary>
    ''' If Price Check is Rejected then the reason fr the rejection
    ''' </summary>
    ''' <remarks></remarks>
    Dim strRejectMessage As String
    ''' <summary>
    ''' Business Centre Flag , Value from "A" - "Z"
    ''' </summary>
    ''' <remarks></remarks>
    Dim cBusinessCentre As Char
    ''' <summary>
    ''' Business Centre Description Example: "Health Care"
    ''' </summary>
    ''' <remarks></remarks>
    Dim strBCDescription As String
    Dim cOSSRFlag As Char
    Dim arrDealSum As ArrayList
    Dim strCorePlannerLoc As String
    Dim strSalesPlanerLoc As String
    Dim cRecallItem As Char
    Dim cMarkDown As Char
    'For AFF PL CR-14/04/2010
    Dim strProductGp As String
    Dim strRecallType As String

End Structure