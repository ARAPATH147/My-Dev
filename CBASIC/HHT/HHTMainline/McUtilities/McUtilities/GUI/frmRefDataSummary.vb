Public Class frmSummaryScreen
    Public strInvokingform As String
    Private Sub btnOk_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        Me.Close()
    End Sub

    Private Sub frmRefDataSummary_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim strFileName As String = ""
        Dim iUpperFileLimit As Integer = 10
        Dim m_XMLStatusFile As New Xml.XmlDocument
        Dim StrException As String
        Dim iCount As Integer = 0
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        If (strInvokingform = "ActiveDataSummary") Then
            strFileName = Macros.ACT_EXP_STATUSFILE
            iUpperFileLimit = 7
            Try
                If System.IO.File.Exists(strFileName) Then
                    m_XMLStatusFile.Load(strFileName.ToString)
                    Dim m_NodeList As Xml.XmlNodeList
                    Dim m_Node As Xml.XmlNode
                    m_NodeList = m_XMLStatusFile.SelectNodes("/ProcessingStatus/add")

                    For Each m_Node In m_NodeList
                        StrException = m_Node.Attributes.GetNamedItem("exception").Value.ToString
                        SummaryList.Items.Add(New ListViewItem(m_Node.Attributes.GetNamedItem("key").Value.ToString))
                        If (StrException <> "NA") Then
                            SummaryList.Items(iCount).SubItems.Add("Failed")
                        Else
                            SummaryList.Items(iCount).SubItems.Add("Success")
                        End If
                        iCount = iCount + 1
                        If Not iCount < iUpperFileLimit Then
                            Exit For
                        End If
                    Next
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                Else
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                    MessageBox.Show("Active data upload status file - Not Found", _
                                     "Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    Me.Close()
                End If
            Catch ex As Exception
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                MessageBox.Show("Active data upload status file - Not Found", _
                                   "Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                Me.Close()
            End Try
        ElseIf (strInvokingform = "ReferenceFileSummary") Then
            strFileName = Macros.REF_STATUS_FILE.ToString
            iUpperFileLimit = 10
            Dim Status As Xml.XmlElement
            If System.IO.File.Exists(strFileName.ToString) Then
                Try
                    m_XMLStatusFile.Load(strFileName.ToString)
                    Status = m_XMLStatusFile.DocumentElement
                    While iCount < iUpperFileLimit
                        SummaryList.Items.Add(New ListViewItem(Status.GetElementsByTagName("strFileName").ItemOf(iCount).InnerText.ToString))
                        StrException = Status.GetElementsByTagName("exception").ItemOf(iCount).InnerText.ToString()
                        If StrException = "NA" Then
                            SummaryList.Items(iCount).SubItems.Add("Success")
                        Else
                            SummaryList.Items(iCount).SubItems.Add("Failed ")
                        End If

                        iCount = iCount + 1
                    End While
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                Catch ex As Exception
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                    MessageBox.Show("Error while retrieving reference data download status", _
                                     "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    Me.Close()
                End Try
                Status = Nothing
                m_XMLStatusFile = Nothing

            Else
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                MessageBox.Show("Reference data download status file - Not Found", _
                                     "Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                Me.Close()
            End If
        End If

    End Sub
End Class