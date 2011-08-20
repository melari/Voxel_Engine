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
        public static int MAX_X = 100;
        public static int MAX_Y = 100;
        public static int MAX_Z = 100;

        BufferedList<Voxel> voxels = new BufferedList<Voxel>();
        Voxel[, ,] map = new Voxel[MAX_X, MAX_Y, MAX_Z];

        public VoxelManager(Texture2D red, Texture2D blue)
        {
            for (int z = 0; z < 30; z++)
            {
                for (int y = 0; y < 10; y++)
                {
                    for (int x = 0; x < 30; x++)
                    {
                        Texture2D texture = MathExtra.rand.Next(2) == 0 ? red : blue;
                        Add(new Voxel(this, new Vector3(x, y, z), texture));
                    }
                }
            }
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

        private void MapSetByVector(Vector3 position, Voxel value)
        {
            map[(int)position.X, (int)position.Y, (int)position.Z] = value;
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
