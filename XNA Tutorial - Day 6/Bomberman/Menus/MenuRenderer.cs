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
    public class MenuRenderer : DrawableGameComponent
    {
        ContentManager content;        
        SpriteBatch spriteBatch;
        public Scene scene;

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
        Color playerColor = Color.LightSkyBlue;

        // Background
        public int BackgroundIndex;
        Texture2D[] backgrounds = new Texture2D[7];

        // Controllers
        Texture2D controllers;
        Rectangle keyboardLeft, keyboardRight, gamePad;

        // Static models
        StaticModel bomb;
        BasicEffect staticModelsShader;

        // Animated models
        public bool DrawCharacters = false;
        AnimatedModel[] characters = new AnimatedModel[6];
        Effect animatedModelsShader;
        Dictionary<Item, CharacterThumb> characterInfo = new Dictionary<Item, CharacterThumb>();

        // Resizing
        float scale;
        float aspectRatio;


        // CONSTRUCTOR
        public MenuRenderer(Game game, Scene scene)
            : base(game)
        {
            this.scene = scene;
            Initialize();
        }

        // INITIALIZE
        public override void Initialize()
        {
            base.Initialize();

            content = new ContentManager(Game.Services);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Font
            font = content.Load<Font>(@"Content\GUI\font");

            // Background
            for (int i = 0; i < 7; i++)
            {
                backgrounds[i] = content.Load<Texture2D>(@"Content\GUI\background" + i.ToString());
            }

            // Bomb
            bomb = new StaticModel();
            bomb.Model = content.Load<Model>(@"Content\Models\bomb1");
            bomb.NormalMatrix =
                Matrix.CreateScale(0.5f) * 
                Matrix.CreateRotationX(-MathHelper.PiOver2) *
                Matrix.CreateRotationZ(MathHelper.Pi) *
                Matrix.CreateTranslation(0f, 0, 60f);

            // Controllers
            controllers = content.Load<Texture2D>(@"Content\GUI\controllers");

            keyboardRight = new Rectangle(0, 0, 128, 40);
            keyboardLeft = new Rectangle(0, 40, 128, 40);
            gamePad = new Rectangle(0, 80, 128, 40);

            // Characters
            AnimatedModel animatedModel;

            animatedModel = new AnimatedModel();
            animatedModel.Model = content.Load<Model>(@"Content\Characters\vesolc");
            animatedModel.NormalMatrix = Matrix.CreateScale(1.2f);
            characters[(int)CharacterType.Alien] = animatedModel;

            animatedModel = new AnimatedModel();
            animatedModel.Model = content.Load<Model>(@"Content\Characters\buca");
            animatedModel.NormalMatrix = Matrix.CreateScale(1.2f);
            characters[(int)CharacterType.Pumpkin] = animatedModel;

            animatedModel = new AnimatedModel();
            animatedModel.Model = content.Load<Model>(@"Content\Characters\ant");
            animatedModel.NormalMatrix = Matrix.CreateScale(0.85f);
            characters[(int)CharacterType.Ant] = animatedModel;

            animatedModel = new AnimatedModel();
            animatedModel.Model = content.Load<Model>(@"Content\Characters\frog");
            animatedModel.NormalMatrix = Matrix.CreateScale(1.1f);
            characters[(int)CharacterType.Frog] = animatedModel;

            animatedModel = new AnimatedModel();
            animatedModel.Model = content.Load<Model>(@"Content\Characters\robot");
            animatedModel.NormalMatrix = Matrix.CreateScale(1.2f);
            characters[(int)CharacterType.Robot] = animatedModel;

            animatedModel = new AnimatedModel();
            animatedModel.Model = content.Load<Model>(@"Content\Characters\teddy");
            animatedModel.NormalMatrix = Matrix.CreateScale(1.2f);
            characters[(int)CharacterType.Teddy] = animatedModel;


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


            // Shader for animated models
            animatedModelsShader = content.Load<Effect>(@"Content\SkinnedModel");

            animatedModelsShader.Parameters["AmbientColor"].SetValue(staticModelsShader.AmbientLightColor);

            animatedModelsShader.Parameters["Light1Direction"].SetValue(staticModelsShader.DirectionalLight0.Direction);
            animatedModelsShader.Parameters["Light1Color"].SetValue(staticModelsShader.DirectionalLight0.DiffuseColor);

            animatedModelsShader.Parameters["Light2Direction"].SetValue(staticModelsShader.DirectionalLight1.Direction);
            animatedModelsShader.Parameters["Light2Color"].SetValue(staticModelsShader.DirectionalLight1.DiffuseColor);

            animatedModelsShader.Parameters["View"].SetValue(staticModelsShader.View);
            animatedModelsShader.Parameters["Projection"].SetValue(staticModelsShader.Projection);
        }

        public void RandomizeBackground()
        {
            BackgroundIndex = Bomberman.Random.Next(1, 7);         
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


        // UPDATE
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update animations
            List<int> indices = scene.GetItemIndices();
            for (int i = 0; i < indices.Count; i++)
            {
                Item sceneItem = scene[indices[i]];
                if (sceneItem.Is<Controller>())
                {
                    if (sceneItem.As<Controller>().Player != null)
                    {
                        if (!characterInfo.ContainsKey(sceneItem))
                        {
                            characterInfo.Add(sceneItem, new CharacterThumb(characters[(int)sceneItem.As<Controller>().Player.CharacterType]));
                        }
                    }
                }
            }
        }


        // DRAW
        public override void Draw(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedRealTime.TotalSeconds;

            // Prepare for rendering
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.RenderState.DepthBufferEnable = true;
            GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            GraphicsDevice.RenderState.CullMode = CullMode.None;

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

            // Background
            spriteBatch.Draw(
                backgrounds[BackgroundIndex],
                PlaceOnScreen(new Vector3(400,0,300)),
                new Rectangle(0, 0, 600, 800),
                Color.White,
                0,
                new Vector2(300, 400),
                scale,
                SpriteEffects.None,
                1
                );

            // GUI elements
            List<int> indices = scene.GetItemIndices();
            for (int i = 0; i < indices.Count; i++)
            {
                Item sceneItem = scene[indices[i]];
                // Text
                if (sceneItem.Is<TextButton>())
                {
                    TextButton button = sceneItem.As<TextButton>();
                    Vector3 position = sceneItem.Part<PPosition>().Position;
                    font.DrawCentered(button.Text, PlaceOnScreen(position + new Vector3(2, 0, 2)), scale * button.Scale, Color.Black, spriteBatch);
                    font.DrawCentered(button.Text, PlaceOnScreen(position), scale * button.Scale, button.MouseHover ? hoverColor : textColor, spriteBatch);
                }
                // Controller
                if (sceneItem.Is<Controller>())
                {
                    Controller controller = sceneItem.As<Controller>();
                    Vector3 position = sceneItem.Part<PPosition>().Position;
                    Rectangle source;
                    switch (controller.ControlType)
                    {
                        case ControlType.KeyboardLeft:
                            source = keyboardLeft;
                            break;
                        case ControlType.KeyboardRight:
                            source = keyboardRight;
                            break;
                        default:
                            source = gamePad;
                            break;
                    }
                    spriteBatch.Draw(controllers, PlaceOnScreen(position), source, Color.White, 0, new Vector2(64, 20), 1, SpriteEffects.None, 1);
                    if (controller.Player != null)
                    {
                        font.DrawCentered(controller.Player.Name, PlaceOnScreen(position + new Vector3(1, 0, 21)), scale * 0.25f, Color.Black, spriteBatch);
                        font.DrawCentered(controller.Player.Name, PlaceOnScreen(position + new Vector3(0, 0, 20)), scale * 0.25f, playerColor, spriteBatch);
                    }
                }
            }

            spriteBatch.End();

            // Character thumbs
            if (DrawCharacters)
            {
                for (int i = 0; i < indices.Count; i++)
                {
                    Item sceneItem = scene[indices[i]];
                    if (sceneItem.Is<Controller>())
                    {
                        Controller controller = sceneItem.As<Controller>();
                        Vector3 position = sceneItem.Part<PPosition>().Position;
                        if (controller.Player != null)
                        {
                            CharacterThumb info = characterInfo[sceneItem];
                            float updateTime = elapsed;
                            info.Update(updateTime, characters[(int)controller.Player.CharacterType]);
                            info.Model.Render(
                                TimeSpan.FromSeconds(updateTime),
                                Matrix.CreateFromQuaternion(info.Part<PAngularPosition>().AngularPosition) *
                                Matrix.CreateTranslation(position + Vector3.Forward * 14),
                                animatedModelsShader);
                        }
                    }
                }
            }


            // Mouse on top
            for (int i = 0; i < indices.Count; i++)
            {
                Item sceneItem = scene[indices[i]];
                if (sceneItem.Is<MousePointer>())
                {
                    Vector3 position = sceneItem.Part<PPosition>().Position;
                    bomb.Render(Matrix.CreateTranslation(position + Vector3.Up * 50), staticModelsShader);
                }
            }



        } 
    }
}
