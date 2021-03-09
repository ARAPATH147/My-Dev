''' <summary>
''' This is the module that contains the Main () sub and intialises the Parent container for the entire application.
''' </summary>
''' <remarks></remarks>
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
            objAppContainer.AppInitialise()
        Catch ex As Exception
            MessageBox.Show(" Application has errored: " & ex.Message, "Exception", _
                MessageBoxButtons.OK, _
                MessageBoxIcon.Exclamation, _
                MessageBoxDefaultButton.Button1)
        Finally
            Application.Exit()
        End Try
    End Sub
End Module
