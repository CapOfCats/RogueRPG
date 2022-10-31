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
                    graphic.DrawImage(mapImage, 0,0, GetWindow().GetSize().Width *2, GetWindow().GetSize().Height*2);
                    mapLayout = new Bitmap(image, GetWindow().GetSize().Height, GetWindow().GetSize().Height);
                    graphic.Dispose();
                }
                map.Image = mapLayout;
            }
            map.SendToBack();
            ArrowInit();
            PositionCheck();
            map.DoubleClick += (sender, e) => engine.ToggleWindowState();
            controlStash = new List<Control>() { map };
            controlStash.AddRange(arrowStash);
            MapHero = new MapHero(new List<Item>(), new List<Image>(), new Camera(), MapHero.Knight.Frozen);
            //тут заальтерить внешние виды, добавить гуи, листание карты, левелинг
        }
        public enum MapPart
        {
            First,
            Second,
            Third,
            Last
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
            timer = new Timer { Interval = 5, Enabled = false };
            for (int i = 0; i < arrowStash.Count; i++)
            {
                arrowStash[i].BringToFront();
            }
            arrowStash[0].MouseEnter += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                timer.Enabled = true;
                ArrowAnim(arrowStash[0], 0, arrowStash[0].Location, true);
            });
            arrowStash[1].MouseEnter += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                timer.Enabled = true;
                ArrowAnim(arrowStash[1], 1, arrowStash[1].Location, true);
            });
            arrowStash[2].MouseEnter += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                timer.Enabled = true;
                ArrowAnim(arrowStash[2], 2, arrowStash[2].Location, true);
            });
            arrowStash[3].MouseEnter += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                timer.Enabled = true;
                ArrowAnim(arrowStash[3], 3, arrowStash[3].Location,true);
            });
            arrowStash[0].MouseLeave += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                timer.Dispose();
                timer = new Timer { Interval = 5, Enabled = false };
                arrowStash[0].Location = new Point(GetWindow().GetSize().Width / 3 + map.Width / 3 + map.Width / 50, GetWindow().GetSize().Height / 2);
            });
            arrowStash[1].MouseLeave += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                timer.Dispose();
                timer = new Timer { Interval = 5, Enabled = false };
                arrowStash[1].Location = new Point(GetWindow().GetSize().Width / 3 - map.Width / 9, GetWindow().GetSize().Height / 2);
            });
            arrowStash[2].MouseLeave += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                timer.Dispose();
                timer = new Timer { Interval = 5, Enabled = false };
                arrowStash[2].Location = new Point(GetWindow().GetSize().Width / 2 - GetWindow().GetSize().Width / 50, GetWindow().GetSize().Height - GetWindow().GetSize().Height / 6);
            });
            arrowStash[3].MouseLeave += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                timer.Dispose();
                timer = new Timer { Interval = 5, Enabled = false };
                arrowStash[3].Location = new Point(GetWindow().GetSize().Width / 2 - GetWindow().GetSize().Width / 50, GetWindow().GetSize().Height / 50);
            });
            arrowStash[0].Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                timer.Dispose();
                timer = new Timer { Interval = 5, Enabled = false };
                MapMove(this.camLocation, 0);

            });
            arrowStash[1].Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                timer.Dispose();
                timer = new Timer { Interval = 5, Enabled = false };
                MapMove(this.camLocation, 1);
            });
            arrowStash[2].Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                timer.Dispose();
                timer = new Timer { Interval = 5, Enabled = false };
                MapMove(this.camLocation, 2);
            });
            arrowStash[3].Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                timer.Dispose();
                timer = new Timer { Interval = 5, Enabled = false };
                MapMove(this.camLocation,3);
            });
        }
        public void ArrowAnim(PictureBox arrow, int i, Point origin, bool direction)
        {
            timer.Start();
            timer.Tick += (sender, e) =>
            {
                switch (i)
                {
                    case 3:
                        {
                            if (arrow.Location.Y < origin.Y + GetWindow().GetSize().Height/14 - (GetWindow().GetSize().Height / 14 %2) && direction == true)
                            {
                                arrow.Location = new Point(arrow.Location.X, arrow.Location.Y + 2);
                                if (arrow.Location.Y == origin.Y + GetWindow().GetSize().Height/14 - (GetWindow().GetSize().Height / 14 % 2))
                                    direction = !direction;                                
                            }
                            else if (arrow.Location.Y > origin.Y && direction == false)
                            {
                                arrow.Location = new Point(arrow.Location.X, arrow.Location.Y - 2);
                                if (arrow.Location.Y == origin.Y)
                                    direction = !direction;
                            }
                                
                        }break;                        
                    case 2:
                        {
                            if (arrow.Location.Y > origin.Y - GetWindow().GetSize().Height/14 - (GetWindow().GetSize().Height / 14 % 2) && direction == true)
                            {
                                arrow.Location = new Point(arrow.Location.X, arrow.Location.Y - 2);
                                if (arrow.Location.Y == origin.Y - GetWindow().GetSize().Height/14 - (GetWindow().GetSize().Height / 14 % 2))
                                    direction = !direction;
                            }
                            else if (arrow.Location.Y < origin.Y && direction == false)
                            {
                                arrow.Location = new Point(arrow.Location.X, arrow.Location.Y + 2);
                                if (arrow.Location.Y == origin.Y)
                                    direction = !direction;
                            }
                        }
                        break;
                    case 1:
                        {
                            if (arrow.Location.X < origin.X + GetWindow().GetSize().Height/14 - (GetWindow().GetSize().Height / 14 % 2) && direction == true)
                            {
                                arrow.Location = new Point(arrow.Location.X + 2, arrow.Location.Y);
                                if (arrow.Location.X == origin.X + GetWindow().GetSize().Height/14 - (GetWindow().GetSize().Height / 14 % 2))
                                    direction = !direction;
                            }
                            else if (arrow.Location.X > origin.X)
                            {
                                arrow.Location = new Point(arrow.Location.X - 2, arrow.Location.Y);
                                if (arrow.Location.X == origin.X)
                                    direction = !direction;
                            }
                        }
                        break;
                    case 0:
                        {
                            if (arrow.Location.X > origin.X - GetWindow().GetSize().Height / 14 - (GetWindow().GetSize().Height / 14 % 2) && direction == true)
                            {
                                arrow.Location = new Point(arrow.Location.X - 2, arrow.Location.Y);
                                if (arrow.Location.X == origin.X - GetWindow().GetSize().Height / 14- -(GetWindow().GetSize().Height / 14 % 2))
                                    direction = !direction;
                            }
                            else if (arrow.Location.X < origin.X)
                            {
                                arrow.Location = new Point(arrow.Location.X + 2, arrow.Location.Y);
                                if (arrow.Location.X == origin.X)
                                    direction = !direction;
                            }
                        }
                        break;
                }
                
            };      
              
        }

        public void PositionCheck()
        {
            switch(camLocation)
            {
                case MapPart.First:
                    {
                        arrowStash[0].Visible = true;
                        arrowStash[1].Visible = false;
                        arrowStash[2].Visible = false;
                        arrowStash[3].Visible = false;
                    } break;//0 right 1left 2down 3up
                case MapPart.Second:
                    {
                        arrowStash[1].Visible = true;
                        arrowStash[2].Visible = true;
                        arrowStash[0].Visible = false;
                        arrowStash[3].Visible = false;
                    }
                    break;//0 right 1left 2down 3up
                case MapPart.Third:
                    {
                        arrowStash[1].Visible = true;
                        arrowStash[3].Visible = true;
                        arrowStash[0].Visible = false;
                        arrowStash[2].Visible = false;
                    }
                    break;//0 right 1left 2down 3up
                case MapPart.Last:
                    {
                        arrowStash[1].Visible = false;
                        arrowStash[2].Visible = false;
                        arrowStash[0].Visible = true;
                        arrowStash[3].Visible = false;
                    }
                    break;//0 right 1left 2down 3up
            }
        }

        public void MapMove(MapPart mp, int num)
        {
            map.Enabled = false;
            engine.window.mainForm.Enabled = false;
            int x = 0;
            int y = 0;
            int a = 0;
            int b = 0;
            int k = 0;
            int l = 0;
            Image mapImage = Image.FromFile(@"Backgrounds\\lightworld_large.png");
            foreach (PictureBox arrow in arrowStash)
                arrow.Visible = false;

            switch (mp)
            {
                case MapPart.First:
                    {
                        x = -map.Width;
                        camLocation = MapPart.Second;
                    }
                    break;
                case MapPart.Second:
                    {
                        if (num == 1)
                        {
                            camLocation = MapPart.First;                 
                        }
                        else
                        {
                            y = -map.Height;                                 ////////////////////
                            x = -map.Width;
                            camLocation = MapPart.Third;
                        }
                    }
                    break;
                case MapPart.Third:
                    {
                        if (num == 3)
                        {
                            x = -map.Width;
                            camLocation = MapPart.Second;
                        }
                        else
                        {
                            x = 0;
                            y = -map.Height;
                            camLocation = MapPart.Last;
                        }
                    }
                    break;
                case MapPart.Last:
                    {
                        x = -map.Width; ;
                        y = -map.Height;
                        camLocation = MapPart.Third;
                    }
                    break;
            }
            Timer timer2 = new Timer { Interval = 1, Enabled = true };
            timer2.Start();
            timer2.Tick += (sender, e) =>
              {
                   engine.window.GetForm().Invalidate();
                   k+= GetWindow().GetSize().Width / 100;
                  l += GetWindow().GetSize().Height / 100;/////////////////////

                  if (y == 0 && mp != MapPart.Third)
                  {
                      a = (x < 0) ? a - GetWindow().GetSize().Width/100 : -map.Width + k;
                      if (x < 0 && a <= x)
                      {
                          PositionCheck();
                          timer2.Dispose();
                          map.Enabled = true;
                          engine.window.mainForm.Enabled = true;
                      }
                      else
                         if (a >= x && x == 0)
                      {
                          PositionCheck();
                          timer2.Dispose();
                          map.Enabled = true;
                          engine.window.mainForm.Enabled = true;
                      }
                  }
                  else if (x == 0 && mp != MapPart.Third)
                  {
                      b = (y < 0) ? b - GetWindow().GetSize().Height / 100 : -map.Height + l;
                      if (y < 0 && b <= y)
                      {
                          PositionCheck();
                          timer2.Dispose();
                          map.Enabled = true;
                          engine.window.mainForm.Enabled = true;
                      }
                      else if (b >= y && y == 0)
                      {
                          PositionCheck();
                          timer2.Dispose();
                          map.Enabled = true;
                          engine.window.mainForm.Enabled = true;
                      }
                  }
                  else if (mp == MapPart.Third)
                  {
                      if (y == 0)
                      {
                          a = x;
                          b = -map.Height + l;
                          if(b>=0)
                          {
                              PositionCheck();
                              timer2.Dispose();
                              map.Enabled = true;
                              engine.window.mainForm.Enabled = true;
                          }
                      }
                      else if(x==0)
                      {
                          b = y;
                          a = -map.Width + k;
                          if(a>=0)
                          {
                              PositionCheck();
                              timer2.Dispose();
                              map.Enabled = true;
                              engine.window.mainForm.Enabled = true;
                          }
                      }
                  }
                  else
                  {                     
                      if (mp == MapPart.Second)
                      {
                          b -= GetWindow().GetSize().Height / 100;
                          a = x;
                          if (b <= y)
                          {
                              PositionCheck();
                              timer2.Dispose();
                              map.Enabled = true;
                              engine.window.mainForm.Enabled = true;
                          }
                      }
                      else
                      {
                          a -= GetWindow().GetSize().Width / 100;
                          b = y;
                          if (a <= x)
                          {
                              PositionCheck();
                              timer2.Dispose();
                              map.Enabled = true;
                              engine.window.mainForm.Enabled = true;
                          }
                      }                           
                  }
                  using (Bitmap image = new Bitmap(GetWindow().GetSize().Width, GetWindow().GetSize().Height))
                  {
                      using (Graphics graphic = Graphics.FromImage(image))
                      {
                          graphic.DrawImage(mapImage, a, b, GetWindow().GetSize().Width * 2, GetWindow().GetSize().Height * 2);
                          mapLayout = new Bitmap(image, GetWindow().GetSize().Height, GetWindow().GetSize().Height);
                          graphic.Dispose();
                      }
                  }
                  map.Image = mapLayout;                                   
              };
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
