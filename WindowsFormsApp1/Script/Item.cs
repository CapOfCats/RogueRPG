using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue_JRPG
{
    /// <summary>
    /// Сущность, представляющая предмет
    /// </summary>
    class Item
    {
        #region Поля
        /// <summary>
        /// Иконка предмета
        /// </summary>
        public Image icon;
        /// <summary>
        /// Тип предмета
        /// </summary>
        public ItemType thistype;
        /// <summary>
        /// То,какие бонусы даёт предмет
        /// </summary>
        public List<int> statsGiven;
        #endregion

        #region Конструкторы
        public Item(Image ic)
        {
            this.icon = ic;
        }
        public Item() { }
        #endregion

        /// <summary>
        /// Типы предметов
        /// </summary>
        public enum ItemType
        {
            Helmet,
            Armor,
            Weapon,
            OnceToUse
        }
    }
}
