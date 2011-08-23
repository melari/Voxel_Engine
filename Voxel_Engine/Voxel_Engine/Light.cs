using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Voxel_Engine
{
    class Light
    {
        public Vector3 position;
        public float brightness;
        public float distance;

        public Light(Vector3 position, float distance)
            : this(position, distance, 0.75f)
        {            
        }
        public Light(Vector3 position, float distance, float brightness)
        {
            this.position = position;
            this.distance = distance;
            this.brightness = brightness;
        }
    }
}
