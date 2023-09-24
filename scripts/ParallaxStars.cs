using Godot;
using System;

public partial class ParallaxStars : ParallaxBackground
{
    private Tracker Tracker;

    // Amount to scale the velocity of the stars based on the ship's velocity
    private float StarVelocityScale = 0.1F;

    private float StarOffset = 0.0F;

    public override void _Ready()
    {
        Tracker = GetNode<Tracker>("/root/Tracker");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Update the star velocity
        StarOffset += Tracker.Velocity * StarVelocityScale;
        if (StarOffset >= 1.0F)
        {
            // scroll the stars in the negative x direction, relative to the ship's velocity
            ScrollBaseOffset -= new Vector2((float)(int)(StarOffset), 0);
            StarOffset = 0.0F;
        }
    }

    // I made this just to be able to test the scrolling one frame at a time using a key.
    // public override void _UnhandledInput(InputEvent @event)
    // {
    //     if (@event is InputEventKey eventKey)
    //         if (eventKey.Pressed && eventKey.Keycode == Key.N)
    //             ScrollOffset -= new Vector2(1.0F, 0);
    // }
}
