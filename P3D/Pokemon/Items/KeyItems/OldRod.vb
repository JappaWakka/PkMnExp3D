Namespace Items.KeyItems

    <Item(58, "Old Rod")>
    Public Class OldRod

        Inherits KeyItem

        Public Overrides ReadOnly Property Description As String = "An old and beat-up fishing rod. Use it by any body of water to fish for wild aquatic Pokémon."
        Public Overrides ReadOnly Property CanBeUsed As Boolean = True

        Public Sub New()
            _textureRectangle = New Rectangle(240, 48, 24, 24)
        End Sub

        Public Overrides Sub Use()

            If IsInfrontOfWater() = True And Screen.Level.Surfing = False And Screen.Level.Riding = False And Screen.Level.Biking = False Then
				Screen.Level.Fishing = True

				If Core.Player.IsGameJoltSave Then
					If GameJolt.Emblem.GetOnlineSprite(Core.GameJoltSave.GameJoltID, "_Fish") IsNot Nothing Then
						With Screen.Level.OwnPlayer
							Core.Player.TempFishSkin = .SkinName
							Dim SkinName_Fish As String = Core.Player.Skin & "_Fish"
							.SetTexture(SkinName_Fish, True)
						End With
					End If
				Else
					If File.Exists(GameController.GamePath & GameModeManager.ActiveGameMode.ContentPath & "Textures\OverworldSprites\PlayerSkins\" & Core.Player.Skin & "_Fish.png") = True Then
						With Screen.Level.OwnPlayer
							Core.Player.TempFishSkin = .SkinName
							Dim SkinName_Fish As String = Core.Player.Skin & "_Fish"
							.SetTexture(SkinName_Fish, False)
						End With
					End If
				End If

				Dim s As String = "version=2"

				While Core.CurrentScreen.Identification <> Screen.Identifications.OverworldScreen
					Core.CurrentScreen = Core.CurrentScreen.PreScreen
				End While

				Dim p As Pokemon = Nothing

				Dim pokeFile As String = "System\WildEncounters\" & Screen.Level.LevelFile.Remove(Screen.Level.LevelFile.Length - 4, 4) & ".poke"
				If GameModeManager.MapFileExists(pokeFile) = True Then
					p = Spawner.GetPokemon(Screen.Level.LevelFile, Spawner.EncounterMethods.OldRod, False)
				End If

				If p Is Nothing Then
					p = Pokemon.GetPokemonByID(129)
					p.Generate(10, True)
				End If

				Dim PokemonID As Integer = p.Number
				Dim PokemonShiny As String = "N"
				If p.IsShiny = True Then
					PokemonShiny = "S"
				End If

				If Core.Random.Next(0, 3) <> 0 Or Core.Player.Pokemons(0).Ability.Name.ToLower() = "suction cups" Or Core.Player.Pokemons(0).Ability.Name.ToLower() = "sticky hold" Then
					Dim LookingOffset As New Vector3(0)

					Select Case Screen.Camera.GetPlayerFacingDirection()
						Case 0
							LookingOffset.Z = -1
						Case 1
							LookingOffset.X = -1
						Case 2
							LookingOffset.Z = 1
						Case 3
							LookingOffset.X = 1
					End Select

					Dim spawnPosition As Vector3 = New Vector3(Screen.Camera.Position.X + LookingOffset.X, Screen.Camera.Position.Y, Screen.Camera.Position.Z + LookingOffset.Z)

					Dim endRotation As Integer = Screen.Camera.GetPlayerFacingDirection() + 2
					If endRotation > 3 Then
						endRotation = endRotation - 4
					End If

					s &= Environment.NewLine & "@player.showrod(0)" & Environment.NewLine &
						"@text.show(. . . . . . . . . .)" & Environment.NewLine &
						"@text.show(Oh!~A bite!)" & Environment.NewLine &
						"@player.hiderod" & Environment.NewLine &
						"@npc.spawn(" & spawnPosition.X.ToString().Replace(GameController.DecSeparator, ".") & "," & spawnPosition.Y.ToString().Replace(GameController.DecSeparator, ".") & "," & spawnPosition.Z.ToString().Replace(GameController.DecSeparator, ".") & ",0,...,[POKEMON|" & PokemonShiny & "]" & PokemonID & PokemonForms.GetOverworldAddition(p) & ",0," & endRotation & ",POKEMON,1337,Still)" & Environment.NewLine &
						"@Level.Update" & Environment.NewLine &
						"@pokemon.cry(" & PokemonID & ")" & Environment.NewLine &
						"@level.wait(50)" & Environment.NewLine &
						"@text.show(The wild " & p.OriginalName & "~attacked!)" & Environment.NewLine &
						"@npc.remove(1337)" & Environment.NewLine &
						"@battle.setvar(divebattle,true)" & Environment.NewLine &
						"@battle.wild(" & p.GetSaveData() & ")" & Environment.NewLine &
						":end"
				Else
					s &= Environment.NewLine & "@player.showrod(0)" & Environment.NewLine &
						"@text.show(. . . . . . . . . .)" & Environment.NewLine &
						"@text.show(No, there's nothing here...)" & Environment.NewLine &
						"@player.hiderod" & Environment.NewLine &
						":end"
				End If

				Screen.Level.Fishing = False
				If Core.Player.IsGameJoltSave Then
					If GameJolt.Emblem.GetOnlineSprite(Core.GameJoltSave.GameJoltID, "_Fish") IsNot Nothing Then
						With Screen.Level.OwnPlayer
							.SetTexture(Core.Player.TempFishSkin, True)
						End With
					End If
				Else
					If GameModeManager.ContentFileExists(Core.Player.Skin & "_Fish") = True Then
						With Screen.Level.OwnPlayer
							.SetTexture(Core.Player.TempFishSkin, False)
						End With
					End If
				End If
				CType(Core.CurrentScreen, OverworldScreen).ActionScript.StartScript(s, 2)
			Else
				Screen.TextBox.Show("Now is not the time~to use that.", {}, True, True)
			End If
		End Sub

        Public Shared Function IsInfrontOfWater() As Boolean
            Dim lookingAtEntities As New List(Of Entity)

            Dim LookingOffset As New Vector3(0)

            Select Case Screen.Camera.GetPlayerFacingDirection()
                Case 0
                    LookingOffset.Z = -1
                Case 1
                    LookingOffset.X = -1
                Case 2
                    LookingOffset.Z = 1
                Case 3
                    LookingOffset.X = 1
            End Select

            For Each e As Entity In Screen.Level.Entities
                If e.Position.X = Screen.Camera.Position.X + LookingOffset.X And CInt(e.Position.Y) = CInt(Screen.Camera.Position.Y) And e.Position.Z = Screen.Camera.Position.Z + LookingOffset.Z Then
                    lookingAtEntities.Add(e)
                End If
            Next

            If lookingAtEntities.Count > 0 Then
                For Each e As Entity In lookingAtEntities
                    If e.EntityID = "Water" And e.ActionValue = 0 Then
                        Return True
                    End If
                Next
            End If

            Return False
        End Function

    End Class

End Namespace
