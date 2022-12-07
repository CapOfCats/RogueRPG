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
        PictureBox board;
        PictureBox board2;
        PictureBox portrait;
        List<PictureBox> knightIcons;
        List<PictureBox> statIcons;
        List<Label> stats;
        List<PictureBox> equipment;
        ToolTip t = new ToolTip();
        Button inventoryButton;
        bool invOpened = false;
        DataGrid inventory;
        static Dictionary<MapHero.Knight,MapHero> squad;


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
            //
            map.SendToBack();
            SquadInit();
            MapHero = squad[MapHero.Knight.Frozen];
            ArrowInit();
            BoardInit();
            PositionCheck();
            StatInit();
            //          
            map.DoubleClick += (sender, e) =>
            {
                engine.ToggleWindowState();
                foreach (Label l in stats)
                    l.Font = new Font("Trebuchet MS", statIcons[0].Width / 9*2, FontStyle.Italic);
            };
            controlStash = new List<Control>() { map, portrait, board, board2, inventory, inventoryButton };
            controlStash.AddRange(arrowStash);
            controlStash.AddRange(knightIcons);
            controlStash.AddRange(statIcons);
            controlStash.AddRange(stats);
            controlStash.AddRange(equipment);
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
                Engine.PicCreationTransparent(
                    new Point(GetWindow().GetSize().Width/3+map.Width/3+map.Width/50, GetWindow().GetSize().Height/2),//+GetWindow().GetSize().Height/24),
                    new Size(GetWindow().GetSize().Width/11,GetWindow().GetSize().Height/12),
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile(@"Backgrounds\\ArrowPointerRight.png"),
                    true
                    ),
                 Engine.PicCreationTransparent(
                    new Point(GetWindow().GetSize().Width/3-map.Width/9, GetWindow().GetSize().Height/2),
                    new Size(GetWindow().GetSize().Width/11,GetWindow().GetSize().Height/12),
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile(@"Backgrounds\\ArrowPointerLeft.png"),
                    true
                    ),
                  Engine.PicCreationTransparent(
                    new Point(GetWindow().GetSize().Width/2-GetWindow().GetSize().Width/50, GetWindow().GetSize().Height-GetWindow().GetSize().Height/6),
                    new Size(GetWindow().GetSize().Height/12,GetWindow().GetSize().Width/11),
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile(@"Backgrounds\\ArrowPointerDown.png"),
                    true
                    ),
                   Engine.PicCreationTransparent(
                    new Point(GetWindow().GetSize().Width/2-GetWindow().GetSize().Width/50, GetWindow().GetSize().Height/50),
                    new Size(GetWindow().GetSize().Height/12,GetWindow().GetSize().Width/11),
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile(@"Backgrounds\\ArrowPointerUp.png"),
                    true
                    )
            };
            timer = new Timer { Interval = 5, Enabled = false };
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
            //
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
                            y = -map.Height;                                
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
                  l += GetWindow().GetSize().Height / 100;

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

        public void BoardInit()
        {
            board = Engine.PicBoxSkeleton(
                    new Point(0,0),
                    new Size((map.Width-map.Height)/2, GetWindow().GetSize().Height),
                    PictureBoxSizeMode.StretchImage,
                    true
                    );            
            board.BringToFront();
            board.BorderStyle = BorderStyle.None;
            board2 = Engine.PicBoxSkeleton(
                    new Point((map.Width-map.Height)/2+map.Height, 0),
                    new Size((map.Width - map.Height) / 2, GetWindow().GetSize().Height),
                    PictureBoxSizeMode.StretchImage,
                    true
                    );
            board2.BringToFront();           
            board.BackgroundImage = Image.FromFile(@"Map\\board.png");
            board.BackgroundImageLayout = ImageLayout.Stretch;
            board2.BackgroundImage = Image.FromFile(@"Map\\board.png");
            board2.BackgroundImageLayout = ImageLayout.Stretch;
            board.DoubleClick += (sender, e) =>
            {
                engine.ToggleWindowState();
                if (stats!=null)
                    foreach (Label l in stats)
                        l.Font = new Font("Trebuchet MS", statIcons[0].Width / 9 * 2, FontStyle.Italic);
            };
            board2.DoubleClick += (sender, e) =>
            {
                engine.ToggleWindowState();
                if (stats != null)
                    foreach (Label l in stats)
                        l.Font = new Font("Trebuchet MS", statIcons[0].Width / 9 * 2, FontStyle.Italic);
            };
            //
            portrait = Engine.PicCreation(
                new Point(GetWindow().GetSize().Width / 15, GetWindow().GetSize().Height / 7),
                new Size(GetWindow().GetSize().Width / 10, GetWindow().GetSize().Height / 5),
                PictureBoxSizeMode.StretchImage,
                DefineKnight(),
                true
                );
            portrait.BackgroundImage = Image.FromFile(@"appearances\\frozenBack.png");
            portrait.BackgroundImageLayout = ImageLayout.Stretch;
            portrait.BorderStyle = BorderStyle.Fixed3D;
            //
            knightIcons = new List<PictureBox>()
            {
                Engine.PicCreation(
                    new Point(GetWindow().GetSize().Width/25, GetWindow().GetSize().Height/3+map.Height/12),//+GetWindow().GetSize().Height/24),
                    new Size(GetWindow().GetSize().Width/15,GetWindow().GetSize().Height/15),
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile(@"Map\\water.png"),
                    true
                    ),
                 Engine.PicCreation(
                    new Point(GetWindow().GetSize().Width/8, GetWindow().GetSize().Height/3+map.Height/12),
                    new Size(GetWindow().GetSize().Width/15,GetWindow().GetSize().Height/15),
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile(@"Map\\blaze.png"),
                    true
                    ),
                  Engine.PicCreation(
                    new Point(GetWindow().GetSize().Width/25, GetWindow().GetSize().Height/2),
                    new Size(GetWindow().GetSize().Width/15,GetWindow().GetSize().Height/15),
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile(@"Map\\spark.png"),
                    true
                    ),
                   Engine.PicCreation(
                    new Point(GetWindow().GetSize().Width/8, GetWindow().GetSize().Height/2),
                    new Size(GetWindow().GetSize().Width/15,GetWindow().GetSize().Height/15),
                    PictureBoxSizeMode.StretchImage,
                   Image.FromFile(@"Map\\poison.png"),
                    true
                    )
            };
            knightIcons[0].MouseLeave += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                knightIcons[0].Size = new Size(GetWindow().GetSize().Width / 15, GetWindow().GetSize().Height / 15);
                knightIcons[0].Location = new Point(GetWindow().GetSize().Width / 25, GetWindow().GetSize().Height / 3 + map.Height / 12);
            });
            knightIcons[0].MouseEnter += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                knightIcons[0].Size = new Size(GetWindow().GetSize().Width / 18, GetWindow().GetSize().Height / 18);
                knightIcons[0].Location = new Point(knightIcons[0].Location.X + knightIcons[0].Width / 6, knightIcons[0].Location.Y + knightIcons[0].Height / 6);
            });
            knightIcons[1].MouseLeave += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                knightIcons[1].Size = new Size(GetWindow().GetSize().Width / 15, GetWindow().GetSize().Height / 15);
                knightIcons[1].Location = new Point(GetWindow().GetSize().Width / 8, GetWindow().GetSize().Height / 3 + map.Height / 12);                
            });
            knightIcons[1].MouseEnter += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                knightIcons[1].Size = new Size(GetWindow().GetSize().Width / 18, GetWindow().GetSize().Height / 18);
                knightIcons[1].Location = new Point(knightIcons[1].Location.X + knightIcons[1].Width / 6, knightIcons[1].Location.Y + knightIcons[1].Height / 6);
            });
            knightIcons[2].MouseLeave += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                knightIcons[2].Size = new Size(GetWindow().GetSize().Width / 15, GetWindow().GetSize().Height / 15);
                knightIcons[2].Location = new Point(GetWindow().GetSize().Width / 25, GetWindow().GetSize().Height / 2);
            });
            knightIcons[2].MouseEnter += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                knightIcons[2].Size = new Size(GetWindow().GetSize().Width / 18, GetWindow().GetSize().Height / 18);
                knightIcons[2].Location = new Point(knightIcons[2].Location.X + knightIcons[2].Width / 6, knightIcons[2].Location.Y + knightIcons[2].Height / 6);
            });
            knightIcons[3].MouseLeave += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                knightIcons[3].Size = new Size(GetWindow().GetSize().Width / 15, GetWindow().GetSize().Height / 15);
                knightIcons[3].Location = new Point(GetWindow().GetSize().Width / 8, GetWindow().GetSize().Height / 2);
            });
            knightIcons[3].MouseEnter += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                knightIcons[3].Size = new Size(GetWindow().GetSize().Width / 18, GetWindow().GetSize().Height / 18);
                knightIcons[3].Location = new Point(knightIcons[3].Location.X + knightIcons[3].Width / 6, knightIcons[3].Location.Y + knightIcons[3].Height / 6);
            });
            knightIcons[0].Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                MapHero = squad[MapHero.Knight.Frozen];
                portrait.Image = Image.FromFile(@"appearances\\frozen.png");
                portrait.BackgroundImage = Image.FromFile(@"appearances\\frozenBack.png");
                StatInit();
            });
            knightIcons[1].Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                MapHero = squad[MapHero.Knight.Blazy];
                portrait.Image = Image.FromFile(@"appearances\\blazy.png");
                portrait.BackgroundImage = Image.FromFile(@"appearances\\lavaBack.jpg");
                StatInit();
            });
            knightIcons[2].Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                MapHero = squad[MapHero.Knight.Electric];
                portrait.Image = Image.FromFile(@"appearances\\electric.png");
                portrait.BackgroundImage = Image.FromFile(@"appearances\\thunderBack.jpg");
                StatInit();
            });
            knightIcons[3].Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                MapHero = squad[MapHero.Knight.Poisonous];
                portrait.Image = Image.FromFile(@"appearances\\poisonous.png");
                portrait.BackgroundImage = Image.FromFile(@"appearances\\swampBack.png");
                StatInit();
            });
            foreach (PictureBox pb in knightIcons)
                pb.BorderStyle = BorderStyle.Fixed3D;
            
            t.SetToolTip(knightIcons[0], "Выбрать рыцаря Мороза");
            t.SetToolTip(knightIcons[1], "Выбрать рыцаря Пламени");
            t.SetToolTip(knightIcons[2], "Выбрать рыцаря Грозы");
            t.SetToolTip(knightIcons[3], "Выбрать рыцаря Болот");            
            //
            //LevelCheck();
            //
            statIcons = new List<PictureBox>()
            {
                Engine.PicCreation(
                    new Point(GetWindow().GetSize().Width/27, GetWindow().GetSize().Height/3*2 -map.Height/12),//+GetWindow().GetSize().Height/24),
                    new Size(GetWindow().GetSize().Width/25,GetWindow().GetSize().Height/25),
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile(@"propsItems\\Crystal.png"),
                    true
                    ),
                 Engine.PicCreation(
                    new Point(GetWindow().GetSize().Width/27, GetWindow().GetSize().Height/3*2),
                    new Size(GetWindow().GetSize().Width/25,GetWindow().GetSize().Height/25),
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile(@"propsItems\\sword.png"),
                    true
                    ),
                  Engine.PicCreation(
                    new Point(GetWindow().GetSize().Width/27, GetWindow().GetSize().Height/3*2+map.Height/6-map.Height/12),
                    new Size(GetWindow().GetSize().Width/25,GetWindow().GetSize().Height/25),
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile(@"propsItems\\staff.png"),
                    true
                    ),
                   Engine.PicCreation(
                    new Point(GetWindow().GetSize().Width/8, GetWindow().GetSize().Height/3*2-map.Height/12),
                    new Size(GetWindow().GetSize().Width/25,GetWindow().GetSize().Height/25),
                    PictureBoxSizeMode.StretchImage,
                   Image.FromFile(@"propsItems\\shield.png"),
                    true
                    ),
                   Engine.PicCreation(
                    new Point(GetWindow().GetSize().Width/8, GetWindow().GetSize().Height/3*2),
                    new Size(GetWindow().GetSize().Width/25,GetWindow().GetSize().Height/25),
                    PictureBoxSizeMode.StretchImage,
                   Image.FromFile(@"propsItems\\bible.png"),
                    true
                    ),
                   Engine.PicCreation(
                    new Point(GetWindow().GetSize().Width/8, GetWindow().GetSize().Height/3*2+map.Height/6-map.Height/12),
                    new Size(GetWindow().GetSize().Width/25,GetWindow().GetSize().Height/25),
                    PictureBoxSizeMode.StretchImage,
                   Image.FromFile(@"propsItems\\heart.jpg"),
                    true
                    )
            };
            t.SetToolTip(statIcons[0], "Уровень рыцаря");
            t.SetToolTip(statIcons[1], "Физическая атака");
            t.SetToolTip(statIcons[2], "Магическая атака");
            t.SetToolTip(statIcons[3], "Физическая защита");
            t.SetToolTip(statIcons[4], "Магическая защита");
            t.SetToolTip(statIcons[5], "Здоровье");
            foreach(PictureBox pb in statIcons)
            {
                pb.BackgroundImage = Image.FromFile(@"Backgrounds\\DNDTemplate1.png");
                pb.BorderStyle = BorderStyle.Fixed3D;
            }
            //
            stats = new List<Label>()
            {
                Engine.LabCreation(
                    new Point(GetWindow().GetSize().Width/25*2, GetWindow().GetSize().Height/3*2 -map.Height/12),
                    new Size(GetWindow().GetSize().Height/25,GetWindow().GetSize().Height/25),
                    "11",
                    true,
                    ContentAlignment.MiddleCenter,
                    new Font("Trebuchet MS",statIcons[0].Width/9*2, FontStyle.Italic),
                    Color.Silver,
                    Color.SaddleBrown
                    ),
                Engine.LabCreation(
                    new Point(GetWindow().GetSize().Width/25*2, GetWindow().GetSize().Height/3*2),
                    new Size(GetWindow().GetSize().Height/25,GetWindow().GetSize().Height/25),
                    "11",
                    true,
                    ContentAlignment.MiddleCenter,
                    new Font("Trebuchet MS",statIcons[0].Width/9*2, FontStyle.Italic),
                    Color.Silver,
                    Color.SaddleBrown
                    ),
                Engine.LabCreation(
                    new Point(GetWindow().GetSize().Width/25*2, GetWindow().GetSize().Height/3*2+map.Height/6-map.Height/12),
                    new Size(GetWindow().GetSize().Height/25,GetWindow().GetSize().Height/25),
                    "11",
                    true,
                    ContentAlignment.MiddleCenter,
                    new Font("Trebuchet MS",statIcons[0].Width/9*2, FontStyle.Italic),
                    Color.Silver,
                    Color.SaddleBrown
                    ),
                Engine.LabCreation(
                    new Point(GetWindow().GetSize().Width/6, GetWindow().GetSize().Height/3*2 -map.Height/12),
                    new Size(GetWindow().GetSize().Height/25,GetWindow().GetSize().Height/25),
                    "11",
                    true,
                    ContentAlignment.MiddleCenter,
                    new Font("Trebuchet MS",statIcons[0].Width/9*2, FontStyle.Italic),
                    Color.Silver,
                    Color.SaddleBrown
                    ),
                Engine.LabCreation(
                    new Point(GetWindow().GetSize().Width/6, GetWindow().GetSize().Height/3*2),
                    new Size(GetWindow().GetSize().Height/25,GetWindow().GetSize().Height/25),
                    "11",
                    true,
                    ContentAlignment.MiddleCenter,
                    new Font("Trebuchet MS",statIcons[0].Width/9*2, FontStyle.Italic),
                    Color.Silver,
                    Color.SaddleBrown
                    ),
                Engine.LabCreation(
                    new Point(GetWindow().GetSize().Width/6, GetWindow().GetSize().Height/3*2+map.Height/6-map.Height/12),
                    new Size(GetWindow().GetSize().Height/25,GetWindow().GetSize().Height/25),
                    "11",
                    true,
                    ContentAlignment.MiddleCenter,
                    new Font("Trebuchet MS",statIcons[0].Width/9*2, FontStyle.Italic),
                    Color.Silver,
                    Color.SaddleBrown
                    ),
            };

            equipment = new List<PictureBox>()
            {
                Engine.PicBoxSkeleton(
                    new Point(GetWindow().GetSize().Width/25, GetWindow().GetSize().Height/3*2+map.Height/7),
                    new Size(GetWindow().GetSize().Width/25,GetWindow().GetSize().Width/25),
                    PictureBoxSizeMode.StretchImage,
                    true
                    ),
                 Engine.PicBoxSkeleton(
                    new Point(GetWindow().GetSize().Width/11, GetWindow().GetSize().Height/3*2+map.Height/7),
                    new Size(GetWindow().GetSize().Width/25,GetWindow().GetSize().Width/25),
                    PictureBoxSizeMode.StretchImage,
                    true
                    ),
                  Engine.PicBoxSkeleton(
                    new Point(GetWindow().GetSize().Width/7, GetWindow().GetSize().Height/3*2+map.Height/7),
                    new Size(GetWindow().GetSize().Width/25,GetWindow().GetSize().Width/25),
                    PictureBoxSizeMode.StretchImage,
                    true
                    )
            };

            foreach (PictureBox pb in equipment)
            {
                pb.BackColor = Color.DarkGray;
                pb.BorderStyle = BorderStyle.Fixed3D;
            }

            InvInit();  

            inventoryButton = new Button()
            {
                Location = new Point(GetWindow().GetSize().Width / 25, GetWindow().GetSize().Height / 3 * 2 + map.Height / 9*2),
                Size = new Size(GetWindow().GetSize().Width / 7, GetWindow().GetSize().Height / 20),
                Text = "Инвентарь",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Trebuchet MS", equipment[0].Width / 15 * 2, FontStyle.Bold),
                FlatStyle = FlatStyle.Standard,
                BackgroundImageLayout = ImageLayout.Stretch,
                Image = Image.FromFile(@"Backgrounds\\inv.png"),
            };

            inventoryButton.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                invOpened = !invOpened;
                InvFlag(invOpened);
            });
        }

        public void InvInit()
        {
            inventory = new DataGrid()
            {
                BackgroundColor = SystemColors.ActiveCaption,
                BorderStyle = BorderStyle.Fixed3D,
                PreferredColumnWidth = GetWindow().GetSize().Height / 25,
                PreferredRowHeight = GetWindow().GetSize().Height / 25,
                GridLineColor = Color.Beige,
                GridLineStyle = DataGridLineStyle.Solid,
                Location = new Point(GetWindow().GetSize().Width / 9*2, GetWindow().GetSize().Height /3*2),
                ImeMode = ImeMode.NoControl,
                RightToLeft = RightToLeft.Yes,
                Size = new Size((GetWindow().GetSize().Height / 3 * 2 + map.Height / 7)/3, (GetWindow().GetSize().Height / 3 * 2 + map.Height / 7)/3),
                Visible =false
            };
        }
        public void InvFlag(bool o)
        {
            inventory.Visible = o;
        }
        public void LevelCheck()
        {
            foreach (PictureBox ic in knightIcons)
            {
                ic.Enabled = false;
                ic.Image = Image.FromFile(@"Map\\lockedknight.png");
            }
            if (MapHero.levelCount >= 0)
            {
                knightIcons[0].Image = Image.FromFile(@"Map\\water.png");
                knightIcons[0].Enabled = true;
            }
            if (MapHero.levelCount >= 2)
            {
                knightIcons[1].Image = Image.FromFile(@"Map\\blaze.png");
                knightIcons[1].Enabled = true;
            }
            if (MapHero.levelCount >= 4)
            {
                knightIcons[2].Image = Image.FromFile(@"Map\\spark.png");
                knightIcons[2].Enabled = true;
            }
            if (MapHero.levelCount >= 5)
            {
                knightIcons[3].Image = Image.FromFile(@"Map\\poison.png");
                knightIcons[3].Enabled = true;
            }
        }
        public void SquadInit()
        {
            if (squad == null)
            {
                squad = new Dictionary<MapHero.Knight, MapHero>();
                squad.Add(
                    MapHero.Knight.Frozen,
                        new MapHero(
                            MapHero.Knight.Frozen,
                            new List<int>() { 1, 2, 3, 3, 2, 12 }
                            )
                        );
                squad.Add(
                    MapHero.Knight.Blazy,
                        new MapHero(
                            MapHero.Knight.Blazy,
                            new List<int>() { 1, 3, 2, 2, 3, 14 }
                            )
                        );
                squad.Add(
                    MapHero.Knight.Electric,
                        new MapHero(
                            MapHero.Knight.Electric,
                            new List<int>() { 1, 1, 4, 2, 4, 13 }
                            )
                        );
                squad.Add(
                    MapHero.Knight.Poisonous,
                        new MapHero(
                            MapHero.Knight.Poisonous,
                            new List<int>() { 1, 5, 3, 2, 5, 16 }
                            )
                        );
            }
        }
        public void StatInit()
        {
            for (int i = 0; i < 6; i++)
                stats[i].Text = $"{MapHero.stats[i]}";
        }

        public Image DefineKnight()
        {
            switch (MapHero.who)
            {
                case MapHero.Knight.Blazy: return Image.FromFile(@"appearances\\blazy.png");
                case MapHero.Knight.Electric: return Image.FromFile(@"appearances\\electric.png");
                case MapHero.Knight.Poisonous: return Image.FromFile(@"appearances\\poisonous.png");
                default: return Image.FromFile(@"appearances\\frozen.png"); 
            }
        }
        public override void Load()
        {
            GetWindow().GetControl().Controls.Add(portrait);
            GetWindow().GetControl().Controls.Add(stats[0]);
            GetWindow().GetControl().Controls.Add(stats[1]);
            GetWindow().GetControl().Controls.Add(stats[2]);
            GetWindow().GetControl().Controls.Add(stats[3]);
            GetWindow().GetControl().Controls.Add(stats[4]);
            GetWindow().GetControl().Controls.Add(stats[5]);
            GetWindow().GetControl().Controls.Add(statIcons[0]);
            GetWindow().GetControl().Controls.Add(statIcons[1]);
            GetWindow().GetControl().Controls.Add(statIcons[2]);
            GetWindow().GetControl().Controls.Add(statIcons[3]);
            GetWindow().GetControl().Controls.Add(statIcons[4]);
            GetWindow().GetControl().Controls.Add(statIcons[5]);
            GetWindow().GetControl().Controls.Add(equipment[0]);
            GetWindow().GetControl().Controls.Add(equipment[1]);
            GetWindow().GetControl().Controls.Add(equipment[2]);
            GetWindow().GetControl().Controls.Add(inventory);
            GetWindow().GetControl().Controls.Add(inventoryButton);
            GetWindow().GetControl().Controls.Add(knightIcons[0]);
            GetWindow().GetControl().Controls.Add(knightIcons[1]);
            GetWindow().GetControl().Controls.Add(knightIcons[2]);
            GetWindow().GetControl().Controls.Add(knightIcons[3]);
            GetWindow().GetControl().Controls.Add(board);
            GetWindow().GetControl().Controls.Add(board2);           
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
