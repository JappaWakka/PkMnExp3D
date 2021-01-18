Namespace BattleSystem.Moves.Fire

    Public Class Ember

        Inherits Attack

        Public Sub New()
            '#Definitions
            Me.Type = New Element(Element.Types.Fire)
            Me.ID = 52
            Me.OriginalPP = 25
            Me.CurrentPP = 25
            Me.MaxPP = 25
            Me.Power = 40
            Me.Accuracy = 100
            Me.Category = Categories.Special
            Me.ContestCategory = ContestCategories.Cute
            Me.Name = "Ember"
            Me.Description = "The target is attacked with small flames. It may also leave the target with a burn."
            Me.CriticalChance = 1
            Me.IsHMMove = False
            Me.Target = Targets.OneAdjacentTarget
            Me.Priority = 0
            Me.TimesToAttack = 1
            '#End

            '#SpecialDefinitions
            Me.MakesContact = False
            Me.ProtectAffected = True
            Me.MagicCoatAffected = False
            Me.SnatchAffected = False
            Me.MirrorMoveAffected = True
            Me.KingsrockAffected = False
            Me.CounterAffected = False

            Me.DisabledWhileGravity = False
            Me.UseEffectiveness = True
            Me.ImmunityAffected = True
            Me.HasSecondaryEffect = True
            Me.RemovesFrozen = False

            Me.IsHealingMove = False
            Me.IsRecoilMove = False
            Me.IsPunchingMove = False
            Me.IsDamagingMove = True
            Me.IsProtectMove = False
            Me.IsSoundMove = False

            Me.IsAffectedBySubstitute = True
            Me.IsOneHitKOMove = False
            Me.IsWonderGuardAffected = True
            '#End
            Me.AIField1 = AIField.Damage
            Me.AIField2 = AIField.CanBurn

            EffectChances.Add(10)
        End Sub

        Public Overrides Sub MoveHits(own As Boolean, BattleScreen As BattleScreen)
            Dim p As Pokemon = BattleScreen.OwnPokemon
            Dim op As Pokemon = BattleScreen.OppPokemon
            If own = False Then
                p = BattleScreen.OppPokemon
                op = BattleScreen.OwnPokemon
            End If

            Dim chance As Integer = GetEffectChance(0, own, BattleScreen)
            If Core.Random.Next(0, 100) < chance Then
                BattleScreen.Battle.InflictBurn(Not own, own, BattleScreen, "", "move:ember")
            End If
        End Sub

        Public Overrides Sub InternalUserPokemonMoveAnimation(ByVal BattleScreen As BattleScreen, ByVal own As Boolean)
            Dim CurrentEntity As NPC
            Dim BAFlip As Boolean
            If own = True Then
                CurrentEntity = BattleScreen.OwnPokemonNPC
                BAFlip = False
            Else
                CurrentEntity = BattleScreen.OppPokemonNPC
                BAFlip = True
            End If
            Dim MoveAnimation As MoveAnimationQueryObject = New MoveAnimationQueryObject(CurrentEntity, BAFlip)
            MoveAnimation.AttacksSpawnMovingAnimation(0.0, 0.0, 0.0, "Textures\Battle\Fire\EmberBall", 0.2, 0.2, 0.2, 4.0, 0.0, 0.0, 0.036, False, True, 0.0, 0.0)
            For i = 0 To 12
                MoveAnimation.AttacksSpawnOpacityAnimation(CSng(i * 0.2), 0.0, 0.0, "Textures\Battle\Fire\Fire", 0.2, 0.2, 0.2, 0.01, False, 0.0, CSng(i * 0.6), 0.0)
                i += 1
            Next
            BattleScreen.BattleQuery.Add(MoveAnimation)
        End Sub

        Public Overrides Sub InternalOpponentPokemonMoveAnimation(ByVal BattleScreen As BattleScreen, ByVal own As Boolean)
            Dim CurrentEntity As NPC
            Dim BAFlip As Boolean
            If own = True Then
                CurrentEntity = BattleScreen.OppPokemonNPC
                BAFlip = True
            Else
                CurrentEntity = BattleScreen.OwnPokemonNPC
                BAFlip = False
            End If
            Dim MoveAnimation As MoveAnimationQueryObject = New MoveAnimationQueryObject(CurrentEntity, BAFlip)

            MoveAnimation.AttacksSpawnMovingAnimation(0.1, 0, 0.0, "Textures\Battle\Fire\Flame_1", 0.45, 0.45, 0.45, 0.1, 0, 0.0, 0.0, False, False, 1.0, 1.0)

            MoveAnimation.AttacksSpawnMovingAnimation(0.1, 0, 0.0, "Textures\Battle\Fire\Flame_2", 0.45, 0.45, 0.45, 0.1, 0, 0.0, 0.0, False, False, 2, 1)
            MoveAnimation.AttacksSpawnMovingAnimation(0.1, 0, 0.0, "Textures\Battle\Fire\Flame_3", 0.45, 0.45, 0.45, 0.1, 0, 0.0, 0.0, False, False, 3, 1)
            MoveAnimation.AttacksSpawnMovingAnimation(0.1, 0, 0.0, "Textures\Battle\Fire\Flame_4", 0.45, 0.45, 0.45, 0.1, 0, 0.0, 0.0, False, False, 4, 1)


            MoveAnimation.AttacksSpawnMovingAnimation(0.5, 0, 0.5, "Textures\Battle\Fire\Flame_1", 0.45, 0.45, 0.45, 0.5, 0, 0.5, 0.0, False, False, 1.0, 1.0)

            MoveAnimation.AttacksSpawnMovingAnimation(0.5, 0, 0.5, "Textures\Battle\Fire\Flame_2", 0.45, 0.45, 0.45, 0.5, 0, 0.5, 0.0, False, False, 2, 1)
            MoveAnimation.AttacksSpawnMovingAnimation(0.5, 0, 0.5, "Textures\Battle\Fire\Flame_3", 0.45, 0.45, 0.45, 0.5, 0, 0.5, 0.0, False, False, 3, 1)
            MoveAnimation.AttacksSpawnMovingAnimation(0.5, 0, 0.5, "Textures\Battle\Fire\Flame_4", 0.45, 0.45, 0.45, 0.5, 0, 0.5, 0.0, False, False, 4, 1)

            MoveAnimation.AttacksSpawnMovingAnimation(-0.5, 0, -0.5, "Textures\Battle\Fire\Flame_1", 0.45, 0.45, 0.45, -0.5, 0, -0.5, 0.0, False, False, 1.0, 1.0)

            MoveAnimation.AttacksSpawnMovingAnimation(-0.5, 0, -0.5, "Textures\Battle\Fire\Flame_2", 0.45, 0.45, 0.45, -0.5, 0, -0.5, 0.0, False, False, 2, 1)
            MoveAnimation.AttacksSpawnMovingAnimation(-0.5, 0, -0.5, "Textures\Battle\Fire\Flame_3", 0.45, 0.45, 0.45, -0.5, 0, -0.5, 0.0, False, False, 3, 1)
            MoveAnimation.AttacksSpawnMovingAnimation(-0.5, 0, -0.5, "Textures\Battle\Fire\Flame_4", 0.45, 0.45, 0.45, -0.5, 0, -0.5, 0.0, False, False, 4, 1)
            BattleScreen.BattleQuery.Add(MoveAnimation)
        End Sub
    End Class

End Namespace