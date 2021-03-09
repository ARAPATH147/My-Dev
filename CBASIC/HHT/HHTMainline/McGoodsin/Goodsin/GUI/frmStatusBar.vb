Imports System
Imports System.Drawing
Imports System.Collections
Imports System.Windows.Forms
Imports System.Data
Imports System.Runtime.InteropServices
Public Class StatusBar
    Inherits System.Windows.Forms.Form
    Private Event MessageChanged(ByVal strMessage As String)
    Private WithEvents tmrMainMenu As New System.Windows.Forms.Timer

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents lblMessage As System.Windows.Forms.TextBox
    Friend WithEvents lblStoreId As System.Windows.Forms.TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.lblMessage = New System.Windows.Forms.TextBox
        Me.lblStoreId = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'lblMessage
        '
        Me.lblMessage.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.lblMessage.BackColor = System.Drawing.Color.WhiteSmoke
        Me.lblMessage.Location = New System.Drawing.Point(0, 274)
        Me.lblMessage.Name = "lblMessage"
        Me.lblMessage.Size = New System.Drawing.Size(192, 21)
        Me.lblMessage.TabIndex = 1
        Me.lblMessage.Text = "ACTIVE FILE DOWNLOAD"
        Me.lblMessage.WordWrap = False
        '
        'lblStoreId
        '
        Me.lblStoreId.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.lblStoreId.BackColor = System.Drawing.Color.DarkGray
        Me.lblStoreId.Location = New System.Drawing.Point(192, 274)
        Me.lblStoreId.Name = "lblStoreId"
        Me.lblStoreId.Size = New System.Drawing.Size(48, 21)
        Me.lblStoreId.TabIndex = 0
        Me.lblStoreId.Text = "1190"
        Me.lblStoreId.WordWrap = False
        '
        'StatusBar
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblMessage)
        Me.Controls.Add(Me.lblStoreId)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "StatusBar"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region



    ''' <summary>
    ''' Allows the user to set the message on the label
    ''' </summary>
    ''' <param name="messageStatus"></param>
    ''' <remarks></remarks>
    ''' 
    Public Sub SetMessage(ByVal messageStatus As MSGTYPE)
        Dim strMessage As String = ""
        Dim strActiveDataTime As DateTime
        Dim strLastActBuildTime As String = Nothing

        'Checks for the type of message and does the appropriate processing
        Select Case messageStatus
            Case MSGTYPE.ACT_DATATTIME
#If RF Then
                Cursor.Current = Cursors.Default
                lblMessage.Text = "Connected..."
                If objAppContainer.bCommFailure Then
                    lblMessage.Text = "Disconnected..."
                End If
#ElseIf NRF Then
                Cursor.Current = Cursors.Default
                strLastActBuildTime = ConfigDataMgr.GetInstance.GetParam(ConfigKey.LAST_ACTBUILD_TIME)
                strActiveDataTime = DateTime.ParseExact(strLastActBuildTime, "yyyy-MM-dd HH:mm:ss", Nothing)
                strMessage = "Last Dock & Transmit done @ " + _
                             strActiveDataTime.Hour.ToString().PadLeft(2, "0") + ":" + _
                             strActiveDataTime.Minute.ToString().PadLeft(2, "0")
                lblMessage.Text = strMessage
#End If

            Case MSGTYPE.PLEASE_WAIT
                lblMessage.Text = "Please Wait..."
                Cursor.Current = Cursors.WaitCursor
            Case MSGTYPE.PROCESSING
                lblMessage.Text = "Processing..."
                Cursor.Current = Cursors.WaitCursor
            Case MSGTYPE.EMPTY
                Cursor.Current = Cursors.Default
                lblMessage.Text = ""
            Case MSGTYPE.DOCKANDTRANSMIT
                Cursor.Current = Cursors.Default
                lblMessage.Text = "DOCK && TRANSMIT"
        End Select

        lblMessage.Invalidate()
        Application.DoEvents()

    End Sub
    Public Sub SetMessage(ByVal messageStatus As MSGTYPE, ByVal strMessage As String)
#If RF Then
        RaiseEvent MessageChanged(strMessage)
#ElseIf NRF Then
        ' RaiseEvent MessageChanged(strMessage)
        lblMessage.Text = strMessage
        Cursor.Current = Cursors.WaitCursor
#End If


    End Sub
    ''' <summary>
    ''' Allows user to set the store id
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetStoreId()
        'Sets the store id to the status bar
        Dim strStoreId As String = ""
        strStoreId = ConfigDataMgr.GetInstance().GetParam(ConfigKey.STORE_NO)
        lblStoreId.Text = strStoreId
    End Sub
    ''' <summary>
    ''' Method to set the custom message to status bar
    ''' </summary>
    ''' <param name="strMessage"></param>
    ''' <remarks></remarks>
    Public Sub SetMessage(ByVal strMessage As String)
#If RF Then
        If strMessage <> "Waiting for server to respond..." Then
            Cursor.Current = Cursors.Default
        End If
#End If
        lblMessage.Text = strMessage
    End Sub
    Private Sub EventMessageChanged(ByVal strMessage As String)

        lblMessage.Text = strMessage
        lblMessage.Invalidate()
        Application.DoEvents()
        Cursor.Current = Cursors.WaitCursor

    End Sub
    Private Sub StatusBar_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.lblMessage.BackColor = Color.Gray
        Me.lblStoreId.BackColor = Color.DarkGray
        SetStoreId()
        tmrMainMenu.Interval = 100
        AddHandler tmrMainMenu.Tick, AddressOf ShowMainMenu
        tmrMainMenu.Enabled = True
    End Sub
    Private Sub ShowMainMenu(ByVal sender As Object, ByVal e As System.EventArgs)
        tmrMainMenu.Enabled = False
        tmrMainMenu.Dispose()
        objAppContainer.objGoodsInMenu.ShowDialog()
        Me.Close()
    End Sub
    ''' <summary>
    ''' Enum to handle message type
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum MSGTYPE
        PLEASE_WAIT
        PROCESSING
        ACT_DATATTIME
        EMPTY
        CUSTOM
        DOCKANDTRANSMIT
    End Enum


End Class
