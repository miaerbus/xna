using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Artificial.XNATutorial;

namespace Artificial.XNATutorial
{
    public class SceneABT : Part
    {
        AdaptiveBinaryTree abt;
        public AdaptiveBinaryTree ABT
        {
            get
            {
                return abt;
            }
        }
        Dictionary<Item, IABTItem> ABTMap = new Dictionary<Item, IABTItem>();
        List<ABTLinkedList> filteredItems = new List<ABTLinkedList>();

        // CONSTRUCTOR
        public override void Install(Item parent)
        {
            Scene scene = parent.As<Scene>();
            if (scene == null) throw new Exception("Parent requires a scene");

            // Add scene handlers
            scene.OnItemAdd += sceneItemAdd;
            scene.OnItemRemove += sceneItemRemoved;

            // Create ABTs
            abt = new AdaptiveBinaryTree(8);
        }


        // SCENE ITEM ADDED
        void sceneItemAdd(object sender, Scene.SceneManipulationEventArgs e)
        {
            // Update sceneABT
            Item item = e.Item;
            if (item != null)
            {
                PPositionWithEvents itemPosition = item.As<PPositionWithEvents>();
                PParticleRadius itemParticleRadius = item.As<PParticleRadius>();
                PBoundingSphere itemBoundingSphere = item.As<PBoundingSphere>();

                if (itemPosition != null)
                {
                    CustomBoundingBoxABTItem i = new CustomBoundingBoxABTItem();
                    i.PositionPart = item;
                    if (itemParticleRadius != null)
                    {
                        i.BoundingBox = new BoundingBox(itemParticleRadius.ParticleRadius * -Vector3.One, itemParticleRadius.ParticleRadius * Vector3.One);
                    }
                    else if (itemBoundingSphere != null)
                    {
                        i.BoundingBox = BoundingBox.CreateFromSphere(itemBoundingSphere.BoundingSphere);
                    }
                    else
                    {
                        i.BoundingBox = new BoundingBox(-Vector3.One, Vector3.One);
                    }
                    abt.Add(i);
                    ABTMap.Add(item, i);
                }
            }
        }

        // SCENE ITEM REMOVED
        void sceneItemRemoved(object sender, Scene.SceneManipulationEventArgs e)
        {
            // Update sceneABT
            if (ABTMap.ContainsKey(e.Item))
            {
                IABTItem item = ABTMap[e.Item];
                if (item != null)
                {
                    abt.Remove(item);
                }
            }
        }
    }
}
