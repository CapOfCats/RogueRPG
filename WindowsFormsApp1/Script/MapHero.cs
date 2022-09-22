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
        public MapHero(List<Item> _inventory, List<Image> _appearances, Camera _cam, Knight _who)
        {
            inventory = _inventory;
            appearances = _appearances;
            cam = _cam;
            who = _who;
        }
        public List<Item> inventory; //вся куча итемов
        public List<Image> appearances; //4 пикчи(по поворотам)
        public PictureBox pb;
        public Camera cam; // для трекинга,возможно тут не нужна будет
        public Knight who; // цвет, по нему определяем входные пикчи
        public Dictionary<Item.ItemType, Item> equipment; //Хз делать это или обойдемся листом
        public enum Knight
        {
            Frozen,
            Blazy,
            Electric,
            Poisonous
        }
        //public void Animation(){} //Анимка движения
        
    }

}

    
       
       

