using Godot;
using System;

public partial class TitleScreen : Control
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("TitleScreen ready");
        GrabFocus();
        GrabClickFocus();
    }
}
