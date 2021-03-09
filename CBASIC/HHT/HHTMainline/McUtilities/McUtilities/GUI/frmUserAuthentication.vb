Imports System
Imports System.IO
Imports System.String

''' <summary>
''' The form for User Authentication
''' </summary>
''' <remarks></remarks>
Public Class frmUserAuthentication
    Inherits System.Windows.Forms.Form
    Public CurrentControl As TextBox
    Private strPassword As String = ""
    Private strUserId As String = Nothing

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        GetUserID()
        GeneratePassword()
        ' Add any initialization after the InitializeComponent() call.

    End Sub
    ''' <summary>
    ''' Function to set the User ID
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetUserID()
        'Getting the uSer ID from the configuration file and setting the corresponding value
        strUserId = ConfigDataMgr.GetInstance.GetParam("Default_UserID").ToString
    End Sub
    ''' <summary>
    ''' Function to Generate Password
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GeneratePassword()
        Try
            'Getting the date in the needed Format
            Dim strPasswd As String = DateAndTime.Today.ToString("ddMMyy")
            Dim iSum As Integer
            Dim strTempDate As String = strPasswd
            Dim iPassword As Integer
            'Sequence to be used
            Dim arrPassSeq() As String = ConfigDataMgr.GetInstance.GetParam("PasswordSeq").ToString.Split(",")
            Dim Password_Array(8) As Integer
            Dim iCount As Integer
            'Finding the sum of the digits
            While strTempDate.Length > 0
                iSum = iSum + (CInt(strTempDate) Mod 10)
                strTempDate = strTempDate.Substring(0, (strTempDate.Length - 1))
            End While
            'Adding the Sum to the date
            iPassword = CLng(strPasswd) + iSum
            iPassword = iPassword * 2867
            strPasswd = Str(iPassword)
            'Checking the length and Padding 0  or removing the significant digit appropriately
            If strPasswd.Length > 8 Then
                strPasswd = strPasswd.Substring((strPasswd.Length - 8), 8)
            ElseIf strPasswd.Length < 8 Then
                strPasswd = strPasswd.PadLeft(8, "0")
            End If
            ' Spllit the numbers into Variables
            iCount = 7
            While strPasswd.Length > 0
                Password_Array(iCount) = (CLng(strPasswd) Mod 10)
                strPasswd = strPasswd.Substring(0, (strPasswd.Length - 1))
                iCount = iCount - 1
            End While
            strPasswd = ""
            iCount = 0
            'Rearraging the array to get the Password
            While strPassword.Length < 8
                strPasswd = Str(Password_Array(CLng(arrPassSeq(iCount) - 1)))
                strPassword = strPassword + strPasswd.TrimStart(" ")
                iCount = iCount + 1
            End While
        Catch ex As Exception

        End Try
    End Sub
    ''' <summary>
    ''' To declare all the User Authentication details
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbHelp.Click
        MessageBox.Show("Type in your user ID and password then press 'Sign On'. If you make a" & _
         " mistake press 'Del' to delete last key or 'Clr' to start again", "Admin Login - Help", MessageBoxButtons.OK, _
            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
    End Sub
    Private Sub pbNum1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum1.Click
        If Len(CurrentControl.Text) < Macros.MAX_PASSWORD Then
            CurrentControl.Text += "1"
            CurrentControl.SelectionStart = Len(CurrentControl.Text)
        End If
        'If tbPassword.Focused = True Then
        '    If Len(tbPassword.Text) < Macros.MAX_PASSWORD Then
        '        tbPassword.Text += "1"
        '        tbPassword.SelectionStart = Len(tbPassword.Text)
        '    End If
        'Else
        '    If Len(tbUserID.Text) < Macros.MAX_PASSWORD Then
        '        tbUserID.Text += "1"
        '        tbUserID.SelectionStart = Len(tbUserID.Text)
        '    End If
        'End If
    End Sub
    Private Sub pbNum2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum2.Click
        If Len(CurrentControl.Text) < Macros.MAX_PASSWORD Then
            CurrentControl.Text += "2"
            CurrentControl.SelectionStart = Len(CurrentControl.Text)
        End If
        'If tbPassword.Focused = True Then
        '    If Len(tbPassword.Text) < Macros.MAX_PASSWORD Then
        '        tbPassword.Text += "2"
        '        tbPassword.SelectionStart = Len(tbPassword.Text)
        '    End If
        'Else
        '    If Len(tbUserID.Text) < Macros.MAX_PASSWORD Then
        '        tbUserID.Text += "2"
        '        tbUserID.SelectionStart = Len(tbUserID.Text)
        '    End If
        'End If
    End Sub
    Private Sub pbNum3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum3.Click
        If Len(CurrentControl.Text) < Macros.MAX_PASSWORD Then
            CurrentControl.Text += "3"
            CurrentControl.SelectionStart = Len(CurrentControl.Text)
        End If
        'If tbPassword.Focused = True Then
        '    If Len(tbPassword.Text) < Macros.MAX_PASSWORD Then
        '        tbPassword.Text += "3"
        '        tbPassword.SelectionStart = Len(tbPassword.Text)
        '    End If
        'Else
        '    If Len(tbUserID.Text) < Macros.MAX_PASSWORD Then
        '        tbUserID.Text += "3"
        '        tbUserID.SelectionStart = Len(tbUserID.Text)
        '    End If
        'End If
    End Sub
    Private Sub pbNum4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum4.Click
        If Len(CurrentControl.Text) < Macros.MAX_PASSWORD Then
            CurrentControl.Text += "4"
            CurrentControl.SelectionStart = Len(CurrentControl.Text)
        End If
        'If tbPassword.Focused = True Then
        '    If Len(tbPassword.Text) < Macros.MAX_PASSWORD Then
        '        tbPassword.Text += "4"
        '        tbPassword.SelectionStart = Len(tbPassword.Text)
        '    End If
        'Else
        '    If Len(tbUserID.Text) < Macros.MAX_PASSWORD Then
        '        tbUserID.Text += "4"
        '        tbUserID.SelectionStart = Len(tbUserID.Text)
        '    End If
        'End If
    End Sub
    Private Sub pbNum5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum5.Click
        If Len(CurrentControl.Text) < Macros.MAX_PASSWORD Then
            CurrentControl.Text += "5"
            CurrentControl.SelectionStart = Len(CurrentControl.Text)
        End If
        'If tbPassword.Focused = True Then
        '    If Len(tbPassword.Text) < Macros.MAX_PASSWORD Then
        '        tbPassword.Text += "5"
        '        tbPassword.SelectionStart = Len(tbPassword.Text)
        '    End If
        'Else
        '    If Len(tbUserID.Text) < Macros.MAX_PASSWORD Then
        '        tbUserID.Text += "5"
        '        tbUserID.SelectionStart = Len(tbUserID.Text)
        '    End If
        'End If
    End Sub
    Private Sub pbNum6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum6.Click
        If Len(CurrentControl.Text) < Macros.MAX_PASSWORD Then
            CurrentControl.Text += "6"
            CurrentControl.SelectionStart = Len(CurrentControl.Text)
        End If
        'If tbPassword.Focused = True Then
        '    If Len(tbPassword.Text) < Macros.MAX_PASSWORD Then
        '        tbPassword.Text += "6"
        '        tbPassword.SelectionStart = Len(tbPassword.Text)
        '    End If
        'Else
        '    If Len(tbUserID.Text) < Macros.MAX_PASSWORD Then
        '        tbUserID.Text += "6"
        '        tbUserID.SelectionStart = Len(tbUserID.Text)
        '    End If
        'End If
    End Sub
    Private Sub pbNum7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum7.Click
        If Len(CurrentControl.Text) < Macros.MAX_PASSWORD Then
            CurrentControl.Text += "7"
            CurrentControl.SelectionStart = Len(CurrentControl.Text)
        End If
        'If tbPassword.Focused = True Then
        '    If Len(tbPassword.Text) < Macros.MAX_PASSWORD Then
        '        tbPassword.Text += "7"
        '        tbPassword.SelectionStart = Len(tbPassword.Text)
        '    End If
        'Else
        '    If Len(tbUserID.Text) < Macros.MAX_PASSWORD Then
        '        tbUserID.Text += "7"
        '        tbUserID.SelectionStart = Len(tbUserID.Text)
        '    End If
        'End If
    End Sub
    Private Sub pbNum8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum8.Click
        If Len(CurrentControl.Text) < Macros.MAX_PASSWORD Then
            CurrentControl.Text += "8"
            CurrentControl.SelectionStart = Len(CurrentControl.Text)
        End If
        'If tbPassword.Focused = True Then
        '    If Len(tbPassword.Text) < Macros.MAX_PASSWORD Then
        '        tbPassword.Text += "8"
        '        tbPassword.SelectionStart = Len(tbPassword.Text)
        '    End If
        'Else
        '    If Len(tbUserID.Text) < Macros.MAX_PASSWORD Then
        '        tbUserID.Text += "8"
        '        tbUserID.SelectionStart = Len(tbUserID.Text)
        '    End If
        'End If
    End Sub
    Private Sub pbNum9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum9.Click
        If Len(CurrentControl.Text) < Macros.MAX_PASSWORD Then
            CurrentControl.Text += "9"
            CurrentControl.SelectionStart = Len(CurrentControl.Text)
        End If
        'If tbPassword.Focused = True Then
        '    If Len(tbPassword.Text) < Macros.MAX_PASSWORD Then
        '        tbPassword.Text += "9"
        '        tbPassword.SelectionStart = Len(tbPassword.Text)
        '    End If
        'Else
        '    If Len(tbUserID.Text) < Macros.MAX_PASSWORD Then
        '        tbUserID.Text += "9"
        '        tbUserID.SelectionStart = Len(tbUserID.Text)
        '    End If
        'End If
    End Sub
    Private Sub pbNum0_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum0.Click
        If Len(CurrentControl.Text) < Macros.MAX_PASSWORD Then
            CurrentControl.Text += "0"
            CurrentControl.SelectionStart = Len(CurrentControl.Text)
        End If
        'If tbPassword.Focused = True Then
        '    If Len(tbPassword.Text) < Macros.MAX_PASSWORD Then
        '        tbPassword.Text += "0"
        '        tbPassword.SelectionStart = Len(tbPassword.Text)
        '    End If
        'Else
        '    If Len(tbUserID.Text) < Macros.MAX_PASSWORD Then
        '        tbUserID.Text += "0"
        '        tbUserID.SelectionStart = Len(tbUserID.Text)
        '    End If
        'End If
    End Sub
    Private Sub pbDel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbDel.Click
        If tbPassword.Focused = True Then
            If Len(tbPassword.Text) > 0 Then
                tbPassword.Text = SWLeft(tbPassword.Text, Len(tbPassword.Text) - 1)
            End If
            tbPassword.Focus()
            tbPassword.SelectionStart = Len(tbPassword.Text)
        Else
            If Len(tbUserID.Text) > 0 Then
                tbUserID.Text = SWLeft(tbUserID.Text, Len(tbUserID.Text) - 1)
            End If
            tbUserID.Focus()
            tbUserID.SelectionStart = Len(tbUserID.Text)
        End If

    End Sub
    Private Sub pbClr_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbClr.Click
        tbUserID.Text = ""
        tbPassword.Text = ""
        tbUserID.Focus()
        '  objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
    End Sub
    Private Sub pbEnter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbEnter.Click
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            'Declare local variable for holding the user entered text
            Dim strUserEntered As String = Nothing
            'Store the entered text in a local variable
            strUserEntered = tbPassword.Text

            'Validate the user credentials
            If tbUserID.Text = "" And Not tbPassword.Text = "" Then
                MessageBox.Show("Please enter the User ID", "Info " _
                                , MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                tbUserID.Focus()
            ElseIf tbPassword.Text = "" And Not tbUserID.Text = "" Then
                MessageBox.Show("Please enter the Password", "Info " _
                               , MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
            ElseIf tbUserID.Text = "" And tbPassword.Text = "" Then
                MessageBox.Show("Please enter the User ID and Password", "Info " _
                               , MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
            Else
                If (tbUserID.Text = strUserId) And (tbPassword.Text = strPassword) Then
                    'AuthenticationManager.GetInstance.EndSession()
                    DialogResult = Windows.Forms.DialogResult.OK
                    ' Else
                    'Show pop up message saying invalid credentials
                    Me.tbPassword.Text = ""
                    Me.Refresh()
                Else
                    MessageBox.Show("Invalid User ID/Password", "Info " _
                               , MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                End If
            End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
        Catch ex As Exception
            'MessageBox.Show(ex.Message)
            DialogResult = Windows.Forms.DialogResult.Cancel
        End Try
    End Sub
    Private Sub pbExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbExit.Click
        'Set the status bar
        '  objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
        'Decale a variable
        Dim diaresult As New DialogResult
        'Messagebox for confirming quit
        diaresult = MessageBox.Show("Do you really want to quit", _
                                    "Alert", _
                                    MessageBoxButtons.YesNo, _
                                    MessageBoxIcon.Question, _
                                    MessageBoxDefaultButton.Button1)
        If diaresult = Windows.Forms.DialogResult.No Then
            ' Show pop up message saying invalid credentials
            Me.tbPassword.Text = ""
            Me.Refresh()
            'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        ElseIf diaresult = Windows.Forms.DialogResult.Yes Then
            'Dispose the User Authentication module
            DialogResult = Windows.Forms.DialogResult.Cancel
        End If
    End Sub
    Public Function SWLeft(ByVal sString As String, ByVal lLth As Long)
        'Delete the value appended to the right most side of the string
        SWLeft = sString.Substring(0, lLth)
    End Function
    'Private Sub frmUserAuthentication_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
    ' objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
    'End Sub

    Private Sub frmUserAuthentication_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        tbPassword.Focus()
        CurrentControl = tbPassword
    End Sub

    Private Sub btnTab_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTab.Click
        If tbUserID.Focused = True Then
            tbPassword.Focus()
        Else
            tbUserID.Focus()
        End If
    End Sub

    Private Sub tbUserID_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tbUserID.GotFocus
        CurrentControl = tbUserID
    End Sub
    Private Sub tbPassword_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tbPassword.GotFocus
        CurrentControl = tbPassword
    End Sub
End Class