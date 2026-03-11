using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace FountainOfObjects
{
    class Program
    {
        public enum GameState { Playing, Won, Lost }
        public static GameState gameState = GameState.Playing;
        public enum Difficulty { Easy, Medium, Hard}
        public static Difficulty gameDifficulty = Difficulty.Easy;
        public static Cave cave;
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

            while (gameState == GameState.Playing)
            {
                UpdateScreen();
                Room room = cave.GetPlayerRoom();

                Console.ForegroundColor = ConsoleColor.White;
                
                actions.Clear();
                List<PlayerAction> roomActions = room.EnterRoom();

                //Add room actions
                for (int i = 0; i < roomActions.Count; i++)
                {
                    actions.Add(roomActions[i]);
                }
                if (room.entity != null)
                {
                    if (room.entity.GetType() != typeof(Monster))
                    {
                        //If there's no monster, allow the player to move
                        if (cave.PlayerY > 0) actions.Add(new PlayerAction("Move Up", () => cave.MovePlayer(0, -1)));
                        if (cave.PlayerX < caveHeight - 1) actions.Add(new PlayerAction("Move Right", () => cave.MovePlayer(1, 0)));
                        if (cave.PlayerY < caveHeight - 1) actions.Add(new PlayerAction("Move Down", () => cave.MovePlayer(0, 1)));
                        if (cave.PlayerX > 0) actions.Add(new PlayerAction("Move Left", () => cave.MovePlayer(-1, 0)));
                    }
                }
                else
                {
                    if (cave.PlayerY > 0) actions.Add(new PlayerAction("Move Up", () => cave.MovePlayer(0, -1)));
                    if (cave.PlayerX < caveHeight - 1) actions.Add(new PlayerAction("Move Right", () => cave.MovePlayer(1, 0)));
                    if (cave.PlayerY < caveHeight - 1) actions.Add(new PlayerAction("Move Down", () => cave.MovePlayer(0, 1)));
                    if (cave.PlayerX > 0) actions.Add(new PlayerAction("Move Left", () => cave.MovePlayer(-1, 0)));
                }

                    Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("What do you want to do?");
                for (int i = 0; i < actions.Count; i++)
                {
                    Console.WriteLine($"\t[{i}]: {actions[i].Name}");
                }

                int input = Convert.ToInt16(Console.ReadLine());

                actions[input].Action();
            }

            if (gameState == GameState.Won)
            {
                Console.WriteLine("Congratulations! You've won!");
            }
            else
            {
                Console.WriteLine("You've lost...");
            }

        }
        public static void UpdateScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Fountain of Objects");
            cave.UpdateCave();
        }
        public class Cave
        {
            public Room[,] rooms;
            public int PlayerX { get; set; } = 0;
            public int PlayerY { get; set; } = 0;
            public FountainOfObjects fountain = new FountainOfObjects();
            public Player player = new Player();

            public Cave(int width, int height)
            {
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
                int goblins = r.Next(2, 4) * difficultyMod;

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
            }
            public void UpdateCave()
            {
                for (int yy = 0; yy < rooms.GetLength(1); yy++)
                {

                    for (int xx = 0; xx < rooms.GetLength(0); xx++)
                    {
                        Room room = rooms[xx, yy];

                        Console.SetCursorPosition(xx * 5, (yy * 3) + 1);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
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
                                if (room.entity != null)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"  X  ");
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

                        Console.ForegroundColor = ConsoleColor.DarkGray;
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
        }
    }
}