Imports System.IO.Ports
Imports System.IO
''' ***************************************************************************
''' <fileName>MobilePrintSessionManager.vb</fileName>
''' <summary>The Print SEL Container Class for mobile printing.</summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>03-Dec-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''*****************************************************************************
'''***************************************************************************** 
''' * Modification Log 
''' ******************************************************************** 
''' * 1.1   Mathew Jerry Thomas. Defect 68. Release 14 A 
''' Added new checking in SubstituteTemplateVariables  (Case "$27")
''' The Unit price starts with "£/€" then prints the symbol while printing SEL
''' ********************************************************************/

Public Class MobilePrintSessionManager
    Private Shared m_MobPrintSessionMgr As MobilePrintSessionManager = Nothing
    'variabel corresponding to the printer.
    Public Shared sPrinterStatusCommand As String = Chr(&H1B) + "h" + vbCrLf
    ' Returns 1-byte
    '         bit 4: Printer Reset   (0=reset cleared, 1=printer reset)
    '         bit 3: Battery Status  (0=voltage OK,    1=low battery)
    '         bit 2: Latch Status    (0=latch open,    1=latch closed)
    '         bit 1: Paper Status    (0=paper present, 1=out of paper)
    '         bit 0: Printer Status  (0=printer ready, 1=printer busy)
    ' 
    '         Status byte AND &H0F:   00h = OK otherwise printer problem

    Private sPrinterInfoCommand As String = Chr(&H1B) + "v" + vbCrLf
    ' Typically returns 16-bytes eg: "Zebra QL 320+ V9"

    Private sExtendedPrinterStatusCommand As String = Chr(&H1B) + "i" + vbCrLf
    ' Typically returns 35-bytes: "0.12 09/21/05 55D8 XXVQ06-49-5042**"  
    ' where ** = non-printable chars (00 10)
    ' in practice you should search for the byte after the NULL (00) byte.
    '         bit 7: Ribbon Status   (0=ribbon detected, 1=ribbon not detected)
    '         bit 6: Paper Status    (0=OK,              1=low)
    '         bit 5: Peeler Sensor   (0=label removed,   1=label not removed)
    '         bit 4: Paper Jam       (0=detected,        1=not detected)
    '
    '     mask bits 0 - 3 when reading status
    '     Issue PAPER JAM command before requesting paper jam status

    Private m_CommPort As New SerialPort("COM1:", 19200, 0, 8, 1)

    Public Shared StandardLabelTemplate As New ArrayList
    Public Shared WasNowLabelTemplate As New ArrayList
    Public Shared WasWasNowLabelTemplate As New ArrayList
    Public Shared ClearanceLabelTemplate As New ArrayList
    Public Shared PainKillerLabelTemplate As New ArrayList
    Public Shared WEEELabelTemplate As New ArrayList
    Public Shared RFClearanceLabelTemplate As New ArrayList
    Public Shared CWClearanceLabelTemplate As New ArrayList 'Darwin change

    'Private objItem As ItemInfoData      'holds the item data
    Private m_ItemDetails As PSProductInfo = New PSProductInfo()
    Private bNotFirstPrint As Boolean = False
    Private m_LabelType As LabelType
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        Try
            'Create the coomunication port connection.
            'm_CommPort = New SerialPort("COM1:", 19200, 0, 8, 1)
            'constructor code goes here
            ReadTemplateFiles()
        Catch ex As Exception

        End Try
    End Sub
    ''' <summary>
    ''' Enum to set the label type before printing.
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum LabelType
        STD
        WN
        WWN
        CLR
        RCLR
        CWCLR   'Darwin change
        WEEE
    End Enum
    ''' <summary>
    ''' Property to share the mobile pritner status.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property MobilePrinterStatus() As Boolean
        Get
            Return CheckPrinterStatus()
        End Get
    End Property
    ''' <summary>
    ''' Function to check and return the mobie ptinter status.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckPrinterStatus() As Boolean
        Dim strBufferText As String
        Dim strMsg As String
        Dim btBufferVal As Byte
        Dim bRetVal As Boolean
        Try
            'Create the coomunication port connection.
            'm_CommPort = New SerialPort("COM1:", 19200, 0, 8, 1)
            Threading.Thread.Sleep(500) 'wait for 3 sec and then read from port.
            m_CommPort.FlushComIn()  'Clear the data that is currently residing in the buffer.
            m_CommPort.TransmitBinary(sPrinterStatusCommand)
            Threading.Thread.Sleep(2000)
            strBufferText = m_CommPort.ReadFromPort()

            If strBufferText.Length > 0 Then
                'Mobile printer definitely attached to serial COM port 1
                btBufferVal = Asc(strBufferText)
                btBufferVal = btBufferVal And &HF
                If btBufferVal <> 0 Then
                    If btBufferVal = 1 Then ' Should not happen
                        strMsg = "Mobile Printer Busy - Please Retry."
                    ElseIf btBufferVal = 3 Then
                        strMsg = "Please check labels in mobile printer."
                    ElseIf btBufferVal = 7 Then
                        strMsg = "Please check latch on mobile printer."
                    ElseIf btBufferVal = 8 Then
                        strMsg = "Mobile printer needs recharging."
                    Else
                        strMsg = "Please check mobile printer."
                    End If
                    'Display the error message to the user.
                    MessageBox.Show(strMsg, "Printer Error")
                End If
                'MessageBox.Show(btBufferVal.ToString())
                'set the return value.
                bRetVal = True
            Else
                bRetVal = False
            End If
        Catch
            'Mobile printer NOT attached
            strMsg = "Mobile Printer not responding. " & vbCrLf & vbCrLf & _
                     "Please Sign Off, re-attach printer and Sign On again."
            'Display the error message to the user.
            MessageBox.Show(strMsg, "Printer Error")
            bRetVal = False
        End Try
        'return the status value.
        ' MessageBox.Show(bRetVal)
        Return bRetVal
    End Function
    ''' <summary>
    ''' To return one instance for this class at any point of time.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As MobilePrintSessionManager
        If m_MobPrintSessionMgr Is Nothing Then
            Try
                m_MobPrintSessionMgr = New MobilePrintSessionManager()
                Return m_MobPrintSessionMgr
            Catch ex As Exception
                'Any error occurred during the serial port connection.
                m_MobPrintSessionMgr = Nothing
                Return m_MobPrintSessionMgr
            End Try
        Else
            Return m_MobPrintSessionMgr
        End If
    End Function
    ''' <summary>
    ''' To read all the template files and store it in array variable.
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub ReadTemplateFiles()
        ReadTemplateFileIntoArray(Macros.STANDARD_TEMPLATE_FILE, StandardLabelTemplate)
        ReadTemplateFileIntoArray(Macros.WASNOW_TEMPLATE_FILE, WasNowLabelTemplate)
        ReadTemplateFileIntoArray(Macros.WASWASNOW_TEMPLATE_FILE, WasWasNowLabelTemplate)
        ReadTemplateFileIntoArray(Macros.CLEARANCE_TEMPLATE_FILE, ClearanceLabelTemplate)
        ReadTemplateFileIntoArray(Macros.PAINKILLER_TEMPLATE_FILE, PainKillerLabelTemplate)
        ReadTemplateFileIntoArray(Macros.WEEE_TEMPLATE_FILE, WEEELabelTemplate)
        ReadTemplateFileIntoArray(Macros.RF_CLEARANCE_TEMPLATE_FILE, RFClearanceLabelTemplate)
        ReadTemplateFileIntoArray(Macros.CW_CLEARANCE_TEMPLATE_FILE, CWClearanceLabelTemplate) 'Darwin change
    End Sub
    ''' <summary>
    ''' To read the template files to arrays.
    ''' </summary>
    ''' <param name="TemplateFileName"></param>
    ''' <param name="TemplateArray"></param>
    ''' <remarks></remarks>
    Public Shared Sub ReadTemplateFileIntoArray(ByVal TemplateFileName As String, ByRef TemplateArray As ArrayList)
        Dim Line As String
        Try
            Dim reader As New StreamReader(TemplateFileName)
            Line = reader.ReadLine
            While Line <> Nothing
                TemplateArray.Add(Line)
                Line = reader.ReadLine
            End While
            reader.Close()
            reader = Nothing
        Catch ex As Exception
            MsgBox("Label Template file is missing. Please contact the Service Desk.", MsgBoxStyle.OkOnly, "ERROR")
        End Try
    End Sub
    ''' <summary>
    ''' To print SEL for the item scanned using Mobile printer or 
    ''' instant batch print or batch print for batch mode.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function PrintLabels(ByRef m_PSProductInfo As PSProductInfo) As Boolean
        If CheckPrinterStatus() Then
            'Print the lable using the mobile printer.
            MobilePrintSessionManager.GetInstance.CreateLabels(m_PSProductInfo)
#If RF Then
        Else
            'Incase if in RF and mobile printer not attached send PRT request.
            objAppContainer.objExportDataManager.CreatePRT(m_PSProductInfo.BootsCode, _
                                                           SMTransactDataManager.ExFileType.EXData)
#End If
        End If
    End Function
    ''' <summary>
    ''' To create labels according to the quantity requested.
    ''' </summary>
    ''' <param name="objProductDetails"></param>
    ''' <remarks></remarks>
    Public Sub CreateLabels(ByVal objProductDetails As PSProductInfo)
        Dim strLabel As String = ""
        Dim iIndex As Integer = 0
        Dim iNoOfLabels As Integer = 0
        Try
            'copy the object content
            m_ItemDetails = objProductDetails

            iNoOfLabels = m_ItemDetails.LabelQuantity
            'Create the unit price line and format the supply route.

            m_ItemDetails.UnitPriceLine = objAppContainer.objHelper.GenerateUnitPriceLine(m_ItemDetails.UnitPriceFlag, _
                                                                                          m_ItemDetails.CurrentPrice, _
                                                                                          m_ItemDetails.UnitMeasure, _
                                                                                          m_ItemDetails.UnitQuantity, _
                                                                                          m_ItemDetails.UnitType, _
                                                                                          m_ItemDetails.CurrencyType, _
                                                                                          m_ItemDetails.MajorCurrencySymbol)
            'format supply route
            m_ItemDetails.SupplyRoute = objAppContainer.objHelper.FormatSupplyRoute(m_ItemDetails.SupplyRoute)
            'MessageBox.Show("Starting to print....")
            Select Case objProductDetails.PHFType
                Case LabelType.STD
                    If Left(m_ItemDetails.PainKillerMessage, 1) = " " Then
                        strLabel = BuildLabel(StandardLabelTemplate)
                    Else
                        strLabel = BuildLabel(PainKillerLabelTemplate)
                    End If

                Case LabelType.WN
                    strLabel = BuildLabel(WasNowLabelTemplate)
                Case LabelType.WWN
                    strLabel = BuildLabel(WasWasNowLabelTemplate)
                Case LabelType.CLR
                    strLabel = BuildLabel(ClearanceLabelTemplate)
                Case LabelType.WEEE
                    strLabel = BuildLabel(WEEELabelTemplate)
                Case LabelType.RCLR
                    strLabel = BuildLabel(RFClearanceLabelTemplate)
                Case LabelType.CWCLR
                    strLabel = BuildLabel(CWClearanceLabelTemplate)
            End Select
         
            'MessageBox.Show(strLabel)
            For i = 1 To iNoOfLabels
                m_CommPort.FlushComIn()       'Clear the buffer.
                m_CommPort.TransmitBinary(strLabel)          'issue the print.
                'delays the printing by half second.
                Threading.Thread.Sleep(500)
            Next
            'Clean the private object so that for next item scanned to use.
            m_ItemDetails = New PSProductInfo()
        Catch ex As Exception
            'MessageBox.Show(ex.Message)
            objAppContainer.objLogger.WriteAppLog("Exception Occured @ Freeze Controls " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To build label using corresponding template file.
    ''' </summary>
    ''' <param name="SELtemplate"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function BuildLabel(ByRef SELtemplate As ArrayList) As String
        Dim strLine As String
        Dim strPrintLine As String = ""
        Try
            Dim myEnumerator As System.Collections.IEnumerator = SELtemplate.GetEnumerator()
            'MessageBox.Show("building label")
            'Line = myEnumerator.Current
            While myEnumerator.MoveNext()
                strLine = myEnumerator.Current
                strPrintLine += SubstituteTemplateVariables(strLine) + vbCrLf
            End While
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        'Return the label after building it.
        Return strPrintLine
    End Function
    ''' <summary>
    ''' This function takes a line as input, substitutes the parameter 
    ''' variable denoted by a $ and returns the new string
    ''' NB: A selection of fonts have been used in order to:
    '''  1. Match the labels printed by the Blaster Advantage printer.
    '''  2. Overcome font limitations - some fonts are just numeric symbols only ie.
    '''     cannot print £, p and €, c symbols.
    ''' </summary>
    ''' <param name="strChangeString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SubstituteTemplateVariables(ByVal strChangeString As String) As String
        Dim ReturnString As String = ""
        Dim TemplateVariable As String
        Dim Loc As Integer
        Dim NextSpace As Integer
        Dim iWEEEprfPrice As Integer
        Dim iWEEEDiff As Integer
        Dim iSELPrice As Integer
        Try
            ReturnString = strChangeString
            'Locate parameter in string
            Loc = strChangeString.IndexOf("$", 0)

            If Loc >= 0 Then
                'Position of minor currency symbol
                If strChangeString.IndexOf("$17", 0) = 11 Then
                    If m_ItemDetails.CurrentPrice < 10 Then
                        strChangeString = Replace(strChangeString, "X", "130", 1, -1, CompareMethod.Text)
                        strChangeString = Replace(strChangeString, "Y", "90", 1, -1, CompareMethod.Text)    'Darwin change
                    Else
                        strChangeString = Replace(strChangeString, "X", "155", 1, -1, CompareMethod.Text)
                        strChangeString = Replace(strChangeString, "Y", "116", 1, -1, CompareMethod.Text)   'Darwin change
                    End If
                    Loc = strChangeString.IndexOf("$", 0)
                End If
                ReturnString = Left(strChangeString, Loc)
                Loc += 1 'Fix offset as IndexOf is base 0 whereas other string methods are base 1

                If strChangeString.Length = (Loc + 1) Then 'Single Digit at end
                    TemplateVariable = Mid(strChangeString, Loc, 2)
                    Loc += 2
                Else 'either 2 digits at end or more data after $x or $xx parameter
                    NextSpace = strChangeString.IndexOf(" ", Loc)
                    If NextSpace = -1 Then ' Must be 2 digits on end with no more text
                        TemplateVariable = Mid(strChangeString, Loc, 3)
                        Loc += 3
                    Else 'One or more digits with more text after
                        NextSpace += 1
                        TemplateVariable = Mid(strChangeString, Loc, (NextSpace - Loc))
                        Loc = NextSpace - 1
                    End If
                End If
                'MessageBox.Show(TemplateVariable)
                Select Case TemplateVariable
                    Case "$1"   'Currency Symbol      (£ or €)                                EQR
                        If Val(m_ItemDetails.CurrentPrice) > 99 Then
                            ReturnString += CheckCurrency(m_ItemDetails.CurrencyType)
                        End If
                    Case "$2"   'Description Line 1   Character string 15 max                 EQR
                        If m_ItemDetails.PHFType = LabelType.CWCLR Or _
                        m_ItemDetails.PHFType = LabelType.RCLR Then
                            ReturnString += Trim(Mid(m_ItemDetails.ShortDescription, 1, 20))
                        Else
                            ReturnString += Trim(Mid(m_ItemDetails.Description, 1, 15))
                        End If
                    Case "$3"   'Description Line 2   Character string 15 max                 EQR
                        ReturnString += Trim(Mid(m_ItemDetails.Description, 16, 15))
                    Case "$4"   'Description Line 3   Character string 15 max                 EQR
                        ReturnString += Trim(Mid(m_ItemDetails.Description, 31, 15))
                    Case "$5"   'Unit Price Line      Character string 15 max                 LPR
                        ReturnString += m_ItemDetails.UnitPriceLine
                    Case "$6"   'Current Price        999.99                                  EQR
                        ReturnString += Helper.FormatMoney(m_ItemDetails.CurrentPrice)
                    Case "$7"   'Was Price 1          999.99 (populated type 1/2 only)        LPR
                        ReturnString += Helper.FormatItemPrice(m_ItemDetails.WasPrice1)
                        ReturnString += AddMinorCurrencySymbol(m_ItemDetails.WasPrice1, _
                                                               m_ItemDetails.CurrencyType)
                    Case "$8"   'Was Price 2          999.99 (populated type 2 only)          LPR
                        ReturnString += Helper.FormatItemPrice(m_ItemDetails.WasPrice2)
                        ReturnString += AddMinorCurrencySymbol(m_ItemDetails.WasPrice2, _
                                                               m_ItemDetails.CurrencyType)
                    Case "$9"   'SEL Barcode data: nnnnnn99999c generated
                        ReturnString += FormatPrintedBarcode(m_ItemDetails.BootsCode, _
                                                             m_ItemDetails.CurrentPrice.ToString())
                    Case "$10"  'AdCard Marker        * or blank                              EQR
                        ReturnString += m_ItemDetails.Advantage
                    Case "$11"  'Boots Code           nn-nn-nnn                               EQR
                        '$11  $12 combined
                        ReturnString += objAppContainer.objHelper.FormatBarcode(m_ItemDetails.BootsCode)
                        ReturnString += " " + m_ItemDetails.SupplyRoute
                    Case "$12"  'Supply Route         E or C or Blank   (W changed to E)      EQR
                        ReturnString += m_ItemDetails.SupplyRoute
                    Case "$13"  'WEEE value           999.99                                  LPR
                        ReturnString += m_ItemDetails.WEEEPrfPrice
                    Case "$14"  'MS Marker            MS or " "                               LPR
                        If m_ItemDetails.MSFlag.Equals("Y") Then
                            ReturnString += "MS"
                        Else
                            ReturnString += " "
                        End If
                    Case "$15"  'WEEE Product Price Line
                        iWEEEprfPrice = Val(m_ItemDetails.WEEEPrfPrice)
                        iSELPrice = Val(m_ItemDetails.CurrentPrice)
                        iWEEEDiff = iSELPrice - iWEEEprfPrice
                        ReturnString += Helper.FormatItemPrice(iWEEEDiff.ToString)
                        ReturnString += AddMinorCurrencySymbol(iWEEEDiff.ToString, _
                                                               m_ItemDetails.CurrencyType)
                    Case "$16"  'WEEE Recycling Price Line  
                        ReturnString += Helper.FormatItemPrice(m_ItemDetails.WEEEPrfPrice)
                        ReturnString += AddMinorCurrencySymbol(m_ItemDetails.WEEEPrfPrice, _
                                                               m_ItemDetails.CurrencyType)
                    Case "$17"  'p - pence or c - cents (in smaller font!)
                        ReturnString += AddMinorCurrencySymbol(m_ItemDetails.CurrentPrice, _
                                                               m_ItemDetails.CurrencyType)
                    Case "$18"  'Painkiller Warning eg. Incl. Aspirin
                        ReturnString += m_ItemDetails.PainKillerMessage
                    Case "$19"  '£ - Pounds or € - Euros for WAS/NOW
                        ReturnString += AddMajorCurrencySymbol(m_ItemDetails.WasPrice1, _
                                                               m_ItemDetails.CurrencyType)
                    Case "$20"  '£ - Pounds or € - Euros for WAS/WAS NOW
                        ReturnString += AddMajorCurrencySymbol(m_ItemDetails.WasPrice2, _
                                                               m_ItemDetails.CurrencyType)
                    Case "$21"  'p - pence or c - cents for Clearance
                        ReturnString += AddMinorCurrencySymbol(m_ItemDetails.CurrentPrice, _
                                                               m_ItemDetails.CurrencyType)
                    Case "$22"  '£ - Pounds or € - Euros for WEEE Recycle Price
                        iWEEEprfPrice = Val(m_ItemDetails.WEEEPrfPrice)
                        iSELPrice = Val(m_ItemDetails.CurrentPrice)
                        iWEEEDiff = iSELPrice - iWEEEprfPrice
                        ReturnString += AddMajorCurrencySymbol(iWEEEDiff.ToString, _
                                                               m_ItemDetails.CurrencyType)
                    Case "$23"  '£ - Pounds or € - Euros for WEEE Product Price
                        ReturnString += AddMajorCurrencySymbol(m_ItemDetails.WEEEPrfPrice, _
                                                               m_ItemDetails.CurrencyType)
                    Case "$24"  'p - pence or c - cents for Now (in WAS or WAS/WAS)
                        ReturnString += AddMinorCurrencySymbol(m_ItemDetails.CurrentPrice, _
                                                               m_ItemDetails.CurrencyType)
                    Case "$25"  '£ - Pounds or € - Euros for Unit Price Line
                        If m_ItemDetails.UnitPriceFlag = "Y" Then
                            ReturnString += m_ItemDetails.MajorCurrencySymbol
                        End If
                    Case "$26" 'A8B Mobile Printing Phase 2 - 16/01/2008 - CIP item flag
                        If m_ItemDetails.CIPFlag = "Y" Then
                            ReturnString += Macros.CIP_MARKER
                        End If

                        'Start 1.1
                        'Added new validation for "$27" to check a symbol precedes the Unitprice
                    Case "$27"   'Returns  £/€ for unit price
                        If m_ItemDetails.UnitPriceLine.Trim() <> "" Then
                            If m_ItemDetails.UnitPriceLine.Trim().Substring(0, 1) = "£" Or _
                            m_ItemDetails.UnitPriceLine.Trim().Substring(0, 1) = "€" Then
                                ReturnString += m_ItemDetails.UnitPriceLine.Substring(0, 1)
                            Else
                                ReturnString += ""
                            End If
                        Else
                            ReturnString += ""
                        End If
                        'End 1.1

                End Select
                If Loc < strChangeString.Length Then   'Add rest of string
                    ReturnString += Right(strChangeString, (strChangeString.Length - Loc))
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        'Return the string with all required fields substituted.
        Return ReturnString
    End Function
    ''' <summary>
    ''' To amend minor currency symbol.
    ''' </summary>
    ''' <param name="strPrice"></param>
    ''' <param name="CurrencySymbol"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddMinorCurrencySymbol(ByVal strPrice As String, ByVal CurrencySymbol As String) As String
        Dim CurrMinor As String
        AddMinorCurrencySymbol = ""
        If Val(strPrice) < 100 Then
            CurrMinor = Macros.CENTS_SYMBOL
            If CurrencySymbol <> "E" Then 'Default to Sterling
                CurrMinor = Macros.PENCE_SYMBOL
            End If
            AddMinorCurrencySymbol = CurrMinor
        End If
    End Function
    ''' <summary>
    ''' To amend the major currency symbol into template.
    ''' </summary>
    ''' <param name="strPrice"></param>
    ''' <param name="CurrencySymbol"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddMajorCurrencySymbol(ByVal strPrice As String, ByVal CurrencySymbol As String) As String
        Dim CurrMajor As String
        AddMajorCurrencySymbol = ""
        If Val(strPrice) > 99 Then
            CurrMajor = Macros.EURO_SYMBOL
            If CurrencySymbol <> "E" Then 'Default to Sterling
                CurrMajor = Macros.POUND_SYMBOL
            End If
            AddMajorCurrencySymbol = CurrMajor
        End If
    End Function
    ''' <summary>
    ''' Check the currency type and return the string accordingly.
    ''' </summary>
    ''' <param name="currencyCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckCurrency(ByVal currencyCode As String) As String
        Dim currencySymbol As String = ""
        Select Case currencyCode.ToUpper()
            Case "S"
                currencySymbol = "£"
            Case "E"
                currencySymbol = "€"
        End Select
        CheckCurrency = currencySymbol
    End Function
    ''' <summary>
    ''' Function to format the SEL code to be printed on the ticket.
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <param name="strPrice"></param>
    ''' <returns>Type = 5 when the print request is to print clearance label - RCLR.</returns>
    ''' <remarks></remarks>
    Public Function FormatPrintedBarcode(ByVal strBootsCode As String, ByVal strPrice As String) As String
        '7-digit Boots Code (with checkdigit)
        Dim strSELCode As String = ""
        Dim sCode25 As String
        Dim sWeighting As String
        Dim iCheckDigit As Integer
        Dim iPos As Integer
        Dim sLength As Integer
        Try
            If m_ItemDetails.PHFType = LabelType.RCLR Then
                sLength = 15
                sCode25 = "8270" + Left(strBootsCode, 6) + Right("00000" + strPrice, 5)
                sWeighting = "131313131313131"
            ElseIf m_ItemDetails.PHFType = LabelType.CWCLR Then 'Darwin change
                sLength = 21
                sCode25 = "8270" + Left(strBootsCode, 6) + Right("00000" + strPrice, 5) + Right("000000" + m_ItemDetails.OriginalPrice, 6)
                sWeighting = "131313131313131313131"
            Else
                sLength = 11
                sCode25 = Left(strBootsCode, 6) + Right("00000" + strPrice, 5)
                sWeighting = "13131313131"
            End If

            iCheckDigit = 0

            'Calc. the product of the Boots Code/Price string with weighting
            'A8B Mobile Printing Phase 2 - 16/01/2008 - Mark Goode also calculates weighting for RF clearance barcodes
            For iPos = 1 To sLength
                iCheckDigit = iCheckDigit + Val(Mid(sCode25, iPos, 1)) * Val(Mid(sWeighting, iPos, 1))
                iCheckDigit = iCheckDigit Mod 10
            Next iPos
            'Calculate the check by subracting the reminder from 10.
            iCheckDigit = 10 - iCheckDigit

            'Print the Code 25 Bar code of the 12 digit string / 16 digit string
            '( Boots code / Price / Check Digit )/( 8270 / Boots code / Price / Check Digit 
            strSELCode = sCode25 + Right(iCheckDigit.ToString, 1)
        Catch ex As Exception

        End Try
        Return strSELCode
    End Function
    ''' <summary>
    ''' To send required font and templates to the mobile printer
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendFonts(ByVal bReader As BinaryReader) As Boolean
        Dim bBytes(48) As Byte
        Try
            m_CommPort.FlushComIn()

            bBytes = bReader.ReadBytes(48)

            While bBytes.Length > 0
                m_CommPort.TransmitFont(bBytes)
                bBytes = bReader.ReadBytes(48)
            End While

            m_CommPort.TransmitFont(bBytes)

            bReader.Close()
        Catch ex As Exception

        End Try
    End Function
    ''' <summary>
    ''' To perform a test print.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function TestPrint(ByVal sLabelText As String) As Boolean
        Try
            If MobilePrintSessionManager.GetInstance.MobilePrinterStatus Then
                m_CommPort.FlushComIn()
                m_CommPort.TransmitBinary(sLabelText)
                'Display the warning message.
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M82"), "Printer Information", _
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            Else
                'display the message if the mobile printer is not detected.
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M83"), "Printer Information", _
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            End If
        Catch ex As Exception

        End Try
    End Function
End Class
''' <summary>
''' Class to define and execute the Serial port functions.
''' </summary>
''' <remarks></remarks>
Public Class SerialPort
    Implements IDisposable

    'Variables and inheritted funstions used by the class.
    Private Declare Function CreateFile Lib "coredll.dll" (ByVal lpFileName As String, ByVal dwDesiredAccess As Int32, ByVal dwShareMode As Int32, ByVal lpSecurityAttributes As IntPtr, ByVal dwCreationDisposition As Int32, ByVal dwFlagsAndAttributes As Int32, ByVal hTemplateFile As IntPtr) As IntPtr
    Private Declare Function GetCommState Lib "coredll.dll" (ByVal nCid As IntPtr, ByRef lpDCB As DCB) As Boolean
    Private Declare Function SetCommState Lib "coredll.dll" (ByVal nCid As IntPtr, ByRef lpDCB As DCB) As Boolean
    Private Declare Function GetCommTimeouts Lib "coredll.dll" (ByVal hFile As IntPtr, ByRef lpCommTimeouts As COMMTIMEOUTS) As Boolean
    Private Declare Function SetCommTimeouts Lib "coredll.dll" (ByVal hFile As IntPtr, ByRef lpCommTimeouts As COMMTIMEOUTS) As Boolean
    Private Declare Function WriteFile Lib "coredll.dll" (ByVal hFile As IntPtr, ByVal lpBuffer As Byte(), ByVal nNumberOfBytesToWrite As Int32, ByRef lpNumberOfBytesWritten As Int32, ByVal lpOverlapped As IntPtr) As Boolean
    Private Declare Function ReadFile Lib "coredll.dll" (ByVal hFile As IntPtr, ByVal lpBuffer As Byte(), ByVal nNumberOfBytesToRead As Int32, ByRef lpNumberOfBytesRead As Int32, ByVal lpOverlapped As IntPtr) As Boolean
    Private Declare Function CloseHandle Lib "coredll.dll" (ByVal hObject As IntPtr) As Boolean

    Public Const NOPARITY As Int32 = 0    'No Parity
    Public Const ONESTOPBIT As Int32 = 0  '1 Stop Bit
    ''' <summary>
    ''' Structure used to perform operation
    ''' </summary>
    ''' <remarks></remarks>
    Private Structure DCB
        Public DCBlength As Int32
        Public BaudRate As Int32
        Public fBitFields As Int32 'See Win32API.Txt
        Public wReserved As Int16
        Public XonLim As Int16
        Public XoffLim As Int16
        Public ByteSize As Byte
        Public Parity As Byte
        Public StopBits As Byte
        Public XonChar As Byte
        Public XoffChar As Byte
        Public ErrorChar As Byte
        Public EofChar As Byte
        Public EvtChar As Byte
        Public wReserved1 As Int16
    End Structure
    Private fBinary As Integer = 1                   'binary mode, no EOF check
    Private fParity As Integer = 2                   'enable parity checking
    Private fOutxCtsFlow As Integer = 4              'CTS output flow control
    Private fOutxDsrFlow As Integer = 8              'DSR output flow control
    Private fDTRControlDisable As Integer = 0        'DTR flow control type (2 bits)
    Private fDtrControlEnable As Integer = 16        'Enables DTR Line, and leaves it on.
    Private fDTRControlHandshake As Integer = 32     'Standard DTR handshaking
    Private fDsrSensitivity As Integer = 64          'DSR sensitivity
    Private fTXContinueOnXoff As Integer = 128       'XOFF continues Tx
    Private fOutX As Integer = 256                   'XON/XOFF out flow control
    Private fInX As Integer = 512                    'XON/XOFF in flow control
    Private fErrorChar As Integer = 1024             'enable error replacement
    Private fNull As Integer = 2048                  'enable null stripping
    Private fRTSControlDisable As Integer = 0        'Flow control Line (2 bits)
    Private fRTSControlEnable As Integer = 4096      'Enable RTS Flow control line, and leave it on
    Private fRTSControlHandshake As Integer = 8192   'Enables standard RTS handshaking
    Private fAbortOnError As Integer = 16384         'abort reads/writes on error
    Private fDummy2 As Integer = 32768               'Reserved 

    Private hSerialPort As IntPtr
    Private MyDCB As DCB
    Private MyCommTimeouts As COMMTIMEOUTS
    Private oEncoder As New System.Text.ASCIIEncoding
    Private oEnc As System.Text.Encoding = oEncoder.GetEncoding(1252)
    Private sPortName As String

    Private Const GENERIC_READ As Int32 = &H80000000
    Private Const GENERIC_WRITE As Int32 = &H40000000
    Private Const OPEN_EXISTING As Int32 = 3
    Private Const FILE_ATTRIBUTE_NORMAL As Int32 = &H80
    ''' <summary>
    ''' Structure to set the time out values.
    ''' </summary>
    ''' <remarks></remarks>
    Private Structure COMMTIMEOUTS
        Public ReadIntervalTimeout As Int32
        Public ReadTotalTimeoutMultiplier As Int32
        Public ReadTotalTimeoutConstant As Int32
        Public WriteTotalTimeoutMultiplier As Int32
        Public WriteTotalTimeoutConstant As Int32
    End Structure
    ''' <summary>
    ''' Opens the serial port using the parameters supplied.
    ''' </summary>
    ''' <param name="strPortName"></param>
    ''' <param name="iBaudRate"></param>
    ''' <param name="bParity"></param>
    ''' <param name="bByteSize"></param>
    ''' <param name="bStopBits"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal strPortName As String, ByVal iBaudRate As Integer, ByVal bParity As Byte, ByVal bByteSize As Byte, ByVal bStopBits As Byte)
        Me.sPortName = strPortName
        OpenPort(strPortName, iBaudRate, bParity, bByteSize, bStopBits)
    End Sub
    ''' <summary>
    ''' Transmits up to 48 bytes out the port.
    ''' </summary>
    ''' <param name="bToSend"></param>
    ''' <remarks></remarks>
    Public Sub TransmitBinary(ByVal bToSend As String)
        Dim iBytesWritten As Int32
        Dim bOutBuffer(48) As Byte
        Dim bSuccess As Boolean

        bOutBuffer = oEnc.GetBytes(bToSend)
        bSuccess = WriteFile(hSerialPort, bOutBuffer, bOutBuffer.Length, iBytesWritten, IntPtr.Zero)

        If bSuccess = False Then
            Throw New CommException("Unable to write to " & Me.sPortName)
        End If
    End Sub
    ''' <summary>
    ''' Used to transmit the fonts.
    ''' </summary>
    ''' <param name="bToSend"></param>
    ''' <remarks></remarks>
    Public Sub TransmitFont(ByVal bToSend As Byte())
        Dim bytesWritten As Int32
        Dim bSuccess As Boolean

        bSuccess = WriteFile(hSerialPort, bToSend, bToSend.Length, bytesWritten, IntPtr.Zero)

        If bSuccess = False Then
            Throw New CommException("Unable to write to " & sPortName)
        End If
    End Sub
    ''' <summary>
    ''' Reads from port until some data is recieved, or timeOut is exceeded
    ''' Timeout is currently hardcoded at 3000 mS, in openPort() - now set to 500ms = 0.5s
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadFromPort() As String
        Dim iBytesRead As Int32
        Dim bBuffer(48) As Byte
        Dim strReturnVal As String
        Dim bSuccess As Boolean

        bSuccess = ReadFile(hSerialPort, bBuffer, bBuffer.Length, iBytesRead, IntPtr.Zero)
        If bSuccess = False Then
            Throw New CommException("Unable to read from " & sPortName)
        End If
        strReturnVal = oEnc.GetString(bBuffer, 0, iBytesRead)
        Return strReturnVal
    End Function
    ''' <summary>
    ''' To open the communication port for connecting to the printer.
    ''' </summary>
    ''' <param name="portName"></param>
    ''' <param name="baudRate"></param>
    ''' <param name="parity"></param>
    ''' <param name="byteSize"></param>
    ''' <param name="stopBits"></param>
    ''' <remarks></remarks>
    Private Sub OpenPort(ByVal portName As String, ByVal baudRate As Integer, ByVal parity As Byte, ByVal byteSize As Byte, ByVal stopBits As Byte)
        Dim bSuccess As Boolean
        'Obtain a handle to the serial port.
        hSerialPort = CreateFile(portName, GENERIC_READ Or GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero)

        'Verify that the obtained handle is valid.
        If hSerialPort.ToInt32 = -1 Then
            Throw New CommException("Unable to obtain a handle to the COM port")
        End If

        'Retrieve the current control settings.
        bSuccess = GetCommState(hSerialPort, MyDCB)
        If bSuccess = False Then
            Throw New CommException("Unable to retrieve the current control settings")
        End If

        'Modify the properties of MyDCB 
        MyDCB.BaudRate = baudRate
        MyDCB.ByteSize = byteSize
        MyDCB.Parity = parity
        MyDCB.StopBits = stopBits
        MyDCB.fBitFields = MyDCB.fBitFields Or fRTSControlDisable Or fDTRControlDisable

        'Reconfigure port based on the properties of MyDCB.
        bSuccess = SetCommState(hSerialPort, MyDCB)
        If bSuccess = False Then
            Throw New CommException("Unable to reconfigure COM")
        End If

        'Retrieve the current time-out settings.
        bSuccess = GetCommTimeouts(hSerialPort, MyCommTimeouts)
        If bSuccess = False Then
            Throw New CommException("Unable to retrieve current time-out settings")
        End If

        'Modify the properties of MyCommTimeouts
        MyCommTimeouts.ReadIntervalTimeout = 5
        MyCommTimeouts.ReadTotalTimeoutConstant = 500   'Was 3000 = 3 seconds
        MyCommTimeouts.ReadTotalTimeoutMultiplier = 0
        MyCommTimeouts.WriteTotalTimeoutConstant = 0
        MyCommTimeouts.WriteTotalTimeoutMultiplier = 0

        'Reconfigure the time-out settings, based on the properties of MyCommTimeouts.
        bSuccess = SetCommTimeouts(hSerialPort, MyCommTimeouts)
        If bSuccess = False Then
            Throw New CommException("Unable to reconfigure the time-out settings")
        End If

        'Flush any garbage on port
        FlushComIn()
    End Sub
    ''' <summary>
    ''' Closes the physical port, and prepares this object for destruction
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Dispose() Implements System.IDisposable.Dispose
        Static disposed As Boolean = False
        ' Release the handle to port.
        CloseHandle(hSerialPort)
        disposed = True
    End Sub
    ''' <summary>
    ''' flushes any data already received on the port
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub FlushComIn()
        Dim ExistingCommTimeouts As COMMTIMEOUTS
        Dim tempCommTimeouts As COMMTIMEOUTS
        Dim bSuccess As Boolean
        Dim iBytesRead As Int32
        Dim bBuffer(32) As Byte

        'Retrieve the current time-out settings.
        bSuccess = GetCommTimeouts(hSerialPort, ExistingCommTimeouts)

        With tempCommTimeouts
            .ReadIntervalTimeout = 1
            .WriteTotalTimeoutConstant = 0
            .WriteTotalTimeoutMultiplier = 0
            .ReadTotalTimeoutConstant = 1
        End With

        'Temporarily reconfigure the time-out settings
        bSuccess = SetCommTimeouts(hSerialPort, tempCommTimeouts)

        'Read data from port.
        bSuccess = ReadFile(hSerialPort, bBuffer, 32, iBytesRead, IntPtr.Zero)

        'Reset the time-out settings to previous values
        bSuccess = SetCommTimeouts(hSerialPort, ExistingCommTimeouts)
    End Sub
    ''' <summary>
    ''' Class to get the exceptions during the communication with the printer.
    ''' </summary>
    ''' <remarks></remarks>
    Class CommException
        Inherits ApplicationException
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="Reason"></param>
        ''' <remarks></remarks>
        Sub New(ByVal Reason As String)
            MyBase.New(Reason)
        End Sub
    End Class
End Class