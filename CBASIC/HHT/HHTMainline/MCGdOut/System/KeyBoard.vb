Imports System
Imports System.Runtime.InteropServices
Imports Symbol.Keyboard
Imports Symbol.StandardForms
Imports System.Security
Imports System.Security.Permissions
Imports Microsoft.Win32

Public Class KeyBoard

    Private Shared objKeyBoard As KeyBoard
    Private MyKeypad As KeyPad
    'the class to be changed in case of QWERTY keyboard
    Private Sub New()
        MyKeypad = New KeyPad()
    End Sub

    Public Shared Function GetInstance() As KeyBoard
        If objKeyBoard Is Nothing Then
            objKeyBoard = New KeyBoard()
            Return objKeyBoard
        Else
            Return objKeyBoard
        End If
    End Function
    Public Sub KeyBoard_AlphabeticMode()
        'Symbol.Keyboard.KeyPad Symbol.Keyboard.KeyPad MyKeyPad
        MyKeypad.AlphaMode = True
    End Sub

    Public Sub KeyBoard_NumericMode()
        'Symbol.Keyboard.KeyPad Symbol.Keyboard.KeyPad MyKeyPad
        MyKeypad.AlphaMode = False
    End Sub
    Public Function KeyBoard_Version() As String
        'Symbol.Keyboard.KeyPad Symbol.Keyboard.KeyPad MyKeyPad
        Return MyKeypad.Version.AssemblyVersion
    End Function

    <DllImport("coredll.dll")> _
    Public Shared Function EnableHardwareKeyboard(ByRef bEnable As Boolean) As Boolean
    End Function
    Public Function KeyBoard_Disable() As Boolean
        Try
            Return EnableHardwareKeyboard(False)
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function KeyBoard_Enable() As Boolean
        Try
            Return EnableHardwareKeyboard(True)
        Catch ex As Exception
            Return False
        End Try
    End Function
End Class
