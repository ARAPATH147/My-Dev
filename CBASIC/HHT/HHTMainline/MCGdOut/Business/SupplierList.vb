'''***************************************************************
''' <FileName>SupplierList.vb</FileName>
''' <summary>
''' Stores the supplier list fetched from the data access layer 
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
Public Class SupplierList
    Private m_SupplierID As String
    Public Property SupplierID() As String
        Get
            Return m_SupplierID
        End Get
        Set(ByVal value As String)
            m_SupplierID = value
        End Set
    End Property

    Private m_SupplierName As String
    Public Property SupplierName() As String
        Get
            Return m_SupplierName
        End Get
        Set(ByVal value As String)
            m_SupplierName = value
        End Set
    End Property
End Class
