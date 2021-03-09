Public Class DBPopulate
    Public Shared m_objLock As System.Object

    Public Sub New()
        If m_objLock Is Nothing Then
            m_objLock = New System.Object()
        End If

    End Sub
    Public Function Populate(ByVal strFileName As String) As Boolean
        Dim objReferenceFileParser As ReferenceFileParser = Nothing
        Dim bReturn As Boolean = False
        Try
            SyncLock m_objLock
                objReferenceFileParser = New ReferenceFileParser()
                AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus _
                ("The data from the file " + strFileName + " is updated to the database.")
                bReturn = objReferenceFileParser.ParseFile(strFileName)
                If bReturn Then
                    BatchConfigParser.GetInstance().UpdateDBpopulationStatus(strFileName, "Y")
                    If strFileName.Equals(AppContainer.GetInstance.m_objConfigParams.BARCODE.Trim()) Then
                        'Execute the sample query if the parsed file is BARCODE.CSV
                        objReferenceFileParser.ExecuteSampleQuery("Barcode")
                    ElseIf strFileName.Equals(AppContainer.GetInstance.m_objConfigParams.POGMODULE.Trim()) Then
                        'Execute the sample query if the parsed file is MODULE.CSV
                        objReferenceFileParser.ExecuteSampleQuery("Module")
                    End If
                End If
            End SyncLock
        Catch ex As Exception
            BatchConfigParser.GetInstance().UpdateDBPopulationIndex(strFileName, CType(ex.ToString, Integer), ex.Message)
        Finally
            objReferenceFileParser.Terminate()
        End Try
        Return bReturn
    End Function
End Class
