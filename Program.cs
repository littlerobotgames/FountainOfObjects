using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace FountainOfObjects
{
    class Program
    {
        public enum GameState { Exploring, Fighting, Won, Lost }
        public static GameState gameState = GameState.Exploring;
        public enum Difficulty { Easy, Medium, Hard }
        public static Difficulty gameDifficulty = Difficulty.Easy;
        public static Cave cave;
        public static List<LogLine> gameLog = new List<LogLine>();
        public static void Main(string[] args)
        {
            int caveWidth = 4;
            int caveHeight = 4;
            int pits = 1;
            int maelstroms = 1;
            int amaroks = 1;

            Console.WriteLine("Select your difficulty:");
            Console.WriteLine("\t[0]: Easy");
            Console.WriteLine("\t[1]: Medium");
            Console.WriteLine("\t[2]: Hard");

            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 0:
                    gameDifficulty = Difficulty.Easy;
                    break;
                case 1:
                    caveWidth = 6;
                    caveHeight = 6;
                    gameDifficulty = Difficulty.Medium;
                    break;
                case 2:
                    caveWidth = 8;
                    caveHeight = 8;
                    gameDifficulty = Difficulty.Hard;
                    break;
            }

            cave = new Cave(caveWidth, caveHeight);

            List<PlayerAction> actions = new List<PlayerAction>();

            cave.Initialize();

            while (gameState == GameState.Exploring || gameState == GameState.Fighting)
            {
                UpdateScreen();
                Room room = cave.GetPlayerRoom();

                Console.ForegroundColor = ConsoleColor.White;
                
                actions.Clear();

                if (gameState == GameState.Exploring)
                {
                    List<PlayerAction> roomActions = room.EnterRoom();
                    //Add room actions
                    for (int i = 0; i < roomActions.Count; i++)
                    {
                        actions.Add(roomActions[i]);
                    }

                    if (room.entity != null)
                    {
                        if (room.entity is not Monster)
                        {
                            //If there's no monster, allow the player to move
                            AddPlayerBasicMovement(ref actions);
                        }
                        else
                        {
                            actions.Add(new PlayerAction($"Flee [{cave.player.Dodge}%]", cave.player.Flee));
                        }
                    }
                    else
                    {
                        AddPlayerBasicMovement(ref actions);
                    }
                }
                if (gameState == GameState.Fighting)
                {
                    Player player = cave.player;
                    Monster monster = (Monster)room.entity;
                    foreach (Item item in player.items)
                    {
                        if (item is Weapon)
                        {
                            Weapon weapon = (Weapon)item;
                            actions.Add(new PlayerAction(weapon.GetDescription(player, monster), () => player.Attack(monster, weapon)));
                        }
                        if (item is Consumable)
                        {
                            Consumable consumable = (Consumable)item;
                            actions.Add(new PlayerAction($"Use {consumable.GetDescription()}", () => consumable.Use(player)));
                        }
                    }

                    UpdateScreen();
                }

                UpdateLog();

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("What do you want to do?");
                for (int i = 0; i < actions.Count; i++)
                {
                    Console.WriteLine($"\t[{i}]: {actions[i].Name}");
                }

                int input = Convert.ToInt16(Console.ReadLine());

                actions[input].Action();

                cave.CheckForDead();

                if (gameState == GameState.Fighting)
                {
                    Monster monster = (Monster)room.entity;

                    if (monster.FirstTurn)
                    {
                        monster.FirstTurn = false;
                    }
                    else
                    {
                        monster.Attack(cave.player, (Weapon)monster.items[0]);

                        if (cave.player.Health <= 0)
                        {
                            gameState = GameState.Lost;
                        }
                    }
                        
                }
            }

            if (gameState == GameState.Won)
            {
                Console.WriteLine("Congratulations! You've escaped!");
            }
            else
            {
                Console.WriteLine("You've died...");
            }

        }
        public static void UpdateScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Fountain of Objects");
            cave.UpdateCave();

            if (gameState == GameState.Fighting)
            {
                Entity entity = cave.GetPlayerRoom().entity;

                if (entity != null)
                {
                    Monster monster = (Monster)entity;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"You: {cave.player.Health}\t\t\t{monster.Name}: {monster.Health}");
                }
            }
        }
        public static void UpdateLog()
        {
            (int ConsoleX, int ConsoleY) consolePosPre = Console.GetCursorPosition();

            int printX = (cave.CaveWidth * 5) + 5;
            int printY = 1;

            foreach(LogLine logLine in gameLog)
            {
                Console.SetCursorPosition(printX, printY);
                logLine.Print();
                printY++;
            }

            Console.SetCursorPosition(consolePosPre.ConsoleX, consolePosPre.ConsoleY);
        }
        public static void AddLog(LogLine logLine)
        {
            gameLog.Add(logLine);

            if (gameLog.Count > 10)
            {
                gameLog.RemoveAt(0);
            }
        }
        public static void AddPlayerBasicMovement(ref List<PlayerAction> actions)
        {
            if (cave.PlayerY > 0) actions.Add(new PlayerAction("Move Up", () => cave.MovePlayer(0, -1)));
            if (cave.PlayerX < cave.CaveHeight - 1) actions.Add(new PlayerAction("Move Right", () => cave.MovePlayer(1, 0)));
            if (cave.PlayerY < cave.CaveHeight - 1) actions.Add(new PlayerAction("Move Down", () => cave.MovePlayer(0, 1)));
            if (cave.PlayerX > 0) actions.Add(new PlayerAction("Move Left", () => cave.MovePlayer(-1, 0)));
        }
        public static void GameStateUpdate(GameState state)
        {
            gameState = state;
        }
        public class Cave
        {
            public Room[,] rooms;
            public int PlayerX { get; set; } = 0;
            public int PlayerY { get; set; } = 0;
            public FountainOfObjects fountain = new FountainOfObjects();
            public Player player = new Player();

            public int CaveWidth = 0;
            public int CaveHeight = 0;

            public Cave(int width, int height)
            {
                CaveWidth = width;
                CaveHeight = height;
                rooms = new Room[width, height];
            }
            public void Initialize()
            {
                for (int yy = 0; yy < rooms.GetLength(1); yy++)
                {
                    for (int xx = 0; xx < rooms.GetLength(0); xx++)
                    {
                        rooms[yy, xx] = new Room();
                    }
                }
                int difficultyMod = 1;
                Random r = new Random();

                switch (gameDifficulty)
                {
                    case Difficulty.Medium:
                        difficultyMod = 2;
                        break;
                    case Difficulty.Hard:
                        difficultyMod = 4;
                        break;
                }
                int goblins = r.Next(2 * difficultyMod, 4 * difficultyMod);
                int amaroks = difficultyMod;

                //Place Fountain room
                if (r.Next(2) == 0)
                {
                    int placeX = rooms.GetLength(0) - 1;
                    int placeY = r.Next(rooms.GetLength(1) - 1);
                    rooms[placeX, placeY].entity = fountain;
                }
                else
                {
                    int placeX = r.Next(rooms.GetLength(1) - 1);
                    int placeY = rooms.GetLength(0) - 1;
                    rooms[placeX, placeY].entity = fountain;
                }

                while(goblins > 0)
                {
                    int placeX = r.Next(1, rooms.GetLength(0));
                    int placeY = r.Next(1, rooms.GetLength(1));

                    while (rooms[placeX, placeY].entity != null)
                    {
                        placeX = r.Next(1, rooms.GetLength(0));
                        placeY = r.Next(1, rooms.GetLength(1));
                    }

                    rooms[placeX, placeY].entity = new Goblin();
                    goblins--;
                }
                while (amaroks > 0)
                {
                    int placeX = r.Next(1, rooms.GetLength(0));
                    int placeY = r.Next(1, rooms.GetLength(1));

                    while (rooms[placeX, placeY].entity != null)
                    {
                        placeX = r.Next(1, rooms.GetLength(0));
                        placeY = r.Next(1, rooms.GetLength(1));
                    }

                    rooms[placeX, placeY].entity = new Amarok();
                    amaroks--;
                }
            }
            public void UpdateCave()
            {
                for (int yy = 0; yy < rooms.GetLength(1); yy++)
                {

                    for (int xx = 0; xx < rooms.GetLength(0); xx++)
                    {
                        Room room = rooms[xx, yy];

                        Console.SetCursorPosition(xx * 5, (yy * 3) + 1);

                        if (xx == 0 && yy == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }
                        else if (room.Discovered)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        }
                            
                        Console.WriteLine("╔   ╗");

                        Console.SetCursorPosition(xx * 5, (yy * 3) + 2);
                        if (xx == PlayerX && yy == PlayerY)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("  O  ");
                        }
                        else
                        {
                            if (room.Discovered)
                            {
                                if (room.entity is FountainOfObjects)
                                {
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.WriteLine($"  F  ");
                                }
                                else
                                {
                                    Console.WriteLine($"     ");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"     ");
                            }
                        }
                        if (xx == 0 && yy == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }
                        else if (room.Discovered)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        }
                        Console.SetCursorPosition(xx * 5, (yy * 3) + 3);
                        Console.WriteLine("╚   ╝");
                    }
                }
            }
            public void MovePlayer(int x, int y)
            {
                PlayerX += x;
                PlayerY += y;

                if (PlayerX == 0 && PlayerY == 0)
                {
                    if (fountain.Enabled)
                    {
                        gameState = GameState.Won;
                    }
                }
            }
            public Room GetPlayerRoom()
            {
                return rooms[PlayerX, PlayerY];
            }
            public void CheckForDead()
            {
                Room room = GetPlayerRoom();
                
                if (room.entity != null)
                {
                    if (room.entity is Monster)
                    {
                        Monster entity = (Monster)room.entity;
                        
                        if (entity.dead)
                        {
                            room.entity = null;
                            gameState = GameState.Exploring;
                        }
                    }
                }
            }
        }
    }
}