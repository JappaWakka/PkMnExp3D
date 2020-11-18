Public Class Waterfall

    Inherits Entity

    Shared WaterfallTexturesTemp As New Dictionary(Of String, Texture2D)
    Shared WaterTexturesTemp As New Dictionary(Of String, Texture2D)

    Dim waterFallTextureName As String = ""
    Dim waterTextureName As String = ""

    Dim WaterAnimation As Animation
    Dim currentRectangle As New Rectangle(0, 0, 0, 0)

    Public Shared Sub ClearAnimationResources()
        WaterfallTexturesTemp.Clear()
        WaterTexturesTemp.Clear()
    End Sub

    Public Overrides Sub Initialize()
        MyBase.Initialize()

        WaterAnimation = New Animation(TextureManager.GetTexture("Textures\TextureSheets\WaterFall"), 1, 4, 16, 16, 9, 0, 0)

        CreateWaterTextureTemp()
    End Sub

    Private Sub CreateWaterTextureTemp()
        If Core.GameOptions.GraphicStyle = 1 Then
            Dim textureData As List(Of String) = Me.AdditionalValue.Split(CChar(",")).ToList()

            If textureData.Count >= 5 Then
                Dim r As New Rectangle(CInt(textureData(1)), CInt(textureData(2)), CInt(textureData(3)), CInt(textureData(4)))
                Dim texturePath As String = textureData(0)
                Me.waterFallTextureName = AdditionalValue
                If Waterfall.WaterfallTexturesTemp.ContainsKey(AdditionalValue & "_0") = False Then
                    Waterfall.WaterTexturesTemp.Add(AdditionalValue & "_0", TextureManager.GetTexture(texturePath, New Rectangle(r.X, r.Y, r.Width, r.Height)))
                    Waterfall.WaterTexturesTemp.Add(AdditionalValue & "_1", TextureManager.GetTexture(texturePath, New Rectangle(r.X + r.Width, r.Y, r.Width, r.Height)))
                    Waterfall.WaterTexturesTemp.Add(AdditionalValue & "_2", TextureManager.GetTexture(texturePath, New Rectangle(r.X + r.Width * 2, r.Y, r.Width, r.Height)))
                    Waterfall.WaterTexturesTemp.Add(AdditionalValue & "_3", TextureManager.GetTexture(texturePath, New Rectangle(r.X + r.Width * 3, r.Y, r.Width, r.Height)))
                    Waterfall.WaterTexturesTemp.Add(AdditionalValue & "_4", TextureManager.GetTexture(texturePath, New Rectangle(r.X + r.Width * 4, r.Y, r.Width, r.Height)))
                    Waterfall.WaterTexturesTemp.Add(AdditionalValue & "_5", TextureManager.GetTexture(texturePath, New Rectangle(r.X + r.Width * 5, r.Y, r.Width, r.Height)))
                    Waterfall.WaterTexturesTemp.Add(AdditionalValue & "_6", TextureManager.GetTexture(texturePath, New Rectangle(r.X + r.Width * 6, r.Y, r.Width, r.Height)))
                    Waterfall.WaterTexturesTemp.Add(AdditionalValue & "_7", TextureManager.GetTexture(texturePath, New Rectangle(r.X + r.Width * 7, r.Y, r.Width, r.Height)))
                    Waterfall.WaterTexturesTemp.Add(AdditionalValue & "_8", TextureManager.GetTexture(texturePath, New Rectangle(r.X + r.Width * 8, r.Y, r.Width, r.Height)))
                    Waterfall.WaterTexturesTemp.Add(AdditionalValue & "_9", TextureManager.GetTexture(texturePath, New Rectangle(r.X + r.Width * 9, r.Y, r.Width, r.Height)))
                    Waterfall.WaterTexturesTemp.Add(AdditionalValue & "_10", TextureManager.GetTexture(texturePath, New Rectangle(r.X + r.Width * 10, r.Y, r.Width, r.Height)))
                    Waterfall.WaterTexturesTemp.Add(AdditionalValue & "_11", TextureManager.GetTexture(texturePath, New Rectangle(r.X + r.Width * 11, r.Y, r.Width, r.Height)))
                End If
            Else
                If Waterfall.WaterfallTexturesTemp.ContainsKey("_0") = False Then
                    Waterfall.WaterfallTexturesTemp.Add("_0", TextureManager.GetTexture("TextureSheets\WaterFall", New Rectangle(0, 0, 16, 16)))
                    Waterfall.WaterfallTexturesTemp.Add("_1", TextureManager.GetTexture("TextureSheets\WaterFall", New Rectangle(16, 0, 16, 16)))
                    Waterfall.WaterfallTexturesTemp.Add("_2", TextureManager.GetTexture("TextureSheets\WaterFall", New Rectangle(32, 0, 16, 16)))
                    Waterfall.WaterfallTexturesTemp.Add("_3", TextureManager.GetTexture("TextureSheets\WaterFall", New Rectangle(48, 0, 16, 16)))
                End If
            End If

            If textureData.Count >= 10 Then
                Dim r As New Rectangle(CInt(textureData(6)), CInt(textureData(7)), CInt(textureData(8)), CInt(textureData(9)))
                Dim texturePath As String = textureData(5)
                Me.waterTextureName = AdditionalValue
                If Waterfall.WaterTexturesTemp.ContainsKey(AdditionalValue & "_0") = False Then
                    Waterfall.WaterTexturesTemp.Add(AdditionalValue & "_0", TextureManager.GetTexture(texturePath, New Rectangle(r.X, r.Y, r.Width, r.Height)))
                    Waterfall.WaterTexturesTemp.Add(AdditionalValue & "_1", TextureManager.GetTexture(texturePath, New Rectangle(r.X + r.Width, r.Y, r.Width, r.Height)))
                    Waterfall.WaterTexturesTemp.Add(AdditionalValue & "_2", TextureManager.GetTexture(texturePath, New Rectangle(r.X + r.Width * 2, r.Y, r.Width, r.Height)))
                    Waterfall.WaterTexturesTemp.Add(AdditionalValue & "_3", TextureManager.GetTexture(texturePath, New Rectangle(r.X + r.Width * 3, r.Y, r.Width, r.Height)))
                End If
            Else
                If Waterfall.WaterTexturesTemp.ContainsKey("_0") = False Then
                    Waterfall.WaterTexturesTemp.Add("_0", TextureManager.GetTexture("Textures\TextureSheets\WaterFall", New Rectangle(0, 0, 16, 16)))
                    Waterfall.WaterTexturesTemp.Add("_1", TextureManager.GetTexture("Textures\TextureSheets\WaterFall", New Rectangle(16, 0, 16, 16)))
                    Waterfall.WaterTexturesTemp.Add("_2", TextureManager.GetTexture("Textures\TextureSheets\WaterFall", New Rectangle(32, 0, 16, 16)))
                    Waterfall.WaterTexturesTemp.Add("_3", TextureManager.GetTexture("TextureSheets\WaterFall", New Rectangle(48, 0, 16, 16)))
                End If
            End If

        End If
    End Sub

    Public Overrides Sub UpdateEntity()
        If Not WaterAnimation Is Nothing Then
            WaterAnimation.Update(0.01)
            If currentRectangle <> WaterAnimation.TextureRectangle Then
                ChangeTexture()

                currentRectangle = WaterAnimation.TextureRectangle
            End If
        End If

        MyBase.UpdateEntity()
    End Sub

    Private Sub ChangeTexture()
        If Core.GameOptions.GraphicStyle = 1 Then
            If WaterfallTexturesTemp.Count = 0 Or WaterTexturesTemp.Count = 0 Then
                ClearAnimationResources()
                CreateWaterTextureTemp()
            End If

            Select Case WaterAnimation.CurrentColumn
                Case 0
                    Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterFallTextureName & "_0")
                Case 1
                    Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterFallTextureName & "_1")
                Case 2
                    Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterFallTextureName & "_2")
                Case 3
                    Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterFallTextureName & "_3")
            End Select
            Select Case Me.Rotation.Y
                Case 0, MathHelper.TwoPi
                    Select Case WaterAnimation.CurrentColumn
                        Case 0
                            Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterTextureName & "_0")
                        Case 1
                            Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterTextureName & "_1")
                        Case 2
                            Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterTextureName & "_2")
                        Case 3
                            Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterTextureName & "_3")
                    End Select
                Case MathHelper.Pi * 0.5F
                    Select Case WaterAnimation.CurrentColumn
                        Case 0
                            Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterTextureName & "_0")
                        Case 1
                            Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterTextureName & "_1")
                        Case 2
                            Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterTextureName & "_2")
                        Case 3
                            Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterTextureName & "_3")
                    End Select
                Case MathHelper.Pi
                    Select Case WaterAnimation.CurrentColumn
                        Case 0
                            Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterTextureName & "_0")
                        Case 1
                            Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterTextureName & "_1")
                        Case 2
                            Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterTextureName & "_2")
                        Case 3
                            Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterTextureName & "_3")
                    End Select
                Case MathHelper.Pi * 1.5
                    Select Case WaterAnimation.CurrentColumn
                        Case 0
                            Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterTextureName & "_0")
                        Case 1
                            Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterTextureName & "_1")
                        Case 2
                            Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterTextureName & "_2")
                        Case 3
                            Me.Textures(0) = Waterfall.WaterfallTexturesTemp(waterTextureName & "_3")
                    End Select
            End Select
        End If
    End Sub

    Public Overrides Sub Render()
        Me.Draw(Me.Model, Textures, False)
    End Sub

    Private Function ReturnWaterFallPokemonName() As Pokemon
        For Each p As Pokemon In Core.Player.Pokemons
            If p.IsEgg() = False Then
                For Each a As BattleSystem.Attack In p.Attacks
                    If a.Name.ToLower() = "waterfall" Then
                        Return p
                    End If
                Next
            End If
        Next
        Return Nothing
    End Function

    Public Overrides Sub WalkOntoFunction()
        If Me.ActionValue = 1 Then
            Exit Sub
        End If

        Dim isOnTop As Boolean = True
        Dim OnTopcheckPosition As Vector3 = New Vector3(Me.Position.X, Me.Position.Y + 1, Me.Position.Z)
        Dim Oe As Entity = GetEntity(Screen.Level.Entities, OnTopcheckPosition, True, {GetType(Waterfall)})
        If Not Oe Is Nothing Then
            If Oe.EntityID = "Waterfall" Then
                isOnTop = False
            End If
        End If

        If isOnTop = True Then
            Dim s As String = ""

            Dim Steps As Integer = 0
            If Screen.Level.Surfing = False Then
                Steps = 1
            End If

            Dim checkPosition As New Vector3(Me.Position.X, Me.Position.Y - 1, Me.Position.Z)
            Dim foundSteps As Boolean = True
            While foundSteps = True
                Dim e As Entity = GetEntity(Screen.Level.Entities, checkPosition, True, {GetType(Waterfall)})
                If Not e Is Nothing Then
                    If e.EntityID = "Waterfall" Then
                        Steps += 1
                        checkPosition.Y -= 1
                    Else
                        foundSteps = False
                    End If
                Else
                    foundSteps = False
                End If
            End While

            s = "version=2" & Environment.NewLine &
                "@pokemon.hide" & Environment.NewLine &
                "@player.move(2)" & Environment.NewLine &
                "@player.setmovement(0,-1,0)" & Environment.NewLine &
                "@pokemon.hide" & Environment.NewLine &
                "@player.move(" & Steps & ")" & Environment.NewLine &
                "@pokemon.hide" & Environment.NewLine &
                ":end"

            CType(Core.CurrentScreen, OverworldScreen).ActionScript.StartScript(s, 2)
        End If
    End Sub

    Public Overrides Function WalkAgainstFunction() As Boolean
        If Me.ActionValue = 1 Then
            Return Me.Collision
        End If

        Dim p As Pokemon = ReturnWaterFallPokemonName()
        If Badge.CanUseHMMove(Badge.HMMoves.Waterfall) = True And Not p Is Nothing Or GameController.IS_DEBUG_ACTIVE = True Or Core.Player.SandBoxMode = True Then
            Dim s As String = ""

            Dim pName As String = ""
            Dim pNumber As Integer = 1
            If Not p Is Nothing Then
                pName = p.GetDisplayName()
                pNumber = p.Number
            End If

            Dim Steps As Integer = 1
            If Screen.Level.Surfing = False Then
                Steps = 0
            End If

            Dim checkPosition As New Vector3(Me.Position.X, Me.Position.Y + 1, Me.Position.Z)
            Dim foundSteps As Boolean = True
            While foundSteps = True
                Dim e As Entity = GetEntity(Screen.Level.Entities, checkPosition, True, {GetType(Waterfall)})
                If Not e Is Nothing Then
                    If e.EntityID = "Waterfall" Then
                        Steps += 1
                        checkPosition.Y += 1
                    Else
                        foundSteps = False
                    End If
                Else
                    foundSteps = False
                End If
            End While

            Screen.Camera.PlannedMovement = New Vector3(0, 1, 0)

			s = "version=2" & Environment.NewLine &
				"@player.stopmovement" & Environment.NewLine &
				"@pokemon.cry(" & pNumber & ")" & Environment.NewLine &
				"@sound.play(select)" & Environment.NewLine &
				"@text.show(" & pName & " used~Waterfall.)" & Environment.NewLine &
				"@player.move(" & Steps & ")" & Environment.NewLine &
				"@pokemon.hide" & Environment.NewLine &
				"@player.move(2)" & Environment.NewLine &
				"@pokemon.hide" & Environment.NewLine &
				":end"

			PlayerStatistics.Track("Waterfall used", 1)
            CType(Core.CurrentScreen, OverworldScreen).ActionScript.StartScript(s, 2)

            Return False
        End If

        If Me.Collision = True Then
            Return False
        Else
            Return True
        End If
    End Function

End Class