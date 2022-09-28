using GameEngine;
using System.Windows.Forms;
using System.Drawing;

namespace Rogue_JRPG.Frames
{
    public class Map : Frame
    {

        private PictureBox map;

        public Map(Engine engine) : base(engine)
        {
            //this.map = new PictureBox();
            //this.map.Location = new Point(265, 100);
            //this.map.Size = new Size(450, 100);
            //this.map.Image = Utils.Resize(Image.FromFile(@"Resources\\.png"), new Size(450, 100));
        }

        public override void Load()
        {
            this.GetWindow().GetControl().Controls.Add(this.map);

        }

        public override void UnLoad()
        {
            this.GetWindow().GetControl().Controls.Clear();
        }

    }
}
