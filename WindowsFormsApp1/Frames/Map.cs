using GameEngine;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace Rogue_JRPG.Frames
{
    public class Map : Frame
    {

        private PictureBox map;
        private MapHero mapHero;
        
        internal MapHero MapHero { get => mapHero; set => mapHero = value; }
        public Map(Engine engine) : base(engine)
        {
            
            map = Engine.PicCreation(
                new Point(0, 0), //265 100
                new Size(GetWindow().GetSize().Width, GetWindow().GetSize().Height), //450 100
                PictureBoxSizeMode.Zoom,
                Utils.Resize(Image.FromFile(@"Backgrounds\\darkworld_large.png"),
                new Size(GetWindow().GetSize().Width, GetWindow().GetSize().Height)), //450 100
                true
                );
            map.SendToBack();
            map.DoubleClick += (sender, e) => engine.ToggleWindowState();
            controlStash = new List<Control>() { map }; // костыль,но через Add ругается на null
            MapHero = new MapHero(new List<Item>(), new List<Image>(), new Camera(), MapHero.Knight.Frozen);
            //тут заальтерить внешние виды, добавить гуи, листание карты, левелинг
        }

        public override void Load()
        {
            GetWindow().GetControl().Controls.Add(map);
        }

        public override void UnLoad()
        {
            GetWindow().GetControl().Controls.Clear();
        }

    }
}
