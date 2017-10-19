using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Minotaur;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.ViewportAdapters;
using SampleLogic;
using SampleLogic.Scenes;
using SampleLogic.Utilities;

namespace SampleWindows
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ViewportAdapter viewport;
        SampleWorld world;
        KeyboardListener keyboard;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 500;

            keyboard = new KeyboardListener(new KeyboardListenerSettings());
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GraphicsDevice.Viewport = new Viewport(0, 0, 500, 500);
            viewport = new BoxingViewportAdapter(Window, GraphicsDevice, 500, 500, 0, 0);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            var gameObject = new SampleGameObject(Content, spriteBatch, GraphicsDevice, viewport);
            InitializeInput(gameObject);

            world = new SampleWorld(gameObject);
            world.AddScene("start", new StartScene());
            world.AddScene("main", new MainScene());

            base.Initialize();

            world.Initialize();
            world.Switch("start");
        }

        private void InitializeInput(SampleGameObject game)
        {
            keyboard.KeyPressed += (sender, args) =>
            {
                var key = args.Key;
                if (key == Keys.Up)
                {
                    game.Input.IsUp = true;
                }
                else if (key == Keys.Right)
                {
                    game.Input.IsRight = true;
                }
                else if (key == Keys.Down)
                {
                    game.Input.IsDown = true;
                }
                else if (key == Keys.Left)
                {
                    game.Input.IsLeft = true;
                }
                else if (key == Keys.Space)
                {
                    game.Bus.Notify(Events.SPACE_PRESS);
                }
            };

            keyboard.KeyReleased += (sender, args) =>
            {
                var key = args.Key;
                if (key == Keys.Up)
                {
                    game.Input.IsUp = false;
                }
                else if (key == Keys.Right)
                {
                    game.Input.IsRight = false;
                }
                else if (key == Keys.Down)
                {
                    game.Input.IsDown = false;
                }
                else if (key == Keys.Left)
                {
                    game.Input.IsLeft = false;
                }
            };
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            world.LoadContent();
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keyboard.Update(gameTime);
            world.Update(gameTime.ElapsedGameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(transformMatrix: viewport.GetScaleMatrix());
            world.Draw(gameTime.ElapsedGameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
