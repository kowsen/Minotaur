using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;
using Microsoft.Xna.Framework.Graphics;

namespace BounceLogic.Components
{
    public class TextureComponent : IComponent
    {
        public Texture2D Texture;

        public TextureComponent(Texture2D texture)
        {
            Texture = texture;
        }
    }
}
