using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelTest2.Primitives
{
    public class Block
    {
        public bool isActive { get; set; } = false;
        public bool isHighlighted { get; set; } = false;
        public float hardness { get; set; } = 1.0f;
        public BlockTypes blockType { get; set; } = 0;
        public void SetActive (bool isActive){ this.isActive = isActive; }
        public Block(int id)
        {
            blockType = (BlockTypes)id;
            if(id != 0)
            {
                isActive = true;
            }
        }

    }
    public enum BlockTypes
    {
        BT_Air = 0,
        BT_Stone = 1,
        BT_Grass = 2,
        BT_Alien = 3,
    }
}
