'''******************************************************************************
''' <FileName>AppMainModule.vb</FileName>
''' <summary>
''' This is the module that contains the Main () sub and 
''' intialises the Parent container for the entire application.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''******************************************************************************

#Const DEVICE = True
Module AppMainModule

    Public objAppContainer As AppContainer
    ''' <summary>
    ''' The Applicaiton startup sub routine.
    ''' Initilises the application container and passes the control to the App Container.
    ''' </summary>
    ''' <remarks></remarks>
    Sub Main()
        Dim objInstanceCheck As InstanceChecker = Nothing
        Try
            'Perform all application pre requisites here.
            objInstanceCheck = New InstanceChecker()
            'Check if this is the first application instance or 
            'application might exit due to error conditon or crash
            If objInstanceCheck.IsInstanceRunning() Then
                Application.Exit()
                Exit Sub
            End If

            objAppContainer = New AppContainer()
            'If Not objAppContainer.IsOtherProgramRunning() Then
            objAppContainer.AppInitialise()
            'End If

            'Before exitting application release mutex.
            objInstanceCheck.ClearMutex()
        Catch ex As Exception
            'Fix for exception thrown when there is no config file in the device.
            'MessageBox.Show(" Application Error: Cannot Initialise " & ex.Message, "Exception", _
            '    MessageBoxButtons.OK, _
            '    MessageBoxIcon.Exclamation, _
            '    MessageBoxDefaultButton.Button1)
            'Checking for socket status
            If Not objAppContainer.objExportDataManager Is Nothing Then
                objAppContainer.objExportDataManager.EndSession()
                objAppContainer.objExportDataManager = Nothing
            End If
            objAppContainer.objLogger.WriteAppLog("Exception occured at AppMainModule: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        Finally
            objAppContainer = Nothing
            objInstanceCheck = Nothing
            Application.Exit()
#If RF Then
            DATAPOOL.getInstance.EndSession()
#End If

        End Try
    End Sub
End Module
