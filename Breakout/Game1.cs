using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Breakout
{
    enum GameState
    {
        StartMenu,
        Playing,
        GameOver
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Gamestates and different screens
        GameState currentState;
        Texture2D gameOverBackground;
        Texture2D startMenuBackground;

        //Start button in startmenu
        Texture2D startButtonTexture;
        Vector2 startButtonPos;
        Rectangle startButtonRect;
        Color startButtonColor;

        //player and player stats
        Player player;
        int points;
        int ballsAlive = 3;

        //variables for the ball
        Ball ball;
        Vector2 ballStartingPosition;
        Vector2 ballStartingDirection;
        Random randomDirection = new Random();

        //variables for the brick
        Brick brick;
        Vector2 brickStartingPosition;
        int numOfRows = 8;
        int numOfCols = 14;
        Brick[,]brickArray = new Brick[8,14];
        Texture2D brickBorder;

        //font settings
        SpriteFont spriteFont;
        Vector2 pointDisplayPos = new Vector2(0, 410);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //initializing the point counter
            points = 0;

            //initializing the bricks
            Texture2D brickTexture = Content.Load<Texture2D>(@"brick");
            brickStartingPosition = Vector2.Zero;

            FillBlocks(brickTexture);

            //changing window size to fit the bricks
            _graphics.PreferredBackBufferWidth = brickTexture.Width * 14;
            _graphics.ApplyChanges();

            int centerPosition = Window.ClientBounds.Width / 2;

            //setting up start button
            startButtonColor = Color.Gray;
            startButtonTexture = Content.Load<Texture2D>(@"startbutton");
            startButtonPos = new Vector2(centerPosition - startButtonTexture.Width / 2, Window.ClientBounds.Height / 2 - startButtonTexture.Height / 2);
            startButtonRect = new Rectangle((int)startButtonPos.X, (int)startButtonPos.Y, startButtonTexture.Width, startButtonTexture.Height);

            //setting the starting Gamestate
            currentState = GameState.StartMenu;

            //initializing the player block
            Texture2D playerTexture = Content.Load<Texture2D>(@"player");
            int centerPlayerPosition = centerPosition - playerTexture.Width / 2;
            player = new Player(playerTexture, new Vector2(centerPlayerPosition, 400));

            //initializing the ball
            Texture2D ballTexture = Content.Load<Texture2D>(@"ball");
            ballStartingPosition = new Vector2(centerPlayerPosition, 350);
            ball = new Ball(ballTexture, ballStartingPosition);

            ballStartingDirection = new Vector2(1, -1);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            brickBorder = Content.Load<Texture2D>(@"brickborder");
            spriteFont = Content.Load<SpriteFont>("spritefont1");
            gameOverBackground = Content.Load<Texture2D>(@"gameoverscreen");
            startMenuBackground = Content.Load<Texture2D>(@"startmenubackground");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //console title
            Window.Title = $"Breakout {points} points {ballsAlive} lives left";

            //checks if the player has lost
            if (ballsAlive == 0)
            {
                currentState = GameState.GameOver;
            }
            
            switch(currentState)
            {
                case GameState.StartMenu:
                    StartMenuUpdate();
                    break;

                case GameState.Playing:
                    PlayingUpdate();
                    break;

                case GameState.GameOver:
                    GameOverUpdate();
                    break;
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.FrontToBack);

            switch(currentState)
            {
                case GameState.StartMenu:
                    StartMenuDraw();
                    break;

                case GameState.Playing:
                    PlayingDraw();
                    break;

                case GameState.GameOver:
                    GameOverDraw();
                    break;
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void StartMenuUpdate()
        {
            MouseState mouse = Mouse.GetState();

            if (startButtonRect.Contains(mouse.X, mouse.Y))
            {
                startButtonColor = Color.White;

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    currentState = GameState.Playing;
                }
            }
            else
            {
                startButtonColor = Color.Gray;
            }
        }

        protected void PlayingUpdate()
        {
            var keys = Keyboard.GetState();
            
            //collision between player and ball. The bounce is randomized
            if (player.rect.Intersects(ball.rect))
            {
                double randomBounce = randomDirection.Next(1, 3);
                ball.movementX = (float)randomBounce * -1;
                ball.movementY *= -1;
            }

            //player movement
            player.Move(keys, Window.ClientBounds.Width);
            player.UpdateRectanglePosition();

            //ball movement
            ball.Move(ballStartingDirection);
            ball.UpdateRectanglePosition();

            //collision between ball and screen
            if (ball.position.X < 0 || ball.position.X + ball.texture.Width > Window.ClientBounds.Width)
            {
                ball.movementX *= -1;
            }
            else if (ball.position.Y < 0)
            {
                ball.movementY *= -1;
            }
            else if (ball.position.Y + ball.texture.Height > Window.ClientBounds.Height)
            {
                ball.position = ballStartingPosition;
                ball.movementX = ball.speed;
                ball.movementY = ball.speed;
                ballsAlive--;
            }

            //collision between ball and bricks
            CheckCollision();
        }

        protected void GameOverUpdate()
        {

        }

        protected void StartMenuDraw()
        {
            _spriteBatch.Draw(startButtonTexture, startButtonPos, null, startButtonColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            _spriteBatch.Draw(startMenuBackground, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        protected void PlayingDraw()
        {
            player.Draw(_spriteBatch);
            ball.Draw(_spriteBatch);
            DrawBlocks();
        }

        protected void GameOverDraw()
        {
            _spriteBatch.Draw(gameOverBackground, Vector2.Zero, Color.White);
        }

        protected void DrawBlocks()
        {
            Brick currentBrick;
            
            for (int row = 0; row < numOfRows; row++)
            {
                for (int col = 0; col < numOfCols; col++)
                {
                    currentBrick = brickArray[row, col];
                    if (currentBrick.isAlive)
                    {
                        currentBrick = brickArray[row, col];
                        currentBrick.Draw(_spriteBatch);
                        _spriteBatch.Draw(brickBorder, currentBrick.position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
                    }
                }
            }
        }

        protected void FillBlocks(Texture2D brickTexture)
        {
            Vector2 changedPosition = brickStartingPosition;
            int currentBrickPointValue = 16; //different points are given depending on which row the blocks are
            Color currentBlockColor;
            for (int row = 0; row < numOfRows; row++)
            {
                for (int col = 0; col < numOfCols; col++)
                {
                    currentBlockColor = ColorBlocks(row);
                    brick = new(brickTexture, changedPosition, currentBrickPointValue, currentBlockColor);
                    brickArray[row, col] = brick;
                    changedPosition.X += brickTexture.Width;
                }
                //changing position for the next row of blocks and giving them a lesser point value
                changedPosition.X = 0;
                changedPosition.Y += brickTexture.Height;
                currentBrickPointValue -= 2;
            }

        }

        protected Color ColorBlocks(int rowNumber)
        {
            Color blockColor;

            if (rowNumber < 2)
            {
                blockColor = Color.Red;
            }
            else if (rowNumber < 4)
            {
                blockColor = Color.Magenta;
            }
            else if (rowNumber < 6)
            {
                blockColor = Color.Aqua;
            }
            else
            {
                blockColor = Color.Green;
            }

            return blockColor;
        }

        protected void CheckCollision()
        {
            int offsetX = 1000;
            int offsetY = 1000; 
            Brick currentBrick;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 14; col++)
                {
                    currentBrick = brickArray[row, col];
                    if (currentBrick.rect.Intersects(ball.rect))
                    {
                        currentBrick.isAlive = false;
                        currentBrick.rect.Offset(offsetX, offsetY); //moves the hitbox out of bounds to "unload" the dead brick
                        ball.movementY *= -1;
                        points += currentBrick.pointValue;
                    }
                }
            }
        }
    }
}