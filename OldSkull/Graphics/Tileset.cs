using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.Graphics
{
    class Tileset: Entity
    {
        Tilemap image;
        string csv;
        Subtexture texture;
        Vector2 tileSize;

        public Tileset(int layerIndex,string csv,Subtexture texture,Vector2 tileSize)
            : base(layerIndex)
        {
            this.csv = csv;
            this.texture = texture;
            this.tileSize = tileSize;
        }

        public override void Added()
        {
            base.Added();
            GameLevel.PlatformerLevel Level = (GameLevel.PlatformerLevel)Scene;
            image = new Tilemap(Level.Width, Level.Height);

            image.SetTileset(texture, (int)tileSize.X, (int)tileSize.Y);

            image.BeginTiling();
            image.LoadCSV(csv);
            image.EndTiling();
            Add(image);
        }

    }
}
