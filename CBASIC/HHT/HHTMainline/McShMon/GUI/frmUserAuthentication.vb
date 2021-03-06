Imports System
Imports System.IO
Imports System.String
Imports MCShMon.SMTransactDataManager
''' <summary>
''' The form for User Authentication
''' </summary>
''' <remarks></remarks>
Public Class frmUserAuthentication
    Inherits System.Windows.Forms.Form
    ''' <summary>
    ''' To declare all the User Authentication details
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub pbHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbHelp.Click
        MessageBox.Show(MessageManager.GetInstance.GetMessage("M37"), "Help")
    End Sub
    Private Sub pbNum1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum1.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "1"
        End If
    End Sub
    Private Sub pbNum2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum2.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "2"
        End If
    End Sub
    Private Sub pbNum3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum3.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "3"
        End If
    End Sub
    Private Sub pbNum4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum4.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "4"
        End If
    End Sub
    Private Sub pbNum5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum5.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "5"
        End If
    End Sub
    Private Sub pbNum6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum6.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "6"
        End If
    End Sub
    Private Sub pbNum7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum7.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "7"
        End If
    End Sub
    Private Sub pbNum8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum8.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "8"
        End If
    End Sub
    Private Sub pbNum9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum9.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "9"
        End If
    End Sub
    Private Sub pbNum0_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNum0.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "0"
        End If
    End Sub
    Private Sub pbDel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbDel.Click
        If Len(tbSignOn.Text) > 0 Then
            tbSignOn.Text = SWLeft(tbSignOn.Text, Len(tbSignOn.Text) - 1)
        End If
        tbSignOn.Focus()
    End Sub
    Private Sub pbClr_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbClr.Click
        tbSignOn.Text = ""
        tbSignOn.Focus()
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
    End Sub
    Private Sub pbEnter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbEnter.Click
        Try
            'Declare local variable for holding the user entered text
            Dim strUserEntered As String = Nothing
            'Store the entered text in a local variable
            strUserEntered = tbSignOn.Text

            'Check for condition if user has entered a text 
            If Not tbSignOn.Text = "" Then
                'Set status bar
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            End If
            'anoop:Start
            'Validate the user credentials
            If objAppContainer.objUserSessionMgr.ValidateUser(strUserEntered) Then
                'Authentication Success
                'Assigning the passsword for Reconnect
#If RF Then
                objAppContainer.strPassword = strUserEntered.Substring(3)
#End If
                'anoop:verify reference data only in the case of NRF

#If NRF Then
            If Not objAppContainer.objHelper.VerifyRefDataUpdation Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M68"), "Info", _
                           MessageBoxButtons.OK, _
                           MessageBoxIcon.Asterisk, _
                           MessageBoxDefaultButton.Button1)
            End If
#ElseIf RF Then
                objAppContainer.strPassword = strUserEntered.Substring(3)
#End If


                'Authenticate user and apply check condition
                If objAppContainer.objUserSessionMgr.HandleUserAuthentication() Then
                    'Dispose the User Authentication module
                    objAppContainer.objUserSessionMgr.EndSession() 'anoop
                    objAppContainer.objSplashScreen.ChangeLabelText("Please wait while the application loads")
                End If
            Else
                ' Show pop up message saying invalid credentials
                Me.tbSignOn.Text = ""
                Me.Refresh()
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, Logger.LogLevel.RELEASE)
#If RF Then
            objAppContainer.objLogger.WriteAppLog("Connection Lost @ Sign on and not reestablished. closing the application", Logger.LogLevel.RELEASE)
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                objAppContainer.objSplashScreen.ChangeLabelText("Application logging off. Please wait...")
                'Dispose the User Authentication module
                objAppContainer.objUserSessionMgr.EndSession()
                'Exit from the application.
                objAppContainer.AppTerminate()
            End If
#End If
        End Try
    End Sub
    Private Sub pbExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbExit.Click
        'Set the status bar
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
        'Decale a variable
        Dim diaresult As New DialogResult
        'Messagebox for confirming quit
        diaresult = MessageBox.Show(MessageManager.GetInstance.GetMessage("M39"), _
                                    "Alert", _
                                    MessageBoxButtons.YesNo, _
                                    MessageBoxIcon.Question, _
                                    MessageBoxDefaultButton.Button1)
        If diaresult = Windows.Forms.DialogResult.No Then
            ' Show pop up message saying invalid credentials
            Me.tbSignOn.Text = ""
            Me.Refresh()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        ElseIf diaresult = Windows.Forms.DialogResult.Yes Then
            'Set splash screen message.
            objAppContainer.objSplashScreen.ChangeLabelText("Application logging off. Please wait...")
            'Dispose the User Authentication module
            objAppContainer.objUserSessionMgr.EndSession()
            'Exit from the application.
            objAppContainer.AppTerminate()
        End If
    End Sub
    Public Function SWLeft(ByVal sString As String, ByVal lLth As Long)
        'Delete the value appended to the right most side of the string
        SWLeft = sString.Substring(0, lLth)
    End Function
    Private Sub frmUserAuthentication_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
    End Sub
End Class