Public Class ItemInfo
    Private m_ItemDesc As String
    Private m_BootsCode As String
    Private m_ProductCode As String
    Private m_ItemQty As Integer
    Private m_ParentCode As String
    Private m_Barcode As String
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub
    ''' <summary>
    ''' to get or set the Item Description
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ItemDesc() As String
        Get
            Return m_ItemDesc
        End Get
        Set(ByVal value As String)
            m_ItemDesc = value
        End Set
    End Property
    ''' <summary>
    ''' to get or set the Boots Code
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BootsCode() As String
        Get
            Return m_BootsCode
        End Get
        Set(ByVal value As String)
            m_BootsCode = value
        End Set
    End Property
    ''' <summary>
    ''' to get or set the Product code
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ProductCode() As String
        Get
            Return m_ProductCode
        End Get
        Set(ByVal value As String)
            m_ProductCode = value
        End Set
    End Property
    ''' <summary>
    ''' to get or set the Item quantity
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ItemQty() As Integer
        Get
            Return m_ItemQty
        End Get
        Set(ByVal value As Integer)
            m_ItemQty = value
        End Set
    End Property
    Public Property ParentCode() As String
        Get
            Return m_ParentCode
        End Get
        Set(ByVal value As String)
            m_ParentCode = value
        End Set
    End Property
    Public Property Barcode() As String
        Get
            Return m_Barcode
        End Get
        Set(ByVal value As String)
            m_Barcode = value
        End Set
    End Property
End Class
