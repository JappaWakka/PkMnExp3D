﻿Public Class StatisticsScreen

    Inherits Screen

    Dim Statistics As New Dictionary(Of String, Integer)
    Dim texture As Texture2D
    Dim TileOffset As Integer = 0
    Dim Scroll As Integer = 0

    Dim GJStatistics As New Dictionary(Of String, Decimal)
    Dim GJGrabIndex As Integer = 0
    Dim GJCanGrabNewScore As Boolean = False
    Dim GJGrabDelay As Single = 0.0F
    Dim StatisticsStartIndex As Integer = 0

    Public Sub New(ByVal currentScreen As Screen)
        Me.PreScreen = currentScreen
        Me.Identification = Identifications.StatisticsScreen

        Me.texture = TextureManager.GetTexture("GUI\Menus\General")

        Me.CanBePaused = True
        Me.CanMuteAudio = True

        If Core.Player.IsGamejoltSave = True Then
            StatisticsStartIndex = 2
        End If

        Me.LoadStatistics()
    End Sub

    Private Sub LoadStatistics()
        If Core.Player.IsGamejoltSave = True Then
            Me.Statistics.Add("Level", GameJolt.Emblem.GetPlayerLevel(Core.GameJoltSave.Points))
            Me.Statistics.Add("Points", Core.GameJoltSave.Points)
        End If

        Dim data As String = PlayerStatistics.GetData()
        For Each line As String In data.SplitAtNewline()
            If line.Contains(",") = True Then
                Dim statName As String = line.Remove(line.IndexOf(","))
                Dim statValue As Integer = CInt(line.Remove(0, line.IndexOf(",") + 1))

                If Statistics.ContainsKey(statName) = True Then
                    Statistics.Remove(statName)
                End If
                Statistics.Add(statName, statValue)
            End If
        Next

        If Me.Statistics.Count > Me.StatisticsStartIndex Then ' And Basic.Player.IsGamejoltSave = True And GameJolt.API.LoggedIn = True
            Me.GJGrabIndex = Me.StatisticsStartIndex
            Me.GJCanGrabNewScore = True
        End If
    End Sub

    Public Overrides Sub Draw()
        Canvas.DrawRectangle(Core.windowSize, New Color(84, 198, 216))

        For i = 0 To Me.Statistics.Count - 1
            Dim ItemID As String = ""
            Dim ItemIDX As Integer = 0
            Dim name As String = Me.Statistics.Keys(i)
            If name.StartsWith("[") = True And name.Contains("]") = True Then
                ItemID = name.Remove(name.IndexOf("]")).Remove(0, 1)
                ItemIDX = 40
                name = name.Remove(0, name.IndexOf("]") + 1)
            End If

            Dim value As Integer = Me.Statistics.Values(i)

            If ItemID <> "" Then
                Dim Item As Item = Item.GetItemByID(CInt(ItemID))
                Core.SpriteBatch.Draw(Item.Texture, New Rectangle(150, 192 + i * 64 + Scroll, 48, 48), Color.White)
            End If

            Core.SpriteBatch.DrawString(FontManager.MainFontWhite, name, New Vector2(150 + ItemIDX, 200 + i * 64 + Scroll), Color.White, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F)

            If GJStatistics.ContainsKey(Me.Statistics.Keys(i)) = True Then
                Core.SpriteBatch.DrawString(FontManager.MainFontWhite, GJStatistics(Me.Statistics.Keys(i)).ToString(), New Vector2(Core.windowSize.Width - 418, 200 + i * 64 + Scroll), Color.White, 0.0F, Vector2.Zero, 1.0F, SpriteEffects.None, 0.0F)
                Core.SpriteBatch.DrawString(FontManager.MainFontWhite, value.ToString(), New Vector2(Core.windowSize.Width - 420, 200 + i * 64 + Scroll), Color.White, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F)
            Else
                Core.SpriteBatch.DrawString(FontManager.MainFontWhite, value.ToString(), New Vector2(Core.windowSize.Width - 420, 200 + i * 64 + Scroll), Color.White, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F)
            End If

            Canvas.DrawRectangle(New Rectangle(130, 200 + i * 64 + Scroll, Core.windowSize.Width - 360, 2), Color.White)
        Next

        Canvas.DrawRectangle(New Rectangle(0, 0, Core.windowSize.Width, 150), New Color(84, 198, 216))
        Canvas.DrawRectangle(New Rectangle(0, Core.windowSize.Height - 100, Core.windowSize.Width, 100), New Color(84, 198, 216))

        Canvas.DrawGradient(New Rectangle(50, 150, 50, 4), New Color(255, 255, 255, 0), Color.White, True, -1)
        Canvas.DrawRectangle(New Rectangle(100, 150, Core.windowSize.Width - 300, 4), Color.White)
        Canvas.DrawGradient(New Rectangle(Core.windowSize.Width - 200, 150, 50, 4), Color.White, New Color(255, 255, 255, 0), True, -1)

        Canvas.DrawGradient(New Rectangle(Core.windowSize.Width - 450, 100, 2, 50), New Color(255, 255, 255, 0), Color.White, False, -1)
        Canvas.DrawRectangle(New Rectangle(Core.windowSize.Width - 450, 150, 2, Core.windowSize.Height - 250), Color.White)
        Canvas.DrawGradient(New Rectangle(Core.windowSize.Width - 450, Core.windowSize.Height - 100, 2, 50), Color.White, New Color(255, 255, 255, 0), False, -1)

        Canvas.DrawGradient(New Rectangle(50, Core.windowSize.Height - 100, 50, 4), New Color(255, 255, 255, 0), Color.White, True, -1)
        Canvas.DrawRectangle(New Rectangle(100, Core.windowSize.Height - 100, Core.windowSize.Width - 300, 4), Color.White)
        Canvas.DrawGradient(New Rectangle(Core.windowSize.Width - 200, Core.windowSize.Height - 100, 50, 4), Color.White, New Color(255, 255, 255, 0), True, -1)


        Core.SpriteBatch.DrawString(FontManager.MainFontWhite, "Name", New Vector2(150, 144), Color.White, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F)
        Core.SpriteBatch.DrawString(FontManager.MainFontWhite, "Value", New Vector2(Core.windowSize.Width - 420, 144), Color.White, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F)

        Core.SpriteBatch.DrawString(FontManager.MainFontWhite, "Statistics", New Vector2(100, 48), Color.White, 0.0F, Vector2.Zero, 2.0F, SpriteEffects.None, 0.0F)
    End Sub

    Public Overrides Sub Update()
        If Controls.Up(False, True, False, True, True, True) = True Then
            If Controls.ShiftDown() = True Then
                Me.Scroll += 14
            Else
                Me.Scroll += 7
            End If
        End If
        If Controls.Down(False, True, False, True, True, True) = True Then
            If Controls.ShiftDown() = True Then
                Me.Scroll -= 14
            Else
                Me.Scroll -= 7
            End If
        End If

        If Controls.Up(True, False, True, False, False, False) = True Then
            If Controls.ShiftDown() = True Then
                Me.Scroll += 70
            Else
                Me.Scroll += 35
            End If
        End If
        If Controls.Down(True, False, True, False, False, False) = True Then
            If Controls.ShiftDown() = True Then
                Me.Scroll -= 70
            Else
                Me.Scroll -= 35
            End If
        End If

        If -Me.Statistics.Count * 50 + Core.windowSize.Height - 250 >= 0 Then
            Me.Scroll = 0
        Else
            Me.Scroll = Me.Scroll.Clamp(-Me.Statistics.Count * 50 + Core.windowSize.Height - 250, 0)
        End If

        If Me.GJCanGrabNewScore = True Then
            If GJGrabDelay <= 0.0F Then
                Me.GJCanGrabNewScore = False
                GameJolt.GameJoltStatistics.GetStatisticValue(Me.Statistics.Keys(Me.GJGrabIndex), AddressOf Me.GetGJStatistic)
            Else
                GJGrabDelay -= 0.1F
            End If
        End If

        If Controls.Dismiss() = True Then
            Core.SetScreen(New TransitionScreen(Me, Me.PreScreen, Color.White, False))
        End If

        TileOffset += 1
        If TileOffset >= 64 Then
            TileOffset = 0
        End If
    End Sub

    Private Sub GetGJStatistic(ByVal result As String)
        Dim statName As String = Me.Statistics.Keys(Me.GJGrabIndex)
        Dim statValue As String = "0"

        Dim list As List(Of GameJolt.API.JoltValue) = GameJolt.API.HandleData(result)
        If CBool(list(0).Value) = True Then
            statValue = list(1).Value
            If Me.GJStatistics.ContainsKey(statName) = True Then
                Me.GJStatistics(statName) = CDec(statValue)
            Else
                Me.GJStatistics.Add(statName, CDec(statValue))
            End If
        End If

        Me.GJGrabIndex += 1

        If Me.GJGrabIndex > Me.Statistics.Count - 1 Then
            Me.GJGrabIndex = Me.StatisticsStartIndex
            Me.GJGrabDelay = 25.0F + CSng(Me.Statistics.Count)
        End If

        Me.GJCanGrabNewScore = True
    End Sub

End Class