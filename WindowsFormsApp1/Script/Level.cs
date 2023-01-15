using GameEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rogue_JRPG
{
    /// <summary>
    /// Сущность уровня(подземелья)
    /// </summary>
    class Level
    {
        #region Конструктор
        public Level(LevelStyle _lvlStyle, List<Room>_rooms)
        {
            lvlstyle = _lvlStyle;
            rooms = _rooms;
        }
        #endregion

        #region Поля
        /// <summary>
        /// Карта подземелья
        /// </summary>
        public PictureBox map;
        /// <summary>
        /// Стилистика внешнего вида текущего подземелья
        /// </summary>
        public LevelStyle lvlstyle;
        /// <summary>
        /// Хранилище комнат уровня
        /// </summary>
        public List<Room> rooms;
        /// <summary>
        /// Стилистика внешнего вида уровней
        /// </summary>
        #endregion
        public enum LevelStyle
        {
            Cave,
            Dungeon,
            FrozenCave,
            Jungle,
            Volcano
        }

        #region Методы
        /// <summary>
        /// Запускает генерацию комнат и врагов,задаёт поведение
        /// </summary>
        /// <param name="rooms">Актуальный список комнат, с которым работает генератор</param>
        public PictureBox RoomLoad(List<Room> rooms)
        {
            map = new PictureBox();
            map.Size = new Size(1000,1000);
            map.Location = new Point(100, 100);
            foreach(Room room in rooms)
            {
                foreach (Cell c in room.cells)
                {
                    if (c == null) continue;
                    if (c.type == CellType.DOOR)
                    {
                        c.pb.Image = Utils.Crop(c.appearance[0], 0, 0, 32, 32);
                        c.pb.Size = new Size(32, 32);
                    }
                    else
                    {
                        c.pb.Image = c.appearance[Vars.random.Next(0, c.appearance.Count)];
                        c.pb.Size = new Size(16, 16);
                    }
                    c.pb.Location = c.location;
                    map.Controls.Add(c.pb);
                }
                foreach (Enemy e in room.enemies)
                {
                    if (e == null) continue;
                    //e.pb.Location = new Point(Vars.random.Next(room.position.X + 128, room.Size.Width), Vars.random.Next(room.position.Y + 128, room.Size.Height));
                    e.pb.Size = new Size(e.SizeX, e.SizeY);
                    e.Move(room);
                    map.Controls.Add(e.pb);
                }
            }
            return map;
        }
        #endregion
    }
}
