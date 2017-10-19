using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;
using SampleLogic.Utilities;
using SampleLogic.Components;
using Microsoft.Xna.Framework.Graphics;

namespace SampleLogic.Systems
{
    public class BoxAddSystem : GameSystem<SampleGameObject>
    {
        private Texture2D _boxTexture;
        private Random _rand;
        
        public BoxAddSystem(Texture2D boxTexture, Random rand)
        {
            _boxTexture = boxTexture;
            _rand = rand;
        }

        public override void OnActivate()
        {
            Game.Bus.Register(Events.SPACE_PRESS, AddBox);
        }

        public override void OnDeactivate()
        {
            Game.Bus.Remove(Events.SPACE_PRESS, AddBox);
        }

        private void AddBox(EventBusArgs args)
        {
            var boxEntity = CreateEntity();
            boxEntity.AddComponent(new PositionComponent(_rand.Next(0, Game.Viewport.VirtualWidth), _rand.Next(0, Game.Viewport.VirtualHeight)));
            boxEntity.AddComponent(new MovementComponent(_rand.Next(-2, 2), _rand.Next(-2, 2)));
            boxEntity.AddComponent(new TextureComponent(_boxTexture));
        }
    }
}
