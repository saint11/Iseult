using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull;
using OldSkull.GameLevel;
using Monocle;
using Microsoft.Xna.Framework;

namespace Iseult
{
    public class Hud: PlatformLevelEntity
    {
        public Hud()
            : base(GameLevel.HUD_LAYER)
        {
            Add(new Image(IseultGame.Atlas["hud/hudBg"]));
            Position = new Vector2(10);
        }
    }
}
