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
            inventory = new List<Item>();
        }
        public List<int> stats; // 0-lvl 1-ATKphys 2-ATKmag 3-DEFPhys 4-DEFMag 5-Health
        public PictureBox pb = new PictureBox();
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
        
        public void Loot(Item i)
        {
            if (inventory.Count < 26)
                inventory.Add(i);
        }
        
        public void ThrowOut(int which)
        {
            inventory.RemoveAt(which);
            inventory.TrimExcess();
        }
        public void Equip()
        {

        }
        
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
    }

}

    
       
       

