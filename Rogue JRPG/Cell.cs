using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursovik3
{
    class Cell
    {
        public Cell(bool _block, bool _interact, PictureBox _appearance)
        {
            block = _block;
            interact = _interact;
            appearance = _appearance;
        }
        private bool block; 
        public bool interact; //флаги
        public PictureBox appearance; //дименшны и текстура
    }
}
