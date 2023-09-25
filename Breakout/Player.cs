using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout
{
    internal class Player : BaseGameObject
    {
        private float playerSpeed = 5f;
        MouseState mouse;
        Vector2 direction;
        
        public Player(Texture2D texture, Vector2 position) : base(texture, position)
        {
            this.texture = texture;
            this.position = position;
            direction = new Vector2(mouse.X, position.Y);
        }

        public void Draw(SpriteBatch sprite)
        {
            sprite.Draw(texture, position, Color.White);
        }

        public void Move(KeyboardState keys, int width)
        {
            //mouse controls
            mouse = Mouse.GetState();
            position = direction;
            direction.X = mouse.X;

            if (position.X < 0)
            {
                position.X = 0;
            }
            if (position.X + texture.Width >= width)
            {
                position.X = width - texture.Width;
            }

            //keyboard controls
            if (keys.IsKeyDown(Keys.D) && position.X + texture.Width <= width)
            {
                position.X += playerSpeed;
            }
            if (keys.IsKeyDown(Keys.A) && position.X >= 0)
            {
                position.X -= playerSpeed;
            }
        }

    }
}
