'''****************************************************************************
''' <FileName> Initialise.vb </FileName> 
''' <summary> Main Initialisation Module - Program Entry Point/Startup object</summary> 
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
Module Initialise


    Public oAppMain As AppMain

    ''' <summary>
    ''' The Applicaiton startup sub routine.
    ''' Initilises the application container and passes the control to the App Container.
    ''' </summary>
    ''' <remarks></remarks>
    Sub Main()

        Dim oInstanceCheck As InstanceChecker = Nothing
        Try

            oInstanceCheck = New InstanceChecker()
            'Check if this is the first application instance or 
            'application might exit due to error conditon or crash

            If oInstanceCheck.fIsInstanceRunning() Then
                Windows.Forms.Application.Exit()
                Exit Sub
            End If

            If Not (IO.File.Exists(ConfigKey.CONFIG_FILE_PATH)) Then
                Exit Sub
            End If

            oAppMain = New AppMain()
            oAppMain.AppInitialise()
            oInstanceCheck.fClearMutex()
        Catch ex As Exception

        Finally
            oAppMain = Nothing
            oInstanceCheck = Nothing
            Windows.Forms.Application.Exit()
        End Try
    End Sub

End Module
