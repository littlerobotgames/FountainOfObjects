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
            Strength = 3;
            Accuracy = 50;
            Dodge = 25;
            AddItem(new WeaponHand());
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
