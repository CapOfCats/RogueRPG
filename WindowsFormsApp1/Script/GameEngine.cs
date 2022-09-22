using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GameEngine
{

    public class Utils
    {
        #region Functions
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

        
        public static Bitmap resize(Image src, Size size)
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
        #endregion
    }

    public enum WindowState
    {
        Unknown, Windowed, Borderless, Fullscreen
    }

    public class Window
    {
        #region Properties
        private Form MainForm;
        private Panel Panel;
        //
        public WindowState WindowState;
        //
        #endregion

        #region Constructor
        public Window(Form form)
        {
            this.MainForm = form;
            this.Panel = new Panel();
            this.Setup();
        }
        #endregion

        #region Functions
        private void Setup()
        {
            this.MainForm.SuspendLayout(); // Edit Mode : ON
            this.MainForm.AutoScaleDimensions = new SizeF(6F, 13F);
            this.MainForm.AutoScaleMode = AutoScaleMode.None;
            this.MainForm.ClientSize = new Size(1000, 1000);
            this.MainForm.AllowTransparency = true;
            //
            this.SetLocation(new Point(0, 0));
            this.SetSize(new Size(1000, 1000));
            Utils.SetDoubleBuffered(this.Panel);
            //
            this.MainForm.SizeChanged += (sender, e) => this.SetSize(this.MainForm.Size);
            //
            this.MainForm.Controls.Add(this.Panel);
            this.MainForm.ResumeLayout(false); // Edit Mode : OFF
            this.MainForm.Invalidate();
            //
            this.SetWindowState(WindowState.Windowed);
        }

        public void Refresh()
        {
            this.Panel.Refresh();
        }

        public void SetLocation(Point location)
        {
            this.Panel.Location = location;
            //
            this.MainForm.Invalidate();
        }

        public void SetSize(Size size)
        {
            this.Panel.ClientSize = size;
            this.Panel.Size = size;
            //
            this.MainForm.Invalidate();
        }

        public void SetWindowState(WindowState state)
        {
            if (this.WindowState == state)
            {
                return;
            }
            switch (state)
            {
                case WindowState.Borderless:
                case WindowState.Fullscreen:
                    this.MainForm.WindowState = FormWindowState.Normal;
                    this.MainForm.FormBorderStyle = FormBorderStyle.None;
                    this.MainForm.WindowState = FormWindowState.Maximized;
                    break;
                case WindowState.Windowed:
                    this.MainForm.FormBorderStyle = FormBorderStyle.Sizable;
                    this.MainForm.WindowState = FormWindowState.Normal;
                    break;
                default:
                    return;
            }
            this.WindowState = state;
            this.MainForm.Invalidate();
        }

        public Form GetForm()
        {
            return this.MainForm;
        }

        public Control GetControl()
        {
            return this.Panel;
        }
        #endregion
    }

    public abstract class Frame
    {
        #region Properties
        public Engine Engine;
        #endregion

        #region Constructor
        public Frame(Engine engine)
        {
            this.Engine = engine;
        }
        #endregion

        #region Functions
        public Window GetWindow()
        {
            return this.Engine.Window;
        }

        public abstract void Load();

        public abstract void UnLoad();
        #endregion
    }

    public class Engine
    {
        #region Properties
        public Window Window;
        public Timer Timer;
        public Dictionary<string, Frame> Frames;
        public string CurrentFrame;
        #endregion

        #region Constructor
        public Engine(Form form)
        {
            this.Window = new Window(form);
            this.Window.GetControl().DoubleClick += (sender, e) => this.ToggleWindowState();
            //
            this.Timer = new Timer();
            this.Timer.Interval = 60;
            this.Timer.Tick += (sender, e) => this.Window.Refresh();
            this.Timer.Enabled = true;
            //
            this.Frames = new Dictionary<string, Frame>();
            this.CurrentFrame = "None";
        }
        #endregion

        #region Functions
        public void LoadFrame(string str)
        {
            if (this.Frames.Count == 0 || !this.Frames.ContainsKey(str))
            {
                return; // Dictionary is empty OR No Frame exist with the key str
            }
            this.UnLoadFrame(this.CurrentFrame);
            this.Frames[str].Load();
            this.CurrentFrame = str;
            this.Window.GetForm().Invalidate();
        }

        public void UnLoadFrame(string str)
        {
            if (this.Frames.Count == 0 || !this.Frames.ContainsKey(this.CurrentFrame))
            {
                return; // Dictionary is empty OR No Frame exist with the key str
            }
            this.Frames[str].UnLoad();
            this.CurrentFrame = "None";
            this.Window.GetForm().Invalidate();
        }

        public void AddFrame(string name, Frame frame)
        {
            if (this.Frames.ContainsKey(name))
            {
                return;
            }
            this.Frames.Add(name, frame);
        }

        public void DelFrame(string name)
        {
            if (!this.Frames.ContainsKey(name))
            {
                return;
            }
            this.Frames.Remove(name);
        }

        public void ToggleWindowState()
        {
            if (this.Window.WindowState == WindowState.Windowed)
            {
                this.Window.SetWindowState(WindowState.Fullscreen);
            }
            else
            {
                this.Window.SetWindowState(WindowState.Windowed);
            }
        }
        #endregion
    }

}