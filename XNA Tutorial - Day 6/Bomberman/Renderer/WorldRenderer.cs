using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;
using FontRenderer;

namespace Artificial.XNATutorial.Bomberman
{
    public class WorldRenderer : DrawableGameComponent
    {
        ContentManager content;        
        SpriteBatch spriteBatch;
        public Level level;
        float walkSpeed = 0.04f;

        // Font
        Font font;
        Color playerColor = Color.White;

        // Static models
        StaticModel[] buildings = new StaticModel[40];
        StaticModel bomb;
        StaticModel door;
        BasicEffect staticModelsShader;

        // Animated models
        AnimatedModel[] characters = new AnimatedModel[6];
        Effect animatedModelsShader;

        // Explosions
        List<Sprite> explosionSprites = new List<Sprite>();
        Texture2D sceneRendering;
        Texture2D explosionRendering;
        Vector3 cameraTarget;
        Vector3 cameraShake;
        float shakeSpeed = Bomberman.GridSize * 2;
        float shakePower;

        // Rendering collections
        List<Texture2D> textures = new List<Texture2D>();
        Dictionary<Texture2D, List<StaticModel>> models = new Dictionary<Texture2D, List<StaticModel>>();
        Dictionary<Texture2D, Dictionary<StaticModel, List<Matrix>>> worldMatrices = new Dictionary<Texture2D, Dictionary<StaticModel, List<Matrix>>>();
        Dictionary<StaticModel, Texture2D> textureMap = new Dictionary<StaticModel, Texture2D>();
        Dictionary<Item, CharacterRenderInfo> characterInfo = new Dictionary<Item, CharacterRenderInfo>();
        LinkedList<ExplosionFX> explosions = new LinkedList<ExplosionFX>();
        LinkedListNode<ExplosionFX> explosionNode;

        // CONSTRUCTOR
        public WorldRenderer(Game game, Level level)
            : base(game)
        {
            this.level = level;
            Initialize();
        }

        // INITIALIZE
        public override void Initialize()
        {
            base.Initialize();

            content = new ContentManager(Game.Services);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            StaticModel model;
            Sprite sprite;

            //Font
            font = content.Load<Font>(@"Content\GUI\font");

            // Buildings
            for (int i = 1; i <= 9; i++)
            {
                model = new StaticModel();
                model.Model = content.Load<Model>(@"Content\Models\building" + i);               
                model.NormalMatrix = Matrix.CreateScale(0.05f);
                addModel((model.Model.Meshes[0].Effects[0] as BasicEffect).Texture,model, (BuildingType)i);
            }

            //Lobby
            model = new StaticModel();
            model.Model = content.Load<Model>(@"Content\Lobby\floor");
            model.NormalMatrix = Matrix.CreateScale(1f);
            addModel((model.Model.Meshes[0].Effects[0] as BasicEffect).Texture, model, BuildingType.LobbyFloor);

            model = new StaticModel();
            model.Model = content.Load<Model>(@"Content\Lobby\doorframe");
            model.NormalMatrix = Matrix.CreateRotationX(MathHelper.PiOver2);
            addModel((model.Model.Meshes[0].Effects[0] as BasicEffect).Texture, model, BuildingType.LobbyDoorFrame);

            model = new StaticModel();
            model.Model = content.Load<Model>(@"Content\Lobby\exit");
            model.NormalMatrix = Matrix.CreateScale(1f);
            addModel((model.Model.Meshes[0].Effects[0] as BasicEffect).Texture, model, BuildingType.LobbyExit);

            model = new StaticModel();
            model.Model = content.Load<Model>(@"Content\Lobby\host");
            model.NormalMatrix = Matrix.CreateScale(1f);
            addModel((model.Model.Meshes[0].Effects[0] as BasicEffect).Texture, model, BuildingType.LobbyHost);

            model = new StaticModel();
            model.Model = content.Load<Model>(@"Content\Lobby\join");
            model.NormalMatrix = Matrix.CreateScale(1f);
            addModel((model.Model.Meshes[0].Effects[0] as BasicEffect).Texture, model, BuildingType.LobbyJoin);

            model = new StaticModel();
            model.Model = content.Load<Model>(@"Content\Lobby\play");
            model.NormalMatrix = Matrix.CreateScale(1f);
            addModel((model.Model.Meshes[0].Effects[0] as BasicEffect).Texture, model, BuildingType.LobbyPlay);

            door = new StaticModel();
            door.Model = content.Load<Model>(@"Content\Lobby\door");
            door.NormalMatrix = Matrix.CreateScale(1f);


            // Roads
            for (int i = 1; i <= 5; i++)
            {
                model = new StaticModel();
                model.Model = content.Load<Model>(@"Content\Models\road" + i);
                model.NormalMatrix = Matrix.CreateScale(0.05f);
                addModel((model.Model.Meshes[0].Effects[0] as BasicEffect).Texture, model, (BuildingType)(10 + i));
            }

            // Obstacles
            for (int i = 1; i <= 2; i++)
            {
                model = new StaticModel();
                model.Model = content.Load<Model>(@"Content\Models\box_" + (i == 1 ? "green_blue" : "red_blue"));
                model.NormalMatrix = Matrix.CreateScale(0.05f);
                addModel((model.Model.Meshes[0].Effects[0] as BasicEffect).Texture, model, (BuildingType)(20 + i));
            }

            //Bomb
            bomb = new StaticModel();
            bomb.Model = content.Load<Model>(@"Content\Models\bomb1");
            bomb.NormalMatrix = Matrix.CreateScale(0.07f) * Matrix.CreateTranslation(0, -6f, 0);

            // Explosions
            for (int i = 1; i <= 7; i++)
            {
                sprite  = new Sprite();
                sprite.Texture = content.Load<Texture2D>(@"Content\Special FX\explozija" + i);
                sprite.NormalMatrix = Matrix.CreateScale(30f);
                if (i == 3) sprite.NormalMatrix = Matrix.CreateScale(35f);
                if (i > 3) sprite.NormalMatrix = Matrix.CreateScale(25f);
                sprite.Origin = new Vector2(64, 64);
                if (i == 6) sprite.Origin = new Vector2(64, 96);
                explosionSprites.Add(sprite);
            }

            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Characters
            AnimatedModel animatedModel;

            animatedModel = new AnimatedModel();
            animatedModel.Model = content.Load<Model>(@"Content\Characters\vesolc");
            animatedModel.NormalMatrix = Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateScale(0.1f) * Matrix.CreateTranslation(0, 2.5f - Bomberman.GridSize * 0.5f, 0);
            animatedModel.Animation = "Idle";
            characters[(int)CharacterType.Alien] = animatedModel;

            animatedModel = new AnimatedModel();
            animatedModel.Model = content.Load<Model>(@"Content\Characters\buca");
            animatedModel.NormalMatrix = Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateScale(0.1f) * Matrix.CreateTranslation(0, 2.5f - Bomberman.GridSize * 0.5f, 0);
            animatedModel.Animation = "Idle";
            characters[(int)CharacterType.Pumpkin] = animatedModel;

            animatedModel = new AnimatedModel();
            animatedModel.Model = content.Load<Model>(@"Content\Characters\ant");
            animatedModel.NormalMatrix = Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateScale(0.07f) * Matrix.CreateTranslation(0, 2.5f - Bomberman.GridSize * 0.5f, 0);
            animatedModel.Animation = "Idle";
            characters[(int)CharacterType.Ant] = animatedModel;

            animatedModel = new AnimatedModel();
            animatedModel.Model = content.Load<Model>(@"Content\Characters\frog");
            animatedModel.NormalMatrix = Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateScale(-0.09f) * Matrix.CreateTranslation(0, 2.5f - Bomberman.GridSize * 0.5f, 0);
            animatedModel.Animation = "Idle";
            characters[(int)CharacterType.Frog] = animatedModel;

            animatedModel = new AnimatedModel();
            animatedModel.Model = content.Load<Model>(@"Content\Characters\robot");
            animatedModel.NormalMatrix = Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateScale(0.1f) * Matrix.CreateTranslation(0, 2.5f - Bomberman.GridSize * 0.5f, 0);
            animatedModel.Animation = "Idle";
            characters[(int)CharacterType.Robot] = animatedModel;

            animatedModel = new AnimatedModel();
            animatedModel.Model = content.Load<Model>(@"Content\Characters\teddy");
            animatedModel.NormalMatrix = Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateScale(-0.1f) * Matrix.CreateTranslation(0, 2.5f - Bomberman.GridSize * 0.5f, 0);
            animatedModel.Animation = "Idle";
            characters[(int)CharacterType.Teddy] = animatedModel;


            // Shader for static models
            staticModelsShader = new BasicEffect(GraphicsDevice, null);
            staticModelsShader.TextureEnabled = true;

            staticModelsShader.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            staticModelsShader.SpecularColor = new Vector3(0.2f, 0.2f, 0.2f);
            staticModelsShader.SpecularPower = 8f;

            staticModelsShader.LightingEnabled = true;
            staticModelsShader.AmbientLightColor = new Vector3(0.3f, 0.35f, 0.4f); ;            

            staticModelsShader.DirectionalLight0.Enabled = true;
            staticModelsShader.DirectionalLight0.SpecularColor = new Vector3(0.5f, 0.3f, 0.1f);
            staticModelsShader.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.8f, 0.7f);
            staticModelsShader.DirectionalLight0.Direction = new Vector3(-2, -3, -1);

            staticModelsShader.DirectionalLight1.Enabled = true;
            staticModelsShader.DirectionalLight1.SpecularColor = new Vector3(0.05f, 0.0f, 0.0f);
            staticModelsShader.DirectionalLight1.DiffuseColor = new Vector3(0.4f, 0.2f, 0.2f);
            staticModelsShader.DirectionalLight1.Direction = new Vector3(1, 0, 0);

            staticModelsShader.DirectionalLight2.Enabled = true;
            staticModelsShader.DirectionalLight2.SpecularColor = new Vector3(0.0f, 0.0f, 0.1f);
            staticModelsShader.DirectionalLight2.DiffuseColor = new Vector3(0.3f, 0.3f, 0.4f);
            staticModelsShader.DirectionalLight2.Direction = new Vector3(1, 0, 1);


            // Shader for animated models
            animatedModelsShader = content.Load<Effect>(@"Content\SkinnedModel");

            animatedModelsShader.Parameters["AmbientColor"].SetValue(staticModelsShader.AmbientLightColor);

            animatedModelsShader.Parameters["Light1Direction"].SetValue(staticModelsShader.DirectionalLight0.Direction);
            animatedModelsShader.Parameters["Light1Color"].SetValue(staticModelsShader.DirectionalLight0.DiffuseColor);

            animatedModelsShader.Parameters["Light2Direction"].SetValue(staticModelsShader.DirectionalLight1.Direction);
            animatedModelsShader.Parameters["Light2Color"].SetValue(staticModelsShader.DirectionalLight1.DiffuseColor);
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);
            sceneRendering = new Texture2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 1, ResourceUsage.ResolveTarget, SurfaceFormat.Color, ResourceManagementMode.Manual); ;
            explosionRendering = new Texture2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 1, ResourceUsage.ResolveTarget, SurfaceFormat.Color, ResourceManagementMode.Manual); ;
        }

        void addModel(Texture2D texture, StaticModel model, BuildingType type)
        {
            buildings[(int)type]= model;
            if (!textures.Contains(texture))
            {
                textures.Add(texture);
                models[texture] = new List<StaticModel>();
                worldMatrices[texture] = new Dictionary<StaticModel, List<Matrix>>();
            }
            textureMap.Add(model, texture);
            models[texture].Add(model);
            worldMatrices[texture].Add(model, new List<Matrix>());
        }

        // UPDATE
        public override void Update(GameTime gameTime)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update animations
            List<int> indices = level.Scene.GetItemIndices();
            for (int i = 0; i < indices.Count; i++)
            {
                Item sceneItem = level.Scene[indices[i]];
                if (sceneItem.Is<Character>())
                {
                    if (!characterInfo.ContainsKey(sceneItem))
                    {
                        characterInfo.Add(sceneItem, new CharacterRenderInfo(sceneItem.As<Character>(), characters[sceneItem.As<Character>().Type]));
                    }
                    CharacterRenderInfo info = characterInfo[sceneItem];
                    info.Update(gameTime);
                }
                if (sceneItem.Is<Explosion>())
                {
                    Explosion e = sceneItem.As<Explosion>();
                    if (e.Direction == Vector3.Zero)
                    {
                        if (e.Frame == 1)
                        {
                            Vector3 p = e.Part<PPosition>().Position + Vector3.Up * Bomberman.GridSize * 0.5f;
                            for (int j = 0; j < 3; j++)
                            {
                                float distance = 0.1f;
                                Vector3 o = new Vector3();
                                o.X += ((float)Bomberman.Random.NextDouble() - 0.5f) * Bomberman.GridSize * distance;
                                o.Z += ((float)Bomberman.Random.NextDouble() - 0.5f) * Bomberman.GridSize * distance;
                                o.Y += ((float)Bomberman.Random.NextDouble() - 0.5f) * Bomberman.GridSize * distance;
                                int s = Bomberman.Random.Next(2, 7);
                                //if (s == 3) s = 5;
                                explosions.AddLast(new ExplosionFX(explosionSprites[s], p + o));
                                shakePower++;
                            }
                        }
                    }
                    else
                    {
                        if (e.Frame == 1)
                        {
                            Vector3 p = e.Part<PPosition>().Position + Vector3.Up * Bomberman.GridSize * 0.5f - e.Direction * Bomberman.GridSize;
                            for (int j = 0; j < 6; j++)
                            {
                                p += j * e.Direction * Bomberman.GridSize * 0.1f;
                                float distance = 0.1f;
                                Vector3 o = new Vector3();
                                o.X += ((float)Bomberman.Random.NextDouble() - 0.5f) * Bomberman.GridSize * distance;
                                o.Z += ((float)Bomberman.Random.NextDouble() - 0.5f) * Bomberman.GridSize * distance;
                                o.Y += ((float)Bomberman.Random.NextDouble() - 0.5f) * Bomberman.GridSize * distance;
                                int s = Bomberman.Random.Next(0, 2);
                                explosions.AddLast(new ExplosionFX(explosionSprites[s], p + o));
                            }
                        }
                    }
                }
            }

            // Update camera shake
            cameraShake += new Vector3(Bomberman.Random.Next(-10, 10), Bomberman.Random.Next(-10, 10), Bomberman.Random.Next(-10, 10)) * 0.1f * shakeSpeed * elapsed;
            cameraShake *= 0.5f;
            shakePower *= 0.9f;

        }


        // DRAW
        public override void Draw(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedRealTime.TotalSeconds;

            Texture2D texture;
            for (int j = 0; j < textures.Count; j++)
            {
                texture = textures[j];
                for (int i = 0; i < models[texture].Count; i++)
                {
                    worldMatrices[texture][models[texture][i]].Clear();
                }
            }

            // Build visible item lists
            StaticModel model;
            List<int> indices = level.Scene.GetItemIndices();
            for (int i = 0; i < indices.Count; i++)
            {
                Item sceneItem = level.Scene[indices[i]];
                if (sceneItem.Is<Building>())
                {
                    model = buildings[((Building)sceneItem).Type];
                    if (model != null)
                    {
                        worldMatrices[textureMap[model]][model].Add(sceneItem.Part<PWorldMatrix>().WorldMatrix);
                    }
                }
            }

            // Extra ambient
            int numExplosions = 0;
            float extraYellow = 0;
            explosionNode = explosions.First;
            while (explosionNode != null)
            {
                numExplosions++;
                extraYellow += (float)Math.Pow(0.95, numExplosions) * explosionNode.Value.Intensity;
                explosionNode = explosionNode.Next;
            }
            extraYellow *= 0.02f;

            staticModelsShader.AmbientLightColor = new Vector3(0.1f + extraYellow, 0.15f + extraYellow, 0.2f);
            animatedModelsShader.Parameters["AmbientColor"].SetValue(staticModelsShader.AmbientLightColor);


            // Camera shaking
            cameraTarget = level.Camera.Target;
            level.Camera.Target = cameraTarget + cameraShake * shakePower;


            // Prepare for rendering
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.RenderState.DepthBufferEnable = true;
            GraphicsDevice.RenderState.DepthBufferWriteEnable = true;


            // Render static models
            staticModelsShader.View = level.Camera.ViewMatrix;
            staticModelsShader.Projection = level.Camera.ProjectionMatrix;
            StaticModel.Render(textures, models, worldMatrices, staticModelsShader);

            // Render items
            for (int i = 0; i < indices.Count; i++)
            {
                Item sceneItem = level.Scene[indices[i]];
                if (sceneItem.Is<Bomb>())
                {
                    bomb.Render(Matrix.CreateTranslation(sceneItem.Part<PPosition>().Position), staticModelsShader);
                }
                if (sceneItem.Is<Door>())
                {
                    door.Render(sceneItem.Part<PRotationMatrix>().Rotation * Matrix.CreateTranslation(sceneItem.Part<PPosition>().Position), staticModelsShader);
                }
            }

            // Render animated models
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            animatedModelsShader.Parameters["View"].SetValue(level.Camera.ViewMatrix);
            animatedModelsShader.Parameters["Projection"].SetValue(level.Camera.ProjectionMatrix);
            for (int i = 0; i < indices.Count; i++)
            {
                Item sceneItem = level.Scene[indices[i]];
                if (sceneItem.Is<Character>())
                {
                    Character c = sceneItem.As<Character>();
                    if (!c.IsDead)
                    {
                        CharacterRenderInfo info = characterInfo[sceneItem];
                        float updateTime = elapsed;
                        if (characters[sceneItem.As<Character>().Type].Animation == "Walk")
                        {
                            updateTime *= info.Speed * walkSpeed;
                        }
                        info.Model.Render(TimeSpan.FromSeconds(updateTime), info.WorldMatrix, animatedModelsShader);
                        FontTransformer.Write(c.Player.Name, font, spriteBatch, 0.25f, info.WorldMatrix * Matrix.CreateTranslation(0, -5, 0), level.Camera, playerColor, Vector2.One);
                    }
                }
            }
            spriteBatch.End();


            // Render explosions

            // Save the currect buffer
            GraphicsDevice.ResolveBackBuffer(sceneRendering);

            // Start rendering the explosion texture
            GraphicsDevice.Clear(ClearOptions.Target, Color.TransparentBlack, 0, 0);

            // Draw explosions
            if (explosions.Count > 0)
            {
                LinkedListNode<ExplosionFX> tmpNode;
                spriteBatch.Begin(SpriteBlendMode.Additive, SpriteSortMode.Immediate, SaveStateMode.SaveState);

                GraphicsDevice.RenderState.DepthBufferEnable = true;
                GraphicsDevice.RenderState.DepthBufferWriteEnable = false;

                explosionNode = explosions.First;
                while (explosionNode != null)
                {
                    explosionNode.Value.Render(gameTime, spriteBatch);
                    if (explosionNode.Value.Ended)
                    {
                        tmpNode = explosionNode.Next;
                        explosions.Remove(explosionNode);
                        explosionNode = tmpNode;
                    }
                    else
                    {
                        explosionNode = explosionNode.Next;
                    }
                }

                spriteBatch.End();
            }

            // Explosion texture is finished. Save it to texture
            GraphicsDevice.ResolveBackBuffer(explosionRendering);

            level.Camera.Target = cameraTarget;

            // Render final composition
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);

            // Draw the scene
            spriteBatch.Draw(sceneRendering, Vector2.Zero, Color.White);
            // Draw xplosions on top
            spriteBatch.Draw(explosionRendering, Vector2.Zero, Color.White);
            // Draw the GUI            
            //string time = ((int)Bomberman.TimeLeft).ToString();
            //if (Bomberman.TimeLeft <= 0) time = "0";
            //if (Bomberman.TimeLeft < 10) time = "0" + time;
            //font.DrawCentered(time, new Vector2(GraphicsDevice.Viewport.Width * 0.5f, 30), 1, Color.White, spriteBatch);

            spriteBatch.End();



        }        
    }
}
