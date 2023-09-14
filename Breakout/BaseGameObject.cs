using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout
{
    internal class BaseGameObject
    {
        public Texture2D texture;
        public Vector2 position;
        public Rectangle rect;

        public BaseGameObject(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
            GetRectangle();
        }

        public Rectangle GetRectangle()
        {
            rect = rect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            return rect;
        }

        public void UpdateRectanglePosition()
        {
            rect.X = (int)position.X;
            rect.Y = (int)position.Y;
        }
    }
}
