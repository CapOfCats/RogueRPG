using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rogue_JRPG
{

    public enum Difficulty
    {
        Easy,
        Medium,
        Hell
    }

    class RoomUtility
    {
        public static Room RoomGen(Point start, Size size, Difficulty difficulty)
        {
            Room room = new Room(difficulty) { position = start, Size = size };

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

            int randX = Vars.random.Next(32, room.Size.Width - 32);
            int randY = Vars.random.Next(32, room.Size.Height - 32);
            int maxX = room.Size.Width;
            int maxY = room.Size.Height;

            Dictionary<int, Tuple<int, int>> dir_vars = new Dictionary<int, Tuple<int, int>>()
            {
                { Constants.UP, new Tuple<int, int>(randX, -32) },
                { Constants.LEFT, new Tuple<int, int>(-32, randY) },
                { Constants.DOWN, new Tuple<int, int>(randX, maxY+32) },
                { Constants.RIGHT, new Tuple<int, int>(maxX+32, randY) }
            };

            int ran_direction = Vars.random.Next(1, 5);
            Tuple<int, int> vars = dir_vars[ran_direction];
            int X = vars.Item1, Y = vars.Item2;
            room.cells.Add(new Door() { location = new Point(X, Y), direction = ran_direction});

            EnemyGen(room);
            return room;
        }

        public static List<Room> NextRoom(List<Room> rooms, Room start)
        {
            Size randSize = new Size(Vars.random.Next(100, 500), Vars.random.Next(100, 500));
            int maxX = randSize.Width;
            int maxY = randSize.Height;

            Dictionary<int, Tuple<int, int>> dir_vars = new Dictionary<int, Tuple<int, int>>()
            {
                { Constants.UP, new Tuple<int, int>(-maxX/2, -maxY) },
                { Constants.LEFT, new Tuple<int, int>(-maxX, -maxY/2) },
                { Constants.DOWN, new Tuple<int, int>(-maxX/2, 32) },
                { Constants.RIGHT, new Tuple<int, int>(32, -maxY/2) }
            };

            Door door = (Door)start.cells.Find(d => d.type == CellType.DOOR);
            door.DoorDirection();
            
            Tuple<int, int> vars = dir_vars[door.direction];
            int dx = vars.Item1, dy = vars.Item2;
            int x = door.location.X; int y = door.location.Y;

            rooms.Add(RoomGen(
                 new Point(x + dx, y + dy),
                 randSize,
                 start.thisDifficulty));
            return rooms;
        }

        public static void EnemyGen(Room room)
        {
            switch (room.thisDifficulty)//здесь пресеты комнаты в зависимости от параметров. Кейзы будут дополняться - сделаем всё в одном свиче
            {
                default:
                case Difficulty.Easy: room.enemyNumber = 1; break;
                case Difficulty.Medium: room.enemyNumber = 2; break;
                case Difficulty.Hell: room.enemyNumber = 3; break;
            }

            Array values = Enum.GetValues(typeof(EnemyType));

            for (int i = 0; i < room.enemyNumber; i++)
            {
                EnemyType randomEnemyType = (EnemyType)values.GetValue(Vars.random.Next(values.Length));
                Enemy enemy = null;
                switch (randomEnemyType)
                {
                    case EnemyType.SLIME:       enemy = new Slime(); break;
                    case EnemyType.BAT:         enemy = new Bat(); break;
                    case EnemyType.GOBLIN:      enemy = new Goblin(); break;
                    case EnemyType.ZOMBIE:      enemy = new Zombie(); break;
                    case EnemyType.BOSS_SKOLL:  enemy = new BossSkoll(); break;
                    default: break;
                }
                room.enemies.Add(enemy);
            }
        }
    }

    class Room
    {

        public Difficulty thisDifficulty;
        public List<Cell> cells;
        public List<Enemy> enemies;
        public bool spawnedBoss;
        public int enemyNumber;
        public int doorNumber;

        public Size Size;
        public Point position;

        public Room(Difficulty _thisDifficulty)
        {
            this.thisDifficulty = _thisDifficulty;
            this.cells = new List<Cell>();
            this.enemies = new List<Enemy>();
            this.spawnedBoss = false;
            this.enemyNumber = 0;
            this.doorNumber = 0;
        }
    }
}
