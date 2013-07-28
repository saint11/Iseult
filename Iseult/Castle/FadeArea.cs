using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;
using OldSkull;
using Iseult.Castle;

namespace Iseult
{
    public class FadeArea:PlatformLevelEntity
    {
        Image image;
        private bool Fading;
        private string uid;

        public FadeArea(Vector2 Position, Vector2 Size, string uid)
            : base(GameLevel.FRONT_GAMEPLAY_LAYER)
        {
            this.Position = Position;
            this.uid = uid;

            Collider = new Hitbox(Size.X,Size.Y);

            image = new Image(new Texture((int)Size.X, (int)Size.Y, Color.Black));
            image.Color.A = 245;
            Add(image);
        }

        public override void Update()
        {
            base.Update();
            
            if (Level.CollideCheck(Collider.Bounds,GameTags.Player))
            {
                if (!Fading)
                {
                    Fading = true;
                    Tween.Alpha(image, 0, 40, Ease.CubeIn).OnComplete = (t) =>
                        {
                            IseultGame.Stats.SetTrigger(uid);
                            RemoveSelf();
                        };
                }
            }
        }
    }
}
