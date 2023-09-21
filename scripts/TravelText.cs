using Godot;
using System;

public partial class TravelText : RichTextLabel
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Text = "Space is chocked full of nothing...";
		var tracker = GetNode<Tracker>("/root/Tracker");
		tracker.Fuel -= 1;
	}
}
