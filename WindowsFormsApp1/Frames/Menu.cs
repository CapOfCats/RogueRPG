using GameEngine;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace Rogue_JRPG.Frames
{
    public class Main_Menu : Frame
    {

        private PictureBox title;
        private PictureBox start;
        

        public Main_Menu(Engine engine) : base(engine)
        {
            title = Engine.PicCreation(
                new Point(GetWindow().GetSize().Width/2-GetWindow().GetSize().Width/8, GetWindow().GetSize().Height / 4),  // 2-8 4
                new Size (GetWindow().GetSize().Width/4,GetWindow().GetSize().Height/8), // 4 8
                PictureBoxSizeMode.StretchImage,
                Image.FromFile(@"..\\..\\Menu\\title.png"),                
                true
                );
            start = Engine.PicCreation(
                new Point(GetWindow().GetSize().Width / 2 - GetWindow().GetSize().Width / 12, GetWindow().GetSize().Height / 2 - GetWindow().GetSize().Height/12), //2-12 2
                new Size(GetWindow().GetSize().Width / 6, GetWindow().GetSize().Height / 8),//6 8
                PictureBoxSizeMode.StretchImage,
                Image.FromFile(@"..\\..\\Menu\\start.png"),               
                true
                );
            start.Click += (sender, e) =>
            {
                engine.LoadFrame("Levelmap");
                engine.DelFrame("Main_Menu");
            };
            controlStash = new List<Control>() { title, start };
        }
        

        public override void Load()
        {
            GetWindow().GetControl().Paint += Background;
            GetWindow().GetControl().Controls.Add(start);
            GetWindow().GetControl().Controls.Add(title);
        }

        public override void UnLoad()
        {
            GetWindow().GetControl().Paint -= Background;
            GetWindow().GetControl().Controls.Clear();
        }

        private void Background(object sender, PaintEventArgs e)
        {
            GetWindow().GetControl().BackColor = Color.FromArgb(0, 69, 25, 52); //Цвет заднего фона
        }
    }
}
