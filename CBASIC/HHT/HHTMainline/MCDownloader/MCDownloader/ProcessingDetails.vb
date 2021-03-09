Public Class ProcessingDetails
    Public ProcessingStatus As New List(Of FileDetails)()
End Class
Public Class FileDetails
    Public strFileName As String
    Public strBuildStatus As String
    Public dIndex As Double
    Public strException As String
    Public dtLastBuild As DateTime
    Public Sub New()
        dIndex = 0
        strException = "NA"
    End Sub
End Class