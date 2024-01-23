using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // ++++++++++++++++++++++++++++
        // Deklaracja zmiennych globalnych

        public int ScrWidth, ScrHeight;
        public Texture2D player1Button, //textury
                         player2Button,
                         sidewall;
        public Vector2 player1ButtonPos, //pozycje
                       player2ButtonPos,
                       menuTextPos;                      
        public enum State //enumerator stanu gry
        {
            Menu,
            Game1p,
            Game2p,
            gameover
        }
        public State state; // stan gry
        SpriteFont MenuText;

        Paddle paddle1 = new Paddle(); 
        Paddle paddle2 = new Paddle();
        Ball ball = new Ball();

        bool cpuUP = true;

        public Vector2 player1ScorePos, player2ScorePos;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            // ++++++++++++++++++++++++++++
            // Ustawienia wyœwietlania na pe³nym ekranie
            graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
                                   
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // ++++++++++++++++++++++++++++
            // Przekazanie rozmiaru ekranu do zmiennych
            ScrWidth = GraphicsDevice.Viewport.Width;
            ScrHeight = GraphicsDevice.Viewport.Height;

            // ++++++++++++++++++++++++++++
            // Ustawienie stanu gry
            state = State.Menu;

            // ++++++++++++++++++++++++++++
            // Wczytanie tekstur i czcionek
            player1Button = this.Content.Load<Texture2D>("Button1tx");
            player2Button = this.Content.Load<Texture2D>("Button2tx");
            MenuText = this.Content.Load<SpriteFont>("MenuFont");

            // ++++++++++++++++++++++++++++
            // Ustawienie pozycji poszczególnych elementów
            player1ButtonPos = new Vector2(ScrWidth / 2 - 150, ScrHeight / 3 + 50);
            player2ButtonPos = new Vector2(ScrWidth / 2 - 150, ScrHeight / 3 + 175);
            menuTextPos = new Vector2(ScrWidth / 2 - 170, -30);

            // ++++++++++++++++++++++++++++
            // Wczytanie tekstur paletek i cian bocznych
            paddle1.texture = this.Content.Load<Texture2D>("tx1");
            paddle2.texture = this.Content.Load<Texture2D>("tx1");
            sidewall = this.Content.Load<Texture2D>("tx1");

            // ++++++++++++++++++++++++++++
            // Ustawienie w³aœciwoœci paletek i wysokoœci ekranu
            paddle1.height = ScrHeight / 5;
            paddle2.height = ScrHeight / 5;
            paddle1.defaultposition = new Vector2(30, ScrHeight / 2 - paddle1.height / 2);
            paddle2.defaultposition = new Vector2(ScrWidth - 30 - paddle2.width, 
                                           ScrHeight / 2 - paddle1.height / 2);
            paddle1.position = paddle1.defaultposition;
            paddle2.position = paddle2.defaultposition;
            paddle1.ScrHeight = ScrHeight;
            paddle2.ScrHeight = ScrHeight;

            // ++++++++++++++++++++++++++++
            // Ustawienie tekstur i w³aœciwoœci pi³ki oraz wysokoœci i szerokoœci ekranu
            ball.texture = this.Content.Load<Texture2D>("tx2");
            ball.defaultposition.X = ScrWidth / 2 - ball.width / 2;
            ball.defaultposition.Y = ScrHeight / 2 - ball.height / 2;
            ball.position = ball.defaultposition;
            ball.ScrHeight = ScrHeight;
            ball.ScrWidth = ScrWidth;

            // ++++++++++++++++++++++++++++
            // Ustawienie pozycji punktów graczy
            player1ScorePos = new Vector2(ScrWidth / 2 - 300, -50);
            player2ScorePos = new Vector2(ScrWidth / 2 + 200, -50);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit           
            // TODO: Add your update logic here

            // ++++++++++++++++++++++++++++
            // Sterowanie
            Control();

            // ++++++++++++++++++++++++++++
            // Update - menu
            if (state == State.Menu)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();
            }
            
            // ++++++++++++++++++++++++++++
            // Update - jeden gracz
            if (state == State.Game1p)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    state = State.Menu;

                // ++++++++++++++++++++++++++++
                // Utrzymywanie paletek w polu gry
                paddle1.Update();
                paddle2.Update();

                // ++++++++++++++++++++++++++++
                // Sterowanie oraz utrzymywanie pi³ki w polu gry
                ball.Update();

                // ++++++++++++++++++++++++++++
                // Kolizja - pi³ka paletka
                if (paddle1colision())
                {
                    // ++++++++++++++++++++++++++++
                    // Odbicie pi³ki ze sta³ym kierunkiem zale¿nym od miejsca odbicia
                    if (paddle1.position.Y <= ball.position.Y + ball.height / 2 && 
                        paddle1.position.Y + paddle1.height / 2 >= ball.position.Y + ball.height / 2)
                    {
                        ball.move = Ball.Moving.UR;
                    }
                    else
                    {
                        ball.move = Ball.Moving.DR;
                    }
                    changeangle();
                }

                if (paddle2colision())
                {
                    if (paddle2.position.Y <= ball.position.Y + ball.height / 2 && 
                        paddle2.position.Y + paddle2.height / 2 >= ball.position.Y + ball.height / 2)
                    {
                        ball.move = Ball.Moving.UL;
                    }
                    else
                    {
                        ball.move = Ball.Moving.DL;
                    }
                    changeangle();
                }
                // ++++++++++++++++++++++++++++
                // Sterowanie - komputer
                cpu();

                // ++++++++++++++++++++++++++++
                // Sprawdzenie czy gracz zdoby³ 10 punktów
                if (ball.player1Score == 10 || ball.player2Score == 10)
                {
                    state = State.gameover;
                }
            }

            // ++++++++++++++++++++++++++++
            // Update - dwóch graczy
            if (state == State.Game2p)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    state = State.Menu;

                // ++++++++++++++++++++++++++++
                // Utrzymywanie paletek w polu gry
                paddle1.Update();
                paddle2.Update();

                // ++++++++++++++++++++++++++++
                // Sterowanie oraz utrzymywanie pi³ki w polu gry
                ball.Update();

                // ++++++++++++++++++++++++++++
                // Kolizja - pi³ka paletka
                if (paddle1colision())
                {
                    // ++++++++++++++++++++++++++++
                    // Odbicie pi³ki ze sta³ym kierunkiem zale¿nym od miejsca odbicia
                    if (paddle1.position.Y <= ball.position.Y + ball.height / 2 &&
                        paddle1.position.Y + paddle1.height / 2 >= ball.position.Y + ball.height / 2)
                    {
                        ball.move = Ball.Moving.UR;
                    }
                    else
                    {
                        ball.move = Ball.Moving.DR;
                    }
                    changeangle();
                }

                if (paddle2colision())
                {
                    if (paddle2.position.Y <= ball.position.Y + ball.height / 2 &&
                        paddle2.position.Y + paddle2.height / 2 >= ball.position.Y + ball.height / 2)
                    {
                        ball.move = Ball.Moving.UL;
                    }
                    else
                    {
                        ball.move = Ball.Moving.DL;
                    }
                    changeangle();
                }

                // ++++++++++++++++++++++++++++
                // Sprawdzenie czy gracz zdoby³ 10 punktów
                if (ball.player1Score == 10 || ball.player2Score == 10)
                {
                    state = State.gameover;
                }
            }

            // ++++++++++++++++++++++++++++
            // Obs³uga klawisza "wstecz" dla ekranu "game over"
            if (state == State.gameover)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    state = State.Menu;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            // TODO: Add your drawing code here

            spriteBatch.Begin();
            // ++++++++++++++++++++++++++++
            // Menu
            if (state == State.Menu)
            {
                //spriteBatch.Draw(menuButton, menuButtonPos, Color.White);
                spriteBatch.DrawString(MenuText, "Pong", menuTextPos,
                                       Color.FromNonPremultiplied(34, 177, 76, 250));
                spriteBatch.Draw(player1Button, player1ButtonPos, Color.White);
                spriteBatch.Draw(player2Button, player2ButtonPos, Color.White);
            }

            // ++++++++++++++++++++++++++++
            // Jeden gracz
            if (state == State.Game1p)
            {
                // ++++++++++++++++++++++++++++
                // Rysowanie œcian bocznych
                spriteBatch.Draw(sidewall, new Rectangle(0, 0, ScrWidth, 5), Color.White);
                spriteBatch.Draw(sidewall, new Rectangle(0, ScrHeight - 5, ScrWidth, 5), Color.White);

                // ++++++++++++++++++++++++++++
                // Rysowanie paletek
                paddle1.Draw(spriteBatch);
                paddle2.Draw(spriteBatch);

                // ++++++++++++++++++++++++++++
                // Rysowanie pi³ki
                ball.Draw(spriteBatch);

                // ++++++++++++++++++++++++++++
                // Rysowanie punktów
                spriteBatch.DrawString(MenuText, ball.player1Score.ToString(), 
                                       player1ScorePos, 
                                       Color.FromNonPremultiplied(34, 177, 76, 250));
                spriteBatch.DrawString(MenuText, ball.player2Score.ToString(), 
                                       player2ScorePos, 
                                       Color.FromNonPremultiplied(34, 177, 76, 250));
            }
            // ++++++++++++++++++++++++++++
            // Dwóch graczy
            if (state == State.Game2p)
            {
                // ++++++++++++++++++++++++++++
                // Rysowanie œcian bocznych
                spriteBatch.Draw(sidewall, new Rectangle(0, 0, ScrWidth, 5), Color.White);
                spriteBatch.Draw(sidewall, new Rectangle(0, ScrHeight - 5, ScrWidth, 5), Color.White);

                // ++++++++++++++++++++++++++++
                // Rysowanie paletek
                paddle1.Draw(spriteBatch);
                paddle2.Draw(spriteBatch);

                // ++++++++++++++++++++++++++++
                // Rysowanie pi³ki
                ball.Draw(spriteBatch);

                // ++++++++++++++++++++++++++++
                // Rysowanie punktów
                spriteBatch.DrawString(MenuText, ball.player1Score.ToString(),
                                       player1ScorePos, 
                                       Color.FromNonPremultiplied(34, 177, 76, 250));
                spriteBatch.DrawString(MenuText, ball.player2Score.ToString(),
                                       player2ScorePos, 
                                       Color.FromNonPremultiplied(34, 177, 76, 250));
            }

            // ++++++++++++++++++++++++++++
            // Rysowanie ekranu "game over"
            if (state == State.gameover)
            {
                if (ball.player1Score == 10)
                    spriteBatch.DrawString(MenuText, "game over\n  P1 win", new Vector2(
                                    50, 0), Color.FromNonPremultiplied(34, 177, 76, 250));

                if (ball.player2Score == 10)
                    spriteBatch.DrawString(MenuText, "game over\n  P2 win", new Vector2(
                                    50, 0), Color.FromNonPremultiplied(34, 177, 76, 250));
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void Control()
        {
            TouchCollection touch = TouchPanel.GetState();
            // ++++++++++++++++++++++++++++
            // Sterowanie - menu
            if (state == State.Menu)
            {
                if (touch.Count > 0)
                {
                    if (touch[0].Position.X > player1ButtonPos.X && 
                        touch[0].Position.X < player1ButtonPos.X + 300 && 
                        touch[0].Position.Y > player1ButtonPos.Y && 
                        touch[0].Position.Y < player1ButtonPos.Y + 100)
                    {
                        state = State.Game1p;

                        // ++++++++++++++++++++++++++++
                        // Ustawienie domyœlnych wartoœci
                        paddle1.position = paddle1.defaultposition;
                        paddle2.position = paddle2.defaultposition;
                        ball.position = ball.defaultposition;
                        ball.player1Score = 0;
                        ball.player2Score = 0;
                    }

                    if (touch[0].Position.X > player2ButtonPos.X && 
                        touch[0].Position.X < player2ButtonPos.X + 300 && 
                        touch[0].Position.Y > player2ButtonPos.Y && 
                        touch[0].Position.Y < player2ButtonPos.Y + 100)
                    {
                        state = State.Game2p;

                        // ++++++++++++++++++++++++++++
                        // Ustawienie domyœlnych wartoœci
                        paddle1.position = paddle1.defaultposition;
                        paddle2.position = paddle2.defaultposition;
                        ball.position = ball.defaultposition;
                        ball.player1Score = 0;
                        ball.player2Score = 0;
                    }
                }
            }

            // ++++++++++++++++++++++++++++
            // Sterowanie - jeden gracz
            if (state == State.Game1p)
            {
                if (touch.Count >= 1)
                {
                    if (touch[0].Position.Y > ScrHeight / 2)
                        paddle1.position.Y += paddle1.speed;
                    if (touch[0].Position.Y < ScrHeight / 2)
                        paddle1.position.Y -= paddle1.speed;
                }
            }

            // ++++++++++++++++++++++++++++
            // Sterowanie - dwóch graczy
            if (state == State.Game2p)
            {
                if (touch.Count == 1)
                {
                    if (touch[0].Position.Y > ScrHeight / 2 && touch[0].Position.X < ScrWidth / 2)
                        paddle1.position.Y += paddle1.speed;
                    if (touch[0].Position.Y < ScrHeight / 2 && touch[0].Position.X < ScrWidth / 2)
                        paddle1.position.Y -= paddle1.speed;
                    if (touch[0].Position.Y > ScrHeight / 2 && touch[0].Position.X > ScrWidth / 2)
                        paddle2.position.Y += paddle2.speed;
                    if (touch[0].Position.Y < ScrHeight / 2 && touch[0].Position.X > ScrWidth / 2)
                        paddle2.position.Y -= paddle2.speed;
                }
                if (touch.Count == 2)
                {
                    if (touch[0].Position.Y > ScrHeight / 2 && touch[0].Position.X < ScrWidth / 2)
                        paddle1.position.Y += paddle1.speed;
                    if (touch[0].Position.Y < ScrHeight / 2 && touch[0].Position.X < ScrWidth / 2)
                        paddle1.position.Y -= paddle1.speed;
                    if (touch[0].Position.Y > ScrHeight / 2 && touch[0].Position.X > ScrWidth / 2)
                        paddle2.position.Y += paddle2.speed;
                    if (touch[0].Position.Y < ScrHeight / 2 && touch[0].Position.X > ScrWidth / 2)
                        paddle2.position.Y -= paddle2.speed;

                    if (touch[1].Position.Y > ScrHeight / 2 && touch[1].Position.X < ScrWidth / 2)
                        paddle1.position.Y += paddle1.speed;
                    if (touch[1].Position.Y < ScrHeight / 2 && touch[1].Position.X < ScrWidth / 2)
                        paddle1.position.Y -= paddle1.speed;
                    if (touch[1].Position.Y > ScrHeight / 2 && touch[1].Position.X > ScrWidth / 2)
                        paddle2.position.Y += paddle2.speed;
                    if (touch[1].Position.Y < ScrHeight / 2 && touch[1].Position.X > ScrWidth / 2)
                        paddle2.position.Y -= paddle2.speed;
                }
            }
        }

        public bool paddle1colision()
        {
            if (ball.position.Y + ball.height / 2 >= paddle1.position.Y &&
                ball.position.Y + ball.height / 2 <= paddle1.position.Y + paddle1.height && 
                ball.position.X <= paddle1.position.X + paddle1.width && 
                ball.position.X >= paddle1.position.X)
                return true;
            else return false;
        }

        public bool paddle2colision()
        {
            if (ball.position.Y + ball.height / 2 >= paddle2.position.Y &&
                ball.position.Y + ball.height / 2<= paddle2.position.Y + paddle2.height && 
                ball.position.X + ball.width >= paddle2.position.X &&
                ball.position.X + ball.width <= paddle2.position.X + paddle2.width)
                return true;
            else return false;
        }

        public void cpu()
        {
            if (ball.position.X >= paddle2.position.X - 100)
            {
                if (ball.position.Y + ball.height / 2 > paddle2.position.Y + 
                    paddle2.height / 3)
                {
                    paddle2.position.Y += paddle2.speed / 2;
                }
                if (ball.position.Y + ball.height / 2 < paddle2.position.Y + 
                    paddle2.height - paddle2.height / 3)
                {
                    paddle2.position.Y -= paddle2.speed / 2;
                }
            }
            else
            {
                if (paddle2.position.Y <= 0 + 5)
                    cpuUP = false;
                if (paddle2.position.Y + paddle2.height >= ScrHeight - 5)
                    cpuUP = true;
                if (cpuUP)
                    paddle2.position.Y -= paddle2.speed / 4;
                if (!cpuUP)
                    paddle2.position.Y += paddle2.speed / 4;
            }
        }

        // ++++++++++++++++++++++++++++
        // Metoda zmieniaj¹ca tor lotu pi³ki
        public void changeangle()
        {
            if (paddle1.position.Y + paddle1.height / 6 > ball.position.Y + ball.height / 2 &&
                        ball.position.Y + ball.height / 2 > paddle1.position.Y)
            {
                ball.speedX = 6;
                ball.speedY = 10;
            }

            if (paddle1.position.Y + 2 * (paddle1.height / 6) > ball.position.Y + ball.height / 2 && 
                        ball.position.Y + ball.height / 2 > paddle1.position.Y + paddle1.height / 6)
            {
                ball.speedX = 8;
                ball.speedY = 8;
            }

            if (paddle1.position.Y + 4 * (paddle1.height / 6) > ball.position.Y + ball.height / 2 && 
                        ball.position.Y + ball.height / 2 > paddle1.position.Y + 2 * (paddle1.height / 6))
            {
                ball.speedX = 10;
                ball.speedY = 6;
            }

            if (paddle1.position.Y + 5 * (paddle1.height / 6) > ball.position.Y + ball.height / 2 &&
                        ball.position.Y + ball.height / 2 > paddle1.position.Y + 4 * (paddle1.height / 6))
            {
                ball.speedX = 8;
                ball.speedY = 8;
            }

            if (paddle1.position.Y + 6 * (paddle1.height / 6) > ball.position.Y + ball.height / 2 &&
                        ball.position.Y + ball.height / 2 > paddle1.position.Y + 5 * (paddle1.height / 6))
            {
                ball.speedX = 6;
                ball.speedY = 10;
            }
        }
    }
}
