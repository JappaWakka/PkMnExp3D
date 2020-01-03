Namespace Items.Medicine

    <Item(266, "Fanta")>
    Public Class Fanta

        Inherits MedicineItem

        Public Overrides ReadOnly Property PokeDollarPrice As Integer = 750
        Public Overrides ReadOnly Property Description As String = "Fizzzzzz. When consumed, it restores 250 HP to an injured Pokémon."
        Public Overrides ReadOnly Property IsHealingItem As Boolean = True

        Public Sub New()
            _textureRectangle = New Rectangle(48, 264, 24, 24)
        End Sub

        Public Overrides Sub Use()
            If CBool(GameModeManager.GetGameRuleValue("CanUseHealItem", "1")) = False Then
                Screen.TextBox.Show("Cannot use heal items.", {}, False, False)
                Exit Sub
            End If
            Dim selScreen = New PartyScreenV2(Core.CurrentScreen, Me, AddressOf Me.UseOnPokemon, "Use " & Me.Name, True) With {.Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection, .CanExit = True}
            AddHandler selScreen.SelectedObject, AddressOf UseItemhandler

            Core.SetScreen(selScreen)
        End Sub

        Public Overrides Function UseOnPokemon(ByVal PokeIndex As Integer) As Boolean
            Return HealPokemon(PokeIndex, 250)
        End Function

    End Class

End Namespace
