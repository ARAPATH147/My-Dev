'''****************************************************************************
''' <FileName> AppMAin.vb </FileName> 
''' <summary> Main Initialisation Module</summary> 
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

Public Class AppMain

    'Variables used for network connection
    Public bConnect As Boolean = False

    ' Build information (RF or BATCH)
    Public cControllerBuildType As String = Nothing
    Public cControllerSoftwareLevel As String = Nothing
    Public cDeviceBuildType As String = Nothing

    Public Sub AppInitialise()

        cDeviceBuildType = ConfigFileManager.GetInstance.GetParam(ConfigKey.BUILD_TYPE)

        DeviceInformation.GetInstance.GetDeviceInfo()

    End Sub

End Class

