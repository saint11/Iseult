using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using Monocle;

namespace Iseult.Castle
{
    public class Mist:PlatformLevelEntity
    {
        private TiledImage Image;
        private float baseX = 0;
        private float baseY = 0;

        public Mist(int layerIndex)
            :base(layerIndex)
        {
            
        }
        public override void Added()
        {
            base.Added();
            Image = new TiledImage(IseultGame.Atlas["environment/mist"],
                (int)(Scene.Camera.Viewport.Width),
                (int)(Scene.Camera.Viewport.Height));

            Add(Image);
            
        }

        public override void Step()
        {
            base.Step();
            Image.OffsetX = (int)(Scene.Camera.Position.X * 1.15f) + (int)baseX;
            Image.OffsetY = (int)(Scene.Camera.Position.Y * 1.15f) + (int)baseY;

            baseX += -.2f;
            baseY =(float)Math.Sin(baseX*0.05) * 40;
        }
    }
}
