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
        public Vector3 lookToPosition;
        float ratio;
        float distance = 0;
        float angle = 0;

        public Camera(Vector3 position, Vector3 lookToPosition, float ratio)
        {
            this.position = position;
            this.lookToPosition = lookToPosition;
            this.ratio = ratio;
        }
        public Camera(float distance, float angle, Vector3 lookToPosition, float ratio)
            : this(lookToPosition + new Vector3(0, (float)(distance * Math.Cos(angle)), (float)(distance * Math.Sin(angle))), lookToPosition, ratio)
        {
            this.distance = distance;
            this.angle = angle;
        }

        public void Update(InputHandle input)
        {
            if (input.getKey(Keys.A).down)
            {
                position.X -= 0.5f;
                lookToPosition.X -= 0.5f;
            }

            if (input.getKey(Keys.D).down)
            {
                position.X += 0.5f;
                lookToPosition.X += 0.5f;
            }

            if (input.getKey(Keys.W).down)
            {
                position.Z -= 0.5f;
                lookToPosition.Z -= 0.5f;
            }

            if (input.getKey(Keys.S).down)
            {
                position.Z += 0.5f;
                lookToPosition.Z += 0.5f;
            }

            if (input.leftMouseDown)
            {
                distance--;
                position = lookToPosition + new Vector3(0, (float)(distance * Math.Cos(angle)), (float)(distance * Math.Sin(angle)));
            }
            if (input.rightMouseDown)
            {
                distance++;
                position = lookToPosition + new Vector3(0, (float)(distance * Math.Cos(angle)), (float)(distance * Math.Sin(angle)));
            }
        }

        public void UpdateMatrices(Effect effect)
        {
            //(From, To, Normal)//
            viewMatrix = Matrix.CreateLookAt(position, lookToPosition, Vector3.UnitY);

            //(ViewAngle, resolutionRatio, MinDrawDist, MaxDrawDist)//
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, ratio, 1.0f, 300.0f);

            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
        }
    }
}
