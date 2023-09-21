using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

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
        Texture2D backgroundTexture;
        Vector2 backGroundPos;

        //text in startmenu
        string startMenuText = "Breakout by Logan";
        Vector2 startMenuTextPos;

        //Start button in startmenu
        Texture2D startButtonTexture;
        Vector2 startButtonPos;
        Rectangle startButtonRect;
        Color startButtonColor;

        //shark animation in startmenu
        Texture2D sharkSpriteSheet;
        Vector2 sharkPos;
        int sharkSpriteWidth = 402;
        int sharkSpriteHeight = 172;
        int sharkFrame;
        double frameTimer = 100;
        double frameInterval = 100;
        Rectangle sharkSpriteRect;

        //end game screen
        int numOfFishes = 5;
        Texture2D fishTexture;
        Fish[] arrayOfFishes;
        Random randFishPlacement = new Random();

        //end game screen text
        Vector2 gameOverTextPos;
        string gameOverText;

        //points and lives display
        Vector2 pointDisplayPos = new(0, 410);
        StringBuilder sbPointDisplay = new StringBuilder();

        //player and player stats
        Player player;
        int points;
        int ballsAlive = 3;
        int timeInGame;

        //variables for the ball
        Ball ball;
        Vector2 ballStartingPosition;
        Vector2 ballStartingDirection;

        //variables for the brick and brick array
        Brick brick;
        Vector2 brickStartingPosition;
        int numOfRows = 8;
        int numOfCols = 14;
        Brick[,]brickArray;
        Texture2D brickBorder;

        //font settings
        SpriteFont spriteFont;


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
            brickArray = new Brick[numOfRows, numOfCols];
            FillBlocks(brickTexture);

            //changing window size to fit the bricks
            _graphics.PreferredBackBufferWidth = brickTexture.Width * 14;
            _graphics.ApplyChanges();

            int centerPositionX = Window.ClientBounds.Width / 2;
            int centerPositionY = Window.ClientBounds.Height / 2;


            //setting up start menu background
            startMenuTextPos = new(centerPositionX, 20);
            backgroundTexture = Content.Load<Texture2D>(@"background");
            backGroundPos = CalculateCenterPositioning(centerPositionX, centerPositionY, backgroundTexture);

            //setting up start button
            startButtonColor = Color.Gray;
            startButtonTexture = Content.Load<Texture2D>(@"startbutton");
            startButtonPos = CalculateCenterPositioning(centerPositionX, centerPositionY, startButtonTexture);
            startButtonRect = new Rectangle((int)startButtonPos.X, (int)startButtonPos.Y, startButtonTexture.Width, startButtonTexture.Height);

            //setting the starting Gamestate
            currentState = GameState.StartMenu;

            //end game screen text position
            gameOverTextPos = new Vector2(centerPositionX, centerPositionY);

            //initializing the player block
            Texture2D playerTexture = Content.Load<Texture2D>(@"player");
            int centerPlayerPosition = centerPositionX - playerTexture.Width / 2;
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

            //end screen fishes
            fishTexture = Content.Load<Texture2D>(@"goldfish");
            arrayOfFishes = new Fish[numOfFishes];
            FillFishArray();

            //shark animation
            sharkSpriteSheet = Content.Load<Texture2D>(@"sharks");
            sharkSpriteRect = new Rectangle(0, 0, sharkSpriteWidth, sharkSpriteHeight);
            sharkPos = new Vector2(100, 350);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //checks if the player has lost
            if (ballsAlive == 0 || timeInGame == 25)
            {
                currentState = GameState.GameOver;
                gameOverText = "You lost!" +
                                $"\nTotal Points: {points}" +
                                $"\nTotal Game Time: {timeInGame}";
            }
            
            
            switch(currentState)
            {
                case GameState.StartMenu:
                    StartMenuUpdate(gameTime);
                    break;

                case GameState.Playing:
                    PlayingUpdate(gameTime);
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

        protected void StartMenuUpdate(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            
            //shark animation in start menu
            frameTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (frameTimer <= 0)
            {
                frameTimer = frameInterval;
                sharkFrame++;

                if(sharkFrame >= 6)
                {
                    sharkFrame = 0;
                    sharkSpriteRect.Y += sharkSpriteRect.Y;
                }
                if (sharkFrame > 12)
                {
                    sharkFrame = 0;
                    sharkSpriteRect.Y -= sharkSpriteRect.Y;
                }
                sharkSpriteRect.X = sharkFrame * sharkSpriteWidth;
            }

            //logic for start button
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

        protected void PlayingUpdate(GameTime gameTime)
        {
            timeInGame = gameTime.TotalGameTime.Seconds;
            var keys = Keyboard.GetState();
            IsMouseVisible = false;

            //collision between player and ball.
            if (player.rect.Intersects(ball.rect))
            {
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

            sbPointDisplay.Clear();
            sbPointDisplay.Append($"Points: {points}" +
                                 $"\nLives left: {ballsAlive}" +
                                 $"\nTime: {timeInGame} seconds");
        }

        protected void GameOverUpdate()
        {
            IsMouseVisible = true;
        }

        protected void StartMenuDraw()
        {
            Vector2 textMiddlePoint = CalculateTextMiddlePoint(startMenuText);  
            
            _spriteBatch.DrawString(spriteFont, startMenuText, startMenuTextPos, Color.Black, 0, textMiddlePoint, 1.5f, SpriteEffects.None, 0.1f);
            _spriteBatch.Draw(sharkSpriteSheet, sharkPos, sharkSpriteRect, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0.1f);
            _spriteBatch.Draw(startButtonTexture, startButtonPos, null, startButtonColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            _spriteBatch.Draw(backgroundTexture, backGroundPos, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        protected void PlayingDraw()
        {
            _spriteBatch.DrawString(spriteFont, sbPointDisplay, pointDisplayPos, Color.White);
            player.Draw(_spriteBatch);
            ball.Draw(_spriteBatch);
            DrawBlocks();
        }

        protected void GameOverDraw()
        {
            Vector2 textMiddlePoint = CalculateTextMiddlePoint(gameOverText);

            _spriteBatch.DrawString(spriteFont, gameOverText, gameOverTextPos, Color.Black, 0, textMiddlePoint, 1.5f, SpriteEffects.None, 0.1f);
            _spriteBatch.Draw(backgroundTexture, backGroundPos, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            foreach (Fish fish in arrayOfFishes)
            {
                fish.Draw(_spriteBatch);
            }
        }

        protected void FillFishArray()
        {
            int screenEdgeX = Window.ClientBounds.Width - fishTexture.Width;
            int screenEdgeY = Window.ClientBounds.Height - fishTexture.Height;
            Vector2 currentFishPos;
            Fish currentFish;

            for (int i = 0; i < arrayOfFishes.Length; i++)
            {
                currentFishPos.X = randFishPlacement.Next(0, screenEdgeX);
                currentFishPos.Y = randFishPlacement.Next(0, screenEdgeY);
                currentFish = new Fish(fishTexture, currentFishPos);
                arrayOfFishes[i] = currentFish;
            }
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
            int rowPointDifference = 2; //every row has a 2 point difference, the top row having the highest amount
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
                currentBrickPointValue -= rowPointDifference;
            }

        }

        protected Color ColorBlocks(int rowNumber)
        {
            Color blockColor;

            int redBlockRows = 2;
            int magentaBlockRows = 4;
            int aquaBlockRows = 6;
            
            if (rowNumber < redBlockRows)
            {
                blockColor = Color.Red;
            }
            else if (rowNumber < magentaBlockRows)
            {
                blockColor = Color.Magenta;
            }
            else if (rowNumber < aquaBlockRows)
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
            for (int row = 0; row < numOfRows; row++)
            {
                for (int col = 0; col < numOfCols; col++)
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

        protected bool CheckIfWin()
        {
            Brick currentBrick;
            for (int row = 0;row < numOfRows; row++)
            {
                for (int col = 0;col < numOfCols; col++)
                {
                    currentBrick = brickArray[row, col];
                    if (currentBrick.isAlive)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected Vector2 CalculateCenterPositioning(int centerPositionX, int centerPositionY, Texture2D texture)
        {
            return new Vector2(centerPositionX - texture.Width / 2, centerPositionY - texture.Height / 2);
        }

        protected Vector2 CalculateTextMiddlePoint(string inputString)
        {
            Vector2 textMiddlePoint = spriteFont.MeasureString(inputString) / 2;
            return textMiddlePoint;
        }
    }
}