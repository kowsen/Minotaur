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
            AddComponentRequirement<PositionComponent>();
            AddComponentRequirement<TextureComponent>();
        }

        //public override void Draw(TimeSpan time, Entity entity)
        //{
        //    var position = entity.GetComponent<PositionComponent>();
        //    var texture = entity.GetComponent<TextureComponent>();
        //    _spriteBatch.Draw(texture.Texture, new Vector2(position.X, position.Y), Color.Black);
        //}

        public override void Draw(TimeSpan time, EntitySet entities)
        {
            var bigEntities = entities.Query(ComponentSignatureManager.GenerateComponentSignature(new List<Type>() { typeof(BigFlagComponent) }, null));
            for (var i = 0; i < bigEntities.Entities.Count; i++)
            {
                DrawEntity(bigEntities.Entities[i], 2);
            }

            var smallEntities = entities.Query(ComponentSignatureManager.GenerateComponentSignature(null, new List<Type>() { typeof(BigFlagComponent) }));
            for (var i = 0; i < smallEntities.Entities.Count; i++)
            {
                DrawEntity(smallEntities.Entities[i], 1);
            }
        }

        private void DrawEntity(Entity entity, float scale)
        {
            var position = entity.GetComponent<PositionComponent>();
            var texture = entity.GetComponent<TextureComponent>();
            _spriteBatch.Draw(texture.Texture, new Vector2(position.X, position.Y), Color.Black);
            _spriteBatch.Draw(texture.Texture, new Vector2(position.X, position.Y), null, Color.Black, 0, new Vector2(0, 0), scale, SpriteEffects.None, 1);
        }
    }
}
