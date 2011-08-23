using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Voxel_Engine
{
    static class FpsCounter
    {
        public static bool ENABLED = true;

        static int fps = 0;
        static int count = 0;
        static TimeSpan elapsedTime = TimeSpan.Zero;

        public static void Update(GameTime gameTime)
        {
            if (!ENABLED) { return; }

            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                fps = count;
                count = 0;
            }
        }

        public static void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            if (!ENABLED) { return; }

            count++;
            spriteBatch.DrawString(font, "FPS: " + fps.ToString(), new Vector2(33, 33), Color.Black);
            spriteBatch.DrawString(font, "FPS: " + fps.ToString(), new Vector2(32, 32), Color.White);
        }
    }
}
