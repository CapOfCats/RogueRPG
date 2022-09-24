using GameEngine;
using System.Windows.Forms;
using System.Drawing;

namespace Rogue_JRPG.Frames
{
    public class Main_Menu : Frame
    {

        private PictureBox Title;
        private PictureBox Start;

        public Main_Menu(Engine engine) : base(engine)
        {
            this.Title = new PictureBox();
            this.Title.Location = new Point(265, 100);
            this.Title.Size = new Size(450, 100);
            this.Title.Image = Utils.resize(Image.FromFile(@"..\\..\\Menu\\title.png"), new Size(450, 100));

            this.Start = new PictureBox();
            this.Start.Location = new Point(350, 250);
            this.Start.Size = new Size(284, 108);
            this.Start.Image = Image.FromFile(@"..\\..\\Menu\\start.png");
            this.Start.Click += (sender, e) => this.Engine.LoadFrame("Test");
        }

        public override void Load()
        {
            this.GetWindow().GetControl().Paint += Background;
            this.GetWindow().GetControl().Controls.Add(this.Start);
            this.GetWindow().GetControl().Controls.Add(this.Title);

        }

        public override void UnLoad()
        {
            this.GetWindow().GetControl().Paint -= Background;
            this.GetWindow().GetControl().Controls.Clear();
        }

        private void Background(object sender, PaintEventArgs e)
        {
            this.GetWindow().GetControl().BackColor = Color.FromArgb(255, 69, 25, 52); //Цвет заднего фона
        }
    }
}
