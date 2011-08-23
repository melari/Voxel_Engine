using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Voxel_Engine
{
    class Editor
    {
        VoxelManager manager;
        Vector3 position = Vector3.Zero;
        Voxel toCreate;
        Light light;        
        bool stack = true;

        string[] textures = new string[] { "leaves", "trunk", "grass", "cobblestone", "torch" };
        int texture_index = 0;
        string texture { get { return textures[texture_index]; } }

        public Editor(VoxelManager manager)
        {
            this.manager = manager;
            toCreate = new Voxel(manager, position, TextureManager.getTexture(texture), TextureManager.containsAlpha(texture), TextureManager.isCube(texture));
            light = new Light(position, 10);        
            manager.lights.Add(light);
        }

        public void Update(InputHandle input)
        {
            if (input.getKey(Keys.RightControl).pressed) { stack = !stack; }

            if ((input.getKey(Keys.Up).pressed || (input.getKey(Keys.Up).down && input.getKey(Keys.LeftShift).down)) && position.Z > 0)
                position.Z--;
            if ((input.getKey(Keys.Down).pressed || (input.getKey(Keys.Down).down && input.getKey(Keys.LeftShift).down)) && position.Z < VoxelManager.MAX_Z - 1)
                position.Z++;
            if ((input.getKey(Keys.Left).pressed || (input.getKey(Keys.Left).down && input.getKey(Keys.LeftShift).down)) && position.X > 0)
                position.X--;
            if ((input.getKey(Keys.Right).pressed || (input.getKey(Keys.Right).down && input.getKey(Keys.LeftShift).down)) && position.X < VoxelManager.MAX_X - 1)
                position.X++;

            if (stack) { position.Y = manager.FindTopSpace((int)position.X, (int)position.Z); }
            else
            {
                if (input.getKey(Keys.PageUp).pressed && position.Y < VoxelManager.MAX_Y - 1) { position.Y++; }
                if (input.getKey(Keys.PageDown).pressed && position.Y > 0) { position.Y--; }
            }

            light.position = position;
            toCreate.position = position;
            toCreate.ForceVerticeUpdate();            

            if (input.getKey(Keys.RightShift).pressed)
            {
                manager.lights.Add(new Light(position, 10));
            }


            if (input.getKey(Keys.Space).pressed && position.Y != -1)
            {
                manager.Add(toCreate);
                bool test = TextureManager.containsAlpha(texture);
                toCreate = new Voxel(manager, position, TextureManager.getTexture(texture), TextureManager.containsAlpha(texture), TextureManager.isCube(texture), texture == "torch");
            }

            if (input.getKey(Keys.P).pressed) //clear map
            {
                manager.Clear();
            }


            if (input.getKey(Keys.OemPeriod).pressed)
            {
                texture_index++;
                if (texture_index >= textures.Count()) { texture_index = 0; }
                toCreate.texture = TextureManager.getTexture(texture);
                toCreate.alpha = TextureManager.containsAlpha(texture);
                toCreate.cube = TextureManager.isCube(texture);
            }
            if (input.getKey(Keys.OemComma).pressed)
            {
                texture_index--;
                if (texture_index < 0) { texture_index = textures.Count()-1; }
                toCreate.texture = TextureManager.getTexture(texture);
                toCreate.alpha = TextureManager.containsAlpha(texture);
                toCreate.cube = TextureManager.isCube(texture);
            }
        }        

        public void Draw(GraphicsDevice device, Effect effect)
        {
            toCreate.Draw(device, effect);
        }
    }
}
