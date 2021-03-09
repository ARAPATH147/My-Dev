Imports Microsoft.Win32

''' <summary>
''' MCF v1.1    Kiran Sajeev    Jan 2013
''' Form added for Airbeam IP configuration module in the utilities application. 
''' The Airbeam IP is stored in the Airbeam registry which is lookedup by the Airbeam client
''' while synchronizing the application image with the controller.
''' </summary>

Public Class frmIPConfigure
    Private regActiveServerIP As RegistryKey
    Private strServerIPRegKey As String
    ''' <summary>
    ''' When click on airbeam IP Configure image.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PicBox_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbSaveBtn.Click
        Dim strSelectedIP As String = Nothing
        Try
            'Get the selected IP value
            strSelectedIP = GetSelectedIP()
            'Access the value from registry
            strServerIPRegKey = "SOFTWARE\AIRBEAM"
            regActiveServerIP = Registry.LocalMachine.OpenSubKey(strServerIPRegKey, True)
            'Compare the IP to set the value
            If regActiveServerIP.GetValue("SERVERIP").ToString() = strSelectedIP Then
                MessageBox.Show("Server IP address already active", _
                                "Alert", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                 MessageBoxDefaultButton.Button1)
                Return
            End If
            regActiveServerIP.SetValue("SERVERIP", strSelectedIP)
            MessageBox.Show("Server IP address updated")
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured while updating AirBEAM IP", Logger.LogLevel.RELEASE)
            MessageBox.Show("Error updating registry value", "Error", _
                             MessageBoxButtons.OK, _
                             MessageBoxIcon.Exclamation, _
                             MessageBoxDefaultButton.Button1)
        End Try
    End Sub

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Access the value from registry
        strServerIPRegKey = "SOFTWARE\AIRBEAM"
        regActiveServerIP = Registry.LocalMachine.OpenSubKey(strServerIPRegKey, True)
        'Compare the IP to set the value
        If lblPrimaryIP.Text = regActiveServerIP.GetValue("SERVERIP").ToString() Then
            rbtnPrimaryIp.Checked = True
            rbtnSecondaryIp.Checked = False
        Else
            rbtnSecondaryIp.Checked = True
            rbtnPrimaryIp.Checked = False
        End If
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbQuitBtn.Click
        AirbeamIPConfig.GetInstance().EndSession()
    End Sub
    ''' <summary>
    ''' setthe IP based on the radio button selection
    ''' </summary>
    ''' <remarks></remarks>
    Private Function GetSelectedIP() As String
        Dim strValue As String = Nothing
        If rbtnPrimaryIp.Checked Then
            strValue = lblPrimaryIP.Text()
        Else
            strValue = lblSecondaryIp.Text()
        End If
        Return strValue
    End Function
End Class
