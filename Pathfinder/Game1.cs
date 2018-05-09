#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
//using Microsoft.Xna.Framework.Net;
//using Microsoft.Xna.Framework.Storage;

#endregion

namespace Pathfinder
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {   
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //sprite texture for tiles, player, and ai bot
        Texture2D tile1Texture;
        Texture2D tile2Texture;
        Texture2D aiTexture;
        Texture2D playerTexture;

        //objects representing the level map, bot, and player 
        private Level level;
        private AiBotBase bot;
        private Player player;

        //screen size and frame rate
        private const int TargetFrameRate = 50;
        private const int BackBufferWidth = 600;
        private const int BackBufferHeight = 600;
    
        public Game1()
        {
            //constructor
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = BackBufferHeight;
            graphics.PreferredBackBufferWidth = BackBufferWidth;
            Window.Title = "Pathfinder";
            Content.RootDirectory = "../../../Content";
            //set frame rate
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);
            //load level map
            level = new Level();
            level.Loadmap("../../../Content/1.txt");
            //instantiate bot and player objects
            //bot = new AiBotRandom(10, 20);
            bot = new AiBotSimple2(10, 20);
            player = new Player(30, 20);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //load the sprite textures
            Content.RootDirectory = "../../../Content";
            tile1Texture = Content.Load<Texture2D>("tile1");
            tile2Texture = Content.Load<Texture2D>("tile2");
            aiTexture = Content.Load<Texture2D>("ai");
            playerTexture = Content.Load<Texture2D>("target");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //player movement: read keyboard
            KeyboardState keyState = Keyboard.GetState();
            Coord2 currentPos = new Coord2();
            currentPos = player.GridPosition;
            if(keyState.IsKeyDown(Keys.Up))
            {
                currentPos.Y -= 1;
                player.SetNextLocation(currentPos, level);
                level.astar.Build(level, bot, player);
            }
            else if (keyState.IsKeyDown(Keys.Down))
            {
                currentPos.Y += 1;
                player.SetNextLocation(currentPos, level);
                level.astar.Build(level, bot, player);
            }
            else if (keyState.IsKeyDown(Keys.Left))
            {
                currentPos.X -= 1;
                player.SetNextLocation(currentPos, level);
                level.astar.Build(level, bot, player);
            }
            else if (keyState.IsKeyDown(Keys.Right))
            {
                currentPos.X += 1;
                player.SetNextLocation(currentPos, level);
                level.astar.Build(level, bot, player);
            }
            else if (keyState.IsKeyDown(Keys.P))
            {
                level.dijkstra.Build(level, bot, player);
            }
            else if (keyState.IsKeyDown(Keys.A))
            {
                level.astar.Build(level, bot, player);
            }

            //update bot and player
            bot.Update(gameTime, level, player);
            player.Update(gameTime, level);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            //draw level map
            DrawGrid();
            //draw bot
            spriteBatch.Draw(aiTexture, bot.ScreenPosition, Color.White);
            //drawe player
            spriteBatch.Draw(playerTexture, player.ScreenPosition, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGrid()
        {
            //draws the map grid
            int sz = level.GridSize;
            for (int x = 0; x < sz; x++)
            {
                for (int y = 0; y < sz; y++)
                {
                    Coord2 pos = new Coord2((x*15), (y*15));
                    //if (level.tiles[x, y] == 0) spriteBatch.Draw(tile1Texture, pos, Color.White);
                    //else spriteBatch.Draw(tile2Texture, pos, Color.White);

                    if (level.tiles[x, y] == 0)
                    {
                        if (level.astar.inPath[x, y])
                        {
                            spriteBatch.Draw(tile1Texture, pos, Color.Red);
                        }
                        else if (level.astar.closed[x, y])
                        {
                            spriteBatch.Draw(tile1Texture, pos, Color.Blue);
                        }
                        else if (!level.astar.closed[x, y] && level.astar.cost[x, y] < 1000000)
                        {
                            spriteBatch.Draw(tile1Texture, pos, Color.LightBlue);
                        }
                        else
                        {
                            spriteBatch.Draw(tile1Texture, pos, Color.White);
                        }
                    }
                    else
                    {
                        spriteBatch.Draw(tile2Texture, pos, Color.White);
                    }
                }
            }
        }
    }
}
