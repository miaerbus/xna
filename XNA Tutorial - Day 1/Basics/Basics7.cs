
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Artificial.XNATutorial.Basics
{
    /// <summary>
    /// XNA TUTORIAL - BASICS PART 7
    /// 
    /// We change from 2D approach to full 3D. XNA uses a right hand coordinate system so because
    /// we'll still be changing the X and Y coordinates (but with Y not being reversed anymore - so 
    /// positive Y points up in our top-down view approach), that plane represents our ground.
    /// The new Z coordinate will point into the sky (positive Z is backwards - out of the monitor).
    /// 
    /// As you can see this is getting a little more complicated so it's also the end of the basics.
    /// </summary>
    public class Basics7 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;
        GraphicsDevice device;
        SpriteBatch spriteBatch;
        Texture2D particleTexture;

        float speed = 300;

        // PARTICLE
        class Particle
        {
            public Vector3 Position; // Position is now in 3D
            public Color Color;

            public Particle(Vector3 position, Color color)
            {
                Position = position;
                Color = color;
            }
        }

        List<Particle> particles = new List<Particle>();
        Particle playerParticle;

        // Camera variables
        Vector3 cameraTarget;
        Vector3 cameraPosition = new Vector3(0, 0, 2000); // This is where the camera looks from
        float cameraSpeed = 2000;
        float cameraFOV = (float)Math.PI / 3f; // Zoom is actually achived by changing the field of view
                                               // angle. Units are radians (PI radians = 180 degrees)
        float zoomSpeed = 1;

        // GAME CONSTRUCTOR
        public Basics7()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = false;
        }


        // INITIALIZE
        protected override void Initialize()
        {
            device = graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(device);
            content = new ContentManager(Services);

            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;
            this.Window.Title = "XNA Tutorial - Basics Part 7";
            this.Window.AllowUserResizing = true;

            playerParticle = new Particle(Vector3.Zero, Color.Gold);
            particles.Add(playerParticle);

            Random random = new Random();
            for (int i = 0; i < 50; i++)
            {
                particles.Add(new Particle(new Vector3(
                    random.Next(-1000, 1000),
                    random.Next(-1000, 1000), // We have three coordinates this time around and we make
                    random.Next(-1000, 1000)) // the particles be extending into the sky
                    , Color.White));
            }

            base.Initialize();
        }


        // LOAD GRAPHICS CONTENT
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                particleTexture = content.Load<Texture2D>("particle");
            }
        }


        // UNLOAD GRAPHICS CONTENT
        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent == true)
            {
                content.Unload();
            }
        }


        // UPDATE
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState keyState = Keyboard.GetState();
            // Vector3 includes some nice constants for us to use. Note that we can now use
            // positive Y direction for up, since we will transform the coordinates into
            // screen space at the end.
            if (keyState.IsKeyDown(Keys.Left))
            {
                playerParticle.Position += Vector3.Left * elapsed * speed;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                playerParticle.Position += Vector3.Right * elapsed * speed;
            }
            if (keyState.IsKeyDown(Keys.Up))
            {
                playerParticle.Position += Vector3.Up * elapsed * speed;
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                playerParticle.Position += Vector3.Down * elapsed * speed;
            }
            // We make the camera track the player ...
            cameraTarget = playerParticle.Position;
            // ... and move from where we're looking onto the action
            if (keyState.IsKeyDown(Keys.A))
            {
                cameraPosition += Vector3.Left * elapsed * cameraSpeed;
            }
            if (keyState.IsKeyDown(Keys.D))
            {
                cameraPosition += Vector3.Right * elapsed * cameraSpeed;
            }
            if (keyState.IsKeyDown(Keys.W))
            {
                cameraPosition += Vector3.Up * elapsed * cameraSpeed;
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                cameraPosition += Vector3.Down * elapsed * cameraSpeed;
            }
            // Instead of zoom we're changin the FOV of the camera
            if (keyState.IsKeyDown(Keys.R))
            {
                cameraFOV += cameraFOV * elapsed * zoomSpeed;
            }
            if (keyState.IsKeyDown(Keys.F))
            {
                cameraFOV -= cameraFOV * elapsed * zoomSpeed;
            }

            GamePadState padState = GamePad.GetState(PlayerIndex.One);
            // Some adjustments as we go into 3D
            playerParticle.Position +=
                new Vector3(padState.ThumbSticks.Left * new Vector2(1, 1) * elapsed * speed, 0);
            cameraPosition +=
                new Vector3(padState.ThumbSticks.Right * new Vector2(1, 1) * elapsed * cameraSpeed, 0);
            cameraFOV += cameraFOV * padState.Triggers.Right * elapsed * zoomSpeed;
            cameraFOV -= cameraFOV * padState.Triggers.Left * elapsed * zoomSpeed;

            base.Update(gameTime);
        }


        // DRAW
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None);

            // We let XNA create the matrices from now on. The view matrix will transform the absolute
            // world coordinates into the space relative to the camera position.
            Matrix view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
            // Projection matrix will transform the relative world coordinates to projection space,
            // where the screen bounds go from -1 to 1 at the edges of our field of view and the depth
            // (Z coordinate in this new space) is 0 at object at near plane and 1 at the far plane.
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(
                cameraFOV, // Field of view angle, describing how wide is the cone projected
                           // out of the cameraPosition.

                (float)device.Viewport.Width / (float)device.Viewport.Height, // The aspect ratio of our 
                                                                              // view, because our view isn't
                                                                              // square, so the horizontal
                                                                              // and vertical FOV aren't
                                                                              // the same.

                10, // The near plane of the view. We don't draw things nearer than this.
                10000); // The far plane of our view. We also don't draw things farther than this.              

            // In usual 3D rendering we would already have all the transforms. But because the sprite
            // batch excepts coordinates in screen pixels, we need to transform our -1 to 1 view into
            // 0 to 800/600 values (using viewport variables instead of hardcoded values this time)
            Matrix screen = Matrix.CreateTranslation(1, -1, 0) * // Move the left-bottom corner to zero
                Matrix.CreateScale(device.Viewport.Width / 2,
                -device.Viewport.Height / 2, // Inverse Y to make positive Y point down as is the case in
                                             // screen coordinates
                1); // Leave the depth the same

            // We also need to specify the size of the sprite in 3D world units.
            float size = 256;

            for (int i = 0; i < particles.Count; i++)
            {
                // First transform the position from world space to view space with the view matrix
                // We'll need this intermediate step later, when we calculate the scale
                Vector3 viewposition = Vector3.Transform(particles[i].Position, view);
                // We transform from view space to screen space. Because we're applying a projection
                // matrix we need to do the transform in 4D homogenous coordinates.
                Vector4 position4D = Vector4.Transform(viewposition, projection * screen);
                // We now divide by the fourth coordinate to come back to W=1 space
                position4D /= position4D.W;
                // Use X and Y for the sprite position
                Vector2 position = new Vector2(position4D.X, position4D.Y);
                // And the Z for the depth value
                float depth = position4D.Z;

                // In the view space we add the size of our sprite in world units to the previouslly 
                // calculated position of our sprite. We transform this into screen space to see what
                // is the size of the sprite in pixels. Scale is then the ratio between the desired size
                // and the size of the texture.
                position4D = Vector4.Transform(viewposition + Vector3.UnitY * size, projection * screen);
                position4D /= position4D.W;
                Vector2 position2 = new Vector2(position4D.X, position4D.Y);
                float scale = (position2 - position).Length() / 256f;
                                
                spriteBatch.Draw(particleTexture, position, new Rectangle(0, 0, 256, 256),
                    particles[i].Color, 0, new Vector2(128, 128), scale, SpriteEffects.None, depth);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}