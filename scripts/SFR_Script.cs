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

    [Export] public string ButtonStat;

    // Called when the node enters the scene for the first time - START
    public override void _Ready()
	{
        Tracker = GetNode<Tracker>("/root/Tracker");

        SearchButton = GetNode<Button>("/root/Tracker/MenuPanel/Choice/ActionsContainer/CenterContainer2/SearchButton");
        FixButton = GetNode<Button>("/root/Tracker/MenuPanel/Choice/ActionsContainer/CenterContainer2/FixButton");
        RestButton = GetNode<Button>("/root/Tracker/MenuPanel/Choice/ActionsContainer/CenterContainer2/RestButton");
        StatsButton = GetNode<Button>("/root/Tracker/MenuPanel/Choice/ActionsContainer/CenterContainer/StatsButton");
        ContinueButton = GetNode<Button>("/root/Tracker/MenuPanel/Choice/ActionsContainer/CenterContainer3/ContinueButton");
        Choice = GetNode<RichTextLabel>("/root/Tracker/MenuPanel/Choice/ChoicePrompt");
        TextLabel = GetNode<Label>("/root/Tracker/TextLabel");
        TextTimer = GetNode<Timer>("/root/Tracker/TextLabel/TextTimer");

        ButtonStat = "Fix"; //set to "fix" by default just for testing
        TextLabel.Visible = false;
        SwitchButton();
    }

	// Called every frame - UPDATE 
	public override void _Process(double delta)
    { }

	public void SwitchButton()
	{ //put this here just so it doesn't run every frame
        ButtonsOn();
        StatsButton.GrabFocus(); //make stats button default starting button
        Choice.Text = "What will you do?";
        TextLabel.Visible = false;
        GD.Print("Switching Buttons");
        switch (ButtonStat)
        { //switch based on button status
            case "NULL": //Button Status is NULL
                SearchButton.Visible = false;
                FixButton.Visible = false;
                RestButton.Visible = false;
                GD.Print("SFR BUTTONS NULL");
                break;

            case "Fix": //Button Status is Fix
                SearchButton.Visible = false;
                FixButton.Visible = true;
                RestButton.Visible = false;
                GD.Print("Fix On");               
                break;

            case "Search": //Button Status is Search
                SearchButton.Visible = true;
                FixButton.Visible = false;
                RestButton.Visible = false;
                GD.Print("Search On");
                break;

            case "Rest": //Button Status is Rest
                SearchButton.Visible = false;
                FixButton.Visible = false;
                RestButton.Visible = true;
                GD.Print("Rest On");
                break;
        }
    }

    public async void Fix()
    { //the function for fixing the ship
        ButtonsOff(); //turn off all buttons
        TextLabel.Visible = true; //turn on text
        GD.Print("Fixing Ship . . . "); //set debug log text
        TextLabel.Text = "Fixing Ship . . . "; //set in game text

        TextTimer.Start(); //start timer
        await ToSignal(TextTimer, "timeout"); //wait a few seconds
        //play fixing animation

        var hpADD = GD.RandRange(1, 6); ; //random number between 1 and 5 to be the hp added to the ship's hp

        Tracker.Scrap -= 1; //remove 1 scrap from the inventory
        Tracker.ShipHP += hpADD; //add that random value to the ship health

        GD.Print("Ship Fixed!"); //print "ship fixed" in the debug log
        TextLabel.Text = "Ship Fixed!"; //change text to say "ship fixed"
        GD.Print("Scrap Left " + Tracker.Scrap); //set text in debug log

        TextTimer.Start(); //start timer
        await ToSignal(TextTimer, "timeout"); //wait a few seconds

        Choice.Text = null; //reset the text on the ChoicePrompt
        ButtonStat = "Search"; //Then switch the button status to Search
        SwitchButton(); //run button switch
    }

    public async void Search()
    { //the function for searching debris
        ButtonsOff();
        TextLabel.Visible = true;
        GD.Print("Searching . . . ");
        TextLabel.Text = "Searching . . . ";

        TextTimer.Start(); //start timer
        await ToSignal(TextTimer, "timeout"); //wait a few seconds

        //play some kind of radar/scan animation

        var rn = GD.Randi() % 11; //random number between 0 and 10, jus to decide if loot is found or not
        if (rn <= 5)
        { //if the rng is less than 0...
            //..then nothing is found
            GD.Print("Nothing Was Found");
            TextLabel.Text = "Nothing Was Found!";
            TextTimer.Start(); //start timer
            await ToSignal(TextTimer, "timeout"); //wait a few seconds
        }
        else
        { //if the rng is more than 0..
            //..then loot is found
            GD.Print("Loot Found!");
            TextLabel.Text = "Loot Found!";

            TextTimer.Start(); //start timer
            await ToSignal(TextTimer, "timeout"); //wait a few seconds

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

            Tracker.Food += fd;
            Tracker.Fuel += fl;
            Tracker.Scrap += sp;

            GD.Print("Found " + fd + " Food, " + fl + " Fuel and " + sp + " Scrap!");
            TextLabel.Text = "Found " + fd + " Food, " + fl + " Fuel and " + sp + " Scrap!";
            //add loot to inventory

            TextTimer.Start(); //start timer
            await ToSignal(TextTimer, "timeout"); //wait a few seconds
            GD.Print("Added loot to inventory!");
            TextLabel.Text = "Added loot to inventory!";
            GD.Print("Food: " + Tracker.Food + ". Fuel: " + Tracker.Fuel + ". Scrap: " + Tracker.Scrap); //set text in debug log
        }
        TextTimer.Start(); //start timer
        await ToSignal(TextTimer, "timeout"); //wait a few seconds
        Choice.Text = null; //reset the text on the ChoicePrompt 
        ButtonStat = "Rest"; //Then switch the button status to Rest
        SwitchButton();
    }

    public async void Rest()
    { //the function for resting 
        ButtonsOff();
        TextLabel.Visible = true;
        GD.Print("Resting . . . ");
        TextLabel.Text = "Resting . . . ";
        //play some kind of fade/rest animation

        Tracker.Food -= 1; //remove 1 food from the inventory
        GD.Print("Food Left " + Tracker.Food); //set text in debug log
        Tracker.Days += 1; //add one day to the counter

        GD.Print("Day " + Tracker.Days); //set text in debug log

        Tracker.WaitMenus(); //displays the current day and waits

        Choice.Text = null;
        ButtonStat = "NULL"; //turn off Find/Search/Fix buttons
        SwitchButton(); 
    }

    public void Travel()
    {//travel button press make this go and pick random
        var FixChance = GD.RandRange(0, 11); ; //random number between 1 and 10 as the chance to fix ship

        if (FixChance <= 1)
        {//if FixChance is 0 or 1..
            ButtonStat = "Fix"; //Then switch the button status to Fix
            SwitchButton(); //run button switch
        }
        else 
        {//if FixChance is more than 1..
            ButtonStat = "Search"; //Then switch the button status to Search
            SwitchButton(); //run button switch
        }
    }

    public void ButtonsOff()
    { //turns off ALL the buttons
        Choice.Text = null;
        SearchButton.Visible = false;
        FixButton.Visible = false;
        RestButton.Visible = false;
        StatsButton.Visible = false;
        ContinueButton.Visible = false;
    }

    public void ButtonsOn()
    { //turns ON all the buttons
        Choice.Text = "What will you do?";
        SearchButton.Visible = true;
        FixButton.Visible = true;
        RestButton.Visible = true;
        StatsButton.Visible = true;
        ContinueButton.Visible = true;
        StatsButton.GrabFocus(); //make stats button default starting button
    }
}
