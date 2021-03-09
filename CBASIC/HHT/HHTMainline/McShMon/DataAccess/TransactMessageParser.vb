Imports MCShMon.Message
#If RF Then

'''******************************************************************************
''' <FileName>TransactMessageParser.vb</FileName>
''' <summary>
''' This Class implements the parsers for all responce messages from the TRANSACT.
''' ParseResponse method of the ExDataTransmitter class should use this method.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>18-Oct-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for PPC</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''******************************************************************************
Public Class TransactMessageParser
    Private m_TempDataPool As ArrayList
    Private isFirstParse As Boolean
    ''' <summary>
    ''' Constructor for the class.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        isFirstParse = True
        m_TempDataPool = New ArrayList
    End Sub

    'This divison s according to the message Protocol document
    'The division is for better readability and logical division
#Region "Generic"
    ''' <summary>
    ''' Parse ACK message Parses the ACK Message string to ACK Structure
    ''' </summary>
    ''' <param name="strACK_Message">Message String</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function ParseACK(ByVal strACK_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim objACK As New ACKRecord
        Try
            With objACK

                If strACK_Message.Length > 3 Then
                    .strAckMessage = strACK_Message.Substring(ACK.MESSAGE_OFFSET)
                Else
                    .strAckMessage = "An ACK recieved without a message"
                End If
                If strACK_Message.Length > 3 Then
                    .strAckMessage = strACK_Message.Substring(ACK.MESSAGE_OFFSET)
                Else
                    .strAckMessage = "ACK Recieved Without A Message"
                End If
            End With
            '' Include Code for ACK PArsing
            DATAPOOL.getInstance.addObject(objACK)
            bTemp = True
            objACK = Nothing
        Catch ex As Exception
            bTemp = False
            AppMainModule.objAppContainer.objLogger.WriteAppLog(ex.Message.ToString(), Logger.LogLevel.RELEASE)
        Finally
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' Parse EQR parses EQR Messages
    ''' </summary>
    ''' <param name="strEQR_Message">Message From the controller</param>
    ''' <returns>The Parsing Status as boolean</returns>
    ''' <remarks></remarks>
    Public Function ParseEQR(ByVal strEQR_Message As String) As Boolean
        Dim bTemp As Boolean
        Dim icount As Integer = 0
        Try
            Dim objEQR As New EQRRecord
            With objEQR
                .strBootsCode = strEQR_Message.Substring(EQR.BCDE_OFFSET, EQR.BCDE)
                .strParentBC = strEQR_Message.Substring(EQR.PARENT_OFFSET, EQR.PARENT)
                .strItemDesc = strEQR_Message.Substring(EQR.DESC_OFFSET, EQR.DESC)
                .strPrice = strEQR_Message.Substring(EQR.PRICE_OFFSET, EQR.PRICE)
                .strSELDesc = strEQR_Message.Substring(EQR.SELDesc_OFFSET, EQR.SELDesc)
                .cStatus = strEQR_Message.Substring(EQR.STATUS_OFFSET, EQR.STATUS)
                .cSupply = strEQR_Message.Substring(EQR.SUPPLY_OFFSET, EQR.SUPPLY)
                .cReedemable = strEQR_Message.Substring(EQR.REDEEM_OFFSET, EQR.REDEEM)
                .strStockFigure = strEQR_Message.Substring(EQR.STOCKFIG_OFFSET, EQR.STOCKFIG)
                .strPriceCHKTarget = strEQR_Message.Substring(EQR.PCHKTARGET_OFFSET, EQR.PCHKTARGET)
                .strPriceCHKDone = strEQR_Message.Substring(EQR.PCHKDONE_OFFSET, EQR.PCHKDONE)
                .strEMUPrice = strEQR_Message.Substring(EQR.EMUPRICE_OFFSET, EQR.EMUPRICE)
                .cPrimaryCurrency = strEQR_Message.Substring(EQR.PRIMCURR_OFFSET, EQR.PRIMCURR)
                .strBarcode = strEQR_Message.Substring(EQR.BARCODE_OFFSET, EQR.BARCODE)
                .cActiveDeal = strEQR_Message.Substring(EQR.ACTVDEAL_OFFSET, EQR.ACTVDEAL)
                .cFlagPriceCheck = strEQR_Message.Substring(EQR.CHECKACCEPTED_OFFSET, EQR.CHECKACCEPTED)
                .strRejectMessage = strEQR_Message.Substring(EQR.REJECTMESSAGE_OFFSET, EQR.REJECTMESSAGE)
                .cBusinessCentre = strEQR_Message.Substring(EQR.BC_OFFSET, EQR.BC)
                .strBCDescription = strEQR_Message.Substring(EQR.BCDESC_OFFSET, EQR.BCDESC)
                .cOSSRFlag = strEQR_Message.Substring(EQR.OSSRITEM_OFFSET, EQR.OSSRITEM)
                .arrDealSum = New ArrayList()
                While (icount < EQR.MAX_Count_DEALSUM)
                    If (Not ((strEQR_Message.Substring((icount * EQR.DEALSUM + EQR.DEALSUM_OFFSET), EQR.DEALSUM)).Trim("0") = "")) Then
                        .arrDealSum.Add(strEQR_Message.Substring((icount * EQR.DEALSUM + EQR.DEALSUM_OFFSET), EQR.DEALSUM))
                        icount = icount + 1
                    Else
                        'When there is no more deals then the deal numbers will be all 0s.
                        Exit While
                    End If
                End While
                .strCorePlannerLoc = strEQR_Message.Substring(EQR.PLANLOC1_OFFSET, EQR.PLANLOC1)
                .strSalesPlanerLoc = strEQR_Message.Substring(EQR.PLANLOC2_OFFSET, EQR.PLANLOC2)
                .cRecallItem = strEQR_Message.Substring(EQR.RECALLITEM_OFFSET, EQR.RECALLITEM)
                .cMarkDown = strEQR_Message.Substring(EQR.MARKDOWN_OFFSET, EQR.MARKDOWN)
                .cRecallType = strEQR_Message.Substring(EQR.RECALLTYPE_OFFSET, EQR.RECALLTYPE)
                .strPGGroup = strEQR_Message.Substring(EQR.PGGRP_OFFSET, EQR.PGGRP)
                .cPendSale = strEQR_Message.Substring(EQR.PENDSALE_OFFSET, EQR.PENDSALE)    'Added as per SFA
            End With

            DATAPOOL.getInstance.addObject(objEQR)
            objAppContainer.objLogger.WriteAppLog("Parse EQR::Added EQR Record to the Data Pool", Logger.LogLevel.INFO)
            'since This is a Single Response Notify the data engine
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
            bTemp = True
        Catch ex As Exception
            bTemp = False
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' Parses NAK Message
    ''' </summary>
    ''' <param name="strNAK_Message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseNAK(ByVal strNAK_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Try
            '' Include Code to parse the NAK
            If strNAK_Message.StartsWith("NAKERROR") Then
                Dim objNAK As New NAKERRORRecord
                Dim CurrentActiveModule As AppContainer.ACTIVEMODULE = objAppContainer.objActiveModule
                objNAK.strErrorMessage = strNAK_Message.Substring(NAKERROR.MESSAGE_OFFSET)
                DATAPOOL.getInstance.addObject(objNAK)
                'SFA - Fix for DEF 781
                If Not (objNAK.strErrorMessage = "") Then
                    If CurrentActiveModule = AppContainer.ACTIVEMODULE.CUNTLIST AndAlso _
                                        CLSessionMgr.GetInstance.m_bIsCreateOwnList Then
                        MessageBox.Show(objNAK.strErrorMessage, _
                                        "Error", MessageBoxButtons.OK, _
                                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    Else
                        MessageBox.Show("Received error from controller " + objNAK.strErrorMessage, _
                                        "Error", MessageBoxButtons.OK, _
                                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                Else
                    'MessageBox.Show("NAK Error recieved Without an error message, Please Contact Support Help Desk", "NAK - ERROR", MessageBoxButtons.OK, _
                    '           MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    objAppContainer.objLogger.WriteAppLog("NAK ERROR WITHOUT A MESSAGE", Logger.LogLevel.RELEASE)
                End If
                    objNAK = Nothing
                Else
                    Dim objNAK As New NAKRecord
                    With objNAK
                        .strErrorMessage = strNAK_Message.Substring(NAK.MESSAGE_OFFSET)
                    End With
                    DATAPOOL.getInstance.addObject(objNAK)
                    objNAK = Nothing
                End If

                bTemp = True
                If Not m_TempDataPool Is Nothing Then
                    m_TempDataPool.Clear()
                End If
        Catch ex As Exception
            bTemp = False
            If Not m_TempDataPool Is Nothing Then
                m_TempDataPool.Clear()
            End If
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        End Try
        DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        Return bTemp
    End Function
    ''' <summary>
    ''' Parses the Sign On Response
    ''' </summary>
    ''' <param name="strSNR_Message">the message from the controller</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function ParseSNR(ByVal strSNR_Message As String)
        Dim bTemp As Boolean = False
        Dim objSNR As SNRRecord = New SNRRecord()
        Try
            With objSNR
                .cAuthorityFlag = strSNR_Message.Substring(SNR.AUTH_OFFSET, SNR.AUTH)
                .cStockAccess = strSNR_Message.Substring(SNR.STKACCESS_OFFSET, SNR.STKACCESS)
                .cOSSRFlag = strSNR_Message.Substring(SNR.OSSR_OFFSET, SNR.OSSR)
                .strDateTime = strSNR_Message.Substring(SNR.DATETIME_OFFSET, SNR.DATETIME)
                .strOperatorID = strSNR_Message.Substring(SNR.ID_OFFSET, SNR.ID)
                .strPrinterDescription = strSNR_Message.Substring(SNR.PRTDESC_OFFSET)
                .strPrinterNumber = strSNR_Message.Substring(SNR.PRTNUM_OFFSET, SNR.PRTNUM)
                .strUserName = strSNR_Message.Substring(SNR.UNAME_OFFSET, SNR.UNAME).Trim()
            End With
            DATAPOOL.getInstance.addObject(objSNR)
            bTemp = True
            objSNR = Nothing
        Catch ex As Exception
            ''Error handling
            DATAPOOL.getInstance.IsError = True
            DATAPOOL.getInstance.ErrorMessage = "Error in Parsing the DATA"
            bTemp = False
        Finally
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        End Try

        ParseSNR = bTemp
    End Function
#End Region

#Region "Shelf Monitor"
    ''' <summary>
    ''' Parses the GAP Record
    ''' </summary>
    ''' <param name="strGAR_Message">the String Message that has to be parsed</param>
    ''' <returns>Boolean if the Parse is Successful</returns>
    ''' <remarks></remarks>
    Public Function ParseGAR(ByVal strGAR_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim objGAR As GARRecord
        Try
            With objGAR
                .strListID = strGAR_Message.Substring(GAR.ID_OFFSET, GAR.ID)
                .strPriceCHK_Done = strGAR_Message.Substring(GAR.PRCHKDNE_OFFSET, GAR.PRCHKDNE)
                .strPriceCHK_Target = strGAR_Message.Substring(GAR.PRCHKTGT_OFFSET, GAR.PRCHKTGT)
            End With
            bTemp = True
            DATAPOOL.getInstance.addObject(objGAR)
        Catch ex As Exception
            ' TBD
            ' Logg the error
            bTemp = False
        End Try
        objGAR = Nothing
        DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        Return bTemp
    End Function
#End Region

#Region "Picking List"
    ''' <summary>
    ''' Parse PLI Request
    ''' </summary>
    ''' <param name="strPLI_Message">Message To be parsed</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParsePLI(ByVal strPLI_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim objPLI As PLIRecord
        Dim SequenceNo As String = ""
        Try
            objPLI = New PLIRecord
            With objPLI
                .cActiveDeal = strPLI_Message.Substring(PLI.ACTDEAL_OFFSET, PLI.ACTDEAL)
                .cGapFlags = strPLI_Message.Substring(PLI.GAPFLAG_OFFSET, PLI.GAPFLAG)
                .cMS_Flag = strPLI_Message.Substring(PLI.MSFLAG_OFFSET, PLI.MSFLAG)
                .cOSSR_Flag = strPLI_Message.Substring(PLI.OSSRITEM_OFFSET, PLI.OSSRITEM)
                .cStatus = strPLI_Message.Substring(PLI.STATUS_OFFSET, PLI.STATUS)
                .strBackShopQuantity = strPLI_Message.Substring(PLI.BKSHP_OFFSET, PLI.BKSHP)
                .strBarcode = strPLI_Message.Substring(PLI.BARCODE_OFFSET, PLI.BARCODE)
                .strBootsCode = strPLI_Message.Substring(PLI.BCDE_OFFSET, PLI.BCDE)
                .strDescription = strPLI_Message.Substring(PLI.DESC_OFFSET, PLI.DESC)
                '.strOperatorID = strPLI_Message.Substring(PLI.ID_OFFSET, PLI.ID)
                .strParentBootsCode = strPLI_Message.Substring(PLI.PARENT_OFFSET, PLI.PARENT)
                .strRequired = strPLI_Message.Substring(PLI.REQUIRED_OFFSET, PLI.REQUIRED)
                .strSEL_Desc = strPLI_Message.Substring(PLI.SELD_OFFSET, PLI.SELD)
                SequenceNo = strPLI_Message.Substring(PLI.SEQ_OFFSET, PLI.SEQ)
                .strSequence = SequenceNo
                .strShelfQuantity = strPLI_Message.Substring(PLI.QTYSHELF_OFFSET, PLI.QTYSHELF)
                .strStockFigure = strPLI_Message.Substring(PLI.STOCKFIG_OFFSET, PLI.STOCKFIG)

            End With
            m_TempDataPool.Add(objPLI)
            objPLI = Nothing
            objAppContainer.objExportDataManager.SendNextSequence((CInt(SequenceNo) + 1).ToString())
        Catch ex As Exception
            objPLI = Nothing
            m_TempDataPool.Clear()
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
            Return bTemp
        End Try
    End Function
    ''' <summary> 
    ''' Function to parse PLL record
    ''' </summary>
    ''' <param name="strPLL_Message">The String that has to be parsed</param>
    ''' <returns>Boolean - Which implies the Parsing status</returns>
    ''' <remarks></remarks>
    Public Function ParsePLL(ByVal strPLL_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim objPLL As New PLLRecord
        Dim SequenceNo As String
        Try
            With objPLL
                .cStatus = strPLL_Message.Substring(PLL.STATUS_OFFSET, PLL.STATUS)
                .DateTime = strPLL_Message.Substring(PLL.DATETIME_OFFSET, PLL.DATETIME)
                .strDisplayName = strPLL_Message.Substring(PLL.UNAME_OFFSET, PLL.UNAME).Trim()
                .strLines = strPLL_Message.Substring(PLL.LINES_OFFSET, PLL.LINES)
                .strListID = strPLL_Message.Substring(PLL.ID_OFFSET, PLL.ID)
                SequenceNo = strPLL_Message.Substring(PLL.SEQ_OFFSET, PLL.SEQ)
                .cStockLocation = strPLL_Message.Substring(PLL.LOC_OFFSET, PLL.LOC)
                .strSeqNumber = SequenceNo
                .strPickerID = strPLL_Message.Substring(PLL.PICKER_OFFSET, PLL.PICKER)
            End With
            m_TempDataPool.Add(objPLL)
            objPLL = Nothing
            objAppContainer.objExportDataManager.SendNextSequence((CInt(SequenceNo) + 1).ToString())
        Catch ex As Exception
            m_TempDataPool.Clear()
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        End Try
    End Function
    ''' <summary>
    ''' Parses PLE record 
    ''' </summary>
    ''' <param name="strPLE_Message">The Message that has to be parsed</param>
    ''' <returns>Boolean Which Tells the status of the parsing </returns>
    ''' <remarks></remarks>
    Public Function ParsePLE(ByVal strPLE_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Try
            For Each objRecord In m_TempDataPool
                DATAPOOL.getInstance.addObject(objRecord)
            Next
            m_TempDataPool.Clear()
            bTemp = True
        Catch ex As Exception
            m_TempDataPool.Clear()
            'Error handling
            DATAPOOL.getInstance.IsError = True
            DATAPOOL.getInstance.ErrorMessage = "Error in Parsing the DATA"
            bTemp = False
        Finally
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        End Try
        ParsePLE = bTemp
    End Function

#End Region

#Region "Print SEL"
    ''' <summary>
    ''' PArse LPR Message
    ''' </summary>
    ''' <param name="strLPR_Message">Message to be Parsed</param>
    ''' <param name="objLPR">The Reference to the LPR Record</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function ParseLPR(ByVal strLPR_Message As String, ByRef objLPR As LPRRecord) As Boolean
        Dim bTemp As Boolean = False
        Try
            With objLPR
                .charMS_Flag = Convert.ToChar(strLPR_Message.Substring(LPR.MSFLAG_OFFSET, LPR.MSFLAG))
                .cPHF_SEL_Type = Convert.ToChar(strLPR_Message.Substring(LPR.PHFTYPE_OFFSET, LPR.PHFTYPE))
                .cUnitPriceFlag = Convert.ToChar(strLPR_Message.Substring(LPR.UNITPRICEFLAG_OFFSET, LPR.UNITPRICEFLAG))
                .cWEEE_Flag = Convert.ToChar(strLPR_Message.Substring(LPR.WEEEFLAG_OFFSET, LPR.WEEEFLAG))
                .strUnitMeasure = strLPR_Message.Substring(LPR.UNITMEASURE_OFFSET, LPR.UNITMEASURE)
                .strUnitQuantity = strLPR_Message.Substring(LPR.UNITQTY_OFFSET, LPR.UNITQTY)
                .strUnittype = strLPR_Message.Substring(LPR.UNITTYPE_OFFSET, LPR.UNITTYPE)
                .strWEE_Price = strLPR_Message.Substring(LPR.WEEEPRFPRICE_OFFSET, LPR.WEEEPRFPRICE)
                .WASPrice1 = strLPR_Message.Substring(LPR.WASPRICE1_OFFSET, LPR.WASPRICE1)
                .WASPrice2 = strLPR_Message.Substring(LPR.WASPRICE2_OFFSET, LPR.WASPRICE2)
                .charMS_Flag = strLPR_Message.Substring(LPR.MSFLAG_OFFSET, LPR.MSFLAG)
                .strPainKillerMessage = strLPR_Message.Substring(LPR.PAINKILLERMSG_OFFSET)
                If .cWEEE_Flag.Trim() = "" Or .cWEEE_Flag.Trim = Nothing Then
                    .cWEEE_Flag = " "
                End If
                If .strWEE_Price.Trim = "" Or .strWEE_Price.Trim = Nothing Then
                    .strWEE_Price = "000000"
                End If
                bTemp = True
            End With
            DATAPOOL.getInstance.addObject(objLPR)
            bTemp = True
            objLPR = Nothing

        Catch ex As Exception
            bTemp = False
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        Finally
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        End Try
        Return bTemp
    End Function
#End Region

#Region "Price Check"
    ''' <summary>
    ''' Parses the PCR Message 
    ''' </summary>
    ''' <param name="strPCR_Message"></param>
    ''' <returns>True- if success
    ''' false if fails
    ''' </returns>
    ''' <remarks></remarks>
    Public Function ParsePCR(ByVal strPCR_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim objPCR As New PCRRecord
        Try
            With objPCR
                .strPriceChkDone = strPCR_Message.Substring(PCR.PRCHKDNE_OFFSET, PCR.PRCHKDNE)
                .strPriceChkTarget = strPCR_Message.Substring(PCR.PRCHKTGT_OFFSET, PCR.PRCHKTGT)
                bTemp = True
            End With
            DATAPOOL.getInstance.addObject(objPCR)
        Catch ex As Exception
            bTemp = False
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        Finally
            objPCR = Nothing
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        End Try
        Return bTemp
    End Function
#End Region

#Region "Count List"
    ''' <summary>
    ''' Parses CLI Message 
    ''' </summary>
    ''' <param name="strCLI_Message">The String to be parsed</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function ParseCLI(ByVal strCLI_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim objCLI As CLIRecord
        Dim strSeqNo As String = ""
        Dim strCount As String
        Dim iCount As Integer
        Dim CLIStructure As New ArrayList
        Dim strMoreItem As String
        Dim nextItemCount As Integer
        Dim arrCLIList As ArrayList


        Try
            objCLI = New CLIRecord

            strSeqNo = strCLI_Message.Substring(CLI.SEQID_OFFSET, CLI.SEQID)
            strCount = strCLI_Message.Substring(CLI.ITEMINLIST_OFFSET, CLI.ITEMINLIST)
            strMoreItem = strCLI_Message.Substring(CLI.MOREITEM_OFFSET, CLI.MOREITEM)
            If objAppContainer.bIsCreateOwnList Then
                strCount = 1
            End If
            With objCLI
                For iCount = 0 To Val(strCount) - 1
                    'objAppContainer.m_iCLIIndex = objAppContainer.m_iCLIIndex + 1
                    nextItemCount = 0
                    nextItemCount = (iCount * CLI.NEXTITEM)
                    .strSequence = strCLI_Message.Substring(CLI.SEQ_OFFSET + nextItemCount, CLI.SEQ)
                    '.strSequence = objAppContainer.m_iCLIIndex
                    .strBootsCode = strCLI_Message.Substring(CLI.BCDE_OFFSET + nextItemCount, CLI.BCDE)
                    .strParentBootsCode = strCLI_Message.Substring(CLI.PARENT_OFFSET + nextItemCount, CLI.PARENT)
                    .strBarcode = strCLI_Message.Substring(CLI.BARCODE_OFFSET + nextItemCount, CLI.BARCODE)
                    .strSEL_Desc = strCLI_Message.Substring(CLI.SELD_OFFSET + nextItemCount, CLI.SELD)
                    .cActiveDeal = strCLI_Message.Substring(CLI.ACTDEAL_OFFSET + nextItemCount, CLI.ACTDEAL)
                    .strProdGroup = strCLI_Message.Substring(CLI.PRODGP_OFFSET + nextItemCount, CLI.PRODGP)
                    .iBSMBS_Count = strCLI_Message.Substring(CLI.BSCNT_OFFSET + nextItemCount, CLI.BSCNT)
                    .iBSPSP_Count = strCLI_Message.Substring(CLI.BSPSPCNT_OFFSET + nextItemCount, CLI.BSPSPCNT)
                    .iSF_Count = strCLI_Message.Substring(CLI.SFCNT_OFFSET + nextItemCount, CLI.SFCNT)
                    .iOSSRMBS_Count = strCLI_Message.Substring(CLI.OSSRBSCNT_OFFSET + nextItemCount, CLI.OSSRBSCNT)
                    .iOSSRPSP_Count = strCLI_Message.Substring(CLI.OSSRPSPCNT_OFFSET + nextItemCount, CLI.OSSRPSPCNT)
                    .cStatus = strCLI_Message.Substring(CLI.STATUS_OFFSET + nextItemCount, CLI.STATUS)
                    .cOSSR_Flag = strCLI_Message.Substring(CLI.OSSRITEM_OFFSET + nextItemCount, CLI.OSSRITEM)
                    .strDateLastCount = strCLI_Message.Substring(CLI.LSTCNTDATE_OFFSET + nextItemCount, CLI.LSTCNTDATE)
                    .strPendSale_Flag = strCLI_Message.Substring(CLI.PENDSALE_OFFSET + nextItemCount, CLI.PENDSALE)
                    .strStockFigure = strCLI_Message.Substring(CLI.STOCKFIG_OFFSET + nextItemCount, CLI.STOCKFIG)
                    CLIStructure.Add(objCLI)   '132
                Next
            End With
            m_TempDataPool.Add(CLIStructure)

            If strMoreItem.Equals("Y") Then
                'objAppContainer.m_iCLIIndex = Convert.ToInt16(strSeqNo) - 1
                objAppContainer.objExportDataManager.SendNextSequence(strSeqNo.ToString())
            Else
                arrCLIList = New ArrayList()
                For Each objRecord In m_TempDataPool
                    For Each objItem In objRecord
                        arrCLIList.Add(objItem)
                    Next
                Next
                DATAPOOL.getInstance.addObject(arrCLIList)
                DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
                m_TempDataPool.Clear()
                bTemp = True
            End If         'objCLI = Nothing
            'objAppContainer.objExportDataManager.SendNextSequence((CInt(SequenceNo) + 1).ToString())
        Catch ex As Exception
            objCLI = Nothing
            bTemp = False
            'm_TempDataPool.Clear()
            'DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        Finally
            objCLI = Nothing
        End Try
        Return bTemp
    End Function

    Public Function ParseCLL(ByVal strCLL_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim objCLL As New CLLRecord
        Dim SequenceNo As String
        Try
            With objCLL
                .strReturnedListID = strCLL_Message.Substring(CLL.RETLISTID_OFFSET, CLL.RETLISTID)
                .strOSSR_Item = strCLL_Message.Substring(CLL.OSSRITEMS_OFFSET, CLL.OSSRITEMS)
                .cActiveFlag = strCLL_Message.Substring(CLL.ACTIVE_OFFSET, CLL.ACTIVE)
                .cListType = strCLL_Message.Substring(CLL.LSTYPE_OFFSET, CLL.LSTYPE)
                .strBusinessGroup = strCLL_Message.Substring(CLL.BUNAME_OFFSET, CLL.BUNAME)
                .strNo_BS_Items = strCLL_Message.Substring(CLL.BSITEMS_OFFSET, CLL.BSITEMS)
                .strNo_SF_Items = strCLL_Message.Substring(CLL.SFITEMS_OFFSET, CLL.SFITEMS)
                SequenceNo = strCLL_Message.Substring(CLL.SEQ_OFFSET, CLL.SEQ)
                .strSeqNumber = SequenceNo
                'SFA DEF#823 - To display the outstanding total item
                '.strTotalItems = strCLL_Message.Substring(CLL.TOTITEMS_OFFSET, CLL.TOTITEMS)
                .strTotalItems = strCLL_Message.Substring(CLL.SFITEMS_OFFSET, CLL.SFITEMS)
                .strLastCntDate = strCLL_Message.Substring(CLL.LSTCNTDT_OFFSET, CLL.LSTCNTDT)   'Added as per SFA
                .strCounterID = strCLL_Message.Substring(CLL.COUNTERID_OFFSET, CLL.COUNTERID)   'Added as per SFA
            End With
            m_TempDataPool.Add(objCLL)
            objCLL = Nothing
            objAppContainer.objExportDataManager.SendNextSequence((CInt(SequenceNo) + 1).ToString())
        Catch ex As Exception
            m_TempDataPool.Clear()
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        End Try
    End Function

    ''' <summary>
    ''' Parse the CLE Message
    ''' </summary>
    ''' <param name="strCLE_Message">The CLE message that has to be parsed</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function ParseCLE(ByVal strCLE_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Try
            ' Include Code to parse the CLE
            'For Each objCLL In m_TempDataPool
            '    DATAPOOL.getInstance.addObject(objCLL)
            'Next
            For Each objRecord In m_TempDataPool
                DATAPOOL.getInstance.addObject(objRecord)
            Next

            m_TempDataPool.Clear()
            bTemp = True
        Catch ex As Exception
            m_TempDataPool.Clear()
            'Error handling
            DATAPOOL.getInstance.IsError = True
            DATAPOOL.getInstance.ErrorMessage = "Error in Parsing the DATA"
            bTemp = False
        Finally
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        End Try
        ParseCLE = bTemp
    End Function
#Region "Added as part of SFA"
    ''' <summary>
    ''' Parses CLB Message 
    ''' </summary>
    ''' <param name="strCLB_Message">The String to be parsed</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function ParseCLB(ByVal strCLB_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim objCLB As New CLBRecord
        Try
            'objCLB = New CLBRecord
            With objCLB
                .strListID = strCLB_Message.Substring(CLB.LISTID_OFFSET, CLB.LISTID)
                bTemp = True
            End With
            DATAPOOL.getInstance.addObject(objCLB)
        Catch ex As Exception
            m_TempDataPool.Clear()
        Finally
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
            objCLB = Nothing
        End Try
    End Function
#End Region
#End Region

#Region "Sales"
    ''' <summary>
    ''' Function Parses the SSR message
    ''' </summary>
    ''' <param name="strSSR_Message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseSSR(ByVal strSSR_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim objSSR As New SSRRecord
        Try
            With objSSR
                objSSR.strTodaysSales = strSSR_Message.Substring(SSR.SVT_OFFSET, SSR.SVT)
                objSSR.strWeeklySales = strSSR_Message.Substring(SSR.SVPREV_OFFSET, SSR.SVPREV)

            End With
            DATAPOOL.getInstance.addObject(objSSR)
            bTemp = True
        Catch ex As Exception
            bTemp = False
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        Finally
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
            objSSR = Nothing
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' Function to parse the dela response message.
    ''' </summary>
    ''' <param name="strDQR_Message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseDQR(ByVal strDQR_Message As String) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered Parse DQR Function, Message:" + strDQR_Message, _
                                              Logger.LogLevel.INFO)
        Dim bTemp As Boolean = False
        Dim objDQR As New DQRRECORD
        Try
            With objDQR
                .strDealDesc = strDQR_Message.Substring(DQR.DEAL_DESC_OFFSET)
                .strDealNumber = strDQR_Message.Substring(DQR.DEAL_NUMBER_OFFSET, DQR.DEAL_NUMBER)
                .strEndDate = strDQR_Message.Substring(DQR.END_DATE_OFFSET, DQR.END_DATE)
                .strStartDate = strDQR_Message.Substring(DQR.START_DATE_OFFSET, DQR.START_DATE)
                bTemp = True
            End With
            DATAPOOL.getInstance.addObject(objDQR)
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        Catch ex As Exception
            '     log    the    Error   here   '
            objAppContainer.objLogger.WriteAppLog("Error Occured, Message:" + ex.Message, Logger.LogLevel.INFO)
        Finally
            objDQR = Nothing
        End Try
        objAppContainer.objLogger.WriteAppLog("Exited Parse DQR Function", Logger.LogLevel.INFO)
        Return bTemp
    End Function
    ''' <summary>
    ''' Function to parse item sales response message.
    ''' </summary>
    ''' <param name="strISR_Message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseISR(ByVal strISR_Message As String) As Boolean
        Dim bTemp As Boolean
        Dim objISR As New ISRRecord
        Try
            With objISR
                .strItemDescription = strISR_Message.Substring(ISR.DESC_OFFSET, ISR.DESC)
                .strItemSoldTody = strISR_Message.Substring(ISR.CQTY_OFFSET, ISR.CQTY)
                .strValueItemSoldToday = strISR_Message.Substring(ISR.CVAL_OFFSET, ISR.CVAL)
                .strItemSoldWeek = strISR_Message.Substring(ISR.PQTY_OFFSET, ISR.PQTY)
                .strValueItemSoldWeek = strISR_Message.Substring(ISR.PVAL_OFFSET, ISR.PVAL)
            End With
            DATAPOOL.getInstance.addObject(objISR)
            bTemp = True
        Catch ex As Exception
            bTemp = False
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        Finally
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
            objISR = Nothing
        End Try
        Return bTemp
    End Function
    Public Function ParseRLF(ByVal strRLF_Message As String) As Boolean
        Dim bTemp As Boolean
        Try
            DATAPOOL.getInstance.addObject(strRLF_Message)
            bTemp = True
        Catch ex As Exception
            bTemp = False
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        Finally
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        End Try
        Return bTemp
    End Function
#End Region

#Region "Reports Section"
    ''' <summary>
    ''' Parse the RUP Message
    ''' </summary>
    ''' <param name="strRUP_Message">The Message to be Parsed</param>
    ''' <param name="objRUP">Reference to the Object </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseRUP(ByVal strRUP_Message As String, ByRef objRUP As RUPRecord) As Boolean
        Dim bTemp As Boolean = False
        Dim iCount As Integer = 0
        Try
            With objRUP
                .strCount = strRUP_Message.Substring(RUP.COUNT_OFFSET, RUP.COUNT)
                If (.arrRUPInnerStucture Is Nothing) Then
                    .arrRUPInnerStucture = New ArrayList()
                End If
                Dim objRUPInnerRecord As New RUPInnerStructure
                For iCount = 0 To Val(.strCount - 1)
                    objRUPInnerRecord.cFunction = strRUP_Message.Substring((RUP.FUNC_OFFSET + (iCount * RUP.TRAILERTOTAL)), RUP.FUNC)
                    objRUPInnerRecord.cLevel = strRUP_Message.Substring((RUP.LEVEL_OFFSET + (iCount * RUP.TRAILERTOTAL)), RUP.LEVEL)
                    objRUPInnerRecord.strData = strRUP_Message.Substring((RUP.DATA_OFFSET + (iCount * RUP.TRAILERTOTAL)), RUP.DATA)
                    .arrRUPInnerStucture.Add(objRUPInnerRecord)
                Next
            End With
            bTemp = True
            DATAPOOL.getInstance().addObject(objRUP)
        Catch ex As Exception
            bTemp = False
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        Finally
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
            objRUP = Nothing
        End Try
        Return bTemp
    End Function

    ''' <summary>
    ''' Parses the RLD Record
    ''' </summary>
    ''' <param name="strRLD_Message">The RLD Message</param>
    ''' <param name="objRLD">The Reference to the RLD Record</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseRLD(ByVal strRLD_Message As String, ByRef objRLD As RLDRecord) As Boolean
        Dim bTemp As Boolean = False
        Dim iCount As Integer = 0
        Try
            With objRLD
                .strCount = strRLD_Message.Substring(RLD.COUNT_OFFSET, RLD.COUNT)
                If .arrRLDInnerStructure Is Nothing Then
                    .arrRLDInnerStructure = New ArrayList()
                End If
                Dim objRLDInnerRecord As RLDInnerStructure
                For iCount = 0 To Val(.strCount) - 1
                    objRLDInnerRecord.strSequence = strRLD_Message.Substring(RLD.SEQ_OFFSET + (iCount * 24), RLD.SEQ)
                    objRLDInnerRecord.strData = strRLD_Message.Substring(RLD.DATA_OFFSET + (iCount * 24), RLD.DATA)
                    .arrRLDInnerStructure.Add(objRLDInnerRecord)
                Next
            End With
            bTemp = True
            DATAPOOL.getInstance().addObject(objRLD)
        Catch ex As Exception
            bTemp = False
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        Finally
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
            objRLD = Nothing
        End Try
        Return bTemp
    End Function

    Public Function ParseRLR(ByVal strRLR_Message As String, ByRef objRLR As RLRRecord) As Boolean
        Dim bTemp As Boolean = False
        Try
            With objRLR
                .strSequence = strRLR_Message.Substring(RLR.SEQ_OFFSET, RLR.SEQ)
                .strTitle = strRLR_Message.Substring(RLR.TITLE_OFFSET, RLR.TITLE)
                .strReportId = strRLR_Message.Substring(RLR.REPORTID_OFFSET, RLR.REPORTID)
            End With
            bTemp = True
            DATAPOOL.getInstance().addObject(objRLR)
        Catch ex As Exception
            bTemp = False
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        Finally
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
            objRLR = Nothing
        End Try
        Return bTemp
    End Function
#End Region

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="PGG_Message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParsePGG(ByVal PGG_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim iCount As Integer = 0
        Dim objPGG As PGGRecord
        Dim strSeqNo As String = "0000"
        Try
            '' Include Code to parse the PGG

            For iCount = 0 To PGG.MAX_COUNT - 1
                objPGG = New PGGRecord
                With objPGG
                    strSeqNo = PGG_Message.Substring( _
                      (PGG.HEADERTOTAL + PGG.SEQ_OFFSET + (iCount * PGG.TRAILERTOTAL)), PGG.SEQ)
                    .strSequence = strSeqNo
                    .strDescription = PGG_Message.Substring( _
                      (PGG.HEADERTOTAL + PGG.DESC_OFFSET + (iCount * PGG.TRAILERTOTAL)), PGG.DESC)
                    .strPOG_StartPtr = PGG_Message.Substring( _
                      (PGG.HEADERTOTAL + PGG.STRTPTR_OFFSET + (iCount * PGG.TRAILERTOTAL)), PGG.STRTPTR)
                    .strFamilyType = PGG_Message.Substring( _
                      (PGG.HEADERTOTAL + PGG.FAMTYPE_OFFSET + (iCount * PGG.TRAILERTOTAL)), PGG.FAMTYPE)
                    .strHierachy = PGG_Message.Substring( _
                      (PGG.HEADERTOTAL + PGG.HIREARCHY_OFFSET + (iCount * PGG.TRAILERTOTAL)), PGG.HIREARCHY)
                End With
                If objPGG.strSequence.Equals("FFFF") Then
                    objPGG = Nothing
                    Exit For
                Else
                    m_TempDataPool.Add(objPGG)
                End If
                objPGG = Nothing
            Next
            If PGG_Message.EndsWith("Y") Then
                bTemp = objAppContainer.objExportDataManager.SendNextSequence((Convert.ToInt16(strSeqNo) + 1).ToString())
            Else
                For Each objPGG_Record In m_TempDataPool
                    DATAPOOL.getInstance.addObject(objPGG_Record)
                Next
                m_TempDataPool.Clear()
                DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
                bTemp = True
            End If
        Catch ex As Exception
            bTemp = False
            m_TempDataPool.Clear()
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        End Try
        Return bTemp
    End Function
    Public Function ParsePGN(ByVal strPGN_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim iCount As Integer = 0
        Dim objPGN As PGNRecord
        Dim strSeqNo As String = "0000"
        Try
            '' Include Code to parse the PGR
            For iCount = 0 To PGR.COUNT_MAX - 1
                objPGN = New PGNRecord()
                With objPGN
                    strSeqNo = strPGN_Message.Substring( _
                     (PGN.HEADERTOTAL + PGN.MODULESEQ_OFFSET + (iCount * PGN.TRAILERTOTAL)), PGN.MODULESEQ)
                    .strMODSeq = strSeqNo
                    .strMODDesc = strPGN_Message.Substring((PGN.HEADERTOTAL + _
                                PGN.MOD_DESC_OFFSET + (iCount * PGN.TRAILERTOTAL)), PGN.MOD_DESC)
                    .strShelfCount = strPGN_Message.Substring((PGN.HEADERTOTAL + _
                                PGN.SHELF_COUNT_OFFSET + (iCount * PGN.TRAILERTOTAL)), PGN.SHELF_COUNT)
                    .cFilter = strPGN_Message.Substring((PGN.HEADERTOTAL + _
                                PGN.FILTER_OFFSET + (iCount * PGN.TRAILERTOTAL)), PGN.FILTER)

                End With
                If strSeqNo.Trim("F") = "" Then
                    objPGN = Nothing
                    Exit For
                Else
                    m_TempDataPool.Add(objPGN)
                    objPGN = Nothing
                End If
            Next
            If strPGN_Message.EndsWith("Y") Then
                Dim iNextSequence As Integer = (Convert.ToInt16(strSeqNo) + 1)
                'The maximum sequence Number can be 255
                'Check for that 
                If iNextSequence <= 255 Then
                    bTemp = objAppContainer.objExportDataManager.SendNextSequence(iNextSequence.ToString())
                Else
                    For Each objPGRRecord In m_TempDataPool
                        DATAPOOL.getInstance.addObject(objPGRRecord)
                    Next
                    m_TempDataPool.Clear()
                    DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
                End If
            Else
                For Each objPGRRecord In m_TempDataPool
                    DATAPOOL.getInstance.addObject(objPGRRecord)
                Next
                m_TempDataPool.Clear()
                DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
            End If
            bTemp = True
        Catch ex As Exception
            bTemp = False
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        End Try
        Return bTemp
    End Function

    Public Function ParsePSR(ByVal strPSR_Message As String, ByVal strSendSequence As String) As Boolean
        Dim bTemp As Boolean = False
        Dim iCount As Integer = 0
        Dim objPSRRecord As PSRRecord
        Dim strNextSequence As String
        Dim strModuleID As String
        Dim strShelfNumber As String
        Dim strShelfDesc As String
        Dim strNotchNumber As String
        Dim strSequenceSend As String
        Dim iShelfNo As Integer
        Dim iNotchNo As Integer
        Dim Count As Integer = 0
        Try


            strSequenceSend = strSendSequence.Substring(5, strSendSequence.Length - 5)
            strModuleID = strSequenceSend.Substring(PSL.MODULE_ID_OFFSET, PSL.MODULE_ID)
            'DEFECT FIX - BTCPR00004153(RF Mode :: Planner :: Notch number missing in the line list screen in planner module.)
            iShelfNo = Convert.ToInt32(strModuleID)
            strModuleID = iShelfNo.ToString()
            strShelfNumber = strSequenceSend.Substring(PSL.SHELF_NUMBER_OFFSET, PSL.SHELF_NUMBER)
            iShelfNo = Convert.ToInt32(strShelfNumber)
            strShelfNumber = iShelfNo.ToString()
            'strModuleID = strSendSequence.Substring(PSL.MODULE_ID_OFFSET, PSL.MODULE_ID)
            'strShelfNumber = strSendSequence.Substring(PSL.SHELF_NUMBER_OFFSET, PSL.SHELF_NUMBER)
            strShelfDesc = strPSR_Message.Substring(PSR.SHELF_DESC_OFFSET, PSR.SHELF_DESC)
            strNotchNumber = strPSR_Message.Substring(PSR.NOTCH_NO_OFFSET, PSR.NOTCH_NO)
            iNotchNo = Convert.ToInt32(strNotchNumber)
            strNotchNumber = iNotchNo.ToString()

            For iCount = 0 To PSR.COUNT_MAX - 1
                objPSRRecord = New PSRRecord
                With objPSRRecord
                    .strBootsCode = strPSR_Message.Substring((PSR.HEADERTOTAL + _
                        (PSR.TRAILERTOTAL * iCount) + PSR.BOOTSCODE_OFFSET), PSR.BOOTSCODE)
                    .strFacings = strPSR_Message.Substring((PSR.HEADERTOTAL + _
                       (PSR.TRAILERTOTAL * iCount) + PSR.FACINGS_OFFSET), PSR.FACINGS)
                    .strItemDesc = strPSR_Message.Substring((PSR.HEADERTOTAL + _
                       (PSR.TRAILERTOTAL * iCount) + PSR.ITEM_DESC_OFFSET), PSR.ITEM_DESC)

                    If objPSRRecord.strBootsCode.Trim("F") <> "" Then
                        .strModuleID = strModuleID
                        .strShelfNumber = strShelfNumber
                        .strShelfDesc = strShelfDesc
                        .strNotchNo = strNotchNumber
                        m_TempDataPool.Add(objPSRRecord)
                        objPSRRecord = Nothing
                    Else
                        objPSRRecord = Nothing
                        Exit For
                    End If
                End With
            Next

            If strPSR_Message.Substring(PSR.NEXT_SHELF_OFFSET, PSR.NEXT_SHELF) = "FFF" And _
                   strPSR_Message.Substring(PSR.NEXT_CHAIN_OFFSET, PSR.NEXT_CHAIN) = "FF" And _
                   strPSR_Message.Substring(PSR.NEXT_ITEM_OFFSET, PSR.NEXT_ITEM) = "FF" Then
                For Each objRecord In m_TempDataPool
                    DATAPOOL.getInstance.addObject(objRecord)
                Next
                m_TempDataPool.Clear()
                objPSRRecord = Nothing
                DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
            Else
                strNextSequence = ""
                objPSRRecord = Nothing
                strNextSequence = strPSR_Message.Substring(PSR.NEXT_SHELF_OFFSET, PSR.NEXT_SHELF + 4)
                objAppContainer.objExportDataManager.SendNextSequence(strNextSequence)
            End If

        Catch ex As Exception
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        End Try
        Return bTemp
    End Function


    Public Function ParsePGR(ByVal PGR_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim iCount As Integer = 0
        Dim objPGR As PGRRecord
        Dim strNextSequence As String
        Try
            '' Include Code to parse the PGR

            For iCount = 0 To PGR.COUNT_MAX - 1
                objPGR = New PGRRecord()
                With objPGR
                    .strPOG_IndexPtr = PGR_Message.Substring((PGR.HEADERTOTAL + PGR.POGIPTR_OFFSET + _
                        (iCount * PGR.TRAILERTOTAL)), PGR.POGIPTR)
                    .strPOGDesc = PGR_Message.Substring((PGR.HEADERTOTAL + PGR.POGDESC_OFFSET + _
                       (iCount * PGR.TRAILERTOTAL)), PGR.POGDESC)
                    .strActDate = PGR_Message.Substring((PGR.HEADERTOTAL + PGR.ACTDATE_OFFSET + _
                        (iCount * PGR.TRAILERTOTAL)), PGR.ACTDATE)
                    .strDeacDate = PGR_Message.Substring((PGR.HEADERTOTAL + PGR.DACTDATE_OFFSET + _
                        (iCount * PGR.TRAILERTOTAL)), PGR.DACTDATE)
                    .strMODCount = PGR_Message.Substring((PGR.HEADERTOTAL + PGR.MODECNT_OFFSET + _
                        (iCount * PGR.TRAILERTOTAL)), PGR.MODECNT)
                End With
                If objPGR.strPOG_IndexPtr.Trim("F") = "" Then
                    objPGR = Nothing
                    Exit For
                Else
                    m_TempDataPool.Add(objPGR)
                    objPGR = Nothing
                End If
            Next
            strNextSequence = PGR_Message.Substring(PGR.NXTPOGIREC_OFFSET)
            If strNextSequence.Trim("F") <> "" Then
                bTemp = objAppContainer.objExportDataManager.SendNextSequence(strNextSequence)
            Else
                For Each objPGRRecord In m_TempDataPool
                    DATAPOOL.getInstance.addObject(objPGRRecord)
                Next
                m_TempDataPool.Clear()
                DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
            End If
            bTemp = True
        Catch ex As Exception
            bTemp = False
            m_TempDataPool.Clear()
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        Finally

        End Try
        Return bTemp
    End Function

    Public Function ParsePGN(ByVal PGN_Message As String, ByRef objPGN As PGNRecord) As Boolean
        Dim bTemp As Boolean = False
        Try
            '' Include Code to parse the PGN
        Catch ex As Exception
            bTemp = False
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        End Try
        Return bTemp
    End Function

    Public Function ParsePGI(ByVal strPGI_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim iCounter As Integer
        Dim strSequence As String
        Try
            Dim objPGI As PGIRecord
            For iCounter = 0 To PGI.MAX_COUNT - 1
                objPGI = New PGIRecord
                With objPGI
                    .strMODCNT = strPGI_Message.Substring((PGI.HEADERTOTAL + PGI.MODULE_COUNT_OFFSET + _
                                (iCounter * PGI.TRAILERTOTAL)), PGI.MODULE_COUNT)
                    .strPOGDESC = strPGI_Message.Substring((PGI.HEADERTOTAL + PGI.POGDESC_OFFSET + _
                         (iCounter * PGI.TRAILERTOTAL)), PGI.POGDESC)
                    .strPOGKEY = strPGI_Message.Substring((PGI.HEADERTOTAL + PGI.POGKEY_OFFSET + _
                     (iCounter * PGI.TRAILERTOTAL)), PGI.POGKEY)
                End With
                If objPGI.strPOGKEY.Trim("F") <> "" Then
                    m_TempDataPool.Add(objPGI)
                    objPGI = Nothing
                Else
                    objPGI = Nothing
                    Exit For
                End If
            Next

            strSequence = strPGI_Message.Substring(PGI.NEXTSEQ_OFFSET, PGI.NEXTSEQ)
            If strSequence.Trim("F") = "" Then
                For Each objItem In m_TempDataPool
                    DATAPOOL.getInstance.addObject(objItem)
                Next
                m_TempDataPool.Clear()
                DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
                bTemp = True
            Else
                Return objAppContainer.objExportDataManager.SendNextSequence(strSequence)
            End If
        Catch ex As Exception
            bTemp = False
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        End Try
        Return bTemp
    End Function
    'anoop:Start
    Public Function ParsePGB(ByVal PGB_Message As String, ByRef objPGB As PGBRecord) As Boolean
        Dim bTemp As Boolean = False
        Dim iCount As Integer = 0
        Dim strNextChain As String
        Dim strNextModuleKeyOffset As String
        Dim strNextItemField As String
        Dim strNextReuestString As New System.Text.StringBuilder()

        Try
            '' Include Code to parse the PGR
            With objPGB
                For iCount = 0 To PGB.COUNT_MAX - 1
                    .strPOGKey = PGB_Message.Substring(( _
                       PGB.POGKEY_OFFSET + iCount * PGB.TRAILERTOTAL), PGB.POGKEY)
                    .strModuleCount = PGB_Message.Substring((PGB.MODCOUNT_OFFSET + _
                       iCount * PGB.TRAILERTOTAL), PGB.MODCOUNT)
                    .strRepeatCount = PGB_Message.Substring((PGB.REPEATCOUNT_OFFSET + _
                        iCount * PGB.TRAILERTOTAL), PGB.RPTCOUNT)
                    .strPOGDesc = PGB_Message.Substring((PGB.POGDESC_OFFSET + _
                        iCount * PGB.TRAILERTOTAL), PGB.POGDESC)
                    .strModuleDesc = PGB_Message.Substring((PGB.MODDESC_OFFSET + _
                        iCount * PGB.TRAILERTOTAL), PGB.MODEDESC)
                    .strMDQ = PGB_Message.Substring((PGB.MDQ_OFFSET + _
                        iCount * PGB.TRAILERTOTAL), PGB.MDQ)
                    .strPSC = PGB_Message.Substring((PGB.PSC_OFFSET + _
                        iCount * PGB.TRAILERTOTAL), PGB.PSC)
                    'Donot add to the array if the response is coming as "FF..."
                    If Not (objPGB.strPOGKey.StartsWith("FFF")) Then
                        m_TempDataPool.Add(objPGB)
                    End If
                    ''''''''''''''''''''''''''''''''''''''''''To be done
                    ''Logic for adding this record to the array list has to be completed
                Next
                strNextChain = PGB_Message.Substring(PGB.NEXTCHAIN_OFFSET, PGB.NEXTCHAIN)
                If strNextChain <> "FFF" Then
                    strNextModuleKeyOffset = PGB_Message.Substring(PGB.NEXTMOD_OFFSET, PGB.NEXTMOD)
                    strNextItemField = PGB_Message.Substring(PGB.NEXTITEM_OFFSET, PGB.NEXTITEM)
                    strNextReuestString.Append(strNextChain)
                    strNextReuestString.Append(strNextModuleKeyOffset)
                    strNextReuestString.Append(strNextItemField)
                    objAppContainer.objExportDataManager.SendNextSequence(strNextReuestString.ToString())
                Else
                    'Only when completed the temp data pool has to be cleared and added to the data pool
                    'TODO:Add check for repeats.

                    For Each objPGBRECORD As PGBRecord In m_TempDataPool
                        DATAPOOL.getInstance.addObject(objPGBRECORD)
                    Next
                    m_TempDataPool.Clear()
                    DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
                End If

                'If objPGB.strNextChain = "FFF" Then
                'Else

                'End If
                bTemp = True
            End With
        Catch ex As Exception
            'm_TempDataPool.Clear()
            bTemp = False
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        Finally
            objPGB = Nothing
        End Try
        Return bTemp
    End Function
End Class
#End If
