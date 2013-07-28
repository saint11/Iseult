using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using Monocle;

namespace Iseult
{
    public class Collectible:PlatformLevelEntity
    {
        private int Selected=0;
        public string ItemName { get; private set; }
        private Sprite<string> image;

        public Collectible(Vector2 Position, string ItemName)
            : base(0)
        {
            this.ItemName = ItemName;
            this.Position=Position;
            Collider = new Hitbox(32, 32);
            image = IseultGame.SpriteData.GetSpriteString("item");
            ((Sprite<string>)image).Play(ItemName);
            Add(image);

            Tag(GameTags.Item);
        }

        public override void Update()
        {
            base.Update();
            Selected--;
            if (Selected == 0) Tween.Scale(image, new Vector2(1, 1), 15, Ease.BackOut);
        }

        internal void OnTouched(IseultPlayer iseultPlayer)
        {
            if (Selected<=0)
            {
                Tween.Scale(image, new Vector2(1.2f, 1.2f), 15, Ease.BackOut);
            }
            Selected = 2;
        }

        public void onPickUp()
        {
            RemoveSelf();
            switch (ItemName)
            {
                case "wood":
                    IseultGame.Stats.AddStats("materials", 1);
                    break;
                default:
                    IseultGame.Stats.AddStats(ItemName, 1);
                    break;
            }
        }

        internal void onDrop(Vector2 Position, Scene Scene)
        {
            uint dropCount = 0;
            switch (ItemName)
            {
                case "wood":
                    dropCount = (uint)IseultGame.Stats.GetStats("materials");
                    IseultGame.Stats.SetStats("materials", 0);
                    break;
                default:
                    dropCount = (uint)IseultGame.Stats.GetStats(ItemName);
                    IseultGame.Stats.SetStats(ItemName, 0);
                    break;
            }

            for (int i = 0; i < dropCount; i++)
            {
                Scene.Add(new Collectible(Position, ItemName));
            }
        }
    }
}
