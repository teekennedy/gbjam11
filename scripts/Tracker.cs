using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

// test change for Ky
public partial class Tracker : Control
{
    //reminder that doing this does put it in the editor, but wont change during play!
	[Export] public int Days = 0; //we start on day 0 so when player presses travel, we dont get asked "what happend to day 1?". just...trust me on this ~K
	[Export] public int Fuel = 10;
	[Export] public int Food = 5;
	[Export] public int Scrap = 5;
	[Export] public int ShipHP = 10;
	[Export] public int Distance = 0; //start at 0 as player has not moved any place yet


	//ok, so it turns out that global nodes are for keeping var's the same through scenes, so when we want to reset them we need to keep the og value
	//I know this is kinda inefficient, but it works to keep this script global
	private int DaysOG;
	private int FuelOG;
	private int FoodOG;
	private int ScrapOG;
	private int ShipHPOG;
	private int DistanceOG;

    // Button status used by SFR_Script to keep track of which button should be active.
    [Export] public string ButtonStat = "Search";


	// Scene for main menu
	private PackedScene _MenuScene;
	// Scene for Game Over screen
	private PackedScene _GameOverScene;
	// Scene for displaying a message
	private PackedScene _MessageScene;
	// Scene for the DayTracker popup
	private PackedScene _DayTrackerScene;
	// Base node of the game in gb_screen.tscn
	private Node _GameNode;
	// Node representing the current action (Menu, Travel, Stats, Fix, etc). Gets replaced by new action.
	private CanvasItem _ActionNode;
	// Node for the Day Tracker overlay
	private Control _DayTrackerNode;

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
		//setting the og values for tracker here so if we change the values in an editor or someplace later it wont mess up
		DaysOG = Days;
		FuelOG = Fuel;
		FoodOG = Food;
		ScrapOG = Scrap;
		ShipHPOG = ShipHP;
		DistanceOG = Distance;

		// Get the main Game node.
		// This only works if the Main scene is running.
		// Otherwise it will be null.
		_GameNode = GetNodeOrNull("/root/Main/SubViewportContainer/GBScreen/Game");
		// Get reference to Day Tracker node
		_DayTrackerNode = _GameNode?.GetNode<Control>("%DayTracker");

		// Load the scenes for the various actions
		_MenuScene = GD.Load<PackedScene>("res://scenes/Menu.tscn");
		_GameOverScene = GD.Load<PackedScene>("res://scenes/GameOver.tscn");
		_MessageScene = GD.Load<PackedScene>("res://scenes/Message.tscn");

		// Setup starting action
		OnMenuAction();
	}

	// Helper method to change the current UI action to a different scene
	public void SetUIAction(PackedScene scene)
	{
        // Remove existing node
        _ActionNode?.QueueFree();
		// Instantiate scene as new node
		_ActionNode = scene.Instantiate<CanvasItem>();
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

		//display distance traveled under gameover
		var Dis_Text = _ActionNode.GetNode<Label>("%GO_Text/Distance");
		Dis_Text.Text += "\n" + Distance + " x.z."; //x.z is not a real unit of mesurment, i just made it up

    }
	public bool CheckGameOver() //this is checked after travel is pressed, but before the main menu comes back
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

		if (ShipHP <= 0) //game over for ship exploding
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

        //Play fixing sound sound
        var FixSound = GetNodeOrNull<AudioStreamPlayer>("/root/Sounds/FixFX");
        FixSound?.Play();

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
		await GenerateEvent(); //run the generate event function
        //^ right now, this only picks a type of event and removes/adds resources. NEEDS ANIMATIONS pls <3

        //^^^ BUG: there's code in the GenerateEvent() function to change the text based on the events, but it doesn't appear on screen?
        //
        // (GenerateEvent() is at the bottom of this script)
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        // putting this here so the bug is easier to find, remove after bug is fixed :p

        await ShowMessage("Space. The final frontier.");
		Fuel -= 1; // Use some fuel
		Food -= 1; //consume food

        Days += 1; // Advance the day
		Distance += 1; //advance ship

        //MAKE SURE THIS HAPPENS AFTER WE CHANGE STATS!!!
        //to show us in the console what the stats are
        //just to help debug without bringing up stats menu
        GD.Print("Day " + Days);
        GD.Print("Fuel left " + Fuel);
        GD.Print("Food left " + Food);
        GD.Print("Scrap " + Scrap);
        GD.Print("Ship HP " + ShipHP);
		GD.Print("Distance traveld " + Distance);

        var FixChance = GD.RandRange(0, 11); ; //random number between 1 and 10 as the chance to fix broken ship

        if (FixChance <= 1)
        {//if FixChance is 0 or 1..
            ButtonStat = "Fix"; //Then switch the button status to Fix
        }
        else
        {//if FixChance is more than 1..
            ButtonStat = "Search"; //Then switch the button status to Search
        }

        await WaitMenus(); // display the current day

        OnMenuAction();
    }

	public void ReloadGame()
	{
        //reset tracker ints
        Days = DaysOG;
        Fuel = FuelOG;
        Food = FoodOG;
        Scrap = ScrapOG;
        ShipHP = ShipHPOG;
		Distance = DistanceOG;

        SetUIAction(_MenuScene); //instead of reloading the whole scene we will just change menus back to default

        GD.Print("Reset game to day " + Days);
        

    }

	public async Task WaitMenus() //for display of current day
	{
		if (_DayTrackerNode == null) {
			GD.Print("_DayTrackerNode is null, skip showing day");
			return;
		}
		// Make existing menu invisible if it exists
		if (_ActionNode != null) {
			_ActionNode.Visible = false;
		}
		// Set the text for the current day
		_DayTrackerNode.GetNode<Label>("%CurrentDay").Text = "DAY: " + Days;
		_DayTrackerNode.Visible = true;

        //Play next day sound
        var DaySound = GetNodeOrNull<AudioStreamPlayer>("/root/Sounds/DayCountFX");
        DaySound?.Play();

        // Get Timer for DayTracker
        Timer timer = _DayTrackerNode.GetNode<Timer>("%PopupTimer");

		timer.Start(); //start timer
		await ToSignal(timer, "timeout"); //wait a few seconds
		_DayTrackerNode.Visible = false;
		if (_ActionNode != null) {
			_ActionNode.Visible = true;
		}
	}

    public int eventType;

    public async Task GenerateEvent() //for selecting a random event
    {
        int _scrap = 0; //set temp int to 0
        int _food = 0; //set temp int to 0
        int _fuel = 0; //set temp int to 0
        int _hp = 0; //set temp int to 0

        GD.Print("Generating Event"); //debug
        eventType = GD.RandRange(0, 11); //random number between 0 and 11

        //EVENT KEY
        //0-5 = Nothing happens
        //6 = Raid
        //7 = Aid
        //8 = Extra Scrap
        //9 = Extra Food
        //10 = BONK

        switch (eventType)
        {
            case 6: //1 = Raid
                    //AT LEAST 1 of EACH resource is removed from the ship inventory
                GD.Print("Event = Raid");
                await ShowMessage("Oh No! A Raid Ship!"); //change text

                //animation of a raid ship

                _scrap = GD.RandRange(1, 5); //random number between 1 and 4
                _food = GD.RandRange(1, 5); //random number between 1 and 4
                _fuel = GD.RandRange(1, 5); //random number between 1 and 4

                Scrap -= _scrap; //remove from ship inventory
                Food -= _food; //remove from ship inventory
                Fuel -= _fuel; //remove from ship inventory

                await ShowMessage("Lost " + _scrap + " scrap, " + _food + " food and " + _fuel + " fuel!"); //change text

                break;

            case 7: //2 = Aid
                    //AT LEAST 2 of EACH resource is added to ship inventory
                GD.Print("Event = Aid");
                await ShowMessage("An Aiding Ship!"); //change text

                //animation of a helpful ship

                _scrap = GD.RandRange(2, 5); //random number between 2 and 4
                _food = GD.RandRange(2, 5); //random number between 2 and 4
                _fuel = GD.RandRange(2, 5); //random number between 2 and 4

                Scrap += _scrap; //add to ship inventory
                Food += _food; //add to ship inventory
                Fuel += _fuel; //add to ship inventory

                await ShowMessage("Gained " + _scrap + " scrap, " + _food + " food and " + _fuel + " fuel!"); //change text

                break;

            case 8: //3 = Extra Scrap
                    //AT LEAST 2 scrap is added to ship inventory
                GD.Print("Event = Extra Scrap Found");
                _scrap = GD.RandRange(2, 5); //random number between 2 and 4
                Scrap += _food; //add to ship inventory
                await ShowMessage("Found "+ _scrap + " extra scrap!"); //change text
                break;

            case 9: //4 = Extra Food
                    //AT LEAST 2 food is added to ship inventory
                GD.Print("Event = Extra Food Found");
                _food = GD.RandRange(2, 5); //random number between 2 and 4
                Food += _food; //add to ship inventory
                await ShowMessage("Found " + _food + " extra food!"); //change text
                break;

            case 10: //5 = BONK
                GD.Print("Event = BONK");
                await ShowMessage("We Hit . . . Something?"); //change text

                //play an animation of bumping into something(asteroid, space debris, garfield)

                _hp = GD.RandRange(2, 5); //random number between 2 and 4
                ShipHP -= _hp; //remove ship health
                break;
        }

		if (eventType < 6)
		{
            GD.Print("Event = Nothing");
            await ShowMessage("Safe Travels!"); //change text
        }
    }
}
