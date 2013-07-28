using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using Monocle;

namespace Iseult
{
    public class Collectible:PlatformerObject
    {
        private int Selected=0;
        private string ItemName;

        public Collectible(Vector2 Position, string ItemName)
            : base(Position, new Vector2(32, 32))
        {
            this.ItemName = ItemName;
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
    }
}
