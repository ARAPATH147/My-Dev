#If NRF Then
Public Class DBPopulate
    Public Shared m_objLock As System.Object
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        If m_objLock Is Nothing Then
            m_objLock = New System.Object()
        End If
    End Sub
    ''' <summary>
    ''' Function to parse the files and populate the data in database.
    ''' </summary>
    ''' <param name="obFileDet"></param>
    ''' <param name="bPurgeExistingData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Populate(ByVal obFileDet As FileDetails, ByVal bPurgeExistingData As Boolean) As Boolean
        Dim objReferenceFileParser As ReferenceFileParser = Nothing
        Dim bReturn As Boolean = False
        Try
            SyncLock m_objLock
                objReferenceFileParser = New ReferenceFileParser()
                If bPurgeExistingData Then
                    'Delete the Corresponding Table Data
                    Select Case obFileDet.strFileName
                        Case ReferenceFileMacro.USER
                            objReferenceFileParser.PurgeReferenceData("UserList")
                        Case ReferenceFileMacro.BARCODE
                            objReferenceFileParser.PurgeReferenceData("BarCodeView")
                        Case ReferenceFileMacro.BOOTCODE
                            objReferenceFileParser.PurgeReferenceData("BootsCodeView")
                        Case ReferenceFileMacro.DEAL
                            objReferenceFileParser.PurgeReferenceData("DealList")
                        Case ReferenceFileMacro.LIVEPOG
                            objReferenceFileParser.PurgeReferenceData("LivePOG")
                        Case ReferenceFileMacro.MODULE_LIST
                            objReferenceFileParser.PurgeReferenceData("ModuleListItems")
                            objReferenceFileParser.PurgeReferenceData("ModuleList")
                        Case ReferenceFileMacro.CATEGORY
                            objReferenceFileParser.PurgeReferenceData("POGCategory")
                        Case ReferenceFileMacro.PGROUP
                            objReferenceFileParser.PurgeReferenceData("ProductGroup")
                        Case ReferenceFileMacro.RECALL
                            objReferenceFileParser.PurgeReferenceData("RecallListItems")
                            objReferenceFileParser.PurgeReferenceData("RecallList")
                        Case ReferenceFileMacro.SUPPLIER
                            objReferenceFileParser.PurgeReferenceData("SupplierBC")
                            objReferenceFileParser.PurgeReferenceData("SuppliersList")
                        Case ReferenceFileMacro.SHELFDES
                            objReferenceFileParser.PurgeReferenceData("ShelfDesc")
                    End Select
                End If
                'Start parsing the file and update teh status after updating db.
                ReferenceDataMgr.GetInstance.objDownloadReferenceData.SetCurrentStatus _
                ("The data from the file " + obFileDet.strFileName + " is updated to the database.")
                bReturn = objReferenceFileParser.ParseFile(obFileDet.strFileName)
                If bReturn Then
                    BatchConfigParser.GetInstance().UpdateDBpopulationStatus(obFileDet.strFileName, "Y")
                    If obFileDet.strFileName.Equals("BARCODE.CSV") Then
                        'Execute the sample query if the parsed file is BARCODE.CSV
                        objReferenceFileParser.ExecuteSampleQuery("Barcode")
                    ElseIf obFileDet.strFileName.Equals("MODULE.CSV") Then
                        'Execute the sample query if the parsed file is MODULE.CSV
                        objReferenceFileParser.ExecuteSampleQuery("Module")
                    End If
                End If
            End SyncLock
        Catch ex As Exception
            BatchConfigParser.GetInstance().UpdateDBPopulationIndex(obFileDet.strFileName, _
                                                                    0, ex.Message.ToString)
        Finally
            objReferenceFileParser.Terminate()
        End Try
        Return bReturn
    End Function
    ''' <summary>
    ''' Overridden function for populating the database.
    ''' </summary>
    ''' <param name="obFileDet"></param>
    ''' <param name="dIndex"></param>
    ''' <remarks></remarks>
    Public Sub Populate(ByVal obFileDet As FileDetails, ByVal dIndex As Double)
        Dim objReferenceFileParser As ReferenceFileParser = Nothing
        Dim bReturn As Boolean = False
        Try
            SyncLock m_objLock
                'Start parsing the file and update teh status after updating db.
                ReferenceDataMgr.GetInstance.objDownloadReferenceData.SetCurrentStatus _
                ("The data from the file " + obFileDet.strFileName + " is updated to the database")
                bReturn = objReferenceFileParser.ParseFile(obFileDet.strFileName, dIndex)
                If bReturn Then
                    BatchConfigParser.GetInstance().UpdateDBpopulationStatus(obFileDet.strFileName, "Y")
                    If obFileDet.strFileName.Equals("BARCODE.CSV") Then
                        'Execute the sample query if the parsed file is BARCODE.CSV
                        objReferenceFileParser.ExecuteSampleQuery("Barcode")
                    ElseIf obFileDet.strFileName.Equals("MODULE.CSV") Then
                        'Execute the sample query if the parsed file is MODULE.CSV
                        objReferenceFileParser.ExecuteSampleQuery("Module")
                    End If
                End If
            End SyncLock
        Catch ex As Exception
            BatchConfigParser.GetInstance().UpdateDBPopulationIndex(obFileDet.strFileName, _
                                                                    0, ex.Message.ToString)
        Finally
            objReferenceFileParser.Terminate()
        End Try
    End Sub
End Class
#End If