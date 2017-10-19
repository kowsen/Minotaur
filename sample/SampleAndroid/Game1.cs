using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.ViewportAdapters;
using SampleLogic;
using SampleLogic.Scenes;
using SampleLogic.Utilities;
using System;

namespace SampleAndroid
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
        TouchListener touch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 540;
            graphics.PreferredBackBufferHeight = 960;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;

            touch = new TouchListener(new TouchListenerSettings());
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GraphicsDevice.Viewport = new Viewport(0, 0, 540, 960);
            viewport = new BoxingViewportAdapter(Window, GraphicsDevice, 540, 960, 0, 0);

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
            Point startPoint = new Point();
            touch.TouchStarted += HandleTouchStart;
            touch.TouchEnded += HandleTouchEnd;

            void HandleTouchStart(object sender, TouchEventArgs args)
            {
                startPoint = args.Position;
            }

            void HandleTouchEnd(object sender, TouchEventArgs args)
            {
                var endPoint = args.Position;
                HandleSwipe(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
            }

            void HandleSwipe(int dx, int dy)
            {
                if (Math.Abs(dx) + Math.Abs(dy) < 50)
                {
                    game.Bus.Notify(Events.SPACE_PRESS);
                }
                else if (-dy > Math.Abs(dx))
                {
                    // NORTH
                    ResetInputState(game.Input);
                    game.Input.IsUp = true;
                }
                else if (dx >= Math.Abs(dy))
                {
                    // EAST
                    ResetInputState(game.Input);
                    game.Input.IsRight = true;
                }
                else if (dy > Math.Abs(dx))
                {
                    // SOUTH
                    ResetInputState(game.Input);
                    game.Input.IsDown = true;
                }
                else if (-dx >= Math.Abs(dy))
                {
                    // WEST
                    ResetInputState(game.Input);
                    game.Input.IsLeft = true;
                }
            }

            void ResetInputState(InputState state)
            {
                game.Input.IsUp = false;
                game.Input.IsRight = false;
                game.Input.IsDown = false;
                game.Input.IsLeft = false;
            }
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            touch.Update(gameTime);
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
