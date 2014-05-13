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
        Scene scene;

        // CREATE
        public void Create(int itemsPerLeaf)
        {
            // Remove scene handlers
            if (scene != null)
            {
                scene.OnItemAdd -= sceneItemAdd;
                scene.OnItemRemove -= sceneItemRemoved;
            }

            scene = parent.As<Scene>();

            // Add scene handlers
            scene.OnItemAdd += sceneItemAdd;
            scene.OnItemRemove += sceneItemRemoved;

            // Create ABTs
            abt = new AdaptiveBinaryTree(itemsPerLeaf);

            // Add current items
            List<int> indices = scene.GetItemIndices();
            for (int i = 0; i < indices.Count; i++)
            {
                itemAdd(scene[indices[i]], false);
            }
            abt.ForceSubdivide();
        }

        // SCENE ITEM ADDED
        void sceneItemAdd(object sender, Scene.SceneManipulationEventArgs e)
        {
            // Update sceneABT
            itemAdd(e.Item,true);
        }

        void itemAdd(Item item, bool subdivide)
        {
            PPositionWithEvents itemPosition = item.As<PPositionWithEvents>();
            PParticleRadius itemParticleRadius = item.As<PParticleRadius>();
            PBoundingSphere itemBoundingSphere = item.As<PBoundingSphere>();
            PBoundingBox itemBoundingBox = item.As<PBoundingBox>();

            if (itemPosition != null)
            {
                CustomBoundingBoxABTItem i = new CustomBoundingBoxABTItem();
                i.PositionPart = item;
                if (itemParticleRadius != null)
                {
                    i.BoundingBox = new BoundingBox(itemParticleRadius.ParticleRadius * -Vector3.One, itemParticleRadius.ParticleRadius * Vector3.One);
                }
                else if (itemBoundingBox != null)
                {
                    i.BoundingBox = itemBoundingBox.BoundingBox;
                }
                else if (itemBoundingSphere != null)
                {
                    i.BoundingBox = BoundingBox.CreateFromSphere(itemBoundingSphere.BoundingSphere);
                }
                else
                {
                    i.BoundingBox = new BoundingBox(-Vector3.One, Vector3.One);
                }
                if (subdivide)
                {
                    abt.Add(i);
                }
                else
                {
                    abt.AddWithoutSubdivide(i);
                }
                ABTMap.Add(item, i);
            }
        }

        // SCENE ITEM REMOVED
        void sceneItemRemoved(object sender, Scene.SceneManipulationEventArgs e)
        {
            // Update sceneABT
            if (ABTMap.ContainsKey(e.Item))
            {
                IABTItem item = ABTMap[e.Item];
                ABTMap.Remove(e.Item);
                CustomBoundingBoxABTItem i = item as CustomBoundingBoxABTItem;                
                if (item != null)
                {
                    abt.Remove(item);
                    if (i != null)
                    {
                        i.PositionPart = null;
                    }
                }
            }
        }
    }
}
