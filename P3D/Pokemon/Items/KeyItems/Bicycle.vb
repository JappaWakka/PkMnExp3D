Namespace Items.KeyItems

    <Item(6, "Bicycle")>
    Public Class Bicycle

        Inherits KeyItem

        Public Overrides ReadOnly Property Description As String = Localization.GetString("item_desc_6")
		Public Overrides ReadOnly Property CanBeUsed As Boolean = True

		Public Sub New()
            _textureRectangle = New Rectangle(120, 0, 24, 24)
        End Sub
        Public Overrides Sub Use()
            If Screen.Level.Biking = True Then
                Screen.Level.Biking = False
                Screen.Level.OwnPlayer.SetTexture(Core.Player.TempBikeSkin, True)
                Core.Player.Skin = Core.Player.TempBikeSkin

				Screen.TextBox.Show("<player.name> stepped off the Bicycle.")
				While Core.CurrentScreen.Identification <> Screen.Identifications.OverworldScreen
					Core.CurrentScreen = Core.CurrentScreen.PreScreen
				End While

				If Screen.Level.IsRadioOn = False OrElse GameJolt.PhoneScreen.StationCanPlay(Screen.Level.SelectedRadioStation) = False Then
					MusicManager.Play(Screen.Level.MusicLoop)
				End If

			Else
				If Screen.Level.Surfing = False And Screen.Level.Biking = False And Screen.Level.Riding = False And Screen.Camera.IsMoving() = False And Screen.Camera.Turning = False And Screen.Level.CanBike() = True Then
					Dim BikeSkin As String
					If Core.Player.IsGameJoltSave = True Then
						If Core.Player.Gender = "Female" Then
							BikeSkin = "PlayerSkins\Generic_Bike_Female"
						Else
							BikeSkin = "PlayerSkins\Generic_Bike_Male"
						End If
					Else
						If File.Exists(GameController.GamePath & GameModeManager.ActiveGameMode.ContentPath & "Textures\OverworldSprites\PlayerSkins\" & Core.Player.Skin & "_Bike.png") = False Then
							If Core.Player.Gender = "Female" Then
								BikeSkin = "Generic_Bike_Female"
							Else
								BikeSkin = "Generic_Bike_Male"
							End If
						Else
							BikeSkin = Core.Player.Skin & "_Bike"
						End If
					End If
					Core.Player.TempBikeSkin = Core.Player.Skin
					Screen.Level.Biking = True
					Screen.Level.OwnPlayer.SetTexture(BikeSkin, False)
					Core.Player.Skin = BikeSkin
					While Core.CurrentScreen.Identification <> Screen.Identifications.OverworldScreen
						Core.CurrentScreen = Core.CurrentScreen.PreScreen
					End While
					If GameModeManager.ContentFileExists("Sounds\Bicycle") Then
						SoundManager.PlaySound("bicycle")
					End If
					If GameModeManager.ContentFileExists("Songs\" + Screen.Level.CurrentRegion + "_Bike") Then
						MusicManager.Play(Screen.Level.CurrentRegion + "_Bike", True)
					Else
						MusicManager.Play("Hoenn_Bike", True)
					End If
					PlayerStatistics.Track("Bicycle used", 1)
				Else
					Screen.TextBox.Show("Now is not the time~to use that.", {}, True, False)
                End If
            End If
        End Sub
    End Class

End Namespace
