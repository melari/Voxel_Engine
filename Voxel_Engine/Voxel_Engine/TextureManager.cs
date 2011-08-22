using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Voxel_Engine
{    
    static class TextureManager
    {
        private class Tex
        {
            public Texture2D texture;
            public bool containsAlpha;

            public Tex(Texture2D texture, bool containsAlpha)
            {
                this.texture = texture;
                this.containsAlpha = containsAlpha;
            }
        }

        private static Dictionary<string, List<Tex>> textures = new Dictionary<string, List<Tex>>();

        public static void Init(ContentManager manager)
        {            
            AddTexture("trunk", manager.Load<Texture2D>("trunk"), false);
            AddTexture("leaves", manager.Load<Texture2D>("leaves"), true);

            AddTexture("grass", manager.Load<Texture2D>("grass1"), false);
            //AddTexture("grass", manager.Load<Texture2D>("grass2"));
        }

        public static void AddTexture(string name, Texture2D texture, bool containsAlpha)
        {
            if (!textures.ContainsKey(name))
                textures.Add(name, new List<Tex>());
            textures[name].Add(new Tex(texture, containsAlpha));                
        }

        public static Texture2D getTexture(string name)
        {
            return textures[name][MathExtra.rand.Next(textures[name].Count)].texture;
        }

        public static bool containsAlpha(string name)
        {
            return textures[name][0].containsAlpha;
        }
    }
}
