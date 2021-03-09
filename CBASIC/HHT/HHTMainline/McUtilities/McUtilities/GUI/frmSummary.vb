Public Class frmSummaryScreen
    Private strInvokingform As String
    Public Sub New(ByVal strInvokeForm As String)
        strInvokingform = strInvokeForm
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Private Sub btnOk_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        Me.Close()
    End Sub
    Private Sub frmRefDataSummary_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Initialising Variables 
        Dim strFileName As String = ""
        Dim iUpperFileLimit As Integer = 10
        Dim m_XMLStatusFile As New Xml.XmlDocument
        Dim strStatus As String = Nothing
        Dim iCount As Integer = 0
        'setting the process bar status
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        'Identifying the invoking Form
        If (strInvokingform = "ActiveDataSummary") Then
            'setting the file name in case of active data 
            strFileName = Macros.ACT_EXP_STATUSFILE
            iUpperFileLimit = Macros.NO_ACT_FILES
            Try
                'Checking the existence of the active file
                If System.IO.File.Exists(strFileName) Then
                    'Loading the active File
                    m_XMLStatusFile.Load(strFileName.ToString)
                    'Defining Lode and Node list
                    Dim m_NodeList As Xml.XmlNodeList
                    Dim m_Node As Xml.XmlNode
                    'Selecting the nodes in Processing Status
                    m_NodeList = m_XMLStatusFile.SelectSingleNode("/filestatus").ChildNodes                    ' Processing Each node
                    For Each m_Node In m_NodeList
                        'Cating the exception status of the node
                        strStatus = m_Node.Attributes.GetNamedItem("status").Value.ToString
                        'Adding the file name to the list view
                        SummaryList.Items.Add(New ListViewItem(m_Node.Attributes.GetNamedItem("name").Value.ToString))
                        'Checking for the exception
                        If (strStatus = "F") Then
                            'Exception occured and hence adding "Failed" to List view subitem
                            SummaryList.Items(iCount).SubItems.Add("Failed")
                        ElseIf (strStatus = "P") Then
                            'No Exception occured and hence adding "Success" to List view subitem
                            SummaryList.Items(iCount).SubItems.Add("Success")
                        Else
                            'Exception occured and hence adding "Failed" to List view subitem
                            SummaryList.Items(iCount).SubItems.Add("NA")
                        End If
                        iCount = iCount + 1
                        If Not iCount < iUpperFileLimit Then
                            'Executing till the fist seven nodes are passed
                            'There are seven active files and the rest three nodes has export data status 
                            Exit For
                        End If
                    Next
                    'Closing the processing status
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                Else
                    'In case file not found then displaying the error message
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                    MessageBox.Show("Active data upload status file - Not Found", _
                                     "Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    'Closing the form
                    Me.Close()
                End If
            Catch ex As Exception
                ' In Case of Error displaying the error message
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                MessageBox.Show("Unable to retrieve the active data build status.", _
                                   "Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                'Closing the form
                Me.Close()
            End Try
            'Checking for the invoking form to be Reference data manager
        ElseIf (strInvokingform = "ReferenceFileSummary") Then
            strFileName = Macros.REF_STATUS_FILE.ToString
            iUpperFileLimit = Macros.NO_REF_FILES
            Dim ndList As Xml.XmlNodeList
            'checking the existence of the file
            If System.IO.File.Exists(strFileName.ToString) Then
                Try
                    m_XMLStatusFile.Load(strFileName.ToString)
                    ndList = m_XMLStatusFile.DocumentElement.FirstChild.ChildNodes
                    'Processing until all node are parsed
                    For Each xmlItem As Xml.XmlNode In ndList
                        'Adding File Name to list view
                        SummaryList.Items.Add(New ListViewItem(xmlItem.SelectSingleNode("strFileName").InnerText.ToString))
                        'Getting the exception status 
                        strStatus = xmlItem.SelectSingleNode("strBuildStatus").InnerText.ToString()
                        If strStatus = "Y" Then
                            'No Exception occured and hence assigning "Success" to the subitem
                            SummaryList.Items(iCount).SubItems.Add("Success")
                        ElseIf strStatus = "N" Then
                            'Exception occured and hence assigning "Failed" to the subitem
                            SummaryList.Items(iCount).SubItems.Add("Failed ")
                        Else
                            'Exception occured and hence assigning "Failed" to the subitem
                            SummaryList.Items(iCount).SubItems.Add("NA ")
                        End If
                        iCount = iCount + 1
                    Next
                    'Setting the processing status to null
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                Catch ex As Exception
                    'In Case of error the message is displayed and writing to the logger
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                    MessageBox.Show("Unable to retrieve the active data build status.", _
                                     "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    Me.Close()
                End Try
                ndList = Nothing
                m_XMLStatusFile = Nothing

            Else
                'If file not found then displaying the appropriate message
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                MessageBox.Show("Reference data upload status file - Not Found", _
                                     "Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                Me.Close()
            End If
        End If

    End Sub
End Class