using GameEngine;
using System.Windows.Forms;
using System.Drawing;

namespace Rogue_JRPG.Frames
{
    public class Main_Menu : Frame
    {

        private PictureBox title;
        private PictureBox start;

        public Main_Menu(Engine engine) : base(engine)
        {
            title = Engine.PicCreation(
                new Point(265,100),
                new Size (450,100),
                PictureBoxSizeMode.StretchImage,
                Utils.Resize(Image.FromFile(@"..\\..\\Menu\\title.png"),
                new Size(450, 100)),
                true
                );
            /*title = new PictureBox();
            title.Location = new Point(265, 100);
            title.Size = new Size(450, 100);
            title.Image = Utils.Resize(Image.FromFile(@"..\\..\\Menu\\title.png"), new Size(450, 100));
            */
            start = Engine.PicCreation(
                new Point(350, 250),
                new Size(284, 108),
                PictureBoxSizeMode.StretchImage,
                Image.FromFile(@"..\\..\\Menu\\start.png"),
                true
                );
            start.Click += (sender, e) => this.engine.LoadFrame("Levelmap"); //Levelmap
            /*start = new PictureBox();
            start.Location = new Point(350, 250);
            start.Size = new Size(284, 108);
            start.Image = Image.FromFile(@"..\\..\\Menu\\start.png");
            start.Click += (sender, e) => this.engine.LoadFrame("Test");*/
            //Приколы с формы
        }
        

        public override void Load()
        {
            GetWindow().GetControl().Paint += Background;
            GetWindow().GetControl().Controls.Add(start);
            GetWindow().GetControl().Controls.Add(title);

        }

        public override void UnLoad()
        {
            GetWindow().GetControl().Paint -= Background;
            GetWindow().GetControl().Controls.Clear();
        }

        private void Background(object sender, PaintEventArgs e)
        {
            GetWindow().GetControl().BackColor = Color.FromArgb(255, 69, 25, 52); //Цвет заднего фона
        }
    }
}
