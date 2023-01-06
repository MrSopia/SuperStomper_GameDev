using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperStomper.GameScripts.Engine;
using System;

namespace SuperStomper.GameScripts.Game
{
    internal class GameManager
    {
        private enum GameState { MainMenu, LevelSelect, Playing, GameOver, Won}

        public static Vector2 cameraPosition;

        private MapLoader levelLoader;
        private int currentLevel;
        private int coinsCollected;
        private int allCoinsCollected;
        private float coinPitch;
        private float lastTimePickedACoin;
        private SoundEffect coinSound;
        private GameState gameState;
        private SpriteFont font;
        private int selected;
        private KeyboardState lastKeyboardState;
        private int textFadeDirection;
        private float textFadeValue;
        private CoinUI coinUI;

        private readonly Windowbox windowbox;
        private readonly ContentManager content;
        private readonly Color selectColor;

        private const float resetCoinPitchAfter = 0.5f;
        private const string gameName = "Super Stomper";
        private const string startText = "Start";
        private const string levelSelectText = "Select Level";
        private const string gameOverText = "Game Over!";
        private const string wonText = "You Won!";
        private const string backToMainMenuText = "Press Escape to Start Over";
        private const int numberOfLevels = 3;

        public GameManager(ContentManager content, Windowbox windowbox)
        {
            this.windowbox = windowbox;
            this.content = content;
            selectColor = Color.Gray;
            selected = 0;

            coinPitch = 0;
            lastTimePickedACoin = 0;
            coinSound = content.Load<SoundEffect>(@"SoundEffects\Coin");
            currentLevel = 1;
            cameraPosition = Vector2.Zero;
            coinsCollected = 0;
            allCoinsCollected = 0;
            gameState = GameState.MainMenu;
            font = content.Load<SpriteFont>(@"Fonts\Font");
            lastKeyboardState = Keyboard.GetState();
            textFadeDirection = 1;
            textFadeValue = 0;
            coinUI = new CoinUI(content, windowbox, font);
        }

        public void Reset()
        {
            selected = 0;
            cameraPosition = Vector2.Zero;
            coinsCollected = 0;
        }

        public void Update(float deltaTime)
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                    allCoinsCollected = 0; // coin counter
                    currentLevel = 1;
                    if (Keyboard.GetState().IsKeyDown(Keys.Down) && !lastKeyboardState.IsKeyDown(Keys.Down))
                    {
                        selected++;
                        selected %= 2;

                        selected -= (selected / 2) * 2;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Up) && !lastKeyboardState.IsKeyDown(Keys.Up))
                    {
                        selected--;
                        selected %= 2;
                        selected = Math.Abs(selected);

                        selected += (selected / 2) * 2;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        // Start Game
                        if (selected == 0)
                        {
                            gameState = GameState.Playing;
                            levelLoader = new MapLoader(content, currentLevel);
                        }
                        else if (selected == 1) // Select Level
                        {
                            selected = 0;
                            gameState = GameState.LevelSelect;
                        }
                    }

                    break;
                case GameState.LevelSelect:
                    if (Keyboard.GetState().IsKeyDown(Keys.Down) && !lastKeyboardState.IsKeyDown(Keys.Down))
                    {
                        selected++;
                        selected %= numberOfLevels;

                        selected -= (selected / numberOfLevels) * numberOfLevels;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Up) && !lastKeyboardState.IsKeyDown(Keys.Up))
                    {
                        selected = selected > 0 ? selected - 1 : numberOfLevels - 1;
                        selected %= numberOfLevels;
                        selected = Math.Abs(selected);

                        selected += (selected / numberOfLevels) * numberOfLevels;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !lastKeyboardState.IsKeyDown(Keys.Enter))
                    {
                        // Select level
                        gameState = GameState.Playing;
                        currentLevel = selected + 1;
                        levelLoader = new MapLoader(content, selected + 1);
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        selected = 0;
                        gameState = GameState.Playing;
                    }

                    break;
                case GameState.Playing:
                    coinUI.Update(deltaTime);

                    Mario mario = levelLoader.mario;

                    // Game Over
                    if (mario.gameOver)
                    {
                        Reset();
                        gameState = GameState.GameOver;
                        break;
                    }

                    // Won the game
                    if (mario.hitbox.IsTouching(levelLoader.castle.hitbox))
                    {
                        if (currentLevel == numberOfLevels)
                        {
                            Reset();
                            gameState = GameState.Won;
                            break;
                        }
                        else
                        {
                            Reset();
                            levelLoader = new MapLoader(content, ++currentLevel);
                            break;
                        }
                    }

                    foreach (Hitbox hitbox in levelLoader.colliders)
                    {
                        // Mario collision handling (Ignore collisions if mario is dead)
                        if (mario.lives != 0)
                            levelLoader.mario.physics.CollisionHandling(mario.movement, mario.hitbox, hitbox);

                        // Gombas collision handling
                        foreach (Gomba gomba in levelLoader.gombas)
                        {
                            int collisionType = gomba.physics.CollisionHandling(gomba.movement, gomba.hitbox, hitbox);

                            if (collisionType == 1 || collisionType == 3)
                                gomba.direction *= -1;
                        }
                    }

                    mario.physics.velocity = mario.physics.desiredVelocity;

                    foreach (Gomba gomba in levelLoader.gombas)
                        gomba.physics.velocity = gomba.physics.desiredVelocity;

                    mario.Update(deltaTime);

                    for (int i = 0; i < levelLoader.gombas.Count; i++)
                    {
                        levelLoader.gombas[i].Update(deltaTime);

                        // Remove Gomba after it has been stomped on for some time
                        if (levelLoader.gombas[i].shouldBeRemoved)
                        {
                            levelLoader.gombas.RemoveAt(i--);
                            continue;
                        }

                        // Stomp on Gombas if you land vertically on one
                        int collisionType = levelLoader.gombas[i].physics.CollisionHandling(mario.movement, mario.hitbox, levelLoader.gombas[i].hitbox, true);
                        if (!levelLoader.gombas[i].stompedOn && (collisionType == 2 || collisionType == 3) && mario.physics.velocity.Y > 0 && mario.lives > 0)
                        {
                            levelLoader.gombas[i].stompedOn = true;
                            levelLoader.gombas[i].physics.desiredVelocity.X = 0;
                            mario.physics.desiredVelocity.Y = -Gomba.stompRepulsionForce;
                        }
                        else if ((collisionType == 1 || (collisionType == 0 && mario.hitbox.IsTouching(levelLoader.gombas[i].hitbox))) && !levelLoader.gombas[i].stompedOn) // Touched a Gomba from the side
                        {
                            // Damage mario
                            mario.DamageMario();
                        }
                    }

                    // Collision with coins
                    for (int i = 0; i < levelLoader.coins.Count; i++)
                    {
                        levelLoader.coins[i].Update(deltaTime);

                        if (mario.hitbox.IsTouching(levelLoader.coins[i].hitbox))
                        {
                            // Coind sound effect
                            if (lastTimePickedACoin >= resetCoinPitchAfter)
                                coinPitch = 0;
                            else
                                coinPitch = System.Math.Clamp(coinPitch + 0.1f, 0, 1);

                            lastTimePickedACoin = 0;

                            coinSound.Play(1, coinPitch, 0);

                            coinsCollected++;
                            allCoinsCollected++;
                            levelLoader.coins.RemoveAt(i--);
                            continue;
                        }
                    }

                    foreach (PiranhaPlant piranha in levelLoader.piranhaPlants)
                    {
                        piranha.Update(deltaTime);

                        // Mario collision with piranhas
                        if (mario.hitbox.IsTouching(piranha.hitbox))
                            mario.DamageMario();
                    }

                    lastTimePickedACoin += deltaTime;

                    // Clamp camera position to not go offscreen
                    cameraPosition.X = System.Math.Clamp(-mario.movement.position.X + Main.designedResolutionWidth / 2, -levelLoader.levelMaxWidth + Main.designedResolutionWidth, 0);
                    break;
                default: // GameOver/Won
                    coinUI.Update(deltaTime);

                    textFadeValue += deltaTime * textFadeDirection;

                    if (textFadeValue < 0 || textFadeValue > 1)
                        textFadeDirection *= -1;

                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        gameState = GameState.MainMenu;
                        Reset();
                    }
                    break;
            }

            lastKeyboardState = Keyboard.GetState();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                    Point windowSize = windowbox.GetScaledRect().Size;

                    // Game name
                    spriteBatch.DrawString(font, gameName, new Vector2(windowSize.X / 2, windowSize.Y * 0.25f), Color.White, 0, font.MeasureString(gameName) / 2, 0.75f, SpriteEffects.None, 0);

                    // Start Game
                    spriteBatch.DrawString(font, startText, new Vector2(windowSize.X / 2, windowSize.Y * 0.5f), selected == 0 ? selectColor : Color.White, 0, font.MeasureString(startText) / 2, 0.5f, SpriteEffects.None, 0);
                    
                    // Select level
                    spriteBatch.DrawString(font, levelSelectText, new Vector2(windowSize.X / 2, windowSize.Y * 0.6f), selected == 1 ? selectColor : Color.White, 0, font.MeasureString(levelSelectText) / 2, 0.5f, SpriteEffects.None, 0);

                    break;
                case GameState.LevelSelect:
                    windowSize = windowbox.GetScaledRect().Size;

                    for (int i = 0; i < numberOfLevels; i++)
                        spriteBatch.DrawString(font, "Level " + (i + 1).ToString(), new Vector2(windowSize.X / 2, windowSize.Y * (0.2f + 0.1f * i)), selected == i ? selectColor : Color.White, 0, font.MeasureString("Level " + (i + 1).ToString()) / 2, 0.5f, SpriteEffects.None, 0);

                    break;
                case GameState.GameOver:
                    windowSize = windowbox.GetScaledRect().Size;

                    spriteBatch.DrawString(font, gameOverText, new Vector2(windowSize.X / 2, windowSize.Y * 0.3f), Color.White, 0, font.MeasureString(gameOverText) / 2, 0.5f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, backToMainMenuText, new Vector2(windowSize.X / 2, windowSize.Y * 0.7f), Color.White * textFadeValue, 0, font.MeasureString(backToMainMenuText) / 2, 0.5f, SpriteEffects.None, 0);
                    break;
                case GameState.Won:
                    windowSize = windowbox.GetScaledRect().Size;

                    spriteBatch.DrawString(font, wonText, new Vector2(windowSize.X / 2, windowSize.Y * 0.3f), Color.White, 0, font.MeasureString(wonText) / 2, 0.5f, SpriteEffects.None, 0);

                    // Coin drawing
                    coinUI.Draw(spriteBatch, allCoinsCollected, new Vector2(windowSize.X / 2, windowSize.Y * 0.5f));

                    spriteBatch.DrawString(font, backToMainMenuText, new Vector2(windowSize.X / 2, windowSize.Y * 0.7f), Color.White * textFadeValue, 0, font.MeasureString(backToMainMenuText) / 2, 0.5f, SpriteEffects.None, 0);
                    break;
                case GameState.Playing:
                    // Draw level tiles
                    foreach (Tile tile in levelLoader.tiles)
                        tile.Draw(spriteBatch);

                    // Draw the castle
                    levelLoader.castle.Draw(spriteBatch);

                    // Draw the coins spreadout through the level
                    foreach (Coin coin in levelLoader.coins)
                        coin.Draw(spriteBatch);

                    // Draw the Gombas
                    foreach (Gomba gomba in levelLoader.gombas)
                        gomba.Draw(spriteBatch);

                    // Draw Piranha plants
                    foreach (PiranhaPlant piranha in levelLoader.piranhaPlants)
                        piranha.Draw(spriteBatch);

                    // Draw Pipes
                    foreach (Pipe pipe in levelLoader.pipes)
                        pipe.Draw(spriteBatch);

                    // Finally draw Mario, he should be on top of everything
                    levelLoader.mario.Draw(spriteBatch);

                    // Coin UI
                    coinUI.Draw(spriteBatch, coinsCollected);
                    break;
            }
        }
    }
}
