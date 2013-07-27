using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OldSkull.GameLevel;
using OldSkull;
using Monocle;

namespace Iseult.Castle
{
    class Throwable:PlatformLevelEntity
    {
        private int side;
        private string ItemName;

        public Throwable(Vector2 Position, int side, string ItemName)
            :base(GameLevel.GAMEPLAY_LAYER)
        {
            this.Position = Position;
            this.side = side;
            this.ItemName = ItemName;
            Sprite<string> Image = OldSkullGame.SpriteData.GetSpriteString("throwable");
            Image.Play(ItemName); Image.Effects =
                 Image.Effects = side == -1 ? Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally :
                 Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
            Add(Image);
        }

        public override void Update()
        {
            base.Update();
            X += 12*side;
        }
    }
}
