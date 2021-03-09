Imports System.IO

'''****************************************************************************
''' <FileName> MessageManager.vb </FileName>
''' <summary>
''' Class to handle the reading of the Order and Collect CSV Messages file.
''' </summary> 
''' <Version>1.0</Version> 
''' <Author>Andrew Paton</Author> 
''' <DateModified>11-05-2016</DateModified> 
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC55RF</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK </CopyRight>
'''****************************************************************************
'''* Modification Log 
'''**************************************************************************** 
'''  1.0    Andrew Paton                             11/05/2016        
'''         Inital Version.
''' 
'''**************************************************************************** 

Public Class MessageManager

    Private Shared messageList As New List(Of String)
    Private Shared m_Instance As MessageManager

    ''' <summary>
    ''' Read and load message config file when class is initialised
    ''' </summary>
    Private Sub New()
        Try
            Using readConfigFile As New StreamReader(ConfigKey.MESSAGE_FILE_PATH)
                While Not readConfigFile.EndOfStream
                    messageList.Add(readConfigFile.ReadLine)
                End While
            End Using
        Catch
            MessageBox.Show("File not found: " + ConfigKey.MESSAGE_FILE_PATH, "Exception")
        End Try
    End Sub

    ''' <summary>
    ''' The shared function GetInstance will implement a check for the instantiation
    ''' of class MessageManager to make sure that the class has only one instance
    ''' </summary>
    ''' <returns>m_Instance</returns>
    Public Shared Function GetInstance() As MessageManager
        'Create only a single instance of MessageManager class
        If m_Instance Is Nothing Then
            m_Instance = New MessageManager
        End If
        Return m_Instance
    End Function

    ''' <summary>
    ''' Reads message list and returns it to the calling function
    ''' </summary>
    ''' <param name="messageId">Message ID eg. M1</param>
    ''' <returns>String</returns>
    Public Function GetMessage(ByVal messageId As String) As String
        Dim messageString As String = ""
        For Each record In messageList
            If record.StartsWith(messageId + ",") Then
                Dim sParam As String() = record.Split(",")
                messageString = sParam(1)
                Exit For
            End If
        Next
        Return messageString
    End Function

    Public Sub CloseSession()
        m_Instance = Nothing
    End Sub

End Class
