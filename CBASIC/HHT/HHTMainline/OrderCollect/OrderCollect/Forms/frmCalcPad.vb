

Public Class frmCalcPad

    Private m_iMaxLength As Integer  ' Maximum length of EAN
    Private m_CallType As AppMain.CALCPADUSE

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
    End Sub

    ''' <summary>
    ''' To set the timer at load event close calcpad before autologoff.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub frmCalcPad_Load() Handles MyBase.Load
        'Add any initialization after the InitializeComponent() call.
        Dim iInterval As Integer = CInt(ConfigFileManager.GetInstance().GetParam(ConfigKey.AUTO_LOGOFF_TIMEOUT))
        'Convert minutes to milliseconds.
        tmrChecker.Interval = (iInterval * 60 * 1000) - 30000
        tmrChecker.Enabled = True
    End Sub

    Private Sub pbDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbDelete.Click
        tbValue.Focus()
        If Len(tbValue.Text) <> 0 Then
            tbValue.Text = Strings.Left(tbValue.Text, (Len(tbValue.Text) - 1))
        End If
        tbValue.SelectionStart = Len(tbValue.Text)
    End Sub

    Private Sub pbQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbQuit.Click

        If m_CallType = AppMain.CALCPADUSE.PARCELORDERNUMBER Then
            'Prevent user leaving screen without entering the Parcel Order No.
            MessageBox.Show(MessageManager.GetInstance.GetMessage("M23"), "WARNING", _
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        ElseIf m_CallType = AppMain.CALCPADUSE.LOCATIONBARCODE Then
            'Prevent user leaving screen without entering a location
            MessageBox.Show(MessageManager.GetInstance.GetMessage("M25"), "WARNING", _
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        Else
            tbValue.Text = ""
            Me.Visible = False
        End If

    End Sub

    Public Sub pbOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbOk.Click
        'Checks the case to do applicable processing

        Select Case m_CallType
            Case AppMain.CALCPADUSE.ASNBARCODE
                Me.Visible = False
                ParcelSession.GetInstance.CartonScanned(tbValue.Text)
            Case AppMain.CALCPADUSE.LOCATIONBARCODE
                Me.Visible = False
                ParcelSession.GetInstance.LocationScanned(tbValue.Text.PadLeft(18, "0"))
            Case AppMain.CALCPADUSE.PARCELORDERNUMBER
                If tbValue.Text.Length >= 8 Then
                    Me.Visible = False
                    ParcelSession.GetInstance.ParcelNumberEntered(tbValue.Text.PadLeft(10, "0"))
                Else
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M20"), "WARNING", _
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                                        MessageBoxDefaultButton.Button1)
                    'Me.Visible = True
                End If
        End Select
        tbValue.Text = ""

    End Sub

    ''' <summary>
    ''' Handle the tick event to close calcpad form.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub tmrChecker_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrChecker.Tick
        tmrChecker.Enabled = False
        'Close the form.
        Me.Close()
    End Sub

    Public Sub setMaxLength(ByVal calltype As AppMain.CALCPADUSE)
        Select Case callType
            Case AppMain.CALCPADUSE.ASNBARCODE
                'The Maximum Value That a Barcode can Have is 18
                m_iMaxLength = 18
                lblInfo.Text = "Enter ASN barcode"
            Case AppMain.CALCPADUSE.LOCATIONBARCODE
                m_iMaxLength = 3
                lblInfo.Text = "Enter location number"
            Case AppMain.CALCPADUSE.PARCELORDERNUMBER
                m_iMaxLength = 10
                lblInfo.Text = "Enter order number from the parcel"
        End Select
        'Textbox needs to be empty
        m_CallType = calltype
        tbValue.Text = ""
        tbValue.MaxLength = m_iMaxLength
        tbValue.SelectionStart = tbValue.TextLength
    End Sub

    Private Sub pbZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbZero.Click
        inputProcess("0")
    End Sub

    Private Sub pbOne_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbOne.Click
        inputProcess("1")
    End Sub

    Private Sub pbTwo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbTwo.Click
        inputProcess("2")
    End Sub

    Private Sub pbThree_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbThree.Click
        inputProcess("3")
    End Sub

    Private Sub pbFour_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbFour.Click
        inputProcess("4")
    End Sub

    Private Sub pbFive_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbFive.Click
        inputProcess("5")
    End Sub

    Private Sub pbSix_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSix.Click
        inputProcess("6")
    End Sub

    Private Sub pbSeven_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSeven.Click
        inputProcess("7")
    End Sub

    Private Sub pbEight_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbEight.Click
        inputProcess("8")
    End Sub

    Private Sub pbNine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbNine.Click
        inputProcess("9")
    End Sub

    Private Sub inputProcess(ByVal value As String)
        'Checks Whether the length is not equal to maximum length
        If Len(tbValue.Text) < m_iMaxLength Then
            tbValue.Focus()
            tbValue.Text += value
            tbValue.SelectionStart = Len(tbValue.Text)
        Else
            'MessageBox.Show("The Maximum length for " + m_CallType.ToString + " Entry is " + (m_iMaxLength.ToString) + " Characters", m_CallType.ToString + " Entry:", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            MessageBox.Show("The Maximum length allowed is " + _
                    (m_iMaxLength.ToString) + " digits", " Information", _
                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End If
        tbValue.Focus()
    End Sub


End Class