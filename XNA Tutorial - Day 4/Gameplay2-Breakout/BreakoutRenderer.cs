using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Breakout
{
    class BreakoutRenderer : DrawableGameComponent
    {
        ContentManager content;
        
        SpriteBatch spriteBatch;
        Texture2D breakout;

        Sprite brick;
        Sprite ball;
        Sprite paddle;

        Color[] brickColor = new Color[] { Color.Red, Color.Orange, Color.Yellow, Color.Lime, Color.Blue };

        Scene scene;
        Camera camera;

        // CONSTRUCTOR
        public BreakoutRenderer(Game game, Scene scene, Camera camera)
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
            breakout = content.Load<Texture2D>(@"Content\breakout");

            paddle = new Sprite();
            paddle.Texture = breakout;
            paddle.NormalMatrix = Matrix.CreateScale(125, 50, 1);
            paddle.SourceRectangle=new Rectangle(0, 0, 125, 50);
            paddle.Origin = new Vector2(62, 30);
            paddle.Color = Color.Black;

            ball = new Sprite();
            ball.Texture = breakout;
            ball.NormalMatrix = Matrix.CreateScale(25);
            ball.SourceRectangle = new Rectangle(225, 0, 25, 25);
            ball.Origin = new Vector2(13, 13);
            ball.Color = Color.Black;

            brick = new Sprite();
            brick.Texture = breakout;
            brick.NormalMatrix = Matrix.CreateScale(50, 25, 1);
            brick.SourceRectangle = new Rectangle(125, 0, 50, 25);
            brick.Origin = new Vector2(25, 13);

        }

        // DRAW
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Color.WhiteSmoke);
            spriteBatch.Begin();

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
                if (sceneItem as Brick != null)
                {
                    int row = (int)((-sceneItem.As<PPosition>().Position.Y + 200) / 25f);
                    brick.Render(spriteBatch, Matrix.CreateTranslation(sceneItem.As<PPosition>().Position), camera, brickColor[row]);
                }
            }

            // Score
            for (int i = 0; i < Breakout.Lives; i++)
            {
                float x = 750 - i * 50;
                spriteBatch.Draw(breakout, new Vector2(x, 0), new Rectangle(178, 2, 46, 21), Color.Black);
            }

            spriteBatch.End();
        }

    }
}
