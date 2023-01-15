using GameEngine;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace Rogue_JRPG.Frames
{
    /// <summary>
    /// Окно карты
    /// </summary>
    public class Map : Frame
    {
        #region Поля
        /// <summary>
		/// Мейн бэкграунд
		/// </summary>	
        private PictureBox map;
        /// <summary>
		/// Экземпляр героя в карту
		/// </summary>	
        private MapHero mapHero;
        /// <summary>
		/// Лэйаут для анимации отрисовки
		/// </summary>	
        Bitmap mapLayout;
        /// <summary>
        /// Текущая позиция камеры (1 из 4)
        /// </summary>	
        MapPart camLocation;
        /// <summary>
		///  Хранилище стрелок
		/// </summary>	
        List<PictureBox> arrowStash;
        /// <summary>
		///  Отдельный таймер для анимации
		/// </summary>
        Timer timer;
        /// <summary>
		///  ГУИ слева
		/// </summary>
        PictureBox board;
        /// <summary>
		///  ГУИ справа
		/// </summary>
        PictureBox board2;
        /// <summary>
        ///  Аватар
        /// </summary>
        PictureBox portrait;
        /// <summary>
        /// Иконки рыцарей
        /// </summary>
        List<PictureBox> knightIcons;
        /// <summary>
        ///  Иконки характеристик
        /// </summary>
        List<PictureBox> statIcons;
        /// <summary>
        ///  Значения характеристик
        /// </summary>
        List<Label> stats;
        /// <summary>
        ///  Картинки надетого снаряжения
        /// </summary>
        List<PictureBox> equipment;
        /// <summary>
        /// Подсказыватель
        /// </summary>
        ToolTip t = new ToolTip();
        /// <summary>
        /// Открыть/закрыть инвентарь
        /// </summary>
        Button inventoryButton;
        /// <summary>
        /// Флаг инвентаря
        /// </summary>
        bool invOpened = false;
        /// <summary>
        /// Ячейки инвентаря
        /// </summary>
        List<InventoryCell> inventory;
        /// <summary>
        /// Наш отряд рыцарей
        /// </summary>
        static Dictionary<MapHero.Knight, MapHero> squad;
        /// <summary>
        /// Хранилище координат для ячеек инвентаря
        /// </summary>
        public List<Point> invLocations;
        /// <summary>
        /// Первый уровень
        /// </summary>
        PictureBox level;
        /// <summary>
        /// Экземпляр героя для работы
        /// </summary>
        internal MapHero MapHero { get => mapHero; set => mapHero = value; }
        #endregion

        #region Конструктор
        public Map(Engine engine) : base(engine)
        {
            this.camLocation = MapPart.First; // задаём первую позицию камере
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
                    mapLayout = new Bitmap(image, GetWindow().GetSize().Height, GetWindow().GetSize().Height);
                    graphic.Dispose();
                }
                map.Image = mapLayout;
            }
            //тут отрисовываем карту в зависимости от разрешения
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
                    l.Font = new Font("Trebuchet MS", statIcons[0].Width / 9 * 2, FontStyle.Italic);
            };
            //хандлер для смены экранного режима
            
            controlStash = new List<Control>() { map, portrait, board, board2, inventoryButton, level };
            controlStash.AddRange(arrowStash);
            controlStash.AddRange(knightIcons);
            controlStash.AddRange(statIcons);
            controlStash.AddRange(stats);
            controlStash.AddRange(equipment);
            foreach (InventoryCell ic in inventory)
                controlStash.Add(ic.button);
            // тут добавляем все элементы в родительский стэш, чтобы можно было к ним обращаться, как к общему семейству
        }
        #endregion

        /// <summary>
        /// Часть карты
        /// </summary>
        public enum MapPart
        {
            First,
            Second,
            Third,
            Last
        }

        #region Методы
        /// <summary>
		/// Выполняет настройку стрелок на карте
		/// </summary>
        public void ArrowInit()
        {
            List<Point> coordinateTemp = new List<Point>()
            {
                new Point(GetBound(true)/3+map.Width/3+map.Width/50, GetBound(false)/2),
                new Point(GetBound(true)/3-map.Width/9, GetBound(false)/2),
                new Point(GetBound(true)/2-GetBound(true)/50, GetBound(false)-GetBound(false)/6),
                new Point(GetBound(true)/2-GetBound(true)/50, GetBound(false)/50)
            };
            List<Size> sizeTemp = new List<Size>()
            {
                new Size(GetBound(true)/11,GetBound(false)/12),
                new Size(GetBound(true)/11,GetBound(false)/12),
                new Size(GetBound(false)/12,GetBound(true)/11),
                new Size(GetBound(false)/12,GetBound(true)/11)
            };
            arrowStash = new List<PictureBox>();
            for (int i=0;i<4;i++)
            {
                arrowStash.Add(
                    Engine.PicCreationTransparent(
                    coordinateTemp[i],
                    sizeTemp[i],
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile($@"Backgrounds\\ArrowPointer{i+1}.png"),
                    true
                    ));
            }           
            //4 стрелки для листания

            timer = new Timer { Interval = 5, Enabled = false }; // таймер для анимации стрелок

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
            
            //пока мышь на стрелке - она движется


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
            // мышь выходит за пределы - картинка вновь статична

            
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
                MapMove(this.camLocation, 3);
            });
            
            //по клику меняется позиция карты

        }
        /// <summary>
        /// Выполняет настройку анимации стрелок
        /// </summary>
        /// <param name="arrow">Стрелка.</param>
        /// <param name="i">Номер стрелки</param>
        /// <param name="origin">Начальные координаты</param>
        /// <param name="direction">Направление движения</param>
        public void ArrowAnim(PictureBox arrow, int i, Point origin, bool direction)
        {
            timer.Start();
            timer.Tick += (sender, e) =>
            {
                switch (i)
                {
                    case 3:
                        {
                            if (arrow.Location.Y < origin.Y + GetBound(false) / 14 - (GetBound(false) / 14 %2) && direction == true)
                            {
                                arrow.Location = new Point(arrow.Location.X, arrow.Location.Y + 2);
                                if (arrow.Location.Y == origin.Y + GetBound(false) / 14 - (GetBound(false) / 14 % 2))
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
                            if (arrow.Location.Y > origin.Y - GetBound(false) / 14 - (GetBound(false) / 14 % 2) && direction == true)
                            {
                                arrow.Location = new Point(arrow.Location.X, arrow.Location.Y - 2);
                                if (arrow.Location.Y == origin.Y - GetBound(false) / 14 - (GetBound(false) / 14 % 2))
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
                            if (arrow.Location.X < origin.X + GetBound(false) / 14 - (GetBound(false) / 14 % 2) && direction == true)
                            {
                                arrow.Location = new Point(arrow.Location.X + 2, arrow.Location.Y);
                                if (arrow.Location.X == origin.X + GetBound(false) / 14 - (GetBound(false) / 14 % 2))
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
                            if (arrow.Location.X > origin.X - GetBound(false) / 14 - (GetBound(false) / 14 % 2) && direction == true)
                            {
                                arrow.Location = new Point(arrow.Location.X - 2, arrow.Location.Y);
                                if (arrow.Location.X == origin.X - GetBound(false) / 14- -(GetBound(false) / 14 % 2))
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

        /// <summary>
		/// Показывает стрелки на карте в зависимости от положения камеры
		/// </summary>
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
                    } break;
                case MapPart.Second:
                    {
                        arrowStash[1].Visible = true;
                        arrowStash[2].Visible = true;
                        arrowStash[0].Visible = false;
                        arrowStash[3].Visible = false;
                    }
                    break;
                case MapPart.Third:
                    {
                        arrowStash[1].Visible = true;
                        arrowStash[3].Visible = true;
                        arrowStash[0].Visible = false;
                        arrowStash[2].Visible = false;
                    }
                    break;
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

        /// <summary>
        /// Метод анимации движения по карте
        /// </summary>
        /// <param name="mp">Текущая позиция камеры</param>
        /// <param name="num">Номер стрелки, по которой произведён клик</param>
        public void MapMove(MapPart mp, int num) // определяем 1 из 4 частей бэкграунда в зависимости от позиции
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
                   k+= GetBound(true) / 100;
                  l += GetBound(false) / 100;

                  if (y == 0 && mp != MapPart.Third)
                  {
                      a = (x < 0) ? a - GetBound(true) / 100 : -map.Width + k;
                      if (x < 0 && a <= x)
                      {
                          PositionCheck();
                          timer2.Dispose();                         
                      }
                      else
                         if (a >= x && x == 0)
                      {
                          PositionCheck();
                          timer2.Dispose();                        
                      }
                  }
                  else if (x == 0 && mp != MapPart.Third)
                  {
                      b = (y < 0) ? b - GetBound(false) / 100 : -map.Height + l;
                      if (y < 0 && b <= y)
                      {
                          PositionCheck();
                          timer2.Dispose();                       
                      }
                      else if (b >= y && y == 0)
                      {
                          PositionCheck();
                          timer2.Dispose();                         
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
                          }
                      }
                  }
                  else
                  {                     
                      if (mp == MapPart.Second)
                      {
                          b -= GetBound(false) / 100;
                          a = x;
                          if (b <= y)
                          {
                              PositionCheck();
                              timer2.Dispose();                            
                          }
                      }
                      else
                      {
                          a -= GetBound(true) / 100;
                          b = y;
                          if (a <= x)
                          {
                              PositionCheck();
                              timer2.Dispose();  // enable again                            
                          }
                      }                           
                  }                 
                  //тут вся математика движения

                  using (Bitmap image = new Bitmap(GetBound(true), GetBound(false)))
                  {
                      using (Graphics graphic = Graphics.FromImage(image))
                      {
                          graphic.DrawImage(mapImage, a, b, GetBound(true) * 2, GetBound(false) * 2);
                          mapLayout = new Bitmap(image, GetBound(false), GetBound(false));
                          graphic.Dispose();
                      }
                  }
                  map.Image = mapLayout;
                  //поэтапная отрисовка
              };
            map.Enabled = true;
            engine.window.mainForm.Enabled = true;
        }

        /// <summary>
        /// Создание ГУИ 
        /// </summary>
        public void BoardInit()
        {
            board = Engine.PicBoxSkeleton(
                    new Point(0,0),
                    new Size((map.Width-map.Height)/2, GetBound(false)),
                    PictureBoxSizeMode.StretchImage,
                    true
                    );            
            board.BringToFront();
            board.BorderStyle = BorderStyle.None;
            board2 = Engine.PicBoxSkeleton(
                    new Point((map.Width-map.Height)/2+map.Height, 0),
                    new Size((map.Width - map.Height) / 2, GetBound(false)),
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
            //вешаем на всю неактивную область хандлеры для смены режима окна

            portrait = Engine.PicCreation(
                new Point(GetBound(true) / 15, GetBound(false) / 7),
                new Size(GetBound(true) / 10, GetBound(false) / 5),
                PictureBoxSizeMode.StretchImage,
                DefineKnight(),
                true
                );
            portrait.BackgroundImage = Image.FromFile(@"appearances\\frozenBack.png");
            portrait.BackgroundImageLayout = ImageLayout.Stretch;
            portrait.BorderStyle = BorderStyle.Fixed3D;
            //тут наш рыцарь во всей красе

            knightIcons = new List<PictureBox>();
            List<Point> coordinateTemp = new List<Point>()
            {
                new Point(GetBound(true)/25, GetBound(false)/3+map.Height/12),
                new Point(GetBound(true)/8, GetBound(false)/3+map.Height/12),
                new Point(GetBound(true)/25, GetBound(false)/2),
                new Point(GetBound(true)/8, GetBound(false)/2)
            };
            for (int i=0;i<4;i++)
            {
                knightIcons.Add(Engine.PicCreation(
                    coordinateTemp[i],
                    new Size(GetBound(true) / 15, GetBound(false) / 15),
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile($@"Map\\ic{i}.png"),
                    true
                    ));
            }
            //иконки для смены рыцаря

            level = Engine.PicCreation(
                new Point(GetBound(true) / 2, GetBound(false) / 2),
                new Size(GetBound(true) / 12, GetBound(true) / 12),
                PictureBoxSizeMode.StretchImage,
                Image.FromFile(@"Map\\level1.png"),
                true
                );
            level.BringToFront();
            level.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                engine.LoadFrame("Test");
                engine.DelFrame("Levelmap");
            });
            //кнопка первого уровня(тест)

            knightIcons[0].MouseLeave += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                knightIcons[0].Size = new Size(GetBound(true) / 15, GetBound(false) / 15);
                knightIcons[0].Location = new Point(GetBound(true) / 25, GetBound(false) / 3 + map.Height / 12);
            });
            knightIcons[0].MouseEnter += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                knightIcons[0].Size = new Size(GetBound(true) / 18, GetBound(false) / 18);
                knightIcons[0].Location = new Point(knightIcons[0].Location.X + knightIcons[0].Width / 6, knightIcons[0].Location.Y + knightIcons[0].Height / 6);
            });
            knightIcons[1].MouseLeave += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                knightIcons[1].Size = new Size(GetBound(true) / 15, GetBound(false) / 15);
                knightIcons[1].Location = new Point(GetBound(true) / 8, GetBound(false) / 3 + map.Height / 12);                
            });
            knightIcons[1].MouseEnter += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                knightIcons[1].Size = new Size(GetBound(true) / 18, GetBound(false) / 18);
                knightIcons[1].Location = new Point(knightIcons[1].Location.X + knightIcons[1].Width / 6, knightIcons[1].Location.Y + knightIcons[1].Height / 6);
            });
            knightIcons[2].MouseLeave += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                knightIcons[2].Size = new Size(GetBound(true) / 15, GetBound(false) / 15);
                knightIcons[2].Location = new Point(GetBound(true) / 25, GetBound(false) / 2);
            });
            knightIcons[2].MouseEnter += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                knightIcons[2].Size = new Size(GetBound(true) / 18, GetBound(false) / 18);
                knightIcons[2].Location = new Point(knightIcons[2].Location.X + knightIcons[2].Width / 6, knightIcons[2].Location.Y + knightIcons[2].Height / 6);
            });
            knightIcons[3].MouseLeave += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                knightIcons[3].Size = new Size(GetBound(true) / 15, GetBound(false) / 15);
                knightIcons[3].Location = new Point(GetBound(true) / 8, GetBound(false) / 2);
            });
            knightIcons[3].MouseEnter += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                knightIcons[3].Size = new Size(GetBound(true) / 18, GetBound(false) / 18);
                knightIcons[3].Location = new Point(knightIcons[3].Location.X + knightIcons[3].Width / 6, knightIcons[3].Location.Y + knightIcons[3].Height / 6);
            });
            //минимальная анимка

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
            //cмена рыцаря


            foreach (PictureBox pb in knightIcons)
                pb.BorderStyle = BorderStyle.Fixed3D;
            
            t.SetToolTip(knightIcons[0], "Выбрать рыцаря Мороза");
            t.SetToolTip(knightIcons[1], "Выбрать рыцаря Пламени");
            t.SetToolTip(knightIcons[2], "Выбрать рыцаря Грозы");
            t.SetToolTip(knightIcons[3], "Выбрать рыцаря Болот");    
            
            //
            LevelCheck();// анлок рыцарей, по мере прогресса
            //

            statIcons = new List<PictureBox>();
            coordinateTemp = new List<Point>()
            {
                new Point(GetBound(true)/27, GetBound(false)/3*2 -map.Height/12),
                new Point(GetBound(true)/27, GetBound(false)/3*2),
                new Point(GetBound(true)/27, GetBound(false)/3*2+map.Height/6-map.Height/12),
                new Point(GetBound(true)/8, GetBound(false)/3*2-map.Height/12),
                new Point(GetBound(true)/8, GetBound(false)/3*2),
                new Point(GetBound(true)/8, GetBound(false)/3*2+map.Height/6-map.Height/12)
            };
            for (int i=0;i<6;i++)
            {
                statIcons.Add(
                    Engine.PicCreation(
                    coordinateTemp[i],
                    new Size(GetBound(true) / 25, GetBound(false) / 25),
                    PictureBoxSizeMode.StretchImage,
                    Image.FromFile($@"propsItems\\st{i}.png"),
                    true
                    ));
            }            
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
            //иконки статов

            stats = new List<Label>();
            coordinateTemp = new List<Point>()
            {
                new Point(GetBound(true)/25*2, GetBound(false)/3*2 -map.Height/12),
                new Point(GetBound(true)/25*2, GetBound(false)/3*2),
                new Point(GetBound(true)/25*2, GetBound(false)/3*2+map.Height/6-map.Height/12),
                new Point(GetBound(true)/6, GetBound(false)/3*2 -map.Height/12),
                new Point(GetBound(true)/6, GetBound(false)/3*2),
                new Point(GetBound(true)/6, GetBound(false)/3*2+map.Height/6-map.Height/12)
            };
            for (int i = 0; i < 6; i++)
            {
                stats.Add(Engine.LabCreation(
                    coordinateTemp[i],
                    new Size(GetBound(false) / 25, GetBound(false) / 25),
                    "11",
                    true,
                    ContentAlignment.MiddleCenter,
                    new Font("Trebuchet MS", statIcons[0].Width / 9 * 2, FontStyle.Italic),
                    Color.Silver,
                    Color.SaddleBrown
                    ));
            }
            //значения статов

            equipment = new List<PictureBox>();
            coordinateTemp = new List<Point>()
            {
                new Point(GetBound(true)/25, GetBound(false)/3*2+map.Height/7),
                new Point(GetBound(true)/11, GetBound(false)/3*2+map.Height/7),
                 new Point(GetBound(true)/7, GetBound(false)/3*2+map.Height/7),
            };
            for (int i = 0; i < 3; i++)
            {
                equipment.Add(Engine.PicBoxSkeleton(
                    coordinateTemp[i],
                    new Size(GetBound(true) / 25, GetBound(true) / 25),
                    PictureBoxSizeMode.StretchImage,
                    true
                    ));
            }            
            //надетое снаряжение

            foreach (PictureBox pb in equipment)
            {
                pb.BackColor = Color.DarkGray;
                pb.BorderStyle = BorderStyle.Fixed3D;
            }
            //
            InvInit(); //тут инициализируем инвентарь 
            inventoryButton = new Button()
            {
                Location = new Point(GetBound(true) / 25, GetBound(false) / 3 * 2 + map.Height / 9*2),
                Size = new Size(GetBound(true) / 7, GetBound(false) / 20),
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

        /// <summary>
        /// Метод создания инвентаря
        /// </summary>
        public void InvInit()
        {
            int additionWidth = GetBound(true) / 30;
            inventory = new List<InventoryCell>();
            int cellRatio = GetBound(false) / 25;
            invLocations = new List<Point>();
            for( int i=0;i<5;i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    invLocations.Add(new Point(GetBound(true) / 5 + additionWidth+ (cellRatio+cellRatio/3)*j, GetBound(false) / 3 * 2+(cellRatio+cellRatio/3)*i));
                }
            }           
            //инит каждой ячейки

            for (int i=0;i<25; i++)
            {
                inventory.Add(
                    new InventoryCell(
                    Engine.ButtonCreation(
                        invLocations[i],
                        new Size(GetBound(false)/25,GetBound(false)/25),
                        false,
                        string.Empty,
                        Color.Transparent
                        )
                    )
                );                
                inventory[i].CheckState();
                inventory[i].Block();
                inventory[i].button.Visible = false;                
            }
            inventory[0].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[0].GetState() == InventoryCell.State.Empty || (inventory[0].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(0);
            });
            inventory[1].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[1].GetState() == InventoryCell.State.Empty || (inventory[1].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(1);
            });
            inventory[2].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[2].GetState() == InventoryCell.State.Empty || (inventory[2].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(2);
            });
            inventory[3].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[3].GetState() == InventoryCell.State.Empty || (inventory[3].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(3);
            });
            inventory[4].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[4].GetState() == InventoryCell.State.Empty || (inventory[4].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(4);
            });
            inventory[5].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[5].GetState() == InventoryCell.State.Empty || (inventory[5].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(5);
            });
            inventory[6].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[6].GetState() == InventoryCell.State.Empty || (inventory[6].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(6);
            });
            inventory[7].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[7].GetState() == InventoryCell.State.Empty || (inventory[7].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(7);
            });
            inventory[8].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[8].GetState() == InventoryCell.State.Empty || (inventory[8].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(8);
            });
            inventory[9].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[9].GetState() == InventoryCell.State.Empty || (inventory[9].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(9);
            });
            inventory[10].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[10].GetState() == InventoryCell.State.Empty || (inventory[10].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(10);
            });
            inventory[11].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[11].GetState() == InventoryCell.State.Empty || (inventory[11].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(11);
            });
            inventory[12].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[12].GetState() == InventoryCell.State.Empty || (inventory[12].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(12);
            });
            inventory[13].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[13].GetState() == InventoryCell.State.Empty || (inventory[13].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(13);
            });
            inventory[14].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[14].GetState() == InventoryCell.State.Empty || (inventory[14].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(14);
            });
            inventory[15].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[15].GetState() == InventoryCell.State.Empty || (inventory[15].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(15);
            });
            inventory[16].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[16].GetState() == InventoryCell.State.Empty || (inventory[16].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(16);
            });
            inventory[17].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[17].GetState() == InventoryCell.State.Empty || (inventory[17].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(17);
            });
            inventory[18].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[18].GetState() == InventoryCell.State.Empty || (inventory[18].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(18);
            });
            inventory[19].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[19].GetState() == InventoryCell.State.Empty || (inventory[19].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(19);
            });
            inventory[20].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[20].GetState() == InventoryCell.State.Empty || (inventory[20].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(20);
            });
            inventory[21].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[21].GetState() == InventoryCell.State.Empty || (inventory[21].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(21);
            });
            inventory[22].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[22].GetState() == InventoryCell.State.Empty || (inventory[22].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(22);
            });
            inventory[23].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[23].GetState() == InventoryCell.State.Empty || (inventory[23].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(23);
            });
            inventory[24].button.Click += new System.EventHandler((object sender, System.EventArgs e) =>
            {
                if (inventory[24].GetState() == InventoryCell.State.Empty || (inventory[24].item.thistype == Item.ItemType.OnceToUse)) return;
                else Equip(24);
            });
            MapHero.Loot(new Item(Image.FromFile(@"propsItems\\st0.png")) { statsGiven = new List<int>() { 1, 1, 1, 1, 1, 1 }, thistype = Item.ItemType.Helmet }); //тестовый
            //
            InventorySync();//Синхронизируем фронт с хранилищем внутри героя
            //
            EquipmentSync();//Синхронизируем фронт со снаряжением внутри героя
            //
            inventory[0].button.KeyPress += new KeyPressEventHandler((object sender, KeyPressEventArgs e) =>
            {
                if (inventory[0].GetState() == InventoryCell.State.Empty)   return;
                else    DeleteItem(0);
            }); //Если вставить цикл - выбрасывает неизвестную ошибку, засорять, как выше, не буду
        }

        /// <summary>
        /// Синхронизирует изображения в инвентаре в соответствии с реальным хранилищем внутри MapHero
        /// </summary>
        public void InventorySync()
        {
            foreach (InventoryCell ic in inventory)
            {
                ic.item = new Item();
                ic.CheckState();
                ic.Block();
            }
            if (MapHero.inventory.Count != 0)

                for (int i = 0; i < MapHero.inventory.Count; i++)
                {
                    inventory[i].item = MapHero.inventory[i];
                    inventory[i].CheckState();
                    inventory[i].Block();
                }            
        }

        /// <summary>
        /// Синхронизирует изображения снаряжения в соответствии с реальным хранилищем внутри MapHero
        /// </summary>
        public void EquipmentSync()
        {
            foreach (PictureBox pb in equipment)
            {
                if (pb.Image!=null)
                pb.Image.Dispose();                
            }
            if (MapHero.equipment!=null)
            if (MapHero.equipment.ContainsKey(Item.ItemType.Helmet) || MapHero.equipment.ContainsKey(Item.ItemType.Armor) || MapHero.equipment.ContainsKey(Item.ItemType.Weapon))
                for (int i = 0; i < 3; i++)
                {
                    if (MapHero.equipment.ContainsKey((Item.ItemType)i))
                    {
                        equipment[i].Image = MapHero.equipment[(Item.ItemType)i].icon;
                    }
                }
        }

        /// <summary>
        /// Удаляет предмет из инвентаря
        /// </summary>
        /// /// <param name="num">Индекс предмета</param>
        public void DeleteItem(int num)
        {
            inventory[num].ChangeState();
            MapHero.ThrowOut(num);
            inventory[num].AnnihilateItem();
            inventory[num].CheckState();
            inventory[num].Block();
            InventorySync();
        }

        /// <summary>
        /// Надевает предмет снаряжения,если это возможно
        /// </summary>
        /// /// <param name="num">Индекс предмета в инвентаре</param>
        public void Equip(int num)
        {
            if (!MapHero.equipment.ContainsKey(inventory[num].item.thistype))
            {
                MapHero.equipment.Add(inventory[num].item.thistype, inventory[num].item);
                equipment[System.Convert.ToInt32(inventory[num].item.thistype)].Image = inventory[num].button.Image;
                for (int i=0;i<6;i++)
                {
                    MapHero.stats[i] += MapHero.equipment[inventory[num].item.thistype].statsGiven[i];
                }
                DeleteItem(num);
            }
            else
            {
                Item temp = new Item(inventory[num].item.icon);
                inventory[num].item = MapHero.equipment[inventory[num].item.thistype]; // изменение в инвентаре фронт
                MapHero.inventory[num] = MapHero.equipment[inventory[num].item.thistype]; // изменение в статик инвентаре
                for (int i = 0; i < 6; i++)
                {
                    MapHero.stats[i] -= MapHero.equipment[inventory[num].item.thistype].statsGiven[i];
                }
                inventory[num].button.Image = equipment[System.Convert.ToInt32(inventory[num].item.thistype)].Image; // Изменение картинки в инвентаре
                MapHero.equipment[inventory[num].item.thistype] = temp; // Изменение в надетых у героя 
                for (int i = 0; i < 6; i++)
                {
                    MapHero.stats[i] += MapHero.equipment[inventory[num].item.thistype].statsGiven[i];
                }
                equipment[System.Convert.ToInt32(inventory[num].item.thistype)].Image = temp.icon; // картинка следом
            }
            InventorySync();
            EquipmentSync();
            StatInit();
        }

        /// <summary>
        /// Меняет видимость инвентаря
        /// </summary>
        /// /// <param name="o">Флаг состояния инвентаря</param>
        public void InvFlag(bool o)
        {
            foreach (InventoryCell ic in inventory)
                ic.button.Visible = o;
        }

        /// <summary>
        /// Открывает доступ к другим рыцарям по мере прогресса
        /// </summary>
        public void LevelCheck()
        {
            foreach (PictureBox ic in knightIcons)
            {
                ic.Enabled = false;
                ic.Image = Image.FromFile(@"Map\\lockedknight.png");
            }
            if (MapHero.levelCount >= 0)
            {
                knightIcons[0].Image = Image.FromFile(@"Map\\ic0.png");
                knightIcons[0].Enabled = true;
            }
            if (MapHero.levelCount >= 2)
            {
                knightIcons[1].Image = Image.FromFile(@"Map\\ic1.png");
                knightIcons[1].Enabled = true;
            }
            if (MapHero.levelCount >= 4)
            {
                knightIcons[2].Image = Image.FromFile(@"Map\\ic2.png");
                knightIcons[2].Enabled = true;
            }
            if (MapHero.levelCount >= 5)
            {
                knightIcons[3].Image = Image.FromFile(@"Map\\ic3.png");
                knightIcons[3].Enabled = true;
            }
        }

        /// <summary>
        /// Задаёт начальные значения всем рыцарям
        /// </summary>
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

        /// <summary>
        /// Синхронизация видимых характеристик рыцаря с реальными значениями
        /// </summary>
        public void StatInit()
        {
            for (int i = 0; i < 6; i++)
                stats[i].Text = $"{MapHero.stats[i]}";
        }

        /// <summary>
        /// Определяет текущего рыцаря и его портрет
        /// </summary>
        /// <returns>Изображение рыцаря</returns>
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

        /// <summary>
        /// Получает информацию о текущих размерах окна
        /// </summary>
        /// <param name="side">Горизонтальный или вертикальный</param>
        /// <returns>Значение ширины/высоты окна</returns>
        int GetBound(bool side)
        {
            if (side)
                return GetWindow().GetSize().Width;
            else
                return GetWindow().GetSize().Height;
        }

        /// <summary>
        /// Загружает элементы управления в общий стэш
        /// </summary>
        public override void Load()
        {
            GetWindow().GetControl().Controls.Add(portrait);
            GetWindow().GetControl().Controls.Add(level);
            for (int i = 0; i < 6; i++)
            {
                GetWindow().GetControl().Controls.Add(stats[i]);
                GetWindow().GetControl().Controls.Add(statIcons[i]);
            }
            for (int i = 0; i < 3; i++)
            {
                GetWindow().GetControl().Controls.Add(equipment[i]);
            }
            for (int i = 0; i < 25; i++)
            {
                GetWindow().GetControl().Controls.Add(inventory[i].button);
            }
            GetWindow().GetControl().Controls.Add(inventoryButton);
            for (int i = 0; i < 4; i++)
            {
                GetWindow().GetControl().Controls.Add(arrowStash[i]);
                GetWindow().GetControl().Controls.Add(knightIcons[i]);
            }
            GetWindow().GetControl().Controls.Add(board);
            GetWindow().GetControl().Controls.Add(board2);                    
            GetWindow().GetControl().Controls.Add(map);
            
        }

        /// <summary>
        /// Разгружает элементы управления из общего стэша
        /// </summary>
        public override void UnLoad()
        {
            GetWindow().GetControl().Controls.Clear();
        }
    }
    #endregion
}
