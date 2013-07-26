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
        private int uid;
        private int interacting = 0;

        public DoorWay(Vector2 Position, string Destiny, bool Back, int uid)
            : base(Back? GameLevel.GAMEPLAY_LAYER:GameLevel.FRONT_GAMEPLAY_LAYER)
        {
            this.Position = Position;
            this.Back = Back;
            this.Destiny = Destiny;
            this.uid = uid;

            Tag(GameTags.Door);

            Image = IseultGame.SpriteData.GetSpriteString("door");
            Add(Image);
            Image.Color.A = (byte)((0.8f) * 255);

            Collider = new Hitbox(Image.Width, Image.Height);
            interacting = 0;
        }

        public override void Update()
        {
            base.Update();

            if (interacting == 0)
            {
                Tween.Alpha(Image, 0.8f, 25, null);
                interacting--;
            }
            else
            {
                interacting--;
            }
        }

        internal void OnTouched(Iseult Player)
        {
            if (interacting<=0)
            {
                Tween.Alpha(Image, 0.3f, 25, null);
                interacting = 5;
            }
        }

        internal void Enter()
        {
            PlatformerLevelLoader loader = PlatformerLevelLoader.load(Destiny);
            GameLevel level = new GameLevel(loader, PlatformerLevel.Side.Door, uid);

            Engine.Instance.Scene = level;
        }
    }
}
