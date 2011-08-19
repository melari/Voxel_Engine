using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Voxel_Engine
{
    class FpsCounter
    {
        int fps = 0;
        int count = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                fps = count;
                count = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            count++;
            spriteBatch.DrawString(font, "FPS: " + fps.ToString(), new Vector2(33, 33), Color.Black);
            spriteBatch.DrawString(font, "FPS: " + fps.ToString(), new Vector2(32, 32), Color.White);
        }
    }
}
