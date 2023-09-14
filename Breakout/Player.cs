using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Breakout
{
    internal class Player : BaseGameObject
    {
        private float playerSpeed = 8f;
        
        
        public Player(Texture2D texture, Microsoft.Xna.Framework.Vector2 position) : base(texture, position)
        {
            this.texture = texture;
            this.position = position;
        }

        public void Draw(SpriteBatch sprite)
        {
            sprite.Draw(texture, position, Microsoft.Xna.Framework.Color.White);
        }

        public void Move(KeyboardState keys, int width)
        {
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
