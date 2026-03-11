namespace FountainOfObjects
{
    public class Entity
    {
        public string Name { get; set; } = "";
        
        public List<Item> items = new List<Item>();
        public List<PlayerAction> actions = new List<PlayerAction>();
        public void AddItem(Item item)
        {
            items.Add(item);
        }
        public void RemoveItem(Item item)
        {
            items.Remove(item);
        }
    }
    public class CombatEntity : Entity
    {
        public int Health { get; set; }
        public int Strength { get; set; }
        public int Accuracy { get; set; }
        public int Dodge { get; set; }
        public bool dead = false;
        public void Attack(CombatEntity target, Item weapon)
        {
            Random r = new Random();

            bool attackHit = r.Next(0, 100) <= Accuracy + weapon.Accuracy;
            bool attackDodged = r.Next(0, 100) <= target.Dodge;

            if (attackHit && !attackDodged)
            {
                target.TakeDamage(this, weapon.GetDamage() + Strength);
            }
        }
        public void TakeDamage(CombatEntity attacker, int damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                Die(attacker);
            }
        }
        public void Die(CombatEntity attacker)
        {
            dead = true;
            attacker.AddItem(items[0]);
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
