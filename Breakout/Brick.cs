using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout
{
    internal class Brick : BaseGameObject
    {
        public bool isAlive = true;
        public int pointValue;
        public Color color;
        public Brick(Texture2D texture, Vector2 position, int pointValue, Color color) : base(texture, position)
        {
            this.texture = texture;
            this.position = position;
            this.pointValue = pointValue;
            this.color = color;
        }

        public void Draw(SpriteBatch sprite) 
        {
                sprite.Draw(texture, position, color);
        }
    }
}
