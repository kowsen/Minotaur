using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;
using SampleLogic.Utilities;
// using Microsoft.Xna.Framework.Graphics;
using SampleLogic.Components;
using SampleLogic.Systems;

namespace SampleLogic.Scenes
{
    public class MainScene : Scene<SampleGameObject>
    {
        private string _playerTexture;
        private string _boxTexture;
        private string _directionsTexture;

        private Random _rand;

        public MainScene()
        {
            _rand = new Random();
        }

        public override void LoadContent()
        {
            // _playerTexture = Game.Content.Load<Texture2D>("player");
            // _boxTexture = Game.Content.Load<Texture2D>("box");
            // _directionsTexture = Game.Content.Load<Texture2D>("directions");
        }

        public override void Initialize()
        {
            var directionsEntity = Entities.Create();
            directionsEntity.AddComponent<PositionComponent>().Init(50, 400);
            directionsEntity.AddComponent<TextureComponent>().Init(_directionsTexture);

            var playerEntity = Entities.Create();
            playerEntity.AddComponent<PositionComponent>().Init(50, 50);
            playerEntity.AddComponent<MovementComponent>().Init(0, 0);
            playerEntity.AddComponent<PlayerFlagComponent>();
            playerEntity.AddComponent<TextureComponent>().Init(_playerTexture);

            for (var i = 0; i < 5; i++)
            {
                AddBox();
            }

            Systems.AddSystem(new PlayerInputSystem());
            Systems.AddSystem(new MovementSystem());
            Systems.AddSystem(new BounceNotifySystem());
            Systems.AddSystem(new BoxAddSystem(_boxTexture, _rand));

            Systems.AddSystem(new RenderSystem());
        }

        private void AddBox()
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
