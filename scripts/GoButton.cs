using Godot;
using System;

public partial class GoButton : Button
{
    public Tracker StatTracker;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        //button stuff
        var focusedIcon = GD.Load<CompressedTexture2D>("res://ui/focused.png");
        FocusEntered += () => Icon = focusedIcon;
        Pressed += () => Icon = focusedIcon;
        FocusExited += () => Icon = null;

        //this should find the node through the filepath and grab the script called Tracker.cs
        //CAUSE THIS NODE IS NOT A CHILD OF THE NODE THIS SCRIPT IS ON, FILE PATH THE ROOT!!!
        StatTracker = GetNode<Tracker>("/root/Tracker");

        if(StatTracker == null) { GD.Print("TRACKER NOT FOUND!"); }
        else { GD.Print("Tracker Found!"); }
    }

    public void OnButtonPressed()
    {

        //when the go button is pressed it will progress the day and use resourses
        StatTracker.Days += 1;
        StatTracker.Fuel -= 1;
        StatTracker.Food -= 1;

        //to show us in the console what the stats are
        GD.Print("Day " + StatTracker.Days);
        GD.Print("Fuel left " + StatTracker.Fuel);
        GD.Print("Food Count " + StatTracker.Food);
        GD.Print("Scrapt left " + StatTracker.Scrap);
        GD.Print("ShipHP " + StatTracker.ShipHP);

        //play menu prompts
        StatTracker.WaitMenus();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{

    }
}
