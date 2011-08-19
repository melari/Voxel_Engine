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
        public static Vector3 SIZE = new Vector3(1.0f, 1.0f, 1.0f);                

        private VertexPositionColor[] vertices;
        public VertexPositionColor[] Vertices { get { return vertices; } }

        public Vector3 position;
        public Color color;
        private Color darkColor;
        private int[] indices;

        public Voxel(Vector3 position, Color color)
        {
            this.position = position;
            this.color = color;
            this.darkColor = Color.Lerp(color, Color.Black, 0.5f);

            SetUpIndices();
            SetUpVertices();
        }

        private void SetUpIndices()
        {
            indices = new int[36];

            //**FRONT**//
            indices[0] = 2;
            indices[1] = 1;
            indices[2] = 0;
            indices[3] = 3;
            indices[4] = 2;
            indices[5] = 0;

            //**RIGHT**//
            indices[6] = 1;
            indices[7] = 2;
            indices[8] = 4;
            indices[9] = 4;
            indices[10] = 5;
            indices[11] = 1;

            //**LEFT**//
            indices[12] = 6;
            indices[13] = 3;
            indices[14] = 0;
            indices[15] = 0;
            indices[16] = 7;
            indices[17] = 6;

            //**BOTTOM**//
            indices[18] = 0;
            indices[19] = 1;
            indices[20] = 7;
            indices[21] = 1;
            indices[22] = 5;
            indices[23] = 7;

            //**TOP**//
            indices[24] = 2;
            indices[25] = 3;
            indices[26] = 6;
            indices[27] = 6;
            indices[28] = 4;
            indices[29] = 2;

            //**BACK**//
            indices[30] = 7;
            indices[31] = 4;
            indices[32] = 6;
            indices[33] = 7;
            indices[34] = 5;
            indices[35] = 4;
        }
          
        private void SetUpVertices()
        {
            vertices = new VertexPositionColor[8];            

            Vector3 xSize = new Vector3(SIZE.X, 0, 0);
            Vector3 ySize = new Vector3(0, SIZE.Y, 0);
            Vector3 zSize = new Vector3(0, 0, SIZE.Z);

            //**Front**//
            vertices[0].Position = position;
            vertices[0].Color = color;
            vertices[1].Position = position + xSize;
            vertices[1].Color = color;
            vertices[2].Position = position + xSize + ySize;
            vertices[2].Color = darkColor;
            vertices[3].Position = position + ySize;
            vertices[3].Color = darkColor;

            //**RIGHT**//            
            vertices[4].Position = position + xSize + ySize - zSize;
            vertices[4].Color = darkColor;
            vertices[5].Position = position + xSize - zSize;
            vertices[5].Color = color;

            //**LEFT**//
            vertices[6].Position = position + ySize - zSize;
            vertices[6].Color = darkColor;
            vertices[7].Position = position - zSize;
            vertices[7].Color = color;
        }

        public void Draw(GraphicsDevice device)
        {
            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionColor.VertexDeclaration);
        }
    }
}
