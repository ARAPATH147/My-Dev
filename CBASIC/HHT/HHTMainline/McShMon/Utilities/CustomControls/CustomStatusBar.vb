Imports System
Imports System.Drawing
Imports System.Collections
Imports System.Windows.Forms
Imports System.Data
Imports System.Runtime.InteropServices
'''***************************************************************
''' <FileName>CustomStatusBar.vb</FileName>
''' <summary>
''' This class defines the custom status bar to display information to the user
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>30 Oct 2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
Public Class CustomStatusBar
    Private Event MessageChanged(ByVal strMessage As String)
    ''' <summary>
    ''' Allows the user to set the message on the label
    ''' </summary>
    ''' <param name="messageStatus"></param>
    ''' <remarks></remarks>

     Public Sub SetMessage(ByVal messageStatus As MSGTYPE)
        Dim strMessage As String = ""
#If NRF Then
        Dim strActiveDataTime As DateTime
#End If

        Dim strLastActBuildTime As String = Nothing
        'Checks for the type of message and does the appropriate processing
        Select Case messageStatus
            Case MSGTYPE.ACT_DATATTIME
#If RF Then
                If objAppContainer.ConnectionStatus Then
                    lblMessage.Text = "Connected..."
                Else
                    lblMessage.Text = "Disconnected..."
                End If
                Cursor.Current = Cursors.Default
#ElseIf NRF Then
                Cursor.Current = Cursors.Default
                strLastActBuildTime = ConfigDataMgr.GetInstance.GetParam(ConfigKey.LAST_ACTBUILD_TIME)
                strActiveDataTime = DateTime.ParseExact(strLastActBuildTime, "yyyy-MM-dd HH:mm:ss", Nothing)
                strMessage = "Dock && Transmit: " + _
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
        End Select
        lblMessage.Invalidate()
        lblMessage.Refresh()
    'Application.DoEvents()
    End Sub
    Public Sub SetMessage(ByVal messageStatus As MSGTYPE, ByVal strMessage As String)
        RaiseEvent MessageChanged(strMessage)
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
        lblMessage.Text = strMessage
    End Sub
    Private Sub EventMessageChanged(ByVal strMessage As String)
        lblMessage.Text = strMessage
        lblMessage.Invalidate()
        'Application.DoEvents()
        lblMessage.Refresh()
        Cursor.Current = Cursors.WaitCursor
    End Sub
    ''' <summary>
    ''' Enum to handle message type
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum MSGTYPE
        ''' <summary>
        ''' Shows "Please Wait" in status bar and shows sand clock
        ''' </summary>
        ''' <remarks></remarks>
        PLEASE_WAIT
        ''' <summary>
        ''' Shows "Processing" in status bar and shows sand clock
        ''' </summary>
        ''' <remarks></remarks>
        PROCESSING
#If NRF Then
        ''' <summary>
        ''' Last Active data Upload Time
        ''' </summary>
        ''' <remarks></remarks>
#ElseIf RF Then
        ''' <summary>
        ''' Shows Connection or disconnection status based on Appcontainer, Connection status flag
        ''' </summary>
        ''' <remarks></remarks>
#End If
        ACT_DATATTIME
        ''' <summary>
        ''' shows no text in status bar
        ''' </summary>
        ''' <remarks></remarks>
        EMPTY
        CUSTOM
    End Enum
End Class

