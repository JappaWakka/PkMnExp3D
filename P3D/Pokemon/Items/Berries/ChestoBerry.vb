Namespace Items.Berries

    <Item(2001, "Chesto")>
    Public Class ChestoBerry

        Inherits Berry

        Public Sub New()
            MyBase.New(10800, "A Berry to be consumed by Pokémon. If a Pokémon holds one, it can recover from sleep on its own in battle.", "8.0cm", "Super Hard", 2, 3)

            Me.Spicy = 0
            Me.Dry = 10
            Me.Sweet = 0
            Me.Bitter = 0
            Me.Sour = 0

            Me.Type = Element.Types.Water
            Me.Power = 80
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
