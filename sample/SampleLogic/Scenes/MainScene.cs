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
            var directionsEntity = _entities.Create();
            directionsEntity.AddComponent<PositionComponent>().Init(50, 400);
            directionsEntity.AddComponent<TextureComponent>().Init(_directionsTexture);

            var playerEntity = _entities.Create();
            playerEntity.AddComponent<PositionComponent>().Init(50, 50);
            playerEntity.AddComponent<MovementComponent>().Init(0, 0);
            playerEntity.AddComponent<PlayerFlagComponent>();
            playerEntity.AddComponent<TextureComponent>().Init(_playerTexture);

            for (var i = 0; i < 5; i++)
            {
                AddBox();
            }

            _systems.AddSystem(new PlayerInputSystem());
            _systems.AddSystem(new MovementSystem());
            _systems.AddSystem(new BounceNotifySystem());
            _systems.AddSystem(new BoxAddSystem(_boxTexture, _rand));

            _systems.AddSystem(new RenderSystem());
        }

        private void AddBox()
        {
            var boxEntity = _entities.Create();
            boxEntity
                .AddComponent<PositionComponent>()
                .Init(
                    _rand.Next(0, _game.Viewport.VirtualWidth),
                    _rand.Next(0, _game.Viewport.VirtualHeight)
                );
            boxEntity.AddComponent<MovementComponent>().Init(_rand.Next(-2, 2), _rand.Next(-2, 2));
            boxEntity.AddComponent<TextureComponent>().Init(_boxTexture);
        }
    }
}
