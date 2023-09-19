using Godot;
using System;

public partial class Tracker : Control
{
    //reminder that doing this does put it in the editor, but wont change during play!
    //also these are the default values
    [Export] public int Days = 0;
    [Export] public int Fuel = 10;
    [Export] public int Food = 5;
    [Export] public int Scrap = 5;
    [Export] public int ShipHP = 10;

    private PanelContainer GO_Screen;
    private PackedScene Menu;
    private Node _ActionNode;
    private Button RetryButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        Menu = GD.Load<PackedScene>("res://scenes/Menu.tscn");
        //OnMenuAction();

        GO_Screen = GetNode<PanelContainer>("GameOverScreen");
        RetryButton = GetNode<Button>("GameOverScreen/RetryGame");

        //make stats button default starting button
        GetNode<Button>("MenuPanel/Choice/ActionsContainer/CenterContainer/StatsButton").GrabFocus();
    }

    public void OnMenuAction()
    {
        if (_ActionNode != null)
        {
            _ActionNode.QueueFree();
        }
        _ActionNode = Menu.Instantiate() as ActionControl;
        // GD.Print(_ActionNode);
        AddChild(_ActionNode);
    }

    public void OnGameOverAction()
    {
        if (_ActionNode != null)
        {
            _ActionNode.QueueFree();
            _ActionNode = null;
        }
        //toggle screen visabillity
        GO_Screen.Visible = true;
        RetryButton.GrabFocus(); //put focus on this button once screen is active
        GetNode<AudioStreamPlayer>("SoundHolder/GameOverFX").Play(); //play game over sound

    }

    public async void WaitMenus() //for display of current day
    {
        GetNode<Button>("MenuPanel/Choice/ActionsContainer/CenterContainer/StatsButton").GrabFocus();

        //this will display a prompt for a short time to show player what day it is
        Timer TextTime = GetNode<Timer>("DayCounter/DaysTextTimer");
        PanelContainer Menu = GetNode<PanelContainer>("MenuPanel");
        PanelContainer DayCount = GetNode<PanelContainer>("DayCounter");
        RichTextLabel DayText = GetNode<RichTextLabel>("DayCounter/CurrentDay");

        //display current day
        DayText.Text = ("DAY: " + Days);

        //switch menus
        DayCount.Visible = true;
        Menu.Visible = false;

        TextTime.Start(); //start timer
        await ToSignal(TextTime, "timeout"); //wait a few seconds

        //switch menus back
        DayCount.Visible = false;
        Menu.Visible = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        //grab game over text to show cause
        var GO_Text = GetNode<Label>("GameOverScreen/GO_Text");

        //(this is kinda a place holder for until we had the drifting in space part)
        if (Fuel <= 0) //game over for out of fuel
        {
            OnGameOverAction();
            GO_Text.Text = "game over \nno fuel";
            GD.Print("OUTA FUEL");
        }

        if (Food < -2) //made this LESS than 0 so player can try to make it 2 days without food
        {
            OnGameOverAction();
            GO_Text.Text = "game over \nstarved";
            GD.Print("OUTA FOOD");
        }

        if (ShipHP <= 0)
        {
            OnGameOverAction();
            GO_Text.Text = "game over \nimploaded";
            GD.Print("OUTA HP");
        }

    }
}
