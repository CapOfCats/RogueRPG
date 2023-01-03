using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameEngine;

namespace Rogue_JRPG
{
    class InventoryCell
    {
        //Image image;
        public Button button;
        public Item item;
        State state;
        public InventoryCell(Button b,Item i)
        {
            this.item= i;
            this.button = b;
        }
        public InventoryCell(Button b)
        {
            this.button = b;
        }
        public enum State
        {
            Empty,
            Occupied
        }

        public State GetState()
        {
            return this.state;
        }

        public void ChangeState()
        {
            if (state == State.Occupied)
                state = State.Empty;
            else state = State.Occupied;
        }
        public void AnnihilateItem()
        {
            item = null;
        }

        public void CheckState()
        {
            if (item == null)
            {
                state = State.Empty;
                //button.Image.Dispose();
                button.BackColor = Color.AntiqueWhite;
            }
            else
            {
                state = State.Occupied;
                button.Image = item.icon;
            }
        }
        
        public void Block()
        {
            if (state == State.Occupied)
                button.Enabled = true;
            else button.Enabled = false;
        }
    }
}
