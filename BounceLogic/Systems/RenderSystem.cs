using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BounceLogic
{
    public class RenderSystem : EntitySystem
    {
        private SpriteBatch _spriteBatch;

        public RenderSystem(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            AddComponentConstraint<PositionComponent>();
            AddComponentConstraint<TextureComponent>();
        }

        public override void Draw(TimeSpan time, Entity entity)
        {
            var position = entity.GetComponent<PositionComponent>();
            var texture = entity.GetComponent<TextureComponent>();
            _spriteBatch.Draw(texture.Texture, new Vector2(position.X, position.Y), Color.Black);
        }
    }
}
