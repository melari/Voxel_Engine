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
        FpsCounter fpsCounter = new FpsCounter();

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
            
            SetUpCamera();
        }                                        

        Matrix viewMatrix;
        Matrix projectionMatrix;
        Vector3 cameraPosition = new Vector3(0f, 20.0f, 50.0f);
        private void SetUpCamera()
        {
            //(From, To, Normal)//
            viewMatrix = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.UnitY);

            //(ViewAngle, resolutionRatio, MinDrawDist, MaxDrawDist)//
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1.0f, 300.0f);
        }


        private float angle = 0.0f;
        private KeyboardState prev_ks = new KeyboardState();
        protected override void Update(GameTime gameTime)
        {
            if (fpsCounter != null) { fpsCounter.Update(gameTime); }

            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Escape))
                this.Exit();            

            angle += 0.005f;

            if (ks.IsKeyDown(Keys.A))
            {
                cameraPosition.X -= 0.1f;
            }

            if (ks.IsKeyDown(Keys.D))
            {
                cameraPosition.X += 0.1f;
            }

            prev_ks = ks;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            SetUpCamera();
            device.Clear(Color.Black);

            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);

            //Matrix worldMatrix = Matrix.CreateTranslation(-20.0f / 3.0f, -10.0f / 3.0f, 0.0f);// *Matrix.CreateRotationZ(3 * angle);
            //effect.Parameters["xWorld"].SetValue(worldMatrix);
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);

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
            spriteBatch.Begin();
            if (fpsCounter != null) { fpsCounter.Draw(spriteBatch, defFont); }            
            spriteBatch.End();
            

            base.Draw(gameTime);
        }
    }
}
