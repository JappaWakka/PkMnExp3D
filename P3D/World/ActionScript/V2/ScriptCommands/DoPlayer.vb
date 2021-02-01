Namespace ScriptVersion2

    Partial Class ScriptCommander

        ' --------------------------------------------------------------------------------------------------------------------------
        ' Contains the @player commands.
        ' --------------------------------------------------------------------------------------------------------------------------

        Private Shared Sub DoPlayer(ByVal subClass As String)
            Dim command As String = ScriptComparer.GetSubClassArgumentPair(subClass).Command
            Dim argument As String = ScriptComparer.GetSubClassArgumentPair(subClass).Argument

            Select Case command.ToLower()
                Case "receivepokedex"
                    Core.Player.HasPokedex = True
                    For Each p As Pokemon In Core.Player.Pokemons
                        Dim i As Integer = 2
                        If p.IsShiny = True Then
                            i = 3
                        End If
                        Core.Player.PokedexData = Pokedex.ChangeEntry(Core.Player.PokedexData, p.Number, i)
                    Next
                    IsReady = True
                Case "receivephone"
                    Core.Player.HasPhone = True
                    IsReady = True
                Case "renamerival"
					Dim RivalTexture As String = Core.Player.RivalSkin
					Dim RivalName As String
					RivalName = "???"
					Core.SetScreen(New NameObjectScreen(Core.CurrentScreen, TextureManager.GetTexture(RivalTexture, New Rectangle(0, 64, 32, 32)), False, False, RivalName, "???", AddressOf Script.NameRival))
					IsReady = True
					CanContinue = False
				Case "setrivalskin"
					Core.Player.RivalSkin = argument
					IsReady = True
				Case "setrivalname"
					Core.Player.RivalName = argument
					IsReady = True
				Case "wearskin"
                    With Screen.Level.OwnPlayer
                        Dim TextureID As String = argument
                        .SetTexture(TextureID, False, True)

                        .UpdateEntity()
                    End With
                    IsReady = True
                Case "move"
                    If Started = False Then
                        If OverworldCamera._thirdPerson = False Then
                            Screen.Camera.PlayerFacing = Screen.Camera.GetPlayerFacingDirection()
                        End If
                        Screen.Camera.Move(sng(argument))
                        Started = True
                        Screen.Level.OverworldPokemon.Visible = False
                    Else
                        Screen.Level.UpdateEntities()
                        Screen.Camera.Update()
                        If Screen.Camera.IsMoving() = False Then
                            Screen.Level.OwnPlayer.AnimationX = 1
                            IsReady = True
                            Screen.Level.OverworldPokemon.Visible = False
                        End If
                    End If
                Case "moveasync"
                    If OverworldCamera._thirdPerson = False Then
                        Screen.Camera.PlayerFacing = Screen.Camera.GetPlayerFacingDirection()
                    End If
                    Screen.Camera.Move(sng(argument))
                    IsReady = True
                    If Screen.Camera.IsMoving() = False Then
                        Screen.Level.OwnPlayer.AnimationX = 1
                    End If
                    Screen.Level.OverworldPokemon.Visible = False
                Case "turn"
                    If Started = False Then
                        Screen.Camera.Turn(int(argument))
                        Started = True
                        Screen.Level.OverworldPokemon.Visible = False
                    Else
                        Screen.Camera.Update()
                        Screen.Level.UpdateEntities()
                        If Screen.Camera.Turning = False Then
                            IsReady = True
                        End If
                    End If
                Case "dance"
                    Screen.Level.OwnPlayer().isDancing = True
                    If Started = False Then
                        Screen.Camera.Move(sng(argument))
                        Started = True
                        Screen.Level.OverworldPokemon.Visible = False
                    Else
                        Screen.Level.UpdateEntities()
                        Screen.Camera.Update()
                        If Screen.Camera.IsMoving() = False Then
                            IsReady = True
                            Screen.Level.OverworldPokemon.Visible = False
                        End If
                    End If
                Case "turnasync"
                    Screen.Camera.Turn(int(argument))
                    IsReady = True
                    Screen.Level.OverworldPokemon.Visible = False
                Case "turntoasync"
                    Dim turns As Integer = int(argument) - Screen.Camera.GetPlayerFacingDirection()
                    If turns < 0 Then
                        turns = turns + 4
                    End If

                    If turns > 0 Then
                        Screen.Camera.Turn(turns)
                        Started = True
                        Screen.Level.OverworldPokemon.Visible = False
                    End If

                    IsReady = True
                Case "turnto"
                    If Started = False Then
                        If OverworldCamera._thirdPerson = False Then
                            Screen.Camera.PlayerFacing = Screen.Camera.GetPlayerFacingDirection()
                        End If
                        Dim turns As Integer = int(argument) - Screen.Camera.PlayerFacing
                        If turns < 0 Then
                            turns = turns + 4
                        End If

                        If turns > 0 Then
                            Screen.Camera.Turn(turns)
                            Started = True
                            Screen.Level.OverworldPokemon.Visible = False
                        Else
                            IsReady = True
                        End If
                    Else
                        Screen.Camera.Update()
                        Screen.Level.UpdateEntities()
                        If Screen.Camera.Turning = False Then
                            IsReady = True
                        End If
                    End If
                Case "warp"
                    Dim commas As Integer = 0
                    For Each c As Char In argument
                        If c = "," Then
                            commas += 1
                        End If
                    Next

                    Dim cPosition As Vector3 = Screen.Camera.Position

					Select Case commas
						Case 5
							Screen.Level.WarpData.WarpDestination = argument.GetSplit(0)
							Screen.Level.WarpData.WarpPosition = New Vector3(sng(argument.GetSplit(1).Replace("~", CStr(cPosition.X)).Replace(".", GameController.DecSeparator)),
																		  sng(argument.GetSplit(2).Replace("~", CStr(cPosition.Y)).Replace(".", GameController.DecSeparator)),
																		  sng(argument.GetSplit(3).Replace("~", CStr(cPosition.Z)).Replace(".", GameController.DecSeparator)))
							Screen.Level.WarpData.WarpRotations = int(argument.GetSplit(4))
							Dim WarpSoundData As Integer = CInt(argument.GetSplit(5))
							Select Case WarpSoundData
								Case 0
									Screen.Level.WarpData.WarpSound = "Warp_Exit"
								Case 1
									Screen.Level.WarpData.WarpSound = "Warp_RegularDoor"
								Case 2
									Screen.Level.WarpData.WarpSound = "Warp_SlideDoor"
								Case 3
									Screen.Level.WarpData.WarpSound = ""
								Case Else
									Screen.Level.WarpData.WarpSound = "Exit"
							End Select
							Screen.Level.WarpData.DoWarpInNextTick = True
							Screen.Level.WarpData.CorrectCameraYaw = Screen.Camera.Yaw
						Case 4
							Screen.Level.WarpData.WarpDestination = argument.GetSplit(0)
							Screen.Level.WarpData.WarpPosition = New Vector3(sng(argument.GetSplit(1).Replace("~", CStr(cPosition.X)).Replace(".", GameController.DecSeparator)),
																		  sng(argument.GetSplit(2).Replace("~", CStr(cPosition.Y)).Replace(".", GameController.DecSeparator)),
																		  sng(argument.GetSplit(3).Replace("~", CStr(cPosition.Z)).Replace(".", GameController.DecSeparator)))
							Screen.Level.WarpData.WarpRotations = int(argument.GetSplit(4))
							Screen.Level.WarpData.WarpSound = "Warp_Exit"
							Screen.Level.WarpData.DoWarpInNextTick = True
							Screen.Level.WarpData.CorrectCameraYaw = Screen.Camera.Yaw
						Case 3
							Screen.Level.WarpData.WarpDestination = argument.GetSplit(0)
							Screen.Level.WarpData.WarpPosition = New Vector3(sng(argument.GetSplit(1).Replace("~", CStr(cPosition.X)).Replace(".", GameController.DecSeparator)),
																		  sng(argument.GetSplit(2).Replace("~", CStr(cPosition.Y)).Replace(".", GameController.DecSeparator)),
																		  sng(argument.GetSplit(3).Replace("~", CStr(cPosition.Z)).Replace(".", GameController.DecSeparator)))
							Screen.Level.WarpData.WarpRotations = 0
							Screen.Level.WarpData.DoWarpInNextTick = True
							Screen.Level.WarpData.CorrectCameraYaw = Screen.Camera.Yaw
						Case 2
							Screen.Camera.Position = New Vector3(sng(argument.GetSplit(0).Replace("~", CStr(cPosition.X)).Replace(".", GameController.DecSeparator)),
																 sng(argument.GetSplit(1).Replace("~", CStr(cPosition.Y)).Replace(".", GameController.DecSeparator)),
																 sng(argument.GetSplit(2).Replace("~", CStr(cPosition.Z)).Replace(".", GameController.DecSeparator)))
						Case 0
							Screen.Level.WarpData.WarpDestination = argument
							Screen.Level.WarpData.WarpPosition = Screen.Camera.Position
							Screen.Level.WarpData.WarpRotations = 0
							Screen.Level.WarpData.DoWarpInNextTick = True
							Screen.Level.WarpData.CorrectCameraYaw = Screen.Camera.Yaw
					End Select

					Screen.Level.OverworldPokemon.warped = True
                    Screen.Level.OverworldPokemon.Visible = False

                    IsReady = True
                Case "stopmovement"
                    Screen.Camera.StopMovement()

                    IsReady = True
                Case "money", "addmoney"
                    Core.Player.Money += int(argument)

                    IsReady = True
                Case "setmovement"
                    Dim movements() As String = argument.Split(CChar(","))

                    Screen.Camera.PlannedMovement = New Vector3(int(movements(0)),
                                                                sng(movements(1)),
                                                                int(movements(2)))
                    IsReady = True
                Case "resetmovement"
                    Screen.Camera.PlannedMovement = Vector3.Zero

                    IsReady = True
                Case "setspeed"
                    CType(Screen.Camera, OverworldCamera).CameraSpeed = sng(argument.Replace(".", GameController.DecSeparator)) * 0.04F
                    IsReady = True
                Case "resetspeed"
                    CType(Screen.Camera, OverworldCamera).CameraSpeed = 0.04F
                    IsReady = True
                Case "getbadge"
                    If StringHelper.IsNumeric(argument) Then
                        If Core.Player.Badges.Contains(int(argument)) = False Then
                            Core.Player.Badges.Add(int(argument))
                            SoundManager.PlaySound("badge_acquired", True)
                            Screen.TextBox.TextColor = TextBox.PlayerColor
                            Screen.TextBox.Show(Core.Player.Name & " received the~" & Badge.GetBadgeName(int(argument)) & " Badge.", {}, False, False)

                            Core.Player.AddPoints(10, "Got a badge.")
                        End If
                    End If

                    IsReady = True

                    CanContinue = False
                Case "removebadge"
                    If StringHelper.IsNumeric(argument) Then
                        If Core.Player.Badges.Contains(int(argument)) = True Then
                            Core.Player.Badges.Remove(int(argument))
                        End If
                    End If

                    IsReady = True
                Case "addbadge"
                    If StringHelper.IsNumeric(argument) Then
                        If Core.Player.Badges.Contains(int(argument)) = False Then
                            Core.Player.Badges.Add(int(argument))
                        End If
                    End If

                    IsReady = True
                Case "achieveemblem"
                    GameJolt.Emblem.AchieveEmblem(argument)

                    IsReady = True
                Case "addbp"
                    Dim bp As Integer = int(argument)

                    Core.Player.BP += bp

                    If bp > 0 Then
                        PlayerStatistics.Track("Obtained BP", bp)
                    End If

                    IsReady = True
                Case "showrod"
                    If Core.CurrentScreen.Identification = Screen.Identifications.OverworldScreen Then
                        OverworldScreen.DrawRodID = int(argument)
                    End If

                    IsReady = True
                Case "hiderod"
                    OverworldScreen.DrawRodID = -1

                    IsReady = True
                Case "showpokemonfollow"
                    Screen.Level.OverworldPokemon.Visible = True

                    IsReady = True

                Case "hidepokemonfollow"
                    Screen.Level.OverworldPokemon.Visible = False

                    IsReady = True

                Case "togglepokemonfollow"
                    Screen.Level.OverworldPokemon.Visible = Not Screen.Level.OverworldPokemon.Visible

                    IsReady = True
                Case "save"
                    Core.Player.SaveGame(False)

                    IsReady = True
                Case "setname"
                    Core.Player.Name = argument
                    IsReady = True
                Case "setgender"
                    Select Case argument
                        Case "0", "Male", "male"
                            Core.Player.Gender = "Male"
                        Case "1", "Female", "female"
                            Core.Player.Gender = "Female"
                        Case Else
                            Core.Player.Gender = "Other"
                    End Select
					IsReady = True
				Case "setopacity"
                    Dim newOpacity As Single = sng(argument.Replace("~", Screen.Level.OwnPlayer.Opacity.ToString().Replace(".", GameController.DecSeparator)))
                    Screen.Level.OwnPlayer.Opacity = newOpacity
                    IsReady = True
                Case Else
                    IsReady = True
            End Select
        End Sub

    End Class

End Namespace