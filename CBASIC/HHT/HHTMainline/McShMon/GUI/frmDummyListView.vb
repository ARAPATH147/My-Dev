Public Class frmDummyListView
    Dim AdvancedColumnHeader1 As AdvancedColumnHeader
    Dim AdvancedColumnHeader2 As AdvancedColumnHeader
    Dim AdvancedColumnHeader3 As AdvancedColumnHeader
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        Me.advancedListView1.CurrentSortedColumn = Nothing
        Me.AdvancedColumnHeader1 = New AdvancedColumnHeader("Name")
        Me.AdvancedColumnHeader2 = New AdvancedColumnHeader("Age")
        Me.AdvancedColumnHeader3 = New AdvancedColumnHeader("EmpId")

        Me.advancedListView1.Columns.Add(Me.AdvancedColumnHeader1)
        Me.advancedListView1.Columns.Add(Me.AdvancedColumnHeader2)
        Me.advancedListView1.Columns.Add(Me.AdvancedColumnHeader3)

        Me.advancedListView1.Items.Add(New ListViewItem(New String() {"Richard", "4", System.DateTime.Now.ToString()}))
        Me.advancedListView1.Items.Add(New ListViewItem(New String() {"John", "5", System.DateTime.Now.AddDays(-1).ToString()}))
        Me.advancedListView1.Items.Add(New ListViewItem(New String() {"Anand", "6", System.DateTime.Now.AddDays(-2).ToString()}))
        Me.advancedListView1.Items.Add(New ListViewItem(New String() {"Karthik", "7", System.DateTime.Now.AddDays(-9).ToString()}))

        AdvancedColumnHeader1.SortType = New ComparerString()
        AdvancedColumnHeader2.SortType = New ComparerStringAsInt()
        AdvancedColumnHeader3.SortType = New ComparerStringAsDateTime()
        AdvancedColumnHeader1.Ascending = True

    End Sub

End Class
