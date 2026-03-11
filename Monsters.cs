using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FountainOfObjects
{
    public class Monster : CombatEntity
    {
        public int Difficulty { get; set; }

        public Monster()
        {
            actions.Add(new PlayerAction("Fight", Fight));
            actions.Add(new PlayerAction("Attempt to Flee", Flee));
        }
        public void Fight()
        {

        }
        public void Flee()
        {

        }
    }
    public class Goblin : Monster
    {
        public Goblin()
        {
            Name = "Goblin";
            Health = 5;
            Accuracy = 35;
            Dodge = 30;
            Strength = 1;
            Difficulty = 1;
            AddItem(new ItemDagger());
        }
    }
}
