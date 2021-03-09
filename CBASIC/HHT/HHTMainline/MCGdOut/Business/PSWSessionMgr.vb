'''***************************************************************
''' <FileName>PSWSessionMgr.vb</FileName>
''' <summary>
''' The Goods out feature class which will intialise all the 
''' parameters with respect to Pharmacy special Waste.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
Public Class PSWSessionMgr
    'Declaring the Form objects
    Private m_frmPSWDespatch As frmPSWDespatch
    Private m_frmPSWSummary As frmPSWSummary
    Private m_frmPSWUODScreen As frmPSWUODScreen

    'Declaring the objects that store items in a list
    Private Shared m_objPSWSessionMgr As PSWSessionMgr = Nothing

    'Stores the Transaction Data held within the session
    Private m_UODNumber As String = Nothing

    ''' <summary>
    ''' Constructor initiates data 
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()
        Try
            'Instantiate all the objects required
            ' Me.StartSession()
        Catch ex As Exception
            'Handle Goods out Init Exception here.
            Me.EndSession()
        End Try


    End Sub
    ''' <summary>
    ''' Shared Function to return the object of the class singleton
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As PSWSessionMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.PHSLWT
        If m_objPSWSessionMgr Is Nothing Then
            m_objPSWSessionMgr = New PSWSessionMgr
        End If
        Return m_objPSWSessionMgr
    End Function
    ''' <summary>
    ''' Initialises the Goods out Session 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function StartSession() As Boolean
#If RF Then
        Dim objUOS As UOSRecord = Nothing
        objUOS.strIsListType = "G"
        If Not objAppContainer.objExportDataManager.CreateUOS(objUOS) Then
            objUOS = Nothing
            Return False
        End If
        objUOS = Nothing
#End If
        'All the Goods out related forms are instantiated.
        m_frmPSWDespatch = New frmPSWDespatch
        m_frmPSWSummary = New frmPSWSummary
        m_frmPSWUODScreen = New frmPSWUODScreen
        Return True
    End Function
    Public Sub UpdateStatusBar()
        Try
            m_frmPSWDespatch.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_frmPSWSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_frmPSWUODScreen.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occured @ :" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' The Method handles scan the scan data returned form the barcode scanner.
    ''' This method implements the business logic to populate the data to the corresponding
    ''' UI element after validation.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="Type"></param>
    ''' <remarks></remarks>
    Public Sub HandleScanData(ByVal strBarcode As String, ByVal Type As BCType)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered HandleScanData of  PSWSessionMgr", Logger.LogLevel.INFO)
        PSWSessionMgr.GetInstance().m_frmPSWUODScreen.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        Try
            Select Case Type
                Case BCType.UOD
                    PSWSessionMgr.GetInstance().ProcessScanUOD(strBarcode, False)
                Case BCType.UODManualEntry
                    PSWSessionMgr.GetInstance().ProcessScanUOD(strBarcode, True)
            End Select
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in HandleScanData of  PSWSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
#If RF Then
            If (Not m_frmPSWUODScreen Is Nothing) AndAlso (Not m_frmPSWUODScreen.IsDisposed) Then
                PSWSessionMgr.GetInstance().m_frmPSWUODScreen.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End If
#ElseIf NRF Then
             PSWSessionMgr.GetInstance().m_frmPSWUODScreen.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#End If
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit HandleScanData of  PSWSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Process the hand keyed or scanned UOD and store it in class Members
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="bIsManual"></param>
    ''' <remarks></remarks>
    Private Sub ProcessScanUOD(ByVal strBarcode As String, ByVal bIsManual As Boolean)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered ProcessScanUOD of  PSWSessionMgr", Logger.LogLevel.INFO)
        Dim bErrorMessagePrompted As Boolean = False
        Try
            'Check if the UOD is valid or not
            If (objAppContainer.objHelper.ValidateUOD(strBarcode, bErrorMessagePrompted)) Then
                objAppContainer.objLogger.WriteAppLog("PSWSessionMgr::ProcessScanItem:UOD scanned= " & strBarcode, Logger.LogLevel.RELEASE)
                'Set the UOD data to the class member
                PSWSessionMgr.GetInstance().SetUOD(strBarcode)
                'Go to the next screen
                PSWSessionMgr.GetInstance().DisplayPSWScreen(PSWSCREENS.Despatch)
            Else
                If Not bErrorMessagePrompted Then
                    If bIsManual Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M13"), _
                                 "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                  MessageBoxDefaultButton.Button1)
                    Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), _
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                MessageBoxDefaultButton.Button1)
                    End If
                End If

            End If
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in ProcessScanUOD of  PSWSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
#If RF Then
            'This code block clears the barcode text from the text box when there is a connection loss 
            'and connection regained during the retry attempts
            If ex.Message = Macros.CONNECTION_REGAINED Then
                If ((Not m_frmPSWUODScreen Is Nothing) AndAlso (Not m_frmPSWUODScreen.IsDisposed)) Then
                    m_frmPSWUODScreen.txtBarcode.Text = ""
                End If
            End If
#End If
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit ProcessScanUOD of  PSWSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Generates the Export data using Data Access Layer API
    ''' </summary> 
    ''' <remarks></remarks>
#If NRF Then
        Public Sub GenerateExportData()
#ElseIf RF Then
    Public Function GenerateExportData() As Boolean
#End If
#If NRF Then
  'Creating a variable of the type CreateUOS
        Dim objUOS As UOSRecord = Nothing
#End If
        Dim objUOA As UOARecord = Nothing
        Dim objUOX As UOXRecord = Nothing
        Dim totalPrice As Double = 0.0
        m_frmPSWDespatch.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing UOS")
        'Handling Autologoff scenario for no data
        If m_UODNumber = Nothing Then
#If NRF Then
            Exit Sub
#ElseIf RF Then
            Exit Function
#End If
        End If

        'TODO : Call UOS in DAL
#If NRF Then
        objUOS.strIsListType = "G"
        objAppContainer.objExportDataManager.CreateUOS(objUOS)
        '#ElseIf RF Then
        '        If Not objAppContainer.objExportDataManager.CreateUOS(objUOS) Then
        '            Return False
        '        End If
#End If
        objAppContainer.objLogger.WriteAppLog("PSW : UOS written successfully ", Logger.LogLevel.RELEASE)
        'Variable to define a sequence number
        m_frmPSWDespatch.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing UOX")
        'Creating a varible of the type CreateUOX to be sent to DAL
        objUOX.strisListType = "G"
        objUOX.strUOD = m_UODNumber
        'TODO : Status can be cancel
        objUOX.strIsStatus = "D"
        objUOX.strItemCount = ""
        objUOX.strIsStockFigure = "Y" 'Hardcoded to Y as per the RF Goods out Credit claim DD
        'TODO : Get the data from the DAL and then add the supply route to this variable
        objUOX.strSupplierRoute = ""
        objUOX.strDisplayLoc = "" 'Empty as per PPC
        objUOX.strBCname = ""
        objUOX.strBCdesc = ""
        objUOX.strRecall = ""
        objUOX.strAuthCode = ""
        objUOX.strSupplier = ""
        objUOX.strMethod = WorkflowMgr.GetInstance().MethodOfReturn
        objUOX.strCarrier = WorkflowMgr.GetInstance().Carrier
        objUOX.strNumbird = "" 'Hardcoded as empty always
        objUOX.strNumReason = WorkflowMgr.GetInstance().ReasonCodeNum
        objUOX.strRecStore = ""
        objUOX.strDestination = WorkflowMgr.GetInstance().Destination
        'Depends on the supplier route if supplier route is C then warehouse is C else its R
        objUOX.strWroute = ""
        objUOX.strIsUODType = WorkflowMgr.GetInstance().UODType
        If objUOX.strIsUODType = "03" Or objUOX.strIsUODType = "3" Then
            objUOX.strReasonDamage = "02"
        Else
            objUOX.strReasonDamage = ""
        End If
        'TODO : Call UOX in DAL
#If NRF Then
          objAppContainer.objExportDataManager.CreateUOX(objUOX)
#ElseIf RF Then
        Try
            If Not objAppContainer.objExportDataManager.CreateUOX(objUOX) Then
                Return False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occured in Generate Export data - " + _
                                                               "PSWSession Manager", Logger.LogLevel.RELEASE)
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Or ex.Message = Macros.CONNECTION_REGAINED Then
                Throw ex
            End If
        Finally
            objUOX = Nothing
        End Try
#End If

        objAppContainer.objLogger.WriteAppLog("PSW : UOX written successfully ", Logger.LogLevel.RELEASE)
        'Update the UOD collection
        objAppContainer.objUODCollection.Add(m_UODNumber)
#If NRF Then
  End Sub
#ElseIf RF Then
        Return True
    End Function
#End If

    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by PSW session Manager.
    ''' </summary>
    ''' <returns>True if terminate is sucess else False</returns>
    ''' <remarks></remarks>
    Public Function EndSession()
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered EndSession of  PSWSessionMgr", Logger.LogLevel.INFO)
        Try
            'Save and data and perform all Exit Operations.
            'Close and Dispose all forms.
            m_frmPSWDespatch.Close()
            m_frmPSWDespatch.Dispose()

            m_frmPSWSummary.Close()
            m_frmPSWSummary.Dispose()

            m_frmPSWUODScreen.Close()
            m_frmPSWUODScreen.Dispose()

            'Release all objects and Set to nothig.
            m_UODNumber = Nothing
            'Closing the class object
            m_objPSWSessionMgr = Nothing

        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in EndSession of  PSWSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit EndSession of  PSWSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function

    ''' <summary>
    ''' Screen Display method for Goods Out. 
    ''' All Goods Out sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName">Enum SMSCREENS</param>
    ''' <returns>True if display is sucess else False</returns>
    ''' <remarks></remarks>
    Public Function DisplayPSWScreen(ByVal ScreenName As PSWSCREENS)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayPSWScreen of  PSWSessionMgr", Logger.LogLevel.INFO)
        Try
            Select Case ScreenName
                Case PSWSCREENS.ScanUOD
                    m_frmPSWUODScreen.Invoke(New EventHandler(AddressOf DisplayScanUOD))
                Case PSWSCREENS.Despatch
                    m_frmPSWDespatch.Invoke(New EventHandler(AddressOf DisplayDispatchScreen))
                Case PSWSCREENS.Summary
                    m_frmPSWSummary.Invoke(New EventHandler(AddressOf DisplaySummaryScreen))
            End Select
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayPSWScreen of  PSWSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayPSWScreen of  PSWSessionMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Populate and display the Scan UOD Label Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub DisplayScanUOD(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayScanUOD of  PSWSessionMgr", Logger.LogLevel.INFO)
        Try
            'Populating the controls
            With m_frmPSWUODScreen
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                .pnScanLabelColourIndicator.BackColor = WorkflowMgr.GetInstance().FetchLabelColourCode
                .lblScanColour.Text = "Scan " & WorkflowMgr.GetInstance().Labelcolour & " Label"
                .Visible = True
                .Refresh()

            End With
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayScanUOD of  PSWSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmPSWUODScreen.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayScanUOD of  PSWSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Populate and display the Despatch Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub DisplayDispatchScreen(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplayDispatchScreen of  PSWSessionMgr", Logger.LogLevel.INFO)
        Try
            With m_frmPSWDespatch
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                .lblUODData.Text = m_UODNumber
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog("Exception occured in DisplayDispatchScreen of  PSWSessionMgr. Exception is: " _
                                                             + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmPSWDespatch.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayDispatchScreen of  PSWSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Populate and display the Summary Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub DisplaySummaryScreen(ByVal o As Object, ByVal e As EventArgs)
        'Write to Log INFO File while entry
        objAppContainer.objLogger.WriteAppLog("Entered DisplaySummaryScreen of  PSWSessionMgr", Logger.LogLevel.INFO)
        Try
            With m_frmPSWSummary
                .lblTitle.Text = WorkflowMgr.GetInstance().Title
                .lblUODData.Text = m_UODNumber
#If RF Then
                 'MC55 RF text - Added text
                .lblCompleteInstruction.Text = "Place the Credit Claim page(s) in UOD"
#End If
                .Visible = True
                .Refresh()
            End With
        Catch ex As Exception
            'Writing the Error to the Log File
            objAppContainer.objLogger.WriteAppLog(ex.ToString() + " occured in DisplaySummaryScreen of  PSWSessionMgr. Exception is: " _
                                                              + ex.StackTrace.ToString, Logger.LogLevel.RELEASE)
        Finally
            m_frmPSWSummary.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplaySummaryScreen of  PSWSessionMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Store the UOD Number
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetUOD(ByVal sUODNumber As String)
        m_UODNumber = sUODNumber
    End Sub
    ''' <summary>
    ''' Enum Class that defines all screens for Shelf Monitor module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum PSWSCREENS
        ScanUOD
        Despatch
        Summary
    End Enum
End Class
