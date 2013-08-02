using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OldSkull.GameLevel
{
    class SolidGrid : Entity
    {
        public SolidGrid(Grid grid)
            :base(-3)
        {
            Collider = grid;
            Tag(GameTags.Solid);
        }

        public override void Render()
        {
            base.Render();
#if DEBUG
            Collider.Render(Color.Black);
#endif
        }

    }
}
