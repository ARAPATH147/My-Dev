'''***************************************************************
''' <FileName>ProductInfo.vb</FileName>
''' <summary>
''' This class stores the generic product information details
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
Public Class ProductInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private itemBootsCode As String
    Private prodCode As String
    Private prodDescription As String
    Private stockFigure As Integer
    Private itemStatus As String
    Private m_ItemPrice As String
    Private m_shortDescription As String
    Private m_supplyRoute As String
    ''' <summary>
    ''' Constructor for the class ProductInfo
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub
    ''' <summary>
    ''' To set or get the Boots code.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ShortDescription() As String
        Get
            Return m_shortDescription
        End Get
        Set(ByVal value As String)
            m_shortDescription = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get the Boots code.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SupplyRoute() As String
        Get
            Return m_supplyRoute
        End Get
        Set(ByVal value As String)
            m_supplyRoute = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get the Boots code.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BootsCode() As String
        Get
            Return itemBootsCode
        End Get
        Set(ByVal value As String)
            itemBootsCode = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get Product code.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ProductCode() As String
        Get
            Return prodCode
        End Get
        Set(ByVal value As String)
            prodCode = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get the Product description.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Description() As String
        Get
            Return prodDescription
        End Get
        Set(ByVal value As String)
            prodDescription = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get the Product status.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Status() As String
        Get
            Return itemStatus
        End Get
        Set(ByVal value As String)
            itemStatus = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get the Stock Figure.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TSF() As String
        Get
            Return stockFigure
        End Get
        Set(ByVal value As String)
            stockFigure = value
        End Set
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ItemPrice() As String
        Get
            Return m_ItemPrice
        End Get
        Set(ByVal value As String)
            m_ItemPrice = value
        End Set
    End Property
End Class