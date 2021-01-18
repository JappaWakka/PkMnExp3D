Public Class BASound

    Inherits BattleAnimation3D

    Private startedsound As Boolean
    Private soundfile As String
    Private stopMusic As Boolean


    Public Sub New(ByVal sound As String, ByVal startDelay As Single, ByVal endDelay As Single, Optional ByVal stopMusic As Boolean = False)
        MyBase.New(New Vector3(0.0F), TextureManager.DefaultTexture, New Vector3(1.0F), startDelay, endDelay)

        Me.Scale = New Vector3(1.0F)
        startedsound = False
        soundfile = sound
        Me.Visible = False
        Me.stopMusic = stopMusic
        AnimationType = AnimationTypes.Sound
    End Sub

    Public Overrides Sub DoActionActive()
        If startedsound = False Or soundfile = "" Then
            SoundManager.PlaySound(soundfile, stopMusic)
            startedsound = True
            Me.Ready = True
        End If
    End Sub
End Class