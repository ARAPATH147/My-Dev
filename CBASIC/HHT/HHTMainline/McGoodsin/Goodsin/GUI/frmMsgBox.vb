'''*******************************************************************************
''' Modification Log 
'''******************************************************************************* 
''' No:      Author            Date            Description
''' 1.1 Christopher Kitto  14/05/2015   Amended method btnTwo_Click
'''           (CK)                      
''' 
'''     Kiran Krishnan     14/05/2015   Amended method btnTwo_Click
'''           (KK)
'''********************************************************************************
Imports System.Runtime.InteropServices
Imports System.Windows.Forms.Form
Imports System.Drawing
Imports System.Windows.Forms
Imports System.EventArgs
Public Class MsgBx
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblTwo As System.Windows.Forms.Label
    Friend WithEvents btnOne As System.Windows.Forms.Button
    Friend WithEvents btnTwo As System.Windows.Forms.Button

#Region " Windows Form Designer generated code "
#If RF Then
 Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        'Me.lblCaption.BackColor = System.Drawing.Color.LightBlue
        'Add any initialization after the InitializeComponent() call

    End Sub
#ElseIf NRF Then
    Private Sub New(ByVal sText As String, ByVal sCaption As String, ByVal BtnType As BUTTON_TYPE)
        ' 
        ' Required for Windows Form Designer support 
        ' 

        InitializeComponent()

        lbtnType = BtnType
        ButtonType = BtnType
        ' lblTwo.Text = sCaption.ToUpper()
        lblTwo.Text = sText
        lblCaption.Text = sCaption

        Select Case BtnType
            ' case BUTTON_TYPE.OKCANCEL: 
            ' bFocus = true; 
            ' btnOK.Focus(); 
            ' break; 
            Case BUTTON_TYPE.YESNO
                btnOne.Text = "YES"
                btnThree.Text = "NO"
                btnOne.Visible = True
                btnTwo.Visible = False
                btnThree.Visible = True
                DlgResultOK = DialogResult.Yes
                DlgResultCancel = DialogResult.No
                ' bFocus = true; 
                ' btnOK.Focus(); 
                Exit Select
            Case BUTTON_TYPE.OKCANCEL
                btnOne.Text = "OK"
                btnThree.Text = "Cancel"
                btnOne.Visible = True
                btnTwo.Visible = False
                btnThree.Visible = True
                Exit Select
            Case BUTTON_TYPE.CONTINE
                btnTwo.Text = "Continue"
                btnOne.Visible = False
                btnTwo.Visible = True
                btnThree.Visible = False
                DlgResultOK = DialogResult.OK
                DlgResultCancel = DialogResult.Cancel
            Case BUTTON_TYPE.NOTONFILE
                btnOne.Text = "Accept"
                btnTwo.Text = "Cancel Last"
                btnThree.Text = "Reject"
                Exit Select

        End Select

    End Sub
#End If




    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents btnThree As System.Windows.Forms.Button
    Friend WithEvents ListView1 As System.Windows.Forms.ListView
    Friend WithEvents lblCaption As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.lblTwo = New System.Windows.Forms.Label
        Me.btnOne = New System.Windows.Forms.Button
        Me.btnTwo = New System.Windows.Forms.Button
        Me.btnThree = New System.Windows.Forms.Button
        Me.ListView1 = New System.Windows.Forms.ListView
        Me.lblCaption = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'lblTwo
        '
        Me.lblTwo.Location = New System.Drawing.Point(8, 24)
        Me.lblTwo.Name = "lblTwo"
        Me.lblTwo.Size = New System.Drawing.Size(208, 107)
        '
        'btnOne
        '
        Me.btnOne.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.btnOne.Location = New System.Drawing.Point(4, 134)
        Me.btnOne.Name = "btnOne"
        Me.btnOne.Size = New System.Drawing.Size(56, 22)
        Me.btnOne.TabIndex = 3
        Me.btnOne.Text = "Accept"
        '
        'btnTwo
        '
        Me.btnTwo.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold)
        Me.btnTwo.Location = New System.Drawing.Point(66, 134)
        Me.btnTwo.Name = "btnTwo"
        Me.btnTwo.Size = New System.Drawing.Size(88, 22)
        Me.btnTwo.TabIndex = 2
        Me.btnTwo.Text = "Empty Crate"
        '
        'btnThree
        '
        Me.btnThree.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold)
        Me.btnThree.Location = New System.Drawing.Point(160, 134)
        Me.btnThree.Name = "btnThree"
        Me.btnThree.Size = New System.Drawing.Size(56, 22)
        Me.btnThree.TabIndex = 1
        Me.btnThree.Text = "Reject"
        '
        'ListView1
        '
        Me.ListView1.Location = New System.Drawing.Point(0, 0)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(225, 159)
        Me.ListView1.TabIndex = 5
        '
        'lblCaption
        '
        Me.lblCaption.Enabled = False
        Me.lblCaption.Location = New System.Drawing.Point(0, 0)
        Me.lblCaption.Name = "lblCaption"
        Me.lblCaption.Size = New System.Drawing.Size(225, 20)
        Me.lblCaption.TabIndex = 0
        Me.lblCaption.Text = "UOD NOT ON FILE"
        '
        'MsgBx
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(225, 159)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblCaption)
        Me.Controls.Add(Me.btnThree)
        Me.Controls.Add(Me.btnTwo)
        Me.Controls.Add(Me.btnOne)
        Me.Controls.Add(Me.lblTwo)
        Me.Controls.Add(Me.ListView1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Location = New System.Drawing.Point(7, 65)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "MsgBx"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Public Enum BUTTON_TYPE As Integer
        OKCANCEL = 0
        YESNO = 1
        NOTONFILE = 2
        CONTINE = 3
    End Enum
    Public Enum BUTTON_VALUE As Integer
        EMPTYCRATE = 0
        ACCEPT = 1
        REJECT = 2
        [CONTINUE] = 3
        NONE
    End Enum


    Private alblLines As System.Windows.Forms.Label()
    Private lbtnType As BUTTON_TYPE
    Private Shared DlgResult As DialogResult
    Private Shared buttonvalue As BUTTON_VALUE = BUTTON_VALUE.NONE
    Private DlgResultOK As DialogResult = DialogResult.OK
    Private DlgResultCancel As DialogResult = DialogResult.Cancel
    Private Const iNoOfLines As Integer = 2
    Public ButtonType As BUTTON_TYPE
    Private iMessage As Integer
    Private Sub btnOne_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOne.Click
        If ButtonType = BUTTON_TYPE.NOTONFILE Then

            buttonvalue = BUTTON_VALUE.ACCEPT
#If RF Then
            Me.Hide()
            BDSessionMgr.GetInstance().NotOnFile(buttonvalue)

#End If

        End If
#If NRF Then
        Me.Close()
#End If

    End Sub

    Private Sub btnTwo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTwo.Click
#If RF Then
  If ButtonType = BUTTON_TYPE.NOTONFILE Then
            buttonvalue = BUTTON_VALUE.EMPTYCRATE
            Me.Hide()
            BDSessionMgr.GetInstance().NotOnFile(buttonvalue)
        ElseIf ButtonType = BUTTON_TYPE.CONTINE Then
            buttonvalue = BUTTON_VALUE.[CONTINUE]
            Me.Hide()
            If iMessage = 1 Then
                BDSessionMgr.GetInstance().DisplayBDUODScreen(buttonvalue)
            ElseIf iMessage = 2 Then
                BDSessionMgr.GetInstance().AcceptMisDirect(buttonvalue)
            ElseIf iMessage = 3 Then
                BDSessionMgr.GetInstance().UODNotOnEPOS()
            ElseIf iMessage = 4 Then
                BDSessionMgr.GetInstance().UODNotinFileMessage(buttonvalue)
            ElseIf iMessage = 5 Then
                BDSessionMgr.GetInstance().NoBadge()
            ElseIf iMessage = 6 Then
                BDSessionMgr.GetInstance().NoBadgeSessionConfirm()
            ElseIf iMessage = 7 Then
                BDSessionMgr.GetInstance().MisdirectRejected()
            ElseIf iMessage = 8 Then
                ' V1.1 - KK
                ' If iMessage is 8 call function DallasUODNotonFileMessage
                ' from BDSessionMgr
                BDSessionMgr.GetInstance().DallasUODNotonFileMessage(buttonvalue)
            ElseIf iMessage = 9 Then
                ' V1.1 - CK
                ' If iMessage is 9 call function DisplayAuditScreen from
                ' AUODSessionManager
                AUODSessionManager.GetInstance().DisplayAuditScreen()
            ElseIf iMessage = 10 Then
                ' V1.1 - KK
                ' If iMessage is 10 call function DisplayVDUODScreen from
                ' VUODSessionManager
                VUODSessionMgr.GetInstance().DisplayVDUODScreen()
            End If
        End If
#ElseIf NRF Then
        If ButtonType = BUTTON_TYPE.NOTONFILE Then
            buttonvalue = BUTTON_VALUE.EMPTYCRATE
        ElseIf ButtonType = BUTTON_TYPE.CONTINE Then
            buttonvalue = BUTTON_VALUE.CONTINUE
        End If
        Me.Close()
#End If


            'Me.Close()
    End Sub

    Private Sub btnThree_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnThree.Click
#If RF Then
  buttonvalue = BUTTON_VALUE.REJECT
        Me.Hide()

        '  Me.Close()
        BDSessionMgr.GetInstance().NotOnFile(buttonvalue)
#ElseIf NRF Then
        buttonvalue = BUTTON_VALUE.REJECT
        Me.Close()
#End If

    End Sub

    Public Shared Function DisplayMessage(ByVal sText As String, ByVal sCaption As String, ByVal BtnType As BUTTON_TYPE) As BUTTON_VALUE
#If NRF Then
        Dim objMsgBx As New MsgBx(sText, sCaption, BtnType)
        objMsgBx.ShowDialog()
        objMsgBx.Dispose()
#End If

        Return buttonvalue

    End Function
    Public Sub MsgBoxInitialize(ByVal sText As String, ByVal sCaption As String, ByVal BtnType As BUTTON_TYPE, _
                                Optional ByVal MessageType As Integer = 0)


        lbtnType = BtnType
        ButtonType = BtnType
        'lblCaption.BackColor = System.Drawing.SystemColors.ActiveCaption
        lblCaption.Text = sCaption.ToUpper()
        lblTwo.Text = sText
        iMessage = MessageType
        Select Case BtnType

            Case BUTTON_TYPE.YESNO
                btnOne.Text = "YES"
                btnThree.Text = "NO"
                btnOne.Visible = True
                btnTwo.Visible = False
                btnThree.Visible = True
                DlgResultOK = DialogResult.Yes
                DlgResultCancel = DialogResult.No
                ' bFocus = true; 
                ' btnOK.Focus(); 
                Exit Select
            Case BUTTON_TYPE.OKCANCEL
                btnOne.Text = "OK"
                btnThree.Text = "Cancel"
                btnOne.Visible = True
                btnTwo.Visible = False
                btnThree.Visible = True
                Exit Select
            Case BUTTON_TYPE.CONTINE
                btnTwo.Text = "Continue"
                btnOne.Visible = False
                btnTwo.Visible = True
                btnThree.Visible = False
                DlgResultOK = DialogResult.OK
                DlgResultCancel = DialogResult.Cancel
            Case BUTTON_TYPE.NOTONFILE
                btnOne.Text = "Accept"
                btnTwo.Text = "Cancel Last"
                btnThree.Text = "Reject"
                btnOne.Visible = True
                btnTwo.Visible = True
                btnThree.Visible = True
                Exit Select

        End Select
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblCaption.Click

    End Sub
End Class
