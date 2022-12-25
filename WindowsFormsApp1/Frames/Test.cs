using GameEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rogue_JRPG.Frames
{
    public class Test : Frame
    {

        PictureBox pb = new PictureBox();
        
        public Test(Engine engine) : base(engine)
        {
            List<Image> appearances = new List<Image>();
            appearances.Add(Image.FromFile("poison.png"));
            appearances.Add(Image.FromFile("blazy.png"));
            appearances.Add(Image.FromFile("frozen.png"));
            appearances.Add(Image.FromFile("electric.png"));
            pb.Location = new Point(200, 200);
            
            MapHero Hero = new MapHero(inventory, appearances, MapHero.Knight.Electric);
            Hero.pb = pb;

            void GetSheet()
            {
                if (Hero.who == MapHero.Knight.Poisonous) pb.Image = appearances[0];
                if (Hero.who == MapHero.Knight.Blazy) pb.Image = appearances[1];
                if (Hero.who == MapHero.Knight.Frozen) pb.Image = appearances[2];
                if (Hero.who == MapHero.Knight.Electric) pb.Image = appearances[3];
            }
            GetSheet();
            pb.Image = Utils.Crop(pb.Image, 0, 0);

            int counter = 0;
            void HeroAnimation(int offset)
            {
                GetSheet();
                Image sheet = pb.Image;
                int x = 0;

                //Cмена кадров
                if (counter != 0) x += 32;
                pb.Image = Utils.Crop(sheet, x, offset);
                counter++;
                if (counter == 2) { x = 0; counter = 0; }
            }

            this.GetWindow().GetForm().KeyDown += new KeyEventHandler(KeyDown);

            void KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.W)
                {
                    HeroAnimation(96);
                    pb.Location = new Point(pb.Location.X, pb.Location.Y - 8);
                }

                if (e.KeyCode == Keys.A)
                {
                    HeroAnimation(32);
                    pb.Location = new Point(pb.Location.X - 8, pb.Location.Y);
                }

                if (e.KeyCode == Keys.S)
                {
                    HeroAnimation(0);
                    pb.Location = new Point(pb.Location.X, pb.Location.Y + 8);
                }

                if (e.KeyCode == Keys.D)
                {
                    HeroAnimation(64);
                    pb.Location = new Point(pb.Location.X + 8, pb.Location.Y);
                }
            }
        }

        public override void Load()
        {
            this.GetWindow().GetControl().Controls.Add(pb);
        }

        public override void UnLoad()
        {
            this.GetWindow().GetControl().Controls.Clear();
        }
    }
}
