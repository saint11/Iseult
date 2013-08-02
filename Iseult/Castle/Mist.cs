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
        private float CameraMult = 1.2f;
        private int TileWidth;
        private int TileHeight;

        public Mist(int layerIndex)
            :base(layerIndex)
        {
            
        }
        public override void Added()
        {
            base.Added();
            TileWidth = IseultGame.Atlas["environment/mist"].Width;
            TileHeight = IseultGame.Atlas["environment/mist"].Height;
            Image = new TiledImage(IseultGame.Atlas["environment/mist"],
                (int)(Scene.Camera.Viewport.Width * CameraMult + TileWidth / 2),
                (int)(Scene.Camera.Viewport.Height * CameraMult + TileHeight));

            Add(Image);
            X = Scene.Camera.X - TileWidth / 2;
            Y = Scene.Camera.Y - TileHeight;
        }

        public override void Step()
        {
            base.Step();
            Image.OffsetX = (int)(Scene.Camera.Position.X * CameraMult) + (int)baseX;

            Image.OffsetY = (int)(Scene.Camera.Position.Y * CameraMult) + (int)(Math.Sin(baseY * 0.05) * 40);

            baseX += -0.2f;
            if (baseX < -TileWidth)
                baseX = baseX + TileWidth;

            baseY += .05f;
        }
    }
}
