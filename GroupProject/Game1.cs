using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
namespace GroupProject
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        bool GameRunning = true;
        bool GameLose;
        bool GameWin;

        private SoundEffect death;
        private SoundEffect shoot;
        private SoundEffect node;
        private SoundEffect bgm;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Collision collision = new Collision();
        Sprites test = new Sprites(1280, 720, 1f);  // Create new Sprite/Player
        Player player = new Player(50, 50, 1f);
        Sprites win = new Sprites(1280, 720, 1f);
        Sprites lose = new Sprites(1280, 720, 1f);
        Sprites cursor = new Sprites(11, 19, 1f);
        Text score = new Text();    // Create Text
        Grid Level = new Grid(22,11);
        AI_MainFrame[] AITest;
        Intel[] Intelligence;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            graphics.PreferredBackBufferWidth = 1280;   // Set Screen X
            graphics.PreferredBackBufferHeight = 720;   // Set Screen Y
            player.Position = new Vector2(150, 100);
            
            //Intelligence.Position = new Vector2(50, 350);

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Level.LoadGrid();
            Level.MakeSprite();
            AITest = new AI_MainFrame[8];
            AITest[0] = new AI_MainFrame(new Vector2(10,5), new Vector2(10,4), new Vector2(10,9), Level);
            AITest[1] = new AI_MainFrame(new Vector2(3,5), new Vector2(4,5), new Vector2(7,5), Level);
            AITest[2] = new AI_MainFrame(new Vector2(6, 7), new Vector2(6, 8), new Vector2(6, 9), Level);

            AITest[3] = new AI_MainFrame(new Vector2(12, 3), new Vector2(12, 2), new Vector2(12, 5), Level);
            AITest[4] = new AI_MainFrame(new Vector2(14, 7), new Vector2(14, 6), new Vector2(14, 9), Level);
            AITest[5] = new AI_MainFrame(new Vector2(16, 8), new Vector2(16, 7), new Vector2(16, 9), Level);

            AITest[6] = new AI_MainFrame(new Vector2(16, 5), new Vector2(16, 3), new Vector2(16, 2), Level);
            AITest[7] = new AI_MainFrame(new Vector2(20, 2), new Vector2(19, 2), new Vector2(17, 2), Level);

            Intelligence = new Intel[3];

            Intelligence[0] = new Intel(1, 1, 1f);
            Intelligence[1] = new Intel(1, 1, 1f);
            Intelligence[2] = new Intel(1, 1, 1f);

            Intelligence[0].Position = new Vector2(150, 350);
            Intelligence[1].Position = new Vector2(1000, 200);
            Intelligence[2].Position = new Vector2(1000, 400);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here


            Intelligence[0].tex = Content.Load<Texture2D>("Sprites/Intel");
            Intelligence[1].tex = Content.Load<Texture2D>("Sprites/Intel");
            Intelligence[2].tex = Content.Load<Texture2D>("Sprites/Intel");

            win.tex = Content.Load<Texture2D>("Sprites/win");
            lose.tex = Content.Load<Texture2D>("Sprites/lose");
            cursor.tex = Content.Load<Texture2D>("Sprites/cursor");
            player.tex = Content.Load<Texture2D>("Sprites/Gcircle");
            player.bullet.tex = Content.Load<Texture2D>("Sprites/Gbullet");
            score.font = Content.Load<SpriteFont>("Fonts/Score"); // Use the name of your sprite font file here instead of 'Score'.
            
            bgm = Content.Load<SoundEffect>("Sounds/bgm");
            SoundEffectInstance soundEffectInstance = bgm.CreateInstance();
            soundEffectInstance.IsLooped = true;
            death = Content.Load<SoundEffect>("Sounds/die");
            shoot = Content.Load<SoundEffect>("Sounds/gun");
            node = Content.Load<SoundEffect>("Sounds/node");

            for (int j = 0; j < AITest.Length; j++)
            {
                AITest[j].AISprite.tex = Content.Load<Texture2D>("Sprites/Rcircle");
            }
            for (int i = 0; i < Level.GridSprites.Length; i++)
            {
                Level.GridSprites[i].tex = Content.Load<Texture2D>("Sprites/Wsquare");
            }
            soundEffectInstance.Play();
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
            // TODO: Add your update logic here

            base.Update(gameTime);

            Vector2 oldPos = new Vector2(player.Position.X, player.Position.Y); // Position for the player
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < AITest.Length; j++)
                {
                    AITest[j].Patrol();
                    AITest[j].AISprite.Update();
                    if (AITest[j].AISprite.isDead == false)
                    {
                        if (Collision.CheckCollision(player.bullet, AITest[j].AISprite)) // Check collision for player and wall
                        {
                            AITest[j].AISprite.isDead = true;
                            player.isShot = false;
                        }
                        if (Collision.CheckCollision(player, AITest[j].AISprite)) // Check collision for player and wall
                        {
                            death.Play();
                            GameRunning = false;
                            GameLose = true;
                        }
                    }
                }
            }

            player.Update(Level, shoot);
            cursor.Position = new Vector2(player.MouseX, player.MouseY);
            for (int i = 0; i < Level.GridSprites.Length; i++)
            {
                if (Collision.CheckCollision(player, Level.GridSprites[i])) // Check collision for player and wall
                {
                    player.Position = oldPos;
                }
                if (Collision.CheckCollision(player.bullet, Level.GridSprites[i]))
                {
                    player.isShot = false;
                }
            }

            for (int i = 0; i < Intelligence.Length; i++)
            {
                if (Intelligence[i].IsCaptured == false)
                {
                    if (Collision.CheckCollision(player, Intelligence[i]))
                    {
                        node.Play();
                        Intelligence[i].IsCaptured = true;
                        score.score++;

                    }
                }
            }

            if (score.score == 3)
            {
                GameRunning = false;
                GameWin = true;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);
            KeyboardState newState = Keyboard.GetState();  // Check for keyboard input

            spriteBatch.Begin();
            if (GameRunning)
            {
                // TODO: Add your drawing code here

                if (player.isShot)
                {
                    player.bullet.Draw(spriteBatch);
                }
                for (int i = 0; i < Intelligence.Length; i++)
                {
                    if (Intelligence[i].IsCaptured == false)
                    {
                        Intelligence[i].Draw(spriteBatch);
                    }
                }

                player.Draw(spriteBatch);

                for (int j = 0; j < AITest.Length; j++)
                {
                    if (AITest[j].AISprite.isDead == false)
                    {
                        AITest[j].AISprite.Draw(spriteBatch);
                    }
                }

                for (int i = 0; i < Level.GridSprites.Length; i++)
                {
                    Level.GridSprites[i].Draw(spriteBatch);
                }


            cursor.Draw(spriteBatch);


                score.Draw(spriteBatch, player.shots);

                base.Draw(gameTime);
            }

            if (GameWin)
            {
                win.Draw(spriteBatch);
                player.Position = new Vector2(150, 100);

                for (int i = 0; i < Intelligence.Length; i++)
                {
                    Intelligence[i].IsCaptured = false;
                }

                score.score = 0;
                if (newState.IsKeyDown(Keys.Y))
                {
                    GameWin = false;
                    GameRunning = true;
                    player.shots = 3;
                }

                if (newState.IsKeyDown(Keys.N))
                {
                    GameLose = false;
                    Exit();
                }

                for (int i = 0; i < AITest.Length; i++)
                {
                    AITest[i].AISprite.isDead = false;
                }
            }
            if (GameLose)
            {
                lose.Draw(spriteBatch);
                player.Position = new Vector2(150, 100);

                for (int i = 0; i < Intelligence.Length; i++)
                {
                    Intelligence[i].IsCaptured = false;
                }

                score.score = 0;
                if (newState.IsKeyDown(Keys.Y))
                {
                    GameLose = false;
                    GameRunning = true;
                    player.shots = 3;
                }

                if (newState.IsKeyDown(Keys.N))
                {
                    GameLose = false;
                    Exit();
                }
                for (int i = 0; i < AITest.Length; i++)
                {
                    AITest[i].AISprite.isDead = false;
                }
            }
            spriteBatch.End();
        }
    }
}
