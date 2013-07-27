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
        public Collectible(Vector2 Position,string ImageName)
            : base(Position, new Vector2(32, 32))
        {
            image = IseultGame.SpriteData.GetSpriteString("item");
            ((Sprite<string>)image).Play(ImageName);
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
            IseultGame.Stats.AddStats("knife", 1);
        }
    }
}
