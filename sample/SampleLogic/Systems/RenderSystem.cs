using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;
// using Microsoft.Xna.Framework;
using SampleLogic.Components;
using SampleLogic.Utilities;

namespace SampleLogic.Systems
{
    public class RenderSystem : EntitySystem<SampleGameObject>
    {
        public RenderSystem()
        {
            Signature.SetRequirements(typeof(PositionComponent), typeof(TextureComponent));
        }

        public override void Draw(TimeSpan time, Entity entity)
        {
            var position = entity.GetComponent<PositionComponent>();
            var texture = entity.GetComponent<TextureComponent>().Texture;

            // Game.Batch.Draw(texture, new Vector2(position.X, position.Y), Color.White);
        }
    }
}
