using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue_JRPG
{
    class Item
    {
        public Image icon;
        public ItemType thistype;
        public List<int> statsGiven;
        public Item(Image ic)
        {
            this.icon = ic;
        }
        public enum ItemType //тут понятно
        {
            Helmet,
            Armor,
            Weapon,
            OnceToUse
        }
    }
}
