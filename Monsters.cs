using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FountainOfObjects
{
    public class Monster : CombatEntity
    {
        public bool FirstTurn = true;
        public int Difficulty { get; set; }

        public Monster()
        {
            actions.Add(new PlayerAction("Fight", Fight));
        }
        public void Fight()
        {
            Program.gameState = Program.GameState.Fighting;
        }
    }
    public class Goblin : Monster
    {
        public Goblin()
        {
            Name = "Goblin";
            Health = 5;
            MaxHealth = Health;
            Accuracy = 35;
            Dodge = 10;
            Strength = 1;
            Difficulty = 1;
            AddItem(new ItemDagger(), false);

            Random r = new Random();
            
            if (r.Next(4) == 0)
            {
                AddItem(new ConsumePotionSmall(), false);
            }
        }
    }
    public class Amarok : Monster
    {
        public Amarok()
        {
            Name = "Amarok";
            Health = 12;
            MaxHealth = Health;
            Accuracy = 15;
            Dodge = 12;
            Difficulty = 2;
            AddItem(new ItemGreatSword(), false);

            Random r = new Random();

            if (r.Next(4) == 0)
            {
                AddItem(new ConsumePotionMedium(), false);
            }
        }


    }
}
