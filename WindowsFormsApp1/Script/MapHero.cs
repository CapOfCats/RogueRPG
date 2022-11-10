using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Rogue_JRPG
{
    class MapHero
    {
        public MapHero( Knight who, List<int>stats)
        {
            //appearances = _appearances;
            //cam = _cam;
            this.stats = stats;
            this.who = who;
        }
        public List<int> stats; // 0-lvl 1-ATKphys 2-ATKmag 3-DEFPhys 4-DEFMag 5-Health
        public List<Image> appearances; //4 пикчи(по поворотам)
        public PictureBox pb;
        //public Camera cam; // для трекинга,возможно тут не нужна будет
        public Knight who; // цвет, по нему определяем входные пикчи
        public Dictionary<Item.ItemType, Item> equipment; //Хз делать это или обойдемся листом
        public static int levelCount = 0;
        static public List<Item> inventory; //вся куча итемов
        public enum Knight
        {
            Frozen,
            Blazy,
            Electric,
            Poisonous
        }
        //public void Animation(){} //Анимка движения
        
        static void LoadAppearances()
        {

        }
    }

}

    
       
       

