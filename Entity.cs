using System.ComponentModel.Design;

namespace FountainOfObjects
{
    public class Entity
    {
        public string Name { get; set; } = "";
        
        public List<Item> items = new List<Item>();
        public List<PlayerAction> actions = new List<PlayerAction>();
        public void AddItem(Item item, bool log)
        {
            if (!ItemsHasItemType(item.GetType()))
            {
                items.Add(item);

                if (log)
                {
                    Program.AddLog(new LogLine($"{Name} obtained a {item.Name}", ConsoleColor.Blue));
                }
            }
        }
        public void RemoveItem(Item item)
        {
            items.Remove(item);
        }
        public bool ItemsHasItemType(Type type)
        {
            foreach(Item item in items)
            {
                if (item.GetType() == type)
                {
                    return true;
                }
            }

            return false;
        }
    }
    public class CombatEntity : Entity
    {
        public int MaxHealth = 0;
        public int Health { get; set; }
        public int Strength { get; set; }
        public int Accuracy { get; set; }
        public int Dodge { get; set; }
        public bool dead = false;
        public void Attack(CombatEntity target, Weapon weapon)
        {
            Random r = new Random();

            bool attackHit = r.Next(0, 100) <= Accuracy + weapon.Accuracy;
            bool attackDodged = r.Next(0, 100) <= target.Dodge;

            if (attackHit && !attackDodged)
            {
                target.TakeDamage(this, weapon.GetDamage() + Strength);
            }
            else
            {
                Program.AddLog(new LogLine($"{Name} missed their attack on {target.Name}", ConsoleColor.Gray));
            }
        }
        public void TakeDamage(CombatEntity attacker, int damage)
        {
            Health -= damage;

            Program.AddLog(new LogLine($"{attacker.Name} dealt {damage} damage to {Name}", ConsoleColor.Red));

            if (Health <= 0)
            {
                Die(attacker);
            }
        }
        public void GainHealth(int amount)
        {
            Health += amount;

            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }

            Program.AddLog(new LogLine($"{Name} gained {amount} Health", ConsoleColor.Green));
        }
        public void Die(CombatEntity attacker)
        {
            Program.AddLog(new LogLine($"{Name} died", ConsoleColor.DarkGreen));
            dead = true;

            foreach(Item item in items)
            {
                attacker.AddItem(item, true);
            }
            
        }
    }
    public class FountainOfObjects : Entity
    {
        public bool Enabled = false;
        public FountainOfObjects()
        {
            actions.Add(new PlayerAction("Enable Fountain", EnableFountain));
        }
        public void EnableFountain()
        {
            Enabled = true;
            actions.Clear();
        }
    }
}
