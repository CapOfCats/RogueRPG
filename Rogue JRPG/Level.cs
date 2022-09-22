using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kursovik3
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
        
        private Random rand = new Random();
        public bool spawned = false;
        
        
        
        //public void Dungeon_Generation()
        //{
        //    rooms.Add(start);
        //    if (spawned == false)
        //    {
        //        if (openingDirection == 1)
        //        {
        //            // Need to spawn a room with a BOTTOM door.
        //            rooms.Add(bottomRooms[rand]);
        //        }
        //        else if (openingDirection == 2)
        //        {
        //            // Need to spawn a room with a TOP door.
        //            rooms.Add(topRooms[rand]);
        //        }
        //        else if (openingDirection == 3)
        //        {
        //            // Need to spawn a room with a LEFT door.
        //            rooms.Add(leftRooms[rand]);
        //        }
        //        else if (openingDirection == 4)
        //        {
        //            // Need to spawn a room with a RIGHT door.
        //            rooms.Add(rightRooms[rand]);
        //        }
        //        spawned = true;
        //    }
        //} 
    }
}
