Imports System.Globalization
'''***************************************************************
''' <FileName>VUODSessionManager.vb</FileName>
''' <summary>
''' The View UOD Container Class.
''' Implements all business logic and GUI navigation for View UOD.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>08-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 1.1 for PPC</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
'''* Modification Log 
'''******************************************************************************* 
'''* No:       Author            Date            Description 
'''* 1.1     Kiran Krishnan  09/04/2015      Modified as part of DALLAS project
'''                  (KK)                    Amended the methods HandleScanData,
'''                                          DisposeVUOD, DisplayVUODScan,
'''                                          DisplayContainerInfo and GetUODSummaryList.
'''                                          Added a new function GetDallasUOD Summary List                          
'''********************************************************************************
Public Class VUODSessionMgr
    Private Shared m_VUODSessionMgr As VUODSessionMgr
    Private m_ViewUODDolly As frmViewUODDolly
    Private m_ViewUODNotDolly As frmViewUODNonDolly
    Private m_ViewUODMenu As frmViewUODMenu
    Private m_ViewUOD As frmViewUOD
    Private m_VUODInfo As UODInfo
    Private m_ItemInfo As ItemInfo
    Private m_VCartonInfo As CartonInfo
    Private m_arrUODListSummary As New ArrayList()
    Private m_SortedCrateList As New ArrayList
    'V1.1 - KK
    Private m_MsgBox As MsgBx

    Public m_ItemList As ArrayList = Nothing
    Public m_UODList As ArrayList = Nothing
    'V1.1 - KK
    'Added new arraylist for holding Dallas Delivery details
    Public m_DalUODList As ArrayList = Nothing
    Public m_DalUODTodayView As ArrayList = Nothing
    Public m_DalUODFutureView As ArrayList = Nothing
    Public m_CrateList As ArrayList = Nothing
    Public m_strPeriod As String = ""
    Public strUODDollyId As String = ""
    Public strCrateId As String = ""
    Public strType As String = ""
    Public strUODBkdInStatus As String = ""
    Public strCrateBkdInStatus As String = ""
    Public strBkdInDate As String = ""
    Public strExptdDate As String = ""
    Public strNoofItems As String = ""
    Public strUODType As String = ""
    Public strBkdDate As String = ""
    Public strSequence As String = "00000"
    Public strUODTypes() As String = {ContainerType.Dolly, ContainerType.Crate, _
                                      ContainerType.Pallet, ContainerType.RollCage, _
                                      ContainerType.Outer, ContainerType.IST}
    Public bTransition As Boolean = False
    Public bUODScanned As Boolean = False
    Private bCheckItem As Boolean

    Public m_UODSelectionCheck As String
    Public m_bMisdirected As Boolean = False
    ''' <summary>
    ''' Initialises the View UOD session
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartSession()
        Try
            'SETTING STATUSBAR MESSAGE
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
            'Do all View UOD related Initialisations here.
            m_ItemList = New ArrayList
            m_UODList = New ArrayList
            'V1.1 - KK
            'Initialised m_DalUODList,m_DalUODTodayView & m_DalUODFutureView
            m_DalUODList = New ArrayList
            m_DalUODTodayView = New ArrayList
            m_DalUODFutureView = New ArrayList
            ' V1.1 _ CK
            ' Initialised m_Msgbox
#If RF Then
            m_MsgBox = New MsgBx
#End If
            m_CrateList = New ArrayList
            m_arrUODListSummary = New ArrayList
            m_ViewUOD = New frmViewUOD
            m_ViewUODDolly = New frmViewUODDolly
            m_ViewUODMenu = New frmViewUODMenu
            m_ViewUODNotDolly = New frmViewUODNonDolly
            m_VUODInfo = New UODInfo
            m_ItemInfo = New ItemInfo
            m_UODSelectionCheck = Nothing
            m_bMisdirected = False
            Me.DisplayVUODScreen(VUODSCREENS.ViewUODMenu)
        Catch ex As Exception
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at View UOD Start Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try

    End Sub

    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by VUODSessionMgr
    ''' </summary>
    ''' <remarks></remarks>
    Public Function EndSession() As Boolean
        'Save and data and perform all Exit Operations.
        'Close and Dispose all forms.
        Try
            VUODSessionMgr.GetInstance.QuitSession()
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at View UOD End Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Functions for getting the object instance for the VUODSessionMgr. 
    ''' Use this method to get the object reference for the  VUODSessionMgr.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of VUODSessionMgr Class</remarks>
    Public Shared Function GetInstance() As VUODSessionMgr
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.VUOD
        If m_VUODSessionMgr Is Nothing Then
            m_VUODSessionMgr = New VUODSessionMgr
            Return m_VUODSessionMgr
        Else
            Return m_VUODSessionMgr
        End If
    End Function

    ''' <summary>
    ''' The Method handles scan the scan data returned form the barcode scanner
    ''' This method implements the business logic to populate the data to the corresponding
    ''' UI element after validation.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="Type"></param>
    ''' <remarks></remarks>
    Public Sub HandleScanData(ByVal strBarcode As String, ByVal Type As BCType)
        Try
            Dim strValidCode As String = strBarcode
            Dim bTemp As Boolean = False
            Dim bNotRecog As Boolean = False
            If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINVIEWUOD Then
                ' V1.1 - KK
                ' For Dallas positive receipt enabled stores when burton uod is scanned show the message
                If objAppContainer.bDallasPosReceiptEnabled Then
                    If strBarcode.StartsWith("0501") And strBarcode.Length = 14 Then
#If RF Then
                        DisplayMsgBox(MessageManager.GetInstance().GetMessage("M136"), "VIEW NOT AVAILABLE", MsgBx.BUTTON_TYPE.CONTINE, 10)
#End If
#If NRF Then
                        MsgBx.DisplayMessage(MessageManager.GetInstance().GetMessage("M136"), "VIEW NOT AVAILABLE", MsgBx.BUTTON_TYPE.CONTINE)
                        m_VUODSessionMgr.DisplayVUODScreen(VUODSCREENS.ViewUOD)
                        m_ViewUOD.txtProductCode.Text = ""
#End If
                        Exit Sub
                    End If
                End If

                Select Case Type

                    Case BCType.ManualEntry
                        'if validate UODBarCode
                        bUODScanned = True
                        'validating the UOD barcode entered
                        If objAppContainer.objHelper.ValidateUODBarcode(strValidCode, strBarcode) Then
                            bNotRecog = True
                        ElseIf objAppContainer.objHelper.ValidateISTCode(strValidCode, strBarcode) Then
                            bNotRecog = True
                        Else
                            bNotRecog = False
                        End If
                        If bNotRecog Then
                            bTemp = False
                            'checking the UOD list for details of UOD scanned
                            For Each objUODList As GIValueHolder.UODList In m_arrUODListSummary
                                If objUODList.UODID = strBarcode Then
                                    strUODDollyId = objUODList.UODID.ToString()
                                    'send the sequence number of the UOD scanned
                                    strSequence = objUODList.Sequencenumber
                                    With m_VUODInfo
                                        .UODNumber = objUODList.UODID
                                        .SequenceNumber = objUODList.Sequencenumber
                                        .UODStatus = objUODList.BookedIn
                                        .UODType = objUODList.UODType
                                        .ExpectedDeliveryDate = objUODList.ExptDate
                                    End With
                                    strUODBkdInStatus = m_VUODInfo.UODStatus.ToString()
                                    bTemp = True
                                    'Invoke the ProcessUODScanned method
                                    VUODSessionMgr.GetInstance.ProcessUODScanned()

                                    Exit For
                                End If


                            Next

                            If Not bTemp Then
                                'Retieving the UOD details through a arraylist if UOD code is valid
                                If (objAppContainer.objDataEngine.ValidateUODScanned(strBarcode, m_VUODInfo, AppContainer.FunctionType.View)) Then
                                    strUODDollyId = m_VUODInfo.UODNumber.ToString()
                                    'send the sequence number of the UOD scanned
                                    strSequence = m_VUODInfo.SequenceNumber
                                    If m_VUODInfo.UODStatus = Status.Booked Then
                                        strUODBkdInStatus = Macros.Y
                                    ElseIf m_VUODInfo.UODStatus = Status.UnBooked Then
                                        strUODBkdInStatus = Macros.N
                                    Else
                                        strUODBkdInStatus = Status.Audited
                                    End If

                                    'Invoke the ProcessUODScanned method
                                    VUODSessionMgr.GetInstance.ProcessUODScanned()

                                Else
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M102"), "Alert ", _
                                                   MessageBoxButtons.OK, _
                                                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                                    objAppContainer.objStatusBar.SetMessage("")
                                End If
                            End If
                        End If
                        If Not bNotRecog Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", _
                                           MessageBoxButtons.OK, _
                                                MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                            objAppContainer.objStatusBar.SetMessage("")
                        End If

                    Case BCType.I2O5
                        bUODScanned = True
                        'validating the UOD barcode scanned
                        If objAppContainer.objHelper.ValidateUODBarcode(strValidCode, strBarcode) Then
                            'checking the UOD list for details of UOD scanned
                            For Each objUODList As GIValueHolder.UODList In m_arrUODListSummary
                                If objUODList.UODID = strBarcode Then
                                    strUODDollyId = objUODList.UODID.ToString()
                                    'send the sequence number of the UOD scanned
                                    strSequence = objUODList.Sequencenumber
                                    With m_VUODInfo
                                        .UODNumber = objUODList.UODID
                                        .SequenceNumber = objUODList.Sequencenumber
                                        .UODStatus = objUODList.BookedIn
                                        .UODType = objUODList.UODType
                                        .ExpectedDeliveryDate = objUODList.ExptDate

                                    End With
                                    strUODBkdInStatus = m_VUODInfo.UODStatus.ToString()
                                    strUODDollyId = m_VUODInfo.UODNumber.ToString()
                                    strSequence = m_VUODInfo.SequenceNumber
                                    bTemp = True
                                    'Invoke the ProcessUODScanned method
                                    VUODSessionMgr.GetInstance.ProcessUODScanned()

                                    Exit For
                                End If


                            Next
                            If Not bTemp Then
#If RF Then
                                objAppContainer.m_ModScreen = AppContainer.ModScreen.UODSCAN
#End If
                                'Retieving the UOD details through a arraylist if UOD code is valid
                                If (objAppContainer.objDataEngine.ValidateUODScanned(strBarcode, _
                                                                                     m_VUODInfo, AppContainer.FunctionType.View)) Then
                                    strUODDollyId = m_VUODInfo.UODNumber.ToString()
                                    m_VUODInfo.ExpectedDeliveryDate = m_VUODInfo.DespatchDate
                                    strSequence = m_VUODInfo.SequenceNumber
                                    If m_VUODInfo.UODStatus = Status.Booked Then
                                        strUODBkdInStatus = Macros.Y
                                    ElseIf m_VUODInfo.UODStatus = Status.UnBooked Then
                                        strUODBkdInStatus = Macros.N
                                    Else
                                        strUODBkdInStatus = Status.Audited
                                    End If
                                    'Invoke the ProcessUODScanned method
                                    VUODSessionMgr.GetInstance.ProcessUODScanned()
                                Else
#If RF Then
                                    'TIMEOUT
                                    If objAppContainer.bTimeOut Then
                                        Exit Sub
                                    End If
                                    If Not objAppContainer.bReconnectSuccess Then
                                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M102"), "Alert ", _
                                                        MessageBoxButtons.OK, _
                                                             MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                                    End If
                                    objAppContainer.bReconnectSuccess = False
#ElseIf NRF Then
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M102"), "Alert ", _
                                                        MessageBoxButtons.OK, _
                                                             MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
#End If

                                    objAppContainer.objStatusBar.SetMessage("")
                                End If
                            End If
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", _
                                                      MessageBoxButtons.OK, _
                                                           MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                            objAppContainer.objStatusBar.SetMessage("")
                        End If
                    Case BCType.CODE128
                        bUODScanned = True
                        If objAppContainer.objHelper.ValidateISTCode(strValidCode, strBarcode) Then
                            For Each objUODList As GIValueHolder.UODList In m_arrUODListSummary
                                If objUODList.UODID = strBarcode Then
                                    strUODDollyId = objUODList.UODID.ToString()
                                    'send the sequence number of the UOD scanned
                                    strSequence = objUODList.Sequencenumber
                                    With m_VUODInfo
                                        .UODNumber = objUODList.UODID
                                        .SequenceNumber = objUODList.Sequencenumber
                                        .UODStatus = objUODList.BookedIn
                                        .UODType = objUODList.UODType
                                        .ExpectedDeliveryDate = objUODList.ExptDate

                                    End With
                                    strUODDollyId = m_VUODInfo.UODNumber.ToString()
                                    strSequence = m_VUODInfo.SequenceNumber
                                    bTemp = True
                                    'Invoke the ProcessUODScanned method
                                    VUODSessionMgr.GetInstance.ProcessUODScanned()

                                    Exit For
                                End If


                            Next
                            If Not bTemp Then
                                If (objAppContainer.objDataEngine.ValidateUODScanned(strBarcode, m_VUODInfo, AppContainer.FunctionType.View)) Then
                                    strUODDollyId = m_VUODInfo.UODNumber.ToString()
                                    strSequence = m_VUODInfo.SequenceNumber
                                    If m_VUODInfo.UODStatus = Status.Booked Then
                                        strUODBkdInStatus = Macros.Y
                                    ElseIf m_VUODInfo.UODStatus = Status.UnBooked Then
                                        strUODBkdInStatus = Macros.N
                                    Else
                                        strUODBkdInStatus = Status.Audited
                                    End If
                                    'Invoke the ProcessUODScanned method
                                    VUODSessionMgr.GetInstance.ProcessUODScanned()
                                Else
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M102"), "Alert ", _
                                                    MessageBoxButtons.OK, _
                                                      MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                                    objAppContainer.objStatusBar.SetMessage("")
                                End If
                            End If
                        Else
                            'If UOD barcode not valid then display appropriate error message
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", _
                                             MessageBoxButtons.OK, _
                                                  MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                            objAppContainer.objStatusBar.SetMessage("")
                        End If
                    Case Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M54"), "Error ", _
                                               MessageBoxButtons.OK, _
                                                    MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                        objAppContainer.objStatusBar.SetMessage("")

                End Select
                'Clear the Scannable field text box
                m_ViewUOD.txtProductCode.Text = ""
            End If
        Catch ex As Exception
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at View UOD Handle Scan Data Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub

    ''' <summary>
    ''' Screen Display method for View UOD
    ''' All View UOD sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName">Enum VUODSCREENS</param>
    ''' <returns>True if display is success else False</returns>
    ''' <remarks></remarks>
    Public Function DisplayVUODScreen(ByVal ScreenName As VUODSCREENS) As Boolean
        Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
            Select Case ScreenName
                Case VUODSCREENS.ViewUODMenu
                    m_ViewUODMenu.Invoke(New EventHandler(AddressOf DisplayUODMenu))
                Case VUODSCREENS.ViewUOD
                    m_ViewUOD.Invoke(New EventHandler(AddressOf DisplayVUODScan))
                Case VUODSCREENS.UODDolly
                    m_ViewUODDolly.Invoke(New EventHandler(AddressOf DisplayUODDolly))
                Case VUODSCREENS.UODNonDolly
                    m_ViewUODNotDolly.Invoke(New EventHandler(AddressOf DisplayUODNonDolly))

            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at View UOD Display Screen", Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function

    Public Sub DisplayVDUODScreen()
        DisplayVUODScreen(VUODSCREENS.ViewUOD)
        m_ViewUOD.txtProductCode.Text = ""
    End Sub


    ''' <summary>
    ''' Quiting the VUOD session
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub QuitSession()
        Dim iResult As Integer = 0
        Try


#If RF Then
            'RF RECONNECT
            If Not objAppContainer.bCommFailure Then
                'SETTING STATUSBAR MESSAGE
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
#End If
                iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M103"), "Confirmation", MessageBoxButtons.YesNo, _
                                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                m_ViewUODMenu.UnFreezeControls()
                If iResult = MsgBoxResult.Yes Then
#If RF Then
                    objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
                    'sending GIX for export Data writing
                    If objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.SSC, AppContainer.FunctionType.Audit, AppContainer.IsAbort.No) Then

#ElseIf NRF Then
                    'sending GIX for export Data writing
                    objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.SSC, AppContainer.FunctionType.Audit, AppContainer.IsAbort.No)
#End If


                    m_ViewUODDolly.Close()
                    m_ViewUODNotDolly.Close()
                    m_ViewUODMenu.Close()
                    m_ViewUOD.Close()
                    m_ViewUODDolly.Dispose()
                    m_ViewUODNotDolly.Dispose()
                    m_ViewUODMenu.Dispose()
                        m_ViewUOD.Dispose()
                        m_ItemList = Nothing
                        m_UODList = Nothing
                        'V1.1 KK
                       'Disposing all Dallas related array lists
                        m_DalUODList = Nothing
                        m_DalUODTodayView = Nothing
                        m_DalUODFutureView = Nothing
                        m_CrateList = Nothing
                        m_arrUODListSummary = Nothing
                        m_ViewUOD = Nothing
                        m_ViewUODDolly = Nothing
                        m_ViewUODMenu = Nothing
                        m_ViewUODNotDolly = Nothing
                        m_VUODInfo = Nothing
                        m_ItemInfo = Nothing
                        m_UODSelectionCheck = Nothing

#If RF Then
                    End If
                    objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
                    objAppContainer.bReconnectSuccess = False
                    'RF RECONNECT
                End If
            Else
                'if Connection Lost and user enter View UOD it should not lock him in the VIew UOD menu screen
                iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M103"), "Confirmation", MessageBoxButtons.YesNo, _
                                           MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                m_ViewUODMenu.UnFreezeControls()
                If iResult = MsgBoxResult.Yes Then
                    DisposeVUOD()
                End If
#End If


                End If
#If NRF Then
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
#End If

        Catch ex As Exception

        End Try

    End Sub
    ''' <summary>
    ''' Quiting the VUOD session
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub AutoQuitSession()
        Dim iResult As Integer = 0
        Try
            'sending GIX for export Data writing
            objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.SSC, AppContainer.FunctionType.Audit, AppContainer.IsAbort.No)

            m_ViewUODDolly.Close()
            m_ViewUODNotDolly.Close()
            m_ViewUODMenu.Close()
            m_ViewUOD.Close()
            m_ViewUODDolly.Dispose()
            m_ViewUODNotDolly.Dispose()
            m_ViewUODMenu.Dispose()
            m_ViewUOD.Dispose()
            m_ItemList = Nothing
            m_UODList = Nothing
            'V1.1-KK
            'Disposing all Dallas related variables
            m_DalUODList = Nothing
            m_DalUODTodayView = Nothing
            m_DalUODFutureView = Nothing
            m_CrateList = Nothing
            m_arrUODListSummary = Nothing
            m_ViewUOD = Nothing
            m_ViewUODDolly = Nothing
            m_ViewUODMenu = Nothing
            m_ViewUODNotDolly = Nothing
            m_VUODInfo = Nothing
            m_ItemInfo = Nothing
            m_UODSelectionCheck = Nothing
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at AutoQuitSession" + ex.Message, _
                                                  Logger.LogLevel.RELEASE)
        End Try
    End Sub
#If RF Then
    Public Sub DisposeVUOD()


        m_ViewUODDolly.Close()
        m_ViewUODNotDolly.Close()
        m_ViewUODMenu.Close()
        m_ViewUOD.Close()
        m_ViewUODDolly.Dispose()
        m_ViewUODNotDolly.Dispose()
        m_ViewUODMenu.Dispose()
        m_ViewUOD.Dispose()
        m_ItemList = Nothing
        m_UODList = Nothing
        'V1.1 KK
        'Disposing all Dallas related varibales
        m_DalUODList = Nothing
        m_DalUODTodayView = Nothing
        m_DalUODFutureView = Nothing
        m_CrateList = Nothing
        m_arrUODListSummary = Nothing
        m_ViewUOD = Nothing
        m_ViewUODDolly = Nothing
        m_ViewUODMenu = Nothing
        m_ViewUODNotDolly = Nothing
        m_VUODInfo = Nothing
        m_ItemInfo = Nothing
        m_UODSelectionCheck = Nothing
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
    ''' To Display the Today's or Future UOD List Screen  
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayVUODScan(ByVal o As Object, ByVal e As EventArgs)
        Try
#If RF Then
            objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
#End If
            'SETTING STATUSBAR MESSAGE
            'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
            m_SortedCrateList.Clear()
            ' V1.1 - CK
            ' If the store is enabled for Dallas +ve receiving and the m_DalUODList count
            ' is zero then invoke GetDallasUODSummaryList
            If m_DalUODList.Count = 0 And objAppContainer.bDallasPosReceiptEnabled Then
                VUODSessionMgr.GetInstance.GetDallasUODSummaryList()
            End If
            'Invoke the GetUODSummaryList if arraylist count is zero
            If m_arrUODListSummary.Count = 0 Then
                VUODSessionMgr.GetInstance.GetUODSummaryList(m_strPeriod)
#If RF Then
                If objAppContainer.bCommFailure Then
                    Exit Sub
                End If
#End If
                ' v1.1 - CK
                ' If m_arrUODListSummary and m_DalUODTodayView/m_DalUODFutureView (depending on today/future)
                ' are 0
                If m_arrUODListSummary.Count = 0 And _
                   Convert.ToInt32(IIf(m_strPeriod = "T", m_DalUODTodayView.Count, m_DalUODFutureView.Count)) = 0 Then
#If RF Then
                    If Not objAppContainer.bCommFailure AndAlso Not objAppContainer.bReconnectSuccess Then

                        VUODSessionMgr.GetInstance.DisplayVUODScreen(VUODSCREENS.ViewUODMenu)
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M104"), "Alert", MessageBoxButtons.OK, _
                                                   MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                    objAppContainer.bReconnectSuccess = False
#ElseIf NRF Then
                    VUODSessionMgr.GetInstance.DisplayVUODScreen(VUODSCREENS.ViewUODMenu)
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M104"), "Alert", MessageBoxButtons.OK, _
                                               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
#End If


                    Exit Sub
                End If
            End If
            
            With m_ViewUOD
                .lvwUOD.BeginUpdate()
                If .lvwUOD.Columns.Count = 0 Then
                    Select Case m_strPeriod
                        Case Macros.TODAY
                            .lblUOD.Text = "Today's UODs"
                            'Adding the columns to the Todays UOD list view
                            .lvwUOD.Columns.Add("UOD", 90 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                            .lvwUOD.Columns.Add("Type", 50 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                            .lvwUOD.Columns.Add("Bkd In", 55 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                        Case Else
                            .lblUOD.Text = "Future UODs"
                            'Adding the columns to the Futire UOD list view
                            .lvwUOD.Columns.Add("UOD", 85 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                            .lvwUOD.Columns.Add("Type", 40 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                            .lvwUOD.Columns.Add("Exptd", 68 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                            .lvwUOD.Columns.Add("Bkd In", 54 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    End Select
                End If

                If .lvwUOD.Items.Count <> 0 Then
                    'Clearing the UOD list view items
                    .lvwUOD.Items.Clear()
                End If
                If m_DalUODList.Count <> 0 And objAppContainer.bDallasPosReceiptEnabled Then
                    Select Case m_strPeriod
                        Case "T"
                            Dim objUODListView As ListViewItem = New ListViewItem
                            'Adding items to the Today's UOD list view
                            For Each objDallasUOD As GIValueHolder.DallasDeliverySummary In m_DalUODTodayView
                                objUODListView = m_ViewUOD.lvwUOD.Items.Add(New ListViewItem(objDallasUOD.DallasBarcode.PadLeft(14, "0")))
                                objUODListView.SubItems.Add(ContainerType.Burton)
                                If objDallasUOD.Status = "R" Then
                                    objUODListView.SubItems.Add("Y")
                                ElseIf objDallasUOD.Status = "U" Then
                                    objUODListView.SubItems.Add("N")
                                End If
                            Next
                        Case Else
                            Dim objUODListView As ListViewItem = New ListViewItem
                            'Adding items to the Future UOD list view
                            For Each objDallasUOD As GIValueHolder.DallasDeliverySummary In m_DalUODFutureView
                                objUODListView = m_ViewUOD.lvwUOD.Items.Add(New ListViewItem(objDallasUOD.DallasBarcode.PadLeft(14, "0")))
                                objUODListView.SubItems.Add(ContainerType.Burton)
                                If objDallasUOD.Status = "R" Then
                                    objUODListView.SubItems.Add("Y")
                                ElseIf objDallasUOD.Status = "U" Then
                                    objUODListView.SubItems.Add("N")
                                End If
                            Next
                    End Select
                End If

                If m_arrUODListSummary.Count <> 0 Then
                    Select Case m_strPeriod
                        Case "T"
                            Dim objUODListView As ListViewItem = New ListViewItem
                            'Adding items to the Today's UOD list view
                            For Each objAudit As GIValueHolder.UODList In m_arrUODListSummary
                                objUODListView = m_ViewUOD.lvwUOD.Items.Add(New ListViewItem(objAudit.UODID))
                                objUODListView.SubItems.Add(objAudit.UODType)
                                objUODListView.SubItems.Add(objAudit.BookedIn)
                            Next
                        Case Else
                            Dim objUODListView As ListViewItem = New ListViewItem
                            'Adding items to the Future UOD list view
                            For Each objBkd As GIValueHolder.UODList In m_arrUODListSummary
                                objUODListView = m_ViewUOD.lvwUOD.Items.Add(New ListViewItem(objBkd.UODID))
                                objUODListView.SubItems.Add(objBkd.UODType)
                                objUODListView.SubItems.Add(ReformatDate(objBkd.ExptDate))
                                objUODListView.SubItems.Add(objBkd.BookedIn)
                            Next
                    End Select
                End If
                m_ViewUOD.lvwUOD.EndUpdate()
                'SETTING STATUSBAR MESSAGE
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)

                .Visible = True
                .Refresh()
                objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINVIEWUOD
                'SETTING STATUSBAR MESSAGE
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
            End With
        Catch ex As Exception
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at View UOD Display Scan Screen Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try

    End Sub
    ''' <summary>
    ''' Displaying the UOD dolly Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayUODDolly(ByVal o As Object, ByVal e As EventArgs)
        Try
            'SETTING STATUSBAR MESSAGE
#If RF Then
            objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
#End If
        If m_SortedCrateList.Count = 0 Then
            'Invoke the function if the cratelist count is zero
                If VUODSessionMgr.GetInstance.GetUODListforDollySelected() Then
                    'checking if the UOD scanned or selected is a misdirected or not
                    If m_VUODInfo.UODReason = "M" Then
                        MessageBox.Show("Misdirect. No UOD Details available", "Alert", _
                             MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        m_bMisdirected = True
                        bCheckItem = False
                        Exit Sub
                    End If
                    bCheckItem = True
#If RF Then
                Else
                    If objAppContainer.bReconnectSuccess Then
                        DisplayVUODScreen(VUODSCREENS.ViewUOD)
                    End If
                    objAppContainer.bReconnectSuccess = False
                    objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                    Exit Sub
#End If
                End If
            End If
#If RF Then
            If Not objAppContainer.bCommFailure Then
#End If
                If m_ViewUODDolly.lvwUODDolly.Items.Count > 0 Then
                    'clearing the items of the UOD list view
                    m_ViewUODDolly.lvwUODDolly.Items.Clear()
                End If
                m_ViewUODDolly.lvwUODDolly.BeginUpdate()
                'Adding columns to the list view
                If m_ViewUODDolly.lvwUODDolly.Columns.Count = 0 Then
                    With m_ViewUODDolly
                    .lvwUODDolly.Columns.Add("UOD", 91 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lvwUODDolly.Columns.Add("Type", 51 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lvwUODDolly.Columns.Add("Bkd In", 61 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                        ' .lvwUODDolly.Columns.Add("Items", 50, HorizontalAlignment.Left)
                    End With
                End If
                'Adding items to the UOD dolly list View
                For Each objCrateDetails As GIValueHolder.CrateList In m_SortedCrateList
                    Dim objListView As ListViewItem = New ListViewItem
                    m_ViewUODDolly.lvwUODDolly.Items.Add(New ListViewItem(New String() {objCrateDetails.CrateId, _
                                                                                         objCrateDetails.CrateType, objCrateDetails.BookedIn}))
                    ', objCrateDetails.NoOfItems
                Next
                m_ViewUODDolly.lvwUODDolly.EndUpdate()
                'SETTING STATUSBAR MESSAGE
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)

                With m_ViewUODDolly
                    objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINVIEWUODDOLLY
                    .Visible = True
                    .Refresh()
                    .Focus()
                    objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                End With
#If RF Then
            
            End If
#End If
        Catch ex As Exception
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at View UOD Display UOD Dolly Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
        
    End Sub
    ''' <summary>
    ''' Displaying the UOD non Dolly screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayUODNonDolly(ByVal o As Object, ByVal e As EventArgs)
#If RF Then
        objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
#End If
        'SETTING STATUSBAR MESSAGE
        ' objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
        'Invoke the GetItemList function
        VUODSessionMgr.GetInstance.GetItemList()
        If bCheckItem = False Then
            Exit Sub
        End If
        'SETTING STATUSBAR MESSAGE
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
        With m_ViewUODNotDolly
            objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINVIEWUODNONDOLLY
            objAppContainer.objStatusBar.SetMessage("")
            .Visible = True
            .Refresh()
            .Focus()
        End With
        'SETTING STATUSBAR MESSAGE
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
    End Sub
    Private Sub DisplayUODMenu(ByVal o As Object, ByVal e As EventArgs)
#If RF Then
        objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
#End If
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
        With m_ViewUODMenu
            objAppContainer.objStatusBar.SetMessage("")
            objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINVIEWUODMENU
            .Visible = True
            .Refresh()
#If RF Then
            .Btn_Quit_small1.Enabled = True
            .pnlFutureUOD.Enabled = True
            .pnlTodayUOD.Enabled = True
#End If
           
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        End With
    End Sub
    ''' <summary>
    ''' To perform transition to the Corresponding parent forms when the bacak button is clicked
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Transition()
        If bTransition = False Then
            'Display the View UOD screen
            m_VUODSessionMgr.DisplayVUODScreen(VUODSCREENS.ViewUOD)
        Else
            'Display the View UODDolly screen
            m_VUODSessionMgr.DisplayVUODScreen(VUODSCREENS.UODDolly)
        End If
    End Sub
    ''' <summary>
    ''' Get the container type based on the UOD type
    ''' </summary>
    ''' <param name="strUODType"></param>
    ''' <param name="strContainerType"></param>
    ''' <remarks></remarks>
    Public Sub GetContainerType(ByVal strUODType As String, ByRef strContainerType As String)
        If strUODType = ContainerType.RollCage Then
            strContainerType = ContainerNam.Rollcage
        ElseIf strUODType = ContainerType.Crate Then
            strContainerType = ContainerNam.Crate
        ElseIf strUODType = ContainerType.Outer Then
            strContainerType = ContainerNam.Outer
        ElseIf strUODType = ContainerType.Pallet Then
            strContainerType = ContainerNam.Pallet
        ElseIf strUODType = ContainerType.IST Then
            strContainerType = ContainerNam.IST
        End If
    End Sub
    ''' <summary>
    ''' Process the details of the UOD scanned
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessUODScanned()
        Try
            Dim strUODId As String = ""
            Dim strUODType As String = ""
            Dim strUODBkdStatus As String = ""
            Dim strUODBkdInDate As String = ""
            Dim strUODExptDate As String = ""
            Dim strUODCOntainerType As String = ""

            strUODId = m_VUODInfo.UODNumber.ToString()
            strUODType = m_VUODInfo.UODType.ToString
            strUODBkdStatus = m_VUODInfo.UODStatus.ToString()
            'formatstrDate is invoked to change the string data into date format
            strUODExptDate = ReformatDate(m_VUODInfo.ExpectedDeliveryDate).ToString
            If m_VUODInfo.UODType = ContainerType.Dolly Then

#If RF Then
                strSequence = m_VUODInfo.SequenceNumber.ToString
#End If

                m_VUODSessionMgr.bTransition = True
                'Display the UODDolly screen
                m_VUODSessionMgr.DisplayVUODScreen(VUODSCREENS.UODDolly)
                With m_ViewUODDolly
                    .lblTypeValue.Text = "Dolly"
                    .lblUODValue.Text = strUODId.ToString
                    'Display the Booked In status label 
                    If strUODBkdStatus = Macros.Y Then
                        .lblBkdInValue.Text = "Yes"
                        'Invoke the ReformatDate to format string data into Date format
                        strUODBkdInDate = ReformatDate(m_VUODInfo.BookInDate).ToString
                        .lblBkdDateValue.Text = strUODBkdInDate.ToString
                        .lblBkdDateValue.Visible = True
                        .lblBkdDateText.Visible = True
                    ElseIf strUODBkdStatus = Status.Audited Then
                        .lblBkdInValue.Text = "Audit"
                        strUODBkdInDate = ReformatDate(m_VUODInfo.BookInDate).ToString
                        .lblBkdDateValue.Text = strUODBkdInDate.ToString
                        .lblBkdDateValue.Visible = True
                        .lblBkdDateText.Visible = True
                    Else
                        .lblBkdInValue.Text = "No"
                        .lblBkdDateValue.Visible = False
                        .lblBkdDateText.Visible = False
                    End If
                    .lblExptdDateValue.Text = strUODExptDate.ToString
                End With
                m_VUODSessionMgr.bTransition = True
                'Display the UODDolly screen
                m_VUODSessionMgr.DisplayVUODScreen(VUODSCREENS.UODDolly)
            Else
                strCrateId = strUODId.ToString
#If RF Then
                strSequence = m_VUODInfo.SequenceNumber.ToString()
#End If
                m_VUODSessionMgr.bTransition = False
                m_ViewUODDolly.bDollySelected = False
                'To display the Non dolly screen
                '  m_VUODSessionMgr.DisplayVUODScreen(VUODSCREENS.UODNonDolly)
                With m_ViewUODNotDolly
                    .lblUODValue.Text = strCrateId.ToString
                    'Invoking GetContainerType function to get the Container type
                    GetContainerType(strUODType, strUODCOntainerType)
                    .lblTypeValue.Text = strUODCOntainerType.ToString()
                    .lblExptdDateValue.Text = strUODExptDate.ToString()
                    If strUODBkdStatus = Status.Booked Then
                        .lblStatusValue.Text = "Yes"
                        strUODBkdInDate = ReformatDate(m_VUODInfo.BookInDate).ToString
                        .lblBkdDateValue.Text = strUODBkdInDate.ToString
                        .lblBkdDateValue.Visible = True
                        .lblBkdDateText.Visible = True
                    ElseIf strUODBkdStatus = Status.Audited Then
                        .lblStatusValue.Text = "Audit"
                        strUODBkdInDate = ReformatDate(m_VUODInfo.BookInDate).ToString
                        .lblBkdDateValue.Text = strUODBkdInDate.ToString
                        .lblBkdDateValue.Visible = True
                        .lblBkdDateText.Visible = True
                    Else
                        .lblStatusValue.Text = "No"
                        .lblBkdDateValue.Visible = False
                        .lblBkdDateText.Visible = False
                    End If
                End With
                m_VUODSessionMgr.bTransition = False
                m_ViewUODDolly.bDollySelected = False
                'Invoke the GetItemList method 
                m_VUODSessionMgr.DisplayVUODScreen(VUODSCREENS.UODNonDolly)
            End If
        Catch ex As Exception
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at View UOD Process UOD Scan Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try

    End Sub
    ''' <summary>
    ''' 'formatstrDate is invoked to convert the yymmdd string format
    ''' into ddmmyy date format
    ''' </summary>
    ''' <param name="strDate"></param>
    ''' <returns>strDate</returns>
    ''' <remarks></remarks>
    Public Function ReformatDate(ByRef strDate As String) As String
        If strDate <> Nothing Then
            If strDate.Length = 6 Then
                Dim yy As String = strDate.Substring(0, 2)
                Dim mm As String = strDate.Substring(2, 2)
                Dim dd As String = strDate.Substring(4, 2)
                strDate = dd + "/" + mm + "/" + yy
                Return strDate
            Else
                Return strDate
            End If
        Else
            Return " "
        End If

    End Function
    ''' <summary>
    ''' To Populate Item list View details
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GetItemList()
        Dim strUODCrateId As String = ""
        Dim strCrateBkdInStatus As String = ""
        Dim strCrateType As String = ""
        Dim strCrateNoOfItems As String = ""
        Dim bIsDataAvailable As Boolean = False
        Dim strNonDollyId As String = ""
        Dim strContainerType As String = ""
        Dim objItemComparer As New ItemComparer
        Try
            'getting data based on the UOD selected 
            If m_ViewUODDolly.bDollySelected = True Then
                With m_ViewUODDolly
                    Dim iCounter As Integer = 0
                    If .lvwUODDolly.SelectedIndices.Count > 0 Then
                        For iCounter = 0 To .lvwUODDolly.Items.Count - 1
                            If .lvwUODDolly.Items(iCounter).Selected Then
                                bIsDataAvailable = True
                                'Get the CrateId,Crate type,status and no of items of the UOD selected
                                strUODCrateId = .lvwUODDolly.Items(iCounter).Text
                                strCrateType = .lvwUODDolly.Items(iCounter).SubItems(1).Text
                                strCrateBkdInStatus = .lvwUODDolly.Items(iCounter).SubItems(2).Text
                                'strCrateNoOfItems = .lvwUODDolly.Items(iCounter).SubItems(3).Text
                                Exit For
                            End If
                        Next
                    Else
                        strUODCrateId = strUODDollyId.ToString()
                        strCrateType = strType.ToString()
                        strCrateBkdInStatus = strUODBkdInStatus.ToString()
                    End If
                End With

            Else
                strUODCrateId = strUODDollyId.ToString
            End If
            With m_ViewUODNotDolly
                'Clearing the Item of the Item Listview
                .lvwUODNotDolly.Items.Clear()
                .lvwUODNotDolly.BeginUpdate()
                'Adding Columns to the Item Listview
                If .lvwUODNotDolly.Columns.Count = 0 Then
                    .lvwUODNotDolly.Columns.Add("Item Code", 80 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lvwUODNotDolly.Columns.Add("Desc", 180 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                    .lvwUODNotDolly.Columns.Add("Qty", 40 * objAppContainer.iOffSet, HorizontalAlignment.Left)
                End If
            End With
            'Retrieving Details for the respective UOD selected
            If objAppContainer.objDataEngine.GetItemListForView(strUODCrateId, m_ItemList, m_VUODInfo, strSequence) Then
                bCheckItem = True
                'Populating the Item ListView
                m_ItemList.Sort(0, m_ItemList.Count, objItemComparer)
                For Each ObjItem As GIValueHolder.ItemList In m_ItemList
                    Dim objUODItemList As ListViewItem = New ListViewItem
                    objUODItemList = m_ViewUODNotDolly.lvwUODNotDolly.Items.Add(New ListViewItem(ObjItem.ItemCode))
                    objUODItemList.SubItems.Add(ObjItem.ItemDesc)
                    objUODItemList.SubItems.Add(ObjItem.ItemQty)
                Next
                '---------------------------------
                With m_ViewUODNotDolly
                    .lblBkdDateText.Visible = True
                    .lblBkdDateValue.Visible = True
                    .lblUODValue.Text = strUODCrateId.ToString()
                    .lblExptdDateValue.Text = ReformatDate(m_VUODInfo.ExpectedDeliveryDate.ToString()).ToString()
                    strCrateBkdInStatus = m_VUODInfo.UODStatus.ToString()
                    'GetContainerType is invoked to get the UOD type and display their respective container names
                    GetContainerType(m_VUODInfo.UODType, strContainerType)
                    .lblTypeValue.Text = strContainerType.ToString()
                    .lblBkdDateValue.Text = ReformatDate(m_VUODInfo.BookInDate.ToString())
                    'checking the Booked in status of the UOD selected
                    If strCrateBkdInStatus = Macros.Y Then
                        .lblStatusValue.Text = "Yes"
                        .lblBkdDateText.Visible = True
                        .lblBkdDateValue.Visible = True
                    ElseIf strCrateBkdInStatus = Status.Audited Then
                        .lblStatusValue.Text = "Audit"
                        .lblBkdDateText.Visible = True
                        .lblBkdDateValue.Visible = True
                    Else
                        'If Booked in status is "No" then Booked in date should not be visible
                        .lblStatusValue.Text = "No"
                        'If m_strPeriod = "F" Then
                        .lblBkdDateText.Visible = False
                        .lblBkdDateValue.Visible = False
                        'End If
                    End If
                    ' m_ViewUODDolly.Visible = False
                End With
                '---------------------------------
            Else

#If RF Then
                If Not objAppContainer.bCommFailure Then
                    If Not objAppContainer.bReconnectSuccess Then
                        MessageBox.Show("UOD Details not available", "Alert", _
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If
                    objAppContainer.bReconnectSuccess = False
                End If
#ElseIf NRF Then
                 MessageBox.Show("UOD Details not available", "Alert", _
                         MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
#End If

                    bCheckItem = False
                    objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                    Exit Sub
                End If
                m_ViewUODNotDolly.lvwUODNotDolly.EndUpdate()
                'Clearing the ArrayList
                m_ItemList.Clear()
                m_VUODSessionMgr.bUODScanned = False
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at View UOD Get Item List Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To Retrieve Container data based on the UOD selected.
    ''' Sorting the data
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayContainerInfo()
        Dim strUODId As String = ""
        Dim strContainerType As String = ""
        Dim bIsDataAvailable As Boolean = False
        Dim strReason As String = " "
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
        With m_ViewUOD
            Dim iCounter As Integer = 0
            .lvwUOD.Refresh()
            'Retrieving data of the UOD based on the UOD selected 
            If .lvwUOD.SelectedIndices.Count > 0 Then
                'iCounter is the index of the UOD selected
                For iCounter = 0 To .lvwUOD.Items.Count - 1
                    If .lvwUOD.Items(iCounter).Selected Then
                        m_UODSelectionCheck = "Y"
                        bIsDataAvailable = True
                        strUODId = .lvwUOD.Items(iCounter).Text
                        If (strUODId = "") Then
                            Exit Sub
                        End If
                        ' V1.1 - CK
                        ' If the selected one is Dallas barcode then exit method after showing the message
                        If strUODId.StartsWith("0501") And strUODId.Length = 14 Then
#If RF Then
                            DisplayMsgBox(MessageManager.GetInstance().GetMessage("M136"), "VIEW NOT AVAILABLE", MsgBx.BUTTON_TYPE.CONTINE, 10)
#End If
#If NRF Then
                            MsgBx.DisplayMessage(MessageManager.GetInstance().GetMessage("M136"), "VIEW NOT AVAILABLE", MsgBx.BUTTON_TYPE.CONTINE)
                            m_VUODSessionMgr.DisplayVUODScreen(VUODSCREENS.ViewUOD)
#End If
                            m_UODSelectionCheck = "N"
                            Exit Sub
                        End If
                        strUODType = .lvwUOD.Items(iCounter).SubItems(1).Text
                        Exit For
                    End If
                Next
            End If
        End With
        Dim ArrUODList As New ArrayList
        For Each obj As GIValueHolder.UODList In m_UODList
            If obj.UODID = strUODId Then
                strExptdDate = ReformatDate(obj.ExptDate).ToString()
                ArrUODList.Add(obj)
                strSequence = obj.Sequencenumber
                'strReason = obj.Reason
                If obj.UODID.Substring(0, 1) = ConfigDataMgr.GetInstance().GetParam(ConfigKey.DOLLYID) Then
                    strType = ContainerType.Dolly
                Else
                    Select Case obj.UODID.Substring(0, 1)
                        Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.CRATEID)
                            strType = ContainerType.Crate
                        Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.ROLLYCAGEID)
                            strType = ContainerType.RollCage
                        Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.PALLETSID)
                            strType = ContainerType.Pallet
                        Case ConfigDataMgr.GetInstance().GetParam(ConfigKey.OUTERSID)
                            If obj.UODID.Substring(0, 4) = "8888" Then
                                strType = ContainerType.IST
                            Else
                                strType = ContainerType.Outer
                            End If
                    End Select

                End If
            End If
        Next
        
        'Displaying the UOD types based on the UOD type variable
        If strType = ContainerType.Dolly Then
            strUODDollyId = strUODId.ToString()
            'Set the transition boolean variable to True
            bTransition = True

            DisplayVUODScreen(VUODSCREENS.UODDolly)
            'FIx for OK Click after Mis Directed UOD message box going to Main Screen.
            If bCheckItem = False Then
                m_UODSelectionCheck = "N"
                Exit Sub
            End If
#If RF Then
            If Not objAppContainer.bCommFailure Then
#End If
                strExptdDate = ReformatDate(m_VUODInfo.ExpectedDeliveryDate).ToString()
                Select Case m_VUODInfo.UODStatus
                    Case "B"
                        strUODBkdInStatus = Macros.Y
                    Case "U"
                        strUODBkdInStatus = Macros.N
                    Case "A"
                        strUODBkdInStatus = Status.Audited
                        'Fix for Partial UOD
                    Case "P"
                        strUODBkdInStatus = Macros.N
                    Case Else
                        strUODBkdInStatus = m_VUODInfo.UODStatus.ToString()
                End Select
                strType = m_VUODInfo.UODType.ToString()
                'IF Booked in status is No then the Booked in date should not be visible
                If strUODBkdInStatus = Macros.N Then
                    With m_ViewUODDolly
                        .lblBkdDateText.Visible = False
                        .lblBkdDateValue.Visible = False
                    End With
                    With m_ViewUODNotDolly
                        .lblBkdDateValue.Visible = False
                        .lblBkdDateText.Visible = False
                    End With
                Else
                    strBkdDate = ReformatDate(m_VUODInfo.BookInDate).ToString()
                    m_ViewUODDolly.lblBkdDateValue.Text = strBkdDate
                    m_ViewUODDolly.lblBkdDateValue.Visible = True
                    m_ViewUODDolly.lblBkdDateText.Visible = True
                End If
                With m_ViewUODDolly
                    .lblExptdDateValue.Text = strExptdDate.ToString()
                    .lblTypeValue.Text = "Dolly"
                    .lblUODValue.Text = strUODId.ToString()
                End With
                'Displaying the Booked in Status based on the Status variable 
                If strUODBkdInStatus = Macros.Y Then
                    m_ViewUODDolly.lblBkdInValue.Text = "Yes"
                    m_ViewUODDolly.lblBkdDateText.Visible = True
                    m_ViewUODDolly.lblBkdDateValue.Visible = True
                ElseIf strUODBkdInStatus = Status.Audited Then
                    m_ViewUODDolly.lblBkdInValue.Text = "Audit"
                    m_ViewUODDolly.lblBkdDateText.Visible = True
                    m_ViewUODDolly.lblBkdDateValue.Visible = True
                Else
                    ' If status is No then set Booked in date label visibity to false
                    m_ViewUODDolly.lblBkdInValue.Text = "No"
                    m_ViewUODDolly.lblBkdDateText.Visible = False
                    m_ViewUODDolly.lblBkdDateValue.Visible = False
                End If
#If RF Then
            End If
#End If

            Else
                With m_ViewUODNotDolly
                    bTransition = False
                    strUODDollyId = strUODId.ToString()
                    'Displaying the UODNonDolly Screen
                DisplayVUODScreen(VUODSCREENS.UODNonDolly)
#If RF Then
                If Not objAppContainer.bCommFailure Then
#End If
                    If bCheckItem = False Then
                        m_UODSelectionCheck = "N"
                        Exit Sub
                    End If
                    strExptdDate = ReformatDate(m_VUODInfo.ExpectedDeliveryDate).ToString()
                    Select Case m_VUODInfo.UODStatus
                        Case "B"
                            strUODBkdInStatus = Macros.Y
                        Case "U"
                            strUODBkdInStatus = Macros.N
                        Case "A"
                            strUODBkdInStatus = Status.Audited
                        Case Else
                            strUODBkdInStatus = m_VUODInfo.UODStatus.ToString()
                    End Select

                    strType = m_VUODInfo.UODType.ToString()
                    strBkdInDate = ReformatDate(m_VUODInfo.BookInDate).ToString()
                    'If UOD type variable is not "D" displaying the corresponding UOD type
                    GetContainerType(strType, strContainerType)
                    .lblTypeValue.Text = strContainerType.ToString()
                    .lblUODValue.Text = strUODId.ToString()
                    'Displaying the Expected date
                    .lblExptdDateValue.Text = strExptdDate.ToString()
                    .lblBkdDateValue.Text = strBkdInDate.ToString()
                    ' Display the status label depending on the status variable
                    If strUODBkdInStatus = Macros.Y Then
                        .lblStatusValue.Text = "Yes"
                        .lblBkdDateText.Visible = True
                        .lblBkdDateValue.Visible = True
                    ElseIf strUODBkdInStatus = Status.Audited Then
                        .lblStatusValue.Text = "Audit"
                        .lblBkdDateText.Visible = True
                        .lblBkdDateValue.Visible = True
                    Else
                        ' If status is No then set Booked in date label visibity to false
                        .lblStatusValue.Text = "No"
                        If m_strPeriod = "F" Then
                            .lblBkdDateText.Visible = False
                            .lblBkdDateValue.Visible = False
                        End If
                End If
#If RF Then
                        End If
#End If
                End With
            
            End If

    End Sub
    ''' <summary>
    ''' Getting the UOD list based on the UOD selected
    ''' </summary>
    ''' <remarks></remarks>
    Public Function GetUODListforDollySelected() As Boolean
        Dim arrCrateList As New ArrayList
        Dim objUODComparer As New ContainerComparer
        Dim ArrCratetempList As New ArrayList
        Dim ArrAuditList As New ArrayList
        Dim ArrBookedList As New ArrayList
        Dim ArrUnBookedList As New ArrayList

        Try
            'Filling the Arraylist with the data of tbe UOD id selected or scanned
            If objAppContainer.objDataEngine.GetCrateListForView(strUODDollyId, m_CrateList, m_VUODInfo, strSequence) Then
                For Each strObj As String In strUODTypes
                    'Clearing the Arraylist
                    ArrCratetempList.Clear()
                    ArrAuditList.Clear()
                    ArrBookedList.Clear()
                    ArrUnBookedList.Clear()
                    'Sorting the Cratelist based on the container types
                    For Each objCrateTempList As GIValueHolder.CrateList In m_CrateList
                        If objCrateTempList.CrateType = strObj Then
                            ArrCratetempList.Add(objCrateTempList)
                        End If
                    Next
                    If ArrCratetempList.Count > 0 Then
                        For Each objStatus As GIValueHolder.CrateList In ArrCratetempList

                            Select Case objStatus.BookedIn
                                Case "A"
                                    ArrAuditList.Add(objStatus)
                                Case "Y"
                                    ArrBookedList.Add(objStatus)
                                Case "N"
                                    ArrUnBookedList.Add(objStatus)

                            End Select

                        Next

                        If ArrAuditList.Count > 0 Then
                            For Each objAuditTemp As GIValueHolder.CrateList In ArrAuditList
                                arrCrateList.Add(objAuditTemp)
                            Next
                            arrCrateList.Sort(0, arrCrateList.Count, objUODComparer)
                            m_SortedCrateList.AddRange(arrCrateList)
                            arrCrateList.Clear()
                        End If
                        If ArrBookedList.Count > 0 Then
                            For Each objBkd As GIValueHolder.CrateList In ArrBookedList
                                arrCrateList.Add(objBkd)
                            Next
                            arrCrateList.Sort(0, arrCrateList.Count, objUODComparer)
                            m_SortedCrateList.AddRange(arrCrateList)
                            arrCrateList.Clear()
                        End If
                        If ArrUnBookedList.Count > 0 Then
                            For Each objUnBkd As GIValueHolder.CrateList In ArrUnBookedList
                                arrCrateList.Add(objUnBkd)
                            Next
                            arrCrateList.Sort(0, arrCrateList.Count, objUODComparer)
                            m_SortedCrateList.AddRange(arrCrateList)
                            arrCrateList.Clear()
                        End If
                    End If
                Next
                m_CrateList.Clear()
                m_VUODSessionMgr.bUODScanned = False
            Else
                Return False
            End If
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at View UOD Get List for Dolly Selected Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
            Return False

        End Try
        Return True
    End Function
    ''' <summary>
    ''' To Populate the UOD list
    ''' </summary>
    ''' <param name="strPeriod"></param>
    ''' <remarks></remarks>
    Public Sub GetUODSummaryList(ByVal strPeriod As String)
        Dim arrUODList As New ArrayList
        Dim objUODComparer As New UODComparer
        Dim strStatus As String = ""
        Dim ArrUODTempList As New ArrayList
        Dim ArrAuditList As New ArrayList
        Dim ArrBookedList As New ArrayList
        Dim ArrUnBookedList As New ArrayList

        Try
            If Not objAppContainer.objDataEngine.GetUODListForView(strPeriod, m_UODList) Then
                Return
            End If

            If Not m_UODList.Count > 0 Then
                Return
            End If
            For Each strObj As String In strUODTypes
                'Clearing the Arraylist
                ArrUODTempList.Clear()
                ArrAuditList.Clear()
                ArrBookedList.Clear()
                ArrUnBookedList.Clear()
                arrUODList.Clear()
                'Sorting the UOD list based on the Container types 
                For Each objUODTempList As GIValueHolder.UODList In m_UODList
                    If objUODTempList.UODType = strObj Then
                        ArrUODTempList.Add(objUODTempList)
                    End If
                Next
                If ArrUODTempList.Count > 0 Then
                    For Each objAudit As GIValueHolder.UODList In ArrUODTempList
                        If objAudit.BookedIn = "A" Then
                            ArrAuditList.Add(objAudit)
                        End If
                    Next
                    For Each objBooked As GIValueHolder.UODList In ArrUODTempList
                        If objBooked.BookedIn = "Y" Then
                            ArrBookedList.Add(objBooked)
                        End If
                    Next
                    For Each ObjUnBooked As GIValueHolder.UODList In ArrUODTempList
                        If ObjUnBooked.BookedIn = "N" Then
                            ArrUnBookedList.Add(ObjUnBooked)
                        End If
                    Next
                    If ArrAuditList.Count > 0 Then
                        For Each objAuditTemp As GIValueHolder.UODList In ArrAuditList
                            arrUODList.Add(objAuditTemp)
                        Next
                        arrUODList.Sort(0, arrUODList.Count, objUODComparer)
                        m_arrUODListSummary.AddRange(arrUODList)
                        arrUODList.Clear()
                    End If
                    If ArrBookedList.Count > 0 Then
                        For Each objBkd As GIValueHolder.UODList In ArrBookedList
                            arrUODList.Add(objBkd)
                        Next
                        arrUODList.Sort(0, arrUODList.Count, objUODComparer)
                        m_arrUODListSummary.AddRange(arrUODList)
                        arrUODList.Clear()
                    End If
                    If ArrUnBookedList.Count > 0 Then
                        For Each objUnBkd As GIValueHolder.UODList In ArrUnBookedList
                            arrUODList.Add(objUnBkd)
                        Next
                        arrUODList.Sort(0, arrUODList.Count, objUODComparer)
                        m_arrUODListSummary.AddRange(arrUODList)
                        arrUODList.Clear()
                    End If
                End If
            Next
        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at View UOD Get UOD Summary List Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.

        End Try
    End Sub

    Public Sub GetDallasUODSummaryList()
        'V1.1 - KK
        'New function added for Populating Dallas UOD details in View list Menu
        Dim iDateCompare As Integer
        Dim objDallasUODComparer As New DallasUODComparer
        Dim dtToday As DateTime
        Dim dtExpectedDelDate As DateTime
        Dim m_arrDalListTemp As New ArrayList
        Dim arrDallasReceiptedList As New ArrayList
        Dim arrDallasUnReceiptedList As New ArrayList
        Try
            If objAppContainer.bDallasPosReceiptEnabled Then
                If Not objAppContainer.objDataEngine.GetDallasUODListForView(m_arrDalListTemp) Then
                    Return
                End If

                For Each objReceipted As GIValueHolder.DallasDeliverySummary In m_arrDalListTemp
                    If objReceipted.Status = "R" Then
                        arrDallasReceiptedList.Add(objReceipted)
                    End If
                Next

                For Each objUnreceipted As GIValueHolder.DallasDeliverySummary In m_arrDalListTemp
                    If objUnreceipted.Status = "U" Then
                        arrDallasUnReceiptedList.Add(objUnreceipted)
                    End If
                Next

                ' Sorting the Burton deliveries that are receipted based on their UOD no.s and adding
                ' to the arraylist
                If arrDallasReceiptedList.Count > 0 Then
                    arrDallasReceiptedList.Sort(0, arrDallasReceiptedList.Count, objDallasUODComparer)
                    m_DalUODList.AddRange(arrDallasReceiptedList)
                End If

                ' Sorting the Burton deliveries that are unreceipted based on their UOD no.s and adding
                ' to the arraylist
                If arrDallasUnReceiptedList.Count > 0 Then
                    arrDallasUnReceiptedList.Sort(0, arrDallasUnReceiptedList.Count, objDallasUODComparer)
                    m_DalUODList.AddRange(arrDallasUnReceiptedList)
                End If
            End If

            If m_DalUODList.Count > 0 Then
                For Each objDallasDeliverySummary As GIValueHolder.DallasDeliverySummary In m_DalUODList
                    dtToday = DateTime.Now.Date
                    dtExpectedDelDate = DateTime.ParseExact(objDallasDeliverySummary.ExpectedDelDate, "yyMMdd", CultureInfo.InvariantCulture)
                    iDateCompare = DateTime.Compare(dtExpectedDelDate, dtToday)
                    If iDateCompare = 0 Then
                        m_DalUODTodayView.Add(objDallasDeliverySummary)
                    ElseIf iDateCompare < 0 And objDallasDeliverySummary.Status = "U" Then
                        m_DalUODTodayView.Add(objDallasDeliverySummary)
                    ElseIf iDateCompare > 0 Then
                        m_DalUODFutureView.Add(objDallasDeliverySummary)
                    End If
                Next
            End If

        Catch ex As Exception
            'Add the exception to the application log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Exception occured at View UOD Get DallasUOD Summary List Session: " + ex.ToString(), Logger.LogLevel.RELEASE)
            'return false after logging the exception to the log.
        End Try
    End Sub

    Public Enum VUODSCREENS
        ViewUOD
        UODDolly
        UODNonDolly
        ViewUODMenu
        Calcpad
    End Enum

    Public Sub New()

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class

Public Class ItemInfo1
    Private m_itemQty As Integer
    Private m_productCode As String
    Private m_productDesc As String
    Private m_productName As String



    Public Property ItemQty() As Integer
        Get
            Return m_itemQty
        End Get
        Set(ByVal value As Integer)
            m_itemQty = value
        End Set
    End Property


    Public Property ProductCode() As String
        Get
            Return m_productCode
        End Get
        Set(ByVal value As String)
            m_productCode = value
        End Set
    End Property

    Public Property ProductDescription() As String
        Get
            Return m_productDesc
        End Get
        Set(ByVal value As String)
            m_productDesc = value
        End Set
    End Property

    Public Property ProductName() As String
        Get
            Return m_productName
        End Get
        Set(ByVal value As String)
            m_productName = value
        End Set
    End Property
End Class
Public Class UODComparer
    Implements IComparer
    Private bSorting As Boolean = False
    ''' <summary>
    ''' To sort the UOD Arraylist based on the UOD code
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
        Dim objItemX As GIValueHolder.UODList = DirectCast(x, GIValueHolder.UODList)
        Dim objItemY As GIValueHolder.UODList = DirectCast(y, GIValueHolder.UODList)
        Return IIf(True, Compare(objItemX, objItemY), Compare(objItemY, objItemX))
    End Function
    Public Function Compare(ByVal objItemA As GIValueHolder.UODList, ByVal objItemB As GIValueHolder.UODList) As Integer
        Return Int64.Parse(objItemA.UODID).CompareTo(Int64.Parse(objItemB.UODID))
    End Function
End Class
' V1.1 - CK
' New class DallasUODComparer for comparing Dallas UODs
Public Class DallasUODComparer
    Implements IComparer
    ''' <summary>
    ''' To sort the UOD Arraylist based on the UOD code
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
        Dim objItemX As GIValueHolder.DallasDeliverySummary = DirectCast(x, GIValueHolder.DallasDeliverySummary)
        Dim objItemY As GIValueHolder.DallasDeliverySummary = DirectCast(y, GIValueHolder.DallasDeliverySummary)
        Return Compare(objItemX, objItemY)
    End Function
    Public Function Compare(ByVal objItemA As GIValueHolder.DallasDeliverySummary, ByVal objItemB As GIValueHolder.DallasDeliverySummary) As Integer
        Return Int64.Parse(objItemA.DallasBarcode).CompareTo(Int64.Parse(objItemB.DallasBarcode))
    End Function
End Class
Public Class ItemComparer
    Implements IComparer
    Public Function Compare(ByVal objx As Object, ByVal objy As Object) As Integer Implements IComparer.Compare
        If TypeOf objx Is GIValueHolder.ItemList Then
            Dim objItemX As GIValueHolder.ItemList = DirectCast(objx, GIValueHolder.ItemList)
            Dim objItemY As GIValueHolder.ItemList = DirectCast(objy, GIValueHolder.ItemList)
            Return String.Compare(objItemX.ItemDesc, objItemY.ItemDesc)
        End If
    End Function
End Class
Public Class ContainerComparer
    Implements IComparer
    Private bSorting As Boolean = False
    ''' <summary>
    ''' To sort the UOD Arraylist based on the UOD code
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
        Dim objItemX As GIValueHolder.CrateList = DirectCast(x, GIValueHolder.CrateList)
        Dim objItemY As GIValueHolder.CrateList = DirectCast(y, GIValueHolder.CrateList)
        Return IIf(True, Compare(objItemX, objItemY), Compare(objItemY, objItemX))
    End Function
    Public Function Compare(ByVal objItemA As GIValueHolder.CrateList, ByVal objItemB As GIValueHolder.CrateList) As Integer
        Return Int64.Parse(objItemA.CrateId).CompareTo(Int64.Parse(objItemB.CrateId))
    End Function
End Class
