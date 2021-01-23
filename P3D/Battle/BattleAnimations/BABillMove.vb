Public Class BABillMove

    Inherits BattleAnimation3D

    Public TargetEntity As Entity
    Public Destination As Vector3
    Public MoveSpeed As Single
    Public SpinX As Boolean = False
    Public SpinZ As Boolean = False
    Public SpinSpeedX As Single = 0.1F
    Public SpinSpeedZ As Single = 0.1F
    Public MovementCurve As Integer = 0

    Public Sub New(ByRef entity As Entity, ByVal Destination As Vector3, ByVal Speed As Single, ByVal SpinX As Boolean, ByVal SpinZ As Boolean, ByVal startDelay As Single, ByVal endDelay As Single, Optional ByVal SpinXSpeed As Single = 0.1F, Optional ByVal SpinZSpeed As Single = 0.1F, Optional MovementCurve As Integer = 1)
        MyBase.New(New Vector3(0.0F), TextureManager.DefaultTexture, New Vector3(1.0F), startDelay, endDelay)

        Me.Destination = Destination
        Me.MoveSpeed = Speed
        Me.MovementCurve = MovementCurve

        Me.SpinX = SpinX
        Me.SpinZ = SpinZ
        Me.SpinSpeedX = SpinXSpeed
        Me.SpinSpeedZ = SpinZSpeed

        Me.Visible = False
        Me.TargetEntity = entity

        Me.AnimationType = AnimationTypes.BillMove
    End Sub

    Public Overrides Sub DoActionUpdate()
        Spin()
    End Sub

    Public Overrides Sub DoActionActive()
        Move()
    End Sub

    Private Sub Spin()
        If Me.SpinX = True Then
            Dim targetEntity = Me.TargetEntity
            targetEntity.Rotation.X += SpinSpeedX
        End If
        If Me.SpinZ = True Then
            TargetEntity.Rotation.Z += SpinSpeedZ
        End If
    End Sub

    Private Sub Move()
        Select Case MovementCurve
            Case 0
                If TargetEntity.Position.X < Me.Destination.X Then
                    TargetEntity.Position.X += Me.MoveSpeed

                    If TargetEntity.Position.X >= Me.Destination.X Then
                        TargetEntity.Position.X = Me.Destination.X
                    End If
                ElseIf TargetEntity.Position.X > Me.Destination.X Then
                    TargetEntity.Position.X -= Me.MoveSpeed

                    If TargetEntity.Position.X <= Me.Destination.X Then
                        TargetEntity.Position.X = Me.Destination.X
                    End If
                End If
                If TargetEntity.Position.Y < Me.Destination.Y Then
                    TargetEntity.Position.Y += Me.MoveSpeed

                    If TargetEntity.Position.Y >= Me.Destination.Y Then
                        TargetEntity.Position.Y = Me.Destination.Y
                    End If
                ElseIf TargetEntity.Position.Y > Me.Destination.Y Then
                    TargetEntity.Position.Y -= Me.MoveSpeed

                    If TargetEntity.Position.Y <= Me.Destination.Y Then
                        TargetEntity.Position.Y = Me.Destination.Y
                    End If
                End If
                If TargetEntity.Position.Z < Me.Destination.Z Then
                    TargetEntity.Position.Z += Me.MoveSpeed

                    If TargetEntity.Position.Z >= Me.Destination.Z Then
                        TargetEntity.Position.Z = Me.Destination.Z
                    End If
                ElseIf TargetEntity.Position.Z > Me.Destination.Z Then
                    TargetEntity.Position.Z -= Me.MoveSpeed

                    If TargetEntity.Position.Z <= Me.Destination.Z Then
                        TargetEntity.Position.Z = Me.Destination.Z
                    End If
                End If
            Case 1
                If TargetEntity.Position.X <> Me.Destination.X Then
                    TargetEntity.Position.X = MathHelper.Lerp(TargetEntity.Position.X, Me.Destination.X, Me.MoveSpeed)

                    If Math.Abs(TargetEntity.Position.X - Me.Destination.X) < 0.1F Then
                        TargetEntity.Position.X = Me.Destination.X
                    End If
                End If
                If TargetEntity.Position.Y <> Me.Destination.Y Then
                    TargetEntity.Position.X = MathHelper.Lerp(TargetEntity.Position.Y, Me.Destination.Y, Me.MoveSpeed)

                    If Math.Abs(TargetEntity.Position.Y - Me.Destination.Y) < 0.1F Then
                        TargetEntity.Position.Y = Me.Destination.Y
                    End If
                End If
                If TargetEntity.Position.Z < Me.Destination.Z Then
                    TargetEntity.Position.Z = MathHelper.Lerp(TargetEntity.Position.Z, Me.Destination.Z, Me.MoveSpeed)

                    If Math.Abs(TargetEntity.Position.Z - Me.Destination.Z) < 0.1F Then
                        TargetEntity.Position.Z = Me.Destination.Z
                    End If
                End If
        End Select

        If TargetEntity.Position = Destination Then
            Me.Ready = True
        End If
    End Sub

End Class