using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelTest2.Primitives;

namespace VoxelTest2.World
{
    public class ChunkManager
    {
        const int WORLD_SIZE = 4;
        public Chunk[][] chunks;
        public ChunkManager()
        {
            chunks = new Chunk[WORLD_SIZE][];
            for (int x = 0; x < WORLD_SIZE; x++)
            {
                chunks[x] = new Chunk[WORLD_SIZE];
                for (int z = 0; z < WORLD_SIZE; z++)
                {
                    chunks[x][z] = new Chunk();
                }
            }
        }
    }
}
