<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSummaryScreen
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSummaryScreen))
        Me.SummaryList = New System.Windows.Forms.ListView
        Me.FileName = New System.Windows.Forms.ColumnHeader
        Me.Downloading = New System.Windows.Forms.ColumnHeader
        Me.btnOk = New System.Windows.Forms.PictureBox
        Me.objStatusBar = New McUtilities.CustomStatusBar
        Me.SuspendLayout()
        '
        'SummaryList
        '
        Me.SummaryList.Columns.Add(Me.FileName)
        Me.SummaryList.Columns.Add(Me.Downloading)
        Me.SummaryList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.SummaryList.Location = New System.Drawing.Point(0, 5)
        Me.SummaryList.Name = "SummaryList"
        Me.SummaryList.Size = New System.Drawing.Size(240, 221)
        Me.SummaryList.TabIndex = 0
        Me.SummaryList.View = System.Windows.Forms.View.Details
        '
        'FileName
        '
        Me.FileName.Text = "File Name"
        Me.FileName.Width = 100
        '
        'Downloading
        '
        Me.Downloading.Text = "Download status"
        Me.Downloading.Width = 139
        '
        'btnOk
        '
        Me.btnOk.Image = CType(resources.GetObject("btnOk.Image"), System.Drawing.Image)
        Me.btnOk.Location = New System.Drawing.Point(103, 232)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(41, 37)
        Me.btnOk.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'objStatusBar
        '
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 97
        '
        'frmSummaryScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.SummaryList)
        Me.Name = "frmSummaryScreen"
        Me.Text = "Summary Screen"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SummaryList As System.Windows.Forms.ListView
    Friend WithEvents FileName As System.Windows.Forms.ColumnHeader
    Friend WithEvents Downloading As System.Windows.Forms.ColumnHeader
    Friend WithEvents btnOk As System.Windows.Forms.PictureBox
    Public WithEvents objStatusBar As McUtilities.CustomStatusBar
End Class
