<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmDummyListView
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.advancedListView1 = New MCShMon.AdvancedListView
        Me.SuspendLayout()
        '
        'advancedListView1
        '
        Me.advancedListView1.FullRowSelect = True
        Me.advancedListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.advancedListView1.Location = New System.Drawing.Point(33, 55)
        Me.advancedListView1.Name = "advancedListView1"
        Me.advancedListView1.Size = New System.Drawing.Size(169, 113)
        Me.advancedListView1.TabIndex = 0
        Me.advancedListView1.View = System.Windows.Forms.View.Details
        '
        'frmDummyListView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.Controls.Add(Me.advancedListView1)
        Me.Name = "frmDummyListView"
        Me.Text = "FrmDummyListView"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents advancedListView1 As AdvancedListView
    'Friend WithEvents AdvancedColumnHeader1 As AdvancedColumnHeader
    'Friend WithEvents AdvancedColumnHeader2 As AdvancedColumnHeader
    'Friend WithEvents AdvancedColumnHeader3 As AdvancedColumnHeader

End Class
