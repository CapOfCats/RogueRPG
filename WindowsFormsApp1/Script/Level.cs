using GameEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rogue_JRPG
{
    class Level
    {
        public Level(LevelStyle _lvlStyle, List<Room>_rooms)
        {
            lvlstyle = _lvlStyle;
            rooms = _rooms;
        }
        public LevelStyle lvlstyle;
        public List<Room> rooms;
        public enum LevelStyle
        {
            Cave,
            Dungeon,
            FrozenCave,
            Jungle,
            Volcano
        }

        public PictureBox RoomLoad(List<Room> rooms)
        {
            PictureBox pb = new PictureBox();
            pb.Size = new Size(1000, 1000);
            pb.Location = new Point(100, 100);
            Random r = new Random();
            foreach(Room room in rooms)
            {
                foreach (Cell c in room.cells)
                {
                    if (c == null) continue;
                    if (c.type == Cell.CellType.DOOR)
                    {
                        c.pb.Image = Utils.Crop(c.appearance[0], 0, 0, 32, 32);
                        c.pb.Size = new Size(32, 32);
                    }
                    else
                    {
                        c.pb.Image = c.appearance[r.Next(0, c.appearance.Count)];
                        c.pb.Size = new Size(16, 16);
                    }
                    c.pb.Location = c.location;
                    pb.Controls.Add(c.pb);
                }
                foreach (Enemy e in room.enemies)
                {
                    if (e == null) continue;
                    e.pb.Location = new Point(r.Next(0, room.Size.Width), r.Next(0, room.Size.Height));
                    e.pb.Size = new Size(e.SizeX, e.SizeY);
                    e.Move();
                    pb.Controls.Add(e.pb);
                }
            }
            return pb;
        }
    }
}
