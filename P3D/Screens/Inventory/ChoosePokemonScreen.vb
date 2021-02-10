Public Class ChoosePokemonScreen

    Inherits Screen

    Private PokemonList As New List(Of Pokemon)
    Private AltPokemonList As New List(Of Pokemon)

    Public Shared Selected As Integer = -1
    Public Shared Exited As Boolean = False

    Public index As Integer = 0
    Dim MainTexture As Texture2D
    Dim yOffset As Single = 0

    Dim Item As Item
    Dim Title As String = ""

    Dim used As Boolean = False
    Public canExit As Boolean = True

    Public CanChooseFainted As Boolean = True
    Public CanChooseEgg As Boolean = True
    Public CanChooseHMPokemon As Boolean = True

    Public Delegate Sub DoStuff(ByVal PokeIndex As Integer)
    Dim ChoosePokemon As DoStuff
    Public ExitedSub As DoStuff

    Public LearnAttack As BattleSystem.Attack
    Public LearnType As Integer = 0
    Public TextColor As SpriteFont = FontManager.MainFontBlack

    Public Sub New(ByVal currentScreen As Screen, ByVal Item As Item, ByVal ChoosePokemon As DoStuff, ByVal Title As String, ByVal canExit As Boolean, ByVal canChooseFainted As Boolean, ByVal canChooseEgg As Boolean, Optional ByVal _pokemonList As List(Of Pokemon) = Nothing)
        Me.PreScreen = currentScreen
        Me.Identification = Identifications.ChoosePokemonScreen

        Me.MouseVisible = False
        Me.CanChat = Me.PreScreen.CanChat
        Me.CanBePaused = Me.PreScreen.CanBePaused

        Me.Item = Item
        Me.Title = Title
        Me.canExit = canExit

        Me.CanChooseEgg = canChooseEgg
        Me.CanChooseFainted = canChooseFainted

        MainTexture = TextureManager.GetTexture("GUI\Menus\Menu")


        Me.index = Player.Temp.PokemonScreenIndex
        Me.ChoosePokemon = ChoosePokemon
        Me.AltPokemonList = _pokemonList

        GetPokemonList()
    End Sub

    Public Sub New(ByVal currentScreen As Screen, ByVal Item As Item, ByVal ChoosePokemon As DoStuff, ByVal Title As String, ByVal canExit As Boolean)
        Me.New(currentScreen, Item, ChoosePokemon, Title, canExit, True, True)
    End Sub

    Public Sub New(ByVal currentScreen As Screen, ByVal Item As Item, ByVal Title As String, ByVal canExit As Boolean)
        Me.New(currentScreen, Item, Nothing, Title, canExit, True, True)
    End Sub

    Private Sub GetPokemonList()
        Me.PokemonList.Clear()
        If AltPokemonList IsNot Nothing Then
            For Each p As Pokemon In AltPokemonList
                Me.PokemonList.Add(Pokemon.GetPokemonByData(p.GetSaveData()))
            Next
        Else
            For Each p As Pokemon In Core.Player.Pokemons
                Me.PokemonList.Add(Pokemon.GetPokemonByData(p.GetSaveData()))
            Next
        End If
    End Sub

    Public Overrides Sub Update()
        TextBox.Update()
        yOffset += 0.45F

        If TextBox.Showing = False Then
            If used = True Then
                Core.SetScreen(Me.PreScreen)
            Else
                If ChooseBox.Showing = False Then
                    If Controls.Dismiss() = True And Me.canExit = True Then
                        Exited = True
                        Selected = -1
                        If Not ExitedSub Is Nothing Then
                            used = True
                            ExitedSub(index)
                        Else
                            Core.SetScreen(Me.PreScreen)
                        End If
                    End If
                    If Controls.Accept() = True Then
						ShowMenu()
						SoundManager.PlaySound("Select")
					End If

                    If Controls.Right(True, False) Then
                        index += 1
                    End If
                    If Controls.Left(True, False) Then
                        index -= 1
                    End If
                    If Controls.Down(True, False, False) Then
                        index += 2
                    End If
                    If Controls.Up(True, False, False) Then
                        index -= 2
                    End If

                    index = CInt(MathHelper.Clamp(index, 0, Me.PokemonList.Count - 1))
                Else
                    ChooseBox.Update(False)
                    If Controls.Dismiss() = True Then
                        ChooseBox.Showing = False
                    End If
                    If Controls.Accept() = True Then
						AcceptMenu()
						SoundManager.PlaySound("Select")
					End If
                End If
            End If
        End If
    End Sub

    Private Sub AcceptMenu()
        Select Case ChooseBox.index
            Case 0
                If CanChoosePokemon(Me.PokemonList(index)) = True Then
                    Player.Temp.PokemonScreenIndex = Me.index
                    ChooseBox.Showing = False

                    Selected = index

                    If Not ChoosePokemon Is Nothing Then
                        ChoosePokemon(index)
                        GetPokemonList()
                    End If

                    used = True
                    Exited = False
                Else
                    ChooseBox.Showing = False
                    TextBox.Show("Cannot choose this~Pokémon.")
                End If
            Case 1
                ChooseBox.Showing = False
                Core.SetScreen(New PokemonStatusScreen(Me, index, {}, Me.PokemonList(index), True))
            Case 2
                ChooseBox.Showing = False
        End Select
    End Sub

    Private Function CanChoosePokemon(ByVal p As Pokemon) As Boolean
        If Me.CanChooseFainted = False Then
            If p.HP <= 0 Or p.Status = Pokemon.StatusProblems.Fainted Then
                Return False
            End If
        End If
        If Me.CanChooseEgg = False Then
            If p.IsEgg() = True Then
                Return False
            End If
        End If
        If Me.CanChooseHMPokemon = False Then
            If p.HasHMMove() = True Then
                Return False
            End If
        End If
        Return True
    End Function

    Dim MenuID As Integer = 0
    Dim mPressed As Boolean = False
    Private Sub ShowMenu()
        Me.MenuID = 0
        ChooseBox.Show({"Select", Localization.GetString("party_screen_summary"), Localization.GetString("party_screen_back")}, 0, {})
    End Sub

    Public Overrides Sub Draw()
        Me.PreScreen.Draw()

        Dim BackgroundTexture As Texture2D = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(160, 224, 48, 48), "")
        Dim CanvasTexture As Texture2D = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(0, 0, 48, 48), "")

        Canvas.DrawImageBorder(BackgroundTexture, 2, New Rectangle(60, 100, 800, 480))
        Canvas.DrawImageBorder(CanvasTexture, 2, New Rectangle(60, 100, 480, 64))
        Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Me.Title, New Vector2(142, 132), Color.White)
        Core.SpriteBatch.Draw(Item.Texture, New Rectangle(78, 124, 32, 32), Color.White)

        If Me.canExit = True Then
            Core.SpriteBatch.DrawString(FontManager.MainFontWhite, Localization.GetString("party_screen_backadvice"), New Vector2(1200 - FontManager.MainFontBlack.MeasureString(Localization.GetString("party_screen_backadvice")).X - 320, 572), Color.White)
        End If

        For i = 0 To Me.PokemonList.Count - 1
            DrawPokemonTile(i, Me.PokemonList(i))
        Next
        If Me.PokemonList.Count < 6 Then
            For i = Me.PokemonList.Count To 5
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
            If Pokemon.Status = P3D.Pokemon.StatusProblems.Fainted Then
                BorderTexture = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(160, 128, 48, 48), "")
            Else
                BorderTexture = TextureManager.GetTexture("GUI\Menus\Menu", New Rectangle(208, 128, 48, 48), "")
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
                Core.SpriteBatch.Draw(MainTexture, New Rectangle(CInt(p.X + 192 - 32), CInt(p.Y + 32), 134, 16), New Rectangle(0, 240, 67, 8), Color.White)
                Dim barX As Integer = CInt((Pokemon.HP / Pokemon.MaxHP.Clamp(1, Integer.MaxValue)) * 50)
                Dim barRectangle As Rectangle
                Dim barPercentage As Integer = CInt((Pokemon.HP / Pokemon.MaxHP.Clamp(1, Integer.MaxValue)) * 100)

                If barPercentage > 50 Then
                    barRectangle = New Rectangle(112, 0, 2, 3)
                ElseIf barPercentage <= 50 And barPercentage > 25 Then
                    barRectangle = New Rectangle(114, 0, 2, 3)
                ElseIf barPercentage <= 25 Then
                    barRectangle = New Rectangle(116, 0, 2, 3)
                End If
                For x = 0 To barX
                    .Draw(MainTexture, New Rectangle(CInt(p.X + 192 + x), CInt(p.Y + 38), 4, 6), barRectangle, Color.White)
                Next

                .DrawString(TextColor, Localization.GetString("HP") & " " & Pokemon.HP & " / " & Pokemon.MaxHP, New Vector2(CInt(p.X + 160), CInt(p.Y + 48)), Color.White)
            End If

            Dim offset As Single = CSng(Math.Sin(yOffset))
            If i = index Then
                offset *= 3
            End If
            If Pokemon.Status = P3D.Pokemon.StatusProblems.Fainted Then
                offset = 0
            End If

            .Draw(Pokemon.GetMenuTexture(), New Rectangle(CInt(p.X + 5), CInt(p.Y + offset + 10), 64, 64), BattleStats.GetStatColor(Pokemon.Status))
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
                .Draw(Pokemon.Item.Texture, New Rectangle(CInt(p.X + 40), CInt(p.Y + 42), 32, 32), Color.White)
            End If

            Dim space As String = ""
            For x = 1 To 3 - Pokemon.Level.ToString().Length
                space &= " "
            Next

            Dim AttackLable As String = ""
            If LearnType > 0 Then
                AttackLable = Localization.GetString("party_screen_teach_move_unable")
                Select Case LearnType
                    Case 1 ' Technical/Hidden Machine
                        If CType(moveLearnArg, Items.TechMachine).CanTeach(Pokemon) = "" Then
                            AttackLable = Localization.GetString("party_screen_teach_move_able")
                        End If
                End Select
            End If

            If Pokemon.IsEgg() = False Then
                .DrawString(TextColor, Localization.GetString("Lv.") & space & Pokemon.Level, New Vector2(CInt(p.X + 72), CInt(p.Y + 48)), Color.White)
                .DrawString(TextColor, AttackLable, New Vector2(CInt(p.X + 230), CInt(p.Y + 18)), Color.White)
            End If

            Dim StatusTexture As Texture2D = BattleStats.GetStatImage(Pokemon.Status)
            If Not StatusTexture Is Nothing Then
                Core.SpriteBatch.Draw(StatusTexture, New Rectangle(CInt(p.X + 240), CInt(p.Y + 30), 38, 12), Color.White)
            End If
        End With
    End Sub

    Dim moveLearnArg As Object = Nothing

    Public Sub SetupLearnAttack(ByVal a As BattleSystem.Attack, ByVal learnType As Integer, ByVal arg As Object)
        Me.LearnAttack = a
        Me.LearnType = learnType
        Me.moveLearnArg = arg
    End Sub

End Class