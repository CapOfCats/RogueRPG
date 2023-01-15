using GameEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rogue_JRPG
{

    /// <summary>
    /// Виды врагов
    /// </summary>
    public enum EnemyType
    {
        SLIME, BAT, BOSS_SKOLL, GOBLIN, ZOMBIE
    }

    /// <summary>
    /// Сущность врага: Слизень
    /// </summary>
    class Slime : Enemy
    {
        #region Конструктор
        public Slime() : base(EnemyType.SLIME)
        {
            this.appearance.Add(Image.FromFile("enemies/slime_run.png"));
            this.frames_num = 6;
            this.offsetX = 0;
            this.offsetY = 0;
            this.SizeX = 16;
            this.SizeY = 16;
        }
        #endregion
    }

    /// <summary>
    /// Сущность врага: Летучая мышь
    /// </summary>
    class Bat : Enemy
    {
        #region Конструктор
        public Bat() : base(EnemyType.BAT)
        {
            this.appearance.Add(Image.FromFile("enemies/fly.png"));
            this.frames_num = 4;
            this.offsetX = 0;
            this.offsetY = 0;
            this.SizeX = 16;
            this.SizeY = 16;
        }
        #endregion
    }

    /// <summary>
    /// Сущность врага: БОСС
    /// </summary>
    class BossSkoll : Enemy
    {
        #region Конструктор
        public BossSkoll() : base(EnemyType.BOSS_SKOLL)
        {
            this.appearance.Add(Image.FromFile("enemies/Skoll.png"));
            this.frames_num = 3;
            this.offsetX = 0;
            this.offsetY = 0;
            this.SizeX = 48;
            this.SizeY = 48;
        }
        #endregion
    }

    /// <summary>
    /// Сущность врага: Гоблин
    /// </summary>
    class Goblin : Enemy
    {
        #region Конструктор
        public Goblin() : base(EnemyType.GOBLIN)
        {
            this.appearance.Add(Image.FromFile("enemies/goblin_run.png"));
            this.frames_num = 6;
            this.offsetX = 0;
            this.offsetY = 0;
            this.SizeX = 16;
            this.SizeY = 16;
        }
        #endregion
    }

    /// <summary>
    /// Сущность врага: Зомби
    /// </summary>
    class Zombie : Enemy
    {
        #region Конструктор
        public Zombie() : base(EnemyType.ZOMBIE)
        {
            this.appearance.Add(Image.FromFile("enemies/zombie.png"));
            this.frames_num = 3;
            this.offsetX = 151;
            this.offsetY = 4;
            this.SizeX = 24;
            this.SizeY = 20;
        }
        #endregion
    }

    /// <summary>
    /// Сущность, представляющая противника
    /// </summary>
    class Enemy
    {
        #region Поля
        /// <summary>
        /// Вид текущего врага
        /// </summary>
        public EnemyType type;
        /// <summary>
        /// Хранилище текстур внешнего вида
        /// </summary>
        public List<Image> appearance;
        /// <summary>
        /// Иконка
        /// </summary>
        public PictureBox pb;
        /// <summary>
        /// Координаты местонахождения
        /// </summary>
        public Point location;
        /// <summary>
        /// Характеристики противника
        /// </summary>
        public List<int> stats; // 0-lvl 1-ATKphys 2-ATKmag 3-DEFPhys 4-DEFMag 5-Health
        /// <summary>
        /// Внутренний таймер
        /// </summary>
        public Timer timer;
        /// <summary>
        /// количество кадров для анимации
        /// </summary>
        public int frames_num;
        /// <summary>
        /// Отступ по горизонтали
        /// </summary>
        public int offsetX;
        /// <summary>
        /// Отступ по вертикали
        /// </summary>
        public int offsetY;
        /// <summary>
        /// Ширина
        /// </summary>
        public int SizeX;
        /// <summary>
        /// Высота
        /// </summary>
        public int SizeY;
        /// <summary>
        /// Номер направления движения
        /// </summary>
        public int direction;
        /// <summary>
        /// Флаг разворота
        /// </summary>
        public bool flip;
        #endregion

        #region Конструктор
        protected Enemy(EnemyType type)
        {
            this.type = type;
            this.appearance = new List<Image>();
            this.pb = new PictureBox();
            this.location = new Point();
            this.stats = new List<int>();

            this.timer = new Timer();
            this.frames_num = 0;
            this.offsetX = this.offsetY = 0;
            this.SizeX = this.SizeY = 0;
            this.direction = Constants.UP;
            this.flip = false;
        }
        #endregion

        #region Методы
        /// <summary>
        /// Метод жизни(движения) противника
        /// </summary>
        /// <param name="room">комната, в которой он находится</param>
        public void Move(Room room)
        {
            Image img = this.appearance[0];
            Image[] frames = new Image[this.frames_num];
            Image[] flipped_frames = new Image[this.frames_num];

            int c = 0;
            for (int i = 0; i < this.frames_num * this.SizeX; i += this.SizeX) 
            {
                int idx = c;
                frames[idx] = Utils.Crop(img, i + this.offsetX, this.offsetY, this.SizeX, this.SizeY);
                flipped_frames[idx] = (Image) frames[idx].Clone();
                flipped_frames[idx].RotateFlip(RotateFlipType.RotateNoneFlipX);
                c++;
            }
            
            int counter = 0;
            this.timer.Interval = 1000;
            this.timer.Start();
            this.timer.Tick += (e, sender) =>
            {
                int x = this.pb.Location.X;
                int y = this.pb.Location.Y;

                List<Point> pts = new List<Point>() { 
                    new Point(x, y - 16),
                    new Point(x - 16, y),
                    new Point(x, y + 16),
                    new Point(x + 16, y)
                };

                bool h(Point a, List<Cell> cells)
                {
                    foreach (Cell cell in cells)
                    {
                        if (cell.type == CellType.FLOOR || cell.type == CellType.DOOR)
                        {
                            if (cell.pb.Bounds.Contains(a))
                                return true;
                        }
                    }
                    return false;
                }

                Point p;
                int ran_direction;
                do
                {
                    ran_direction = Vars.random.Next(1, pts.Count);
                    p = pts[ran_direction - 1];
                    if (!h(p, room.cells))
                    {
                        pts.RemoveAt(ran_direction - 1);
                    }
                    break;
                }
                while (pts.Count != 0);

                if (ran_direction != direction) flip = !flip;

                //Атлас
                this.direction = ran_direction;
                this.pb.Image = flip ? flipped_frames[counter] : frames[counter];
                counter++;
                if (this.frames_num == counter) counter = 0;
            };
        }

        /// <summary>
        /// Начинает битву с рыцарем
        /// </summary>
        /// <param name="Hero">Экземпляр игрока</param>
        public void Battle(MapHero Hero)
        {
            if (this.pb.Bounds.IntersectsWith(Hero.pb.Bounds))
                this.timer.Stop(); //Сюда фрейм батлы
        }
        #endregion
    }
}
