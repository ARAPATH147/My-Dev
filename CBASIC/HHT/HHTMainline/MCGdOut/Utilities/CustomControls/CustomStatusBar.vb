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
    ''' 
    Public Sub SetMessage(ByVal messageStatus As MSGTYPE)
        Dim strMessage As String = ""
        Dim strActiveDataTime As DateTime
        Dim strLastActBuildTime As String = Nothing
        'Checks for the type of message and does the appropriate processing
        Select Case messageStatus
            Case MSGTYPE.ACT_DATATTIME
                'System Testing Fix for Status bar message for RF mode.
#If RF Then
                Cursor.Current = Cursors.Default
                lblMessage.Text = "Connected..."
#ElseIf NRF Then

                Cursor.Current = Cursors.Default
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
        Application.DoEvents()

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
        Application.DoEvents()
        Cursor.Current = Cursors.WaitCursor

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
    End Enum
End Class

