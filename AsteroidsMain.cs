using AsteroidsMain.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace AsteroidsMain
{
    internal class AsteroidsMain : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        private MouseState previousMouseState;

        public static List<Asteroid> Polygons = new List<Asteroid>();

        public AsteroidsMain()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public static T NewRandom<T>(int min, int max) where T : struct
        {
            double value = min + (new Random().NextDouble() * (max - min));

            // Use Convert.ChangeType to cast to the desired type
            return (T)Convert.ChangeType(value, typeof(T));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                Asteroid.AddAsteroid(GraphicsDevice, new Vector2(mouseState.Position.X, mouseState.Position.Y), new Vector2(NewRandom<float>(-3, 3), NewRandom<float>(-3, 3)), NewRandom<float>(-7, 7));
            }

            previousMouseState = mouseState;

            var screenWidth = GraphicsDevice.Viewport.Width;
            var screenHeight = GraphicsDevice.Viewport.Height;

            if (AsteroidsMain.Polygons.Count > 0)
            {
                foreach (var polygon in AsteroidsMain.Polygons)
                {
                    polygon.Update();
                    Collision.Check(polygon);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.BlueViolet);

            _spriteBatch.Begin();

            if (Polygons.Count > 0)  
            {
                foreach (var polygon in Polygons)
                {
                    polygon.DrawAtPosition(_spriteBatch);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
