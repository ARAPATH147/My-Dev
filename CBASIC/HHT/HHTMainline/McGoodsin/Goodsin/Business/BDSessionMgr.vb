Imports System.Globalization
'''***************************************************************
''' <FileName> BDSessionMgr.vb</FileName>
''' <summary>
''' The Book In Delivery Container Class.
''' Implements all business logic and GUI navigation for Book In Delivery.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
'''* Modification Log 
'''******************************************************************************* 
'''* No:       Author            Date            Description 
'''* 1.1     Christopher Kitto  09/04/2015   Modified as part of DALLAS project
'''                  (CK)                  to pass an extra argument to the function
'''                                        GetBookInDeliverySummary(). Modified class
'''                                        scancount for keeping the Dallas UOD count.
'''                                        Amended the function PopulateExpectedList()
'''                                        to update the Dallas summary in Initial 
'''                                        Summary screen. Modified functions confirmbatch(),
'''                                        confirmdelivery() and DisplayDrvrConfirmation(). 
'''          Kiran Krishnan    06/05/2015  Modified to include the barcode processing
'''                 (KK)                   changes for Dallas UOD                         
'''********************************************************************************


Public Class BDSessionMgr
    Private Shared objBDSessionMgr As BDSessionMgr
    Private m_BDUODHome As frmBDUODHome
    Private m_BDUODInitialSummary As frmBDUODInitialSummary
    Private m_BDUODScan As frmBDUODScan
    Private m_BDUODDrvrConfrmation As frmBDUODDrvrConfrmation
    Private m_BDUODFinalDrvrConfirmation As frmBDUODFinalDrvrConfirmation
    Private m_BDUODSummary As frmBDUODSummary
    Private bMisdirectReturn As Boolean = False
    'V1.1 KK
    'Added new variable for Dallas Misdirect UOD return
    Private m_bDalMisdirectReturn As Boolean = False
    Private m_MsgBox As MsgBx
    Private strUODType As String = ""
    Private strNumber As String = ""
    'Variable for driver badge id
    Private strBadgeID As String = ""
    'Use new data source instance of from Appcontainer -----------------
    Private m_UODInfo As UODInfo = Nothing
    Private m_UODParentDetails As UODParentDetails = Nothing
    Private m_objSessionScanCount As scanCount = Nothing
    'Private m_objBatchCount As scanCount = Nothing
    Private m_ItemList As ArrayList = Nothing
    ' V1.1 - CK
    ' Added new arraylist m_DALLASItemList for holding details of Dallas UOD, as part of Dallas project
    Private m_DalUODInfo As GIValueHolder.DallasScanDetail = Nothing
    Private m_DALLASItemList As ArrayList = Nothing
    Public m_arrListBatch As ArrayList = Nothing
    'V1.1 - KK
    'Added new arraylist m_arrDalScanBatch for storing full details of scanned Dallas UOD 
    Private m_arrDalScanBatch As ArrayList = Nothing
    Public m_arrScannedUOD As ArrayList = Nothing
    ' V1.1 - CK
    ' Added new arraylist arrDallasScannedUOD for holding details of scanned Dallas UODs.
    Public arrDallasScannedUOD As ArrayList = Nothing
    Public m_arrDallasConfirmedUOD As ArrayList = Nothing
    ' V1.1 - CK
    ' Added new arraylist m_arrDallasMisdirectRScanned for holding the barcodes of scanned
    ' misdirect returned barcodes.
    Private m_arrDallasMisdirectRScanned As ArrayList = Nothing
    Public m_arrDriverDetails As ArrayList = Nothing
    Public m_arrScannedUODBatch As ArrayList = Nothing
    Public m_arrBookInDetails As ArrayList = Nothing
    Public m_arrParentDetails As ArrayList = Nothing
    'new varialbles for partial- reny
    Public m_arrPartialDetails As ArrayList = Nothing
    Public m_arrBatchPartialDetails As ArrayList = Nothing
    Public m_arrDallasBankedUOD As ArrayList = Nothing

    'For final screen
    Private m_iFinalExpectedCrateCount As Integer = 0
    Private m_iFinalOutCrateCount As Integer = 0
    'CR for Future UOD
    Private m_iFinalFutureCrateCount As Integer = 0
    Public m_arrMisdirectReturnUODBatch As ArrayList = Nothing
    Public m_arrMisdirectReturnUOD As ArrayList = Nothing
    ''ISSUE
    Private m_objPartialDollyScanned As PartialDollyScanned
    Private m_bPartialDollyScanned As Boolean = False
    Private m_arrPartialDollyChildCount As ArrayList = Nothing
    Private m_arrBatchParentDetails As ArrayList = Nothing
    Public bIsFinished As Boolean = False
    Public bIsQuit As Boolean = False
    Public bIsInHours As Boolean = True
    Public bIsUODCombination As Boolean = False
    Private bIsParent As Boolean = False
    'Variable Holds the expected and outstanding count
    Private m_objExpected As scanCount = Nothing
    Private m_objOutstanding As scanCount = Nothing
    ' V1.1 - KK
    ' Added m_objFuture to obtain the count of future Dallas UOD
    Private m_objFuture As scanCount = Nothing
    'Holds the outstanding and Expected count for scanned items
    Private m_objExpectedScanned As scanCount = Nothing
    Private m_objOutstandingScanned As scanCount = Nothing
    'CR for Future UOD
    Private m_objFutureBatch As scanCount = Nothing
    Private m_objFutureScanned As scanCount = Nothing
    'Holds the data corresponding to the batch 
    Private m_objExpectedBatch As scanCount = Nothing
    Private m_objOutStandingBatch As scanCount = Nothing
    'Holds data cooresponding to misdirected deliveries
    Private m_objMisdirectedBatch As scanCount = Nothing
    Private m_objMisdirectedScanned As scanCount = Nothing
    Private m_objMisdirectedReturnBatch As scanCount = Nothing
    Private m_objMisdirectedReturnScanned As scanCount = Nothing
    'V1.1 - KK
    'Added m_objDallasBatch to obtain the details of Dallas UOD's for sending DAR
    'Private m_objDallasBatch As scanCount = Nothing

    Private m_objExpectedRollback As scanCount = Nothing
    Private m_objOutstandingRollback As scanCount = Nothing
    'Variable to hold GIT Note
    Public bGITNote As Boolean = False
    'Holds data for batch count
    Private m_iCount As Integer = 0
    ' V1.1 - CK
    ' Integer variables m_iExpectedDalFinalCount, m_iOutstandingDalFinalCount
    ' m_iFutureDalFinalCount & m_iMisdirectReturnedDalFinalCount for holding 
    ' the count of scanned Dallas UODs to display on the Final driver confirmation screen
    Private m_iExpectedDalFinalCount As Integer = 0
    Private m_iOutstandingDalFinalCount As Integer = 0
    Private m_iFutureDalFinalCount As Integer = 0
    Private m_iNotOnFileDalFinalCount As Integer = 0
    Private m_iMisdirectReturnedDalFinalCount As Integer = 0
    'CR variable to check if the Dolly is Partially Booked in or Not
    Public bPartialBkd As Boolean = False
    Private m_iUODChildCount As Integer = 0
    Private m_arrChildBkdDolly As ArrayList = Nothing
    Private strNotOnFileBarcode As String = ""
    Public bReconnected As Boolean = True
    ' V1.1 - KK 
    ' Dallas Misdirect & Not on file Variables
    Private m_cDallasMisdirectBarcode As String = ""
    Private m_cDallasNotOnFileBarcode As String = ""

    ''' <summary>
    '''  Initialises the Book In Delivery Session 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartSession()
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
            m_UODInfo = New UODInfo
            bIsInHours = True 'Get the Config value-----------------------------------------------------------------
            m_objSessionScanCount = New scanCount
            m_BDUODHome = New frmBDUODHome
            m_BDUODInitialSummary = New frmBDUODInitialSummary
            m_BDUODScan = New frmBDUODScan
            m_BDUODDrvrConfrmation = New frmBDUODDrvrConfrmation
            m_BDUODFinalDrvrConfirmation = New frmBDUODFinalDrvrConfirmation
            m_BDUODSummary = New frmBDUODSummary
#If RF Then
            objAppContainer.bReconnectSuccess = False
            m_MsgBox = New MsgBx
#End If


            m_ItemList = New ArrayList

            ' V1.1 - CK
            'New arraylist m_DALLASItemList for holding details of Dallas UOD
            m_DALLASItemList = New ArrayList

            m_arrListBatch = New ArrayList
            m_arrDalScanBatch = New ArrayList
            'm_arrDriverDetails = New ArrayList
            m_arrBookInDetails = New ArrayList
            m_arrParentDetails = New ArrayList
            m_arrBatchParentDetails = New ArrayList
            m_arrScannedUOD = New ArrayList
            ' V1.1 - CK
            ' New arraylist arrDallasScannedUOD for holding details of
            ' scanned Dallas UODs
            arrDallasScannedUOD = New ArrayList
            m_arrDallasBankedUOD = New ArrayList
            m_arrDallasConfirmedUOD = New ArrayList
            ' V1.1 - CK
            ' Initialising m_arrDallasMisdirectRScanned
            m_arrDallasMisdirectRScanned = New ArrayList
            m_arrScannedUODBatch = New ArrayList
            m_arrMisdirectReturnUODBatch = New ArrayList
            m_arrMisdirectReturnUOD = New ArrayList
            'Partial handling
            m_arrPartialDetails = New ArrayList
            m_arrBatchPartialDetails = New ArrayList
            m_objOutstanding = New scanCount
            m_objExpected = New scanCount
            m_objExpectedScanned = New scanCount
            m_objOutstandingScanned = New scanCount
            m_objExpectedBatch = New scanCount
            m_objOutStandingBatch = New scanCount
            'CR for Future UOD
            m_objFutureBatch = New scanCount
            ' V1.1 - KK
            ' Added m_objFuture to obtain the count of future Dallas UOD
            m_objFuture = New scanCount
            m_objFutureScanned = New scanCount
            m_objMisdirectedBatch = New scanCount
            m_objMisdirectedScanned = New scanCount
            m_objExpectedRollback = New scanCount
            m_objOutstandingRollback = New scanCount

            'CR
            ' m_objPartialDollyScanned = New PartialDollyScanned
            m_objMisdirectedReturnBatch = New scanCount
            m_objMisdirectedReturnScanned = New scanCount
            bIsFinished = False
            bIsParent = False
            'CR
            m_iCount = CType(objAppContainer.objConfigValues.BatchSize, Integer)
            'CR
            m_arrPartialDollyChildCount = New ArrayList
            bPartialBkd = False
            m_arrChildBkdDolly = New ArrayList
            m_iUODChildCount = 0
            'To check whether the delivery is for In hours or out of hours.
            If objAppContainer.objConfigValues.ONightDelivery = Macros.Y Then
                bIsInHours = False
            Else
                bIsInHours = True
            End If
            m_BDUODScan.Btn_Finish1.Visible = False
            m_BDUODScan.Btn_Finish1.Enabled = False

            ' V1.1 - CK
            ' Amending the below line to pass an extra parameter - array to hold the detail of Dallas UODs
            ' If objAppContainer.objDataEngine.GetBookInDeliverySummary(m_ItemList) Then

            If objAppContainer.objDataEngine.GetBookInDeliverySummary(m_ItemList, m_DALLASItemList) Then
                Me.DisplayBDScreen(BDSCREENS.BDUODHome)
            Else
                objAppContainer.objLogger.WriteAppLog("Book In Delivery Start Session:Recieved False", Logger.LogLevel.RELEASE)
#If RF Then
                objAppContainer.bReconnectSuccess = False
#End If
            End If
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Start Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Functions for getting the object instance for the BDSessionMgr. 
    ''' Use this method to get the object reference for the Singleton BDSessionMgr.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As BDSessionMgr
        'Setting the Active Module as Book In Delivery
        Try
            objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.BOOKINDELIVERY
            If objBDSessionMgr Is Nothing Then
                objBDSessionMgr = New BDSessionMgr
                Return objBDSessionMgr
            Else
                Return objBDSessionMgr
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at  instantiating an object Book In Delivery Session : " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    ''' The Method handles scan the scan data returned form the barcode scanner.
    ''' This method implements the business logic to populate the data to the corresponding
    ''' UI element after validation.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="Type"></param>
    ''' <remarks></remarks>
    Public Sub HandleScanData(ByVal strBarcode As String, ByVal Type As BCType)
        Try
#If RF Then
            If objAppContainer.bReconnectSuccess Then
                objAppContainer.bReconnectSuccess = False
            End If
#End If
            Dim strValidCode As String = ""
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
            If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINUODSCAN Then
#If RF Then
                objAppContainer.m_ModScreen = AppContainer.ModScreen.FIRSTBATCH
                If objAppContainer.m_PreviousScreen = AppContainer.ModScreen.DRVRBDGESCAN Then
                    objAppContainer.m_ModScreen = AppContainer.ModScreen.DRVRBDGESCAN
                End If
#End If
                HandleUOD(strBarcode, Type)

                'Setting the Active screen as Driver Confirmation screen
            ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BATCHDRVRCONFRM Then
#If RF Then

                objAppContainer.m_ModScreen = AppContainer.ModScreen.DRVRBDGESCAN
                objAppContainer.m_ModScreen = AppContainer.ModScreen.DRVRBDGESCAN
#End If
                If HandleDriverBadge(strBarcode, Type) Then
                    ConfirmBatch()
                End If
            ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.FINALDRVRCNFRM Then
                If HandleDriverBadge(strBarcode, Type) Then
                    ConfirmDelivery()
                End If
            End If

            'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Handle Scan Data: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To Confirm the delivery 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ConfirmDelivery()
        'An object to hold the details of a driver and to send these details to the EPOS controller.
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
            ' V1.1 - KK
            ' Logic included to allow the following piece of code to only run in non DPR stores, DPR stores with non Dallas UOD
            ' count greater than zero
            If Not objAppContainer.bDallasPosReceiptEnabled Or (objAppContainer.bDallasPosReceiptEnabled And bIsUODCombination = True) Then
#If RF Then
                'RF RECONNECT
                objAppContainer.m_ModScreen = AppContainer.ModScreen.POSTFINISH
#End If
                Dim objDriverScanDetails As New GIValueHolder.DriverDetails
                objDriverScanDetails.DriverBadge = strBadgeID
                objDriverScanDetails.ScanDate = Format(DateTime.Now, "yyyyMMdd")
                objDriverScanDetails.ScanTime = Format(DateTime.Now, "HHmmss")
                objDriverScanDetails.ScanLevel = m_UODInfo.UODType
                objDriverScanDetails.BatchRescan = "N"
                If Not bIsInHours Then
                    If bGITNote Then
                        objDriverScanDetails.GITNote = "Y"
                    Else
                        objDriverScanDetails.GITNote = "N"
                    End If
                Else
                    objDriverScanDetails.GITNote = " "
                End If
                bGITNote = False

#If RF Then
                'RECONNECT
                If objAppContainer.objDataEngine.SendDeliveryConfirmation(objDriverScanDetails) Then
                    If Not (m_arrDalScanBatch.Count > 0) Then
                        Me.FinishSession()
                    End If
                End If

#ElseIf NRF Then
                objAppContainer.objDataEngine.SendDeliveryConfirmation(objDriverScanDetails)
                If Not (m_arrDalScanBatch.Count > 0) Then
                    Me.FinishSession()
                End If
#End If
            End If
            ' V1.1 - KK
            ' Logic included to allow the following piece of code to only run in DPR stores, DPR stores with Dallas UOD
            ' scanned and banked count greater than zero
            If objAppContainer.bDallasPosReceiptEnabled And (m_arrDalScanBatch.Count > 0) Then
#If RF Then
                'RF RECONNECT
                objAppContainer.m_ModScreen = AppContainer.ModScreen.POSTFINISH
#End If
#If RF Then
                'RECONNECT
                ' Sending dallas delivery confrimation
                If objAppContainer.objDataEngine.SendDallasDeliveryConfirmation(m_arrDalScanBatch) Then
                    Me.FinishSession()
                End If

#ElseIf NRF Then
                ' Sending dallas delivery confirmation
                If objAppContainer.objDataEngine.SendDallasDeliveryConfirmation(m_arrDalScanBatch) Then
                    Me.FinishSession()
                End If
#End If
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Delivery Confirmation: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
    End Sub
    Private Function HandleDriverBadge(ByVal strBarcode As String, ByVal Type As BCType) As Boolean
        Try
            Dim strValidCode As String = ""
            Select Case Type
                Case BCType.ManualEntry
                    'Validating Driver badge Id.
                    If objAppContainer.objHelper.ValidateDriverBadgeID(strBarcode) Then
#If RF Then
                        'RECONNECT
                        If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BATCHDRVRCONFRM Then
                            objAppContainer.m_ModScreen = AppContainer.ModScreen.DRVRBDGESCAN
                            objAppContainer.m_PreviousScreen = AppContainer.ModScreen.DRVRBDGESCAN
                        Else
                            objAppContainer.m_ModScreen = AppContainer.ModScreen.POSTFINISH
                        End If
#End If
                        strBadgeID = strBarcode
                        Return True
                    ElseIf objAppContainer.objHelper.ValidateUODBarcode(strBarcode, strValidCode) Then
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        'MessageBox.Show(MessageManager.GetInstance().GetMessage("M75"), "Error ", MessageBoxButtons.OK, _
                        'MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                        If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BATCHDRVRCONFRM Then
                            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M101"), "Error ", MessageBoxButtons.OK, _
                                                                                   MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                            m_BDUODDrvrConfrmation.txtProductCode.Text = ""
                            m_BDUODDrvrConfrmation.txtProductCode.Focus()
                        ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.FINALDRVRCNFRM Then
                            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                            If bIsInHours Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M101"), "Error ", MessageBoxButtons.OK, _
                                                                                       MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                            Else
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M101"), "Error ", MessageBoxButtons.OK, _
                                                                                                                 MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                            End If
                            m_BDUODFinalDrvrConfirmation.txtProductCode.Text = ""
                            m_BDUODFinalDrvrConfirmation.txtProductCode.Focus()
                        End If
                    Else
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BATCHDRVRCONFRM Then
                            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M75"), "Error ", MessageBoxButtons.OK, _
                                                                                   MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                            m_BDUODDrvrConfrmation.txtProductCode.Text = ""
                            m_BDUODDrvrConfrmation.txtProductCode.Focus()
                        ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.FINALDRVRCNFRM Then
                            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M75"), "Error ", MessageBoxButtons.OK, _
                                                                                   MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                            m_BDUODFinalDrvrConfirmation.txtProductCode.Text = ""
                            m_BDUODFinalDrvrConfirmation.txtProductCode.Focus()
                        End If
                    End If
                Case BCType.I2O5
                    If objAppContainer.objHelper.ValidateDriverBadgeID(strBarcode) Then
#If RF Then
                        If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BATCHDRVRCONFRM Then
                            objAppContainer.m_ModScreen = AppContainer.ModScreen.DRVRBDGESCAN
                            objAppContainer.m_PreviousScreen = AppContainer.ModScreen.DRVRBDGESCAN
                        Else
                            objAppContainer.m_ModScreen = AppContainer.ModScreen.POSTFINISH
                        End If

#End If
                        strBadgeID = strBarcode
                        Return True
                    ElseIf objAppContainer.objHelper.ValidateUODBarcode(strBarcode, strValidCode) Then
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BATCHDRVRCONFRM Then
                            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M101"), "Error ", MessageBoxButtons.OK, _
                                                                                   MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                            m_BDUODDrvrConfrmation.txtProductCode.Text = ""
                            m_BDUODDrvrConfrmation.txtProductCode.Focus()
                        ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.FINALDRVRCNFRM Then
                            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M101"), "Error ", MessageBoxButtons.OK, _
                                                                                   MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                            m_BDUODFinalDrvrConfirmation.txtProductCode.Text = ""
                            m_BDUODFinalDrvrConfirmation.txtProductCode.Focus()
                        End If
                    Else
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BATCHDRVRCONFRM Then
                            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M75"), "Error ", MessageBoxButtons.OK, _
                                                                                   MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                            m_BDUODDrvrConfrmation.txtProductCode.Text = ""
                            m_BDUODDrvrConfrmation.txtProductCode.Focus()
                        ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.FINALDRVRCNFRM Then
                            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M75"), "Error ", MessageBoxButtons.OK, _
                                                                                   MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                            m_BDUODFinalDrvrConfirmation.txtProductCode.Text = ""
                            m_BDUODFinalDrvrConfirmation.txtProductCode.Focus()
                        End If
                    End If
                Case BCType.EAN

                    If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BATCHDRVRCONFRM Then
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M75"), "Error ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                        m_BDUODDrvrConfrmation.txtProductCode.Text = ""
                        m_BDUODDrvrConfrmation.txtProductCode.Focus()
                    ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.FINALDRVRCNFRM Then
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M75"), "Error ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                        m_BDUODFinalDrvrConfirmation.txtProductCode.Text = ""
                        m_BDUODFinalDrvrConfirmation.txtProductCode.Focus()
                    End If

                Case BCType.CODE128
                    If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BATCHDRVRCONFRM Then
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M101"), "Error ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                        m_BDUODDrvrConfrmation.txtProductCode.Text = ""
                        m_BDUODDrvrConfrmation.txtProductCode.Focus()
                    ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.FINALDRVRCNFRM Then
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M101"), "Error ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                        m_BDUODFinalDrvrConfirmation.txtProductCode.Text = ""
                        m_BDUODFinalDrvrConfirmation.txtProductCode.Focus()
                    End If
                Case Else
                    If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BATCHDRVRCONFRM Then
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M75"), "Error ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                        m_BDUODDrvrConfrmation.txtProductCode.Text = ""
                        m_BDUODDrvrConfrmation.txtProductCode.Focus()
                    ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.FINALDRVRCNFRM Then
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M75"), "Error ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                        m_BDUODFinalDrvrConfirmation.txtProductCode.Text = ""
                        m_BDUODFinalDrvrConfirmation.txtProductCode.Focus()
                    End If
            End Select
            Return False
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Handle Driver Badge Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    ''' Function to handle barcode depending upon the type of barcode.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="Type"></param>
    ''' <remarks></remarks>
    Private Sub HandleUOD(ByVal strBarcode As String, ByVal Type As BCType)
        Try
            Dim strValidCode As String = ""
            Select Case Type
                Case BCType.ManualEntry
                    ' V1.1 - KK
                    ' Code for handling Dallas UOD barcode manual entry
                    If objAppContainer.bDallasPosReceiptEnabled Then
                        If ValidateDallasUOD(strBarcode) Then
                            Exit Sub
                        End If
                    End If

                    If ValidateUOD(strBarcode) Then
                        Exit Sub
                    ElseIf ValidateIST(strBarcode) Then
                        Exit Sub
                    Else
#If RF Then
                        If Not objAppContainer.bCommFailure Then
                            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                            If Not objAppContainer.bReconnectSuccess Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                                                                       MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                            End If
                            m_BDUODScan.txtProductCode.Text = ""
                            m_BDUODScan.txtProductCode.Focus()
                        End If
#ElseIf NRF Then
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                        m_BDUODScan.txtProductCode.Text = ""
                        m_BDUODScan.txtProductCode.Focus()

#End If

                    End If
                Case BCType.CODE128
                    ' V1.1 - KK
                    ' Code for handling Dallas UOD barcode when scanned
                    If objAppContainer.bDallasPosReceiptEnabled Then
                        If strBarcode.StartsWith("0501") And (ValidateDallasUOD(strBarcode) = True) Then
                            Exit Sub
                        End If
                    End If
                    If Not ValidateIST(strBarcode) Then
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                        m_BDUODScan.txtProductCode.Text = ""
                        m_BDUODScan.txtProductCode.Focus()
                    End If

                Case BCType.I2O5
                    If Not ValidateUOD(strBarcode) Then
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
#If RF Then
                        'to exit app if time out occurred
                        If objAppContainer.bTimeOut Then
                            Exit Sub
                        End If
                        If Not objAppContainer.bReconnectSuccess Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                                                             MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                        Else
                            objAppContainer.bReconnectSuccess = False
                        End If
#ElseIf NRF Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                                                             MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)

#End If


                        m_BDUODScan.txtProductCode.Text = ""
                        m_BDUODScan.txtProductCode.Focus()
                    End If
                Case BCType.EAN
                    If objAppContainer.objHelper.ValidateEAN(strBarcode) _
                          OrElse objAppContainer.objHelper.ValidateBootsCode(strBarcode) Then
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M98"), "Alert ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    Else
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                    End If
                    m_BDUODScan.txtProductCode.Text = ""
                    m_BDUODScan.txtProductCode.Focus()
                Case Else
                    objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                    m_BDUODScan.txtProductCode.Text = ""
                    m_BDUODScan.txtProductCode.Focus()
            End Select
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Handle UOD Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Function to Validate UOD Barcode
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ValidateUOD(ByVal strBarcode As String) As Boolean
        Try
            Dim strValidCode As String = ""
            'Flag which is used to hold the information for a valid UOD.If it is returned as true then it means that it is a valid UOD.
            Dim bReturn As Boolean = False
            If objAppContainer.objHelper.ValidateUODBarcode(strBarcode, strValidCode) Then
                'To Check whether the scanned UOD is booked in at the parent level or the Child level
                If CheckPreviousScan(strValidCode) Then
                    objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M65"), "Alert ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) '----------------------
                    m_BDUODScan.txtProductCode.Text = ""
                    m_BDUODScan.txtProductCode.Focus()
                    Return True
                End If
                bReturn = True
                'To check the child level scanning

                If (objAppContainer.objDataEngine.ValidateUODScanned(strValidCode, m_UODInfo, AppContainer.FunctionType.BookIn)) Then
                    If Not m_UODInfo.UODStatus = Macros.UNBOOKED Then
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M65"), "Alert ", MessageBoxButtons.OK, _
                                                                              MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        m_BDUODScan.txtProductCode.Text = ""
                        m_BDUODScan.txtProductCode.Focus()
                        Return True
                    End If
                    If Not m_UODInfo.ImmLicenseNo = Macros.ZEROUOD Then
                        If CheckPreviousScan(m_UODInfo.ImmLicenseNo) Then
                            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M65"), "Alert ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            m_BDUODScan.txtProductCode.Text = ""
                            m_BDUODScan.txtProductCode.Focus()
                            Return True
                        End If
                    End If
                    If m_UODInfo.BOLUOD = Macros.Y Then
                        'To Check whether the Scanned UOD is Dolly or not
                        If strBarcode.Chars(0) = ConfigDataMgr.GetInstance().GetParam(ConfigKey.DOLLYID) Then
                            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M100"), "Alert ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)

                        Else
                            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                            MessageBox.Show("This " + GetContainerName(strValidCode) + " holds opticians items. Please pass the complete UOD onto Boots Opticians. Your stock file has not been updated for these items.", "Alert ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        End If
                    End If
                    'To Process the UOD scanned
                    ProcessUODScan(strValidCode)
                Else
#If RF Then
                    If objAppContainer.bTimeOut Then
                        Return False
                    End If
                    If Not objAppContainer.bCommFailure And Not objAppContainer.bReconnectSuccess Then
                        'if reconnect success then no need to continue processing the UOD Scanned
                        'If objAppContainer.bReconnectSuccess Then
                        '    'resetting Reconnect Success so that application behaves normally.
                        '    objAppContainer.bReconnectSuccess = False
                        '    Return False
                        'End If
#End If
                        'Need to add code for incrementing batch count when a misdirect is accepted----------------
                        'To check whether UOD is previously scanned or not for UOD that are not on file.
                        If Not CheckPreviousScan(strValidCode) Then
                            UODNotOnFile(strValidCode)
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M65"), "Alert ", MessageBoxButtons.OK, _
                                                                                   MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) '----------------------
                            m_BDUODScan.txtProductCode.Text = ""
                            m_BDUODScan.txtProductCode.Focus()
                        End If
#If RF Then
                    End If
#End If
                End If
            Else
                Return False
            End If
            Return bReturn
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Validate UOD Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    '''  Function to validate IST Barcode           
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ValidateIST(ByVal strBarcode As String) As Boolean
        Try
            Dim strValidCode As String = ""
            Dim bReturn As Boolean = False
            If objAppContainer.objHelper.ValidateISTCode(strBarcode, strValidCode) Then
                bReturn = True
                If (objAppContainer.objDataEngine.ValidateUODScanned(strValidCode, m_UODInfo, AppContainer.FunctionType.BookIn)) Then
                    If Not CheckPreviousScan(strValidCode) Then
                        'Fix to check if the status of the UOD scanned is Booked In.
                        If Not m_UODInfo.UODStatus = Macros.UNBOOKED Then
                            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M65"), "Alert ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                            m_BDUODScan.txtProductCode.Text = ""
                            m_BDUODScan.txtProductCode.Focus()
                        ElseIf m_UODInfo.BOLUOD = Macros.Y Then
                            If strBarcode.Chars(0) = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DOLLYID) Then
                                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M100"), "Alert ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                                m_BDUODScan.txtProductCode.Text = ""
                                m_BDUODScan.txtProductCode.Focus()
                            Else
                                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                                MessageBox.Show("This IST holds opticians items. Please pass the complete UOD onto Boots Opticians. Your stock file has not been updated for these items.", "Alert ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                                m_BDUODScan.txtProductCode.Text = ""
                                m_BDUODScan.txtProductCode.Focus()
                            End If
                            ProcessUODScan(strValidCode)
                        Else
                            ProcessUODScan(strValidCode)
                        End If
                    Else
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M65"), "Alert ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) '----------------------
                        m_BDUODScan.txtProductCode.Text = ""
                        m_BDUODScan.txtProductCode.Focus()
                    End If
                Else
                    'Need to add code for incrementing batch count when a misdirect is accepted----------------
                    'To check whether IST is previously scanned or not for IST that are not on file.
                    If Not CheckPreviousScan(strValidCode) Then
                        'Method for IST when not on file.
                        InterStoreTransfer(strValidCode)
                    Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M65"), "Alert ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) '----------------------
                        m_BDUODScan.txtProductCode.Text = ""
                        m_BDUODScan.txtProductCode.Focus()
                    End If
                End If
            Else
                Return False
            End If
            Return bReturn
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Validate IST Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Function
    ' V1.1 - KK
    ' Added new function ValidateDallasUOD to handle processing of all Dallas UOD's

    ''' <summary>
    ''' Function to Validate Dallas UOD Barcode
    ''' </summary>
    ''' <param name="cBarcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ValidateDallasUOD(ByVal cBarcode As String) As Boolean
        Try
            Dim cValidCode As String = ""
            'Flag which is used to hold the information for a valid UOD.If it is returned as true then it means that it is a valid UOD.
            Dim bReturn As Boolean = False
            If objAppContainer.objHelper.ValidateDallasUODBarcode(cBarcode, cValidCode) Then
                ' Function to check for misdirected Dallas UOD
                DallasUODMisdirectCheck(cBarcode)
                If m_bDalMisdirectReturn Then
                    m_BDUODScan.txtProductCode.Text = ""
                    m_BDUODScan.txtProductCode.Focus()
                    Return True
                End If
                'To Check whether the scanned UOD is booked in previously
                If CheckPreviousScan(cBarcode) Then
                    objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M65"), "Alert ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    m_BDUODScan.txtProductCode.Text = ""
                    m_BDUODScan.txtProductCode.Focus()
                    Return True
                End If
                bReturn = True
                ' Function to check whether the Dallas UOD barcode is valid
                If (objAppContainer.objDataEngine.ValidateDallasUODScanned(cValidCode, m_DalUODInfo, m_DALLASItemList, AppContainer.FunctionType.BookIn)) Then
                    ' Function to prevent booking in of already accepted Dallas UOD's
                    If Not m_DalUODInfo.ScanStatus = Macros.UNRECEIPTED Then
                        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M65"), "Alert ", MessageBoxButtons.OK, _
                                                                              MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        m_BDUODScan.txtProductCode.Text = ""
                        m_BDUODScan.txtProductCode.Focus()
                        Return True
                    End If
                    ' To Process the Dallas UOD scanned and updating the DAR message 
                    ProcessDallasUODScan(cValidCode)
                Else
#If RF Then
                    If objAppContainer.bTimeOut Then
                        Return False
                    End If
                    If Not objAppContainer.bCommFailure And Not objAppContainer.bReconnectSuccess Then
#End If
                        ' To check whether UOD is previously scanned or not for UOD that are not on file.
                        If Not CheckPreviousScan(cValidCode) Then
                            ' Function to handle Dallas UOD not on file processing
                            DallasUODNotOnFile(cValidCode)
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M65"), "Alert ", MessageBoxButtons.OK, _
                                                                                   MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) '----------------------
                            m_BDUODScan.txtProductCode.Text = ""
                            m_BDUODScan.txtProductCode.Focus()
                        End If
#If RF Then
                    End If
#End If
                End If
            Else
                Return False
            End If
            Return bReturn
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Validate UOD Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Function

    ''' <summary>
    ''' Method to Check if the UOD is already scanned or not
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckPreviousScan(ByVal strBarcode As String) As Boolean
        'Status flag to check whether the UOD is already scanned or not
        Try
            Dim bAlreadyBookedIn As Boolean = False
            'to check whether the scanned UOD is already a midirect return
            For Each strPreviousUOD As String In m_arrMisdirectReturnUOD
                If strPreviousUOD = strBarcode Then
                    bMisdirectReturn = True
                    Return False
                    Exit For
                End If
            Next
            'To Check whether the scanned UOD is already present in the Batch arraylist.
            For Each strPreviousUOD As String In m_arrScannedUODBatch
                If strPreviousUOD = strBarcode Then
                    bAlreadyBookedIn = True
                    Exit For
                End If
            Next
            For Each strPreviousUOD As String In m_arrMisdirectReturnUOD
                If strPreviousUOD = strBarcode Then
                    bMisdirectReturn = True
                    Return False
                    Exit For
                End If
            Next
            ' V1.1 - KK
            'Added check to prevent already scanned Dallas UOD to be scanned again
            For Each strPreviousUOD As String In arrDallasScannedUOD
                If strPreviousUOD = strBarcode Then
                    bAlreadyBookedIn = True
                    Exit For
                End If
            Next
            ' V1.1 - KK
            'Added check to prevent already scanned Banked UOD's to be scanned again
            For Each strPreviousUOD As String In m_arrDallasBankedUOD
                If strPreviousUOD = strBarcode Then
                    bAlreadyBookedIn = True
                    Exit For
                End If
            Next
            If Not bAlreadyBookedIn Then
                'CR to check the parent dolly is already scanned or not
                For Each strParentUOD As String In m_arrChildBkdDolly
                    If strParentUOD = strBarcode Then
                        bAlreadyBookedIn = True
                        Exit For
                    End If
                Next
            End If
            'To Check whether the sacnned UOD is already present in the ScannedUOD arraylist.
            If Not bAlreadyBookedIn Then
                For Each strPreviousUOD As String In m_arrScannedUOD
                    If strPreviousUOD = strBarcode Then
                        bAlreadyBookedIn = True
                        Exit For
                    End If
                Next
            End If
            Return bAlreadyBookedIn
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Checking Previous Scan Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    ''' Method to Process the UOD Scan
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <remarks></remarks>
    Private Sub ProcessUODScan(ByVal strBarcode As String)
        'An object to hold the details of a scanned UOD.
        Try
            Dim objScanDetails As New GIValueHolder.ScanDetails
            objScanDetails.ScannedCode = strBarcode
            objScanDetails.ScanDate = Format(DateTime.Now, "yyyyMMdd")
            objScanDetails.ScanTime = Format(DateTime.Now, "HHmmss")
            objScanDetails.ScanType = ScanType.Booked
            objScanDetails.ScanLevel = "D"
            objScanDetails.DespatchDate = m_UODInfo.DespatchDate
            objScanDetails.Rejected = "N"
            objScanDetails.Parent = m_UODInfo.ImmLicenseNo

            'To add the details of the scanned UOD to the arraylist that holds the batch information.
            m_arrListBatch.Add(objScanDetails)
            'Adds the currently scanned barcode to arraylist that holds the batch of barcodes scanned
            m_arrScannedUODBatch.Add(strBarcode)

            ' Check if the delivery is expected or outstanding 
            'To check whether the Delivery is Expected or Outstanding and thereby incrementing the corresponding count of the deliveries
            If m_UODInfo.ExpectedDeliveryDate = Macros.EXPECTED Then
                Select Case m_UODInfo.UODType
                    Case ContainerType.Dolly
                        m_objExpectedBatch.TotalDolly += 1
                    Case ContainerType.Crate
                        m_objExpectedBatch.TotalCrate += 1
                    Case ContainerType.RollCage
                        m_objExpectedBatch.TotalRollCage += 1
                    Case ContainerType.Pallet
                        m_objExpectedBatch.TotalPallet += 1
                    Case ContainerType.Outer
                        m_objExpectedBatch.TotalOuter += 1
                    Case ContainerType.IST
                        m_objExpectedBatch.TotalIST += 1
                End Select
                m_objExpectedBatch.TotalUOD += 1
            ElseIf m_UODInfo.ExpectedDeliveryDate = Macros.OUTSTANDING Then
                'Incrementing the count of deliveries if the delivery is Outstanding
                Select Case m_UODInfo.UODType
                    Case ContainerType.Dolly
                        m_objOutStandingBatch.TotalDolly += 1
                    Case ContainerType.Crate
                        m_objOutStandingBatch.TotalCrate += 1
                    Case ContainerType.RollCage
                        m_objOutStandingBatch.TotalRollCage += 1
                    Case ContainerType.Pallet
                        m_objOutStandingBatch.TotalPallet += 1
                    Case ContainerType.Outer
                        m_objOutStandingBatch.TotalOuter += 1
                    Case ContainerType.IST
                        m_objOutStandingBatch.TotalIST += 1
                End Select
                m_objOutStandingBatch.TotalUOD += 1
            Else
                'Incrementing the count of deliveries if the delivery is Future
                Select Case m_UODInfo.UODType
                    Case ContainerType.Dolly
                        m_objFutureBatch.TotalDolly += 1
                    Case ContainerType.Crate
                        m_objFutureBatch.TotalCrate += 1
                    Case ContainerType.RollCage
                        m_objFutureBatch.TotalRollCage += 1
                    Case ContainerType.Pallet
                        m_objFutureBatch.TotalPallet += 1
                    Case ContainerType.Outer
                        m_objFutureBatch.TotalOuter += 1
                    Case ContainerType.IST
                        m_objFutureBatch.TotalIST += 1
                End Select
                m_objFutureBatch.TotalUOD += 1
            End If

            If m_UODInfo.UODType = ContainerType.Dolly AndAlso Not CheckChildLevelScanning(strBarcode) Then
                If m_UODInfo.PartialBkd = Macros.Y Then
                    If objAppContainer.objDataEngine.GetUODChildCount(strBarcode, m_iUODChildCount) Then
                        RevertBookInCounts(m_UODInfo.ExpectedDeliveryDate, m_iUODChildCount)
                    End If
                End If
            End If
            'To check if the UOD scanned has a parent or not
            'If parent is there then the immediate licence number will not be zero
            If Not m_UODInfo.ImmLicenseNo = Macros.ZEROUOD Then
                Dim bParentFoundInBatch As Boolean = False
                Dim bParentFound As Boolean = False
                'new variables to hold the parial UOD info
                Dim bParentFoundInPartialBatch As Boolean = False
                Dim bParentFoundPartial As Boolean = False
                'checks if the user has already started with scanning the parent container at child level
                'The parent info is checked first in the arraylist that holds that info about parents that 
                'are scanned at child level in an unconfirmed batch.
                For Each objParent As UODParentDetails In m_arrBatchParentDetails
                    If objParent.UODParent = m_UODInfo.ImmLicenseNo Then
                        objParent.UODScannedCount += 1
                        'If objParent.UODScannedCount = objParent.UODChildCount Then
                        '    'CR add dolly id whose all child crates are booked in
                        '    m_arrChildBkdDolly.Add(m_UODInfo.ImmLicenseNo)
                        'End If
                        bParentFoundInBatch = True
                        Exit For
                    End If
                Next
                If Not bParentFoundInBatch Then
                    'If the parent info is not presend in parent info holder for batch, the immediate licence
                    'is check against the parent details for already confirmed batches.
                    For Each objParent As UODParentDetails In m_arrParentDetails
                        If objParent.UODParent = m_UODInfo.ImmLicenseNo Then
                            Dim objParentClone As New UODParentDetails
                            objParentClone.UODChildCount = objParent.UODChildCount
                            objParentClone.UODExpectedDate = objParent.UODExpectedDate
                            objParentClone.UODParent = objParent.UODParent
                            objParentClone.UODScannedCount = objParent.UODScannedCount + 1
                            m_arrBatchParentDetails.Add(objParentClone)
                            bParentFound = True
                            Exit For
                        End If
                    Next
                End If
                If Not (bParentFoundInBatch OrElse bParentFound) Then
                    For Each objPartial As UODParentDetails In m_arrBatchPartialDetails
                        '
                        If objPartial.UODParent = m_UODInfo.ImmLicenseNo Then
                            objPartial.UODScannedCount += 1
                            bParentFoundInPartialBatch = True
                            Exit For
                        End If
                    Next
                    If Not bParentFoundInPartialBatch Then
                        'If the parent info is not presend in parent info holder for batch, the immediate licence
                        'is check against the parent details for already confirmed batches.
                        For Each objPartial As UODParentDetails In m_arrPartialDetails
                            If objPartial.UODParent = m_UODInfo.ImmLicenseNo Then
                                Dim objParentClone As New UODParentDetails
                                objParentClone.UODChildCount = objPartial.UODChildCount
                                objParentClone.UODExpectedDate = objPartial.UODExpectedDate
                                objParentClone.UODParent = objPartial.UODParent
                                objParentClone.UODScannedCount = objPartial.UODScannedCount + 1

                                m_arrBatchParentDetails.Add(objParentClone)
                                bParentFoundPartial = True
                                Exit For
                            End If
                        Next
                    End If
                End If

                If Not (bParentFoundInBatch OrElse bParentFound _
                        OrElse bParentFoundInPartialBatch OrElse bParentFoundPartial) Then
                    'Validate and add to arraylist batch
                    Dim objUODInfo As New UODInfo
                    If (objAppContainer.objDataEngine.ValidateUODScanned(m_UODInfo.ImmLicenseNo, objUODInfo, AppContainer.FunctionType.BookIn)) Then
                        If objAppContainer.objDataEngine.GetUODChildCount(m_UODInfo.ImmLicenseNo, m_iUODChildCount) Then
                            'CR Setting the No of children of Parial Booked dolly as the no of unbooked child crates
                            objUODInfo.NoOfChildren = m_iUODChildCount.ToString()
                        End If
                        Dim objParent As New UODParentDetails
                        objParent.UODChildCount = CType(objUODInfo.NoOfChildren, Integer)
                        objParent.UODExpectedDate = objUODInfo.ExpectedDeliveryDate
                        objParent.UODParent = objUODInfo.UODNumber
                        objParent.UODScannedCount = 1 'objUODInfo.NoOfChildren + 1

                        If objUODInfo.PartialBkd = Macros.Y Then
                            m_arrBatchPartialDetails.Add(objParent)
                        Else
                            m_arrBatchParentDetails.Add(objParent)
                            ScannedAtChildLevel(m_UODInfo.ExpectedDeliveryDate, CType(objUODInfo.NoOfChildren, Integer))
                        End If
                    End If

                End If

            End If
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
            'Check whether the Batch count is equal to the scan count.
            If m_arrListBatch.Count = m_iCount Then
                BDSessionMgr.GetInstance().DisplayBDScreen(BDSCREENS.BDUODBatchDrvrConfrm)
            Else
                DisplayBDScreen(BDSCREENS.BDUODScan)
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Process UOD Scan Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ' V1.1 - KK
    ' Added new method ProcessDallasUODScan

    ''' <summary>
    ''' Method to Process the Dallas UOD Scanned data
    ''' </summary>
    ''' <param name="cBarcode"></param>
    ''' <remarks></remarks>
    Private Sub ProcessDallasUODScan(ByVal cBarcode As String)
        Try
            Dim dttoday As DateTime
            Dim dtExpectedDelDate As DateTime
            Dim iDateCompare As Integer
            'Saving the date when the Dallas UOD is scanned in store for sending in DAR message
            m_DalUODInfo.DallasScanDate = Format(DateTime.Now, "yyMMdd")
            m_DalUODInfo.ScanStatus = Macros.RECEIPTED
            'Adds the currently scanned barcode to arraylist that holds the batch of barcodes scanned
            m_arrDalScanBatch.Add(m_DalUODInfo)
            arrDallasScannedUOD.Add(cBarcode)
            m_arrDallasConfirmedUOD.Add(cBarcode)
            ' Check if the delivery scanned is expected or outstanding 
            'To check whether the Delivery is Expected or Outstanding and thereby incrementing the corresponding count of the deliveries
            dttoday = DateTime.Now.Date
            dtExpectedDelDate = DateTime.ParseExact(m_DalUODInfo.DallasExpectedDate, "yyMMdd", CultureInfo.InvariantCulture)
            'Comparing the UOD Expected Date with today's date
            iDateCompare = DateTime.Compare(dtExpectedDelDate, dttoday)
            If iDateCompare < 0 Then
                'Updating the outstanding Dallas UOD count
                m_objOutstandingScanned.TotalBurton += 1
                m_objOutStandingBatch.TotalUOD += 1
            ElseIf iDateCompare = 0 Then
                'Updating the Expected Dallas UOD Count
                m_objExpectedScanned.TotalBurton += 1
                m_objExpectedBatch.TotalUOD += 1
            ElseIf iDateCompare > 0 Then
                'Updating the Future Dallas UOD Count
                m_objFutureScanned.TotalBurton += 1
                m_objFutureBatch.TotalUOD += 1
            End If

            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
            m_BDUODScan.Enabled = True
            DisplayBDScreen(BDSCREENS.BDUODScan)

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Process Dallas UOD Scan Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub

    ''' <summary>
    ''' Method to handle the scenario when a container is scanned at child level
    ''' Eg.Like a dolly is getting scanned at crate level while its expected to be 
    ''' scanned at dolly level itself.
    ''' </summary>
    ''' <param name="strDate"></param>
    ''' <param name="iCrateCount"></param>
    ''' <remarks></remarks>
    Private Sub ScannedAtChildLevel(ByVal strDate As String, ByVal iCrateCount As Integer)
        'The total dolly count in decremented and the corresponding crate count is incremented
        'by number of cartons present in the dolly
        Try
            If strDate = Macros.EXPECTED Then
                m_objExpected.TotalDolly -= 1
                m_objExpected.TotalCrate += iCrateCount
            Else
                m_objOutstanding.TotalDolly -= 1
                m_objOutstanding.TotalCrate += iCrateCount
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Scanning at Child Level Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Function to check whether the delivery is booked in at parent or child level.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckChildLevelScanning(ByVal strBarcode As String) As Boolean
        Try
            bIsParent = True
            'To check for the details of the parent UOD in the Parent batch arraylist.
            For Each objParent As UODParentDetails In m_arrBatchParentDetails
                'To Check whether the scanned UOD is parent or not.
                If objParent.UODParent = strBarcode Then
                    RevertBookInCounts(objParent.UODExpectedDate, (objParent.UODChildCount - objParent.UODScannedCount))
                    Return True
                End If
            Next
            'To check for the details of the parent UOD in the Parent details arraylist.
            For Each objParent As UODParentDetails In m_arrParentDetails
                If objParent.UODParent = strBarcode Then
                    RevertBookInCounts(objParent.UODExpectedDate, (objParent.UODChildCount - objParent.UODScannedCount))
                    Return True
                End If
            Next
            For Each objParent As UODParentDetails In m_arrBatchPartialDetails
                'To Check whether the scanned UOD is parent or not.
                If objParent.UODParent = strBarcode Then
                    RevertBookInCounts(objParent.UODExpectedDate, (objParent.UODChildCount - objParent.UODScannedCount))
                    Return True
                End If
            Next
            'To check for the details of the parent UOD in the Parent details arraylist.
            For Each objParent As UODParentDetails In m_arrPartialDetails
                If objParent.UODParent = strBarcode Then
                    RevertBookInCounts(objParent.UODExpectedDate, (objParent.UODChildCount - objParent.UODScannedCount))
                    Return True
                End If
            Next
            Return False
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Checking Child Level Scanning Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Function
    ''' <summary>
    ''' Method to revert back the count of the Parent or the Child items when the Rescan button is selected.
    ''' </summary>
    ''' <param name="strDate"></param>
    ''' <param name="iCrateCount"></param>
    ''' <remarks></remarks>
    Private Sub RevertBookInCounts(ByVal strExpected As String, ByVal iCrateCount As Integer)
        'To Revert the Book In Counts for Expected delivery when Rescan button is selected.
        Try
            If strExpected = Macros.EXPECTED Then
                m_objExpected.TotalDolly += 1
                m_objExpected.TotalCrate -= iCrateCount
                'To Revert the Book In Counts for Outstanding delivery when Rescan button is selected.
            Else
                m_objOutstanding.TotalDolly += 1
                m_objOutstanding.TotalCrate -= iCrateCount
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Revert Book In Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' The method Displays the Book In Delivery Home Screen.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayBDScan(ByVal o As Object, ByVal e As EventArgs)
        With m_BDUODHome
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            .Visible = True
            objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINDELIVERYHOME
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' Method to display the expected and outstanding deliveries.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayInitialSummary(ByVal o As Object, ByVal e As EventArgs)
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            With m_BDUODInitialSummary
                'To Populate the listview for Expected and Outstanding deliveries.
                PopulateExpectedList()
                .Visible = True
                objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERINITIAL
                .Refresh()
            End With
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Initial Screen Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Public Function RemainingContainer(ByVal iVal As Integer) As Integer
        If iVal < 0 Then
            iVal = 0
        End If
        Return iVal
    End Function
    ''' <summary>
    ''' Method to scan the UOD and also to display the count of deliveries for expected,outstanding and not on file deliveries.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayBookInUODScan(ByVal o As Object, ByVal e As EventArgs)
        Try
            objAppContainer.objStatusBar.SetMessage("")
            Dim iVal As Integer = 0
            Dim bExpected As Boolean = False
            Dim bOutstanding As Boolean = False
            Dim bNotOnFile As Boolean = False
            If m_arrListBatch.Count = 0 Then
                CreateRollbackPoint()
            End If
            If m_arrListBatch.Count > 0 Or m_arrScannedUOD.Count > 0 Or (m_arrDalScanBatch.Count > 0 And m_arrListBatch.Count = 0) Then
                m_BDUODScan.Btn_Finish1.Visible = True
                m_BDUODScan.Btn_Finish1.Enabled = True
            ElseIf m_arrListBatch.Count = 0 And m_arrScannedUOD.Count = 0 Or m_arrDalScanBatch.Count = 0 Then
                m_BDUODScan.Btn_Finish1.Visible = False
                m_BDUODScan.Btn_Finish1.Enabled = False
            End If
            With m_BDUODScan
                'TODO
                m_BDUODScan.lstUODScan.BeginUpdate()
                m_BDUODScan.lstUODScan.Items.Clear()
                'To add the count of expected deliveries to the listview.                          
                If (m_objExpected.TotalUOD > 0 Or m_objExpectedScanned.TotalUOD > 0 Or m_objExpectedBatch.TotalUOD > 0 Or m_objFutureBatch.TotalUOD > 0 Or m_objFutureScanned.TotalUOD > 0 Or (m_iExpectedDalFinalCount + m_iFutureDalFinalCount) > 0) Then
                    .lstUODScan.Items.Add("Expected")

                    If (m_objExpected.TotalDolly > 0 Or m_objExpectedScanned.TotalDolly > 0 Or m_objExpectedBatch.TotalDolly > 0 Or m_objFutureBatch.TotalDolly > 0 Or m_objFutureScanned.TotalDolly > 0) Then

                        .lstUODScan.Items.Add( _
 (ContainerName.Dolly.PadRight(Macros.BLEN, " ") + (m_objExpectedBatch.TotalDolly + m_objExpectedScanned.TotalDolly + m_objFutureBatch.TotalDolly + m_objFutureScanned.TotalDolly).ToString().PadRight(Macros.RLEN, " ") + _
                                             (RemainingContainer(m_objExpected.TotalDolly - (m_objExpectedBatch.TotalDolly + m_objExpectedScanned.TotalDolly))).ToString()))
                    End If

                    If (m_objExpected.TotalCrate > 0 Or m_objExpectedScanned.TotalCrate > 0 Or m_objExpectedBatch.TotalCrate > 0 Or m_objFutureBatch.TotalCrate > 0 Or m_objFutureScanned.TotalCrate > 0) Then
                        .lstUODScan.Items.Add( _
 (ContainerName.Crate.PadRight(Macros.BLEN, " ") + (m_objExpectedBatch.TotalCrate + m_objExpectedScanned.TotalCrate + m_objFutureBatch.TotalCrate + m_objFutureScanned.TotalCrate).ToString.PadRight(Macros.RLEN, " ") + _
                                             (RemainingContainer(m_objExpected.TotalCrate - (m_objExpectedBatch.TotalCrate + m_objExpectedScanned.TotalCrate))).ToString()))
                    End If
                    If (m_objExpected.TotalPallet > 0 Or m_objExpectedScanned.TotalPallet > 0 Or m_objExpectedBatch.TotalPallet > 0 Or m_objFutureBatch.TotalPallet > 0 Or m_objFutureScanned.TotalPallet > 0) Then
                        .lstUODScan.Items.Add( _
(ContainerName.Pallet.PadRight(Macros.BLEN, " ") + (m_objExpectedBatch.TotalPallet + m_objExpectedScanned.TotalPallet + m_objFutureBatch.TotalPallet + m_objFutureScanned.TotalPallet).ToString.PadRight(Macros.RLEN, " ") + _
                                          (RemainingContainer(m_objExpected.TotalPallet - (m_objExpectedBatch.TotalPallet + m_objExpectedScanned.TotalPallet))).ToString()))
                    End If
                    'V1.1 KK
                    'Added as part of Dallas Project for displaying remaining Burton UOD count
                    If (m_objExpected.TotalBurton > 0 Or m_objExpectedScanned.TotalBurton > 0 Or m_iExpectedDalFinalCount > 0 Or m_objFutureScanned.TotalBurton > 0 Or m_iFutureDalFinalCount > 0) Then
                        .lstUODScan.Items.Add( _
(ContainerName.Burton.PadRight(Macros.BLEN, " ") + (m_objExpectedScanned.TotalBurton + m_iExpectedDalFinalCount + m_objFutureScanned.TotalBurton + m_iFutureDalFinalCount).ToString.PadRight(Macros.RLEN, " ") + _
                                          (RemainingContainer(m_objExpected.TotalBurton - (m_objExpectedScanned.TotalBurton + m_iExpectedDalFinalCount))).ToString()))
                    End If
                    If (m_objExpected.TotalRollCage > 0 Or m_objExpectedScanned.TotalRollCage > 0 Or m_objExpectedBatch.TotalRollCage > 0 Or m_objFutureBatch.TotalRollCage > 0 Or m_objFutureScanned.TotalRollCage > 0) Then
                        .lstUODScan.Items.Add( _
(ContainerName.Rollcage.PadRight(Macros.BLEN, " ") + (m_objExpectedBatch.TotalRollCage + m_objExpectedScanned.TotalRollCage + m_objFutureBatch.TotalRollCage + m_objFutureScanned.TotalRollCage).ToString.PadRight(Macros.RLEN, " ") + _
                                         (RemainingContainer(m_objExpected.TotalRollCage - (m_objExpectedBatch.TotalRollCage + m_objExpectedScanned.TotalRollCage))).ToString()))
                    End If
                    If (m_objExpected.TotalOuter > 0 Or m_objExpectedScanned.TotalOuter > 0 Or m_objExpectedBatch.TotalOuter > 0 Or m_objFutureBatch.TotalOuter > 0 Or m_objFutureScanned.TotalOuter > 0) Then
                        .lstUODScan.Items.Add( _
(ContainerName.Outer.PadRight(Macros.BLEN, " ") + (m_objExpectedBatch.TotalOuter + m_objExpectedScanned.TotalOuter + m_objFutureBatch.TotalOuter + m_objFutureScanned.TotalOuter).ToString.PadRight(Macros.RLEN, " ") + _
                                        (RemainingContainer(m_objExpected.TotalOuter - (m_objExpectedBatch.TotalOuter + m_objExpectedScanned.TotalOuter))).ToString()))
                    End If
                    If (m_objExpected.TotalIST > 0 Or m_objExpectedScanned.TotalIST > 0 Or m_objExpectedBatch.TotalIST > 0 Or m_objFutureBatch.TotalIST > 0 Or m_objFutureScanned.TotalIST > 0) Then
                        .lstUODScan.Items.Add( _
(ContainerName.IST.PadRight(Macros.BLEN, " ") + (m_objExpectedBatch.TotalIST + m_objExpectedScanned.TotalIST + m_objFutureBatch.TotalIST + m_objFutureScanned.TotalIST).ToString.PadRight(Macros.RLEN, " ") + _
                                     (RemainingContainer(m_objExpected.TotalIST - (m_objExpectedBatch.TotalIST + m_objExpectedScanned.TotalIST))).ToString()))
                    End If
                    bExpected = True
                End If

                ''To add the count of outstanding deliveries to the listview.
                If (m_objOutstanding.TotalUOD > 0 Or m_objOutstandingScanned.TotalUOD > 0 Or m_objOutStandingBatch.TotalUOD > 0 Or m_iOutstandingDalFinalCount > 0) Then
                    If bExpected Then
                        .lstUODScan.Items.Add("------------------------------------------------------------------------------------------------")
                    End If
                    .lstUODScan.Items.Add("Outstanding")
                    If (m_objOutstanding.TotalDolly > 0 Or m_objOutstandingScanned.TotalDolly > 0 Or m_objOutStandingBatch.TotalDolly > 0) Then
                        .lstUODScan.Items.Add( _
(ContainerName.Dolly.PadRight(Macros.BLEN, " ") + (m_objOutStandingBatch.TotalDolly + m_objOutstandingScanned.TotalDolly).ToString.PadRight(Macros.RLEN, " ") + _
                                            (RemainingContainer(m_objOutstanding.TotalDolly - (m_objOutStandingBatch.TotalDolly + m_objOutstandingScanned.TotalDolly))).ToString()))

                    End If
                    If (m_objOutstanding.TotalCrate > 0 Or m_objOutstandingScanned.TotalCrate > 0 Or m_objOutStandingBatch.TotalCrate > 0) Then
                        .lstUODScan.Items.Add( _
 (ContainerName.Crate.PadRight(Macros.BLEN, " ") + (m_objOutStandingBatch.TotalCrate + m_objOutstandingScanned.TotalCrate).ToString.PadRight(Macros.RLEN, " ") + _
                                             (RemainingContainer(m_objOutstanding.TotalCrate - (m_objOutStandingBatch.TotalCrate + m_objOutstandingScanned.TotalCrate))).ToString()))
                    End If
                    If (m_objOutstanding.TotalPallet > 0 Or m_objOutstandingScanned.TotalPallet > 0 Or m_objOutStandingBatch.TotalPallet > 0) Then
                        .lstUODScan.Items.Add( _
   (ContainerName.Pallet.PadRight(Macros.BLEN, " ") + (m_objOutStandingBatch.TotalPallet + m_objOutstandingScanned.TotalPallet).ToString.PadRight(Macros.RLEN, " ") + _
                                               (RemainingContainer(m_objOutstanding.TotalPallet - (m_objOutStandingBatch.TotalPallet + m_objOutstandingScanned.TotalPallet))).ToString()))
                    End If
                    'V1.1 KK
                    'Added as part of Dallas Project for displaying outstanding Burton count
                    If (m_objOutstanding.TotalBurton > 0 Or m_objOutstandingScanned.TotalBurton > 0 Or m_iOutstandingDalFinalCount > 0) Then
                        .lstUODScan.Items.Add( _
   (ContainerName.Burton.PadRight(Macros.BLEN, " ") + (m_objOutstandingScanned.TotalBurton + m_iOutstandingDalFinalCount).ToString.PadRight(Macros.RLEN, " ") + _
                                               (RemainingContainer(m_objOutstanding.TotalBurton - (m_objOutstandingScanned.TotalBurton + m_iOutstandingDalFinalCount))).ToString()))
                    End If
                    If (m_objOutstanding.TotalRollCage > 0 Or m_objOutstandingScanned.TotalRollCage > 0 Or m_objOutStandingBatch.TotalRollCage > 0) Then
                        .lstUODScan.Items.Add( _
   (ContainerName.Rollcage.PadRight(Macros.BLEN, " ") + (m_objOutStandingBatch.TotalRollCage + m_objOutstandingScanned.TotalRollCage).ToString.PadRight(Macros.RLEN, " ") + _
                                               (RemainingContainer(m_objOutstanding.TotalRollCage - (m_objOutStandingBatch.TotalRollCage + m_objOutstandingScanned.TotalRollCage))).ToString()))
                    End If
                    If (m_objOutstanding.TotalOuter > 0 Or m_objOutstandingScanned.TotalOuter > 0 Or m_objOutStandingBatch.TotalOuter > 0) Then
                        .lstUODScan.Items.Add( _
   (ContainerName.Outer.PadRight(Macros.BLEN, " ") + (m_objOutStandingBatch.TotalOuter + m_objOutstandingScanned.TotalOuter).ToString.PadRight(Macros.RLEN, " ") + _
                                               (RemainingContainer(m_objOutstanding.TotalOuter - (m_objOutStandingBatch.TotalOuter + m_objOutstandingScanned.TotalOuter))).ToString()))
                    End If
                    If (m_objOutstanding.TotalIST > 0 Or m_objOutstandingScanned.TotalIST > 0 Or m_objOutStandingBatch.TotalIST > 0) Then
                        .lstUODScan.Items.Add( _
   (ContainerName.IST.PadRight(Macros.BLEN, " ") + (m_objOutStandingBatch.TotalIST + m_objOutstandingScanned.TotalIST).ToString.PadRight(Macros.RLEN, " ") + _
                                               (RemainingContainer(m_objOutstanding.TotalIST - (m_objOutStandingBatch.TotalIST + m_objOutstandingScanned.TotalIST))).ToString()))
                    End If
                    bOutstanding = True
                End If
                '--------------------------
                'To add the count of misdirected deliveries to the listview.
                If (m_objMisdirectedScanned.TotalUOD > 0 Or m_objMisdirectedBatch.TotalUOD > 0 Or m_objMisdirectedScanned.TotalBurton > 0 Or m_iNotOnFileDalFinalCount) Then
                    If bExpected Or bOutstanding Then
                        .lstUODScan.Items.Add("------------------------------------------------------------------------------------------------")
                    End If
                    .lstUODScan.Items.Add("Not On File")
                    If (m_objMisdirectedScanned.TotalDolly > 0 Or m_objMisdirectedBatch.TotalDolly > 0) Then
                        .lstUODScan.Items.Add( _
   (ContainerName.Dolly.PadRight(Macros.BLEN, " ") + (m_objMisdirectedScanned.TotalDolly + m_objMisdirectedBatch.TotalDolly).ToString()))

                    End If
                    If (m_objMisdirectedScanned.TotalCrate > 0 Or m_objMisdirectedBatch.TotalCrate > 0) Then

                        .lstUODScan.Items.Add( _
  (ContainerName.Crate.PadRight(Macros.BLEN, " ") + (m_objMisdirectedScanned.TotalCrate + m_objMisdirectedBatch.TotalCrate).ToString()))
                    End If
                    If (m_objMisdirectedScanned.TotalPallet > 0 Or m_objMisdirectedBatch.TotalPallet > 0) Then
                        .lstUODScan.Items.Add( _
  (ContainerName.Pallet.PadRight(Macros.BLEN, " ") + (m_objMisdirectedScanned.TotalPallet + m_objMisdirectedBatch.TotalPallet).ToString()))
                    End If
                    'V1.1 KK
                    'Added as part of Dallas Project for displaying Burton count
                    If (m_objMisdirectedScanned.TotalBurton > 0 Or m_iNotOnFileDalFinalCount > 0) Then
                        .lstUODScan.Items.Add( _
  (ContainerName.Burton.PadRight(Macros.BLEN, " ") + (m_objMisdirectedScanned.TotalBurton + m_iNotOnFileDalFinalCount).ToString()))
                    End If
                    If (m_objMisdirectedScanned.TotalRollCage > 0 Or m_objMisdirectedBatch.TotalRollCage > 0) Then
                        .lstUODScan.Items.Add( _
 (ContainerName.Rollcage.PadRight(Macros.BLEN, " ") + (m_objMisdirectedScanned.TotalRollCage + m_objMisdirectedBatch.TotalRollCage).ToString()))
                    End If
                    If (m_objMisdirectedScanned.TotalOuter > 0 Or m_objMisdirectedBatch.TotalOuter > 0) Then
                        .lstUODScan.Items.Add( _
 (ContainerName.Outer.PadRight(Macros.BLEN, " ") + (m_objMisdirectedScanned.TotalOuter + m_objMisdirectedBatch.TotalOuter).ToString()))
                    End If
                    If (m_objMisdirectedScanned.TotalIST > 0 Or m_objMisdirectedBatch.TotalIST > 0) Then
                        .lstUODScan.Items.Add( _
(ContainerName.IST.PadRight(Macros.BLEN, " ") + (m_objMisdirectedScanned.TotalIST + m_objMisdirectedBatch.TotalIST).ToString()))
                    End If
                    bNotOnFile = True
                End If



                If (m_objMisdirectedReturnScanned.TotalUOD > 0 Or m_objMisdirectedReturnBatch.TotalUOD > 0 Or m_objMisdirectedReturnScanned.TotalBurton > 0 Or m_iMisdirectReturnedDalFinalCount > 0) Then
                    If bExpected Or bOutstanding Or bNotOnFile Then
                        .lstUODScan.Items.Add("------------------------------------------------------------------------------------------------")

                    End If
                    .lstUODScan.Items.Add("Misdirected(Return)")
                    If (m_objMisdirectedReturnScanned.TotalDolly > 0 Or m_objMisdirectedReturnBatch.TotalDolly > 0) Then
                        .lstUODScan.Items.Add( _
   (ContainerName.Dolly.PadRight(Macros.BLEN, " ") + (m_objMisdirectedReturnScanned.TotalDolly + m_objMisdirectedReturnBatch.TotalDolly).ToString()))

                    End If
                    If (m_objMisdirectedReturnScanned.TotalCrate > 0 Or m_objMisdirectedReturnBatch.TotalCrate > 0) Then

                        .lstUODScan.Items.Add( _
  (ContainerName.Crate.PadRight(Macros.BLEN, " ") + (m_objMisdirectedReturnScanned.TotalCrate + m_objMisdirectedReturnBatch.TotalCrate).ToString()))
                    End If
                    If (m_objMisdirectedReturnScanned.TotalPallet > 0 Or m_objMisdirectedReturnBatch.TotalPallet > 0) Then
                        .lstUODScan.Items.Add( _
  (ContainerName.Pallet.PadRight(Macros.BLEN, " ") + (m_objMisdirectedReturnScanned.TotalPallet + m_objMisdirectedReturnBatch.TotalPallet).ToString()))
                    End If
                    'V1.1 KK
                    'Added as part of Dallas Project for displaying Burton count
                    If (m_objMisdirectedReturnScanned.TotalBurton > 0 Or m_iMisdirectReturnedDalFinalCount > 0) Then
                        .lstUODScan.Items.Add( _
  (ContainerName.Burton.PadRight(Macros.BLEN, " ") + (m_objMisdirectedReturnScanned.TotalBurton + m_iMisdirectReturnedDalFinalCount).ToString()))
                    End If
                    If (m_objMisdirectedReturnScanned.TotalRollCage > 0 Or m_objMisdirectedReturnBatch.TotalRollCage > 0) Then
                        .lstUODScan.Items.Add( _
 (ContainerName.Rollcage.PadRight(Macros.BLEN, " ") + (m_objMisdirectedReturnScanned.TotalRollCage + m_objMisdirectedReturnBatch.TotalRollCage).ToString()))
                    End If
                    If (m_objMisdirectedReturnScanned.TotalOuter > 0 Or m_objMisdirectedReturnBatch.TotalOuter > 0) Then
                        .lstUODScan.Items.Add( _
 (ContainerName.Outer.PadRight(Macros.BLEN, " ") + (m_objMisdirectedReturnScanned.TotalOuter + m_objMisdirectedReturnBatch.TotalOuter).ToString()))
                    End If
                    If (m_objMisdirectedReturnScanned.TotalIST > 0 Or m_objMisdirectedReturnBatch.TotalIST > 0) Then
                        .lstUODScan.Items.Add( _
(ContainerName.IST.PadRight(Macros.BLEN, " ") + (m_objMisdirectedReturnScanned.TotalIST + m_objMisdirectedReturnBatch.TotalIST).ToString()))
                    End If
                End If

                'remove reference to this array list ------------------------------------------------------------------
                'm_arrBookInDetails.Add(.lvwUODScan)
                m_BDUODScan.lstUODScan.EndUpdate()
                'Setting the active screen as Book In UOD Scan screen
                objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINUODSCAN
                .Visible = True
                'Clearing the Textbox and setting the Focus to the texbox
                m_BDUODScan.txtProductCode.Text = ""
                m_BDUODScan.txtProductCode.Focus()
                .Refresh()
            End With
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery UOD Scan Screen : " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Method to display the driver confirmation screen when the number of items becomes equal to the batch count.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayDrvrConfirmation(ByVal o As Object, ByVal e As EventArgs)
        Try
            objAppContainer.objStatusBar.SetMessage("")
            Dim bBatchBooked As Boolean = False
            Dim bNotOnfile As Boolean = False

            With m_BDUODDrvrConfrmation
                ' m_BDUODDrvrConfrmation.lstvwBatch.BeginUpdate()
                m_BDUODDrvrConfrmation.lstvwBatch.Items.Clear()
                If (m_objExpectedBatch.TotalUOD + m_objOutStandingBatch.TotalUOD + m_objFutureBatch.TotalUOD) > 0 Or arrDallasScannedUOD.Count > 0 Then
                    If (m_objExpectedBatch.TotalDolly + m_objOutStandingBatch.TotalDolly + m_objFutureBatch.TotalDolly) > 0 Then
                        .lstvwBatch.Items.Add( _
                         (ContainerName.Dolly.PadRight(Macros.BLEN, " ") + (m_objExpectedBatch.TotalDolly + m_objOutStandingBatch.TotalDolly + m_objFutureBatch.TotalDolly).ToString()))
                    End If
                    If (m_objExpectedBatch.TotalCrate + m_objOutStandingBatch.TotalCrate + m_objFutureBatch.TotalCrate) > 0 Then
                        .lstvwBatch.Items.Add( _
                         (ContainerName.Crate.PadRight(Macros.BLEN, " ") + (m_objExpectedBatch.TotalCrate + m_objOutStandingBatch.TotalCrate + m_objFutureBatch.TotalCrate).ToString()))
                    End If
                    If (m_objExpectedBatch.TotalPallet + m_objOutStandingBatch.TotalPallet + m_objFutureBatch.TotalPallet) > 0 Then
                        .lstvwBatch.Items.Add( _
                         (ContainerName.Pallet.PadRight(Macros.BLEN, " ") + (m_objExpectedBatch.TotalPallet + m_objOutStandingBatch.TotalPallet + m_objFutureBatch.TotalPallet).ToString()))
                    End If
                    ' v1.1 - CK
                    ' Dallas summary added to the arraylist 
                    If (m_objExpectedScanned.TotalBurton + m_objOutstandingScanned.TotalBurton + m_objFutureScanned.TotalBurton) > 0 Then
                        .lstvwBatch.Items.Add( _
                        (ContainerName.Burton.PadRight(Macros.BLEN, " ") + (m_objExpectedScanned.TotalBurton + m_objOutstandingScanned.TotalBurton + m_objFutureScanned.TotalBurton).ToString()))
                    End If
                    If (m_objExpectedBatch.TotalRollCage + m_objOutStandingBatch.TotalRollCage + m_objFutureBatch.TotalRollCage) > 0 Then
                        .lstvwBatch.Items.Add( _
                         (ContainerName.Rollcage.PadRight(Macros.BLEN, " ") + (m_objExpectedBatch.TotalRollCage + m_objOutStandingBatch.TotalRollCage + m_objFutureBatch.TotalRollCage).ToString()))
                    End If
                    If (m_objExpectedBatch.TotalOuter + m_objOutStandingBatch.TotalOuter + m_objFutureBatch.TotalOuter) > 0 Then
                        .lstvwBatch.Items.Add( _
                         (ContainerName.Outer.PadRight(Macros.BLEN, " ") + (m_objExpectedBatch.TotalOuter + m_objOutStandingBatch.TotalOuter + m_objFutureBatch.TotalOuter).ToString()))
                    End If
                    If (m_objExpectedBatch.TotalIST + m_objOutStandingBatch.TotalIST + m_objFutureBatch.TotalIST) > 0 Then
                        .lstvwBatch.Items.Add( _
                        (ContainerName.IST.PadRight(Macros.BLEN, " ") + (m_objExpectedBatch.TotalIST + m_objOutStandingBatch.TotalIST + m_objFutureBatch.TotalIST).ToString()))
                    End If

                    bBatchBooked = True
                End If

                If m_objMisdirectedBatch.TotalUOD > 0 Or m_objMisdirectedScanned.TotalBurton > 0 Then
                    If bBatchBooked Then
                        .lstvwBatch.Items.Add("------------------------------------------------------------------------------------------------")
                    End If
                    .lstvwBatch.Items.Add("Not on File")
                    If m_objMisdirectedBatch.TotalDolly > 0 Then
                        .lstvwBatch.Items.Add( _
                         (ContainerName.Dolly.PadRight(Macros.BLEN, " ") + m_objMisdirectedBatch.TotalDolly.ToString()))
                    End If
                    If m_objMisdirectedBatch.TotalCrate > 0 Then
                        .lstvwBatch.Items.Add( _
                         (ContainerName.Crate.PadRight(Macros.BLEN, " ") + m_objMisdirectedBatch.TotalCrate.ToString()))
                    End If
                    If m_objMisdirectedBatch.TotalPallet > 0 Then
                        .lstvwBatch.Items.Add( _
                         (ContainerName.Pallet.PadRight(Macros.BLEN, " ") + m_objMisdirectedBatch.TotalPallet.ToString()))
                    End If
                    ' V1.1 - CK
                    ' The booked in count of 'not on file' Dallas UODs
                    If m_objMisdirectedScanned.TotalBurton > 0 Then
                        .lstvwBatch.Items.Add( _
                        (ContainerName.Burton.PadRight(Macros.BLEN, " ") + m_objMisdirectedScanned.TotalBurton.ToString()))
                    End If
                    If m_objMisdirectedBatch.TotalRollCage > 0 Then
                        .lstvwBatch.Items.Add( _
                         (ContainerName.Rollcage.PadRight(Macros.BLEN, " ") + m_objMisdirectedBatch.TotalRollCage.ToString()))
                    End If
                    If m_objMisdirectedBatch.TotalOuter > 0 Then
                        .lstvwBatch.Items.Add( _
                         (ContainerName.Outer.PadRight(Macros.BLEN, " ") + m_objMisdirectedBatch.TotalOuter.ToString()))
                    End If
                    If m_objMisdirectedBatch.TotalIST > 0 Then
                        .lstvwBatch.Items.Add( _
                        (ContainerName.IST.PadRight(Macros.BLEN, " ") + m_objMisdirectedBatch.TotalIST.ToString()))
                    End If
                    bNotOnfile = True
                End If
                If m_objMisdirectedReturnBatch.TotalUOD > 0 Or m_objMisdirectedReturnScanned.TotalBurton > 0 Then
                    If bBatchBooked OrElse bNotOnfile Then
                        .lstvwBatch.Items.Add("------------------------------------------------------------------------------------------------")
                    End If
                    .lstvwBatch.Items.Add("Misdirect(Return)")
                    If m_objMisdirectedReturnBatch.TotalDolly > 0 Then
                        .lstvwBatch.Items.Add( _
                         (ContainerName.Dolly.PadRight(Macros.BLEN, " ") + m_objMisdirectedReturnBatch.TotalDolly.ToString()))
                    End If
                    If m_objMisdirectedReturnBatch.TotalCrate > 0 Then
                        .lstvwBatch.Items.Add( _
                         (ContainerName.Crate.PadRight(Macros.BLEN, " ") + m_objMisdirectedReturnBatch.TotalCrate.ToString()))
                    End If
                    If m_objMisdirectedReturnBatch.TotalPallet > 0 Then
                        .lstvwBatch.Items.Add( _
                         (ContainerName.Pallet.PadRight(Macros.BLEN, " ") + m_objMisdirectedReturnBatch.TotalPallet.ToString()))
                    End If
                    'V1.1 - KK
                    'Count of Misdirected return Dallas UOD's
                    If m_objMisdirectedReturnScanned.TotalBurton > 0 Then
                        .lstvwBatch.Items.Add( _
                         (ContainerName.Burton.PadRight(Macros.BLEN, " ") + m_objMisdirectedReturnScanned.TotalBurton.ToString()))
                    End If
                    If m_objMisdirectedReturnBatch.TotalRollCage > 0 Then
                        .lstvwBatch.Items.Add( _
                         (ContainerName.Rollcage.PadRight(Macros.BLEN, " ") + m_objMisdirectedReturnBatch.TotalRollCage.ToString()))
                    End If
                    If m_objMisdirectedReturnBatch.TotalOuter > 0 Then
                        .lstvwBatch.Items.Add( _
                         (ContainerName.Outer.PadRight(Macros.BLEN, " ") + m_objMisdirectedReturnBatch.TotalOuter.ToString()))
                    End If
                    If m_objMisdirectedReturnBatch.TotalIST > 0 Then
                        .lstvwBatch.Items.Add( _
                        (ContainerName.IST.PadRight(Macros.BLEN, " ") + m_objMisdirectedReturnBatch.TotalIST.ToString()))
                    End If

                End If


                'Checking whether the deliveries has to be booked in at In hours or Out of hours.
                If bIsInHours Then
                    .lblMsg.Visible = True
                    .txtProductCode.Visible = True
                    .btnConfirm.Visible = False
                    .btnNoBadge.Visible = True
                    .lblConfirmMessage.Visible = False
                    .lblDrvrConfrm.Visible = True
                    .lblStoreConfirmation.Visible = False
                    .Btn_CalcPad_small1.Visible = True
                Else
                    .lblMsg.Visible = False
                    .lblConfirmMessage.Visible = True
                    .btnConfirm.Visible = True
                    .btnNoBadge.Visible = False
                    .txtProductCode.Visible = False
                    .lblDrvrConfrm.Visible = False
                    .lblStoreConfirmation.Visible = True
                    .Btn_CalcPad_small1.Visible = False
                End If
                '      m_BDUODDrvrConfrmation.lstvwBatch.EndUpdate()
                .Visible = True
                'Setting the active screen as Batch Driver Confirmation screen
                objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BATCHDRVRCONFRM
                m_BDUODDrvrConfrmation.txtProductCode.Text = ""
                m_BDUODDrvrConfrmation.txtProductCode.Focus()
                .Refresh()
            End With
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Driver Confirmation Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Method to display the Final driver confirmation screen.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayFinalDrvrCnfrm(ByVal o As Object, ByVal e As EventArgs)
        Try
            objAppContainer.objStatusBar.SetMessage("")
            With m_BDUODFinalDrvrConfirmation
                Dim bBooked As Boolean = False

                'To add the deliveries which are Booked In to the listview.
                ' m_BDUODFinalDrvrConfirmation.lvwBookedIn.BeginUpdate()
                m_BDUODFinalDrvrConfirmation.lvwBookedIn.Items.Clear()
                'To add the deliveries which are remaining to be Booked In in to the listview.
                '  m_BDUODFinalDrvrConfirmation.lvwRemainingToday.BeginUpdate()
                m_BDUODFinalDrvrConfirmation.lvwRemainingToday.Items.Clear()
                CalculateBookInCount()
                'To display the Booked in UOD numbers
                If (m_objExpectedScanned.TotalUOD + m_objOutstandingScanned.TotalUOD + m_objFutureScanned.TotalUOD) > 0 Or (m_arrDalScanBatch.Count) > 0 Then
                    If (m_objExpectedScanned.TotalDolly + m_objOutstandingScanned.TotalDolly + m_objFutureScanned.TotalDolly) > 0 Then
                        .lvwBookedIn.Items.Add( _
                         (ContainerName.Dolly.PadRight(Macros.BLEN, " ") + (m_objExpectedScanned.TotalDolly + m_objOutstandingScanned.TotalDolly + m_objFutureScanned.TotalDolly).ToString()))
                    End If
                    If (m_iFinalExpectedCrateCount + m_iFinalOutCrateCount + m_iFinalFutureCrateCount) > 0 Then
                        .lvwBookedIn.Items.Add( _
                         (ContainerName.Crate.PadRight(Macros.BLEN, " ") + (m_iFinalExpectedCrateCount + m_iFinalOutCrateCount + m_iFinalFutureCrateCount).ToString()))
                    End If
                    If (m_objExpectedScanned.TotalPallet + m_objOutstandingScanned.TotalPallet + m_objFutureScanned.TotalPallet) > 0 Then
                        .lvwBookedIn.Items.Add( _
                         (ContainerName.Pallet.PadRight(Macros.BLEN, " ") + (m_objExpectedScanned.TotalPallet + m_objOutstandingScanned.TotalPallet + m_objFutureScanned.TotalPallet).ToString()))
                    End If
                    ' V1.1 - CK
                    ' Adding booked in Dallas Summary
                    If (m_iExpectedDalFinalCount + m_iOutstandingDalFinalCount + m_iFutureDalFinalCount) > 0 Then
                        .lvwBookedIn.Items.Add( _
                        (ContainerName.Burton.PadRight(Macros.BLEN, " ") + (m_iExpectedDalFinalCount + m_iOutstandingDalFinalCount + m_iFutureDalFinalCount).ToString()))
                    End If
                    If (m_objExpectedScanned.TotalRollCage + m_objOutstandingScanned.TotalRollCage + m_objFutureScanned.TotalRollCage) > 0 Then
                        .lvwBookedIn.Items.Add( _
                         (ContainerName.Rollcage.PadRight(Macros.BLEN, " ") + (m_objExpectedScanned.TotalRollCage + m_objOutstandingScanned.TotalRollCage + +m_objFutureScanned.TotalRollCage).ToString()))
                    End If
                    If (m_objExpectedScanned.TotalOuter + m_objOutstandingScanned.TotalOuter + m_objFutureScanned.TotalOuter) > 0 Then
                        .lvwBookedIn.Items.Add( _
                         (ContainerName.Outer.PadRight(Macros.BLEN, " ") + (m_objExpectedScanned.TotalOuter + m_objOutstandingScanned.TotalOuter + m_objFutureScanned.TotalOuter).ToString()))
                    End If
                    If (m_objExpectedScanned.TotalIST + m_objOutstandingScanned.TotalIST + m_objFutureScanned.TotalIST) > 0 Then
                        .lvwBookedIn.Items.Add( _
                        (ContainerName.IST.PadRight(Macros.BLEN, " ") + (m_objExpectedScanned.TotalIST + m_objOutstandingScanned.TotalIST + +m_objFutureScanned.TotalIST).ToString()))
                    End If

                    bBooked = True
                End If
                'to Display the Not on file Booked in UODs numbers
                If m_objMisdirectedScanned.TotalUOD > 0 Or m_iNotOnFileDalFinalCount > 0 Then
                    If bBooked Then
                        .lvwBookedIn.Items.Add("------------------------------------------------------------------------------------------------")
                    End If
                    .lvwBookedIn.Items.Add("Not on File")
                    If m_objMisdirectedScanned.TotalDolly > 0 Then
                        .lvwBookedIn.Items.Add( _
                         (ContainerName.Dolly.PadRight(Macros.BLEN, " ") + m_objMisdirectedScanned.TotalDolly.ToString()))
                    End If
                    If m_objMisdirectedScanned.TotalCrate > 0 Then
                        .lvwBookedIn.Items.Add( _
                         (ContainerName.Crate.PadRight(Macros.BLEN, " ") + m_objMisdirectedScanned.TotalCrate.ToString()))
                    End If
                    If m_objMisdirectedScanned.TotalPallet > 0 Then
                        .lvwBookedIn.Items.Add( _
                         (ContainerName.Pallet.PadRight(Macros.BLEN, " ") + m_objMisdirectedScanned.TotalPallet.ToString()))
                    End If
                    ' V1.1 - CK
                    ' Adding booked in 'not on file' Dallas UODs number
                    If m_iNotOnFileDalFinalCount > 0 Then
                        .lvwBookedIn.Items.Add( _
                        (ContainerName.Burton.PadRight(Macros.BLEN, " ") + m_iNotOnFileDalFinalCount.ToString()))
                    End If
                    If m_objMisdirectedScanned.TotalRollCage > 0 Then
                        .lvwBookedIn.Items.Add( _
                         (ContainerName.Rollcage.PadRight(Macros.BLEN, " ") + m_objMisdirectedScanned.TotalRollCage.ToString()))
                    End If
                    If m_objMisdirectedScanned.TotalOuter > 0 Then
                        .lvwBookedIn.Items.Add( _
                         (ContainerName.Outer.PadRight(Macros.BLEN, " ") + m_objMisdirectedScanned.TotalOuter.ToString()))
                    End If
                    If m_objMisdirectedScanned.TotalIST > 0 Then
                        .lvwBookedIn.Items.Add( _
                        (ContainerName.IST.PadRight(Macros.BLEN, " ") + m_objMisdirectedScanned.TotalIST.ToString()))
                    End If

                End If

                'To add the Remaining deliveries which are not booked in from expected deliveries for that day
                ' If (m_objExpected.TotalUOD - m_objExpectedScanned.TotalUOD) > 0 Then
                If (m_objExpected.TotalDolly - m_objExpectedScanned.TotalDolly) > 0 Then
                    .lvwRemainingToday.Items.Add( _
                     (ContainerName.Dolly.PadRight(Macros.BLEN, " ") + (m_objExpected.TotalDolly - m_objExpectedScanned.TotalDolly).ToString()))
                End If
                If (m_objExpected.TotalCrate - m_objExpectedScanned.TotalCrate) > 0 Then
                    .lvwRemainingToday.Items.Add( _
                     (ContainerName.Crate.PadRight(Macros.BLEN, " ") + (m_objExpected.TotalCrate - m_objExpectedScanned.TotalCrate).ToString()))
                End If
                If (m_objExpected.TotalPallet - m_objExpectedScanned.TotalPallet) > 0 Then
                    .lvwRemainingToday.Items.Add( _
                     (ContainerName.Pallet.PadRight(Macros.BLEN, " ") + (m_objExpected.TotalPallet - m_objExpectedScanned.TotalPallet).ToString()))
                End If
                ' V1.1 - CK
                ' Adding remaining Dallas deliveries
                If (m_objExpected.TotalBurton - m_iExpectedDalFinalCount) > 0 Then
                    .lvwRemainingToday.Items.Add( _
                    (ContainerName.Burton.PadRight(Macros.BLEN, " ") + (m_objExpected.TotalBurton - m_iExpectedDalFinalCount).ToString()))
                End If
                If (m_objExpected.TotalRollCage - m_objExpectedScanned.TotalRollCage) > 0 Then
                    .lvwRemainingToday.Items.Add( _
                     (ContainerName.Rollcage.PadRight(Macros.BLEN, " ") + (m_objExpected.TotalRollCage - m_objExpectedScanned.TotalRollCage).ToString()))
                End If
                If (m_objExpected.TotalOuter - m_objExpectedScanned.TotalOuter) > 0 Then
                    .lvwRemainingToday.Items.Add( _
                     (ContainerName.Outer.PadRight(Macros.BLEN, " ") + (m_objExpected.TotalOuter - m_objExpectedScanned.TotalOuter).ToString()))
                End If
                If (m_objExpected.TotalIST - m_objExpectedScanned.TotalIST) > 0 Then
                    .lvwRemainingToday.Items.Add( _
                    (ContainerName.IST.PadRight(Macros.BLEN, " ") + (m_objExpected.TotalIST - m_objExpectedScanned.TotalIST).ToString()))
                End If


                'Checking whether the deliveries has to be booked in at In hours or Out of hours.
                If bIsInHours Then
                    .lblMsg.Visible = True
                    .txtProductCode.Visible = True
                    .btnConfirm.Visible = False
                    .btnNoBadge.Visible = True
                    .lblConfirmMessage.Visible = False
                    .lblFinalConfrmtn.Visible = True
                    .lblConfirmation.Visible = False
                    .Btn_CalcPad_small1.Visible = True
                Else
                    .lblMsg.Visible = False
                    .txtProductCode.Visible = False
                    .btnConfirm.Visible = True
                    .btnNoBadge.Visible = False
                    .lblConfirmMessage.Visible = True
                    .lblFinalConfrmtn.Visible = False
                    .lblConfirmation.Visible = True
                    .Btn_CalcPad_small1.Visible = False
                End If
                .Visible = True
                'Setting the active screen as Final Driver Confirmation screen
                objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.FINALDRVRCNFRM
                'Claering the textbox after scanning the barcode.
                m_BDUODFinalDrvrConfirmation.txtProductCode.Text = ""
                m_BDUODFinalDrvrConfirmation.txtProductCode.Focus()

                .Refresh()
            End With
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Final Driver Confirmation Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Method to display the Book In Delivery Summary Screen when the quit button is selected from the Driver Confirmation Screen.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayBookInSummary(ByVal o As Object, ByVal e As EventArgs)
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            With m_BDUODSummary
                m_BDUODSummary.lvwExpectedSummary.BeginUpdate()
                Dim strContainer As String = ""
                For Each objDeliverySummary As GIValueHolder.DeliverySummary In m_ItemList
                    'To Populate the Expected Deliveries in the Summary Screen.
                    If objDeliverySummary.SummaryType = Macros.EXPECTED Then
                        Select Case objDeliverySummary.ContainerType
                            Case ContainerType.Dolly
                                strContainer = ContainerName.Dolly
                                m_objExpected.TotalDolly = objDeliverySummary.ContainerQty
                            Case ContainerType.Crate
                                strContainer = ContainerName.Crate
                                m_objExpected.TotalCrate = objDeliverySummary.ContainerQty
                            Case ContainerType.Pallet
                                strContainer = ContainerName.Pallet
                                m_objExpected.TotalPallet = objDeliverySummary.ContainerQty
                            Case ContainerType.RollCage
                                strContainer = ContainerName.Rollcage
                                m_objExpected.TotalRollCage = objDeliverySummary.ContainerQty
                            Case ContainerType.Outer
                                strContainer = ContainerName.Outer
                                m_objExpected.TotalOuter = objDeliverySummary.ContainerQty
                            Case ContainerType.IST
                                strContainer = ContainerName.IST
                                m_objExpected.TotalIST = objDeliverySummary.ContainerQty

                        End Select
                        'to display only Containers which have expected deliveries greater than 0
                        m_objExpected.TotalUOD += objDeliverySummary.ContainerQty

                    End If
                Next
                'Adding the items to the expected list view
                If m_objExpected.TotalDolly > 0 Then
                    .lvwExpectedSummary.Items.Add((New ListViewItem(New String() {ContainerName.Dolly, m_objExpected.TotalDolly})))
                End If
                If m_objExpected.TotalCrate > 0 Then
                    .lvwExpectedSummary.Items.Add((New ListViewItem(New String() {ContainerName.Crate, m_objExpected.TotalCrate})))
                End If

                If m_objExpected.TotalPallet > 0 Then
                    .lvwExpectedSummary.Items.Add((New ListViewItem(New String() {ContainerName.Pallet, m_objExpected.TotalPallet})))
                End If
                ' V1.1 - CK
                ' Adding Burton item to the expected list view
                If m_objExpected.TotalBurton > 0 Then
                    .lvwExpectedSummary.Items.Add((New ListViewItem(New String() {ContainerName.Burton, m_objExpected.TotalBurton})))
                End If
                If m_objExpected.TotalRollCage > 0 Then
                    .lvwExpectedSummary.Items.Add((New ListViewItem(New String() {ContainerName.Rollcage, m_objExpected.TotalRollCage})))
                End If
                If m_objExpected.TotalOuter > 0 Then
                    .lvwExpectedSummary.Items.Add((New ListViewItem(New String() {ContainerName.Outer, m_objExpected.TotalOuter})))
                End If
                If m_objExpected.TotalIST > 0 Then
                    .lvwExpectedSummary.Items.Add((New ListViewItem(New String() {ContainerName.IST, m_objExpected.TotalIST})))
                End If
                m_BDUODSummary.lvwExpectedSummary.EndUpdate()
                .Visible = True
                'Setting the active screen as Book In Summary screen
                objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINSUMMARY
                .Refresh()
            End With
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Summary Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Screen Display method for Book In Delivery 
    ''' All sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DisplayBDScreen(ByVal ScreenName As BDSCREENS) As Boolean
        Try
            Select Case ScreenName
                Case BDSCREENS.BDUODHome
                    m_BDUODHome.Invoke(New EventHandler(AddressOf DisplayBDScan))
                Case BDSCREENS.BDUODInitialSummary
                    m_BDUODInitialSummary.Invoke(New EventHandler(AddressOf DisplayInitialSummary))
                Case BDSCREENS.BDUODScan
                    m_BDUODScan.Invoke(New EventHandler(AddressOf DisplayBookInUODScan))
                Case BDSCREENS.BDUODBatchDrvrConfrm
                    m_BDUODDrvrConfrmation.Invoke(New EventHandler(AddressOf DisplayDrvrConfirmation))
                Case BDSCREENS.BDUODFinalDrvrCnfrm
                    m_BDUODFinalDrvrConfirmation.Invoke(New EventHandler(AddressOf DisplayFinalDrvrCnfrm))
                Case BDSCREENS.BDUODSummary
                    m_BDUODSummary.Invoke(New EventHandler(AddressOf DisplayBookInSummary))
            End Select
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Display Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' Method to Populate the listview's for the Expected and Outstanding deliveries.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PopulateExpectedList()
        ' V1.1 - CK
        ' Declaring iDateCompare, dtToday & dtExpectedDelDate
        Dim iDateCompare As Integer
        Dim dtToday As DateTime
        Dim dtExpectedDelDate As DateTime
        With m_BDUODInitialSummary
            Try
                For Each objDeliverySummary As GIValueHolder.DeliverySummary In m_ItemList
                    'Checking whether the summary type is Expected or Outstanding delivery.
                    If objDeliverySummary.SummaryType = Macros.OUTSTANDING Then
                        Dim strContainer As String = ""
                        Select Case objDeliverySummary.ContainerType
                            Case ContainerType.Dolly
                                strContainer = ContainerName.Dolly
                                m_objOutstanding.TotalDolly = objDeliverySummary.ContainerQty
                            Case ContainerType.Crate
                                strContainer = ContainerName.Crate
                                m_objOutstanding.TotalCrate = objDeliverySummary.ContainerQty
                            Case ContainerType.Pallet
                                strContainer = ContainerName.Pallet
                                m_objOutstanding.TotalPallet = objDeliverySummary.ContainerQty
                            Case ContainerType.RollCage
                                strContainer = ContainerName.Rollcage
                                m_objOutstanding.TotalRollCage = objDeliverySummary.ContainerQty
                            Case ContainerType.Outer
                                strContainer = ContainerName.Outer
                                m_objOutstanding.TotalOuter = objDeliverySummary.ContainerQty
                            Case ContainerType.IST
                                strContainer = ContainerName.IST
                                m_objOutstanding.TotalIST = objDeliverySummary.ContainerQty
                        End Select
                        'to show only UODs with container quantity greater than 0
                        m_objOutstanding.TotalUOD += objDeliverySummary.ContainerQty


                    Else
                        Dim strContainer As String = ""
                        Select Case objDeliverySummary.ContainerType
                            Case ContainerType.Dolly
                                strContainer = ContainerName.Dolly
                                m_objExpected.TotalDolly = objDeliverySummary.ContainerQty
                            Case ContainerType.Crate
                                strContainer = ContainerName.Crate
                                m_objExpected.TotalCrate = objDeliverySummary.ContainerQty
                            Case ContainerType.Pallet
                                strContainer = ContainerName.Pallet
                                m_objExpected.TotalPallet = objDeliverySummary.ContainerQty
                            Case ContainerType.RollCage
                                strContainer = ContainerName.Rollcage
                                m_objExpected.TotalRollCage = objDeliverySummary.ContainerQty
                            Case ContainerType.Outer
                                strContainer = ContainerName.Outer
                                m_objExpected.TotalOuter = objDeliverySummary.ContainerQty
                            Case ContainerType.IST
                                strContainer = ContainerName.IST
                                m_objExpected.TotalIST = objDeliverySummary.ContainerQty

                        End Select
                        'to show only UODs with container quantity greater than 0
                        m_objExpected.TotalUOD += objDeliverySummary.ContainerQty

                    End If
                Next

                ' V1.1 - CK
                ' Getting the count of Expected and Outstanding Dallas delivery for stores
                ' that are enabled for Dallas positive receiving.
                If objAppContainer.bDallasPosReceiptEnabled Then
                    For Each objDallasDeliverySummary As GIValueHolder.DallasDeliverySummary In m_DALLASItemList
                        If objDallasDeliverySummary.Status = Macros.UNRECEIPTED Then
                            dtToday = DateTime.Now.Date
                            dtExpectedDelDate = DateTime.ParseExact(objDallasDeliverySummary.ExpectedDelDate, "yyMMdd", CultureInfo.InvariantCulture)
                            iDateCompare = DateTime.Compare(dtExpectedDelDate, dtToday)
                            If iDateCompare < 0 Then
                                ' If expected delivery date is earlier than today's
                                ' date then increment the count for outstanding
                                m_objOutstanding.TotalBurton += 1
                                m_objOutstanding.TotalUOD += 1
                            ElseIf iDateCompare = 0 Then
                                ' If expected delivery date is today's date then
                                ' increment the count for expected
                                m_objExpected.TotalBurton += 1
                                m_objExpected.TotalUOD += 1
                            ElseIf iDateCompare > 0 Then
                                ' If expected delivery date is later than today's
                                ' date then increment the count for future
                                m_objFuture.TotalBurton += 1
                            End If
                        End If
                    Next
                End If


                'Adding the items to the expected list view
                If m_objExpected.TotalDolly > 0 Then
                    .lvwExpected.Items.Add((New ListViewItem(New String() {ContainerName.Dolly, m_objExpected.TotalDolly})))
                End If
                If m_objExpected.TotalCrate > 0 Then
                    .lvwExpected.Items.Add((New ListViewItem(New String() {ContainerName.Crate, m_objExpected.TotalCrate})))
                End If

                If m_objExpected.TotalPallet > 0 Then
                    .lvwExpected.Items.Add((New ListViewItem(New String() {ContainerName.Pallet, m_objExpected.TotalPallet})))
                End If
                ' V1.1 - CK
                ' Adding Dallas items to the expected list view
                If m_objExpected.TotalBurton > 0 Then
                    .lvwExpected.Items.Add((New ListViewItem(New String() {ContainerName.Burton, m_objExpected.TotalBurton})))
                End If
                If m_objExpected.TotalRollCage > 0 Then
                    .lvwExpected.Items.Add((New ListViewItem(New String() {ContainerName.Rollcage, m_objExpected.TotalRollCage})))
                End If
                If m_objExpected.TotalOuter > 0 Then
                    .lvwExpected.Items.Add((New ListViewItem(New String() {ContainerName.Outer, m_objExpected.TotalOuter})))
                End If
                If m_objExpected.TotalIST > 0 Then
                    .lvwExpected.Items.Add((New ListViewItem(New String() {ContainerName.IST, m_objExpected.TotalIST})))
                End If


                'Adding the items to the outsanding list view
                If m_objOutstanding.TotalDolly > 0 Then
                    .lvwOutstanding.Items.Add((New ListViewItem(New String() {ContainerName.Dolly, m_objOutstanding.TotalDolly})))
                End If

                If m_objOutstanding.TotalCrate > 0 Then
                    .lvwOutstanding.Items.Add((New ListViewItem(New String() {ContainerName.Crate, m_objOutstanding.TotalCrate})))
                End If

                If m_objOutstanding.TotalPallet > 0 Then
                    .lvwOutstanding.Items.Add((New ListViewItem(New String() {ContainerName.Pallet, m_objOutstanding.TotalPallet})))
                End If
                ' V1.1 - CK
                ' Adding Dallas items to the outstanding list view
                If m_objOutstanding.TotalBurton > 0 Then
                    .lvwOutstanding.Items.Add((New ListViewItem(New String() {ContainerName.Burton, m_objOutstanding.TotalBurton})))
                End If
                If m_objOutstanding.TotalRollCage > 0 Then
                    .lvwOutstanding.Items.Add((New ListViewItem(New String() {ContainerName.Rollcage, m_objOutstanding.TotalRollCage})))
                End If
                If m_objOutstanding.TotalOuter > 0 Then
                    .lvwOutstanding.Items.Add((New ListViewItem(New String() {ContainerName.Outer, m_objOutstanding.TotalOuter})))
                End If
                If m_objOutstanding.TotalIST > 0 Then
                    .lvwOutstanding.Items.Add((New ListViewItem(New String() {ContainerName.IST, m_objOutstanding.TotalIST})))
                End If

                'To Check whether there are any expected or outstanding deliveries and accordingly enable and disable the appropriate labels.
                If m_BDUODInitialSummary.lvwExpected.Items.Count = 0 Then
                    m_BDUODInitialSummary.lvwExpected.Visible = False
                    m_BDUODInitialSummary.lblNoExpected.Visible = True
                    m_BDUODInitialSummary.lblExpected.Visible = False
                End If
                If m_BDUODInitialSummary.lvwOutstanding.Items.Count = 0 Then
                    m_BDUODInitialSummary.lvwOutstanding.Visible = False
                    m_BDUODInitialSummary.lblNoOutstanding.Visible = True
                    m_BDUODInitialSummary.lblOutstanding.Visible = False
                End If

            Catch ex As Exception
                'Add the exception to the application log.
                AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Population of Expected nad Outstanding List: " + ex.ToString(), Logger.LogLevel.RELEASE)
            End Try
        End With
    End Sub
    ''' <summary>
    ''' Enum Class that defines all screens for Book In Delivery module
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum BDSCREENS
        BDUODHome
        BDUODInitialSummary
        BDUODScan
        BDUODBatchDrvrConfrm
        BDUODFinalDrvrCnfrm
        BDUODSummary
    End Enum
    Private Function GetContainerName(ByVal strUODNUmber As String) As String
        Dim strContainer As String = ""
        Select Case strUODNUmber.Chars(0)
            Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.DOLLYID)
                strContainer = ContainerNam.Dolly

            Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.CRATEID)
                strContainer = ContainerNam.Crate

            Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.PALLETSID)
                strContainer = ContainerNam.Pallet

            Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.ROLLYCAGEID)
                strContainer = ContainerNam.Rollcage

            Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.OUTERSID)
                strContainer = ContainerNam.Outer
        End Select
        Return strContainer
    End Function
    ''' <summary>
    ''' Method to revert the count of deliveries when the RESCAN button is selected from the driver confirmation screen.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RescanBatch()
        Try
            Dim iPartialDollyOutsChildCount As Integer = 0
            Dim iPartialDollyExpChildCount As Integer = 0
            Dim iResult As Integer = 0
            BDSessionMgr.GetInstance().bIsFinished = False
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M78"), "Alert", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            If iResult = MsgBoxResult.Ok Then
                'To Reset the Count of Expected and outstanding deliveries.

                m_arrScannedUODBatch.Clear()
                m_arrMisdirectReturnUODBatch.Clear()
                m_arrBatchPartialDetails.Clear()
                m_objExpectedBatch.ResetCount()
                m_objOutStandingBatch.ResetCount()
                'CR for Future UOD
                m_objFutureBatch.ResetCount()
                m_objMisdirectedBatch.ResetCount()
                m_objMisdirectedReturnBatch.ResetCount()
                m_arrBatchParentDetails.Clear()
                'To Send the driver details and batch details to the EPOS Controller
                Dim objDriverScanDetails As New GIValueHolder.DriverDetails
                If strBadgeID.Equals("") Then
                    objDriverScanDetails.DriverBadge = Macros.NOBADGE
                Else
                    objDriverScanDetails.DriverBadge = strBadgeID
                End If
                objDriverScanDetails.ScanDate = Format(DateTime.Now, "yyyyMMdd")
                objDriverScanDetails.ScanTime = Format(DateTime.Now, "HHmmss")
                objDriverScanDetails.ScanLevel = m_UODInfo.UODType
                objDriverScanDetails.BatchRescan = "Y"
                'V1.1 - KK
                'Added check to prevent Driver Badge details being send across for only Dallas Delivery scan session
                If Not objAppContainer.bDallasPosReceiptEnabled Or (objAppContainer.bDallasPosReceiptEnabled And m_arrListBatch.Count > 0) Then
#If RF Then
                    If objAppContainer.objDataEngine.SendBookInDetails(m_arrListBatch) Then
                        If objAppContainer.objDataEngine.SendBatchConfirmation(objDriverScanDetails) Then
#ElseIf NRF Then
                    objAppContainer.objDataEngine.SendBookInDetails(m_arrListBatch)
                    objAppContainer.objDataEngine.SendBatchConfirmation(objDriverScanDetails)
#End If

                            m_arrListBatch.Clear()
                            m_objExpectedScanned.ResetCount()
                            m_objOutstandingScanned.ResetCount()
                            m_objFutureScanned.ResetCount()

                            RevertToRollbackPoint()
                            If Not (m_arrDalScanBatch.Count > 0) Then
                                BDSessionMgr.GetInstance.DisplayBDScreen(BDSessionMgr.BDSCREENS.BDUODScan)
                            End If
#If RF Then
                        End If
                    End If
#End If
                End If
                If objAppContainer.bDallasPosReceiptEnabled And m_arrDalScanBatch.Count > 0 Then
                    'V1.1 KK
                    'Clearing all Dallas UOD Data for ReScan Processing
                    m_objExpectedScanned.ResetCount()
                    m_objOutstandingScanned.ResetCount()
                    m_objFutureScanned.ResetCount()
                    m_arrDalScanBatch.Clear()
                    arrDallasScannedUOD.Clear()
                    m_arrDallasConfirmedUOD.Clear()
                    m_arrDallasBankedUOD.Clear()
                    m_arrDallasMisdirectRScanned.Clear()
                    m_objMisdirectedScanned.ResetCount()
                    m_objMisdirectedReturnScanned.ResetCount()
                    ' V1.1 - CK
                    ' Clearing the Scanned Dallas Counts
                    m_iExpectedDalFinalCount = 0
                    m_iFutureDalFinalCount = 0
                    m_iOutstandingDalFinalCount = 0
                    m_iNotOnFileDalFinalCount = 0
                    m_iMisdirectReturnedDalFinalCount = 0
                    BDSessionMgr.GetInstance.DisplayBDScreen(BDSessionMgr.BDSCREENS.BDUODScan)
                End If
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Rescan Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To display the Driver Id during Book In Delivery In Hours in Driver Confiramtion Screen.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub NoBadge()

        If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BATCHDRVRCONFRM Then
            m_BDUODDrvrConfrmation.Enabled = True
            DisplayBDScreen(BDSCREENS.BDUODBatchDrvrConfrm)
        ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.FINALDRVRCNFRM Then
            m_BDUODFinalDrvrConfirmation.Enabled = True
            DisplayBDScreen(BDSCREENS.BDUODFinalDrvrCnfrm)
        End If

        'Set the default drive id
        strBadgeID = Macros.NOBADGE
        Me.ConfirmBatch()
    End Sub
    ''' <summary>
    ''' To display the Driver Id during Book In Delivery In Hours in Final Driver Confiramtion Screen.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub NoBadgeSessionConfirm()
        If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.FINALDRVRCNFRM Then
            m_BDUODFinalDrvrConfirmation.Enabled = True
            DisplayBDScreen(BDSCREENS.BDUODFinalDrvrCnfrm)
        End If
        strBadgeID = Macros.NOBADGE
        Me.ConfirmDelivery()
    End Sub
    ''' <summary>
    ''' To display the Driver Id during Book In Delivery Out Of Hours in Driver Confiramtion Screen.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub OutOfHoursBatchConfirm()
        'set the default driver id here 
        strBadgeID = Macros.OUTOFHOURS
        Me.ConfirmBatch()
    End Sub
    ''' <summary>
    ''' To display the Driver Id during Book In Delivery Out Of Hours in  Final Driver Confiramtion Screen.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub OutOfHoursFinalConfirm()
        strBadgeID = Macros.OUTOFHOURS
        Me.ConfirmDelivery()
    End Sub
    ''' <summary>
    ''' Method to Confirm the batch during Book In Delivery Out Of Hours.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ConfirmBatch()
        Try
            ' V1.1 - KK
            ' Change to incorporate the check to prevent sending book in details & Batch confirmation for Dallas UOD
            If Not objAppContainer.bDallasPosReceiptEnabled Or (objAppContainer.bDallasPosReceiptEnabled And m_arrListBatch.Count > 0) Then
                Dim objDriverScanDetails As New GIValueHolder.DriverDetails
                objDriverScanDetails.DriverBadge = strBadgeID
                objDriverScanDetails.ScanDate = Format(DateTime.Now, "yyyyMMdd")
                objDriverScanDetails.ScanTime = Format(DateTime.Now, "HHmmss")
                objDriverScanDetails.ScanLevel = m_UODInfo.UODType
                objDriverScanDetails.BatchRescan = "N"
                If bGITNote AndAlso Not bIsInHours Then
                    objDriverScanDetails.GITNote = "Y"
                ElseIf Not bGITNote AndAlso Not bIsInHours Then
                    objDriverScanDetails.GITNote = "N"
                Else
                    objDriverScanDetails.GITNote = " "
                End If

#If RF Then
                If objAppContainer.objDataEngine.SendBookInDetails(m_arrListBatch) Then
                    If objAppContainer.objDataEngine.SendBatchConfirmation(objDriverScanDetails) Then
#ElseIf NRF Then
            objAppContainer.objDataEngine.SendBookInDetails(m_arrListBatch)
            objAppContainer.objDataEngine.SendBatchConfirmation(objDriverScanDetails)
#End If

                        'V1.1 - KK
                        'Added to flag that both Dallas and Non Dallas UOD is scanned in session
                        bIsUODCombination = True
                        m_arrListBatch.Clear()
                        m_objExpectedScanned.TotalCrate += m_objExpectedBatch.TotalCrate
                        m_iFinalExpectedCrateCount += m_objExpectedBatch.TotalCrate
                        m_objExpectedScanned.TotalDolly += m_objExpectedBatch.TotalDolly
                        m_objExpectedScanned.TotalOuter += m_objExpectedBatch.TotalOuter
                        m_objExpectedScanned.TotalPallet += m_objExpectedBatch.TotalPallet
                        m_objExpectedScanned.TotalRollCage += m_objExpectedBatch.TotalRollCage
                        m_objExpectedScanned.TotalIST += m_objExpectedBatch.TotalIST
                        m_objExpectedScanned.TotalUOD += m_objExpectedBatch.TotalUOD

                        m_objFutureScanned.TotalCrate += m_objFutureBatch.TotalCrate
                        m_iFinalFutureCrateCount += m_objFutureBatch.TotalCrate
                        m_objFutureScanned.TotalDolly += m_objFutureBatch.TotalDolly
                        m_objFutureScanned.TotalOuter += m_objFutureBatch.TotalOuter
                        m_objFutureScanned.TotalPallet += m_objFutureBatch.TotalPallet
                        m_objFutureScanned.TotalRollCage += m_objFutureBatch.TotalRollCage
                        m_objFutureScanned.TotalIST += m_objFutureBatch.TotalIST
                        m_objFutureScanned.TotalUOD += m_objFutureBatch.TotalUOD

                        m_objOutstandingScanned.TotalCrate += m_objOutStandingBatch.TotalCrate
                        m_iFinalOutCrateCount += m_objOutStandingBatch.TotalCrate
                        m_objOutstandingScanned.TotalDolly += m_objOutStandingBatch.TotalDolly
                        m_objOutstandingScanned.TotalOuter += m_objOutStandingBatch.TotalOuter
                        m_objOutstandingScanned.TotalPallet += m_objOutStandingBatch.TotalPallet
                        m_objOutstandingScanned.TotalRollCage += m_objOutStandingBatch.TotalRollCage
                        m_objOutstandingScanned.TotalIST += m_objOutStandingBatch.TotalIST
                        m_objOutstandingScanned.TotalUOD += m_objOutStandingBatch.TotalUOD

                        m_objMisdirectedScanned.TotalCrate += m_objMisdirectedBatch.TotalCrate
                        m_objMisdirectedScanned.TotalDolly += m_objMisdirectedBatch.TotalDolly
                        m_objMisdirectedScanned.TotalOuter += m_objMisdirectedBatch.TotalOuter
                        m_objMisdirectedScanned.TotalPallet += m_objMisdirectedBatch.TotalPallet
                        m_objMisdirectedScanned.TotalRollCage += m_objMisdirectedBatch.TotalRollCage
                        m_objMisdirectedScanned.TotalIST += m_objMisdirectedBatch.TotalIST
                        m_objMisdirectedScanned.TotalUOD += m_objMisdirectedBatch.TotalUOD

                        m_objMisdirectedReturnScanned.TotalCrate += m_objMisdirectedReturnBatch.TotalCrate
                        m_objMisdirectedReturnScanned.TotalDolly += m_objMisdirectedReturnBatch.TotalDolly
                        m_objMisdirectedReturnScanned.TotalOuter += m_objMisdirectedReturnBatch.TotalOuter
                        m_objMisdirectedReturnScanned.TotalPallet += m_objMisdirectedReturnBatch.TotalPallet
                        m_objMisdirectedReturnScanned.TotalRollCage += m_objMisdirectedReturnBatch.TotalRollCage
                        m_objMisdirectedReturnScanned.TotalIST += m_objMisdirectedReturnBatch.TotalIST
                        m_objMisdirectedReturnScanned.TotalUOD += m_objMisdirectedReturnBatch.TotalUOD

                        For Each strCode As String In m_arrScannedUODBatch
                            m_arrScannedUOD.Add(strCode)
                        Next

                        m_arrScannedUODBatch.Clear()
                        m_arrMisdirectReturnUODBatch.Clear()
                        For Each objParent As UODParentDetails In m_arrBatchParentDetails
                            Dim bObjFound As Boolean = False
                            For Each objParentSession As UODParentDetails In m_arrParentDetails
                                If objParentSession.UODParent = objParent.UODParent Then
                                    objParentSession.UODScannedCount = objParent.UODScannedCount
                                    bObjFound = True
                                    Exit For

                                End If
                            Next
                            If Not bObjFound Then
                                m_arrParentDetails.Add(objParent)
                            End If
                        Next
                        For Each objParent As UODParentDetails In m_arrBatchPartialDetails
                            Dim bObjFound As Boolean = False
                            For Each objParentSession As UODParentDetails In m_arrPartialDetails
                                If objParentSession.UODParent = objParent.UODParent Then
                                    objParentSession.UODScannedCount = objParent.UODScannedCount
                                    bObjFound = True
                                    Exit For

                                End If
                            Next
                            If Not bObjFound Then
                                m_arrPartialDetails.Add(objParent)
                            End If
                        Next
                        'V1.1 - KK
                        'Added to flag that both Dallas and Non Dallas UOD is scanned in session
                        bIsUODCombination = True
                        m_arrBatchParentDetails.Clear()
                        m_arrBatchPartialDetails.Clear()
                        m_objExpectedBatch.ResetCount()
                        m_objOutStandingBatch.ResetCount()
                        'CR for Future UOD
                        m_objFutureBatch.ResetCount()
                        m_objMisdirectedBatch.ResetCount()
                        m_objMisdirectedReturnBatch.ResetCount()
                        'Clear array list
                        'send the driver info
                        If bIsFinished Then
                            DisplayBDScreen(BDSCREENS.BDUODFinalDrvrCnfrm)
                        Else
#If RF Then
                            objAppContainer.m_ModScreen = AppContainer.ModScreen.PREFINISH
#End If
                            DisplayBDScreen(BDSCREENS.BDUODScan)
                        End If
#If RF Then
                    End If
                End If
#End If

            End If
            ' V1.1 - KK
            ' If Dallas UOD's are scanned then process the below logic
            If objAppContainer.bDallasPosReceiptEnabled And (m_arrDalScanBatch.Count > 0) Then
                If bIsFinished Then
                    m_arrDallasConfirmedUOD.Clear()
                    ' V1.1 - CK
                    ' Incrementing the count of scanned Dallas deliveries to display over the
                    ' final driver confirmation screen
                    m_iExpectedDalFinalCount += m_objExpectedScanned.TotalBurton
                    m_iOutstandingDalFinalCount += m_objOutstandingScanned.TotalBurton
                    m_iFutureDalFinalCount += m_objFutureScanned.TotalBurton
                    m_iNotOnFileDalFinalCount += m_objMisdirectedScanned.TotalBurton
                    m_iMisdirectReturnedDalFinalCount += m_objMisdirectedReturnScanned.TotalBurton
                    ' V1.1 - CK
                    ' Clearing the count of scanned Burton UODs
                    ' from m_objExpectedScanned, m_objOutstandingScanned,
                    ' m_objFutureScanned, m_objMisdirectedScanned &
                    ' m_objMisdirectedReturnScanned
                    m_objExpectedScanned.TotalBurton = 0
                    m_objOutstandingScanned.TotalBurton = 0
                    m_objFutureScanned.TotalBurton = 0
                    m_objMisdirectedScanned.TotalBurton = 0
                    m_objMisdirectedReturnScanned.TotalBurton = 0
                    DisplayBDScreen(BDSCREENS.BDUODFinalDrvrCnfrm)
                Else
#If RF Then
                    objAppContainer.m_ModScreen = AppContainer.ModScreen.PREFINISH
#End If
                    DisplayBDScreen(BDSCREENS.BDUODScan)
                End If
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Confirm Batch Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Method to display a confirmation message when the user selects the quit button from the Book In delivery Home Screen.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub QuitSession()
        Try
            Dim iResult As Integer = 0
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M79"), "Confirmation", MessageBoxButtons.YesNo, _
                                      MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
            If iResult = MsgBoxResult.Yes Then
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
                Me.EndSession(AppContainer.IsAbort.Yes)
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Quit Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Method to display the messages when the Finish button is selected from the UOD scan screen depending on RF or Non-RF.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub FinishSession()
        'send data to epos about normal session end - send the Abort status as N------------------------------------------
        Try
#If RF Then
            objAppContainer.objStatusBar.SetMessage("Stock file update in Progress")
            objAppContainer.m_ModScreen = AppContainer.ModScreen.POSTFINISH
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M77"), "Alert ", MessageBoxButtons.OK, _
                                                                               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            'End If
            Me.EndSession(AppContainer.IsAbort.No)
#ElseIf NRF Then
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            'If objAppContainer.strDeviceType = Macros.RF Then
            '    MessageBox.Show(MessageManager.GetInstance().GetMessage("M77"))
            '    'Else
            '    '    MessageBox.Show(MessageManager.GetInstance().GetMessage("M76"), "Alert ", MessageBoxButtons.OK, _
            '    '                                                                       MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            'End If
            Me.EndSession(AppContainer.IsAbort.No)
#End If


        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Finish Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub UODMisdirectCheck(ByVal strBarcode As String)

        If bMisdirectReturn Then
            If m_arrMisdirectReturnUODBatch.Count() > 0 Then
                Dim arrlistTemp As New ArrayList
                arrlistTemp.AddRange(m_arrListBatch)

                For Each objMisdirectReturn As GIValueHolder.ScanDetails In arrlistTemp
                    If strBarcode = objMisdirectReturn.ScannedCode _
                       AndAlso objMisdirectReturn.ScanType = ScanType.ReturnedMisdirect Then
                        m_arrListBatch.Remove(objMisdirectReturn)
                    End If
                Next
                For Each strReturnUOD As String In m_arrMisdirectReturnUODBatch
                    If strReturnUOD = strBarcode Then
                        m_arrMisdirectReturnUODBatch.Remove(strBarcode)
                        m_arrMisdirectReturnUOD.Remove(strBarcode)
                        m_arrScannedUODBatch.Remove(strBarcode)
                        RevertMisdirectedCounts(strBarcode)
                        Exit For
                    End If
                Next
            End If
            If m_arrMisdirectReturnUOD.Count() > 0 Then
                For Each strReturnUOD As String In m_arrMisdirectReturnUOD
                    If strReturnUOD = strBarcode Then
                        m_arrMisdirectReturnUOD.Remove(strBarcode)
                        '  m_arrScannedUODBatch.Remove(strBarcode)
                        RevertMisdirectScannedUOD(strBarcode)
                        Exit For
                    End If
                Next
            End If
        End If
        bMisdirectReturn = False
    End Sub
    ' V1.1 - KK
    ' Added new method DallasUODMisdirectCheck
    ''' <summary>
    ''' Method to display the messages if the scanned UOD is not for this store.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DallasUODMisdirectCheck(ByVal cBarcode As String)
#If RF Then
        Try
            m_cDallasMisdirectBarcode = cBarcode
            Dim iResult As Integer = 0
            m_bDalMisdirectReturn = False
            ' Checking if the store number in Dallas UOD barcode matches the store number in config file
            If Not cBarcode.Substring(4, 4) = ConfigDataMgr.GetInstance.GetParam(ConfigKey.STORE_NO) Then
                m_BDUODScan.Enabled = False
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                ' Checking if this barcode is already scanned
                If Not m_arrDallasMisdirectRScanned.Contains(cBarcode) Then
                    ' If the barcode is not already scanned then 
                    ' Incrementing misdirected returned Burton count
                    m_objMisdirectedReturnScanned.TotalBurton += 1
                    ' Adding the barcode to the array of scanned misdirect returned barcode
                    m_arrDallasMisdirectRScanned.Add(cBarcode)
                End If
                DisplayBDScreen(BDSCREENS.BDUODScan)
                m_bDalMisdirectReturn = True
                DisplayMsgBox(MessageManager.GetInstance().GetMessage("M135"), "RETURN TO DRIVER", MsgBx.BUTTON_TYPE.CONTINE, 1)
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Dallas UOD Misdirect Check: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
#End If
#If NRF Then
        Try
            'NRF Processing for Misdirected Dallas UOD processing
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
            'Check for store number in Dallas UOD barcode
            If Not cBarcode.Substring(4, 4) = ConfigDataMgr.GetInstance.GetParam(ConfigKey.STORE_NO) Then
                MsgBx.DisplayMessage(MessageManager.GetInstance().GetMessage("M135"), "RETURN TO DRIVER", MsgBx.BUTTON_TYPE.CONTINE)
                m_bDalMisdirectReturn = True
                ' Checking if this barcode is already scanned
                If Not m_arrDallasMisdirectRScanned.Contains(cBarcode) Then
                    ' If the barcode is not already scanned then 
                    ' incrementing misdirected returned Burton count
                    m_objMisdirectedReturnScanned.TotalBurton += 1
                    ' Adding the barcode to the array of scanned misdirect returned barcode
                    m_arrDallasMisdirectRScanned.Add(cBarcode)
                End If
                DisplayBDScreen(BDSCREENS.BDUODScan)
            Else
                m_bDalMisdirectReturn = False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occurred at Book In Delivery Dallas UOD Misdirect Check: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try

#End If

    End Sub

    ''' <summary>
    ''' Method to display the appropriate messages when UOD is not on file.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UODNotOnFile(ByVal strBarcode As String)
#If RF Then
        Try
            strNotOnFileBarcode = strBarcode
            Dim iResult As Integer = 0
            Dim strValidCode As String = ""
            m_BDUODScan.Enabled = False
            DisplayMsgBox(MessageManager.GetInstance().GetMessage("M88"), "UOD NOT ON FILE", MsgBx.BUTTON_TYPE.NOTONFILE)

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery UOD Not On File Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
#End If
#If NRF Then
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
            Dim iResult As Integer = 0
            Dim strValidCode As String = ""
            Dim messageResult As MsgBx.BUTTON_VALUE = MsgBx.DisplayMessage(MessageManager.GetInstance().GetMessage("M88"), "UOD NOT ON FILE", MsgBx.BUTTON_TYPE.NOTONFILE)
            If messageResult = MsgBx.BUTTON_VALUE.ACCEPT Then
                If MsgBx.DisplayMessage(MessageManager.GetInstance().GetMessage("M89"), "Alert", MsgBx.BUTTON_TYPE.CONTINE) = MsgBx.BUTTON_VALUE.CONTINUE Then
                    If strBarcode(0) = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DOLLYID) Then
                        iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M90"), "UOD NOT ON FILE", MessageBoxButtons.YesNo, _
                                                         MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        If iResult = MsgBoxResult.Yes Then
                            'Incrementing the count for misdirected dolly.
                            m_objMisdirectedBatch.TotalDolly += 1
                            m_objMisdirectedBatch.TotalUOD += 1


                            Dim objScanDetails As New GIValueHolder.ScanDetails
                            objScanDetails.ScannedCode = strBarcode
                            objScanDetails.ScanDate = Format(DateTime.Now, "yyyyMMdd")
                            objScanDetails.ScanTime = Format(DateTime.Now, "HHmmss")
                            objScanDetails.ScanType = ScanType.LateUOD
                            objScanDetails.ScanLevel = ScanLevel.Delivery
                            objScanDetails.DespatchDate = "000000"
                            objScanDetails.Rejected = "N"
                            m_arrListBatch.Add(objScanDetails)
                            m_arrScannedUODBatch.Add(strBarcode)

                            UODMisdirectCheck(strBarcode)
                            MsgBx.DisplayMessage(MessageManager.GetInstance().GetMessage("M91"), "DOLLY NOT ON FILE", MsgBx.BUTTON_TYPE.CONTINE)
                            'Check whether the Batch count is equal to the scan count.
                            If m_arrListBatch.Count = m_iCount Then
                                BDSessionMgr.GetInstance().DisplayBDScreen(BDSCREENS.BDUODBatchDrvrConfrm)
                            Else
                                DisplayBDScreen(BDSCREENS.BDUODScan)
                            End If
                        Else
                            MsgBx.DisplayMessage(MessageManager.GetInstance().GetMessage("M92"), "DOLLY NOT ON FILE", MsgBx.BUTTON_TYPE.CONTINE)
                            DisplayBDScreen(BDSCREENS.BDUODScan)
                        End If
                    Else
                        Select Case strBarcode(0)
                            Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.CRATEID)
                                m_objMisdirectedBatch.TotalCrate += 1
                            Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.ROLLYCAGEID)
                                m_objMisdirectedBatch.TotalRollCage += 1
                            Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.PALLETSID)
                                m_objMisdirectedBatch.TotalPallet += 1
                            Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.OUTERSID)
                                m_objMisdirectedBatch.TotalOuter += 1
                        End Select
                        m_objMisdirectedBatch.TotalUOD += 1
                        If MsgBx.DisplayMessage(MessageManager.GetInstance().GetMessage("M91"), "UOD NOT ON FILE", MsgBx.BUTTON_TYPE.CONTINE) = MsgBx.BUTTON_VALUE.CONTINUE Then
                            Dim objScanDetails As New GIValueHolder.ScanDetails
                            objScanDetails.ScannedCode = strBarcode
                            objScanDetails.ScanDate = Format(DateTime.Now, "yyyyMMdd")
                            objScanDetails.ScanTime = Format(DateTime.Now, "HHmmss")
                            objScanDetails.ScanType = ScanType.LateUOD
                            objScanDetails.ScanLevel = ScanLevel.Delivery
                            objScanDetails.DespatchDate = "000000"
                            objScanDetails.Rejected = "N"
                            m_arrListBatch.Add(objScanDetails)
                            m_arrScannedUODBatch.Add(strBarcode)

                            UODMisdirectCheck(strBarcode)
                            'Check whether the Batch count is equal to the scan count.
                            If m_arrListBatch.Count = m_iCount Then
                                BDSessionMgr.GetInstance().DisplayBDScreen(BDSCREENS.BDUODBatchDrvrConfrm)
                            Else
                                DisplayBDScreen(BDSCREENS.BDUODScan)
                            End If
                        End If
                    End If
                End If
            ElseIf messageResult = MsgBx.BUTTON_VALUE.EMPTYCRATE Then

                MsgBx.DisplayMessage(MessageManager.GetInstance().GetMessage("M93"), "Alert", MsgBx.BUTTON_TYPE.CONTINE)
                DisplayBDScreen(BDSCREENS.BDUODScan)
            ElseIf messageResult = MsgBx.BUTTON_VALUE.REJECT Then

                MsgBx.DisplayMessage(MessageManager.GetInstance().GetMessage("M94"), "RETURN TO DRIVER", MsgBx.BUTTON_TYPE.CONTINE)

                Dim bRejectedMisdirect As Boolean = False
                'Checking in Misdirect return for a batch
                If m_arrMisdirectReturnUODBatch.Count > 0 Then
                    For Each strMisdirectReturn As String In m_arrMisdirectReturnUODBatch
                        If strBarcode = strMisdirectReturn Then
                            bRejectedMisdirect = True
                            Exit For
                        End If
                    Next
                End If
                'Checking in Misdirect return for a Session
                If m_arrMisdirectReturnUOD.Count > 0 Then
                    For Each strMisdirectReturn As String In m_arrMisdirectReturnUOD
                        If strBarcode = strMisdirectReturn Then
                            bRejectedMisdirect = True
                            Exit For
                        End If
                    Next
                End If
                'if not already returned misdirect, add the details to the batch and session count 
                If Not bRejectedMisdirect Then
                    Select Case strBarcode(0)
                        Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.CRATEID)
                            m_objMisdirectedReturnBatch.TotalCrate += 1
                        Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.DOLLYID)
                            m_objMisdirectedReturnBatch.TotalDolly += 1
                        Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.ROLLYCAGEID)
                            m_objMisdirectedReturnBatch.TotalRollCage += 1
                        Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.PALLETSID)
                            m_objMisdirectedReturnBatch.TotalPallet += 1
                        Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.OUTERSID)
                            m_objMisdirectedReturnBatch.TotalOuter += 1
                    End Select
                    m_objMisdirectedReturnBatch.TotalUOD += 1
                    Dim objScanDetails As New GIValueHolder.ScanDetails
                    objScanDetails.ScannedCode = strBarcode
                    objScanDetails.ScanDate = Format(DateTime.Now, "yyyyMMdd")
                    objScanDetails.ScanTime = Format(DateTime.Now, "HHmmss")
                    objScanDetails.ScanType = ScanType.ReturnedMisdirect
                    objScanDetails.ScanLevel = ScanLevel.Delivery
                    objScanDetails.DespatchDate = "000000"
                    objScanDetails.Rejected = "N"
                    m_arrListBatch.Add(objScanDetails)
                    m_arrScannedUODBatch.Add(strBarcode)
                    m_arrMisdirectReturnUODBatch.Add(strBarcode)
                    m_arrMisdirectReturnUOD.Add(strBarcode)
                    bRejectedMisdirect = True
                End If
                'Check whether the Batch count is equal to the scan count.
                If m_arrListBatch.Count = m_iCount Then
                    BDSessionMgr.GetInstance().DisplayBDScreen(BDSCREENS.BDUODBatchDrvrConfrm)
                Else
                    DisplayBDScreen(BDSCREENS.BDUODScan)
                End If
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery UOD Not On File Session: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
#End If

    End Sub
    ' V1.1 - KK
    ' Added new method DallasUODNotOnFile processing. The Not on file Dallas data will be send to controller as Banked data
    ''' <summary>
    ''' Method to display the appropriate messages when UOD is not on file.
    ''' </summary>
    ''' <Parameter> cBarcode </Parameter>
    ''' <remarks></remarks>
    Public Sub DallasUODNotOnFile(ByVal cBarcode As String)
#If RF Then
        Try
            'RF processing for Dallas UOD not on file
            m_cDallasNotOnFileBarcode = cBarcode
            Dim iResult As Integer = 0
            m_BDUODScan.Enabled = False
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
            DisplayBDScreen(BDSCREENS.BDUODScan)
            DisplayMsgBox(MessageManager.GetInstance().GetMessage("M134"), "UOD NOT ON FILE", MsgBx.BUTTON_TYPE.CONTINE, 8)
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Dallas UOD Not On File Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
#End If
#If NRF Then
        Try
            'NRF Processing for Dallas UOD not on file
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
            If MsgBx.DisplayMessage(MessageManager.GetInstance().GetMessage("M134"), "UOD NOT ON FILE", MsgBx.BUTTON_TYPE.CONTINE) = MsgBx.BUTTON_VALUE.CONTINUE Then
                Dim objScanDetails As New GIValueHolder.DallasScanDetail
                objScanDetails.DallasBarcode = cBarcode
                objScanDetails.DallasExpectedDate = "000000"
                objScanDetails.DallasScanDate = Format(DateTime.Now, "yyMMdd")
                objScanDetails.ScanStatus = Macros.BANKED
                'saving the Dallas UOD banked data
                m_arrDalScanBatch.Add(objScanDetails)
                arrDallasScannedUOD.Add(cBarcode)
                m_arrDallasBankedUOD.Add(cBarcode)
                m_arrDallasConfirmedUOD.Add(cBarcode)
                'Incrementing the misdirect scanned burton count
                m_objMisdirectedScanned.TotalBurton += 1
            End If

            DisplayBDScreen(BDSCREENS.BDUODScan)
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery Dallas UOD Not On File Session: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
#End If

    End Sub

    ' V1.1 - KK
    ' Added new method DallasUODNotonFileMessage for RF device UOD not on File Processing
    ''' <summary>
    ''' Method to switch back the screen to Bookin scan screen.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DallasUODNotonFileMessage(ByVal messageboxResult As MsgBx.BUTTON_VALUE)
        Try
            If messageboxResult = MsgBx.BUTTON_VALUE.CONTINUE Then
                Dim objScanDetails As New GIValueHolder.DallasScanDetail
                objScanDetails.DallasBarcode = m_cDallasNotOnFileBarcode
                objScanDetails.DallasExpectedDate = "000000"
                objScanDetails.DallasScanDate = Format(DateTime.Now, "yyMMdd")
                objScanDetails.ScanStatus = Macros.BANKED

                m_arrDalScanBatch.Add(objScanDetails)
                m_arrDallasConfirmedUOD.Add(m_cDallasNotOnFileBarcode)
                arrDallasScannedUOD.Add(m_cDallasNotOnFileBarcode)
                m_arrDallasBankedUOD.Add(m_cDallasNotOnFileBarcode)
                m_arrDallasConfirmedUOD.Add(m_cDallasNotOnFileBarcode)
                m_objMisdirectedScanned.TotalBurton += 1

                m_BDUODScan.Enabled = True
                DisplayBDScreen(BDSCREENS.BDUODScan)
            End If

        Catch ex As Exception

        End Try
    End Sub


    Private Sub RevertMisdirectedCounts(ByVal strBarcode As String)
        Select Case strNotOnFileBarcode.Chars(0)
            Case "2"
                m_objMisdirectedReturnBatch.TotalDolly -= 1
            Case "1"
                m_objMisdirectedReturnBatch.TotalCrate -= 1
            Case "3"
                m_objMisdirectedReturnBatch.TotalRollCage -= 1
            Case "4"
                m_objMisdirectedReturnBatch.TotalPallet -= 1
            Case "8"
                If strNotOnFileBarcode.Substring(0, 4) = "8888" Then
                    m_objMisdirectedReturnBatch.TotalIST -= 1
                Else
                    m_objMisdirectedReturnBatch.TotalOuter -= 1
                End If
        End Select
        m_objMisdirectedReturnBatch.TotalUOD -= 1
    End Sub
    Private Sub RevertMisdirectScannedUOD(ByVal strBarcode As String)
        Select Case strNotOnFileBarcode.Chars(0)
            Case "2"
                m_objMisdirectedReturnScanned.TotalDolly -= 1
            Case "1"
                m_objMisdirectedReturnScanned.TotalCrate -= 1
            Case "3"
                m_objMisdirectedReturnScanned.TotalRollCage -= 1
            Case "4"
                m_objMisdirectedReturnScanned.TotalPallet -= 1
            Case "8"
                If strNotOnFileBarcode.Substring(0, 4) = "8888" Then
                    m_objMisdirectedReturnScanned.TotalIST -= 1
                Else
                    m_objMisdirectedReturnScanned.TotalOuter -= 1

                End If

        End Select
        m_objMisdirectedReturnScanned.TotalUOD -= 1
    End Sub
    Public Sub NotOnFile(ByVal messageResult As MsgBx.BUTTON_VALUE)
        Dim iResult As Integer = 0
        Try
            DisplayBDScreen(BDSCREENS.BDUODScan)

            If messageResult = MsgBx.BUTTON_VALUE.ACCEPT Then
                DisplayMsgBox(MessageManager.GetInstance().GetMessage("M89"), "Alert", MsgBx.BUTTON_TYPE.CONTINE, 2)

            ElseIf messageResult = MsgBx.BUTTON_VALUE.EMPTYCRATE Then
                DisplayMsgBox(MessageManager.GetInstance().GetMessage("M93"), "Alert", MsgBx.BUTTON_TYPE.CONTINE, 1)

            ElseIf messageResult = MsgBx.BUTTON_VALUE.REJECT Then
                DisplayMsgBox(MessageManager.GetInstance().GetMessage("M94"), "RETURN TO DRIVER", MsgBx.BUTTON_TYPE.CONTINE, 7)

            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery : NotOnFile() : " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Public Sub Misdirect(ByVal messageboxResult As MsgBx.BUTTON_VALUE)
        Dim iResult As Integer = 0
        DisplayMsgBox(MessageManager.GetInstance().GetMessage("M89"), "Alert", MsgBx.BUTTON_TYPE.CONTINE, 2)

    End Sub
    ' function to handle the Returned misdirect UOD.
    Public Sub MisdirectRejected()



        Dim bRejectedMisdirect As Boolean = False
        'Checking in Misdirect return for a batch
        If m_arrMisdirectReturnUODBatch.Count > 0 Then
            For Each strMisdirectReturn As String In m_arrMisdirectReturnUODBatch
                If strNotOnFileBarcode = strMisdirectReturn Then
                    bRejectedMisdirect = True
                    Exit For
                End If
            Next
        End If
        'Checking in Misdirect return for a Session
        If m_arrMisdirectReturnUOD.Count > 0 Then
            For Each strMisdirectReturn As String In m_arrMisdirectReturnUOD
                If strNotOnFileBarcode = strMisdirectReturn Then
                    bRejectedMisdirect = True
                    Exit For
                End If
            Next
        End If
        'if not already returned misdirect, add the details to the batch and session count 
        If Not bRejectedMisdirect Then
            Select Case strNotOnFileBarcode.Chars(0)
                Case "1"
                    m_objMisdirectedReturnBatch.TotalCrate += 1
                Case "2"
                    m_objMisdirectedReturnBatch.TotalDolly += 1
                Case "3"
                    m_objMisdirectedReturnBatch.TotalRollCage += 1
                Case "4"
                    m_objMisdirectedReturnBatch.TotalPallet += 1
                Case "8"
                    If strNotOnFileBarcode.Substring(0, 4) = "8888" Then
                        m_objMisdirectedReturnBatch.TotalIST += 1
                    Else
                        m_objMisdirectedReturnBatch.TotalOuter += 1
                    End If
            End Select
            m_objMisdirectedReturnBatch.TotalUOD += 1
            Dim objScanDetails As New GIValueHolder.ScanDetails
            objScanDetails.ScannedCode = strNotOnFileBarcode
            objScanDetails.ScanDate = Format(DateTime.Now, "yyyyMMdd")
            objScanDetails.ScanTime = Format(DateTime.Now, "HHmmss")
            objScanDetails.ScanType = ScanType.ReturnedMisdirect
            objScanDetails.ScanLevel = ScanLevel.Delivery
            objScanDetails.DespatchDate = "000000"
            objScanDetails.Rejected = "N"
            m_arrListBatch.Add(objScanDetails)
            m_arrScannedUODBatch.Add(strNotOnFileBarcode)

            m_arrMisdirectReturnUODBatch.Add(strNotOnFileBarcode)
            'To add the all the UODs in the MisdirectedReturn Batch to the MisdirectedReturnUOD arraylist.
            'm_arrMisdirectReturnUOD.AddRange(m_arrMisdirectReturnUODBatch)
            m_arrMisdirectReturnUOD.Add(strNotOnFileBarcode)

            bRejectedMisdirect = True
        End If



        m_BDUODScan.Enabled = True
        'Check whether the Batch count is equal to the scan count.
        If m_arrListBatch.Count = m_iCount Then
            BDSessionMgr.GetInstance().DisplayBDScreen(BDSCREENS.BDUODBatchDrvrConfrm)
        Else

            DisplayBDScreen(BDSCREENS.BDUODScan)
        End If

    End Sub
    Public Sub AcceptMisDirect(ByVal messageboxResult As MsgBx.BUTTON_VALUE)
        Dim iResult As Integer = 0
        Try


            If messageboxResult = MsgBx.BUTTON_VALUE.CONTINUE Then
                m_MsgBox.Hide()
                DisplayBDScreen(BDSCREENS.BDUODScan)
                If strNotOnFileBarcode.Chars(0) = ConfigDataMgr.GetInstance.GetParam(ConfigKey.DOLLYID) Then

                    iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M90"), "UOD NOT ON FILE", MessageBoxButtons.YesNo, _
                                                     MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    DisplayBDScreen(BDSCREENS.BDUODScan)
                    If iResult = MsgBoxResult.Yes Then
                        'Incrementing the count for misdirected dolly.
                        m_objMisdirectedBatch.TotalDolly += 1
                        m_objMisdirectedBatch.TotalUOD += 1

                        Dim objScanDetails As New GIValueHolder.ScanDetails
                        objScanDetails.ScannedCode = strNotOnFileBarcode
                        objScanDetails.ScanDate = Format(DateTime.Now, "yyyyMMdd")
                        objScanDetails.ScanTime = Format(DateTime.Now, "HHmmss")
                        objScanDetails.ScanType = ScanType.LateUOD
                        objScanDetails.ScanLevel = ScanLevel.Delivery
                        objScanDetails.DespatchDate = "000000"
                        objScanDetails.Rejected = "N"
                        m_arrListBatch.Add(objScanDetails)
                        m_arrScannedUODBatch.Add(strNotOnFileBarcode)
                        UODMisdirectCheck(strNotOnFileBarcode)
                        DisplayMsgBox(MessageManager.GetInstance().GetMessage("M91"), "DOLLY NOT ON FILE", MsgBx.BUTTON_TYPE.CONTINE, 3)

                    Else
                        DisplayMsgBox(MessageManager.GetInstance().GetMessage("M92"), "DOLLY NOT ON FILE", MsgBx.BUTTON_TYPE.CONTINE, 1)
                        'MsgBx.DisplayMessage(MessageManager.GetInstance().GetMessage("M92"), "DOLLY NOT ON FILE", MsgBx.BUTTON_TYPE.CONTINE)
                        'DisplayBDScreen(BDSCREENS.BDUODScan)
                    End If
                Else
                    Select Case strNotOnFileBarcode.Chars(0)
                        Case "1"
                            m_objMisdirectedBatch.TotalCrate += 1
                        Case "3"
                            m_objMisdirectedBatch.TotalRollCage += 1
                        Case "4"
                            m_objMisdirectedBatch.TotalPallet += 1
                        Case "8"
                            m_objMisdirectedBatch.TotalOuter += 1
                    End Select
                    m_objMisdirectedBatch.TotalUOD += 1
                    DisplayMsgBox(MessageManager.GetInstance().GetMessage("M91"), "UOD NOT ON FILE", MsgBx.BUTTON_TYPE.CONTINE, 4)

                End If
            End If
        Catch ex As Exception

        End Try
    End Sub
    Public Sub UODNotOnEPOS()
        'Check whether the Batch count is equal to the scan count.
        If m_arrListBatch.Count = m_iCount Then
            m_BDUODScan.Enabled = True
            BDSessionMgr.GetInstance().DisplayBDScreen(BDSCREENS.BDUODBatchDrvrConfrm)
        Else
            m_BDUODScan.Enabled = True
            DisplayBDScreen(BDSCREENS.BDUODScan)
        End If

    End Sub
    Public Sub UODNotinFileMessage(ByVal messageboxResult As MsgBx.BUTTON_VALUE)
        Try


            If messageboxResult = MsgBx.BUTTON_VALUE.CONTINUE Then
                If strNotOnFileBarcode.Substring(0, 4) = "8888" Then
                    m_objMisdirectedBatch.TotalIST += 1
                    m_objMisdirectedBatch.TotalUOD += 1
                End If
                Dim objScanDetails As New GIValueHolder.ScanDetails
                objScanDetails.ScannedCode = strNotOnFileBarcode
                objScanDetails.ScanDate = Format(DateTime.Now, "yyyyMMdd")
                objScanDetails.ScanTime = Format(DateTime.Now, "HHmmss")
                objScanDetails.ScanType = ScanType.LateUOD
                objScanDetails.ScanLevel = ScanLevel.Delivery
                objScanDetails.DespatchDate = "000000"
                objScanDetails.Rejected = "N"
                m_arrListBatch.Add(objScanDetails)
                m_arrScannedUODBatch.Add(strNotOnFileBarcode)
                UODMisdirectCheck(strNotOnFileBarcode)


                'Check whether the Batch count is equal to the scan count.
                If m_arrListBatch.Count = m_iCount Then
                    m_BDUODScan.Enabled = True
                    BDSessionMgr.GetInstance().DisplayBDScreen(BDSCREENS.BDUODBatchDrvrConfrm)
                Else
                    m_BDUODScan.Enabled = True
                    DisplayBDScreen(BDSCREENS.BDUODScan)
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub


    Public Sub DisplayBDUODScreen(ByVal messageResult As MsgBx.BUTTON_VALUE)
        m_BDUODScan.Enabled = True
        DisplayBDScreen(BDSCREENS.BDUODScan)
    End Sub
    ''' <summary>
    ''' Method to display the messages when IST is not on file.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub InterStoreTransfer(ByVal strBarcode As String)
        Try
            strNotOnFileBarcode = strBarcode
            Dim iResult As Integer = 0
            iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M95"), "IST NOT ON FILE", MessageBoxButtons.YesNo, _
                                 MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            If iResult = MsgBoxResult.Yes Then
                DisplayMsgBox(MessageManager.GetInstance().GetMessage("M91"), "UOD NOT ON FILE", MsgBx.BUTTON_TYPE.CONTINE, 4)

            ElseIf iResult = MsgBoxResult.No Then
                DisplayMsgBox(MessageManager.GetInstance().GetMessage("M94"), "RETURN TO DRIVER", MsgBx.BUTTON_TYPE.CONTINE, 7)

            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery InterStore Transfer Not On File Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by BDSessionMgr.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EndSession(ByVal isAbort As AppContainer.IsAbort)
        Try


#If RF Then
            objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
            'If Not objAppContainer.bCommFailure Then
            If objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.SSC, AppContainer.FunctionType.BookIn, isAbort) Then
#ElseIf NRF Then
                objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.SSC, AppContainer.FunctionType.BookIn, isAbort)
#End If
                m_BDUODHome.Close()
                m_BDUODHome.Dispose()
                m_BDUODInitialSummary.Close()
                m_BDUODInitialSummary.Dispose()
                m_BDUODScan.Close()
                m_BDUODScan.Dispose()
                m_BDUODScan = Nothing
                m_BDUODDrvrConfrmation.Close()
                m_BDUODDrvrConfrmation.Dispose()
                m_BDUODFinalDrvrConfirmation.Close()
                m_BDUODFinalDrvrConfirmation.Dispose()
                m_BDUODSummary.Close()
                m_BDUODSummary.Dispose()
                m_ItemList = Nothing
                m_UODInfo = Nothing
                m_arrParentDetails = Nothing
                m_arrBatchParentDetails = Nothing
                m_arrListBatch = Nothing
                m_arrBookInDetails = Nothing
                m_objSessionScanCount = Nothing
                m_arrChildBkdDolly = Nothing
                m_objOutstanding = Nothing
                m_objExpected = Nothing
                m_objExpectedScanned = Nothing
                m_objOutstandingScanned = Nothing
                m_objExpectedBatch = Nothing
                m_objOutStandingBatch = Nothing
                m_objMisdirectedBatch = Nothing
                m_objMisdirectedScanned = Nothing
                m_arrMisdirectReturnUOD = Nothing
                m_objMisdirectedReturnBatch = Nothing
                m_objMisdirectedReturnScanned = Nothing
                ' V1.1 - KK
                ' Clearing all Dallas related array lists
                arrDallasScannedUOD = Nothing
                m_arrDallasBankedUOD = Nothing
                m_arrDallasConfirmedUOD = Nothing
                m_arrDallasMisdirectRScanned = Nothing
                ' V1.1 - CK
                ' Clearing the Scanned Dallas Counts
                m_iExpectedDalFinalCount = Nothing
                m_iFutureDalFinalCount = Nothing
                m_iOutstandingDalFinalCount = Nothing
                m_iNotOnFileDalFinalCount = Nothing
                m_iMisdirectReturnedDalFinalCount = Nothing
                m_objFuture = Nothing
                m_DalUODInfo = Nothing
                m_DALLASItemList = Nothing
                m_DalUODInfo = Nothing
#If RF Then

            End If
            objAppContainer.bReconnectSuccess = False
            objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
            objAppContainer.objGoodsInMenu.Visible = True
            ' End If
#End If


#If NRF Then
            objAppContainer.objGoodsInMenu.Visible = True
            'CR for Forced Log off
            If isAbort = AppContainer.IsAbort.No Then
                objAppContainer.bForceLogOff = True
                objAppContainer.ForcedLogOff()
            End If
#End If

            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at Book In Delivery End Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
#If RF Then
    Public Sub DisposeBookInUOD()
        m_BDUODHome.Close()
        m_BDUODHome.Dispose()
        m_BDUODInitialSummary.Close()
        m_BDUODInitialSummary.Dispose()
        m_BDUODScan.Close()
        m_BDUODScan.Dispose()
        m_BDUODDrvrConfrmation.Close()
        m_BDUODDrvrConfrmation.Dispose()
        m_BDUODFinalDrvrConfirmation.Close()
        m_BDUODFinalDrvrConfirmation.Dispose()
        m_BDUODSummary.Close()
        m_BDUODSummary.Dispose()
        m_ItemList = Nothing
        m_UODInfo = Nothing
        m_arrParentDetails = Nothing
        m_arrBatchParentDetails = Nothing
        m_arrListBatch = Nothing
        m_arrBookInDetails = Nothing
        m_objSessionScanCount = Nothing
        m_arrChildBkdDolly = Nothing
        m_objOutstanding = Nothing
        m_objExpected = Nothing
        m_objExpectedScanned = Nothing
        m_objOutstandingScanned = Nothing
        m_objExpectedBatch = Nothing
        m_objOutStandingBatch = Nothing
        m_objMisdirectedBatch = Nothing
        m_objMisdirectedScanned = Nothing
        m_arrMisdirectReturnUOD = Nothing
        m_objMisdirectedReturnBatch = Nothing
        m_objMisdirectedReturnScanned = Nothing
        'V1.1 KK
        'Disposing all Dallas UOD related variables
        arrDallasScannedUOD = Nothing
        m_arrDallasBankedUOD = Nothing
        m_arrDallasConfirmedUOD = Nothing
        m_arrDallasMisdirectRScanned = Nothing
        m_arrDalScanBatch = Nothing
        m_DALLASItemList = Nothing
        m_DalUODInfo = Nothing
        m_objFuture = Nothing
        ' V1.1 - CK
        ' Clearing the Scanned Dallas Counts
        m_iExpectedDalFinalCount = Nothing
        m_iFutureDalFinalCount = Nothing
        m_iOutstandingDalFinalCount = Nothing
        m_iNotOnFileDalFinalCount = Nothing
        m_iMisdirectReturnedDalFinalCount = Nothing
        objAppContainer.objGoodsInMenu.Visible = True
        objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
    End Sub
#End If
    ''' <summary>
    ''' Custom message box called by BDSessionMgr.
    ''' </summary>
    ''' <remarks></remarks>
    Public Function DisplayMsgBox(ByVal sText As String, ByVal sCaption As String, _
                                  ByVal BtnType As MsgBx.BUTTON_TYPE, Optional ByVal MessageType As Integer = 0) As Boolean
        Try
            m_MsgBox.MsgBoxInitialize(sText, sCaption, BtnType, MessageType)
            m_MsgBox.Invoke(New EventHandler(AddressOf DisplayMsgBoxScreen))
        Catch ex As Exception
            Return False

        End Try
        Return True

    End Function
    ''' <summary>
    ''' Custom message box shown
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayMsgBoxScreen(ByVal o As Object, ByVal e As EventArgs)
        With m_MsgBox
            .Show()
            ' .Visible = True
            .Refresh()
        End With
    End Sub
    ''' <summary>
    ''' Message box shown for 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub NoBadgeSelect(ByVal iMessageType As Integer)
        If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BATCHDRVRCONFRM Then
            m_BDUODDrvrConfrmation.Enabled = False
        ElseIf objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.FINALDRVRCNFRM Then
            m_BDUODFinalDrvrConfirmation.Enabled = False
        End If
        DisplayMsgBox(MessageManager.GetInstance().GetMessage("M99"), "Alert", MsgBx.BUTTON_TYPE.CONTINE, iMessageType)
    End Sub

    Public Sub CreateRollbackPoint()

        m_objExpectedRollback.TotalDolly = m_objExpected.TotalDolly
        m_objExpectedRollback.TotalCrate = m_objExpected.TotalCrate


        m_objOutstandingRollback.TotalCrate = m_objOutstanding.TotalCrate
        m_objOutstandingRollback.TotalDolly = m_objOutstanding.TotalDolly
    End Sub
    Public Sub RevertToRollbackPoint()
        m_objExpected.TotalDolly = m_objExpectedRollback.TotalDolly
        m_objExpected.TotalCrate = m_objExpectedRollback.TotalCrate


        m_objOutstanding.TotalCrate = m_objOutstandingRollback.TotalCrate
        m_objOutstanding.TotalDolly = m_objOutstandingRollback.TotalDolly

    End Sub



    Public Sub CalculateBookInCount()

        m_iFinalExpectedCrateCount = m_objExpectedScanned.TotalCrate
        m_iFinalOutCrateCount = m_objOutstandingScanned.TotalCrate
        m_iFinalFutureCrateCount = m_objFutureScanned.TotalCrate
        For Each objParent As UODParentDetails In m_arrParentDetails
            For Each objUOD As String In m_arrScannedUOD
                If objUOD = objParent.UODParent Then
                    If objParent.UODExpectedDate = Macros.EXPECTED Then
                        m_iFinalExpectedCrateCount -= objParent.UODScannedCount
                    ElseIf objParent.UODExpectedDate = Macros.OUTSTANDING Then
                        m_iFinalOutCrateCount -= objParent.UODScannedCount
                    Else
                        m_iFinalFutureCrateCount -= objParent.UODScannedCount
                    End If
                    Exit For
                End If

            Next
        Next
        For Each objParent As UODParentDetails In m_arrPartialDetails
            For Each objUOD As String In m_arrScannedUOD
                If objUOD = objParent.UODParent Then
                    If objParent.UODExpectedDate = Macros.EXPECTED Then
                        m_iFinalExpectedCrateCount -= objParent.UODScannedCount
                    ElseIf objParent.UODExpectedDate = Macros.OUTSTANDING Then
                        m_iFinalOutCrateCount -= objParent.UODScannedCount
                    Else
                        m_iFinalFutureCrateCount -= objParent.UODScannedCount
                    End If
                    Exit For
                End If
            Next
        Next


    End Sub


End Class



Public Class PartialDollyScanned

    Private m_iPartailDollyChildCount As Integer

    Private m_Expected As Boolean
    Public Property Expected() As Boolean
        Get
            Return m_Expected
        End Get
        Set(ByVal value As Boolean)
            m_Expected = value
        End Set
    End Property

    Public Property PartailDollyChildCount() As Integer
        Get
            Return m_iPartailDollyChildCount
        End Get
        Set(ByVal value As Integer)
            m_iPartailDollyChildCount = value
        End Set
    End Property
End Class
''' <summary>
''' The value class for getting the total confirmed scan count for  the deliveries.
''' </summary>
''' <remarks></remarks>
Public Class scanCount
    Private iTotalCrate As Integer = 0
    Private iTotalDolly As Integer = 0
    Private iTotalRollCage As Integer = 0
    Private iTotalUOD As Integer = 0
    Private iTotalPallet As Integer = 0
    Private iTotalOuter As Integer = 0
    ' v1.1 - CK
    ' New variable for Dallas count
    Private m_iTotalBurton As Integer = 0
    Private iTotalIST As Integer = 0

    Public Property TotalIST() As Integer
        Get
            Return iTotalIST
        End Get
        Set(ByVal value As Integer)
            iTotalIST = value
        End Set
    End Property
    Public Property TotalOuter() As Integer
        Get
            Return iTotalOuter
        End Get
        Set(ByVal value As Integer)
            iTotalOuter = value
        End Set
    End Property
    Public Property TotalCrate() As Integer
        Get
            Return iTotalCrate
        End Get
        Set(ByVal value As Integer)
            iTotalCrate = value
        End Set
    End Property
    Public Property TotalPallet() As Integer
        Get
            Return iTotalPallet
        End Get
        Set(ByVal value As Integer)
            iTotalPallet = value
        End Set
    End Property
    Public Property TotalDolly() As Integer
        Get
            Return iTotalDolly
        End Get
        Set(ByVal value As Integer)
            iTotalDolly = value
        End Set
    End Property

    Public Property TotalRollCage() As Integer
        Get
            Return iTotalRollCage
        End Get
        Set(ByVal value As Integer)
            iTotalRollCage = value
        End Set
    End Property
    ' v1.1 - CK
    ' New Property for Dallas count
    Public Property TotalBurton() As Integer
        Get
            Return m_iTotalBurton
        End Get
        Set(ByVal value As Integer)
            m_iTotalBurton = value
        End Set
    End Property
    Public Property TotalUOD() As Integer
        Get
            Return iTotalUOD
        End Get
        Set(ByVal value As Integer)
            iTotalUOD = value
        End Set
    End Property
    Public Sub ResetCount()
        iTotalCrate = 0
        iTotalDolly = 0
        iTotalRollCage = 0
        iTotalUOD = 0
        iTotalPallet = 0
        iTotalOuter = 0
        iTotalIST = 0
        ' v1.1 - CK
        ' Reseting m_iTotalBurton
        m_iTotalBurton = 0
    End Sub
End Class
