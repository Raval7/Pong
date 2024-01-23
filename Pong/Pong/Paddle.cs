using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Pong
{
    class Paddle : Microsoft.Xna.Framework.Game
    {
        public Texture2D texture;
        public Vector2 position, defaultposition;
        public int width, height, speed, ScrHeight;

        public Paddle()
        {
            texture = null;
            width = 10;            
            speed = 20;
        }
    
        public void Update()
        {
            if (position.Y <= 0 + 5)
                position.Y = 5;
            if (position.Y >= ScrHeight - height - 5)
                position.Y = ScrHeight - height - 5;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle((int)position.X, 
                                                    (int)position.Y, 
                                                    width, 
                                                    height), Color.White);
        }
    }
}
