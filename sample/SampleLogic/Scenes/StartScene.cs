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
    public class StartScene : Scene<SampleGameObject>
    {
        private string _clickTexture;

        public override void LoadContent()
        {
            // _clickTexture = Game.Content.Load<Texture2D>("click");
        }

        public override void Initialize()
        {
            var clickEntity = Entities.Create();
            clickEntity.AddComponent(new PositionComponent(100, 100));
            clickEntity.AddComponent(new TextureComponent(_clickTexture));

            Systems.AddSystem(new StartListenSystem());

            Systems.AddSystem(new RenderSystem());
        }
    }
}
