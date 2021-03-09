Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Collections
Imports System.Windows.Forms

Public MustInherit Class ListViewComparer
    Implements IComparer
    Private bSorting As Boolean = False
    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
        Dim lvItemX As ListViewItem = DirectCast(x, ListViewItem)
        Dim lvItemY As ListViewItem = DirectCast(y, ListViewItem)

        Return IIf(Sorting, Compare(lvItemX, lvItemY), Compare(lvItemY, lvItemX))
    End Function

    Public Property Sorting() As Boolean
        Get
            Return bSorting
        End Get
        Set(ByVal value As Boolean)
            bSorting = value
        End Set
    End Property
    Private iIndex As Integer = -1


    Public Property SortColumn() As Integer
        Get
            Return iIndex
        End Get
        Set(ByVal value As Integer)
            iIndex = value
        End Set
    End Property
    Protected MustOverride Function Compare(ByVal lvItemA As ListViewItem, ByVal lvItemB As ListViewItem) As Integer
End Class


Public Class ComparerString
    Inherits ListViewComparer
    ''' <summary>
    ''' Compares two specified ListViewItem objects as strings.
    ''' </summary>

    Protected Overloads Overrides Function Compare(ByVal lvItemA As ListViewItem, ByVal lvItemB As ListViewItem) As Integer
        Return lvItemA.SubItems(SortColumn).Text.CompareTo(lvItemB.SubItems(SortColumn).Text)
    End Function
End Class

Public Class ComparerStringAsInt
    Inherits ListViewComparer
    ''' <summary>
    ''' Compares two specified ListViewItem objects as ints.
    ''' </summary>

    Protected Overloads Overrides Function Compare(ByVal lvItemA As ListViewItem, ByVal lvItemB As ListViewItem) As Integer
        Return Int32.Parse(lvItemA.SubItems(SortColumn).Text).CompareTo(Int32.Parse(lvItemB.SubItems(SortColumn).Text))
    End Function
End Class
Public Class ComparerStringAsDateTime
    Inherits ListViewComparer
    ''' <summary>
    ''' Compares two specified ListViewItem objects as datetimes.
    ''' </summary>

    Protected Overloads Overrides Function Compare(ByVal lvItemA As ListViewItem, ByVal lvItemB As ListViewItem) As Integer
        Return DateTime.Parse(lvItemA.SubItems(SortColumn).Text).CompareTo(DateTime.Parse(lvItemB.SubItems(SortColumn).Text))
    End Function
End Class

