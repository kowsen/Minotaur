using Game1;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Minotaur;
using System;

namespace AndroidMinotaur
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        EntityComponentManager _ecs;
        SystemManager _sys;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            _ecs = new EntityComponentManager();
            _sys = new SystemManager(_ecs);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            var sprite = Content.Load<Texture2D>("sprite");

            var r = new Random();
            for (var i = 0; i < 100; i++)
            {
                AddBounceBox(r.Next(0, 400), r.Next(0, 400), r.Next(1, 5), r.Next(1, 5), sprite);
            }

            var entity2 = _ecs.CreateEntity();
            entity2.AddComponent(new PositionComponent(100, 200));
            entity2.AddComponent(new TextureComponent(sprite));

            _sys.AddSystem(new RenderSystem(spriteBatch));
            _sys.AddSystem(new MovementSystem());
            _sys.AddSystem(new BounceSystem(GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height));
        }

        private void AddBounceBox(int x, int y, int vx, int vy, Texture2D sprite)
        {
            var entity = _ecs.CreateEntity();
            entity.AddComponent(new PositionComponent(x, y));
            entity.AddComponent(new VelocityComponent(vx, vy));
            entity.AddComponent(new TextureComponent(sprite));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            // TODO: Add your update logic here
            _sys.Update(gameTime.ElapsedGameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            _sys.Draw(gameTime.ElapsedGameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
