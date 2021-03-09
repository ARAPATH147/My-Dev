'******************************************************************************
' <FileName>AppContainer.vb</FileName>
' <summary>
' The Main application container class which will 
' intialise all the applciation parameters.
' </summary>
' <Version>1.0</Version>
' <Author>Infosys Technologies Ltd.</Author>
' <DateModified>21-Nov-2008</DateModified>
' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'******************************************************************************
Imports System
Imports System.IO
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports System.Collections.Generic
''' <summary>
''' Class BatchConfigParser - A singleton class to perform the data handling operations
''' of the details of reference files downloaded
''' The SyncLock is used to restric access to block of code
''' </summary>
''' <remarks></remarks>
Public Class BatchConfigParser
    ''' <summary>
    ''' To handle system lock during file read and write.
    ''' </summary>
    ''' <remarks></remarks>
    Private m_objLock As System.Object
    ''' <summary>
    ''' To implement singleton class
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared m_objBatchConfigParser As BatchConfigParser
    ''' <summary>
    ''' To hold file path and file name for pulling the files form controller.
    ''' </summary>
    ''' <remarks></remarks>
    Private m_strFilePath As String
    Private m_strFileName As String = ""
    ''' <summary>
    ''' Object to handle XML read and write using serializer
    ''' </summary>
    ''' <remarks></remarks>
    Private m_objSerializer As XmlSerializer
    ''' <summary>
    ''' Object to store processing details of reference files.
    ''' </summary>
    ''' <remarks></remarks>
    Private m_objProcessingDeatils As ProcessingDetails
    ''' <summary>
    ''' Construction 
    ''' Does the xml creation if the xml is not created
    ''' Reads data from xml for general operations
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
        Try
            'checks if the config file name 
            If m_strFileName = "" Then
                m_strFilePath = System.IO.Path.GetDirectoryName( _
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
                m_strFileName = m_strFilePath + "\" + "BatchProcess.xml"
            End If
            'creates the serializer object
            If m_objSerializer Is Nothing Then
                m_objSerializer = New XmlSerializer(GetType(ProcessingDetails))
            End If
            If Not File.Exists(m_strFileName) Then
                'write code to create the defual config xml
                m_objProcessingDeatils = New ProcessingDetails()
            Else
                ReadXml()
            End If
            m_objLock = New System.Object()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error During Initialisation of Batch Config Parser" + _
                                                  ex.Message.ToString, Logger.LogLevel.RELEASE)
            'AppContainer.GetInstance().objLogger.WriteAppLog("BatchConfigParser:New" + ex.Message, Logger.LogLevel.Release)
        End Try
    End Sub
    ''' <summary>
    ''' Function to clear download data from the object m_objProcessingDeatils
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ClearDownloadData()
        Try
            'Clear the reference download data present in processing details
            m_objProcessingDeatils.ProcessingStatus.Clear()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' GetInstance method of singleton class
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As BatchConfigParser
        objAppContainer.objLogger.WriteAppLog("Entered GetInstance of BatchConfigParser", Logger.LogLevel.INFO)
        If m_objBatchConfigParser Is Nothing Then
            m_objBatchConfigParser = New BatchConfigParser()
        End If
        Return m_objBatchConfigParser
        objAppContainer.objLogger.WriteAppLog("Exiting ValidateUser of BatchConfigParser", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Method to write the object parameters to an XML file
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub WriteXml()
        objAppContainer.objLogger.WriteAppLog("Entered WriteXml of BatchConfigParser", Logger.LogLevel.INFO)
        Try
            Dim fsWriter As New StreamWriter(m_strFileName)
            ' Call the serialize method to put object to xml
            m_objSerializer.Serialize(fsWriter, m_objProcessingDeatils)
            fsWriter.Close()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            '  objAppContainer.objLogger.WriteAppLog("BatchConfigParser:Writexml" + ex.Message, Logger.LogLevel.Release)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting WriteXml of BatchConfigParser", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Method to read the xml into and object
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ReadXml()
        objAppContainer.objLogger.WriteAppLog("Entered ReadXml of BatchConfigParser", Logger.LogLevel.INFO)
        Try
            Dim fsReader As New FileStream(m_strFileName, FileMode.Open)
            ' Call the Deserialize method to restore the object's state.
            m_objProcessingDeatils = CType(m_objSerializer.Deserialize(fsReader), ProcessingDetails)
            fsReader.Close()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            ' objAppContainer.objLogger.WriteAppLog("BatchConfigParser:readxml" + ex.Message, Logger.LogLevel.Release)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting ReadXml of BatchConfigParser", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Check if the file active as per synctrlf file is already
    ''' downloaded during a previous session
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsFileAlreadyDownloaded(ByVal strFileName As String) As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered IsFileAlreadyDownloaded of BatchConfigParser", Logger.LogLevel.INFO)
        Try
            Dim lstDetails As List(Of FileDetails)
            Dim bReturn As Boolean
            bReturn = False
            'lock the object so that another thread doesnt modify it
            SyncLock m_objLock
                lstDetails = m_objProcessingDeatils.ProcessingStatus
                'check the list for a match
                For Each objFileDetails As FileDetails In lstDetails
                    If objFileDetails.strFileName = strFileName Then
                        bReturn = True
                        Exit For
                    End If
                Next
                'release lock
            End SyncLock
            objAppContainer.objLogger.WriteAppLog("Exiting IsFileAlreadyDownloaded of BatchConfigParser", Logger.LogLevel.INFO)
            Return bReturn
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    ''' UpdateDBpopulationStatus method to update file status
    ''' when the file is succesfull populated to DB
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <param name="strStatus"></param>
    ''' <remarks></remarks>
    Public Sub UpdateDBpopulationStatus(ByVal strFileName As String, ByVal strStatus As String)
        objAppContainer.objLogger.WriteAppLog("Entered UpdateDBpopulationStatus of BatchConfigParser", Logger.LogLevel.INFO)
        Try
            Dim lstDetails As List(Of FileDetails)
            SyncLock m_objLock
                lstDetails = m_objProcessingDeatils.ProcessingStatus
                'search for file name and update
                For Each objFileDetails As FileDetails In lstDetails
                    If objFileDetails.strFileName = strFileName Then
                        objFileDetails.strBuildStatus = strStatus
                        Exit For
                    End If
                Next
                'write updated details to xml
                WriteXml()
            End SyncLock
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting UpdateDBpopulationStatus of BatchConfigParser", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' UpdateDBPopulationIndex, method to keep the pointer on file
    ''' upto which the DB has been populated( Used to restart a session
    ''' when DB population fails due to exception
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <param name="iIndex"></param>
    ''' <remarks></remarks>
    Public Sub UpdateDBPopulationIndex(ByVal strFileName As String, ByVal iIndex As Integer, Optional ByVal exception As String = "NA")
        objAppContainer.objLogger.WriteAppLog("Entered UpdateDBPopulationIndex of BatchConfigParser", Logger.LogLevel.INFO)
        Try
            Dim lstDetails As List(Of FileDetails)
            'Acquire lock
            SyncLock m_objLock
                lstDetails = m_objProcessingDeatils.ProcessingStatus
                'search for filename and update the details
                For Each objFileDetails As FileDetails In lstDetails
                    If objFileDetails.strFileName = strFileName Then
                        objFileDetails.dIndex = iIndex
                        objFileDetails.strException = exception.ToString
                        Exit For
                    End If
                Next
                'wriet the updated details to xml
                WriteXml()
            End SyncLock
            'release lock
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting UpdateDBPopulationIndex of BatchConfigParser", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' AddToXML, method to add downloaded file details to XML
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <remarks></remarks>
    Public Sub AddToXml(ByVal strName As String)
        objAppContainer.objLogger.WriteAppLog("Entered AddToXml of BatchConfigParser", Logger.LogLevel.INFO)
        Try
            Dim objFileDet As New FileDetails()
            Dim lstDetails As List(Of FileDetails)
            Dim bIsNodeAlreadyExists As Boolean = False
            objFileDet.strBuildStatus = "N"
            objFileDet.strFileName = strName
            SyncLock m_objLock
                'Fix for file details written multiple times for the same set of files
                'Check if there is alread
                lstDetails = m_objProcessingDeatils.ProcessingStatus
                'search for filename and update the details
                For Each objFileDetails As FileDetails In lstDetails
                    If objFileDetails.strFileName = strName Then
                        bIsNodeAlreadyExists = True
                        Exit For
                    End If
                Next
                'If the details for the file does not exist create a new element 
                'for the file.
                If Not bIsNodeAlreadyExists Then
                    'If details for the file not present.
                    m_objProcessingDeatils.ProcessingStatus.Add(objFileDet)
                End If
                'End fix
            End SyncLock
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting AddToXml of BatchConfigParser", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Method to get the list of partially populated files
    ''' Files may be partially populated to db due to error scenarios
    ''' The recovery is done using the last index populated
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPartiallyProcessedFiles() As ArrayList
        objAppContainer.objLogger.WriteAppLog("Entered GetPartiallyProcessedFiles of BatchConfigParser", Logger.LogLevel.INFO)
        Dim arrlPartiallyProcessed As New ArrayList()
        Dim lstDetails As List(Of FileDetails)
        Try
            'Lock acquired
            SyncLock m_objLock
                lstDetails = m_objProcessingDeatils.ProcessingStatus
                'Get details one by one to a list
                For Each objFileDetails As FileDetails In lstDetails
                    If objFileDetails.dIndex <> 0 AndAlso objFileDetails.strBuildStatus = "N" Then
                        Dim objFDetails As New FileDetails()
                        objFDetails.strFileName = objFileDetails.strFileName
                        objFDetails.strBuildStatus = objFileDetails.strBuildStatus
                        objFDetails.dIndex = objFileDetails.dIndex
                        'add the partially populated file details to list
                        arrlPartiallyProcessed.Add(objFDetails)
                    End If
                Next
            End SyncLock

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting GetPartiallyProcessedFiles of BatchConfigParser", Logger.LogLevel.INFO)
        'return the list to parent method
        Return arrlPartiallyProcessed
    End Function
    ''' <summary>
    ''' Method to update the xml when a number of parameters are modified
    ''' Something like a bulk commit
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateParams()
        SyncLock m_objLock
            WriteXml()
        End SyncLock
    End Sub
    ''' <summary>
    ''' Subroutine to clear the reference file processing status.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PurgeFile()
        SyncLock m_objLock
            m_objProcessingDeatils.ProcessingStatus.Clear()
            WriteXml()
        End SyncLock
    End Sub
End Class
