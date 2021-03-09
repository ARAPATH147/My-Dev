'''****************************************************************************
''' <FileName> ParcelSession.vb </FileName> 
''' <summary> Session manager for Bookin Only, Bookin and Put Away and Put Away/Move</summary> 
''' <Version>1.0</Version> 
''' <Author>Andrew Paton</Author> 
''' <DateModified>11-05-2016</DateModified> 
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC55RF</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK </CopyRight>
'''****************************************************************************
'''* Modification Log 
'''**************************************************************************** 
'''  1.0    Andrew Paton                             11/05/2016        
'''         Inital Version.
''' 
'''**************************************************************************** 

Public Class ParcelSession

    Private Shared moBookIn As ParcelSession

    Private mfrmBookInScreen As frmBookInOrder
    Private mfrmLocationScreen As frmSelectLocation

    Private mASNCode As ASNCode
    Private mParcel As ParcelRecord
    Private mCurrentParcel As CurrentParcel
    Private mLocation As LocationRecord
    Private mOrder As OrderRecord
    Private mCount As Integer
    Private mcParcelList As List(Of String)
    Private mcBootsSupplierCode As String
    Private bParcelOnFile As Boolean

    ''' <summary>
    ''' The shared function GetInstance will implement a check for the instantiation
    ''' of class ParcelSession to make sure that the class has only one instance
    ''' </summary>
    ''' <returns>moBookIn</returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As ParcelSession
        If moBookIn Is Nothing Then
            moBookIn = New ParcelSession()
            Return moBookIn
        Else
            Return moBookIn
        End If
    End Function

    Public Sub StartSession()
        mfrmBookInScreen = New frmBookInOrder
        mCount = 0
        mParcel = New ParcelRecord
        mCurrentParcel = New CurrentParcel
        mLocation = New LocationRecord
        mOrder = New OrderRecord
        mcParcelList = New List(Of String)
        mcBootsSupplierCode = ConfigFileManager.GetInstance.GetParam(ConfigKey.SUPPLIER_NUMBER)

        Select Case oAppMain.enActiveModule
            Case AppMain.ACTIVEMODULE.BOOKINWITHLOCATION
                mfrmBookInScreen.lblSelectParcel.Text = "Book In and put away order"
                RequestOutstandingCount()
            Case AppMain.ACTIVEMODULE.BOOKINORDER
                RequestOutstandingCount()
            Case AppMain.ACTIVEMODULE.MOVEPUTAWAY
                'Change screen label for Put Away/Move Parcel scan screen                  
                mfrmBookInScreen.lblSelectParcel.Text = "Put away / move order"
                mfrmBookInScreen.lblBookedIn.Visible = False
                mfrmBookInScreen.lblOutstanding.Visible = False
                mfrmBookInScreen.lblBookedInCount.Visible = False
                mfrmBookInScreen.lblOutStandingCount.Visible = False
            Case AppMain.ACTIVEMODULE.QUERYCOLLECT
                'Change screen label for Put Query/Collect Parcel scan screen                  
                mfrmBookInScreen.lblSelectParcel.Text = "Query / collect order"
                mfrmBookInScreen.lblBookedIn.Visible = False
                mfrmBookInScreen.lblOutstanding.Visible = False
                mfrmBookInScreen.lblBookedInCount.Visible = False
                mfrmBookInScreen.lblOutStandingCount.Visible = False
        End Select

        If oAppMain.bConnect Or oAppMain.enActiveModule = AppMain.ACTIVEMODULE.MOVEPUTAWAY Then
            mfrmBookInScreen.Visible = True
        Else
            ' unable to reconnect to the controller.
            CloseSession()
        End If

    End Sub


    Public Sub CancelCurrentBookIn()
        'discard current order details. 
        mfrmLocationScreen.Close()
        oAppMain.enActiveScreen = AppMain.ACTIVESCREEN.BOOKINPARCEL
    End Sub

    Public Sub RequestOutstandingCount()
        Dim cEmptyString As String = New String("0", 16)

        If QueryController(cEmptyString, QUERY.COUNT) Then
            mfrmBookInScreen.lblOutStandingCount.Text = mCount
        End If
    End Sub

    Public Sub HandleScanData(ByVal cBarcode As String, ByVal enType As BCType)
        If enType = BCType.CODE128 Or enType = BCType.EAN Then
            Select Case oAppMain.enActiveModule
                Case AppMain.ACTIVEMODULE.BOOKINORDER
                    CartonScanned(cBarcode)
                Case AppMain.ACTIVEMODULE.BOOKINWITHLOCATION
                    If oAppMain.enActiveScreen = _
                                         AppMain.ACTIVESCREEN.BOOKINPARCEL Then

                        CartonScanned(cBarcode)
                    ElseIf oAppMain.enActiveScreen = _
                                       AppMain.ACTIVESCREEN.SELECTLOCATION Then

                        LocationScanned(cBarcode)
                    End If
                Case AppMain.ACTIVEMODULE.MOVEPUTAWAY
                    If oAppMain.enActiveScreen = _
                                         AppMain.ACTIVESCREEN.BOOKINPARCEL Then

                        CartonScanned(cBarcode)
                    ElseIf oAppMain.enActiveScreen = _
                                       AppMain.ACTIVESCREEN.SELECTLOCATION Then

                        LocationScanned(cBarcode)
                    End If
                Case AppMain.ACTIVEMODULE.QUERYCOLLECT
                    CartonScanned(cBarcode)
            End Select
        Else
            MessageBox.Show(MessageManager.GetInstance.GetMessage("M3"), _
                         "Error ", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                                MessageBoxDefaultButton.Button1)
            Exit Sub
        End If

    End Sub

    Public Sub CartonScanned(ByVal cBarCode As String) 
        If ValidateBarcode(cBarCode) Then
            If mASNCode.cSupplierNumber = mcBootsSupplierCode Then

                ' Get order details
                If QueryController(mASNCode.cSupplierNumber _
                                   & mASNCode.cCartonNumber, QUERY.PARCEL) Then
                    bParcelOnFile = True
                    ' Set parcel as the current parcel
                    mCurrentParcel = RecordFunc.saveToCurrentParcel(mParcel)

                    If CheckParcelStatus() Then
                        Select Case oAppMain.enActiveModule
                            Case AppMain.ACTIVEMODULE.BOOKINWITHLOCATION
                                OpenLocationScreen()
                            Case AppMain.ACTIVEMODULE.MOVEPUTAWAY
                                OpenLocationScreen()
                            Case AppMain.ACTIVEMODULE.BOOKINORDER
                                ' Transact will not use this value but need to
                                ' be populated for the record length
                                mCurrentParcel.cLocationCurrent = "0000"
                                UpdateParcel(UPDATE.BOOK_IN_ONLY)
                            Case AppMain.ACTIVEMODULE.QUERYCOLLECT
                                'Code Placeholder
                        End Select
                    End If
                Else
                    '---------------------------
                    'Handle NOT-ON-FILE scenario
                    '---------------------------
                    If oAppMain.bConnect = True Then
                        If oAppMain.enActiveModule <> AppMain.ACTIVEMODULE.MOVEPUTAWAY Then

                            bParcelOnFile = False

                            'Use scanned data and default values that Transact will use
                            'to update the BDCP file. Populate the parcel record with
                            'the details that we scanned.
                            mCurrentParcel.cParentOrderNumber = "0000000000"
                            mCurrentParcel.cSupplierNumber = mASNCode.cSupplierNumber
                            mCurrentParcel.cParcelNumber = mASNCode.cCartonNumber
                            mCurrentParcel.cLocationCurrent = "0000"


                            'PARCEL NOT ON FILE - Is this parcel for your store?
                            If MessageBox.Show(MessageManager.GetInstance.GetMessage("M4"), "Warning", _
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, _
                                   MessageBoxDefaultButton.Button1) = System.Windows.Forms.DialogResult.No Then

                                mCurrentParcel.cCurrentStatus = "R" 'Set status even though NOT for this store

                                UpdateParcel(UPDATE.BOOK_IN_ONLY)

                                'Parcel is NOT on file and NOT for this store
                                'Please follow the mis-direct process in order to send the parcel back to the warehouse
                                MessageBox.Show(MessageManager.GetInstance.GetMessage("M16"))

                            Else
                                '-------------------------------------------
                                'Parcel is NOT ON file and is for this store
                                '-------------------------------------------
                                mCurrentParcel.cCurrentStatus = "R" 'In store ready for customer collection

                                AppMain.displayCalcPadScreen(AppMain.CALCPADUSE.PARCELORDERNUMBER)

                            End If
                        Else
                            'A parcel must already be booked-in in order to use the MOVEPUTAWAY option
                            MessageBox.Show(MessageManager.GetInstance.GetMessage("M19"), "WARNING", _
                                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                                            MessageBoxDefaultButton.Button1)
                        End If
                    Else
                        ' unable to reconnect to the controller.
                        CloseSession()
                    End If
                End If
            Else
                Select Case oAppMain.enActiveModule
                    Case AppMain.ACTIVEMODULE.BOOKINWITHLOCATION, AppMain.ACTIVEMODULE.MOVEPUTAWAY
                        'Please scan the parcel before scanning the location
                        MessageBox.Show(MessageManager.GetInstance.GetMessage("M24"))
                    Case AppMain.ACTIVEMODULE.BOOKINORDER
                        'Item scanned is not a parcel
                        MessageBox.Show(MessageManager.GetInstance.GetMessage("M5"))
                    Case AppMain.ACTIVEMODULE.QUERYCOLLECT
                        'Code Placeholder
                End Select
            End If
        Else
            'Not a valid barcode
            MessageBox.Show(MessageManager.GetInstance.GetMessage("M6"))
        End If
    End Sub

    Private Sub ContinueBookingIn()
        Select Case oAppMain.enActiveModule
            Case AppMain.ACTIVEMODULE.BOOKINWITHLOCATION
                OpenLocationScreen()
            Case AppMain.ACTIVEMODULE.BOOKINORDER
                UpdateParcel(UPDATE.BOOK_IN_ONLY)
            Case AppMain.ACTIVEMODULE.MOVEPUTAWAY
                OpenLocationScreen()
            Case AppMain.ACTIVEMODULE.QUERYCOLLECT
                ' Code Placeholder
        End Select
    End Sub

    Private Sub OpenLocationScreen()
        ' Order number if parcel scanned is a misdirect and waiting for ASN 
        Dim cNotOnFile As String = "0000000000"

        mfrmLocationScreen = New frmSelectLocation

        ' Clear the list box to remove unwanted data
        mfrmLocationScreen.lstParcelLocation.Items.Clear()

        ' Check if the order number is not all 0's 
        If mCurrentParcel.cParentOrderNumber <> cNotOnFile Then
            ' Check for other parcels in the order
            ProcessOrder()

            ' If there are other parcels for the order 
            If mcParcelList.Count <> 0 Then
                ' Populate a list box with all other booked in  
                ' parcels in the order with there locations 
                For Each item In mcParcelList
                    mfrmLocationScreen.lstParcelLocation.Items.Add(item)
                Next
                SetControlVisibility(True)
            Else
                SetControlVisibility(False)
            End If
        End If

        mfrmLocationScreen.lblOrderNumber.Text = _
                                              mCurrentParcel.cParentOrderNumber

        ' Change screen label for Put Away/Move Parcel New Location scan screen     
        If oAppMain.enActiveModule = AppMain.ACTIVEMODULE.MOVEPUTAWAY Then
            mfrmLocationScreen.lblSelectLocation.Text = _
                 "Scan New Location for parcel " & mCurrentParcel.cParcelNumber
            mfrmLocationScreen.lblScanSelectLocation.Text = _
                                                    "Scan / Enter New Location"
        Else
            mfrmLocationScreen.lblSelectLocation.Text += _
                                                   mCurrentParcel.cParcelNumber
        End If
        If oAppMain.bConnect = True Then
            mfrmLocationScreen.Visible = True
        End If
    End Sub

    Public Sub ParcelNumberEntered(ByVal cOrderNumber As String)
        mCurrentParcel.cParentOrderNumber = cOrderNumber
        ContinueBookingIn()
    End Sub

    Public Sub ProcessOrder()
        ' Get the order record for the scanned parcel
        If QueryController(mCurrentParcel.cSupplierNumber & mCurrentParcel.cParentOrderNumber, QUERY.ORDER) Then
            mcParcelList.Clear()
            For Each item In mOrder.Cartons
                ' If not the scanned parcel number
                If item <> mCurrentParcel.cParcelNumber Then
                    ' Get the parcel record
                    If QueryController(mOrder.cSupplierNumber & item, QUERY.PARCEL) Then
                        ' If the parcel is booking in and awaiting collection
                        If mParcel.cCurrentStatus = "R" Then
                            ' Get the location record as long as the parcel location is set
                            If mParcel.cLocationCurrent <> "" Then
                                QueryController(mParcel.cLocationCurrent, QUERY.LOCATION)
                            End If
                            Dim entry As String = String.Format("{0,-10}{1,-6}{2,-20}", _
                                                                mParcel.cParcelNumber, _
                                                                mParcel.cLocationCurrent, _
                                                                 mLocation.cLongDescription)
                            ' Add details to parcel list
                            mcParcelList.Add(entry)
                        End If
                    Else
                        If oAppMain.bConnect = False Then
                            ' unable to reconnect to the controller.
                            CloseSession()
                        End If
                    End If
                End If
            Next
        Else
            If oAppMain.bConnect = False Then
                ' unable to reconnect to the controller.
                CloseSession()
            End If
            'MessageBox.Show("Order not on file")
        End If
    End Sub

    Public Sub LocationScanned(ByVal cBarCode As String)
        If cBarCode.Length = 18 And cBarCode.StartsWith("0000000000") And Convert.ToInt64(cBarCode) > 0 Then
            cBarCode = cBarCode.Remove(0, 2)
            If QueryController(cBarCode, QUERY.LOCATION) Then
                If mLocation.cStatus = "A" Then
                    'Use last 4-digits of barcode in order to match TRANSACT OCU message
                    mCurrentParcel.cLocationCurrent = cBarCode.Remove(0, 12)
                    mCurrentParcel.cCurrentStatus = "R"
                    Select Case oAppMain.enActiveModule
                        Case AppMain.ACTIVEMODULE.BOOKINWITHLOCATION
                            If bParcelOnFile Then
                                UpdateParcel(UPDATE.BOOK_IN_PUTWAY)
                            Else
                                UpdateParcel(UPDATE.MISDIRECT)
                            End If
                        Case AppMain.ACTIVEMODULE.MOVEPUTAWAY
                            UpdateParcel(UPDATE.PUTAWAY_MOVE)
                    End Select
                    mfrmLocationScreen.Close()
                    oAppMain.enActiveScreen = AppMain.ACTIVESCREEN.BOOKINPARCEL
                Else
                    'Selected Location is not active, please select another location
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M7"))
                End If
            Else
                If oAppMain.bConnect = True Then
                    'Not a valid Location try again
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M8"))
                Else
                    ' unable to reconnect to the controller.
                    CloseSession()
                End If
            End If
        Else
            'Not a valid Location try again
            MessageBox.Show(MessageManager.GetInstance.GetMessage("M8"))
        End If
    End Sub

    Public Sub UpdateParcel(ByVal enUpdateFLag As UPDATE)
        'Send booking to controller 
        Dim cResponseData As String = ""
        Dim cTID As String = ""

        Dim cRecord As String = RecordFunc.buildParcelRecord(mCurrentParcel)
        If RFDataManager.GetInstance.CheckReconnect Then
            If RFDataManager.GetInstance.SendUpdate(cRecord, enUpdateFLag) Then
                If RFDataManager.GetInstance.WaitForResponse(cResponseData) Then
                    cTID = cResponseData.Substring(0, 3)
                    Select Case cTID
                        'Booking complete
                        Case "ACK"
                            If enUpdateFLag <> UPDATE.PUTAWAY_MOVE Then
                                '*** Uncomment code for development purposes ONLY ***
                                '***If enUpdateFLag = UPDATE.BOOK_IN_ONLY Then
                                '***MessageBox.Show("Parcel " & mASNCode.cCartonNumber & _
                                '***                " Booked in ")
                                '***ElseIf enUpdateFLag = UPDATE.BOOK_IN_PUTWAY Then
                                '***MessageBox.Show("Parcel " & mASNCode.cCartonNumber & _
                                '***             " Booked in and moved to location " & _
                                '***             mCurrentParcel.cLocationCurrent)
                                '***End If
                                mfrmBookInScreen.lblBookedInCount.Text += 1
                                If bParcelOnFile Then
                                    If mfrmBookInScreen.lblOutStandingCount.Text <> 0 Then
                                        mfrmBookInScreen.lblOutStandingCount.Text -= 1
                                    End If
                                End If
                                '***Else
                                '***    MessageBox.Show("Parcel " & mASNCode.cCartonNumber & _
                                '***        " is now in location " & mCurrentParcel.cLocationCurrent)
                            End If

                            ' Booking failed 
                        Case "NAK"
                            'Display message passed by Transact 
                            MessageBox.Show(Right(cResponseData, cResponseData.Length - 3), "ERROR")
                    End Select
                End If
                If oAppMain.bTimeOut Then
                    ConnectionManager.GetInstance.HandleTimeOut()
                End If
            End If
        Else
            ' unable to reconnect to the controller. 
            CloseSession()
        End If
    End Sub

    Public Sub SetControlVisibility(ByVal status As Boolean)
        mfrmLocationScreen.lblBookedInMessage.Visible = status
        mfrmLocationScreen.lblParcelLocationList.Visible = status
        mfrmLocationScreen.lstParcelLocation.Visible = status
    End Sub

    ''' <summary>
    ''' Function to send an OCQ query message to TRANSACT and process the response  
    ''' </summary>
    ''' <param name="cQueryString">Query Data</param>
    ''' <param name="enQueryType">Query Type</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function QueryController(ByVal cQueryString As String, ByVal enQueryType As QUERY) As Boolean
        Dim cResponseData As String = ""
        Dim cTID As String = ""
        cQueryString = cQueryString.PadLeft(16, "0")
        If RFDataManager.GetInstance.CheckReconnect Then
            If RFDataManager.GetInstance.SendOCQ(cQueryString, enQueryType) Then
                If RFDataManager.GetInstance.WaitForResponse(cResponseData) Then
                    cTID = cResponseData.Substring(0, 3)
                    Select Case cTID
                        Case "OCN"
                            ' Not on file response
                            Return False
                        Case "OCD"
                            ' Parcel details  
                            mParcel = RecordFunc.processParcelRecord(cResponseData, mParcel)
                        Case "OCL"
                            ' Location details 
                            mLocation = RecordFunc.processLocationRecord(cResponseData, mLocation)
                        Case "OCO"
                            ' Order details
                            mOrder = RecordFunc.processOrderRecord(cResponseData, mOrder)
                        Case "OCC"
                            mCount = cResponseData.Substring(4, 5)
                        Case "NAK"
                            'Display message passed by Transact 
                            MessageBox.Show(Right(cResponseData, cResponseData.Length - 3), "ERROR")
                            Return False
                    End Select
                Else
                    If Not RFDataManager.GetInstance.CheckReconnect() Then
                        'if retry selected on timeout 
                        If oAppMain.bTimeOut And oAppMain.bRetryAtTimeout Then
                            oAppMain.bTimeOut = False
                            Do Until Not oAppMain.bRetryAtTimeout
                                RFDataManager.GetInstance.CheckReconnect(True)
                            Loop
                        End If
                        Return False
                    Else
                        Return False
                    End If
                End If
            Else
                If Not RFDataManager.GetInstance.CheckReconnect() Then
                    If oAppMain.bTimeOut And oAppMain.bRetryAtTimeout Then
                        oAppMain.bTimeOut = False
                        Do Until Not oAppMain.bRetryAtTimeout
                            RFDataManager.GetInstance.CheckReconnect(True)
                        Loop
                    End If
                    'Unable to send request
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M14"), "error")
                End If
            End If
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Function to check Parcel Status and display the corresponding pop-up message  
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function CheckParcelStatus() As Boolean
        Select Case mParcel.cCurrentStatus
            Case "R" 'In store ready for customer collection
                If oAppMain.enActiveModule <> AppMain.ACTIVEMODULE.MOVEPUTAWAY Then
                    If mCurrentParcel.cLocationCurrent <> "0000" Then
                        If QueryController(mCurrentParcel.cLocationCurrent, QUERY.LOCATION) Then
                            'Parcel already booked in at location: <display last 3 digits> 
                            MessageBox.Show(MessageManager.GetInstance.GetMessage("M9") & _
                                            Right(mCurrentParcel.cLocationCurrent, 3))
                        End If
                    Else
                        'Parcel already booked in but no Location has been set
                        MessageBox.Show(MessageManager.GetInstance.GetMessage("M10"))
                    End If
                    Return False
                End If
            Case "O" 'On way to store
                If oAppMain.enActiveModule = AppMain.ACTIVEMODULE.MOVEPUTAWAY Then
                    'Parcel has not yet been booked in
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M11"))
                    Return False
                End If
            Case "C" 'Collected by customer
                'Parcel showing as collected
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M12"), "WARNING")
                Return False
            Case "U" 'Uncollected and returned to centre
                'Parcel to be returned to centre
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M13"))
                Return False
        End Select
        Return True
    End Function

    ''' <summary>
    ''' Function to validate the barcode and extract the component fields  
    ''' </summary>
    ''' <param name="cBarcode">Barcode scanned or key-entered</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function ValidateBarcode(ByVal cBarcode As String) As Boolean
        mASNCode = New ASNCode
        If cBarcode.Length = 18 Then
            With mASNCode
                .cSupplierNumber = cBarcode.Substring(0, 6)
                .cCartonNumber = cBarcode.Substring(6, 8)
                .cNoOfCartons = cBarcode.Substring(14, 4)
            End With
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub CloseSession()
        If Not mfrmLocationScreen Is Nothing Then
            CancelCurrentBookIn()
        End If
        mfrmLocationScreen = Nothing
        mfrmBookInScreen.Close()
        mfrmBookInScreen = Nothing
        moBookIn = Nothing
    End Sub

    Public Enum QUERY
        PARCEL
        LOCATION
        ORDER
        COUNT
    End Enum

    Public Enum UPDATE
        BOOK_IN_ONLY
        BOOK_IN_PUTWAY
        PUTAWAY_MOVE
        COLLECT
        MISDIRECT
    End Enum

    Public Structure ASNCode
        Public cSupplierNumber As String
        Public cCartonNumber As String
        Public cNoOfCartons As String
    End Structure

    Public Structure CurrentParcel
        Public cSupplierNumber As String
        Public cParcelNumber As String
        Public cParentOrderNumber As String
        Public cExpectedDelivery As String
        Public cCurrentStatus As String
        Public cDeliveryDateTime As String
        Public cIsDeliveryExported As String
        Public cCollectedDateTime As String
        Public cCollectedReasonCode As String
        Public cIsCollectionExported As String
        Public cReturnedToCentreDateTime As String
        Public cIsReturnToCentreExported As String
        Public cLostDateTime As String
        Public cIsLostEventExported As String
        Public cFoundDateTime As String
        Public cIsFoundEventExported As String
        Public cLocationCurrent As String
        Public cLocationStatus As String
        Public cFiller As String
    End Structure

    Public Structure ParcelRecord
        Public cSupplierNumber As String
        Public cParcelNumber As String
        Public cParentOrderNumber As String
        Public cExpectedDelivery As String
        Public cCurrentStatus As String
        Public cDeliveryDateTime As String
        Public cIsDeliveryExported As String
        Public cCollectedDateTime As String
        Public cCollectedReasonCode As String
        Public cIsCollectionExported As String
        Public cReturnedToCentreDateTime As String
        Public cIsReturnToCentreExported As String
        Public cLostDateTime As String
        Public cIsLostEventExported As String
        Public cFoundDateTime As String
        Public cIsFoundEventExported As String
        Public cLocationCurrent As String
        Public cLocationStatus As String
        Public cFiller As String
    End Structure

    Public Structure OrderRecord
        Public cSupplierNumber As String
        Public cOrderNumber As String
        Public Cartons As List(Of String)
        Public cFiller As String
    End Structure

    Public Structure LocationRecord
        Public cStatus As String
        Public cShortDescription As String
        Public cLongDescription As String
        Public cParcelCount As String
        Public cFiller As String
    End Structure

End Class
