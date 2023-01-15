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
    /// <summary>
    /// Сущность ячейки инвентаря в окне карты
    /// </summary>
    class InventoryCell
    {
        #region Поля
        /// <summary>
        /// Кнопка ячейки
        /// </summary>
        public Button button;
        /// <summary>
        /// Кроющийся в ней экземпляр предмета
        /// </summary>
        public Item item;
        /// <summary>
        /// Состояние заполненности
        /// </summary>
        State state;
        #endregion

        #region Конструкторы
        public InventoryCell(Button b,Item i)
        {
            this.item= i;
            this.button = b;
        }
        public InventoryCell(Button b)
        {
            this.button = b;
        }
        #endregion

        /// <summary>
        /// Заполненность
        /// </summary>
        public enum State
        {
            Empty,
            Occupied
        }

        #region Методы
        /// <summary>
        /// Получает состояние ячейки
        /// </summary>
        /// <returns>State ячейки</returns>
        public State GetState()
        {
            return this.state;
        }

        /// <summary>
        /// Меняет состояние на противоположное
        /// </summary>
        public void ChangeState()
        {
            if (state == State.Occupied)
                state = State.Empty;
            else state = State.Occupied;
        }

        /// <summary>
        /// Уничтожает внутренний предмет
        /// </summary>
        public void AnnihilateItem()
        {
            item=null;
        }

        /// <summary>
        /// Меняет способ отображения в зависимости от наличия предмета внутри
        /// </summary>
        public void CheckState()
        {
            if (item == null)
            {
                state = State.Empty;
                if( button.Image!=null)
                button.Image.Dispose();
                button.BackColor = Color.AntiqueWhite;
            }
            else
            {
                state = State.Occupied;
                button.Image = item.icon;
            }
        }

        /// <summary>
        /// Открывает или блокирует доступ к кнопке в зависимости от наличия предмета внутри
        /// </summary>
        public void Block()
        {
            button.Enabled = true;
            /*if (state == State.Empty)
                button.Enabled = false;
            else button.Enabled = true;*/
        }
        #endregion
    }
}
