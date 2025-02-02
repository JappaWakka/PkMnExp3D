﻿Public Class NewGameScreen

    Inherits Screen

	Dim skinFiles() As String = {GameModeManager.ActiveGameMode.SkinFiles.ToString}
	Dim skinNames() As String = {GameModeManager.ActiveGameMode.SkinNames.ToString}
	Dim skinGenders() As String = {GameModeManager.ActiveGameMode.SkinGenders.ToString}
	Dim backColors() As Color = {New Color(248, 176, 32), New Color(248, 216, 88), New Color(56, 88, 200), New Color(216, 96, 112)}

    Public Index As Integer = 0
    Dim pokeIndex As Integer = 0

    Dim ProfAlpha As Integer = 0
    Dim OtherAlpha As Integer = 0

    Dim Name As String = ""
    Dim SkinIndex As Integer = 0
    Dim skinTexture As Texture2D
    Dim enterCorrectName As Boolean = False
    Dim nameMessage As String = "This name is too short."

    Dim mainTexture As Texture2D
    Dim pokeTexture As Texture2D

    Dim ballPosition As Vector2
    Dim ballIndex As Vector2 = New Vector2(0, 0)
    Dim ballAnimationDelay As Single = 0.2F
    Dim ballVelocity As Single = -4.0F
    Dim pokePosition As Vector2
    Dim pokeID As Integer = 0

    Dim CurrentText As String = ""

    Dim currentBackColor As Color = New Color(59, 123, 165)
    Dim normalColor As Color = New Color(59, 123, 165)

    Dim pokemonRange() As Integer = {1, 252}
    Dim introMusic As String = "welcome"
	Dim startMap As String = "Hoenn\Cities\Littleroot\InsideTruck.dat"
	Dim startPosition As Vector3 = New Vector3(9, 0.1F, 10)
	Dim startLocation As String = "Your Room"
	Dim startYaw As Single = MathHelper.PiOver2

	Dim Dialogues As New List(Of String)

	Public Sub New()
		For Each s As String In Core.GameOptions.ContentPackNames
			ContentPackManager.Load(GameController.GamePath & "\ContentPacks\" & s & "\exceptions.dat")
		Next

		BattleSystem.GameModeAttackLoader.Load()

		Localization.ReloadGameModeTokens()
		SmashRock.Load()
		Badge.Load()
		Pokedex.Load()
		BattleSystem.BattleScreen.ResetVars()
		LevelLoader.ClearTempStructures()
		PokemonForms.Initialize()

		Me.Identification = Identifications.NewGameScreen
		Me.CanChat = False
		mainTexture = TextureManager.GetTexture("GUI\Intro")

		LoadIntroValues()

		Dim p As Pokemon = Pokemon.GetPokemonByID(Core.Random.Next(pokemonRange(0), pokemonRange(1)))
		p.Generate(1, True)
		pokeID = p.Number
		pokeTexture = p.GetTexture(True)

		CurrentText = ""

		TextBox.Showing = False
		Me.Index = 0
		TextBox.reDelay = 0
		Dim skinTexture2D = TextureManager.GetTexture("Textures\OverworldSprites\PlayerSkins\" & skinFiles(SkinIndex))
		Dim skinRectangle As New Rectangle(0, CInt(skinTexture2D.Height / 4 * 2), CInt(skinTexture2D.Width / 3), CInt(skinTexture2D.Height / 4))
		skinTexture = TextureManager.GetTexture(skinTexture2D, skinRectangle)

		MusicManager.Play("nomusic")
	End Sub

	Private Sub LoadIntroValues()
		Dim GameMode As GameMode = GameModeManager.ActiveGameMode

		Me.skinNames = GameMode.SkinNames.ToArray()
		Me.skinFiles = GameMode.SkinFiles.ToArray()
		Me.skinGenders = GameMode.SkinGenders.ToArray()
		Me.backColors = GameMode.SkinColors.ToArray()

		Me.pokemonRange = GameMode.PokemonRange
		Me.introMusic = GameMode.IntroMusic
		Me.startMap = GameMode.StartMap
		Me.startPosition = GameMode.StartPosition
		Me.startLocation = GameMode.StartLocationName
		Me.startYaw = GameMode.StartRotation
		Me.normalColor = GameMode.StartColor
		Me.currentBackColor = GameMode.StartColor

		If GameMode.StartDialogue <> "" Then
			If GameMode.StartDialogue.CountSplits("|") >= 3 Then
				Dim Splits() As String = GameMode.StartDialogue.Split(CChar("|"))
				Me.Dialogues.AddRange(Splits)
			End If
		Else
			If Me.Dialogues.Count < 3 Then
				Me.Dialogues.Clear()
				Me.Dialogues.AddRange({Localization.GetString("new_game_intro_1"), Localization.GetString("new_game_intro_2"), Localization.GetString("new_game_intro_3")})
			End If
		End If
	End Sub

	Public Overrides Sub Update()
		If Index = 5 Then
			Core.GameInstance.IsMouseVisible = True
		Else
			Core.GameInstance.IsMouseVisible = False
		End If

		If ProfAlpha < 255 And Index = 0 Then
			ProfAlpha += 2
			If ProfAlpha >= 255 Then
				MusicManager.Play(Me.introMusic, True, 0.0F)
			End If
		ElseIf ProfAlpha >= 255 Or Index > 0 Then
			TextBox.Update()

			If TextBox.Showing = False Then
				Select Case Index
					Case 1
						UpdatePokemon()
					Case 3
						UpdateTransition(False)
					Case 5
						UpdateTextbox()
					Case 7
						UpdateTransition(True)
					Case 9
						If ProfAlpha > 0 Then
							ProfAlpha -= 2
						End If
						If ProfAlpha <= 0 Then
							CreateGame()
						End If
					Case 4
						UpdateChooseSkin()
					Case Else
						ShowText()
				End Select
			End If
		End If

		Dim AimColor As Color = normalColor

		If Me.Index = 4 Then
			AimColor = backColors(SkinIndex)
		End If

		Dim diffR As Byte = 5
		If (CInt(currentBackColor.R) - CInt(AimColor.R)).ToPositive() < 5 Then
			diffR = 1
		End If
		Dim diffG As Byte = 5
		If (CInt(currentBackColor.G) - CInt(AimColor.G)).ToPositive() < 5 Then
			diffG = 1
		End If
		Dim diffB As Byte = 5
		If (CInt(currentBackColor.B) - CInt(AimColor.B)).ToPositive() < 5 Then
			diffB = 1
		End If
		If currentBackColor.R < AimColor.R Then
			currentBackColor.R += diffR
		ElseIf currentBackColor.R > AimColor.R Then
			currentBackColor.R -= diffR
		End If
		If currentBackColor.G < AimColor.G Then
			currentBackColor.G += diffG
		ElseIf currentBackColor.G > AimColor.G Then
			currentBackColor.G -= diffG
		End If
		If currentBackColor.B < AimColor.B Then
			currentBackColor.B += diffB
		ElseIf currentBackColor.B > AimColor.B Then
			currentBackColor.B -= diffB
		End If
	End Sub

	Private Sub ShowText()
		Dim Text As String = ""

		Select Case Index
			Case 0
				Text = Dialogues(0)
			Case 2
				Text = Dialogues(1)
			Case 6
				Text = NameReaction(Name)
			Case 8
				Text = Name & Dialogues(2)
		End Select

		TextBox.reDelay = 0
		TextBox.Show(Text, {})

		Index += 1
	End Sub


	Public Overrides Sub Draw()
		Canvas.DrawRectangle(New Rectangle(0, 0, Core.windowSize.Width, Core.windowSize.Height), currentBackColor)

		TextBox.Draw()

		Core.SpriteBatch.Draw(mainTexture, New Rectangle(CInt(Core.windowSize.Width / 2) - 62, CInt(Core.windowSize.Height / 2) - 218, 130, 256), New Rectangle(0, 0, 62, 128), New Color(255, 255, 255, ProfAlpha))
		Core.SpriteBatch.Draw(skinTexture, New Rectangle(CInt(Core.windowSize.Width / 2) - 128, CInt(Core.windowSize.Height / 2) - 218, 256, 256), New Color(255, 255, 255, OtherAlpha))

		Select Case pokeIndex
			Case 1
				Core.SpriteBatch.Draw(mainTexture, New Rectangle(CInt(ballPosition.X), CInt(ballPosition.Y), 22, 22), New Rectangle(62 + CInt(ballIndex.X * 22), 48 + CInt(ballIndex.Y * 22), 22, 22), Color.White)
			Case 2
				Core.SpriteBatch.Draw(pokeTexture, New Rectangle(CInt(pokePosition.X) - 100, CInt(pokePosition.Y) - 218, 256, 256), Color.White)
			Case 3
				If Index < 6 Then
					Core.SpriteBatch.Draw(pokeTexture, New Rectangle(CInt(Core.windowSize.Width / 2) - 300, CInt(Core.windowSize.Height / 2) - 218, 256, 256), New Color(255, 255, 255, ProfAlpha))
				End If
		End Select

		Select Case Index
			Case 5
				Core.SpriteBatch.DrawString(FontManager.MainFontWhite, Localization.GetString("new_game_your_name") & ": ", New Vector2(TextboxPosition.X, TextboxPosition.Y - 32), Color.White)
				DrawTextBox()

				If enterCorrectName = True Then
					Core.SpriteBatch.DrawString(FontManager.MainFontWhite, Localization.GetString("new_game_name_too_short"), New Vector2(TextboxPosition.X, TextboxPosition.Y + 48), Color.DarkRed)
				End If
			Case 4
				Canvas.DrawRectangle(New Rectangle(CInt(TextboxPosition.X - 5), CInt(TextboxPosition.Y - 24), 138, 42), New Color(0, 0, 0, 80))

				Core.SpriteBatch.DrawString(FontManager.MainFontWhite, Localization.GetString("new_game_choose_skin") & ":" & Environment.NewLine & skinNames(SkinIndex) & Environment.NewLine & skinGenders(SkinIndex), New Vector2(TextboxPosition.X, TextboxPosition.Y - 24), Color.White)

				Canvas.DrawScrollBar(New Vector2(TextboxPosition.X, TextboxPosition.Y + 48), skinFiles.Count, 1, SkinIndex, New Size(128, 4), True, TextureManager.GetTexture(TextureManager.GetTexture("GUI\Menus\Menu"), New Rectangle(112, 12, 1, 1)), TextureManager.GetTexture(TextureManager.GetTexture("GUI\Menus\Menu"), New Rectangle(113, 12, 1, 1)))
		End Select
	End Sub

	Private Sub DrawTextBox()
		Canvas.DrawRectangle(New Rectangle(CInt(Core.windowSize.Width / 2) - 74, CInt(Core.windowSize.Height / 2) + 138, 148, 32), New Color(101, 142, 255))
		Canvas.DrawRectangle(New Rectangle(CInt(Core.windowSize.Width / 2) - 70, CInt(Core.windowSize.Height / 2) + 142, 140, 24), Color.White)

		Dim t As String = Me.CurrentText
		If t.Length < 14 Then
			t &= "_"
		End If
		Core.SpriteBatch.DrawString(FontManager.MainFontBlack, t, TextboxPosition(), Color.White)

		Dim l As New Dictionary(Of Buttons, String)
		l.Add(Buttons.A, Localization.GetString("controls_accept", "Accept"))
		l.Add(Buttons.X, "Edit name")
		Me.DrawGamePadControls(l)
	End Sub

	Private Function TextboxPosition() As Vector2
		Return New Vector2(CInt(Core.windowSize.Width / 2) - 70, CInt(Core.windowSize.Height / 2) + 138)
	End Function

	Private Sub UpdatePokemon()
		Select Case pokeIndex
			Case 0
				ballPosition = New Vector2(CInt(Core.windowSize.Width / 2) - 40, CInt(Core.windowSize.Height / 2) - 110)
				pokePosition = New Vector2(CInt(Core.windowSize.Width / 2) - 200, CInt(Core.windowSize.Height / 2) - 110)
				pokeIndex = 1
				AnimateBall()
			Case 1
				If ballPosition.X > CInt(Core.windowSize.Width / 2) - 200 Then
					ballPosition.X -= 3
					ballPosition.Y += ballVelocity
					ballVelocity += 0.2F
				Else
					pokeIndex = 2
				End If
				AnimateBall()
			Case 2
				If pokePosition.Y < CInt(Core.windowSize.Height / 2) Then
					pokePosition.Y += 5
				Else
					Dim p As Pokemon = Pokemon.GetPokemonByID(pokeID)
					p.PlayCry()
					pokeIndex = 3
					Index = 2
				End If
		End Select
	End Sub

	Private Sub AnimateBall()
		If ballAnimationDelay <= 0.0F Then
			ballIndex.X += 1
			If ballIndex.X = 2 And ballIndex.Y = 2 Then
				ballIndex = New Vector2(0, 0)
			End If
			If ballIndex.X = 2 Then
				ballIndex.X = 0
				ballIndex.Y += 1
			End If
			ballAnimationDelay = 0.2F
		Else
			ballAnimationDelay -= 0.1F
		End If
	End Sub

	Private Sub UpdateTransition(ByVal ToProf As Boolean)
		If ToProf = True Then
			If OtherAlpha > 0 Then
				OtherAlpha -= 5
			End If
			If OtherAlpha <= 0 Then
				If ProfAlpha < 255 Then
					ProfAlpha += 5

					If ProfAlpha >= 255 Then
						Index += 1
					End If
				End If
			End If
		Else
			If ProfAlpha > 0 Then
				ProfAlpha -= 5
			End If
			If ProfAlpha <= 0 Then
				If OtherAlpha < 255 Then
					OtherAlpha += 5

					If OtherAlpha >= 255 Then
						Index += 1
					End If
				End If
			End If
		End If
	End Sub

	Private Sub UpdateChooseSkin()
		Dim sIndex As Integer = SkinIndex
		If Controls.Right(True, True, True, True) = True Then
			SkinIndex += 1
		End If
		If Controls.Left(True, True, True, True) = True Then
			SkinIndex -= 1
		End If

		SkinIndex = CInt(MathHelper.Clamp(SkinIndex, 0, skinFiles.Count - 1))
		Dim skinTexture2D = TextureManager.GetTexture("Textures\OverworldSprites\PlayerSkins\" & skinFiles(SkinIndex))
		Dim skinRectangle As New Rectangle(0, CInt(skinTexture2D.Height / 4 * 2), CInt(skinTexture2D.Width / 3), CInt(skinTexture2D.Height / 4))
		If sIndex <> SkinIndex Then
			skinTexture = TextureManager.GetTexture(skinTexture2D, skinRectangle)
		End If

		If Controls.Accept() = True Then
			Index += 1
		End If
	End Sub

	Private Sub UpdateTextbox()
		CanMuteAudio = False

		If ControllerHandler.ButtonPressed(Buttons.X) = True Then
			Core.SetScreen(New InputScreen(Core.CurrentScreen, "Player", InputScreen.InputModes.Name, Me.CurrentText, 14, {TextureManager.GetTexture(TextureManager.GetTexture("Textures\OverworldSprites\PlayerSkins\" & skinFiles(SkinIndex)), New Rectangle(0, 64, 32, 32))}.ToList(), AddressOf Me.ConfirmInput))
		Else
			KeyBindings.GetNameInput(Me.CurrentText, 14)

			Me.CurrentText = CorrectChars(Me.CurrentText)

			If Controls.Accept(False, True, True) = True And KeyBoardHandler.KeyPressed(Keys.Space) = False Then
				If CurrentText.Length > 2 And String.IsNullOrWhiteSpace(CurrentText) = False Then
					Me.Name = CurrentText
					Index += 1
					CanMuteAudio = True
				Else
					enterCorrectName = True
				End If
			End If
		End If
	End Sub

	Private Sub ConfirmInput(ByVal input As String)
		Me.CurrentText = CorrectChars(input)
	End Sub

	Private Function CorrectChars(ByVal t As String) As String
		Dim exclude() As String = {"\", "/", ":", "*", "?", """", "<", ">", "|", ",", "."}
		Dim s As String = ""
		For Each c As Char In t
			If exclude.Contains(c.ToString()) = True Then
				c = CChar(" ")
			End If
			s &= c.ToString()
		Next
		Return s
	End Function

	Private Sub CreateGame()
		Dim folderPath As String = Name
		Dim folderPrefix As Integer = 0

		If folderPath.ToLower() = "autosave" Then
			folderPath = "autosave0"
		End If

		Dim savePath As String = GameController.GamePath & "\Save\"

		While System.IO.Directory.Exists(savePath & folderPath) = True
			If folderPath <> Name Then
				folderPath = folderPath.Remove(folderPath.Length - folderPrefix.ToString().Length, folderPrefix.ToString().Length)
			End If

			folderPath &= folderPrefix

			folderPrefix += 1
		End While

		If System.IO.Directory.Exists(GameController.GamePath & "\Save") = False Then
			System.IO.Directory.CreateDirectory(GameController.GamePath & "\Save")
		End If

		System.IO.Directory.CreateDirectory(savePath & folderPath)

		System.IO.File.WriteAllText(savePath & folderPath & "\Player.dat", GetPlayerData())
		System.IO.File.WriteAllText(savePath & folderPath & "\Pokedex.dat", "")
		System.IO.File.WriteAllText(savePath & folderPath & "\Items.dat", "")
		System.IO.File.WriteAllText(savePath & folderPath & "\Register.dat", "")
		System.IO.File.WriteAllText(savePath & folderPath & "\Berries.dat", GetBerryData())
		System.IO.File.WriteAllText(savePath & folderPath & "\Apricorns.dat", "")
		System.IO.File.WriteAllText(savePath & folderPath & "\Daycare.dat", "")
		System.IO.File.WriteAllText(savePath & folderPath & "\Party.dat", "")
		System.IO.File.WriteAllText(savePath & folderPath & "\ItemData.dat", "")
		System.IO.File.WriteAllText(savePath & folderPath & "\Options.dat", GetOptionsData())
		System.IO.File.WriteAllText(savePath & folderPath & "\Box.dat", "")
		System.IO.File.WriteAllText(savePath & folderPath & "\NPC.dat", "")
		System.IO.File.WriteAllText(savePath & folderPath & "\HallOfFame.dat", "")
		System.IO.File.WriteAllText(savePath & folderPath & "\SecretBase.dat", "")
		System.IO.File.WriteAllText(savePath & folderPath & "\RoamingPokemon.dat", "")
		System.IO.File.WriteAllText(savePath & folderPath & "\Statistics.dat", "")

		Core.Player.IsGameJoltSave = False
		Core.Player.LoadGame(folderPath)
		Core.SetScreen(New TransitionScreen(Me, New OverworldScreen(), Color.Black, False, 5))
	End Sub

	Private Function GetPlayerData() As String
		Dim ot As String = Core.Random.Next(0, 65256).ToString()
		While ot.Length < 5
			ot = "0" & ot
		End While

		Dim s As String = "Name|" & Name & Environment.NewLine &
			"Position|" & Me.startPosition.X.ToString().Replace(GameController.DecSeparator, ".") & "," & Me.startPosition.Y.ToString().Replace(GameController.DecSeparator, ".") & "," & Me.startPosition.Z.ToString().Replace(GameController.DecSeparator, ".") & Environment.NewLine &
			"MapFile|" & Me.startMap & Environment.NewLine &
			"Rotation|" & Me.startYaw.ToString() & Environment.NewLine &
			"RivalName|???" & Environment.NewLine &
			"Money|3000" & Environment.NewLine &
			"Badges|0" & Environment.NewLine &
			"Gender|Male" & Environment.NewLine &
			"PlayTime|0,0,0" & Environment.NewLine &
			"OT|" & ot & Environment.NewLine &
			"Points|0" & Environment.NewLine &
			"hasPokedex|0" & Environment.NewLine &
			"hasPhone|0" & Environment.NewLine &
			"freeCamera|1" & Environment.NewLine &
			"thirdPerson|0" & Environment.NewLine &
			"skin|" & skinFiles(SkinIndex) & Environment.NewLine &
			"location|" & Me.startLocation & Environment.NewLine &
			"battleAnimations|2" & Environment.NewLine &
			"BoxAmount|5" & Environment.NewLine &
			"LastRestPlace|Hoenn\Cities\Littleroot\InsideTruck.dat" & Environment.NewLine &
			"LastRestPlacePosition|3,0.1,3" & Environment.NewLine &
			"DiagonalMovement|0" & Environment.NewLine &
			"RepelSteps|0" & Environment.NewLine &
			"LastSavePlace|Hoenn\Cities\Littleroot\InsideTruck.dat" & Environment.NewLine &
			"LastSavePlacePosition|3,0.1,3" & Environment.NewLine &
			"Difficulty|" & GameModeManager.GetGameRuleValue("Difficulty", "0") & Environment.NewLine &
			"BattleStyle|0" & Environment.NewLine &
			"saveCreated|" & GameController.GAMEDEVELOPMENTSTAGE & " " & GameController.GAMEVERSION & Environment.NewLine &
			"LastPokemonPosition|999,999,999" & Environment.NewLine &
			"DaycareSteps|0" & Environment.NewLine &
			"GameMode|" & GameModeManager.ActiveGameMode.DirectoryName & Environment.NewLine &
			"PokeFiles|" & Environment.NewLine &
			"VisitedMaps|Hoenn\Cities\Littleroot\InsideTruck.dat" & Environment.NewLine &
			"TempSurfSkin|Brendan" & Environment.NewLine &
			"TempFishSkin|Brendan" & Environment.NewLine &
			"TempBikeSkin|Brendan" & Environment.NewLine &
			"Surfing|0" & Environment.NewLine &
			"ShowModels|1" & Environment.NewLine &
			"GTSStars|4" & Environment.NewLine &
			"SandBoxMode|0"

		Return s
    End Function

    Public Shared Function GetOptionsData() As String
        Dim s As String = "FOV|50" & Environment.NewLine &
            "TextSpeed|2" & Environment.NewLine &
            "MouseSpeed|12"

        Return s
    End Function

    Public Shared Function GetBerryData() As String
		Dim s As String = "{Johto\Routes\29.dat|13,0,5|6|2|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\29.dat|14,0,5|6|2|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\29.dat|15,0,5|6|2|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Cities\Azalea\Main.dat|9,0,3|0|2|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Cities\Azalea\Main.dat|9,0,4|1|1|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Cities\Azalea\Main.dat|9,0,5|0|2|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\30.dat|7,0,41|10|1|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\30.dat|14,0,5|2|1|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\30.dat|15,0,5|6|2|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\30.dat|16,0,5|2|1|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\35.dat|0,0,4|7|2|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\35.dat|1,0,4|8|2|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\36.dat|37,0,7|0|1|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\36.dat|38,0,7|4|1|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\36.dat|39,0,7|3|2|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\39.dat|8,0,2|9|1|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\39.dat|8,0,3|6|1|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\38.dat|13,0,12|16|2|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\38.dat|14,0,12|23|1|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\38.dat|15,0,12|16|2|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\43.dat|13,0,45|23|2|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\43.dat|13,0,46|24|2|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\Routes\43.dat|13,0,47|25|2|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\SafariZone\main.dat|3,0,11|5|2|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\SafariZone\main.dat|4,0,11|0|2|0|2012,9,21,4,0,0|1}" & Environment.NewLine &
			"{Johto\SafariZone\main.dat|5,0,11|6|3|0|2012,9,21,4,0,0|1}"

		Return s
    End Function

    Private Function NameReaction(ByVal name As String) As String
        Dim WeirdNames() As String = {"derp", "karp"}
        Dim KnownNames() As String = {"ash", "gary", "misty", "brock", "tracey", "may", "max", "dawn", "iris", "cilan", "red", "blue", "green", "gold", "silver"}
        'MAKE THIS EDITABLE IN LOCALIZATION: Dim OwnNames() As String = {"oak", "samuel", "prof. oak", "prof oak"}
        Dim OwnNames() As String = {"baro", "boar", "prof. baro", "prof baro", "tehbaro", "dr. baro", "dr baro"}

        Select Case True
            Case WeirdNames.Contains(name.ToLower())
                Return Localization.GetString("new_game_intro_weird_name_1") & name & Localization.GetString("new_game_intro_weird_name_2")
            Case KnownNames.Contains(name.ToLower())
                Return Localization.GetString("new_game_intro_known_name_1") & name & Localization.GetString("new_game_intro_known_name_2")
            Case OwnNames.Contains(name.ToLower())
                Return Localization.GetString("new_game_intro_same_name_1") & name & Localization.GetString("new_game_intro_same_name_2")
        End Select

        Return Localization.GetString("new_game_intro_name_1") & name & Localization.GetString("new_game_intro_name_2")
    End Function
End Class