using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rogue_JRPG
{
    class Room
    {
        public Room(Difficulty _thisDifficulty)//cells, возможно, заменим двумерным массивом
        {
            thisDifficulty = _thisDifficulty;
        }

        public enum Difficulty
        {
            Easy,
            Medium,
            Hell
        }
        //public List<PictureBox> textures;
        public Difficulty thisDifficulty;
        public List<Cell> cells = new List<Cell>();
        public List<Enemy> enemies = new List<Enemy>();
        public int enemyNumber;
        public int doorNumber;

        public bool spawnedBoss = false;
        public Size Size;
        public Point position;
        public const int UP = 1;
        public const int LEFT = 2;
        public const int DOWN = 3;
        public const int RIGHT = 4;

        public Room RoomGen(Point start, Difficulty dif)
        {
            Random r = new Random();
            Size randSize = new Size(r.Next(100, 500), r.Next(100, 500));
            Room room = new Room(dif) { position = start, Size = randSize};
           
            int x = 0; int y = 0;
            for (int i = 0; i < room.Size.Height / 16; i++)
            {
               for (int j = 0; j < room.Size.Width / 16; j++)
                   {
                   room.cells.Add(new Floor() { location = new Point(start.X + x, start.Y + y) });
                   x += 16;
                   }
               y += 16; x = 0;
            }

            int randX = r.Next(32, room.Size.Width - 32);
            int randY = r.Next(32, room.Size.Height - 32);
            int maxX = room.Size.Width;
            int maxY = room.Size.Height;

            Dictionary<int, Tuple<int, int>> dir_vars = new Dictionary<int, Tuple<int, int>>()
            {
                { UP, new Tuple<int, int>(randX, -32) },
                { LEFT, new Tuple<int, int>(-32, randY) },
                { DOWN, new Tuple<int, int>(randX, maxY+32) },
                { RIGHT, new Tuple<int, int>(maxX+32, randY) }
            };

            int ran_direction = r.Next(1, 5);
            Tuple<int, int> vars = dir_vars[ran_direction];
            int X = vars.Item1, Y = vars.Item2;
            room.cells.Add(new Door() { location = new Point(X, Y), direction = ran_direction});

            EnemyGen(room);
            return room;
        }

        public List<Room> NextRoom(List<Room> rooms, Room start)
        {
            int maxX = start.Size.Width;
            int maxY = start.Size.Height;

            Dictionary<int, Tuple<int, int>> dir_vars = new Dictionary<int, Tuple<int, int>>()
            {
                { UP, new Tuple<int, int>(-maxX/2, -maxY) },
                { LEFT, new Tuple<int, int>(-maxX, -maxY/2) },
                { DOWN, new Tuple<int, int>(-maxX/2, 32) },
                { RIGHT, new Tuple<int, int>(32, -maxY/2) }
            };

            Cell door = start.cells.Find(d => d.type == Cell.CellType.DOOR);
            Tuple<int, int> vars = dir_vars[door.direction];
            int dx = vars.Item1, dy = vars.Item2;

            
            int x = door.location.X;
            int y = door.location.Y;
            
            rooms.Add(RoomGen(
                 new Point(x+dx, y+dy),
                 start.thisDifficulty));
            return rooms;
        }

        public void EnemyGen(Room room)
        {
            switch (thisDifficulty)//здесь пресеты комнаты в зависимости от параметров. Кейзы будут дополняться - сделаем всё в одном свиче
            {
                case Difficulty.Easy: enemyNumber = 1; break;
                case Difficulty.Medium: enemyNumber = 2; break;
                case Difficulty.Hell: enemyNumber = 3; break;
                default: break;
            }

            Array values = Enum.GetValues(typeof(Enemy.EnemyType));
            Random random = new Random();

            for (int i = 0; i<enemyNumber; i++)
            {
                Enemy.EnemyType randomEnemy = (Enemy.EnemyType)values.GetValue(random.Next(values.Length));
                enemies.Add(new Enemy(randomEnemy));
            }
        }

    }
}
