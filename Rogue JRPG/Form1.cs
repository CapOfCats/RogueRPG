using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameEngine;
using Utilities;

namespace Kursovik3
{
    public partial class Form1 : Form
    {
        private Engine Engine = null;
        public Form1()
        {
            this.Engine = new Engine(this);
            this.Engine.AddFrame("Test", new Test(this.Engine));
            this.Engine.LoadFrame("Test");
            InitializeComponent();
        }

        public class Test : Frame
        {
            PictureBox pb = new PictureBox();
            public Test(Engine engine) : base(engine)
            {
                List<Image> appearances = new List<Image>();
                appearances.Add(Image.FromFile("w.png"));
                appearances.Add(Image.FromFile("a.png"));
                appearances.Add(Image.FromFile("s.png"));
                appearances.Add(Image.FromFile("d.png"));
                pb.Location = new Point(200, 200);
                pb.Image = appearances[2];
                void Animation()
                {
                    
                void KeyDown(object sender, KeyEventArgs e)
                {
                        if (e.KeyCode == Keys.W)
                        {
                            pb.Image = appearances[0];
                            pb.Location = new Point(pb.Location.X, pb.Location.Y-16);
                        }

                        if (e.KeyCode == Keys.A)
                        {
                            pb.Image = appearances[1];
                            pb.Location = new Point(pb.Location.X-16, pb.Location.Y);
                        }

                        if (e.KeyCode == Keys.S)
                        {
                            pb.Image = appearances[2];
                            pb.Location = new Point(pb.Location.X, pb.Location.Y + 16);
                        }

                        if (e.KeyCode == Keys.D)
                        {
                            pb.Image = appearances[3];
                            pb.Location = new Point(pb.Location.X+16, pb.Location.Y);
                        }

                        e.Handled = true;
                }

                }
                
                Animation();
            }
            
            public override void Load()
            {
                this.GetWindow().GetControl().Controls.Add(pb);
            }

            public override void UnLoad()
            {
                this.GetWindow().GetControl().Controls.Clear();
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            PreLoad();
        }

        public void PreLoad() //метод,с которого начнем
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
