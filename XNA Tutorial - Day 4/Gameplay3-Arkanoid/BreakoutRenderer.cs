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
        Sprite paddleLeft;
        Sprite paddleRight;
        Sprite paddleMiddle;
        Sprite[] powerup = new Sprite[10];

        Color[] brickColor = new Color[] {Color.Red, Color.Yellow, Color.Blue, Color.Fuchsia, Color.Lime };
        Color[] powerBrickColor = new Color[] { Color.Gray, Color.LightGray };

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

            paddleLeft = new Sprite();
            paddleLeft.Texture = breakout;
            paddleLeft.NormalMatrix = Matrix.CreateScale(25, 25, 1);
            paddleLeft.SourceRectangle = new Rectangle(125, 26, 25, 23);
            paddleLeft.Origin = new Vector2(25, 12);

            paddleRight = new Sprite();
            paddleRight.Texture = breakout;
            paddleRight.NormalMatrix = Matrix.CreateScale(25, 25, 1);
            paddleRight.SourceRectangle = new Rectangle(200, 26, 25, 23);
            paddleRight.Origin = new Vector2(0, 12);

            paddleMiddle = new Sprite();
            paddleMiddle.Texture = breakout;
            paddleMiddle.NormalMatrix = Matrix.CreateScale(50, 25, 1);
            paddleMiddle.SourceRectangle = new Rectangle(150, 26, 50, 23);
            paddleMiddle.Origin = new Vector2(25, 12);

            ball = new Sprite();
            ball.Texture = breakout;
            ball.NormalMatrix = Matrix.CreateScale(25, 25, 1);
            ball.SourceRectangle = new Rectangle(225, 0, 25, 25);
            ball.Origin = new Vector2(13, 13);

            brick = new Sprite();
            brick.Texture = breakout;
            brick.NormalMatrix = Matrix.CreateScale(50, 25, 1);
            brick.SourceRectangle = new Rectangle(125, 0, 50, 25);
            brick.Origin = new Vector2(25, 13);

            for (int i = 0; i < powerup.Length; i++)
            {
                powerup[i] = new Sprite();
                powerup[i].Texture = breakout;
                powerup[i].NormalMatrix = Matrix.CreateScale(50);
                int x = i % 5;
                int y = i / 5;
                powerup[i].SourceRectangle = new Rectangle(x * 50, 50 + y * 50, 50, 50);
                powerup[i].Origin = new Vector2(25, 25);
            }
        }

        // DRAW
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Color.DarkBlue);
            spriteBatch.Begin();

            // Items on scene
            List<int> indices = scene.GetItemIndices();
            for (int i = 0; i < indices.Count; i++)
            {
                Item sceneItem = scene[indices[i]];
                if (sceneItem as Paddle != null)
                {
                    float width = sceneItem.As<Paddle>().Width;
                    Vector3 position = sceneItem.As<PPosition>().Position;

                    paddleMiddle.NormalMatrix = Matrix.CreateScale(width-30, 25, 1);
                    paddleMiddle.Render(spriteBatch, Matrix.CreateTranslation(position), camera);

                    position = sceneItem.As<PPosition>().Position ;
                    paddleLeft.Render(spriteBatch, Matrix.CreateTranslation(position + Vector3.Left*(width / 2 - 15)), camera);

                    position = sceneItem.As<PPosition>().Position ;
                    paddleRight.Render(spriteBatch, Matrix.CreateTranslation(position + Vector3.Right*(width / 2 - 15)), camera);

                }
                if (sceneItem as Ball != null)
                {
                    ball.Render(spriteBatch, Matrix.CreateTranslation(sceneItem.As<PPosition>().Position), camera);
                }
                if (sceneItem as Brick != null)
                {
                    int row = (int)((-sceneItem.As<PPosition>().Position.Y + 300) / 25f);
                    Color c = brickColor[row % brickColor.Length];
                    int power = sceneItem.As<Brick>().Power;
                    if (power > 1) c = powerBrickColor[power - 2];
                    brick.Render(spriteBatch, Matrix.CreateTranslation(sceneItem.As<PPosition>().Position), camera, c);
                }
                if (sceneItem as Magnet != null)
                {
                    powerup[2].Render(spriteBatch, Matrix.CreateTranslation(sceneItem.As<PPosition>().Position), camera);
                }
                if (sceneItem as Breakthrough != null)
                {
                    powerup[3].Render(spriteBatch, Matrix.CreateTranslation(sceneItem.As<PPosition>().Position), camera);
                }

            }

            // Score
            for (int i = 0; i < Breakout.Lives; i++)
            {
                float x = i * 50;
                spriteBatch.Draw(breakout, new Vector2(x, 575), new Rectangle(0, 150, 50, 25), Color.White);
            }

            spriteBatch.End();
        }

    }
}
