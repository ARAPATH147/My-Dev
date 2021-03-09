Imports System
Imports System.Data
Imports System.IO
'Namespace Boots.Utilities


Public Class FileIO

    Public Sub New()

    End Sub

    Public Shared Function WriteDataIntoFile(ByVal sFileName As String, ByVal sRecord As String, ByVal bAppend As Boolean) As Boolean
        Dim sFilePath As String = sFileName
        Dim fsFileStream As FileStream
        Dim swWriter As StreamWriter

        SyncLock GetType(FileIO)
            If bAppend Then
                Try
                    fsFileStream = New FileStream(sFilePath, FileMode.Append, FileAccess.Write)
                Catch ex As Exception
                    fsFileStream = Nothing
                    Return False
                End Try
                swWriter = New StreamWriter(fsFileStream)
                swWriter.Write(sRecord)
            Else
                Try

                    fsFileStream = New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                Catch
                    fsFileStream = Nothing
                    Return False
                End Try

                swWriter = New StreamWriter(fsFileStream)
                swWriter.Write(sRecord)
            End If

            swWriter.Close()
            fsFileStream.Close()
        End SyncLock
        Return True
    End Function


End Class
'End Namespace
