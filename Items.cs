using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FountainOfObjects
{
    public class Item
    {
        public string Name { get; set; } = "";

        public virtual string GetDescription() { return ""; }
    }
    public class Weapon : Item
    {
        public int Accuracy { get; set; }
        public int DamageMin { get; set; }
        public int DamageMax { get; set; }
        public int GetDamage()
        {
            Random r = new Random();
            return r.Next(DamageMin, DamageMax + 1);
        }
        public string GetDescription(CombatEntity attacker, CombatEntity defender)
        {
            return $"{Name} [{(int)(((((double)attacker.Accuracy + Accuracy) / 100) * ((100 - (double)defender.Dodge) / 100)) * 100)}% to hit] {DamageMin + attacker.Strength} - {DamageMax + attacker.Strength} damage";
        }
    }
    public class Consumable : Item
    {
        public virtual void Use(CombatEntity entity) { }
    }
    public class WeaponHand : Weapon
    {
        public WeaponHand()
        {
            Name = "Hand";
            Accuracy = 0;
            DamageMin = 1;
            DamageMax = 1;
        }
    }
    public class ItemDagger : Weapon
    {
        public ItemDagger()
        {
            Name = "Dagger";
            Accuracy = 10;
            DamageMin = 1;
            DamageMax = 3;
        }
    }
    public class ItemGreatSword : Weapon
    {
        public ItemGreatSword()
        {
            Name = "Greatsword";
            Accuracy = -10;
            DamageMin = 3;
            DamageMax = 8;
        }
    }
    public class ConsumePotionSmall : Consumable
    {
        public int healMin = 2;
        public int healMax = 4;
        public ConsumePotionSmall()
        {
            Name = "Small Potion";
        }
        public override void Use(CombatEntity entity)
        {
            Random r = new Random();
            entity.GainHealth(r.Next(healMin, healMax + 1));
            entity.items.Remove(this);
        }
        public override string GetDescription()
        {
            return $"{Name} (Gain {healMin} - {healMax} Health)";
        }
    }
    public class ConsumePotionMedium : Consumable
    {
        public int healMin = 4;
        public int healMax = 8;
        public ConsumePotionMedium()
        {
            Name = "Medium Potion";
        }
        public override void Use(CombatEntity entity)
        {
            Random r = new Random();
            entity.GainHealth(r.Next(healMin, healMax + 1));
            entity.items.Remove(this);
        }
        public override string GetDescription()
        {
            return $"{Name} (Gain {healMin} - {healMax} Health)";
        }
    }
}
