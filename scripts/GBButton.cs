using Godot;
using System;

public partial class GBButton : Button
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var focusedIcon = GD.Load<CompressedTexture2D>("res://ui/focused.png");
        FocusEntered += () => Icon = focusedIcon;
        FocusExited += () => Icon = null;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
