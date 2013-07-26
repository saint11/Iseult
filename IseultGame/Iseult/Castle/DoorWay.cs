using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using Monocle;
using OldSkull;

namespace Iseult
{
    public class DoorWay:PlatformLevelEntity
    {
        private string Destiny;
        private bool Back;
        
        private Rectangle Body;
        private Sprite<string> Image;
        private bool interacting=false;

        public DoorWay(Vector2 Position, string Destiny, bool Back)
            : base(Back? GameLevel.GAMEPLAY_LAYER:GameLevel.FRONT_GAMEPLAY_LAYER)
        {
            this.Position = Position;
            this.Back = Back;
            this.Destiny = Destiny;

            Image = IseultGame.SpriteData.GetSpriteString("door");
            Add(Image);
            Image.Color.A = (byte)((0.9f) * 255);

            Body = new Rectangle((int)X, (int)Y, (int)Image.Width, (int)Image.Height);
        }

        public override void Update()
        {
            base.Update();
            
        }

        internal void TouchDoor(Iseult Player)
        {
            if (!interacting)
                {
                    Tween.Alpha(Image, 0.4f, 25, null);
                    interacting = true;
                }
            }
        }
    }
}
