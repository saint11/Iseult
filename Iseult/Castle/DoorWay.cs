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
        
        private Sprite<string> Image;
        private bool interacting=false;

        public DoorWay(Vector2 Position, string Destiny, bool Back)
            : base(Back? GameLevel.GAMEPLAY_LAYER:GameLevel.FRONT_GAMEPLAY_LAYER)
        {
            this.Position = Position;
            this.Back = Back;
            this.Destiny = Destiny;

            Tag(GameTags.Door);

            Image = IseultGame.SpriteData.GetSpriteString("door");
            Add(Image);
            Image.Color.A = (byte)((0.7f) * 255);

            Collider = new Hitbox(Image.Width, Image.Height);
            interacting = false;
        }

        public override void Update()
        {
            base.Update();
            interacting = false;
        }

        internal void OnTouched(Iseult Player)
        {
            if (!interacting)
            {
                Tween.Alpha(Image, 0.3f, 25, null);
                interacting = true;
            }
        }
    }
}
