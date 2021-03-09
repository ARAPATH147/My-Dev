''' <summary>
''' This business class manages all data specific to the actions on the calc pad during a calc pad session
''' </summary>
''' <remarks></remarks>
Public Class CalcPadSessionMgr
    Private Shared m_CalcPadSessionMgr As CalcPadSessionMgr = Nothing
    'Fix for the maximum quantity for item is 9999 instead of 999
    Private Const MAX_QUANTITY As Integer = 9999
    Private IsValidated As Boolean = False
    Private ReferenceValue As New Control
    Private EnumCalcpad As CalcPadSessionMgr.EntryTypeEnum
    Dim iQty As Integer
    Public Enum EntryTypeEnum
        Quantity = 0
        UOD = 1
        Barcode = 2
        Supplier = 3
    End Enum

    Public cEntryType As String
    Public strAvailableData As Boolean
    Private m_Calcpad As frmCalcPad
    Public Sub StartSession(ByRef refValue As Control, ByVal EntryTypeTemp As CalcPadSessionMgr.EntryTypeEnum)
        Try
            m_Calcpad = New frmCalcPad(refValue, EntryTypeTemp)
            EnumCalcpad = EntryTypeTemp
            ReferenceValue = refValue
            If EntryTypeTemp = EntryTypeEnum.Quantity Then
                iQty = Convert.ToInt32(refValue.Text.ToString())
            End If
            DisplayCalcpad(refValue, EntryTypeTemp)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Calcpad session cannot be started", _
                                                           Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Public Function EndSession() As Boolean
        Try
            m_Calcpad.Close()


        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Calcpad Session Manager EndSession failure", _
                                      Logger.LogLevel.RELEASE)
        End Try
    End Function
    Public Function DisplayCalcpad(ByRef refValue As Control, ByVal EntryTypeTemp As CalcPadSessionMgr.EntryTypeEnum) As Boolean
        Try
            ' m_Calcpad.CalPadInitialize(refValue, EntryTypeTemp)
            m_Calcpad.Invoke(New EventHandler(AddressOf DisplayCalcPadScreen))
        Catch ex As Exception
            Return False

        End Try
        Return True

    End Function

    Public Sub DisplayCalcPadScreen(ByVal o As Object, ByVal e As EventArgs)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
        With m_Calcpad
            ' objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.CALCPAD
            .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' Functions for getting the object instance for the CalcPadSessionMgr. 
    ''' Use this method to get the object refernce for the Singleton CalcPadSessionMgr.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of CalcPadSessionMgr Class</remarks>
    Public Shared Function GetInstance() As CalcPadSessionMgr
        If m_CalcPadSessionMgr Is Nothing Then
            m_CalcPadSessionMgr = New CalcPadSessionMgr
            Return m_CalcPadSessionMgr
        Else
            Return m_CalcPadSessionMgr
        End If

    End Function

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
                bIsValid = ProcessQuantityEntry(tbValue)
            Case EntryTypeEnum.Barcode
                bIsValid = ProcessBarcodeEntry(tbValue)
            Case EntryTypeEnum.UOD
                bIsValid = ProcessUODEntry(tbValue)
            Case EntryTypeEnum.Supplier
                bIsValid = ProcessSupplierEntry(tbValue)
        End Select

        Return bIsValid
    End Function
    ''' <summary>
    ''' Processes the Supplier number entered
    ''' </summary>
    ''' <param name="tbValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessSupplierEntry(ByRef tbValue As Control) As Boolean
        Try
            If (Len(tbValue.Text.Trim) = 6) Then
                Return True
            Else
                MessageBox.Show("Invalid input:" + vbCr + "Supplier number should be a 6 Digit Value", "Supplier Entry - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show("Invalid input:" + vbCr + "Supplier number should be a 6 Digit Value", "Supplier Entry - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            Return False
        End Try
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

        Dim cTemp As String
        cTemp = ProcessTextEntry(tbValue.Text)

        Dim objArray As String() = cTemp.Split(" ")
        Dim iIndex As Int32 = 0  'keep track of position in array 
        Dim bIsValidEntry As Boolean = True

        Try
            'Checks if the data entered is of length one
            If objArray.Length = 1 Then

                cResultAsString = objArray.GetValue(iIndex)
                If cResultAsString <> "0" Then
                    'If cResultAsString.Equals("+") Or cResultAsString.Equals("-") Or cResultAsString.Equals("x") Then
                    '    '  MessageBox.Show(MessageManager.GetInstance().GetMessage("M20"))
                    '    Return False
                    'End If
                    iResult = Val(cResultAsString)
                    If iResult > MAX_QUANTITY Then
                        MessageBox.Show("Maximum Quantity allowed is " + MAX_QUANTITY.ToString + vbCr + "Please Verify.", "Quantity Entry - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        Return False
                    Else
                        If (Not ValidateGreaterThanFifty(iResult)) Or (Not ValidateDoubleDigitEntry(iResult)) Then
                            Return False
                        End If
                    End If
                    tbValue.Text = CStr(iResult.ToString())
                    ProcessEnteredValue(tbValue, bIsValidEntry)

                Else
                    ProcessEnteredValue(tbValue, bIsValidEntry)
                End If
            Else
                iValue1 = Val(objArray.GetValue(iIndex))
                Dim iFirstVal As Integer = 0

                iFirstVal = Val(objArray.GetValue(iIndex))

              

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

                If iResult < 0 Then
                    '  MessageBox.Show(MessageManager.GetInstance().GetMessage("M1"))
                    MessageBox.Show("The Value cannot be less than zero", "Quantity Entry - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    Return False
                ElseIf iResult > MAX_QUANTITY Then
                    MessageBox.Show("Maximum Quantity allowed is " + MAX_QUANTITY.ToString + vbCr + "Please Verify.", "Quantity Entry - Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    Return False
                ElseIf (Not ValidateGreaterThanFifty(iResult)) Or (Not ValidateDoubleDigitEntry(iValue1)) Then
                    Return False
                End If
                'resultAsString = result.ToString()
                tbValue.Text = CStr(iResult.ToString())
                ProcessEnteredValue(tbValue, bIsValidEntry)

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
            If IsValidated = False Then
                If objAppContainer.objHelper.ValidateDoubleDigitQuantity(iResult) Then
                    bIsValidEntry = True
                Else

                    Dim iMsgRet As Integer = 0
                    iMsgRet = MessageBox.Show("You entered " + iResult.ToString() + ". Is this correct?", "Qunatity Entry - Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
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
    Public Function RetrieveCurrentVal(ByRef currVal As Integer, ByRef tbValue As TextBox) As Integer
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
    Public Function ProcessEnteredValue(ByRef tbValue As TextBox, ByRef bIsValidEntry As Boolean)

        Select Case EnumCalcpad
            Case EntryTypeEnum.Barcode
                If objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.BOOKINCARTON Then
                    BCSessionMgr.GetInstance().HandleScanData(tbValue.Text, BCType.ManualEntry)
                Else
                    If tbValue.Text.Trim().Length > 0 Then
                        BCReader.GetInstance().EventBCScannedHandler(tbValue.Text.ToString().Trim(), BCType.ManualEntry)
                    End If
                End If

            Case EntryTypeEnum.Quantity

                Select Case objAppContainer.objActiveModule
                    Case AppContainer.ACTIVEMODULE.AUDITUOD

                        'Checks whether zero is entered
                        If Not objAppContainer.objHelper.ValidateZeroQty(CInt(tbValue.Text)) Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M1"), "Quantity Entry - Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            tbValue.Text = iQty.ToString()
                            bIsValidEntry = False

                            'Me.Refresh()
                            'checks whether the quantity has changed
                        ElseIf Not iQty = CInt(tbValue.Text) Then
                            AUODSessionManager.GetInstance().SetItemQuantity(tbValue.Text)
                            AUODSessionManager.GetInstance().DisplayAUODScreen(AUODSessionManager.AUODSCREENS.AuditItem)
                        End If

                    Case AppContainer.ACTIVEMODULE.AUDITCARTON
                        'Checks whether zero is entered
                        If Not objAppContainer.objHelper.ValidateZeroQty(CInt(tbValue.Text)) Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M1"), "Quantity Entry - Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)

                            tbValue.Text = iQty.ToString()
                            bIsValidEntry = False

                            'checks whether the quantity has changed
                        ElseIf Not iQty = CInt(tbValue.Text) Then

                            ACSessionManager.GetInstance().SetItemQty(tbValue.Text)
                            ACSessionManager.GetInstance().DisplayACScreen(ACSCREENS.AuditItem)

                        End If
                        tbValue.Text = ""
                    Case AppContainer.ACTIVEMODULE.BOOKINCARTON
                        'Checks whether zero is entered
                        If Not objAppContainer.objHelper.ValidateZeroQty(CInt(tbValue.Text)) Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M1"), "Quantity Entry - Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            tbValue.Text = iQty.ToString()
                            bIsValidEntry = False

                            'Check whether the qty entered is higher than expected qty
                        ElseIf Not BCSessionMgr.GetInstance().CheckExpectedOrderQty(CInt(tbValue.Text)) Then
                            ' tbValue.Text = iQty.ToString()
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M68"), "Alert ", MessageBoxButtons.OK, _
                                                                                  MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            BCSessionMgr.GetInstance().SetQuantity(tbValue.Text)
                            bIsValidEntry = True
                            objAppContainer.bCalcpad = True
                            'checks whether the quantity has changed
                        ElseIf Not iQty = CInt(tbValue.Text) Then
                            BCSessionMgr.GetInstance().SetItemQty(tbValue.Text)

                        End If
                End Select
            Case EntryTypeEnum.Supplier
                BCSessionMgr.GetInstance().SupplierNoEntry(tbValue.Text)


        End Select
        tbValue.Text = ""
    End Function

End Class
