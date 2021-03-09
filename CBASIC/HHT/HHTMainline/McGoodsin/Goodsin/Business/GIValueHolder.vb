'''******************************************************************************
'''* Modification Log 
'''******************************************************************************* 
'''* No:      Author            Date                 Description 
'''* 1.1     Christopher Kitto  09/04/2015  Modified as part of DALLAS project. Added
'''                  (CK)                  new value class DallasDeliverySummary under 
'''                                        GIValueHolder class to hold the data of Dallas 
'''                                        UODs obtained from DAD message. Added new value
'''                                        class DallasScanDetail
'''********************************************************************************

Public Class GIValueHolder
    ''' <summary>
    ''' value class to hold the delivery summary details
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DeliverySummary
        ''' <summary>
        ''' Member variables
        ''' </summary>
        ''' <remarks></remarks>
        Private m_SummaryType As String
        Private m_ContainerType As String
        Private m_ContainerQty As Integer
        ''' <summary>
        ''' Delivery Type will be E/O
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SummaryType() As String
            Get
                Return m_SummaryType
            End Get
            Set(ByVal value As String)
                m_SummaryType = value
            End Set
        End Property
        ''' <summary>
        ''' Container Type can be D/C/R/O/I
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ContainerType() As String
            Get
                Return m_ContainerType
            End Get
            Set(ByVal value As String)
                m_ContainerType = value
            End Set
        End Property
        Public Property ContainerQty() As Integer
            Get
                Return m_ContainerQty
            End Get
            Set(ByVal value As Integer)
                m_ContainerQty = value
            End Set
        End Property
    End Class
    ''' <summary>
    ''' Value Class to hold data of the UOD Scanned
    ''' </summary>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Value Class to hold data of the UOD Scanned
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ScanDetails
        ''' <summary>
        ''' Member Variables
        ''' </summary>
        ''' <remarks></remarks>
        Private m_ScannedCode As String
        Private m_DespatchDate As String
        Private m_ScanDate As String
        Private m_ScanTime As String
        Private m_ScanType As String
        Private m_ScanLevel As String
        Private m_ProductCode As String
        Private m_BootCode As String
        Private m_ItemQty As String
        Private m_ItemStatus As String
        Private m_ParentCode As String
        Private m_Rejected As Char
        Public Property Rejected() As String
            Get
                Return m_Rejected
            End Get
            Set(ByVal value As String)
                m_Rejected = value
            End Set
        End Property
        Public Property Parent() As String
            Get
                Return m_ParentCode
            End Get
            Set(ByVal value As String)
                m_ParentCode = value
            End Set
        End Property
        Public Property ItemStatus() As String
            Get
                Return m_ItemStatus
            End Get
            Set(ByVal value As String)
                m_ItemStatus = value
            End Set
        End Property

        Public Property ItemQty() As String
            Get
                Return m_ItemQty
            End Get
            Set(ByVal value As String)
                m_ItemQty = value
            End Set
        End Property

        Public Property BootCode() As String
            Get
                Return m_BootCode
            End Get
            Set(ByVal value As String)
                m_BootCode = value
            End Set
        End Property


        Public Property ProductCode() As String
            Get
                Return m_ProductCode
            End Get
            Set(ByVal value As String)
                m_ProductCode = value
            End Set
        End Property

        Public Property ScannedCode() As String
            Get
                Return m_ScannedCode
            End Get
            Set(ByVal value As String)
                m_ScannedCode = value
            End Set
        End Property
        ''' <summary>
        ''' Despatch Date in YYMMDD Format
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DespatchDate() As String
            Get
                Return m_DespatchDate
            End Get
            Set(ByVal value As String)
                m_DespatchDate = value
            End Set
        End Property
        ''' <summary>
        ''' Scan Date in YYMMDD Format
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ScanDate() As String
            Get
                Return m_ScanDate
            End Get
            Set(ByVal value As String)
                m_ScanDate = value
            End Set
        End Property
        Public Property ScanTime() As String
            Get
                Return m_ScanTime
            End Get
            Set(ByVal value As String)
                m_ScanTime = value
            End Set
        End Property
        ''' <summary>
        ''' Scan time in 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ScanType() As String
            Get
                Return m_ScanType
            End Get
            Set(ByVal value As String)
                m_ScanType = value
            End Set
        End Property
        Public Property ScanLevel() As String
            Get
                Return m_ScanLevel
            End Get
            Set(ByVal value As String)
                m_ScanLevel = value
            End Set
        End Property
    End Class
    ' V1.1 - CK
    ' Added new class DallasScanDetail

    ''' <summary>
    ''' Value Class to hold data of the Dallas UOD Scanned
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DallasScanDetail
        ''' <summary>
        ''' Member Variables  
        ''' </summary>
        ''' <remarks></remarks>
        Private m_cDallasBarcode As String
        Private m_cScanStatus As String
        Private m_cDallasExpectedDate As String
        Private m_cDallasScanDate As String
        ''' <summary>
        ''' Scanned Dallas barcode
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DallasBarcode() As String
            Get
                Return m_cDallasBarcode
            End Get
            Set(ByVal value As String)
                m_cDallasBarcode = value
            End Set
        End Property
        ''' <summary>
        ''' Scan status - 'R' for receipted / 'B' for banked
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ScanStatus() As String
            Get
                Return m_cScanStatus
            End Get
            Set(ByVal value As String)
                m_cScanStatus = value
            End Set
        End Property

        ''' <summary>
        ''' Dallas Scan Date in YYMMDD Format
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DallasScanDate() As String
            Get
                Return m_cDallasScanDate
            End Get
            Set(ByVal value As String)
                m_cDallasScanDate = value
            End Set
        End Property

        ''' <summary>
        ''' Expected Date in YYMMDD Format
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DallasExpectedDate() As String
            Get
                Return m_cDallasExpectedDate
            End Get
            Set(ByVal value As String)
                m_cDallasExpectedDate = value
            End Set
        End Property

    End Class
    ''' <summary>
    ''' Value class to hold the data of the Carton scanned
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CartonDetails
        ''' <summary>
        ''' Member Variables
        ''' </summary>
        ''' <remarks></remarks>
        Private m_CartonNumber As String
        Private m_DespatchDate As String
        Private m_ScanDate As String
        Private m_ScanTime As String
        Private m_ScanType As String
        Private m_ScanLevel As String
        Private m_ProductCode As String
        Private m_BootCode As String
        Private m_ItemQty As String
        Private m_ItemStatus As String
        Public Property ItemStatus() As String
            Get
                Return m_ItemStatus
            End Get
            Set(ByVal value As String)
                m_ItemStatus = value
            End Set
        End Property

        Public Property ItemQty() As String
            Get
                Return m_ItemQty
            End Get
            Set(ByVal value As String)
                m_ItemQty = value
            End Set
        End Property

        Public Property BootCode() As String
            Get
                Return m_BootCode
            End Get
            Set(ByVal value As String)
                m_BootCode = value
            End Set
        End Property


        Public Property ProductCode() As String
            Get
                Return m_ProductCode
            End Get
            Set(ByVal value As String)
                m_ProductCode = value
            End Set
        End Property



        Public Property CartonNumber() As String
            Get
                Return m_CartonNumber
            End Get
            Set(ByVal value As String)
                m_CartonNumber = value
            End Set
        End Property
        ''' <summary>
        ''' Despatch Date in YYMMDD Format
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DespatchDate() As String
            Get
                Return m_DespatchDate
            End Get
            Set(ByVal value As String)
                m_DespatchDate = value
            End Set
        End Property
        ''' <summary>
        ''' Scan Date in YYMMDD Format
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ScanDate() As String
            Get
                Return m_ScanDate
            End Get
            Set(ByVal value As String)
                m_ScanDate = value
            End Set
        End Property
        Public Property ScanTime() As String
            Get
                Return m_ScanTime
            End Get
            Set(ByVal value As String)
                m_ScanTime = value
            End Set
        End Property
        ''' <summary>
        ''' Scan time in 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ScanType() As String
            Get
                Return m_ScanType
            End Get
            Set(ByVal value As String)
                m_ScanType = value
            End Set
        End Property
        Public Property ScanLevel() As String
            Get
                Return m_ScanLevel
            End Get
            Set(ByVal value As String)
                m_ScanLevel = value
            End Set
        End Property
    End Class
    ''' <summary>
    ''' Value class to hold details of Confirmation of Book In Driver/No Driver
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DriverDetails
        ''' <summary>
        ''' Member variables
        ''' </summary>
        ''' <remarks></remarks>
        Private m_DriverBadge As String
        Private m_ScanTime As String
        Private m_ScanDate As String
        Private m_ScanLevel As String
        Private m_GITNote As String
        Private m_BatchRescan As String
        Public Property DriverBadge() As String
            Get
                Return m_DriverBadge
            End Get
            Set(ByVal value As String)
                m_DriverBadge = value
            End Set
        End Property
        Public Property ScanTime() As String
            Get
                Return m_ScanTime
            End Get
            Set(ByVal value As String)
                m_ScanTime = value
            End Set
        End Property
        Public Property ScanDate() As String
            Get
                Return m_ScanDate
            End Get
            Set(ByVal value As String)
                m_ScanDate = value
            End Set
        End Property
        Public Property ScanLevel() As String
            Get
                Return m_ScanLevel
            End Get
            Set(ByVal value As String)
                m_ScanLevel = value
            End Set
        End Property
        Public Property GITNote() As String
            Get
                Return m_GITNote
            End Get
            Set(ByVal value As String)
                m_GITNote = value
            End Set
        End Property
        Public Property BatchRescan() As String
            Get
                Return m_BatchRescan
            End Get
            Set(ByVal value As String)
                m_BatchRescan = value
            End Set
        End Property
    End Class
    ''' <summary>
    ''' Value class to hold the UOD List
    ''' </summary>
    ''' <remarks></remarks>
    Public Class UODList
        ''' <summary>
        ''' Member variables
        ''' </summary>
        ''' <remarks></remarks>
        Private m_UODId As String
        Private m_UODType As String
        Private m_BookedIn As String
        Private m_ExptDate As String
        Private m_BookedInDate As String
        Private m_SequenceNumber As String
        Private m_Reason As String
        Public Property Reason() As String
            Get
                Return m_Reason
            End Get
            Set(ByVal Value As String)
                m_Reason = Value
            End Set
        End Property
        Public Property Sequencenumber() As String
            Get
                Return m_SequenceNumber
            End Get
            Set(ByVal Value As String)
                m_SequenceNumber = Value
            End Set
        End Property
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <remarks></remarks>
        Sub New()

        End Sub
        Public Property UODID() As String
            Get
                Return m_UODId
            End Get
            Set(ByVal value As String)
                m_UODId = value
            End Set
        End Property
        Public Property UODType() As String
            Get
                Return m_UODType
            End Get
            Set(ByVal value As String)
                m_UODType = value
            End Set
        End Property
        Public Property BookedIn() As String
            Get
                Return m_BookedIn
            End Get
            Set(ByVal value As String)
                m_BookedIn = value
            End Set
        End Property
        Public Property ExptDate() As String
            Get
                Return m_ExptDate
            End Get
            Set(ByVal value As String)
                m_ExptDate = value
            End Set
        End Property
        Public Property BookedInDate() As String
            Get
                Return m_BookedInDate
            End Get
            Set(ByVal value As String)
                m_BookedInDate = value
            End Set
        End Property
    End Class
    ''' <summary>
    ''' Value class to hold the Crate List
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CrateList
        ''' <summary>
        ''' Member Variables
        ''' </summary>
        ''' <remarks></remarks>
        Private m_CrateId As String
        Private m_CrateType As String
        Private m_BookedIn As String
        Private m_NoOfItems As String
        Private m_Sequence As String



        Public Property Sequence() As String
            Get
                Return m_Sequence
            End Get
            Set(ByVal value As String)
                m_Sequence = value
            End Set
        End Property
        Public Property CrateId() As String
            Get
                Return m_CrateId
            End Get
            Set(ByVal value As String)
                m_CrateId = value
            End Set
        End Property
        Public Property CrateType() As String
            Get
                Return m_CrateType
            End Get
            Set(ByVal value As String)
                m_CrateType = value
            End Set
        End Property
        Public Property BookedIn() As String
            Get
                Return m_BookedIn
            End Get
            Set(ByVal value As String)
                m_BookedIn = value
            End Set
        End Property
        Public Property NoOfItems() As String
            Get
                Return m_NoOfItems
            End Get
            Set(ByVal value As String)
                m_NoOfItems = value
            End Set
        End Property
    End Class
    ''' <summary>
    ''' Value class to hold the Item list
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ItemList
        'Implements IComparable
        ''' <summary>
        ''' Member Variables
        ''' </summary>
        ''' <remarks></remarks>
        Private m_ItemCode As String
        Private m_ItemDesc As String
        Private m_ItemQty As String
        Private m_ProductCode As String
        Public Property ProductCode() As String
            Get
                Return m_ProductCode
            End Get
            Set(ByVal value As String)
                m_ProductCode = value
            End Set
        End Property

        Public Property ItemCode() As String
            Get
                Return m_ItemCode
            End Get
            Set(ByVal value As String)
                m_ItemCode = value
            End Set
        End Property
        Public Property ItemQty() As String
            Get
                Return m_ItemQty
            End Get
            Set(ByVal value As String)
                m_ItemQty = value
            End Set
        End Property
        Public Property ItemDesc() As String
            Get
                Return m_ItemDesc
            End Get
            Set(ByVal value As String)
                m_ItemDesc = value
            End Set
        End Property
    End Class
    Public Class CartonList
        Implements IComparable
        Private m_CartonId As String
        Private m_ExptdDate As String
        Private m_BkdInStatus As String
        Private m_NoOfItems As String
        Sub New()

        End Sub
        Public Property CartonId() As String
            Get
                Return m_CartonId
            End Get
            Set(ByVal value As String)
                m_CartonId = value
            End Set
        End Property
        Public Property ExptdDate() As String
            Get
                Return m_ExptdDate
            End Get
            Set(ByVal value As String)
                m_ExptdDate = value
            End Set
        End Property
        Public Property BkdInStatus() As String
            Get
                Return m_BkdInStatus
            End Get
            Set(ByVal value As String)
                m_BkdInStatus = value
            End Set
        End Property
        Public Property NoOfItems() As String
            Get
                Return m_NoOfItems
            End Get
            Set(ByVal value As String)
                m_NoOfItems = value
            End Set
        End Property
        Public Function CompareTo(ByVal obj As Object) As Integer _
         Implements System.IComparable.CompareTo
            If TypeOf obj Is GIValueHolder.CartonList Then
                Dim objCartonListCompare As GIValueHolder.CartonList = CType(obj, GIValueHolder.CartonList)
                Dim iResult As Integer = Me.ExptdDate.CompareTo(objCartonListCompare.ExptdDate)
                If iResult = 0 Then
                    iResult = Me.BkdInStatus.CompareTo(objCartonListCompare.BkdInStatus)
                End If
                Return iResult
            End If
        End Function
    End Class
    Public Class CartonItemList
        Implements IComparable
        ''' <summary>
        ''' Member Variables
        ''' </summary>
        ''' <remarks></remarks>
        Private m_ItemCode As String
        Private m_ItemDesc As String
        Private m_ItemQty As String
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <remarks></remarks>
        Sub New()

        End Sub
        Public Property ItemCode() As String
            Get
                Return m_ItemCode
            End Get
            Set(ByVal value As String)
                m_ItemCode = value
            End Set
        End Property
        Public Property ItemQty() As String
            Get
                Return m_ItemQty
            End Get
            Set(ByVal value As String)
                m_ItemQty = value
            End Set
        End Property
        Public Property ItemDesc() As String
            Get
                Return m_ItemDesc
            End Get
            Set(ByVal value As String)
                m_ItemDesc = value
            End Set
        End Property
        Public Function CompareTo(ByVal obj As Object) As Integer _
        Implements System.IComparable.CompareTo
            If TypeOf obj Is GIValueHolder.CartonItemList Then
                Dim objCartonItemListCompare As GIValueHolder.CartonItemList = CType(obj, GIValueHolder.CartonItemList)
                Dim iResult As Integer = Me.ItemDesc.CompareTo(objCartonItemListCompare.ItemDesc)
                If iResult = 0 Then
                    iResult = Me.ItemCode.CompareTo(objCartonItemListCompare.ItemCode)
                End If
                Return iResult
            End If
        End Function
    End Class
    ''' <summary>
    ''' Value Class for holding Supplier Data
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SupplierList
        Implements IComparable
        'Implements IComparable
        ''' <summary>
        ''' Member Variables
        ''' </summary>
        ''' <remarks></remarks>
        Private m_SupplierNo As String
        Private m_SupplierName As String
        Private m_SupplierType As String
        Private m_SupplierQty As String
        Private m_StaticSupplier As String
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SupplierNo() As String
            Get
                Return m_SupplierNo
            End Get
            Set(ByVal value As String)
                m_SupplierNo = value
            End Set
        End Property
        Public Property SupplierName() As String
            Get
                Return m_SupplierName
            End Get
            Set(ByVal value As String)
                m_SupplierName = value
            End Set
        End Property
        Public Property SupplierType() As String
            Get
                Return m_SupplierType
            End Get
            Set(ByVal value As String)
                m_SupplierType = value
            End Set
        End Property
        Public Property SupplierQty() As String
            Get
                Return m_SupplierQty
            End Get
            Set(ByVal value As String)
                m_SupplierQty = value
            End Set
        End Property
        Public Property StaticSupplier() As String
            Get
                Return m_StaticSupplier
            End Get
            Set(ByVal value As String)
                m_StaticSupplier = value
            End Set
        End Property
        '   Public Property OrderCount() As String
        '       Get
        '           Return m_OrderCount
        '       End Get
        '       Set(ByVal value As String)
        '           m_OrderCount = value
        '       End Set
        '   End Property
        Public Function CompareTo(ByVal obj As Object) As Integer _
     Implements System.IComparable.CompareTo
            If TypeOf obj Is GIValueHolder.SupplierList Then
                Dim objSupplierListCompare As GIValueHolder.SupplierList = CType(obj, GIValueHolder.SupplierList)
                Dim iResult As Integer = Me.SupplierName.CompareTo(objSupplierListCompare.SupplierName)
                If iResult = 0 Then
                    iResult = Me.SupplierQty.CompareTo(objSupplierListCompare.SupplierQty)
                End If
                Return iResult
            End If
        End Function
    End Class
    Public Class SupplierListForView
        Implements IComparable
        ''' <summary>
        ''' Member variables
        ''' </summary>
        ''' <remarks></remarks>
        Private m_SupplierName As String
        Private m_OrderCount As String
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <remarks></remarks>
        Sub New()

        End Sub
        Public Property SupplierName() As String
            Get
                Return m_SupplierName
            End Get
            Set(ByVal value As String)
                m_SupplierName = value
            End Set
        End Property
        Public Property OrderCount() As String
            Get
                Return m_OrderCount
            End Get
            Set(ByVal value As String)
                m_OrderCount = value
            End Set
        End Property
        Public Function CompareTo(ByVal obj As Object) As Integer _
          Implements System.IComparable.CompareTo
            If TypeOf obj Is SupplierListForView Then
                Dim objSupplierListCompare As SupplierListForView = CType(obj, SupplierListForView)
                Dim iResult As Integer = Me.SupplierName.CompareTo(objSupplierListCompare.SupplierName)
                If iResult = 0 Then
                    iResult = Me.OrderCount.CompareTo(objSupplierListCompare.OrderCount)
                End If
                Return iResult
            End If
        End Function
    End Class
    ''' <summary>
    ''' Value Class to hold Order Number Data
    ''' </summary>
    ''' <remarks></remarks>
    Public Class OrderList
        ''' <summary>
        ''' Member Variables
        ''' </summary>
        ''' <remarks></remarks>
        Private m_EstDeliveryDate As String
        Private m_BookInDate As String
        Private m_SupplierNo As String
        Private m_OrderNo As String
        Private m_BusCentre As String
        Private m_Source As String
        Private m_Code As String
        Private m_BookInStatus As String
        Private m_PageNo As String
        Public Property PageNo() As String
            Get
                Return m_PageNo
            End Get
            Set(ByVal value As String)
                m_PageNo = value
            End Set
        End Property
        Public Property EstDeliveryDate() As String
            Get
                Return m_EstDeliveryDate
            End Get
            Set(ByVal value As String)
                m_EstDeliveryDate = value
            End Set
        End Property
        Public Property SupplierNo() As String
            Get
                Return m_SupplierNo
            End Get
            Set(ByVal value As String)
                m_SupplierNo = value
            End Set
        End Property
        Public Property BookInDate() As String
            Get
                Return m_BookInDate
            End Get
            Set(ByVal value As String)
                m_BookInDate = value
            End Set
        End Property
        Public Property OrderNo() As String
            Get
                Return m_OrderNo
            End Get
            Set(ByVal value As String)
                m_OrderNo = value
            End Set
        End Property
        Public Property BusCentre() As String
            Get
                Return m_BusCentre
            End Get
            Set(ByVal value As String)
                m_BusCentre = value
            End Set
        End Property
        Public Property Source() As String
            Get
                Return m_Source
            End Get
            Set(ByVal value As String)
                m_Source = value
            End Set
        End Property
        Public Property Code() As String
            Get
                Return m_Code
            End Get
            Set(ByVal value As String)
                m_Code = value
            End Set
        End Property
        Public Property BookInStatus() As String
            Get
                Return m_BookInStatus
            End Get
            Set(ByVal value As String)
                m_BookInStatus = value
            End Set
        End Property
    End Class
    ''' <summary>
    ''' Value Class to hold Item details for an Order
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ItemListForOrder
        Private m_ProductCode As String
        Private m_BootsCode As String
        Private m_ExptDate As String
        Private m_Status As String
        Private m_ItemDesc As String
        Private m_ExptdQty As String
        Private m_PageNo As String
        Public Property PageNo() As String
            Get
                Return m_PageNo
            End Get
            Set(ByVal value As String)
                m_PageNo = value
            End Set
        End Property

        Public Property ProductCode() As String
            Get
                Return m_ProductCode
            End Get
            Set(ByVal value As String)
                m_ProductCode = value
            End Set
        End Property
        Public Property ExptdQty() As String
            Get
                Return m_ExptdQty
            End Get
            Set(ByVal value As String)
                m_ExptdQty = value
            End Set
        End Property
        Public Property BootsCode() As String
            Get
                Return m_BootsCode
            End Get
            Set(ByVal value As String)
                m_BootsCode = value
            End Set
        End Property
        Public Property ExptDate() As String
            Get
                Return m_ExptDate
            End Get
            Set(ByVal value As String)
                m_ExptDate = value
            End Set
        End Property
        Public Property Status() As String
            Get
                Return m_Status
            End Get
            Set(ByVal value As String)
                m_Status = value
            End Set
        End Property
        Public Property ItemDesc() As String
            Get
                Return m_ItemDesc
            End Get
            Set(ByVal value As String)
                m_ItemDesc = value
            End Set
        End Property
    End Class
    ''' <summary>
    ''' Value Class to hold supplier data
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SupplierData
        Private m_SupplierNo As String
        Private m_SupplierName As String
        Private m_SupplierType As String
        Private m_StaticSupplier As String
        Public Property SupplierNo() As String
            Get
                Return m_SupplierNo
            End Get
            Set(ByVal value As String)
                m_SupplierNo = value
            End Set
        End Property
        Public Property SupplierName() As String
            Get
                Return m_SupplierName
            End Get
            Set(ByVal value As String)
                m_SupplierName = value
            End Set
        End Property
        Public Property SupplierType() As String
            Get
                Return m_SupplierType
            End Get
            Set(ByVal value As String)
                m_SupplierType = value
            End Set
        End Property
        Public Property StaticSupplier() As String
            Get
                Return m_StaticSupplier
            End Get
            Set(ByVal value As String)
                m_StaticSupplier = value
            End Set
        End Property
    End Class
    ''' <summary>
    ''' Value class to hold the details of Carton inside a supplier
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SupplierDetails
        Private m_ASNNumber As String
        Private m_CartonNumber As String
        Private m_ExptDate As String
        Private m_Status As String
        Private m_TotalItemsInCarton As String

        Public Property ASNNumber() As String
            Get
                Return m_ASNNumber
            End Get
            Set(ByVal value As String)
                m_ASNNumber = value
            End Set
        End Property

        Public Property TotalItemsInCarton() As String
            Get
                Return m_TotalItemsInCarton
            End Get
            Set(ByVal value As String)
                m_TotalItemsInCarton = value
            End Set
        End Property
        Public Property CartonNumber() As String
            Get
                Return m_CartonNumber
            End Get
            Set(ByVal value As String)
                m_CartonNumber = value
            End Set
        End Property
        Public Property ExptDate() As String
            Get
                Return m_ExptDate
            End Get
            Set(ByVal value As String)
                m_ExptDate = value
            End Set
        End Property
        Public Property Status() As String
            Get
                Return m_Status
            End Get
            Set(ByVal value As String)
                m_Status = value
            End Set
        End Property
    End Class
    ''' <summary>
    ''' Value class to hold the details of item
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ItemDetails

        Private m_ItemCode As String
        Private m_ExpectedDate As String
        Private m_ItemDesc As String
        Private m_Status As String
        Private m_ItemQty As String

        Private m_ProductCode As String
        Public Property ProductCode() As String
            Get
                Return m_ProductCode
            End Get
            Set(ByVal value As String)
                m_ProductCode = value
            End Set
        End Property

        Public Property ItemCode() As String
            Get
                Return m_ItemCode
            End Get
            Set(ByVal value As String)
                m_ItemCode = value
            End Set
        End Property
        Public Property ExpectedDate() As String
            Get
                Return m_ExpectedDate
            End Get
            Set(ByVal value As String)
                m_ExpectedDate = value
            End Set
        End Property
        Public Property ItemQty() As String
            Get
                Return m_ItemQty
            End Get
            Set(ByVal value As String)
                m_ItemQty = value
            End Set
        End Property
        Public Property ItemDesc() As String
            Get
                Return m_ItemDesc
            End Get
            Set(ByVal value As String)
                m_ItemDesc = value
            End Set
        End Property
        Public Property Status() As String
            Get
                Return m_Status
            End Get
            Set(ByVal value As String)
                m_Status = value
            End Set
        End Property
    End Class
    ''' <summary>
    ''' Value class to hold the Details of scanned Items
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ScannedItemDetails
        Private m_ScannedCode As String
        Private m_BootCode As String
        Private m_ScanType As String
        Private m_ScanLevel As String
        Private m_ScanDate As String
        Private m_ScanTime As String
        Private m_ItemQty As String
        Private m_ItemStatus As String
        Private m_ProductCode As String
        Public Property ProductCode() As String
            Get
                Return m_ProductCode
            End Get
            Set(ByVal value As String)
                m_ProductCode = value
            End Set
        End Property

        Public Property ItemStatus() As String
            Get
                Return m_ItemStatus
            End Get
            Set(ByVal value As String)
                m_ItemStatus = value
            End Set
        End Property

        Public Property ItemQty() As String
            Get
                Return m_ItemQty
            End Get
            Set(ByVal value As String)
                m_ItemQty = value
            End Set
        End Property

        Public Property ScanTime() As String
            Get
                Return m_ScanTime
            End Get
            Set(ByVal value As String)
                m_ScanTime = value
            End Set
        End Property

        Public Property ScanDate() As String
            Get
                Return m_ScanDate
            End Get
            Set(ByVal value As String)
                m_ScanDate = value
            End Set
        End Property

        Public Property ScanLevel() As String
            Get
                Return m_ScanLevel
            End Get
            Set(ByVal value As String)
                m_ScanLevel = value
            End Set
        End Property


        Public Property ScannedCode() As String
            Get
                Return m_ScannedCode
            End Get
            Set(ByVal value As String)
                m_ScannedCode = value
            End Set
        End Property

        Public Property BootsCode() As String
            Get
                Return m_BootCode
            End Get
            Set(ByVal value As String)
                m_BootCode = value
            End Set
        End Property

        Public Property ScanType() As String
            Get
                Return m_ScanType
            End Get
            Set(ByVal value As String)
                m_ScanType = value
            End Set
        End Property



    End Class
    ''' <summary>
    ''' Value class to hold the data of Carton
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CartonInfo
        Private m_CartonNumber As String
        Private m_SupplierNo As String
        Private m_Status As String
        Private m_ExpDeliveryDate As String
        Private m_TotalItemsInCarton As String
        Private m_SupplierName As String
        Private m_ASNNumber As String
        Public Property ASNNumber() As String
            Get
                Return m_ASNNumber
            End Get
            Set(ByVal value As String)
                m_ASNNumber = value
            End Set
        End Property


        Public Property SupplierName() As String
            Get
                Return m_SupplierName
            End Get
            Set(ByVal value As String)
                m_SupplierName = value
            End Set
        End Property
        Public Property CartonNumber() As String
            Get
                Return m_CartonNumber
            End Get
            Set(ByVal value As String)
                m_CartonNumber = value
            End Set
        End Property
        Public Property Status() As String
            Get
                Return m_Status
            End Get
            Set(ByVal value As String)
                m_Status = value
            End Set
        End Property
        Public Property SupplierNo() As String
            Get
                Return m_SupplierNo
            End Get
            Set(ByVal value As String)
                m_SupplierNo = value
            End Set
        End Property
        Public Property ExpDeliveryDate() As String
            Get
                Return m_ExpDeliveryDate
            End Get
            Set(ByVal value As String)
                m_ExpDeliveryDate = value
            End Set
        End Property
        Public Property TotalItemsInCarton() As String
            Get
                Return m_TotalItemsInCarton
            End Get
            Set(ByVal value As String)
                m_TotalItemsInCarton = value
            End Set
        End Property
    End Class

    ''' <summary>
    ''' Enumerated data for Period for View
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum UODPeriod
        Today
        Future
    End Enum
    ''' <summary>
    ''' Enumerated data for Container Types for UOD
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum UODContainerType
        Dolly
        Crate
        RollyCage
        Paller
        IST
    End Enum

    Public Enum IsAbort
        Yes
        No
    End Enum
    ''' <summary>
    ''' Value class to hold the data of scanned or entered ASN code
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ASNCode
        Private m_SupplierNumber As String
        Private m_CartonNumber As String
        Private m_NoOfCartons As String
        Public Property SupplierNumber() As String
            Get
                Return m_SupplierNumber
            End Get
            Set(ByVal value As String)
                m_SupplierNumber = value
            End Set
        End Property
        Public Property CartonNumber() As String
            Get
                Return m_CartonNumber
            End Get
            Set(ByVal value As String)
                m_CartonNumber = value
            End Set
        End Property
        Public Property NoOfCartons() As String
            Get
                Return m_NoOfCartons
            End Get
            Set(ByVal value As String)
                m_NoOfCartons = value
            End Set
        End Property

    End Class

    ' V1.1 - CK
    ' Added new class DallasDeliverySummary

    ''' <summary>
    ''' Value class to hold data of Dallas UODs obtained from DAD message
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DallasDeliverySummary
        ''' <summary>
        ''' Member variables
        ''' </summary>
        ''' <remarks></remarks>
        Private m_cDallasBarcode As String
        Private m_cExpectedDelDate As String
        Private m_cStatus As Char
        ''' <summary>
        ''' Dallas Barcode
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DallasBarcode() As String
            Get
                Return m_cDallasBarcode
            End Get
            Set(ByVal value As String)
                m_cDallasBarcode = value
            End Set
        End Property
        ''' <summary>
        ''' Expected Delivery date
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ExpectedDelDate() As String
            Get
                Return m_cExpectedDelDate
            End Get
            Set(ByVal value As String)
                m_cExpectedDelDate = value
            End Set
        End Property
        ''' <summary>
        ''' Status can be 'R' (Receipted)/'B' (Banked)/'U' (Un-receipted)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Status() As Char
            Get
                Return m_cStatus
            End Get
            Set(ByVal value As Char)
                m_cStatus = value
            End Set
        End Property
    End Class

End Class

