Public Class frmBookInCartonSummary
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblBookIn As System.Windows.Forms.Label
    Friend WithEvents lblCartonsBooked As System.Windows.Forms.Label
    Friend WithEvents lblMsg As System.Windows.Forms.Label
    Friend WithEvents lblCartonCount As System.Windows.Forms.Label
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox

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
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBookInCartonSummary))
        Me.lblBookIn = New System.Windows.Forms.Label
        Me.lblCartonsBooked = New System.Windows.Forms.Label
        Me.lblMsg = New System.Windows.Forms.Label
        Me.lblCartonCount = New System.Windows.Forms.Label
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'lblBookIn
        '
        Me.lblBookIn.Location = New System.Drawing.Point(24, 32)
        Me.lblBookIn.Name = "lblBookIn"
        Me.lblBookIn.Size = New System.Drawing.Size(100, 32)
        Me.lblBookIn.Text = "Book in Carton Summary"
        '
        'lblCartonsBooked
        '
        Me.lblCartonsBooked.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblCartonsBooked.Location = New System.Drawing.Point(24, 88)
        Me.lblCartonsBooked.Name = "lblCartonsBooked"
        Me.lblCartonsBooked.Size = New System.Drawing.Size(120, 16)
        Me.lblCartonsBooked.Text = "Cartons Booked In"
        '
        'lblMsg
        '
        Me.lblMsg.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMsg.Location = New System.Drawing.Point(24, 160)
        Me.lblMsg.Name = "lblMsg"
        Me.lblMsg.Size = New System.Drawing.Size(144, 32)
        Me.lblMsg.Text = "Collect and file carton Book In Report"
        Me.lblMsg.Visible = False
        '
        'lblCartonCount
        '
        Me.lblCartonCount.Location = New System.Drawing.Point(160, 88)
        Me.lblCartonCount.Name = "lblCartonCount"
        Me.lblCartonCount.Size = New System.Drawing.Size(24, 20)
        Me.lblCartonCount.Text = "1"
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(160, 208)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmBookInCartonSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.lblCartonCount)
        Me.Controls.Add(Me.lblMsg)
        Me.Controls.Add(Me.lblCartonsBooked)
        Me.Controls.Add(Me.lblBookIn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmBookInCartonSummary"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub frmBookInCartonSummary_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StopRead()
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONSUMMARY
    End Sub
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
#If NRF Then
        If BCSessionMgr.GetInstance().m_strBookIncartonShowMsg = "Y" Then
                'If summary sreen is exited after Finishing the session
            If objAppContainer.objPrevMod <> AppContainer.ACTIVEMODULE.AUDITCARTON Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M85"), "Alert", MessageBoxButtons.OK, _
                                          MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If
                
                BCSessionMgr.GetInstance().m_strBookIncartonShowMsg = Nothing
                BCSessionMgr.GetInstance().EndSession(AppContainer.IsAbort.No)
        Else
            BCSessionMgr.GetInstance().EndSession(AppContainer.IsAbort.Yes)
        End If
        If objAppContainer.objPrevMod = AppContainer.ACTIVEMODULE.AUDITCARTON Then
            ACSessionManager.GetInstance().DisplayACScreen(ACSCREENS.Audit)
        End If
#ElseIf RF Then
        objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
            If BCSessionMgr.GetInstance().m_strBookIncartonShowMsg = "Y" Then
                'If summary sreen is exited after Finishing the session
                BCSessionMgr.GetInstance().EndSession(AppContainer.IsAbort.No)
            Else
                BCSessionMgr.GetInstance().EndSession(AppContainer.IsAbort.Yes)
        End If
        If objAppContainer.objPrevMod = AppContainer.ACTIVEMODULE.AUDITCARTON Then
            ACSessionManager.GetInstance().DisplayACScreen(ACSCREENS.Audit)
        End If
#End If
            'If Not objAppContainer.strDeviceType = Macros.RF Then
            '    If BCSessionMgr.GetInstance().m_strBookIncartonShowMsg = "Y" Then
            '        'If summary sreen is exited after Finishing the session

            '        MessageBox.Show(MessageManager.GetInstance().GetMessage("M85"), "Alert", MessageBoxButtons.OK, _
            '                                  MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            '        BCSessionMgr.GetInstance().m_strBookIncartonShowMsg = Nothing
            '        BCSessionMgr.GetInstance().EndSession(AppContainer.IsAbort.No)
            '    Else
            '        BCSessionMgr.GetInstance().EndSession(AppContainer.IsAbort.Yes)
            '    End If
            'Else
            '    If BCSessionMgr.GetInstance().m_strBookIncartonShowMsg = "Y" Then
            '        'If summary sreen is exited after Finishing the session
            '        BCSessionMgr.GetInstance().EndSession(AppContainer.IsAbort.No)
            '    Else
            '        BCSessionMgr.GetInstance().EndSession(AppContainer.IsAbort.Yes)
            '    End If

            'End If
    End Sub
    Public Sub FreezeControls()
        Me.Btn_Quit_small1.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.Btn_Quit_small1.Enabled = True
    End Sub
End Class


