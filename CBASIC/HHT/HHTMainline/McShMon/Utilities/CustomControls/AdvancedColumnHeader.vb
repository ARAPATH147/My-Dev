Imports System.Windows.Forms
''' <summary>
    ''' AdvancedColumnHeader provides the Sorting property to set the sort order for the column,
    ''' and the SortComparer property to set the comparer to be used to sort items in the 
    ''' column.
    ''' </summary>
    ''' <remarks></remarks>
    
Public Class AdvancedColumnHeader
    Inherits ColumnHeader
    Private bAscending As Boolean = False

    Private objListviewSorter As ListViewComparer = Nothing
    Private strColName As String = String.Empty

    Public Property ColumnName() As String
        Get
            Return strColName
        End Get
        Set(ByVal value As String)
            strColName = value
        End Set
    End Property


    Public Sub New(ByVal strText As String)
        Me.Text = strText
        strColName = strText
    End Sub

    'Sort Type (ascending or descending)
    Public Property Ascending() As Boolean
        Get
            Return bAscending
        End Get
        Set(ByVal value As Boolean)
            bAscending = value
            SortType.Sorting = bAscending
        End Set
    End Property


    'Returns the type of column to be sorted
    Public Property SortType() As ListViewComparer
        Get
            Return objListviewSorter
        End Get
        Set(ByVal value As ListViewComparer)
            objListviewSorter = value
        End Set
    End Property

End Class
