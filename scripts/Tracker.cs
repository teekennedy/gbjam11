using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public partial class Tracker : Control
{
    //reminder that doing this does put it in the editor, but wont change during play!
    [Export] public int Days = 1;
    [Export] public int Fuel = 3;
    [Export] public int Food = 5;
    [Export] public int Scrap = 5;
    [Export] public int ShipHP = 10;

    // Button status used by SFR_Script to keep track of which button should be active.
    [Export] public string ButtonStat = "Fix"; //set to "fix" by default just for testing


    // Scene for main menu
    private PackedScene _MenuScene;
    // Scene for Game Over screen
    private PackedScene _GameOverScene;
    // Scene for displaying a message
    private PackedScene _MessageScene;
    // Scene for the DayTracker popup
    private PackedScene _DayTrackerScene;
    private Node _GameNode;
    private Node _ActionNode;
    private Button RetryButton;

    // Event handler for when a button is pressed.
    public void OnButtonPressed(string node)
    {
        GD.Print("Button pressed: ", node);
        switch (node)
        {
            case "StatsButton":
                OnStatsAction();
                break;
            case "MenuButton":
                OnMenuAction();
                break;
            case "RestButton": // Rest button from Menu scene
                OnRestAction();
                break;
            case "RetryGameButton": // Retry button from Game Over scene.
                ReloadGame();
                break;
            case "SearchButton":
                OnSearchAction();
                break;
            case "TravelButton":
                OnTravelAction();
                break;
            case "FixButton":
                OnFixAction();
                break;
            default:
                GD.Print("Warning: unhandled button name ", node);
                break;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get the main Game node.
        // This only works if the Main scene is running.
        // Otherwise it will be null.
        _GameNode = GetNodeOrNull("/root/Main/SubViewportContainer/GBScreen/Game");

        // Load the scenes for the various actions
        _MenuScene = GD.Load<PackedScene>("res://scenes/Menu.tscn");
        _GameOverScene = GD.Load<PackedScene>("res://scenes/GameOver.tscn");
        _MessageScene = GD.Load<PackedScene>("res://scenes/Message.tscn");
        _DayTrackerScene = GD.Load<PackedScene>("res://scenes/DayTracker.tscn");

        // Setup starting action
        OnMenuAction();
    }

    // Helper method to change the current UI action to a different scene
    public void SetUIAction(PackedScene scene)
    {
        // Remove existing node
        _ActionNode?.QueueFree();
        // Instantiate scene as new node
        _ActionNode = scene.Instantiate<Node>();
        // Add new node as child to TrackerNode
        _GameNode?.AddChild(_ActionNode);
    }

    public SignalAwaiter ShowMessage(string text)
    {
        SetUIAction(_MessageScene);
        RichTextLabel message = _ActionNode.GetNode<RichTextLabel>("%Message");
        message.Text = text;
        return ToSignal(message, "MessageFinished");
    }

    public void OnGameOver(string reason)
    {
        SetUIAction(_GameOverScene);
        var GO_Text = _ActionNode.GetNode<Label>("%GO_Text");
        GO_Text.Text += "\n" + reason;
    }
    public bool CheckGameOver()
    {
        List<string> reasons = new();
        //(this is kinda a place holder for until we had the drifting in space part)
        if (Fuel <= 0) //game over for out of fuel
        {
            reasons.Add("no fuel");
            GD.Print("OUTA FUEL");
        }

        if (Food < -2) //made this LESS than 0 so player can try to make it 2 days without food
        {
            reasons.Add("starved");
            GD.Print("OUTA FOOD");
        }

        if (ShipHP <= 0)
        {
            reasons.Add("imploaded");
            GD.Print("OUTA HP");
        }
        if (reasons.Count > 0)
        {
            OnGameOver(Strings.Join(reasons.ToArray(), "\n"));
            return true;
        }
        return false;
    }

    public void OnMenuAction()
    {
        // Is it game over?
        // This automatically loads the GameOver scene if one of the game over conditions are met.
        if (CheckGameOver())
        {
            return;
        }

        SetUIAction(_MenuScene);
    }

    // the function for resting 
    public async void OnRestAction()
    {
        GD.Print("Resting . . . ");
        await ShowMessage("Resting . . . ");
        //play some kind of fade/rest animation

        Food -= 1; //remove 1 food from the inventory
        GD.Print("Food Left " + Food); //set text in debug log

        GD.Print("Day " + Days); //set text in debug log

        ButtonStat = "NULL"; //turn off Find/Search/Fix buttons
        Days += 1; // Advance the day

        await WaitMenus(); // display the current day

        OnMenuAction(); // Back to main menu
    }

    // the function for searching debris
    public async void OnSearchAction()
    {
        GD.Print("Searching . . . ");
        await ShowMessage("Searching . . . ");

        // play some kind of radar/scan animation

        var rn = GD.Randi() % 11; //random number between 0 and 10, jus to decide if loot is found or not
        if (rn <= 5)
        { //if the rng is less than 0...
            //..then nothing is found
            GD.Print("Nothing Was Found");
            await ShowMessage("Nothing Was Found!");
        }
        else
        { //if the rng is more than 0..
            //..then loot is found
            GD.Print("Loot Found!");
            await ShowMessage("Loot Found!");

            var fd = GD.RandRange(0, 11); //random number between 0 and 10 of Food
            var fl = GD.RandRange(0, 11); //random number between 0 and 10 of Fuel
            var sp = GD.RandRange(0, 6); //random number between 0 and 5 of Scrap

            if (fd == 0 && fl == 0 && sp == 0)
            { //if all three are 0.. 
                //then pick new numbers
                fd = GD.RandRange(0, 11); //random number between 0 and 10 of Food
                fl = GD.RandRange(0, 11); //random number between 0 and 10 of Fuel
                sp = GD.RandRange(0, 6); //random number between 0 and 5 of Scrap
            }

            Food += fd;
            Fuel += fl;
            Scrap += sp;

            GD.Print("Found " + fd + " Food, " + fl + " Fuel and " + sp + " Scrap!");
            await ShowMessage("Found " + fd + " Food, " + fl + " Fuel and " + sp + " Scrap!");
            //add loot to inventory

            GD.Print("Added loot to inventory!");
            await ShowMessage("Added loot to inventory!");
            GD.Print("Food: " + Food + ". Fuel: " + Fuel + ". Scrap: " + Scrap); //set text in debug log
        }
        ButtonStat = "Rest"; //Then switch the button status to Rest
        Days += 1; // Advance the day

        await WaitMenus(); // display the current day

        OnMenuAction(); // Back to main menu
    }

    public async void OnStatsAction()
    {
        string message = "Day: " + Days + "\t";
        message += "Food: " + Food + "\n";
        message += "Fuel: " + Fuel + "  ";
        message += "Scrap: " + Scrap + "\n";
        message += "Ship HP: " + ShipHP;

        await ShowMessage(message);
        OnMenuAction();
    }

    public async void OnFixAction()
    {
        GD.Print("Fixing Ship . . . "); //set debug log text

        await ShowMessage("Fixing ship . . .");
        //play fixing animation

        var hpADD = GD.RandRange(1, 6); ; //random number between 1 and 5 to be the hp added to the ship's hp

        Scrap -= 1; //remove 1 scrap from the inventory
        ShipHP += hpADD; //add that random value to the ship health

        GD.Print("Ship Fixed!"); //print "ship fixed" in the debug log
        await ShowMessage("Ship Fixed!"); //change text to say "ship fixed"
        GD.Print("Scrap Left " + Scrap); //set text in debug log

        ButtonStat = "Search"; //Then switch the button status to Search
        Days += 1; // Advance the day

        await WaitMenus(); // display the current day
        OnMenuAction();
    }

    public async void OnTravelAction()
    {
        //travel button press make this go and pick random
        var FixChance = GD.RandRange(0, 11); ; //random number between 1 and 10 as the chance to fix ship

        if (FixChance <= 1)
        {//if FixChance is 0 or 1..
            ButtonStat = "Fix"; //Then switch the button status to Fix
        }
        else
        {//if FixChance is more than 1..
            ButtonStat = "Search"; //Then switch the button status to Search
        }

        // TODO add more travel events choose one at random.
        await ShowMessage("Space. The final frontier.");
        Fuel -= 1; // Use some fuel
        Days += 1; // Advance the day

        await WaitMenus(); // display the current day

        OnMenuAction();
    }

    public void ReloadGame()
    {
        GetTree().ReloadCurrentScene(); //reloads current scene
        _Ready(); // re-initialize tracker
    }

    public async Task WaitMenus() //for display of current day
    {
        // Instantiate the DayTracker scene as a node
        var dayTrackerNode = _DayTrackerScene.Instantiate();
        // Set the text for the current day
        dayTrackerNode.GetNode<Label>("%CurrentDay").Text = "DAY: " + Days;
        // Add dayTrackerNode to the game
        _GameNode.AddChild(dayTrackerNode);

        // Get Timer for DayTracker
        Timer timer = dayTrackerNode.GetNode<Timer>("%PopupTimer");

        timer.Start(); //start timer
        await ToSignal(timer, "timeout"); //wait a few seconds

        // remove daytracker
        dayTrackerNode.QueueFree();
    }
}
