using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Rogue_JRPG
{
    /// <summary>
    /// Сущность игрока
    /// </summary>
    class MapHero
    {
        #region Конструктор
        public MapHero( Knight who, List<int>stats)
        {
            //appearances = _appearances;
            equipment = new Dictionary<Item.ItemType, Item>();
            this.stats = stats;
            this.who = who;
            inventory = new List<Item>();
        }
        #endregion

        #region Поля
        /// <summary>
        /// Характеристики рыцаря
        /// </summary>
        public List<int> stats; // 0-lvl 1-ATKphys 2-ATKmag 3-DEFPhys 4-DEFMag 5-Health
        /// <summary>
        /// Аватар рыцаря
        /// </summary>
        public PictureBox pb = new PictureBox();
        /// <summary>
        /// Стихия рыцаря
        /// </summary>
        public Knight who;
        /// <summary>
        /// Надетое снаряжение
        /// </summary>
        public Dictionary<Item.ItemType, Item> equipment;
        /// <summary>
        /// Количество пройденных уровней
        /// </summary>
        public static int levelCount = 0;
        /// <summary>
        /// Хранилище предметов игрока
        /// </summary>
        public List<Item> inventory;
        #endregion

        /// <summary>
        /// Стихия рыцаря
        /// </summary>
        public enum Knight
        {
            Frozen,
            Blazy,
            Electric,
            Poisonous
        }

        #region Методы
        /// <summary>
        /// Добавляет предмет в инвентарь
        /// </summary>
        /// <param name="i">Предмет</param>
        public void Loot(Item i)
        {
            if (inventory.Count < 26)
                inventory.Add(i);
        }

        /// <summary>
        /// Выбрасывает предмет из инвентаря
        /// </summary>
        /// <param name="which">Индекс предмета</param>
        public void ThrowOut(int which)
        {
            inventory.RemoveAt(which);
        }

        /// <summary>
        /// Загружает текстуры внешнего вида
        /// </summary>
        public void LoadAppearances()
        {
            List<Image> appearances = new List<Image>()
                { 
                Image.FromFile("poison.png"),
                Image.FromFile("blazy.png"),
                Image.FromFile("frozen.png"),
                Image.FromFile("electric.png")
                };
            
            switch(who)
                {
                case MapHero.Knight.Poisonous: pb.Image = appearances[0]; break;
                case MapHero.Knight.Blazy: pb.Image = appearances[1]; break;
                case MapHero.Knight.Frozen: pb.Image = appearances[2]; break;
                case MapHero.Knight.Electric: pb.Image = appearances[3]; break;
                }   
        }
        #endregion
    }

}

    
       
       

