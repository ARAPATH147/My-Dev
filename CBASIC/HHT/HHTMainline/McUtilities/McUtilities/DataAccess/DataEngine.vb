Imports System
Imports System.Data
'''****************************************************************************
''' <FileName>DataEngine.vb</FileName>
''' <summary>
''' Data Engine class being a part of the data access layer exposes 
''' APIs to business layer for accessing data from the location database.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''****************************************************************************
Public Class DataEngine
    Dim isRFDevice As Boolean = False
    Dim boolStatus As Boolean = False
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        GetDeviceType()
    End Sub
   
    ''' <summary>
    ''' Gets the device type read from the configuration file.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetDeviceType()
        'Read the device type from the cionfiguration file.
        Dim strDeviceType As String = Nothing
        strDeviceType = ConfigDataMgr.GetInstance().GetParam(ConfigKey.DEVICE_TYPE).ToString()

        If strDeviceType = Macros.MC70 Then
            isRFDevice = False
        ElseIf strDeviceType = Macros.RF Then
            isRFDevice = True
        End If

    End Sub
End Class
' Public Function GetUserDetails(ByVal strUserID As String, ByRef objUserInfo As UserInfo) As Boolean
' If isRFDevice Then
'        'Insert the code for RF based solution.
'    Else
'        'Dim objNRFDataSource As New UtilNRFDataSource()
'        Try
'            'Get the item details.
'            boolStatus = objNRFDataSource.GetUserDetails(strUserID, objUserInfo)
'            Return boolStatus
'        Catch ex As Exception
'            Return False
'        Finally
'            'Clear the variables
'            objNRFDataSource = Nothing
'        End Try
'    End If
'End Function
