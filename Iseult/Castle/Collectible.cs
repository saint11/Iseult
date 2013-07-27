using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using Monocle;

namespace Iseult
{
    public class Equip:PlatformerObject
    {
        public Equip(Vector2 Position,string ImageName)
            : base(Position, new Vector2(32, 32))
        {
            image = IseultGame.SpriteData.GetSpriteString("item");
            ((Sprite<string>)image).Play(ImageName);
            Add(image);
        }

    }
}
