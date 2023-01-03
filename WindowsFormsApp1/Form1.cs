using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameEngine;
using Rogue_JRPG.Frames;

namespace Rogue_JRPG
{
    public partial class Form1 : Form
    {
        private Engine engine;

        public Form1()
        {
            
            engine = new Engine(this);
            
            engine.AddFrame("Main_Menu", new Main_Menu(engine));
            engine.AddFrame("Levelmap", new Map(engine));
            engine.AddFrame("Test", new Test(engine));
            engine.LoadFrame("Main_Menu");
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }
        
        /*protected override void OnPaint(PaintEventArgs e)
        {
            // Не забываем вызвать базовый метод, чтобы перерисовалась форма
            //engine.frmGame_Paint(e);
            //base.OnPaint(e);
        }*/


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }
      

    }
}
