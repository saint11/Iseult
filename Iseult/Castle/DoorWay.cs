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
        public string Destiny { get; private set; }
        private bool Back;
        
        protected Sprite<string> Image;
        private int uid;
        private int interacting = 0;
        private int hp = 10;

        public DoorWay(Vector2 Position, bool Back, int uid, string Destiny="")
            : base(Back? GameLevel.GAMEPLAY_LAYER:GameLevel.FRONT_GAMEPLAY_LAYER)
        {
            this.Position = Position;
            this.Back = Back;
            this.Destiny = Destiny;
            this.uid = uid;

            Tag(GameTags.Door);

            interacting = 0;

            if (IseultGame.Stats.HasTrigger("Door" + uid)) hp = 0;
            else hp = (int)IseultGame.Stats.GetStats("Door" + uid);
        }
        public override void Added()
        {
            base.Added();
            LoadImage();
            Image.Color.A = (byte)((0.8f) * 255);

            Collider = new Hitbox(Image.Width, Image.Height);
        }
        protected virtual void LoadImage()
        {
            Image = IseultGame.SpriteData.GetSpriteString("door");
            Add(Image);
        }

        public override void Update()
        {
            base.Update();

            if (interacting == 0)
            {
                if (!Back) Tween.Alpha(Image, 0.8f, 25, null);
                interacting--;
            }
            else
            {
                interacting--;
            }
        }

        internal void OnTouched(IseultPlayer Player)
        {
            if (interacting<=0)
            {
                if (!Back) Tween.Alpha(Image, 0.3f, 25, null);
                interacting = 5;
            }
        }

        public virtual void Enter(PlatformLevelEntity Entity)
        {
            ((GameLevel)Scene).GoToLevel(Destiny,uid);
        }
    }
}
