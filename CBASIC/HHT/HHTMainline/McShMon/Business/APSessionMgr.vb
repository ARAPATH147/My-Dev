Imports System.IO
''' ***************************************************************************
''' <fileName>APSessionMgr.vb</fileName>
''' <summary>To assign printer for the session and test.
''' Implements all business logic and GUI navigation for Print SEL. 
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Dec-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''*****************************************************************************
Public Class APSessionMgr
    Private m_APHome As frmAPHome
    Private m_APUtilities As frmAPLocalPrinterUtilities
    Private Shared m_APSessionMgr As APSessionMgr = Nothing
    Private Sub New()
        
    End Sub
    ''' <summary>
    ''' Functions for getting the object instance for the PSSessionMgr. 
    ''' Use this method to get the object refernce for the Singleton PSSessionMgr.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of PSSessionMgr Class</remarks>
    Public Shared Function GetInstance() As APSessionMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.ASSIGNPRINTER
        If m_APSessionMgr Is Nothing Then
            m_APSessionMgr = New APSessionMgr()
            Return m_APSessionMgr
        Else
            Return m_APSessionMgr
        End If
    End Function
    ''' <summary>
    ''' Initialises the Print SEL Session 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartSession()
        Try
            'Do all module related initalisation here
            m_APHome = New frmAPHome()
            m_APUtilities = New frmAPLocalPrinterUtilities()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at APSessionMgr: Session cannot be started" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit APSessionMgr StartSession", Logger.LogLevel.RELEASE)
    End Sub
#If RF Then
    ''' <summary>
    ''' Updates the Status bar of all the forms in the session manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBarMessage()
        Try
            m_APHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM)
            m_APUtilities.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured, Trace: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End If
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by PSSessionMgr.
    ''' </summary>
    ''' <returns>True if terminate is sucess else False</returns>
    ''' <remarks></remarks>
    Public Function EndSession() As Boolean
        'Save and data and perform all Exit Operations.
        'Close and Dispose all forms.
        Try
            'write export data for batch mode operation.
            m_APHome.Dispose()
            m_APUtilities.Dispose()
            m_APHome = Nothing
            m_APUtilities = Nothing
            m_APSessionMgr = Nothing
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at APSessionMgr EndSession failure" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
        objAppContainer.objLogger.WriteAppLog("Exit APSessionMgr EndSession", Logger.LogLevel.RELEASE)
    End Function
    ''' <summary>
    ''' Screen Display method for Assign Printer.
    ''' </summary>
    ''' <param name="ScreenName">Enum APSCREENS</param>
    ''' <returns>True if display is success else False</returns>
    ''' <remarks></remarks>
    Public Function DisplayAPScreen(ByVal ScreenName As APSCREENS)
        'Invoke method for other displaying the screens
        Try
            Select Case ScreenName
                Case APSCREENS.Home
                    m_APHome.Invoke(New EventHandler(AddressOf DisplayAPHome))
                Case APSCREENS.Utilities
                    m_APUtilities.Invoke(New EventHandler(AddressOf DisplayUtilities))
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at APSessionMgr Display Screen" + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' To display the scan screen.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayAPHome(ByVal o As Object, ByVal e As EventArgs)
        Dim iCount As Integer = 0
        With m_APHome
            'Set current printer
            .lblCurPrinter.Text = objAppContainer.strCurrentPrinter
            .lstAvailablePrinter.Items.Clear()
            .lstAvailablePrinter.Columns.Clear()
            'Add view list header.
            .lstAvailablePrinter.Columns.Add("Printer List", 162 * objAppContainer.iOffSet, HorizontalAlignment.Left)
            'create list item
            If MobilePrintSessionManager.GetInstance() IsNot Nothing Then
                Try
                    If MobilePrintSessionManager.GetInstance().MobilePrinterStatus Then
                        objAppContainer.bMobilePrinterAttachedAtSignon = True
                        objAppContainer.strCurrentPrinter = "1     Mobile Printer"
                        .lblCurPrinter.Text = objAppContainer.strCurrentPrinter
                        objAppContainer.strPrintFlag = Macros.PRINT_LOCAL
                        .lstAvailablePrinter.Items.Add(New ListViewItem("1     Mobile Printer"))
                    Else
                        While iCount < objAppContainer.aPrintNos.Length And Not _
                    objAppContainer.aPrintNos(iCount).ToString().Equals("X")
                            If objAppContainer.aPrinterList(iCount).Trim IsNot Nothing Then
                                .lstAvailablePrinter.Items.Add(New ListViewItem( _
                                                               objAppContainer.aPrintNos(iCount) & "     " _
                                                               & objAppContainer.aPrinterList(iCount).TrimStart(vbCrLf)))
                            Else
                                .lstAvailablePrinter.Items.Add(New ListViewItem( _
                                                                   objAppContainer.aPrintNos(iCount) & "     " _
                                                                   & "Location Unknown"))
                            End If
                            iCount += 1
                        End While
                        objAppContainer.bMobilePrinterAttachedAtSignon = False
                        .lblCurPrinter.Text = objAppContainer.strCurrentPrinter
                    End If
                Catch ex As Exception
                    objAppContainer.objLogger.WriteAppLog("APSessionMgr - Exception in full DisplayAPHome - getting printer status", _
                                                          Logger.LogLevel.RELEASE)
                    'To handle the scenario where the mobie printer is disconnected in between a session.
                    While iCount < objAppContainer.aPrintNos.Length And Not _
                    objAppContainer.aPrintNos(iCount).ToString().Equals("X")
                        If objAppContainer.aPrinterList(iCount).Trim IsNot Nothing Then
                            .lstAvailablePrinter.Items.Add(New ListViewItem( _
                                                           objAppContainer.aPrintNos(iCount) & "     " _
                                                           & objAppContainer.aPrinterList(iCount).TrimStart(vbCrLf)))
                        Else
                            .lstAvailablePrinter.Items.Add(New ListViewItem( _
                                                           objAppContainer.aPrintNos(iCount) & "     " _
                                                           & "Location Unknown"))
                        End If
                        iCount += 1
                    End While
                    If objAppContainer.bMobilePrinterAttachedAtSignon Then
                        objAppContainer.strCurrentPrinter = objAppContainer.aPrintNos(0).ToString() + "     " + _
                                                                                  objAppContainer.aPrinterList(0).ToString()
                        .lblCurPrinter.Text = objAppContainer.strCurrentPrinter
                        objAppContainer.strPrintFlag = Macros.PRINT_BATCH
                    End If
                    objAppContainer.bMobilePrinterAttachedAtSignon = False

                End Try
            ElseIf objAppContainer.aPrintNos.Length > 1 Then
                While iCount < objAppContainer.aPrintNos.Length And Not _
                    objAppContainer.aPrintNos(iCount).ToString().Equals("X")
                    If objAppContainer.aPrinterList(iCount).Trim IsNot Nothing Then
                        .lstAvailablePrinter.Items.Add(New ListViewItem( _
                                                       objAppContainer.aPrintNos(iCount) & "     " _
                                                       & objAppContainer.aPrinterList(iCount).TrimStart(vbCrLf)))
                    Else
                        .lstAvailablePrinter.Items.Add(New ListViewItem( _
                                                           objAppContainer.aPrintNos(iCount) & "     " _
                                                           & "Location Unknown"))
                    End If
                    iCount += 1
                End While
            Else
                EndSession()
                Exit Sub
            End If
            .lstAvailablePrinter.Visible = True
            .Visible = True
            'Sets the store id and active data time to the status bar
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' To display item details screen.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayUtilities(ByVal o As Object, ByVal e As EventArgs)
        Dim iCount As Integer = 0
        Dim strBarcode As String = ""
        Try
            With m_APUtilities
                .pbSendFonts.Enabled = True
                .pbTestPrint.Enabled = True
                .pbTestClearancePrint.Enabled = True
                .Visible = True
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("APSessionMgr - Exception in full DisplayPSItemDetails", _
                                                  Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Function to trim Null character if any present inthe string.
    ''' </summary>
    ''' <param name="strText"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function NTrim(ByVal strText As String) As String
        Dim Pos As Long
        Dim strNewText As String = ""
        Try
            Pos = InStr(strText, vbNullChar)

            If Pos > 0 Then
                strNewText = Left$(strText, Pos - 1)
            Else
                strNewText = strText
            End If
        Catch ex As Exception
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Error in trimming null character" _
                                                                & ex.Message.ToString(), _
                                                                Logger.LogLevel.RELEASE)
        End Try
        NTrim = strNewText
    End Function
    ''' <summary>
    ''' Processes the selction of Print button click
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SendFonts()
        Dim aFiles As String()
        Dim strFontFile As String
        Dim iCnt As Integer
        Dim iNoOfFonts As Integer

        Try
            iCnt = 0
            m_APUtilities.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "PLEASE WAIT...")
            aFiles = Directory.GetFiles(Macros.FONTS_DIRECTORY, "@*.cpf")

            iNoOfFonts = aFiles.GetUpperBound(0) + 1

            For Each strFontFile In aFiles
                'Dim input As New FileStream("\Application\@Ar06pt.cpf", _
                Dim input As New FileStream(strFontFile, _
                                            FileMode.Open, _
                                            FileAccess.Read, _
                                            FileShare.None)

                Dim bReader As New BinaryReader(input)

                'Increment the counter.
                iCnt += 1
                m_APUtilities.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, _
                                                      "PLEASE WAIT...  Font: " & iCnt.ToString & " of " & iNoOfFonts.ToString)
                'Now initiate the font transmission
                MobilePrintSessionManager.GetInstance.SendFonts(bReader)

                'Wait 10 seconds before send next font to provide enough time to commit the font file.
                System.Threading.Thread.Sleep(10000)
            Next

            If aFiles.Length > 0 Then
                MessageBox.Show("Fonts installed successfully." + vbCrLf + vbCrLf + _
                                "Please power printer OFF and back ON and " + _
                                "then print a test Label", "Printer Information", _
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            Else
                MessageBox.Show("Font files are missing." + vbCrLf + vbCrLf + _
                                "Please contact the Help Desk", "Printer Error", _
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            End If
        Catch ex As Exception
            MessageBox.Show("Exception: " & ex.Message, "Printer Error", _
                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            If Not ex.Message = "EndOfStreamException" Then
                MessageBox.Show("Exception: " & ex.Message + vbCrLf + vbCrLf + _
                                "Problem installing fonts. Please contact the Help Desk", "Printer Error", _
                                MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            End If
        End Try
        'Set status bar text.
        m_APUtilities.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
    End Sub
    ''' <summary>
    ''' To test SEL printing.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub TestPrint()
        Try
            Dim sTestLabel As String = _
                       "! 0 200 200 252 1" + vbCrLf + _
                       "CONTRAST 0" + vbCrLf + _
                       "TONE 0" + vbCrLf + _
                       "SPEED 3" + vbCrLf + _
                       "PAGE-WIDTH 396" + vbCrLf + _
                       "COUNTRY UK" + vbCrLf + _
                       "BAR-SENSE" + vbCrLf + _
                       "SET-TOF 20" + vbCrLf + _
                       "PRESENT-AT 35 2" + vbCrLf + _
                       "T 4 0 77 0 TEST PRINT" + vbCrLf + _
                       "IL 0 0 0 44 394" + vbCrLf + _
                       "T 4 0 30 42 £" + vbCrLf + _
                       "T @Ar20Bpt.cpf 0 60 42 8.21" + vbCrLf + _
                       "T @Ar08pt.cpf 0 60 95 Print Test" + vbCrLf + _
                       "T @Ar08pt.cpf 0 60 120 Testing Item" + vbCrLf + _
                       "T @Ar08pt.cpf 0 60 145 500ml" + vbCrLf + _
                       "T @Ar08pt.cpf 0 60 168 42.9p per 100ml" + vbCrLf + _
                       "T @Ar06pt.cpf 0 60 234 50-51-541 E" + vbCrLf + _
                       "T @Ar06pt.cpf 0 313 234 MS" + vbCrLf + _
                       "B I2OF5 2 1 30 60 202 505154000824" + vbCrLf + _
                       "T 5 3 30 140 *" + vbCrLf + _
                       "FORM" + vbCrLf + _
                       "PRINT" + vbCrLf
            'print the test label.
            MobilePrintSessionManager.GetInstance.TestPrint(sTestLabel)
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("APSessionMgr - Exception in TestPrint", _
                                                  Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To test Clearance label printing.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub TestClearancePrint()
        Try
            Dim sTestLabel As String = _
                       "! 0 200 200 252 1" + vbCrLf + _
                       "CONTRAST 0" + vbCrLf + _
                       "TONE 0" + vbCrLf + _
                       "SPEED 3" + vbCrLf + _
                       "PAGE-WIDTH 396" + vbCrLf + _
                       "COUNTRY UK" + vbCrLf + _
                       "BAR-SENSE" + vbCrLf + _
                       "SET-TOF 20" + vbCrLf + _
                       "PRESENT-AT 35 2" + vbCrLf + _
                       "T @Ar20Bpt.cpf 0 95 62 8" + vbCrLf + _
                       "T 4 0 125 62 p" + vbCrLf + _
                       "T @Ar07pt.cpf 0 75 120 TEST CLEARANCE LABEL" + vbCrLf + _
                       "T @Ar06pt.cpf 0 75 180 50-51-541 E" + vbCrLf + _
                       "B I2OF5 1 1 30 75 150 8270142342000528" + vbCrLf + _
                       "T @Ar06pt.cpf 0 290 180 CIP" + vbCrLf + _
                       "FORM" + vbCrLf + _
                       "PRINT" + vbCrLf
            'print the test label.
            MobilePrintSessionManager.GetInstance.TestPrint(sTestLabel)
        Catch ex As Exception
#If RF Then
            If ex.Message = Macros.CONNECTION_LOST_EXCEPTION_MESSAGE Then
                Throw ex
            End If
#End If
            objAppContainer.objLogger.WriteAppLog("APSessionMgr - Exception in TestClearancePrint", _
                                                  Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Enum Class that defines all screens for Print SEL module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum APSCREENS
        Home
        Utilities
    End Enum
End Class