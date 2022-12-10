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
        #endregion
    }

    public enum WindowState
    {
        Unknown,
        Windowed,
        Borderless,
        Fullscreen
    }

    public class Window
    {
        #region Properties
        public Form mainForm;
        private Panel panel;
        private Screen screen;
        //
        public WindowState windowState;
        public List<Control> controls;
        //
        #endregion

        #region Constructor
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

        #region Functions
        /*private void Setup()
        {
            mainForm.SuspendLayout(); // Edit Mode : ON
            mainForm.AutoScaleDimensions = new SizeF(6F, 13F);
            mainForm.AutoScaleMode = AutoScaleMode.None;
            mainForm.ClientSize = new Size(1000, 1000);
            mainForm.AllowTransparency = true;
            //
            SetLocation(new Point(0, 0));
            SetSize(new Size(1000, 1000));
            Utils.SetDoubleBuffered(panel);
            //
            mainForm.SizeChanged += (sender, e) => this.SetSize(mainForm.Size);
            //
            mainForm.Controls.Add(this.panel);
            mainForm.ResumeLayout(false); // Edit Mode : OFF
            mainForm.Invalidate();
            //
            SetWindowState(WindowState.Windowed);
        }*/ 
        //его постигла судьба UnloadFrame()
        public Size GetSize()
        {
            return mainForm.Size;
        }
        public Size GetScreenSize()
        {
            return new Size(screen.Bounds.Width, screen.Bounds.Height);
        }

        public void Refresh()
        {
            panel.Refresh();
        }

        public void SetLocation(Point location)
        {
            panel.Location = location;
            //
            mainForm.Invalidate();
        }

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

        public Size ControlResize(Control c)
        {
            if (windowState == WindowState.Windowed)
                return new Size(c.Width / 2, c.Height / 2);
            else
                return new Size(c.Width * 2, c.Height * 2);
            /*float xRatio =  (float)c.Size.Width.CompareTo(GetSize().Width);
             float yRatio =  (float)c.Size.Height.CompareTo(GetSize().Height);
            //
            
            int newW = (int)(c.Width * xRatio); //*
            int newH = (int)(c.Height * yRatio); //**/
            ; //
        }
        public Point ControlRelocation(Control c)
        {
            if (windowState == WindowState.Windowed)
                return new Point(c.Location.X / 2, c.Location.Y / 2);
            else
                return new Point(c.Location.X * 2, c.Location.Y * 2);
        }

        public Form GetForm()
        {
            return mainForm;
        }

        public Control GetControl()
        {
            return panel;
        }
        #endregion
    }

    public abstract class Frame
    {
        #region Properties
        public Engine engine;
        public List<Control> controlStash;
        #endregion

        #region Constructor
        public Frame(Engine engine)
        {
            this.engine = engine;
        }
        #endregion

        #region Functions
        public Window GetWindow()
        {
            return engine.window;
        }
        public abstract void Load();

        public abstract void UnLoad();
        #endregion
    }

    public class Engine
    {
        #region Properties
        public Window window;
        public Timer timer;
        public Dictionary<string, Frame> frames;
        public string currentFrame;
        #endregion

        #region Constructor
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
                //window.mainForm.Invalidate();
                //frmGame_Paint(e);
            };
           timer.Enabled = true;
            //
            frames = new Dictionary<string, Frame>();
            currentFrame = "None";

        }
        /*public void frmGame_Paint(PaintEventArgs e)
        {
            //window.mainForm.DoubleBuffered = true;
            for (int i = 0; i < frames[currentFrame].controlStash.Count; i++)
                if (frames[currentFrame].controlStash[i].GetType() == typeof(PictureBox))
                {
                    var p = frames[currentFrame].controlStash[i] as PictureBox;
                    p.Visible = false;
                    e.Graphics.DrawImage(p.Image, p.Left, p.Top, p.Width, p.Height);
                    //frames[currentFrame].controlStash[i].Invalidate();
                }
        }*/
        #endregion

        #region Functions
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
            
            //window.mainForm.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

       /* public void UnLoadFrame(string str)
        {
            if (this.frames.Count == 0 || !this.frames.ContainsKey(this.currentFrame))
            {
                return; // Dictionary is empty OR No Frame exist with the key str
            }
            this.frames[str].UnLoad();
            this.currentFrame = "None";
            this.window.GetForm().Invalidate();
        }*/
       //пока склеил. Ненужное разделение

        public void AddFrame(string name, Frame frame)
        {
            if (frames.ContainsKey(name))
            {
                return;
            }
            frames.Add(name, frame);
        }

        public void DelFrame(string name)
        {
            if (!frames.ContainsKey(name))
            {
                return;
            }
            frames.Remove(name);
        }

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