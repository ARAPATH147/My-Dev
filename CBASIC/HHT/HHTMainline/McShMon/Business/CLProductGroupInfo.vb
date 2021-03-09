''' ***************************************************************************
''' <fileName>CLProductGroupInfo.vb</fileName>
''' <summary>To Store/Retreive the details of Count list.
''' </summary>
'''  <Version>1.0</Version>
''' <author>Infosys Technologies Ltd.,</author>
''' <DateModified>27-Jan-2009</DateModified>
''' <remarks></remarks>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
''' ***************************************************************************
Public Class CLProductGroupInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_ListID As String
    Private m_ListType As String
    Private m_ListDescription As String
    Private m_NumberOfItems As Integer
    Private m_bIsUpdate As Boolean
    Private m_DateOfLastCount As String
    Private m_iBS_PSP As Integer
    Private m_iOSSR_PSP As Integer
    Private m_NotPlannerItemCount As Integer
    Private m_iSFItemcount As Integer
    Private m_iBSItemCount As Integer
    Private m_iOSSRItemCount As Integer
    Private m_bIsCompleted As Boolean
    Private m_SFCountedItems As Integer
    Private m_BSCountedItems As Integer
    Private m_OSSRCoutedItems As Integer
    Private m_arrNotOnPlannerItemList As ArrayList
    Private m_arrUnknownItemList As ArrayList
    Private m_UnknownItemCount As Integer
    Private m_IsActive As Boolean
    Private m_iCurrentItemCount As Integer
    Private m_strCounterID As String
#If RF Then
    'For RF
    Private cActiveType As Char
    Private m_BackShopCount As Integer
    Private m_SalesFloorCount As String
    Private m_OSSRCount As String
    Private m_strSeqNumber As String
    Private m_strSiteType As String
    Public Property ActiveType() As String
        Get
            Return cActiveType
        End Get
        Set(ByVal value As String)
            cActiveType = value
        End Set
    End Property

    Public Property BackshopCount() As String
        Get
            Return m_BackShopCount
        End Get
        Set(ByVal value As String)
            m_BackShopCount = value
        End Set
    End Property
    Public Property SalesFloorCount() As String
        Get
            Return m_SalesFloorCount
        End Get
        Set(ByVal value As String)
            m_SalesFloorCount = value
        End Set
    End Property
    Public Property OSSRCount() As String
        Get
            Return m_OSSRCount
        End Get
        Set(ByVal value As String)
            m_OSSRCount = value
        End Set
    End Property
     Public Property SeqNumber() As String
        Get
            Return m_strSeqNumber
        End Get
        Set(ByVal value As String)
            m_strSeqNumber = value
        End Set
    End Property
     ''' <summary>
    ''' Gets or sets the SiteType for Create own list
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SiteType() As String
        Get
            Return m_strSiteType
        End Get
        Set(ByVal value As String)
            m_strSiteType = value
        End Set
    End Property
#End If
    ''' <summary>
    ''' Constructor 
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
    End Sub
 
    ''' <summary>
    ''' Gets or sets the List ID
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ListID() As String
        Get
            Return m_ListID
        End Get
        Set(ByVal value As String)
            m_ListID = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the List type
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ListType() As String
        Get
            Return m_ListType
        End Get
        Set(ByVal value As String)
            m_ListType = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the list description
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ListDescription() As String
        Get
            Return m_ListDescription
        End Get
        Set(ByVal value As String)
            m_ListDescription = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the number of items
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NumberOfItems() As Integer
        Get
            Return m_NumberOfItems
        End Get
        Set(ByVal value As Integer)
            m_NumberOfItems = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the IsLocked field
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsUpdate() As Boolean
        Get
            Return m_bIsUpdate
        End Get
        Set(ByVal value As Boolean)
            m_bIsUpdate = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the date of last coutn for a list
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DateOfLastCount() As String
        Get
            Return m_DateOfLastCount
        End Get
        Set(ByVal value As String)
            m_DateOfLastCount = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the count of BS PSP items
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BackShopPSPCount() As Integer
        Get
            Return m_iBS_PSP
        End Get
        Set(ByVal value As Integer)
            m_iBS_PSP = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the count of OSSR PSP items
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OSSRPSPCount() As Integer
        Get
            Return m_iOSSR_PSP
        End Get
        Set(ByVal value As Integer)
            m_iOSSR_PSP = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the count of OSSR PSP items
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NotOnPlannerItemCount() As Integer
        Get
            Return m_NotPlannerItemCount
        End Get
        Set(ByVal value As Integer)
            m_NotPlannerItemCount = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the not on planner item list
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NotOnPlannerItemList() As ArrayList
        Get
            Return m_arrNotOnPlannerItemList
        End Get
        Set(ByVal value As ArrayList)
            m_arrNotOnPlannerItemList = value
        End Set
    End Property
   
    ''' <summary>
    ''' Gets or sets the count of SF items
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SFItemCount() As Integer
        Get
            Return m_iSFItemcount
        End Get
        Set(ByVal value As Integer)
            m_iSFItemcount = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the count of BS items
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BSItemCount() As Integer
        Get
            Return m_iBSItemCount
        End Get
        Set(ByVal value As Integer)
            m_iBSItemCount = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the count of SF items
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OSSRItemCount() As Integer
        Get
            Return m_iOSSRItemCount
        End Get
        Set(ByVal value As Integer)
            m_iOSSRItemCount = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the complte status
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IsComplete() As Boolean
        Get
            Return m_bIsCompleted
        End Get
        Set(ByVal value As Boolean)
            m_bIsCompleted = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the SF counted items
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SFCountedItems() As Integer
        Get
            Return m_SFCountedItems
        End Get
        Set(ByVal value As Integer)
            m_SFCountedItems = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the BS counted items
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BSCountedItems() As Integer
        Get
            Return m_BSCountedItems
        End Get
        Set(ByVal value As Integer)
            m_BSCountedItems = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the OSSR Counted items
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OSSRCountedItems() As Integer
        Get
            Return m_OSSRCoutedItems
        End Get
        Set(ByVal value As Integer)
            m_OSSRCoutedItems = value
        End Set
    End Property
    Public Property UnknownItemList() As ArrayList
        Get
            Return m_arrUnknownItemList
        End Get
        Set(ByVal value As ArrayList)
            m_arrUnknownItemList = value
        End Set
    End Property
    Public Property UnknownItemCount() As Integer
        Get
            Return m_UnknownItemCount
        End Get
        Set(ByVal value As Integer)
            m_UnknownItemCount = value
        End Set
    End Property
    Public Property IsActive() As Boolean
        Get
            Return m_IsActive
        End Get
        Set(ByVal value As Boolean)
            m_IsActive = value
        End Set
    End Property
    Public Property CurrentItemCount() As Integer
        Get
            Return m_iCurrentItemCount
        End Get
        Set(ByVal value As Integer)
            m_iCurrentItemCount = value
        End Set
    End Property

    Public Property CounterID() As String
        Get
            Return m_strCounterID
        End Get
        Set(ByVal value As String)
            m_strCounterID = value
        End Set
    End Property
End Class