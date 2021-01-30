﻿Namespace BattleSystem.Moves.Normal

    Public Class Pound

        Inherits Attack

        Public Sub New()
            '#Definitions
            Me.Type = New Element(Element.Types.Normal)
            Me.ID = 1
            Me.OriginalPP = 25
            Me.CurrentPP = 25
            Me.MaxPP = 25
            Me.Power = 40
            Me.Accuracy = 100
            Me.Category = Categories.Physical
            Me.ContestCategory = ContestCategories.Tough
            Me.Name = "Pound"
            Me.Description = "The target is physically pounded with a long tail or a foreleg, etc."
            Me.CriticalChance = 1
            Me.IsHMMove = False
            Me.Target = Targets.OneAdjacentTarget
            Me.Priority = 0
            Me.TimesToAttack = 1
            '#End

            '#SpecialDefinitions
            Me.MakesContact = True
            Me.ProtectAffected = True
            Me.MagicCoatAffected = False
            Me.SnatchAffected = False
            Me.MirrorMoveAffected = True
            Me.KingsrockAffected = True
            Me.CounterAffected = True

            Me.DisabledWhileGravity = False
            Me.UseEffectiveness = True
            Me.ImmunityAffected = True
            Me.RemovesFrozen = False
            Me.HasSecondaryEffect = False

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
            Me.AIField2 = AIField.Nothing
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
            MoveAnimation.AnimationPlaySound("Battle\Attacks\Pound", 0.5, 2.5)
            MoveAnimation.AnimationSpawnFadingEntity(0, -0.25, 0, "Textures\Battle\Physical\Pound", 0.5, 0.5, 0.5, 0.02, False, 1.0, 0, 3)
            BattleScreen.BattleQuery.Add(MoveAnimation)
        End Sub

    End Class

End Namespace