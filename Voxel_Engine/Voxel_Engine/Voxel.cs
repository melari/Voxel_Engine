using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Voxel_Engine
{
    class Voxel
    {
        public static Vector3 SIZE = new Vector3(1f, 1f, 1f);

        VertexPositionNormalTexture[] front;
        VertexPositionNormalTexture[] back;
        VertexPositionNormalTexture[] left;
        VertexPositionNormalTexture[] right;
        VertexPositionNormalTexture[] top;
        VertexPositionNormalTexture[] bottom;
        public Vector3 position;                
        private int[] indices = new int[]{ 2, 1, 0, 3, 2, 0 };
        private Texture2D texture;
        public Vector3 velocity;

        VoxelManager manager;

        public Voxel(VoxelManager manager, Vector3 position, Texture2D texture)
        {
            this.manager = manager;
            this.position = position;
            this.texture = texture;
            
            SetUpVertices();            
        }

        public bool IsMoving()
        {
            return velocity != Vector3.Zero;
        }

        public bool IsAligned()
        {
            return position.X % 1 == 0 && position.Y % 1 == 0 && position.Z % 1 == 0;
        }        
          
        private void SetUpVertices()
        {            
            front = new VertexPositionNormalTexture[4];
            back = new VertexPositionNormalTexture[4];
            top = new VertexPositionNormalTexture[4];
            bottom = new VertexPositionNormalTexture[4];
            left = new VertexPositionNormalTexture[4];
            right = new VertexPositionNormalTexture[4];

            Vector3 xSize = new Vector3(SIZE.X, 0, 0);
            Vector3 ySize = new Vector3(0, SIZE.Y, 0);
            Vector3 zSize = new Vector3(0, 0, SIZE.Z);

            //**Front**//
            front[0].Position = position;
            front[0].TextureCoordinate = Vector2.Zero;
            front[0].Normal = Vector3.UnitZ;
            front[1].Position = position + xSize;
            front[1].TextureCoordinate = Vector2.Zero;
            front[1].Normal = Vector3.UnitZ;
            front[2].Position = position + xSize + ySize;
            front[2].TextureCoordinate = Vector2.Zero;
            front[2].Normal = Vector3.UnitZ;
            front[3].Position = position + ySize;
            front[3].TextureCoordinate = Vector2.Zero;
            front[3].Normal = Vector3.UnitZ;

            //**RIGHT**//            
            right[0].Position = position + xSize;
            right[0].TextureCoordinate = Vector2.Zero;
            right[0].Normal = Vector3.UnitX;
            right[1].Position = position + xSize - zSize;
            right[1].TextureCoordinate = Vector2.Zero;
            right[1].Normal = Vector3.UnitX;
            right[2].Position = position + xSize + ySize - zSize;
            right[2].TextureCoordinate = Vector2.Zero;
            right[2].Normal = Vector3.UnitX;
            right[3].Position = position + xSize + ySize;
            right[3].TextureCoordinate = Vector2.Zero;
            right[3].Normal = Vector3.UnitX;

            //**LEFT**//
            left[0].Position = position - zSize;
            left[0].TextureCoordinate = Vector2.Zero;
            left[0].Normal = -Vector3.UnitX;
            left[1].Position = position;
            left[1].TextureCoordinate = Vector2.Zero;
            left[1].Normal = -Vector3.UnitX;
            left[2].Position = position + ySize;
            left[2].TextureCoordinate = Vector2.Zero;
            left[2].Normal = -Vector3.UnitX;
            left[3].Position = position + ySize - zSize;
            left[3].TextureCoordinate = Vector2.Zero;
            left[3].Normal = -Vector3.UnitX;

            //**TOP**//
            top[0].Position = position + ySize;
            top[0].TextureCoordinate = Vector2.Zero;
            top[0].Normal = Vector3.UnitY;
            top[1].Position = position + ySize + xSize;
            top[1].TextureCoordinate = Vector2.Zero;
            top[1].Normal = Vector3.UnitY;
            top[2].Position = position + xSize + ySize - zSize;
            top[2].TextureCoordinate = Vector2.Zero;
            top[2].Normal = Vector3.UnitY;
            top[3].Position = position + ySize - zSize;
            top[3].TextureCoordinate = Vector2.Zero;
            top[3].Normal = Vector3.UnitY;

            //**BOTTOM**//
                bottom[0].Position = position + xSize;
            bottom[0].TextureCoordinate = Vector2.Zero;
            bottom[0].Normal = -Vector3.UnitY;
                bottom[1].Position = position;
            bottom[1].TextureCoordinate = Vector2.Zero;
            bottom[1].Normal = -Vector3.UnitY;
                bottom[2].Position = position - zSize;
            bottom[2].TextureCoordinate = Vector2.Zero;
            bottom[2].Normal = -Vector3.UnitY;
                bottom[3].Position = position + xSize - zSize;
            bottom[3].TextureCoordinate = Vector2.Zero;
            bottom[3].Normal = -Vector3.UnitY;

            //**BACK**//
            back[0].Position = position + xSize - zSize;
            back[0].TextureCoordinate = Vector2.Zero;
            back[0].Normal = -Vector3.UnitZ;
            back[1].Position = position - zSize;
            back[1].TextureCoordinate = Vector2.Zero;
            back[1].Normal = -Vector3.UnitZ;
            back[2].Position = position + ySize - zSize;
            back[2].TextureCoordinate = Vector2.Zero;
            back[2].Normal = -Vector3.UnitZ;
            back[3].Position = position + xSize + ySize - zSize;
            back[3].TextureCoordinate = Vector2.Zero;
            back[3].Normal = -Vector3.UnitZ;
        }

        public void Draw(GraphicsDevice device, Effect effect)
        {
            effect.Parameters["xTexture"].SetValue(texture);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                if (manager.GetVoxelByPosition(position + Vector3.UnitZ) == null)
                    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, front, 0, front.Length, indices, 0, indices.Length / 3);
                if (manager.GetVoxelByPosition(position + Vector3.UnitX) == null)
                    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, right, 0, front.Length, indices, 0, indices.Length / 3);
                if (manager.GetVoxelByPosition(position - Vector3.UnitX) == null)
                    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, left, 0, front.Length, indices, 0, indices.Length / 3);
                if (manager.GetVoxelByPosition(position + Vector3.UnitY) == null)
                    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, top, 0, front.Length, indices, 0, indices.Length / 3);
                //if (manager.GetVoxelByPosition(position - Vector3.UnitY) == null && position.Y != 0)
                    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, bottom, 0, front.Length, indices, 0, indices.Length / 3);
                if (manager.GetVoxelByPosition(position - Vector3.UnitZ) == null)
                    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, back, 0, front.Length, indices, 0, indices.Length / 3);
            }
            
        }
    }
}
