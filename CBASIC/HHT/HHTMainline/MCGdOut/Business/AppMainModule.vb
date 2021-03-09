'''***************************************************************
''' <FileName>AppMainModule.vb</FileName>
''' <summary>
''' This is the module that contains the Main () sub and intialises
''' the Parent container for the entire application.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''***************************************************************
Public Module AppMainModule
    'Declare object of AppContainer class
    Public objAppContainer As AppContainer
    ''' <summary>
    ''' The Applicaiton startup sub routine.
    ''' Initilises the application container and passes the control to the App Container.
    ''' </summary>
    ''' <remarks></remarks>
    Sub Main()
        Dim objInstanceCheck As InstanceChecker = Nothing
        Try
            objInstanceCheck = New InstanceChecker()
            'Check if this is the first application instance or 
            'application might exit due to error conditon or crash
            If objInstanceCheck.IsInstanceRunning() Then
                Application.Exit()
                Exit Sub
            End If
            'Instantiating the AppContainer Class and then initializing it
            objAppContainer = New AppContainer
            'If Not objAppContainer.IsOtherProgramRunning Then
            objAppContainer.Initialise()
            'End If
            'Before exitting application release mutex.
            objInstanceCheck.ClearMutex()
        Catch ex As Exception
            MessageBox.Show(" Application has errored: " & ex.Message, "Exception", _
            MessageBoxButtons.OK, _
            MessageBoxIcon.Hand, _
            MessageBoxDefaultButton.Button1)
        Finally
            objAppContainer = Nothing
            objInstanceCheck = Nothing
            Application.Exit()
        End Try
    End Sub
End Module


