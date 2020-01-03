Namespace Items.Medicine

    <Item(12, "Awakening")>
    Public Class Awakening

        Inherits MedicineItem

        Public Overrides ReadOnly Property PokeDollarPrice As Integer = 250
        Public Overrides ReadOnly Property Description As String = "A spray-type medicine used against sleep. It can be used once to rouse a Pokémon from the clutches of sleep."

        Public Sub New()
            _textureRectangle = New Rectangle(240, 0, 24, 24)
        End Sub

        Public Overrides Sub Use()
            Dim selScreen = New PartyScreenV2(Core.CurrentScreen, Me, AddressOf Me.UseOnPokemon, "Use " & Me.Name, True) With {.Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection, .CanExit = True}
            AddHandler selScreen.SelectedObject, AddressOf UseItemhandler

            Core.SetScreen(selScreen)
        End Sub

        Public Overrides Function UseOnPokemon(ByVal PokeIndex As Integer) As Boolean
            Return WakeUp(PokeIndex)
        End Function

    End Class

End Namespace
