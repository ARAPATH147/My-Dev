Public Class BCReader
    'Barcode Reader
    Private Shared objBCReader As BCReader

    Private hndlReader As Symbol.Barcode.Reader = Nothing
    Private hndlReaderData As Symbol.Barcode.ReaderData = Nothing
    Private readerEventHandler As System.EventHandler = Nothing
    Public Shared PublicBarCodeType As String ' 23/3/05 PAB

    'Events
    Public Event evtBCScanned(ByVal barcode As String, ByVal BarcodeType As BCType)


#Region " BarcodeScanner "
    'Initialize the reader.

    Private Sub New()
        'Mark Goode - remove when released
        '#If Not MakeDebug Then
        InitBCReader()                                'DEBUGCSK put back in for testing on PPC
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

    Public Function InitBCReader() As Boolean

        ' If reader is already present then fail initialize
        If Not (Me.hndlReader Is Nothing) Then

            Return False

        End If

        'Create new reader, first available reader will be used.
        Me.hndlReader = New Symbol.Barcode.Reader

        'Create reader data

        Me.hndlReaderData = New Symbol.Barcode.ReaderData( _
                                   Symbol.Barcode.ReaderDataTypes.Text, _
                                   Symbol.Barcode.ReaderDataLengths.DefaultText)


        ' create event handler delegate
        Me.readerEventHandler = New System.EventHandler(AddressOf MyReader_ReadNotify)

        'Enable reader, with wait cursor
        Me.hndlReader.Actions.Enable()

        'need to enable for I2OF5 barcodes
        Me.hndlReader.Decoders.I2OF5.MaximumLength = 12
        'need to enable for I2OF5 barcodes
        Me.hndlReader.Decoders.I2OF5.MinimumLength = 8
        Return True

    End Function

    'Stop reading and disable/close reader

    Public Sub TerminateBCReader()

        'If we have a reader
        If Not (Me.hndlReader Is Nothing) Then

            'Disable reader, with wait cursor
            Me.hndlReader.Actions.Disable()

            'free it up
            Me.hndlReader.Dispose()

            ' Indicate we no longer have one
            Me.hndlReader = Nothing

        End If

        ' If we have a reader data
        If Not (Me.hndlReaderData Is Nothing) Then

            'Free it up
            Me.hndlReaderData.Dispose()

            'Indicate we no longer have one
            Me.hndlReaderData = Nothing

        End If

    End Sub

    'Start a read on the reader

    Public Sub StartRead()
        Try

        
        'If we have both a reader and a reader data
        If Not ((Me.hndlReader Is Nothing) And (Me.hndlReaderData Is Nothing)) Then

            'Only read if we are not in a simulation (emulator)
            If Not Me.hndlReader.Info.IsSimulating Then

                'Submit a read
                AddHandler hndlReader.ReadNotify, Me.readerEventHandler

                Me.hndlReader.Actions.Read(Me.hndlReaderData)

            End If

            End If
        Catch ex As Exception
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at StartRead in BCREader: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try

    End Sub

    'Stop all reads on the reader

    Public Sub StopRead()

        'If we have a reader
        If Not (Me.hndlReader Is Nothing) Then

            'Flush (Cancel all pending reads)
            RemoveHandler hndlReader.ReadNotify, Me.readerEventHandler

            Me.hndlReader.Actions.Flush()

        End If

    End Sub

    'Read complete or failure notification

    Private Sub MyReader_ReadNotify(ByVal o As Object, ByVal e As EventArgs)

        Dim TheReaderData As Symbol.Barcode.ReaderData = Me.hndlReader.GetNextReaderData()

        'If it is a successful read (as opposed to a failed one)
        If (TheReaderData.Result = Symbol.Results.SUCCESS) Then
            'Start the reader
            Me.StartRead()
            'Handle the data from this read
            Me.HandleData(TheReaderData)

        End If

    End Sub

    'Handle data from the reader

    Private Sub HandleData(ByVal ScanData As Symbol.Barcode.ReaderData)

        Dim barcodeType As BCType = BCType.None
        Dim bValidCode As Boolean = False

        Select Case ScanData.Type
            Case Symbol.Barcode.DecoderTypes.EAN128
                bValidCode = True
                barcodeType = BCType.EAN
            Case Symbol.Barcode.DecoderTypes.CODE128
                bValidCode = True
                barcodeType = BCType.CODE128
            Case Symbol.Barcode.DecoderTypes.EAN13
                bValidCode = True
                barcodeType = BCType.EAN

            Case Symbol.Barcode.DecoderTypes.EAN8
                bValidCode = True
                barcodeType = BCType.EAN

            Case Symbol.Barcode.DecoderTypes.I2OF5
                bValidCode = True
                barcodeType = BCType.I2O5
            Case Symbol.Barcode.DecoderTypes.UPCA
                bValidCode = True
                barcodeType = BCType.EAN

        End Select
        If bValidCode = True Then
            ' RaiseEvent evtBCScanned(ScanData.Text, barcodeType)
            If (barcodeType = BCType.EAN) Then
                'Padding the barcode to 13 Digits.
                RaiseEvent evtBCScanned((ScanData.Text).PadLeft(13, "0"), barcodeType)
            Else
                RaiseEvent evtBCScanned(ScanData.Text, barcodeType)
            End If
        Else
            MessageBox.Show("Invalid Barcode scanned", "Error", MessageBoxButtons.OK, _
                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            Me.StartRead()
        End If

    End Sub
    Public Sub EnableDecoder(ByVal DecoderType As Symbol.Barcode.DecoderTypes)
        Select Case DecoderType
            Case Symbol.Barcode.DecoderTypes.I2OF5
                hndlReader.Decoders.I2OF5.Enabled = True
            Case Symbol.Barcode.DecoderTypes.CODE128
                hndlReader.Decoders.CODE128.Enabled = True
            Case Symbol.Barcode.DecoderTypes.EAN128
                hndlReader.Decoders.CODE128.Enabled = True

        End Select


    End Sub

    Public Sub DisableDecoder(ByVal DecoderType As Symbol.Barcode.DecoderTypes)
        Select Case DecoderType
            Case Symbol.Barcode.DecoderTypes.I2OF5
                hndlReader.Decoders.I2OF5.Enabled = False
            Case Symbol.Barcode.DecoderTypes.CODE128
                hndlReader.Decoders.CODE128.Enabled = False
            Case Symbol.Barcode.DecoderTypes.EAN128
                hndlReader.Decoders.CODE128.Enabled = False

        End Select
    End Sub
    Public Sub EventBCScannedHandler(ByVal strBarcode As String, ByVal Type As BCType)
        ' Write code to handle the event raised when a Barcode is scanned, depending on from which module it was scanned. 
        Select Case objAppContainer.objActiveModule
            Case AppContainer.ACTIVEMODULE.AUDITUOD
                AUODSessionManager.GetInstance().HandleScanData(strBarcode, Type)
            Case AppContainer.ACTIVEMODULE.AUDITCARTON
                ACSessionManager.GetInstance().HandleScanData(strBarcode, Type)
            Case AppContainer.ACTIVEMODULE.BOOKINDELIVERY
                BDSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
            Case AppContainer.ACTIVEMODULE.VCARTON
                VCSessionManager.GetInstance.HandleScanData(strBarcode, Type)
            Case AppContainer.ACTIVEMODULE.VUOD
                VUODSessionMgr.GetInstance.HandleScanData(strBarcode, Type)
            Case AppContainer.ACTIVEMODULE.BOOKINCARTON
                BCSessionMgr.GetInstance().HandleScanData(strBarcode, Type)
                '    'Dim a As Integer = 0
            Case Else

        End Select
    End Sub
#End Region
End Class
Public Enum BCType
    I2O5
    ManualEntry
    EAN
    UPC
    CODE128
    None
End Enum
