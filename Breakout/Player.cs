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
        private float playerSpeed = 8f;
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
            mouse = Mouse.GetState();
            if (position.X + texture.Width <= width || mouse.X >=0)
            {
                direction = new Vector2(mouse.X, position.Y);
                position = direction;
            }
            if (keys.IsKeyDown(Keys.Right) && position.X + texture.Width <= width)
            {
                position.X += playerSpeed;
            }
            if (keys.IsKeyDown(Keys.Left) && position.X >= 0)
            {
               position.X -= playerSpeed;
            }
        }

    }
}
