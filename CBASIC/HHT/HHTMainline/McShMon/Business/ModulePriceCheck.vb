#If NRF Then
#Region " NRF"

'''***************************************************************
''' <FileName>ModulePriceCheck.vb</FileName>
''' <summary>
''' Method to do Partial Price Check and to proceed to Full Price Check if required
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
Public Class ModulePriceCheck
    Private m_MPCItemList As ArrayList = New ArrayList()
    'UAT - Array to store ENQ objects
    Private m_QueuedPCMList As ArrayList = New ArrayList()
    Private iPriceCheckCount As Integer = 0
    Dim objPriceCheckData As PriceCheckInfo = New PriceCheckInfo()
    Private cCurencySymb As String = Macros.POUND_SYMBOL
    Public Sub New()
            'Assign the currency symbol
            Dim strTemp As String = ConfigDataMgr.GetInstance.GetParam(ConfigKey.VALID_CURRENCY).ToString()
            If strTemp = "S" Then
                cCurencySymb = Macros.POUND_SYMBOL
            ElseIf strTemp = "E" Then
                cCurencySymb = Macros.EURO_SYMBOL
            End If
        End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <param name="strSEL"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DoPartialPriceCheck(ByVal strProductCode As String, ByVal strSEL As String) As String
        objAppContainer.objLogger.WriteAppLog("Enter ModulePriceCheck DoPartialPriceCheck", Logger.LogLevel.RELEASE)
        Dim strSELPrice As String = ""
        Dim strBootsCode As String = ""
        Dim dcPendingPrice As Decimal
        Dim dcCurrentPrice As Decimal
        'Retrieves the boots code and price from SEL
        objAppContainer.objHelper.GetBootsCodeFromSEL(strSEL, strBootsCode)
        strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
        objAppContainer.objHelper.GetPriceFromSEL(strSEL, strSELPrice)

        objAppContainer.objDataEngine.GetPriceCheckInfo(strBootsCode, strProductCode, objPriceCheckData)
        'Pilot CR: Suppress price check for items from product groups with
        'SEL printing flag set to N
        If objPriceCheckData.SELPrintFlag = "N" Then
            Return "-1"
        End If

        'System testing
        If objPriceCheckData.CurrentPrice Is Nothing Then
            Return "1"
        End If
        Try
            If Not (objPriceCheckData.CurrentPrice = "") Then
                dcCurrentPrice = CDec(objPriceCheckData.CurrentPrice)
            Else
                dcCurrentPrice = 0D
            End If
            If Not (objPriceCheckData.PendingPrice = "") Then
                dcPendingPrice = CDec(objPriceCheckData.PendingPrice)
            Else
                dcPendingPrice = 0D
            End If

            If Not strSELPrice.Equals("") Then
                '@Service Fix - Pending price check date included
                'REmoved date check
                If (Not ComparePrice(objPriceCheckData.CurrentPrice, strSELPrice)) Then
                    'And (objPriceCheckData.PendingPriceDate.Date <= Date.Now()) Then
                    'Price mismatch. So full price check need not be done
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M48") + "    Current Price : " + _
                                    cCurencySymb + " " + objPriceCheckData.CurrentPrice.Insert(6, ".").TrimStart("0"), _
                                    "SEL Price Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                    MessageBoxDefaultButton.Button1)
                    'If (CreateENQ(strBootsCode, "P")) Then
                    CreatePCM(strBootsCode, dcPendingPrice, dcCurrentPrice, "P")
                    'End If
                    Return "0"
                Else
                    Return "1"
                End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(" ModulePriceCheck- Cannot convert Price to Decimal in DoPartialPriceCheck", Logger.LogLevel.RELEASE)
        End Try
    End Function
    Public Function IsFullPriceCheckRequired(ByVal ProductScanPrompt As Boolean) As Boolean
        objAppContainer.objLogger.WriteAppLog("Enter ModulePriceCheck IsFullPriceCheckRequired", Logger.LogLevel.RELEASE)
        Dim dcCurrentPrice As Decimal
        Dim dcPendingPrice As Decimal
        If objPriceCheckData.LastPriceCheckDate Is Nothing Then
            Return False
        End If
        Try
            If Not (objPriceCheckData.CurrentPrice = "") Then
                dcCurrentPrice = CDec(objPriceCheckData.CurrentPrice)
            Else
                dcCurrentPrice = 0D
            End If
            If Not (objPriceCheckData.PendingPrice = "") Then
                dcPendingPrice = CDec(objPriceCheckData.PendingPrice)
            Else
                dcPendingPrice = 0D
            End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("DoFullPriceCheck - Cannot convert Price to Decimal", Logger.LogLevel.RELEASE)
        End Try

        If (IsPriceCheckPeriodValid(objPriceCheckData)) Then
            If (ProductScanPrompt) Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M25"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                'System testing
            End If
            Return True
        Else
            If Not dcPendingPrice = 0D And (dcPendingPrice < dcCurrentPrice) Then
                objAppContainer.iCompletedCount += 1
            End If
            Return False
        End If
    End Function
    ''' <summary>
    ''' Method to compare the current price and the price obtained from SEL
    ''' </summary>
    ''' <param name="strCurrentPrice"></param>
    ''' <param name="strSELPrice"></param>
    ''' <returns>True if prices are same, else returns false</returns>
    ''' <remarks></remarks>
    Private Function ComparePrice(ByVal strCurrentPrice As String, ByVal strSELPrice As String) As Boolean
        Dim dcCurrentPrice As Decimal
        Dim dcSELPrice As Decimal
        'Converts the current price and SEL price to decimal
        dcCurrentPrice = CDec(strCurrentPrice)
        dcSELPrice = CDec(strSELPrice)
        'Checks if the current price is equal to price obtained from SEL
        If dcCurrentPrice = dcSELPrice Then
            Return True
        Else
            Return False
        End If
    End Function
    ''' <summary>
    ''' Function to check whether a price check has been performed within the last 28 days(configurable)
    ''' </summary>
    ''' <param name="objPriceCheckData"></param>
    ''' <returns>True if price check has not been done for more than configurable number of days</returns>
    ''' <remarks></remarks>
    Private Function IsPriceCheckPeriodValid(ByVal objPriceCheckData As PriceCheckInfo) As Boolean
        Dim dtLastPC As String
        Dim iThreshold As Integer = 0
        Dim iDateDiff As Integer
        Dim tsSpan As TimeSpan
        'Obtains the configurable number of days from database
        Dim objPCTargetDetails As PCTargetDetails = New PCTargetDetails()
        Try
            If objAppContainer.objDataEngine.GetPCTargetDetails(objPCTargetDetails) Then
                iThreshold = CInt(objPCTargetDetails.PCThreshold)
            End If
            'Obtains the last price check date and calculates the difference 
            'dtLastPC = CDate(objPriceCheckData.LastPriceCheckDate)
            'UT Changes: Convert date from database to required format 
            'System Testing
            ' Removed if else block as the date already in correct format in database
            'If CInt(objPriceCheckData.LastPriceCheckDate) = 0 Then
            '    dtLastPC = DateTime.ParseExact(ConfigDataMgr.GetInstance.GetParam( _
            '                                   ConfigKey.DEFAULT_LAST_PC_DATE).ToString(), _
            '                                   "yyyyMMdd", _
            '                                   System.Globalization.CultureInfo.CurrentCulture)
            'Else
            '    dtLastPC = DateTime.ParseExact(objPriceCheckData.LastPriceCheckDate, _
            '                                   "yyyyMMdd", _
            '                                   System.Globalization.CultureInfo.CurrentCulture)
            'End If
            ' Added new line
            dtLastPC = objPriceCheckData.LastPriceCheckDate
            'If Not (CInt(objPriceCheckData.LastPriceCheckDate) = 0) Then
            'Get the number of days elapsed after the last PC date.
            tsSpan = Now.Subtract(dtLastPC)
            iDateDiff = tsSpan.Days

            'Checks if the difference in date is more than the configurable number of days
            If (iDateDiff) > iThreshold Then
                Return True
            ElseIf (iDateDiff < 0) Then
                objAppContainer.objLogger.WriteAppLog("Module Price Check- checking PC Valid Period failure", Logger.LogLevel.RELEASE)
                Return False
                'Else
                '    Return False
                'End If
            Else
                Return False
            End If
            'Return False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("ModulePriceCheck - Exception in IsPriceCheckPeriodValid", Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ModulePriceCheck IsPriceCheckPeriodValid", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Implements the full price check for the item
    ''' </summary>
    ''' <param name="strSEL"></param>
    ''' <param name="strProductCode"></param>
    ''' <returns>True if Price Check succeeds and false if there is price error</returns>
    ''' <remarks></remarks>
    Public Function DoFullPriceCheck(ByVal strSEL As String, ByVal strProductCode As String) As Boolean
        Dim dcCurrentPrice As Decimal
        Dim dcPendingPrice As Decimal
        Dim dcSELPrice As Decimal
        Dim bComparePrice As Boolean = False
        Try
            Dim strBootsCode As String = ""
            Dim strSELPrice As String = ""
            'Retrieves the boots code and price from SEL
            objAppContainer.objHelper.GetBootsCodeFromSEL(strSEL, strBootsCode)
            'fix for having 6digits in the ENQ message
            strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
            'End fix for having 6digits in the ENQ message
            objAppContainer.objHelper.GetPriceFromSEL(strSEL, strSELPrice)
            'Converts the values to decimal
            Try
                If Not (objPriceCheckData.CurrentPrice = "") Then
                    dcCurrentPrice = CDec(objPriceCheckData.CurrentPrice)
                Else
                    dcCurrentPrice = 0D
                End If
                If Not (objPriceCheckData.PendingPrice = "") Then
                    dcPendingPrice = CDec(objPriceCheckData.PendingPrice)
                Else
                    dcPendingPrice = 0D
                End If
                If Not (strSELPrice = "") Then
                    dcSELPrice = CDec(strSELPrice)
                Else
                    dcSELPrice = 0D
                End If
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("DoFullPriceCheck - Cannot convert Prioce to Decimal", Logger.LogLevel.RELEASE)
            End Try

            If dcPendingPrice = 0D Then
                'UAT
                CreateENQ(strBootsCode, "P")
                Return True
            ElseIf dcPendingPrice > dcCurrentPrice Then
                '@Service fix - pending price check date included
                If objPriceCheckData.PendingPriceDate.Date <= Date.Now.Date Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M17"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                    If Not (IsSupervisor()) Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M18"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                    End If
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M16"), "Replace SEL", _
                                MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                    'If (CreateENQ(strBootsCode, " ")) Then
                    CreatePCM(strBootsCode, dcPendingPrice, dcCurrentPrice, " ")
                    'End If
                    Return False
                Else
                    CreateENQ(strBootsCode, "P")
                    Return True
                End If
            ElseIf dcPendingPrice < dcCurrentPrice Then
                'If counted towards weekly target, write PCM and ENQ records 
                'If (CreateENQ(strBootsCode, "P")) Then
                '@Service fix - pending price check date included
                If objPriceCheckData.PendingPriceDate.Date <= Date.Now.Date Then
                    CreatePCM(strBootsCode, dcPendingPrice, dcCurrentPrice, "P")
                    'End If
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M16"), "Replace SEL", _
                                MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                    Return False
                Else
                    CreateENQ(strBootsCode, "P")
                    Return True
                End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in full price check", Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit ModulePriceCheck DoFullPriceCheck", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' This function returns true if the user who has logged in is the superviser
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreatePCM(ByVal strBootsCode As String, ByVal dcPendingPrice As Decimal, ByVal dcCurrentPrice As Decimal, ByVal strPC As String) As Boolean
        Dim objPCMData As PCMRecord = New PCMRecord()
        Dim iVariance As Integer = 0
        objPCMData.strBootscode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
        iVariance = dcPendingPrice - dcCurrentPrice
        objPCMData.strNumVariance = iVariance.ToString()
        objPCMData.strPriceCheck = strPC
        UpdatePCMRecordList(objPCMData)
    End Function
    ''' <summary>
    ''' This function creates the ENQ records and queue it in the array
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateENQ(ByVal strBootsCode As String, ByVal strPriceCheck As String) As Boolean
        Dim objENQData As ENQRecord = New ENQRecord()
        objENQData.strBootsCode = strBootsCode
        objENQData.strPriceCheck = strPriceCheck
        UpdateMPCItemList(objENQData)
    End Function
    ''' <summary>
    ''' This function returns true if the user who has logged in is the superviser
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsSupervisor() As Boolean
        If objAppContainer.strSupervisorFlag.Equals(Macros.SUPERVISOR_YES) Then
            Return True
        Else
            Return False
        End If
    End Function

    'Integration testing
    ''' <summary>
    ''' Update the item list for already price checked Items
    ''' </summary>
    ''' <param name="objENQData">Current Product</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateMPCItemList(ByVal objENQData As ENQRecord) As Boolean
        Try
            Dim bIsPresent As Boolean = False
            For iCount = 0 To m_MPCItemList.Count - 1
                Dim objENQRecord As ENQRecord = New ENQRecord
                objENQRecord = m_MPCItemList.Item(iCount)
                If objENQRecord.strBootsCode.Equals(objENQData.strBootsCode) Then
                    m_MPCItemList.RemoveAt(iCount)
                    m_MPCItemList.Insert(iCount, objENQData)
                    bIsPresent = True
                    Exit For
                End If
            Next
            If Not bIsPresent Then
                m_MPCItemList.Add(objENQData)
                If objENQData.strPriceCheck = "P" Then
                    objAppContainer.iCompletedCount += 1
                    iPriceCheckCount += 1
                End If
            End If
            Return bIsPresent
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Price Check Session Exception at UpdateItemList" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    ''' Update the item list for already price checked Items
    ''' </summary>
    ''' <param name="objPCMData">Current Product</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdatePCMRecordList(ByVal objPCMData As PCMRecord) As Boolean
        Try
            Dim bIsPresent As Boolean = False
            For iCount = 0 To m_QueuedPCMList.Count - 1
                Dim objPCMRecord As PCMRecord = New PCMRecord
                objPCMRecord = m_QueuedPCMList.Item(iCount)
                If objPCMRecord.strBootscode.Equals(objPCMData.strBootscode) Then
                    m_QueuedPCMList.RemoveAt(iCount)
                    m_QueuedPCMList.Insert(iCount, objPCMData)
                    bIsPresent = True
                    Exit For
                End If
            Next
            If Not bIsPresent Then
                m_QueuedPCMList.Add(objPCMData)
                If objPCMData.strPriceCheck = "P" Then
                    objAppContainer.iCompletedCount += 1
                    iPriceCheckCount += 1
                End If
            End If
            Return bIsPresent
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Price Check Session Exception at UpdateItemList" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    ''' Writes the export data
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteExportData(ByVal arrQueuedSELList As ArrayList) As Boolean
        Dim iItemsChecked As Integer = 0
        Try
            Dim iSELCount As Integer = arrQueuedSELList.Count
            Dim arrPRTList As ArrayList = New ArrayList()
            arrPRTList = arrQueuedSELList
            'Dim objSMDataManager As SMTransactDataManager = New SMTransactDataManager()
            'Write PCS and PCX only if there is atleast one PCM record.
            If m_MPCItemList.Count > 0 Or m_QueuedPCMList.Count > 0 Then
                'Write PCS record
                objAppContainer.objExportDataManager.CreatePCS()
            Else
                'If there are no records created as part of price check.
                Return False
            End If

            'Write ENQ record
            For Each objENQ As ENQRecord In m_MPCItemList
                objAppContainer.objExportDataManager.CreateENQ(objENQ)
            Next

            'Write PCM records.
            For Each objPCM As PCMRecord In m_QueuedPCMList
                objAppContainer.objExportDataManager.CreatePCM(objPCM)
                Dim iReqNos As Integer = arrQueuedSELList.Count
                For iCounter As Integer = 0 To iReqNos - 1
                    Dim objPRT As PRTRecord = arrQueuedSELList(iCounter)
                    If objPRT.strBootscode = objPCM.strBootscode Then
                        objAppContainer.objExportDataManager.CreatePRT(objPRT.strBootscode, SMTransactDataManager.ExFileType.EXData)
                        'Remove the item at location iCounter which is written.
                        arrQueuedSELList.RemoveAt(iCounter)
                        'exit for loop.
                        Exit For
                    End If
                Next
            Next

            'if there are still some more PRTs left write those records.
            If arrPRTList.Count > 0 Then
                For Each objPRT As PRTRecord In arrPRTList
                    objAppContainer.objExportDataManager.CreatePRT(objPRT.strBootscode, SMTransactDataManager.ExFileType.EXData)
                Next
            End If

            If m_MPCItemList.Count() > 0 Or m_QueuedPCMList.Count() > 0 Then
                'Create PCX record
                Dim objPCXData As PCXRecord = New PCXRecord()
                objPCXData.strSELs = iSELCount.ToString()
                objPCXData.strCheckedItems = iPriceCheckCount.ToString()
                objAppContainer.objExportDataManager.CreatePCX(objPCXData)
            End If
            EndSession()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Price Check Session Exception at WriteExportData" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PCSessionMgr WriteExportData", Logger.LogLevel.RELEASE)
        Return True
    End Function
    ''' <summary>
    ''' Writes the export data
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteExportData(ByVal strBootsCode As String) As Boolean
        Dim iItemsChecked As Integer = 0
        Try
            'Dim objSMDataManager As SMTransactDataManager = New SMTransactDataManager()
            'Select ENQ records available for strBootsCode
            Dim enqList = From objENQ As ENQRecord In m_MPCItemList _
                           Select objENQ Where objENQ.strBootsCode = strBootsCode
            'Write ENQ record
            For Each objENQ As ENQRecord In enqList
                objAppContainer.objExportDataManager.CreateENQ(objENQ)
            Next
            'Select PCM record from the list
            Dim pcmList = From objPCM As PCMRecord In m_QueuedPCMList _
                           Select objPCM Where objPCM.strBootscode = strBootsCode
            'Write PCM and ENQ message
            For Each objPCM As PCMRecord In pcmList
                'Fix:System Testing:Time taken to write export data.
                objAppContainer.objExportDataManager.CreatePCM(objPCM)
            Next
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Price Check Session Exception at WriteExportData" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit PCSessionMgr WriteExportData", Logger.LogLevel.RELEASE)
        Return True
    End Function
    Public Function GetPCCountForCurrentSession() As Integer
        'Dim iPCCount As Integer = 0
        'Try
        '    Dim linQuery = From objPCM As PCMRecord In m_QueuedPCMList _
        '                   Select objPCM Where objPCM.strPriceCheck = "P"
        '    iPCCount = linQuery.Count() + m_MPCItemList.Count
        'Catch ex As Exception
        '    objAppContainer.objLogger.WriteAppLog("Get PC Count failed." + ex.StackTrace, _
        '                                          Logger.LogLevel.RELEASE)
        '    Return iPCCount
        'End Try
        ''Return the count of PC target acheived.
        'Return iPCCount
        Return iPriceCheckCount
    End Function
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by ModulePriceCheck
    ''' </summary>
    ''' <returns>True if terminate is sucess else False</returns>
    ''' <remarks></remarks>
    Public Function EndSession()
        objAppContainer.objLogger.WriteAppLog("Enter Module Price Check End Session", Logger.LogLevel.INFO)
        Try
            'Release all objects and Set to nothing.
            m_MPCItemList = Nothing
            m_QueuedPCMList = Nothing
            objPriceCheckData = Nothing
            iPriceCheckCount = Nothing
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Module Price Check Session End failure" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
        objAppContainer.objLogger.WriteAppLog("Exit Module Price Check End Session", Logger.LogLevel.RELEASE)
    End Function
End Class
#End Region


#ElseIf RF Then
''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class ModulePriceCheck
    ''' <summary>
    ''' Variable is used to track the Number of Price check done in a session
    ''' </summary>
    ''' <remarks></remarks>
    Private iPriceCheckDone As Integer = 0
    Private cCurencySymb As String = Macros.POUND_SYMBOL

    Public Function IsFullPriceCheckRequired(ByVal strBootscode As String, ByVal strProductCode As String) As Boolean
        Dim btemp As Boolean = False
        Try
            Dim objPCProductinfo As New PriceCheckInfo()
            objAppContainer.objDataEngine.GetPriceCheckInfo(strBootscode, strProductCode, objPCProductinfo)
            If (objPCProductinfo.PriceAcceptedFlag = "Y") And _
                (objPCProductinfo.PCComplete < objPCProductinfo.PCTarget) Then
                btemp = True
                iPriceCheckDone = iPriceCheckDone + 1
            Else
                'Display the reject message here if the price chekc is rejected.
                'MessageBox.Show("This item was last checked on " & objPCProductinfo.RejectMessage.ToString(), _
                '                "Information")
                btemp = False
            End If
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
            objAppContainer.objLogger.WriteAppLog("Error in IsFullPriceCheckRequired :: " + ex.Message, _
                                                  Logger.LogLevel.RELEASE)
        End Try
        Return btemp
    End Function
    Public Function DoPartialPriceCheck(ByVal strSEL As String, ByVal strActualPrice As String) As String
        Try
            Dim strSELPrice As String = ""
            Dim strBootsCode As String = ""
            objAppContainer.objHelper.GetBootsCodeFromSEL(strSEL, strBootsCode)
            strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
            objAppContainer.objHelper.GetPriceFromSEL(strSEL, strSELPrice)
            Dim Variance As Decimal = ComparePrice(strSELPrice, strActualPrice)
            If Variance = 0 Then
                Return "1"
            Else
                'In case of RF we insert a "." after 3rd digit
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M48") + "    Current Price : " + _
                               cCurencySymb + " " + strActualPrice.Insert(4, ".").TrimStart("0"), _
                               "SEL Price Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                               MessageBoxDefaultButton.Button1)
                'Create and send PCM message here
                objAppContainer.objExportDataManager.CreatePCM(strBootsCode, Variance)
                Return "0"
            End If
        Catch ex As Exception
            Return "-1"
        End Try
    End Function

    ' '' All the calculations  are done in IS Full Check Required?? Function
    ' '' Just returning a true
    ' '' In RF no logic is needed here

    'Public Function DoFullPriceCheck(ByVal strProductCode As String) As Boolean
 
    '    Return True
    ''Dim objEQR As EQRRecord = Nothing
    ''Dim objData As Object = Nothing
    ''Try
    ''    ''Create ENQ with price check flag as True
    ''    'send a price check ENQ
    ''    If objAppContainer.objExportDataManager.CreateENQ(strProductCode, True) Then
    ''        If (DATAPOOL.getInstance.GetNextObject(objData)) Then
    ''            If TypeOf (objData) Is EQRRecord Then
    ''                objEQR = CType(objData, EQRRecord)
    ''                'send a ENQ with flag "C" 
    ''                'So done a price check. Increase the price check count for every ENQ with "C" send
    ''                iPriceCheckDone = iPriceCheckDone + 1
    ''                If objEQR.cFlagPriceCheck = "Y" Then
    ''                    Return True
    ''                Else
    ''                    'Reject Message is displayed here
    ''                    'The Message is recieved from message config file - tag M85
    ''                    Return False
    ''                End If

    ''            End If
    ''        Else
    ''            Return False
    ''        End If
    ''    End If
    ''Catch ex As Exception
    ''    objAppContainer.objLogger.WriteAppLog("Exception Thrown at Do Full Price Check :: " _
    ''                                          + ex.Message, Logger.LogLevel.RELEASE)
    ''    Return False
    ''Finally
    ''    'Disposing any objects if at all assigned
    ''    objData = Nothing
    ''    objEQR = Nothing
    '    'End Try
    'End Function


    ''' <summary>
    ''' Method to compare the current price and the price obtained from SEL
    ''' </summary>
    ''' <param name="strCurrentPrice"></param>
    ''' <param name="strSELPrice"></param>
    ''' <returns>True if prices are same, else returns false</returns>
    ''' <remarks></remarks>
    Private Function ComparePrice(ByVal strCurrentPrice As String, ByVal strSELPrice As String) As Decimal

        Dim dcCurrentPrice As Decimal
        Dim dcSELPrice As Decimal
        'Converts the current price and SEL price to decimal
        dcCurrentPrice = CDec(strCurrentPrice)
        dcSELPrice = CDec(strSELPrice)
        'Checks if the current price is equal to price obtained from SEL
        Return (dcSELPrice - dcCurrentPrice)
    End Function

    ''''Have to complete the Target Details here
    'Iprice check variable to be inserted here
    Public Function GetPCCountForCurrentSession() As Integer
        Return iPriceCheckDone
    End Function
    Public Sub SetPCCOunt(ByVal Count As Integer)
        iPriceCheckDone = Count
    End Sub

    Public Sub New()
        'Assign the currency symbol
        Dim strTemp As String = ConfigDataMgr.GetInstance.GetParam(ConfigKey.VALID_CURRENCY).ToString()
        If strTemp = "S" Then
            cCurencySymb = Macros.POUND_SYMBOL
        ElseIf strTemp = "E" Then
            cCurencySymb = Macros.EURO_SYMBOL
        End If
    End Sub
End Class
#End If

