Public Class BattleGrowStatsScreen

    Inherits Screen

    Dim mainTexture As Texture2D

    Dim newMaxHP As Integer = 0
    Dim newAttack As Integer = 0
    Dim newDefense As Integer = 0
    Dim newSpDefense As Integer = 0
    Dim newSpAttack As Integer = 0
    Dim newSpeed As Integer = 0

    Dim Delay As Single = 0.0F

    Dim Pokemon As Pokemon = Nothing
    Dim OldStats() As Integer

    Public Sub New(ByVal currentScreen As Screen, ByVal p As Pokemon, ByVal OldStats() As Integer)
        Me.mainTexture = TextureManager.GetTexture("GUI\Menus\Menu")
        Me.PreScreen = currentScreen
        Me.Identification = Identifications.BattleGrowStatsScreen

        newMaxHP = p.MaxHP - OldStats(0)
        newAttack = p.Attack - OldStats(1)
        newDefense = p.Defense - OldStats(2)
        newSpAttack = p.SpAttack - OldStats(3)
        newSpDefense = p.SpDefense - OldStats(4)
        newSpeed = p.Speed - OldStats(5)

        Me.Pokemon = p
        Me.OldStats = OldStats
    End Sub

    Public Overrides Sub Draw()
        PreScreen.Draw()

        Dim p As New Vector2(Core.windowSize.Width - (544), 32)

        Canvas.DrawImageBorder(TextureManager.GetTexture(mainTexture, New Rectangle(0, 0, 48, 48)), 2, New Rectangle(CInt(p.X), CInt(p.Y), 480, 352))
        Dim pokeTexture = Pokemon.GetMenuTexture()
        Core.SpriteBatch.Draw(pokeTexture, New Rectangle(CInt(p.X + 20), CInt(p.Y + 20), pokeTexture.Width * 2, 64), Color.White)
        Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Pokemon.GetDisplayName(), New Vector2(p.X + 90, p.Y + 32), Color.White)
        Core.SpriteBatch.DrawString(FontManager.MainFontBlack, " reached level " & Pokemon.Level & "!", New Vector2(p.X + 90 + FontManager.MainFontWhite.MeasureString(Pokemon.GetDisplayName()).X, p.Y + 41), Color.White)

        If Delay >= 3.0F Then
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "Max HP:      " & OldStats(0).ToString(), New Vector2(p.X + 32, p.Y + 84), Color.White)
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "Attack:       " & OldStats(1).ToString(), New Vector2(p.X + 32, p.Y + 124), Color.White)
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "Defense:     " & OldStats(2).ToString(), New Vector2(p.X + 32, p.Y + 164), Color.White)
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "Sp Attack:   " & OldStats(3).ToString(), New Vector2(p.X + 32, p.Y + 204), Color.White)
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "Sp Defense: " & OldStats(4).ToString(), New Vector2(p.X + 32, p.Y + 244), Color.White)
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "Speed:        " & OldStats(5).ToString(), New Vector2(p.X + 32, p.Y + 284), Color.White)
        End If
        If Delay >= 5.0F Then
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "+ " & newMaxHP, New Vector2(p.X + 200, p.Y + 84), Color.White)
        End If
        If Delay >= 5.5F Then
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "+ " & newAttack, New Vector2(p.X + 200, p.Y + 124), Color.White)
        End If
        If Delay >= 6.0F Then
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "+ " & newDefense, New Vector2(p.X + 200, p.Y + 164), Color.White)
        End If
        If Delay >= 6.5F Then
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "+ " & newSpAttack, New Vector2(p.X + 200, p.Y + 204), Color.White)
        End If
        If Delay >= 7.0F Then
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "+ " & newSpDefense, New Vector2(p.X + 200, p.Y + 244), Color.White)
        End If
        If Delay >= 7.5F Then
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "+ " & newSpeed, New Vector2(p.X + 200, p.Y + 284), Color.White)
        End If
        If Delay >= 9.0F Then
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "= " & Pokemon.MaxHP, New Vector2(p.X + 252, p.Y + 84), Color.White)
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "= " & Pokemon.Attack, New Vector2(p.X + 252, p.Y + 124), Color.White)
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "= " & Pokemon.Defense, New Vector2(p.X + 252, p.Y + 164), Color.White)
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "= " & Pokemon.SpAttack, New Vector2(p.X + 252, p.Y + 204), Color.White)
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "= " & Pokemon.SpDefense, New Vector2(p.X + 252, p.Y + 244), Color.White)
            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "= " & Pokemon.Speed, New Vector2(p.X + 252, p.Y + 284), Color.White)
        End If
        If Delay >= 11.0F Then
            Dim newStat As Integer = 0
            newStat = newAttack + newDefense + newSpAttack + newMaxHP + newSpDefense + newSpeed

            Core.SpriteBatch.DrawString(FontManager.MainFontWhite, Pokemon.GetDisplayName() & " got a boost of " & newStat.ToString() & "!", New Vector2(p.X + 32, p.Y + 320), Color.DarkRed)
        End If
    End Sub

    Public Overrides Sub Update()
        Delay += 0.1F

        If Controls.Accept() = True Then
            If Delay >= 13.0F Then
                Core.SetScreen(Me.PreScreen)
            End If
        End If
    End Sub
End Class