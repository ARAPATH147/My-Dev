Public Class CartonInfo
    Private m_CartonNo As String
    Private m_Status As String
    Private m_SupplierNo As String
    Private m_TotalItemsInCarton As String
    Private m_ExpDeliveryDate As String
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

    Public Property CartonNo() As String
        Get
            Return m_CartonNo
        End Get
        Set(ByVal value As String)
            m_CartonNo = value
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
