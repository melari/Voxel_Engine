using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Voxel_Engine
{
    class VoxelManager
    {        
        public static int MAX_X = 64;
        public static int MAX_Y = 32;
        public static int MAX_Z = 64;

        BufferedList<Voxel> voxels = new BufferedList<Voxel>();
        Voxel[, ,] map = new Voxel[MAX_X, MAX_Y, MAX_Z];

        public BufferedList<Light> lights = new BufferedList<Light>();

        public VoxelManager()
        {               
            for (int z = 0; z < 32; z++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 32; x++)
                    {                        
                        Add(new Voxel(this, new Vector3(x, y, z), TextureManager.getTexture("grass")));
                    }
                }
            }
            for (int i = 0; i < 20; i++)
                lights.Add(new Light(new Vector3(0, 33, 0), 30));
        }

        public void Add(Voxel voxel)
        {
            if (voxel.IsMoving() || !voxel.IsAligned())
            {
                throw new ArgumentException("Cannot add moving or non-aligned voxels. Use AddMoving() instead.");
            }

            Voxel prev_voxel = GetVoxelByPosition(voxel.position);
            if (prev_voxel != null)
            {
                Remove(prev_voxel);
            }
            voxels.Add(voxel);
            MapSetByVector(voxel.position, voxel);
        }

        public void Remove(Voxel voxel)
        {
            voxel.Destroy();
            MapSetByVector(voxel.position, null);
            voxels.Remove(voxel);
        }

        public Voxel GetVoxelByPosition(Vector3 position)
        {
            int x = (int)position.X;
            int y = (int)position.Y;
            int z = (int)position.Z;
            if (x < 0 || y < 0 || z < 0 || x > MAX_X - 1 || y > MAX_Y - 1 || z > MAX_Z - 1) { return null; }
            return map[(int)position.X, (int)position.Y, (int)position.Z];                        
        }

        public bool IsOpaqueAt(Vector3 position)
        {
            Voxel v = GetVoxelByPosition(position);
            if (v == null)
                return false;
            return !v.alpha;
        }

        private void MapSetByVector(Vector3 position, Voxel value)
        {
            map[(int)position.X, (int)position.Y, (int)position.Z] = value;
        }

        public int FindTopSpace(int x, int z)
        {
            for (int y = 0; y < MAX_Y; y++)
            {
                if (GetVoxelByPosition(new Vector3(x, y, z)) == null) 
                { 
                    return y; 
                }
            }
            return -1; //no space.
        }

        public void Clear()
        {
            voxels.Clear();
            map = new Voxel[MAX_X, MAX_Y, MAX_Z];
        }

        public void SendLightsToShader(Effect effect)
        {
            Vector3[] _lightPosition = new Vector3[lights.Count];
            float[] _lightBrightness = new float[lights.Count];
            float[] _lightDistance = new float[lights.Count];
            
            for (int i = 0; i < lights.Count; i++)
            {
                _lightPosition[i] = lights[i].position;
                _lightBrightness[i] = lights[i].brightness;
                _lightDistance[i] = lights[i].distance;
            }

            effect.Parameters["xLightCount"].SetValue(lights.Count);
            effect.Parameters["xLightPosition"].SetValue(_lightPosition);
            effect.Parameters["xLightBrightness"].SetValue(_lightBrightness);
            effect.Parameters["xLightDistance"].SetValue(_lightDistance);
        }

        public void Draw(GraphicsDevice device, Effect effect)
        {
            foreach (Voxel voxel in voxels)
            {
                voxel.Draw(device, effect);
            }
        }
    }
}
