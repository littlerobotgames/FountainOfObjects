using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FountainOfObjects
{
    public class Room
    {
        public bool Discovered { get; set; } = false;
        public Entity? entity { get; set; }

        public List<PlayerAction> EnterRoom()
        {
            Discovered = true;
            if (entity != null)
            {
                if (entity is Monster)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{entity.Name} is in the room");
                }
                
                return entity.actions;
            }
            return new List<PlayerAction>();
        }
    }
}
