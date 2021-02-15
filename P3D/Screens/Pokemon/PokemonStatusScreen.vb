Public Class PokemonStatusScreen

    Inherits Screen
    Dim pageIndex As Integer = 0
    Dim PokeIndex As Integer = 0
    Dim BoxIndex As Integer = 0
    Dim BoxPokemon() As Pokemon
    Dim MainTexture As Texture2D
    Dim Pokemon As Pokemon
    Dim FrontView As Boolean = True
    Dim AttackIndex As Integer = 0
    Dim AttackToggle As Boolean = False
    Dim AttackPos As Single = 0
    Dim SwitchIndex As Integer = -1
    Dim dummyPokeIndex As Integer = -1

    Dim viewParty As Boolean = True

    Dim EVColors As List(Of Color) = {New Color(0, 210, 0), New Color(253, 83, 0), New Color(0, 154, 226), New Color(253, 183, 97), New Color(100, 201, 226), New Color(178, 108, 204)}.ToList()

    Public Sub New(ByVal currentScreen As Screen, ByVal Index As Integer, ByVal BoxPokemon() As Pokemon, ByVal Pokemon As Pokemon, ByVal viewParty As Boolean)
        Me.Identification = Identifications.PokemonStatusScreen
        Me.PreScreen = currentScreen
        Me.PokeIndex = Index
        Me.Pokemon = Pokemon
        Me.BoxPokemon = BoxPokemon
        Me.BoxIndex = Index
        Me.viewParty = viewParty

        Me.MainTexture = TextureManager.GetTexture("GUI\Menus\Menu")

        Me.pageIndex = Player.Temp.PokemonStatusPageIndex
    End Sub

    Public Overrides Sub Update()
        If AttackToggle = False Then
            If Me.AttackPos > 0.0F Then
                Me.AttackPos -= 15.0F
                If Me.AttackPos <= 0.0F Then
                    Me.AttackPos = 0.0F
                End If
            End If

            If Controls.Down(True, True, False) Then
                Me.PokeIndex += 1
                FrontView = True
                AttackIndex = 0
            End If
            If Controls.Up(True, True, False) Then
                Me.PokeIndex -= 1
                FrontView = True
                AttackIndex = 0
            End If

            If Me.viewParty = True Then
                If Me.PokeIndex < 0 Then
                    Me.PokeIndex = 0
                ElseIf Me.PokeIndex > Core.Player.Pokemons.Count - 1 Then
                    Me.PokeIndex = Core.Player.Pokemons.Count - 1
                End If

                Me.Pokemon = Core.Player.Pokemons(PokeIndex)
            Else
                If Me.PokeIndex < 0 Then
                    Me.PokeIndex = 0
                ElseIf Me.PokeIndex > BoxPokemon.Count - 1 Then
                    Me.PokeIndex = BoxPokemon.Count - 1
                End If

                Me.Pokemon = Me.BoxPokemon(PokeIndex)
            End If

            If Me.dummyPokeIndex <> Me.PokeIndex Then
                If Me.Pokemon.EggSteps = 0 Then
                    Me.Pokemon.PlayCry()
                    Me.dummyPokeIndex = Me.PokeIndex
                End If
            End If
        Else
            If Me.AttackPos < 320.0F Then
                Me.AttackPos += 15.0F
                If Me.AttackPos >= 320.0F Then
                    Me.AttackPos = 320.0F
                End If
            End If

            If Controls.Down(True, True, True) Then
                Me.AttackIndex += 1
            End If
            If Controls.Up(True, True, True) Then
                Me.AttackIndex -= 1
            End If

            If AttackIndex < 0 Then
                AttackIndex = 0
            ElseIf AttackIndex > Pokemon.Attacks.Count - 1 Then
                AttackIndex = Pokemon.Attacks.Count - 1
            End If
        End If

        If SwitchIndex = -1 Then
            If Controls.Right(True, True, True) Then
                pageIndex += 1
                AttackToggle = False
                AttackIndex = 0
            End If
            If Controls.Left(True, True, True) Then
                pageIndex -= 1
                AttackToggle = False
                AttackIndex = 0
            End If
            If pageIndex < 0 Then
                pageIndex = 0
            ElseIf pageIndex > 2 Then
                pageIndex = 2
            End If
        End If

        Player.Temp.PokemonStatusPageIndex = Me.pageIndex
        Player.Temp.PokemonScreenIndex = Me.PokeIndex

        If pageIndex = 0 Then
            If Controls.Accept() Then
                FrontView = Not FrontView
            End If
        ElseIf pageIndex = 2 Then
            If Controls.Accept() And Me.Pokemon.EggSteps = 0 Then
                If AttackToggle = False Then
                    AttackToggle = True
                Else
                    If SwitchIndex = -1 Then
                        SwitchIndex = AttackIndex
                    Else
                        Dim A1 As BattleSystem.Attack = Me.Pokemon.Attacks(SwitchIndex)
                        Dim A2 As BattleSystem.Attack = Me.Pokemon.Attacks(AttackIndex)

                        Me.Pokemon.Attacks(AttackIndex) = A1
                        Me.Pokemon.Attacks(SwitchIndex) = A2

                        SwitchIndex = -1
                    End If
                End If
            End If
        End If

        If Controls.Dismiss() Then
            If AttackToggle = True Then
                If SwitchIndex <> -1 Then
                    SwitchIndex = -1
                Else
                    AttackToggle = False
                End If
            Else
                Core.SetScreen(Me.PreScreen)
            End If
        End If
    End Sub

    Public Overrides Sub Draw()
        PreScreen.PreScreen.Draw()
        Dim PageTitleText As String = ""
        Dim TexturePositionPage As Vector2

        Select Case pageIndex
            Case 0
                Canvas.DrawImageBorder(TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(160, 272, 48, 48), ""), 2, New Rectangle(60, 100, 544, 480))
                If Me.Pokemon.EggSteps = 0 Then
                    DrawPage1()
                End If
                PageTitleText = Localization.GetString("poke_status_screen_stats_page")
                TexturePositionPage = New Vector2(0, 272)
            Case 1
                Canvas.DrawImageBorder(TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(208, 272, 48, 48), ""), 2, New Rectangle(60, 100, 544, 480))
                DrawPage2()
                PageTitleText = Localization.GetString("poke_status_screen_details_page")
                TexturePositionPage = New Vector2(0, 288)
            Case 2
                Canvas.DrawImageBorder(TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(256, 272, 48, 48), ""), 2, New Rectangle(60, 100, 544, 480))
                If Me.Pokemon.EggSteps = 0 Then
                    DrawPage3()
                End If
                PageTitleText = Localization.GetString("poke_status_screen_moves_page")
                If AttackToggle = False Then
                    TexturePositionPage = New Vector2(0, 304)
                Else
                    TexturePositionPage = New Vector2(0, 320)
                End If
        End Select
        DrawHeader()
        Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Localization.GetString("poke_status_screen_backadvice"), New Vector2(144 + 208, 100 + 48), Color.White)
        Core.SpriteBatch.Draw(MainTexture, New Rectangle(60 + 576 - 320 + 32, 100, 320, 32), New Rectangle(CInt(TexturePositionPage.X), CInt(TexturePositionPage.Y), 160, 16), Color.White)
        Core.SpriteBatch.DrawString(FontManager.MainFontBlack, PageTitleText, New Vector2(60 + 576 - 320 + 32 + 24, 100), Color.White)



    End Sub

    Private Sub DrawHeader()
        Dim CanvasTexture As Texture2D = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(0, 0, 48, 48), "")

        ' Effort Value colors:
        With Me.Pokemon
            Dim AllEVs As Integer = .EVHP + .EVAttack + .EVDefense + .EVSpAttack + .EVSpDefense + .EVSpeed

            If AllEVs > 0 Then
                Dim EVMax As Integer = 510
                If AllEVs > EVMax Then
                    EVMax = AllEVs
                End If

                Dim BlockWidth As Double = 490 / EVMax
                Dim CurrentWidth As Integer = 70
                Dim BlockY As Integer = 192

                Canvas.DrawRectangle(New Rectangle(CurrentWidth, BlockY, CInt(BlockWidth * .EVHP), 10), EVColors(0))
                CurrentWidth += CInt(BlockWidth * .EVHP)

                Canvas.DrawRectangle(New Rectangle(CurrentWidth, BlockY, CInt(BlockWidth * .EVAttack), 10), EVColors(1))
                CurrentWidth += CInt(BlockWidth * .EVAttack)

                Canvas.DrawRectangle(New Rectangle(CurrentWidth, BlockY, CInt(BlockWidth * .EVDefense), 10), EVColors(2))
                CurrentWidth += CInt(BlockWidth * .EVDefense)

                Canvas.DrawRectangle(New Rectangle(CurrentWidth, BlockY, CInt(BlockWidth * .EVSpAttack), 10), EVColors(3))
                CurrentWidth += CInt(BlockWidth * .EVSpAttack)

                Canvas.DrawRectangle(New Rectangle(CurrentWidth, BlockY, CInt(BlockWidth * .EVSpDefense), 10), EVColors(4))
                CurrentWidth += CInt(BlockWidth * .EVSpDefense)

                Canvas.DrawRectangle(New Rectangle(CurrentWidth, BlockY, CInt(BlockWidth * .EVSpeed), 10), EVColors(5))
                Canvas.DrawRectangle(New Rectangle(CurrentWidth, BlockY, CInt(BlockWidth * .EVSpeed), 10), EVColors(5))
                CurrentWidth += CInt(BlockWidth * .EVSpeed)
            End If
        End With

        Canvas.DrawImageBorder(CanvasTexture, 2, New Rectangle(60, 100, 576, 64))
        If Pokemon.GetDisplayName() = Pokemon.GetName() Or Pokemon.IsEgg() = True Then
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Pokemon.GetDisplayName(), New Vector2(158, 132), Color.White)
        Else
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Pokemon.GetDisplayName(), New Vector2(158, 122), Color.White)
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Pokemon.GetName(), New Vector2(164, 152), Color.White)
        End If
        Core.SpriteBatch.Draw(Pokemon.GetMenuTexture(), New Rectangle(70, 110, 64, 64), BattleStats.GetStatColor(Pokemon.Status))
        If Not Pokemon.Item Is Nothing And Pokemon.EggSteps = 0 Then
            Core.SpriteBatch.Draw(Pokemon.Item.Texture, New Rectangle(118, 150, 48, 48), Color.White)
        End If

        ' Portrait:
        Canvas.DrawImageBorder(CanvasTexture, 2, New Rectangle(60, 196, 128, 128))
		Core.SpriteBatch.Draw(Pokemon.GetTexture(FrontView), New Rectangle(72, 208, 128, 128), Color.White)
		Core.SpriteBatch.Draw(Pokemon.CatchBall.Texture, New Rectangle(74, 318, 24, 24), Color.White)
        If Me.Pokemon.IsShiny = True And Me.Pokemon.IsEgg() = False Then
            Core.SpriteBatch.Draw(MainTexture, New Rectangle(78, 218, 18, 18), New Rectangle(118, 4, 9, 9), Color.White)
        End If

        ' Other:
        If Me.Pokemon.EggSteps = 0 Then
            Canvas.DrawImageBorder(CanvasTexture, 2, New Rectangle(60, 356, 128, 224))
            Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\Menus\Types"), New Rectangle(76, 380, 48, 16), Me.Pokemon.Type1.GetElementImage(), Color.White)
            If Me.Pokemon.Type2.Type <> Element.Types.Blank Then
                Core.SpriteBatch.Draw(TextureManager.GetTexture("GUI\Menus\Types"), New Rectangle(124, 380, 48, 16), Me.Pokemon.Type2.GetElementImage(), Color.White)
            End If

            Dim r As New Rectangle(96, 0, 6, 10)
            If Me.Pokemon.Gender = P3D.Pokemon.Genders.Female Then
                r = New Rectangle(102, 0, 6, 10)
            End If
            If Me.Pokemon.Gender <> P3D.Pokemon.Genders.Genderless Then
                Core.SpriteBatch.Draw(MainTexture, New Rectangle(180, 376, 12, 20), r, Color.White)
            End If

			Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Localization.GetString("Level") & ": " & Me.Pokemon.Level & Environment.NewLine & Localization.GetString("poke_status_screen_number") & Pokemon.Number & Environment.NewLine & Localization.GetString("poke_status_screen_nature") & ":" & Environment.NewLine & Me.Pokemon.Nature.ToString(), New Vector2(76, 410), Color.White)

			Dim StatusTexture As Texture2D = BattleStats.GetStatImage(Pokemon.Status)
            If Not StatusTexture Is Nothing Then
                Dim Y As Integer = 139
                If Pokemon.GetDisplayName() <> Pokemon.GetName() Then
                    Y = 127
                End If
                Core.SpriteBatch.Draw(StatusTexture, New Rectangle(CInt(172 + FontManager.MainFontWhite.MeasureString(Pokemon.GetDisplayName()).X), Y + 2, 40, 16), Color.White)
            End If
        End If
    End Sub

    Private Sub DrawPage1()
        Dim CanvasTexture As Texture2D = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(160, 272, 48, 48), "")
        Dim HPBar As Rectangle = New Rectangle(0, 240, 67, 8)
        Dim EXPBar As Rectangle = New Rectangle(0, 248, 83, 8)

        Dim p As Vector2 = New Vector2(144, 192)

        With Core.SpriteBatch
            ' Stats:
            Canvas.DrawImageBorder(CanvasTexture, 2, New Rectangle(220, 196, 416, 256))
            ' HP Bar:
            Core.SpriteBatch.Draw(MainTexture, New Rectangle(CInt(p.X + 128), CInt(p.Y + 32), 134, 16), HPBar, Color.White)

            Dim barX As Integer = CInt((Pokemon.HP / Pokemon.MaxHP) * 92)
            Dim barRectangle As Rectangle
            Dim barPercentage As Integer = CInt((Pokemon.HP / Pokemon.MaxHP) * 100).Clamp(0, 100)

            If barPercentage > 50 Then
                barRectangle = New Rectangle(112, 0, 2, 3)
            ElseIf barPercentage <= 50 And barPercentage > 25 Then
                barRectangle = New Rectangle(114, 0, 2, 3)
            ElseIf barPercentage <= 25 Then
                barRectangle = New Rectangle(116, 0, 2, 3)
            End If
            For x = 0 To barX Step 4
                .Draw(MainTexture, New Rectangle(CInt(p.X + 128 + x + 32), CInt(p.Y + 38), 4, 6), barRectangle, Color.White)
            Next

            Dim redText As String = Environment.NewLine
			Dim blueText As String = Environment.NewLine
			Dim blackText As String = Localization.GetString("HP") & Environment.NewLine
			For i = 0 To 4
				Dim statText As String = ""
				Dim stat As String = ""
				Select Case i
					Case 0
						statText = Localization.GetString("Attack")
						stat = "Attack"
					Case 1
						statText = Localization.GetString("Defense")
						stat = "Defense"
					Case 2
						statText = Localization.GetString("Special_Attack")
						stat = "SpAttack"
					Case 3
						statText = Localization.GetString("Special_Defense")
						stat = "SpDefense"
					Case 4
						statText = Localization.GetString("Speed")
						stat = "Speed"
				End Select

				Dim m As Single = Nature.GetMultiplier(Pokemon.Nature, stat)
				If m > 1.0F Then
					redText &= statText & Environment.NewLine
					blueText &= Environment.NewLine
					blackText &= Environment.NewLine
				ElseIf m < 1.0F Then
					redText &= Environment.NewLine
					blueText &= statText & Environment.NewLine
					blackText &= Environment.NewLine
				Else
					redText &= Environment.NewLine
					blueText &= Environment.NewLine
					blackText &= statText & Environment.NewLine
				End If
			Next

            .DrawString(FontManager.MainFontBlack, blackText, New Vector2(CInt(p.X + 100), CInt(p.Y + 24)), Color.White)
            .DrawString(FontManager.MainFontWhite, redText, New Vector2(CInt(p.X + 100), CInt(p.Y + 24)), New Color(226, 84, 65))
            .DrawString(FontManager.MainFontWhite, blueText, New Vector2(CInt(p.X + 100), CInt(p.Y + 24)), New Color(62, 116, 195))
            .DrawString(FontManager.MainFontBlack, Pokemon.HP & " / " & Pokemon.MaxHP & Environment.NewLine & Pokemon.Attack & Environment.NewLine & Pokemon.Defense & Environment.NewLine & Pokemon.SpAttack & Environment.NewLine & Pokemon.SpDefense & Environment.NewLine & Pokemon.Speed, New Vector2(CInt(p.X + 280), CInt(p.Y + 24)), Color.White)

            ' Experience Points:
            Canvas.DrawImageBorder(CanvasTexture, 2, New Rectangle(220, 484, 416, 96))
            If Pokemon.Level < CInt(GameModeManager.GetGameRuleValue("MaxLevel", "100")) Then

                Dim NextLvExp As Integer = Pokemon.NeedExperience(Me.Pokemon.Level + 1) - Pokemon.NeedExperience(Me.Pokemon.Level)
                Dim currentExp As Integer = Me.Pokemon.Experience - Pokemon.NeedExperience(Me.Pokemon.Level)
                If Pokemon.Level = 1 Then
                    NextLvExp = Pokemon.NeedExperience(Me.Pokemon.Level + 1)
                    currentExp = Me.Pokemon.Experience
                End If

                Dim NeedExp As Integer = NextLvExp - currentExp

                If Pokemon.Level = CInt(GameModeManager.GetGameRuleValue("MaxLevel", "100")) Then
                    NextLvExp = 0
                Else
                    barPercentage = CInt((currentExp / NextLvExp) * 100)
                    barX = CInt(128 / 100 * barPercentage)
                End If

                ' EXP Bar
                Core.SpriteBatch.Draw(MainTexture, New Rectangle(CInt(240 + FontManager.MainFontColor.MeasureString(barPercentage & " %").X + 12), 572, 166, 16), EXPBar, Color.White)

                .DrawString(FontManager.MainFontBlack, Localization.GetString("poke_status_screen_all_exp") & ": " & Pokemon.Experience & Environment.NewLine & Localization.GetString("poke_status_screen_nxt_lv") & ": " & NextLvExp - currentExp, New Vector2(p.X + 100, 504), Color.White)

                For x = 0 To barX Step 4
                    .Draw(MainTexture, New Rectangle(CInt(x + 240 + FontManager.MainFontColor.MeasureString(barPercentage & " %").X + 20 + 24), 572 + 6, 4, 6), New Rectangle(118, 0, 2, 3), Color.White)
                Next

                If barPercentage = 100 Then
                    barPercentage -= 1
                End If

                .DrawString(FontManager.MainFontColor, barPercentage & " %", New Vector2(240, 568), New Color(80, 160, 232))
            End If
        End With

    End Sub

    Private Sub DrawPage2()
        Dim CanvasTexture As Texture2D = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(208, 272, 48, 48), "")
        Dim p As Vector2 = New Vector2(144, 192)

        ' Capture Information: 
        Canvas.DrawImageBorder(CanvasTexture, 2, New Rectangle(220, 196, 416, 96))
        With Core.SpriteBatch
            .DrawString(FontManager.MainFontBlack, Localization.GetString("poke_status_screen_OT") & ": " & Pokemon.OT & " /" & Pokemon.CatchTrainerName & Environment.NewLine & Pokemon.CatchMethod & Environment.NewLine & Pokemon.CatchLocation, New Vector2(p.X + 100, p.Y + 24), Color.White)
        End With

        ' Item:
        Canvas.DrawImageBorder(CanvasTexture, 2, New Rectangle(220, 324, 416, 128))
        If Me.Pokemon.EggSteps = 0 Then
            If Not Pokemon.Item Is Nothing Then
                Core.SpriteBatch.Draw(Pokemon.Item.Texture, New Rectangle(240, 348, 48, 48), Color.White)
                Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Localization.GetString("poke_status_screen_Item") & ": " & Pokemon.Item.Name, New Vector2(p.X + 100 + 32, 340), Color.White)
                Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Pokemon.Item.GetDescription().CropStringToWidth(FontManager.MainFontBlack, 392), New Vector2(p.X + 100, 376), Color.White)
            Else
                Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Localization.GetString("poke_status_screen_Item") & ": " & Localization.GetString("poke_status_screen_no_item"), New Vector2(p.X + 100, 344), Color.White)
            End If
        End If

        ' Ability:
        Canvas.DrawImageBorder(CanvasTexture, 2, New Rectangle(220, 484, 416, 96))
        If Me.Pokemon.EggSteps = 0 Then
            With Core.SpriteBatch
                .DrawString(FontManager.MainFontBlack, Localization.GetString("poke_status_screen_ability") & ": " & Me.Pokemon.Ability.Name & Environment.NewLine & Me.Pokemon.Ability.Description.CropStringToWidth(FontManager.MainFontBlack, 328), New Vector2(p.X + 100, 504), Color.White)
            End With
        Else
            With Core.SpriteBatch
                Dim s As String = """The Egg Watch""" & Environment.NewLine
                Dim percent As Integer = CInt((Me.Pokemon.EggSteps / Me.Pokemon.BaseEggSteps) * 100)
                If percent <= 33 Then
                    s &= "It looks like this Egg will" & Environment.NewLine & "take a long time to hatch."
                ElseIf percent > 33 And percent <= 66 Then
                    s &= "It's getting warmer and moves" & Environment.NewLine & "a little. It will hatch soon."
                Else
                    s &= "There is strong movement" & Environment.NewLine & "noticeable. It will hatch soon!"
                End If

                .DrawString(FontManager.MainFontBlack, s, New Vector2(p.X + 100, 504), Color.White)
            End With
        End If
    End Sub

    Private Sub DrawPage3()
        Dim CanvasTexture As Texture2D = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(256, 272, 48, 48), "")
        Dim p As Vector2 = New Vector2(144, 180)

        If Pokemon.Attacks.Count > 0 Then
            Canvas.DrawImageBorder(CanvasTexture, 2, New Rectangle(CInt(p.X + 128 + AttackPos), 196, 320, 384))
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Localization.GetString("poke_status_screen_closeadvice"), New Vector2(p.X + 214 + AttackPos, 572), Color.White)
            Dim A As BattleSystem.Attack = Pokemon.Attacks(AttackIndex)
            With Core.SpriteBatch
                Dim fullText As String = A.Description
                Dim t As String = ""
                Dim i As Integer = 0
                Dim n As String = ""
                For i = 0 To fullText.Length - 1
                    Dim c As Char = CChar(fullText(i).ToString().Replace("’", "'"))

                    If c = CChar(" ") Then
                        If FontManager.MainFontBlack.MeasureString(n & c).X > 160 Then
                            t &= Environment.NewLine
                            n = ""
                        Else
                            t &= " "
                            n &= " "
                        End If
                    Else
                        t &= c
                        n &= c
                    End If
                Next

                Dim power As String = A.Power.ToString()
                If power = "0" Then
                    power = "-"
                End If

                Dim acc As String = A.Accuracy.ToString()
                If acc = "0" Then
                    acc = "-"
                End If

                .DrawString(FontManager.MainFontBlack, Localization.GetString("poke_status_screen_power") & ": " & power & Environment.NewLine & Localization.GetString("poke_status_screen_accuracy") & ": " & acc & Environment.NewLine & t, New Vector2(CInt(576 - 312 + AttackPos + 96), 218), Color.White)
                .Draw(A.GetDamageCategoryImage(), New Rectangle(CInt(p.X + 256 + 80 + AttackPos), 218, 56, 28), Color.White)
            End With

            Canvas.DrawImageBorder(CanvasTexture, 2, New Rectangle(220, 196, 416, 384))

            For i = 0 To Me.Pokemon.Attacks.Count - 1
                DrawAttack(i, Me.Pokemon.Attacks(i))
            Next
        End If
    End Sub

    Private Sub DrawAttack(ByVal i As Integer, ByVal A As BattleSystem.Attack)
        Dim p As New Vector2(256, 210 + i * (64 + 32))
        Dim FontType As SpriteFont = FontManager.MainFontBlack
        Dim CanvasTexture As Texture2D = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(0, 0, 48, 48), "")
        If Me.AttackToggle = True And Me.AttackIndex = i Then
            CanvasTexture = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(0, 48, 48, 48), "")
            FontType = FontManager.MainFontWhite
        Else
            If SwitchIndex <> -1 And i = SwitchIndex Then
                CanvasTexture = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(0, 48, 48, 48), "")
                FontType = FontManager.MainFontWhite
            End If
        End If

        Canvas.DrawImageBorder(CanvasTexture, 2, New Rectangle(CInt(p.X + 12), CInt(p.Y), 320, 64))

        With Core.SpriteBatch
            .DrawString(FontType, A.Name, New Vector2(CInt(p.X + 30), CInt(p.Y + 26)), Color.White)

            Dim FontColor As Color = Color.White

            Dim per As Integer = CInt((A.CurrentPP / A.MaxPP) * 100)

            If per <= 33 And per > 10 Then
                FontColor = Color.Orange
                FontType = FontManager.MainFontColor
            ElseIf per <= 10 Then
                FontColor = Color.IndianRed
                FontType = FontManager.MainFontColor
            Else
                FontColor = Color.White
            End If

            .DrawString(FontType, Localization.GetString("PP") & " " & A.CurrentPP & " / " & A.MaxPP, New Vector2(CInt(p.X + 160), CInt(p.Y + 58)), FontColor)

            .Draw(TextureManager.GetTexture("GUI\Menus\Types", A.Type.GetElementImage(), ""), New Rectangle(CInt(p.X + 30), CInt(p.Y + 54), 48, 16), Color.White)
        End With
    End Sub

    Public Overrides Sub ChangeTo()
        If dummyPokeIndex <> Me.PokeIndex Then
            If Me.Pokemon.EggSteps = 0 Then
                Me.Pokemon.PlayCry()
                Me.dummyPokeIndex = Me.PokeIndex
            End If
        End If
    End Sub

End Class