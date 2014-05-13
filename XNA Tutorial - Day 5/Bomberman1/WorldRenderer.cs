using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial.Bomberman
{
    class WorldRenderer : DrawableGameComponent
    {
        ContentManager content;        
        SpriteBatch spriteBatch;
        float walkSpeed = 0.04f;

        // Static models
        Dictionary<BuildingType, StaticModel> buildings = new Dictionary<BuildingType, StaticModel>();
        StaticModel bomb;
        BasicEffect staticModelsShader;

        // Animated models
        Dictionary<CharacterType, AnimatedModel> characters = new Dictionary<CharacterType, AnimatedModel>();
        Effect animatedModelsShader;

        // Explosions
        List<Sprite> explosionSprites = new List<Sprite>();
        SpriteBatch explosionBatch;
        Texture2D sceneRendering;
        Texture2D explosionRendering;

        // Rendering collections
        List<Texture2D> textures = new List<Texture2D>();
        Dictionary<Texture2D, List<StaticModel>> models = new Dictionary<Texture2D, List<StaticModel>>();
        Dictionary<Texture2D, Dictionary<StaticModel, List<Matrix>>> worldMatrices = new Dictionary<Texture2D, Dictionary<StaticModel, List<Matrix>>>();
        Dictionary<StaticModel, Texture2D> textureMap = new Dictionary<StaticModel, Texture2D>();
        Dictionary<Item, CharacterRenderInfo> characterInfo = new Dictionary<Item, CharacterRenderInfo>();
        LinkedList<ExplosionFX> explosions = new LinkedList<ExplosionFX>();
        LinkedListNode<ExplosionFX> explosionNode;

        // CONSTRUCTOR
        public WorldRenderer(Game game)
            : base(game)
        {
        }

        // INITIALIZE
        public override void Initialize()
        {
            base.Initialize();

            content = new ContentManager(Game.Services);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            StaticModel model;
            Sprite sprite;

            // Buildings
            for (int i = 1; i <= 9; i++)
            {
                model = new StaticModel();
                model.Model = content.Load<Model>(@"Content\Models\building" + i);               
                model.NormalMatrix = Matrix.CreateScale(0.05f);
                addModel((model.Model.Meshes[0].Effects[0] as BasicEffect).Texture,model, (BuildingType)i);
            }


            // Roads
            for (int i = 1; i <= 5; i++)
            {
                model = new StaticModel();
                model.Model = content.Load<Model>(@"Content\Models\road" + i);
                model.NormalMatrix = Matrix.CreateScale(0.05f);
                addModel((model.Model.Meshes[0].Effects[0] as BasicEffect).Texture, model, (BuildingType)(100 + i));
            }

            // Obstacles
            for (int i = 1; i <= 2; i++)
            {
                model = new StaticModel();
                model.Model = content.Load<Model>(@"Content\Models\box_" + (i == 1 ? "green_blue" : "red_blue"));
                model.NormalMatrix = Matrix.CreateScale(0.05f);
                addModel((model.Model.Meshes[0].Effects[0] as BasicEffect).Texture, model, (BuildingType)(200 + i));
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

            explosionBatch = new SpriteBatch(GraphicsDevice);

            // Characters
            AnimatedModel alien = new AnimatedModel();
            alien.Model = content.Load<Model>(@"Content\Characters\vesolc");
            alien.NormalMatrix = Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateScale(0.1f) * Matrix.CreateTranslation(0, 2.5f - Bomberman.GridSize * 0.5f, 0);
            alien.Animation = "Idle";
            characters.Add(CharacterType.Alien, alien);

            AnimatedModel pumpkin = new AnimatedModel();
            pumpkin.Model = content.Load<Model>(@"Content\Characters\buca");
            pumpkin.NormalMatrix = Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateScale(0.1f) * Matrix.CreateTranslation(0, 2.5f - Bomberman.GridSize * 0.5f, 0);
            pumpkin.Animation = "Idle";
            characters.Add(CharacterType.Pumpkin, pumpkin);


            // Shader for static models
            staticModelsShader = new BasicEffect(GraphicsDevice, null);
            staticModelsShader.TextureEnabled = true;

            staticModelsShader.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            staticModelsShader.SpecularColor = new Vector3(0.2f, 0.2f, 0.2f);
            staticModelsShader.SpecularPower = 8f;

            staticModelsShader.LightingEnabled = true;
            staticModelsShader.AmbientLightColor = new Vector3(0.2f, 0.25f, 0.3f); ;            

            staticModelsShader.DirectionalLight0.Enabled = true;
            staticModelsShader.DirectionalLight0.SpecularColor = new Vector3(0.5f, 0.3f, 0.1f);
            staticModelsShader.DirectionalLight0.DiffuseColor = new Vector3(0.9f, 0.8f, 0.7f);
            staticModelsShader.DirectionalLight0.Direction = new Vector3(-1, -3, -2);

            staticModelsShader.DirectionalLight1.Enabled = true;
            staticModelsShader.DirectionalLight1.SpecularColor = new Vector3(0.05f, 0.0f, 0.0f);
            staticModelsShader.DirectionalLight1.DiffuseColor = new Vector3(0.2f, 0.0f, 0.0f);
            staticModelsShader.DirectionalLight1.Direction = new Vector3(0, 0, 1);


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
            buildings.Add(type, model);
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
            base.Update(gameTime);

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update animations
            List<int> indices = Bomberman.CurrentLevel.Scene.GetItemIndices();
            for (int i = 0; i < indices.Count; i++)
            {
                Item sceneItem = Bomberman.CurrentLevel.Scene[indices[i]];
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
        }


        // DRAW
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

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
            List<int> indices = Bomberman.CurrentLevel.Scene.GetItemIndices();
            for (int i = 0; i < indices.Count; i++)
            {
                Item sceneItem = Bomberman.CurrentLevel.Scene[indices[i]];
                if (sceneItem.Is<Building>())
                {
                    model = buildings[((Building)sceneItem).Type];
                    worldMatrices[textureMap[model]][model].Add(sceneItem.Part<PWorldMatrix>().WorldMatrix);
                }
            }


            // Prepare for rendering
            GraphicsDevice.Clear(Color.DimGray);
            GraphicsDevice.RenderState.DepthBufferEnable = true;
            GraphicsDevice.RenderState.DepthBufferWriteEnable = true;


            // Render static models
            staticModelsShader.View = Bomberman.CurrentLevel.Camera.ViewMatrix;
            staticModelsShader.Projection = Bomberman.CurrentLevel.Camera.ProjectionMatrix;
            StaticModel.Render(textures, models, worldMatrices, staticModelsShader);

            // Render bombs
            for (int i = 0; i < indices.Count; i++)
            {
                Item sceneItem = Bomberman.CurrentLevel.Scene[indices[i]];
                if (sceneItem.Is<Bomb>())
                {                    
                    bomb.Render(Matrix.CreateTranslation(sceneItem.Part<PPosition>().Position), staticModelsShader);
                }
            }

            // Render animated models
            animatedModelsShader.Parameters["View"].SetValue(Bomberman.CurrentLevel.Camera.ViewMatrix);
            animatedModelsShader.Parameters["Projection"].SetValue(Bomberman.CurrentLevel.Camera.ProjectionMatrix);
            for (int i = 0; i < indices.Count; i++)
            {
                Item sceneItem = Bomberman.CurrentLevel.Scene[indices[i]];
                if (sceneItem.Is<Character>())
                {
                    CharacterRenderInfo info = characterInfo[sceneItem];
                    float updateTime = elapsed;
                    if (characters[sceneItem.As<Character>().Type].Animation == "Walk")
                    {
                        updateTime *= info.Speed * walkSpeed;
                    }
                    characters[sceneItem.As<Character>().Type].Render(TimeSpan.FromSeconds(updateTime), info.WorldMatrix, animatedModelsShader);
                }
            }


            // Render explosions

            // Save the currect buffer
            GraphicsDevice.ResolveBackBuffer(sceneRendering);

            // Start rendering the explosion texture
            GraphicsDevice.Clear(ClearOptions.Target, Color.TransparentBlack, 0, 0);

            // Draw explosions
            if (explosions.Count > 0)
            {
                LinkedListNode<ExplosionFX> tmpNode;
                explosionBatch.Begin(SpriteBlendMode.Additive, SpriteSortMode.Immediate, SaveStateMode.SaveState);

                GraphicsDevice.RenderState.DepthBufferEnable = true;
                GraphicsDevice.RenderState.DepthBufferWriteEnable = false;

                explosionNode = explosions.First;
                while (explosionNode != null)
                {
                    explosionNode.Value.Render(gameTime, explosionBatch);
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

                explosionBatch.End();
            }

            // Explosion texture is finished. Save it to texture
            GraphicsDevice.ResolveBackBuffer(explosionRendering);


            // Draw the scene and explosions on top
            explosionBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            explosionBatch.Draw(sceneRendering, Vector2.Zero, Color.White);
            explosionBatch.Draw(explosionRendering, Vector2.Zero, Color.White);
            explosionBatch.End();

        }        
    }
}
