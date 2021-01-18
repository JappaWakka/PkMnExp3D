Public Class BABillMove

    Inherits BattleAnimation3D

    Public TargetEntity As Entity
    Public Destination As Vector3
    Public MoveSpeed As Single
    Public SpinX As Boolean = False
    Public SpinZ As Boolean = False
    Public SpinSpeedX As Single = 0.1F
    Public SpinSpeedZ As Single = 0.1F

    Public Sub New(ByRef entity As Entity, ByVal Destination As Vector3, ByVal Speed As Single, ByVal SpinX As Boolean, ByVal SpinZ As Boolean, ByVal startDelay As Single, ByVal endDelay As Single)
        MyBase.New(New Vector3(0.0F), TextureManager.DefaultTexture, New Vector3(1.0F), startDelay, endDelay)
        Me.Destination = Destination
        Me.MoveSpeed = Speed

        Me.SpinX = SpinX
        Me.SpinZ = SpinZ
        Me.SpinSpeedX = 0.1F
        Me.SpinSpeedZ = 0.1F
        Me.Visible = False
        Me.Destination = Destination
        Me.MoveSpeed = Speed
        Me.Scale = New Vector3(1.0F)
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

        If Me.Position = Destination Then
            Me.Ready = True
        End If
    End Sub

End Class