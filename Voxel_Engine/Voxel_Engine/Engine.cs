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

        public static int RES_WIDTH = 1600;
        public static int RES_HEIGHT = 900;
        public static bool FULLSCREEN = false;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;

        Effect effect;
        
        SpriteFont defFont;

        Camera camera;
        InputHandle input = new InputHandle();

        VoxelManager voxelManager;
        BufferedList<Light> lights = new BufferedList<Light>();
        Editor editor;
        
        Vector3 lightDirection = new Vector3(-0.455f, -0.455f, -0.091f);        
        float lightAmbientStrength = 0.5f;

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        SamplerState samplerState;
        protected override void Initialize()
        {
            InitRes();
            Window.Title = "Panacea Voxel Engine v0.1_0";
            graphics.PreferMultiSampling = false;           
            base.Initialize();
                        

            RasterizerState rs = new RasterizerState();
            if (!CULLING) { rs.CullMode = CullMode.None; }
            if (WIREFRAME) { rs.FillMode = FillMode.WireFrame; }            
            device.RasterizerState = rs;

            device.BlendState = BlendState.AlphaBlend;

            samplerState = new SamplerState();
            samplerState.Filter = TextureFilter.Point;
            device.SamplerStates[0] = samplerState;
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
            
            effect.CurrentTechnique = effect.Techniques["Textured"];

            defFont = Content.Load<SpriteFont>("default");            
            voxelManager = new VoxelManager();
            
            camera = new Camera(75, MathHelper.ToRadians(30), new Vector3(VoxelManager.MAX_X / 2, 0, VoxelManager.MAX_Z / 2 + 3), device.Viewport.AspectRatio);
        }                                        
                
        protected override void Update(GameTime gameTime)
        {
            FpsCounter.Update(gameTime);
            input.update();
            
            if (input.getKey(Keys.Escape).down)
                this.Exit();

            camera.Update(input);
            if (editor != null) { editor.Update(input, lights); }
            else if (input.getKey(Keys.OemTilde).released) { editor = new Editor(voxelManager, lights); }
            
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            camera.UpdateMatrices(effect);
            device.Clear(Color.Black);                                    
            device.BlendState = BlendState.NonPremultiplied;
                        
            voxelManager.Draw(device, effect, lights);
            if (editor != null) { editor.Draw(device, effect, lights); }


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
