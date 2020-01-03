Namespace ScriptVersion2

    Partial Class ScriptComparer

        ' --------------------------------------------------------------------------------------------------------------------------
        ' Contains the <phone> constructs.
        ' --------------------------------------------------------------------------------------------------------------------------

        Private Shared Function DoPhone(ByVal subClass As String) As Object
            Dim command As String = GetSubClassArgumentPair(subClass).Command
            Dim argument As String = GetSubClassArgumentPair(subClass).Argument

            Select Case command.ToLower()
                Case "callflag"
                    Return GameJolt.PhoneScreen.Call_Flag
                Case "got"
                    Return ReturnBoolean(Core.Player.HasPhone)
            End Select

            Return DEFAULTNULL
        End Function

    End Class

End Namespace