using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rogue_JRPG
{
    class Room
    {
        public Room(List<Cell>_cells, Difficulty _thisDifficulty, List<PictureBox>_textures)//cells, возможно, заменим двумерным массивом
        {
            cells = _cells;
            thisDifficulty = _thisDifficulty;
            textures = _textures;
        }

        public enum Difficulty
        {
            Easy,
            Medium,
            Hell
        }
        public List<PictureBox> textures;
        public Difficulty thisDifficulty;
        public List<Cell> cells;
        public int enemyNumber;

        //
        public int openingDirection;
        // 1 --> need bottom door
        // 2 --> need top door
        // 3 --> need left door
        // 4 --> need right door

        public bool spawned = false;
        public bool spawnedBoss = false;
        public List<Room> bottomRooms;
        public List<Room> topRooms;
        public List<Room> leftRooms;
        public List<Room> rightRooms;
        public int Position;


        public void Generation()
        {
            switch (thisDifficulty)//здесь пресеты комнаты в зависимости от параметров. Кейзы будут дополняться - сделаем всё в одном свиче
            {
                case Difficulty.Easy: enemyNumber = 1; break;
                case Difficulty.Medium: enemyNumber = 2; break;
                case Difficulty.Hell: enemyNumber = 3; break;
            }    
        }

    }
}
