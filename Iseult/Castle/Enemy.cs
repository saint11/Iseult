using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Monocle;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using OldSkull;

namespace Iseult
{
    public class Enemy:PlatformerObject
    {
        public Enemy(Vector2 Position)
            : base(Position, new Vector2(48, 90))
        {
            image = IseultGame.SpriteData.GetSpriteString("fanatic");
            Add(image);
        }
    }
}
