using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameEngine;

namespace Rogue_JRPG
{

    public class Cell
    {
        public enum CellType
        {
            UNKNOWN, WALL, FLOOR, DOOR
        }

        public static List<Image> Wall_apeareance = new List<Image>()
        {
            Image.FromFile("tiles/wall/wall_1.png"),
            Image.FromFile("tiles/wall/wall_2.png"),
            Image.FromFile("tiles/wall/wall_3.png"),
            Image.FromFile("tiles/wall/wall_crack.png")
        };
        public static List<Image> Floor_appeareance = new List<Image>()
        {
            Image.FromFile("tiles/floor/floor_1.png"),
            Image.FromFile("tiles/floor/floor_2.png"),
            Image.FromFile("tiles/floor/floor_3.png"),
            Image.FromFile("tiles/floor/floor_4.png"),
            Image.FromFile("tiles/floor/floor_5.png"),
            Image.FromFile("tiles/floor/floor_6.png"),
            Image.FromFile("tiles/floor/floor_7.png"),
            Image.FromFile("tiles/floor/floor_8.png"),
            Image.FromFile("tiles/floor/floor_9.png"),
            Image.FromFile("tiles/floor/floor_10.png")
        };

        public static List<Image> Door_appearance = new List<Image>()
        {
           Image.FromFile("tiles/wall/door_spritesheet.png")
        };


        public CellType type;
        public bool block = false;
        public bool interact = false; //флаги
        public List<Image> appearance = new List<Image>(); //дименшны и текстура
        public PictureBox pb = new PictureBox();
        public Point location;
        public int direction;

        public Cell() {}
        public Cell(CellType type)
        {
            this.type = type;
            switch (type)
            {
                case CellType.FLOOR:
                    block = false;
                    interact = false;
                    appearance = Floor_appeareance;
                    break;
                case CellType.WALL:
                    block = true;
                    interact = false;
                    appearance = Wall_apeareance;
                    break;
                case CellType.DOOR:
                    block = true;
                    interact = true;
                    appearance = Door_appearance;
                    break;
                case CellType.UNKNOWN:
                default: break;
            }
        }
    }

    class Wall : Cell
    {
        public Wall() : base(CellType.WALL) { }
    }

    class Floor : Cell
    {
        public Floor() : base(CellType.FLOOR) { }
    }

    class Door : Cell
    {
        public Door() : base(CellType.DOOR) { DoorDirection(); } //хз можно ли так
        public void DoorDirection()
        {
            const int UP = 1;
            const int LEFT = 2;
            const int DOWN = 3;
            const int RIGHT = 4;
            Image door = appearance[0];
            switch (direction)
            {
                case UP: break;
                case LEFT: door.RotateFlip(RotateFlipType.Rotate270FlipNone); break;
                case DOWN: door.RotateFlip(RotateFlipType.Rotate180FlipNone); break;
                case RIGHT: door.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
            }
            appearance[0] = door;
        }
        public void Open()
        {
            for (int i = 0; i < 13 * 32; i += 32)
                pb.Image = Utils.Crop(appearance[0], i, 0, 32, 32);
        }
    }
}
