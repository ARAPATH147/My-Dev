'''***************************************************************
''' <FileName>Program.vb</FileName>
''' <summary>
''' The Main Program Entry Point.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''***************************************************************
Module mainModule
    Sub Main()
        'AppContainer start is called to start the program exectution
        Try
            AppContainer.GetInstance().Start()
        Catch ex As Exception
            Try
                AppContainer.GetInstance().obLogger.WriteAppLog(ex.Message, Logger.LogLevel.ERROR)
            Catch exInner As Exception
                MsgBox("Application exited : No LogFile generated")
            End Try
            'Leave unattended mode
            AppContainer.GetInstance().obLogger.WriteAppLog("Program:: Main::TimerEvent - Restarting the device", _
                                                            Logger.LogLevel.ERROR)
            'Restart the device in case of connection lost.
            Restart.GetInstance.ResetPocketPC()
        End Try
    End Sub
End Module