Imports System.Runtime.InteropServices
'Imports Symbol.ResourceCoordination
'Imports OpenNETCF.Win32
Imports System.Text
Imports System.IO
Imports System.Xml
Imports Symbol.ResourceCoordination

'''***************************************************************
''' <FileName>Helper.vb</FileName>
''' <summary>
''' This is the generic helper class. 
''' This class consists of functionalities that are used across multiple modules.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>30 Oct 2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
'''*********************************************************************
''' * Modification Log 
''' ******************************************************************** 
''' * 1.1. Mathew Jerry Thomas. Defect 68. Release 14 A 
'''  Defect 68:“Display of pricing information on Shelf Edge Label 
''' does not show currency or Unit type”
''' Uncommented "BeforeSymbol" in GenerateUnitPriceLine, to include the symbol  in unit price line
''' 


Public Class Helper
    <DllImport("IphlpApi", SetLastError:=True)> _
    Public Shared Function GetAdaptersInfo(ByVal pAdapterInfo As IntPtr, ByRef pOutBufLen As Integer) As Integer
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Structure IP_ADDR_STRING
        Private [Next] As IntPtr
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)> _
        Friend IpAddress As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=16)> _
        Friend IpMask As String
        Private Context As Integer
    End Structure '_IP_ADDR_STRING
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    <StructLayout(LayoutKind.Sequential)> Structure IP_ADAPTER_INFO
        Friend [Next] As IntPtr
        Private ComboIndex As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=260)> _
        Friend AdapterName() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=132)> _
        Friend Description() As Byte
        Friend AddressLength As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> _
        Friend Address() As Byte
        Private Index As System.UInt32
        'Private Type As System.UInt32
        Friend Type As System.UInt32
        Private DhcpEnabled As Integer
        Private CurrentIpAddress As IntPtr
        Friend IpAddressList As IP_ADDR_STRING
        Friend GatewayList As IP_ADDR_STRING
        Friend DhcpServer As IP_ADDR_STRING
        Private HaveWins As Boolean
        Friend PrimaryWinsServer As IP_ADDR_STRING
        Friend SecondaryWinsServer As IP_ADDR_STRING
        Private LeaseObtained As Integer
        Private LeaseExpires As Integer
    End Structure
    ''' <summary>
    ''' Checks Whether the Build Failed during reference data download
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function VerifyRefDataUpdation() As Boolean
        'declaring the Build Status
        Dim bIsBuildPassed As Boolean = True
        Dim xdDoc As XmlDocument
        Dim xdNode As XmlNode = Nothing
        Dim xdNodeList As XmlNodeList = Nothing
        Dim dtLastBuildTime As DateTime = New DateTime()
        Dim sTemp As String = Nothing
        Try
            If (File.Exists(Macros.REF_FILE_STATUS_FILE.ToString())) And _
               (File.Exists(Macros.MCDOWNLOADER_CONFIG.ToString())) Then
                Dim iCount As Integer = 0
                Dim xmlDocument As New Xml.XmlDocument
                Dim xmlBuildStatus As Xml.XmlElement
                xmlDocument.Load(Macros.REF_FILE_STATUS_FILE)
                xmlBuildStatus = xmlDocument.DocumentElement
                While iCount < Macros.No_REF_FILES
                    If (xmlBuildStatus.GetElementsByTagName("strBuildStatus" _
                         ).ItemOf(iCount).InnerText.ToString() <> "Y") Then
                        bIsBuildPassed = False
                        Exit While
                    End If
                    iCount = iCount + 1
                End While
                'Check if the file contains details for all files using icount and ref files count.
                If Macros.No_REF_FILES <> iCount Then
                    bIsBuildPassed = False
                End If
                'Check if BootsCode last update time is same as today's date
                xdDoc = New XmlDocument()
                xdDoc.Load(Macros.MCDOWNLOADER_CONFIG)
                xdNode = xdDoc.SelectSingleNode("ConfigParams")
                If xdNode IsNot Nothing Then
                    xdNodeList = xdNode.ChildNodes
                    For Each xmlKey As XmlNode In xdNodeList
                        If xmlKey.Name = "LastBuildBOOTCODE" Then
                            Try
                                dtLastBuildTime = DateTime.ParseExact(xmlKey.InnerText.Replace("T", " "), _
                                                                      "yyyy-MM-dd HH:mm:ss", _
                                                                      Nothing)
                            Catch ex As Exception
                                Throw ex
                            End Try
                            Exit For
                        End If
                    Next
                End If
                'Check if date is today's date or not
                If dtLastBuildTime.Date = Now.Date Then
                    bIsBuildPassed = True
                Else
                    bIsBuildPassed = False
                End If
                'return value.
                Return bIsBuildPassed
            Else
                Return False
            End If
        Catch ex As Exception
            'If any exception ocurred while trying to access a index that is not available.
            Return False
        End Try
    End Function
    ''' <summary>
    ''' This method validates whether the quantity entered 
    ''' is double digit. for eg: 11,22,33,44.
    ''' </summary>
    ''' <param name="iInputValue"></param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function ValidateDoubleDigitQuantity(ByVal iInputValue As Long) As Boolean
        Dim bDoubleDigitVal As Boolean = True
        Dim iCounter As Integer
        Dim chDataArray() As Char
        Dim strValue As String
        Try
            'Converts the integer value to string
            strValue = CStr(iInputValue)

            'Converts the string to a character array to enable parsing
            chDataArray = strValue.ToCharArray

            If chDataArray.Count = 2 Then

                'Iterate through the string using the character array
                iCounter = 0
                If (chDataArray(iCounter).Equals(chDataArray(iCounter + 1))) Then
                    bDoubleDigitVal = False
                    Return bDoubleDigitVal
                End If

            End If

        Catch exGeneralException As Exception
            Return False
        End Try

        Return bDoubleDigitVal

    End Function
    ''' <summary>
    ''' This method validates the product code that is entered.
    ''' </summary>
    ''' <param name="strProductCode"></param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function ValidateProductCode(ByVal strProductCode As String) As Boolean
        Dim bProductCodeValid As Boolean
        bProductCodeValid = False
        'Check for the product code
        'If product code is valid then return true

        Return bProductCodeValid
    End Function
    ''' <summary>
    ''' This method validates whether the quantity entered is greater than 50
    ''' </summary>
    ''' <param name="iInputValue"></param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function ValidateQtyGreaterThanFifty(ByVal iInputValue As Long) As Boolean

        'Checks if the value is greater than fifty. 
        'If so return true else return false
        If (iInputValue > 50) Then
            Return False
        Else
            Return True
        End If

    End Function
    ''' <summary>
    ''' This method validates whether the quantity entered is zero
    ''' </summary>
    ''' <param name="iInputValue"></param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function ValidateZeroQty(ByVal iInputValue As Long) As Boolean
        'Checks if the input value is equal to zero
        'If its equal to zero then return true else false
        If (iInputValue = 0) Then
            Return False
        Else
            Return True
        End If
    End Function
    ''' <summary>
    ''' Validates the CDV for EAN
    ''' </summary>
    ''' <param name="strInputEANVal"></param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function ValidateEAN(ByVal strInputEANVal As String) As Boolean
        Dim chArray() As Char
        Dim iProductOfEAN As Integer = 0
        Dim iCDVfromEAN As Integer

        Try
            strInputEANVal = strInputEANVal.PadLeft(13, "0")
            'Converts the string to a character array to enable parsing
            chArray = strInputEANVal.ToCharArray
            iCDVfromEAN = (Microsoft.VisualBasic.Val(chArray(chArray.Length - 1)))
            'Iterate through the string using the character array
            For iCounter = 0 To chArray.Length - 2
                'If (iCounter = 12) Then
                '    Exit For
                'End If
                'If the index position is 0, then assign the value multiplied by 3 to the variable
                If (iCounter = 0) Then
                    iProductOfEAN = Microsoft.VisualBasic.Val(chArray(iCounter)) * 1
                    'If the index position is odd, then add the value multiplied by 1 to the variable
                ElseIf (iCounter Mod 2 <> 0) Then
                    iProductOfEAN = iProductOfEAN + (Microsoft.VisualBasic.Val(chArray(iCounter)) * 3)
                    'If the index position is even, then add the value multiplied by 3 to the variable
                ElseIf (iCounter <> 0 And iCounter Mod 2 = 0) Then
                    iProductOfEAN = iProductOfEAN + (Microsoft.VisualBasic.Val(chArray(iCounter)) * 1)
                End If
            Next

            'Divide the sum of products by 10
            Dim iRemainderProductOfEAN As Integer
            iRemainderProductOfEAN = iProductOfEAN Mod 10

            'Subtract the remainder from 10
            Dim iObtainedCDVVal As Integer
            iObtainedCDVVal = 10 - iRemainderProductOfEAN

            'If the value obtained after subtraction is 10, then CDV is 0
            If (iObtainedCDVVal = 10) Then
                iObtainedCDVVal = 0
            End If

            If (iCDVfromEAN = iObtainedCDVVal) Then
                Return True
            End If
        Catch ex As Exception

        End Try

        Return False
    End Function
    ''' <summary>
    ''' Validates the CDV for SEL
    ''' </summary>
    ''' <param name="strInputEANVal"></param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function ValidateSEL(ByVal strInputEANVal As String) As Boolean
        Dim chArray() As Char
        Dim iProductOfEAN As Integer = 0
        Dim iCDVfromSEL As Integer

        Try
            strInputEANVal = strInputEANVal.PadLeft(12, "0")
            'Converts the string to a character array to enable parsing
            chArray = strInputEANVal.ToCharArray
            iCDVfromSEL = (Microsoft.VisualBasic.Val(chArray(chArray.Length - 1)))
            'Iterate through the string using the character array
            For iCounter = 0 To chArray.Length - 1
                If (iCounter = 11) Then
                    Exit For
                End If
                'If the index position is 0, then assign the value multiplied by 3 to the variable
                If (iCounter = 0) Then
                    iProductOfEAN = Microsoft.VisualBasic.Val(chArray(iCounter)) * 1
                    'If the index position is odd, then add the value multiplied by 1 to the variable
                ElseIf (iCounter Mod 2 <> 0) Then
                    iProductOfEAN = iProductOfEAN + (Microsoft.VisualBasic.Val(chArray(iCounter)) * 3)
                    'If the index position is even, then add the value multiplied by 3 to the variable
                ElseIf (iCounter <> 0 And iCounter Mod 2 = 0) Then
                    iProductOfEAN = iProductOfEAN + (Microsoft.VisualBasic.Val(chArray(iCounter)) * 1)
                End If
            Next

            'Divide the sum of products by 10
            Dim iRemainderProductOfEAN As Integer
            iRemainderProductOfEAN = iProductOfEAN Mod 10

            'Subtract the remainder from 10
            Dim iObtainedCDVVal As Integer
            iObtainedCDVVal = 10 - iRemainderProductOfEAN

            'If the value obtained after subtraction is 10, then CDV is 0
            If (iObtainedCDVVal = 10) Then
                iObtainedCDVVal = 0
            End If

            If (iCDVfromSEL = iObtainedCDVVal) Then
                Return True
            End If
        Catch ex As Exception

        End Try

        Return False
    End Function
    ''' <summary>
    ''' validates the CDV for Boots Code
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateBootsCode(ByVal strBarcode As String) As Boolean
        Dim result As Boolean = False
        Dim check As Int32 = 0
        'Dim treble As Boolean = False
        Dim counter As Int32 = 1
        Dim total As Int32 = 0
        Dim factor As Int32 = 7
        Dim iBtsCodeLen As Int32 = 0

        iBtsCodeLen = strBarcode.Length

        If iBtsCodeLen = 7 Then
            check = Val(Mid(strBarcode, 7, 1))

            While counter < 7
                total = total + (Val(Mid(strBarcode, counter, 1)) * factor)
                counter = counter + 1
                factor = factor - 1
            End While

            total = 11 - (total Mod 11)
            If total > 9 Then
                total = 0
            End If

            If total = check Then
                result = True
            End If
        End If
        ValidateBootsCode = result

    End Function
    ''' <summary>
    ''' To parse the clearance barcode scanned.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseClearanceBarcode(ByVal strBarcode As String, ByRef strBootsCode As String, ByRef strClrPrice As String, ByRef strOrignalPrice As String) As Boolean
        Try
            strBootsCode = Mid(strBarcode, 5, 6)
            strClrPrice = Mid(strBarcode, 11, 5)
            If strBarcode.Length = 22 Then
                strOrignalPrice = Mid(strBarcode, 16, 6)
            Else
                strOrignalPrice = "0"
            End If
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' To parse the clearance barcode scanned.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetBaseBarcode(ByVal strBarcode As String) As String
        Dim strTemp As String = ""
        Try
            If strBarcode.StartsWith("2") Then
                'Replace vpppp with 0s in 2naaaaavppppc barcode type
                strTemp = Mid(strBarcode, 1, 7)
                'strPrice = Mid(strBarcode, 9, 4)
                strTemp = strTemp + "00000"
            ElseIf strBarcode.StartsWith("02") Then
                'Replace vppppp with 0s in the barcode type 02aaaavpppppc
                strTemp = Mid(strBarcode, 1, 6)
                'strPrice = Mid(strBarcode, 8, 5)
                strTemp = strTemp + "000000"
            End If
        Catch ex As Exception
            Return "0"
        End Try
        Return strTemp
    End Function
    ''' <summary>
    ''' To parse the clearance barcode scanned.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetBaseBarcodeWithPrice(ByVal strBarcode As String, ByRef strPrice As String, ByRef strBaseCode As String) As Boolean
        Dim strTemp As String = ""
        Try
            If strBarcode.StartsWith("2") Then
                'Replace vpppp with 0s in 2naaaaavppppc barcode type
                strTemp = Mid(strBarcode, 1, 7)
                strPrice = Mid(strBarcode, 9, 4)
                strBaseCode = strTemp + "00000"
            ElseIf strBarcode.StartsWith("02") Then
                'Replace vppppp with 0s in the barcode type 02aaaavpppppc
                strTemp = Mid(strBarcode, 1, 6)
                strPrice = Mid(strBarcode, 8, 5)
                strBaseCode = strTemp + "000000"
            End If
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' Formats the barcode
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Function FormatBarcode(ByVal strBarcode As String) As String
        Dim strFormattedCode As String = ""
        Try

            'Finds the barcode length and formats it accordingly
            Select Case strBarcode.Length
                Case 7
                    strFormattedCode = Mid(strBarcode, 1, 2) & _
                    "-" & Mid(strBarcode, 3, 2) & _
                    "-" & Mid(strBarcode, 5, 3)
                Case 8
                    strFormattedCode = Mid(strBarcode, 1, 4) & _
                    "-" & Mid(strBarcode, 5, 4)

                Case 9
                    strFormattedCode = Mid(strBarcode, 1, 5) & _
                    "-" & Mid(strBarcode, 6, 4)

                Case 10
                    strFormattedCode = Mid(strBarcode, 1, 5) & _
                    "-" & Mid(strBarcode, 6, 5)

                Case 11
                    strFormattedCode = Mid(strBarcode, 1, 5) & _
                    "-" & Mid(strBarcode, 6, 6)

                Case 12
                    strFormattedCode = Mid(strBarcode, 1, 6) & _
                    "-" & Mid(strBarcode, 7, 6)

                Case 13
                    strFormattedCode = Mid(strBarcode, 1, 1) & _
                    "-" & Mid(strBarcode, 2, 6) & _
                    "-" & Mid(strBarcode, 8, 6)
            End Select
        Catch ex As Exception

        End Try

        Return strFormattedCode
    End Function
    ''' <summary>
    ''' Unformats the formatted bar code
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Function UnFormatBarcode(ByVal strBarcode As String) As String
        Dim strUnformattedCode As String = strBarcode
        Try
            'Removes the formatting from the string
            While strUnformattedCode.LastIndexOf("-") > -1
                strUnformattedCode = strUnformattedCode.Remove(strUnformattedCode.LastIndexOf("-"), 1)
            End While
        Catch ex As Exception

        End Try
        'Returns the unformatted string
        Return strUnformattedCode
    End Function
    ''' <summary>
    ''' Retrives the Boots Code from SEL
    ''' </summary>
    ''' <param name="strSEL"></param>
    ''' <param name="strBootsCode"></param>
    ''' <returns>True on Success</returns>
    ''' <remarks></remarks>
    Public Function GetBootsCodeFromSEL(ByVal strSEL As String, ByRef strBootsCode As String) As Boolean
        Try
            'First Six digits of SEL is Boots Code
            strBootsCode = strSEL.Substring(0, 6)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    'Integration testing
    ''' <summary>
    ''' Function to format the currency to be displayed
    ''' </summary>
    ''' <param name="strPrice"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FormatPrice(ByVal strPrice As String) As String
        Dim formattedMoney As String = strPrice

        formattedMoney = Val(formattedMoney).ToString()

        If formattedMoney.Length > 2 Then
            formattedMoney = Left(formattedMoney, Len(formattedMoney) - 2) + "." + Right(formattedMoney, 2)
        ElseIf formattedMoney.Length = 2 Then
            formattedMoney = "0" + "." + formattedMoney
        Else
            formattedMoney = "0" + "." + formattedMoney.PadLeft(2, "0")
        End If

        FormatPrice = formattedMoney

    End Function
    ''' <summary>
    ''' Retrieves the Price from SEL
    ''' </summary>
    ''' <param name="strSEL"></param>
    ''' <param name="strPrice"></param>
    ''' <returns>True on success</returns>
    ''' <remarks></remarks>
    Public Function GetPriceFromSEL(ByVal strSEL As String, ByRef strPrice As String) As Boolean
        Try
            'Five digits in SEL after Boots Code is Price
            strPrice = strSEL.Substring(6, 5)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function GetStatusDescription(ByVal strStatus As String) As String
        Dim strStatusDescription As String = ""
        'Assigns the corresponding status description based on the type of status
        Select Case strStatus
            Case "A"
                strStatusDescription = Macros.STATUS_ACTIVE
            Case " "
                strStatusDescription = Macros.STATUS_ACTIVE
            Case "B"
                strStatusDescription = Macros.STATUS_DISCONTINUED
            Case "C"
                strStatusDescription = Macros.STATUS_OUTSTANDING_CANCEL
            Case "D"
                strStatusDescription = Macros.STATUS_DISCONTINUED
            Case "P"
                strStatusDescription = Macros.STATUS_SUSPENDED
            Case "U"
                strStatusDescription = Macros.STATUS_UNSUPPLIABLE
            Case "X"
                strStatusDescription = Macros.STATUS_DELETED
            Case "Z"
                strStatusDescription = Macros.STATUS_DELETED
        End Select
        GetStatusDescription = strStatusDescription
    End Function
    ''' <summary>
    ''' Function to get advantage remeeable flag from item status bit flag.
    ''' </summary>
    ''' <param name="strItemStatus3"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRedeemableFlag(ByVal strItemStatus3 As String) As String
        Dim iRedemption As Integer
        Dim iCheckBit As Integer = 4
        Try
            iRedemption = CType(strItemStatus3, Integer)
            If (iRedemption And iCheckBit) <> 0 Then
                Return "*"
            Else
                Return " "
            End If
        Catch ex As Exception
            Return " "
        End Try
    End Function
    ''' <summary>
    ''' Formats the description to split into three lines of 15 characters each.
    ''' </summary>
    ''' <param name="strData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFormattedDescription(ByVal strData As String) As ArrayList
        Dim iNumSet As Integer = 0
        Dim strDataArray As ArrayList = New ArrayList()
        Try
            iNumSet = strData.Length / 15
            Dim iNumMod As Integer = 0
            iNumMod = strData.Length Mod 15

            If iNumSet > 3 Then
                iNumSet = 3
                iNumMod = 0
            End If

            'If the reminder is non zero then approximate to the next highest integer
            If iNumMod >= 8 Then
                iNumSet = iNumSet - 1
            End If

            'Checks if the string length is less than 15
            If iNumSet = 0 Then
                strDataArray.Add(FormatEscapeSequence(strData))
                Dim iCount As Integer = 0
                For iCount = strDataArray.Count To 3
                    strDataArray.Add("")
                Next
                Return strDataArray
            End If

            Dim iCounter As Integer = 0
            Dim iFirstIndex As Integer = 0
            Dim iLastIndex As Integer = 0

            While iCounter < iNumSet
                iFirstIndex = iCounter * 15
                iLastIndex = 15

                strDataArray.Add(FormatEscapeSequence(strData.Substring(iFirstIndex, iLastIndex)))

                iCounter = iCounter + 1
            End While

            If iCounter < 3 Then
                If iNumMod <> 0 Then
                    iFirstIndex = iCounter * 15
                    strDataArray.Add(FormatEscapeSequence(strData.Substring(iFirstIndex, iNumMod)))
                End If
            End If

            If strDataArray.Count < 3 Then
                Dim iCount As Integer = 0
                For iCount = strDataArray.Count To 3
                    strDataArray.Add("")
                Next
            End If

            Return strDataArray
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    ''' <summary>
    ''' Formats the regular expression case of ampersand
    ''' </summary>
    ''' <param name="strData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FormatEscapeSequence(ByVal strData As String) As String
        'Formats the string data by adding && to handle single & condition
        If Not strData.Equals("") Then
            Return System.Text.RegularExpressions.Regex.Replace(strData, System.Text.RegularExpressions.Regex.Escape("&"), "&&")
        Else
            Return strData
        End If
    End Function
    ''' <summary>
    ''' Calculates the Boots Code based on CDV
    ''' </summary>
    ''' <param name="bootsCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GenerateBCwithCDV(ByVal bootsCode As String) As String
        Dim validBootsCode As String = ""
        Dim total As Int32 = 0
        Dim counter As Int32 = 1
        Dim factor As Int32 = 7
        If bootsCode.Length < 7 Then
            While counter < 7
                total = total + (Val(Mid(bootsCode, counter, 1)) * factor)
                counter = counter + 1
                factor = factor - 1
            End While
            total = 11 - (total Mod 11)
            If total > 9 Then
                total = 0
            End If
            validBootsCode = bootsCode & total
            GenerateBCwithCDV = validBootsCode
        Else
            GenerateBCwithCDV = bootsCode
        End If
    End Function
    ''' <summary>
    ''' Calculates the CDV and appends to the product code
    ''' </summary>
    ''' <param name="strBarCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneratePCwithCDV(ByVal strBarCode As String) As String
        Dim validProductCode As String = ""
        Dim iProductOfEAN As Integer = 0
        Dim chArray() As Char
        Dim strCheckVal As String = ""
        strCheckVal = strBarCode
        'Converts the string to a character array to enable parsing
        chArray = strCheckVal.ToCharArray
        'Check if length of the barcode is 13 which is in case of EAN is with CDV then just return the same.
        If strBarCode.Length = 13 Then
            Return strBarCode
        End If
        'Iterate through the string using the character array
        For iCounter = 0 To chArray.Length - 1
            'If the index position is 0, then assign the value multiplied by 3 to the variable
            If (iCounter = 0) Then
                iProductOfEAN = Microsoft.VisualBasic.Val(chArray(iCounter)) * 1
                'If the index position is odd, then add the value multiplied by 1 to the variable
            ElseIf (iCounter Mod 2 <> 0) Then
                iProductOfEAN = iProductOfEAN + (Microsoft.VisualBasic.Val(chArray(iCounter)) * 3)
                'If the index position is even, then add the value multiplied by 3 to the variable
            ElseIf (iCounter <> 0 And iCounter Mod 2 = 0) Then
                iProductOfEAN = iProductOfEAN + (Microsoft.VisualBasic.Val(chArray(iCounter)) * 1)
            End If
        Next

        'Divide the sum of products by 10
        Dim iRemainderProductOfEAN As Integer
        iRemainderProductOfEAN = iProductOfEAN Mod 10

        'Subtract the remainder from 10
        Dim iObtainedCDVVal As Integer
        iObtainedCDVVal = 10 - iRemainderProductOfEAN

        'If the value obtained after subtraction is 10, then CDV is 0
        If (iObtainedCDVVal = 10) Then
            iObtainedCDVVal = 0
        End If

        validProductCode = strBarCode & iObtainedCDVVal
        validProductCode = validProductCode.PadLeft(13, "0")
        GeneratePCwithCDV = validProductCode

    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="string_"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function StringToDateConversion(ByVal string_ As String) As DateTime
        Dim strToConvert As String 'String Variable
        Dim dateResult As DateTime  'DateTime variable

        strToConvert = ""           'Just initialize string to null
        dateResult = Nothing        'Just initialize date to nothing
        'strToConvert = "Thu, Feb 03 2005 10:50 am"
        strToConvert = string_
        Try
            dateResult = FormatDateTime(strToConvert, DateFormat.ShortDate)
        Catch ex As Exception
            'Check if there is some exception here
            Return Nothing
        Finally
            'and finally destroy objects if any..
        End Try
        Return dateResult
    End Function
    ''' <summary>
    ''' Gets the MAC address of the device to be sent in SOR
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMacAddress() As String

        Dim structSize As Int32 = Marshal.SizeOf(GetType(IP_ADAPTER_INFO))
        Dim pArray As IntPtr = Marshal.AllocHGlobal(structSize)
        Dim len As UInt64 = Convert.ToUInt64(structSize)
        Dim ret As Int32 = GetAdaptersInfo(pArray, len)
        Dim strMacAddress As String = "000000000000"
        Dim setppc As Boolean = False
#If RF Then
        'Check the OS version to identify between PPC and MC55 device.

        If Not Environment.OSVersion.Version.ToString().StartsWith("5.2.") Then
            setppc = True
        End If

#End If

        'Dotnet CF returns 6 as the adapter type for ethernet and wireless
        Dim uintAdapterType As System.UInt32 = 6
        Dim adapterIndex As Integer = Nothing

#If NRF Then
        adapterIndex = 1
#ElseIf RF Then
        If setppc = True Then
            'For PPC adapter index will be 1
            adapterIndex = 1
        Else
            'For MC55RF the adapter index will be 2
            adapterIndex = 2
        End If
#End If
        Try
            If ret = 111 Then
                ' Buffer was too small, reallocate the correct size for the buffer.
                pArray = Marshal.ReAllocHGlobal(pArray, New IntPtr(Convert.ToInt64(len)))
                ret = GetAdaptersInfo(pArray, len)
            End If

            If ret = 0 Then
                Dim pAdapterInfo As IntPtr = pArray
                Dim loopIndex As Integer = 1

                Do
                    ' Retrieve the adapter info from the memory address.
                    Dim currAdapterInfo As IP_ADAPTER_INFO = _
                    CType(Marshal.PtrToStructure(pAdapterInfo, GetType(IP_ADAPTER_INFO)), IP_ADAPTER_INFO)
                    'Check for valid adapter type - ethernet/wireless
                    If currAdapterInfo.Type = uintAdapterType And _
                        loopIndex = adapterIndex Then
                        ' Loop through and extract MAC address
                        Dim StrMac As String = Nothing
                        For iLoop As Integer = 0 To 5
                            StrMac = StrMac & Hex(currAdapterInfo.Address(iLoop)).PadLeft(2, "0")
                        Next
                        strMacAddress = StrMac.Trim
                        Marshal.FreeHGlobal(pArray)
                        Return strMacAddress
                    End If
                    loopIndex = loopIndex + 1
                    pAdapterInfo = currAdapterInfo.Next
                Loop Until IntPtr.op_Equality(pAdapterInfo, IntPtr.Zero)
            End If
        Catch ex As Exception
            If Not objAppContainer.objHelper Is Nothing Then
                objAppContainer.objLogger.WriteAppLog("Device MAC retreival failure :" + _
                ex.Message & ex.StackTrace, Logger.LogLevel.RELEASE)
            End If
            ' Always return something...
            Marshal.FreeHGlobal(pArray)
            Return strMacAddress
        End Try

        ' Always return something...
        Marshal.FreeHGlobal(pArray)
        objAppContainer.objLogger.WriteAppLog("MAC address empty." _
                                              , Logger.LogLevel.RELEASE)

        Return strMacAddress
    End Function
    ''' <summary>
    ''' Function to get device serial number.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSerialNumber() As String
#If NRF Then
        Try
            Dim objTerminalInfo As TerminalInfo = New TerminalInfo()
            'objAppContainer.objLogger.WriteAppLog("Serial Number of Device : " + _
            '                             objTerminalInfo.ESN.ToString(), Logger.LogLevel.RELEASE)
            If objTerminalInfo.ESN.Length > 12 Then
                Return objTerminalInfo.ESN.Substring(objTerminalInfo.ESN.Length - 12, 12).ToString()
            Else
                Return objTerminalInfo.ESN.ToString()
            End If
        Catch ex As Exception
            'objAppContainer.objLogger.WriteAppLog("Device Serial Number retreival failed" + _
            '                          ex.StackTrace, Logger.LogLevel.RELEASE)
            Return "000000000000"
        End Try
#Else
        Return "000000000000"
#End If
    End Function
    ''' <summary>
    ''' To check for the dynamic IP generated when the device is docked
    ''' </summary>
    ''' <remarks></remarks>
    Public Function GetIPAddress() As String
        'Declare the local variable and the get the host name
        Dim sDnsName As String = Nothing
        Dim m_IpHostEntry As System.Net.IPHostEntry = Nothing
        Dim m_aIPAddressArray As System.Net.IPAddress() = Nothing
        Dim strIP As String = ""
        Dim iIndex As Integer = 0
        Dim aIPSubnet() As String = Nothing
        Try
            sDnsName = System.Net.Dns.GetHostName()
            objAppContainer.objLogger.WriteAppLog("Getting DNS name" + _
                                                  sDnsName, Logger.LogLevel.INFO)
            m_IpHostEntry = System.Net.Dns.GetHostEntry(sDnsName)
            objAppContainer.objLogger.WriteAppLog("Getting IP Host", _
                                                  Logger.LogLevel.INFO)
            m_aIPAddressArray = m_IpHostEntry.AddressList()
            objAppContainer.objLogger.WriteAppLog("Getting IP address" + _
                                                  m_IpHostEntry.AddressList(0).ToString(), _
                                                  Logger.LogLevel.INFO)
            ' Check if the address array has a default value
            If m_aIPAddressArray.Length > 0 Then
                ' Check within a loop whether the IP is else then 127.0.0.1
                For iIndex = 0 To m_aIPAddressArray.Length - 1
                    ' If the address IP is else then convert it into string 
                    If m_aIPAddressArray(iIndex).ToString() = "127.0.0.1" And _
                       m_aIPAddressArray.Length = 1 Then
                        strIP = m_aIPAddressArray(iIndex).ToString()
                        objAppContainer.objLogger.WriteAppLog("IP of device is" _
                                                              & strIP, _
                                                              Logger.LogLevel.RELEASE)
                    ElseIf m_aIPAddressArray(iIndex).ToString() <> "127.0.0.1" Then
                        strIP = m_aIPAddressArray(iIndex).ToString()
                        objAppContainer.objLogger.WriteAppLog("IP of device is" _
                                                              & strIP, _
                                                              Logger.LogLevel.RELEASE)
                    End If
                Next
                ' Return the new IP generated when the device is docked into 
                'the(cradle)
                'format the IP address to have 3 digits in all the three subnets
                aIPSubnet = strIP.Split(".")
                aIPSubnet(0) = aIPSubnet(0).PadLeft(3, "0")
                aIPSubnet(1) = aIPSubnet(1).PadLeft(3, "0")
                aIPSubnet(2) = aIPSubnet(2).PadLeft(3, "0")
                aIPSubnet(3) = aIPSubnet(3).PadLeft(3, "0")
                strIP = aIPSubnet(0) & "." & aIPSubnet(1) & "." & _
                           aIPSubnet(2) & "." & aIPSubnet(3)

                'returnt he IP address to the calling function
                Return strIP
            Else
                'Return the default IP of the device when the device is 
                'not docked into the cradle
                Return "127.000.000.001"
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Device IP retreival failure" + _
                                      ex.StackTrace, Logger.LogLevel.RELEASE)
            Return "127.000.000.001"
        End Try
    End Function
    Public Function PurgeFiles(ByVal strDirectoryPath As String) As Boolean
        Try
            Dim straFileEntries As String() = Directory.GetFiles(strDirectoryPath)
            For Each strFileName As String In straFileEntries
                File.Delete(strFileName)
            Next
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' To check for free memory in the device
    ''' </summary>
    ''' <param name="folder"></param>
    ''' <param name="iFreemem"></param>
    ''' <remarks>The folder of which the free memory needs to be calculated</remarks>
    Public Function CheckForFreeMemory(ByVal folder As String, ByRef iFreemem As Long) As String
        'Declare the local variables
        Dim folderName As String = folder
        Dim freeBytesAvailableToCaller As UInteger = 0
        Dim totalNumberOfBytes As UInteger = 0
        Dim totalNumberOfFreeBytes As UInteger = 0
        Try
            'Call GetDiskFreeSpaceEx for getting the free memory space available 
            'in the specified device
            GetDiskFreeSpaceEx(folderName, freeBytesAvailableToCaller, _
                               totalNumberOfBytes, totalNumberOfFreeBytes)
            Dim totalFreeMemoryinMB As Long = totalNumberOfFreeBytes / (1024 * 1024)
            iFreemem = totalFreeMemoryinMB * 8
            objAppContainer.objLogger.WriteAppLog("Free memory available in device is" _
                                                  & iFreemem, Logger.LogLevel.INFO)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Free memory available in device " _
                                                & "calculation failure" & iFreemem, _
                                                Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return iFreemem
    End Function
    ''' <summary>
    ''' To get the Disk free space using system DLL
    ''' </summary>
    ''' <param name="directory"></param>
    ''' <param name="lpFreeBytesAvailableToCaller"></param>
    ''' <param name="lpTotalNumberOfBytes"></param>
    ''' <param name="lpTotalNumberOfFreeBytes"></param>
    ''' <remarks></remarks>
    <DllImport("coredll.dll")> _
    Public Shared Function GetDiskFreeSpaceEx(ByVal directory As String, ByRef lpFreeBytesAvailableToCaller As Integer, ByRef lpTotalNumberOfBytes As Integer, ByRef lpTotalNumberOfFreeBytes As Integer) As Boolean
    End Function
    ''' <summary>
    ''' To Update the file status for active files and the export 
    ''' data file present in the device.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateFileStatus(ByVal cName As String, ByVal cStatus As String, ByVal cException As String) As Boolean
        Dim strFileName As String = Nothing
        Dim bIsStatusSet As Boolean = False
        'xml document object
        Dim xd As New XmlDocument
        Dim newAtt As XmlAttribute
        Try
            strFileName = Macros.ACT_FILE_STATUS_FILE.ToString()
            If System.IO.File.Exists(strFileName) Then
                'load the xml file
                xd.Load(strFileName)
                Dim xNode As XmlNode = xd.DocumentElement.SelectSingleNode("/filestatus")
                'Get the entire list of nodes.
                For Each xdNode As Xml.XmlNode In xNode.ChildNodes
                    If xdNode.Attributes.GetNamedItem("name").Value.ToString().Equals(cName) Then
                        xdNode.Attributes.GetNamedItem("status").Value = cStatus
                        xdNode.Attributes.GetNamedItem("exception").Value = cException
                        bIsStatusSet = True
                    End If
                Next

                'If the node request is not available, create a node and append to the
                'xml document xd.
                If bIsStatusSet = False Then
                    'Generate all the child nodes with argument name, status and exception.
                    Dim xdNode As XmlNode = xd.CreateElement("file")
                    'Create name attribute
                    newAtt = xd.CreateAttribute("name")
                    newAtt.Value = cName
                    xdNode.Attributes.Append(newAtt)
                    'Create status attribute
                    newAtt = xd.CreateAttribute("status")
                    newAtt.Value = cStatus
                    xdNode.Attributes.Append(newAtt)
                    'Create exception attribute
                    newAtt = xd.CreateAttribute("exception")
                    newAtt.Value = cException
                    xdNode.Attributes.Append(newAtt)

                    'look for the appsettings node
                    Dim xdRoot As XmlNode = xd.DocumentElement.SelectSingleNode("/filestatus")

                    'add the new child node (this key)
                    If Not xdRoot Is Nothing Then
                        xdRoot.AppendChild(xdNode)
                    Else
                        CreateActiveAndExDataStatusFile(cName, cStatus, cException)
                    End If
                End If

                'finally, save the new version of the config file
                xd.Save(strFileName)
            Else
                'Create the file
                CreateActiveAndExDataStatusFile(cName, cStatus, cException)
            End If
            UpdateFileStatus = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Could not set the file status", _
                                                  Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    ''' Creates the XML file to store the parsing detail for active file and 
    ''' download details for export data.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateActiveAndExDataStatusFile(ByVal cName As String, ByVal cStatus As String, ByVal cException As String) As Boolean
        Dim xmlDoc As XmlDocument
        'Dim xmlTxtWriter As XmlTextWriter
        Dim newAtt As XmlAttribute
        'Declare local variables
        Dim strFileName As String = Nothing
        Dim strControlFilePath As String = Nothing
        Dim strControlFileName As String = Nothing
        Dim strControlFile As String = Nothing
        Dim cIsType As Char = Nothing
        Dim srCtrlFile As StreamReader = Nothing
        Dim isFileNotPresent As Boolean = True
        Dim iTemp As Integer = 0
        'Instantiate the File class
        Dim objFile As New ControllerFile

        Try
            xmlDoc = New XmlDocument()
            'xmlTxtWriter = New XmlTextWriter(Macros.ACT_FILE_STATUS_FILE.ToString(), _
            '                                 System.Text.Encoding.Default)
            ' Use the XmlDeclaration class to place the
            ' <?xml version="1.0"?> declaration at the top of our XML file
            Dim xmlDec As XmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", _
                                             Nothing, Nothing)
            xmlDoc.AppendChild(xmlDec)
            Dim DocRoot As XmlElement = xmlDoc.CreateElement("filestatus")
            xmlDoc.AppendChild(DocRoot)

            'Read the path of control file 
            strControlFilePath = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.LOCAL_PATH)
            strControlFileName = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.CONTROL_FILE_NAME)
            strControlFile = strControlFilePath + strControlFileName

            Dim str As String = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.FILENAME_FIELD_START_INDEX)

            'Gather the details about the Sync Conrol File      
            objFile.Name = strControlFile
            objFile.FileNameField.StartIndex = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.FILENAME_FIELD_START_INDEX)
            objFile.FileNameField.Length = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.FILENAME_FIELD_LENGTH)
            objFile.IsType.StartIndex = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.IS_TYPE_STARTINDEX)
            objFile.IsType.Length = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.IS_TYPE_LENGTH)

            'Declare a variable for reading a line fromm sync control file
            Dim strLine As String = Nothing
            'Instantiate the stream reader class
            srCtrlFile = New StreamReader(strControlFile)
            'Read line by line and process the file details
            Do While srCtrlFile.Peek() > -1
                'Read the file line by line
                strLine = srCtrlFile.ReadLine()
                'Read the type of file
                cIsType = strLine.Substring(objFile.IsType.StartIndex, _
                                            objFile.IsType.Length)
                'Get the file name for whom the status is "E"
                strFileName = strLine.Substring( _
                                objFile.FileNameField.StartIndex, _
                                objFile.FileNameField.Length).Trim()
                'If the file is of type Active file and status of the file is End
                If cIsType = "A" And strFileName <> cName Then
                    'Generate all the child nodes with argument name, status and exception.
                    Dim fileDetail As XmlNode = xmlDoc.CreateElement("File")
                    'Create name attribute
                    newAtt = xmlDoc.CreateAttribute("name")
                    newAtt.Value = strFileName
                    fileDetail.Attributes.Append(newAtt)
                    'Create status attribute
                    newAtt = xmlDoc.CreateAttribute("status")
                    newAtt.Value = "X"
                    fileDetail.Attributes.Append(newAtt)
                    'Create exception attribute
                    newAtt = xmlDoc.CreateAttribute("exception")
                    newAtt.Value = "NA"
                    fileDetail.Attributes.Append(newAtt)
                    'Append the key to the document.
                    DocRoot.AppendChild(fileDetail)
                ElseIf cIsType = "A" And strFileName = cName Then
                    'Generate all the child nodes with argument name, status and exception.
                    Dim fileDetail As XmlNode = xmlDoc.CreateElement("File")
                    'Create name attribute
                    newAtt = xmlDoc.CreateAttribute("name")
                    newAtt.Value = strFileName
                    fileDetail.Attributes.Append(newAtt)
                    'Create status attribute
                    newAtt = xmlDoc.CreateAttribute("status")
                    newAtt.Value = cStatus
                    fileDetail.Attributes.Append(newAtt)
                    'Create exception attribute
                    newAtt = xmlDoc.CreateAttribute("exception")
                    newAtt.Value = cException
                    fileDetail.Attributes.Append(newAtt)
                    'Append the key to the document.
                    DocRoot.AppendChild(fileDetail)
                End If
            Loop

            'Create the XML file after creating all the nodes.
            'xmlDoc.WriteContentTo(xmlTxtWriter)
            xmlDoc.Save(Macros.ACT_FILE_STATUS_FILE)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("ActiveFileParser - " _
                                                  & "Exception in CreateActiveAndExDataStatusFile" + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
        Finally
            'Close the streamreader object and dispose it 
            srCtrlFile.Close()
            srCtrlFile.Dispose()
            xmlDoc = Nothing
        End Try
    End Function
    ''' <summary>
    ''' This function is to register the downloader run time everytime we log off from
    ''' the application
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RegisterDownloader() As Boolean
        Dim strFirstInvokeTime As String = Nothing
        Dim strAppName As String = Nothing
        Dim bReturn As Boolean = False
        Dim tySystemTime As SystemTime
        Dim intptrTime As IntPtr
        Dim m_dtFirstInvokeToday As DateTime
        Dim xdDoc As XmlDocument
        Dim xdNode As XmlNode = Nothing
        Dim xdNodeList As XmlNodeList = Nothing
        Try
            xdDoc = New XmlDocument()
            xdDoc.Load(Macros.MCDOWNLOADER_CONFIG)
            xdNode = xdDoc.SelectSingleNode("ConfigParams")
            If xdNode IsNot Nothing Then
                xdNodeList = xdNode.ChildNodes
                For Each xmlKey As XmlNode In xdNodeList
                    If xmlKey.Name = "AppName" Then
                        strAppName = xmlKey.InnerText.ToString()
                    ElseIf xmlKey.Name = "FirstInvokeTime" Then
                        strFirstInvokeTime = xmlKey.InnerText.ToString()
                    End If
                    If strAppName <> Nothing And strFirstInvokeTime <> Nothing Then
                        Exit For
                    End If
                Next
            End If

            m_dtFirstInvokeToday = New DateTime(Today.Year, Today.Month, _
                                                Today.Day, _
                                                Hour(strFirstInvokeTime), _
                                                Minute(strFirstInvokeTime), 0)
            m_dtFirstInvokeToday = m_dtFirstInvokeToday.AddDays(1)
            tySystemTime = ConvertDateTime.FromDateTime(m_dtFirstInvokeToday)
            intptrTime = Marshal.AllocHGlobal(Marshal.SizeOf(tySystemTime))
            Marshal.StructureToPtr(tySystemTime, intptrTime, False)
            objAppContainer.objLogger.WriteAppLog("Setting MCDownloader for next day run at " & m_dtFirstInvokeToday.ToString(), _
                                                  Logger.LogLevel.RELEASE)
            'register the application to run
            Dim bTemp As Boolean = CeRunAppAtTime(strAppName, intptrTime)

            objAppContainer.objLogger.WriteAppLog("Setting MCDownloader returned " & bTemp.ToString(), _
                                                  Logger.LogLevel.RELEASE)
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("RegistryDownloader " _
                                                  & "Exception in RegistryDownloader" + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    'Lakshmi 
    ''' <summary>
    ''' To Format the sales value
    ''' </summary>
    ''' <param name="strSalesValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FormatSalesValue(ByVal strSalesValue As String) As String
        Dim iDelimPos As Integer
        strSalesValue = strSalesValue.TrimStart("0")
        If strSalesValue = "" Then
            strSalesValue = 0
        End If
        iDelimPos = strSalesValue.Length - 3
        While iDelimPos >= 1
            strSalesValue = strSalesValue.Insert(iDelimPos, ",")
            iDelimPos = iDelimPos - 2
        End While
        Return strSalesValue
    End Function
    'End-Lakshmi

    ''' <summary>
    ''' Imports CeRunappTime of Coredll to register an application to run at a specific time
    ''' </summary>
    ''' <param name="AppName"></param>
    ''' <param name="ExecTime"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DllImport("CoreDLL.dll")> _
    Private Shared Function CeRunAppAtTime(ByVal AppName As String, ByVal ExecTime As IntPtr) As Boolean
    End Function
    ''' <summary>
    ''' Function to format the price of the item.
    ''' </summary>
    ''' <param name="rawMoney"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function FormatMoney(ByVal rawMoney As String) As String
        Dim formattedMoney As String = rawMoney
        Try
            'Remove leading zeros
            While formattedMoney.StartsWith("0") And formattedMoney.Length <> 0
                formattedMoney = formattedMoney.Remove(0, 1)
            End While
            Select Case formattedMoney.Length
                Case 0
                    formattedMoney = "0.00"
                Case 1
                    formattedMoney = formattedMoney
                Case 2
                    formattedMoney = formattedMoney
                Case 3
                    formattedMoney = Mid(formattedMoney, 1, 1) & "." & Mid(formattedMoney, 2, 2)
                Case 4
                    formattedMoney = Mid(formattedMoney, 1, 2) & "." & Mid(formattedMoney, 3, 2)
                Case 5
                    formattedMoney = Mid(formattedMoney, 1, 3) & "." & Mid(formattedMoney, 4, 2)
                Case 6
                    formattedMoney = Mid(formattedMoney, 1, 1) & "," & Mid(formattedMoney, 2, 3) & _
                                     "." & Mid(formattedMoney, 5, 2)
                Case 7
                    formattedMoney = Mid(formattedMoney, 1, 2) & "," & Mid(formattedMoney, 3, 3) & _
                                     "." & Mid(formattedMoney, 6, 2)
                Case 8
                    formattedMoney = Mid(formattedMoney, 1, 3) & "," & Mid(formattedMoney, 4, 3) & _
                                     "." & Mid(formattedMoney, 7, 2)
                Case 9
                    formattedMoney = Mid(formattedMoney, 1, 1) & "," & Mid(formattedMoney, 2, 3) & _
                                     "," & Mid(formattedMoney, 5, 3) & "." & Mid(formattedMoney, 8, 2)
                Case 10
                    formattedMoney = Mid(formattedMoney, 1, 2) & "," & Mid(formattedMoney, 3, 3) & _
                                     "," & Mid(formattedMoney, 6, 3) & "." & Mid(formattedMoney, 9, 2)
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in Format money" & ex.StackTrace & ex.Message, _
                                                  Logger.LogLevel.RELEASE)
        End Try
        FormatMoney = formattedMoney
    End Function
    ''' <summary>
    ''' Format quantity
    ''' </summary>
    ''' <param name="strQuantity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function FormatQuantity(ByVal strQuantity As String) As String
        Dim strFormattedQuantity As String = strQuantity
        Try
            While strFormattedQuantity.StartsWith("0") And strFormattedQuantity.Length <> 1
                strFormattedQuantity = strFormattedQuantity.Remove(0, 1)
            End While
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in Format quantity" & ex.StackTrace & ex.Message, _
                                                  Logger.LogLevel.RELEASE)
        End Try
        FormatQuantity = strFormattedQuantity
    End Function
    ''' <summary>
    ''' Format the item price to be printed on SEL.
    ''' </summary>
    ''' <param name="strItemPrice"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function FormatItemPrice(ByVal strItemPrice As String) As String
        Dim iAmount As Integer
        Dim RetString As String = ""
        Try
            iAmount = Val(strItemPrice)
            RetString = iAmount.ToString
            If RetString.Length > 2 Then
                RetString = Left(RetString, Len(RetString) - 2) + "." + Right(RetString, 2)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in Format price" & ex.StackTrace & ex.Message, _
                                                  Logger.LogLevel.RELEASE)
        End Try
        'return formatted price
        FormatItemPrice = RetString
    End Function
    ''' <summary>
    ''' Format Supply Route
    ''' </summary>
    ''' <param name="SupplyRoute"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FormatSupplyRoute(ByVal SupplyRoute As String) As String
        If SupplyRoute <> "E" And SupplyRoute <> "W" And SupplyRoute <> "C" Then
            SupplyRoute = " "
        End If

        If SupplyRoute = "W" Then 'Change Warehouse identifier
            SupplyRoute = "E"
        End If
        'Set supply route.
        FormatSupplyRoute = SupplyRoute
    End Function
    ''' <summary>
    ''' 'Returns Unit Price value which will be formatted later eg. 42.9p per 100ml or €18.00 per Kg
    ''' </summary>
    ''' <param name="UnitPriceFlag"></param>
    ''' <param name="SELPrice"></param>
    ''' <param name="UnitMeasure"></param>
    ''' <param name="UnitQty"></param>
    ''' <param name="UnitType"></param>
    ''' <param name="CurrencySymbol"></param>
    ''' <param name="MajorCurrencySymbolToPrint"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GenerateUnitPriceLine(ByVal UnitPriceFlag As String, _
                                                 ByVal SELPrice As String, _
                                                 ByVal UnitMeasure As String, _
                                                 ByVal UnitQty As String, _
                                                 ByVal UnitType As String, _
                                                 ByVal CurrencySymbol As String, _
                                                 ByRef MajorCurrencySymbolToPrint As String)
        Dim iUnitMeasure As Integer
        Dim iUnitPrice As Integer
        Dim iItemQty As Integer
        Dim iSELPrice As Integer
        Dim BeforeSymbol As String
        Dim AfterSymbol As String
        Dim FormattedPrice As String
        Dim MinorCurrencySymbolToPrint As String
        Dim strUM As String
        Dim bROIgTokgMlToLitreConversion As Boolean

        GenerateUnitPriceLine = ""
        Try

            iUnitMeasure = Val(UnitMeasure)
            iItemQty = Val(UnitQty) / 1000
            iSELPrice = Val(SELPrice)

            bROIgTokgMlToLitreConversion = False

            If CurrencySymbol = "E" Then
                ' Force units to Litres or Kgs if ROI store
                UnitType = Trim(UnitType)
                If UnitType = "ml" Then
                    'iItemQty /= 1000
                    UnitType = "Litre"
                ElseIf UnitType = "g" Then
                    'iItemQty /= 1000
                    UnitType = "Kg"
                End If
                If UnitType = "Litre" Or UnitType = "Kg" Then
                    'UnitMeasure = "1"
                    iUnitMeasure = 1
                    bROIgTokgMlToLitreConversion = True
                End If
            End If

            iUnitPrice = CalcUnitPrice(iSELPrice, iItemQty, iUnitMeasure, bROIgTokgMlToLitreConversion)

            MajorCurrencySymbolToPrint = "€"
            MinorCurrencySymbolToPrint = "c"
            If CurrencySymbol <> "E" Then 'Default to Sterling
                MajorCurrencySymbolToPrint = "£"
                MinorCurrencySymbolToPrint = "p"
            End If

            'Format price
            'Note that unit price formatting is slightly more complicated, as
            'we may require an extra decimal place for amounts less than 100.
            '        £1.00 per litre
            '        99.3p per litre
            '         0.1p per litre
            '         €1.1 per litre (for currencies with no minor units)
            '
            'If unit price is larger than £1.00, display normally


            If iUnitPrice > 999 Then
                BeforeSymbol = MajorCurrencySymbolToPrint
                AfterSymbol = ""
                iUnitPrice /= 10
                FormattedPrice = FormatDP(iUnitPrice, 2)
            Else ' Price is < £1.00
                BeforeSymbol = ""
                MajorCurrencySymbolToPrint = ""
                AfterSymbol = MinorCurrencySymbolToPrint
                FormattedPrice = FormatDP(iUnitPrice, 1)
            End If

            If iUnitMeasure = 1 Then 'Don't display "per 1 Unit"
                strUM = " "
            Else
                strUM = " " & iUnitMeasure.ToString & " "
            End If
           
            ''GenerateUnitPriceLine = BeforeSymbol & FormattedPrice & AfterSymbol + _  'PUT BACK IF FIND FONT THAT PRINTS £/€

            'Start 1.1
            'Uncommented "BeforeSymbol" variable to include the symbol  in unit price line
            GenerateUnitPriceLine = BeforeSymbol + " " + FormattedPrice + " " + AfterSymbol + _
                                    " per" + _
                                    strUM + _
                                    Trim(UnitType)
            'End 1.1

        Catch ex As Exception
            'MessageBox.Show(ex.Message)
        End Try
    End Function
    ''' <summary>
    ''' Function  :  CALC.UNIT.PRICE (converted from PSBF10)
    ''' Purpose   :  Calculates the unit price with 1 implied decimal place
    ''' </summary>
    ''' <param name="Price"></param>
    ''' <param name="Qty"></param>
    ''' <param name="Unit"></param>
    ''' <param name="bROIconversion"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CalcUnitPrice(ByVal Price As Integer, _
                                         ByVal Qty As Integer, _
                                         ByVal Unit As Integer, _
                                         ByVal bROIconversion As Boolean) As Integer
        Dim RealPrice As Double
        Dim RealQty As Double
        Dim IntegerPrice As Integer

        RealQty = Qty

        If bROIconversion Then 'Converts ROI items from g to Kg and ml to Litres
            RealQty /= 1000
        End If

        'Special case for zero priced items, they are allowed 0 unit price ;)
        If Price = 0 Then
            IntegerPrice = 0
        Else
            RealPrice = (Price * 10 * Unit) / RealQty
            'MsgBox("RealPrice:" & RealPrice & " Price:" & Price & " Unit:" & Unit & " Qty:" & Qty & " RealQty:" & RealQty) 'XXX DEBUG
            If Math.Abs(RealPrice) >= 1000 Then 'Round(UnitPrice, -1, 0)
                RealPrice += 0.5 'Force 0.5 to round up
                RealPrice /= 10

                IntegerPrice = Math.Round(RealPrice, 1)
                IntegerPrice *= 10
            Else
                IntegerPrice = Math.Round(RealPrice, 1) 'Unit = Round(UnitPrice, 0, 0)
            End If
            'Special case, unit prices must not be zero unless price is 0
            If IntegerPrice = 0 Then
                IntegerPrice = 1
            End If
        End If
        'Return unit price with one implied decimal place
        CalcUnitPrice = IntegerPrice
    End Function
    ''' <summary>
    ''' Function  :  FormatDP (converted from PSBF10)
    ''' Purpose   :  Formats an amount with the required decimal places
    ''' </summary>
    ''' <param name="Amount"></param>
    ''' <param name="DP"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function FormatDP(ByVal Amount As Integer, ByVal DP As Integer) As String
        Dim FormatString As String
        FormatString = Amount.ToString
        If DP = 2 Then ' Format as major currency
            FormatString = Left(FormatString, Len(FormatString) - DP) & "." & Right(FormatString, DP)
        Else ' Format as minor currency DP=1
            If Amount Mod 10 = 0 Then 'Lose the decimal point and zero eg. convert 52.0p to 52p
                Amount /= 10
                FormatString = Amount.ToString
            Else
                If Len(FormatString) = 1 Then 'Precede decimal point with a zero ie .2p = 0.2p
                    FormatString = "0" & "." & FormatString
                Else
                    FormatString = Left(FormatString, Len(FormatString) - DP) & "." & Right(FormatString, DP)
                End If
            End If
        End If

        FormatDP = FormatString

    End Function
    ''' <summary>
    ''' Function to get the currency charancter
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCurrency() As String
        Dim strRet As String = ""
        Try
            Dim strTemp As String = ConfigDataMgr.GetInstance.GetParam(ConfigKey.CURRENCY_KEY)
            If strTemp.Equals("S") Then
                strRet = Macros.POUND_SYMBOL
            ElseIf strTemp.Equals("E") Then
                strRet = Macros.EURO_SYMBOL
            End If
        Catch ex As Exception
            strRet = Macros.POUND_SYMBOL
        End Try
        GetCurrency = strRet
    End Function
    ''' <summary>
    ''' Function to get the Stock File Header
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetStockFileHeader() As String
        
        Dim strLastActBuildTime As String = Nothing
        Dim strActiveDataTime As DateTime = Nothing
        Dim strtempActiveDataTime As String = Nothing
        Dim strStockFigureHeader As String = Nothing

#If RF Then
                strStockFigureHeader = "Total Stock File:"
#ElseIf NRF Then
        strLastActBuildTime = ConfigDataMgr.GetInstance.GetParam(ConfigKey.LAST_ACTBUILD_TIME)
        strActiveDataTime = DateTime.ParseExact(strLastActBuildTime, "yyyy-MM-dd HH:mm:ss", Nothing)
        strtempActiveDataTime = strActiveDataTime.Hour.ToString().PadLeft(2, "0") + ":" + _
                            strActiveDataTime.Minute.ToString().PadLeft(2, "0")
        strStockFigureHeader = ""
        strStockFigureHeader = "Total Stock File at " + strtempActiveDataTime + "  :"
#End If
        Return strStockFigureHeader
    End Function
    ''' <summary>
    ''' Function to format the release version from the config file
    ''' </summary>
    ''' <returns>
    ''' The formatted string to display in splash screen
    ''' </returns>
    ''' <remarks></remarks>
    Public Function GetReleaseVersion() As String
        Dim aReleaseVersion() As String = Nothing
        Dim strAppVersion As String = Nothing
        Dim strFinalVersion As String = "RCL v"
        Dim strTemp As String = Nothing
        Try
            strAppVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
            aReleaseVersion = strAppVersion.Split("-")
            strAppVersion = ""
            strAppVersion = aReleaseVersion(1)
            strTemp = strAppVersion.Substring(0, 2)
            strFinalVersion = strFinalVersion + CInt(strTemp).ToString() + "."
            strTemp = ""
            strTemp = strAppVersion.Substring(2, 2)
            strFinalVersion = strFinalVersion + strTemp
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in formatting release version", Logger.LogLevel.RELEASE)
        End Try
        Return strFinalVersion
    End Function
End Class
