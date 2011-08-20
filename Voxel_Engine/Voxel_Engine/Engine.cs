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
        public static bool LIGHTING = true;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;

        Effect effect;
        
        SpriteFont defFont;

        Camera camera;
        InputHandle input = new InputHandle();
        VoxelManager voxelManager;

        Texture2D red, blue;
        Vector3 lightDirection = new Vector3(-0.455f, -0.455f, -0.091f);
        float lightDirectionalStrength = 1.0f;
        float lightAmbientStrength = 0.1f;

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
        }        

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;

            effect = Content.Load<Effect>("effects");
            defFont = Content.Load<SpriteFont>("default");
            red = Content.Load<Texture2D>("red");
            blue = Content.Load<Texture2D>("blue");
            voxelManager = new VoxelManager(red, blue);

            camera = new Camera(new Vector3(0f, 20.0f, 50.0f), device.Viewport.AspectRatio);
            
        }                                        
                
        protected override void Update(GameTime gameTime)
        {
            FpsCounter.Update(gameTime);
            input.update();
            
            if (input.getKey(Keys.Escape).down)
                this.Exit();

            camera.Update(input);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            camera.UpdateMatrices(effect);            

            effect.Parameters["xEnableLighting"].SetValue(LIGHTING);
            //lightDirection = -camera.position;
            //lightDirection.Normalize();
            effect.Parameters["xLightDirection"].SetValue(lightDirection * lightDirectionalStrength);
            effect.Parameters["xAmbient"].SetValue(lightAmbientStrength);

            device.Clear(Color.Black);

            effect.CurrentTechnique = effect.Techniques["Textured"];            
            voxelManager.Draw(device, effect);


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
