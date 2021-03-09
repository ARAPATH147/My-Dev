Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.IO
Imports System.Collections


''' The AdvancedListView class extends the existing .Net Compact Framework ListView class, 
''' when it used in a details view, with ListView.View set to View.Details. In details view, 
''' the ListView control has a column header. The AdvancedListView class will automatically sort 
''' the items in the ListView control when a column header is clicked.

Public Class AdvancedListView
    Inherits ListView
    Private cntxMenu As New ContextMenu()
    'Event handler for implementing the press and hold functionality        
    Public Event OnPressAndHold As EventHandler
    Private objCurrColumn As AdvancedColumnHeader = Nothing

    'To get the clicked column
    Public Property CurrentSortedColumn() As AdvancedColumnHeader
        Get
            Return objCurrColumn
        End Get
        Set(ByVal value As AdvancedColumnHeader)
            objCurrColumn = value
        End Set
    End Property
    'Constructor
    Public Sub New()
        Me.View = View.Details
        AddHandler cntxMenu.Popup, AddressOf cntxMenu_Popup
        AddHandler Me.ColumnClick, AddressOf AdvancedListView_ColumnClick
        Me.ContextMenu = cntxMenu
    End Sub
    Private objCurrSorting As EnumClass.SortOrder = EnumClass.SortOrder.None
    Private Property CurrentSorting() As EnumClass.SortOrder
        Get
            Return objCurrSorting
        End Get
        Set(ByVal value As EnumClass.SortOrder)
            objCurrSorting = value
        End Set
    End Property

    'This method is for adding the icon to the current sorted column depending on the sorting type
    Private Sub changeHeaderName(ByVal clickedCol As AdvancedColumnHeader, ByVal ascending As Boolean)
        Dim strColName As String = String.Empty
        Dim iIndex As Integer = clickedCol.Text.IndexOf(" ")
        If iIndex <> -1 Then
            strColName = clickedCol.Text.Substring(0, iIndex)
        Else
            strColName = clickedCol.Text
        End If
        If CurrentSortedColumn IsNot Nothing AndAlso clickedCol IsNot Me.CurrentSortedColumn Then
            CurrentSortedColumn.Text = CurrentSortedColumn.ColumnName
        End If
        If ascending Then
            clickedCol.Text = strColName + " /\"
        Else
            clickedCol.Text = strColName + " \/"
        End If
    End Sub


    ' This event triggers the press and hold event

    Private Sub cntxMenu_Popup(ByVal sender As Object, ByVal e As EventArgs)
        RaiseEvent OnPressAndHold(sender, e)
    End Sub


    Sub AdvancedListView_ColumnClick(ByVal sender As Object, ByVal e As ColumnClickEventArgs)
        ' Create an instance of the ColHeader class.
        Dim clickedCol As AdvancedColumnHeader = DirectCast(Me.Columns(e.Column), AdvancedColumnHeader)

        clickedCol.SortType.SortColumn = e.Column
        clickedCol.Ascending = Not clickedCol.Ascending


        Dim arrayItems As ListViewItem() = New ListViewItem(Items.Count) {}
        Items.CopyTo(arrayItems, 0)

        ' Sort the copied items.
        Array.Sort(arrayItems, 0, arrayItems.Length, clickedCol.SortType)

        ' Turn off display while data is repoplulated.
        Me.BeginUpdate()

        ' Before clearing the existing items in the control, save which ones are selected.

        Items.Clear()

        For Each lvItem As ListViewItem In arrayItems
            Items.Add(lvItem)
        Next

        Me.EndUpdate()
    End Sub
End Class

