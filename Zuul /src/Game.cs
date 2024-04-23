using System;
using System.Collections.Generic;

class Game
{
    // Private fields
    private Parser parser;
    private Player player;
    private bool keyUsed = false;

Item mercedes_c63s_amg = new Item(59995, 1, "mercedes_c63s_amg");
Item mercedes_g43_amg = new Item(92995, 1, "mercedes_g43_amg");
Item mercedes_c350e = new Item(37995, 1, "mercedes_c350e");
Item mercedes_brabus_800 = new Item(510995, 1, "mercedes_brabus_800");
Item cash = new Item(1000000, 1, "Cash");







    // Constructor
    public Game()
    {
        parser = new Parser();
        player = new Player();
        CreateRooms();
    }

    // Initialise the Rooms (and the Items)
    private void CreateRooms()
    {
        Room outside = new Room("Outside");
        Room entrance = new Room("Entrance");
        Room showroom1 = new Room("Showroom 1");
        Room showroom2 = new Room("Showroom 2");
        Room showroom3 = new Room("Showroom 3");
        Room showroom4 = new Room("Showroom 4");

        outside.AddExit("entrance", entrance);

        entrance.AddExit("outside", outside);
        entrance.AddExit("showroom1", showroom1);
        entrance.AddExit("showroom2", showroom2);
        entrance.AddExit("showroom3", showroom3);
        entrance.AddExit("showroom4", showroom4);

        showroom1.AddExit("entrance", entrance);
        showroom2.AddExit("entrance", entrance);
        showroom3.AddExit("entrance", entrance);
        showroom4.AddExit("entrance", entrance);

        showroom1.Chest.Put("mercedes_c63s_amg", mercedes_c63s_amg); // Place cash in showroom 1
        showroom2.Chest.Put("mercedes_g43_amg", mercedes_g43_amg); // Place cash in showroom 1
        showroom3.Chest.Put("mercedes_c350e", mercedes_c350e); // Place cash in showroom 1
        showroom4.Chest.Put("mercedes_brabus_800", mercedes_brabus_800); // Place cash in showroom 1
        entrance.Chest.Put("cash", cash); // Place cash in showroom 1

        player.CurrentRoom = outside;
    }

    // Main play routine. Loops until end of play.
    public void Play()
    {
        PrintWelcome();

        // Enter the main command loop. Here we repeatedly read commands and
        // execute them until the player wants to quit.
        bool finished = false;
        while (!finished)
        {
            Command command = parser.GetCommand();
            finished = ProcessCommand(command);
        }
        Console.WriteLine("Thank you for playing.");
        Console.WriteLine("Press [Enter] to continue.");
        Console.ReadLine();
    }

    // Print out the opening message for the player.
    private void PrintWelcome()
    {
        Console.WriteLine();
        Console.WriteLine("Welcome to Stefan's carshop");
        Console.WriteLine("We sell Mercedes!");
        Console.WriteLine("Type 'help' if you need help.");
        Console.WriteLine();
        Console.WriteLine(player.CurrentRoom.GetLongDescription(player));
    }

    // Given a command, process (that is: execute) the command.
    // If this command ends the game, it returns true.
    // Otherwise false is returned.
    private bool ProcessCommand(Command command)
    {
        bool wantToQuit = false;

        if (!player.IsAlive() && command.CommandWord != "quit")
        {
            Console.WriteLine("You bled out, you died...");
            Console.WriteLine("You can only use the command:");
            Console.WriteLine("quit");
            return wantToQuit;
        }

        if (keyUsed && command.CommandWord != "quit")
        {
            Console.WriteLine("You have won the game, the only allowed command is 'quit'.");
            return wantToQuit;
        }

        if (command.IsUnknown())
        {
            Console.WriteLine("I don't know what you mean...");
            return wantToQuit;
        }

        switch (command.CommandWord)
        {
            case "help":
                PrintHelp();
                break;
            case "look":
                Look();
                break;
            case "take":
                Take(command);
                break;
            case "drop":
                Drop(command);
                break;
            case "status":
                Health();
                break;
            case "go":
                GoRoom(command);
                break;
            case "use":
                UseItem(command, out keyUsed);
                break;
            case "quit":
                wantToQuit = true;
                break;
        }

        return wantToQuit;
    }

    // Print out some help information.
    // Here we print the mission and a list of the command words.
    private void PrintHelp()
    {
        Console.WriteLine("You are going to buy a car!");
        Console.WriteLine("Check out the cars and make sure to have some cash!");
        Console.WriteLine();
        // let the parser print the commands
        parser.PrintValidCommands();
    }

    private void Look()
    {
        Console.WriteLine(player.CurrentRoom.GetLongDescription(player));

        Dictionary<string, Item> roomItems = player.CurrentRoom.Chest.GetItems();
        if (roomItems.Count > 0)
        {
            Console.WriteLine("Items in this room:");
            foreach (var itemEntry in roomItems)
            {
                Console.WriteLine($"{itemEntry.Value.Description} - (${itemEntry.Value.Price} )");
            }
        }
    }

private void Take(Command command)
{
    if (!command.HasSecondWord())
    {
        Console.WriteLine("Take what?");
        return;
    }

    string itemName = command.SecondWord.ToLower();

    bool success = player.TakeFromChest(itemName);

    if (success && itemName == "cash") // Check if taken item is cash
    {
        Console.WriteLine("You picked up some cash.");
    }
    else if (success && (itemName == "mercedes_c63s_amg" || itemName == "mercedes_g43_amg" || itemName == "mercedes_c350e" || itemName == "mercedes_brabus_800")) // Check if taken item is any car
    {
        string displayName = GetCarDisplayName(itemName);
        Console.WriteLine($"You bought a {displayName}. Congratulations!");
        Console.WriteLine("Thank you for playing.");
        Console.WriteLine("Press [Enter] to continue.");
        Console.ReadLine();
        Environment.Exit(0); // Quit the game
    }
}

// Method to get the display name of the car
private string GetCarDisplayName(string internalName)
{
    switch (internalName.ToLower())
    {
        case "mercedes_c63s_amg":
            return "Mercedes C63s AMG";
        case "mercedes_g43_amg":
            return "Mercedes G43 AMG";
        case "mercedes_c350e":
            return "Mercedes C350e";
        case "mercedes_brabus_800":
            return "Mercedes BRABUS 800";
        default:
            return internalName;
    }
}



    private void Drop(Command command)
    {
        if (!command.HasSecondWord())
        {
            Console.WriteLine("Drop what?");
            return;
        }

        string itemName = command.SecondWord.ToLower();

        bool success = player.DropToChest(itemName);
    }

    private void Health()
    {
        Console.WriteLine($"Your health is: {player.GetHealth()}");

        Dictionary<string, Item> items = player.GetItems();

        if (items.Count > 0)
        {
            Console.WriteLine("Your current items:");

            // Iterate over each item in player's inventory
            foreach (var itemEntry in items)
            {
                Console.WriteLine($"- {itemEntry.Key}: Amount {itemEntry.Value.Price}");
            }
        }
        else
        {
            Console.WriteLine("You have no items in your inventory.");
        }
    }

    // Try to go to one direction. If there is an exit, enter the new
    // room, otherwise print an error message.
    private void GoRoom(Command command)
    {
        if (!command.HasSecondWord())
        {
            // if there is no second word, we don't know where to go...
            Console.WriteLine("Go where?");
            return;
        }

        string direction = command.SecondWord;

        // Try to go to the next room.
        Room nextRoom = player.CurrentRoom.GetExit(direction);
        if (nextRoom == null)
        {
            Console.WriteLine("There is no door to " + direction + "!");
            return;
        }

        player.Damage(0);
        player.CurrentRoom = nextRoom;
        Console.WriteLine(player.CurrentRoom.GetLongDescription(player));
        if (player.CurrentRoom.GetExit("door") != null)
        {
            Console.WriteLine("");
        }

        if (!player.IsAlive())
        {
            Console.WriteLine("");
        }
    }

    private void UseItem(Command command, out bool keyUsed)
    {
        if (!command.HasSecondWord())
        {
            Console.WriteLine("Use what?");
            keyUsed = false;
            return;
        }

        string itemName = command.SecondWord.ToLower();

        bool itemUsed = player.Use(itemName, out keyUsed);
    }
}
