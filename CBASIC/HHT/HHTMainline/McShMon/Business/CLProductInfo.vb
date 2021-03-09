''' ***************************************************************************
''' <fileName>CLProductGroupInfo.vb</fileName>
''' <summary>Class that manages the items in a particular product group for countlist
''' </summary>
'''  <Version>1.0</Version>
''' <author>Infosys Technologies Ltd.,</author>
''' <DateModified>27-Jan-2009</DateModified>
''' <remarks></remarks>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
''' ***************************************************************************

Public Class CLProductInfo
    Inherits ProductInfo

    'Member variables
    'Holds the back shop quantity
    Private m_iBackShopQuantity As Integer
    'Holds the sales floor quantity
    Private m_iSalesFloorQuantity As Integer

    'ambli
    'For OSSR
    'Holds the OSSR quantity
    Private m_iOSSRQuantity As Integer
  
    'Holds Product Group
    Private m_strProductGroup As String
    'Holds Deal info
    Private m_cActiveDeal As Char

    'Stock File Accuracy 
    Private m_bIsNotPlannerItem As Boolean
    Private m_iBSMBSQuantity As Integer
    Private m_iBSPSPQuantity As Integer
    Private m_iOSSRMBSQuantity As Integer
    Private m_iOSSRPSPQuantity As Integer
    Private m_strCountStatus As String
    Private m_iTotalSiteCount As Integer
    Private m_iCurrentSiteCount As Integer
    Private m_arrSFMultiSiteList As ArrayList
    Private m_arrBSMultiSiteList As ArrayList
    Private m_arrOSSRSiteList As ArrayList
    Private m_iTotalBSSiteCount As Integer
    Private m_iTotalOSSRSiteCount As Integer
    Private m_bIsSFDifferentSession As Boolean
    Private m_arrDistinctPOGList As ArrayList
    Private m_bIsSFItemCounted As Boolean
    Private m_bIsMBSItemCounted As Boolean
    Private m_bIsBSPSPItemCounted As Boolean
    Private m_iSalesAtPODDock As Integer
    Private m_bIsUnknownItem As Boolean
    Private m_bIsSEL As Boolean
    'Holds the total Quantity
    Private m_iTotalQuantity As Integer

    Private m_strSequenceNumber As String

    Private m_iNumSELQueued As Integer
    Private m_FirstBarcode As String
    Private m_iMultiSiteCount As Integer
    Private m_strLastCountedDate As String
    Private m_strCreatedLocation As String
    ''' <summary>
    ''' Gets/Sets the back shop quantity
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BackShopQuantity() As Integer
        Get
            Return m_iBackShopQuantity
        End Get
        Set(ByVal value As Integer)
            m_iBackShopQuantity = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the sales floor quantity
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SalesFloorQuantity() As Integer
        Get
            Return m_iSalesFloorQuantity
        End Get
        Set(ByVal value As Integer)
            m_iSalesFloorQuantity = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the OSSR quantity
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OSSRQuantity() As Integer
        Get
            Return m_iOSSRQuantity
        End Get
        Set(ByVal value As Integer)
            m_iOSSRQuantity = value
        End Set
    End Property

    Public Property ProductGroup() As String
        Get
            Return m_strProductGroup
        End Get
        Set(ByVal value As String)
            m_strProductGroup = value
        End Set
    End Property
    Public Property ActiveDeal() As Char
        Get
            Return m_cActiveDeal
        End Get
        Set(ByVal value As Char)
            m_cActiveDeal = value
        End Set
    End Property
    Public Property LastCountDate() As String
        Get
            Return m_strLastCountedDate
        End Get
        Set(ByVal value As String)
            m_strLastCountedDate = value
        End Set
    End Property
    

    ''' <summary>
    ''' Gets/Sets the total quantity
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TotalQuantity() As Integer
        Get
            Return m_iTotalQuantity
        End Get
        Set(ByVal value As Integer)
            m_iTotalQuantity = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the sequence number
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SequenceNumber() As String
        Get
            Return m_strSequenceNumber
        End Get
        Set(ByVal value As String)
            m_strSequenceNumber = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the number of SELs queued
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NumSELsQueued() As Integer
        Get
            Return m_iNumSELQueued
        End Get
        Set(ByVal value As Integer)
            m_iNumSELQueued = value
        End Set
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FirstBarcode() As String
        Get
            Return m_FirstBarcode
        End Get
        Set(ByVal value As String)
            m_FirstBarcode = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the count status
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CountStatus() As String
        Get
            Return m_strCountStatus
        End Get
        Set(ByVal value As String)
            m_strCountStatus = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the SF multi site list
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SFMultiSiteList() As ArrayList
        Get
            Return m_arrSFMultiSiteList
        End Get
        Set(ByVal value As ArrayList)
            m_arrSFMultiSiteList = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the BS multi site list
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BSMultiSiteList() As ArrayList
        Get
            Return m_arrBSMultiSiteList
        End Get
        Set(ByVal value As ArrayList)
            m_arrBSMultiSiteList = value
        End Set
    End Property

    ''' <summary>
    ''' Gets/Sets the count of sites
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MultiSiteCount() As Integer
        Get
            Return m_iMultiSiteCount
        End Get
        Set(ByVal value As Integer)
            m_iMultiSiteCount = value
        End Set
    End Property

    ''' <summary>
    ''' Gets/Sets the OSSR multi site list
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OSSRMultiSiteList() As ArrayList
        Get
            Return m_arrOSSRSiteList
        End Get
        Set(ByVal value As ArrayList)
            m_arrOSSRSiteList = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the total BS site count
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TotalBSSiteCount() As Integer
        Get
            Return m_iTotalBSSiteCount
        End Get
        Set(ByVal value As Integer)
            m_iTotalBSSiteCount = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the total OSSR site count
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TotalOSSRSiteCount() As Integer
        Get
            Return m_iTotalOSSRSiteCount
        End Get
        Set(ByVal value As Integer)
            m_iTotalOSSRSiteCount = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the total BS - MBS qty
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BackShopMBSQuantity() As Integer
        Get
            Return m_iBSMBSQuantity
        End Get
        Set(ByVal value As Integer)
            m_iBSMBSQuantity = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the total BS - PSP qty
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BackShopPSPQuantity() As Integer
        Get
            Return m_iBSPSPQuantity
        End Get
        Set(ByVal value As Integer)
            m_iBSPSPQuantity = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the total OSSR - MBS qty
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OSSRMBSQuantity() As Integer
        Get
            Return m_iOSSRMBSQuantity
        End Get
        Set(ByVal value As Integer)
            m_iOSSRMBSQuantity = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the total OSSR - PSP qty
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OSSRPSPQuantity() As Integer
        Get
            Return m_iOSSRPSPQuantity
        End Get
        Set(ByVal value As Integer)
            m_iOSSRPSPQuantity = value
        End Set
    End Property

    ''' <summary>
    ''' Gets/Sets the total site count for an item in planner
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TotalSiteCount() As Integer
        Get
            Return m_iTotalSiteCount
        End Get
        Set(ByVal value As Integer)
            m_iTotalSiteCount = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the current site count for an item in planner
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CurrentSiteCount() As Integer
        Get
            Return m_iCurrentSiteCount
        End Get
        Set(ByVal value As Integer)
            m_iCurrentSiteCount = value
        End Set
    End Property

    ''' <summary>
    ''' Gets/Sets the current site count for an item in planner
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsNotPlannerItem() As Boolean
        Get
            Return m_bIsNotPlannerItem
        End Get
        Set(ByVal value As Boolean)
            m_bIsNotPlannerItem = value
        End Set
    End Property

    ''' <summary>
    ''' Gets/Sets the current site count for an item in planner
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsSFDifferentSession() As Boolean
        Get
            Return m_bIsSFDifferentSession
        End Get
        Set(ByVal value As Boolean)
            m_bIsSFDifferentSession = value
        End Set
    End Property

    ''' <summary>
    ''' Gets/Sets the current site count for an item in planner
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DistinctPOGList() As ArrayList
        Get
            Return m_arrDistinctPOGList
        End Get
        Set(ByVal value As ArrayList)
            m_arrDistinctPOGList = value
        End Set

    End Property
  ''' <summary>
    ''' Gets/Sets the creates location
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CreatedLocation() As String
        Get
            Return m_strCreatedLocation
        End Get
        Set(ByVal value As String)
            m_strCreatedLocation = value
        End Set
    End Property
                 
    ''' <summary>
    ''' Gets/Sets the item count status for Sales Floor
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsSFItemCounted() As Boolean
        Get
            Return m_bIsSFItemCounted
        End Get
        Set(ByVal value As Boolean)
            m_bIsSFItemCounted = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the item count status for BS
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsMBSItemCounted() As Boolean
        Get
            Return m_bIsMBSItemCounted
        End Get
        Set(ByVal value As Boolean)
            m_bIsMBSItemCounted = value
        End Set
    End Property
    ''' <summary>
    ''' Gets/Sets the item count status for BS PSP
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsBSPSPItemCounted() As Boolean
        Get
            Return m_bIsBSPSPItemCounted
        End Get
        Set(ByVal value As Boolean)
            m_bIsBSPSPItemCounted = value
        End Set
    End Property
    Public Property SalesAtPODDock() As Integer
        Get
            Return m_iSalesAtPODDock
        End Get
        Set(ByVal value As Integer)
            m_iSalesAtPODDock = value
        End Set
    End Property
    Public Property IsUnknownItem() As Boolean
        Get
            Return m_bIsUnknownItem
        End Get
        Set(ByVal value As Boolean)
            m_bIsUnknownItem = value
        End Set
    End Property
    Public Property IsSEL() As Boolean
        Get
            Return m_bIsSEL
        End Get
        Set(ByVal value As Boolean)
            m_bIsSEL = value
        End Set
    End Property
End Class

