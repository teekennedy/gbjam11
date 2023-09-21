using Godot;
using System;

public partial class Window : Node
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetWindow().Size = new Vector2I(160 * 4, 144 * 4);
    }
}
