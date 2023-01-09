using GameEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rogue_JRPG.Frames
{
    public class Test : Frame
    {
        PictureBox map = new PictureBox();
        Random random = new Random();
        public Test(Engine engine) : base(engine)
        {
            controlStash = new List<Control>() { map };

            List<int> stats = new List<int>() { 1,1,1,1,1 };
            MapHero Hero = new MapHero(MapHero.Knight.Electric, stats);

            //map
            Size randSize = new Size(random.Next(100, 500), random.Next(100, 500));
            List<Room> rooms = new List<Room>() { RoomUtility.RoomGen(new Point(100, 100), randSize, Difficulty.Easy) };
            Level lvl = new Level(Level.LevelStyle.Dungeon, rooms);
            RoomUtility.NextRoom(rooms, rooms[0]);
            map = lvl.RoomLoad(lvl.rooms);

            foreach (Room room in lvl.rooms)
                foreach (Enemy e in room.enemies)
                    e.Battle(Hero);
            
            /*foreach (Room room in lvl.rooms)
            {
                Door door = (Door)room.cells.Find(d => d.type == Cell.CellType.DOOR);
                door.Open();
            }*/
                

            Utils.SetDoubleBuffered(map);

            map.Paint += (s, e) =>
            {
                for (int i = 0; i < map.Controls.Count; i++)
                {
                    if (map.Controls[i].GetType() != typeof(PictureBox)) continue;
                    var obj = map.Controls[i] as PictureBox;
                    if (obj.Image == null) continue;
                    obj.Visible = false;
                    e.Graphics.DrawImage(obj.Image, obj.Left, obj.Top, obj.Width, obj.Height);
                }
            };

            Hero.pb.Size = new Size(32, 32);
            Hero.pb.Location = new Point(lvl.rooms[0].Size.Width / 2, lvl.rooms[0].Size.Height / 2);
            map.Controls.Add(Hero.pb);

            //first image
            Hero.LoadAppearances();
            Hero.pb.Image = Utils.Crop(Hero.pb.Image, 0, 0, 32, 32);
            
            //animation
            int counter = 1;
            void HeroAnimation(int offset)
            {
                Hero.LoadAppearances();
                Image sheet = Hero.pb.Image;
                int x = 0;

                //Cмена кадров
                if (counter != 0) x += 32;
                Hero.pb.Image = Utils.Crop(sheet, x, offset, 32, 32);
                counter++;
                if (counter == 2) { x = 0; counter = 0; }
            }

            this.GetWindow().GetForm().KeyDown += new KeyEventHandler(KeyDown);

            void KeyDown(object sender, KeyEventArgs e)
            {
                int dx = 0, dy = 0;

                if (e.KeyCode == Keys.W)
                {
                    HeroAnimation(96); dy = -8;
                }

                else if (e.KeyCode == Keys.A)
                {
                    HeroAnimation(32); dx = -8;
                }

                else if (e.KeyCode == Keys.S)
                {
                    HeroAnimation(0); dy = 8;
                }

                else if (e.KeyCode == Keys.D)
                {
                    HeroAnimation(64); dx = 8;
                }

                Point p = new Point(Hero.pb.Location.X + dx, Hero.pb.Location.Y + dy);
                Point pa= new Point(Hero.pb.Location.X + dx + 32, Hero.pb.Location.Y + dy + 32);
                if (map.Bounds.Contains(pa))
                    Hero.pb.Location = p;
            }
        }        

        public override void Load()
        {
            this.GetWindow().GetControl().Controls.Add(map);
        }

        public override void UnLoad()
        {
            this.GetWindow().GetControl().Controls.Clear();
        }
    }
}
