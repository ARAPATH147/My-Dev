Imports System
Imports System.Data
Imports System.IO
'''****************************************************************************
''' <FileName>FileIO.vb</FileName>
''' <summary>
''' Used for writing export data records to export data file.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''****************************************************************************
Public Class FileIO
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

    End Sub
    ''' <summary>
    ''' To write data into a file.
    ''' </summary>
    ''' <param name="sFileName">Name of the file to which the data is written.</param>
    ''' <param name="sRecord">Record to be written into the file.</param>
    ''' <param name="bAppend">Append the dat or not</param>
    ''' <returns>Bool
    ''' True - If successfully written the record.
    ''' False - Any error occured while writing record to the file.
    ''' </returns>
    ''' <remarks></remarks>
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
    ''' <summary>
    ''' Create directory with the name as specified in the argument.
    ''' </summary>
    ''' <param name="strDirectoryPath">Name and path of the directory to be created.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateDirectory(ByVal strDirectoryPath) As Boolean
        Try
            If Not (Directory.Exists(strDirectoryPath)) Then
                Directory.CreateDirectory(strDirectoryPath)
            End If
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function
    ''Function to delete all previous log files on application initialisation
    Public Shared Function LogFileDelete()
        Try
            Dim strFileName As String
            For Each strFileName In _
                   Directory.GetFiles(ConfigDataMgr.GetInstance().GetParam(ConfigKey.LOG_FILE_PATH))
                File.Delete(strFileName)
            Next
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function
End Class