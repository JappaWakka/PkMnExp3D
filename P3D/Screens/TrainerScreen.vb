﻿Public Class TrainerScreen

    Inherits Screen

    Dim mainTexture As Texture2D
    Dim charTexture As Texture2D

    Dim index As Vector2 = New Vector2(0)

    Public Sub New(ByVal currentScreen As Screen)
        Me.Identification = Identifications.TrainerScreen
        Me.PreScreen = currentScreen

        Me.mainTexture = TextureManager.GetTexture("GUI\Menus\Menu")

        If Screen.Level.Surfing = True Then
            Me.charTexture = TextureManager.GetTexture("Textures\NPC\" & Core.Player.TempSurfSkin)
        Else
            If Screen.Level.Riding = True Then
                Me.charTexture = TextureManager.GetTexture("Textures\NPC\" & Core.Player.TempRideSkin)
            Else
                Me.charTexture = Screen.Level.OwnPlayer.Texture
            End If
        End If

        Dim frameSize As Size = New Size(CInt(Me.charTexture.Width / 3), CInt(Me.charTexture.Height / 4))

        Me.charTexture = TextureManager.GetTexture(Me.charTexture, New Rectangle(0, frameSize.Width * 2, frameSize.Width, frameSize.Height))

        Dim hasKanto As Boolean = True
        For i = 1 To 8
            If Core.Player.Badges.Contains(i) = False Then
                hasKanto = False
            End If
        Next
        Dim hasJohto As Boolean = True
        For i = 9 To 16
            If Core.Player.Badges.Contains(i) = False Then
                hasJohto = False
            End If
        Next

        If hasKanto = True Then
            GameJolt.Emblem.AchieveEmblem("kanto")
        End If
        If hasJohto = True Then
            GameJolt.Emblem.AchieveEmblem("johto")
        End If
    End Sub

    Public Overrides Sub Draw()
        Me.PreScreen.Draw()

        Canvas.DrawImageBorder(TextureManager.GetTexture(mainTexture, New Rectangle(0, 0, 48, 48)), 2, New Rectangle(60, 100, 800, 480))
        DrawHeader()
        DrawContent()
        DrawBadges()

        Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Localization.GetString("trainer_screen_backadvice"), New Vector2(860 - FontManager.MainFontBlack.MeasureString(Localization.GetString("trainer_screen_backadvice")).X, 572), Color.White)
    End Sub

    Private Sub DrawHeader()
        Canvas.DrawImageBorder(TextureManager.GetTexture(mainTexture, New Rectangle(0, 0, 48, 48)), 2, New Rectangle(60, 100, 480, 64))
        Core.SpriteBatch.Draw(mainTexture, New Rectangle(78, 124, 60, 48), New Rectangle(108, 112, 20, 16), Color.White)
		Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Localization.GetString("trainer_screen_trainer_card"), New Vector2(154, 132), Color.White)
	End Sub

    Private Sub DrawContent()
        If Core.Player.IsGameJoltSave = True Then
            GameJolt.Emblem.Draw(GameJolt.API.username, Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Points, Core.GameJoltSave.Gender, Core.GameJoltSave.Emblem, New Vector2(80, 205), 4, Core.GameJoltSave.DownloadedSprite)

            Dim currentLevel As Integer = GameJolt.Emblem.GetPlayerLevel(Core.GameJoltSave.Points)

            Dim needPoints As Integer = 0
            Dim totalNeedPoints As Integer = 0

            Dim hasPoints As Integer = Core.GameJoltSave.Points
            Dim needPointsCurrentLevel As Integer = GameJolt.Emblem.GetPointsForLevel(currentLevel)
            Dim needPointsNextLevel As Integer = GameJolt.Emblem.GetPointsForLevel(currentLevel + 1)

            totalNeedPoints = needPointsNextLevel - needPointsCurrentLevel
            needPoints = totalNeedPoints - (hasPoints - needPointsCurrentLevel)

            Dim hasPointsThisLevel As Integer = totalNeedPoints - needPoints

            Dim currentSprite As Texture2D = GameJolt.Emblem.GetPlayerSprite(currentLevel, Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Gender)
            Dim nextSprite As Texture2D = GameJolt.Emblem.GetPlayerSprite(currentLevel + 1, Core.GameJoltSave.GameJoltID, Core.GameJoltSave.Gender)

            Core.SpriteBatch.Draw(currentSprite, New Rectangle(100, 364, 32, 32), New Rectangle(0, 64, 32, 32), Color.White)
            Core.SpriteBatch.Draw(nextSprite, New Rectangle(470, 364, 32, 32), New Rectangle(0, 64, 32, 32), Color.White)

            If totalNeedPoints > 0 Then
                Canvas.DrawScrollBar(New Vector2(140, 380), totalNeedPoints, hasPointsThisLevel, 0, New Size(320, 16), True, Color.Black, New Color(255, 165, 0))
                Canvas.DrawScrollBar(New Vector2(140, 380), totalNeedPoints, hasPointsThisLevel, 0, New Size(320, 6), True, Color.Black, New Color(255, 203, 108))
            Else
                Canvas.DrawRectangle(New Rectangle(140, 380, 320, 16), Color.Black)
            End If

			Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "Rank: " & currentLevel, New Vector2(100, 400), Color.White)
			Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "Rank: " & currentLevel + 1, New Vector2(430, 400), Color.White)

			If needPoints = 1 Then
				Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "Need " & needPoints & " point", New Vector2(232, 400), Color.White)
			Else
				Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "Need " & needPoints & " points", New Vector2(232, 400), Color.White)
			End If

            Dim EmblemName As String = Core.GameJoltSave.Emblem
			Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "Current Emblem: """ & EmblemName(0).ToString().ToUpper() & EmblemName.Substring(1, EmblemName.Length - 1) & """ (" & CStr(Core.GameJoltSave.AchievedEmblems.IndexOf(EmblemName) + 1) & "/" & Core.GameJoltSave.AchievedEmblems.Count & ") - use arrow keys to change.", New Vector2(80, 333), Color.White)

            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Localization.GetString("trainer_screen_money") & ": " & Environment.NewLine & Localization.GetString("trainer_screen_time") & ": ", New Vector2(610, 220), Color.White)
            With Core.Player
                Core.SpriteBatch.DrawString(FontManager.MainFontBlack, "$" & .Money & Environment.NewLine & Environment.NewLine & TimeHelpers.GetDisplayTime(TimeHelpers.GetCurrentPlayTime(), True), New Vector2(700, 220), Color.White)
            End With
		Else
			Canvas.DrawImageBorder(TextureManager.GetTexture(mainTexture, New Rectangle(0, 0, 48, 48)), 2, New Rectangle(572, 100, 288, 288))

            Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Localization.GetString("trainer_screen_name") & ": " & Environment.NewLine & Localization.GetString("trainer_screen_points") & ": " & Environment.NewLine & Localization.GetString("trainer_screen_money") & ": " & Environment.NewLine & Localization.GetString("trainer_screen_time") & ": ", New Vector2(108, 220), Color.White)

            Dim displayPoints As Integer = Core.Player.Points
			If Core.Player.IsGameJoltSave = True Then
				displayPoints = Core.GameJoltSave.Points
			End If

			With Core.Player
                Core.SpriteBatch.DrawString(FontManager.MainFontBlack, .Name & " /" & .OT & Environment.NewLine & displayPoints.ToString() & Environment.NewLine & "$" & .Money & Environment.NewLine & TimeHelpers.GetDisplayTime(TimeHelpers.GetCurrentPlayTime(), True), New Vector2(258, 220), Color.White, 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F)
            End With
            Core.SpriteBatch.Draw(charTexture, New Rectangle(601, 126, 256, 256), Color.White)
        End If

        Canvas.DrawImageBorder(TextureManager.GetTexture(mainTexture, New Rectangle(0, 0, 48, 48)), 2, New Rectangle(60, 420, 800, 160))
    End Sub

    Private Sub DrawBadges()
        If Core.Player.Badges.Count > 0 Then
			Core.SpriteBatch.DrawString(FontManager.MainFontBlack, Localization.GetString("trainer_screen_collected_badges") & ": " & Core.Player.Badges.Count, New Vector2(108, 450), Color.White)

			Dim selectedRegion As String = Badge.GetRegion(CInt(index.Y))
            Dim badgesCount As Integer = Badge.GetBadgesCount(selectedRegion)

            For i = 0 To badgesCount - 1
                Dim badgeID As Integer = Badge.GetBadgeID(selectedRegion, i)

                Dim c As Color = Color.White
                Dim t As String = Badge.GetBadgeName(badgeID) & Localization.GetString("trainer_screen_badge")
                If Badge.PlayerHasBadge(badgeID) = False Then
                    c = Color.Black
                    t = Localization.GetString("trainer_screen_empty_badge")
                End If

                Core.SpriteBatch.Draw(Badge.GetBadgeTexture(badgeID), New Rectangle(60 + (i + 1) * 64, 480, 50, 50), c)
                If i = CInt(index.X) Then
					Core.SpriteBatch.DrawString(FontManager.MainFontBlack, t, New Vector2(60 + (i + 1) * 64 + 25 - FontManager.MainFontBlack.MeasureString(t).X / 2, 540), Color.White)
				End If
            Next
        End If
    End Sub

    Public Overrides Sub Update()
        If Controls.Right(True, True, False, True) = True Then
            index.X += 1
        End If
        If Controls.Left(True, True, False, True) = True Then
            index.X -= 1
        End If
        If Controls.Up(True, False, True, True) = True Then
            index.Y -= 1
        End If
        If Controls.Down(True, False, True, True) = True Then
            index.Y += 1
        End If

        If Core.Player.IsGameJoltSave = True Then
            Dim EmblemName As String = Core.GameJoltSave.Emblem
            Dim newIndex As Integer = Core.GameJoltSave.AchievedEmblems.IndexOf(EmblemName)
            If Controls.Up(True, True, False, False) = True Then
                newIndex -= 1
            End If
            If Controls.Down(True, True, False, False) = True Then
                newIndex += 1
            End If
            If newIndex >= 0 And newIndex < Core.GameJoltSave.AchievedEmblems.Count Then
                Core.GameJoltSave.Emblem = Core.GameJoltSave.AchievedEmblems(newIndex)
            End If
        End If

        index.Y = index.Y.Clamp(0, Badge.GetRegionCount() - 1)
        index.X = index.X.Clamp(0, Badge.GetBadgesCount(Badge.GetRegion(CInt(index.Y))) - 1)

        If Controls.Dismiss() = True Then
            Core.SetScreen(Me.PreScreen)
        End If
    End Sub
End Class