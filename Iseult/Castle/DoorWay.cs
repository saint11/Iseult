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
        
        protected Sprite<string> Image;
        private int uid;
        private int interacting = 0;

        public DoorWay(Vector2 Position, bool Back, int uid, string Destiny="")
            : base(Back? GameLevel.GAMEPLAY_LAYER:GameLevel.FRONT_GAMEPLAY_LAYER)
        {
            this.Position = Position;
            this.Back = Back;
            this.Destiny = Destiny;
            this.uid = uid;

            Tag(GameTags.Door);

            interacting = 0;
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
            PlatformerLevelLoader loader = PlatformerLevelLoader.load(Destiny);
            GameLevel level = new GameLevel(loader, PlatformerLevel.Side.Door, uid);

            Engine.Instance.Scene = level;
        }
    }
}
