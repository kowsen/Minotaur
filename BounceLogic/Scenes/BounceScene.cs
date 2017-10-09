using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BounceLogic
{
    public class BounceScene : Scene
    {
        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private int _width;
        private int _height;

        public BounceScene(ContentManager content, SpriteBatch spriteBatch, int width, int height) : base()
        {
            _content = content;
            _spriteBatch = spriteBatch;
            _width = width;
            _height = height;
        }

        public override void Initialize(EventBus bus)
        {
            base.Initialize(bus);
            var sprite = _content.Load<Texture2D>("sprite");

            var r = new Random();
            for (var i = 0; i < 1000; i++)
            {
                AddBounceBox(r.Next(0, 400), r.Next(0, 400), r.Next(1, 5), r.Next(1, 5), sprite);
            }

            var entity2 = _ecm.CreateEntity();
            entity2.AddComponent(new PositionComponent(100, 200));
            entity2.AddComponent(new TextureComponent(sprite));

            _sm.AddSystem(new RenderSystem(_spriteBatch));
            _sm.AddSystem(new MovementSystem());
            _sm.AddSystem(new BounceSystem(_width, _height));
            _sm.AddSystem(new EndMessengerSystem(_bus, "bounce_end", 5000));
        }

        private void AddBounceBox(int x, int y, int vx, int vy, Texture2D sprite)
        {
            var entity = _ecm.CreateEntity();
            entity.AddComponent(new PositionComponent(x, y));
            entity.AddComponent(new VelocityComponent(vx, vy));
            entity.AddComponent(new TextureComponent(sprite));
        }
    }
}
