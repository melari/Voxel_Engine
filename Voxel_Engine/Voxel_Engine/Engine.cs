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

        public static int RES_WIDTH = 1920;
        public static int RES_HEIGHT = 1080;
        public static bool FULLSCREEN = true;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;

        Effect effect;
        
        SpriteFont defFont;

        Camera camera;
        InputHandle input = new InputHandle();

        VoxelManager voxelManager;        
        Editor editor;

        RenderTarget2D renderTarget;
        Texture2D shadowMap;
                
        float lightAmbientStrength = 0.1f;        

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
       
        protected override void Initialize()
        {
            InitRes();
            Window.Title = "Panacea Voxel Engine v0.1_0";
            //graphics.PreferMultiSampling = false;           
            base.Initialize();
                        

            RasterizerState rs = new RasterizerState();
            if (!CULLING) { rs.CullMode = CullMode.None; }
            if (WIREFRAME) { rs.FillMode = FillMode.WireFrame; }            
            device.RasterizerState = rs;

            //device.BlendState = BlendState.AlphaBlend;            
        }

        public void InitRes()
        {            
            graphics.PreferredBackBufferWidth = RES_WIDTH;
            graphics.PreferredBackBufferHeight = RES_HEIGHT;
            graphics.IsFullScreen = FULLSCREEN;
            graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;
            TextureManager.Init(Content);

            effect = Content.Load<Effect>("effects");            
            effect.Parameters["xEnableLighting"].SetValue(LIGHTING);            
            effect.Parameters["xAmbient"].SetValue(lightAmbientStrength);
                        

            defFont = Content.Load<SpriteFont>("default");            
            voxelManager = new VoxelManager();

            PresentationParameters pp = device.PresentationParameters;
            renderTarget = new RenderTarget2D(device, pp.BackBufferWidth, pp.BackBufferHeight, true, device.DisplayMode.Format, DepthFormat.Depth24);
            
            camera = new Camera(75, MathHelper.ToRadians(30), new Vector3(VoxelManager.MAX_X / 2, 0, VoxelManager.MAX_Z / 2 + 3), device.Viewport.AspectRatio);
        }                                        
                
        protected override void Update(GameTime gameTime)
        {
            FpsCounter.Update(gameTime);
            input.update();
            
            if (input.getKey(Keys.Escape).down)
                this.Exit();

            camera.Update(input);
            if (editor != null) { editor.Update(input); }
            else if (input.getKey(Keys.OemTilde).released) { editor = new Editor(voxelManager); }
            
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {            
            camera.UpdateMatrices(effect);

            effect.CurrentTechnique = effect.Techniques["ShadowMap"];
            device.SetRenderTarget(renderTarget);
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Purple, 1.0f, 0);
                         
            device.BlendState = BlendState.NonPremultiplied;
            voxelManager.SendLightsToShader(effect);
                        
            voxelManager.Draw(device, effect);
            if (editor != null) { editor.Draw(device, effect); }


            //2D Overlay
            if (FpsCounter.ENABLED)
            {
                spriteBatch.Begin();
                FpsCounter.Draw(spriteBatch, defFont);
                spriteBatch.End();
            }
                       
            
            device.SetRenderTarget(null);
            shadowMap = (Texture2D)renderTarget;

            /*
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Green, 1.0f, 0);
            using (SpriteBatch sprite = new SpriteBatch(device))
             {
                 sprite.Begin();
                 sprite.Draw(shadowMap, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 1);
                 sprite.End();
             }
            */

            
            effect.Parameters["xShadowMap"].SetValue(shadowMap);
            effect.CurrentTechnique = effect.Techniques["Standard"];
            device.Clear(Color.Black);

            voxelManager.Draw(device, effect);
            if (editor != null) { editor.Draw(device, effect); }


            //2D Overlay
            if (FpsCounter.ENABLED)
            {
                spriteBatch.Begin();
                FpsCounter.Draw(spriteBatch, defFont);
                spriteBatch.End();
            }

            shadowMap = null;
            base.Draw(gameTime);
        }
    }
}
