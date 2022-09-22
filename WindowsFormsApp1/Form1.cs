using System;
using System.Windows.Forms;
using GameEngine;
using Rogue_JRPG.Frames;

namespace Rogue_JRPG
{
    public partial class Form1 : Form
    {
        private Engine Engine = null;

        public Form1()
        {
            this.Engine = new Engine(this);
            this.Engine.AddFrame("Main_Menu", new Main_Menu(this.Engine));
            this.Engine.AddFrame("Map", new Map(this.Engine));
            this.Engine.LoadFrame("Main_Menu");
            //InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            PreLoad();
        }

        public void PreLoad() //метод,с которого начнем
        {

        }

    }
}
