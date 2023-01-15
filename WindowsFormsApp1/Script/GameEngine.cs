using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GameEngine
{

    /// <summary>
    /// Набор общих инструментов
    /// </summary>
    public class Utils
    {
        #region Методы
        /// <summary>
        /// Пропускает элемент управления через алгоритм двойной буферизации
        /// </summary>
        /// <param name="control">Элемент, подвергающийся обработке</param>
        public static void SetDoubleBuffered(Control control)
        {
            if (SystemInformation.TerminalServerSession)
            {
                return; // Remote Desktop
            }

            System.Reflection.PropertyInfo props =
                typeof(Control).GetProperty(
                    "DoubleBuffered",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

            props.SetValue(control, true, null);
        }

        /// <summary>
        /// Изменение размеров изображения
        /// </summary>
        /// <param name="src">Исходное изображение</param>
        /// <param name="size">Задаваемый размер</param>
        public static Bitmap Resize(Image src, Size size)
        {
                int srcWidth = src.Width;
                int srcHeigth = src.Height;
                float nPercent = 0;
                float nPercentW = 0;
                float nPercentH = 0;
                nPercentW = ((float)size.Width / (float)srcWidth);
                nPercentH = ((float)size.Height / (float)srcHeigth);
                nPercent = (nPercentW > nPercentH) ? nPercentH : nPercentW; // Ternary Expression
                int destWidth = (int)(srcWidth * nPercent);
                int destHeigth = (int)(srcHeigth * nPercent);
                Bitmap bitmap = new Bitmap(destWidth, destHeigth);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(src, 0, 0, destWidth, destHeigth);
                graphics.Dispose();
                return bitmap;
        }

        /// <summary>
        /// Обрезает изображение
        /// </summary>
        /// <param name="img">Изображение</param>
        /// <param name="offsetX">Отступ по горизонтали</param>
        /// <param name="offsetY">Отступ по вертикали</param>
        /// <param name="w">Ширина обрезки</param>
        /// <param name="h">Высота обрезки</param>
        public static Image Crop(Image img, int offsetX, int offsetY, int w, int h)
        {
            Rectangle cropArea = new Rectangle();
            cropArea.X = offsetX; cropArea.Y = offsetY; cropArea.Width = w; cropArea.Height = h;
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }
        #endregion
    }

    /// <summary>
    /// Режимы отображения окна
    /// </summary>
    public enum WindowState
    {
        Unknown,
        Windowed,
        Borderless,
        Fullscreen
    }

    /// <summary>
    /// Сущность, представляющая окно
    /// </summary>
    public class Window
    {
        #region Поля
        /// <summary>
        /// Экземпляр формы
        /// </summary>
        public Form mainForm;
        /// <summary>
        /// Экземпляр панели
        /// </summary>
        private Panel panel;
        /// <summary>
        /// Экземпляр экрана
        /// </summary>
        private Screen screen;
        //
        /// <summary>
        /// Текущее состояние окна
        /// </summary>
        public WindowState windowState;
        /// <summary>
        /// Хранилище элементов управления
        /// </summary>
        public List<Control> controls;
        //
        #endregion

        #region Конструктор
        public Window(Form form)
        {
            screen = System.Windows.Forms.Screen.PrimaryScreen;
            mainForm = form;
            panel = new Panel();
            //Setup();
            mainForm.SuspendLayout(); // Edit Mode : ON
            mainForm.AutoScaleDimensions = new SizeF(6F, 13F);
            mainForm.AllowTransparency = true;
            mainForm.FormBorderStyle = FormBorderStyle.FixedSingle; //НЕ ПАШЕТ
            mainForm.WindowState = FormWindowState.Normal;
            mainForm.MaximizeBox = false;
            mainForm.MinimizeBox = false;
            SetLocation(new Point(0, 0));
            Utils.SetDoubleBuffered(panel);
            mainForm.ResumeLayout(false); // Edit Mode : OFF
            mainForm.Invalidate();
            //            
            SetWindowState(WindowState.Fullscreen);
            mainForm.Controls.Add(panel);
            
        }
        #endregion

        #region Методы
        /// <summary>
        /// Получает размеры формы
        /// </summary>
        /// <returns>Актуальные размеры формы</returns>
        public Size GetSize()
        {
            return mainForm.Size;
        }

        /// <summary>
        /// Получает размеры окна
        /// </summary>
        /// <returns>Актуальные размеры окна</returns>
        public Size GetScreenSize()
        {
            return new Size(screen.Bounds.Width, screen.Bounds.Height);
        }

        /// <summary>
        /// Выполняет полную перерисовку формы
        /// </summary>
        public void Refresh()
        {
            panel.Refresh();
        }

        /// <summary>
        /// Задаёт положение окна(панели)
        /// </summary>
        /// <param name="location">Положение, которое задаётся</param>
        public void SetLocation(Point location)
        {
            panel.Location = location;
            //
            mainForm.Invalidate();
        }

        /// <summary>
        /// Задаёт размеры элементам управления, исходя из размеров окна
        /// </summary>
        /// <param name="controls">Актуальное хранилище элементов управления</param>
        public void SetSize(ref List<Control> controls)
        {
            
            if (controls != null)         
                for (int i = 0; i < controls.Count; i++)
                {
                    controls[i].Size = ControlResize(controls[i]);
                    controls[i].Location = ControlRelocation(controls[i]);
                }                    
            //
            mainForm.Invalidate();
        }

        /// <summary>
        /// Задаёт режим отображения окна и меняет размеры следом
        /// </summary>
        /// <param name="state">Задаваемый режим отображения</param>
        public void SetWindowState(WindowState state)
        {
            if (windowState == state)
            {
                return;
            }
            switch (state)
            {
                case WindowState.Borderless:
                case WindowState.Fullscreen:
                    mainForm.FormBorderStyle = FormBorderStyle.None;
                    mainForm.WindowState = FormWindowState.Maximized;
                    mainForm.ClientSize=(new Size(GetScreenSize().Width, GetScreenSize().Height)); //
                    mainForm.Size = (new Size(GetScreenSize().Width, GetScreenSize().Height));
                    break;
                case WindowState.Windowed:
                    mainForm.FormBorderStyle = FormBorderStyle.Sizable;
                    mainForm.WindowState = FormWindowState.Normal;
                    mainForm.ClientSize=(new Size(GetScreenSize().Width/2, GetScreenSize().Height/2));//
                    mainForm.Size = (new Size(GetScreenSize().Width / 2, GetScreenSize().Height / 2)); //+gss/38
                    break;
                default:
                    return;
            }
            panel.ClientSize = GetSize();
            panel.Size = GetSize();
            windowState = state;
            mainForm.Invalidate();
        }

        /// <summary>
        /// Изменяет размеры 1 элемента управления, опираясь на текущие размеры окна
        /// </summary>
        /// <param name="c">Входящий ЭУ</param>
        /// <returns>Итоговый размер</returns>
        public Size ControlResize(Control c)
        {
            if (windowState == WindowState.Windowed)
                return new Size(c.Width / 2, c.Height / 2);
            else
                return new Size(c.Width * 2, c.Height * 2);
        }

        /// <summary>
        /// Изменяет расположение 1 элемента управления, опираясь на текущие размеры окна
        /// </summary>
        /// <param name="c">Входящий ЭУ</param>
        /// <returns>Итоговое положение</returns>
        public Point ControlRelocation(Control c)
        {
            if (windowState == WindowState.Windowed)
                return new Point(c.Location.X / 2, c.Location.Y / 2);
            else
                return new Point(c.Location.X * 2, c.Location.Y * 2);
        }

        /// <summary>
        /// Даёт доступ к экземпляру формы
        /// </summary>
        /// <returns>Актуальный экземпляр формы</returns>
        public Form GetForm()
        {
            return mainForm;
        }

        /// <summary>
        /// Даёт доступ к экземпляру панели
        /// </summary>
        /// <returns>Актуальный экземпляр панели</returns>
        public Control GetControl()
        {
            return panel;
        }
        #endregion
    }

    /// <summary>
    /// Сущность, представляющая текущий тип игрового окна, относящийся к разным частям игры(карта,подземелье,бой)
    /// </summary>
    public abstract class Frame
    {
        #region Поля
        /// <summary>
        /// Экземпляр используемого движка
        /// </summary>
        public Engine engine;
        /// <summary>
        /// Хранилище элементов управления на фрейме
        /// </summary>
        public List<Control> controlStash;
        #endregion

        #region Конструктор
        public Frame(Engine engine)
        {
            this.engine = engine;
        }
        #endregion

        #region Методы
        /// <summary>
        /// Даёт доступ к актуальному экземпляру окна
        /// </summary>
        /// <returns>Экземпляр окна</returns>
        public Window GetWindow()
        {
            return engine.window;
        }
        public abstract void Load();

        public abstract void UnLoad();
        #endregion
    }

    /// <summary>
    /// Сущность, представляющая игровой движок
    /// </summary>
    public class Engine
    {
        #region Поля
        /// <summary>
        /// Экземпляр окна
        /// </summary>
        public Window window;
        /// <summary>
        /// Экземпляр таймера
        /// </summary>
        public Timer timer;
        /// <summary>
        /// Хранилище фреймов
        /// </summary>
        public Dictionary<string, Frame> frames;
        /// <summary>
        /// Название текущего фрейма
        /// </summary>
        public string currentFrame;
        #endregion

        #region Конструктор
        public Engine(Form form)
        {
            window = new Window(form);
            window.GetControl().DoubleClick += (sender, e) => ToggleWindowState();
            //
           timer = new Timer();
           timer.Interval = 5;
            timer.Tick += (sender, e) =>
            {
                window.Refresh();
            };
           timer.Enabled = true;
            //
            frames = new Dictionary<string, Frame>();
            currentFrame = "None";

        }
        #endregion

        #region Методы
        /// <summary>
        /// Меняет игровое окно
        /// </summary>
        /// <param name="str">Название игрового окна</param>
        public void LoadFrame(string str)
        {
            
            if (frames.Count == 0 || !frames.ContainsKey(str))
            {
                return; // Dictionary is empty OR No Frame exist with the key str
            }
            frames[str].UnLoad();
            currentFrame = "None";
            window.GetForm().Invalidate();
            frames[str].Load();
            currentFrame = str;
            window.GetForm().Invalidate();
            window.controls = new List<Control>();
            window.controls.AddRange(frames[str].controlStash);
            if (window.windowState == WindowState.Windowed)
                window.SetSize(ref window.controls);
        }

        /// <summary>
        /// Добавляет игровое окно в общее их хранилище
        /// </summary>
        /// <param name="name">Название игрового окна</param>
        /// <param name="frame">Экземпляр фрейма</param>
        public void AddFrame(string name, Frame frame)
        {
            if (frames.ContainsKey(name))
            {
                return;
            }
            frames.Add(name, frame);
        }

        /// <summary>
        /// Удаляет игровое окно из общего их хранилища
        /// </summary>
        /// <param name="name">Название игрового окна</param>
        public void DelFrame(string name)
        {
            if (!frames.ContainsKey(name))
            {
                return;
            }
            frames.Remove(name);
        }

        /// <summary>
        /// Меняет экранный режим на противоположный
        /// </summary>
        public void ToggleWindowState()
        {
            if (window.windowState == WindowState.Windowed)
            {
                window.SetWindowState(WindowState.Fullscreen);               
            }
            else
            {
                window.SetWindowState(WindowState.Windowed);               
            }
            window.SetSize(ref window.controls);
        }

        /// <summary>
        /// Быстрый способ создать PictureBox
        /// </summary>
        /// <param name="p">Расположение</param>
        /// <param name="s">Размеры</param>
        /// <param name="sm">Способ размещения изображения</param>
        /// <param name="i">Изображение</param>
        /// <param name="visible">Виден ли</param>
        /// <returns>Созданный по параметрам PictureBox</returns>
        public static PictureBox PicCreation(Point p, Size s, PictureBoxSizeMode sm, Image i, bool visible)
        {
            PictureBox pb = new PictureBox();
            pb.Location = p;
            pb.Size = s;
            pb.SizeMode = sm;
            pb.Image = i;
            pb.Visible = visible;
            pb.BringToFront();
            return pb;
        }

        /// <summary>
        /// Быстрый способ создать PictureBox без изображения
        /// </summary>
        /// <param name="p">Расположение</param>
        /// <param name="s">Размеры</param>
        /// <param name="sm">Способ размещения изображения</param>
        /// <param name="visible">Виден ли</param>
        /// <returns>Созданный по параметрам PictureBox</returns>
        public static PictureBox PicBoxSkeleton(Point p, Size s, PictureBoxSizeMode sm, bool visible)
        {
            PictureBox pb = new PictureBox();
            pb.Location = p;
            pb.Size = s;
            pb.SizeMode = sm;
            pb.Visible = visible;
            pb.BringToFront();
            return pb;
        }

        /// <summary>
        /// Быстрый способ создать PictureBox с прозрачным фоном
        /// </summary>
        /// <param name="p">Расположение</param>
        /// <param name="s">Размеры</param>
        /// <param name="sm">Способ размещения изображения</param>
        /// <param name="i">Изображение</param>
        /// <param name="visible">Виден ли</param>
        /// <returns>Созданный по параметрам PictureBox</returns>
        public static PictureBox PicCreationTransparent(Point p,Size s, PictureBoxSizeMode sm, Image i, bool visible)
        {
            PictureBox pb = new PictureBox();
            pb.Location = p;
            pb.Size = s;
            pb.SizeMode = sm;
            pb.BackgroundImage = i;
            pb.BackgroundImageLayout = ImageLayout.Stretch;
            pb.Visible = visible;
            pb.BackColor = Color.Transparent;
            pb.BringToFront();
            return pb;
        }

        /// <summary>
        /// Быстрый способ создать Label
        /// </summary>
        /// <param name="p">Расположение</param>
        /// <param name="s">Размеры</param>
        /// <param name="text">Носимая текстовая информация</param>
        /// <param name="ta">Способ размещения текста</param>
        /// <param name="visible">Виден ли</param>
        /// <param name="f">Шрифт, которым будет написан текст</param>
        /// <param name="fc">Цвет текста</param>
        /// <param name="bc">Цвет заднего фона</param>
        /// <returns>Созданный по параметрам Label</returns>
        public static Label LabCreation(Point p, Size s, string text, bool visible, ContentAlignment ta,Font f,Color fc,Color bc)
        {
            Label lab = new Label();
            lab.Location = p;
            lab.Size = s;
            lab.Text = text;
            lab.AutoSize = false;
            lab.TextAlign = ta;
            lab.Font = f;
            lab.Visible = visible;
            lab.ForeColor = fc;
            lab.BackColor = bc;
            lab.BringToFront();
            return lab;
        }

        /// <summary>
        /// Быстрый способ создать Кнопку
        /// </summary>
        /// <param name="p">Расположение</param>
        /// <param name="s">Размеры</param>
        /// <param name="enabled">Флаг доступности</param>
        /// <param name="sign">Носимый текст</param>
        /// <param name="c">Цвет кнопки</param>
        /// <returns>Созданный по параметрам Button</returns>
        public static Button ButtonCreation(Point p, Size s, bool enabled, string sign, Color c)
        {
            Button b = new Button();
            b.Location = p;
            b.Size = s;
            b.TextAlign = ContentAlignment.MiddleCenter;
            b.Enabled = enabled;
            b.BringToFront();
            //b.Font = f;
            b.Text = sign;
            b.ForeColor = c;
            return b;
        }
        #endregion
    }
   

}