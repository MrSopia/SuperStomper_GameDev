using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperStomper.GameScripts.Engine;
using SuperStomper.GameScripts.Game;

namespace SuperStomper
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Windowbox windowbox;
        private GameManager gameManager;

        public const int designedResolutionWidth = 320;
        public const int designedResolutionHeight = 256;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            //initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //this.Content to load your game content here
            _graphics.PreferredBackBufferWidth = designedResolutionWidth;
            _graphics.PreferredBackBufferHeight = designedResolutionHeight;
            _graphics.ApplyChanges();

            windowbox = new Windowbox(this, designedResolutionWidth, designedResolutionHeight);

            gameManager = new GameManager(Content, windowbox);
        }

        protected override void Update(GameTime gameTime)
        {
            // update logic here
            gameManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //drawing code here
            windowbox.Draw(_spriteBatch, gameManager.Draw, samplerState: SamplerState.PointClamp, transformMatrix: Matrix.CreateTranslation(GameManager.cameraPosition.X, 0, 0));

            base.Draw(gameTime);
        }
    }
}