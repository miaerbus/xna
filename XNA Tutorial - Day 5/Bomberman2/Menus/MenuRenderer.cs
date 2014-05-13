using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;
using Artificial.XNATutorial.GUI;
using FontRenderer;

namespace Artificial.XNATutorial.Bomberman
{
    class MenuRenderer : DrawableGameComponent
    {
        ContentManager content;        
        SpriteBatch spriteBatch;
        Scene scene;

        // Font
        Font font;
        public Font Font
        {
            get
            {
                return font;
            }
        }
        Color textColor = Color.White;
        Color hoverColor = Color.Red;

        // Background
        Texture2D background;

        // Static models
        StaticModel bomb;
        BasicEffect staticModelsShader;

        // Resizing
        float scale;
        float aspectRatio;


        // CONSTRUCTOR
        public MenuRenderer(Game game, Scene scene)
            : base(game)
        {
            this.scene = scene;
        }

        // INITIALIZE
        public override void Initialize()
        {
            base.Initialize();

            content = new ContentManager(Game.Services);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Font
            font = content.Load<Font>(@"Content\GUI\font");

            // Background
            background = content.Load<Texture2D>(@"Content\GUI\background" + Bomberman.Random.Next(1,5).ToString());

            //Bomb
            bomb = new StaticModel();
            bomb.Model = content.Load<Model>(@"Content\Models\bomb1");
            bomb.NormalMatrix =
                Matrix.CreateScale(0.5f) * 
                Matrix.CreateRotationX(-MathHelper.PiOver2) *
                Matrix.CreateRotationZ(MathHelper.Pi) *
                Matrix.CreateTranslation(0f, 0, 60f);


            // Shader for static models
            staticModelsShader = new BasicEffect(GraphicsDevice, null);
            staticModelsShader.TextureEnabled = true;

            staticModelsShader.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);
            staticModelsShader.SpecularColor = new Vector3(0.2f, 0.2f, 0.2f);
            staticModelsShader.SpecularPower = 8f;

            staticModelsShader.LightingEnabled = true;
            staticModelsShader.AmbientLightColor = new Vector3(0.1f, 0.15f, 0.2f); ;

            staticModelsShader.DirectionalLight0.Enabled = true;
            staticModelsShader.DirectionalLight0.SpecularColor = new Vector3(0.5f, 0.3f, 0.1f);
            staticModelsShader.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.9f, 0.9f);
            staticModelsShader.DirectionalLight0.Direction = new Vector3(1, -2, 1);

            staticModelsShader.DirectionalLight1.Enabled = true;
            staticModelsShader.DirectionalLight1.SpecularColor = new Vector3(0.0f, 0.0f, 1.0f);
            staticModelsShader.DirectionalLight1.DiffuseColor = new Vector3(0.0f, 0.0f, 1.0f);
            staticModelsShader.DirectionalLight1.Direction = new Vector3(-1, -0.5f, -1);

            aspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;
            scale = (float)GraphicsDevice.Viewport.Height / 600f;
            float offsetX = Bomberman.Device.Viewport.Width - (float)Bomberman.Device.Viewport.Height * 1.33333f;
            offsetX *= scale;

            staticModelsShader.Projection = Matrix.CreateOrthographic(600 * aspectRatio, 600, -1000, 1000);
            staticModelsShader.View =
                Matrix.CreateRotationX(MathHelper.PiOver2) *
                Matrix.CreateTranslation(-400, 300, 0);
        }

        // Transform
        Vector2 PlaceOnScreen(Vector3 position)
        {
            Vector2 result = new Vector2(position.X, position.Z);
            result -= new Vector2(400, 300);
            result *= scale;
            result +=new Vector2(GraphicsDevice.Viewport.Width * 0.5f, GraphicsDevice.Viewport.Height * 0.5f);
            return result;
        }

        // DRAW
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            float elapsed = (float)gameTime.ElapsedRealTime.TotalSeconds;

            // Prepare for rendering
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.RenderState.DepthBufferEnable = true;
            GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            GraphicsDevice.RenderState.CullMode = CullMode.None;

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

            // Background
            spriteBatch.Draw(
                background,
                PlaceOnScreen(new Vector3(400,0,300)),
                new Rectangle(0, 0, 600, 800),
                Color.White,
                0,
                new Vector2(300, 400),
                scale,
                SpriteEffects.None,
                0
                );

            // Text
            List<int> indices = scene.GetItemIndices();
            for (int i = 0; i < indices.Count; i++)
            {
                Item sceneItem = scene[indices[i]];
                if (sceneItem.Is<TextButton>())
                {
                    TextButton button = sceneItem.As<TextButton>();
                    Vector3 position = sceneItem.Part<PPosition>().Position;
                    font.DrawCentered(button.Text, PlaceOnScreen(position + new Vector3(2, 0, 2)), scale, Color.Black, spriteBatch);
                    font.DrawCentered(button.Text, PlaceOnScreen(position), scale, button.MouseHover ? hoverColor : textColor, spriteBatch);
                }
            }

            spriteBatch.End();


            // Mouse on top
            for (int i = 0; i < indices.Count; i++)
            {
                Item sceneItem = scene[indices[i]];
                if (sceneItem.Is<MousePointer>())
                {
                    Vector3 position = sceneItem.Part<PPosition>().Position;
                    bomb.Render(Matrix.CreateTranslation(position), staticModelsShader);
                }
            }

        } 
    }
}
