using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout
{
    internal class Ball
    {
        public Texture2D texture;
        public Vector2 position;

        public float speed = 5f;
        public float speedX;
        public float speedY;

        public Ball(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;

            speedX = speed;
            speedY = speed;
        }

        public void Draw(SpriteBatch sprite)
        {
            sprite.Draw(texture, position, Color.White);
        }

        public void Move(Vector2 direction)
        {
            position.X += direction.X * speedX;
            position.Y += direction.Y * speedY;
        }
    }
}
