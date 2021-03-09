#If NRF Then
Imports System.IO
Public Class ReferenceDataMgr
    'Initialising the class, form, stream and reader
    Private Shared m_ReferenceDataMgr As ReferenceDataMgr = Nothing
    Public objDownloadReferenceData As frmDownloadReferenceData = Nothing
    Public objRefDataSummary As frmSummaryScreen = Nothing
    Private m_BatchProcessor As BatchProcessor
    Private m_objPwrState As PowerState
    Private m_FileArray As ArrayList
    ''' <summary>
    ''' Initialising The Variables
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()

    End Sub
    ''' <summary>
    ''' Getting instanace of the  Session Manager
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As ReferenceDataMgr
        If m_ReferenceDataMgr Is Nothing Then
            m_ReferenceDataMgr = New ReferenceDataMgr
            Return m_ReferenceDataMgr
        Else
            Return m_ReferenceDataMgr
        End If
    End Function
    ''' <summary>
    ''' Check if there are any files failed due to exception.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckFileException() As Boolean
        objAppContainer.objLogger.WriteAppLog("Validating the Previous Download for any Exceptions", Logger.LogLevel.INFO)
        Dim XMLfile As New Xml.XmlDocument
        Dim XMLElement As Xml.XmlElement
        Dim strError As String
        Dim strFile As String
        Dim iUpperLimit As Integer = 10
        Dim iCount As Integer = 0
        m_FileArray = New ArrayList
        Try
            If File.Exists(Macros.REF_STATUS_FILE) Then
                ' Load the file to get file exception details
                Try
                    XMLfile.Load(Macros.REF_STATUS_FILE)
                    XMLElement = XMLfile.DocumentElement
                Catch ex As Exception
                    XMLfile = Nothing
                    XMLElement = Nothing
                    MessageBox.Show("Unable to retrieve previous upload status.", "Alert")
                    Return False
                End Try
                'Adding Files to array list which has exception
                While iCount < XMLElement.GetElementsByTagName("strFileName").Count
                    strFile = XMLElement.GetElementsByTagName("strFileName").ItemOf(iCount).InnerText.ToString
                    strError = XMLElement.GetElementsByTagName("strBuildStatus").ItemOf(iCount).InnerText.ToString
                    If (strError <> "Y") Then
                        m_FileArray.Add(strFile)
                    End If
                    iCount = iCount + 1
                End While
                'Checking Error Occurence
                If (m_FileArray.Count > 0) Then
                    Return True
                End If
            Else
                MessageBox.Show("Unable to retrieve previous upload status.", "Alert")
                Return False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.INFO)
            'If Error Occurs during the operation the return start downloading the entire files list
            MessageBox.Show("Unable to retrieve previous download status", "Alert")
            Return False
        Finally
            'Releasing the memory allocated to hold the XML file and the Elements
            XMLfile = Nothing
            XMLElement = Nothing
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting CheckFileException Function", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Initiates the process to get the list of files failed and provides
    ''' option to download.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartDownloadsession()
        objAppContainer.objLogger.WriteAppLog("Entered Start Download Session", Logger.LogLevel.INFO)
        Try
            'Checking Whether the device is docked before starting the download
            If CheckDeviceDocked() Then
                objDownloadReferenceData = New frmDownloadReferenceData()
                m_objPwrState = New PowerState()
                PopulateListbox()
                m_objPwrState.UnSetSleepTimeOut()
                objDownloadReferenceData.ShowDialog()
                m_objPwrState.SetSleepTimeOut(CInt(objAppContainer.objConfigFileParams.BattSuspendTimeout))
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.INFO)
            MessageBox.Show("Error while uploading the reference files", "Error", _
                            MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting download session", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' To populate details in the drop down lost box.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PopulateListbox()
        Try
            objDownloadReferenceData.cmdDownloadOptions.Items.Clear()
            objDownloadReferenceData.cmdDownloadOptions.Visible = True
            objDownloadReferenceData.lblProcess.Visible = False
            objDownloadReferenceData.lblProcessindicator.Visible = False
            objDownloadReferenceData.Label1.Visible = False
            objDownloadReferenceData.pbFileTransfer.Visible = False
            objDownloadReferenceData.lblConnectionLost1.Visible = False
            objDownloadReferenceData.lblConnectionLost2.Visible = False
            objDownloadReferenceData.lblDBUpdation.Visible = False
            objDownloadReferenceData.lblPercentage.Visible = False
            objDownloadReferenceData.pgBar.Visible = False
            objDownloadReferenceData.cmdDownloadOptions.Items.Add("- Select an option -")

            If CheckFileException() Then
                objDownloadReferenceData.btnQuit.Visible = True
                objDownloadReferenceData.btnOk.Visible = False
                For Each objItems As String In m_FileArray
                    objDownloadReferenceData.cmdDownloadOptions.Items.Add(objItems)
                Next
                If m_FileArray.Count > 1 Then
                    objDownloadReferenceData.cmdDownloadOptions.Items.Add("Upload all Failed Files")
                End If
                objDownloadReferenceData.cmdDownloadOptions.Items.Add("Upload all Files")
            Else
                objDownloadReferenceData.btnQuit.Visible = False
                objDownloadReferenceData.btnOk.Visible = True
                objDownloadReferenceData.SetCurrentStatus("Reference data has been uploaded successfully. Click 'OK' for details. ")
                objDownloadReferenceData.cmdDownloadOptions.Items.Add("Upload all Files")
            End If
            'Make the first item in the list selected.
            objDownloadReferenceData.cmdDownloadOptions.SelectedIndex = 0
            'Refresh the form to display correct contents.
            objDownloadReferenceData.Refresh()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            MessageBox.Show("Error while populating files", "Error", _
                            MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
        End Try
    End Sub
    ''' <summary>
    ''' Reads the export data file and displays to the user in a text box
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RecieveData(ByVal strFileName As String)
        objAppContainer.objLogger.WriteAppLog("Entered RecieveData of ReferenceDataMgr", Logger.LogLevel.INFO)
        Try
            'Disabling the controls that are not required
            objDownloadReferenceData.StartUserControl()
            objDownloadReferenceData.btnOk.Visible = False
            objDownloadReferenceData.cmdDownloadOptions.Visible = False
            objDownloadReferenceData.btnQuit.Visible = False
            'Set the status text.
            objDownloadReferenceData.lblProcess.Visible = True
            objDownloadReferenceData.lblProcess.Text = "Uploading Reference Data"
            objDownloadReferenceData.lblProcessindicator.Visible = True
            objDownloadReferenceData.lblProcessindicator.Text = "Initialising Upload...."
            objDownloadReferenceData.Refresh()
            'Allocating Memory to Start Download
            m_BatchProcessor = New BatchProcessor()
            If strFileName = "0" Then
                m_BatchProcessor.BatchProcess()
            ElseIf strFileName = "1" Then
                m_BatchProcessor.DownloadSelectedFiles(m_FileArray)
            ElseIf strFileName <> Nothing Then
                Dim arrFileList As ArrayList = New ArrayList()
                arrFileList.Add(strFileName)
                m_BatchProcessor.DownloadSelectedFiles(arrFileList)
            Else
                MessageBox.Show("Select a valid item from the list.", "MCUtilities")
            End If
            'Clear the list that is holding the files that need to be downloaded.
            m_FileArray.Clear()
            ConfigDataMgr.GetInstance.SetParam("ReferenceFileDownload", DateAndTime.Now.ToString("dd/MM/yyyy HH:MM:ss"))
            'run sample queries to index the tables
            Dim objRefFileParser As ReferenceFileParser = New ReferenceFileParser()
            'execute teh query
            objRefFileParser.ExecuteSampleQuery("Module")
            'Close the connection to tha database.
            objRefFileParser.Terminate()
            'end run sample queries
            'Diabling the user control after complete download
            objDownloadReferenceData.StopUserControl()
            PopulateListbox()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            MessageBox.Show("Error while uploading the reference files", "Error", _
                            MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting RecieveData of ReferenceDataMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' To check for the dynamic IP generated when the device is docked
    ''' </summary>
    ''' <remarks></remarks>
    Public Function GetIPAddress() As String
        objAppContainer.objLogger.WriteAppLog("Entered GetIPAddress of ReferenceDataMgr", Logger.LogLevel.INFO)
        'Declare the local variable and the get the host name
        Dim sDnsName As String = Nothing
        Dim m_IpHostEntry As System.Net.IPHostEntry = Nothing
        Dim m_aIPAddressArray As System.Net.IPAddress() = Nothing
        Dim strIP As String = ""
        Dim iIndex As Integer = 0
        Dim aIPSubnet() As String = Nothing
        Try
            sDnsName = System.Net.Dns.GetHostName()
            m_IpHostEntry = System.Net.Dns.GetHostEntry(sDnsName)
            m_aIPAddressArray = m_IpHostEntry.AddressList()
            ' Check if the address array has a default value
            If m_aIPAddressArray.Length > 0 Then
                ' Check within a loop whether the IP is else then 127.0.0.1
                For iIndex = 0 To m_aIPAddressArray.Length - 1
                    ' If the address IP is else then convert it into string 
                    If m_aIPAddressArray(iIndex).ToString() = "127.0.0.1" And _
                       m_aIPAddressArray.Length = 1 Then
                        strIP = m_aIPAddressArray(iIndex).ToString()

                    ElseIf m_aIPAddressArray(iIndex).ToString() <> "127.0.0.1" Then
                        strIP = m_aIPAddressArray(iIndex).ToString()
                    End If
                Next
                'Return the new IP generated when the device is docked into the cradle
                'format the IP address to have 3 digits in all the three subnets
                aIPSubnet = strIP.Split(".")
                aIPSubnet(0) = aIPSubnet(0).PadLeft(3, "0")
                aIPSubnet(1) = aIPSubnet(1).PadLeft(3, "0")
                aIPSubnet(2) = aIPSubnet(2).PadLeft(3, "0")
                aIPSubnet(3) = aIPSubnet(3).PadLeft(3, "0")
                strIP = aIPSubnet(0) & "." & aIPSubnet(1) & "." & _
                           aIPSubnet(2) & "." & aIPSubnet(3)

                'returnt he IP address to the calling function
                objAppContainer.objLogger.WriteAppLog("Exiting GetIPAddress of ReferenceDataMgr", Logger.LogLevel.INFO)
                Return strIP
            Else
                'Return the default IP of the device when the device is 
                'not docked into the cradle
                objAppContainer.objLogger.WriteAppLog("Exiting GetIPAddress of ReferenceDataMgr", Logger.LogLevel.INFO)
                Return "127.000.000.001"
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            Return "127.000.000.001"
        End Try
    End Function
    ''' <summary>
    ''' To check whether the device has been docked into cradle or not
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CheckDeviceDocked() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered CheckDeviceDocked of ReferenceDataMgr", Logger.LogLevel.INFO)
        'Declare local variable
        Try
            Dim strDeviceIP As String = Nothing
            Dim bTemp As Boolean = False
            Dim iCount As Integer = 3
            Do
                'Check for device IP if it has been docked 
                strDeviceIP = GetIPAddress()
                If strDeviceIP <> "127.000.000.001" And strDeviceIP <> "" And _
                                            strDeviceIP <> Nothing Then
                    bTemp = True
                Else
                    'Displaying Error Message if the device is not Docked
                    MessageBox.Show("Please make sure that the device has been docked properly.", _
                                    "Alert", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                    MessageBoxDefaultButton.Button1)
                    bTemp = False
                    iCount = iCount - 1
                    'Waiting for user to Dock
                    Threading.Thread.Sleep(2000)
                End If
                'Giving the user Three Chances or exiting from download
            Loop Until bTemp = True Or iCount = 0
            objAppContainer.objLogger.WriteAppLog("Exiting CheckDeviceDocked of ReferenceDataMgr", Logger.LogLevel.INFO)
            Return bTemp
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    ''' Displaying the summary screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplaySummary()
        objAppContainer.objLogger.WriteAppLog("Entered DisplaySummary of ReferenceDataMgr", Logger.LogLevel.INFO)
        Try
            'Allocating Memory to the summary screen
            'Displaying the screen
            'Releasing all the memory occupied by the screen
            objRefDataSummary = New frmSummaryScreen("ReferenceFileSummary")
            'objRefDataSummary.strInvokingform = "ReferenceFileSummary"
            objRefDataSummary.Text = "Reference Data Upload Status"
            objRefDataSummary.ShowDialog()
            objRefDataSummary.Dispose()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting DisplaySummary of ReferenceDataMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Releases all the memory resourses used by export data Viewer
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EndSession()
        objAppContainer.objLogger.WriteAppLog("Entered EndSession of ReferenceDataMgr", Logger.LogLevel.INFO)
        'Destroying all the memory Locations Occupied
        Try
            m_BatchProcessor = Nothing
            objDownloadReferenceData = Nothing
            m_ReferenceDataMgr = Nothing
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            MessageBox.Show("Error while uploading the reference files.", "Error", _
                             MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting EndSession of ReferenceDataMgr", Logger.LogLevel.INFO)
    End Sub
End Class
#End If