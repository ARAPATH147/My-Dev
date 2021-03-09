'Imports MCShMon.Message
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
    Private iSequenceNumber As Integer
    Private isFirstParse As Boolean = True
    Private m_TEMPDATAPOOL As ArrayList
    Private objRCFRecord As RCFRecord
    Public Sub New()
        isFirstParse = True
        iSequenceNumber = 0
        m_TEMPDATAPOOL = New ArrayList()
    End Sub
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
                    .strAckMessage = "ACK recieved without a message"
                End If
            End With
            '' Include Code for ACK PArsing
            DATAPOOL.getInstance.addObject(objACK)
            bTemp = True
            objACK = Nothing
        Catch ex As Exception
            bTemp = False
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        End Try
        DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
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
        Dim icount As Integer = 1
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
                '.arrDealSum = New ArrayList()
                'While ((icount <= EQR.MAX_Count_DEALSUM) And (Not ((strEQR_Message.Substring((icount * EQR.DEALSUM + EQR.DEALSUM_OFFSET), EQR.DEALSUM)).Trim("0") = "")))
                '    .arrDealSum.Add(strEQR_Message.Substring((icount * EQR.DEALSUM + EQR.DEALSUM_OFFSET), EQR.DEALSUM))
                '    icount = icount + 1
                'End While
                .strCorePlannerLoc = strEQR_Message.Substring(EQR.PLANLOC1_OFFSET, EQR.PLANLOC1)
                .strSalesPlanerLoc = strEQR_Message.Substring(EQR.PLANLOC2_OFFSET, EQR.PLANLOC2)
                .cRecallItem = strEQR_Message.Substring(EQR.RECALLITEM_OFFSET, EQR.RECALLITEM)
                .cMarkDown = strEQR_Message.Substring(EQR.MARKDOWN_OFFSET, EQR.MARKDOWN)
                'Recall CR
                .strRecallType = strEQR_Message.Substring(EQR.RECALLTYPE_OFFSET, EQR.RECALLTYPE)
                'AFF PL CR
                .strProductGp = strEQR_Message.Substring(EQR.PGGROUP_OFFSET, EQR.PGGROUP)
            End With

            DATAPOOL.getInstance.addObject(objEQR)
            objEQR = Nothing
            'since This is a Single Response Notify the data engine
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

    ''' <summary>
    ''' Parses NAK Message
    ''' </summary>
    ''' <param name="strNAK_Message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseNAK(ByVal strNAK_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim objNAK As New NAKRecord
        Try
            With objNAK
                .isNAKERROR = False
                .strErrorMessage = strNAK_Message.Substring(NAK.MESSAGE_OFFSET)
            End With
            bTemp = True
            DATAPOOL.getInstance.addObject(objNAK)
            objNAK = Nothing
            '' Include Code to parse the NAK
            If Not m_TEMPDATAPOOL Is Nothing Then
                m_TEMPDATAPOOL.Clear()
            End If
        Catch ex As Exception
            bTemp = False
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                ex.Message.ToString(), _
                                Logger.LogLevel.RELEASE)
        End Try
        DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        Return bTemp
    End Function
    Public Function ParseNAKERROR(ByVal strNAK_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim objNAK As New NAKERRORRecord
        Try
            With objNAK
                .isNAKERROR = True
                .strErrorMessage = strNAK_Message.Substring(NAKERROR.MESSAGE_OFFSET)
            End With
            bTemp = True
            If objNAK.strErrorMessage.Trim(" ") <> "" Then
                'assuming NAK error will always Get a message - confirmed with govindh.
                MessageBox.Show(objNAK.strErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                'Else
                '    MessageBox.Show(objNAK.strErrorMessage, "NAK-ERROR", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If
            DATAPOOL.getInstance.addObject(objNAK)
            objNAK = Nothing
            '' Include Code to parse the NAK
        Catch ex As Exception
            bTemp = False
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
                .strAccessStock = strSNR_Message.Substring(SNR.STKACCESS_OFFSET, SNR.STKACCESS)
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
#Region "Goods Out"
    Public Function ParseUOR(ByVal strUOR_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim objUOR As New UORRecord
        Try
            objUOR.strBusinessCentre = strUOR_Message.Substring(UOR.VALID_BC_OFFSET, UOR.VALID_BC)
            objUOR.strListNumber = strUOR_Message.Substring(UOR.LIST_ID_OFFSET, UOR.List_ID)
            objUOR.strOperatorId = strUOR_Message.Substring(UOR.OP_ID_OFFSET, UOR.OP_ID)
            DATAPOOL.getInstance.addObject(objUOR)
            objUOR = Nothing
            bTemp = True
        Catch ex As Exception
            bTemp = False
            objAppContainer.objLogger.WriteAppLog("PARSE UOR ERROR:: " + ex.Message, Logger.LogLevel.INFO)
        End Try
        DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        Return bTemp
    End Function
#End Region
#Region "Supplier List Parse"
    Public Function ParseDSR(ByVal strDSR_Message As String)
        Dim bTemp As Boolean = False
        Dim objDSRRecord As New DSRRecord
        Dim strSeqNumber As String
        Try
            With objDSRRecord
                .cBusinessCentre = strDSR_Message.Substring(DSR.BUSINESS_CENTRE_OFFSET, DSR.BUSINESS_CENTRE)
                strSeqNumber = strDSR_Message.Substring(DSR.SEQUENCE_NUMBER_OFFSET, DSR.SEQUENCE_NUMBER)
                .strSequenceNo = strSeqNumber
                .strSupplierName = strDSR_Message.Substring(DSR.SUPP_NAME_OFFSET, DSR.SUPP_NAME)
                .strSupplierNo = strDSR_Message.Substring(DSR.SUPPLIER_NUMBER_OFFSET, DSR.SUPPLIER_NUMBER)
            End With
            m_TEMPDATAPOOL.Add(objDSRRecord)
            objDSRRecord = Nothing
            objAppContainer.objExportDataManager.SendNextSequence((Val(strSeqNumber) + 1).ToString())
            bTemp = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("PARSE DSR:: Exception " + ex.Message(), Logger.LogLevel.RELEASE)
            m_TEMPDATAPOOL.Clear()
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        End Try
        Return bTemp
    End Function

    Public Function ParseDSE() As Boolean
        Dim bTemp As Boolean = False
        Try
            For Each objDSR In m_TEMPDATAPOOL
                DATAPOOL.getInstance.addObject(objDSR)
            Next
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("PARSE DSE:: Exception " + ex.Message(), Logger.LogLevel.RELEASE)
        Finally
            m_TEMPDATAPOOL.Clear()
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        End Try
    End Function
#End Region
#Region "Recalls"
    ''' <summary>
    ''' Parse the special instruction reponse message.
    ''' </summary>
    ''' <param name="strRCJ_Message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseRCJ(ByVal strRCJ_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim objRCJ As New RCJRecord
        Try
            With objRCJ
                .strRecallRefNo = strRCJ_Message.Substring(RCJ.RECALL_REF_OFFSET, RCJ.RECALL_REF)
                .strSpecialInst = strRCJ_Message.Substring(RCJ.SPECIAL_INST_OFFSET)
            End With
            DATAPOOL.getInstance.addObject(objRCJ)
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
            objRCJ = Nothing
        Catch ex As Exception
            DATAPOOL.getInstance.ResetPoolData()
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        End Try
    End Function
    ''' <summary>
    ''' Parse the recall list response message.
    ''' </summary>
    ''' <param name="strRCC_Message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseRCC(ByVal strRCC_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim objRCC As New RCCRecord
        Dim strIndex As String
        Try
            With objRCC
                .cRecallType = strRCC_Message.Substring(RCC.RECALL_TYPE_OFFSET, RCC.RECALL_TYPE)
                .cSpecialInst = strRCC_Message.Substring(RCC.SPECIAL_INSTRUCTION_OFFSET, RCC.SPECIAL_INSTRUCTION)
                .strActiveDate = strRCC_Message.Substring(RCC.ACTIVE_DATE_OFFSET, RCC.ACTIVE_DATE)
                strIndex = strRCC_Message.Substring(RCC.INDEX_OFFSET, RCC.INDEX)
                .strIndex = strIndex
                .strLabType = strRCC_Message.Substring(RCC.LAB_TYPE_OFFSET, RCC.LAB_TYPE)
                .strMRQ = strRCC_Message.Substring(RCC.MRQ_OFFSET, RCC.MRQ)
                .strRecallCount = strRCC_Message.Substring(RCC.RECALL_COUNT_OFFSET, RCC.RECALL_COUNT)
                .strRecallDesc = strRCC_Message.Substring(RCC.RECALL_DESC_OFFSET, RCC.RECALL_DESC)
                .strRecallRefNo = strRCC_Message.Substring(RCC.RECALL_REF_OFFSET, RCC.RECALL_REF)
                'Tailoring
                If (strRCC_Message.Substring(RCC.TAILORED_OFFSET, RCC.TAILORED)) = "Y" Then
                    If .cRecallType = "R" Or .cRecallType = "S" Then
                        .strTailored = True
                    Else
                        .strTailored = False
                    End If
                Else
                    .strTailored = False
                End If
                .strListStatus = strRCC_Message.Substring(RCC.RECALL_LIST_STATUS, RCC.RECALL_STATUS)
                'change for Batch nos
                .strBatchNos = strRCC_Message.Substring(RCC.BATCH_NOS_OFFSET, RCC.BATCH_NOS)
            End With
            m_TEMPDATAPOOL.Add(objRCC)
            objRCC = Nothing
            bTemp = objAppContainer.objExportDataManager.SendNextSequence((Convert.ToInt16(strIndex) + 1).ToString())
        Catch ex As Exception
            m_TEMPDATAPOOL.Clear()
            DATAPOOL.getInstance.ResetPoolData()
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        End Try
    End Function
    ''' <summary>
    ''' Parse response to recall exit.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseRCE() As Boolean
        Dim bTemp As Boolean = False
        Try
            For Each objDSR In m_TEMPDATAPOOL
                DATAPOOL.getInstance.addObject(objDSR)
            Next
            'Fix to handle the issue where the number of items in recall is in 10s then the 
            'last RCF sent contains moretocome flag set to 'Y' even though all the items are 
            'received.
            If objRCFRecord.strRecallRef IsNot Nothing Then
                DATAPOOL.getInstance.addObject(objRCFRecord)
                isFirstParse = True
                objRCFRecord = Nothing
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("PARSE RCE:: Exception " + ex.Message(), Logger.LogLevel.RELEASE)
        Finally
            m_TEMPDATAPOOL.Clear()
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        End Try
    End Function
    ''' <summary>
    ''' Parse the recall list item response message that carries the list of
    ''' items in a selected recall.
    ''' </summary>
    ''' <param name="strRCF_Message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseRCF(ByVal strRCF_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim cMoreToComeFlag As Char
        Dim objRCFInnerRecord As RCFInnerRecord
        Dim counter As Integer = 0
        Dim iIndexPtr As Integer = 0
        '
        Dim bActionedRecall As Boolean = False
        Try
            cMoreToComeFlag = strRCF_Message.Substring(RCF.MORE_ITEMS_OFFSET, RCF.MORE_ITEMS)
            If isFirstParse Then
                isFirstParse = False
                objRCFRecord = New RCFRecord()
                'Get recall reference number and the recall status.
                objRCFRecord.strRecallRef = strRCF_Message.Substring(RCF.RECALL_REF_NUMBER_OFFSET, RCF.RECALL_REF_NUM)
                objRCFRecord.cRecallStatus = strRCF_Message.Substring(RCF.RECALL_STATUS_OFFSET, RCF.RECALL_STATUS)
                If (objRCFRecord.InnerRecordArrays Is Nothing) Then
                    objRCFRecord.InnerRecordArrays = New ArrayList()
                End If
            End If
            Do
                objRCFInnerRecord = New RCFInnerRecord()
                objRCFInnerRecord.strRecallItem = strRCF_Message.Substring( _
                                                RCF.HEADERTOTAL + (RCF.TRAILERTOTAL * counter) + RCF.RECALL_ITEM_OFFSET, _
                                                RCF.RECALL_ITEM)
                If Not (objRCFInnerRecord.strRecallItem = "FFFFFF") Then
                    'Gets the short item description for the item.
                    objRCFInnerRecord.strItemDesc = strRCF_Message.Substring( _
                                              RCF.HEADERTOTAL + (RCF.TRAILERTOTAL * counter) + RCF.ITEM_DESCRIPTION_OFFSET, _
                                              RCF.ITEM_DESCRIPTION)
                    'Reads the item status flag for the Boots code.
                    objRCFInnerRecord.cItemFlag = strRCF_Message.Substring( _
                                           RCF.HEADERTOTAL + (RCF.TRAILERTOTAL * counter) + RCF.ITEM_FLAG_OFFSET, _
                                           RCF.ITEM_FLAG)

                    'Gets the TSF for the item.
                    objRCFInnerRecord.strTSF = strRCF_Message.Substring( _
                                               RCF.HEADERTOTAL + (RCF.TRAILERTOTAL * counter) + RCF.TSF_OFFSET, _
                                               RCF.TSF)
                    'Gets the recall count for the item in case if the item is already added to the recall in a previous session.
                    objRCFInnerRecord.strRecallCount = strRCF_Message.Substring( _
                                                    RCF.HEADERTOTAL + (RCF.TRAILERTOTAL * counter) + RCF.RECALL_COUNT_OFFSET, _
                                                    RCF.RECALL_COUNT)
                    'Get the item visible flag. Used when the list is tailored list.
                    objRCFInnerRecord.cVisible = strRCF_Message.Substring( _
                                                RCF.HEADERTOTAL + (RCF.TRAILERTOTAL * counter) + RCF.VISIBLE_OFFSET, _
                                                RCF.VISIBLE)
                    'Adds item details to an array in the structure.
                    objRCFRecord.InnerRecordArrays.Add(objRCFInnerRecord)
                Else
                    objRCFInnerRecord = Nothing
                    Exit Do
                End If
                objRCFInnerRecord = Nothing
                counter = counter + 1       'Increment the count to move to next item.
            Loop Until (counter = RCF.MAX_COUNT)
            If cMoreToComeFlag = "Y" Then
                'DATAPOOL.getInstance.addObject(objRCFRecord)
                'iIndexPtr = iIndexPtr + Macros.RECALL_RCF_ITEM_COUNT
                objAppContainer.objExportDataManager.SendNextSequence("0000")
            Else
                isFirstParse = True
                DATAPOOL.getInstance.addObject(objRCFRecord)
                objRCFRecord.InnerRecordArrays = Nothing
                objRCFRecord = Nothing
                DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
            End If
        Catch ex As Exception
            isFirstParse = True
            objRCFInnerRecord = Nothing
            objRCFRecord = Nothing
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        End Try
        If bActionedRecall Then

            bActionedRecall = False
            isFirstParse = True
            objRCFInnerRecord = Nothing
            objRCFRecord = Nothing
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)

        End If
    End Function
    ''' <summary>
    ''' Parse response for stock take request.
    ''' </summary>
    ''' <param name="strSTR_Message"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseSTR(ByVal strSTR_Message As String) As Boolean
        Dim bTemp As Boolean = False
        Dim objSTRRecord As New STRRecord()
        Try
            objSTRRecord.strUODNo = strSTR_Message.Substring(STR.UOD_NUMBER_OFFSET, STR.UOD_NUMBER)
            objSTRRecord.strUODSuffix = strSTR_Message.Substring(STR.UOD_SUFFIX_OFFSET, _
                                                                 STR.UOD_SUFFIX)
            DATAPOOL.getInstance.addObject(objSTRRecord)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error wile Parsing STR Record ::" + ex.Message, Logger.LogLevel.RELEASE)
        Finally
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
            objSTRRecord = Nothing
        End Try
    End Function
#End Region
End Class
#End If
