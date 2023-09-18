using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Breakout
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

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
        Brick[,]brickArray = new Brick[8,14];
        Texture2D blueBrickTexture;
        Texture2D greenBrickTexture;
        Texture2D purpleBrickTexture;
        //List<Brick> bricks = new List<Brick>();

        //font settings
        SpriteFont spriteFont;
        Vector2 pointDisplayPos = new Vector2(0, 410);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            //initializing the point counter
            points = 0;

            int centerPosition = Window.ClientBounds.Width / 2;

            //initializing the bricks
            Texture2D brickTexture = Content.Load<Texture2D>(@"brick");
            brickStartingPosition = Vector2.Zero;

            //changing window size to fit the bricks
            _graphics.PreferredBackBufferWidth = brickTexture.Width * 14;
            _graphics.ApplyChanges();

            FillBlocks(brickTexture);
            /* //fills top part of the screen with bricks
            for (int i = 0; i < 14; i++)
            {
                brick = new Brick(brickTexture, brickStartingPosition);
                bricks.Add(brick);
                brickStartingPosition.X += brickTexture.Width;
            }*/


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
            //container for all the non default brick textures
            blueBrickTexture = Content.Load<Texture2D>(@"bluebrick");
            greenBrickTexture = Content.Load<Texture2D>(@"greenbrick");
            purpleBrickTexture = Content.Load<Texture2D>(@"purplebrick");
            spriteFont = Content.Load<SpriteFont>("spritefont1");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var keys = Keyboard.GetState();

            //console title
            Window.Title = $"Breakout {points} points {ballsAlive} lives left";

            if (ballsAlive == 0)
            {
                Exit();
            }

            //player movement
            player.Move(keys, Window.ClientBounds.Width);
            player.UpdateRectanglePosition();

            //ball movement
            ball.Move(ballStartingDirection);
            ball.UpdateRectanglePosition();

            //collision between player and ball
            if (player.rect.Intersects(ball.rect))
            {
                double randomBounce = randomDirection.Next(1, 6);
                ball.movementX = (float)randomBounce * -1;
                ball.movementY *= -1;
            }

            //collision between ball and screen
            if (ball.position.X < 0 || ball.position.X + ball.texture.Width > Window.ClientBounds.Width)
            {
                ball.movementX *= -1;
            }
            if (ball.position.Y < 0)
            {
                ball.movementY *= -1;
            }
            if (ball.position.Y + ball.texture.Height >  Window.ClientBounds.Height)
            {
                ball.position = ballStartingPosition;
                ball.movementX = ball.speed;
                ball.movementY = ball.speed;
                ballsAlive--;
            }

            //collision between ball and bricks
            CheckCollision();
            
            
            /*for (int index = 0; index < bricks.Count; index++)
            {
                if (bricks[index].rect.Intersects(ball.rect))
                {
                    bricks[index].isAlive = false;
                    bricks.RemoveAt(index);
                    ball.movementY *= -1;
                    points++;
                }
            }*/

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Brick currentBrick;
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(spriteFont, points.ToString(), pointDisplayPos, Color.Aqua, 0, Vector2.Zero, 2, SpriteEffects.None, 1);
            player.Draw(_spriteBatch);
            ball.Draw(_spriteBatch);

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 14; col++)
                {
                    currentBrick = brickArray[row, col];
                    if (currentBrick.isAlive)
                    {
                        currentBrick = brickArray[row, col];
                        currentBrick.Draw(_spriteBatch);
                    }
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void FillBlocks(Texture2D brickTexture)
        {
            Vector2 changedPosition = brickStartingPosition;
            int currentBrickPointValue = 16; //different points are given depending on which row the blocks are
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 14; col++)
                {
                    brick = new(brickTexture, changedPosition, currentBrickPointValue);
                    brickArray[row, col] = brick;
                    changedPosition.X += brickTexture.Width;
                }
                //changing position for the next row of blocks and giving them a lesser point value
                changedPosition.X = 0;
                changedPosition.Y += brickTexture.Height;
                currentBrickPointValue -= 2;
            }

        }

        protected void CheckCollision()
        {
            Brick currentBrick;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 14; col++)
                {
                    currentBrick = brickArray[row, col];
                    if (currentBrick.rect.Intersects(ball.rect))
                    {
                        currentBrick.isAlive = false;
                        currentBrick.rect.Offset(1000, 1000); //moves the hitbox out of bounds to "unload" the dead brick
                        ball.movementY *= -1;
                        points += currentBrick.pointValue;
                    }
                }
            }
        }
    }
}