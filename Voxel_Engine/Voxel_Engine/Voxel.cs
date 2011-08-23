using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Voxel_Engine
{    
    struct VoxelVertex
    {
        public Vector3 Position;        
        public Vector3 Normal;
        public Vector2 TextureCoordinate;

        public VoxelVertex(Vector3 position, Vector3 normal, Vector2 textureCoordinate)
        {
            Position = position;
            Normal = normal;
            TextureCoordinate = textureCoordinate;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
              (
                  new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                  new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                  new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
              );
        
     }   

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
        public Texture2D texture;
        public Vector3 velocity;
        public bool alpha;
        public bool cube;
        public Light light;

        VoxelManager manager;

        public Voxel(VoxelManager manager, Vector3 position, Texture2D texture, bool alpha = false, bool cube = true, bool light = false)
        {
            this.manager = manager;
            this.position = position;
            this.texture = texture;
            this.alpha = alpha;
            this.cube = cube;

            if (light)
            {
                this.light = new Light(position + Vector3.UnitY, 10);
                manager.lights.Add(this.light);
            }
            
            SetUpVertices();            
        }
        public void Destroy()
        {
            if (light != null)
                manager.lights.Remove(light);
        }

        public bool IsMoving()
        {
            return velocity != Vector3.Zero;
        }

        public bool IsAligned()
        {
            return position.X % 1 == 0 && position.Y % 1 == 0 && position.Z % 1 == 0;
        }

        private void SetUpTextureCoordinates(VertexPositionNormalTexture[] vertices)
        {
            vertices[0].TextureCoordinate = Vector2.UnitY;
            vertices[1].TextureCoordinate = Vector2.One;
            vertices[2].TextureCoordinate = Vector2.UnitX;
            vertices[3].TextureCoordinate = Vector2.Zero;
        }
        private void SetUpVertices()
        {
            front = new VertexPositionNormalTexture[4];
            back = new VertexPositionNormalTexture[4];
            top = new VertexPositionNormalTexture[4];
            bottom = new VertexPositionNormalTexture[4];
            left = new VertexPositionNormalTexture[4];
            right = new VertexPositionNormalTexture[4];

            SetUpTextureCoordinates(front);
            SetUpTextureCoordinates(bottom);
            SetUpTextureCoordinates(right);
            SetUpTextureCoordinates(left);
            SetUpTextureCoordinates(back);
            SetUpTextureCoordinates(top);

            for (int i = 0; i < 4; i++)
            {
                bottom[i].Normal = -Vector3.UnitY;
                back[i].Normal = -Vector3.UnitZ;
                top[i].Normal = Vector3.UnitY;
                left[i].Normal = -Vector3.UnitX;
                right[i].Normal = Vector3.UnitX;
                front[i].Normal = Vector3.UnitZ;
            }           

            ForceVerticeUpdate();
        }

        public void ForceVerticeUpdate()
        {
            Vector3 xSize = new Vector3(SIZE.X, 0, 0);
            Vector3 ySize = new Vector3(0, SIZE.Y, 0);
            Vector3 zSize = new Vector3(0, 0, SIZE.Z);
            
            front[0].Position = position;
            front[1].Position = position + xSize;
            front[2].Position = position + xSize + ySize;
            front[3].Position = position + ySize;

            back[0].Position = position + xSize - zSize;
            back[1].Position = position - zSize;
            back[2].Position = position + ySize - zSize;
            back[3].Position = position + xSize + ySize - zSize;            

            left[0].Position = position - zSize;
            left[1].Position = position;
            left[2].Position = position + ySize;
            left[3].Position = position + ySize - zSize;

            right[0].Position = position + xSize;
            right[1].Position = position + xSize - zSize;
            right[2].Position = position + xSize + ySize - zSize;
            right[3].Position = position + xSize + ySize;

            if (cube)
            {
                bottom[0].Position = position + xSize;
                bottom[1].Position = position;
                bottom[2].Position = position - zSize;
                bottom[3].Position = position + xSize - zSize;

                top[0].Position = position + ySize;
                top[1].Position = position + ySize + xSize;
                top[2].Position = position + xSize + ySize - zSize;
                top[3].Position = position + ySize - zSize;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    front[i].Position -= zSize / 2;
                    back[i].Position += zSize / 2;
                    left[i].Position += xSize / 2;
                    right[i].Position -= xSize / 2;
                }                
            }
        }

        private float[] CalculateLighting(VertexPositionNormalTexture[] vertices, BufferedList<Light> lights)
        {
            float strength = 0;
            foreach (Light light in lights)
            {                
                Vector3 lightRay = vertices[0].Position - light.position;
                float length = lightRay.Length();
                lightRay.Normalize();
                float dot = Vector3.Dot(vertices[0].Normal, -lightRay);
                float distAdjust = Math.Max(0, (light.distance - length) / light.distance);
                strength += Math.Max(0, dot * distAdjust * light.brightness);
            }

            return new float[] {strength};
        }

        public void Draw(GraphicsDevice device, Effect effect)
        {
            if (velocity != Vector3.Zero)
            {
                ForceVerticeUpdate();
            }

            effect.Parameters["xTexture"].SetValue(texture);            
            
            if (!manager.IsOpaqueAt(position + Vector3.UnitZ))
            {
                //effect.Parameters["xLights"].SetValue(CalculateLighting(front, manager.lights));
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, front, 0, front.Length, indices, 0, indices.Length / 3);
                }
            }
            
            if (!manager.IsOpaqueAt(position + Vector3.UnitX))
            {
                //effect.Parameters["xLights"].SetValue(CalculateLighting(right, manager.lights));
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, right, 0, right.Length, indices, 0, indices.Length / 3);
                }
            }
            
            if (!manager.IsOpaqueAt(position - Vector3.UnitX))
            {
                //effect.Parameters["xLights"].SetValue(CalculateLighting(left, manager.lights));
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, left, 0, left.Length, indices, 0, indices.Length / 3);
                }
            }
            
            if (cube && !manager.IsOpaqueAt(position + Vector3.UnitY))
            {
                //effect.Parameters["xLights"].SetValue(CalculateLighting(top, manager.lights));
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, top, 0, top.Length, indices, 0, indices.Length / 3);
                }
            }
            
            if (cube && !manager.IsOpaqueAt(position - Vector3.UnitY) && position.Y != 0)
            {
                //effect.Parameters["xLights"].SetValue(CalculateLighting(bottom, manager.lights));
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, bottom, 0, bottom.Length, indices, 0, indices.Length / 3);
                }
            }
            
            if (!manager.IsOpaqueAt(position - Vector3.UnitZ))
            {
                //effect.Parameters["xLights"].SetValue(CalculateLighting(back, manager.lights));
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, back, 0, back.Length, indices, 0, indices.Length / 3);
                }
            }
        }
    }
}
