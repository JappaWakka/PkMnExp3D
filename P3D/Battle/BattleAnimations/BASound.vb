Public Class BASound

    Inherits BattleAnimation3D

    Private soundStarted As Boolean = False
    Private endTime As Date
    Private soundfile As String
    Private stopMusic As Boolean


    Public Sub New(ByVal sound As String, ByVal startDelay As Single, ByVal endDelay As Single, Optional ByVal stopMusic As Boolean = False)
        MyBase.New(New Vector3(0.0F), TextureManager.DefaultTexture, New Vector3(1.0F), startDelay, endDelay)
        Me.Scale = New Vector3(1.0F)
        soundfile = sound
        Me.Visible = False
        Me.stopMusic = stopMusic
        AnimationType = AnimationTypes.Sound
    End Sub

    Public Overrides Sub DoActionActive()
        If soundStarted = False Then
            endTime = Date.Now + SoundManager.GetSoundEffect(soundfile).Sound.Duration
            SoundManager.PlaySound(soundfile, stopMusic)
            soundStarted = True
        End If
        If soundStarted = True And Date.Now >= endTime Then
            Me.Ready = True
        End If
    End Sub
End Class