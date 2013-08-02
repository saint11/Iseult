using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using Monocle;
using Microsoft.Xna.Framework.Graphics;

namespace Iseult.Castle
{
    public class BlockBlocker : Solid
    {
        private bool flip;

        public BlockBlocker(Vector2 Position, bool flip)
            : base((int)Position.X, (int)Position.Y, 16, 16)
        {
            this.Position  = Position;
            this.flip = flip;

            Tags = new List<GameTags>();
            Tag(GameTags.BlockSolid);

            Image image = new Image(IseultGame.Atlas["environment/blockBlocker"]);
            image.Effects = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Add(image);
        }
    }
}
