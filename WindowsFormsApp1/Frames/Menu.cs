using GameEngine;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace Rogue_JRPG.Frames
{
    /// <summary>
    /// Начальное игровое главное меню
    /// </summary>
    public class Main_Menu : Frame
    {
        #region Поля
        /// <summary>
        /// Заголовок
        /// </summary>
        private PictureBox title;
        /// <summary>
        /// Кнопка начала
        /// </summary>
        private PictureBox start;
        #endregion

        #region Конструктор
        public Main_Menu(Engine engine) : base(engine)
        {
            title = Engine.PicCreation(
                new Point(GetWindow().GetSize().Width/2-GetWindow().GetSize().Width/8, GetWindow().GetSize().Height / 4),  // 2-8 4
                new Size (GetWindow().GetSize().Width/4,GetWindow().GetSize().Height/8), // 4 8
                PictureBoxSizeMode.StretchImage,
                Image.FromFile(@"..\\..\\Menu\\title.png"),                
                true
                );
            start = Engine.PicCreation(
                new Point(GetWindow().GetSize().Width / 2 - GetWindow().GetSize().Width / 12, GetWindow().GetSize().Height / 2 - GetWindow().GetSize().Height/12), //2-12 2
                new Size(GetWindow().GetSize().Width / 6, GetWindow().GetSize().Height / 8),//6 8
                PictureBoxSizeMode.StretchImage,
                Image.FromFile(@"..\\..\\Menu\\start.png"),               
                true
                );
            title.Click += (sender, e) =>
            {
                
            };
            start.Click += (sender, e) =>
            {
                engine.LoadFrame("Levelmap");
                engine.DelFrame("Main_Menu");
            };
           
            controlStash = new List<Control>() { title, start };
        }
        #endregion

        #region Методы
        /// <summary>
        /// Загружает все элементы управления в общий стэш
        /// </summary>
        public override void Load()
        {
            GetWindow().GetControl().Paint += Background;
            GetWindow().GetControl().Controls.Add(start);
            GetWindow().GetControl().Controls.Add(title);
        }

        /// <summary>
        /// Разгружает элементы управления из общего стэша
        /// </summary>
        public override void UnLoad()
        {
            GetWindow().GetControl().Paint -= Background;
            GetWindow().GetControl().Controls.Clear();
        }

        /// <summary>
        /// Задаёт цвет заднего фона
        /// </summary>
        private void Background(object sender, PaintEventArgs e)
        {
            GetWindow().GetControl().BackColor = Color.FromArgb(0, 69, 25, 52); //Цвет заднего фона
        }
        #endregion
    }
}
