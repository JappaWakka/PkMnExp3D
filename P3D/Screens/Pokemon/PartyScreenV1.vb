﻿Public Class PartyScreenV1

    Inherits Screen

    Dim index As Integer = 0
    Dim MainTexture As Texture2D
    Dim yOffset As Single = 0
    Dim MenuID As Integer = 0
    Dim mPressed As Boolean = False
    Dim switchIndex As Integer = -1
    Dim TextColor As SpriteFont

    Public Sub New(ByVal currentScreen As Screen, ByVal PokeIndex As Integer)
        Me.Identification = Identifications.PartyScreen
        Me.PreScreen = currentScreen
        MainTexture = TextureManager.GetTexture("GUI\Menus\Menu")

        Me.index = PokeIndex

        If Core.Player.Pokemons.Count = 6 Then
            Dim has100 As Boolean = True
            For i = 0 To 5
                If Core.Player.Pokemons(i).Level < 100 Then
                    has100 = False
                    Exit For
                End If
            Next
            If has100 = True Then
                GameJolt.Emblem.AchieveEmblem("overkill")
            End If
        End If

        Me.CheckForLegendaryEmblem()
    End Sub

    Public Overrides Sub Draw()
        Me.PreScreen.Draw()

        Dim BackgroundTexture As Texture2D = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(160, 224, 48, 48), "")
        Dim CanvasTexture As Texture2D = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(0, 0, 48, 48), "")

        Canvas.DrawImageBorder(BackgroundTexture, 2, New Rectangle(60, 100, 800, 480))
        Canvas.DrawImageBorder(CanvasTexture, 2, New Rectangle(60, 100, 480, 64))
        Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Localization.GetString("party_screen_title"), New Vector2(142, 132), Color.White)
        Core.SpriteBatch.Draw(MainTexture, New Rectangle(78, 124, 48, 48), New Rectangle(96, 16, 18, 18), Color.White)
        Core.SpriteBatch.DrawString(FontManager.MainFontWhite, Localization.GetString("party_screen_backadvice"), New Vector2(1200 - FontManager.MainFontBlack.MeasureString(Localization.GetString("party_screen_backadvice")).X - 320, 572), Color.White)

        For i = 0 To Core.Player.Pokemons.Count - 1
            DrawPokemonTile(i, Core.Player.Pokemons(i))
        Next
        If Core.Player.Pokemons.Count < 6 Then
            For i = Core.Player.Pokemons.Count To 5
                DrawEmptyTile(i)
            Next
        End If

        If ChooseBox.Showing = True Then
            Dim Position As New Vector2(0, 0)
            Select Case Me.index
                Case 0, 2, 4
                    Position = New Vector2(606, 566 - ChooseBox.Options.Count * 48)
                Case 1, 3, 5
                    Position = New Vector2(60, 566 - ChooseBox.Options.Count * 48)
            End Select
            ChooseBox.Draw(Position)
        End If

        TextBox.Draw()
    End Sub

    Public Overrides Sub Update()
        TextBox.reDelay = 0.0F
        yOffset += 0.45F

        If TextBox.Showing = True Then
            TextBox.Update()
        Else
            Dim mState As MouseState = Mouse.GetState()
            If mPressed = False Then
                If ChooseBox.Showing = False Then
                    NavigateMain()
                Else
                    ChooseBox.Update(False)
                End If

                If Controls.Accept() Then
                    mPressed = True
					AcceptKeyPressed()
					SoundManager.PlaySound("Select")
				End If
            Else
                If mState.LeftButton = ButtonState.Released And MouseHandler.ButtonUp(MouseHandler.MouseButtons.LeftButton) = True Then
                    mPressed = False
                End If
            End If

            If Controls.Dismiss() Then
                CancelKeyPressed()
            End If
        End If
    End Sub

    Private Sub AcceptKeyPressed()
        If ChooseBox.Showing = True Then
            Select Case MenuID
                Case 0
                    Select Case ChooseBox.Options(ChooseBox.index)
                        Case Localization.GetString("global_summary")
                            ChooseBox.Showing = False
                            Core.SetScreen(New PokemonStatusScreen(Me, index, {}, Core.Player.Pokemons(index), True))
                        Case Localization.GetString("global_switch")
                            switchIndex = index
                            ChooseBox.Showing = False
                        Case Localization.GetString("party_screen_item")
                            ChooseBox.Show({Localization.GetString("global_give"), Localization.GetString("global_take"), Localization.GetString("global_back")}, 0, {})
                            Me.MenuID = 1
                        Case Localization.GetString("global_back")
                            ChooseBox.Showing = False
                        Case Localization.GetString("global_pokemon_move_flash")
                            UseFlash()
                        Case Localization.GetString("global_pokemon_move_fly")
                            UseFly()
                        Case Localization.GetString("global_pokemon_move_ride")
                            UseRide()
                        Case Localization.GetString("global_pokemon_move_cut")
                            UseCut()
                        Case Localization.GetString("global_pokemon_move_dig")
                            UseDig()
                        Case Localization.GetString("global_pokemon_move_teleport")
                            UseTeleport()
                    End Select
                Case 1
                    Select Case ChooseBox.index
                        Case 0
                            Core.SetScreen(New InventoryScreen(Me, {}, AddressOf GiveItem))
                        Case 1
                            Me.TakeItem()
                        Case 2
                            ShowMenu()
                    End Select
            End Select
        Else
            If switchIndex = -1 Then
                ShowMenu()
            Else
                Dim p1 As Pokemon = Core.Player.Pokemons(switchIndex)
                Dim p2 As Pokemon = Core.Player.Pokemons(index)

                Core.Player.Pokemons(switchIndex) = p2
                Core.Player.Pokemons(index) = p1
                switchIndex = -1

                Screen.Level.OverworldPokemon.ForceTextureChange()
                Screen.Level.OverworldPokemon.Update()
                Screen.Level.OverworldPokemon.UpdateEntity()
                Screen.Level.OverworldPokemon.Render()
            End If
        End If
    End Sub

    Private Sub TakeItem()
        If Core.Player.Pokemons(index).IsEgg() = False Then
            If Core.Player.Pokemons(index).Item Is Nothing Then
                TextBox.Show(Core.Player.Pokemons(index).GetDisplayName() & Localization.GetString("party_screen_doesnt_hold_item"), {})
            Else
                If Core.Player.Pokemons(index).Item.AdditionalData <> "" Then
                    TextBox.Show(Localization.GetString("party_screen_mail_taken"), {}, False, False)

                    Dim i As Item = Core.Player.Pokemons(index).Item
                    Core.Player.Pokemons(index).Item = Nothing

                    Core.Player.Mails.Add(Items.MailItem.GetMailDataFromString(i.AdditionalData))

                    Me.MenuID = 0
                    ChooseBox.Showing = False
                Else
                    Dim i As Item = Core.Player.Pokemons(index).Item

                    Core.Player.Inventory.AddItem(i.ID, 1)
                    Core.Player.Pokemons(index).Item = Nothing

                    TextBox.TextColor = TextBox.PlayerColor
                    TextBox.Show("<playername> " & Localization.GetString("party_screen_take_item_1") & i.Name & Localization.GetString("party_screen_take_item_2") & Core.Player.Pokemons(index).GetDisplayName() & ".*" & Core.Player.Inventory.GetMessageReceive(i, 1))

                    Me.MenuID = 0
                    ChooseBox.Showing = False
                End If
            End If
        Else
            TextBox.Show(Localization.GetString("global_eggs_cannot_hold"))
        End If
    End Sub

    Private Sub GiveItem(ByVal ItemID As Integer)
        Dim Item As Item = Item.GetItemByID(ItemID)
        Dim Pokemon As Pokemon = Core.Player.Pokemons(index)

        If Pokemon.IsEgg() = False Then
            If Item.CanBeHold = True Then
                Core.Player.Inventory.RemoveItem(Item.ID, 1)

                Dim reItem As Item = Nothing
                If Not Pokemon.Item Is Nothing Then
                    reItem = Pokemon.Item
                    If reItem.AdditionalData = "" Then
                        Core.Player.Inventory.AddItem(reItem.ID, 1)
                    Else
                        Core.Player.Mails.Add(Items.MailItem.GetMailDataFromString(reItem.AdditionalData))
                    End If
                End If

                Pokemon.Item = Item

                TextBox.reDelay = 0.0F

                Dim t As String = Localization.GetString("party_screen_give_item_1") & Item.Name & Localization.GetString("party_screen_give_item_2") & Pokemon.GetDisplayName() & Localization.GetString("party_screen_give_item_3")
                If Not reItem Is Nothing Then
                    If reItem.AdditionalData = "" Then
                        t &= Localization.GetString("party_screen_give_item_4") & reItem.Name & Localization.GetString("party_screen_give_item_5")
                    Else
                        t &= "*" & Localization.GetString("party_screen_mail_taken")
                    End If
                Else
                    t &= "."
                End If

                TextBox.Show(t, {})
            Else
                TextBox.Show(Pokemon.GetDisplayName() & Localization.GetString("party_screen_cannot_hold") & Item.Name & ".")
            End If
        Else
            TextBox.Show(Localization.GetString("global_eggs_cannot_hold"))
        End If

        Me.MenuID = 0
        ChooseBox.Showing = False
    End Sub

    Private Sub CancelKeyPressed()
        If ChooseBox.Showing = True Then
            Select Case MenuID
                Case 0
                    ChooseBox.Showing = False
                Case 1
                    ShowMenu()
            End Select
        Else
            If switchIndex = -1 Then
                Core.SetScreen(Me.PreScreen)
            Else
                switchIndex = -1
            End If
        End If
    End Sub

    Private Sub ShowMenu()
        Me.MenuID = 0
        ChooseBox.Show({Localization.GetString("global_summary"), Localization.GetString("global_switch"), Localization.GetString("party_screen_item"), Localization.GetString("global_back")}, 0, {})

        If (PokemonHasMove(Core.Player.Pokemons(index), Localization.GetString("global_pokemon_move_cut")) = True And Badge.CanUseHMMove(Badge.HMMoves.Cut) = True And Core.Player.Pokemons(index).IsEgg() = False) OrElse GameController.IS_DEBUG_ACTIVE = True OrElse Core.Player.SandBoxMode Then
            Dim options As List(Of String) = ChooseBox.Options.ToList()
            options.Insert(1, Localization.GetString("global_pokemon_move_cut"))
            ChooseBox.Options = options.ToArray()
        End If
        If (PokemonHasMove(Core.Player.Pokemons(index), Localization.GetString("global_pokemon_move_flash")) = True And Badge.CanUseHMMove(Badge.HMMoves.Flash) = True And Core.Player.Pokemons(index).IsEgg() = False) OrElse GameController.IS_DEBUG_ACTIVE = True OrElse Core.Player.SandBoxMode Then
            Dim options As List(Of String) = ChooseBox.Options.ToList()
            options.Insert(1, Localization.GetString("global_pokemon_move_flash"))
            ChooseBox.Options = options.ToArray()
        End If
        If (PokemonHasMove(Core.Player.Pokemons(index), Localization.GetString("global_pokemon_move_ride")) = True And Core.Player.Pokemons(index).IsEgg() = False) OrElse GameController.IS_DEBUG_ACTIVE = True OrElse Core.Player.SandBoxMode Then
            Dim options As List(Of String) = ChooseBox.Options.ToList()
            options.Insert(1, Localization.GetString("global_pokemon_move_ride"))
            ChooseBox.Options = options.ToArray()
        End If
        If (PokemonHasMove(Core.Player.Pokemons(index), Localization.GetString("global_pokemon_move_rocksmash")) = True And Badge.CanUseHMMove(Badge.HMMoves.RockSmash) = True And Core.Player.Pokemons(index).IsEgg() = False) OrElse GameController.IS_DEBUG_ACTIVE = True OrElse Core.Player.SandBoxMode Then
            Dim options As List(Of String) = ChooseBox.Options.ToList()
            options.Insert(1, Localization.GetString("global_pokemon_move_rocksmash"))
            ChooseBox.Options = options.ToArray()
        End If
        If (PokemonHasMove(Core.Player.Pokemons(index), Localization.GetString("global_pokemon_move_dig")) = True And Core.Player.Pokemons(index).IsEgg() = False) OrElse GameController.IS_DEBUG_ACTIVE = True OrElse Core.Player.SandBoxMode Then
            Dim options As List(Of String) = ChooseBox.Options.ToList()
            options.Insert(1, Localization.GetString("global_pokemon_move_dig"))
            ChooseBox.Options = options.ToArray()
        End If
        If (PokemonHasMove(Core.Player.Pokemons(index), Localization.GetString("global_pokemon_move_teleport")) = True And Core.Player.Pokemons(index).IsEgg() = False) OrElse GameController.IS_DEBUG_ACTIVE = True OrElse Core.Player.SandBoxMode Then
            Dim options As List(Of String) = ChooseBox.Options.ToList()
            options.Insert(1, Localization.GetString("global_pokemon_move_teleport"))
            ChooseBox.Options = options.ToArray()
        End If
        If (PokemonHasMove(Core.Player.Pokemons(index), Localization.GetString("global_pokemon_move_fly")) = True And Badge.CanUseHMMove(Badge.HMMoves.Fly) = True And Core.Player.Pokemons(index).IsEgg() = False) OrElse GameController.IS_DEBUG_ACTIVE = True OrElse Core.Player.SandBoxMode Then
            Dim options As List(Of String) = ChooseBox.Options.ToList()
            options.Insert(1, Localization.GetString("global_pokemon_move_fly"))
            ChooseBox.Options = options.ToArray()
        End If
    End Sub

    Private Sub NavigateMain()
        If Controls.Right(True, True, True) Then
            index += 1
        End If
        If Controls.Left(True, True, True) Then
            index -= 1
        End If
        If Controls.Down(True, True) Then
            index += 2
        End If
        If Controls.Up(True, True) Then
            index -= 2
        End If
        If KeyBoardHandler.KeyPressed(Keys.End) = True Then
            index = 5
        End If
        If KeyBoardHandler.KeyPressed(Keys.Home) = True Then
            index = 0
        End If

        If index < 0 Then
            index = 0
        ElseIf index > Core.Player.Pokemons.Count - 1 Then
            index = Core.Player.Pokemons.Count - 1
        End If

        Player.Temp.PokemonScreenIndex = Me.index
    End Sub

    Private Sub DrawEmptyTile(ByVal i As Integer)
        Dim BorderTexture As Texture2D
        BorderTexture = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(208, 224, 48, 48), "")

        Dim p As Vector2
        Select Case i
            Case 0, 2, 4
                p = New Vector2(32, 32 + (48 + 10) * i)
            Case Else
                p = New Vector2(416, 32 + (48 + 10) * (i - 1))
        End Select
        p.X += 80
        p.Y += 180

        With Core.SpriteBatch
            .Draw(BorderTexture, New Rectangle(CInt(p.X), CInt(p.Y), 32, 96), New Rectangle(0, 0, 16, 48), Color.White)
            For x = p.X + 32 To p.X + 288 Step 32
                .Draw(BorderTexture, New Rectangle(CInt(x), CInt(p.Y), 32, 96), New Rectangle(16, 0, 16, 48), Color.White)
            Next
            .Draw(BorderTexture, New Rectangle(CInt(p.X) + 320, CInt(p.Y), 32, 96), New Rectangle(32, 0, 16, 48), Color.White)

            .DrawString(FontManager.MainFontWhite, Localization.GetString("party_screen_EMPTY"), New Vector2(CInt(p.X + 72), CInt(p.Y + 18)), Color.White)
        End With
    End Sub

    Private Sub DrawPokemonTile(ByVal i As Integer, ByVal Pokemon As Pokemon)
        Dim BorderTexture As Texture2D
        If i = index Then
            TextColor = FontManager.MainFontWhite
            If Pokemon.Status = P3D.Pokemon.StatusProblems.Fainted Then
                BorderTexture = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(160, 176, 48, 48), "")
            Else
                BorderTexture = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(208, 176, 48, 48), "")
            End If
        Else
            TextColor = FontManager.MainFontWhite
            If switchIndex <> -1 And i = switchIndex Then
                BorderTexture = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(208, 176, 48, 48), "")
            Else
                If Pokemon.Status = P3D.Pokemon.StatusProblems.Fainted Then
                    BorderTexture = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(160, 128, 48, 48), "")
                Else
                    BorderTexture = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(208, 128, 48, 48), "")
                End If
            End If
        End If

        Dim p As Vector2
        Select Case i
            Case 0, 2, 4
                p = New Vector2(32, 32 + (48 + 10) * i)
            Case Else
                p = New Vector2(416, 32 + (48 + 10) * (i - 1))
        End Select
        p.X += 80
        p.Y += 180

        With Core.SpriteBatch
            .Draw(BorderTexture, New Rectangle(CInt(p.X), CInt(p.Y), 32, 96), New Rectangle(0, 0, 16, 48), Color.White)
            For x = p.X + 32 To p.X + 288 Step 32
                .Draw(BorderTexture, New Rectangle(CInt(x), CInt(p.Y), 32, 96), New Rectangle(16, 0, 16, 48), Color.White)
            Next
            .Draw(BorderTexture, New Rectangle(CInt(p.X) + 320, CInt(p.Y), 32, 96), New Rectangle(32, 0, 16, 48), Color.White)

            If Pokemon.IsEgg() = False Then
                Core.SpriteBatch.Draw(MainTexture, New Rectangle(CInt(p.X + 200), CInt(p.Y + 32), 134, 16), New Rectangle(0, 240, 67, 8), Color.White)
                Dim barX As Integer = CInt((Pokemon.HP / Pokemon.MaxHP.Clamp(1, Integer.MaxValue)) * 94)
                Dim barRectangle As Rectangle
                Dim barPercentage As Integer = CInt((Pokemon.HP / Pokemon.MaxHP.Clamp(1, Integer.MaxValue)) * 100)

                If barPercentage > 50 Then
                    barRectangle = New Rectangle(112, 0, 2, 3)
                ElseIf barPercentage <= 50 And barPercentage > 25 Then
                    barRectangle = New Rectangle(114, 0, 2, 3)
                ElseIf barPercentage <= 25 Then
                    barRectangle = New Rectangle(116, 0, 2, 3)
                End If
                If barPercentage > 0 Then
                    For x = 0 To barX Step 4
                        .Draw(MainTexture, New Rectangle(CInt(p.X + 200 + x + 32), CInt(p.Y + 38), 4, 6), barRectangle, Color.White)
                    Next
                End If
                .DrawString(TextColor, Localization.GetString("HP") & " " & Pokemon.HP & " / " & Pokemon.MaxHP, New Vector2(CInt(p.X + 200), CInt(p.Y + 48)), Color.White)
            End If

            Dim offset As Single = CSng(Math.Sin(yOffset))
            If i = index Then
                offset *= 3
            End If
            If Pokemon.Status = P3D.Pokemon.StatusProblems.Fainted Then
                offset = 0
            End If

            .Draw(Pokemon.GetMenuTexture(), New Rectangle(CInt(p.X + 5), CInt(p.Y + offset + 4), 64, 64), BattleStats.GetStatColor(Pokemon.Status))
            .DrawString(TextColor, Pokemon.GetDisplayName(), New Vector2(CInt(p.X + 72), CInt(p.Y + 18)), Color.White)

            If Pokemon.IsEgg() = False Then
                .Draw(MainTexture, New Rectangle(CInt(p.X + 72), CInt(p.Y + 46), 26, 12), New Rectangle(96, 10, 13, 6), Color.White)

                If Pokemon.Gender = P3D.Pokemon.Genders.Male Then
                    .Draw(MainTexture, New Rectangle(CInt(p.X + TextColor.MeasureString(Pokemon.GetDisplayName()).X + 80), CInt(p.Y + 20), 12, 20), New Rectangle(96, 0, 6, 10), Color.White)
                ElseIf Pokemon.Gender = P3D.Pokemon.Genders.Female Then
                    .Draw(MainTexture, New Rectangle(CInt(p.X + TextColor.MeasureString(Pokemon.GetDisplayName()).X + 80), CInt(p.Y + 20), 12, 20), New Rectangle(102, 0, 6, 10), Color.White)
                End If
            End If

            If Not Pokemon.Item Is Nothing And Pokemon.IsEgg() = False Then
                .Draw(Pokemon.Item.Texture, New Rectangle(CInt(p.X + 40), CInt(p.Y + 42), 24, 24), Color.White)
            End If

            Dim space As String = ""
            For x = 1 To 3 - Pokemon.Level.ToString().Length
                space &= " "
            Next

            If Pokemon.IsEgg() = False Then
                .DrawString(TextColor, Localization.GetString("Lv.") & space & Pokemon.Level, New Vector2(CInt(p.X + 72), CInt(p.Y + 48)), Color.White)
            End If

            Dim StatusTexture As Texture2D = BattleStats.GetStatImage(Pokemon.Status)
            If Not StatusTexture Is Nothing Then
                Core.SpriteBatch.Draw(StatusTexture, New Rectangle(CInt(p.X + 256), CInt(p.Y + 16), 40, 16), Color.White)
            End If
        End With

    End Sub

    Public Overrides Sub ChangeTo()
        Me.index = Player.Temp.PokemonScreenIndex
    End Sub

    Private Function PokemonHasMove(ByVal p As Pokemon, ByVal moveName As String) As Boolean
        If GameController.IS_DEBUG_ACTIVE = True OrElse Core.Player.SandBoxMode Then
            Return True
        Else
            For Each a As BattleSystem.Attack In p.Attacks
                If a.Name.ToLower() = moveName.ToLower() Then
                    Return True
                End If
            Next
        End If
        Return False
    End Function

    Private Sub UseFlash()
        ChooseBox.Showing = False
        Core.SetScreen(Me.PreScreen)
        If Core.CurrentScreen.Identification = Identifications.MenuScreen Then
            Core.SetScreen(Core.CurrentScreen.PreScreen)
        End If
        If Screen.Level.IsDark = True Then
            Dim s As String = "version=2" & Environment.NewLine &
                              "@text.show(" & Core.Player.Pokemons(index).GetDisplayName() & Localization.GetString("party_screen_move_used") & Localization.GetString("global_pokemon_move_flash") & "!)" & Environment.NewLine &
                              "@environment.toggledarkness" & Environment.NewLine &
                              "@sound.play(FieldMove_Flash)" & Environment.NewLine &
                              "@text.show(" & Localization.GetString("party_screen_flash_lit") & ")" & Environment.NewLine &
                              ":end"
            PlayerStatistics.Track("Flash used", 1)
            CType(Core.CurrentScreen, OverworldScreen).ActionScript.StartScript(s, 2)
        Else
            Dim s As String = "version=2" & Environment.NewLine &
                "@text.show(" & Core.Player.Pokemons(index).GetDisplayName() & Localization.GetString("party_screen_move_used") & Localization.GetString("global_pokemon_move_flash") & "!)" & Environment.NewLine &
                                            "@sound.play(FieldMove_Flash)" & Environment.NewLine &
                                            "@text.show(" & Localization.GetString("party_screen_flash_already_lit") & ")" & Environment.NewLine &
                                            ":end"
            CType(Core.CurrentScreen, OverworldScreen).ActionScript.StartScript(s, 2)
        End If
    End Sub

    Private Sub UseFly()
        If Level.CanFly = True Or GameController.IS_DEBUG_ACTIVE = True Or Core.Player.SandBoxMode = True Then
            ChooseBox.Showing = False
            Core.SetScreen(Me.PreScreen)
            If Core.CurrentScreen.Identification = Identifications.MenuScreen Then
                Core.SetScreen(Core.CurrentScreen.PreScreen)
            End If

            If Screen.Level.CurrentRegion.Contains(",") = True Then
                Dim regions As List(Of String) = Screen.Level.CurrentRegion.Split(CChar(",")).ToList()
                Core.SetScreen(New TransitionScreen(Core.CurrentScreen, New MapScreen(Core.CurrentScreen, regions, 0, {"Fly", Core.Player.Pokemons(index)}), Color.White, False))
            Else
                Dim startRegion As String = Screen.Level.CurrentRegion
                Core.SetScreen(New TransitionScreen(Core.CurrentScreen, New MapScreen(Core.CurrentScreen, startRegion, {"Fly", Core.Player.Pokemons(index)}), Color.White, False))
            End If
        Else
            TextBox.Show(Localization.GetString("party_screen_cannot_fly"), {}, True, False)
        End If
    End Sub

    Private Sub UseCut()
        Dim grassEntities = Grass.GetGrassTilesAroundPlayer(2.4F)
        If grassEntities.Count > 0 Then
            ChooseBox.Showing = False
            Core.SetScreen(Me.PreScreen)
            If Core.CurrentScreen.Identification = Identifications.MenuScreen Then
                Core.SetScreen(Core.CurrentScreen.PreScreen)
            End If

            PlayerStatistics.Track("Cut used", 1)
            TextBox.Show(Core.Player.Pokemons(index).GetDisplayName() & Localization.GetString("party_screen_move_used") & Localization.GetString("global_pokemon_move_cut") & "!", {}, True, False)
            Core.Player.Pokemons(index).PlayCry()
            For Each e As Entity In grassEntities
                Screen.Level.Entities.Remove(e)
            Next
        Else
            TextBox.Show(Localization.GetString("party_screen_cannot_cut"), {}, True, False)
        End If
    End Sub

    Private Sub UseRide()
        If Screen.Level.Riding = True Then
            Screen.Level.Riding = False
            Screen.Level.OwnPlayer.SetTexture(Core.Player.TempRideSkin, True)
            Core.Player.Skin = Core.Player.TempRideSkin

            ChooseBox.Showing = False
            Core.SetScreen(Me.PreScreen)
            If Core.CurrentScreen.Identification = Identifications.MenuScreen Then
                Core.SetScreen(Core.CurrentScreen.PreScreen)
            End If

            If Screen.Level.IsRadioOn = False OrElse GameJolt.PhoneScreen.StationCanPlay(Screen.Level.SelectedRadioStation) = False Then
                MusicManager.Play(Level.MusicLoop)
            End If
        Else
            If Screen.Level.Surfing = False And Screen.Camera.IsMoving() = False And Screen.Camera.Turning = False And Level.CanRide() = True Then
                ChooseBox.Showing = False
                Core.SetScreen(Me.PreScreen)
                If Core.CurrentScreen.Identification = Identifications.MenuScreen Then
                    Core.SetScreen(Core.CurrentScreen.PreScreen)
                End If

                Screen.Level.Riding = True
                Core.Player.TempRideSkin = Core.Player.Skin

                Dim skin As String = "[POKEMON|"
                If Core.Player.Pokemons(index).IsShiny = True Then
                    skin &= "S]"
                Else
                    skin &= "N]"
                End If
                skin &= Core.Player.Pokemons(index).Number & PokemonForms.GetOverworldAddition(Core.Player.Pokemons(index))

                Screen.Level.OwnPlayer.SetTexture(skin, False)

                SoundManager.PlayPokemonCry(Core.Player.Pokemons(index).Number)

                TextBox.Show(Core.Player.Pokemons(index).GetDisplayName() & Localization.GetString("party_screen_move_used") & Localization.GetString("global_pokemon_move_ride") & "!", {}, True, False)
                PlayerStatistics.Track("Ride used", 1)

                If Screen.Level.IsRadioOn = False OrElse GameJolt.PhoneScreen.StationCanPlay(Screen.Level.SelectedRadioStation) = False Then
                    If GameModeManager.ContentFileExists("Songs\" + Screen.Level.CurrentRegion + "_Bike") Then
                        MusicManager.Play(Screen.Level.CurrentRegion + "_Bike", True)
                    Else
                        MusicManager.Play("Hoenn_Bike", True)
                    End If
                End If
            Else
                TextBox.Show(Localization.GetString("party_screen_cannot_ride"), {}, True, False)
            End If
        End If
    End Sub

    Private Sub UseDig()
        If Screen.Level.CanDig = True Or GameController.IS_DEBUG_ACTIVE = True Or Core.Player.SandBoxMode = True Then
            ChooseBox.Showing = False
            Core.SetScreen(Me.PreScreen)
            If Core.CurrentScreen.Identification = Identifications.MenuScreen Then
                Core.SetScreen(Core.CurrentScreen.PreScreen)
            End If

            Dim setToFirstPerson As Boolean = Not CType(Screen.Camera, OverworldCamera).ThirdPerson

            Dim s As String = "version=2
@text.show(" & Core.Player.Pokemons(index).GetDisplayName() & Localization.GetString("party_screen_move_used") & Localization.GetString("global_pokemon_move_dig") & "!)
@level.wait(20)
@camera.activatethirdperson
@camera.reset
@camera.fix
@player.turnto(0)
@sound.play(destroy)
:while:<player.position(y)>>" & (Screen.Camera.Position.Y - 1.4).ToString().ReplaceDecSeparator() & "
@player.turn(1)
@player.warp(~,~-0.1,~)
@level.wait(1)
:endwhile
@screen.fadeout
@camera.defix
@player.warp(" & Core.Player.LastRestPlace & "," & Core.Player.LastRestPlacePosition & ",0)" & Environment.NewLine &
"@player.turnto(2)"

            If setToFirstPerson = True Then
                s &= Environment.NewLine & "@camera.deactivatethirdperson"
            End If
            s &= Environment.NewLine &
"@level.update
@screen.fadein
:end"

            PlayerStatistics.Track("Dig used", 1)
            CType(Core.CurrentScreen, OverworldScreen).ActionScript.StartScript(s, 2)
        Else
            TextBox.Show(Localization.GetString("party_screen_cannot_dig"), {}, True, False)
        End If
    End Sub

    Private Sub UseTeleport()
        If Screen.Level.CanTeleport = True Or GameController.IS_DEBUG_ACTIVE = True Or Core.Player.SandBoxMode = True Then
            ChooseBox.Showing = False
            Core.SetScreen(Me.PreScreen)
            If Core.CurrentScreen.Identification = Identifications.MenuScreen Then
                Core.SetScreen(Core.CurrentScreen.PreScreen)
            End If

            Dim setToFirstPerson As Boolean = Not CType(Screen.Camera, OverworldCamera).ThirdPerson

            Dim yFinish As String = (Screen.Camera.Position.Y + 2.9F).ToString().ReplaceDecSeparator()

            Dim s As String = "version=2
@text.show(" & Core.Player.Pokemons(index).GetDisplayName() & Localization.GetString("party_screen_move_used") & Localization.GetString("global_pokemon_move_teleport") & "!)
@level.wait(20)
@camera.activatethirdperson
@camera.reset
@camera.fix
@player.turnto(0)
@sound.play(teleport)
:while:<player.position(y)><" & yFinish & "
@player.turn(1)
@player.warp(~,~+0.1,~)
@level.wait(1)
:endwhile
@screen.fadeout
@camera.defix
@player.warp(" & Core.Player.LastRestPlace & "," & Core.Player.LastRestPlacePosition & ",0)
@player.turnto(2)"

            If setToFirstPerson = True Then
                s &= Environment.NewLine & "@camera.deactivatethirdperson"
            End If
            s &= Environment.NewLine &
"@level.update
@screen.fadein
:end"

            PlayerStatistics.Track("Teleport used", 1)
            CType(Core.CurrentScreen, OverworldScreen).ActionScript.StartScript(s, 2)
        Else
            TextBox.Show(Localization.GetString("party_screen_cannot_teleport"), {}, True, False)
        End If
    End Sub

    Private Sub CheckForLegendaryEmblem()
        ' This sub checks if Ho-Oh, Lugia and Suicune are in the player's party.
        Dim hasHoOh As Boolean = False
        Dim hasLugia As Boolean = False
        Dim hasSuicune As Boolean = False

        For Each p As Pokemon In Core.Player.Pokemons
            Select Case p.Number
                Case 245
                    hasSuicune = True
                Case 249
                    hasLugia = True
                Case 250
                    hasHoOh = True
            End Select
        Next

        If hasSuicune = True And hasLugia = True And hasHoOh = True Then
            GameJolt.Emblem.AchieveEmblem("legendary")
        End If
    End Sub

End Class