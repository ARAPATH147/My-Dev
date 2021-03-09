'''***************************************************************
''' <FileName>ProductInfo.vb</FileName>
''' <summary>
''' Class to set the item details
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>27-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
Public Class ProductInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_BootsCode As String
    Private m_ProductCode As String
    Private m_Description As String
    Private m_StockFigure As String
    Private m_ItemStatus As String
    Private m_ShortDescription As String
    Private m_Sequence As String
    Private m_Price As String
    Private m_MSFlag As String
    Private m_SupplyRoute As String
    Private m_CIPFlag As String
    Private m_Advantage As String
    Private m_PendSaleFlag As Boolean
    Private m_bIsAllSitesCounted As Boolean '@Service Fix

    ''' <summary>
    ''' Constructor for the class ProductInfo
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()

    End Sub

#If RF Then
    Private cActiveDeal As Char
    Private cGapFlag As Char
    Private cOSSRFlag As Char
    Private strParentsCode As String
    Private strOperatorID As String
    Private strRequired As String
    Public Property ParentsCode() As String
        Get
            Return strParentsCode
        End Get
        Set(ByVal value As String)
            strParentsCode = value
        End Set
    End Property
    Public Property OperatorID() As String
        Get
            Return strOperatorID
        End Get
        Set(ByVal value As String)
            strOperatorID = value
        End Set
    End Property
    Public Property Required() As String
        Get
            Return strRequired
        End Get
        Set(ByVal value As String)
            strRequired = value
        End Set
    End Property
  
    Public Property ActiveDeal() As String
        Get
            Return cActiveDeal
        End Get
        Set(ByVal value As String)
            cActiveDeal = value
        End Set
    End Property

    Public Property GapFlag() As String
        Get
            Return cGapFlag
        End Get
        Set(ByVal value As String)
            cGapFlag = value
        End Set
    End Property
    Public Property OSSRFlag() As String
        Get
            Return cOSSRFlag
        End Get
        Set(ByVal value As String)
            If value.Equals("Y") Then
                cOSSRFlag = "O"
            Else
                cOSSRFlag = value
            End If
        End Set
    End Property
#End If
    ''' <summary>
    ''' To set or get the Boots code.
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
    ''' Sequence Fix
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Sequence() As String
        Get
            Return m_Sequence
        End Get
        Set(ByVal value As String)
            m_Sequence = value
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
            Return m_ProductCode
        End Get
        Set(ByVal value As String)
            m_ProductCode = value
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
            Return m_Description
        End Get
        Set(ByVal value As String)
            m_Description = value
        End Set
    End Property
    ''' <summary>
    ''' Item short description
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
    ''' To set or get the Product status.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Status() As String
        Get
            Return m_ItemStatus
        End Get
        Set(ByVal value As String)
            m_ItemStatus = value
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
            Return m_StockFigure
        End Get
        Set(ByVal value As String)
            m_StockFigure = value
        End Set
    End Property
    ''' <summary>
    ''' Item Price
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Price() As String
        Get
            Return m_Price
        End Get
        Set(ByVal value As String)
            m_Price = value
        End Set
    End Property
    ''' <summary>
    ''' Multisite Prsence of the item
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MSFlag() As String
        Get
            If m_MSFlag = "Y" Then
                Return "Y"
            Else
                Return " "
            End If
        End Get
        Set(ByVal value As String)
            m_MSFlag = value
        End Set
    End Property
    ''' <summary>
    ''' Supply Route for the item.
    ''' </summary>
    ''' <value>W/C/' ' - W (Warehouse) changed to E (Epsom)</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SupplyRoute() As String
        Get
            Return m_SupplyRoute
        End Get
        Set(ByVal value As String)
            m_SupplyRoute = value
        End Set
    End Property
    ''' <summary>
    ''' Advantage Redemption flag
    ''' </summary>
    ''' <value>* or space</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Advantage() As String
        Get
            Return m_Advantage
        End Get
        Set(ByVal value As String)
            m_Advantage = value
        End Set
    End Property
    ''' <summary>
    ''' CIF Flag.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CIPFlag() As String
        Get
            Return m_CIPFlag
        End Get
        Set(ByVal value As String)
            If value = "Y" Or value = "y" Then
                m_CIPFlag = "Y"
            Else
                m_CIPFlag = "N"
            End If
        End Set
    End Property
    ''' <summary>
    ''' PSP Flag.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PendingSalesFlag() As Boolean
        Get
            Return m_PendSaleFlag
        End Get
        Set(ByVal value As Boolean)
            m_PendSaleFlag = value
        End Set
    End Property

    ''' <summary>
    '''  '@Service Fix   Variable set to true if all sites counted for an item
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsAllSitesCounted() As Boolean
        Get
            Return m_bIsAllSitesCounted
        End Get
        Set(ByVal value As Boolean)
            m_bIsAllSitesCounted = value
        End Set
    End Property

End Class
