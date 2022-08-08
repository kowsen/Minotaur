using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;
using SampleLogic.Utilities;
using SampleLogic.Components;

// using Microsoft.Xna.Framework.Graphics;

namespace SampleLogic.Systems
{
    public class BoxAddSystem : GameSystem<SampleGameObject>
    {
        private string _boxTexture;
        private Random _rand;

        public BoxAddSystem(string boxTexture, Random rand)
        {
            _boxTexture = boxTexture;
            _rand = rand;
        }

        public override void OnActivate()
        {
            Game.Bus.Register<SpacePressEvent>(AddBox);
        }

        public override void OnDeactivate()
        {
            Game.Bus.Remove<SpacePressEvent>(AddBox);
        }

        private void AddBox(SpacePressEvent spacePressEvent)
        {
            var boxEntity = Entities.Create();
            boxEntity
                .AddComponent<PositionComponent>()
                .Init(
                    _rand.Next(0, Game.Viewport.VirtualWidth),
                    _rand.Next(0, Game.Viewport.VirtualHeight)
                );
            boxEntity.AddComponent<MovementComponent>().Init(_rand.Next(-2, 2), _rand.Next(-2, 2));
            boxEntity.AddComponent<TextureComponent>().Init(_boxTexture);
        }
    }
}
