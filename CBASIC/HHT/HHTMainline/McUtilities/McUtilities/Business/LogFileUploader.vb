#If NRF Then
Imports System.IO
Public Class LogFileUploader
    Public Sub Start(ByVal strFileName As String, Optional ByVal isSendAll As Boolean = False)
        If Not (isSendAll) Then
            objAppContainer.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_SEND, _
                                LOGTransmitter.Status.START, LOGTransmitter.FileName.POD_Log_Files, LOGTransmitter.Reasons.Downloading_File)
        End If
        objAppContainer.objLogger.WriteAppLog("Started Uploading Log Files from start function of LogfileUploader", Logger.LogLevel.INFO)
        Try
            Dim LocalPath As String = objAppContainer.objConfigFileParams.LocalLogFilePath
            Dim RemotePath As String = objAppContainer.objConfigFileParams.RemoteLogFilePath
            If LocalPath = "" Then
                objAppContainer.objLogger.WriteAppLog("LogFileUpLoader:Start" + "Log Directory not found on config file", Logger.LogLevel.RELEASE)
                Exit Sub
            End If
            If Directory.Exists(LocalPath) Then
                ' Process the list of files found in the directory.
                Dim strDeviceIP As String = ReferenceDataMgr.GetInstance.GetIPAddress().Split(".")(3)
                Dim objTFTPSession As New TFTPClient.TFTPSession()
                Dim tyOptions As New TFTPClient.TransferOptions()
                'RemoteFileNameTxt.Text;
                'tyOptions.Host = objAppContainer.objConfigFileParams.TFTP.Host
                tyOptions.Host = objAppContainer.strActiveIP
                tyOptions.Port = objAppContainer.objConfigFileParams.TFTP.Port
                'tOptions.Action = getRadio.Checked == true ? TransferType.Get : TransferType.Put;
                tyOptions.Action = TFTPClient.TransferType.Put
                'upload files one after another
                'strFilename itself is giving full file path
                tyOptions.LocalFilename = strFileName
                'LocalFileNameTxt.Text;
                If RemotePath = "" Then
                    tyOptions.RemoteFilename = Path.GetFileName(strFileName)
                    '.Split(".")(0) + "." + strDeviceIP
                Else
                    tyOptions.RemoteFilename = RemotePath.TrimEnd("\") + "\" + Path.GetFileName(strFileName)
                    '.Split(".")(0) + "." + strDeviceIP
                End If
                objTFTPSession.Put(tyOptions)
                'Deleting File is not necessary in Case of Copy to Controller
                'File.Delete(strFileName)
                objTFTPSession = Nothing
                If Not (isSendAll) Then
                    objAppContainer.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_SEND, _
                               LOGTransmitter.Status.END_OK, LOGTransmitter.FileName.POD_Log_Files, LOGTransmitter.Reasons.Download_Complete)
                End If
            Else
                objAppContainer.objLogger.WriteAppLog("LogFileUpLoader:Start" + "Log Directory doesnot exist", Logger.LogLevel.RELEASE)
                'Send Directory Doesnt Exist as error Log
                objAppContainer.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_SEND, _
                                LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.POD_Log_Files, LOGTransmitter.Reasons.File_Not_Found)
            End If
        Catch ex As Exception
            'Send the Error as Log 
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.INFO)
            objAppContainer.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_SEND, _
                                LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.POD_Log_Files, LOGTransmitter.Reasons.Other_Errors)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting Log File Download Start Function", Logger.LogLevel.INFO)
    End Sub
End Class
#End If