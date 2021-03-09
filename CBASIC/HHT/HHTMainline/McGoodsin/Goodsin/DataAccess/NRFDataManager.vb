'******************************************************************************
' <FileName> SMExportDataManager</FileName>
' <summary>
' The file contains the structure definition for the ExportDataClass required
' by the shelf management modules
' </summary>
' <Author>Infosys Technologies Ltd.,</Author>
' <DateModified>08-Nov-2008</DateModified>
' <include></include>
' <remarks></remarks>          
'******************************************************************************
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Connects to alternate controller while primary is down
''' </Summary>
'****************************************************************************
' No:      Author            Date            Description 
' 1.2 Christopher Kitto   28/04/2015   Modified as part of DALLAS project.
'           (CK)                       Added new function CreateDAR
'********************************************************************************
#If NRF Then
Imports System.Text
Imports System.Environment
Imports Goodsin.FileIO
Imports Goodsin.RFDataStructure
Public Class NRFDataManager

    ' Declaration of private member variable for reading export data file name
    Private strExFileName As String = Nothing
    Private strExFilePath As String = Nothing
    Private strExportFile As String = Nothing
    '  Private strPLListID As String = Nothing
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        'Read the Export Data File path
        strExFilePath = ConfigDataMgr.GetInstance.GetParam(ConfigKey.EXPORT_FILE_PATH)

        ' Read the Export Data File name for actual export data
        strExFileName = GetFileName(ExFileType.EXData)

        'Preapare the full export data path appended with file name
        strExportFile = strExFilePath + strExFileName
    End Sub
    ''' <summary>
    ''' To get the option to which file the data has to be written.
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum ExFileType
        GITemp
        EXData
    End Enum
    ''' <summary>
    ''' Returns the export data file name to which records are written.
    ''' </summary>
    ''' <param name="eFileType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetFileName(ByVal eFileType As ExFileType) As String
        Select Case eFileType
            Case ExFileType.EXData
                ' Read the Export Data File name for actual export data
                Return Macros.EXPORT_FILE_NAME
            Case Else
                ' Read the Export Data File name for actual export data
                Return Macros.EXPORT_FILE_NAME
        End Select
    End Function
    '**************************************************************************
    ' <summary>
    ' The function creates then transact message for GIA
    ' </summary>
    ' <param name=none></param>
    ' <returnvalue>boolean</returnvalue>
    '<remarks></remarks>	
    '**************************************************************************
    Public Function CreateGIA( ByRef objGIARecord As GIARecord) As Boolean

        'Local variable declaration 
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()

        'strExFileName = GetFileName(eExFile)
        ''Preapare the full export data path appended with file name
        'strExportFile = strExFilePath + strExFileName

        ' Appending the details to the string 
        strExportDataString.Append("GIA,")

        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID.TrimStart("0").PadLeft(3, "0") + ",")
        ' strExportDataString.Append(objGIARecord.strOperatorID + ",")
        strExportDataString.Append(objGIARecord.strDeliveryType + ",")
        strExportDataString.Append(objGIARecord.strFunction + ",")
        strExportDataString.Append(objGIARecord.strRequestType + ",")
        strExportDataString.Append(objGIARecord.strPeriod + ",")
        strExportDataString.Append(objGIARecord.iPointer.ToString().PadLeft(6, "0"))

        ' Take the Operator ID from the Appcontainer
        '  strExportDataString.Append(objAppContainer.strCurrentUserID)
        strExportDataString.Append(Environment.NewLine)

        Try
            'Writing the received data into the export data file
            bTemp = FileIO.WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)

            'TODO -> FOR EVERYFUNCITON TO RESET THE EXPORT DATA STRING 
            strExportDataString.Length = 0
        Catch ex As Exception
            Return False
        End Try
        Return bTemp
    End Function
    '**************************************************************************
    ' <summary>
    ' The function creates then transact message for GIF
    ' </summary>
    ' <param name=none></param>
    ' <returnvalue>boolean</returnvalue>
    '<remarks></remarks>	
    '**************************************************************************
    Public Function CreateGIF(ByRef objGIFRecord As GIFRecord) As Boolean

        ' Declaration of local variable

        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()

        'strExFileName = GetFileName(eExFile)
        ''Preapare the full export data path appended with file name
        'strExportFile = strExFilePath + strExFileName

        ' Appending the details to the  string 
        strExportDataString.Append("GIF,")

        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID.TrimStart("0").PadLeft(3, "0") + ",")
        ' strExportDataString.Append(objGIFRecord.strOperatorID + ",")
        strExportDataString.Append(objGIFRecord.strDeliveryType + ",")
        strExportDataString.Append(objGIFRecord.strFunction + ",")

        strExportDataString.Append(objGIFRecord.strScanCode + ",")


        strExportDataString.Append(objGIFRecord.strDespatchDate + ",")
        strExportDataString.Append(objGIFRecord.cScanType + ",")
        strExportDataString.Append(objGIFRecord.cScanLevel.ToString() + ",")
        strExportDataString.Append(objGIFRecord.strScanDate + ",")
        strExportDataString.Append(objGIFRecord.strScanTime + ",")
        strExportDataString.Append(objGIFRecord.strDriverBadge + ",")
        strExportDataString.Append(objGIFRecord.cGITNote.ToString() + ",")
        strExportDataString.Append(objGIFRecord.cBatchRescan.ToString() + ",")
        strExportDataString.Append(objGIFRecord.strBarcode + ",")
        strExportDataString.Append(objGIFRecord.strQuantity + ",")
        strExportDataString.Append(objGIFRecord.strItemStatus + ",")
        strExportDataString.Append(objGIFRecord.Sequence.ToString())


        strExportDataString.Append(Environment.NewLine)

        Try
            'Writing the received data into the export data file
            bTemp = FileIO.WriteDataIntoFile(strExportFile, _
                                                  strExportDataString.ToString(), True)

        Catch ex As Exception
            Return False
        End Try
        Return bTemp
    End Function

    ' V1.2 - CK
    ' Added new function CreateDAR

    '''**************************************************************************
    ''' <summary>
    ''' The function creates the transact message for DAR
    ''' </summary>
    ''' <param name="objDARRecord"></param>
    ''' <returnvalue>boolean</returnvalue>
    '''<remarks></remarks>	
    '''**************************************************************************
    Public Function CreateDAR(ByRef objDARRecord As DARRecord) As Boolean
        Dim bTemp As Boolean = False
        Dim cExportDataString As New System.Text.StringBuilder()

        ' Appending the transaction ID to the string
        cExportDataString.Append("DAR,")

        ' Appending the Operator ID
        cExportDataString.Append(objAppContainer.strCurrentUserID.TrimStart("0").PadLeft(3, "0") + ",")

        ' Appending Scanned Dallas barcode
        cExportDataString.Append(objDARRecord.cDallasBarcode + ",")

        ' Appending Scanned date
        cExportDataString.Append(objDARRecord.cScanDate + ",")

        ' Appending scan status
        cExportDataString.Append(objDARRecord.cScanStatus + ",")

        cExportDataString.Append(Environment.NewLine)

        Try
            ' Writing the received data into the export data file
            bTemp = FileIO.WriteDataIntoFile(strExportFile, _
                                             cExportDataString.ToString(), True)
        Catch ex As Exception
            Return False
        End Try
        Return bTemp

    End Function
    '**************************************************************************
    ' <summary>
    ' The function creates then transact message for GIX(Goods In Exit)
    ' </summary>
    ' <param name=none></param>
    ' <returnvalue>boolean</returnvalue>
    '<remarks></remarks>	
    '**************************************************************************
    Public Function CreateGIX(ByRef objGIXRecord As GIXRecord) As Boolean

        ' Declaration of local variable
        Dim bTemp As Boolean = False
        Dim strExportDataString As New System.Text.StringBuilder()
        strExportDataString.Append("GIX,")
        ' Take the Operator ID from the Appcontainer
        strExportDataString.Append(objAppContainer.strCurrentUserID.TrimStart("0").PadLeft(3, "0") + ",")
        strExportDataString.Append(objGIXRecord.strDeliveryType + ",")
        strExportDataString.Append(objGIXRecord.strFunction + ",")
        strExportDataString.Append(objGIXRecord.cIsAbort)
        strExportDataString.Append(Environment.NewLine)

        Try
            'Writing the received data into the export data file
            bTemp = FileIO.WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)

        Catch ex As Exception
            Return False
        End Try
        Return bTemp
    End Function

    '**************************************************************************
    ' <summary>
    ' The function creates the transact message for SOR(Sign On Request)
    ' </summary>
    ' <param name="objSOR"></param>
    ' <returnvalue>boolean</returnvalue>
    '<remarks></remarks>	
    '**************************************************************************
    Public Function CreateSOR(ByVal strPassword As String) As Boolean
        'Local variable declaration
        Dim bTemp As Boolean = False
        Dim strExportDataString As String = Nothing
        Try
            'Get the SOR record.
            strExportDataString = GenerateSOR(strPassword)
            'Writing the received data into the export data file
            bTemp = WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("NRFDataManager: SOR record write success", _
                                                  Logger.LogLevel.RELEASE)
        Catch ex As Exception
            'Write to log file
            objAppContainer.objLogger.WriteAppLog("NRFDataManager: SOR record write failure", _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' Generate SOR export
    ''' </summary>
    ''' <param name="strPassword"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GenerateSOR(ByVal strPassword As String, Optional ByVal strUserID As String = "") As String
        Dim strFreeMem As String = Nothing  'Free memory in Mega Bits (Mb)
        Dim strAppId As String = Nothing
        Dim strMACId As String = Nothing
        Dim strIPAdd As String = Nothing
        Dim strAppVersion As String = Nothing
        Dim strStoreType As String = Nothing
        Dim strExportDataString As New System.Text.StringBuilder()
        Dim lFreeMem As Long = Nothing
        Dim strVersion As String = Nothing
        'Read app version from config file
        'To split the appverion and release version. Actual app version should be send in SOR
        Dim aReleaseVersion() As String = Nothing

        strVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
        aReleaseVersion = strVersion.Split("-")
        strAppVersion = aReleaseVersion(1)
        'Read app version from config file
        'strAppVersion = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_VERSION)
        strAppId = ConfigDataMgr.GetInstance.GetParam(ConfigKey.APP_ID)
        strStoreType = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DEVICE_TYPE)
        strMACId = objAppContainer.objHelper.GetMacAddress()
        objAppContainer.objLogger.WriteAppLog("ExportDataManager: Get MAC ID success" + strMACId, _
                                              Logger.LogLevel.RELEASE)
        strFreeMem = objAppContainer.objHelper.CheckForFreeMemory("Program Files", lFreeMem)
        objAppContainer.objLogger.WriteAppLog("ExportDataManager: Get Free Mem success" + strFreeMem, _
                                              Logger.LogLevel.RELEASE)

        'Right justify,zero-filled
        strAppVersion = strAppVersion.PadLeft(4, "0")
        strAppVersion = strAppVersion.Replace(".", "0")
        strFreeMem = strFreeMem.PadLeft(8, "0")

        'Split each subnet and pad left with 0s
        strIPAdd = objAppContainer.objHelper.GetIPAddress()
        objAppContainer.objLogger.WriteAppLog("ExportDataManager: Get IP success" + strIPAdd, _
                                              Logger.LogLevel.INFO)

        'Appending the details to the string 
        strExportDataString.Append("SOR")
        strExportDataString.Append(",")
        ' Take the Operator ID from the Appcontainer
        If strUserID <> "" Then
            strExportDataString.Append(strUserID)
        Else
            strExportDataString.Append(objAppContainer.strCurrentUserID.TrimStart("0").PadLeft(3, "0"))
        End If
        strExportDataString.Append(",")
        strExportDataString.Append(strPassword)
        strExportDataString.Append(",")
        strExportDataString.Append(strAppId)
        strExportDataString.Append(",")
        strExportDataString.Append(strAppVersion)
        strExportDataString.Append(",")
        strExportDataString.Append(strMACId)
        strExportDataString.Append(",")
        strExportDataString.Append(strStoreType)
        strExportDataString.Append(",")
        strExportDataString.Append(strIPAdd)
        strExportDataString.Append(",")
        strExportDataString.Append(strFreeMem)
        strExportDataString.Append(Environment.NewLine)

        'Return the SOR record generated.
        GenerateSOR = strExportDataString.ToString()
    End Function
    '**************************************************************************
    ' <summary>
    ' The function creates the transact message for OFF(HHT Sign Off)
    ' </summary>
    ' <param name=>None</param>
    ' <returnvalue>boolean</returnvalue>
    '<remarks></remarks>	
    '**************************************************************************
    Public Function CreateOFF(ByVal IsCallForCrash As Boolean) As Boolean

        ' Local variable declaration
        Dim bTemp As Boolean
        Dim strExportDataString As New System.Text.StringBuilder()

        'Appending the details to the string 
        strExportDataString.Append("OFF")
        strExportDataString.Append(",")

        'Check whether the OFF record is for crash recovery or not
        If Not IsCallForCrash Then
            ' Take the Operator ID from the Appcontainer
            strExportDataString.Append(objAppContainer.strCurrentUserID.TrimStart("0").PadLeft(3, "0"))
        Else
            'Take the Operation ID from the Config File
            strExportDataString.Append(ConfigDataMgr.GetInstance.GetParam(ConfigKey.PREVIOUS_USER).TrimStart("0").PadLeft(3, "0"))
        End If
        strExportDataString.Append(Environment.NewLine)

        Try
            'Writing the received data into the export data file
            bTemp = FileIO.WriteDataIntoFile(strExportFile, _
                                      strExportDataString.ToString(), True)
        Catch ex As Exception
            Return False
        End Try
        Return bTemp
    End Function
    'v1.1 MCF Change
    '' <summary>
    '' change the IP to alternate controller IP
    '' </summary>
    '' <remarks>none</remarks>
    Public Sub sConnectAlternateInBatch()
        Try
            objAppContainer.iConnectedToAlternate = 1
            If objAppContainer.strActiveIP = _
                              ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString() Then
                objAppContainer.strActiveIP = _
                              ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.SECONDARY_IPADDRESS).ToString()
            Else
                objAppContainer.strActiveIP = _
                              ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString()
            End If
            objAppContainer.objLogger.WriteAppLog("IP ADDRESS CHANGED TO " + objAppContainer.strActiveIP, Logger.LogLevel.RELEASE)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in sConnectAlternateInBatch " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class
#End If

