using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

// using Microsoft.Xna.Framework.Graphics;

namespace SampleLogic.Components
{
    public class TextureComponent : Component
    {
        public string Texture { get; set; }

        public void Init(string texture)
        {
            Texture = texture;
        }

        public override void Reset()
        {
            Texture = "";
        }
    }
}
