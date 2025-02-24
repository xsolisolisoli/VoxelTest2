using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelTest2
{
    public class Env
    {
        public static void EnvSetup()
        {
            Console.WriteLine("Env.cs:: Setting up environment");
            Environment.SetEnvironmentVariable("BLOCK_RENDER_SIZE", "0.5");
            Environment.SetEnvironmentVariable("TITLE", System.Reflection.Assembly.GetExecutingAssembly().ToString());
        }
    }
}
