using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Pong
{
    class PongRenderer : DrawableGameComponent
    {
        ContentManager content;
        
        SpriteBatch spriteBatch;
        Texture2D pong;

        Sprite paddle;
        Sprite ball;           

        Scene scene;
        Camera camera;

        // CONSTRUCTOR
        public PongRenderer(Game game, Scene scene, Camera camera)
            : base(game)
        {
            // Add scene handlers
            this.scene = scene;
            this.camera = camera;
        }

        // INITIALIZE
        public override void Initialize()
        {
            // Graphics
            base.Initialize();
            content = new ContentManager(Game.Services);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pong = content.Load<Texture2D>(@"Content\pong");

            paddle = new Sprite();
            paddle.Texture = pong;
            paddle.NormalMatrix = Matrix.CreateScale(34, 100, 1);
            paddle.SourceRectangle=new Rectangle(100, 800, 34, 100);
            paddle.Origin = new Vector2(16, 50);

            ball = new Sprite();
            ball.Texture = pong;
            ball.NormalMatrix = Matrix.CreateScale(28);
            ball.SourceRectangle = new Rectangle(200, 800, 30, 30);
            ball.Origin = new Vector2(14, 14);

        }

        // DRAW
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            // Background
            spriteBatch.Draw(pong, Vector2.Zero, new Rectangle(0, 0, 800, 600), Color.White);

            // Items on scene
            List<int> indices = scene.GetItemIndices();
            for (int i = 0; i < indices.Count; i++)
            {
                Item sceneItem = scene[indices[i]];
                if (sceneItem as Paddle != null)
                {
                    paddle.Render(spriteBatch, Matrix.CreateTranslation(sceneItem.As<PPosition>().Position), camera);
                }
                if (sceneItem as Ball != null)
                {
                    ball.Render(spriteBatch, Matrix.CreateTranslation(sceneItem.As<PPosition>().Position), camera);
                }
            }

            // Score
            for (int i = 0; i < 2; i++)
            {
                float x = (i * 2 - 1) * 220 + 350;
                spriteBatch.Draw(pong, new Vector2(x, 80), new Rectangle(Pong.Score[i] * 100, 610, 100, 80), Color.White);
            }

            spriteBatch.End();
        }

    }
}
