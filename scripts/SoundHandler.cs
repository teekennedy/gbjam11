using Godot;
using System;

public partial class SoundHandler : Node
{
    public void Pressed()
    {
        //play selected sound
        var SelectFX = GetNode<AudioStreamPlayer>("/root/Tracker/SoundHolder/Selected");
        SelectFX.Play();
    }

    public void FocusEnter()
    {
        //play select sound when over button
        var ChangeFX = GetNode<AudioStreamPlayer>("/root/Tracker/SoundHolder/Change");
        ChangeFX.Play();
    }
}
