using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout
{
    internal class Brick
    {
        
        Texture2D texture;
        Vector2 position;


        public Brick(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
        }

        public void UpdateBricks()
        {

        }

        public void Draw(SpriteBatch sprite) 
        {
                sprite.Draw(texture, position, Color.White);
        }
    }
}
