using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Minotaur;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleLogic.Utilities
{
    public class SampleGameObject
    {
        public ContentManager Content { get; private set; }
        public SpriteBatch Batch { get; private set; }
        public GraphicsDevice Graphics { get; private set; }
        public ViewportAdapter Viewport { get; private set; }
        public EventBus<Events> Bus { get; private set; }
        public InputState Input { get; private set; }
        public SampleGameState State { get; private set; }

        public SampleGameObject(ContentManager content, SpriteBatch batch, GraphicsDevice graphics, ViewportAdapter viewport)
        {
            Content = content;
            Batch = batch;
            Graphics = graphics;
            Viewport = viewport;
            Bus = new EventBus<Events>();
            Input = new InputState();
            State = new SampleGameState();
        }
    }
}
