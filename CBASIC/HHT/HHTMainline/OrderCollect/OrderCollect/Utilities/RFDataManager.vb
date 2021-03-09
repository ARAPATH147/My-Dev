Imports System.Runtime.InteropServices

'''****************************************************************************
''' <FileName> RFDataManager.vb </FileName>
''' <summary>
''' Class to handle the Requests and Responses from the Server
''' </summary> 
''' <Version>1.0</Version> 
''' <Author>Andrew Paton</Author> 
''' <DateModified>11-05-2016</DateModified> 
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC55RF</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK </CopyRight>
'''****************************************************************************
'''* Modification Log 
'''**************************************************************************** 
'''  1.0    Andrew Paton                             11/05/2016        
'''         Inital Version.
''' 
'''**************************************************************************** 

Public Class RFDataManager

    Private Shared moRFDataManager As RFDataManager
    Private mcData As String

    ''' <summary>
    ''' The shared function GetInstance will implement a check for the instantiation
    ''' of class RFDataManager to make sure that the class has only one instance
    ''' </summary>
    Public Shared Function GetInstance() As RFDataManager
        If moRFDataManager Is Nothing Then
            moRFDataManager = New RFDataManager
        End If
        Return moRFDataManager
    End Function

    ''' <summary>
    ''' wait for a response from the Server
    ''' </summary>
    ''' <param name="strReceivedData">Message received from Server</param>
    ''' <returns>Boolean</returns>
    Public Function WaitForResponse(ByRef strReceivedData As String) As Boolean
        Dim bTemp As Boolean
        Dim iCount As Integer = 0
        strReceivedData = ""
        If ConnectionManager.GetInstance.CheckTimeout() Then
            ' bTemp = Receive(strReceivedData)
            bTemp = ConnectionManager.GetInstance.Receive(strReceivedData)
        Else
            'objAppContainer.bTimeOut = True
        End If
        Return bTemp
    End Function

    ''' <summary>
    ''' Handles SOR Signon Request  and 
    '''         SNR Signon Response from the Server
    ''' </summary>
    ''' <param name="cUserId">3-digit User Id plus 3-digit password</param>
    ''' <returns>Boolean</returns>
    Public Function GetUserDetails(ByVal cUserId As String) As Boolean

        'Dim oSNRMessage As UserSession.SNRMessage
        Try
            oAppMain.bUserSession = True
            If CheckReconnect() Then
                If SendSOR(cUserId.Substring(0, 3), cUserId.Substring(3, 3)) Then
                    If WaitForResponse(mcData) Then
                        Select Case mcData.Substring(0, 3)
                            Case "SNR"
                                ProcessSNR(mcData, oAppMain.mSNRMessage)
                                oAppMain.mSNRMessage.cUserPassword = cUserId.Substring(3, 3)
                                Return True
                            Case "NAK"
                                'Display the recevied NAK message to the user.
                                Dim strNakMessage As String = ""
                                strNakMessage = mcData.Replace("NAK", "")    'Supress NAK String
                                strNakMessage = strNakMessage.Replace("NAKERROR", "")   'Suppress NAKERROR string
                                MessageBox.Show(strNakMessage, _
                                                "Error", MessageBoxButtons.OK, _
                                                MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                                ' if user login failed
                                Return False
                        End Select
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    ''' <summary>
    ''' Handles OFF Signoff Request
    ''' </summary>
    ''' <returns>Boolean</returns>
    Public Function LogOff() As Boolean
        Try
            If SendOFF(oAppMain.cCurrentUserID) Then
                If WaitForResponse(mcData) Then
                    Select Case mcData.Substring(0, 3)
                        Case "ACK"
                            Return True
                        Case "NAK"
                            Return False
                    End Select
                Else

                    If Not CheckReconnect() Then
                        '    'if retry selected on timeout 
                        If oAppMain.bTimeOut And oAppMain.bRetryAtTimeout Then
                            oAppMain.bTimeOut = False
                            Do Until Not oAppMain.bRetryAtTimeout
                                CheckReconnect(True)
                            Loop
                        End If
                        Return False
                        Else
                            Return False
                        End If
                    End If
            Else
                    If Not CheckReconnect() Then
                        Return False
                    Else
                        Return False
                    End If
            End If
        Catch ex As Exception
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Function to check the connection to the Controller/server and reconnect if needed 
    ''' </summary>
    ''' <returns>Boolean</returns>
    Public Function CheckReconnect(Optional ByVal bCheckReconnect As Boolean = False) As Boolean
        Dim strRecieve As String = Nothing '"NAK"
        Dim bTemp As Boolean = True

        Try
            If Not ConnectionManager.GetInstance.Connected() Or bCheckReconnect Then
                If ConnectionManager.GetInstance.ModuleReconnect() Then
                    If oAppMain.bUserSession Then
                        oAppMain.bReconnectSuccess = True
                        Return True
                    Else
                        oAppMain.bUserSession = False
                        If SendRCN() Then
                            If WaitForResponse(strRecieve) Then
                                'TIMEOUT
                                oAppMain.bRetryAtTimeout = False
                                oAppMain.bTimeOut = False
                                If strRecieve.Substring(0, 3) = "ACK" Then
                                    If oAppMain.iConnectedToAlternate <> 1 Then
                                        MessageBox.Show(strRecieve.Substring(3, strRecieve.Length - 3), "Reconnect Successful", _
                                                               MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                                               MessageBoxDefaultButton.Button1)
                                    End If
                                    oAppMain.bReconnectSuccess = True
                                    oAppMain.bCommFailure = False
                                    Return True
                                    ''checking if any details of item/cartons are saved during connection loss
                                ElseIf strRecieve.Substring(0, 3) = "NAK" Then
                                    Dim strMessage As String = strRecieve.Substring(3, strRecieve.Length - 3)
                                    If strMessage <> "" Then
                                        MessageBox.Show(strMessage, "Error while Connecting", _
                                                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                                    MessageBoxDefaultButton.Button1)
                                    Else
                                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M64"), "Error while Connecting", _
                                                  MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
                                                  MessageBoxDefaultButton.Button1)
                                    End If
                                    oAppMain.bCommFailure = True
                                    oAppMain.bReconnectSuccess = False
                                    Return False
                                End If

                            Else
                                If oAppMain.bTimeOut Then
                                    ConnectionManager.GetInstance.HandleTimeOut()
                                End If
                                Return False
                            End If
                        Else
                            CheckReconnect()
                        End If

                    End If
                Else
                    oAppMain.bConnect = False
                End If
            Else
                If oAppMain.bTimeOut Then
                    ConnectionManager.GetInstance.HandleTimeOut()
                End If
                Return True
            End If
        Catch ex As Exception

        End Try
    End Function


    ''' <summary>
    ''' (SNR) Process response for sign on request.
    '''       Extract fields from message 
    ''' </summary>
    ''' <param name="objSNRMessage">Signon Response Message string</param>
    ''' <param name="strSNRResponse">Signon Response Structure</param>
    ''' <returns>Boolean</returns>
    Public Function ProcessSNR(ByVal strSNRResponse As String, ByRef objSNRMessage As AppMain.SNRMessage) As Boolean
        Dim strSystemTime As String
        'oAppMain.bUserSession = False
        Try
            With objSNRMessage
                .cUserID = strSNRResponse.Substring(3, 3)
                .cUserName = strSNRResponse.Substring(7, 15)
                strSystemTime = strSNRResponse.Substring(22, 12)
                .cAuthorityFlag = Convert.ToChar(strSNRResponse.Substring(6, 1))
                'SetDeviceDateTime(strSystemTime)
            End With
        Catch ex As Exception

        End Try

    End Function

    ''' <summary>
    ''' (SOR) Send sign on request to the controller.
    ''' </summary>
    ''' <param name="UserName">User Name</param>
    ''' <param name="Password">Password</param>
    ''' <returns>Boolean</returns>
    Public Function SendSOR(ByVal UserName As String, ByVal Password As String) As Boolean
        Try
            Dim sSend As String = "SOR" & UserName & Password & _
                                 ConfigFileManager.GetInstance().GetParam(ConfigKey.APP_ID)
            ConnectionManager.GetInstance.Send(sSend)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ' <summary>
    ' (RCN) Reconnect request to the controller
    ' </summary>
    ' <return>boolean</return>
    '<remarks></remarks>	
    Public Function SendRCN() As Boolean
        Try
            Dim lFreeMem As Long = Nothing
            Dim bTransactionSent As Boolean = False
            Dim strMacID As String = DeviceFunc.GetInstance.GetMacAddress()
            'Read app version from config file
            'To split the appverion and release version. Actual app version should be send in SOR
            Dim strAppVersion As String = Nothing
            Dim aReleaseVersion() As String = Nothing
            Dim strDeviceType As String = ConfigFileManager.GetInstance.GetParam(ConfigKey.DEVICE_TYPE).Substring(0, 1)
            strAppVersion = ConfigFileManager.GetInstance.GetParam(ConfigKey.APP_VERSION).ToString()
            aReleaseVersion = strAppVersion.Split("-")
            strAppVersion = aReleaseVersion(1)
            Dim sSend As String = "RCN" + _
                                   oAppMain.mSNRMessage.cUserID + _
                                   oAppMain.mSNRMessage.cUserPassword
            sSend += ConfigFileManager.GetInstance().GetParam(ConfigKey.APP_ID) & strAppVersion.PadLeft(4, "0")
            If strMacID = "" Then
                sSend += "000000000000"
            Else
                sSend += strMacID.PadLeft(12, "0")
            End If
            sSend += strDeviceType & DeviceFunc.GetInstance.GetIPAddress()
            sSend += DeviceFunc.GetInstance.CheckForFreeMemory("Program Files", lFreeMem).PadLeft(8, "0")
            sSend += "000"
            '  Dim bytes() As Byte = ASCII.GetBytes(sSend)
            ConnectionManager.GetInstance.Send(sSend)
            bTransactionSent = True

            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    ''' <summary>
    ''' (OFF) Send sign off request to the controller.
    ''' </summary>
    ''' <returns>Boolean</returns>
    Public Function SendOFF(ByVal UserName As String) As Boolean
        ' Sign Off (OFF)
        'Dim bTransactionSent As Boolean = False
        Try
            Dim sSend As String = "OFF" & UserName

            ConnectionManager.GetInstance.Send(sSend)
            Return True
        Catch ex As Exception
            'If Not _client.IsConnected() Then
            '    _client.EstablishConnection()
            'End If
            Return False
        End Try

    End Function

    ''' <summary>
    ''' (OCQ) Order and Collect Query. 
    ''' </summary>
    ''' <param name="cParcelNumber">Parcel Number</param>
    ''' <param name="iFlag">Type of query</param>
    ''' <returns>Boolean: True if Query was sent and received or else False </returns>
    Public Function SendOCQ(ByVal cParcelNumber As String, ByVal iFlag As Integer) As Boolean
        Try
            'Dim cMessage As String = ("OCQ" & cParcelNumber & iFlag).PadLeft(16, "0")
            Dim cMessage As String = "OCQ" & cParcelNumber & iFlag
            If ConnectionManager.GetInstance.Send(cMessage) = True Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' (OCU) Order and Collect Update. 
    ''' </summary>
    ''' <param name="cUpdate">Update string</param>
    ''' <param name="iFlag">Type of update</param>
    ''' <returns>Boolean: True if update sent or else False </returns>
    Public Function SendUpdate(ByVal cUpdate As String, ByVal iFlag As Integer) As Boolean
        Try
            Dim cMessage As String = "OCU" & cUpdate & iFlag
            ConnectionManager.GetInstance.Send(cMessage)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

End Class
