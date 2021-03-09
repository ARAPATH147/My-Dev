Module appMain
    Sub Main()
        Try
            'Using objMainForm As New WLANSetup
            'objMainForm.ShowDialog()
            Using objRadiusUtils As RadioUtils = New RadioUtils
                ' objRadiusUtils.CreateRegistryFile("BUK-Stores", Macro.STORE_CONTROLLER)
                objRadiusUtils.CreateRegistryFile("BUK-Stores")
            End Using

            Using objHotButtonUtil As HotKeyBtnUtil = New HotKeyBtnUtil
                objHotButtonUtil.disableActionButton()
            End Using

        Catch ex As Exception
            MessageBox.Show("Message:" + ex.Message, "Error")
        End Try

        Application.Exit()
    End Sub
End Module
