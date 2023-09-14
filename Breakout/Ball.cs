using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout
{
    internal class Ball : BaseGameObject
    {

        public float speed = 4f;
        public float movementX;
        public float movementY;

        public Ball(Texture2D texture, Vector2 position) : base(texture, position)
        {
            this.texture = texture;
            this.position = position;

            movementX = speed;
            movementY = speed;
        }

        public void Draw(SpriteBatch sprite)
        {
            sprite.Draw(texture, position, Color.White);
        }

        public void Move(Vector2 direction)
        {
            position.X += direction.X * movementX;
            position.Y += direction.Y * movementY;
        }
    }
}
