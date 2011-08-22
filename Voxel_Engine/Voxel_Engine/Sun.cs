using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Voxel_Engine
{
    class Sun : Light
    {
        public const float OFFSET = 5;
        
        private float time = 0; //0 to pi during daytime...        

        public Sun()
            : base(new Vector3(VoxelManager.MAX_X, VoxelManager.MAX_Y, VoxelManager.MAX_Z), 0, 0)
        {
        }

        public void SetTime(float time)
        {
            this.time = time;

        }
    }
}
