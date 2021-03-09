Imports System.Runtime.InteropServices
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
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
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
            For iCounter = 0 To chArray.Length - 1
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
    ''' <param name="iInputEANVal"></param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function ValidateSEL(ByVal iInputEANVal As String) As Boolean
        Dim chArray() As Char
        Dim iProductOfEAN As Integer = 0
        Dim iCDVfromSEL As Integer

        Try
            iInputEANVal = iInputEANVal.PadLeft(12, "0")
            'Converts the string to a character array to enable parsing
            chArray = iInputEANVal.ToCharArray
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
    ''' <param name="barcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateBootsCode(ByVal barcode As String) As Boolean
        Dim result As Boolean = False
        Dim check As Int32 = 0
        'Dim treble As Boolean = False
        Dim counter As Int32 = 1
        Dim total As Int32 = 0
        Dim factor As Int32 = 7
        Dim iBtsCodeLen As Int32 = 0
        barcode = objAppContainer.objHelper.GenerateBCwithCDV(barcode)
        iBtsCodeLen = barcode.Length

        If iBtsCodeLen = 7 Then
            check = Val(Mid(barcode, 7, 1))

            While counter < 7
                total = total + (Val(Mid(barcode, counter, 1)) * factor)
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
    ''' <summary>
    ''' Gets Status description
    ''' </summary>
    ''' <param name="strStatus"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetStatusDescription(ByVal strStatus As String) As String
        Dim strStatusDescription As String = ""
        'Assigns the corresponding status description based on the type of status
        Select Case strStatus
            Case "A"
                strStatusDescription = Macros.STATUS_ACTIVE
            Case "B"
                strStatusDescription = Macros.STATUS_DELETED
            Case "X"
                strStatusDescription = Macros.STATUS_DISCONTINUED
            Case "B"
                strStatusDescription = Macros.STATUS_RECALLED
        End Select
        GetStatusDescription = strStatusDescription
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
    ''' Calculates the Boots Code based on CDV
    ''' </summary>
    ''' <param name="bootsCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GenerateBCwithCDV(ByVal bootsCode As String) As String
        Dim validBootsCode As String = bootsCode
        Dim total As Int32 = 0
        Dim counter As Int32 = 1
        Dim factor As Int32 = 7
        If Not (bootsCode.Length = 7 Or bootsCode.Length = 13) Then
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
        End If

        GenerateBCwithCDV = validBootsCode

    End Function
    ''' <summary>
    ''' Validate Authorization Id
    ''' </summary>
    ''' <param name="sAuthid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateAuthid(ByVal sAuthid As String) As Boolean
        If sAuthid.Length = Macros.AUTHMAXSIZE And Val(sAuthid) > 0 Then
            Return True
        End If
        Return False
    End Function
    ''' <summary>
    ''' Validate the scanned UOD
    ''' </summary>
    ''' <param name="sUODLabel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateUOD(ByVal sUODLabel As String, ByRef bErrorPrompt As Boolean) As Boolean
        Dim sPrefix As String = ""
        'Start
        bErrorPrompt = False
        Dim labelColour As String
        labelColour = WorkflowMgr.GetInstance().Labelcolour

        'Check if UOD has been already scanned
#If NRF Then
        For Each objUOD In objAppContainer.objUODCollection
            If Trim(objUOD.ToString) = Trim(sUODLabel) Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), _
                           "Invalid UOD", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                            MessageBoxDefaultButton.Button1)
                bErrorPrompt = True
                Return False
                Exit For
            End If
        Next
#ElseIf RF Then
        'To check the UOD is valid or not, we send a UOQ message to the controller
        If Not objAppContainer.objExportDataManager.CreateUOQ(sUODLabel) Then
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), _
                          "Invalid UOD", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                           MessageBoxDefaultButton.Button1)
            bErrorPrompt = True
            Return False
        End If
#End If

        If sUODLabel.Length <> 14 Then
            Return False
        End If
        If labelColour = "RLCOLOR" Or labelColour = "RLCOLOUR" Then
            If objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.RECALL Then

                Dim strUODLblType As String = RLSessionMgr.GetInstance().strLabelType
                Select Case strUODLblType
                    Case "01"
                        labelColour = "BLACK"
                    Case "02"
                        labelColour = "GREY"
                    Case "03"
                        labelColour = "YELLOW"
                    Case "04"
                        labelColour = "ORANGE"
                    Case "05"
                        labelColour = "RED"
                    Case "06"
                        labelColour = "WHITE"
                    Case "07"
                        labelColour = "PURPLE"
                End Select
            End If
        End If
        Select Case UCase(labelColour)
            Case "PURPLE"
                sPrefix = "00000006"
            Case "BLACK"
                sPrefix = "00009999"
            Case "GREY"
                sPrefix = "00007777"
            Case "YELLOW"
                sPrefix = "00006666"
            Case "ORANGE"
                sPrefix = "00008888"
            Case "RED"
                sPrefix = "00000083"
            Case "WHITE"
                sPrefix = "00000000"
                'Case "RLCOLOUR"     'Fix for Recall CR.
                'sPrefix = "00000006"
        End Select

        If UCase(labelColour) = "WHITE" Then
            If Mid(sUODLabel, 1, sPrefix.Length) = sPrefix Then
                Return True
            Else
                sPrefix = "00000000"
                If Mid(sUODLabel, 1, sPrefix.Length) = sPrefix Then
                    Return True
                Else
                    Return False
                End If
            End If
            'Fix for Recall CR: allowing Black and grey label.
        ElseIf UCase(labelColour) = "RLCOLOUR" Or UCase(labelColour) = "RLCOLOR" Then
            If Not objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.RECALL Then
                Dim sBlackPrefix As String = "00009999"
                Dim sGreyPrefix As String = "00007777"
                Dim sUODStart As String = Mid(sUODLabel, 1, sPrefix.Length)
                If sUODStart = sPrefix Or sUODStart = sBlackPrefix Or sUODStart = sGreyPrefix Then
                    Return True
                Else
                    Return False
                End If
            End If
            ' Chnges end.
        Else
            If Mid(sUODLabel, 1, sPrefix.Length) = sPrefix Then
                Return True
            Else
                Return False
            End If
        End If
    End Function
    ''' <summary>
    ''' Resolves the Escape sequence issue where a label doesnot recognize ampersand
    ''' </summary>
    ''' <param name="strData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FormatEscapeSequence(ByVal strData As String) As String
        Return System.Text.RegularExpressions.Regex.Replace(strData, System.Text.RegularExpressions.Regex.Escape("&"), "&&")
    End Function
    ''' <summary>
    ''' Removes the CDV from the boots code number
    ''' </summary>
    ''' <param name="strBootsCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RemoveBootsCodeCDV(ByVal strBootsCode As String) As String
        If strBootsCode.Length > 0 Then
            Return strBootsCode.Substring(0, strBootsCode.Length - 1)
        Else
            Return "000000"
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

        'Dotnet CF returns 6 as the adapter type for ethernet and wireless
        Dim uintAdapterType As System.UInt32 = 6
        Dim adapterIndex As Integer = Nothing

#If NRF Then
        adapterIndex = 1
#ElseIf RF Then
        adapterIndex = 2
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
                    MessageBox.Show("The Current Adapter Info Type:" + currAdapterInfo.Type.ToString())
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
            objAppContainer.objLogger.WriteAppLog("Helper: Getting DNS name" + _
                                                  sDnsName, Logger.LogLevel.INFO)
            m_IpHostEntry = System.Net.Dns.GetHostEntry(sDnsName)
            objAppContainer.objLogger.WriteAppLog("Helper: Getting IP Host", _
                                                  Logger.LogLevel.INFO)
            m_aIPAddressArray = m_IpHostEntry.AddressList()
            objAppContainer.objLogger.WriteAppLog("Helper: Getting IP address" + _
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
                        objAppContainer.objLogger.WriteAppLog("Helper: IP of device is" _
                                                              & strIP, _
                                                              Logger.LogLevel.RELEASE)
                    ElseIf m_aIPAddressArray(iIndex).ToString() <> "127.0.0.1" Then
                        strIP = m_aIPAddressArray(iIndex).ToString()
                        objAppContainer.objLogger.WriteAppLog("Helper: IP of device is" _
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
            objAppContainer.objLogger.WriteAppLog("Helper: Device IP retreival failure" + _
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
            objAppContainer.objLogger.WriteAppLog("Setting MCDownloader for next day run", _
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
            objAppContainer.objLogger.WriteAppLog("Helper: Free memory available in device is" _
                                                  & iFreemem, Logger.LogLevel.INFO)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Helper: Free memory available in device " _
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
    ''' Imports CeRunappTime of Coredll to register an application to run at a specific time
    ''' </summary>
    ''' <param name="AppName"></param>
    ''' <param name="ExecTime"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DllImport("CoreDLL.dll")> _
    Private Shared Function CeRunAppAtTime(ByVal AppName As String, ByVal ExecTime As IntPtr) As Boolean
    End Function
#If NRF Then
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
#End If
#If NRF Then
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
                                                ConfigKey.FILE_TYPE_STARTINDEX)
            objFile.IsType.Length = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.FILE_TYPE_LENGTH)

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
#End If

    ''' <summary>
    ''' Create the Base Barcode by replacing Price by 0's to query the database/Conroller
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetBaseBarcode(ByVal strPriceBarcode As String) As String
        Dim strTemp As String = ""
        Try
            If strPriceBarcode.StartsWith("2") Then
                'Replace vpppp with 0s in 2naaaaavppppc barcode type
                strTemp = Mid(strPriceBarcode, 1, 7)
                strTemp = strTemp + "00000"
            ElseIf strPriceBarcode.StartsWith("02") Then
                'Replace vppppp with 0s in the barcode type 02aaaavpppppc
                strTemp = Mid(strPriceBarcode, 1, 6)
                strTemp = strTemp + "000000"
            End If
        Catch ex As Exception
            Return "0"
        End Try
        Return strTemp
        'Check if the Price Barcode was of format UPCA
        ' If strPriceBarcode.Substring(0, 1) = "0" Then
        'Replace the Price in Price Barcode with 0's
        'strPriceBarcode = strPriceBarcode.Replace(strPriceBarcode.Substring(7, 5), "00000")
        ' strPriceBarcode = strPriceBarcode.Replace(strPriceBarcode.Substring(7, 6), "000000")
        ' Else
        'Replace the Price in Price Barcode with 0's
        ' strPriceBarcode = strPriceBarcode.Replace(strPriceBarcode.Substring(8, 4), "0000")
        'strPriceBarcode = strPriceBarcode.Replace(strPriceBarcode.Substring(8, 5), "00000")
        ' End If
        '  Return strPriceBarcode
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
    Public Function RemoveMultipleSpaces(ByVal strData) As String
        Dim strTemp As String = TrimSpaces(New String(strData))
        RemoveMultipleSpaces = IIf(Len(strTemp) = 0, strData, strTemp)
    End Function
    ''' <summary>
    ''' Function to Trim Spaces in a String
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function TrimSpaces(ByRef strTrimText) As String
        Dim changeon As String = Replace(strTrimText, "  ", " ")
        If Len(strTrimText) = Len(changeon) Then TrimSpaces = strTrimText : Exit Function
        TrimSpaces = RemoveMultipleSpaces(changeon)
    End Function
    ''' <summary>
    ''' Function to convert Decimal to Binary
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DecimaltoBinary(ByVal strDecVal As String) As String
        Try
            Dim iDecVal As Long = CType(strDecVal, Long)
            Dim strBinaryVal As String = ""
            Do Until iDecVal = 0
                If (iDecVal Mod 2) Then
                    strBinaryVal = "1" & strBinaryVal
                Else
                    strBinaryVal = "0" & strBinaryVal
                End If
                iDecVal = iDecVal \ 2
            Loop
            Return strBinaryVal.PadLeft(8, "0")
        Catch ex As Exception
            Return "00000000"
        End Try
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
        strAppVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
        aReleaseVersion = strAppVersion.Split("-")
        strAppVersion = ""
        strAppVersion = aReleaseVersion(1)
        strTemp = strAppVersion.Substring(0, 2)
        strFinalVersion = strFinalVersion + CInt(strTemp).ToString() + "."
        strTemp = ""
        strTemp = strAppVersion.Substring(2, 2)
        strFinalVersion = strFinalVersion + strTemp
        Return strFinalVersion
    End Function
End Class
