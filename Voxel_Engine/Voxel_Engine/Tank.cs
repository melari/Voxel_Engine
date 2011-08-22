using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Voxel_Engine
{
    class Tank
    {
        Vector3 position;
        Vector3 velocity = Vector3.Zero;
        float acceleration = 0.2f;

        public Tank(Vector3 position)
        {
            this.position = position;
        }

        public void Update(InputHandle input)
        {
            if (input.getKey(Keys.Left).down)
            {
                velocity.X -= acceleration;
            }

            position += velocity;
        }

        public void Draw(GraphicsDevice device)
        {

        }
    }
}
