Public Class UODInfo
    Private m_UODNumber As String
    Private m_UODStatus As String
    Private m_BOLUOD As String
    Private m_DespatchDate As String
    Private m_UODType As String
    Private m_ExpectedDeliveryDate As String
    Private m_ImmLicenseNo As String
    Private m_NoOfChildren As String
    Private m_NoOfItems As Integer
    Private m_UltLicensePlate As String
    Private m_BookInDate As String
    Private m_SequenceNumber As String
    Private m_UODReason As String
    'CR for Partail Booked DOlly
    Private m_PartialBkd As String

    Public Property PartialBkd() As String
        Get
            Return m_PartialBkd
        End Get
        Set(ByVal Value As String)
            m_PartialBkd = Value
        End Set
    End Property
    Public Property SequenceNumber() As String
        Get
            Return m_SequenceNumber
        End Get
        Set(ByVal Value As String)
            m_SequenceNumber = Value
        End Set
    End Property
    Public Property UODReason() As String
        Get
            Return m_UODReason
        End Get
        Set(ByVal value As String)
            m_UODReason = value
        End Set
    End Property
    Public Property ImmLicenseNo() As String

        Get
            Return m_ImmLicenseNo
        End Get
        Set(ByVal value As String)
            m_ImmLicenseNo = value
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
    Public Property NoOfItems() As Integer
        Get
            Return m_NoOfItems
        End Get
        Set(ByVal value As Integer)
            m_NoOfItems = value
        End Set
    End Property
    Public Property NoOfChildren() As String
        Get
            Return m_NoOfChildren
        End Get
        Set(ByVal value As String)
            m_NoOfChildren = value
        End Set
    End Property
    Public Property UltLicensePlate() As String
        Get
            Return m_UltLicensePlate
        End Get
        Set(ByVal value As String)
            m_UltLicensePlate = value
        End Set
    End Property
    Public Property ExpectedDeliveryDate() As String

        Get
            Return m_ExpectedDeliveryDate
        End Get
        Set(ByVal value As String)
            m_ExpectedDeliveryDate = value
        End Set
    End Property
    Public Property UODNumber() As String
        Get
            Return m_UODNumber
        End Get
        Set(ByVal value As String)
            m_UODNumber = value
        End Set
    End Property
    Public Property UODStatus() As String
        Get
            Return m_UODStatus
        End Get
        Set(ByVal value As String)
            m_UODStatus = value
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
    Public Property DespatchDate() As String
        Get
            Return m_DespatchDate
        End Get
        Set(ByVal value As String)
            m_DespatchDate = value
        End Set
    End Property
    Public Property BOLUOD() As String
        Get
            Return m_BOLUOD
        End Get
        Set(ByVal value As String)
            m_BOLUOD = value
        End Set
    End Property
End Class
Public Class UODParentDetails
    Private m_UODParent As String
    Private m_UODChildCount As Integer
    Private m_UODScannedCount As Integer
    Private m_UODExpectedDate As String
    Private m_UODBOLStatus As String
    Private m_bReverted As Boolean = False
    'Private m_UODType As String
    Public Property Reverted() As Boolean
        Get
            Return m_bReverted
        End Get
        Set(ByVal value As Boolean)
            m_bReverted = True
        End Set
    End Property

    Public Property BOL() As String
        Get
            Return m_UODBOLStatus
        End Get
        Set(ByVal value As String)
            m_UODBOLStatus = value
        End Set
    End Property

    Public Property UODExpectedDate() As String
        Get
            Return m_UODExpectedDate
        End Get
        Set(ByVal value As String)
            m_UODExpectedDate = value
        End Set
    End Property
    Public Property UODParent() As String
        Get
            Return m_UODParent
        End Get
        Set(ByVal value As String)
            m_UODParent = value
        End Set
    End Property
    Public Property UODChildCount() As Integer
        Get
            Return m_UODChildCount
        End Get
        Set(ByVal value As Integer)
            m_UODChildCount = value
        End Set
    End Property
    Public Property UODScannedCount() As Integer
        Get
            Return m_UODScannedCount
        End Get
        Set(ByVal value As Integer)
            m_UODScannedCount = value
        End Set
    End Property
End Class
