''' <summary>
''' This class implements the interfaces for Scanning Barcodes and extracting embedded data for 
''' businees modules of ShelfManagement, GoodsOut and GoodsIn
''' </summary>
''' <remarks></remarks>

Public Class BCReader

    Private Shared objBCReader As BCReader

    Private hndlReader As Symbol.Barcode.Reader = Nothing
    Private hndlReaderData As Symbol.Barcode.ReaderData = Nothing
    Private ReadNotifyHandler As Global.System.EventHandler = Nothing
    Private StatusNotifyHandler As Global.System.EventHandler = Nothing
    Private hndlDecoder As Symbol.Barcode.DecoderParamsAPI = Nothing


    Public Event evtBCScanned(ByVal barcode As String, ByVal BarcodeType As BCType)
    Public Shared BCReaderStatus As String


    Private Sub New()
        '#If DEVICE = True Then
        'Initialise Barcode Reader
        Me.InitBCReader()
        '#End If

    End Sub
    Public Shared Function GetInstance() As BCReader
        If objBCReader Is Nothing Then
            objBCReader = New BCReader()
            Return objBCReader
        Else
            Return objBCReader
        End If
    End Function
    ''' <summary>
    ''' InitBCReader intialises the Barcode Reader hardware
    ''' </summary>
    ''' <returns>Ture is succesfull and False if failure</returns>
    ''' <remarks></remarks>
    Private Function InitBCReader()

        If Not (Me.hndlReader Is Nothing) Then
            Return False
        End If

        Try

            'Get Selected device from user
            Dim MyDevice As Symbol.Generic.Device _
                = Symbol.StandardForms.SelectDevice.Select( _
                                        Symbol.Barcode.Device.Title, _
                                        Symbol.Barcode.Device.AvailableDevices)
            If (MyDevice Is Nothing) Then
                MessageBox.Show("No Device Selected", "SelectDevice")
                Return False
            End If

            'create the reader, based on selected device
            Me.hndlReader = New Symbol.Barcode.Reader(MyDevice)

            ' Create reader data
            Me.hndlReaderData = New Symbol.Barcode.ReaderData( _
             Symbol.Barcode.ReaderDataTypes.Text, _
             Symbol.Barcode.ReaderDataLengths.DefaultText)

            'Enable reader, with wait cursor
            Me.hndlReader.Actions.Enable()

            'Set maximum length of I2OF5 barcode as 12 for scaning SELs.
            Me.hndlReader.Decoders.I2OF5.MaximumLength = Macros.MAX_I2OF5_LEN

            'create event handler
            Me.ReadNotifyHandler = New EventHandler(AddressOf hndlReader_ReadNotify)
            Me.StatusNotifyHandler = New EventHandler(AddressOf hndlReader_StatusNotify)

            Return True

        Catch ex As Symbol.Exceptions.InvalidRequestException
            MessageBox.Show("InitReader\n" + "Invalid Operation\n" + ex.Message())
            Return False

        Catch ex As Symbol.Exceptions.OperationFailureException
            MessageBox.Show("InitReader\n" + "Operation Failure" + ex.Message())
            Return False

        Catch ex As Symbol.Exceptions.UnimplementedFunctionException
            MessageBox.Show("InitReader\n" + "Unimplemented Function" + ex.Message())
            Return False

        End Try
    End Function
    'Stop reading and disable/close reader

    Public Sub TerminateBCReader()
        '#If DEVICE = True Then
        'If we have a reader
        If Not (Me.hndlReader Is Nothing) Then

            'stop all notifications
            Me.StopRead()
            Me.StopStatus()
            Try

                'Disable the Reader
                Me.hndlReader.Actions.Disable()

                'free it up and assign nothing 
                Me.hndlReader.Dispose()

                'Indiacte we no longer have one
                Me.hndlReader = Nothing

            Catch ex As Symbol.Exceptions.InvalidRequestException
                MessageBox.Show("InitReader\n" + "Invalid Operation\n" + ex.Message())

            Catch ex As Symbol.Exceptions.OperationFailureException

                MessageBox.Show("InitReader\n" + "Operation Failure" + ex.Message())

            Catch ex As Symbol.Exceptions.UnimplementedFunctionException

                MessageBox.Show("InitReader\n" + "Unimplemented Function" + ex.Message())

            End Try

        End If

        If Not (Me.hndlReaderData Is Nothing) Then

            'Free it up and assign nothing
            Me.hndlReaderData.Dispose()

            'Indicate we no longer have one
            Me.hndlReaderData = Nothing

        End If
        '#End If
    End Sub

    'Status notification handler

    Private Sub hndlReader_StatusNotify(ByVal sender As Object, ByVal e As EventArgs)

        'Get Current status
        Dim ScanEvent As Symbol.Barcode.BarcodeStatus = Me.hndlReader.GetNextStatus

        'set Status to Global.
        BCReaderStatus = ScanEvent.Text

    End Sub

    'Read notification handler

    Private Sub hndlReader_ReadNotify(ByVal sender As Object, ByVal e As EventArgs)

        'Get ReaderData
        Dim ScanData As Symbol.Barcode.ReaderData = Me.hndlReader.GetNextReaderData

        Select Case ScanData.Result

            Case Symbol.Results.SUCCESS

                'Handle the data from this read

                Me.HandleData(ScanData)

                '

            Case Symbol.Results.CANCELED

            Case Else

                'Dim sMsg As String
                'sMsg = "Read Failed\n" + "Result = " + (CInt(ScanData.Result)).ToString("X8")
                'MessageBox.Show(sMsg, "ReadNotify")

        End Select
    End Sub

    'Handle data from the reader

    Private Sub HandleData(ByVal ScanData As Symbol.Barcode.ReaderData)

        Dim barcodeType As BCType = BCType.None
        Dim bValidCode As Boolean = False

        Select Case ScanData.Type
            Case Symbol.Barcode.DecoderTypes.EAN128 '' Write code to handle the different types of barcodes for SM and Goods Out.
                bValidCode = True
                barcodeType = BCType.EAN
            Case Symbol.Barcode.DecoderTypes.CODE128
                bValidCode = True
                barcodeType = BCType.EAN
            Case Symbol.Barcode.DecoderTypes.EAN13
                bValidCode = True
                barcodeType = BCType.EAN
            Case Symbol.Barcode.DecoderTypes.EAN8
                bValidCode = True
                barcodeType = BCType.EAN
            Case Symbol.Barcode.DecoderTypes.I2OF5
                bValidCode = True
                barcodeType = BCType.SEL
            Case Symbol.Barcode.DecoderTypes.UPCA
                bValidCode = True
                'barcodeType = BCType.UPC
                barcodeType = BCType.EAN    'Changed to support 12-digit product codes.
        End Select
        If bValidCode = True Then
            If (barcodeType = BCType.EAN) Then
                'Padding the barcode to 13 Digits.
                RaiseEvent evtBCScanned((ScanData.Text).PadLeft(13, "0"), barcodeType)
            ElseIf ScanData.Text.StartsWith("8270") Then
                Dim tmpLength As Integer = ScanData.Text.ToString.Length
                If tmpLength = 16 Or tmpLength = 22 Then
                    barcodeType = BCType.CLRLBL
                    RaiseEvent evtBCScanned(ScanData.Text, barcodeType)
                Else
                    MessageBox.Show("Invalid Barcode scanned", "Error", MessageBoxButtons.OK, _
                                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    Me.StartRead()
                End If
            Else
                RaiseEvent evtBCScanned(ScanData.Text, barcodeType)
            End If
        Else
            MessageBox.Show("Invalid Barcode scanned", "Error", MessageBoxButtons.OK, _
                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            Me.StartRead()
        End If

    End Sub
    ' start a read on reader

    Public Sub StartRead()
        '#If DEVICE = True Then
        'if we have both a reader and readerdata
        If Not ((Me.hndlReader Is Nothing) And (Me.hndlReaderData Is Nothing)) Then

            Try

                AddHandler hndlReader.ReadNotify, Me.ReadNotifyHandler

                Me.hndlReader.Actions.Flush()
                'Submit a read
                Me.hndlReader.Actions.Read(Me.hndlReaderData)
                'Cursor.Current = Cursors.WaitCursor
            Catch ex As Symbol.Exceptions.UnimplementedFunctionException
                MessageBox.Show("StartRead\n" + "Unimplemented Function\n" + ex.Message())

            Catch ex As Symbol.Exceptions.InvalidIndexerException
                MessageBox.Show("StartRead\n" + "Invalid Indexer\n" + ex.Message())

            Catch ex As Symbol.Exceptions.OperationFailureException
                MessageBox.Show("StopRead\n" + "Operation Failure\n" + "Result = 0x" + (ex.Result).ToString("X8") + "\n" + ex.Message())

            Catch ex As Symbol.Exceptions.InvalidRequestException
                MessageBox.Show("StartRead\n" + "Invalid Request\n" + ex.Message)

            End Try

        End If
        '#End If
    End Sub

    ' stop all the read

    Public Sub StopRead()
        '#If DEVICE = True Then
        'if we do not have a reader, then do nothing
        If (Me.hndlReader Is Nothing) Then

            Return

        End If
        Try

            'remove read notification handler
            RemoveHandler hndlReader.ReadNotify, Me.ReadNotifyHandler

            'Flush (Cancel all pending reads)
            Me.hndlReader.Actions.Flush()

        Catch ex As Symbol.Exceptions.UnimplementedFunctionException
            MessageBox.Show("StopRead\n" + "Unimplemented Function\n" + ex.Message())

        Catch ex As Symbol.Exceptions.InvalidRequestException
            MessageBox.Show("StopRead\n" + "Invalid Request\n" + ex.Message())

        Catch ex As Symbol.Exceptions.OperationFailureException
            MessageBox.Show("StopRead\n" + "Operation Failure\n" + "Result = 0x" + (ex.Result).ToString("X8") + "\n" + ex.Message())

        End Try
        '#End If
    End Sub

    'Start Status notification

    Private Sub StartStatus()

        If Not (Me.hndlReader Is Nothing) Then

            'Attach  status notification handler
            AddHandler hndlReader.StatusNotify, Me.StatusNotifyHandler

        End If

    End Sub

    'Stop all status notifications

    Private Sub StopStatus()

        If Not (Me.hndlReader Is Nothing) Then

            'detach status notification handler
            RemoveHandler hndlReader.StatusNotify, Me.StatusNotifyHandler

        End If

    End Sub


    Public Sub EnableDecoder(ByVal DecoderType As Symbol.Barcode.DecoderTypes)
        '#If DEVICE = True Then
        Select Case DecoderType
            Case Symbol.Barcode.DecoderTypes.I2OF5
                If objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PRTCLEARANCE Then
                    hndlReader.Decoders.I2OF5.MinimumLength = 12
                    hndlReader.Decoders.I2OF5.MaximumLength = 22
                End If
                hndlReader.Decoders.I2OF5.Enabled = True
            Case Symbol.Barcode.DecoderTypes.CODE128
                hndlReader.Decoders.CODE128.Enabled = True
            Case Symbol.Barcode.DecoderTypes.EAN8
                hndlReader.Decoders.EAN8.Enabled = True
            Case Symbol.Barcode.DecoderTypes.EAN13
                hndlReader.Decoders.EAN13.Enabled = True
            Case Symbol.Barcode.DecoderTypes.UPCA
                hndlReader.Decoders.UPCA.Enabled = True
            Case Symbol.Barcode.DecoderTypes.CODE93
                hndlReader.Decoders.CODE39.Enabled = True
        End Select
        '#End If

    End Sub

    Public Sub DisableDecoder(ByVal DecoderType As Symbol.Barcode.DecoderTypes)
        '#If DEVICE = True Then
        Select Case DecoderType
            Case Symbol.Barcode.DecoderTypes.I2OF5
                If objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PRTCLEARANCE Then
                    hndlReader.Decoders.I2OF5.MinimumLength = 11
                    hndlReader.Decoders.I2OF5.MaximumLength = 12
                Else
                    hndlReader.Decoders.I2OF5.Enabled = False
                End If
            Case Symbol.Barcode.DecoderTypes.CODE128
                hndlReader.Decoders.CODE128.Enabled = False
            Case Symbol.Barcode.DecoderTypes.CODE93
                hndlReader.Decoders.CODE39.Enabled = False
            Case Symbol.Barcode.DecoderTypes.EAN8
                hndlReader.Decoders.EAN8.Enabled = False
            Case Symbol.Barcode.DecoderTypes.EAN13
                hndlReader.Decoders.EAN13.Enabled = False
            Case Symbol.Barcode.DecoderTypes.UPCA
                hndlReader.Decoders.UPCA.Enabled = False
        End Select
        '#End If
    End Sub
    Public Sub EventBCScannedHandler(ByVal strBarcode As String, ByVal Type As BCType)
        '#If DEVICE = True Then
        ' Write code to handle the event raised when a Barcode is scanned, depending on from which module it was scanned. 
        Select Case objAppContainer.objActiveModule
            Case AppContainer.ACTIVEMODULE.SHLFMNTR
                SMSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
            Case AppContainer.ACTIVEMODULE.FASTFILL
                FFSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
            Case AppContainer.ACTIVEMODULE.CUNTLIST
                CLSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
            Case AppContainer.ACTIVEMODULE.EXCSSTCK
                EXSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
            Case AppContainer.ACTIVEMODULE.ITEMINFO
                ItemInfoSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
#If RF Then


            Case AppContainer.ACTIVEMODULE.ITEMSALES
                ISSessionManager.GetInstance().HandleScanData(strBarcode, Type)
#End If
            Case AppContainer.ACTIVEMODULE.PICKGLST
                PLSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
            Case AppContainer.ACTIVEMODULE.SPCEPLAN
                SPSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
            Case AppContainer.ACTIVEMODULE.PRICECHK
                PCSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
            Case AppContainer.ACTIVEMODULE.PRINTSEL
                PSSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
            Case AppContainer.ACTIVEMODULE.PRTCLEARANCE                         'For clearance label priting
                CLRSessionMgr.GetInstance().HandleScanData(strBarcode, Type)     ' ,,
            Case AppContainer.ACTIVEMODULE.AUTOSTUFFYOURSHELVES
                AutoSYSSessionManager.GetInstance().HandleScanData(strBarcode, Type)
        End Select
        '#End If
    End Sub

End Class



'Public Class BCReader

'    Private Shared objBCReader As BCReader

'    Private hndlReader As Symbol.Barcode.Reader = Nothing
'    Private hndlReaderData As Symbol.Barcode.ReaderData = Nothing
'    Private ReadNotifyHandler As Global.System.EventHandler = Nothing
'    Private StatusNotifyHandler As Global.System.EventHandler = Nothing
'    Private hndlDecoder As Symbol.Barcode.DecoderParamsAPI = Nothing


'    Public Event evtBCScanned(ByVal barcode As String, ByVal BarcodeType As BCType)
'    Public Shared BCReaderStatus As String


'    Private Sub New()
'        '#If DEVICE = True Then
'        'Initialise Barcode Reader
'        Me.InitBCReader()
'        '#End If

'    End Sub
'    Public Shared Function GetInstance() As BCReader
'        If objBCReader Is Nothing Then
'            objBCReader = New BCReader()
'            Return objBCReader
'        Else
'            Return objBCReader
'        End If
'    End Function
'    ''' <summary>
'    ''' InitBCReader intialises the Barcode Reader hardware
'    ''' </summary>
'    ''' <returns>Ture is succesfull and False if failure</returns>
'    ''' <remarks></remarks>
'    Private Function InitBCReader()

'        'If Not (Me.hndlReader Is Nothing) Then
'        '    Return False
'        'End If

'        'Try

'        '    'Get Selected device from user
'        '    Dim MyDevice As Symbol.Generic.Device _
'        '        = Symbol.StandardForms.SelectDevice.Select( _
'        '                                Symbol.Barcode.Device.Title, _
'        '                                Symbol.Barcode.Device.AvailableDevices)
'        '    If (MyDevice Is Nothing) Then
'        '        MessageBox.Show("No Device Selected", "SelectDevice")
'        '        Return False
'        '    End If

'        '    'create the reader, based on selected device
'        '    Me.hndlReader = New Symbol.Barcode.Reader(MyDevice)

'        '    ' Create reader data
'        '    Me.hndlReaderData = New Symbol.Barcode.ReaderData( _
'        '     Symbol.Barcode.ReaderDataTypes.Text, _
'        '     Symbol.Barcode.ReaderDataLengths.DefaultText)

'        '    'Enable reader, with wait cursor
'        '    Me.hndlReader.Actions.Enable()

'        '    'Set maximum length of I2OF5 barcode as 12 for scaning SELs.
'        '    Me.hndlReader.Decoders.I2OF5.MaximumLength = Macros.MAX_I2OF5_LEN

'        '    'create event handler
'        '    Me.ReadNotifyHandler = New EventHandler(AddressOf hndlReader_ReadNotify)
'        '    Me.StatusNotifyHandler = New EventHandler(AddressOf hndlReader_StatusNotify)

'        '    Return True

'        'Catch ex As Symbol.Exceptions.InvalidRequestException
'        '    MessageBox.Show("InitReader\n" + "Invalid Operation\n" + ex.Message())
'        '    Return False

'        'Catch ex As Symbol.Exceptions.OperationFailureException
'        '    MessageBox.Show("InitReader\n" + "Operation Failure" + ex.Message())
'        '    Return False

'        'Catch ex As Symbol.Exceptions.UnimplementedFunctionException
'        '    MessageBox.Show("InitReader\n" + "Unimplemented Function" + ex.Message())
'        '    Return False

'        'End Try
'    End Function
'    'Stop reading and disable/close reader

'    Public Sub TerminateBCReader()
'        ''#If DEVICE = True Then
'        ''If we have a reader
'        'If Not (Me.hndlReader Is Nothing) Then

'        '    'stop all notifications
'        '    Me.StopRead()
'        '    Me.StopStatus()
'        '    Try

'        '        'Disable the Reader
'        '        Me.hndlReader.Actions.Disable()

'        '        'free it up and assign nothing 
'        '        Me.hndlReader.Dispose()

'        '        'Indiacte we no longer have one
'        '        Me.hndlReader = Nothing

'        '    Catch ex As Symbol.Exceptions.InvalidRequestException
'        '        MessageBox.Show("InitReader\n" + "Invalid Operation\n" + ex.Message())

'        '    Catch ex As Symbol.Exceptions.OperationFailureException

'        '        MessageBox.Show("InitReader\n" + "Operation Failure" + ex.Message())

'        '    Catch ex As Symbol.Exceptions.UnimplementedFunctionException

'        '        MessageBox.Show("InitReader\n" + "Unimplemented Function" + ex.Message())

'        '    End Try

'        'End If

'        'If Not (Me.hndlReaderData Is Nothing) Then

'        '    'Free it up and assign nothing
'        '    Me.hndlReaderData.Dispose()

'        '    'Indicate we no longer have one
'        '    Me.hndlReaderData = Nothing

'        'End If
'        ''#End If
'    End Sub

'    'Status notification handler

'    Private Sub hndlReader_StatusNotify(ByVal sender As Object, ByVal e As EventArgs)

'        ''Get Current status
'        'Dim ScanEvent As Symbol.Barcode.BarcodeStatus = Me.hndlReader.GetNextStatus

'        ''set Status to Global.
'        'BCReaderStatus = ScanEvent.Text

'    End Sub

'    'Read notification handler

'    Private Sub hndlReader_ReadNotify(ByVal sender As Object, ByVal e As EventArgs)

'        ''Get ReaderData
'        'Dim ScanData As Symbol.Barcode.ReaderData = Me.hndlReader.GetNextReaderData

'        'Select Case ScanData.Result

'        '    Case Symbol.Results.SUCCESS

'        '        'Handle the data from this read

'        '        Me.HandleData(ScanData)

'        '        '

'        '    Case Symbol.Results.CANCELED

'        '    Case Else

'        '        'Dim sMsg As String
'        '        'sMsg = "Read Failed\n" + "Result = " + (CInt(ScanData.Result)).ToString("X8")
'        '        'MessageBox.Show(sMsg, "ReadNotify")

'        'End Select
'    End Sub

'    'Handle data from the reader

'    Private Sub HandleData(ByVal ScanData As Symbol.Barcode.ReaderData)

'        'Dim barcodeType As BCType = BCType.None
'        'Dim bValidCode As Boolean = False

'        'Select Case ScanData.Type
'        '    Case Symbol.Barcode.DecoderTypes.EAN128 '' Write code to handle the different types of barcodes for SM and Goods Out.
'        '        bValidCode = True
'        '        barcodeType = BCType.EAN
'        '    Case Symbol.Barcode.DecoderTypes.CODE128
'        '        bValidCode = True
'        '        barcodeType = BCType.EAN
'        '    Case Symbol.Barcode.DecoderTypes.EAN13
'        '        bValidCode = True
'        '        barcodeType = BCType.EAN
'        '    Case Symbol.Barcode.DecoderTypes.EAN8
'        '        bValidCode = True
'        '        barcodeType = BCType.EAN
'        '    Case Symbol.Barcode.DecoderTypes.I2OF5
'        '        bValidCode = True
'        '        barcodeType = BCType.SEL
'        '    Case Symbol.Barcode.DecoderTypes.UPCA
'        '        bValidCode = True
'        '        'barcodeType = BCType.UPC
'        '        barcodeType = BCType.EAN    'Changed to support 12-digit product codes.
'        'End Select
'        'If bValidCode = True Then
'        '    If (barcodeType = BCType.EAN) Then
'        '        'Padding the barcode to 13 Digits.
'        '        RaiseEvent evtBCScanned((ScanData.Text).PadLeft(13, "0"), barcodeType)
'        '    Else
'        '        RaiseEvent evtBCScanned(ScanData.Text, barcodeType)
'        '    End If

'        'Else
'        '    MessageBox.Show("Invalid Barcode scanned", "Error", MessageBoxButtons.OK, _
'        '        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
'        '    Me.StartRead()
'        'End If

'    End Sub
'    ' start a read on reader

'    Public Sub StartRead()
'        ''#If DEVICE = True Then
'        ''if we have both a reader and readerdata
'        'If Not ((Me.hndlReader Is Nothing) And (Me.hndlReaderData Is Nothing)) Then

'        '    Try

'        '        AddHandler hndlReader.ReadNotify, Me.ReadNotifyHandler

'        '        Me.hndlReader.Actions.Flush()
'        '        'Submit a read
'        '        Me.hndlReader.Actions.Read(Me.hndlReaderData)

'        '    Catch ex As Symbol.Exceptions.UnimplementedFunctionException
'        '        MessageBox.Show("StartRead\n" + "Unimplemented Function\n" + ex.Message())

'        '    Catch ex As Symbol.Exceptions.InvalidIndexerException
'        '        MessageBox.Show("StartRead\n" + "Invalid Indexer\n" + ex.Message())

'        '    Catch ex As Symbol.Exceptions.OperationFailureException
'        '        MessageBox.Show("StopRead\n" + "Operation Failure\n" + "Result = 0x" + (ex.Result).ToString("X8") + "\n" + ex.Message())

'        '    Catch ex As Symbol.Exceptions.InvalidRequestException
'        '        MessageBox.Show("StartRead\n" + "Invalid Request\n" + ex.Message)

'        '    End Try

'        'End If
'        ''#End If
'    End Sub

'    ' stop all the read

'    Public Sub StopRead()
'        ''#If DEVICE = True Then
'        ''if we do not have a reader, then do nothing
'        'If (Me.hndlReader Is Nothing) Then

'        '    Return

'        'End If
'        'Try

'        '    'remove read notification handler
'        '    RemoveHandler hndlReader.ReadNotify, Me.ReadNotifyHandler

'        '    'Flush (Cancel all pending reads)
'        '    Me.hndlReader.Actions.Flush()

'        'Catch ex As Symbol.Exceptions.UnimplementedFunctionException
'        '    MessageBox.Show("StopRead\n" + "Unimplemented Function\n" + ex.Message())

'        'Catch ex As Symbol.Exceptions.InvalidRequestException
'        '    MessageBox.Show("StopRead\n" + "Invalid Request\n" + ex.Message())

'        'Catch ex As Symbol.Exceptions.OperationFailureException
'        '    MessageBox.Show("StopRead\n" + "Operation Failure\n" + "Result = 0x" + (ex.Result).ToString("X8") + "\n" + ex.Message())

'        'End Try
'        ''#End If
'    End Sub

'    'Start Status notification

'    Private Sub StartStatus()

'        'If Not (Me.hndlReader Is Nothing) Then

'        '    'Attach  status notification handler
'        '    AddHandler hndlReader.StatusNotify, Me.StatusNotifyHandler

'        'End If

'    End Sub

'    'Stop all status notifications

'    Private Sub StopStatus()

'        'If Not (Me.hndlReader Is Nothing) Then

'        '    'detach status notification handler
'        '    RemoveHandler hndlReader.StatusNotify, Me.StatusNotifyHandler

'        'End If

'    End Sub


'    Public Sub EnableDecoder(ByVal DecoderType As Symbol.Barcode.DecoderTypes)
'        ''#If DEVICE = True Then
'        'Select Case DecoderType
'        '    Case Symbol.Barcode.DecoderTypes.I2OF5
'        '        hndlReader.Decoders.I2OF5.Enabled = True
'        '    Case Symbol.Barcode.DecoderTypes.CODE128
'        '        hndlReader.Decoders.CODE128.Enabled = True
'        '    Case Symbol.Barcode.DecoderTypes.EAN8
'        '        hndlReader.Decoders.EAN8.Enabled = True
'        '    Case Symbol.Barcode.DecoderTypes.EAN13
'        '        hndlReader.Decoders.EAN13.Enabled = True
'        '    Case Symbol.Barcode.DecoderTypes.UPCA
'        '        hndlReader.Decoders.UPCA.Enabled = True
'        '    Case Symbol.Barcode.DecoderTypes.CODE93
'        '        hndlReader.Decoders.CODE39.Enabled = True
'        'End Select
'        ''#End If

'    End Sub

'    Public Sub DisableDecoder(ByVal DecoderType As Symbol.Barcode.DecoderTypes)
'        ''#If DEVICE = True Then
'        'Select Case DecoderType
'        '    Case Symbol.Barcode.DecoderTypes.I2OF5
'        '        hndlReader.Decoders.I2OF5.Enabled = False
'        '    Case Symbol.Barcode.DecoderTypes.CODE128
'        '        hndlReader.Decoders.CODE128.Enabled = False
'        '    Case Symbol.Barcode.DecoderTypes.CODE93
'        '        hndlReader.Decoders.CODE39.Enabled = False
'        '    Case Symbol.Barcode.DecoderTypes.EAN8
'        '        hndlReader.Decoders.EAN8.Enabled = False
'        '    Case Symbol.Barcode.DecoderTypes.EAN13
'        '        hndlReader.Decoders.EAN13.Enabled = False
'        '    Case Symbol.Barcode.DecoderTypes.UPCA
'        '        hndlReader.Decoders.UPCA.Enabled = False
'        'End Select
'        ''#End If
'    End Sub
'    Public Sub EventBCScannedHandler(ByVal strBarcode As String, ByVal Type As BCType)
'        ''#If DEVICE = True Then
'        '' Write code to handle the event raised when a Barcode is scanned, depending on from which module it was scanned. 
'        'Select Case objAppContainer.objActiveModule
'        '    Case AppContainer.ACTIVEMODULE.SHLFMNTR
'        '        SMSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
'        '    Case AppContainer.ACTIVEMODULE.FASTFILL
'        '        FFSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
'        '    Case AppContainer.ACTIVEMODULE.CUNTLIST
'        '        CLSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
'        '    Case AppContainer.ACTIVEMODULE.EXCSSTCK
'        '        EXSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
'        '    Case AppContainer.ACTIVEMODULE.ITEMINFO
'        '        ItemInfoSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
'        '    Case AppContainer.ACTIVEMODULE.PICKGLST
'        '        PLSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
'        '    Case AppContainer.ACTIVEMODULE.SPCEPLAN
'        '        SPSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
'        '    Case AppContainer.ACTIVEMODULE.PRICECHK
'        '        PCSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
'        '    Case AppContainer.ACTIVEMODULE.PRINTSEL
'        '        PSSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
'        '    Case AppContainer.ACTIVEMODULE.AUTOSTUFFYOURSHELVES
'        '        AutoSYSSessionManager.GetInstance().HandleScanData(strBarcode, Type)
'        'End Select
'        ''#End If
'    End Sub

'End Class
'#End If

Public Enum BCType
    SEL
    ManualEntry
    EAN
    UPC
    CLRLBL              'Added for Darwin
    None
End Enum

