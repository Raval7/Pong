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
    class Ball : Microsoft.Xna.Framework.Game
    {
        public Vector2 position, defaultposition;
        public Texture2D texture;
        public int width, height, ScrHeight, ScrWidth;
        public int speedX, speedY;
        public int player1Score, player2Score;
        public enum Moving
        {
            UL,
            UR,
            DL,
            DR
        }
        public Moving move;

        public Ball()
        {
            width = 20;
            height = 20;
            speedX = 8;
            speedY = 8;
            move = Moving.UL;
            position = Vector2.Zero;
            defaultposition = Vector2.Zero;
            texture = null;
            player1Score = 0;
            player2Score = 0;
        }
    
        public void Update()
        {
            // ++++++++++++++++++++++++++++
            // Ruch pi³ki w okreœlonym kierunku
            if (move == Moving.UL)
            {
                position.X -= speedX;
                position.Y -= speedY;
            }
            if (move == Moving.UR)
            {
                position.X += speedX;
                position.Y -= speedY;
            }
            if (move == Moving.DL)
            {
                position.X -= speedX;
                position.Y += speedY;
            }
            if (move == Moving.DR)
            {
                position.X += speedX;
                position.Y += speedY;
            }

            // ++++++++++++++++++++++++++++
            // Kolizja pi³ki - œciana górna i dolna
            if (move == Moving.UL && position.Y <= 5)            
                move = Moving.DL;
            
            if (move == Moving.DL && position.Y >= ScrHeight - height - 5)            
                move = Moving.UL;
            
            if (move == Moving.UR && position.Y <= 5)            
                move = Moving.DR;
            
            if (move == Moving.DR && position.Y >= ScrHeight - height - 5)            
                move = Moving.UR;

            // ++++++++++++++++++++++++++++
            // Kolizje pi³ka - œciany za paletkami
            if (move == Moving.UL && position.X <= 0)
            {
                move = Moving.DR;
                position = defaultposition;
                player2Score += 1;
            }
            if (move == Moving.DL && position.X <= 0)
            {
                move = Moving.UR;
                position = defaultposition;
                player2Score += 1;
            }
            if (move == Moving.UR && position.X >= ScrWidth - width)
            {
                move = Moving.UL;
                position = defaultposition;
                player1Score += 1;
            }
            if (move == Moving.DR && position.X >= ScrWidth - width)
            {
                move = Moving.DL;
                position = defaultposition;
                player1Score += 1;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle((int)position.X, 
                                                    (int)position.Y, 
                                                    width, height), Color.White);
        }
    }
}
