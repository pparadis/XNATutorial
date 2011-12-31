using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace XNATutorial
{
    public struct PlayerData
    {
        public Vector2 Position;
        public bool IsAlive;
        public Color Color;
        public float Angle;
        public float Power;
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;
        private Texture2D backgroundTexture;
        private Texture2D foregroundTexture;
        private Texture2D carriageTexture;
        private Texture2D cannonTexture;
        private int screenWidth;
        private int screenHeight;
        PlayerData[] players;
        int numberOfPlayers = 4;
        private float playerScaling;
        private int currentPlayer = 0;
        private SpriteFont font;
        private Texture2D rocketTexture;

        private bool rocketFlying = false;

        private Vector2 rocketPosition;
        private Vector2 rocketDirection;
        private float rocketAngle;
        private float rocketScaling = 0.1f;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        private void SetUpPlayers()
        {
            var playerColors = new Color[10];
            playerColors[0] = Color.Red;
            playerColors[1] = Color.Green;
            playerColors[2] = Color.Blue;
            playerColors[3] = Color.Purple;
            playerColors[4] = Color.Orange;
            playerColors[5] = Color.Indigo;
            playerColors[6] = Color.Yellow;
            playerColors[7] = Color.SaddleBrown;
            playerColors[8] = Color.Tomato;
            playerColors[9] = Color.Turquoise;

            players = new PlayerData[numberOfPlayers];
            for (var i = 0; i < numberOfPlayers; i++)
            {
                players[i].IsAlive = true;
                players[i].Color = playerColors[i];
                players[i].Angle = MathHelper.ToRadians(90);
                players[i].Power = 100;
            }

            players[0].Position = new Vector2(100, 193);
            players[1].Position = new Vector2(200, 212);
            players[2].Position = new Vector2(300, 361);
            players[3].Position = new Vector2(400, 164);
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 500;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Riemer's 2D XNA Tutorial";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;

            backgroundTexture = Content.Load<Texture2D>("background");
            foregroundTexture = Content.Load<Texture2D>("foreground");

            carriageTexture = Content.Load<Texture2D>("carriage");
            cannonTexture = Content.Load<Texture2D>("cannon");

            font = Content.Load<SpriteFont>("myFont");

            rocketTexture = Content.Load<Texture2D>("rocket");

            screenWidth = device.PresentationParameters.BackBufferWidth;
            screenHeight = device.PresentationParameters.BackBufferHeight;

            SetUpPlayers();

            playerScaling = 40.0f/carriageTexture.Width;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            ProcessKeyboard();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            base.Update(gameTime);
        }

        private void ProcessKeyboard()
        {
            var keyboardState = Keyboard.GetState();
            if(keyboardState.IsKeyDown(Keys.Left))
            {
                players[currentPlayer].Angle -= 0.01f;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                players[currentPlayer].Angle += 0.01f;
            }

            if(players[currentPlayer].Angle > MathHelper.PiOver2)
            {
                players[currentPlayer].Angle = -MathHelper.PiOver2;
            }

            if (players[currentPlayer].Angle < -MathHelper.PiOver2)
            {
                players[currentPlayer].Angle = MathHelper.PiOver2;
            }

            if(keyboardState.IsKeyDown(Keys.Down))
            {
                players[currentPlayer].Power -= 1;
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                players[currentPlayer].Power += 1;
            }

            if (keyboardState.IsKeyDown(Keys.PageDown))
            {
                players[currentPlayer].Power -= 20;
            }

            if (keyboardState.IsKeyDown(Keys.PageUp))
            {
                players[currentPlayer].Power += 20;
            }

            if(players[currentPlayer].Power > 1000)
            {
                players[currentPlayer].Power = 1000;
            }

            if (players[currentPlayer].Power < 0)
            {
                players[currentPlayer].Power = 0;
            }

            if(keyboardState.IsKeyDown(Keys.Enter) || keyboardState.IsKeyDown(Keys.Space))
            {
                rocketFlying = true;

                rocketPosition = players[currentPlayer].Position;
                rocketPosition.X += 20;
                rocketPosition.Y -=10;
                rocketAngle = players[currentPlayer].Angle;
                Vector2 up = new Vector2(0, -1);
                Matrix rotMatrix = Matrix.CreateRotationZ(rocketAngle);
                rocketDirection = Vector2.Transform(up, rotMatrix);
                rocketDirection *= players[currentPlayer].Power/50.0f;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            DrawScenery();
            DrawPlayers();
            DrawText();
            DrawRocket();
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawScenery()
        {
            var screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            spriteBatch.Draw(foregroundTexture, screenRectangle, Color.White);
        }

        private void DrawPlayers()
        {
            foreach (var player in players)
            {
                if (player.IsAlive)
                {
                    var xPos = (int) player.Position.X;
                    var yPos = (int) player.Position.Y;
                    var cannonOrigin = new Vector2(11,50);
                    spriteBatch.Draw(cannonTexture,new Vector2(xPos+20, yPos - 10), null, player.Color, player.Angle, cannonOrigin,playerScaling,SpriteEffects.None,1);
                    spriteBatch.Draw(carriageTexture, player.Position, null, player.Color, 0, new Vector2(0, carriageTexture.Height), playerScaling, SpriteEffects.None, 0);
                }
            }
        }

        private void DrawText()
        {
            var player = players[currentPlayer];
            var currentAngle = (int) MathHelper.ToDegrees(player.Angle);
            spriteBatch.DrawString(font, "Cannon angle: " + currentAngle, new Vector2(20, 20), player.Color);
            spriteBatch.DrawString(font, "Cannon power: " + player.Power, new Vector2(20, 45), player.Color);
        }

        private void DrawRocket()
        {
            if(rocketFlying)
            {
                spriteBatch.Draw(rocketTexture, rocketPosition, null,players[currentPlayer].Color, rocketAngle, new Vector2(42, 240), rocketScaling, SpriteEffects.None, 1 );
            }
        }
    }
}
