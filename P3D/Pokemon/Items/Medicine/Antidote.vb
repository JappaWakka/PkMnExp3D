Namespace Items.Medicine

    <Item(9, "Antidote")>
    Public Class Antidote

        Inherits MedicineItem

        Public Overrides ReadOnly Property PokeDollarPrice As Integer = 100
        Public Overrides ReadOnly Property Description As String = "A spray-type medicine for poisoning. It can be used once to lift the effects of being poisoned from a Pokémon."

        Public Sub New()
            _textureRectangle = New Rectangle(168, 0, 24, 24)
        End Sub

        Public Overrides Sub Use()
            Dim selScreen = New PartyScreenV2(Core.CurrentScreen, Me, AddressOf Me.UseOnPokemon, "Use " & Me.Name, True) With {.Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection, .CanExit = True}
            AddHandler selScreen.SelectedObject, AddressOf UseItemhandler

            Core.SetScreen(selScreen)
        End Sub

        Public Overrides Function UseOnPokemon(ByVal PokeIndex As Integer) As Boolean
            Return CurePoison(PokeIndex)
        End Function

    End Class

End Namespace
