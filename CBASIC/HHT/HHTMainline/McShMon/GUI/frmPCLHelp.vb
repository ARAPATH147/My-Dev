''' * Modification Log
''' ********************************************************************
''' * 1.1   Archana Chandramathi    13 C Chilled Food Changes
''' Remove the unwanted letters: BAU: Clearance label print screen shows
''' blocks within the text
''' ********************************************************************/
Public Class frmPCLHelp
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Btn_Ok1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Ok1.Click
        Try
            Btn_Ok1.Enabled = False
            CLRSessionMgr.GetInstance.DisplayPCLScreen(CLRSessionMgr.PCLSCREENS.Home)
            Btn_Ok1.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occurred @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub frmPCLHelp_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '1.1 Added for line break to avoid unwanted characters
        'Archana Chandramathi
        '13C Chilled Food project 
        txtHelpText.Text = "Check that the correct Clearance Label" + vbCrLf + "stationary is installed in the mobile" + vbCrLf + _
                           "printer before attempting to print any" + vbCrLf + "labels. Clearance Labels can only be " + vbCrLf + _
                           "printed on the mobile printer."
    End Sub
End Class