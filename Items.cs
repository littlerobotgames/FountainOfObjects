using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FountainOfObjects
{
    public class Item
    {
        public string Name { get; set; } = "";
        public int Accuracy { get; set; }
        public int DamageMin { get; set; }
        public int DamageMax { get; set; }
        public int GetDamage()
        {
            Random r = new Random();
            return r.Next(DamageMin, DamageMax + 1);
        }
    }
    public class WeaponHand : Item
    {
        public WeaponHand()
        {
            Name = "Hand";
            Accuracy = 0;
            DamageMin = 1;
            DamageMax = 1;
        }
    }
    public class ItemDagger : Item
    {
        public ItemDagger()
        {
            Name = "Dagger";
            Accuracy = 10;
            DamageMin = 1;
            DamageMax = 3;
        }
    }
}
