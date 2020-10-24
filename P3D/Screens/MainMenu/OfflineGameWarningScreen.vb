﻿Public Class OfflineGameWarningScreen

    Inherits Screen

    Public Sub New(ByVal currentScreen As Screen)
        Me.PreScreen = currentScreen
        Me.Identification = Identifications.OfflineGameWarningScreen

        Me.CanBePaused = False
        Me.CanChat = False
        Me.CanDrawDebug = True
        Me.CanGoFullscreen = True
        Me.CanMuteMusic = True
        Me.CanTakeScreenshot = True
        Me.MouseVisible = True
    End Sub

    Public Overrides Sub Draw()
        Me.PreScreen.Draw()

        Canvas.DrawRectangle(Core.windowSize, New Color(0, 0, 0, 150))
        Canvas.DrawRectangle(New Rectangle(CInt(Core.windowSize.Width / 2 - 310), 90, 620, 420), New Color(16, 16, 16))
        Canvas.DrawRectangle(New Rectangle(CInt(Core.windowSize.Width / 2 - 300), 100, 600, 400), New Color(39, 39, 39))

		Core.SpriteBatch.DrawString(FontManager.MainFontWhite, Localization.GetString("offline_newgame_start", "Start a new offline game."), New Vector2(CSng(Core.windowSize.Width / 2 - 280), 128), Color.White)

		Dim t As String = Localization.GetString("offline_newgame_warning", "If you start a game in ""Offline Mode"" by pressing the ""New Game"" button, you cannot access the official Pokémon 3D server and various online features there such as trading and trainer customization. Click on the GameJolt button in the lower right corner in order to start a game in ""Online Mode"". You can still access servers that allow offline save games by pressing the ""Load Game"" button after saving your offline game.")
        t = t.CropStringToWidth(FontManager.MainFontWhite, 450)

		Core.SpriteBatch.DrawString(FontManager.MainFontWhite, t, New Vector2(CSng(Core.windowSize.Width / 2 - 280) + 50, 160), Color.White)

		Dim d As New Dictionary(Of Buttons, String)
        d.Add(Buttons.A, Localization.GetString("game_interaction_accept", "Accept"))
        d.Add(Buttons.B, Localization.GetString("game_interaction_dismiss", "Dismiss"))
        DrawGamePadControls(d, New Vector2(CSng(Core.windowSize.Width / 2) - 140, 420))
    End Sub

    Public Overrides Sub Update()
        If Controls.Accept(True, True, True) = True Then
            Core.SetScreen(Me.PreScreen)
        End If
        If Controls.Dismiss(True, True, True) = True Then
            Core.SetScreen(Me.PreScreen)
            Core.GameOptions.StartedOfflineGame = False
            Core.GameOptions.SaveOptions()
        End If
    End Sub

End Class