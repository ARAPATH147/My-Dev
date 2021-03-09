'''****************************************************************************
''' <FileName> RecordFunc.vb </FileName>
''' <summary>
''' Functions for processing records from Transact
''' </summary> 
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

Public Class RecordFunc

    ''' <summary>
    ''' Extracts parcel record into its component fields
    ''' </summary>
    ''' <param name="cRecord">String containing complete Parcel record</param>
    ''' <param name="parcel">Parcel record structure</param>
    ''' <returns>Structure Parcel Record</returns>
    Public Shared Function processParcelRecord(ByVal cRecord As String, _
             ByVal parcel As ParcelSession.ParcelRecord) As ParcelSession.ParcelRecord
        With parcel
            .cSupplierNumber = cRecord.Substring(3, 6)
            .cParcelNumber = cRecord.Substring(9, 8)
            .cParentOrderNumber = cRecord.Substring(17, 10)
            .cExpectedDelivery = cRecord.Substring(27, 6)
            .cCurrentStatus = cRecord.Substring(33, 1)
            .cDeliveryDateTime = cRecord.Substring(34, 12)
            .cIsDeliveryExported = cRecord.Substring(46, 1)
            .cCollectedDateTime = cRecord.Substring(47, 12)
            .cCollectedReasonCode = cRecord.Substring(59, 1)
            .cIsCollectionExported = cRecord.Substring(60, 1)
            .cReturnedToCentreDateTime = cRecord.Substring(61, 12)
            .cIsReturnToCentreExported = cRecord.Substring(73, 1)
            .cLostDateTime = cRecord.Substring(74, 12)
            .cIsLostEventExported = cRecord.Substring(86, 1)
            .cFoundDateTime = cRecord.Substring(87, 12)
            .cIsFoundEventExported = cRecord.Substring(99, 1)
            .cLocationCurrent = cRecord.Substring(100, 4)
            .cLocationStatus = cRecord.Substring(104, 4)
            .cFiller = cRecord.Substring(108, 45)
        End With
        Return parcel
    End Function

    ''' <summary>
    ''' Extracts Order record into its component fields
    ''' </summary>
    ''' <param name="cRecord">String containing complete Order record</param>
    ''' <param name="order">Order record structure</param>
    ''' <returns>Structure Order Record</returns>
    Public Shared Function processOrderRecord(ByVal cRecord As String, _
                ByVal order As ParcelSession.OrderRecord) As ParcelSession.OrderRecord

        Dim fieldLength As Integer = 8 'Length of parcel number.

        order.cSupplierNumber = cRecord.Substring(3, 6)
        order.cOrderNumber = cRecord.Substring(9, 10)
        order.Cartons = New List(Of String)

        For iIndex As Integer = 19 To 458 Step fieldLength
            'only add valid parcel numbers to list 
            If cRecord.Substring(iIndex, fieldLength) <> "00000000" Then
                order.Cartons.Add(cRecord.Substring(iIndex, fieldLength))
            End If
        Next
        order.cFiller = cRecord.Substring(459, 26)
        Return order
    End Function

    ''' <summary>
    ''' Extracts Location record into its component fields
    ''' </summary>
    ''' <param name="cRecord">String containing complete Location record</param>
    ''' <param name="Location">Location record structure</param>
    ''' <returns>Structure Location Record</returns>
    Public Shared Function processLocationRecord(ByVal cRecord As String, _
              ByVal Location As ParcelSession.LocationRecord) As ParcelSession.LocationRecord

        With Location
            .cStatus = cRecord.Substring(3, 1)
            .cShortDescription = cRecord.Substring(4, 10)
            .cLongDescription = cRecord.Substring(14, 20)
            .cParcelCount = cRecord.Substring(34, 4)
            .cFiller = cRecord.Substring(38, 13)
        End With
        Return Location
    End Function

    ''' <summary>
    ''' Assigns latest Parcel structure to Current Parcel structure
    ''' </summary>
    ''' <param name="parcel">String containing current parcel record</param>
    ''' <returns>Structure Current Parcel Record</returns>
    Public Shared Function saveToCurrentParcel(ByVal parcel As ParcelSession.ParcelRecord) As ParcelSession.CurrentParcel
        Dim currentParcel As New ParcelSession.CurrentParcel
        With currentParcel
            .cSupplierNumber = parcel.cSupplierNumber
            .cParcelNumber = parcel.cParcelNumber
            .cParentOrderNumber = parcel.cParentOrderNumber
            .cExpectedDelivery = parcel.cExpectedDelivery
            .cCurrentStatus = parcel.cCurrentStatus
            .cDeliveryDateTime = parcel.cDeliveryDateTime
            .cIsDeliveryExported = parcel.cIsDeliveryExported
            .cCollectedDateTime = parcel.cCollectedDateTime
            .cCollectedReasonCode = parcel.cCollectedReasonCode
            .cIsCollectionExported = parcel.cIsCollectionExported
            .cReturnedToCentreDateTime = parcel.cReturnedToCentreDateTime
            .cIsReturnToCentreExported = parcel.cIsReturnToCentreExported
            .cLostDateTime = parcel.cLostDateTime
            .cIsLostEventExported = parcel.cIsLostEventExported
            .cFoundDateTime = parcel.cFoundDateTime
            .cIsFoundEventExported = parcel.cIsFoundEventExported
            .cLocationCurrent = parcel.cLocationCurrent
            .cLocationStatus = parcel.cLocationStatus
        End With

        Return currentParcel
    End Function

    ''' <summary>
    ''' Builds Parcel record that will be used to update server via OCU message
    ''' </summary>
    ''' <param name="oParcel">String containing current parcel record</param>
    ''' <returns>String</returns>
    Public Shared Function buildParcelRecord(ByVal oParcel As ParcelSession.CurrentParcel)
        Dim cRecord As String = ""
        'oParcel.cFiller = New String(" ", 45)
        cRecord = oParcel.cSupplierNumber + _
                  oParcel.cParcelNumber + _
                  oParcel.cParentOrderNumber + _
                  oParcel.cLocationCurrent + _
                  oParcel.cCurrentStatus
        Return cRecord
    End Function

End Class
