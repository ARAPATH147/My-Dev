''' <summary>
''' This business class manages all data specific to the actions on the calc pad during a calc pad session
''' </summary>
''' <remarks></remarks>
''' * Modification Log
''' **********************************************************************
''' * 1.1   Archana Chandramathi    13 C Chilled Food Changes
''' set the MAX_QUANTITY to 99 in case of clearance label printing
''' and also Add validation for double digit and Quantity greater than 50
''' **********************************************************************/
Public Class CalcPadSessionMgr
    Private Shared m_CalcPadSessionMgr As CalcPadSessionMgr = Nothing
    Private MAX_QUANTITY As Integer = 999
    Private MIN_QUANTITY As Integer = 0
    Private IsValidated As Boolean = False
    Public Enum EntryTypeEnum
        Quantity = 0
        UOD = 1
        Barcode = 2
        PrintQuantity = 3
        ClearancePrice = 4
        TSF = 5
    End Enum
    'Public variables
    Public cEntryType As String
    Public strAvailableData As Boolean
    ''' <summary>
    ''' Functions for getting the object instance for the CalcPadSessionMgr. 
    ''' Use this method to get the object refernce for the Singleton CalcPadSessionMgr.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of CalcPadSessionMgr Class</remarks>
    Public Shared Function GetInstance() As CalcPadSessionMgr
        If m_CalcPadSessionMgr Is Nothing Then
            m_CalcPadSessionMgr = New CalcPadSessionMgr()
            Return m_CalcPadSessionMgr
        Else
            Return m_CalcPadSessionMgr
        End If

    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property GetSetEntryType() As String
        Get
            Return cEntryType
        End Get
        Set(ByVal value As String)
            cEntryType = value
        End Set
    End Property
    ''' <summary>
    ''' Processes the delete button click
    ''' </summary>
    ''' <param name="tbValue"></param>
    ''' <remarks></remarks>
    Public Sub ProcessDelete(ByRef tbValue As TextBox)

        If Len(tbValue.Text) > 0 Then
            '  If tbValue.Text.Substring(Len(tbValue.Text) - 1) <> " " Then
            tbValue.Text = Me.SWLeft(tbValue.Text, Len(tbValue.Text) - 1)
            'Else
            '    tbValue.Text = Me.SWLeft(tbValue.Text, Len(tbValue.Text) - 3)
            ' End If

        End If

    End Sub
    ''' <summary>
    ''' Processes the OK button click
    ''' </summary>
    ''' <param name="tbValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessOK(ByRef tbValue As Control) As Boolean
        Dim bIsValid As Boolean = True
        IsValidated = False
        'Checks the case to do applicable processing
        Select Case cEntryType
            Case EntryTypeEnum.Quantity
                MAX_QUANTITY = 999
                MIN_QUANTITY = 0
                bIsValid = ProcessQuantityEntry(tbValue)
            Case EntryTypeEnum.Barcode
                bIsValid = ProcessBarcodeEntry(tbValue)
            Case EntryTypeEnum.UOD
                bIsValid = ProcessUODEntry(tbValue)
            Case EntryTypeEnum.PrintQuantity
                MAX_QUANTITY = 9
                '1.1 set the MAX_QUANTITY to 99
                'Archana Chandramathi
                '13C Chilled Food project 
                If objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PRTCLEARANCE Then
                    MAX_QUANTITY = 99
                End If
                MIN_QUANTITY = 1
                bIsValid = ProcessQuantityEntry(tbValue)
            Case EntryTypeEnum.ClearancePrice
                bIsValid = ProcessPriceEntry(tbValue)
        End Select
        Return bIsValid
    End Function
    ''' <summary>
    ''' Processes the quantity entered
    ''' </summary>
    ''' <param name="tbValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessUODEntry(ByRef tbValue As Control) As Boolean

        Try
            If (Len(tbValue.Text.Trim) = 14) Then
                Return True
            Else
                MessageBox.Show("Invalid input:" + vbCr + "A UOD should be a 14 Digit Value", "UOD Entry - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show("Invalid input:" + vbCr + "A UOD should be a 14 Digit Value", "UOD Entry - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            Return False
        End Try


    End Function
    ''' <summary>
    ''' Processes the string for calculation
    ''' </summary>
    ''' <param name="tbValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ProcessTextEntry(ByVal tbValue As String) As String
        Try
            Dim tempstring() As String
            Dim index As Integer = 0
            tempstring = tbValue.Split("+")
            While (index + 1 < tempstring.Length)
                If (index = 0) Then
                    tbValue = tempstring(index) + " + " + tempstring(index + 1)
                Else
                    tbValue = tbValue + " + " + tempstring(index + 1)
                End If

                index += 1
            End While
            index = 0
            tempstring = tbValue.Split("-")
            While (index + 1 < tempstring.Length)
                If (index = 0) Then
                    tbValue = tempstring(index) + " - " + tempstring(index + 1)
                Else
                    tbValue = tbValue + " - " + tempstring(index + 1)
                End If
                index += 1
            End While
            index = 0
            tempstring = tbValue.Split("x")
            While (index + 1 < tempstring.Length)
                If (index = 0) Then
                    tbValue = tempstring(index) + " x " + tempstring(index + 1)
                Else
                    tbValue = tbValue + " x " + tempstring(index + 1)
                End If
                index += 1
            End While
        Catch ex As Exception
            Return Nothing
        End Try

        Return tbValue
    End Function
    ''' <summary>
    ''' Processes the quantity entered
    ''' </summary>
    ''' <param name="tbValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ProcessQuantityEntry(ByRef tbValue As Control) As Boolean
        'Define variables
        Dim iResult As Int64 = 0     'running total
        Dim cResultAsString As String = ""
        Dim iValue1 As Int64 = 0     '1st numeric value
        Dim iValue2 As Int64 = 0     '2nd numeric value
        Dim cElement As String       'holds current math function
        Dim cSymbol As String = "+"  'stores + or - 
        'Internal -   Calc pad warning message for 1+11 
        Dim iLastElement As String = "" 'holds the last entered number for double digit check

        Dim cTemp As String
        cTemp = ProcessTextEntry(tbValue.Text)

        Dim objArray As String() = cTemp.Split(" ")
        Dim iIndex As Int32 = 0  'keep track of position in array 
        Dim bIsValidEntry As Boolean = True

        Try
            'Checks if the data entered is of length one
            If objArray.Length = 1 Then
                cResultAsString = objArray.GetValue(iIndex)
                'If cResultAsString.Equals("+") Or cResultAsString.Equals("-") Or cResultAsString.Equals("x") Then
                '    '  MessageBox.Show(MessageManager.GetInstance().GetMessage("M20"))
                '    Return False
                'End If
                iResult = Val(cResultAsString)
                If cEntryType = EntryTypeEnum.Quantity Then
                    If iResult > MAX_QUANTITY Then
                        MessageBox.Show("Maximum Quantity allowed is " + MAX_QUANTITY.ToString + vbCr + "Please Verify.", "Quantity Entry - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        Return False
                    Else
                        If (Not ValidateGreaterThanFifty(iResult)) Or (Not ValidateDoubleDigitEntry(iResult)) Then
                            Return False
                        End If
                    End If

                ElseIf cEntryType = EntryTypeEnum.PrintQuantity Then
                    If iResult > MAX_QUANTITY Then
                        If objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PRINTSEL Then            'Defect  BTCPR00003816 
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M22"), _
                                            "Quantity Entry - Error", MessageBoxButtons.OK, _
                                            MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)        'Defect  BTCPR00003816
                        ElseIf objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PRTCLEARANCE Then    'Defect  BTCPR00003816
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M87"), _
                                            "Quantity Entry - Error", MessageBoxButtons.OK, _
                                            MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)        'Defect  BTCPR00003816
                        End If
                        Return False
                        'iResult = 1
                    ElseIf iResult < MIN_QUANTITY Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M1"), _
                                                           "Quantity Entry - Error", _
                                                           MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                                                           MessageBoxDefaultButton.Button1)
                        Return False
                        'iResult = 1
                        '1.1 Add validation for double digit and Qty greater than 50
                        'Archana Chandramathi
                        '13C Chilled Food project 
                    ElseIf objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PRTCLEARANCE Then
                        If (Not ValidateDoubleDigitEntry(iResult)) Or (Not ValidateGreaterThanFifty(iResult)) Then
                            Return False
                        End If
                    End If
                End If
                tbValue.Text = CStr(iResult.ToString())

            Else
                'Internal   Calc pad warning message for 1+11 
                'Compute double digit check for the last element
                iLastElement = objArray.Last()
                If ValidateDoubleDigitEntry(iLastElement) Then

                    iValue1 = Val(objArray.GetValue(iIndex))
                    Dim iFirstVal As Integer = 0

                    iFirstVal = Val(objArray.GetValue(iIndex))

                    'Does the validation for first val if it is not passed from form and is user entered
                    'If strAvailableData.Equals("") Then
                    '    bIsValidEntry = ValidateEntry(iFirstVal)
                    '    If Not bIsValidEntry Then
                    '        Return False
                    '    End If
                    'End If

                    'Checks if there are more elements after the element ie +,-,x
                    If iIndex + 2 = objArray.Length Then
                        ' MessageBox.Show(MessageManager.GetInstance().GetMessage("M20"))
                        MessageBox.Show("Invalid Input", "Quantity Entry - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        Return False
                    End If

                    While iIndex < objArray.Length - 2
                        'Identifies the element
                        cElement = objArray.GetValue(iIndex + 1)

                        'Retrieves the next element from array
                        Dim strValue2 As String = ""
                        strValue2 = objArray.GetValue(iIndex + 2).ToString()

                        'Checks if the next value is + or - or x
                        If strValue2.Equals("+") Or strValue2.Equals("-") Or strValue2.Equals("x") Or strValue2.Equals("") Then
                            'MessageBox.Show(MessageManager.GetInstance().GetMessage("M20"))
                            MessageBox.Show("Invalid Input", "Quantity Entry - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            Return False
                        Else
                            'Obtains the next value as integer
                            iValue2 = objArray.GetValue(iIndex + 2)

                            'Validates the quantity
                            'bIsValidEntry = ValidateEntry(iValue2)

                        End If

                        'Returns false if the entry is not valid
                        If Not bIsValidEntry Then
                            MessageBox.Show("Invalid Input", "Quantity Entry - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            Return False
                        End If

                        'Does the calculation based on the element ie + or - or x
                        If cElement = "x" Then
                            iValue1 = iValue1 * iValue2
                        ElseIf cElement = "+" Or cElement = "-" Then
                            If cSymbol = "+" Then
                                iResult = iResult + iValue1
                            Else
                                iResult = iResult - iValue1
                            End If
                            cSymbol = cElement
                            iValue1 = iValue2
                        End If
                        iIndex = iIndex + 2

                    End While

                    If cSymbol = "+" Then
                        iResult = iResult + iValue1
                    Else
                        iResult = iResult - iValue1
                    End If

                    If iResult < MIN_QUANTITY Then
                        If cEntryType = EntryTypeEnum.PrintQuantity Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M1"), _
                                                                "Quantity Entry - Error", _
                                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                                                                MessageBoxDefaultButton.Button1)
                            ' iResult = 1
                            Return False
                        Else
                            MessageBox.Show("Minimum Quantity allowed is " + MIN_QUANTITY.ToString + vbCr + "Please Verify.", "Quantity Entry - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            '  MessageBox.Show(MessageManager.GetInstance().GetMessage("M1"))
                            Return False
                        End If

                    ElseIf iResult > MAX_QUANTITY Then
                        '1.1 Add message 
                        'Archana Chandramathi
                        '13C Chilled Food project
                        If objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PRTCLEARANCE Then    'Defect  BTCPR00003816
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M87"), _
                                            "Quantity Entry - Error", MessageBoxButtons.OK, _
                                            MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)        'Defect  BTCPR00003816
                            Return False
                        End If

                        If cEntryType = EntryTypeEnum.PrintQuantity Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M22"), _
                                                                "Quantity Entry - Error", _
                                                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                                                                MessageBoxDefaultButton.Button1)
                            Return False
                            'iResult = 1
                        Else
                            MessageBox.Show("Maximum Quantity allowed is " + MAX_QUANTITY.ToString + vbCr + "Please Verify.", "Quantity Entry - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            Return False
                        End If
                        ElseIf (Not ValidateGreaterThanFifty(iResult)) Then
                            Return False
                        End If
                    'resultAsString = result.ToString()
                    tbValue.Text = CStr(iResult.ToString())
                Else
                    Return False
                End If
            End If

            ' Persist this value to the calling form
            If bIsValidEntry Then
                tbValue.Text = CStr(iResult.ToString()) ' Persist this value to the calling form
            End If

        Catch ex As Exception
            MessageBox.Show("Invalid Expression Entered", "Quantity Entry - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            iResult = 0
            Return False
        End Try

        Return bIsValidEntry
    End Function
    ''' <summary>
    ''' Processes the Barcode entered
    ''' </summary>
    ''' <param name="tbValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ProcessBarcodeEntry(ByRef tbValue As Control) As Boolean
        Dim strData As String = ""
        Try
            strData = tbValue.Text.Trim()
            If (strData.Length < 6) Or (Val(strData) < 100000) Then
                MessageBox.Show("Invalid Barcode Entered", "Barcode Entry - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                Return False
            Else
                Return True
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show("Invalid Barcode Entered", "Barcode Entry - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            Return False
        End Try

    End Function
    '**********Govindh Nov 09*** Changes for including clearance label printing
    ''' <summary>
    ''' Processes the Price entered
    ''' </summary>
    ''' <param name="tbValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ProcessPriceEntry(ByRef tbValue As Control) As Boolean
        Dim iCPrice As Integer = 0
        Dim iCurrentPrice As Integer = 0
        Try
            iCPrice = Convert.ToInt32(tbValue.Text.Trim())
            iCurrentPrice = CLRSessionMgr.GetInstance.GetCurrentPrice()

            If iCPrice = 0 Then
                MessageBox.Show("Clearance price cannot be zero. Please re-enter.", _
                                "Clearance Price - Error", MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                Return False
            ElseIf iCPrice < 0 Then
                MessageBox.Show("Clearance price cannot be less than zero. Please re-enter.", _
                                "Clearance Price - Error", MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                Return False
            ElseIf iCPrice > iCurrentPrice Then
                MessageBox.Show("Clearance price cannot be greater than current price. Please re-enter.", _
                                "Clearance Price - Error", MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                Return False
            ElseIf iCPrice = iCurrentPrice Then                                                 'Defect  BTCPR00003815
                MessageBox.Show("Clearance price cannot be same as current price. Please re-enter.", _
                                "Clearance Price - Error", MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                Return False
            End If

            Return True
        Catch ex As Exception
            MessageBox.Show("Invalid Price Entered.", "Clearance Price - Error", _
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                            MessageBoxDefaultButton.Button1)                                    'Defect  BTCPR00003813
            Return False
        End Try
    End Function
    '*******End change
    ''' <summary>
    ''' Validates the quantity entered
    ''' </summary>
    ''' <param name="iResult"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ValidateDoubleDigitEntry(ByVal iResult As Long) As Boolean
        Dim bIsValidEntry As Boolean = True
        Try
            'From Helper class
            If Not IsValidated Then
                If objAppContainer.objHelper.ValidateDoubleDigitQuantity(iResult) Then
                    bIsValidEntry = True
                Else

                    Dim iMsgRet As Integer = 0
                    iMsgRet = MessageBox.Show("You entered " + iResult.ToString() + ". Is this correct?", "Quantity Entry - Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                    IsValidated = True
                    If iMsgRet = MsgBoxResult.Yes Then
                        bIsValidEntry = True
                    Else
                        bIsValidEntry = False
                    End If
                End If
            End If
        Catch ex As Exception
            Return False
        End Try

        Return bIsValidEntry
    End Function
    ''' <summary>
    ''' Validates Quantity Greater than Fifty
    ''' </summary>
    ''' <param name="iResult"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ValidateGreaterThanFifty(ByVal iResult As Long) As Boolean
        Dim bIsValidEntry As Boolean = True
        Try
            If Not IsValidated Then
                If objAppContainer.objHelper.ValidateQtyGreaterThanFifty(iResult) Then
                    bIsValidEntry = True
                Else
                    Dim iMsgRet As Integer = 0
                    iMsgRet = MessageBox.Show("Quantity greater than 50. Quantity Entered " + iResult.ToString() + ". Are you sure?", "Quantity Entry - Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                    IsValidated = True
                    If iMsgRet = MsgBoxResult.Yes Then
                        bIsValidEntry = True
                    Else
                        bIsValidEntry = False
                    End If
                End If
            End If
        Catch ex As Exception
            Return False
        End Try
        Return bIsValidEntry
    End Function

    ''' <summary>
    ''' Processes the help button click
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessHelp()

    End Sub
    ''' <summary>
    ''' Retrieves the current value of text field
    ''' </summary>
    ''' <param name="currVal"></param>
    ''' <param name="tbValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RetrieveCurrentVal(ByRef currVal As VariantType, ByRef tbValue As TextBox) As VariantType
        currVal = tbValue.Text
        Return currVal
    End Function
    ' Proxy string handling commands
    Public Function SWLeft(ByVal sString As String, ByVal lLth As Long) As String

        SWLeft = Microsoft.VisualBasic.Strings.Left(sString, lLth)
    End Function

    Public Function SWRight(ByVal sString As String, ByVal lLth As Long)
        SWRight = Microsoft.VisualBasic.Strings.Right(sString, lLth)
    End Function


End Class
