'''***************************************************************
''' <FileName>ISSessionMgr.vb</FileName>
''' <summary>
''' The Item Sales Container Class.
''' Implements all business logic and GUI navigation for Item Sales.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author> 
''' <DateModified>18-Nov-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
''' 
#If RF Then
Public Class ISSessionManager
    Implements IISInterface
#Region "Variables"
    Private Shared m_ISSessionMgr As ISSessionManager
    Private m_IShome As frmISHome
    Private m_ISItemDetails As frmISItemDetails
    Private m_ItemSalesInfo As ItemSalesInfo
    Private m_strSEL As String
#End Region
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
#Region "Constructor"
    Private Sub New()
    End Sub
#End Region

#Region "Methods"
#If RF Then
    ''' <summary>
    ''' Updates the Status bar of all the forms in the session manager
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateStatusBarMessage()
        Try
            m_IShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            m_ISItemDetails.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception Occured, Trace : " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End If

    Public Function StartSession() As Boolean Implements ISMInterface.StartSession
        objAppContainer.objLogger.WriteAppLog("Enter Item Sales Start session", Logger.LogLevel.INFO)
        Try
#If RF Then
            If Not (objAppContainer.objExportDataManager.CreateINS()) Then
                objAppContainer.objLogger.WriteAppLog("Cannot Create INS record at Item Sales Start Session", Logger.LogLevel.RELEASE)
                Return False
            End If
#End If
            'Initialisation of all Item sales details
            m_IShome = New frmISHome()
            m_ISItemDetails = New frmISItemDetails()
            m_ItemSalesInfo = New ItemSalesInfo
            m_strSEL = ""
            Me.DisplayISScreen(ISSCREENS.Home)
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at Item Sales Start Session: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Item Sales Start Session", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Gracefully Terminate all forms and objects created by ItemSalesSessionMgr.
    ''' </summary>
    ''' <remarks></remarks>
#If NRF Then
        Public Function EndSession()as Boolean 
#ElseIf RF Then
    Public Function EndSession(Optional ByVal isConnectivityLoss As Boolean = False) As Boolean
#End If
        objAppContainer.objLogger.WriteAppLog("Enter Item Sales End Session", Logger.LogLevel.INFO)
        Try
            Dim objResponse As Object = Nothing
#If RF Then
            If Not isConnectivityLoss Then
                If Not objAppContainer.objExportDataManager.CreateINX() Then
                    objAppContainer.objLogger.WriteAppLog("Cannot create INX record in Itemsales End Session", Logger.LogLevel.RELEASE)
                    Return False
                End If
            End If
#End If
            m_IShome.Dispose()
            m_ISItemDetails.Dispose()
            m_ItemSalesInfo = Nothing
            m_strSEL = Nothing
            m_ISSessionMgr = Nothing
            Return True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception at Item Sales End Session", Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Item Sales End Session", Logger.LogLevel.INFO)
    End Function
    ''' <summary>
    ''' Functions for getting the object instance for the ItemSalesSessionMgr. 
    ''' Use this method to get the object reference for the Singleton ItemSalesSessionMgr.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Object reference of ItemSalesSessionMgr Class</remarks>
    Public Shared Function GetInstance() As ISSessionManager
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.ITEMSALES
        If m_ISSessionMgr Is Nothing Then
            m_ISSessionMgr = New ISSessionManager()
            Return m_ISSessionMgr
        Else
            Return m_ISSessionMgr
        End If
    End Function
    ''' <summary>
    ''' The Method handles the data returned form the barcode scanner.
    ''' This method implements the business logic to populate the data to the corresponding
    ''' UI element after validation.
    ''' </summary>
    ''' <param name="strBarcode"></param>
    ''' <param name="Type"></param>
    ''' <remarks></remarks>
    Public Sub HandleScanData(ByVal strBarcode As String, ByVal Type As BCType) Implements IScanInterface.HandleScanData
        objAppContainer.objLogger.WriteAppLog("Enter Item Sales HandleScanData", Logger.LogLevel.RELEASE)
        m_IShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        Try
            Select Case Type
                Case BCType.EAN
                    If Not (objAppContainer.objHelper.ValidateEAN(strBarcode)) Or _
                      Val(strBarcode) = 0 Then
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        m_IShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        Return
                    Else
                        strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                        If Not (objAppContainer.objDataEngine.GetItemSalesDetailsAllUsingPC(strBarcode, m_ItemSalesInfo)) Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                            m_IShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Exit Sub
                        End If
                    End If
                Case BCType.ManualEntry
                    Dim strBootsCode As String = ""
                    If strBarcode.Length < 8 Then
                        'strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBarcode)
                        'strBarcode = strBarcode.Substring(0, 6)
                        'strBarcode = strBarcode.PadLeft(12, "0")
                        If objAppContainer.objHelper.ValidateBootsCode(strBarcode) Then
                            If Not (objAppContainer.objDataEngine.GetItemSalesDetailsAllUsingBC(strBarcode, m_ItemSalesInfo)) Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                                m_IShome.ProdSEL1.txtProduct.Text = ""
                                m_IShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                Exit Sub
                            End If
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                            m_IShome.ProdSEL1.txtProduct.Text = ""
                            m_IShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
                        End If
                    Else
                        If (objAppContainer.objHelper.ValidateEAN(strBarcode)) Then
                            strBarcode = strBarcode.PadLeft(13, "0")
                            strBarcode = strBarcode.Remove(strBarcode.Length - 1, 1)
                            If Not (objAppContainer.objDataEngine.GetItemSalesDetailsAllUsingPC(strBarcode, m_ItemSalesInfo)) Then
                                MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
                                m_IShome.ProdSEL1.txtProduct.Text = ""
                                m_IShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                                Exit Sub
                            End If
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M2"), "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
                            m_IShome.ProdSEL1.txtProduct.Text = ""
                            m_IShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Return
                        End If
                    End If
                Case BCType.SEL
                    If objAppContainer.objHelper.ValidateSEL(strBarcode) Then
                        Dim strBootsCode As String = ""
                        objAppContainer.objHelper.GetBootsCodeFromSEL(strBarcode, strBootsCode)
                        strBootsCode = objAppContainer.objHelper.GenerateBCwithCDV(strBootsCode)
                        If Not (objAppContainer.objDataEngine.GetItemSalesDetailsAllUsingBC(strBootsCode, m_ItemSalesInfo)) Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M14"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                            m_IShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                            Exit Sub
                        End If
                    Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M4"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                        m_IShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                        Exit Sub
                    End If
            End Select
            ISSessionManager.GetInstance().DisplayISScreen(ISSCREENS.ItemSalesDetails)
            m_IShome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            BCReader.GetInstance.StartRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at Item Sales HandleScanData:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        Finally
            If Not m_IShome Is Nothing Then
                If Not m_IShome.ProdSEL1.txtProduct.IsDisposed AndAlso Not m_IShome.ProdSEL1.txtSEL.IsDisposed Then
                    m_IShome.ProdSEL1.txtProduct.Text = ""
                    m_IShome.ProdSEL1.txtSEL.Text = ""
                End If
            End If
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Item Sales HandleScanData", Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Screen Display method for Item Sales. 
    ''' All Item Sales sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName">Enum ISSCREENS</param>
    ''' <remarks></remarks>
    Public Sub DisplayISScreen(ByVal ScreenName As ISSCREENS) Implements IISInterface.DisplayISScreen
        objAppContainer.objLogger.WriteAppLog("Enter Item Sales Display Screen", Logger.LogLevel.INFO)
        Try
            Select Case ScreenName
                Case ISSCREENS.Home
                    m_IShome.Invoke(New EventHandler(AddressOf DisplayHomeScreen))
                Case ISSCREENS.ItemSalesDetails
                    m_ISItemDetails.Invoke(New EventHandler(AddressOf DisplayItemSalesDetailsScreen))
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured Item Sales Display Screen: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Item Sales Display Screen", Logger.LogLevel.INFO)
        Return
    End Sub
    ''' <summary>
    ''' The method Displays the Item Sales Home Screen
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DisplayHomeScreen(ByVal o As Object, ByVal e As EventArgs) Implements ISMInterface.DisplayHomeScreen
        With m_IShome
            .ProdSEL1.txtProduct.Text = ""
            .ProdSEL1.txtSEL.Text = ""
            'Fix for Status bar doesnt display connected for RF Mode
            .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            .Visible = True
            .Refresh()
        End With
    End Sub
    Public Sub DisplayItemSalesDetailsScreen(ByVal o As Object, ByVal e As System.EventArgs) Implements IISInterface.DisplayItemSalesDetailsScreen
        objAppContainer.objLogger.WriteAppLog("Enter Item Sales DisplayISItemDetails", Logger.LogLevel.RELEASE)
        Try
            Dim objDescriptionArray As ArrayList = New ArrayList()
            objDescriptionArray = objAppContainer.objHelper.GetFormattedDescription(m_ItemSalesInfo.Description)
            Dim strBarcode As String = ""
            strBarcode = objAppContainer.objHelper.GeneratePCwithCDV(m_ItemSalesInfo.ProductCode)

            If m_ItemSalesInfo.TSF = "      " Then
                m_ItemSalesInfo.TSF = 0
            End If


            With m_ISItemDetails
                .lblBootsCode.Text = objAppContainer.objHelper.FormatBarcode(m_ItemSalesInfo.BootsCode)
                'System Testing Bug Fix for dispalying 13 digits with proper formatting.
                .lblProductCode.Text = objAppContainer.objHelper.FormatBarcode(strBarcode)
                .lblProdDescription1.Text = objDescriptionArray.Item(0)
                .lblProdDescription2.Text = objDescriptionArray.Item(1)
                .lblProdDescription3.Text = objDescriptionArray.Item(2)
                .lblStatusText.Text = objAppContainer.objHelper.GetStatusDescription(m_ItemSalesInfo.Status)
                .lblThisWeekUnits.Text = objAppContainer.objHelper.FormatSalesValue(m_ItemSalesInfo.ThisWeekUnits)
                '.lblThisWeekValue.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.CURRENCY_KEY) + " " + objAppContainer.objHelper.FormatSalesValue(m_ItemSalesInfo.ThisWeekValue)
                .lblTodayUnits.Text = objAppContainer.objHelper.FormatSalesValue(m_ItemSalesInfo.TodayUnits)
                '.lblTodayValue.Text = ConfigDataMgr.GetInstance.GetParam(ConfigKey.CURRENCY_KEY) + " " + objAppContainer.objHelper.FormatSalesValue(m_ItemSalesInfo.TodayValue)

                'Fix for displaying the value along with decimal
                .lblThisWeekValue.Text = objAppContainer.objHelper.GetCurrency() + " " + Helper.FormatMoney(m_ItemSalesInfo.ThisWeekValue)
                .lblTodayValue.Text = objAppContainer.objHelper.GetCurrency() + " " + Helper.FormatMoney(m_ItemSalesInfo.TodayValue)
                .lblStockFigureText.Text = Val(m_ItemSalesInfo.TSF)
                'Fix for Status bar doesnt display connected for RF Mode
                .objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                .Visible = True
                .Refresh()
            End With

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at Item Sales DispalyItemSalesDetails:" + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Item Sales DisplayItemSalesDetails", Logger.LogLevel.RELEASE)
    End Sub

#End Region
End Class

''' <summary>
''' The value class for getting and managing Item Sales Details.
''' </summary>
''' <remarks></remarks>
Public Class ItemSalesInfo
    Inherits ProductInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
#Region "Variables"
    Private m_TodayUnits As String
    Private m_ThisWeekUnits As String
    Private m_TodayValue As String
    Private m_ThisWeekValue As String
#End Region
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
#Region "Constructor"
    Sub New()
    End Sub
#End Region
#Region "Methods"
    ''' <summary>
    ''' Gets or sets the units of items sold today
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TodayUnits() As String
        Get
            Return m_TodayUnits
        End Get
        Set(ByVal value As String)
            m_TodayUnits = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the units of items sold this week
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ThisWeekUnits() As String
        Get
            Return m_ThisWeekUnits
        End Get
        Set(ByVal value As String)
            m_ThisWeekUnits = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the value of items sold today
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TodayValue() As String
        Get
            Return m_TodayValue
        End Get
        Set(ByVal value As String)
            m_TodayValue = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the value of items sold this week
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ThisWeekValue() As String
        Get
            Return m_ThisWeekValue

        End Get
        Set(ByVal value As String)
            m_ThisWeekValue = value
        End Set
    End Property
#End Region
End Class
#End If
