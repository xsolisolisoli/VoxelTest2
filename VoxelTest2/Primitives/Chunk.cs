using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelTest2.Primitives
{
    public class Chunk
    {
        const int CHUNK_SIZE = 16;
        private Block[][][] _data;
        
        void render()
        {

        }
        public void CreateCube()
        {


        }
        public void CreateMesh()
        {

        }
        public void CreateChunk()
        {
            // Initialize the 3D jagged array
            _data = new Block[CHUNK_SIZE][][];

            for (int i = 0; i < CHUNK_SIZE; i++)
            {
                _data[i] = new Block[CHUNK_SIZE][];

                for (int j = 0; j < CHUNK_SIZE; j++)
                {
                    _data[i][j] = new Block[CHUNK_SIZE];

                    // Initialize each Block element (if Block is a class)
                    for (int k = 0; k < CHUNK_SIZE; k++)
                    {
                        _data[i][j][k] = new Block();
                    }
                }
            }
        }
    }
}
