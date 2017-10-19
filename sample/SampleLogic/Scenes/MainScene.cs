using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;
using SampleLogic.Utilities;
using Microsoft.Xna.Framework.Graphics;
using SampleLogic.Components;
using SampleLogic.Systems;

namespace SampleLogic.Scenes
{
    public class MainScene : Scene<SampleGameObject>
    {
        private Texture2D _playerTexture;
        private Texture2D _boxTexture;
        private Texture2D _directionsTexture;

        private Random _rand;

        public MainScene()
        {
            _rand = new Random();
        }

        public override void LoadContent()
        {
            _playerTexture = _game.Content.Load<Texture2D>("player");
            _boxTexture = _game.Content.Load<Texture2D>("box");
            _directionsTexture = _game.Content.Load<Texture2D>("directions");
        }

        public override void Initialize()
        {
            var directionsEntity = _ecm.CreateEntity();
            directionsEntity.AddComponent(new PositionComponent(50, 400));
            directionsEntity.AddComponent(new TextureComponent(_directionsTexture));

            var playerEntity = _ecm.CreateEntity();
            playerEntity.AddComponent(new PositionComponent(50, 50));
            playerEntity.AddComponent(new MovementComponent(0, 0));
            playerEntity.AddComponent(new PlayerFlagComponent());
            playerEntity.AddComponent(new TextureComponent(_playerTexture));

            for (var i = 0; i < 5; i++)
            {
                AddBox();
            }

            _sm.AddSystem(new PlayerInputSystem());
            _sm.AddSystem(new MovementSystem());
            _sm.AddSystem(new BounceNotifySystem());
            _sm.AddSystem(new BoxAddSystem(_boxTexture, _rand));
            
            _sm.AddSystem(new RenderSystem());
        }

        private void AddBox()
        {
            var boxEntity = _ecm.CreateEntity();
            boxEntity.AddComponent(new PositionComponent(_rand.Next(0, 500), _rand.Next(0, 500)));
            boxEntity.AddComponent(new MovementComponent(_rand.Next(-2, 2), _rand.Next(-2, 2)));
            boxEntity.AddComponent(new TextureComponent(_boxTexture));
        }
    }
}
