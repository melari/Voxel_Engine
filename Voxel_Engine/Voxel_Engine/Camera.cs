using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Voxel_Engine
{
    class Camera
    {
        Matrix viewMatrix;        
        Matrix projectionMatrix;

        public Vector3 position;
        float ratio;

        public Camera(Vector3 position, float ratio)
        {
            this.position = position;
            this.ratio = ratio;
        }

        public void Update(InputHandle input)
        {
            if (input.getKey(Keys.A).down)
            {
                position.X -= 0.5f;
            }

            if (input.getKey(Keys.D).down)
            {
                position.X += 0.5f;
            }

            if (input.getKey(Keys.W).down)
            {
                position.Z -= 0.5f;
            }

            if (input.getKey(Keys.S).down)
            {
                position.Z += 0.5f;
            }
        }

        public void UpdateMatrices(Effect effect)
        {
            //(From, To, Normal)//
            viewMatrix = Matrix.CreateLookAt(position, Vector3.Zero, Vector3.UnitY);

            //(ViewAngle, resolutionRatio, MinDrawDist, MaxDrawDist)//
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, ratio, 1.0f, 300.0f);

            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
        }
    }
}
