using Godot;
using System;

public partial class RetryGame : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        //button stuff
        var focusedIcon = GD.Load<CompressedTexture2D>("res://ui/focused.png");
        FocusEntered += () => Icon = focusedIcon;
        Pressed += () => Icon = focusedIcon;
        FocusExited += () => Icon = null;
    }

	public void ReloadGame()
    {
        GetTree().ReloadCurrentScene(); //reloads current scene
    }
}
