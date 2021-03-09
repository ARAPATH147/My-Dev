Imports Goodsin.GIValueHolder
Public Class SendGIA
    Private strData As String
    Private strPreviosGIA As String
    Dim m_arrUODList As New ArrayList
    Public Sub New()
        strData = ""
        strPreviosGIA = ""
    End Sub
    Public Function SendGIA(ByVal strDataTemp As String) As Boolean
        strData = strDataTemp
        'If strData <> strPreviosGIA AndAlso strData.Substring(10, 6) = strPreviosGIA.Substring(10, 6) Then
        '    'send data already fetched
        'End If
        Dim bReturn As Boolean = False
        Select Case strData.Substring(6, 1)
            Case RFDataStructure.DeliveryType.SSCReceiving.ToString
                Return SendGIA_SSC()
                'Case RFDataStructure.DeliveryType.DirectsReceiving.ToString
                '    Return SendGIA_Direct()
                'Case "X"
                '    Return SendGIA_Start()
            Case Else
                Return bReturn
        End Select
        ' if previous GIA there then return based on the list already available
    End Function
    Public Function SendGIA_SSC() As Boolean
        Select Case strData.Substring(6, 1)
            Case RFDataStructure.GFunction.BookIn.ToString()
                Select Case strData.Substring(8, 1)
                    Case "S"
                        ProcessSSCBookin()
                        Return True
                    Case Else
                        Return False
                End Select
            Case RFDataStructure.GFunction.Audit.ToString()
                Select Case strData.Substring(8, 1)
                    'Case "S"
                    '    objServer.SendACK()
                    '    Return True
                    Case Else
                        Return False
                End Select
            Case RFDataStructure.GFunction.View.ToString()
                Select Case strData.Substring(8, 1)
                    Case "L"

                        Select Case strData.Substring(9, 1)
                            Case "T"
                                ProcessSSCView("T")
                                Return True
                            Case "F"
                                ProcessSSCView("F")
                                Return True
                            Case Else
                                Return False
                        End Select
                    Case Else
                        Return False
                End Select
            Case Else
                Return False
        End Select
    End Function
    'Public Function ProcessGIA_Direct() As Boolean
    '    Select Case strData.Substring(6, 1)
    '        Case GFunction.BookIn.ToString()
    '            Select Case strData.Substring(8, 1)
    '                Case "S"
    '                Case "L"
    '                Case Else
    '                    Return False
    '            End Select
    '        Case GFunction.Audit.ToString()
    '            Select Case strData.Substring(8, 1)
    '                Case "S"
    '                    objServer.SendACK()
    '                    Return True
    '                Case Else
    '                    Return False
    '            End Select
    '        Case GFunction.View.ToString()
    '            Select Case strData.Substring(8, 1)
    '                Case "S"

    '                Case Else
    '                    Return False
    '            End Select
    '        Case Else
    '            Return False
    '    End Select
    'End Function
    Public Function ProcessGIA_Start() As Boolean
        Select Case strData.Substring(6, 1)
            Case "X"
                Select Case strData.Substring(8, 1)
                    Case "C"
                        'Get config and send it
                    Case Else
                        Return False
                End Select
            Case Else
                Return False
        End Select
    End Function
    Public Sub ProcessSSCBookin()
        Dim arrList As New ArrayList
        '     objDataEngine.GetBookInDeliverySummary(arrList)
        Dim strResponse As String = ""
        strResponse = Left(strData, 6)
        strResponse += "S"
        strResponse += arrList.Count.ToString().PadLeft(2, "0")
        strResponse += "0000-1" 'DD say's send -1
        For Each objDeliverySummary As GIValueHolder.DeliverySummary In arrList
            strResponse += objDeliverySummary.SummaryType.PadRight(10, " ")
            strResponse += "XXXXX" '5
            strResponse += objDeliverySummary.ContainerType.PadLeft(20, " ")
            strResponse += "X"
            strResponse += "X"
            strResponse += "XXXXXX" 'need to populate but not in DB querry right now
            strResponse += "U" 'Only unbooked list
            strResponse += objDeliverySummary.ContainerQty.ToString().PadLeft(6, "0")
        Next
        'Hope the below statement is not required
        'strResponse.PadRight(1055," ")
        '  objServer.SendMessage(strResponse)
    End Sub
    Public Sub ProcessSSCView(ByVal strPeriod As String)
        Dim iPointer As Integer = 0
        Dim iArraylistCount As Integer = 0
        If strData <> strPreviosGIA AndAlso strData.Substring(0, 10) = strPreviosGIA.Substring(0, 10) Then
            'send data already fetched
            iPointer = CType(strData.Substring(10, 6).TrimStart("0"), Integer)
        Else
            m_arrUODList.Clear()
            '   objDataEngine.GetUODListForView(strPeriod, m_arrUODList)
            iPointer = 0
        End If
        iArraylistCount = m_arrUODList.Count

        Dim strResponse As String = ""
        strResponse = Left(strData, 6)
        strResponse += "S"
        strResponse += IIf((iArraylistCount - iPointer) <= 20, "000020", (iArraylistCount - iPointer).ToString().PadLeft(6, "0"))
        strResponse += IIf((iArraylistCount - iPointer) <= 20, "0000-1", (iPointer + 20).ToString().PadLeft(6, "0")) '"0000-1" 'DD say's send -1

        For i As Integer = iPointer To iArraylistCount
            Dim objUODList As UODList = m_arrUODList(i)
            strResponse += objUODList.UODID.PadLeft(10, "0")
            'need to verify sequence number ------------------------------
            strResponse += "XXXXX" '5
            strResponse += objUODList.UODType.PadLeft(20, " ")
            strResponse += "X"
            'This value depends on the container, passing C for time being -------------------
            strResponse += "C"
            strResponse += objUODList.ExptDate
            strResponse += objUODList.BookedIn
            strResponse += "XXXXXX"
            i += 1
            If ((i - iPointer) = 20) Then
                strPreviosGIA = strData
                Exit For
            ElseIf (iArraylistCount = i) Then
                strPreviosGIA = ""
                Exit For
            End If
        Next
        'Hope the below statement is not required
        'strResponse.PadRight(1055," ")
        ' objServer.SendMessage(strResponse)
    End Sub
    Public Sub ProcessSSCView()
        Dim m_SupplierList As New ArrayList
        '     objDataEngine.GetSupplierList(m_SupplierList)

        Dim strResponse As String = ""
        strResponse = Left(strData, 6)
        strResponse += "S"

        'DD says the list of Suppliers cannot be more than 15
        strResponse += m_SupplierList.Count.ToString().PadLeft(2, "0")
        strResponse += "0000-1" 'DD say's send -1
        For Each objSupplier As SupplierList In m_SupplierList
            strResponse += objSupplier.SupplierNo.PadLeft(10, " ")
            strResponse += "XXXXX" '5
            strResponse += objSupplier.SupplierName.PadLeft(20, " ")
            strResponse += objSupplier.SupplierType
            strResponse += "X"
            strResponse += "XXXXXX" 'need to populate but not in DB querry right now
            strResponse += "U" 'Only unbooked list
            strResponse += objSupplier.SupplierQty.ToString().PadLeft(6, "0")
        Next
        'Hope the below statement is not required
        'strResponse.PadRight(1055," ")
        '  objServer.SendMessage(strResponse)
    End Sub

End Class
