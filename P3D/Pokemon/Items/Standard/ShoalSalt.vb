Namespace Items.Standard

    <Item(677, "ShoalSalt")>
    Public Class ShoalSalt

        Inherits Item

        Public Overrides ReadOnly Property Description As String = "Salt obtained from deep inside the Shoal Cave."
        Public Overrides ReadOnly Property PokeDollarPrice As Integer = 20
        Public Overrides ReadOnly Property CanBeUsedInBattle As Boolean = False
        Public Overrides ReadOnly Property CanBeUsed As Boolean = False

        Public Sub New()
            _textureRectangle = New Rectangle(216, 408, 24, 24)
        End Sub

    End Class

End Namespace
