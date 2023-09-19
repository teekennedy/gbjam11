using Godot;
using System;
using static Godot.GD;

public partial class SFR_Script : Node
{
    public Tracker Tracker;

    public Button SearchButton;
    public Button FixButton;
    public Button RestButton;
    public Button StatsButton;
    public Button ContinueButton;
    public RichTextLabel Choice;
    public Label TextLabel;
    public Timer TextTimer;

    // Called when the node enters the scene for the first time - START
    public override void _Ready()
    {
        Tracker = GetNode<Tracker>("/root/Tracker");


        SearchButton = GetNode<Button>("%SearchButton");
        FixButton = GetNode<Button>("%FixButton");
        RestButton = GetNode<Button>("%RestButton");
        StatsButton = GetNode<Button>("%StatsButton");
        ContinueButton = GetNode<Button>("%TravelButton");

        Choice = GetNode<RichTextLabel>("%ChoicePrompt");

        SwitchButton();
    }

    public void SwitchButton()
    { //put this here just so it doesn't run every frame
        StatsButton.GrabFocus(); //make stats button default starting button
        GD.Print("Switching Buttons");
        switch (Tracker.ButtonStat)
        { //switch based on button status
            case "Fix": //Button Status is Fix
                FixButton.Visible = true;
                GD.Print("Fix On");
                break;

            case "Search": //Button Status is Search
                SearchButton.Visible = true;
                GD.Print("Search On");
                break;

            case "Rest": //Button Status is Rest
                RestButton.Visible = true;
                GD.Print("Rest On");
                break;

            default:
                GD.Print("SFR button " + Tracker.ButtonStat);
                break;
        }
    }
}
