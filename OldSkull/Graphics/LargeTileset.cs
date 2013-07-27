using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.Graphics
{
    class LargeTileset: Entity
    {
        Tilemap[,] image;
        string csv;
        Subtexture texture;
        Vector2 tileSize;

        public LargeTileset(int layerIndex,string csv,Subtexture texture,Vector2 tileSize)
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

            int Width = (int)Level.Width;
            int Height = (int)Level.Height;

            Vector2 tilemapSize = new Vector2(1);
            for (int x = Width; x > 0; x -= 2048)
            {
                tilemapSize.X++;
                for (int y = Height; y > 0; y -= 2048)
                {
                    tilemapSize.Y++;
                }
            }
            image = new Tilemap[(int)tilemapSize.X,(int)tilemapSize.Y];
            for (int x = Width; x > 0; x -= 2048)
            {
                for (int y = Height; y > 0; y -= 2048)
                {
                    int XPos = (int)(x/2048);
                    int YPos = (int)(y/2048);
                    image[XPos,YPos] = new Tilemap(Math.Min(x, 2048), Math.Min(y, 2048));
                    image[XPos,YPos].SetTileset(texture, 32, 32);
                    image[XPos, YPos].X = XPos * 2048;
                    image[XPos, YPos].Y = YPos * 2048;
                    //image[XPos,YPos].BeginTiling();
                    //currentTile.LoadCSV(Trim(csv,(int)(x / 2048),(int)(y / 2048)));
                    //currentTile.EndTiling();
                    Add(image[XPos,YPos]);
                }
            }
            CreateTiles(csv);
        }

        private void CreateTiles(string data)
        {
            char[] find = new char[] { ',', '\n' };

            int x = 0;
            int y = 0;

            Tilemap cur = image[0,0];
            Vector2 LastPosition = new Vector2(-1);

            while (data.Length > 0)
            {
                int currentTile;
                char found = ' ';

                int at = data.IndexOfAny(find);
                if (at == -1)
                {
                    currentTile = Convert.ToInt32(data);
                    data = "";
                }
                else
                {
                    currentTile = Convert.ToInt32(data.Substring(0, at));
                    found = data[at];
                    data = data.Substring(at + 1);
                }

                if (currentTile != -1)
                {
                    Vector2 Position = new Vector2((int)((x * 32) / 2048), (int)((y * 32) / 2048));
                    cur = image[(int)Position.X, (int)Position.Y];

                    if (Position.X != LastPosition.X || Position.Y != LastPosition.Y)
                    {
                        if (LastPosition.X >= 0 && LastPosition.Y >= 0) cur.EndTiling();
                        LastPosition = Position;
                        cur = image[(int)Position.X, (int)Position.Y];
                        cur.BeginTiling();
                    }

                    cur.DrawTile(currentTile, x * cur.tileRects[0].Width, y * cur.tileRects[0].Height);
                    //image[1, 1].DrawTile(currentTile, x * tileRects[0].Width, y * tileRects[0].Height);
                }

                if (found == ',')
                    x++;
                else if (found == '\n')
                {
                    x = 0;
                    y++;
                }
            }
            cur.EndTiling();
        }

    }
}
