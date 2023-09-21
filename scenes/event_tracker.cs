using Godot;
using System;

public partial class event_tracker : Node2D
{
    /// <summary>
    /// This script holds ONLY the background code for "picking" a randomized event
    /// feel free to move code to a better place if needed :p
    /// </summary>

	public int eventType;

	public void GenerateEvent()
	{
        int _scrap = 0; //set temp int to 0
        int _food = 0; //set temp int to 0
        int _fuel = 0; //set temp int to 0
        int _hp = 0; //set temp int to 0

        GD.Print("Generating Event");
        eventType = GD.RandRange(0, 6); //random number between 0 and 5

		//EVENT KEY
		//0 = Nothing happens
		//1 = Raid
		//2 = Aid
		//3 = Extra Scrap
		//4 = Extra Food
		//5 = BONK

		switch (eventType)
		{
            case 0: //0 = Nothing happens
                GD.Print("Event = Nothing");
                break;
            
            case 1: //1 = Raid
            //AT LEAST 1 of EACH resource is removed from the ship inventory
                GD.Print("Event = Raid"); 

                //animation of a raid ship

                _scrap = GD.RandRange(1, 5); //random number between 1 and 4
                _food = GD.RandRange(1, 5); //random number between 1 and 4
                _fuel = GD.RandRange(1, 5); //random number between 1 and 4

                GetNode<Tracker>("root/Tracker").Scrap -= _scrap; //remove from ship inventory
                GetNode<Tracker>("root/Tracker").Food -= _food; //remove from ship inventory
                GetNode<Tracker>("root/Tracker").Fuel -= _fuel; //remove from ship inventory
            break;

            case 2: //2 = Aid
            //AT LEAST 2 of EACH resource is added to ship inventory
                GD.Print("Event = Aid");

                //animation of a helpful ship

                _scrap = GD.RandRange(2, 5); //random number between 2 and 4
                _food = GD.RandRange(2, 5); //random number between 2 and 4
                _fuel = GD.RandRange(2, 5); //random number between 2 and 4

                GetNode<Tracker>("root/Tracker").Scrap += _scrap; //add to ship inventory
                GetNode<Tracker>("root/Tracker").Food += _food; //add to ship inventory
                GetNode<Tracker>("root/Tracker").Fuel += _fuel; //add to ship inventory
            break;

            case 3: //3 = Extra Scrap
            //AT LEAST 2 scrap is added to ship inventory
                GD.Print("Event = Extra Scrap Found"); 
                _scrap = GD.RandRange(2, 5); //random number between 2 and 4
                GetNode<Tracker>("root/Tracker").Scrap += _food; //add to ship inventory
                break;

            case 4: //4 = Extra Food
             //AT LEAST 2 food is added to ship inventory
                GD.Print("Event = Extra Food Found");
                _food = GD.RandRange(2, 5); //random number between 2 and 4
                GetNode<Tracker>("root/Tracker").Food += _food; //add to ship inventory
                break;

            case 5: //5 = BONK
                GD.Print("Event = Hit . . . Something?");

            //play an animation of bumping into something(asteroid, space debris, garfield)

            _hp = GD.RandRange(2, 5); //random number between 2 and 4
                GetNode<Tracker>("root/Tracker").ShipHP -= _hp; //remove ship health
            break;
        }
    }
}
