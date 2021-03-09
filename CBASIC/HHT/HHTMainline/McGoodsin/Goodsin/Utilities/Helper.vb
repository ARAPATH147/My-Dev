Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Xml
Imports System.Text
Imports Symbol.ResourceCoordination
Imports Goodsin.GIValueHolder

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
Public Class Helper
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

            If chDataArray.Length = 2 Then

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
            If strInputEANVal = "0" Then
                Return False

            End If
            strInputEANVal = strInputEANVal.PadLeft(13, "0")
            'Converts the string to a character array to enable parsing
            chArray = strInputEANVal.ToCharArray
            iCDVfromEAN = (Microsoft.VisualBasic.Val(chArray(chArray.Length - 1)))
            'Iterate through the string using the character array
            For iCounter As Integer = 0 To chArray.Length - 1
                If (iCounter = 12) Then
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
            For iCounter As Integer = 0 To chArray.Length - 1
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
    ''' Formats the regular expression case of '&'
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

        'Iterate through the string using the character array
        For iCounter As Integer = 0 To chArray.Length - 1
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
    ''' String to Date Conversion
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
    ''' Validates the IST barcode
    ''' </summary>
    ''' <param name="strISTBarcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateISTCode(ByVal strISTBarcode As String, ByRef strISTCode As String) As Boolean
        If strISTBarcode.Length = 14 AndAlso strISTBarcode.Substring(0, 4) = "0000" _
        AndAlso strISTBarcode.Substring(4, 4) = "8888" Then
            strISTCode = strISTBarcode.Substring(4, 10)
            Return True
        End If
        Return False
    End Function
    ''' <summary>
    ''' Validates the CDV for UOD barcode
    ''' </summary>
    ''' <param name="strUODNumber"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateUODBarcode(ByVal strUODNumber As String, ByRef strBarcode As String) As Boolean
        Dim iUODlength As Integer = strUODNumber.Length
        If iUODlength = 12 AndAlso CheckDigitValidation(strUODNumber) Then
            strBarcode = strUODNumber.Substring(0, 10)
            Return True
        End If
        Return False
    End Function
    ''' <summary>
    ''' Validates the ASN barcode
    ''' </summary>
    ''' <param name="strASNBarcode"></param>
    ''' <param name="objASNCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateASNBarcode(ByVal strASNBarcode As String, ByRef objASNCode As ASNCode) As Boolean
        objASNCode = New ASNCode
        If strASNBarcode.Length = 18 Then
            With objASNCode
                .SupplierNumber = strASNBarcode.Substring(0, 6)
                .CartonNumber = strASNBarcode.Substring(6, 8)
                .NoOfCartons = strASNBarcode.Substring(14, 4)
            End With
            Return True
        Else
            Return False
        End If
    End Function

    Public Function ValidateDallasUODBarcode(ByVal strUODBarcode As String, ByRef strDalBarcode As String) As Boolean
        If strUODBarcode.Length = 14 AndAlso strUODBarcode.Substring(0, 4) = "0501" Then
            strDalBarcode = strUODBarcode
            Return True
        End If
        Return False
    End Function

    ''' <summary>
    ''' Validates the Driver Badge Number
    ''' </summary>
    ''' <param name="strDriverBadgeId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateDriverBadgeID(ByVal strDriverBadgeId As String) As Boolean
        If strDriverBadgeId.Length = 8 AndAlso CheckDigitValidation(strDriverBadgeId) Then
            Return True
        End If
        Return False
    End Function
    ''' <summary>
    ''' Generic function to set the column width for all list view controls.
    ''' </summary>
    ''' <param name="lstViewControl"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SetColumnWidth(ByRef lstViewControl As ListView) As Boolean
        Try
            'Dim iTotalColumns As Integer = lstViewControl.Columns.Count
            For Each ctrlHeader As Windows.Forms.ColumnHeader In lstViewControl.Columns
                ctrlHeader.Width = ctrlHeader.Width * objAppContainer.iOffSet
            Next
        Catch ex As Exception

        End Try
    End Function
    ''' <summary>
    ''' <summary>
    ''' Create the Base Barcode by replacing Price by 0's to query the database/Conroller
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetBaseBarcode(ByVal strPriceBarcode As String) As String
        'Check if the Price Barcode was of format UPCA
        If strPriceBarcode.StartsWith("02") Then
            'Replace the Price in Price Barcode with 0's
            strPriceBarcode = strPriceBarcode.Replace(strPriceBarcode.Substring(7, 5), "00000")
        ElseIf strPriceBarcode.StartsWith("2") Then
            'Replace the Price in Price Barcode with 0's
            strPriceBarcode = strPriceBarcode.Replace(strPriceBarcode.Substring(8, 4), "0000")
        End If
        Return strPriceBarcode
    End Function
    ''' Check digit validation for Barcodes
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckDigitValidation(ByVal strBarcode As String) As Boolean
        Dim chArray() As Char
        Dim iProductOfBarcode As Integer = 0
        Dim iCDVfromBarcode As Integer
        'Converts the string to a character array to enable parsing
        chArray = strBarcode.ToCharArray
        iCDVfromBarcode = (Microsoft.VisualBasic.Val(chArray(chArray.Length - 1)))
        'Iterate through the string using the character array
        For iCounter As Integer = 0 To chArray.Length - 1
            If (iCounter = (chArray.Length - 1)) Then
                Exit For
            End If
            'If the index position is 0, then assign the value multiplied by 3 to the variable
            If (iCounter = 0) Then
                iProductOfBarcode = Microsoft.VisualBasic.Val(chArray(iCounter)) * 3
                'If the index position is odd, then add the value multiplied by 1 to the variable
            ElseIf (iCounter Mod 2 <> 0) Then
                iProductOfBarcode = iProductOfBarcode + (Microsoft.VisualBasic.Val(chArray(iCounter)) * 1)
                'If the index position is even, then add the value multiplied by 3 to the variable
            ElseIf (iCounter <> 0 And iCounter Mod 2 = 0) Then
                iProductOfBarcode = iProductOfBarcode + (Microsoft.VisualBasic.Val(chArray(iCounter)) * 3)
            End If
        Next

        'Divide the sum of products by 10
        Dim iRemainderProductOfBarcode As Integer
        iRemainderProductOfBarcode = iProductOfBarcode Mod 10

        'Subtract the remainder from 10
        Dim iObtainedCDVVal As Integer
        iObtainedCDVVal = 10 - iRemainderProductOfBarcode

        'If the value obtained after subtraction is 10, then CDV is 0
        If (iObtainedCDVVal = 10) Then
            iObtainedCDVVal = 0
        End If

        If (iCDVfromBarcode = iObtainedCDVVal) Then
            Return True
        End If
        Return False
    End Function
    Public Function ConvertToDate(ByVal strDate As String) As DateTime
        Dim theDate As DateTime
        theDate = New DateTime(2000 + Convert.ToInt32(strDate.Substring(0, 2)), strDate.Substring(2, 2), strDate.Substring(4, 2), 0, 0, 0)
        Return theDate
    End Function
#If RF Then
        Public Function ValidateLogFile(ByVal strFileName As String) As Boolean
        Try
            Dim strPath As String = ConfigDataMgr.GetParam("LogFilePath")
            Dim fileList() As String = IO.Directory.GetFiles(strPath)
            Dim dtCreationDate As Date
            Dim dtPreviousDays As Date = Now.AddDays(-2)
            If Directory.Exists(strPath) Then
                For Each objFile As String In fileList
                    dtCreationDate = File.GetCreationTime(objFile)
                    If dtCreationDate < dtPreviousDays Then
                        File.Delete(objFile)
                    End If
                Next
            End If
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Cannot delete Log File.", Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
#End If
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
#Region "NRF"
#If NRF Then


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

            'register the application to run
            CeRunAppAtTime(Macros.MCDOWNLOADER_CONFIG, intptrTime)
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("RegistryDownloader " _
                                                  & "Exception in RegistryDownloader" + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
#End If
#End Region
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