using Godot;
using System;

public partial class GBButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _EnterTree()
	{
		// Use an arrow icon to indicate focus
		var focusedIcon = GD.Load<CompressedTexture2D>("res://ui/focused.png");
		var focusedSound = GetNodeOrNull<AudioStreamPlayer>("/root/Sounds/ButtonFocused");
		var pressedSound = GetNodeOrNull<AudioStreamPlayer>("/root/Sounds/ButtonPressed");

		void onFocusEntered()
		{
			Icon = focusedIcon;
			focusedSound?.Play();
		};
		FocusEntered += onFocusEntered;
		FocusExited += () => Icon = null;

		Pressed += () => pressedSound?.Play();

		// Connect this button's pressed signal to Tracker
		var tracker = GetNode<Tracker>("/root/Tracker");
		void action() { tracker.OnButtonPressed(Name); }
		Connect(SignalName.Pressed, Callable.From(action));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _ExitTree()
	{
	}
}
