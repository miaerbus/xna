using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;
using Artificial.XNATutorial.Physics;

namespace Artificial.XNATutorial.Bomberman
{
    public class Lobby : Part
    {
        // Level
        public Level Level;
        State state;

        // Gameplay component
        int updateOrder;

        // Renderer
        WorldRenderer renderer;
        PhysicsRenderer physicsRenderer;
        ABTRenderer abtRenderer;

        // Simulator
        Simulator simulator;

        // CONSTRUCTOR
        public Lobby(int updateOrder)
        {
            this.updateOrder = updateOrder;
            state = Require<State>();
        }


        // INSTALL
        public override void Install(Item parent)
        {
            base.Install(parent);
            parent.As<Simulator>().RegisterUpdateMethod(updateOrder, Update);

            // Create scene
            Level = LobbyGenerator.Generate(Exit, StartGame);
            Bomberman.Level = Level;

            float placeX = ((Level.Size - Bomberman.ActivePlayers.Count) * 0.5f + 1) * Bomberman.GridSize;
            float deltaX = Bomberman.GridSize;
            float placeZ = ((float)Level.Size * 0.5f + 3) * Bomberman.GridSize;
            for (int i = 0; i < Bomberman.ActivePlayers.Count; i++)
            {
                Bomberman.ActivePlayers[i].Controller.ActivateGameControls();
                Bomberman.ActivePlayers[i].Character.Reset();
                Bomberman.ActivePlayers[i].Character.BombCount = 0;
                Bomberman.ActivePlayers[i].Character.Part<PPosition>().Position = new Vector3(placeX, 0, placeZ);
                Bomberman.ActivePlayers[i].Character.Part<PAngularPosition>().AngularPosition = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2);
                placeX += deltaX;
                Level.AddCharacter(Bomberman.ActivePlayers[i].Character);
            }

            // Prepare simulation engine
            simulator = new Simulator(4);
            simulator.Scene = Level.Scene;
            simulator.Aggregate(new SimpleSceneIterator(50));
            simulator.Aggregate(new ItemProcessStarter(10));
            simulator.Aggregate(new ClassicalMechanics(20));
            simulator.Aggregate(new CollisionDetector(30));
            simulator.Aggregate(new ParticleParticleCollisions(0));
            simulator.Aggregate(new ParticleConvexCollisions(1));
            simulator.Aggregate(new ConstantsApplier(40, 15));

            // Create main renderer
            renderer = Bomberman.WorldRenderer;
            renderer.level = Level;
            renderer.DrawOrder = 0;
            Bomberman.Game.Components.Add(renderer);

            // Create debug renderers
            abtRenderer = new ABTRenderer(Bomberman.Game, simulator.Part<SceneABT>().ABT);
            abtRenderer.DrawOrder = 1;
            abtRenderer.RenderVolumes = true;
            abtRenderer.Camera = Level.Camera;
            //Bomberman.Game.Components.Add(abtRenderer);

            physicsRenderer = new PhysicsRenderer(Bomberman.Game, Level.Scene, simulator);
            physicsRenderer.DrawOrder = 2;
            physicsRenderer.Camera = Level.Camera;
            physicsRenderer.polyEdgeColor = Color.Orange;
            physicsRenderer.particleRadiusColor = Color.Orange;
            //Bomberman.Game.Components.Add(physicsRenderer);
        }

        public override void Uninstall()
        {
            parent.As<Simulator>().UnregisterUpdateMethod(Update);
            Bomberman.Game.Components.Remove(renderer);
        }

        void Update(float dt)
        {
            for (int i = 0; i < Bomberman.ActivePlayers.Count; i++)
            {
                Bomberman.ActivePlayers[i].Controller.Update(dt);
            }

            // Simulate game
            simulator.Simulate(dt);
        }

        void StartGame()
        {
            state.NextState = Bomberman.States.OnlineGameplay;
            state.Finished = true;
        }

        void Exit()
        {
            state.NextState = Bomberman.States.MainMenu;
            state.Finished = true;
        }
    }
}
