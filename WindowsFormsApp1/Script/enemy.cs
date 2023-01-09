using GameEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rogue_JRPG
{

    public enum EnemyType
    {
        SLIME, BAT, BOSS_SKOLL, GOBLIN, ZOMBIE
    }

    class Slime : Enemy
    {
        public Slime() : base(EnemyType.SLIME)
        {
            this.appearance.Add(Image.FromFile("enemies/slime_run.png"));
            this.frames_num = 6;
            this.offsetX = 0;
            this.offsetY = 0;
            this.SizeX = 16;
            this.SizeY = 16;
        }
    }

    class Bat : Enemy
    {
        public Bat() : base(EnemyType.BAT)
        {
            this.appearance.Add(Image.FromFile("enemies/fly.png"));
            this.frames_num = 4;
            this.offsetX = 0;
            this.offsetY = 0;
            this.SizeX = 16;
            this.SizeY = 16;
        }
    }

    class BossSkoll : Enemy
    {
        public BossSkoll() : base(EnemyType.BOSS_SKOLL)
        {
            this.appearance.Add(Image.FromFile("enemies/Skoll.png"));
            this.frames_num = 3;
            this.offsetX = 0;
            this.offsetY = 0;
            this.SizeX = 48;
            this.SizeY = 48;
        }
    }

    class Goblin : Enemy
    {
        public Goblin() : base(EnemyType.GOBLIN)
        {
            this.appearance.Add(Image.FromFile("enemies/goblin_run.png"));
            this.frames_num = 6;
            this.offsetX = 0;
            this.offsetY = 0;
            this.SizeX = 16;
            this.SizeY = 16;
        }
    }

    class Zombie : Enemy
    {
        public Zombie() : base(EnemyType.ZOMBIE)
        {
            this.appearance.Add(Image.FromFile("enemies/zombie.png"));
            this.frames_num = 3;
            this.offsetX = 151;
            this.offsetY = 4;
            this.SizeX = 24;
            this.SizeY = 20;
        }
    }

    class Enemy
    {
        
        public EnemyType type;
        public List<Image> appearance;
        public PictureBox pb;
        public Point location;
        public List<int> stats; // 0-lvl 1-ATKphys 2-ATKmag 3-DEFPhys 4-DEFMag 5-Health
        
        public Timer timer;

        public int frames_num;
        public int offsetX;
        public int offsetY;
        public int SizeX;
        public int SizeY;
        public int direction;
        public bool flip;
       

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

        public void Battle(MapHero Hero)
        {
            if (this.pb.Bounds.IntersectsWith(Hero.pb.Bounds))
                this.timer.Stop(); //Сюда фрейм батлы
        }
    }
}
