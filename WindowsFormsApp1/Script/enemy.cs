using GameEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rogue_JRPG
{
    class Enemy
    {
        public enum EnemyType
        {
           SLIME, BAT, BOSS_SKOLL, GOBLIN, ZOMBIE
        }
        
        public EnemyType type;
        public List<Image> appearance = new List<Image>(); //дименшны и текстура
        public PictureBox pb = new PictureBox();
        public Point loc;
        public List<int> stats; // 0-lvl 1-ATKphys 2-ATKmag 3-DEFPhys 4-DEFMag 5-Health
        public Timer timer = new Timer();
        public int frames_num;
        public int offsetX;
        public int offsetY;
        public int SizeX;
        public int SizeY;
        public int direction;
        public bool flip = false;
        
        public const int UP = 1;
        public const int LEFT = 2;
        public const int DOWN = 3;
        public const int RIGHT = 4;

        public Enemy(EnemyType type)
        {
            this.type = type;
            switch (type)
            {
                case EnemyType.SLIME:
                    appearance.Add(Image.FromFile("enemies/slime_run.png"));
                    frames_num = 6; offsetX = 0; offsetY = 0; SizeX = 16; SizeY = 16;
                    break;
                case EnemyType.BAT:
                    appearance.Add(Image.FromFile("enemies/fly.png"));
                    frames_num = 4; offsetX = 0; offsetY = 0; SizeX = 16; SizeY = 16;
                    break;
                case EnemyType.GOBLIN:
                    appearance.Add(Image.FromFile("enemies/goblin_run.png"));
                    frames_num = 6; offsetX = 0; offsetY = 0; SizeX = 16; SizeY = 16;
                    break;
                case EnemyType.ZOMBIE:
                    appearance.Add(Image.FromFile("enemies/zombie.png"));
                    frames_num = 3; offsetX = 151; offsetY = 4; SizeX = 12; SizeY = 20;
                    break;
                case EnemyType.BOSS_SKOLL:
                    appearance.Add(Image.FromFile("enemies/Skoll.png"));
                    frames_num = 3; offsetX = 0; offsetY = 0; SizeX = 48; SizeY = 48;
                    break;
                default: break;
            }
        }

        public void Move()
        {
            Random r = new Random();
            Image img = appearance[0];
            Image[] frames = new Image[frames_num];
            Image[] flipped_frames = new Image[frames_num];

            int c = 0;
            for (int i = 0; i < frames_num * SizeX; i += SizeX) 
            {
                int idx = c;
                frames[idx] = Utils.Crop(img, i + offsetX, offsetY, SizeX, SizeY);
                flipped_frames[idx] = (Image) frames[idx].Clone();
                flipped_frames[idx].RotateFlip(RotateFlipType.RotateNoneFlipX);
                c++;
            }
            
            int counter = 0;
            timer.Interval = 1000;
            timer.Start();
            timer.Tick += (e, sender) =>
            {
                int x = pb.Location.X;
                int y = pb.Location.Y;
                int ran_direction = r.Next(1, 5);
                switch (ran_direction)
                {
                    case UP: pb.Location = new Point(x, y - SizeY); break;
                    case LEFT: pb.Location = new Point(x - SizeX, y); break; 
                    case DOWN: pb.Location = new Point(x, y + SizeY); break;
                    case RIGHT: pb.Location = new Point(x + SizeX, y); break;
                }
                if (ran_direction != direction) flip = !flip;

                //Атлас
                direction = ran_direction;
                pb.Image = flip ? flipped_frames[counter] : frames[counter];
                counter++;
                if (counter == frames_num) counter = 0;
            };
        }

        public void Battle(MapHero Hero)
        {
            if (pb.Bounds.IntersectsWith(Hero.pb.Bounds))
                timer.Stop(); //Сюда фрейм батлы
        }
    }
}
