using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Voxel_Engine
{
    public class Engine : Microsoft.Xna.Framework.Game
    {
        public static bool CULLING = true;
        public static bool WIREFRAME = false;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;

        Effect effect;

        List<Voxel> voxels = new List<Voxel>();
        SpriteFont defFont;

        Camera camera;
        InputHandle input = new InputHandle();

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            Window.Title = "Panacea Voxel Engine v0.1_0";
            graphics.PreferMultiSampling = true;           
            base.Initialize();


            RasterizerState rs = new RasterizerState();
            if (!CULLING) { rs.CullMode = CullMode.None; }
            if (WIREFRAME) { rs.FillMode = FillMode.WireFrame; }
            device.RasterizerState = rs;

            for (int z = -10; z < 10; z++)
            {
                for (int y = -10; y < 10; y++)
                {
                    for (int x = -10; x < 10; x++)
                    {
                        voxels.Add(new Voxel(new Vector3(x, y, z), Color.Red));
                    }
                }
            }
        }        

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;

            effect = Content.Load<Effect>("effects");
            defFont = Content.Load<SpriteFont>("default");

            camera = new Camera(new Vector3(0f, 20.0f, 50.0f), device.Viewport.AspectRatio);
            
        }                                        
        
        private KeyboardState prev_ks = new KeyboardState();
        protected override void Update(GameTime gameTime)
        {
            FpsCounter.Update(gameTime);
            input.update();
            
            if (input.getKey(Keys.Escape).down)
                this.Exit();            
            
            if (input.getKey(Keys.A).down)
            {
                camera.position.X -= 0.1f;
            }

            if (input.getKey(Keys.D).down)
            {
                camera.position.X += 0.1f;
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            camera.UpdateMatrices(effect);
            device.Clear(Color.Black);

            effect.CurrentTechnique = effect.Techniques["ColoredNoShading"];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                foreach (Voxel voxel in voxels)
                {
                    voxel.Draw(device);
                }
            }


            //2D Overlay
            if (FpsCounter.ENABLED)
            {
                spriteBatch.Begin();
                FpsCounter.Draw(spriteBatch, defFont);
                spriteBatch.End();
            }
            

            base.Draw(gameTime);
        }
    }
}
