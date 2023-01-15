using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameEngine;

namespace Rogue_JRPG
{
    /// <summary>
    /// Типы ячеек
    /// </summary>
    public enum CellType
    {
        UNKNOWN, WALL, FLOOR, DOOR
    }

    /// <summary>
    /// Сущность ячейки, которыми заполняется комната
    /// </summary>
    public class Cell
    {
        #region Конструкторы
        static Cell()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Start();
        }
        protected Cell(CellType type)
        {
            this.type = type;
            this.block = false;
            this.interact = false;
            this.appearance = new List<Image>();
            this.pb = new PictureBox();
            this.location = new Point();
            this.direction = Constants.UP;
        }
        #endregion

        #region Поля
        /// <summary>
        /// Внутренний таймер
        /// </summary>
        private static Timer timer;
        /// <summary>
        /// Тип данной ячейки
        /// </summary>
        public CellType type;
        /// <summary>
        /// Флаг проходимости
        /// </summary>
        public bool block;
        /// <summary>
        /// Флаг взаимодействия
        /// </summary>
        public bool interact;
        /// <summary>
        /// Хранилище текстур внешнего вида ячейки
        /// </summary>
        public List<Image> appearance;
        /// <summary>
        /// Актуальный образ ячейки
        /// </summary>
        public PictureBox pb;
        /// <summary>
        /// Расположение
        /// </summary>
        public Point location;
        /// <summary>
        /// Направление отрисовки
        /// </summary>
        public int direction;
        #endregion

        public Timer getTimer() => timer;

    }

    /// <summary>
    /// Дочерняя сущность ячейки: Стена
    /// </summary>
    class Wall : Cell
    {
        #region Поля
        /// <summary>
        /// Хранилище загруженных текстур ячейки
        /// </summary>
        public static List<Image> Wall_apeareance = new List<Image>()
        {
            Image.FromFile("tiles/wall/wall_1.png"),
            Image.FromFile("tiles/wall/wall_2.png"),
            Image.FromFile("tiles/wall/wall_3.png"),
            Image.FromFile("tiles/wall/wall_crack.png")
        };
        #endregion

        #region Конструктор
        public Wall() : base(CellType.WALL)
        {
            this.block = true;
            this.interact = false;
            this.appearance = Wall_apeareance;
        }
        #endregion
    }

    /// <summary>
    /// Дочерняя сущность ячейки: Пол
    /// </summary>
    class Floor : Cell
    {
        #region Поля
        /// <summary>
        /// Хранилище загруженных текстур ячейки
        /// </summary>
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
        #endregion

        #region Конструктор
        public Floor() : base(CellType.FLOOR)
        {
            this.block = false;
            this.interact = false;
            this.appearance = Floor_appeareance;
        }
        #endregion
    }

    /// <summary>
    /// Дочерняя сущность ячейки: Дверь
    /// </summary>
    class Door : Cell
    {
        #region Поля
        /// <summary>
        /// Хранилище загруженных текстур ячейки
        /// </summary>
        public static List<Image> Door_appearance = new List<Image>()
        {
           Image.FromFile("tiles/wall/door_spritesheet.png")
        };
        #endregion

        #region Конструктор
        public Door() : base(CellType.DOOR)
        {
            this.block = true;
            this.interact = true;
            this.appearance = Door_appearance;
        }
        #endregion

        #region Методы
        /// <summary>
        /// Определяет и задаёт направление двери
        /// </summary>
        public void DoorDirection()
        {
            
            Image door = appearance[0];
            switch (this.direction)
            {
                case Constants.UP:      break;
                case Constants.LEFT:    door.RotateFlip(RotateFlipType.Rotate270FlipNone); break;
                case Constants.DOWN:    door.RotateFlip(RotateFlipType.Rotate180FlipNone); break;
                case Constants.RIGHT:   door.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
            }
            appearance[0] = door;
        }

        /// <summary>
        /// Взаимодействует с дверью(открытие)
        /// </summary>
        public void Open()
        {
            List<Image> frames = new List<Image>();
            switch (this.direction) //доделать
            {
                case Constants.UP:
                    for (int i = 0; i < 13 * 32; i += 32)
                        frames.Add(Utils.Crop(appearance[0], i, 0, 32, 32));
                    break;
                case Constants.LEFT:
                    for (int i = 13 * 32; i < 0; i -= 32)
                        frames.Add(Utils.Crop(appearance[0], 0, 0, 32, 32));
                    break;
                case Constants.DOWN:
                    for (int i = 13 * 32; i < 0; i -= 32)
                        frames.Add(Utils.Crop(appearance[0], i, 0, 32, 32));
                    break;
                case Constants.RIGHT:
                    for (int i = 13 * 32; i < 0; i -= 32)
                        frames.Add(Utils.Crop(appearance[0], 0, i, 32, 32));
                    break;
            }

            this.getTimer().Tick += (e, sender) =>
            {
                for (int c = 0; c < frames.Count; c++) pb.Image = frames[c];
            };
        }
        #endregion
    }
}
