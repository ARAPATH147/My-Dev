Imports System.Reflection
Public Class NumericTextbox
    Private SpaceOK As Boolean = False
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

    End Sub



    Private Sub tbNumeric_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtNumeric.KeyPress
        Dim numberFormatInfo As Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat
        Dim decimalSeparator As String = numberFormatInfo.NumberDecimalSeparator
        Dim groupSeparator As String = numberFormatInfo.NumberGroupSeparator
        Dim negativeSign As String = numberFormatInfo.NegativeSign

        Dim keyInput As String = e.KeyChar.ToString()

        If [Char].IsDigit(e.KeyChar) Then
            ' Digits are OK
        ElseIf keyInput.Equals(decimalSeparator) OrElse keyInput.Equals(groupSeparator) OrElse keyInput.Equals(negativeSign) Then
            ' Decimal separator is OK
        ElseIf e.KeyChar = vbBack Then
            ' Backspace key is OK
            '    else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
            '    {
            '     // Let the edit control handle control and alt key combinations
            '    }
        ElseIf Me.SpaceOK AndAlso e.KeyChar = " "c Then

        Else
            ' Consume this invalid key and beep.
            e.Handled = True
        End If

    End Sub
    Public ReadOnly Property IntValue() As Integer
        Get
            Return Int32.Parse(Me.Text)
        End Get
    End Property


    Public ReadOnly Property DecimalValue() As Decimal
        Get
            Return [Decimal].Parse(Me.Text)
        End Get
    End Property


    Public Property AllowSpace() As Boolean

        Get
            Return Me.SpaceOK
        End Get
        Set(ByVal value As Boolean)
            Me.SpaceOK = value
        End Set
    End Property

    Private Sub NumericTextbox_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
        Me.txtNumeric.Focus()
    End Sub

    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        Dim objSftKeyPad As frmCalcPad = Nothing
        If objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.GDSTFR Then
            objSftKeyPad = New frmCalcPad(txtNumeric, CalcPadSessionMgr.EntryTypeEnum.Destination_Store_Id)
        ElseIf objAppContainer.bCreateRecall = True Then
            objSftKeyPad = New frmCalcPad(txtNumeric, CalcPadSessionMgr.EntryTypeEnum.Recall_Id)
        Else
            objSftKeyPad = New frmCalcPad(txtNumeric, CalcPadSessionMgr.EntryTypeEnum.Authorization_Id)
        End If

        objSftKeyPad.ShowDialog()
        objSftKeyPad.Close()
        objSftKeyPad.Dispose()
        If txtNumeric.Text.Length > 0 Then
            InvokeKeyEventHandler(txtNumeric, "KeyDown", Keys.Enter)
            txtNumeric.Text = ""
        End If

    End Sub
    Private Sub InvokeKeyEventHandler(ByVal c As Control, ByVal eventName As String, ByVal k As Keys)
        Dim fi As FieldInfo = Nothing
        Dim typeControl As Type = c.[GetType]()
        ' find correct FieldInfo for passed eventName
        While typeControl IsNot GetType(Object)
            fi = typeControl.GetField(eventName, BindingFlags.NonPublic Or BindingFlags.Instance)
            If fi IsNot Nothing Then
                Exit While
            End If
            typeControl = typeControl.BaseType
        End While

        If fi Is Nothing Then
            Throw New ArgumentException("Impossible to find: " + eventName)
        End If

        ' invokes each handler in the invocation list
        For Each keh As KeyEventHandler In (TryCast(fi.GetValue(c), MulticastDelegate)).GetInvocationList()
            keh.Invoke(c, New KeyEventArgs(k))
        Next
    End Sub

    Private Sub txtNumeric_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtNumeric.KeyDown
        If txtNumeric.Text.Length > 0 Then
            If e.KeyValue = Keys.Enter Then
                MyBase.OnKeyDown(e)
            End If
        End If
    End Sub
End Class
