Namespace Items.Berries

    <Item(2020, "Pomeg")>
    Public Class PomegBerry

        Inherits Berry

        Public Sub New()
            MyBase.New(10800, "A Berry to be consumed by Pokémon. Using it on a Pokémon makes it more friendly but lowers its base HP.", "13.0cm", "Hard", 2, 3)

            Me.Spicy = 10
            Me.Dry = 0
            Me.Sweet = 10
            Me.Bitter = 10
            Me.Sour = 0

            Me.Type = Element.Types.Ice
            Me.Power = 90
        End Sub

        Public Overrides Sub Use()
            Dim selScreen = New PartyScreenV2(Core.CurrentScreen, Me, AddressOf Me.UseOnPokemon, "Use " & Me.Name, True) With {.Mode = Screens.UI.ISelectionScreen.ScreenMode.Selection, .CanExit = True}
            AddHandler selScreen.SelectedObject, AddressOf UseItemhandler

            Core.SetScreen(selScreen)
        End Sub

        Public Overrides Function UseOnPokemon(ByVal PokeIndex As Integer) As Boolean
            Dim p As Pokemon = Core.Player.Pokemons(PokeIndex)

            If p.EVHP > 0 Then
                Dim reduce As Integer = 10
                If p.EVHP < reduce Then
                    reduce = p.EVHP
                End If
                
                p.ChangeFriendShip(Pokemon.FriendShipCauses.EVBerry)
                p.EVHP -= reduce
                p.CalculateStats()

                If p.HP <= 0 Then
                    If p.Status = Pokemon.StatusProblems.Fainted Then
                        p.HP = 0
                    Else
                        p.HP = 1
                    End If
                End If

                Screen.TextBox.Show("Raised friendship of~" & p.GetDisplayName() & "." & RemoveItem(), {}, True, False)
                Return True
            Else
                Screen.TextBox.Show("Cannot raise the friendship~of " & p.GetDisplayName() & ".", {}, True, False)
                Return False
            End If
        End Function

    End Class

End Namespace
