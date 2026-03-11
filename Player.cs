using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FountainOfObjects
{
    public class Player : CombatEntity
    {
        public Player()
        {
            Name = "Player";
            Health = 20;
            MaxHealth = Health;
            Strength = 3;
            Accuracy = 50;
            Dodge = 25;
            AddItem(new WeaponHand(), false);
        }
        public void Flee()
        {
            Random r = new Random();
            if (r.Next(100) <= Dodge)
            {
                Monster monster = Program.cave.GetPlayerRoom().entity as Monster;
                monster.dead = true;

                Program.AddLog(new LogLine($"You were able to flee from {monster.Name}", ConsoleColor.Cyan));
            }
            else
            {
                Program.gameState = Program.GameState.Fighting;
            }
        }
    }
    public class PlayerAction
    {
        public string Name { get; set; } = "";
        public Action Action { get; set; }
        public PlayerAction(string name, Action action)
        {
            Name = name;
            Action = action;
        }
    }
}
