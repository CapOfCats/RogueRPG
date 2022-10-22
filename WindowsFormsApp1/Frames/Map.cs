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
        Bitmap mapLayout;
        MapPart camLocation;
        List<PictureBox> arrowStash;
        Timer timer;

        internal MapHero MapHero { get => mapHero; set => mapHero = value; }
        public Map(Engine engine) : base(engine)
        {
            this.camLocation = MapPart.First;
            map = Engine.PicBoxSkeleton(
                new Point(0, 0),
                new Size(GetWindow().GetSize().Width, GetWindow().GetSize().Height),
                PictureBoxSizeMode.Zoom,
                true
                );
            using (Bitmap image = new Bitmap(GetWindow().GetSize().Width, GetWindow().GetSize().Height))
            {
                using (Graphics graphic = Graphics.FromImage(image))
                {
                    Image mapImage = Image.FromFile(@"Backgrounds\\lightworld_large.png");
                    graphic.DrawImage(mapImage, 0, 0, GetWindow().GetSize().Width * 2, GetWindow().GetSize().Height * 2);
                    //graphic.DrawImage(mapImage, 0, 0, GetWindow().GetSize().Height, GetWindow().GetSize().Height);
                    //graphic.SetClip(new Rectangle(0, 0, GetWindow().GetSize().Height/4, GetWindow().GetSize().Height/4));
                    mapLayout = new Bitmap(image, GetWindow().GetSize().Height, GetWindow().GetSize().Height);
                    graphic.Dispose();
                }
                map.Image = mapLayout;
            }
            map.SendToBack();
            ArrowInit();
            map.DoubleClick += (sender, e) => engine.ToggleWindowState();
            controlStash = new List<Control>() { map };
            controlStash.AddRange(arrowStash);
            MapHero = new MapHero(new List<Item>(), new List<Image>(), new Camera(), MapHero.Knight.Frozen);
            //тут заальтерить внешние виды, добавить гуи, листание карты, левелинг
            //map.Image = mapLayout;
            /*mapLayout = new Bitmap(Image.FromFile(@"Backgrounds\\darkworld_large.png"),GetWindow().GetSize().Width, GetWindow().GetSize().Height);
            using (g = Graphics.FromImage(mapLayout))
            {
                g = Graphics.FromImage(mapLayout);
                g.DrawImageUnscaledAndClipped(
                    mapLayout,
                    new Rectangle(
                         0,
                         0,
                         GetWindow().GetSize().Width / 4,
                         GetWindow().GetSize().Height / 4)
                    );
                mapLayout = new Bitmap(GetWindow().GetSize().Width, GetWindow().GetSize().Height, g);
            }
            map.Image = mapLayout;
            /* g.DrawImage(
                 Image.FromFile(@"Backgrounds\\darkworld_large.png"),
                 0.0F,
                 0.0F,
                 new Rectangle(
                     0,
                     0,
                     GetWindow().GetSize().Width / 4,
                     GetWindow().GetSize().Height / 4),
                 GraphicsUnit.Point
                 );        */
            //map.Paint += new PaintEventHandler(DrawImageFRect);
            //g.Dispose();

        }
        public void ArrowInit()
        {
            arrowStash = new List<PictureBox>()
            {
                Engine.PicCreation(
                    new Point(GetWindow().GetSize().Width/3+map.Width/3+map.Width/50, GetWindow().GetSize().Height/2),//+GetWindow().GetSize().Height/24),
                    new Size(GetWindow().GetSize().Width/11,GetWindow().GetSize().Height/12),
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile(@"Backgrounds\\ArrowPointerRight.png"),
                    true
                    ),
                 Engine.PicCreation(
                    new Point(GetWindow().GetSize().Width/3-map.Width/9, GetWindow().GetSize().Height/2),
                    new Size(GetWindow().GetSize().Width/11,GetWindow().GetSize().Height/12),
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile(@"Backgrounds\\ArrowPointerLeft.png"),
                    true
                    ),
                  Engine.PicCreation(
                    new Point(GetWindow().GetSize().Width/2-GetWindow().GetSize().Width/50, GetWindow().GetSize().Height-GetWindow().GetSize().Height/6),
                    new Size(GetWindow().GetSize().Height/12,GetWindow().GetSize().Width/11),
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile(@"Backgrounds\\ArrowPointerDown.png"),
                    true
                    ),
                   Engine.PicCreation(
                    new Point(GetWindow().GetSize().Width/2-GetWindow().GetSize().Width/50, GetWindow().GetSize().Height/50),
                    new Size(GetWindow().GetSize().Height/12,GetWindow().GetSize().Width/11),
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile(@"Backgrounds\\ArrowPointerUp.png"),
                    true
                    )
            };
            for (int i = 0; i < arrowStash.Count; i++)
            {
                arrowStash[i].BringToFront();
            }
            arrowStash[0].MouseEnter += new System.EventHandler((object sender, System.EventArgs e) =>
            {
               // ArrowAnim(arrowStash[0], 0);
            });
            arrowStash[1].MouseEnter += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                //ArrowAnim(arrowStash[1], 1);
            });
            arrowStash[2].MouseEnter += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                //ArrowAnim(arrowStash[2], 2);
            });
            arrowStash[3].MouseEnter += new System.EventHandler((object sender, System.EventArgs e) =>
            {                
                ArrowAnim(arrowStash[3], 3, arrowStash[3].Location,true);
            });
        }
        public void ArrowAnim(PictureBox arrow, int i, Point origin, bool direction)
        {            
            timer = new Timer { Interval = 10, Enabled = true };
            timer.Tick += (sender, e) =>
            {
                switch (i)
                {
                    case 3:
                        {
                            timer.Enabled = false;
                            if (arrow.Location.Y < origin.Y + 200 && direction == true)
                            {
                                arrow.Location = new Point(arrow.Location.X, arrow.Location.Y + 1);
                                if (arrow.Location.Y == origin.Y + 200)
                                    ArrowAnim(arrow, i, origin, false);                                
                            }
                            else if (arrow.Location.Y > origin.Y && direction == false)
                            {
                                arrow.Location = new Point(arrow.Location.X, arrow.Location.Y - 1);
                                if (arrow.Location.Y == origin.Y)
                                    ArrowAnim(arrow, i, origin, true);
                            }
                                
                        }break;                        
                    case 2:
                        {
                            timer.Enabled = false;
                            if (arrow.Location.Y > origin.Y - 200 && direction == true)
                            {
                                arrow.Location = new Point(arrow.Location.X, arrow.Location.Y - 1);
                                if (arrow.Location.Y == origin.Y - 200)
                                    ArrowAnim(arrow, i, origin, false);
                            }
                            else if (arrow.Location.Y < origin.Y && direction == false)
                            {
                                arrow.Location = new Point(arrow.Location.X, arrow.Location.Y + 1);
                                if (arrow.Location.Y == origin.Y)
                                    ArrowAnim(arrow, i, origin, true);
                            }
                        }
                        break;
                    case 1:
                        {
                            timer.Enabled = false;
                            if (arrow.Location.X < origin.X + 200 && direction == true)
                            {
                                arrow.Location = new Point(arrow.Location.X + 1, arrow.Location.Y);
                                if (arrow.Location.X == origin.X + 200)
                                    ArrowAnim(arrow, i, origin, false);
                            }
                            else if (arrow.Location.X > origin.X)
                            {
                                arrow.Location = new Point(arrow.Location.X - 1, arrow.Location.Y);
                                if (arrow.Location.X == origin.X)
                                    ArrowAnim(arrow, i, origin, true);
                            }
                        }
                        break;
                    case 0:
                        {
                            timer.Enabled = false;
                            if (arrow.Location.X > origin.X - 200 && direction == true)
                            {
                                arrow.Location = new Point(arrow.Location.X - 1, arrow.Location.Y);
                                if (arrow.Location.X == origin.X - 200)
                                    ArrowAnim(arrow, i, origin, false);
                            }
                            else if (arrow.Location.X < origin.X)
                            {
                                arrow.Location = new Point(arrow.Location.X + 1, arrow.Location.Y);
                                if (arrow.Location.X == origin.X)
                                    ArrowAnim(arrow, i, origin, true);
                            }
                        }
                        break;
                }
                
            };
                  //timer.Dispose();
                 
                 
              
        }

        public enum MapPart
        {
            First,
            Second,
            Third,
            Last
        }

        public override void Load()
        {
            GetWindow().GetControl().Controls.Add(arrowStash[0]);
            GetWindow().GetControl().Controls.Add(arrowStash[1]);
            GetWindow().GetControl().Controls.Add(arrowStash[2]);
            GetWindow().GetControl().Controls.Add(arrowStash[3]);
            GetWindow().GetControl().Controls.Add(map);
           
        }

        public override void UnLoad()
        {
            GetWindow().GetControl().Controls.Clear();
        }

    }
}
