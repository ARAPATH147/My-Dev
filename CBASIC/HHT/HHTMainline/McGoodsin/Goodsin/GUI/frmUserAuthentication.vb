Public Class frmUserAuthentication
    Inherits System.Windows.Forms.Form
    Friend WithEvents status As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents pbExit As System.Windows.Forms.PictureBox
    Friend WithEvents pbEnter As System.Windows.Forms.PictureBox
    Friend WithEvents pbClr As System.Windows.Forms.PictureBox
    Friend WithEvents pbDel As System.Windows.Forms.PictureBox
    Friend WithEvents pbHelp As System.Windows.Forms.PictureBox
    Friend WithEvents pbThree As System.Windows.Forms.Button
    Friend WithEvents pbTwo As System.Windows.Forms.Button
    Friend WithEvents pbSix As System.Windows.Forms.Button
    Friend WithEvents pbFive As System.Windows.Forms.Button
    Friend WithEvents pbOne As System.Windows.Forms.Button
    Friend WithEvents pbNine As System.Windows.Forms.Button
    Friend WithEvents pbFour As System.Windows.Forms.Button
    Friend WithEvents pbEight As System.Windows.Forms.Button
    Friend WithEvents pbSeven As System.Windows.Forms.Button
    Friend WithEvents pbZero As System.Windows.Forms.Button
    Friend WithEvents tbSignOn As System.Windows.Forms.TextBox

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()


    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUserAuthentication))
        Me.status = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.pbExit = New System.Windows.Forms.PictureBox
        Me.pbEnter = New System.Windows.Forms.PictureBox
        Me.pbClr = New System.Windows.Forms.PictureBox
        Me.pbDel = New System.Windows.Forms.PictureBox
        Me.pbHelp = New System.Windows.Forms.PictureBox
        Me.tbSignOn = New System.Windows.Forms.TextBox
        Me.pbThree = New System.Windows.Forms.Button
        Me.pbTwo = New System.Windows.Forms.Button
        Me.pbSix = New System.Windows.Forms.Button
        Me.pbFive = New System.Windows.Forms.Button
        Me.pbOne = New System.Windows.Forms.Button
        Me.pbNine = New System.Windows.Forms.Button
        Me.pbFour = New System.Windows.Forms.Button
        Me.pbEight = New System.Windows.Forms.Button
        Me.pbSeven = New System.Windows.Forms.Button
        Me.pbZero = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'status
        '
        Me.status.BackColor = System.Drawing.Color.LightGray
        Me.status.Location = New System.Drawing.Point(0, 296)
        Me.status.Name = "status"
        Me.status.ReadOnly = True
        Me.status.Size = New System.Drawing.Size(200, 21)
        Me.status.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(16, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(128, 20)
        Me.Label1.Text = "Enter User ID"
        '
        'pbExit
        '
        Me.pbExit.Image = CType(resources.GetObject("pbExit.Image"), System.Drawing.Image)
        Me.pbExit.Location = New System.Drawing.Point(176, 73)
        Me.pbExit.Name = "pbExit"
        Me.pbExit.Size = New System.Drawing.Size(40, 40)
        Me.pbExit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbEnter
        '
        Me.pbEnter.Image = CType(resources.GetObject("pbEnter.Image"), System.Drawing.Image)
        Me.pbEnter.Location = New System.Drawing.Point(71, 216)
        Me.pbEnter.Name = "pbEnter"
        Me.pbEnter.Size = New System.Drawing.Size(90, 40)
        Me.pbEnter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbClr
        '
        Me.pbClr.Image = CType(resources.GetObject("pbClr.Image"), System.Drawing.Image)
        Me.pbClr.Location = New System.Drawing.Point(176, 169)
        Me.pbClr.Name = "pbClr"
        Me.pbClr.Size = New System.Drawing.Size(40, 40)
        Me.pbClr.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbDel
        '
        Me.pbDel.Image = CType(resources.GetObject("pbDel.Image"), System.Drawing.Image)
        Me.pbDel.Location = New System.Drawing.Point(176, 121)
        Me.pbDel.Name = "pbDel"
        Me.pbDel.Size = New System.Drawing.Size(40, 40)
        Me.pbDel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbHelp
        '
        Me.pbHelp.Image = CType(resources.GetObject("pbHelp.Image"), System.Drawing.Image)
        Me.pbHelp.Location = New System.Drawing.Point(176, 33)
        Me.pbHelp.Name = "pbHelp"
        Me.pbHelp.Size = New System.Drawing.Size(32, 32)
        Me.pbHelp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'tbSignOn
        '
        Me.tbSignOn.Font = New System.Drawing.Font("Tahoma", 18.0!, System.Drawing.FontStyle.Bold)
        Me.tbSignOn.Location = New System.Drawing.Point(18, 33)
        Me.tbSignOn.Name = "tbSignOn"
        Me.tbSignOn.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.tbSignOn.Size = New System.Drawing.Size(146, 35)
        Me.tbSignOn.TabIndex = 17
        '
        'pbThree
        '
        Me.pbThree.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbThree.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbThree.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbThree.Location = New System.Drawing.Point(124, 169)
        Me.pbThree.Name = "pbThree"
        Me.pbThree.Size = New System.Drawing.Size(40, 40)
        Me.pbThree.TabIndex = 28
        Me.pbThree.Text = "3"
        '
        'pbTwo
        '
        Me.pbTwo.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbTwo.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbTwo.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbTwo.Location = New System.Drawing.Point(71, 169)
        Me.pbTwo.Name = "pbTwo"
        Me.pbTwo.Size = New System.Drawing.Size(40, 40)
        Me.pbTwo.TabIndex = 27
        Me.pbTwo.Text = "2"
        '
        'pbSix
        '
        Me.pbSix.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbSix.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbSix.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbSix.Location = New System.Drawing.Point(124, 121)
        Me.pbSix.Name = "pbSix"
        Me.pbSix.Size = New System.Drawing.Size(40, 40)
        Me.pbSix.TabIndex = 30
        Me.pbSix.Text = "6"
        '
        'pbFive
        '
        Me.pbFive.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbFive.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbFive.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbFive.Location = New System.Drawing.Point(71, 121)
        Me.pbFive.Name = "pbFive"
        Me.pbFive.Size = New System.Drawing.Size(40, 40)
        Me.pbFive.TabIndex = 29
        Me.pbFive.Text = "5"
        '
        'pbOne
        '
        Me.pbOne.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbOne.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbOne.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbOne.Location = New System.Drawing.Point(18, 169)
        Me.pbOne.Name = "pbOne"
        Me.pbOne.Size = New System.Drawing.Size(40, 40)
        Me.pbOne.TabIndex = 26
        Me.pbOne.Text = "1"
        '
        'pbNine
        '
        Me.pbNine.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNine.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNine.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNine.Location = New System.Drawing.Point(124, 73)
        Me.pbNine.Name = "pbNine"
        Me.pbNine.Size = New System.Drawing.Size(40, 40)
        Me.pbNine.TabIndex = 23
        Me.pbNine.Text = "9"
        '
        'pbFour
        '
        Me.pbFour.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbFour.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbFour.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbFour.Location = New System.Drawing.Point(18, 121)
        Me.pbFour.Name = "pbFour"
        Me.pbFour.Size = New System.Drawing.Size(40, 40)
        Me.pbFour.TabIndex = 22
        Me.pbFour.Text = "4"
        '
        'pbEight
        '
        Me.pbEight.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbEight.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbEight.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbEight.Location = New System.Drawing.Point(71, 73)
        Me.pbEight.Name = "pbEight"
        Me.pbEight.Size = New System.Drawing.Size(40, 40)
        Me.pbEight.TabIndex = 25
        Me.pbEight.Text = "8"
        '
        'pbSeven
        '
        Me.pbSeven.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbSeven.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbSeven.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbSeven.Location = New System.Drawing.Point(18, 73)
        Me.pbSeven.Name = "pbSeven"
        Me.pbSeven.Size = New System.Drawing.Size(40, 40)
        Me.pbSeven.TabIndex = 24
        Me.pbSeven.Text = "7"
        '
        'pbZero
        '
        Me.pbZero.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbZero.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbZero.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbZero.Location = New System.Drawing.Point(18, 216)
        Me.pbZero.Name = "pbZero"
        Me.pbZero.Size = New System.Drawing.Size(40, 40)
        Me.pbZero.TabIndex = 31
        Me.pbZero.Text = "0"
        '
        'frmUserAuthentication
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 295)
        Me.ControlBox = False
        Me.Controls.Add(Me.pbZero)
        Me.Controls.Add(Me.pbThree)
        Me.Controls.Add(Me.pbTwo)
        Me.Controls.Add(Me.pbSix)
        Me.Controls.Add(Me.pbFive)
        Me.Controls.Add(Me.pbOne)
        Me.Controls.Add(Me.pbNine)
        Me.Controls.Add(Me.pbFour)
        Me.Controls.Add(Me.pbEight)
        Me.Controls.Add(Me.pbSeven)
        Me.Controls.Add(Me.status)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.pbExit)
        Me.Controls.Add(Me.pbEnter)
        Me.Controls.Add(Me.pbClr)
        Me.Controls.Add(Me.pbDel)
        Me.Controls.Add(Me.pbHelp)
        Me.Controls.Add(Me.tbSignOn)
        Me.Font = New System.Drawing.Font("Tahoma", 14.25!, System.Drawing.FontStyle.Regular)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmUserAuthentication"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region


    ''' <summary>
    ''' To declare all the User Authentication details
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>

    Private Sub pbHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbHelp.Click
        MessageBox.Show(MessageManager.GetInstance.GetMessage("M37"), "Help")
    End Sub
    Private Sub pbNum1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbOne.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "1"
        End If
    End Sub
    Private Sub pbNum2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbTwo.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "2"
        End If
    End Sub
    Private Sub pbNum3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbThree.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "3"
        End If
    End Sub
    Private Sub pbNum4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbFour.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "4"
        End If
    End Sub
    Private Sub pbNum5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbFive.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "5"
        End If
    End Sub
    Private Sub pbNum6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSix.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "6"
        End If
    End Sub
    Private Sub pbNum7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSeven.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "7"
        End If
    End Sub
    Private Sub pbNum8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbEight.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "8"
        End If
    End Sub
    Private Sub pbNum9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNine.Click
        If Len(tbSignOn.Text) < Macros.MAX_PASSWORD Then
            tbSignOn.Text += "9"
        End If
    End Sub
    Private Sub pbNum0_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbZero.Click
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
    End Sub
    Private Sub pbEnter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbEnter.Click
#If RF Then
        pbEnter.Enabled = False
#End If

        'Declare local variable for holding the user entered text

        'Dim strUserEntered As String = Nothing

        ''Store the entered text in a local variable
        'strUserEntered = tbSignOn.Text

        ''Validate the user credentials
        'If UserSessionManager.GetInstance.ValidateUser(strUserEntered) Then
        '    'Authenticate user and apply check condition
        '    If UserSessionManager.GetInstance.HandleUserAuthentication() Then
        '        'Dispose the User Authentication module
        '        UserSessionManager.GetInstance.EndSession()
        '        objAppContainer.objSplashScreen.ChangeLabelText("Please wait while the application loads")
        '    End If

        '    '  UserSessionManager.GetInstance.HandleUserAuthentication()
        '    'Dispose the User Authentication module
        '    '  UserSessionManager.GetInstance.EndSession()
        '    'Me.Close()

        'Else
        '    ' Show pop up message saying invalid credentials
        '    Me.tbSignOn.Text = ""
        '    Me.Refresh()
        'End If
#If NRF Then
          Dim strUserEntered As String = Nothing

        'Store the entered text in a local variable
        strUserEntered = tbSignOn.Text
        'Check for condition if user has entered a text 
        If Not tbSignOn.Text = "" Then
            'Set status bar
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
        End If
        'Validate the user credentials
        If UserSessionManager.GetInstance.ValidateUser(strUserEntered) Then
               'Authenticate user and apply check condition
            If UserSessionManager.GetInstance.HandleUserAuthentication() Then
                'Dispose the User Authentication module
                UserSessionManager.GetInstance.EndSession()
                objAppContainer.objSplashScreen.ChangeLabelText("Please wait while the application loads")
                'Me.Close()
            End If
        Else
            ' Show pop up message saying invalid credentials
            Me.tbSignOn.Text = ""
            Me.Refresh()
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        End If
#ElseIf RF Then
        Dim strUserEntered As String = Nothing

        'Store the entered text in a local variable
        strUserEntered = tbSignOn.Text

        'Validate the user credentials
        If UserSessionManager.GetInstance.ValidateUser(strUserEntered) Then
            pbEnter.Enabled = True
            'Authenticate user and apply check condition
            'UserSessionManager.GetInstance.HandleUserAuthentication()
            'Dispose the User Authentication module
            'UserSessionManager.GetInstance.EndSession()
            Me.Close()

        Else
            pbEnter.Enabled = True
            If objAppContainer.bCommFailure Then
                'DEFECT FIX BTCPR00004357(PPC - Goods In - out of rf range - sign on - failed retries - 
                'should return to main system icons not sign on screen)
                'Dispose the User Authentication module
                UserSessionManager.GetInstance.EndSession()
            Else
                ' Show pop up message saying invalid credentials
                Me.tbSignOn.Text = ""
                Me.Refresh()
            End If

        End If
#End If

    End Sub
    Private Sub pbExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbExit.Click
        Dim iResult As Integer = 0
        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M79"), "Confirmation", MessageBoxButtons.YesNo, _
                                      MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
        If iResult = MsgBoxResult.Yes Then
#If NRF Then
            objAppContainer.bIsAppRegisterRequired = False
#End If
            'Dispose the User Authentication module
            UserSessionManager.GetInstance.EndSession()
        End If

    End Sub
    Public Function SWLeft(ByVal sString As String, ByVal lLth As Long)
        SWLeft = sString.Substring(0, lLth)
    End Function

End Class
