
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
    /// XNA TUTORIAL - BASICS PART 1
    /// 
    /// Explains basic options available to configure the graphics device and initialize the game.
    /// </summary>
    public class Basics1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics; // Configures and creates the graphics device.
        ContentManager content; // Loads content files added to the project.

        /// <summary>
        /// GAME CONSTRUCTOR
        /// 
        /// Called when you create a new instance of this game class. It is the first thing
        /// that gets executed in this class, before the graphics device is created. So it
        /// is the place where you tell the graphics device manager, what kind of device
        /// (resolution, bit depth, multisampling ... ) it should create.
        /// </summary>
        public Basics1()
        {
            // Create the graphics manager
            graphics = new GraphicsDeviceManager(this);

            // Confifure the device    
            graphics.PreferredBackBufferWidth = 800; // Width of the back buffer
            graphics.PreferredBackBufferHeight = 600; // Height of the back buffer
            
            graphics.IsFullScreen = false; // Run in fullscreen or windowed
           
            graphics.PreferMultiSampling = false; // Use antialliasing or not.
            
            graphics.SynchronizeWithVerticalRetrace = true; // Redraw at the monitor refresh frequency or not.
                                                            // If this is false, the game will render as many
                                                            // times as possible, using 100% of the CPU.
            
            graphics.PreferredBackBufferFormat = SurfaceFormat.Color; // Back buffer format
            graphics.PreferredDepthStencilFormat = DepthFormat.Depth24; // Depth and stencil format            
            
            graphics.MinimumVertexShaderProfile = ShaderProfile.VS_1_1; // Minimum vertex shader requirements
            graphics.MinimumPixelShaderProfile = ShaderProfile.PS_1_1; // Minimum pixel shader requirements
            
            // Create the content manager
            content = new ContentManager(Services);
       }


        /// <summary>
        /// INITIALIZE
        /// 
        /// When the Game.Run() method is called (the template does this from Program.cs file) it first
        /// creates a graphics device that suites the settings we specified. It is possible that such
        /// device is impossible to create (for example specifying Pixel Shader 3.0 and running on a
        /// graphics card that supports only up to 2.0) which will result in an error. If all goes OK
        /// and the device is successfully created, initialize is called. So this is the place to
        /// initialize all starting values for running your game, except loading the content.
        /// </summary>
        protected override void Initialize()
        {

            // Configure the way the game runs
            this.IsFixedTimeStep = true; // Tells if the game should update at constant frame rate or at
                                         // maximum speed. This only controls how often update is called
                                         // as the game will still use render as often as possible if it
                                         // is not synchronized with vertical retrace. If the game doesn't
                                         // have a fixed time step, update and draw will be called in
                                         // sequence as fast as possible.

            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0); // If the frame rate is constant,
                                                                       // set the elapsed time
      
            this.IsMouseVisible = false; // Display the mouse cursor in windows or not

            this.InactiveSleepTime = TimeSpan.FromMilliseconds(500); // If the game becomes inactive (user
                                                                     // switches to another window), how
                                                                     // often to check if it has become
                                                                     // active again

            this.Window.Title = "XNA Tutorial - Basics Part 1"; // Title of the game window

            this.Window.AllowUserResizing = false; // Allows the user to resize the game window
            
            
            // When base.Initialize() is called, all components added to GameComponents collection
            // are initialized as well. So call this at the end of Initialize, when you've already
            // created and added all the components.
            // In addition, LoadGraphicsContent will be called.
            base.Initialize();
        }


        /// <summary>
        /// LOAD GRAPHICS CONTENT
        /// 
        /// This is called when the Game class initializes and every time a graphics device is recreated.
        /// When the device resets (if you change the game resolution for example) all the content data
        /// (textures, models) that were created with the device get lost and need to be reloaded.
        /// 
        /// XNA automatically reloads all the content that is placed in the automatic resource management
        /// mode, which is true for most of the content. However, if you created any content with the
        /// ResourceManagementMode.Manual flag, you have to manualy reload it from here.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                // Here we load all content added to the project from the solution explorer

                // Nothing to load yet
            }

            // We don't have any manual content
        }


        /// <summary>
        /// UNLOAD GRAPHICS CONTENT
        /// 
        /// This will be called before the Game object is disposed at the end of the game lifetime.
        /// In addition it is called when the device is being reset so you can clean any content in
        /// the manual resource pool or do any other things before the game changes to a new device.        
        /// Note that when unloadAllContent is true (when the game is exiting), the device is
        /// already disposed at this point.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent == true)
            {
                //Game is exiting so unload all content still in memory
                content.Unload();
            }
        }


        /// <summary>
        /// UPDATE
        /// 
        /// This is where you should create all the changes to the game objects that appeared in
        /// the current frame. The gameTime provides the time elapsed since last update and this
        /// value is usually used in all physics or animation calculations. If the game is set
        /// to fixed time step this time is guaranteed to always be the same (exactly as set).
        /// 
        /// The update usualy includes this sequence of actions:
        /// - Get input from keyboard / mouse / gamepad
        /// - Acording to the game state, transform this input into appropriate game actions
        /// - Artificial intelligence provides game actions for AI controlled objects
        /// - All actions are applied, changing the world before the simulating step
        /// - Game simulates the changes according to the game laws (physics, rules ... )
        /// - The new world state is analyzed to see if any (winning) conditions are met
        /// - If they are, the game state changes (game ends, menu shows ... )
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the default game to exit on Xbox 360 and Windows. The gamepad state is
            // only relevant if you use the gamepad, so for all those with only keyboards on
            // windows we add the escape key as well. On windows you can exit also if you
            // close the windows (click on the red X or ALT+F4 in fullscreen).
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            // TODO: Add your update logic here

            // Nothing to do yet

            // In the base update all updatable game components are also updated in the
            // priority order specified by assigning priorities to game components.
            base.Update(gameTime);
        }


        /// <summary>
        /// DRAW
        /// 
        /// This is called when the game should draw a new image that will be presented to the
        /// screen. Before the Draw is called, the game sets the back buffer to be the current
        /// render target as well as the depth buffer. If you want to do something before this
        /// happens you can ovveride the BeginDraw method of the game class.
        /// 
        /// After the Draw finishes, EndDraw is called which can again be overrided. In EndDraw,
        /// the game presents the back buffer to the screen 
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // If I see CornflowerBlue again I'm going to throw up. Use emo colors instead.
            // In this first part, this is the only visible difference from the default
            // XNA Windows game template.
            graphics.GraphicsDevice.Clear(Color.Gray);

            // TODO: Add your drawing code here

            // See next part for actually rendering something

            // Similar to the base update, inn the base draw, all drawable game components are
            // drawn in the assigned order.
            base.Draw(gameTime);
        }
    }
}