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
    public class StartScene : Scene<SampleGameObject>
    {
        private Texture2D _clickTexture;

        public override void LoadContent()
        {
            _clickTexture = _game.Content.Load<Texture2D>("click");
        }

        public override void Initialize()
        {
            var clickEntity = _ecm.CreateEntity();
            clickEntity.AddComponent(new PositionComponent(100, 100));
            clickEntity.AddComponent(new TextureComponent(_clickTexture));

            _sm.AddSystem(new StartListenSystem());

            _sm.AddSystem(new RenderSystem());
        }
    }
}
