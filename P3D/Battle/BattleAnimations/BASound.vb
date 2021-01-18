Public Class BASound

    Inherits BattleAnimation3D

    Private startedsound As Boolean
    Private soundfile As String


    Public Sub New(ByVal sound As String, ByVal startDelay As Single, ByVal endDelay As Single)
        MyBase.New(New Vector3(0.0F), TextureManager.DefaultTexture, New Vector3(1.0F), startDelay, endDelay)

        Me.Scale = New Vector3(1.0F)
        startedsound = False
        soundfile = sound
        Visible = False
        AnimationType = AnimationTypes.Sound
    End Sub

    Public Overrides Sub DoActionActive()
        If startedsound OrElse Not soundfile = "" Then Return
        SoundManager.PlaySound(soundfile, True)
        startedsound = True
        Ready = True
    End Sub
End Class