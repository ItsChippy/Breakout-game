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

        //variables for the ball
        Ball ball;
        Vector2 ballStartingPosition;
        Vector2 ballStartingDirection;
        Random randomDirection = new Random();

        //variables for the brick
        Brick brick;
        Vector2 brickStartingPosition;
        List<Brick> bricks = new List<Brick>();


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

            //fills top part of the screen with bricks
            for (int i = 0; i < 14; i++)
            {
                brick = new Brick(brickTexture, brickStartingPosition);
                bricks.Add(brick);
                brickStartingPosition.X += brickTexture.Width;
            }


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
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var keys = Keyboard.GetState();

            //console title
            Window.Title = $"Breakout {points} points";

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
                Exit();
            }

            //collision between ball and bricks
            for (int index = 0; index < bricks.Count; index++)
            {
                if (bricks[index].rect.Intersects(ball.rect))
                {
                    bricks[index].isAlive = false;
                    bricks.RemoveAt(index);
                    ball.movementY *= -1;
                    points++;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            player.Draw(_spriteBatch);
            ball.Draw(_spriteBatch);

            foreach (var brick in bricks)
            {
                if (brick.isAlive)
                {
                    brick.Draw(_spriteBatch);
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}