'''****************************************************************************
''' <FileName>QueryMacro.vb</FileName>
''' <summary>
''' This class is the data source class for the Shelf Management application.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''****************************************************************************
Public Class QueryMacro
    ''' <summary>
    ''' Query to Get all the available Count list.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const GET_USER_DETAILS = _
    "SELECT User_ID, Password, Supervisor_Flag " _
    & "FROM UserList " _
    & "WHERE (User_ID = {0})"

End Class

